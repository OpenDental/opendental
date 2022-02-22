/* ====================================================================
    Copyright (C) 2004-2005  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    The RDL project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General public License for more details.

    You should have received a copy of the GNU General public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace fyiReporting.RDL
{
	///<summary>
	/// Represents all the pages of a report.  Needed when you need
	/// render based on pages.  e.g. PDF
	///</summary>
	public class Pages : IEnumerable
	{
		Bitmap _bm;						// bitmap to build graphics object 
		Graphics _g;					// graphics object
		Report _report;					// owner report
		ArrayList _pages;				// array of pages
		Page _currentPage;				// the current page; 1st page if null
		float _BottomOfPage;			// the bottom of the page
		float _PageHeight;				// default height for all pages
		float _PageWidth;				// default width for all pages
	
		public Pages(Report r)
		{
			_report = r;
			_pages = new ArrayList();	// array of Page objects

			_bm = new Bitmap(10, 10);	// create a small bitmap to base our graphics
			_g = Graphics.FromImage(_bm); 
		}

		public Page this[int index]
		{
			get {return (Page) _pages[index];}
		}

		public int Count
		{
			get {return _pages.Count;}
		}

		public void AddPage(Page p)
		{
			_pages.Add(p);
			_currentPage = p;
		}

		public void NextOrNew()
		{
			if (_currentPage == this.LastPage)
				AddPage(new Page(PageCount+1));
			else
			{
				_currentPage = (Page) _pages[_currentPage.PageNumber];  
				_currentPage.SetEmpty();			
			}
		}

		/// <summary>
		/// CleanUp should be called after every render to reduce resource utilization.
		/// </summary>
		public void CleanUp()
		{
			_g.Dispose();
			_g = null;
			_bm.Dispose();
			_bm = null;
		}

		public float BottomOfPage
		{
			get { return _BottomOfPage; }
			set { _BottomOfPage = value; }
		}

		public Page CurrentPage
		{
			get 
			{ 
				if (_currentPage != null)
					return _currentPage;
				
				if (_pages.Count >= 1)
				{
					_currentPage = (Page) _pages[0];
					return _currentPage;
				}

				return null;
			}

			set
			{
				_currentPage = value;
			}
		}

		public Page FirstPage
		{
			get 
			{
				if (_pages.Count <= 0)
					return null;
				else
					return (Page) _pages[0];
			}
		}

		public Page LastPage
		{
			get 
			{
				if (_pages.Count <= 0)
					return null;
				else
					return (Page) (_pages[_pages.Count-1]);
			}
		}

		public float PageHeight
		{
			get {return _PageHeight;}
			set {_PageHeight = value;}
		}

		public float PageWidth
		{
			get {return _PageWidth;}
			set {_PageWidth = value;}
		}

		public void RemoveLastPage()
		{
			Page lp = LastPage;

			if (lp == null)				// if no last page nothing to do
				return;			

			_pages.RemoveAt(_pages.Count-1);	// remove the page

			if (this.CurrentPage == lp)	// reset the current if necessary
			{
				if (_pages.Count <= 0)
					CurrentPage = null;
				else
					CurrentPage = (Page) (_pages[_pages.Count-1]);
			}

			return;
		}

		public Graphics G
		{
			get 
			{
				if (_g == null)
				{
					_bm = new Bitmap(10, 10);	// create a small bitmap to base our graphics
					_g = Graphics.FromImage(_bm); 
				}
				return _g; 
			}
		}

		public int PageCount
		{
			get { return _pages.Count; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()		// just loop thru the pages
		{
			return _pages.GetEnumerator();
		}

		#endregion
	}

	public class Page : IEnumerable
	{
		// note: all sizes are in points
		int _pageno;
		ArrayList _items;				// array of items on the page
		float _yOffset;					// current y offset; top margin, page header, other details, ... 
		float _xOffset;					// current x offset; margin, body taken into account?
		int _emptyItems;				// # of items which constitute empty

		public Page(int page)
		{
			_pageno = page;
			_items = new ArrayList();
			_emptyItems = 0;
		}

		public void AddObject(PageItem pi)
		{
			// adjust the page item locations
			pi.X += _xOffset;
			pi.Y += _yOffset;
			if (pi is PageLine)
			{
				PageLine pl = pi as PageLine;
				pl.X2 += _xOffset;
				pl.Y2 += _yOffset;
			}
			_items.Add(pi);
		}

		public bool IsEmpty()
		{
			return _items.Count > _emptyItems? false: true;
		}

		public void ResetEmpty()
		{
			_emptyItems = 0;
		}

		public void SetEmpty()
		{
			_emptyItems = _items.Count;
		}

		public int PageNumber
		{
			get { return _pageno;}
		}

		public float XOffset
		{
			get { return _xOffset; }
			set { _xOffset = value; }
		}

		public float YOffset
		{
			get { return _yOffset; }
			set { _yOffset = value; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()		// just loop thru the pages
		{
			return _items.GetEnumerator();
		}

		#endregion
	}

	public class PageItem
	{
		float x;				// x coordinate
		float y;				// y coordinate
		float h;				// height  --- line redefines as Y2
		float w;				// width   --- line redefines as X2
		StyleInfo si;			// all the style information evaluated

		public float X
		{
			get { return x;}
			set { x = value;}
		}

		public float Y
		{
			get { return y;}
			set { y = value;}
		}

		public float H
		{
			get { return h;}
			set { h = value;}
		}

		public float W
		{
			get { return w;}
			set { w = value;}
		}

		public StyleInfo SI
		{
			get { return si;}
			set { si = value;}
		}
	}

	public class PageImage : PageItem
	{
		string name;				// name of object if constant image
		ImageFormat imf;			// type of image; png, jpeg are supported
		byte[] imageData;
		int samplesW;
		int samplesH;
		ImageRepeat repeat;
		ImageSizingEnum sizing;

		public PageImage(ImageFormat im, byte[] image, int w, int h)
		{
			Debug.Assert(im == ImageFormat.Jpeg || im == ImageFormat.Png, 
							"PageImage only supports Jpeg and Png image formats.");
			imf = im;
			imageData = image;
			samplesW = w;
			samplesH = h;
			repeat = ImageRepeat.NoRepeat;
			sizing = ImageSizingEnum.AutoSize;
		}

		public byte[] ImageData
		{
			get {return imageData;}
		}

		public ImageFormat ImgFormat
		{
			get {return imf;}
		}

		public string Name
		{
			get {return name;}
			set {name = value;}
		}

		public ImageRepeat Repeat
		{
			get {return repeat;}
			set {repeat = value;}
		}

		public ImageSizingEnum Sizing
		{
			get {return sizing;}
			set {sizing = value;}
		}

		public int SamplesW
		{
			get {return samplesW;}
		}

		public int SamplesH
		{
			get {return samplesH;}
		}
	}

	public enum ImageRepeat
	{
		Repeat,			// repeat image in both x and y directions
		NoRepeat,		// don't repeat
		RepeatX,		// repeat image in x direction
		RepeatY			// repeat image in y direction
	}

	public class PageLine : PageItem
	{
		public PageLine()
		{
		}

		public float X2
		{
			get {return W;}
			set {W = value;}
		}

		public float Y2
		{
			get {return H;}
			set {H = value;}
		}
	}

	public class PageRectangle : PageItem
	{
		public PageRectangle()
		{
		}
	}

	public class PageText : PageItem
	{
		string text;
		bool bGrow;

		public PageText(string t)
		{
			text = t;
			bGrow=false;
		}

		public string Text
		{
			get {return text;}
			set {text = value;}
		}

		public bool CanGrow
		{
			get {return bGrow;}
			set {bGrow = value;}
		}
	}
}
