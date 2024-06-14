using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailHostingAddressVerification:FormODBase {
		private ConcurrentDictionary<long,Identities> _concurrentDictionaryIdentities=new ConcurrentDictionary<long,Identities>();

		public FormEmailHostingAddressVerification() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Gets the current list of email addresses that are verified with EmailHosting to be used to send emails.</summary>
		public List<IdentityResource> GetVerifiedAddresses(long clinicNum) {
			if(!_concurrentDictionaryIdentities.TryGetValue(clinicNum,out Identities identities)) {
				return new List<IdentityResource>();
			}
			return identities.ListIdentityResources;
		}

		private void FormEmailHostingAddressVerification_Load(object sender,EventArgs e) {
			UpdateClinicUi();
		}

		private void FormEmailAddressVerification_Shown(object sender,EventArgs e) {
			//Load identities for all available clinics.
			List<long> listClinicNums=comboBoxClinicPicker1.ListClinics.Select(x=>x.ClinicNum).ToList();
			if(!PrefC.HasClinicsEnabled) {
				listClinicNums.Add(0);
			}
			Refresh(listClinicNums.ToArray());
		}

		///<summary>Loads verified Identities from the EmailHosting api, then fills the grid for the selected clinic.  Tracks any errors that occur 
		///during load.</summary>
		private void LoadIdentities(bool doFillGrid,params long[] longArrayClinicNums) {
			List<long> listClinicNums=longArrayClinicNums.Distinct().ToList();
			string emailVerificationLoading="email_verifications_loading";
			for(int i=0;i<listClinicNums.Count;i++) {
				//Due to callback we MUST capture clinic num inside this loop
				long clinicNum=listClinicNums[i];
				ODThread thread=new ODThread(o => {					
					if(!Clinics.IsMassEmailSignedUp(clinicNum) && !Clinics.IsSecureEmailSignedUp(clinicNum)) {
						return;
					}
					IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(clinicNum);
					GetIdentitiesResponse getIdentitiesResponse=iAccountApi.GetIdentities(new GetIdentitiesRequest());
					_concurrentDictionaryIdentities[clinicNum]=new Identities();
					_concurrentDictionaryIdentities[clinicNum].ListIdentityResources=getIdentitiesResponse.ListIdentityResources;
				});
				thread.GroupName=emailVerificationLoading;
				thread.AddExceptionHandler(ex => {
					string desc=Clinics.GetAbbr(clinicNum);
					if(clinicNum==0) {
						desc=comboBoxClinicPicker1.HqDescription;
					}
					_concurrentDictionaryIdentities[clinicNum]=new Identities {
						FriendlyMessage=Lan.g(this,"An error occurred loading email verifications for clinic: ")+desc,
						ExceptionCur=ex,
					};
				});
				thread.AddExitHandler(o => {
					if(!doFillGrid) {
						return;
					} 
					this.Invoke(() => {
						if(comboBoxClinicPicker1.ClinicNumSelected==clinicNum) {
							FillGrid();
						}
					});
				});
				thread.Start();
			}
			ODThread.JoinThreadsByGroupName(Timeout.Infinite,emailVerificationLoading);
		}

		private void FillGrid() {
			try {
				gridMain.BeginUpdate();
				gridMain.Columns.Clear();
				GridColumn col;
				col=new GridColumn("Email Address",250);
				gridMain.Columns.Add(col);
				col=new GridColumn("Verification Status",250);
				gridMain.Columns.Add(col);
				gridMain.ListGridRows.Clear();
				GridRow row;
				if(!_concurrentDictionaryIdentities.TryGetValue(comboBoxClinicPicker1.ClinicNumSelected,out Identities identities) 
					|| !string.IsNullOrWhiteSpace(identities.FriendlyMessage)) 
				{
					return;
				}
				for(int i = 0;i<identities.ListIdentityResources.Count;i++) {
					IdentityResource identityResourceEmailAddress=identities.ListIdentityResources[i];
					row=new GridRow();
					row.Cells.Add(identityResourceEmailAddress.Identity);
					row.Cells.Add(identityResourceEmailAddress.VerificationStatus.GetDescription());
					row.Tag=identityResourceEmailAddress;
					gridMain.ListGridRows.Add(row);
				}
			}
			finally {
				gridMain.EndUpdate();
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			//Load identities for this one clinic.
			Refresh(comboBoxClinicPicker1.ClinicNumSelected);
		}

		private void Refresh(params long[] longArrayClinicNums) {
			ShowProgress(() => {
				LoadIdentities(doFillGrid:true,longArrayClinicNums);
			});
			string error=string.Join("\n",_concurrentDictionaryIdentities.Select(x => x.Value.FriendlyMessage??""))+"\n\n"
				+string.Join("\n",_concurrentDictionaryIdentities.Where(x => x.Value.ExceptionCur!=null).Select(x => MiscUtils.GetExceptionText(x.Value.ExceptionCur)));
			if(!string.IsNullOrWhiteSpace(error)) {
				ShowError(error);
			}
		}

		private void AddIdentity(string emailAddress,long clinicNum) {
			IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(clinicNum);
			CreateEmailAddressIdentityRequest createEmailAddressIdentityRequest = new CreateEmailAddressIdentityRequest();
			createEmailAddressIdentityRequest.EmailAddress=new EmailAddressResource();
			createEmailAddressIdentityRequest.EmailAddress.Address=emailAddress;
			iAccountApi.CreateEmailAddressIdentity(createEmailAddressIdentityRequest);
		}

		private void DeleteIdentity(string emailAddress,long clinicNum) {			
			IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(clinicNum);
			DeleteIdentityRequest deleteIdentityRequest = new DeleteIdentityRequest();
			deleteIdentityRequest.Address=emailAddress;
			iAccountApi.DeleteIdentity(deleteIdentityRequest);
		}

		private void UpdateClinicUi() {
			long clinicNum=comboBoxClinicPicker1.ClinicNumSelected;
			bool isActivated=Clinics.IsMassEmailSignedUp(clinicNum) || Clinics.IsSecureEmailSignedUp(clinicNum);
			butAdd.Enabled=isActivated;
			butDelete.Enabled=isActivated;
			butRefresh.Enabled=isActivated;
			labelNotActivated.Visible=!isActivated;
			//Defaults to practice preference if clinicNum=0 or not found.
			checkUseNoReply.Checked=ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,clinicNum);
			checkUseNoReply.Enabled=isActivated;
		}

		private void comboBoxClinicPicker1_SelectionChangeCommitted(object sender,EventArgs e) {
			UpdateClinicUi();
			FillGrid();
		}

		private void ShowError(string error) {
			MsgBox.Show(this,error);
		}

		private void checkUseNoReply_Click(object sender,EventArgs e) {
			if(comboBoxClinicPicker1.IsUnassignedSelected) {
				Prefs.UpdateBool(PrefName.EmailHostingUseNoReply,checkUseNoReply.Checked);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingUseNoReply,comboBoxClinicPicker1.ClinicNumSelected,POut.Bool(checkUseNoReply.Checked));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEmailAddresses=new FormEmailAddresses();
			formEmailAddresses.IsSelectionMode=true;
			formEmailAddresses.ShowDialog();
			if(formEmailAddresses.DialogResult==DialogResult.OK) {
				long emailAddressNum=formEmailAddresses.EmailAddressNum;
				long clinicNum=comboBoxClinicPicker1.ClinicNumSelected;
				EmailAddress emailAddress=EmailAddresses.GetOne(emailAddressNum);
				string error="";
				ShowProgress(() => {					
					if(emailAddress is null) {
						error=Lan.g(this,"Email Address not found.");
						return;
					}
					try {
						AddIdentity(emailAddress.EmailUsername,clinicNum);
						LoadIdentities(doFillGrid:true,clinicNum);
					}
					catch(Exception ex) {
						error=Lan.g(this,"An error occurred verifying the email address.  Please contact support.\n")+MiscUtils.GetExceptionText(ex);
					}
				});
				if(string.IsNullOrWhiteSpace(error)) {
					MessageBox.Show(this,Lan.g(this,"An email containing a link to complete the verification process has been sent to: ")+emailAddress.EmailUsername);
				}
				else {
					ShowError(error);
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IdentityResource identityResource=gridMain.SelectedTag<IdentityResource>();
			if(identityResource is null) {
				return;
			}
			string emailAddress=identityResource.Identity;
			if(MessageBox.Show(this,Lan.g(this,"Are you sure you want to delete the email address verification for: ")+emailAddress+"?"
				,Lan.g(this,"Delete Verification?"),MessageBoxButtons.YesNo)==DialogResult.No) 
			{
				return;
			}
			long clinicNum=comboBoxClinicPicker1.ClinicNumSelected;
			string error="";
			ShowProgress(() => {
				try {
					DeleteIdentity(emailAddress,clinicNum);
					LoadIdentities(doFillGrid:true,clinicNum);
				}
				catch(Exception ex) {					
					error=Lan.g(this,"An error occurred verifying the email address.  Please contact support.\n")+MiscUtils.GetExceptionText(ex);
				}
			});			
			if(!string.IsNullOrWhiteSpace(error)) {
				ShowError(error);
			}
		}

		private void ShowProgress(Action action) {
			ProgressWin progress=new ProgressWin();
				progress.ActionMain=() => {
					action();
			};
			progress.StartingMessage=Lan.g(this,"Loading");
			progress.ShowDialog();
			if(progress.IsCancelled){
				return;
			}
		}

		private void FormEmailHostingAddressVerification_FormClosing(object sender,FormClosingEventArgs e) {
			//Refresh all clinics on the way out.
			ShowProgress(() => LoadIdentities(doFillGrid:false,comboBoxClinicPicker1.ListClinics.Select(x => x.ClinicNum).ToArray()));
		}

		private class Identities {
			public List<IdentityResource> ListIdentityResources;
			public string FriendlyMessage;
			public Exception ExceptionCur;
		}
	}
}