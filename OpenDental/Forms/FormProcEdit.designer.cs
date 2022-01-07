namespace OpenDental {
	partial class FormProcEdit {
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

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcEdit));
			this.labelDate = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelTooth = new System.Windows.Forms.Label();
			this.labelSurfaces = new System.Windows.Forms.Label();
			this.labelAmount = new System.Windows.Forms.Label();
			this.textProc = new System.Windows.Forms.TextBox();
			this.textTooth = new System.Windows.Forms.TextBox();
			this.textSurfaces = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textDesc = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.labelRange = new System.Windows.Forms.Label();
			this.textRange = new System.Windows.Forms.TextBox();
			this.groupQuadrant = new System.Windows.Forms.GroupBox();
			this.radioLR = new System.Windows.Forms.RadioButton();
			this.radioLL = new System.Windows.Forms.RadioButton();
			this.radioUL = new System.Windows.Forms.RadioButton();
			this.radioUR = new System.Windows.Forms.RadioButton();
			this.groupArch = new System.Windows.Forms.GroupBox();
			this.radioL = new System.Windows.Forms.RadioButton();
			this.radioU = new System.Windows.Forms.RadioButton();
			this.panelSurfaces = new System.Windows.Forms.Panel();
			this.butD = new OpenDental.UI.Button();
			this.butBF = new OpenDental.UI.Button();
			this.butL = new OpenDental.UI.Button();
			this.butM = new OpenDental.UI.Button();
			this.butV = new OpenDental.UI.Button();
			this.butOI = new OpenDental.UI.Button();
			this.groupSextant = new System.Windows.Forms.GroupBox();
			this.radioS6 = new System.Windows.Forms.RadioButton();
			this.radioS5 = new System.Windows.Forms.RadioButton();
			this.radioS4 = new System.Windows.Forms.RadioButton();
			this.radioS2 = new System.Windows.Forms.RadioButton();
			this.radioS3 = new System.Windows.Forms.RadioButton();
			this.radioS1 = new System.Windows.Forms.RadioButton();
			this.label9 = new System.Windows.Forms.Label();
			this.labelDx = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelTaxEst = new System.Windows.Forms.Label();
			this.textTaxAmt = new OpenDental.ValidDouble();
			this.textOrigDateComp = new OpenDental.ValidDate();
			this.labelOrigDateComp = new System.Windows.Forms.Label();
			this.textTimeFinal = new System.Windows.Forms.TextBox();
			this.textTimeStart = new System.Windows.Forms.TextBox();
			this.textTimeEnd = new System.Windows.Forms.TextBox();
			this.textDate = new OpenDental.ValidDate();
			this.butNow = new OpenDental.UI.Button();
			this.textDateTP = new OpenDental.ValidDate();
			this.label27 = new System.Windows.Forms.Label();
			this.listBoxTeeth = new System.Windows.Forms.ListBox();
			this.textDateEntry = new OpenDental.ValidDate();
			this.label12 = new System.Windows.Forms.Label();
			this.textProcFee = new OpenDental.ValidDouble();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.labelEndTime = new System.Windows.Forms.Label();
			this.listBoxTeeth2 = new System.Windows.Forms.ListBox();
			this.butChange = new OpenDental.UI.Button();
			this.labelTimeFinal = new System.Windows.Forms.Label();
			this.textDrugQty = new System.Windows.Forms.TextBox();
			this.labelDrugQty = new System.Windows.Forms.Label();
			this.labelDrugNDC = new System.Windows.Forms.Label();
			this.textDrugNDC = new System.Windows.Forms.TextBox();
			this.comboDrugUnit = new System.Windows.Forms.ComboBox();
			this.labelDrugUnit = new System.Windows.Forms.Label();
			this.textRevCode = new System.Windows.Forms.TextBox();
			this.labelRevCode = new System.Windows.Forms.Label();
			this.textUnitQty = new System.Windows.Forms.TextBox();
			this.labelUnitQty = new System.Windows.Forms.Label();
			this.textCodeMod4 = new System.Windows.Forms.TextBox();
			this.textCodeMod3 = new System.Windows.Forms.TextBox();
			this.textCodeMod2 = new System.Windows.Forms.TextBox();
			this.labelCodeMods = new System.Windows.Forms.Label();
			this.textCodeMod1 = new System.Windows.Forms.TextBox();
			this.checkIsPrincDiag = new System.Windows.Forms.CheckBox();
			this.labelDiagnosisCode = new System.Windows.Forms.Label();
			this.textDiagnosisCode = new System.Windows.Forms.TextBox();
			this.labelMedicalCode = new System.Windows.Forms.Label();
			this.textMedicalCode = new System.Windows.Forms.TextBox();
			this.labelClaim = new System.Windows.Forms.Label();
			this.comboPlaceService = new System.Windows.Forms.ComboBox();
			this.labelPlaceService = new System.Windows.Forms.Label();
			this.labelPriority = new System.Windows.Forms.Label();
			this.labelSetComplete = new System.Windows.Forms.Label();
			this.checkNoBillIns = new System.Windows.Forms.CheckBox();
			this.labelIncomplete = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.comboPriority = new System.Windows.Forms.ComboBox();
			this.comboDx = new System.Windows.Forms.ComboBox();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.textUser = new System.Windows.Forms.TextBox();
			this.comboBillingTypeTwo = new System.Windows.Forms.ComboBox();
			this.labelBillingTypeTwo = new System.Windows.Forms.Label();
			this.comboBillingTypeOne = new System.Windows.Forms.ComboBox();
			this.labelBillingTypeOne = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textSite = new System.Windows.Forms.TextBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.checkHideGraphics = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.listProsth = new OpenDental.UI.ListBoxOD();
			this.groupProsth = new System.Windows.Forms.GroupBox();
			this.checkIsDateProsthEst = new System.Windows.Forms.CheckBox();
			this.textDateOriginalProsth = new OpenDental.ValidDate();
			this.checkTypeCodeA = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeB = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeC = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeE = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeL = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeX = new System.Windows.Forms.CheckBox();
			this.checkTypeCodeS = new System.Windows.Forms.CheckBox();
			this.groupCanadianProcTypeCode = new System.Windows.Forms.GroupBox();
			this.comboPrognosis = new System.Windows.Forms.ComboBox();
			this.labelPrognosis = new System.Windows.Forms.Label();
			this.comboProcStatus = new OpenDental.UI.ComboBoxOD();
			this.label13 = new System.Windows.Forms.Label();
			this.textReferral = new System.Windows.Forms.TextBox();
			this.labelClaimNote = new System.Windows.Forms.Label();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageFinancial = new System.Windows.Forms.TabPage();
			this.butAddExistAdj = new OpenDental.UI.Button();
			this.gridPay = new OpenDental.UI.GridOD();
			this.gridAdj = new OpenDental.UI.GridOD();
			this.label20 = new System.Windows.Forms.Label();
			this.textDiscount = new OpenDental.ValidDouble();
			this.butAddEstimate = new OpenDental.UI.Button();
			this.butAddAdjust = new OpenDental.UI.Button();
			this.gridIns = new OpenDental.UI.GridOD();
			this.tabPageMedical = new System.Windows.Forms.TabPage();
			this.checkIsEmergency = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butPickOrderProvInternal = new OpenDental.UI.Button();
			this.textOrderingProviderOverride = new OpenDental.ODtextBox();
			this.butPickOrderProvReferral = new OpenDental.UI.Button();
			this.butNoneOrderProv = new OpenDental.UI.Button();
			this.labelIcdVersionUncheck = new System.Windows.Forms.Label();
			this.checkIcdVersion = new System.Windows.Forms.CheckBox();
			this.butNoneDiagnosisCode = new OpenDental.UI.Button();
			this.butDiagnosisCode = new OpenDental.UI.Button();
			this.butNoneDiagnosisCode2 = new OpenDental.UI.Button();
			this.butDiagnosisCode2 = new OpenDental.UI.Button();
			this.butNoneDiagnosisCode4 = new OpenDental.UI.Button();
			this.butDiagnosisCode4 = new OpenDental.UI.Button();
			this.butNoneDiagnosisCode3 = new OpenDental.UI.Button();
			this.butDiagnosisCode3 = new OpenDental.UI.Button();
			this.textDiagnosisCode2 = new System.Windows.Forms.TextBox();
			this.labelDiagnosisCode2 = new System.Windows.Forms.Label();
			this.textDiagnosisCode3 = new System.Windows.Forms.TextBox();
			this.labelDiagnosisCode3 = new System.Windows.Forms.Label();
			this.textDiagnosisCode4 = new System.Windows.Forms.TextBox();
			this.labelDiagnosisCode4 = new System.Windows.Forms.Label();
			this.butNoneSnomedBodySite = new OpenDental.UI.Button();
			this.butSnomedBodySiteSelect = new OpenDental.UI.Button();
			this.labelSnomedBodySite = new System.Windows.Forms.Label();
			this.textSnomedBodySite = new System.Windows.Forms.TextBox();
			this.labelUnitType = new System.Windows.Forms.Label();
			this.comboUnitType = new System.Windows.Forms.ComboBox();
			this.tabPageMisc = new System.Windows.Forms.TabPage();
			this.textBillingNote = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.butPickSite = new OpenDental.UI.Button();
			this.tabPageCanada = new System.Windows.Forms.TabPage();
			this.labelCanadaLabFee2 = new System.Windows.Forms.Label();
			this.labelCanadaLabFee1 = new System.Windows.Forms.Label();
			this.textCanadaLabFee2 = new OpenDental.ValidDouble();
			this.textCanadaLabFee1 = new OpenDental.ValidDouble();
			this.labelLocked = new System.Windows.Forms.Label();
			this.butSearch = new OpenDental.UI.Button();
			this.butLock = new OpenDental.UI.Button();
			this.butInvalidate = new OpenDental.UI.Button();
			this.butAppend = new OpenDental.UI.Button();
			this.textClaimNote = new OpenDental.ODtextBox();
			this.butReferral = new OpenDental.UI.Button();
			this.butPickProv = new OpenDental.UI.Button();
			this.buttonUseAutoNote = new OpenDental.UI.Button();
			this.textNotes = new OpenDental.ODtextBox();
			this.butSetComplete = new OpenDental.UI.Button();
			this.butEditAnyway = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.butChangeUser = new OpenDental.UI.Button();
			this.labelPermAlert = new System.Windows.Forms.Label();
			this.butEditAutoNote = new OpenDental.UI.Button();
			this.groupQuadrant.SuspendLayout();
			this.groupArch.SuspendLayout();
			this.panelSurfaces.SuspendLayout();
			this.groupSextant.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupProsth.SuspendLayout();
			this.groupCanadianProcTypeCode.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPageFinancial.SuspendLayout();
			this.tabPageMedical.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageMisc.SuspendLayout();
			this.tabPageCanada.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelDate
			// 
			this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDate.Location = new System.Drawing.Point(8, 45);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(96, 14);
			this.labelDate.TabIndex = 0;
			this.labelDate.Text = "Date";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(26, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "Procedure";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelTooth
			// 
			this.labelTooth.Location = new System.Drawing.Point(68, 107);
			this.labelTooth.Name = "labelTooth";
			this.labelTooth.Size = new System.Drawing.Size(36, 12);
			this.labelTooth.TabIndex = 2;
			this.labelTooth.Text = "Tooth";
			this.labelTooth.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelTooth.Visible = false;
			// 
			// labelSurfaces
			// 
			this.labelSurfaces.Location = new System.Drawing.Point(33, 135);
			this.labelSurfaces.Name = "labelSurfaces";
			this.labelSurfaces.Size = new System.Drawing.Size(73, 16);
			this.labelSurfaces.TabIndex = 3;
			this.labelSurfaces.Text = "Surfaces";
			this.labelSurfaces.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelSurfaces.Visible = false;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(30, 195);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(75, 16);
			this.labelAmount.TabIndex = 4;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProc
			// 
			this.textProc.Location = new System.Drawing.Point(106, 61);
			this.textProc.Name = "textProc";
			this.textProc.ReadOnly = true;
			this.textProc.Size = new System.Drawing.Size(76, 20);
			this.textProc.TabIndex = 6;
			// 
			// textTooth
			// 
			this.textTooth.Location = new System.Drawing.Point(106, 105);
			this.textTooth.Name = "textTooth";
			this.textTooth.Size = new System.Drawing.Size(35, 20);
			this.textTooth.TabIndex = 7;
			this.textTooth.Visible = false;
			this.textTooth.Validating += new System.ComponentModel.CancelEventHandler(this.textTooth_Validating);
			// 
			// textSurfaces
			// 
			this.textSurfaces.Location = new System.Drawing.Point(106, 133);
			this.textSurfaces.Name = "textSurfaces";
			this.textSurfaces.Size = new System.Drawing.Size(68, 20);
			this.textSurfaces.TabIndex = 4;
			this.textSurfaces.Visible = false;
			this.textSurfaces.TextChanged += new System.EventHandler(this.textSurfaces_TextChanged);
			this.textSurfaces.Validating += new System.ComponentModel.CancelEventHandler(this.textSurfaces_Validating);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(2, 81);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 16);
			this.label6.TabIndex = 13;
			this.label6.Text = "Description";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDesc
			// 
			this.textDesc.BackColor = System.Drawing.SystemColors.Control;
			this.textDesc.Location = new System.Drawing.Point(106, 81);
			this.textDesc.Name = "textDesc";
			this.textDesc.Size = new System.Drawing.Size(284, 20);
			this.textDesc.TabIndex = 16;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(429, 163);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "&Notes";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelRange
			// 
			this.labelRange.Location = new System.Drawing.Point(24, 107);
			this.labelRange.Name = "labelRange";
			this.labelRange.Size = new System.Drawing.Size(82, 16);
			this.labelRange.TabIndex = 33;
			this.labelRange.Text = "Tooth Range";
			this.labelRange.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelRange.Visible = false;
			// 
			// textRange
			// 
			this.textRange.Location = new System.Drawing.Point(106, 105);
			this.textRange.Name = "textRange";
			this.textRange.Size = new System.Drawing.Size(100, 20);
			this.textRange.TabIndex = 34;
			this.textRange.Visible = false;
			// 
			// groupQuadrant
			// 
			this.groupQuadrant.Controls.Add(this.radioLR);
			this.groupQuadrant.Controls.Add(this.radioLL);
			this.groupQuadrant.Controls.Add(this.radioUL);
			this.groupQuadrant.Controls.Add(this.radioUR);
			this.groupQuadrant.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupQuadrant.Location = new System.Drawing.Point(104, 134);
			this.groupQuadrant.Name = "groupQuadrant";
			this.groupQuadrant.Size = new System.Drawing.Size(108, 56);
			this.groupQuadrant.TabIndex = 36;
			this.groupQuadrant.TabStop = false;
			this.groupQuadrant.Text = "Quadrant";
			this.groupQuadrant.Visible = false;
			// 
			// radioLR
			// 
			this.radioLR.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioLR.Location = new System.Drawing.Point(12, 36);
			this.radioLR.Name = "radioLR";
			this.radioLR.Size = new System.Drawing.Size(40, 16);
			this.radioLR.TabIndex = 3;
			this.radioLR.Text = "LR";
			this.radioLR.Click += new System.EventHandler(this.radioLR_Click);
			// 
			// radioLL
			// 
			this.radioLL.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioLL.Location = new System.Drawing.Point(64, 36);
			this.radioLL.Name = "radioLL";
			this.radioLL.Size = new System.Drawing.Size(40, 16);
			this.radioLL.TabIndex = 1;
			this.radioLL.Text = "LL";
			this.radioLL.Click += new System.EventHandler(this.radioLL_Click);
			// 
			// radioUL
			// 
			this.radioUL.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioUL.Location = new System.Drawing.Point(64, 16);
			this.radioUL.Name = "radioUL";
			this.radioUL.Size = new System.Drawing.Size(40, 16);
			this.radioUL.TabIndex = 0;
			this.radioUL.Text = "UL";
			this.radioUL.Click += new System.EventHandler(this.radioUL_Click);
			// 
			// radioUR
			// 
			this.radioUR.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioUR.Location = new System.Drawing.Point(12, 16);
			this.radioUR.Name = "radioUR";
			this.radioUR.Size = new System.Drawing.Size(40, 16);
			this.radioUR.TabIndex = 0;
			this.radioUR.Text = "UR";
			this.radioUR.Click += new System.EventHandler(this.radioUR_Click);
			// 
			// groupArch
			// 
			this.groupArch.Controls.Add(this.radioL);
			this.groupArch.Controls.Add(this.radioU);
			this.groupArch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupArch.Location = new System.Drawing.Point(104, 134);
			this.groupArch.Name = "groupArch";
			this.groupArch.Size = new System.Drawing.Size(60, 56);
			this.groupArch.TabIndex = 3;
			this.groupArch.TabStop = false;
			this.groupArch.Text = "Arch";
			this.groupArch.Visible = false;
			this.groupArch.Validating += new System.ComponentModel.CancelEventHandler(this.groupArch_Validating);
			// 
			// radioL
			// 
			this.radioL.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioL.Location = new System.Drawing.Point(12, 36);
			this.radioL.Name = "radioL";
			this.radioL.Size = new System.Drawing.Size(28, 16);
			this.radioL.TabIndex = 1;
			this.radioL.Text = "L";
			this.radioL.Click += new System.EventHandler(this.radioL_Click);
			// 
			// radioU
			// 
			this.radioU.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioU.Location = new System.Drawing.Point(12, 16);
			this.radioU.Name = "radioU";
			this.radioU.Size = new System.Drawing.Size(32, 16);
			this.radioU.TabIndex = 0;
			this.radioU.Text = "U";
			this.radioU.Click += new System.EventHandler(this.radioU_Click);
			// 
			// panelSurfaces
			// 
			this.panelSurfaces.Controls.Add(this.butD);
			this.panelSurfaces.Controls.Add(this.butBF);
			this.panelSurfaces.Controls.Add(this.butL);
			this.panelSurfaces.Controls.Add(this.butM);
			this.panelSurfaces.Controls.Add(this.butV);
			this.panelSurfaces.Controls.Add(this.butOI);
			this.panelSurfaces.Location = new System.Drawing.Point(177, 109);
			this.panelSurfaces.Name = "panelSurfaces";
			this.panelSurfaces.Size = new System.Drawing.Size(96, 66);
			this.panelSurfaces.TabIndex = 100;
			this.panelSurfaces.Visible = false;
			// 
			// butD
			// 
			this.butD.BackColor = System.Drawing.SystemColors.Control;
			this.butD.Location = new System.Drawing.Point(61, 23);
			this.butD.Name = "butD";
			this.butD.Size = new System.Drawing.Size(24, 20);
			this.butD.TabIndex = 27;
			this.butD.Text = "D";
			this.butD.UseVisualStyleBackColor = false;
			this.butD.Click += new System.EventHandler(this.butD_Click);
			// 
			// butBF
			// 
			this.butBF.BackColor = System.Drawing.SystemColors.Control;
			this.butBF.Location = new System.Drawing.Point(22, 3);
			this.butBF.Name = "butBF";
			this.butBF.Size = new System.Drawing.Size(28, 20);
			this.butBF.TabIndex = 28;
			this.butBF.Text = "B/F";
			this.butBF.UseVisualStyleBackColor = false;
			this.butBF.Click += new System.EventHandler(this.butBF_Click);
			// 
			// butL
			// 
			this.butL.BackColor = System.Drawing.SystemColors.Control;
			this.butL.Location = new System.Drawing.Point(32, 43);
			this.butL.Name = "butL";
			this.butL.Size = new System.Drawing.Size(24, 20);
			this.butL.TabIndex = 29;
			this.butL.Text = "L";
			this.butL.UseVisualStyleBackColor = false;
			this.butL.Click += new System.EventHandler(this.butL_Click);
			// 
			// butM
			// 
			this.butM.BackColor = System.Drawing.SystemColors.Control;
			this.butM.Location = new System.Drawing.Point(3, 23);
			this.butM.Name = "butM";
			this.butM.Size = new System.Drawing.Size(24, 20);
			this.butM.TabIndex = 25;
			this.butM.Text = "M";
			this.butM.UseVisualStyleBackColor = false;
			this.butM.Click += new System.EventHandler(this.butM_Click);
			// 
			// butV
			// 
			this.butV.BackColor = System.Drawing.SystemColors.Control;
			this.butV.Location = new System.Drawing.Point(50, 3);
			this.butV.Name = "butV";
			this.butV.Size = new System.Drawing.Size(17, 20);
			this.butV.TabIndex = 30;
			this.butV.Text = "V";
			this.butV.UseVisualStyleBackColor = false;
			this.butV.Click += new System.EventHandler(this.butV_Click);
			// 
			// butOI
			// 
			this.butOI.BackColor = System.Drawing.SystemColors.Control;
			this.butOI.Location = new System.Drawing.Point(27, 23);
			this.butOI.Name = "butOI";
			this.butOI.Size = new System.Drawing.Size(34, 20);
			this.butOI.TabIndex = 26;
			this.butOI.Text = "O/I";
			this.butOI.UseVisualStyleBackColor = false;
			this.butOI.Click += new System.EventHandler(this.butOI_Click);
			// 
			// groupSextant
			// 
			this.groupSextant.Controls.Add(this.radioS6);
			this.groupSextant.Controls.Add(this.radioS5);
			this.groupSextant.Controls.Add(this.radioS4);
			this.groupSextant.Controls.Add(this.radioS2);
			this.groupSextant.Controls.Add(this.radioS3);
			this.groupSextant.Controls.Add(this.radioS1);
			this.groupSextant.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupSextant.Location = new System.Drawing.Point(104, 134);
			this.groupSextant.Name = "groupSextant";
			this.groupSextant.Size = new System.Drawing.Size(156, 56);
			this.groupSextant.TabIndex = 5;
			this.groupSextant.TabStop = false;
			this.groupSextant.Text = "Sextant";
			this.groupSextant.Visible = false;
			this.groupSextant.Validating += new System.ComponentModel.CancelEventHandler(this.groupSextant_Validating);
			// 
			// radioS6
			// 
			this.radioS6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS6.Location = new System.Drawing.Point(12, 36);
			this.radioS6.Name = "radioS6";
			this.radioS6.Size = new System.Drawing.Size(36, 16);
			this.radioS6.TabIndex = 5;
			this.radioS6.Text = "6";
			this.radioS6.Click += new System.EventHandler(this.radioS6_Click);
			// 
			// radioS5
			// 
			this.radioS5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS5.Location = new System.Drawing.Point(60, 36);
			this.radioS5.Name = "radioS5";
			this.radioS5.Size = new System.Drawing.Size(36, 16);
			this.radioS5.TabIndex = 4;
			this.radioS5.Text = "5";
			this.radioS5.Click += new System.EventHandler(this.radioS5_Click);
			// 
			// radioS4
			// 
			this.radioS4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS4.Location = new System.Drawing.Point(108, 36);
			this.radioS4.Name = "radioS4";
			this.radioS4.Size = new System.Drawing.Size(36, 16);
			this.radioS4.TabIndex = 1;
			this.radioS4.Text = "4";
			this.radioS4.Click += new System.EventHandler(this.radioS4_Click);
			// 
			// radioS2
			// 
			this.radioS2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS2.Location = new System.Drawing.Point(60, 16);
			this.radioS2.Name = "radioS2";
			this.radioS2.Size = new System.Drawing.Size(36, 16);
			this.radioS2.TabIndex = 2;
			this.radioS2.Text = "2";
			this.radioS2.Click += new System.EventHandler(this.radioS2_Click);
			// 
			// radioS3
			// 
			this.radioS3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS3.Location = new System.Drawing.Point(108, 16);
			this.radioS3.Name = "radioS3";
			this.radioS3.Size = new System.Drawing.Size(36, 16);
			this.radioS3.TabIndex = 0;
			this.radioS3.Text = "3";
			this.radioS3.Click += new System.EventHandler(this.radioS3_Click);
			// 
			// radioS1
			// 
			this.radioS1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioS1.Location = new System.Drawing.Point(12, 16);
			this.radioS1.Name = "radioS1";
			this.radioS1.Size = new System.Drawing.Size(36, 16);
			this.radioS1.TabIndex = 0;
			this.radioS1.Text = "1";
			this.radioS1.Click += new System.EventHandler(this.radioS1_Click);
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(403, 53);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 14);
			this.label9.TabIndex = 45;
			this.label9.Text = "Provider";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDx
			// 
			this.labelDx.Location = new System.Drawing.Point(5, 242);
			this.labelDx.Name = "labelDx";
			this.labelDx.Size = new System.Drawing.Size(100, 14);
			this.labelDx.TabIndex = 46;
			this.labelDx.Text = "Diagnosis";
			this.labelDx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.AllowDrop = true;
			this.panel1.Controls.Add(this.textTooth);
			this.panel1.Controls.Add(this.labelTaxEst);
			this.panel1.Controls.Add(this.textTaxAmt);
			this.panel1.Controls.Add(this.textOrigDateComp);
			this.panel1.Controls.Add(this.labelOrigDateComp);
			this.panel1.Controls.Add(this.textTimeFinal);
			this.panel1.Controls.Add(this.textTimeStart);
			this.panel1.Controls.Add(this.textTimeEnd);
			this.panel1.Controls.Add(this.textDate);
			this.panel1.Controls.Add(this.butNow);
			this.panel1.Controls.Add(this.panelSurfaces);
			this.panel1.Controls.Add(this.textDateTP);
			this.panel1.Controls.Add(this.label27);
			this.panel1.Controls.Add(this.listBoxTeeth);
			this.panel1.Controls.Add(this.textDesc);
			this.panel1.Controls.Add(this.textDateEntry);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.labelTooth);
			this.panel1.Controls.Add(this.labelSurfaces);
			this.panel1.Controls.Add(this.labelAmount);
			this.panel1.Controls.Add(this.textSurfaces);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.groupArch);
			this.panel1.Controls.Add(this.groupQuadrant);
			this.panel1.Controls.Add(this.textProcFee);
			this.panel1.Controls.Add(this.labelStartTime);
			this.panel1.Controls.Add(this.labelEndTime);
			this.panel1.Controls.Add(this.labelRange);
			this.panel1.Controls.Add(this.labelDate);
			this.panel1.Controls.Add(this.textProc);
			this.panel1.Controls.Add(this.listBoxTeeth2);
			this.panel1.Controls.Add(this.textRange);
			this.panel1.Controls.Add(this.butChange);
			this.panel1.Controls.Add(this.groupSextant);
			this.panel1.Controls.Add(this.labelTimeFinal);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(397, 214);
			this.panel1.TabIndex = 2;
			// 
			// labelTaxEst
			// 
			this.labelTaxEst.Location = new System.Drawing.Point(178, 194);
			this.labelTaxEst.Name = "labelTaxEst";
			this.labelTaxEst.Size = new System.Drawing.Size(75, 16);
			this.labelTaxEst.TabIndex = 107;
			this.labelTaxEst.Text = "Tax Est";
			this.labelTaxEst.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelTaxEst.Visible = false;
			// 
			// textTaxAmt
			// 
			this.textTaxAmt.Location = new System.Drawing.Point(254, 191);
			this.textTaxAmt.MaxVal = 100000000D;
			this.textTaxAmt.MinVal = -100000000D;
			this.textTaxAmt.Name = "textTaxAmt";
			this.textTaxAmt.ReadOnly = true;
			this.textTaxAmt.Size = new System.Drawing.Size(68, 20);
			this.textTaxAmt.TabIndex = 108;
			this.textTaxAmt.Visible = false;
			// 
			// textOrigDateComp
			// 
			this.textOrigDateComp.Location = new System.Drawing.Point(314, 1);
			this.textOrigDateComp.Name = "textOrigDateComp";
			this.textOrigDateComp.ReadOnly = true;
			this.textOrigDateComp.Size = new System.Drawing.Size(76, 20);
			this.textOrigDateComp.TabIndex = 105;
			// 
			// labelOrigDateComp
			// 
			this.labelOrigDateComp.Location = new System.Drawing.Point(205, 3);
			this.labelOrigDateComp.Name = "labelOrigDateComp";
			this.labelOrigDateComp.Size = new System.Drawing.Size(109, 18);
			this.labelOrigDateComp.TabIndex = 106;
			this.labelOrigDateComp.Text = "Original Date Comp";
			this.labelOrigDateComp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeFinal
			// 
			this.textTimeFinal.Location = new System.Drawing.Point(314, 61);
			this.textTimeFinal.Name = "textTimeFinal";
			this.textTimeFinal.Size = new System.Drawing.Size(55, 20);
			this.textTimeFinal.TabIndex = 104;
			this.textTimeFinal.Visible = false;
			// 
			// textTimeStart
			// 
			this.textTimeStart.Location = new System.Drawing.Point(236, 41);
			this.textTimeStart.Name = "textTimeStart";
			this.textTimeStart.Size = new System.Drawing.Size(55, 20);
			this.textTimeStart.TabIndex = 102;
			this.textTimeStart.TextChanged += new System.EventHandler(this.textTimeStart_TextChanged);
			// 
			// textTimeEnd
			// 
			this.textTimeEnd.Location = new System.Drawing.Point(314, 41);
			this.textTimeEnd.Name = "textTimeEnd";
			this.textTimeEnd.Size = new System.Drawing.Size(55, 20);
			this.textTimeEnd.TabIndex = 102;
			this.textTimeEnd.Visible = false;
			this.textTimeEnd.TextChanged += new System.EventHandler(this.textTimeEnd_TextChanged);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(106, 41);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(76, 20);
			this.textDate.TabIndex = 102;
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(369, 41);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(27, 20);
			this.butNow.TabIndex = 101;
			this.butNow.Text = "Now";
			this.butNow.UseVisualStyleBackColor = true;
			this.butNow.Visible = false;
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// textDateTP
			// 
			this.textDateTP.Location = new System.Drawing.Point(106, 21);
			this.textDateTP.Name = "textDateTP";
			this.textDateTP.Size = new System.Drawing.Size(76, 20);
			this.textDateTP.TabIndex = 99;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(34, 25);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(70, 14);
			this.label27.TabIndex = 98;
			this.label27.Text = "Date TP";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxTeeth
			// 
			this.listBoxTeeth.AllowDrop = true;
			this.listBoxTeeth.ColumnWidth = 16;
			this.listBoxTeeth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxTeeth.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
			this.listBoxTeeth.Location = new System.Drawing.Point(106, 101);
			this.listBoxTeeth.MultiColumn = true;
			this.listBoxTeeth.Name = "listBoxTeeth";
			this.listBoxTeeth.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.listBoxTeeth.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxTeeth.Size = new System.Drawing.Size(275, 17);
			this.listBoxTeeth.TabIndex = 1;
			this.listBoxTeeth.Visible = false;
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(106, 1);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(76, 20);
			this.textDateEntry.TabIndex = 95;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(3, 3);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(103, 18);
			this.label12.TabIndex = 96;
			this.label12.Text = "Date Entry";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProcFee
			// 
			this.textProcFee.Location = new System.Drawing.Point(106, 192);
			this.textProcFee.MaxVal = 100000000D;
			this.textProcFee.MinVal = -100000000D;
			this.textProcFee.Name = "textProcFee";
			this.textProcFee.Size = new System.Drawing.Size(68, 20);
			this.textProcFee.TabIndex = 6;
			this.textProcFee.Validating += new System.ComponentModel.CancelEventHandler(this.textProcFee_Validating);
			// 
			// labelStartTime
			// 
			this.labelStartTime.Location = new System.Drawing.Point(181, 44);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(56, 14);
			this.labelStartTime.TabIndex = 0;
			this.labelStartTime.Text = "Time Start";
			this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEndTime
			// 
			this.labelEndTime.Location = new System.Drawing.Point(259, 44);
			this.labelEndTime.Name = "labelEndTime";
			this.labelEndTime.Size = new System.Drawing.Size(56, 14);
			this.labelEndTime.TabIndex = 0;
			this.labelEndTime.Text = "End";
			this.labelEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelEndTime.Visible = false;
			// 
			// listBoxTeeth2
			// 
			this.listBoxTeeth2.ColumnWidth = 16;
			this.listBoxTeeth2.Items.AddRange(new object[] {
            "32",
            "31",
            "30",
            "29",
            "28",
            "27",
            "26",
            "25",
            "24",
            "23",
            "22",
            "21",
            "20",
            "19",
            "18",
            "17"});
			this.listBoxTeeth2.Location = new System.Drawing.Point(106, 115);
			this.listBoxTeeth2.MultiColumn = true;
			this.listBoxTeeth2.Name = "listBoxTeeth2";
			this.listBoxTeeth2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxTeeth2.Size = new System.Drawing.Size(275, 17);
			this.listBoxTeeth2.TabIndex = 2;
			this.listBoxTeeth2.Visible = false;
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(184, 61);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(74, 20);
			this.butChange.TabIndex = 37;
			this.butChange.Text = "C&hange";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// labelTimeFinal
			// 
			this.labelTimeFinal.Location = new System.Drawing.Point(259, 65);
			this.labelTimeFinal.Name = "labelTimeFinal";
			this.labelTimeFinal.Size = new System.Drawing.Size(56, 14);
			this.labelTimeFinal.TabIndex = 103;
			this.labelTimeFinal.Text = "Final";
			this.labelTimeFinal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTimeFinal.Visible = false;
			// 
			// textDrugQty
			// 
			this.textDrugQty.Location = new System.Drawing.Point(123, 148);
			this.textDrugQty.Name = "textDrugQty";
			this.textDrugQty.Size = new System.Drawing.Size(59, 20);
			this.textDrugQty.TabIndex = 174;
			// 
			// labelDrugQty
			// 
			this.labelDrugQty.Location = new System.Drawing.Point(6, 150);
			this.labelDrugQty.Name = "labelDrugQty";
			this.labelDrugQty.Size = new System.Drawing.Size(116, 17);
			this.labelDrugQty.TabIndex = 173;
			this.labelDrugQty.Text = "Drug Qty";
			this.labelDrugQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDrugNDC
			// 
			this.labelDrugNDC.Location = new System.Drawing.Point(6, 109);
			this.labelDrugNDC.Name = "labelDrugNDC";
			this.labelDrugNDC.Size = new System.Drawing.Size(116, 17);
			this.labelDrugNDC.TabIndex = 170;
			this.labelDrugNDC.Text = "Drug NDC";
			this.labelDrugNDC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDrugNDC
			// 
			this.textDrugNDC.Location = new System.Drawing.Point(123, 107);
			this.textDrugNDC.Name = "textDrugNDC";
			this.textDrugNDC.ReadOnly = true;
			this.textDrugNDC.Size = new System.Drawing.Size(109, 20);
			this.textDrugNDC.TabIndex = 171;
			// 
			// comboDrugUnit
			// 
			this.comboDrugUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDrugUnit.FormattingEnabled = true;
			this.comboDrugUnit.Location = new System.Drawing.Point(123, 127);
			this.comboDrugUnit.Name = "comboDrugUnit";
			this.comboDrugUnit.Size = new System.Drawing.Size(92, 21);
			this.comboDrugUnit.TabIndex = 169;
			// 
			// labelDrugUnit
			// 
			this.labelDrugUnit.Location = new System.Drawing.Point(6, 129);
			this.labelDrugUnit.Name = "labelDrugUnit";
			this.labelDrugUnit.Size = new System.Drawing.Size(116, 17);
			this.labelDrugUnit.TabIndex = 168;
			this.labelDrugUnit.Text = "Drug Unit";
			this.labelDrugUnit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRevCode
			// 
			this.textRevCode.Location = new System.Drawing.Point(123, 87);
			this.textRevCode.MaxLength = 48;
			this.textRevCode.Name = "textRevCode";
			this.textRevCode.Size = new System.Drawing.Size(59, 20);
			this.textRevCode.TabIndex = 112;
			// 
			// labelRevCode
			// 
			this.labelRevCode.Location = new System.Drawing.Point(6, 89);
			this.labelRevCode.Name = "labelRevCode";
			this.labelRevCode.Size = new System.Drawing.Size(116, 17);
			this.labelRevCode.TabIndex = 111;
			this.labelRevCode.Text = "Revenue Code";
			this.labelRevCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUnitQty
			// 
			this.textUnitQty.Location = new System.Drawing.Point(123, 46);
			this.textUnitQty.MaxLength = 15;
			this.textUnitQty.Name = "textUnitQty";
			this.textUnitQty.Size = new System.Drawing.Size(29, 20);
			this.textUnitQty.TabIndex = 110;
			// 
			// labelUnitQty
			// 
			this.labelUnitQty.Location = new System.Drawing.Point(6, 48);
			this.labelUnitQty.Name = "labelUnitQty";
			this.labelUnitQty.Size = new System.Drawing.Size(116, 17);
			this.labelUnitQty.TabIndex = 108;
			this.labelUnitQty.Text = "Unit Quantity";
			this.labelUnitQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeMod4
			// 
			this.textCodeMod4.Location = new System.Drawing.Point(210, 26);
			this.textCodeMod4.MaxLength = 2;
			this.textCodeMod4.Name = "textCodeMod4";
			this.textCodeMod4.Size = new System.Drawing.Size(29, 20);
			this.textCodeMod4.TabIndex = 106;
			this.textCodeMod4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCodeMod3
			// 
			this.textCodeMod3.Location = new System.Drawing.Point(181, 26);
			this.textCodeMod3.MaxLength = 2;
			this.textCodeMod3.Name = "textCodeMod3";
			this.textCodeMod3.Size = new System.Drawing.Size(29, 20);
			this.textCodeMod3.TabIndex = 105;
			this.textCodeMod3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCodeMod2
			// 
			this.textCodeMod2.Location = new System.Drawing.Point(152, 26);
			this.textCodeMod2.MaxLength = 2;
			this.textCodeMod2.Name = "textCodeMod2";
			this.textCodeMod2.Size = new System.Drawing.Size(29, 20);
			this.textCodeMod2.TabIndex = 104;
			this.textCodeMod2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelCodeMods
			// 
			this.labelCodeMods.Location = new System.Drawing.Point(6, 28);
			this.labelCodeMods.Name = "labelCodeMods";
			this.labelCodeMods.Size = new System.Drawing.Size(116, 17);
			this.labelCodeMods.TabIndex = 102;
			this.labelCodeMods.Text = "Mods";
			this.labelCodeMods.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeMod1
			// 
			this.textCodeMod1.Location = new System.Drawing.Point(123, 26);
			this.textCodeMod1.MaxLength = 2;
			this.textCodeMod1.Name = "textCodeMod1";
			this.textCodeMod1.Size = new System.Drawing.Size(29, 20);
			this.textCodeMod1.TabIndex = 103;
			this.textCodeMod1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// checkIsPrincDiag
			// 
			this.checkIsPrincDiag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPrincDiag.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsPrincDiag.Location = new System.Drawing.Point(336, 27);
			this.checkIsPrincDiag.Name = "checkIsPrincDiag";
			this.checkIsPrincDiag.Size = new System.Drawing.Size(178, 17);
			this.checkIsPrincDiag.TabIndex = 101;
			this.checkIsPrincDiag.Text = "Princ Diag";
			this.checkIsPrincDiag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiagnosisCode
			// 
			this.labelDiagnosisCode.Location = new System.Drawing.Point(336, 64);
			this.labelDiagnosisCode.Name = "labelDiagnosisCode";
			this.labelDiagnosisCode.Size = new System.Drawing.Size(164, 17);
			this.labelDiagnosisCode.TabIndex = 99;
			this.labelDiagnosisCode.Text = "ICD-10 Diagnosis Code 1";
			this.labelDiagnosisCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiagnosisCode
			// 
			this.textDiagnosisCode.Location = new System.Drawing.Point(501, 62);
			this.textDiagnosisCode.Name = "textDiagnosisCode";
			this.textDiagnosisCode.Size = new System.Drawing.Size(76, 20);
			this.textDiagnosisCode.TabIndex = 100;
			// 
			// labelMedicalCode
			// 
			this.labelMedicalCode.Location = new System.Drawing.Point(6, 8);
			this.labelMedicalCode.Name = "labelMedicalCode";
			this.labelMedicalCode.Size = new System.Drawing.Size(116, 17);
			this.labelMedicalCode.TabIndex = 97;
			this.labelMedicalCode.Text = "Medical Code";
			this.labelMedicalCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMedicalCode
			// 
			this.textMedicalCode.Location = new System.Drawing.Point(123, 6);
			this.textMedicalCode.Name = "textMedicalCode";
			this.textMedicalCode.Size = new System.Drawing.Size(76, 20);
			this.textMedicalCode.TabIndex = 98;
			// 
			// labelClaim
			// 
			this.labelClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelClaim.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelClaim.Location = new System.Drawing.Point(111, 652);
			this.labelClaim.Name = "labelClaim";
			this.labelClaim.Size = new System.Drawing.Size(480, 44);
			this.labelClaim.TabIndex = 50;
			this.labelClaim.Text = "This procedure is attached to a claim, so certain fields should not be edited.  Y" +
    "ou should reprint the claim if any significant changes are made.";
			this.labelClaim.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelClaim.Visible = false;
			// 
			// comboPlaceService
			// 
			this.comboPlaceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlaceService.Location = new System.Drawing.Point(119, 98);
			this.comboPlaceService.MaxDropDownItems = 30;
			this.comboPlaceService.Name = "comboPlaceService";
			this.comboPlaceService.Size = new System.Drawing.Size(177, 21);
			this.comboPlaceService.TabIndex = 6;
			// 
			// labelPlaceService
			// 
			this.labelPlaceService.Location = new System.Drawing.Point(4, 101);
			this.labelPlaceService.Name = "labelPlaceService";
			this.labelPlaceService.Size = new System.Drawing.Size(114, 16);
			this.labelPlaceService.TabIndex = 53;
			this.labelPlaceService.Text = "Place of Service";
			this.labelPlaceService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPriority
			// 
			this.labelPriority.Location = new System.Drawing.Point(32, 263);
			this.labelPriority.Name = "labelPriority";
			this.labelPriority.Size = new System.Drawing.Size(72, 16);
			this.labelPriority.TabIndex = 56;
			this.labelPriority.Text = "Priority";
			this.labelPriority.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSetComplete
			// 
			this.labelSetComplete.Location = new System.Drawing.Point(723, 9);
			this.labelSetComplete.Name = "labelSetComplete";
			this.labelSetComplete.Size = new System.Drawing.Size(157, 16);
			this.labelSetComplete.TabIndex = 58;
			this.labelSetComplete.Text = "changes date and adds note.";
			// 
			// checkNoBillIns
			// 
			this.checkNoBillIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoBillIns.Location = new System.Drawing.Point(142, 12);
			this.checkNoBillIns.Name = "checkNoBillIns";
			this.checkNoBillIns.Size = new System.Drawing.Size(152, 18);
			this.checkNoBillIns.TabIndex = 9;
			this.checkNoBillIns.Text = "Do Not Bill to Ins";
			this.checkNoBillIns.ThreeState = true;
			this.checkNoBillIns.Click += new System.EventHandler(this.checkNoBillIns_Click);
			// 
			// labelIncomplete
			// 
			this.labelIncomplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelIncomplete.ForeColor = System.Drawing.Color.DarkRed;
			this.labelIncomplete.Location = new System.Drawing.Point(547, 115);
			this.labelIncomplete.Name = "labelIncomplete";
			this.labelIncomplete.Size = new System.Drawing.Size(155, 18);
			this.labelIncomplete.TabIndex = 73;
			this.labelIncomplete.Text = "Incomplete";
			this.labelIncomplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(467, 28);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(214, 21);
			this.comboClinic.TabIndex = 74;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(403, 7);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(99, 16);
			this.label14.TabIndex = 77;
			this.label14.Text = "Procedure Status";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(389, 327);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(110, 41);
			this.label15.TabIndex = 79;
			this.label15.Text = "Signature /\r\nInitials";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(429, 138);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(73, 16);
			this.label16.TabIndex = 80;
			this.label16.Text = "User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPriority
			// 
			this.comboPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPriority.Location = new System.Drawing.Point(106, 259);
			this.comboPriority.MaxDropDownItems = 30;
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(177, 21);
			this.comboPriority.TabIndex = 98;
			// 
			// comboDx
			// 
			this.comboDx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDx.Location = new System.Drawing.Point(106, 238);
			this.comboDx.MaxDropDownItems = 30;
			this.comboDx.Name = "comboDx";
			this.comboDx.Size = new System.Drawing.Size(177, 21);
			this.comboDx.TabIndex = 99;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(504, 49);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(158, 21);
			this.comboProv.TabIndex = 100;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(504, 137);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(116, 20);
			this.textUser.TabIndex = 101;
			// 
			// comboBillingTypeTwo
			// 
			this.comboBillingTypeTwo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBillingTypeTwo.FormattingEnabled = true;
			this.comboBillingTypeTwo.Location = new System.Drawing.Point(119, 33);
			this.comboBillingTypeTwo.MaxDropDownItems = 30;
			this.comboBillingTypeTwo.Name = "comboBillingTypeTwo";
			this.comboBillingTypeTwo.Size = new System.Drawing.Size(198, 21);
			this.comboBillingTypeTwo.TabIndex = 102;
			// 
			// labelBillingTypeTwo
			// 
			this.labelBillingTypeTwo.Location = new System.Drawing.Point(15, 35);
			this.labelBillingTypeTwo.Name = "labelBillingTypeTwo";
			this.labelBillingTypeTwo.Size = new System.Drawing.Size(102, 16);
			this.labelBillingTypeTwo.TabIndex = 103;
			this.labelBillingTypeTwo.Text = "Billing Type 2";
			this.labelBillingTypeTwo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBillingTypeOne
			// 
			this.comboBillingTypeOne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBillingTypeOne.FormattingEnabled = true;
			this.comboBillingTypeOne.Location = new System.Drawing.Point(119, 12);
			this.comboBillingTypeOne.MaxDropDownItems = 30;
			this.comboBillingTypeOne.Name = "comboBillingTypeOne";
			this.comboBillingTypeOne.Size = new System.Drawing.Size(198, 21);
			this.comboBillingTypeOne.TabIndex = 104;
			// 
			// labelBillingTypeOne
			// 
			this.labelBillingTypeOne.Location = new System.Drawing.Point(13, 14);
			this.labelBillingTypeOne.Name = "labelBillingTypeOne";
			this.labelBillingTypeOne.Size = new System.Drawing.Size(104, 16);
			this.labelBillingTypeOne.TabIndex = 105;
			this.labelBillingTypeOne.Text = "Billing Type 1";
			this.labelBillingTypeOne.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSite
			// 
			this.textSite.AcceptsReturn = true;
			this.textSite.Location = new System.Drawing.Point(119, 77);
			this.textSite.Name = "textSite";
			this.textSite.ReadOnly = true;
			this.textSite.Size = new System.Drawing.Size(153, 20);
			this.textSite.TabIndex = 111;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(4, 78);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(114, 17);
			this.labelSite.TabIndex = 110;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHideGraphics
			// 
			this.checkHideGraphics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideGraphics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideGraphics.Location = new System.Drawing.Point(5, 217);
			this.checkHideGraphics.Name = "checkHideGraphics";
			this.checkHideGraphics.Size = new System.Drawing.Size(114, 18);
			this.checkHideGraphics.TabIndex = 162;
			this.checkHideGraphics.Text = "Hide Graphics";
			this.checkHideGraphics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 41);
			this.label3.TabIndex = 0;
			this.label3.Text = "Crown, Bridge, Denture, or RPD";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 16);
			this.label4.TabIndex = 4;
			this.label4.Text = "Original Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listProsth
			// 
			this.listProsth.Location = new System.Drawing.Point(91, 14);
			this.listProsth.Name = "listProsth";
			this.listProsth.Size = new System.Drawing.Size(163, 43);
			this.listProsth.TabIndex = 0;
			// 
			// groupProsth
			// 
			this.groupProsth.Controls.Add(this.checkIsDateProsthEst);
			this.groupProsth.Controls.Add(this.listProsth);
			this.groupProsth.Controls.Add(this.textDateOriginalProsth);
			this.groupProsth.Controls.Add(this.label4);
			this.groupProsth.Controls.Add(this.label3);
			this.groupProsth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupProsth.Location = new System.Drawing.Point(15, 283);
			this.groupProsth.Name = "groupProsth";
			this.groupProsth.Size = new System.Drawing.Size(269, 80);
			this.groupProsth.TabIndex = 7;
			this.groupProsth.TabStop = false;
			this.groupProsth.Text = "Prosthesis Replacement";
			// 
			// checkIsDateProsthEst
			// 
			this.checkIsDateProsthEst.Location = new System.Drawing.Point(169, 61);
			this.checkIsDateProsthEst.Name = "checkIsDateProsthEst";
			this.checkIsDateProsthEst.Size = new System.Drawing.Size(96, 16);
			this.checkIsDateProsthEst.TabIndex = 181;
			this.checkIsDateProsthEst.Text = "Is Estimated";
			this.checkIsDateProsthEst.UseVisualStyleBackColor = true;
			// 
			// textDateOriginalProsth
			// 
			this.textDateOriginalProsth.Location = new System.Drawing.Point(91, 58);
			this.textDateOriginalProsth.Name = "textDateOriginalProsth";
			this.textDateOriginalProsth.Size = new System.Drawing.Size(73, 20);
			this.textDateOriginalProsth.TabIndex = 1;
			// 
			// checkTypeCodeA
			// 
			this.checkTypeCodeA.Location = new System.Drawing.Point(10, 16);
			this.checkTypeCodeA.Name = "checkTypeCodeA";
			this.checkTypeCodeA.Size = new System.Drawing.Size(268, 17);
			this.checkTypeCodeA.TabIndex = 0;
			this.checkTypeCodeA.Text = "Not initial placement.  Repair of a prior service.";
			this.checkTypeCodeA.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeB
			// 
			this.checkTypeCodeB.Location = new System.Drawing.Point(10, 33);
			this.checkTypeCodeB.Name = "checkTypeCodeB";
			this.checkTypeCodeB.Size = new System.Drawing.Size(239, 17);
			this.checkTypeCodeB.TabIndex = 1;
			this.checkTypeCodeB.Text = "Temporary placement or service.";
			this.checkTypeCodeB.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeC
			// 
			this.checkTypeCodeC.Location = new System.Drawing.Point(10, 50);
			this.checkTypeCodeC.Name = "checkTypeCodeC";
			this.checkTypeCodeC.Size = new System.Drawing.Size(239, 17);
			this.checkTypeCodeC.TabIndex = 2;
			this.checkTypeCodeC.Text = "Correction of TMJ";
			this.checkTypeCodeC.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeE
			// 
			this.checkTypeCodeE.Location = new System.Drawing.Point(10, 67);
			this.checkTypeCodeE.Name = "checkTypeCodeE";
			this.checkTypeCodeE.Size = new System.Drawing.Size(268, 17);
			this.checkTypeCodeE.TabIndex = 3;
			this.checkTypeCodeE.Text = "Implant, or in conjunction with implants";
			this.checkTypeCodeE.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeL
			// 
			this.checkTypeCodeL.Location = new System.Drawing.Point(10, 84);
			this.checkTypeCodeL.Name = "checkTypeCodeL";
			this.checkTypeCodeL.Size = new System.Drawing.Size(113, 17);
			this.checkTypeCodeL.TabIndex = 4;
			this.checkTypeCodeL.Text = "Appliance lost";
			this.checkTypeCodeL.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeX
			// 
			this.checkTypeCodeX.Location = new System.Drawing.Point(10, 118);
			this.checkTypeCodeX.Name = "checkTypeCodeX";
			this.checkTypeCodeX.Size = new System.Drawing.Size(239, 17);
			this.checkTypeCodeX.TabIndex = 5;
			this.checkTypeCodeX.Text = "None of the above are applicable";
			this.checkTypeCodeX.UseVisualStyleBackColor = true;
			// 
			// checkTypeCodeS
			// 
			this.checkTypeCodeS.Location = new System.Drawing.Point(10, 101);
			this.checkTypeCodeS.Name = "checkTypeCodeS";
			this.checkTypeCodeS.Size = new System.Drawing.Size(113, 17);
			this.checkTypeCodeS.TabIndex = 6;
			this.checkTypeCodeS.Text = "Appliance stolen";
			this.checkTypeCodeS.UseVisualStyleBackColor = true;
			// 
			// groupCanadianProcTypeCode
			// 
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeS);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeX);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeL);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeE);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeC);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeB);
			this.groupCanadianProcTypeCode.Controls.Add(this.checkTypeCodeA);
			this.groupCanadianProcTypeCode.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupCanadianProcTypeCode.Location = new System.Drawing.Point(18, 16);
			this.groupCanadianProcTypeCode.Name = "groupCanadianProcTypeCode";
			this.groupCanadianProcTypeCode.Size = new System.Drawing.Size(316, 142);
			this.groupCanadianProcTypeCode.TabIndex = 163;
			this.groupCanadianProcTypeCode.TabStop = false;
			this.groupCanadianProcTypeCode.Text = "Procedure Type Code";
			// 
			// comboPrognosis
			// 
			this.comboPrognosis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPrognosis.Location = new System.Drawing.Point(119, 55);
			this.comboPrognosis.MaxDropDownItems = 30;
			this.comboPrognosis.Name = "comboPrognosis";
			this.comboPrognosis.Size = new System.Drawing.Size(153, 21);
			this.comboPrognosis.TabIndex = 165;
			// 
			// labelPrognosis
			// 
			this.labelPrognosis.Location = new System.Drawing.Point(3, 57);
			this.labelPrognosis.Name = "labelPrognosis";
			this.labelPrognosis.Size = new System.Drawing.Size(114, 17);
			this.labelPrognosis.TabIndex = 166;
			this.labelPrognosis.Text = "Prognosis";
			this.labelPrognosis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProcStatus
			// 
			this.comboProcStatus.Location = new System.Drawing.Point(504, 6);
			this.comboProcStatus.Name = "comboProcStatus";
			this.comboProcStatus.Size = new System.Drawing.Size(133, 21);
			this.comboProcStatus.TabIndex = 167;
			this.comboProcStatus.SelectionChangeCommitted += new System.EventHandler(this.comboProcStatus_SelectionChangeCommitted);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(418, 85);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(84, 16);
			this.label13.TabIndex = 168;
			this.label13.Text = "Referral";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textReferral
			// 
			this.textReferral.BackColor = System.Drawing.SystemColors.Control;
			this.textReferral.ForeColor = System.Drawing.Color.DarkRed;
			this.textReferral.Location = new System.Drawing.Point(504, 82);
			this.textReferral.Name = "textReferral";
			this.textReferral.ReadOnly = true;
			this.textReferral.Size = new System.Drawing.Size(198, 20);
			this.textReferral.TabIndex = 169;
			this.textReferral.Text = "test";
			// 
			// labelClaimNote
			// 
			this.labelClaimNote.Location = new System.Drawing.Point(0, 364);
			this.labelClaimNote.Name = "labelClaimNote";
			this.labelClaimNote.Size = new System.Drawing.Size(104, 41);
			this.labelClaimNote.TabIndex = 174;
			this.labelClaimNote.Text = "E-claim Note (keep it very short)";
			this.labelClaimNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageFinancial);
			this.tabControl.Controls.Add(this.tabPageMedical);
			this.tabControl.Controls.Add(this.tabPageMisc);
			this.tabControl.Controls.Add(this.tabPageCanada);
			this.tabControl.Location = new System.Drawing.Point(1, 424);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(962, 244);
			this.tabControl.TabIndex = 175;
			this.tabControl.SizeChanged += new System.EventHandler(this.tabControl_SizeChanged);
			// 
			// tabPageFinancial
			// 
			this.tabPageFinancial.Controls.Add(this.butAddExistAdj);
			this.tabPageFinancial.Controls.Add(this.gridPay);
			this.tabPageFinancial.Controls.Add(this.gridAdj);
			this.tabPageFinancial.Controls.Add(this.label20);
			this.tabPageFinancial.Controls.Add(this.textDiscount);
			this.tabPageFinancial.Controls.Add(this.butAddEstimate);
			this.tabPageFinancial.Controls.Add(this.checkNoBillIns);
			this.tabPageFinancial.Controls.Add(this.butAddAdjust);
			this.tabPageFinancial.Controls.Add(this.gridIns);
			this.tabPageFinancial.Location = new System.Drawing.Point(4, 22);
			this.tabPageFinancial.Name = "tabPageFinancial";
			this.tabPageFinancial.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFinancial.Size = new System.Drawing.Size(954, 218);
			this.tabPageFinancial.TabIndex = 0;
			this.tabPageFinancial.Text = "Financial";
			this.tabPageFinancial.UseVisualStyleBackColor = true;
			// 
			// butAddExistAdj
			// 
			this.butAddExistAdj.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddExistAdj.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddExistAdj.Location = new System.Drawing.Point(589, 6);
			this.butAddExistAdj.Name = "butAddExistAdj";
			this.butAddExistAdj.Size = new System.Drawing.Size(126, 24);
			this.butAddExistAdj.TabIndex = 118;
			this.butAddExistAdj.Text = "Link Existing Adj";
			this.butAddExistAdj.Click += new System.EventHandler(this.butAddExistAdj_Click);
			// 
			// gridPay
			// 
			this.gridPay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridPay.Location = new System.Drawing.Point(3, 137);
			this.gridPay.Name = "gridPay";
			this.gridPay.Size = new System.Drawing.Size(449, 76);
			this.gridPay.TabIndex = 117;
			this.gridPay.Title = "Patient Payments";
			this.gridPay.TranslationName = "TableProcPay";
			this.gridPay.WrapText = false;
			this.gridPay.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPay_CellDoubleClick);
			// 
			// gridAdj
			// 
			this.gridAdj.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAdj.Location = new System.Drawing.Point(458, 137);
			this.gridAdj.Name = "gridAdj";
			this.gridAdj.Size = new System.Drawing.Size(494, 76);
			this.gridAdj.TabIndex = 116;
			this.gridAdj.Title = "Adjustments";
			this.gridAdj.TranslationName = "TableProcAdj";
			this.gridAdj.WrapText = false;
			this.gridAdj.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAdj_CellDoubleClick);
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label20.Location = new System.Drawing.Point(807, 12);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(75, 16);
			this.label20.TabIndex = 114;
			this.label20.Text = "Discount";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDiscount
			// 
			this.textDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscount.Location = new System.Drawing.Point(883, 9);
			this.textDiscount.MaxVal = 100000000D;
			this.textDiscount.MinVal = -100000000D;
			this.textDiscount.Name = "textDiscount";
			this.textDiscount.Size = new System.Drawing.Size(68, 20);
			this.textDiscount.TabIndex = 115;
			// 
			// butAddEstimate
			// 
			this.butAddEstimate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddEstimate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddEstimate.Location = new System.Drawing.Point(3, 6);
			this.butAddEstimate.Name = "butAddEstimate";
			this.butAddEstimate.Size = new System.Drawing.Size(111, 24);
			this.butAddEstimate.TabIndex = 60;
			this.butAddEstimate.Text = "Add Estimate";
			this.butAddEstimate.Click += new System.EventHandler(this.butAddEstimate_Click);
			// 
			// butAddAdjust
			// 
			this.butAddAdjust.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAdjust.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAdjust.Location = new System.Drawing.Point(458, 6);
			this.butAddAdjust.Name = "butAddAdjust";
			this.butAddAdjust.Size = new System.Drawing.Size(126, 24);
			this.butAddAdjust.TabIndex = 72;
			this.butAddAdjust.Text = "Add New Adj";
			this.butAddAdjust.Click += new System.EventHandler(this.butAddAdjust_Click);
			// 
			// gridIns
			// 
			this.gridIns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridIns.Location = new System.Drawing.Point(3, 32);
			this.gridIns.Name = "gridIns";
			this.gridIns.Size = new System.Drawing.Size(949, 102);
			this.gridIns.TabIndex = 113;
			this.gridIns.Title = "Insurance Estimates and Payments";
			this.gridIns.TranslationName = "TableProcIns";
			this.gridIns.WrapText = false;
			this.gridIns.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridIns_CellDoubleClick);
			// 
			// tabPageMedical
			// 
			this.tabPageMedical.Controls.Add(this.checkIsEmergency);
			this.tabPageMedical.Controls.Add(this.groupBox1);
			this.tabPageMedical.Controls.Add(this.labelIcdVersionUncheck);
			this.tabPageMedical.Controls.Add(this.checkIcdVersion);
			this.tabPageMedical.Controls.Add(this.butNoneDiagnosisCode);
			this.tabPageMedical.Controls.Add(this.butDiagnosisCode);
			this.tabPageMedical.Controls.Add(this.butNoneDiagnosisCode2);
			this.tabPageMedical.Controls.Add(this.butDiagnosisCode2);
			this.tabPageMedical.Controls.Add(this.butNoneDiagnosisCode4);
			this.tabPageMedical.Controls.Add(this.butDiagnosisCode4);
			this.tabPageMedical.Controls.Add(this.butNoneDiagnosisCode3);
			this.tabPageMedical.Controls.Add(this.butDiagnosisCode3);
			this.tabPageMedical.Controls.Add(this.textDiagnosisCode2);
			this.tabPageMedical.Controls.Add(this.labelDiagnosisCode2);
			this.tabPageMedical.Controls.Add(this.textDiagnosisCode3);
			this.tabPageMedical.Controls.Add(this.labelDiagnosisCode3);
			this.tabPageMedical.Controls.Add(this.textDiagnosisCode4);
			this.tabPageMedical.Controls.Add(this.labelDiagnosisCode4);
			this.tabPageMedical.Controls.Add(this.butNoneSnomedBodySite);
			this.tabPageMedical.Controls.Add(this.butSnomedBodySiteSelect);
			this.tabPageMedical.Controls.Add(this.labelSnomedBodySite);
			this.tabPageMedical.Controls.Add(this.textSnomedBodySite);
			this.tabPageMedical.Controls.Add(this.labelUnitType);
			this.tabPageMedical.Controls.Add(this.comboUnitType);
			this.tabPageMedical.Controls.Add(this.textDrugQty);
			this.tabPageMedical.Controls.Add(this.labelDrugQty);
			this.tabPageMedical.Controls.Add(this.labelMedicalCode);
			this.tabPageMedical.Controls.Add(this.labelDrugNDC);
			this.tabPageMedical.Controls.Add(this.textMedicalCode);
			this.tabPageMedical.Controls.Add(this.textDrugNDC);
			this.tabPageMedical.Controls.Add(this.textDiagnosisCode);
			this.tabPageMedical.Controls.Add(this.comboDrugUnit);
			this.tabPageMedical.Controls.Add(this.labelDiagnosisCode);
			this.tabPageMedical.Controls.Add(this.labelDrugUnit);
			this.tabPageMedical.Controls.Add(this.checkIsPrincDiag);
			this.tabPageMedical.Controls.Add(this.textRevCode);
			this.tabPageMedical.Controls.Add(this.textCodeMod1);
			this.tabPageMedical.Controls.Add(this.labelRevCode);
			this.tabPageMedical.Controls.Add(this.labelCodeMods);
			this.tabPageMedical.Controls.Add(this.textUnitQty);
			this.tabPageMedical.Controls.Add(this.textCodeMod2);
			this.tabPageMedical.Controls.Add(this.labelUnitQty);
			this.tabPageMedical.Controls.Add(this.textCodeMod3);
			this.tabPageMedical.Controls.Add(this.textCodeMod4);
			this.tabPageMedical.Location = new System.Drawing.Point(4, 22);
			this.tabPageMedical.Name = "tabPageMedical";
			this.tabPageMedical.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMedical.Size = new System.Drawing.Size(954, 218);
			this.tabPageMedical.TabIndex = 3;
			this.tabPageMedical.Text = "Medical";
			this.tabPageMedical.UseVisualStyleBackColor = true;
			// 
			// checkIsEmergency
			// 
			this.checkIsEmergency.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEmergency.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsEmergency.Location = new System.Drawing.Point(6, 169);
			this.checkIsEmergency.Name = "checkIsEmergency";
			this.checkIsEmergency.Size = new System.Drawing.Size(130, 17);
			this.checkIsEmergency.TabIndex = 301;
			this.checkIsEmergency.Text = "Is Emergency";
			this.checkIsEmergency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butPickOrderProvInternal);
			this.groupBox1.Controls.Add(this.textOrderingProviderOverride);
			this.groupBox1.Controls.Add(this.butPickOrderProvReferral);
			this.groupBox1.Controls.Add(this.butNoneOrderProv);
			this.groupBox1.Location = new System.Drawing.Point(495, 146);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(278, 66);
			this.groupBox1.TabIndex = 300;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Ordering Provider Override";
			// 
			// butPickOrderProvInternal
			// 
			this.butPickOrderProvInternal.Location = new System.Drawing.Point(6, 16);
			this.butPickOrderProvInternal.Name = "butPickOrderProvInternal";
			this.butPickOrderProvInternal.Size = new System.Drawing.Size(58, 20);
			this.butPickOrderProvInternal.TabIndex = 294;
			this.butPickOrderProvInternal.Text = "Internal";
			this.butPickOrderProvInternal.Click += new System.EventHandler(this.butPickOrderProvInternal_Click);
			// 
			// textOrderingProviderOverride
			// 
			this.textOrderingProviderOverride.AcceptsTab = true;
			this.textOrderingProviderOverride.BackColor = System.Drawing.SystemColors.Control;
			this.textOrderingProviderOverride.DetectLinksEnabled = false;
			this.textOrderingProviderOverride.DetectUrls = false;
			this.textOrderingProviderOverride.Location = new System.Drawing.Point(6, 39);
			this.textOrderingProviderOverride.Name = "textOrderingProviderOverride";
			this.textOrderingProviderOverride.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textOrderingProviderOverride.ReadOnly = true;
			this.textOrderingProviderOverride.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textOrderingProviderOverride.Size = new System.Drawing.Size(266, 21);
			this.textOrderingProviderOverride.SpellCheckIsEnabled = false;
			this.textOrderingProviderOverride.TabIndex = 299;
			this.textOrderingProviderOverride.Text = "";
			// 
			// butPickOrderProvReferral
			// 
			this.butPickOrderProvReferral.Location = new System.Drawing.Point(65, 16);
			this.butPickOrderProvReferral.Name = "butPickOrderProvReferral";
			this.butPickOrderProvReferral.Size = new System.Drawing.Size(58, 20);
			this.butPickOrderProvReferral.TabIndex = 297;
			this.butPickOrderProvReferral.Text = "Referral";
			this.butPickOrderProvReferral.Click += new System.EventHandler(this.butPickOrderProvReferral_Click);
			// 
			// butNoneOrderProv
			// 
			this.butNoneOrderProv.Location = new System.Drawing.Point(124, 16);
			this.butNoneOrderProv.Name = "butNoneOrderProv";
			this.butNoneOrderProv.Size = new System.Drawing.Size(58, 20);
			this.butNoneOrderProv.TabIndex = 298;
			this.butNoneOrderProv.Text = "None";
			this.butNoneOrderProv.Click += new System.EventHandler(this.butNoneOrderProv_Click);
			// 
			// labelIcdVersionUncheck
			// 
			this.labelIcdVersionUncheck.Location = new System.Drawing.Point(515, 43);
			this.labelIcdVersionUncheck.Name = "labelIcdVersionUncheck";
			this.labelIcdVersionUncheck.Size = new System.Drawing.Size(115, 17);
			this.labelIcdVersionUncheck.TabIndex = 288;
			this.labelIcdVersionUncheck.Text = "(uncheck for ICD-9)";
			this.labelIcdVersionUncheck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIcdVersion
			// 
			this.checkIcdVersion.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIcdVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIcdVersion.Location = new System.Drawing.Point(336, 43);
			this.checkIcdVersion.Name = "checkIcdVersion";
			this.checkIcdVersion.Size = new System.Drawing.Size(178, 17);
			this.checkIcdVersion.TabIndex = 287;
			this.checkIcdVersion.Text = "Use ICD-10 Diagnosis Codes";
			this.checkIcdVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIcdVersion.Click += new System.EventHandler(this.checkIcdVersion_Click);
			// 
			// butNoneDiagnosisCode
			// 
			this.butNoneDiagnosisCode.Location = new System.Drawing.Point(599, 62);
			this.butNoneDiagnosisCode.Name = "butNoneDiagnosisCode";
			this.butNoneDiagnosisCode.Size = new System.Drawing.Size(50, 20);
			this.butNoneDiagnosisCode.TabIndex = 194;
			this.butNoneDiagnosisCode.Text = "None";
			this.butNoneDiagnosisCode.Click += new System.EventHandler(this.butNoneDiagnosisCode1_Click);
			// 
			// butDiagnosisCode
			// 
			this.butDiagnosisCode.Location = new System.Drawing.Point(578, 62);
			this.butDiagnosisCode.Name = "butDiagnosisCode";
			this.butDiagnosisCode.Size = new System.Drawing.Size(20, 20);
			this.butDiagnosisCode.TabIndex = 193;
			this.butDiagnosisCode.Text = "...";
			this.butDiagnosisCode.Click += new System.EventHandler(this.butDiagnosisCode1_Click);
			// 
			// butNoneDiagnosisCode2
			// 
			this.butNoneDiagnosisCode2.Location = new System.Drawing.Point(599, 82);
			this.butNoneDiagnosisCode2.Name = "butNoneDiagnosisCode2";
			this.butNoneDiagnosisCode2.Size = new System.Drawing.Size(50, 20);
			this.butNoneDiagnosisCode2.TabIndex = 192;
			this.butNoneDiagnosisCode2.Text = "None";
			this.butNoneDiagnosisCode2.Click += new System.EventHandler(this.butNoneDiagnosisCode2_Click);
			// 
			// butDiagnosisCode2
			// 
			this.butDiagnosisCode2.Location = new System.Drawing.Point(578, 82);
			this.butDiagnosisCode2.Name = "butDiagnosisCode2";
			this.butDiagnosisCode2.Size = new System.Drawing.Size(20, 20);
			this.butDiagnosisCode2.TabIndex = 191;
			this.butDiagnosisCode2.Text = "...";
			this.butDiagnosisCode2.Click += new System.EventHandler(this.butDiagnosisCode2_Click);
			// 
			// butNoneDiagnosisCode4
			// 
			this.butNoneDiagnosisCode4.Location = new System.Drawing.Point(599, 122);
			this.butNoneDiagnosisCode4.Name = "butNoneDiagnosisCode4";
			this.butNoneDiagnosisCode4.Size = new System.Drawing.Size(50, 20);
			this.butNoneDiagnosisCode4.TabIndex = 190;
			this.butNoneDiagnosisCode4.Text = "None";
			this.butNoneDiagnosisCode4.Click += new System.EventHandler(this.butNoneDiagnosisCode4_Click);
			// 
			// butDiagnosisCode4
			// 
			this.butDiagnosisCode4.Location = new System.Drawing.Point(578, 122);
			this.butDiagnosisCode4.Name = "butDiagnosisCode4";
			this.butDiagnosisCode4.Size = new System.Drawing.Size(20, 20);
			this.butDiagnosisCode4.TabIndex = 189;
			this.butDiagnosisCode4.Text = "...";
			this.butDiagnosisCode4.Click += new System.EventHandler(this.butDiagnosisCode4_Click);
			// 
			// butNoneDiagnosisCode3
			// 
			this.butNoneDiagnosisCode3.Location = new System.Drawing.Point(599, 102);
			this.butNoneDiagnosisCode3.Name = "butNoneDiagnosisCode3";
			this.butNoneDiagnosisCode3.Size = new System.Drawing.Size(50, 20);
			this.butNoneDiagnosisCode3.TabIndex = 188;
			this.butNoneDiagnosisCode3.Text = "None";
			this.butNoneDiagnosisCode3.Click += new System.EventHandler(this.butNoneDiagnosisCode3_Click);
			// 
			// butDiagnosisCode3
			// 
			this.butDiagnosisCode3.Location = new System.Drawing.Point(578, 102);
			this.butDiagnosisCode3.Name = "butDiagnosisCode3";
			this.butDiagnosisCode3.Size = new System.Drawing.Size(20, 20);
			this.butDiagnosisCode3.TabIndex = 187;
			this.butDiagnosisCode3.Text = "...";
			this.butDiagnosisCode3.Click += new System.EventHandler(this.butDiagnosisCode3_Click);
			// 
			// textDiagnosisCode2
			// 
			this.textDiagnosisCode2.Location = new System.Drawing.Point(501, 82);
			this.textDiagnosisCode2.Name = "textDiagnosisCode2";
			this.textDiagnosisCode2.Size = new System.Drawing.Size(76, 20);
			this.textDiagnosisCode2.TabIndex = 186;
			// 
			// labelDiagnosisCode2
			// 
			this.labelDiagnosisCode2.Location = new System.Drawing.Point(336, 84);
			this.labelDiagnosisCode2.Name = "labelDiagnosisCode2";
			this.labelDiagnosisCode2.Size = new System.Drawing.Size(164, 17);
			this.labelDiagnosisCode2.TabIndex = 185;
			this.labelDiagnosisCode2.Text = "ICD-10 Diagnosis Code 2";
			this.labelDiagnosisCode2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiagnosisCode3
			// 
			this.textDiagnosisCode3.Location = new System.Drawing.Point(501, 102);
			this.textDiagnosisCode3.Name = "textDiagnosisCode3";
			this.textDiagnosisCode3.Size = new System.Drawing.Size(76, 20);
			this.textDiagnosisCode3.TabIndex = 184;
			// 
			// labelDiagnosisCode3
			// 
			this.labelDiagnosisCode3.Location = new System.Drawing.Point(336, 104);
			this.labelDiagnosisCode3.Name = "labelDiagnosisCode3";
			this.labelDiagnosisCode3.Size = new System.Drawing.Size(164, 17);
			this.labelDiagnosisCode3.TabIndex = 183;
			this.labelDiagnosisCode3.Text = "ICD-10 Diagnosis Code 3";
			this.labelDiagnosisCode3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiagnosisCode4
			// 
			this.textDiagnosisCode4.Location = new System.Drawing.Point(501, 122);
			this.textDiagnosisCode4.Name = "textDiagnosisCode4";
			this.textDiagnosisCode4.Size = new System.Drawing.Size(76, 20);
			this.textDiagnosisCode4.TabIndex = 182;
			// 
			// labelDiagnosisCode4
			// 
			this.labelDiagnosisCode4.Location = new System.Drawing.Point(336, 124);
			this.labelDiagnosisCode4.Name = "labelDiagnosisCode4";
			this.labelDiagnosisCode4.Size = new System.Drawing.Size(164, 17);
			this.labelDiagnosisCode4.TabIndex = 181;
			this.labelDiagnosisCode4.Text = "ICD-10 Diagnosis Code 4";
			this.labelDiagnosisCode4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNoneSnomedBodySite
			// 
			this.butNoneSnomedBodySite.Location = new System.Drawing.Point(795, 6);
			this.butNoneSnomedBodySite.Name = "butNoneSnomedBodySite";
			this.butNoneSnomedBodySite.Size = new System.Drawing.Size(50, 20);
			this.butNoneSnomedBodySite.TabIndex = 180;
			this.butNoneSnomedBodySite.Text = "None";
			this.butNoneSnomedBodySite.Click += new System.EventHandler(this.butNoneSnomedBodySite_Click);
			// 
			// butSnomedBodySiteSelect
			// 
			this.butSnomedBodySiteSelect.Location = new System.Drawing.Point(774, 6);
			this.butSnomedBodySiteSelect.Name = "butSnomedBodySiteSelect";
			this.butSnomedBodySiteSelect.Size = new System.Drawing.Size(20, 20);
			this.butSnomedBodySiteSelect.TabIndex = 179;
			this.butSnomedBodySiteSelect.Text = "...";
			this.butSnomedBodySiteSelect.Click += new System.EventHandler(this.butSnomedBodySiteSelect_Click);
			// 
			// labelSnomedBodySite
			// 
			this.labelSnomedBodySite.Location = new System.Drawing.Point(336, 8);
			this.labelSnomedBodySite.Name = "labelSnomedBodySite";
			this.labelSnomedBodySite.Size = new System.Drawing.Size(164, 17);
			this.labelSnomedBodySite.TabIndex = 178;
			this.labelSnomedBodySite.Text = "SNOMED CT Body Site";
			this.labelSnomedBodySite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSnomedBodySite
			// 
			this.textSnomedBodySite.Location = new System.Drawing.Point(501, 6);
			this.textSnomedBodySite.Name = "textSnomedBodySite";
			this.textSnomedBodySite.ReadOnly = true;
			this.textSnomedBodySite.Size = new System.Drawing.Size(272, 20);
			this.textSnomedBodySite.TabIndex = 177;
			// 
			// labelUnitType
			// 
			this.labelUnitType.Location = new System.Drawing.Point(6, 68);
			this.labelUnitType.Name = "labelUnitType";
			this.labelUnitType.Size = new System.Drawing.Size(116, 17);
			this.labelUnitType.TabIndex = 176;
			this.labelUnitType.Text = "Unit Type";
			this.labelUnitType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUnitType
			// 
			this.comboUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnitType.FormattingEnabled = true;
			this.comboUnitType.Location = new System.Drawing.Point(123, 66);
			this.comboUnitType.Name = "comboUnitType";
			this.comboUnitType.Size = new System.Drawing.Size(117, 21);
			this.comboUnitType.TabIndex = 175;
			// 
			// tabPageMisc
			// 
			this.tabPageMisc.Controls.Add(this.textBillingNote);
			this.tabPageMisc.Controls.Add(this.label18);
			this.tabPageMisc.Controls.Add(this.comboBillingTypeOne);
			this.tabPageMisc.Controls.Add(this.labelBillingTypeTwo);
			this.tabPageMisc.Controls.Add(this.comboBillingTypeTwo);
			this.tabPageMisc.Controls.Add(this.labelBillingTypeOne);
			this.tabPageMisc.Controls.Add(this.comboPrognosis);
			this.tabPageMisc.Controls.Add(this.labelPrognosis);
			this.tabPageMisc.Controls.Add(this.textSite);
			this.tabPageMisc.Controls.Add(this.labelSite);
			this.tabPageMisc.Controls.Add(this.butPickSite);
			this.tabPageMisc.Controls.Add(this.comboPlaceService);
			this.tabPageMisc.Controls.Add(this.labelPlaceService);
			this.tabPageMisc.Location = new System.Drawing.Point(4, 22);
			this.tabPageMisc.Name = "tabPageMisc";
			this.tabPageMisc.Size = new System.Drawing.Size(954, 218);
			this.tabPageMisc.TabIndex = 4;
			this.tabPageMisc.Text = "Misc";
			this.tabPageMisc.UseVisualStyleBackColor = true;
			// 
			// textBillingNote
			// 
			this.textBillingNote.Location = new System.Drawing.Point(119, 120);
			this.textBillingNote.Multiline = true;
			this.textBillingNote.Name = "textBillingNote";
			this.textBillingNote.Size = new System.Drawing.Size(259, 83);
			this.textBillingNote.TabIndex = 168;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(6, 122);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(111, 14);
			this.label18.TabIndex = 167;
			this.label18.Text = "Billing Note";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickSite
			// 
			this.butPickSite.Location = new System.Drawing.Point(273, 76);
			this.butPickSite.Name = "butPickSite";
			this.butPickSite.Size = new System.Drawing.Size(19, 21);
			this.butPickSite.TabIndex = 112;
			this.butPickSite.TabStop = false;
			this.butPickSite.Text = "...";
			this.butPickSite.Click += new System.EventHandler(this.butPickSite_Click);
			// 
			// tabPageCanada
			// 
			this.tabPageCanada.Controls.Add(this.labelCanadaLabFee2);
			this.tabPageCanada.Controls.Add(this.labelCanadaLabFee1);
			this.tabPageCanada.Controls.Add(this.groupCanadianProcTypeCode);
			this.tabPageCanada.Controls.Add(this.textCanadaLabFee2);
			this.tabPageCanada.Controls.Add(this.textCanadaLabFee1);
			this.tabPageCanada.Location = new System.Drawing.Point(4, 22);
			this.tabPageCanada.Name = "tabPageCanada";
			this.tabPageCanada.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCanada.Size = new System.Drawing.Size(954, 218);
			this.tabPageCanada.TabIndex = 1;
			this.tabPageCanada.Text = "Canada";
			this.tabPageCanada.UseVisualStyleBackColor = true;
			// 
			// labelCanadaLabFee2
			// 
			this.labelCanadaLabFee2.Location = new System.Drawing.Point(340, 37);
			this.labelCanadaLabFee2.Name = "labelCanadaLabFee2";
			this.labelCanadaLabFee2.Size = new System.Drawing.Size(75, 20);
			this.labelCanadaLabFee2.TabIndex = 167;
			this.labelCanadaLabFee2.Text = "Lab Fee 2";
			this.labelCanadaLabFee2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCanadaLabFee1
			// 
			this.labelCanadaLabFee1.Location = new System.Drawing.Point(340, 16);
			this.labelCanadaLabFee1.Name = "labelCanadaLabFee1";
			this.labelCanadaLabFee1.Size = new System.Drawing.Size(75, 20);
			this.labelCanadaLabFee1.TabIndex = 166;
			this.labelCanadaLabFee1.Text = "Lab Fee 1";
			this.labelCanadaLabFee1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCanadaLabFee2
			// 
			this.textCanadaLabFee2.Location = new System.Drawing.Point(421, 37);
			this.textCanadaLabFee2.MaxVal = 100000000D;
			this.textCanadaLabFee2.MinVal = -100000000D;
			this.textCanadaLabFee2.Name = "textCanadaLabFee2";
			this.textCanadaLabFee2.Size = new System.Drawing.Size(68, 20);
			this.textCanadaLabFee2.TabIndex = 165;
			// 
			// textCanadaLabFee1
			// 
			this.textCanadaLabFee1.Location = new System.Drawing.Point(421, 16);
			this.textCanadaLabFee1.MaxVal = 100000000D;
			this.textCanadaLabFee1.MinVal = -100000000D;
			this.textCanadaLabFee1.Name = "textCanadaLabFee1";
			this.textCanadaLabFee1.Size = new System.Drawing.Size(68, 20);
			this.textCanadaLabFee1.TabIndex = 164;
			// 
			// labelLocked
			// 
			this.labelLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelLocked.ForeColor = System.Drawing.Color.DarkRed;
			this.labelLocked.Location = new System.Drawing.Point(834, 115);
			this.labelLocked.Name = "labelLocked";
			this.labelLocked.Size = new System.Drawing.Size(123, 18);
			this.labelLocked.TabIndex = 176;
			this.labelLocked.Text = "Locked";
			this.labelLocked.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.labelLocked.Visible = false;
			// 
			// butSearch
			// 
			this.butSearch.Location = new System.Drawing.Point(443, 232);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(59, 24);
			this.butSearch.TabIndex = 180;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// butLock
			// 
			this.butLock.Location = new System.Drawing.Point(874, 91);
			this.butLock.Name = "butLock";
			this.butLock.Size = new System.Drawing.Size(80, 22);
			this.butLock.TabIndex = 178;
			this.butLock.Text = "Lock";
			this.butLock.Click += new System.EventHandler(this.butLock_Click);
			// 
			// butInvalidate
			// 
			this.butInvalidate.Location = new System.Drawing.Point(879, 77);
			this.butInvalidate.Name = "butInvalidate";
			this.butInvalidate.Size = new System.Drawing.Size(80, 22);
			this.butInvalidate.TabIndex = 179;
			this.butInvalidate.Text = "Invalidate";
			this.butInvalidate.Visible = false;
			this.butInvalidate.Click += new System.EventHandler(this.butInvalidate_Click);
			// 
			// butAppend
			// 
			this.butAppend.Location = new System.Drawing.Point(874, 136);
			this.butAppend.Name = "butAppend";
			this.butAppend.Size = new System.Drawing.Size(80, 22);
			this.butAppend.TabIndex = 177;
			this.butAppend.Text = "Append";
			this.butAppend.Visible = false;
			this.butAppend.Click += new System.EventHandler(this.butAppend_Click);
			// 
			// textClaimNote
			// 
			this.textClaimNote.AcceptsTab = true;
			this.textClaimNote.BackColor = System.Drawing.SystemColors.Window;
			this.textClaimNote.DetectLinksEnabled = false;
			this.textClaimNote.DetectUrls = false;
			this.textClaimNote.Location = new System.Drawing.Point(106, 364);
			this.textClaimNote.MaxLength = 80;
			this.textClaimNote.Name = "textClaimNote";
			this.textClaimNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textClaimNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textClaimNote.Size = new System.Drawing.Size(277, 43);
			this.textClaimNote.TabIndex = 173;
			this.textClaimNote.Text = "";
			// 
			// butReferral
			// 
			this.butReferral.Location = new System.Drawing.Point(707, 82);
			this.butReferral.Name = "butReferral";
			this.butReferral.Size = new System.Drawing.Size(18, 21);
			this.butReferral.TabIndex = 170;
			this.butReferral.Text = "...";
			this.butReferral.Click += new System.EventHandler(this.butReferral_Click);
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(663, 49);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(18, 21);
			this.butPickProv.TabIndex = 161;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// buttonUseAutoNote
			// 
			this.buttonUseAutoNote.Location = new System.Drawing.Point(663, 135);
			this.buttonUseAutoNote.Name = "buttonUseAutoNote";
			this.buttonUseAutoNote.Size = new System.Drawing.Size(80, 22);
			this.buttonUseAutoNote.TabIndex = 106;
			this.buttonUseAutoNote.Text = "Auto Note";
			this.buttonUseAutoNote.Click += new System.EventHandler(this.buttonUseAutoNote_Click);
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.HasAutoNotes = true;
			this.textNotes.Location = new System.Drawing.Point(504, 157);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(450, 164);
			this.textNotes.TabIndex = 1;
			this.textNotes.Text = "";
			this.textNotes.TextChanged += new System.EventHandler(this.textNotes_TextChanged);
			// 
			// butSetComplete
			// 
			this.butSetComplete.Location = new System.Drawing.Point(642, 5);
			this.butSetComplete.Name = "butSetComplete";
			this.butSetComplete.Size = new System.Drawing.Size(79, 22);
			this.butSetComplete.TabIndex = 54;
			this.butSetComplete.Text = "Set Complete";
			this.butSetComplete.Click += new System.EventHandler(this.butSetComplete_Click);
			// 
			// butEditAnyway
			// 
			this.butEditAnyway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditAnyway.Location = new System.Drawing.Point(594, 671);
			this.butEditAnyway.Name = "butEditAnyway";
			this.butEditAnyway.Size = new System.Drawing.Size(104, 24);
			this.butEditAnyway.TabIndex = 51;
			this.butEditAnyway.Text = "&Edit Anyway";
			this.butEditAnyway.Visible = false;
			this.butEditAnyway.Click += new System.EventHandler(this.butEditAnyway_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(2, 671);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(870, 671);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(779, 671);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(505, 325);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(364, 81);
			this.signatureBoxWrapper.TabIndex = 181;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.SignatureChanged += new System.EventHandler(this.signatureBoxWrapper_SignatureChanged);
			// 
			// butChangeUser
			// 
			this.butChangeUser.Location = new System.Drawing.Point(622, 135);
			this.butChangeUser.Name = "butChangeUser";
			this.butChangeUser.Size = new System.Drawing.Size(23, 22);
			this.butChangeUser.TabIndex = 182;
			this.butChangeUser.Text = "...";
			this.butChangeUser.Click += new System.EventHandler(this.butChangeUser_Click);
			// 
			// labelPermAlert
			// 
			this.labelPermAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPermAlert.ForeColor = System.Drawing.Color.DarkRed;
			this.labelPermAlert.Location = new System.Drawing.Point(504, 409);
			this.labelPermAlert.Name = "labelPermAlert";
			this.labelPermAlert.Size = new System.Drawing.Size(450, 27);
			this.labelPermAlert.TabIndex = 210;
			this.labelPermAlert.Text = "Notes and Signature locked.  \r\nNeed either ProcedureNoteFull or ProcedureNoteUser" +
    " to edit.";
			this.labelPermAlert.Visible = false;
			// 
			// butEditAutoNote
			// 
			this.butEditAutoNote.Location = new System.Drawing.Point(749, 135);
			this.butEditAutoNote.Name = "butEditAutoNote";
			this.butEditAutoNote.Size = new System.Drawing.Size(93, 22);
			this.butEditAutoNote.TabIndex = 211;
			this.butEditAutoNote.Text = "Edit Auto Note";
			this.butEditAutoNote.Click += new System.EventHandler(this.butEditAutoNote_Click);
			// 
			// FormProcEdit
			// 
			this.ClientSize = new System.Drawing.Size(962, 696);
			this.Controls.Add(this.butEditAutoNote);
			this.Controls.Add(this.labelPermAlert);
			this.Controls.Add(this.butChangeUser);
			this.Controls.Add(this.signatureBoxWrapper);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.butLock);
			this.Controls.Add(this.butInvalidate);
			this.Controls.Add(this.butAppend);
			this.Controls.Add(this.labelLocked);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.labelClaimNote);
			this.Controls.Add(this.textClaimNote);
			this.Controls.Add(this.butReferral);
			this.Controls.Add(this.textReferral);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.comboProcStatus);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.checkHideGraphics);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.buttonUseAutoNote);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.comboDx);
			this.Controls.Add(this.comboPriority);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelPriority);
			this.Controls.Add(this.labelDx);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.groupProsth);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.labelIncomplete);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.butSetComplete);
			this.Controls.Add(this.butEditAnyway);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelSetComplete);
			this.Controls.Add(this.labelClaim);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcEdit";
			this.ShowInTaskbar = false;
			this.Text = "Procedure Info";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProcEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormProcInfo_Load);
			this.Shown += new System.EventHandler(this.FormProcEdit_Shown);
			this.groupQuadrant.ResumeLayout(false);
			this.groupArch.ResumeLayout(false);
			this.panelSurfaces.ResumeLayout(false);
			this.groupSextant.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupProsth.ResumeLayout(false);
			this.groupProsth.PerformLayout();
			this.groupCanadianProcTypeCode.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPageFinancial.ResumeLayout(false);
			this.tabPageFinancial.PerformLayout();
			this.tabPageMedical.ResumeLayout(false);
			this.tabPageMedical.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.tabPageMisc.ResumeLayout(false);
			this.tabPageMisc.PerformLayout();
			this.tabPageCanada.ResumeLayout(false);
			this.tabPageCanada.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelAmount;
		private System.Windows.Forms.TextBox textProc;
		private System.Windows.Forms.TextBox textSurfaces;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textDesc;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textRange;
		private System.Windows.Forms.Label labelTooth;
		private System.Windows.Forms.Label labelRange;
		private System.Windows.Forms.Label labelSurfaces;
		private System.Windows.Forms.GroupBox groupQuadrant;
		private System.Windows.Forms.RadioButton radioUR;
		private System.Windows.Forms.RadioButton radioUL;
		private System.Windows.Forms.RadioButton radioLL;
		private System.Windows.Forms.RadioButton radioLR;
		private System.Windows.Forms.GroupBox groupArch;
		private System.Windows.Forms.RadioButton radioU;
		private System.Windows.Forms.RadioButton radioL;
		private System.Windows.Forms.GroupBox groupSextant;
		private System.Windows.Forms.RadioButton radioS1;
		private System.Windows.Forms.RadioButton radioS3;
		private System.Windows.Forms.RadioButton radioS2;
		private System.Windows.Forms.RadioButton radioS4;
		private System.Windows.Forms.RadioButton radioS5;
		private System.Windows.Forms.RadioButton radioS6;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelClaim;
		private System.Windows.Forms.ListBox listBoxTeeth;
		private System.Windows.Forms.ListBox listBoxTeeth2;
		private OpenDental.UI.Button butChange;
		private System.Windows.Forms.TextBox textTooth;
		private OpenDental.UI.Button butEditAnyway;
		private System.Windows.Forms.Label labelDx;
		private System.Windows.Forms.ComboBox comboPlaceService;
		private System.Windows.Forms.Label labelPlaceService;
		private OpenDental.UI.Button butSetComplete;
		private System.Windows.Forms.Label labelPriority;
		private System.Windows.Forms.Label labelSetComplete;
		private OpenDental.UI.Button butAddEstimate;
		private OpenDental.ValidDouble textProcFee;
		private System.Windows.Forms.CheckBox checkNoBillIns;
		private OpenDental.ODtextBox textNotes;
		private OpenDental.UI.Button butAddAdjust;
		private System.Windows.Forms.Label labelIncomplete;
		private OpenDental.ValidDate textDateEntry;
		private System.Windows.Forms.Label label12;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label labelMedicalCode;
		private System.Windows.Forms.TextBox textMedicalCode;
		private System.Windows.Forms.Label labelDiagnosisCode;
		private System.Windows.Forms.TextBox textDiagnosisCode;//ENP
		private System.Windows.Forms.CheckBox checkIsPrincDiag;//ENP
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private UI.ComboBoxOD comboProv;
		private System.Windows.Forms.ComboBox comboDx;
		private System.Windows.Forms.ComboBox comboPriority;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label labelCodeMods;
		private System.Windows.Forms.TextBox textCodeMod1;
		private System.Windows.Forms.ComboBox comboBillingTypeTwo;
		private System.Windows.Forms.Label labelBillingTypeTwo;
		private System.Windows.Forms.ComboBox comboBillingTypeOne;
		private System.Windows.Forms.Label labelBillingTypeOne;
		private System.Windows.Forms.TextBox textCodeMod4;
		private System.Windows.Forms.TextBox textCodeMod3;
		private System.Windows.Forms.TextBox textCodeMod2;
		private System.Windows.Forms.TextBox textRevCode;
		private System.Windows.Forms.Label labelRevCode;
		private System.Windows.Forms.TextBox textUnitQty;
		private System.Windows.Forms.Label labelUnitQty;
		private OpenDental.UI.Button buttonUseAutoNote;
		private System.Windows.Forms.ToolTip toolTip1;
		private ValidDate textDateTP;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Panel panelSurfaces;
		private OpenDental.UI.Button butD;
		private OpenDental.UI.Button butBF;
		private OpenDental.UI.Button butL;
		private OpenDental.UI.Button butM;
		private OpenDental.UI.Button butV;
		private OpenDental.UI.Button butOI;
		private OpenDental.UI.Button butPickSite;
		private System.Windows.Forms.TextBox textSite;
		private System.Windows.Forms.Label labelSite;
		private UI.GridOD gridIns;
		private OpenDental.UI.Button butPickProv;
		private System.Windows.Forms.CheckBox checkHideGraphics;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private ValidDate textDateOriginalProsth;
		private OpenDental.UI.ListBoxOD listProsth;
		private System.Windows.Forms.GroupBox groupProsth;
		private System.Windows.Forms.CheckBox checkTypeCodeA;
		private System.Windows.Forms.CheckBox checkTypeCodeB;
		private System.Windows.Forms.CheckBox checkTypeCodeC;
		private System.Windows.Forms.CheckBox checkTypeCodeE;
		private System.Windows.Forms.CheckBox checkTypeCodeL;
		private System.Windows.Forms.CheckBox checkTypeCodeX;
		private System.Windows.Forms.CheckBox checkTypeCodeS;
		private System.Windows.Forms.GroupBox groupCanadianProcTypeCode;
		private System.Windows.Forms.Label labelEndTime;
		private OpenDental.UI.Button butNow;
		private ValidDate textDate;
		private System.Windows.Forms.TextBox textTimeEnd;
		private System.Windows.Forms.TextBox textTimeStart;
		private System.Windows.Forms.Label labelStartTime;
		private System.Windows.Forms.ComboBox comboPrognosis;
		private System.Windows.Forms.Label labelPrognosis;
		private UI.ComboBoxOD comboProcStatus;
		private System.Windows.Forms.ComboBox comboDrugUnit;
		private System.Windows.Forms.Label labelDrugUnit;
		private System.Windows.Forms.Label labelDrugNDC;
		private System.Windows.Forms.TextBox textDrugNDC;
		private System.Windows.Forms.Label labelDrugQty;
		private System.Windows.Forms.TextBox textDrugQty;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textReferral;
		private UI.Button butReferral;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageFinancial;
		private System.Windows.Forms.TabPage tabPageMedical;
		private System.Windows.Forms.TabPage tabPageMisc;
		private System.Windows.Forms.TabPage tabPageCanada;
		private System.Windows.Forms.Label labelUnitType;
		private System.Windows.Forms.ComboBox comboUnitType;
		private System.Windows.Forms.Label labelCanadaLabFee2;
		private System.Windows.Forms.Label labelCanadaLabFee1;
		private ValidDouble textCanadaLabFee2;
		private ValidDouble textCanadaLabFee1;
		private UI.Button butAppend;
		private UI.Button butLock;
		private UI.Button butInvalidate;
		private System.Windows.Forms.Label label18;
		private UI.Button butSearch;
		private UI.Button butSnomedBodySiteSelect;
		private UI.Button butNoneSnomedBodySite;
		private UI.Button butNoneDiagnosisCode;
		private UI.Button butDiagnosisCode;
		private UI.Button butNoneDiagnosisCode2;
		private UI.Button butDiagnosisCode2;
		private UI.Button butNoneDiagnosisCode4;
		private UI.Button butDiagnosisCode4;
		private UI.Button butNoneDiagnosisCode3;
		private UI.Button butDiagnosisCode3;
		private System.Windows.Forms.Label label20;
		private ValidDouble textDiscount;
		private System.Windows.Forms.CheckBox checkIsDateProsthEst;
		private System.Windows.Forms.CheckBox checkIcdVersion;
		private System.Windows.Forms.Label labelIcdVersionUncheck;
		private UI.Button butChangeUser;
		private UI.Button butAddExistAdj;
		private ValidDate textOrigDateComp;
		private ODtextBox textOrderingProviderOverride;
		private UI.Button butPickOrderProvReferral;
		private UI.Button butNoneOrderProv;
		private UI.Button butPickOrderProvInternal;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butEditAutoNote;
		private System.Windows.Forms.ErrorProvider errorProvider2=new System.Windows.Forms.ErrorProvider();
		private System.Windows.Forms.Label labelLocked;
		private System.Windows.Forms.Label labelClaimNote;
		private ODtextBox textClaimNote;
		private System.Windows.Forms.TextBox textTimeFinal;
		private System.Windows.Forms.Label labelTimeFinal;
		private System.Windows.Forms.TextBox textBillingNote;
		private System.Windows.Forms.Label labelSnomedBodySite;
		private System.Windows.Forms.TextBox textSnomedBodySite;
		private System.Windows.Forms.TextBox textDiagnosisCode2;
		private System.Windows.Forms.Label labelDiagnosisCode2;
		private System.Windows.Forms.TextBox textDiagnosisCode3;
		private System.Windows.Forms.Label labelDiagnosisCode3;
		private System.Windows.Forms.TextBox textDiagnosisCode4;
		private System.Windows.Forms.Label labelDiagnosisCode4;
		private UI.GridOD gridAdj;
		private UI.GridOD gridPay;
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private System.Windows.Forms.Label labelOrigDateComp;
		private System.Windows.Forms.Label labelPermAlert;
		private System.Windows.Forms.Label labelTaxEst;
		private ValidDouble textTaxAmt;
		private System.Windows.Forms.CheckBox checkIsEmergency;
	}
}
