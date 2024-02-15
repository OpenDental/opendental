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
	public partial class FrmAutoNotePromptList:FrmODBase {
		///<summary>Set this value externally. This is used to fill in the prompt textbox for the form</summary>
		public string PromptText;
		///<summary>What items the user has selected on the current auto note. Saves the user's selected response(s) for the parent form to use.</summary>
		public string ResultText;
		///<summary>The string value representing the list to pick from.  One item per line.</summary>
		public string PromptOptions;
		///<summary>If user has the option to go back</summary>
		public bool IsGoBack;
		///<summary>The string value of previous user response. This will be null if the user is moving on to the next form. Otherwise, it will have the previous response data of the form they are returning to</summary>
		public string PreviousResponse="";
		/// <summary>The string of the autonote description which is used to bring up the actual prompt.</summary>
		private string _noteDescription;
		///<summary>Set true when user clicks back button</summary>
		public bool IsRetry;
		/// <summary>True if the form is multi response, false if one response.</summary>
		public bool IsMultiResponse;

		public FrmAutoNotePromptList(string noteDescription) {
			InitializeComponent();
			_noteDescription=noteDescription;
			Load+=FrmAutoNotePromptList_Load;
			KeyDown+=FrmAutoNotePromptList_KeyDown;
			PreviewKeyDown+=FrmAutoNotePromptList_PreviewKeyDown;
			listMain.MouseDoubleClick+=listMain_MouseDoubleClick;
		}

		private void FrmAutoNotePromptList_Load(object sender,EventArgs e) {
			Lang.F(this);
			labelPrompt.Text=PromptText;
			butBack.Visible=IsGoBack;
			//PromptOptions Example: "Lido 1:100,000 / manf\r\nSepto\r\nCarbo\r\nMeprivo\r\nTopical"
			string[] stringArrayLines=PromptOptions.Split(new string[] {"\r\n"},StringSplitOptions.RemoveEmptyEntries);
			if(PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete)) {
				butRemovePrompt.IsEnabled=false;
			}
			List<string> listPreviousLines;
			IsRetry=false;
			if(IsMultiResponse){
				//PreviousResponse example for multi response: "Lido 1:100,000 / manf, Septo, Carbo".
				//These items will then get selected in the loop below.
				//Normally this is empty because they did not go back.
				listPreviousLines=PreviousResponse.Split(new string[] {", "},StringSplitOptions.RemoveEmptyEntries).ToList();
				listMain.SelectionMode=SelectionMode.CheckBoxes;
			}
			else{//single
				listPreviousLines=new List<string>();
				listPreviousLines.Add(PreviousResponse);//example: 5
				butSelectAll.Visible=false;
				butSelectNone.Visible=false;
			}
			//This fills the main list with each auto note prompt option the user can select.
			for(int i=0;i<stringArrayLines.Length;i++) {
				listMain.Items.Add(stringArrayLines[i]);
				//If the user has hit back, reselect:
				if(listPreviousLines.Contains(stringArrayLines[i])) {
					listMain.SetSelected(listMain.Items.Count-1,true);
				}
			}
			int adj=ScaleFormValue(150);
			_formFrame.Top+=adj;
			Focus();//so that Alt keys will work.
		}

		private void listMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(IsMultiResponse || listMain.SelectedIndex==-1) {
				return;
			}
			ResultText=listMain.SelectedItem.ToString();
			IsDialogOK=true;
		}

		private void FrmAutoNotePromptList_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key!=Key.Enter) {
				return;
			}
			ResultText=listMain.GetStringSelectedItems();
			if(IsMultiResponse){//If it's multi response, we don't need to validate anything else here.
				IsDialogOK=true;
				return;
			}
			if(listMain.SelectedIndex==-1) {
				MsgBox.Show(this,"One response must be selected");
				return;
			}
			IsDialogOK=true;
		}
		private void FrmAutoNotePromptList_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butRemovePrompt.IsAltKey(Key.R,e)) {
				butRemovePrompt_Click(this,new EventArgs());
				return;
			}
			if(butNext.IsAltKey(Key.N,e)) {
				butNext_Click(this,new EventArgs());
				return;
			}
			if(butBack.IsAltKey(Key.B,e)) {
				butBack_Click(this,new EventArgs());
				return;
			}
			if(butPreview.IsAltKey(Key.P,e)) {
				butPreview_Click(this,new EventArgs());
				return;
			}
			if(butSelectAll.IsAltKey(Key.A,e)) {
				butSelectAll_Click(this,new EventArgs());
				return;
			}
			if(butSelectNone.IsAltKey(Key.O,e)) {
				butSelectNone_Click(this,new EventArgs());
				return;
			}
			if(butExit.IsAltKey(Key.E,e)) {
				butExit_Click(this,new EventArgs());
				return;
			}
		}

		private void butRemovePrompt_Click(object sender,EventArgs e) {
			ResultText="";
			IsDialogOK=true;
		}

		private void butNext_Click(object sender,EventArgs e) {
			ResultText=listMain.GetStringSelectedItems();
			if(listMain.SelectedIndices.Count==0) {
				ResultText="[Prompt:\""+_noteDescription+"\"]";
			}
			IsDialogOK=true;
		}

		private void butBack_Click(object sender,EventArgs e) {
			IsRetry=true;
			IsDialogOK=false;
		}
		
		private void butPreview_Click(object sender,EventArgs e) {
			if(!IsMultiResponse && listMain.SelectedIndex==-1) {
				MsgBox.Show(this,"One response must be selected");
				return;
			}
			FrmAutoNotePromptPreview frmAutoNotePromptPreview=new FrmAutoNotePromptPreview();
			ResultText=listMain.GetStringSelectedItems();
			frmAutoNotePromptPreview.ResultText=ResultText;
			frmAutoNotePromptPreview.ShowDialog();
			if(frmAutoNotePromptPreview.IsDialogOK) {
				ResultText=frmAutoNotePromptPreview.ResultText;
				IsDialogOK=true;
			}
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			listMain.SetAll(true);
		}

		private void butSelectNone_Click(object sender,EventArgs e) {
			listMain.SetAll(false);
		}

		private void butExit_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Abort autonote entry?")) {
				return;
			}
			IsDialogOK=false;
		}


	}
}