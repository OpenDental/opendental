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
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;


namespace fyiReporting.RDL
{
	///<summary>
	/// The Size definition.  Held in a normalized format but convertible to multiple measurements.
	///</summary>
	[Serializable]
	internal class RSize
	{
		int _Size;					// Normalized size in 1/100,000 meters
		string _Original;			// save original string for recreation of syntax

		internal RSize(ReportDefn r, string t)
		{
			// Size is specified in CSS Length Units
			// format is <decimal number nnn.nnn><optional space><unit>
			// in -> inches (1 inch = 2.54 cm)
			// cm -> centimeters (.01 meters)
			// mm -> millimeters (.001 meters)
			// pt -> points (1 point = 1/72.27 inches)
			// pc -> Picas (1 pica = 12 points)
			_Original = t;					// Save original string for recreation
			t = t.Trim();
			int space = t.LastIndexOf(' '); 
			string n;						// number string
			string u;						// unit string
			decimal d;						// initial number
			try		// Convert.ToDecimal can be very picky
			{
				if (space != -1)	// any spaces
				{
					n = t.Substring(0,space).Trim();	// number string
					u = t.Substring(space).Trim();	// unit string
				}
				else if (t.Length >= 3)
				{
					n = t.Substring(0, t.Length-2).Trim();
					u = t.Substring(t.Length-2).Trim();
				}
				else
				{
					// Illegal unit
					r.rl.LogError(4, string.Format("Illegal size '{0}' specified, assuming 0 length.", t));
					_Size = 0;
					return;
				}
				if (!Regex.IsMatch(n, @"\A[ ]*[-]?[0-9]*[.]?[0-9]*[ ]*\Z"))
				{
					r.rl.LogError(4, string.Format("Unknown characters in '{0}' specified.  Number must be of form '###.##'.  Local conversion will be attempted.", t));
					d = Convert.ToDecimal(n, NumberFormatInfo.InvariantInfo);		// initial number
				}
				else
					d = Convert.ToDecimal(n, NumberFormatInfo.InvariantInfo);		// initial number
			}
			catch (Exception ex) 
			{
				// Illegal unit
				r.rl.LogError(4, "Illegal size '" + t + "' specified, assuming 0 length.\r\n"+ex.Message);
				_Size = 0;
				return;
			}

			switch(u)			// convert to millimeters
			{
				case "in":
					_Size = (int) (d * 2540m);
					break;
				case "cm":
					_Size = (int) (d * 1000m);
					break;
				case "mm":
					_Size = (int) (d * 100m);
					break;
				case "pt":
					_Size = (int) (d * (2540m / POINTSIZEM));
					break;
				case "pc":
					_Size = (int) (d * (2540m / POINTSIZEM * 12m));
					break;
				default:	 
					// Illegal unit
					r.rl.LogError(4, "Unknown sizing unit '" + u + "' specified, assuming inches.");
					_Size = (int) (d * 2540m);
					break;
			}
			if (_Size > 160 * 2540)	// Size can't be greater than 160 inches according to spec
			{   // but RdlEngine supports higher values so just do a warning
				r.rl.LogError(4, "Size '" + this._Original + "' is larger than the RDL specification maximum of 160 inches.");
//				_Size = 160 * 2540;     // this would force maximum to spec max of 160
			}
		}

		internal RSize(ReportDefn r, XmlNode xNode):this(r, xNode.InnerText)
		{
		}

		internal int Size
		{
			get { return  _Size; }
			set {  _Size = value; }
		}

		// Return value as if specified as px
		internal int PixelsX
		{
			get
			{	// For now assume 96 dpi;  TODO: what would be better way; shouldn't use server display pixels
				decimal p = _Size;
				p = p / 2540m;		// get it in inches
				p = p * 96;				// 
				return (int) p;
			}
		}

		static internal readonly float POINTSIZED = 72.27f;
		static internal readonly decimal POINTSIZEM = 72.27m;

		static internal int PixelsFromPoints(float x)
		{
			int result = (int) (x * 96 / POINTSIZED);	// convert to pixels

			return result;
		}

		static internal int PixelsFromPoints(Graphics g, float x)
		{
			int result = (int) (x * g.DpiX / POINTSIZED);	// convert to pixels

			return result;
		}
		
		internal int PixelsY
		{
			get
			{	// For now assume 96 dpi
				decimal p = _Size;
				p = p / 2540m;		// get it in inches
				p = p * 96;				// 
				return (int) p;
			}
		}

		internal float Points
		{
			get
			{	
				return (float) ((double) _Size / 2540.0 * POINTSIZED);
			}
		}

		static internal float PointsFromPixels(Graphics g, int x)
		{
			float result = (float) ((x * POINTSIZED) / g.DpiX);	// convert to points from pixels

			return result;
		}

		static internal float PointsFromPixels(Graphics g, float x)
		{
			float result = (float) ((x * POINTSIZED) / g.DpiX);	// convert to points from pixels

			return result;
		}

		internal string Original
		{
			get { return  _Original; }
			set {  _Original = value; }
		}
	}
}
