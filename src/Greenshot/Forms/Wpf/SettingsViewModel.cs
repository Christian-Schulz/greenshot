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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Greenshot.Base;
using Greenshot.Base.Core;
using Greenshot.Base.Core.Enums;
using Greenshot.Base.IniFile;
using Greenshot.Base.Interfaces;
using Greenshot.Configuration;
using Greenshot.Destinations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Greenshot.Forms.Wpf
{
    /// <summary>
    /// ViewModel for the WPF Settings Window
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _expertModeEnabled;

        [ObservableProperty]
        private bool _autoStartEnabled;

        [ObservableProperty]
        private bool _pickerSelected;

        [ObservableProperty]
        private string _selectedLanguage;

        public SettingsViewModel()
        {
            CoreConfiguration = IniConfig.GetIniSection<CoreConfiguration>();
            _expertModeEnabled = !CoreConfiguration.HideExpertSettings;
            // AutoStart is handled separately from INI config
            _autoStartEnabled = false;

            // Initialize language
            _selectedLanguage = Language.CurrentLanguage;

            // Initialize image formats
            InitializeImageFormats();

            // Initialize destinations
            InitializeDestinations();
        }

        public CoreConfiguration CoreConfiguration { get; }

        public IList<LanguageFile> SupportedLanguages => Language.SupportedLanguages;

        partial void OnSelectedLanguageChanged(string value)
        {
            Language.CurrentLanguage = value;
            CoreConfiguration.Language = value;
        }

        public List<ImageFormatItem> ImageFormats { get; private set; }

        public ObservableCollection<DestinationItem> Destinations { get; private set; }

        partial void OnPickerSelectedChanged(bool value)
        {
            // When picker is selected, deselect all destinations
            if (value && Destinations != null)
            {
                foreach (var dest in Destinations)
                {
                    dest.IsSelected = false;
                }
            }
        }

        private void InitializeImageFormats()
        {
            ImageFormats = ((IEnumerable<OutputFormat>)System.Enum.GetValues(typeof(OutputFormat)))
                .Select(format => new ImageFormatItem
                {
                    Value = format,
                    Description = Language.Translate(format)
                })
                .ToList();
        }

        private void InitializeDestinations()
        {
            Destinations = new ObservableCollection<DestinationItem>();

            foreach (IDestination destination in DestinationHelper.GetAllDestinations())
            {
                // Skip picker - it's handled separately
                if (nameof(WellKnownDestinations.Picker).Equals(destination.Designation))
                {
                    PickerSelected = CoreConfiguration.OutputDestinations.Contains(destination.Designation);
                    continue;
                }

                var destItem = new DestinationItem
                {
                    Destination = destination,
                    Description = destination.Description,
                    IsSelected = CoreConfiguration.OutputDestinations.Contains(destination.Designation)
                };

                Destinations.Add(destItem);
            }
        }

        // New: move SaveSettings logic into the ViewModel so the View becomes thinner
        public void SaveSettings()
        {
            var destinations = new List<string>();

            if (PickerSelected)
            {
                destinations.Add(nameof(WellKnownDestinations.Picker));
            }
            else if (Destinations != null)
            {
                foreach (var destItem in Destinations.Where(d => d.IsSelected))
                {
                    destinations.Add(destItem.Destination.Designation);
                }
            }

            CoreConfiguration.OutputDestinations = destinations;

            // Force save of all configuration sections
            IniConfig.Save();
        }
    }

    public class ImageFormatItem
    {
        public OutputFormat Value { get; set; }
        public string Description { get; set; }
    }

    public partial class DestinationItem : ObservableObject
    {
        public IDestination Destination { get; set; }
        public string Description { get; set; }

        [ObservableProperty]
        private bool _isSelected;
    }
}
