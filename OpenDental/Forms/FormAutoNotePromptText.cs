using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutoNotePromptText:FormODBase {
		///<summary>Set this value externally.</summary>
		public string PromptText;
		///<summary>What the user entered.  This can be set externally to the default value.</summary>
		public string ResultText;
		///<summary>If user has the option to go back</summary>
		public bool IsGoBack;
		///<summary>The string value of previous user response</summary>
		public string CurPromptResponse;
		/// <summary>The string of the autonote description which is used to bring up the actual prompt.</summary>
		private string _noteDescript;

		public FormAutoNotePromptText(string noteDescription) {
			InitializeComponent();
			InitializeLayoutManager();
			_noteDescript=noteDescription;
			Lan.F(this);
		}

		private void FormAutoNotePromptText_Load(object sender,EventArgs e) {
			CurPromptResponse=!string.IsNullOrEmpty(CurPromptResponse) ? CurPromptResponse : "";
			Location=new Point(Left,Top+150);
			butBack.Visible=IsGoBack;
			labelPrompt.Text=PromptText;
			if(CurPromptResponse!="") {//display previous user response
				textMain.Text=CurPromptResponse;
			}
			else {
				textMain.Text=ResultText;
			}
			textMain.SelectionStart=textMain.Text.Length;//Start cursor at the end of the string passed in. 
		}

		private void butRemovePrompt_Click(object sender,EventArgs e) {
			ResultText="";
			DialogResult=DialogResult.OK;
		}

		private void butSkipForNow_Click(object sender,EventArgs e) {
			ResultText="[Prompt:\""+_noteDescript+"\"]";
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