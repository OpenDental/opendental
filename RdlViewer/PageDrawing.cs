/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This library is distributed in the hope that it will be useful,
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using fyiReporting.RDL;

namespace fyiReporting.RdlViewer
{
	/// <summary>
	/// PageDrawing draws to a graphics context the loaded Pages class.   This 
	/// class is usually created from running a RDL file thru the renderer.
	/// </summary>
	public class PageDrawing : UserControl
	{
		private Pages _pgs;					// the pages of the report to view

		// During drawing these are set
		float _left;
		float _top;
		float _vScroll;
		float _hScroll;
		float DpiX;
		float DpiY;

		// Mouse handling
        List<HitListEntry> _HitList;
		float _LastZoom;
		ToolTip _tt;

		public PageDrawing(Pages pgs)
		{
			// Set up the tooltip
			_tt = new ToolTip();
			_tt.Active = false;
			_tt.ShowAlways = true;

            _HitList = new List<HitListEntry>();
			_LastZoom=1;

			_pgs = pgs;

			// Get our graphics DPI					   
			Graphics ga = null;				
			try
			{
				ga = this.CreateGraphics(); 
				DpiX = ga.DpiX;
				DpiY = ga.DpiY;
			}
			catch
			{
				DpiX = DpiY = 96;
			}
			finally
			{
				if (ga != null)
					ga.Dispose();
			}
			// force to double buffering for smoother drawing
			this.SetStyle(ControlStyles.DoubleBuffer | 
				ControlStyles.UserPaint | 
				ControlStyles.AllPaintingInWmPaint,
				true);

		}

		internal Pages Pgs
		{
			get {return _pgs;}
			set {_pgs = value;}
		}

  /// <summary>
  /// Draw- simple draw of an entire page.  Useful when printing or creating an image.
  /// </summary>
  /// <param name="g"></param>
  /// <param name="page"></param>
  /// <param name="clipRectangle"></param>
		public void Draw(Graphics g, int page, System.Drawing.Rectangle clipRectangle, bool drawBackground)
		{
			DpiX = g.DpiX;			 // this can change (e.g. printing graphics context)
			DpiY = g.DpiY;

//			g.InterpolationMode = InterpolationMode.HighQualityBilinear;	// try to unfuzz charts
			g.PageUnit = GraphicsUnit.Pixel;
			g.ScaleTransform(1, 1); 
			_left = 0;
			_top = 0;
			_hScroll = 0;
			_vScroll = 0;

			RectangleF r = new RectangleF(clipRectangle.X, clipRectangle.Y, 
											clipRectangle.Width, clipRectangle.Height);

			if (drawBackground)
				g.FillRectangle(Brushes.White, PixelsX(_left), PixelsY(_top), 
					PixelsX(_pgs.PageWidth), PixelsY(_pgs.PageHeight));

			ProcessPage(g, _pgs[page], r, false);					
		}
/// <summary>
/// Draw: accounting for scrolling and zoom factors
/// </summary>
/// <param name="g"></param>
/// <param name="zoom"></param>
/// <param name="leftOffset"></param>
/// <param name="pageGap"></param>
/// <param name="hScroll"></param>
/// <param name="vScroll"></param>
/// <param name="clipRectangle"></param>
		public void Draw(Graphics g, float zoom, float leftOffset, float pageGap,
							float hScroll, float vScroll,
							System.Drawing.Rectangle clipRectangle)
		{	
			// init for mouse handling
			_HitList.Clear();			// remove all items from list
			_LastZoom = zoom;

			if (_pgs == null)	
			{	// No pages; means nothing to draw
				g.FillRectangle(Brushes.White, clipRectangle);
				return;
			}

			g.PageUnit = GraphicsUnit.Pixel;
			g.ScaleTransform(zoom, zoom);
			DpiX = g.DpiX;
			DpiY = g.DpiY;

			// Zoom affects how much will show on the screen.  Adjust our perceived clipping rectangle
			//  to account for it.
			RectangleF r;
			r = new RectangleF((clipRectangle.X)/zoom, (clipRectangle.Y)/zoom, 
				(clipRectangle.Width)/zoom, (clipRectangle.Height)/zoom);

			// Calculate the top of the page
			int fpage = (int) (vScroll / (_pgs.PageHeight + pageGap));
			int lpage = (int) ((vScroll + r.Height) / (_pgs.PageHeight + pageGap)) + 1;
			if (fpage >= _pgs.PageCount)
				return;
			if (lpage >= _pgs.PageCount)
				lpage = _pgs.PageCount-1;

			_hScroll = hScroll;
			_left = leftOffset;
			_top = pageGap;
			// Loop thru the visible pages
			for (int p = fpage; p <= lpage; p++)
			{
				_vScroll = vScroll - p * (_pgs.PageHeight + pageGap);

				System.Drawing.Rectangle pr = 
					new System.Drawing.Rectangle((int)PixelsX(_left-_hScroll), (int)PixelsY(_top-_vScroll), 
													(int)PixelsX(_pgs.PageWidth), (int)PixelsY(_pgs.PageHeight));
				g.FillRectangle(Brushes.White, pr);

				ProcessPage(g, _pgs[p], r, true);					

				// Draw the page outline
				using(Pen pn = new Pen(Brushes.Black, 1))
				{
					int z3 = Math.Min((int) (3f / zoom), 3);
					if (z3 <= 0)
						z3 = 1;
					int z4 = Math.Min((int) (4f / zoom), 4);
					if (z4 <= 0)
						z4 = 1;
					g.DrawRectangle(pn, pr);					// outline of page
					g.FillRectangle(Brushes.Black, 
						pr.X+pr.Width, pr.Y+z3, z3, pr.Height);		// right side of page
					g.FillRectangle(Brushes.Black, 
						pr.X+z3, pr.Y+pr.Height, pr.Width, z4);		// bottom of page
				}
			}					 
		}

		override protected void OnMouseDown(MouseEventArgs mea)
		{
			base.OnMouseDown(mea);			// allow base to process first
		
			HitListEntry hle = this.GetHitListEntry(mea);
			SetHitListCursor(hle);			// set the cursor based on the hit list entry

			if (mea.Button != MouseButtons.Left || hle == null)
				return;

            if (hle.pi.HyperLink != null)
            {
                try
                {
                    System.Diagnostics.Process.Start(hle.pi.HyperLink);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Unable to link to {0}{1}{2}", 
                        hle.pi.HyperLink, Environment.NewLine, ex.Message), "HyperLink Error");
                }
            }
		}

		override protected void OnMouseLeave(EventArgs ea)
		{ 		   
			_tt.Active = false;
		}

		override protected void OnMouseMove(MouseEventArgs mea)
		{
			Cursor c = Cursors.Default;

			HitListEntry hle = this.GetHitListEntry(mea);
			SetHitListCursor(hle);
			SetHitListTooltip(hle);
			return;
		}

		private void SetHitListCursor(HitListEntry hle)
		{
			Cursor c = Cursors.Default;
			if (hle == null)
			{}
			else if (hle.pi.HyperLink != null || hle.pi.BookmarkLink != null)
				c = Cursors.Hand;

			if (Cursor.Current != c)
				Cursor.Current = c;
		}

		private void SetHitListTooltip(HitListEntry hle)
		{
			if (hle == null || hle.pi.Tooltip == null)
				_tt.Active = false;
			else
			{
				_tt.SetToolTip(this, hle.pi.Tooltip);
				_tt.Active = true;
			}
		}

		private HitListEntry GetHitListEntry(MouseEventArgs mea)
		{
			if (_HitList.Count <= 0)
				return null;

			PointF p = new PointF(mea.X / _LastZoom, mea.Y / _LastZoom);
			try 
			{
				foreach(HitListEntry hle in this._HitList)
				{
					if (hle.rect.Contains(p))
						return hle;
				}
			}
			catch  
			{
				// can get synchronization error due to multi-threading; just skip the error
			}
			return null;
		}

		private float PixelsX(float x)
		{
			return (x * DpiX / 72.0f);
		}

		private float PixelsY(float y)
		{
			return (y * DpiY / 72.0f);
		}

		// render all the objects in a page (or any composite object
		private void ProcessPage(Graphics g, IEnumerable p, RectangleF clipRect, bool bHitList)
		{
			foreach (PageItem pi in p)
			{
				if (pi is PageTextHtml)
				{	// PageTextHtml is actually a composite object (just like a page)
					ProcessHtml(pi as PageTextHtml, g, clipRect, bHitList);
					continue;
				}

				if (pi is PageLine)
				{
					PageLine pl = pi as PageLine;
					DrawLine(pl.SI.BColorLeft, pl.SI.BStyleLeft, pl.SI.BWidthLeft, 
						g, PixelsX(pl.X + _left - _hScroll), PixelsY(pl.Y + _top - _vScroll), 
						PixelsX(pl.X2 + _left - _hScroll), PixelsY(pl.Y2 + _top - _vScroll));
					continue;
				}

				RectangleF rect = new RectangleF(PixelsX(pi.X + _left - _hScroll), PixelsY(pi.Y + _top - _vScroll), 
																	PixelsX(pi.W), PixelsY(pi.H));

				// Maintain the hit list
				if (bHitList)	
				{
					// Only care about items with links and tips
					if (pi.HyperLink != null || pi.BookmarkLink != null || pi.Tooltip != null)
					{
						_HitList.Add(new HitListEntry(rect, pi));
					}
				}

				if (!rect.IntersectsWith(clipRect))
					continue;

				if (pi.SI.BackgroundImage != null)
				{	// put out any background image
					PageImage i = pi.SI.BackgroundImage;
					DrawImage(i, g, rect);
				}
				
				if (pi is PageText)
				{
					PageText pt = pi as PageText;
					DrawString(pt, g, rect);
				}
				else if (pi is PageImage)
				{
					PageImage i = pi as PageImage;
					DrawImage(i, g, rect);
				}
				else if (pi is PageRectangle)
				{
					this.DrawBackground(g, rect, pi.SI);
				}

				DrawBorder(pi, g, rect);
			}
		}

		private void DrawBackground(Graphics g, System.Drawing.RectangleF rect, StyleInfo si)
		{
			LinearGradientBrush linGrBrush = null;
			SolidBrush sb=null;
			try
			{
				if (si.BackgroundGradientType != BackgroundGradientTypeEnum.None &&
					!si.BackgroundGradientEndColor.IsEmpty &&
					!si.BackgroundColor.IsEmpty)
				{
					Color c = si.BackgroundColor;	
					Color ec = si.BackgroundGradientEndColor; 	

					switch (si.BackgroundGradientType)
					{
						case BackgroundGradientTypeEnum.LeftRight:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.Horizontal); 
							break;
						case BackgroundGradientTypeEnum.TopBottom:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.Vertical); 
							break;
						case BackgroundGradientTypeEnum.Center:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.Horizontal); 
							break;
						case BackgroundGradientTypeEnum.DiagonalLeft:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.ForwardDiagonal); 
							break;
						case BackgroundGradientTypeEnum.DiagonalRight:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.BackwardDiagonal); 
							break;
						case BackgroundGradientTypeEnum.HorizontalCenter:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.Horizontal); 
							break;
						case BackgroundGradientTypeEnum.VerticalCenter:
							linGrBrush = new LinearGradientBrush(rect, c, ec, LinearGradientMode.Vertical); 
							break;
						default:
							break;
					}
				}

				if (linGrBrush != null)
				{
					g.FillRectangle(linGrBrush, rect);
					linGrBrush.Dispose();
				}
				else if (!si.BackgroundColor.IsEmpty)
				{
					sb = new SolidBrush(si.BackgroundColor);
					g.FillRectangle(sb, rect);
					sb.Dispose();
				}
			}
			finally
			{
				if (linGrBrush != null)
					linGrBrush.Dispose();
				if (sb != null)
					sb.Dispose();
			}
			return;
		}

		private void DrawBorder(PageItem pi, Graphics g, RectangleF r)
		{
			if (r.Height <= 0 || r.Width <= 0)		// no bounding box to use
				return;
			
			StyleInfo si = pi.SI;

			DrawLine(si.BColorTop, si.BStyleTop, si.BWidthTop, g, r.X, r.Y, r.Right, r.Y);

			DrawLine(si.BColorRight, si.BStyleRight, si.BWidthRight, g, r.Right, r.Y, r.Right, r.Bottom);
			
			DrawLine(si.BColorLeft, si.BStyleLeft, si.BWidthLeft, g, r.X, r.Y, r.X, r.Bottom);
			
			DrawLine(si.BColorBottom, si.BStyleBottom, si.BWidthBottom, g, r.X, r.Bottom, r.Right, r.Bottom);

			return;
			
		}

		private void DrawImage(PageImage pi, Graphics g, RectangleF r)
		{
			Stream strm=null;
			System.Drawing.Image im=null;
			try 
			{
				strm = new MemoryStream(pi.ImageData); 
				im = System.Drawing.Image.FromStream(strm);
				DrawImageSized(pi, im, g, r);
			}
			finally
			{
				if (strm != null)
					strm.Close();
				if (im != null)
					im.Dispose();
			}
			
		}

		private void DrawImageSized(PageImage pi, Image im, Graphics g, RectangleF r)
		{
			float height, width;		// some work variables
			StyleInfo si = pi.SI;

			// adjust drawing rectangle based on padding
			RectangleF r2 = new RectangleF(r.Left + PixelsX(si.PaddingLeft),
				r.Top + PixelsY(si.PaddingTop),
				r.Width - PixelsX(si.PaddingLeft + si.PaddingRight),
				r.Height - PixelsY(si.PaddingTop + si.PaddingBottom));
		
			Rectangle ir;	// int work rectangle
			switch (pi.Sizing)
			{
				case ImageSizingEnum.AutoSize:
					// Note: GDI+ will stretch an image when you only provide
					//  the left/top coordinates.  This seems pretty stupid since
					//  it results in the image being out of focus even though
					//  you don't want the image resized.
                    if (g.DpiX == im.HorizontalResolution &&
                        g.DpiY == im.VerticalResolution)
                    {
                        ir = new Rectangle(Convert.ToInt32(r2.Left), Convert.ToInt32(r2.Top),
                                                        im.Width, im.Height);
                    }
                    else
                        ir = new Rectangle(Convert.ToInt32(r2.Left), Convert.ToInt32(r2.Top),
                                           Convert.ToInt32(r2.Width), Convert.ToInt32(r2.Height));
                    g.DrawImage(im, ir);

					break;
				case ImageSizingEnum.Clip:
					Region saveRegion = g.Clip;
					Region clipRegion = new Region(g.Clip.GetRegionData());
					clipRegion.Intersect(r2);
					g.Clip = clipRegion;
                    if (g.DpiX == im.HorizontalResolution &&
                        g.DpiY == im.VerticalResolution)
                    {
                        ir = new Rectangle(Convert.ToInt32(r2.Left), Convert.ToInt32(r2.Top),
                                                        im.Width, im.Height);
                    }
                    else
                        ir = new Rectangle(Convert.ToInt32(r2.Left), Convert.ToInt32(r2.Top),
                                           Convert.ToInt32(r2.Width), Convert.ToInt32(r2.Height));
                    g.DrawImage(im, ir);
                    g.Clip = saveRegion;
					break;
				case ImageSizingEnum.FitProportional:
					float ratioIm = (float) im.Height / (float) im.Width;
					float ratioR = r2.Height / r2.Width;
					height = r2.Height;
					width = r2.Width;
					if (ratioIm > ratioR)
					{	// this means the rectangle width must be corrected
						width = height * (1/ratioIm);
					}
					else if (ratioIm < ratioR)
					{	// this means the ractangle height must be corrected
						height = width * ratioIm;
					}
					r2 = new RectangleF(r2.X, r2.Y, width, height);
					g.DrawImage(im, r2);
					break;
				case ImageSizingEnum.Fit:
				default:
					g.DrawImage(im, r2);
					break;
			}
			return;
		}

		private void DrawLine(Color c, BorderStyleEnum bs, float w, Graphics g, 
								float x, float y, float x2, float y2)
		{
			if (bs == BorderStyleEnum.None || c.IsEmpty || w <= 0)	// nothing to draw
				return;

			Pen p=null;
			try
			{
				p = new Pen(c, w);
				switch (bs)
				{
					case BorderStyleEnum.Dashed:
						p.DashStyle = DashStyle.Dash;
						break;
					case BorderStyleEnum.Dotted:
						p.DashStyle = DashStyle.Dot;
						break;
					case BorderStyleEnum.Double:
					case BorderStyleEnum.Groove:
					case BorderStyleEnum.Inset:
					case BorderStyleEnum.Solid:
					case BorderStyleEnum.Outset:
					case BorderStyleEnum.Ridge:
					case BorderStyleEnum.WindowInset:
					default:
						p.DashStyle = DashStyle.Solid;		
						break;
				}

				g.DrawLine(p,  x, y, x2, y2);
			}
			finally
			{
				if (p != null)
					p.Dispose();
			}

		}

		private void ProcessHtml(PageTextHtml pth, Graphics g, RectangleF clipRect, bool bHitList)
		{
			pth.Build(g);				// Builds the subobjects that make up the html
			this.ProcessPage(g, pth, clipRect, bHitList);
		}

		private void DrawString(PageText pt, Graphics g, RectangleF r)
		{
			StyleInfo si = pt.SI;
			string s = pt.Text;

			Font drawFont=null;
			StringFormat drawFormat=null;
			Brush drawBrush=null;
			try
			{
				// STYLE
				System.Drawing.FontStyle fs = 0;
				if (si.FontStyle == FontStyleEnum.Italic)
					fs |= System.Drawing.FontStyle.Italic;

				switch (si.TextDecoration)
				{
					case TextDecorationEnum.Underline:
						fs |= System.Drawing.FontStyle.Underline;
						break;
					case TextDecorationEnum.LineThrough:
						fs |= System.Drawing.FontStyle.Strikeout;
						break;
					case TextDecorationEnum.Overline:
					case TextDecorationEnum.None:
						break;
				}

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
				drawFont = new Font(si.GetFontFamily(), si.FontSize, fs);	// si.FontSize already in points
				// ALIGNMENT
				drawFormat = new StringFormat();
				switch (si.TextAlign)
				{
					case TextAlignEnum.Right:
						drawFormat.Alignment = StringAlignment.Far;
						break;
					case TextAlignEnum.Center:
						drawFormat.Alignment = StringAlignment.Center;
						break;
					case TextAlignEnum.Left:
					default:
						drawFormat.Alignment = StringAlignment.Near;
						break;
				}
				if (pt.SI.WritingMode == WritingModeEnum.tb_rl)
				{
					drawFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					drawFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
				}
				switch (si.VerticalAlign)
				{
					case VerticalAlignEnum.Bottom:
						drawFormat.LineAlignment = StringAlignment.Far;
						break;
					case VerticalAlignEnum.Middle:
						drawFormat.LineAlignment = StringAlignment.Center;
						break;
					case VerticalAlignEnum.Top:
					default:
						drawFormat.LineAlignment = StringAlignment.Near;
						break;
				}
				// draw the background 
				DrawBackground(g, r, si);

				// adjust drawing rectangle based on padding
				RectangleF r2 = new RectangleF(r.Left + si.PaddingLeft,
											   r.Top + si.PaddingTop,
											   r.Width - si.PaddingLeft - si.PaddingRight,
											   r.Height - si.PaddingTop - si.PaddingBottom);
				
				drawBrush = new SolidBrush(si.Color);
				if (pt.NoClip)	// request not to clip text
					g.DrawString(pt.Text, drawFont, drawBrush, new PointF(r.Left, r.Top), drawFormat);
				else
					g.DrawString(pt.Text, drawFont, drawBrush, r2, drawFormat);

			}
			finally
			{
				if (drawFont != null)
					drawFont.Dispose();
				if (drawFormat != null)
					drawFont.Dispose();
				if (drawBrush != null)
					drawBrush.Dispose();
			}
		}
		
		class HitListEntry
		{
			internal RectangleF rect;
			internal PageItem pi;
			internal HitListEntry(RectangleF r, PageItem pitem)
			{
				rect = r;
				pi = pitem;
			}
		}
	}
}
