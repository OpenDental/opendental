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
	/// The collection of Drillthrough parameters.
	///</summary>
	[Serializable]
	internal class DrillthroughParameters : ReportLink
	{
        List<DrillthroughParameter> _Items;			// list of report items

		internal DrillthroughParameters(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			DrillthroughParameter d;
            _Items = new List<DrillthroughParameter>();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Parameter":
						d = new DrillthroughParameter(r, this, xNodeLoop);
						break;
					default:	
						d=null;		// don't know what this is
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Parameters element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
				if (d != null)
					_Items.Add(d);
			}
			if (_Items.Count > 0)
                _Items.TrimExcess();
		}
		
		override internal void FinalPass()
		{
			foreach (DrillthroughParameter r in _Items)
			{
				r.FinalPass();
			}
			return;
		}

        internal List<DrillthroughParameter> Items
		{
			get { return  _Items; }
		}
	}
}
