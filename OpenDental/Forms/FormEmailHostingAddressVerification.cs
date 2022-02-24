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
		private ConcurrentDictionary<long,Identities> _dictIdentities=new ConcurrentDictionary<long,Identities>();

		public FormEmailHostingAddressVerification() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Gets the current list of email addresses that are verified with EmailHosting to be used to send emails.</summary>
		public List<IdentityResource> GetVerifiedAddresses(long clinicNum) {
			if(!_dictIdentities.TryGetValue(clinicNum,out Identities identities)) {
				return new List<IdentityResource>();
			}
			return identities.ListIdentities;
		}

		private void FormEmailHostingAddressVerification_Load(object sender,EventArgs e) {
			UpdateClinicUi();
		}

		private void FormEmailAddressVerification_Shown(object sender,EventArgs e) {
			//Load identities for all available clinics.
			Refresh(comboBoxClinicPicker1.ListClinics.Select(x => x.ClinicNum).ToArray());
		}

		///<summary>Loads verified Identities from the EmailHosting api, then fills the grid for the selected clinic.  Tracks any errors that occur 
		///during load.</summary>
		private void LoadIdentities(bool doFillGrid,params long[] arrClinicNums) {
			string emailVerificationLoading="email_verifications_loading";
			foreach(long clinicNum in arrClinicNums) {
				ODThread thread=new ODThread(o => {					
					if(!Clinics.IsMassEmailSignedUp(clinicNum) && !Clinics.IsSecureEmailSignedUp(clinicNum)) {
						return;
					}
					IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
					GetIdentitiesResponse response=api.GetIdentities(new GetIdentitiesRequest());
					_dictIdentities[clinicNum]=new Identities {
						ListIdentities=response.ListIdentityResources,
					};
				});
				thread.GroupName=emailVerificationLoading;
				thread.AddExceptionHandler(ex => {
					string desc=Clinics.GetAbbr(clinicNum);
					if(clinicNum==0) {
						desc=comboBoxClinicPicker1.HqDescription;
					}
					_dictIdentities[clinicNum]=new Identities {
						FriendlyMessage=Lan.g(this,"An error occurred loading email verifications for clinic: ")+desc,
						Exception=ex,
					};
				});
				thread.AddExitHandler(o => {
					if(!doFillGrid) {
						return;
					} 
					this.Invoke(() => {
						if(comboBoxClinicPicker1.SelectedClinicNum==clinicNum) {
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
				gridMain.ListGridColumns.Clear();
				GridColumn col;
				col=new GridColumn("Email Address",250);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn("Verification Status",250);
				gridMain.ListGridColumns.Add(col);
				gridMain.ListGridRows.Clear();
				GridRow row;
				if(!_dictIdentities.TryGetValue(comboBoxClinicPicker1.SelectedClinicNum,out Identities identities) || !string.IsNullOrWhiteSpace(identities.FriendlyMessage)) {
					return;
				}
				foreach(IdentityResource emailAddress in identities.ListIdentities) {
					row=new GridRow();
					row.Cells.Add(emailAddress.Identity);
					row.Cells.Add(emailAddress.VerificationStatus.GetDescription());
					row.Tag=emailAddress;
					gridMain.ListGridRows.Add(row);
				}
			}
			finally {
				gridMain.EndUpdate();
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			//Load identities for this one clinic.
			Refresh(comboBoxClinicPicker1.SelectedClinicNum);
		}

		private void Refresh(params long[] arrClinicNums) {
			ShowProgress(() => {
				LoadIdentities(true,arrClinicNums);
			});
			string error=string.Join("\n",_dictIdentities.Select(x => x.Value.FriendlyMessage??""))+"\n\n"
				+string.Join("\n",_dictIdentities.Where(x => x.Value.Exception!=null).Select(x => MiscUtils.GetExceptionText(x.Value.Exception)));
			if(!string.IsNullOrWhiteSpace(error)) {
				ShowError(error);
			}
		}

		private void AddIdentity(string emailAddress,long clinicNum) {
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			api.CreateEmailAddressIdentity(new CreateEmailAddressIdentityRequest {
				EmailAddress=new EmailAddressResource {
					Address=emailAddress,
				},
			});
		}

		private void DeleteIdentity(string emailAddress,long clinicNum) {			
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			api.DeleteIdentity(new DeleteIdentityRequest {
				Address=emailAddress,
			});
		}

		private void UpdateClinicUi() {
			long clinicNum=comboBoxClinicPicker1.SelectedClinicNum;
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
				ClinicPrefs.Upsert(PrefName.EmailHostingUseNoReply,comboBoxClinicPicker1.SelectedClinicNum,POut.Bool(checkUseNoReply.Checked));
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEA=new FormEmailAddresses();
			formEA.IsSelectionMode=true;
			formEA.ShowDialog();
			if(formEA.DialogResult==DialogResult.OK) {
				long emailAddressNum=formEA.EmailAddressNum;
				long clinicNum=comboBoxClinicPicker1.SelectedClinicNum;
				EmailAddress emailAddress=EmailAddresses.GetOne(emailAddressNum);
				string error="";
				ShowProgress(() => {					
					if(emailAddress is null) {
						error=Lan.g(this,"Email Address not found.");
						return;
					}
					try {
						AddIdentity(emailAddress.EmailUsername,clinicNum);
						LoadIdentities(true,clinicNum);
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
			IdentityResource identity=gridMain.SelectedTag<IdentityResource>();
			if(identity is null) {
				return;
			}
			string emailAddress=identity.Identity;
			if(MessageBox.Show(this,Lan.g(this,"Are you sure you want to delete the email address verification for: ")+emailAddress+"?"
				,Lan.g(this,"Delete Verification?"),MessageBoxButtons.YesNo)==DialogResult.No) 
			{
				return;
			}
			long clinicNum=comboBoxClinicPicker1.SelectedClinicNum;
			string error="";
			ShowProgress(() => {
				try {
					DeleteIdentity(emailAddress,clinicNum);
					LoadIdentities(true,clinicNum);
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
			ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => {
					action();
			};
			progressOD.StartingMessage=Lan.g(this,"Loading");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
		}

		private void FormEmailHostingAddressVerification_FormClosing(object sender,FormClosingEventArgs e) {
			//Refresh all clinics on the way out.
			ShowProgress(() => LoadIdentities(false,comboBoxClinicPicker1.ListClinics.Select(x => x.ClinicNum).ToArray()));
		}

		private class Identities {
			public List<IdentityResource> ListIdentities;
			public string FriendlyMessage;
			public Exception Exception;
		}
	}
}