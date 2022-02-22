using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormPractice : FormODBase {
		private List<Def> _listBillingTypeDefs;

		///<summary></summary>
		public FormPractice(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPractice_Load(object sender, System.EventArgs e) {
			checkIsMedicalOnly.Checked=PrefC.GetBool(PrefName.PracticeIsMedicalOnly);
			if(Programs.UsingEcwTightOrFullMode()) {
				checkIsMedicalOnly.Visible=false;
			}
			textPracticeTitle.Text=PrefC.GetString(PrefName.PracticeTitle);
			textAddress.Text=PrefC.GetString(PrefName.PracticeAddress);
			textAddress2.Text=PrefC.GetString(PrefName.PracticeAddress2);
			textCity.Text=PrefC.GetString(PrefName.PracticeCity);
			textST.Text=PrefC.GetString(PrefName.PracticeST);
			textZip.Text=PrefC.GetString(PrefName.PracticeZip);
			textPhone.Text=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
			textFax.Text=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticeFax));
			checkUseBillingAddressOnClaims.Checked=PrefC.GetBool(PrefName.UseBillingAddressOnClaims);
			textBillingAddress.Text=PrefC.GetString(PrefName.PracticeBillingAddress);
			textBillingAddress2.Text=PrefC.GetString(PrefName.PracticeBillingAddress2);
			textBillingCity.Text=PrefC.GetString(PrefName.PracticeBillingCity);
			textBillingST.Text=PrefC.GetString(PrefName.PracticeBillingST);
			textBillingZip.Text=PrefC.GetString(PrefName.PracticeBillingZip);
			textBillingPhone.Text=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticeBillingPhone));
			textPayToAddress.Text=PrefC.GetString(PrefName.PracticePayToAddress);
			textPayToAddress2.Text=PrefC.GetString(PrefName.PracticePayToAddress2);
			textPayToCity.Text=PrefC.GetString(PrefName.PracticePayToCity);
			textPayToST.Text=PrefC.GetString(PrefName.PracticePayToST);
			textPayToZip.Text=PrefC.GetString(PrefName.PracticePayToZip);
			textPayToPhone.Text=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePayToPhone));
			textBankNumber.Text=PrefC.GetString(PrefName.PracticeBankNumber);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CH")) {//CH is for switzerland. eg de-CH
				textBankRouting.Text=PrefC.GetString(PrefName.BankRouting);
				textBankAddress.Text=PrefC.GetString(PrefName.BankAddress);
			}
			else {
				groupSwiss.Visible=false;
			}
			comboProv.Items.AddProvsFull(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(PrefC.GetLong(PrefName.PracticeDefaultProv));
			listBillType.Items.Clear();
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<_listBillingTypeDefs.Count;i++){
				listBillType.Items.Add(_listBillingTypeDefs[i].ItemName);
				if(_listBillingTypeDefs[i].DefNum==PrefC.GetLong(PrefName.PracticeDefaultBillType)) {
					listBillType.SelectedIndex=i;
				}
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				labelPlaceService.Visible=false;
				listPlaceService.Visible=false;
			}
			listPlaceService.Items.Clear();
			listPlaceService.Items.AddEnums<PlaceOfService>();
			listPlaceService.SelectedIndex=PrefC.GetInt(PrefName.DefaultProcedurePlaceService);
			long selectedBillingProvNum=PrefC.GetLong(PrefName.InsBillingProv);
			comboInsBillingProv.Items.AddProvsFull(Providers.GetDeepCopy(true));//selectedIndex could remain -1
			if(selectedBillingProvNum==0) {
				radioInsBillingProvDefault.Checked=true;//default=0
			}
			else if(selectedBillingProvNum==-1) {
				radioInsBillingProvTreat.Checked=true;//treat=-1
			}
			else {
				radioInsBillingProvSpecific.Checked=true;//specific=any number >0. Foreign key to ProvNum
				comboInsBillingProv.SetSelectedProvNum(selectedBillingProvNum);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			string phone=textPhone.Text;
			string billingPhone=textBillingPhone.Text;
			string payTophone=textPayToPhone.Text;
			if(!TelephoneNumbers.IsNumberValidTenDigit(ref phone)) {
				MsgBox.Show(this,"Invalid Phone number.  Must contain exactly ten digits.");
				return;
			}
			if(!TelephoneNumbers.IsNumberValidTenDigit(ref billingPhone)) {
				MsgBox.Show(this,"Invalid Billing section Phone number.  Must contain exactly ten digits.");
				return;
			}
			if(!TelephoneNumbers.IsNumberValidTenDigit(ref payTophone)) {
				MsgBox.Show(this,"Invalid Pay To section Phone number.  Must contain exactly ten digits.");
				return;
			}
			string fax=textFax.Text;
			if(!TelephoneNumbers.IsNumberValidTenDigit(ref fax)) {
				MsgBox.Show(this,"Invalid Fax.  Must contain exactly ten digits.");
				return;
			}
			if(radioInsBillingProvSpecific.Checked && comboInsBillingProv.SelectedIndex ==-1){
				MsgBox.Show(this,"You must select a provider.");
				return;
			}
			if(comboProv.Items.Count==0){
				MsgBox.Show(this,"Database must have at least one provider entered, first.");
				return;
			}
			if(comboProv.SelectedIndex==-1){//practice really needs a default prov
				comboProv.SelectedIndex=0;
			}
			if(Providers.GetProv(comboProv.GetSelectedProvNum()).FeeSched==0){
				MsgBox.Show(this,"The selected provider must have a fee schedule set before they can be the default provider.");
				return;
			}
			bool changed=false;
			if( Prefs.UpdateBool(PrefName.PracticeIsMedicalOnly,checkIsMedicalOnly.Checked)
				| Prefs.UpdateString(PrefName.PracticeTitle,textPracticeTitle.Text)
				| Prefs.UpdateString(PrefName.PracticeAddress,textAddress.Text)
				| Prefs.UpdateString(PrefName.PracticeAddress2,textAddress2.Text)
				| Prefs.UpdateString(PrefName.PracticeCity,textCity.Text)
				| Prefs.UpdateString(PrefName.PracticeST,textST.Text)
				| Prefs.UpdateString(PrefName.PracticeZip,textZip.Text)
				| Prefs.UpdateString(PrefName.PracticePhone,phone)
				| Prefs.UpdateString(PrefName.PracticeFax,fax)
				| Prefs.UpdateBool(PrefName.UseBillingAddressOnClaims,checkUseBillingAddressOnClaims.Checked)
				| Prefs.UpdateString(PrefName.PracticeBillingAddress,textBillingAddress.Text)
				| Prefs.UpdateString(PrefName.PracticeBillingAddress2,textBillingAddress2.Text)
				| Prefs.UpdateString(PrefName.PracticeBillingCity,textBillingCity.Text)
				| Prefs.UpdateString(PrefName.PracticeBillingST,textBillingST.Text)
				| Prefs.UpdateString(PrefName.PracticeBillingZip,textBillingZip.Text)
				| Prefs.UpdateString(PrefName.PracticeBillingPhone,billingPhone)
				| Prefs.UpdateString(PrefName.PracticePayToAddress,textPayToAddress.Text)
				| Prefs.UpdateString(PrefName.PracticePayToAddress2,textPayToAddress2.Text)
				| Prefs.UpdateString(PrefName.PracticePayToCity,textPayToCity.Text)
				| Prefs.UpdateString(PrefName.PracticePayToST,textPayToST.Text)
				| Prefs.UpdateString(PrefName.PracticePayToZip,textPayToZip.Text)
				| Prefs.UpdateString(PrefName.PracticePayToPhone,payTophone)
				| Prefs.UpdateString(PrefName.PracticeBankNumber,textBankNumber.Text))
			{
				changed=true;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CH")) {//CH is for switzerland. eg de-CH
				if( Prefs.UpdateString(PrefName.BankRouting,textBankRouting.Text)
					| Prefs.UpdateString(PrefName.BankAddress,textBankAddress.Text))
				{
					changed=true;
				}
			}
			if(Prefs.UpdateLong(PrefName.PracticeDefaultProv,comboProv.GetSelectedProvNum())){
				changed=true;
			}
			if(listBillType.SelectedIndex!=-1){
				if(Prefs.UpdateLong(PrefName.PracticeDefaultBillType
					,_listBillingTypeDefs[listBillType.SelectedIndex].DefNum))
				{
					changed=true;
				}
			}
			if(Prefs.UpdateLong(PrefName.DefaultProcedurePlaceService,listPlaceService.SelectedIndex)){
				changed=true;
			}
			if(radioInsBillingProvDefault.Checked){//default=0
				if(Prefs.UpdateLong(PrefName.InsBillingProv,0)){
					changed=true;
				}
			}
			else if(radioInsBillingProvTreat.Checked){//treat=-1
				if(Prefs.UpdateLong(PrefName.InsBillingProv,-1)){
					changed=true;
				}
			}
			else{
				if(Prefs.UpdateLong(PrefName.InsBillingProv,comboInsBillingProv.GetSelectedProvNum())){
					changed=true;
				}
			}
			if(changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FormEServicesMobileSynch.UploadPreference(PrefName.PracticeTitle);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
