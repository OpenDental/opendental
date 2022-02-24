namespace OpenDental {
	partial class FormClaimEdit {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimEdit));
			this.label30 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.label32 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.contextMenuAttachments = new System.Windows.Forms.ContextMenu();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemRename = new System.Windows.Forms.MenuItem();
			this.menuItemRemove = new System.Windows.Forms.MenuItem();
			this.contextAdjust = new System.Windows.Forms.ContextMenu();
			this.menuItemAddAdj = new System.Windows.Forms.MenuItem();
			this.butViewEob = new OpenDental.UI.Button();
			this.groupFinalizePayment = new System.Windows.Forms.GroupBox();
			this.butBatch = new OpenDental.UI.Button();
			this.butThisClaimOnly = new OpenDental.UI.Button();
			this.labelBatch = new System.Windows.Forms.Label();
			this.tabMain = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.groupAccident = new System.Windows.Forms.GroupBox();
			this.comboAccident = new System.Windows.Forms.ComboBox();
			this.label44 = new System.Windows.Forms.Label();
			this.textAccidentDate = new OpenDental.ValidDate();
			this.label42 = new System.Windows.Forms.Label();
			this.textAccidentST = new System.Windows.Forms.TextBox();
			this.labelAccidentST = new System.Windows.Forms.Label();
			this.groupReferral = new System.Windows.Forms.GroupBox();
			this.label45 = new System.Windows.Forms.Label();
			this.textRefProv = new System.Windows.Forms.TextBox();
			this.butReferralEdit = new OpenDental.UI.Button();
			this.label47 = new System.Windows.Forms.Label();
			this.butReferralNone = new OpenDental.UI.Button();
			this.butReferralSelect = new OpenDental.UI.Button();
			this.textRefNum = new System.Windows.Forms.TextBox();
			this.label46 = new System.Windows.Forms.Label();
			this.groupProsth = new System.Windows.Forms.GroupBox();
			this.labelMissingTeeth = new System.Windows.Forms.Label();
			this.textPriorDate = new OpenDental.ValidDate();
			this.label18 = new System.Windows.Forms.Label();
			this.radioProsthN = new System.Windows.Forms.RadioButton();
			this.radioProsthR = new System.Windows.Forms.RadioButton();
			this.radioProsthI = new System.Windows.Forms.RadioButton();
			this.label16 = new System.Windows.Forms.Label();
			this.comboEmployRelated = new System.Windows.Forms.ComboBox();
			this.groupOrtho = new System.Windows.Forms.GroupBox();
			this.textOrthoTotalM = new OpenDental.ValidNum();
			this.label96 = new System.Windows.Forms.Label();
			this.textOrthoDate = new OpenDental.ValidDate();
			this.labelOrthoDate = new System.Windows.Forms.Label();
			this.textOrthoRemainM = new OpenDental.ValidNum();
			this.checkIsOrtho = new System.Windows.Forms.CheckBox();
			this.labelOrthoRemainM = new System.Windows.Forms.Label();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.comboPlaceService = new System.Windows.Forms.ComboBox();
			this.label48 = new System.Windows.Forms.Label();
			this.label49 = new System.Windows.Forms.Label();
			this.tabAttachments = new System.Windows.Forms.TabPage();
			this.tabAttach = new System.Windows.Forms.TabControl();
			this.tabNEA = new System.Windows.Forms.TabPage();
			this.groupAttachments = new System.Windows.Forms.GroupBox();
			this.label65 = new System.Windows.Forms.Label();
			this.textAttachID = new System.Windows.Forms.TextBox();
			this.label64 = new System.Windows.Forms.Label();
			this.radioAttachElect = new System.Windows.Forms.RadioButton();
			this.radioAttachMail = new System.Windows.Forms.RadioButton();
			this.checkAttachMisc = new System.Windows.Forms.CheckBox();
			this.checkAttachPerio = new System.Windows.Forms.CheckBox();
			this.checkAttachNarrative = new System.Windows.Forms.CheckBox();
			this.checkAttachEoB = new System.Windows.Forms.CheckBox();
			this.label63 = new System.Windows.Forms.Label();
			this.textAttachModels = new OpenDental.ValidNum();
			this.labelOralImages = new System.Windows.Forms.Label();
			this.textAttachImages = new OpenDental.ValidNum();
			this.labelRadiographs = new System.Windows.Forms.Label();
			this.textRadiographs = new OpenDental.ValidNum();
			this.groupAttachedImages = new System.Windows.Forms.GroupBox();
			this.butExport = new OpenDental.UI.Button();
			this.butAttachAdd = new OpenDental.UI.Button();
			this.butAttachPerio = new OpenDental.UI.Button();
			this.labelAttachedImagesWillNotBeSent = new System.Windows.Forms.Label();
			this.listAttachments = new OpenDental.UI.ListBoxOD();
			this.tabDXC = new System.Windows.Forms.TabPage();
			this.butClaimAttachment = new OpenDental.UI.Button();
			this.textAttachmentID = new OpenDental.ODtextBox();
			this.labelAttachmentID = new System.Windows.Forms.Label();
			this.gridSent = new OpenDental.UI.GridOD();
			this.tabMisc = new System.Windows.Forms.TabPage();
			this.textClaimIdOriginal = new System.Windows.Forms.TextBox();
			this.labelClaimIdOriginal = new System.Windows.Forms.Label();
			this.labelShareOfCost = new System.Windows.Forms.Label();
			this.labelClaimIdentifierDentiCal = new System.Windows.Forms.Label();
			this.labelOrigRefNumDentiCal = new System.Windows.Forms.Label();
			this.labelPriorAuthDentiCal = new System.Windows.Forms.Label();
			this.textOrigRefNum = new System.Windows.Forms.TextBox();
			this.labelCorrectionType = new System.Windows.Forms.Label();
			this.labelPriorAuth = new System.Windows.Forms.Label();
			this.labelOrigRefNum = new System.Windows.Forms.Label();
			this.textPriorAuth = new System.Windows.Forms.TextBox();
			this.labelSpecialProgram = new System.Windows.Forms.Label();
			this.textClaimIdentifier = new System.Windows.Forms.TextBox();
			this.comboSpecialProgram = new System.Windows.Forms.ComboBox();
			this.labelClaimIdentifier = new System.Windows.Forms.Label();
			this.comboCorrectionType = new System.Windows.Forms.ComboBox();
			this.textShareOfCost = new OpenDental.ValidDouble();
			this.tabUB04 = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butPickOrderProvInternal = new OpenDental.UI.Button();
			this.textOrderingProviderOverride = new OpenDental.ODtextBox();
			this.butPickOrderProvReferral = new OpenDental.UI.Button();
			this.butNoneOrderProv = new OpenDental.UI.Button();
			this.groupUb04 = new System.Windows.Forms.GroupBox();
			this.labelBillType = new System.Windows.Forms.Label();
			this.groupValueCodes = new System.Windows.Forms.GroupBox();
			this.textVC41dAmt = new System.Windows.Forms.TextBox();
			this.textVC41cAmt = new System.Windows.Forms.TextBox();
			this.textVC41bAmt = new System.Windows.Forms.TextBox();
			this.textVC41aAmt = new System.Windows.Forms.TextBox();
			this.textVC41dCode = new System.Windows.Forms.TextBox();
			this.textVC41cCode = new System.Windows.Forms.TextBox();
			this.textVC41bCode = new System.Windows.Forms.TextBox();
			this.textVC41aCode = new System.Windows.Forms.TextBox();
			this.labelVC41Amt = new System.Windows.Forms.Label();
			this.labelVC41Code = new System.Windows.Forms.Label();
			this.labelVC41d = new System.Windows.Forms.Label();
			this.labelVC41c = new System.Windows.Forms.Label();
			this.labelVC41b = new System.Windows.Forms.Label();
			this.labelVC41a = new System.Windows.Forms.Label();
			this.textVC40dAmt = new System.Windows.Forms.TextBox();
			this.textVC40cAmt = new System.Windows.Forms.TextBox();
			this.textVC40bAmt = new System.Windows.Forms.TextBox();
			this.textVC40aAmt = new System.Windows.Forms.TextBox();
			this.textVC40dCode = new System.Windows.Forms.TextBox();
			this.textVC40cCode = new System.Windows.Forms.TextBox();
			this.textVC40bCode = new System.Windows.Forms.TextBox();
			this.textVC40aCode = new System.Windows.Forms.TextBox();
			this.labelVC40Amt = new System.Windows.Forms.Label();
			this.labelVC40Code = new System.Windows.Forms.Label();
			this.labelVC40d = new System.Windows.Forms.Label();
			this.labelVC40c = new System.Windows.Forms.Label();
			this.labelVC40b = new System.Windows.Forms.Label();
			this.labelVC40a = new System.Windows.Forms.Label();
			this.labelVC41 = new System.Windows.Forms.Label();
			this.labelVC40 = new System.Windows.Forms.Label();
			this.labelVC39 = new System.Windows.Forms.Label();
			this.textVC39dAmt = new System.Windows.Forms.TextBox();
			this.textVC39cAmt = new System.Windows.Forms.TextBox();
			this.textVC39bAmt = new System.Windows.Forms.TextBox();
			this.textVC39aAmt = new System.Windows.Forms.TextBox();
			this.textVC39dCode = new System.Windows.Forms.TextBox();
			this.textVC39cCode = new System.Windows.Forms.TextBox();
			this.textVC39bCode = new System.Windows.Forms.TextBox();
			this.textVC39aCode = new System.Windows.Forms.TextBox();
			this.labelVC39Amt = new System.Windows.Forms.Label();
			this.labelVC39Code = new System.Windows.Forms.Label();
			this.labelVC39d = new System.Windows.Forms.Label();
			this.labelVC39c = new System.Windows.Forms.Label();
			this.labelVC39b = new System.Windows.Forms.Label();
			this.labelVC39a = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelCode10 = new System.Windows.Forms.Label();
			this.labelCode9 = new System.Windows.Forms.Label();
			this.labelCode8 = new System.Windows.Forms.Label();
			this.labelCode7 = new System.Windows.Forms.Label();
			this.labelCode6 = new System.Windows.Forms.Label();
			this.labelCode5 = new System.Windows.Forms.Label();
			this.labelCode4 = new System.Windows.Forms.Label();
			this.labelCode3 = new System.Windows.Forms.Label();
			this.labelCode2 = new System.Windows.Forms.Label();
			this.labelCode1 = new System.Windows.Forms.Label();
			this.labelCode0 = new System.Windows.Forms.Label();
			this.textCode10 = new System.Windows.Forms.TextBox();
			this.textCode9 = new System.Windows.Forms.TextBox();
			this.textCode8 = new System.Windows.Forms.TextBox();
			this.textCode7 = new System.Windows.Forms.TextBox();
			this.textCode6 = new System.Windows.Forms.TextBox();
			this.textCode5 = new System.Windows.Forms.TextBox();
			this.textCode4 = new System.Windows.Forms.TextBox();
			this.textCode3 = new System.Windows.Forms.TextBox();
			this.textCode2 = new System.Windows.Forms.TextBox();
			this.textCode1 = new System.Windows.Forms.TextBox();
			this.textCode0 = new System.Windows.Forms.TextBox();
			this.textBillType = new System.Windows.Forms.TextBox();
			this.labelAdmissionType = new System.Windows.Forms.Label();
			this.textPatientStatus = new System.Windows.Forms.TextBox();
			this.textAdmissionType = new System.Windows.Forms.TextBox();
			this.labelPatientStatus = new System.Windows.Forms.Label();
			this.labelAdmissionSource = new System.Windows.Forms.Label();
			this.textAdmissionSource = new System.Windows.Forms.TextBox();
			this.groupDateIllnessInjuryPreg = new System.Windows.Forms.GroupBox();
			this.comboDateIllnessQualifier = new OpenDental.UI.ComboBoxOD();
			this.textDateIllness = new OpenDental.ValidDate();
			this.labelDateIllness = new System.Windows.Forms.Label();
			this.labelDateIllnessQualifier = new System.Windows.Forms.Label();
			this.checkIsOutsideLab = new System.Windows.Forms.CheckBox();
			this.groupDateOtherCondOrTreatment = new System.Windows.Forms.GroupBox();
			this.textDateOther = new OpenDental.ValidDate();
			this.labelDateOtherQualifier = new System.Windows.Forms.Label();
			this.labelDateOther = new System.Windows.Forms.Label();
			this.comboDateOtherQualifier = new OpenDental.UI.ComboBoxOD();
			this.tabCanadian = new System.Windows.Forms.TabPage();
			this.textCanadaTransRefNum = new System.Windows.Forms.TextBox();
			this.groupCanadaOrthoPredeterm = new System.Windows.Forms.GroupBox();
			this.textCanadaExpectedPayCycle = new System.Windows.Forms.TextBox();
			this.textCanadaAnticipatedPayAmount = new System.Windows.Forms.TextBox();
			this.label82 = new System.Windows.Forms.Label();
			this.textCanadaNumPaymentsAnticipated = new System.Windows.Forms.TextBox();
			this.label81 = new System.Windows.Forms.Label();
			this.textCanadaTreatDuration = new System.Windows.Forms.TextBox();
			this.label80 = new System.Windows.Forms.Label();
			this.label79 = new System.Windows.Forms.Label();
			this.textCanadaInitialPayment = new System.Windows.Forms.TextBox();
			this.label78 = new System.Windows.Forms.Label();
			this.label77 = new System.Windows.Forms.Label();
			this.textDateCanadaEstTreatStartDate = new OpenDental.ValidDate();
			this.label76 = new System.Windows.Forms.Label();
			this.butReverse = new OpenDental.UI.Button();
			this.textMissingTeeth = new System.Windows.Forms.TextBox();
			this.labelCanadaMissingTeeth = new System.Windows.Forms.Label();
			this.labelExtractedTeeth = new System.Windows.Forms.Label();
			this.listExtractedTeeth = new OpenDental.UI.ListBoxOD();
			this.checkCanadianIsOrtho = new System.Windows.Forms.CheckBox();
			this.label43 = new System.Windows.Forms.Label();
			this.butMissingTeethHelp = new OpenDental.UI.Button();
			this.groupMandPros = new System.Windows.Forms.GroupBox();
			this.comboMandProsth = new System.Windows.Forms.ComboBox();
			this.label66 = new System.Windows.Forms.Label();
			this.textDateInitialLower = new OpenDental.ValidDate();
			this.label67 = new System.Windows.Forms.Label();
			this.comboMandProsthMaterial = new System.Windows.Forms.ComboBox();
			this.label68 = new System.Windows.Forms.Label();
			this.groupMaxPros = new System.Windows.Forms.GroupBox();
			this.comboMaxProsth = new System.Windows.Forms.ComboBox();
			this.label69 = new System.Windows.Forms.Label();
			this.textDateInitialUpper = new OpenDental.ValidDate();
			this.label70 = new System.Windows.Forms.Label();
			this.comboMaxProsthMaterial = new System.Windows.Forms.ComboBox();
			this.label71 = new System.Windows.Forms.Label();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.checkImages = new System.Windows.Forms.CheckBox();
			this.checkXrays = new System.Windows.Forms.CheckBox();
			this.checkModels = new System.Windows.Forms.CheckBox();
			this.checkCorrespondence = new System.Windows.Forms.CheckBox();
			this.checkEmail = new System.Windows.Forms.CheckBox();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.label73 = new System.Windows.Forms.Label();
			this.label72 = new System.Windows.Forms.Label();
			this.comboReferralReason = new System.Windows.Forms.ComboBox();
			this.textReferralProvider = new System.Windows.Forms.TextBox();
			this.textCanadianAccidentDate = new OpenDental.ValidDate();
			this.tabHistory = new System.Windows.Forms.TabPage();
			this.butAdd = new OpenDental.UI.Button();
			this.gridStatusHistory = new OpenDental.UI.GridOD();
			this.butViewEra = new OpenDental.UI.Button();
			this.butPickProvTreat = new OpenDental.UI.Button();
			this.butPickProvBill = new OpenDental.UI.Button();
			this.butResend = new OpenDental.UI.Button();
			this.textDateSent = new OpenDental.ValidDate();
			this.labelDateSent = new System.Windows.Forms.Label();
			this.comboClaimForm = new System.Windows.Forms.ComboBox();
			this.label87 = new System.Windows.Forms.Label();
			this.comboMedType = new System.Windows.Forms.ComboBox();
			this.label86 = new System.Windows.Forms.Label();
			this.comboClaimType = new System.Windows.Forms.ComboBox();
			this.butHistory = new OpenDental.UI.Button();
			this.textReasonUnder = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.gridPay = new OpenDental.UI.GridOD();
			this.groupEnterPayment = new System.Windows.Forms.GroupBox();
			this.butPaySupp = new OpenDental.UI.Button();
			this.butPayTotal = new OpenDental.UI.Button();
			this.butPayProc = new OpenDental.UI.Button();
			this.label20 = new System.Windows.Forms.Label();
			this.butSend = new OpenDental.UI.Button();
			this.gridProc = new OpenDental.UI.GridOD();
			this.butSplit = new OpenDental.UI.Button();
			this.butLabel = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textDateService = new OpenDental.ValidDate();
			this.textWriteOff = new OpenDental.ValidDouble();
			this.textInsPayEst = new System.Windows.Forms.TextBox();
			this.textInsPayAmt = new OpenDental.ValidDouble();
			this.textClaimFee = new System.Windows.Forms.TextBox();
			this.textDedApplied = new OpenDental.ValidDouble();
			this.textPredeterm = new System.Windows.Forms.TextBox();
			this.textDateSentOrig = new OpenDental.ValidDate();
			this.textDateRec = new OpenDental.ValidDate();
			this.butPreview = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butRecalc = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.butOtherNone = new OpenDental.UI.Button();
			this.butOtherCovChange = new OpenDental.UI.Button();
			this.comboPatRelat2 = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textPlan2 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboPatRelat = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textPlan = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboProvTreat = new OpenDental.UI.ComboBoxOD();
			this.comboProvBill = new OpenDental.UI.ComboBoxOD();
			this.label21 = new System.Windows.Forms.Label();
			this.labelPredeterm = new System.Windows.Forms.Label();
			this.labelDateService = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this._recalcErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.comboClaimStatus = new System.Windows.Forms.ComboBox();
			this.textPatResp = new OpenDental.ValidDouble();
			this.groupFinalizePayment.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.groupAccident.SuspendLayout();
			this.groupReferral.SuspendLayout();
			this.groupProsth.SuspendLayout();
			this.groupOrtho.SuspendLayout();
			this.tabAttachments.SuspendLayout();
			this.tabAttach.SuspendLayout();
			this.tabNEA.SuspendLayout();
			this.groupAttachments.SuspendLayout();
			this.groupAttachedImages.SuspendLayout();
			this.tabDXC.SuspendLayout();
			this.tabMisc.SuspendLayout();
			this.tabUB04.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupUb04.SuspendLayout();
			this.groupValueCodes.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupDateIllnessInjuryPreg.SuspendLayout();
			this.groupDateOtherCondOrTreatment.SuspendLayout();
			this.tabCanadian.SuspendLayout();
			this.groupCanadaOrthoPredeterm.SuspendLayout();
			this.groupMandPros.SuspendLayout();
			this.groupMaxPros.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.tabHistory.SuspendLayout();
			this.groupEnterPayment.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._recalcErrorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// label30
			// 
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(358, 16);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(42, 13);
			this.label30.TabIndex = 48;
			this.label30.Text = "amount";
			// 
			// label31
			// 
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(314, 16);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(31, 13);
			this.label31.TabIndex = 47;
			this.label31.Text = "code";
			// 
			// label32
			// 
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(295, 92);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(13, 13);
			this.label32.TabIndex = 46;
			this.label32.Text = "d";
			// 
			// label33
			// 
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(295, 73);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(13, 13);
			this.label33.TabIndex = 45;
			this.label33.Text = "c";
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(295, 54);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(13, 13);
			this.label34.TabIndex = 44;
			this.label34.Text = "b";
			// 
			// label35
			// 
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(295, 35);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(13, 13);
			this.label35.TabIndex = 43;
			this.label35.Text = "a";
			// 
			// contextMenuAttachments
			// 
			this.contextMenuAttachments.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOpen,
            this.menuItemRename,
            this.menuItemRemove});
			this.contextMenuAttachments.Popup += new System.EventHandler(this.contextMenuAttachments_Popup);
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.Text = "Open";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemRename
			// 
			this.menuItemRename.Index = 1;
			this.menuItemRename.Text = "Rename";
			this.menuItemRename.Click += new System.EventHandler(this.menuItemRename_Click);
			// 
			// menuItemRemove
			// 
			this.menuItemRemove.Index = 2;
			this.menuItemRemove.Text = "Remove";
			this.menuItemRemove.Click += new System.EventHandler(this.menuItemRemove_Click);
			// 
			// contextAdjust
			// 
			this.contextAdjust.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAddAdj});
			this.contextAdjust.Popup += new System.EventHandler(this.contextAdjust_Popup);
			// 
			// menuItemAddAdj
			// 
			this.menuItemAddAdj.Enabled = false;
			this.menuItemAddAdj.Index = 0;
			this.menuItemAddAdj.Text = "Add Adjustment";
			this.menuItemAddAdj.Click += new System.EventHandler(this.menuItemAddAdj_Click);
			// 
			// butViewEob
			// 
			this.butViewEob.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butViewEob.Location = new System.Drawing.Point(100, 364);
			this.butViewEob.Name = "butViewEob";
			this.butViewEob.Size = new System.Drawing.Size(94, 24);
			this.butViewEob.TabIndex = 266;
			this.butViewEob.Text = "View EOB";
			this.butViewEob.Click += new System.EventHandler(this.butViewEob_Click);
			// 
			// groupFinalizePayment
			// 
			this.groupFinalizePayment.Controls.Add(this.butBatch);
			this.groupFinalizePayment.Controls.Add(this.butThisClaimOnly);
			this.groupFinalizePayment.Controls.Add(this.labelBatch);
			this.groupFinalizePayment.Location = new System.Drawing.Point(577, 383);
			this.groupFinalizePayment.Name = "groupFinalizePayment";
			this.groupFinalizePayment.Size = new System.Drawing.Size(170, 92);
			this.groupFinalizePayment.TabIndex = 265;
			this.groupFinalizePayment.TabStop = false;
			this.groupFinalizePayment.Text = "Finalize Payment";
			// 
			// butBatch
			// 
			this.butBatch.Icon = OpenDental.UI.EnumIcons.Add;
			this.butBatch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBatch.Location = new System.Drawing.Point(22, 14);
			this.butBatch.Name = "butBatch";
			this.butBatch.Size = new System.Drawing.Size(130, 24);
			this.butBatch.TabIndex = 150;
			this.butBatch.Text = "Batch";
			this.butBatch.Click += new System.EventHandler(this.butBatch_Click);
			// 
			// butThisClaimOnly
			// 
			this.butThisClaimOnly.Icon = OpenDental.UI.EnumIcons.Add;
			this.butThisClaimOnly.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butThisClaimOnly.Location = new System.Drawing.Point(22, 66);
			this.butThisClaimOnly.Name = "butThisClaimOnly";
			this.butThisClaimOnly.Size = new System.Drawing.Size(130, 24);
			this.butThisClaimOnly.TabIndex = 264;
			this.butThisClaimOnly.Text = "This Claim Only";
			this.butThisClaimOnly.Click += new System.EventHandler(this.butThisClaimOnly_Click);
			// 
			// labelBatch
			// 
			this.labelBatch.Location = new System.Drawing.Point(2, 38);
			this.labelBatch.Name = "labelBatch";
			this.labelBatch.Size = new System.Drawing.Size(167, 28);
			this.labelBatch.TabIndex = 151;
			this.labelBatch.Text = "Click Batch after entering all ins payments for one EOB; or";
			this.labelBatch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabMain
			// 
			this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabMain.Controls.Add(this.tabGeneral);
			this.tabMain.Controls.Add(this.tabAttachments);
			this.tabMain.Controls.Add(this.tabMisc);
			this.tabMain.Controls.Add(this.tabUB04);
			this.tabMain.Controls.Add(this.tabCanadian);
			this.tabMain.Controls.Add(this.tabHistory);
			this.tabMain.Location = new System.Drawing.Point(2, 478);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(1148, 164);
			this.tabMain.TabIndex = 133;
			// 
			// tabGeneral
			// 
			this.tabGeneral.AutoScroll = true;
			this.tabGeneral.BackColor = System.Drawing.Color.Transparent;
			this.tabGeneral.Controls.Add(this.groupAccident);
			this.tabGeneral.Controls.Add(this.groupReferral);
			this.tabGeneral.Controls.Add(this.groupProsth);
			this.tabGeneral.Controls.Add(this.comboEmployRelated);
			this.tabGeneral.Controls.Add(this.groupOrtho);
			this.tabGeneral.Controls.Add(this.labelNote);
			this.tabGeneral.Controls.Add(this.textNote);
			this.tabGeneral.Controls.Add(this.comboPlaceService);
			this.tabGeneral.Controls.Add(this.label48);
			this.tabGeneral.Controls.Add(this.label49);
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Size = new System.Drawing.Size(1140, 138);
			this.tabGeneral.TabIndex = 2;
			this.tabGeneral.Text = "General";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// groupAccident
			// 
			this.groupAccident.Controls.Add(this.comboAccident);
			this.groupAccident.Controls.Add(this.label44);
			this.groupAccident.Controls.Add(this.textAccidentDate);
			this.groupAccident.Controls.Add(this.label42);
			this.groupAccident.Controls.Add(this.textAccidentST);
			this.groupAccident.Controls.Add(this.labelAccidentST);
			this.groupAccident.Location = new System.Drawing.Point(3, 165);
			this.groupAccident.Name = "groupAccident";
			this.groupAccident.Size = new System.Drawing.Size(286, 79);
			this.groupAccident.TabIndex = 149;
			this.groupAccident.TabStop = false;
			this.groupAccident.Text = "Accident";
			// 
			// comboAccident
			// 
			this.comboAccident.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAccident.FormattingEnabled = true;
			this.comboAccident.Location = new System.Drawing.Point(136, 14);
			this.comboAccident.Name = "comboAccident";
			this.comboAccident.Size = new System.Drawing.Size(101, 21);
			this.comboAccident.TabIndex = 142;
			// 
			// label44
			// 
			this.label44.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label44.Location = new System.Drawing.Point(17, 36);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(117, 17);
			this.label44.TabIndex = 130;
			this.label44.Text = "Accident Date";
			this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAccidentDate
			// 
			this.textAccidentDate.Location = new System.Drawing.Point(136, 35);
			this.textAccidentDate.Name = "textAccidentDate";
			this.textAccidentDate.Size = new System.Drawing.Size(75, 20);
			this.textAccidentDate.TabIndex = 128;
			// 
			// label42
			// 
			this.label42.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label42.Location = new System.Drawing.Point(17, 15);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(117, 17);
			this.label42.TabIndex = 143;
			this.label42.Text = "Accident Related";
			this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAccidentST
			// 
			this.textAccidentST.Location = new System.Drawing.Point(136, 55);
			this.textAccidentST.Name = "textAccidentST";
			this.textAccidentST.Size = new System.Drawing.Size(30, 20);
			this.textAccidentST.TabIndex = 129;
			// 
			// labelAccidentST
			// 
			this.labelAccidentST.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelAccidentST.Location = new System.Drawing.Point(17, 56);
			this.labelAccidentST.Name = "labelAccidentST";
			this.labelAccidentST.Size = new System.Drawing.Size(117, 17);
			this.labelAccidentST.TabIndex = 134;
			this.labelAccidentST.Text = "Accident State";
			this.labelAccidentST.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupReferral
			// 
			this.groupReferral.Controls.Add(this.label45);
			this.groupReferral.Controls.Add(this.textRefProv);
			this.groupReferral.Controls.Add(this.butReferralEdit);
			this.groupReferral.Controls.Add(this.label47);
			this.groupReferral.Controls.Add(this.butReferralNone);
			this.groupReferral.Controls.Add(this.butReferralSelect);
			this.groupReferral.Controls.Add(this.textRefNum);
			this.groupReferral.Controls.Add(this.label46);
			this.groupReferral.Location = new System.Drawing.Point(306, 118);
			this.groupReferral.Name = "groupReferral";
			this.groupReferral.Size = new System.Drawing.Size(297, 118);
			this.groupReferral.TabIndex = 118;
			this.groupReferral.TabStop = false;
			this.groupReferral.Text = "Claim Referral";
			// 
			// label45
			// 
			this.label45.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label45.Location = new System.Drawing.Point(10, 16);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(282, 30);
			this.label45.TabIndex = 133;
			this.label45.Text = "Only enter referring provider and referral number if required by your insurance c" +
    "arrier.";
			// 
			// textRefProv
			// 
			this.textRefProv.BackColor = System.Drawing.SystemColors.Window;
			this.textRefProv.Location = new System.Drawing.Point(109, 49);
			this.textRefProv.Name = "textRefProv";
			this.textRefProv.ReadOnly = true;
			this.textRefProv.Size = new System.Drawing.Size(175, 20);
			this.textRefProv.TabIndex = 139;
			// 
			// butReferralEdit
			// 
			this.butReferralEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butReferralEdit.Location = new System.Drawing.Point(227, 70);
			this.butReferralEdit.Name = "butReferralEdit";
			this.butReferralEdit.Size = new System.Drawing.Size(57, 24);
			this.butReferralEdit.TabIndex = 144;
			this.butReferralEdit.Text = "Edit";
			this.butReferralEdit.Click += new System.EventHandler(this.butReferralEdit_Click);
			// 
			// label47
			// 
			this.label47.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label47.Location = new System.Drawing.Point(9, 51);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(99, 14);
			this.label47.TabIndex = 131;
			this.label47.Text = "Referring Provider";
			this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butReferralNone
			// 
			this.butReferralNone.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butReferralNone.Location = new System.Drawing.Point(109, 70);
			this.butReferralNone.Name = "butReferralNone";
			this.butReferralNone.Size = new System.Drawing.Size(57, 24);
			this.butReferralNone.TabIndex = 135;
			this.butReferralNone.Text = "&None";
			this.butReferralNone.Click += new System.EventHandler(this.butReferralNone_Click);
			// 
			// butReferralSelect
			// 
			this.butReferralSelect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butReferralSelect.Location = new System.Drawing.Point(168, 70);
			this.butReferralSelect.Name = "butReferralSelect";
			this.butReferralSelect.Size = new System.Drawing.Size(57, 24);
			this.butReferralSelect.TabIndex = 138;
			this.butReferralSelect.Text = "Select";
			this.butReferralSelect.Click += new System.EventHandler(this.butReferralSelect_Click);
			// 
			// textRefNum
			// 
			this.textRefNum.Location = new System.Drawing.Point(109, 95);
			this.textRefNum.Name = "textRefNum";
			this.textRefNum.Size = new System.Drawing.Size(175, 20);
			this.textRefNum.TabIndex = 127;
			// 
			// label46
			// 
			this.label46.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label46.Location = new System.Drawing.Point(18, 97);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(90, 18);
			this.label46.TabIndex = 132;
			this.label46.Text = "Referral Number";
			this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupProsth
			// 
			this.groupProsth.BackColor = System.Drawing.SystemColors.Window;
			this.groupProsth.Controls.Add(this.labelMissingTeeth);
			this.groupProsth.Controls.Add(this.textPriorDate);
			this.groupProsth.Controls.Add(this.label18);
			this.groupProsth.Controls.Add(this.radioProsthN);
			this.groupProsth.Controls.Add(this.radioProsthR);
			this.groupProsth.Controls.Add(this.radioProsthI);
			this.groupProsth.Controls.Add(this.label16);
			this.groupProsth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupProsth.Location = new System.Drawing.Point(3, 3);
			this.groupProsth.Name = "groupProsth";
			this.groupProsth.Size = new System.Drawing.Size(286, 114);
			this.groupProsth.TabIndex = 9;
			this.groupProsth.TabStop = false;
			this.groupProsth.Text = "Crown, Bridge, or Denture";
			// 
			// labelMissingTeeth
			// 
			this.labelMissingTeeth.Location = new System.Drawing.Point(3, 77);
			this.labelMissingTeeth.Name = "labelMissingTeeth";
			this.labelMissingTeeth.Size = new System.Drawing.Size(280, 32);
			this.labelMissingTeeth.TabIndex = 28;
			this.labelMissingTeeth.Text = "For bridges, dentures, and partials, missing teeth must have been correctly enter" +
    "ed in the Chart module. ";
			// 
			// textPriorDate
			// 
			this.textPriorDate.Location = new System.Drawing.Point(168, 36);
			this.textPriorDate.Name = "textPriorDate";
			this.textPriorDate.Size = new System.Drawing.Size(66, 20);
			this.textPriorDate.TabIndex = 3;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(6, 60);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(246, 18);
			this.label18.TabIndex = 29;
			this.label18.Text = "(Might need a note. Might need to attach x-ray)";
			// 
			// radioProsthN
			// 
			this.radioProsthN.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioProsthN.Location = new System.Drawing.Point(12, 18);
			this.radioProsthN.Name = "radioProsthN";
			this.radioProsthN.Size = new System.Drawing.Size(46, 16);
			this.radioProsthN.TabIndex = 0;
			this.radioProsthN.Text = "No";
			this.radioProsthN.Click += new System.EventHandler(this.radioProsthN_Click);
			// 
			// radioProsthR
			// 
			this.radioProsthR.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioProsthR.Location = new System.Drawing.Point(132, 18);
			this.radioProsthR.Name = "radioProsthR";
			this.radioProsthR.Size = new System.Drawing.Size(104, 16);
			this.radioProsthR.TabIndex = 2;
			this.radioProsthR.Text = "Replacement";
			this.radioProsthR.Click += new System.EventHandler(this.radioProsthR_Click);
			// 
			// radioProsthI
			// 
			this.radioProsthI.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioProsthI.Location = new System.Drawing.Point(64, 18);
			this.radioProsthI.Name = "radioProsthI";
			this.radioProsthI.Size = new System.Drawing.Size(64, 16);
			this.radioProsthI.TabIndex = 1;
			this.radioProsthI.Text = "Initial";
			this.radioProsthI.Click += new System.EventHandler(this.radioProsthI_Click);
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(6, 40);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(157, 16);
			this.label16.TabIndex = 16;
			this.label16.Text = "Prior Date of Placement";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboEmployRelated
			// 
			this.comboEmployRelated.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmployRelated.FormattingEnabled = true;
			this.comboEmployRelated.Location = new System.Drawing.Point(134, 143);
			this.comboEmployRelated.Name = "comboEmployRelated";
			this.comboEmployRelated.Size = new System.Drawing.Size(155, 21);
			this.comboEmployRelated.TabIndex = 141;
			// 
			// groupOrtho
			// 
			this.groupOrtho.BackColor = System.Drawing.SystemColors.Window;
			this.groupOrtho.Controls.Add(this.textOrthoTotalM);
			this.groupOrtho.Controls.Add(this.label96);
			this.groupOrtho.Controls.Add(this.textOrthoDate);
			this.groupOrtho.Controls.Add(this.labelOrthoDate);
			this.groupOrtho.Controls.Add(this.textOrthoRemainM);
			this.groupOrtho.Controls.Add(this.checkIsOrtho);
			this.groupOrtho.Controls.Add(this.labelOrthoRemainM);
			this.groupOrtho.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupOrtho.Location = new System.Drawing.Point(306, 3);
			this.groupOrtho.Name = "groupOrtho";
			this.groupOrtho.Size = new System.Drawing.Size(192, 114);
			this.groupOrtho.TabIndex = 11;
			this.groupOrtho.TabStop = false;
			this.groupOrtho.Text = "Ortho";
			// 
			// textOrthoTotalM
			// 
			this.textOrthoTotalM.Location = new System.Drawing.Point(115, 58);
			this.textOrthoTotalM.Name = "textOrthoTotalM";
			this.textOrthoTotalM.Size = new System.Drawing.Size(39, 20);
			this.textOrthoTotalM.TabIndex = 105;
			// 
			// label96
			// 
			this.label96.Location = new System.Drawing.Point(2, 59);
			this.label96.Name = "label96";
			this.label96.Size = new System.Drawing.Size(112, 18);
			this.label96.TabIndex = 106;
			this.label96.Text = "Months Total";
			this.label96.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOrthoDate
			// 
			this.textOrthoDate.Location = new System.Drawing.Point(115, 36);
			this.textOrthoDate.Name = "textOrthoDate";
			this.textOrthoDate.Size = new System.Drawing.Size(66, 20);
			this.textOrthoDate.TabIndex = 1;
			// 
			// labelOrthoDate
			// 
			this.labelOrthoDate.Location = new System.Drawing.Point(5, 40);
			this.labelOrthoDate.Name = "labelOrthoDate";
			this.labelOrthoDate.Size = new System.Drawing.Size(109, 16);
			this.labelOrthoDate.TabIndex = 104;
			this.labelOrthoDate.Text = "Date of Placement";
			this.labelOrthoDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textOrthoRemainM
			// 
			this.textOrthoRemainM.Location = new System.Drawing.Point(115, 80);
			this.textOrthoRemainM.Name = "textOrthoRemainM";
			this.textOrthoRemainM.Size = new System.Drawing.Size(39, 20);
			this.textOrthoRemainM.TabIndex = 2;
			// 
			// checkIsOrtho
			// 
			this.checkIsOrtho.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsOrtho.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsOrtho.Location = new System.Drawing.Point(38, 15);
			this.checkIsOrtho.Name = "checkIsOrtho";
			this.checkIsOrtho.Size = new System.Drawing.Size(90, 18);
			this.checkIsOrtho.TabIndex = 0;
			this.checkIsOrtho.Text = "Is For Ortho";
			this.checkIsOrtho.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOrthoRemainM
			// 
			this.labelOrthoRemainM.Location = new System.Drawing.Point(2, 81);
			this.labelOrthoRemainM.Name = "labelOrthoRemainM";
			this.labelOrthoRemainM.Size = new System.Drawing.Size(112, 18);
			this.labelOrthoRemainM.TabIndex = 102;
			this.labelOrthoRemainM.Text = "Months Remaining";
			this.labelOrthoRemainM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(616, 3);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(299, 16);
			this.labelNote.TabIndex = 19;
			this.labelNote.Text = "Claim Note (this will show on the claim when submitted)";
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(619, 22);
			this.textNote.MaxLength = 400;
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(508, 136);
			this.textNote.TabIndex = 118;
			this.textNote.Text = "";
			// 
			// comboPlaceService
			// 
			this.comboPlaceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlaceService.FormattingEnabled = true;
			this.comboPlaceService.Location = new System.Drawing.Point(134, 122);
			this.comboPlaceService.Name = "comboPlaceService";
			this.comboPlaceService.Size = new System.Drawing.Size(155, 21);
			this.comboPlaceService.TabIndex = 140;
			// 
			// label48
			// 
			this.label48.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label48.Location = new System.Drawing.Point(15, 123);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(117, 17);
			this.label48.TabIndex = 136;
			this.label48.Text = "Place of Service";
			this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label49
			// 
			this.label49.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label49.Location = new System.Drawing.Point(15, 144);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(117, 17);
			this.label49.TabIndex = 137;
			this.label49.Text = "Employment Related";
			this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabAttachments
			// 
			this.tabAttachments.AutoScroll = true;
			this.tabAttachments.Controls.Add(this.tabAttach);
			this.tabAttachments.Location = new System.Drawing.Point(4, 22);
			this.tabAttachments.Name = "tabAttachments";
			this.tabAttachments.Padding = new System.Windows.Forms.Padding(3);
			this.tabAttachments.Size = new System.Drawing.Size(1140, 274);
			this.tabAttachments.TabIndex = 6;
			this.tabAttachments.Text = "Attachments";
			this.tabAttachments.UseVisualStyleBackColor = true;
			// 
			// tabAttach
			// 
			this.tabAttach.Controls.Add(this.tabNEA);
			this.tabAttach.Controls.Add(this.tabDXC);
			this.tabAttach.Location = new System.Drawing.Point(6, 6);
			this.tabAttach.Name = "tabAttach";
			this.tabAttach.SelectedIndex = 0;
			this.tabAttach.Size = new System.Drawing.Size(1128, 262);
			this.tabAttach.TabIndex = 152;
			// 
			// tabNEA
			// 
			this.tabNEA.Controls.Add(this.groupAttachments);
			this.tabNEA.Controls.Add(this.groupAttachedImages);
			this.tabNEA.Location = new System.Drawing.Point(4, 22);
			this.tabNEA.Name = "tabNEA";
			this.tabNEA.Padding = new System.Windows.Forms.Padding(3);
			this.tabNEA.Size = new System.Drawing.Size(1120, 236);
			this.tabNEA.TabIndex = 0;
			this.tabNEA.Text = "NEA/Manual";
			this.tabNEA.UseVisualStyleBackColor = true;
			// 
			// groupAttachments
			// 
			this.groupAttachments.Controls.Add(this.label65);
			this.groupAttachments.Controls.Add(this.textAttachID);
			this.groupAttachments.Controls.Add(this.label64);
			this.groupAttachments.Controls.Add(this.radioAttachElect);
			this.groupAttachments.Controls.Add(this.radioAttachMail);
			this.groupAttachments.Controls.Add(this.checkAttachMisc);
			this.groupAttachments.Controls.Add(this.checkAttachPerio);
			this.groupAttachments.Controls.Add(this.checkAttachNarrative);
			this.groupAttachments.Controls.Add(this.checkAttachEoB);
			this.groupAttachments.Controls.Add(this.label63);
			this.groupAttachments.Controls.Add(this.textAttachModels);
			this.groupAttachments.Controls.Add(this.labelOralImages);
			this.groupAttachments.Controls.Add(this.textAttachImages);
			this.groupAttachments.Controls.Add(this.labelRadiographs);
			this.groupAttachments.Controls.Add(this.textRadiographs);
			this.groupAttachments.Location = new System.Drawing.Point(11, 16);
			this.groupAttachments.Name = "groupAttachments";
			this.groupAttachments.Size = new System.Drawing.Size(319, 178);
			this.groupAttachments.TabIndex = 149;
			this.groupAttachments.TabStop = false;
			this.groupAttachments.Text = "Attachments";
			// 
			// label65
			// 
			this.label65.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label65.Location = new System.Drawing.Point(4, 16);
			this.label65.Name = "label65";
			this.label65.Size = new System.Drawing.Size(313, 18);
			this.label65.TabIndex = 152;
			this.label65.Text = "The attachments indicated here must be sent separately.";
			// 
			// textAttachID
			// 
			this.textAttachID.Location = new System.Drawing.Point(171, 152);
			this.textAttachID.Name = "textAttachID";
			this.textAttachID.Size = new System.Drawing.Size(142, 20);
			this.textAttachID.TabIndex = 133;
			// 
			// label64
			// 
			this.label64.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label64.Location = new System.Drawing.Point(170, 120);
			this.label64.Name = "label64";
			this.label64.Size = new System.Drawing.Size(141, 29);
			this.label64.TabIndex = 134;
			this.label64.Text = "Attachment ID Number\r\n(example: NEA#1234567)";
			this.label64.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// radioAttachElect
			// 
			this.radioAttachElect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAttachElect.Location = new System.Drawing.Point(171, 59);
			this.radioAttachElect.Name = "radioAttachElect";
			this.radioAttachElect.Size = new System.Drawing.Size(104, 16);
			this.radioAttachElect.TabIndex = 129;
			this.radioAttachElect.Text = "Electronically";
			// 
			// radioAttachMail
			// 
			this.radioAttachMail.Checked = true;
			this.radioAttachMail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAttachMail.Location = new System.Drawing.Point(171, 39);
			this.radioAttachMail.Name = "radioAttachMail";
			this.radioAttachMail.Size = new System.Drawing.Size(104, 16);
			this.radioAttachMail.TabIndex = 128;
			this.radioAttachMail.TabStop = true;
			this.radioAttachMail.Text = "By Mail";
			// 
			// checkAttachMisc
			// 
			this.checkAttachMisc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachMisc.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAttachMisc.Location = new System.Drawing.Point(9, 154);
			this.checkAttachMisc.Name = "checkAttachMisc";
			this.checkAttachMisc.Size = new System.Drawing.Size(112, 18);
			this.checkAttachMisc.TabIndex = 125;
			this.checkAttachMisc.Text = "Misc Support Data";
			this.checkAttachMisc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAttachPerio
			// 
			this.checkAttachPerio.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachPerio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAttachPerio.Location = new System.Drawing.Point(31, 137);
			this.checkAttachPerio.Name = "checkAttachPerio";
			this.checkAttachPerio.Size = new System.Drawing.Size(90, 18);
			this.checkAttachPerio.TabIndex = 124;
			this.checkAttachPerio.Text = "Perio Chart";
			this.checkAttachPerio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAttachNarrative
			// 
			this.checkAttachNarrative.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachNarrative.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAttachNarrative.Location = new System.Drawing.Point(31, 120);
			this.checkAttachNarrative.Name = "checkAttachNarrative";
			this.checkAttachNarrative.Size = new System.Drawing.Size(90, 18);
			this.checkAttachNarrative.TabIndex = 123;
			this.checkAttachNarrative.Text = "Narrative";
			this.checkAttachNarrative.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAttachEoB
			// 
			this.checkAttachEoB.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachEoB.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAttachEoB.Location = new System.Drawing.Point(31, 103);
			this.checkAttachEoB.Name = "checkAttachEoB";
			this.checkAttachEoB.Size = new System.Drawing.Size(90, 18);
			this.checkAttachEoB.TabIndex = 122;
			this.checkAttachEoB.Text = "EoB";
			this.checkAttachEoB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label63
			// 
			this.label63.Location = new System.Drawing.Point(28, 81);
			this.label63.Name = "label63";
			this.label63.Size = new System.Drawing.Size(79, 18);
			this.label63.TabIndex = 121;
			this.label63.Text = "Models";
			this.label63.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAttachModels
			// 
			this.textAttachModels.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textAttachModels.Location = new System.Drawing.Point(108, 81);
			this.textAttachModels.Name = "textAttachModels";
			this.textAttachModels.Size = new System.Drawing.Size(39, 20);
			this.textAttachModels.TabIndex = 120;
			this.textAttachModels.Text = "0";
			// 
			// labelOralImages
			// 
			this.labelOralImages.Location = new System.Drawing.Point(28, 60);
			this.labelOralImages.Name = "labelOralImages";
			this.labelOralImages.Size = new System.Drawing.Size(79, 18);
			this.labelOralImages.TabIndex = 119;
			this.labelOralImages.Text = "Oral Images";
			this.labelOralImages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAttachImages
			// 
			this.textAttachImages.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textAttachImages.Location = new System.Drawing.Point(108, 60);
			this.textAttachImages.Name = "textAttachImages";
			this.textAttachImages.Size = new System.Drawing.Size(39, 20);
			this.textAttachImages.TabIndex = 118;
			this.textAttachImages.Text = "0";
			// 
			// labelRadiographs
			// 
			this.labelRadiographs.Location = new System.Drawing.Point(28, 39);
			this.labelRadiographs.Name = "labelRadiographs";
			this.labelRadiographs.Size = new System.Drawing.Size(79, 18);
			this.labelRadiographs.TabIndex = 117;
			this.labelRadiographs.Text = "Radiographs";
			this.labelRadiographs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRadiographs
			// 
			this.textRadiographs.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textRadiographs.Location = new System.Drawing.Point(108, 39);
			this.textRadiographs.Name = "textRadiographs";
			this.textRadiographs.Size = new System.Drawing.Size(39, 20);
			this.textRadiographs.TabIndex = 116;
			this.textRadiographs.Text = "0";
			// 
			// groupAttachedImages
			// 
			this.groupAttachedImages.Controls.Add(this.butExport);
			this.groupAttachedImages.Controls.Add(this.butAttachAdd);
			this.groupAttachedImages.Controls.Add(this.butAttachPerio);
			this.groupAttachedImages.Controls.Add(this.labelAttachedImagesWillNotBeSent);
			this.groupAttachedImages.Controls.Add(this.listAttachments);
			this.groupAttachedImages.Location = new System.Drawing.Point(336, 16);
			this.groupAttachedImages.Name = "groupAttachedImages";
			this.groupAttachedImages.Size = new System.Drawing.Size(319, 143);
			this.groupAttachedImages.TabIndex = 150;
			this.groupAttachedImages.TabStop = false;
			this.groupAttachedImages.Text = "Attached Images";
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butExport.Location = new System.Drawing.Point(252, 42);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(62, 24);
			this.butExport.TabIndex = 150;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butAttachAdd
			// 
			this.butAttachAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAttachAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butAttachAdd.Location = new System.Drawing.Point(122, 42);
			this.butAttachAdd.Name = "butAttachAdd";
			this.butAttachAdd.Size = new System.Drawing.Size(62, 24);
			this.butAttachAdd.TabIndex = 147;
			this.butAttachAdd.Text = "Add";
			this.butAttachAdd.Click += new System.EventHandler(this.butAttachAdd_Click);
			// 
			// butAttachPerio
			// 
			this.butAttachPerio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAttachPerio.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butAttachPerio.Location = new System.Drawing.Point(187, 42);
			this.butAttachPerio.Name = "butAttachPerio";
			this.butAttachPerio.Size = new System.Drawing.Size(62, 24);
			this.butAttachPerio.TabIndex = 146;
			this.butAttachPerio.Text = "Perio";
			this.butAttachPerio.Click += new System.EventHandler(this.butAttachPerio_Click);
			// 
			// labelAttachedImagesWillNotBeSent
			// 
			this.labelAttachedImagesWillNotBeSent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelAttachedImagesWillNotBeSent.Location = new System.Drawing.Point(7, 16);
			this.labelAttachedImagesWillNotBeSent.Name = "labelAttachedImagesWillNotBeSent";
			this.labelAttachedImagesWillNotBeSent.Size = new System.Drawing.Size(282, 30);
			this.labelAttachedImagesWillNotBeSent.TabIndex = 151;
			this.labelAttachedImagesWillNotBeSent.Text = "These images will NOT be automatically sent with an electronic claim.";
			// 
			// listAttachments
			// 
			this.listAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listAttachments.Location = new System.Drawing.Point(10, 68);
			this.listAttachments.Name = "listAttachments";
			this.listAttachments.Size = new System.Drawing.Size(304, 69);
			this.listAttachments.TabIndex = 149;
			// 
			// tabDXC
			// 
			this.tabDXC.Controls.Add(this.butClaimAttachment);
			this.tabDXC.Controls.Add(this.textAttachmentID);
			this.tabDXC.Controls.Add(this.labelAttachmentID);
			this.tabDXC.Controls.Add(this.gridSent);
			this.tabDXC.Location = new System.Drawing.Point(4, 22);
			this.tabDXC.Name = "tabDXC";
			this.tabDXC.Padding = new System.Windows.Forms.Padding(3);
			this.tabDXC.Size = new System.Drawing.Size(1120, 236);
			this.tabDXC.TabIndex = 1;
			this.tabDXC.Text = "DXC";
			this.tabDXC.UseVisualStyleBackColor = true;
			// 
			// butClaimAttachment
			// 
			this.butClaimAttachment.Location = new System.Drawing.Point(6, 62);
			this.butClaimAttachment.Name = "butClaimAttachment";
			this.butClaimAttachment.Size = new System.Drawing.Size(113, 24);
			this.butClaimAttachment.TabIndex = 152;
			this.butClaimAttachment.Text = "Add Attachment";
			this.butClaimAttachment.UseVisualStyleBackColor = true;
			this.butClaimAttachment.Click += new System.EventHandler(this.buttonClaimAttachment_Click);
			// 
			// textAttachmentID
			// 
			this.textAttachmentID.AcceptsTab = true;
			this.textAttachmentID.BackColor = System.Drawing.SystemColors.Control;
			this.textAttachmentID.DetectLinksEnabled = false;
			this.textAttachmentID.DetectUrls = false;
			this.textAttachmentID.Location = new System.Drawing.Point(6, 35);
			this.textAttachmentID.Name = "textAttachmentID";
			this.textAttachmentID.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textAttachmentID.ReadOnly = true;
			this.textAttachmentID.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAttachmentID.Size = new System.Drawing.Size(176, 21);
			this.textAttachmentID.TabIndex = 10;
			this.textAttachmentID.Text = "";
			// 
			// labelAttachmentID
			// 
			this.labelAttachmentID.AutoSize = true;
			this.labelAttachmentID.Location = new System.Drawing.Point(3, 19);
			this.labelAttachmentID.Name = "labelAttachmentID";
			this.labelAttachmentID.Size = new System.Drawing.Size(75, 13);
			this.labelAttachmentID.TabIndex = 11;
			this.labelAttachmentID.Text = "Attachment ID";
			// 
			// gridSent
			// 
			this.gridSent.Location = new System.Drawing.Point(200, 6);
			this.gridSent.Name = "gridSent";
			this.gridSent.Size = new System.Drawing.Size(261, 224);
			this.gridSent.TabIndex = 0;
			this.gridSent.Title = "Attachments Sent";
			this.gridSent.TranslationName = "gridSent";
			this.gridSent.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSent_CellDoubleClick);
			// 
			// tabMisc
			// 
			this.tabMisc.AutoScroll = true;
			this.tabMisc.Controls.Add(this.textClaimIdOriginal);
			this.tabMisc.Controls.Add(this.labelClaimIdOriginal);
			this.tabMisc.Controls.Add(this.labelShareOfCost);
			this.tabMisc.Controls.Add(this.labelClaimIdentifierDentiCal);
			this.tabMisc.Controls.Add(this.labelOrigRefNumDentiCal);
			this.tabMisc.Controls.Add(this.labelPriorAuthDentiCal);
			this.tabMisc.Controls.Add(this.textOrigRefNum);
			this.tabMisc.Controls.Add(this.labelCorrectionType);
			this.tabMisc.Controls.Add(this.labelPriorAuth);
			this.tabMisc.Controls.Add(this.labelOrigRefNum);
			this.tabMisc.Controls.Add(this.textPriorAuth);
			this.tabMisc.Controls.Add(this.labelSpecialProgram);
			this.tabMisc.Controls.Add(this.textClaimIdentifier);
			this.tabMisc.Controls.Add(this.comboSpecialProgram);
			this.tabMisc.Controls.Add(this.labelClaimIdentifier);
			this.tabMisc.Controls.Add(this.comboCorrectionType);
			this.tabMisc.Controls.Add(this.textShareOfCost);
			this.tabMisc.Location = new System.Drawing.Point(4, 22);
			this.tabMisc.Name = "tabMisc";
			this.tabMisc.Padding = new System.Windows.Forms.Padding(3);
			this.tabMisc.Size = new System.Drawing.Size(1140, 274);
			this.tabMisc.TabIndex = 4;
			this.tabMisc.Text = "Misc";
			this.tabMisc.UseVisualStyleBackColor = true;
			// 
			// textClaimIdOriginal
			// 
			this.textClaimIdOriginal.Location = new System.Drawing.Point(139, 65);
			this.textClaimIdOriginal.Name = "textClaimIdOriginal";
			this.textClaimIdOriginal.ReadOnly = true;
			this.textClaimIdOriginal.Size = new System.Drawing.Size(150, 20);
			this.textClaimIdOriginal.TabIndex = 166;
			// 
			// labelClaimIdOriginal
			// 
			this.labelClaimIdOriginal.Location = new System.Drawing.Point(6, 67);
			this.labelClaimIdOriginal.Name = "labelClaimIdOriginal";
			this.labelClaimIdOriginal.Size = new System.Drawing.Size(132, 17);
			this.labelClaimIdOriginal.TabIndex = 167;
			this.labelClaimIdOriginal.Text = "Default Claim Identifier";
			this.labelClaimIdOriginal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelShareOfCost
			// 
			this.labelShareOfCost.Location = new System.Drawing.Point(6, 127);
			this.labelShareOfCost.Name = "labelShareOfCost";
			this.labelShareOfCost.Size = new System.Drawing.Size(132, 17);
			this.labelShareOfCost.TabIndex = 164;
			this.labelShareOfCost.Text = "Share of Cost Amount";
			this.labelShareOfCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimIdentifierDentiCal
			// 
			this.labelClaimIdentifierDentiCal.Location = new System.Drawing.Point(290, 87);
			this.labelClaimIdentifierDentiCal.Name = "labelClaimIdentifierDentiCal";
			this.labelClaimIdentifierDentiCal.Size = new System.Drawing.Size(165, 17);
			this.labelClaimIdentifierDentiCal.TabIndex = 162;
			this.labelClaimIdentifierDentiCal.Text = "Denti-Cal PDCN";
			this.labelClaimIdentifierDentiCal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelOrigRefNumDentiCal
			// 
			this.labelOrigRefNumDentiCal.Location = new System.Drawing.Point(290, 107);
			this.labelOrigRefNumDentiCal.Name = "labelOrigRefNumDentiCal";
			this.labelOrigRefNumDentiCal.Size = new System.Drawing.Size(165, 17);
			this.labelOrigRefNumDentiCal.TabIndex = 161;
			this.labelOrigRefNumDentiCal.Text = "Denti-Cal Replacement DCN";
			this.labelOrigRefNumDentiCal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPriorAuthDentiCal
			// 
			this.labelPriorAuthDentiCal.Location = new System.Drawing.Point(290, 26);
			this.labelPriorAuthDentiCal.Name = "labelPriorAuthDentiCal";
			this.labelPriorAuthDentiCal.Size = new System.Drawing.Size(165, 17);
			this.labelPriorAuthDentiCal.TabIndex = 160;
			this.labelPriorAuthDentiCal.Text = "Denti-Cal NOA DCN";
			this.labelPriorAuthDentiCal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textOrigRefNum
			// 
			this.textOrigRefNum.Location = new System.Drawing.Point(139, 105);
			this.textOrigRefNum.Name = "textOrigRefNum";
			this.textOrigRefNum.Size = new System.Drawing.Size(150, 20);
			this.textOrigRefNum.TabIndex = 158;
			// 
			// labelCorrectionType
			// 
			this.labelCorrectionType.Location = new System.Drawing.Point(6, 5);
			this.labelCorrectionType.Name = "labelCorrectionType";
			this.labelCorrectionType.Size = new System.Drawing.Size(132, 17);
			this.labelCorrectionType.TabIndex = 154;
			this.labelCorrectionType.Text = "Correction Type";
			this.labelCorrectionType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPriorAuth
			// 
			this.labelPriorAuth.Location = new System.Drawing.Point(6, 26);
			this.labelPriorAuth.Name = "labelPriorAuth";
			this.labelPriorAuth.Size = new System.Drawing.Size(132, 17);
			this.labelPriorAuth.TabIndex = 142;
			this.labelPriorAuth.Text = "Prior Authorization (rare)";
			this.labelPriorAuth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOrigRefNum
			// 
			this.labelOrigRefNum.Location = new System.Drawing.Point(6, 107);
			this.labelOrigRefNum.Name = "labelOrigRefNum";
			this.labelOrigRefNum.Size = new System.Drawing.Size(132, 17);
			this.labelOrigRefNum.TabIndex = 159;
			this.labelOrigRefNum.Text = "Original Reference Num";
			this.labelOrigRefNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPriorAuth
			// 
			this.textPriorAuth.Location = new System.Drawing.Point(139, 24);
			this.textPriorAuth.Name = "textPriorAuth";
			this.textPriorAuth.Size = new System.Drawing.Size(150, 20);
			this.textPriorAuth.TabIndex = 141;
			// 
			// labelSpecialProgram
			// 
			this.labelSpecialProgram.Location = new System.Drawing.Point(6, 46);
			this.labelSpecialProgram.Name = "labelSpecialProgram";
			this.labelSpecialProgram.Size = new System.Drawing.Size(132, 17);
			this.labelSpecialProgram.TabIndex = 143;
			this.labelSpecialProgram.Text = "Special Program";
			this.labelSpecialProgram.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClaimIdentifier
			// 
			this.textClaimIdentifier.Location = new System.Drawing.Point(139, 85);
			this.textClaimIdentifier.Name = "textClaimIdentifier";
			this.textClaimIdentifier.Size = new System.Drawing.Size(150, 20);
			this.textClaimIdentifier.TabIndex = 156;
			// 
			// comboSpecialProgram
			// 
			this.comboSpecialProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSpecialProgram.Location = new System.Drawing.Point(139, 44);
			this.comboSpecialProgram.MaxDropDownItems = 100;
			this.comboSpecialProgram.Name = "comboSpecialProgram";
			this.comboSpecialProgram.Size = new System.Drawing.Size(150, 21);
			this.comboSpecialProgram.TabIndex = 144;
			// 
			// labelClaimIdentifier
			// 
			this.labelClaimIdentifier.Location = new System.Drawing.Point(6, 87);
			this.labelClaimIdentifier.Name = "labelClaimIdentifier";
			this.labelClaimIdentifier.Size = new System.Drawing.Size(132, 17);
			this.labelClaimIdentifier.TabIndex = 157;
			this.labelClaimIdentifier.Text = "Claim Identifier (CLM01)";
			this.labelClaimIdentifier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCorrectionType
			// 
			this.comboCorrectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCorrectionType.Location = new System.Drawing.Point(139, 3);
			this.comboCorrectionType.MaxDropDownItems = 100;
			this.comboCorrectionType.Name = "comboCorrectionType";
			this.comboCorrectionType.Size = new System.Drawing.Size(150, 21);
			this.comboCorrectionType.TabIndex = 155;
			// 
			// textShareOfCost
			// 
			this.textShareOfCost.Location = new System.Drawing.Point(139, 125);
			this.textShareOfCost.MaxVal = 100000000D;
			this.textShareOfCost.MinVal = 0D;
			this.textShareOfCost.Name = "textShareOfCost";
			this.textShareOfCost.Size = new System.Drawing.Size(150, 20);
			this.textShareOfCost.TabIndex = 165;
			// 
			// tabUB04
			// 
			this.tabUB04.AutoScroll = true;
			this.tabUB04.BackColor = System.Drawing.Color.Transparent;
			this.tabUB04.Controls.Add(this.groupBox4);
			this.tabUB04.Controls.Add(this.groupUb04);
			this.tabUB04.Controls.Add(this.groupDateIllnessInjuryPreg);
			this.tabUB04.Controls.Add(this.checkIsOutsideLab);
			this.tabUB04.Controls.Add(this.groupDateOtherCondOrTreatment);
			this.tabUB04.Location = new System.Drawing.Point(4, 22);
			this.tabUB04.Name = "tabUB04";
			this.tabUB04.Padding = new System.Windows.Forms.Padding(3);
			this.tabUB04.Size = new System.Drawing.Size(1140, 274);
			this.tabUB04.TabIndex = 0;
			this.tabUB04.Text = "Medical";
			this.tabUB04.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.butPickOrderProvInternal);
			this.groupBox4.Controls.Add(this.textOrderingProviderOverride);
			this.groupBox4.Controls.Add(this.butPickOrderProvReferral);
			this.groupBox4.Controls.Add(this.butNoneOrderProv);
			this.groupBox4.Location = new System.Drawing.Point(6, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(322, 65);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Ordering Provider Override";
			// 
			// butPickOrderProvInternal
			// 
			this.butPickOrderProvInternal.Location = new System.Drawing.Point(6, 15);
			this.butPickOrderProvInternal.Name = "butPickOrderProvInternal";
			this.butPickOrderProvInternal.Size = new System.Drawing.Size(58, 20);
			this.butPickOrderProvInternal.TabIndex = 1;
			this.butPickOrderProvInternal.Text = "Internal";
			this.butPickOrderProvInternal.Click += new System.EventHandler(this.butPickOrderProvInternal_Click);
			// 
			// textOrderingProviderOverride
			// 
			this.textOrderingProviderOverride.AcceptsTab = true;
			this.textOrderingProviderOverride.BackColor = System.Drawing.SystemColors.Control;
			this.textOrderingProviderOverride.DetectLinksEnabled = false;
			this.textOrderingProviderOverride.DetectUrls = false;
			this.textOrderingProviderOverride.Location = new System.Drawing.Point(6, 38);
			this.textOrderingProviderOverride.Name = "textOrderingProviderOverride";
			this.textOrderingProviderOverride.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textOrderingProviderOverride.ReadOnly = true;
			this.textOrderingProviderOverride.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textOrderingProviderOverride.Size = new System.Drawing.Size(310, 21);
			this.textOrderingProviderOverride.SpellCheckIsEnabled = false;
			this.textOrderingProviderOverride.TabIndex = 0;
			this.textOrderingProviderOverride.TabStop = false;
			this.textOrderingProviderOverride.Text = "";
			// 
			// butPickOrderProvReferral
			// 
			this.butPickOrderProvReferral.Location = new System.Drawing.Point(65, 15);
			this.butPickOrderProvReferral.Name = "butPickOrderProvReferral";
			this.butPickOrderProvReferral.Size = new System.Drawing.Size(58, 20);
			this.butPickOrderProvReferral.TabIndex = 2;
			this.butPickOrderProvReferral.Text = "Referral";
			this.butPickOrderProvReferral.Click += new System.EventHandler(this.butPickOrderProvReferral_Click);
			// 
			// butNoneOrderProv
			// 
			this.butNoneOrderProv.Location = new System.Drawing.Point(124, 15);
			this.butNoneOrderProv.Name = "butNoneOrderProv";
			this.butNoneOrderProv.Size = new System.Drawing.Size(58, 20);
			this.butNoneOrderProv.TabIndex = 3;
			this.butNoneOrderProv.Text = "None";
			this.butNoneOrderProv.Click += new System.EventHandler(this.butNoneOrderProv_Click);
			// 
			// groupUb04
			// 
			this.groupUb04.Controls.Add(this.labelBillType);
			this.groupUb04.Controls.Add(this.groupValueCodes);
			this.groupUb04.Controls.Add(this.groupBox1);
			this.groupUb04.Controls.Add(this.textBillType);
			this.groupUb04.Controls.Add(this.labelAdmissionType);
			this.groupUb04.Controls.Add(this.textPatientStatus);
			this.groupUb04.Controls.Add(this.textAdmissionType);
			this.groupUb04.Controls.Add(this.labelPatientStatus);
			this.groupUb04.Controls.Add(this.labelAdmissionSource);
			this.groupUb04.Controls.Add(this.textAdmissionSource);
			this.groupUb04.Location = new System.Drawing.Point(334, 3);
			this.groupUb04.Name = "groupUb04";
			this.groupUb04.Size = new System.Drawing.Size(614, 197);
			this.groupUb04.TabIndex = 5;
			this.groupUb04.TabStop = false;
			this.groupUb04.Text = "UB04";
			// 
			// labelBillType
			// 
			this.labelBillType.Location = new System.Drawing.Point(6, 14);
			this.labelBillType.Name = "labelBillType";
			this.labelBillType.Size = new System.Drawing.Size(155, 17);
			this.labelBillType.TabIndex = 0;
			this.labelBillType.Text = "Type of Bill (3 digit)";
			this.labelBillType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupValueCodes
			// 
			this.groupValueCodes.Controls.Add(this.textVC41dAmt);
			this.groupValueCodes.Controls.Add(this.textVC41cAmt);
			this.groupValueCodes.Controls.Add(this.textVC41bAmt);
			this.groupValueCodes.Controls.Add(this.textVC41aAmt);
			this.groupValueCodes.Controls.Add(this.textVC41dCode);
			this.groupValueCodes.Controls.Add(this.textVC41cCode);
			this.groupValueCodes.Controls.Add(this.textVC41bCode);
			this.groupValueCodes.Controls.Add(this.textVC41aCode);
			this.groupValueCodes.Controls.Add(this.labelVC41Amt);
			this.groupValueCodes.Controls.Add(this.labelVC41Code);
			this.groupValueCodes.Controls.Add(this.labelVC41d);
			this.groupValueCodes.Controls.Add(this.labelVC41c);
			this.groupValueCodes.Controls.Add(this.labelVC41b);
			this.groupValueCodes.Controls.Add(this.labelVC41a);
			this.groupValueCodes.Controls.Add(this.textVC40dAmt);
			this.groupValueCodes.Controls.Add(this.textVC40cAmt);
			this.groupValueCodes.Controls.Add(this.textVC40bAmt);
			this.groupValueCodes.Controls.Add(this.textVC40aAmt);
			this.groupValueCodes.Controls.Add(this.textVC40dCode);
			this.groupValueCodes.Controls.Add(this.textVC40cCode);
			this.groupValueCodes.Controls.Add(this.textVC40bCode);
			this.groupValueCodes.Controls.Add(this.textVC40aCode);
			this.groupValueCodes.Controls.Add(this.labelVC40Amt);
			this.groupValueCodes.Controls.Add(this.labelVC40Code);
			this.groupValueCodes.Controls.Add(this.labelVC40d);
			this.groupValueCodes.Controls.Add(this.labelVC40c);
			this.groupValueCodes.Controls.Add(this.labelVC40b);
			this.groupValueCodes.Controls.Add(this.labelVC40a);
			this.groupValueCodes.Controls.Add(this.labelVC41);
			this.groupValueCodes.Controls.Add(this.labelVC40);
			this.groupValueCodes.Controls.Add(this.labelVC39);
			this.groupValueCodes.Controls.Add(this.textVC39dAmt);
			this.groupValueCodes.Controls.Add(this.textVC39cAmt);
			this.groupValueCodes.Controls.Add(this.textVC39bAmt);
			this.groupValueCodes.Controls.Add(this.textVC39aAmt);
			this.groupValueCodes.Controls.Add(this.textVC39dCode);
			this.groupValueCodes.Controls.Add(this.textVC39cCode);
			this.groupValueCodes.Controls.Add(this.textVC39bCode);
			this.groupValueCodes.Controls.Add(this.textVC39aCode);
			this.groupValueCodes.Controls.Add(this.labelVC39Amt);
			this.groupValueCodes.Controls.Add(this.labelVC39Code);
			this.groupValueCodes.Controls.Add(this.labelVC39d);
			this.groupValueCodes.Controls.Add(this.labelVC39c);
			this.groupValueCodes.Controls.Add(this.labelVC39b);
			this.groupValueCodes.Controls.Add(this.labelVC39a);
			this.groupValueCodes.Location = new System.Drawing.Point(215, 74);
			this.groupValueCodes.Name = "groupValueCodes";
			this.groupValueCodes.Size = new System.Drawing.Size(393, 117);
			this.groupValueCodes.TabIndex = 6;
			this.groupValueCodes.TabStop = false;
			this.groupValueCodes.Text = "Value Codes";
			// 
			// textVC41dAmt
			// 
			this.textVC41dAmt.Location = new System.Drawing.Point(321, 91);
			this.textVC41dAmt.Name = "textVC41dAmt";
			this.textVC41dAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC41dAmt.TabIndex = 24;
			this.textVC41dAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC41cAmt
			// 
			this.textVC41cAmt.Location = new System.Drawing.Point(321, 72);
			this.textVC41cAmt.Name = "textVC41cAmt";
			this.textVC41cAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC41cAmt.TabIndex = 23;
			this.textVC41cAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC41bAmt
			// 
			this.textVC41bAmt.Location = new System.Drawing.Point(321, 53);
			this.textVC41bAmt.Name = "textVC41bAmt";
			this.textVC41bAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC41bAmt.TabIndex = 22;
			this.textVC41bAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC41aAmt
			// 
			this.textVC41aAmt.Location = new System.Drawing.Point(321, 34);
			this.textVC41aAmt.Name = "textVC41aAmt";
			this.textVC41aAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC41aAmt.TabIndex = 21;
			this.textVC41aAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC41dCode
			// 
			this.textVC41dCode.Location = new System.Drawing.Point(289, 91);
			this.textVC41dCode.MaxLength = 2;
			this.textVC41dCode.Name = "textVC41dCode";
			this.textVC41dCode.Size = new System.Drawing.Size(26, 20);
			this.textVC41dCode.TabIndex = 20;
			this.textVC41dCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC41cCode
			// 
			this.textVC41cCode.Location = new System.Drawing.Point(289, 72);
			this.textVC41cCode.MaxLength = 2;
			this.textVC41cCode.Name = "textVC41cCode";
			this.textVC41cCode.Size = new System.Drawing.Size(26, 20);
			this.textVC41cCode.TabIndex = 19;
			this.textVC41cCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC41bCode
			// 
			this.textVC41bCode.Location = new System.Drawing.Point(289, 53);
			this.textVC41bCode.MaxLength = 2;
			this.textVC41bCode.Name = "textVC41bCode";
			this.textVC41bCode.Size = new System.Drawing.Size(26, 20);
			this.textVC41bCode.TabIndex = 18;
			this.textVC41bCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC41aCode
			// 
			this.textVC41aCode.Location = new System.Drawing.Point(289, 34);
			this.textVC41aCode.MaxLength = 2;
			this.textVC41aCode.Name = "textVC41aCode";
			this.textVC41aCode.Size = new System.Drawing.Size(26, 20);
			this.textVC41aCode.TabIndex = 17;
			this.textVC41aCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelVC41Amt
			// 
			this.labelVC41Amt.Location = new System.Drawing.Point(321, 16);
			this.labelVC41Amt.Name = "labelVC41Amt";
			this.labelVC41Amt.Size = new System.Drawing.Size(66, 17);
			this.labelVC41Amt.TabIndex = 0;
			this.labelVC41Amt.Text = "amount";
			this.labelVC41Amt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41Code
			// 
			this.labelVC41Code.Location = new System.Drawing.Point(287, 16);
			this.labelVC41Code.Name = "labelVC41Code";
			this.labelVC41Code.Size = new System.Drawing.Size(31, 17);
			this.labelVC41Code.TabIndex = 0;
			this.labelVC41Code.Text = "code";
			this.labelVC41Code.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41d
			// 
			this.labelVC41d.Location = new System.Drawing.Point(264, 93);
			this.labelVC41d.Name = "labelVC41d";
			this.labelVC41d.Size = new System.Drawing.Size(19, 17);
			this.labelVC41d.TabIndex = 0;
			this.labelVC41d.Text = "d";
			this.labelVC41d.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41c
			// 
			this.labelVC41c.Location = new System.Drawing.Point(264, 74);
			this.labelVC41c.Name = "labelVC41c";
			this.labelVC41c.Size = new System.Drawing.Size(19, 17);
			this.labelVC41c.TabIndex = 0;
			this.labelVC41c.Text = "c";
			this.labelVC41c.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41b
			// 
			this.labelVC41b.Location = new System.Drawing.Point(264, 55);
			this.labelVC41b.Name = "labelVC41b";
			this.labelVC41b.Size = new System.Drawing.Size(19, 17);
			this.labelVC41b.TabIndex = 0;
			this.labelVC41b.Text = "b";
			this.labelVC41b.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41a
			// 
			this.labelVC41a.Location = new System.Drawing.Point(264, 36);
			this.labelVC41a.Name = "labelVC41a";
			this.labelVC41a.Size = new System.Drawing.Size(19, 17);
			this.labelVC41a.TabIndex = 0;
			this.labelVC41a.Text = "a";
			this.labelVC41a.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textVC40dAmt
			// 
			this.textVC40dAmt.Location = new System.Drawing.Point(192, 91);
			this.textVC40dAmt.Name = "textVC40dAmt";
			this.textVC40dAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC40dAmt.TabIndex = 16;
			this.textVC40dAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC40cAmt
			// 
			this.textVC40cAmt.Location = new System.Drawing.Point(192, 72);
			this.textVC40cAmt.Name = "textVC40cAmt";
			this.textVC40cAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC40cAmt.TabIndex = 15;
			this.textVC40cAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC40bAmt
			// 
			this.textVC40bAmt.Location = new System.Drawing.Point(192, 53);
			this.textVC40bAmt.Name = "textVC40bAmt";
			this.textVC40bAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC40bAmt.TabIndex = 14;
			this.textVC40bAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC40aAmt
			// 
			this.textVC40aAmt.Location = new System.Drawing.Point(192, 34);
			this.textVC40aAmt.Name = "textVC40aAmt";
			this.textVC40aAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC40aAmt.TabIndex = 13;
			this.textVC40aAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC40dCode
			// 
			this.textVC40dCode.Location = new System.Drawing.Point(160, 91);
			this.textVC40dCode.MaxLength = 2;
			this.textVC40dCode.Name = "textVC40dCode";
			this.textVC40dCode.Size = new System.Drawing.Size(26, 20);
			this.textVC40dCode.TabIndex = 12;
			this.textVC40dCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC40cCode
			// 
			this.textVC40cCode.Location = new System.Drawing.Point(160, 72);
			this.textVC40cCode.MaxLength = 2;
			this.textVC40cCode.Name = "textVC40cCode";
			this.textVC40cCode.Size = new System.Drawing.Size(26, 20);
			this.textVC40cCode.TabIndex = 11;
			this.textVC40cCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC40bCode
			// 
			this.textVC40bCode.Location = new System.Drawing.Point(160, 53);
			this.textVC40bCode.MaxLength = 2;
			this.textVC40bCode.Name = "textVC40bCode";
			this.textVC40bCode.Size = new System.Drawing.Size(26, 20);
			this.textVC40bCode.TabIndex = 10;
			this.textVC40bCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC40aCode
			// 
			this.textVC40aCode.Location = new System.Drawing.Point(160, 34);
			this.textVC40aCode.MaxLength = 2;
			this.textVC40aCode.Name = "textVC40aCode";
			this.textVC40aCode.Size = new System.Drawing.Size(26, 20);
			this.textVC40aCode.TabIndex = 9;
			this.textVC40aCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelVC40Amt
			// 
			this.labelVC40Amt.Location = new System.Drawing.Point(192, 16);
			this.labelVC40Amt.Name = "labelVC40Amt";
			this.labelVC40Amt.Size = new System.Drawing.Size(66, 17);
			this.labelVC40Amt.TabIndex = 0;
			this.labelVC40Amt.Text = "amount";
			this.labelVC40Amt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40Code
			// 
			this.labelVC40Code.Location = new System.Drawing.Point(158, 16);
			this.labelVC40Code.Name = "labelVC40Code";
			this.labelVC40Code.Size = new System.Drawing.Size(31, 17);
			this.labelVC40Code.TabIndex = 0;
			this.labelVC40Code.Text = "code";
			this.labelVC40Code.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40d
			// 
			this.labelVC40d.Location = new System.Drawing.Point(135, 93);
			this.labelVC40d.Name = "labelVC40d";
			this.labelVC40d.Size = new System.Drawing.Size(19, 17);
			this.labelVC40d.TabIndex = 0;
			this.labelVC40d.Text = "d";
			this.labelVC40d.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40c
			// 
			this.labelVC40c.Location = new System.Drawing.Point(135, 74);
			this.labelVC40c.Name = "labelVC40c";
			this.labelVC40c.Size = new System.Drawing.Size(19, 17);
			this.labelVC40c.TabIndex = 0;
			this.labelVC40c.Text = "c";
			this.labelVC40c.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40b
			// 
			this.labelVC40b.Location = new System.Drawing.Point(135, 55);
			this.labelVC40b.Name = "labelVC40b";
			this.labelVC40b.Size = new System.Drawing.Size(19, 17);
			this.labelVC40b.TabIndex = 0;
			this.labelVC40b.Text = "b";
			this.labelVC40b.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40a
			// 
			this.labelVC40a.Location = new System.Drawing.Point(135, 36);
			this.labelVC40a.Name = "labelVC40a";
			this.labelVC40a.Size = new System.Drawing.Size(19, 17);
			this.labelVC40a.TabIndex = 0;
			this.labelVC40a.Text = "a";
			this.labelVC40a.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC41
			// 
			this.labelVC41.Location = new System.Drawing.Point(264, 16);
			this.labelVC41.Name = "labelVC41";
			this.labelVC41.Size = new System.Drawing.Size(19, 17);
			this.labelVC41.TabIndex = 0;
			this.labelVC41.Text = "41";
			this.labelVC41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC40
			// 
			this.labelVC40.Location = new System.Drawing.Point(135, 16);
			this.labelVC40.Name = "labelVC40";
			this.labelVC40.Size = new System.Drawing.Size(19, 17);
			this.labelVC40.TabIndex = 0;
			this.labelVC40.Text = "40";
			this.labelVC40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39
			// 
			this.labelVC39.Location = new System.Drawing.Point(6, 16);
			this.labelVC39.Name = "labelVC39";
			this.labelVC39.Size = new System.Drawing.Size(19, 17);
			this.labelVC39.TabIndex = 0;
			this.labelVC39.Text = "39";
			this.labelVC39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textVC39dAmt
			// 
			this.textVC39dAmt.Location = new System.Drawing.Point(63, 91);
			this.textVC39dAmt.Name = "textVC39dAmt";
			this.textVC39dAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC39dAmt.TabIndex = 8;
			this.textVC39dAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC39cAmt
			// 
			this.textVC39cAmt.Location = new System.Drawing.Point(63, 72);
			this.textVC39cAmt.Name = "textVC39cAmt";
			this.textVC39cAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC39cAmt.TabIndex = 7;
			this.textVC39cAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC39bAmt
			// 
			this.textVC39bAmt.Location = new System.Drawing.Point(63, 53);
			this.textVC39bAmt.Name = "textVC39bAmt";
			this.textVC39bAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC39bAmt.TabIndex = 6;
			this.textVC39bAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC39aAmt
			// 
			this.textVC39aAmt.Location = new System.Drawing.Point(63, 34);
			this.textVC39aAmt.Name = "textVC39aAmt";
			this.textVC39aAmt.Size = new System.Drawing.Size(66, 20);
			this.textVC39aAmt.TabIndex = 5;
			this.textVC39aAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textVC39dCode
			// 
			this.textVC39dCode.Location = new System.Drawing.Point(31, 91);
			this.textVC39dCode.MaxLength = 2;
			this.textVC39dCode.Name = "textVC39dCode";
			this.textVC39dCode.Size = new System.Drawing.Size(26, 20);
			this.textVC39dCode.TabIndex = 4;
			this.textVC39dCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC39cCode
			// 
			this.textVC39cCode.Location = new System.Drawing.Point(31, 72);
			this.textVC39cCode.MaxLength = 2;
			this.textVC39cCode.Name = "textVC39cCode";
			this.textVC39cCode.Size = new System.Drawing.Size(26, 20);
			this.textVC39cCode.TabIndex = 3;
			this.textVC39cCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC39bCode
			// 
			this.textVC39bCode.Location = new System.Drawing.Point(31, 53);
			this.textVC39bCode.MaxLength = 2;
			this.textVC39bCode.Name = "textVC39bCode";
			this.textVC39bCode.Size = new System.Drawing.Size(26, 20);
			this.textVC39bCode.TabIndex = 2;
			this.textVC39bCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textVC39aCode
			// 
			this.textVC39aCode.Location = new System.Drawing.Point(31, 34);
			this.textVC39aCode.MaxLength = 2;
			this.textVC39aCode.Name = "textVC39aCode";
			this.textVC39aCode.Size = new System.Drawing.Size(26, 20);
			this.textVC39aCode.TabIndex = 1;
			this.textVC39aCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelVC39Amt
			// 
			this.labelVC39Amt.Location = new System.Drawing.Point(63, 16);
			this.labelVC39Amt.Name = "labelVC39Amt";
			this.labelVC39Amt.Size = new System.Drawing.Size(66, 17);
			this.labelVC39Amt.TabIndex = 0;
			this.labelVC39Amt.Text = "amount";
			this.labelVC39Amt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39Code
			// 
			this.labelVC39Code.Location = new System.Drawing.Point(29, 16);
			this.labelVC39Code.Name = "labelVC39Code";
			this.labelVC39Code.Size = new System.Drawing.Size(31, 17);
			this.labelVC39Code.TabIndex = 0;
			this.labelVC39Code.Text = "code";
			this.labelVC39Code.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39d
			// 
			this.labelVC39d.Location = new System.Drawing.Point(6, 93);
			this.labelVC39d.Name = "labelVC39d";
			this.labelVC39d.Size = new System.Drawing.Size(19, 17);
			this.labelVC39d.TabIndex = 0;
			this.labelVC39d.Text = "d";
			this.labelVC39d.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39c
			// 
			this.labelVC39c.Location = new System.Drawing.Point(6, 74);
			this.labelVC39c.Name = "labelVC39c";
			this.labelVC39c.Size = new System.Drawing.Size(19, 17);
			this.labelVC39c.TabIndex = 0;
			this.labelVC39c.Text = "c";
			this.labelVC39c.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39b
			// 
			this.labelVC39b.Location = new System.Drawing.Point(6, 55);
			this.labelVC39b.Name = "labelVC39b";
			this.labelVC39b.Size = new System.Drawing.Size(19, 17);
			this.labelVC39b.TabIndex = 0;
			this.labelVC39b.Text = "b";
			this.labelVC39b.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVC39a
			// 
			this.labelVC39a.Location = new System.Drawing.Point(6, 36);
			this.labelVC39a.Name = "labelVC39a";
			this.labelVC39a.Size = new System.Drawing.Size(19, 17);
			this.labelVC39a.TabIndex = 0;
			this.labelVC39a.Text = "a";
			this.labelVC39a.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelCode10);
			this.groupBox1.Controls.Add(this.labelCode9);
			this.groupBox1.Controls.Add(this.labelCode8);
			this.groupBox1.Controls.Add(this.labelCode7);
			this.groupBox1.Controls.Add(this.labelCode6);
			this.groupBox1.Controls.Add(this.labelCode5);
			this.groupBox1.Controls.Add(this.labelCode4);
			this.groupBox1.Controls.Add(this.labelCode3);
			this.groupBox1.Controls.Add(this.labelCode2);
			this.groupBox1.Controls.Add(this.labelCode1);
			this.groupBox1.Controls.Add(this.labelCode0);
			this.groupBox1.Controls.Add(this.textCode10);
			this.groupBox1.Controls.Add(this.textCode9);
			this.groupBox1.Controls.Add(this.textCode8);
			this.groupBox1.Controls.Add(this.textCode7);
			this.groupBox1.Controls.Add(this.textCode6);
			this.groupBox1.Controls.Add(this.textCode5);
			this.groupBox1.Controls.Add(this.textCode4);
			this.groupBox1.Controls.Add(this.textCode3);
			this.groupBox1.Controls.Add(this.textCode2);
			this.groupBox1.Controls.Add(this.textCode1);
			this.groupBox1.Controls.Add(this.textCode0);
			this.groupBox1.Location = new System.Drawing.Point(215, 11);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(393, 60);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Condition Codes";
			// 
			// labelCode10
			// 
			this.labelCode10.Location = new System.Drawing.Point(326, 16);
			this.labelCode10.Name = "labelCode10";
			this.labelCode10.Size = new System.Drawing.Size(26, 17);
			this.labelCode10.TabIndex = 0;
			this.labelCode10.Text = "28";
			this.labelCode10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode9
			// 
			this.labelCode9.Location = new System.Drawing.Point(294, 16);
			this.labelCode9.Name = "labelCode9";
			this.labelCode9.Size = new System.Drawing.Size(26, 17);
			this.labelCode9.TabIndex = 0;
			this.labelCode9.Text = "27";
			this.labelCode9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode8
			// 
			this.labelCode8.Location = new System.Drawing.Point(262, 16);
			this.labelCode8.Name = "labelCode8";
			this.labelCode8.Size = new System.Drawing.Size(26, 17);
			this.labelCode8.TabIndex = 0;
			this.labelCode8.Text = "26";
			this.labelCode8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode7
			// 
			this.labelCode7.Location = new System.Drawing.Point(230, 16);
			this.labelCode7.Name = "labelCode7";
			this.labelCode7.Size = new System.Drawing.Size(26, 17);
			this.labelCode7.TabIndex = 0;
			this.labelCode7.Text = "25";
			this.labelCode7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode6
			// 
			this.labelCode6.Location = new System.Drawing.Point(198, 16);
			this.labelCode6.Name = "labelCode6";
			this.labelCode6.Size = new System.Drawing.Size(26, 17);
			this.labelCode6.TabIndex = 0;
			this.labelCode6.Text = "24";
			this.labelCode6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode5
			// 
			this.labelCode5.Location = new System.Drawing.Point(167, 16);
			this.labelCode5.Name = "labelCode5";
			this.labelCode5.Size = new System.Drawing.Size(26, 17);
			this.labelCode5.TabIndex = 0;
			this.labelCode5.Text = "23";
			this.labelCode5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode4
			// 
			this.labelCode4.Location = new System.Drawing.Point(134, 16);
			this.labelCode4.Name = "labelCode4";
			this.labelCode4.Size = new System.Drawing.Size(26, 17);
			this.labelCode4.TabIndex = 0;
			this.labelCode4.Text = "22";
			this.labelCode4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode3
			// 
			this.labelCode3.Location = new System.Drawing.Point(102, 16);
			this.labelCode3.Name = "labelCode3";
			this.labelCode3.Size = new System.Drawing.Size(26, 17);
			this.labelCode3.TabIndex = 0;
			this.labelCode3.Text = "21";
			this.labelCode3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode2
			// 
			this.labelCode2.Location = new System.Drawing.Point(70, 16);
			this.labelCode2.Name = "labelCode2";
			this.labelCode2.Size = new System.Drawing.Size(26, 17);
			this.labelCode2.TabIndex = 0;
			this.labelCode2.Text = "20";
			this.labelCode2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode1
			// 
			this.labelCode1.Location = new System.Drawing.Point(38, 16);
			this.labelCode1.Name = "labelCode1";
			this.labelCode1.Size = new System.Drawing.Size(26, 17);
			this.labelCode1.TabIndex = 0;
			this.labelCode1.Text = "19";
			this.labelCode1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCode0
			// 
			this.labelCode0.Location = new System.Drawing.Point(6, 16);
			this.labelCode0.Name = "labelCode0";
			this.labelCode0.Size = new System.Drawing.Size(26, 17);
			this.labelCode0.TabIndex = 0;
			this.labelCode0.Text = "18";
			this.labelCode0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textCode10
			// 
			this.textCode10.Location = new System.Drawing.Point(326, 34);
			this.textCode10.MaxLength = 2;
			this.textCode10.Name = "textCode10";
			this.textCode10.Size = new System.Drawing.Size(26, 20);
			this.textCode10.TabIndex = 11;
			this.textCode10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode9
			// 
			this.textCode9.Location = new System.Drawing.Point(294, 34);
			this.textCode9.MaxLength = 2;
			this.textCode9.Name = "textCode9";
			this.textCode9.Size = new System.Drawing.Size(26, 20);
			this.textCode9.TabIndex = 10;
			this.textCode9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode8
			// 
			this.textCode8.Location = new System.Drawing.Point(262, 34);
			this.textCode8.MaxLength = 2;
			this.textCode8.Name = "textCode8";
			this.textCode8.Size = new System.Drawing.Size(26, 20);
			this.textCode8.TabIndex = 9;
			this.textCode8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode7
			// 
			this.textCode7.Location = new System.Drawing.Point(230, 34);
			this.textCode7.MaxLength = 2;
			this.textCode7.Name = "textCode7";
			this.textCode7.Size = new System.Drawing.Size(26, 20);
			this.textCode7.TabIndex = 8;
			this.textCode7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode6
			// 
			this.textCode6.Location = new System.Drawing.Point(198, 34);
			this.textCode6.MaxLength = 2;
			this.textCode6.Name = "textCode6";
			this.textCode6.Size = new System.Drawing.Size(26, 20);
			this.textCode6.TabIndex = 7;
			this.textCode6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode5
			// 
			this.textCode5.Location = new System.Drawing.Point(167, 34);
			this.textCode5.MaxLength = 2;
			this.textCode5.Name = "textCode5";
			this.textCode5.Size = new System.Drawing.Size(26, 20);
			this.textCode5.TabIndex = 6;
			this.textCode5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode4
			// 
			this.textCode4.Location = new System.Drawing.Point(134, 34);
			this.textCode4.MaxLength = 2;
			this.textCode4.Name = "textCode4";
			this.textCode4.Size = new System.Drawing.Size(26, 20);
			this.textCode4.TabIndex = 5;
			this.textCode4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode3
			// 
			this.textCode3.Location = new System.Drawing.Point(102, 34);
			this.textCode3.MaxLength = 2;
			this.textCode3.Name = "textCode3";
			this.textCode3.Size = new System.Drawing.Size(26, 20);
			this.textCode3.TabIndex = 4;
			this.textCode3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode2
			// 
			this.textCode2.Location = new System.Drawing.Point(70, 34);
			this.textCode2.MaxLength = 2;
			this.textCode2.Name = "textCode2";
			this.textCode2.Size = new System.Drawing.Size(26, 20);
			this.textCode2.TabIndex = 3;
			this.textCode2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode1
			// 
			this.textCode1.Location = new System.Drawing.Point(38, 34);
			this.textCode1.MaxLength = 2;
			this.textCode1.Name = "textCode1";
			this.textCode1.Size = new System.Drawing.Size(26, 20);
			this.textCode1.TabIndex = 2;
			this.textCode1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textCode0
			// 
			this.textCode0.Location = new System.Drawing.Point(6, 34);
			this.textCode0.MaxLength = 2;
			this.textCode0.Name = "textCode0";
			this.textCode0.Size = new System.Drawing.Size(26, 20);
			this.textCode0.TabIndex = 1;
			this.textCode0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textBillType
			// 
			this.textBillType.Location = new System.Drawing.Point(162, 12);
			this.textBillType.Name = "textBillType";
			this.textBillType.Size = new System.Drawing.Size(47, 20);
			this.textBillType.TabIndex = 1;
			// 
			// labelAdmissionType
			// 
			this.labelAdmissionType.Location = new System.Drawing.Point(6, 35);
			this.labelAdmissionType.Name = "labelAdmissionType";
			this.labelAdmissionType.Size = new System.Drawing.Size(155, 17);
			this.labelAdmissionType.TabIndex = 0;
			this.labelAdmissionType.Text = "Admission Type (1 digit)";
			this.labelAdmissionType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientStatus
			// 
			this.textPatientStatus.Location = new System.Drawing.Point(162, 75);
			this.textPatientStatus.Name = "textPatientStatus";
			this.textPatientStatus.Size = new System.Drawing.Size(47, 20);
			this.textPatientStatus.TabIndex = 4;
			// 
			// textAdmissionType
			// 
			this.textAdmissionType.Location = new System.Drawing.Point(162, 33);
			this.textAdmissionType.Name = "textAdmissionType";
			this.textAdmissionType.Size = new System.Drawing.Size(47, 20);
			this.textAdmissionType.TabIndex = 2;
			// 
			// labelPatientStatus
			// 
			this.labelPatientStatus.Location = new System.Drawing.Point(6, 77);
			this.labelPatientStatus.Name = "labelPatientStatus";
			this.labelPatientStatus.Size = new System.Drawing.Size(155, 17);
			this.labelPatientStatus.TabIndex = 0;
			this.labelPatientStatus.Text = "Patient Status (2 digit)";
			this.labelPatientStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAdmissionSource
			// 
			this.labelAdmissionSource.Location = new System.Drawing.Point(6, 56);
			this.labelAdmissionSource.Name = "labelAdmissionSource";
			this.labelAdmissionSource.Size = new System.Drawing.Size(155, 17);
			this.labelAdmissionSource.TabIndex = 0;
			this.labelAdmissionSource.Text = "Admission Source (1 char)";
			this.labelAdmissionSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAdmissionSource
			// 
			this.textAdmissionSource.Location = new System.Drawing.Point(162, 54);
			this.textAdmissionSource.Name = "textAdmissionSource";
			this.textAdmissionSource.Size = new System.Drawing.Size(47, 20);
			this.textAdmissionSource.TabIndex = 3;
			// 
			// groupDateIllnessInjuryPreg
			// 
			this.groupDateIllnessInjuryPreg.Controls.Add(this.comboDateIllnessQualifier);
			this.groupDateIllnessInjuryPreg.Controls.Add(this.textDateIllness);
			this.groupDateIllnessInjuryPreg.Controls.Add(this.labelDateIllness);
			this.groupDateIllnessInjuryPreg.Controls.Add(this.labelDateIllnessQualifier);
			this.groupDateIllnessInjuryPreg.Location = new System.Drawing.Point(6, 71);
			this.groupDateIllnessInjuryPreg.Name = "groupDateIllnessInjuryPreg";
			this.groupDateIllnessInjuryPreg.Size = new System.Drawing.Size(322, 63);
			this.groupDateIllnessInjuryPreg.TabIndex = 2;
			this.groupDateIllnessInjuryPreg.TabStop = false;
			this.groupDateIllnessInjuryPreg.Text = "Current Illness, Injury, or Pregnancy (LMP)";
			// 
			// comboDateIllnessQualifier
			// 
			this.comboDateIllnessQualifier.Location = new System.Drawing.Point(91, 36);
			this.comboDateIllnessQualifier.Name = "comboDateIllnessQualifier";
			this.comboDateIllnessQualifier.Size = new System.Drawing.Size(225, 21);
			this.comboDateIllnessQualifier.TabIndex = 2;
			// 
			// textDateIllness
			// 
			this.textDateIllness.Location = new System.Drawing.Point(91, 16);
			this.textDateIllness.Name = "textDateIllness";
			this.textDateIllness.Size = new System.Drawing.Size(66, 20);
			this.textDateIllness.TabIndex = 1;
			// 
			// labelDateIllness
			// 
			this.labelDateIllness.Location = new System.Drawing.Point(6, 18);
			this.labelDateIllness.Name = "labelDateIllness";
			this.labelDateIllness.Size = new System.Drawing.Size(84, 17);
			this.labelDateIllness.TabIndex = 0;
			this.labelDateIllness.Text = "Date";
			this.labelDateIllness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateIllnessQualifier
			// 
			this.labelDateIllnessQualifier.Location = new System.Drawing.Point(6, 38);
			this.labelDateIllnessQualifier.Name = "labelDateIllnessQualifier";
			this.labelDateIllnessQualifier.Size = new System.Drawing.Size(84, 17);
			this.labelDateIllnessQualifier.TabIndex = 0;
			this.labelDateIllnessQualifier.Text = "Date Qualifier";
			this.labelDateIllnessQualifier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsOutsideLab
			// 
			this.checkIsOutsideLab.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsOutsideLab.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsOutsideLab.Location = new System.Drawing.Point(6, 203);
			this.checkIsOutsideLab.Name = "checkIsOutsideLab";
			this.checkIsOutsideLab.Size = new System.Drawing.Size(104, 17);
			this.checkIsOutsideLab.TabIndex = 4;
			this.checkIsOutsideLab.Text = "Is Outside Lab";
			this.checkIsOutsideLab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupDateOtherCondOrTreatment
			// 
			this.groupDateOtherCondOrTreatment.Controls.Add(this.textDateOther);
			this.groupDateOtherCondOrTreatment.Controls.Add(this.labelDateOtherQualifier);
			this.groupDateOtherCondOrTreatment.Controls.Add(this.labelDateOther);
			this.groupDateOtherCondOrTreatment.Controls.Add(this.comboDateOtherQualifier);
			this.groupDateOtherCondOrTreatment.Location = new System.Drawing.Point(6, 137);
			this.groupDateOtherCondOrTreatment.Name = "groupDateOtherCondOrTreatment";
			this.groupDateOtherCondOrTreatment.Size = new System.Drawing.Size(322, 63);
			this.groupDateOtherCondOrTreatment.TabIndex = 3;
			this.groupDateOtherCondOrTreatment.TabStop = false;
			this.groupDateOtherCondOrTreatment.Text = "Other Condition or Treatment";
			// 
			// textDateOther
			// 
			this.textDateOther.Location = new System.Drawing.Point(91, 16);
			this.textDateOther.Name = "textDateOther";
			this.textDateOther.Size = new System.Drawing.Size(66, 20);
			this.textDateOther.TabIndex = 1;
			// 
			// labelDateOtherQualifier
			// 
			this.labelDateOtherQualifier.Location = new System.Drawing.Point(6, 38);
			this.labelDateOtherQualifier.Name = "labelDateOtherQualifier";
			this.labelDateOtherQualifier.Size = new System.Drawing.Size(84, 17);
			this.labelDateOtherQualifier.TabIndex = 0;
			this.labelDateOtherQualifier.Text = "Date Qualifier";
			this.labelDateOtherQualifier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateOther
			// 
			this.labelDateOther.Location = new System.Drawing.Point(6, 18);
			this.labelDateOther.Name = "labelDateOther";
			this.labelDateOther.Size = new System.Drawing.Size(84, 17);
			this.labelDateOther.TabIndex = 0;
			this.labelDateOther.Text = "Date";
			this.labelDateOther.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDateOtherQualifier
			// 
			this.comboDateOtherQualifier.Location = new System.Drawing.Point(91, 36);
			this.comboDateOtherQualifier.Name = "comboDateOtherQualifier";
			this.comboDateOtherQualifier.Size = new System.Drawing.Size(225, 21);
			this.comboDateOtherQualifier.TabIndex = 2;
			// 
			// tabCanadian
			// 
			this.tabCanadian.AutoScroll = true;
			this.tabCanadian.Controls.Add(this.textCanadaTransRefNum);
			this.tabCanadian.Controls.Add(this.groupCanadaOrthoPredeterm);
			this.tabCanadian.Controls.Add(this.label76);
			this.tabCanadian.Controls.Add(this.butReverse);
			this.tabCanadian.Controls.Add(this.textMissingTeeth);
			this.tabCanadian.Controls.Add(this.labelCanadaMissingTeeth);
			this.tabCanadian.Controls.Add(this.labelExtractedTeeth);
			this.tabCanadian.Controls.Add(this.listExtractedTeeth);
			this.tabCanadian.Controls.Add(this.checkCanadianIsOrtho);
			this.tabCanadian.Controls.Add(this.label43);
			this.tabCanadian.Controls.Add(this.butMissingTeethHelp);
			this.tabCanadian.Controls.Add(this.groupMandPros);
			this.tabCanadian.Controls.Add(this.groupMaxPros);
			this.tabCanadian.Controls.Add(this.groupBox8);
			this.tabCanadian.Controls.Add(this.groupBox9);
			this.tabCanadian.Controls.Add(this.textCanadianAccidentDate);
			this.tabCanadian.Location = new System.Drawing.Point(4, 22);
			this.tabCanadian.Name = "tabCanadian";
			this.tabCanadian.Size = new System.Drawing.Size(1140, 274);
			this.tabCanadian.TabIndex = 3;
			this.tabCanadian.Text = "Canadian";
			this.tabCanadian.UseVisualStyleBackColor = true;
			// 
			// textCanadaTransRefNum
			// 
			this.textCanadaTransRefNum.Location = new System.Drawing.Point(96, 122);
			this.textCanadaTransRefNum.Name = "textCanadaTransRefNum";
			this.textCanadaTransRefNum.ReadOnly = true;
			this.textCanadaTransRefNum.Size = new System.Drawing.Size(100, 20);
			this.textCanadaTransRefNum.TabIndex = 148;
			// 
			// groupCanadaOrthoPredeterm
			// 
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textCanadaExpectedPayCycle);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textCanadaAnticipatedPayAmount);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label82);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textCanadaNumPaymentsAnticipated);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label81);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textCanadaTreatDuration);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label80);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label79);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textCanadaInitialPayment);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label78);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.label77);
			this.groupCanadaOrthoPredeterm.Controls.Add(this.textDateCanadaEstTreatStartDate);
			this.groupCanadaOrthoPredeterm.Enabled = false;
			this.groupCanadaOrthoPredeterm.Location = new System.Drawing.Point(7, 180);
			this.groupCanadaOrthoPredeterm.Name = "groupCanadaOrthoPredeterm";
			this.groupCanadaOrthoPredeterm.Size = new System.Drawing.Size(550, 90);
			this.groupCanadaOrthoPredeterm.TabIndex = 147;
			this.groupCanadaOrthoPredeterm.TabStop = false;
			this.groupCanadaOrthoPredeterm.Text = "Ortho Treatment (Predetermination Only)";
			// 
			// textCanadaExpectedPayCycle
			// 
			this.textCanadaExpectedPayCycle.Location = new System.Drawing.Point(196, 66);
			this.textCanadaExpectedPayCycle.Name = "textCanadaExpectedPayCycle";
			this.textCanadaExpectedPayCycle.Size = new System.Drawing.Size(75, 20);
			this.textCanadaExpectedPayCycle.TabIndex = 158;
			// 
			// textCanadaAnticipatedPayAmount
			// 
			this.textCanadaAnticipatedPayAmount.Location = new System.Drawing.Point(466, 66);
			this.textCanadaAnticipatedPayAmount.Name = "textCanadaAnticipatedPayAmount";
			this.textCanadaAnticipatedPayAmount.Size = new System.Drawing.Size(75, 20);
			this.textCanadaAnticipatedPayAmount.TabIndex = 157;
			// 
			// label82
			// 
			this.label82.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label82.Location = new System.Drawing.Point(280, 65);
			this.label82.Name = "label82";
			this.label82.Size = new System.Drawing.Size(180, 20);
			this.label82.TabIndex = 156;
			this.label82.Text = "Anticipated Pay Amount";
			this.label82.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCanadaNumPaymentsAnticipated
			// 
			this.textCanadaNumPaymentsAnticipated.Location = new System.Drawing.Point(466, 42);
			this.textCanadaNumPaymentsAnticipated.Name = "textCanadaNumPaymentsAnticipated";
			this.textCanadaNumPaymentsAnticipated.Size = new System.Drawing.Size(75, 20);
			this.textCanadaNumPaymentsAnticipated.TabIndex = 155;
			// 
			// label81
			// 
			this.label81.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label81.Location = new System.Drawing.Point(277, 43);
			this.label81.Name = "label81";
			this.label81.Size = new System.Drawing.Size(183, 20);
			this.label81.TabIndex = 154;
			this.label81.Text = "Number of Payments Anticipated";
			this.label81.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCanadaTreatDuration
			// 
			this.textCanadaTreatDuration.Location = new System.Drawing.Point(466, 19);
			this.textCanadaTreatDuration.Name = "textCanadaTreatDuration";
			this.textCanadaTreatDuration.Size = new System.Drawing.Size(75, 20);
			this.textCanadaTreatDuration.TabIndex = 153;
			// 
			// label80
			// 
			this.label80.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label80.Location = new System.Drawing.Point(280, 19);
			this.label80.Name = "label80";
			this.label80.Size = new System.Drawing.Size(180, 20);
			this.label80.TabIndex = 152;
			this.label80.Text = "Treatment Duration (Months)";
			this.label80.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label79
			// 
			this.label79.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label79.Location = new System.Drawing.Point(8, 65);
			this.label79.Name = "label79";
			this.label79.Size = new System.Drawing.Size(182, 20);
			this.label79.TabIndex = 150;
			this.label79.Text = "Expected Payment Cycle (Months)";
			this.label79.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCanadaInitialPayment
			// 
			this.textCanadaInitialPayment.Location = new System.Drawing.Point(196, 43);
			this.textCanadaInitialPayment.Name = "textCanadaInitialPayment";
			this.textCanadaInitialPayment.Size = new System.Drawing.Size(75, 20);
			this.textCanadaInitialPayment.TabIndex = 149;
			// 
			// label78
			// 
			this.label78.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label78.Location = new System.Drawing.Point(8, 42);
			this.label78.Name = "label78";
			this.label78.Size = new System.Drawing.Size(174, 20);
			this.label78.TabIndex = 141;
			this.label78.Text = "Initial Payment";
			this.label78.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label77
			// 
			this.label77.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label77.Location = new System.Drawing.Point(5, 20);
			this.label77.Name = "label77";
			this.label77.Size = new System.Drawing.Size(177, 20);
			this.label77.TabIndex = 140;
			this.label77.Text = "Estimated Treatment Start Date";
			this.label77.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateCanadaEstTreatStartDate
			// 
			this.textDateCanadaEstTreatStartDate.Location = new System.Drawing.Point(196, 20);
			this.textDateCanadaEstTreatStartDate.Name = "textDateCanadaEstTreatStartDate";
			this.textDateCanadaEstTreatStartDate.Size = new System.Drawing.Size(75, 20);
			this.textDateCanadaEstTreatStartDate.TabIndex = 139;
			// 
			// label76
			// 
			this.label76.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label76.Location = new System.Drawing.Point(9, 122);
			this.label76.Name = "label76";
			this.label76.Size = new System.Drawing.Size(82, 16);
			this.label76.TabIndex = 146;
			this.label76.Text = "Trans Ref Num";
			this.label76.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butReverse
			// 
			this.butReverse.Enabled = false;
			this.butReverse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReverse.Location = new System.Drawing.Point(202, 120);
			this.butReverse.Name = "butReverse";
			this.butReverse.Size = new System.Drawing.Size(59, 24);
			this.butReverse.TabIndex = 138;
			this.butReverse.Text = "Reverse";
			this.butReverse.Click += new System.EventHandler(this.butReverse_Click);
			// 
			// textMissingTeeth
			// 
			this.textMissingTeeth.Location = new System.Drawing.Point(764, 142);
			this.textMissingTeeth.Multiline = true;
			this.textMissingTeeth.Name = "textMissingTeeth";
			this.textMissingTeeth.Size = new System.Drawing.Size(172, 44);
			this.textMissingTeeth.TabIndex = 144;
			// 
			// labelCanadaMissingTeeth
			// 
			this.labelCanadaMissingTeeth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelCanadaMissingTeeth.Location = new System.Drawing.Point(762, 122);
			this.labelCanadaMissingTeeth.Name = "labelCanadaMissingTeeth";
			this.labelCanadaMissingTeeth.Size = new System.Drawing.Size(83, 17);
			this.labelCanadaMissingTeeth.TabIndex = 143;
			this.labelCanadaMissingTeeth.Text = "Missing Teeth";
			this.labelCanadaMissingTeeth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelExtractedTeeth
			// 
			this.labelExtractedTeeth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelExtractedTeeth.Location = new System.Drawing.Point(762, 5);
			this.labelExtractedTeeth.Name = "labelExtractedTeeth";
			this.labelExtractedTeeth.Size = new System.Drawing.Size(143, 17);
			this.labelExtractedTeeth.TabIndex = 142;
			this.labelExtractedTeeth.Text = "Extracted Teeth";
			this.labelExtractedTeeth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listExtractedTeeth
			// 
			this.listExtractedTeeth.Location = new System.Drawing.Point(764, 24);
			this.listExtractedTeeth.Name = "listExtractedTeeth";
			this.listExtractedTeeth.Size = new System.Drawing.Size(172, 95);
			this.listExtractedTeeth.TabIndex = 141;
			// 
			// checkCanadianIsOrtho
			// 
			this.checkCanadianIsOrtho.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCanadianIsOrtho.Location = new System.Drawing.Point(36, 156);
			this.checkCanadianIsOrtho.Name = "checkCanadianIsOrtho";
			this.checkCanadianIsOrtho.Size = new System.Drawing.Size(216, 17);
			this.checkCanadianIsOrtho.TabIndex = 140;
			this.checkCanadianIsOrtho.Text = "Treatment Required for Ortho";
			// 
			// label43
			// 
			this.label43.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label43.Location = new System.Drawing.Point(25, 85);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(117, 17);
			this.label43.TabIndex = 139;
			this.label43.Text = "Accident Date";
			this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butMissingTeethHelp
			// 
			this.butMissingTeethHelp.Location = new System.Drawing.Point(911, 0);
			this.butMissingTeethHelp.Name = "butMissingTeethHelp";
			this.butMissingTeethHelp.Size = new System.Drawing.Size(25, 24);
			this.butMissingTeethHelp.TabIndex = 137;
			this.butMissingTeethHelp.Text = "?";
			this.butMissingTeethHelp.Click += new System.EventHandler(this.butMissingTeethHelp_Click);
			// 
			// groupMandPros
			// 
			this.groupMandPros.Controls.Add(this.comboMandProsth);
			this.groupMandPros.Controls.Add(this.label66);
			this.groupMandPros.Controls.Add(this.textDateInitialLower);
			this.groupMandPros.Controls.Add(this.label67);
			this.groupMandPros.Controls.Add(this.comboMandProsthMaterial);
			this.groupMandPros.Controls.Add(this.label68);
			this.groupMandPros.Location = new System.Drawing.Point(402, 94);
			this.groupMandPros.Name = "groupMandPros";
			this.groupMandPros.Size = new System.Drawing.Size(355, 86);
			this.groupMandPros.TabIndex = 13;
			this.groupMandPros.TabStop = false;
			this.groupMandPros.Text = "Mandibular Prosthesis";
			// 
			// comboMandProsth
			// 
			this.comboMandProsth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMandProsth.FormattingEnabled = true;
			this.comboMandProsth.Location = new System.Drawing.Point(136, 11);
			this.comboMandProsth.Name = "comboMandProsth";
			this.comboMandProsth.Size = new System.Drawing.Size(213, 21);
			this.comboMandProsth.TabIndex = 14;
			this.comboMandProsth.SelectionChangeCommitted += new System.EventHandler(this.comboMandProsth_SelectionChangeCommitted);
			// 
			// label66
			// 
			this.label66.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label66.Location = new System.Drawing.Point(7, 14);
			this.label66.Name = "label66";
			this.label66.Size = new System.Drawing.Size(128, 17);
			this.label66.TabIndex = 1;
			this.label66.Text = "Initial placement lower?";
			this.label66.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateInitialLower
			// 
			this.textDateInitialLower.Location = new System.Drawing.Point(136, 35);
			this.textDateInitialLower.Name = "textDateInitialLower";
			this.textDateInitialLower.Size = new System.Drawing.Size(83, 20);
			this.textDateInitialLower.TabIndex = 2;
			// 
			// label67
			// 
			this.label67.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label67.Location = new System.Drawing.Point(72, 37);
			this.label67.Name = "label67";
			this.label67.Size = new System.Drawing.Size(61, 17);
			this.label67.TabIndex = 3;
			this.label67.Text = "Initial Date";
			this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboMandProsthMaterial
			// 
			this.comboMandProsthMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMandProsthMaterial.FormattingEnabled = true;
			this.comboMandProsthMaterial.Location = new System.Drawing.Point(136, 58);
			this.comboMandProsthMaterial.Name = "comboMandProsthMaterial";
			this.comboMandProsthMaterial.Size = new System.Drawing.Size(213, 21);
			this.comboMandProsthMaterial.TabIndex = 4;
			// 
			// label68
			// 
			this.label68.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label68.Location = new System.Drawing.Point(10, 59);
			this.label68.Name = "label68";
			this.label68.Size = new System.Drawing.Size(125, 18);
			this.label68.TabIndex = 7;
			this.label68.Text = "Prosthesis Material";
			this.label68.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupMaxPros
			// 
			this.groupMaxPros.Controls.Add(this.comboMaxProsth);
			this.groupMaxPros.Controls.Add(this.label69);
			this.groupMaxPros.Controls.Add(this.textDateInitialUpper);
			this.groupMaxPros.Controls.Add(this.label70);
			this.groupMaxPros.Controls.Add(this.comboMaxProsthMaterial);
			this.groupMaxPros.Controls.Add(this.label71);
			this.groupMaxPros.Location = new System.Drawing.Point(402, 5);
			this.groupMaxPros.Name = "groupMaxPros";
			this.groupMaxPros.Size = new System.Drawing.Size(355, 86);
			this.groupMaxPros.TabIndex = 12;
			this.groupMaxPros.TabStop = false;
			this.groupMaxPros.Text = "Maxillary Prosthesis";
			// 
			// comboMaxProsth
			// 
			this.comboMaxProsth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMaxProsth.FormattingEnabled = true;
			this.comboMaxProsth.Location = new System.Drawing.Point(128, 11);
			this.comboMaxProsth.Name = "comboMaxProsth";
			this.comboMaxProsth.Size = new System.Drawing.Size(221, 21);
			this.comboMaxProsth.TabIndex = 14;
			this.comboMaxProsth.SelectionChangeCommitted += new System.EventHandler(this.comboMaxProsth_SelectionChangeCommitted);
			// 
			// label69
			// 
			this.label69.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label69.Location = new System.Drawing.Point(2, 14);
			this.label69.Name = "label69";
			this.label69.Size = new System.Drawing.Size(125, 17);
			this.label69.TabIndex = 1;
			this.label69.Text = "Initial placement upper?";
			this.label69.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateInitialUpper
			// 
			this.textDateInitialUpper.Location = new System.Drawing.Point(128, 35);
			this.textDateInitialUpper.Name = "textDateInitialUpper";
			this.textDateInitialUpper.Size = new System.Drawing.Size(83, 20);
			this.textDateInitialUpper.TabIndex = 2;
			// 
			// label70
			// 
			this.label70.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label70.Location = new System.Drawing.Point(64, 37);
			this.label70.Name = "label70";
			this.label70.Size = new System.Drawing.Size(61, 17);
			this.label70.TabIndex = 3;
			this.label70.Text = "Initial Date";
			this.label70.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboMaxProsthMaterial
			// 
			this.comboMaxProsthMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMaxProsthMaterial.FormattingEnabled = true;
			this.comboMaxProsthMaterial.Location = new System.Drawing.Point(128, 58);
			this.comboMaxProsthMaterial.Name = "comboMaxProsthMaterial";
			this.comboMaxProsthMaterial.Size = new System.Drawing.Size(221, 21);
			this.comboMaxProsthMaterial.TabIndex = 4;
			// 
			// label71
			// 
			this.label71.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label71.Location = new System.Drawing.Point(2, 59);
			this.label71.Name = "label71";
			this.label71.Size = new System.Drawing.Size(125, 18);
			this.label71.TabIndex = 7;
			this.label71.Text = "Prosthesis Material";
			this.label71.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.checkImages);
			this.groupBox8.Controls.Add(this.checkXrays);
			this.groupBox8.Controls.Add(this.checkModels);
			this.groupBox8.Controls.Add(this.checkCorrespondence);
			this.groupBox8.Controls.Add(this.checkEmail);
			this.groupBox8.Location = new System.Drawing.Point(265, 75);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(128, 106);
			this.groupBox8.TabIndex = 10;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Materials Forwarded";
			// 
			// checkImages
			// 
			this.checkImages.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkImages.Location = new System.Drawing.Point(11, 84);
			this.checkImages.Name = "checkImages";
			this.checkImages.Size = new System.Drawing.Size(110, 17);
			this.checkImages.TabIndex = 4;
			this.checkImages.Text = "Images";
			this.checkImages.UseVisualStyleBackColor = true;
			// 
			// checkXrays
			// 
			this.checkXrays.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkXrays.Location = new System.Drawing.Point(11, 67);
			this.checkXrays.Name = "checkXrays";
			this.checkXrays.Size = new System.Drawing.Size(110, 17);
			this.checkXrays.TabIndex = 3;
			this.checkXrays.Text = "X-rays";
			this.checkXrays.UseVisualStyleBackColor = true;
			// 
			// checkModels
			// 
			this.checkModels.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkModels.Location = new System.Drawing.Point(11, 50);
			this.checkModels.Name = "checkModels";
			this.checkModels.Size = new System.Drawing.Size(110, 17);
			this.checkModels.TabIndex = 2;
			this.checkModels.Text = "Models";
			this.checkModels.UseVisualStyleBackColor = true;
			// 
			// checkCorrespondence
			// 
			this.checkCorrespondence.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkCorrespondence.Location = new System.Drawing.Point(11, 33);
			this.checkCorrespondence.Name = "checkCorrespondence";
			this.checkCorrespondence.Size = new System.Drawing.Size(110, 17);
			this.checkCorrespondence.TabIndex = 1;
			this.checkCorrespondence.Text = "Correspondence";
			this.checkCorrespondence.UseVisualStyleBackColor = true;
			// 
			// checkEmail
			// 
			this.checkEmail.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkEmail.Location = new System.Drawing.Point(11, 16);
			this.checkEmail.Name = "checkEmail";
			this.checkEmail.Size = new System.Drawing.Size(110, 17);
			this.checkEmail.TabIndex = 0;
			this.checkEmail.Text = "E-Mail";
			this.checkEmail.UseVisualStyleBackColor = true;
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.label73);
			this.groupBox9.Controls.Add(this.label72);
			this.groupBox9.Controls.Add(this.comboReferralReason);
			this.groupBox9.Controls.Add(this.textReferralProvider);
			this.groupBox9.Location = new System.Drawing.Point(7, 5);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(386, 70);
			this.groupBox9.TabIndex = 11;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Referring Provider";
			// 
			// label73
			// 
			this.label73.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label73.Location = new System.Drawing.Point(17, 14);
			this.label73.Name = "label73";
			this.label73.Size = new System.Drawing.Size(87, 30);
			this.label73.TabIndex = 2;
			this.label73.Text = "CDA Number\r\nor Identifier";
			this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label72
			// 
			this.label72.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label72.Location = new System.Drawing.Point(14, 44);
			this.label72.Name = "label72";
			this.label72.Size = new System.Drawing.Size(90, 18);
			this.label72.TabIndex = 4;
			this.label72.Text = "Reason";
			this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReferralReason
			// 
			this.comboReferralReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReferralReason.FormattingEnabled = true;
			this.comboReferralReason.Location = new System.Drawing.Point(105, 44);
			this.comboReferralReason.Name = "comboReferralReason";
			this.comboReferralReason.Size = new System.Drawing.Size(273, 21);
			this.comboReferralReason.TabIndex = 1;
			// 
			// textReferralProvider
			// 
			this.textReferralProvider.Location = new System.Drawing.Point(105, 20);
			this.textReferralProvider.Name = "textReferralProvider";
			this.textReferralProvider.Size = new System.Drawing.Size(100, 20);
			this.textReferralProvider.TabIndex = 0;
			// 
			// textCanadianAccidentDate
			// 
			this.textCanadianAccidentDate.Location = new System.Drawing.Point(144, 84);
			this.textCanadianAccidentDate.Name = "textCanadianAccidentDate";
			this.textCanadianAccidentDate.Size = new System.Drawing.Size(75, 20);
			this.textCanadianAccidentDate.TabIndex = 138;
			// 
			// tabHistory
			// 
			this.tabHistory.AutoScroll = true;
			this.tabHistory.Controls.Add(this.butAdd);
			this.tabHistory.Controls.Add(this.gridStatusHistory);
			this.tabHistory.Location = new System.Drawing.Point(4, 22);
			this.tabHistory.Name = "tabHistory";
			this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
			this.tabHistory.Size = new System.Drawing.Size(1140, 274);
			this.tabHistory.TabIndex = 5;
			this.tabHistory.Text = "Status History";
			this.tabHistory.UseVisualStyleBackColor = true;
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(6, 6);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(84, 24);
			this.butAdd.TabIndex = 157;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridStatusHistory
			// 
			this.gridStatusHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridStatusHistory.Location = new System.Drawing.Point(6, 33);
			this.gridStatusHistory.Name = "gridStatusHistory";
			this.gridStatusHistory.Size = new System.Drawing.Size(1128, 233);
			this.gridStatusHistory.TabIndex = 156;
			this.gridStatusHistory.Title = "Claim Custom Tracking Status History";
			this.gridStatusHistory.TranslationName = "TableStatusHistory";
			this.gridStatusHistory.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridStatusHistory_CellDoubleClick);
			// 
			// butViewEra
			// 
			this.butViewEra.Location = new System.Drawing.Point(2, 364);
			this.butViewEra.Name = "butViewEra";
			this.butViewEra.Size = new System.Drawing.Size(94, 24);
			this.butViewEra.TabIndex = 263;
			this.butViewEra.Text = "View ERA";
			this.butViewEra.Click += new System.EventHandler(this.butViewEra_Click);
			// 
			// butPickProvTreat
			// 
			this.butPickProvTreat.Location = new System.Drawing.Point(501, 95);
			this.butPickProvTreat.Name = "butPickProvTreat";
			this.butPickProvTreat.Size = new System.Drawing.Size(20, 21);
			this.butPickProvTreat.TabIndex = 262;
			this.butPickProvTreat.Text = "...";
			this.butPickProvTreat.Click += new System.EventHandler(this.butPickProvTreat_Click);
			// 
			// butPickProvBill
			// 
			this.butPickProvBill.Location = new System.Drawing.Point(501, 74);
			this.butPickProvBill.Name = "butPickProvBill";
			this.butPickProvBill.Size = new System.Drawing.Size(20, 21);
			this.butPickProvBill.TabIndex = 261;
			this.butPickProvBill.Text = "...";
			this.butPickProvBill.Click += new System.EventHandler(this.butPickProvBill_Click);
			// 
			// butResend
			// 
			this.butResend.Location = new System.Drawing.Point(195, 97);
			this.butResend.Name = "butResend";
			this.butResend.Size = new System.Drawing.Size(51, 20);
			this.butResend.TabIndex = 154;
			this.butResend.Text = "Resend";
			this.butResend.Click += new System.EventHandler(this.butResend_Click);
			// 
			// textDateSent
			// 
			this.textDateSent.Location = new System.Drawing.Point(111, 97);
			this.textDateSent.Name = "textDateSent";
			this.textDateSent.Size = new System.Drawing.Size(82, 20);
			this.textDateSent.TabIndex = 152;
			// 
			// labelDateSent
			// 
			this.labelDateSent.Location = new System.Drawing.Point(5, 100);
			this.labelDateSent.Name = "labelDateSent";
			this.labelDateSent.Size = new System.Drawing.Size(104, 16);
			this.labelDateSent.TabIndex = 153;
			this.labelDateSent.Text = "Date Sent";
			this.labelDateSent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboClaimForm
			// 
			this.comboClaimForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimForm.Location = new System.Drawing.Point(351, 53);
			this.comboClaimForm.MaxDropDownItems = 100;
			this.comboClaimForm.Name = "comboClaimForm";
			this.comboClaimForm.Size = new System.Drawing.Size(126, 21);
			this.comboClaimForm.TabIndex = 148;
			// 
			// label87
			// 
			this.label87.Location = new System.Drawing.Point(254, 56);
			this.label87.Name = "label87";
			this.label87.Size = new System.Drawing.Size(95, 17);
			this.label87.TabIndex = 147;
			this.label87.Text = "ClaimForm";
			this.label87.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboMedType
			// 
			this.comboMedType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMedType.Location = new System.Drawing.Point(351, 32);
			this.comboMedType.MaxDropDownItems = 100;
			this.comboMedType.Name = "comboMedType";
			this.comboMedType.Size = new System.Drawing.Size(126, 21);
			this.comboMedType.TabIndex = 146;
			// 
			// label86
			// 
			this.label86.Location = new System.Drawing.Point(254, 35);
			this.label86.Name = "label86";
			this.label86.Size = new System.Drawing.Size(95, 17);
			this.label86.TabIndex = 145;
			this.label86.Text = "Med/Dent";
			this.label86.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboClaimType
			// 
			this.comboClaimType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimType.Enabled = false;
			this.comboClaimType.Location = new System.Drawing.Point(111, 36);
			this.comboClaimType.MaxDropDownItems = 100;
			this.comboClaimType.Name = "comboClaimType";
			this.comboClaimType.Size = new System.Drawing.Size(109, 21);
			this.comboClaimType.TabIndex = 140;
			// 
			// butHistory
			// 
			this.butHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butHistory.Location = new System.Drawing.Point(595, 665);
			this.butHistory.Name = "butHistory";
			this.butHistory.Size = new System.Drawing.Size(86, 24);
			this.butHistory.TabIndex = 136;
			this.butHistory.Text = "History";
			this.butHistory.Click += new System.EventHandler(this.butHistory_Click);
			// 
			// textReasonUnder
			// 
			this.textReasonUnder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textReasonUnder.Location = new System.Drawing.Point(753, 418);
			this.textReasonUnder.MaxLength = 255;
			this.textReasonUnder.Multiline = true;
			this.textReasonUnder.Name = "textReasonUnder";
			this.textReasonUnder.Size = new System.Drawing.Size(397, 57);
			this.textReasonUnder.TabIndex = 130;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(752, 389);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(213, 26);
			this.label4.TabIndex = 131;
			this.label4.Text = "Reasons underpaid:  (shows on patient bill)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridPay
			// 
			this.gridPay.Location = new System.Drawing.Point(2, 389);
			this.gridPay.Name = "gridPay";
			this.gridPay.Size = new System.Drawing.Size(569, 86);
			this.gridPay.TabIndex = 135;
			this.gridPay.Title = "Insurance Payments";
			this.gridPay.TranslationName = "TableClaimPay";
			this.gridPay.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPay_CellDoubleClick);
			this.gridPay.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPay_CellClick);
			// 
			// groupEnterPayment
			// 
			this.groupEnterPayment.BackColor = System.Drawing.SystemColors.Control;
			this.groupEnterPayment.Controls.Add(this.butPaySupp);
			this.groupEnterPayment.Controls.Add(this.butPayTotal);
			this.groupEnterPayment.Controls.Add(this.butPayProc);
			this.groupEnterPayment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupEnterPayment.Location = new System.Drawing.Point(809, 12);
			this.groupEnterPayment.Name = "groupEnterPayment";
			this.groupEnterPayment.Size = new System.Drawing.Size(133, 107);
			this.groupEnterPayment.TabIndex = 132;
			this.groupEnterPayment.TabStop = false;
			this.groupEnterPayment.Text = "Enter Payment";
			// 
			// butPaySupp
			// 
			this.butPaySupp.Location = new System.Drawing.Point(17, 78);
			this.butPaySupp.Name = "butPaySupp";
			this.butPaySupp.Size = new System.Drawing.Size(99, 24);
			this.butPaySupp.TabIndex = 102;
			this.butPaySupp.Text = "S&upplemental";
			this.butPaySupp.Click += new System.EventHandler(this.butPaySupp_Click);
			// 
			// butPayTotal
			// 
			this.butPayTotal.Location = new System.Drawing.Point(17, 16);
			this.butPayTotal.Name = "butPayTotal";
			this.butPayTotal.Size = new System.Drawing.Size(99, 24);
			this.butPayTotal.TabIndex = 100;
			this.butPayTotal.Text = "As &Total";
			this.butPayTotal.Click += new System.EventHandler(this.butPayTotal_Click);
			// 
			// butPayProc
			// 
			this.butPayProc.Location = new System.Drawing.Point(17, 42);
			this.butPayProc.Name = "butPayProc";
			this.butPayProc.Size = new System.Drawing.Size(99, 24);
			this.butPayProc.TabIndex = 101;
			this.butPayProc.Text = "&By Procedure";
			this.butPayProc.Click += new System.EventHandler(this.butPayProc_Click);
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label20.Location = new System.Drawing.Point(894, 644);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(215, 18);
			this.label20.TabIndex = 92;
			this.label20.Text = "(does not cancel payment edits)";
			this.label20.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSend.Image = ((System.Drawing.Image)(resources.GetObject("butSend.Image")));
			this.butSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSend.Location = new System.Drawing.Point(503, 665);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(86, 24);
			this.butSend.TabIndex = 130;
			this.butSend.Text = "Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// gridProc
			// 
			this.gridProc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProc.Location = new System.Drawing.Point(2, 159);
			this.gridProc.MinimumSize = new System.Drawing.Size(1149, 200);
			this.gridProc.Name = "gridProc";
			this.gridProc.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProc.Size = new System.Drawing.Size(1149, 200);
			this.gridProc.TabIndex = 128;
			this.gridProc.Title = "Procedures";
			this.gridProc.TranslationName = "TableClaimProc";
			this.gridProc.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellDoubleClick);
			this.gridProc.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellClick);
			// 
			// butSplit
			// 
			this.butSplit.Location = new System.Drawing.Point(826, 128);
			this.butSplit.Name = "butSplit";
			this.butSplit.Size = new System.Drawing.Size(99, 24);
			this.butSplit.TabIndex = 127;
			this.butSplit.Text = "Split Claim";
			this.butSplit.Click += new System.EventHandler(this.butSplit_Click);
			// 
			// butLabel
			// 
			this.butLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabel.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabel.Location = new System.Drawing.Point(163, 665);
			this.butLabel.Name = "butLabel";
			this.butLabel.Size = new System.Drawing.Size(81, 24);
			this.butLabel.TabIndex = 126;
			this.butLabel.Text = "Label";
			this.butLabel.Click += new System.EventHandler(this.butLabel_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Enabled = false;
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(314, 11);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(207, 21);
			this.comboClinic.TabIndex = 121;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// textDateService
			// 
			this.textDateService.Location = new System.Drawing.Point(111, 57);
			this.textDateService.Name = "textDateService";
			this.textDateService.Size = new System.Drawing.Size(82, 20);
			this.textDateService.TabIndex = 119;
			// 
			// textWriteOff
			// 
			this.textWriteOff.Location = new System.Drawing.Point(684, 363);
			this.textWriteOff.MaxVal = 100000000D;
			this.textWriteOff.MinVal = -100000000D;
			this.textWriteOff.Name = "textWriteOff";
			this.textWriteOff.ReadOnly = true;
			this.textWriteOff.Size = new System.Drawing.Size(63, 20);
			this.textWriteOff.TabIndex = 113;
			this.textWriteOff.TabStop = false;
			this.textWriteOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textInsPayEst
			// 
			this.textInsPayEst.Location = new System.Drawing.Point(560, 363);
			this.textInsPayEst.Name = "textInsPayEst";
			this.textInsPayEst.ReadOnly = true;
			this.textInsPayEst.Size = new System.Drawing.Size(63, 20);
			this.textInsPayEst.TabIndex = 40;
			this.textInsPayEst.TabStop = false;
			this.textInsPayEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textInsPayAmt
			// 
			this.textInsPayAmt.Location = new System.Drawing.Point(622, 363);
			this.textInsPayAmt.MaxVal = 100000000D;
			this.textInsPayAmt.MinVal = -100000000D;
			this.textInsPayAmt.Name = "textInsPayAmt";
			this.textInsPayAmt.ReadOnly = true;
			this.textInsPayAmt.Size = new System.Drawing.Size(63, 20);
			this.textInsPayAmt.TabIndex = 6;
			this.textInsPayAmt.TabStop = false;
			this.textInsPayAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textClaimFee
			// 
			this.textClaimFee.Location = new System.Drawing.Point(436, 363);
			this.textClaimFee.Name = "textClaimFee";
			this.textClaimFee.ReadOnly = true;
			this.textClaimFee.Size = new System.Drawing.Size(63, 20);
			this.textClaimFee.TabIndex = 51;
			this.textClaimFee.TabStop = false;
			this.textClaimFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textDedApplied
			// 
			this.textDedApplied.Location = new System.Drawing.Point(498, 363);
			this.textDedApplied.MaxVal = 100000000D;
			this.textDedApplied.MinVal = -100000000D;
			this.textDedApplied.Name = "textDedApplied";
			this.textDedApplied.ReadOnly = true;
			this.textDedApplied.Size = new System.Drawing.Size(63, 20);
			this.textDedApplied.TabIndex = 4;
			this.textDedApplied.TabStop = false;
			this.textDedApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPredeterm
			// 
			this.textPredeterm.Location = new System.Drawing.Point(351, 116);
			this.textPredeterm.Name = "textPredeterm";
			this.textPredeterm.Size = new System.Drawing.Size(170, 20);
			this.textPredeterm.TabIndex = 1;
			// 
			// textDateSentOrig
			// 
			this.textDateSentOrig.Location = new System.Drawing.Point(111, 77);
			this.textDateSentOrig.Name = "textDateSentOrig";
			this.textDateSentOrig.ReadOnly = true;
			this.textDateSentOrig.Size = new System.Drawing.Size(82, 20);
			this.textDateSentOrig.TabIndex = 6;
			// 
			// textDateRec
			// 
			this.textDateRec.Location = new System.Drawing.Point(111, 117);
			this.textDateRec.Name = "textDateRec";
			this.textDateRec.Size = new System.Drawing.Size(82, 20);
			this.textDateRec.TabIndex = 7;
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPreview.Location = new System.Drawing.Point(250, 665);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(92, 24);
			this.butPreview.TabIndex = 115;
			this.butPreview.Text = "P&review";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(347, 665);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(86, 24);
			this.butPrint.TabIndex = 114;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.ButPrint_Click);
			// 
			// butRecalc
			// 
			this.butRecalc.Location = new System.Drawing.Point(824, 361);
			this.butRecalc.Name = "butRecalc";
			this.butRecalc.Size = new System.Drawing.Size(148, 24);
			this.butRecalc.TabIndex = 112;
			this.butRecalc.Text = "Recalculate &Estimates";
			this.butRecalc.Click += new System.EventHandler(this.butRecalc_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(5, 665);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(91, 24);
			this.butDelete.TabIndex = 106;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1037, 665);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 15;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(956, 665);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 14;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.butOtherNone);
			this.groupBox3.Controls.Add(this.butOtherCovChange);
			this.groupBox3.Controls.Add(this.comboPatRelat2);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.textPlan2);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(531, 73);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(269, 85);
			this.groupBox3.TabIndex = 111;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Other Coverage";
			// 
			// butOtherNone
			// 
			this.butOtherNone.Location = new System.Drawing.Point(196, 9);
			this.butOtherNone.Name = "butOtherNone";
			this.butOtherNone.Size = new System.Drawing.Size(65, 22);
			this.butOtherNone.TabIndex = 5;
			this.butOtherNone.Text = "None";
			this.butOtherNone.Click += new System.EventHandler(this.butOtherNone_Click);
			// 
			// butOtherCovChange
			// 
			this.butOtherCovChange.Location = new System.Drawing.Point(129, 9);
			this.butOtherCovChange.Name = "butOtherCovChange";
			this.butOtherCovChange.Size = new System.Drawing.Size(65, 22);
			this.butOtherCovChange.TabIndex = 4;
			this.butOtherCovChange.Text = "Change";
			this.butOtherCovChange.Click += new System.EventHandler(this.butOtherCovChange_Click);
			// 
			// comboPatRelat2
			// 
			this.comboPatRelat2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPatRelat2.Location = new System.Drawing.Point(90, 57);
			this.comboPatRelat2.Name = "comboPatRelat2";
			this.comboPatRelat2.Size = new System.Drawing.Size(173, 21);
			this.comboPatRelat2.TabIndex = 3;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 60);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(84, 17);
			this.label10.TabIndex = 2;
			this.label10.Text = "Relationship";
			// 
			// textPlan2
			// 
			this.textPlan2.Location = new System.Drawing.Point(8, 34);
			this.textPlan2.Name = "textPlan2";
			this.textPlan2.ReadOnly = true;
			this.textPlan2.Size = new System.Drawing.Size(253, 20);
			this.textPlan2.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboPatRelat);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textPlan);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(531, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(269, 70);
			this.groupBox2.TabIndex = 110;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Insurance Plan";
			// 
			// comboPatRelat
			// 
			this.comboPatRelat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPatRelat.Location = new System.Drawing.Point(90, 43);
			this.comboPatRelat.Name = "comboPatRelat";
			this.comboPatRelat.Size = new System.Drawing.Size(171, 21);
			this.comboPatRelat.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "Relationship";
			// 
			// textPlan
			// 
			this.textPlan.Location = new System.Drawing.Point(8, 20);
			this.textPlan.Name = "textPlan";
			this.textPlan.ReadOnly = true;
			this.textPlan.Size = new System.Drawing.Size(253, 20);
			this.textPlan.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(14, 39);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(95, 17);
			this.label9.TabIndex = 109;
			this.label9.Text = "Claim Type";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 14);
			this.label2.TabIndex = 104;
			this.label2.Text = "Claim Status";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboProvTreat
			// 
			this.comboProvTreat.Location = new System.Drawing.Point(351, 95);
			this.comboProvTreat.Name = "comboProvTreat";
			this.comboProvTreat.Size = new System.Drawing.Size(150, 21);
			this.comboProvTreat.TabIndex = 99;
			// 
			// comboProvBill
			// 
			this.comboProvBill.Location = new System.Drawing.Point(351, 74);
			this.comboProvBill.Name = "comboProvBill";
			this.comboProvBill.Size = new System.Drawing.Size(150, 21);
			this.comboProvBill.TabIndex = 97;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(249, 98);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(102, 15);
			this.label21.TabIndex = 93;
			this.label21.Text = "Treating Provider";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelPredeterm
			// 
			this.labelPredeterm.Location = new System.Drawing.Point(214, 120);
			this.labelPredeterm.Name = "labelPredeterm";
			this.labelPredeterm.Size = new System.Drawing.Size(138, 16);
			this.labelPredeterm.TabIndex = 11;
			this.labelPredeterm.Text = "Predeterm Benefits";
			this.labelPredeterm.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDateService
			// 
			this.labelDateService.Location = new System.Drawing.Point(3, 61);
			this.labelDateService.Name = "labelDateService";
			this.labelDateService.Size = new System.Drawing.Size(107, 16);
			this.labelDateService.TabIndex = 8;
			this.labelDateService.Text = "Date of Service";
			this.labelDateService.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(256, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(94, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Billing Provider";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(318, 366);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 15);
			this.label1.TabIndex = 50;
			this.label1.Text = "Totals";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(2, 120);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(108, 16);
			this.label6.TabIndex = 5;
			this.label6.Text = "Date Received";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 80);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(104, 16);
			this.label8.TabIndex = 7;
			this.label8.Text = "Date Orig Sent";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _recalcErrorProvider
			// 
			this._recalcErrorProvider.ContainerControl = this;
			// 
			// comboClaimStatus
			// 
			this.comboClaimStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimStatus.FormattingEnabled = true;
			this.comboClaimStatus.Location = new System.Drawing.Point(111, 15);
			this.comboClaimStatus.Name = "comboClaimStatus";
			this.comboClaimStatus.Size = new System.Drawing.Size(150, 21);
			this.comboClaimStatus.TabIndex = 267;
			this.comboClaimStatus.SelectionChangeCommitted += new System.EventHandler(this.comboClaimStatus_SelectionChangeCommitted);
			// 
			// textPatResp
			// 
			this.textPatResp.Location = new System.Drawing.Point(746, 363);
			this.textPatResp.MaxVal = 100000000D;
			this.textPatResp.MinVal = -100000000D;
			this.textPatResp.Name = "textPatResp";
			this.textPatResp.ReadOnly = true;
			this.textPatResp.Size = new System.Drawing.Size(63, 20);
			this.textPatResp.TabIndex = 268;
			this.textPatResp.TabStop = false;
			this.textPatResp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// FormClaimEdit
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1156, 696);
			this.Controls.Add(this.textPatResp);
			this.Controls.Add(this.comboClaimStatus);
			this.Controls.Add(this.butViewEob);
			this.Controls.Add(this.groupFinalizePayment);
			this.Controls.Add(this.tabMain);
			this.Controls.Add(this.butViewEra);
			this.Controls.Add(this.butPickProvTreat);
			this.Controls.Add(this.butPickProvBill);
			this.Controls.Add(this.butResend);
			this.Controls.Add(this.textDateSent);
			this.Controls.Add(this.labelDateSent);
			this.Controls.Add(this.comboClaimForm);
			this.Controls.Add(this.label87);
			this.Controls.Add(this.comboMedType);
			this.Controls.Add(this.label86);
			this.Controls.Add(this.comboClaimType);
			this.Controls.Add(this.butHistory);
			this.Controls.Add(this.textReasonUnder);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.gridPay);
			this.Controls.Add(this.groupEnterPayment);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.gridProc);
			this.Controls.Add(this.butSplit);
			this.Controls.Add(this.butLabel);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.textDateService);
			this.Controls.Add(this.textWriteOff);
			this.Controls.Add(this.textInsPayEst);
			this.Controls.Add(this.textInsPayAmt);
			this.Controls.Add(this.textClaimFee);
			this.Controls.Add(this.textDedApplied);
			this.Controls.Add(this.textPredeterm);
			this.Controls.Add(this.textDateSentOrig);
			this.Controls.Add(this.textDateRec);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butRecalc);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboProvTreat);
			this.Controls.Add(this.comboProvBill);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.labelPredeterm);
			this.Controls.Add(this.labelDateService);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label8);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Claim";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormClaimEdit_CloseXClicked);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClaimEdit_Closing);
			this.Load += new System.EventHandler(this.FormClaimEdit_Load);
			this.Shown += new System.EventHandler(this.FormClaimEdit_Shown);
			this.LocationChanged += new System.EventHandler(this.FormClaimEdit_LocationChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormClaimEdit_Paint);
			this.groupFinalizePayment.ResumeLayout(false);
			this.tabMain.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.groupAccident.ResumeLayout(false);
			this.groupAccident.PerformLayout();
			this.groupReferral.ResumeLayout(false);
			this.groupReferral.PerformLayout();
			this.groupProsth.ResumeLayout(false);
			this.groupProsth.PerformLayout();
			this.groupOrtho.ResumeLayout(false);
			this.groupOrtho.PerformLayout();
			this.tabAttachments.ResumeLayout(false);
			this.tabAttach.ResumeLayout(false);
			this.tabNEA.ResumeLayout(false);
			this.groupAttachments.ResumeLayout(false);
			this.groupAttachments.PerformLayout();
			this.groupAttachedImages.ResumeLayout(false);
			this.tabDXC.ResumeLayout(false);
			this.tabDXC.PerformLayout();
			this.tabMisc.ResumeLayout(false);
			this.tabMisc.PerformLayout();
			this.tabUB04.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupUb04.ResumeLayout(false);
			this.groupUb04.PerformLayout();
			this.groupValueCodes.ResumeLayout(false);
			this.groupValueCodes.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupDateIllnessInjuryPreg.ResumeLayout(false);
			this.groupDateIllnessInjuryPreg.PerformLayout();
			this.groupDateOtherCondOrTreatment.ResumeLayout(false);
			this.groupDateOtherCondOrTreatment.PerformLayout();
			this.tabCanadian.ResumeLayout(false);
			this.tabCanadian.PerformLayout();
			this.groupCanadaOrthoPredeterm.ResumeLayout(false);
			this.groupCanadaOrthoPredeterm.PerformLayout();
			this.groupMandPros.ResumeLayout(false);
			this.groupMandPros.PerformLayout();
			this.groupMaxPros.ResumeLayout(false);
			this.groupMaxPros.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox9.PerformLayout();
			this.tabHistory.ResumeLayout(false);
			this.groupEnterPayment.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._recalcErrorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labelBillType;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelVC39a;
		private System.Windows.Forms.Label labelVC39b;
		private System.Windows.Forms.Label labelVC39c;
		private System.Windows.Forms.Label labelVC39d;
		private System.Windows.Forms.Label labelVC39Code;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label labelVC40Amt;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label labelVC40Code;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label labelVC39Amt;
		private System.Windows.Forms.Label labelVC40d;
		private System.Windows.Forms.Label labelVC40c;
		private System.Windows.Forms.Label labelVC39;
		private System.Windows.Forms.Label labelVC40;
		private System.Windows.Forms.Label labelVC41;
		private System.Windows.Forms.Label labelVC40b;
		private System.Windows.Forms.Label labelVC40a;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label labelVC41Amt;
		private System.Windows.Forms.Label labelVC41Code;
		private System.Windows.Forms.Label labelVC41d;
		private System.Windows.Forms.Label labelVC41c;
		private System.Windows.Forms.Label labelVC41b;
		private System.Windows.Forms.Label labelVC41a;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label labelCode10;
		private System.Windows.Forms.Label labelCode9;
		private System.Windows.Forms.Label labelCode8;
		private System.Windows.Forms.Label labelCode7;
		private System.Windows.Forms.Label labelCode6;
		private System.Windows.Forms.Label labelCode5;
		private System.Windows.Forms.Label labelCode4;
		private System.Windows.Forms.Label labelCode3;
		private System.Windows.Forms.Label labelCode2;
		private System.Windows.Forms.Label labelCode1;
		private System.Windows.Forms.Label labelCode0;
		private System.Windows.Forms.Label labelAttachedImagesWillNotBeSent;
		private System.Windows.Forms.Label label63;
		private System.Windows.Forms.Label label64;
		private System.Windows.Forms.Label label65;
		private System.Windows.Forms.Label label66;
		private System.Windows.Forms.Label label67;
		private System.Windows.Forms.Label label68;
		private System.Windows.Forms.Label label69;
		private System.Windows.Forms.Label label70;
		private System.Windows.Forms.Label label71;
		private System.Windows.Forms.Label label72;
		private System.Windows.Forms.Label label73;
		private System.Windows.Forms.Label labelShareOfCost;
		private System.Windows.Forms.Label label76;
		private System.Windows.Forms.Label label77;
		private System.Windows.Forms.Label label78;
		private System.Windows.Forms.Label label79;
		private System.Windows.Forms.Label label80;
		private System.Windows.Forms.Label label81;
		private System.Windows.Forms.Label label82;
		private System.Windows.Forms.Label labelAdmissionType;
		private System.Windows.Forms.Label labelAdmissionSource;
		private System.Windows.Forms.Label labelPatientStatus;
		private System.Windows.Forms.Label label86;
		private System.Windows.Forms.Label label87;
		private System.Windows.Forms.Label labelClaimIdentifierDentiCal;
		private System.Windows.Forms.Label labelCorrectionType;
		private System.Windows.Forms.Label labelClaimIdentifier;
		private System.Windows.Forms.Label labelOrigRefNum;
		private System.Windows.Forms.Label labelOrigRefNumDentiCal;
		private System.Windows.Forms.Label labelPriorAuthDentiCal;
		private System.Windows.Forms.Label label96;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.Label labelNote;
		private System.Windows.Forms.GroupBox groupProsth;
		private System.Windows.Forms.Label labelMissingTeeth;
		private System.Windows.Forms.TextBox textPredeterm;
		private OpenDental.ValidDate textDateRec;
		private OpenDental.ValidDate textDateSentOrig;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.RadioButton radioProsthN;
		private System.Windows.Forms.RadioButton radioProsthR;
		private System.Windows.Forms.RadioButton radioProsthI;
		private System.Windows.Forms.TextBox textPlan;
		private System.Windows.Forms.TextBox textClaimFee;
		private OpenDental.ValidDouble textDedApplied;
		private OpenDental.ValidDouble textInsPayAmt;
		private OpenDental.ValidDate textPriorDate;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupOrtho;
		private System.Windows.Forms.Label labelOrthoRemainM;
		private System.Windows.Forms.CheckBox checkIsOrtho;
		private OpenDental.ValidNum textOrthoRemainM;
		private OpenDental.ValidDate textOrthoDate;
		private System.Windows.Forms.Label labelOrthoDate;
		private System.Windows.Forms.Label labelPredeterm;
		private System.Windows.Forms.Label labelDateService;
		private UI.ComboBoxOD comboProvBill;
		private UI.ComboBoxOD comboProvTreat;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.ComboBox comboPatRelat;
		private System.Windows.Forms.ComboBox comboPatRelat2;
		private System.Windows.Forms.TextBox textPlan2;
		private OpenDental.UI.Button butRecalc;
		private System.Windows.Forms.TextBox textInsPayEst;
		private OpenDental.ValidDouble textWriteOff;
		private OpenDental.UI.Button butPreview;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butOtherNone;
		private OpenDental.UI.Button butOtherCovChange;
		private OpenDental.ODtextBox textNote;
		private UI.ComboBoxClinicPicker comboClinic;
		private OpenDental.UI.Button butLabel;
		private OpenDental.UI.Button butSplit;
		private OpenDental.UI.Button butSend;
		private OpenDental.UI.Button butHistory;
		private OpenDental.ValidDate textDateService;
		private UI.Button butExport;
		private UI.Button butAttachAdd;
		private UI.Button butAttachPerio;
		private OpenDental.UI.Button butPaySupp;
		private OpenDental.UI.Button butPayTotal;
		private OpenDental.UI.Button butPayProc;
		private OpenDental.UI.Button butReferralSelect;
		private OpenDental.UI.Button butReferralNone;
		private OpenDental.UI.Button butReferralEdit;
		private OpenDental.UI.Button butMissingTeethHelp;
		private UI.Button butBatch;
		private UI.Button butReverse;
		private UI.Button butViewEra;
		private UI.Button butResend;
		private UI.Button butPickProvBill;
		private UI.Button butPickProvTreat;
		private UI.Button butPickOrderProvReferral;
		private UI.Button butNoneOrderProv;
		private UI.Button butPickOrderProvInternal;
		private UI.Button butAdd;
		private System.Windows.Forms.CheckBox checkImages;
		private System.Windows.Forms.CheckBox checkXrays;
		private System.Windows.Forms.CheckBox checkModels;
		private System.Windows.Forms.CheckBox checkCorrespondence;
		private System.Windows.Forms.CheckBox checkEmail;
		private System.Windows.Forms.ComboBox comboReferralReason;
		private System.Windows.Forms.TextBox textReferralProvider;
		private System.Windows.Forms.ComboBox comboMaxProsth;
		private System.Windows.Forms.GroupBox groupMandPros;
		private System.Windows.Forms.ComboBox comboMandProsth;
		private ValidDate textDateInitialLower;
		private System.Windows.Forms.ComboBox comboMandProsthMaterial;
		private ValidDate textCanadianAccidentDate;
		private System.Windows.Forms.GroupBox groupAccident;
		private System.Windows.Forms.CheckBox checkCanadianIsOrtho;
		private System.Windows.Forms.TextBox textMissingTeeth;
		private System.Windows.Forms.Label labelCanadaMissingTeeth;
		private System.Windows.Forms.Label labelExtractedTeeth;
		private OpenDental.UI.ListBoxOD listExtractedTeeth;
		private System.Windows.Forms.GroupBox groupValueCodes;
		private System.Windows.Forms.TextBox textVC39dAmt;
		private System.Windows.Forms.TextBox textVC39cAmt;
		private System.Windows.Forms.TextBox textVC39bAmt;
		private System.Windows.Forms.TextBox textVC39aAmt;
		private System.Windows.Forms.TextBox textVC39dCode;
		private System.Windows.Forms.TextBox textVC39cCode;
		private System.Windows.Forms.TextBox textVC39bCode;
		private System.Windows.Forms.TextBox textVC39aCode;
		private System.Windows.Forms.TextBox textVC40dAmt;
		private System.Windows.Forms.TextBox textVC40cAmt;
		private System.Windows.Forms.TextBox textVC40bAmt;
		private System.Windows.Forms.TextBox textVC40aAmt;
		private System.Windows.Forms.TextBox textVC40dCode;
		private System.Windows.Forms.TextBox textVC40cCode;
		private System.Windows.Forms.TextBox textVC40bCode;
		private System.Windows.Forms.TextBox textVC40aCode;
		private System.Windows.Forms.TextBox textVC41dAmt;
		private System.Windows.Forms.TextBox textVC41cAmt;
		private System.Windows.Forms.TextBox textVC41bAmt;
		private System.Windows.Forms.TextBox textVC41aAmt;
		private System.Windows.Forms.TextBox textVC41dCode;
		private System.Windows.Forms.TextBox textVC41cCode;
		private System.Windows.Forms.TextBox textVC41bCode;
		private System.Windows.Forms.TextBox textVC41aCode;
		private System.Windows.Forms.TextBox textReasonUnder;
		private System.Windows.Forms.ComboBox comboAccident;
		private System.Windows.Forms.Label labelAccidentST;
		private System.Windows.Forms.ComboBox comboEmployRelated;
		private System.Windows.Forms.TextBox textAccidentST;
		private System.Windows.Forms.ComboBox comboPlaceService;
		private ValidDate textAccidentDate;
		private System.Windows.Forms.TextBox textRefProv;
		private System.Windows.Forms.TextBox textRefNum;
		private System.Windows.Forms.TextBox textCode10;
		private System.Windows.Forms.TextBox textCode9;
		private System.Windows.Forms.TextBox textCode8;
		private System.Windows.Forms.TextBox textCode7;
		private System.Windows.Forms.TextBox textCode6;
		private System.Windows.Forms.TextBox textCode5;
		private System.Windows.Forms.TextBox textCode4;
		private System.Windows.Forms.TextBox textCode3;
		private System.Windows.Forms.TextBox textCode2;
		private System.Windows.Forms.TextBox textCode1;
		private System.Windows.Forms.TextBox textCode0;
		private System.Windows.Forms.GroupBox groupMaxPros;
		private ValidDate textDateInitialUpper;
		private System.Windows.Forms.ComboBox comboMaxProsthMaterial;
		private System.Windows.Forms.GroupBox groupCanadaOrthoPredeterm;
		private ValidDate textDateCanadaEstTreatStartDate;
		private System.Windows.Forms.TextBox textCanadaTransRefNum;
		private System.Windows.Forms.TextBox textCanadaInitialPayment;
		private System.Windows.Forms.TextBox textCanadaTreatDuration;
		private System.Windows.Forms.TextBox textCanadaNumPaymentsAnticipated;
		private System.Windows.Forms.TextBox textCanadaAnticipatedPayAmount;
		private System.Windows.Forms.TextBox textCanadaExpectedPayCycle;
		private System.Windows.Forms.ComboBox comboClaimType;
		private System.Windows.Forms.TextBox textPriorAuth;
		private System.Windows.Forms.Label labelPriorAuth;
		private System.Windows.Forms.ComboBox comboSpecialProgram;
		private System.Windows.Forms.Label labelSpecialProgram;
		private System.Windows.Forms.TextBox textBillType;
		private System.Windows.Forms.TextBox textPatientStatus;
		private System.Windows.Forms.TextBox textAdmissionSource;
		private System.Windows.Forms.TextBox textAdmissionType;
		private System.Windows.Forms.ComboBox comboMedType;
		private System.Windows.Forms.ComboBox comboClaimForm;
		private System.Windows.Forms.Label labelBatch;
		private ValidDate textDateSent;
		private System.Windows.Forms.Label labelDateSent;
		private System.Windows.Forms.TextBox textOrigRefNum;
		private System.Windows.Forms.TextBox textClaimIdentifier;
		private System.Windows.Forms.ComboBox comboCorrectionType;
		private System.Windows.Forms.GroupBox groupAttachments;
		private System.Windows.Forms.TextBox textAttachID;
		private System.Windows.Forms.RadioButton radioAttachElect;
		private System.Windows.Forms.RadioButton radioAttachMail;
		private System.Windows.Forms.CheckBox checkAttachMisc;
		private System.Windows.Forms.CheckBox checkAttachPerio;
		private System.Windows.Forms.CheckBox checkAttachNarrative;
		private System.Windows.Forms.CheckBox checkAttachEoB;
		private ValidNum textAttachModels;
		private System.Windows.Forms.Label labelOralImages;
		private ValidNum textAttachImages;
		private System.Windows.Forms.Label labelRadiographs;
		private ValidNum textRadiographs;
		private ValidDouble textShareOfCost;
		private System.Windows.Forms.GroupBox groupEnterPayment;
		private System.Windows.Forms.GroupBox groupReferral;
		private System.Windows.Forms.GroupBox groupUb04;
		private System.Windows.Forms.GroupBox groupAttachedImages;
		private OpenDental.UI.ListBoxOD listAttachments;
		private System.Windows.Forms.TabControl tabMain;
		private System.Windows.Forms.TabPage tabUB04;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.TabPage tabCanadian;
		private ValidNum textOrthoTotalM;
		private System.Windows.Forms.TabPage tabHistory;
		private System.Windows.Forms.TabPage tabAttachments;
		private System.Windows.Forms.TabPage tabMisc;
		private ODtextBox textOrderingProviderOverride;
		private UI.GridOD gridPay;
		private System.Windows.Forms.ContextMenu contextMenuAttachments;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemRename;
		private System.Windows.Forms.MenuItem menuItemRemove;
		private UI.GridOD gridStatusHistory;
		private UI.GridOD gridProc;
		private System.Windows.Forms.ContextMenu contextAdjust;
		private System.Windows.Forms.MenuItem menuItemAddAdj;
		private System.Windows.Forms.TextBox textClaimIdOriginal;
		private System.Windows.Forms.Label labelClaimIdOriginal;
		private System.Windows.Forms.GroupBox groupFinalizePayment;
		private UI.Button butThisClaimOnly;
		private UI.Button butViewEob;
		private System.Windows.Forms.TabControl tabAttach;
		private System.Windows.Forms.TabPage tabNEA;
		private System.Windows.Forms.TabPage tabDXC;
		private UI.GridOD gridSent;
		private ODtextBox textAttachmentID;
		private System.Windows.Forms.Label labelAttachmentID;
		private UI.Button butClaimAttachment;
		private System.Windows.Forms.ErrorProvider _recalcErrorProvider;
		private System.Windows.Forms.GroupBox groupDateOtherCondOrTreatment;
		private ValidDate textDateOther;
		private System.Windows.Forms.Label labelDateOtherQualifier;
		private System.Windows.Forms.Label labelDateOther;
		private UI.ComboBoxOD comboDateOtherQualifier;
		private System.Windows.Forms.Label labelDateIllnessQualifier;
		private UI.ComboBoxOD comboDateIllnessQualifier;
		private ValidDate textDateIllness;
		private System.Windows.Forms.Label labelDateIllness;
		private System.Windows.Forms.GroupBox groupDateIllnessInjuryPreg;
		private System.Windows.Forms.CheckBox checkIsOutsideLab;
		private System.Windows.Forms.ComboBox comboClaimStatus;
		private ValidDouble textPatResp;
	}
}
