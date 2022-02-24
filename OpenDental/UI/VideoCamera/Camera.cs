using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OpenDental.UI {
	public class Camera:IDisposable {
		protected readonly CameraMethods _cameraMethods;
		protected readonly object _bitmapLock = new object();
		public string Name;
		public object NativeInterface; 
		public int Width=480;
		public int Height=640;

		public Camera(CameraMethods cameraMethods,object nativeInterface,string name){
			_cameraMethods = cameraMethods;
			Name = name;
			NativeInterface = nativeInterface;
			_cameraMethods.OnImageCapture += CaptureCallbackProc;
		}

		void IDisposable.Dispose() {
			if(NativeInterface != null) {
				Marshal.ReleaseComObject(NativeInterface);
				NativeInterface = null;
			}
		}

		public void Dispose() {
			StopCapture();
		}

		/// <summary>Event fired when an image from the camera is captured.</summary>
		public event EventHandler<CameraEventArgs> ImageCaptured;

		///<summary>Returns the camera name as the ToString implementation</summary>
		public override string ToString() {
			return Name;
		}

		internal virtual void StartCapture() {
			_cameraMethods.StartCamera(this);
		}

		internal void StopCapture() {
			_cameraMethods.StopCamera();
		}

		///<summary>Here is where the images come in as they are collected, as fast as they can and on a background thread.</summary>
		private void CaptureCallbackProc(int dataSize,byte[] data) {
			GCHandle handle = GCHandle.Alloc(data,GCHandleType.Pinned);
			int scan0 = (int)handle.AddrOfPinnedObject();
			byte[] datacopy = new byte[dataSize];
			Marshal.Copy((IntPtr)scan0,datacopy,0,dataSize);
			handle.Free();
			// pass it to the camera for its events and processing
			ImageCaptured?.Invoke(this,new CameraEventArgs(datacopy,new Size(Width,Height)));
		}

		public void FireImageCaptured(byte[] byteArray,Size sizeImage) {
			ImageCaptured?.Invoke(this,new CameraEventArgs(byteArray,sizeImage));
		}

	}

	///<summary></summary>
	public class CameraEventArgs:EventArgs {
		public Size SizeImage;
		public byte[] ByteArray;

		public CameraEventArgs(byte[] byteArray,Size size) {
			ByteArray = byteArray;
			SizeImage = size;
		}

	}
}
