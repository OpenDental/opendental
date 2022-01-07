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
	/// Obtain the runtime value of a report parameter label.
	/// </summary>
	[Serializable]
	internal class FunctionReportParameterLabel : FunctionReportParameter
	{
		/// <summary>
		/// obtain value of ReportParameter
		/// </summary>
		public FunctionReportParameterLabel(ReportParameter parm): base(parm) 
		{
		}

		public override TypeCode GetTypeCode()
		{
            if (this.ParameterMethod == ReportParameterMethodEnum.Value)
                return TypeCode.String;
            else
                return base.GetTypeCode();
		}

		public override bool IsConstant()
		{
			return false;
		}

		public override IExpr ConstantOptimization()
		{	// not a constant expression
			return this;
		}

		// Evaluate is for interpretation  (and is relatively slow)
		public override object Evaluate(Report rpt, Row row)
		{
			string v = base.EvaluateString(rpt, row);

			if (p.ValidValues == null)
				return v;

			string[] displayValues = p.ValidValues.DisplayValues(rpt);
			object[] dataValues = p.ValidValues.DataValues(rpt);

			for (int i=0; i < dataValues.Length; i++)
			{
				if (dataValues[i].ToString() == v)
					return displayValues[i];
			}

			return v;
		}
		
		public override double EvaluateDouble(Report rpt, Row row)
		{	
			string r = EvaluateString(rpt, row);

			return r == null? double.MinValue: Convert.ToDouble(r);
		}
		
		public override decimal EvaluateDecimal(Report rpt, Row row)
		{
			string r = EvaluateString(rpt, row);

			return r == null? decimal.MinValue: Convert.ToDecimal(r);
		}

		public override string EvaluateString(Report rpt, Row row)
		{
			return (string) Evaluate(rpt, row);
		}

		public override DateTime EvaluateDateTime(Report rpt, Row row)
		{
			string r = EvaluateString(rpt, row);

			return r == null? DateTime.MinValue: Convert.ToDateTime(r);
		}

		public override bool EvaluateBoolean(Report rpt, Row row)
		{
			string r = EvaluateString(rpt, row);

			return r.ToLower() == "true"? true: false;
		}
	}
}
