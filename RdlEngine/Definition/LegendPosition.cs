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
	/// Handle Legend position enumeration: TopLeft, LeftTop, ...
	///</summary>
	internal enum LegendPositionEnum
	{
		TopLeft,
		TopCenter,
		TopRight,
		LeftTop,
		LeftCenter,
		LeftBottom,
		RightTop,
		RightCenter,
		RightBottom,
		BottomRight,
		BottomCenter,
		BottomLeft
	}
	internal class LegendPosition
	{
		static internal LegendPositionEnum GetStyle(string s, ReportLog rl)
		{
			LegendPositionEnum rs;

			switch (s)
			{		
				case "TopLeft":
					rs = LegendPositionEnum.TopLeft;
					break;
				case "TopCenter":
					rs = LegendPositionEnum.TopCenter;
					break;
				case "TopRight":
					rs = LegendPositionEnum.TopRight;
					break;
				case "LeftTop":
					rs = LegendPositionEnum.LeftTop;
					break;
				case "LeftCenter":
					rs = LegendPositionEnum.LeftCenter;
					break;
				case "LeftBottom":
					rs = LegendPositionEnum.LeftBottom;
					break;
				case "RightTop":
					rs = LegendPositionEnum.RightTop;
					break;
				case "RightCenter":
					rs = LegendPositionEnum.RightCenter;
					break;
				case "RightBottom":
					rs = LegendPositionEnum.RightBottom;
					break;
				case "BottomRight":
					rs = LegendPositionEnum.BottomRight;
					break;
				case "BottomCenter":
					rs = LegendPositionEnum.BottomCenter;
					break;
				case "BottomLeft":
					rs = LegendPositionEnum.BottomLeft;
					break;
				default:		
					rl.LogError(4, "Unknown LegendPosition '" + s + "'.  RightTop assumed.");
					rs = LegendPositionEnum.RightTop;
					break;
			}
			return rs;
		}
	}

}
