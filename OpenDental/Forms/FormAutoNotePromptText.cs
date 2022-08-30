using System;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNotePromptText:FormODBase {
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

		public FormAutoNotePromptText(string noteDescription) {
			InitializeComponent();
			InitializeLayoutManager();
			_noteDescription=noteDescription;
			Lan.F(this);
		}

		private void FormAutoNotePromptText_Load(object sender,EventArgs e) {
			PromptResponseCur=string.IsNullOrEmpty(PromptResponseCur) ? "" : PromptResponseCur;
			Point point=new Point(Left,Top+150);
			Location=point;
			butBack.Visible=IsGoBack;
			labelPrompt.Text=PromptText;
			if(PromptResponseCur!="") {//display previous user response
				textMain.Text=PromptResponseCur;
			}
			else {
				textMain.Text=ResultText;
			}
			if(PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete)) {
				butSkip.Enabled=false;
			}
			textMain.SelectionStart=textMain.Text.Length;//Start cursor at the end of the string passed in. 
		}

		private void butRemovePrompt_Click(object sender,EventArgs e) {
			ResultText="";
			DialogResult=DialogResult.OK;
		}

		private void butSkipForNow_Click(object sender,EventArgs e) {
			ResultText="[Prompt:\""+_noteDescription+"\"]";
			DialogResult=DialogResult.OK;
		}

		private void butNext_Click(object sender,EventArgs e) {
			ResultText=textMain.Text;	
			DialogResult=DialogResult.OK;
		}

		private void butBack_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Retry;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			ResultText=textMain.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Abort autonote entry?")) {
				return;
			}
			DialogResult=DialogResult.Cancel;
		}

		
	}
}