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
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using fyiReporting.RDL;

namespace fyiReporting.RDL
{
	/// <summary>
	///Represents the font dictionary used in a pdf page
	/// </summary>
	internal class PdfImages
	{
		PdfAnchor pa;
		Hashtable images;
		internal PdfImages(PdfAnchor a)
		{
			pa = a;
			images = new Hashtable();
		}

		internal Hashtable Images
		{
			get { return images; }
		}

		internal string GetPdfImage(PdfPage p, string imgname, int contentRef, ImageFormat imf, byte[] ba, int width, int height)
		{
			PdfImageEntry ie;
			if (imgname != null)
			{
				ie = (PdfImageEntry) images[imgname];
				if (ie != null)
				{
					p.AddResource(ie, contentRef);
					return ie.name;
				}
			}
			else
				imgname = "I" + (images.Count + 1).ToString();
			ie = new PdfImageEntry(pa, p, contentRef, imgname, imf, ba, width, height);
			images.Add(imgname, ie);
			return ie.name;
		}

		/// <summary>
		/// Gets the image entries to be written to the file
		/// </summary>
		/// <returns></returns>
		internal byte[] GetImageDict(long filePos,out int size)
		{
			MemoryStream ms=new MemoryStream();
			int s;
			byte[] ba;
			foreach (PdfImageEntry ie in images.Values)
			{
				ObjectList objList=new ObjectList(ie.objectNum,filePos);
				ba = PdfUtility.GetUTF8Bytes(ie.imgDict, out s);
				ms.Write(ba, 0, ba.Length);
				filePos += s;

				ms.Write(ie.ba, 0, ie.ba.Length);		// write out the image
				filePos += ie.ba.Length;

				ba = PdfUtility.GetUTF8Bytes("endstream\r\nendobj\r\n", out s);
				ms.Write(ba, 0, ba.Length);
				filePos += s;
				ie.xref.offsets.Add(objList);
			}
			
			ba = ms.ToArray();
			size = ba.Length;
			return ba;
		}
	}

	/// <summary>
	///Represents a image entry used in a pdf page
	/// </summary>
	internal class PdfImageEntry:PdfBase
	{
		internal string name;
		internal ImageFormat imf;
		internal byte[] ba;
		internal string imgDict;

		/// <summary>
		/// Create the image Dictionary
		/// </summary>
		internal PdfImageEntry(PdfAnchor pa, PdfPage p, int contentRef, string nm, ImageFormat imgf, byte[] im, int width, int height):base(pa)
		{
			name=nm;
			imf = imgf;
			ba=im;

			string filter;
			if (imf == ImageFormat.Jpeg)
				filter = "/DCTDecode";
			else if (imf == ImageFormat.Png)    // TODO: this still doesn't work
                filter = "/FlateDecode /DecodeParms <</Predictor 15 /Colors 3 /BitsPerComponent 8 /Columns 80>>";
			else if (imf == ImageFormat.Gif)    // TODO: this still doesn't work
				filter = "/LZWDecode";
			else
				filter = "";

			imgDict=string.Format("\r\n{0} 0 obj<</Type/XObject/Subtype /Image /Width {1} /Height {2} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Length {3} /Filter {4} >>\nstream\n",
				this.objectNum,width, height, ba.Length, filter);
					
			p.AddResource(this, contentRef);
		}

	}
}
