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

namespace fyiReporting.RDL
{
	///<summary>
	/// A Row in a data set.
	///</summary>
	internal class Row
	{
		int _RowNumber;		// Original row #
		int _Level;			// Usually 0; set when row is part of group with ParentGroup (ie recursive hierarchy)
		GroupEntry _GroupEntry;		//   like level; 
		Rows _R;			// Owner of row collection
		object[] _Data;		// Row of data
	
		internal Row(Rows r, Row rd)			// Constructor that uses existing Row data
		{
			_R = r;
			_Data = rd.Data;
			_Level = rd.Level;
		}

		internal Row(Rows r, int columnCount)
		{
			_R = r;
			_Data = new object[columnCount];
			_Level=0;
		}

		internal object[] Data
		{
			get { return  _Data; }
			set { _Data = value; }
		}

		internal Rows R
		{
			get { return  _R; }
			set { _R = value; }
		}

		internal GroupEntry GroupEntry
		{
			get { return  _GroupEntry; }
			set {  _GroupEntry = value; }
		}

		internal int Level
		{
			get { return  _Level; }
			set {  _Level = value; }
		}

		internal int RowNumber
		{
			get { return  _RowNumber; }
			set {  _RowNumber = value; }
		}
	}
}
