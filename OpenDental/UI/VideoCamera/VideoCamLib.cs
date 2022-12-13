using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OpenDental.UI{
	//This is just a DirectShow wrapper.  It would be better if we wrote our own some day so that we knew exactly what it was doing.
	//That would also give us access to the capture button pin.
	public static class VideoCamLib{
		public delegate void CaptureCallbackProc(int dwSize,[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 0)] byte[] abData);

		[DllImport("VideoCamLib.dll")]
		public static extern int Cleanup();

		[DllImport("VideoCamLib.dll")]
		public static extern int GetCameraDetails(int index,[Out, MarshalAs(UnmanagedType.Interface)] out object nativeInterface,out IntPtr name);

		[DllImport("VideoCamLib.dll")]
		public static extern int Initialize();

		[DllImport("VideoCamLib.dll")]
		public static extern int RefreshCameraList(ref int count);

		[DllImport("VideoCamLib.dll")]
		public static extern int StartCamera(
				[In, MarshalAs(UnmanagedType.Interface)] object nativeInterface,
				CaptureCallbackProc lpCaptureFunc,
				int sminwidth,
				int minheight,
				ref int width,
				ref int height,
				ref int stride
				);

		[DllImport("VideoCamLib.dll")]
		public static extern int StartVideoFile(
				[In, MarshalAs(UnmanagedType.LPWStr)] StringBuilder wFileName,
				CaptureCallbackProc lpCaptureFunc,
				ref int width,
				ref int height,
				ref int stride
				);

		[DllImport("VideoCamLib.dll")]
		public static extern int StopCamera();

		
	}
}
