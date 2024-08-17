using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormQuickPasteNoteEdit : FormODBase {
		public QuickPasteNote QuickPasteNoteCur;

		///<summary></summary>
		public FormQuickPasteNoteEdit(QuickPasteNote quickPasteNote){
			//
			// Required for Windows Form Designer support
			//
			QuickPasteNoteCur=quickPasteNote.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQuickPasteNoteEdit_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit,true)) {
				textAbbreviation.ReadOnly=true;
				textAbbreviation.BackColor=SystemColors.Window;
				textNote.ReadOnly=true;
				textNote.BackColor=SystemColors.Window;
				butDelete.Enabled=false;
				butOK.Enabled=false;
			}
			textAbbreviation.Text=QuickPasteNoteCur.Abbreviation;
			textNote.Text=QuickPasteNoteCur.Note;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Delete note?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			QuickPasteNoteCur=null;//triggers an action in the calling form
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textAbbreviation.Text.Contains("?")) {
				MsgBox.Show(this,"Question mark not allowed in abbreviation.  Use the question mark later when trying to insert a quick note.");
				return;
			}
			if(textAbbreviation.Text.Contains("*")) {
				MsgBox.Show(this,"Asterisk character not allowed in abbreviation.  Use the asterisk later when trying to insert a quick note.");
				return;
			}
			QuickPasteNoteCur.Abbreviation=textAbbreviation.Text;
			if(QuickPasteNoteCur.Abbreviation!=""){
				string msgText=QuickPasteNotes.AbbrAlreadyInUse(QuickPasteNoteCur);
				if(!String.IsNullOrEmpty(msgText) && MessageBox.Show(msgText,Lan.g(this,"Warning"),MessageBoxButtons.YesNo)==DialogResult.No) {
					return;
				}
			}
			QuickPasteNoteCur.Note=textNote.Text;
			DialogResult=DialogResult.OK;
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			//We do not normally use TextChanged but we need to perform maintenance of the data when it loads, is saved, or is pasted to.
			textNote.Text=textNote.Text.Replace("\r\n","\n")//convert windows to \n
				.Replace("\r","\n")									//replace linux or other \n
																			//Mac is already \n
				.Replace("\n","\r\n");								//reset to Windows newline character		
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
	}
}





















