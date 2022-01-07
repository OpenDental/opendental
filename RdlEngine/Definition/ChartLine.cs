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
	/// Line chart definition and processing.
	///</summary>
	[Serializable]
	internal class ChartLine: ChartColumn
	{

		internal ChartLine(Report r, Row row, Chart c, MatrixCellEntry[,] m) : base(r, row, c, m)
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
				DrawTitle(rpt, g, ChartDefn.Title, new System.Drawing.Rectangle(0, 0, Layout.Width, Layout.TopMargin));

				// Adjust the left margin to depend on the Value Axis
				Size vaSize = ValueAxisSize(rpt, g, min, max);
				Layout.LeftMargin = vaSize.Width;

				// Draw legend
				System.Drawing.Rectangle lRect = DrawLegend(rpt,g, ChartDefn.Type == ChartTypeEnum.Area? false: true, true);

				// Adjust the bottom margin to depend on the Category Axis
				Size caSize = CategoryAxisSize(rpt, g);
				Layout.BottomMargin = caSize.Height;

				AdjustMargins(lRect);		// Adjust margins based on legend.

				// Draw Plot area
				DrawPlotAreaStyle(rpt, g, lRect);

				// Draw Value Axis
				if (vaSize.Width > 0)	// If we made room for the axis - we need to draw it
					DrawValueAxis(rpt, g, min, max, 
						new System.Drawing.Rectangle(Layout.LeftMargin - vaSize.Width, Layout.TopMargin, vaSize.Width, Layout.PlotArea.Height), Layout.LeftMargin, _bm.Width - Layout.RightMargin);

				// Draw Category Axis
				if (caSize.Height > 0)
					DrawCategoryAxis(rpt, g,  
						new System.Drawing.Rectangle(Layout.LeftMargin, _bm.Height-Layout.BottomMargin, Layout.PlotArea.Width, caSize.Height));

				// Draw Plot area data 
				if (ChartDefn.Type == ChartTypeEnum.Area)
				{
					if (ChartDefn.Subtype == ChartSubTypeEnum.Stacked)
						DrawPlotAreaAreaStacked(rpt, g, max);
					else if (ChartDefn.Subtype == ChartSubTypeEnum.PercentStacked)
						DrawPlotAreaAreaPercentStacked(rpt, g);
					else
						DrawPlotAreaArea(rpt, g, max);
				}
				else
				{
					DrawPlotAreaLine(rpt, g, max);
				}
				DrawLegend(rpt, g, ChartDefn.Type == ChartTypeEnum.Area? false: true, false);
			}
		}

		void DrawPlotAreaArea(Report rpt, Graphics g, double max)
		{
			// Draw Plot area data 
			int maxPointHeight = (int) Layout.PlotArea.Height;	
			double widthCat = ((double) (Layout.PlotArea.Width) / (CategoryCount-1));
			Point[] saveP = new Point[CategoryCount];	// used for drawing lines between points
			for (int iCol=1; iCol <= SeriesCount; iCol++)
			{
				for (int iRow=1; iRow <= CategoryCount; iRow++)
				{
					double v = this.GetDataValue(rpt, iRow, iCol);

					int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat);
					int y = (int) ((Math.Min(v,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveP[iRow-1] = p;
					DrawLinePoint(rpt, g, SeriesBrush[iCol-1], ChartMarkerEnum.None, p, iRow, iCol );
				}
				DrawAreaBetweenPoints(g, SeriesBrush[iCol-1], saveP, null);
			}
			return;
		}

		void DrawPlotAreaAreaPercentStacked(Report rpt, Graphics g)
		{
			double max = 1;				// 100% is the max
			// Draw Plot area data 
			int maxPointHeight = (int) Layout.PlotArea.Height;	
			double widthCat = ((double) (Layout.PlotArea.Width) / (CategoryCount-1));
			Point[,] saveAllP = new Point[CategoryCount,SeriesCount];	// used to collect all data points

			// Loop thru calculating all the data points
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat);
				double sum=0;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					sum += GetDataValue(rpt, iRow, iCol);
				}
				double v=0;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					v += GetDataValue(rpt, iRow, iCol);

					int y = (int) ((Math.Min(v/sum,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveAllP[iRow-1, iCol-1] = p;
				}
			}

			// Now loop thru and plot all the points
			Point[] saveP = new Point[CategoryCount];	// used for drawing lines between points
			Point[] priorSaveP= new Point[CategoryCount];
			for (int iCol=1; iCol <= SeriesCount; iCol++)
			{
				for (int iRow=1; iRow <= CategoryCount; iRow++)
				{
					double v = this.GetDataValue(rpt, iRow, iCol);

					int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat);
					int y = (int) ((Math.Min(v,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveP[iRow-1] = saveAllP[iRow-1, iCol-1];
					DrawLinePoint(rpt, g, SeriesBrush[iCol-1], ChartMarkerEnum.None, p, iRow, iCol );
				}
				DrawAreaBetweenPoints(g, SeriesBrush[iCol-1], saveP, iCol==1?null:priorSaveP);
				// Save prior point values
				for (int i=0; i < CategoryCount; i++)
					priorSaveP[i] = saveP[i];
			}
			return;
		}

		void DrawPlotAreaAreaStacked(Report rpt, Graphics g, double max)
		{
			// Draw Plot area data 
			int maxPointHeight = (int) Layout.PlotArea.Height;	
			double widthCat = ((double) (Layout.PlotArea.Width) / (CategoryCount-1));
			Point[,] saveAllP = new Point[CategoryCount,SeriesCount];	// used to collect all data points

			// Loop thru calculating all the data points
			for (int iRow = 1; iRow <= CategoryCount; iRow++)
			{
				int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat);
				double v=0;
				for (int iCol = 1; iCol <= SeriesCount; iCol++)
				{
					v += GetDataValue(rpt, iRow, iCol);
					int y = (int) ((Math.Min(v,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveAllP[iRow-1, iCol-1] = p;
				}
			}

			// Now loop thru and plot all the points
			Point[] saveP = new Point[CategoryCount];	// used for drawing lines between points
			Point[] priorSaveP= new Point[CategoryCount];
			for (int iCol=1; iCol <= SeriesCount; iCol++)
			{
				for (int iRow=1; iRow <= CategoryCount; iRow++)
				{
					double v = this.GetDataValue(rpt, iRow, iCol);

					int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat);
					int y = (int) ((Math.Min(v,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveP[iRow-1] = saveAllP[iRow-1, iCol-1];
					DrawLinePoint(rpt, g, SeriesBrush[iCol-1], ChartMarkerEnum.None, p, iRow, iCol );
				}
				DrawAreaBetweenPoints(g, SeriesBrush[iCol-1], saveP, iCol==1?null:priorSaveP);
				// Save prior point values
				for (int i=0; i < CategoryCount; i++)
					priorSaveP[i] = saveP[i];
			}
			return;
		}

		void DrawPlotAreaLine(Report rpt, Graphics g, double max)
		{
			// Draw Plot area data 
			int maxPointHeight = (int) Layout.PlotArea.Height;	
			double widthCat = ((double) (Layout.PlotArea.Width) / CategoryCount);
			Point[] saveP = new Point[CategoryCount];	// used for drawing lines between points
			for (int iCol=1; iCol <= SeriesCount; iCol++)
			{
				for (int iRow=1; iRow <= CategoryCount; iRow++)
				{
					double v = this.GetDataValue(rpt, iRow, iCol);

					int x = (int) (Layout.PlotArea.Left + (iRow-1) * widthCat + widthCat/2 );
					int y = (int) ((Math.Min(v,max) / max) * maxPointHeight);
					Point p = new Point(x, Layout.PlotArea.Top + (maxPointHeight -  y));
					saveP[iRow-1] = p;
					DrawLinePoint(rpt, g, SeriesBrush[iCol-1], SeriesMarker[iCol-1], p, iRow, iCol );
				}
				DrawLineBetweenPoints(g, SeriesBrush[iCol-1], saveP);
			}
			return;
		}

		void DrawAreaBetweenPoints(Graphics g, Brush brush, Point[] points, Point[] previous)
		{
			if (points.Length <= 1)		// Need at least 2 points
				return;

			Pen p=null;
			try
			{
				p = new Pen(brush, 1);    // todo - use line from style ????
				g.DrawLines(p, points);
				Point[] poly;
				if (previous == null)
				{	// The bottom is the bottom of the chart
					poly = new Point[points.Length + 3];
					int i=0;
					foreach (Point pt in points)
					{
						poly[i++] = pt;
					}
					poly[i++] = new Point(points[points.Length-1].X, Layout.PlotArea.Bottom);
					poly[i++] = new Point(points[0].X, Layout.PlotArea.Bottom);
					poly[i] = new Point(points[0].X, points[0].Y); 
				}
				else
				{	// The bottom is the previous line
					poly = new Point[points.Length * 2 + 1];
					int i=0;
					foreach (Point pt in points)
					{
						poly[i] = pt;
						poly[points.Length+i] = previous[previous.Length - 1 - i];
						i++;
					}
					poly[poly.Length-1] = poly[0];
				}
				g.FillPolygon(brush, poly);
			}
			finally
			{
				if (p != null)
					p.Dispose();
			}
			return;
		}

		void DrawLineBetweenPoints(Graphics g, Brush brush, Point[] points)
		{
			if (points.Length <= 1)		// Need at least 2 points
				return;

			Pen p=null;
			try
			{
				p = new Pen(brush, 1);    // todo - use line from style ????
				if (ChartDefn.Subtype == ChartSubTypeEnum.Smooth && points.Length > 2)
					g.DrawCurve(p, points, 0.5F);
				else
					g.DrawLines(p, points);
			}
			finally
			{
				if (p != null)
					p.Dispose();
			}
			return;
		}

		void DrawLinePoint(Report rpt, Graphics g, Brush brush, ChartMarkerEnum marker, Point p, int iRow, int iCol)
		{
			Pen pen=null;
			try
			{
				pen = new Pen(brush);
				DrawLegendMarker(g, brush, pen, marker, p.X-3, p.Y-3, 7);
				DrawDataPoint(rpt, g, new Point(p.X-3, p.Y+3), iRow, iCol);
			}
			finally
			{
				if (pen != null)
					pen.Dispose();
			}

			return;
		}

	}
}
