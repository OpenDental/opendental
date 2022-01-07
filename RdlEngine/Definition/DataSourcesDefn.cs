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
	/// Contains list of DataSource about how to connect to sources of data used by the DataSets.
	///</summary>
	[Serializable]
	internal class DataSourcesDefn : ReportLink
	{
		ListDictionary _Items;			// list of report items

		internal DataSourcesDefn(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			// Run thru the attributes
//			foreach(XmlAttribute xAttr in xNode.Attributes)
//			{
//			}
			_Items = new ListDictionary();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				if (xNodeLoop.Name == "DataSource")
				{
					DataSourceDefn ds = new DataSourceDefn(r, this, xNodeLoop);
					if (ds.Name != null)
						_Items.Add(ds.Name.Nm, ds);
				}
			}
			if (_Items.Count == 0)
				OwnerReport.rl.LogError(8, "For DataSources at least one DataSource is required.");
		}

		public DataSourceDefn this[string name]
		{
			get 
			{
				return _Items[name] as DataSourceDefn;
			}
		}

		internal void CleanUp(Report rpt)		// closes any connections
		{
			foreach (DataSourceDefn ds in _Items.Values)
			{
				ds.CleanUp(rpt);
			}
		}
		
		override internal void FinalPass()
		{
			foreach (DataSourceDefn ds in _Items.Values)
			{
				ds.FinalPass();
			}
			return;
		}

		internal bool ConnectDataSources(Report rpt)
		{
			// Handle any parent connections if any	(ie we're in a subreport and want to use parent report connections
			if (rpt.ParentConnections != null && rpt.ParentConnections.Items != null)
			{	// we treat subreport merged transaction connections as set by the User 
				foreach (DataSourceDefn ds in _Items.Values)
				{
					foreach (DataSourceDefn dsp in rpt.ParentConnections.Items.Values)
					{
						if (ds.AreSameDataSource(dsp))
						{
							ds.SetUserConnection(rpt, dsp.GetConnection(rpt));
							break;
						}
					}
				}
			}

			foreach (DataSourceDefn ds in _Items.Values)
			{
				ds.ConnectDataSource(rpt);
			}
			return true;
		}


		internal ListDictionary Items
		{
			get { return  _Items; }
		}
	}
}
