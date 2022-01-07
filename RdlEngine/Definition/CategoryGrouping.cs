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

namespace fyiReporting.RDL
{
	///<summary>
	/// CategoryGrouping definition and processing.
	///</summary>
	[Serializable]
	internal class CategoryGrouping : ReportLink
	{
		// A CategoryGrouping must have either DynamicCategories or StaticCategories
		//  but can't have both.
		DynamicCategories _DynamicCategories;	// Dynamic Category headings for this grouping
		StaticCategories _StaticCategories;		// Category headings for this grouping		
	
		internal CategoryGrouping(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_DynamicCategories=null;
			_StaticCategories=null;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "DynamicCategories":
						_DynamicCategories = new DynamicCategories(r, this, xNodeLoop);
						break;
					case "StaticCategories":
						_StaticCategories = new StaticCategories(r, this, xNodeLoop);
						break;
					default:	
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown CategoryGrouping element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
			if ((_DynamicCategories == null && _StaticCategories == null) ||
				(_DynamicCategories != null && _StaticCategories != null))
				OwnerReport.rl.LogError(8, "CategoryGrouping requires either DynamicCategories element or StaticCategories element, but not both.");
		}
		
		override internal void FinalPass()
		{
			if (_DynamicCategories != null)
				_DynamicCategories.FinalPass();
			if (_StaticCategories != null)
				_StaticCategories.FinalPass();
			return;
		}

		internal DynamicCategories DynamicCategories
		{
			get { return  _DynamicCategories; }
			set {  _DynamicCategories = value; }
		}

		internal StaticCategories StaticCategories
		{
			get { return  _StaticCategories; }
			set {  _StaticCategories = value; }
		}
	}
}
