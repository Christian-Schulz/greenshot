/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2004-2026 Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: https://getgreenshot.org/
 * The Greenshot project is hosted on GitHub https://github.com/greenshot/greenshot
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;
using log4net;

namespace Greenshot.Base.Interop
{
    /// <summary>
    /// COM object creation helper for .NET 10
    /// This is a simplified version that removes the RealProxy-based wrapping which is not available in modern .NET
    /// </summary>
    /// <remarks>
    /// In .NET Framework, this class used System.Runtime.Remoting.Proxies.RealProxy for method interception.
    /// That infrastructure has been removed in .NET Core/.NET 10 and is no longer needed for basic COM interop.
    /// Modern .NET handles COM interop directly through Runtime Callable Wrappers (RCW).
    /// </remarks>
    public static class COMWrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(COMWrapper));

        /// <summary>
        /// Create a COM instance for the specified interface type.
        /// The type must have a ComProgId attribute specifying the COM ProgID or CLSID.
        /// </summary>
        /// <typeparam name="T">Interface type with ComProgId attribute</typeparam>
        /// <returns>COM object instance or default(T) if creation fails</returns>
        public static T CreateInstance<T>()
        {
            Type type = typeof(T);
            if (null == type)
            {
                throw new ArgumentNullException(nameof(T));
            }

            if (!type.IsInterface)
            {
                throw new ArgumentException("The specified type must be an interface.", nameof(T));
            }

            ComProgIdAttribute progIdAttribute = ComProgIdAttribute.GetAttribute(type);
            if (string.IsNullOrEmpty(progIdAttribute?.Value))
            {
                throw new ArgumentException("The specified type must define a ComProgId attribute.", nameof(T));
            }

            string progId = progIdAttribute.Value;
            Type comType = null;
            
            if (progId.StartsWith("clsid:"))
            {
                Guid guid = new Guid(progId.Substring(6));
                try
                {
                    comType = Type.GetTypeFromCLSID(guid);
                }
                catch (Exception ex)
                {
                    Log.WarnFormat("Error getting type for CLSID {0}: {1}", progId, ex.Message);
                }
            }
            else
            {
                try
                {
                    comType = Type.GetTypeFromProgID(progId, true);
                }
                catch (Exception ex)
                {
                    Log.WarnFormat("Error getting type for ProgID {0}: {1}", progId, ex.Message);
                }
            }

            object comObject = null;
            if (comType != null)
            {
                try
                {
                    comObject = Activator.CreateInstance(comType);
                    if (comObject != null)
                    {
                        Log.DebugFormat("Created new instance of {0} COM object.", progId);
                    }
                }
                catch (Exception e)
                {
                    Log.WarnFormat("Error creating COM object for {0}: {1}", progId, e.Message);
                    throw;
                }
            }

            if (comObject != null)
            {
                // Modern .NET handles COM interop directly through RCW (Runtime Callable Wrapper)
                // No need for manual proxying - the CLR provides this automatically
                return (T)comObject;
            }

            return default;
        }

        /// <summary>
        /// Helper method to safely release a COM object
        /// </summary>
        /// <param name="comObject">The COM object to release</param>
        public static void ReleaseComObject(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                try
                {
                    int count;
                    do
                    {
                        count = Marshal.ReleaseComObject(comObject);
                    } while (count > 0);
                }
                catch (Exception ex)
                {
                    Log.Warn("Problem releasing COM object", ex);
                }
            }
        }
    }
}
