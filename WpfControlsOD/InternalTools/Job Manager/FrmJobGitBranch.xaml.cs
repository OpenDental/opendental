using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
/*
You must implement Alt key for the Access Key on the button.
*/
	public partial class FrmJobGitBranch:FrmODBase {
		public string BranchName;

		public FrmJobGitBranch() {
			InitializeComponent();
			Load+=FrmJobGitBranch_Load;
			radioVersioned.Click+=radioVersioned_Click;
			radioUnversioned.Click+=radioUnversioned_Click;
			Lang.F(this);
		}
		
		private void FrmJobGitBranch_Load(object sender,EventArgs e) {
			textBranchName.Text=BranchName;
			textPath.Text="C:\\Development\\Versioned"; //default path
		}

		private void radioVersioned_Click(object sender,EventArgs e) {
			textPath.Text="C:\\Development\\Versioned";
		}

		private void radioUnversioned_Click(object sender,EventArgs e) {
			textPath.Text="C:\\Development\\Unversioned";
		}

		private void butCreateBranch_Click(object sender,EventArgs e) {
			Git git=new Git();
			git.RepoPath=textPath.Text;
			//Validate path
			if(!Directory.Exists(git.RepoPath)) {
				string message=Lang.g(this,"The directory")+" "+git.RepoPath+" "+Lang.g(this,"does not exist.");
				MsgBox.Show(message);
				return;
			}
			//Create and checkout local branch based on master then push to remote.
			try {
				//All of the git methods can throw exceptions
				if(!git.IsCleanWorkingTree()) {
					MsgBox.Show(this,"There are changes in your working tree. Commit or stash them before continuing.");
					return;
				}
				if(git.RemoteBranchExists(BranchName)) {
					string message=Lang.g(this,"The branch origin/")+BranchName+" "+Lang.g(this,"already exists.");
					MsgBox.Show(message);
					return;
				}
				git.Checkout("master");
				git.Pull();
				git.CreateLocalBranchAndCheckout(BranchName);
				git.PushToRemote();
			}
			catch(ODException ex) {//Let the engineer handle any git errors
				MsgBox.Show(ex.Message);
				return;
			}
			IsDialogOK=true;
		}
	}
}