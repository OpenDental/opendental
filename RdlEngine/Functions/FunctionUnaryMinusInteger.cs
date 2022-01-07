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
	/// Unary minus operator for a integer operand
	/// </summary>
	[Serializable]
	internal class FunctionUnaryMinusInteger : IExpr
	{
		IExpr _rhs;			// rhs

		/// <summary>
		/// Do minus on decimal data type
		/// </summary>
		public FunctionUnaryMinusInteger() 
		{
			_rhs = null;
		}

		public FunctionUnaryMinusInteger(IExpr r) 
		{
			_rhs = r;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Int32;
		}

		public bool IsConstant()
		{
			return _rhs.IsConstant();
		}

		public IExpr ConstantOptimization()
		{
			_rhs = _rhs.ConstantOptimization();
			if (_rhs.IsConstant())
			{
				double d = EvaluateDouble(null, null);
				return new ConstantInteger((int) d);
			}

			return this;
		}

		// Evaluate is for interpretation  (and is relatively slow)
		public object Evaluate(Report rpt, Row row)
		{
			return (int) EvaluateDouble(rpt, row);
		}
		
		public double EvaluateDouble(Report rpt, Row row)
		{
			double result = _rhs.EvaluateDouble(rpt, row);

			return -result;
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			double result = EvaluateDouble(rpt, row);

			return Convert.ToDecimal(result);
		}

		public string EvaluateString(Report rpt, Row row)
		{
			int result = (int) EvaluateDouble(rpt, row);
			return result.ToString();
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			int result = (int) EvaluateDouble(rpt, row);
			return Convert.ToDateTime(result);
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			int result = (int) EvaluateDouble(rpt, row);
			return result == 0? false:true;
		}

		public IExpr Rhs
		{
			get { return  _rhs; }
			set {  _rhs = value; }
		}

	}
}
