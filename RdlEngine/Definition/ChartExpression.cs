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
using System.IO;


namespace fyiReporting.RDL
{
	///<summary>
	/// ChartExpression definition and processing.
	///</summary>
	[Serializable]
	internal class ChartExpression : ReportItem
	{
		Expression _Value;	// (Variant) An expression, the value of which is
							// displayed in the chart
		DataPoint _DataPoint;	// The data point that generated this
		internal ChartExpression(ReportDefn r, ReportLink p, XmlNode xNode):base(r,p,xNode)
		{
			_Value=null;
		
			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Value":
						_Value = new Expression(r, this, xNodeLoop, ExpressionType.Variant);
						break;
					case "DataPoint":
						_DataPoint = (DataPoint) this.OwnerReport.LUDynamicNames[xNodeLoop.InnerText];
						break;
					default:
						if (ReportItemElement(xNodeLoop))	// try at ReportItem level
							break;
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Textbox element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
		}

		// Handle parsing of function in final pass
		override internal void FinalPass()
		{
			base.FinalPass();
			if (_Value != null)
				_Value.FinalPass();
			return;
		}

		override internal void Run(IPresent ip, Row row)
		{
			return;
		}

		internal Expression Value
		{
			get { return  _Value; }
			set {  _Value = value; }
		}

		internal DataPoint DP
		{
			get { return  _DataPoint; }
		}

	}
}
