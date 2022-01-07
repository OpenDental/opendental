using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProvEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProvEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.gridProvIdent = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.comboSchoolClass = new System.Windows.Forms.ComboBox();
			this.labelSchoolClass = new System.Windows.Forms.Label();
			this.checkIsInstructor = new System.Windows.Forms.CheckBox();
			this.labelPassDescription = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.tabControlProvider = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.odColorPickerOutline = new OpenDental.UI.ODColorPicker();
			this.odColorPickerAppt = new OpenDental.UI.ODColorPicker();
			this.groupClinicOverrides = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.butClinicOverrides = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textStateLicense = new System.Windows.Forms.TextBox();
			this.textDEANum = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textStateRxID = new System.Windows.Forms.TextBox();
			this.textStateWhereLicensed = new System.Windows.Forms.TextBox();
			this.checkAllowLegacy = new System.Windows.Forms.CheckBox();
			this.labelTermDate = new System.Windows.Forms.Label();
			this.textProdGoalHr = new OpenDental.ValidDouble();
			this.labelProdGoalHr = new System.Windows.Forms.Label();
			this.textBirthdate = new OpenDental.ValidDate();
			this.label22 = new System.Windows.Forms.Label();
			this.textSchedRules = new System.Windows.Forms.TextBox();
			this.labelSchedRules = new System.Windows.Forms.Label();
			this.checkUseErx = new System.Windows.Forms.CheckBox();
			this.checkIsHiddenOnReports = new System.Windows.Forms.CheckBox();
			this.label21 = new System.Windows.Forms.Label();
			this.textCustomID = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.textProviderID = new System.Windows.Forms.TextBox();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.label19 = new System.Windows.Forms.Label();
			this.labelEhrMU = new System.Windows.Forms.Label();
			this.comboEhrMu = new System.Windows.Forms.ComboBox();
			this.checkIsNotPerson = new System.Windows.Forms.CheckBox();
			this.textEcwID = new System.Windows.Forms.TextBox();
			this.labelEcwID = new System.Windows.Forms.Label();
			this.checkIsCDAnet = new System.Windows.Forms.CheckBox();
			this.textTaxonomyOverride = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupAnesthProvType = new System.Windows.Forms.GroupBox();
			this.radAsstCirc = new System.Windows.Forms.RadioButton();
			this.radAnesthSurg = new System.Windows.Forms.RadioButton();
			this.radNone = new System.Windows.Forms.RadioButton();
			this.labelAnesthProvs = new System.Windows.Forms.Label();
			this.textCanadianOfficeNum = new System.Windows.Forms.TextBox();
			this.labelCanadianOfficeNum = new System.Windows.Forms.Label();
			this.textNationalProvID = new System.Windows.Forms.TextBox();
			this.labelNPI = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.textMedicaidID = new System.Windows.Forms.TextBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.textMI = new System.Windows.Forms.TextBox();
			this.textSuffix = new System.Windows.Forms.TextBox();
			this.textAbbr = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkSigOnFile = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioTIN = new System.Windows.Forms.RadioButton();
			this.radioSSN = new System.Windows.Forms.RadioButton();
			this.textSSN = new System.Windows.Forms.TextBox();
			this.listSpecialty = new OpenDental.UI.ListBoxOD();
			this.listFeeSched = new OpenDental.UI.ListBoxOD();
			this.checkIsSecondary = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.labelColor = new System.Windows.Forms.Label();
			this.dateTerm = new OpenDental.UI.ODDatePicker();
			this.tabSupplementalIDs = new System.Windows.Forms.TabPage();
			this.tabDentalSchools = new System.Windows.Forms.TabPage();
			this.tabWebSched = new System.Windows.Forms.TabPage();
			this.butPictureNone = new OpenDental.UI.Button();
			this.butPickPict = new OpenDental.UI.Button();
			this.label24 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.pictureWebSched = new System.Windows.Forms.PictureBox();
			this.textWebSchedDescript = new System.Windows.Forms.TextBox();
			this.tabClinics = new System.Windows.Forms.TabPage();
			this.labelClinicsDesc = new System.Windows.Forms.Label();
			this.checkAllClinics = new System.Windows.Forms.CheckBox();
			this.listBoxClinics = new OpenDental.UI.ListBoxOD();
			this.labelClinics = new System.Windows.Forms.Label();
			this.textPreferredName = new System.Windows.Forms.TextBox();
			this.label25 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.tabControlProvider.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.groupClinicOverrides.SuspendLayout();
			this.groupAnesthProvType.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabSupplementalIDs.SuspendLayout();
			this.tabDentalSchools.SuspendLayout();
			this.tabWebSched.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureWebSched)).BeginInit();
			this.tabClinics.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(726, 629);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 35;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(807, 629);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 36;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.gridProvIdent);
			this.groupBox2.Controls.Add(this.butAdd);
			this.groupBox2.Controls.Add(this.butDelete);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(19, 15);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(496, 157);
			this.groupBox2.TabIndex = 33;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Supplemental Provider Identifiers";
			// 
			// gridProvIdent
			// 
			this.gridProvIdent.Location = new System.Drawing.Point(11, 58);
			this.gridProvIdent.Name = "gridProvIdent";
			this.gridProvIdent.Size = new System.Drawing.Size(319, 88);
			this.gridProvIdent.TabIndex = 45;
			this.gridProvIdent.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProvIdent_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(360, 59);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(90, 24);
			this.butAdd.TabIndex = 0;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(360, 94);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(90, 24);
			this.butDelete.TabIndex = 1;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(481, 32);
			this.label2.TabIndex = 44;
			this.label2.Text = "This is where you store provider IDs assigned by individual insurance companies, " +
    "especially BC/BS.";
			// 
			// comboSchoolClass
			// 
			this.comboSchoolClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolClass.Location = new System.Drawing.Point(126, 19);
			this.comboSchoolClass.MaxDropDownItems = 30;
			this.comboSchoolClass.Name = "comboSchoolClass";
			this.comboSchoolClass.Size = new System.Drawing.Size(158, 21);
			this.comboSchoolClass.TabIndex = 0;
			this.comboSchoolClass.Visible = false;
			// 
			// labelSchoolClass
			// 
			this.labelSchoolClass.Location = new System.Drawing.Point(33, 20);
			this.labelSchoolClass.Name = "labelSchoolClass";
			this.labelSchoolClass.Size = new System.Drawing.Size(93, 16);
			this.labelSchoolClass.TabIndex = 89;
			this.labelSchoolClass.Text = "Class";
			this.labelSchoolClass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSchoolClass.Visible = false;
			// 
			// checkIsInstructor
			// 
			this.checkIsInstructor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsInstructor.Enabled = false;
			this.checkIsInstructor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsInstructor.Location = new System.Drawing.Point(32, 119);
			this.checkIsInstructor.Name = "checkIsInstructor";
			this.checkIsInstructor.Size = new System.Drawing.Size(107, 17);
			this.checkIsInstructor.TabIndex = 4;
			this.checkIsInstructor.Text = "Is Instructor";
			this.checkIsInstructor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPassDescription
			// 
			this.labelPassDescription.Location = new System.Drawing.Point(145, 120);
			this.labelPassDescription.Name = "labelPassDescription";
			this.labelPassDescription.Size = new System.Drawing.Size(138, 27);
			this.labelPassDescription.TabIndex = 248;
			this.labelPassDescription.Text = "To keep the old password, leave the box empty.";
			this.labelPassDescription.Visible = false;
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(126, 69);
			this.textUserName.MaxLength = 100;
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(157, 20);
			this.textUserName.TabIndex = 2;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(126, 93);
			this.textPassword.MaxLength = 100;
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(157, 20);
			this.textPassword.TabIndex = 3;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(126, 45);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.ReadOnly = true;
			this.textProvNum.Size = new System.Drawing.Size(157, 20);
			this.textProvNum.TabIndex = 1;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(30, 73);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(95, 14);
			this.label17.TabIndex = 116;
			this.label17.Text = "User Name";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(56, 48);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(69, 14);
			this.label16.TabIndex = 112;
			this.label16.Text = "ProvNum";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(30, 97);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(95, 14);
			this.label18.TabIndex = 115;
			this.label18.Text = "Password";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tabControlProvider
			// 
			this.tabControlProvider.Controls.Add(this.tabGeneral);
			this.tabControlProvider.Controls.Add(this.tabSupplementalIDs);
			this.tabControlProvider.Controls.Add(this.tabDentalSchools);
			this.tabControlProvider.Controls.Add(this.tabWebSched);
			this.tabControlProvider.Controls.Add(this.tabClinics);
			this.tabControlProvider.Location = new System.Drawing.Point(12, 12);
			this.tabControlProvider.Name = "tabControlProvider";
			this.tabControlProvider.SelectedIndex = 0;
			this.tabControlProvider.Size = new System.Drawing.Size(870, 611);
			this.tabControlProvider.TabIndex = 268;
			// 
			// tabGeneral
			// 
			this.tabGeneral.BackColor = System.Drawing.SystemColors.Control;
			this.tabGeneral.Controls.Add(this.textPreferredName);
			this.tabGeneral.Controls.Add(this.label25);
			this.tabGeneral.Controls.Add(this.odColorPickerOutline);
			this.tabGeneral.Controls.Add(this.odColorPickerAppt);
			this.tabGeneral.Controls.Add(this.groupClinicOverrides);
			this.tabGeneral.Controls.Add(this.checkAllowLegacy);
			this.tabGeneral.Controls.Add(this.labelTermDate);
			this.tabGeneral.Controls.Add(this.textProdGoalHr);
			this.tabGeneral.Controls.Add(this.labelProdGoalHr);
			this.tabGeneral.Controls.Add(this.textBirthdate);
			this.tabGeneral.Controls.Add(this.label22);
			this.tabGeneral.Controls.Add(this.textSchedRules);
			this.tabGeneral.Controls.Add(this.labelSchedRules);
			this.tabGeneral.Controls.Add(this.checkUseErx);
			this.tabGeneral.Controls.Add(this.checkIsHiddenOnReports);
			this.tabGeneral.Controls.Add(this.label21);
			this.tabGeneral.Controls.Add(this.textCustomID);
			this.tabGeneral.Controls.Add(this.label20);
			this.tabGeneral.Controls.Add(this.textProviderID);
			this.tabGeneral.Controls.Add(this.comboProv);
			this.tabGeneral.Controls.Add(this.label19);
			this.tabGeneral.Controls.Add(this.labelEhrMU);
			this.tabGeneral.Controls.Add(this.comboEhrMu);
			this.tabGeneral.Controls.Add(this.checkIsNotPerson);
			this.tabGeneral.Controls.Add(this.textEcwID);
			this.tabGeneral.Controls.Add(this.labelEcwID);
			this.tabGeneral.Controls.Add(this.checkIsCDAnet);
			this.tabGeneral.Controls.Add(this.textTaxonomyOverride);
			this.tabGeneral.Controls.Add(this.label4);
			this.tabGeneral.Controls.Add(this.groupAnesthProvType);
			this.tabGeneral.Controls.Add(this.textCanadianOfficeNum);
			this.tabGeneral.Controls.Add(this.labelCanadianOfficeNum);
			this.tabGeneral.Controls.Add(this.textNationalProvID);
			this.tabGeneral.Controls.Add(this.labelNPI);
			this.tabGeneral.Controls.Add(this.label14);
			this.tabGeneral.Controls.Add(this.textMedicaidID);
			this.tabGeneral.Controls.Add(this.textLName);
			this.tabGeneral.Controls.Add(this.textFName);
			this.tabGeneral.Controls.Add(this.textMI);
			this.tabGeneral.Controls.Add(this.textSuffix);
			this.tabGeneral.Controls.Add(this.textAbbr);
			this.tabGeneral.Controls.Add(this.label13);
			this.tabGeneral.Controls.Add(this.checkSigOnFile);
			this.tabGeneral.Controls.Add(this.groupBox1);
			this.tabGeneral.Controls.Add(this.listSpecialty);
			this.tabGeneral.Controls.Add(this.listFeeSched);
			this.tabGeneral.Controls.Add(this.checkIsSecondary);
			this.tabGeneral.Controls.Add(this.label10);
			this.tabGeneral.Controls.Add(this.label9);
			this.tabGeneral.Controls.Add(this.label8);
			this.tabGeneral.Controls.Add(this.label7);
			this.tabGeneral.Controls.Add(this.label6);
			this.tabGeneral.Controls.Add(this.label5);
			this.tabGeneral.Controls.Add(this.label1);
			this.tabGeneral.Controls.Add(this.checkIsHidden);
			this.tabGeneral.Controls.Add(this.labelColor);
			this.tabGeneral.Controls.Add(this.dateTerm);
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabGeneral.Size = new System.Drawing.Size(862, 585);
			this.tabGeneral.TabIndex = 0;
			this.tabGeneral.Text = "General";
			// 
			// odColorPickerOutline
			// 
			this.odColorPickerOutline.BackgroundColor = System.Drawing.Color.Empty;
			this.odColorPickerOutline.Location = new System.Drawing.Point(201, 496);
			this.odColorPickerOutline.Name = "odColorPickerOutline";
			this.odColorPickerOutline.Size = new System.Drawing.Size(74, 21);
			this.odColorPickerOutline.TabIndex = 343;
			// 
			// odColorPickerAppt
			// 
			this.odColorPickerAppt.BackgroundColor = System.Drawing.Color.Empty;
			this.odColorPickerAppt.Location = new System.Drawing.Point(201, 475);
			this.odColorPickerAppt.Name = "odColorPickerAppt";
			this.odColorPickerAppt.Size = new System.Drawing.Size(74, 21);
			this.odColorPickerAppt.TabIndex = 342;
			// 
			// groupClinicOverrides
			// 
			this.groupClinicOverrides.Controls.Add(this.label15);
			this.groupClinicOverrides.Controls.Add(this.butClinicOverrides);
			this.groupClinicOverrides.Controls.Add(this.label3);
			this.groupClinicOverrides.Controls.Add(this.label11);
			this.groupClinicOverrides.Controls.Add(this.textStateLicense);
			this.groupClinicOverrides.Controls.Add(this.textDEANum);
			this.groupClinicOverrides.Controls.Add(this.label12);
			this.groupClinicOverrides.Controls.Add(this.textStateRxID);
			this.groupClinicOverrides.Controls.Add(this.textStateWhereLicensed);
			this.groupClinicOverrides.Location = new System.Drawing.Point(37, 290);
			this.groupClinicOverrides.Name = "groupClinicOverrides";
			this.groupClinicOverrides.Size = new System.Drawing.Size(340, 98);
			this.groupClinicOverrides.TabIndex = 341;
			this.groupClinicOverrides.TabStop = false;
			this.groupClinicOverrides.Text = "Clinic Overrides";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(24, 36);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(139, 14);
			this.label15.TabIndex = 319;
			this.label15.Text = "State Where Licensed";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butClinicOverrides
			// 
			this.butClinicOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClinicOverrides.Location = new System.Drawing.Point(287, 72);
			this.butClinicOverrides.Name = "butClinicOverrides";
			this.butClinicOverrides.Size = new System.Drawing.Size(47, 21);
			this.butClinicOverrides.TabIndex = 329;
			this.butClinicOverrides.Text = "Edit";
			this.butClinicOverrides.UseVisualStyleBackColor = true;
			this.butClinicOverrides.Click += new System.EventHandler(this.butClinicOverrides_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(22, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(142, 14);
			this.label3.TabIndex = 285;
			this.label3.Text = "State License Number";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(26, 57);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(137, 14);
			this.label11.TabIndex = 299;
			this.label11.Text = "DEA Number";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textStateLicense
			// 
			this.textStateLicense.Location = new System.Drawing.Point(165, 11);
			this.textStateLicense.MaxLength = 15;
			this.textStateLicense.Name = "textStateLicense";
			this.textStateLicense.Size = new System.Drawing.Size(102, 20);
			this.textStateLicense.TabIndex = 275;
			// 
			// textDEANum
			// 
			this.textDEANum.Location = new System.Drawing.Point(165, 53);
			this.textDEANum.MaxLength = 15;
			this.textDEANum.Name = "textDEANum";
			this.textDEANum.Size = new System.Drawing.Size(102, 20);
			this.textDEANum.TabIndex = 277;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(27, 78);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(137, 14);
			this.label12.TabIndex = 318;
			this.label12.Text = "State Rx ID";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textStateRxID
			// 
			this.textStateRxID.Location = new System.Drawing.Point(165, 74);
			this.textStateRxID.MaxLength = 15;
			this.textStateRxID.Name = "textStateRxID";
			this.textStateRxID.Size = new System.Drawing.Size(102, 20);
			this.textStateRxID.TabIndex = 279;
			// 
			// textStateWhereLicensed
			// 
			this.textStateWhereLicensed.Location = new System.Drawing.Point(165, 32);
			this.textStateWhereLicensed.MaxLength = 15;
			this.textStateWhereLicensed.Name = "textStateWhereLicensed";
			this.textStateWhereLicensed.Size = new System.Drawing.Size(34, 20);
			this.textStateWhereLicensed.TabIndex = 276;
			// 
			// checkAllowLegacy
			// 
			this.checkAllowLegacy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowLegacy.Location = new System.Drawing.Point(640, 526);
			this.checkAllowLegacy.Name = "checkAllowLegacy";
			this.checkAllowLegacy.Size = new System.Drawing.Size(189, 17);
			this.checkAllowLegacy.TabIndex = 340;
			this.checkAllowLegacy.Text = "Allow Legacy eRx Option";
			this.checkAllowLegacy.Visible = false;
			// 
			// labelTermDate
			// 
			this.labelTermDate.Location = new System.Drawing.Point(106, 457);
			this.labelTermDate.Name = "labelTermDate";
			this.labelTermDate.Size = new System.Drawing.Size(95, 16);
			this.labelTermDate.TabIndex = 330;
			this.labelTermDate.Text = "Term Date";
			this.labelTermDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProdGoalHr
			// 
			this.textProdGoalHr.Location = new System.Drawing.Point(202, 561);
			this.textProdGoalHr.MaxVal = 100000000D;
			this.textProdGoalHr.MinVal = -100000000D;
			this.textProdGoalHr.Name = "textProdGoalHr";
			this.textProdGoalHr.Size = new System.Drawing.Size(102, 20);
			this.textProdGoalHr.TabIndex = 291;
			// 
			// labelProdGoalHr
			// 
			this.labelProdGoalHr.Location = new System.Drawing.Point(33, 561);
			this.labelProdGoalHr.Name = "labelProdGoalHr";
			this.labelProdGoalHr.Size = new System.Drawing.Size(168, 21);
			this.labelProdGoalHr.TabIndex = 328;
			this.labelProdGoalHr.Text = "Hourly Production Goal";
			this.labelProdGoalHr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(202, 185);
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.Size = new System.Drawing.Size(102, 20);
			this.textBirthdate.TabIndex = 273;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(54, 189);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(147, 14);
			this.label22.TabIndex = 326;
			this.label22.Text = "Birthdate";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textSchedRules
			// 
			this.textSchedRules.AcceptsReturn = true;
			this.textSchedRules.Location = new System.Drawing.Point(419, 240);
			this.textSchedRules.Multiline = true;
			this.textSchedRules.Name = "textSchedRules";
			this.textSchedRules.Size = new System.Drawing.Size(330, 60);
			this.textSchedRules.TabIndex = 300;
			// 
			// labelSchedRules
			// 
			this.labelSchedRules.Location = new System.Drawing.Point(417, 223);
			this.labelSchedRules.Name = "labelSchedRules";
			this.labelSchedRules.Size = new System.Drawing.Size(170, 14);
			this.labelSchedRules.TabIndex = 325;
			this.labelSchedRules.Text = "Scheduling Note";
			this.labelSchedRules.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkUseErx
			// 
			this.checkUseErx.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseErx.Location = new System.Drawing.Point(419, 526);
			this.checkUseErx.Name = "checkUseErx";
			this.checkUseErx.Size = new System.Drawing.Size(212, 17);
			this.checkUseErx.TabIndex = 310;
			this.checkUseErx.Text = "Use Electronic Prescriptions (eRx)";
			// 
			// checkIsHiddenOnReports
			// 
			this.checkIsHiddenOnReports.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHiddenOnReports.Location = new System.Drawing.Point(419, 510);
			this.checkIsHiddenOnReports.Name = "checkIsHiddenOnReports";
			this.checkIsHiddenOnReports.Size = new System.Drawing.Size(158, 17);
			this.checkIsHiddenOnReports.TabIndex = 309;
			this.checkIsHiddenOnReports.Text = "Hidden On Reports";
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(417, 301);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(107, 14);
			this.label21.TabIndex = 324;
			this.label21.Text = "Custom ID";
			this.label21.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCustomID
			// 
			this.textCustomID.Location = new System.Drawing.Point(419, 318);
			this.textCustomID.MaxLength = 255;
			this.textCustomID.Name = "textCustomID";
			this.textCustomID.Size = new System.Drawing.Size(108, 20);
			this.textCustomID.TabIndex = 301;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(64, 41);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(136, 14);
			this.label20.TabIndex = 323;
			this.label20.Text = "Provider ID";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProviderID
			// 
			this.textProviderID.Location = new System.Drawing.Point(202, 38);
			this.textProviderID.MaxLength = 255;
			this.textProviderID.Name = "textProviderID";
			this.textProviderID.ReadOnly = true;
			this.textProviderID.Size = new System.Drawing.Size(121, 20);
			this.textProviderID.TabIndex = 322;
			this.textProviderID.TabStop = false;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(202, 539);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(161, 21);
			this.comboProv.TabIndex = 290;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(34, 539);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(168, 21);
			this.label19.TabIndex = 321;
			this.label19.Text = "Claim Billing Prov Override";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEhrMU
			// 
			this.labelEhrMU.Location = new System.Drawing.Point(57, 517);
			this.labelEhrMU.Name = "labelEhrMU";
			this.labelEhrMU.Size = new System.Drawing.Size(144, 21);
			this.labelEhrMU.TabIndex = 320;
			this.labelEhrMU.Text = "EHR Meaningful Use";
			this.labelEhrMU.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEhrMu
			// 
			this.comboEhrMu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEhrMu.Location = new System.Drawing.Point(202, 517);
			this.comboEhrMu.MaxDropDownItems = 30;
			this.comboEhrMu.Name = "comboEhrMu";
			this.comboEhrMu.Size = new System.Drawing.Size(121, 21);
			this.comboEhrMu.TabIndex = 288;
			// 
			// checkIsNotPerson
			// 
			this.checkIsNotPerson.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsNotPerson.Location = new System.Drawing.Point(419, 478);
			this.checkIsNotPerson.Name = "checkIsNotPerson";
			this.checkIsNotPerson.Size = new System.Drawing.Size(410, 17);
			this.checkIsNotPerson.TabIndex = 307;
			this.checkIsNotPerson.Text = "Not a Person (for example, a dummy provider representing the organization)";
			// 
			// textEcwID
			// 
			this.textEcwID.Location = new System.Drawing.Point(202, 17);
			this.textEcwID.MaxLength = 255;
			this.textEcwID.Name = "textEcwID";
			this.textEcwID.ReadOnly = true;
			this.textEcwID.Size = new System.Drawing.Size(121, 20);
			this.textEcwID.TabIndex = 316;
			// 
			// labelEcwID
			// 
			this.labelEcwID.Location = new System.Drawing.Point(65, 21);
			this.labelEcwID.Name = "labelEcwID";
			this.labelEcwID.Size = new System.Drawing.Size(136, 14);
			this.labelEcwID.TabIndex = 317;
			this.labelEcwID.Text = "eCW ID";
			this.labelEcwID.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsCDAnet
			// 
			this.checkIsCDAnet.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsCDAnet.Location = new System.Drawing.Point(419, 430);
			this.checkIsCDAnet.Name = "checkIsCDAnet";
			this.checkIsCDAnet.Size = new System.Drawing.Size(168, 17);
			this.checkIsCDAnet.TabIndex = 304;
			this.checkIsCDAnet.Text = "Is CDAnet Member";
			this.checkIsCDAnet.Visible = false;
			// 
			// textTaxonomyOverride
			// 
			this.textTaxonomyOverride.Location = new System.Drawing.Point(595, 318);
			this.textTaxonomyOverride.MaxLength = 255;
			this.textTaxonomyOverride.Name = "textTaxonomyOverride";
			this.textTaxonomyOverride.Size = new System.Drawing.Size(154, 20);
			this.textTaxonomyOverride.TabIndex = 302;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(592, 301);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(154, 14);
			this.label4.TabIndex = 315;
			this.label4.Text = "Taxonomy Code Override";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupAnesthProvType
			// 
			this.groupAnesthProvType.Controls.Add(this.radAsstCirc);
			this.groupAnesthProvType.Controls.Add(this.radAnesthSurg);
			this.groupAnesthProvType.Controls.Add(this.radNone);
			this.groupAnesthProvType.Controls.Add(this.labelAnesthProvs);
			this.groupAnesthProvType.Location = new System.Drawing.Point(407, 346);
			this.groupAnesthProvType.Name = "groupAnesthProvType";
			this.groupAnesthProvType.Size = new System.Drawing.Size(347, 83);
			this.groupAnesthProvType.TabIndex = 303;
			this.groupAnesthProvType.TabStop = false;
			this.groupAnesthProvType.Text = "Anesthesia Provider Groups (optional)";
			// 
			// radAsstCirc
			// 
			this.radAsstCirc.AutoSize = true;
			this.radAsstCirc.Location = new System.Drawing.Point(16, 56);
			this.radAsstCirc.Name = "radAsstCirc";
			this.radAsstCirc.Size = new System.Drawing.Size(116, 17);
			this.radAsstCirc.TabIndex = 9;
			this.radAsstCirc.Text = "Assistant/Circulator";
			this.radAsstCirc.UseVisualStyleBackColor = true;
			// 
			// radAnesthSurg
			// 
			this.radAnesthSurg.AutoSize = true;
			this.radAnesthSurg.Location = new System.Drawing.Point(16, 37);
			this.radAnesthSurg.Name = "radAnesthSurg";
			this.radAnesthSurg.Size = new System.Drawing.Size(122, 17);
			this.radAnesthSurg.TabIndex = 8;
			this.radAnesthSurg.Text = "Anesthetist/Surgeon";
			this.radAnesthSurg.UseVisualStyleBackColor = true;
			// 
			// radNone
			// 
			this.radNone.AutoSize = true;
			this.radNone.Checked = true;
			this.radNone.Location = new System.Drawing.Point(16, 18);
			this.radNone.Name = "radNone";
			this.radNone.Size = new System.Drawing.Size(51, 17);
			this.radNone.TabIndex = 7;
			this.radNone.TabStop = true;
			this.radNone.Text = "None";
			this.radNone.UseVisualStyleBackColor = true;
			// 
			// labelAnesthProvs
			// 
			this.labelAnesthProvs.Location = new System.Drawing.Point(157, 22);
			this.labelAnesthProvs.Name = "labelAnesthProvs";
			this.labelAnesthProvs.Size = new System.Drawing.Size(188, 52);
			this.labelAnesthProvs.TabIndex = 4;
			this.labelAnesthProvs.Text = "Assign this user to a group. This will populate the corresponding dropdowns on th" +
    "e Anesthetic Record.";
			// 
			// textCanadianOfficeNum
			// 
			this.textCanadianOfficeNum.Location = new System.Drawing.Point(202, 433);
			this.textCanadianOfficeNum.MaxLength = 20;
			this.textCanadianOfficeNum.Name = "textCanadianOfficeNum";
			this.textCanadianOfficeNum.Size = new System.Drawing.Size(102, 20);
			this.textCanadianOfficeNum.TabIndex = 283;
			// 
			// labelCanadianOfficeNum
			// 
			this.labelCanadianOfficeNum.Location = new System.Drawing.Point(60, 437);
			this.labelCanadianOfficeNum.Name = "labelCanadianOfficeNum";
			this.labelCanadianOfficeNum.Size = new System.Drawing.Size(141, 14);
			this.labelCanadianOfficeNum.TabIndex = 314;
			this.labelCanadianOfficeNum.Text = "Office Number";
			this.labelCanadianOfficeNum.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNationalProvID
			// 
			this.textNationalProvID.Location = new System.Drawing.Point(202, 412);
			this.textNationalProvID.MaxLength = 20;
			this.textNationalProvID.Name = "textNationalProvID";
			this.textNationalProvID.Size = new System.Drawing.Size(102, 20);
			this.textNationalProvID.TabIndex = 282;
			// 
			// labelNPI
			// 
			this.labelNPI.Location = new System.Drawing.Point(60, 416);
			this.labelNPI.Name = "labelNPI";
			this.labelNPI.Size = new System.Drawing.Size(141, 14);
			this.labelNPI.TabIndex = 313;
			this.labelNPI.Text = "National Provider ID";
			this.labelNPI.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(62, 499);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(140, 16);
			this.label14.TabIndex = 312;
			this.label14.Text = "Highlight Outline Color";
			this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textMedicaidID
			// 
			this.textMedicaidID.Location = new System.Drawing.Point(202, 391);
			this.textMedicaidID.MaxLength = 20;
			this.textMedicaidID.Name = "textMedicaidID";
			this.textMedicaidID.Size = new System.Drawing.Size(102, 20);
			this.textMedicaidID.TabIndex = 280;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(202, 80);
			this.textLName.MaxLength = 100;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(161, 20);
			this.textLName.TabIndex = 269;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(202, 101);
			this.textFName.MaxLength = 100;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(161, 20);
			this.textFName.TabIndex = 270;
			// 
			// textMI
			// 
			this.textMI.Location = new System.Drawing.Point(202, 122);
			this.textMI.MaxLength = 100;
			this.textMI.Name = "textMI";
			this.textMI.Size = new System.Drawing.Size(63, 20);
			this.textMI.TabIndex = 271;
			// 
			// textSuffix
			// 
			this.textSuffix.Location = new System.Drawing.Point(202, 143);
			this.textSuffix.MaxLength = 100;
			this.textSuffix.Name = "textSuffix";
			this.textSuffix.Size = new System.Drawing.Size(102, 20);
			this.textSuffix.TabIndex = 272;
			// 
			// textAbbr
			// 
			this.textAbbr.Location = new System.Drawing.Point(202, 59);
			this.textAbbr.MaxLength = 255;
			this.textAbbr.Name = "textAbbr";
			this.textAbbr.Size = new System.Drawing.Size(121, 20);
			this.textAbbr.TabIndex = 268;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(60, 395);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(141, 14);
			this.label13.TabIndex = 311;
			this.label13.Text = "Medicaid ID";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkSigOnFile
			// 
			this.checkSigOnFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSigOnFile.Location = new System.Drawing.Point(419, 462);
			this.checkSigOnFile.Name = "checkSigOnFile";
			this.checkSigOnFile.Size = new System.Drawing.Size(121, 17);
			this.checkSigOnFile.TabIndex = 306;
			this.checkSigOnFile.Text = "Signature on File";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioTIN);
			this.groupBox1.Controls.Add(this.radioSSN);
			this.groupBox1.Controls.Add(this.textSSN);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(194, 211);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(156, 80);
			this.groupBox1.TabIndex = 274;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "SSN or TIN (no dashes)";
			// 
			// radioTIN
			// 
			this.radioTIN.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioTIN.Location = new System.Drawing.Point(13, 34);
			this.radioTIN.Name = "radioTIN";
			this.radioTIN.Size = new System.Drawing.Size(135, 15);
			this.radioTIN.TabIndex = 1;
			this.radioTIN.Text = "TIN";
			this.radioTIN.Click += new System.EventHandler(this.radioTIN_Click);
			// 
			// radioSSN
			// 
			this.radioSSN.Checked = true;
			this.radioSSN.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioSSN.Location = new System.Drawing.Point(13, 17);
			this.radioSSN.Name = "radioSSN";
			this.radioSSN.Size = new System.Drawing.Size(104, 14);
			this.radioSSN.TabIndex = 0;
			this.radioSSN.TabStop = true;
			this.radioSSN.Text = "SSN";
			this.radioSSN.Click += new System.EventHandler(this.radioSSN_Click);
			// 
			// textSSN
			// 
			this.textSSN.Location = new System.Drawing.Point(8, 54);
			this.textSSN.Name = "textSSN";
			this.textSSN.Size = new System.Drawing.Size(102, 20);
			this.textSSN.TabIndex = 2;
			// 
			// listSpecialty
			// 
			this.listSpecialty.Items.AddRange(new object[] {
            "Dental General Practice",
            "Dental Hygienist",
            "Endodontics",
            "Pediatric Dentistry",
            "Periodontics",
            "Prosthodontics",
            "Orthodontics",
            "Denturist",
            "Surgery, Oral & Maxillofacial",
            "Dental Assistant",
            "Dental Laboratory Technician",
            "Pathology, Oral & MaxFac",
            "Public Health",
            "Radiology"});
			this.listSpecialty.Location = new System.Drawing.Point(598, 34);
			this.listSpecialty.Name = "listSpecialty";
			this.listSpecialty.Size = new System.Drawing.Size(154, 186);
			this.listSpecialty.TabIndex = 297;
			// 
			// listFeeSched
			// 
			this.listFeeSched.Location = new System.Drawing.Point(419, 34);
			this.listFeeSched.Name = "listFeeSched";
			this.listFeeSched.Size = new System.Drawing.Size(168, 186);
			this.listFeeSched.TabIndex = 296;
			// 
			// checkIsSecondary
			// 
			this.checkIsSecondary.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsSecondary.Location = new System.Drawing.Point(419, 446);
			this.checkIsSecondary.Name = "checkIsSecondary";
			this.checkIsSecondary.Size = new System.Drawing.Size(155, 17);
			this.checkIsSecondary.TabIndex = 305;
			this.checkIsSecondary.Text = "Secondary Provider (Hyg)";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(57, 84);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(143, 14);
			this.label10.TabIndex = 298;
			this.label10.Text = "Last Name";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(54, 147);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(147, 14);
			this.label9.TabIndex = 295;
			this.label9.Text = "Suffix (MD,DMD,DDS,etc)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(63, 105);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(138, 14);
			this.label8.TabIndex = 293;
			this.label8.Text = "First Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(94, 126);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(102, 14);
			this.label7.TabIndex = 338;
			this.label7.Text = "MI";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(417, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 14);
			this.label6.TabIndex = 289;
			this.label6.Text = "Fee Schedule";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(598, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(116, 14);
			this.label5.TabIndex = 287;
			this.label5.Text = "Specialty";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(65, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 14);
			this.label1.TabIndex = 281;
			this.label1.Text = "Abbreviation";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHidden.Location = new System.Drawing.Point(419, 494);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(158, 17);
			this.checkIsHidden.TabIndex = 308;
			this.checkIsHidden.Text = "Hidden";
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(62, 478);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(140, 16);
			this.labelColor.TabIndex = 278;
			this.labelColor.Text = "Appointment Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// dateTerm
			// 
			this.dateTerm.BackColor = System.Drawing.SystemColors.Control;
			this.dateTerm.CalendarLocation = OpenDental.UI.ODDatePicker.CalendarLocationOptions.ToTheRight;
			this.dateTerm.Location = new System.Drawing.Point(139, 453);
			this.dateTerm.Name = "dateTerm";
			this.dateTerm.Size = new System.Drawing.Size(228, 21);
			this.dateTerm.TabIndex = 339;
			// 
			// tabSupplementalIDs
			// 
			this.tabSupplementalIDs.BackColor = System.Drawing.SystemColors.Control;
			this.tabSupplementalIDs.Controls.Add(this.groupBox2);
			this.tabSupplementalIDs.Location = new System.Drawing.Point(4, 22);
			this.tabSupplementalIDs.Name = "tabSupplementalIDs";
			this.tabSupplementalIDs.Padding = new System.Windows.Forms.Padding(3);
			this.tabSupplementalIDs.Size = new System.Drawing.Size(862, 599);
			this.tabSupplementalIDs.TabIndex = 1;
			this.tabSupplementalIDs.Text = "Supplemental IDs";
			// 
			// tabDentalSchools
			// 
			this.tabDentalSchools.BackColor = System.Drawing.SystemColors.Control;
			this.tabDentalSchools.Controls.Add(this.labelPassDescription);
			this.tabDentalSchools.Controls.Add(this.textUserName);
			this.tabDentalSchools.Controls.Add(this.comboSchoolClass);
			this.tabDentalSchools.Controls.Add(this.textPassword);
			this.tabDentalSchools.Controls.Add(this.checkIsInstructor);
			this.tabDentalSchools.Controls.Add(this.textProvNum);
			this.tabDentalSchools.Controls.Add(this.labelSchoolClass);
			this.tabDentalSchools.Controls.Add(this.label17);
			this.tabDentalSchools.Controls.Add(this.label18);
			this.tabDentalSchools.Controls.Add(this.label16);
			this.tabDentalSchools.Location = new System.Drawing.Point(4, 22);
			this.tabDentalSchools.Name = "tabDentalSchools";
			this.tabDentalSchools.Padding = new System.Windows.Forms.Padding(3);
			this.tabDentalSchools.Size = new System.Drawing.Size(862, 599);
			this.tabDentalSchools.TabIndex = 2;
			this.tabDentalSchools.Text = "Dental Schools";
			// 
			// tabWebSched
			// 
			this.tabWebSched.BackColor = System.Drawing.SystemColors.Control;
			this.tabWebSched.Controls.Add(this.butPictureNone);
			this.tabWebSched.Controls.Add(this.butPickPict);
			this.tabWebSched.Controls.Add(this.label24);
			this.tabWebSched.Controls.Add(this.label23);
			this.tabWebSched.Controls.Add(this.pictureWebSched);
			this.tabWebSched.Controls.Add(this.textWebSchedDescript);
			this.tabWebSched.Location = new System.Drawing.Point(4, 22);
			this.tabWebSched.Name = "tabWebSched";
			this.tabWebSched.Padding = new System.Windows.Forms.Padding(3);
			this.tabWebSched.Size = new System.Drawing.Size(862, 599);
			this.tabWebSched.TabIndex = 3;
			this.tabWebSched.Text = "Web Sched";
			// 
			// butPictureNone
			// 
			this.butPictureNone.Location = new System.Drawing.Point(320, 141);
			this.butPictureNone.Name = "butPictureNone";
			this.butPictureNone.Size = new System.Drawing.Size(57, 23);
			this.butPictureNone.TabIndex = 117;
			this.butPictureNone.TabStop = false;
			this.butPictureNone.Text = "None";
			this.butPictureNone.Click += new System.EventHandler(this.butPictureNone_Click);
			// 
			// butPickPict
			// 
			this.butPickPict.Location = new System.Drawing.Point(286, 141);
			this.butPickPict.Name = "butPickPict";
			this.butPickPict.Size = new System.Drawing.Size(27, 23);
			this.butPickPict.TabIndex = 116;
			this.butPickPict.Text = "...";
			this.butPickPict.Click += new System.EventHandler(this.butPickPict_Click);
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(54, 141);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(93, 16);
			this.label24.TabIndex = 115;
			this.label24.Text = "Picture";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(54, 37);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(93, 16);
			this.label23.TabIndex = 114;
			this.label23.Text = "Description";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pictureWebSched
			// 
			this.pictureWebSched.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureWebSched.Location = new System.Drawing.Point(153, 141);
			this.pictureWebSched.Name = "pictureWebSched";
			this.pictureWebSched.Size = new System.Drawing.Size(128, 128);
			this.pictureWebSched.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureWebSched.TabIndex = 113;
			this.pictureWebSched.TabStop = false;
			// 
			// textWebSchedDescript
			// 
			this.textWebSchedDescript.AcceptsReturn = true;
			this.textWebSchedDescript.AcceptsTab = true;
			this.textWebSchedDescript.BackColor = System.Drawing.SystemColors.Window;
			this.textWebSchedDescript.Location = new System.Drawing.Point(153, 36);
			this.textWebSchedDescript.MaxLength = 65000;
			this.textWebSchedDescript.Multiline = true;
			this.textWebSchedDescript.Name = "textWebSchedDescript";
			this.textWebSchedDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textWebSchedDescript.Size = new System.Drawing.Size(366, 96);
			this.textWebSchedDescript.TabIndex = 1;
			// 
			// tabClinics
			// 
			this.tabClinics.BackColor = System.Drawing.SystemColors.Control;
			this.tabClinics.Controls.Add(this.labelClinicsDesc);
			this.tabClinics.Controls.Add(this.checkAllClinics);
			this.tabClinics.Controls.Add(this.listBoxClinics);
			this.tabClinics.Controls.Add(this.labelClinics);
			this.tabClinics.Location = new System.Drawing.Point(4, 22);
			this.tabClinics.Name = "tabClinics";
			this.tabClinics.Padding = new System.Windows.Forms.Padding(3);
			this.tabClinics.Size = new System.Drawing.Size(862, 599);
			this.tabClinics.TabIndex = 4;
			this.tabClinics.Text = "Clinics";
			// 
			// labelClinicsDesc
			// 
			this.labelClinicsDesc.Location = new System.Drawing.Point(12, 9);
			this.labelClinicsDesc.Name = "labelClinicsDesc";
			this.labelClinicsDesc.Size = new System.Drawing.Size(504, 45);
			this.labelClinicsDesc.TabIndex = 48;
			this.labelClinicsDesc.Text = resources.GetString("labelClinicsDesc.Text");
			this.labelClinicsDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAllClinics
			// 
			this.checkAllClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClinics.Location = new System.Drawing.Point(12, 72);
			this.checkAllClinics.Name = "checkAllClinics";
			this.checkAllClinics.Size = new System.Drawing.Size(154, 16);
			this.checkAllClinics.TabIndex = 45;
			this.checkAllClinics.Text = "All";
			this.checkAllClinics.CheckedChanged += new System.EventHandler(this.checkAllClinics_CheckedChanged);
			// 
			// listBoxClinics
			// 
			this.listBoxClinics.Location = new System.Drawing.Point(12, 91);
			this.listBoxClinics.Name = "listBoxClinics";
			this.listBoxClinics.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxClinics.Size = new System.Drawing.Size(154, 186);
			this.listBoxClinics.TabIndex = 46;
			this.listBoxClinics.SelectedIndexChanged += new System.EventHandler(this.listBoxClinics_SelectedIndexChanged);
			// 
			// labelClinics
			// 
			this.labelClinics.Location = new System.Drawing.Point(12, 54);
			this.labelClinics.Name = "labelClinics";
			this.labelClinics.Size = new System.Drawing.Size(154, 16);
			this.labelClinics.TabIndex = 47;
			this.labelClinics.Text = "Clinics";
			this.labelClinics.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPreferredName
			// 
			this.textPreferredName.Location = new System.Drawing.Point(202, 164);
			this.textPreferredName.MaxLength = 100;
			this.textPreferredName.Name = "textPreferredName";
			this.textPreferredName.Size = new System.Drawing.Size(161, 20);
			this.textPreferredName.TabIndex = 344;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(63, 168);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(138, 14);
			this.label25.TabIndex = 345;
			this.label25.Text = "Preferred Name";
			this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormProvEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(895, 665);
			this.Controls.Add(this.tabControlProvider);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProvEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Provider";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormProvEdit_Closing);
			this.Load += new System.EventHandler(this.FormProvEdit_Load);
			this.groupBox2.ResumeLayout(false);
			this.tabControlProvider.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabGeneral.PerformLayout();
			this.groupClinicOverrides.ResumeLayout(false);
			this.groupClinicOverrides.PerformLayout();
			this.groupAnesthProvType.ResumeLayout(false);
			this.groupAnesthProvType.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabSupplementalIDs.ResumeLayout(false);
			this.tabDentalSchools.ResumeLayout(false);
			this.tabDentalSchools.PerformLayout();
			this.tabWebSched.ResumeLayout(false);
			this.tabWebSched.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureWebSched)).EndInit();
			this.tabClinics.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region UI Elements
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.ComboBox comboSchoolClass;
		private System.Windows.Forms.Label labelSchoolClass;
		private CheckBox checkIsInstructor;
		private TextBox textUserName;
		private TextBox textPassword;
		private TextBox textProvNum;
		private Label label17;
		private Label label16;
		private Label label18;
		private Label labelPassDescription;
		private TabControl tabControlProvider;
		private TabPage tabGeneral;
		private ValidDate textBirthdate;
		private Label label22;
		private TextBox textSchedRules;
		private Label labelSchedRules;
		private CheckBox checkUseErx;
		private CheckBox checkIsHiddenOnReports;
		private Label label21;
		private TextBox textCustomID;
		private Label label20;
		private TextBox textProviderID;
		private UI.ComboBoxOD comboProv;
		private Label label19;
		private Label labelEhrMU;
		private ComboBox comboEhrMu;
		private TextBox textStateWhereLicensed;
		private Label label15;
		private CheckBox checkIsNotPerson;
		private TextBox textStateRxID;
		private Label label12;
		private TextBox textEcwID;
		private Label labelEcwID;
		private CheckBox checkIsCDAnet;
		private TextBox textTaxonomyOverride;
		private Label label4;
		private GroupBox groupAnesthProvType;
		private RadioButton radAsstCirc;
		private RadioButton radAnesthSurg;
		private RadioButton radNone;
		private Label labelAnesthProvs;
		private TextBox textCanadianOfficeNum;
		private Label labelCanadianOfficeNum;
		private TextBox textNationalProvID;
		private Label labelNPI;
		private Label label14;
		private TextBox textMedicaidID;
		private TextBox textDEANum;
		private TextBox textLName;
		private TextBox textFName;
		private TextBox textMI;
		private TextBox textSuffix;
		private TextBox textStateLicense;
		private TextBox textAbbr;
		private Label label13;
		private CheckBox checkSigOnFile;
		private GroupBox groupBox1;
		private RadioButton radioTIN;
		private RadioButton radioSSN;
		private TextBox textSSN;
		private UI.ListBoxOD listSpecialty;
		private UI.ListBoxOD listFeeSched;
		private CheckBox checkIsSecondary;
		private Label label11;
		private Label label10;
		private Label label9;
		private Label label8;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label3;
		private Label label1;
		private CheckBox checkIsHidden;
		private Label labelColor;
		private TabPage tabSupplementalIDs;
		private TabPage tabDentalSchools;
		private TabPage tabWebSched;
		private TextBox textWebSchedDescript;
		private Label label24;
		private Label label23;
		private PictureBox pictureWebSched;
		private UI.Button butPickPict;
		private UI.Button butPictureNone;
		private TabPage tabClinics;
		private CheckBox checkAllClinics;
		private UI.ListBoxOD listBoxClinics;
		private Label labelClinics;
		private ValidDouble textProdGoalHr;
		private Label labelProdGoalHr;
		private Label labelTermDate;
		private UI.ODDatePicker dateTerm;
		private CheckBox checkAllowLegacy;
		private UI.Button butClinicOverrides;
		private GroupBox groupClinicOverrides;
		private Label labelClinicsDesc;
		private UI.ODColorPicker odColorPickerOutline;
		private UI.ODColorPicker odColorPickerAppt;
		private TextBox textPreferredName;
		private Label label25;
		private UI.GridOD gridProvIdent;
		#endregion UI Elements
	}
}
