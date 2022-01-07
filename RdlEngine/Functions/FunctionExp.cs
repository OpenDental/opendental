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
	/// Exponentiation operator  of form lhs ^ rhs
	/// </summary>
	[Serializable]
	internal class FunctionExp : FunctionBinary, IExpr
	{

		/// <summary>
		/// Do exponentiation on double data types
		/// </summary>
		public FunctionExp() 
		{
		}

		public FunctionExp(IExpr lhs, IExpr rhs) 
		{
			_lhs = lhs;
			_rhs = rhs;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Double;
		}

		public IExpr ConstantOptimization()
		{
			_lhs = _lhs.ConstantOptimization();
			_rhs = _rhs.ConstantOptimization();
			bool bLeftConst = _lhs.IsConstant();
			bool bRightConst = _rhs.IsConstant();
			if (bLeftConst && bRightConst)
			{
				double d = EvaluateDouble(null, null);
				return new ConstantDouble(d);
			}
			else if (bRightConst)
			{
				double d = _rhs.EvaluateDouble(null, null);
				if (d == 1)
					return _lhs;
			}
			else if (bLeftConst)
			{
				double d = _lhs.EvaluateDouble(null, null);
				if (d == 0)
					return new ConstantDouble(0);
			}

			return this;
		}

		// Evaluate is for interpretation  (and is relatively slow)
		public object Evaluate(Report rpt, Row row)
		{
			return EvaluateDouble(rpt, row);
		}
		
		public double EvaluateDouble(Report rpt, Row row)
		{
			double lhs = _lhs.EvaluateDouble(rpt, row);
			double rhs = _rhs.EvaluateDouble(rpt, row);

			return Math.Pow(lhs,rhs);
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			double result = EvaluateDouble(rpt, row);

			return Convert.ToDecimal(result);
		}

		public string EvaluateString(Report rpt, Row row)
		{
			double result = EvaluateDouble(rpt, row);
			return result.ToString();
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			double result = EvaluateDouble(rpt, row);
			return Convert.ToDateTime(result);
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			double result = EvaluateDouble(rpt, row);
			return Convert.ToBoolean(result);
		}
	}
}
