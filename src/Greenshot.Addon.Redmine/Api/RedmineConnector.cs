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

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.Factory;
//TODO use System.Text.Json (Dapplo.HttpExtensions.SystemTextJson)
//using System.Text.Json;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.Log;
using Greenshot.Addon.Redmine.Api.Types;
using Greenshot.Addon.Redmine.Configuration;
using Greenshot.Addons.Core;

namespace Greenshot.Addon.Redmine.Api
{
    public class RedmineConnector
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
                ThrowOnError = false, //TODO: überlege, ob wirklich notwendig
                HttpSettings = httpConfiguration,
                JsonSerializer = new JsonNetJsonSerializer(),
                //TODO use System.Text.Json (Dapplo.HttpExtensions.SystemTextJson)
                //JsonSerializer = new SystemTextJsonSerializer(),
                OnHttpClientCreated = httpClient =>
                {
                    httpClient.AddDefaultRequestHeader("X-Redmine-API-Key", _redmineConfiguration.APIToken);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                }
            };
        }

        public async Task<CurrentUser> GetCurrentUser(CancellationToken token = default)
        {
            var currentUserUri = new Uri(_redmineConfiguration.Url)
                                        .AppendSegments("users")
                                        .AppendSegments("current.json");
            Log.Debug().WriteLine($"Get current user whith url {currentUserUri.ToString()}");

            Behaviour.MakeCurrent();
            using var client = HttpClientFactory.Create(currentUserUri);
            try
            {
                var response = await client.GetAsync(currentUserUri, token);
                await response.HandleErrorAsync().ConfigureAwait(false);
                // var test = await response.GetAsAsync<dynamic>(token);

                //User ustest = new User(){ FirstName = "hallo", LastName = "momo" };

                //string output = JsonConvert.SerializeObject(ustest);

                //User ustest2 = JsonConvert.DeserializeObject<User>(output);

                //var test2 = await response.GetAsAsync<string>(token);
                //CurrentUser us = JsonConvert.DeserializeObject<CurrentUser>(test2);

                //TODO: CurrentUser durch dynamic ersetzen und local User-Objekt daraus machen

                return await response.GetAsAsync<CurrentUser>(token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}