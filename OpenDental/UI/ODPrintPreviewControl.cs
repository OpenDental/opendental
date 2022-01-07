using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI {
	///<summary>Almost identical to base PrintPreviewControl but when using WEB this control will not show a print progress dialog.
	///Code copied from MSDN and modified.</summary>
	class ODPrintPreviewControl: PrintPreviewControl {
		#region Defaults
		private const double _defaultZoom=.3;
		///<summary>null if needs refreshing</summary>
		private PreviewPageInfo[] _arrayPageInfo;
		///<summary>Gets or sets the number of pages displayed vertically down the screen.</summary>
		private int _countDisplayedVerticalPages=1;
		///<summary>Gets or sets the number of pages displayed horizontally across the screen.</summary>
		private int _countDisplayHorizontalPages=1;
		///<summary>Gets or sets a value If true (default), resizing the control or changing the number of pages shown will automatically adjust Zoom to make everything visible.</summary>
		private bool autoZoom=true;
		///<summary>The virtual coordinate of the upper left visible pixel</summary>
		private Point position=new Point(0,0);
		///<summary>How big the control would be if the screen was infinitely large.</summary>
		private Size _virtualSize=new Size(1,1);
		///<summary>Mimics .Nets interanl print preview logic.</summary>
		public static readonly PrintingPermission _permSafePrinting=new PrintingPermission(PrintingPermissionLevel.SafePrinting);
		#endregion
		#region Constants
		///<summary>Spacing per page, in mm</summary>
		private const int _border=10;
		private const int SB_HORZ=0;
		private const int SB_VERT=1;
		private const int SIF_RANGE=0x0001;
		private const int SIF_PAGE=0x0002;
		private const int WM_VSCROLL=277;//0x115
		private const int SB_PAGEUP=2;
		private const int SB_PAGEDOWN=3;
		#endregion
		#region DLL import methods
		[DllImport("user32.dll",ExactSpelling=true,CharSet=CharSet.Auto)]
		[ResourceExposure(ResourceScope.None)]
		private static extern int SetScrollInfo(HandleRef hWnd,int fnBar,SCROLLINFO si,bool redraw);

		[DllImport("user32.dll",ExactSpelling=true,CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		[ResourceExposure(ResourceScope.None)]
		private static extern bool ScrollWindow(HandleRef hWnd,int nXAmount,int nYAmount,ref Rectangle rectScrollRegion,ref Rectangle rectClip);

		[DllImport("user32.dll",ExactSpelling=true,CharSet=CharSet.Auto,SetLastError=true)]
		[ResourceExposure(ResourceScope.None)]
		private static extern int SetScrollPos(HandleRef hWnd,int nBar,int nPos,bool bRedraw);
		#endregion
		#region The following are computed by ComputeLayout()
		///<summary></summary>
		private bool _isLayoutOk;
		///<summary>100ths of inch, not pixels</summary>
		private Size imageSize=System.Drawing.Size.Empty;
		///<summary></summary>
		private Point screendpi=Point.Empty;
		///<summary></summary>
		private double zoom=_defaultZoom;
		///<summary></summary>
		private bool pageInfoCalcPending;
		///<summary></summary>
		private bool isExceptionPrinting=false;
		#endregion

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(ModifierKeys.HasFlag(Keys.Control)) {
				Zoom=Math.Min(4.0,Math.Max(0.25,Zoom+(e.Delta<0?-0.25:0.25)));//max zoom 4x, min zoom 0.25x
			}
			else {
				MiscUtils.SendMessage(Handle,WM_VSCROLL,e.Delta<0?SB_PAGEDOWN:SB_PAGEUP,0);
			}
		}

		protected override void OnPaint(PaintEventArgs pevent) {
			if(!ODBuild.IsWeb()){//Always use base methods when not using web.
				base.OnPaint(pevent);
				return;
			}
			#region Web specific code
			//Code is almost identical to base.OnPaint(...) => https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Printing/PrintPreviewControl.cs,6256c0e1f6a3124b
			Brush backBrush=new SolidBrush(BackColor);
			try {
				if(_arrayPageInfo==null||_arrayPageInfo.Length==0) {
					#region No pages or Exception printing
					pevent.Graphics.FillRectangle(backBrush,ClientRectangle);
					if(_arrayPageInfo!=null||isExceptionPrinting) {
						// Calculate formats
						StringFormat format=new StringFormat();
						format.Alignment=StringAlignment.Center;//ControlPaint.TranslateAlignment(ContentAlignment.MiddleCenter);
						format.LineAlignment=StringAlignment.Center;// ControlPaint.TranslateLineAlignment(ContentAlignment.MiddleCenter);
						// Do actual drawing
						SolidBrush brush=new SolidBrush(ForeColor);
						try {
							if(isExceptionPrinting) {
								pevent.Graphics.DrawString("EXCEPTION PRINTING",Font,brush,ClientRectangle,format);
							}
							else {
								pevent.Graphics.DrawString("PRINT PREVIEW NO PAGES",Font,brush,ClientRectangle,format);
							}
						}
						finally {
							brush.Dispose();
							format.Dispose();
						}
					}
					else {
						BeginInvoke(new MethodInvoker(CalculatePageInfo));
					}
					#endregion
				}
				else {
					if(!_isLayoutOk) {
						ComputeLayout(pevent.Graphics);
					}
					Size controlPhysicalSize=new Size(PixelsToPhysical(new Point(Size), screendpi));
					//Point imagePixels=PhysicalToPixels(new Point(imageSize), screendpi);
					Point virtualPixels=new Point(_virtualSize);
					// center pages on screen if small enough
					Point offset=new Point(Math.Max(0, (Size.Width - virtualPixels.X) / 2),Math.Max(0, (Size.Height - virtualPixels.Y) / 2));
					offset.X-=position.X;
					offset.Y-=position.Y;
					int borderPixelsX=PhysicalToPixels(_border, screendpi.X);
					int borderPixelsY=PhysicalToPixels(_border, screendpi.Y);
					Region originalClip=pevent.Graphics.Clip;
					Rectangle[] pageRenderArea=new Rectangle[_countDisplayedVerticalPages * _countDisplayHorizontalPages];
					Point lastImageSize=Point.Empty;
					int maxImageHeight=0;
					try {
						for(int row=0;row<_countDisplayedVerticalPages;row++) {
							//Initialize our LastImageSize variable...
							lastImageSize.X=0;
							lastImageSize.Y=maxImageHeight*row;
							for(int column=0;column<_countDisplayHorizontalPages;column++) {
								int imageIndex=StartPage + column + row*_countDisplayHorizontalPages;
								if(imageIndex<_arrayPageInfo.Length) {
									Size pageSize=_arrayPageInfo[imageIndex].PhysicalSize;
									if(autoZoom) {
										double zoomX=((double) controlPhysicalSize.Width - _border*(_countDisplayHorizontalPages + 1)) / (_countDisplayHorizontalPages*pageSize.Width);
										double zoomY=((double) controlPhysicalSize.Height - _border*(_countDisplayedVerticalPages + 1)) / (_countDisplayedVerticalPages*pageSize.Height);
										zoom=Math.Min(zoomX,zoomY);
									}
									imageSize=new Size((int)(zoom*pageSize.Width),(int)(zoom*pageSize.Height));
									Point imagePixels=PhysicalToPixels(new Point(imageSize), screendpi);
									int x=offset.X + borderPixelsX * (column + 1) + lastImageSize.X;
									int y=offset.Y + borderPixelsY * (row + 1) + lastImageSize.Y;
									lastImageSize.X+=imagePixels.X;
									//The Height is the Max of any PageHeight..
									maxImageHeight=Math.Max(maxImageHeight,imagePixels.Y);
									pageRenderArea[imageIndex-StartPage]=new Rectangle(x,y,imagePixels.X,imagePixels.Y);
									pevent.Graphics.ExcludeClip(pageRenderArea[imageIndex-StartPage]);
								}
							}
						}
						pevent.Graphics.FillRectangle(backBrush,ClientRectangle);
					}
					finally {
						pevent.Graphics.Clip=originalClip;
					}
					for(int i=0;i<pageRenderArea.Length;i++) {
						if(i+StartPage<_arrayPageInfo.Length) {
							Rectangle box=pageRenderArea[i];
							pevent.Graphics.DrawRectangle(Pens.Black,box);
							using(SolidBrush brush=new SolidBrush(ForeColor)) {
								pevent.Graphics.FillRectangle(brush,box);
							}
							box.Inflate(-1,-1);
							if(_arrayPageInfo[i+StartPage].Image!=null) {
								pevent.Graphics.DrawImage(_arrayPageInfo[i+StartPage].Image,box);
							}
							box.Width--;
							box.Height--;
							pevent.Graphics.DrawRectangle(Pens.Black,box);
						}
					}
				}
			}
			finally {
				backBrush.Dispose();
			}
			#endregion
		}

		#region Web specific drawing code
		///<summary>This function computes everything in terms of physical size (millimeters), not pixels</summary>
		private void ComputeLayout(Graphics g) {
			//Debug.Assert(pageInfo != null,"Must call ComputePreview first");
			_isLayoutOk=true;
			if(_arrayPageInfo.Length==0) {
				ClientSize=Size;
				return;
			}
			/*Original .Net code
			Graphics tempGraphics=CreateGraphicsInternal();
      IntPtr dc=tempGraphics.GetHdc();
      screendpi=new Point(UnsafeNativeMethods.GetDeviceCaps(new HandleRef(tempGraphics, dc), NativeMethods.LOGPIXELSX),
                            UnsafeNativeMethods.GetDeviceCaps(new HandleRef(tempGraphics, dc), NativeMethods.LOGPIXELSY));
      tempGraphics.ReleaseHdcInternal(dc);
      tempGraphics.Dispose();
			*/
			screendpi=new Point((int)g.DpiX,(int)g.DpiY);
			Size pageSize=_arrayPageInfo[StartPage].PhysicalSize;
			Size controlPhysicalSize=new Size(PixelsToPhysical(new Point(Size), screendpi));
			if(autoZoom) {
				double zoomX=((double) controlPhysicalSize.Width - _border*(_countDisplayHorizontalPages + 1)) / (_countDisplayHorizontalPages*pageSize.Width);
				double zoomY=((double) controlPhysicalSize.Height - _border*(_countDisplayedVerticalPages + 1)) / (_countDisplayedVerticalPages*pageSize.Height);
				zoom=Math.Min(zoomX,zoomY);
			}
			imageSize=new Size((int)(zoom*pageSize.Width),(int)(zoom*pageSize.Height));
			int virtualX=(imageSize.Width * _countDisplayHorizontalPages) + _border * (_countDisplayHorizontalPages +1);
			int virtualY=(imageSize.Height * _countDisplayedVerticalPages) + _border * (_countDisplayedVerticalPages +1);
			SetVirtualSizeNoInvalidate(new Size(PhysicalToPixels(new Point(virtualX,virtualY),screendpi)));
		}

		private void SetVirtualSizeNoInvalidate(Size value) {
			_virtualSize=value;
			SetPositionNoInvalidate(position); // Make sure it's within range
			#region Original .Net code
			//NativeMethods.SCROLLINFO info=new NativeMethods.SCROLLINFO();
			//info.fMask=NativeMethods.SIF_RANGE|NativeMethods.SIF_PAGE;
			//info.nMin=0;
			//info.nMax=Math.Max(Height,virtualSize.Height)-1;
			//info.nPage=Height;
			//UnsafeNativeMethods.SetScrollInfo(new HandleRef(this,Handle),NativeMethods.SB_VERT,info,true);
			#endregion
			SCROLLINFO info=new SCROLLINFO();
			info.fMask=SIF_RANGE|SIF_PAGE;
			info.nMin=0;
			info.nMax=Math.Max(Height,_virtualSize.Height)-1;
			info.nPage=Height;
			SetScrollInfo(new HandleRef(this,Handle),SB_VERT,info,true);

			#region Original .Net code
			//info.fMask=NativeMethods.SIF_RANGE | NativeMethods.SIF_PAGE;
			//info.nMin=0;
			//info.nMax=Math.Max(Width,virtualSize.Width) - 1;
			//info.nPage=Width;
			//UnsafeNativeMethods.SetScrollInfo(new HandleRef(this,Handle),NativeMethods.SB_HORZ,info,true);
			#endregion
			info.fMask=SIF_RANGE|SIF_PAGE;
			info.nMin=0;
			info.nMax=Math.Max(Width,_virtualSize.Width)-1;
			info.nPage=Width;
			SetScrollInfo(new HandleRef(this,Handle),SB_HORZ,info,true);
		}

		private void SetPositionNoInvalidate(Point value) {
			Point current=position;

			position=value;
			position.X=Math.Min(position.X,_virtualSize.Width-Width);
			position.Y=Math.Min(position.Y,_virtualSize.Height-Height);
			if(position.X<0) position.X=0;
			if(position.Y<0) position.Y=0;

			#region Original .Net code
			//NativeMethods.RECT scroll=NativeMethods.RECT.FromXYWH(rect.X, rect.Y, rect.Width, rect.Height);
			//SafeNativeMethods.ScrollWindow(new HandleRef(this,Handle),
			//										 current.X - position.X,
			//										 current.Y - position.Y,
			//										 ref scroll,
			//										 ref scroll);
			//UnsafeNativeMethods.SetScrollPos(new HandleRef(this,Handle),NativeMethods.SB_HORZ,position.X,true);
			//UnsafeNativeMethods.SetScrollPos(new HandleRef(this,Handle),NativeMethods.SB_VERT,position.Y,true);
			#endregion
			Rectangle rect=ClientRectangle;
			Rectangle scroll=new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
			ScrollWindow(new HandleRef(this,Handle),current.X-position.X,current.Y-position.Y,ref scroll,ref scroll);
			SetScrollPos(new HandleRef(this,Handle),SB_HORZ,position.X,true);
			SetScrollPos(new HandleRef(this,Handle),SB_VERT,position.Y,true);
		}

		private void CalculatePageInfo() {
			if(pageInfoCalcPending) {
				return;
			}
			pageInfoCalcPending=true;
			try {
				if(_arrayPageInfo==null) {
					try {
						ComputePreview();
					}
					catch {
						isExceptionPrinting=true;
						throw;
					}
					finally {
						Invalidate();
					}
				}
			}
			finally {
				pageInfoCalcPending=false;
			}
		}

		///<summary>"Prints" the document to memory</summary>
		private void ComputePreview() {
			int oldStart=StartPage;
			if(Document==null)
				_arrayPageInfo=new PreviewPageInfo[0];
			else {
				_permSafePrinting.Demand();//Mimics original line of: IntSecurity.SafePrinting.Demand();
				PrintController oldController=Document.PrintController;
				PreviewPrintController previewController=new PreviewPrintController();
				previewController.UseAntiAlias=UseAntiAlias;
				if(ODBuild.IsWeb()) {
					Document.PrintController=previewController;
				}
				else {
					//.Net uses PrintControllerWithStatusDialog which breaks the web.
					//Document.PrintController=new PrintControllerWithStatusDialog(previewController,"DIALOG TEXT");
					throw new ODException("You can not use the ODWebPrintPreviewControl when not using WEB");
				}
				// Want to make sure we've reverted any security asserts before we call Print -- that calls into user code
				Document.Print();
				_arrayPageInfo=previewController.GetPreviewPageInfo();
				//Debug.Assert(pageInfo != null,"ReviewPrintController did not give us preview info");
				Document.PrintController=oldController;
			}
			if(oldStart!=StartPage) {
				OnStartPageChanged(EventArgs.Empty);
			}
		}

		private int PhysicalToPixels(int physicalSize,int dpi) {
			return (int)(physicalSize*dpi/100.0);
		}

		private Point PhysicalToPixels(Point physical,Point dpi) {
			return new Point(PhysicalToPixels(physical.X,dpi.X),PhysicalToPixels(physical.Y,dpi.Y));
		}

		private int PixelsToPhysical(int pixels,int dpi) {
			return (int)(pixels*100.0/dpi);
		}

		private Point PixelsToPhysical(Point pixels,Point dpi) {
			return new Point(PixelsToPhysical(pixels.X,dpi.X),PixelsToPhysical(pixels.Y,dpi.Y));
		}

		[StructLayout(LayoutKind.Sequential)]
		private class SCROLLINFO {
			public int cbSize=Marshal.SizeOf(typeof(SCROLLINFO));
			public int fMask;
			public int nMin;
			public int nMax;
			public int nPage;
			public int nPos;
			public int nTrackPos;

			public SCROLLINFO() {
			}

			public SCROLLINFO(int mask,int min,int max,int page,int pos) {
				fMask=mask;
				nMin=min;
				nMax=max;
				nPage=page;
				nPos=pos;
			}
		}
		#endregion
	}
}
