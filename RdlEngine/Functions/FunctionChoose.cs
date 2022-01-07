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
	/// Choose function of the form Choose(int, expr1, expr2, ...)
	/// 
	///	
	/// </summary>
	[Serializable]
	internal class FunctionChoose : IExpr
	{
		IExpr[] _expr;
		TypeCode _tc;

		/// <summary>
		/// Choose function of the form Choose(int, expr1, expr2, ...)
		/// </summary>
		public FunctionChoose(IExpr[] ie) 
		{
			_expr = ie;
			_tc = _expr[1].GetTypeCode();

		}

		public TypeCode GetTypeCode()
		{
			return _tc;
		}

		public bool IsConstant()
		{
			return false;		// we could be more sophisticated here; but not much benefit
		}

		public IExpr ConstantOptimization()
		{
			// simplify all expression if possible
			for (int i=0; i < _expr.Length; i++)
			{
				_expr[i] = _expr[i].ConstantOptimization();
			}

			return this;
		}

		// 
		public object Evaluate(Report rpt, Row row)
		{
			double di = _expr[0].EvaluateDouble(rpt, row);
			int i = (int) di;		// force it to integer; we'll accept truncation
			if (i >= _expr.Length || i <= 0)
				return null;
			
			return _expr[i].Evaluate(rpt, row);
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			object result = Evaluate(rpt, row);
			return Convert.ToBoolean(result);
		}
		
		public double EvaluateDouble(Report rpt, Row row)
		{
			object result = Evaluate(rpt, row);
			return Convert.ToDouble(result);
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			object result = Evaluate(rpt, row);
			return Convert.ToDecimal(result);
		}

		public string EvaluateString(Report rpt, Row row)
		{
			object result = Evaluate(rpt, row);
			return Convert.ToString(result);
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			object result = Evaluate(rpt, row);
			return Convert.ToDateTime(result);
		}
	}
}
