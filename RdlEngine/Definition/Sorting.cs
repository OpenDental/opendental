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
	/// Collection of sort bys.
	///</summary>
	[Serializable]
	internal class Sorting : ReportLink
	{
        List<SortBy> _Items;			// list of SortBy

		internal Sorting(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			SortBy s;
            _Items = new List<SortBy>();
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "SortBy":
						s = new SortBy(r, this, xNodeLoop);
						break;
					default:	
						s=null;		// don't know what this is
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Sorting element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
				if (s != null)
					_Items.Add(s);
			}
			if (_Items.Count == 0)
				OwnerReport.rl.LogError(8, "Sorting requires at least one SortBy be defined.");
			else
                _Items.TrimExcess();
		}
		
		override internal void FinalPass()
		{
			foreach (SortBy s in _Items)
			{
				s.FinalPass();
			}
			return;
		}

        internal List<SortBy> Items
		{
			get { return  _Items; }
		}
	}
}
