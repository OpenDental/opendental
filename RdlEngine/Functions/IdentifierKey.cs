/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General public License for more details.

    You should have received a copy of the GNU Lesser General public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.IO;
using System.Reflection;


using fyiReporting.RDL;


namespace fyiReporting.RDL
{
	/// <summary>
	/// IdentifierKey
	/// </summary>
	public enum IdentifierKeyEnum
	{
		/// <summary>
		/// Recursive
		/// </summary>
		Recursive,
		/// <summary>
		/// Simple
		/// </summary>
		Simple	
	}

	[Serializable]
	internal class IdentifierKey : IExpr
	{
		IdentifierKeyEnum _Value;		// value of the identifier

		/// <summary>
		/// 
		/// </summary>
		public IdentifierKey(IdentifierKeyEnum v) 
		{
			_Value = v;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;			
		}

		public bool IsConstant()
		{
			return false;
		}

		public IdentifierKeyEnum Value
		{
			get {return _Value;}
		}

		public IExpr ConstantOptimization()
		{	
			return this;
		}

		public object Evaluate(Report rpt, Row row)
		{	
			return _Value;
		}

		public double EvaluateDouble(Report rpt, Row row)
		{
			return Double.NaN;
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			return Decimal.MinValue;
		}

		public string EvaluateString(Report rpt, Row row)
		{
			return null;
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			return DateTime.MinValue;
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			return false;
		}
	}
}
