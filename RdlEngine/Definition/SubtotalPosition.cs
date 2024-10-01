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
	/// Handle the matrix subtotal position: before, after
	///</summary>
	internal enum SubtotalPositionEnum
	{
		Before,			// left/above
		After			// right/below

	}

	internal class SubtotalPosition
	{
		static internal SubtotalPositionEnum GetStyle(string s, ReportLog rl)
		{
			SubtotalPositionEnum rs;

			switch (s)
			{		
				case "Before":
					rs = SubtotalPositionEnum.Before;
					break;
				case "After":
					rs = SubtotalPositionEnum.After;
					break;
				default:		
					rl.LogError(4, "Unknown SubtotalPosition '" + s + "'.  Before assumed.");
					rs = SubtotalPositionEnum.Before;
					break;
			}
			return rs;
		}
	}

}
