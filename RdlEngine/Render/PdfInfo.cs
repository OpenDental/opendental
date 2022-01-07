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

namespace fyiReporting.RDL
{
	/// <summary>
	///Store information about the document,Title, Author, Company, 
	/// </summary>
	internal class PdfInfo:PdfBase
	{
		private string info;
		internal PdfInfo(PdfAnchor pa):base(pa)
		{
			info=null;
		}
		/// <summary>
		/// Fill the Info Dict
		/// </summary>
		internal void SetInfo(string title,string author,string subject, string company)
		{
			info=string.Format("\r\n{0} 0 obj<</ModDate({1})/CreationDate({1})/Title({2})/Creator(fyiReporting Software, LLC)"+
				"/Author({3})/Subject ({4})/Producer(fyiReporting Software, LLC)/Company({5})>>\tendobj\t",
				this.objectNum,
				GetDateTime(),
				title==null?"":title,
				author==null?"":author,
				subject==null?"":subject,
				company==null?"":company);

		}
		/// <summary>
		/// Get the Document Information Dictionary
		/// </summary>
		/// <returns></returns>
		internal byte[] GetInfoDict(long filePos,out int size)
		{
			return GetUTF8Bytes(info,filePos,out size);
		}
		/// <summary>
		/// Get Date as Adobe needs ie similar to ISO/IEC 8824 format
		/// </summary>
		/// <returns></returns>
		private string GetDateTime()
		{
			DateTime universalDate=DateTime.UtcNow;
			DateTime localDate=DateTime.Now;
			string pdfDate=string.Format("D:{0:yyyyMMddhhmmss}", localDate);
			TimeSpan diff=localDate.Subtract(universalDate);
			int uHour=diff.Hours;
			int uMinute=diff.Minutes;
			char sign='+';
			if(uHour<0)
				sign='-';
			uHour=Math.Abs(uHour);
			pdfDate+=string.Format("{0}{1}'{2}'",sign,uHour.ToString().PadLeft(2,'0'),uMinute.ToString().PadLeft(2,'0'));
			return pdfDate;
		}

	}
}
