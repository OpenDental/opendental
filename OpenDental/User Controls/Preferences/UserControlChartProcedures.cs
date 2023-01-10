using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlChartProcedures:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlChartProcedures() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butAgingProcLifoDetails_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"Not recommended. Determines whether or not insurance estimates are created when a procedure is entered with a previous date (earlier than today) and an " +
				"entry status of Complete. Typically only used by those who regularly enter completed procedures for previous dates.\r\n\r\n" +
				"To prevent accidental activation, a password prompt will display when enabling this option. The password is abracadabra.");
		}

		private void butProcProvChangesCpDetails_Click(object sender,EventArgs e) {
			string html=@"Leave this checked to prevent provider mismatches. See
				<a href='https://www.opendental.com/manual/claimprocedureprovider.html' target='_blank'>Claimproc Provider</a>.<br><br>
				 Claim procedures not attached to a claim (e.g. for insurance estimates) always inherit the procedure provider.";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.SizeWindow=new Size(475,175);
			formWebBrowserPrefs.PointStart=PointToScreen(butProcProvChangesCpDetails.Location);
			formWebBrowserPrefs.ShowDialog();
		}

		private void checkClaimProcsAllowEstimatesOnCompl_Click(object sender,EventArgs e) {
			if(checkClaimProcsAllowEstimatesOnCompl.Checked) {//user is attempting to Allow Estimates to be created for backdated complete procedures
				using InputBox inputBox=new InputBox("Please enter password");
				inputBox.ShowDialog();
				if(inputBox.DialogResult!=DialogResult.OK) {
					checkClaimProcsAllowEstimatesOnCompl.Checked=false;
					return;
				}
				if(inputBox.textResult.Text!="abracadabra") {//To prevent unaware users from clicking this box
					checkClaimProcsAllowEstimatesOnCompl.Checked=false;
					MsgBox.Show(this,"Wrong password");
					return;
				}
			}
		}

		private void checkProcLockingIsAllowed_Click(object sender,EventArgs e) {
			if(checkProcLockingIsAllowed.Checked) {//if user is checking box			
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This option is not normally used, because all notes are already locked internally, and all changes to notes are viewable in the audit mode of the Chart module.  This option is only for offices that insist on locking each procedure and only allowing notes to be appended.  Using this option, there really is no way to unlock a procedure, regardless of security permission.  So locked procedures can instead be marked as invalid in the case of mistakes.  But it's a hassle to mark procedures invalid, and they also cause clutter.  This option can be turned off later, but locked procedures will remain locked.\r\n\r\nContinue anyway?")) {
					checkProcLockingIsAllowed.Checked=false;
				}
			}
			else {//unchecking box
				MsgBox.Show(this,"Turning off this option will not affect any procedures that are already locked or invalidated.");
			}
		}

		private void linkLabelProcLockingIsAllowedDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			try {
				Process.Start("https://www.opendental.com/manual/procedurelocking.html");
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+"https://www.opendental.com/manual/procedurelocking.html"+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillChartProcedures() {
			checkProcGroupNoteDoesAggregate.Checked=PrefC.GetBool(PrefName.ProcGroupNoteDoesAggregate);
			checkProcsPromptForAutoNote.Checked=PrefC.GetBool(PrefName.ProcPromptForAutoNote);
			checkSignatureAllowDigital.Checked=PrefC.GetBool(PrefName.SignatureAllowDigital);
			checkProcNoteConcurrencyMerge.Checked=PrefC.GetBool(PrefName.ProcNoteConcurrencyMerge);
			checkNotesProviderSigOnly.Checked=PrefC.GetBool(PrefName.NotesProviderSignatureOnly);
			checkAllowSettingProcsComplete.Checked=PrefC.GetBool(PrefName.AllowSettingProcsComplete);
			checkProcLockingIsAllowed.Checked=PrefC.GetBool(PrefName.ProcLockingIsAllowed);
			checkProcEditRequireAutoCode.Checked=PrefC.GetBool(PrefName.ProcEditRequireAutoCodes);
			checkClaimProcsAllowEstimatesOnCompl.Checked=PrefC.GetBool(PrefName.ClaimProcsAllowedToBackdate);
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"No prompt, don't change fee"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"No prompt, always change fee"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"Prompt, when patient portion changes"));
			comboProcFeeUpdatePrompt.Items.Add(Lan.g(this,"Prompt, always"));
			comboProcFeeUpdatePrompt.SelectedIndex=PrefC.GetInt(PrefName.ProcFeeUpdatePrompt);
			checkProcProvChangesCp.Checked=PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim);
			checkProcNoteSigsBlocked.Checked=PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete);
		}

		public bool SaveChartProcedures() {
			Changed|=Prefs.UpdateBool(PrefName.ProcGroupNoteDoesAggregate,checkProcGroupNoteDoesAggregate.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SignatureAllowDigital,checkSignatureAllowDigital.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProcNoteConcurrencyMerge,checkProcNoteConcurrencyMerge.Checked);
			Changed|=Prefs.UpdateBool(PrefName.NotesProviderSignatureOnly,checkNotesProviderSigOnly.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProcPromptForAutoNote,checkProcsPromptForAutoNote.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowSettingProcsComplete,checkAllowSettingProcsComplete.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProcLockingIsAllowed,checkProcLockingIsAllowed.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProcEditRequireAutoCodes,checkProcEditRequireAutoCode.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,checkClaimProcsAllowEstimatesOnCompl.Checked);
			Changed|=Prefs.UpdateLong(PrefName.ProcFeeUpdatePrompt,comboProcFeeUpdatePrompt.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.ProcProvChangesClaimProcWithClaim,checkProcProvChangesCp.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete,checkProcNoteSigsBlocked.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
