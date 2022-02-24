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
	/// Chart series definition and processing.
	///</summary>
	[Serializable]
	internal class ChartSeries : ReportLink
	{
		DataPoints _Datapoints;	// Data points within a series
		PlotTypeEnum _PlotType;		// Indicates whether the series should be plotted
								// as a line in a Column chart. If set to auto,
								// should be plotted per the primary chart type.
								// Auto (Default) | Line		
	
		internal ChartSeries(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Datapoints=null;
			_PlotType=PlotTypeEnum.Auto;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "DataPoints":
						_Datapoints = new DataPoints(r, this, xNodeLoop);
						break;
					case "PlotType":
						_PlotType = fyiReporting.RDL.PlotType.GetStyle(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					default:	
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown ChartSeries element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
			if (_Datapoints == null)
				OwnerReport.rl.LogError(8, "ChartSeries requires the DataPoints element.");
		}
		
		override internal void FinalPass()
		{
			if (_Datapoints != null)
				_Datapoints.FinalPass();
			return;
		}

		internal DataPoints Datapoints
		{
			get { return  _Datapoints; }
			set {  _Datapoints = value; }
		}

		internal PlotTypeEnum PlotType
		{
			get { return  _PlotType; }
			set {  _PlotType = value; }
		}
	}
}
