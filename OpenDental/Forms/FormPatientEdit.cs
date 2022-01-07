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
		private County[] countiesArray;
		private string _empOriginal;//used in the emp dropdown logic
		private bool isMouseInListEmps;
		///<summary>This is the object that is altered in this form.</summary>
		private Patient _patientCur;
		///<summary>This is the object that is altered in this form, since it is an extension of the patient table.</summary>
		private PatientNote _patientNoteCur;
		//private RefAttach[] RefList;
		private Family _familyCur;
		private Patient _patientOld;
		///<summary>Will include the languages setup in the settings, and also the language of this patient if that language is not on the selection list.</summary>
		private List<string> _listStringLanguages;
		private List<Guardian> _listGuardians;
		///<summary>If the user presses cancel, use this list to revert any changes to the guardians for all family members.</summary>
		private List<Guardian> _listGuardiansForFamOld;
		///<summary>Local cache of RefAttaches for the current patient.  Set in FillReferrals().</summary>
		private List<RefAttach> _listRefAttaches;
		private System.Windows.Forms.ListBox listBoxMedicaidStates;//displayed from within code, not designer
		private List<RequiredField> _listRequiredFields;
		private string _medicaidStateOriginal;//used in the medicaidState dropdown logic
		private bool _mouseIsInListMedicaidStates;
		private System.Windows.Forms.ListBox listBoxStates;//displayed from within code, not designer
		private string _stateOriginal;//used in the medicaidState dropdown logic
		private bool _isMouseInListStates;
		private bool _isMissingRequiredFields;
		private bool _isLoad;//To keep track if ListBoxes' selected index is changed by the user
		private ErrorProvider _errorProvider=new ErrorProvider();
		private bool _isValidating=false;
		private EhrPatient _ehrPatientCur;
		private List<PatientRace> _listPatientRaces;
		private ComboBoxOD comboBoxMultiRace;
		private bool _hasGuardiansChanged=false;
		///<summary>Because adding the new feature where patients can choose their race from hundreds of options would cause us to need to recertify EHR, 
		///we committed all the code for the new feature while keeping the old behavior for EHR users. When we are ready to switch to the new feature, 
		///all we need to do is set this boolean to true (hopefully).</summary>
		private bool _isUsingNewRaceFeature=!PrefC.GetBool(PrefName.ShowFeatureEhr);
		private DefLink _defLinkPatientCur;
		private CommOptOut _commOptOut;
		///<summary>List of PatientStatuses shown in listStatus, must be 1:1 with listStatus.
		///Deleted is excluded, unless PatCur is flagged as deleted.
		///Needed due to index differences when deleted is not present.</summary>
		private List<PatientStatus> _listPatientStatuses=new List<PatientStatus> ();
		///<summary>Used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.</summary>
		private string _maskedSSNOld="";
		///<summary>Used to keep track of what masked DOB was shown when the form was loaded, and stop us from storing masked DOBs on accident.</summary>
		private string _maskedDOBOld="";
		///<summary>Used to display the available billing types. Will contain hidden billing types.</summary>
		private List<Def> _listBillingTypes;

		///<summary></summary>
		public FormPatientEdit(Patient patientCur,Family familyCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patientCur=patientCur;
			_patientNoteCur=PatientNotes.Refresh(patientCur.PatNum,patientCur.Guarantor);
			_familyCur=familyCur;
			_patientOld=patientCur.Copy();
			listEmps=new ListBox();
			listEmps.Location=new Point(LayoutManager.Scale(textEmployer.Left),LayoutManager.Scale(textEmployer.Bottom));
			listEmps.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listEmps.Visible=false;
			listEmps.Click += new System.EventHandler(listEmps_Click);
			listEmps.DoubleClick += new System.EventHandler(listEmps_DoubleClick);
			listEmps.MouseEnter += new System.EventHandler(listEmps_MouseEnter);
			listEmps.MouseLeave += new System.EventHandler(listEmps_MouseLeave);
			LayoutManager.Add(listEmps,this);
			listEmps.BringToFront();
			listCounties=new ListBox();
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
			listSites=new ListBox();
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
			listBoxMedicaidStates=new ListBox();
			listBoxMedicaidStates.Location=new Point(LayoutManager.Scale(textMedicaidState.Left),LayoutManager.Scale(textMedicaidState.Bottom));
			listBoxMedicaidStates.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listBoxMedicaidStates.Visible=false;
			listBoxMedicaidStates.Click += new System.EventHandler(listMedicaidStates_Click);
			listBoxMedicaidStates.MouseEnter += new System.EventHandler(listMedicaidStates_MouseEnter);
			listBoxMedicaidStates.MouseLeave += new System.EventHandler(listMedicaidStates_MouseLeave);
			LayoutManager.Add(listBoxMedicaidStates,this);
			listBoxMedicaidStates.BringToFront();
			listBoxStates=new ListBox();
			listBoxStates.Location=new Point(LayoutManager.Scale(textState.Left+groupBox1.Left),LayoutManager.Scale(textState.Bottom+groupBox1.Top));
			listBoxStates.Font=new Font(this.Font.FontFamily,LayoutManager.ScaleF(this.Font.Size));
			listBoxStates.Visible=false;
			listBoxStates.Click += new System.EventHandler(listStates_Click);
			listBoxStates.MouseEnter += new System.EventHandler(listStates_MouseEnter);
			listBoxStates.MouseLeave += new System.EventHandler(listStates_MouseLeave);
			LayoutManager.Add(listBoxStates,this);
			listBoxStates.BringToFront();
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
			List<Patient> listPatientsFamily=_familyCur.ListPats.ToList();
			if(!isAuthArchivedEdit) {
				//Exclude Archived pats if user does not have permission.  If user doesn't have the permission and all non-Archived patients pass the
				//the conditions, but there is an Archived patient who does not, the check will still be true.  The Archived will not be updated on OK.
				listPatientsFamily=listPatientsFamily.Where(x => x.PatStatus!=PatientStatus.Archived).ToList();
			}
			#region SameForFamily
			//for the comparison logic to work, any nulls must be converted to ""
			if(_patientCur.HmPhone is null) _patientCur.HmPhone="";
			if(_patientCur.Address is null) _patientCur.Address="";
			if(_patientCur.Address2 is null) _patientCur.Address2="";
			if(_patientCur.City is null) _patientCur.City="";
			if(_patientCur.State is null) _patientCur.State="";
			if(_patientCur.Zip is null) _patientCur.Zip="";
			if(_patientCur.Country is null) _patientCur.Country="";
			if(_patientCur.CreditType is null) _patientCur.CreditType="";
			if(_patientCur.AddrNote is null) _patientCur.AddrNote="";
			if(_patientCur.WirelessPhone is null) _patientCur.WirelessPhone="";
			if(_patientCur.WkPhone is null) _patientCur.WkPhone="";
			if(_patientCur.Email is null) _patientCur.Email="";
			for(int i=0;i<listPatientsFamily.Count;i++){
				if(_patientCur.HmPhone!=listPatientsFamily[i].HmPhone
					|| _patientCur.Address!=listPatientsFamily[i].Address
					|| _patientCur.Address2!=listPatientsFamily[i].Address2
					|| _patientCur.City!=listPatientsFamily[i].City
					|| _patientCur.State!=listPatientsFamily[i].State
					|| _patientCur.Zip!=listPatientsFamily[i].Zip
					|| _patientCur.Country!=listPatientsFamily[i].Country)
				{
					checkAddressSame.Checked=false;
				}
				if(_patientCur.CreditType!=listPatientsFamily[i].CreditType
					|| _patientCur.BillingType!=listPatientsFamily[i].BillingType
					|| _patientCur.PriProv!=listPatientsFamily[i].PriProv
					|| _patientCur.SecProv!=listPatientsFamily[i].SecProv
					|| _patientCur.FeeSched!=listPatientsFamily[i].FeeSched)
				{
					checkBillProvSame.Checked=false;
				}
				if(_patientCur.AddrNote!=listPatientsFamily[i].AddrNote){
					checkNotesSame.Checked=false;
				}
				if(_patientCur.WirelessPhone!=listPatientsFamily[i].WirelessPhone
					|| _patientCur.WkPhone!=listPatientsFamily[i].WkPhone
					|| _patientCur.Email!=listPatientsFamily[i].Email) 
				{
					checkEmailPhoneSame.Checked=false;
				}
				if(_patientCur.AskToArriveEarly!=listPatientsFamily[i].AskToArriveEarly) {
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
				&& _patientCur.SuperFamily!=0
				&& _patientCur.PatNum==_patientCur.SuperFamily) //Has to be the Super Head.
			{
				checkAddressSameForSuperFam.Visible=true;
				//Check all superfam members for any with differing information
				checkAddressSameForSuperFam.Checked=Patients.SuperFamHasSameAddrPhone(_patientCur,isAuthArchivedEdit);
			}
			#endregion SameForFamily
			checkRestrictSched.Checked=PatRestrictionL.IsRestricted(_patientCur.PatNum,PatRestrict.ApptSchedule,true);
			textPatNum.Text=_patientCur.PatNum.ToString();
			textLName.Text=_patientCur.LName;
			textFName.Text=_patientCur.FName;
			textMiddleI.Text=_patientCur.MiddleI;
			textPreferred.Text=_patientCur.Preferred;
			textTitle.Text=_patientCur.Title;
			textSalutation.Text=_patientCur.Salutation;
			textIceName.Text=_patientNoteCur.ICEName;
			textIcePhone.Text=_patientNoteCur.ICEPhone;
			_ehrPatientCur=EhrPatients.Refresh(_patientCur.PatNum);
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
			Program programErx=Programs.GetCur(ProgramName.eRx);
			ErxOption erxOption=PIn.Enum<ErxOption>(ProgramProperties.GetPropForProgByDesc(programErx.ProgramNum,Erx.PropertyDescs.ErxOption).PropertyValue);
			if(programErx.Enabled && (erxOption==ErxOption.DoseSpot || erxOption==ErxOption.DoseSpotWithLegacy)) {
				checkDoseSpotConsent.Visible=true;
				checkDoseSpotConsent.Checked=_patientNoteCur.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx);
				if(checkDoseSpotConsent.Checked) {//once checked, should never be able to be unchecked.
					checkDoseSpotConsent.Enabled=false;
				}
			}
			foreach(PatientStatus patientStatus in Enum.GetValues(typeof(PatientStatus))) {
				if(patientStatus==PatientStatus.Deleted && _patientCur.PatStatus!=PatientStatus.Deleted) {
					continue;//Only display 'Deleted' if PatCur is 'Deleted'.  Shouldn't happen, but has been observed.
				}
				_listPatientStatuses.Add(patientStatus);
				listStatus.Items.Add(Lan.g("enumPatientStatus",patientStatus.ToString()));//Not using .GetDescription() because of prior behavior.
			}
			listGender.Items.Add(Lan.g("enumPatientGender","Male"));
			listGender.Items.Add(Lan.g("enumPatientGender","Female"));
			listGender.Items.Add(Lan.g("enumPatientGender","Unknown"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Single"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Married"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Child"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Widowed"));
			listPosition.Items.Add(Lan.g("enumPatientPosition","Divorced"));
			listStatus.SetSelected(_listPatientStatuses.IndexOf(_patientCur.PatStatus),true);//using _listPatStatuses because it is 1:1 with listStatus.
			switch (_patientCur.Gender){
				case PatientGender.Male : listGender.SelectedIndex=0; break;
				case PatientGender.Female : listGender.SelectedIndex=1; break;
				case PatientGender.Unknown : listGender.SelectedIndex=2; break;}
			switch (_patientCur.Position){
				case PatientPosition.Single : listPosition.SelectedIndex=0; break;
				case PatientPosition.Married : listPosition.SelectedIndex=1; break;
				case PatientPosition.Child : listPosition.SelectedIndex=2; break;
				case PatientPosition.Widowed : listPosition.SelectedIndex=3; break;
				case PatientPosition.Divorced : listPosition.SelectedIndex=4; break;}
			FillGuardians();
			_listGuardiansForFamOld=_familyCur.ListPats.SelectMany(x => Guardians.Refresh(x.PatNum)).ToList();
			if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
				//turn off validation until the user changes text, or unmasks.
				odDatePickerBirthDate.SetMaskedDate(Patients.DOBFormatHelper(_patientCur.Birthdate,true));//If birthdate is not set this will return ""
				_maskedDOBOld=odDatePickerBirthDate.GetTextDate();
				if(odDatePickerBirthDate.GetTextDate()!="") {
					odDatePickerBirthDate.ReadOnly=true;
				}
				butViewBirthdate.Enabled=(Security.IsAuthorized(Permissions.PatientDOBView,true) && odDatePickerBirthDate.GetTextDate()!="");//Disable if no DOB
				butViewBirthdate.Visible=true;
			}
			else {
				odDatePickerBirthDate.SetDateTime(_patientCur.Birthdate);
				//butViewBirthdate is disabled and not visible from designer.
				//Move age back over since butViewBirthdate is not showing
				LayoutManager.MoveLocation(label20,new Point(label20.Location.X-72,label20.Location.Y));
				LayoutManager.MoveLocation(textAge,new Point(textAge.Location.X-72,textAge.Location.Y));
			}
			if(_patientCur.DateTimeDeceased.Year > 1880) {
				dateTimePickerDateDeceased.Value=_patientCur.DateTimeDeceased;
			}
            else {
				//if there is no datetime deceased, used to show blank instead of the default of todays date.
				dateTimePickerDateDeceased.CustomFormat=" ";
				dateTimePickerDateDeceased.Format=DateTimePickerFormat.Custom;
            }
			textAge.Text=PatientLogic.DateToAgeString(_patientCur.Birthdate,_patientCur.DateTimeDeceased);
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				textSSN.Text=Patients.SSNFormatHelper(_patientCur.SSN,true);//If PatCur.SSN is null or empty, returns empty string.
				_maskedSSNOld=textSSN.Text;
				butViewSSN.Enabled=(Security.IsAuthorized(Permissions.PatientSSNView,true) && textSSN.Text!="");//Disable button if no SSN entered
				butViewSSN.Visible=true;
			}
			else {
				textSSN.Text=Patients.SSNFormatHelper(_patientCur.SSN,false);
				//butViewSSN is disabled and not visible from designer.
			}
			textMedicaidID.Text=_patientCur.MedicaidID;
			textMedicaidState.Text=_ehrPatientCur.MedicaidState;
			textAddress.Text=_patientCur.Address;
			textAddress2.Text=_patientCur.Address2;
			textCity.Text=_patientCur.City;
			textState.Text=_patientCur.State;
			textCountry.Text=_patientCur.Country;
			textZip.Text=_patientCur.Zip;
			textHmPhone.Text=_patientCur.HmPhone;
			textWkPhone.Text=_patientCur.WkPhone;
			textWirelessPhone.Text=_patientCur.WirelessPhone;
			listTextOk.SelectedIndex=(int)_patientCur.TxtMsgOk;
			FillShortCodes(_patientCur.TxtMsgOk);
			textEmail.Text=_patientCur.Email;
			textCreditType.Text=_patientCur.CreditType;
			odDatePickerDateFirstVisit.SetDateTime(_patientCur.DateFirstVisit);
			if(_patientCur.AskToArriveEarly>0){
				textAskToArriveEarly.Text=_patientCur.AskToArriveEarly.ToString();
			}
			textChartNumber.Text=_patientCur.ChartNumber;
			textEmployer.Text=Employers.GetName(_patientCur.EmployerNum);
			//textEmploymentNote.Text=PatCur.EmploymentNote;
			_listStringLanguages=new List<string>();
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)!="") {
				string[] stringArrayLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',');
				for(int i=0;i<stringArrayLanguages.Length;i++) {
					if(stringArrayLanguages[i]=="") {
						continue;
					}
					_listStringLanguages.Add(stringArrayLanguages[i]);
				}
			}
			if(_patientCur.Language!="" && _patientCur.Language!=null && !_listStringLanguages.Contains(_patientCur.Language)) {
				_listStringLanguages.Add(_patientCur.Language);
			}
			comboLanguage.Items.Add(Lan.g(this,"None"));//regardless of how many languages are listed, the first item is "none"
			comboLanguage.SelectedIndex=0;
			for(int i=0;i<_listStringLanguages.Count;i++) {
				if(_listStringLanguages[i]=="") {
					continue;
				}
				CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listStringLanguages[i]);
				if(cultureInfo==null) {//custom language
					comboLanguage.Items.Add(_listStringLanguages[i]);
				}
				else {
					comboLanguage.Items.Add(cultureInfo.DisplayName);
				}
				if(_patientCur.Language==_listStringLanguages[i]) {
					comboLanguage.SelectedIndex=i+1;
				}
			}
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.Add(Lan.g(this,"None"),new FeeSched());
			comboFeeSched.SelectedIndex=0;
			List<FeeSched> listFeeScheds=FeeScheds.GetDeepCopy(false);
			foreach(FeeSched feeSched in listFeeScheds) {
				if(feeSched.IsHidden && feeSched.FeeSchedNum!=_patientCur.FeeSched) {
					continue;//skip hidden fee schedules as long as not assigned to this patient. This will only occur in rare occurrences.
				}
				comboFeeSched.Items.Add(feeSched.Description+(feeSched.IsHidden ? " "+Lans.g(this,"(Hidden)") : ""),feeSched);
				if(feeSched.FeeSchedNum==_patientCur.FeeSched) {
					comboFeeSched.SelectedIndex=comboFeeSched.Items.Count-1;
				}
			}
			_listBillingTypes=Defs.GetDefsForCategory(DefCat.BillingTypes);
			comboBillType.Items.AddDefs(_listBillingTypes.FindAll(x => !x.IsHidden || x.DefNum==_patientCur.BillingType));
			//If a new patient is being created from the select patient window, the BillingType will be the selected clinic's default billing type.
			//If a new patient is being made with the Add button of the Family Module, the BillingType will be the selected patient's billing type.
			//This is determined in the methods that initialize _patientCur and pre-insert it before this form is instantiated.
			comboBillType.SetSelectedDefNum(_patientCur.BillingType);
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
			if(_patientCur.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(Permissions.ArchivedPatientEdit,false)) {
				butReferredFrom.Enabled=false;
				textReferredFrom.Enabled=false;
				butOK.Enabled=false;
			}
			comboClinic.SelectedClinicNum=_patientCur.ClinicNum;
			FillCombosProv();
			comboPriProv.SetSelectedProvNum(_patientCur.PriProv);
			comboSecProv.SetSelectedProvNum(_patientCur.SecProv);
			if(!Security.IsAuthorized(Permissions.PatPriProvEdit,DateTime.MinValue,true,true) && _patientCur.PriProv>0) {
				//user not allowed to change existing prov.  Warning messages are suppressed here.
				string strToolTip=Lan.g("Security","Not authorized for")+" "+GroupPermissions.GetDesc(Permissions.PatPriProvEdit);
				_priProvEditToolTip.SetToolTip(butPickPrimary,strToolTip);
				_priProvEditToolTip.SetToolTip(comboPriProv,strToolTip);
				comboPriProv.Enabled=false;
			}
			switch(_patientCur.StudentStatus){
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
			textSchool.Text=_patientCur.SchoolName;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSchoolName.Text=Lan.g(this,"Name of School");
				comboCanadianEligibilityCode.Items.Add("0 - Please Choose");
				comboCanadianEligibilityCode.Items.Add("1 - Full-time student");
				comboCanadianEligibilityCode.Items.Add("2 - Disabled");
				comboCanadianEligibilityCode.Items.Add("3 - Disabled student");
				comboCanadianEligibilityCode.Items.Add("4 - Code not applicable");
				comboCanadianEligibilityCode.SelectedIndex=_patientCur.CanadianEligibilityCode;
			}
			textAddrNotes.Text=_patientCur.AddrNote;
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
				_listPatientRaces=PatientRaces.GetForPatient(_patientCur.PatNum);
				textRace.Text=PatientRaces.GetRaceDescription(_listPatientRaces);
				textEthnicity.Text=PatientRaces.GetEthnicityDescription(_listPatientRaces);
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
				List<PatientRace> listPatientRaces=PatientRaces.GetForPatient(_patientCur.PatNum);
				comboEthnicity.SelectedIndex=0;//none
				foreach(PatientRace patientRace in listPatientRaces) {
					switch(patientRace.CdcrecCode) {
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
							if(patientRace.IsEthnicity) {
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
			textCounty.Text=_patientCur.County;
			textSite.Text=Sites.GetDescription(_patientCur.SiteNum);
			string[] stringArrayEnumPatientGrade=Enum.GetNames(typeof(PatientGrade));
			for(int i=0;i<stringArrayEnumPatientGrade.Length;i++){
				comboGradeLevel.Items.Add(Lan.g("enumGrade",stringArrayEnumPatientGrade[i]));
			}
			comboGradeLevel.SelectedIndex=(int)_patientCur.GradeLevel;
			string[] stringArrayEnumTreatmentUrgency=Enum.GetNames(typeof(TreatmentUrgency));
			for(int i=0;i<stringArrayEnumTreatmentUrgency.Length;i++){
				comboUrgency.Items.Add(Lan.g("enumUrg",stringArrayEnumTreatmentUrgency[i]));
			}
			comboUrgency.SelectedIndex=(int)_patientCur.Urgency;
			if(_patientCur.ResponsParty!=0){
				textResponsParty.Text=Patients.GetLim(_patientCur.ResponsParty).GetNameLF();
			}
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)){
				textTrophyFolder.Text=_patientCur.TrophyFolder;
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
			textWard.Text=_patientCur.Ward;
			for(int i=0;i<Enum.GetNames(typeof(ContactMethod)).Length;i++){
				comboContact.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
				comboConfirm.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
				comboRecall.Items.Add(Lan.g("enumContactMethod",Enum.GetNames(typeof(ContactMethod))[i]));
			}
			comboContact.SelectedIndex=(int)_patientCur.PreferContactMethod;
			comboConfirm.SelectedIndex=(int)_patientCur.PreferConfirmMethod;
			comboRecall.SelectedIndex=(int)_patientCur.PreferRecallMethod;
			_commOptOut=CommOptOuts.Refresh(_patientCur.PatNum);
			int idx=1; //Starts at 1 to account for "None"
			comboExcludeECR.Items.Add("None");
			foreach(CommOptOutMode commOptOutMode in Enum.GetValues(typeof(CommOptOutMode))) {
				comboExcludeECR.Items.Add(commOptOutMode.GetDescription(),commOptOutMode);
				if(_commOptOut.IsOptedOut(commOptOutMode,CommOptOutType.All)) {
					comboExcludeECR.SetSelected(idx,true);
				}
				idx++;
			}
			if(comboExcludeECR.SelectedIndices.Count==0) {
				comboExcludeECR.SetSelected(0);
			}
			if(_patientCur.AdmitDate.Year>1880){
				odDatePickerAdmitDate.SetDateTime(_patientCur.AdmitDate);
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
			_errorProvider.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)
				&& _patientCur.SuperFamily!=0 
				&& _patientCur.Guarantor==_patientCur.PatNum) 
			{
				//If the patient is a guarantor in a superfamily then enable the checkbox to opt into superfamily billing.
				checkSuperBilling.Visible=true;
				checkSuperBilling.Checked=_patientCur.HasSuperBilling;
			}
			//Loop through the SexOrientation enum and display the Description attribute. If the Snomed attribute equals the patient's SexualOrientation, 
			//set that item as the selected index.
			foreach(SexOrientation sexOrientation in (SexOrientation[])Enum.GetValues(typeof(SexOrientation))) {
				comboSexOrientation.Items.Add(sexOrientation.GetDescription());
				if(_ehrPatientCur.SexualOrientation==EnumTools.GetAttributeOrDefault<EhrAttribute>(sexOrientation).Snomed) {
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
			checkBoxSignedTil.Checked=_patientCur.HasSignedTil;
			Plugins.HookAddCode(this,"FormPatientEdit.Load_end",_patientCur);
		}

		private void butShortCodeOptIn_Click(object sender,EventArgs e) {
			using FormShortCodeOptIn formShortCodeOptIn=new FormShortCodeOptIn(_patientCur);
			if(formShortCodeOptIn.ShowDialog()==DialogResult.OK) {
				FillShortCodes((YN)listTextOk.SelectedIndex);
			}
		}

		private void FillShortCodes(YN txtMsgOk) {
			Patient patient=_patientCur.Copy();
			patient.TxtMsgOk=txtMsgOk;
			PatComm patComm=Patients.GetPatComms(ListTools.FromSingle(patient)).FirstOrDefault();
			bool isShortCodeInfoNeeded=patComm?.IsPatientShortCodeEligible(patient.ClinicNum)??false;
			butShortCodeOptIn.Visible=isShortCodeInfoNeeded;
			labelApptTexts.Visible=isShortCodeInfoNeeded;
			listBoxApptTexts.Visible=isShortCodeInfoNeeded;
			if(isShortCodeInfoNeeded) {
				string optIn;
				switch(patient.ShortCodeOptIn) {
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
			_defLinkPatientCur=DefLinks.GetOneByFKey(_patientCur.PatNum,DefLinkType.Patient);
			if(_defLinkPatientCur==null) {
				comboSpecialty.SetSelectedDefNum(0);
			}
			else{
				comboSpecialty.SetSelectedDefNum(_defLinkPatientCur.DefNum);
			}
		}

		private void butPickPrimary_Click(object sender,EventArgs e) {
			if(_patientCur.PriProv>0 && !Security.IsAuthorized(Permissions.PatPriProvEdit)) {
				return;
			}
			using FormProviderPick formProviderPick = new FormProviderPick(comboPriProv.Items.GetAll<Provider>());
			formProviderPick.SelectedProvNum=comboPriProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboPriProv.SetSelectedProvNum(formProviderPick.SelectedProvNum);
		}

		private void butPickSecondary_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboSecProv.Items.GetAll<Provider>());
			formProviderPick.SelectedProvNum=comboSecProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboSecProv.SetSelectedProvNum(formProviderPick.SelectedProvNum);
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
				&& _familyCur.ListPats.Any(x => x.PatNum!=_patientCur.PatNum && x.PriProv!=comboPriProv.GetSelectedProvNum()) //a family member has a different PriProv
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
			foreach(ZipCode zipCode in listZipCodes) {
				comboZip.Items.Add(zipCode.ZipCodeDigits+" ("+zipCode.City+")",zipCode);
			}
		}

		private void FillGuardians(){
			_listGuardians=Guardians.Refresh(_patientCur.PatNum);
			listRelationships.Items.Clear();
			for(int i=0;i<_listGuardians.Count;i++){
				listRelationships.Items.Add(_familyCur.GetNameInFamFirst(_listGuardians[i].PatNumGuardian)+" "
					+Guardians.GetGuardianRelationshipStr(_listGuardians[i].Relationship));
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
							if(listTextOk.SelectedIndex>-1 && listTextOk.Items[listTextOk.SelectedIndex].ToString()==Lan.g(this,"??")) {
								_isMissingRequiredFields=true;
								if(_isValidating) {
									_errorProvider.SetError(listTextOk,Lan.g(this,"Selection cannot be '??'."));
								}
							}
							else {
								_errorProvider.SetError(listTextOk,"");
							}
						}
						else {
							_errorProvider.SetError(listTextOk,"");
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
						if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
							areConditionsMet=true;
							break;
						}
						areConditionsMet=CheckDateConditions(odDatePickerAdmitDate.GetDateTime().ToString(),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.BillingType:
						//Conditions of type BillingType store the DefNum as the ConditionValue.
						areConditionsMet=ConditionComparerHelper(comboBillType.GetSelectedDefNum().ToString(),i,listRequiredFieldConditions);
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
						DateTime dateTimeBirthdate=PIn.Date(odDatePickerBirthDate.GetDateTime().ToString());
						int ageEntered=DateTime.Today.Year-dateTimeBirthdate.Year;
						if(dateTimeBirthdate>DateTime.Today.AddYears(-ageEntered)) {
							ageEntered--;
						}
						List<RequiredFieldCondition> listRequiredFieldConditionsAge=listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Birthdate);
						//There should be no more than 2 conditions of type Birthdate
						List<bool> listAreCondsMet=new List<bool>();
						for(int j=0;j<listRequiredFieldConditionsAge.Count;j++) {
							listAreCondsMet.Add(CondOpComparer(ageEntered,listRequiredFieldConditionsAge[j].Operator,PIn.Int(listRequiredFieldConditionsAge[j].ConditionValue)));
						}
						if(listAreCondsMet.Count<2 || listRequiredFieldConditionsAge[1].ConditionRelationship==LogicalOperator.And) {
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
						areConditionsMet=ConditionComparerHelper(comboClinic.SelectedClinicNum.ToString(),i,listRequiredFieldConditions);//includes none clinic
						break;								
					case RequiredFieldName.DateTimeDeceased:
						if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							areConditionsMet=true;
							break;
						}
						areConditionsMet=CheckDateConditions(dateTimePickerDateDeceased.Text,i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Gender:
						areConditionsMet=ConditionComparerHelper(listGender.Items.GetTextShowingAt(listGender.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Language:
						areConditionsMet=ConditionComparerHelper(comboLanguage.Items[comboLanguage.SelectedIndex].ToString(),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.MedicaidID:
						if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
							areConditionsMet=true;
							break;
						}
						//The only possible value for ConditionValue is 'Blank'
						if((listRequiredFieldConditions[i].Operator==ConditionOperator.Equals && textMedicaidID.Text=="")
							|| (listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals && textMedicaidID.Text!="")) {
							areConditionsMet=true;
						}
						break;
					case RequiredFieldName.MedicaidState:
						if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
							areConditionsMet=true;
							break;
						}
						//The only possible value for ConditionValue is '' (an empty string)
						if((listRequiredFieldConditions[i].Operator==ConditionOperator.Equals && textMedicaidState.Text=="")
							|| (listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals && textMedicaidState.Text!="")) {
							areConditionsMet=true;
						}
						break;
					case RequiredFieldName.PatientStatus:
						areConditionsMet=ConditionComparerHelper(listStatus.Items.GetTextShowingAt(listStatus.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.Position:
						areConditionsMet=ConditionComparerHelper(listPosition.Items.GetTextShowingAt(listPosition.SelectedIndex),i,listRequiredFieldConditions);
						break;
					case RequiredFieldName.PrimaryProvider:
						//Conditions of type PrimaryProvider store the ProvNum as the ConditionValue.
						areConditionsMet=ConditionComparerHelper(comboPriProv.GetSelectedProvNum().ToString(),i,listRequiredFieldConditions);
						break;							
					case RequiredFieldName.StudentStatus:
						areConditionsMet=CheckStudentStatusConditions(i,listRequiredFieldConditions);
						break;
				}
				previousFieldName=(int)listRequiredFieldConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		///<summary>Returns true if the operator is Equals and the value is in the list of conditions or if the operator is NotEquals and the value is 
		///not in the list of conditions.</summary>
		private bool ConditionComparerHelper(string val,int condCurIndex,List<RequiredFieldCondition> listRequiredFieldConditions) {
			RequiredFieldCondition requiredFieldConditionCur = listRequiredFieldConditions[condCurIndex];//Variable for convenience
			switch(requiredFieldConditionCur.Operator) {
				case ConditionOperator.Equals:
					return listRequiredFieldConditions.Any(x => x.ConditionType==requiredFieldConditionCur.ConditionType && x.ConditionValue==val);
				case ConditionOperator.NotEquals:
					return !listRequiredFieldConditions.Any(x => x.ConditionType==requiredFieldConditionCur.ConditionType && x.ConditionValue==val);
				default:
					return false;
			}
		}

		///<summary>Returns true if the conditions for this date condition are true.</summary>
		private bool CheckDateConditions(string dateStr,int condCurIndex,List<RequiredFieldCondition> listRequiredFieldConditions) {
			DateTime dateTimeCur=DateTime.MinValue;
			if(dateStr=="" || !DateTime.TryParse(dateStr,out dateTimeCur)) {
				return false;
			}
			List<RequiredFieldCondition> listRequiredFieldConditionDate=listRequiredFieldConditions.FindAll(x => x.ConditionType==listRequiredFieldConditions[condCurIndex].ConditionType);
			if(listRequiredFieldConditionDate.Count<1) {
				return false;
			}
			//There should be no more than 2 conditions of a date type
			List<bool> listAreCondsMet=new List<bool>();
			for(int i=0;i<listRequiredFieldConditionDate.Count;i++) {
				listAreCondsMet.Add(CondOpComparer(dateTimeCur,listRequiredFieldConditionDate[i].Operator,PIn.Date(listRequiredFieldConditionDate[i].ConditionValue)));
			}
			if(listAreCondsMet.Count<2 || listRequiredFieldConditionDate[1].ConditionRelationship==LogicalOperator.And) {
				return !listAreCondsMet.Contains(false);
			}
			return listAreCondsMet.Contains(true);
		}

		///<summary>Returns true if the conditions for StudentStatus are true.</summary>
		private bool CheckStudentStatusConditions(int condCurIndex,List<RequiredFieldCondition> listRequiredFieldConditions) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) { //Canadian. en-CA or fr-CA
				return true;
			}
			if(listRequiredFieldConditions[condCurIndex].Operator==ConditionOperator.Equals) {
				if((radioStudentN.Checked && listRequiredFieldConditions[condCurIndex].ConditionValue==Lan.g(this,"Nonstudent"))
					|| (radioStudentF.Checked && listRequiredFieldConditions[condCurIndex].ConditionValue==Lan.g(this,"Fulltime"))
					|| (radioStudentP.Checked && listRequiredFieldConditions[condCurIndex].ConditionValue==Lan.g(this,"Parttime")))
				{
					return true;
				}
				return false;
			}
			else { //condCur.Operator==ConditionOperator.NotEquals
				List<RequiredFieldCondition> listRequiredFieldConditionsStudent=listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.StudentStatus);
				if((radioStudentN.Checked && listRequiredFieldConditionsStudent.Any(x => x.ConditionValue==Lan.g(this,"Nonstudent")))
					|| (radioStudentF.Checked && listRequiredFieldConditionsStudent.Any(x => x.ConditionValue==Lan.g(this,"Fulltime")))
					|| (radioStudentP.Checked && listRequiredFieldConditionsStudent.Any(x => x.ConditionValue==Lan.g(this,"Parttime"))))
				{
					return false;
				}
				return true;
			}
		}

		///<summary>Evaluates two dates using the provided operator.</summary>
		private bool CondOpComparer(DateTime dateTime1,ConditionOperator conditionOperator,DateTime dateTime2) {
			return CondOpComparer(DateTime.Compare(dateTime1,dateTime2),conditionOperator,0);
		}

		///<summary>Evaluates two integers using the provided operator.</summary>
		private bool CondOpComparer(int value1,ConditionOperator conditionOperator,int value2) {
			switch(conditionOperator) {
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
				_errorProvider.SetError(textMedicaidID,Lan.g(this,"Medicaid ID length must be ")+reqLength.ToString()+Lan.g(this," digits for the state of ")
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
		private void SetRequiredComboBox(Label labelCur,ComboBox comboBoxCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			SetRequiredControl(labelCur,comboBoxCur,areConditionsMet,disallowedIdx,new List<int>(),errorMsg);
		}

		private void SetRequiredComboBoxPlus(Label labelCur,ComboBoxOD comboBoxCur,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			SetRequiredControl(labelCur,comboBoxCur,areConditionsMet,disallowedIdx,new List<int>(),errorMsg);
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If a disallowedIndices is also selected, sets the error provider.</summary>
		private void SetRequiredComboBoxPlus(Label labelCur,ComboBoxOD comboBoxCur,bool areConditionsMet,List<int> listDisallowedIndices,string errorMsg) {
			SetRequiredControl(labelCur,comboBoxCur,areConditionsMet,-1,listDisallowedIndices,errorMsg);
		}	

		private void SetRequiredControl(Label labelCur,Control control,bool areConditionsMet,int disallowedIdx,List<int> listDisallowedIndices,
			string errorMsg) 
		{
			if(areConditionsMet) {
				labelCur.Text=labelCur.Text.Replace("*","")+"*";
				if((control is ComboBox && ((ComboBox)control).SelectedIndex==disallowedIdx)
					|| (control is ComboBoxOD && ((ComboBoxOD)control).SelectionModeMulti && ((ComboBoxOD)control).SelectedIndices.Exists(x => listDisallowedIndices.Exists(y => y==x)))
					|| (control is ComboBoxOD && !((ComboBoxOD)control).SelectionModeMulti && ((ComboBoxOD)control).SelectedIndex==disallowedIdx)
					|| (control is TextBox && ((TextBox)control).Text=="")) 
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
				labelCur.Text=labelCur.Text.Replace("*","")+"";
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
		
		private void ListBox_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(_isLoad) {
				return;
			}
			SetRequiredFields();
			Plugins.HookAddCode(this,"FormPatientEdit.ListBox_SelectedIndexChanged_end",_patientCur);
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
			_patientCur.StudentStatus="N";
			SetRequiredFields();
		}

		private void radioStudentF_Click(object sender, System.EventArgs e) {
			_patientCur.StudentStatus="F";
			SetRequiredFields();
		}

		private void radioStudentP_Click(object sender, System.EventArgs e) {
			_patientCur.StudentStatus="P";
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
			ValidDate textBirthdayCheck=new ValidDate();
			textBirthdayCheck.Text=odDatePickerBirthDate.GetTextDate();//Use text date for slash validation and formatting
			textBirthdayCheck.Validate();
			if(!textBirthdayCheck.IsValid()) {
				return false;
			}
			odDatePickerBirthDate.Text=textBirthdayCheck.Text;
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
			DateTime dateTimeBirthdate=PIn.Date(odDatePickerBirthDate.Text);
			odDatePickerBirthDate.SetDateTime(dateTimeBirthdate);//Need to update UI because odDatePickerBirthDate is what is used in OK click to fill column.
			DateTime dateTimeTo=DateTime.Now;
			if(!string.IsNullOrWhiteSpace(dateTimePickerDateDeceased.Text)) { 
				try {
					dateTimeTo=DateTime.Parse(dateTimePickerDateDeceased.Text);
				}
				catch {
					return;
				}
			}
			textAge.Text=PatientLogic.DateToAgeString(dateTimeBirthdate,dateTimeTo);
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
				ZipCode ZipCodeCur=new ZipCode();
				ZipCodeCur.ZipCodeDigits=textZip.Text;
				using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
				formZipCodeEdit.ZipCodeCur=ZipCodeCur;
				formZipCodeEdit.IsNew=true;
				formZipCodeEdit.ShowDialog();
				if(formZipCodeEdit.DialogResult!=DialogResult.OK){
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
				using FormZipSelect formZipSelect=new FormZipSelect(textZip.Text);
				formZipSelect.ShowDialog();
				if(formZipSelect.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=formZipSelect.ZipSelected.City;
				textState.Text=formZipSelect.ZipSelected.State;
				textZip.Text=formZipSelect.ZipSelected.ZipCodeDigits;
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
				using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
				formZipCodeEdit.ZipCodeCur=new ZipCode();
				formZipCodeEdit.ZipCodeCur.ZipCodeDigits=textZip.Text;
				formZipCodeEdit.IsNew=true;
				formZipCodeEdit.ShowDialog();
				if(formZipCodeEdit.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=formZipCodeEdit.ZipCodeCur.City;
				textState.Text=formZipCodeEdit.ZipCodeCur.State;
				textZip.Text=formZipCodeEdit.ZipCodeCur.ZipCodeDigits;
			}
			else{
				using FormZipSelect formZipSelect=new FormZipSelect(textZip.Text);
				formZipSelect.ShowDialog();
				if(formZipSelect.DialogResult!=DialogResult.OK){
					return;
				}
				//Not needed:
				//DataValid.SetInvalid(InvalidTypes.ZipCodes);
				textCity.Text=formZipSelect.ZipSelected.City;
				textState.Text=formZipSelect.ZipSelected.State;
				textZip.Text=formZipSelect.ZipSelected.ZipCodeDigits;
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
		private void PromptForSmsIfNecessary(Patient patientOriginal,Patient patientNew) {
			if(!Clinics.IsTextingEnabled(patientNew.ClinicNum)) {
				return;//Office doesn't use texting.
			}
			if(!ClinicPrefs.GetBool(PrefName.ShortCodeOptInOnApptComplete,patientNew.ClinicNum)) {
				return;//Office has turned off this prompt.
			}
			if(patientNew.TxtMsgOk!=YN.Yes || string.IsNullOrWhiteSpace(PhoneNumbers.RemoveNonDigitsAndTrimStart(patientNew.WirelessPhone))) {
				return;//Not set to YES or no phone number, so no need to send a test message.
			}
			if(!HasPhoneChanged(patientOriginal.WirelessPhone,patientNew.WirelessPhone) && patientOriginal.TxtMsgOk==YN.Yes) {
				return;//Phone number hasn't changed and TxtMsgOK was already YES => No changes, no need to prompt.
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Texting settings have changed.  Would you like to send a message now?","Send a message?")) {
				string message=PrefC.GetString(PrefName.ShortCodeOptInScript);
				message=FormShortCodeOptIn.FillInTextTemplate(message,patientNew);
				FormOpenDental.S_TxtMsg_Click(patientNew.PatNum,message);
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
				_errorProvider.SetError(textChartNumber,"");
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
			_patientCur.LName=textLName.Text;
			_patientCur.FName=textFName.Text;
			_patientCur.MiddleI=textMiddleI.Text;
			_patientCur.Preferred=textPreferred.Text;
			for(int i=0;i<_familyCur.ListPats.Length;i++) {//update the Family object as well
				if(_familyCur.ListPats[i].PatNum==_patientCur.PatNum) {
					_familyCur.ListPats[i]=_patientCur.Copy();
					break;
				}
			}
			if(_patientCur.ResponsParty==_patientCur.PatNum) {
				textResponsParty.Text=_patientCur.GetNameLF();
			}
			for(int i=0;i<_listGuardians.Count;i++) {
				if(_listGuardians[i].PatNumGuardian==_patientCur.PatNum) {
					listRelationships.Items.SetValue(i,Patients.GetNameFirst(_patientCur.FName,_patientCur.Preferred)+" "
						+Guardians.GetGuardianRelationshipStr(_listGuardians[i].Relationship));
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
			countiesArray=Counties.Refresh(textCounty.Text);
			//similarSchools=
				//Carriers.GetSimilarNames(textCounty.Text);
			for(int i=0;i<countiesArray.Length;i++){
				listCounties.Items.Add(countiesArray[i].CountyName);
			}
			int h=13*countiesArray.Length+5;
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
				textCounty.Text=countiesArray[listCounties.SelectedIndex].CountyName;
			}
			listCounties.Visible=false;
			SetRequiredFields();
		}

		private void listCounties_Click(object sender, System.EventArgs e){
			textCounty.Text=countiesArray[listCounties.SelectedIndex].CountyName;
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
				_patientCur.SiteNum=_listSitesFiltered[listSites.SelectedIndex].SiteNum;
			}
			listSites.Visible=false;
			SetRequiredFields();
		}

		private void listSites_Click(object sender,System.EventArgs e) {
			textSite.Text=_listSitesFiltered[listSites.SelectedIndex].Description;
			_patientCur.SiteNum=_listSitesFiltered[listSites.SelectedIndex].SiteNum;
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

		private void butPickSite_Click(object sender,EventArgs e) {
			using FormSites FormS=new FormSites();
			FormS.IsSelectionMode=true;
			FormS.SelectedSiteNum=_patientCur.SiteNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			_patientCur.SiteNum=FormS.SelectedSiteNum;
			textSite.Text=Sites.GetDescription(_patientCur.SiteNum);
			SetRequiredFields();
		}

		private void butPickResponsParty_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			using FormFamilyMemberSelect formFamilyMemberSelect=new FormFamilyMemberSelect(_familyCur);
			formFamilyMemberSelect.ShowDialog();
			if(formFamilyMemberSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_patientCur.ResponsParty=formFamilyMemberSelect.SelectedPatNum;
			//saves a call to the db if this pat's responsible party is self and name in db could be different than local PatCur name
			if(_patientCur.PatNum==_patientCur.ResponsParty) {
				textResponsParty.Text=_patientCur.GetNameLF();
			}
			else {
				textResponsParty.Text=Patients.GetLim(_patientCur.ResponsParty).GetNameLF();
			}
			_errorProvider.SetError(textResponsParty,"");
		}

		private void butClearResponsParty_Click(object sender,EventArgs e) {
			_patientCur.ResponsParty=0;
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
				listBoxMedicaidStates.Visible=false;
				return;
			}
			if(textMedicaidState.Text=="") {
				listBoxMedicaidStates.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listBoxMedicaidStates.Items.Count==0) {
					return;
				}
				if(listBoxMedicaidStates.SelectedIndex==-1) {
					listBoxMedicaidStates.SelectedIndex=0;
					textMedicaidState.Text=listBoxMedicaidStates.SelectedItem.ToString();
				}
				else if(listBoxMedicaidStates.SelectedIndex==listBoxMedicaidStates.Items.Count-1) {
					listBoxMedicaidStates.SelectedIndex=-1;
					textMedicaidState.Text=_medicaidStateOriginal;
				}
				else {
					listBoxMedicaidStates.SelectedIndex++;
					textMedicaidState.Text=listBoxMedicaidStates.SelectedItem.ToString();
				}
				textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listBoxMedicaidStates.Items.Count==0) {
					return;
				}
				if(listBoxMedicaidStates.SelectedIndex==-1) {
					listBoxMedicaidStates.SelectedIndex=listBoxMedicaidStates.Items.Count-1;
					textMedicaidState.Text=listBoxMedicaidStates.SelectedItem.ToString();
				}
				else if(listBoxMedicaidStates.SelectedIndex==0) {
					listBoxMedicaidStates.SelectedIndex=-1;
					textMedicaidState.Text=_medicaidStateOriginal;
				}
				else {
					listBoxMedicaidStates.SelectedIndex--;
					textMedicaidState.Text=listBoxMedicaidStates.SelectedItem.ToString();
				}
				textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
				return;
			}
			if(textMedicaidState.Text.Length==1) {
				textMedicaidState.Text=textMedicaidState.Text.ToUpper();
				textMedicaidState.SelectionStart=1;
			}
			_medicaidStateOriginal=textMedicaidState.Text;//the original text is preserved when using up and down arrows
			listBoxMedicaidStates.Items.Clear();
			List<StateAbbr> listStateAbbrsSimilar=StateAbbrs.GetSimilarAbbrs(textMedicaidState.Text);
			for(int i=0;i<listStateAbbrsSimilar.Count;i++) {
				listBoxMedicaidStates.Items.Add(listStateAbbrsSimilar[i].Abbr);
			}
			int h=13*listStateAbbrsSimilar.Count+5;
			if(h > ClientSize.Height-listBoxMedicaidStates.Top) {
				h=ClientSize.Height-listBoxMedicaidStates.Top;
			}
			LayoutManager.MoveSize(listBoxMedicaidStates,new Size(textMedicaidState.Width,LayoutManager.Scale(h)));
			listBoxMedicaidStates.Visible=true;
		}

		private void textMedicaidState_Leave(object sender,System.EventArgs e) {
			if(_mouseIsInListMedicaidStates) {
				return;
			}
			listBoxMedicaidStates.Visible=false;
			SetRequiredFields();
		}

		private void listMedicaidStates_Click(object sender,System.EventArgs e) {
			textMedicaidState.Text=listBoxMedicaidStates.SelectedItem.ToString();
			textMedicaidState.Focus();
			textMedicaidState.SelectionStart=textMedicaidState.Text.Length;
			listBoxMedicaidStates.Visible=false;
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
				listBoxStates.Visible=false;
				return;
			}
			if(textState.Text=="") {
				listBoxStates.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listBoxStates.Items.Count==0) {
					return;
				}
				if(listBoxStates.SelectedIndex==-1) {
					listBoxStates.SelectedIndex=0;
					textState.Text=listBoxStates.SelectedItem.ToString();
				}
				else if(listBoxStates.SelectedIndex==listBoxStates.Items.Count-1) {
					listBoxStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listBoxStates.SelectedIndex++;
					textState.Text=listBoxStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listBoxStates.Items.Count==0) {
					return;
				}
				if(listBoxStates.SelectedIndex==-1) {
					listBoxStates.SelectedIndex=listBoxStates.Items.Count-1;
					textState.Text=listBoxStates.SelectedItem.ToString();
				}
				else if(listBoxStates.SelectedIndex==0) {
					listBoxStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listBoxStates.SelectedIndex--;
					textState.Text=listBoxStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(textState.Text.Length==1) {
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=1;
			}
			_stateOriginal=textState.Text;//the original text is preserved when using up and down arrows
			listBoxStates.Items.Clear();
			List<StateAbbr> listStateAbbrsSimilar=StateAbbrs.GetSimilarAbbrs(textState.Text);
			for(int i=0;i<listStateAbbrsSimilar.Count;i++) {
				listBoxStates.Items.Add(listStateAbbrsSimilar[i].Abbr);
			}
			int h=13*listStateAbbrsSimilar.Count+5;
			if(h > ClientSize.Height-listBoxStates.Top) {
				h=ClientSize.Height-listBoxStates.Top;
			}
			LayoutManager.MoveSize(listBoxStates,new Size(textState.Width,LayoutManager.Scale(h)));
			listBoxStates.Visible=true;
		}

		private void textState_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListStates) {
				return;
			}
			listBoxStates.Visible=false;
			SetRequiredFields();
		}

		private void listStates_Click(object sender,System.EventArgs e) {
			if(listBoxStates.SelectedItem==null) {
				return;
			}
			textState.Text=listBoxStates.SelectedItem.ToString();
			textState.Focus();
			textState.SelectionStart=textState.Text.Length;
			listBoxStates.Visible=false;
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
			using FormGuardianEdit formGuardianEdit=new FormGuardianEdit(_listGuardians[listRelationships.SelectedIndex],_familyCur);
			if(formGuardianEdit.ShowDialog()==DialogResult.OK) {
				FillGuardians();
			}
		}

		private void butAddGuardian_Click(object sender,EventArgs e) {
			UpdateLocalNameHelper();
			Guardian guardian=new Guardian();
			guardian.IsNew=true;
			guardian.PatNumChild=_patientCur.PatNum;
			//no patnumGuardian set
			using FormGuardianEdit formG=new FormGuardianEdit(guardian,_familyCur);
			if(formG.ShowDialog()==DialogResult.OK) {
				_hasGuardiansChanged=true;
				FillGuardians();
			}
		}

		private void butGuardianDefaults_Click(object sender,EventArgs e) {
			if(Guardians.ExistForFamily(_patientCur.Guarantor)) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Replace existing relationships with default relationships for entire family?")) {
					return;
				}
				//don't delete existing guardians for family until we are certain we can replace them with the defaults
				//Guardians.DeleteForFamily(PatCur.Guarantor);
			}
			List<Patient> listPatientsAdults=new List<Patient>();
			List<Patient> listPatientsChildren=new List<Patient>();
			PatientPosition patientPosition;
			for(int p=0;p<_familyCur.ListPats.Length;p++){
				if(_familyCur.ListPats[p].PatNum==_patientCur.PatNum) {
					patientPosition=(PatientPosition)listPosition.SelectedIndex;
				}
				else {
					patientPosition=_familyCur.ListPats[p].Position;
				}
				if(patientPosition==PatientPosition.Child){
					listPatientsChildren.Add(_familyCur.ListPats[p]);
				}
				else{
					listPatientsAdults.Add(_familyCur.ListPats[p]);
				}
			}
			Patient patientEldestMaleAdult=null;
			Patient patientEldestFemaleAdult=null;
			for(int i=0;i<listPatientsAdults.Count;i++) {
				if(listPatientsAdults[i].Gender==PatientGender.Male 
					&& (patientEldestMaleAdult==null || listPatientsAdults[i].Age>patientEldestMaleAdult.Age)) 
				{
						patientEldestMaleAdult=listPatientsAdults[i];
				}
				if(listPatientsAdults[i].Gender==PatientGender.Female
					&& (patientEldestFemaleAdult==null || listPatientsAdults[i].Age>patientEldestFemaleAdult.Age)) 
				{
					patientEldestFemaleAdult=listPatientsAdults[i];
				}
				//Do not do anything for the other genders.
			}
			if(listPatientsAdults.Count<1) {
				MsgBox.Show(this,"No adults found.\r\nFamily relationships will not be changed.");
				return;
			}
			if(listPatientsChildren.Count<1) {
				MsgBox.Show(this,"No children found.\r\nFamily relationships will not be changed.");
				return;
			}
			if(patientEldestFemaleAdult==null && patientEldestMaleAdult==null) {
				MsgBox.Show(this,"No male or female adults found.\r\nFamily relationships will not be changed.");
				return;
			}
			_hasGuardiansChanged=true;
			if(Guardians.ExistForFamily(_patientCur.Guarantor)) {
				//delete all guardians for the family, original family relationships are saved on load so this can be undone if the user presses cancel.
				Guardians.DeleteForFamily(_patientCur.Guarantor);
			}
			for(int i=0;i<listPatientsChildren.Count;i++) {
				if(patientEldestFemaleAdult!=null) {
					//Create Parent=>Child relationship
					Guardian guardianMother=new Guardian();
					guardianMother.PatNumChild=patientEldestFemaleAdult.PatNum;
					guardianMother.PatNumGuardian=listPatientsChildren[i].PatNum;
					guardianMother.Relationship=GuardianRelationship.Child;
					Guardians.Insert(guardianMother);
					//Create Child=>Parent relationship
					Guardian guardianChild=new Guardian();
					guardianChild.PatNumChild=listPatientsChildren[i].PatNum;
					guardianChild.PatNumGuardian=patientEldestFemaleAdult.PatNum;
					guardianChild.Relationship=GuardianRelationship.Mother;
					guardianChild.IsGuardian=true;
					Guardians.Insert(guardianChild);
				}
				if(patientEldestMaleAdult!=null) {
					//Create Parent=>Child relationship
					Guardian guardianFather=new Guardian();
					guardianFather.PatNumChild=patientEldestMaleAdult.PatNum;
					guardianFather.PatNumGuardian=listPatientsChildren[i].PatNum;
					guardianFather.Relationship=GuardianRelationship.Child;
					Guardians.Insert(guardianFather);
					//Create Child=>Parent relationship
					Guardian guardianChild=new Guardian();
					guardianChild.PatNumChild=listPatientsChildren[i].PatNum;
					guardianChild.PatNumGuardian=patientEldestMaleAdult.PatNum;
					guardianChild.Relationship=GuardianRelationship.Father;
					guardianChild.IsGuardian=true;
					Guardians.Insert(guardianChild);
				}
			}
			FillGuardians();
		}

		private void butRaceEthnicity_Click(object sender,EventArgs e) {
			using FormPatientRaceEthnicity formPatientRaceEthnicity=new FormPatientRaceEthnicity(_patientCur,_listPatientRaces);
			if(formPatientRaceEthnicity.ShowDialog()==DialogResult.OK) {
				_listPatientRaces=formPatientRaceEthnicity.PatientRaces;
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
			if(_patientCur.EmployerNum==0){//no employer was previously entered.
				if(textEmployer.Text==""){
					//no change
				}
				else{
					_patientCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
			else{//an employer was previously entered
				if(textEmployer.Text==""){
					_patientCur.EmployerNum=0;
				}
				//if text has changed
				else if(Employers.GetName(_patientCur.EmployerNum)!=textEmployer.Text){
					_patientCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
		}

		private void butReferredFrom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachAdd)) {
				return;
			}
			Referral referralCur=new Referral();
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?")) {
				using FormPatientSelect formPatientSelect=new FormPatientSelect();
				formPatientSelect.SelectionModeOnly=true;
				formPatientSelect.ShowDialog();
				if(formPatientSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				referralCur.PatNum=formPatientSelect.SelectedPatNum;
				bool isReferralNew=true;
				Referral referralMatch=Referrals.GetFirstOrDefault(x => x.PatNum==formPatientSelect.SelectedPatNum);
				if(referralMatch!=null) {
					referralCur=referralMatch;
					isReferralNew=false;
				}
				using FormReferralEdit formReferralEdit=new FormReferralEdit(referralCur);//the ReferralNum must be added here
				formReferralEdit.IsNew=isReferralNew;
				formReferralEdit.ShowDialog();
				if(formReferralEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				referralCur=formReferralEdit.RefCur;//not needed, but it makes it clear that we are editing the ref in FormRefEdit
			}
			else {//not a patient referral, must be a doctor or marketing/other so show the referral select window with doctor and other check boxes checked
				using FormReferralSelect formReferralSelect=new FormReferralSelect();
				formReferralSelect.IsSelectionMode=true;
				formReferralSelect.IsShowPat=false;
				formReferralSelect.ShowDialog();
				if(formReferralSelect.DialogResult!=DialogResult.OK) {
					FillReferrals();//the user may have edited a referral and then cancelled attaching to the patient, refill the text box to reflect any changes
					return;
				}
				referralCur=formReferralSelect.SelectedReferral;
			}
			RefAttach refAttach=new RefAttach();
			refAttach.ReferralNum=referralCur.ReferralNum;
			refAttach.PatNum=_patientCur.PatNum;
			refAttach.RefType=ReferralType.RefFrom;
			refAttach.RefDate=DateTime.Today;
			if(referralCur.IsDoctor) {//whether using ehr or not
				refAttach.IsTransitionOfCare=true;
			}
			refAttach.ItemOrder=_listRefAttaches.Select(x => x.ItemOrder).DefaultIfEmpty().Max()+1;
			RefAttaches.Insert(refAttach);
			SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,_patientCur.PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
			FillReferrals();
		}

		///<summary>Fills the Referred From text box with the oldest (lowest ItemOrder) referral source with ReferralType.RefFrom.</summary>
		private void FillReferrals() {
			textReferredFrom.Clear();
			_listRefAttaches=RefAttaches.Refresh(_patientCur.PatNum);
			string firstRefNameTypeAbbr="";
			string firstRefType="";
			string firstRefFullName="";
			RefAttach refAttach=_listRefAttaches.FirstOrDefault(x => x.RefType==ReferralType.RefFrom);
			if(refAttach==null) {
				return;
			}
			Referral referralCur=ReferralL.GetReferral(refAttach.ReferralNum);
			if(referralCur==null) {
				return;
			}
			firstRefFullName=Referrals.GetNameLF(referralCur.ReferralNum);
			if(referralCur.PatNum>0) {
				firstRefType=" (patient)";
			}
			else if(referralCur.IsDoctor) {
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
			using FormReferralsPatient formReferralsPatient=new FormReferralsPatient();
			formReferralsPatient.PatNum=_patientCur.PatNum;
			formReferralsPatient.ShowDialog();
			FillReferrals();
			SetRequiredFields();
		}

		private void butViewSSN_Click(object sender,EventArgs e) {
			textSSN.Text=Patients.SSNFormatHelper(_patientOld.SSN,false);
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(Permissions.PatientSSNView,_patientCur.PatNum,logtext);
		}

		private void butViewBirthdate_Click(object sender,EventArgs e) {
			//Button not visible unless Birthdate is masked
			odDatePickerBirthDate.SetDateTime(_patientOld.Birthdate);
			string logtext="Date of birth unmasked in Patient Edit";
			SecurityLogs.MakeLogEntry(Permissions.PatientDOBView,_patientCur.PatNum,logtext);
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
			object[] objectArrayParameters=new object[] { isValid,_patientCur };
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_beginning",objectArrayParameters);
			Patient patientOld=_patientCur.Copy();
			if((bool)objectArrayParameters[0]==false) {//Didn't pass plug-in validation
				return;
			}
			bool isCDSinterventionCheckRequired=false;//checks selected values
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
				string usedBy=Patients.ChartNumUsedBy(textChartNumber.Text,_patientCur.PatNum);
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
				_patientCur.SiteNum=0;
			}
			if(textSite.Text != "" && textSite.Text != Sites.GetDescription(_patientCur.SiteNum)) {
				long matchingSite=Sites.FindMatchSiteNum(textSite.Text);
				if(matchingSite==-1) {
					MessageBox.Show(Lan.g(this,"Invalid Site description."));
					return;
				}
				else {
					_patientCur.SiteNum=matchingSite;
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
				_patientCur.PriProv=comboPriProv.GetSelectedProvNum();
			}
			//Don't allow changing status from Archived if this is a merged patient.
			if(_patientOld.PatStatus!=_listPatientStatuses[listStatus.SelectedIndex] && _patientOld.PatStatus==PatientStatus.Archived && 
				PatientLinks.WasPatientMerged(_patientOld.PatNum)) 
			{
				MsgBox.Show(this,"Not allowed to change the status of a merged patient.");
				return;
			}
			if(IsNew && PrefC.HasClinicsEnabled) {
				if(!PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters) && comboClinic.SelectedClinicNum==0) {
					MsgBox.Show(this,"Current settings for clinics do not allow patients to be added to the 'Unassigned' clinic. Please select a clinic.");
					return;
				}
				Def selectedBillingType=comboBillType.GetSelected<Def>();
				long clinicDefaultBillingTypeDefNum=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,comboClinic.SelectedClinicNum));
				if(selectedBillingType.DefNum!=clinicDefaultBillingTypeDefNum) {//New patient's selected default billing type doesn't match the clinic's.
					string clinicDefaultBillingTypeName=_listBillingTypes.Find(x => x.DefNum==clinicDefaultBillingTypeDefNum).ItemName;
					//Confirm whether or not the user wants to change patient's default billing type to the clinic's default.
					if(MessageBox.Show(Lan.g(this,"The selected billing type does not match the selected clinic's default billing type. "+
						"Would you like to change this patient's billing type from ")+selectedBillingType.ItemName+" to "+clinicDefaultBillingTypeName+"?","",
						MessageBoxButtons.YesNo)==DialogResult.Yes)
					{
						comboBillType.SetSelectedDefNum(clinicDefaultBillingTypeDefNum);
					}
				}
			}
			//Check SSN for US Formatting.  If SSN is masked and hasn't been changed, don't check.  Same checks from textSSN_Validating()
			if(CultureInfo.CurrentCulture.Name=="en-US"	&& textSSN.Text!=""//Only validate if US and SSN is not blank.
				&& ((PrefC.GetBool(PrefName.PatientSSNMasked) && textSSN.Text!=_maskedSSNOld)//If SSN masked, only validate if changed
					|| (!PrefC.GetBool(PrefName.PatientSSNMasked) && textSSN.Text!=_patientOld.SSN)))//If SSN isn't masked only validate if changed
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
				SecurityLogs.MakeLogEntry(Permissions.RequiredFields,_patientCur.PatNum,"Saved patient with required fields missing.");
			}
			_patientCur.LName=textLName.Text;
			_patientCur.FName=textFName.Text;
			_patientCur.MiddleI=textMiddleI.Text;
			_patientCur.Preferred=textPreferred.Text;
			_patientCur.Title=textTitle.Text;
			_patientCur.Salutation=textSalutation.Text;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Mother's maiden name UI is only used when EHR is enabled.
				_ehrPatientCur.MotherMaidenFname=textMotherMaidenFname.Text;
				_ehrPatientCur.MotherMaidenLname=textMotherMaidenLname.Text;
			}
			_patientCur.PatStatus=_listPatientStatuses[listStatus.SelectedIndex];//1:1
			switch(_patientCur.PatStatus) {
				case PatientStatus.Deceased: 
					if(_patientOld.PatStatus!=PatientStatus.Deceased) {
						List<Appointment> listAppointmentsFuture=Appointments.GetFutureSchedApts(_patientCur.PatNum);
						if(listAppointmentsFuture.Count>0) {
							string apptDates=string.Join("\r\n",listAppointmentsFuture.Take(10).Select(x => x.AptDateTime.ToString()));
							if(listAppointmentsFuture.Count>10) {
								apptDates+="(...)";
							}
							if(MessageBox.Show(Lan.g(this,"This patient has scheduled appointments in the future")+":\r\n"
									+apptDates+"\r\n"
									+Lan.g(this,"Would you like to delete them and set the patient to Deceased?"),Lan.g(this,"Delete future appointments?"),MessageBoxButtons.YesNo)==DialogResult.Yes) 
							{
								foreach(Appointment appointment in listAppointmentsFuture) {
									Appointments.Delete(appointment.AptNum,true);
								}
							}
							else {
								_patientCur.PatStatus=_patientOld.PatStatus;
								return;
							}
						}
					}
					break;
			}
			switch(listGender.SelectedIndex){
				case 0: _patientCur.Gender=PatientGender.Male; break;
				case 1: _patientCur.Gender=PatientGender.Female; break;
				case 2: _patientCur.Gender=PatientGender.Unknown; break;
			}
			switch(listPosition.SelectedIndex){
				case 0: _patientCur.Position=PatientPosition.Single; break;
				case 1: _patientCur.Position=PatientPosition.Married; break;
				case 2: _patientCur.Position=PatientPosition.Child; break;
				case 3: _patientCur.Position=PatientPosition.Widowed; break;
				case 4: _patientCur.Position=PatientPosition.Divorced; break;
			}
			//Only update birthdate if it was changed, might be masked.
			if(!PrefC.GetBool(PrefName.PatientDOBMasked) || _maskedDOBOld!=odDatePickerBirthDate.GetTextDate()) {
				_patientCur.Birthdate=PIn.Date(odDatePickerBirthDate.GetDateTime().ToString());
			}
			_patientCur.DateTimeDeceased=dateTimeDeceased;
			if(!PrefC.GetBool(PrefName.PatientSSNMasked) || _maskedSSNOld!=textSSN.Text) {//Only update SSN if it was changed, might be masked.
				if(CultureInfo.CurrentCulture.Name=="en-US") {
					if(Regex.IsMatch(textSSN.Text,@"^\d\d\d-\d\d-\d\d\d\d$")) {
						_patientCur.SSN=textSSN.Text.Substring(0,3)+textSSN.Text.Substring(4,2)
							+textSSN.Text.Substring(7,4);
					}
					else {
						_patientCur.SSN=textSSN.Text;
					}
				}
				else {//other cultures
					_patientCur.SSN=textSSN.Text;
				}
			}
			if(IsNew) {//Check if patient already exists.
				List<Patient> listPatients=Patients.GetListByName(_patientCur.LName,_patientCur.FName,_patientCur.PatNum);
				for(int i=0;i<listPatients.Count;i++) {
					//If dates match or aren't entered there might be a duplicate patient.
					if(listPatients[i].Birthdate==_patientCur.Birthdate
					|| listPatients[i].Birthdate.Year<1880
					|| _patientCur.Birthdate.Year<1880) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This patient might already exist.  Continue anyway?")) {
							return;
						}
						break;
					}
				}
			}
			//Send medication history consent to DoseSpot if checkDoseSpotConsent is checked.
			if(!_patientNoteCur.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx) && checkDoseSpotConsent.Checked) {
				try {
					long clinicNum=Clinics.ClinicNum;
					if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
						clinicNum=_patientCur.ClinicNum;
					}
					//Create a temporary patient to set important information to give to DoseSpot in case a patient is needed to be created in DoseSpot.
					Patient patientForDoseSpot=_patientCur.Copy();
					patientForDoseSpot.HmPhone=textHmPhone.Text;
					patientForDoseSpot.WkPhone=textWkPhone.Text;
					patientForDoseSpot.WirelessPhone=textWirelessPhone.Text;
					patientForDoseSpot.TxtMsgOk=(YN)listTextOk.SelectedIndex;
					patientForDoseSpot.Email=textEmail.Text;
					patientForDoseSpot.Address=textAddress.Text;
					patientForDoseSpot.Address2=textAddress2.Text;
					patientForDoseSpot.City=textCity.Text;
					patientForDoseSpot.State=textState.Text.Trim();
					patientForDoseSpot.Country=textCountry.Text;
					patientForDoseSpot.Zip=textZip.Text.Trim();
					DoseSpot.SetMedicationHistConsent(patientForDoseSpot,clinicNum);
					_patientNoteCur.Consent=PatConsentFlags.ShareMedicationHistoryErx;
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to set patient medication access consent for DoseSpot: ")+ex.Message);
					return;
				}
			}
			_patientCur.MedicaidID=textMedicaidID.Text;
			_ehrPatientCur.MedicaidState=textMedicaidState.Text;
			//Retrieve the value of the Snomed attribute for the selected SexOrientation.
			if(comboSexOrientation.SelectedIndex>=0) {
				SexOrientation sexOrientationEnum=(SexOrientation)comboSexOrientation.SelectedIndex;
				_ehrPatientCur.SexualOrientation=EnumTools.GetAttributeOrDefault<EhrAttribute>(sexOrientationEnum).Snomed;
				if(sexOrientationEnum==SexOrientation.AdditionalOrientation) {
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
			_patientCur.WkPhone=textWkPhone.Text;
			_patientCur.WirelessPhone=textWirelessPhone.Text;
			_patientCur.TxtMsgOk=(YN)listTextOk.SelectedIndex;
			_patientCur.Email=textEmail.Text;
			//PatCur.RecallInterval=PIn.PInt(textRecall.Text);
			_patientCur.ChartNumber=textChartNumber.Text;
			_patientCur.SchoolName=textSchool.Text;
			//address:
			_patientCur.HmPhone=textHmPhone.Text;
			_patientCur.Address=textAddress.Text;
			_patientCur.Address2=textAddress2.Text;
			_patientCur.City=textCity.Text;
			_patientCur.State=textState.Text.Trim();
			_patientCur.Country=textCountry.Text;
			_patientCur.Zip=textZip.Text.Trim();
			_patientCur.CreditType=textCreditType.Text;
			_patientCur.HasSuperBilling=checkSuperBilling.Checked;
			GetEmployerNum();
			//PatCur.EmploymentNote=textEmploymentNote.Text;
			if(comboLanguage.SelectedIndex==0){
				_patientCur.Language="";
			}
			else{
				_patientCur.Language=_listStringLanguages[comboLanguage.SelectedIndex-1];
			}
			_patientCur.AddrNote=textAddrNotes.Text;
			_patientCur.DateFirstVisit=PIn.Date(odDatePickerDateFirstVisit.GetDateTime().ToString());
			_patientCur.AskToArriveEarly=PIn.Int(textAskToArriveEarly.Text);
			_patientCur.SecProv=comboSecProv.GetSelectedProvNum();
			if(comboFeeSched.SelectedIndex==0){
				_patientCur.FeeSched=0;
			}
			else{
				_patientCur.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			_patientCur.BillingType=comboBillType.GetSelectedDefNum();
			_patientCur.ClinicNum=comboClinic.SelectedClinicNum;
			if(!_isUsingNewRaceFeature) {
				_listPatientRaces=new List<PatientRace>();
				for(int i=0;i<comboBoxMultiRace.SelectedIndices.Count;i++) {
					int selectedIdx=(int)comboBoxMultiRace.SelectedIndices[i];
					if(selectedIdx==0) {//If the none option was chosen, then ensure that no other race information is saved.
						_listPatientRaces.Clear();
						break;
					}
					if(selectedIdx==1) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2054-5"));//AfricanAmerican
					}
					else if(selectedIdx==2) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"1002-5"));//AmericanIndian
					}
					else if(selectedIdx==3) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2028-9"));//Asian
					}
					else if(selectedIdx==4) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));//DeclinedToSpecifyRace
					}
					else if(selectedIdx==5) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2076-8"));//HawaiiOrPacIsland
					}
					else if(selectedIdx==6) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2131-1"));//Other
					}
					else if(selectedIdx==7) {
						_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2106-3"));//White
					}
				}
				if(_listPatientRaces.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE)) {
					//If DeclinedToSpecify was chosen, then ensure that no other races are saved.
					_listPatientRaces.Clear();
					_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));
				}
				else if(_listPatientRaces.Any(x => x.CdcrecCode=="2131-1")) {//If Other was chosen, then ensure that no other races are saved.
					_listPatientRaces.Clear();
					_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2131-1"));
				}
				//In order to pass EHR G2 MU testing you must be able to have an ethnicity without a race, or a race without an ethnicity.  This will mean that patients will not count towards
				//meaningful use demographic calculations.  If we have time in the future we should probably alert EHR users when a race is chosen but no ethnicity, or a ethnicity but no race.
				if(comboEthnicity.SelectedIndex==1) {
					_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE));
				}
				else if(comboEthnicity.SelectedIndex==2) {
					_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2186-5"));//NotHispanic
				}
				else if(comboEthnicity.SelectedIndex==3) {
					_listPatientRaces.Add(new PatientRace(_patientCur.PatNum,"2135-2"));//Hispanic
				}
			}
			PatientRaces.Reconcile(_patientCur.PatNum,_listPatientRaces);//Insert, Update, Delete if needed.
			_patientCur.County=textCounty.Text;
			//site set when user picks from list.
			_patientCur.GradeLevel=(PatientGrade)comboGradeLevel.SelectedIndex;
			_patientCur.Urgency=(TreatmentUrgency)comboUrgency.SelectedIndex;
			//ResponsParty handled when buttons are pushed.
			if(Programs.IsEnabled(ProgramName.TrophyEnhanced)) {
				_patientCur.TrophyFolder=textTrophyFolder.Text;
			}
			_patientCur.Ward=textWard.Text;
			_patientCur.PreferContactMethod=(ContactMethod)comboContact.SelectedIndex;
			_patientCur.PreferConfirmMethod=(ContactMethod)comboConfirm.SelectedIndex;
			_patientCur.PreferRecallMethod=(ContactMethod)comboRecall.SelectedIndex;
			if(ListTools.In(0,comboExcludeECR.SelectedIndices)) { //If "None" is selected
				_commOptOut.OptOutSms=0;
				_commOptOut.OptOutEmail=0;
			}
			else {
				List<CommOptOutMode> listCommOptOutMode=comboExcludeECR.GetListSelected<CommOptOutMode>();
				//In the future, we could enhance this UI to allow granularity of selecting certain CommOptOutTypes and not others.
				_commOptOut.OptOutSms=listCommOptOutMode.Any(x => x==CommOptOutMode.Text) ? CommOptOutType.All : 0;
				_commOptOut.OptOutEmail=listCommOptOutMode.Any(x => x==CommOptOutMode.Email) ? CommOptOutType.All : 0;
			}
			CommOptOuts.Upsert(_commOptOut);
			_patientCur.AdmitDate=PIn.Date(odDatePickerAdmitDate.GetDateTime().ToString());
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_patientCur.CanadianEligibilityCode=(byte)comboCanadianEligibilityCode.SelectedIndex;
			}
			if(_patientCur.Guarantor==0){
				_patientCur.Guarantor=_patientCur.PatNum;
			}
			_patientNoteCur.ICEName=textIceName.Text;
			_patientNoteCur.ICEPhone=textIcePhone.Text;
			_patientCur.HasSignedTil=checkBoxSignedTil.Checked;//Update Signed Truth in Lending State
			Patients.Update(_patientCur,_patientOld);
			PatientNotes.Update(_patientNoteCur,_patientCur.Guarantor);
			string strPatPriProvDesc=Providers.GetLongDesc(_patientCur.PriProv);
			Patients.InsertPrimaryProviderChangeSecurityLogEntry(_patientOld,_patientCur);
			if(checkRestrictSched.Checked) {
				PatRestrictions.Upsert(_patientCur.PatNum,PatRestrict.ApptSchedule);//will only insert if one does not already exist in the db.
			}
			else {
				PatRestrictions.RemovePatRestriction(_patientCur.PatNum,PatRestrict.ApptSchedule);
			}
			if(_patientCur.Birthdate!=_patientOld.Birthdate || _patientCur.Gender!=_patientOld.Gender) {
				isCDSinterventionCheckRequired=true;
			}
			if(isCDSinterventionCheckRequired && CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(_patientCur,_patientCur);//both should be patCur.
				formCDSIntervention.ShowIfRequired(false);
			}
			#region 'Same' Checkboxes
			bool isAuthArchivedEdit=Security.IsAuthorized(Permissions.ArchivedPatientEdit,true);
			if(checkArriveEarlySame.Checked){
				Patients.UpdateArriveEarlyForFam(_patientCur,isAuthArchivedEdit);
			}
			//Only family checked.
			if(checkAddressSame.Checked && !checkAddressSameForSuperFam.Checked){
				//might want to include a mechanism for comparing fields to be overwritten
				Patients.UpdateAddressForFam(_patientCur,false,isAuthArchivedEdit);
			}
			//SuperFamily is checked, family could be checked or unchecked.
			else if(checkAddressSameForSuperFam.Checked) {
				Patients.UpdateAddressForFam(_patientCur,true,isAuthArchivedEdit);
			}
			if(checkBillProvSame.Checked) {
				List<Patient> listPatientsForPriProvEdit=_familyCur.ListPats.ToList().FindAll(x => x.PatNum!=_patientCur.PatNum && x.PriProv!=_patientCur.PriProv);
				if(!isAuthArchivedEdit) {//Remove Archived patients if not allowed to edit so we don't create a log for them.
					listPatientsForPriProvEdit.RemoveAll(x => x.PatStatus==PatientStatus.Archived);
				}
				//true if any family member has a different PriProv and the user is authorized for PriProvEdit
				bool isChangePriProvs=(listPatientsForPriProvEdit.Count>0 && Security.IsAuthorized(Permissions.PatPriProvEdit,DateTime.MinValue,true,true));
				Patients.UpdateBillingProviderForFam(_patientCur,isChangePriProvs,isAuthArchivedEdit);//if user is not authorized this will not update PriProvs for fam
			}
			if(checkNotesSame.Checked){
				Patients.UpdateNotesForFam(_patientCur,isAuthArchivedEdit);
			}
			if(checkEmailPhoneSame.Checked) {
				Patients.UpdateEmailPhoneForFam(_patientCur,isAuthArchivedEdit);
			}
			#endregion 'Same' Checkboxes
			if(_patientCur.BillingType!=_patientOld.BillingType) {
				AutomationL.Trigger(AutomationTrigger.SetBillingType,null,_patientCur.PatNum);
				Patients.InsertBillTypeChangeSecurityLogEntry(_patientOld,_patientCur);
			}
			//If this patient is also a referral source,
			//keep address info synched:
			Referral referral=Referrals.GetFirstOrDefault(x => x.PatNum==_patientCur.PatNum);
			if(referral!=null) {
				referral.LName=_patientCur.LName;
				referral.FName=_patientCur.FName;
				referral.MName=_patientCur.MiddleI;
				referral.Address=_patientCur.Address;
				referral.Address2=_patientCur.Address2;
				referral.City=_patientCur.City;
				referral.ST=_patientCur.State;
				referral.SSN=_patientCur.SSN;
				referral.Zip=_patientCur.Zip;
				referral.Telephone=TelephoneNumbers.FormatNumbersExactTen(_patientCur.HmPhone);
				referral.EMail=_patientCur.Email;
				Referrals.Update(referral);
				Referrals.RefreshCache();
			}
			//if patient is inactive, deceased, etc., then disable any recalls
			Patients.UpdateRecalls(_patientCur,_patientOld,"Edit Patient Window");
			//If there is an existing HL7 def enabled, send an ADT message if there is an outbound ADT message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//new patients get the A04 ADT, updating existing patients we send an A08
				MessageHL7 messageHL7=null;
				if(IsNew) {
					messageHL7=MessageConstructor.GenerateADT(_patientCur,Patients.GetPat(_patientCur.Guarantor),EventTypeHL7.A04);
				}
				else {
					messageHL7=MessageConstructor.GenerateADT(_patientCur,Patients.GetPat(_patientCur.Guarantor),EventTypeHL7.A08);
				}
				//Will be null if there is no outbound ADT message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=_patientCur.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show(this,messageHL7.ToString());
					}
				}
			}
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(_patientCur.PatNum));
			}
			long defNum=comboSpecialty.GetSelectedDefNum();
			if(_defLinkPatientCur!=null) {
				if(defNum==0) {
					DefLinks.Delete(_defLinkPatientCur.DefLinkNum);
				}
				else {
					_defLinkPatientCur.DefNum=defNum;
					DefLinks.Update(_defLinkPatientCur);
				}
			}
			else if(defNum!=0){//if the patient does not have a specialty and "Unspecified" is not selected. 
				DefLink defLinkNew=new DefLink();
				defLinkNew.DefNum=defNum;
				defLinkNew.FKey=_patientCur.PatNum;
				defLinkNew.LinkType=DefLinkType.Patient;
				DefLinks.Insert(defLinkNew);
			}
			//The specialty could have changed so invalidate the cached specialty on the currently selected patient object in PatientL.cs
			PatientL.InvalidateSelectedPatSpecialty();
			if(!IsNew) {
				Patients.InsertAddressChangeSecurityLogEntry(_patientOld,_patientCur);
				PatientEvent.Fire(ODEventType.Patient,_patientCur);
			}
			PromptForSmsIfNecessary(patientOld,_patientCur);
			Plugins.HookAddCode(this,"FormPatientEdit.butOK_Click_end",_patientCur,_patientOld);
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
				List<RefAttach> listRefAttaches=RefAttaches.GetRefAttaches(new List<long> { _patientCur.PatNum });
				for(int i=0;i<listRefAttaches.Count;i++) {
					RefAttaches.Delete(listRefAttaches[i]);
				}
				Patients.Delete(_patientCur);
				SecurityLogs.MakeLogEntry(Permissions.PatientEdit,_patientCur.PatNum,Lan.g(this,"Canceled creating new patient. Deleting patient record."));
			}
			if(_hasGuardiansChanged) {  //If guardian information was changed, and user canceled.
				//revert any changes to the guardian list for all family members
				Guardians.RevertChanges(_listGuardiansForFamOld,_familyCur.ListPats.Select(x => x.PatNum).ToList());
			}
		}
    }
}









