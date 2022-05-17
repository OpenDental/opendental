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
	/// Filter operators
	///</summary>
	internal enum FilterOperatorEnum
	{
		Equal,
		Like,
		NotEqual,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		TopN,
		BottomN,
		TopPercent,
		BottomPercent,
		In,
		Between,
		Unknown					// prior to definition or illegal value
	}
	internal class FilterOperator
	{
		static internal FilterOperatorEnum GetStyle(string s)
		{
			FilterOperatorEnum rs;

			switch (s)
			{		
				case "Equal":
				case "=":
					rs = FilterOperatorEnum.Equal;
					break;
				case "TopN":
					rs = FilterOperatorEnum.TopN;
					break;
				case "BottomN":
					rs = FilterOperatorEnum.BottomN;
					break;
				case "TopPercent":
					rs = FilterOperatorEnum.TopPercent;
					break;
				case "BottomPercent":
					rs = FilterOperatorEnum.BottomPercent;
					break;
				case "In":
					rs = FilterOperatorEnum.In;
					break;
				case "LessThanOrEqual":
				case "<=":
					rs = FilterOperatorEnum.LessThanOrEqual;
					break;
				case "LessThan":
				case "<":
					rs = FilterOperatorEnum.LessThan;
					break;
				case "GreaterThanOrEqual":
				case ">=":
					rs = FilterOperatorEnum.GreaterThanOrEqual;
					break;
				case "GreaterThan":
				case ">":
					rs = FilterOperatorEnum.GreaterThan;
					break;
				case "NotEqual":
				case "!=":
					rs = FilterOperatorEnum.NotEqual;
					break;
				case "Between":
					rs = FilterOperatorEnum.Between;
					break;
				case "Like":
					rs = FilterOperatorEnum.Like;
					break;
				default:		// user error just force to normal TODO
					rs = FilterOperatorEnum.Unknown;
					break;
			}
			return rs;
		}
	}

}
