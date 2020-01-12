//  Greenshot - a free and open source screenshot tool
//  Copyright (C) 2007-2020 Thomas Braun, Jens Klingen, Robin Krom
// 
//  For more information see: http://getgreenshot.org/
//  The Greenshot project is hosted on GitHub: https://github.com/greenshot
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 1 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;
using Dapplo.Config.Language;

namespace Greenshot.Addon.Redmine.Configuration
{
    /// <summary>
    /// Translations for the Redmine add-on
    /// Mapping to the language resource name via <see cref="Dapplo.Config.AbcComparer"/> (RemoveNonAlphaDigitsToLower)
    /// </summary>
    [Language("Redmine")]
	public interface IRedmineLanguage : ILanguage
	{
       
        string UploadMenuItem { get; }

        string UploadSuccess { get; }

        string UploadFailure { get; }


        [DefaultValue("Ok")]
        string Ok { get; }

        [DefaultValue("Cancel")]
        string Cancel { get; }

        [DefaultValue("Redmine settings")]
        string SettingsTitle { get; }

        [DefaultValue("Connection details")]
        string SettingsConnectionDetailsTitle { get; }

        [DefaultValue("Test connection")]
        string SettingsTestConnection { get; }

        [DefaultValue("Url")]
        string LabelUrl { get; }

        [DefaultValue("APIToken")]
        string LabelAPIToken { get; }

        string SettingsConnectedAsUser { get; }
        string SettingsConnectionValid { get; }
        string CommunicationWait { get; }

    }
}