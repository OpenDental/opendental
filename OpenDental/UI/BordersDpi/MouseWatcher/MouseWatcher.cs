using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static OpenDental.WindowsHookApiTaskbarInterop;

namespace OpenDental {
/*
The purpose of the MouseWatcher is to handle an edge case in Windows.
If a user has their taskbar hidden, then it would normally show when they hover at the bottom of the screen.
But it's a known Windows issue that a maximized window can block this.
We already built a workaround years ago into FormODBase.
That code notices if the mouse is at the bottom of the screen when any form is maximized and it manually pops open the taskbar.
But if a modal dialog is sitting on top of a maximized window, then that code does not get hit.
This MouseWatcher class is to handle that edge case.
We also decided that it is good enough to replace the similar code in FormODBase.

This is a performance critical class. 
We cache as many things as possible to reduce api calls,
and we hide as many calculations as possible behind inexpensive early return checks.
*/
	public static class MouseWatcher {
		///<summary>Delegates are often stored in a static field because passing a method directly could result in the method being garbage collected. By keeping a static reference,the program ensures the method remains alive for the duration of the hook.</summary>
		private static readonly DelegateLowLevelMouseProc _delegateLowLevelMouseProc=HookCallback;
		private static IntPtr _intPtrHookID=IntPtr.Zero;
		///<summary>Cache for the taskbar window handle to avoid repeated FindWindow() calls.</summary>
		private static IntPtr _intPtrTaskbarWnd=IntPtr.Zero;
		///<summary>We remember which window had focus so that when they move away from task bar we can assign focus back to that window.</summary>
		private static IntPtr _intPtrCachedForegroundWindow=IntPtr.Zero;
		private static int _heightTaskbar;

		//Specific values here are determined by the Windows API.
		//Names follow Windows API conventions.
		///<summary>Low-level mouse hook identifier used to monitor low-level mouse input events.</summary>
		private const int WH_MOUSE_LL=14;
		///<summary>Windows message identifier used to detect and respond to mouse movement events.</summary>
		private const int WM_MOUSEMOVE=0x0200;

		public static void Start() {
			APPBARDATA aPPBARDATA=new APPBARDATA();
			aPPBARDATA.cbSize=(uint)Marshal.SizeOf(aPPBARDATA);
			aPPBARDATA.hWnd=_intPtrTaskbarWnd;
			TaskbarState taskbarState=(TaskbarState)SHAppBarMessage((int)TaskBarCommand.ABM_GETSTATE,ref aPPBARDATA);
			bool isAutoHideEnabled=(taskbarState & TaskbarState.ABS_AUTOHIDE)!=0;
			if(!isAutoHideEnabled) {
				return; //No need to hook if if auto-hide is disabled.
				//If user hides taskbar after starting OD, then this watcher will not be running and user will still have the issue.
			}
			_intPtrTaskbarWnd=FindWindow("Shell_TrayWnd",null); //Get and cache taskbar handle. FindWindow returns IntPtr.Zero on fail.
			if(_intPtrTaskbarWnd==IntPtr.Zero) {
				return; //No need to hook if if we couldn't find the taskbar.
			}
			using Process processCur=Process.GetCurrentProcess();
			using ProcessModule processModuleCur=processCur.MainModule;
			_intPtrHookID=SetWindowsHookEx(WH_MOUSE_LL,_delegateLowLevelMouseProc,GetModuleHandle(processModuleCur.ModuleName),0);
			RECT rectTaskbar=new RECT();
			GetWindowRect(_intPtrTaskbarWnd,ref rectTaskbar);
			_heightTaskbar=rectTaskbar.bottom-rectTaskbar.top;
			Application.ApplicationExit+=Application_ApplicationExit;
		}

		public static void Application_ApplicationExit(object sender,EventArgs e) {
			if(_intPtrHookID!=IntPtr.Zero) {
				UnhookWindowsHookEx(_intPtrHookID);
				_intPtrHookID=IntPtr.Zero;
			}
		}

		private static IntPtr HookCallback(int nCode,IntPtr wParam,IntPtr lParam) {
			if(nCode<0 || wParam!=(IntPtr)WM_MOUSEMOVE) {
				return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam);
			}
			if(_intPtrTaskbarWnd==IntPtr.Zero) { //True=>Taskbar handle is invalid (Maybe explorer.exe crashed?)
				_intPtrTaskbarWnd=FindWindow("Shell_TrayWnd",null); //Reinitialize taskbar handle if it's invalid
				return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam); //Return here in case it's still invalid.
			}
			Point cursorPosition=Cursor.Position;
			Screen screenMouseLocation=Screen.FromPoint(cursorPosition);
			if(cursorPosition.Y<screenMouseLocation.Bounds.Bottom-(_heightTaskbar*3)) { //Mouse isn't near bottom of it's screen
				return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam);
			}
			IntPtr mainWindowHandle=Process.GetCurrentProcess().MainWindowHandle;
			if(mainWindowHandle==IntPtr.Zero || !IsWindow(mainWindowHandle)) { //Safety check before Screen.FromHandle() call
				return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam);
			}
			Screen screenAppLocation=Screen.FromHandle(mainWindowHandle);
			if(screenAppLocation.DeviceName!=screenMouseLocation.DeviceName) { //Mouse isn't on the same screen as our main form
				return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam);
			}
			IntPtr intPtrActiveWindow=GetForegroundWindow();
			if(intPtrActiveWindow!=_intPtrTaskbarWnd && intPtrActiveWindow!=_intPtrCachedForegroundWindow) {
				_intPtrCachedForegroundWindow=intPtrActiveWindow; //Cache active/focused window
			}
			bool isMouseAtBottom=cursorPosition.Y>=screenAppLocation.Bounds.Bottom-1; //Very bottom of screen (-1px)
			bool isMouseAboveTaskbar=cursorPosition.Y<screenAppLocation.Bounds.Bottom-(_heightTaskbar*1.2); //Slightly above taskbar
			bool hasValidCachedWindow=_intPtrCachedForegroundWindow!=IntPtr.Zero && IsWindow(_intPtrCachedForegroundWindow);
			if(isMouseAtBottom) {
				SetWindowToForeground(_intPtrTaskbarWnd);
			}
			else if(isMouseAboveTaskbar && hasValidCachedWindow) {
				SetWindowToForeground(_intPtrCachedForegroundWindow);
			}
			return CallNextHookEx(_intPtrHookID,nCode,wParam,lParam);
		}

		private static void SetWindowToForeground(IntPtr hWnd) {
			uint foregroundThread=GetWindowThreadProcessId(_intPtrCachedForegroundWindow, IntPtr.Zero);
			uint currentThread=GetCurrentThreadId();
			if(foregroundThread!=currentThread) {
				//Attach the current thread to the foreground window thread
				AttachThreadInput(currentThread,foregroundThread,fAttach:true);
				SetForegroundWindow(hWnd);
				//Detach the threads
				AttachThreadInput(currentThread,foregroundThread,fAttach:false);
			}
			else {
				SetForegroundWindow(hWnd);
			}
		}
	}
}
