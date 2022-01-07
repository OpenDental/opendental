using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>This is a full replacement for GDI+ Graphics.  Use it in Paint.  To get started, see the 3 From...() methods. There are also examples at the bottom of this file.  This is internally double buffered.  It's a work in progress.</summary>
	public sealed class Direct2d:IDisposable{
		private Size _size;
		private Control _control;
		private Graphics _graphics;
		private IntPtr _pDirect2dWrapper;
		private IntPtr _pD2DWrapperHwnd;
		private IntPtr _hWnd;
		private bool _isDrawingPath;
		private EnumContext _context;

		private enum EnumContext{
			WindowHandle,
			Bitmap,
			Graphics
		}

		#region Constructors
		private Direct2d(Control control){
			//Similar to Graphics, this has no public constructor.
			_context=EnumContext.WindowHandle;
			_control=control;
			//_size=control.ClientSize;
			//_hWnd=control.Handle;//might change
			_pD2DWrapperHwnd=D2DWrapperHwnd.WrapperHwnd_Create();
		}

		///<summary></summary>
		private Direct2d(Bitmap bitmap){
			_context=EnumContext.Bitmap;
			_size=bitmap.Size;
			_graphics=Graphics.FromImage(bitmap);
			_pDirect2dWrapper=Direct2dWrapper.Wrapper_Create();
		}

		///<summary></summary>
		private Direct2d(Graphics graphics){
			_context=EnumContext.Graphics;
			_size=graphics.VisibleClipBounds.Size.ToSize();
			_graphics=graphics;
			_pDirect2dWrapper=Direct2dWrapper.Wrapper_Create();
		}

		///<summary>Releases wrapper resources, disposes of graphics, and releases references.</summary>
		public void Dispose(){
			ReleaseWrapperResources();
			if(_context==EnumContext.WindowHandle){
				_control=null;
			}
			if(_context==EnumContext.Bitmap){
				_graphics.Dispose();//because we created the graphics
				_graphics=null;
			}
			if(_context==EnumContext.Graphics){
				_graphics=null;//release the external graphics
			}
			//for Bitmap and Graphics types, the HDC gets released earlier, at the end of drawing, to make the drawing show
		}

		private void ReleaseWrapperResources(){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Delete(_pD2DWrapperHwnd);
			}
			else{
				Direct2dWrapper.Wrapper_ReleaseDeviceResources(_pDirect2dWrapper);
				Direct2dWrapper.Wrapper_Delete(_pDirect2dWrapper);
			}
		}

		///<summary>Supply the Control that you are painting on.  Surround drawing with BeginDraw/EndDraw.  Dispose when control disposed.  Direct2D paints on the entire control and does not allow GDI+ drawing commands to intermingle.</summary>
		public static Direct2d FromControl(Control control){
			Direct2d direct2D;
			direct2D=new Direct2d(control);
			direct2D.Initialize();
			return direct2D;
		}

		///<summary>Supply the bitmap that you are painting on.  Surround drawing with BeginDraw/EndDraw and Dispose when done.  Haven't quite figured out how to do a transparent background, and it may not be possible. So, you must first call d.Clear or some other background to avoid ragged edges.</summary>
		public static Direct2d FromBitmap(Bitmap bitmap){
			//this is never cached, and depends on user calling Dispose.
			Direct2d direct2D=new Direct2d(bitmap);
			direct2D.Initialize();
			return direct2D;
		}

		///<summary>This allows sending both GDI+ and Direct2D drawing commands to the same Graphics object.  Surround drawing with BeginDraw/EndDraw and dispose when done.  This looks pretty good, but it does leave a few stray lines of pixels when resizing.  GDI+ and Direct2D don't work flawlessly together.  And, since we have to build a new Direct2D each time, it might be slightly slow.</summary>
		public static Direct2d FromGraphics(Graphics graphics){
			//caching was attempted, but doesn't work because a new graphics object gets sent in each time anyway
			Direct2d direct2D;
			direct2D=new Direct2d(graphics);
			direct2D.Initialize();
			return direct2D;
		}
		#endregion Constructors

		#region Methods Initialize
		public void Initialize(){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Initialize(_pD2DWrapperHwnd);//ensures we have a static factory instantiated. Probably reuses the existing one.
			}
			else{
				Direct2dWrapper.Wrapper_Initialize(_pDirect2dWrapper);
			}
		}

		///<summary>Only tests the render target, but that test determines whether all the other device resources (render target, gradients, and bitmaps) need to be recreated as well.</summary>
		public bool DeviceResourcesNeedCreate(){
			if(_context==EnumContext.WindowHandle){
				return D2DWrapperHwnd.WrapperHwnd_DeviceResourcesNeedRecreate(_pD2DWrapperHwnd);
			}
			else{
				//not supported
				return true;
			}
		}

		///<summary>Only gets called if DeviceResourcesNeedCreate is true.</summary>
		public void CreateRenderTarget(){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_CreateRenderTarget(_pD2DWrapperHwnd,_control.Width,_control.Height,_control.Handle);
			}
			else{
				//not supported
			}
		}

		///<summary>Only gets called if DeviceResourcesNeedCreate is true. brushNum is 0 index item in a list of brushes.</summary>
		public void CreateGradientBrush(int brushNum,Color color1,Color color2,float x1,float y1,float x2,float y2){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_CreateGradientBrush(_pD2DWrapperHwnd,brushNum,color1.ToArgb(),color2.ToArgb(),x1,y1,x2,y2);
			}
			else{
				Direct2dWrapper.Wrapper_CreateGradientBrush(_pDirect2dWrapper,brushNum,color1.ToArgb(),color2.ToArgb(),x1,y1,x2,y2);
			}
		}

		//Todo: Atlas effect to pack multiple icons or thumbnails into a single image.
		///<summary>Only call this if DeviceResourcesNeedCreate is true or bitmap changes.  bitmapNum is 0 index item in a list of bitmaps.</summary>
		public void CreateBitmap(int bitmapNum,Bitmap bitmap){
			if(_context==EnumContext.WindowHandle){
				int width=bitmap.Width;
				int height=bitmap.Height;
				//bitmap.Save(@"E:\Documents\Temp\bitmap.gif");
				BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.ReadWrite,PixelFormat.Format32bppPArgb);//supports transparency
				IntPtr intPtrBitmap = bitmapData.Scan0;
				byte[] bytesBitmap=new byte[Math.Abs(bitmapData.Stride) * bitmap.Height];
				Marshal.Copy(intPtrBitmap,bytesBitmap,0,bytesBitmap.Length);
				//It does work to pass the pointer to C++ instead of bytes, but it adds complexity.
				bitmap.UnlockBits(bitmapData);
				bitmap.Dispose();
				D2DWrapperHwnd.WrapperHwnd_CreateBitmap(_pD2DWrapperHwnd,bitmapNum,bytesBitmap,width,height);
			}
			else{
				//not supported
			}
		}

		public void CreateBitmapBlank(int bitmapNum,int width,int height){
			D2DWrapperHwnd.WrapperHwnd_CreateBitmapBlank(_pD2DWrapperHwnd,bitmapNum,width,height);
		}

		///<summary>After this, call BeginDraw, do all drawing, EndDraw, SetTargetToOriginal.</summary>
		public void SetTargetToBitmap(int bitmapNum){
			D2DWrapperHwnd.WrapperHwnd_SetTargetToBitmap(_pD2DWrapperHwnd,bitmapNum);
		}

		///<summary>Use after SetTargetToBitmap after completely done drawing on the bitmap.</summary>
		public void SetTargetToOriginal(){
			D2DWrapperHwnd.WrapperHwnd_SetTargetToOriginal(_pD2DWrapperHwnd);
		}


		/*
		public static extern void WrapperHwnd_CreateBitmapBlank(IntPtr pD2DWrapperHwnd,int bitmapNum,int width,int height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_SetTargetToBitmap(IntPtr pD2DWrapperHwnd,int bitmapNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_SetTargetToOriginal(IntPtr pD2DWrapperHwnd);
		*/

		public void BeginDraw(){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_BeginDraw(_pD2DWrapperHwnd);
			}
			else{
				IntPtr hdc=_graphics.GetHdc();
				Direct2dWrapper.Wrapper_BeginDraw(_pDirect2dWrapper,_size.Width,_size.Height,hdc);
			}
		}

		///<summary>Calls EndDraw and releases Hdc.</summary>
		public void EndDraw(){
			if(_context==EnumContext.WindowHandle){
				int hr=D2DWrapperHwnd.WrapperHwnd_EndDraw(_pD2DWrapperHwnd);
				if(hr!=0){
					throw new ApplicationException("EndDraw failure.  Hresult="+hr.ToString());
				}
			}
			else{
				Direct2dWrapper.Wrapper_EndDraw(_pDirect2dWrapper);
				_graphics.ReleaseHdc();//important to be able to see results
			}
		}
		#endregion Methods Initialize

		#region Drawing
		//All commands from here down are alphabetical
		///<summary>Add an arc segment to a path.</summary>
		public void AddArc(float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW){
			if(!_isDrawingPath){
				throw new Exception("Must first call BeginPath.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_AddArc(_pD2DWrapperHwnd,x,y,width,height,rotation,isLargeArc,isCW);
			}
			else{
				Direct2dWrapper.Wrapper_AddArc(_pDirect2dWrapper,x,y,width,height,rotation,isLargeArc,isCW);
			}
		}

		public void AddBezier(float x1,float y1,float x2,float y2,float x,float y){
			if(!_isDrawingPath){
				throw new Exception("Must first call BeginPath.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_AddBezier(_pD2DWrapperHwnd,x1,y1,x2,y2,x,y);
			}
			else{
				Direct2dWrapper.Wrapper_AddBezier(_pDirect2dWrapper,x1,y1,x2,y2,x,y);
			}
		}

		///<summary>Add a line segment to a path.</summary>
		public void AddLine(float x,float y){
			if(!_isDrawingPath){
				throw new Exception("Must first call BeginPath.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_AddLine(_pD2DWrapperHwnd,x,y);
			}
			else{
				Direct2dWrapper.Wrapper_AddLine(_pDirect2dWrapper,x,y);
			}
		}

		public void AddQuadraticBezier(float x1,float y1,float x,float y){
			if(!_isDrawingPath){
				throw new Exception("Must first call BeginPath.");
			}
			Direct2dWrapper.Wrapper_AddQuadraticBezier(_pDirect2dWrapper,x1,y1,x,y);
		}

		///<summary>If this figure will have a fill applied, use isFilled=true.</summary>
		public void BeginFigure(float xStart,float yStart,bool isFilled){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_BeginFigure(_pD2DWrapperHwnd,xStart,yStart,isFilled);
			}
			else{
				Direct2dWrapper.Wrapper_BeginFigure(_pDirect2dWrapper,xStart,yStart,isFilled);
			}
		}

		///<summary>Call BeginFigure after this.</summary>
		public void BeginPath(){
			if(_isDrawingPath){
				throw new Exception("Already drawing a path. EndPath was not called.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_BeginPath(_pD2DWrapperHwnd);
			}
			else{
				Direct2dWrapper.Wrapper_BeginPath(_pDirect2dWrapper);
			}
			_isDrawingPath=true;
		}

		///<summary>Default is white.</summary>
		public void Clear(Color? color=null){
			if(color is null){
				color=Color.White;
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Clear(_pD2DWrapperHwnd,color.Value.ToArgb());
			}
			else{
				Direct2dWrapper.Wrapper_Clear(_pDirect2dWrapper,color.Value.ToArgb());
			}
		}

		public void DrawAtlas(int bitmapNum,int xSource,int ySource,int sizeSource,int x, int y,int size){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_DrawAtlas(_pD2DWrapperHwnd,bitmapNum,xSource,ySource,sizeSource,x,y,size);
			}
			//not supported yet
		}

		public void DrawBitmap(int bitmapNum,int x,int y,int width,int height){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_DrawBitmap(_pD2DWrapperHwnd,bitmapNum,x,y,width,height);
			}
			//not supported yet
		}

		public void DrawBitmapImmediate(Bitmap bitmap,Rectangle rectangle){  //int x,int y,int width,int height){
			if(_context==EnumContext.WindowHandle){
				//bitmap.Save(@"E:\Documents\Temp\bitmap.gif");
				BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.ReadWrite,PixelFormat.Format32bppPArgb);//supports transparency
				IntPtr intPtrBitmap = bitmapData.Scan0;
				byte[] bytesBitmap=new byte[Math.Abs(bitmapData.Stride) * bitmap.Height];
				Marshal.Copy(intPtrBitmap,bytesBitmap,0,bytesBitmap.Length);
				//It does work to pass the pointer to C++ instead of bytes, but it adds complexity.
				bitmap.UnlockBits(bitmapData);
				D2DWrapperHwnd.WrapperHwnd_DrawBitmapImmediate(_pD2DWrapperHwnd,bytesBitmap,bitmap.Width,bitmap.Height,rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height);
				bitmap.Dispose();
			}
			//not supported yet
		}

		public void DrawEllipse(Color color,float cx,float cy,float rx,float ry,float strokeWidth=1f){
			Direct2dWrapper.Wrapper_DrawEllipse(_pDirect2dWrapper,color.ToArgb(),cx,cy,rx,ry,strokeWidth);
		}

		public void DrawLine(Color color,float x1,float y1,float x2,float y2,float strokeWidth=1f){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_DrawLine(_pD2DWrapperHwnd,color.ToArgb(),x1,y1,x2,y2,strokeWidth);
			}
			else{
				Direct2dWrapper.Wrapper_DrawLine(_pDirect2dWrapper,color.ToArgb(),x1,y1,x2,y2,strokeWidth);
			}
		}

		public void DrawText(Color color,float x,float y,float width,float height,float fontSize,string text){
			if(_context==EnumContext.WindowHandle){
				float fontSizeDip=fontSize/72f*96f;//fontSize in DIP is 1/96" insted of 1/72"
				D2DWrapperHwnd.WrapperHwnd_DrawText(_pD2DWrapperHwnd,x,y,width,height,color.ToArgb(),fontSizeDip,text);
			}
			//not supported yet
		}

		public void DrawRectangle(Color color,float x,float y,float width,float height,float strokeWidth=1f){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_DrawRectangle(_pD2DWrapperHwnd,color.ToArgb(),x,y,width,height,strokeWidth);
			}
			else{
				Direct2dWrapper.Wrapper_DrawRectangle(_pDirect2dWrapper,color.ToArgb(),x,y,width,height,strokeWidth);
			}
		}

		public void DrawRoundedRectangle(Color color,float x,float y,float width,float height,float radiusX,float strokeWidth=1f){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_DrawRoundedRectangle(_pD2DWrapperHwnd,color.ToArgb(),x,y,width,height,radiusX,strokeWidth);
			}
			else{
				//not yet
			}
		}

		///<summary></summary>
		public void EndFigure(bool isClosed=true){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_EndFigure(_pD2DWrapperHwnd,isClosed);
			}
			else{
				Direct2dWrapper.Wrapper_EndFigure(_pDirect2dWrapper,isClosed);
			}
		}

		///<summary>Make sure to call EndFigure first.  Frequently only a few of these parameters are needed.  Name the ones you need and ignore the rest. Default colors are black.  You can do fill and/or outline.</summary>
		public void EndPath(bool isFilled=true,bool isOutline=true,Color? colorFill=null,Color? colorOutline=null,float strokeWidth=1){
			if(!_isDrawingPath){
				throw new Exception("Must first call BeginPath and add segments.");
			}
			if(!isFilled && !isOutline){
				throw new Exception("Must specify either a fill or an outline.");
			}
			if(colorFill is null){
				colorFill=Color.Black;
			}
			if(colorOutline is null){
				colorOutline=Color.Black;
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_EndPath(_pD2DWrapperHwnd,isFilled,isOutline,colorFill.Value.ToArgb(),colorOutline.Value.ToArgb(),strokeWidth);
			}
			else{
				Direct2dWrapper.Wrapper_EndPath(_pDirect2dWrapper,isFilled,isOutline,colorFill.Value.ToArgb(),colorOutline.Value.ToArgb(),strokeWidth);
			}
			_isDrawingPath=false;
		}

		public void FillEllipse(Color color,float cx,float cy,float rx,float ry){
			Direct2dWrapper.Wrapper_FillEllipse(_pDirect2dWrapper,color.ToArgb(),cx,cy,rx,ry);
		}

		public void FillRectangle(Color color,float x,float y,float width,float height){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_FillRectangle(_pD2DWrapperHwnd,color.ToArgb(),x,y,width,height);
			}
			else{
				Direct2dWrapper.Wrapper_FillRectangle(_pDirect2dWrapper,color.ToArgb(),x,y,width,height);
			}
		}

		///<summary>Specify gradientNum=0 until we need and support multiple gradients</summary>
		public void FillRectangleGradient(int gradientNum,float x,float y,float width,float height){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_FillRectangleGradient(_pD2DWrapperHwnd,gradientNum,x,y,width,height);
			}
			else{
				Direct2dWrapper.Wrapper_FillRectangleGradient(_pDirect2dWrapper,gradientNum,x,y,width,height);
			}
		}

		public void FillRoundedRectangle(Color color,float x,float y,float width,float height,float radiusX){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_FillRoundedRectangle(_pD2DWrapperHwnd,color.ToArgb(),x,y,width,height,radiusX);
			}
			else{
				Direct2dWrapper.Wrapper_FillRoundedRectangle(_pDirect2dWrapper,color.ToArgb(),x,y,width,height,radiusX);
			}
		}

		///<summary>Use after SaveDrawingState.</summary>
		public void RestoreDrawingState(int levelNum=0){
			if(levelNum>1){
				throw new Exception("Only zero and one are supported for now.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_RestoreDrawingState(_pD2DWrapperHwnd,levelNum);
			}
			else{
				Direct2dWrapper.Wrapper_RestoreDrawingState(_pDirect2dWrapper,levelNum);
			}
		}

		///<summary></summary>
		public void Rotate(float angle){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Rotate(_pD2DWrapperHwnd,angle);
			}
			else{
				Direct2dWrapper.Wrapper_Rotate(_pDirect2dWrapper,angle);
			}
		}

		///<summary>Caller must keep track of which level they want to call.  Also see RestoreDrawingState.</summary>
		public void SaveDrawingState(int levelNum=0){
			if(levelNum>1){
				throw new Exception("Only zero and one are supported for now.");
			}
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_SaveDrawingState(_pD2DWrapperHwnd,levelNum);
			}
			else{
				Direct2dWrapper.Wrapper_SaveDrawingState(_pDirect2dWrapper,levelNum);
			}
		}

		///<summary></summary>
		public void Scale(float scale){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Scale(_pD2DWrapperHwnd,scale);
			}
			else{
				Direct2dWrapper.Wrapper_Scale(_pDirect2dWrapper,scale);
			}
		}

		///<summary></summary>
		public void Translate(float x,float y){
			if(_context==EnumContext.WindowHandle){
				D2DWrapperHwnd.WrapperHwnd_Translate(_pD2DWrapperHwnd,x,y);
			}
			else{
				Direct2dWrapper.Wrapper_Translate(_pDirect2dWrapper,x,y);
			}
		}
		#endregion Drawing

	}

	#region D2DWrapperHwnd methods
	///<summary>This class is just a container for the DllImport methods that call the C++ dll.  This group of methods is for drawing on HWNDs.</summary>
	public class D2DWrapperHwnd{
		#region Initialize
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WrapperHwnd_Create();

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WrapperHwnd_Initialize(IntPtr pD2DWrapperHwnd);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WrapperHwnd_DeviceResourcesNeedRecreate(IntPtr pD2DWrapperHwnd);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WrapperHwnd_CreateRenderTarget(IntPtr pD2DWrapperHwnd,int width,int height,IntPtr hwnd);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_CreateGradientBrush(IntPtr pD2DWrapperHwnd,int brushNum,int color1,int color2,float x1,float y1,float x2,float y2);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_CreateBitmap(IntPtr pD2DWrapperHwnd,int bitmapNum,byte[] bytesBitmap,int width,int height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_CreateBitmapBlank(IntPtr pD2DWrapperHwnd,int bitmapNum,int width,int height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_SetTargetToBitmap(IntPtr pD2DWrapperHwnd,int bitmapNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_SetTargetToOriginal(IntPtr pD2DWrapperHwnd);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_BeginDraw(IntPtr pD2DWrapperHwnd);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WrapperHwnd_EndDraw(IntPtr pD2DWrapperHwnd);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WrapperHwnd_Delete(IntPtr pD2DWrapperHwnd);
		#endregion Initialize

		//From here down everything is alphabetical-------------------------------------------------------------------
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_AddArc(IntPtr pD2DWrapperHwnd,float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_AddBezier(IntPtr pD2DWrapperHwnd,float x1,float y1,float x2,float y2,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_AddLine(IntPtr pD2DWrapperHwnd,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_AddQuadraticBezier(IntPtr pD2DWrapperHwnd,float x1,float y1,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_BeginFigure(IntPtr pD2DWrapperHwnd,float x,float y,bool isFilled);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_BeginPath(IntPtr pD2DWrapperHwnd);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_Clear(IntPtr pD2DWrapperHwnd,int color);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawAtlas(IntPtr pD2DWrapperHwnd,int bitmapNum,int xSource,int ySource,int sizeSource,int x,int y,int size);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawBitmap(IntPtr pD2DWrapperHwnd,int bitmapNum,int x,int y,int width,int height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawBitmapImmediate(IntPtr pD2DWrapperHwnd,byte[] bytesBitmap,int widthBitmap,int heightBitmap,int x,int y,int width,int height);
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawEllipse(IntPtr pD2DWrapperHwnd,int color,float cx,float cy,float rx,float ry,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawLine(IntPtr pD2DWrapperHwnd,int color,float x1,float y1,float x2,float y2,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawRectangle(IntPtr pD2DWrapperHwnd,int color,float x,float y,float width,float height,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_DrawRoundedRectangle(IntPtr pD2DWrapperHwnd,int color,float x,float y,float width,float height,float radiusX,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
		public static extern void WrapperHwnd_DrawText(IntPtr pD2DWrapperHwnd,float x,float y,float width,float height,int color,float fontSize,string text);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_EndFigure(IntPtr pD2DWrapperHwnd,bool isClosed);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_EndPath(IntPtr pD2DWrapperHwnd,bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_FillEllipse(IntPtr pD2DWrapperHwnd,int color,float cx,float cy,float rx,float ry);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_FillRectangle(IntPtr pD2DWrapperHwnd,int color,float x,float y,float width,float height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_FillRectangleGradient(IntPtr pD2DWrapperHwnd,int gradientNum,float x,float y,float width,float height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_FillRoundedRectangle(IntPtr pD2DWrapperHwnd,int color,float x,float y,float width,float height,float radiusX);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_RestoreDrawingState(IntPtr pD2DWrapperHwnd,int levelNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_Rotate(IntPtr pD2DWrapperHwnd,float angle);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_SaveDrawingState(IntPtr pD2DWrapperHwnd,int levelNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_Scale(IntPtr pD2DWrapperHwnd,float scale);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WrapperHwnd_Translate(IntPtr pD2DWrapperHwnd,float x,float y);
	}
	#endregion D2DWrapperHwnd methods

	#region Direct2dWrapper methods
	///<summary>This class is just a container for the DllImport methods that call the C++ dll.  This group of methods is for drawing on DCs.</summary>
	public class Direct2dWrapper{
		#region Initialize
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Wrapper_Create();

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Wrapper_Initialize(IntPtr pDirect2dWrapper);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Wrapper_BeginDraw(IntPtr pDirect2dWrapper,int width,int height,IntPtr hdc);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_CreateGradientBrush(IntPtr pDirect2dWrapper,int brushNum,int color1,int color2,float x1,float y1,float x2,float y2);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Wrapper_EndDraw(IntPtr pDirect2dWrapper);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_ReleaseDeviceResources(IntPtr pDirect2dWrapper);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Wrapper_Delete(IntPtr pDirect2dWrapper);
		#endregion Initialize

		//From here down everything is alphabetical-------------------------------------------------------------------
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_AddArc(IntPtr pDirect2dWrapper,float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_AddBezier(IntPtr pDirect2dWrapper,float x1,float y1,float x2,float y2,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_AddLine(IntPtr pDirect2dWrapper,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_AddQuadraticBezier(IntPtr pDirect2dWrapper,float x1,float y1,float x,float y);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_BeginFigure(IntPtr pDirect2dWrapper,float x,float y,bool isFilled);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_BeginPath(IntPtr pDirect2dWrapper);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_Clear(IntPtr pDirect2dWrapper,int color);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_DrawEllipse(IntPtr pDirect2dWrapper,int color,float cx,float cy,float rx,float ry,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_DrawLine(IntPtr pDirect2dWrapper,int color,float x1,float y1,float x2,float y2,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_DrawRectangle(IntPtr pDirect2dWrapper,int color,float x,float y,float width,float height,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_EndFigure(IntPtr pDirect2dWrapper,bool isClosed);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_EndPath(IntPtr pDirect2dWrapper,bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_FillEllipse(IntPtr pDirect2dWrapper,int color,float cx,float cy,float rx,float ry);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_FillRectangle(IntPtr pDirect2dWrapper,int color,float x,float y,float width,float height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_FillRectangleGradient(IntPtr pDirect2dWrapper,int gradientNum,float x,float y,float width,float height);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_FillRoundedRectangle(IntPtr pDirect2dWrapper,int color,float x,float y,float width,float height,float radiusX);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_RestoreDrawingState(IntPtr pDirect2dWrapper,int levelNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_Rotate(IntPtr pDirect2dWrapper,float angle);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_SaveDrawingState(IntPtr pDirect2dWrapper,int levelNum);

		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_Scale(IntPtr pDirect2dWrapper,float scale);
		
		[DllImport("Direct2dWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Wrapper_Translate(IntPtr pDirect2dWrapper,float x,float y);

	}
	#endregion Direct2dWrapper methods
}

/* Coordinate system:
Coordinates are "pixel edge" not "pixel center", so coordinates will be off by .5 pixels compared to GDI+
This is more mathematically correct than GDI
Rasterization has rules, as seen in the antialiased section of this page (not very good):
https://docs.microsoft.com/en-us/windows/win32/direct3d11/d3d10-graphics-programming-guide-rasterizer-stage-rules
This page is better:
https://docs.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates
Lines are drawn as rectangles with a width of 1, centered on the given coordinates. 
If you want single pixel-width lines, draw on pixel centers.
We usually think of rectangle drawing/filling, as the exterior of the rectangle (inset in GDI+ terminology)
So, to draw a rectangle "inset", so that the line is completely within a rectangle 5x5, use center coords:
DrawRectangle x=0.5,y=0.5,w=4,h=4  Rectangles will nearly always have .5 added to x and y like this to look good.
But to fill a rectangle, use edge coords:
FillRectangle x=0,y=0,w=5,h=5
Lines follow a different rule, since they use x1y1-x2y2.
Horizontal line uses center coords for y and edge coords for x:
x1=0,y1=0.5,x2=5,y2=0.5  (notice that x2 is specifying right, not width.  This can be confusing)
*/


/* Typical example 
Direct2d d=Direct2d.FromControl(this);
Inside of OnPaint():
d.Clear();
d.DrawLine(Color.Blue,25,25,30,55);
d.EndDraw();
//Call Dispose when control is disposed

Example for drawing on a bitmap:
Direct2d d=Direct2d.FromBitmap(bitmap);
d.Clear();
d.DrawLine(Color.Blue,25,25,30,55);
d.EndDraw();
d.Dispose();//because it's only used once

Example for intermingling with GDI+ drawing commands:
Direct2d d=Direct2d.FromGraphics(g);
d.DrawLine(Color.Blue,25,25,30,55);
d.EndDraw();
d.Dispose();

*/