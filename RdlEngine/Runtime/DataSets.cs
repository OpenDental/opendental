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
	public class DataSets
	{
		Report _rpt;				// runtime report
		IDictionary _Items;			// list of report items

		internal DataSets(Report rpt, DataSetsDefn dsn)
		{
			_rpt = rpt;

			if (dsn.Items.Count < 10)
				_Items = new ListDictionary();	// Hashtable is overkill for small lists
			else
				_Items = new Hashtable(dsn.Items.Count);

			// Loop thru all the child nodes
			foreach(DataSetDefn dsd in dsn.Items.Values)
			{
				DataSet ds = new DataSet(rpt, dsd);
				_Items.Add(dsd.Name.Nm, ds);
			}
		}
		
		public DataSet this[string name]
		{
			get 
			{
				return _Items[name] as DataSet;
			}
		}
	}
}
