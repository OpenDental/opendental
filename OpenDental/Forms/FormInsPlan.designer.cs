﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsPlan {
		private System.ComponentModel.IContainer components = null;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsPlan));
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.checkAssign = new OpenDental.UI.CheckBox();
			this.checkRelease = new OpenDental.UI.CheckBox();
			this.textSubscriber = new System.Windows.Forms.TextBox();
			this.groupSubscriber = new OpenDental.UI.GroupBox();
			this.butChange = new OpenDental.UI.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.textSubscriberID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateEffect = new OpenDental.ValidDate();
			this.textDateTerm = new OpenDental.ValidDate();
			this.textSubscNote = new OpenDental.ODtextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.butImportTrojan = new OpenDental.UI.Button();
			this.butIapFind = new OpenDental.UI.Button();
			this.butBenefitNotes = new OpenDental.UI.Button();
			this.butHistoryElect = new OpenDental.UI.Button();
			this.butGetElectronic = new OpenDental.UI.Button();
			this.butSubstCodes = new OpenDental.UI.Button();
			this.labelDrop = new System.Windows.Forms.Label();
			this.groupRequestBen = new OpenDental.UI.GroupBox();
			this.labelHistElect = new System.Windows.Forms.Label();
			this.textElectBenLastDate = new System.Windows.Forms.TextBox();
			this.labelTrojanID = new System.Windows.Forms.Label();
			this.textTrojanID = new System.Windows.Forms.TextBox();
			this.label26 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.comboRelationship = new OpenDental.UI.ComboBox();
			this.label31 = new System.Windows.Forms.Label();
			this.checkIsPending = new OpenDental.UI.CheckBox();
			this.label32 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.listAdj = new OpenDental.UI.ListBox();
			this.label35 = new System.Windows.Forms.Label();
			this.textPatID = new System.Windows.Forms.TextBox();
			this.labelPatID = new System.Windows.Forms.Label();
			this.panelPat = new System.Windows.Forms.Panel();
			this.butAuditPat = new OpenDental.UI.Button();
			this.butHistory = new OpenDental.UI.Button();
			this.butPatOrtho = new OpenDental.UI.Button();
			this.label30 = new System.Windows.Forms.Label();
			this.textDateLastVerifiedPatPlan = new OpenDental.ValidDate();
			this.butVerifyPatPlan = new OpenDental.UI.Button();
			this.textPatPlanNum = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.textOrdinal = new OpenDental.ValidNum();
			this.butAdjAdd = new OpenDental.UI.Button();
			this.butDrop = new OpenDental.UI.Button();
			this.label18 = new System.Windows.Forms.Label();
			this.radioChangeAll = new System.Windows.Forms.RadioButton();
			this.groupChanges = new OpenDental.UI.GroupBox();
			this.radioCreateNew = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.label34 = new System.Windows.Forms.Label();
			this.checkDontVerify = new OpenDental.UI.CheckBox();
			this.butVerifyBenefits = new OpenDental.UI.Button();
			this.textDateLastVerifiedBenefits = new OpenDental.ValidDate();
			this.butAudit = new OpenDental.UI.Button();
			this.butPick = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butLabel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.tabControlInsPlan = new OpenDental.UI.TabControl();
			this.tabPageInsPlanInfo = new OpenDental.UI.TabPage();
			this.panelPlan = new System.Windows.Forms.Panel();
			this.checkUseBlueBook = new OpenDental.UI.CheckBox();
			this.groupCarrierAllowedAmounts = new OpenDental.UI.GroupBox();
			this.labelManualBlueBook = new System.Windows.Forms.Label();
			this.comboManualBlueBook = new OpenDental.UI.ComboBox();
			this.labelOutOfNetwork = new System.Windows.Forms.Label();
			this.comboOutOfNetwork = new OpenDental.UI.ComboBox();
			this.comboFeeSched = new OpenDental.UI.ComboBox();
			this.groupCoPay = new OpenDental.UI.GroupBox();
			this.labelCopayFeeSched = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboCopay = new OpenDental.UI.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textInsPlanNum = new System.Windows.Forms.TextBox();
			this.label29 = new System.Windows.Forms.Label();
			this.groupPlan = new OpenDental.UI.GroupBox();
			this.butOtherSubscribers = new OpenDental.UI.Button();
			this.textBIN = new System.Windows.Forms.TextBox();
			this.labelBIN = new System.Windows.Forms.Label();
			this.textDivisionNo = new System.Windows.Forms.TextBox();
			this.textGroupName = new System.Windows.Forms.TextBox();
			this.textEmployer = new System.Windows.Forms.TextBox();
			this.groupCarrier = new OpenDental.UI.GroupBox();
			this.labelSendElectronically = new System.Windows.Forms.Label();
			this.comboSendElectronically = new OpenDental.UI.ComboBox();
			this.butPickCarrier = new OpenDental.UI.Button();
			this.textPhone = new OpenDental.ValidPhone();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.comboElectIDdescript = new OpenDental.UI.ComboBox();
			this.textElectID = new System.Windows.Forms.TextBox();
			this.butSearch = new OpenDental.UI.Button();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.textZip = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.labelElectronicID = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.labelCitySTZip = new System.Windows.Forms.Label();
			this.checkIsMedical = new OpenDental.UI.CheckBox();
			this.textGroupNum = new System.Windows.Forms.TextBox();
			this.labelGroupNum = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboLinked = new OpenDental.UI.ComboBox();
			this.textLinkedNum = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelDivisionDash = new System.Windows.Forms.Label();
			this.labelMedicalInsurance = new System.Windows.Forms.Label();
			this.comboPlanType = new OpenDental.UI.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.tabPageOtherInsInfo = new OpenDental.UI.TabPage();
			this.panelOrthInfo = new System.Windows.Forms.Panel();
			this.label41 = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBoxPerVisitAmounts = new OpenDental.UI.GroupBox();
			this.labelPerVisit = new System.Windows.Forms.Label();
			this.textPerVisitInsAmount = new OpenDental.ValidDouble();
			this.textPerVisitPatAmount = new OpenDental.ValidDouble();
			this.labelPerVisitPatAmount = new System.Windows.Forms.Label();
			this.labelPerVisitInsuranceAmount = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.labelZeroWriteoffAnnualMax = new System.Windows.Forms.Label();
			this.comboZeroWriteOffsOnLimitationOrAging = new OpenDental.UI.ComboBox();
			this.comboBoxZeroWriteOffsOnAnnualMaxLimitation = new OpenDental.UI.ComboBox();
			this.label39 = new System.Windows.Forms.Label();
			this.comboExclusionFeeRule = new OpenDental.UI.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.checkPpoSubWo = new OpenDental.UI.CheckBox();
			this.comboBillType = new OpenDental.UI.ComboBox();
			this.label38 = new System.Windows.Forms.Label();
			this.comboCobRule = new OpenDental.UI.ComboBox();
			this.label20 = new System.Windows.Forms.Label();
			this.comboFilingCodeSubtype = new OpenDental.UI.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.checkCodeSubst = new OpenDental.UI.CheckBox();
			this.checkShowBaseUnits = new OpenDental.UI.CheckBox();
			this.comboFilingCode = new OpenDental.UI.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.comboClaimForm = new OpenDental.UI.ComboBox();
			this.label23 = new System.Windows.Forms.Label();
			this.checkAlternateCode = new OpenDental.UI.CheckBox();
			this.checkClaimsUseUCR = new OpenDental.UI.CheckBox();
			this.tabPageCanadian = new OpenDental.UI.TabPage();
			this.panelCanadian = new System.Windows.Forms.Panel();
			this.groupCanadian = new OpenDental.UI.GroupBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textCanadianInstCode = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textCanadianDiagCode = new System.Windows.Forms.TextBox();
			this.checkIsPMP = new OpenDental.UI.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.textPlanFlag = new System.Windows.Forms.TextBox();
			this.textDentaide = new OpenDental.ValidNum();
			this.labelDentaide = new System.Windows.Forms.Label();
			this.tabPageOrtho = new OpenDental.UI.TabPage();
			this.panelOrtho = new System.Windows.Forms.Panel();
			this.textOrthoAutoFee = new OpenDental.ValidDouble();
			this.labelOrthoAutoFee = new System.Windows.Forms.Label();
			this.butDefaultAutoOrthoProc = new OpenDental.UI.Button();
			this.butPickOrthoProc = new OpenDental.UI.Button();
			this.textOrthoAutoProc = new System.Windows.Forms.TextBox();
			this.label37 = new System.Windows.Forms.Label();
			this.comboOrthoClaimType = new OpenDental.UI.ComboBox();
			this.comboOrthoAutoProcPeriod = new OpenDental.UI.ComboBox();
			this.labelAutoOrthoProcPeriod = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.checkOrthoWaitDays = new OpenDental.UI.CheckBox();
			this.gridBenefits = new OpenDental.UI.GridOD();
			this.textPlanNote = new OpenDental.ODtextBox();
			this.groupSubscriber.SuspendLayout();
			this.groupRequestBen.SuspendLayout();
			this.panelPat.SuspendLayout();
			this.groupChanges.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabControlInsPlan.SuspendLayout();
			this.tabPageInsPlanInfo.SuspendLayout();
			this.panelPlan.SuspendLayout();
			this.groupCarrierAllowedAmounts.SuspendLayout();
			this.groupCoPay.SuspendLayout();
			this.groupPlan.SuspendLayout();
			this.groupCarrier.SuspendLayout();
			this.tabPageOtherInsInfo.SuspendLayout();
			this.panelOrthInfo.SuspendLayout();
			this.groupBoxPerVisitAmounts.SuspendLayout();
			this.groupBoxOD1.SuspendLayout();
			this.tabPageCanadian.SuspendLayout();
			this.panelCanadian.SuspendLayout();
			this.groupCanadian.SuspendLayout();
			this.tabPageOrtho.SuspendLayout();
			this.panelOrtho.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(1, 57);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 15);
			this.label5.TabIndex = 5;
			this.label5.Text = "Effective Dates";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(173, 57);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(30, 15);
			this.label6.TabIndex = 6;
			this.label6.Text = "To";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(1, 78);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(47, 18);
			this.label28.TabIndex = 28;
			this.label28.Text = "Note";
			this.label28.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkAssign
			// 
			this.checkAssign.Location = new System.Drawing.Point(285, 54);
			this.checkAssign.Name = "checkAssign";
			this.checkAssign.Size = new System.Drawing.Size(224, 20);
			this.checkAssign.TabIndex = 4;
			this.checkAssign.Text = "Assignment of Benefits (pay provider)";
			this.checkAssign.Click += new System.EventHandler(this.CheckAssign_Click);
			// 
			// checkRelease
			// 
			this.checkRelease.Location = new System.Drawing.Point(285, 36);
			this.checkRelease.Name = "checkRelease";
			this.checkRelease.Size = new System.Drawing.Size(205, 20);
			this.checkRelease.TabIndex = 3;
			this.checkRelease.Text = "Release of Information";
			// 
			// textSubscriber
			// 
			this.textSubscriber.Location = new System.Drawing.Point(100, 14);
			this.textSubscriber.Name = "textSubscriber";
			this.textSubscriber.ReadOnly = true;
			this.textSubscriber.Size = new System.Drawing.Size(298, 20);
			this.textSubscriber.TabIndex = 109;
			// 
			// groupSubscriber
			// 
			this.groupSubscriber.Controls.Add(this.butChange);
			this.groupSubscriber.Controls.Add(this.checkAssign);
			this.groupSubscriber.Controls.Add(this.label25);
			this.groupSubscriber.Controls.Add(this.checkRelease);
			this.groupSubscriber.Controls.Add(this.textSubscriber);
			this.groupSubscriber.Controls.Add(this.textSubscriberID);
			this.groupSubscriber.Controls.Add(this.label2);
			this.groupSubscriber.Controls.Add(this.textDateEffect);
			this.groupSubscriber.Controls.Add(this.label5);
			this.groupSubscriber.Controls.Add(this.textDateTerm);
			this.groupSubscriber.Controls.Add(this.label6);
			this.groupSubscriber.Controls.Add(this.textSubscNote);
			this.groupSubscriber.Controls.Add(this.label28);
			this.groupSubscriber.Location = new System.Drawing.Point(468, 94);
			this.groupSubscriber.Name = "groupSubscriber";
			this.groupSubscriber.Size = new System.Drawing.Size(510, 176);
			this.groupSubscriber.TabIndex = 2;
			this.groupSubscriber.Text = "Subscriber Information";
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(404, 13);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(73, 21);
			this.butChange.TabIndex = 121;
			this.butChange.Text = "Change";
			this.toolTip1.SetToolTip(this.butChange, "Change subscriber name");
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(1, 18);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(97, 15);
			this.label25.TabIndex = 115;
			this.label25.Text = "Name";
			this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textSubscriberID
			// 
			this.textSubscriberID.Location = new System.Drawing.Point(100, 34);
			this.textSubscriberID.MaxLength = 20;
			this.textSubscriberID.Name = "textSubscriberID";
			this.textSubscriberID.Size = new System.Drawing.Size(129, 20);
			this.textSubscriberID.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(1, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(97, 15);
			this.label2.TabIndex = 114;
			this.label2.Text = "Subscriber ID";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEffect
			// 
			this.textDateEffect.Location = new System.Drawing.Point(100, 54);
			this.textDateEffect.Name = "textDateEffect";
			this.textDateEffect.Size = new System.Drawing.Size(72, 20);
			this.textDateEffect.TabIndex = 1;
			// 
			// textDateTerm
			// 
			this.textDateTerm.Location = new System.Drawing.Point(206, 54);
			this.textDateTerm.Name = "textDateTerm";
			this.textDateTerm.Size = new System.Drawing.Size(72, 20);
			this.textDateTerm.TabIndex = 2;
			// 
			// textSubscNote
			// 
			this.textSubscNote.AcceptsTab = true;
			this.textSubscNote.BackColor = System.Drawing.SystemColors.Window;
			this.textSubscNote.DetectLinksEnabled = false;
			this.textSubscNote.DetectUrls = false;
			this.textSubscNote.Location = new System.Drawing.Point(48, 75);
			this.textSubscNote.Name = "textSubscNote";
			this.textSubscNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
			this.textSubscNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSubscNote.Size = new System.Drawing.Size(439, 98);
			this.textSubscNote.TabIndex = 5;
			this.textSubscNote.Text = "1 - InsPlan subscriber\n2\n3 lines will show here in 46 vert.\n4 lines will show her" +
    "e in 59 vert.\n5 lines in 72 vert\n6 lines in 85 vert\n7 lines in 98";
			// 
			// butImportTrojan
			// 
			this.butImportTrojan.Location = new System.Drawing.Point(6, 14);
			this.butImportTrojan.Name = "butImportTrojan";
			this.butImportTrojan.Size = new System.Drawing.Size(55, 21);
			this.butImportTrojan.TabIndex = 0;
			this.butImportTrojan.Text = "Trojan";
			this.toolTip1.SetToolTip(this.butImportTrojan, "Edit all the similar plans at once");
			this.butImportTrojan.Click += new System.EventHandler(this.butImportTrojan_Click);
			// 
			// butIapFind
			// 
			this.butIapFind.Location = new System.Drawing.Point(64, 14);
			this.butIapFind.Name = "butIapFind";
			this.butIapFind.Size = new System.Drawing.Size(55, 21);
			this.butIapFind.TabIndex = 1;
			this.butIapFind.Text = "IAP";
			this.toolTip1.SetToolTip(this.butIapFind, "Edit all the similar plans at once");
			this.butIapFind.Click += new System.EventHandler(this.butIapFind_Click);
			// 
			// butBenefitNotes
			// 
			this.butBenefitNotes.Location = new System.Drawing.Point(122, 14);
			this.butBenefitNotes.Name = "butBenefitNotes";
			this.butBenefitNotes.Size = new System.Drawing.Size(60, 21);
			this.butBenefitNotes.TabIndex = 2;
			this.butBenefitNotes.Text = "Notes";
			this.toolTip1.SetToolTip(this.butBenefitNotes, "Edit all the similar plans at once");
			this.butBenefitNotes.Click += new System.EventHandler(this.butBenefitNotes_Click);
			// 
			// butHistoryElect
			// 
			this.butHistoryElect.Location = new System.Drawing.Point(89, 38);
			this.butHistoryElect.Name = "butHistoryElect";
			this.butHistoryElect.Size = new System.Drawing.Size(70, 21);
			this.butHistoryElect.TabIndex = 120;
			this.butHistoryElect.Text = "History";
			this.toolTip1.SetToolTip(this.butHistoryElect, "Edit all the similar plans at once");
			this.butHistoryElect.Click += new System.EventHandler(this.butHistoryElect_Click);
			// 
			// butGetElectronic
			// 
			this.butGetElectronic.Location = new System.Drawing.Point(14, 38);
			this.butGetElectronic.Name = "butGetElectronic";
			this.butGetElectronic.Size = new System.Drawing.Size(70, 21);
			this.butGetElectronic.TabIndex = 116;
			this.butGetElectronic.Text = "Request";
			this.toolTip1.SetToolTip(this.butGetElectronic, "Edit all the similar plans at once");
			this.butGetElectronic.Click += new System.EventHandler(this.butGetElectronic_Click);
			// 
			// butSubstCodes
			// 
			this.butSubstCodes.Location = new System.Drawing.Point(370, 30);
			this.butSubstCodes.Name = "butSubstCodes";
			this.butSubstCodes.Size = new System.Drawing.Size(79, 20);
			this.butSubstCodes.TabIndex = 187;
			this.butSubstCodes.Text = "Subst Codes";
			this.toolTip1.SetToolTip(this.butSubstCodes, "Edit all the similar plans at once");
			this.butSubstCodes.Click += new System.EventHandler(this.butSubstCodes_Click);
			// 
			// labelDrop
			// 
			this.labelDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDrop.Location = new System.Drawing.Point(80, 70);
			this.labelDrop.Name = "labelDrop";
			this.labelDrop.Size = new System.Drawing.Size(532, 15);
			this.labelDrop.TabIndex = 124;
			this.labelDrop.Text = "Drop a plan when a patient changes carriers or is no longer covered.  This does n" +
    "ot delete the plan.";
			this.labelDrop.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupRequestBen
			// 
			this.groupRequestBen.Controls.Add(this.butHistoryElect);
			this.groupRequestBen.Controls.Add(this.labelHistElect);
			this.groupRequestBen.Controls.Add(this.textElectBenLastDate);
			this.groupRequestBen.Controls.Add(this.butGetElectronic);
			this.groupRequestBen.Location = new System.Drawing.Point(468, 272);
			this.groupRequestBen.Name = "groupRequestBen";
			this.groupRequestBen.Size = new System.Drawing.Size(165, 63);
			this.groupRequestBen.TabIndex = 10;
			this.groupRequestBen.Text = "Request Electronic Benefits";
			// 
			// labelHistElect
			// 
			this.labelHistElect.Location = new System.Drawing.Point(3, 20);
			this.labelHistElect.Name = "labelHistElect";
			this.labelHistElect.Size = new System.Drawing.Size(84, 15);
			this.labelHistElect.TabIndex = 119;
			this.labelHistElect.Text = "Last Request";
			this.labelHistElect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textElectBenLastDate
			// 
			this.textElectBenLastDate.Location = new System.Drawing.Point(89, 17);
			this.textElectBenLastDate.MaxLength = 30;
			this.textElectBenLastDate.Name = "textElectBenLastDate";
			this.textElectBenLastDate.Size = new System.Drawing.Size(70, 20);
			this.textElectBenLastDate.TabIndex = 118;
			// 
			// labelTrojanID
			// 
			this.labelTrojanID.Location = new System.Drawing.Point(192, 18);
			this.labelTrojanID.Name = "labelTrojanID";
			this.labelTrojanID.Size = new System.Drawing.Size(23, 15);
			this.labelTrojanID.TabIndex = 9;
			this.labelTrojanID.Text = "ID";
			this.labelTrojanID.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textTrojanID
			// 
			this.textTrojanID.Location = new System.Drawing.Point(217, 15);
			this.textTrojanID.MaxLength = 30;
			this.textTrojanID.Name = "textTrojanID";
			this.textTrojanID.Size = new System.Drawing.Size(113, 20);
			this.textTrojanID.TabIndex = 8;
			// 
			// label26
			// 
			this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label26.Location = new System.Drawing.Point(20, 28);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(148, 14);
			this.label26.TabIndex = 127;
			this.label26.Text = "Relationship to Subscriber";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
			this.panel1.Location = new System.Drawing.Point(0, 90);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(988, 2);
			this.panel1.TabIndex = 128;
			// 
			// comboRelationship
			// 
			this.comboRelationship.Location = new System.Drawing.Point(170, 24);
			this.comboRelationship.Name = "comboRelationship";
			this.comboRelationship.Size = new System.Drawing.Size(151, 21);
			this.comboRelationship.TabIndex = 0;
			// 
			// label31
			// 
			this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label31.Location = new System.Drawing.Point(329, 30);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(138, 17);
			this.label31.TabIndex = 130;
			this.label31.Text = "Order";
			this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsPending
			// 
			this.checkIsPending.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPending.Location = new System.Drawing.Point(515, 29);
			this.checkIsPending.Name = "checkIsPending";
			this.checkIsPending.Size = new System.Drawing.Size(97, 16);
			this.checkIsPending.TabIndex = 3;
			this.checkIsPending.Text = "Pending";
			// 
			// label32
			// 
			this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label32.Location = new System.Drawing.Point(6, 5);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(304, 19);
			this.label32.TabIndex = 134;
			this.label32.Text = "Insurance Plan Information";
			// 
			// label33
			// 
			this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label33.Location = new System.Drawing.Point(5, 0);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(188, 19);
			this.label33.TabIndex = 135;
			this.label33.Text = "Patient Information";
			// 
			// listAdj
			// 
			this.listAdj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listAdj.ItemStrings = new string[] {
        "03/05/2001       Ins Used:  $124.00       Ded Used:  $50.00",
        "03/05/2002       Ins Used:  $0.00       Ded Used:  $50.00"};
			this.listAdj.Location = new System.Drawing.Point(678, 28);
			this.listAdj.Name = "listAdj";
			this.listAdj.Size = new System.Drawing.Size(296, 56);
			this.listAdj.TabIndex = 137;
			this.listAdj.DoubleClick += new System.EventHandler(this.listAdj_DoubleClick);
			// 
			// label35
			// 
			this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label35.Location = new System.Drawing.Point(680, 8);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(218, 17);
			this.label35.TabIndex = 138;
			this.label35.Text = "Adjustments to Insurance Benefits: ";
			this.label35.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPatID
			// 
			this.textPatID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textPatID.Location = new System.Drawing.Point(170, 46);
			this.textPatID.MaxLength = 100;
			this.textPatID.Name = "textPatID";
			this.textPatID.Size = new System.Drawing.Size(151, 20);
			this.textPatID.TabIndex = 1;
			// 
			// labelPatID
			// 
			this.labelPatID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPatID.Location = new System.Drawing.Point(30, 48);
			this.labelPatID.Name = "labelPatID";
			this.labelPatID.Size = new System.Drawing.Size(138, 16);
			this.labelPatID.TabIndex = 143;
			this.labelPatID.Text = "Optional Patient ID";
			this.labelPatID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelPat
			// 
			this.panelPat.Controls.Add(this.butAuditPat);
			this.panelPat.Controls.Add(this.butHistory);
			this.panelPat.Controls.Add(this.butPatOrtho);
			this.panelPat.Controls.Add(this.label30);
			this.panelPat.Controls.Add(this.textDateLastVerifiedPatPlan);
			this.panelPat.Controls.Add(this.butVerifyPatPlan);
			this.panelPat.Controls.Add(this.textPatPlanNum);
			this.panelPat.Controls.Add(this.label27);
			this.panelPat.Controls.Add(this.comboRelationship);
			this.panelPat.Controls.Add(this.label33);
			this.panelPat.Controls.Add(this.textOrdinal);
			this.panelPat.Controls.Add(this.butAdjAdd);
			this.panelPat.Controls.Add(this.listAdj);
			this.panelPat.Controls.Add(this.label35);
			this.panelPat.Controls.Add(this.textPatID);
			this.panelPat.Controls.Add(this.labelPatID);
			this.panelPat.Controls.Add(this.labelDrop);
			this.panelPat.Controls.Add(this.butDrop);
			this.panelPat.Controls.Add(this.label26);
			this.panelPat.Controls.Add(this.label31);
			this.panelPat.Controls.Add(this.checkIsPending);
			this.panelPat.Location = new System.Drawing.Point(0, 0);
			this.panelPat.Name = "panelPat";
			this.panelPat.Size = new System.Drawing.Size(982, 90);
			this.panelPat.TabIndex = 15;
			// 
			// butAuditPat
			// 
			this.butAuditPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAuditPat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butAuditPat.Location = new System.Drawing.Point(169, 1);
			this.butAuditPat.Name = "butAuditPat";
			this.butAuditPat.Size = new System.Drawing.Size(69, 22);
			this.butAuditPat.TabIndex = 151;
			this.butAuditPat.Text = "Audit Trail";
			this.butAuditPat.Click += new System.EventHandler(this.butAuditPat_Click);
			// 
			// butHistory
			// 
			this.butHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butHistory.Location = new System.Drawing.Point(618, 7);
			this.butHistory.Name = "butHistory";
			this.butHistory.Size = new System.Drawing.Size(59, 21);
			this.butHistory.TabIndex = 150;
			this.butHistory.Text = "Hist";
			this.butHistory.Click += new System.EventHandler(this.butHistory_Click);
			// 
			// butPatOrtho
			// 
			this.butPatOrtho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPatOrtho.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butPatOrtho.Location = new System.Drawing.Point(618, 61);
			this.butPatOrtho.Name = "butPatOrtho";
			this.butPatOrtho.Size = new System.Drawing.Size(59, 21);
			this.butPatOrtho.TabIndex = 149;
			this.butPatOrtho.Text = "Ortho";
			this.butPatOrtho.Click += new System.EventHandler(this.butPatOrtho_Click);
			// 
			// label30
			// 
			this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label30.Location = new System.Drawing.Point(329, 50);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(138, 17);
			this.label30.TabIndex = 148;
			this.label30.Text = "Eligibility Last Verified";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateLastVerifiedPatPlan
			// 
			this.textDateLastVerifiedPatPlan.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.textDateLastVerifiedPatPlan.Location = new System.Drawing.Point(468, 48);
			this.textDateLastVerifiedPatPlan.Name = "textDateLastVerifiedPatPlan";
			this.textDateLastVerifiedPatPlan.Size = new System.Drawing.Size(70, 20);
			this.textDateLastVerifiedPatPlan.TabIndex = 146;
			// 
			// butVerifyPatPlan
			// 
			this.butVerifyPatPlan.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.butVerifyPatPlan.Location = new System.Drawing.Point(539, 48);
			this.butVerifyPatPlan.Name = "butVerifyPatPlan";
			this.butVerifyPatPlan.Size = new System.Drawing.Size(32, 21);
			this.butVerifyPatPlan.TabIndex = 147;
			this.butVerifyPatPlan.Text = "Now";
			this.butVerifyPatPlan.UseVisualStyleBackColor = true;
			this.butVerifyPatPlan.Click += new System.EventHandler(this.butVerifyPatPlan_Click);
			// 
			// textPatPlanNum
			// 
			this.textPatPlanNum.BackColor = System.Drawing.SystemColors.Control;
			this.textPatPlanNum.Location = new System.Drawing.Point(468, 8);
			this.textPatPlanNum.Name = "textPatPlanNum";
			this.textPatPlanNum.ReadOnly = true;
			this.textPatPlanNum.Size = new System.Drawing.Size(144, 20);
			this.textPatPlanNum.TabIndex = 144;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(329, 10);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(138, 17);
			this.label27.TabIndex = 145;
			this.label27.Text = "Patient Plan ID";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOrdinal
			// 
			this.textOrdinal.Location = new System.Drawing.Point(468, 28);
			this.textOrdinal.MaxVal = 10;
			this.textOrdinal.MinVal = 1;
			this.textOrdinal.Name = "textOrdinal";
			this.textOrdinal.ShowZero = false;
			this.textOrdinal.Size = new System.Drawing.Size(45, 20);
			this.textOrdinal.TabIndex = 2;
			// 
			// butAdjAdd
			// 
			this.butAdjAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdjAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butAdjAdd.Location = new System.Drawing.Point(915, 7);
			this.butAdjAdd.Name = "butAdjAdd";
			this.butAdjAdd.Size = new System.Drawing.Size(59, 21);
			this.butAdjAdd.TabIndex = 4;
			this.butAdjAdd.Text = "Add";
			this.butAdjAdd.Click += new System.EventHandler(this.butAdjAdd_Click);
			// 
			// butDrop
			// 
			this.butDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDrop.Location = new System.Drawing.Point(7, 67);
			this.butDrop.Name = "butDrop";
			this.butDrop.Size = new System.Drawing.Size(72, 21);
			this.butDrop.TabIndex = 5;
			this.butDrop.Text = "Drop";
			this.butDrop.Click += new System.EventHandler(this.butDrop_Click);
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(12, 563);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(272, 15);
			this.label18.TabIndex = 156;
			this.label18.Text = "Plan Note";
			this.label18.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// radioChangeAll
			// 
			this.radioChangeAll.Location = new System.Drawing.Point(6, 17);
			this.radioChangeAll.Name = "radioChangeAll";
			this.radioChangeAll.Size = new System.Drawing.Size(211, 17);
			this.radioChangeAll.TabIndex = 158;
			this.radioChangeAll.Text = "Change Plan for all subscribers";
			this.radioChangeAll.UseVisualStyleBackColor = true;
			// 
			// groupChanges
			// 
			this.groupChanges.Controls.Add(this.radioCreateNew);
			this.groupChanges.Controls.Add(this.radioChangeAll);
			this.groupChanges.Location = new System.Drawing.Point(467, 655);
			this.groupChanges.Name = "groupChanges";
			this.groupChanges.Size = new System.Drawing.Size(240, 35);
			this.groupChanges.TabIndex = 159;
			// 
			// radioCreateNew
			// 
			this.radioCreateNew.Checked = true;
			this.radioCreateNew.Location = new System.Drawing.Point(6, 1);
			this.radioCreateNew.Name = "radioCreateNew";
			this.radioCreateNew.Size = new System.Drawing.Size(211, 17);
			this.radioCreateNew.TabIndex = 159;
			this.radioCreateNew.TabStop = true;
			this.radioCreateNew.Text = "Create new Plan if needed";
			this.radioCreateNew.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butImportTrojan);
			this.groupBox1.Controls.Add(this.butIapFind);
			this.groupBox1.Controls.Add(this.butBenefitNotes);
			this.groupBox1.Controls.Add(this.textTrojanID);
			this.groupBox1.Controls.Add(this.labelTrojanID);
			this.groupBox1.Location = new System.Drawing.Point(637, 272);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(341, 40);
			this.groupBox1.TabIndex = 160;
			this.groupBox1.Text = "Import Benefits";
			// 
			// label34
			// 
			this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label34.Location = new System.Drawing.Point(634, 317);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(126, 17);
			this.label34.TabIndex = 149;
			this.label34.Text = "Benefits Last Verified";
			this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDontVerify
			// 
			this.checkDontVerify.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDontVerify.Location = new System.Drawing.Point(888, 317);
			this.checkDontVerify.Name = "checkDontVerify";
			this.checkDontVerify.Size = new System.Drawing.Size(80, 17);
			this.checkDontVerify.TabIndex = 161;
			this.checkDontVerify.Text = "Don\'t Verify";
			// 
			// butVerifyBenefits
			// 
			this.butVerifyBenefits.Location = new System.Drawing.Point(833, 313);
			this.butVerifyBenefits.Name = "butVerifyBenefits";
			this.butVerifyBenefits.Size = new System.Drawing.Size(32, 23);
			this.butVerifyBenefits.TabIndex = 150;
			this.butVerifyBenefits.Text = "Now";
			this.butVerifyBenefits.UseVisualStyleBackColor = true;
			this.butVerifyBenefits.Click += new System.EventHandler(this.butVerifyBenefits_Click);
			// 
			// textDateLastVerifiedBenefits
			// 
			this.textDateLastVerifiedBenefits.Location = new System.Drawing.Point(762, 315);
			this.textDateLastVerifiedBenefits.Name = "textDateLastVerifiedBenefits";
			this.textDateLastVerifiedBenefits.Size = new System.Drawing.Size(70, 20);
			this.textDateLastVerifiedBenefits.TabIndex = 149;
			// 
			// butAudit
			// 
			this.butAudit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAudit.Location = new System.Drawing.Point(230, 4);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(69, 23);
			this.butAudit.TabIndex = 153;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// butPick
			// 
			this.butPick.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPick.Location = new System.Drawing.Point(327, 4);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(90, 23);
			this.butPick.TabIndex = 153;
			this.butPick.Text = "Pick From List";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(814, 671);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butLabel
			// 
			this.butLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabel.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabel.Location = new System.Drawing.Point(201, 671);
			this.butLabel.Name = "butLabel";
			this.butLabel.Size = new System.Drawing.Size(81, 24);
			this.butLabel.TabIndex = 125;
			this.butLabel.Text = "Label";
			this.butLabel.Click += new System.EventHandler(this.butLabel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 671);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 112;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(900, 671);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// tabControlInsPlan
			// 
			this.tabControlInsPlan.Controls.Add(this.tabPageInsPlanInfo);
			this.tabControlInsPlan.Controls.Add(this.tabPageOtherInsInfo);
			this.tabControlInsPlan.Controls.Add(this.tabPageCanadian);
			this.tabControlInsPlan.Controls.Add(this.tabPageOrtho);
			this.tabControlInsPlan.Location = new System.Drawing.Point(7, 96);
			this.tabControlInsPlan.Name = "tabControlInsPlan";
			this.tabControlInsPlan.Size = new System.Drawing.Size(455, 466);
			this.tabControlInsPlan.TabIndex = 122;
			// 
			// tabPageInsPlanInfo
			// 
			this.tabPageInsPlanInfo.Controls.Add(this.panelPlan);
			this.tabPageInsPlanInfo.Controls.Add(this.butAudit);
			this.tabPageInsPlanInfo.Controls.Add(this.label32);
			this.tabPageInsPlanInfo.Controls.Add(this.butPick);
			this.tabPageInsPlanInfo.Location = new System.Drawing.Point(2, 21);
			this.tabPageInsPlanInfo.Name = "tabPageInsPlanInfo";
			this.tabPageInsPlanInfo.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageInsPlanInfo.Size = new System.Drawing.Size(451, 443);
			this.tabPageInsPlanInfo.TabIndex = 0;
			this.tabPageInsPlanInfo.Text = "Plan Info";
			// 
			// panelPlan
			// 
			this.panelPlan.AutoScroll = true;
			this.panelPlan.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.panelPlan.BackColor = System.Drawing.Color.White;
			this.panelPlan.Controls.Add(this.checkUseBlueBook);
			this.panelPlan.Controls.Add(this.groupCarrierAllowedAmounts);
			this.panelPlan.Controls.Add(this.comboFeeSched);
			this.panelPlan.Controls.Add(this.groupCoPay);
			this.panelPlan.Controls.Add(this.label1);
			this.panelPlan.Controls.Add(this.textInsPlanNum);
			this.panelPlan.Controls.Add(this.label29);
			this.panelPlan.Controls.Add(this.groupPlan);
			this.panelPlan.Controls.Add(this.comboPlanType);
			this.panelPlan.Controls.Add(this.label14);
			this.panelPlan.Location = new System.Drawing.Point(-3, 33);
			this.panelPlan.Name = "panelPlan";
			this.panelPlan.Size = new System.Drawing.Size(454, 406);
			this.panelPlan.TabIndex = 154;
			// 
			// checkUseBlueBook
			// 
			this.checkUseBlueBook.Location = new System.Drawing.Point(329, 331);
			this.checkUseBlueBook.Name = "checkUseBlueBook";
			this.checkUseBlueBook.Size = new System.Drawing.Size(107, 18);
			this.checkUseBlueBook.TabIndex = 191;
			this.checkUseBlueBook.Text = "Use Blue Book";
			this.checkUseBlueBook.CheckedChanged += new System.EventHandler(this.checkUseBlueBook_CheckedChanged);
			// 
			// groupCarrierAllowedAmounts
			// 
			this.groupCarrierAllowedAmounts.Controls.Add(this.labelManualBlueBook);
			this.groupCarrierAllowedAmounts.Controls.Add(this.comboManualBlueBook);
			this.groupCarrierAllowedAmounts.Controls.Add(this.labelOutOfNetwork);
			this.groupCarrierAllowedAmounts.Controls.Add(this.comboOutOfNetwork);
			this.groupCarrierAllowedAmounts.Location = new System.Drawing.Point(14, 353);
			this.groupCarrierAllowedAmounts.Name = "groupCarrierAllowedAmounts";
			this.groupCarrierAllowedAmounts.Size = new System.Drawing.Size(404, 70);
			this.groupCarrierAllowedAmounts.TabIndex = 112;
			this.groupCarrierAllowedAmounts.Text = "Carrier Allowed Amounts";
			// 
			// labelManualBlueBook
			// 
			this.labelManualBlueBook.Location = new System.Drawing.Point(6, 42);
			this.labelManualBlueBook.Name = "labelManualBlueBook";
			this.labelManualBlueBook.Size = new System.Drawing.Size(138, 20);
			this.labelManualBlueBook.TabIndex = 113;
			this.labelManualBlueBook.Text = "Manual Blue Book";
			this.labelManualBlueBook.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboManualBlueBook
			// 
			this.comboManualBlueBook.Location = new System.Drawing.Point(145, 42);
			this.comboManualBlueBook.Name = "comboManualBlueBook";
			this.comboManualBlueBook.Size = new System.Drawing.Size(212, 21);
			this.comboManualBlueBook.TabIndex = 112;
			// 
			// labelOutOfNetwork
			// 
			this.labelOutOfNetwork.Location = new System.Drawing.Point(6, 19);
			this.labelOutOfNetwork.Name = "labelOutOfNetwork";
			this.labelOutOfNetwork.Size = new System.Drawing.Size(138, 20);
			this.labelOutOfNetwork.TabIndex = 111;
			this.labelOutOfNetwork.Text = "Out of Network (Old)";
			this.labelOutOfNetwork.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboOutOfNetwork
			// 
			this.comboOutOfNetwork.Location = new System.Drawing.Point(145, 19);
			this.comboOutOfNetwork.Name = "comboOutOfNetwork";
			this.comboOutOfNetwork.Size = new System.Drawing.Size(212, 21);
			this.comboOutOfNetwork.TabIndex = 1;
			// 
			// comboFeeSched
			// 
			this.comboFeeSched.Location = new System.Drawing.Point(126, 329);
			this.comboFeeSched.Name = "comboFeeSched";
			this.comboFeeSched.Size = new System.Drawing.Size(200, 21);
			this.comboFeeSched.TabIndex = 180;
			this.comboFeeSched.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSched_SelectionChangeCommitted);
			// 
			// groupCoPay
			// 
			this.groupCoPay.Controls.Add(this.labelCopayFeeSched);
			this.groupCoPay.Controls.Add(this.label3);
			this.groupCoPay.Controls.Add(this.comboCopay);
			this.groupCoPay.Location = new System.Drawing.Point(14, 426);
			this.groupCoPay.Name = "groupCoPay";
			this.groupCoPay.Size = new System.Drawing.Size(404, 64);
			this.groupCoPay.TabIndex = 181;
			this.groupCoPay.Text = "Other Fee Schedules";
			// 
			// labelCopayFeeSched
			// 
			this.labelCopayFeeSched.Location = new System.Drawing.Point(6, 36);
			this.labelCopayFeeSched.Name = "labelCopayFeeSched";
			this.labelCopayFeeSched.Size = new System.Drawing.Size(138, 20);
			this.labelCopayFeeSched.TabIndex = 109;
			this.labelCopayFeeSched.Text = "Patient Co-pay Amounts";
			this.labelCopayFeeSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(1, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(390, 17);
			this.label3.TabIndex = 106;
			this.label3.Text = "Don\'t use this unless you understand how it will affect your estimates";
			// 
			// comboCopay
			// 
			this.comboCopay.Location = new System.Drawing.Point(145, 36);
			this.comboCopay.Name = "comboCopay";
			this.comboCopay.Size = new System.Drawing.Size(212, 21);
			this.comboCopay.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 330);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 17);
			this.label1.TabIndex = 182;
			this.label1.Text = "Fee Schedule";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPlanNum
			// 
			this.textInsPlanNum.BackColor = System.Drawing.SystemColors.Control;
			this.textInsPlanNum.Location = new System.Drawing.Point(126, 5);
			this.textInsPlanNum.Name = "textInsPlanNum";
			this.textInsPlanNum.ReadOnly = true;
			this.textInsPlanNum.Size = new System.Drawing.Size(144, 20);
			this.textInsPlanNum.TabIndex = 151;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(15, 6);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(110, 17);
			this.label29.TabIndex = 152;
			this.label29.Text = "Insurance Plan ID";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPlan
			// 
			this.groupPlan.Controls.Add(this.butOtherSubscribers);
			this.groupPlan.Controls.Add(this.textBIN);
			this.groupPlan.Controls.Add(this.labelBIN);
			this.groupPlan.Controls.Add(this.textDivisionNo);
			this.groupPlan.Controls.Add(this.textGroupName);
			this.groupPlan.Controls.Add(this.textEmployer);
			this.groupPlan.Controls.Add(this.groupCarrier);
			this.groupPlan.Controls.Add(this.textGroupNum);
			this.groupPlan.Controls.Add(this.labelGroupNum);
			this.groupPlan.Controls.Add(this.label8);
			this.groupPlan.Controls.Add(this.comboLinked);
			this.groupPlan.Controls.Add(this.textLinkedNum);
			this.groupPlan.Controls.Add(this.label16);
			this.groupPlan.Controls.Add(this.label4);
			this.groupPlan.Controls.Add(this.labelDivisionDash);
			this.groupPlan.Controls.Add(this.labelMedicalInsurance);
			this.groupPlan.Controls.Add(this.checkIsMedical);
			this.groupPlan.Location = new System.Drawing.Point(9, 26);
			this.groupPlan.Name = "groupPlan";
			this.groupPlan.Size = new System.Drawing.Size(425, 277);
			this.groupPlan.TabIndex = 148;
			// 
			// butOtherSubscribers
			// 
			this.butOtherSubscribers.Location = new System.Drawing.Point(399, 254);
			this.butOtherSubscribers.Name = "butOtherSubscribers";
			this.butOtherSubscribers.Size = new System.Drawing.Size(108, 20);
			this.butOtherSubscribers.TabIndex = 156;
			this.butOtherSubscribers.Text = "List Subscribers";
			this.butOtherSubscribers.Visible = false;
			this.butOtherSubscribers.Click += new System.EventHandler(this.butOtherSubscribers_Click);
			// 
			// textBIN
			// 
			this.textBIN.Location = new System.Drawing.Point(341, 212);
			this.textBIN.MaxLength = 20;
			this.textBIN.Name = "textBIN";
			this.textBIN.Size = new System.Drawing.Size(62, 20);
			this.textBIN.TabIndex = 115;
			// 
			// labelBIN
			// 
			this.labelBIN.Location = new System.Drawing.Point(307, 213);
			this.labelBIN.Name = "labelBIN";
			this.labelBIN.Size = new System.Drawing.Size(32, 17);
			this.labelBIN.TabIndex = 114;
			this.labelBIN.Text = "BIN";
			this.labelBIN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDivisionNo
			// 
			this.textDivisionNo.Location = new System.Drawing.Point(330, 233);
			this.textDivisionNo.MaxLength = 20;
			this.textDivisionNo.Name = "textDivisionNo";
			this.textDivisionNo.Size = new System.Drawing.Size(73, 20);
			this.textDivisionNo.TabIndex = 3;
			// 
			// textGroupName
			// 
			this.textGroupName.Location = new System.Drawing.Point(117, 212);
			this.textGroupName.MaxLength = 50;
			this.textGroupName.Name = "textGroupName";
			this.textGroupName.Size = new System.Drawing.Size(188, 20);
			this.textGroupName.TabIndex = 2;
			// 
			// textEmployer
			// 
			this.textEmployer.Location = new System.Drawing.Point(117, 21);
			this.textEmployer.MaxLength = 255;
			this.textEmployer.Name = "textEmployer";
			this.textEmployer.Size = new System.Drawing.Size(286, 20);
			this.textEmployer.TabIndex = 0;
			this.textEmployer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEmployer_KeyUp);
			this.textEmployer.Leave += new System.EventHandler(this.textEmployer_Leave);
			// 
			// groupCarrier
			// 
			this.groupCarrier.Controls.Add(this.labelSendElectronically);
			this.groupCarrier.Controls.Add(this.comboSendElectronically);
			this.groupCarrier.Controls.Add(this.butPickCarrier);
			this.groupCarrier.Controls.Add(this.textPhone);
			this.groupCarrier.Controls.Add(this.textAddress);
			this.groupCarrier.Controls.Add(this.comboElectIDdescript);
			this.groupCarrier.Controls.Add(this.textElectID);
			this.groupCarrier.Controls.Add(this.butSearch);
			this.groupCarrier.Controls.Add(this.textAddress2);
			this.groupCarrier.Controls.Add(this.textZip);
			this.groupCarrier.Controls.Add(this.label10);
			this.groupCarrier.Controls.Add(this.textCity);
			this.groupCarrier.Controls.Add(this.label7);
			this.groupCarrier.Controls.Add(this.textCarrier);
			this.groupCarrier.Controls.Add(this.labelElectronicID);
			this.groupCarrier.Controls.Add(this.label21);
			this.groupCarrier.Controls.Add(this.label17);
			this.groupCarrier.Controls.Add(this.textState);
			this.groupCarrier.Controls.Add(this.labelCitySTZip);
			this.groupCarrier.Location = new System.Drawing.Point(10, 42);
			this.groupCarrier.Name = "groupCarrier";
			this.groupCarrier.Size = new System.Drawing.Size(402, 167);
			this.groupCarrier.TabIndex = 1;
			this.groupCarrier.Text = "Carrier";
			// 
			// labelSendElectronically
			// 
			this.labelSendElectronically.Location = new System.Drawing.Point(6, 145);
			this.labelSendElectronically.Name = "labelSendElectronically";
			this.labelSendElectronically.Size = new System.Drawing.Size(100, 17);
			this.labelSendElectronically.TabIndex = 155;
			this.labelSendElectronically.Text = "Send Electronically";
			this.labelSendElectronically.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSendElectronically
			// 
			this.comboSendElectronically.Location = new System.Drawing.Point(107, 144);
			this.comboSendElectronically.Name = "comboSendElectronically";
			this.comboSendElectronically.Size = new System.Drawing.Size(286, 21);
			this.comboSendElectronically.TabIndex = 154;
			// 
			// butPickCarrier
			// 
			this.butPickCarrier.Location = new System.Drawing.Point(376, 16);
			this.butPickCarrier.Name = "butPickCarrier";
			this.butPickCarrier.Size = new System.Drawing.Size(19, 21);
			this.butPickCarrier.TabIndex = 153;
			this.butPickCarrier.Text = "...";
			this.butPickCarrier.Click += new System.EventHandler(this.butPickCarrier_Click);
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(107, 38);
			this.textPhone.MaxLength = 30;
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(157, 20);
			this.textPhone.TabIndex = 1;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(107, 59);
			this.textAddress.MaxLength = 60;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(286, 20);
			this.textAddress.TabIndex = 2;
			this.textAddress.TextChanged += new System.EventHandler(this.textAddress_TextChanged);
			// 
			// comboElectIDdescript
			// 
			this.comboElectIDdescript.Location = new System.Drawing.Point(162, 122);
			this.comboElectIDdescript.Name = "comboElectIDdescript";
			this.comboElectIDdescript.Size = new System.Drawing.Size(159, 20);
			this.comboElectIDdescript.TabIndex = 125;
			this.comboElectIDdescript.SelectedIndexChanged += new System.EventHandler(this.comboElectIDdescript_SelectedIndexChanged);
			// 
			// textElectID
			// 
			this.textElectID.Location = new System.Drawing.Point(107, 122);
			this.textElectID.MaxLength = 20;
			this.textElectID.Name = "textElectID";
			this.textElectID.Size = new System.Drawing.Size(54, 20);
			this.textElectID.TabIndex = 7;
			this.textElectID.Validating += new System.ComponentModel.CancelEventHandler(this.textElectID_Validating);
			// 
			// butSearch
			// 
			this.butSearch.Location = new System.Drawing.Point(322, 122);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(71, 21);
			this.butSearch.TabIndex = 124;
			this.butSearch.Text = "Search IDs";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(107, 80);
			this.textAddress2.MaxLength = 60;
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(286, 20);
			this.textAddress2.TabIndex = 3;
			this.textAddress2.TextChanged += new System.EventHandler(this.textAddress2_TextChanged);
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(322, 101);
			this.textZip.MaxLength = 10;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(71, 20);
			this.textZip.TabIndex = 6;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 60);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(100, 17);
			this.label10.TabIndex = 10;
			this.label10.Text = "Address";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(107, 101);
			this.textCity.MaxLength = 40;
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(153, 20);
			this.textCity.TabIndex = 4;
			this.textCity.TextChanged += new System.EventHandler(this.textCity_TextChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 39);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 17);
			this.label7.TabIndex = 7;
			this.label7.Text = "Phone";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrier
			// 
			this.textCarrier.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textCarrier.Location = new System.Drawing.Point(107, 16);
			this.textCarrier.MaxLength = 50;
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(268, 21);
			this.textCarrier.TabIndex = 0;
			this.textCarrier.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCarrier_KeyUp);
			this.textCarrier.Leave += new System.EventHandler(this.textCarrier_Leave);
			// 
			// labelElectronicID
			// 
			this.labelElectronicID.Location = new System.Drawing.Point(6, 123);
			this.labelElectronicID.Name = "labelElectronicID";
			this.labelElectronicID.Size = new System.Drawing.Size(100, 17);
			this.labelElectronicID.TabIndex = 15;
			this.labelElectronicID.Text = "Electronic ID";
			this.labelElectronicID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(6, 81);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(100, 17);
			this.label21.TabIndex = 79;
			this.label21.Text = "Address 2";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(6, 17);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(100, 17);
			this.label17.TabIndex = 152;
			this.label17.Text = "Carrier";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(261, 101);
			this.textState.MaxLength = 2;
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(60, 20);
			this.textState.TabIndex = 5;
			this.textState.TextChanged += new System.EventHandler(this.textState_TextChanged);
			// 
			// labelCitySTZip
			// 
			this.labelCitySTZip.Location = new System.Drawing.Point(6, 102);
			this.labelCitySTZip.Name = "labelCitySTZip";
			this.labelCitySTZip.Size = new System.Drawing.Size(100, 17);
			this.labelCitySTZip.TabIndex = 11;
			this.labelCitySTZip.Text = "City,ST,Zip";
			this.labelCitySTZip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsMedical
			// 
			this.checkIsMedical.Location = new System.Drawing.Point(117, 4);
			this.checkIsMedical.Name = "checkIsMedical";
			this.checkIsMedical.Size = new System.Drawing.Size(18, 17);
			this.checkIsMedical.TabIndex = 113;
			// 
			// textGroupNum
			// 
			this.textGroupNum.Location = new System.Drawing.Point(117, 233);
			this.textGroupNum.MaxLength = 25;
			this.textGroupNum.Name = "textGroupNum";
			this.textGroupNum.Size = new System.Drawing.Size(160, 20);
			this.textGroupNum.TabIndex = 3;
			// 
			// labelGroupNum
			// 
			this.labelGroupNum.Location = new System.Drawing.Point(6, 234);
			this.labelGroupNum.Name = "labelGroupNum";
			this.labelGroupNum.Size = new System.Drawing.Size(110, 17);
			this.labelGroupNum.TabIndex = 9;
			this.labelGroupNum.Text = "Group Num";
			this.labelGroupNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 213);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(110, 17);
			this.label8.TabIndex = 8;
			this.label8.Text = "Group Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLinked
			// 
			this.comboLinked.Location = new System.Drawing.Point(155, 254);
			this.comboLinked.Name = "comboLinked";
			this.comboLinked.Size = new System.Drawing.Size(248, 21);
			this.comboLinked.TabIndex = 68;
			// 
			// textLinkedNum
			// 
			this.textLinkedNum.BackColor = System.Drawing.Color.White;
			this.textLinkedNum.Location = new System.Drawing.Point(117, 254);
			this.textLinkedNum.Multiline = true;
			this.textLinkedNum.Name = "textLinkedNum";
			this.textLinkedNum.ReadOnly = true;
			this.textLinkedNum.Size = new System.Drawing.Size(37, 21);
			this.textLinkedNum.TabIndex = 67;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(6, 22);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(110, 17);
			this.label16.TabIndex = 73;
			this.label16.Text = "Employer";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 255);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110, 17);
			this.label4.TabIndex = 66;
			this.label4.Text = "Other Subscribers";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDivisionDash
			// 
			this.labelDivisionDash.Location = new System.Drawing.Point(278, 234);
			this.labelDivisionDash.Name = "labelDivisionDash";
			this.labelDivisionDash.Size = new System.Drawing.Size(53, 17);
			this.labelDivisionDash.TabIndex = 111;
			this.labelDivisionDash.Text = "Div. No.";
			this.labelDivisionDash.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMedicalInsurance
			// 
			this.labelMedicalInsurance.Location = new System.Drawing.Point(132, 4);
			this.labelMedicalInsurance.Name = "labelMedicalInsurance";
			this.labelMedicalInsurance.Size = new System.Drawing.Size(110, 17);
			this.labelMedicalInsurance.TabIndex = 192;
			this.labelMedicalInsurance.Text = "Medical Insurance";
			this.labelMedicalInsurance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboPlanType
			// 
			this.comboPlanType.Location = new System.Drawing.Point(126, 306);
			this.comboPlanType.Name = "comboPlanType";
			this.comboPlanType.Size = new System.Drawing.Size(200, 21);
			this.comboPlanType.TabIndex = 149;
			this.comboPlanType.SelectionChangeCommitted += new System.EventHandler(this.comboPlanType_SelectionChangeCommitted);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(15, 307);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(110, 17);
			this.label14.TabIndex = 150;
			this.label14.Text = "Plan Type";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabPageOtherInsInfo
			// 
			this.tabPageOtherInsInfo.Controls.Add(this.panelOrthInfo);
			this.tabPageOtherInsInfo.Location = new System.Drawing.Point(2, 21);
			this.tabPageOtherInsInfo.Name = "tabPageOtherInsInfo";
			this.tabPageOtherInsInfo.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageOtherInsInfo.Size = new System.Drawing.Size(451, 443);
			this.tabPageOtherInsInfo.TabIndex = 1;
			this.tabPageOtherInsInfo.Text = "Other Ins Info";
			// 
			// panelOrthInfo
			// 
			this.panelOrthInfo.AutoScroll = true;
			this.panelOrthInfo.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.panelOrthInfo.BackColor = System.Drawing.Color.White;
			this.panelOrthInfo.Controls.Add(this.butSubstCodes);
			this.panelOrthInfo.Controls.Add(this.label41);
			this.panelOrthInfo.Controls.Add(this.label40);
			this.panelOrthInfo.Controls.Add(this.label12);
			this.panelOrthInfo.Controls.Add(this.groupBoxPerVisitAmounts);
			this.panelOrthInfo.Controls.Add(this.groupBoxOD1);
			this.panelOrthInfo.Controls.Add(this.checkPpoSubWo);
			this.panelOrthInfo.Controls.Add(this.comboBillType);
			this.panelOrthInfo.Controls.Add(this.label38);
			this.panelOrthInfo.Controls.Add(this.comboCobRule);
			this.panelOrthInfo.Controls.Add(this.label20);
			this.panelOrthInfo.Controls.Add(this.comboFilingCodeSubtype);
			this.panelOrthInfo.Controls.Add(this.label15);
			this.panelOrthInfo.Controls.Add(this.checkIsHidden);
			this.panelOrthInfo.Controls.Add(this.checkCodeSubst);
			this.panelOrthInfo.Controls.Add(this.checkShowBaseUnits);
			this.panelOrthInfo.Controls.Add(this.comboFilingCode);
			this.panelOrthInfo.Controls.Add(this.label13);
			this.panelOrthInfo.Controls.Add(this.comboClaimForm);
			this.panelOrthInfo.Controls.Add(this.label23);
			this.panelOrthInfo.Controls.Add(this.checkAlternateCode);
			this.panelOrthInfo.Controls.Add(this.checkClaimsUseUCR);
			this.panelOrthInfo.Location = new System.Drawing.Point(-3, 2);
			this.panelOrthInfo.Name = "panelOrthInfo";
			this.panelOrthInfo.Size = new System.Drawing.Size(454, 438);
			this.panelOrthInfo.TabIndex = 1;
			// 
			// label41
			// 
			this.label41.Location = new System.Drawing.Point(233, 14);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(153, 15);
			this.label41.TabIndex = 198;
			this.label41.Text = "(for some Medicaid plans)";
			this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label40
			// 
			this.label40.Location = new System.Drawing.Point(233, 32);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(153, 15);
			this.label40.TabIndex = 197;
			this.label40.Text = "(e.g. posterior composites)";
			this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(233, 104);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(153, 15);
			this.label12.TabIndex = 196;
			this.label12.Text = "(does not affect billed amt)";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxPerVisitAmounts
			// 
			this.groupBoxPerVisitAmounts.Controls.Add(this.labelPerVisit);
			this.groupBoxPerVisitAmounts.Controls.Add(this.textPerVisitInsAmount);
			this.groupBoxPerVisitAmounts.Controls.Add(this.textPerVisitPatAmount);
			this.groupBoxPerVisitAmounts.Controls.Add(this.labelPerVisitPatAmount);
			this.groupBoxPerVisitAmounts.Controls.Add(this.labelPerVisitInsuranceAmount);
			this.groupBoxPerVisitAmounts.Location = new System.Drawing.Point(72, 347);
			this.groupBoxPerVisitAmounts.Name = "groupBoxPerVisitAmounts";
			this.groupBoxPerVisitAmounts.Size = new System.Drawing.Size(367, 71);
			this.groupBoxPerVisitAmounts.TabIndex = 196;
			this.groupBoxPerVisitAmounts.Text = "Per Visit Amounts";
			// 
			// labelPerVisit
			// 
			this.labelPerVisit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerVisit.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPerVisit.Location = new System.Drawing.Point(216, 3);
			this.labelPerVisit.Name = "labelPerVisit";
			this.labelPerVisit.Size = new System.Drawing.Size(142, 64);
			this.labelPerVisit.TabIndex = 379;
			this.labelPerVisit.Text = "These will not work without first setting Per Vist procedure codes in Preferences" +
    ", Family Ins.";
			this.labelPerVisit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPerVisitInsAmount
			// 
			this.textPerVisitInsAmount.Location = new System.Drawing.Point(146, 44);
			this.textPerVisitInsAmount.MaxVal = 100000000D;
			this.textPerVisitInsAmount.MinVal = 0D;
			this.textPerVisitInsAmount.Name = "textPerVisitInsAmount";
			this.textPerVisitInsAmount.Size = new System.Drawing.Size(64, 20);
			this.textPerVisitInsAmount.TabIndex = 196;
			// 
			// textPerVisitPatAmount
			// 
			this.textPerVisitPatAmount.Location = new System.Drawing.Point(146, 20);
			this.textPerVisitPatAmount.MaxVal = 100000000D;
			this.textPerVisitPatAmount.MinVal = 0D;
			this.textPerVisitPatAmount.Name = "textPerVisitPatAmount";
			this.textPerVisitPatAmount.Size = new System.Drawing.Size(64, 20);
			this.textPerVisitPatAmount.TabIndex = 195;
			// 
			// labelPerVisitPatAmount
			// 
			this.labelPerVisitPatAmount.Location = new System.Drawing.Point(3, 23);
			this.labelPerVisitPatAmount.Name = "labelPerVisitPatAmount";
			this.labelPerVisitPatAmount.Size = new System.Drawing.Size(140, 19);
			this.labelPerVisitPatAmount.TabIndex = 192;
			this.labelPerVisitPatAmount.Text = "Patient Copay";
			this.labelPerVisitPatAmount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelPerVisitInsuranceAmount
			// 
			this.labelPerVisitInsuranceAmount.Location = new System.Drawing.Point(6, 45);
			this.labelPerVisitInsuranceAmount.Name = "labelPerVisitInsuranceAmount";
			this.labelPerVisitInsuranceAmount.Size = new System.Drawing.Size(137, 19);
			this.labelPerVisitInsuranceAmount.TabIndex = 194;
			this.labelPerVisitInsuranceAmount.Text = "Insurance";
			this.labelPerVisitInsuranceAmount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.labelZeroWriteoffAnnualMax);
			this.groupBoxOD1.Controls.Add(this.comboZeroWriteOffsOnLimitationOrAging);
			this.groupBoxOD1.Controls.Add(this.comboBoxZeroWriteOffsOnAnnualMaxLimitation);
			this.groupBoxOD1.Controls.Add(this.label39);
			this.groupBoxOD1.Controls.Add(this.comboExclusionFeeRule);
			this.groupBoxOD1.Controls.Add(this.label11);
			this.groupBoxOD1.Location = new System.Drawing.Point(72, 246);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(367, 95);
			this.groupBoxOD1.TabIndex = 195;
			this.groupBoxOD1.Text = "Write-Offs for Non-Covered Services Overrides";
			// 
			// labelZeroWriteoffAnnualMax
			// 
			this.labelZeroWriteoffAnnualMax.Location = new System.Drawing.Point(8, 47);
			this.labelZeroWriteoffAnnualMax.Name = "labelZeroWriteoffAnnualMax";
			this.labelZeroWriteoffAnnualMax.Size = new System.Drawing.Size(136, 19);
			this.labelZeroWriteoffAnnualMax.TabIndex = 192;
			this.labelZeroWriteoffAnnualMax.Text = "Annual Max";
			this.labelZeroWriteoffAnnualMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboZeroWriteOffsOnLimitationOrAging
			// 
			this.comboZeroWriteOffsOnLimitationOrAging.Location = new System.Drawing.Point(146, 69);
			this.comboZeroWriteOffsOnLimitationOrAging.Name = "comboZeroWriteOffsOnLimitationOrAging";
			this.comboZeroWriteOffsOnLimitationOrAging.Size = new System.Drawing.Size(212, 21);
			this.comboZeroWriteOffsOnLimitationOrAging.TabIndex = 193;
			// 
			// comboBoxZeroWriteOffsOnAnnualMaxLimitation
			// 
			this.comboBoxZeroWriteOffsOnAnnualMaxLimitation.Location = new System.Drawing.Point(146, 45);
			this.comboBoxZeroWriteOffsOnAnnualMaxLimitation.Name = "comboBoxZeroWriteOffsOnAnnualMaxLimitation";
			this.comboBoxZeroWriteOffsOnAnnualMaxLimitation.Size = new System.Drawing.Size(212, 21);
			this.comboBoxZeroWriteOffsOnAnnualMaxLimitation.TabIndex = 191;
			// 
			// label39
			// 
			this.label39.Location = new System.Drawing.Point(11, 71);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(133, 19);
			this.label39.TabIndex = 194;
			this.label39.Text = "Age or Frequency Limits";
			this.label39.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboExclusionFeeRule
			// 
			this.comboExclusionFeeRule.Location = new System.Drawing.Point(146, 21);
			this.comboExclusionFeeRule.Name = "comboExclusionFeeRule";
			this.comboExclusionFeeRule.Size = new System.Drawing.Size(212, 21);
			this.comboExclusionFeeRule.TabIndex = 189;
			this.comboExclusionFeeRule.SelectionChangeCommitted += new System.EventHandler(this.comboExclusionFeeRule_SelectionChangeCommitted);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(7, 23);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(137, 19);
			this.label11.TabIndex = 190;
			this.label11.Text = "Exclusions";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkPpoSubWo
			// 
			this.checkPpoSubWo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPpoSubWo.Location = new System.Drawing.Point(4, 50);
			this.checkPpoSubWo.Name = "checkPpoSubWo";
			this.checkPpoSubWo.Size = new System.Drawing.Size(227, 16);
			this.checkPpoSubWo.TabIndex = 188;
			this.checkPpoSubWo.Text = "PPO substitution calculate writeoffs";
			// 
			// comboBillType
			// 
			this.comboBillType.Location = new System.Drawing.Point(218, 222);
			this.comboBillType.Name = "comboBillType";
			this.comboBillType.Size = new System.Drawing.Size(212, 21);
			this.comboBillType.TabIndex = 185;
			this.comboBillType.SelectionChangeCommitted += new System.EventHandler(this.comboBillType_SelectionChangeCommitted);
			// 
			// label38
			// 
			this.label38.Location = new System.Drawing.Point(74, 224);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(142, 19);
			this.label38.TabIndex = 186;
			this.label38.Text = "Billing Type";
			this.label38.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboCobRule
			// 
			this.comboCobRule.Location = new System.Drawing.Point(218, 150);
			this.comboCobRule.Name = "comboCobRule";
			this.comboCobRule.Size = new System.Drawing.Size(126, 21);
			this.comboCobRule.TabIndex = 183;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(120, 153);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(95, 15);
			this.label20.TabIndex = 184;
			this.label20.Text = "COB Rule";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFilingCodeSubtype
			// 
			this.comboFilingCodeSubtype.Location = new System.Drawing.Point(218, 198);
			this.comboFilingCodeSubtype.Name = "comboFilingCodeSubtype";
			this.comboFilingCodeSubtype.Size = new System.Drawing.Size(212, 21);
			this.comboFilingCodeSubtype.TabIndex = 177;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(106, 200);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(110, 19);
			this.label15.TabIndex = 182;
			this.label15.Text = "Filing Code Subtype";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(152, 86);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(79, 16);
			this.checkIsHidden.TabIndex = 172;
			this.checkIsHidden.Text = "Hidden";
			// 
			// checkCodeSubst
			// 
			this.checkCodeSubst.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCodeSubst.Location = new System.Drawing.Point(66, 32);
			this.checkCodeSubst.Name = "checkCodeSubst";
			this.checkCodeSubst.Size = new System.Drawing.Size(165, 16);
			this.checkCodeSubst.TabIndex = 170;
			this.checkCodeSubst.Text = "Don\'t Substitute Codes";
			// 
			// checkShowBaseUnits
			// 
			this.checkShowBaseUnits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowBaseUnits.Location = new System.Drawing.Point(56, 104);
			this.checkShowBaseUnits.Name = "checkShowBaseUnits";
			this.checkShowBaseUnits.Size = new System.Drawing.Size(175, 16);
			this.checkShowBaseUnits.TabIndex = 178;
			this.checkShowBaseUnits.Text = "Claims show base units";
			// 
			// comboFilingCode
			// 
			this.comboFilingCode.Location = new System.Drawing.Point(218, 174);
			this.comboFilingCode.Name = "comboFilingCode";
			this.comboFilingCode.Size = new System.Drawing.Size(212, 21);
			this.comboFilingCode.TabIndex = 176;
			this.comboFilingCode.SelectionChangeCommitted += new System.EventHandler(this.comboFilingCode_SelectionChangeCommitted);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(116, 176);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(100, 19);
			this.label13.TabIndex = 181;
			this.label13.Text = "Filing Code";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboClaimForm
			// 
			this.comboClaimForm.Location = new System.Drawing.Point(218, 126);
			this.comboClaimForm.Name = "comboClaimForm";
			this.comboClaimForm.Size = new System.Drawing.Size(212, 21);
			this.comboClaimForm.TabIndex = 174;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(120, 129);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(95, 15);
			this.label23.TabIndex = 180;
			this.label23.Text = "Claim Form";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAlternateCode
			// 
			this.checkAlternateCode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAlternateCode.Location = new System.Drawing.Point(86, 14);
			this.checkAlternateCode.Name = "checkAlternateCode";
			this.checkAlternateCode.Size = new System.Drawing.Size(145, 16);
			this.checkAlternateCode.TabIndex = 169;
			this.checkAlternateCode.Text = "Use Alternate Code";
			// 
			// checkClaimsUseUCR
			// 
			this.checkClaimsUseUCR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsUseUCR.Location = new System.Drawing.Point(16, 68);
			this.checkClaimsUseUCR.Name = "checkClaimsUseUCR";
			this.checkClaimsUseUCR.Size = new System.Drawing.Size(215, 16);
			this.checkClaimsUseUCR.TabIndex = 171;
			this.checkClaimsUseUCR.Text = "Claims show UCR fee, not billed fee";
			// 
			// tabPageCanadian
			// 
			this.tabPageCanadian.Controls.Add(this.panelCanadian);
			this.tabPageCanadian.Location = new System.Drawing.Point(2, 21);
			this.tabPageCanadian.Name = "tabPageCanadian";
			this.tabPageCanadian.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCanadian.Size = new System.Drawing.Size(451, 443);
			this.tabPageCanadian.TabIndex = 2;
			this.tabPageCanadian.Text = "Canadian";
			// 
			// panelCanadian
			// 
			this.panelCanadian.AutoScroll = true;
			this.panelCanadian.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.panelCanadian.BackColor = System.Drawing.Color.White;
			this.panelCanadian.Controls.Add(this.groupCanadian);
			this.panelCanadian.Location = new System.Drawing.Point(-3, 1);
			this.panelCanadian.Name = "panelCanadian";
			this.panelCanadian.Size = new System.Drawing.Size(454, 438);
			this.panelCanadian.TabIndex = 2;
			// 
			// groupCanadian
			// 
			this.groupCanadian.Controls.Add(this.label19);
			this.groupCanadian.Controls.Add(this.textCanadianInstCode);
			this.groupCanadian.Controls.Add(this.label9);
			this.groupCanadian.Controls.Add(this.textCanadianDiagCode);
			this.groupCanadian.Controls.Add(this.checkIsPMP);
			this.groupCanadian.Controls.Add(this.label24);
			this.groupCanadian.Controls.Add(this.label22);
			this.groupCanadian.Controls.Add(this.textPlanFlag);
			this.groupCanadian.Controls.Add(this.textDentaide);
			this.groupCanadian.Controls.Add(this.labelDentaide);
			this.groupCanadian.Location = new System.Drawing.Point(14, 14);
			this.groupCanadian.Name = "groupCanadian";
			this.groupCanadian.Size = new System.Drawing.Size(404, 129);
			this.groupCanadian.TabIndex = 13;
			this.groupCanadian.Text = "Canadian";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(37, 106);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(140, 19);
			this.label19.TabIndex = 173;
			this.label19.Text = "Institution Code";
			this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCanadianInstCode
			// 
			this.textCanadianInstCode.Location = new System.Drawing.Point(181, 103);
			this.textCanadianInstCode.MaxLength = 20;
			this.textCanadianInstCode.Name = "textCanadianInstCode";
			this.textCanadianInstCode.Size = new System.Drawing.Size(88, 20);
			this.textCanadianInstCode.TabIndex = 172;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(37, 85);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(140, 19);
			this.label9.TabIndex = 171;
			this.label9.Text = "Diagnostic Code";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCanadianDiagCode
			// 
			this.textCanadianDiagCode.Location = new System.Drawing.Point(181, 82);
			this.textCanadianDiagCode.MaxLength = 20;
			this.textCanadianDiagCode.Name = "textCanadianDiagCode";
			this.textCanadianDiagCode.Size = new System.Drawing.Size(88, 20);
			this.textCanadianDiagCode.TabIndex = 170;
			// 
			// checkIsPMP
			// 
			this.checkIsPMP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPMP.Location = new System.Drawing.Point(18, 62);
			this.checkIsPMP.Name = "checkIsPMP";
			this.checkIsPMP.Size = new System.Drawing.Size(178, 17);
			this.checkIsPMP.TabIndex = 169;
			this.checkIsPMP.Text = "Is Provincial Medical Plan";
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(221, 39);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(140, 19);
			this.label24.TabIndex = 168;
			this.label24.Text = "A, V, N, or blank";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(37, 41);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(140, 19);
			this.label22.TabIndex = 167;
			this.label22.Text = "Plan Flag";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPlanFlag
			// 
			this.textPlanFlag.Location = new System.Drawing.Point(181, 38);
			this.textPlanFlag.MaxLength = 20;
			this.textPlanFlag.Name = "textPlanFlag";
			this.textPlanFlag.Size = new System.Drawing.Size(37, 20);
			this.textPlanFlag.TabIndex = 1;
			// 
			// textDentaide
			// 
			this.textDentaide.Location = new System.Drawing.Point(181, 17);
			this.textDentaide.Name = "textDentaide";
			this.textDentaide.ShowZero = false;
			this.textDentaide.Size = new System.Drawing.Size(37, 20);
			this.textDentaide.TabIndex = 0;
			// 
			// labelDentaide
			// 
			this.labelDentaide.Location = new System.Drawing.Point(37, 20);
			this.labelDentaide.Name = "labelDentaide";
			this.labelDentaide.Size = new System.Drawing.Size(140, 19);
			this.labelDentaide.TabIndex = 160;
			this.labelDentaide.Text = "Dentaide Card Sequence";
			this.labelDentaide.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tabPageOrtho
			// 
			this.tabPageOrtho.Controls.Add(this.panelOrtho);
			this.tabPageOrtho.Location = new System.Drawing.Point(2, 21);
			this.tabPageOrtho.Name = "tabPageOrtho";
			this.tabPageOrtho.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageOrtho.Size = new System.Drawing.Size(451, 443);
			this.tabPageOrtho.TabIndex = 3;
			this.tabPageOrtho.Text = "Ortho";
			// 
			// panelOrtho
			// 
			this.panelOrtho.AutoScroll = true;
			this.panelOrtho.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.panelOrtho.BackColor = System.Drawing.Color.White;
			this.panelOrtho.Controls.Add(this.textOrthoAutoFee);
			this.panelOrtho.Controls.Add(this.labelOrthoAutoFee);
			this.panelOrtho.Controls.Add(this.butDefaultAutoOrthoProc);
			this.panelOrtho.Controls.Add(this.butPickOrthoProc);
			this.panelOrtho.Controls.Add(this.textOrthoAutoProc);
			this.panelOrtho.Controls.Add(this.label37);
			this.panelOrtho.Controls.Add(this.comboOrthoClaimType);
			this.panelOrtho.Controls.Add(this.comboOrthoAutoProcPeriod);
			this.panelOrtho.Controls.Add(this.labelAutoOrthoProcPeriod);
			this.panelOrtho.Controls.Add(this.label36);
			this.panelOrtho.Controls.Add(this.checkOrthoWaitDays);
			this.panelOrtho.Location = new System.Drawing.Point(-4, 1);
			this.panelOrtho.Name = "panelOrtho";
			this.panelOrtho.Size = new System.Drawing.Size(454, 438);
			this.panelOrtho.TabIndex = 3;
			// 
			// textOrthoAutoFee
			// 
			this.textOrthoAutoFee.Location = new System.Drawing.Point(153, 58);
			this.textOrthoAutoFee.MaxVal = 100000000D;
			this.textOrthoAutoFee.MinVal = -100000000D;
			this.textOrthoAutoFee.Name = "textOrthoAutoFee";
			this.textOrthoAutoFee.Size = new System.Drawing.Size(133, 20);
			this.textOrthoAutoFee.TabIndex = 183;
			// 
			// labelOrthoAutoFee
			// 
			this.labelOrthoAutoFee.Location = new System.Drawing.Point(13, 59);
			this.labelOrthoAutoFee.Name = "labelOrthoAutoFee";
			this.labelOrthoAutoFee.Size = new System.Drawing.Size(140, 19);
			this.labelOrthoAutoFee.TabIndex = 182;
			this.labelOrthoAutoFee.Text = "Ortho Auto Fee";
			this.labelOrthoAutoFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDefaultAutoOrthoProc
			// 
			this.butDefaultAutoOrthoProc.Location = new System.Drawing.Point(313, 36);
			this.butDefaultAutoOrthoProc.Name = "butDefaultAutoOrthoProc";
			this.butDefaultAutoOrthoProc.Size = new System.Drawing.Size(52, 20);
			this.butDefaultAutoOrthoProc.TabIndex = 180;
			this.butDefaultAutoOrthoProc.Text = "Default";
			this.butDefaultAutoOrthoProc.Click += new System.EventHandler(this.butDefaultAutoOrthoProc_Click);
			// 
			// butPickOrthoProc
			// 
			this.butPickOrthoProc.Location = new System.Drawing.Point(289, 36);
			this.butPickOrthoProc.Name = "butPickOrthoProc";
			this.butPickOrthoProc.Size = new System.Drawing.Size(21, 20);
			this.butPickOrthoProc.TabIndex = 179;
			this.butPickOrthoProc.Text = "...";
			this.butPickOrthoProc.Click += new System.EventHandler(this.butPickOrthoProc_Click);
			// 
			// textOrthoAutoProc
			// 
			this.textOrthoAutoProc.Location = new System.Drawing.Point(153, 36);
			this.textOrthoAutoProc.Name = "textOrthoAutoProc";
			this.textOrthoAutoProc.ReadOnly = true;
			this.textOrthoAutoProc.Size = new System.Drawing.Size(133, 20);
			this.textOrthoAutoProc.TabIndex = 178;
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(12, 37);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(140, 19);
			this.label37.TabIndex = 177;
			this.label37.Text = "Ortho Auto Proc";
			this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboOrthoClaimType
			// 
			this.comboOrthoClaimType.Location = new System.Drawing.Point(153, 12);
			this.comboOrthoClaimType.Name = "comboOrthoClaimType";
			this.comboOrthoClaimType.Size = new System.Drawing.Size(212, 21);
			this.comboOrthoClaimType.TabIndex = 175;
			this.comboOrthoClaimType.SelectionChangeCommitted += new System.EventHandler(this.comboOrthoClaimType_SelectionChangeCommitted);
			// 
			// comboOrthoAutoProcPeriod
			// 
			this.comboOrthoAutoProcPeriod.Location = new System.Drawing.Point(153, 81);
			this.comboOrthoAutoProcPeriod.Name = "comboOrthoAutoProcPeriod";
			this.comboOrthoAutoProcPeriod.Size = new System.Drawing.Size(212, 21);
			this.comboOrthoAutoProcPeriod.TabIndex = 176;
			// 
			// labelAutoOrthoProcPeriod
			// 
			this.labelAutoOrthoProcPeriod.Location = new System.Drawing.Point(12, 83);
			this.labelAutoOrthoProcPeriod.Name = "labelAutoOrthoProcPeriod";
			this.labelAutoOrthoProcPeriod.Size = new System.Drawing.Size(140, 19);
			this.labelAutoOrthoProcPeriod.TabIndex = 163;
			this.labelAutoOrthoProcPeriod.Text = "Auto Proc Period";
			this.labelAutoOrthoProcPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label36
			// 
			this.label36.Location = new System.Drawing.Point(12, 14);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(140, 19);
			this.label36.TabIndex = 161;
			this.label36.Text = "Ortho Claim Type";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkOrthoWaitDays
			// 
			this.checkOrthoWaitDays.Enabled = false;
			this.checkOrthoWaitDays.Location = new System.Drawing.Point(8, 105);
			this.checkOrthoWaitDays.Name = "checkOrthoWaitDays";
			this.checkOrthoWaitDays.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkOrthoWaitDays.Size = new System.Drawing.Size(355, 21);
			this.checkOrthoWaitDays.TabIndex = 0;
			this.checkOrthoWaitDays.Text = "Wait 30 days before creating the first automatic claim";
			// 
			// gridBenefits
			// 
			this.gridBenefits.Location = new System.Drawing.Point(468, 337);
			this.gridBenefits.Name = "gridBenefits";
			this.gridBenefits.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridBenefits.Size = new System.Drawing.Size(504, 316);
			this.gridBenefits.TabIndex = 146;
			this.gridBenefits.Title = "Benefit Information";
			this.gridBenefits.TranslationName = "TableBenefits";
			this.gridBenefits.DoubleClick += new System.EventHandler(this.gridBenefits_DoubleClick);
			// 
			// textPlanNote
			// 
			this.textPlanNote.AcceptsTab = true;
			this.textPlanNote.BackColor = System.Drawing.SystemColors.Window;
			this.textPlanNote.DetectLinksEnabled = false;
			this.textPlanNote.DetectUrls = false;
			this.textPlanNote.Location = new System.Drawing.Point(14, 581);
			this.textPlanNote.Name = "textPlanNote";
			this.textPlanNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
			this.textPlanNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPlanNote.Size = new System.Drawing.Size(395, 85);
			this.textPlanNote.TabIndex = 1;
			this.textPlanNote.Text = "1 - InsPlan\n2\n3 lines will show here in 46 vert.\n4 lines will show here in 59 ver" +
    "t.\n5 lines in 72 vert\n6 in 85";
			// 
			// FormInsPlan
			// 
			this.ClientSize = new System.Drawing.Size(984, 696);
			this.Controls.Add(this.tabControlInsPlan);
			this.Controls.Add(this.checkDontVerify);
			this.Controls.Add(this.label34);
			this.Controls.Add(this.butVerifyBenefits);
			this.Controls.Add(this.textDateLastVerifiedBenefits);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridBenefits);
			this.Controls.Add(this.groupChanges);
			this.Controls.Add(this.textPlanNote);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.panelPat);
			this.Controls.Add(this.butLabel);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupRequestBen);
			this.Controls.Add(this.groupSubscriber);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInsPlan";
			this.ShowInTaskbar = false;
			this.Text = "Edit Insurance Plan";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormInsPlan_Closing);
			this.Load += new System.EventHandler(this.FormInsPlan_Load);
			this.groupSubscriber.ResumeLayout(false);
			this.groupSubscriber.PerformLayout();
			this.groupRequestBen.ResumeLayout(false);
			this.groupRequestBen.PerformLayout();
			this.panelPat.ResumeLayout(false);
			this.panelPat.PerformLayout();
			this.groupChanges.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabControlInsPlan.ResumeLayout(false);
			this.tabPageInsPlanInfo.ResumeLayout(false);
			this.panelPlan.ResumeLayout(false);
			this.panelPlan.PerformLayout();
			this.groupCarrierAllowedAmounts.ResumeLayout(false);
			this.groupCoPay.ResumeLayout(false);
			this.groupPlan.ResumeLayout(false);
			this.groupPlan.PerformLayout();
			this.groupCarrier.ResumeLayout(false);
			this.groupCarrier.PerformLayout();
			this.tabPageOtherInsInfo.ResumeLayout(false);
			this.panelOrthInfo.ResumeLayout(false);
			this.groupBoxPerVisitAmounts.ResumeLayout(false);
			this.groupBoxPerVisitAmounts.PerformLayout();
			this.groupBoxOD1.ResumeLayout(false);
			this.tabPageCanadian.ResumeLayout(false);
			this.panelCanadian.ResumeLayout(false);
			this.groupCanadian.ResumeLayout(false);
			this.groupCanadian.PerformLayout();
			this.tabPageOrtho.ResumeLayout(false);
			this.panelOrtho.ResumeLayout(false);
			this.panelOrtho.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label28;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.ValidDate textDateEffect;
		private OpenDental.ValidDate textDateTerm;
		private OpenDental.UI.CheckBox checkRelease;
		private System.Windows.Forms.TextBox textSubscriber;
		private OpenDental.UI.GroupBox groupSubscriber;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textSubscriberID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip toolTip1;
		private OpenDental.UI.CheckBox checkAssign;
		private System.Windows.Forms.Label labelDrop;
		private OpenDental.UI.Button butDrop;
		private OpenDental.ODtextBox textSubscNote;
		private OpenDental.UI.Button butLabel;
		private OpenDental.UI.GroupBox groupRequestBen;
		private System.Windows.Forms.Label labelTrojanID;
		private System.Windows.Forms.TextBox textTrojanID;
		private OpenDental.UI.Button butImportTrojan;
		private OpenDental.UI.Button butIapFind;
		private OpenDental.UI.Button butBenefitNotes;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label labelPatID;
		private OpenDental.UI.ComboBox comboRelationship;
		private OpenDental.UI.CheckBox checkIsPending;
		private System.Windows.Forms.TextBox textPatID;
		private OpenDental.UI.Button butAdjAdd;
		private OpenDental.UI.ListBox listAdj;
		private System.Windows.Forms.Panel panelPat;
		private OpenDental.ValidNum textOrdinal;
		private OpenDental.UI.GridOD gridBenefits;
		private OpenDental.UI.Button butPick;
		private ODtextBox textPlanNote;
		private Label label18;
		private OpenDental.UI.Button butGetElectronic;
		private TextBox textElectBenLastDate;
		private Label labelHistElect;
		private OpenDental.UI.Button butHistoryElect;
		private RadioButton radioChangeAll;
		private RadioButton radioCreateNew;
		private UI.Button butChange;
		private UI.Button butAudit;
		private TextBox textPatPlanNum;
		private Label label27;
		private UI.Button butVerifyPatPlan;
		private ValidDate textDateLastVerifiedPatPlan;
		private UI.Button butVerifyBenefits;
		private ValidDate textDateLastVerifiedBenefits;
		private Label label30;
		private Label label34;
		private OpenDental.UI.CheckBox checkDontVerify;
		private OpenDental.UI.TabControl tabControlInsPlan;
		private OpenDental.UI.TabPage tabPageInsPlanInfo;
		private Panel panelPlan;
		private TextBox textInsPlanNum;
		private Label label29;
		private UI.Button butOtherSubscribers;
		private TextBox textBIN;
		private Label labelBIN;
		private TextBox textDivisionNo;
		private TextBox textGroupName;
		private TextBox textEmployer;
		private UI.Button butPickCarrier;
		private ValidPhone textPhone;
		private TextBox textAddress;
		private OpenDental.UI.ComboBox comboElectIDdescript;
		private TextBox textElectID;
		private UI.Button butSearch;
		private TextBox textAddress2;
		private TextBox textZip;
		private Label label10;
		private TextBox textCity;
		private Label label7;
		private TextBox textCarrier;
		private Label labelElectronicID;
		private Label label21;
		private Label label17;
		private TextBox textState;
		private Label labelCitySTZip;
		private OpenDental.UI.CheckBox checkIsMedical;
		private TextBox textGroupNum;
		private Label labelGroupNum;
		private Label label8;
		private OpenDental.UI.ComboBox comboLinked;
		private TextBox textLinkedNum;
		private Label label16;
		private Label label4;
		private Label labelDivisionDash;
		private OpenDental.UI.ComboBox comboPlanType;
		private Label label14;
		private OpenDental.UI.TabPage tabPageOtherInsInfo;
		private Panel panelOrthInfo;
		private OpenDental.UI.ComboBox comboCobRule;
		private Label label20;
		private UI.ComboBox comboFilingCodeSubtype;
		private Label label15;
		private OpenDental.UI.CheckBox checkIsHidden;
		private OpenDental.UI.CheckBox checkCodeSubst;
		private OpenDental.UI.CheckBox checkShowBaseUnits;
		private UI.ComboBox comboFilingCode;
		private Label label13;
		private UI.ComboBox comboClaimForm;
		private Label label23;
		private OpenDental.UI.CheckBox checkAlternateCode;
		private OpenDental.UI.CheckBox checkClaimsUseUCR;
		private OpenDental.UI.TabPage tabPageCanadian;
		private Panel panelCanadian;
		private Label label19;
		private TextBox textCanadianInstCode;
		private Label label9;
		private TextBox textCanadianDiagCode;
		private OpenDental.UI.CheckBox checkIsPMP;
		private Label label24;
		private Label label22;
		private TextBox textPlanFlag;
		private ValidNum textDentaide;
		private Label labelDentaide;
		private OpenDental.UI.TabPage tabPageOrtho;
		private Panel panelOrtho;
		private Label labelAutoOrthoProcPeriod;
		private Label label36;
		private OpenDental.UI.CheckBox checkOrthoWaitDays;
		private OpenDental.UI.ComboBox comboOrthoClaimType;
		private OpenDental.UI.ComboBox comboOrthoAutoProcPeriod;
		private UI.Button butPatOrtho;
		private UI.Button butPickOrthoProc;
		private TextBox textOrthoAutoProc;
		private Label label37;
		private UI.Button butAuditPat;
		private Label labelManualBlueBook;
		private UI.ComboBox comboManualBlueBook;
		private UI.ComboBox comboSendElectronically;
		private Label labelSendElectronically;
		private OpenDental.UI.CheckBox checkPpoSubWo;
		private UI.Button butHistory;
		private OpenDental.UI.ComboBox comboExclusionFeeRule;
		private Label label11;
		private UI.Button butDefaultAutoOrthoProc;
		private Label labelOrthoAutoFee;
		private ValidDouble textOrthoAutoFee;
		private UI.ComboBox comboFeeSched;
		private Label labelOutOfNetwork;
		private UI.ComboBox comboOutOfNetwork;
		private Label labelCopayFeeSched;
		private Label label3;
		private UI.ComboBox comboCopay;
		private Label label1;
		private UI.ComboBox comboBillType;
		private Label label38;
		private UI.Button butSubstCodes;
		private OpenDental.UI.GroupBox groupChanges;
		private OpenDental.UI.GroupBox groupBox1;
		private OpenDental.UI.GroupBox groupPlan;
		private OpenDental.UI.GroupBox groupCarrier;
		private OpenDental.UI.GroupBox groupCanadian;
		private OpenDental.UI.GroupBox groupCarrierAllowedAmounts;
		private OpenDental.UI.GroupBox groupCoPay;
		private OpenDental.UI.CheckBox checkUseBlueBook;
		private UI.ComboBox comboZeroWriteOffsOnLimitationOrAging;
		private Label label39;
		private UI.ComboBox comboBoxZeroWriteOffsOnAnnualMaxLimitation;
		private Label labelZeroWriteoffAnnualMax;
		private UI.GroupBox groupBoxOD1;
		private Label labelMedicalInsurance;
		private OpenDental.UI.GroupBox groupBoxPerVisitAmounts;
		private Label labelPerVisitPatAmount;
		private Label labelPerVisitInsuranceAmount;
		private ValidDouble textPerVisitInsAmount;
		private ValidDouble textPerVisitPatAmount;
		private Label label41;
		private Label label40;
		private Label label12;
		private Label labelPerVisit;
	}
}
