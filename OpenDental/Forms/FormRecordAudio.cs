using CodeBase;
using OpenDentBusiness;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRecordAudio:FormODBase {
		public string Sound;
		private DateTime _recordStart;
		private string _tempPath;

		public FormRecordAudio() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		/// <summary>mciSendString uses command strings to control MCI devices.  This link gives an example of using mciSendString():
		/// http://stackoverflow.com/questions/3694274/how-do-i-record-audio-with-c-wpf. </summary>
		[DllImport("winmm.dll",EntryPoint="mciSendStringA",CharSet=CharSet.Ansi,SetLastError=true,ExactSpelling=true)]
		private static extern int mciSendString(string lpstrCommand,string lpstrReturnString,int uReturnLength,int hwndCallback);

		private void FormRecordAudio_Load(object sender,EventArgs e) {
			butPlay.Enabled=false;
			butSave.Enabled=false;
		}

		private void timerRecord_Tick(object sender,EventArgs e) {
			labelTimer.Text=(DateTime.Now-_recordStart).ToStringHmmss();
		}

		private void butStart_Click(object sender,EventArgs e) {
			if(butStart.Text==Lan.g(this,"Record")) {
				timerRecord.Start();
				_recordStart=DateTime.Now;
				mciSendString("open new Type waveaudio Alias recsound","",0,0);
				mciSendString("record recsound","",0,0);
				butStart.Text=Lan.g(this,"Stop");
				butStart.Image=imageListMain.Images[1];
				butPlay.Enabled=false;
				butSave.Enabled=false;
			}
			else {//butStart.Text=="Stop"
				timerRecord.Stop();
				_tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),"recsound"+".wav");
				mciSendString("save recsound "+_tempPath,"",0,0);
				mciSendString("close recsound ","",0,0);
				butStart.Text=Lan.g(this,"Record");
				butStart.Image=imageListMain.Images[0];
				butPlay.Enabled=true;
				butSave.Enabled=true;
			}
		}

		private void butPlay_Click(object sender,EventArgs e) {
			byte[] rawData=File.ReadAllBytes(_tempPath);
			SoundHelper.PlaySound(rawData);			
		}

		private void butSave_Click(object sender,EventArgs e) {
			using SaveFileDialog saveDlg=new SaveFileDialog();
			string filename="message.wav";
			saveDlg.FileName=filename;
			if(saveDlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			File.Copy(_tempPath,saveDlg.FileName,true);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!File.Exists(_tempPath)) {
				MsgBox.Show(this,"Please record a sound first.");
				return;
			}
			try {
				Sound=POut.Sound(_tempPath);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message+"\r\n"+ex.StackTrace);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormRecordAudio_FormClosing(object sender,FormClosingEventArgs e) {
			//Delete the temp file since we don't need it anymore.
			try {
				File.Delete(_tempPath);
			}
			catch {
				//Do nothing.  This file will likely get cleaned up later.
			}
		}

	}
}