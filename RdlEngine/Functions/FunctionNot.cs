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
	/// NOT operator of the form ! a
	/// </summary>
	[Serializable]
	internal class FunctionNot : IExpr
	{
		IExpr _rhs;

		/// <summary>
		/// NOT operator of the form ! a
		/// </summary>
		public FunctionNot(IExpr rhs) 
		{
			_rhs = rhs;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Boolean;
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
				bool b = EvaluateBoolean(null, null);
				return new ConstantBoolean(b);
			}

			return this;
		}

		// Evaluate is for interpretation  (and is relatively slow)
		public object Evaluate(Report rpt, Row row)
		{
			return EvaluateBoolean(rpt, row);
		}
		
		public double EvaluateDouble(Report rpt, Row row)
		{
			return Double.NaN;
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			return decimal.MinValue;
		}

		public string EvaluateString(Report rpt, Row row)
		{
			bool result = EvaluateBoolean(rpt, row);
			return result.ToString();
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			return DateTime.MinValue;
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			return ! _rhs.EvaluateBoolean(rpt, row);
		}
	}
}
