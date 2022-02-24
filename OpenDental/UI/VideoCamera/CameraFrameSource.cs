using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace OpenDental.UI {
	///<summary>Contains a camera.  The event from the camera bubble up to here, which then causes the NewFrame event to fire.</summary>
	public class CameraFrameSource:IFrameSource {
		private Camera _camera;

		///<summary>Constructor</summary>
		public CameraFrameSource(Camera camera) {
			if(camera==null){
				throw new ArgumentNullException("camera");
			}
			this.Camera = camera;
		}

		public event Action<IFrameSource,byte[],Size> NewFrame;

		public bool IsCapturing {
			get; private set;
		}

		public Camera Camera{
			get {
				return _camera;
			}
			internal set {
				if(_camera==value){
					return;
				}
				bool restart=IsCapturing;
				if(IsCapturing) {
					StopFrameCapture();
				}
				_camera=value;
				if(restart) {
					StartFrameCapture();
				}
			}
		}

		public void StartFrameCapture() {
			if(!IsCapturing && this.Camera != null) {
				this.Camera.ImageCaptured += Camera_ImageCaptured;
				this.Camera.StartCapture();
				IsCapturing = true;
			}
		}

		public void StopFrameCapture() {
			if(IsCapturing) {
				this.Camera.StopCapture();
				this.Camera.ImageCaptured -= Camera_ImageCaptured;
				this.IsCapturing = false;
			}
		}

		public void ReleaseEvents() {
			StopFrameCapture();
			NewFrame = null;
		}

		private void Camera_ImageCaptured(object sender,CameraEventArgs e) {
			if(!IsCapturing){
				return;
			}
			NewFrame?.Invoke(this,e.ByteArray,e.SizeImage);
		}



	}
}
