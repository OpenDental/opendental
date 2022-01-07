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
using System.Collections.Specialized;
using System.Xml;


namespace fyiReporting.RDL
{
	///<summary>
	/// The sets of data (defined by DataSet) that are retrieved as part of the Report.
	///</summary>
	[Serializable]
	internal class DataSetsDefn : ReportLink
	{
		IDictionary _Items;			// list of report items

		internal DataSetsDefn(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			if (xNode.ChildNodes.Count < 10)
				_Items = new ListDictionary();	// Hashtable is overkill for small lists
			else
				_Items = new Hashtable(xNode.ChildNodes.Count);

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				if (xNodeLoop.Name == "DataSet")
				{
					DataSetDefn ds = new DataSetDefn(r, this, xNodeLoop);
					if (ds != null && ds.Name != null)
						_Items.Add(ds.Name.Nm, ds);
				}
			}
		}

		internal DataSetDefn this[string name]
		{
			get 
			{
				return _Items[name] as DataSetDefn;
			}
		}
		
		override internal void FinalPass()
		{
			foreach (DataSetDefn ds in _Items.Values)
			{
				ds.FinalPass();
			}
			return;
		}

		internal void GetData(Report rpt)
		{
			foreach (DataSetDefn ds in _Items.Values)
			{
				ds.GetData(rpt);
			}

			return;
		}

		internal IDictionary Items
		{
			get { return  _Items; }
		}
	}
}
