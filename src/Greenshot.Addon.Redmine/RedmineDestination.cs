// Greenshot - a free and open source screenshot tool
// Copyright (C) 2007-2018 Thomas Braun, Jens Klingen, Robin Krom
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
using System.Windows.Forms;
using Autofac.Features.OwnedInstances;
using Dapplo.Addons;
using Dapplo.HttpExtensions;
using Dapplo.Log;
using Dapplo.Windows.Clipboard;
using Greenshot.Addon.Redmine.Configuration;
using Greenshot.Addons;
using Greenshot.Addons.Components;
using Greenshot.Addons.Controls;
using Greenshot.Addons.Core;
using Greenshot.Addons.Extensions;
using Greenshot.Addons.Interfaces;
using Greenshot.Gfx;

namespace Greenshot.Addon.Redmine
{
    /// <summary>
    ///     Description of RedmineDestination.
    /// </summary>
    [Destination("Redmine")]
    public class RedmineDestination : AbstractDestination
    {
        private static readonly LogSource Log = new LogSource();
        private readonly ExportNotification _exportNotification;
        // private readonly IRedmineConfiguration _RedmineConfiguration;
        private readonly IRedmineLanguage _RedmineLanguage;
        private readonly Func<CancellationTokenSource, Owned<PleaseWaitForm>> _pleaseWaitFormFactory;
        private readonly IResourceProvider _resourceProvider;

        public RedmineDestination(
            ICoreConfiguration coreConfiguration,
            IGreenshotLanguage greenshotLanguage,
            ExportNotification exportNotification,
            //  IRedmineConfiguration RedmineConfiguration,
            IRedmineLanguage RedmineLanguage,
            Func<CancellationTokenSource, Owned<PleaseWaitForm>> pleaseWaitFormFactory,
            IResourceProvider resourceProvider) : base(coreConfiguration, greenshotLanguage)
        {
            _exportNotification = exportNotification;
            //_RedmineConfiguration = RedmineConfiguration;
            _RedmineLanguage = RedmineLanguage;
            _pleaseWaitFormFactory = pleaseWaitFormFactory;
            _resourceProvider = resourceProvider;
        }

        public override string Description => _RedmineLanguage.UploadMenuItem;

        public override IBitmapWithNativeSupport DisplayIcon
        {
            get
            {
                // TODO: Optimize this, by caching
                using var bitmapStream = _resourceProvider.ResourceAsStream(GetType().Assembly, "Redmine.png");
                return BitmapHelper.FromStream(bitmapStream);
            }
        }

        public override async Task<ExportInformation> ExportCaptureAsync(bool manuallyInitiated, ISurface surface, ICaptureDetails captureDetails)
        {
            var uploadUrl = await Upload(captureDetails, surface).ConfigureAwait(true);

            var exportInformation = new ExportInformation(Designation, Description)
            {
                ExportMade = uploadUrl != null,
                Uri = uploadUrl?.AbsoluteUri
            };
            _exportNotification.NotifyOfExport(this, exportInformation, surface);
            return exportInformation;
        }

        /// <summary>
        /// Upload the capture to Redmine
        /// </summary>
        /// <param name="captureDetails">ICaptureDetails</param>
        /// <param name="surfaceToUpload">ISurface</param>
        /// <returns>Uri </returns>
        private async Task<Uri> Upload(ICaptureDetails captureDetails, ISurface surfaceToUpload)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            // TODO: Replace the form
            using var ownedPleaseWaitForm = _pleaseWaitFormFactory(cancellationTokenSource);

            try
            {
                ownedPleaseWaitForm.Value.SetDetails("Redmine", _RedmineLanguage.CommunicationWait);
                ownedPleaseWaitForm.Value.Show();

                // TODO: is nur Pseudo-Upload
                var uploadUri = new Uri("TODO:URL").AppendSegments("upload.json");
                var reponse = await uploadUri.PostAsync<Uri>(surfaceToUpload, cancellationTokenSource.Token).ConfigureAwait(false);
                // RedmineImage = await _RedmineApi.UploadToRedmineAsync(surfaceToUpload, captureDetails.Title, null, cancellationTokenSource.Token).ConfigureAwait(true);                  
                return reponse;
            }
            catch (Exception e)
            {
                Log.Error().WriteLine(e, "Error uploading.");
                MessageBox.Show(_RedmineLanguage.UploadFailure + " " + e.Message);
            }
            finally
            {
                ownedPleaseWaitForm.Value.Close();
            }
            return null;
        }
    }
}