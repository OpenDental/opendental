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
using System.Globalization;

using fyiReporting.RDL;


namespace fyiReporting.RDL
{
	/// <summary>
	/// Handle references to the User collection.  e.g. User("UserID")
	/// </summary>
	[Serializable]
	internal class FunctionUserCollection : IExpr
	{
		private IDictionary _User;
		private IExpr _ArgExpr;

		/// <summary>
		/// obtain value of Field
		/// </summary>
		public FunctionUserCollection(IDictionary user, IExpr arg) 
		{
			_User = user;
			_ArgExpr = arg;
		}

		public virtual TypeCode GetTypeCode()
		{
			return TypeCode.String;		// all the user types happen to be string
		}

		public virtual bool IsConstant()
		{
			return false;
		}

		public virtual IExpr ConstantOptimization()
		{	
			_ArgExpr = _ArgExpr.ConstantOptimization();

			if (_ArgExpr.IsConstant())
			{
				string o = _ArgExpr.EvaluateString(null, null);
				if (o == null)
					throw new Exception("User collection argument is null"); 
				string lo = o.ToLower();
				if (lo == "userid")
					return new FunctionUserID();
				if (lo == "language")
					return new FunctionUserLanguage();
				throw new Exception(string.Format("User collection argument {0} is invalid.", o)); 
			}

			return this;
		}

		// 
		public virtual object Evaluate(Report rpt, Row row)
		{
			if (rpt == null)
				return null;
			string u = _ArgExpr.EvaluateString(rpt, row);
			if (u == null)
				return null;
			switch (u.ToLower())
			{
				case "userid":
					return rpt.UserID;
				case "language":
					return rpt.ClientLanguage == null?
						CultureInfo.CurrentCulture.ThreeLetterISOLanguageName:
						rpt.ClientLanguage;
				default:
					return null;
			}
		}
		
		public virtual double EvaluateDouble(Report rpt, Row row)
		{
			if (row == null)
				return Double.NaN;
			return Convert.ToDouble(Evaluate(rpt, row), NumberFormatInfo.InvariantInfo);
		}
		
		public virtual decimal EvaluateDecimal(Report rpt, Row row)
		{
			if (row == null)
				return decimal.MinValue;
			return Convert.ToDecimal(Evaluate(rpt, row), NumberFormatInfo.InvariantInfo);
		}

		public virtual string EvaluateString(Report rpt, Row row)
		{
			if (row == null)
				return null;
			return Convert.ToString(Evaluate(rpt, row));
		}

		public virtual DateTime EvaluateDateTime(Report rpt, Row row)
		{
			if (row == null)
				return DateTime.MinValue;
			return Convert.ToDateTime(Evaluate(rpt, row));
		}

		public virtual bool EvaluateBoolean(Report rpt, Row row)
		{
			if (row == null)
				return false;
			return Convert.ToBoolean(Evaluate(rpt, row));
		}
	}
}
