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
    private bool IsPatient;
		///<summary></summary>
		public Referral RefCur;
		private List<SheetDef> SlipList;
		///<summary>Tracks the list of clinics that the referral is associated with.</summary>
		private List<ReferralClinicLink> _listReferralClinicLinks;

		///<summary></summary>
		public FormReferralEdit(Referral refCur){
			InitializeComponent();
			InitializeLayoutManager();
			RefCur=refCur;
			if(refCur.PatNum>0){
				IsPatient=true;
			}
			Lan.F(this);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				groupSSN.Text=Lan.g(this,"CDA Number");
				radioSSN.Visible=false;
				radioTIN.Visible=false;
			}
		}

		private void FormReferralEdit_Load(object sender, System.EventArgs e) {
			if(Plugins.HookMethod(this,"FormReferralEdit.Load",RefCur,IsNew)) {
				return;
			}
			listSpecialty.Items.Clear();
			Def[] specDefs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
			for(int i=0;i<specDefs.Length;i++) {
				listSpecialty.Items.Add(Lan.g("enumDentalSpecialty",specDefs[i].ItemName));
			}
			if(IsPatient){
				if(IsNew){
					Text=Lan.g(this,"Add Referral"); 
					Family FamCur=Patients.GetFamily(RefCur.PatNum);
					Patient PatCur=FamCur.GetPatient(RefCur.PatNum);
					RefCur.Address=PatCur.Address;
					RefCur.Address2=PatCur.Address2;	
					RefCur.City=PatCur.City;	
					RefCur.EMail=PatCur.Email;	
					RefCur.FName=PatCur.FName;	
					RefCur.LName=PatCur.LName;	
					RefCur.MName=PatCur.MiddleI;	
					//RefCur.PatNum=Patients.Cur.PatNum;//already handled
					RefCur.SSN=PatCur.SSN;
					RefCur.Telephone=TelephoneNumbers.FormatNumbersExactTen(PatCur.HmPhone);
					if(PatCur.WkPhone==""){
						RefCur.Phone2=PatCur.WirelessPhone;
					}
					else{
						RefCur.Phone2=PatCur.WkPhone;
					}
					RefCur.ST=PatCur.State;	
					RefCur.Zip=PatCur.Zip;
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
					RefCur=new Referral();
					RefCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,"General");
				}
				Def specDefCur=Defs.GetDef(DefCat.ProviderSpecialties,RefCur.Specialty);
				if(specDefCur!=null) {
					for(int i=0;i<specDefs.Length;i++) {
						if(i==0 || specDefs[i].ItemName==specDefCur.ItemName) {//default to the first specialty in the list
							listSpecialty.SelectedIndex=i;
						}
					}
				}
				textLName.Select();
			}
			checkIsDoctor.Checked=RefCur.IsDoctor;
			checkNotPerson.Checked=RefCur.NotPerson;
			checkHidden.Checked=RefCur.IsHidden;
			checkEmailTrustDirect.Checked=RefCur.IsTrustedDirect;
			textLName.Text=RefCur.LName;
			textFName.Text=RefCur.FName;
			textMName.Text=RefCur.MName;
			textTitle.Text=RefCur.Title;
			textAddress.Text=RefCur.Address;
			textAddress2.Text=RefCur.Address2;
			textCity.Text=RefCur.City;         
			textST.Text=RefCur.ST;
			textZip.Text=RefCur.Zip;
			string phone=RefCur.Telephone;
			if(phone!=null && phone.Length==10){
				textPhone1.Text=phone.Substring(0,3);
				textPhone2.Text=phone.Substring(3,3);
				textPhone3.Text=phone.Substring(6);
			}
			textSSN.Text=RefCur.SSN;
			if(RefCur.UsingTIN){ 
				radioTIN.Checked=true;
			} 
			else{
				radioSSN.Checked=true;
			}
			textNationalProvID.Text=RefCur.NationalProvID;
			textOtherPhone.Text=RefCur.Phone2;  
			textEmail.Text=RefCur.EMail; 
			textBusinessName.Text=RefCur.BusinessName;
			textNotes.Text=RefCur.Note;
			textDisplayNote.Text=RefCur.DisplayNote;
			_listReferralClinicLinks=ReferralClinicLinks.GetAllForReferral(RefCur.ReferralNum);
			if(_listReferralClinicLinks.Count==0) {
				comboClinicPicker.SelectedClinicNum=0;
			}
			else {
				comboClinicPicker.ListSelectedClinicNums=_listReferralClinicLinks.Select(x=>x.ClinicNum).ToList();
			}
			//Patients using:
			string[] patsTo=RefAttaches.GetPats(RefCur.ReferralNum,ReferralType.RefTo);
			string[] patsFrom=RefAttaches.GetPats(RefCur.ReferralNum,ReferralType.RefFrom);
			comboPatientsTo.Items.Clear();
			comboPatientsFrom.Items.Clear();
			Dictionary<string,int> dictPatRefCount=new Dictionary<string,int>();
			for(int i=0;i<patsTo.Length;i++) {
				if(dictPatRefCount.ContainsKey(patsTo[i])) {
					dictPatRefCount[patsTo[i]]+=1;
				}
				else {
					dictPatRefCount[patsTo[i]]=1;
				}
			}
			foreach(KeyValuePair<string,int> pat in dictPatRefCount) {
				//Add each patient referral once with the count of how many times they've been referred.
				comboPatientsTo.Items.Add(pat.Key+" ("+pat.Value+")");
			}
			textPatientsNumTo.Text=dictPatRefCount.Count.ToString();
			dictPatRefCount=new Dictionary<string,int>();
			for(int i=0;i<patsFrom.Length;i++) {
				if(dictPatRefCount.ContainsKey(patsFrom[i])) {
					dictPatRefCount[patsFrom[i]]+=1;
				}
				else {
					dictPatRefCount[patsFrom[i]]=1;
				}
			}
			foreach(KeyValuePair<string,int> pat in dictPatRefCount) {
				comboPatientsFrom.Items.Add(pat.Key+" ("+pat.Value+")");
			}
			textPatientsNumFrom.Text=dictPatRefCount.Count.ToString();
			if(patsTo.Length>0){
				comboPatientsTo.SelectedIndex=0;
			}
			if(patsFrom.Length>0){
				comboPatientsFrom.SelectedIndex=0;
			}
			comboSlip.Items.Add(Lan.g(this,"Default"));
			comboSlip.SelectedIndex=0;
			SlipList=SheetDefs.GetCustomForType(SheetTypeEnum.ReferralSlip);
			for(int i=0;i<SlipList.Count;i++){
				comboSlip.Items.Add(SlipList[i].Description);
				if(RefCur.Slip==SlipList[i].SheetDefNum){
					comboSlip.SelectedIndex=i+1;
				}
			}
			if(!IsNew && !Security.IsAuthorized(Permissions.ReferralEdit,true)) {//User may have add permissions but may not have access to edit.
				this.Text+=" - "+Lan.g(this,"Read Only");
			}
			checkIsPreferred.Checked=RefCur.IsPreferred;
		}

		private void radioSSN_Click(object sender, System.EventArgs e) {
			RefCur.UsingTIN=false;
		}

		private void radioTIN_Click(object sender, System.EventArgs e) {
			RefCur.UsingTIN=true;
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
				Referrals.Delete(RefCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			string secLogText="Referral '"+RefCur.LName+", "+RefCur.FName+"' deleted";
			SecurityLogs.MakeLogEntry(Permissions.ReferralEdit,RefCur.PatNum,secLogText); //RefCur.PatNum should be 0 if not a patient.
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
			if(listSpecialty.SelectedIndex==-1 && !IsPatient) {
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
			RefCur.IsHidden=checkHidden.Checked;
			RefCur.NotPerson=checkNotPerson.Checked;
			RefCur.IsTrustedDirect=checkEmailTrustDirect.Checked;
			RefCur.LName=textLName.Text;
			RefCur.FName=textFName.Text;
			RefCur.MName=textMName.Text;
			RefCur.Title=textTitle.Text;
      RefCur.Address=textAddress.Text;
      RefCur.Address2=textAddress2.Text;
      RefCur.City=textCity.Text;
			RefCur.ST=textST.Text;
      RefCur.Zip=textZip.Text;
			RefCur.Telephone=phone;
      RefCur.Phone2=textOtherPhone.Text;    
			RefCur.SSN=textSSN.Text;
			RefCur.NationalProvID=textNationalProvID.Text;
      RefCur.EMail=textEmail.Text;
			RefCur.BusinessName=textBusinessName.Text;
      RefCur.Note=textNotes.Text; 
			RefCur.DisplayNote=textDisplayNote.Text;
			RefCur.Slip=0;
			RefCur.IsPreferred=checkIsPreferred.Checked;
			if(comboSlip.SelectedIndex>0){
				RefCur.Slip=SlipList[comboSlip.SelectedIndex-1].SheetDefNum;
			}
			//RefCur.UsingTIN already taken care of
      if(!IsPatient){
				RefCur.IsDoctor=checkIsDoctor.Checked;
				RefCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,listSpecialty.SelectedItem.ToString());
      }
			if(IsNew){
				if(Referrals.GetExists(x => x.LName+x.FName==RefCur.LName+RefCur.FName)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Referral of same name exists. Add anyway?")) {
						DialogResult=DialogResult.Cancel;
						return;
					}
				}
				string secLogText="Referral '"+RefCur.LName+", "+RefCur.FName+"' created";
				SecurityLogs.MakeLogEntry(Permissions.ReferralAdd,RefCur.PatNum,secLogText);
				Referrals.Insert(RefCur);
				Signalods.SetInvalid(InvalidType.Referral);//Make sure other instances have the new referral in their cache.
				ReferralClinicLinks.InsertClinicLinksForReferral(RefCur.ReferralNum,comboClinicPicker.ListSelectedClinicNums,false);
			}
			else{
				string secLogText="Referral '"+RefCur.LName+", "+RefCur.FName+"' edited";
				SecurityLogs.MakeLogEntry(Permissions.ReferralEdit,RefCur.PatNum,secLogText);
				Referrals.Update(RefCur);
				ReferralClinicLinks.InsertClinicLinksForReferral(RefCur.ReferralNum,comboClinicPicker.ListSelectedClinicNums,true);
			}
			Referrals.RefreshCache();//We might want to enhance this to call DataValid.SetInvalid(InvalidType.Referral); eventually.
			//MessageBox.Show(RefCur.ReferralNum.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
		

		

		


	}
}
