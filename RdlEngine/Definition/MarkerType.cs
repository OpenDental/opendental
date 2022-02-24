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
	/// Handle MarkerType enumeration: Square, circle, ...
	///</summary>
	internal enum MarkerTypeEnum
	{
		None,
		Square,
		Circle,
		Diamond,
		Triangle,
		Cross,
		Auto
	}
	internal class MarkerType
	{
		static internal MarkerTypeEnum GetStyle(string s, ReportLog rl)
		{
			MarkerTypeEnum rs;

			switch (s)
			{		
				case "None":
					rs = MarkerTypeEnum.None;
					break;
				case "Square":
					rs = MarkerTypeEnum.Square;
					break;
				case "Circle":
					rs = MarkerTypeEnum.Circle;
					break;
				case "Diamond":
					rs = MarkerTypeEnum.Diamond;
					break;
				case "Triangle":
					rs = MarkerTypeEnum.Triangle;
					break;
				case "Cross":
					rs = MarkerTypeEnum.Cross;
					break;
				case "Auto":
					rs = MarkerTypeEnum.Auto;
					break;
				default:		
					rl.LogError(4, "Unknown MarkerType '" + s + "'.  None assumed.");
					rs = MarkerTypeEnum.None;
					break;
			}
			return rs;
		}
	}

}
