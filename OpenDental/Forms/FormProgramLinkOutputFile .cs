using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProgramLinkOutputFile:FormODBase {

		private Program _program;

		public FormProgramLinkOutputFile(Program program) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_program=program;
		}

		private void FormProgramLinkOutputFile_Load(object sender,EventArgs e) {
			textTemplate.Text=_program.FileTemplate;
			textPath.Text=_program.FilePath;
		}

		private void butImport_Click(object sender,EventArgs e) {
			string[] stringArrayFileNames;
			if(!ODBuild.IsWeb() && ODCloudClient.IsAppStream) {
				List<string> listImportFilePaths=new List<string>(){ODCloudClient.ImportFileForCloud()};
				if(listImportFilePaths[0].IsNullOrEmpty()) {
					return;
				}
				stringArrayFileNames=listImportFilePaths.ToArray();
			}
			else {
				OpenFileDialog openFileDialog=new OpenFileDialog();
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				stringArrayFileNames=openFileDialog.FileNames;
			}
			if(stringArrayFileNames.Length<1) {
				return;
			}
			textPath.Text=stringArrayFileNames[0];
		}

		private void butOK_Click(object sender,EventArgs e) {
			_program.FilePath=textPath.Text;
			_program.FileTemplate=textTemplate.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butReplacements_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.Referral);
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(listMessageReplaceTypes);
			formMessageReplacements.IsSelectionMode=true;
			formMessageReplacements.ShowDialog();
			if(formMessageReplacements.DialogResult!=DialogResult.OK) {
				return;
			}
			textTemplate.Focus();
			int indexCursor=textTemplate.SelectionStart;
			textTemplate.Text=textTemplate.Text.Insert(indexCursor,formMessageReplacements.Replacement);
			textTemplate.SelectionStart=indexCursor+formMessageReplacements.Replacement.Length;
		}

	}
}