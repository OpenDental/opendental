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

namespace fyiReporting.RDL
{
	///<summary>
	/// The full list of supported chart subtypes
	///</summary>
	internal enum ChartSubTypeEnum
	{
		Plain, 
		Stacked,
		PercentStacked,
		Smooth,
		Exploded,
		Line, 
		SmoothLine,
		HighLowClose, 
		OpenHighLowClose, 
		Candlestick
	}

	internal class ChartSubType
	{
		static internal ChartSubTypeEnum GetStyle(string s, ReportLog rl)
		{
			ChartSubTypeEnum st;

			switch (s)
			{		
				case "Plain":
					st = ChartSubTypeEnum.Plain;
					break;
				case "Stacked":
					st = ChartSubTypeEnum.Stacked;
					break;
				case "PercentStacked":
					st = ChartSubTypeEnum.PercentStacked;
					break;
				case "Smooth":
					st = ChartSubTypeEnum.Smooth;
					break;
				case "Exploded":
					st = ChartSubTypeEnum.Exploded;
					break;
				case "Line":
					st = ChartSubTypeEnum.Line;
					break;
				case "SmoothLine":
					st = ChartSubTypeEnum.SmoothLine;
					break;
				case "HighLowClose":
					st = ChartSubTypeEnum.HighLowClose;
					break;
				case "OpenHighLowClose":
					st = ChartSubTypeEnum.OpenHighLowClose;
					break;
				case "Candlestick":
					st = ChartSubTypeEnum.Candlestick;
					break;
				default:		
					rl.LogError(4, "Unknown ChartSubType '" + s + "'.  Plain assumed.");
					st = ChartSubTypeEnum.Plain;
					break;
			}
			return st;
		}
	}

}
