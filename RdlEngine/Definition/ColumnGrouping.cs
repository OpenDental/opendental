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
	/// ColumnGrouping definition and processing.
	///</summary>
	[Serializable]
	internal class ColumnGrouping : ReportLink
	{
		RSize _Height;		// Height of the column header
		DynamicColumns _DynamicColumns;	// Dynamic column headings for this grouping
		StaticColumns _StaticColumns;		// Static column headings for this grouping		
	
		internal ColumnGrouping(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Height=null;
			_DynamicColumns=null;
			_StaticColumns=null;

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
					case "DynamicColumns":
						_DynamicColumns = new DynamicColumns(r, this, xNodeLoop);
						break;
					case "StaticColumns":
						_StaticColumns = new StaticColumns(r, this, xNodeLoop);
						break;
					default:
						break;
				}
			}
			if (_Height == null)
				OwnerReport.rl.LogError(8, "ColumnGrouping requires the Height element to be specified.");

			if ((_DynamicColumns != null && _StaticColumns != null) ||
				(_DynamicColumns == null && _StaticColumns == null))
				OwnerReport.rl.LogError(8, "ColumnGrouping requires either the DynamicColumns element or StaticColumns element but not both.");
		}
		
		override internal void FinalPass()
		{
			if (_DynamicColumns != null)
				_DynamicColumns.FinalPass();
			if (_StaticColumns != null)
				_StaticColumns.FinalPass();
			return ;
		}


		internal RSize Height
		{
			get { return  _Height; }
			set {  _Height = value; }
		}

		internal DynamicColumns DynamicColumns
		{
			get { return  _DynamicColumns; }
			set {  _DynamicColumns = value; }
		}

		internal StaticColumns StaticColumns
		{
			get { return  _StaticColumns; }
			set {  _StaticColumns = value; }
		}
	}
}
