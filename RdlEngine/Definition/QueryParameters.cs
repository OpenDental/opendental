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
using System.Collections.Generic;
using System.Xml;

namespace fyiReporting.RDL
{
	///<summary>
	/// Collection of query parameters.
	///</summary>
	[Serializable]
	internal class QueryParameters : ReportLink
	{
        List<QueryParameter> _Items;			// list of QueryParameter
        bool _ContainsArray;                   // true if any of the parameters is an array reference

		internal QueryParameters(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
            _ContainsArray = false;
			QueryParameter q;
            _Items = new List<QueryParameter>();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "QueryParameter":
						q = new QueryParameter(r, this, xNodeLoop);
						break;
					default:	
						q=null;		// don't know what this is
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown QueryParameters element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
				if (q != null)
					_Items.Add(q);
			}
			if (_Items.Count == 0)
				OwnerReport.rl.LogError(8, "For QueryParameters at least one QueryParameter is required.");
			else
                _Items.TrimExcess();
		}
		
		override internal void FinalPass()
		{
			foreach (QueryParameter q in _Items)
			{
				q.FinalPass();
                if (q.IsArray)
                    _ContainsArray = true;
			}
			return;
		}

        internal List<QueryParameter> Items
		{
			get { return  _Items; }
		}
        internal bool ContainsArray
        {
            get { return _ContainsArray; }
        }
	}
}
