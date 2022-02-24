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
	public class DataSources
	{
		Report _rpt;				// Runtime report
		ListDictionary _Items;		// list of report items

		internal DataSources(Report rpt, DataSourcesDefn dsds)
		{
			_rpt = rpt;
			_Items = new ListDictionary();

			// Loop thru all the child nodes
			foreach(DataSourceDefn dsd in dsds.Items.Values)
			{
				DataSource ds = new DataSource(rpt, dsd);
				_Items.Add(dsd.Name.Nm,	ds);
			}
		}

		public DataSource this[string name]
		{
			get 
			{
				return _Items[name] as DataSource;
			}
		}
	}
}
