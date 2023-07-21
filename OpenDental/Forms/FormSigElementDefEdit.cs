using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental{
	///<summary></summary>
	public partial class FormSigElementDefEdit : FormODBase {
		///<summary>Required to be set before opening this form.</summary>
		public SigElementDef SigElementDefCur;
		public bool IsNew;

		///<summary></summary>
		public FormSigElementDefEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSigElementDefEdit_Load(object sender,EventArgs e) {
			listType.Items.Clear();
			listType.Items.AddEnums<SignalElementType>();
			listType.SetSelectedEnum(SigElementDefCur.SigElementType);
			textSigText.Text=SigElementDefCur.SigText;
			SetSoundButtons();
			textLightRow.Text=SigElementDefCur.LightRow.ToString();
			butColor.BackColor=SigElementDefCur.LightColor;
		}

		private void SetSoundButtons(){
			if(String.IsNullOrEmpty(SigElementDefCur.Sound)){
				butPlay.Enabled=false;
				butExport.Enabled=false;
			}
			else{
				butPlay.Enabled=true;
				butExport.Enabled=true;
			}
		}

		private void butPlay_Click(object sender,EventArgs e) {
			try {
				byte[] rawData=Convert.FromBase64String(SigElementDefCur.Sound);
				SoundHelper.PlaySound(rawData);
			}
			catch {
				MsgBox.Show(this,"Invalid sound");
			}
		}

		private void butDeleteSound_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete sound?")){
				return;
			}
			SigElementDefCur.Sound="";
			SetSoundButtons();
		}

		private void butRecord_Click(object sender,EventArgs e) {
			//The following article was used to figure out how to launch the appropriate executable:
			//http://blogs.microsoft.co.il/blogs/tamir/archive/2007/12/04/seek-and-hide-x64-or-where-my-sound-recoder.aspx
			try{
				//Try to launch the sound recorder program within the Windows operating system
				//for all versions of Windows prior to Windows Vista.
				ODFileUtils.ProcessStart("sndrec32.exe");
				//Process.Start("sndrec32.exe");
				return;
			}
			catch{ }
			//We are on a Windows Vista or later Operating System.
			//The path to the SoundRecorder.exe changes depending on if the Operating System
			//is 32 bit or 64 bit.
			try{
				//First try to launch the SoundRecorder.exe for 32 bit Operating Systems.
				ODFileUtils.ProcessStart("SoundRecorder.exe","/file outputfile.wav");
				//Process.Start("SoundRecorder.exe","/file outputfile.wav");
				return;
			}
			catch{ }
			//This is a 64 bit Operating System. A special environment variable path must be used to indirectly access
			//the SoundRecoder.exe file. The resulting path inside of the soundRecoderVirtualPath variable will only
			//exist inside of this program and does not actually exist if one tries to browse to it.
			string soundRecorderVirtualPath=Environment.ExpandEnvironmentVariables(@"%systemroot%\Sysnative")+"\\SoundRecorder.exe";
			try {
				ODFileUtils.ProcessStart(soundRecorderVirtualPath,"/file outputfile.wav");
				//Process.Start(soundRecorderVirtualPath,"/file outputfile.wav");
			}
			catch {
				//Windows 10 does not have this sound recorder program anymore.
				MsgBox.Show(this,"Cannot find Windows Sound Recorder. Use the 'Record New' button to record a message sound.");
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			using OpenFileDialog openFileDialog1=new OpenFileDialog();
			openFileDialog1.FileName="";
			openFileDialog1.DefaultExt="wav";
			if(openFileDialog1.ShowDialog() !=DialogResult.OK){
				return;
			}
			try{
				SigElementDefCur.Sound=POut.Sound(openFileDialog1.FileName);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			SetSoundButtons();
		}

		private void butExport_Click(object sender,EventArgs e) {
			#region Web Build
			if(ODBuild.IsWeb()) {
				string fileName=SigElementDefCur.SigText+".wav";
				string tempPath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
				try {
					PIn.Sound(SigElementDefCur.Sound,tempPath);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				ThinfinityUtils.ExportForDownload(tempPath);
				return;
			}
			#endregion Web Build
			using SaveFileDialog saveFileDialog1=new SaveFileDialog();
			saveFileDialog1.FileName="";
			saveFileDialog1.DefaultExt="wav";
			if(saveFileDialog1.ShowDialog() !=DialogResult.OK) {
				return;
			}
			try {
				PIn.Sound(SigElementDefCur.Sound,saveFileDialog1.FileName);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog1=new ColorDialog();
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
				SigElementDefs.Delete(SigElementDefCur);
				DialogResult=DialogResult.OK;
			}
		}

		private void butRecordNew_Click(object sender,EventArgs e) {
			using FormRecordAudio formRecordAudio=new FormRecordAudio();
			if(formRecordAudio.ShowDialog()==DialogResult.OK) {
				SigElementDefCur.Sound=formRecordAudio.Sound;
			}
			SetSoundButtons();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textLightRow.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textSigText.Text==""){
				MsgBox.Show(this,"Please enter a text description first.");
				return;
			}
			SigElementDefCur.SigElementType=listType.GetSelected<SignalElementType>();
			SigElementDefCur.SigText=textSigText.Text;
			SigElementDefCur.LightRow=PIn.Byte(textLightRow.Text);
			SigElementDefCur.LightColor=butColor.BackColor;
			if(IsNew){
				SigElementDefs.Insert(SigElementDefCur);
			}
			else{
				SigElementDefs.Update(SigElementDefCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


		

		


		




	}
}





















