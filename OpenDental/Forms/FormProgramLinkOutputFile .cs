using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProgramLinkOutputFile:FormODBase {

		private Program _curProg;

		public FormProgramLinkOutputFile(Program program) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_curProg=program;
		}

		private void FormProgramLinkOutputFile_Load(object sender,EventArgs e) {
			textTemplate.Text=_curProg.FileTemplate;
			textPath.Text=_curProg.FilePath;
		}

		private void butImport_Click(object sender,EventArgs e) {
			OpenFileDialog openFileDialog=new OpenFileDialog();
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string[] fileNames=openFileDialog.FileNames;
			if(fileNames.Length<1) {
				return;
			}
			textPath.Text=fileNames[0];
		}

		private void butOK_Click(object sender,EventArgs e) {
			_curProg.FilePath=textPath.Text;
			_curProg.FileTemplate=textTemplate.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butReplacements_Click(object sender,EventArgs e) {
			using FormMessageReplacements formMR=new FormMessageReplacements(MessageReplaceType.Patient|MessageReplaceType.Referral);
			formMR.IsSelectionMode=true;
			formMR.ShowDialog();
			if(formMR.DialogResult!=DialogResult.OK) {
				return;
			}
			textTemplate.Focus();
			int cursorIndex=textTemplate.SelectionStart;
			textTemplate.Text=textTemplate.Text.Insert(cursorIndex,formMR.Replacement);
			textTemplate.SelectionStart=cursorIndex+formMR.Replacement.Length;
		}

	}
}