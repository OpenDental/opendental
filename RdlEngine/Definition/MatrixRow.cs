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
	/// Handle a Matrix Row: i.e. height and matrix cells that make up the row.
	///</summary>
	[Serializable]
	internal class MatrixRow : ReportLink
	{
		RSize _Height;	// Height of each detail cell in this row.
		MatrixCells _MatrixCells;	// The set of cells in a row in the detail section of the Matrix.		
	
		internal MatrixRow(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Height=null;
			_MatrixCells=null;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Height":
						_Height = new RSize(r, xNodeLoop);
						break;
					case "MatrixCells":
						_MatrixCells = new MatrixCells(r, this, xNodeLoop);
						break;
					default:
						break;
				}
			}
			if (_MatrixCells == null)
				OwnerReport.rl.LogError(8, "MatrixRow requires the MatrixCells element.");
		}
		
		override internal void FinalPass()
		{
			if (_MatrixCells != null)
				_MatrixCells.FinalPass();
			return;
		}

		internal RSize Height
		{
			get { return  _Height; }
			set {  _Height = value; }
		}

		internal MatrixCells MatrixCells
		{
			get { return  _MatrixCells; }
			set {  _MatrixCells = value; }
		}
	}
}
