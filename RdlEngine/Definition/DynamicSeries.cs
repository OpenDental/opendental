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
	/// In Charts, the dynamic series definition.
	///</summary>
	[Serializable]
	internal class DynamicSeries : ReportLink
	{
		Grouping _Grouping;	// The expressions to group the data by. Page
							// breaks in the grouping are not allowed.
		Sorting _Sorting;	// The expressions to sort the columns by
		Expression _Label;	// (string) The label displayed on the legend.		
	
		internal DynamicSeries(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Grouping=null;
			_Sorting=null;
			_Label=null;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Grouping":
						_Grouping = new Grouping(r, this, xNodeLoop);
						break;
					case "Sorting":
						_Sorting = new Sorting(r, this, xNodeLoop);
						break;
					case "Label":
						_Label = new Expression(r, this, xNodeLoop, ExpressionType.String);
						break;
					default:
						break;
				}
			}
			if (_Grouping == null)
				OwnerReport.rl.LogError(8, "DynamicSeries requires the Grouping element.");
			if (_Label == null)
				OwnerReport.rl.LogError(8, "DynamicSeries requires the Label element.");
		}

		// Handle parsing of function in final pass
		override internal void FinalPass()
		{
			if (_Grouping != null)
				_Grouping.FinalPass();
			if (_Sorting != null)
				_Sorting.FinalPass();
			if (_Label != null)
				_Label.FinalPass();
			return;
		}

		internal Grouping Grouping
		{
			get { return  _Grouping; }
			set {  _Grouping = value; }
		}

		internal Sorting Sorting
		{
			get { return  _Sorting; }
			set {  _Sorting = value; }
		}

		internal Expression Label
		{
			get { return  _Label; }
			set {  _Label = value; }
		}
	}
}
