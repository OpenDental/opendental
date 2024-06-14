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
	///Data types
	///</summary>
	internal class DataType
	{
		static internal TypeCode GetStyle(string s, ReportDefn r)
		{
			TypeCode rs;

			if (s.StartsWith("System."))
				s = s.Substring(7);

			switch (s)
			{		
				case "Boolean":
					rs = TypeCode.Boolean;
					break;
				case "DateTime":
					rs = TypeCode.DateTime;
					break;
				case "Decimal":
					rs = TypeCode.Decimal;
					break;
				case "Integer":
				case "Int16":
				case "Int32":
					rs = TypeCode.Int32;
					break;   
				case "Int64":
					rs = TypeCode.Int64;
					break;   
				case "Float":
				case "Single":
				case "Double":
					rs = TypeCode.Double;
					break;
				case "String":
				case "Char":
					rs = TypeCode.String;
					break;
				default:		// user error
					rs = TypeCode.Object;
					r.rl.LogError(4, string.Format("'{0}' is not a recognized type, assuming System.Object.", s));
					break;
			}
			return rs;
		}

		static internal bool IsNumeric(TypeCode tc)
		{
			switch (tc)
			{		
				case TypeCode.Int64:
				case TypeCode.Int32:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				default:		// user error
					return false;
			}
		}
	}

}
