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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using Greenshot.Base.IniFile;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Plugin;
using Greenshot.Editor.FileFormatHandlers;
using Greenshot.Plugin.Pdf.Configuration;

namespace Greenshot.Plugin.Pdf
{
    internal class PdfFileFormatHandler : AbstractFileFormatHandler, IFileFormatHandler
    {
        private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(typeof(PdfFileFormatHandler));
        private readonly IReadOnlyCollection<string> _ourExtensions = new[] { ".pdf" };
        private PdfExportSettings _settings;

        public PdfFileFormatHandler()
        {
            SupportedExtensions[FileFormatHandlerActions.SaveToStream] = _ourExtensions;
            SupportedExtensions[FileFormatHandlerActions.SaveToFile] = _ourExtensions;
            _settings = IniConfig.GetIniSection<PdfExportSettings>();
        }

        /// <summary>
        /// Set the export settings for PDF generation
        /// </summary>
        public void SetSettings(PdfExportSettings settings)
        {
            _settings = settings ?? new PdfExportSettings();
        }

        public override bool TryLoadFromStream(Stream stream, string extension, out Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public override bool TrySaveToStream(Bitmap bitmap, Stream destination, string extension, ISurface surface = null, SurfaceOutputSettings surfaceOutputSettings = null)
        {
            return TrySaveToPdf(bitmap, destination);
        }

        /// <summary>
        /// Saves bitmap to PDF with configured settings
        /// </summary>
        private bool TrySaveToPdf(Bitmap bitmap, Stream destination)
        {
            try
            {
                // 1. use JPEG because pdf supports it natively
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] jpegData = ms.ToArray();

                // 2. calculate page and image dimensions
                double pageWidthMm = _settings.PageWidth;
                double pageHeightMm = _settings.PageHeight;

                // Calculate available space for image (page minus margins)
                double availableWidthMm = pageWidthMm - _settings.MarginLeft - _settings.MarginRight;
                double availableHeightMm = pageHeightMm - _settings.MarginTop - _settings.MarginBottom;

                // Get image aspect ratio
                double imageAspectRatio = (double)bitmap.Height / bitmap.Width;

                // Determine image dimensions based on FitToPage setting
                double imageWidthMm;
                double imageHeightMm;

                if (_settings.FitToPage)
                {
                    // FitToPage ON: Scale image to fit maximum available space
                    imageWidthMm = availableWidthMm;
                    imageHeightMm = availableWidthMm * imageAspectRatio;

                    // If height exceeds available space, scale by height instead
                    if (imageHeightMm > availableHeightMm)
                    {
                        imageHeightMm = availableHeightMm;
                        imageWidthMm = availableHeightMm / imageAspectRatio;
                    }
                }
                else
                {
                    // FitToPage OFF: Image at 100% size (assume 96 DPI)
                    const double dpi = 96.0;
                    const double mmPerPixel = 25.4 / dpi;
                    
                    imageWidthMm = bitmap.Width * mmPerPixel;
                    imageHeightMm = bitmap.Height * mmPerPixel;
                }

                // Apply RetainRatio: Trim extra space by adjusting page height to content
                if (_settings.RetainRatio)
                {
                    double calculatedHeightMm = imageHeightMm + _settings.MarginTop + _settings.MarginBottom;
                    pageHeightMm = calculatedHeightMm;
                    availableHeightMm = pageHeightMm - _settings.MarginTop - _settings.MarginBottom;
                }

                // Calculate centering offsets
                double centerOffsetXMm = (availableWidthMm - imageWidthMm) / 2.0;
                double centerOffsetYMm = (availableHeightMm - imageHeightMm) / 2.0;
                
                // Ensure offsets are not negative (image at minimum position)
                double contentLeftMm = _settings.MarginLeft + (centerOffsetXMm > 0 ? centerOffsetXMm : 0);
                double contentTopMm = _settings.MarginTop + (centerOffsetYMm > 0 ? centerOffsetYMm : 0);

                // Convert mm to points (72 points = 1 inch = 25.4 mm)
                double mmToPt = 72.0 / 25.4;
                double pageWidthPt = pageWidthMm * mmToPt;
                double pageHeightPt = pageHeightMm * mmToPt;
                double contentLeftPt = contentLeftMm * mmToPt;
                double contentTopPt = (pageHeightMm - contentTopMm - imageHeightMm) * mmToPt;
                double imageWidthPt = imageWidthMm * mmToPt;
                double imageHeightPt = imageHeightMm * mmToPt;

                // 3. PDF structure
                using var writer = new StreamWriter(destination, Encoding.ASCII, 1024, leaveOpen: true);
                string F(double value) => value.ToString("F2", CultureInfo.InvariantCulture);

                // Header
                writer.Write("%PDF-1.4\n");
                writer.Flush();

                // Objects
                long obj1Offset = destination.Position;
                writer.Write("1 0 obj\n<</Type/Catalog/Pages 2 0 R>>\nendobj\n");
                writer.Flush();

                long obj2Offset = destination.Position;
                writer.Write("2 0 obj\n<</Type/Pages/Kids[3 0 R]/Count 1>>\nendobj\n");
                writer.Flush();

                long obj3Offset = destination.Position;
                writer.Write($"3 0 obj\n<</Type/Page/Parent 2 0 R/MediaBox[0 0 {F(pageWidthPt)} {F(pageHeightPt)}]/Resources<</XObject<</Img1 4 0 R>>>>/Contents 5 0 R>>\n");
                writer.Write("endobj\n");
                writer.Flush();

                long obj4Offset = destination.Position;
                writer.Write($"4 0 obj\n<</Type/XObject/Subtype/Image/Width {bitmap.Width}/Height {bitmap.Height}/ColorSpace/DeviceRGB/BitsPerComponent 8/Filter/DCTDecode/Length {jpegData.Length}>>\nstream\n");
                writer.Flush();
                destination.Write(jpegData, 0, jpegData.Length);
                writer.Write("\nendstream\nendobj\n");
                writer.Flush();

                long obj5Offset = destination.Position;
                string content = $"q {F(imageWidthPt)} 0 0 {F(imageHeightPt)} {F(contentLeftPt)} {F(contentTopPt)} cm /Img1 Do Q";
                writer.Write($"5 0 obj\n<</Length {content.Length}>>\nstream\n{content}\nendstream\nendobj\n");
                writer.Flush();

                // Cross-Reference Table
                long xrefOffset = destination.Position;
                writer.Write("xref\n0 6\n0000000000 65535 f \n");
                writer.Write($"{obj1Offset:0000000000} 00000 n \n");
                writer.Write($"{obj2Offset:0000000000} 00000 n \n");
                writer.Write($"{obj3Offset:0000000000} 00000 n \n");
                writer.Write($"{obj4Offset:0000000000} 00000 n \n");
                writer.Write($"{obj5Offset:0000000000} 00000 n \n");

                // Trailer
                writer.Write($"trailer\n<</Size 6/Root 1 0 R>>\nstartxref\n{xrefOffset}\n%%EOF");
                writer.Flush();

                return true;
            }
            catch (Exception ex)
            {
                LOG.Error("Error saving PDF: ", ex);
                return false;
            }
        }
    }
}
