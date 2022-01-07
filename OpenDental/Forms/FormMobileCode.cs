using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using ZXing;

namespace OpenDental {
	public partial class FormMobileCode:FormODBase {
		///<summary>An external function that is called when we have a valid unlock code that is ready to use.
		///Should only be called in TryInvoke(...) so that we can handle and display intentional thrown exception errors.</summary>
		private Func<string,MobileDataByte> _unlockGeneratedFunc;
		///<summary>Can be null.</summary>
		private MobileDataByte _mobileDataByte;
		///<summary>Timer used to track when a unlock code has been removed from the DB.</summary>
		private Timer _unlockCodeTimer=new Timer();
		///<summary>True when a QR is beind displayed in the form, otherwise false.
		///When false user can edit unlock code textboxs.</summary>
		private bool _isFormLocked;

		///<summary>Set unlockGeneratedFunc to a fuction that inserts and returns the MobileDataBye row once we have a valid unlock code.</summary>
		public FormMobileCode(Func<string,MobileDataByte> unlockGeneratedFunc) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_unlockGeneratedFunc=unlockGeneratedFunc;
		}

		private void FormMobileCode_Shown(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.MobileAutoUnlockCode)) {
				VerifyAndSetUnlockCode(MobileDataBytes.GenerateUnlockCode());
			}
			_unlockCodeTimer.Interval=1000;
			_unlockCodeTimer.Tick+=UnlockCodeTimer_Tick;
		}

		///<summary>Verifies the given unlockCode and Invokes _unlockGeneratedFuc before setting the form locked.</summary>
		private void VerifyAndSetUnlockCode(string unlockCode) {
			if(unlockCode.IsNullOrEmpty()) {
				MsgBox.Show(this,"Unlock code is empty.");
				return;
			}	
			if(unlockCode.Length!=6) {//Verify that code is valid.
				MsgBox.Show(this,"Unlock code must be 6 characters.");
				return;
			}
			if(MobileDataBytes.IsValidUnlockCode(unlockCode)) {//Code alreay in use.
				MsgBox.Show(this,"Please choose another unlock code.");
				return;
			}
			if(!TryInvoke(unlockCode,out _mobileDataByte)) {
				return;
			}
			if(!MobileDataBytes.IsValidUnlockCode(unlockCode)) {//Verify that code made it into db.
				MsgBox.Show(this,"Unlock code invalid.");
				return;
			}
			SetFormLocked(unlockCode);
		}

		///<summary>This is the only method that should call _unlockGeneratedFunc.Invoke(...).</summary>
		private bool TryInvoke(string unlockCode,out MobileDataByte dataByte){
			dataByte=null;
			try {
				dataByte=_unlockGeneratedFunc.Invoke(unlockCode);
			}
			catch(Exception ex){
				picBoxMain.TextNullImage=ex.Message;
				picBoxMain.Image=null;
			}
			return (dataByte!=null);
		}

		///<summary>Once we have a valid Unlock Code in the DB we check to see if the Unlock Code has been used yet.
		///Once it has we close the form.</summary>
		private void UnlockCodeTimer_Tick(object sender,EventArgs e) {
			string unlockCode=GetUiUnlockCode();
			if(_isFormLocked && !MobileDataBytes.IsValidUnlockCode(unlockCode)) {
				_unlockCodeTimer.Stop();
				DialogResult=DialogResult.OK;
			}
		}
		
		///<summary>Returns the 6 textboxes Text as a single string.</summary>
		private string GetUiUnlockCode(){
			string unlockCode="";
			unlockCode+=textBox1.Text;
			unlockCode+=textBox2.Text;
			unlockCode+=textBox3.Text;
			unlockCode+=textBox4.Text;
			unlockCode+=textBox5.Text;
			unlockCode+=textBox6.Text;
			return unlockCode;
		}
		
		///<summary>Locks the form so that a mobile device can scan the QR code for the given unlockCode.
		///Also sets various UI fields.</summary>
		private void SetFormLocked(string unlockCode){
			_isFormLocked=true;
			BarcodeWriter qrWritier=new BarcodeWriter {
				Options=new ZXing.Common.EncodingOptions() {
					PureBarcode=true,
					Width=300,
					Height=300
				},
				Format=ZXing.BarcodeFormat.QR_CODE
			};
			using(Bitmap qrBitmap=new Bitmap(qrWritier.Write(unlockCode))) {
				picBoxMain.Image=(qrBitmap.Clone() as Bitmap);//Picture box will handle disposing
			}
			#region Fill and disable textbox using rawCode.
			char[] rawCodeArray=unlockCode.ToCharArray();
			int i=1;
			foreach(char codeChar in rawCodeArray) {
				TextBox textBox;
				switch(i) {
					case 1:
						textBox=textBox1;
						break;
					case 2:
						textBox=textBox2;
						break;
					case 3:
						textBox=textBox3;
						break;
					case 4:
						textBox=textBox4;
						break;
					case 5:
						textBox=textBox5;
						break;
					case 6:
						textBox=textBox6;
						break;
					default:
						throw new ApplicationException("Error parsing unlock code");
				}
				textBox.Text=codeChar.ToString();
				textBox.Enabled=false;
				i++;
				#endregion
			}
			_unlockCodeTimer.Start();
		}
		
		///<summary>Unlocks the form so the user can enter an Unlock Code.</summary>
		private void SetFormUnlocked(){
			_isFormLocked=false;
			_unlockCodeTimer.Stop();
			_mobileDataByte=null;
			picBoxMain.Image=null;
			this.Controls.OfType<TextBox>().ToList().ForEach(x => {
				x.Enabled=true;
				x.Text="";
			});
		}

		///<summary>Currently not used. We might bring this back one day.
		///Locks or Unlocks the form when clicked.</summary>
		private void butLock_Click(object sender,EventArgs e) {
			//switch(butLock.ImageIndex){
			//	case 0://Locked to Unlocked
			//		if(!MsgBox.Show(MsgBoxButtons.YesNo,"Are you sure you would like to clear the existing unlock code?","Attention")){
			//			return;
			//		}
			//		MobileDataBytes.Delete(_mobileDataByte.MobileDataByteNum);
			//		SetFormUnlocked();
			//		break;
			//	case 1://Unlocked to Locked
			//		VerifyAndSetUnlockCode(GetUiUnlockCode());
			//		break;
			//}
		}

		///<summary>Moves the focus between the textBoxes and butLock as needed.</summary>
		private void textBox_KeyUp(object sender,KeyEventArgs e) {
			if(!e.KeyCode.Between(Keys.D0,Keys.Z) && !e.KeyCode.Between(Keys.NumPad0,Keys.NumPad9)){
				(sender as TextBox).Text="";
				return;
			}
			if((sender as TextBox).Text.IsNullOrEmpty()){
				return;
			}
			if(sender==textBox1){
				textBox2.Focus();
			}
			else if(sender==textBox2){
				textBox3.Focus();
			}
			else if(sender==textBox3){
				textBox4.Focus();
			}
			else if(sender==textBox4){
				textBox5.Focus();
			}
			else if(sender==textBox5){
				textBox6.Focus();
			}
			//else if(sender==textBox6){
			//	butLock.Focus();
			//}
		}

		///<summary>When the form closes make sure to delete any MobileDataByte rows.</summary>
		private void FormMobileCode_FormClosing(object sender,FormClosingEventArgs e) {
			_unlockCodeTimer.Stop();
			_unlockCodeTimer.Dispose();
			_unlockCodeTimer=null;
			if(_mobileDataByte!=null) {//User might have never set a code.
				MobileDataBytes.Delete(_mobileDataByte.MobileDataByteNum);
			}
			picBoxMain.Image?.Dispose();
		}

	}
}