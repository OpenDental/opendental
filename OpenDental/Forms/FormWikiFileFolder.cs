using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormWikiFileFolder:FormODBase {
		public bool IsFolderMode;
		public string LinkSelected;

		public FormWikiFileFolder() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiFileFolder_Load(object sender,EventArgs e) {
			if(IsFolderMode) {
				Text=Lan.g(this,"Insert Folder Link");
			}
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			if(IsFolderMode) {
				using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
				if(folderBrowserDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				textLink.Text=folderBrowserDialog.SelectedPath;
				return;
			}
			if(ODCloudClient.IsAppStream) {
				//Block File browsing to prevent access to file directory of the VM. We will redirect to use the ODCloudClient in a future job.
				MessageBox.Show(Lans.g(this,"File browsing not allowed in web mode."));
				return;
			}
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textLink.Text=openFileDialog.FileName;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(IsFolderMode){
				if(!ODBuild.IsWeb() && !Directory.Exists(textLink.Text)) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Folder does not exist. Continue anyway?")) {
						return;
					}
					/*try {
						Directory.CreateDirectory(textLink.Text);
					}
					catch(Exception ex) {
						MessageBox.Show(this,ex.Message);
						return;
					}*/
				}
			}
			else{//file mode
				if(!ODBuild.IsWeb() && !File.Exists(textLink.Text)) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"File does not exist. Continue anyway?")) {
						return;
					}
				}
			}
			LinkSelected=textLink.Text;
			DialogResult=DialogResult.OK;
		}

	}
}