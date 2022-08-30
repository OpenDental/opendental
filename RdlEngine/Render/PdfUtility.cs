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
using System.Text;
using System.Collections;
using System.IO;

namespace fyiReporting.RDL
{
	/// <summary>
	/// This class contains general Utility for the creation of pdf
	/// Creates the Header
	/// Creates XrefTable
	/// Creates the Trailer
	/// </summary>
	internal class PdfUtility
	{
		private int numTableEntries; 
		PdfAnchor pa;
		internal PdfUtility(PdfAnchor p)
		{
			pa = p;
			numTableEntries=0;
		}
		/// <summary>
		/// Creates the xref table using the byte offsets in the array.
		/// </summary>
		/// <returns></returns>
		internal byte[] CreateXrefTable(long fileOffset,out int size)
		{
			//Store the Offset of the Xref table for startxRef
			string table=null;
			try
			{	
				ObjectList objList=new ObjectList(0,fileOffset);
				pa.offsets.Add(objList);	
				pa.offsets.Sort();
				numTableEntries=(int)pa.offsets.Count;
				table=string.Format("\r\nxref {0} {1}\r\n0000000000 65535 f\r\n",0,numTableEntries);
				for(int entries=1; entries<numTableEntries; entries++)
				{
					ObjectList obj=pa.offsets[entries];
					table+=obj.offset.ToString().PadLeft(10,'0');
					table+=" 00000 n\r\n";
				}
			} 
			catch(Exception e)
			{
				Exception error=new Exception(e.Message+" In Utility.CreateXrefTable()");
				throw error;
			}
			return GetUTF8Bytes(table,out size);
		}
		/// <summary>
		/// Returns the Header
		/// </summary>
		/// <param name="version"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		internal byte[] GetHeader(string version,out int size)
		{
			string header=string.Format("%PDF-{0}\r%{1}\r\n",version,"\x82\x82");
			return GetUTF8Bytes(header,out size);
		}
		/// <summary>
		/// Creates the trailer and return the bytes array
		/// </summary>
		/// <returns></returns>
		internal byte[] GetTrailer(int refRoot,int refInfo,out int size)
		{
			string trailer=null;
			string infoDict;
			try
			{
				if(refInfo>0)
				{
					infoDict=string.Format("/Info {0} 0 R",refInfo);
				}
				else 
					infoDict="";
				//The sorted array will be already sorted to contain the file offset at the zeroth position
				ObjectList objList=pa.offsets[0];
				trailer=string.Format("trailer\n<</Size {0}/Root {1} 0 R {2}"+
					">>\r\nstartxref\r\n{3}\r\n%%EOF\r\n"
					,numTableEntries,refRoot,infoDict,objList.offset);

				pa.Reset();
			}
			catch(Exception e)
			{
				Exception error=new Exception(e.Message+" In Utility.GetTrailer()");
				throw error;
			}
	
			return GetUTF8Bytes(trailer,out size);
		}
		/// <summary>
		/// Converts the string to byte array in utf 8 encoding
		/// </summary>
		/// <param name="str"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		static internal byte[] GetUTF8Bytes(string str,out int size)
		{
			try
			{
				byte[] ubuf = Encoding.Unicode.GetBytes(str);
				Encoding enc = Encoding.GetEncoding(1252);
				byte[] abuf = Encoding.Convert(Encoding.Unicode, enc, ubuf);
				size=abuf.Length;
				return abuf;
			}
			catch(Exception e)
			{
				Exception error=new Exception(e.Message+" In Utility.GetUTF8Bytes()");
				throw error;
			}
		}
	}

	internal class Ascii85Encode
	{
		byte [] bain;
		readonly uint width = 72;	// max characters per line
		uint pos;			// tracks # of characters put out in line
		uint tuple = 0;
		int count=0;
		StringWriter sw;

		internal Ascii85Encode(byte [] ba)
		{
			bain = ba;
		}

		override public string ToString()
		{
			sw = new StringWriter();
			tuple = 0;
			count = 0;

			sw.Write("<~");
			pos = 2;

			byte b;
			for (int i =0; i < bain.Length; i++)
			{
				b = bain[i];
				switch (count++) 
				{
					case 0:	tuple |= ((uint)b << 24); break;
					case 1: tuple |= ((uint)b << 16); break;
					case 2:	tuple |= ((uint)b <<  8); break;
					case 3:
						tuple |= b;
						if (tuple == 0) 
						{
							sw.Write('z');
							if (pos++ >= width) 
							{
								pos = 0;
								sw.Write('\n');
							}
						} 
						else
						{
							encode(tuple, count);
						}
						tuple = 0;
						count = 0;
						break;
				}
			}
			// handle some clean up at end of processing
			if (count > 0)
				encode(tuple, count);
			if (pos + 2 > width)
				sw.Write('\n');

			sw.Write("~>\n");

			string baout = sw.ToString();
			sw.Close();
			sw=null;
			return baout;
		}
		
		void encode(uint tuple, int count) 
		{
			int j;
			char[] buf = new char[5];
			int s = 0;
			j = 5;
			do 
			{
				buf[s++] = (char) (tuple % 85);
				tuple /= 85;
			} while (--j > 0);
			j = count;
			do 
			{
				sw.Write((char) (buf[--s] + '!'));	// '!' == 32 
				if (pos++ >= width) 
				{
					pos = 0;
					sw.Write('\n');
				}
			} while (j-- > 0);
		}
	}


	internal class AsciiHexEncode
	{
		byte [] bain;
		readonly int width = 72;	// max characters per line

		internal AsciiHexEncode(byte [] ba)
		{
			bain = ba;
		}

		override public string ToString()
		{
			StringWriter sw = new StringWriter();
			int pos=0;

			for (int i =0; i < bain.Length; i++)
			{
				if (pos >= width)
				{
					sw.Write('\n');
					pos = 0;
				}

				string t = Convert.ToString(bain[i], 16);
				if (t.Length == 1)
					t = "0" + t;
				sw.Write(t);
				pos += 2;
			}

			string baout = sw.ToString();
			sw.Close();
			sw=null;
			return baout;
		}
	}

}
