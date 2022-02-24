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
using System.Xml.Xsl;
using System.Text;
using System.IO;
using System.Drawing;			// for Color class
using System.Reflection;

namespace fyiReporting.RDL
{
	///<summary>
	/// Some utility classes consisting entirely of static routines.
	///</summary>
	public sealed class XmlUtil
	{
		static internal bool Boolean(string tf, ReportLog rl)
		{
			string low_tf = tf.ToLower();
			if (low_tf.CompareTo("true") == 0)
				return true;
			if (low_tf.CompareTo("false") == 0)
				return false;
			rl.LogError(4, "Unknown True/False value '" + tf + "'.  False assumed.");
			return false;
		}
		
		static internal Color ColorFromHtml(string sc, Color dc)
		{
			return ColorFromHtml(sc, dc, null);
		}

		static internal Color ColorFromHtml(string sc, Color dc, Report rpt)
		{
			Color c;
			try 
			{
				c = ColorTranslator.FromHtml(sc);
			}
			catch 
			{
				c = dc;
				if (rpt != null)
					rpt.rl.LogError(4, string.Format("'{0}' is an invalid HTML color.", sc));
			}
			return c;
		}

		static internal int Integer(string i)
		{
			return Convert.ToInt32(i);
		}

		/// <summary>
		/// Takes an arbritrary string and returns a string that can be embedded in an
		/// XML element.  For example, '&lt;' is changed to '&amp;lt;'
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		static public string XmlAnsi(string s)
		{
			StringBuilder rs = new StringBuilder(s.Length);

			foreach (char c in s)
			{
				if (c == '<')
					rs.Append("&lt;");
				else if (c == '&')
					rs.Append("&amp;");
				else if ((int) c <= 127)	// in ANSI range
					rs.Append(c);
				else
					rs.Append("&#" + ((int) c).ToString() + ";");
			}

			return rs.ToString();
		}

		static internal void XslTrans(string xslFile, string inXml, Stream outResult)
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(inXml);

            XslCompiledTransform xslt = new XslCompiledTransform();

			//Load the stylesheet.
			xslt.Load(xslFile);

			xslt.Transform(xDoc,null,outResult);
           
			return;
		}

		static internal string EscapeXmlAttribute(string s)
		{
			string result;

			result = s.Replace("'", "&#39;");

			return result;
		}
		/// <summary>
		/// Loads assembly from file; tries up to 3 time; load with name, load from BaseDirectory, 
		/// and load from BaseDirectory concatenated with Relative directory.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		static internal Assembly AssemblyLoadFrom(string s)
		{
			Assembly ra=null;
			try
			{	// try 1) loading just from name
				ra = Assembly.LoadFrom(s);
			}
			catch
			{	// try 2) loading from the base directory name
				string f = AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(s);
				try
				{
					ra = Assembly.LoadFile(f);
				}
				catch (Exception e)
				{
					// try 3) loading from the relative search path
					string relative = AppDomain.CurrentDomain.RelativeSearchPath;
					if (relative == null || relative == String.Empty)
						throw e;
                     
					f = relative + Path.DirectorySeparatorChar + Path.GetFileName(s);
					ra = Assembly.LoadFile(f);
				}
			}

			return ra;
		}

		static internal Type GetTypeFromTypeCode(TypeCode tc)
		{
			Type t =null;
			switch (tc)
			{
				case TypeCode.Boolean:
					t = Type.GetType("System.Boolean");
					break;
				case TypeCode.Byte:
					t = Type.GetType("System.Byte");
					break;
				case TypeCode.Char:
					t = Type.GetType("System.Char");
					break;
				case TypeCode.DateTime:
					t = Type.GetType("System.DateTime");
					break;
				case TypeCode.Decimal:
					t = Type.GetType("System.Decimal");
					break;
				case TypeCode.Double:
					t = Type.GetType("System.Double");
					break;
				case TypeCode.Int16:
					t = Type.GetType("System.Int16");
					break;
				case TypeCode.Int32:
					t = Type.GetType("System.Int32");
					break;
				case TypeCode.Int64:
					t = Type.GetType("System.Int64");
					break;
				case TypeCode.Object:
					t = Type.GetType("System.Object");
					break;
				case TypeCode.SByte:
					t = Type.GetType("System.SByte");
					break;
				case TypeCode.Single:
					t = Type.GetType("System.Single");
					break;
				case TypeCode.String:
					t = Type.GetType("System.String");
					break;
				case TypeCode.UInt16:
					t = Type.GetType("System.UInt16");
					break;
				case TypeCode.UInt32:
					t = Type.GetType("System.UInt32");
					break;
				case TypeCode.UInt64:
					t = Type.GetType("System.UInt64");
					break;
				default:
					t = Type.GetType("Object");
					break;
			}
			return t;
		}
	}
}
