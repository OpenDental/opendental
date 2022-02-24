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


namespace fyiReporting.RDL
{
	/// <summary>
	/// Class is used to evaluate static system classes.   System meaning
	/// any class that is part of this assembly.   The parser restricts this
	/// to Math, String, Convert, Financial, ...
	/// </summary>
	[Serializable]
	internal class FunctionSystem : IExpr
	{
		string _Cls;		// class name
		string _Func;		// function/operator
		IExpr[] _Args;		// arguments 
		TypeCode _ReturnTypeCode;	// the return type

		/// <summary>
		/// passed class name, function name, and args for evaluation
		/// </summary>
		public FunctionSystem(string c, string f, IExpr[] a, TypeCode type) 
		{
			_Cls = c;
			_Func = f;
			_Args = a;
			_ReturnTypeCode = type;
		}

		public TypeCode GetTypeCode()
		{
			return _ReturnTypeCode;
		}

		public bool IsConstant()
		{
			return false;		// Can't know what the function does
		}

		public IExpr ConstantOptimization()
		{
			// Do constant optimization on all the arguments
			for (int i=0; i < _Args.GetLength(0); i++)
			{
				IExpr e = (IExpr)_Args[i];
				_Args[i] = e.ConstantOptimization();
			}

			// Can't assume that the function doesn't vary
			//   based on something other than the args e.g. Now()
			return this;
		}

		// Evaluate is for interpretation  (and is relatively slow)
		public object Evaluate(Report rpt, Row row)
		{
			// get the results
			object[] argResults = new object[_Args.Length];
			int i=0;
			foreach(IExpr a  in _Args)
			{
				argResults[i++] = a.Evaluate(rpt, row);
			}
			Type[] argTypes = Type.GetTypeArray(argResults);

			// We can definitely optimize this by caching some info TODO

			// Get ready to call the function
			object returnVal;
			Type theClassType= Type.GetType(_Cls);
			MethodInfo mInfo = theClassType.GetMethod(_Func, argTypes);
			returnVal = mInfo.Invoke(theClassType, argResults);

			return returnVal;
		}

		public double EvaluateDouble(Report rpt, Row row)
		{
			return Convert.ToDouble(Evaluate(rpt, row));
		}
		
		public decimal EvaluateDecimal(Report rpt, Row row)
		{
			return Convert.ToDecimal(Evaluate(rpt, row));
		}

		public string EvaluateString(Report rpt, Row row)
		{
			return Convert.ToString(Evaluate(rpt, row));
		}

		public DateTime EvaluateDateTime(Report rpt, Row row)
		{
			return Convert.ToDateTime(Evaluate(rpt, row));
		}


		public bool EvaluateBoolean(Report rpt, Row row)
		{
			return Convert.ToBoolean(Evaluate(rpt, row));
		}

		public string Cls
		{
			get { return  _Cls; }
			set {  _Cls = value; }
		}

		public string Func
		{
			get { return  _Func; }
			set {  _Func = value; }
		}

		public IExpr[] Args
		{
			get { return  _Args; }
			set {  _Args = value; }
		}
	}

}
