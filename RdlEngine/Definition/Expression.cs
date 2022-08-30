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
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using fyiReporting.RDL;

namespace fyiReporting.RDL
{
	///<summary>
	/// A report expression: includes original source, parsed expression and type information.
	///</summary>
	[Serializable]
	internal class Expression : ReportLink, IExpr
	{
		string _Source;			// source of expression
		IExpr _Expr;			// expression after parse
		TypeCode _Type;			// type of expression; only available after parsed
		ExpressionType _ExpectedType;	// expected type of expression
		string _UniqueName;		// unique name of expression; not always created
	
		internal Expression(ReportDefn r, ReportLink p, XmlNode xNode, ExpressionType et) : base(r, p)
		{
			_Source=xNode.InnerText;
			_Type = TypeCode.Empty;
			_ExpectedType = et;
			_Expr = null;
		}

		// UniqueName of expression
		internal string UniqueName
		{
			get {return _UniqueName;}
		}

		override internal void FinalPass()
		{
			// optimization: avoid expression overhead if this isn't really an expression
			if (_Source == null)
			{
				_Expr = new Constant("");
				return;
			}
			else if (_Source == "" ||			// empty expression
				_Source.Substring(0,1) != "=")	// if 1st char not '='
			{
				_Expr = new Constant(_Source);	//   this is a constant value
				return;
			}

			Parser p = new Parser(OwnerReport.DataCache);

			// find the fields that are part of the DataRegion (if there is one)
			IDictionary fields=null;
			ReportLink dr = Parent;
			Grouping grp= null;		// remember if in a table group or detail group or list group
			Matrix m=null;
			ReportLink phpf=null;
			while (dr != null)
			{
				if (dr is Grouping)
					p.NoAggregateFunctions = true;
				else if (dr is TableGroup)
					grp = ((TableGroup) dr).Grouping;
				else if (dr is Matrix)
				{
					m = (Matrix) dr;		// if matrix we need to pass special
					break;
				}
				else if (dr is Details)
				{
					grp = ((Details) dr).Grouping;
				}
				else if (dr is List)
				{
					grp = ((List) dr).Grouping;
					break;
				}
				else if (dr is PageHeader || dr is PageFooter)
				{
					phpf = dr;
				}
				else if (dr is DataRegion || dr is DataSetDefn)
					break;
				dr = dr.Parent;
			}
			if (dr != null)
			{
				if (dr is DataSetDefn)
				{
					DataSetDefn d = (DataSetDefn) dr;
					if (d.Fields != null)
						fields = d.Fields.Items;
				}
				else	// must be a DataRegion
				{
					DataRegion d = (DataRegion) dr;
					if (d.DataSetDefn != null &&
						d.DataSetDefn.Fields != null)
						fields = d.DataSetDefn.Fields.Items;
				}
			}

			NameLookup lu = new NameLookup(fields, OwnerReport.LUReportParameters,
				OwnerReport.LUReportItems,OwnerReport.LUGlobals,
				OwnerReport.LUUser, OwnerReport.LUAggrScope,
				grp, m, OwnerReport.CodeModules, OwnerReport.Classes, OwnerReport.DataSetsDefn,
				OwnerReport.CodeType);

			if (phpf != null)
			{
				// Non-null when expression is in PageHeader or PageFooter; 
				//   Expression name needed for dynamic lookup of ReportItems on a page.
				lu.PageFooterHeader = phpf;
				lu.ExpressionName = _UniqueName = "xn_" + Interlocked.Increment(ref Parser.Counter).ToString();
			}

			try 
			{
				_Expr = p.Parse(lu, _Source);
			}
			catch (Exception e)
			{
				_Expr = new ConstantError(e.Message);
				// Invalid expression
				OwnerReport.rl.LogError(8, "Expression '" + _Source + "' failed to parse: " + e.Message);
			}

			// Optimize removing any expression that always result in a constant
			try
			{
				_Expr = _Expr.ConstantOptimization();
			}
			catch(Exception ex)
			{
				OwnerReport.rl.LogError(4, "Expression:" + _Source + "\r\nConstant Optimization exception:\r\n" + ex.Message + "\r\nStack trace:\r\n" + ex.StackTrace );
			}
			_Type = _Expr.GetTypeCode();

			return;
		}

		private void ReportError(Report rpt, int severity, string err)
		{
			if (rpt == null)
				OwnerReport.rl.LogError(severity, err);
			else
				rpt.rl.LogError(severity, err);
		}

		internal string Source
		{
			get { return  _Source; }
		}
		internal IExpr Expr
		{
			get { return  _Expr; }
		}
		internal TypeCode Type
		{
			get { return  _Type; }
		}
		internal ExpressionType ExpectedType
		{
			get { return  _ExpectedType; }
		}
		#region IExpr Members

		public System.TypeCode GetTypeCode()
		{
			return _Expr.GetTypeCode();
		}

		public bool IsConstant()
		{
			return _Expr.IsConstant();
		}

		public IExpr ConstantOptimization()
		{
			return this;
		}

		public object Evaluate(Report rpt, Row row)
		{
			try 
			{
				// Check to see if we're evaluating an expression in a page header or footer;
				//   If that is the case the rows are cached by page.
				if (row == null && this.UniqueName != null)
				{
					Rows rows = rpt.GetPageExpressionRows(UniqueName);
					if (rows != null && rows.Data != null && rows.Data.Count > 0)
						row = rows.Data[0];
				}

				return _Expr.Evaluate(rpt, row);
			}
			catch (Exception e)
			{
				string err;
				if (e.InnerException != null)
					err = String.Format("Exception evaluating {0}.  {1}.  {2}", _Source, e.Message, e.InnerException.Message);
				else
					err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);

				ReportError(rpt, 4, err);
				return null;
			}
		}

		public string EvaluateString(Report rpt, Row row)
		{
			try 
			{
				return _Expr.EvaluateString(rpt, row);
			}
			catch (Exception e)
			{	
				string err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);
				ReportError(rpt, 4, err);
				return null;
			}
		}

		public double EvaluateDouble(Report rpt, Row row)
		{
			try 
			{
				return _Expr.EvaluateDouble(rpt, row);
			}
			catch (Exception e)
			{	
				string err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);
				ReportError(rpt, 4, err);
				return double.NaN;
			}
		}

		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			try 
			{
				return _Expr.EvaluateDecimal(rpt, row);
			}
			catch (Exception e)
			{	
				string err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);
				ReportError(rpt, 4, err);
				return decimal.MinValue;
			}
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			try 
			{
				return _Expr.EvaluateDateTime(rpt, row);
			}
			catch (Exception e)
			{	
				string err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);
				ReportError(rpt, 4, err);
				return DateTime.MinValue;
			}
		}

		public bool EvaluateBoolean(Report rpt, Row row)
		{
			try 
			{
				return _Expr.EvaluateBoolean(rpt, row);
			}
			catch (Exception e)
			{	
				string err = String.Format("Exception evaluating {0}.  {1}", _Source, e.Message);
				ReportError(rpt, 4, err);
				return false;
			}
		}

		#endregion
	}
}
