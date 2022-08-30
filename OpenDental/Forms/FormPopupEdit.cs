using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Globalization;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormPopupEdit : FormODBase{
		public Popup PopupCur;
		public Popup PopupAudit;
		///<summary>The last edit date of the current popup.  Should be set before this form loads.</summary>
		public DateTime DateLastEdit;
		private Patient _patient;

		///<summary></summary>
		public FormPopupEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPopupEdit_Load(object sender,EventArgs e) {
			_patient=Patients.GetPat(PopupCur.PatNum);
			textPatient.Text=_patient.GetNameLF();
			if(PopupCur.IsNew) {//If popup is new User is the logged-in user and create date is now.
				butAudit.Visible=false;
				textUser.Text=Security.CurUser.UserName;
				textCreateDate.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
				if(!String.IsNullOrWhiteSpace(PrefC.GetString(PrefName.PopupsDisableTimeSpan))) { 
					textDateTimeDisabled.Text=DateTime.Now.Add(TimeSpan.Parse(PrefC.GetString(PrefName.PopupsDisableTimeSpan),CultureInfo.InvariantCulture)).ToString("MM/dd/yyyy HH:mm:ss");
				}
			}
			else {
				if(PopupCur.UserNum!=0) {//This check is so that any old popups without a user will still display correctly.
					//Display last user to edit PopupCur, or "Unknown(5)" if user not found.
					textUser.Text=Userods.GetUser(PopupCur.UserNum)?.UserName??(Lan.g(this,"Unknown")+$"({POut.Long(PopupCur.UserNum)})");
				}
				if(PopupAudit!=null) {//This checks if this window opened from FormPopupAudit
					textCreateDate.Text="";
					if(PopupAudit.DateTimeEntry.Year > 1880) {
						textCreateDate.Text=PopupAudit.DateTimeEntry.ToShortDateString()+" "+PopupAudit.DateTimeEntry.ToShortTimeString();//Sets the original creation date.
					}
					textEditDate.Text="";
					if(DateLastEdit.Year > 1880) {
						textEditDate.Text=DateLastEdit.ToShortDateString()+" "+DateLastEdit.ToShortTimeString();
					}
				}
				else {
					textCreateDate.Text="";
					if(PopupCur.DateTimeEntry.Year > 1880) {
						textCreateDate.Text=PopupCur.DateTimeEntry.ToShortDateString()+" "+PopupCur.DateTimeEntry.ToShortTimeString();//Sets the original creation date.
					}
					DateTime dateT=Popups.GetLastEditDateTimeForPopup(PopupCur.PopupNum);
					textEditDate.Text="";
					if(dateT.Year > 1880) {
						textEditDate.Text=dateT.ToShortDateString()+" "+dateT.ToShortTimeString();//Sets the Edit date to the entry date of the last popup change that was archived for this popup.
					}
					dateT=PopupCur.DateTimeDisabled;
					textDateTimeDisabled.Text="";
					if(dateT.Year > 1880) {
						textDateTimeDisabled.Text=dateT.ToString("MM/dd/yyyy HH:mm:ss"); 
					}
				}
			}
			comboPopupLevel.Items.Add(Lan.g("enumEnumPopupFamily",Enum.GetNames(typeof(EnumPopupLevel))[0]));//Patient
			comboPopupLevel.Items.Add(Lan.g("enumEnumPopupFamily",Enum.GetNames(typeof(EnumPopupLevel))[1]));//Family
			if(_patient.SuperFamily!=0 || PopupCur.PopupLevel==EnumPopupLevel.SuperFamily) {
				//Previously if a superfamily head was moved out to their own family the associated superfamily popups were incorrectly copied.
				//This would cause the comboPopupLevel selection logic below to error.
				comboPopupLevel.Items.Add(Lan.g("enumEnumPopupFamily",Enum.GetNames(typeof(EnumPopupLevel))[2]));//SuperFamily
			}
			comboPopupLevel.SelectedIndex=(int)PopupCur.PopupLevel;
			if(PopupCur.IsNew) {//Set default selected level to Patient for popups.
				//If no index is found nothing is set, this shouldn't happen.
				comboPopupLevel.SelectedIndex=comboPopupLevel.Items.GetAll<string>().IndexOf(Enum.GetNames(typeof(EnumPopupLevel))[0]);//Patient - E37468
				if(PrefC.IsODHQ) {//Use Family for HQ
					comboPopupLevel.SelectedIndex=comboPopupLevel.Items.GetAll<string>().IndexOf(Enum.GetNames(typeof(EnumPopupLevel))[1]);//Family
				}
			}
			//checkIsDisabled.Checked=PopupCur.IsDisabled;
			textDescription.Text=PopupCur.Description;
			if(!PopupCur.IsNew && PopupCur.UserNum != Security.CurUser.UserNum 
				&& !Security.IsAuthorized(Permissions.PopupEdit,true)) 
			{
				DisableAllExcept(butCancel,butAudit);
			}
			if(PopupCur.IsArchived) {
				DisableAllExcept(butCancel);
			}
			if(PopupCur.PopupNumArchive!=0) {
				butAudit.Visible=false;
			}
		}

		private void butAudit_Click(object sender,EventArgs e) {
			using FormPopupAudit FormPopupAudit=new FormPopupAudit();
			FormPopupAudit.PopupCur=PopupCur;
			FormPopupAudit.PatientCur=_patient;
			FormPopupAudit.ShowDialog();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(PopupCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			//don't ask user to make it go faster.
			PopupCur.IsArchived=true;
			PopupCur.PopupNumArchive=0;
			Popups.Update(PopupCur);//Runs an update to "archive" the popup, but allows it to be shown under the deleted section.
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter text first");
				return;
			}
			//If DateDisabled is populated and a valid date
			if(String.IsNullOrWhiteSpace(textDateTimeDisabled.Text)) {
				PopupCur.DateTimeDisabled=DateTime.MinValue;
			}
			else { 
				try {
					PopupCur.DateTimeDisabled=DateTime.Parse(textDateTimeDisabled.Text,CultureInfo.InvariantCulture);
				}
				catch {
					MsgBox.Show(this,"Date Time Disabled is invalid.");
					return;
				}
			}
			//PatNum cannot be set
			if(PopupCur.IsNew) {
				//PopCur date got set on load
				PopupCur.PopupLevel=(EnumPopupLevel)comboPopupLevel.SelectedIndex;
				//PopupCur.IsDisabled=checkIsDisabled.Checked;
				PopupCur.Description=textDescription.Text;
				PopupCur.UserNum=Security.CurUser.UserNum;
				Popups.Insert(PopupCur);
			}
			else {
				if(PopupCur.Description!=textDescription.Text) {//if user changed the description
					Popup popupArchive=PopupCur.Copy();
					popupArchive.IsArchived=true;
					popupArchive.PopupNumArchive=PopupCur.PopupNum;
					Popups.Insert(popupArchive);
					PopupCur.Description=textDescription.Text;
					PopupCur.UserNum=Security.CurUser.UserNum;
				}//No need to make an archive entry for changes to PopupLevel or IsDisabled so they get set on every OK Click.
				PopupCur.PopupLevel=(EnumPopupLevel)comboPopupLevel.SelectedIndex;
				//PopupCur.IsDisabled=checkIsDisabled.Checked;
				Popups.Update(PopupCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateTimeDisabled.Text=DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
		}
	}
}





















