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

using System.Reactive.Disposables;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Greenshot.Addon.Redmine.Configuration;
using Greenshot.Addons;
using Greenshot.Addons.Core.Enums;
using Greenshot.Addons.ViewModels;

namespace Greenshot.Addon.Redmine.ViewModels
{
    /// <summary>
    /// This is the view model for Redmine configuration
    /// </summary>
    public sealed class RedmineConfigViewModel : SimpleConfigScreen
    {
        /// <summary>
        ///     Here all disposables are registered, so we can clean the up
        /// </summary>
        private CompositeDisposable _disposables;

        /// <summary>
        /// Provide IRedmineConfiguration to the view
        /// </summary>
        public IRedmineConfiguration RedmineConfiguration { get; }

        /// <summary>
        /// Provide IRedmineLanguage to the view
        /// </summary>
        public IRedmineLanguage RedmineLanguage { get; }

        /// <summary>
        /// Provide IGreenshotLanguage to the view
        /// </summary>
        public IGreenshotLanguage GreenshotLanguage { get; }

        /// <summary>
        /// Provide FileConfigPartViewModel to the view
        /// </summary>
        public FileConfigPartViewModel FileConfigPartViewModel { get; }

        /// <summary>
        /// DI constructor
        /// </summary>
        /// <param name="redmineConfiguration">IRedmineConfiguration</param>
        /// <param name="redmineLanguage">IRedmineLanguage</param>
        /// <param name="fileConfigPartViewModel">FileConfigPartViewModel</param>
        public RedmineConfigViewModel(IRedmineConfiguration redmineConfiguration,
            IRedmineLanguage redmineLanguage,
            IGreenshotLanguage greenshotLanguage,
            FileConfigPartViewModel fileConfigPartViewModel
            )
        {
            RedmineConfiguration = redmineConfiguration;
            RedmineLanguage = redmineLanguage;
            GreenshotLanguage = greenshotLanguage;
            FileConfigPartViewModel = fileConfigPartViewModel;
        }

        /// <summary>
        /// default constructor for Design time
        /// </summary>
        public RedmineConfigViewModel()
        {
            if (Execute.InDesignMode)
            {
                RedmineConfiguration = IniSection<IRedmineConfiguration>.Create();
                RedmineLanguage = Language<IRedmineLanguage>.Create();
                GreenshotLanguage = Language<IGreenshotLanguage>.Create();
            }
        }

        /// <inheritdoc />
        public override void Initialize(IConfig config)
        {
            FileConfigPartViewModel.DestinationFileConfiguration = RedmineConfiguration;
            // Prepare disposables
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            // Place this under the Ui parent
            ParentId = nameof(ConfigIds.Destinations);

            // Make sure Commit/Rollback is called on the IUiConfiguration
            config.Register(RedmineConfiguration);

            // automatically update the DisplayName
            var RedmineLanguageBinding = RedmineLanguage.CreateDisplayNameBinding(this, nameof(RedmineLanguage.SettingsTitle));

            // Make sure the greenshotLanguageBinding is disposed when this is no longer active
            _disposables.Add(RedmineLanguageBinding);

            base.Initialize(config);
        }


        /// <inheritdoc />
        protected override void OnDeactivate(bool close)
        {
            _disposables.Dispose();
            base.OnDeactivate(close);
        }
    }
}
