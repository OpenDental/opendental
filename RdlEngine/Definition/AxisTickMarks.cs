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
	/// AxisTickMarks definition and processing.
	///</summary>
	internal enum AxisTickMarksEnum
	{
		None,
		Inside,
		Outside,
		Cross
	}

	internal class AxisTickMarks
	{
		static internal AxisTickMarksEnum GetStyle(string s, ReportLog rl)
		{
			AxisTickMarksEnum rs;

			switch (s)
			{		
				case "None":
					rs = AxisTickMarksEnum.None;
					break;
				case "Inside":
					rs = AxisTickMarksEnum.Inside;
					break;
				case "Outside":
					rs = AxisTickMarksEnum.Outside;
					break;
				case "Cross":
					rs = AxisTickMarksEnum.Cross;
					break;
				default:		
					rl.LogError(4, "Unknown Axis Tick Mark '" + s + "'.  None assumed.");
					rs = AxisTickMarksEnum.None;
					break;
			}
			return rs;
		}
	}

}
