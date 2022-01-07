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


namespace fyiReporting.RDL
{
	///<summary>
	/// Column chart definition and processing.
	///</summary>
	internal class ChartColumn: ChartBase
	{
		int _GapSize=12;		// TODO: hard code for now

		internal ChartColumn(Report r, Row row, Chart c, MatrixCellEntry[,] m) : base(r, row, c, m)
		{
		}

		override internal void Draw(Report rpt)
		{
			CreateSizedBitmap();

			using (Graphics g = Graphics.FromImage(_bm))
			{
				// Adjust the top margin to depend on the title height
				Size titleSize = DrawTitleMeasure(rpt, g, ChartDefn.Title);
				Layout.TopMargin = titleSize.Height;

				double max=0,min=0;	// Get the max and min values
				GetValueMaxMin(rpt, ref max, ref min);

				DrawChartStyle(rpt, g);
				
				// Draw title; routine determines if necessary
				DrawTitle(rpt, g, ChartDefn.Title, new System.Drawing.Rectangle(0, 0, _bm.Width, Layout.TopMargin));

				// Adjust the left margin to depend on the Value Axis
				Size vaSize = ValueAxisSize(rpt, g, min, max);
				Layout.LeftMargin = vaSize.Width;

				// Draw legend
				System.Drawing.Rectangle lRect = DrawLegend(rpt, g, false, true);

				// Adjust the bottom margin to depend on the Category Axis
				Size caSize = CategoryAxisSize(rpt, g);
				Layout.BottomMargin = caSize.Height;

				AdjustMargins(lRect);		// Adjust margins based on legend.

				// Draw Plot area
				DrawPlotAreaStyle(rpt, g, lRect);
																															   
				// Draw Value Axis
				if (vaSize.Width > 0)	// If we made room for the axis - we need to draw it
					DrawValueAxis(rpt, g, min, max, 
						new System.Drawing.Rectangle(Layout.LeftMargin - vaSize.Width, Layout.TopMargin, vaSize.Width, _bm.Height - Layout.TopMargin - Layout.BottomMargin), Layout.LeftMargin, Layout.Width - Layout.RightMargin);

				// Draw Category Axis
				if (caSize.Height > 0)
					DrawCategoryAxis(rpt, g,  
						new System.Drawing.Rectangle(Layout.LeftMargin, _bm.Height-Layout.BottomMargin, Layout.PlotArea.Width, caSize.Height));

				if (ChartDefn.Subtype == ChartSubTypeEnum.Stacked)
					DrawPlotAreaStacked(rpt, g, max, min);
				else if (ChartDefn.Subtype == ChartSubTypeEnum.PercentStacked)
					DrawPlotAreaPercentStacked(rpt, g);
				else
					DrawPlotAreaPlain(rpt, g, max, min);

				DrawLegend(rpt, g, false, false);

			}
		}

		void DrawPlotAreaPercentStacked(Report rpt, Graphics g)
		{
			int barsNeeded = CategoryCount; 
			int gapsNeeded = CategoryCount * 2;

			// Draw Plot area data
			double max = 1;

			int widthBar = (int) ((Layout.PlotArea.Width - gapsNeeded*_GapSize) / barsNeeded);
			int maxBarHeight = (int) (Layout.PlotArea.Height);	

			// Loop thru calculating all the data points
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				int barLoc=(int) (Layout.PlotArea.Left + (iRow-1) * ((double) (Layout.PlotArea.Width) / CategoryCount));
				barLoc += _GapSize;	// space before series

				double sum=0;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					double t = GetDataValue(rpt, iRow, iCol);
					if (t.CompareTo(double.NaN) == 0)
						t = 0;
					sum += t;
				}
				double v=0;
				Point saveP=Point.Empty;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					double t = GetDataValue(rpt, iRow, iCol);
					if (t.CompareTo(double.NaN)==0)
						t = 0;
					v += t;

					int h = (int) ((Math.Min(v/sum,max) / max) * maxBarHeight);
					Point p = new Point(barLoc, Layout.PlotArea.Top + (maxBarHeight -  h));

					System.Drawing.Rectangle rect;
					if (saveP == Point.Empty)
						rect = new System.Drawing.Rectangle(p, new Size(widthBar,h));
					else
						rect = new System.Drawing.Rectangle(p, new Size(widthBar, saveP.Y - p.Y));
					DrawColumnBar(rpt, g, SeriesBrush[iCol-1], rect, iRow, iCol);

					saveP = p;
				}
			}

			return;
		}


		void DrawPlotAreaPlain(Report rpt, Graphics g, double max, double min)
		{
			int barsNeeded = SeriesCount * CategoryCount; 
			int gapsNeeded = CategoryCount * 2;

			// Draw Plot area data
			int widthBar = (int) ((Layout.PlotArea.Width - gapsNeeded*_GapSize) / barsNeeded);
			int maxBarHeight = (int) (Layout.PlotArea.Height);	

			//int barLoc=Layout.LeftMargin;
			for (int iRow=1; iRow <= CategoryCount; iRow++)
			{
				int barLoc=(int) (Layout.PlotArea.Left + (iRow-1) * ((double) (Layout.PlotArea.Width) / CategoryCount));

				barLoc += _GapSize;	// space before series
				for (int iCol=1; iCol <= SeriesCount; iCol++)
				{
					double v = this.GetDataValue(rpt, iRow, iCol);
					if (v.CompareTo(double.NaN) == 0)   
						v = min;
					int h = (int) ((Math.Min(v,max) / max) * maxBarHeight);
						
					DrawColumnBar(rpt, g, SeriesBrush[iCol-1], 
						new System.Drawing.Rectangle(barLoc, Layout.PlotArea.Top + (maxBarHeight -  h), widthBar, h), iRow, iCol);

					barLoc += widthBar;
				}
			}
		}

		void DrawPlotAreaStacked(Report rpt, Graphics g, double max, double min)
		{
			int barsNeeded = CategoryCount; 
			int gapsNeeded = CategoryCount * 2;

			int widthBar = (int) ((Layout.PlotArea.Width - gapsNeeded*_GapSize) / barsNeeded);
			int maxBarHeight = (int) (Layout.PlotArea.Height);	

			// Loop thru calculating all the data points
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				int barLoc=(int) (Layout.PlotArea.Left + (iRow-1) * ((double) (Layout.PlotArea.Width) / CategoryCount));
				barLoc += _GapSize;	// space before series

				double v=0;
				Point saveP=Point.Empty;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					double t = GetDataValue(rpt, iRow, iCol);
					if (t.CompareTo(double.NaN) == 0)
						t = 0;
					v += t;

					int h = (int) ((Math.Min(v,max) / max) * maxBarHeight);
					Point p = new Point(barLoc, Layout.PlotArea.Top + (maxBarHeight -  h));

					System.Drawing.Rectangle rect;
					if (saveP == Point.Empty)
						rect = new System.Drawing.Rectangle(p, new Size(widthBar,h));
					else
						rect = new System.Drawing.Rectangle(p, new Size(widthBar, saveP.Y - p.Y));
					DrawColumnBar(rpt, g, SeriesBrush[iCol-1], rect, iRow, iCol);

					saveP = p;
				}
			}

			return;
		}

		// Calculate the size of the category axis
		protected Size CategoryAxisSize(Report rpt, Graphics g)
		{
			_LastCategoryWidth = 0;

			Size size=Size.Empty;
			if (this.ChartDefn.CategoryAxis == null)
				return size;
			Axis a = this.ChartDefn.CategoryAxis.Axis;
			if (a == null)
				return size;
			Style s = a.Style;

			// Measure the title
			size = DrawTitleMeasure(rpt, g, a.Title);

			if (!a.Visible)		// don't need to calculate the height
				return size;

			// Calculate the tallest category name
			TypeCode tc;
			int maxHeight=0;
			for (int iRow=1; iRow <= CategoryCount; iRow++)
			{
				object v = this.GetCategoryValue(rpt, iRow, out tc);
				Size tSize;
				if (s == null)
					tSize = Style.MeasureStringDefaults(rpt, g, v, tc, null, int.MaxValue);
				else
					tSize =s.MeasureString(rpt, g, v, tc, null, int.MaxValue);

				if (tSize.Height > maxHeight)
					maxHeight = tSize.Height;

				if (iRow == CategoryCount)
					_LastCategoryWidth = tSize.Width;
			}

			// Add on the tallest category name
			size.Height += maxHeight;

			// Account for tick marks
			int tickSize=0;
			if (a.MajorTickMarks == AxisTickMarksEnum.Cross ||
				a.MajorTickMarks == AxisTickMarksEnum.Outside)
				tickSize = this.AxisTickMarkMajorLen;
			else if (a.MinorTickMarks == AxisTickMarksEnum.Cross ||
				a.MinorTickMarks == AxisTickMarksEnum.Outside)
				tickSize = this.AxisTickMarkMinorLen;

			size.Height += tickSize;
			return size;
		}

		// DrawCategoryAxis 
		protected void DrawCategoryAxis(Report rpt, Graphics g, System.Drawing.Rectangle rect)
		{
			if (this.ChartDefn.CategoryAxis == null)
				return;
			Axis a = this.ChartDefn.CategoryAxis.Axis;
			if (a == null)
				return;
			Style s = a.Style;

			Size tSize = DrawTitleMeasure(rpt, g, a.Title);
			DrawTitle(rpt, g, a.Title, new System.Drawing.Rectangle(rect.Left, rect.Bottom-tSize.Height, rect.Width, tSize.Height));

			// Account for tick marks
			int tickSize=0;
			if (a.MajorTickMarks == AxisTickMarksEnum.Cross ||
				a.MajorTickMarks == AxisTickMarksEnum.Outside)
				tickSize = this.AxisTickMarkMajorLen;
			else if (a.MinorTickMarks == AxisTickMarksEnum.Cross ||
				a.MinorTickMarks == AxisTickMarksEnum.Outside)
				tickSize = this.AxisTickMarkMinorLen;

			int drawWidth;
			int catCount = ChartDefn.Type == ChartTypeEnum.Area? CategoryCount-1: CategoryCount;
			drawWidth = rect.Width / catCount;

			TypeCode tc;
			for (int iRow=1; iRow <= CategoryCount; iRow++)
			{
				object v = this.GetCategoryValue(rpt, iRow, out tc);

				int drawLoc=(int) (rect.Left + (iRow-1) * ((double) rect.Width / catCount));

				// Draw the category text
				if (a.Visible)
				{
					System.Drawing.Rectangle drawRect;
					if (ChartDefn.Type == ChartTypeEnum.Area)
					{	// Area chart - value is centered under the tick mark
						if (s != null)
						{
							Size size = s.MeasureString(rpt, g, v, TypeCode.Double, null, int.MaxValue);
							drawRect = 
								new System.Drawing.Rectangle(drawLoc - size.Width/2, rect.Top+tickSize, size.Width, size.Height);
						}
						else
						{
							Size size = Style.MeasureStringDefaults(rpt, g, v, TypeCode.Double, null, int.MaxValue);
							drawRect = 
								new System.Drawing.Rectangle(drawLoc - size.Width/2, rect.Top+tickSize, size.Width, size.Height);
						}
					}
					else	// Column/Line charts are just centered in the region.
						drawRect = new System.Drawing.Rectangle(drawLoc, rect.Top+tickSize, drawWidth, rect.Height-tSize.Height);

					if (s == null)
						Style.DrawStringDefaults(g, v, drawRect);
					else
						s.DrawString(rpt, g, v, tc, null, drawRect);
				}
				// Draw the Major Tick Marks (if necessary)
				DrawCategoryAxisTick(g, true, a.MajorTickMarks, new Point(drawLoc, rect.Top));
			}

			// Draw the end on (if necessary)
			DrawCategoryAxisTick(g, true, a.MajorTickMarks, new Point(rect.Right, rect.Top));

			return;
		}

		protected void DrawCategoryAxisTick(Graphics g, bool bMajor, AxisTickMarksEnum tickType, Point p)
		{
			int len = bMajor? AxisTickMarkMajorLen: AxisTickMarkMinorLen;
			switch (tickType)
			{
				case AxisTickMarksEnum.Outside:
					g.DrawLine(Pens.Black, new Point(p.X, p.Y), new Point(p.X, p.Y+len));
					break;
				case AxisTickMarksEnum.Inside:
					g.DrawLine(Pens.Black, new Point(p.X, p.Y), new Point(p.X, p.Y-len));
					break;
				case AxisTickMarksEnum.Cross:
					g.DrawLine(Pens.Black, new Point(p.X, p.Y-len), new Point(p.X, p.Y+len));
					break;
				case AxisTickMarksEnum.None:
				default:
					break;
			}
			return;
		}

		void DrawColumnBar(Report rpt, Graphics g, Brush brush, System.Drawing.Rectangle rect, int iRow, int iCol)
		{
			if (rect.Height <= 0)
				return;
			g.FillRectangle(brush, rect);
			g.DrawRectangle(Pens.Black, rect);

			if (ChartDefn.Subtype == ChartSubTypeEnum.Stacked ||
				ChartDefn.Subtype == ChartSubTypeEnum.PercentStacked)
			{
				DrawDataPoint(rpt, g, rect, iRow, iCol);
			}
			else
			{
				Point p;
				p = new Point(rect.Left, rect.Top - 14); // todo: 14 is arbitrary
				DrawDataPoint(rpt, g, p, iRow, iCol);
			}

			return;
		}

		protected void DrawValueAxis(Report rpt, Graphics g, double min, double max, 
						System.Drawing.Rectangle rect, int plotLeft, int plotRight)
		{
			if (this.ChartDefn.ValueAxis == null)
				return;
			Axis a = this.ChartDefn.ValueAxis.Axis;
			if (a == null)
				return;
			Style s = a.Style;

			double incr = (max - min) / 10;
	
			Size tSize = DrawTitleMeasure(rpt, g, a.Title);
			DrawTitle(rpt, g, a.Title, new System.Drawing.Rectangle(rect.Left, rect.Top, tSize.Width, rect.Height));

			double v = max;
			for (int i=0; i < 11; i++)	// TODO: hard coding 11 is too simplistic 
			{
				int h = (int) ((Math.Min(v,max) / max) * rect.Height);
				if (h < 0)		// this is really some form of error
				{
					v -= incr;
					continue;
				}

				if (!a.Visible)
				{
					// nothing to do
				}
				else if (s != null)
				{
					Size size = s.MeasureString(rpt, g, v, TypeCode.Double, null, int.MaxValue);
					System.Drawing.Rectangle vRect = 
						new System.Drawing.Rectangle(rect.Left+tSize.Width, rect.Top + rect.Height - h - size.Height/2, rect.Width-tSize.Width, size.Height);
					s.DrawString(rpt, g, v, TypeCode.Double, null, vRect);
				}
				else
				{
					Size size = Style.MeasureStringDefaults(rpt, g, v, TypeCode.Double, null, int.MaxValue);
					System.Drawing.Rectangle vRect = 
						new System.Drawing.Rectangle(rect.Left+tSize.Width, rect.Top + rect.Height - h - size.Height/2, rect.Width-tSize.Width, size.Height);
					Style.DrawStringDefaults(g, v, vRect);
				}

				DrawValueAxisGrid(rpt, g, a.MajorGridLines, new Point(plotLeft, rect.Top + rect.Height - h), new Point(plotRight, rect.Top + rect.Height - h));
				DrawValueAxisTick(rpt, g, true, a.MajorTickMarks, a.MajorGridLines, new Point(plotLeft, rect.Top + rect.Height - h));

				v -= incr;
			}

			// Draw the end points of the major grid lines
			DrawValueAxisGrid(rpt, g, a.MajorGridLines, new Point(plotLeft, rect.Top), new Point(plotLeft, rect.Bottom));
			DrawValueAxisTick(rpt, g, true, a.MajorTickMarks, a.MajorGridLines, new Point(plotLeft, rect.Top));
			DrawValueAxisGrid(rpt, g, a.MajorGridLines, new Point(plotRight, rect.Top), new Point(plotRight, rect.Bottom));
			DrawValueAxisTick(rpt, g, true, a.MajorTickMarks, a.MajorGridLines, new Point(plotRight, rect.Bottom));

			return;
		}

		protected void DrawValueAxisGrid(Report rpt, Graphics g, ChartGridLines gl, Point s, Point e)
		{
			if (gl == null || !gl.ShowGridLines)
				return;

			if (gl.Style != null)
				gl.Style.DrawStyleLine(rpt, g, null, s, e);
			else
				g.DrawLine(Pens.Black, s, e);

			return;
		}

		protected void DrawValueAxisTick(Report rpt, Graphics g, bool bMajor, AxisTickMarksEnum tickType, ChartGridLines gl, Point p)
		{
			if (tickType == AxisTickMarksEnum.None)
				return;

			int len = bMajor? AxisTickMarkMajorLen: AxisTickMarkMinorLen;
			Point s, e;
			switch (tickType)
			{
				case AxisTickMarksEnum.Inside:
					s = new Point(p.X, p.Y); 
					e = new Point(p.X+len, p.Y);
					break;
				case AxisTickMarksEnum.Cross:
					s = new Point(p.X-len, p.Y);
					e = new Point(p.X+len, p.Y);
					break;
				case AxisTickMarksEnum.Outside:
				default:
					s = new Point(p.X-len, p.Y);
					e = new Point(p.X, p.Y);
					break;
			}
			Style style = gl.Style;

			if (style != null)
				style.DrawStyleLine(rpt, g, null, s, e);
			else
				g.DrawLine(Pens.Black, s, e);

			return;
		}

		// Calculate the size of the value axis; width is max value width + title width
		//										 height is max value height
		protected Size ValueAxisSize(Report rpt, Graphics g, double min, double max)
		{
			Size size=Size.Empty;
			if (ChartDefn.ValueAxis == null)
				return size;
			Axis a = ChartDefn.ValueAxis.Axis;
			if (a == null)
				return size;

			Size minSize;
			Size maxSize;
			if (!a.Visible)
			{
				minSize = maxSize = Size.Empty;
			}
			else if (a.Style != null)
			{
				minSize = a.Style.MeasureString(rpt, g, min, TypeCode.Double, null, int.MaxValue);
				maxSize = a.Style.MeasureString(rpt, g, max, TypeCode.Double, null, int.MaxValue);
			}
			else
			{
				minSize = Style.MeasureStringDefaults(rpt, g, min, TypeCode.Double, null, int.MaxValue);
				maxSize = Style.MeasureStringDefaults(rpt, g, max, TypeCode.Double, null, int.MaxValue);
			}
			// Choose the largest
			size.Width = Math.Max(minSize.Width, maxSize.Width);
			size.Height = Math.Max(minSize.Height, maxSize.Height);

			// Now we need to add in the width of the title (if any)
			Size titleSize = DrawTitleMeasure(rpt, g, a.Title);
			size.Width += titleSize.Width;

			return size;
		}
	}
}
