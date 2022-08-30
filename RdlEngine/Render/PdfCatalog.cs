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
using System.Diagnostics;

namespace fyiReporting.RDL
{
	/// <summary>
	/// Models the Catalog dictionary within a pdf file. This is the first created object. 
	/// It contains references to all other objects within the List of Pdf Objects.
	/// </summary>
	internal class PdfCatalog:PdfBase
	{
		private string catalog;
		private string lang;
		internal PdfCatalog(PdfAnchor pa, string l):base(pa)
		{
			if (l != null)
				lang = String.Format("/Lang({0})", l);
			else
				lang = "";
		}
		/// <summary>
		///Returns the Catalog Dictionary 
		/// </summary>
		/// <returns></returns>
		internal byte[] GetCatalogDict(int outline, int refPageTree,long filePos,out int size)
		{
			Debug.Assert(refPageTree >= 1);
			
            if (outline >= 0)
                catalog=string.Format("\r\n{0} 0 obj<</Type /Catalog{2}/Pages {1} 0 R /Outlines {3} 0 R>>\tendobj\t",
				    this.objectNum,refPageTree, lang, outline);
            else
                catalog = string.Format("\r\n{0} 0 obj<</Type /Catalog{2}/Pages {1} 0 R>>\tendobj\t",
                    this.objectNum, refPageTree, lang);
            
            return this.GetUTF8Bytes(catalog, filePos, out size);
		}

	}
}
