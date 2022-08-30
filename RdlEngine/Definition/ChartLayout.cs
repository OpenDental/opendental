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
using System.Drawing;

namespace fyiReporting.RDL
{
	///<summary>
	/// Class for defining chart layout.  For example, the plot area of a chart.
	///</summary>
	internal class ChartLayout
	{
		int _Height;			// total width of layout
		int _Width;				// total height
		int _LeftMargin;		// Margins
		int _RightMargin;
		int _TopMargin;
		int _BottomMargin;
		System.Drawing.Rectangle _PlotArea;
	
		internal ChartLayout(int width, int height)
		{
			_Width = width;
			_Height = height;
			_LeftMargin = _RightMargin = _TopMargin = _BottomMargin = 0;
			_PlotArea = System.Drawing.Rectangle.Empty;
		}
		
		internal int Width
		{
			get { return  _Width; }
		}
		internal int Height
		{
			get { return  _Height; }
		}
		internal int LeftMargin
		{
			get { return  _LeftMargin; }
			set {  _LeftMargin = value; }
		}
		internal int RightMargin
		{
			get { return  _RightMargin; }
			set {  _RightMargin = value; }
		}
		internal int TopMargin
		{
			get { return  _TopMargin; }
			set {  _TopMargin = value; }
		}
		internal int BottomMargin
		{
			get { return  _BottomMargin; }
			set {  _BottomMargin = value; }
		}
		internal System.Drawing.Rectangle PlotArea
		{
			get 
			{ 
				if (_PlotArea == System.Drawing.Rectangle.Empty)
				{
					int w = _Width - _LeftMargin - _RightMargin;
					if (w <= 0)
						throw new Exception("Plot area width is less than or equal to 0");
					int h =_Height - _TopMargin - _BottomMargin;
					if (h <= 0)
						throw new Exception("Plot area height is less than or equal to 0");
				
					_PlotArea = new System.Drawing.Rectangle(_LeftMargin, _TopMargin, w, h); 
				}

				return _PlotArea;
			}
		}
	}
}
