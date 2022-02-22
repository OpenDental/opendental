/* File: OpenGLWinFormsControl.cs
 * Description: A Windows Forms control that supports OpenGL rendering using the Tao library.
 * Author: Michael Hansen (hansen.mike@gmail.com)
 * Date Created: 3/2/2006
 * Date Modified: 3/30/2006
 * 
 * Overview
 * ========
 * This control provides a simple Windows Forms control to do OpenGL render using Tao. The control
 * behaves properly in design mode (while being designed and while embedded in another control that's
 * being designed).
 * 
 * Example Usage
 * =============
 * To use this control, add it to your Form or Control using the designer or programmatically.
 * In your initialization routine, you MUST set TaoRenderEnabled to true for the control to use
 * OpenGL rendering (this is initially set to false to allow for designing).
 * 
 * For the most basic usage, add event handlers to the TaoSetupContext and TaoRenderScene events.
 * In TaoSetupContext, you will do your normal OpenGL setup routine.
 * In TaoRenderScene, you will do the actual drawing (by default, glFinish will be called for you after rendering).
 * During your initialization routine, call TaoInitializeContexts; this will create the device
 * and rendering contexts as well as call your TaoSetupContext event handler.
 * During each frame, call TaoDraw to redraw the scene. This will call Invalidate on the control and
 * call your TaoRenderScene event handler.
 * 
 * The step-by-step usage of the control would then be this:
 * 1. Add event handlers to TaoSetupContext (OpenGL initialization) and TaoRenderScene (rendering)
 * 2. Call TaoInitializeContexts
 * 3. Set TaoRenderEnabled to true
 * 4. Call TaoDraw during each frame
 * 
 * Advanced Usage
 * ==============
 * For more advanced usage, you may set the number of bits for the accumulator (TaoAccumBits),
 * color depth (TaoColorBits), depth buffer (TaoDepthBits), and stencil buffer (TaoStencilBits)
 * before calling TaoInitializeContexts.
 * 
 * Add an event handler to TaoControlSizeChanged to be notified whenever the control resizes.
 * The event arguments will give you the control's new width and height.  Adding a handler to this
 * event will disable the default resizing behavior of the control (reset the viewport and redraw).
 * 
 * Add an event handler to TaoOpenGLError to receive notifications of any errors that occur during
 * rendering. The event arguments will give you the error code and a brief description of the
 * error that occurred.
 */

#region Imported Namespaces

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;


using Tao.OpenGl;
using Tao.Platform.Windows;

#endregion

namespace CodeBase {
	/// <summary>
	/// A Windows Forms control that supports OpenGL rendering using the Tao library.
	/// </summary>
	public class OpenGLWinFormsControl:Control {
		#region Protected Fields

		protected IntPtr deviceContext = IntPtr.Zero,renderContext = IntPtr.Zero;
		protected bool renderEnabled = false;

		protected bool autoMakeCurrent = true,autoSwapBuffers = false,usehardware=true;
		public bool autoFinish=false;

		protected byte accumBits = 0,colorBits = 32,depthBits = 16,stencilBits = 0;
		protected int lastErrorCode = Gl.GL_NO_ERROR;

		#endregion

		#region Events

		/// <summary>
		/// A user-defined event that renders the scene (called during each redraw).
		/// </summary>
		public event EventHandler TaoRenderScene;

		/// <summary>
		/// A user-defined event that sets up the OpenGL context (called once during TaoInitializeContexts).
		/// </summary>
		public event EventHandler TaoSetupContext;

		/// <summary>
		/// A user-defined event that's called when the control resizes
		/// (by default, the control resets the viewport and redraws itself).
		/// </summary>
		public event EventHandler<SizeChangedEventArgs> TaoControlSizeChanged;

		/// <summary>
		/// Fired whenever an error occurs during rendering.
		/// </summary>
		public event EventHandler<OpenGLErrorEventArgs> TaoOpenGLError;

		#endregion

		#region Properties

		/// <summary>
		/// Enables / disables rendering. IMPORTANT: This property is initially set to false to allow for smooth designing.
		/// You MUST set this to true before any rendering will take place.
		/// </summary>
		public bool TaoRenderEnabled {
			get {
				return (renderEnabled);
			}
			set {
				renderEnabled = value;
			}
		}

		/// <summary>
		/// True if both the device and rendering contexts have been created
		/// </summary>
		protected bool ContextsReady {
			get {
				return ((deviceContext != IntPtr.Zero) && (renderContext != IntPtr.Zero));
			}
		}

		#endregion

		public OpenGLWinFormsControl() {
			//Setup the control's styles
			SetStyle(	ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint,true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer,false);//Disable C# double buffering.
			this.DoubleBuffered = false;												//so that it does not interfere with OpenGL.

			//Set default size
			this.Size = new Size(100,100);
		}

		#region Protected Methods

		#region int _DescribePixelFormat(System.IntPtr hdc,int iPixelFormat,uint nBytes,ref Gdi.PIXELFORMATDESCRIPTOR ppfd)
		///<summary>Creates an unmanaged reference to DescribePixelFormat(), which is used to choose an appropriate device pixel format for the current OS and video card.</summary>
		[DllImport("gdi32.dll",EntryPoint="DescribePixelFormat",SetLastError=true),SuppressUnmanagedCodeSecurity]
		public static extern int _DescribePixelFormat(System.IntPtr hdc,int iPixelFormat,uint nBytes,ref Gdi.PIXELFORMATDESCRIPTOR ppfd);
		#endregion int _DescribePixelFormat(System.IntPtr hdc,int iPixelFormat,uint nBytes,ref Gdi.PIXELFORMATDESCRIPTOR ppfd)

		/// <summary>
		/// Creates the device and rendering contexts using the supplied settings
		/// in accumBits, colorBits, depthBits, and stencilBits. Returns the selected
		/// pixel format number.
		/// </summary>
		protected virtual int CreateContexts(IntPtr pDeviceContext,int preferredPixelFormatNum) {
			deviceContext = pDeviceContext;
			if(deviceContext == IntPtr.Zero) {
				throw new Exception("CreateContexts: Unable to create an OpenGL device context");
			}
			int selectedFormat=0;
			Gdi.PIXELFORMATDESCRIPTOR pixelFormat=new Gdi.PIXELFORMATDESCRIPTOR();
			pixelFormat.nSize=(short)Marshal.SizeOf(pixelFormat);
			pixelFormat.nVersion=1;
			//Here we care most about finding a format that will allow the proper creation of the control first and foremost, because even if the format is so bad that output to the display cannot be understood by the user, then the user is able to change the graphical options from within the program. We care second most about finding a format which is quick to load and which will be most time efficient during graphics display. Again, if a wrong choice is made, the user can make a choice of format manually.
			try {//Simply try the preferred pixel format number. If it works, then that is all we care about. Remember, 0 is an invalid format number.
				if(_DescribePixelFormat(deviceContext,preferredPixelFormatNum,(uint)pixelFormat.nSize,ref pixelFormat)==0 ||
					!Gdi.SetPixelFormat(deviceContext,preferredPixelFormatNum,ref pixelFormat)) {
					throw new Exception(string.Format("Unable to set the requested pixel format ({0})",selectedFormat));
				}
				selectedFormat=preferredPixelFormatNum;
			}catch{//Could not set the preferred pixel format for some reason. Possibly initial startup or the graphics card or driver changed since the program was last started.
				//Now try to auto-select the best pixel-format for speed efficientcy and graphical quality.
				try{
					PixelFormatValue pfv=ChoosePixelFormatEx(deviceContext);
					if(!Gdi.SetPixelFormat(deviceContext,pfv.formatNumber,ref pfv.pfd)) {
						throw new Exception("");
					}
					pixelFormat=pfv.pfd;
					selectedFormat=pfv.formatNumber;
				}catch{
					pixelFormat=new Gdi.PIXELFORMATDESCRIPTOR();//Zero out the old pixel format.
					pixelFormat.nSize=(short)Marshal.SizeOf(pixelFormat);
					pixelFormat.nVersion=1;
					//Unable to select a good pixel format. Now we are desperate. Try all formats starting from 1 until we get some format which at least works. That way the user can change the pixel format from the graphical options from inside the program after this point.
					selectedFormat=0;
					do{
						selectedFormat++;
						if(selectedFormat>_DescribePixelFormat(deviceContext,selectedFormat,(uint)pixelFormat.nSize,ref pixelFormat)){
 							throw new Exception("There are no acceptable pixel formats for OpenGL graphics.");
						}
					}while(!Gdi.SetPixelFormat(deviceContext,selectedFormat,ref pixelFormat));
				}
			}
			colorBits=pixelFormat.cColorBits;
			depthBits=pixelFormat.cDepthBits;
			autoSwapBuffers=FormatSupportsDoubleBuffering(pixelFormat);
			usehardware=FormatSupportsAcceleration(pixelFormat);

			//Create rendering context
			renderContext = Wgl.wglCreateContext(deviceContext);

			if(renderContext == IntPtr.Zero) {
				throw new Exception("CreateContexts: Unable to create an OpenGL rendering context");
			}

			//Make this the current context
			MakeCurrentContext();
			return selectedFormat;
		}

		/// <summary>
		/// Deletes both the device and rendering contexts if they've been created.
		/// </summary>
		protected virtual void DisposeContext() {
			//Dispose of rendering context
			if(renderContext != IntPtr.Zero) {
				Wgl.wglMakeCurrent(deviceContext,renderContext);
				Wgl.wglDeleteContext(renderContext);
				renderContext = IntPtr.Zero;
			}

			//Dispose of device context
			if(deviceContext != IntPtr.Zero) {
				User.ReleaseDC(this.Handle,deviceContext);
				deviceContext = IntPtr.Zero;
			}
		}

		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeContext();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Sets this control's OpenGL context as the current context.
		/// </summary>
		public void MakeCurrentContext() {
			if(!Wgl.wglMakeCurrent(deviceContext,renderContext)) {
				throw new Exception("MakeCurrentContext: Unable to active this control's OpenGL rendering context");
			}
		}

		/// <summary>
		/// Draws the design-mode background for the control.
		/// By default, a message is displayed to inform the user that the control is in design mode
		/// and how they can switch to rendering mode.
		/// </summary>
		/// <param name="controlGraphics"></param>
		protected void DrawDesignBackground(Graphics controlGraphics) {
			controlGraphics.Clear(Color.White);

			//Draw heading string
			controlGraphics.DrawString("Tao OpenGL WinForms Control",
					new Font("Arial",14.0f,FontStyle.Bold),Brushes.Black,10.0f,10.0f);

			//Draw information string
			Font infoFont = new Font("Arial",12.0f);

			controlGraphics.DrawString("This control is currently in design mode.",
					infoFont,Brushes.Black,10.0f,35.0f);

			controlGraphics.DrawString("You must set TaoRenderEnabled to true for OpenGL rendering.",
					infoFont,Brushes.Black,10.0f,55.0f);
		}

		#endregion

		#region Public Methods And Objects

		public class PixelFormatValue:IComparable {
			public Gdi.PIXELFORMATDESCRIPTOR pfd;
			public int formatNumber;

			public int CompareTo(object obj) {
				PixelFormatValue other=(PixelFormatValue)obj;
				//Sort by color depth, with greatest color depth first.
				if(other.pfd.cColorBits>pfd.cColorBits) {
					return 1;
				} else if(other.pfd.cColorBits<pfd.cColorBits) {
					return -1;
				}
				//Next, sort by z-buffer depth, with greatest z-depth first.
				if(other.pfd.cDepthBits>pfd.cDepthBits) {
					return 1;
				} else if(other.pfd.cDepthBits<pfd.cDepthBits) {
					return -1;
				}
				//Now by format number.
				if(other.formatNumber<formatNumber){
					return 1;
				}else if(other.formatNumber>formatNumber){
					return -1;
				}
				//At this point the formats are considered equivalent.
				return 0;
			}
		};

		public static PixelFormatValue[] PrioritizePixelFormats(Gdi.PIXELFORMATDESCRIPTOR[] unsortedFormats,bool requireDoubleBuffering,bool requireHardwareAccerleration) {
			ArrayList sortedFormats=new ArrayList();
			for(int i=0;i<unsortedFormats.Length;i++) {
				Gdi.PIXELFORMATDESCRIPTOR pfd=unsortedFormats[i];
				long bpp=pfd.cColorBits;
				long depth=pfd.cDepthBits;
				bool pal=FormatUsesPalette(pfd);
				bool hardware=FormatSupportsAcceleration(pfd);
				bool opengl=FormatSupportsOpenGL(pfd);
				bool window=FormatSupportsWindow(pfd);
				bool bitmap=FormatSupportsBitmap(pfd);
				bool dbuff=FormatSupportsDoubleBuffering(pfd);
				//Recognize formats which do not meet minimum requirements first and foremost.
				if(!opengl||!window||bpp<8||depth<8||pal||requireDoubleBuffering!=dbuff||requireHardwareAccerleration!=hardware) {
					continue;
				}
				PixelFormatValue pfv=new PixelFormatValue();
				pfv.pfd=pfd;
				pfv.formatNumber=i+1;
				sortedFormats.Add(pfv);
			}
			sortedFormats.Sort();
			return (PixelFormatValue[])sortedFormats.ToArray(typeof(PixelFormatValue));
		}

		///<summary>Returns the pixel formats from 1 to Max(maximumCount,maximum available pixel formats).</summary>
		public static Gdi.PIXELFORMATDESCRIPTOR[] GetPixelFormats(System.IntPtr hdc) {
			Gdi.PIXELFORMATDESCRIPTOR pfd=new Gdi.PIXELFORMATDESCRIPTOR();
			pfd.nSize=(short)Marshal.SizeOf(pfd);
			pfd.nVersion=1;
			int numFormats=_DescribePixelFormat(hdc,1,(uint)pfd.nSize,ref pfd);
			Gdi.PIXELFORMATDESCRIPTOR[] pixelFormats=new Gdi.PIXELFORMATDESCRIPTOR[numFormats];
			for(int i=0;i<pixelFormats.Length;i++) {
				pixelFormats[i]=new Gdi.PIXELFORMATDESCRIPTOR();
				pixelFormats[i].nSize=(short)Marshal.SizeOf(pixelFormats[i]);
				pixelFormats[i].nVersion=1;
				_DescribePixelFormat(hdc,i+1,(uint)pixelFormats[i].nSize,ref pixelFormats[i]);
			}
			return pixelFormats;
		}

		///<summary>Does the pixel format support a color palette?</summary>
		public static bool FormatUsesPalette(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.iPixelType==Gdi.PFD_TYPE_COLORINDEX);
		}

		///<summary>Returns true if the given pixel format supports some kind of hardware acceleration, false if the format is a software only graphics.</summary>
		public static bool FormatSupportsAcceleration(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.dwFlags&Gdi.PFD_GENERIC_FORMAT)==0;
		}

		///<summary>Returns true if the given pixel format supports OpenGL rendering, false otherwise.</summary>
		public static bool FormatSupportsOpenGL(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.dwFlags&Gdi.PFD_SUPPORT_OPENGL)!=0;
		}

		///<summary>Returns true if the given pixel format supports windowed rendering, false otherwise.</summary>
		public static bool FormatSupportsWindow(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.dwFlags&Gdi.PFD_DRAW_TO_WINDOW)!=0;
		}

		///<summary>Returns true if the given pixel format supports bitmapped rendering, false otherwise.</summary>
		public static bool FormatSupportsBitmap(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.dwFlags&Gdi.PFD_DRAW_TO_BITMAP)!=0;
		}

		///<summary>Returns true if the given pixel format supports double-buffering, false otherwise.</summary>
		public static bool FormatSupportsDoubleBuffering(Gdi.PIXELFORMATDESCRIPTOR pfd) {
			return (pfd.dwFlags&Gdi.PFD_DOUBLEBUFFER)!=0;
		}

		public IntPtr GetHDC(){
			return User.GetDC(this.Handle);
		}

		///<summary>Tries to automatically select the pixel format which will be most efficient if it works. Use in the case that the program is being loaded for the first time.</summary>
		public static PixelFormatValue ChoosePixelFormatEx(System.IntPtr hdc) {
			Gdi.PIXELFORMATDESCRIPTOR[] saneformats=GetPixelFormats(hdc);
			PixelFormatValue[] formats=null;
			formats=PrioritizePixelFormats(saneformats,false,true);//Non-buffered and accelerated.
			if(formats.Length>0) {
				return formats[0];
			}
			formats=PrioritizePixelFormats(saneformats,true,true);//Buffered and accelerated.
			if(formats.Length>0) {
				return formats[0];
			}
			formats=PrioritizePixelFormats(saneformats,true,false);//Buffered and non-accelerated.
			if(formats.Length>0) {
				return formats[0];
			}
			formats=PrioritizePixelFormats(saneformats,false,false);//Non-buffered and non-accelerated.
			if(formats.Length>0) {
				return formats[0];
			}
			return new PixelFormatValue();//Invalid format, since its formatNumber is zero.
		}

		///<summary>Returns the selected pixel format of the tooth chart.</summary>
		public int TaoInitializeContexts(int preferredPixelFormatNum) {
			//Make sure the handle for this control has been created
			if(this.Handle==IntPtr.Zero) {
				throw new Exception("CreateContexts: The control's window handle has not been created.");
			}
			return TaoInitializeContexts(GetHDC(),preferredPixelFormatNum);
		}

		/// <summary>
		/// Creates device and rendering contexts then fires the user-defined SetupContext event
		/// (if the contexts have not already been created). Call this in your initialization routine.
		/// Returns the selected pixel format for the tooth chart.
		/// </summary>
		public int TaoInitializeContexts(IntPtr pDeviceContext,int preferredPixelFormatNum) {
				int selectedFormat=0;
				if(!ContextsReady) {
						selectedFormat=CreateContexts(pDeviceContext,preferredPixelFormatNum);
						//Fire the user-defined TaoSetupContext event
						if(TaoSetupContext != null) {
								TaoSetupContext(this,null);
						}
				}
				return selectedFormat;
		}

		/// <summary>
		/// Call this method to redraw the control every frame (internally, this calls Invalidate)
		/// </summary>
		public void TaoDraw() {
			Invalidate();
		}

		///<summary>Reads the contents of the front OpenGL drawing surface and returns an unaltered, unscaled copy as an image. The idea is that one would first draw using OpenGL, perform a swap (if double buffering), then read the contents of the resulting image to perform an operation on it, then use the final image for rendering.</summary>
		public Bitmap ReadFrontBuffer(){
			byte[] data=new byte[3*this.Width*this.Height];//3 components in each pixel of the width X height image.
			Gl.glReadPixels(0,0,this.Width,this.Height,Gl.GL_RGB,Gl.GL_UNSIGNED_BYTE,data);
			//The red and blue components are swapped in comparison of the returned OpenGL image and a windows bitmap. The returned image data is also inverted on over the x-axis (in the y or vertical direction). Otherwise, this function would be very fast, because we could just basically return the data into the bitmap in just a few lines of code.
			for(int i=0;i<3*this.Width*this.Height;i+=3){
				//Swap the red and blue components of the current pixel.
				byte temp=data[i];
				data[i]=data[i+2];
				data[i+2]=temp;
			}
			IntPtr dataPtr=GCHandle.Alloc(data,GCHandleType.Pinned).AddrOfPinnedObject();
			Bitmap result=new Bitmap(this.Width,this.Height,3*this.Width,PixelFormat.Format24bppRgb,dataPtr);
			result.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return result;
		}

		#endregion

		#region Control Methods

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			//Do not paint background
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Render(e);
		}

		public void Render(PaintEventArgs e) {
			//Only draw with OpenGL if rendering is enabled (disabled by default for designing)
			if(renderEnabled) {
				//Initialize the device and rendering contexts if the user hasn't already
				//TaoInitializeContexts(e.Graphics.GetHdc());

				//Make this the current context
				if(autoMakeCurrent) {
					//Only switch contexts if this is already not the current context
					if(renderContext!=Wgl.wglGetCurrentContext()) {
						MakeCurrentContext();
					}
				}

				//Fire the user-defined TaoRenderScene event
				if(TaoRenderScene!=null) {
					TaoRenderScene(this,null);
				}

				//Automatically finish the scene
				if(autoFinish) {
					Gl.glFinish();
				}

				//Automatically check for errors
				lastErrorCode=Gl.glGetError();

				if(lastErrorCode!=Gl.GL_NO_ERROR) {
					//Fire the error handling event
					if(TaoOpenGLError!=null) {
						TaoOpenGLError(this,new OpenGLErrorEventArgs(lastErrorCode));
					}
				}

				//Swap the OpenGL buffer to the display
				if(autoSwapBuffers) {
					Gdi.SwapBuffersFast(deviceContext);
				}
			} else {
				//Draw the background for this control when it's in design
				//mode (TaoRenderEnabled = false)
				DrawDesignBackground(e.Graphics);
			}
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);

			if(ContextsReady && renderEnabled) {
				//Fire the user-defined TaoControlSizeChanged event
				if(TaoControlSizeChanged != null) {
					TaoControlSizeChanged(this,new SizeChangedEventArgs(this.Size));
				}
				else {
					//By default, resize the viewport and request a re-draw
					Gl.glViewport(0,0,this.Width,this.Height);
					Invalidate();
				}
			}
		}

		#endregion
	}

	#region EventArgs Classes

	public class SizeChangedEventArgs:EventArgs {
		private Size newSize = new Size();

		/// <summary>
		/// The new size of the control that has been resized.
		/// </summary>
		public Size NewSize {
			get {
				return (newSize);
			}
		}

		public SizeChangedEventArgs(Size newSize) {
			this.newSize = newSize;
		}
	}

	public class OpenGLErrorEventArgs:EventArgs {
		private int errorCode = Gl.GL_NO_ERROR;
		private string description = "";

		/// <summary>
		/// A brief description of the error.
		/// </summary>
		public string Description {
			get {
				return (description);
			}
		}

		/// <summary>
		/// The OpenGL error code.
		/// </summary>
		public int ErrorCode {
			get {
				return (errorCode);
			}
		}

		public OpenGLErrorEventArgs(int errorCode) {
			this.errorCode = errorCode;

			switch(errorCode) {
				case Gl.GL_INVALID_ENUM:
					description = "GL_INVALID_ENUM - An unacceptable value has been specified for an enumerated argument.  The offending function has been ignored.";
					break;

				case Gl.GL_INVALID_VALUE:
					description = "GL_INVALID_VALUE - A numeric argument is out of range.  The offending function has been ignored.";
					break;

				case Gl.GL_INVALID_OPERATION:
					description = "GL_INVALID_OPERATION - The specified operation is not allowed in the current state.  The offending function has been ignored.";
					break;

				case Gl.GL_STACK_OVERFLOW:
					description = "GL_STACK_OVERFLOW - This function would cause a stack overflow.  The offending function has been ignored.";
					break;

				case Gl.GL_STACK_UNDERFLOW:
					description = "GL_STACK_UNDERFLOW - This function would cause a stack underflow.  The offending function has been ignored.";
					break;

				case Gl.GL_OUT_OF_MEMORY:
					description = "GL_OUT_OF_MEMORY - There is not enough memory left to execute the function.  The state of OpenGL has been left undefined.";
					break;

				default:
					description = "Unknown OpenGL Error.";
					break;
			}
		}
	}

	#endregion
}
