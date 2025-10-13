/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2021 Thomas Braun, Jens Klingen, Robin Krom
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Greenshot.Base.Core
{
    /// <summary>
    /// Extension methods which work for objects
    /// </summary>
    public static class ObjectExtensions
    {       
        /// <summary>
        /// Compare two lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l1">IList</param>
        /// <param name="l2">IList</param>
        /// <returns>true if they are the same</returns>
        public static bool CompareLists<T>(IList<T> l1, IList<T> l2)
        {
            if (l1.Count != l2.Count)
            {
                return false;
            }

            int matched = 0;
            foreach (T item in l1)
            {
                if (!l2.Contains(item))
                {
                    return false;
                }

                matched++;
            }

            return matched == l1.Count;
        }
    }
}