using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using System.Linq;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormPatientAddAll:FormODBase {
		public string LName;
		public string FName;
		public DateTime Birthdate;
		public long PatNumSelected;
		private string _lnameOld1;
		private string _fnameOld2="";
		private string _fnameOld3="";
		private string _fnameOld4="";
		private string _fnameOld5="";
		private string _emailOld1="";
		private string _emailOld2="";
		private string _emailOld3="";
		private string _emailOld4="";
		private string _emailOld5="";
		private string _phoneOld1="";
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listEmps1;
		private bool _isMouseInListEmps1;
		private string _empOriginal1;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listEmps2;
		private bool _isMouseInListEmps2;
		private string _empOriginal2;
		private List<Carrier> _listCarriersSimilar1;
		private string _carOriginal1;
		private System.Windows.Forms.ListBox listCars1;
		private bool _isMouseInListCars1;
		private Carrier _carrierSelected1;
		private List<Carrier> _listCarriersSimilar2;
		private string _carOriginal2;
		private System.Windows.Forms.ListBox listCars2;
		private bool _isMouseInListCars2;
		private Carrier _carrierSelected2;
		///<summary>If user picks a plan from list, but then changes one of the critical fields, this will be ignored.  Keep in mind that the plan here is just a copy.  It can't be updated, but must instead be inserted.</summary>
		private InsPlan _insPlanSelected1;
		private InsPlan _insPlanSelected2;
		private ToolTip _toolTipReferred;
		private Referral _referral;
		private System.Windows.Forms.ListBox listStates;//displayed from within code, not designer
		private string _stateOriginal;//used in the medicaidState dropdown logic
		private bool _isMouseInListStates;
		private List<RequiredField> _listRequiredFields;
		private bool _isMissingRequiredFields;
		private bool _isLoad;//To keep track if ListBoxes' selected index is changed by the user
		private ErrorProvider _errorProvider=new ErrorProvider();
		private bool _isValidating=false;
		private Commlog _commlog;
		private List<Provider> _listProviders;
		private List<ZipCode> _listZipCodes;
		private List<Def> _listDefsBillingType;

		public FormPatientAddAll() {
			InitializeComponent();
			InitializeLayoutManager();
			InitializeControls();
			Lan.F(this);
		}

		private void InitializeControls() {
			_toolTipReferred=new ToolTip();
			_toolTipReferred.InitialDelay=500;
			_toolTipReferred.ReshowDelay=100;
			listEmps1=new ListBox();
			listEmps1.Location=new Point(groupIns1.Left+textEmployer1.Left,groupIns1.Top+textEmployer1.Bottom);
			listEmps1.Size=new Size(254,100);
			listEmps1.Visible=false;
			listEmps1.Click += new System.EventHandler(listEmps1_Click);
			listEmps1.DoubleClick += new System.EventHandler(listEmps1_DoubleClick);
			listEmps1.MouseEnter += new System.EventHandler(listEmps1_MouseEnter);
			listEmps1.MouseLeave += new System.EventHandler(listEmps1_MouseLeave);
			LayoutManager.Add(listEmps1,this);
			listEmps1.BringToFront();
			listEmps2=new ListBox();
			listEmps2.Location=new Point(groupIns2.Left+textEmployer2.Left,
				groupIns2.Top+textEmployer2.Bottom);
			listEmps2.Size=new Size(254,100);
			listEmps2.Visible=false;
			listEmps2.Click += new System.EventHandler(listEmps2_Click);
			listEmps2.DoubleClick += new System.EventHandler(listEmps2_DoubleClick);
			listEmps2.MouseEnter += new System.EventHandler(listEmps2_MouseEnter);
			listEmps2.MouseLeave += new System.EventHandler(listEmps2_MouseLeave);
			LayoutManager.Add(listEmps2,this);
			listEmps2.BringToFront();
			listCars1=new ListBox();
			listCars1.Location=new Point(groupIns1.Left+textCarrier1.Left,groupIns1.Top+textCarrier1.Bottom);
			listCars1.Size=new Size(700,100);
			listCars1.HorizontalScrollbar=true;
			listCars1.Visible=false;
			listCars1.Click += new System.EventHandler(listCars1_Click);
			listCars1.DoubleClick += new System.EventHandler(listCars1_DoubleClick);
			listCars1.MouseEnter += new System.EventHandler(listCars1_MouseEnter);
			listCars1.MouseLeave += new System.EventHandler(listCars1_MouseLeave);
			LayoutManager.Add(listCars1,this);
			listCars1.BringToFront();
			listCars2=new ListBox();
			listCars2.Location=new Point(groupIns2.Left+textCarrier2.Left,groupIns2.Top+textCarrier2.Bottom);
			listCars2.Size=new Size(700,100);
			listCars2.HorizontalScrollbar=true;
			listCars2.Visible=false;
			listCars2.Click += new System.EventHandler(listCars2_Click);
			listCars2.DoubleClick += new System.EventHandler(listCars2_DoubleClick);
			listCars2.MouseEnter += new System.EventHandler(listCars2_MouseEnter);
			listCars2.MouseLeave += new System.EventHandler(listCars2_MouseLeave);
			LayoutManager.Add(listCars2,this);
			listCars2.BringToFront();
			listStates=new ListBox();
			listStates.Location=new Point(textState.Left+groupBox1.Left,textState.Bottom+groupBox1.Top);
			listStates.Size=new Size(textState.Width,100);
			listStates.Visible=false;
			listStates.Click += new System.EventHandler(listStates_Click);
			listStates.MouseEnter += new System.EventHandler(listStates_MouseEnter);
			listStates.MouseLeave += new System.EventHandler(listStates_MouseLeave);
			Controls.Add(listStates);
			LayoutManager.Add(listStates,this);
			listStates.BringToFront();
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				textEmployer1.ReadOnly=true;
				textEmployer2.ReadOnly=true;
				textPhone1.ReadOnly=true;
				textPhone2.ReadOnly=true;
				textCarrier1.ReadOnly=true;
				textCarrier2.ReadOnly=true;
				textGroupName1.ReadOnly=true;
				textGroupName2.ReadOnly=true;
				textGroupNum1.ReadOnly=true;
				textGroupNum2.ReadOnly=true;
			}
		}

		private void FormPatientAddAll_Load(object sender,EventArgs e) {
			_isLoad=true;
			textLName1.Text=LName;
			textFName1.Text=FName;
			if(Birthdate.Year<1880) {
				textBirthdate1.Text="";
			}
			else {
				textBirthdate1.Text=Birthdate.ToShortDateString();
			}
			textBirthdate1_Validated(this,null);
			listTextOk1.SelectedIndex=0;
			listTextOk2.SelectedIndex=0;
			listTextOk3.SelectedIndex=0;
			listTextOk4.SelectedIndex=0;
			listTextOk5.SelectedIndex=0;
			SetGenderListBox(listGender1);
			SetGenderListBox(listGender2);
			SetGenderListBox(listGender3);
			SetGenderListBox(listGender4);
			SetGenderListBox(listGender5);
			listPosition1.SelectedIndex=1;
			listPosition2.SelectedIndex=1;
			if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				comboPriProv1.Items.Add(Lan.g(this,"Select Provider"));
				comboPriProv2.Items.Add(Lan.g(this,"Select Provider"));
				comboPriProv3.Items.Add(Lan.g(this,"Select Provider"));
				comboPriProv4.Items.Add(Lan.g(this,"Select Provider"));
				comboPriProv5.Items.Add(Lan.g(this,"Select Provider"));
			}
			comboSecProv1.Items.Add(Lan.g(this,"none"));
			comboSecProv1.SelectedIndex=0;
			comboSecProv2.Items.Add(Lan.g(this,"none"));
			comboSecProv2.SelectedIndex=0;
			comboSecProv3.Items.Add(Lan.g(this,"none"));
			comboSecProv3.SelectedIndex=0;
			comboSecProv4.Items.Add(Lan.g(this,"none"));
			comboSecProv4.SelectedIndex=0;
			comboSecProv5.Items.Add(Lan.g(this,"none"));
			comboSecProv5.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++){
				comboPriProv1.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv1.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv2.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv2.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv3.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv3.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv4.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv4.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv5.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv5.Items.Add(_listProviders[i].GetLongDesc());
			}
			int defaultindex=0;
			if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=0) {
					defaultindex=Providers.GetIndex((Clinics.GetFirstOrDefault(x => x.ClinicNum==Clinics.ClinicNum).DefaultProv));
				}
				else {
					defaultindex=Providers.GetIndex(PrefC.GetLong(PrefName.PracticeDefaultProv));
				}
				if(defaultindex==-1) {//default provider hidden
					defaultindex=0;
				}
			}
			comboPriProv1.SelectedIndex=defaultindex;
			comboPriProv2.SelectedIndex=defaultindex;
			comboPriProv3.SelectedIndex=defaultindex;
			comboPriProv4.SelectedIndex=defaultindex;
			comboPriProv5.SelectedIndex=defaultindex;
			FillListPatStatus(listStatus1);
			FillListPatStatus(listStatus2);
			FillListPatStatus(listStatus3);
			FillListPatStatus(listStatus4);
			FillListPatStatus(listStatus5);
			FillComboBillTypes(comboBillType1,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType2,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType3,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType4,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType5,Clinics.ClinicNum);
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
			}
			else if(PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters)){
				comboClinic1.IncludeUnassigned=true;
				comboClinic2.IncludeUnassigned=true;
				comboClinic3.IncludeUnassigned=true;
				comboClinic4.IncludeUnassigned=true;
				comboClinic5.IncludeUnassigned=true;
			}
			if(!Security.IsAuthorized(Permissions.RefAttachAdd,true)) {
				butClearReferralSource.Enabled=false;
				butReferredFrom.Enabled=false;
			}
			if(!PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				labelST.Text="ST";
				textCountry.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSSN.Text=Lan.g(this,"SIN");
				labelZip.Text=Lan.g(this,"Postal Code");
				labelST.Text=Lan.g(this,"Province");
				labelGroupNum1.Text=Lan.g(this,"Plan Number");
				labelGroupNum2.Text=Lan.g(this,"Plan Number");
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("GB")) {//en-GB
				labelZip.Text=Lan.g(this,"Postcode");
				labelST.Text="";//no such thing as state in GB
			}
			_listZipCodes=ZipCodes.GetDeepCopy(true);
			FillComboZip();
			ResetSubscriberLists();
			_listRequiredFields=RequiredFields.GetWhere(x => x.FieldType==RequiredFieldType.PatientInfo);
			RemoveUnnecessaryRequiredFields();
			_errorProvider.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			SetRequiredFields();
			_isLoad=false;
			Plugins.HookAddCode(this,"FormPatientAddAll.FormPatientAddAll_Load_end");
		}

		private void SetGenderListBox(ListBoxOD listBox) {
			listBox.Items.Add(PatientGender.Male.ToString(),PatientGender.Male);
			listBox.Items.Add(PatientGender.Female.ToString(),PatientGender.Female);
			listBox.Items.Add(PatientGender.Other.ToString(),PatientGender.Other);
			listBox.Items.Add(PatientGender.Unknown.ToString(),PatientGender.Unknown);
		}

		///<summary>Removes required fields that are not used in this window.</summary>
		private void RemoveUnnecessaryRequiredFields() {
			//Remove the ones that are only on the Edit Patient Information window
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.AdmitDate);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.AskArriveEarly);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.ChartNumber);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.CollegeName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.County);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.CreditType);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DateFirstVisit);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DateTimeDeceased);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DischargeDate);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EligibilityExceptCode);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Ethnicity);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.FeeSchedule);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GradeLevel);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Language);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MedicaidID);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MedicaidState);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MiddleInitial);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenFirstName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenLastName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferConfirmMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferContactMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferRecallMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferredName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Race);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.ResponsibleParty);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Salutation);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Site);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.StudentStatus);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Title);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.TreatmentUrgency);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.TrophyFolder);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Ward);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.WorkPhone);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EmergencyName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EmergencyPhone);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.SexualOrientation);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GenderIdentity);
		}

		private void FormPatientAddAll_Shown(object sender,EventArgs e) {
			
		}

		//<summary>Puts an asterisk next to the label and highlights the textbox/listbox/combobox/radiobutton background for all RequiredFields that
		///have their conditions met.</summary>butok_
		private void SetRequiredFields() {
			_isMissingRequiredFields=false;
			bool areConditionsMet;
			if(_listRequiredFields==null) {
				return;
			}
			for(int i=0;i<_listRequiredFields.Count;i++) {
				areConditionsMet=ConditionsAreMet(_listRequiredFields[i]);
				if(areConditionsMet) {
					labelRequiredFields.Visible=true;
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
							if(!labelAddrNotes.Text.Contains("*")) {
								labelAddrNotes.Text+="*";
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
							if(labelAddrNotes.Text.Contains("*")) {
								labelAddrNotes.Text=labelAddrNotes.Text.Replace("*","");
							}
							_errorProvider.SetError(textAddrNotes,"");
						}
						break;
					case RequiredFieldName.BillingType:
						SetRequiredComboBoxOD(labelBillType,comboBillType1,areConditionsMet,-1,"A billing type must be selected for the guarantor.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName2,textLName2,comboBillType2,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName3,textLName3,comboBillType3,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName4,textLName4,comboBillType4,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName5,textLName5,comboBillType5,areConditionsMet,-1,"A billing type must be selected.");
						break;
					case RequiredFieldName.Birthdate:
						SetRequiredTextBox(labelBirthAge,textBirthdate1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelBirthAge,textFName2,textLName2,textBirthdate2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelBirthAge,textFName3,textLName3,textBirthdate3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelBirthAge,textFName4,textLName4,textBirthdate4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelBirthAge,textFName5,textLName5,textBirthdate5,areConditionsMet);
						break;
					case RequiredFieldName.Carrier:
						SetRequiredTextBox(labelCarrier1,textCarrier1,areConditionsMet);
						break;
					case RequiredFieldName.City:
						SetRequiredTextBox(labelCity,textCity,areConditionsMet);
						break;
					case RequiredFieldName.Clinic:
						SetRequiredComboClinicPicker(labelClinic,comboClinic1,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic2,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic3,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic4,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic5,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						break;
					case RequiredFieldName.EmailAddress:
						SetRequiredTextBox(labelEmail,textEmail1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName2,textLName2,textEmail2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName3,textLName3,textEmail3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName4,textLName4,textEmail4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName5,textLName5,textEmail5,areConditionsMet);
						break;
					case RequiredFieldName.Employer:
						SetRequiredTextBox(labelEmployer1,textEmployer1,areConditionsMet);
						break;
					case RequiredFieldName.FirstName:
						SetRequiredTextBox(labelFName,textFName1,areConditionsMet);
						if(textLName2.Text!="") {
							SetRequiredTextBox(labelFName,textFName2,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textFName2,"");
						}
						if(textLName3.Text!="") {
							SetRequiredTextBox(labelFName,textFName3,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textFName3,"");
						}
						if(textLName4.Text!="") {
							SetRequiredTextBox(labelFName,textFName4,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textFName4,"");
						}
						if(textLName5.Text!="") {
							SetRequiredTextBox(labelFName,textFName5,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textFName5,"");
						}
						break;
					case RequiredFieldName.Gender:
						string strErrorMsg=Lan.g(this,"Gender cannot be 'Unknown'.");
						SetRequiredListBoxOD(labelGenPos,listGender1,areConditionsMet,2,strErrorMsg,ErrorIconAlignment.BottomLeft);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName2,textLName2,listGender2,areConditionsMet,2,strErrorMsg,ErrorIconAlignment.BottomLeft);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName3,textLName3,listGender3,areConditionsMet,2,strErrorMsg,ErrorIconAlignment.BottomLeft);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName4,textLName4,listGender4,areConditionsMet,2,strErrorMsg,ErrorIconAlignment.BottomLeft);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName5,textLName5,listGender5,areConditionsMet,2,strErrorMsg,ErrorIconAlignment.BottomLeft);
						break;
					case RequiredFieldName.GroupName:
						SetRequiredTextBox(labelGroupName1,textGroupName1,areConditionsMet);
						break;
					case RequiredFieldName.GroupNum:
						SetRequiredTextBox(labelGroupNum1,textGroupNum1,areConditionsMet);
						break;
					case RequiredFieldName.HomePhone:
						SetRequiredTextBox(labelHmPhone,textHmPhone,areConditionsMet);
						break;
					case RequiredFieldName.InsurancePhone:
						SetRequiredTextBox(labelPhone1,textPhone1,areConditionsMet);
						break;
					case RequiredFieldName.InsuranceSubscriber:
						SetRequiredComboBoxOD(labelSubscriber1,comboSubscriber1,areConditionsMet,0,"Selection cannot be 'none'");
						break;
					case RequiredFieldName.InsuranceSubscriberID:
						SetRequiredTextBox(labelSubscriberID1,textSubscriberID1,areConditionsMet);
						break;
					case RequiredFieldName.LastName:
						SetRequiredTextBox(labelLName,textLName1,areConditionsMet);
						if(textFName2.Text!="") {
							SetRequiredTextBox(labelLName,textLName2,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textLName2,"");
						}
						if(textFName3.Text!="") {
							SetRequiredTextBox(labelLName,textLName3,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textLName3,"");
						}
						if(textFName4.Text!="") {
							SetRequiredTextBox(labelLName,textLName4,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textLName4,"");
						}
						if(textFName5.Text!="") {
							SetRequiredTextBox(labelLName,textLName5,areConditionsMet);
						}
						else {
							_errorProvider.SetError(textLName5,"");
						}
						break;
					case RequiredFieldName.PrimaryProvider:
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							SetRequiredComboBoxOD(labelPriProv,comboPriProv1,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName2,textLName2,comboPriProv2,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName3,textLName3,comboPriProv3,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName4,textLName4,comboPriProv4,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName5,textLName5,comboPriProv5,areConditionsMet,0,"Selection cannot be 'Select Provider'");
						}
						break;
					case RequiredFieldName.ReferredFrom:
						SetRequiredTextBox(labelReferred,textReferredFrom,areConditionsMet);
						break;
					case RequiredFieldName.SecondaryProvider:
						SetRequiredComboBoxOD(labelSecProv,comboSecProv1,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName2,textLName2,comboSecProv2,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName3,textLName3,comboSecProv3,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName4,textLName4,comboSecProv4,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName5,textLName5,comboSecProv5,areConditionsMet,0,"Selection cannot be 'none'");
						break;
					case RequiredFieldName.SocialSecurityNumber:
						SetRequiredTextBox(labelSSN,textSSN1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName2,textLName2,textSSN2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName3,textLName3,textSSN3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName4,textLName4,textSSN4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName5,textLName5,textSSN5,areConditionsMet);
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
					case RequiredFieldName.TextOK:
						SetRequiredListControl(labelTextOk,listTextOk1,areConditionsMet,0,"Selection cannot be '??'");
						SetRequiredListOrComboControlNonGuarantor(labelTextOk,textFName2,textLName2,listTextOk2,areConditionsMet,0,"Selection cannot be '??'");
						SetRequiredListOrComboControlNonGuarantor(labelTextOk,textFName3,textLName3,listTextOk3,areConditionsMet,0,"Selection cannot be '??'");
						SetRequiredListOrComboControlNonGuarantor(labelTextOk,textFName4,textLName4,listTextOk4,areConditionsMet,0,"Selection cannot be '??'");
						SetRequiredListOrComboControlNonGuarantor(labelTextOk,textFName5,textLName5,listTextOk5,areConditionsMet,0,"Selection cannot be '??'");
						break;
					case RequiredFieldName.WirelessPhone:
						SetRequiredTextBox(labelWirelessPhone,textWirelessPhone1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName2,textLName2,textWirelessPhone2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName3,textLName3,textWirelessPhone3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName4,textLName4,textWirelessPhone4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName5,textLName5,textWirelessPhone5,areConditionsMet);
						break;
					case RequiredFieldName.Zip:
						SetRequiredTextBox(labelZip,textZip,areConditionsMet);
						break;
				}
			}
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
				if(!areConditionsMet && previousFieldName!=-1) {
					if((int)listRequiredFieldConditions[i].ConditionType!=previousFieldName) {
						return false;//None of the conditions of the previous type were met
					}
				}
				areConditionsMet=false;
				switch(listRequiredFieldConditions[i].ConditionType) {
					case RequiredFieldName.Birthdate://But actually using Age for calculations	
						//All Birthdate conditions will be true if any of the ages meet the conditions.
						List<ValidDate> listValidDates=new List<ValidDate>();
						listValidDates.Add(textBirthdate1);
						listValidDates.Add(textBirthdate2);
						listValidDates.Add(textBirthdate3);
						listValidDates.Add(textBirthdate4);
						listValidDates.Add(textBirthdate5);
						for(int j=0;j<listValidDates.Count;j++) {
							if(listValidDates[j].Text=="" || !listValidDates[j].IsValid()) {
								continue;
							}
							DateTime birthdate=PIn.Date(listValidDates[j].Text);
							if(birthdate>DateTime.Today) {
								birthdate=birthdate.AddYears(-100);
							}
							int ageEntered=DateTime.Today.Year-birthdate.Year;
							if(birthdate>DateTime.Today.AddYears(-ageEntered)) {
								ageEntered--;
							}
							List<RequiredFieldCondition> listRequiredFieldConditionsAge=listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Birthdate);
							List<bool> listAreCondsMet=new List<bool>();
							for(int k=0;k<listRequiredFieldConditionsAge.Count;k++) {
								listAreCondsMet.Add(CondOpComparer(ageEntered,listRequiredFieldConditionsAge[k].Operator,PIn.Int(listRequiredFieldConditionsAge[k].ConditionValue)));
							}
							if(listAreCondsMet.Count<2 || listRequiredFieldConditionsAge[1].ConditionRelationship==LogicalOperator.And) {
								areConditionsMet=!listAreCondsMet.Contains(false);
							}
							else {
								areConditionsMet=listAreCondsMet.Contains(true);
							}
							if(areConditionsMet) {
								break;//From the for loop
							}
						}
						break;
					case RequiredFieldName.Clinic:
						if(!PrefC.HasClinicsEnabled) {
							areConditionsMet=true;
							break;
						}
						List<ComboBoxClinicPicker> listComboBoxClinicPickers=new List<ComboBoxClinicPicker>();
						listComboBoxClinicPickers.Add(comboClinic1);
						listComboBoxClinicPickers.Add(comboClinic2);
						listComboBoxClinicPickers.Add(comboClinic3);
						listComboBoxClinicPickers.Add(comboClinic4);
						listComboBoxClinicPickers.Add(comboClinic5);
						for(int j = 0;j<listComboBoxClinicPickers.Count;j++) {
							long selectedClinicNum=listComboBoxClinicPickers[j].SelectedClinicNum;
							if(selectedClinicNum<0) {
								continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& PIn.Long(listRequiredFieldConditions[i].ConditionValue)==selectedClinicNum) 
							{
								areConditionsMet=true;
								break;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Clinic)
										.Any(x => x.ConditionValue==selectedClinicNum.ToString())) 
							{
								areConditionsMet=true;
								break;
							}
						}
						break;
					case RequiredFieldName.Gender:
						//All gender conditions will be true if any gender list box meets the condition.
						List<ListBoxOD> listBoxesGender=new List<ListBoxOD>();
						listBoxesGender.Add(listGender1);
						listBoxesGender.Add(listGender2);
						listBoxesGender.Add(listGender3);
						listBoxesGender.Add(listGender4);
						listBoxesGender.Add(listGender5);
						for(int j=0;j<listBoxesGender.Count;j++) {
							if(listBoxesGender[j].SelectedItem==null) {
							 continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& listRequiredFieldConditions[i].ConditionValue==listBoxesGender[j].SelectedItem.ToString()) 
							{
								areConditionsMet=true;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Gender)
										.Any(x => x.ConditionValue==listBoxesGender[j].SelectedItem.ToString()))
							{
								areConditionsMet=true;
							}
						}
						break;
					case RequiredFieldName.PrimaryProvider:
						//Conditions of type PrimaryProvider store the ProvNum as the ConditionValue.
						//All Primary Provider conditions will be true if any of the Primary Providers meet the condition.
						List<UI.ComboBox> listComboBoxesProvider = new List<UI.ComboBox>();
						listComboBoxesProvider.Add(comboPriProv1);
						listComboBoxesProvider.Add(comboPriProv2);
						listComboBoxesProvider.Add(comboPriProv3);
						listComboBoxesProvider.Add(comboPriProv4);
						listComboBoxesProvider.Add(comboPriProv5);
						for(int j=0;j<listComboBoxesProvider.Count;j++) {
							int provIdx=listComboBoxesProvider[j].SelectedIndex;
							if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
								provIdx--;//To account for 'Select Provider'
							}
							if(provIdx<0) {
								continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& PIn.Long(listRequiredFieldConditions[i].ConditionValue)==_listProviders[provIdx].ProvNum) 
							{
								areConditionsMet=true;
								break;//From the for loop
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.PrimaryProvider)
										.Any(x => x.ConditionValue==_listProviders[provIdx].ProvNum.ToString())) 
							{
								areConditionsMet=true;
								break;//From the for loop
							}
						}
						break;
					default://The field is not on this form
						areConditionsMet=true;
						break;
				}
				previousFieldName=(int)listRequiredFieldConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it also blank, highlights the textbox
		///background.</summary>
		private void SetRequiredTextBox(Label label,TextBox textBox,bool areConditionsMet) {
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(textBox.Text=="") {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(textBox,"Text box cannot be blank.");
					}
				}
				else {
					_errorProvider.SetError(textBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				_errorProvider.SetError(textBox,"");
			}
			if(textBox.Name==textReferredFrom.Name) {
				_errorProvider.SetIconPadding(textBox,44);//Width of the pick and remove buttons
			}
			else if(textBox.Name==textZip.Name) {
				_errorProvider.SetIconPadding(textBox,21);//Width of the drop down button
			}
		}

		private void SetRequiredTextBoxNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,TextBox textBoxRequired,
			bool areConditionsMet) 
		{
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredTextBox(label,textBoxRequired,areConditionsMet);
			}
			else {
				_errorProvider.SetError(textBoxRequired,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredListControl(Label label,ListControl listControl,bool areConditionsMet,int disallowedIdx,string errorMsg,
			ErrorIconAlignment errorIconAlignment=ErrorIconAlignment.MiddleRight)
		{
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(listControl.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(listControl,errorMsg);
						_errorProvider.SetIconAlignment(listControl,errorIconAlignment);
					}
				}
				else {
					_errorProvider.SetError(listControl,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				_errorProvider.SetError(listControl,"");
			} 
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredComboBoxOD(Label label,UI.ComboBox comboBox,bool areConditionsMet,int disallowedIdx,string errorMsg,
			ErrorIconAlignment errorIconAlignment=ErrorIconAlignment.MiddleRight)
		{
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(comboBox.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(comboBox,errorMsg);
						_errorProvider.SetIconAlignment(comboBox,errorIconAlignment);
					}
				}
				else {
					_errorProvider.SetError(comboBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				_errorProvider.SetError(comboBox,"");
			} 
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the listbox background.</summary>
		private void SetRequiredListBoxOD(Label label,ListBoxOD listBox,bool areConditionsMet,int disallowedIdx,string errorMsg,
			ErrorIconAlignment errorIconAlignment=ErrorIconAlignment.MiddleRight) {
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(listBox.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(listBox,errorMsg);
						_errorProvider.SetIconAlignment(listBox,errorIconAlignment);
					}
				}
				else {
					_errorProvider.SetError(listBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				_errorProvider.SetError(listBox,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredComboClinicPicker(Label label,ComboBoxClinicPicker comboBoxClinicPicker,bool areConditionsMet,int disallowedClinicNum,string errorMsg){
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(comboBoxClinicPicker.SelectedClinicNum==disallowedClinicNum) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						_errorProvider.SetError(comboBoxClinicPicker,errorMsg);
						_errorProvider.SetIconAlignment(comboBoxClinicPicker,ErrorIconAlignment.MiddleRight);
					}
				}
				else {
					_errorProvider.SetError(comboBoxClinicPicker,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				_errorProvider.SetError(comboBoxClinicPicker,"");
			} 
		}

		private void SetRequiredListOrComboControlNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,Control controlRequired,
			bool areConditionsMet,int disallowedIdx,string errorMsg,ErrorIconAlignment errorIconAlignment=ErrorIconAlignment.MiddleRight)
		{
			if(string.IsNullOrWhiteSpace(textBoxFName.Text) && string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				_errorProvider.SetError(controlRequired,"");
				return;
			}
			//Either ListBox or ComboBoxOD.
			if(controlRequired is ListBox) {
				SetRequiredListControl(label,(ListControl)controlRequired,areConditionsMet,disallowedIdx,errorMsg,errorIconAlignment);
			}
			else {
				SetRequiredComboBoxOD(label,(UI.ComboBox)controlRequired,areConditionsMet,disallowedIdx,errorMsg,errorIconAlignment);
			}
		}

		///<summary>Calls the set required method that puts an asterisk next to the label if the field is required if the textbox first or last
		///name are empty when one is filled.</summary>
		private void SetRequiredListBoxODNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,ListBoxOD listBoxRequired,
			bool areConditionsMet,int disallowedIdx,string errorMsg,ErrorIconAlignment errorIconAlignment=ErrorIconAlignment.MiddleRight) {
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredListBoxOD(label,listBoxRequired,areConditionsMet,disallowedIdx,errorMsg,errorIconAlignment);
			}
			else {
				_errorProvider.SetError(listBoxRequired,"");
			}
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
		
		private void textBox_Leave(object sender,System.EventArgs e) {
			SetRequiredFields();
		}
		
		private void ListBox_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(_isLoad) {
				return;
			}
			SetRequiredFields();
		}

		private void ComboBox_SelectionChangeCommited(object sender,System.EventArgs e) {
			SetRequiredFields();
		}
		
		private void butAddComm_Click(object sender,EventArgs e) {
			//if there is no commlog associated with this 
			if(_commlog==null) {
				_commlog=new Commlog();
				_commlog.CommDateTime=DateTime.Now;
				_commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
				_commlog.UserNum=Security.CurUser.UserNum;
				_commlog.IsNew=true;
			}
			using FormCommItem formCommItem=new FormCommItem(_commlog);
			if(formCommItem.ShowDialog()==DialogResult.OK) {
				//if the commlog was deleted, clear the stored object
				if(_commlog==null || Commlogs.GetOne(_commlog.CommlogNum)==null) {
					_commlog=null;
				}
				//otherwise, update IsNew to prevent saving multiple entries
				else {
					_commlog.IsNew=false;
				}
			}
		}

		#region Names
		private void textLName1_TextChanged(object sender,EventArgs e) {
			if(textLName1.Text.Length==1){
				textLName1.Text=textLName1.Text.ToUpper();
				textLName1.SelectionStart=1;
			}
			SetLnames();
			_lnameOld1=textLName1.Text;
		}

		private void textLName2_TextChanged(object sender,EventArgs e) {
			if(textLName2.Text.Length==1){
				textLName2.Text=textLName2.Text.ToUpper();
				textLName2.SelectionStart=1;
			}
		}

		private void textLName3_TextChanged(object sender,EventArgs e) {
			if(textLName3.Text.Length==1){
				textLName3.Text=textLName3.Text.ToUpper();
				textLName3.SelectionStart=1;
			}
		}

		private void textLName4_TextChanged(object sender,EventArgs e) {
			if(textLName4.Text.Length==1){
				textLName4.Text=textLName4.Text.ToUpper();
				textLName4.SelectionStart=1;
			}
		}

		private void textLName5_TextChanged(object sender,EventArgs e) {
			if(textLName5.Text.Length==1){
				textLName5.Text=textLName5.Text.ToUpper();
				textLName5.SelectionStart=1;
			}
		}

		private void textFName1_TextChanged(object sender,EventArgs e) {
			if(textFName1.Text.Length==1){
				textFName1.Text=textFName1.Text.ToUpper();
				textFName1.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
		}

		private void textFName2_TextChanged(object sender,EventArgs e) {
			if(textFName2.Text.Length==1){
				textFName2.Text=textFName2.Text.ToUpper();
				textFName2.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld2=textFName2.Text;
			_emailOld2=textEmail2.Text;
		}

		private void textFName3_TextChanged(object sender,EventArgs e) {
			if(textFName3.Text.Length==1){
				textFName3.Text=textFName3.Text.ToUpper();
				textFName3.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld3=textFName3.Text;
			_emailOld3=textEmail3.Text;
		}

		private void textFName4_TextChanged(object sender,EventArgs e) {
			if(textFName4.Text.Length==1){
				textFName4.Text=textFName4.Text.ToUpper();
				textFName4.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld4=textFName4.Text;
			_emailOld4=textEmail4.Text;
		}

		private void textFName5_TextChanged(object sender,EventArgs e) {
			if(textFName5.Text.Length==1){
				textFName5.Text=textFName5.Text.ToUpper();
				textFName5.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld5=textFName5.Text;
			_emailOld5=textEmail5.Text;
		}

		private void SetLnames() {
			SetLname(textLName2,textFName2,_fnameOld2);
			SetLname(textLName3,textFName3,_fnameOld3);
			SetLname(textLName4,textFName4,_fnameOld4);
			SetLname(textLName5,textFName5,_fnameOld5);
			ResetSubscriberLists();
		}

		private void SetLname(TextBox textLname,TextBox textFname,string fnameOld) {
			if(textLname.Text=="" && textFname.Text=="") {
				textLname.Text="";
			}
			else if(textLname.Text=="" && textFname.Text!="") {
				textLname.Text=textLName1.Text;
			}
			else if(textLname.Text==_lnameOld1 && textFname.Text=="") {
				if(fnameOld!="" && textFname.Text=="") {
					textLname.Text="";
				}
				else {
					textLname.Text=textLName1.Text;
				}
			}
			else if(textLname.Text==_lnameOld1 && textFname.Text!="") {
				textLname.Text=textLName1.Text;
			}
		}
		#endregion Names

		#region BirthdateAndAge
		private void textBirthdate1_Validated(object sender,EventArgs e) {
			if(!textBirthdate1.IsValid()){
				textAge1.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate1.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge1.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate2_Validated(object sender,EventArgs e) {
			if(!textBirthdate2.IsValid()){
				textAge2.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate2.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge2.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate3_Validated(object sender,EventArgs e) {
			if(!textBirthdate3.IsValid()){
				textAge3.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate3.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge3.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate4_Validated(object sender,EventArgs e) {
			if(!textBirthdate4.IsValid()){
				textAge4.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate4.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge4.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate5_Validated(object sender,EventArgs e) {
			if(!textBirthdate5.IsValid()){
				textAge5.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate5.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge5.Text=PatientLogic.DateToAgeString(birthdate);
		}
		#endregion BirthdateAndAge

		#region InsCheckProvAutomation
		private void checkInsOne1_Click(object sender,EventArgs e) {
			if(textFName2.Text!="" && checkInsOne1.Checked){
				checkInsOne2.Checked=true;
			}
			else{
				checkInsOne2.Checked=false;
			}
			if(textFName3.Text!="" && checkInsOne1.Checked){
				checkInsOne3.Checked=true;
			}
			else{
				checkInsOne3.Checked=false;
			}
			if(textFName4.Text!="" && checkInsOne1.Checked){
				checkInsOne4.Checked=true;
			}
			else{
				checkInsOne4.Checked=false;
			}
			if(textFName5.Text!="" && checkInsOne1.Checked){
				checkInsOne5.Checked=true;
			}
			else{
				checkInsOne5.Checked=false;
			}
		}

		private void checkInsTwo1_Click(object sender,EventArgs e) {
			if(textFName2.Text!="" && checkInsTwo1.Checked){
				checkInsTwo2.Checked=true;
			}
			else{
				checkInsTwo2.Checked=false;
			}
			if(textFName3.Text!="" && checkInsTwo1.Checked){
				checkInsTwo3.Checked=true;
			}
			else{
				checkInsTwo3.Checked=false;
			}
			if(textFName4.Text!="" && checkInsTwo1.Checked){
				checkInsTwo4.Checked=true;
			}
			else{
				checkInsTwo4.Checked=false;
			}
			if(textFName5.Text!="" && checkInsTwo1.Checked){
				checkInsTwo5.Checked=true;
			}
			else{
				checkInsTwo5.Checked=false;
			}
		}

		private void comboPriProv1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboPriProv2.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv3.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv4.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv5.SelectedIndex=comboPriProv1.SelectedIndex;
			SetRequiredFields();
		}

		private void comboSecProv1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboSecProv2.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv3.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv4.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv5.SelectedIndex=comboSecProv1.SelectedIndex;
			SetRequiredFields();
		}

		private void comboBillType1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboBillType2.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType3.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType4.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType5.SelectedIndex=comboBillType1.SelectedIndex;
			SetRequiredFields();
		}
		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(sender==comboClinic1) {
				if(comboClinic1.SelectedClinicNum>-1) {
					comboClinic2.SelectedClinicNum=comboClinic1.SelectedClinicNum;
					comboClinic3.SelectedClinicNum=comboClinic1.SelectedClinicNum;
					comboClinic4.SelectedClinicNum=comboClinic1.SelectedClinicNum;
					comboClinic5.SelectedClinicNum=comboClinic1.SelectedClinicNum;
				}
				//Also pre-select the default billing type for all patients since guarantor's clinic has changed.
				FillComboBillTypes(comboBillType1,comboClinic1.SelectedClinicNum);
				comboBillType2.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType3.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType4.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType5.SelectedIndex=comboBillType1.SelectedIndex;
			}
			else if(sender==comboClinic2) {
				FillComboBillTypes(comboBillType2,comboClinic2.SelectedClinicNum);
			}
			else if(sender==comboClinic3) {
				FillComboBillTypes(comboBillType3,comboClinic3.SelectedClinicNum);
			}
			else if(sender==comboClinic4) {
				FillComboBillTypes(comboBillType4,comboClinic4.SelectedClinicNum);
			}
			else if(sender==comboClinic5) {
				FillComboBillTypes(comboBillType5,comboClinic5.SelectedClinicNum);
			}
		}
		#endregion InsCheckProvAutomation

		#region Email
		
		private void textEmail1_TextChanged(object sender,EventArgs e) {
			SetEmail();
			_emailOld1=textEmail1.Text;
		}

		private void SetEmail() {
			if(PIn.Bool(PrefName.AddFamilyInheritsEmail.ToString())) {
				SetEmail(textEmail2,textFName2,_emailOld2);
				SetEmail(textEmail3,textFName3,_emailOld3);
				SetEmail(textEmail4,textFName4,_emailOld4);
				SetEmail(textEmail5,textFName5,_emailOld5);
			}
		}

		private void SetEmail(TextBox textEmail,TextBox textFname,string emailOld) {
			if(textEmail.Text=="" && textFname.Text=="") {
				textEmail.Text="";
			}
			else if(textEmail.Text=="" && textFname.Text!="") {
				textEmail.Text=textEmail1.Text;
			}
			else if(textEmail.Text==_emailOld1 && textFname.Text=="") {
				if(emailOld!="") {
					textEmail.Text="";
				}
				else {
					textEmail.Text=textEmail1.Text;
				}
			}
			else if(textEmail.Text==_emailOld1 && textFname.Text!="") {
				textEmail.Text=textEmail1.Text;
			}
		}
		#endregion Email

		#region Wireless Phone

		private void textWirelessPhone1_TextChanged(object sender,EventArgs e) {
			SetWirelessPhone();
			_phoneOld1=textWirelessPhone1.Text;
		}

		private void SetWirelessPhone() {
			SetWirelessPhone(textWirelessPhone2,textFName2);
			SetWirelessPhone(textWirelessPhone3,textFName3);
			SetWirelessPhone(textWirelessPhone4,textFName4);
			SetWirelessPhone(textWirelessPhone5,textFName5);
		}

		private void SetWirelessPhone(TextBox textPhone,TextBox textFname) {
			if(textFname.Text=="") {
				textPhone.Text="";
			}
			else if(textPhone.Text=="" || textPhone.Text==_phoneOld1) {
				textPhone.Text=textWirelessPhone1.Text;
			}
		}

		private void listTextOk1_SelectedIndexChanged(object sender,EventArgs e) {
			listTextOk2.SelectedIndex=listTextOk1.SelectedIndex;
			listTextOk3.SelectedIndex=listTextOk1.SelectedIndex;
			listTextOk4.SelectedIndex=listTextOk1.SelectedIndex;
			listTextOk5.SelectedIndex=listTextOk1.SelectedIndex;
			SetRequiredFields();
		}

		#endregion Wireless Phone

		#region SSN
		private void textSSN_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
			TextBox textSSN=(TextBox)sender;
			if(CultureInfo.CurrentCulture.Name != "en-US" || textSSN.Text==""){
				return;
			}
			string formattedSSN;
			if(!Patients.IsValidSSN(textSSN.Text,out formattedSSN)){
				if(MessageBox.Show("SSN not valid. Continue anyway?","",MessageBoxButtons.OKCancel) != DialogResult.OK)	{
					e.Cancel=true;
				}
			}	
			textSSN.Text=formattedSSN;
		}
		
		#endregion

		#region PatientStatus and BillingTypes
		private void FillListPatStatus(ListBoxOD listBox) {
			listBox.Items.Clear();
			listBox.Items.AddEnums<PatientStatus>();
			for(int i=listBox.Items.Count-1;i>=0;i--){//We iterate backwards here so that when the target is removed, we do not run past the loop bounds
				if(listBox.Items.GetTextShowingAt(i) == "Deleted"){
					listBox.Items.RemoveAt(i);
				}
			}
			listBox.SelectedIndex=0;
		}

		private void FillComboBillTypes(UI.ComboBox comboBox,long clinicNum) {
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboBox.Items.Clear();
			long defNum=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,clinicNum));
			for(int i=0;i<_listDefsBillingType.Count;i++){
				comboBox.Items.Add(_listDefsBillingType[i].ItemName);
				if(_listDefsBillingType[i].DefNum==defNum) {
					comboBox.SelectedIndex=i;
				}
			}
		}		
		#endregion

		#region Address

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

		private void textZip_TextChanged(object sender, System.EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender, System.EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			if(comboZip.SelectedIndex==-1) {
				SetRequiredFields();
				return;
			}
			textCity.Text=(_listZipCodes[comboZip.SelectedIndex]).City;
			textState.Text=(_listZipCodes[comboZip.SelectedIndex]).State;
			textZip.Text=(_listZipCodes[comboZip.SelectedIndex]).ZipCodeDigits;
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
				using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
				formZipCodeEdit.ZipCodeCur=zipCode;
				formZipCodeEdit.IsNew=true;
				formZipCodeEdit.ShowDialog();
				if(formZipCodeEdit.DialogResult!=DialogResult.OK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				FillComboZip();
				textCity.Text=zipCode.City;
				textState.Text=zipCode.State;
				textZip.Text=zipCode.ZipCodeDigits;
				return;
			}
			if(listZipCodes.Count==1){
				//only one match found.  Use it.
				textCity.Text=listZipCodes[0].City;
				textState.Text=listZipCodes[0].State;
				return;
			}
			//multiple matches found.  Pick one
			using FormZipSelect formZipSelect=new FormZipSelect();
			formZipSelect.ShowDialog();
			FillComboZip();
			if(formZipSelect.DialogResult!=DialogResult.OK){
				return;
			}
			DataValid.SetInvalid(InvalidType.ZipCodes);
			textCity.Text=formZipSelect.ZipSelected.City;
			textState.Text=formZipSelect.ZipSelected.State;
			textZip.Text=formZipSelect.ZipSelected.ZipCodeDigits;
		}

		private void FillComboZip(){
			comboZip.Items.Clear();
			for(int i=0;i<_listZipCodes.Count;i++){
				comboZip.Items.Add((_listZipCodes[i]).ZipCodeDigits
					+"("+(_listZipCodes[i]).City+")");
			}
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
			listStates.Size=new Size(textState.Width,h);
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
		#endregion Address

		#region Referral
		private void butReferredFrom_Click(object sender,EventArgs e) {
			Referral referral=new Referral();
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?")) {//patient referral
				using FormPatientSelect formPatientSelect=new FormPatientSelect();
				formPatientSelect.IsSelectionModeOnly=true;
				formPatientSelect.ShowDialog();
				if(formPatientSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				referral.PatNum=formPatientSelect.PatNumSelected;
				bool isReferralNew=true;
				Referral referralMatch=Referrals.GetFirstOrDefault(x => x.PatNum==formPatientSelect.PatNumSelected);
				if(referralMatch!=null) {
					referral=referralMatch;
					isReferralNew=false;
				}
				using FormReferralEdit formReferralEdit=new FormReferralEdit(referral);//the ReferralNum must be added here
				formReferralEdit.IsNew=isReferralNew;
				formReferralEdit.ShowDialog();
				if(formReferralEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				referral=formReferralEdit.ReferralCur;//not needed, but it makes it clear that we are editing the ref in FormRefEdit
				_referral=referral;
				FillReferredFrom();
				return;
			}
			//not a patient referral, must be a doctor or marketing/other so show the referral select window with doctor and other check boxes checked
			using FormReferralSelect formReferralSelect=new FormReferralSelect();
			formReferralSelect.IsSelectionMode=true;
			formReferralSelect.IsShowPat=false;
			formReferralSelect.ShowDialog();
			if(formReferralSelect.DialogResult!=DialogResult.OK) {
				if(_referral!=null && _referral.ReferralNum>0) {
					//_refCur.ReferralNum could be invalid if user deleted from FormReferralSelect, _refCur will be null
					_referral=Referrals.GetFromList(_referral.ReferralNum);
					FillReferredFrom();//may have edited a referral and then cancelled attaching to the patient, refill the text box to reflect any changes
				}
				return;
			}
			referral=formReferralSelect.ReferralSelected;
			_referral=referral;
			FillReferredFrom();
		}

		private void butClearReferralSource_Click(object sender,EventArgs e) {
			_referral=null;
			textReferredFrom.Clear();
			SetRequiredFields();
		}

		///<summary>Fills the Referred From text box with the name and referral type from the private classwide variable _refCur.</summary>
		private void FillReferredFrom() {
			string firstRefNameTypeAbbr="";
			string firstRefType="";
			string firstRefFullName="";
			if(_referral==null) {
				textReferredFrom.Clear();
				SetRequiredFields();
				return;
			}
			firstRefFullName=Referrals.GetNameLF(_referral.ReferralNum);
			if(_referral.PatNum>0) {
				firstRefType=" (patient)";
			}
			else if(_referral.IsDoctor) {
				firstRefType=" (doctor)";
			}
			firstRefNameTypeAbbr=firstRefFullName;
			for(int i=1;i<firstRefFullName.Length+1;i++) {//i is used as the length to substring, not an index, so i<firstRefName.Length+1 is safe
				if(TextRenderer.MeasureText(firstRefFullName.Substring(0,i)+firstRefType,textReferredFrom.Font).Width<textReferredFrom.Width) {
					continue;
				}
				firstRefNameTypeAbbr=firstRefFullName.Substring(0,i-1);
				break;
			}
			firstRefNameTypeAbbr+=firstRefType;//firstRefType could be blank, but it will show regardless of the length of firstRefName
			//Example: Schmidt, John Jacob Jingleheimer, DDS (doctor) (+5 more) 
			//might be shortened to : Schmidt, John Jaco (doctor) (+5 more) 
			textReferredFrom.Text=firstRefNameTypeAbbr;//text box might be something like: Schmidt, John Jaco (doctor) (+5 more)
			_toolTipReferred.SetToolTip(textReferredFrom,firstRefFullName+firstRefType);//tooltip will be: Schmidt, John Jacob Jingleheimer, DDS (doctor)
			_errorProvider.SetError(textReferredFrom,"");
		}
		#endregion Referral

		#region Subscriber
		///<summary>Resets the text for each of the six options in the dropdown.  Does this without changing the selected index.</summary>
		private void ResetSubscriberLists(){
			int selectedIndex1=comboSubscriber1.SelectedIndex;
			int selectedIndex2=comboSubscriber2.SelectedIndex;
			comboSubscriber1.Items.Clear();
			comboSubscriber2.Items.Clear();
			comboSubscriber1.Items.Add(Lan.g(this,"none"));
			comboSubscriber2.Items.Add(Lan.g(this,"none"));
			string str;
			for(int i=0;i<5;i++){
				str=(i+1).ToString()+" - ";
				switch(i){
					case 0:
						str+=textLName1.Text+", "+textFName1.Text;
						break;
					case 1:
						str+=textLName2.Text+", "+textFName2.Text;
						break;
					case 2:
						str+=textLName3.Text+", "+textFName3.Text;
						break;
					case 3:
						str+=textLName4.Text+", "+textFName4.Text;
						break;
					case 4:
						str+=textLName5.Text+", "+textFName5.Text;
						break;
				}
				comboSubscriber1.Items.Add(str);
				comboSubscriber2.Items.Add(str);
			}
			if(selectedIndex1==-1){
				comboSubscriber1.SelectedIndex=0;
			}
			else{
				comboSubscriber1.SelectedIndex=selectedIndex1;
			}
			if(selectedIndex2==-1){
				comboSubscriber2.SelectedIndex=0;
			}
			else{
				comboSubscriber2.SelectedIndex=selectedIndex2;
			}
		}
		#endregion Subscriber

		#region Employer
		private void textEmployer1_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				return;
			}
			if(e.KeyCode==Keys.Return) {
				listEmps1.Visible=false;
				textCarrier1.Focus();
				return;
			}
			if(textEmployer1.Text=="") {
				listEmps1.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listEmps1.Items.Count==0) {
					return;
				}
				if(listEmps1.SelectedIndex==-1) {
					listEmps1.SelectedIndex=0;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				else if(listEmps1.SelectedIndex==listEmps1.Items.Count-1) {
					listEmps1.SelectedIndex=-1;
					textEmployer1.Text=_empOriginal1;
				}
				else {
					listEmps1.SelectedIndex++;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				textEmployer1.SelectionStart=textEmployer1.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listEmps1.Items.Count==0) {
					return;
				}
				if(listEmps1.SelectedIndex==-1) {
					listEmps1.SelectedIndex=listEmps1.Items.Count-1;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				else if(listEmps1.SelectedIndex==0) {
					listEmps1.SelectedIndex=-1;
					textEmployer1.Text=_empOriginal1;
				}
				else {
					listEmps1.SelectedIndex--;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				textEmployer1.SelectionStart=textEmployer1.Text.Length;
				return;
			}
			if(textEmployer1.Text.Length==1) {
				textEmployer1.Text=textEmployer1.Text.ToUpper();
				textEmployer1.SelectionStart=1;
			}
			_empOriginal1=textEmployer1.Text;//the original text is preserved when using up and down arrows
			listEmps1.Items.Clear();
			List<Employer> listEmployersSimilar=Employers.GetSimilarNames(textEmployer1.Text);
			for(int i=0;i<listEmployersSimilar.Count;i++) {
				listEmps1.Items.Add(listEmployersSimilar[i].EmpName);
			}
			int h=13*listEmployersSimilar.Count+5;
			if(h > ClientSize.Height-listEmps1.Top){
				h=ClientSize.Height-listEmps1.Top;
			}
			LayoutManager.MoveSize(listEmps1,new Size(231,h));
			listEmps1.Visible=true;
		}

		private void textEmployer1_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListEmps1) {
				return;
			}
			listEmps1.Visible=false;
			SetRequiredFields();
		}

		private void listEmps1_Click(object sender,System.EventArgs e) {
			textEmployer1.Text=listEmps1.SelectedItem.ToString();
			textEmployer1.Focus();
			textEmployer1.SelectionStart=textEmployer1.Text.Length;
			listEmps1.Visible=false;
		}

		private void listEmps1_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
			textEmployer1.Text=listEmps1.SelectedItem.ToString();
			textEmployer1.Focus();
			textEmployer1.SelectionStart=textEmployer1.Text.Length;
			listEmps1.Visible=false;
		}

		private void listEmps1_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListEmps1=true;
		}

		private void listEmps1_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListEmps1=false;
		}

		private void textEmployer2_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				return;
			}
			if(e.KeyCode==Keys.Return) {
				listEmps2.Visible=false;
				textCarrier2.Focus();
				return;
			}
			if(textEmployer2.Text=="") {
				listEmps2.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listEmps2.Items.Count==0) {
					return;
				}
				if(listEmps2.SelectedIndex==-1) {
					listEmps2.SelectedIndex=0;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				else if(listEmps2.SelectedIndex==listEmps2.Items.Count-1) {
					listEmps2.SelectedIndex=-1;
					textEmployer2.Text=_empOriginal2;
				}
				else {
					listEmps2.SelectedIndex++;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				textEmployer2.SelectionStart=textEmployer2.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listEmps2.Items.Count==0) {
					return;
				}
				if(listEmps2.SelectedIndex==-1) {
					listEmps2.SelectedIndex=listEmps2.Items.Count-1;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				else if(listEmps2.SelectedIndex==0) {
					listEmps2.SelectedIndex=-1;
					textEmployer2.Text=_empOriginal2;
				}
				else {
					listEmps2.SelectedIndex--;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				textEmployer2.SelectionStart=textEmployer2.Text.Length;
				return;
			}
			if(textEmployer2.Text.Length==1) {
				textEmployer2.Text=textEmployer2.Text.ToUpper();
				textEmployer2.SelectionStart=1;
			}
			_empOriginal2=textEmployer2.Text;//the original text is preserved when using up and down arrows
			listEmps2.Items.Clear();
			List<Employer> listEmployersSimilar2=Employers.GetSimilarNames(textEmployer2.Text);
			for(int i=0;i<listEmployersSimilar2.Count;i++) {
				listEmps2.Items.Add(listEmployersSimilar2[i].EmpName);
			}
			int h=13*listEmployersSimilar2.Count+5;
			if(h > ClientSize.Height-listEmps2.Top){
				h=ClientSize.Height-listEmps2.Top;
			}
			LayoutManager.MoveSize(listEmps2,new Size(231,h));
			listEmps2.Visible=true;
		}

		private void textEmployer2_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListEmps2) {
				return;
			}
			listEmps2.Visible=false;
		}

		private void listEmps2_Click(object sender,System.EventArgs e) {
			textEmployer2.Text=listEmps2.SelectedItem.ToString();
			textEmployer2.Focus();
			textEmployer2.SelectionStart=textEmployer2.Text.Length;
			listEmps2.Visible=false;
		}

		private void listEmps2_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
			textEmployer2.Text=listEmps2.SelectedItem.ToString();
			textEmployer2.Focus();
			textEmployer2.SelectionStart=textEmployer2.Text.Length;
			listEmps2.Visible=false;
		}

		private void listEmps2_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListEmps2=true;
		}

		private void listEmps2_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListEmps2=false;
		}
		#endregion Employer

		#region Carrier
		///<summary>Fills the carrier fields on the form based on the specified carrierNum.</summary>
		private void FillCarrier1(long carrierNum) {
			_carrierSelected1=Carriers.GetCarrier(carrierNum);
			textCarrier1.Text=_carrierSelected1.CarrierName;
			textPhone1.Text=_carrierSelected1.Phone;
		}

		private void textCarrier1_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				return;
			}
			if(e.KeyCode==Keys.Return) {
				if(listCars1.SelectedIndex==-1) {
					textPhone1.Focus();
				}
				else {
					FillCarrier1(_listCarriersSimilar1[listCars1.SelectedIndex].CarrierNum);
					textCarrier1.Focus();
					textCarrier1.SelectionStart=textCarrier1.Text.Length;
				}
				listCars1.Visible=false;
				return;
			}
			if(textCarrier1.Text=="") {
				listCars1.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listCars1.Items.Count==0) {
					return;
				}
				if(listCars1.SelectedIndex==-1) {
					listCars1.SelectedIndex=0;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				else if(listCars1.SelectedIndex==listCars1.Items.Count-1) {
					listCars1.SelectedIndex=-1;
					textCarrier1.Text=_carOriginal1;
				}
				else {
					listCars1.SelectedIndex++;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				textCarrier1.SelectionStart=textCarrier1.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listCars1.Items.Count==0) {
					return;
				}
				if(listCars1.SelectedIndex==-1) {
					listCars1.SelectedIndex=listCars1.Items.Count-1;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				else if(listCars1.SelectedIndex==0) {
					listCars1.SelectedIndex=-1;
					textCarrier1.Text=_carOriginal1;
				}
				else {
					listCars1.SelectedIndex--;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				textCarrier1.SelectionStart=textCarrier1.Text.Length;
				return;
			}
			if(textCarrier1.Text.Length==1) {
				textCarrier1.Text=textCarrier1.Text.ToUpper();
				textCarrier1.SelectionStart=1;
			}
			_carOriginal1=textCarrier1.Text;//the original text is preserved when using up and down arrows
			listCars1.Items.Clear();
			_listCarriersSimilar1=Carriers.GetSimilarNames(textCarrier1.Text);
			for(int i=0;i<_listCarriersSimilar1.Count;i++) {
				listCars1.Items.Add(_listCarriersSimilar1[i].CarrierName+", "
					+_listCarriersSimilar1[i].Phone+", "
					+_listCarriersSimilar1[i].Address+", "
					+_listCarriersSimilar1[i].Address2+", "
					+_listCarriersSimilar1[i].City+", "
					+_listCarriersSimilar1[i].State+", "
					+_listCarriersSimilar1[i].Zip);
			}
			int h=13*_listCarriersSimilar1.Count+5;
			if(h > ClientSize.Height-listCars1.Top){
				h=ClientSize.Height-listCars1.Top;
			}
			listCars1.Size=new Size(listCars1.Width,h);
			listCars1.Visible=true;
		}

		private void textCarrier1_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListCars1) {
				return;
			}
			//or if user clicked on a different text box.
			if(listCars1.SelectedIndex!=-1) {
				FillCarrier1(_listCarriersSimilar1[listCars1.SelectedIndex].CarrierNum);
			}
			listCars1.Visible=false;
		}

		private void listCars1_Click(object sender,System.EventArgs e) {
			FillCarrier1(_listCarriersSimilar1[listCars1.SelectedIndex].CarrierNum);
			textCarrier1.Focus();
			textCarrier1.SelectionStart=textCarrier1.Text.Length;
			listCars1.Visible=false;
		}

		private void listCars1_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
		}

		private void listCars1_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListCars1=true;
		}

		private void listCars1_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListCars1=false;
		}

		///<summary>Fills the carrier fields on the form based on the specified carrierNum.</summary>
		private void FillCarrier2(long carrierNum) {
			_carrierSelected2=Carriers.GetCarrier(carrierNum);
			textCarrier2.Text=_carrierSelected2.CarrierName;
			textPhone2.Text=_carrierSelected2.Phone;
		}

		private void textCarrier2_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				return;
			}
			if(e.KeyCode==Keys.Return) {
				if(listCars2.SelectedIndex==-1) {
					textPhone2.Focus();
				}
				else {
					FillCarrier2(_listCarriersSimilar2[listCars2.SelectedIndex].CarrierNum);
					textCarrier2.Focus();
					textCarrier2.SelectionStart=textCarrier2.Text.Length;
				}
				listCars2.Visible=false;
				return;
			}
			if(textCarrier2.Text=="") {
				listCars2.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listCars2.Items.Count==0) {
					return;
				}
				if(listCars2.SelectedIndex==-1) {
					listCars2.SelectedIndex=0;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				else if(listCars2.SelectedIndex==listCars2.Items.Count-1) {
					listCars2.SelectedIndex=-1;
					textCarrier2.Text=_carOriginal2;
				}
				else {
					listCars2.SelectedIndex++;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				textCarrier2.SelectionStart=textCarrier2.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listCars2.Items.Count==0) {
					return;
				}
				if(listCars2.SelectedIndex==-1) {
					listCars2.SelectedIndex=listCars2.Items.Count-1;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				else if(listCars2.SelectedIndex==0) {
					listCars2.SelectedIndex=-1;
					textCarrier2.Text=_carOriginal2;
				}
				else {
					listCars2.SelectedIndex--;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				textCarrier2.SelectionStart=textCarrier2.Text.Length;
				return;
			}
			if(textCarrier2.Text.Length==1) {
				textCarrier2.Text=textCarrier2.Text.ToUpper();
				textCarrier2.SelectionStart=1;
			}
			_carOriginal2=textCarrier2.Text;//the original text is preserved when using up and down arrows
			listCars2.Items.Clear();
			_listCarriersSimilar2=Carriers.GetSimilarNames(textCarrier2.Text);
			for(int i=0;i<_listCarriersSimilar2.Count;i++) {
				listCars2.Items.Add(_listCarriersSimilar2[i].CarrierName+", "
					+_listCarriersSimilar2[i].Phone+", "
					+_listCarriersSimilar2[i].Address+", "
					+_listCarriersSimilar2[i].Address2+", "
					+_listCarriersSimilar2[i].City+", "
					+_listCarriersSimilar2[i].State+", "
					+_listCarriersSimilar2[i].Zip);
			}
			int h=13*_listCarriersSimilar2.Count+5;
			if(h > ClientSize.Height-listCars2.Top){
				h=ClientSize.Height-listCars2.Top;
			}
			listCars2.Size=new Size(listCars2.Width,h);
			listCars2.Visible=true;
		}

		private void textCarrier2_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListCars2) {
				return;
			}
			//or if user clicked on a different text box.
			if(listCars2.SelectedIndex!=-1) {
				FillCarrier2(_listCarriersSimilar2[listCars2.SelectedIndex].CarrierNum);
			}
			listCars2.Visible=false;
		}

		private void listCars2_Click(object sender,System.EventArgs e) {
			FillCarrier2(_listCarriersSimilar2[listCars2.SelectedIndex].CarrierNum);
			textCarrier2.Focus();
			textCarrier2.SelectionStart=textCarrier2.Text.Length;
			listCars2.Visible=false;
		}

		private void listCars2_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
		}

		private void listCars2_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListCars2=true;
		}

		private void listCars2_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListCars2=false;
		}
		#endregion Carrier

		#region InsPlanPick
		private void butPick1_Click(object sender,EventArgs e) {
			using FormInsPlans formInsPlans=new FormInsPlans();
			formInsPlans.empText=textEmployer1.Text;
			formInsPlans.carrierText=textCarrier1.Text;
			formInsPlans.IsSelectMode=true;
			formInsPlans.ShowDialog();
			if(formInsPlans.DialogResult==DialogResult.Cancel) {
				return;
			}
			_insPlanSelected1=formInsPlans.InsPlanSelected.Copy();
			//Non-synched fields:
			//selectedPlan1.SubscriberID=textSubscriberID.Text;//later
			//selectedPlan1.DateEffective=DateTime.MinValue;
			//selectedPlan1.DateTerm=DateTime.MinValue;
			//PlanCur.ReleaseInfo=checkRelease.Checked;
			//PlanCur.AssignBen=checkAssign.Checked;
			//PlanCur.SubscNote=textSubscNote.Text;
			//Benefits will be created when click OK.
			textEmployer1.Text=Employers.GetName(_insPlanSelected1.EmployerNum);
			FillCarrier1(_insPlanSelected1.CarrierNum);
			textGroupName1.Text=_insPlanSelected1.GroupName;
			textGroupNum1.Text=_insPlanSelected1.GroupNum;
			SetRequiredFields();
		}

		private void butPick2_Click(object sender,EventArgs e) {
			using FormInsPlans formInsPlans=new FormInsPlans();
			formInsPlans.empText=textEmployer2.Text;
			formInsPlans.carrierText=textCarrier2.Text;
			formInsPlans.IsSelectMode=true;
			formInsPlans.ShowDialog();
			if(formInsPlans.DialogResult==DialogResult.Cancel) {
				return;
			}
			_insPlanSelected2=formInsPlans.InsPlanSelected.Copy();
			//Non-synched fields:
			//selectedPlan2.SubscriberID=textSubscriberID.Text;//later
			//selectedPlan2.DateEffective=DateTime.MinValue;
			//selectedPlan2.DateTerm=DateTime.MinValue;
			//PlanCur.ReleaseInfo=checkRelease.Checked;
			//PlanCur.AssignBen=checkAssign.Checked;
			//PlanCur.SubscNote=textSubscNote.Text;
			//Benefits will be created when click OK.
			textEmployer2.Text=Employers.GetName(_insPlanSelected2.EmployerNum);
			FillCarrier2(_insPlanSelected2.CarrierNum);
			textGroupName2.Text=_insPlanSelected2.GroupName;
			textGroupNum2.Text=_insPlanSelected2.GroupNum;
		}
		#endregion InsPlanPick
		
		///<summary>Adds a new patient to the passed-in list based on passed-in strings.</summary>
		private void AddPatToList(string lname, string fname,string birthday,long clinicNum,List<Patient> listPatients) {
			if(lname=="" && fname=="") { //validation should prevent this from happening, but just in case.
				return; //dont add patient to list
			}
			Patient patient=new Patient();
			patient.LName=lname;
			patient.FName=fname;
			patient.ClinicNum=clinicNum;
			if(birthday!=""){
				patient.Birthdate=PIn.Date(birthday);
			}
			listPatients.Add(patient);
		}

		///<summary>Gets the clinic num from the corresponding comboClinic using the passed in index, which matches the order of the Patients in the form.</summary>
		private long getClinicNum(long index) {
			long clinicNum;
			switch(index) {
				case 0://guarantor
					clinicNum=comboClinic1.SelectedClinicNum;
					break;
				case 1:
					clinicNum=comboClinic2.SelectedClinicNum;
					break;
				case 2:
					clinicNum=comboClinic3.SelectedClinicNum;
					break;
				case 3:
					clinicNum=comboClinic4.SelectedClinicNum;
					break;
				case 4:
					clinicNum=comboClinic5.SelectedClinicNum;
					break;
				default:
					clinicNum=comboClinic1.SelectedClinicNum;
					break;
			}
			return clinicNum;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormPatientAddAll.butOK_Click_start")) {
				return;
			}
			#region Validation		
			if(!textBirthdate1.IsValid()
				|| !textBirthdate2.IsValid()
				|| !textBirthdate3.IsValid()
				|| !textBirthdate4.IsValid()
				|| !textBirthdate5.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//no validation on birthdate reasonableness.
			if(textLName1.Text=="" || textFName1.Text==""){
				MsgBox.Show(this,"Guarantor name must be entered.");
				return;
			}
			if(PrefC.HasClinicsEnabled
				&& (comboClinic1.IsNothingSelected || comboClinic2.IsNothingSelected
				|| comboClinic3.IsNothingSelected ||comboClinic4.IsNothingSelected
				|| comboClinic5.IsNothingSelected)) 
			{
				MsgBox.Show(this,"Valid clinic must be selected.");
				return;
			}
			#region Validate Insurance Subscribers
			if((comboSubscriber1.SelectedIndex==2 || comboSubscriber2.SelectedIndex==2) && (textFName2.Text=="" || textLName2.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==3 || comboSubscriber2.SelectedIndex==3) && (textFName3.Text=="" || textLName3.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==4 || comboSubscriber2.SelectedIndex==4) && (textFName4.Text=="" || textLName4.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==5 || comboSubscriber2.SelectedIndex==5) && (textFName5.Text=="" || textLName5.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			#endregion Validate Insurance Subscribers
			#region Validate Insurance Plans
			bool isInsComplete1=false;
			bool isInsComplete2=false;
			if(comboSubscriber1.SelectedIndex>0
				&& textSubscriberID1.Text!=""
				&& textCarrier1.Text!="")
			{
				isInsComplete1=true;
			}
			if(comboSubscriber2.SelectedIndex>0
				&& textSubscriberID2.Text!=""
				&& textCarrier2.Text!="")
			{
				isInsComplete2=true;
			}
			//test for insurance having only some of the critical fields filled in
			if(comboSubscriber1.SelectedIndex>0
				|| textSubscriberID1.Text!=""
				|| textCarrier1.Text!="")
			{
				if(!isInsComplete1) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(comboSubscriber2.SelectedIndex>0
				|| textSubscriberID2.Text!=""
				|| textCarrier2.Text!="")
			{
				if(!isInsComplete2) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(checkInsOne1.Checked
				|| checkInsOne2.Checked
				|| checkInsOne3.Checked
				|| checkInsOne4.Checked
				|| checkInsOne5.Checked)
			{
				if(!isInsComplete1) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(checkInsTwo1.Checked
				|| checkInsTwo2.Checked
				|| checkInsTwo3.Checked
				|| checkInsTwo4.Checked
				|| checkInsTwo5.Checked)
			{
				if(!isInsComplete2) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			#endregion Validate Insurance Plans
			#region Validate Insurance Subscriptions
			if(isInsComplete1) {
				if(!checkInsOne1.Checked
					&& !checkInsOne2.Checked
					&& !checkInsOne3.Checked
					&& !checkInsOne4.Checked
					&& !checkInsOne5.Checked)
				{
					MsgBox.Show(this,"Insurance information has been filled in, but has not been assigned to any patients.");
					return;
				}
				if(checkInsOne1.Checked && (textLName1.Text=="" || textFName1.Text=="")//Insurance1 assigned to invalid patient1
					|| checkInsOne2.Checked && (textLName2.Text=="" || textFName2.Text=="")//Insurance1 assigned to invalid patient2
					|| checkInsOne3.Checked && (textLName3.Text=="" || textFName3.Text=="")//Insurance1 assigned to invalid patient3
					|| checkInsOne4.Checked && (textLName4.Text=="" || textFName4.Text=="")//Insurance1 assigned to invalid patient4
					|| checkInsOne5.Checked && (textLName5.Text=="" || textFName5.Text=="")) //Insurance1 assigned to invalid patient5
				{
					MsgBox.Show(this,"Insurance information 1 has been filled in, but has been assigned to a patient with no name.");
					return;
				}
			}
			if(isInsComplete2) {
				if(!checkInsTwo1.Checked
					&& !checkInsTwo2.Checked
					&& !checkInsTwo3.Checked
					&& !checkInsTwo4.Checked
					&& !checkInsTwo5.Checked) {
					MsgBox.Show(this,"Insurance information 2 has been filled in, but has not been assigned to any patients.");
					return;
				}
				if(checkInsTwo1.Checked && (textLName1.Text=="" || textFName1.Text=="")//Insurance2 assigned to invalid patient1
					|| checkInsTwo2.Checked && (textLName2.Text=="" || textFName2.Text=="")//Insurance2 assigned to invalid patient2
					|| checkInsTwo3.Checked && (textLName3.Text=="" || textFName3.Text=="")//Insurance2 assigned to invalid patient3
					|| checkInsTwo4.Checked && (textLName4.Text=="" || textFName4.Text=="")//Insurance2 assigned to invalid patient4
					|| checkInsTwo5.Checked && (textLName5.Text=="" || textFName5.Text=="")) //Insurance2 assigned to invalid patient5
				{
					MsgBox.Show(this,"Insurance information 2 has been filled in, but has been assigned to a patient with no name.");
					return;
				}
			}
			#endregion Validate Insurance Subscriptions
			#region Validate Clinics
			if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters)) {
				if((comboClinic1.SelectedClinicNum==0 && textLName1.Text!="" && textFName1.Text!="")
					|| (comboClinic2.SelectedClinicNum==0 && textLName2.Text!="" && textFName2.Text!="")
					|| (comboClinic3.SelectedClinicNum==0 && textLName3.Text!="" && textFName3.Text!="")
					|| (comboClinic4.SelectedClinicNum==0 && textLName4.Text!="" && textFName4.Text!="")
					|| (comboClinic5.SelectedClinicNum==0 && textLName5.Text!="" && textFName5.Text!="")) 
				{
					MsgBox.Show(this,"Current settings for clinics do not allow patients to be added to the 'Unassigned' clinic. Please select a clinic.");
					return;
				}
			}
			#endregion
			if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				if((comboPriProv1.SelectedIndex==0 && textLName1.Text!="" && textFName1.Text!="")//Patient has 'Select Provider' as Primary Provider
					|| (comboPriProv2.SelectedIndex==0 && textLName2.Text!="" && textFName2.Text!="")
					|| (comboPriProv3.SelectedIndex==0 && textLName3.Text!="" && textFName3.Text!="")
					|| (comboPriProv4.SelectedIndex==0 && textLName4.Text!="" && textFName4.Text!="")
					|| (comboPriProv5.SelectedIndex==0 && textLName5.Text!="" && textFName5.Text!="")) 
				{
					MsgBox.Show(this,"Primary provider must be set.");
					return;
				}
			}
			bool hasSavedMissingFields=false;
			if(_isMissingRequiredFields) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Required fields are missing or incorrect.  Click OK to save anyway or Cancel to return and "
						+"finish editing patient information.")) {
					_isValidating=true;
					SetRequiredFields();
					return;
				}
				else {
					hasSavedMissingFields=true;
					//Will make an audit trail further down once we know the guarantor's PatNum
				}
			}
			//check for duplicate patients
			List<Patient> listPatientsAdding = new List<Patient>();
			AddPatToList(textLName1.Text.Trim(),textFName1.Text.Trim(),textBirthdate1.Text,comboClinic1.SelectedClinicNum,listPatientsAdding);
			AddPatToList(textLName2.Text.Trim(),textFName2.Text.Trim(),textBirthdate2.Text,comboClinic2.SelectedClinicNum,listPatientsAdding);
			AddPatToList(textLName3.Text.Trim(),textFName3.Text.Trim(),textBirthdate3.Text,comboClinic3.SelectedClinicNum,listPatientsAdding);
			AddPatToList(textLName4.Text.Trim(),textFName4.Text.Trim(),textBirthdate4.Text,comboClinic4.SelectedClinicNum,listPatientsAdding);
			AddPatToList(textLName5.Text.Trim(),textFName5.Text.Trim(),textBirthdate5.Text,comboClinic5.SelectedClinicNum,listPatientsAdding);
			for(int i=0;i<listPatientsAdding.Count;i++) {//Check all the patients that we're trying to add.
				//get a list of all current patients that have the same name. PatNum here will be 0 which is fine.
				List<Patient> listPatients = Patients.GetListByName(listPatientsAdding[i].LName,listPatientsAdding[i].FName,listPatientsAdding[i].PatNum);
				for(int j=0;j<listPatients.Count;j++) {
					//If dates match or aren't entered there might be a duplicate patient.
					if(listPatients[j].Birthdate==listPatientsAdding[i].Birthdate
						|| listPatients[j].Birthdate.Year<1880
						|| listPatientsAdding[i].Birthdate.Year<1880) 
					{
						string msgText=Lan.g(this,"Patient")+" '"+listPatientsAdding[i].LName+", "+listPatientsAdding[i].FName+"' "
							+Lan.g(this,"may already exist. Continue anyway?");
						if(MessageBox.Show(msgText,"Potential Duplicate Patient",MessageBoxButtons.OKCancel) != DialogResult.OK) {
							return;
						}
						break;
					}
				}
			}
			#endregion Validation
			#region Create Family
			Patient[] patientArrayInFam=new Patient[5];
			Patient patient=new Patient();
			patient.PatStatus=PatientStatus.Patient;
			patient.HmPhone=textHmPhone.Text;
			patient.Address=textAddress.Text;
			patient.Address2=textAddress2.Text;
			patient.City=textCity.Text;
			patient.State=textState.Text.Trim();
			patient.Country=textCountry.Text;
			patient.Zip=textZip.Text.Trim();
			patient.AddrNote=textAddrNotes.Text;
			RefAttach refAttach=new RefAttach();
			if(_referral!=null) {
				refAttach.ReferralNum=_referral.ReferralNum;
				refAttach.RefType=ReferralType.RefFrom;
				refAttach.RefDate=DateTime.Today;
				if(_referral.IsDoctor) {//whether using ehr or not
					refAttach.IsTransitionOfCare=true;
				}
				refAttach.ItemOrder=1;
			}
			for(int i=0;i<patientArrayInFam.Length;i++) {
				//this is just in case, since we are using the same Patient object for every family member inserted
				//probably not necessary since inserting will assign a new PatNum
				patient.PatNum=0;
				patient.ImageFolder=null; //Need to reset image folder to null so we can overwrite for next patient.
				if(PrefC.HasClinicsEnabled) {
					patient.ClinicNum=getClinicNum(i);//Assign clinic num for the currently iterated patient if clinics is enabled.
				}
				switch(i) {
					case 0://guarantor
						patient.LName=textLName1.Text;
						patient.FName=textFName1.Text;
						if(listGender1.SelectedIndex==-1) { 
							listGender1.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender1.GetSelected<PatientGender>();
						patient.Position=(PatientPosition)listPosition1.SelectedIndex;
						patient.Birthdate=PIn.Date(textBirthdate1.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv1.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv1.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv1.SelectedIndex].ProvNum;
						}
						if(comboSecProv1.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv1.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN1.Text);
						patient.Email=textEmail1.Text;
						patient.WirelessPhone=textWirelessPhone1.Text;
						patient.TxtMsgOk=(YN)listTextOk1.SelectedIndex;
						patient.PatStatus=listStatus1.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType1.SelectedIndex].DefNum;
						break;
					case 1://patient 2
						if(textFName2.Text=="" || textLName2.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName2.Text;
						patient.FName=textFName2.Text;
						if(listGender2.SelectedIndex==-1) { 
							listGender2.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender2.GetSelected<PatientGender>();
						patient.Position=(PatientPosition)listPosition2.SelectedIndex;
						patient.Birthdate=PIn.Date(textBirthdate2.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv2.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv2.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv2.SelectedIndex].ProvNum;
						}
						if(comboSecProv2.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv2.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN2.Text);
						patient.Email=textEmail2.Text;
						patient.WirelessPhone=textWirelessPhone2.Text;
						patient.TxtMsgOk=(YN)listTextOk2.SelectedIndex;
						patient.PatStatus=listStatus2.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType2.SelectedIndex].DefNum;
						break;
					case 2://patient 3
						if(textFName3.Text=="" || textLName3.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName3.Text;
						patient.FName=textFName3.Text;
						if(listGender3.SelectedIndex==-1) { 
							listGender3.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender3.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate3.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv3.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv3.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv3.SelectedIndex].ProvNum;
						}
						if(comboSecProv3.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv3.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN3.Text);
						patient.Email=textEmail3.Text;
						patient.WirelessPhone=textWirelessPhone3.Text;
						patient.TxtMsgOk=(YN)listTextOk3.SelectedIndex;
						patient.PatStatus=listStatus3.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType3.SelectedIndex].DefNum;
						break;
					case 3://patient 4
						if(textFName4.Text=="" || textLName4.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName4.Text;
						patient.FName=textFName4.Text;
						if(listGender4.SelectedIndex==-1) { 
							listGender4.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender4.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate4.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv4.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv4.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv4.SelectedIndex].ProvNum;
						}
						if(comboSecProv4.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv4.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN4.Text);
						patient.Email=textEmail4.Text;
						patient.WirelessPhone=textWirelessPhone4.Text;
						patient.TxtMsgOk=(YN)listTextOk4.SelectedIndex;
						patient.PatStatus=listStatus4.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType4.SelectedIndex].DefNum;
						break;
					case 4://patient 5
						if(textFName5.Text=="" || textLName5.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName5.Text;
						patient.FName=textFName5.Text;
						if(listGender5.SelectedIndex==-1) { 
							listGender5.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender5.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate5.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv5.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv5.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv5.SelectedIndex].ProvNum;
						}
						if(comboSecProv5.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv5.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN5.Text);
						patient.Email=textEmail5.Text;
						patient.WirelessPhone=textWirelessPhone5.Text;
						patient.TxtMsgOk=(YN)listTextOk5.SelectedIndex;
						patient.PatStatus=listStatus5.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType5.SelectedIndex].DefNum;
						break;
				}
				Patients.Insert(patient,false);
				ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				SecurityLogs.MakeLogEntry(Permissions.PatientCreate,patient.PatNum,"Created from Add Family window.");
				//if this is the first family member it is the guarantor, so set pat.Guarantor=pat.PatNum and update
				//if this is not the first family member, the guarantor has been inserted and pat.Guarantor will already be set before inserting
				if(i==0) {
					Patient patientOld=patient.Copy();
					patient.Guarantor=patient.PatNum;
					Patients.Update(patient,patientOld);
					//if this is the guarantor, also check if there is a commlog to be saved
					if(_commlog != null && _commlog.CommlogNum>0) {
						_commlog.PatNum=patient.PatNum;
						Commlogs.Update(_commlog);
					}
				}
				patientArrayInFam[i]=patient.Copy();//add patient to local patient array of family members, arrayPatsInFam[0] will be the guarantor
				if(_referral!=null) {
					refAttach.PatNum=patient.PatNum;
					RefAttaches.Insert(refAttach);
					SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,patient.PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
				}
				CustReference custReference=new CustReference();
				custReference.PatNum=patient.PatNum;
				CustReferences.Insert(custReference);
			}
			#endregion Create Family
			#region Insurance
			InsSub insSub1=null;
			InsSub insSub2=null;
			List<CovCat> listCovCats=CovCats.GetWhere(x => x.DefaultPercent!=-1,true);
			#region Validate Plans
			//validate the ins fields.  If they don't match perfectly, then set the selected plan to null
			if(_insPlanSelected1!=null) {
				if(Employers.GetName(_insPlanSelected1.EmployerNum)!=textEmployer1.Text
					|| Carriers.GetName(_insPlanSelected1.CarrierNum)!=textCarrier1.Text
					|| _insPlanSelected1.GroupName!=textGroupName1.Text
					|| _insPlanSelected1.GroupNum!=textGroupNum1.Text)
				{
					_insPlanSelected1=null;
				}
			}
			if(_insPlanSelected2!=null) {
				if(Employers.GetName(_insPlanSelected2.EmployerNum)!=textEmployer2.Text
					|| Carriers.GetName(_insPlanSelected2.CarrierNum)!=textCarrier2.Text
					|| _insPlanSelected2.GroupName!=textGroupName2.Text
					|| _insPlanSelected2.GroupNum!=textGroupNum2.Text)
				{
					_insPlanSelected2=null;
				}
			}
			//validate the carrier fields.  If they don't match perfectly, then set the selected plan to null
			if(_carrierSelected1!=null) {
				if(_carrierSelected1.CarrierName!=textCarrier1.Text || _carrierSelected1.Phone!=textPhone1.Text) {
					_carrierSelected1=null;
				}
			}
			if(_carrierSelected2!=null) {
				if(_carrierSelected2.CarrierName!=textCarrier2.Text || _carrierSelected2.Phone!=textPhone2.Text) {
					_carrierSelected2=null;
				}
			}
			#endregion Validate Plans
			#region Insert InsPlans, Benefits, and InsSubs
			#region Primary Ins
			if(isInsComplete1) {
				if(_carrierSelected1==null) {
					//get a carrier, possibly creating a new one if needed.
					_carrierSelected1=Carriers.GetByNameAndPhone(textCarrier1.Text,textPhone1.Text);
				}
				if(_insPlanSelected1==null) {
					//don't try to get a copy of an existing plan. Instead, start from scratch.
					_insPlanSelected1=new InsPlan();
					_insPlanSelected1.EmployerNum=Employers.GetEmployerNum(textEmployer1.Text);
					_insPlanSelected1.CarrierNum=_carrierSelected1.CarrierNum;
					_insPlanSelected1.GroupName=textGroupName1.Text;
					_insPlanSelected1.GroupNum=textGroupNum1.Text;
					_insPlanSelected1.PlanType="";
					if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
						_insPlanSelected1.PlanType="p";
					}
					if(_insPlanSelected1.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {
						_insPlanSelected1.ClaimsUseUCR=true;
					}
					_insPlanSelected1.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
					InsPlans.Insert(_insPlanSelected1);
					Benefit benefit=new Benefit();
					benefit.PlanNum=_insPlanSelected1.PlanNum;//same for all benefits inserted
					benefit.BenefitType=InsBenefitType.CoInsurance;//same for all benefits inserted from CovCats.ListShort
					benefit.MonetaryAmt=-1;//same for all benefits inserted from CovCats.ListShort
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;//same for all benefits inserted
					benefit.CodeNum=0;//same for all benefits inserted
					benefit.CoverageLevel=BenefitCoverageLevel.None;//same for all benefits inserted from CovCats.ListShort
					for(int i=0;i<listCovCats.Count;i++) {
						benefit.CovCatNum=listCovCats[i].CovCatNum;
						benefit.Percent=listCovCats[i].DefaultPercent;
						Benefits.Insert(benefit);
					}
					benefit.BenefitType=InsBenefitType.Deductible;//same for Diagnostic and RoutinePreventive
					benefit.Percent=-1;//same for Diagnostic and RoutinePreventive
					benefit.MonetaryAmt=0;//same for Diagnostic and RoutinePreventive
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;//same for Diagnostic and RoutinePreventive
					//Zero deductible diagnostic
					if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						Benefits.Insert(benefit);
					}
					//Zero deductible preventive
					if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						Benefits.Insert(benefit);
					}
				}
				insSub1=new InsSub();
				insSub1.PlanNum=_insPlanSelected1.PlanNum;
				insSub1.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub1.ReleaseInfo=true;
				insSub1.DateEffective=DateTime.MinValue;
				insSub1.DateTerm=DateTime.MinValue;
				if(comboSubscriber1.SelectedIndex>0) {//comboSubscriber has been validated to contain the same number of indexes as the family list
					insSub1.Subscriber=patientArrayInFam[comboSubscriber1.SelectedIndex-1].PatNum;
				}
				insSub1.SubscriberID=textSubscriberID1.Text;
				InsSubs.Insert(insSub1);
			}
			#endregion Primary Ins
			#region Secondary Ins
			if(isInsComplete2) {
				if(_carrierSelected2==null) {
					_carrierSelected2=Carriers.GetByNameAndPhone(textCarrier2.Text,textPhone2.Text);
				}
				if(_insPlanSelected2==null) {
					//don't try to get a copy of an existing plan. Instead, start from scratch.
					_insPlanSelected2=new InsPlan();
					_insPlanSelected2.EmployerNum=Employers.GetEmployerNum(textEmployer2.Text);
					_insPlanSelected2.CarrierNum=_carrierSelected2.CarrierNum;
					_insPlanSelected2.GroupName=textGroupName2.Text;
					_insPlanSelected2.GroupNum=textGroupNum2.Text;
					_insPlanSelected2.PlanType="";
					if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
						_insPlanSelected2.PlanType="p";
					}
					if(_insPlanSelected2.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {
						_insPlanSelected2.ClaimsUseUCR=true;
					}
					_insPlanSelected2.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
					InsPlans.Insert(_insPlanSelected2);
					Benefit benefit=new Benefit();
					benefit.PlanNum=_insPlanSelected2.PlanNum;//same for all benefits inserted
					benefit.BenefitType=InsBenefitType.CoInsurance;//same for all benefits inserted from CovCats.ListShort
					benefit.MonetaryAmt=-1;//same for all benefits inserted from CovCats.ListShort
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;//same for all benefits inserted
					benefit.CodeNum=0;//same for all benefits inserted
					benefit.CoverageLevel=BenefitCoverageLevel.None;//same for all benefits inserted from CovCats.ListShort
					for(int i=0;i<listCovCats.Count;i++) {
						benefit.CovCatNum=listCovCats[i].CovCatNum;
						benefit.Percent=listCovCats[i].DefaultPercent;
						Benefits.Insert(benefit);
					}
					benefit.BenefitType=InsBenefitType.Deductible;//same for Diagnostic and RoutinePreventive
					benefit.Percent=-1;//same for Diagnostic and RoutinePreventive
					benefit.MonetaryAmt=0;//same for Diagnostic and RoutinePreventive
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;//same for Diagnostic and RoutinePreventive
					//Zero deductible diagnostic
					if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						Benefits.Insert(benefit);
					}
					//Zero deductible preventive
					if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						Benefits.Insert(benefit);
					}
				}
				insSub2=new InsSub();
				insSub2.PlanNum=_insPlanSelected2.PlanNum;
				insSub2.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub2.ReleaseInfo=true;
				insSub2.DateEffective=DateTime.MinValue;
				insSub2.DateTerm=DateTime.MinValue;
				if(comboSubscriber2.SelectedIndex>0) {//comboSubscriber has been validated to contain the same number of indexes as the family list
					insSub2.Subscriber=patientArrayInFam[comboSubscriber2.SelectedIndex-1].PatNum;
				}
				insSub2.SubscriberID=textSubscriberID2.Text;
				InsSubs.Insert(insSub2);
			}
			#endregion Secondary Ins
			#endregion Insert InsPlans, Benefits, and InsSubs
			#region Create PatPlans
			PatPlan patPlan1=new PatPlan();
			if(isInsComplete1) {
				patPlan1.InsSubNum=insSub1.InsSubNum;
			}
			PatPlan patPlan2=new PatPlan();
			if(isInsComplete2) {
				patPlan2.InsSubNum=insSub2.InsSubNum;
			}
			bool hasPatPlanAdded=false;
			for(int i=0;i<patientArrayInFam.Length;i++) {//loop for each possible family member. Position 0 is the guarantor and is required but all others could be null
				if(!isInsComplete1 && !isInsComplete2) {
					break;
				}
				if(patientArrayInFam[i]==null) {
					continue;
				}
				patPlan1.PatNum=patientArrayInFam[i].PatNum;
				patPlan1.Ordinal=1;
				patPlan1.Relationship=Relat.Child;//default realtionship to child, will be set to Self or Spouse for the two adults in the family
				patPlan2.PatNum=patientArrayInFam[i].PatNum;
				patPlan2.Ordinal=2;
				patPlan2.Relationship=Relat.Child;//default realtionship to child, will be set to Self or Spouse for the two adults in the family
				switch(i) {
					case 0://guarantor, 1st adult
						if(isInsComplete1 && checkInsOne1.Checked) {
							//the only situation where ordinal would be 2 is if ins2 is checked and ins1 does not have this pat as the subscriber and ins2 does
							if(checkInsTwo1.Checked && comboSubscriber1.SelectedIndex!=1 && comboSubscriber2.SelectedIndex==1) {//both combo boxes contain 'none'
								patPlan1.Ordinal=2;
							}
							patPlan1.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber1.SelectedIndex==2) {
								patPlan1.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo1.Checked) {
							//the only situations where ordinal would be 1 is if ins1 is not checked or if ins2 has this patient as subscriber and ins1 does not.
							if(!checkInsOne1.Checked || (comboSubscriber2.SelectedIndex==1 && comboSubscriber1.SelectedIndex!=1))	{
								patPlan2.Ordinal=1;
							}
							patPlan2.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber2.SelectedIndex==2) {
								patPlan2.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 1://patient 1, 2nd adult
						if(isInsComplete1 && checkInsOne2.Checked) {
							//the only situation where ordinal would be 2 is if ins2 is checked and ins1 does not have this pat as the subscriber and ins2 does
							if(checkInsTwo2.Checked && comboSubscriber1.SelectedIndex!=2 && comboSubscriber2.SelectedIndex==2) {
								patPlan1.Ordinal=2;
							}
							patPlan1.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber1.SelectedIndex==1) {
								patPlan1.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo2.Checked) {
							//the only situations where ordinal would be 1 is if ins1 is not checked or if ins2 has this patient as subscriber and ins1 does not.
							if(!checkInsOne2.Checked || (comboSubscriber2.SelectedIndex==2 && comboSubscriber1.SelectedIndex!=2))	{
								patPlan2.Ordinal=1;
							}
							patPlan2.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber2.SelectedIndex==1) {
								patPlan2.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 2://patient 2, 1st child
						if(isInsComplete1 && checkInsOne3.Checked) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo3.Checked) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(!checkInsOne3.Checked) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 3://patient 3, 2nd child
						if(isInsComplete1 && checkInsOne4.Checked) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo4.Checked) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(!checkInsOne4.Checked) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 4://patient 4, 3rd child
						if(isInsComplete1 && checkInsOne5.Checked) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo5.Checked) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(!checkInsOne5.Checked) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
				}
			}
			if(hasPatPlanAdded) {
				SecurityLogs.MakeLogEntry(Permissions.PatPlanCreate,patientArrayInFam[0].PatNum,"Multiple PatPlans created when adding multiple patients.");
			}
			#endregion Create PatPlans
			#endregion Insurance
			PatNumSelected=patientArrayInFam[0].PatNum;//Guarantor
			if(hasSavedMissingFields) {
				SecurityLogs.MakeLogEntry(Permissions.RequiredFields,PatNumSelected,"Saved patient with required fields missing.");
			}
			#region Send HL7 if Applicable
			//If there is an existing HL7 def enabled, send an ADT message for each patient inserted if there is an outbound ADT message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				for(int i=0;i<5;i++) {
					if(patientArrayInFam[i]==null) {
						continue;
					}
					//new patients get the A04 ADT, updating existing patients we send an A08
					MessageHL7 messageHL7=MessageConstructor.GenerateADT(patientArrayInFam[i],patientArrayInFam[0],EventTypeHL7.A04);//arrayPatsInFam[0] is guar, never null
					//Will be null if there is no outbound ADT message defined, so do nothing
					if(messageHL7==null) {
						continue;
					}
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patientArrayInFam[i].PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show(this,messageHL7.ToString());
					}
				}
			}
			#endregion Send HL7 if Applicable
			#region Insert HieQueue if Applicable
			if(HieClinics.IsEnabled()) {
				for(int i=0;i<5;i++) {
					if(patientArrayInFam[i]==null) {
						continue;
					}
					HieQueues.Insert(new HieQueue(patientArrayInFam[i].PatNum));
				}
			}
			#endregion
			MessageBox.Show("Done");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatientAddAll_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(_commlog == null || _commlog.CommlogNum <= 0) {
				return;
			}
			try {
				Commlogs.Delete(_commlog);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}


	}
}