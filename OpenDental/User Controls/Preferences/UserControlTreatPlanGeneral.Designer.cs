﻿
namespace OpenDental {
	partial class UserControlTreatPlanGeneral {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.groupTreatPlan = new OpenDental.UI.GroupBox();
			this.checkTreatPlanShowCompleted = new OpenDental.UI.CheckBox();
			this.checkTreatPlanItemized = new OpenDental.UI.CheckBox();
			this.checkTPSaveSigned = new OpenDental.UI.CheckBox();
			this.checkPromptSaveTP = new OpenDental.UI.CheckBox();
			this.groupBoxDiscounts = new OpenDental.UI.GroupBox();
			this.comboProcDiscountType = new OpenDental.UI.ComboBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textDiscountPercentage = new System.Windows.Forms.TextBox();
			this.labelDiscountPercentage = new System.Windows.Forms.Label();
			this.groupBoxInsuranceHistory = new OpenDental.UI.GroupBox();
			this.textInsHistBW = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioUR = new System.Windows.Forms.Label();
			this.labelInsHistPerioUL = new System.Windows.Forms.Label();
			this.labelInsHistPerioLR = new System.Windows.Forms.Label();
			this.labelInsHistProphy = new System.Windows.Forms.Label();
			this.labelInsHistPerioLL = new System.Windows.Forms.Label();
			this.textInsHistProphy = new System.Windows.Forms.TextBox();
			this.labelInsHistDebridement = new System.Windows.Forms.Label();
			this.labelInsHistPerioMaint = new System.Windows.Forms.Label();
			this.labelInsHistBW = new System.Windows.Forms.Label();
			this.labelInsHistExam = new System.Windows.Forms.Label();
			this.textInsHistFMX = new System.Windows.Forms.TextBox();
			this.textInsHistPerioLR = new System.Windows.Forms.TextBox();
			this.textInsHistExam = new System.Windows.Forms.TextBox();
			this.textInsHistDebridement = new System.Windows.Forms.TextBox();
			this.textInsHistPerioMaint = new System.Windows.Forms.TextBox();
			this.textInsHistPerioUL = new System.Windows.Forms.TextBox();
			this.labelInsHistFMX = new System.Windows.Forms.Label();
			this.textInsHistPerioLL = new System.Windows.Forms.TextBox();
			this.textInsHistPerioUR = new System.Windows.Forms.TextBox();
			this.groupTreatPlanSort = new OpenDental.UI.GroupBox();
			this.radioTreatPlanSortTooth = new System.Windows.Forms.RadioButton();
			this.radioTreatPlanSortOrder = new System.Windows.Forms.RadioButton();
			this.textTreatNote = new OpenDental.ODtextBox();
			this.groupTreatPlan.SuspendLayout();
			this.groupBoxDiscounts.SuspendLayout();
			this.groupBoxInsuranceHistory.SuspendLayout();
			this.groupTreatPlanSort.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 52);
			this.label1.TabIndex = 242;
			this.label1.Text = "Default Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupTreatPlan
			// 
			this.groupTreatPlan.Controls.Add(this.checkTreatPlanShowCompleted);
			this.groupTreatPlan.Controls.Add(this.checkTreatPlanItemized);
			this.groupTreatPlan.Controls.Add(this.checkTPSaveSigned);
			this.groupTreatPlan.Controls.Add(this.checkPromptSaveTP);
			this.groupTreatPlan.Location = new System.Drawing.Point(20, 80);
			this.groupTreatPlan.Name = "groupTreatPlan";
			this.groupTreatPlan.Size = new System.Drawing.Size(450, 94);
			this.groupTreatPlan.TabIndex = 367;
			this.groupTreatPlan.Text = "Treatment Plan";
			// 
			// checkTreatPlanShowCompleted
			// 
			this.checkTreatPlanShowCompleted.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTreatPlanShowCompleted.Location = new System.Drawing.Point(90, 10);
			this.checkTreatPlanShowCompleted.Name = "checkTreatPlanShowCompleted";
			this.checkTreatPlanShowCompleted.Size = new System.Drawing.Size(350, 17);
			this.checkTreatPlanShowCompleted.TabIndex = 243;
			this.checkTreatPlanShowCompleted.Text = "Show completed work on graphical tooth chart";
			// 
			// checkTreatPlanItemized
			// 
			this.checkTreatPlanItemized.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTreatPlanItemized.Location = new System.Drawing.Point(152, 29);
			this.checkTreatPlanItemized.Name = "checkTreatPlanItemized";
			this.checkTreatPlanItemized.Size = new System.Drawing.Size(288, 17);
			this.checkTreatPlanItemized.TabIndex = 246;
			this.checkTreatPlanItemized.Text = "Show itemized fees";
			// 
			// checkTPSaveSigned
			// 
			this.checkTPSaveSigned.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTPSaveSigned.Location = new System.Drawing.Point(138, 48);
			this.checkTPSaveSigned.Name = "checkTPSaveSigned";
			this.checkTPSaveSigned.Size = new System.Drawing.Size(302, 17);
			this.checkTPSaveSigned.TabIndex = 247;
			this.checkTPSaveSigned.Text = "Save signed Treatment Plans to PDF";
			// 
			// checkPromptSaveTP
			// 
			this.checkPromptSaveTP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptSaveTP.Location = new System.Drawing.Point(62, 67);
			this.checkPromptSaveTP.Name = "checkPromptSaveTP";
			this.checkPromptSaveTP.Size = new System.Drawing.Size(378, 17);
			this.checkPromptSaveTP.TabIndex = 252;
			this.checkPromptSaveTP.Text = "When saving Treatment Plans, prompt for name change";
			// 
			// groupBoxDiscounts
			// 
			this.groupBoxDiscounts.Controls.Add(this.comboProcDiscountType);
			this.groupBoxDiscounts.Controls.Add(this.label19);
			this.groupBoxDiscounts.Controls.Add(this.textDiscountPercentage);
			this.groupBoxDiscounts.Controls.Add(this.labelDiscountPercentage);
			this.groupBoxDiscounts.Location = new System.Drawing.Point(20, 239);
			this.groupBoxDiscounts.Name = "groupBoxDiscounts";
			this.groupBoxDiscounts.Size = new System.Drawing.Size(450, 65);
			this.groupBoxDiscounts.TabIndex = 274;
			this.groupBoxDiscounts.Text = "Discounts";
			// 
			// comboProcDiscountType
			// 
			this.comboProcDiscountType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProcDiscountType.Location = new System.Drawing.Point(277, 10);
			this.comboProcDiscountType.Name = "comboProcDiscountType";
			this.comboProcDiscountType.Size = new System.Drawing.Size(163, 21);
			this.comboProcDiscountType.TabIndex = 244;
			// 
			// label19
			// 
			this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label19.Location = new System.Drawing.Point(74, 13);
			this.label19.Margin = new System.Windows.Forms.Padding(0);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(200, 17);
			this.label19.TabIndex = 250;
			this.label19.Text = "Procedure discount adj type";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountPercentage
			// 
			this.textDiscountPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountPercentage.Location = new System.Drawing.Point(387, 35);
			this.textDiscountPercentage.Name = "textDiscountPercentage";
			this.textDiscountPercentage.Size = new System.Drawing.Size(53, 20);
			this.textDiscountPercentage.TabIndex = 245;
			// 
			// labelDiscountPercentage
			// 
			this.labelDiscountPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountPercentage.Location = new System.Drawing.Point(138, 38);
			this.labelDiscountPercentage.Margin = new System.Windows.Forms.Padding(0);
			this.labelDiscountPercentage.Name = "labelDiscountPercentage";
			this.labelDiscountPercentage.Size = new System.Drawing.Size(246, 17);
			this.labelDiscountPercentage.TabIndex = 251;
			this.labelDiscountPercentage.Text = "Default Procedure discount percentage";
			this.labelDiscountPercentage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBoxInsuranceHistory
			// 
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistBW);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistPerioUR);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistPerioUL);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistPerioLR);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistProphy);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistPerioLL);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistProphy);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistDebridement);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistPerioMaint);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistBW);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistExam);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistFMX);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistPerioLR);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistExam);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistDebridement);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistPerioMaint);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistPerioUL);
			this.groupBoxInsuranceHistory.Controls.Add(this.labelInsHistFMX);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistPerioLL);
			this.groupBoxInsuranceHistory.Controls.Add(this.textInsHistPerioUR);
			this.groupBoxInsuranceHistory.Location = new System.Drawing.Point(20, 309);
			this.groupBoxInsuranceHistory.Name = "groupBoxInsuranceHistory";
			this.groupBoxInsuranceHistory.Size = new System.Drawing.Size(450, 256);
			this.groupBoxInsuranceHistory.TabIndex = 273;
			this.groupBoxInsuranceHistory.Text = "Insurance History";
			// 
			// textInsHistBW
			// 
			this.textInsHistBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistBW.Location = new System.Drawing.Point(341, 10);
			this.textInsHistBW.Name = "textInsHistBW";
			this.textInsHistBW.Size = new System.Drawing.Size(99, 20);
			this.textInsHistBW.TabIndex = 253;
			// 
			// labelInsHistPerioUR
			// 
			this.labelInsHistPerioUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioUR.Location = new System.Drawing.Point(197, 229);
			this.labelInsHistPerioUR.Name = "labelInsHistPerioUR";
			this.labelInsHistPerioUR.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistPerioUR.TabIndex = 268;
			this.labelInsHistPerioUR.Text = "Perio Scaling UR Code";
			this.labelInsHistPerioUR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistPerioUL
			// 
			this.labelInsHistPerioUL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioUL.Location = new System.Drawing.Point(197, 205);
			this.labelInsHistPerioUL.Name = "labelInsHistPerioUL";
			this.labelInsHistPerioUL.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistPerioUL.TabIndex = 269;
			this.labelInsHistPerioUL.Text = "Perio Scaling UL Code";
			this.labelInsHistPerioUL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistPerioLR
			// 
			this.labelInsHistPerioLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioLR.Location = new System.Drawing.Point(197, 181);
			this.labelInsHistPerioLR.Name = "labelInsHistPerioLR";
			this.labelInsHistPerioLR.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistPerioLR.TabIndex = 271;
			this.labelInsHistPerioLR.Text = "Perio Scaling LR Code";
			this.labelInsHistPerioLR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistProphy
			// 
			this.labelInsHistProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistProphy.Location = new System.Drawing.Point(197, 85);
			this.labelInsHistProphy.Name = "labelInsHistProphy";
			this.labelInsHistProphy.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistProphy.TabIndex = 272;
			this.labelInsHistProphy.Text = "Prophylaxis Code";
			this.labelInsHistProphy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistPerioLL
			// 
			this.labelInsHistPerioLL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioLL.Location = new System.Drawing.Point(197, 157);
			this.labelInsHistPerioLL.Name = "labelInsHistPerioLL";
			this.labelInsHistPerioLL.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistPerioLL.TabIndex = 270;
			this.labelInsHistPerioLL.Text = "Perio Scaling LL Code";
			this.labelInsHistPerioLL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistProphy
			// 
			this.textInsHistProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistProphy.Location = new System.Drawing.Point(341, 82);
			this.textInsHistProphy.Name = "textInsHistProphy";
			this.textInsHistProphy.Size = new System.Drawing.Size(99, 20);
			this.textInsHistProphy.TabIndex = 256;
			// 
			// labelInsHistDebridement
			// 
			this.labelInsHistDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistDebridement.Location = new System.Drawing.Point(197, 109);
			this.labelInsHistDebridement.Name = "labelInsHistDebridement";
			this.labelInsHistDebridement.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistDebridement.TabIndex = 265;
			this.labelInsHistDebridement.Text = "Full Debridement Code";
			this.labelInsHistDebridement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistPerioMaint
			// 
			this.labelInsHistPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioMaint.Location = new System.Drawing.Point(197, 133);
			this.labelInsHistPerioMaint.Name = "labelInsHistPerioMaint";
			this.labelInsHistPerioMaint.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistPerioMaint.TabIndex = 266;
			this.labelInsHistPerioMaint.Text = "Perio Maintenance Code";
			this.labelInsHistPerioMaint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistBW
			// 
			this.labelInsHistBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistBW.Location = new System.Drawing.Point(197, 13);
			this.labelInsHistBW.Name = "labelInsHistBW";
			this.labelInsHistBW.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistBW.TabIndex = 263;
			this.labelInsHistBW.Text = "Bitewing Code";
			this.labelInsHistBW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsHistExam
			// 
			this.labelInsHistExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistExam.Location = new System.Drawing.Point(197, 61);
			this.labelInsHistExam.Name = "labelInsHistExam";
			this.labelInsHistExam.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistExam.TabIndex = 264;
			this.labelInsHistExam.Text = "Exam Code";
			this.labelInsHistExam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistFMX
			// 
			this.textInsHistFMX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistFMX.Location = new System.Drawing.Point(341, 34);
			this.textInsHistFMX.Name = "textInsHistFMX";
			this.textInsHistFMX.Size = new System.Drawing.Size(99, 20);
			this.textInsHistFMX.TabIndex = 254;
			// 
			// textInsHistPerioLR
			// 
			this.textInsHistPerioLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioLR.Location = new System.Drawing.Point(341, 178);
			this.textInsHistPerioLR.Name = "textInsHistPerioLR";
			this.textInsHistPerioLR.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioLR.TabIndex = 260;
			// 
			// textInsHistExam
			// 
			this.textInsHistExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistExam.Location = new System.Drawing.Point(341, 58);
			this.textInsHistExam.Name = "textInsHistExam";
			this.textInsHistExam.Size = new System.Drawing.Size(99, 20);
			this.textInsHistExam.TabIndex = 255;
			// 
			// textInsHistDebridement
			// 
			this.textInsHistDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistDebridement.Location = new System.Drawing.Point(341, 106);
			this.textInsHistDebridement.Name = "textInsHistDebridement";
			this.textInsHistDebridement.Size = new System.Drawing.Size(99, 20);
			this.textInsHistDebridement.TabIndex = 257;
			// 
			// textInsHistPerioMaint
			// 
			this.textInsHistPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioMaint.Location = new System.Drawing.Point(341, 130);
			this.textInsHistPerioMaint.Name = "textInsHistPerioMaint";
			this.textInsHistPerioMaint.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioMaint.TabIndex = 258;
			// 
			// textInsHistPerioUL
			// 
			this.textInsHistPerioUL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioUL.Location = new System.Drawing.Point(341, 202);
			this.textInsHistPerioUL.Name = "textInsHistPerioUL";
			this.textInsHistPerioUL.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioUL.TabIndex = 261;
			// 
			// labelInsHistFMX
			// 
			this.labelInsHistFMX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistFMX.Location = new System.Drawing.Point(197, 37);
			this.labelInsHistFMX.Name = "labelInsHistFMX";
			this.labelInsHistFMX.Size = new System.Drawing.Size(143, 17);
			this.labelInsHistFMX.TabIndex = 267;
			this.labelInsHistFMX.Text = "Pano/FMX Code";
			this.labelInsHistFMX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioLL
			// 
			this.textInsHistPerioLL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioLL.Location = new System.Drawing.Point(341, 154);
			this.textInsHistPerioLL.Name = "textInsHistPerioLL";
			this.textInsHistPerioLL.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioLL.TabIndex = 259;
			// 
			// textInsHistPerioUR
			// 
			this.textInsHistPerioUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioUR.Location = new System.Drawing.Point(341, 226);
			this.textInsHistPerioUR.Name = "textInsHistPerioUR";
			this.textInsHistPerioUR.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioUR.TabIndex = 262;
			// 
			// groupTreatPlanSort
			// 
			this.groupTreatPlanSort.BackColor = System.Drawing.Color.White;
			this.groupTreatPlanSort.Controls.Add(this.radioTreatPlanSortTooth);
			this.groupTreatPlanSort.Controls.Add(this.radioTreatPlanSortOrder);
			this.groupTreatPlanSort.Location = new System.Drawing.Point(20, 179);
			this.groupTreatPlanSort.Name = "groupTreatPlanSort";
			this.groupTreatPlanSort.Size = new System.Drawing.Size(450, 55);
			this.groupTreatPlanSort.TabIndex = 249;
			this.groupTreatPlanSort.Text = "Sort procedures by";
			// 
			// radioTreatPlanSortTooth
			// 
			this.radioTreatPlanSortTooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortTooth.Location = new System.Drawing.Point(283, 9);
			this.radioTreatPlanSortTooth.Name = "radioTreatPlanSortTooth";
			this.radioTreatPlanSortTooth.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioTreatPlanSortTooth.Size = new System.Drawing.Size(157, 18);
			this.radioTreatPlanSortTooth.TabIndex = 54;
			this.radioTreatPlanSortTooth.Text = "Tooth";
			this.radioTreatPlanSortTooth.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortTooth.Click += new System.EventHandler(this.radioTreatPlanSortTooth_Click);
			// 
			// radioTreatPlanSortOrder
			// 
			this.radioTreatPlanSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortOrder.Checked = true;
			this.radioTreatPlanSortOrder.Location = new System.Drawing.Point(283, 28);
			this.radioTreatPlanSortOrder.Name = "radioTreatPlanSortOrder";
			this.radioTreatPlanSortOrder.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioTreatPlanSortOrder.Size = new System.Drawing.Size(157, 18);
			this.radioTreatPlanSortOrder.TabIndex = 53;
			this.radioTreatPlanSortOrder.TabStop = true;
			this.radioTreatPlanSortOrder.Text = "Order Entered";
			this.radioTreatPlanSortOrder.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortOrder.Click += new System.EventHandler(this.radioTreatPlanSortOrder_Click);
			// 
			// textTreatNote
			// 
			this.textTreatNote.AcceptsTab = true;
			this.textTreatNote.BackColor = System.Drawing.SystemColors.Window;
			this.textTreatNote.DetectLinksEnabled = false;
			this.textTreatNote.DetectUrls = false;
			this.textTreatNote.Location = new System.Drawing.Point(97, 10);
			this.textTreatNote.MaxLength = 32767;
			this.textTreatNote.Name = "textTreatNote";
			this.textTreatNote.QuickPasteType = OpenDentBusiness.QuickPasteType.TreatPlan;
			this.textTreatNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTreatNote.Size = new System.Drawing.Size(363, 66);
			this.textTreatNote.TabIndex = 248;
			this.textTreatNote.Text = "";
			// 
			// UserControlTreatPlanGeneral
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupTreatPlan);
			this.Controls.Add(this.groupBoxDiscounts);
			this.Controls.Add(this.groupBoxInsuranceHistory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupTreatPlanSort);
			this.Controls.Add(this.textTreatNote);
			this.Name = "UserControlTreatPlanGeneral";
			this.Size = new System.Drawing.Size(494, 624);
			this.Load += new System.EventHandler(this.UserControlTreatPlanGeneral_Load);
			this.groupTreatPlan.ResumeLayout(false);
			this.groupBoxDiscounts.ResumeLayout(false);
			this.groupBoxDiscounts.PerformLayout();
			this.groupBoxInsuranceHistory.ResumeLayout(false);
			this.groupBoxInsuranceHistory.PerformLayout();
			this.groupTreatPlanSort.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.CheckBox checkPromptSaveTP;
		private System.Windows.Forms.Label labelDiscountPercentage;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label1;
		private UI.GroupBox groupTreatPlanSort;
		private System.Windows.Forms.RadioButton radioTreatPlanSortTooth;
		private System.Windows.Forms.RadioButton radioTreatPlanSortOrder;
		private ODtextBox textTreatNote;
		private OpenDental.UI.CheckBox checkTPSaveSigned;
		private UI.ComboBox comboProcDiscountType;
		private OpenDental.UI.CheckBox checkTreatPlanShowCompleted;
		private System.Windows.Forms.TextBox textDiscountPercentage;
		private OpenDental.UI.CheckBox checkTreatPlanItemized;
		private System.Windows.Forms.TextBox textInsHistProphy;
		private System.Windows.Forms.Label labelInsHistProphy;
		private System.Windows.Forms.Label labelInsHistBW;
		private System.Windows.Forms.TextBox textInsHistPerioLR;
		private System.Windows.Forms.TextBox textInsHistDebridement;
		private System.Windows.Forms.Label labelInsHistPerioLR;
		private System.Windows.Forms.Label labelInsHistExam;
		private System.Windows.Forms.TextBox textInsHistPerioLL;
		private System.Windows.Forms.TextBox textInsHistBW;
		private System.Windows.Forms.Label labelInsHistPerioLL;
		private System.Windows.Forms.Label labelInsHistDebridement;
		private System.Windows.Forms.TextBox textInsHistPerioUL;
		private System.Windows.Forms.TextBox textInsHistExam;
		private System.Windows.Forms.Label labelInsHistPerioUL;
		private System.Windows.Forms.Label labelInsHistPerioMaint;
		private System.Windows.Forms.TextBox textInsHistPerioUR;
		private System.Windows.Forms.TextBox textInsHistPerioMaint;
		private System.Windows.Forms.Label labelInsHistPerioUR;
		private System.Windows.Forms.Label labelInsHistFMX;
		private System.Windows.Forms.TextBox textInsHistFMX;
		private UI.GroupBox groupBoxInsuranceHistory;
		private UI.GroupBox groupBoxDiscounts;
		private UI.GroupBox groupTreatPlan;
	}
}
