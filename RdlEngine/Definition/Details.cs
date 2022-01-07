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
using System.IO;

namespace fyiReporting.RDL
{
	///<summary>
	/// For tabular reports, defines the detail rows with grouping and sorting.
	///</summary>
	[Serializable]
	internal class Details : ReportLink
	{
		TableRows _TableRows;	// The details rows for the table. The details rows
								// cannot contain any DataRegions in any of their TableCells.
		Grouping _Grouping;		// The expressions to group the detail data by
		Sorting _Sorting;		// The expressions to sort the detail data by
		Visibility _Visibility;	// Indicates if the details should be hidden	
		Textbox _ToggleTextbox;	//  resolved TextBox for toggling visibility
	
		internal Details(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_TableRows=null;
			_Grouping=null;
			_Sorting=null;
			_Visibility=null;
			_ToggleTextbox = null;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "TableRows":
						_TableRows = new TableRows(r, this, xNodeLoop);
						break;
					case "Grouping":
						_Grouping = new Grouping(r, this, xNodeLoop);
						break;
					case "Sorting":
						_Sorting = new Sorting(r, this, xNodeLoop);
						break;
					case "Visibility":
						_Visibility = new Visibility(r, this, xNodeLoop);
						break;
					default:
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Details element " + xNodeLoop.Name + " ignored.");
						break;
				}
			}
			if (_TableRows == null)
				OwnerReport.rl.LogError(8, "Details requires the TableRows element.");
		}
		
		override internal void FinalPass()
		{
			_TableRows.FinalPass();
			if (_Grouping != null)
				_Grouping.FinalPass();
			if (_Sorting != null)
				_Sorting.FinalPass();
			if (_Visibility != null)
			{
				_Visibility.FinalPass();
				if (_Visibility.ToggleItem != null)
				{
					_ToggleTextbox = (Textbox) (OwnerReport.LUReportItems[_Visibility.ToggleItem]);
					if (_ToggleTextbox != null)
						_ToggleTextbox.IsToggle = true;
				}
			}
			return;
		}
		
		internal void Run(IPresent ip, Rows rs, int start, int end)
		{
			// if no rows output or rows just leave
			if (rs == null || rs.Data == null)
				return;

			for (int r=start; r <= end; r++)
			{
				_TableRows.Run(ip, rs.Data[r]);
			}
			return;
		}
		
		internal void RunPage(Pages pgs, Rows rs, int start, int end, float footerHeight)
		{
			// if no rows output or rows just leave
			if (rs == null || rs.Data == null)
				return;

			Page p;

			Row row;
			for (int r=start; r <= end; r++)
			{
				p = pgs.CurrentPage;			// this can change after running a row
				row = rs.Data[r];
				float hrows = HeightOfRows(pgs, row);	// height of all the rows in the details
				float height = p.YOffset + hrows;
				if (r == end)
					height += footerHeight;		// on last row; may need additional room for footer
				if (height > pgs.BottomOfPage)
				{
					p = OwnerTable.RunPageNew(pgs, p);
					OwnerTable.RunPageHeader(pgs, row, false, null);
				}
				_TableRows.RunPage(pgs, row, hrows > pgs.BottomOfPage);
			}
			return;
		}
  
		internal TableRows TableRows
		{
			get { return  _TableRows; }
			set {  _TableRows = value; }
		}

		internal float HeightOfRows(Pages pgs, Row r)
		{
			return _TableRows.HeightOfRows(pgs, r);
		}

		internal Grouping Grouping
		{
			get { return  _Grouping; }
			set {  _Grouping = value; }
		}

		internal Sorting Sorting
		{
			get { return  _Sorting; }
			set {  _Sorting = value; }
		}

		internal Table OwnerTable
		{
			get { return (Table) (this.Parent); }
		}

		internal Visibility Visibility
		{
			get { return  _Visibility; }
			set {  _Visibility = value; }
		}

		internal Textbox ToggleTextbox
		{
			get { return  _ToggleTextbox; }
		}
	}
}
