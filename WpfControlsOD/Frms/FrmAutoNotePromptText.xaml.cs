using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmAutoNotePromptText:FrmODBase {
//todo: Button image alignment not supported yet: MiddleRight
		///<summary>Set this value externally.</summary>
		public string PromptText;
		///<summary>What the user entered.  This can be set externally to the default value.</summary>
		public string ResultText;
		///<summary>If user has the option to go back</summary>
		public bool IsGoBack;
		///<summary>The string value of previous user response</summary>
		public string PromptResponseCur;
		/// <summary>The string of the autonote description which is used to bring up the actual prompt.</summary>
		private string _noteDescription;
		/// <summary>Replaces DialogueResult.Retry. Use IsDialogueOk=False and this public field for the same behavior.</summary>
		public bool IsRetry;

		public FrmAutoNotePromptText(string noteDescription) {
			InitializeComponent();
			_noteDescription=noteDescription;
			Load+=FrmAutoNotePromptText_Load;
			PreviewKeyDown+=FrmAutoNotePromptText_PreviewKeyDown;
		}

		private void FrmAutoNotePromptText_Load(object sender,EventArgs e) {
			Lang.F(this);
			PromptResponseCur=string.IsNullOrEmpty(PromptResponseCur) ? "" : PromptResponseCur;
			butBack.Visible=IsGoBack;
			labelPrompt.Text=PromptText;
			if(PromptResponseCur!="") {//display previous user response
				textMain.Text=PromptResponseCur;
			}
			else {
				textMain.Text=ResultText;
			}
			if(PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete)) {
				butRemovePrompt.IsEnabled=false;
			}
			textMain.SelectionStart=textMain.Text.Length;//Start cursor at the end of the string passed in.
			IsRetry=false;
			int adj=ScaleFormValue(150);
			_formFrame.Top+=adj;
		}

		private void FrmAutoNotePromptText_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butRemovePrompt.IsAltKey(Key.R,e)) {
				butRemovePrompt_Click(this,new EventArgs());
			}
			if(butSkipForNow.IsAltKey(Key.S,e)) {
				butSkipForNow_Click(this,new EventArgs());
			}
			if(butNext.IsAltKey(Key.N,e)) {
				butNext_Click(this,new EventArgs());
			}
			if(butBack.IsAltKey(Key.B,e)) {
				butBack_Click(this,new EventArgs());
			}
			if(butExit.IsAltKey(Key.E,e)) {
				butExit_Click(this,new EventArgs());
			}
		}

		private void butRemovePrompt_Click(object sender,EventArgs e) {
			ResultText="";
			IsDialogOK=true;
		}

		private void butSkipForNow_Click(object sender,EventArgs e) {
			ResultText="[Prompt:\""+_noteDescription+"\"]";
			IsDialogOK=true;
		}

		private void butNext_Click(object sender,EventArgs e) {
			ResultText=textMain.Text;
			IsDialogOK=true;
		}

		private void butBack_Click(object sender,EventArgs e) {
			IsDialogOK=false;
			IsRetry=true;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			ResultText=textMain.Text;
			IsDialogOK=true;
		}

		private void butExit_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Abort autonote entry?")) {
				return;
			}
			IsDialogOK=false;
		}
		
	}
}