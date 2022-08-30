using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClinicEdit : FormODBase {
		public Clinic ClinicCur;
		//private List<Provider> _listProv;
		///<summary>True if an HL7Def is enabled with the type HL7InternalType.MedLabv2_3, otherwise false.</summary>
		private bool _isMedLabHL7DefEnabled;
		private List<Def> _listDefsRegions;
		///<summary>DefLink.FKey is ClinicNum to link to defs representing specialties for that clinic.</summary>
		public List<DefLink> ListDefLinksSpecialties;
		///<summary></summary>
		public FormClinicEdit(Clinic clinic)
		{
			//
			// Required for Windows Form Designer support
			//
			ClinicCur=clinic;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClinicEdit_Load(object sender, System.EventArgs e) {
			checkIsMedicalOnly.Checked=ClinicCur.IsMedicalOnly;
			if(Programs.UsingEcwTightOrFullMode()) {
				checkIsMedicalOnly.Visible=false;
			}
			if(ClinicCur.ClinicNum!=0) {
				textClinicNum.Text=ClinicCur.ClinicNum.ToString();
			}
			textExternalID.Text=ClinicCur.ExternalID.ToString();
			textDescription.Text=ClinicCur.Description;
			textClinicAbbr.Text=ClinicCur.Abbr;
			textPhone.Text=TelephoneNumbers.ReFormat(ClinicCur.Phone);
			textFax.Text=TelephoneNumbers.ReFormat(ClinicCur.Fax);
			checkUseBillingAddressOnClaims.Checked=ClinicCur.UseBillAddrOnClaims;
			checkExcludeFromInsVerifyList.Checked=ClinicCur.IsInsVerifyExcluded;
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				checkProcCodeRequired.Enabled=true;
				checkProcCodeRequired.Checked=(ClinicCur.IsNew || ClinicCur.HasProcOnRx);
			}
			checkHidden.Checked=ClinicCur.IsHidden;
			textAddress.Text=ClinicCur.Address;
			textAddress2.Text=ClinicCur.Address2;
			textCity.Text=ClinicCur.City;
			textState.Text=ClinicCur.State;
			textZip.Text=ClinicCur.Zip;
			textBillingAddress.Text=ClinicCur.BillingAddress;
			textBillingAddress2.Text=ClinicCur.BillingAddress2;
			textBillingCity.Text=ClinicCur.BillingCity;
			textBillingST.Text=ClinicCur.BillingState;
			textBillingZip.Text=ClinicCur.BillingZip;
			textPayToAddress.Text=ClinicCur.PayToAddress;
			textPayToAddress2.Text=ClinicCur.PayToAddress2;
			textPayToCity.Text=ClinicCur.PayToCity;
			textPayToST.Text=ClinicCur.PayToState;
			textPayToZip.Text=ClinicCur.PayToZip;
			textBankNumber.Text=ClinicCur.BankNumber;
			textSchedRules.Text=ClinicCur.SchedNote;
			comboPlaceService.Items.Clear();
			comboPlaceService.Items.AddList<string>(Enum.GetNames(typeof(PlaceOfService)));
			comboPlaceService.SelectedIndex=(int)ClinicCur.DefaultPlaceService;
			comboInsBillingProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(ClinicCur.ClinicNum));
			if(ClinicCur.InsBillingProv==0){
				radioInsBillingProvDefault.Checked=true;//default=0
			}
			else if(ClinicCur.InsBillingProv==-1){
				radioInsBillingProvTreat.Checked=true;//treat=-1
			}
			else{
				radioInsBillingProvSpecific.Checked=true;//specific=any number >0. Foreign key to ProvNum
				comboInsBillingProv.SetSelectedProvNum(ClinicCur.InsBillingProv);
			}
			comboDefaultProvider.Items.AddProvsAbbr(Providers.GetProvsForClinic(ClinicCur.ClinicNum));
			if(ClinicCur.DefaultProv>0){
				comboDefaultProvider.SetSelectedProvNum(ClinicCur.DefaultProv);
			}
			ListDefLinksSpecialties=DefLinks.GetDefLinksForClinicSpecialties(ClinicCur.ClinicNum);
			FillSpecialty();
			comboRegion.Items.Clear();
			comboRegion.Items.Add(Lan.g(this,"None"));
			comboRegion.SelectedIndex=0;
			_listDefsRegions=Defs.GetDefsForCategory(DefCat.Regions,true);
			for(int i=0;i<_listDefsRegions.Count;i++) {
				comboRegion.Items.Add(_listDefsRegions[i].ItemName);
				if(_listDefsRegions[i].DefNum==ClinicCur.Region) {
					comboRegion.SelectedIndex=i+1;
				}
			}
			//Pre-select billing type if there is a clinicpref associated to the chosen clinic, otherwise "Use Global Preference".
			comboDefaultBillingType.Items.AddDefNone(Lan.g(this,"Use Global Preference"));
			comboDefaultBillingType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes,true));
			comboDefaultBillingType.SetSelectedDefNum(ClinicPrefs.GetLong(PrefName.PracticeDefaultBillType,ClinicCur.ClinicNum));
			comboInsAutoReceiveNoAssign.Items.Add("Off");
			comboInsAutoReceiveNoAssign.Items.Add("On");
			comboInsAutoReceiveNoAssign.Items.Add("Use Global Preference");
			comboInsAutoReceiveNoAssign.SetSelected(2);//Default to use global preference.
			ClinicPref clinicPrefInsAutoReceiveNoAssign=ClinicPrefs.GetPref(PrefName.InsAutoReceiveNoAssign,ClinicCur.ClinicNum);
			if(clinicPrefInsAutoReceiveNoAssign!=null) {
				comboInsAutoReceiveNoAssign.SetSelected(PIn.Int(clinicPrefInsAutoReceiveNoAssign.ValueString));
			}
			//"Always Assign Benefits to the Patient" checkbox is an override. If the clinic has this pref value, this means it is checked.
			ClinicPref clinicPrefAlwaysAssignBenToPatient=ClinicPrefs.GetPref(PrefName.InsDefaultAssignBen,ClinicCur.ClinicNum);
			if(clinicPrefAlwaysAssignBenToPatient!=null) {
				checkAlwaysAssignBenToPatient.Checked=true;
			}
			EmailAddress emailAddress=EmailAddresses.GetOne(ClinicCur.EmailAddressNum);
			if(emailAddress!=null) {
				textEmail.Text=emailAddress.GetFrom();
				butEmailNone.Enabled=true;
			}
			textClinicEmailAliasOverride.Text=ClinicCur.EmailAliasOverride;
			_isMedLabHL7DefEnabled=HL7Defs.IsExistingHL7Enabled(0,true);
			if(_isMedLabHL7DefEnabled) {
				textMedLabAcctNum.Visible=true;
				labelMedLabAcctNum.Visible=true;
				textMedLabAcctNum.Text=ClinicCur.MedLabAccountNum;
			}
			comboBoxTimeZone.Items.Clear();
			List<TimeZoneInfo> listTimeZoneInfos=TimeZoneInfo.GetSystemTimeZones().ToList();
			comboBoxTimeZone.Items.AddList(listTimeZoneInfos, x=> x.DisplayName);
			for(int i=0;i<listTimeZoneInfos.Count;i++) {
				if(ClinicCur.TimeZone==listTimeZoneInfos[i].Id) {
					comboBoxTimeZone.SetSelected(i);
				}
			}
			if(comboBoxTimeZone.SelectedIndex<0) {
				int index=comboBoxTimeZone.Items.GetAll<TimeZoneInfo>().FindIndex(x => x.Id==TimeZoneInfo.Local.Id);
				if(index>-1) {
					comboBoxTimeZone.SetSelected(index);
				}
			}
		}

		private void butPickInsBillingProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(comboInsBillingProv.Items.GetAll<Provider>());
			formProviderPick.SelectedProvNum=comboInsBillingProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboInsBillingProv.SetSelectedProvNum(formProviderPick.SelectedProvNum);
		}

		private void butPickDefaultProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(comboDefaultProvider.Items.GetAll<Provider>());
			formProviderPick.SelectedProvNum=comboDefaultProvider.GetSelectedProvNum();//this is 0 if selectedIndex -1
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formProviderPick.SelectedProvNum>0){
				comboDefaultProvider.SetSelectedProvNum(formProviderPick.SelectedProvNum);
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			comboDefaultProvider.SetSelectedProvNum(0);
		}

		private void FillSpecialty() {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ClinicSpecialty);
			gridSpecialty.BeginUpdate();
			gridSpecialty.Columns.Clear();
			gridSpecialty.Columns.Add(new GridColumn(Lan.g(gridSpecialty.TranslationName,"Specialty"),100));
			gridSpecialty.ListGridRows.Clear();
			GridRow row;
			string specialtyDescript;
			for(int i = 0;i<ListDefLinksSpecialties.Count;i++) {
				row=new GridRow();
				specialtyDescript="";
				Def def = listDefs.FirstOrDefault(x => x.DefNum==ListDefLinksSpecialties[i].DefNum);
				if(def!=null) {
					specialtyDescript=def.ItemName+(def.IsHidden ? (" ("+Lan.g(this,"hidden")+")") : "");
				}
				row.Cells.Add(specialtyDescript);
				row.Tag=ListDefLinksSpecialties[i];
				gridSpecialty.ListGridRows.Add(row);
			}
			gridSpecialty.EndUpdate();
		}
		
		private void butAdd_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ClinicSpecialty);
			formDefinitionPicker.HasShowHiddenOption=false;
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				for(int i=0;i<formDefinitionPicker.ListDefsSelected.Count;i++) {
					if(ListDefLinksSpecialties.Any(x => x.DefNum==formDefinitionPicker.ListDefsSelected[i].DefNum)) {
						continue;//Definition already added to this clinic. 
					}
					DefLink defLink=new DefLink();
					defLink.DefNum=formDefinitionPicker.ListDefsSelected[i].DefNum;
					defLink.FKey=ClinicCur.ClinicNum;//could be 0 if IsNew
					defLink.LinkType=DefLinkType.ClinicSpecialty;
					ListDefLinksSpecialties.Add(defLink);
				}
				FillSpecialty();
			}
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(gridSpecialty.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select a specialty first."));
				return;
			}
			gridSpecialty.SelectedIndices
				.Select(x => (DefLink)gridSpecialty.ListGridRows[x].Tag).ToList()
				.ForEach(x => ListDefLinksSpecialties.Remove(x));
			FillSpecialty();
		}

		private void butEmail_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEmailAddresses=new FormEmailAddresses();
			formEmailAddresses.IsSelectionMode=true;
			formEmailAddresses.ShowDialog();
			if(formEmailAddresses.DialogResult!=DialogResult.OK) {
				return;
			}
			ClinicCur.EmailAddressNum=formEmailAddresses.EmailAddressNum;
			textEmail.Text=EmailAddresses.GetOne(formEmailAddresses.EmailAddressNum).GetFrom();
			butEmailNone.Enabled=true;
		}

		private void buttDetachEmail_Click(object sender,EventArgs e) {
			ClinicCur.EmailAddressNum=0;
			textEmail.Text="";
			butEmailNone.Enabled=false;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			#region Validation 
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			if(textClinicAbbr.Text==""){
				MsgBox.Show(this,"Abbreviation cannot be blank.");
				return;
			}
			if(radioInsBillingProvSpecific.Checked && comboInsBillingProv.SelectedIndex==-1){ 
				MsgBox.Show(this,"You must select a provider.");
				return;
			}
			string phone=textPhone.Text;
			if(Application.CurrentCulture.Name=="en-US"){
				phone=phone.Replace("(","");
				phone=phone.Replace(")","");
				phone=phone.Replace(" ","");
				phone=phone.Replace("-","");
				if(phone.Length!=0 && phone.Length!=10){
					MsgBox.Show(this,"Invalid phone");
					return;
				}
			}
			string fax=textFax.Text;
			if(Application.CurrentCulture.Name=="en-US") {
				fax=fax.Replace("(","");
				fax=fax.Replace(")","");
				fax=fax.Replace(" ","");
				fax=fax.Replace("-","");
				if(fax.Length!=0 && fax.Length!=10) {
					MsgBox.Show(this,"Invalid fax");
					return;
				}
			}
			if(_isMedLabHL7DefEnabled //MedLab HL7 def is enabled, so textMedLabAcctNum is visible
				&& !string.IsNullOrWhiteSpace(textMedLabAcctNum.Text) //MedLabAcctNum has been entered
				&& Clinics.GetWhere(x => x.ClinicNum!=ClinicCur.ClinicNum)
						.Any(x => x.MedLabAccountNum==textMedLabAcctNum.Text.Trim())) //this account num is already in use by another Clinic
			{
				MsgBox.Show(this,"The MedLab Account Number entered is already in use by another clinic.");
				return;
			}
			if(checkHidden.Checked) {
				//ensure that there are no users who have only this clinic assigned to them.
				List<Userod> listUserodsRestricted = Userods.GetUsersOnlyThisClinic(ClinicCur.ClinicNum);
				if(listUserodsRestricted.Count > 0) {
					MessageBox.Show(Lan.g(this,"You may not hide this clinic as the following users are restricted to only this clinic") + ": "
						+ string.Join(", ",listUserodsRestricted.Select(x => x.UserName)));
					return;
				}
			}
			long externalID=0;
			if(textExternalID.Text != "") {
				try {
					externalID=long.Parse(textExternalID.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix data entry errors first."+"\r\n"+", The External ID must be a number. No letters or symbols allowed.");
					return;
				}
			}
			if(!Regex.IsMatch(textClinicEmailAliasOverride.Text,"^[A-Za-z0-9]*$")) {
				MsgBox.Show(this,"The Email Alias Override can only contain letters and numbers.");
				return;
			}
			#endregion Validation
			#region Set Values
			ClinicCur.IsMedicalOnly=checkIsMedicalOnly.Checked;
			ClinicCur.IsInsVerifyExcluded=checkExcludeFromInsVerifyList.Checked;
			ClinicCur.HasProcOnRx=checkProcCodeRequired.Checked;
			ClinicCur.Abbr=textClinicAbbr.Text;
			ClinicCur.Description=textDescription.Text;
			ClinicCur.Phone=phone;
			ClinicCur.Fax=fax;
			ClinicCur.Address=textAddress.Text;
			ClinicCur.Address2=textAddress2.Text;
			ClinicCur.City=textCity.Text;
			ClinicCur.State=textState.Text;
			ClinicCur.Zip=textZip.Text;
			ClinicCur.BillingAddress=textBillingAddress.Text;
			ClinicCur.BillingAddress2=textBillingAddress2.Text;
			ClinicCur.BillingCity=textBillingCity.Text;
			ClinicCur.BillingState=textBillingST.Text;
			ClinicCur.BillingZip=textBillingZip.Text;
			ClinicCur.PayToAddress=textPayToAddress.Text;
			ClinicCur.PayToAddress2=textPayToAddress2.Text;
			ClinicCur.PayToCity=textPayToCity.Text;
			ClinicCur.PayToState=textPayToST.Text;
			ClinicCur.PayToZip=textPayToZip.Text;
			ClinicCur.BankNumber=textBankNumber.Text;
			ClinicCur.DefaultPlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			ClinicCur.UseBillAddrOnClaims=checkUseBillingAddressOnClaims.Checked;
			ClinicCur.IsHidden=checkHidden.Checked;
			ClinicCur.ExternalID=externalID;
			ClinicCur.EmailAliasOverride=textClinicEmailAliasOverride.Text.Trim();
			long defNumRegion=0;
			if(comboRegion.SelectedIndex>0){
				defNumRegion=_listDefsRegions[comboRegion.SelectedIndex-1].DefNum;
			}
			ClinicCur.Region=defNumRegion;
			if(radioInsBillingProvDefault.Checked){//default=0
				ClinicCur.InsBillingProv=0;
			}
			else if(radioInsBillingProvTreat.Checked){//treat=-1
				ClinicCur.InsBillingProv=-1;
			}
			else{
				ClinicCur.InsBillingProv=comboInsBillingProv.GetSelectedProvNum();
			}
			ClinicCur.DefaultProv=comboDefaultProvider.GetSelectedProvNum();//0 for selectedIndex -1
			if(_isMedLabHL7DefEnabled) {
				ClinicCur.MedLabAccountNum=textMedLabAcctNum.Text.Trim();
			}
			ClinicCur.SchedNote=textSchedRules.Text;
			if(comboBoxTimeZone.SelectedIndex>-1){
				ClinicCur.TimeZone=comboBoxTimeZone.GetSelected<TimeZoneInfo>().Id;
			}
			List<PrefName> listClinicPrefsToDelete=new List<PrefName>();
			if(comboDefaultBillingType.SelectedIndex>0) {//If default billing type is not set to default, update/insert clinicpref row.
				string strBillingType=POut.Long(comboDefaultBillingType.GetSelectedDefNum());
				ClinicPrefs.Upsert(PrefName.PracticeDefaultBillType,ClinicCur.ClinicNum,strBillingType);
			}
			else {//"Use Global Preference" selected, delete the pref if it exists.
				listClinicPrefsToDelete.Add(PrefName.PracticeDefaultBillType);
			}
			if(checkAlwaysAssignBenToPatient.Checked) {//If checked, we will upsert into clinicpref table. False accurately represents the pref.
				ClinicPrefs.Upsert(PrefName.InsDefaultAssignBen,ClinicCur.ClinicNum,"0");
			}
			else {//Since we aren't overriding, we want to remove the clinicpref to not confuse as boolean state.
				listClinicPrefsToDelete.Add(PrefName.InsDefaultAssignBen);
			}
			if(comboInsAutoReceiveNoAssign.SelectedIndex==2) {//2=Use Global Preference
				listClinicPrefsToDelete.Add(PrefName.InsAutoReceiveNoAssign);
			}
			else {
				string strInsAutoReceiveNoAssignClinicPref=POut.Int(comboInsAutoReceiveNoAssign.SelectedIndex);
				ClinicPrefs.Upsert(PrefName.InsAutoReceiveNoAssign,ClinicCur.ClinicNum,strInsAutoReceiveNoAssignClinicPref);
			}
			ClinicPrefs.DeletePrefs(ClinicCur.ClinicNum,listClinicPrefsToDelete);
			ClinicPrefs.RefreshCache();
			#endregion Set Values
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void checkHidden_CheckedChanged(object sender,EventArgs e) {
			if(!checkHidden.Checked) { //Unhiding clinic
				return;
			}
			//User is trying to hide clinic
			long numPatients=Patients.GetPatNumsByClinic(ClinicCur.ClinicNum).Count;
			if(numPatients>0) { //Patients attached to this clinic
				MsgBox.Show(this,"There are "+numPatients+" patient(s) attached to this clinic. It cannot be hidden until they are removed.");
				checkHidden.Checked=false; //Return box to unchecked
			}
		}
	}
}
