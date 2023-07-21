using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAdvertisingPostcardsSend:FormODBase {
		private PostcardManiaMetaData _postcardManiaMetaData;
		private List<PatientInfo> _listPatientInfo;

		public FormAdvertisingPostcardsSend() {
			_listPatientInfo=new List<PatientInfo>();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassPostcardSend_Load(object sender,EventArgs e) {
			menuAccountSetup.Add("Setup",ManageAccounts_Click);
			FillData();
			FillUI();
		}

		private void FillData() {
			_postcardManiaMetaData=new PostcardManiaMetaData();
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					_postcardManiaMetaData=AdvertisingPostcards.GetPostcardManiaMetaData();
				};
				progressOD.StartingMessage=Lan.g(this,"Getting Account Data")+"...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
			}
		}

		private void FillUI() {
			butUploadPatients.Enabled=!_postcardManiaMetaData.Accounts.IsNullOrEmpty();
			butViewAccount.Enabled=!_postcardManiaMetaData.Accounts.IsNullOrEmpty();
			comboPostcardAccount.Items.Clear();
			comboPostcardAccount.Items.AddList(_postcardManiaMetaData.Accounts,x=>x.AccountTitle);
			string guid=ClinicPrefs.GetPrefValue(PrefName.AdvertisingPostCardGuid,Clinics.ClinicNum);
			comboPostcardAccount.SelectedIndex=_postcardManiaMetaData.Accounts.FindIndex(x=>x.Guid==guid);
			textNumberOfRecipients.Text=_listPatientInfo.Count.ToString();
		}

		private void ManageAccounts_Click(object sender,EventArgs e) {
			using FormAdvertisingPostcardsSetup formMassPostcardSetup=new FormAdvertisingPostcardsSetup();
			formMassPostcardSetup.ShowDialog();
			FillData();
			FillUI();
		}

		private void ViewAccount_Click(object sender,EventArgs e) {
			string url="";
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					url=AdvertisingPostcards.GetSSO(
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Guid,
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Email);
				};
				progressOD.StartingMessage=Lan.g(this,"Getting Account Data")+"...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				return;
			}
			using FormWebView fweb=new FormWebView(url);
			fweb.IsUrlSingleUse=true;
			fweb.ShowDialog();
		}

		private void ViewAccount() {
			string url="";
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					url=AdvertisingPostcards.GetSSO(
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Guid,
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Email);
				};
				progressOD.StartingMessage=Lan.g(this,"Getting Account Data")+"...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				return;
			}
			using FormWebView fweb=new FormWebView(url);
			fweb.Title=Lan.g(this,"Advertising - Postcards");
			fweb.IsUrlSingleUse=true;
			fweb.ShowDialog();
		}

		private void butAccountManage_Click(object sender,EventArgs e) {
			using FormAdvertisingPostcardsSetup formMassPostcardSetup=new FormAdvertisingPostcardsSetup();
			formMassPostcardSetup.ShowDialog();
			FillData();
			FillUI();
		}

		private void butSelectPatients_Click(object sender,EventArgs e) {
			using FormAdvertisingPatientList formMassPostcardList=new FormAdvertisingPatientList(AdvertisingType.Postcards);
			formMassPostcardList.ListPatientInfos=_listPatientInfo;
			formMassPostcardList.ShowDialog();
			if(formMassPostcardList.DialogResult==DialogResult.OK) {
				_listPatientInfo=formMassPostcardList.ListPatientInfos;
				textNumberOfRecipients.Text=_listPatientInfo.Count.ToString();
			}
		}

		private void butViewAccount_Click(object sender,EventArgs e) {
			if(((PostcardManiaAccountData)comboPostcardAccount.SelectedItem)!=null) {
				SecurityLogs.MakeLogEntry(Permissions.Advertising,0,$"Navigated to Advertising - Postcards web framework for {((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).AccountTitle}");
			}	
			ViewAccount();
		}

		private void butUploadPatients_Click(object sender,EventArgs e) {
			if(textListName.Text=="") {
				MsgBox.Show("Empty 'List name' not allowed.");
				return;
			}
			if(_listPatientInfo.Count==0 && !MsgBox.Show(MsgBoxButtons.YesNo,"No recipients selected, continue to upload an empty list?")) {
				return;
			}
			if(((PostcardManiaAccountData)comboPostcardAccount.SelectedItem)!=null) {
				SecurityLogs.MakeLogEntry(Permissions.Advertising,0,$"Uploaded list to Advertising - Postcards web framework for {((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).AccountTitle}");
			}	
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					AdvertisingPostcards.UploadPatients(
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Guid,
						((PostcardManiaAccountData)comboPostcardAccount.SelectedItem).Email,
						textListName.Text,_listPatientInfo);
				};
				progressOD.StartingMessage=Lan.g(this,"Uploading Patient Data")+"...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				return;
			}
			ViewAccount();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}