using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Crud;

namespace OpenDental {
	public partial class FormWebChatSessionNoteEdit:FormODBase {
		private WebChatNote _webChatNoteCur;

		public FormWebChatSessionNoteEdit(WebChatNote webChatNote) {
			_webChatNoteCur=webChatNote;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebChatSessionNoteEdit_Load(object sender,EventArgs e) {
			textDateTime.Text=_webChatNoteCur.DateTimeNote.ToString();
			if(_webChatNoteCur.TechName!="") {
				textTech.Text=_webChatNoteCur.TechName;
			}
			else {//Notes can be made without taking ownership.
				textTech.Text=Security.CurUser.UserName;
			}
			textNote.Text=_webChatNoteCur.Note;
			//this.Top+=150;
			if(_webChatNoteCur.IsNew) {
				textDateTime.ReadOnly=true;
			}
			//In future if techs editing each other's notes becomes an issue, we can require the TaskNoteEdit permission here.
			//Webchat ended more than a day ago, make textbox for note read-only so that it cannot be edited. Taken from webchatsession.
			if(_webChatNoteCur.DateTimeNote.AddDays(1)<DateTime.Now) {
				textNote.ReadOnly=true;
				butAutoNote.Enabled=false;
				butOK.Enabled=false;
				textDateTime.ReadOnly=true;
			}
		}

		private void butAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose formAutoNoteCompose=new FormAutoNoteCompose();
			formAutoNoteCompose.ShowDialog();
			if(formAutoNoteCompose.DialogResult==DialogResult.OK){
				textNote.Text+=formAutoNoteCompose.StrCompletedNote;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			if(_webChatNoteCur.IsNew) {
				handleCancel();
				return;
			}
			WebChatNotes.Delete(_webChatNoteCur.WebChatNoteNum);
			DialogResult=DialogResult.OK;
			Close();
		}

		/// <summary>Helper method to close the non-modal window on cancel.</summary>
		private void handleCancel() {
			DialogResult=DialogResult.Cancel;
			Close();//Needed because the window is called as a non-modal window.
		}

		private void butCancel_Click(object sender,EventArgs e) {
			handleCancel();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textNote.Text=="") {
				MsgBox.Show(this,"Please enter a note, or delete this entry.");
				return;
			}
			//We need the old datetime to check if the user made any changes.  We overrite WebChatNoteCur's date time below so need to get it here.
			DateTime dateTimeNoteOld=_webChatNoteCur.DateTimeNote;
			try {
				_webChatNoteCur.DateTimeNote=DateTime.Parse(textDateTime.Text);
			}
			catch{
				MsgBox.Show(this,"Please fix date.");
				return;
			}
			if(_webChatNoteCur.IsNew) {
				_webChatNoteCur.IsNew=false;
				_webChatNoteCur.Note=textNote.Text;
				_webChatNoteCur.TechName=textTech.Text;
				WebChatNotes.Insert(_webChatNoteCur);
				DialogResult=DialogResult.OK;
				Close();//Needed because the window is called as a non-modal window.
				return;
			}
			if(_webChatNoteCur.Note!=textNote.Text || dateTimeNoteOld!=_webChatNoteCur.DateTimeNote) {
				_webChatNoteCur.Note=textNote.Text;
				WebChatNotes.Update(_webChatNoteCur);
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			//Intentionally blank, user opened an existing webChatNote and did not change the note but clicked OK.
			//This is effectively equivilent to a Cancel click
			handleCancel();		
		}
	}
}