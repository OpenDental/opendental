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
	/// Error in expression;  don't want iexpr to be null so this is returned.
	/// </summary>
	[Serializable]
	internal class ConstantError : IExpr
	{
		string _Value;
		/// <summary>
		/// Constant - as opposed to an expression
		/// </summary>
		public ConstantError(string v) 
		{
			_Value = v;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		public bool IsConstant()
		{
			return true;
		}

		public IExpr ConstantOptimization()
		{	// already constant expression
			return this;
		}

		public object Evaluate(Report rpt, Row row)
		{
			return _Value;
		}

		public string EvaluateString(Report rpt, Row row)
		{
			return _Value;
		}
		
		public double EvaluateDouble(Report rpt, Row row)
		{
			return Convert.ToDouble(_Value);
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			return Convert.ToDecimal(_Value);
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			return Convert.ToDateTime(_Value);
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			return Convert.ToBoolean(_Value);
		}
	}
}
