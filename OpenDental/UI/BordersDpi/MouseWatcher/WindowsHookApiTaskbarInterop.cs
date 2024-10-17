using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenDental {
	internal static class WindowsHookApiTaskbarInterop {
		#region user32.dll imports

		public delegate IntPtr DelegateLowLevelMouseProc(int nCode,IntPtr wParam,IntPtr lParam);

		[DllImport("user32.dll",CharSet=CharSet.Auto,SetLastError=true)]
		public static extern IntPtr SetWindowsHookEx(int idHook,DelegateLowLevelMouseProc lpfn,IntPtr hMod,uint dwThreadId);

		[DllImport("user32.dll",CharSet=CharSet.Auto,SetLastError=true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll",CharSet=CharSet.Auto,SetLastError=true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk,int nCode,IntPtr wParam,IntPtr lParam);

		[DllImport("user32.dll",CharSet=CharSet.Auto,SetLastError=true)]
		public static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd,ref RECT lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError=true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsWindow(IntPtr hWnd);

		#endregion user32.dll imports

		#region shell32.dll imports

		[DllImport("shell32.dll",SetLastError=true)]
		public static extern int SHAppBarMessage(int dwMessage,ref APPBARDATA pData);

		#endregion shell32.dll imports

		#region kernel32.dll imports

		[DllImport("kernel32.dll",CharSet=CharSet.Auto,SetLastError=true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		#endregion kernel32.dll imports

		#region winAPI struct definitions

		//The [StructLayout(LayoutKind.Sequential)] attribute is used to control how the fields of a struct are laid out in memory. This is important when working with interop (communicating with unmanaged code like Windows API),where you need the layout of the structure in managed memory to match the expected layout in unmanaged memory. It is also preferable to keep the naming schema from the API, which is why these don't follow conventional OD naming patterns.

		[StructLayout(LayoutKind.Sequential)]
		public struct APPBARDATA {
			public uint cbSize;
			public IntPtr hWnd;
			public uint uCallbackMessage;
			public uint uEdge;
			public RECT rc;
			public int lParam;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		#endregion winAPI struct definitions
	}

	public enum TaskBarCommand {
		ABM_NEW=0x00,
		ABM_REMOVE=0x01,
		ABM_QUERYPOS=0x02,
		ABM_SETPOS=0x03,
		ABM_GETSTATE=0x04,
		ABM_GETTASKBARPOS=0x05,
		ABM_ACTIVATE=0x06,
		ABM_GETAUTOHIDEBAR=0x07,
		ABM_SETAUTOHIDEBAR=0x08,
		ABM_WINDOWPOSCHANGED=0x09,
		ABM_SETSTATE=0x0a
	}

	[Flags]
	public enum TaskbarState {
		ABS_AUTOHIDE=0x01,
		ABS_ALWAYSONTOP=0x02,
	}
}
