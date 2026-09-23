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
using System.Linq;
using System.Windows.Forms;
using Greenshot.Editor.Controls;
using Greenshot.Editor.Drawing.NewModel.Models;

namespace Greenshot.Editor.Forms;

partial class NewModelEditorForm
{
    private System.ComponentModel.IContainer components = null;
    private ToolStrip toolStrip;
    private ToolStripButton btnAddRectangle;
    private ToolStripButton btnAddEllipse;
    private ToolStripButton btnAddText;
    private ToolStripButton btnAddArrow;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton btnBlurFilter;
    private ToolStripButton btnHighlightFilter;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripComboBox cmbStyles;
    private ToolStripButton btnApplyStyle;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripButton btnDeleteSelected;
    private Panel layersPanel;
    private CheckedListBox lstLayers;
    private Label lblLayers;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.toolStrip = new ToolStrip();
        this.btnAddRectangle = new ToolStripButton();
        this.btnAddEllipse = new ToolStripButton();
        this.btnAddText = new ToolStripButton();
        this.btnAddArrow = new ToolStripButton();
        this.toolStripSeparator1 = new ToolStripSeparator();
        this.btnBlurFilter = new ToolStripButton();
        this.btnHighlightFilter = new ToolStripButton();
        this.toolStripSeparator2 = new ToolStripSeparator();
        this.cmbStyles = new ToolStripComboBox();
        this.btnApplyStyle = new ToolStripButton();
        this.toolStripSeparator3 = new ToolStripSeparator();
        this.btnDeleteSelected = new ToolStripButton();
        this.layersPanel = new Panel();
        this.lstLayers = new CheckedListBox();
        this.lblLayers = new Label();
        this.statusStrip = new StatusStrip();
        this.statusLabel = new ToolStripStatusLabel();

        _canvasControl = new NewModelCanvasControl();

        this.toolStrip.SuspendLayout();
        this.layersPanel.SuspendLayout();
        this.statusStrip.SuspendLayout();
        this.SuspendLayout();

        // toolStrip
        this.toolStrip.Items.AddRange(new ToolStripItem[] {
            this.btnAddRectangle,
            this.btnAddEllipse,
            this.btnAddText,
            this.btnAddArrow,
            this.toolStripSeparator1,
            this.btnBlurFilter,
            this.btnHighlightFilter,
            this.toolStripSeparator2,
            this.cmbStyles,
            this.btnApplyStyle,
            this.toolStripSeparator3,
            this.btnDeleteSelected
        });
        this.toolStrip.Location = new System.Drawing.Point(0, 0);
        this.toolStrip.Name = "toolStrip";
        this.toolStrip.Size = new System.Drawing.Size(984, 25);
        this.toolStrip.TabIndex = 0;
        this.toolStrip.Text = "toolStrip";

        // btnAddRectangle
        this.btnAddRectangle.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnAddRectangle.Name = "btnAddRectangle";
        this.btnAddRectangle.Size = new System.Drawing.Size(70, 22);
        this.btnAddRectangle.Text = "Rectangle";
        this.btnAddRectangle.Click += new System.EventHandler(this.BtnAddRectangle_Click);

        // btnAddEllipse
        this.btnAddEllipse.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnAddEllipse.Name = "btnAddEllipse";
        this.btnAddEllipse.Size = new System.Drawing.Size(50, 22);
        this.btnAddEllipse.Text = "Ellipse";
        this.btnAddEllipse.Click += new System.EventHandler(this.BtnAddEllipse_Click);

        // btnAddText
        this.btnAddText.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnAddText.Name = "btnAddText";
        this.btnAddText.Size = new System.Drawing.Size(35, 22);
        this.btnAddText.Text = "Text";
        this.btnAddText.Click += new System.EventHandler(this.BtnAddText_Click);

        // btnAddArrow
        this.btnAddArrow.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnAddArrow.Name = "btnAddArrow";
        this.btnAddArrow.Size = new System.Drawing.Size(50, 22);
        this.btnAddArrow.Text = "Arrow";
        this.btnAddArrow.Click += new System.EventHandler(this.BtnAddArrow_Click);

        // toolStripSeparator1
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);

        // btnBlurFilter
        this.btnBlurFilter.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnBlurFilter.Name = "btnBlurFilter";
        this.btnBlurFilter.Size = new System.Drawing.Size(35, 22);
        this.btnBlurFilter.Text = "Blur";
        this.btnBlurFilter.Click += new System.EventHandler(this.BtnBlurFilter_Click);

        // btnHighlightFilter
        this.btnHighlightFilter.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnHighlightFilter.Name = "btnHighlightFilter";
        this.btnHighlightFilter.Size = new System.Drawing.Size(60, 22);
        this.btnHighlightFilter.Text = "Highlight";
        this.btnHighlightFilter.Click += new System.EventHandler(this.BtnHighlightFilter_Click);

        // toolStripSeparator2
        this.toolStripSeparator2.Name = "toolStripSeparator2";
        this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);

        // cmbStyles
        this.cmbStyles.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbStyles.Name = "cmbStyles";
        this.cmbStyles.Size = new System.Drawing.Size(121, 25);
        this.cmbStyles.Items.AddRange(new object[] { "Default", "BlueFilled", "GreenHighlight", "BlackText" });
        this.cmbStyles.SelectedIndex = 0;

        // btnApplyStyle
        this.btnApplyStyle.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnApplyStyle.Name = "btnApplyStyle";
        this.btnApplyStyle.Size = new System.Drawing.Size(75, 22);
        this.btnApplyStyle.Text = "Apply Style";
        this.btnApplyStyle.Click += new System.EventHandler(this.BtnApplyStyle_Click);

        // toolStripSeparator3
        this.toolStripSeparator3.Name = "toolStripSeparator3";
        this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);

        // btnDeleteSelected
        this.btnDeleteSelected.DisplayStyle = ToolStripItemDisplayStyle.Text;
        this.btnDeleteSelected.Name = "btnDeleteSelected";
        this.btnDeleteSelected.Size = new System.Drawing.Size(50, 22);
        this.btnDeleteSelected.Text = "Delete";
        this.btnDeleteSelected.Click += new System.EventHandler(this.BtnDeleteSelected_Click);

        // layersPanel
        this.layersPanel.Controls.Add(this.lstLayers);
        this.layersPanel.Controls.Add(this.lblLayers);
        this.layersPanel.Dock = DockStyle.Right;
        this.layersPanel.Location = new System.Drawing.Point(784, 25);
        this.layersPanel.Name = "layersPanel";
        this.layersPanel.Size = new System.Drawing.Size(200, 586);
        this.layersPanel.TabIndex = 1;

        // lblLayers
        this.lblLayers.Dock = DockStyle.Top;
        this.lblLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.lblLayers.Location = new System.Drawing.Point(0, 0);
        this.lblLayers.Name = "lblLayers";
        this.lblLayers.Padding = new Padding(5);
        this.lblLayers.Size = new System.Drawing.Size(200, 25);
        this.lblLayers.TabIndex = 0;
        this.lblLayers.Text = "Layers";

        // lstLayers
        this.lstLayers.Dock = DockStyle.Fill;
        this.lstLayers.FormattingEnabled = true;
        this.lstLayers.Location = new System.Drawing.Point(0, 25);
        this.lstLayers.Name = "lstLayers";
        this.lstLayers.Size = new System.Drawing.Size(200, 561);
        this.lstLayers.TabIndex = 1;
        this.lstLayers.ItemCheck += new ItemCheckEventHandler(this.LstLayers_ItemCheck);
        this.lstLayers.SelectedIndexChanged += new System.EventHandler(this.LstLayers_SelectedIndexChanged);
        this.lstLayers.MouseDoubleClick += new MouseEventHandler(this.LstLayers_MouseDoubleClick);

        // _canvasControl
        _canvasControl.Dock = DockStyle.Fill;
        _canvasControl.Location = new System.Drawing.Point(0, 25);
        _canvasControl.Name = "_canvasControl";
        _canvasControl.Size = new System.Drawing.Size(784, 586);
        _canvasControl.TabIndex = 2;

        // statusStrip
        this.statusStrip.Items.AddRange(new ToolStripItem[] { this.statusLabel });
        this.statusStrip.Location = new System.Drawing.Point(0, 611);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new System.Drawing.Size(984, 22);
        this.statusStrip.TabIndex = 3;
        this.statusStrip.Text = "statusStrip";

        // statusLabel
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(118, 17);
        this.statusLabel.Text = "New Model Editor - Beta";

        // NewModelEditorForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(984, 633);
        this.Controls.Add(_canvasControl);
        this.Controls.Add(this.layersPanel);
        this.Controls.Add(this.toolStrip);
        this.Controls.Add(this.statusStrip);
        this.KeyPreview = true; // Allow form to intercept keyboard events
        this.Name = "NewModelEditorForm";
        this.Text = "New Model Editor - Beta";
        this.Load += new System.EventHandler(this.NewModelEditorForm_Load);

        this.toolStrip.ResumeLayout(false);
        this.toolStrip.PerformLayout();
        this.layersPanel.ResumeLayout(false);
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void NewModelEditorForm_Load(object sender, System.EventArgs e)
    {
        // Initialize canvas control with our model
        _canvasControl.SetCanvas(_canvas, _renderer);
        _canvasControl.SetActiveLayer(_activeLayer.Id); // Set active layer for selection

        // Populate layers list
        RefreshLayersList();
        
        // Give focus to the canvas control so keyboard events work
        _canvasControl.Focus();
    }

    private void RefreshLayersList()
    {
        lstLayers.Items.Clear();
        foreach (var layer in _canvas.Layers.OrderBy(l => l.ZIndex))
        {
            int index = lstLayers.Items.Add(layer);
            // Set checkbox state based on layer visibility
            lstLayers.SetItemChecked(index, layer.IsVisible);
        }
        lstLayers.DisplayMember = "Name";
        
        // Highlight active layer (selected index)
        if (_activeLayer != null)
        {
            for (int i = 0; i < lstLayers.Items.Count; i++)
            {
                if (lstLayers.Items[i] is Layer layer && layer.Id == _activeLayer.Id)
                {
                    lstLayers.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void BtnAddRectangle_Click(object sender, System.EventArgs e)
    {
        var style = _styleManager.GetStyle(cmbStyles.SelectedItem?.ToString() ?? "Default");
        var rect = new RectangleShape(new Dapplo.Windows.Common.Structs.NativeRect(50, 50, 150, 100), style);
        rect.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(rect);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Rectangle added to layer '{_activeLayer.Name}'";
    }

    private void BtnAddEllipse_Click(object sender, System.EventArgs e)
    {
        var style = _styleManager.GetStyle(cmbStyles.SelectedItem?.ToString() ?? "Default");
        var ellipse = new EllipseShape(new Dapplo.Windows.Common.Structs.NativeRect(200, 50, 150, 100), style);
        ellipse.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(ellipse);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Ellipse added to layer '{_activeLayer.Name}'";
    }

    private void BtnAddText_Click(object sender, System.EventArgs e)
    {
        var style = _styleManager.GetStyle("BlackText");
        var text = new TextShape(new Dapplo.Windows.Common.Structs.NativeRect(100, 200, 200, 50), style, "Sample Text", new System.Drawing.Font("Arial", 12));
        text.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(text);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Text added to layer '{_activeLayer.Name}'";
    }

    private void BtnAddArrow_Click(object sender, System.EventArgs e)
    {
        var style = _styleManager.GetStyle(cmbStyles.SelectedItem?.ToString() ?? "Default");
        var arrow = new ArrowShape(new System.Drawing.Point(350, 50), new System.Drawing.Point(450, 150), style, ArrowShape.ArrowHeadCombination.EndPoint);
        arrow.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(arrow);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Arrow added to layer '{_activeLayer.Name}'";
    }

    private void BtnBlurFilter_Click(object sender, System.EventArgs e)
    {
        var blur = new BlurFilterShape(new Dapplo.Windows.Common.Structs.NativeRect(300, 150, 100, 80), 8);
        blur.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(blur);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Blur filter added to layer '{_activeLayer.Name}'";
    }

    private void BtnHighlightFilter_Click(object sender, System.EventArgs e)
    {
        var highlight = new HighlightFilterShape(new Dapplo.Windows.Common.Structs.NativeRect(400, 150, 120, 90), System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black));
        highlight.LayerId = _activeLayer.Id; // Add to active layer
        _canvas.AddShape(highlight);
        _canvasControl.Invalidate();
        statusLabel.Text = $"Highlight filter added to layer '{_activeLayer.Name}'";
    }

    private void BtnApplyStyle_Click(object sender, System.EventArgs e)
    {
        var selectedShapes = _canvasControl.GetSelectedShapes();
        if (selectedShapes.Any())
        {
            var styleName = cmbStyles.SelectedItem?.ToString() ?? "Default";
            _styleManager.ApplyStyleToShapes(selectedShapes, styleName);
            _canvasControl.Invalidate();
            statusLabel.Text = $"Style '{styleName}' applied to {selectedShapes.Count()} shape(s)";
        }
        else
        {
            statusLabel.Text = "No shapes selected";
        }
    }

    private void BtnDeleteSelected_Click(object sender, System.EventArgs e)
    {
        var selectedShapes = _canvasControl.GetSelectedShapes();
        foreach (var shape in selectedShapes.ToList())
        {
            _canvas.RemoveShape(shape);
        }
        _canvasControl.ClearSelection();
        _canvasControl.Invalidate();
        statusLabel.Text = $"{selectedShapes.Count()} shape(s) deleted";
    }

    private void LstLayers_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        // ItemCheck fires before the checkbox state changes, so schedule the update
        this.BeginInvoke(new Action(() =>
        {
            if (lstLayers.Items[e.Index] is Layer layer)
            {
                layer.IsVisible = lstLayers.GetItemChecked(e.Index);
                _canvasControl.Invalidate();
                statusLabel.Text = $"Layer '{layer.Name}' visibility: {layer.IsVisible}";
            }
        }));
    }

    private void LstLayers_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        // Selection changed - this will be used to show which layer is active
        // We'll use double-click or a separate button to set the active layer
    }

    private void LstLayers_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        // Double-click sets the active layer
        int index = lstLayers.IndexFromPoint(e.Location);
        if (index >= 0 && index < lstLayers.Items.Count)
        {
            if (lstLayers.Items[index] is Layer layer)
            {
                // Don't allow setting background layer as active
                if (layer.Id == _backgroundLayer.Id)
                {
                    statusLabel.Text = "Cannot set background layer as active";
                    return;
                }

                _activeLayer = layer;
                _canvasControl.SetActiveLayer(layer.Id); // Tell canvas control about active layer
                lstLayers.SelectedIndex = index;
                statusLabel.Text = $"Active layer set to '{layer.Name}'";
            }
        }
    }

    protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
    {
        // Forward keyboard commands to the canvas control
        if (keyData == Keys.Delete || keyData == (Keys.Control | Keys.A))
        {
            if (_canvasControl.HandleKeyCommand(keyData))
            {
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }
}
