using System;   
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;  
using System.Text;
using System.ComponentModel; 

namespace CentralManager{
	internal static class NativeMethods{    
		//https://docs.microsoft.com/en-us/windows/win32/winmsg/window-features?redirectedfrom=MSDN#owned_windows
		//C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x64\inspect.exe
		//https://devblogs.microsoft.com/oldnewthing/?p=24863
		//https://stackoverflow.com/questions/7277366/why-does-enumwindows-return-more-windows-than-i-expected
		//top-level OD Windows have no owner.
		//https://stackoverflow.com/questions/5083954/send-message-in-c-sharp
		//https://stackoverflow.com/questions/52380172/sendmessage-fail-using-registerwindowmessage-api

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		internal delegate bool EnumWindowsProc(IntPtr hWnd,ArrayList lParam);

		[DllImport("user32.dll")]
		internal static extern bool EnumChildWindows(IntPtr hWnd,EnumWindowsProc lpEnumFunc,ArrayList lParam);

		[DllImport("user32.dll")]//CharSet = CharSet.Auto,SetLastError = true)]
		internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc,ArrayList lParam);
		
		[DllImport("user32.dll")]
		internal static extern IntPtr FindWindowEx(IntPtr hwndParent,IntPtr hwndChildAfter,string lpszClass,string lpszWindow);//internally, it loops through all windows.

		[DllImport("User32")] 
		internal static extern IntPtr GetAncestor(IntPtr hWnd,int gaFlags);//use 1 for parent. Top-level window should return null

		[DllImport("user32.dll", SetLastError = false)]
		internal static extern IntPtr GetDesktopWindow();

		///<summary>Returns same as supplied window if supplied window was most recently active, or does not own any pop-up, or is not top-level.</summary>
		[DllImport("User32")] 
		internal static extern IntPtr GetLastActivePopup(IntPtr hWnd); 

		///<summary>Gets Parent OR Owner.  See GetAncestor.</summary>
		[DllImport("User32")] 
		internal static extern IntPtr GetParent(IntPtr hWnd); 

		[DllImport("User32")] 
		internal static extern IntPtr GetWindow(IntPtr hWnd,int uCmd); 

    [DllImport("user32.dll",SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd,ref WINDOWPLACEMENT lpwndpl);  

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern int GetWindowText(IntPtr hwnd,StringBuilder lpString, int maxCount);

		[DllImport("user32.dll")]
		internal static extern int GetWindowThreadProcessId(IntPtr hWnd,out int lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd,int nCmdShow);
	}

	internal static class NativeHelpers{
		///<summary>Gets visible top-level windows.  Still need more filtering if want the ones showing in task bar.</summary>
		internal static List<IntPtr> GetAllVisibleWindows() {
			ArrayList arrayHwnds = new ArrayList();
			NativeMethods.EnumWindowsProc callBackPtr = GetWindowHandle;
			NativeMethods.EnumWindows(callBackPtr,arrayHwnds);
			List<IntPtr> listHwnds=new List<IntPtr>();
			for(int i=0;i<arrayHwnds.Count;i++) {
				if(!NativeMethods.IsWindowVisible((IntPtr)arrayHwnds[i])) {//cuts from 500 down to 50
					continue;
				}
				listHwnds.Add((IntPtr)arrayHwnds[i]);
			}
			return listHwnds;
		}

	/*
		public static List<Process> GetChildrenProcesses(IntPtr hParent) {
			List<Process> retVal = new List<Process>();
			IntPtr prevChild = IntPtr.Zero;
			IntPtr currChild = IntPtr.Zero;
			int count=0;
			while(count < 100) {
				currChild = NativeMethods.FindWindowEx(hParent,prevChild,null,null);
				if(currChild == IntPtr.Zero) {
					break;
				}
				int id=NativeMethods.GetProcessId(currChild);
				Process process=Process.GetProcessById(id);
				//if(process.MainWindowHandle!=IntPtr.Zero) {
					retVal.Add(process);
					//but don't add processes with no window
				//}
				prevChild = currChild;
				count++;
			}
			return retVal;
		}

		internal static List<IntPtr> GetChildWindowsShort(IntPtr hWnd) {
			List<IntPtr> retVal=new List<IntPtr>();
			IntPtr hChildWnd;
			int GW_CHILD=5;
			int GW_HWNDNEXT=2;
			hChildWnd = NativeMethods.GetWindow(hWnd, GW_CHILD);
			while(hChildWnd!=IntPtr.Zero){
				hChildWnd = NativeMethods.GetWindow(hChildWnd, GW_HWNDNEXT);
				//todo: could be invalid
				retVal.Add(hChildWnd);
			}
			return retVal; //not any shorter than GetChildWindows
		}*/

		internal static ArrayList GetChildWindows(IntPtr hWnd) {
			ArrayList windowHandles = new ArrayList();
			NativeMethods.EnumWindowsProc callBackPtr = GetWindowHandle;
			NativeMethods.EnumChildWindows(hWnd,callBackPtr,windowHandles);
			return windowHandles;
		}

		internal static int GetProcessID(IntPtr hWnd){
			int lpdwProcessId;
			NativeMethods.GetWindowThreadProcessId(hWnd,out lpdwProcessId);
			return lpdwProcessId;
		}

		///<summary>C++ CallBack</summary>
		private static bool GetWindowHandle(IntPtr windowHandle,ArrayList listHandles) {
			listHandles.Add(windowHandle);
			return true;
		}

		public static bool IsMinimized(ShowState showState){
			switch(showState){
				default:
					return false;
				case ShowState.SW_SHOWMINIMIZED:
				case ShowState.SW_MINIMIZE:
				case ShowState.SW_SHOWMINNOACTIVE:
					return true;
			}
		}

		public static bool IsMaximized(ShowState showState){
			if(showState==ShowState.SW_MAXIMIZE){
				return true;
			}
			return false;
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT {
		public int length;
		public int flags;
		public ShowState showCmd;
		public System.Drawing.Point ptMinPosition;
		public System.Drawing.Point ptMaxPosition;
		public System.Drawing.Rectangle rcNormalPosition;
	}

	///<summary>In the Win API, these constants are used in showCmd.  Used in ShowWindow and GetWindowPlacement.</summary>
	public enum ShowState:int {
		///<summary>0-Opposite of show.</summary>
		SW_HIDE=0,
		///<summary>1-Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position.</summary>
		SW_SHOWNORMAL=1,
		///<summary>2-Activates the window and displays it as a minimized window.</summary>
		SW_SHOWMINIMIZED=2,
		///<summary>3</summary>
		SW_MAXIMIZE=3,
		///<summary>4</summary>
		SW_SHOWNOACTIVATE=4,
		///<summary>5-Opposite of hide.</summary>
		SW_SHOW=5,
		///<summary>6</summary>
		SW_MINIMIZE=6,
		///<summary>7-Similar to SW_SHOWMINIMIZED, except not activated.</summary>
		SW_SHOWMINNOACTIVE=7,
		///<summary>8-Similar to SW_SHOW, except not activated.</summary>
		SW_SHOWNA=8,
		///<summary>9-Use this to restore a minimized window. (except we might want to "restore" to max)</summary>
		SW_RESTORE=9
		//SW_SHOWDEFAULT=10
		//SW_FORCEMINIMIZE=11
		//<summary>This is not a real one.  Represents a closed window.</summary>
		//Closed=10
	}
}
