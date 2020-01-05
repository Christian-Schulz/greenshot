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


using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.Factory;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.Log;
using Greenshot.Addon.Redmine.Api.Types;
using Greenshot.Addon.Redmine.Configuration;
using Greenshot.Addons.Core;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace Greenshot.Addon.Redmine.Api
{
    class RedmineConnector
    {
        private static readonly LogSource Log = new LogSource();
        private readonly IRedmineConfiguration _redmineConfiguration;

        private HttpBehaviour Behaviour { get; }

        public RedmineConnector(
            IHttpConfiguration httpConfiguration,
            IRedmineConfiguration redmineConfiguration)
        {
            _redmineConfiguration = redmineConfiguration;

            Behaviour = new HttpBehaviour
            {
                HttpSettings = httpConfiguration,
                JsonSerializer = new JsonNetJsonSerializer(),
                OnHttpClientCreated = httpClient =>
                {
                    httpClient.SetAuthorization("X-Redmine-API-Key", _redmineConfiguration.APIToken);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                }
            };
        }

        public async Task<User> GetCurrentUser(CancellationToken token = default)
        {
            var imageCurrentUserUri = new Uri(_redmineConfiguration.Url + "/users/current.xml");
            Log.Debug().WriteLine("Get current user whith url {1}", imageCurrentUserUri);

            Behaviour.MakeCurrent();
            using var client = HttpClientFactory.Create(imageCurrentUserUri);

            var response = await client.GetAsync(imageCurrentUserUri, token).ConfigureAwait(false);
            await response.HandleErrorAsync().ConfigureAwait(false);
            return await response.GetAsAsync<User>(token).ConfigureAwait(false);

        }
    }
}
