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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Drawing;
using Greenshot.Editor.Drawing.Fields;
using Greenshot.Editor.Helpers;

namespace Greenshot.Editor.Drawing
{
    /// <summary>
    /// Description of CuttingMarkContainer.
    /// Draws a zigzag/wave pattern line to mark cutting lines.
    /// </summary>
    [Serializable()]
    public class CuttingMarkContainer : DrawableContainer
    {
        /// <summary>
        /// Available CuttingMark orientations
        /// </summary>
        public enum CuttingMarkMode
        {
            /// <summary>
            /// Horizontal cutting mark spanning the full width
            /// </summary>
            Horizontal,
            /// <summary>
            /// Vertical cutting mark spanning the full height
            /// </summary>
            Vertical
        }

        /// <summary>
        /// Available fill patterns for the torn region
        /// </summary>
        public enum FillPattern
        {
            /// <summary>
            /// Solid color fill
            /// </summary>
            Solid,
            /// <summary>
            /// Diagonal lines pattern
            /// </summary>
            DiagonalLines,
            /// <summary>
            /// Dotted grid pattern
            /// </summary>
            DottedGrid,
            /// <summary>
            /// Horizontal lines pattern
            /// </summary>
            HorizontalLines,
            /// <summary>
            /// Vertical lines pattern
            /// </summary>
            VerticalLines,
            /// <summary>
            /// Cross hatch pattern
            /// </summary>
            CrossHatch,
            /// <summary>
            /// Gradient from line color to fill color and back
            /// </summary>
            GradientToCenter
        }

        public CuttingMarkContainer(ISurface parent) : base(parent)
        {
            Init();
            SetFieldValue(FieldType.SHAPE_THICKNESS, 55);
            SetFieldValue(FieldType.FILL_PATTERN, FillPattern.HorizontalLines);
        }

        protected override void InitializeFields()
        {
            AddField(GetType(), FieldType.LINE_THICKNESS, 2);
            AddField(GetType(), FieldType.LINE_COLOR, Color.LightGray);
            AddField(GetType(), FieldType.FILL_COLOR, Color.White);
            AddField(GetType(), FieldType.FILL_PATTERN, FillPattern.GradientToCenter);
            AddField(GetType(), FieldType.SHAPE_THICKNESS, 20);
            AddField(GetType(), FieldType.CUTTINGMARK_MODE, CuttingMarkMode.Horizontal);
        }

        protected override void OnDeserialized(StreamingContext context)
        {
            Init();
        }

        protected void Init()
        {
            switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
            {
                case CuttingMarkMode.Horizontal:
                    InitHorizontalMode();
                    break;
                case CuttingMarkMode.Vertical:
                    InitVerticalMode();
                    break;
            }
        }

        private void InitHorizontalMode()
        {
            int shapeThickness = GetFieldValueAsInt(FieldType.SHAPE_THICKNESS);

            if (_parent?.Image is { } image)
            {
                Size = new Size(image.Width, shapeThickness + 10);
            }
        }

        private void InitVerticalMode()
        {
            int shapeThickness = GetFieldValueAsInt(FieldType.SHAPE_THICKNESS);

            if (_parent?.Image is { } image)
            {
                Size = new Size(shapeThickness + 10, image.Height);
            }
        }

        public override void MoveBy(int dx, int dy)
        {
           // Left += dx;
            //Top += dy;

            switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
            {
                case CuttingMarkMode.Horizontal:
                    Left = 0; // Keep horizontal cutting mark spanning the full width
                    Top += dy;
                    break;
                case CuttingMarkMode.Vertical:
                    Top = 0; // Keep vertical cutting mark spanning the full height
                    Left += dx;
                    break;
            }
        }

        public override bool HandleMouseDown(int x, int y)
        {
            return GetFieldValue(FieldType.CUTTINGMARK_MODE) switch
            {
                CuttingMarkMode.Horizontal => base.HandleMouseDown(0, y),
                CuttingMarkMode.Vertical => base.HandleMouseDown(x, 0),
                _ => base.HandleMouseDown(x, y),
            };
        }

        public override bool HandleMouseMove(int x, int y)
        {
          
            switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
            {
                case CuttingMarkMode.Horizontal:
                    {
                        Left = 0;
                        Top = y;                        
                        break;
                    }
                case CuttingMarkMode.Vertical:
                    {
                        Top = 0;
                        Left = x;
                        break;
                    }
            }

            Invalidate();
            return true;
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);

            if (lineThickness <= 0) return;

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.None;

            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            
            DrawFilledTornRegion(graphics);
            DrawTornEdges(graphics, lineColor, lineThickness);
        }

        private void DrawFilledTornRegion(Graphics graphics)
        {
            int shapeThickness = GetFieldValueAsInt(FieldType.SHAPE_THICKNESS);
            
            using GraphicsPath topPath = CreateTornDocBorderPath(0);
            using GraphicsPath bottomPath = CreateTornDocBorderPath(shapeThickness);
            using GraphicsPath fillPath = CreateFilledTornRegion(topPath, bottomPath);
            
            Color fillColor = GetFieldValueAsColor(FieldType.FILL_COLOR);
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);

            FillPattern pattern = (FillPattern)GetFieldValue(FieldType.FILL_PATTERN);
            
            using Brush fillBrush = CreateFillBrush(pattern, fillColor, lineColor, fillPath);
            graphics.FillPath(fillBrush, fillPath);
        }

        private Brush CreateFillBrush(FillPattern pattern, Color fillColor, Color patternColor, GraphicsPath fillPath)
        {
            switch (pattern)
            {
                case FillPattern.DiagonalLines:
                    return new HatchBrush(HatchStyle.LightUpwardDiagonal, patternColor, fillColor);
                case FillPattern.DottedGrid:
                    return new HatchBrush(HatchStyle.DottedGrid, patternColor, fillColor);
                case FillPattern.HorizontalLines:
                    return new HatchBrush(HatchStyle.LightHorizontal, patternColor, fillColor);
                case FillPattern.VerticalLines:
                    return new HatchBrush(HatchStyle.LightVertical, patternColor, fillColor);
                case FillPattern.CrossHatch:
                    return new HatchBrush(HatchStyle.Cross, patternColor, fillColor);
                case FillPattern.GradientToCenter:
                    return CreateGradientBrush(fillPath, patternColor, fillColor);
                case FillPattern.Solid:
                default:
                    return new SolidBrush(fillColor);
            }
        }

        private Brush CreateGradientBrush(GraphicsPath fillPath, Color edgeColor, Color centerColor)
        {
            try
            {
                // Get the bounding box of the path
                RectangleF bounds = fillPath.GetBounds();
                
                // Determine gradient direction based on cutting mark mode
                LinearGradientBrush linearBrush;
                
                switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
                {
                    case CuttingMarkMode.Horizontal:
                        // Gradient goes from top to bottom (between the two horizontal torn edges)
                        linearBrush = new LinearGradientBrush(
                            new PointF(bounds.Left, bounds.Top),
                            new PointF(bounds.Left, bounds.Bottom),
                            edgeColor,
                            centerColor);
                        
                        // Add color stops for the gradient: edge -> center -> edge
                        ColorBlend colorBlend = new ColorBlend(3)
                        {
                            Colors = new[] {  edgeColor, edgeColor, centerColor, edgeColor, edgeColor },
                            Positions = new[] { 0f, 0.1f, 0.5f, 0.9f, 1f }
                        };
                        linearBrush.InterpolationColors = colorBlend;
                        break;
                        
                    case CuttingMarkMode.Vertical:
                        // Gradient goes from left to right (between the two vertical torn edges)
                        linearBrush = new LinearGradientBrush(
                            new PointF(bounds.Left, bounds.Top),
                            new PointF(bounds.Right, bounds.Top),
                            edgeColor,
                            centerColor);
                        
                        // Add color stops for the gradient: edge -> center -> edge
                        colorBlend = new ColorBlend(3)
                        {
                            Colors = new[] { edgeColor, edgeColor, centerColor, edgeColor, edgeColor },
                            Positions = new[] {0f, 0.1f, 0.5f, 0.9f, 1f }
                        };
                        linearBrush.InterpolationColors = colorBlend;
                        break;
                        
                    default:
                        return new SolidBrush(centerColor);
                }
                
                return linearBrush;
            }
            catch
            {
                // Fallback to solid brush if gradient creation fails
                return new SolidBrush(centerColor);
            }
        }

        private void DrawTornEdges(Graphics graphics, Color lineColor, int lineThickness)
        {
            int shapeThickness = GetFieldValueAsInt(FieldType.SHAPE_THICKNESS);
            
            using GraphicsPath topPath = CreateTornDocBorderPath(0);
            using GraphicsPath bottomPath = CreateTornDocBorderPath(shapeThickness);
            using Pen pen = new Pen(lineColor, lineThickness);
            
            graphics.DrawPath(pen, topPath);
            graphics.DrawPath(pen, bottomPath);
        }

        private GraphicsPath CreateFilledTornRegion(GraphicsPath topPath, GraphicsPath bottomPath)
        {
            GraphicsPath fillPath = new GraphicsPath();
            
            // Add the top path
            fillPath.AddPath(topPath, false);
            
            // Add the bottom path in reverse to close the shape
            PointF[] bottomPoints = bottomPath.PathPoints;
            Array.Reverse(bottomPoints);
            
            if (bottomPoints.Length > 0)
            {
                fillPath.AddLines(bottomPoints);
                fillPath.CloseFigure();
            }
            
            return fillPath;
        }

        private GraphicsPath CreateTornDocBorderPath(int offset)
        {
            GraphicsPath path = new GraphicsPath();
            
            const int margin = 0;
            
            int startX, startY, endX, endY;
            
            // Calculate start and end points based on cutting mark mode
            switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
            {
                case CuttingMarkMode.Horizontal:
                    startX = Left + margin;
                    endX = Left + Width - margin;
                    startY = Top + Height / 2 + offset;
                    endY = Top + Height / 2 + offset;
                    break;
                case CuttingMarkMode.Vertical:
                    startX = Left + Width / 2 + offset;
                    endX = Left + Width / 2 + offset;
                    startY = Top + margin;
                    endY = Top + Height - margin;
                    break;
                default:
                    startX = Left;
                    startY = Top;
                    endX = Left + Width;
                    endY = Top + Height;
                    break;
            }
            
            // Calculate line length and direction
            double dx = endX - startX;
            double dy = endY - startY;
            double lineLength = Math.Sqrt(dx * dx + dy * dy);
            
            if (lineLength == 0)
            {
                return path;
            }
            
            // Normalize direction vectors
            double dirX = dx / lineLength;
            double dirY = dy / lineLength;
            
            // Perpendicular direction for tears
            double perpX = -dirY;
            double perpY = dirX;
            
            // Parameters for torn edge
            double segmentLength = 30;
            int segmentCount = Math.Max(5, (int)(lineLength / segmentLength));
            
            Random random = new Random(GetHashCode());
            
            // Build the torn path
            List<PointF> points = new List<PointF>();
            
            for (int i = 0; i <= segmentCount; i++)
            {
                double t = i / (double)segmentCount;
                
                // Position along the main line
                double baseX = startX + dx * t;
                double baseY = startY + dy * t;
                
                // Random offset perpendicular to the line
                double tearDepth = random.NextDouble() * 12 - 3;
                
                // Add some variation in the length along the line too
                double lengthVariation = (random.NextDouble() - 0.5) * 3;
                
                double finalX = baseX + perpX * tearDepth + dirX * lengthVariation;
                double finalY = baseY + perpY * tearDepth + dirY * lengthVariation;
                
                points.Add(new PointF((float)finalX, (float)finalY));
            }
            
            // Create smooth curves through the points for a more natural torn look
            if (points.Count >= 2)
            {
                path.AddCurve(points.ToArray(), 0.3f);
            }
            
            return path;
        }

        private GraphicsPath CreateZigzagPath()
        {
            GraphicsPath path = new GraphicsPath();
            
            const int margin = 5;
            
            int startX, startY, endX, endY;
            
            // Calculate start and end points based on cutting mark mode
            switch (GetFieldValue(FieldType.CUTTINGMARK_MODE))
            {
                case CuttingMarkMode.Horizontal:
                    // Horizontal line in the middle of the height
                    startX = Left + margin;
                    endX = Left + Width - margin;
                    startY = Top + Height / 2;
                    endY = Top + Height / 2;
                    break;
                case CuttingMarkMode.Vertical:
                    // Vertical line in the middle of the width
                    startX = Left + Width / 2;
                    endX = Left + Width / 2;
                    startY = Top + margin;
                    endY = Top + Height - margin;
                    break;
                default:
                    startX = Left;
                    startY = Top;
                    endX = Left + Width;
                    endY = Top + Height;
                    break;
            }
            
            // Calculate line length and angle
            double dx = endX - startX;
            double dy = endY - startY;
            double lineLength = Math.Sqrt(dx * dx + dy * dy);
            
            // Zigzag amplitude (wave height)
            double amplitude = 8;
            
            // Zigzag frequency (number of zigzags along the line)
            int zigzagCount = Math.Max(3, (int)(lineLength / 20));
            
            // Create points for the zigzag pattern
            PointF[] points = new PointF[zigzagCount * 2 + 1];
            
            for (int i = 0; i <= zigzagCount * 2; i++)
            {
                double t = lineLength > 0 ? (i / (double)(zigzagCount * 2)) : 0;
                
                // Base point on the line
                double baseX = startX + dx * t;
                double baseY = startY + dy * t;
                
                // Perpendicular offset for zigzag
                double perpX = 0;
                double perpY = 0;
                
                if (lineLength > 0)
                {
                    // Normalize perpendicular direction
                    perpX = -dy / lineLength;
                    perpY = dx / lineLength;
                }
                
                // Alternate sides for zigzag effect
                double offset = ((i % 2) == 0) ? amplitude : -amplitude;
                
                points[i] = new PointF(
                    (float)(baseX + perpX * offset),
                    (float)(baseY + perpY * offset)
                );
            }
            
            path.AddLines(points);
            return path;
        }

        public override bool ClickableAt(int x, int y)
        {
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS) + 10;
            int shapeThickness = GetFieldValueAsInt(FieldType.SHAPE_THICKNESS);
            
            if (lineThickness > 0)
            {
                using Pen pen = new Pen(Color.White)
                {
                    Width = lineThickness
                };
                using GraphicsPath path1 = CreateTornDocBorderPath(0);
                using GraphicsPath path2 = CreateTornDocBorderPath(shapeThickness);
                
                return path1.IsOutlineVisible(x, y, pen) || path2.IsOutlineVisible(x, y, pen);
            }

            return false;
        }

        protected override IDoubleProcessor GetAngleRoundProcessor()
        {
            return LineAngleRoundBehavior.INSTANCE;
        }
    }
}
