/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2004-2026 Thomas Braun, Jens Klingen, Robin Krom
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Dapplo.Ini;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Core;
using Greenshot.Base.Interfaces;
using Greenshot.Editor.Controls;
using Greenshot.Editor.Drawing;
using Greenshot.Editor.Drawing.NewModel.Integration;
using Greenshot.Editor.Drawing.NewModel.Models;
using Greenshot.Editor.Drawing.NewModel.Renderers;
using log4net;

namespace Greenshot.Editor.Forms
{
    /// <summary>
    /// Prototype editor using the new drawing model with separated data, rendering, and state.
    /// This is a beta feature demonstrating the new architecture.
    /// </summary>
    public partial class NewModelEditorForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NewModelEditorForm));
        private static readonly ICoreConfiguration CoreConfig = IniConfigRegistry.GetSection<ICoreConfiguration>();

        private ShapeCanvas _canvas;
        private CanvasRenderer _renderer;
        private StyleManager _styleManager;
        private NewModelCanvasControl _canvasControl;

        private Layer _defaultLayer;
        private Layer _backgroundLayer;
        private Layer _activeLayer; // The currently active layer for adding new shapes

        public NewModelEditorForm()
        {
            InitializeComponent();
            InitializeNewModel();
        }

        /// <summary>
        /// Creates a new editor by copying from an existing Surface
        /// </summary>
        public static NewModelEditorForm FromSurface(ISurface surface)
        {
            var editor = new NewModelEditorForm();
            editor.LoadFromSurface(surface);
            return editor;
        }

        private void InitializeNewModel()
        {
            // Initialize the new model components
            _canvas = new ShapeCanvas();
            _renderer = new CanvasRenderer();
            _styleManager = new StyleManager();

            // Use the layers that ShapeCanvas automatically creates
            _backgroundLayer = _canvas.BackgroundLayer;
            _defaultLayer = _canvas.DefaultLayer;
            _activeLayer = _defaultLayer; // Default layer is active by default

            // Create default styles
            _styleManager.RegisterStyle("Default", new ShapeStyle(Color.Red, 2, Color.Empty, false));
            _styleManager.RegisterStyle("BlueFilled", new ShapeStyle(Color.Blue, 2, Color.LightBlue, true));
            _styleManager.RegisterStyle("GreenHighlight", new ShapeStyle(Color.Green, 3, Color.FromArgb(100, Color.GreenYellow), true));
            _styleManager.RegisterStyle("BlackText", new ShapeStyle(Color.Black, 1, Color.Empty, false));
        }

        private void LoadFromSurface(ISurface surface)
        {
            try
            {
                if (surface == null)
                {
                    Log.Warn("Cannot load from null surface");
                    return;
                }

                // Clone the background image to avoid disposing issues
                if (surface.Image != null)
                {
                    var clonedImage = CloneImage(surface.Image) as Bitmap;
                    var imageData = new BitmapImageData(clonedImage);
                    var bounds = new NativeRect(0, 0, clonedImage.Width, clonedImage.Height);
                    var backgroundShape = new BackgroundShape(bounds, imageData);
                    backgroundShape.LayerId = _backgroundLayer.Id;
                    _canvas.AddShape(backgroundShape);
                }

                // Convert existing drawables to new shapes
                if (surface.Elements != null)
                {
                    foreach (DrawableContainer container in surface.Elements)
                    {
                        var shape = IntegrationHelpers.ConvertToShape(container);
                        if (shape != null)
                        {
                            shape.LayerId = _defaultLayer.Id;
                            _canvas.AddShape(shape);
                        }
                    }
                }

                // Set form title
                if (surface.CaptureDetails != null && !string.IsNullOrEmpty(surface.CaptureDetails.Title))
                {
                    Text = $"New Model Editor - {surface.CaptureDetails.Title}";
                }

                // Refresh the canvas
                _canvasControl?.Invalidate();
            }
            catch (Exception ex)
            {
                Log.Error("Error loading from surface", ex);
                MessageBox.Show($"Error loading surface: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Image CloneImage(Image source)
        {
            if (source == null) return null;

            // Create a clone of the image to avoid disposal issues
            var clone = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(clone))
            {
                g.DrawImage(source, 0, 0);
            }
            return clone;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose canvas shapes that hold images
                if (_canvas != null)
                {
                    foreach (var shape in _canvas.Shapes.OfType<ImageShape>())
                    {
                        shape.ImageData?.Dispose();
                    }
                }

                _canvasControl?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
