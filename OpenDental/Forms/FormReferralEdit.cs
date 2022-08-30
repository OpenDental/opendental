using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormReferralEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		private bool _isPatient;
		///<summary></summary>
		public Referral ReferralCur;
		private List<SheetDef> _listSheetDefs;
		///<summary>Tracks the list of clinics that the referral is associated with.</summary>
		private List<ReferralClinicLink> _listReferralClinicLinks;

		///<summary></summary>
		public FormReferralEdit(Referral referral){
			InitializeComponent();
			InitializeLayoutManager();
			ReferralCur=referral;
			if(referral.PatNum>0){
				_isPatient=true;
			}
			Lan.F(this);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				groupSSN.Text=Lan.g(this,"CDA Number");
				radioSSN.Visible=false;
				radioTIN.Visible=false;
			}
		}

		private void FormReferralEdit_Load(object sender, System.EventArgs e) {
			if(Plugins.HookMethod(this,"FormReferralEdit.Load",ReferralCur,IsNew)) {
				return;
			}
			listSpecialty.Items.Clear();
			Def[] defArraySpecs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
			for(int i=0;i<defArraySpecs.Length;i++) {
				listSpecialty.Items.Add(Lan.g("enumDentalSpecialty",defArraySpecs[i].ItemName));
			}
			if(_isPatient){
				if(IsNew){
					Text=Lan.g(this,"Add Referral"); 
					Family family=Patients.GetFamily(ReferralCur.PatNum);
					Patient patient=family.GetPatient(ReferralCur.PatNum);
					ReferralCur.Address=patient.Address;
					ReferralCur.Address2=patient.Address2;	
					ReferralCur.City=patient.City;	
					ReferralCur.EMail=patient.Email;	
					ReferralCur.FName=patient.FName;	
					ReferralCur.LName=patient.LName;	
					ReferralCur.MName=patient.MiddleI;	
					//RefCur.PatNum=Patients.Cur.PatNum;//already handled
					ReferralCur.SSN=patient.SSN;
					ReferralCur.Telephone=TelephoneNumbers.FormatNumbersExactTen(patient.HmPhone);
					if(patient.WkPhone==""){
						ReferralCur.Phone2=patient.WirelessPhone;
					}
					else{
						ReferralCur.Phone2=patient.WkPhone;
					}
					ReferralCur.ST=patient.State;	
					ReferralCur.Zip=patient.Zip;
				}
				labelPatient.Visible=true;
				textLName.ReadOnly=true;
				textFName.ReadOnly=true;
				textMName.ReadOnly=true;
				textTitle.ReadOnly=true;
				textAddress.ReadOnly=true;
				textAddress2.ReadOnly=true;
				textCity.ReadOnly=true;         
				textST.ReadOnly=true;
				textZip.ReadOnly=true;
				checkNotPerson.Enabled=false;
				textPhone1.ReadOnly=true;
				textPhone2.ReadOnly=true;
				textPhone3.ReadOnly=true;
				textSSN.ReadOnly=true;				
				radioTIN.Enabled=false;
				textEmail.ReadOnly=true;
				textBusinessName.ReadOnly=true;
				listSpecialty.Enabled=false;
				listSpecialty.SelectedIndex=-1;
				checkIsDoctor.Enabled=false;
				textNotes.Select();
			}
			else{//non patient
				if(IsNew){
					this.Text=Lan.g(this,"Add Referral"); 
					ReferralCur=new Referral();
					ReferralCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,"General");
				}
				Def specDefCur=Defs.GetDef(DefCat.ProviderSpecialties,ReferralCur.Specialty);
				if(specDefCur!=null) {
					for(int i=0;i<defArraySpecs.Length;i++) {
						if(i==0 || defArraySpecs[i].ItemName==specDefCur.ItemName) {//default to the first specialty in the list
							listSpecialty.SelectedIndex=i;
						}
					}
				}
				textLName.Select();
			}
			checkIsDoctor.Checked=ReferralCur.IsDoctor;
			checkNotPerson.Checked=ReferralCur.NotPerson;
			checkHidden.Checked=ReferralCur.IsHidden;
			checkEmailTrustDirect.Checked=ReferralCur.IsTrustedDirect;
			textLName.Text=ReferralCur.LName;
			textFName.Text=ReferralCur.FName;
			textMName.Text=ReferralCur.MName;
			textTitle.Text=ReferralCur.Title;
			textAddress.Text=ReferralCur.Address;
			textAddress2.Text=ReferralCur.Address2;
			textCity.Text=ReferralCur.City;         
			textST.Text=ReferralCur.ST;
			textZip.Text=ReferralCur.Zip;
			string phone=ReferralCur.Telephone;
			if(phone!=null && phone.Length==10){
				textPhone1.Text=phone.Substring(0,3);
				textPhone2.Text=phone.Substring(3,3);
				textPhone3.Text=phone.Substring(6);
			}
			textSSN.Text=ReferralCur.SSN;
			if(ReferralCur.UsingTIN){ 
				radioTIN.Checked=true;
			} 
			else{
				radioSSN.Checked=true;
			}
			textNationalProvID.Text=ReferralCur.NationalProvID;
			textOtherPhone.Text=ReferralCur.Phone2;  
			textEmail.Text=ReferralCur.EMail; 
			textBusinessName.Text=ReferralCur.BusinessName;
			textNotes.Text=ReferralCur.Note;
			textDisplayNote.Text=ReferralCur.DisplayNote;
			_listReferralClinicLinks=ReferralClinicLinks.GetAllForReferral(ReferralCur.ReferralNum);
			if(_listReferralClinicLinks.Count==0) {
				comboClinicPicker.SelectedClinicNum=0;
			}
			else {
				comboClinicPicker.ListSelectedClinicNums=_listReferralClinicLinks.Select(x=>x.ClinicNum).ToList();
			}
			//Patients using:
			List<string> listPatsTo=RefAttaches.GetPats(ReferralCur.ReferralNum,ReferralType.RefTo);
			List<string> listPatsFrom=RefAttaches.GetPats(ReferralCur.ReferralNum,ReferralType.RefFrom);
			comboPatientsTo.Items.Clear();
			comboPatientsFrom.Items.Clear();
			List<string> listNamesUnique=listPatsTo.Distinct().ToList();
			for(int i = 0;i<listNamesUnique.Count;i++) {
				int count=listPatsTo.Count(x => x==listNamesUnique[i]);
				//Add each patient referral once with the count of how many times they've been referred.
				if(count==1) {//if unique
					comboPatientsTo.Items.Add(listNamesUnique[i]);
				}
				else {//multiple
					comboPatientsTo.Items.Add(listNamesUnique[i]+" ("+count.ToString()+")");
				}
			}
			textPatientsNumTo.Text=listNamesUnique.Count.ToString();
			listNamesUnique=listPatsFrom.Distinct().ToList();
			for(int i=0;i<listNamesUnique.Count;i++) {
				int count=listPatsFrom.Count(x => x==listNamesUnique[i]);
				if(count==1) {
					comboPatientsFrom.Items.Add(listNamesUnique[i]);
				}
				else {
					comboPatientsFrom.Items.Add(listNamesUnique[i]+" ("+count.ToString()+")");
				}
			}
			textPatientsNumFrom.Text=listNamesUnique.Count.ToString();
			if(listPatsTo.Count>0){
				comboPatientsTo.SelectedIndex=0;
			}
			if(listPatsFrom.Count>0){
				comboPatientsFrom.SelectedIndex=0;
			}
			comboSlip.Items.Add(Lan.g(this,"Default"));
			comboSlip.SelectedIndex=0;
			_listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ReferralSlip);
			for(int i=0;i<_listSheetDefs.Count;i++){
				comboSlip.Items.Add(_listSheetDefs[i].Description);
				if(ReferralCur.Slip==_listSheetDefs[i].SheetDefNum){
					comboSlip.SelectedIndex=i+1;
				}
			}
			if(!IsNew && !Security.IsAuthorized(Permissions.ReferralEdit,true)) {//User may have add permissions but may not have access to edit.
				this.Text+=" - "+Lan.g(this,"Read Only");
			}
			checkIsPreferred.Checked=ReferralCur.IsPreferred;
		}

		private void radioSSN_Click(object sender, System.EventArgs e) {
			ReferralCur.UsingTIN=false;
		}

		private void radioTIN_Click(object sender, System.EventArgs e) {
			ReferralCur.UsingTIN=true;
		}

		private void butNone_Click(object sender, System.EventArgs e) {
			listSpecialty.SelectedIndex=-1;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Security.IsAuthorized(Permissions.ReferralEdit)) {
				return;
			}	
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try {
				Referrals.Delete(ReferralCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' deleted";
			SecurityLogs.MakeLogEntry(Permissions.ReferralEdit,ReferralCur.PatNum,secLogText); //ReferralCur.PatNum should be 0 if not a patient.
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!IsNew && !Security.IsAuthorized(Permissions.ReferralEdit)) {//User may have add permissions but may not have access to edit.
				return;
			}
			string phone=textPhone1.Text+textPhone2.Text+textPhone3.Text;
			if(phone.Length > 0 && phone.Length < 10){
				MessageBox.Show(Lan.g(this,"Invalid phone"));
				return;
			}
			if(textLName.Text=="") {
				MsgBox.Show(this,"Please enter a last name.");
				return;
			}
			if(listSpecialty.SelectedIndex==-1 && !_isPatient) {
				MsgBox.Show(this,"Please select a specialty.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(checkEmailTrustDirect.Checked && !EmailMessages.TryAddTrustDirect(textEmail.Text)) {
				string trustEmailErrorMessage=Lan.g("Referral","You elected to trust this email address for Direct messaging.")
					+"  "+Lan.g("Referral","Adding trust for the address failed, because we were unable to locate the public certificate for the address.")
					+"  "+Lan.g("Referral","Check that the email address is correctly typed and is a valid Direct messaging address, then try again.")
					+"  "+Lan.g("Referral","Otherwise, uncheck the E-mail Trust for Direct checkbox before clicking OK or press Cancel if no other changes were made.");
				MessageBox.Show(trustEmailErrorMessage);
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			ReferralCur.IsHidden=checkHidden.Checked;
			ReferralCur.NotPerson=checkNotPerson.Checked;
			ReferralCur.IsTrustedDirect=checkEmailTrustDirect.Checked;
			ReferralCur.LName=textLName.Text;
			ReferralCur.FName=textFName.Text;
			ReferralCur.MName=textMName.Text;
			ReferralCur.Title=textTitle.Text;
			ReferralCur.Address=textAddress.Text;
			ReferralCur.Address2=textAddress2.Text;
			ReferralCur.City=textCity.Text;
			ReferralCur.ST=textST.Text;
			ReferralCur.Zip=textZip.Text;
			ReferralCur.Telephone=phone;
			ReferralCur.Phone2=textOtherPhone.Text;    
			ReferralCur.SSN=textSSN.Text;
			ReferralCur.NationalProvID=textNationalProvID.Text;
			ReferralCur.EMail=textEmail.Text;
			ReferralCur.BusinessName=textBusinessName.Text;
			ReferralCur.Note=textNotes.Text; 
			ReferralCur.DisplayNote=textDisplayNote.Text;
			ReferralCur.Slip=0;
			ReferralCur.IsPreferred=checkIsPreferred.Checked;
			if(comboSlip.SelectedIndex>0){
				ReferralCur.Slip=_listSheetDefs[comboSlip.SelectedIndex-1].SheetDefNum;
			}
			//ReferralCur.UsingTIN already taken care of
			if(!_isPatient){
				ReferralCur.IsDoctor=checkIsDoctor.Checked;
				ReferralCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,listSpecialty.SelectedItem.ToString());
			}
			if(IsNew){
				if(Referrals.GetExists(x => x.LName+x.FName==ReferralCur.LName+ReferralCur.FName)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Referral of same name exists. Add anyway?")) {
						DialogResult=DialogResult.Cancel;
						return;
					}
				}
				string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' created";
				SecurityLogs.MakeLogEntry(Permissions.ReferralAdd,ReferralCur.PatNum,secLogText);
				Referrals.Insert(ReferralCur);
				Signalods.SetInvalid(InvalidType.Referral);//Make sure other instances have the new referral in their cache.
				ReferralClinicLinks.InsertClinicLinksForReferral(ReferralCur.ReferralNum,comboClinicPicker.ListSelectedClinicNums,false);
			}
			else{
				string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' edited";
				SecurityLogs.MakeLogEntry(Permissions.ReferralEdit,ReferralCur.PatNum,secLogText);
				Referrals.Update(ReferralCur);
				ReferralClinicLinks.InsertClinicLinksForReferral(ReferralCur.ReferralNum,comboClinicPicker.ListSelectedClinicNums,true);
			}
			Referrals.RefreshCache();//We might want to enhance this to call DataValid.SetInvalid(InvalidType.Referral); eventually.
			//MessageBox.Show(ReferralCur.ReferralNum.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
		

		

		


	}
}
