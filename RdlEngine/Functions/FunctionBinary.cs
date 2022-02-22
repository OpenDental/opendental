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
	/// Binary operator
	/// </summary>
	[Serializable]
	internal abstract class FunctionBinary
	{
		public IExpr _lhs;			// lhs 
		public IExpr _rhs;			// rhs

		/// <summary>
		/// Arbitrary binary operater; might be a
		/// </summary>
		public FunctionBinary() 
		{
			_lhs = null;
			_rhs = null;
		}

		public FunctionBinary(IExpr l, IExpr r) 
		{
			_lhs = l;
			_rhs = r;
		}

		public bool IsConstant()
		{
			if (_lhs.IsConstant())
				return _rhs.IsConstant();

			return false;
		}

//		virtual public bool EvaluateBoolean(Report rpt, Row row)
//		{
//			return false;
//		}

		public IExpr Lhs
		{
			get { return  _lhs; }
			set {  _lhs = value; }
		}

		public IExpr Rhs
		{
			get { return  _rhs; }
			set {  _rhs = value; }
		}
	}
}
