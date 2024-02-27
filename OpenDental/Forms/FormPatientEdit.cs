using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using System.Diagnostics;
using System.Linq;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormPatientEdit : FormODBase {
		///<summary>Set true if this is a new patient. Patient must have been already inserted. If users clicks cancel, this patient will be deleted.</summary>
		public bool IsNew;
		private string _siteOriginal;
		private bool _isMouseInListSites;
		private List<Site> _listSitesFiltered;
		private string _countyOriginal;
		private bool _isMouseInListCounties;
		private County[] _countyArray;
		private string _empOriginal;//used in the emp dropdown logic
		private bool isMouseInListEmps;
		///<summary>This is the object that is altered in this form.</summary>
		private Patient _patient;
		///<summary>This is the object that is altered in this form, since it is an extension of the patient table.</summary>
		private PatientNote _patientNote;
		//private RefAttach[] RefList;
		private Family _family;
		private Patient _patientOld;
		///<summary>Will include the languages setup in the settings, and also the language of this patient if that language is not on the selection list.</summary>
		private List<string> _listLanguages;
		private List<Guardian> _listGuardians;
		///<summary>If the user presses cancel, use this list to revert any changes to the guardians for all family members.</summary>
		private List<Guardian> _listGuardiansForFamOld;
		///<summary>Local cache of RefAttaches for the current patient.  Set in FillReferrals().</summary>
		private List<RefAttach> _listRefAttaches;
		private System.Windows.Forms.ListBox listMedicaidStates;//displayed from within code, not designer
		private List<RequiredField> _listRequiredFields;
		private string _medicaidStateOriginal;//used in the medicaidState dropdown logic
		private bool _IsMouseInListMedicaidStates;
		private System.Windows.Forms.ListBox listStates;//displayed from within code, not designer
		private string _stateOriginal;//used in the medicaidState dropdown logic
		private bool _isMouseInListStates;
		private bool _isMissingRequiredFields;
		private bool _isLoad;//To keep track if ListBoxes' selected index is changed by the user
		private ErrorProvider _errorProvider=new ErrorProvider();
		private bool _isValidating=false;
		private EhrPatient _ehrPatient;
		private List<PatientRace> _listPatientRaces;
		private UI.ComboBox comboBoxMultiRace;
		private bool _hasGuardiansChanged=false;
		///<summary>Because adding the new feature where patients can choose their race from hundreds of options would cause us to need to recertify EHR, 
		///we committed all the code for the new feature while keeping the old behavior for EHR users. When we are ready to switch to the new feature, 
		///all we need to do is set this boolean to true (hopefully).</summary>
		private bool _isUsingNewRaceFeature=!PrefC.GetBool(PrefName.ShowFeatureEhr);
		private DefLink _defLinkPatient;
		private CommOptOut _commOptOut;
		///<summary>List of PatientStatuses shown in listStatus, must be 1:1 with listStatus.
		///Deleted is excluded, unless PatCur is flagged as deleted.
		///Needed due to index differences when deleted is not present.</summary>
		private List<PatientStatus> _listPatientStatuses=new List<PatientStatus> ();
		///<summary>Used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.</summary>
		private string _maskedSSNOld="";
		///<summary>Used to display the available billing types. Will contain hidden billing types.</summary>
		private List<Def> _listDefsBillingType;
		private bool _isBirthdayMasked;

		///<summary></summary>
		public FormPatientEdit(Patient patient,Family family){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patient=patient;
			_patientNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor);
			_family=family;
			_patientOld=patient.Copy();
			listEmps=new System.Windows.Forms.ListBox();
			listEmps.Location=new Point(LayoutManager.Scale(textEmployer.Left),LayoutManager.Scale(textEmployer.Bottom));
			listEmps.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listEmps.Visible=false;
			listEmps.Click += new System.EventHandler(listEmps_Click);
			listEmps.DoubleClick += new System.EventHandler(listEmps_DoubleClick);
			listEmps.MouseEnter += new System.EventHandler(listEmps_MouseEnter);
			listEmps.MouseLeave += new System.EventHandler(listEmps_MouseLeave);
			LayoutManager.Add(listEmps,this);
			listEmps.BringToFront();
			listCounties=new System.Windows.Forms.ListBox();
			listCounties.Location=new Point(LayoutManager.Scale(tabControlPatInfo.Left+tabPublicHealth.Left+textCounty.Left),
				LayoutManager.Scale(tabControlPatInfo.Top+tabPublicHealth.Top+textCounty.Bottom));
			listCounties.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listCounties.Visible=false;
			listCounties.Click += new System.EventHandler(listCounties_Click);
			//listCounties.DoubleClick += new System.EventHandler(listCars_DoubleClick);
			listCounties.MouseEnter += new System.EventHandler(listCounties_MouseEnter);
			listCounties.MouseLeave += new System.EventHandler(listCounties_MouseLeave);
			LayoutManager.Add(listCounties,this);
			listCounties.BringToFront();
			listSites=new System.Windows.Forms.ListBox();
			listSites.Location=new Point(LayoutManager.Scale(tabControlPatInfo.Left+tabPublicHealth.Left+textSite.Left),
				LayoutManager.Scale(tabControlPatInfo.Top+tabPublicHealth.Top+textSite.Bottom));
			listSites.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listSites.Visible=false;
			listSites.Click += new System.EventHandler(listSites_Click);
			listSites.MouseEnter += new System.EventHandler(listSites_MouseEnter);
			listSites.MouseLeave += new System.EventHandler(listSites_MouseLeave);
			LayoutManager.Add(listSites,this);
			listSites.BringToFront();
			Lan.F(this);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				labelST.Text=Lan.g(this,"ST, Country");
				textCountry.Visible=true;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSSN.Text=Lan.g(this,"SIN");
				labelZip.Text=Lan.g(this,"Postal Code");
				labelST.Text=Lan.g(this,"Province");
				butEditZip.Text=Lan.g(this,"Edit Postal");
				labelCanadianEligibilityCode.Visible=true;
				comboCanadianEligibilityCode.Visible=true;
				radioStudentN.Visible=false;
				radioStudentP.Visible=false;
				radioStudentF.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("GB")){//en-GB
				//labelSSN.Text="?";
				labelZip.Text=Lan.g(this,"Postcode");
				labelST.Text="";//no such thing as state in GB
				butEditZip.Text=Lan.g(this,"Edit Postcode");
			}
			_referredFromToolTip=new ToolTip();
			_referredFromToolTip.InitialDelay=500;
			_referredFromToolTip.ReshowDelay=100;
			listMedicaidStates=new System.Windows.Forms.ListBox();
			listMedicaidStates.Location=new Point(LayoutManager.Scale(textMedicaidState.Left),LayoutManager.Scale(textMedicaidState.Bottom));
			listMedicaidStates.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listMedicaidStates.Visible=false;
			listMedicaidStates.Click += new System.EventHandler(listMedicaidStates_Click);
			listMedicaidStates.MouseEnter += new System.EventHandler(listMedicaidStates_MouseEnter);
			listMedicaidStates.MouseLeave += new System.EventHandler(listMedicaidStates_MouseLeave);
			LayoutManager.Add(listMedicaidStates,this);
			listMedicaidStates.BringToFront();
			listStates=new System.Windows.Forms.ListBox();
			listStates.Location=new Point(LayoutManager.Scale(textState.Left+groupBox1.Left),LayoutManager.Scale(textState.Bottom+groupBox1.Top));
			listStates.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listStates.Visible=false;
			listStates.Click += new System.EventHandler(listStates_Click);
			listStates.MouseEnter += new System.EventHandler(listStates_MouseEnter);
			listStates.MouseLeave += new System.EventHandler(listStates_MouseLeave);
			LayoutManager.Add(listStates,this);
			listStates.BringToFront();
		}

		private void FormPatientEdit_Load(object sender, System.EventArgs e) {
			_isLoad=true;
			if(_listRequiredFields==null) {
				_listRequiredFields=RequiredFields.GetRequiredFields();
			}
			tabEHR.Show();
			tabICE.Show();
			tabOther.Show();
			tabHospitals.Show();
			tabPublicHealth.Show();
			tabPublicHealth.Select();
			checkAddressSame.Checked=true;
			checkBillProvSame.Checked=true;
			checkNotesSame.Checked=true;
			checkEmailPhoneSame.Checked=true;
			checkArriveEarlySame.Checked=true;
			checkSuperBilling.Enabled=(Security.IsAuthorized(EnumPermType.PatientBillingEdit,true));
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.Patient,Patients.IsPatientHashValid(_patient));
			bool isAuthArchivedEdit=Security.IsAuthorized(EnumPermType.ArchivedPatientEdit,true);
			List<Patient> listPatientsFamily=_family.ListPats.ToList();
			if(!isAuthArchivedEdit) {
				//Exclude Archived pats if user does not have permission.  If user doesn't have the permission and all non-Archived patients pass the
				//the conditions, but there is an Archived patient who does not, the check will still be true.  The Archived will not be updated on OK.
				listPatientsFamily=listPatientsFamily.Where(x => x.PatStatus!=PatientStatus.Archived).ToList();
			}
			#region SameForFamily
			//for the comparison logic to work, any nulls must be converted to ""
			if(_patient.HmPhone is null) _patient.HmPhone="";
			if(_patient.Address is null) _patient.Address="";
			if(_patient.Address2 is null) _patient.Address2="";
			if(_patient.City is null) _patient.City="";
			if(_patient.State is null) _patient.State="";
			if(_patient.Zip is null) _patient.Zip="";
			if(_patient.Country is null) _patient.Country="";
			if(_patient.CreditType is null) _patient.CreditType="";
			if(_patient.AddrNote is null) _patient.AddrNote="";
			if(_patient.WirelessPhone is null) _patient.WirelessPhone="";
			if(_patient.WkPhone is null) _patient.WkPhone="";
			if(_patient.Email is null) _patient.Email="";
			for(int i=0;i<listPatientsFamily.Count;i++){
				if(_patient.HmPhone!=listPatientsFamily[i].HmPhone
					|| _patient.Address!=listPatientsFamily[i].Address
					|| _patient.Address2!=listPatientsFamily[i].Address2
					|| _patient.City!=listPatientsFamily[i].City
					|| _patient.State!=listPatientsFamily[i].State
					|| _patient.Zip!=listPatientsFamily[i].Zip
					|| _patient.Country!=listPatientsFamily[i].Country)
				{
					checkAddressSame.Checked=false;
				}
				if(_patient.CreditType!=listPatientsFamily[i].CreditType
					|| _patient.BillingType!=listPatientsFamily[i].BillingType
					|| _patient.PriProv!=listPatientsFamily[i].PriProv
					|| _patient.SecProv!=listPatientsFamily[i].SecProv
					|| _patient.FeeSched!=listPatientsFamily[i].FeeSched)
				{
					checkBillProvSame.Checked=false;
				}
				if(_patient.AddrNote!=listPatientsFamily[i].AddrNote){
					checkNotesSame.Checked=false;
				}
				if(_patient.WirelessPhone!=listPatientsFamily[i].WirelessPhone
					|| _patient.WkPhone!=listPatientsFamily[i].WkPhone
					|| _patient.Email!=listPatientsFamily[i].Email) 
				{
					checkEmailPhoneSame.Checked=false;
				}
				if(_patient.AskToArriveEarly!=listPatientsFamily[i].AskToArriveEarly) {
					checkArriveEarlySame.Checked=false;
				}
			}	
			if(PrefC.GetBool(PrefName.SameForFamilyCheckboxesUnchecked)){
				checkAddressSame.Checked=false;
				checkBillProvSame.Checked=false;
				checkNotesSame.Checked=false;
				checkEmailPhoneSame.Checked=false;
			}
			//SuperFamilies is enabled, Syncing SuperFam Info is enabled, and this is the superfamily head.  Show the sync checkbox.
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)
				&& PrefC.GetBool(PrefName.PatientAllSuperFamilySync)
				&& _patient.SuperFamily!=0
				&& _patient.PatNum==_patient.SuperFamily) //Has to be the Super Head.
			{
				checkAddressSameForSuperFam.Visible=true;
				//Check all superfam members for any with differing information
				checkAddressSameForSuperFam.Checked=Patients.SuperFamHasSameAddrPhone(_patient,isAuthArchivedEdit);
			}
			#endregion SameForFamily
			if(IsNew) {
				_patient.PatStatus=PatientStatus.Patient;
			}
			checkRestrictSched.Checked=PatRestrictionL.IsRestricted(_patient.PatNum,PatRestrict.ApptSchedule,true);
			textPatNum.Text=_patient.PatNum.ToString();
			textLName.Text=_patient.LName;
			textFName.Text=_patient.FName;
			textMiddleI.Text=_patient.MiddleI;
			textPreferred.Text=_patient.Preferred;
			textTitle.Text=_patient.Title;
			textSalutation.Text=_patient.Salutation;
			textIceName.Text=_patientNote.ICEName;
			textIcePhone.Text=_patientNote.ICEPhone;
			_ehrPatient=EhrPatients.Refresh(_patient.PatNum);
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Show mother's maiden name UI if using EHR.
				labelMotherMaidenFname.Visible=true;
				textMotherMaidenFname.Visible=true;
				textMotherMaidenFname.Text=_ehrPatient.MotherMaidenFname;
				labelMotherMaidenLname.Visible=true;
				textMotherMaidenLname.Visible=true;
				textMotherMaidenLname.Text=_ehrPatient.MotherMaidenLname;
				labelDeceased.Visible=true;
				textDateTimeDeceased.Visible=true;
			}
			else {
				tabControlPatInfo.TabPages.Remove(tabEHR);
			}
			/*Show checkDoseSpotConsent if DoseSpot is enabled. Currently, consent cannot be revoked with DoseSpot,
			so the check box is checked and disabled if consent was previously given.*/
			Program programErx=Programs.GetCur(ProgramName.eRx);
			ErxOption erxOption=PIn.Enum<ErxOption>(ProgramProperties.GetPropForProgByDesc(programErx.ProgramNum,Erx.PropertyDescs.ErxOption).PropertyValue);
			if(programErx.Enabled && (erxOption==ErxOption.DoseSpot || erxOption==ErxOption.DoseSpotWithNewCrop)) {
				checkDoseSpotConsent.Visible=true;
				checkDoseSpotConsent.Checked=_patientNote.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx);
				if(checkDoseSpotConsent.Checked) {//once checked, should never be able to be unchecked.
					checkDoseSpotConsent.Enabled=false;
				}
			}
			_listPatientStatuses=Patients.GetPatientStatuses(_patient);
			listStatus.Items.AddList(_listPatientStatuses,x => Lan.g("enumPatientStatus", x.ToString()));
			listStatus.SetSelectedEnum(_patient.PatStatus);
			listGender.Items.AddEnums<PatientGender>();
			listGender.SetSelectedEnum(_patient.Gender);
			listPosition.Items.AddEnums<PatientPosition>();
			listPosition.SetSelectedEnum(_patient.Position);
			FillGuardians();
			_listGuardiansForFamOld=_family.ListPats.SelectMany(x => Guardians.Refresh(x.PatNum)).ToList();
			odDatePickerBirthDate.SetDateTime(_patient.Birthdate);
			textBirthdateMask.Text=Patients.DOBFormatHelper(_patient.Birthdate,doMask:true);//If birthdate is not set this will set text to empty string.
			if(PrefC.GetBool(PrefName.PatientDOBMasked) || !Security.IsAuthorized(EnumPermType.PatientDOBView,true)) {
				//turn off validation until the user unmasks DOB.
				if(!odDatePickerBirthDate.IsEmptyDateTime()) {
					_isBirthdayMasked=true;
					odDatePickerBirthDate.ReadOnly=true;
					odDatePickerBirthDate.Visible=false;
					textBirthdateMask.Visible=true;
				}
				//Disable View button if no DOB
				if(!Security.IsAuthorized(EnumPermType.PatientDOBView,true) || odDatePickerBirthDate.IsEmptyDateTime()) {
					hideButViewBirthDate();
				}
			}
			else {
				hideButViewBirthDate();
			}
			if(_patient.DateTimeDeceased.Year > 1880) {
				textDateTimeDeceased.Text=_patient.DateTimeDeceased.ToShortDateString()+" "+_patient.DateTimeDeceased.ToShortTimeString();
			}
			textAge.Text=PatientLogic.DateToAgeString(_patient.Birthdate,_patient.DateTimeDeceased);
			if(PrefC.GetBool(PrefName.PatientSSNMasked) || !Security.IsAuthorized(EnumPermType.PatientSSNView,true)) {
				//turn off validation until the user unmasks SSN.
				textSSN.Text=Patients.SSNFormatHelper(_patient.SSN,true);//If PatCur.SSN is null or empty, returns empty string.
				if(textSSN.Text!="") {
					textSSN.ReadOnly=true;
				}
				_maskedSSNOld=textSSN.Text;
				//Hide button if no SSN entered
				if(Security.IsAuthorized(EnumPermType.PatientSSNView,true) && textSSN.Text!="") {
					butViewSSN.Visible=true;
				}
			}
			else {
				textSSN.Text=Patients.SSNFormatHelper(_patient.SSN,false);
				//butViewSSN is disabled and not visible from designer.
			}
			textMedicaidID.Text=_patient.MedicaidID;
			textMedicaidState.Text=_ehrPatient.MedicaidState;
			textAddress.Text=_patient.Address;
			textAddress2.Text=_patient.Address2;
			textCity.Text=_patient.City;
			textState.Text=_patient.State;
			textCountry.Text=_patient.Country;
			textZip.Text=_patient.Zip;
			textHmPhone.Text=_patient.HmPhone;
			textWkPhone.Text=_patient.WkPhone;
			textWirelessPhone.Text=_patient.WirelessPhone;
			if(_patient.TxtMsgOk==YN.Yes){
				checkTextingY.Checked=true;
			}
			if(_patient.TxtMsgOk==YN.No){
				checkTextingN.Checked=true;
			}
			FillShortCodes(_patient.TxtMsgOk);
			textEmail.Text=_patient.Email;
			textCreditType.Text=_patient.CreditType;
			odDatePickerDateFirstVisit.SetDateTime(_patient.DateFirstVisit);
			if(_patient.AskToArriveEarly>0){
				textAskToArriveEarly.Text=_patient.AskToArriveEarly.ToString();
			}
			textChartNumber.Text=_patient.ChartNumber;
			textEmployer.Text=Employers.GetName(_patient.EmployerNum);
			//textEmploymentNote.Text=PatCur.EmploymentNote;
			_listLanguages=Patients.GetLanguages(_patient);
			comboLanguage.Items.Add(Lan.g(this,"None"));//regardless of how many languages are listed, the first item is "none"
			comboLanguage.SelectedIndex=0;
			for(int i=0;i<_listLanguages.Count;i++) {
				if(_listLanguages[i]=="") {
					continue;
				}
				CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listLanguages[i]);
				if(cultureInfo==null) {//custom language
					comboLanguage.Items.Add(_listLanguages[i]);
				}
				else {
					comboLanguage.Items.Add(cultureInfo.DisplayName);
				}
				if(_patient.Language==_listLanguages[i]) {
					comboLanguage.SelectedIndex=i+1;
				}
			}
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.Add(Lan.g(this,"None"),new FeeSched());
			comboFeeSched.SelectedIndex=0;
			List<FeeSched> listFeeScheds=FeeScheds.GetDeepCopy(false);
			for(int i=0;i<listFeeScheds.Count;i++) {
				if(listFeeScheds[i].IsHidden && listFeeScheds[i].FeeSchedNum!=_patient.FeeSched) {
					continue;//skip hidden fee schedules as long as not assigned to this patient. This will only occur in rare occurrences.
				}
				comboFeeSched.Items.Add(listFeeScheds[i].Description+(listFeeScheds[i].IsHidden ? " "+Lans.g(this,"(Hidden)") : ""),listFeeScheds[i]);
				if(listFeeScheds[i].FeeSchedNum==_patient.FeeSched) {
					comboFeeSched.SelectedIndex=comboFeeSched.Items.Count-1;
				}
			}
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes);
			comboBillType.Items.AddDefs(_listDefsBillingType.FindAll(x => !x.IsHidden || x.DefNum==_patient.BillingType));
			//If a new patient is being created from the select patient window, the BillingType will be the selected clinic's default billing type.
			//If a new patient is being made with the Add button of the Family Module, the BillingType will be the selected patient's billing type.
			//This is determined in the methods that initialize _patientCur and pre-insert it before this form is instantiated.
			comboBillType.SetSelectedDefNum(_patient.BillingType);
			if(comboBillType.SelectedIndex==-1){
				if(comboBillType.Items.Count==0) {
					MsgBox.Show(this,"Error.  All billing types have been hidden.  Please go to Definitions and unhide at least one.");
					DialogResult=DialogResult.Cancel;
					return;
				}
			}
			if(!Security.IsAuthorized(EnumPermType.PatientBillingEdit,true)){
				//labelBillType.Visible=true;
				comboBillType.Enabled=false;
			}
			if(!Security.IsAuthorized(EnumPermType.PatientApptRestrict,true)) {
				checkRestrictSched.Enabled=false;
			}
			if(_patient.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(EnumPermType.ArchivedPatientEdit,false)) {
				butReferredFrom.Enabled=false;
				textReferredFrom.Enabled=false;
				butSave.Enabled=false;
			}
			comboClinic.ClinicNumSelected=_patient.ClinicNum;
			FillCombosProv();
			comboPriProv.SetSelectedProvNum(_patient.PriProv);
			comboSecProv.SetSelectedProvNum(_patient.SecProv);
			if(!Security.IsAuthorized(EnumPermType.PatPriProvEdit,DateTime.MinValue,true,true) && _patient.PriProv>0) {
				//user not allowed to change existing prov.  Warning messages are suppressed here.
				string strToolTip=Lan.g("Security","Not authorized for")+" "+GroupPermissions.GetDesc(EnumPermType.PatPriProvEdit);
				_priProvEditToolTip.SetToolTip(butPickPrimary,strToolTip);
				_priProvEditToolTip.SetToolTip(comboPriProv,strToolTip);
				comboPriProv.Enabled=false;
			}
			switch(_patient.StudentStatus){
				case "N"://non
				case "":
					radioStudentN.Checked=true;
					break;
				case "P"://parttime
					radioStudentP.Checked=true;
					break;
				case "F"://fulltime
					radioStudentF.Checked=true;
					break;
			}
			textSchool.Text=_patient.SchoolName;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSchoolName.Text=Lan.g(this,"Name of School");
				comboCanadianEligibilityCode.Items.Add("0 - Please Choose");
				comboCanadianEligibilityCode.Items.Add("1 - Full-time student");
				comboCanadianEligibilityCode.Items.Add("2 - Disabled");
				comboCanadianEligibilityCode.Items.Add("3 - Disabled student");
				comboCanadianEligibilityCode.Items.Add("4 - Code not applicable");
				comboCanadianEligibilityCode.SelectedIndex=_patient.CanadianEligibilityCode;
			}
			textAddrNotes.Text=_patient.AddrNote;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				tabControlPatInfo.TabPages.Remove(tabPublicHealth);
			}
			if(PrefC.GetBool(PrefName.EasyHideMedicaid)){
				labelMedicaidID.Visible=false;
				labelPutInInsPlan.Visible=false;
				textMedicaidID.Visible=false;
				textMedicaidState.Visible=false;
			}
			if(_isUsingNewRaceFeature) {
				_listPatientRaces=PatientRaces.GetForPatient(_patient.PatNum);
				textRace.Text=PatientRaces.GetRaceDescription(_listPatientRaces);
				textEthnicity.Text=PatientRaces.GetEthnicityDescription(_listPatientRaces);
				comboBoxMultiRace.Visible=false;
				comboEthnicity.Visible=false;
			}
			else {
				textRace.Visible=false;
				textEthnicity.Visible=false;
				butRaceEthnicity.Visible=false;
				List<string> listMultiRaces=Patients.GetMultiRaces();
				comboBoxMultiRace.Items.AddList(listMultiRaces,x => Lan.g("enumPatRace", x.ToString()));
				List<string> listEthnicities=Patients.GetEthinicities();
				comboEthnicity.Items.AddList(listEthnicities,x => Lan.g(this, x.ToString()));
				List<PatientRace> listPatientRaces=PatientRaces.GetForPatient(_patient.PatNum);
				comboEthnicity.SelectedIndex=0;//none
				for(int i=0;i<listPatientRaces.Count;i++) {
					switch(listPatientRaces[i].CdcrecCode) {
						case "2054-5":
							comboBoxMultiRace.SetSelected(1,true);//AfricanAmerican
							break;
						case "1002-5":
							comboBoxMultiRace.SetSelected(2,true);//AmericanIndian
							break;
						case "2028-9":
							comboBoxMultiRace.SetSelected(3,true);//Asian
							break;
						case PatientRace.DECLINE_SPECIFY_RACE_CODE:
							comboBoxMultiRace.SetSelected(4,true);//DeclinedToSpecify
							break;
						case "2076-8":
							comboBoxMultiRace.SetSelected(5,true);//HawaiiOrPacIsland
							break;
						case "2131-1":
							comboBoxMultiRace.SetSelected(6,true);//Other
							break;
						case "2106-3":
							comboBoxMultiRace.SetSelected(7,true);//White
							break;
						case PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE:
							comboEthnicity.SelectedIndex=1;//DeclinedToSpecify
							break;
						case "2186-5":
							comboEthnicity.SelectedIndex=2;//Not Hispanic
							break;
						case "2135-2":
							comboEthnicity.SelectedIndex=3;//Hispanic
							break;
						default:
							if(listPatientRaces[i].IsEthnicity) {
								comboEthnicity.SelectedIndex=0;
							}
							else {
								comboBoxMultiRace.SetSelected(6,true);//Other
							}
							break;
					}
				}
				if(comboBoxMultiRace.SelectedIndices.Count==0) {//no race set
					comboBoxMultiRace.SetSelected(0,true);//Set to none
				}
			}
			textCounty.Text=_patient.County;
			textSite.Text=Sites.GetDescription(_patient.SiteNum);
			string[] stringArrayEnumPatientGrade=Enum.GetNames(typeof(PatientGrade));
			for(int i=0;i<stringArrayEnumPatientGrade.Length;i++){
				comboGradeLevel.Items.Add(Lan.g("enumGrade",stringArrayEnumPatientGrade[i]));
			}
			comboGradeLevel.SelectedIndex=(int)_patient.GradeLevel;
			string[] stringArrayEnumTreatmentUrgency=Enum.GetNames(typeof(TreatmentUrgency));
			for(int i=0;i<stringArrayEnumTreatmentUrgency.Length;i++){
				comboUrgency.Items.Add(Lan.g("enumUrg",stringArrayEnumTreatmentUrgency[i]));
			}
			comboUrgency.SelectedIndex=(int)_patient.Urgency;
			if(_patient.ResponsParty!=0){
				textResponsParty.Text=Patients.GetLim(_patient.ResponsParty).GetNameLF();
			}
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)){
				textTrophyFolder.Text=_patient.TrophyFolder;
			}
			else{
				labelTrophyFolder.Visible=false;
				textTrophyFolder.Visible=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideHospitals)){
				tabControlPatInfo.TabPages.Remove(tabHospitals);
				//textWard.Visible=false;
				//labelWard.Visible=false;
				//textAdmitDate.Visible=false;
				//labelAdmitDate.Visible=false;
			}
			comboContact.Items.AddEnums<ContactMethod>();
			comboConfirm.Items.AddEnums<ContactMethod>();
			comboRecall.Items.AddEnums<ContactMethod>();
			comboContact.SetSelectedEnum(_patient.PreferContactMethod);
			comboConfirm.SetSelectedEnum(_patient.PreferConfirmMethod);
			comboRecall.SetSelectedEnum(_patient.PreferRecallMethod);
			_commOptOut=CommOptOuts.Refresh(_patient.PatNum);
			int idx=1; //Starts at 1 to account for "None"
			comboExcludeECR.Items.Add("None");
			for(int i=0;i<Enum.GetValues(typeof(CommOptOutMode)).Length;i++) {
				comboExcludeECR.Items.Add(((CommOptOutMode)i).GetDescription(),(CommOptOutMode)i);
				if(_commOptOut.IsOptedOut((CommOptOutMode)i,CommOptOutType.All)) {
					comboExcludeECR.SetSelected(idx,true);
				}
				idx++;
			}
			if(comboExcludeECR.SelectedIndices.Count==0) {
				comboExcludeECR.SetSelected(0);
			}
			if(!PrefC.GetBool(PrefName.EasyHideHospitals)) {
				odDatePickerAdmitDate.SetDateTime(_patient.AdmitDate);
				textWard.Text=_patient.Ward;
				odDatePickerDischargeDate.SetDateTime(_ehrPatient.DischargeDate);
			}
			FillReferrals();
			if(HL7Defs.IsExistingHL7Enabled()) {
				if(HL7Defs.GetOneDeepEnabled().ShowDemographics==HL7ShowDemographics.Show) {//If show, then not edit so disable OK button
					butSave.Enabled=false;
				}
			}
			if(PrefC.GetBool(PrefName.ShowFeatureGoogleMaps)) {
				butShowMap.Visible=true;
			}
			_errorProvider.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)
				&& _patient.SuperFamily!=0 //part of a superfamily
				&& _patient.Guarantor==_patient.PatNum) //and one of the guarantors
			{
				checkSuperBilling.Visible=true;
				checkSuperBilling.Checked=_patient.HasSuperBilling;
			}
			//Loop through the SexOrientation enum and display the Description attribute. If the Snomed attribute equals the patient's SexualOrientation, 
			//set that item as the selected index.
			for(int i=0;i<Enum.GetValues(typeof(SexOrientation)).Length;i++) {
				comboSexOrientation.Items.Add(((SexOrientation)i).GetDescription());
				if(_ehrPatient.SexualOrientation==EnumTools.GetAttributeOrDefault<EhrAttribute>((SexOrientation)i).Snomed) {
					comboSexOrientation.SelectedIndex=comboSexOrientation.Items.Count-1;//Make the last added item the selected one
				}
			}
			textSpecifySexOrientation.Text=_ehrPatient.SexualOrientationNote;
			//Loop through the GenderId enum and display the Description attribute. If the Snomed attribute equals the patient's GenderIdentity, 
			//set that item as the selected index.
			for(int i=0;i<Enum.GetValues(typeof(GenderId)).Length;i++) {
				comboGenderIdentity.Items.Add(((GenderId)i).GetDescription());
				if(_ehrPatient.GenderIdentity==EnumTools.GetAttributeOrDefault<EhrAttribute>((GenderId)i).Snomed) {
					comboGenderIdentity.SelectedIndex=comboGenderIdentity.Items.Count-1;//Make the last added item the selected one
				}
			}
			textSpecifyGender.Text=_ehrPatient.GenderIdentityNote;
			SetRequiredFields();
			//Selecting textLName must happen at the end of load to avoid events from accessing class wide variables that have yet to be loaded.
			//This was a bug because calling Select() was firing textBox_Leave which was accessing _listRequiredFields while it was null.
			textLName.Select();
			FillSpecialty();
			FillComboZip();
			_isLoad=false;
			checkBoxSignedTil.Checked=_patient.HasSignedTil;
			comboPreferredPronouns.Items.AddEnums<PronounPreferred>();
			comboPreferredPronouns.SetSelectedEnum(_patientNote.Pronoun);
			if(!PrefC.GetBool(PrefName.ShowPreferredPronounsForPats)) {
				comboPreferredPronouns.Visible=false;
				labelPreferredPronouns.Visible=false;
			}
			Plugins.HookAddCode(this,"FormPatientEdit.Load_end",_patient);
		}

		private void hideButViewBirthDate() {
			butViewBirthdate.Visible=false;
			//butViewBirthdate is not visible from designer.
			//Move age back over since butViewBirthdate is not showing
			LayoutManager.MoveLocation(label20,new Point(label20.Location.X-LayoutManager.Scale(77),label20.Location.Y));
			LayoutManager.MoveLocation(textAge,new Point(textAge.Location.X-LayoutManager.Scale(77),textAge.Location.Y));
		}

		private void butShortCodeOptIn_Click(object sender,EventArgs e) {
			FrmShortCodeOptIn frmShortCodeOptIn=new FrmShortCodeOptIn();
			frmShortCodeOptIn.PatientCur=_patient;
			YN yn=YN.Unknown;
			if(checkTextingY.Checked){
				yn=YN.Yes;
			}
			if(checkTextingN.Checked){
				yn=YN.No;
			}
			frmShortCodeOptIn.ShowDialog();
			if(frmShortCodeOptIn.IsDialogOK) {
				FillShortCodes(yn);
			}
		}

		private void FillShortCodes(YN txtMsgOk) {
			Patient patient=_patient.Copy();
			patient.TxtMsgOk=txtMsgOk;
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(patient)).FirstOrDefault();
			bool isShortCodeInfoNeeded=patComm?.IsPatientShortCodeEligible(patient.ClinicNum)??false;
			butShortCodeOptIn.Visible=isShortCodeInfoNeeded;
			labelApptTexts.Visible=isShortCodeInfoNeeded;
			textOptIn.Visible=isShortCodeInfoNeeded;
			if(isShortCodeInfoNeeded) {
				switch(patient.ShortCodeOptIn) {
					case YN.Yes:
						textOptIn.Text="Yes";
						break;
					case YN.No:
						textOptIn.Text="No";
						break;
					case YN.Unknown:
					default:
						textOptIn.Text="??";
						break;
				}
			}
		}

		private void FillSpecialty() {
			//Get all non-hidden specialties
			comboSpecialty.Items.Clear();
			//Create a dummy specialty of 0 if there no specialties created.
			comboSpecialty.Items.AddDefNone(Lan.g(this,"Unspecified"));
			comboSpecialty.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ClinicSpecialty,true));
			_defLinkPatient=DefLinks.GetOneByFKey(_patient.PatNum,DefLinkType.Patient);
			if(_defLinkPatient==null) {
				comboSpecialty.SetSelectedDefNum(0);
			}
			else{
				comboSpecialty.SetSelectedDefNum(_defLinkPatient.DefNum);
			}
		}

		private void butPickPrimary_Click(object sender,EventArgs e) {
			if(_patient.PriProv>0 && !Security.IsAuthorized(EnumPermType.PatPriProvEdit)) {
				return;
			}
			FrmProviderPick frmProviderPick = new FrmProviderPick(comboPriProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboPriProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboPriProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		private void butPickSecondary_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick = new FrmProviderPick(comboSecProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboSecProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboSecProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			long provNum=comboPriProv.GetSelectedProvNum();
			List<Provider> listProviders=Providers.GetProvsForClinic(comboClinic.ClinicNumSelected);
			comboPriProv.Items.Clear();
			if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				comboPriProv.Items.AddProvNone("Select Provider");
			}
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {//not dental school
				comboPriProv.Items.AddProvsAbbr(listProviders);
			}
			else{
				comboPriProv.Items.AddProvsFull(listProviders);
			}
			comboPriProv.SetSelectedProvNum(provNum);
			provNum=comboSecProv.GetSelectedProvNum();
			comboSecProv.Items.Clear();
			comboSecProv.Items.AddProvNone();
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {//not dental school
				comboSecProv.Items.AddProvsAbbr(listProviders);
			}
			else{
				comboSecProv.Items.AddProvsFull(listProviders);
			}
			comboSecProv.SetSelectedProvNum(provNum);
		}

		private void checkBillProvSame_Click(object sender,EventArgs e) {
			if(checkBillProvSame.Checked //check box has been checked
				&& _family.ListPats.Any(x => x.PatNum!=_patient.PatNum && x.PriProv!=comboPriProv.GetSelectedProvNum()) //a family member has a different PriProv
				&& !Security.IsAuthorized(EnumPermType.PatPriProvEdit)) //user is not authorized to change PriProv, warning message displays
			{
				checkBillProvSame.Checked=false;//uncheck the box
			}
		}

		private void FillComboZip() {
			List<ZipCode> listZipCodes=ZipCodes.GetDeepCopy(true);
			if(!string.IsNullOrWhiteSpace(textZip.Text)) {
				listZipCodes.RemoveAll(x => x.ZipCodeDigits!=textZip.Text);
			}
			comboZip.Items.Clear();
			for(int i=0;i<listZipCodes.Count;i++) {
				comboZip.Items.Add(listZipCodes[i].ZipCodeDigits+" ("+listZipCodes[i].City+")",listZipCodes[i]);
			}
		}

		private void FillGuardians(){
			_listGuardians=Guardians.Refresh(_patient.PatNum);
			listRelationships.Items.Clear();
			listRelationships.Items.AddList(Patients.GetRelationships(_family,_listGuardians),x => x);
		}

		///<summary>Puts an asterisk next to the label and highlights the textbox/listbox/combobox/radiobutton background for all RequiredFields that
		///have their conditions met.</summary>
		private void SetRequiredFields() {
			_isMissingRequiredFields=false;
			bool areAnyConditionsMet=false;
			bool areConditionsMet;
			if(_listRequiredFields==null) {
				_listRequiredFields=RequiredFields.GetRequiredFields();
			}
			for(int i=0;i<_listRequiredFields.Count;i++) {
				areConditionsMet=ConditionsAreMet(_listRequiredFields[i]);
				if(areConditionsMet) {
					areAnyConditionsMet=true;
					labelRequiredField.Visible=true;
				}
				switch(_listRequiredFields[i].FieldName) {
					case RequiredFieldName.Address:
						SetRequiredTextBox(labelAddress,textAddress,areConditionsMet);
						break;
					case RequiredFieldName.Address2:
						SetRequiredTextBox(labelAddress2,textAddress2,areConditionsMet);
						break;
					case RequiredFieldName.AddressPhoneNotes:
						if(areConditionsMet) {
							if(!groupNotes.Text.Contains("*")) {
								groupNotes.Text+="*";
							}
							if(textAddrNotes.Text=="") {
								_isMissingRequiredFields=true;
								if(_isValidating) {
									_errorProvider.SetError(textAddrNotes,Lan.g(this,"Text box cannot be blank"));
								}
							}
							else {
								_errorProvider.SetError(textAddrNotes,"");
							}
						}
						else {
							if(groupNotes.Text.Contains("*")) {
								groupNotes.Text=groupNotes.Text.Replace("*","");
							}
							_errorProvider.SetError(textAddrNotes,"");
						}
						break;
					case RequiredFieldName.AdmitDate:
						SetRequiredControl(labelAdmitDate,odDatePickerAdmitDate,areConditionsMet,-1,new List<int>(),"Hospital admit date must be selected.");
;						break;
					case RequiredFieldName.AskArriveEarly:
						SetRequiredTextBox(labelAskToArriveEarly,textAskToArriveEarly,areConditionsMet);
						break;
					case RequiredFieldName.BillingType:
						SetRequiredComboBoxOD(labelBillType,comboBillType,areConditionsMet,-1,"A billing type must be selected.");
						break;
					case RequiredFieldName.Birthdate:
						SetRequiredControl(labelBirthdate,odDatePickerBirthDate,areConditionsMet,-1,new List<int>(),"birthdate must be selected.");
						break;
					case RequiredFieldName.ChartNumber:
						SetRequiredTextBox(labelChartNumber,textChartNumber,areConditionsMet);
						break;
					case RequiredFieldName.City:
						SetRequiredTextBox(labelCity,textCity,areConditionsMet);
						break;
					case RequiredFieldName.Clinic:
						SetRequiredClinic(areConditionsMet);
						//SetRequiredComboBox(labelClinic,comboClinic,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						break;
					case RequiredFieldName.CollegeName:
						SetRequiredTextBox(labelSchoolName,textSchool,areConditionsMet);
						break;
					case RequiredFieldName.County:
						SetRequiredTextBox(labelCounty,textCounty,areConditionsMet);
						break;
					case RequiredFieldName.CreditType:
						SetRequiredTextBox(labelCreditType,textCreditType,areConditionsMet);
						break;
					case RequiredFieldName.DateFirstVisit:
						SetRequiredControl(labelDateFirstVisit,odDatePickerDateFirstVisit,areConditionsMet,-1,new List<int>(),"Date First Visit must be selected.");
						break;
					case RequiredFieldName.DateTimeDeceased:
						SetRequiredControl(labelDeceased,textDateTimeDeceased,areConditionsMet,-1,new List<int>(),"Date-Time deceased must be entered.");
						break;
					case RequiredFieldName.DischargeDate:
						SetRequiredControl(labelDischargeDate,odDatePickerDischargeDate,areConditionsMet,-1,new List<int>(),"Hospital discharge date must be selected.");
						break;
					case RequiredFieldName.EligibilityExceptCode:
						SetRequiredComboBoxOD(labelCanadianEligibilityCode,comboCanadianEligibilityCode,areConditionsMet,0,
							"Selection cannot be '0 - Please Choose'.");
						break;
					case RequiredFieldName.EmailAddress:
						SetRequiredTextBox(labelEmail,textEmail,areConditionsMet);
						break;
					case RequiredFieldName.EmergencyName:
						SetRequiredTextBox(labelEmergencyName,textIceName,areConditionsMet);
						break;
					case RequiredFieldName.EmergencyPhone:
						SetRequiredTextBox(labelEmergencyPhone,textIcePhone,areConditionsMet);
						break;
					case RequiredFieldName.Employer:
						SetRequiredTextBox(labelEmployer,textEmployer,areConditionsMet);
						break;
					case RequiredFieldName.Ethnicity:
						//If the EHR Feature is turned off, process an Ethnicity Textbox, else process a ComboBox
						if (_isUsingNewRaceFeature){
							SetRequiredTextBox(labelEthnicity,textEthnicity,areConditionsMet);
						}
						else{
							SetRequiredComboBoxOD(labelEthnicity,comboEthnicity,areConditionsMet,0,"Selection cannot be 'None'.");
						}
						break;
					case RequiredFieldName.FeeSchedule:
						SetRequiredComboBoxOD(labelFeeSched,comboFeeSched,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.FirstName:
						SetRequiredTextBox(labelFName,textFName,areConditionsMet);
						break;
					case RequiredFieldName.Gender:
						labelGender.Text=labelGender.Text.Replace("*","");
						if(areConditionsMet) {
							labelGender.Text=labelGender.Text+"*";
							if(listGender.SelectedIndex>-1 && listGender.Items.GetTextShowingAt(listGender.SelectedIndex)==Lan.g(this,"Unknown")) {
								if(_isValidating) {
									_errorProvider.SetError(listGender,Lan.g(this,"Gender cannot be 'Unknown'."));
									_errorProvider.SetIconAlignment(listGender,ErrorIconAlignment.BottomRight);
								}
								_isMissingRequiredFields=true;
							}
							else {
								_errorProvider.SetError(listGender,"");
							}
						}
						else {
							_errorProvider.SetError(listGender,"");
						}
						break;
					case RequiredFieldName.GenderIdentity:
						SetRequiredComboBoxOD(labelGenderIdentity,comboGenderIdentity,areConditionsMet,-1,"A gender identity must be selected.");
						break;
					case RequiredFieldName.GradeLevel:
						SetRequiredComboBoxOD(labelGradeLevel,comboGradeLevel,areConditionsMet,0,"Selection cannot be 'Unknown'.");
						break;
					case RequiredFieldName.HomePhone:
						SetRequiredTextBox(labelHmPhone,textHmPhone,areConditionsMet);
						break;
					case RequiredFieldName.Language:
						SetRequiredComboBoxOD(labelLanguage,comboLanguage,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.LastName:
						SetRequiredTextBox(labelLName,textLName,areConditionsMet);
						break;
					case RequiredFieldName.MedicaidID:
						SetRequiredTextBox(labelMedicaidID,textMedicaidID,areConditionsMet);						
						break;
					case RequiredFieldName.MedicaidState:
						SetRequiredTextBox(labelMedicaidID,textMedicaidState,areConditionsMet);
						if(textMedicaidState.Text!=""	&& !StateAbbrs.IsValidAbbr(textMedicaidState.Text)) {
							_isMissingRequiredFields=true;
							if(_isValidating) {
								_errorProvider.SetError(textMedicaidState,Lan.g(this,"Invalid state abbreviation"));
							}
						}
						CheckMedicaidIDLength();
						break;
					case RequiredFieldName.MiddleInitial:
						SetRequiredTextBox(labelPreferredAndMiddleI,textMiddleI,areConditionsMet);
						break;
					case RequiredFieldName.MothersMaidenFirstName:
						SetRequiredTextBox(labelMotherMaidenFname,textMotherMaidenFname,areConditionsMet);
						break;
					case RequiredFieldName.MothersMaidenLastName:
						SetRequiredTextBox(labelMotherMaidenLname,textMotherMaidenLname,areConditionsMet);
						break;
					case RequiredFieldName.PreferConfirmMethod:
						SetRequiredComboBoxOD(labelConfirm,comboConfirm,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferContactMethod:
						SetRequiredComboBoxOD(labelContact,comboContact,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferRecallMethod:
						SetRequiredComboBoxOD(labelRecall,comboRecall,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferredName:
						SetRequiredTextBox(labelPreferredAndMiddleI,textPreferred,areConditionsMet);
						break;
					case RequiredFieldName.PrimaryProvider:
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							SetRequiredComboBoxOD(labelPriProv,comboPriProv,areConditionsMet,0,"Selection cannot be 'Select Provider'.");
						}
						break;
					case RequiredFieldName.Race:
						if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							SetRequiredComboBoxOD(labelRace,comboBoxMultiRace,areConditionsMet,new List<int> {0},"Race is required");
						}
						else {
							SetRequiredTextBox(labelRace,textRace,areConditionsMet);
						}
						break;
					case RequiredFieldName.ReferredFrom:
						SetRequiredTextBox(labelReferredFrom,textReferredFrom,areConditionsMet);
						break;
					case RequiredFieldName.ResponsibleParty:
						SetRequiredTextBox(labelResponsParty,textResponsParty,areConditionsMet);
						break;
					case RequiredFieldName.SexualOrientation:
						SetRequiredComboBoxOD(labelSexOrientation,comboSexOrientation,areConditionsMet,-1,"A sexual orientation must be selected.");
						break;
					case RequiredFieldName.Salutation:
						SetRequiredTextBox(labelSalutation,textSalutation,areConditionsMet);
						break;
					case RequiredFieldName.SecondaryProvider:
						SetRequiredComboBoxOD(labelSecProv,comboSecProv,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.Site:
						SetRequiredTextBox(labelSite,textSite,areConditionsMet);
						break;
					case RequiredFieldName.SocialSecurityNumber:
						SetRequiredTextBox(labelSSN,textSSN,areConditionsMet);
					break;
					case RequiredFieldName.State:
						SetRequiredTextBox(labelST,textState,areConditionsMet);
						if(textState.Text!=""	&& !StateAbbrs.IsValidAbbr(textState.Text)) {
							_isMissingRequiredFields=true;
							if(_isValidating) {
								_errorProvider.SetError(textState,Lan.g(this,"Invalid state abbreviation"));
							}
						}
						break;
					case RequiredFieldName.StudentStatus:
						radioStudentN.Text=radioStudentN.Text.Replace("*","");
						radioStudentF.Text=radioStudentF.Text.Replace("*","");
						radioStudentP.Text=radioStudentP.Text.Replace("*","");
						if(areConditionsMet) {
							radioStudentN.Text=radioStudentN.Text+"*";
							radioStudentF.Text=radioStudentF.Text+"*";
							radioStudentP.Text=radioStudentP.Text+"*";
							if(!radioStudentN.Checked && !radioStudentF.Checked && !radioStudentP.Checked) {
								_isMissingRequiredFields=true;
								if(_isValidating) {
									_errorProvider.SetError(radioStudentP,"A student status must be selected.");
								}
							}
							else {
								_errorProvider.SetError(radioStudentP,"");
							}
						}
						else {
							_errorProvider.SetError(radioStudentP,"");
						}
						break;
					case RequiredFieldName.TextOK:
						labelTextOk.Text=labelTextOk.Text.Replace("*","");						
						if(areConditionsMet) {
							labelTextOk.Text=labelTextOk.Text+"*";
							if(checkTextingY.Checked || checkTextingN.Checked) {
								_errorProvider.SetError(checkTextingN,"");
							}
							else{
								_isMissingRequiredFields=true;
								if(_isValidating) {
									_errorProvider.SetError(checkTextingN,Lan.g(this,"Either Y or N must be selected."));
								}
							}
						}
						else {
							_errorProvider.SetError(checkTextingN,"");
						}
						break;
					case RequiredFieldName.Title:
						SetRequiredTextBox(labelTitle,textTitle,areConditionsMet);
						break;
					case RequiredFieldName.TreatmentUrgency:
						SetRequiredComboBoxOD(labelUrgency,comboUrgency,areConditionsMet,0,"Selection cannot be 'Unknown'.");
						break;
					case RequiredFieldName.TrophyFolder:
						SetRequiredTextBox(labelTrophyFolder,textTrophyFolder,areConditionsMet);
						break;
					case RequiredFieldName.Ward:
						SetRequiredTextBox(labelWard,textWard,areConditionsMet);
						break;
					case RequiredFieldName.WirelessPhone:
						SetRequiredTextBox(labelWirelessPhone,textWirelessPhone,areConditionsMet);
						break;
					case RequiredFieldName.WorkPhone:
						SetRequiredTextBox(labelWkPhone,textWkPhone,areConditionsMet);
						break;
					case RequiredFieldName.Zip:
						SetRequiredTextBox(labelZip,textZip,areConditionsMet);
						break;
				}
			}
			if(!areAnyConditionsMet) {
				labelRequiredField.Visible=false;
			}
			YN yNTxtMsgOk=YN.Unknown;
			if(checkTextingY.Checked){
				yNTxtMsgOk=YN.Yes;
			}
			if(checkTextingN.Checked){
				yNTxtMsgOk=YN.No;
			}
			FillShortCodes(yNTxtMsgOk);
		}

		///<summary>Returns true if all the conditions for the RequiredField are met.</summary>
		private bool ConditionsAreMet(RequiredField requiredField) {
			List<RequiredFieldCondition> listRequiredFieldConditions=requiredField.ListRequiredFieldConditions;
			if(listRequiredFieldConditions.Count==0) {//This RequiredField is always required
				return true;
			}
			bool areConditionsMet=false;
			int previousFieldName=-1;
			for(int i=0;i<listRequiredFieldConditions.Count;i++) {
				if(areConditionsMet && (int)listRequiredFieldConditions[i].ConditionType==previousFieldName) {
					continue;//A condition of this type has already been met
				}
				if(!areConditionsMet && previousFieldName!=-1
					&& (int)listRequiredFieldConditions[i].ConditionType!=previousFieldName) 
				{
					return false;//None of the conditions of the previous type were met
				}
				areConditionsMet=false;
				switch(listRequiredFieldConditions[i].ConditionType) {
					case RequiredFieldName.AdmitDate:
						areConditionsMet=RequiredFieldConditions.CheckDateConditions(odDatePickerAdmitDate.GetDateTime().ToString(),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.BillingType:
						//Conditions of type BillingType store the DefNum as the ConditionValue.
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(comboBillType.GetSelectedDefNum().ToString(),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Birthdate://But actually using Age for calculations						
						if(textAge.Text=="") {
							areConditionsMet=false;
							break;
						}
						DateTime dateBirth=DateTime.MinValue;
						if(!_isBirthdayMasked) { //if not masked
							if(!IsBirthdateValid()) {  
								areConditionsMet=false;
								break;
							}
							dateBirth=odDatePickerBirthDate.GetDateTime();
						}
						else {//birthdate is masked
							dateBirth=_patientOld.Birthdate;
						}
						int ageEntered=DateTime.Today.Year-dateBirth.Year;
						if(dateBirth>DateTime.Today.AddYears(-ageEntered)) {
							ageEntered--;
						}
						List<RequiredFieldCondition> listRequiredFieldConditionsAge=listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Birthdate);
						//There should be no more than 2 conditions of type Birthdate
						List<bool> listAreCondsMet=new List<bool>();
						for(int j=0;j<listRequiredFieldConditionsAge.Count;j++) {
							listAreCondsMet.Add(RequiredFieldConditions.CondOpComparer(ageEntered,listRequiredFieldConditionsAge[j].Operator,PIn.Int(listRequiredFieldConditionsAge[j].ConditionValue)));
						}
						if(listAreCondsMet.Count<2 || listRequiredFieldConditionsAge[1].ConditionRelationship==LogicalOperator.And) {
							areConditionsMet=!listAreCondsMet.Contains(false);
							break;
						}
						areConditionsMet=listAreCondsMet.Contains(true);
						break;
					case RequiredFieldName.Clinic:
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(comboClinic.ClinicNumSelected.ToString(),i,listRequiredFieldConditions);//includes none clinic
						break;								
					case RequiredFieldName.DateTimeDeceased:
						areConditionsMet=RequiredFieldConditions.CheckDateConditions(textDateTimeDeceased.Text,i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.DischargeDate:
						areConditionsMet=RequiredFieldConditions.CheckDateConditions(odDatePickerDischargeDate.GetDateTime().ToString(),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Gender:
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(listGender.Items.GetTextShowingAt(listGender.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Language:
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(comboLanguage.Items.GetTextShowingAt(comboLanguage.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.MedicaidID:
						areConditionsMet=RequiredFieldConditions.CheckMedicaidConditions(textMedicaidID.Text,i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.MedicaidState:
						areConditionsMet=RequiredFieldConditions.CheckMedicaidConditions(textMedicaidState.Text,i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.PatientStatus:
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(listStatus.Items.GetTextShowingAt(listStatus.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Position:
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(listPosition.Items.GetTextShowingAt(listPosition.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.PrimaryProvider:
						//Conditions of type PrimaryProvider store the ProvNum as the ConditionValue.
						areConditionsMet=RequiredFieldConditions.ConditionComparerHelper(comboPriProv.GetSelectedProvNum().ToString(),i,listRequiredFieldConditions);
						break;							
					case RequiredFieldName.StudentStatus:
						areConditionsMet=RequiredFieldConditions.CheckStudentStatusConditions(i,listRequiredFieldConditions,radioStudentN.Checked,radioStudentF.Checked,radioStudentP.Checked);
						break;
				}
				previousFieldName=(int)listRequiredFieldConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		///<summary>Checks to see if the Medicaid ID is the proper number of digits for the Medicaid State.</summary>
		private void CheckMedicaidIDLength() {
			string reqLength=RequiredFieldConditions.CheckMedicaidIDLength(textMedicaidState.Text,textMedicaidID.Text);
			if(reqLength.IsNullOrEmpty()) {
				_errorProvider.SetError(textMedicaidID,"");
				return;
			}
			_isMissingRequiredFields=true;
			if(_isValidating) {
				_errorProvider.SetError(textMedicaidID,Lan.g(this,"Medicaid ID length must be ")+reqLength+Lan.g(this," digits for the state of ")
					+textMedicaidState.Text+".");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it also blank, sets the error provider.
		///</summary>
		private void SetRequiredTextBox(Label label,System.Windows.Forms.TextBox textBox,bool areConditionsMet) {
			SetRequiredControl(label,textBox,areConditionsMet,-1,new List<int>(),"Text box cannot be blank.");
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///sets the error provider.</summary>
		private void SetRequiredComboBoxOD(Label label,UI.ComboBox comboBox,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			SetRequiredControl(label,comboBox,areConditionsMet,disallowedIdx,new List<int>(),errorMsg);
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If a disallowedIndices is also selected, sets the error provider.</summary>
		private void SetRequiredComboBoxOD(Label label,UI.ComboBox comboBox,bool areConditionsMet,List<int> listDisallowedIndices,string errorMsg) {
			SetRequiredControl(label,comboBox,areConditionsMet,-1,listDisallowedIndices,errorMsg);
		}	

		private void SetRequiredControl(Label label,Control control,bool areConditionsMet,int disallowedIdx,List<int> listDisallowedIndices,
			string errorMsg) 
		{
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				// masked dates show up as "xx/xx/xxxx" but can still be blank. If it is blank, we still want to trigger the required fields. 
				// Birthdate field is not the only ODDatePicker field, so we have to treat it differently. GetDateTime() will return DateTime.MinValue if blank
				bool showBirthdate=false;
				if(control is ODDatePicker odDatePicker) { 
					if(_isBirthdayMasked && control.Name=="odDatePickerBirthDate" && !odDatePicker.IsEmptyDateTime()) {
						showBirthdate=false; // Birthdate is masked and has a value, don't trigger required field.
					}
					else {
						showBirthdate=odDatePicker.GetDateTime() == DateTime.MinValue;
					}
				}
				if((control is UI.ComboBox && ((UI.ComboBox)control).SelectionModeMulti && ((UI.ComboBox)control).SelectedIndices.Exists(x => listDisallowedIndices.Exists(y => y==x)))
					|| (control is UI.ComboBox && !((UI.ComboBox)control).SelectionModeMulti && ((UI.ComboBox)control).SelectedIndex==disallowedIdx)
					|| (control is System.Windows.Forms.TextBox && ((System.Windows.Forms.TextBox)control).Text=="")
					|| (control is ODDatePicker && showBirthdate) 
				) 
				{
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(control,Lan.g(this,errorMsg));
					}
				}
				else {
					_errorProvider.SetError(control,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","")+"";
				_errorProvider.SetError(control,"");
			}
			if(control.Name==textSite.Name || control.Name==textReferredFrom.Name) {
				_errorProvider.SetIconPadding(control,25);//Width of the pick button
			}
			else if(control.Name==textResponsParty.Name) {
				_errorProvider.SetIconPadding(control,50);//Width of the pick and remove buttons
			}
			else if(control.Name==comboPriProv.Name || control.Name==comboSecProv.Name) {
				_errorProvider.SetIconPadding(control,25);//Width of the pick button
			}
			//If the control is on a tab, add an asterisk to the tab title.
			Control parent=label.Parent;
			while(parent!=null) {
				if(parent is UI.TabPage tabPage) {
					tabPage.Text=tabPage.Text.Replace("*","")+"*";
				}
				parent=parent.Parent;
			}
		}

		private void SetRequiredClinic(bool areConditionsMet){
			//no easy way to show clinic is required. We don't add and remove star from label.
			if(areConditionsMet) {
				if(comboClinic.ClinicNumSelected==0){
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(comboClinic,Lan.g(this,"Selection cannot be 'Unassigned'."));
					}
				}
				else {
					_errorProvider.SetError(comboClinic,"");
				}
			}
			else {
				_errorProvider.SetError(comboClinic,"");
			}
		}
		
		private void textBox_Leave(object sender,System.EventArgs e) {
			SetRequiredFields();
		}

		///<summary>TextZip needs it's own event to fill zip codes in the case where the zip code field is left blank.</summary>
		private void textZip_Leave(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(textZip.Text)) {
				FillComboZip();
			}
			SetRequiredFields();
		}
		
		///<summary>Status and Gender</summary>
		private void ListBox_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(_isLoad) {
				return;
			}
			SetRequiredFields();
			Plugins.HookAddCode(this,"FormPatientEdit.ListBox_SelectedIndexChanged_end",_patient);
		}

		private void checkTextingN_Click(object sender,EventArgs e){
			if(checkTextingN.Checked && checkTextingY.Checked){
				checkTextingY.Checked = false;
			}
			SetRequiredFields();
		}

		private void checkTextingY_Click(object sender,EventArgs e){
			if(checkTextingN.Checked && checkTextingY.Checked){
				checkTextingN.Checked = false;
			}
			SetRequiredFields();
		}

		private void ComboBox_SelectionChangeCommited(object sender,System.EventArgs e) {
			SetRequiredFields();
			if(sender!=comboClinic){
				return;
			}
			Def defBillingTypeSelected=comboBillType.GetSelected<Def>();
			long defNumClinicDefaultBillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,comboClinic.ClinicNumSelected));
			//If patient is new and the selected billing type is not the selected clinics default billing type.
			if(IsNew && defBillingTypeSelected.DefNum!=defNumClinicDefaultBillingType) {
				string clinicDefaultBillingTypeName=_listDefsBillingType.Find(x => x.DefNum==defNumClinicDefaultBillingType)?.ItemName;
				//Ask if the user would like to set the patients billing type to the clinics default.
				if(!clinicDefaultBillingTypeName.IsNullOrEmpty() && MessageBox.Show(Lan.g(this,"The selected billing type does not match the selected clinic's default billing type. "+
						"Would you like to change this patient's billing type from ")+defBillingTypeSelected.ItemName+" to "+clinicDefaultBillingTypeName+"?","",
						MessageBoxButtons.YesNo)==DialogResult.Yes)
				{
					comboBillType.SetSelectedDefNum(defNumClinicDefaultBillingType); //Set to clinics default.
				}
			}
			FillCombosProv();
		}

		//private void butSecClear_Click(object sender, System.EventArgs e) {
		//	listSecProv.SelectedIndex=-1;
		//}

		//private void butClearFee_Click(object sender, System.EventArgs e) {
		//	listFeeSched.SelectedIndex=-1;
		//}

		private void textLName_TextChanged(object sender, System.EventArgs e) {
			if(textLName.Text.Length==1){
				textLName.Text=textLName.Text.ToUpper();
				textLName.SelectionStart=1;
			}
		}

		private void textFName_TextChanged(object sender, System.EventArgs e) {
			if(textFName.Text.Length==1){
				textFName.Text=textFName.Text.ToUpper();
				textFName.SelectionStart=1;
			}
		}

		private void textMiddleI_TextChanged(object sender, System.EventArgs e) {
			if(textMiddleI.Text.Length==1){
				textMiddleI.Text=textMiddleI.Text.ToUpper();
				textMiddleI.SelectionStart=1;
			}
		}

		private void textPreferred_TextChanged(object sender, System.EventArgs e) {
			if(textPreferred.Text.Length==1){
				textPreferred.Text=textPreferred.Text.ToUpper();
				textPreferred.SelectionStart=1;
			}
		}

		private void textSalutation_TextChanged(object sender, System.EventArgs e) {
			if(textSalutation.Text.Length==1){
				textSalutation.Text=textSalutation.Text.ToUpper();
				textSalutation.SelectionStart=1;
			}
		}

		private void textAddress_TextChanged(object sender, System.EventArgs e) {
			if(textAddress.Text.Length==1){
				textAddress.Text=textAddress.Text.ToUpper();
				textAddress.SelectionStart=1;
			}
		}

		private void textAddress2_TextChanged(object sender, System.EventArgs e) {
			if(textAddress2.Text.Length==1){
				textAddress2.Text=textAddress2.Text.ToUpper();
				textAddress2.SelectionStart=1;
			}
		}

		private void radioStudentN_Click(object sender, System.EventArgs e) {
			_patient.StudentStatus="N";
			SetRequiredFields();
		}

		private void radioStudentF_Click(object sender, System.EventArgs e) {
			_patient.StudentStatus="F";
			SetRequiredFields();
		}

		private void radioStudentP_Click(object sender, System.EventArgs e) {
			_patient.StudentStatus="P";
			SetRequiredFields();
		}

		private void textSSN_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(CultureInfo.CurrentCulture.Name!="en-US"){
				return;
			}
			//only reformats if in USA and exactly 9 digits.
			if(textSSN.Text==""){
				return;
			}
			if(PrefC.GetBool(PrefName.PatientSSNMasked) || !Security.IsAuthorized(EnumPermType.PatientSSNView, true)) {
				if(textSSN.Text==_maskedSSNOld) {//If SSN hasn't changed, don't validate.  It is masked.
					return;
				}
			}
			if(!Regex.IsMatch(textSSN.Text,@"^\d{9}$") && !Regex.IsMatch(textSSN.Text,@"^\d{3}-\d{2}-\d{4}$")) {
				MsgBox.Show("Patient's Social Security Number is invalid.");
				_errorProvider.SetError(textSSN, "Invalid social security number.");
				return;
			}
			if(textSSN.Text.Length==9){//if just numbers, try to reformat.
				bool isValidSSN=true;
				for(int i=0;i<textSSN.Text.Length;i++){
					if(!Char.IsNumber(textSSN.Text,i)){
						isValidSSN=false;
					}
				}
				if(isValidSSN){
					textSSN.Text=textSSN.Text.Substring(0,3)+"-"
						+textSSN.Text.Substring(3,2)+"-"+textSSN.Text.Substring(5,4);	
				}
			}
			//Regular expression checking for US ###-##-#### moved to OK click.	
		}

		private void textCreditType_TextChanged(object sender, System.EventArgs e) {
			textCreditType.Text=textCreditType.Text.ToUpper();
			textCreditType.SelectionStart=1;
		}

		private void odDatePickerBirthDate_Validated(object sender, System.EventArgs e) {//Does fire even though textBox is read only.
			CalcAge();
		}

		private void textDateTimeDeceased_Validated(object sender,EventArgs e) {
			CalcAge();
		}

		private bool IsBirthdateValid() {
			//We do it this way to maintain the very useful behavior where it adds /'s for you when leaving.
			//But this will not be needed when moving to wpf, because WpfControls.UI.DatePicker already does this.
			if(odDatePickerBirthDate.IsValid()) {
				return true;
			}
			return false;
		}

		private void CalcAge() {
			if(_isBirthdayMasked) {//showing xx/xx/xxxx
				return;
			}
			textAge.Text="";
			if(odDatePickerBirthDate.IsEmptyDateTime()) {
				return;
			}
			if(!IsBirthdateValid()) {
				MsgBox.Show(this,"Patient's Birthdate is not a valid or allowed date.");
				_errorProvider.SetError(odDatePickerBirthDate,"Valid dates between 1880 and 2100.");
				return;
			}
			DateTime dateBirth=odDatePickerBirthDate.GetDateTime();
			DateTime dateTimeTo=DateTime.Now;
			if(!string.IsNullOrWhiteSpace(textDateTimeDeceased.Text)) { 
				try {
					dateTimeTo=DateTime.Parse(textDateTimeDeceased.Text);
				}
				catch {
					return;
				}
			}
			textAge.Text=PatientLogic.DateToAgeString(dateBirth,dateTimeTo);
			SetRequiredFields();
		}

		private void textZip_TextChanged(object sender, System.EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender, System.EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			ZipCode zipCodeSelected=comboZip.GetSelected<ZipCode>();
			if(zipCodeSelected==null) {//if somehow nothing is selected. Can happen if selecting something and draging up outside of the combobox.
				return;
			}
			textCity.Text=zipCodeSelected.City;
			textState.Text=zipCodeSelected.State;
			textZip.Text=zipCodeSelected.ZipCodeDigits;
			SetRequiredFields();
		}

		private void textZip_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			//fired as soon as control loses focus.
			//it's here to validate if zip is typed in to text box instead of picked from list.
			//if(textZip.Text=="" && (textCity.Text!="" || textState.Text!="")){
			//	if(MessageBox.Show(Lan.g(this,"Delete the City and State?"),"",MessageBoxButtons.OKCancel)
			//		==DialogResult.OK){
			//		textCity.Text="";
			//		textState.Text="";
			//	}	
			//	return;
			//}
			if(textZip.Text.Length<5){
				return;
			}
			if(comboZip.SelectedIndex!=-1){
				return;
			}
			//the autofill only works if both city and state are left blank
			if(textCity.Text!="" || textState.Text!=""){
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0){
				//No match found. Must enter info for new zipcode
				ZipCode zipCode=new ZipCode();
				zipCode.ZipCodeDigits=textZip.Text;
				FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
				frmZipCodeEdit.ZipCodeCur=zipCode;
				frmZipCodeEdit.IsNew=true;
				frmZipCodeEdit.ShowDialog();
				if(!frmZipCodeEdit.IsDialogOK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				textCity.Text=zipCode.City;
				textState.Text=zipCode.State;
				textZip.Text=zipCode.ZipCodeDigits;
			}
			else if(listZipCodes.Count==1){
				//only one match found.  Use it.
				textCity.Text=listZipCodes[0].City;
				textState.Text=listZipCodes[0].State;
			}
			else{
				//multiple matches found.  Pick one
				FrmZipSelect frmZipSelect=new FrmZipSelect();
				frmZipSelect.ZipCodeDigits=textZip.Text;
				frmZipSelect.ShowDialog();
				if(!frmZipSelect.IsDialogOK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=frmZipSelect.ZipCodeSelected.City;
				textState.Text=frmZipSelect.ZipCodeSelected.State;
				textZip.Text=frmZipSelect.ZipCodeSelected.ZipCodeDigits;
			}
			FillComboZip();
			SetRequiredFields();
		}

		private void butEditZip_Click(object sender, System.EventArgs e) {
			if(textZip.Text.Length==0){
				MessageBox.Show(Lan.g(this,"Please enter a zipcode first."));
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0){
				FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
				frmZipCodeEdit.ZipCodeCur=new ZipCode();
				frmZipCodeEdit.ZipCodeCur.ZipCodeDigits=textZip.Text;
				frmZipCodeEdit.IsNew=true;
				frmZipCodeEdit.ShowDialog();
				if(!frmZipCodeEdit.IsDialogOK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=frmZipCodeEdit.ZipCodeCur.City;
				textState.Text=frmZipCodeEdit.ZipCodeCur.State;
				textZip.Text=frmZipCodeEdit.ZipCodeCur.ZipCodeDigits;
				FillComboZip();
				return;
			}
			FrmZipSelect frmZipSelect=new FrmZipSelect();
			frmZipSelect.ZipCodeDigits=textZip.Text;
			frmZipSelect.ShowDialog();
			if(!frmZipSelect.IsDialogOK){
				return;
			}
			//Not needed:
			//DataValid.SetInvalid(InvalidTypes.ZipCodes);
			textCity.Text=frmZipSelect.ZipCodeSelected.City;
			textState.Text=frmZipSelect.ZipCodeSelected.State;
			textZip.Text=frmZipSelect.ZipCodeSelected.ZipCodeDigits;
			FillComboZip();
		}

		private void butShowMap_Click(object sender,EventArgs e) {
			if(textAddress.Text=="" 
				|| textCity.Text=="" 
				|| textState.Text=="") 
			{
				MsgBox.Show(this,"Please fill in Address, City, and ST before using maps.");
				return;
			}
			try {
				Process.Start("http://maps.google.com/maps?t=m&q="+textAddress.Text+" "+textAddress2.Text+" "+textCity.Text+" "+textState.Text);
			}
			catch {
				MsgBox.Show(this,"Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again.");
			}
		}

		///<summary>All text boxes on this form that accept a phone number use this text changed event.</summary>
		private void textAnyPhoneNumber_TextChanged(object sender,System.EventArgs e) {
			if(sender.GetType()!=typeof(ValidPhone)) {
				return;
			}
			Plugins.HookAddCode(sender,"FormPatientEdit.textAnyPhoneNumber_TextChanged_end");
		}

		///<summary>Offers the user, if necessary, the opportunity to send a text message to the patient when changes have been made to texting settings.</summary>
		private void PromptForSmsIfNecessary(Patient patientOriginal,Patient patientNew) {
			if(!Patients.DoPromptForSms(patientOriginal,patientNew)) {
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Texting settings have changed.  Would you like to send a message now?","Send a message?")) {
				string message=PrefC.GetString(PrefName.ShortCodeOptInScript);
				message=FrmShortCodeOptIn.FillInTextTemplate(message,patientNew);
				FormOpenDental.S_TxtMsg_Click(patientNew.PatNum,message);
			}	
		}

		private void butAuto_Click(object sender, System.EventArgs e) {
			try {
				textChartNumber.Text=Patients.GetNextChartNum();
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			_errorProvider.SetError(textChartNumber,"");
		}

		private void textCity_TextChanged(object sender, System.EventArgs e) {
			if(textCity.Text.Length==1){
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender, System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textState.Text.Length==1 || textState.Text.Length==2){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=2;
				}
				return;
			}
			if(textState.Text.Length==1){
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=1;
			}
		}

		private void textMedicaidState_TextChanged(object sender,EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) //Canadian. en-CA or fr-CA
			{
				if(textMedicaidState.Text.Length==1 || textMedicaidState.Text.Length==2) {
					textMedicaidState.Text=textMedicaidState.Text.ToUpper();
					textMedicaidState.SelectionStart=2;
				}
			}
			else {
				if(textMedicaidState.Text.Length==1) {
					textMedicaidState.Text=textMedicaidState.Text.ToUpper();
					textMedicaidState.SelectionStart=1;
				}
			}
		}

		///<summary>Validates there is still a last name entered and updates the last, first, middle and preferred name of PatCur to what is currently
		///typed in the text boxes.  May not match what is in the database.  Does not save the changes to the database so user can safely cancel and
		///revert the changes.</summary>
		private void UpdateLocalNameHelper() {
			if(textLName.Text=="") {
				MsgBox.Show(this,"Last Name must be entered.");
				return;
			}
			_patient.LName=textLName.Text;
			_patient.FName=textFName.Text;
			_patient.MiddleI=textMiddleI.Text;
			_patient.Preferred=textPreferred.Text;
			for(int i=0;i<_family.ListPats.Length;i++) {//update the Family object as well
				if(_family.ListPats[i].PatNum==_patient.PatNum) {
					_family.ListPats[i]=_patient.Copy();
					break;
				}
			}
			if(_patient.ResponsParty==_patient.PatNum) {
				textResponsParty.Text=_patient.GetNameLF();
			}
			for(int i=0;i<_listGuardians.Count;i++) {
				if(_listGuardians[i].PatNumGuardian!=_patient.PatNum) {
					continue;
				}
				listRelationships.Items.SetValue(i,Patients.GetNameFirst(_patient.FName,_patient.Preferred)+" "
					+Guardians.GetGuardianRelationshipStr(_listGuardians[i].Relationship));
				//don't break out of loop since it is possible to add multiple relationships with this patient as the PatNumGuardian
				//break;
			}
		}

		#region Public Health
		
		private void textCounty_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Return){
				listCounties.Visible=false;
				comboGradeLevel.Focus();
				return;
			}
			if(textCounty.Text==""){
				listCounties.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down){
				if(listCounties.Items.Count==0){
					return;
				}
				if(listCounties.SelectedIndex==-1){
					listCounties.SelectedIndex=0;
					textCounty.Text=listCounties.SelectedItem.ToString();
				}
				else if(listCounties.SelectedIndex==listCounties.Items.Count-1){
					listCounties.SelectedIndex=-1;
					textCounty.Text=_countyOriginal;
				}
				else{
					listCounties.SelectedIndex++;
					textCounty.Text=listCounties.SelectedItem.ToString();
				}
				textCounty.SelectionStart=textCounty.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up){
				if(listCounties.Items.Count==0){
					return;
				}
				if(listCounties.SelectedIndex==-1){
					listCounties.SelectedIndex=listCounties.Items.Count-1;
					textCounty.Text=listCounties.SelectedItem.ToString();
				}
				else if(listCounties.SelectedIndex==0){
					listCounties.SelectedIndex=-1;
					textCounty.Text=_countyOriginal;
				}
				else{
					listCounties.SelectedIndex--;
					textCounty.Text=listCounties.SelectedItem.ToString();
				}
				textCounty.SelectionStart=textCounty.Text.Length;
				return;
			}
			if(textCounty.Text.Length==1){
				textCounty.Text=textCounty.Text.ToUpper();
				textCounty.SelectionStart=1;
			}
			_countyOriginal=textCounty.Text;//the original text is preserved when using up and down arrows
			listCounties.Items.Clear();
			_countyArray=Counties.Refresh(textCounty.Text).ToArray();
			//similarSchools=
				//Carriers.GetSimilarNames(textCounty.Text);
			for(int i=0;i<_countyArray.Length;i++){
				listCounties.Items.Add(_countyArray[i].CountyName);
			}
			int h=13*_countyArray.Length+5;
			if(h > ClientSize.Height-listCounties.Top)
				h=ClientSize.Height-listCounties.Top;
			LayoutManager.MoveSize(listCounties,new Size(textCounty.Width,LayoutManager.Scale(h)));
			listCounties.Visible=true;
		}

		private void textCounty_Leave(object sender, System.EventArgs e) {
			if(_isMouseInListCounties){
				return;
			}
			//or if user clicked on a different text box.
			if(listCounties.SelectedIndex!=-1){
				textCounty.Text=_countyArray[listCounties.SelectedIndex].CountyName;
			}
			listCounties.Visible=false;
			SetRequiredFields();
		}

		private void listCounties_Click(object sender, System.EventArgs e){
			if(listCounties.SelectedIndex==-1) { 
				return;
			}
			textCounty.Text=_countyArray[listCounties.SelectedIndex].CountyName;
			textCounty.Focus();
			textCounty.SelectionStart=textCounty.Text.Length;
			listCounties.Visible=false;
		}

		private void listCounties_MouseEnter(object sender, System.EventArgs e){
			_isMouseInListCounties=true;
		}

		private void listCounties_MouseLeave(object sender, System.EventArgs e){
			_isMouseInListCounties=false;
		}

		private void textSite_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Return) {
				listSites.Visible=false;
				comboGradeLevel.Focus();
				return;
			}
			if(textSite.Text=="") {
				listSites.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listSites.Items.Count==0) {
					return;
				}
				if(listSites.SelectedIndex==-1) {
					listSites.SelectedIndex=0;
					textSite.Text=listSites.SelectedItem.ToString();
				}
				else if(listSites.SelectedIndex==listSites.Items.Count-1) {
					listSites.SelectedIndex=-1;
					textSite.Text=_siteOriginal;
				}
				else {
					listSites.SelectedIndex++;
					textSite.Text=listSites.SelectedItem.ToString();
				}
				textSite.SelectionStart=textSite.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listSites.Items.Count==0) {
					return;
				}
				if(listSites.SelectedIndex==-1) {
					listSites.SelectedIndex=listSites.Items.Count-1;
					textSite.Text=listSites.SelectedItem.ToString();
				}
				else if(listSites.SelectedIndex==0) {
					listSites.SelectedIndex=-1;
					textSite.Text=_siteOriginal;
				}
				else {
					listSites.SelectedIndex--;
					textSite.Text=listSites.SelectedItem.ToString();
				}
				textSite.SelectionStart=textSite.Text.Length;
				return;
			}
			if(textSite.Text.Length==1) {
				textSite.Text=textSite.Text.ToUpper();
				textSite.SelectionStart=1;
			}
			_siteOriginal=textSite.Text;//the original text is preserved when using up and down arrows
			listSites.Items.Clear();
			_listSitesFiltered=Sites.GetListFiltered(textSite.Text);
			//similarSchools=
			//Carriers.GetSimilarNames(textSite.Text);
			for(int i=0;i<_listSitesFiltered.Count;i++) {
				listSites.Items.Add(_listSitesFiltered[i].Description);
			}
			int h=13*_listSitesFiltered.Count+5;
			if(h > ClientSize.Height-listSites.Top) {
				h=ClientSize.Height-listSites.Top;
			}
			LayoutManager.MoveSize(listSites,new Size(textSite.Width,LayoutManager.Scale(h)));
			listSites.Visible=true;
		}

		private void textSite_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListSites) {
				return;
			}
			//or if user clicked on a different text box.
			if(listSites.SelectedIndex!=-1) {
				textSite.Text=_listSitesFiltered[listSites.SelectedIndex].Description;
				_patient.SiteNum=_listSitesFiltered[listSites.SelectedIndex].SiteNum;
			}
			listSites.Visible=false;
			SetRequiredFields();
		}

		private void listSites_Click(object sender,System.EventArgs e) {
			if(listSites.SelectedIndex==-1) {
				return;
			}
			textSite.Text=_listSitesFiltered[listSites.SelectedIndex].Description;
			_patient.SiteNum=_listSitesFiltered[listSites.SelectedIndex].SiteNum;
			textSite.Focus();
			textSite.SelectionStart=textSite.Text.Length;
			listSites.Visible=false;
		}

		private void listSites_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListSites=true;
		}

		private void listSites_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListSites=false;
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateTimeDeceased.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
			CalcAge();
		}

		private void butPickSite_Click(object sender,EventArgs e) {
			FrmSites frmSites=new FrmSites();
			frmSites.IsSelectionMode=true;
			frmSites.SiteNumSelected=_patient.SiteNum;
			frmSites.ShowDialog();
			if(!frmSites.IsDialogOK) {
				return;
			}
			_patient.SiteNum=frmSites.SiteNumSelected;
			textSite.Text=Sites.GetDescription(_patient.SiteNum);
			SetRequiredFields();
		}

		private void butPickResponsParty_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			FrmFamilyMemberSelect frmFamilyMemberSelect=new FrmFamilyMemberSelect(_family);
			frmFamilyMemberSelect.ShowDialog();
			if(frmFamilyMemberSelect.IsDialogCancel) {
				return;
			}
			_patient.ResponsParty=frmFamilyMemberSelect.SelectedPatNum;
			//saves a call to the db if this pat's responsible party is self and name in db could be different than local PatCur name
			if(_patient.PatNum==_patient.ResponsParty) {
				textResponsParty.Text=_patient.GetNameLF();
			}
			else {
				textResponsParty.Text=Patients.GetLim(_patient.ResponsParty).GetNameLF();
			}
			_errorProvider.SetError(textResponsParty,"");
		}

		private void butClearResponsParty_Click(object sender,EventArgs e) {
			_patient.ResponsParty=0;
			textResponsParty.Text="";
			SetRequiredFields();
		}
		#endregion

		/*private void butChangeEmp_Click(object sender, System.EventArgs e) {
			using FormEmployers FormE=new FormEmployers();
			FormE.IsSelectMode=true;
			Employers.Cur=Employers.GetEmployer(PatCur.EmployerNum);
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			PatCur.EmployerNum=Employers.Cur.EmployerNum;
			textEmployer.Text=Employers.Cur.EmpName;
		}*/

		private void textEmployer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return){
				listEmps.Visible=false;
				textWirelessPhone.Focus();
				return;
			}
			if(textEmployer.Text==""){
				listEmps.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down){
				if(listEmps.Items.Count==0){
					return;
				}
				if(listEmps.SelectedIndex==-1){
					listEmps.SelectedIndex=0;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				else if(listEmps.SelectedIndex==listEmps.Items.Count-1){
					listEmps.SelectedIndex=-1;
					textEmployer.Text=_empOriginal;
				}
				else{
					listEmps.SelectedIndex++;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up){
				if(listEmps.Items.Count==0){
					return;
				}
				if(listEmps.SelectedIndex==-1){
					listEmps.SelectedIndex=listEmps.Items.Count-1;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				else if(listEmps.SelectedIndex==0){
					listEmps.SelectedIndex=-1;
					textEmployer.Text=_empOriginal;
				}
				else{
					listEmps.SelectedIndex--;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(textEmployer.Text.Length==1){
				textEmployer.Text=textEmployer.Text.ToUpper();
				textEmployer.SelectionStart=1;
			}
			_empOriginal=textEmployer.Text;//the original text is preserved when using up and down arrows
			listEmps.Items.Clear();
			List<Employer> listEmployersSimilar=Employers.GetSimilarNames(textEmployer.Text);
			for(int i=0;i<listEmployersSimilar.Count;i++){
				listEmps.Items.Add(listEmployersSimilar[i].EmpName);
			}
			int h=13*listEmployersSimilar.Count+5;
			if(h > ClientSize.Height-listEmps.Top){
				h=ClientSize.Height-listEmps.Top;
			}
			LayoutManager.MoveSize(listEmps,new Size(LayoutManager.Scale(231),LayoutManager.Scale(h)));
			listEmps.Visible=true;
		}

		private void textEmployer_Leave(object sender, System.EventArgs e) {
			if(isMouseInListEmps){
				return;
			}
			listEmps.Visible=false;
			SetRequiredFields();
		}

		private void listEmps_Click(object sender, System.EventArgs e){
			if(listEmps.SelectedItem==null) {
				return;
			}
			textEmployer.Text=listEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listEmps.Visible=false;
		}

		private void listEmps_DoubleClick(object sender, System.EventArgs e){
			//no longer used
			if(listEmps.SelectedItem==null) {
				return;
			}
			textEmployer.Text=listEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listEmps.Visible=false;
		}

		private void listEmps_MouseEnter(object sender, System.EventArgs e){
			isMouseInListEmps=true;
		}

		private void listEmps_MouseLeave(object sender, System.EventArgs e){
			isMouseInListEmps=false;
		}

		private void textMedicaidState_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return) {
				listMedicaidStates.Visible=false;
				return;
			}
			if(textMedicaidState.Text=="") {
				listMedicaidStates.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listMedicaidStates.Items.Count==0) {
					return;
				}
				if(listMedicaidStates.SelectedIndex==-1) {
					listMedicaidStates.SelectedIndex=0;
					textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
				}
				else if(listMedicaidStates.SelectedIndex==listMedicaidStates.Items.Count-1) {
					listMedicaidStates.SelectedIndex=-1;
					textMedicaidState.Text=_medicaidStateOriginal;
				}
				else {
					listMedicaidStates.SelectedIndex++;
					textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
				}
				textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listMedicaidStates.Items.Count==0) {
					return;
				}
				if(listMedicaidStates.SelectedIndex==-1) {
					listMedicaidStates.SelectedIndex=listMedicaidStates.Items.Count-1;
					textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
				}
				else if(listMedicaidStates.SelectedIndex==0) {
					listMedicaidStates.SelectedIndex=-1;
					textMedicaidState.Text=_medicaidStateOriginal;
				}
				else {
					listMedicaidStates.SelectedIndex--;
					textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
				}
				textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
				return;
			}
			if(textMedicaidState.Text.Length==1) {
				textMedicaidState.Text=textMedicaidState.Text.ToUpper();
				textMedicaidState.SelectionStart=1;
			}
			_medicaidStateOriginal=textMedicaidState.Text;//the original text is preserved when using up and down arrows
			listMedicaidStates.Items.Clear();
			List<StateAbbr> listStateAbbrsSimilar=StateAbbrs.GetSimilarAbbrs(textMedicaidState.Text);
			for(int i=0;i<listStateAbbrsSimilar.Count;i++) {
				listMedicaidStates.Items.Add(listStateAbbrsSimilar[i].Abbr);
			}
			int h=13*listStateAbbrsSimilar.Count+5;
			if(h > ClientSize.Height-listMedicaidStates.Top) {
				h=ClientSize.Height-listMedicaidStates.Top;
			}
			LayoutManager.MoveSize(listMedicaidStates,new Size(textMedicaidState.Width,LayoutManager.Scale(h)));
			listMedicaidStates.Visible=true;
		}

		private void textMedicaidState_Leave(object sender,System.EventArgs e) {
			if(_IsMouseInListMedicaidStates) {
				return;
			}
			listMedicaidStates.Visible=false;
			SetRequiredFields();
		}

		private void listMedicaidStates_Click(object sender,System.EventArgs e) {
			if(listMedicaidStates.SelectedItem==null) {
				return;
			}
			textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
			textMedicaidState.Focus();
			textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
			listMedicaidStates.Visible=false;
		}

		private void listMedicaidStates_MouseEnter(object sender,System.EventArgs e) {
			_IsMouseInListMedicaidStates=true;
		}

		private void listMedicaidStates_MouseLeave(object sender,System.EventArgs e) {
			_IsMouseInListMedicaidStates=false;
		}

		private void textState_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return) {
				listStates.Visible=false;
				return;
			}
			if(textState.Text=="") {
				listStates.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listStates.Items.Count==0) {
					return;
				}
				if(listStates.SelectedIndex==-1) {
					listStates.SelectedIndex=0;
					textState.Text=listStates.SelectedItem.ToString();
				}
				else if(listStates.SelectedIndex==listStates.Items.Count-1) {
					listStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listStates.SelectedIndex++;
					textState.Text=listStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listStates.Items.Count==0) {
					return;
				}
				if(listStates.SelectedIndex==-1) {
					listStates.SelectedIndex=listStates.Items.Count-1;
					textState.Text=listStates.SelectedItem.ToString();
				}
				else if(listStates.SelectedIndex==0) {
					listStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listStates.SelectedIndex--;
					textState.Text=listStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(textState.Text.Length==1) {
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=1;
			}
			_stateOriginal=textState.Text;//the original text is preserved when using up and down arrows
			listStates.Items.Clear();
			List<StateAbbr> listStateAbbrsSimilar=StateAbbrs.GetSimilarAbbrs(textState.Text);
			for(int i=0;i<listStateAbbrsSimilar.Count;i++) {
				listStates.Items.Add(listStateAbbrsSimilar[i].Abbr);
			}
			int h=13*listStateAbbrsSimilar.Count+5;
			if(h > ClientSize.Height-listStates.Top) {
				h=ClientSize.Height-listStates.Top;
			}
			LayoutManager.MoveSize(listStates,new Size(textState.Width,LayoutManager.Scale(h)));
			listStates.Visible=true;
		}

		private void textState_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListStates) {
				return;
			}
			listStates.Visible=false;
			SetRequiredFields();
		}

		private void listStates_Click(object sender,System.EventArgs e) {
			if(listStates.SelectedItem==null) {
				return;
			}
			textState.Text=listStates.SelectedItem.ToString();
			textState.Focus();
			textState.SelectionStart=textState.Text.Length;
			listStates.Visible=false;
		}

		private void listStates_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListStates=true;
		}

		private void listStates_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListStates=false;
		}

		private void listPosition_SelectedIndexChanged(object sender,EventArgs e) {
			if(!_isLoad) {
				SetRequiredFields();
			}
			//CheckGuardianUiState();
		}

		private void listRelationships_DoubleClick(object sender,EventArgs e) {
			if(listRelationships.SelectedIndex==-1) {
				return;
			}
			UpdateLocalNameHelper();
			FrmGuardianEdit frmGuardianEdit=new FrmGuardianEdit(_listGuardians[listRelationships.SelectedIndex],_family);
			frmGuardianEdit.ShowDialog();
			if(frmGuardianEdit.IsDialogOK) {
				FillGuardians();
			}
		}

		private void butAddGuardian_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			Guardian guardian=new Guardian();
			guardian.IsNew=true;
			guardian.PatNumChild=_patient.PatNum;
			//no patnumGuardian set
			FrmGuardianEdit frmGuardianEdit=new FrmGuardianEdit(guardian,_family);
			frmGuardianEdit.ShowDialog();
			if(frmGuardianEdit.IsDialogOK) {
				_hasGuardiansChanged=true;
				FillGuardians();
			}
		}

		private void butGuardianDefaults_Click(object sender,EventArgs e) {
			if(Guardians.ExistForFamily(_patient.Guarantor)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Replace existing relationships with default relationships for entire family?")) {
					return;
				}
				//don't delete existing guardians for family until we are certain we can replace them with the defaults
				//Guardians.DeleteForFamily(PatCur.Guarantor);
			}
			Result result=Patients.SetDefaultRelationships(_patient,_family,(PatientPosition)listPosition.SelectedIndex);
			if(result.IsFailure()) {
				MsgBox.Show(this,result.Msg);
				return;
			}
			_hasGuardiansChanged=true;
			FillGuardians();
		}

		private void butRaceEthnicity_Click(object sender,EventArgs e) {
			FrmPatientRaceEthnicity frmPatientRaceEthnicity=new FrmPatientRaceEthnicity();
			frmPatientRaceEthnicity.PatientCur=_patient;
			frmPatientRaceEthnicity.ListPatientRacesAll=_listPatientRaces;
			frmPatientRaceEthnicity.ShowDialog();
			if(frmPatientRaceEthnicity.IsDialogOK) {
				_listPatientRaces=frmPatientRaceEthnicity.GetPatientRaces();
				textRace.Text=PatientRaces.GetRaceDescription(_listPatientRaces);
				textEthnicity.Text=PatientRaces.GetEthnicityDescription(_listPatientRaces);
				SetRequiredFields();
			}
		}

		private void comboBoxMultiRace_SelectionChangeCommitted(object sender,EventArgs e) {
			RemoveIllogicalRaceCombinations();
			SetRequiredFields();
		}

		///<summary>Disallows the user from selecting illogical combinations such as 'DeclinedToSpecify' and 'Asian'.</summary>
		private void RemoveIllogicalRaceCombinations() {
			if(comboBoxMultiRace.SelectedIndices.Count<2) {
				return;
			}
			int declinedIdx;
			int otherIdx;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				declinedIdx=4;
				otherIdx=6;
			}
			else {
				declinedIdx=5;
				otherIdx=9;
			}
			//The first selected is 'None', so unselect it.
			if(comboBoxMultiRace.SelectedIndices[0]==0) {
				comboBoxMultiRace.SetSelected(0,false);
				if(comboBoxMultiRace.SelectedIndices.Count<2) {
					return;
				}
			}
			//The first selected is 'DeclinedToSpecify', so unselect it.
			if(comboBoxMultiRace.SelectedIndices[0]==declinedIdx) {
				comboBoxMultiRace.SetSelected(declinedIdx,false);
				if(comboBoxMultiRace.SelectedIndices.Count<2) {
					return;
				}
			}
			//The first selected is 'Other', so unselect it.
			if(comboBoxMultiRace.SelectedIndices[0]==otherIdx) {
				comboBoxMultiRace.SetSelected(otherIdx,false);
				if(comboBoxMultiRace.SelectedIndices.Count<2) {
					return;
				}
			}
			//'None' is either the last one selected or in the middle of the items selected, so unselect all but 'None'.
			if(comboBoxMultiRace.SelectedIndices.Contains(0)) {
				comboBoxMultiRace.SelectedIndices.Clear();
				comboBoxMultiRace.SetSelected(0,true);
				return;
			}
			//'DeclinedToSpecify' is either the last one selected or in the middle of the items selected, so unselect all but 'DeclinedToSpecify'.
			if(comboBoxMultiRace.SelectedIndices.Contains(declinedIdx)) {
				comboBoxMultiRace.SelectedIndices.Clear();
				comboBoxMultiRace.SetSelected(declinedIdx,true);
				return;
			}
			//'Other' is either the last one selected or in the middle of the items selected, so unselect all but 'Other'.
			if(comboBoxMultiRace.SelectedIndices.Contains(otherIdx)) {
				comboBoxMultiRace.SelectedIndices.Clear();
				comboBoxMultiRace.SetSelected(otherIdx,true);
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				return;
			}
			//Guaranteed to be at least 2 selected indices if we get here
			int hispanicIdx=7;
			int nonHispanicIdx=11;
			//The last one selected is 'Hispanic' and 'NotHispanic' is also selected.
			if(comboBoxMultiRace.SelectedIndices[comboBoxMultiRace.SelectedIndices.Count-1]==hispanicIdx
				&& comboBoxMultiRace.SelectedIndices.Contains(nonHispanicIdx)) 
			{
				comboBoxMultiRace.SetSelected(nonHispanicIdx,false);
			}
			//The last one selected is 'NotHispanic' and 'Hispanic' is also selected.
			else if(comboBoxMultiRace.SelectedIndices[comboBoxMultiRace.SelectedIndices.Count-1]==nonHispanicIdx
				&& comboBoxMultiRace.SelectedIndices.Contains(hispanicIdx)) 
			{
				comboBoxMultiRace.SetSelected(hispanicIdx,false);
			}
		}

		private void comboSexOrientation_SelectedIndexChanged(object sender,EventArgs e) {
			textSpecifySexOrientation.Visible=((SexOrientation)comboSexOrientation.SelectedIndex==SexOrientation.AdditionalOrientation);
			labelSpecifySexOrientation.Visible=((SexOrientation)comboSexOrientation.SelectedIndex==SexOrientation.AdditionalOrientation);
			SetRequiredFields();
		}

		private void comboGenderIdentity_SelectedIndexChanged(object sender,EventArgs e) {
			textSpecifyGender.Visible=((GenderId)comboGenderIdentity.SelectedIndex==GenderId.AdditionalGenderCategory);
			labelSpecifyGender.Visible=((GenderId)comboGenderIdentity.SelectedIndex==GenderId.AdditionalGenderCategory);
			SetRequiredFields();
		}
		
		///<summary>Gets an employerNum based on the name entered. Called from FillCur</summary>
		private void GetEmployerNum(){
			if(_patient.EmployerNum==0){//no employer was previously entered.
				if(textEmployer.Text==""){
					//no change
				}
				else{
					_patient.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
				return;
			}
			//an employer was previously entered
			if(textEmployer.Text==""){
				_patient.EmployerNum=0;
			}
			//if text has changed
			else if(Employers.GetName(_patient.EmployerNum)!=textEmployer.Text){
				_patient.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
			}
		}

		private void butReferredFrom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd)) {
				return;
			}
			Referral referral=new Referral();
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?")) {
				FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
				frmPatientSelect.ShowDialog();
				if(frmPatientSelect.IsDialogCancel) {
					return;
				}
				referral.PatNum=frmPatientSelect.PatNumSelected;
				bool isReferralNew=true;
				Referral referralMatch=Referrals.GetFirstOrDefault(x => x.PatNum==frmPatientSelect.PatNumSelected);
				if(referralMatch!=null) {
					referral=referralMatch;
					isReferralNew=false;
				}
				FrmReferralEdit frmReferralEdit=new FrmReferralEdit(referral);//the ReferralNum must be added here
				frmReferralEdit.IsNew=isReferralNew;
				frmReferralEdit.ShowDialog();
				if(!frmReferralEdit.IsDialogOK) {
					return;
				}
				referral=frmReferralEdit.ReferralCur;//not needed, but it makes it clear that we are editing the ref in FormRefEdit
			}
			else {//not a patient referral, must be a doctor or marketing/other so show the referral select window with doctor and other check boxes checked
				FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
				frmReferralSelect.IsSelectionMode=true;
				frmReferralSelect.IsShowPat=false;
				frmReferralSelect.ShowDialog();
				if(frmReferralSelect.IsDialogCancel) {
					FillReferrals();//the user may have edited a referral and then cancelled attaching to the patient, refill the text box to reflect any changes
					return;
				}
				referral=frmReferralSelect.ReferralSelected;
			}
			RefAttach refAttach=new RefAttach();
			refAttach.ReferralNum=referral.ReferralNum;
			refAttach.PatNum=_patient.PatNum;
			refAttach.RefType=ReferralType.RefFrom;
			refAttach.RefDate=DateTime.Today;
			if(referral.IsDoctor) {//whether using ehr or not
				refAttach.IsTransitionOfCare=true;
			}
			refAttach.ItemOrder=_listRefAttaches.Select(x => x.ItemOrder).DefaultIfEmpty().Max()+1;
			RefAttaches.Insert(refAttach);
			SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,_patient.PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
			FillReferrals();
		}

		///<summary>Fills the Referred From text box with the oldest (lowest ItemOrder) referral source with ReferralType.RefFrom.</summary>
		private void FillReferrals() {
			textReferredFrom.Clear();
			_listRefAttaches=RefAttaches.Refresh(_patient.PatNum);
			string firstRefNameTypeAbbr="";
			string firstRefType="";
			string firstRefFullName="";
			RefAttach refAttach=_listRefAttaches.FirstOrDefault(x => x.RefType==ReferralType.RefFrom);
			if(refAttach==null) {
				return;
			}
			Referral referral=WpfControls.ReferralL.GetReferral(refAttach.ReferralNum);
			if(referral==null) {
				return;
			}
			firstRefFullName=Referrals.GetNameLF(referral.ReferralNum);
			if(referral.PatNum>0) {
				firstRefType=" (patient)";
			}
			else if(referral.IsDoctor) {
				firstRefType=" (doctor)";
			}
			string suffix="";
			if(_listRefAttaches.Count(x => x.RefType==ReferralType.RefFrom)>1) {
				suffix=" (+"+(_listRefAttaches.Count(x => x.RefType==ReferralType.RefFrom)-1)+" more)";
			}
			firstRefNameTypeAbbr=firstRefFullName;
			for(int i=1;i<firstRefFullName.Length+1;i++) {//i is used as the length to substring, not an index, so i<firstRefName.Length+1 is safe
				if(TextRenderer.MeasureText(firstRefFullName.Substring(0,i)+firstRefType+suffix,textReferredFrom.Font).Width<textReferredFrom.Width)	{
					continue;
				}
				firstRefNameTypeAbbr=firstRefFullName.Substring(0,i-1);
				break;
			}
			firstRefNameTypeAbbr+=firstRefType+suffix;//both firstRefType and suffix could be blank, but they will show regardless of the length of firstRefName
			textReferredFrom.Text=firstRefNameTypeAbbr;
			_referredFromToolTip.SetToolTip(textReferredFrom,firstRefFullName+firstRefType+suffix);
			//Example: Schmidt, John Jacob Jingleheimer, DDS (doctor) (+5 more) 
			//might be shortened to : Schmidt, John Jaco (doctor) (+5 more) 
			_errorProvider.SetError(textReferredFrom,"");
		}

		///<summary>Returns true if any of the checkboxes indicating all family members should be updated with the corresponding data are checked.</summary>
		private bool IsAnySameChecked() {
			return (checkArriveEarlySame.Checked || checkAddressSame.Checked || checkAddressSameForSuperFam.Checked || checkBillProvSame.Checked || checkNotesSame.Checked
				|| checkEmailPhoneSame.Checked);
		}

		private void ShowPatientEditEmail() {
			FrmPatientEditEmail frmPatientEditEmail=new FrmPatientEditEmail(textEmail.Text);
			frmPatientEditEmail.ShowDialog();
			if(frmPatientEditEmail.IsDialogOK) {
				textEmail.Text=frmPatientEditEmail.PatientEmails;
			}
		}

		private void butEmailEdit_Click(object sender,EventArgs e) {
			ShowPatientEditEmail();
		}

		private void textEmail_KeyUp(object sender,KeyEventArgs e) {
			if(textEmail.Text.Length>=100) {
				ShowPatientEditEmail();
			}
		}

		private void textReferredFrom_DoubleClick(object sender,EventArgs e) {
			FrmReferralsPatient frmReferralsPatient=new FrmReferralsPatient();
			frmReferralsPatient.PatNum=_patient.PatNum;
			frmReferralsPatient.ShowDialog();
			FillReferrals();
			SetRequiredFields();
		}

		private void butViewSSN_Click(object sender,EventArgs e) {
			if(textSSN.Text!=_maskedSSNOld && textSSN.Text!=_patientOld.SSN) {
				//SSN text field has changed since the form was loaded, just return.
				return;
			}
			textSSN.Text=Patients.SSNFormatHelper(_patientOld.SSN,false);
			textSSN.ReadOnly=false;
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(EnumPermType.PatientSSNView,_patient.PatNum,logtext);
		}

		private void butViewBirthdate_Click(object sender,EventArgs e) {
			//Button not visible unless Birthdate is masked
			if(odDatePickerBirthDate.GetDateTime()!=_patientOld.Birthdate) {
				//Birthdate text field has changed since form was loaded and would be "unmasked" already. return.
				return;
			}
			odDatePickerBirthDate.SetDateTime(_patientOld.Birthdate);
			string logtext="Date of birth unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(EnumPermType.PatientDOBView,_patient.PatNum,logtext);
			textBirthdateMask.Visible=false;
			odDatePickerBirthDate.ReadOnly=false;
			odDatePickerBirthDate.Visible=true;
			_isBirthdayMasked=false;
			odDatePickerBirthDate.Focus();//Force the validation to happen again when losing focus in case our stored birthdate is already invalid.
		}

		//clear out deceased date time in case it was entered by mistake
		private void butClearDateTimeDeceased_Click(object sender,EventArgs e) {
			textDateTimeDeceased.Text=""; 
		}

		private void butSave_Click(object sender ,System.EventArgs e) {
			bool isValid=true;
			SetRequiredFields();
			object[] objectArrayParameters=new object[] { isValid,_patient };
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_beginning",objectArrayParameters);
			Patient patientOld=_patient.Copy();
			if((bool)objectArrayParameters[0]==false) {//Didn't pass plug-in validation
				return;
			}
			//Place all UI validation in here. Data validation should go in Patients.ValidatePatientEdit(). Even if a field has both UI and data validation, it still needs to be separated.
			#region UI Validation
			if(odDatePickerBirthDate.IsEmptyDateTime()//If not blank
				&& !IsBirthdateValid()//Check for a valid date
				&& !_isBirthdayMasked)//If not masked
			{
				MsgBox.Show(this,"Patient's Birthdate is not a valid or allowed date.");
				return;
			}
			if(!odDatePickerDateFirstVisit.IsValid() 
				|| (!PrefC.GetBool(PrefName.EasyHideHospitals) //Only validate admitDate and dischargeDate when Hospitals is turned on. User has no way to correct errors otherwise.
				&& (!odDatePickerAdmitDate.IsValid() || !odDatePickerDischargeDate.IsValid()))) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime dateTimeDeceased=DateTime.MinValue;
			if(textDateTimeDeceased.Text!="") {
				try {
					dateTimeDeceased=DateTime.Parse(textDateTimeDeceased.Text);
				}
				catch {
					MsgBox.Show(this,"Date time deceased is invalid.");
					return;
				}
			}
			try{
				PIn.Int(textAskToArriveEarly.Text);
			}
			catch{
				MsgBox.Show(this,"Ask To Arrive Early invalid.");
				return;
			}
			//If public health is enabled and the combo box is in an invalid state, warn the user.
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboGradeLevel.SelectedIndex < 0) {
				//This isn't really here to get valid data from the user but to prevent the value of -1 getting entered into the database.
				MsgBox.Show(this,"Grade Level is invalid.");
				return;
			}
			if(comboPriProv.GetSelectedProvNum()==0) {//selected index could be -1 if the provider was selected and then hidden
				MsgBox.Show(this,"Primary provider must be set.");
				_isValidating=true;
				SetRequiredFields();
				return;
			}
			//Check SSN for US Formatting.  If SSN is masked, don't check.  Similar checks to textSSN_Validating()
			if(CultureInfo.CurrentCulture.Name=="en-US" && textSSN.Text!=""//Only validate if US and SSN is not blank
				&& !textSSN.ReadOnly && Patients.SSNRemoveDashes(textSSN.Text)!=_patientOld.SSN)//If SSN isn't masked, it isn't readonly, might have changed. Only validate if changed
			{
				if(!Regex.IsMatch(textSSN.Text,@"^\d\d\d-\d\d-\d\d\d\d$")) {
					if(MessageBox.Show("SSN not valid. Continue anyway?","",MessageBoxButtons.OKCancel)
						!=DialogResult.OK) {
						_errorProvider.SetError(textSSN, "Invalid social security number.");
						return;
					}
				}
			}
			if(_isMissingRequiredFields) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Required fields are missing or incorrect.  Click OK to save anyway or Cancel to return and "
						+"finish editing patient information.")) {
					_isValidating=true;
					SetRequiredFields();
					return;
				}
				SecurityLogs.MakeLogEntry(EnumPermType.RequiredFields,_patient.PatNum,"Saved patient with required fields missing.");
			}
			#endregion UI Validation
			#region FillFromUI
			_patient.LName=textLName.Text;
			_patient.FName=textFName.Text;
			_patient.MiddleI=textMiddleI.Text;
			_patient.Preferred=textPreferred.Text;
			_patient.Title=textTitle.Text;
			_patient.Salutation=textSalutation.Text;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Mother's maiden name UI is only used when EHR is enabled.
				_ehrPatient.MotherMaidenFname=textMotherMaidenFname.Text;
				_ehrPatient.MotherMaidenLname=textMotherMaidenLname.Text;
			}
			_patient.PatStatus=_listPatientStatuses[listStatus.SelectedIndex];//1:1
			if(_patient.PatStatus==PatientStatus.Deceased && _patientOld.PatStatus!=PatientStatus.Deceased) {
				List<Appointment> listAppointmentsFuture=Appointments.GetFutureSchedApts(_patient.PatNum);
				if(listAppointmentsFuture.Count>0) {
					string apptDates=string.Join("\r\n",listAppointmentsFuture.Take(10).Select(x => x.AptDateTime.ToString()));
					if(listAppointmentsFuture.Count>10) {
						apptDates+="(...)";
					}
					if(MessageBox.Show(Lan.g(this,"This patient has scheduled appointments in the future")+":\r\n"
							+apptDates+"\r\n"
							+Lan.g(this,"Would you like to delete them and set the patient to Deceased?"),Lan.g(this,"Delete future appointments?"),MessageBoxButtons.YesNo)==DialogResult.Yes) 
					{
						for(int i=0;i<listAppointmentsFuture.Count;i++) {
							Appointments.Delete(listAppointmentsFuture[i].AptNum,true);
						}
					}
					else {
						_patient.PatStatus=_patientOld.PatStatus;
						return;
					}
				}
			}
			_patient.Gender=listGender.GetSelected<PatientGender>();
			if(PrefC.GetBool(PrefName.ShowPreferredPronounsForPats)) {//Only set when preference is enabled.
				_patientNote.Pronoun=comboPreferredPronouns.GetSelected<PronounPreferred>();
			}
			_patient.Position=listPosition.GetSelected<PatientPosition>();
			//Only update birthdate if it was changed, shouldn't be masked.
			if(!_isBirthdayMasked && _patientOld.Birthdate.Date!=odDatePickerBirthDate.GetDateTime().Date) {
				_patient.Birthdate=odDatePickerBirthDate.GetDateTime();
			}
			_patient.DateTimeDeceased=dateTimeDeceased;
			if(!textSSN.ReadOnly && Patients.SSNRemoveDashes(textSSN.Text)!=_patientOld.SSN) { //Only update SSN if it was changed, readonly must be false to edit, meaning it is also unmasked.
				if(CultureInfo.CurrentCulture.Name=="en-US") {
					if(Regex.IsMatch(textSSN.Text,@"^\d\d\d-\d\d-\d\d\d\d$")) {
						_patient.SSN=textSSN.Text.Substring(0,3)+textSSN.Text.Substring(4,2)
							+textSSN.Text.Substring(7,4);
					}
					else {
						_patient.SSN=textSSN.Text;
					}
				}
				else {//other cultures
					_patient.SSN=textSSN.Text;
				}
			}
			if(IsNew) {//Check if patient already exists.
				List<Patient> listPatients=Patients.GetListByName(_patient.LName,_patient.FName,_patient.PatNum);
				for(int i=0;i<listPatients.Count;i++) {
					//If dates match or aren't entered there might be a duplicate patient.
					if(listPatients[i].Birthdate==_patient.Birthdate
					|| listPatients[i].Birthdate.Year<1880
					|| _patient.Birthdate.Year<1880) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This patient might already exist.  Continue anyway?")) {
							return;
						}
						break;
					}
				}
			}
			YN yNTxtMsgOk=YN.Unknown;
			if(checkTextingY.Checked){
				yNTxtMsgOk=YN.Yes;
			}
			if(checkTextingN.Checked){
				yNTxtMsgOk=YN.No;
			}
			//Send medication history consent to DoseSpot if checkDoseSpotConsent is checked.
			if(!_patientNote.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx) && checkDoseSpotConsent.Checked) {
				long clinicNum=Clinics.ClinicNum;
				if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
					clinicNum=_patient.ClinicNum;
				}
				//Create a temporary patient to set important information to give to DoseSpot in case a patient is needed to be created in DoseSpot.
				Patient patientForDoseSpot=_patient.Copy();
				patientForDoseSpot.HmPhone=textHmPhone.Text;
				patientForDoseSpot.WkPhone=textWkPhone.Text;
				patientForDoseSpot.WirelessPhone=textWirelessPhone.Text;
				patientForDoseSpot.TxtMsgOk=yNTxtMsgOk;
				patientForDoseSpot.Email=textEmail.Text;
				patientForDoseSpot.Address=textAddress.Text;
				patientForDoseSpot.Address2=textAddress2.Text;
				patientForDoseSpot.City=textCity.Text;
				patientForDoseSpot.State=textState.Text.Trim();
				patientForDoseSpot.Country=textCountry.Text;
				patientForDoseSpot.Zip=textZip.Text.Trim();
				try {
					DoseSpot.SetMedicationHistConsent(patientForDoseSpot,clinicNum);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to set patient medication access consent for DoseSpot: ")+ex.Message);
					return;
				}
				_patientNote.Consent=PatConsentFlags.ShareMedicationHistoryErx;
			}
			_patient.MedicaidID=textMedicaidID.Text;
			_ehrPatient.MedicaidState=textMedicaidState.Text;
			//Retrieve the value of the Snomed attribute for the selected SexOrientation.
			if(comboSexOrientation.SelectedIndex>=0) {
				SexOrientation sexOrientation=(SexOrientation)comboSexOrientation.SelectedIndex;
				_ehrPatient.SexualOrientation=EnumTools.GetAttributeOrDefault<EhrAttribute>(sexOrientation).Snomed;
				if(sexOrientation==SexOrientation.AdditionalOrientation) {
					_ehrPatient.SexualOrientationNote=textSpecifySexOrientation.Text;
				}
				else {
					_ehrPatient.SexualOrientationNote="";
				}
			}
			//Retrieve the value of the Snomed attribute for the selected GenderId.
			if(comboGenderIdentity.SelectedIndex>=0) {
				GenderId genderId=(GenderId)comboGenderIdentity.SelectedIndex;
				_ehrPatient.GenderIdentity=EnumTools.GetAttributeOrDefault<EhrAttribute>(genderId).Snomed;
				if(genderId==GenderId.AdditionalGenderCategory) {
					_ehrPatient.GenderIdentityNote=textSpecifyGender.Text;
				}
				else {
					_ehrPatient.GenderIdentityNote="";
				}
			}
			_patient.WkPhone=textWkPhone.Text;
			_patient.WirelessPhone=textWirelessPhone.Text;
			_patient.TxtMsgOk=yNTxtMsgOk;
			_patient.Email=textEmail.Text;
			//PatCur.RecallInterval=PIn.PInt(textRecall.Text);
			_patient.ChartNumber=textChartNumber.Text;
			_patient.SchoolName=textSchool.Text;
			//address:
			_patient.HmPhone=textHmPhone.Text;
			_patient.Address=textAddress.Text;
			_patient.Address2=textAddress2.Text;
			_patient.City=textCity.Text;
			_patient.State=textState.Text.Trim();
			_patient.Country=textCountry.Text;
			_patient.Zip=textZip.Text.Trim();
			_patient.CreditType=textCreditType.Text;
			_patient.HasSuperBilling=checkSuperBilling.Checked;
			GetEmployerNum();
			//PatCur.EmploymentNote=textEmploymentNote.Text;
			if(comboLanguage.SelectedIndex==0){
				_patient.Language="";
			}
			else{
				_patient.Language=_listLanguages[comboLanguage.SelectedIndex-1];
			}
			_patient.AddrNote=textAddrNotes.Text;
			_patient.DateFirstVisit=PIn.Date(odDatePickerDateFirstVisit.GetDateTime().ToString());
			_patient.AskToArriveEarly=PIn.Int(textAskToArriveEarly.Text);
			_patient.PriProv=comboPriProv.GetSelectedProvNum();
			_patient.SecProv=comboSecProv.GetSelectedProvNum();
			if(comboFeeSched.SelectedIndex==0){
				_patient.FeeSched=0;
			}
			else{
				_patient.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			_patient.BillingType=comboBillType.GetSelectedDefNum();
			_patient.ClinicNum=comboClinic.ClinicNumSelected;
			if(!_isUsingNewRaceFeature) {
				_listPatientRaces=new List<PatientRace>();
				for(int i=0;i<comboBoxMultiRace.SelectedIndices.Count;i++) {
					int selectedIdx=(int)comboBoxMultiRace.SelectedIndices[i];
					if(selectedIdx==0) {//If the none option was chosen, then ensure that no other race information is saved.
						_listPatientRaces.Clear();
						break;
					}
					if(selectedIdx==1) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2054-5"));//AfricanAmerican
					}
					else if(selectedIdx==2) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"1002-5"));//AmericanIndian
					}
					else if(selectedIdx==3) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2028-9"));//Asian
					}
					else if(selectedIdx==4) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));//DeclinedToSpecifyRace
					}
					else if(selectedIdx==5) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2076-8"));//HawaiiOrPacIsland
					}
					else if(selectedIdx==6) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2131-1"));//Other
					}
					else if(selectedIdx==7) {
						_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2106-3"));//White
					}
				}
				if(_listPatientRaces.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE)) {
					//If DeclinedToSpecify was chosen, then ensure that no other races are saved.
					_listPatientRaces.Clear();
					_listPatientRaces.Add(new PatientRace(_patient.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));
				}
				else if(_listPatientRaces.Any(x => x.CdcrecCode=="2131-1")) {//If Other was chosen, then ensure that no other races are saved.
					_listPatientRaces.Clear();
					_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2131-1"));
				}
				//In order to pass EHR G2 MU testing you must be able to have an ethnicity without a race, or a race without an ethnicity.  This will mean that patients will not count towards
				//meaningful use demographic calculations.  If we have time in the future we should probably alert EHR users when a race is chosen but no ethnicity, or a ethnicity but no race.
				if(comboEthnicity.SelectedIndex==1) {
					_listPatientRaces.Add(new PatientRace(_patient.PatNum,PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE));
				}
				else if(comboEthnicity.SelectedIndex==2) {
					_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2186-5"));//NotHispanic
				}
				else if(comboEthnicity.SelectedIndex==3) {
					_listPatientRaces.Add(new PatientRace(_patient.PatNum,"2135-2"));//Hispanic
				}
			}
			PatientRaces.Reconcile(_patient.PatNum,_listPatientRaces);//Insert, Update, Delete if needed.
			_patient.County=textCounty.Text;
			//site set when user picks from list.
			_patient.GradeLevel=(PatientGrade)comboGradeLevel.SelectedIndex;
			_patient.Urgency=(TreatmentUrgency)comboUrgency.SelectedIndex;
			//ResponsParty handled when buttons are pushed.
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)) {
				_patient.TrophyFolder=textTrophyFolder.Text;
			}
			_patient.PreferContactMethod=(ContactMethod)comboContact.SelectedIndex;
			_patient.PreferConfirmMethod=(ContactMethod)comboConfirm.SelectedIndex;
			_patient.PreferRecallMethod=(ContactMethod)comboRecall.SelectedIndex;
			if(comboExcludeECR.SelectedIndices.Contains(0)) { //If "None" is selected
				_commOptOut.OptOutSms=0;
				_commOptOut.OptOutEmail=0;
			}
			else {
				List<CommOptOutMode> listCommOptOutMode=comboExcludeECR.GetListSelected<CommOptOutMode>();
				//In the future, we could enhance this UI to allow granularity of selecting certain CommOptOutTypes and not others.
				if(listCommOptOutMode.Any(x => x==CommOptOutMode.Text)) {
					_commOptOut.OptOutSms=CommOptOutType.All;
				}
				else {
					_commOptOut.OptOutSms=0;//flags enum
				}
				if(listCommOptOutMode.Any(x => x==CommOptOutMode.Email)) {
					_commOptOut.OptOutEmail=CommOptOutType.All;
				}
				else {
					_commOptOut.OptOutEmail=0;//flags enum
				}
			}
			CommOptOuts.Upsert(_commOptOut);
			if(!PrefC.GetBool(PrefName.EasyHideHospitals)) { //Only update if Hospital tab is showing
				_patient.AdmitDate=PIn.Date(odDatePickerAdmitDate.GetDateTime().ToString());
				_patient.Ward=textWard.Text;
				_ehrPatient.DischargeDate=PIn.Date(odDatePickerDischargeDate.GetDateTime().ToString());
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_patient.CanadianEligibilityCode=(byte)comboCanadianEligibilityCode.SelectedIndex;
			}
			if(_patient.Guarantor==0){
				_patient.Guarantor=_patient.PatNum;
			}
			if(textSite.Text=="") {
				_patient.SiteNum=0;
			}
			else if(textSite.Text!="" && textSite.Text!=Sites.GetDescription(_patient.SiteNum)) {
				_patient.SiteNum=Sites.FindMatchSiteNum(textSite.Text);
			}
			_patientNote.ICEName=textIceName.Text;
			_patientNote.ICEPhone=textIcePhone.Text;
			_patient.HasSignedTil=checkBoxSignedTil.Checked;//Update Signed Truth in Lending State
			#endregion FillFromUI
			#region ValidateData
			Result result=Patients.ValidatePatientEdit(_patient,_ehrPatient,_patientOld,IsNew,textSite.Text);
			if(result.IsFailure()) {
				MsgBox.Show(Lan.g(this,result.Msg)+result.Msg2);
				return;
			}
			#endregion
			#region Save
			result=Patients.SavePatientEdit(Security.CurUser,_patient,_patientOld,_patientNote,_ehrPatient,_family,IsNew,checkRestrictSched.Checked,checkArriveEarlySame.Checked,
				checkAddressSame.Checked,checkAddressSameForSuperFam.Checked,checkBillProvSame.Checked,checkNotesSame.Checked,checkEmailPhoneSame.Checked,
				_defLinkPatient,comboSpecialty.GetSelectedDefNum());
			if(!string.IsNullOrWhiteSpace(result.Msg)) {
				MsgBox.Show(this,result.Msg);
			}
			#endregion Save
			if(_patient.BillingType!=_patientOld.BillingType) {
				AutomationL.Trigger(EnumAutomationTrigger.BillingTypeSet,null,_patient.PatNum);
			}
			//The specialty could have changed so invalidate the cached specialty on the currently selected patient object in PatientL.cs
			PatientL.InvalidateSelectedPatSpecialty();
			PromptForSmsIfNecessary(patientOld,_patient);
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_end",_patient,_patientOld);
			DialogResult=DialogResult.OK;
		}

		private void FormPatientEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(IsNew){
				List<RefAttach> listRefAttaches=RefAttaches.GetRefAttaches(new List<long> { _patient.PatNum });
				for(int i=0;i<listRefAttaches.Count;i++) {
					RefAttaches.Delete(listRefAttaches[i]);
				}
				if(_patient.SuperFamily!=0) {
					_patient.SuperFamily=0;
					Patients.Update(_patient,_patientOld);
				}
				Patients.Delete(_patient);
				SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,_patient.PatNum,Lan.g(this,"Canceled creating new patient. Deleting patient record."));
			}
			if(_hasGuardiansChanged) {  //If guardian information was changed, and user canceled.
				//revert any changes to the guardian list for all family members
				Guardians.RevertChanges(_listGuardiansForFamOld,_family.ListPats.Select(x => x.PatNum).ToList());
			}
		}

	}
}