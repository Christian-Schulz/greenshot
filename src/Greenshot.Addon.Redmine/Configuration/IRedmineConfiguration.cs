// Greenshot - a free and open source screenshot tool
// Copyright (C) 2007-2020 Thomas Braun, Jens Klingen, Robin Krom
// 
// For more information see: http://getgreenshot.org/
// The Greenshot project is hosted on GitHub https://github.com/greenshot/greenshot
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 1 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;
using System.Runtime.Serialization;
using Dapplo.Config.Ini;
using Greenshot.Addons.Core;

namespace Greenshot.Addon.Redmine.Configuration
{
    /// <summary>
    /// Configuration for Redmine Addon
    /// </summary>
    [IniSection("Redmine")]
    [Description("Greenshot Redmine Addon configuration")]
    public interface IRedmineConfiguration : IIniSection, IDestinationFileConfiguration
    {
        [Description("Base URL to Remine Server")]
        //TODO: change after test [DefaultValue("https://redmine")]
        [DefaultValue("https://192.168.178.159")]
        string Url { get; set; }

        //TODO: change after test [DefaultValue("@user_API_token@")]
        [DefaultValue("aa0c98be0bbb6ed6d52a35cb704bae23b15b9990")]
        [DataMember(EmitDefaultValue = false)]
        string APIToken { get; set; }

    }
}
