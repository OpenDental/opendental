namespace OpenDental{
	partial class FormMedLabEdit {
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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedLabEdit));
			this.labelTotalVol = new System.Windows.Forms.Label();
			this.textTotVol = new System.Windows.Forms.TextBox();
			this.textDateTCollect = new System.Windows.Forms.TextBox();
			this.labelDateTCollect = new System.Windows.Forms.Label();
			this.textDateTReport = new System.Windows.Forms.TextBox();
			this.labelDateTReport = new System.Windows.Forms.Label();
			this.textDateEntered = new System.Windows.Forms.TextBox();
			this.labelDateEntered = new System.Windows.Forms.Label();
			this.labelAdditionalInfo = new System.Windows.Forms.Label();
			this.textPhysicianName = new System.Windows.Forms.TextBox();
			this.labelPhysicianName = new System.Windows.Forms.Label();
			this.labelPhysicianID = new System.Windows.Forms.Label();
			this.textPhysicianID = new System.Windows.Forms.TextBox();
			this.labelPhysicianNPI = new System.Windows.Forms.Label();
			this.textPhysicianNPI = new System.Windows.Forms.TextBox();
			this.labelTestsOrd = new System.Windows.Forms.Label();
			this.labelGenComments = new System.Windows.Forms.Label();
			this.groupPat = new System.Windows.Forms.GroupBox();
			this.butPatSelect = new OpenDental.UI.Button();
			this.textFasting = new System.Windows.Forms.TextBox();
			this.labelFasting = new System.Windows.Forms.Label();
			this.textGender = new System.Windows.Forms.TextBox();
			this.labelGender = new System.Windows.Forms.Label();
			this.textBirthdate = new System.Windows.Forms.TextBox();
			this.labelBirthdate = new System.Windows.Forms.Label();
			this.textPatSSN = new System.Windows.Forms.TextBox();
			this.labelPatSSN = new System.Windows.Forms.Label();
			this.textPatMiddleI = new System.Windows.Forms.TextBox();
			this.textPatFName = new System.Windows.Forms.TextBox();
			this.textPatLName = new System.Windows.Forms.TextBox();
			this.labelPatMiddleName = new System.Windows.Forms.Label();
			this.labelPatFName = new System.Windows.Forms.Label();
			this.labelPatLName = new System.Windows.Forms.Label();
			this.labelPatID = new System.Windows.Forms.Label();
			this.textPatID = new System.Windows.Forms.TextBox();
			this.labelSpecimenNum = new System.Windows.Forms.Label();
			this.textSpecimenNumber = new System.Windows.Forms.TextBox();
			this.groupOrderingPhys = new System.Windows.Forms.GroupBox();
			this.butProvSelect = new OpenDental.UI.Button();
			this.labelShowHL7 = new System.Windows.Forms.Label();
			this.labelPrint = new System.Windows.Forms.Label();
			this.gridFacilities = new OpenDental.UI.GridOD();
			this.gridResults = new OpenDental.UI.GridOD();
			this.butShowHL7 = new OpenDental.UI.Button();
			this.butPDF = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textGenComments = new OpenDental.ODtextBox();
			this.textTestsOrd = new OpenDental.ODtextBox();
			this.textAddlInfo = new OpenDental.ODtextBox();
			this.groupPat.SuspendLayout();
			this.groupOrderingPhys.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTotalVol
			// 
			this.labelTotalVol.Location = new System.Drawing.Point(307, 185);
			this.labelTotalVol.Name = "labelTotalVol";
			this.labelTotalVol.Size = new System.Drawing.Size(113, 16);
			this.labelTotalVol.TabIndex = 270;
			this.labelTotalVol.Text = "Total Volume";
			this.labelTotalVol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotVol
			// 
			this.textTotVol.Location = new System.Drawing.Point(421, 183);
			this.textTotVol.MaxLength = 100;
			this.textTotVol.Name = "textTotVol";
			this.textTotVol.ReadOnly = true;
			this.textTotVol.Size = new System.Drawing.Size(129, 20);
			this.textTotVol.TabIndex = 271;
			// 
			// textDateTCollect
			// 
			this.textDateTCollect.Location = new System.Drawing.Point(421, 121);
			this.textDateTCollect.MaxLength = 100;
			this.textDateTCollect.Name = "textDateTCollect";
			this.textDateTCollect.ReadOnly = true;
			this.textDateTCollect.Size = new System.Drawing.Size(129, 20);
			this.textDateTCollect.TabIndex = 289;
			// 
			// labelDateTCollect
			// 
			this.labelDateTCollect.Location = new System.Drawing.Point(307, 123);
			this.labelDateTCollect.Name = "labelDateTCollect";
			this.labelDateTCollect.Size = new System.Drawing.Size(113, 16);
			this.labelDateTCollect.TabIndex = 288;
			this.labelDateTCollect.Text = "Date/Time Collected";
			this.labelDateTCollect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTReport
			// 
			this.textDateTReport.Location = new System.Drawing.Point(421, 142);
			this.textDateTReport.MaxLength = 100;
			this.textDateTReport.Name = "textDateTReport";
			this.textDateTReport.ReadOnly = true;
			this.textDateTReport.Size = new System.Drawing.Size(129, 20);
			this.textDateTReport.TabIndex = 291;
			// 
			// labelDateTReport
			// 
			this.labelDateTReport.Location = new System.Drawing.Point(307, 144);
			this.labelDateTReport.Name = "labelDateTReport";
			this.labelDateTReport.Size = new System.Drawing.Size(113, 16);
			this.labelDateTReport.TabIndex = 290;
			this.labelDateTReport.Text = "Date/Time Reported";
			this.labelDateTReport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateEntered
			// 
			this.textDateEntered.Location = new System.Drawing.Point(421, 162);
			this.textDateEntered.MaxLength = 100;
			this.textDateEntered.Name = "textDateEntered";
			this.textDateEntered.ReadOnly = true;
			this.textDateEntered.Size = new System.Drawing.Size(129, 20);
			this.textDateEntered.TabIndex = 293;
			// 
			// labelDateEntered
			// 
			this.labelDateEntered.Location = new System.Drawing.Point(307, 164);
			this.labelDateEntered.Name = "labelDateEntered";
			this.labelDateEntered.Size = new System.Drawing.Size(113, 16);
			this.labelDateEntered.TabIndex = 292;
			this.labelDateEntered.Text = "Date Entered";
			this.labelDateEntered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAdditionalInfo
			// 
			this.labelAdditionalInfo.Location = new System.Drawing.Point(561, 28);
			this.labelAdditionalInfo.Name = "labelAdditionalInfo";
			this.labelAdditionalInfo.Size = new System.Drawing.Size(130, 16);
			this.labelAdditionalInfo.TabIndex = 294;
			this.labelAdditionalInfo.Text = "Additional Information";
			this.labelAdditionalInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhysicianName
			// 
			this.textPhysicianName.Location = new System.Drawing.Point(127, 17);
			this.textPhysicianName.MaxLength = 100;
			this.textPhysicianName.Name = "textPhysicianName";
			this.textPhysicianName.ReadOnly = true;
			this.textPhysicianName.Size = new System.Drawing.Size(125, 20);
			this.textPhysicianName.TabIndex = 307;
			// 
			// labelPhysicianName
			// 
			this.labelPhysicianName.Location = new System.Drawing.Point(6, 19);
			this.labelPhysicianName.Name = "labelPhysicianName";
			this.labelPhysicianName.Size = new System.Drawing.Size(120, 16);
			this.labelPhysicianName.TabIndex = 306;
			this.labelPhysicianName.Text = "Name";
			this.labelPhysicianName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPhysicianID
			// 
			this.labelPhysicianID.Location = new System.Drawing.Point(6, 62);
			this.labelPhysicianID.Name = "labelPhysicianID";
			this.labelPhysicianID.Size = new System.Drawing.Size(120, 16);
			this.labelPhysicianID.TabIndex = 311;
			this.labelPhysicianID.Text = "ID";
			this.labelPhysicianID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhysicianID
			// 
			this.textPhysicianID.Location = new System.Drawing.Point(127, 60);
			this.textPhysicianID.Name = "textPhysicianID";
			this.textPhysicianID.ReadOnly = true;
			this.textPhysicianID.Size = new System.Drawing.Size(157, 20);
			this.textPhysicianID.TabIndex = 310;
			// 
			// labelPhysicianNPI
			// 
			this.labelPhysicianNPI.Location = new System.Drawing.Point(6, 41);
			this.labelPhysicianNPI.Name = "labelPhysicianNPI";
			this.labelPhysicianNPI.Size = new System.Drawing.Size(120, 16);
			this.labelPhysicianNPI.TabIndex = 309;
			this.labelPhysicianNPI.Text = "NPI";
			this.labelPhysicianNPI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhysicianNPI
			// 
			this.textPhysicianNPI.Location = new System.Drawing.Point(127, 39);
			this.textPhysicianNPI.Name = "textPhysicianNPI";
			this.textPhysicianNPI.ReadOnly = true;
			this.textPhysicianNPI.Size = new System.Drawing.Size(157, 20);
			this.textPhysicianNPI.TabIndex = 308;
			// 
			// labelTestsOrd
			// 
			this.labelTestsOrd.Location = new System.Drawing.Point(561, 146);
			this.labelTestsOrd.Name = "labelTestsOrd";
			this.labelTestsOrd.Size = new System.Drawing.Size(130, 16);
			this.labelTestsOrd.TabIndex = 313;
			this.labelTestsOrd.Text = "Tests Ordered";
			this.labelTestsOrd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGenComments
			// 
			this.labelGenComments.Location = new System.Drawing.Point(561, 87);
			this.labelGenComments.Name = "labelGenComments";
			this.labelGenComments.Size = new System.Drawing.Size(130, 16);
			this.labelGenComments.TabIndex = 321;
			this.labelGenComments.Text = "General Comments";
			this.labelGenComments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPat
			// 
			this.groupPat.Controls.Add(this.butPatSelect);
			this.groupPat.Controls.Add(this.textFasting);
			this.groupPat.Controls.Add(this.labelFasting);
			this.groupPat.Controls.Add(this.textGender);
			this.groupPat.Controls.Add(this.labelGender);
			this.groupPat.Controls.Add(this.textBirthdate);
			this.groupPat.Controls.Add(this.labelBirthdate);
			this.groupPat.Controls.Add(this.textPatSSN);
			this.groupPat.Controls.Add(this.labelPatSSN);
			this.groupPat.Controls.Add(this.textPatMiddleI);
			this.groupPat.Controls.Add(this.textPatFName);
			this.groupPat.Controls.Add(this.textPatLName);
			this.groupPat.Controls.Add(this.labelPatMiddleName);
			this.groupPat.Controls.Add(this.labelPatFName);
			this.groupPat.Controls.Add(this.labelPatLName);
			this.groupPat.Controls.Add(this.labelPatID);
			this.groupPat.Controls.Add(this.textPatID);
			this.groupPat.Controls.Add(this.labelSpecimenNum);
			this.groupPat.Controls.Add(this.textSpecimenNumber);
			this.groupPat.Location = new System.Drawing.Point(12, 9);
			this.groupPat.Name = "groupPat";
			this.groupPat.Size = new System.Drawing.Size(544, 106);
			this.groupPat.TabIndex = 326;
			this.groupPat.TabStop = false;
			this.groupPat.Text = "Patient";
			// 
			// butPatSelect
			// 
			this.butPatSelect.Location = new System.Drawing.Point(511, 14);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(27, 24);
			this.butPatSelect.TabIndex = 313;
			this.butPatSelect.Text = "...";
			this.butPatSelect.Click += new System.EventHandler(this.butPatSelect_Click);
			// 
			// textFasting
			// 
			this.textFasting.Location = new System.Drawing.Point(513, 60);
			this.textFasting.MaxLength = 100;
			this.textFasting.Name = "textFasting";
			this.textFasting.ReadOnly = true;
			this.textFasting.Size = new System.Drawing.Size(25, 20);
			this.textFasting.TabIndex = 300;
			// 
			// labelFasting
			// 
			this.labelFasting.Location = new System.Drawing.Point(465, 62);
			this.labelFasting.Name = "labelFasting";
			this.labelFasting.Size = new System.Drawing.Size(47, 16);
			this.labelFasting.TabIndex = 299;
			this.labelFasting.Text = "Fasting";
			this.labelFasting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGender
			// 
			this.textGender.Location = new System.Drawing.Point(409, 60);
			this.textGender.MaxLength = 100;
			this.textGender.Name = "textGender";
			this.textGender.ReadOnly = true;
			this.textGender.Size = new System.Drawing.Size(51, 20);
			this.textGender.TabIndex = 298;
			// 
			// labelGender
			// 
			this.labelGender.Location = new System.Drawing.Point(289, 62);
			this.labelGender.Name = "labelGender";
			this.labelGender.Size = new System.Drawing.Size(119, 16);
			this.labelGender.TabIndex = 297;
			this.labelGender.Text = "Gender";
			this.labelGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(409, 39);
			this.textBirthdate.MaxLength = 100;
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.ReadOnly = true;
			this.textBirthdate.Size = new System.Drawing.Size(129, 20);
			this.textBirthdate.TabIndex = 296;
			// 
			// labelBirthdate
			// 
			this.labelBirthdate.Location = new System.Drawing.Point(289, 41);
			this.labelBirthdate.Name = "labelBirthdate";
			this.labelBirthdate.Size = new System.Drawing.Size(119, 16);
			this.labelBirthdate.TabIndex = 295;
			this.labelBirthdate.Text = "Birthdate";
			this.labelBirthdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatSSN
			// 
			this.textPatSSN.Location = new System.Drawing.Point(409, 81);
			this.textPatSSN.MaxLength = 100;
			this.textPatSSN.Name = "textPatSSN";
			this.textPatSSN.ReadOnly = true;
			this.textPatSSN.Size = new System.Drawing.Size(129, 20);
			this.textPatSSN.TabIndex = 292;
			// 
			// labelPatSSN
			// 
			this.labelPatSSN.Location = new System.Drawing.Point(289, 83);
			this.labelPatSSN.Name = "labelPatSSN";
			this.labelPatSSN.Size = new System.Drawing.Size(119, 16);
			this.labelPatSSN.TabIndex = 291;
			this.labelPatSSN.Text = "SS#";
			this.labelPatSSN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatMiddleI
			// 
			this.textPatMiddleI.Location = new System.Drawing.Point(127, 81);
			this.textPatMiddleI.MaxLength = 100;
			this.textPatMiddleI.Name = "textPatMiddleI";
			this.textPatMiddleI.ReadOnly = true;
			this.textPatMiddleI.Size = new System.Drawing.Size(157, 20);
			this.textPatMiddleI.TabIndex = 290;
			// 
			// textPatFName
			// 
			this.textPatFName.Location = new System.Drawing.Point(127, 60);
			this.textPatFName.MaxLength = 100;
			this.textPatFName.Name = "textPatFName";
			this.textPatFName.ReadOnly = true;
			this.textPatFName.Size = new System.Drawing.Size(157, 20);
			this.textPatFName.TabIndex = 289;
			// 
			// textPatLName
			// 
			this.textPatLName.Location = new System.Drawing.Point(127, 39);
			this.textPatLName.MaxLength = 100;
			this.textPatLName.Name = "textPatLName";
			this.textPatLName.ReadOnly = true;
			this.textPatLName.Size = new System.Drawing.Size(157, 20);
			this.textPatLName.TabIndex = 288;
			// 
			// labelPatMiddleName
			// 
			this.labelPatMiddleName.Location = new System.Drawing.Point(6, 83);
			this.labelPatMiddleName.Name = "labelPatMiddleName";
			this.labelPatMiddleName.Size = new System.Drawing.Size(120, 16);
			this.labelPatMiddleName.TabIndex = 285;
			this.labelPatMiddleName.Text = "Middle Name";
			this.labelPatMiddleName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatFName
			// 
			this.labelPatFName.Location = new System.Drawing.Point(6, 62);
			this.labelPatFName.Name = "labelPatFName";
			this.labelPatFName.Size = new System.Drawing.Size(120, 16);
			this.labelPatFName.TabIndex = 286;
			this.labelPatFName.Text = "First Name";
			this.labelPatFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatLName
			// 
			this.labelPatLName.Location = new System.Drawing.Point(6, 41);
			this.labelPatLName.Name = "labelPatLName";
			this.labelPatLName.Size = new System.Drawing.Size(120, 16);
			this.labelPatLName.TabIndex = 287;
			this.labelPatLName.Text = "Last Name";
			this.labelPatLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatID
			// 
			this.labelPatID.Location = new System.Drawing.Point(289, 19);
			this.labelPatID.Name = "labelPatID";
			this.labelPatID.Size = new System.Drawing.Size(119, 16);
			this.labelPatID.TabIndex = 283;
			this.labelPatID.Text = "Patient ID";
			this.labelPatID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatID
			// 
			this.textPatID.Location = new System.Drawing.Point(409, 17);
			this.textPatID.Name = "textPatID";
			this.textPatID.ReadOnly = true;
			this.textPatID.Size = new System.Drawing.Size(97, 20);
			this.textPatID.TabIndex = 282;
			// 
			// labelSpecimenNum
			// 
			this.labelSpecimenNum.Location = new System.Drawing.Point(6, 19);
			this.labelSpecimenNum.Name = "labelSpecimenNum";
			this.labelSpecimenNum.Size = new System.Drawing.Size(120, 16);
			this.labelSpecimenNum.TabIndex = 281;
			this.labelSpecimenNum.Text = "Specimen Number";
			this.labelSpecimenNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecimenNumber
			// 
			this.textSpecimenNumber.Location = new System.Drawing.Point(127, 17);
			this.textSpecimenNumber.Name = "textSpecimenNumber";
			this.textSpecimenNumber.ReadOnly = true;
			this.textSpecimenNumber.Size = new System.Drawing.Size(157, 20);
			this.textSpecimenNumber.TabIndex = 280;
			// 
			// groupOrderingPhys
			// 
			this.groupOrderingPhys.Controls.Add(this.butProvSelect);
			this.groupOrderingPhys.Controls.Add(this.textPhysicianName);
			this.groupOrderingPhys.Controls.Add(this.labelPhysicianName);
			this.groupOrderingPhys.Controls.Add(this.textPhysicianNPI);
			this.groupOrderingPhys.Controls.Add(this.labelPhysicianNPI);
			this.groupOrderingPhys.Controls.Add(this.textPhysicianID);
			this.groupOrderingPhys.Controls.Add(this.labelPhysicianID);
			this.groupOrderingPhys.Location = new System.Drawing.Point(12, 121);
			this.groupOrderingPhys.Name = "groupOrderingPhys";
			this.groupOrderingPhys.Size = new System.Drawing.Size(290, 85);
			this.groupOrderingPhys.TabIndex = 327;
			this.groupOrderingPhys.TabStop = false;
			this.groupOrderingPhys.Text = "Ordering Physician";
			// 
			// butProvSelect
			// 
			this.butProvSelect.Location = new System.Drawing.Point(257, 14);
			this.butProvSelect.Name = "butProvSelect";
			this.butProvSelect.Size = new System.Drawing.Size(27, 24);
			this.butProvSelect.TabIndex = 312;
			this.butProvSelect.Text = "...";
			this.butProvSelect.Click += new System.EventHandler(this.butProvSelect_Click);
			// 
			// labelShowHL7
			// 
			this.labelShowHL7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelShowHL7.Location = new System.Drawing.Point(552, 664);
			this.labelShowHL7.Name = "labelShowHL7";
			this.labelShowHL7.Size = new System.Drawing.Size(228, 16);
			this.labelShowHL7.TabIndex = 329;
			this.labelShowHL7.Text = "Show the original inbound HL7 message(s).";
			// 
			// labelPrint
			// 
			this.labelPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPrint.Location = new System.Drawing.Point(258, 658);
			this.labelPrint.Name = "labelPrint";
			this.labelPrint.Size = new System.Drawing.Size(193, 28);
			this.labelPrint.TabIndex = 331;
			this.labelPrint.Text = "Create results report PDF and\r\nsave it in the patient\'s image folder.";
			// 
			// gridFacilities
			// 
			this.gridFacilities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridFacilities.Location = new System.Drawing.Point(12, 568);
			this.gridFacilities.Name = "gridFacilities";
			this.gridFacilities.Size = new System.Drawing.Size(950, 85);
			this.gridFacilities.TabIndex = 324;
			this.gridFacilities.Title = "Lab Facilities";
			this.gridFacilities.TranslationName = "TableFacilities";
			// 
			// gridResults
			// 
			this.gridResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridResults.Location = new System.Drawing.Point(12, 209);
			this.gridResults.Name = "gridResults";
			this.gridResults.Size = new System.Drawing.Size(950, 354);
			this.gridResults.TabIndex = 315;
			this.gridResults.Title = "Test Results";
			this.gridResults.TranslationName = "TableTestResults";
			this.gridResults.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridResults_CellDoubleClick);
			// 
			// butShowHL7
			// 
			this.butShowHL7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butShowHL7.Location = new System.Drawing.Point(476, 660);
			this.butShowHL7.Name = "butShowHL7";
			this.butShowHL7.Size = new System.Drawing.Size(75, 24);
			this.butShowHL7.TabIndex = 336;
			this.butShowHL7.Text = "Show HL7";
			this.butShowHL7.Click += new System.EventHandler(this.butShowHL7_Click);
			// 
			// butPDF
			// 
			this.butPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPDF.Location = new System.Drawing.Point(182, 660);
			this.butPDF.Name = "butPDF";
			this.butPDF.Size = new System.Drawing.Size(75, 24);
			this.butPDF.TabIndex = 335;
			this.butPDF.Text = "Create PDF";
			this.butPDF.Click += new System.EventHandler(this.butPDF_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(887, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 334;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(806, 660);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 333;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 660);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 332;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textGenComments
			// 
			this.textGenComments.AcceptsTab = true;
			this.textGenComments.BackColor = System.Drawing.SystemColors.Control;
			this.textGenComments.DetectLinksEnabled = false;
			this.textGenComments.DetectUrls = false;
			this.textGenComments.Location = new System.Drawing.Point(692, 85);
			this.textGenComments.Name = "textGenComments";
			this.textGenComments.QuickPasteType = OpenDentBusiness.QuickPasteType.Lab;
			this.textGenComments.ReadOnly = true;
			this.textGenComments.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textGenComments.Size = new System.Drawing.Size(270, 58);
			this.textGenComments.TabIndex = 320;
			this.textGenComments.Text = "";
			// 
			// textTestsOrd
			// 
			this.textTestsOrd.AcceptsTab = true;
			this.textTestsOrd.BackColor = System.Drawing.SystemColors.Control;
			this.textTestsOrd.DetectLinksEnabled = false;
			this.textTestsOrd.DetectUrls = false;
			this.textTestsOrd.Location = new System.Drawing.Point(692, 144);
			this.textTestsOrd.Name = "textTestsOrd";
			this.textTestsOrd.QuickPasteType = OpenDentBusiness.QuickPasteType.Lab;
			this.textTestsOrd.ReadOnly = true;
			this.textTestsOrd.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTestsOrd.Size = new System.Drawing.Size(270, 58);
			this.textTestsOrd.TabIndex = 312;
			this.textTestsOrd.Text = "";
			// 
			// textAddlInfo
			// 
			this.textAddlInfo.AcceptsTab = true;
			this.textAddlInfo.BackColor = System.Drawing.SystemColors.Control;
			this.textAddlInfo.DetectLinksEnabled = false;
			this.textAddlInfo.DetectUrls = false;
			this.textAddlInfo.Location = new System.Drawing.Point(692, 26);
			this.textAddlInfo.Name = "textAddlInfo";
			this.textAddlInfo.QuickPasteType = OpenDentBusiness.QuickPasteType.Lab;
			this.textAddlInfo.ReadOnly = true;
			this.textAddlInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAddlInfo.Size = new System.Drawing.Size(270, 58);
			this.textAddlInfo.TabIndex = 0;
			this.textAddlInfo.Text = "";
			// 
			// FormMedLabEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.butShowHL7);
			this.Controls.Add(this.butPDF);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelPrint);
			this.Controls.Add(this.labelShowHL7);
			this.Controls.Add(this.groupOrderingPhys);
			this.Controls.Add(this.groupPat);
			this.Controls.Add(this.gridFacilities);
			this.Controls.Add(this.labelGenComments);
			this.Controls.Add(this.textGenComments);
			this.Controls.Add(this.gridResults);
			this.Controls.Add(this.labelTestsOrd);
			this.Controls.Add(this.textTestsOrd);
			this.Controls.Add(this.labelAdditionalInfo);
			this.Controls.Add(this.textAddlInfo);
			this.Controls.Add(this.textDateEntered);
			this.Controls.Add(this.labelDateEntered);
			this.Controls.Add(this.textDateTReport);
			this.Controls.Add(this.labelDateTReport);
			this.Controls.Add(this.textDateTCollect);
			this.Controls.Add(this.labelDateTCollect);
			this.Controls.Add(this.textTotVol);
			this.Controls.Add(this.labelTotalVol);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMedLabEdit";
			this.Text = "Medical Lab Edit";
			this.Load += new System.EventHandler(this.FormMedLabEdit_Load);
			this.groupPat.ResumeLayout(false);
			this.groupPat.PerformLayout();
			this.groupOrderingPhys.ResumeLayout(false);
			this.groupOrderingPhys.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelTotalVol;
		private System.Windows.Forms.TextBox textTotVol;
		private System.Windows.Forms.TextBox textDateTCollect;
		private System.Windows.Forms.Label labelDateTCollect;
		private System.Windows.Forms.TextBox textDateTReport;
		private System.Windows.Forms.Label labelDateTReport;
		private System.Windows.Forms.TextBox textDateEntered;
		private System.Windows.Forms.Label labelDateEntered;
		private ODtextBox textAddlInfo;
		private System.Windows.Forms.Label labelAdditionalInfo;
		private System.Windows.Forms.TextBox textPhysicianName;
		private System.Windows.Forms.Label labelPhysicianName;
		private System.Windows.Forms.Label labelPhysicianID;
		private System.Windows.Forms.TextBox textPhysicianID;
		private System.Windows.Forms.Label labelPhysicianNPI;
		private System.Windows.Forms.TextBox textPhysicianNPI;
		private System.Windows.Forms.Label labelTestsOrd;
		private ODtextBox textTestsOrd;
		private UI.GridOD gridResults;
		private System.Windows.Forms.Label labelGenComments;
		private ODtextBox textGenComments;
		private UI.GridOD gridFacilities;
		private System.Windows.Forms.GroupBox groupPat;
		private System.Windows.Forms.TextBox textFasting;
		private System.Windows.Forms.Label labelFasting;
		private System.Windows.Forms.TextBox textGender;
		private System.Windows.Forms.Label labelGender;
		private System.Windows.Forms.TextBox textBirthdate;
		private System.Windows.Forms.Label labelBirthdate;
		private System.Windows.Forms.TextBox textPatSSN;
		private System.Windows.Forms.Label labelPatSSN;
		private System.Windows.Forms.TextBox textPatMiddleI;
		private System.Windows.Forms.TextBox textPatFName;
		private System.Windows.Forms.TextBox textPatLName;
		private System.Windows.Forms.Label labelPatMiddleName;
		private System.Windows.Forms.Label labelPatFName;
		private System.Windows.Forms.Label labelPatLName;
		private System.Windows.Forms.Label labelPatID;
		private System.Windows.Forms.TextBox textPatID;
		private System.Windows.Forms.Label labelSpecimenNum;
		private System.Windows.Forms.TextBox textSpecimenNumber;
		private System.Windows.Forms.GroupBox groupOrderingPhys;
		private System.Windows.Forms.Label labelShowHL7;
		private System.Windows.Forms.Label labelPrint;
		private UI.Button butDelete;
		private UI.Button butOK;
		private UI.Button butPatSelect;
		private UI.Button butProvSelect;
		private UI.Button butCancel;
		private UI.Button butPDF;
		private UI.Button butShowHL7;

	}
}