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
	/// Collection of report parameters.
	///</summary>
	[Serializable]
	internal class ReportParameters : ReportLink, ICollection
	{
		IDictionary _Items;			// list of report items

		internal ReportParameters(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
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
				if (xNodeLoop.Name == "ReportParameter")
				{
					ReportParameter rp = new ReportParameter(r, this, xNodeLoop);
                    if (rp.Name != null)
					    _Items.Add(rp.Name.Nm, rp);
				}
				else
					OwnerReport.rl.LogError(4, "Unknown ReportParameters element '" + xNodeLoop.Name + "' ignored.");
			}
		}
		
		internal void SetRuntimeValues(Report rpt, IDictionary parms)
		{
			// Fill the values to use in the report parameters
			foreach (string pname in parms.Keys)	// Loop thru the passed parameters
			{
				ReportParameter rp = (ReportParameter) _Items[pname];
				if (rp == null)
				{	// When not found treat it as a warning message
					if (!pname.StartsWith("rs:"))	// don't care about report server parameters
						rpt.rl.LogError(4, "Unknown ReportParameter passed '" + pname + "' ignored.");
					continue;
				}

				rp.SetRuntimeValue(rpt, parms[pname]);
			}

			return;
		}

		override internal void FinalPass()
		{
			foreach (ReportParameter rp in _Items.Values)
			{
				rp.FinalPass();
			}
			return;
		}

		internal IDictionary Items
		{
			get { return  _Items; }
		}
		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return _Items.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return _Items.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			_Items.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return _Items.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _Items.Values.GetEnumerator();
		}

		#endregion
	}
}
