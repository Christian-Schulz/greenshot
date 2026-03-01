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

using Greenshot.Base.Controls;
using Greenshot.Plugin.Pdf.Configuration;

namespace Greenshot.Plugin.Pdf.Forms
{
    partial class PdfExportSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelPageWidth = new System.Windows.Forms.Label();
            this.numericPageWidth = new System.Windows.Forms.NumericUpDown();
            this.labelPageHeight = new System.Windows.Forms.Label();
            this.numericPageHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBoxPageSize = new System.Windows.Forms.GroupBox();
            this.groupBoxMargins = new System.Windows.Forms.GroupBox();
            this.labelMarginTop = new System.Windows.Forms.Label();
            this.numericMarginTop = new System.Windows.Forms.NumericUpDown();
            this.labelMarginBottom = new System.Windows.Forms.Label();
            this.numericMarginBottom = new System.Windows.Forms.NumericUpDown();
            this.labelMarginLeft = new System.Windows.Forms.Label();
            this.numericMarginLeft = new System.Windows.Forms.NumericUpDown();
            this.labelMarginRight = new System.Windows.Forms.Label();
            this.numericMarginRight = new System.Windows.Forms.NumericUpDown();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxRetainRatio = new GreenshotCheckBox();
            this.checkBoxFitToPage = new GreenshotCheckBox();
            this.buttonOK = new GreenshotButton();
            this.buttonCancel = new GreenshotButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericPageWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginRight)).BeginInit();
            this.SuspendLayout();

            // labelPageWidth
            this.labelPageWidth.AutoSize = true;
            this.labelPageWidth.Location = new System.Drawing.Point(12, 22);
            this.labelPageWidth.Name = "labelPageWidth";
            this.labelPageWidth.Size = new System.Drawing.Size(93, 13);
            this.labelPageWidth.TabIndex = 0;
            this.labelPageWidth.Text = "Page Width (mm):";

            // numericPageWidth
            this.numericPageWidth.DecimalPlaces = 2;
            this.numericPageWidth.Location = new System.Drawing.Point(120, 19);
            this.numericPageWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.numericPageWidth.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericPageWidth.Name = "numericPageWidth";
            this.numericPageWidth.Size = new System.Drawing.Size(80, 20);
            this.numericPageWidth.TabIndex = 1;
            this.numericPageWidth.Value = new decimal(new int[] { 210, 0, 0, 0 });

            // labelPageHeight
            this.labelPageHeight.AutoSize = true;
            this.labelPageHeight.Location = new System.Drawing.Point(12, 48);
            this.labelPageHeight.Name = "labelPageHeight";
            this.labelPageHeight.Size = new System.Drawing.Size(99, 13);
            this.labelPageHeight.TabIndex = 2;
            this.labelPageHeight.Text = "Page Height (mm):";

            // numericPageHeight
            this.numericPageHeight.DecimalPlaces = 2;
            this.numericPageHeight.Location = new System.Drawing.Point(120, 45);
            this.numericPageHeight.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numericPageHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericPageHeight.Name = "numericPageHeight";
            this.numericPageHeight.Size = new System.Drawing.Size(80, 20);
            this.numericPageHeight.TabIndex = 3;
            this.numericPageHeight.Value = new decimal(new int[] { 297, 0, 0, 0 });

            // groupBoxPageSize
            this.groupBoxPageSize.Controls.Add(this.labelPageWidth);
            this.groupBoxPageSize.Controls.Add(this.numericPageWidth);
            this.groupBoxPageSize.Controls.Add(this.labelPageHeight);
            this.groupBoxPageSize.Controls.Add(this.numericPageHeight);
            this.groupBoxPageSize.Location = new System.Drawing.Point(12, 12);
            this.groupBoxPageSize.Name = "groupBoxPageSize";
            this.groupBoxPageSize.Size = new System.Drawing.Size(220, 80);
            this.groupBoxPageSize.TabIndex = 0;
            this.groupBoxPageSize.TabStop = false;
            this.groupBoxPageSize.Text = "Page Size";

            // labelMarginTop
            this.labelMarginTop.AutoSize = true;
            this.labelMarginTop.Location = new System.Drawing.Point(12, 22);
            this.labelMarginTop.Name = "labelMarginTop";
            this.labelMarginTop.Size = new System.Drawing.Size(84, 13);
            this.labelMarginTop.TabIndex = 0;
            this.labelMarginTop.Text = "Top Margin (mm):";

            // numericMarginTop
            this.numericMarginTop.DecimalPlaces = 2;
            this.numericMarginTop.Location = new System.Drawing.Point(120, 19);
            this.numericMarginTop.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numericMarginTop.Name = "numericMarginTop";
            this.numericMarginTop.Size = new System.Drawing.Size(80, 20);
            this.numericMarginTop.TabIndex = 1;
            this.numericMarginTop.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // labelMarginBottom
            this.labelMarginBottom.AutoSize = true;
            this.labelMarginBottom.Location = new System.Drawing.Point(12, 48);
            this.labelMarginBottom.Name = "labelMarginBottom";
            this.labelMarginBottom.Size = new System.Drawing.Size(98, 13);
            this.labelMarginBottom.TabIndex = 2;
            this.labelMarginBottom.Text = "Bottom Margin (mm):";

            // numericMarginBottom
            this.numericMarginBottom.DecimalPlaces = 2;
            this.numericMarginBottom.Location = new System.Drawing.Point(120, 45);
            this.numericMarginBottom.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numericMarginBottom.Name = "numericMarginBottom";
            this.numericMarginBottom.Size = new System.Drawing.Size(80, 20);
            this.numericMarginBottom.TabIndex = 3;
            this.numericMarginBottom.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // labelMarginLeft
            this.labelMarginLeft.AutoSize = true;
            this.labelMarginLeft.Location = new System.Drawing.Point(12, 74);
            this.labelMarginLeft.Name = "labelMarginLeft";
            this.labelMarginLeft.Size = new System.Drawing.Size(89, 13);
            this.labelMarginLeft.TabIndex = 4;
            this.labelMarginLeft.Text = "Left Margin (mm):";

            // numericMarginLeft
            this.numericMarginLeft.DecimalPlaces = 2;
            this.numericMarginLeft.Location = new System.Drawing.Point(120, 71);
            this.numericMarginLeft.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numericMarginLeft.Name = "numericMarginLeft";
            this.numericMarginLeft.Size = new System.Drawing.Size(80, 20);
            this.numericMarginLeft.TabIndex = 5;
            this.numericMarginLeft.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // labelMarginRight
            this.labelMarginRight.AutoSize = true;
            this.labelMarginRight.Location = new System.Drawing.Point(12, 100);
            this.labelMarginRight.Name = "labelMarginRight";
            this.labelMarginRight.Size = new System.Drawing.Size(96, 13);
            this.labelMarginRight.TabIndex = 6;
            this.labelMarginRight.Text = "Right Margin (mm):";

            // numericMarginRight
            this.numericMarginRight.DecimalPlaces = 2;
            this.numericMarginRight.Location = new System.Drawing.Point(120, 97);
            this.numericMarginRight.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numericMarginRight.Name = "numericMarginRight";
            this.numericMarginRight.Size = new System.Drawing.Size(80, 20);
            this.numericMarginRight.TabIndex = 7;
            this.numericMarginRight.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // groupBoxMargins
            this.groupBoxMargins.Controls.Add(this.labelMarginTop);
            this.groupBoxMargins.Controls.Add(this.numericMarginTop);
            this.groupBoxMargins.Controls.Add(this.labelMarginBottom);
            this.groupBoxMargins.Controls.Add(this.numericMarginBottom);
            this.groupBoxMargins.Controls.Add(this.labelMarginLeft);
            this.groupBoxMargins.Controls.Add(this.numericMarginLeft);
            this.groupBoxMargins.Controls.Add(this.labelMarginRight);
            this.groupBoxMargins.Controls.Add(this.numericMarginRight);
            this.groupBoxMargins.Location = new System.Drawing.Point(12, 98);
            this.groupBoxMargins.Name = "groupBoxMargins";
            this.groupBoxMargins.Size = new System.Drawing.Size(220, 135);
            this.groupBoxMargins.TabIndex = 1;
            this.groupBoxMargins.TabStop = false;
            this.groupBoxMargins.Text = "Margins";

            // checkBoxFitToPage
            this.checkBoxFitToPage.AutoSize = true;
            this.checkBoxFitToPage.Location = new System.Drawing.Point(12, 22);
            this.checkBoxFitToPage.Name = "checkBoxFitToPage";
            this.checkBoxFitToPage.PropertyName = nameof(PdfExportSettings.FitToPage);
            this.checkBoxFitToPage.SectionName = "Pdf";
            this.checkBoxFitToPage.Size = new System.Drawing.Size(153, 17);
            this.checkBoxFitToPage.TabIndex = 0;
            this.checkBoxFitToPage.Text = "Fit Image to Page Margins";
            this.checkBoxFitToPage.UseVisualStyleBackColor = true;

            // checkBoxRetainRatio
            this.checkBoxRetainRatio.AutoSize = true;
            this.checkBoxRetainRatio.Location = new System.Drawing.Point(12, 45);
            this.checkBoxRetainRatio.Name = "checkBoxRetainRatio";
            this.checkBoxRetainRatio.PropertyName = nameof(PdfExportSettings.RetainRatio);
            this.checkBoxRetainRatio.SectionName = "Pdf";
            this.checkBoxRetainRatio.Size = new System.Drawing.Size(185, 17);
            this.checkBoxRetainRatio.TabIndex = 1;
            this.checkBoxRetainRatio.Text = "Retain Image Ratio (Auto Height)";
            this.checkBoxRetainRatio.UseVisualStyleBackColor = true;

            // groupBoxOptions
            this.groupBoxOptions.Controls.Add(this.checkBoxFitToPage);
            this.groupBoxOptions.Controls.Add(this.checkBoxRetainRatio);
            this.groupBoxOptions.Location = new System.Drawing.Point(12, 239);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(220, 75);
            this.groupBoxOptions.TabIndex = 2;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options";

            // buttonOK
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(12, 320);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(55, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;

            // buttonCancel
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(73, 320);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(55, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;

            // PdfExportSettingsForm
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 352);
            this.Controls.Add(this.groupBoxPageSize);
            this.Controls.Add(this.groupBoxMargins);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PdfExportSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PDF Export Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericPageWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMarginRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelPageWidth;
        private System.Windows.Forms.NumericUpDown numericPageWidth;
        private System.Windows.Forms.Label labelPageHeight;
        private System.Windows.Forms.NumericUpDown numericPageHeight;
        private System.Windows.Forms.GroupBox groupBoxPageSize;
        private System.Windows.Forms.GroupBox groupBoxMargins;
        private System.Windows.Forms.Label labelMarginTop;
        private System.Windows.Forms.NumericUpDown numericMarginTop;
        private System.Windows.Forms.Label labelMarginBottom;
        private System.Windows.Forms.NumericUpDown numericMarginBottom;
        private System.Windows.Forms.Label labelMarginLeft;
        private System.Windows.Forms.NumericUpDown numericMarginLeft;
        private System.Windows.Forms.Label labelMarginRight;
        private System.Windows.Forms.NumericUpDown numericMarginRight;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private GreenshotCheckBox checkBoxRetainRatio;
        private GreenshotCheckBox checkBoxFitToPage;
        private GreenshotButton buttonOK;
        private GreenshotButton buttonCancel;
    }
}
