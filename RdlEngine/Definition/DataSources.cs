/* ====================================================================
    Copyright (C) 2004-2005  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    The RDL project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/

using System;
using System.Collections;
using System.Xml;


namespace fyiReporting.RDL
{
	///<summary>
	/// Contains list of DataSource about how to connect to sources of data used by the DataSets.
	///</summary>
	[Serializable]
	public class DataSources : ReportLink
	{
		ArrayList _Items;			// list of report items

		internal DataSources(Report r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			// Run thru the attributes
//			foreach(XmlAttribute xAttr in xNode.Attributes)
//			{
//			}
			_Items = new ArrayList();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				if (xNodeLoop.Name == "DataSource")
				{
					DataSource ds = new DataSource(r, this, xNodeLoop);
					_Items.Add(ds);
				}
			}
			if (_Items.Count == 0)
				OwnerReport.rl.LogError(8, "For DataSources at least one DataSource is required.");
		}

		public DataSource this[string name]
		{
			get 
			{
				foreach (DataSource ds in _Items)
				{
					if (ds.Name != null && ds.Name.Nm == name)
						return ds;
				}
				return null;
			}
		}

		internal void CleanUp()		// closes any connections
		{
			foreach (DataSource ds in _Items)
			{
				ds.CleanUp();
			}
		}
		
		override internal void FinalPass()
		{
			foreach (DataSource ds in _Items)
			{
				ds.FinalPass();
			}
			return;
		}

		internal bool ConnectDataSources()
		{
			foreach (DataSource ds in _Items)
			{
				ds.ConnectDataSource();
			}
			return true;
		}


		internal ArrayList Items
		{
			get { return  _Items; }
		}
	}
}
