using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobNoteEdit:FormODBase {
		public JobNote _jobNote;

		public FormJobNoteEdit(JobNote jobNote) {
			_jobNote=jobNote.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public JobNote JobNoteCur {
			get {
				return _jobNote;
			}
		}

		private void FormJobNoteEdit_Load(object sender,EventArgs e) {
			textDateTime.Text=_jobNote.DateTimeNote.ToString();
			textUser.Text=Userods.GetName(_jobNote.UserNum);
			textNote.Text=_jobNote.Note;
			if(Security.CurUser.UserNum!=_jobNote.UserNum) {
				textNote.ReadOnly=true;
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete note?")) {
				return;
			}
			_jobNote=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textNote.Text=="") {
				MsgBox.Show(this,"Cannot save a blank note.");
				return;
			}
			if(!DateTime.TryParse(textDateTime.Text,out _jobNote.DateTimeNote)) {
				MsgBox.Show(this,"Please fix date.");
				return;
			}
			_jobNote.Note=textNote.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	
	}
}