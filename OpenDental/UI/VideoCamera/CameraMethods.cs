using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental.UI {
	///<summary>Each camera has one of these.  This calls the native methods and passes results up to the Camera.  This also has an internal list of cameras.  Huh?  And its own Camera Huh????</summary>
	public class CameraMethods {
		// Cameras
		static private int _videocamLibCount = 0;
		static private object _videocamLock = new object();
		public List<Camera> ListCameras = null;
		//needed to avoid garbage collection problem
		private VideoCamLib.CaptureCallbackProc _nativeCallback;
		private Camera _cameraCur = null;
		private int _camWidth = 0;
		private int _camHeight = 0;
		private int _camstride = 0;

		///<summary>Constructor.  Also refreshes camera list</summary>
		public CameraMethods() {
			lock(_videocamLock) {
				if(_videocamLibCount == 0) {
					if(VideoCamLib.Initialize() != 0) {
						throw new ApplicationException("Unable to initialize the camera API");
					}
				}
				_videocamLibCount++;
				RefreshCameraList();
			}
		}

		private void RefreshCameraList() {
			int count=0;
			Cleanup();
			ListCameras=new List<Camera>();
			int hResult=VideoCamLib.RefreshCameraList(ref count);
			if(hResult!= 0) {
				return;
			}
			for(int i=0;i<count;i++) {
				IntPtr name = IntPtr.Zero;
				object nativeInterface = null;
				if(VideoCamLib.GetCameraDetails(i,out nativeInterface,out name) == 0) {
					Camera camera = new Camera(this,nativeInterface,Marshal.PtrToStringBSTR(name));
					ListCameras.Add(camera);
				}
				Marshal.FreeBSTR(name);
			}
		}

		public List<Camera> GetListCameras(){
			return ListCameras;
		}

		public event VideoCamLib.CaptureCallbackProc OnImageCapture;

		public void Cleanup() {
			if(_cameraCur != null) {
				VideoCamLib.StopCamera();
				_cameraCur = null;
			}
			if(ListCameras== null) {
				return;
			}
			for(int i=0;i<ListCameras.Count;i++) {
				ListCameras[i].Dispose();
			}
			ListCameras.Clear();
			return;
		}

		public void Dispose() {
			Cleanup();
			lock(_videocamLock) {
				_videocamLibCount--;
				if(_videocamLibCount <= 0) {
					if(VideoCamLib.Cleanup() != 0) {
						throw new ApplicationException("Unable to cleanup the webcam API");  // TODO fix exception
					}
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected virtual void Dispose(bool A_0) {
			Dispose();
		}

		///<summary></summary>
		public void StartCamera(Camera camera){
			if(_cameraCur!=null){
				VideoCamLib.StopCamera();
				_cameraCur=null;
			}
			if(ListCameras==null){
				return;
			}
			for(int i = 0;i < 2;i++) {//looks like it tries twice
				if(camera.NativeInterface==null) {
					throw new InvalidComObjectException("The camera has been disposed");
				}
				_nativeCallback = new VideoCamLib.CaptureCallbackProc(CaptureCallbackProc);
				int rc = VideoCamLib.StartCamera(camera.NativeInterface,
						_nativeCallback,
						camera.Width,//minWidth
						camera.Height,//minHeight
						ref _camWidth,
						ref _camHeight,
						ref _camstride);
				if(rc == 0) {//StartCamera failed internally on the first step
					_cameraCur = camera;
					_cameraCur.Width = _camWidth;
					_cameraCur.Height = _camHeight;
					return;
				}
				else {
					if(i > 0) {
						//This at least tells us which of the 18 steps it failed on
						throw new ApplicationException(string.Format("Unable to start camera. rc={0}",rc)); // TODO make a better exception
					}
				}
			}
		}

		public void StopCamera() {
			if(_cameraCur != null) {
				VideoCamLib.StopCamera();
				_cameraCur = null;
			}
		}

		public void CaptureCallbackProc(int dataSize,byte[] data) {
			if(_cameraCur != null) {
				// Do the magic to create a bitmap
				GCHandle handle = GCHandle.Alloc(data,GCHandleType.Pinned);
				int scan0 = (int)handle.AddrOfPinnedObject();
				byte[] byteArray = new byte[dataSize];
				Marshal.Copy((IntPtr)scan0,byteArray,0,dataSize);
				handle.Free();
				// pass it to the camera for its events and processing
				_cameraCur.FireImageCaptured(byteArray,new Size(_camWidth,_camHeight));
			}
		}
	}

	public class CameraInfo {
		public int index;
		public string name;
	}
}
