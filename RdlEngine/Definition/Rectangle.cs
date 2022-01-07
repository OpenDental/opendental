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
	/// Represent the rectangle report item.
	///</summary>
	[Serializable]
	internal class Rectangle : ReportItem
	{
		ReportItems _ReportItems;	// Report items contained within the bounds of the rectangle.
		bool _PageBreakAtStart;		// Indicates the report should page break at the start of the rectangle.
		bool _PageBreakAtEnd;		// Indicates the report should page break at the end of the rectangle.		

		// constructor that doesn't process syntax
		internal Rectangle(ReportDefn r, ReportLink p, XmlNode xNode, bool bNoLoop):base(r,p,xNode)
		{
			_ReportItems=null;
			_PageBreakAtStart=false;
			_PageBreakAtEnd=false;
		}

		internal Rectangle(ReportDefn r, ReportLink p, XmlNode xNode):base(r,p,xNode)
		{
			_ReportItems=null;
			_PageBreakAtStart=false;
			_PageBreakAtEnd=false;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "ReportItems":
						_ReportItems = new ReportItems(r, this, xNodeLoop);
						break;
					case "PageBreakAtStart":
						_PageBreakAtStart = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					case "PageBreakAtEnd":
						_PageBreakAtEnd = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					default:	
						if (ReportItemElement(xNodeLoop))	// try at ReportItem level
							break;
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Rectangle element " + xNodeLoop.Name + " ignored.");
						break;
				}
			}
		}
 
		override internal void FinalPass()
		{
			base.FinalPass();

			if (_ReportItems != null)
				_ReportItems.FinalPass();

			return;
		}
 
		override internal void Run(IPresent ip, Row row)
		{
			base.Run(ip, row);

			if (_ReportItems == null)
				return;

			if (ip.RectangleStart(this, row))
			{
				_ReportItems.Run(ip, row);
				ip.RectangleEnd(this, row);
			}
		}

		override internal void RunPage(Pages pgs, Row row)
		{
			Report r = pgs.Report;
			if (IsHidden(r, row))
				return;

			SetPagePositionBegin(pgs);

            // Handle page breaking at start
            if (this.PageBreakAtStart && !IsTableOrMatrixCell(r) && !pgs.CurrentPage.IsEmpty())
            {	// force page break at beginning of dataregion
                pgs.NextOrNew();
                pgs.CurrentPage.YOffset = OwnerReport.TopOfPage;
            }

			PageRectangle pr = new PageRectangle();
			SetPagePositionAndStyle(r, pr, row);
			if (pr.SI.BackgroundImage != null)
				pr.SI.BackgroundImage.H = pr.H;		//   and in the background image

			Page p = pgs.CurrentPage;
			p.AddObject(pr);

            // Handle page breaking at end
            if (this.PageBreakAtEnd && !IsTableOrMatrixCell(r) && !pgs.CurrentPage.IsEmpty())
            {	// force page break at beginning of dataregion
                pgs.NextOrNew();
                pgs.CurrentPage.YOffset = OwnerReport.TopOfPage;
            }
			else if (_ReportItems != null)
			{
				float saveY = p.YOffset;
				p.YOffset += (Top == null? 0: this.Top.Points); 
				_ReportItems.RunPage(pgs, row, GetOffsetCalc(pgs.Report)+LeftCalc(r));
				p.YOffset = saveY;
			}

			SetPagePositionEnd(pgs, pgs.CurrentPage.YOffset);
		}

		internal ReportItems ReportItems
		{
			get { return  _ReportItems; }
			set {  _ReportItems = value; }
		}

		internal bool PageBreakAtStart
		{
			get { return  _PageBreakAtStart; }
			set {  _PageBreakAtStart = value; }
		}

		internal bool PageBreakAtEnd
		{
			get { return  _PageBreakAtEnd; }
			set {  _PageBreakAtEnd = value; }
		}
	}
}
