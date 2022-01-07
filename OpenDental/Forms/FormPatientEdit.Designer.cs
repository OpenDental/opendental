using System.Windows.Forms;

namespace OpenDental{
	partial class FormPatientEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent(){
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientEdit));
            this.labelLName = new System.Windows.Forms.Label();
            this.labelFName = new System.Windows.Forms.Label();
            this.labelPreferredAndMiddleI = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelGender = new System.Windows.Forms.Label();
            this.labelPosition = new System.Windows.Forms.Label();
            this.labelBirthdate = new System.Windows.Forms.Label();
            this.labelSSN = new System.Windows.Forms.Label();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelAddress2 = new System.Windows.Forms.Label();
            this.labelCity = new System.Windows.Forms.Label();
            this.labelST = new System.Windows.Forms.Label();
            this.labelZip = new System.Windows.Forms.Label();
            this.labelHmPhone = new System.Windows.Forms.Label();
            this.labelWkPhone = new System.Windows.Forms.Label();
            this.labelWirelessPhone = new System.Windows.Forms.Label();
            this.textLName = new System.Windows.Forms.TextBox();
            this.textFName = new System.Windows.Forms.TextBox();
            this.textMiddleI = new System.Windows.Forms.TextBox();
            this.textPreferred = new System.Windows.Forms.TextBox();
            this.textSSN = new System.Windows.Forms.TextBox();
            this.textAddress = new System.Windows.Forms.TextBox();
            this.textAddress2 = new System.Windows.Forms.TextBox();
            this.textCity = new System.Windows.Forms.TextBox();
            this.textState = new System.Windows.Forms.TextBox();
            this.textHmPhone = new OpenDental.ValidPhone();
            this.textWkPhone = new OpenDental.ValidPhone();
            this.textWirelessPhone = new OpenDental.ValidPhone();
            this.butOK = new OpenDental.UI.Button();
            this.butCancel = new OpenDental.UI.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.textAge = new System.Windows.Forms.TextBox();
            this.textSalutation = new System.Windows.Forms.TextBox();
            this.labelSalutation = new System.Windows.Forms.Label();
            this.textEmail = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelCanadianEligibilityCode = new System.Windows.Forms.Label();
            this.comboCanadianEligibilityCode = new System.Windows.Forms.ComboBox();
            this.textSchool = new System.Windows.Forms.TextBox();
            this.radioStudentN = new System.Windows.Forms.RadioButton();
            this.radioStudentP = new System.Windows.Forms.RadioButton();
            this.radioStudentF = new System.Windows.Forms.RadioButton();
            this.labelSchoolName = new System.Windows.Forms.Label();
            this.labelChartNumber = new System.Windows.Forms.Label();
            this.textChartNumber = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkAddressSameForSuperFam = new System.Windows.Forms.CheckBox();
            this.butShowMap = new OpenDental.UI.Button();
            this.butEditZip = new OpenDental.UI.Button();
            this.textZip = new System.Windows.Forms.TextBox();
            this.comboZip = new OpenDental.UI.ComboBoxOD();
            this.checkAddressSame = new System.Windows.Forms.CheckBox();
            this.textCountry = new System.Windows.Forms.TextBox();
            this.groupNotes = new System.Windows.Forms.GroupBox();
            this.textAddrNotes = new OpenDental.ODtextBox();
            this.checkNotesSame = new System.Windows.Forms.CheckBox();
            this.textPatNum = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.butAuto = new OpenDental.UI.Button();
            this.textMedicaidID = new System.Windows.Forms.TextBox();
            this.labelMedicaidID = new System.Windows.Forms.Label();
            this.listStatus = new OpenDental.UI.ListBoxOD();
            this.listGender = new OpenDental.UI.ListBoxOD();
            this.listPosition = new OpenDental.UI.ListBoxOD();
            this.textEmployer = new System.Windows.Forms.TextBox();
            this.labelEmployer = new System.Windows.Forms.Label();
            this.labelEthnicity = new System.Windows.Forms.Label();
            this.butClearResponsParty = new OpenDental.UI.Button();
            this.butPickResponsParty = new OpenDental.UI.Button();
            this.textResponsParty = new System.Windows.Forms.TextBox();
            this.labelResponsParty = new System.Windows.Forms.Label();
            this.butPickSite = new OpenDental.UI.Button();
            this.comboUrgency = new System.Windows.Forms.ComboBox();
            this.comboGradeLevel = new System.Windows.Forms.ComboBox();
            this.textSite = new System.Windows.Forms.TextBox();
            this.labelUrgency = new System.Windows.Forms.Label();
            this.textCounty = new System.Windows.Forms.TextBox();
            this.labelGradeLevel = new System.Windows.Forms.Label();
            this.labelSite = new System.Windows.Forms.Label();
            this.labelCounty = new System.Windows.Forms.Label();
            this.labelRace = new System.Windows.Forms.Label();
            this.labelDateFirstVisit = new System.Windows.Forms.Label();
            this.labelPutInInsPlan = new System.Windows.Forms.Label();
            this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
            this.textTrophyFolder = new System.Windows.Forms.TextBox();
            this.labelTrophyFolder = new System.Windows.Forms.Label();
            this.textWard = new System.Windows.Forms.TextBox();
            this.labelWard = new System.Windows.Forms.Label();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.comboContact = new System.Windows.Forms.ComboBox();
            this.labelContact = new System.Windows.Forms.Label();
            this.comboConfirm = new System.Windows.Forms.ComboBox();
            this.labelConfirm = new System.Windows.Forms.Label();
            this.comboRecall = new System.Windows.Forms.ComboBox();
            this.labelRecall = new System.Windows.Forms.Label();
            this.labelAdmitDate = new System.Windows.Forms.Label();
            this.textTitle = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.listRelationships = new OpenDental.UI.ListBoxOD();
            this.butAddGuardian = new OpenDental.UI.Button();
            this.butGuardianDefaults = new OpenDental.UI.Button();
            this.textAskToArriveEarly = new System.Windows.Forms.TextBox();
            this.labelAskToArriveEarly = new System.Windows.Forms.Label();
            this.checkArriveEarlySame = new System.Windows.Forms.CheckBox();
            this.label43 = new System.Windows.Forms.Label();
            this.labelTextOk = new System.Windows.Forms.Label();
            this.listTextOk = new System.Windows.Forms.ListBox();
            this.textMotherMaidenFname = new System.Windows.Forms.TextBox();
            this.labelMotherMaidenFname = new System.Windows.Forms.Label();
            this.textMotherMaidenLname = new System.Windows.Forms.TextBox();
            this.labelMotherMaidenLname = new System.Windows.Forms.Label();
            this.labelDeceased = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxApptTexts = new System.Windows.Forms.ListBox();
            this.butShortCodeOptIn = new OpenDental.UI.Button();
            this.labelApptTexts = new System.Windows.Forms.Label();
            this.labelABC = new System.Windows.Forms.Label();
            this.butEmailEdit = new OpenDental.UI.Button();
            this.checkEmailPhoneSame = new System.Windows.Forms.CheckBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.groupBillProv = new System.Windows.Forms.GroupBox();
            this.checkBillProvSame = new System.Windows.Forms.CheckBox();
            this.checkSuperBilling = new System.Windows.Forms.CheckBox();
            this.butPickSecondary = new OpenDental.UI.Button();
            this.comboBillType = new OpenDental.UI.ComboBoxOD();
            this.butPickPrimary = new OpenDental.UI.Button();
            this.labelBillType = new System.Windows.Forms.Label();
            this.labelFeeSched = new System.Windows.Forms.Label();
            this.textCreditType = new System.Windows.Forms.TextBox();
            this.labelSecProv = new System.Windows.Forms.Label();
            this.comboFeeSched = new OpenDental.UI.ComboBoxOD();
            this.labelCreditType = new System.Windows.Forms.Label();
            this.labelPriProv = new System.Windows.Forms.Label();
            this.comboPriProv = new OpenDental.UI.ComboBoxOD();
            this.comboSecProv = new OpenDental.UI.ComboBoxOD();
            this.textReferredFrom = new System.Windows.Forms.TextBox();
            this.butReferredFrom = new OpenDental.UI.Button();
            this.labelReferredFrom = new System.Windows.Forms.Label();
            this.textMedicaidState = new System.Windows.Forms.TextBox();
            this.labelRequiredField = new System.Windows.Forms.Label();
            this.checkRestrictSched = new System.Windows.Forms.CheckBox();
            this.tabControlPatInfo = new System.Windows.Forms.TabControl();
            this.tabPublicHealth = new System.Windows.Forms.TabPage();
            this.comboBoxMultiRace = new OpenDental.UI.ComboBoxOD();
            this.comboEthnicity = new System.Windows.Forms.ComboBox();
            this.textEthnicity = new System.Windows.Forms.TextBox();
            this.textRace = new System.Windows.Forms.TextBox();
            this.butRaceEthnicity = new OpenDental.UI.Button();
            this.labelSexOrientation = new System.Windows.Forms.Label();
            this.labelSpecifySexOrientation = new System.Windows.Forms.Label();
            this.labelGenderIdentity = new System.Windows.Forms.Label();
            this.labelSpecifyGender = new System.Windows.Forms.Label();
            this.textSpecifyGender = new System.Windows.Forms.TextBox();
            this.textSpecifySexOrientation = new System.Windows.Forms.TextBox();
            this.comboGenderIdentity = new System.Windows.Forms.ComboBox();
            this.comboSexOrientation = new System.Windows.Forms.ComboBox();
            this.tabHospitals = new System.Windows.Forms.TabPage();
            this.odDatePickerAdmitDate = new OpenDental.UI.ODDatePicker();
            this.tabOther = new System.Windows.Forms.TabPage();
            this.butViewSSN = new OpenDental.UI.Button();
            this.checkDoseSpotConsent = new System.Windows.Forms.CheckBox();
            this.checkBoxSignedTil = new System.Windows.Forms.CheckBox();
            this.odDatePickerDateFirstVisit = new OpenDental.UI.ODDatePicker();
            this.tabICE = new System.Windows.Forms.TabPage();
            this.labelEmergencyPhone = new System.Windows.Forms.Label();
            this.textIcePhone = new OpenDental.ValidPhone();
            this.textIceName = new System.Windows.Forms.TextBox();
            this.labelEmergencyName = new System.Windows.Forms.Label();
            this.tabEHR = new System.Windows.Forms.TabPage();
            this.butClearDateTimeDeceased = new OpenDental.UI.Button();
            this.dateTimePickerDateDeceased = new System.Windows.Forms.DateTimePicker();
            this.comboSpecialty = new OpenDental.UI.ComboBoxOD();
            this.labelSpecialty = new System.Windows.Forms.Label();
            this.comboExcludeECR = new OpenDental.UI.ComboBoxOD();
            this.labelExcludeECR = new System.Windows.Forms.Label();
            this.butViewBirthdate = new OpenDental.UI.Button();
            this.odDatePickerBirthDate = new OpenDental.UI.ODDatePicker();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupNotes.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBillProv.SuspendLayout();
            this.tabControlPatInfo.SuspendLayout();
            this.tabPublicHealth.SuspendLayout();
            this.tabHospitals.SuspendLayout();
            this.tabOther.SuspendLayout();
            this.tabICE.SuspendLayout();
            this.tabEHR.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelLName
            // 
            this.labelLName.Location = new System.Drawing.Point(5, 51);
            this.labelLName.Name = "labelLName";
            this.labelLName.Size = new System.Drawing.Size(154, 14);
            this.labelLName.TabIndex = 0;
            this.labelLName.Text = "Last Name";
            this.labelLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFName
            // 
            this.labelFName.Location = new System.Drawing.Point(5, 71);
            this.labelFName.Name = "labelFName";
            this.labelFName.Size = new System.Drawing.Size(154, 14);
            this.labelFName.TabIndex = 0;
            this.labelFName.Text = "First Name";
            this.labelFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPreferredAndMiddleI
            // 
            this.labelPreferredAndMiddleI.Location = new System.Drawing.Point(5, 91);
            this.labelPreferredAndMiddleI.Name = "labelPreferredAndMiddleI";
            this.labelPreferredAndMiddleI.Size = new System.Drawing.Size(154, 14);
            this.labelPreferredAndMiddleI.TabIndex = 0;
            this.labelPreferredAndMiddleI.Text = "Preferred Name, Middle Initial";
            this.labelPreferredAndMiddleI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(19, 157);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(105, 14);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelGender
            // 
            this.labelGender.Location = new System.Drawing.Point(134, 157);
            this.labelGender.Name = "labelGender";
            this.labelGender.Size = new System.Drawing.Size(105, 14);
            this.labelGender.TabIndex = 0;
            this.labelGender.Text = "Gender";
            this.labelGender.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelPosition
            // 
            this.labelPosition.Location = new System.Drawing.Point(249, 157);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(105, 14);
            this.labelPosition.TabIndex = 0;
            this.labelPosition.Text = "Position";
            this.labelPosition.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelBirthdate
            // 
            this.labelBirthdate.Location = new System.Drawing.Point(0, 266);
            this.labelBirthdate.Name = "labelBirthdate";
            this.labelBirthdate.Size = new System.Drawing.Size(154, 14);
            this.labelBirthdate.TabIndex = 0;
            this.labelBirthdate.Text = "Birthdate";
            this.labelBirthdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSSN
            // 
            this.labelSSN.Location = new System.Drawing.Point(12, 9);
            this.labelSSN.Name = "labelSSN";
            this.labelSSN.Size = new System.Drawing.Size(142, 14);
            this.labelSSN.TabIndex = 0;
            this.labelSSN.Text = "SS#";
            this.labelSSN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelAddress
            // 
            this.labelAddress.Location = new System.Drawing.Point(6, 54);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(152, 14);
            this.labelAddress.TabIndex = 0;
            this.labelAddress.Text = "Address";
            this.labelAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelAddress2
            // 
            this.labelAddress2.Location = new System.Drawing.Point(6, 74);
            this.labelAddress2.Name = "labelAddress2";
            this.labelAddress2.Size = new System.Drawing.Size(152, 14);
            this.labelAddress2.TabIndex = 0;
            this.labelAddress2.Text = "Address2";
            this.labelAddress2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCity
            // 
            this.labelCity.Location = new System.Drawing.Point(6, 94);
            this.labelCity.Name = "labelCity";
            this.labelCity.Size = new System.Drawing.Size(152, 14);
            this.labelCity.TabIndex = 0;
            this.labelCity.Text = "City";
            this.labelCity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelST
            // 
            this.labelST.Location = new System.Drawing.Point(6, 114);
            this.labelST.Name = "labelST";
            this.labelST.Size = new System.Drawing.Size(152, 14);
            this.labelST.TabIndex = 0;
            this.labelST.Text = "ST";
            this.labelST.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelZip
            // 
            this.labelZip.Location = new System.Drawing.Point(6, 134);
            this.labelZip.Name = "labelZip";
            this.labelZip.Size = new System.Drawing.Size(152, 14);
            this.labelZip.TabIndex = 0;
            this.labelZip.Text = "Zip";
            this.labelZip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHmPhone
            // 
            this.labelHmPhone.Location = new System.Drawing.Point(6, 34);
            this.labelHmPhone.Name = "labelHmPhone";
            this.labelHmPhone.Size = new System.Drawing.Size(152, 14);
            this.labelHmPhone.TabIndex = 0;
            this.labelHmPhone.Text = "Home Phone";
            this.labelHmPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWkPhone
            // 
            this.labelWkPhone.Location = new System.Drawing.Point(6, 54);
            this.labelWkPhone.Name = "labelWkPhone";
            this.labelWkPhone.Size = new System.Drawing.Size(152, 14);
            this.labelWkPhone.TabIndex = 0;
            this.labelWkPhone.Text = "Work Phone";
            this.labelWkPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWirelessPhone
            // 
            this.labelWirelessPhone.Location = new System.Drawing.Point(6, 34);
            this.labelWirelessPhone.Name = "labelWirelessPhone";
            this.labelWirelessPhone.Size = new System.Drawing.Size(152, 14);
            this.labelWirelessPhone.TabIndex = 0;
            this.labelWirelessPhone.Text = "Wireless Phone";
            this.labelWirelessPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textLName
            // 
            this.textLName.Location = new System.Drawing.Point(160, 48);
            this.textLName.MaxLength = 100;
            this.textLName.Name = "textLName";
            this.textLName.Size = new System.Drawing.Size(228, 20);
            this.textLName.TabIndex = 0;
            this.textLName.TextChanged += new System.EventHandler(this.textLName_TextChanged);
            this.textLName.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textFName
            // 
            this.textFName.Location = new System.Drawing.Point(160, 68);
            this.textFName.MaxLength = 100;
            this.textFName.Name = "textFName";
            this.textFName.Size = new System.Drawing.Size(228, 20);
            this.textFName.TabIndex = 1;
            this.textFName.TextChanged += new System.EventHandler(this.textFName_TextChanged);
            this.textFName.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textMiddleI
            // 
            this.textMiddleI.Location = new System.Drawing.Point(306, 88);
            this.textMiddleI.MaxLength = 100;
            this.textMiddleI.Name = "textMiddleI";
            this.textMiddleI.Size = new System.Drawing.Size(82, 20);
            this.textMiddleI.TabIndex = 3;
            this.textMiddleI.TextChanged += new System.EventHandler(this.textMiddleI_TextChanged);
            this.textMiddleI.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textPreferred
            // 
            this.textPreferred.Location = new System.Drawing.Point(160, 88);
            this.textPreferred.MaxLength = 100;
            this.textPreferred.Name = "textPreferred";
            this.textPreferred.Size = new System.Drawing.Size(145, 20);
            this.textPreferred.TabIndex = 2;
            this.textPreferred.TextChanged += new System.EventHandler(this.textPreferred_TextChanged);
            this.textPreferred.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textSSN
            // 
            this.textSSN.Location = new System.Drawing.Point(155, 6);
            this.textSSN.MaxLength = 100;
            this.textSSN.Name = "textSSN";
            this.textSSN.Size = new System.Drawing.Size(102, 20);
            this.textSSN.TabIndex = 11;
            this.textSSN.Leave += new System.EventHandler(this.textBox_Leave);
            this.textSSN.Validating += new System.ComponentModel.CancelEventHandler(this.textSSN_Validating);
            // 
            // textAddress
            // 
            this.textAddress.Location = new System.Drawing.Point(159, 51);
            this.textAddress.MaxLength = 100;
            this.textAddress.Name = "textAddress";
            this.textAddress.Size = new System.Drawing.Size(254, 20);
            this.textAddress.TabIndex = 1;
            this.textAddress.TextChanged += new System.EventHandler(this.textAddress_TextChanged);
            this.textAddress.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textAddress2
            // 
            this.textAddress2.Location = new System.Drawing.Point(159, 71);
            this.textAddress2.MaxLength = 100;
            this.textAddress2.Name = "textAddress2";
            this.textAddress2.Size = new System.Drawing.Size(254, 20);
            this.textAddress2.TabIndex = 2;
            this.textAddress2.TextChanged += new System.EventHandler(this.textAddress2_TextChanged);
            this.textAddress2.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textCity
            // 
            this.textCity.Location = new System.Drawing.Point(159, 91);
            this.textCity.MaxLength = 100;
            this.textCity.Name = "textCity";
            this.textCity.Size = new System.Drawing.Size(198, 20);
            this.textCity.TabIndex = 3;
            this.textCity.TextChanged += new System.EventHandler(this.textCity_TextChanged);
            this.textCity.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textState
            // 
            this.textState.Location = new System.Drawing.Point(159, 111);
            this.textState.MaxLength = 100;
            this.textState.Name = "textState";
            this.textState.Size = new System.Drawing.Size(61, 20);
            this.textState.TabIndex = 4;
            this.textState.TextChanged += new System.EventHandler(this.textState_TextChanged);
            this.textState.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textState_KeyUp);
            this.textState.Leave += new System.EventHandler(this.textState_Leave);
            // 
            // textHmPhone
            // 
            this.textHmPhone.Location = new System.Drawing.Point(159, 31);
            this.textHmPhone.MaxLength = 30;
            this.textHmPhone.Name = "textHmPhone";
            this.textHmPhone.Size = new System.Drawing.Size(198, 20);
            this.textHmPhone.TabIndex = 0;
            this.textHmPhone.TextChanged += new System.EventHandler(this.textAnyPhoneNumber_TextChanged);
            this.textHmPhone.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textWkPhone
            // 
            this.textWkPhone.Location = new System.Drawing.Point(159, 51);
            this.textWkPhone.MaxLength = 30;
            this.textWkPhone.Name = "textWkPhone";
            this.textWkPhone.Size = new System.Drawing.Size(135, 20);
            this.textWkPhone.TabIndex = 1;
            this.textWkPhone.TextChanged += new System.EventHandler(this.textAnyPhoneNumber_TextChanged);
            this.textWkPhone.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textWirelessPhone
            // 
            this.textWirelessPhone.Location = new System.Drawing.Point(159, 31);
            this.textWirelessPhone.MaxLength = 30;
            this.textWirelessPhone.Name = "textWirelessPhone";
            this.textWirelessPhone.Size = new System.Drawing.Size(135, 20);
            this.textWirelessPhone.TabIndex = 0;
            this.textWirelessPhone.TextChanged += new System.EventHandler(this.textAnyPhoneNumber_TextChanged);
            this.textWirelessPhone.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(801, 665);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 26);
            this.butOK.TabIndex = 34;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(887, 665);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 26);
            this.butCancel.TabIndex = 35;
            this.butCancel.Text = "&Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(339, 266);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(29, 14);
            this.label20.TabIndex = 0;
            this.label20.Text = "Age";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textAge
            // 
            this.textAge.Location = new System.Drawing.Point(369, 264);
            this.textAge.Name = "textAge";
            this.textAge.ReadOnly = true;
            this.textAge.Size = new System.Drawing.Size(50, 20);
            this.textAge.TabIndex = 0;
            this.textAge.TabStop = false;
            // 
            // textSalutation
            // 
            this.textSalutation.Location = new System.Drawing.Point(160, 128);
            this.textSalutation.MaxLength = 100;
            this.textSalutation.Name = "textSalutation";
            this.textSalutation.Size = new System.Drawing.Size(228, 20);
            this.textSalutation.TabIndex = 5;
            this.textSalutation.TextChanged += new System.EventHandler(this.textSalutation_TextChanged);
            this.textSalutation.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelSalutation
            // 
            this.labelSalutation.Location = new System.Drawing.Point(5, 131);
            this.labelSalutation.Name = "labelSalutation";
            this.labelSalutation.Size = new System.Drawing.Size(154, 14);
            this.labelSalutation.TabIndex = 0;
            this.labelSalutation.Text = "Salutation (Dear __)";
            this.labelSalutation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textEmail
            // 
            this.textEmail.Location = new System.Drawing.Point(159, 71);
            this.textEmail.MaxLength = 100;
            this.textEmail.Name = "textEmail";
            this.textEmail.Size = new System.Drawing.Size(249, 20);
            this.textEmail.TabIndex = 2;
            this.textEmail.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEmail_KeyUp);
            this.textEmail.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelCanadianEligibilityCode);
            this.groupBox2.Controls.Add(this.comboCanadianEligibilityCode);
            this.groupBox2.Controls.Add(this.textSchool);
            this.groupBox2.Controls.Add(this.radioStudentN);
            this.groupBox2.Controls.Add(this.radioStudentP);
            this.groupBox2.Controls.Add(this.radioStudentF);
            this.groupBox2.Controls.Add(this.labelSchoolName);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(439, 84);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Student Status if Dependent Over 19 (for Ins)";
            // 
            // labelCanadianEligibilityCode
            // 
            this.labelCanadianEligibilityCode.Location = new System.Drawing.Point(6, 60);
            this.labelCanadianEligibilityCode.Name = "labelCanadianEligibilityCode";
            this.labelCanadianEligibilityCode.Size = new System.Drawing.Size(136, 14);
            this.labelCanadianEligibilityCode.TabIndex = 0;
            this.labelCanadianEligibilityCode.Text = "Eligibility Excep. Code";
            this.labelCanadianEligibilityCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelCanadianEligibilityCode.Visible = false;
            // 
            // comboCanadianEligibilityCode
            // 
            this.comboCanadianEligibilityCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCanadianEligibilityCode.FormattingEnabled = true;
            this.comboCanadianEligibilityCode.Location = new System.Drawing.Point(143, 57);
            this.comboCanadianEligibilityCode.Name = "comboCanadianEligibilityCode";
            this.comboCanadianEligibilityCode.Size = new System.Drawing.Size(224, 21);
            this.comboCanadianEligibilityCode.TabIndex = 5;
            this.comboCanadianEligibilityCode.Visible = false;
            this.comboCanadianEligibilityCode.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // textSchool
            // 
            this.textSchool.Location = new System.Drawing.Point(143, 37);
            this.textSchool.MaxLength = 255;
            this.textSchool.Name = "textSchool";
            this.textSchool.Size = new System.Drawing.Size(224, 20);
            this.textSchool.TabIndex = 4;
            this.textSchool.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // radioStudentN
            // 
            this.radioStudentN.Location = new System.Drawing.Point(122, 18);
            this.radioStudentN.Name = "radioStudentN";
            this.radioStudentN.Size = new System.Drawing.Size(117, 16);
            this.radioStudentN.TabIndex = 1;
            this.radioStudentN.TabStop = true;
            this.radioStudentN.Text = "Nonstudent";
            this.radioStudentN.Click += new System.EventHandler(this.radioStudentN_Click);
            // 
            // radioStudentP
            // 
            this.radioStudentP.Location = new System.Drawing.Point(332, 18);
            this.radioStudentP.Name = "radioStudentP";
            this.radioStudentP.Size = new System.Drawing.Size(86, 16);
            this.radioStudentP.TabIndex = 3;
            this.radioStudentP.TabStop = true;
            this.radioStudentP.Text = "Parttime";
            this.radioStudentP.Click += new System.EventHandler(this.radioStudentP_Click);
            // 
            // radioStudentF
            // 
            this.radioStudentF.Location = new System.Drawing.Point(239, 18);
            this.radioStudentF.Name = "radioStudentF";
            this.radioStudentF.Size = new System.Drawing.Size(93, 16);
            this.radioStudentF.TabIndex = 2;
            this.radioStudentF.TabStop = true;
            this.radioStudentF.Text = "Fulltime";
            this.radioStudentF.Click += new System.EventHandler(this.radioStudentF_Click);
            // 
            // labelSchoolName
            // 
            this.labelSchoolName.Location = new System.Drawing.Point(6, 40);
            this.labelSchoolName.Name = "labelSchoolName";
            this.labelSchoolName.Size = new System.Drawing.Size(135, 14);
            this.labelSchoolName.TabIndex = 0;
            this.labelSchoolName.Text = "College Name";
            this.labelSchoolName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelChartNumber
            // 
            this.labelChartNumber.Location = new System.Drawing.Point(5, 306);
            this.labelChartNumber.Name = "labelChartNumber";
            this.labelChartNumber.Size = new System.Drawing.Size(154, 16);
            this.labelChartNumber.TabIndex = 0;
            this.labelChartNumber.Text = "ChartNumber";
            this.labelChartNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textChartNumber
            // 
            this.textChartNumber.Location = new System.Drawing.Point(160, 304);
            this.textChartNumber.MaxLength = 20;
            this.textChartNumber.Name = "textChartNumber";
            this.textChartNumber.Size = new System.Drawing.Size(102, 20);
            this.textChartNumber.TabIndex = 9;
            this.textChartNumber.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkAddressSameForSuperFam);
            this.groupBox1.Controls.Add(this.butShowMap);
            this.groupBox1.Controls.Add(this.textHmPhone);
            this.groupBox1.Controls.Add(this.butEditZip);
            this.groupBox1.Controls.Add(this.textZip);
            this.groupBox1.Controls.Add(this.comboZip);
            this.groupBox1.Controls.Add(this.checkAddressSame);
            this.groupBox1.Controls.Add(this.textCountry);
            this.groupBox1.Controls.Add(this.textState);
            this.groupBox1.Controls.Add(this.labelST);
            this.groupBox1.Controls.Add(this.textAddress);
            this.groupBox1.Controls.Add(this.labelAddress2);
            this.groupBox1.Controls.Add(this.labelCity);
            this.groupBox1.Controls.Add(this.textAddress2);
            this.groupBox1.Controls.Add(this.labelZip);
            this.groupBox1.Controls.Add(this.labelHmPhone);
            this.groupBox1.Controls.Add(this.textCity);
            this.groupBox1.Controls.Add(this.labelAddress);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(488, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(474, 157);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Address and Phone";
            // 
            // checkAddressSameForSuperFam
            // 
            this.checkAddressSameForSuperFam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAddressSameForSuperFam.Location = new System.Drawing.Point(286, 12);
            this.checkAddressSameForSuperFam.Name = "checkAddressSameForSuperFam";
            this.checkAddressSameForSuperFam.Size = new System.Drawing.Size(168, 17);
            this.checkAddressSameForSuperFam.TabIndex = 0;
            this.checkAddressSameForSuperFam.TabStop = false;
            this.checkAddressSameForSuperFam.Text = "Same for entire super family";
            this.checkAddressSameForSuperFam.Visible = false;
            // 
            // butShowMap
            // 
            this.butShowMap.Location = new System.Drawing.Point(428, 131);
            this.butShowMap.Name = "butShowMap";
            this.butShowMap.Size = new System.Drawing.Size(37, 22);
            this.butShowMap.TabIndex = 9;
            this.butShowMap.Text = "Map";
            this.butShowMap.Visible = false;
            this.butShowMap.Click += new System.EventHandler(this.butShowMap_Click);
            // 
            // butEditZip
            // 
            this.butEditZip.Location = new System.Drawing.Point(362, 131);
            this.butEditZip.Name = "butEditZip";
            this.butEditZip.Size = new System.Drawing.Size(65, 22);
            this.butEditZip.TabIndex = 8;
            this.butEditZip.Text = "&Edit Zip";
            this.butEditZip.Click += new System.EventHandler(this.butEditZip_Click);
            // 
            // textZip
            // 
            this.textZip.Location = new System.Drawing.Point(159, 131);
            this.textZip.MaxLength = 100;
            this.textZip.Name = "textZip";
            this.textZip.Size = new System.Drawing.Size(184, 20);
            this.textZip.TabIndex = 6;
            this.textZip.TextChanged += new System.EventHandler(this.textZip_TextChanged);
            this.textZip.Leave += new System.EventHandler(this.textBox_Leave);
            this.textZip.Validating += new System.ComponentModel.CancelEventHandler(this.textZip_Validating);
            // 
            // comboZip
            // 
            this.comboZip.Location = new System.Drawing.Point(159, 131);
            this.comboZip.Name = "comboZip";
            this.comboZip.Size = new System.Drawing.Size(200, 21);
            this.comboZip.TabIndex = 0;
            this.comboZip.TabStop = false;
            this.comboZip.SelectionChangeCommitted += new System.EventHandler(this.comboZip_SelectionChangeCommitted);
            // 
            // checkAddressSame
            // 
            this.checkAddressSame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAddressSame.Location = new System.Drawing.Point(159, 12);
            this.checkAddressSame.Name = "checkAddressSame";
            this.checkAddressSame.Size = new System.Drawing.Size(162, 17);
            this.checkAddressSame.TabIndex = 0;
            this.checkAddressSame.TabStop = false;
            this.checkAddressSame.Text = "Same for entire family";
            // 
            // textCountry
            // 
            this.textCountry.Location = new System.Drawing.Point(221, 111);
            this.textCountry.MaxLength = 100;
            this.textCountry.Name = "textCountry";
            this.textCountry.Size = new System.Drawing.Size(192, 20);
            this.textCountry.TabIndex = 5;
            this.textCountry.Visible = false;
            this.textCountry.TextChanged += new System.EventHandler(this.textState_TextChanged);
            // 
            // groupNotes
            // 
            this.groupNotes.Controls.Add(this.textAddrNotes);
            this.groupNotes.Controls.Add(this.checkNotesSame);
            this.groupNotes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupNotes.Location = new System.Drawing.Point(488, 167);
            this.groupNotes.Name = "groupNotes";
            this.groupNotes.Size = new System.Drawing.Size(474, 81);
            this.groupNotes.TabIndex = 15;
            this.groupNotes.TabStop = false;
            this.groupNotes.Text = "Address and Phone Notes";
            // 
            // textAddrNotes
            // 
            this.textAddrNotes.AcceptsTab = true;
            this.textAddrNotes.BackColor = System.Drawing.SystemColors.Window;
            this.textAddrNotes.DetectLinksEnabled = false;
            this.textAddrNotes.DetectUrls = false;
            this.textAddrNotes.Location = new System.Drawing.Point(159, 31);
            this.textAddrNotes.Name = "textAddrNotes";
            this.textAddrNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.PatAddressNote;
            this.textAddrNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textAddrNotes.Size = new System.Drawing.Size(229, 46);
            this.textAddrNotes.TabIndex = 0;
            this.textAddrNotes.Text = "";
            this.textAddrNotes.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // checkNotesSame
            // 
            this.checkNotesSame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkNotesSame.Location = new System.Drawing.Point(159, 12);
            this.checkNotesSame.Name = "checkNotesSame";
            this.checkNotesSame.Size = new System.Drawing.Size(254, 17);
            this.checkNotesSame.TabIndex = 0;
            this.checkNotesSame.TabStop = false;
            this.checkNotesSame.Text = "Same for entire family";
            // 
            // textPatNum
            // 
            this.textPatNum.Location = new System.Drawing.Point(160, 28);
            this.textPatNum.MaxLength = 100;
            this.textPatNum.Name = "textPatNum";
            this.textPatNum.ReadOnly = true;
            this.textPatNum.Size = new System.Drawing.Size(228, 20);
            this.textPatNum.TabIndex = 0;
            this.textPatNum.TabStop = false;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(5, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(154, 14);
            this.label19.TabIndex = 0;
            this.label19.Text = "Patient Number";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(341, 307);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(117, 17);
            this.label32.TabIndex = 0;
            this.label32.Text = "(if used)";
            // 
            // butAuto
            // 
            this.butAuto.Location = new System.Drawing.Point(271, 304);
            this.butAuto.Name = "butAuto";
            this.butAuto.Size = new System.Drawing.Size(62, 21);
            this.butAuto.TabIndex = 10;
            this.butAuto.Text = "Auto";
            this.butAuto.Click += new System.EventHandler(this.butAuto_Click);
            // 
            // textMedicaidID
            // 
            this.textMedicaidID.Location = new System.Drawing.Point(160, 284);
            this.textMedicaidID.MaxLength = 20;
            this.textMedicaidID.Name = "textMedicaidID";
            this.textMedicaidID.Size = new System.Drawing.Size(102, 20);
            this.textMedicaidID.TabIndex = 7;
            this.textMedicaidID.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelMedicaidID
            // 
            this.labelMedicaidID.Location = new System.Drawing.Point(5, 287);
            this.labelMedicaidID.Name = "labelMedicaidID";
            this.labelMedicaidID.Size = new System.Drawing.Size(154, 14);
            this.labelMedicaidID.TabIndex = 0;
            this.labelMedicaidID.Text = "Medicaid ID, State";
            this.labelMedicaidID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listStatus
            // 
            this.listStatus.Location = new System.Drawing.Point(19, 172);
            this.listStatus.Name = "listStatus";
            this.listStatus.Size = new System.Drawing.Size(105, 69);
            this.listStatus.TabIndex = 0;
            this.listStatus.TabStop = false;
            this.listStatus.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            // 
            // listGender
            // 
            this.listGender.Location = new System.Drawing.Point(134, 172);
            this.listGender.Name = "listGender";
            this.listGender.Size = new System.Drawing.Size(105, 43);
            this.listGender.TabIndex = 0;
            this.listGender.TabStop = false;
            this.listGender.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            // 
            // listPosition
            // 
            this.listPosition.Location = new System.Drawing.Point(249, 172);
            this.listPosition.Name = "listPosition";
            this.listPosition.Size = new System.Drawing.Size(105, 69);
            this.listPosition.TabIndex = 0;
            this.listPosition.TabStop = false;
            this.listPosition.SelectedIndexChanged += new System.EventHandler(this.listPosition_SelectedIndexChanged);
            // 
            // textEmployer
            // 
            this.textEmployer.Location = new System.Drawing.Point(160, 344);
            this.textEmployer.MaxLength = 255;
            this.textEmployer.Name = "textEmployer";
            this.textEmployer.Size = new System.Drawing.Size(228, 20);
            this.textEmployer.TabIndex = 12;
            this.textEmployer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEmployer_KeyUp);
            this.textEmployer.Leave += new System.EventHandler(this.textEmployer_Leave);
            // 
            // labelEmployer
            // 
            this.labelEmployer.Location = new System.Drawing.Point(5, 347);
            this.labelEmployer.Name = "labelEmployer";
            this.labelEmployer.Size = new System.Drawing.Size(154, 14);
            this.labelEmployer.TabIndex = 0;
            this.labelEmployer.Text = "Employer";
            this.labelEmployer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelEthnicity
            // 
            this.labelEthnicity.Location = new System.Drawing.Point(6, 30);
            this.labelEthnicity.Name = "labelEthnicity";
            this.labelEthnicity.Size = new System.Drawing.Size(148, 14);
            this.labelEthnicity.TabIndex = 0;
            this.labelEthnicity.Text = "Ethnicity";
            this.labelEthnicity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butClearResponsParty
            // 
            this.butClearResponsParty.AdjustImageLocation = new System.Drawing.Point(1, 1);
            this.butClearResponsParty.Icon = OpenDental.UI.EnumIcons.DeleteX;
            this.butClearResponsParty.Location = new System.Drawing.Point(409, 129);
            this.butClearResponsParty.Name = "butClearResponsParty";
            this.butClearResponsParty.Size = new System.Drawing.Size(25, 23);
            this.butClearResponsParty.TabIndex = 9;
            this.butClearResponsParty.Click += new System.EventHandler(this.butClearResponsParty_Click);
            // 
            // butPickResponsParty
            // 
            this.butPickResponsParty.Location = new System.Drawing.Point(386, 129);
            this.butPickResponsParty.Name = "butPickResponsParty";
            this.butPickResponsParty.Size = new System.Drawing.Size(23, 23);
            this.butPickResponsParty.TabIndex = 8;
            this.butPickResponsParty.Text = "...";
            this.butPickResponsParty.Click += new System.EventHandler(this.butPickResponsParty_Click);
            // 
            // textResponsParty
            // 
            this.textResponsParty.AcceptsReturn = true;
            this.textResponsParty.Location = new System.Drawing.Point(155, 130);
            this.textResponsParty.Name = "textResponsParty";
            this.textResponsParty.ReadOnly = true;
            this.textResponsParty.Size = new System.Drawing.Size(229, 20);
            this.textResponsParty.TabIndex = 0;
            this.textResponsParty.TabStop = false;
            this.textResponsParty.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelResponsParty
            // 
            this.labelResponsParty.Location = new System.Drawing.Point(6, 133);
            this.labelResponsParty.Name = "labelResponsParty";
            this.labelResponsParty.Size = new System.Drawing.Size(148, 14);
            this.labelResponsParty.TabIndex = 0;
            this.labelResponsParty.Text = "Responsible Party";
            this.labelResponsParty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butPickSite
            // 
            this.butPickSite.Location = new System.Drawing.Point(386, 68);
            this.butPickSite.Name = "butPickSite";
            this.butPickSite.Size = new System.Drawing.Size(23, 20);
            this.butPickSite.TabIndex = 5;
            this.butPickSite.Text = "...";
            this.butPickSite.Click += new System.EventHandler(this.butPickSite_Click);
            // 
            // comboUrgency
            // 
            this.comboUrgency.BackColor = System.Drawing.SystemColors.Window;
            this.comboUrgency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUrgency.Location = new System.Drawing.Point(155, 109);
            this.comboUrgency.Name = "comboUrgency";
            this.comboUrgency.Size = new System.Drawing.Size(229, 21);
            this.comboUrgency.TabIndex = 7;
            this.comboUrgency.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // comboGradeLevel
            // 
            this.comboGradeLevel.BackColor = System.Drawing.SystemColors.Window;
            this.comboGradeLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGradeLevel.Location = new System.Drawing.Point(155, 88);
            this.comboGradeLevel.MaxDropDownItems = 25;
            this.comboGradeLevel.Name = "comboGradeLevel";
            this.comboGradeLevel.Size = new System.Drawing.Size(229, 21);
            this.comboGradeLevel.TabIndex = 6;
            this.comboGradeLevel.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // textSite
            // 
            this.textSite.AcceptsReturn = true;
            this.textSite.Location = new System.Drawing.Point(155, 68);
            this.textSite.Name = "textSite";
            this.textSite.Size = new System.Drawing.Size(229, 20);
            this.textSite.TabIndex = 4;
            this.textSite.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textSite_KeyUp);
            this.textSite.Leave += new System.EventHandler(this.textSite_Leave);
            // 
            // labelUrgency
            // 
            this.labelUrgency.Location = new System.Drawing.Point(6, 112);
            this.labelUrgency.Name = "labelUrgency";
            this.labelUrgency.Size = new System.Drawing.Size(148, 14);
            this.labelUrgency.TabIndex = 0;
            this.labelUrgency.Text = "Treatment Urgency";
            this.labelUrgency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textCounty
            // 
            this.textCounty.AcceptsReturn = true;
            this.textCounty.Location = new System.Drawing.Point(155, 48);
            this.textCounty.Name = "textCounty";
            this.textCounty.Size = new System.Drawing.Size(229, 20);
            this.textCounty.TabIndex = 3;
            this.textCounty.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCounty_KeyUp);
            this.textCounty.Leave += new System.EventHandler(this.textCounty_Leave);
            // 
            // labelGradeLevel
            // 
            this.labelGradeLevel.Location = new System.Drawing.Point(6, 91);
            this.labelGradeLevel.Name = "labelGradeLevel";
            this.labelGradeLevel.Size = new System.Drawing.Size(148, 14);
            this.labelGradeLevel.TabIndex = 0;
            this.labelGradeLevel.Text = "Grade Level";
            this.labelGradeLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSite
            // 
            this.labelSite.Location = new System.Drawing.Point(6, 71);
            this.labelSite.Name = "labelSite";
            this.labelSite.Size = new System.Drawing.Size(148, 14);
            this.labelSite.TabIndex = 0;
            this.labelSite.Text = "Site (or Grade School)";
            this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCounty
            // 
            this.labelCounty.Location = new System.Drawing.Point(6, 51);
            this.labelCounty.Name = "labelCounty";
            this.labelCounty.Size = new System.Drawing.Size(148, 14);
            this.labelCounty.TabIndex = 0;
            this.labelCounty.Text = "County";
            this.labelCounty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRace
            // 
            this.labelRace.Location = new System.Drawing.Point(6, 9);
            this.labelRace.Name = "labelRace";
            this.labelRace.Size = new System.Drawing.Size(148, 14);
            this.labelRace.TabIndex = 0;
            this.labelRace.Text = "Race";
            this.labelRace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDateFirstVisit
            // 
            this.labelDateFirstVisit.Location = new System.Drawing.Point(12, 29);
            this.labelDateFirstVisit.Name = "labelDateFirstVisit";
            this.labelDateFirstVisit.Size = new System.Drawing.Size(142, 14);
            this.labelDateFirstVisit.TabIndex = 0;
            this.labelDateFirstVisit.Text = "Date of First Visit";
            this.labelDateFirstVisit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPutInInsPlan
            // 
            this.labelPutInInsPlan.Location = new System.Drawing.Point(341, 284);
            this.labelPutInInsPlan.Name = "labelPutInInsPlan";
            this.labelPutInInsPlan.Size = new System.Drawing.Size(100, 17);
            this.labelPutInInsPlan.TabIndex = 0;
            this.labelPutInInsPlan.Text = "(put in InsPlan too)";
            this.labelPutInInsPlan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboClinic
            // 
            this.comboClinic.ForceShowUnassigned = true;
            this.comboClinic.IncludeUnassigned = true;
            this.comboClinic.Location = new System.Drawing.Point(130, 572);
            this.comboClinic.Name = "comboClinic";
            this.comboClinic.Size = new System.Drawing.Size(265, 21);
            this.comboClinic.TabIndex = 24;
            this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // textTrophyFolder
            // 
            this.textTrophyFolder.Location = new System.Drawing.Point(155, 46);
            this.textTrophyFolder.MaxLength = 30;
            this.textTrophyFolder.Name = "textTrophyFolder";
            this.textTrophyFolder.Size = new System.Drawing.Size(228, 20);
            this.textTrophyFolder.TabIndex = 22;
            this.textTrophyFolder.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelTrophyFolder
            // 
            this.labelTrophyFolder.Location = new System.Drawing.Point(12, 49);
            this.labelTrophyFolder.Name = "labelTrophyFolder";
            this.labelTrophyFolder.Size = new System.Drawing.Size(142, 14);
            this.labelTrophyFolder.TabIndex = 0;
            this.labelTrophyFolder.Text = "Trophy Folder";
            this.labelTrophyFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textWard
            // 
            this.textWard.Location = new System.Drawing.Point(155, 6);
            this.textWard.MaxLength = 100;
            this.textWard.Name = "textWard";
            this.textWard.Size = new System.Drawing.Size(50, 20);
            this.textWard.TabIndex = 25;
            this.textWard.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelWard
            // 
            this.labelWard.Location = new System.Drawing.Point(17, 9);
            this.labelWard.Name = "labelWard";
            this.labelWard.Size = new System.Drawing.Size(137, 14);
            this.labelWard.TabIndex = 0;
            this.labelWard.Text = "Ward";
            this.labelWard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelLanguage
            // 
            this.labelLanguage.Location = new System.Drawing.Point(12, 554);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(154, 14);
            this.labelLanguage.TabIndex = 0;
            this.labelLanguage.Text = "Language";
            this.labelLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboLanguage
            // 
            this.comboLanguage.BackColor = System.Drawing.SystemColors.Window;
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.Location = new System.Drawing.Point(167, 551);
            this.comboLanguage.MaxDropDownItems = 25;
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(228, 21);
            this.comboLanguage.TabIndex = 23;
            this.comboLanguage.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // comboContact
            // 
            this.comboContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboContact.FormattingEnabled = true;
            this.comboContact.Location = new System.Drawing.Point(167, 467);
            this.comboContact.MaxDropDownItems = 30;
            this.comboContact.Name = "comboContact";
            this.comboContact.Size = new System.Drawing.Size(228, 21);
            this.comboContact.TabIndex = 19;
            this.comboContact.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // labelContact
            // 
            this.labelContact.Location = new System.Drawing.Point(12, 470);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(154, 14);
            this.labelContact.TabIndex = 0;
            this.labelContact.Text = "Prefer Contact Method";
            this.labelContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboConfirm
            // 
            this.comboConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConfirm.FormattingEnabled = true;
            this.comboConfirm.Location = new System.Drawing.Point(167, 488);
            this.comboConfirm.MaxDropDownItems = 30;
            this.comboConfirm.Name = "comboConfirm";
            this.comboConfirm.Size = new System.Drawing.Size(228, 21);
            this.comboConfirm.TabIndex = 20;
            this.comboConfirm.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // labelConfirm
            // 
            this.labelConfirm.Location = new System.Drawing.Point(12, 491);
            this.labelConfirm.Name = "labelConfirm";
            this.labelConfirm.Size = new System.Drawing.Size(154, 14);
            this.labelConfirm.TabIndex = 0;
            this.labelConfirm.Text = "Prefer Confirm Method";
            this.labelConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboRecall
            // 
            this.comboRecall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRecall.FormattingEnabled = true;
            this.comboRecall.Location = new System.Drawing.Point(167, 530);
            this.comboRecall.MaxDropDownItems = 30;
            this.comboRecall.Name = "comboRecall";
            this.comboRecall.Size = new System.Drawing.Size(228, 21);
            this.comboRecall.TabIndex = 21;
            this.comboRecall.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // labelRecall
            // 
            this.labelRecall.Location = new System.Drawing.Point(12, 533);
            this.labelRecall.Name = "labelRecall";
            this.labelRecall.Size = new System.Drawing.Size(154, 14);
            this.labelRecall.TabIndex = 0;
            this.labelRecall.Text = "Prefer Recall Method";
            this.labelRecall.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelAdmitDate
            // 
            this.labelAdmitDate.Location = new System.Drawing.Point(206, 9);
            this.labelAdmitDate.Name = "labelAdmitDate";
            this.labelAdmitDate.Size = new System.Drawing.Size(78, 14);
            this.labelAdmitDate.TabIndex = 0;
            this.labelAdmitDate.Text = "Admit Date";
            this.labelAdmitDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textTitle
            // 
            this.textTitle.Location = new System.Drawing.Point(160, 108);
            this.textTitle.MaxLength = 100;
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(96, 20);
            this.textTitle.TabIndex = 4;
            this.textTitle.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(5, 111);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(154, 14);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Title (Mr., Ms.)";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(364, 157);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(114, 14);
            this.label41.TabIndex = 0;
            this.label41.Text = "Family Relationships";
            this.label41.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // listRelationships
            // 
            this.listRelationships.Location = new System.Drawing.Point(364, 172);
            this.listRelationships.Name = "listRelationships";
            this.listRelationships.Size = new System.Drawing.Size(114, 69);
            this.listRelationships.TabIndex = 0;
            this.listRelationships.TabStop = false;
            this.listRelationships.DoubleClick += new System.EventHandler(this.listRelationships_DoubleClick);
            // 
            // butAddGuardian
            // 
            this.butAddGuardian.Location = new System.Drawing.Point(364, 241);
            this.butAddGuardian.Name = "butAddGuardian";
            this.butAddGuardian.Size = new System.Drawing.Size(57, 22);
            this.butAddGuardian.TabIndex = 0;
            this.butAddGuardian.TabStop = false;
            this.butAddGuardian.Text = "Add";
            this.butAddGuardian.Click += new System.EventHandler(this.butAddGuardian_Click);
            // 
            // butGuardianDefaults
            // 
            this.butGuardianDefaults.Location = new System.Drawing.Point(421, 241);
            this.butGuardianDefaults.Name = "butGuardianDefaults";
            this.butGuardianDefaults.Size = new System.Drawing.Size(57, 22);
            this.butGuardianDefaults.TabIndex = 0;
            this.butGuardianDefaults.TabStop = false;
            this.butGuardianDefaults.Text = "Defaults";
            this.butGuardianDefaults.Click += new System.EventHandler(this.butGuardianDefaults_Click);
            // 
            // textAskToArriveEarly
            // 
            this.textAskToArriveEarly.Location = new System.Drawing.Point(160, 324);
            this.textAskToArriveEarly.MaxLength = 100;
            this.textAskToArriveEarly.Name = "textAskToArriveEarly";
            this.textAskToArriveEarly.Size = new System.Drawing.Size(36, 20);
            this.textAskToArriveEarly.TabIndex = 11;
            this.textAskToArriveEarly.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelAskToArriveEarly
            // 
            this.labelAskToArriveEarly.Location = new System.Drawing.Point(5, 327);
            this.labelAskToArriveEarly.Name = "labelAskToArriveEarly";
            this.labelAskToArriveEarly.Size = new System.Drawing.Size(154, 14);
            this.labelAskToArriveEarly.TabIndex = 0;
            this.labelAskToArriveEarly.Text = "Ask To Arrive Early";
            this.labelAskToArriveEarly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkArriveEarlySame
            // 
            this.checkArriveEarlySame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkArriveEarlySame.Location = new System.Drawing.Point(270, 326);
            this.checkArriveEarlySame.Name = "checkArriveEarlySame";
            this.checkArriveEarlySame.Size = new System.Drawing.Size(208, 17);
            this.checkArriveEarlySame.TabIndex = 0;
            this.checkArriveEarlySame.TabStop = false;
            this.checkArriveEarlySame.Text = "Same for entire family";
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(197, 327);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(68, 14);
            this.label43.TabIndex = 0;
            this.label43.Text = "(minutes)";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTextOk
            // 
            this.labelTextOk.Location = new System.Drawing.Point(295, 30);
            this.labelTextOk.Name = "labelTextOk";
            this.labelTextOk.Size = new System.Drawing.Size(64, 14);
            this.labelTextOk.TabIndex = 0;
            this.labelTextOk.Text = "Text OK";
            this.labelTextOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listTextOk
            // 
            this.listTextOk.ColumnWidth = 30;
            this.listTextOk.Items.AddRange(new object[] {
            "??",
            "Yes",
            "No"});
            this.listTextOk.Location = new System.Drawing.Point(359, 29);
            this.listTextOk.MultiColumn = true;
            this.listTextOk.Name = "listTextOk";
            this.listTextOk.Size = new System.Drawing.Size(95, 17);
            this.listTextOk.TabIndex = 0;
            this.listTextOk.TabStop = false;
            this.listTextOk.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            // 
            // textMotherMaidenFname
            // 
            this.textMotherMaidenFname.Location = new System.Drawing.Point(155, 6);
            this.textMotherMaidenFname.MaxLength = 100;
            this.textMotherMaidenFname.Name = "textMotherMaidenFname";
            this.textMotherMaidenFname.Size = new System.Drawing.Size(228, 20);
            this.textMotherMaidenFname.TabIndex = 7;
            this.textMotherMaidenFname.Visible = false;
            this.textMotherMaidenFname.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelMotherMaidenFname
            // 
            this.labelMotherMaidenFname.Location = new System.Drawing.Point(6, 9);
            this.labelMotherMaidenFname.Name = "labelMotherMaidenFname";
            this.labelMotherMaidenFname.Size = new System.Drawing.Size(148, 14);
            this.labelMotherMaidenFname.TabIndex = 0;
            this.labelMotherMaidenFname.Text = "Mother\'s Maiden First Name";
            this.labelMotherMaidenFname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelMotherMaidenFname.Visible = false;
            // 
            // textMotherMaidenLname
            // 
            this.textMotherMaidenLname.Location = new System.Drawing.Point(155, 26);
            this.textMotherMaidenLname.MaxLength = 100;
            this.textMotherMaidenLname.Name = "textMotherMaidenLname";
            this.textMotherMaidenLname.Size = new System.Drawing.Size(228, 20);
            this.textMotherMaidenLname.TabIndex = 8;
            this.textMotherMaidenLname.Visible = false;
            this.textMotherMaidenLname.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelMotherMaidenLname
            // 
            this.labelMotherMaidenLname.Location = new System.Drawing.Point(6, 29);
            this.labelMotherMaidenLname.Name = "labelMotherMaidenLname";
            this.labelMotherMaidenLname.Size = new System.Drawing.Size(148, 14);
            this.labelMotherMaidenLname.TabIndex = 0;
            this.labelMotherMaidenLname.Text = "Mother\'s Maiden Last Name";
            this.labelMotherMaidenLname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelMotherMaidenLname.Visible = false;
            // 
            // labelDeceased
            // 
            this.labelDeceased.Location = new System.Drawing.Point(6, 49);
            this.labelDeceased.Name = "labelDeceased";
            this.labelDeceased.Size = new System.Drawing.Size(148, 14);
            this.labelDeceased.TabIndex = 0;
            this.labelDeceased.Text = "Date Time Deceased";
            this.labelDeceased.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDeceased.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxApptTexts);
            this.groupBox3.Controls.Add(this.butShortCodeOptIn);
            this.groupBox3.Controls.Add(this.labelApptTexts);
            this.groupBox3.Controls.Add(this.labelABC);
            this.groupBox3.Controls.Add(this.butEmailEdit);
            this.groupBox3.Controls.Add(this.checkEmailPhoneSame);
            this.groupBox3.Controls.Add(this.labelWirelessPhone);
            this.groupBox3.Controls.Add(this.labelWkPhone);
            this.groupBox3.Controls.Add(this.textWkPhone);
            this.groupBox3.Controls.Add(this.labelTextOk);
            this.groupBox3.Controls.Add(this.listTextOk);
            this.groupBox3.Controls.Add(this.textWirelessPhone);
            this.groupBox3.Controls.Add(this.textEmail);
            this.groupBox3.Controls.Add(this.labelEmail);
            this.groupBox3.Location = new System.Drawing.Point(8, 368);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(474, 96);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Email and Phone";
            // 
            // listBoxApptTexts
            // 
            this.listBoxApptTexts.ColumnWidth = 30;
            this.listBoxApptTexts.Location = new System.Drawing.Point(359, 49);
            this.listBoxApptTexts.MultiColumn = true;
            this.listBoxApptTexts.Name = "listBoxApptTexts";
            this.listBoxApptTexts.Size = new System.Drawing.Size(49, 17);
            this.listBoxApptTexts.TabIndex = 59;
            this.listBoxApptTexts.TabStop = false;
            // 
            // butShortCodeOptIn
            // 
            this.butShortCodeOptIn.Location = new System.Drawing.Point(410, 47);
            this.butShortCodeOptIn.Name = "butShortCodeOptIn";
            this.butShortCodeOptIn.Size = new System.Drawing.Size(23, 21);
            this.butShortCodeOptIn.TabIndex = 58;
            this.butShortCodeOptIn.Text = "Info";
            this.butShortCodeOptIn.UseVisualStyleBackColor = true;
            this.butShortCodeOptIn.Click += new System.EventHandler(this.butShortCodeOptIn_Click);
            // 
            // labelApptTexts
            // 
            this.labelApptTexts.Location = new System.Drawing.Point(295, 49);
            this.labelApptTexts.Name = "labelApptTexts";
            this.labelApptTexts.Size = new System.Drawing.Size(64, 14);
            this.labelApptTexts.TabIndex = 57;
            this.labelApptTexts.Text = "Appt Texts";
            this.labelApptTexts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelABC
            // 
            this.labelABC.Location = new System.Drawing.Point(434, 74);
            this.labelABC.Name = "labelABC";
            this.labelABC.Size = new System.Drawing.Size(37, 19);
            this.labelABC.TabIndex = 56;
            this.labelABC.Text = "(a,b,c)";
            // 
            // butEmailEdit
            // 
            this.butEmailEdit.Location = new System.Drawing.Point(410, 70);
            this.butEmailEdit.Name = "butEmailEdit";
            this.butEmailEdit.Size = new System.Drawing.Size(23, 21);
            this.butEmailEdit.TabIndex = 56;
            this.butEmailEdit.Text = "...";
            this.butEmailEdit.UseVisualStyleBackColor = true;
            this.butEmailEdit.Click += new System.EventHandler(this.butEmailEdit_Click);
            // 
            // checkEmailPhoneSame
            // 
            this.checkEmailPhoneSame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEmailPhoneSame.Location = new System.Drawing.Point(159, 12);
            this.checkEmailPhoneSame.Name = "checkEmailPhoneSame";
            this.checkEmailPhoneSame.Size = new System.Drawing.Size(254, 17);
            this.checkEmailPhoneSame.TabIndex = 0;
            this.checkEmailPhoneSame.TabStop = false;
            this.checkEmailPhoneSame.Text = "Same for entire family";
            // 
            // labelEmail
            // 
            this.labelEmail.Location = new System.Drawing.Point(6, 74);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(152, 14);
            this.labelEmail.TabIndex = 0;
            this.labelEmail.Text = "E-mail Addresses";
            this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBillProv
            // 
            this.groupBillProv.Controls.Add(this.checkBillProvSame);
            this.groupBillProv.Controls.Add(this.checkSuperBilling);
            this.groupBillProv.Controls.Add(this.butPickSecondary);
            this.groupBillProv.Controls.Add(this.comboBillType);
            this.groupBillProv.Controls.Add(this.butPickPrimary);
            this.groupBillProv.Controls.Add(this.labelBillType);
            this.groupBillProv.Controls.Add(this.labelFeeSched);
            this.groupBillProv.Controls.Add(this.textCreditType);
            this.groupBillProv.Controls.Add(this.labelSecProv);
            this.groupBillProv.Controls.Add(this.comboFeeSched);
            this.groupBillProv.Controls.Add(this.labelCreditType);
            this.groupBillProv.Controls.Add(this.labelPriProv);
            this.groupBillProv.Controls.Add(this.comboPriProv);
            this.groupBillProv.Controls.Add(this.comboSecProv);
            this.groupBillProv.Location = new System.Drawing.Point(488, 250);
            this.groupBillProv.Name = "groupBillProv";
            this.groupBillProv.Size = new System.Drawing.Size(474, 139);
            this.groupBillProv.TabIndex = 31;
            this.groupBillProv.TabStop = false;
            this.groupBillProv.Text = "Billing and Provider(s)";
            // 
            // checkBillProvSame
            // 
            this.checkBillProvSame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBillProvSame.Location = new System.Drawing.Point(159, 12);
            this.checkBillProvSame.Name = "checkBillProvSame";
            this.checkBillProvSame.Size = new System.Drawing.Size(165, 17);
            this.checkBillProvSame.TabIndex = 0;
            this.checkBillProvSame.TabStop = false;
            this.checkBillProvSame.Text = "Same for entire family";
            this.checkBillProvSame.Click += new System.EventHandler(this.checkBillProvSame_Click);
            // 
            // checkSuperBilling
            // 
            this.checkSuperBilling.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkSuperBilling.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSuperBilling.Location = new System.Drawing.Point(181, 31);
            this.checkSuperBilling.Name = "checkSuperBilling";
            this.checkSuperBilling.Size = new System.Drawing.Size(176, 17);
            this.checkSuperBilling.TabIndex = 0;
            this.checkSuperBilling.TabStop = false;
            this.checkSuperBilling.Text = "Included in Super Family Billing";
            this.checkSuperBilling.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkSuperBilling.Visible = false;
            this.checkSuperBilling.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkSuperBilling_MouseDown);
            // 
            // butPickSecondary
            // 
            this.butPickSecondary.Location = new System.Drawing.Point(390, 93);
            this.butPickSecondary.Name = "butPickSecondary";
            this.butPickSecondary.Size = new System.Drawing.Size(23, 21);
            this.butPickSecondary.TabIndex = 6;
            this.butPickSecondary.Text = "...";
            this.butPickSecondary.Click += new System.EventHandler(this.butPickSecondary_Click);
            // 
            // comboBillType
            // 
            this.comboBillType.Location = new System.Drawing.Point(159, 51);
            this.comboBillType.Name = "comboBillType";
            this.comboBillType.Size = new System.Drawing.Size(229, 21);
            this.comboBillType.TabIndex = 2;
            this.comboBillType.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // butPickPrimary
            // 
            this.butPickPrimary.Location = new System.Drawing.Point(390, 72);
            this.butPickPrimary.Name = "butPickPrimary";
            this.butPickPrimary.Size = new System.Drawing.Size(23, 21);
            this.butPickPrimary.TabIndex = 4;
            this.butPickPrimary.Text = "...";
            this.butPickPrimary.Click += new System.EventHandler(this.butPickPrimary_Click);
            // 
            // labelBillType
            // 
            this.labelBillType.Location = new System.Drawing.Point(6, 54);
            this.labelBillType.Name = "labelBillType";
            this.labelBillType.Size = new System.Drawing.Size(152, 14);
            this.labelBillType.TabIndex = 39;
            this.labelBillType.Text = "Billing Type";
            this.labelBillType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFeeSched
            // 
            this.labelFeeSched.Location = new System.Drawing.Point(6, 117);
            this.labelFeeSched.Name = "labelFeeSched";
            this.labelFeeSched.Size = new System.Drawing.Size(152, 14);
            this.labelFeeSched.TabIndex = 35;
            this.labelFeeSched.Text = "Fee Schedule (rarely used)";
            this.labelFeeSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textCreditType
            // 
            this.textCreditType.Location = new System.Drawing.Point(159, 31);
            this.textCreditType.MaxLength = 1;
            this.textCreditType.Name = "textCreditType";
            this.textCreditType.Size = new System.Drawing.Size(18, 20);
            this.textCreditType.TabIndex = 1;
            this.textCreditType.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelSecProv
            // 
            this.labelSecProv.Location = new System.Drawing.Point(6, 96);
            this.labelSecProv.Name = "labelSecProv";
            this.labelSecProv.Size = new System.Drawing.Size(152, 14);
            this.labelSecProv.TabIndex = 36;
            this.labelSecProv.Text = "Secondary Provider";
            this.labelSecProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboFeeSched
            // 
            this.comboFeeSched.Location = new System.Drawing.Point(159, 114);
            this.comboFeeSched.Name = "comboFeeSched";
            this.comboFeeSched.Size = new System.Drawing.Size(229, 21);
            this.comboFeeSched.TabIndex = 7;
            this.comboFeeSched.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // labelCreditType
            // 
            this.labelCreditType.Location = new System.Drawing.Point(6, 34);
            this.labelCreditType.Name = "labelCreditType";
            this.labelCreditType.Size = new System.Drawing.Size(152, 14);
            this.labelCreditType.TabIndex = 38;
            this.labelCreditType.Text = "Credit Type";
            this.labelCreditType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPriProv
            // 
            this.labelPriProv.Location = new System.Drawing.Point(6, 75);
            this.labelPriProv.Name = "labelPriProv";
            this.labelPriProv.Size = new System.Drawing.Size(152, 14);
            this.labelPriProv.TabIndex = 37;
            this.labelPriProv.Text = "Primary Provider";
            this.labelPriProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboPriProv
            // 
            this.comboPriProv.Location = new System.Drawing.Point(159, 72);
            this.comboPriProv.Name = "comboPriProv";
            this.comboPriProv.Size = new System.Drawing.Size(229, 21);
            this.comboPriProv.TabIndex = 3;
            this.comboPriProv.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // comboSecProv
            // 
            this.comboSecProv.Location = new System.Drawing.Point(159, 93);
            this.comboSecProv.Name = "comboSecProv";
            this.comboSecProv.Size = new System.Drawing.Size(229, 21);
            this.comboSecProv.TabIndex = 5;
            this.comboSecProv.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // textReferredFrom
            // 
            this.textReferredFrom.Location = new System.Drawing.Point(167, 593);
            this.textReferredFrom.MaxLength = 30;
            this.textReferredFrom.Multiline = true;
            this.textReferredFrom.Name = "textReferredFrom";
            this.textReferredFrom.ReadOnly = true;
            this.textReferredFrom.Size = new System.Drawing.Size(228, 20);
            this.textReferredFrom.TabIndex = 0;
            this.textReferredFrom.TabStop = false;
            this.textReferredFrom.WordWrap = false;
            this.textReferredFrom.DoubleClick += new System.EventHandler(this.textReferredFrom_DoubleClick);
            // 
            // butReferredFrom
            // 
            this.butReferredFrom.Location = new System.Drawing.Point(395, 593);
            this.butReferredFrom.Name = "butReferredFrom";
            this.butReferredFrom.Size = new System.Drawing.Size(23, 20);
            this.butReferredFrom.TabIndex = 27;
            this.butReferredFrom.Text = "...";
            this.butReferredFrom.Click += new System.EventHandler(this.butReferredFrom_Click);
            // 
            // labelReferredFrom
            // 
            this.labelReferredFrom.Location = new System.Drawing.Point(12, 596);
            this.labelReferredFrom.Name = "labelReferredFrom";
            this.labelReferredFrom.Size = new System.Drawing.Size(154, 14);
            this.labelReferredFrom.TabIndex = 50;
            this.labelReferredFrom.Text = "Referred From";
            this.labelReferredFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textMedicaidState
            // 
            this.textMedicaidState.Location = new System.Drawing.Point(271, 284);
            this.textMedicaidState.MaxLength = 100;
            this.textMedicaidState.Name = "textMedicaidState";
            this.textMedicaidState.Size = new System.Drawing.Size(61, 20);
            this.textMedicaidState.TabIndex = 8;
            this.textMedicaidState.TextChanged += new System.EventHandler(this.textMedicaidState_TextChanged);
            this.textMedicaidState.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textMedicaidState_KeyUp);
            this.textMedicaidState.Leave += new System.EventHandler(this.textMedicaidState_Leave);
            // 
            // labelRequiredField
            // 
            this.labelRequiredField.Location = new System.Drawing.Point(604, 671);
            this.labelRequiredField.Name = "labelRequiredField";
            this.labelRequiredField.Size = new System.Drawing.Size(180, 14);
            this.labelRequiredField.TabIndex = 9;
            this.labelRequiredField.Text = "* Indicates Required Field";
            this.labelRequiredField.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelRequiredField.Visible = false;
            // 
            // checkRestrictSched
            // 
            this.checkRestrictSched.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkRestrictSched.Location = new System.Drawing.Point(160, 9);
            this.checkRestrictSched.Name = "checkRestrictSched";
            this.checkRestrictSched.Size = new System.Drawing.Size(228, 17);
            this.checkRestrictSched.TabIndex = 0;
            this.checkRestrictSched.TabStop = false;
            this.checkRestrictSched.Text = "Appointment scheduling is restricted";
            // 
            // tabControlPatInfo
            // 
            this.tabControlPatInfo.Controls.Add(this.tabPublicHealth);
            this.tabControlPatInfo.Controls.Add(this.tabHospitals);
            this.tabControlPatInfo.Controls.Add(this.tabOther);
            this.tabControlPatInfo.Controls.Add(this.tabICE);
            this.tabControlPatInfo.Controls.Add(this.tabEHR);
            this.tabControlPatInfo.Location = new System.Drawing.Point(488, 395);
            this.tabControlPatInfo.Name = "tabControlPatInfo";
            this.tabControlPatInfo.SelectedIndex = 0;
            this.tabControlPatInfo.Size = new System.Drawing.Size(465, 264);
            this.tabControlPatInfo.TabIndex = 17;
            // 
            // tabPublicHealth
            // 
            this.tabPublicHealth.BackColor = System.Drawing.SystemColors.Control;
            this.tabPublicHealth.Controls.Add(this.comboBoxMultiRace);
            this.tabPublicHealth.Controls.Add(this.comboEthnicity);
            this.tabPublicHealth.Controls.Add(this.textEthnicity);
            this.tabPublicHealth.Controls.Add(this.textRace);
            this.tabPublicHealth.Controls.Add(this.butRaceEthnicity);
            this.tabPublicHealth.Controls.Add(this.labelSexOrientation);
            this.tabPublicHealth.Controls.Add(this.labelSpecifySexOrientation);
            this.tabPublicHealth.Controls.Add(this.labelGenderIdentity);
            this.tabPublicHealth.Controls.Add(this.labelSpecifyGender);
            this.tabPublicHealth.Controls.Add(this.textSpecifyGender);
            this.tabPublicHealth.Controls.Add(this.textSpecifySexOrientation);
            this.tabPublicHealth.Controls.Add(this.comboGenderIdentity);
            this.tabPublicHealth.Controls.Add(this.comboSexOrientation);
            this.tabPublicHealth.Controls.Add(this.labelRace);
            this.tabPublicHealth.Controls.Add(this.labelEthnicity);
            this.tabPublicHealth.Controls.Add(this.labelCounty);
            this.tabPublicHealth.Controls.Add(this.butClearResponsParty);
            this.tabPublicHealth.Controls.Add(this.labelSite);
            this.tabPublicHealth.Controls.Add(this.butPickResponsParty);
            this.tabPublicHealth.Controls.Add(this.labelGradeLevel);
            this.tabPublicHealth.Controls.Add(this.textResponsParty);
            this.tabPublicHealth.Controls.Add(this.textCounty);
            this.tabPublicHealth.Controls.Add(this.labelResponsParty);
            this.tabPublicHealth.Controls.Add(this.labelUrgency);
            this.tabPublicHealth.Controls.Add(this.butPickSite);
            this.tabPublicHealth.Controls.Add(this.textSite);
            this.tabPublicHealth.Controls.Add(this.comboUrgency);
            this.tabPublicHealth.Controls.Add(this.comboGradeLevel);
            this.tabPublicHealth.Location = new System.Drawing.Point(4, 22);
            this.tabPublicHealth.Name = "tabPublicHealth";
            this.tabPublicHealth.Padding = new System.Windows.Forms.Padding(3);
            this.tabPublicHealth.Size = new System.Drawing.Size(457, 238);
            this.tabPublicHealth.TabIndex = 0;
            this.tabPublicHealth.Text = "Public Health";
            // 
            // comboBoxMultiRace
            // 
            this.comboBoxMultiRace.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxMultiRace.Location = new System.Drawing.Point(155, 6);
            this.comboBoxMultiRace.Name = "comboBoxMultiRace";
            this.comboBoxMultiRace.SelectionModeMulti = true;
            this.comboBoxMultiRace.Size = new System.Drawing.Size(229, 21);
            this.comboBoxMultiRace.TabIndex = 51;
            this.comboBoxMultiRace.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiRace_SelectionChangeCommitted);
            // 
            // comboEthnicity
            // 
            this.comboEthnicity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEthnicity.Location = new System.Drawing.Point(155, 27);
            this.comboEthnicity.MaxDropDownItems = 20;
            this.comboEthnicity.Name = "comboEthnicity";
            this.comboEthnicity.Size = new System.Drawing.Size(229, 21);
            this.comboEthnicity.TabIndex = 52;
            this.comboEthnicity.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SelectionChangeCommited);
            // 
            // textEthnicity
            // 
            this.textEthnicity.AcceptsReturn = true;
            this.textEthnicity.Location = new System.Drawing.Point(155, 28);
            this.textEthnicity.Name = "textEthnicity";
            this.textEthnicity.ReadOnly = true;
            this.textEthnicity.Size = new System.Drawing.Size(229, 20);
            this.textEthnicity.TabIndex = 20;
            this.textEthnicity.TabStop = false;
            // 
            // textRace
            // 
            this.textRace.AcceptsReturn = true;
            this.textRace.Location = new System.Drawing.Point(155, 8);
            this.textRace.Name = "textRace";
            this.textRace.ReadOnly = true;
            this.textRace.Size = new System.Drawing.Size(229, 20);
            this.textRace.TabIndex = 19;
            this.textRace.TabStop = false;
            // 
            // butRaceEthnicity
            // 
            this.butRaceEthnicity.Location = new System.Drawing.Point(386, 17);
            this.butRaceEthnicity.Name = "butRaceEthnicity";
            this.butRaceEthnicity.Size = new System.Drawing.Size(23, 21);
            this.butRaceEthnicity.TabIndex = 18;
            this.butRaceEthnicity.Text = "...";
            this.butRaceEthnicity.Click += new System.EventHandler(this.butRaceEthnicity_Click);
            // 
            // labelSexOrientation
            // 
            this.labelSexOrientation.Location = new System.Drawing.Point(6, 153);
            this.labelSexOrientation.Name = "labelSexOrientation";
            this.labelSexOrientation.Size = new System.Drawing.Size(148, 14);
            this.labelSexOrientation.TabIndex = 17;
            this.labelSexOrientation.Text = "Sexual Orientation";
            this.labelSexOrientation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSpecifySexOrientation
            // 
            this.labelSpecifySexOrientation.Location = new System.Drawing.Point(6, 172);
            this.labelSpecifySexOrientation.Name = "labelSpecifySexOrientation";
            this.labelSpecifySexOrientation.Size = new System.Drawing.Size(148, 14);
            this.labelSpecifySexOrientation.TabIndex = 16;
            this.labelSpecifySexOrientation.Text = "Please Specify";
            this.labelSpecifySexOrientation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSpecifySexOrientation.Visible = false;
            // 
            // labelGenderIdentity
            // 
            this.labelGenderIdentity.Location = new System.Drawing.Point(6, 193);
            this.labelGenderIdentity.Name = "labelGenderIdentity";
            this.labelGenderIdentity.Size = new System.Drawing.Size(148, 14);
            this.labelGenderIdentity.TabIndex = 15;
            this.labelGenderIdentity.Text = "Gender Identity";
            this.labelGenderIdentity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSpecifyGender
            // 
            this.labelSpecifyGender.Location = new System.Drawing.Point(6, 214);
            this.labelSpecifyGender.Name = "labelSpecifyGender";
            this.labelSpecifyGender.Size = new System.Drawing.Size(148, 14);
            this.labelSpecifyGender.TabIndex = 14;
            this.labelSpecifyGender.Text = "Please Specify";
            this.labelSpecifyGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSpecifyGender.Visible = false;
            // 
            // textSpecifyGender
            // 
            this.textSpecifyGender.AcceptsReturn = true;
            this.textSpecifyGender.Location = new System.Drawing.Point(155, 212);
            this.textSpecifyGender.Name = "textSpecifyGender";
            this.textSpecifyGender.Size = new System.Drawing.Size(229, 20);
            this.textSpecifyGender.TabIndex = 13;
            this.textSpecifyGender.Visible = false;
            // 
            // textSpecifySexOrientation
            // 
            this.textSpecifySexOrientation.AcceptsReturn = true;
            this.textSpecifySexOrientation.Location = new System.Drawing.Point(155, 171);
            this.textSpecifySexOrientation.Name = "textSpecifySexOrientation";
            this.textSpecifySexOrientation.Size = new System.Drawing.Size(229, 20);
            this.textSpecifySexOrientation.TabIndex = 12;
            this.textSpecifySexOrientation.Visible = false;
            // 
            // comboGenderIdentity
            // 
            this.comboGenderIdentity.BackColor = System.Drawing.SystemColors.Window;
            this.comboGenderIdentity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGenderIdentity.Location = new System.Drawing.Point(155, 191);
            this.comboGenderIdentity.MaxDropDownItems = 25;
            this.comboGenderIdentity.Name = "comboGenderIdentity";
            this.comboGenderIdentity.Size = new System.Drawing.Size(229, 21);
            this.comboGenderIdentity.TabIndex = 11;
            this.comboGenderIdentity.SelectedIndexChanged += new System.EventHandler(this.comboGenderIdentity_SelectedIndexChanged);
            // 
            // comboSexOrientation
            // 
            this.comboSexOrientation.BackColor = System.Drawing.SystemColors.Window;
            this.comboSexOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSexOrientation.Location = new System.Drawing.Point(155, 150);
            this.comboSexOrientation.MaxDropDownItems = 25;
            this.comboSexOrientation.Name = "comboSexOrientation";
            this.comboSexOrientation.Size = new System.Drawing.Size(229, 21);
            this.comboSexOrientation.TabIndex = 10;
            this.comboSexOrientation.SelectedIndexChanged += new System.EventHandler(this.comboSexOrientation_SelectedIndexChanged);
            // 
            // tabHospitals
            // 
            this.tabHospitals.BackColor = System.Drawing.SystemColors.Control;
            this.tabHospitals.Controls.Add(this.textWard);
            this.tabHospitals.Controls.Add(this.labelWard);
            this.tabHospitals.Controls.Add(this.labelAdmitDate);
            this.tabHospitals.Controls.Add(this.odDatePickerAdmitDate);
            this.tabHospitals.Location = new System.Drawing.Point(4, 22);
            this.tabHospitals.Name = "tabHospitals";
            this.tabHospitals.Padding = new System.Windows.Forms.Padding(3);
            this.tabHospitals.Size = new System.Drawing.Size(457, 238);
            this.tabHospitals.TabIndex = 1;
            this.tabHospitals.Text = "Hospitals";
            // 
            // odDatePickerAdmitDate
            // 
            this.odDatePickerAdmitDate.BackColor = System.Drawing.Color.Transparent;
            this.odDatePickerAdmitDate.Location = new System.Drawing.Point(223, 6);
            this.odDatePickerAdmitDate.Name = "odDatePickerAdmitDate";
            this.odDatePickerAdmitDate.Size = new System.Drawing.Size(227, 23);
            this.odDatePickerAdmitDate.TabIndex = 27;
            // 
            // tabOther
            // 
            this.tabOther.BackColor = System.Drawing.SystemColors.Control;
            this.tabOther.Controls.Add(this.butViewSSN);
            this.tabOther.Controls.Add(this.checkDoseSpotConsent);
            this.tabOther.Controls.Add(this.checkBoxSignedTil);
            this.tabOther.Controls.Add(this.textSSN);
            this.tabOther.Controls.Add(this.labelSSN);
            this.tabOther.Controls.Add(this.labelDateFirstVisit);
            this.tabOther.Controls.Add(this.textTrophyFolder);
            this.tabOther.Controls.Add(this.labelTrophyFolder);
            this.tabOther.Controls.Add(this.groupBox2);
            this.tabOther.Controls.Add(this.odDatePickerDateFirstVisit);
            this.tabOther.Location = new System.Drawing.Point(4, 22);
            this.tabOther.Name = "tabOther";
            this.tabOther.Padding = new System.Windows.Forms.Padding(3);
            this.tabOther.Size = new System.Drawing.Size(457, 238);
            this.tabOther.TabIndex = 2;
            this.tabOther.Text = "Other";
            // 
            // butViewSSN
            // 
            this.butViewSSN.Enabled = false;
            this.butViewSSN.Location = new System.Drawing.Point(259, 6);
            this.butViewSSN.Name = "butViewSSN";
            this.butViewSSN.Size = new System.Drawing.Size(56, 21);
            this.butViewSSN.TabIndex = 40;
            this.butViewSSN.Text = "View";
            this.butViewSSN.Visible = false;
            this.butViewSSN.Click += new System.EventHandler(this.butViewSSN_Click);
            // 
            // checkDoseSpotConsent
            // 
            this.checkDoseSpotConsent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkDoseSpotConsent.Location = new System.Drawing.Point(13, 181);
            this.checkDoseSpotConsent.Name = "checkDoseSpotConsent";
            this.checkDoseSpotConsent.Size = new System.Drawing.Size(427, 17);
            this.checkDoseSpotConsent.TabIndex = 30;
            this.checkDoseSpotConsent.TabStop = false;
            this.checkDoseSpotConsent.Text = "DoseSpot Access Medication History Consent";
            this.checkDoseSpotConsent.Visible = false;
            // 
            // checkBoxSignedTil
            // 
            this.checkBoxSignedTil.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxSignedTil.Location = new System.Drawing.Point(13, 162);
            this.checkBoxSignedTil.Name = "checkBoxSignedTil";
            this.checkBoxSignedTil.Size = new System.Drawing.Size(193, 17);
            this.checkBoxSignedTil.TabIndex = 29;
            this.checkBoxSignedTil.TabStop = false;
            this.checkBoxSignedTil.Text = "Signed Truth in Lending";
            // 
            // odDatePickerDateFirstVisit
            // 
            this.odDatePickerDateFirstVisit.BackColor = System.Drawing.Color.Transparent;
            this.odDatePickerDateFirstVisit.Location = new System.Drawing.Point(92, 25);
            this.odDatePickerDateFirstVisit.Name = "odDatePickerDateFirstVisit";
            this.odDatePickerDateFirstVisit.Size = new System.Drawing.Size(227, 23);
            this.odDatePickerDateFirstVisit.TabIndex = 41;
            // 
            // tabICE
            // 
            this.tabICE.BackColor = System.Drawing.SystemColors.Control;
            this.tabICE.Controls.Add(this.labelEmergencyPhone);
            this.tabICE.Controls.Add(this.textIcePhone);
            this.tabICE.Controls.Add(this.textIceName);
            this.tabICE.Controls.Add(this.labelEmergencyName);
            this.tabICE.Location = new System.Drawing.Point(4, 22);
            this.tabICE.Name = "tabICE";
            this.tabICE.Padding = new System.Windows.Forms.Padding(3);
            this.tabICE.Size = new System.Drawing.Size(457, 238);
            this.tabICE.TabIndex = 4;
            this.tabICE.Text = "Emergency Contact";
            // 
            // labelEmergencyPhone
            // 
            this.labelEmergencyPhone.Location = new System.Drawing.Point(6, 29);
            this.labelEmergencyPhone.Name = "labelEmergencyPhone";
            this.labelEmergencyPhone.Size = new System.Drawing.Size(148, 14);
            this.labelEmergencyPhone.TabIndex = 3;
            this.labelEmergencyPhone.Text = "Emergency Phone";
            this.labelEmergencyPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textIcePhone
            // 
            this.textIcePhone.Location = new System.Drawing.Point(155, 26);
            this.textIcePhone.MaxLength = 30;
            this.textIcePhone.Name = "textIcePhone";
            this.textIcePhone.Size = new System.Drawing.Size(135, 20);
            this.textIcePhone.TabIndex = 4;
            this.textIcePhone.TextChanged += new System.EventHandler(this.textAnyPhoneNumber_TextChanged);
            this.textIcePhone.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // textIceName
            // 
            this.textIceName.Location = new System.Drawing.Point(155, 6);
            this.textIceName.MaxLength = 100;
            this.textIceName.Name = "textIceName";
            this.textIceName.Size = new System.Drawing.Size(228, 20);
            this.textIceName.TabIndex = 1;
            this.textIceName.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // labelEmergencyName
            // 
            this.labelEmergencyName.Location = new System.Drawing.Point(6, 9);
            this.labelEmergencyName.Name = "labelEmergencyName";
            this.labelEmergencyName.Size = new System.Drawing.Size(148, 14);
            this.labelEmergencyName.TabIndex = 2;
            this.labelEmergencyName.Text = "Emergency Name";
            this.labelEmergencyName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabEHR
            // 
            this.tabEHR.BackColor = System.Drawing.SystemColors.Control;
            this.tabEHR.Controls.Add(this.butClearDateTimeDeceased);
            this.tabEHR.Controls.Add(this.dateTimePickerDateDeceased);
            this.tabEHR.Controls.Add(this.textMotherMaidenFname);
            this.tabEHR.Controls.Add(this.labelMotherMaidenFname);
            this.tabEHR.Controls.Add(this.labelMotherMaidenLname);
            this.tabEHR.Controls.Add(this.textMotherMaidenLname);
            this.tabEHR.Controls.Add(this.labelDeceased);
            this.tabEHR.Location = new System.Drawing.Point(4, 22);
            this.tabEHR.Name = "tabEHR";
            this.tabEHR.Padding = new System.Windows.Forms.Padding(3);
            this.tabEHR.Size = new System.Drawing.Size(457, 238);
            this.tabEHR.TabIndex = 3;
            this.tabEHR.Text = "EHR Misc";
            // 
            // butClearDateTimeDeceased
            // 
            this.butClearDateTimeDeceased.Location = new System.Drawing.Point(386, 47);
            this.butClearDateTimeDeceased.Name = "butClearDateTimeDeceased";
            this.butClearDateTimeDeceased.Size = new System.Drawing.Size(40, 20);
            this.butClearDateTimeDeceased.TabIndex = 10;
            this.butClearDateTimeDeceased.Text = "clear";
            this.butClearDateTimeDeceased.UseVisualStyleBackColor = true;
            this.butClearDateTimeDeceased.Click += new System.EventHandler(this.butClearDateTimeDeceased_Click);
            // 
            // dateTimePickerDateDeceased
            // 
            this.dateTimePickerDateDeceased.Checked = false;
            this.dateTimePickerDateDeceased.CustomFormat = "ddddMM/dd/yyyy hh:mm tt";
            this.dateTimePickerDateDeceased.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDateDeceased.Location = new System.Drawing.Point(155, 46);
            this.dateTimePickerDateDeceased.Name = "dateTimePickerDateDeceased";
            this.dateTimePickerDateDeceased.Size = new System.Drawing.Size(228, 20);
            this.dateTimePickerDateDeceased.TabIndex = 9;
            this.dateTimePickerDateDeceased.Value = new System.DateTime(2021, 4, 15, 12, 40, 0, 0);
            this.dateTimePickerDateDeceased.ValueChanged += new System.EventHandler(this.dateTimePickerDateDeceased_ValueChanged);
            // 
            // comboSpecialty
            // 
            this.comboSpecialty.Location = new System.Drawing.Point(167, 613);
            this.comboSpecialty.Name = "comboSpecialty";
            this.comboSpecialty.Size = new System.Drawing.Size(228, 21);
            this.comboSpecialty.TabIndex = 52;
            // 
            // labelSpecialty
            // 
            this.labelSpecialty.Location = new System.Drawing.Point(12, 616);
            this.labelSpecialty.Name = "labelSpecialty";
            this.labelSpecialty.Size = new System.Drawing.Size(154, 14);
            this.labelSpecialty.TabIndex = 51;
            this.labelSpecialty.Text = "Specialty";
            this.labelSpecialty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboExcludeECR
            // 
            this.comboExcludeECR.BackColor = System.Drawing.SystemColors.Window;
            this.comboExcludeECR.Location = new System.Drawing.Point(167, 509);
            this.comboExcludeECR.Name = "comboExcludeECR";
            this.comboExcludeECR.SelectionModeMulti = true;
            this.comboExcludeECR.Size = new System.Drawing.Size(228, 21);
            this.comboExcludeECR.TabIndex = 53;
            // 
            // labelExcludeECR
            // 
            this.labelExcludeECR.Location = new System.Drawing.Point(0, 510);
            this.labelExcludeECR.Name = "labelExcludeECR";
            this.labelExcludeECR.Size = new System.Drawing.Size(165, 16);
            this.labelExcludeECR.TabIndex = 54;
            this.labelExcludeECR.Text = "Exclude Automated Msgs";
            this.labelExcludeECR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butViewBirthdate
            // 
            this.butViewBirthdate.Location = new System.Drawing.Point(271, 262);
            this.butViewBirthdate.Name = "butViewBirthdate";
            this.butViewBirthdate.Size = new System.Drawing.Size(62, 21);
            this.butViewBirthdate.TabIndex = 55;
            this.butViewBirthdate.Text = "View";
            this.butViewBirthdate.Visible = false;
            this.butViewBirthdate.Click += new System.EventHandler(this.butViewBirthdate_Click);
            // 
            // odDatePickerBirthDate
            // 
            this.odDatePickerBirthDate.BackColor = System.Drawing.Color.Transparent;
            this.odDatePickerBirthDate.Location = new System.Drawing.Point(97, 263);
            this.odDatePickerBirthDate.Name = "odDatePickerBirthDate";
            this.odDatePickerBirthDate.Size = new System.Drawing.Size(218, 23);
            this.odDatePickerBirthDate.TabIndex = 6;
            this.odDatePickerBirthDate.Leave += new System.EventHandler(this.odDatePickerBirthDate_Validated);
            // 
            // FormPatientEdit
            // 
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(974, 696);
            this.Controls.Add(this.butViewBirthdate);
            this.Controls.Add(this.textAge);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textMedicaidState);
            this.Controls.Add(this.textMedicaidID);
            this.Controls.Add(this.labelBirthdate);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.labelExcludeECR);
            this.Controls.Add(this.comboExcludeECR);
            this.Controls.Add(this.comboSpecialty);
            this.Controls.Add(this.labelSpecialty);
            this.Controls.Add(this.tabControlPatInfo);
            this.Controls.Add(this.checkRestrictSched);
            this.Controls.Add(this.labelRequiredField);
            this.Controls.Add(this.labelReferredFrom);
            this.Controls.Add(this.butReferredFrom);
            this.Controls.Add(this.textReferredFrom);
            this.Controls.Add(this.groupBillProv);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkArriveEarlySame);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.textAskToArriveEarly);
            this.Controls.Add(this.labelAskToArriveEarly);
            this.Controls.Add(this.butGuardianDefaults);
            this.Controls.Add(this.butAddGuardian);
            this.Controls.Add(this.listRelationships);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.textTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.comboRecall);
            this.Controls.Add(this.labelRecall);
            this.Controls.Add(this.comboConfirm);
            this.Controls.Add(this.labelConfirm);
            this.Controls.Add(this.comboContact);
            this.Controls.Add(this.labelContact);
            this.Controls.Add(this.comboLanguage);
            this.Controls.Add(this.labelLanguage);
            this.Controls.Add(this.comboClinic);
            this.Controls.Add(this.labelPutInInsPlan);
            this.Controls.Add(this.textEmployer);
            this.Controls.Add(this.textPatNum);
            this.Controls.Add(this.textChartNumber);
            this.Controls.Add(this.textSalutation);
            this.Controls.Add(this.textPreferred);
            this.Controls.Add(this.textMiddleI);
            this.Controls.Add(this.textFName);
            this.Controls.Add(this.textLName);
            this.Controls.Add(this.butAuto);
            this.Controls.Add(this.labelEmployer);
            this.Controls.Add(this.labelGender);
            this.Controls.Add(this.listPosition);
            this.Controls.Add(this.listGender);
            this.Controls.Add(this.listStatus);
            this.Controls.Add(this.labelMedicaidID);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.groupNotes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelChartNumber);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelSalutation);
            this.Controls.Add(this.labelPosition);
            this.Controls.Add(this.labelPreferredAndMiddleI);
            this.Controls.Add(this.labelFName);
            this.Controls.Add(this.labelLName);
            this.Controls.Add(this.odDatePickerBirthDate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPatientEdit";
            this.ShowInTaskbar = false;
            this.Text = "Edit Patient Information";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormPatientEdit_Closing);
            this.Load += new System.EventHandler(this.FormPatientEdit_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupNotes.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBillProv.ResumeLayout(false);
            this.groupBillProv.PerformLayout();
            this.tabControlPatInfo.ResumeLayout(false);
            this.tabPublicHealth.ResumeLayout(false);
            this.tabPublicHealth.PerformLayout();
            this.tabHospitals.ResumeLayout(false);
            this.tabHospitals.PerformLayout();
            this.tabOther.ResumeLayout(false);
            this.tabOther.PerformLayout();
            this.tabICE.ResumeLayout(false);
            this.tabICE.PerformLayout();
            this.tabEHR.ResumeLayout(false);
            this.tabEHR.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelLName;
		private System.Windows.Forms.Label labelFName;
		private System.Windows.Forms.Label labelPreferredAndMiddleI;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Label labelGender;
		private System.Windows.Forms.Label labelPosition;
		private System.Windows.Forms.Label labelBirthdate;
		private System.Windows.Forms.Label labelAddress;
		private System.Windows.Forms.Label labelAddress2;
		private System.Windows.Forms.Label labelHmPhone;
		private System.Windows.Forms.Label labelWkPhone;
		private System.Windows.Forms.Label labelWirelessPhone;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textMiddleI;
		private System.Windows.Forms.TextBox textPreferred;
		private System.Windows.Forms.TextBox textSSN;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.TextBox textAddress2;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.TextBox textState;
		private ValidPhone textHmPhone;
		private ValidPhone textWkPhone;
		private ValidPhone textWirelessPhone;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox textAge;
		private System.Windows.Forms.Label labelSalutation;
		private System.Windows.Forms.TextBox textSalutation;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textSchool;
		private System.Windows.Forms.RadioButton radioStudentN;
		private System.Windows.Forms.RadioButton radioStudentP;
		private System.Windows.Forms.RadioButton radioStudentF;
		private System.Windows.Forms.Label labelSchoolName;
		private System.Windows.Forms.Label labelChartNumber;
		private System.Windows.Forms.TextBox textChartNumber;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkAddressSame;
		private UI.ComboBoxOD comboZip;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.GroupBox groupNotes;
		private System.Windows.Forms.CheckBox checkNotesSame;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label32;
		private OpenDental.UI.Button butAuto;
		private System.Windows.Forms.Label labelMedicaidID;
		private System.Windows.Forms.TextBox textMedicaidID;
		private OpenDental.UI.ListBoxOD listStatus;
		private OpenDental.UI.ListBoxOD listGender;
		private OpenDental.UI.ListBoxOD listPosition;
		private System.Windows.Forms.TextBox textEmployer;
		private System.Windows.Forms.Label labelEmployer;
		private System.Windows.Forms.Label labelSSN;
		private System.Windows.Forms.Label labelZip;
		private System.Windows.Forms.Label labelST;
		private OpenDental.UI.Button butEditZip;
		private System.Windows.Forms.Label labelCity;
		private System.Windows.Forms.Label labelRace;
		private System.Windows.Forms.Label labelCounty;
		private System.Windows.Forms.Label labelSite;
		private System.Windows.Forms.Label labelGradeLevel;
		private System.Windows.Forms.TextBox textCounty;
		private System.Windows.Forms.Label labelUrgency;
		private System.Windows.Forms.TextBox textSite;
		private System.Windows.Forms.ComboBox comboGradeLevel;
		private System.Windows.Forms.ComboBox comboUrgency;
		private System.Windows.Forms.ListBox listSites;
		private System.Windows.Forms.ListBox listCounties;//displays dropdown for GradeSchools
		private System.Windows.Forms.Label labelDateFirstVisit;
		private OpenDental.ODtextBox textAddrNotes;
		private System.Windows.Forms.Label labelPutInInsPlan;
		private System.Windows.Forms.ListBox listEmps;//displayed from within code, not designer
		private UI.ComboBoxClinicPicker comboClinic;
		private TextBox textTrophyFolder;
		private Label labelTrophyFolder;
		private TextBox textWard;
		private Label labelWard;
		private Label labelLanguage;
		private ComboBox comboLanguage;
		private ComboBox comboContact;
		private Label labelContact;
		private ComboBox comboConfirm;
		private Label labelConfirm;
		private ComboBox comboRecall;
		private Label labelRecall;
		private Label labelAdmitDate;
		private TextBox textTitle;
		private Label labelTitle;
		private OpenDental.UI.Button butPickSite;
		private OpenDental.UI.Button butPickResponsParty;
		private TextBox textResponsParty;
		private Label labelResponsParty;
		private OpenDental.UI.Button butClearResponsParty;
		private Label labelCanadianEligibilityCode;
		private ComboBox comboCanadianEligibilityCode;
		private Label label41;
		private OpenDental.UI.ListBoxOD listRelationships;
		private OpenDental.UI.Button butAddGuardian;
		private OpenDental.UI.Button butGuardianDefaults;
		private TextBox textAskToArriveEarly;
		private Label labelAskToArriveEarly;
		private CheckBox checkArriveEarlySame;
		private Label label43;
		private Label labelTextOk;
		private ListBox listTextOk;
		private Label labelEthnicity;
		private TextBox textCountry;
		private TextBox textMotherMaidenFname;
		private Label labelMotherMaidenFname;
		private TextBox textMotherMaidenLname;
		private Label labelMotherMaidenLname;
		private Label labelDeceased;
		private GroupBox groupBox3;
		private CheckBox checkEmailPhoneSame;
		private UI.Button butShowMap;
		private GroupBox groupBillProv;
		private UI.Button butPickSecondary;
		private UI.ComboBoxOD comboBillType;
		private UI.Button butPickPrimary;
		private Label labelBillType;
		private Label labelFeeSched;
		private TextBox textCreditType;
		private Label labelSecProv;
		private Label labelCreditType;
		private Label labelPriProv;
		private UI.ComboBoxOD comboPriProv;
		private UI.ComboBoxOD comboFeeSched;
		private UI.ComboBoxOD comboSecProv;
		private CheckBox checkBillProvSame;
		private Label labelEmail;
		private TextBox textReferredFrom;
		private UI.Button butReferredFrom;
		private Label labelReferredFrom;
		private ToolTip _referredFromToolTip;
		private TextBox textMedicaidState;
		private Label labelRequiredField;
		private CheckBox checkSuperBilling;
		private CheckBox checkAddressSameForSuperFam;
		private CheckBox checkRestrictSched;
		private TabControl tabControlPatInfo;
		private TabPage tabPublicHealth;
		private TabPage tabHospitals;
		private TabPage tabOther;
		private TabPage tabICE;
		private TabPage tabEHR;
		private Label labelEmergencyPhone;
		private ValidPhone textIcePhone;
		private TextBox textIceName;
		private Label labelEmergencyName;
		private Label labelSexOrientation;
		private Label labelSpecifySexOrientation;
		private Label labelGenderIdentity;
		private Label labelSpecifyGender;
		private TextBox textSpecifyGender;
		private TextBox textSpecifySexOrientation;
		private ComboBox comboGenderIdentity;
		private ComboBox comboSexOrientation;
		private UI.Button butRaceEthnicity;
		private TextBox textEthnicity;
		private TextBox textRace;
		private ComboBox comboEthnicity;
		private UI.ComboBoxOD comboSpecialty;
		private Label labelSpecialty;
		private ToolTip _priProvEditToolTip=new ToolTip() { ShowAlways=true };
		private OpenDental.UI.ComboBoxOD comboExcludeECR;
		private Label labelExcludeECR;
		private CheckBox checkBoxSignedTil;
		private CheckBox checkDoseSpotConsent;
		private UI.Button butViewSSN;
		private UI.Button butViewBirthdate;
		private UI.Button butEmailEdit;
		private Label labelABC;
		private UI.Button butShortCodeOptIn;
		private Label labelApptTexts;
		private ListBox listBoxApptTexts;
        private UI.ODDatePicker odDatePickerBirthDate;
        private UI.ODDatePicker odDatePickerAdmitDate;
        private UI.ODDatePicker odDatePickerDateFirstVisit;
        private DateTimePicker dateTimePickerDateDeceased;
        private UI.Button butClearDateTimeDeceased;
    }
}