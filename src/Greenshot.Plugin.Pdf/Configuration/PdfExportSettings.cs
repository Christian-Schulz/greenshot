/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2004-2026 Thomas Braun, Jens Klingen, Robin Krom, Francis Noel
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

using Greenshot.Base.IniFile;

namespace Greenshot.Plugin.Pdf.Configuration
{
    /// <summary>
    /// PDF export settings for page layout and image scaling
    /// </summary>
    [IniSection("Pdf", Description = "Greenshot PDF Plugin configuration")]
    internal class PdfExportSettings : IniSection
    {
        /// <summary>
        /// Page width in millimeters
        /// </summary>
        [IniProperty("PageWidth", Description = "Page width in millimeters", DefaultValue = "210.0")]
        public double PageWidth { get; set; } = 210.0;

        /// <summary>
        /// Page height in millimeters
        /// </summary>
        [IniProperty("PageHeight", Description = "Page height in millimeters", DefaultValue = "297.0")]
        public double PageHeight { get; set; } = 297.0;

        /// <summary>
        /// Top margin in millimeters
        /// </summary>
        [IniProperty("MarginTop", Description = "Top margin in millimeters", DefaultValue = "10.0")]
        public double MarginTop { get; set; } = 10.0;

        /// <summary>
        /// Bottom margin in millimeters
        /// </summary>
        [IniProperty("MarginBottom", Description = "Bottom margin in millimeters", DefaultValue = "10.0")]
        public double MarginBottom { get; set; } = 10.0;

        /// <summary>
        /// Left margin in millimeters
        /// </summary>
        [IniProperty("MarginLeft", Description = "Left margin in millimeters", DefaultValue = "10.0")]
        public double MarginLeft { get; set; } = 10.0;

        /// <summary>
        /// Right margin in millimeters
        /// </summary>
        [IniProperty("MarginRight", Description = "Right margin in millimeters", DefaultValue = "10.0")]
        public double MarginRight { get; set; } = 10.0;

        /// <summary>
        /// Scale image to fit within page margins
        /// </summary>
        [IniProperty("FitToPage", Description = "Scale image to fit within page margins", DefaultValue = "true")]
        public bool FitToPage { get; set; } = true;

        /// <summary>
        /// Auto-calculate PageHeight based on screenshot aspect ratio
        /// </summary>
        [IniProperty("RetainRatio", Description = "Auto-calculate PageHeight based on screenshot aspect ratio", DefaultValue = "true")]
        public bool RetainRatio { get; set; } = true;

        /// <summary>
        /// Creates a copy of the current settings
        /// </summary>
        public PdfExportSettings Clone()
        {
            return new PdfExportSettings
            {
                PageWidth = PageWidth,
                PageHeight = PageHeight,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                MarginLeft = MarginLeft,
                MarginRight = MarginRight,
                FitToPage = FitToPage,
                RetainRatio = RetainRatio
            };
        }
    }
}
