/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace fyiReporting.RDL
{
	///<summary>
	/// Base class of all charts.
	///</summary>
	internal abstract class ChartBase : IDisposable
	{
		Chart _ChartDefn;
		MatrixCellEntry[,] _DataDefn;
		protected Bitmap _bm;
		protected ChartLayout Layout;
		Brush[] _SeriesBrush;
		ChartMarkerEnum[] _SeriesMarker;
		protected int _LastCategoryWidth=0;
		protected Row _row;					// row chart created on

		internal ChartBase(Report r, Row row, Chart c, MatrixCellEntry[,] m)
		{
			_ChartDefn = c;
			_row = row;
			_DataDefn = m;
			_bm = null;
			int width = _ChartDefn.WidthCalc(r, null);
			int height = RSize.PixelsFromPoints(_ChartDefn.HeightOrOwnerHeight);
			Layout = new ChartLayout(width, height);
			_SeriesBrush = null;
			_SeriesMarker = null;
		}

		internal virtual void Draw(Report rpt)
		{
		}

		internal void Save(Report rpt, System.IO.Stream stream, ImageFormat im)
		{
			if (_bm == null)
				Draw(rpt);
			
			_bm.Save(stream, im);
		}

		internal System.Drawing.Image Image(Report rpt)
		{
			if (_bm == null)
				Draw(rpt);

			return _bm;
		}

		protected Bitmap CreateSizedBitmap()
		{
			if (_bm != null)
			{
				_bm.Dispose();
				_bm = null;
			}
			_bm = new Bitmap(Layout.Width, Layout.Height);
			return _bm;
		}

		protected int AxisTickMarkMajorLen
		{
			get{return 6;}
		}

		protected int AxisTickMarkMinorLen
		{
			get{return 3;}
		}

		protected int CategoryCount
		{
			get{return (_DataDefn.GetLength(0) - 1);}
		}

		protected Chart ChartDefn
		{
			get{return _ChartDefn;}
		}

		protected MatrixCellEntry[,] DataDefn
		{
			get{return _DataDefn;}
		}

		protected Brush[] SeriesBrush
		{
			get 
			{
				if (_SeriesBrush == null)
					_SeriesBrush = GetSeriesBrushes();	// These are all from Brushes class; so no Dispose should be used
				return _SeriesBrush;
			}				  
		}
	
		protected ChartMarkerEnum[] SeriesMarker
		{
			get
			{
				if (_SeriesMarker == null)
					_SeriesMarker = GetSeriesMarkers();
				return _SeriesMarker;
			}
		}

		protected int SeriesCount
		{
			get{return (_DataDefn.GetLength(1) - 1);}
		}

		protected void DrawChartStyle(Report rpt, Graphics g)
		{
			System.Drawing.Rectangle rect = new  System.Drawing.Rectangle(0, 0, Layout.Width, Layout.Height);
			if (_ChartDefn.Style == null)
			{
				g.FillRectangle(Brushes.White, rect);
			}
			else
			{
				Row r = FirstChartRow(rpt);
				_ChartDefn.Style.DrawBorder(rpt, g, r, rect);
				_ChartDefn.Style.DrawBackground(rpt, g, r, rect);
			}

			return;
		}

		// Draws the Legend and then returns the rectangle it drew in
		protected System.Drawing.Rectangle DrawLegend(Report rpt, Graphics g, bool bMarker, bool bBeforePlotDrawn)
		{
			Legend l = _ChartDefn.Legend;
			if (l == null)
				return System.Drawing.Rectangle.Empty;
			if (!l.Visible)
				return System.Drawing.Rectangle.Empty;
			if (_ChartDefn.SeriesGroupings == null)
				return System.Drawing.Rectangle.Empty;
			if (bBeforePlotDrawn)
			{
				if (this.IsLegendInsidePlotArea())
					return System.Drawing.Rectangle.Empty;
			}
			else if (!IsLegendInsidePlotArea())			// Only draw legend after if inside the plot
				return System.Drawing.Rectangle.Empty;

			Font drawFont = null;
			Brush drawBrush = null;
			StringFormat drawFormat = null;
	
			// calculated bounding rectangle of the legend
			System.Drawing.Rectangle rRect;
			Style s = l.Style;
			try		// no matter what we want to dispose of the graphic resources
			{
				if (s == null)
				{
					drawFont = new Font("Arial", 10);
					drawBrush = new SolidBrush(Color.Black);
					drawFormat = new StringFormat();
					drawFormat.Alignment = StringAlignment.Near;
				}
				else
				{
					drawFont = 	s.GetFont(rpt, null);
					drawBrush = s.GetBrush(rpt, null);
					drawFormat = s.GetStringFormat(rpt, null);
				}

				int x, y, h;
				int maxTextWidth, maxTextHeight;
				drawFormat.FormatFlags |= StringFormatFlags.NoWrap;
				Size[] sizes = DrawLegendMeasure(rpt, g, drawFont, drawFormat, 
					new SizeF(Layout.Width, Layout.Height), out maxTextWidth, out maxTextHeight);
				int boxSize = (int) (maxTextHeight * .8);
				int totalItemWidth = 0;			// width of a legend item
				int totalWidth, totalHeight;	// final height and width of legend

				// calculate the height and width of the rectangle
				switch (l.Layout)
				{
					case LegendLayoutEnum.Row:
						// we need to loop thru all the width
						totalWidth=0;
						for (int i = 0; i < SeriesCount; i++)
						{
							totalWidth += (sizes[i].Width + (boxSize * 2));
						}
						totalHeight = (int) (maxTextHeight + (maxTextHeight * .1));
						h = totalHeight;
						totalItemWidth = maxTextWidth + boxSize * 2; 
						drawFormat.Alignment = StringAlignment.Near;	// Force alignment to near
						break;
					case LegendLayoutEnum.Table:
					case LegendLayoutEnum.Column:
					default:
						totalWidth = totalItemWidth = maxTextWidth + boxSize * 2; 
						h = (int) (maxTextHeight + (maxTextHeight * .1));
						totalHeight = h * SeriesCount;
						break;
				}

				// calculate the location of the legend rectangle
				if (this.IsLegendInsidePlotArea())
				switch (l.Position)
				{
					case LegendPositionEnum.BottomCenter:
						x = Layout.PlotArea.X + (Layout.PlotArea.Width / 2) - (totalWidth / 2);
						y = Layout.PlotArea.Y + Layout.PlotArea.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.BottomLeft:
					case LegendPositionEnum.LeftBottom:
						x = Layout.PlotArea.X+2;
						y = Layout.PlotArea.Y + Layout.PlotArea.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.BottomRight:
					case LegendPositionEnum.RightBottom:
						x = Layout.PlotArea.X + Layout.PlotArea.Width - totalWidth;
						y = Layout.PlotArea.Y + Layout.PlotArea.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.LeftCenter:
						x = Layout.PlotArea.X + 2;
						y = Layout.PlotArea.Y + (Layout.PlotArea.Height / 2) - (totalHeight/2);
						break;
					case LegendPositionEnum.LeftTop:
					case LegendPositionEnum.TopLeft:
						x = Layout.PlotArea.X + 2;
						y = Layout.PlotArea.Y+2;
						break;
					case LegendPositionEnum.RightCenter:
						x = Layout.PlotArea.X + Layout.PlotArea.Width - totalWidth - 2;
						y = Layout.PlotArea.Y + (Layout.PlotArea.Height / 2) - (totalHeight/2);
						break;
					case LegendPositionEnum.TopCenter:
						x = Layout.PlotArea.X + (Layout.PlotArea.Width / 2) - (totalWidth / 2);
						y = Layout.PlotArea.Y + +2;
						break;
					case LegendPositionEnum.TopRight:
					case LegendPositionEnum.RightTop:
					default:
						x = Layout.PlotArea.X + Layout.PlotArea.Width-totalWidth - 2;
						y = Layout.PlotArea.Y + +2;
						break;
				}
				else switch (l.Position)
				{
					case LegendPositionEnum.BottomCenter:
						x = (Layout.Width / 2) - (totalWidth / 2);
						y = Layout.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.BottomLeft:
					case LegendPositionEnum.LeftBottom:
						if (IsLegendInsidePlotArea())
							x = Layout.LeftMargin;
						else
							x = 0;
						y = Layout.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.BottomRight:
					case LegendPositionEnum.RightBottom:
						x = Layout.Width - totalWidth;
						y = Layout.Height - totalHeight - 2;
						break;
					case LegendPositionEnum.LeftCenter:
						x = 2;
						y = (Layout.Height / 2) - (totalHeight/2);
						break;
					case LegendPositionEnum.LeftTop:
					case LegendPositionEnum.TopLeft:
						x = 2;
						y = Layout.TopMargin+2;
						break;
					case LegendPositionEnum.RightCenter:
						x = Layout.Width - totalWidth - 2;
						y = (Layout.Height / 2) - (totalHeight/2);
						break;
					case LegendPositionEnum.TopCenter:
						x = (Layout.Width / 2) - (totalWidth / 2);
						y = Layout.TopMargin+2;
						break;
					case LegendPositionEnum.TopRight:
					case LegendPositionEnum.RightTop:
					default:
						x = Layout.Width-totalWidth - 2;
						y = Layout.TopMargin+2;
						break;
				}

				// We now know enough to calc the bounding rectangle of the legend
				rRect = new System.Drawing.Rectangle(x-1, y-1, totalWidth+2, totalHeight+2);
				if (s != null)
				{
					s.DrawBackground(rpt, g, null, rRect);	// draw (or not draw) background 
					s.DrawBorder(rpt, g, null, rRect);		// draw (or not draw) border depending on style
				}

				for (int iCol=1; iCol <= SeriesCount; iCol++)
				{
					string c = GetSeriesValue(rpt, iCol);
					System.Drawing.Rectangle rect;
					switch (l.Layout)
					{
						case LegendLayoutEnum.Row:
							rect = new System.Drawing.Rectangle(x + boxSize + boxSize/2, y, totalItemWidth - boxSize - boxSize/2, h);
							g.DrawString(c, drawFont, drawBrush, rect, drawFormat);
							DrawLegendBox(g, SeriesBrush[iCol-1],
								bMarker? SeriesMarker[iCol-1]: ChartMarkerEnum.None, 
								x, y+1, boxSize);
							x += (sizes[iCol-1].Width + boxSize*2); 
							break;
						case LegendLayoutEnum.Table:
						case LegendLayoutEnum.Column:
						default:
							rect = new System.Drawing.Rectangle(x + boxSize + boxSize/2, y, maxTextWidth, h);
							g.DrawString(c, drawFont, drawBrush, rect, drawFormat);
							DrawLegendBox(g, SeriesBrush[iCol-1],
								bMarker? SeriesMarker[iCol-1]: ChartMarkerEnum.None, 
								x+1, y, boxSize);
							y += h;
							break;
					}
				}
			}
			finally
			{
				if (drawFont != null)
					drawFont.Dispose();
				if (drawBrush != null)
					drawBrush.Dispose();
				if (drawFormat != null)
					drawFormat.Dispose();
			}
			if (s != null)
				rRect = s.PaddingAdjust(rpt, null, rRect, true);
			return rRect;
		}

		void DrawLegendBox(Graphics g, Brush b, ChartMarkerEnum marker, int x, int y, int boxSize)
		{
			Pen p=null;
			int mSize= boxSize / 2;		// Marker size is 1/2 of box size	
			try
			{
				if (marker != ChartMarkerEnum.None)
				{
					p = new Pen(b, 1);
					g.DrawLine(p,  new Point(x, y + (boxSize + 1)/2), new Point(x + boxSize, y + (boxSize + 1)/2));
					x = x + (boxSize - mSize)/2;
					y = y + (boxSize - mSize)/2;
					if (mSize % 2 == 0)		
						mSize++;
				}
				if (marker == ChartMarkerEnum.None)
				{
					g.FillRectangle(b, x, y, boxSize, boxSize);
				}
				else
				{
					DrawLegendMarker(g, b, p, marker, x, y, mSize);
				}
			}
			finally
			{
				if (p != null)
					p.Dispose();
			}
		}

		internal void DrawLegendMarker(Graphics g, Brush b, Pen p, ChartMarkerEnum marker, int x, int y, int mSize)
		{
			Point[] points;
			switch (marker)
			{
				case ChartMarkerEnum.Circle:
					g.FillEllipse(b, x, y, mSize, mSize);
					break;
				case ChartMarkerEnum.Square:
					g.FillRectangle(b, x, y, mSize, mSize);
					break;
				case ChartMarkerEnum.Plus:
					g.DrawLine(p, new Point(x + (mSize + 1)/2, y), new Point(x + (mSize + 1)/2, y + mSize));
					g.DrawLine(p, new Point(x + (mSize + 1)/2, y + (mSize+1)/2), new Point(x + mSize, y + (mSize+1)/2));
					break;
				case ChartMarkerEnum.Diamond:
					points = new Point[5];
					points[0] = points[4] = new Point(x + (mSize + 1)/2, y);	// starting and ending point
					points[1] = new Point(x, y + (mSize+1)/2);
					points[2] = new Point(x + (mSize+1)/2, y+mSize);
					points[3] = new Point(x + mSize, y + (mSize+1)/2);
					g.FillPolygon(b, points);
					break;
				case ChartMarkerEnum.Triangle:
					points = new Point[4];
					points[0] = points[3] = new Point(x + (mSize + 1)/2, y);	// starting and ending point
					points[1] = new Point(x, y + mSize);
					points[2] = new Point(x + mSize, y + mSize);
					g.FillPolygon(b, points);
					break;
				case ChartMarkerEnum.X:
					g.DrawLine(p, new Point(x, y), new Point(x + mSize, y + mSize));
					g.DrawLine(p, new Point(x + mSize, y + mSize), new Point(x + mSize, y));
					break;
			}
			return;
		}

		// Measures the Legend and then returns the rectangle it drew in
		protected Size[] DrawLegendMeasure(Report rpt, Graphics g, Font f, StringFormat sf, SizeF maxSize, out int maxWidth, out int maxHeight)
		{
			Size[] sizes = new Size[SeriesCount];
			maxWidth = maxHeight = 0;

			for (int iCol=1; iCol <= SeriesCount; iCol++)
			{
				string c = GetSeriesValue(rpt, iCol);
				SizeF ms = g.MeasureString(c, f, maxSize, sf);
				sizes[iCol-1] = new Size((int) Math.Ceiling(ms.Width), 
										 (int) Math.Ceiling(ms.Height));
				if (sizes[iCol-1].Width > maxWidth)
					maxWidth = sizes[iCol-1].Width;
				if (sizes[iCol-1].Height > maxHeight)
					maxHeight = sizes[iCol-1].Height;
			}
			return sizes;
		}

		protected void DrawPlotAreaStyle(Report rpt, Graphics g, System.Drawing.Rectangle crect)
		{
			if (_ChartDefn.PlotArea == null || _ChartDefn.PlotArea.Style == null)
				return;
			System.Drawing.Rectangle rect = Layout.PlotArea;
			Style s = _ChartDefn.PlotArea.Style;

            Row r = FirstChartRow(rpt);

			if (rect.IntersectsWith(crect))
			{
				// This occurs when the legend is drawn inside the plot area
				//    we don't want to draw in the legend
				Region rg=null;
				try
				{
	//				rg = new Region(rect);	// TODO: this doesn't work; nothing draws
	//				rg.Complement(crect);
	//				Region saver = g.Clip;
	//				g.Clip = rg;
					s.DrawBackground(rpt, g, r, rect);
	//				g.Clip = saver;
				}
				finally
				{
					if (rg != null)
						rg.Dispose();
				}
			}
			else
				s.DrawBackground(rpt, g, r, rect);
			
			return;
		}

		protected void DrawTitle(Report rpt, Graphics g, Title t, System.Drawing.Rectangle rect)
		{
			if (t == null)
				return;

			if (t.Caption == null)
				return;

			Row r = FirstChartRow(rpt);
			object title = t.Caption.Evaluate(rpt, r);
			if (t.Style != null)
			{
				t.Style.DrawString(rpt, g, title, t.Caption.GetTypeCode(), r, rect);
				t.Style.DrawBorder(rpt, g, r, rect);
			}
			else
				Style.DrawStringDefaults(g, title, rect);

			return;
		}

		protected Size DrawTitleMeasure(Report rpt, Graphics g, Title t)
		{
			Size size=Size.Empty;

			if (t == null || t.Caption == null)
				return size;

			Row r = FirstChartRow(rpt);
			object title = t.Caption.Evaluate(rpt, r);
			if (t.Style != null)
				size = t.Style.MeasureString(rpt, g, title, t.Caption.GetTypeCode(), r, int.MaxValue);
			else
				size = Style.MeasureStringDefaults(rpt, g, title, t.Caption.GetTypeCode(), r, int.MaxValue);
			
			return size;
		}

		protected object GetCategoryValue(Report rpt, int row, out TypeCode tc)
		{
			MatrixCellEntry mce = _DataDefn[row, 0];
			if (mce == null)
			{
				tc = TypeCode.String;
				return "";					// Not sure what this really means TODO:
			}

			Row lrow;
			this._ChartDefn.ChartMatrix.SetMyData(rpt, mce.Data);		// Must set this for evaluation
			if (mce.Data.Data.Count > 0)
				lrow = mce.Data.Data[0];
			else
				lrow = null;
			ChartExpression ce = (ChartExpression) (mce.DisplayItem);

			object v = ce.Value.Evaluate(rpt, lrow);
			tc = ce.Value.GetTypeCode();
			return v;
		}

		protected double GetDataValue(Report rpt, int row, int col)
		{
			MatrixCellEntry mce = _DataDefn[row, col];
			if (mce == null)
				return 0;					// Not sure what this really means TODO:
			if (mce.Value != double.MinValue)
				return mce.Value;

			// Calculate this value; usually a fairly expensive operation
			//   due to the common use of aggregate values.  We need to
			//   go thru the data more than once if we have to auto scale.
			Row lrow;
			this._ChartDefn.ChartMatrix.SetMyData(rpt, mce.Data);		// Must set this for evaluation
			if (mce.Data.Data.Count > 0)
				lrow = mce.Data.Data[0];
			else
				lrow = null;
			ChartExpression ce = (ChartExpression) (mce.DisplayItem);

			double v = ce.Value.EvaluateDouble(rpt, lrow);
			mce.Value = v;					// cache so we don't need to calculate again
			return v;
		}

		protected void DrawDataPoint(Report rpt, Graphics g, Point p, int row, int col)
		{
			DrawDataPoint(rpt, g, p, System.Drawing.Rectangle.Empty, row, col);
		}

		protected void DrawDataPoint(Report rpt, Graphics g, System.Drawing.Rectangle rect, int row, int col)
		{
			DrawDataPoint(rpt, g, Point.Empty, rect, row, col);
		}

		void DrawDataPoint(Report rpt, Graphics g, Point p, System.Drawing.Rectangle rect, int row, int col)
		{
			MatrixCellEntry mce = _DataDefn[row, col];
			if (mce == null)
				return;					// Not sure what this really means TODO:

			ChartExpression ce = (ChartExpression) (mce.DisplayItem);
			DataPoint dp = ce.DP;

			if (dp.DataLabel == null || !dp.DataLabel.Visible)
				return;

			// Calculate the DataPoint value; usually a fairly expensive operation
			//   due to the common use of aggregate values.  
			Row lrow;
			this._ChartDefn.ChartMatrix.SetMyData(rpt, mce.Data);		// Must set this for evaluation
			if (mce.Data.Data.Count > 0)
				lrow = mce.Data.Data[0];
			else
				lrow = null;

			object v=null;
			TypeCode tc;
			if (dp.DataLabel.Value == null)
			{		// No DataLabel value specified so we use the actual value
				v = ce.Value.EvaluateDouble(rpt, lrow);
				tc = TypeCode.Double;
			}
			else
			{		// Evaluate the DataLable value for the display
				v = dp.DataLabel.Value.Evaluate(rpt, lrow);
				tc = dp.DataLabel.Value.GetTypeCode();
			}

			if (dp.DataLabel.Style == null)
			{
				if (rect == System.Drawing.Rectangle.Empty)
				{
					Size size = Style.MeasureStringDefaults(rpt, g, v, tc, lrow, int.MaxValue);
					rect = new System.Drawing.Rectangle(p, size);
				}
				Style.DrawStringDefaults(g, v, rect);
			}
			else
			{
				if (rect == System.Drawing.Rectangle.Empty)
				{
					Size size = dp.DataLabel.Style.MeasureString(rpt, g, v, tc, lrow, int.MaxValue);
					rect = new System.Drawing.Rectangle(p, size);
				}
				dp.DataLabel.Style.DrawString(rpt, g, v, tc, lrow, rect);
			}

			return;
		}

		protected string GetSeriesValue(Report rpt, int iCol)
		{
			MatrixCellEntry mce = _DataDefn[0, iCol];
			Row lrow;
			if (mce.Data.Data.Count > 0)
				lrow = mce.Data.Data[0];
			else
				lrow = null;
			ChartExpression ce = (ChartExpression) (mce.DisplayItem);

			string v = ce.Value.EvaluateString(rpt, lrow);
			return v;
		}

		protected void GetMaxMinDataValue(Report rpt, out double max, out double min)
		{
			if (_ChartDefn.Subtype == ChartSubTypeEnum.Stacked)
			{
				GetMaxMinDataValueStacked(rpt, out max, out min);
				return;
			}
			min = double.MaxValue;
			max = double.MinValue;

			double v;
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					v = GetDataValue(rpt, iRow, iCol);
					if (v < min)
						min = v;
					if (v > max)
						max = v;
				}
			}
		}

		void GetMaxMinDataValueStacked(Report rpt, out double max, out double min)
		{
			min = double.MaxValue;
			max = double.MinValue;

			double v;
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				v=0;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					v += GetDataValue(rpt, iRow, iCol);
				}
				if (v < min)
					min = v;
				if (v > max)
					max = v;
			}
		}
		
		protected Brush[] GetSeriesBrushes()
		{
			Brush[] b = new Brush[SeriesCount];

			for (int i=0; i < SeriesCount; i++)
			{
				// TODO: In general all the palettes could use a good going over
				//   both in terms of the colors in the lists and their order
				switch (ChartDefn.Palette)
				{
					case ChartPaletteEnum.Default:
						b[i] = GetSeriesBrushesExcel(i); break;
					case ChartPaletteEnum.EarthTones:
						b[i] = GetSeriesBrushesEarthTones(i); break;
					case ChartPaletteEnum.Excel:
						b[i] = GetSeriesBrushesExcel(i); break;
					case ChartPaletteEnum.GrayScale:
						b[i] = GetSeriesBrushesGrayScale(i); break;
					case ChartPaletteEnum.Light:
						b[i] = GetSeriesBrushesLight(i); break;
					case ChartPaletteEnum.Pastel:
						b[i] = GetSeriesBrushesPastel(i); break;
					case ChartPaletteEnum.SemiTransparent:
						b[i] = GetSeriesBrushesExcel(i); break;	// TODO
					default:
						b[i] = GetSeriesBrushesExcel(i); break;
				}
			}

			return b;
		}

		Brush GetSeriesBrushesEarthTones(int i)
		{
			switch (i % 22)
			{
				case 0: return Brushes.Maroon;
				case 1: return Brushes.Brown;
				case 2: return Brushes.Chocolate;
				case 3: return Brushes.IndianRed;
				case 4: return Brushes.Peru;
				case 5: return Brushes.BurlyWood;
				case 6: return Brushes.AntiqueWhite;
				case 7: return Brushes.FloralWhite;
				case 8: return Brushes.Ivory;
				case 9: return Brushes.LightCoral;
				case 10:return Brushes.DarkSalmon;
				case 11: return Brushes.LightSalmon;
				case 12: return Brushes.PeachPuff;
				case 13: return Brushes.NavajoWhite;
				case 14: return Brushes.Moccasin;
				case 15: return Brushes.PapayaWhip;
				case 16: return Brushes.Goldenrod;
				case 17: return Brushes.DarkGoldenrod;
				case 18: return Brushes.DarkKhaki;
				case 19: return Brushes.Khaki;
				case 20: return Brushes.Beige;
				case 21: return Brushes.Cornsilk;
				default: return Brushes.Brown;
			}
		}

		Brush GetSeriesBrushesExcel(int i)
		{
			switch (i % 11)				// Just a guess at what these might actually be
			{
				case 0: return Brushes.DarkBlue;
				case 1: return Brushes.Pink;
				case 2: return Brushes.Yellow;
				case 3: return Brushes.Turquoise;
				case 4: return Brushes.Violet;
				case 5: return Brushes.DarkRed;
				case 6: return Brushes.Teal;
				case 7: return Brushes.Blue;
				case 8: return Brushes.Plum;
				case 9: return Brushes.Ivory;
				case 10: return Brushes.Coral;
				default: return Brushes.DarkBlue;
			}
		}

		Brush GetSeriesBrushesGrayScale(int i)
		{
			switch (i % 10)			
			{
				case 0: return Brushes.Gray;
				case 1: return Brushes.SlateGray;
				case 2: return Brushes.DarkGray;
				case 3: return Brushes.LightGray;
				case 4: return Brushes.DarkSlateGray;
				case 5: return Brushes.DimGray;
				case 6: return Brushes.LightSlateGray;
				case 7: return Brushes.Black;
				case 8: return Brushes.White;
				case 9: return Brushes.Gainsboro;
				default: return Brushes.Gray;
			}
		}

		Brush GetSeriesBrushesLight(int i)
		{
			switch (i % 13)
			{
				case 0: return Brushes.LightBlue;
				case 1: return Brushes.LightCoral;
				case 2: return Brushes.LightCyan;
				case 3: return Brushes.LightGoldenrodYellow;
				case 4: return Brushes.LightGray;
				case 5: return Brushes.LightGreen;
				case 6: return Brushes.LightPink;
				case 7: return Brushes.LightSalmon;
				case 8: return Brushes.LightSeaGreen;
				case 9: return Brushes.LightSkyBlue;
				case 10: return Brushes.LightSlateGray;
				case 11: return Brushes.LightSteelBlue;
				case 12: return Brushes.LightYellow;
				default: return Brushes.LightBlue;
			}
		}

		Brush GetSeriesBrushesPastel(int i)
		{
			switch (i % 26)	
			{
				case 0: return Brushes.CadetBlue;
				case 1: return Brushes.MediumTurquoise;
				case 2: return Brushes.Aquamarine;
				case 3: return Brushes.LightCyan;
				case 4: return Brushes.Azure;
				case 5: return Brushes.AliceBlue;
				case 6: return Brushes.MintCream;
				case 7: return Brushes.DarkSeaGreen;
				case 8: return Brushes.PaleGreen;
				case 9: return Brushes.LightGreen;
				case 10: return Brushes.MediumPurple;
				case 11: return Brushes.CornflowerBlue;
				case 12: return Brushes.Lavender;
				case 13: return Brushes.GhostWhite;
				case 14: return Brushes.PaleGoldenrod;
				case 15: return Brushes.LightGoldenrodYellow;
				case 16: return Brushes.LemonChiffon;
				case 17: return Brushes.LightYellow;
				case 18: return Brushes.Orchid;
				case 19: return Brushes.Plum;
				case 20: return Brushes.LightPink;
				case 21: return Brushes.Pink;
				case 22: return Brushes.LavenderBlush;
				case 23: return Brushes.Linen;
				case 24: return Brushes.PaleTurquoise;
				case 25: return Brushes.OldLace;
				default: return Brushes.CadetBlue;
			}
		}

		protected ChartMarkerEnum[] GetSeriesMarkers()
		{
			ChartMarkerEnum[] m = new ChartMarkerEnum[SeriesCount];

			for (int i=0; i < SeriesCount; i++)
			{
				m[i] = (ChartMarkerEnum) ( i % (int) ChartMarkerEnum.Count);
			}

			return m;
		}

		protected void GetValueMaxMin(Report rpt, ref double max, ref double min)
		{

			if (_ChartDefn.Subtype == ChartSubTypeEnum.PercentStacked)
			{	// Percent stacked is easy; and overrides user provided values
				max = 1;
				min = 0;
				return;
			}
			int valueAxisMax;
			int valueAxisMin;
			if (_ChartDefn.ValueAxis != null &&
				_ChartDefn.ValueAxis.Axis != null)
			{
				valueAxisMax = _ChartDefn.ValueAxis.Axis.MaxEval(rpt, _row);
				valueAxisMin = _ChartDefn.ValueAxis.Axis.MinEval(rpt, _row);
			}
			else
			{
				valueAxisMax = valueAxisMin = int.MinValue;
			}

			// Check for case where both min and max are provided
			if (valueAxisMax != int.MinValue &&
				valueAxisMin != int.MinValue)
			{
				max = valueAxisMax;
				min = valueAxisMin;
				return;
			}

			// OK We have to work for it;  Calculate min/max of data
			GetMaxMinDataValue(rpt, out max, out min);	
			
			if (valueAxisMax != int.MinValue)
				max = valueAxisMax;
			else
			{
				//
				int gridIncrs=10;		// assume 10 grid increments for now
				double incr = max / gridIncrs;	// should be range between max and min?
				double log = Math.Floor(Math.Log10(Math.Abs(incr)));

				double logPow = Math.Pow(10, log) * Math.Sign(max);
				double logDig = (int) (incr / logPow + .5);

				// promote the MSD to either 1, 2, or 5
				if ( logDig > 5.0 )
					logDig = 10.0;
				else if ( logDig > 2.0 )
					logDig = 5.0;
				else if ( logDig > 1.0 )
					logDig = 2.0;
				
				max = logDig * logPow * gridIncrs;
			}

			if (valueAxisMin != int.MinValue)
				min = valueAxisMin;
			else if (min > 0)
				min = 0;
			else
			{
				min = Math.Floor(min);
			}

			return;
		}

		protected void AdjustMargins(System.Drawing.Rectangle legendRect)
		{
			// Adjust the margins based on the legend
			if (!IsLegendInsidePlotArea())	// When inside plot area we don't adjust plot margins
			{
				if (IsLegendLeft())
					Layout.LeftMargin += legendRect.Width;
				else if (IsLegendRight())
					Layout.RightMargin += legendRect.Width;
				if (IsLegendTop())
					Layout.TopMargin += legendRect.Height;
				else if (IsLegendBottom())
					Layout.BottomMargin += legendRect.Height;
			}
			// Force some margins; if any are too small
			int min = new RSize(ChartDefn.OwnerReport, ".2 in").PixelsX;

			if (Layout.RightMargin < min + this._LastCategoryWidth/2)
				Layout.RightMargin = min + this._LastCategoryWidth/2;
			if (Layout.LeftMargin < min)
				Layout.LeftMargin = min;
			if (Layout.TopMargin < min)
				Layout.TopMargin = min;
			if (Layout.BottomMargin < min)
				Layout.BottomMargin = min;
		}

		protected bool IsLegendLeft()
		{
			Legend l = _ChartDefn.Legend;
			if (l == null || !l.Visible)
				return false;

			bool rc;
			switch (l.Position)
			{
				case LegendPositionEnum.BottomLeft:
				case LegendPositionEnum.LeftBottom:
				case LegendPositionEnum.LeftCenter:
				case LegendPositionEnum.LeftTop:
				case LegendPositionEnum.TopLeft:
					rc=true;
					break;
				default:
					rc=false;
					break;
			}

			return rc;
		}

		protected bool IsLegendInsidePlotArea()
		{
			Legend l = _ChartDefn.Legend;
			if (l == null || !l.Visible)
				return false;				// doesn't really matter
			else
				return l.InsidePlotArea;
		}

		protected bool IsLegendRight()
		{
			Legend l = _ChartDefn.Legend;
			if (l == null || !l.Visible)
				return false;

			bool rc;
			switch (l.Position)
			{
				case LegendPositionEnum.BottomRight:
				case LegendPositionEnum.RightBottom:
				case LegendPositionEnum.RightCenter:
				case LegendPositionEnum.TopRight:
				case LegendPositionEnum.RightTop:
					rc=true;
					break;
				default:
					rc=false;
					break;
			}
			return rc;
		}

		protected bool IsLegendTop()
		{
			Legend l = _ChartDefn.Legend;
			if (l == null || !l.Visible)
				return false;

			bool rc;
			switch (l.Position)
			{
				case LegendPositionEnum.LeftTop:
				case LegendPositionEnum.TopLeft:
				case LegendPositionEnum.TopCenter:
				case LegendPositionEnum.TopRight:
				case LegendPositionEnum.RightTop:
					rc=true;
					break;
				default:
					rc=false;
					break;
			}
			return rc;
		}

		protected bool IsLegendBottom()
		{
			Legend l = _ChartDefn.Legend;
			if (l == null || !l.Visible)
				return false;

			bool rc;
			switch (l.Position)
			{
				case LegendPositionEnum.BottomCenter:
				case LegendPositionEnum.BottomLeft:
				case LegendPositionEnum.LeftBottom:
				case LegendPositionEnum.BottomRight:
				case LegendPositionEnum.RightBottom:
					rc=true;
					break;
				default:
					rc=false;
					break;
			}
			return rc;
		}

		private Row FirstChartRow(Report rpt)
		{
			Rows _Data = _ChartDefn.ChartMatrix.GetMyData(rpt);
			if (_Data != null &&
				_Data.Data.Count > 0)
				return _Data.Data[0];
			else
				return null;

		}
		#region IDisposable Members

		public void Dispose()
		{
			if (_bm != null)
				_bm.Dispose();
		}

		#endregion
	}
}
