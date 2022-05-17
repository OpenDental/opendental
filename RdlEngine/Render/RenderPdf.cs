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
using fyiReporting.RDL;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace fyiReporting.RDL
{
	
	///<summary>
	/// Renders a report to PDF.   This is a page oriented formatting renderer.
	///</summary>
	internal class RenderPdf: IPresent
	{
		Report r;					// report
		Stream tw;					// where the output is going
		PdfAnchor anchor;			// anchor tieing together all pdf objects
		PdfCatalog catalog;
		PdfPageTree pageTree;
		PdfInfo info;
		PdfFonts fonts;
		PdfImages images;
        PdfOutline outline;         // holds the bookmarks (if any)
		PdfUtility pdfUtility;
		int filesize;
		PdfPage page;
		PdfContent content;
		PdfElements elements;
		static readonly char[] lineBreak = new char[] {'\n'};
		static readonly char[] wordBreak = new char[] {' '};
		static readonly int MEASUREMAX = int.MaxValue;  //  .Net 2 doesn't seem to have a limit; 1.1 limit was 32

		public RenderPdf(Report rep, IStreamGen sg)
		{
			r = rep;
			tw = sg.GetStream();
		}

		public Report Report()
		{
			return r;
		}

		public bool IsPagingNeeded()
		{
			return true;
		}

		public void Start()		
		{
			// Create the anchor for all pdf objects
			CompressionConfig cc = RdlEngineConfig.GetCompression();
			anchor = new PdfAnchor(cc != null);

			//Create a PdfCatalog
			string lang;
			if (r.ReportDefinition.Language != null)
				lang = r.ReportDefinition.Language.EvaluateString(this.r, null);
			else
				lang = null;
			catalog= new PdfCatalog(anchor, lang);

			//Create a Page Tree Dictionary
			pageTree= new PdfPageTree(anchor);

			//Create a Font Dictionary
			fonts = new PdfFonts(anchor);

			//Create an Image Dictionary
			images = new PdfImages(anchor);

            //Create an Outline Dictionary
            outline = new PdfOutline(anchor);

            //Create the info Dictionary
			info=new PdfInfo(anchor);

			//Set the info Dictionary. 
			info.SetInfo(r.Name,r.Author,r.Description,"");	// title, author, subject, company

			//Create a utility object
			pdfUtility=new PdfUtility(anchor);

			//write out the header
			int size=0;
			tw.Write(pdfUtility.GetHeader("1.5",out size),0,size);
            filesize = size;
		}

		public void End()
		{
			//Write everything 
			int size=0;
			tw.Write(catalog.GetCatalogDict(outline.GetObjectNumber(), pageTree.objectNum, 
				filesize,out size),0,size);
			filesize += size;
			tw.Write(pageTree.GetPageTree(filesize,out size),0,size);
			filesize += size;
			tw.Write(fonts.GetFontDict(filesize,out size),0,size);
			filesize += size;
			if (images.Images.Count > 0)
			{
				tw.Write(images.GetImageDict(filesize,out size),0,size);
				filesize += size;
			}
            if (outline.Bookmarks.Count > 0)
            {
                tw.Write(outline.GetOutlineDict(filesize, out size), 0, size);
                filesize += size;
            }

			tw.Write(info.GetInfoDict(filesize,out size),0,size);
			filesize += size;

			tw.Write(pdfUtility.CreateXrefTable(filesize,out size),0,size);
			filesize += size;

			tw.Write(pdfUtility.GetTrailer(catalog.objectNum,
				info.objectNum,out size),0,size);
			filesize += size;
			return;
		}

		public void RunPages(Pages pgs)	// this does all the work
		{	  
			foreach (Page p in pgs)
			{
				//Create a Page Dictionary representing a visible page
				page=new PdfPage(anchor);
				content=new PdfContent(anchor);

				PdfPageSize pSize=new PdfPageSize((int) r.ReportDefinition.PageWidth.Points, 
									(int) r.ReportDefinition.PageHeight.Points);
				page.CreatePage(pageTree.objectNum,pSize);
				pageTree.AddPage(page.objectNum);

				//Create object that presents the elements in the page
				elements=new PdfElements(page, pSize);

				ProcessPage(pgs, p);

				// after a page
				content.SetStream(elements.EndElements());

				page.AddResource(fonts,content.objectNum);

				int size=0;
				tw.Write(page.GetPageDict(filesize,out size),0,size);
				filesize += size;

				tw.Write(content.GetContentDict(filesize,out size),0,size);
				filesize += size;
			}
			return;
		}
		// render all the objects in a page in PDF
		private void ProcessPage(Pages pgs, IEnumerable items)
		{
			foreach (PageItem pi in items)
			{
				if (pi.SI.BackgroundImage != null)
				{	// put out any background image
					PageImage i = pi.SI.BackgroundImage;
					elements.AddImage(images, i.Name, content.objectNum, i.SI, i.ImgFormat, 
						pi.X, pi.Y, pi.W, pi.H, i.ImageData,i.SamplesW, i.SamplesH, null);
				}

				if (pi is PageTextHtml)
				{
					PageTextHtml pth = pi as PageTextHtml;
					pth.Build(pgs.G);
					ProcessPage(pgs, pth);
					continue;
				}

				if (pi is PageText)
				{
					PageText pt = pi as PageText;
					float[] textwidth;
					string[] sa = MeasureString(pt, pgs.G, out textwidth);
					elements.AddText(pt.X, pt.Y, pt.H, pt.W, sa, pt.SI, 
						fonts, textwidth, pt.CanGrow, pt.HyperLink, pt.NoClip);
                    if (pt.BookmarkLink != null)
                    {
                        outline.Bookmarks.Add(new PdfOutlineEntry(anchor, page.objectNum, pt.BookmarkLink, pt.X, elements.PageSize.yHeight - pt.Y));
                    }
					continue;
				}

				if (pi is PageLine)
				{
					PageLine pl = pi as PageLine;
					elements.AddLine(pl.X, pl.Y, pl.X2, pl.Y2, pl.SI);
					continue;
				}

				if (pi is PageImage)
				{
					PageImage i = pi as PageImage;
					float x = i.X + i.SI.PaddingLeft;
					float y = i.Y + i.SI.PaddingTop;
					float w = i.W - i.SI.PaddingLeft - i.SI.PaddingRight;
					float h = i.H - i.SI.PaddingTop - i.SI.PaddingBottom;
					elements.AddImage(images, i.Name, content.objectNum, i.SI, i.ImgFormat, 
						x, y, w, h, i.ImageData,i.SamplesW, i.SamplesH, i.HyperLink);
					continue;
				}

				if (pi is PageRectangle)
				{
					PageRectangle pr = pi as PageRectangle;
					elements.AddRectangle(pr.X, pr.Y, pr.H, pr.W, pi.SI, pi.HyperLink);
					continue;
				}
			}
		}

		private string[] MeasureString(PageText pt, Graphics g, out float[] width)
		{
			StyleInfo si = pt.SI;
			string s = pt.Text;

			Font drawFont=null;
			StringFormat drawFormat=null;
			SizeF ms;
			string[] sa=null;
			width=null;
			try
			{
				// STYLE
				System.Drawing.FontStyle fs = 0;
				if (si.FontStyle == FontStyleEnum.Italic)
					fs |= System.Drawing.FontStyle.Italic;

				// WEIGHT
				switch (si.FontWeight)
				{
					case FontWeightEnum.Bold:
					case FontWeightEnum.Bolder:
					case FontWeightEnum.W500:
					case FontWeightEnum.W600:
					case FontWeightEnum.W700:
					case FontWeightEnum.W800:
					case FontWeightEnum.W900:
						fs |= System.Drawing.FontStyle.Bold;
						break;
					default:
						break;
				}

				drawFont = new Font(StyleInfo.GetFontFamily(si.FontFamilyFull), si.FontSize, fs);
				drawFormat = new StringFormat();
				drawFormat.Alignment = StringAlignment.Near;

				// Measure string   
				//  pt.NoClip indicates that this was generated by PageTextHtml Build.  It has already word wrapped.
				if (pt.NoClip || pt.SI.WritingMode == WritingModeEnum.tb_rl)	// TODO: support multiple lines for vertical text
				{
					ms = MeasureString(s, g, drawFont, drawFormat);
					width = new float[1];
					width[0] = RSize.PointsFromPixels(g, ms.Width);	// convert to points from pixels
					sa = new string[1];
					sa[0] = s;
					return sa;
				}

				// handle multiple lines;
				//  1) split the string into the forced line breaks (ie "\n and \r")
				//  2) foreach of the forced line breaks; break these into words and recombine 
				s = s.Replace("\r\n", "\n");	// don't want this to result in double lines
				string[] flines = s.Split(lineBreak);
				List<string> lines = new List<string>();
				List<float> lineWidths = new List<float>();
                // remove the size reserved for left and right padding
                float ptWidth = pt.W - pt.SI.PaddingLeft - pt.SI.PaddingRight;
                if (ptWidth <= 0)
                    ptWidth = 1;
				foreach (string tfl in flines)
				{
					string fl;
					if (tfl.Length > 0 && tfl[tfl.Length-1] == ' ')
						fl = tfl.TrimEnd(' ');
					else
						fl = tfl;

                    // Check if entire string fits into a line
                    ms = MeasureString(fl, g, drawFont, drawFormat);
                    float tw = RSize.PointsFromPixels(g, ms.Width);
                    if (tw <= ptWidth)      
                    {                       // line fits don't need to break it down further
                        lines.Add(fl);
                        lineWidths.Add(tw);
                        continue;
                    }
                    
                    // Line too long; need to break into multiple lines
                    // 1) break line into parts; then build up again keeping track of word positions
					string[] parts = fl.Split(wordBreak);	// this is the maximum split of lines
                    StringBuilder sb = new StringBuilder(fl.Length);
                    CharacterRange[] cra = new CharacterRange[parts.Length];
                    for (int i = 0; i < parts.Length; i++)
					{
                        int sc = sb.Length;     // starting character
                        sb.Append(parts[i]);    // endding character
                        int ec = sb.Length;
                        if (i != parts.Length - 1)  // last item doesn't need blank
                            sb.Append(" ");
                        CharacterRange cr = new CharacterRange(sc, ec-sc);
                        cra[i] = cr;            // add to character array
					}

                    // 2) Measure the word locations within the line
                    string wfl = sb.ToString();
                    float[] wordLocations = MeasureString(wfl, g, drawFont, drawFormat, cra);
                    if (wordLocations == null)
                        continue;

					// 3) Loop thru creating new lines as needed
                    int startLoc = 0;
                    CharacterRange crs = cra[startLoc];
                    CharacterRange cre = cra[startLoc];
                    float cwidth = wordLocations[0];    // length of the first
                    string ts;
                    for (int i=1; i < cra.Length; i++)
					{
                        cwidth = wordLocations[i] - (startLoc == 0 ? 0 : wordLocations[startLoc-1]);
                        if (cwidth > ptWidth)
						{	// time for a new line
                            cre = cra[i-1];
                            ts = wfl.Substring(crs.First, cre.First + cre.Length - crs.First);
                            startLoc = i;
                            crs = cre = cra[startLoc];
                            lines.Add(ts);
							lineWidths.Add(cwidth);
						}
                        else
                            cre = cra[i];
                    }
                    ts = fl.Substring(crs.First, cre.First + cre.Length - crs.First);
					lines.Add(ts);
					lineWidths.Add(cwidth);
				}
                // create the final array from the Lists
                string[] la = lines.ToArray();
                width = lineWidths.ToArray(); 
				return la;
			}
			finally
			{
				if (drawFont != null)
					drawFont.Dispose();
				if (drawFormat != null)
					drawFont.Dispose();
			}
		}

		/// <summary>
		/// Measures the location of an arbritrary # of words within a string
		/// </summary>
		private float[] MeasureString(string s, Graphics g, Font drawFont, StringFormat drawFormat, CharacterRange[] cra)
        {
			if (cra.Length <= MEASUREMAX)		// handle the simple case of < MEASUREMAX words
				return MeasureString32(s, g, drawFont, drawFormat, cra);

			// Need to compensate for SetMeasurableCharacterRanges limitation of 32 (MEASUREMAX)
			int mcra = (cra.Length / MEASUREMAX);	// # of full 32 arrays we need
			int ip = cra.Length % MEASUREMAX;		// # of partial entries needed for last array (if any)
			float[] sz = new float[cra.Length];	// this is the final result;
			float startPos=0;
			CharacterRange[] cra32 = new CharacterRange[MEASUREMAX];	// fill out			
			int icra=0;						// index thru the cra 
			for (int i=0; i < mcra; i++)
			{
				// fill out the new array
				int ticra = icra;
				for (int j=0; j < cra32.Length; j++)
				{
					cra32[j] = cra[ticra++];
					cra32[j].First -= cra[icra].First;	// adjust relative offsets of strings
				}

				// measure the word locations (in the new string)
				// ???? should I put a blank in front of it?? 
				string ts = s.Substring(cra[icra].First, 
					cra[icra + cra32.Length-1].First + cra[icra + cra32.Length-1].Length - cra[icra].First);
				float[] pos = MeasureString32(ts, g, drawFont, drawFormat, cra32);

				// copy the values adding in the new starting positions
				for (int j=0; j < pos.Length; j++)
					sz[icra++] = pos[j] + startPos;

				startPos = sz[icra-1];	// reset the start position for the next line
			}
			// handle the remaining character
			if (ip > 0)
			{
				// resize the range array
				cra32 = new CharacterRange[ip];				
				// fill out the new array
				int ticra = icra;
				for (int j=0; j < cra32.Length; j++)
				{
					cra32[j] = cra[ticra++];
					cra32[j].First -= cra[icra].First;	// adjust relative offsets of strings
				}
				// measure the word locations (in the new string)
				// ???? should I put a blank in front of it?? 
				string ts = s.Substring(cra[icra].First, 
					cra[icra + cra32.Length-1].First + cra[icra + cra32.Length-1].Length - cra[icra].First);
				float[] pos = MeasureString32(ts, g, drawFont, drawFormat, cra32);

				// copy the values adding in the new starting positions
				for (int j=0; j < pos.Length; j++)
					sz[icra++] = pos[j] + startPos;
			}
			return sz;
        }

		/// <summary>
		/// Measures the location of words within a string;  limited by .Net 1.1 to 32 words
		///     MEASUREMAX is a constant that defines that limit
		/// </summary>
		/// <param name="s"></param>
		/// <param name="g"></param>
		/// <param name="drawFont"></param>
		/// <param name="drawFormat"></param>
		/// <param name="cra"></param>
		/// <returns></returns>
		private float[] MeasureString32(string s, Graphics g, Font drawFont, StringFormat drawFormat, CharacterRange[] cra)
		{
			if (s == null || s.Length == 0)
				return null;

			drawFormat.SetMeasurableCharacterRanges(cra);
			Region[] rs = new Region[cra.Length];
			rs = g.MeasureCharacterRanges(s, drawFont, new RectangleF(0, 0, float.MaxValue, float.MaxValue),
				drawFormat);
			float[] sz = new float[cra.Length];
			int isz = 0;
			foreach (Region r in rs)
			{
				RectangleF mr = r.GetBounds(g);
				sz[isz++] = RSize.PointsFromPixels(g, mr.Right);
			}
			return sz;
		}

		private SizeF MeasureString(string s, Graphics g, Font drawFont, StringFormat drawFormat)
		{
			if (s == null || s.Length == 0)
				return SizeF.Empty;

			CharacterRange[] cr = {new CharacterRange(0, s.Length)};
			drawFormat.SetMeasurableCharacterRanges(cr);
			Region[] rs = new Region[1];
			rs = g.MeasureCharacterRanges(s, drawFont, new RectangleF(0,0,float.MaxValue,float.MaxValue),
				drawFormat);
			RectangleF mr = rs[0].GetBounds(g);

			return new SizeF(mr.Width, mr.Height);
		}

		// Body: main container for the report
		public void BodyStart(Body b)
		{
		}

		public void BodyEnd(Body b)
		{
		}
 		
		public void PageHeaderStart(PageHeader ph)
		{
		}

		public void PageHeaderEnd(PageHeader ph)
		{
		}
 		
		public void PageFooterStart(PageFooter pf)
		{
		}

		public void PageFooterEnd(PageFooter pf)
		{
		}

		public void Textbox(Textbox tb, string t, Row row)
		{
		}

		public void DataRegionNoRows(DataRegion d, string noRowsMsg)
		{
		}

		// Lists
		public bool ListStart(List l, Row r)
		{
			return true;
		}

		public void ListEnd(List l, Row r)
		{
		}

		public void ListEntryBegin(List l, Row r)
		{
		}

		public void ListEntryEnd(List l, Row r)
		{
		}

		// Tables					// Report item table
		public bool TableStart(Table t, Row row)
		{
			return true;
		}

		public void TableEnd(Table t, Row row)
		{
		}

		public void TableBodyStart(Table t, Row row)
		{
		}

		public void TableBodyEnd(Table t, Row row)
		{
		}

		public void TableFooterStart(Footer f, Row row)
		{
		}

		public void TableFooterEnd(Footer f, Row row)
		{
		}

		public void TableHeaderStart(Header h, Row row)
		{
		}

		public void TableHeaderEnd(Header h, Row row)
		{
		}

		public void TableRowStart(TableRow tr, Row row)
		{
		}

		public void TableRowEnd(TableRow tr, Row row)
		{
		}

		public void TableCellStart(TableCell t, Row row)
		{
			return;
		}

		public void TableCellEnd(TableCell t, Row row)
		{
			return;
		}

		public bool MatrixStart(Matrix m, Row r)				// called first
		{
			return true;
		}

		public void MatrixColumns(Matrix m, MatrixColumns mc)	// called just after MatrixStart
		{
		}

		public void MatrixCellStart(Matrix m, ReportItem ri, int row, int column, Row r, float h, float w, int colSpan)
		{
		}

		public void MatrixCellEnd(Matrix m, ReportItem ri, int row, int column, Row r)
		{
		}

		public void MatrixRowStart(Matrix m, int row, Row r)
		{
		}

		public void MatrixRowEnd(Matrix m, int row, Row r)
		{
		}

		public void MatrixEnd(Matrix m, Row r)				// called last
		{
		}

		public void Chart(Chart c, Row r, ChartBase cb)
		{
		}

		public void Image(fyiReporting.RDL.Image i, Row r, string mimeType, Stream ior)
		{
		}
 
		public void Line(Line l, Row r)
		{
			return;
		}

		public bool RectangleStart(RDL.Rectangle rect, Row r)
		{
			return true;
		}

		public void RectangleEnd(RDL.Rectangle rect, Row r)
		{
		}

		public void Subreport(Subreport s, Row r)
		{
		}

		public void GroupingStart(Grouping g)			// called at start of grouping
		{
		}
		public void GroupingInstanceStart(Grouping g)	// called at start for each grouping instance
		{
		}
		public void GroupingInstanceEnd(Grouping g)	// called at start for each grouping instance
		{
		}
		public void GroupingEnd(Grouping g)			// called at end of grouping
		{
		}
	}
}
