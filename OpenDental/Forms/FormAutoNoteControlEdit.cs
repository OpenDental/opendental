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
		public AutoNoteControl AutoNoteControlCur;

		public FormAutoNoteControlEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteControlEdit_Load(object sender,EventArgs e) {
			textBoxControlDescript.Text=AutoNoteControlCur.Descript;
			textBoxControlLabel.Text=AutoNoteControlCur.ControlLabel;
			comboType.Items.Clear();
			comboType.Items.Add("Text");
			comboType.Items.Add("OneResponse");
			comboType.Items.Add("MultiResponse");
			comboType.SelectedItem=AutoNoteControlCur.ControlType;
			textOptions.Text=AutoNoteControlCur.ControlOptions;
		}

		private void comboType_SelectedIndexChanged(object sender,EventArgs e) {
			switch(comboType.GetSelected<string>()) {
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
			int selStartNum=textOptions.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRowNum=0;
			int sumPreviousLines=0;
			string[] strArrayLinesOrig=new string[textOptions.Lines.Length];
			textOptions.Lines.CopyTo(strArrayLinesOrig,0);
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(selStartNum < sumPreviousLines+textOptions.Lines[l].Length) {
					selectedRowNum=l;
					break;
				}
			}
			//swap rows
			int newSelectedRowNum;
			if(selectedRowNum==0) {
				newSelectedRowNum=0;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newText="";
				for(int l=0;l<textOptions.Lines.Length;l++) {
					if(l>0) {
						newText+="\r\n";
					}
					if(l==selectedRowNum) {
						newText+=strArrayLinesOrig[selectedRowNum-1];
					}
					else if(l==selectedRowNum-1) {
						newText+=strArrayLinesOrig[selectedRowNum];
					}
					else {
						newText+=strArrayLinesOrig[l];
					}
				}
				textOptions.Text=newText;
				newSelectedRowNum=selectedRowNum-1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRowNum==l) {
					textOptions.Select(sumPreviousLines,textOptions.Lines[l].Length);
					break;
				}
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(textOptions.Text=="") {
				return;
			}
			int selectionStart=textOptions.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRow=0;
			int sumPreviousLines=0;
			string[] stringArrayLinesOrig=new string[textOptions.Lines.Length];
			textOptions.Lines.CopyTo(stringArrayLinesOrig,0);
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(selectionStart < sumPreviousLines+textOptions.Lines[l].Length) {
					selectedRow=l;
					break;
				}
			}
			//swap rows
			int newSelectedRowNum;
			if(selectedRow==textOptions.Lines.Length-1) {
				newSelectedRowNum=textOptions.Lines.Length-1;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newText="";
				for(int i=0;i<textOptions.Lines.Length;i++) {
					if(i>0) {
						newText+="\r\n";
					}
					if(i==selectedRow) {
						newText+=stringArrayLinesOrig[selectedRow+1];
					}
					else if(i==selectedRow+1) {
						newText+=stringArrayLinesOrig[selectedRow];
					}
					else {
						newText+=stringArrayLinesOrig[i];
					}
				}
				textOptions.Text=newText;
				newSelectedRowNum=selectedRow+1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int l=0;l<textOptions.Lines.Length;l++) {
				if(l>0) {
					sumPreviousLines+=textOptions.Lines[l-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRowNum==l) {
					textOptions.Select(sumPreviousLines,textOptions.Lines[l].Length);
					break;
				}
			}
		}

		private void butAutoNoteResp_Click(object sender,EventArgs e) {
			if(comboType.GetSelected<string>()!="OneResponse") {
				MsgBox.Show(this,"Can only add AutoNotes to single response types.");
				return;//This shouldn't happen since the button should not be visible. Adding just in case.
			}
			using FormAutoNoteResponsePicker formAutoNoteResponsePicker=new FormAutoNoteResponsePicker();
			if(formAutoNoteResponsePicker.ShowDialog()!=DialogResult.OK) {
				return;//user canceled out of FormAutoNoteResponsePicker.
			}
			//The selected AutoNote should be in the format "Auto Note Response : {AutoNoteName}".
			string selectedResponse=formAutoNoteResponsePicker.AutoNoteResponseText;
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
			AutoNoteControls.Delete(AutoNoteControlCur.AutoNoteControlNum);
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
			AutoNoteControlCur.Descript=textBoxControlDescript.Text.ToString();
			AutoNoteControlCur.ControlLabel=textBoxControlLabel.Text.ToString();
			AutoNoteControlCur.ControlType=comboType.GetSelected<string>();
			AutoNoteControlCur.ControlOptions=textOptions.Text;
			if(IsNew) {
				AutoNoteControls.Insert(AutoNoteControlCur);
			}
			else {
				AutoNoteControls.Update(AutoNoteControlCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}