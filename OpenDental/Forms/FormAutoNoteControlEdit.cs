using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNoteControlEdit:FormODBase {
		public bool IsNew;
		///<summary>The current AutoNoteControl that is being edited, whether new or not.</summary>
		public AutoNoteControl ControlCur;

		public FormAutoNoteControlEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteControlEdit_Load(object sender,EventArgs e) {
			textBoxControlDescript.Text=ControlCur.Descript;
			textBoxControlLabel.Text=ControlCur.ControlLabel;
			comboType.Items.Clear();
			comboType.Items.Add("Text");
			comboType.Items.Add("OneResponse");
			comboType.Items.Add("MultiResponse");
			comboType.SelectedItem=ControlCur.ControlType;
			textOptions.Text=ControlCur.ControlOptions;
		}

		private void comboType_SelectedIndexChanged(object sender,EventArgs e) {
			switch(comboType.SelectedItem.ToString()) {
				case "Text":
					labelResponses.Text=Lan.g(this,"Default text");
					butAutoNoteResp.Visible=false;
					break;
				case "OneResponse":
					labelResponses.Text=Lan.g(this,"Possible responses (one line per item)");
					butAutoNoteResp.Visible=true;
					break;
				case "MultiResponse":
					labelResponses.Text=Lan.g(this,"Possible responses (one line per item)");
					butAutoNoteResp.Visible=false;
					break;
				
			}
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(textOptions.Text==""){
				return;
			}
			int selStart=textOptions.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRow=0;
			int sumPreviousLines=0;
			string[] linesOrig=new string[textOptions.Lines.Length];
			textOptions.Lines.CopyTo(linesOrig,0);
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(selStart < sumPreviousLines+textOptions.Lines[l].Length) {
					selectedRow=l;
					break;
				}
			}
			//swap rows
			int newSelectedRow;
			if(selectedRow==0) {
				newSelectedRow=0;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newtext="";
				for(int l=0;l<textOptions.Lines.Length;l++) {
					if(l>0) {
						newtext+="\r\n";
					}
					if(l==selectedRow) {
						newtext+=linesOrig[selectedRow-1];
					}
					else if(l==selectedRow-1) {
						newtext+=linesOrig[selectedRow];
					}
					else {
						newtext+=linesOrig[l];
					}
				}
				textOptions.Text=newtext;
				newSelectedRow=selectedRow-1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRow==l) {
					textOptions.Select(sumPreviousLines,textOptions.Lines[l].Length);
					break;
				}
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(textOptions.Text=="") {
				return;
			}
			int selStart=textOptions.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRow=0;
			int sumPreviousLines=0;
			string[] linesOrig=new string[textOptions.Lines.Length];
			textOptions.Lines.CopyTo(linesOrig,0);
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(selStart < sumPreviousLines+textOptions.Lines[l].Length) {
					selectedRow=l;
					break;
				}
			}
			//swap rows
			int newSelectedRow;
			if(selectedRow==textOptions.Lines.Length-1) {
				newSelectedRow=textOptions.Lines.Length-1;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newtext="";
				for(int l=0;l<textOptions.Lines.Length;l++) {
					if(l>0) {
						newtext+="\r\n";
					}
					if(l==selectedRow) {
						newtext+=linesOrig[selectedRow+1];
					}
					else if(l==selectedRow+1) {
						newtext+=linesOrig[selectedRow];
					}
					else {
						newtext+=linesOrig[l];
					}
				}
				textOptions.Text=newtext;
				newSelectedRow=selectedRow+1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRow==l) {
					textOptions.Select(sumPreviousLines,textOptions.Lines[l].Length);
					break;
				}
			}
		}

		private void butAutoNoteResp_Click(object sender,EventArgs e) {
			if(comboType.SelectedItem.ToString()!="OneResponse") {
				MsgBox.Show(this,"Can only add AutoNotes to single response types.");
				return;//This shouldn't happen since the button should not be visible. Adding just in case.
			}
			using FormAutoNoteResponsePicker FormARP=new FormAutoNoteResponsePicker();
			if(FormARP.ShowDialog()!=DialogResult.OK) {
				return;//user canceled out of FormAutoNoteResponsePicker.
			}
			//The selected AutoNote should be in the format "Auto Note Response : {AutoNoteName}".
			string selectedResponse=FormARP.AutoNoteResponseText;
			if(textOptions.SelectionStart < textOptions.Text.Length-1) {
				//The cursor is not at the end of the textbox. Insert at the cursor location.
				textOptions.Text=textOptions.Text.Insert(textOptions.SelectionStart,selectedResponse);
			}
			else {//otherwise, just tack it on the end
				textOptions.Text+=selectedResponse+"\r\n";//Add to its own line. 
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Completely delete this prompt?  It will not be available from any AutoNote.")) {
				return;
			}
			AutoNoteControls.Delete(ControlCur.AutoNoteControlNum);
			DialogResult=DialogResult.OK;
		}
		
		private void textBoxControlLabel_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				e.SuppressKeyPress=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textBoxControlDescript.Text.ToString()=="" 
				|| comboType.SelectedIndex==-1) 
			{
				MsgBox.Show(this,"Please make sure that the Description and Type are not blank");
				return;
			}
			if(!Regex.IsMatch(textBoxControlDescript.Text,"^[a-zA-Z_0-9 ]*$")){
				MsgBox.Show(this,"The description can only contain letters, numbers, underscore, and space.");
				return;
			}
			ControlCur.Descript=textBoxControlDescript.Text.ToString();
			ControlCur.ControlLabel=textBoxControlLabel.Text.ToString();
			ControlCur.ControlType=comboType.SelectedItem.ToString();
			ControlCur.ControlOptions=textOptions.Text;
			if(IsNew) {
				AutoNoteControls.Insert(ControlCur);
			}
			else {
				AutoNoteControls.Update(ControlCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}