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
using System.Collections;

namespace fyiReporting.RDL
{
	///<summary>
	/// Runtime data structure representing the group hierarchy
	///</summary>
	internal class MatrixEntry
	{
		Hashtable _HashData;	// Hash table of data values
		SortedList _SortedData;	//  SortedList version of the data 
		BitArray _Rows;			// rows 
		MatrixEntry _Parent;	// parent
		ColumnGrouping _ColumnGroup;	//   Column grouping
		RowGrouping _RowGroup;	// Row grouping
		int _FirstRow;			// First row in _Rows marked true
		int _LastRow;			// Last row in _Rows marked true
		int _rowCount;			//   we save the rowCount so we can delay creating bitArray
		int _StaticColumn=0;	// this is the index to which column to use (always 0 when dynamic)
		int _StaticRow=0;		// this is the index to which row to use (always 0 when dynamic)
		Rows _Data;				// set dynamically when needed
	
		internal MatrixEntry(MatrixEntry p, int rowCount)
		{
			_HashData = new Hashtable();
			_ColumnGroup = null;
			_RowGroup = null;
			_SortedData = null;
			_Data = null;
			_rowCount = rowCount;
			_Rows = null;
			_Parent = p;
			_FirstRow = -1;
			_LastRow = -1;
		}

		internal Hashtable HashData
		{
			get { return  _HashData; }
		}

		internal Rows Data
		{
			get { return _Data; }
			set { _Data = value; }
		}

		internal SortedList SortedData
		{
			get 
			{
				if (_SortedData == null && _HashData != null)
				{
					if (_HashData.Count > 0)
						_SortedData = new SortedList(_HashData);	// TODO provide comparer
					_HashData = null;		// we only keep one
				}

				return  _SortedData; 
			}
		}

		internal MatrixEntry Parent
		{
			get { return  _Parent; }
		}

		internal ColumnGrouping ColumnGroup
		{
			get { return  _ColumnGroup; }
			set {  _ColumnGroup = value; }
		}

		internal int StaticRow
		{
			get { return  _StaticRow; }
			set {  _StaticRow = value; }
		}

		internal int StaticColumn
		{
			get { return  _StaticColumn; }
			set {  _StaticColumn = value; }
		}

		internal RowGrouping RowGroup
		{
			get { return  _RowGroup; }
			set {  _RowGroup = value; }
		}

		internal int FirstRow
		{
			get { return  _FirstRow; }
			set 
			{
				if (_FirstRow == -1)
					_FirstRow = value; 
			}
		}

		internal int LastRow
		{
			get { return  _LastRow; }
			set 
			{
				if (value >= _LastRow)
					_LastRow = value; 
			}
		}

		internal BitArray Rows
		{
			get 
			{
				if (_Rows == null)
					_Rows = new BitArray(_rowCount);

				return  _Rows; 
			}
			set {  _Rows = value; }
		}
	}
}
