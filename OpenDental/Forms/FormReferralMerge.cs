using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReferralMerge:FormODBase {
		private long _referralNumInto;
		private long _referralNumFrom;

		public FormReferralMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butChangeReferralInto_Click(object sender,EventArgs e) {
			using FormReferralSelect formReferralSelect=new FormReferralSelect();
			formReferralSelect.IsSelectionMode=true;
			if(formReferralSelect.ShowDialog()==DialogResult.OK) {
				Referral referral=formReferralSelect.ReferralSelected;
				_referralNumInto=referral.ReferralNum;
				textReferralNameInto.Text=referral.LName+", "+referral.FName;
				textTitleInto.Text=referral.Title;
				checkIsPersonInto.Checked=!referral.NotPerson;
				checkIsDoctorInto.Checked=referral.IsDoctor;
				CheckUIState();
			}
		}

		private void butChangeReferralFrom_Click(object sender,EventArgs e) {
			using FormReferralSelect formReferralSelect=new FormReferralSelect();
			formReferralSelect.IsSelectionMode=true;
			if(formReferralSelect.ShowDialog()==DialogResult.OK) {
				Referral referral=formReferralSelect.ReferralSelected;
				_referralNumFrom=referral.ReferralNum;
				textReferralNameFrom.Text=referral.LName+", "+referral.FName;
				textTitleFrom.Text=referral.Title;
				checkIsPersonFrom.Checked=!referral.NotPerson;
				checkIsDoctorFrom.Checked=referral.IsDoctor;
				CheckUIState();
			}
		}

		private void CheckUIState() {
			butMerge.Enabled=(textReferralNameInto.Text.Trim()!="" && textReferralNameFrom.Text.Trim()!="");
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(_referralNumInto==_referralNumFrom) {
				MsgBox.Show(this,"Cannot merge the same referral.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure?  The results are permanent and cannot be undone.")) {
				return;
			}
			string differentFields="";
			if(textReferralNameInto.Text.Trim()!=textReferralNameFrom.Text.Trim()) {
				differentFields+=Lan.g(this,"Referral Name")+"\r\n";
			}
			if(textTitleInto.Text.Trim()!=textTitleFrom.Text.Trim()) {
				differentFields+=Lan.g(this,"Title")+"\r\n";
			}
			if(checkIsPersonInto.Checked!=checkIsPersonFrom.Checked) {
				differentFields+=Lan.g(this,"Is Person")+"\r\n";
			}
			if(checkIsDoctorInto.Checked!=checkIsDoctorFrom.Checked) {
				differentFields+=Lan.g(this,"Is Doctor")+"\r\n";
			}
			string warningMsg="";
			if(differentFields!="") {
				warningMsg+=Lan.g(this,"The following referral fields do not match")+": \r\n"+differentFields;
			}
			int countPatAttach=Referrals.CountReferralAttach(_referralNumFrom);
			warningMsg+=Lan.g(this,"The selected referrals may be different")+".  "+Lan.g(this,"This change is irreversible! The referral is attached to")+" "
				+countPatAttach+" "+Lan.g(this,"patients")+".  "+Lan.g(this,"Continue anyways?");
			if(MessageBox.Show(warningMsg,"",MessageBoxButtons.YesNo)==DialogResult.No) { 
				return;
			}
			if(!Referrals.MergeReferrals(_referralNumInto,_referralNumFrom)) {
				MsgBox.Show(this,"Referrals failed to merge.");
				return;
			}
			MsgBox.Show(this,"Referrals merged successfully.");
			string logText=Lan.g(this,"Referral Merge from")
				+" "+Referrals.GetNameLF(_referralNumFrom)+" "+Lan.g(this,"to")+" "+Referrals.GetNameLF(_referralNumInto)+"\r\n"
				+Lan.g(this,"Patients attached to this referral")+": "+countPatAttach.ToString();
			//Make log entry here not in parent form because we can merge multiple referrals at a time.
			SecurityLogs.MakeLogEntry(Permissions.ReferralMerge,0,logText);
			textReferralNameFrom.Text="";
			textTitleFrom.Text="";
			checkIsPersonFrom.Checked=false;
			checkIsDoctorFrom.Checked=false;
			CheckUIState();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}


	}
}