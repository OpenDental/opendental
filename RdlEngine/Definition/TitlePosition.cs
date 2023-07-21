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
	/// Handle title position enumeration: center, near, far.
	///</summary>
	internal enum TitlePositionEnum
	{
		Center,
		Near,
		Far
	}
	internal class TitlePosition
	{
		static internal TitlePositionEnum GetStyle(string s, ReportLog rl)
		{
			TitlePositionEnum rs;

			switch (s)
			{		
				case "Center":
					rs = TitlePositionEnum.Center;
					break;
				case "Near":
					rs = TitlePositionEnum.Near;
					break;
				case "Far":
					rs = TitlePositionEnum.Far;
					break;
				default:	
					rl.LogError(4, "Unknown TitlePosition '" + s + "'.  Center assumed.");
					rs = TitlePositionEnum.Center;
					break;
			}
			return rs;
		}
	}

}
