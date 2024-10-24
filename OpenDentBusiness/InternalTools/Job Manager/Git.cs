using CodeBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class Git {
		public string RepoPath;

		/// <summary>Checks out the specified branch.  Throws Exceptions.</summary>
		public void Checkout(string branchName) {
			GitResult gitResult=RunGitCommand("checkout "+branchName);
			if(gitResult.ExitCode!=0) {
				throw new ODException(gitResult.Error);//Something went wrong
			}
		}

		/// <summary>Executes a pull on the current branch.  Assumes the local branch has an upstream set.  Throws Exceptions.</summary>
		public void Pull() {
			GitResult gitResult=RunGitCommand("pull");
			if(gitResult.ExitCode!=0) {
				throw new ODException(gitResult.Error);//Something went wrong
			}
		}

		/// <summary>Creates a local branch called branchName based on the current branch, then checks it out.  Throws Exceptions.</summary>
		public void CreateLocalBranchAndCheckout(string branchName) {
			GitResult gitResult=RunGitCommand("checkout -b "+branchName);
			if(gitResult.ExitCode!=0) {
				throw new ODException(gitResult.Error);//Something went wrong
			}
		}

		/// <summary>Pushes the current local branch to the remote and sets up a tracking relationship between the new remote branch and the current local branch.  Throws Exceptions.</summary>
		public void PushToRemote() {
			GitResult gitResult=RunGitCommand("push -u origin HEAD");//HEAD is used to grab the current branch name
			if(gitResult.ExitCode!=0) {
				throw new ODException(gitResult.Error);//Something went wrong
			}
		}

		/// <summary>Returns true if origin/branchName exists.  Throws Exceptions.</summary>
		public bool RemoteBranchExists(string branchName) {
			//Checks all references in the remote (ls-remote)
			//Only looks at refs/heads (--heads) and compares them to refs/heads/branchName
			//Returns an exit code of 0 if it exits and 2 if it does not (--exit-code)
			GitResult gitResult=RunGitCommand("ls-remote --exit-code --heads origin refs/heads/"+branchName);
			if(gitResult.ExitCode==0) {//Exists
				return true;
			}
			if(gitResult.ExitCode==2) {//Does not exist
				return false;
			}
			throw new ODException(gitResult.Error);//Something went wrong
		}

		/// <summary>Returns true when there are no changes in the current working tree.  Throws Exceptions.</summary>
		public bool IsCleanWorkingTree() {
			//Consider a 'clean' working treen to have no pending changes and no new 'untracked' files.
			RunGitCommand("add .");//Add any untracked files to the index. Otherwise they won't be seen by diff-index.
			GitResult gitResult=RunGitCommand("diff-index --quiet HEAD");//Compares working tree and index with last commit.  Returns 0 if clean and 1 if unclean.
			RunGitCommand("restore --staged .");//Unstage files added to put things back the way they were.
			if(gitResult.ExitCode==0) {//Clean.
				return true;
			}
			if(gitResult.ExitCode==1) {//Unclean.
				return false;
			}
			throw new ODException(gitResult.Error);//Something went wrong
		}

		/// <summary>Takes a command as string, arguments and all, runs it using Git, and then returns a GitResult.</summary>
		private GitResult RunGitCommand(string command) {
			GitResult gitResult=new GitResult();
			ProcessStartInfo processStartInfo=new ProcessStartInfo();
			processStartInfo.FileName="git"; //Assume that the engineer has Git installed
			processStartInfo.Arguments=command;
			processStartInfo.WorkingDirectory=RepoPath;
			processStartInfo.UseShellExecute=false;
			processStartInfo.RedirectStandardOutput=true;
			processStartInfo.RedirectStandardError=true;
			processStartInfo.CreateNoWindow=true;
			using (Process process=new Process()) {
				process.StartInfo=processStartInfo;
				process.Start();
				process.WaitForExit();
				gitResult.Output=process.StandardOutput.ReadToEnd();
				gitResult.Error=process.StandardError.ReadToEnd();
				gitResult.ExitCode=process.ExitCode;
			}
			return gitResult;
		}

		///<summary>Helper class returned by RunGitCommand(). Gives access to the StandardOutput, Standard Error, and ExitCode results from the Process that was used to execute the command.</summary>
		private class GitResult {
			public string Output;
			public string Error;
			public int    ExitCode;
		}
	}
}
