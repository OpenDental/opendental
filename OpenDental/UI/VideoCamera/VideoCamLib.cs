using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OpenDental.UI{
	//This is a MediaFoundation wrapper. 
	//It was based on the Tanta Project, which was written to demonstrate how to use MF
	//Documentation: http://www.ofitselfso.com/Tanta/Windows_Media_Foundation_Getting_Started_CSharp.pdf
	//Code: https://github.com/OfItselfSo/Tanta
	//I copied part of the Tanta code called CaptureToScreenAndFile.
	//Notes for that are on my hard drive at E:\Documents\Open Dental\PROGRAMMING\2020-10-31-Video Capture
	//I then spent 2 days refactoring and simplifying to create what I call VideoCamLib.dll.
	//The source code for VideoCamLib.dll and the testing environment was copied to in My\Jordan\VideoCameraTesting.
	//The C# code from that test environment was then copied to OD proper and further simplified.
	//There is another earlier attempt at MF found in the abandoned project at Versioned\OpenDentalImaging. I would ignore that one.
	//This code far from perfect, but the foundation is in place.
	//The plan is to overhaul the C++ and C# code with good organization and comments, and to actually understand what it's doing instead of just blindly copying.
	//It should be possible since I already understand COM, which is the source of much of the complexity.
	//I think we need to add support for higher resolution cameras and support the capture button pin on the camera.
	//To test, I will need to get a higher resolution camera.
	//These issues are both addressed by code from James Petko at Digital Doc.
	//https://github.com/JimPetko/HighResVideoCaptureSample
	//His code uses SharpDX for the game controller for capture button. This could be done directly with C++ instead, once I inspect what he did.
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
