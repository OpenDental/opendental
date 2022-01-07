using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormSigButDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private SigElementDef[] _arraySigElementDefUser;
		private SigElementDef[] _arraySigElementDefExtras;
		private SigElementDef[] _arraySigElementDefMessages;
		///<summary></summary>
		private SigButDef _sigButDefCur;

		///<summary></summary>
		public FormSigButDefEdit(SigButDef sigButDef)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_sigButDefCur=sigButDef;
		}

		private void FormSigButDefEdit_Load(object sender,EventArgs e) {
			if(_sigButDefCur.ComputerName=="") {
				radioAll.Checked=true;
			}
			else{
				radioOne.Checked=true;
				textComputerName.Text=_sigButDefCur.ComputerName;
			}
			textButtonText.Text=_sigButDefCur.ButtonText;
			textSynchIcon.Text=_sigButDefCur.SynchIcon.ToString();
			_arraySigElementDefUser=SigElementDefs.GetSubList(SignalElementType.User);
			_arraySigElementDefExtras=SigElementDefs.GetSubList(SignalElementType.Extra);
			_arraySigElementDefMessages=SigElementDefs.GetSubList(SignalElementType.Message);
			comboTo.Items.Clear();
			comboTo.Items.Add(Lan.g(this,"none"));
			comboTo.SelectedIndex=0;
			for(int i=0;i<_arraySigElementDefUser.Length;i++) {
				comboTo.Items.Add(_arraySigElementDefUser[i].SigText);
				if(_sigButDefCur.SigElementDefNumUser==_arraySigElementDefUser[i].SigElementDefNum){
					comboTo.SelectedIndex=i+1;
				}
			}
			comboExtras.Items.Clear();
			comboExtras.Items.Add(Lan.g(this,"none"));
			comboExtras.SelectedIndex=0;
			for(int i=0;i<_arraySigElementDefExtras.Length;i++) {
				comboExtras.Items.Add(_arraySigElementDefExtras[i].SigText);
				if(_sigButDefCur.SigElementDefNumExtra==_arraySigElementDefExtras[i].SigElementDefNum) {
					comboExtras.SelectedIndex=i+1;
				}
			}
			comboMessage.Items.Clear();
			comboMessage.Items.Add(Lan.g(this,"none"));
			comboMessage.SelectedIndex=0;
			for(int i=0;i<_arraySigElementDefMessages.Length;i++) {
				comboMessage.Items.Add(_arraySigElementDefMessages[i].SigText);
				if(_sigButDefCur.SigElementDefNumMsg==_arraySigElementDefMessages[i].SigElementDefNum) {
					comboMessage.SelectedIndex=i+1;
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
				SigButDefs.Delete(_sigButDefCur);//also deletes elements
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textSynchIcon.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textButtonText.Text=="") {
				MsgBox.Show(this,"Please enter a text description first.");
				return;
			}
			if(textSynchIcon.Text=="") {
				textSynchIcon.Text="0";
			}
			_sigButDefCur.ButtonText=textButtonText.Text;
			_sigButDefCur.SynchIcon=PIn.Byte(textSynchIcon.Text);
			_sigButDefCur.SigElementDefNumUser=(comboTo.SelectedIndex>0) ? _arraySigElementDefUser[comboTo.SelectedIndex-1].SigElementDefNum : 0;
			_sigButDefCur.SigElementDefNumExtra=(comboExtras.SelectedIndex>0) ? _arraySigElementDefExtras[comboExtras.SelectedIndex-1].SigElementDefNum : 0;
			_sigButDefCur.SigElementDefNumMsg=(comboMessage.SelectedIndex>0) ? _arraySigElementDefMessages[comboMessage.SelectedIndex-1].SigElementDefNum : 0;
			if(IsNew) {
				SigButDefs.Insert(_sigButDefCur);
			}
			else{
				SigButDefs.Update(_sigButDefCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormSigButDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			DataValid.SetInvalid(InvalidType.SigMessages);
		}

		

		

		

		

		


	}
}





















