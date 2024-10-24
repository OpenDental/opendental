using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
/*
Some general notes about how dpi works:
This is a starting point, although they gloss over many details and don't really give any good strategies:
https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms?view=netframeworkdesktop-4.8#configuring-your-windows-forms-app-for-high-dpi-support
OD is dpi aware by default, specifically DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2.
Users can turn this off in one of two ways:
1. Right click exe, Properties, Compatibility, Change high DPI settings, Override, System.
2. Add NoDpi.txt file.
When not dpi aware, the following will all use 96 dpi values:
Form.DesktopBounds, Form.SetDesktopBounds, and Screen.WorkingArea.
This means they will appear proportionally smaller than the real dimensions, which generally works fine.
But these must all be tested in both dpi-aware and non-aware because of lots of little issues.
150% on a 4K monitor is a good testing environment for both.

WPF works in DIPs.
If directly setting a Window.Left or Top, you must convert to windows scale.
Full examples are in WindowComboPicker, ToolTip, and WindowImageFloatWindows.
But I recommend this:
		double scale=VisualTreeHelper.GetDpi(this).DpiScaleX;

Do not use SetDesktopBounds to move a Form.
It will erroneously use the upper left of the working area as 0,0.
If a taskbar is docked left, this will shift your Form to the right by that amount on both screens.
At least on Windows 10.
Intead, use Form.Bounds=...
*/
	public class Dpi{
		public enum DPI_AWARENESS_CONTEXT{
			DPI_AWARENESS_CONTEXT_DEFAULT = 0,
			DPI_AWARENESS_CONTEXT_UNAWARE = -1, 
			DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2, 
			DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
			DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
			DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5
		}

		[DllImport("User32.dll")]
		private static extern DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT dpiContext);

		[DllImport("User32.dll")]
		private static extern bool IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT dpiContext);

		///<summary>This is used to set any Form to be unaware of Dpi.  This will cause Windows to handle dpi the old way, by scaling a bitmap of the form.  This will cause everything to get a little blurry, but it will at least all work correctly.  This buys us time to fix any custom drawing.  Use this just before creating a form (FormExample formExample=new FormExample()).  Then, right after that line, call Dpi.SetAware.</summary>
		public static void SetUnaware(){
			try{
				SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE);
				//We could instead set to DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED, but testing showed that it essentially looked identical.
			}
			catch{
				//requires Win10
			}
		}

		///<summary>Used after SetUnaware.  After this comes Show() or ShowDialog().</summary>
		public static void SetAware(){
			try{
				SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_DEFAULT);
			}
			catch{
				//requires Win10
			}
		}

		public static bool SupportsHighDpi(){
			try{
				if(IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2)){
					return true;
				}
			}
			catch{
				return false;//for old computers that are not Win10 or are servers.
			}
			return false;//RDP session might return this because the workstation is Win10, so it won't fail, but it will recognize that the dpi awareness would not be valid.
		}

		#region FixInitial
		//Initial loading issue
		//https://developercommunity.visualstudio.com/content/problem/262330/high-dpi-support-in-windows-forms.html  (they only partially solved it)
		//Windows is good about resizing and scaling a window when it moves from one monitor to the next, but window launch is different.  
		//Windows frequently does not automatically set the dpi based on the monitor a window was launched on.  
		//We have to check that, and fix it on our own.  
		//This applies to all dialogs and new windows launched on a secondary monitor.
		//Windows scaling doesn't pay attention to docking or anchors.  Since it scales by a ratio, the docking just nicely works out.

		[DllImport("User32.dll")]
		private static extern IntPtr MonitorFromPoint(Point point,uint dwFlags);

//todo: This is a single DPI API. Consider switching to per-montitor API:
//https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-desktop-application-development-on-windows
		[DllImport("Shcore.dll")]
		private static extern IntPtr GetDpiForMonitor(IntPtr hmonitor,DpiType dpiType,out uint dpiX,out uint dpiY);

		[DllImport("User32.dll")]
		private static extern int GetDpiForWindow(IntPtr hwnd);

		[DllImport("user32")]
		private extern static int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private extern static bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, uint wParam, IntPtr lParam);

		public enum DpiType{
			Effective = 0,
			Angular = 1,
			Raw = 2,
		}

		/*
		private struct RECT{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		private const int WM_DPICHANGED = 0x02E0;

		///<summary>Fixes DPI on a window if it's wrong.  If dpi is wrong, it sends a WndProc message. This is then handled as usual by OnResize event handler or by us manually on the other end.  Returns a new ratio representing the scale.</summary>
		public static void FixInitial(FormODBase form){
			Fix(form,false);
		}

		///<summary>Fixes DPI on a form if it's wrong. Preserves the XY Location of the form instead of scaling about the center of the form.  Rarely used. Not quite sure where it will be used yet, or how.</summary>
		public static void FixInitialPreserveLocation(FormODBase form){
			Fix(form,true);
		}

		private static void Fix(FormODBase form,bool preserveLocation){
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromControl(form);//automatically returns screen that contains largest portion of this form
			uint dpiMonitorX=(uint)GetScreenDpi(screen);
			if(dpiMonitorX==form.DeviceDpi && Db.HasDatabaseConnection() && ComputerPrefs.LocalComputer.Zoom==0){
				//The main form will not yet have a db connection, so it will skip the code below. Since it's maximized, that's fine.
				return;
			}
			uint wParam = (dpiMonitorX << 16) | (dpiMonitorX & 0xffff);//32 bits, HIWORD and LOWORD same
			//The above dpi is sent in the WndProc.  
			//From here down, we are calculating new form rectangle
			//We are not including Zoom in that calc because our WndProc handler takes care of zoom separately
			//double ratio=(double)dpiMonitorX/form.DeviceDpi;
			float scale=(float)dpiMonitorX/96;//+ComputerPrefs.LocalComputer.Zoom/100f;
			int height=(int)(form.Height*scale);
			int width=(int)(form.Width*scale);
			RECT rectSuggested = new RECT();
			if(preserveLocation){
				rectSuggested.Left=form.Location.X;
				rectSuggested.Top=form.Location.Y;
			}
			else{
				rectSuggested.Left=form.Location.X-(width-form.Width)/2;
				rectSuggested.Top=form.Location.Y-(height-form.Height)/2;
				//don't try to fix these for spilling out.  Their values can be negative if they are to the left or above primary monitor.
			}
			rectSuggested.Right=rectSuggested.Left+width;
			rectSuggested.Bottom=rectSuggested.Top+height;
			IntPtr pRect=Marshal.AllocHGlobal(Marshal.SizeOf(rectSuggested));
			Marshal.StructureToPtr(rectSuggested, pRect, false);
			SendMessage(form.Handle, WM_DPICHANGED, wParam, pRect);//this takes effect immediately
			Marshal.FreeHGlobal(pRect);
		}*/

		public static int GetScreenDpi(System.Windows.Forms.Screen screen){
			Point point= new Point(screen.Bounds.Left+1,screen.Bounds.Top+1);
			IntPtr hMon=MonitorFromPoint(point,2);//2=MONITOR_DEFAULTTONEAREST
			uint dpiMonitorX, dpiMonitorY;
			try{
				GetDpiForMonitor(hMon, DpiType.Effective, out dpiMonitorX, out dpiMonitorY);//requires Windows 8.1
			}
			catch{//DllNotFoundException){//there can be other exception types as well
				return 96;
			}
			return (int)dpiMonitorX;//same as y
		}

		///<summary>2023-12-23 I'm trying this as replacement for GetScreenDpi. It uses GetDpiForWindow instead of GetDpiForMonitor, as recommended by MS. No references because I never finished debugging it. But it's close to functional</summary>
		public static int GetScreenDpi2(System.Windows.Forms.Screen screen){
			Process process=Process.GetCurrentProcess();
			EnumWindows(WindowHandleFromProcessId, new IntPtr(process.Id));
			//Point point= new Point(screen.Bounds.Left+1,screen.Bounds.Top+1);
			//IntPtr hMon=MonitorFromPoint(point,2);//2=MONITOR_DEFAULTTONEAREST
			int dpiWindow;
			try{
				dpiWindow=GetDpiForWindow(editorWindow);
				//GetDpiForMonitor(hMon, DpiType.Effective, out dpiMonitorX, out dpiMonitorY);//requires Windows 8.1
			}
			catch{//DllNotFoundException){//there can be other exception types as well
				return 96;
			}
			return (int)dpiWindow;
		}

		private static IntPtr editorWindow;

		private static bool WindowHandleFromProcessId(IntPtr hWnd,IntPtr lParam) {
			GetWindowThreadProcessId(hWnd,out int processId);
			if(lParam.ToInt32() == processId) {
				editorWindow = hWnd;
				return false;
			}
			return true;
		}
		#endregion FixInitial


	}



}
