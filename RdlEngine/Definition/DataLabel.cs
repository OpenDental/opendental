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
	/// DataLabel definition and processing.
	///</summary>
	[Serializable]
	internal class DataLabel : ReportLink
	{
		Style _Style;	// Defines text, border and background style
						// properties for the labels
		Expression _Value;	//(Variant) Expression for the value labels. If omitted,
						// values of in the ValueAxis are used for labels.
		bool _Visible;	// Whether the data label is displayed on the
						// chart. Defaults to False.
		DataLabelPositionEnum _Position;	// Position of the label.  Default: auto
		int _Rotation;	// Angle of rotation of the label text		
	
		internal DataLabel(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Style=null;
			_Value=null;
			_Visible=false;
			_Position=DataLabelPositionEnum.Auto;
			_Rotation=0;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Style":
						_Style = new Style(r, this, xNodeLoop);
						break;
					case "Value":
						_Value = new Expression(r, this, xNodeLoop, ExpressionType.Variant);
						break;
					case "Visible":
						_Visible = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					case "Position":
						_Position = DataLabelPosition.GetStyle(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					case "Rotation":
						_Rotation = XmlUtil.Integer(xNodeLoop.InnerText);
						break;
					default:
						break;
				}
			}
		

		}

		// Handle parsing of function in final pass
		override internal void FinalPass()
		{
			if (_Style != null) 
				_Style.FinalPass();
			if (_Value != null) 
				_Value.FinalPass();
			return;
		}


		internal Style Style
		{
			get { return  _Style; }
			set {  _Style = value; }
		}

		internal Expression Value
		{
			get { return  _Value; }
			set {  _Value = value; }
		}

		internal bool Visible
		{
			get { return  _Visible; }
			set {  _Visible = value; }
		}

		internal DataLabelPositionEnum Position
		{
			get { return  _Position; }
			set {  _Position = value; }
		}

		internal int Rotation
		{
			get { return  _Rotation; }
			set {  _Rotation = value; }
		}
	}
}
