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
		private string SiteOriginal;
		private bool mouseIsInListSites;
		private List<Site> listSitesFiltered;
		private string countyOriginal;
		private bool mouseIsInListCounties;
		private County[] CountiesList;
		private string empOriginal;//used in the emp dropdown logic
		private bool mouseIsInListEmps;
		///<summary>This is the object that is altered in this form.</summary>
		private Patient PatCur;
		///<summary>This is the object that is altered in this form, since it is an extension of the patient table.</summary>
		private PatientNote _patCurNote;
		//private RefAttach[] RefList;
		private Family FamCur;
		private Patient PatOld;
		///<summary>Will include the languages setup in the settings, and also the language of this patient if that language is not on the selection list.</summary>
		private List<string> languageList;
		private List<Guardian> GuardianList;
		///<summary>If the user presses cancel, use this list to revert any changes to the guardians for all family members.</summary>
		private List<Guardian> _listGuardiansForFamOld;
		///<summary>Local cache of RefAttaches for the current patient.  Set in FillReferrals().</summary>
		private List<RefAttach> _listRefAttaches;
		private System.Windows.Forms.ListBox listMedicaidStates;//displayed from within code, not designer
		private List<RequiredField> _listRequiredFields;
		private string _medicaidStateOriginal;//used in the medicaidState dropdown logic
		private bool _mouseIsInListMedicaidStates;
		private System.Windows.Forms.ListBox listStates;//displayed from within code, not designer
		private string _stateOriginal;//used in the medicaidState dropdown logic
		private bool _mouseIsInListStates;
		private bool _isMissingRequiredFields;
		private bool _isLoad;//To keep track if ListBoxes' selected index is changed by the user
		private ErrorProvider _errorProv=new ErrorProvider();
		private bool _isValidating=false;
		private EhrPatient _ehrPatientCur;
		private List<PatientRace> _listPatRaces;
		private ComboBoxOD comboBoxMultiRace;
		private bool _hasGuardiansChanged=false;
		///<summary>Because adding the new feature where patients can choose their race from hundreds of options would cause us to need to recertify EHR, 
		///we committed all the code for the new feature while keeping the old behavior for EHR users. When we are ready to switch to the new feature, 
		///all we need to do is set this boolean to true (hopefully).</summary>
		private bool _isUsingNewRaceFeature=!PrefC.GetBool(PrefName.ShowFeatureEhr);
		private DefLink _defLinkPatCur;
		private CommOptOut _commOptOut;
		///<summary>List of PatientStatuses shown in listStatus, must be 1:1 with listStatus.
		///Deleted is excluded, unless PatCur is flagged as deleted.
		///Needed due to index differences when deleted is not present.</summary>
		private List<PatientStatus> _listPatStatuses=new List<PatientStatus> ();
		///<summary>Used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.</summary>
		private string _maskedSSNOld="";
		///<summary>Used to keep track of what masked DOB was shown when the form was loaded, and stop us from storing masked DOBs on accident.</summary>
		private string _maskedDOBOld="";

		///<summary></summary>
		public FormPatientEdit(Patient patCur,Family famCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			PatCur=patCur;
			_patCurNote=PatientNotes.Refresh(patCur.PatNum,patCur.Guarantor);
			FamCur=famCur;
			PatOld=patCur.Copy();
			listEmps=new ListBox();
			listEmps.Location=new Point(textEmployer.Left,textEmployer.Bottom);
			listEmps.Size=new Size(textEmployer.Width,100);
			listEmps.Visible=false;
			listEmps.Click += new System.EventHandler(listEmps_Click);
			listEmps.DoubleClick += new System.EventHandler(listEmps_DoubleClick);
			listEmps.MouseEnter += new System.EventHandler(listEmps_MouseEnter);
			listEmps.MouseLeave += new System.EventHandler(listEmps_MouseLeave);
			LayoutManager.Add(listEmps,this);
			listEmps.BringToFront();
			listCounties=new ListBox();
			listCounties.Location=new Point(tabControlPatInfo.Left+tabPublicHealth.Left+textCounty.Left,tabControlPatInfo.Top+tabPublicHealth.Top+textCounty.Bottom);
			listCounties.Visible=false;
			listCounties.Click += new System.EventHandler(listCounties_Click);
			//listCounties.DoubleClick += new System.EventHandler(listCars_DoubleClick);
			listCounties.MouseEnter += new System.EventHandler(listCounties_MouseEnter);
			listCounties.MouseLeave += new System.EventHandler(listCounties_MouseLeave);
			LayoutManager.Add(listCounties,this);
			listCounties.BringToFront();
			listSites=new ListBox();
			listSites.Location=new Point(tabControlPatInfo.Left+tabPublicHealth.Left+textSite.Left,tabControlPatInfo.Top+tabPublicHealth.Top+textSite.Bottom);
			listSites.Size=new Size(textSite.Width,100);
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
			listMedicaidStates=new ListBox();
			listMedicaidStates.Location=new Point(textMedicaidState.Left,textMedicaidState.Bottom);
			listMedicaidStates.Size=new Size(textMedicaidState.Width,100);
			listMedicaidStates.Visible=false;
			listMedicaidStates.Click += new System.EventHandler(listMedicaidStates_Click);
			listMedicaidStates.MouseEnter += new System.EventHandler(listMedicaidStates_MouseEnter);
			listMedicaidStates.MouseLeave += new System.EventHandler(listMedicaidStates_MouseLeave);
			LayoutManager.Add(listMedicaidStates,this);
			listMedicaidStates.BringToFront();
			listStates=new ListBox();
			listStates.Location=new Point(textState.Left+groupBox1.Left,textState.Bottom+groupBox1.Top);
			listStates.Size=new Size(textState.Width,100);
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
				FillRequiredFieldsListHelper();
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
			bool isAuthArchivedEdit=Security.IsAuthorized(Permissions.ArchivedPatientEdit,true);
			List<Patient> listFamPats=FamCur.ListPats.ToList();
			if(!isAuthArchivedEdit) {
				//Exclude Archived pats if user does not have permission.  If user doesn't have the permission and all non-Archived patients pass the
				//the conditions, but there is an Archived patient who does not, the check will still be true.  The Archived will not be updated on OK.
				listFamPats=listFamPats.Where(x => x.PatStatus!=PatientStatus.Archived).ToList();
			}
			#region SameForFamily
			//for the comparison logic to work, any nulls must be converted to ""
			if(PatCur.HmPhone is null) PatCur.HmPhone="";
			if(PatCur.Address is null) PatCur.Address="";
			if(PatCur.Address2 is null) PatCur.Address2="";
			if(PatCur.City is null) PatCur.City="";
			if(PatCur.State is null) PatCur.State="";
			if(PatCur.Zip is null) PatCur.Zip="";
			if(PatCur.Country is null) PatCur.Country="";
			if(PatCur.CreditType is null) PatCur.CreditType="";
			if(PatCur.AddrNote is null) PatCur.AddrNote="";
			if(PatCur.WirelessPhone is null) PatCur.WirelessPhone="";
			if(PatCur.WkPhone is null) PatCur.WkPhone="";
			if(PatCur.Email is null) PatCur.Email="";
			for(int i=0;i<listFamPats.Count;i++){
				if(PatCur.HmPhone!=listFamPats[i].HmPhone
					|| PatCur.Address!=listFamPats[i].Address
					|| PatCur.Address2!=listFamPats[i].Address2
					|| PatCur.City!=listFamPats[i].City
					|| PatCur.State!=listFamPats[i].State
					|| PatCur.Zip!=listFamPats[i].Zip
					|| PatCur.Country!=listFamPats[i].Country)
				{
					checkAddressSame.Checked=false;
				}
				if(PatCur.CreditType!=listFamPats[i].CreditType
					|| PatCur.BillingType!=listFamPats[i].BillingType
					|| PatCur.PriProv!=listFamPats[i].PriProv
					|| PatCur.SecProv!=listFamPats[i].SecProv
					|| PatCur.FeeSched!=listFamPats[i].FeeSched)
				{
					checkBillProvSame.Checked=false;
				}
				if(PatCur.AddrNote!=listFamPats[i].AddrNote){
					checkNotesSame.Checked=false;
				}
				if(PatCur.WirelessPhone!=listFamPats[i].WirelessPhone
					|| PatCur.WkPhone!=listFamPats[i].WkPhone
					|| PatCur.Email!=listFamPats[i].Email) 
				{
					checkEmailPhoneSame.Checked=false;
				}
				if(PatCur.AskToArriveEarly!=listFamPats[i].AskToArriveEarly) {
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
				&& PatCur.SuperFamily!=0
				&& PatCur.PatNum==PatCur.SuperFamily) //Has to be the Super Head.
			{
				checkAddressSameForSuperFam.Visible=true;
				//Check all superfam members for any with differing information
				checkAddressSameForSuperFam.Checked=Patients.SuperFamHasSameAddrPhone(PatCur,isAuthArchivedEdit);
			}
			#endregion SameForFamily
			checkRestrictSched.Checked=PatRestrictionL.IsRestricted(PatCur.PatNum,PatRestrict.ApptSchedule,true);
			textPatNum.Text=PatCur.PatNum.ToString();
			textLName.Text=PatCur.LName;
			textFName.Text=PatCur.FName;
			textMiddleI.Text=PatCur.MiddleI;
			textPreferred.Text=PatCur.Preferred;
			textTitle.Text=PatCur.Title;
			textSalutation.Text=PatCur.Salutation;
			textIceName.Text=_patCurNote.ICEName;
			textIcePhone.Text=_patCurNote.ICEPhone;
			_ehrPatientCur=EhrPatients.Refresh(PatCur.PatNum);
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Show mother's maiden name UI if using EHR.
				labelMotherMaidenFname.Visible=true;
				textMotherMaidenFname.Visible=true;
				textMotherMaidenFname.Text=_ehrPatientCur.MotherMaidenFname;
				labelMotherMaidenLname.Visible=true;
				textMotherMaidenLname.Visible=true;
				textMotherMaidenLname.Text=_ehrPatientCur.MotherMaidenLname;
				labelDeceased.Visible=true;
				dateTimePickerDateDeceased.Visible=true;
			}
			else {
				tabControlPatInfo.TabPages.Remove(tabEHR);
			}
			/*Show checkDoseSpotConsent if DoseSpot is enabled. Currently, consent cannot be revoked with DoseSpot,
			so the check box is checked and disabled if consent was previously given.*/
			Program progErx=Programs.GetCur(ProgramName.eRx);
			ErxOption erxOption=PIn.Enum<ErxOption>(ProgramProperties.GetPropForProgByDesc(progErx.ProgramNum,Erx.PropertyDescs.ErxOption).PropertyValue);
			if(progErx.Enabled && (erxOption==ErxOption.DoseSpot || erxOption==ErxOption.DoseSpotWithLegacy)) {
				checkDoseSpotConsent.Visible=true;
				checkDoseSpotConsent.Checked=_patCurNote.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx);
				if(checkDoseSpotConsent.Checked) {//once checked, should never be able to be unchecked.
					checkDoseSpotConsent.Enabled=false;
				}
			}
			foreach(PatientStatus status in Enum.GetValues(typeof(PatientStatus))) {
				if(status==PatientStatus.Deleted && PatCur.PatStatus!=PatientStatus.Deleted) {
					continue;//Only display 'Deleted' if PatCur is 'Deleted'.  Shouldn't happen, but has been observed.
				}
				_listPatStatuses.Add(status);
				listStatus.Items.Add(Lan.g("enumPatientStatus",status.ToString()));//Not using .GetDescription() because of prior behavior.
			}
			listGender.Items.Add(Lan.g("enumPatientGender","Male"));
			listGender.Items.Add(Lan.g("enumPatientGender","Female"));
			listGender.Items.Add(Lan.g("enumPatientGender","Unknown"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Single"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Married"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Child"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Widowed"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Divorced"));
			listStatus.SetSelected(_listPatStatuses.IndexOf(PatCur.PatStatus),true);//using _listPatStatuses because it is 1:1 with listStatus.
			switch (PatCur.Gender){
				case PatientGender.Male : listGender.SelectedIndex=0; break;
				case PatientGender.Female : listGender.SelectedIndex=1; break;
				case PatientGender.Unknown : listGender.SelectedIndex=2; break;}
			switch (PatCur.Position){
				case PatientPosition.Single : listPosition.SelectedIndex=0; break;
				case PatientPosition.Married : listPosition.SelectedIndex=1; break;
				case PatientPosition.Child : listPosition.SelectedIndex=2; break;
				case PatientPosition.Widowed : listPosition.SelectedIndex=3; break;
				case PatientPosition.Divorced : listPosition.SelectedIndex=4; break;}
			FillGuardians();
			_listGuardiansForFamOld=FamCur.ListPats.SelectMany(x => Guardians.Refresh(x.PatNum)).ToList();
			if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
				//turn off validation until the user changes text, or unmasks.
				odDatePickerBirthDate.SetMaskedDate(Patients.DOBFormatHelper(PatCur.Birthdate,true));//If birthdate is not set this will return ""
				_maskedDOBOld=odDatePickerBirthDate.GetTextDate();
				if(odDatePickerBirthDate.GetTextDate()!="") {
					odDatePickerBirthDate.ReadOnly=true;
				}
				butViewBirthdate.Enabled=(Security.IsAuthorized(Permissions.PatientDOBView,true) && odDatePickerBirthDate.GetTextDate()!="");//Disable if no DOB
				butViewBirthdate.Visible=true;
			}
			else {
				odDatePickerBirthDate.SetDateTime(PatCur.Birthdate);
				//butViewBirthdate is disabled and not visible from designer.
				//Move age back over since butViewBirthdate is not showing
				LayoutManager.MoveLocation(label20,new Point(label20.Location.X-72,label20.Location.Y));
				LayoutManager.MoveLocation(textAge,new Point(textAge.Location.X-72,textAge.Location.Y));
			}
			if(PatCur.DateTimeDeceased.Year > 1880) {
				dateTimePickerDateDeceased.Value=PatCur.DateTimeDeceased;
			}
            else {
				//if there is no datetime deceased, used to show blank instead of the default of todays date.
				dateTimePickerDateDeceased.CustomFormat=" ";
				dateTimePickerDateDeceased.Format=DateTimePickerFormat.Custom;
            }
			textAge.Text=PatientLogic.DateToAgeString(PatCur.Birthdate,PatCur.DateTimeDeceased);
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				textSSN.Text=Patients.SSNFormatHelper(PatCur.SSN,true);//If PatCur.SSN is null or empty, returns empty string.
				_maskedSSNOld=textSSN.Text;
				butViewSSN.Enabled=(Security.IsAuthorized(Permissions.PatientSSNView,true) && textSSN.Text!="");//Disable button if no SSN entered
				butViewSSN.Visible=true;
			}
			else {
				textSSN.Text=Patients.SSNFormatHelper(PatCur.SSN,false);
				//butViewSSN is disabled and not visible from designer.
			}
			textMedicaidID.Text=PatCur.MedicaidID;
			textMedicaidState.Text=_ehrPatientCur.MedicaidState;
			textAddress.Text=PatCur.Address;
			textAddress2.Text=PatCur.Address2;
			textCity.Text=PatCur.City;
			textState.Text=PatCur.State;
			textCountry.Text=PatCur.Country;
			textZip.Text=PatCur.Zip;
			textHmPhone.Text=PatCur.HmPhone;
			textWkPhone.Text=PatCur.WkPhone;
			textWirelessPhone.Text=PatCur.WirelessPhone;
			listTextOk.SelectedIndex=(int)PatCur.TxtMsgOk;
			FillShortCodes(PatCur.TxtMsgOk);
			textEmail.Text=PatCur.Email;
			textCreditType.Text=PatCur.CreditType;
			odDatePickerDateFirstVisit.SetDateTime(PatCur.DateFirstVisit);
			if(PatCur.AskToArriveEarly>0){
				textAskToArriveEarly.Text=PatCur.AskToArriveEarly.ToString();
			}
			textChartNumber.Text=PatCur.ChartNumber;
			textEmployer.Text=Employers.GetName(PatCur.EmployerNum);
			//textEmploymentNote.Text=PatCur.EmploymentNote;
			languageList=new List<string>();
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)!="") {
				string[] lanstring=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');
				for(int i=0;i<lanstring.Length;i++) {
					if(lanstring[i]=="") {
						continue;
					}
					languageList.Add(lanstring[i]);
				}
			}
			if(PatCur.Language!="" && PatCur.Language!=null && !languageList.Contains(PatCur.Language)) {
				languageList.Add(PatCur.Language);
			}
			comboLanguage.Items.Add(Lan.g(this,"None"));//regardless of how many languages are listed, the first item is "none"
			comboLanguage.SelectedIndex=0;
			for(int i=0;i<languageList.Count;i++) {
				if(languageList[i]=="") {
					continue;
				}
				CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(languageList[i]);
				if(culture==null) {//custom language
					comboLanguage.Items.Add(languageList[i]);
				}
				else {
					comboLanguage.Items.Add(culture.DisplayName);
				}
				if(PatCur.Language==languageList[i]) {
					comboLanguage.SelectedIndex=i+1;
				}
			}
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.Add(Lan.g(this,"None"),new FeeSched());
			comboFeeSched.SelectedIndex=0;
			List<FeeSched> listFeeScheds=FeeScheds.GetDeepCopy(false);
			foreach(FeeSched feeSched in listFeeScheds) {
				if(feeSched.IsHidden && feeSched.FeeSchedNum!=PatCur.FeeSched) {
					continue;//skip hidden fee schedules as long as not assigned to this patient. This will only occur in rare occurrences.
				}
				comboFeeSched.Items.Add(feeSched.Description+(feeSched.IsHidden ? " "+Lans.g(this,"(Hidden)") : ""),feeSched);
				if(feeSched.FeeSchedNum==PatCur.FeeSched) {
					comboFeeSched.SelectedIndex=comboFeeSched.Items.Count-1;
				}
			}
			comboBillType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes,true));
			comboBillType.SetSelectedDefNum(PatCur.BillingType); 
			if(comboBillType.SelectedIndex==-1){
				if(comboBillType.Items.Count==0) {
					MsgBox.Show(this,"Error.  All billing types have been hidden.  Please go to Definitions and unhide at least one.");
					DialogResult=DialogResult.Cancel;
					return;
				}
			}
			if(!Security.IsAuthorized(Permissions.PatientBillingEdit,true)){
				//labelBillType.Visible=true;
				comboBillType.Enabled=false;
			}
			if(!Security.IsAuthorized(Permissions.PatientApptRestrict,true)) {
				checkRestrictSched.Enabled=false;
			}
			if(PatCur.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(Permissions.ArchivedPatientEdit,false)) {
				butReferredFrom.Enabled=false;
				textReferredFrom.Enabled=false;
				butOK.Enabled=false;
			}
			comboClinic.SelectedClinicNum=PatCur.ClinicNum;
			FillCombosProv();
			comboPriProv.SetSelectedProvNum(PatCur.PriProv);
			comboSecProv.SetSelectedProvNum(PatCur.SecProv);
			if(!Security.IsAuthorized(Permissions.PatPriProvEdit,DateTime.MinValue,true,true) && PatCur.PriProv>0) {
				//user not allowed to change existing prov.  Warning messages are suppressed here.
				string strToolTip=Lan.g("Security","Not authorized for")+" "+GroupPermissions.GetDesc(Permissions.PatPriProvEdit);
				_priProvEditToolTip.SetToolTip(butPickPrimary,strToolTip);
				_priProvEditToolTip.SetToolTip(comboPriProv,strToolTip);
				comboPriProv.Enabled=false;
			}
			switch(PatCur.StudentStatus){
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
			textSchool.Text=PatCur.SchoolName;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSchoolName.Text=Lan.g(this,"Name of School");
				comboCanadianEligibilityCode.Items.Add("0 - Please Choose");
				comboCanadianEligibilityCode.Items.Add("1 - Full-time student");
				comboCanadianEligibilityCode.Items.Add("2 - Disabled");
				comboCanadianEligibilityCode.Items.Add("3 - Disabled student");
				comboCanadianEligibilityCode.Items.Add("4 - Code not applicable");
				comboCanadianEligibilityCode.SelectedIndex=PatCur.CanadianEligibilityCode;
			}
			textAddrNotes.Text=PatCur.AddrNote;
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
				_listPatRaces=PatientRaces.GetForPatient(PatCur.PatNum);
				textRace.Text=PatientRaces.GetRaceDescription(_listPatRaces);
				textEthnicity.Text=PatientRaces.GetEthnicityDescription(_listPatRaces);
				comboBoxMultiRace.Visible=false;
				comboEthnicity.Visible=false;
			}
			else {
				textRace.Visible=false;
				textEthnicity.Visible=false;
				butRaceEthnicity.Visible=false;
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","None"));//0
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","AfricanAmerican"));//1
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","AmericanIndian"));//2
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","Asian"));//3
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","DeclinedToSpecify"));//4
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","HawaiiOrPacIsland"));//5
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","Other"));//6
				comboBoxMultiRace.Items.Add(Lan.g("enumPatRace","White"));//7
				comboEthnicity.Items.Add(Lan.g(this,"None"));//0 
				comboEthnicity.Items.Add(Lan.g(this,"DeclinedToSpecify"));//1
				comboEthnicity.Items.Add(Lan.g(this,"Not Hispanic"));//2
				comboEthnicity.Items.Add(Lan.g(this,"Hispanic"));//3
				List<PatientRace> listPatRaces=PatientRaces.GetForPatient(PatCur.PatNum);
				comboEthnicity.SelectedIndex=0;//none
				foreach(PatientRace race in listPatRaces) {
					switch(race.CdcrecCode) {
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
							if(race.IsEthnicity) {
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
			textCounty.Text=PatCur.County;
			textSite.Text=Sites.GetDescription(PatCur.SiteNum);
			string[] enumGrade=Enum.GetNames(typeof(PatientGrade));
			for(int i=0;i<enumGrade.Length;i++){
				comboGradeLevel.Items.Add(Lan.g("enumGrade",enumGrade[i]));
			}
			comboGradeLevel.SelectedIndex=(int)PatCur.GradeLevel;
			string[] enumUrg=Enum.GetNames(typeof(TreatmentUrgency));
			for(int i=0;i<enumUrg.Length;i++){
				comboUrgency.Items.Add(Lan.g("enumUrg",enumUrg[i]));
			}
			comboUrgency.SelectedIndex=(int)PatCur.Urgency;
			if(PatCur.ResponsParty!=0){
				textResponsParty.Text=Patients.GetLim(PatCur.ResponsParty).GetNameLF();
			}
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)){
				textTrophyFolder.Text=PatCur.TrophyFolder;
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
			textWard.Text=PatCur.Ward;
			for(int i=0;i<Enum.GetNames(typeof(ContactMethod)).Length;i++){
				comboContact.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
				comboConfirm.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
				comboRecall.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
			}
			comboContact.SelectedIndex=(int)PatCur.PreferContactMethod;
			comboConfirm.SelectedIndex=(int)PatCur.PreferConfirmMethod;
			comboRecall.SelectedIndex=(int)PatCur.PreferRecallMethod;
			_commOptOut=CommOptOuts.Refresh(PatCur.PatNum);
			int idx=1; //Starts at 1 to account for "None"
			comboExcludeECR.Items.Add("None");
			foreach(CommOptOutMode mode in Enum.GetValues(typeof(CommOptOutMode))) {
				comboExcludeECR.Items.Add(mode.GetDescription(),mode);
				if(_commOptOut.IsOptedOut(mode,CommOptOutType.All)) {
					comboExcludeECR.SetSelected(idx,true);
				}
				idx++;
			}
			if(comboExcludeECR.SelectedIndices.Count==0) {
				comboExcludeECR.SetSelected(0);
			}
			if(PatCur.AdmitDate.Year>1880){
				odDatePickerAdmitDate.SetDateTime(PatCur.AdmitDate);
			}
			FillReferrals();
			if(HL7Defs.IsExistingHL7Enabled()) {
				if(HL7Defs.GetOneDeepEnabled().ShowDemographics==HL7ShowDemographics.Show) {//If show, then not edit so disable OK button
					butOK.Enabled=false;
				}
			}
			if(PrefC.GetBool(PrefName.ShowFeatureGoogleMaps)) {
				butShowMap.Visible=true;
			}
			_errorProv.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)
				&& PatCur.SuperFamily!=0 
				&& PatCur.Guarantor==PatCur.PatNum) 
			{
				//If the patient is a guarantor in a superfamily then enable the checkbox to opt into superfamily billing.
				checkSuperBilling.Visible=true;
				checkSuperBilling.Checked=PatCur.HasSuperBilling;
			}
			//Loop through the SexOrientation enum and display the Description attribute. If the Snomed attribute equals the patient's SexualOrientation, 
			//set that item as the selected index.
			foreach(SexOrientation sexOrientEnum in (SexOrientation[])Enum.GetValues(typeof(SexOrientation))) {
				comboSexOrientation.Items.Add(sexOrientEnum.GetDescription());
				if(_ehrPatientCur.SexualOrientation==EnumTools.GetAttributeOrDefault<EhrAttribute>(sexOrientEnum).Snomed) {
					comboSexOrientation.SelectedIndex=comboSexOrientation.Items.Count-1;//Make the last added item the selected one
				}
			}
			textSpecifySexOrientation.Text=_ehrPatientCur.SexualOrientationNote;
			//Loop through the GenderId enum and display the Description attribute. If the Snomed attribute equals the patient's GenderIdentity, 
			//set that item as the selected index.
			foreach(GenderId genderIdEnum in (GenderId[])Enum.GetValues(typeof(GenderId))) {
				comboGenderIdentity.Items.Add(genderIdEnum.GetDescription());
				if(_ehrPatientCur.GenderIdentity==EnumTools.GetAttributeOrDefault<EhrAttribute>(genderIdEnum).Snomed) {
					comboGenderIdentity.SelectedIndex=comboGenderIdentity.Items.Count-1;//Make the last added item the selected one
				}
			}
			textSpecifyGender.Text=_ehrPatientCur.GenderIdentityNote;
			SetRequiredFields();
			//Selecting textLName must happen at the end of load to avoid events from accessing class wide variables that have yet to be loaded.
			//This was a bug because calling Select() was firing textBox_Leave which was accessing _listRequiredFields while it was null.
			textLName.Select();
			FillSpecialty();
			FillComboZip();
			_isLoad=false;
			checkBoxSignedTil.Checked=PatCur.HasSignedTil;
			Plugins.HookAddCode(this,"FormPatientEdit.Load_end",PatCur);
		}

		private void butShortCodeOptIn_Click(object sender,EventArgs e) {
			using FormShortCodeOptIn formSC=new FormShortCodeOptIn(PatCur);
			if(formSC.ShowDialog()==DialogResult.OK) {
				FillShortCodes((YN)listTextOk.SelectedIndex);
			}
		}

		private void FillShortCodes(YN txtMsgOk) {
			Patient pat=PatCur.Copy();
			pat.TxtMsgOk=txtMsgOk;
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(pat)).FirstOrDefault();
			bool isShortCodeInfoNeeded=patComm?.IsPatientShortCodeEligible(pat.ClinicNum)??false;
			butShortCodeOptIn.Visible=isShortCodeInfoNeeded;
			labelApptTexts.Visible=isShortCodeInfoNeeded;
			listBoxApptTexts.Visible=isShortCodeInfoNeeded;
			if(isShortCodeInfoNeeded) {
				string optIn;
				switch(pat.ShortCodeOptIn) {
					case YN.Yes:
						optIn="Yes";
						break;
					case YN.No:
						optIn="No";
						break;
					case YN.Unknown:
					default:
						optIn="??";
						break;
				}
				listBoxApptTexts.Items.Clear();
				listBoxApptTexts.Items.Add(optIn);
				listBoxApptTexts.SelectedIndex=0;
			}
		}

		private void FillSpecialty() {
			//Get all non-hidden specialties
			comboSpecialty.Items.Clear();
			//Create a dummy specialty of 0 if there no specialties created.
			comboSpecialty.Items.AddDefNone(Lan.g(this,"Unspecified"));
			comboSpecialty.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ClinicSpecialty,true));
			_defLinkPatCur=DefLinks.GetOneByFKey(PatCur.PatNum,DefLinkType.Patient);
			if(_defLinkPatCur==null) {
				comboSpecialty.SetSelectedDefNum(0);
			}
			else{
				comboSpecialty.SetSelectedDefNum(_defLinkPatCur.DefNum);
			}
		}

		private void butPickPrimary_Click(object sender,EventArgs e) {
			if(PatCur.PriProv>0 && !Security.IsAuthorized(Permissions.PatPriProvEdit)) {
				return;
			}
			using FormProviderPick formp = new FormProviderPick(comboPriProv.Items.GetAll<Provider>());
			formp.SelectedProvNum=comboPriProv.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboPriProv.SetSelectedProvNum(formp.SelectedProvNum);
		}

		private void butPickSecondary_Click(object sender,EventArgs e) {
			using FormProviderPick formp = new FormProviderPick(comboSecProv.Items.GetAll<Provider>());
			formp.SelectedProvNum=comboSecProv.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboSecProv.SetSelectedProvNum(formp.SelectedProvNum);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			long provNum=comboPriProv.GetSelectedProvNum();
			List<Provider> listProviders=Providers.GetProvsForClinic(comboClinic.SelectedClinicNum);
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
				&& FamCur.ListPats.Any(x => x.PatNum!=PatCur.PatNum && x.PriProv!=comboPriProv.GetSelectedProvNum()) //a family member has a different PriProv
				&& !Security.IsAuthorized(Permissions.PatPriProvEdit)) //user is not authorized to change PriProv, warning message displays
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
			foreach(ZipCode zip in listZipCodes) {
				comboZip.Items.Add(zip.ZipCodeDigits+" ("+zip.City+")",zip);
			}
		}

		private void FillGuardians(){
			GuardianList=Guardians.Refresh(PatCur.PatNum);
			listRelationships.Items.Clear();
			for(int i=0;i<GuardianList.Count;i++){
				listRelationships.Items.Add(FamCur.GetNameInFamFirst(GuardianList[i].PatNumGuardian)+" "
					+Guardians.GetGuardianRelationshipStr(GuardianList[i].Relationship));
			}
		}

		///<summary>Fills _listRequiredFields from the cache with required fields that are visible on this form.</summary>
		private void FillRequiredFieldsListHelper() {
			_listRequiredFields=RequiredFields.GetWhere(x => x.FieldType==RequiredFieldType.PatientInfo);
			//Remove the RequiredFields that are only on the Add Family window
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.InsuranceSubscriber);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.InsuranceSubscriberID);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Carrier);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.InsurancePhone);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GroupName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GroupNum);
			//Remove RequiredFields where the text field is invisible.
			if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenFirstName);
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenLastName);
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DateTimeDeceased);
			}
			if(!Programs.IsEnabled(Programs.GetProgramNum(ProgramName.TrophyEnhanced))) {
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.TrophyFolder);
			}
			if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Ward);
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.AdmitDate);
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) { //Canadian. en-CA or fr-CA
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.StudentStatus);
			}
			else {//Not Canadian
				_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EligibilityExceptCode);
			}
			//Remove Required Fields if the Public Health Tab(tabPublicHealth) is hidden
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				_listRequiredFields.RemoveAll(x => ListTools.In(x.FieldName,
						RequiredFieldName.Race,RequiredFieldName.Ethnicity,RequiredFieldName.County,
						RequiredFieldName.Site,RequiredFieldName.GradeLevel,RequiredFieldName.TreatmentUrgency,
						RequiredFieldName.ResponsibleParty,RequiredFieldName.SexualOrientation,RequiredFieldName.GenderIdentity
				));
			}
		}

		///<summary>Puts an asterisk next to the label and highlights the textbox/listbox/combobox/radiobutton background for all RequiredFields that
		///have their conditions met.</summary>
		private void SetRequiredFields() {
			_isMissingRequiredFields=false;
			bool areAnyConditionsMet=false;
			bool areConditionsMet;
			if(_listRequiredFields==null) {
				FillRequiredFieldsListHelper();
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
									_errorProv.SetError(textAddrNotes,Lan.g(this,"Text box cannot be blank"));
								}
							}
							else {
								_errorProv.SetError(textAddrNotes,"");
							}
						}
						else {
							if(groupNotes.Text.Contains("*")) {
								groupNotes.Text=groupNotes.Text.Replace("*","");
							}
							_errorProv.SetError(textAddrNotes,"");
						}
						break;
					case RequiredFieldName.AdmitDate:
						SetRequiredControl(labelAdmitDate,odDatePickerAdmitDate,areConditionsMet,-1,new List<int>(),"Hospital admit date must be selected.");
;						break;
					case RequiredFieldName.AskArriveEarly:
						SetRequiredTextBox(labelAskToArriveEarly,textAskToArriveEarly,areConditionsMet);
						break;
					case RequiredFieldName.BillingType:
						SetRequiredComboBoxPlus(labelBillType,comboBillType,areConditionsMet,-1,"A billing type must be selected.");
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
						SetRequiredControl(labelDeceased,dateTimePickerDateDeceased,areConditionsMet,-1,new List<int>(),"Date-Time deceased must be entered.");
						break;
					case RequiredFieldName.EligibilityExceptCode:
						SetRequiredComboBox(labelCanadianEligibilityCode,comboCanadianEligibilityCode,areConditionsMet,0,
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
							SetRequiredComboBox(labelEthnicity,comboEthnicity,areConditionsMet,0,"Selection cannot be 'None'.");
						}
						break;
					case RequiredFieldName.FeeSchedule:
						SetRequiredComboBoxPlus(labelFeeSched,comboFeeSched,areConditionsMet,0,"Selection cannot be 'None'.");
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
									_errorProv.SetError(listGender,Lan.g(this,"Gender cannot be 'Unknown'."));
									_errorProv.SetIconAlignment(listGender,ErrorIconAlignment.BottomRight);
								}
								_isMissingRequiredFields=true;
							}
							else {
								_errorProv.SetError(listGender,"");
							}
						}
						else {
							_errorProv.SetError(listGender,"");
						}
						break;
					case RequiredFieldName.GenderIdentity:
						SetRequiredComboBox(labelGenderIdentity,comboGenderIdentity,areConditionsMet,-1,"A gender identity must be selected.");
						break;
					case RequiredFieldName.GradeLevel:
						SetRequiredComboBox(labelGradeLevel,comboGradeLevel,areConditionsMet,0,"Selection cannot be 'Unknown'.");
						break;
					case RequiredFieldName.HomePhone:
						SetRequiredTextBox(labelHmPhone,textHmPhone,areConditionsMet);
						break;
					case RequiredFieldName.Language:
						SetRequiredComboBox(labelLanguage,comboLanguage,areConditionsMet,0,"Selection cannot be 'None'.");
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
								_errorProv.SetError(textMedicaidState,Lan.g(this,"Invalid state abbreviation"));
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
						SetRequiredComboBox(labelConfirm,comboConfirm,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferContactMethod:
						SetRequiredComboBox(labelContact,comboContact,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferRecallMethod:
						SetRequiredComboBox(labelRecall,comboRecall,areConditionsMet,0,"Selection cannot be 'None'.");
						break;
					case RequiredFieldName.PreferredName:
						SetRequiredTextBox(labelPreferredAndMiddleI,textPreferred,areConditionsMet);
						break;
					case RequiredFieldName.PrimaryProvider:
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							SetRequiredComboBoxPlus(labelPriProv,comboPriProv,areConditionsMet,0,"Selection cannot be 'Select Provider'.");
						}
						break;
					case RequiredFieldName.Race:
						if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							SetRequiredComboBoxPlus(labelRace,comboBoxMultiRace,areConditionsMet,new List<int> {0},"Race is required");
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
						SetRequiredComboBox(labelSexOrientation,comboSexOrientation,areConditionsMet,-1,"A sexual orientation must be selected.");
						break;
					case RequiredFieldName.Salutation:
						SetRequiredTextBox(labelSalutation,textSalutation,areConditionsMet);
						break;
					case RequiredFieldName.SecondaryProvider:
						SetRequiredComboBoxPlus(labelSecProv,comboSecProv,areConditionsMet,0,"Selection cannot be 'None'.");
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
								_errorProv.SetError(textState,Lan.g(this,"Invalid state abbreviation"));
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
									_errorProv.SetError(radioStudentP,"A student status must be selected.");
								}
							}
							else {
								_errorProv.SetError(radioStudentP,"");
							}
						}
						else {
							_errorProv.SetError(radioStudentP,"");
						}
						break;
					case RequiredFieldName.TextOK:
						labelTextOk.Text=labelTextOk.Text.Replace("*","");						
						if(areConditionsMet) {
							labelTextOk.Text=labelTextOk.Text+"*";
							if(listTextOk.SelectedIndex>-1 && listTextOk.Items[listTextOk.SelectedIndex].ToString()==Lan.g(this,"??")) {
								_isMissingRequiredFields=true;
								if(_isValidating) {
									_errorProv.SetError(listTextOk,Lan.g(this,"Selection cannot be '??'."));
								}
							}
							else {
								_errorProv.SetError(listTextOk,"");
							}
						}
						else {
							_errorProv.SetError(listTextOk,"");
						}
						break;
					case RequiredFieldName.Title:
						SetRequiredTextBox(labelTitle,textTitle,areConditionsMet);
						break;
					case RequiredFieldName.TreatmentUrgency:
						SetRequiredComboBox(labelUrgency,comboUrgency,areConditionsMet,0,"Selection cannot be 'Unknown'.");
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
			FillShortCodes((YN)listTextOk.SelectedIndex);
		}

		///<summary>Returns true if all the conditions for the RequiredField are met.</summary>
		private bool ConditionsAreMet(RequiredField reqField) {
			List<RequiredFieldCondition> listConditions=reqField.ListRequiredFieldConditions;
			if(listConditions.Count==0) {//This RequiredField is always required
				return true;
			}
			bool areConditionsMet=false;
			int previousFieldName=-1;
			for(int i=0;i<listConditions.Count;i++) {
				if(areConditionsMet && (int)listConditions[i].ConditionType==previousFieldName) {
					continue;//A condition of this type has already been met
				}
				if(!areConditionsMet && previousFieldName!=-1
					&& (int)listConditions[i].ConditionType!=previousFieldName) 
				{
					return false;//None of the conditions of the previous type were met
				}
				areConditionsMet=false;
				switch(listConditions[i].ConditionType) {
					case RequiredFieldName.AdmitDate:
						if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
							areConditionsMet=true;
							break;
						}
						areConditionsMet=CheckDateConditions(odDatePickerAdmitDate.GetDateTime().ToString(),i,listConditions);
						break;
					case RequiredFieldName.BillingType:
						//Conditions of type BillingType store the DefNum as the ConditionValue.
						areConditionsMet=ConditionComparerHelper(comboBillType.GetSelectedDefNum().ToString(),i,listConditions);
						break;
					case RequiredFieldName.Birthdate://But actually using Age for calculations						
						if(textAge.Text=="") {
							areConditionsMet=false;
							break;
						}
						if(!odDatePickerBirthDate.ReadOnly //if not masked
							&& !IsBirthdateValid()) 
						{
							areConditionsMet=false;
							break;
						}
						DateTime birthdate=PIn.Date(odDatePickerBirthDate.GetDateTime().ToString());
						int ageEntered=DateTime.Today.Year-birthdate.Year;
						if(birthdate>DateTime.Today.AddYears(-ageEntered)) {
							ageEntered--;
						}
						List<RequiredFieldCondition> listAgeConditions=listConditions.FindAll(x => x.ConditionType==RequiredFieldName.Birthdate);
						//There should be no more than 2 conditions of type Birthdate
						List<bool> listAreCondsMet=new List<bool>();
						for(int j=0;j<listAgeConditions.Count;j++) {
							listAreCondsMet.Add(CondOpComparer(ageEntered,listAgeConditions[j].Operator,PIn.Int(listAgeConditions[j].ConditionValue)));
						}
						if(listAreCondsMet.Count<2 || listAgeConditions[1].ConditionRelationship==LogicalOperator.And) {
							areConditionsMet=!listAreCondsMet.Contains(false);
							break;
						}
						areConditionsMet=listAreCondsMet.Contains(true);
						break;
					case RequiredFieldName.Clinic:
						if(!PrefC.HasClinicsEnabled) {
							areConditionsMet=true;
							break;
						}
						areConditionsMet=ConditionComparerHelper(comboClinic.SelectedClinicNum.ToString(),i,listConditions);//includes none clinic
						break;								
					case RequiredFieldName.DateTimeDeceased:
						if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							areConditionsMet=true;
							break;
						}
						areConditionsMet=CheckDateConditions(dateTimePickerDateDeceased.Text,i,listConditions);
						break;
					case RequiredFieldName.Gender:
						areConditionsMet=ConditionComparerHelper(listGender.Items.GetTextShowingAt(listGender.SelectedIndex),i,listConditions);
						break;
					case RequiredFieldName.Language:
						areConditionsMet=ConditionComparerHelper(comboLanguage.Items[comboLanguage.SelectedIndex].ToString(),i,listConditions);
						break;
					case RequiredFieldName.MedicaidID:
						if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
							areConditionsMet=true;
							break;
						}
						//The only possible value for ConditionValue is 'Blank'
						if((listConditions[i].Operator==ConditionOperator.Equals && textMedicaidID.Text=="")
							|| (listConditions[i].Operator==ConditionOperator.NotEquals && textMedicaidID.Text!="")) {
							areConditionsMet=true;
						}
						break;
					case RequiredFieldName.MedicaidState:
						if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
							areConditionsMet=true;
							break;
						}
						//The only possible value for ConditionValue is '' (an empty string)
						if((listConditions[i].Operator==ConditionOperator.Equals && textMedicaidState.Text=="")
							|| (listConditions[i].Operator==ConditionOperator.NotEquals && textMedicaidState.Text!="")) {
							areConditionsMet=true;
						}
						break;
					case RequiredFieldName.PatientStatus:
						areConditionsMet=ConditionComparerHelper(listStatus.Items.GetTextShowingAt(listStatus.SelectedIndex),i,listConditions);
						break;
					case RequiredFieldName.Position:
						areConditionsMet=ConditionComparerHelper(listPosition.Items.GetTextShowingAt(listPosition.SelectedIndex),i,listConditions);
						break;
					case RequiredFieldName.PrimaryProvider:
						//Conditions of type PrimaryProvider store the ProvNum as the ConditionValue.
						areConditionsMet=ConditionComparerHelper(comboPriProv.GetSelectedProvNum().ToString(),i,listConditions);
						break;							
					case RequiredFieldName.StudentStatus:
						areConditionsMet=CheckStudentStatusConditions(i,listConditions);
						break;
				}
				previousFieldName=(int)listConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		///<summary>Returns true if the operator is Equals and the value is in the list of conditions or if the operator is NotEquals and the value is 
		///not in the list of conditions.</summary>
		private bool ConditionComparerHelper(string val,int condCurIndex,List<RequiredFieldCondition> listConds) {
			RequiredFieldCondition conditionCur = listConds[condCurIndex];//Variable for convenience
			switch(conditionCur.Operator) {
				case ConditionOperator.Equals:
					return listConds.Any(x => x.ConditionType==conditionCur.ConditionType && x.ConditionValue==val);
				case ConditionOperator.NotEquals:
					return !listConds.Any(x => x.ConditionType==conditionCur.ConditionType && x.ConditionValue==val);
				default:
					return false;
			}
		}

		///<summary>Returns true if the conditions for this date condition are true.</summary>
		private bool CheckDateConditions(string dateStr,int condCurIndex,List<RequiredFieldCondition> listConds) {
			DateTime dateCur=DateTime.MinValue;
			if(dateStr=="" || !DateTime.TryParse(dateStr,out dateCur)) {
				return false;
			}
			List<RequiredFieldCondition> listDateConditions=listConds.FindAll(x => x.ConditionType==listConds[condCurIndex].ConditionType);
			if(listDateConditions.Count<1) {
				return false;
			}
			//There should be no more than 2 conditions of a date type
			List<bool> listAreCondsMet=new List<bool>();
			for(int i=0;i<listDateConditions.Count;i++) {
				listAreCondsMet.Add(CondOpComparer(dateCur,listDateConditions[i].Operator,PIn.Date(listDateConditions[i].ConditionValue)));
			}
			if(listAreCondsMet.Count<2 || listDateConditions[1].ConditionRelationship==LogicalOperator.And) {
				return !listAreCondsMet.Contains(false);
			}
			return listAreCondsMet.Contains(true);
		}

		///<summary>Returns true if the conditions for StudentStatus are true.</summary>
		private bool CheckStudentStatusConditions(int condCurIndex,List<RequiredFieldCondition> listConds) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) { //Canadian. en-CA or fr-CA
				return true;
			}
			if(listConds[condCurIndex].Operator==ConditionOperator.Equals) {
				if((radioStudentN.Checked && listConds[condCurIndex].ConditionValue==Lan.g(this,"Nonstudent"))
					|| (radioStudentF.Checked && listConds[condCurIndex].ConditionValue==Lan.g(this,"Fulltime"))
					|| (radioStudentP.Checked && listConds[condCurIndex].ConditionValue==Lan.g(this,"Parttime")))
				{
					return true;
				}
				return false;
			}
			else { //condCur.Operator==ConditionOperator.NotEquals
				List<RequiredFieldCondition> listStudentConds=listConds.FindAll(x => x.ConditionType==RequiredFieldName.StudentStatus);
				if((radioStudentN.Checked && listStudentConds.Any(x => x.ConditionValue==Lan.g(this,"Nonstudent")))
					|| (radioStudentF.Checked && listStudentConds.Any(x => x.ConditionValue==Lan.g(this,"Fulltime")))
					|| (radioStudentP.Checked && listStudentConds.Any(x => x.ConditionValue==Lan.g(this,"Parttime"))))
				{
					return false;
				}
				return true;
			}
		}

		///<summary>Evaluates two dates using the provided operator.</summary>
		private bool CondOpComparer(DateTime date1,ConditionOperator oper,DateTime date2) {
			return CondOpComparer(DateTime.Compare(date1,date2),oper,0);
		}

		///<summary>Evaluates two integers using the provided operator.</summary>
		private bool CondOpComparer(int value1,ConditionOperator oper,int value2) {
			switch(oper) {
				case ConditionOperator.Equals:
					return value1==value2;
				case ConditionOperator.NotEquals:
					return value1!=value2;
				case ConditionOperator.GreaterThan:
					return value1>value2;
				case ConditionOperator.GreaterThanOrEqual:
					return value1>=value2;
				case ConditionOperator.LessThan:
					return value1<value2;
				case ConditionOperator.LessThanOrEqual:
					return value1<=value2;
			}
			return false;
		}

		///<summary>Checks to see if the Medicaid ID is the proper number of digits for the Medicaid State.</summary>
		private void CheckMedicaidIDLength() {
			int reqLength=StateAbbrs.GetMedicaidIDLength(textMedicaidState.Text);
			if(reqLength==0 || reqLength==textMedicaidID.Text.Length) {
				return;
			}
			_isMissingRequiredFields=true;
			if(_isValidating) {
				_errorProv.SetError(textMedicaidID,Lan.g(this,"Medicaid ID length must be ")+reqLength.ToString()+Lan.g(this," digits for the state of ")
					+textMedicaidState.Text+".");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it also blank, sets the error provider.
		///</summary>
		private void SetRequiredTextBox(Label labelCur,TextBox textBoxCur,bool areConditionsMet) {
			SetRequiredControl(labelCur,textBoxCur,areConditionsMet,-1,new List<int>(),"Text box cannot be blank.");
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///sets the error provider.</summary>
		private void SetRequiredComboBox(Label labelCur,ComboBox comboCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			SetRequiredControl(labelCur,comboCur,areConditionsMet,disallowedIdx,new List<int>(),errorMsg);
		}

		private void SetRequiredComboBoxPlus(Label labelCur,ComboBoxOD comboCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			SetRequiredControl(labelCur,comboCur,areConditionsMet,disallowedIdx,new List<int>(),errorMsg);
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If a disallowedIndices is also selected, sets the error provider.</summary>
		private void SetRequiredComboBoxPlus(Label labelCur,ComboBoxOD comboCur,bool areConditionsMet,List<int> disallowedIndices,string errorMsg) {
			SetRequiredControl(labelCur,comboCur,areConditionsMet,-1,disallowedIndices,errorMsg);
		}	

		private void SetRequiredControl(Label labelCur,Control contr,bool areConditionsMet,int disallowedIdx,List<int> disallowedIndices,
			string errorMsg) 
		{
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if((contr is ComboBox && ((ComboBox)contr).SelectedIndex==disallowedIdx)
					|| (contr is ComboBoxOD && ((ComboBoxOD)contr).SelectionModeMulti && ((ComboBoxOD)contr).SelectedIndices.Exists(x => disallowedIndices.Exists(y => y==x)))
					|| (contr is ComboBoxOD && !((ComboBoxOD)contr).SelectionModeMulti && ((ComboBoxOD)contr).SelectedIndex==disallowedIdx)
					|| (contr is TextBox && ((TextBox)contr).Text=="")) 
				{
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProv.SetError(contr,Lan.g(this,errorMsg));
					}
				}
				else {
					_errorProv.SetError(contr,"");
				}
			}
			else {
				labelCur.Text=labelCur.Text.Replace("*","")+"";
				_errorProv.SetError(contr,"");
			}
			if(contr.Name==textSite.Name || contr.Name==textReferredFrom.Name) {
				_errorProv.SetIconPadding(contr,25);//Width of the pick button
			}
			else if(contr.Name==textResponsParty.Name) {
				_errorProv.SetIconPadding(contr,50);//Width of the pick and remove buttons
			}
			else if(contr.Name==comboPriProv.Name || contr.Name==comboSecProv.Name) {
				_errorProv.SetIconPadding(contr,25);//Width of the pick button
			}
			//If the control is on a tab, add an asterisk to the tab title.
			Control parent=labelCur.Parent;
			while(parent!=null) {
				if(parent is TabPage) {
					((TabPage)parent).Text=((TabPage)parent).Text.Replace("*","")+"*";
				}
				parent=parent.Parent;
			}
		}

		private void SetRequiredClinic(bool areConditionsMet){
			//no easy way to show clinic is required. We don't add and remove star from label.
			if(areConditionsMet) {
				if(comboClinic.SelectedClinicNum==0){
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProv.SetError(comboClinic,Lan.g(this,"Selection cannot be 'Unassigned'."));
					}
				}
				else {
					_errorProv.SetError(comboClinic,"");
				}
			}
			else {
				_errorProv.SetError(comboClinic,"");
			}
		}
		
		private void textBox_Leave(object sender,System.EventArgs e) {
			SetRequiredFields();
		}
		
		private void ListBox_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(_isLoad) {
				return;
			}
			SetRequiredFields();
			Plugins.HookAddCode(this,"FormPatientEdit.ListBox_SelectedIndexChanged_end",PatCur);
		}

		private void ComboBox_SelectionChangeCommited(object sender,System.EventArgs e) {
			SetRequiredFields();
			if(sender==comboClinic){
				FillCombosProv();
			}
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
			PatCur.StudentStatus="N";
			SetRequiredFields();
		}

		private void radioStudentF_Click(object sender, System.EventArgs e) {
			PatCur.StudentStatus="F";
			SetRequiredFields();
		}

		private void radioStudentP_Click(object sender, System.EventArgs e) {
			PatCur.StudentStatus="P";
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
			if(PrefC.GetBool(PrefName.PatientSSNMasked) && textSSN.Text==_maskedSSNOld) {//If SSN hasn't changed, don't validate.  It is masked.
				return;
			}
			if(textSSN.Text.Length==9){//if just numbers, try to reformat.
				bool SSNisValid=true;
				for(int i=0;i<textSSN.Text.Length;i++){
					if(!Char.IsNumber(textSSN.Text,i)){
						SSNisValid=false;
					}
				}
				if(SSNisValid){
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

		private void textDateDeceased_Validated(object sender,EventArgs e) {
			CalcAge();
		}

		private bool IsBirthdateValid() {
			//We do it this way to maintain the very useful behavior where it adds /'s for you when leaving.
			ValidDate birthdayCheck=new ValidDate();
			birthdayCheck.Text=odDatePickerBirthDate.GetTextDate();//Use text date for slash validation and formatting
			birthdayCheck.Validate();
			if(!birthdayCheck.IsValid()) {
				return false;
			}
			odDatePickerBirthDate.Text=birthdayCheck.Text;
			return true;
		}

		private void CalcAge() {
			if(odDatePickerBirthDate.ReadOnly) {//showing xx/xx/xxxx
				return;
			}
			textAge.Text="";
			if(odDatePickerBirthDate.GetTextDate()=="") {
				return;
			}
			if(!IsBirthdateValid()) {
				MsgBox.Show(this,"Patient's Birthdate is not a valid or allowed date.");
				return;
			}
			DateTime birthdate=PIn.Date(odDatePickerBirthDate.Text);
			odDatePickerBirthDate.SetDateTime(birthdate);//Need to update UI because odDatePickerBirthDate is what is used in OK click to fill column.
			DateTime dateTimeTo=DateTime.Now;
			if(!string.IsNullOrWhiteSpace(dateTimePickerDateDeceased.Text)) { 
				try {
					dateTimeTo=DateTime.Parse(dateTimePickerDateDeceased.Text);
				}
				catch {
					return;
				}
			}
			textAge.Text=PatientLogic.DateToAgeString(birthdate,dateTimeTo);
			SetRequiredFields();
		}

		private void textZip_TextChanged(object sender, System.EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender, System.EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			ZipCode selectedZip=comboZip.GetSelected<ZipCode>();
			if(selectedZip==null) {//if somehow nothing is selected. Can happen if selecting something and draging up outside of the combobox.
				return;
			}
			textCity.Text=selectedZip.City;
			textState.Text=selectedZip.State;
			textZip.Text=selectedZip.ZipCodeDigits;
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
				ZipCode ZipCodeCur=new ZipCode();
				ZipCodeCur.ZipCodeDigits=textZip.Text;
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=ZipCodeCur;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				if(FormZE.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				textCity.Text=ZipCodeCur.City;
				textState.Text=ZipCodeCur.State;
				textZip.Text=ZipCodeCur.ZipCodeDigits;
			}
			else if(listZipCodes.Count==1){
				//only one match found.  Use it.
				textCity.Text=listZipCodes[0].City;
				textState.Text=listZipCodes[0].State;
			}
			else{
				//multiple matches found.  Pick one
				using FormZipSelect FormZS=new FormZipSelect(textZip.Text);
				FormZS.ShowDialog();
				if(FormZS.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
			FillComboZip();
			SetRequiredFields();
		}

		private void checkSuperBilling_MouseDown(object sender,MouseEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Billing)) {
				return;
			}
		}

		private void butEditZip_Click(object sender, System.EventArgs e) {
			if(textZip.Text.Length==0){
				MessageBox.Show(Lan.g(this,"Please enter a zipcode first."));
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0){
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=new ZipCode();
				FormZE.ZipCodeCur.ZipCodeDigits=textZip.Text;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				if(FormZE.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZE.ZipCodeCur.City;
				textState.Text=FormZE.ZipCodeCur.State;
				textZip.Text=FormZE.ZipCodeCur.ZipCodeDigits;
			}
			else{
				using FormZipSelect FormZS=new FormZipSelect(textZip.Text);
				FormZS.ShowDialog();
				if(FormZS.DialogResult!=DialogResult.OK){
					return;
				}
				//Not needed:
				//DataValid.SetInvalid(InvalidTypes.ZipCodes);
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
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

		///<summary>Offers the user, if necessary, the opportunity to send a text message to the patient when changes have been made to texting settings.
		///</summary>
		private void PromptForSmsIfNecessary(Patient patOrig,Patient patNew) {
			if(!Clinics.IsTextingEnabled(patNew.ClinicNum)) {
				return;//Office doesn't use texting.
			}
			if(!ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,patNew.ClinicNum)) {
				return;//Office has turned off this prompt.
			}
			if(patNew.TxtMsgOk!=YN.Yes || string.IsNullOrWhiteSpace(PhoneNumbers.RemoveNonDigitsAndTrimStart(patNew.WirelessPhone))) {
				return;//Not set to YES or no phone number, so no need to send a test message.
			}
			if(!HasPhoneChanged(patOrig.WirelessPhone,patNew.WirelessPhone) && patOrig.TxtMsgOk==YN.Yes) {
				return;//Phone number hasn't changed and TxtMsgOK was already YES => No changes, no need to prompt.
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Texting settings have changed.  Would you like to send a message now?","Send a message?")) {
				string message=PrefC.GetString(PrefName.ShortCodeOptInScript);
				message=FormShortCodeOptIn.FillInTextTemplate(message,patNew);
				FormOpenDental.S_TxtMsg_Click(patNew.PatNum,message);
			}	
		}

		///<summary>Returns true if the given phone number has changed.  Comparison is made with a normalized formatting.</summary>
		private bool HasPhoneChanged(string phoneOriginal,string phoneNew) {
			string phoneOriginalNormalized=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneOriginal);
			string phoneNewNormalized=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneNew);
			return phoneOriginalNormalized!=phoneNewNormalized;
		}

		private void butAuto_Click(object sender, System.EventArgs e) {
			try {
				textChartNumber.Text=Patients.GetNextChartNum();
				_errorProv.SetError(textChartNumber,"");
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
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
			}
			else{
				if(textState.Text.Length==1){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=1;
				}
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
			PatCur.LName=textLName.Text;
			PatCur.FName=textFName.Text;
			PatCur.MiddleI=textMiddleI.Text;
			PatCur.Preferred=textPreferred.Text;
			for(int i=0;i<FamCur.ListPats.Length;i++) {//update the Family object as well
				if(FamCur.ListPats[i].PatNum==PatCur.PatNum) {
					FamCur.ListPats[i]=PatCur.Copy();
					break;
				}
			}
			if(PatCur.ResponsParty==PatCur.PatNum) {
				textResponsParty.Text=PatCur.GetNameLF();
			}
			for(int i=0;i<GuardianList.Count;i++) {
				if(GuardianList[i].PatNumGuardian==PatCur.PatNum) {
					listRelationships.Items.SetValue(i,Patients.GetNameFirst(PatCur.FName,PatCur.Preferred)+" "
						+Guardians.GetGuardianRelationshipStr(GuardianList[i].Relationship));
					//don't break out of loop since it is possible to add multiple relationships with this patient as the PatNumGuardian
					//break;
				}
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
					textCounty.Text=countyOriginal;
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
					textCounty.Text=countyOriginal;
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
			countyOriginal=textCounty.Text;//the original text is preserved when using up and down arrows
			listCounties.Items.Clear();
			CountiesList=Counties.Refresh(textCounty.Text);
			//similarSchools=
				//Carriers.GetSimilarNames(textCounty.Text);
			for(int i=0;i<CountiesList.Length;i++){
				listCounties.Items.Add(CountiesList[i].CountyName);
			}
			int h=13*CountiesList.Length+5;
			if(h > ClientSize.Height-listCounties.Top)
				h=ClientSize.Height-listCounties.Top;
			listCounties.Size=new Size(textCounty.Width,h);
			listCounties.Visible=true;
		}

		private void textCounty_Leave(object sender, System.EventArgs e) {
			if(mouseIsInListCounties){
				return;
			}
			//or if user clicked on a different text box.
			if(listCounties.SelectedIndex!=-1){
				textCounty.Text=CountiesList[listCounties.SelectedIndex].CountyName;
			}
			listCounties.Visible=false;
			SetRequiredFields();
		}

		private void listCounties_Click(object sender, System.EventArgs e){
			textCounty.Text=CountiesList[listCounties.SelectedIndex].CountyName;
			textCounty.Focus();
			textCounty.SelectionStart=textCounty.Text.Length;
			listCounties.Visible=false;
		}

		private void listCounties_MouseEnter(object sender, System.EventArgs e){
			mouseIsInListCounties=true;
		}

		private void listCounties_MouseLeave(object sender, System.EventArgs e){
			mouseIsInListCounties=false;
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
					textSite.Text=SiteOriginal;
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
					textSite.Text=SiteOriginal;
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
			SiteOriginal=textSite.Text;//the original text is preserved when using up and down arrows
			listSites.Items.Clear();
			listSitesFiltered=Sites.GetListFiltered(textSite.Text);
			//similarSchools=
			//Carriers.GetSimilarNames(textSite.Text);
			for(int i=0;i<listSitesFiltered.Count;i++) {
				listSites.Items.Add(listSitesFiltered[i].Description);
			}
			int h=13*listSitesFiltered.Count+5;
			if(h > ClientSize.Height-listSites.Top) {
				h=ClientSize.Height-listSites.Top;
			}
			listSites.Size=new Size(textSite.Width,h);
			listSites.Visible=true;
		}

		private void textSite_Leave(object sender,System.EventArgs e) {
			if(mouseIsInListSites) {
				return;
			}
			//or if user clicked on a different text box.
			if(listSites.SelectedIndex!=-1) {
				textSite.Text=listSitesFiltered[listSites.SelectedIndex].Description;
				PatCur.SiteNum=listSitesFiltered[listSites.SelectedIndex].SiteNum;
			}
			listSites.Visible=false;
			SetRequiredFields();
		}

		private void listSites_Click(object sender,System.EventArgs e) {
			textSite.Text=listSitesFiltered[listSites.SelectedIndex].Description;
			PatCur.SiteNum=listSitesFiltered[listSites.SelectedIndex].SiteNum;
			textSite.Focus();
			textSite.SelectionStart=textSite.Text.Length;
			listSites.Visible=false;
		}

		private void listSites_MouseEnter(object sender,System.EventArgs e) {
			mouseIsInListSites=true;
		}

		private void listSites_MouseLeave(object sender,System.EventArgs e) {
			mouseIsInListSites=false;
		}

		private void butPickSite_Click(object sender,EventArgs e) {
			using FormSites FormS=new FormSites();
			FormS.IsSelectionMode=true;
			FormS.SelectedSiteNum=PatCur.SiteNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			PatCur.SiteNum=FormS.SelectedSiteNum;
			textSite.Text=Sites.GetDescription(PatCur.SiteNum);
			SetRequiredFields();
		}

		private void butPickResponsParty_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			using FormFamilyMemberSelect FormF=new FormFamilyMemberSelect(FamCur);
			FormF.ShowDialog();
			if(FormF.DialogResult!=DialogResult.OK) {
				return;
			}
			PatCur.ResponsParty=FormF.SelectedPatNum;
			//saves a call to the db if this pat's responsible party is self and name in db could be different than local PatCur name
			if(PatCur.PatNum==PatCur.ResponsParty) {
				textResponsParty.Text=PatCur.GetNameLF();
			}
			else {
				textResponsParty.Text=Patients.GetLim(PatCur.ResponsParty).GetNameLF();
			}
			_errorProv.SetError(textResponsParty,"");
		}

		private void butClearResponsParty_Click(object sender,EventArgs e) {
			PatCur.ResponsParty=0;
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
					textEmployer.Text=empOriginal;
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
					textEmployer.Text=empOriginal;
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
			empOriginal=textEmployer.Text;//the original text is preserved when using up and down arrows
			listEmps.Items.Clear();
			List<Employer> similarEmps=Employers.GetSimilarNames(textEmployer.Text);
			for(int i=0;i<similarEmps.Count;i++){
				listEmps.Items.Add(similarEmps[i].EmpName);
			}
			int h=13*similarEmps.Count+5;
			if(h > ClientSize.Height-listEmps.Top){
				h=ClientSize.Height-listEmps.Top;
			}
			listEmps.Size=new Size(231,h);
			listEmps.Visible=true;
		}

		private void textEmployer_Leave(object sender, System.EventArgs e) {
			if(mouseIsInListEmps){
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
			textEmployer.Text=listEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listEmps.Visible=false;
		}

		private void listEmps_MouseEnter(object sender, System.EventArgs e){
			mouseIsInListEmps=true;
		}

		private void listEmps_MouseLeave(object sender, System.EventArgs e){
			mouseIsInListEmps=false;
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
			List<StateAbbr> similarAbbrs=StateAbbrs.GetSimilarAbbrs(textMedicaidState.Text);
			for(int i=0;i<similarAbbrs.Count;i++) {
				listMedicaidStates.Items.Add(similarAbbrs[i].Abbr);
			}
			int h=13*similarAbbrs.Count+5;
			if(h > ClientSize.Height-listMedicaidStates.Top) {
				h=ClientSize.Height-listMedicaidStates.Top;
			}
			listMedicaidStates.Size=new Size(textMedicaidState.Width,h);
			listMedicaidStates.Visible=true;
		}

		private void textMedicaidState_Leave(object sender,System.EventArgs e) {
			if(_mouseIsInListMedicaidStates) {
				return;
			}
			listMedicaidStates.Visible=false;
			SetRequiredFields();
		}

		private void listMedicaidStates_Click(object sender,System.EventArgs e) {
			textMedicaidState.Text=listMedicaidStates.SelectedItem.ToString();
			textMedicaidState.Focus();
			textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
			listMedicaidStates.Visible=false;
		}

		private void listMedicaidStates_MouseEnter(object sender,System.EventArgs e) {
			_mouseIsInListMedicaidStates=true;
		}

		private void listMedicaidStates_MouseLeave(object sender,System.EventArgs e) {
			_mouseIsInListMedicaidStates=false;
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
			List<StateAbbr> similarAbbrs=StateAbbrs.GetSimilarAbbrs(textState.Text);
			for(int i=0;i<similarAbbrs.Count;i++) {
				listStates.Items.Add(similarAbbrs[i].Abbr);
			}
			int h=13*similarAbbrs.Count+5;
			if(h > ClientSize.Height-listStates.Top) {
				h=ClientSize.Height-listStates.Top;
			}
			listStates.Size=new Size(textState.Width,h);
			listStates.Visible=true;
		}

		private void textState_Leave(object sender,System.EventArgs e) {
			if(_mouseIsInListStates) {
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
			_mouseIsInListStates=true;
		}

		private void listStates_MouseLeave(object sender,System.EventArgs e) {
			_mouseIsInListStates=false;
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
			using FormGuardianEdit formG=new FormGuardianEdit(GuardianList[listRelationships.SelectedIndex],FamCur);
			if(formG.ShowDialog()==DialogResult.OK) {
				FillGuardians();
			}
		}

		private void butAddGuardian_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			Guardian guardian=new Guardian();
			guardian.IsNew=true;
			guardian.PatNumChild=PatCur.PatNum;
			//no patnumGuardian set
			using FormGuardianEdit formG=new FormGuardianEdit(guardian,FamCur);
			if(formG.ShowDialog()==DialogResult.OK) {
				_hasGuardiansChanged=true;
				FillGuardians();
			}
		}

		private void butGuardianDefaults_Click(object sender,EventArgs e) {
			if(Guardians.ExistForFamily(PatCur.Guarantor)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Replace existing relationships with default relationships for entire family?")) {
					return;
				}
				//don't delete existing guardians for family until we are certain we can replace them with the defaults
				//Guardians.DeleteForFamily(PatCur.Guarantor);
			}
			List<Patient> listAdults=new List<Patient>();
			List<Patient> listChildren=new List<Patient>();
			PatientPosition pos;
			for(int p=0;p<FamCur.ListPats.Length;p++){
				if(FamCur.ListPats[p].PatNum==PatCur.PatNum) {
					pos=(PatientPosition)listPosition.SelectedIndex;
				}
				else {
					pos=FamCur.ListPats[p].Position;
				}
				if(pos==PatientPosition.Child){
					listChildren.Add(FamCur.ListPats[p]);
				}
				else{
					listAdults.Add(FamCur.ListPats[p]);
				}
			}
			Patient eldestMaleAdult=null;
			Patient eldestFemaleAdult=null;
			for(int i=0;i<listAdults.Count;i++) {
				if(listAdults[i].Gender==PatientGender.Male 
					&& (eldestMaleAdult==null || listAdults[i].Age>eldestMaleAdult.Age)) 
				{
						eldestMaleAdult=listAdults[i];
				}
				if(listAdults[i].Gender==PatientGender.Female
					&& (eldestFemaleAdult==null || listAdults[i].Age>eldestFemaleAdult.Age)) 
				{
					eldestFemaleAdult=listAdults[i];
				}
				//Do not do anything for the other genders.
			}
			if(listAdults.Count<1) {
				MsgBox.Show(this,"No adults found.\r\nFamily relationships will not be changed.");
				return;
			}
			if(listChildren.Count<1) {
				MsgBox.Show(this,"No children found.\r\nFamily relationships will not be changed.");
				return;
			}
			if(eldestFemaleAdult==null && eldestMaleAdult==null) {
				MsgBox.Show(this,"No male or female adults found.\r\nFamily relationships will not be changed.");
				return;
			}
			_hasGuardiansChanged=true;
			if(Guardians.ExistForFamily(PatCur.Guarantor)) {
				//delete all guardians for the family, original family relationships are saved on load so this can be undone if the user presses cancel.
				Guardians.DeleteForFamily(PatCur.Guarantor);
			}
			for(int i=0;i<listChildren.Count;i++) {
				if(eldestFemaleAdult!=null) {
					//Create Parent=>Child relationship
					Guardian motherGuard=new Guardian();
					motherGuard.PatNumChild=eldestFemaleAdult.PatNum;
					motherGuard.PatNumGuardian=listChildren[i].PatNum;
					motherGuard.Relationship=GuardianRelationship.Child;
					Guardians.Insert(motherGuard);
					//Create Child=>Parent relationship
					Guardian childGuard=new Guardian();
					childGuard.PatNumChild=listChildren[i].PatNum;
					childGuard.PatNumGuardian=eldestFemaleAdult.PatNum;
					childGuard.Relationship=GuardianRelationship.Mother;
					childGuard.IsGuardian=true;
					Guardians.Insert(childGuard);
				}
				if(eldestMaleAdult!=null) {
					//Create Parent=>Child relationship
					Guardian fatherGuard=new Guardian();
					fatherGuard.PatNumChild=eldestMaleAdult.PatNum;
					fatherGuard.PatNumGuardian=listChildren[i].PatNum;
					fatherGuard.Relationship=GuardianRelationship.Child;
					Guardians.Insert(fatherGuard);
					//Create Child=>Parent relationship
					Guardian childGuard=new Guardian();
					childGuard.PatNumChild=listChildren[i].PatNum;
					childGuard.PatNumGuardian=eldestMaleAdult.PatNum;
					childGuard.Relationship=GuardianRelationship.Father;
					childGuard.IsGuardian=true;
					Guardians.Insert(childGuard);
				}
			}
			FillGuardians();
		}

		private void butRaceEthnicity_Click(object sender,EventArgs e) {
			using FormPatientRaceEthnicity FormPRE=new FormPatientRaceEthnicity(PatCur,_listPatRaces);
			if(FormPRE.ShowDialog()==DialogResult.OK) {
				_listPatRaces=FormPRE.PatientRaces;
				textRace.Text=PatientRaces.GetRaceDescription(_listPatRaces);
				textEthnicity.Text=PatientRaces.GetEthnicityDescription(_listPatRaces);
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
			if(PatCur.EmployerNum==0){//no employer was previously entered.
				if(textEmployer.Text==""){
					//no change
				}
				else{
					PatCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
			else{//an employer was previously entered
				if(textEmployer.Text==""){
					PatCur.EmployerNum=0;
				}
				//if text has changed
				else if(Employers.GetName(PatCur.EmployerNum)!=textEmployer.Text){
					PatCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
		}

		private void butReferredFrom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachAdd)) {
				return;
			}
			Referral refCur=new Referral();
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?")) {
				using FormPatientSelect FormPS=new FormPatientSelect();
				FormPS.SelectionModeOnly=true;
				FormPS.ShowDialog();
				if(FormPS.DialogResult!=DialogResult.OK) {
					return;
				}
				refCur.PatNum=FormPS.SelectedPatNum;
				bool referralIsNew=true;
				Referral referralMatch=Referrals.GetFirstOrDefault(x => x.PatNum==FormPS.SelectedPatNum);
				if(referralMatch!=null) {
					refCur=referralMatch;
					referralIsNew=false;
				}
				using FormReferralEdit FormRefEdit=new FormReferralEdit(refCur);//the ReferralNum must be added here
				FormRefEdit.IsNew=referralIsNew;
				FormRefEdit.ShowDialog();
				if(FormRefEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				refCur=FormRefEdit.RefCur;//not needed, but it makes it clear that we are editing the ref in FormRefEdit
			}
			else {//not a patient referral, must be a doctor or marketing/other so show the referral select window with doctor and other check boxes checked
				using FormReferralSelect FormRS=new FormReferralSelect();
				FormRS.IsSelectionMode=true;
				FormRS.IsShowPat=false;
				FormRS.ShowDialog();
				if(FormRS.DialogResult!=DialogResult.OK) {
					FillReferrals();//the user may have edited a referral and then cancelled attaching to the patient, refill the text box to reflect any changes
					return;
				}
				refCur=FormRS.SelectedReferral;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=refCur.ReferralNum;
			refattach.PatNum=PatCur.PatNum;
			refattach.RefType=ReferralType.RefFrom;
			refattach.RefDate=DateTime.Today;
			if(refCur.IsDoctor) {//whether using ehr or not
				refattach.IsTransitionOfCare=true;
			}
			refattach.ItemOrder=_listRefAttaches.Select(x => x.ItemOrder).DefaultIfEmpty().Max()+1;
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatCur.PatNum,"Referred From "+Referrals.GetNameFL(refattach.ReferralNum));
			FillReferrals();
		}

		///<summary>Fills the Referred From text box with the oldest (lowest ItemOrder) referral source with ReferralType.RefFrom.</summary>
		private void FillReferrals() {
			textReferredFrom.Clear();
			_listRefAttaches=RefAttaches.Refresh(PatCur.PatNum);
			string firstRefNameTypeAbbr="";
			string firstRefType="";
			string firstRefFullName="";
			RefAttach refAttach=_listRefAttaches.FirstOrDefault(x => x.RefType==ReferralType.RefFrom);
			if(refAttach==null) {
				return;
			}
			Referral refCur=ReferralL.GetReferral(refAttach.ReferralNum);
			if(refCur==null) {
				return;
			}
			firstRefFullName=Referrals.GetNameLF(refCur.ReferralNum);
			if(refCur.PatNum>0) {
				firstRefType=" (patient)";
			}
			else if(refCur.IsDoctor) {
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
			_errorProv.SetError(textReferredFrom,"");
		}

		///<summary>Returns true if any of the checkboxes indicating all family members should be updated with the corresponding data are checked.</summary>
		private bool IsAnySameChecked() {
			return (checkArriveEarlySame.Checked || checkAddressSame.Checked || checkAddressSameForSuperFam.Checked || checkBillProvSame.Checked || checkNotesSame.Checked
				|| checkEmailPhoneSame.Checked);
		}

		private void ShowPatientEditEmail() {
			using FormPatientEditEmail formPatientEditEmail=new FormPatientEditEmail(textEmail.Text);
			formPatientEditEmail.ShowDialog();
			if(formPatientEditEmail.DialogResult==DialogResult.OK) {
				textEmail.Text=formPatientEditEmail.PatientEmails;
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
			using FormReferralsPatient FormRE=new FormReferralsPatient();
			FormRE.PatNum=PatCur.PatNum;
			FormRE.ShowDialog();
			FillReferrals();
			SetRequiredFields();
		}

		private void butViewSSN_Click(object sender,EventArgs e) {
			textSSN.Text=Patients.SSNFormatHelper(PatOld.SSN,false);
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(Permissions.PatientSSNView,PatCur.PatNum,logtext);
		}

		private void butViewBirthdate_Click(object sender,EventArgs e) {
			//Button not visible unless Birthdate is masked
			odDatePickerBirthDate.SetDateTime(PatOld.Birthdate);
			string logtext="Date of birth unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(Permissions.PatientDOBView,PatCur.PatNum,logtext);
			odDatePickerBirthDate.ReadOnly=false;
			odDatePickerBirthDate.Focus();//Force the validation to happen again when losing focus in case our stored birthdate is already invalid.
		}

		//used when adding a date to the date time picker, changes from blank to show a date time.
		private void dateTimePickerDateDeceased_ValueChanged(object sender, EventArgs e) {
			dateTimePickerDateDeceased.CustomFormat="ddddMM/dd/yyyy hh:mm tt";
			dateTimePickerDateDeceased.Format=DateTimePickerFormat.Custom;
		}

		//clear out deceased date time in case it was entered by mistake
		private void butClearDateTimeDeceased_Click(object sender,EventArgs e) {
			dateTimePickerDateDeceased.CustomFormat= " ";
			dateTimePickerDateDeceased.Format=DateTimePickerFormat.Custom;
		}

		private void butOK_Click(object sender ,System.EventArgs e) {
			bool isValid=true;
			object[] parameters=new object[] { isValid,PatCur };
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_beginning",parameters);
			Patient patOld=PatCur.Copy();
			if((bool)parameters[0]==false) {//Didn't pass plug-in validation
				return;
			}
			bool CDSinterventionCheckRequired=false;//checks selected values
			if(odDatePickerBirthDate.GetTextDate()!=""//If not blank
				&& !IsBirthdateValid()//Check for a valid date
				&& !odDatePickerBirthDate.ReadOnly)//If not masked
			{
				MsgBox.Show(this,"Patient's Birthdate is not a valid or allowed date.");
				return;
			}
			if(!odDatePickerDateFirstVisit.IsValid() || !odDatePickerAdmitDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime dateTimeDeceased=DateTime.MinValue;
			try {
				if(dateTimePickerDateDeceased.Text!=" ") {
					dateTimeDeceased=DateTime.Parse(dateTimePickerDateDeceased.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Date time deceased is invalid.");
				return;
			}
			if(textLName.Text==""){
				MsgBox.Show(this,"Last Name must be entered.");
				return;
			}
			//see if chartNum is a duplicate
			if(textChartNumber.Text!=""){
				//the patNum will be 0 for new
				string usedBy=Patients.ChartNumUsedBy(textChartNumber.Text,PatCur.PatNum);
				if(usedBy!=""){
					MessageBox.Show(Lan.g(this,"This chart number is already in use by:")+" "+usedBy);
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
			if(textCounty.Text != "" && !Counties.DoesExist(textCounty.Text)){
				MessageBox.Show(Lan.g(this,"County name invalid. The County entered is not present in the list of Counties. Please add the new County."));
				return;
			}
			if((SexOrientation)comboSexOrientation.SelectedIndex==SexOrientation.AdditionalOrientation
				&& textSpecifySexOrientation.Text.Trim()=="") 
				
			{
				MsgBox.Show(this,"Sexual orientation must be specified.");
				return;
			}
			if((GenderId)comboGenderIdentity.SelectedIndex==GenderId.AdditionalGenderCategory
				&& textSpecifyGender.Text.Trim()=="") 
			{
				MsgBox.Show(this,"Gender identity must be specified.");
				return;
			}
			if(textSite.Text=="") {
				PatCur.SiteNum=0;
			}
			if(textSite.Text != "" && textSite.Text != Sites.GetDescription(PatCur.SiteNum)) {
				long matchingSite=Sites.FindMatchSiteNum(textSite.Text);
				if(matchingSite==-1) {
					MessageBox.Show(Lan.g(this,"Invalid Site description."));
					return;
				}
				else {
					PatCur.SiteNum=matchingSite;
				}
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(comboCanadianEligibilityCode.SelectedIndex==1//FT student
					&& textSchool.Text=="" && PIn.Date(odDatePickerBirthDate.GetDateTime().ToString()).AddYears(18)<=DateTime.Today)
				{
					MsgBox.Show(this,"School should be entered if full-time student and patient is 18 or older.");
					return;
				}
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
			else {
				PatCur.PriProv=comboPriProv.GetSelectedProvNum();
			}
			if(IsNew && PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters) && comboClinic.SelectedClinicNum==0) {
				MsgBox.Show(this,"Current settings for clinics do not allow patients to be added to the 'Unassigned' clinic. Please select a clinic.");
				return;
			}
			//Don't allow changing status from Archived if this is a merged patient.
			if(PatOld.PatStatus!=_listPatStatuses[listStatus.SelectedIndex] && PatOld.PatStatus==PatientStatus.Archived && 
				PatientLinks.WasPatientMerged(PatOld.PatNum)) 
			{
				MsgBox.Show(this,"Not allowed to change the status of a merged patient.");
				return;
			}
			//Check SSN for US Formatting.  If SSN is masked and hasn't been changed, don't check.  Same checks from textSSN_Validating()
			if(CultureInfo.CurrentCulture.Name=="en-US"	&& textSSN.Text!=""//Only validate if US and SSN is not blank.
				&& ((PrefC.GetBool(PrefName.PatientSSNMasked) && textSSN.Text!=_maskedSSNOld)//If SSN masked, only validate if changed
					|| (!PrefC.GetBool(PrefName.PatientSSNMasked) && textSSN.Text!=PatOld.SSN)))//If SSN isn't masked only validate if changed
			{
				if(!Regex.IsMatch(textSSN.Text,@"^\d\d\d-\d\d-\d\d\d\d$")) {
					if(MessageBox.Show("SSN not valid. Continue anyway?","",MessageBoxButtons.OKCancel)
						!=DialogResult.OK) {
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
				SecurityLogs.MakeLogEntry(Permissions.RequiredFields,PatCur.PatNum,"Saved patient with required fields missing.");
			}
			PatCur.LName=textLName.Text;
			PatCur.FName=textFName.Text;
			PatCur.MiddleI=textMiddleI.Text;
			PatCur.Preferred=textPreferred.Text;
			PatCur.Title=textTitle.Text;
			PatCur.Salutation=textSalutation.Text;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Mother's maiden name UI is only used when EHR is enabled.
				_ehrPatientCur.MotherMaidenFname=textMotherMaidenFname.Text;
				_ehrPatientCur.MotherMaidenLname=textMotherMaidenLname.Text;
			}
			PatCur.PatStatus=_listPatStatuses[listStatus.SelectedIndex];//1:1
			switch(PatCur.PatStatus) {
				case PatientStatus.Deceased: 
					if(PatOld.PatStatus!=PatientStatus.Deceased) {
						List<Appointment> listFutureAppts=Appointments.GetFutureSchedApts(PatCur.PatNum);
						if(listFutureAppts.Count>0) {
							string apptDates=string.Join("\r\n",listFutureAppts.Take(10).Select(x => x.AptDateTime.ToString()));
							if(listFutureAppts.Count>10) {
								apptDates+="(...)";
							}
							if(MessageBox.Show(Lan.g(this,"This patient has scheduled appointments in the future")+":\r\n"
									+apptDates+"\r\n"
									+Lan.g(this,"Would you like to delete them and set the patient to Deceased?"),Lan.g(this,"Delete future appointments?"),MessageBoxButtons.YesNo)==DialogResult.Yes) 
							{
								foreach(Appointment appt in listFutureAppts) {
									Appointments.Delete(appt.AptNum,true);
								}
							}
							else {
								PatCur.PatStatus=PatOld.PatStatus;
								return;
							}
						}
					}
					break;
			}
			switch(listGender.SelectedIndex){
				case 0: PatCur.Gender=PatientGender.Male; break;
				case 1: PatCur.Gender=PatientGender.Female; break;
				case 2: PatCur.Gender=PatientGender.Unknown; break;
			}
			switch(listPosition.SelectedIndex){
				case 0: PatCur.Position=PatientPosition.Single; break;
				case 1: PatCur.Position=PatientPosition.Married; break;
				case 2: PatCur.Position=PatientPosition.Child; break;
				case 3: PatCur.Position=PatientPosition.Widowed; break;
				case 4: PatCur.Position=PatientPosition.Divorced; break;
			}
			//Only update birthdate if it was changed, might be masked.
			if(!PrefC.GetBool(PrefName.PatientDOBMasked) || _maskedDOBOld!=odDatePickerBirthDate.GetTextDate()) {
				PatCur.Birthdate=PIn.Date(odDatePickerBirthDate.GetDateTime().ToString());
			}
			PatCur.DateTimeDeceased=dateTimeDeceased;
			if(!PrefC.GetBool(PrefName.PatientSSNMasked) || _maskedSSNOld!=textSSN.Text) {//Only update SSN if it was changed, might be masked.
				if(CultureInfo.CurrentCulture.Name=="en-US") {
					if(Regex.IsMatch(textSSN.Text,@"^\d\d\d-\d\d-\d\d\d\d$")) {
						PatCur.SSN=textSSN.Text.Substring(0,3)+textSSN.Text.Substring(4,2)
							+textSSN.Text.Substring(7,4);
					}
					else {
						PatCur.SSN=textSSN.Text;
					}
				}
				else {//other cultures
					PatCur.SSN=textSSN.Text;
				}
			}
			if(IsNew) {//Check if patient already exists.
				List<Patient> patList=Patients.GetListByName(PatCur.LName,PatCur.FName,PatCur.PatNum);
				for(int i=0;i<patList.Count;i++) {
					//If dates match or aren't entered there might be a duplicate patient.
					if(patList[i].Birthdate==PatCur.Birthdate
					|| patList[i].Birthdate.Year<1880
					|| PatCur.Birthdate.Year<1880) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This patient might already exist.  Continue anyway?")) {
							return;
						}
						break;
					}
				}
			}
			//Send medication history consent to DoseSpot if checkDoseSpotConsent is checked.
			if(!_patCurNote.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx) && checkDoseSpotConsent.Checked) {
				try {
					long clinicNum=Clinics.ClinicNum;
					if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
						clinicNum=PatCur.ClinicNum;
					}
					//Create a temporary patient to set important information to give to DoseSpot in case a patient is needed to be created in DoseSpot.
					Patient patForDoseSpot=PatCur.Copy();
					patForDoseSpot.HmPhone=textHmPhone.Text;
					patForDoseSpot.WkPhone=textWkPhone.Text;
					patForDoseSpot.WirelessPhone=textWirelessPhone.Text;
					patForDoseSpot.TxtMsgOk=(YN)listTextOk.SelectedIndex;
					patForDoseSpot.Email=textEmail.Text;
					patForDoseSpot.Address=textAddress.Text;
					patForDoseSpot.Address2=textAddress2.Text;
					patForDoseSpot.City=textCity.Text;
					patForDoseSpot.State=textState.Text.Trim();
					patForDoseSpot.Country=textCountry.Text;
					patForDoseSpot.Zip=textZip.Text.Trim();
					DoseSpot.SetMedicationHistConsent(patForDoseSpot,clinicNum);
					_patCurNote.Consent=PatConsentFlags.ShareMedicationHistoryErx;
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to set patient medication access consent for DoseSpot: ")+ex.Message);
					return;
				}
			}
			PatCur.MedicaidID=textMedicaidID.Text;
			_ehrPatientCur.MedicaidState=textMedicaidState.Text;
			//Retrieve the value of the Snomed attribute for the selected SexOrientation.
			if(comboSexOrientation.SelectedIndex>=0) {
				SexOrientation sexOrientEnum=(SexOrientation)comboSexOrientation.SelectedIndex;
				_ehrPatientCur.SexualOrientation=EnumTools.GetAttributeOrDefault<EhrAttribute>(sexOrientEnum).Snomed;
				if(sexOrientEnum==SexOrientation.AdditionalOrientation) {
					_ehrPatientCur.SexualOrientationNote=textSpecifySexOrientation.Text;
				}
				else {
					_ehrPatientCur.SexualOrientationNote="";
				}
			}
			//Retrieve the value of the Snomed attribute for the selected GenderId.
			if(comboGenderIdentity.SelectedIndex>=0) {
				GenderId genderIdEnum=(GenderId)comboGenderIdentity.SelectedIndex;
				_ehrPatientCur.GenderIdentity=EnumTools.GetAttributeOrDefault<EhrAttribute>(genderIdEnum).Snomed;
				if(genderIdEnum==GenderId.AdditionalGenderCategory) {
					_ehrPatientCur.GenderIdentityNote=textSpecifyGender.Text;
				}
				else {
					_ehrPatientCur.GenderIdentityNote="";
				}
			}
			EhrPatients.Update(_ehrPatientCur);
			PatCur.WkPhone=textWkPhone.Text;
			PatCur.WirelessPhone=textWirelessPhone.Text;
			PatCur.TxtMsgOk=(YN)listTextOk.SelectedIndex;
			PatCur.Email=textEmail.Text;
			//PatCur.RecallInterval=PIn.PInt(textRecall.Text);
			PatCur.ChartNumber=textChartNumber.Text;
			PatCur.SchoolName=textSchool.Text;
			//address:
			PatCur.HmPhone=textHmPhone.Text;
			PatCur.Address=textAddress.Text;
			PatCur.Address2=textAddress2.Text;
			PatCur.City=textCity.Text;
			PatCur.State=textState.Text.Trim();
			PatCur.Country=textCountry.Text;
			PatCur.Zip=textZip.Text.Trim();
			PatCur.CreditType=textCreditType.Text;
			PatCur.HasSuperBilling=checkSuperBilling.Checked;
			GetEmployerNum();
			//PatCur.EmploymentNote=textEmploymentNote.Text;
			if(comboLanguage.SelectedIndex==0){
				PatCur.Language="";
			}
			else{
				PatCur.Language=languageList[comboLanguage.SelectedIndex-1];
			}
			PatCur.AddrNote=textAddrNotes.Text;
			PatCur.DateFirstVisit=PIn.Date(odDatePickerDateFirstVisit.GetDateTime().ToString());
			PatCur.AskToArriveEarly=PIn.Int(textAskToArriveEarly.Text);
			PatCur.SecProv=comboSecProv.GetSelectedProvNum();
			if(comboFeeSched.SelectedIndex==0){
				PatCur.FeeSched=0;
			}
			else{
				PatCur.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			PatCur.BillingType=comboBillType.GetSelectedDefNum();
			PatCur.ClinicNum=comboClinic.SelectedClinicNum;
			if(!_isUsingNewRaceFeature) {
				_listPatRaces=new List<PatientRace>();
				for(int i=0;i<comboBoxMultiRace.SelectedIndices.Count;i++) {
					int selectedIdx=(int)comboBoxMultiRace.SelectedIndices[i];
					if(selectedIdx==0) {//If the none option was chosen, then ensure that no other race information is saved.
						_listPatRaces.Clear();
						break;
					}
					if(selectedIdx==1) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2054-5"));//AfricanAmerican
					}
					else if(selectedIdx==2) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"1002-5"));//AmericanIndian
					}
					else if(selectedIdx==3) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2028-9"));//Asian
					}
					else if(selectedIdx==4) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));//DeclinedToSpecifyRace
					}
					else if(selectedIdx==5) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2076-8"));//HawaiiOrPacIsland
					}
					else if(selectedIdx==6) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2131-1"));//Other
					}
					else if(selectedIdx==7) {
						_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2106-3"));//White
					}
				}
				if(_listPatRaces.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE)) {
					//If DeclinedToSpecify was chosen, then ensure that no other races are saved.
					_listPatRaces.Clear();
					_listPatRaces.Add(new PatientRace(PatCur.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));
				}
				else if(_listPatRaces.Any(x => x.CdcrecCode=="2131-1")) {//If Other was chosen, then ensure that no other races are saved.
					_listPatRaces.Clear();
					_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2131-1"));
				}
				//In order to pass EHR G2 MU testing you must be able to have an ethnicity without a race, or a race without an ethnicity.  This will mean that patients will not count towards
				//meaningful use demographic calculations.  If we have time in the future we should probably alert EHR users when a race is chosen but no ethnicity, or a ethnicity but no race.
				if(comboEthnicity.SelectedIndex==1) {
					_listPatRaces.Add(new PatientRace(PatCur.PatNum,PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE));
				}
				else if(comboEthnicity.SelectedIndex==2) {
					_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2186-5"));//NotHispanic
				}
				else if(comboEthnicity.SelectedIndex==3) {
					_listPatRaces.Add(new PatientRace(PatCur.PatNum,"2135-2"));//Hispanic
				}
			}
			PatientRaces.Reconcile(PatCur.PatNum,_listPatRaces);//Insert, Update, Delete if needed.
			PatCur.County=textCounty.Text;
			//site set when user picks from list.
			PatCur.GradeLevel=(PatientGrade)comboGradeLevel.SelectedIndex;
			PatCur.Urgency=(TreatmentUrgency)comboUrgency.SelectedIndex;
			//ResponsParty handled when buttons are pushed.
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)) {
				PatCur.TrophyFolder=textTrophyFolder.Text;
			}
			PatCur.Ward=textWard.Text;
			PatCur.PreferContactMethod=(ContactMethod)comboContact.SelectedIndex;
			PatCur.PreferConfirmMethod=(ContactMethod)comboConfirm.SelectedIndex;
			PatCur.PreferRecallMethod=(ContactMethod)comboRecall.SelectedIndex;
			if(ListTools.In(0,comboExcludeECR.SelectedIndices)) { //If "None" is selected
				_commOptOut.OptOutSms=0;
				_commOptOut.OptOutEmail=0;
			}
			else {
				List<CommOptOutMode> listModesOptOut=comboExcludeECR.GetListSelected<CommOptOutMode>();
				//In the future, we could enhance this UI to allow granularity of selecting certain CommOptOutTypes and not others.
				_commOptOut.OptOutSms=listModesOptOut.Any(x => x==CommOptOutMode.Text) ? CommOptOutType.All : 0;
				_commOptOut.OptOutEmail=listModesOptOut.Any(x => x==CommOptOutMode.Email) ? CommOptOutType.All : 0;
			}
			CommOptOuts.Upsert(_commOptOut);
			PatCur.AdmitDate=PIn.Date(odDatePickerAdmitDate.GetDateTime().ToString());
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				PatCur.CanadianEligibilityCode=(byte)comboCanadianEligibilityCode.SelectedIndex;
			}
			if(PatCur.Guarantor==0){
				PatCur.Guarantor=PatCur.PatNum;
			}
			_patCurNote.ICEName=textIceName.Text;
			_patCurNote.ICEPhone=textIcePhone.Text;
			PatCur.HasSignedTil=checkBoxSignedTil.Checked;//Update Signed Truth in Lending State
			Patients.Update(PatCur,PatOld);
			PatientNotes.Update(_patCurNote,PatCur.Guarantor);
			string strPatPriProvDesc=Providers.GetLongDesc(PatCur.PriProv);
			Patients.InsertPrimaryProviderChangeSecurityLogEntry(PatOld,PatCur);
			if(checkRestrictSched.Checked) {
				PatRestrictions.Upsert(PatCur.PatNum,PatRestrict.ApptSchedule);//will only insert if one does not already exist in the db.
			}
			else {
				PatRestrictions.RemovePatRestriction(PatCur.PatNum,PatRestrict.ApptSchedule);
			}
			if(PatCur.Birthdate!=PatOld.Birthdate || PatCur.Gender!=PatOld.Gender) {
				CDSinterventionCheckRequired=true;
			}
			if(CDSinterventionCheckRequired && CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSI=EhrTriggers.TriggerMatch(PatCur,PatCur);//both should be patCur.
				FormCDSI.ShowIfRequired(false);
			}
			#region 'Same' Checkboxes
			bool isAuthArchivedEdit=Security.IsAuthorized(Permissions.ArchivedPatientEdit,true);
			if(checkArriveEarlySame.Checked){
				Patients.UpdateArriveEarlyForFam(PatCur,isAuthArchivedEdit);
			}
			//Only family checked.
			if(checkAddressSame.Checked && !checkAddressSameForSuperFam.Checked){
				//might want to include a mechanism for comparing fields to be overwritten
				Patients.UpdateAddressForFam(PatCur,false,isAuthArchivedEdit);
			}
			//SuperFamily is checked, family could be checked or unchecked.
			else if(checkAddressSameForSuperFam.Checked) {
				Patients.UpdateAddressForFam(PatCur,true,isAuthArchivedEdit);
			}
			if(checkBillProvSame.Checked) {
				List<Patient> listPatsForPriProvEdit=FamCur.ListPats.ToList().FindAll(x => x.PatNum!=PatCur.PatNum && x.PriProv!=PatCur.PriProv);
				if(!isAuthArchivedEdit) {//Remove Archived patients if not allowed to edit so we don't create a log for them.
					listPatsForPriProvEdit.RemoveAll(x => x.PatStatus==PatientStatus.Archived);
				}
				//true if any family member has a different PriProv and the user is authorized for PriProvEdit
				bool isChangePriProvs=(listPatsForPriProvEdit.Count>0 && Security.IsAuthorized(Permissions.PatPriProvEdit,DateTime.MinValue,true,true));
				Patients.UpdateBillingProviderForFam(PatCur,isChangePriProvs,isAuthArchivedEdit);//if user is not authorized this will not update PriProvs for fam
			}
			if(checkNotesSame.Checked){
				Patients.UpdateNotesForFam(PatCur,isAuthArchivedEdit);
			}
			if(checkEmailPhoneSame.Checked) {
				Patients.UpdateEmailPhoneForFam(PatCur,isAuthArchivedEdit);
			}
			#endregion 'Same' Checkboxes
			if(PatCur.BillingType!=PatOld.BillingType) {
				AutomationL.Trigger(AutomationTrigger.SetBillingType,null,PatCur.PatNum);
				Patients.InsertBillTypeChangeSecurityLogEntry(PatOld,PatCur);
			}
			//If this patient is also a referral source,
			//keep address info synched:
			Referral referral=Referrals.GetFirstOrDefault(x => x.PatNum==PatCur.PatNum);
			if(referral!=null) {
				referral.LName=PatCur.LName;
				referral.FName=PatCur.FName;
				referral.MName=PatCur.MiddleI;
				referral.Address=PatCur.Address;
				referral.Address2=PatCur.Address2;
				referral.City=PatCur.City;
				referral.ST=PatCur.State;
				referral.SSN=PatCur.SSN;
				referral.Zip=PatCur.Zip;
				referral.Telephone=TelephoneNumbers.FormatNumbersExactTen(PatCur.HmPhone);
				referral.EMail=PatCur.Email;
				Referrals.Update(referral);
				Referrals.RefreshCache();
			}
			//if patient is inactive, deceased, etc., then disable any recalls
			Patients.UpdateRecalls(PatCur,PatOld,"Edit Patient Window");
			//If there is an existing HL7 def enabled, send an ADT message if there is an outbound ADT message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//new patients get the A04 ADT, updating existing patients we send an A08
				MessageHL7 messageHL7=null;
				if(IsNew) {
					messageHL7=MessageConstructor.GenerateADT(PatCur,Patients.GetPat(PatCur.Guarantor),EventTypeHL7.A04);
				}
				else {
					messageHL7=MessageConstructor.GenerateADT(PatCur,Patients.GetPat(PatCur.Guarantor),EventTypeHL7.A08);
				}
				//Will be null if there is no outbound ADT message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=PatCur.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show(this,messageHL7.ToString());
					}
				}
			}
			long defNum=comboSpecialty.GetSelectedDefNum();
			if(_defLinkPatCur!=null) {
				if(defNum==0) {
					DefLinks.Delete(_defLinkPatCur.DefLinkNum);
				}
				else {
					_defLinkPatCur.DefNum=defNum;
					DefLinks.Update(_defLinkPatCur);
				}
			}
			else if(defNum!=0){//if the patient does not have a specialty and "Unspecified" is not selected. 
				DefLink defLinkNew=new DefLink();
				defLinkNew.DefNum=defNum;
				defLinkNew.FKey=PatCur.PatNum;
				defLinkNew.LinkType=DefLinkType.Patient;
				DefLinks.Insert(defLinkNew);
			}
			//The specialty could have changed so invalidate the cached specialty on the currently selected patient object in PatientL.cs
			PatientL.InvalidateSelectedPatSpecialty();
			if(!IsNew) {
				Patients.InsertAddressChangeSecurityLogEntry(PatOld,PatCur);
				PatientEvent.Fire(ODEventType.Patient,PatCur);
			}
			PromptForSmsIfNecessary(patOld,PatCur);
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_end",PatCur,PatOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatientEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(IsNew){
				//for(int i=0;i<RefList.Length;i++){
				//	RefAttaches.Delete(RefList[i]);
				//}
				Patients.Delete(PatCur);
				SecurityLogs.MakeLogEntry(Permissions.PatientEdit,PatCur.PatNum,Lan.g(this,"Canceled creating new patient. Deleting patient record."));
			}
			if(_hasGuardiansChanged) {  //If guardian information was changed, and user canceled.
				//revert any changes to the guardian list for all family members
				Guardians.RevertChanges(_listGuardiansForFamOld,FamCur.ListPats.Select(x => x.PatNum).ToList());
			}
		}
    }
}









