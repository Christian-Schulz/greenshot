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

using Newtonsoft.Json;

namespace Greenshot.Addon.Redmine.Api.Types
{
    /// <summary>
    /// Main fields of an user object (incomplete)
    /// <see href=http://www.redmine.org/projects/redmine/wiki/Rest_Users">Redmine API - User</see>
    /// </summary>
    [JsonObject("user")]
    public class User
    {
        /// <summary>
        /// User ID
        /// </summary>
        [JsonProperty("id")]
        public int Id;

        /// <summary>
        ///  First name
        /// </summary>        
        [JsonProperty("firstname")]
        public string FirstName;

        /// <summary>
        /// Last name
        /// </summary>
        [JsonProperty("lastname")]
        public string LastName;

        /// <summary>
        /// EMail
        /// </summary>
        [JsonProperty("mail")]
        public string Mail;

        /// <summary>
        /// API-Key (you only get your own key)
        /// </summary>
        [JsonProperty("api_key")]
        public string ApiKey;

    }

    [JsonObject("currentuser")]
    public class CurrentUser
    {

        [JsonProperty("user")]
        public User user;
    }
}