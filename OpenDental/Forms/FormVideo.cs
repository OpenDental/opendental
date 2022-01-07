using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormVideo:FormODBase {
		private CameraFrameSource _cameraFrameSource;
		private static string _lockImg = "";

		[Category("OD")]
		[Description("Fires when user clicks the Capture button.")]
		public event EventHandler<VideoEventArgs> BitmapCaptured;

		public FormVideo() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		#region Methods - Event Handlers
		private void butCapture_Click(object sender,EventArgs e) {
			BitmapCaptured?.Invoke(this,new VideoEventArgs(){Bitmap=(Bitmap)pictureBoxCamera.Image });
		}

		private void butClose_Click(object sender, EventArgs e){
			//tabStop had to be set to false to prevent false triggers from using space bar.
			Close();
		}

		private void CameraFrameSource_NewFrame(IFrameSource frameSource, byte[] data, Size sz){
			int xoffsett=0;
			int xstep=1;
			//if(butFlipH.Checked){
			//	xoffsett=1;
			//	xstep=-1;
			//}
			int yoffsett=1;
			int ystep=-1;
			//if(butFlipV.Checked){
			//	yoffsett=0;
			//	ystep=1;
			//}
			lock (_lockImg){
				BitmapData bitmapData = null;
				Bitmap bitmap = null;
				try{
					bitmap = new Bitmap(sz.Width, sz.Height);
					bitmapData = bitmap.LockBits(new Rectangle(0, 0, sz.Width, sz.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
					unsafe{
						byte* bmpdata = (byte*)bitmapData.Scan0;
						int pos = 3 * ((xoffsett * (sz.Width - 1)) + (sz.Width * (yoffsett * (sz.Height - 1))));
						int xstepBig = 3 * xstep;
						int offlin = ystep * 3 * sz.Width;
						int imgpos = 0;
						for (int iy = 0; iy < sz.Height; iy++){
							int posi = pos;
							for (int ix = 0; ix < sz.Width; ix++){
								bmpdata[imgpos++] = data[pos];
								bmpdata[imgpos++] = data[pos + 1];
								bmpdata[imgpos++] = data[pos + 2];
								bmpdata[imgpos++] = 255;
								pos += xstepBig;
							}
							pos = posi + offlin;
						}
					}
					bitmap.UnlockBits(bitmapData);
					bitmapData = null;
					pictureBoxCamera.BeginInvoke((Action)delegate () { pictureBoxCamera.Image = bitmap; });
				}
				catch{
					if (bitmapData != null){
						bitmap.UnlockBits(bitmapData);
						bitmapData = null;
					}
				}
			}
		}

		private void comboCameras_SelectionChangeCommitted(object sender,EventArgs e) {
			Play();
		}

		private void FormVideo_FormClosing(object sender, FormClosingEventArgs e){
			TrashOldCamera();
		}

		private void FormVideo_KeyDown(object sender, KeyEventArgs e){
			if(e.KeyCode==Keys.Space){
				BitmapCaptured?.Invoke(this,new VideoEventArgs(){Bitmap=(Bitmap)pictureBoxCamera.Image });
			}
		}

		private void FormVideo_Load(object sender, EventArgs e){
			TrashOldCamera();
			comboCameras.Items.Clear();
			CameraMethods cameraMethods=new CameraMethods();
			List<Camera> listCameras=cameraMethods.GetListCameras();
			for(int i=0;i<listCameras.Count;i++){
				comboCameras.Items.Add(listCameras[i]);
			}
			if(comboCameras.Items.Count == 0){
				return;
			}
			comboCameras.SelectedIndex = 0;
			Play();
		}
		#endregion Methods - Event Handlers

		#region Methods
		public void Parent_KeyDown(Keys keys){
			//because FormVideo_KeyDown would not be reliable enough on its own. This form might not have focus.
			if(keys==Keys.Space){
				BitmapCaptured?.Invoke(this,new VideoEventArgs(){Bitmap=(Bitmap)pictureBoxCamera.Image });
			}
		}

		///<summary>This stops any existing camera, first, then plays the new one.</summary>
		private void Play(){
			//if(_cameraFrameSource != null && _cameraFrameSource.Camera == comboCameras.SelectedItem){
			//	return;
			//}
			try{
				pictureBoxCamera.Image = null;
				TrashOldCamera();
				StartVideo();
			}
			catch (Exception ex){
				if (ex.InnerException != null){
					MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
				}
				else{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void SetFrameSource(CameraFrameSource cameraFrameSource){
			if (_cameraFrameSource==cameraFrameSource){
				return;
			}
			_cameraFrameSource=cameraFrameSource;
		}

		private void StartVideo(){
			try{
				Camera camera = (Camera)comboCameras.SelectedItem;
				//camera.Width = Convert.ToInt32(textWidth.Text);
				//camera.Height = Convert.ToInt32(textHeight.Text);
				SetFrameSource(new CameraFrameSource(camera));
				_cameraFrameSource.NewFrame += CameraFrameSource_NewFrame;
				_cameraFrameSource.StartFrameCapture();
			}
			catch (Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void TrashOldCamera(){
			if(_cameraFrameSource == null){
				return;
			}
			_cameraFrameSource.ReleaseEvents();
			_cameraFrameSource.Camera.Dispose();
			SetFrameSource(null);
		}



		#endregion Methods

	
	}
	public class VideoEventArgs{
		public Bitmap Bitmap;
	}
}