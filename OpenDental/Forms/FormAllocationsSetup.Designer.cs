namespace OpenDental{
	partial class FormAllocationsSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAllocationsSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkClaimPayByTotalSplitsAuto = new OpenDental.UI.CheckBox();
			this.checkShowIncomeTransferManager = new OpenDental.UI.CheckBox();
			this.label40 = new System.Windows.Forms.Label();
			this.butLineItem = new OpenDental.UI.Button();
			this.butSimple = new OpenDental.UI.Button();
			this.checkHidePaysplits = new OpenDental.UI.CheckBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.radioPayDont = new System.Windows.Forms.RadioButton();
			this.radioPayAuto = new System.Windows.Forms.RadioButton();
			this.radioPayEnforce = new System.Windows.Forms.RadioButton();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.radioAdjustDont = new System.Windows.Forms.RadioButton();
			this.radioAdjustLink = new System.Windows.Forms.RadioButton();
			this.radioAdjustEnforce = new System.Windows.Forms.RadioButton();
			this.butDefault = new OpenDental.UI.Button();
			this.labelPermission = new System.Windows.Forms.Label();
			this.groupBox5 = new OpenDental.UI.GroupBox();
			this.labelRefundable = new System.Windows.Forms.Label();
			this.checkAllowPrePayToTpProcs = new OpenDental.UI.CheckBox();
			this.checkIsRefundable = new OpenDental.UI.CheckBox();
			this.label57 = new System.Windows.Forms.Label();
			this.comboTpUnearnedType = new OpenDental.UI.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkIncomeTransfersMadeUponClaimReceived = new OpenDental.UI.CheckBox();
			this.labelIncomeTransfersMadeUponClaimReceivedDesc = new System.Windows.Forms.Label();
			this.checkAdjustmentsOffset = new OpenDental.UI.CheckBox();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(719, 536);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(805, 536);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkClaimPayByTotalSplitsAuto
			// 
			this.checkClaimPayByTotalSplitsAuto.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPayByTotalSplitsAuto.Location = new System.Drawing.Point(127, 460);
			this.checkClaimPayByTotalSplitsAuto.Name = "checkClaimPayByTotalSplitsAuto";
			this.checkClaimPayByTotalSplitsAuto.Size = new System.Drawing.Size(255, 17);
			this.checkClaimPayByTotalSplitsAuto.TabIndex = 304;
			this.checkClaimPayByTotalSplitsAuto.Text = "Claim Pay by Total splits automatically";
			// 
			// checkShowIncomeTransferManager
			// 
			this.checkShowIncomeTransferManager.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowIncomeTransferManager.Location = new System.Drawing.Point(127, 437);
			this.checkShowIncomeTransferManager.Name = "checkShowIncomeTransferManager";
			this.checkShowIncomeTransferManager.Size = new System.Drawing.Size(255, 17);
			this.checkShowIncomeTransferManager.TabIndex = 303;
			this.checkShowIncomeTransferManager.Text = "Show Income Transfer Manager";
			// 
			// label40
			// 
			this.label40.Location = new System.Drawing.Point(23, 18);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(189, 54);
			this.label40.TabIndex = 302;
			this.label40.Text = "These buttons set all prefs within this window simultaneously.  Changes don\'t get" +
    " saved until you click OK.";
			// 
			// butLineItem
			// 
			this.butLineItem.Location = new System.Drawing.Point(21, 78);
			this.butLineItem.Name = "butLineItem";
			this.butLineItem.Size = new System.Drawing.Size(109, 24);
			this.butLineItem.TabIndex = 301;
			this.butLineItem.Text = "Rigorous Line Item";
			this.butLineItem.Click += new System.EventHandler(this.butLineItem_Click);
			// 
			// butSimple
			// 
			this.butSimple.Location = new System.Drawing.Point(21, 145);
			this.butSimple.Name = "butSimple";
			this.butSimple.Size = new System.Drawing.Size(109, 24);
			this.butSimple.TabIndex = 300;
			this.butSimple.Text = "Simple";
			this.butSimple.Click += new System.EventHandler(this.butSimple_Click);
			// 
			// checkHidePaysplits
			// 
			this.checkHidePaysplits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidePaysplits.Location = new System.Drawing.Point(70, 415);
			this.checkHidePaysplits.Name = "checkHidePaysplits";
			this.checkHidePaysplits.Size = new System.Drawing.Size(312, 17);
			this.checkHidePaysplits.TabIndex = 297;
			this.checkHidePaysplits.Text = "Hide paysplits from Payment window by default";
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.White;
			this.groupBoxOD1.Controls.Add(this.label3);
			this.groupBoxOD1.Controls.Add(this.label2);
			this.groupBoxOD1.Controls.Add(this.label1);
			this.groupBoxOD1.Controls.Add(this.radioPayDont);
			this.groupBoxOD1.Controls.Add(this.radioPayAuto);
			this.groupBoxOD1.Controls.Add(this.radioPayEnforce);
			this.groupBoxOD1.Location = new System.Drawing.Point(298, 18);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(547, 130);
			this.groupBoxOD1.TabIndex = 305;
			this.groupBoxOD1.Text = "Paysplits";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(93, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(405, 30);
			this.label3.TabIndex = 317;
			this.label3.Text = "Highlight procedures before entering a payment to automatically split the payment" +
    " to those procedures.  Historical clutter can be reallocated by Transfer Manager" +
    " FIFO.\r\n";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(93, 94);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(388, 30);
			this.label2.TabIndex = 315;
			this.label2.Text = "Usually no splits, so this looks simpler.  But if the office ever wishes to move " +
    "to payment of providers by collections, staff will need to be retrained.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(93, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(425, 31);
			this.label1.TabIndex = 306;
			this.label1.Text = "Payments are automatically split by procedure and provider validity is enforced. " +
    " Historical clutter must first be reallocated by Transfer Manager using Rigorous" +
    " complex splits.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioPayDont
			// 
			this.radioPayDont.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayDont.Location = new System.Drawing.Point(7, 95);
			this.radioPayDont.Name = "radioPayDont";
			this.radioPayDont.Size = new System.Drawing.Size(76, 18);
			this.radioPayDont.TabIndex = 297;
			this.radioPayDont.TabStop = true;
			this.radioPayDont.Text = "Manual";
			this.radioPayDont.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayDont.UseVisualStyleBackColor = true;
			// 
			// radioPayAuto
			// 
			this.radioPayAuto.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayAuto.Location = new System.Drawing.Point(7, 58);
			this.radioPayAuto.Name = "radioPayAuto";
			this.radioPayAuto.Size = new System.Drawing.Size(76, 18);
			this.radioPayAuto.TabIndex = 296;
			this.radioPayAuto.TabStop = true;
			this.radioPayAuto.Text = "Auto-Split";
			this.radioPayAuto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayAuto.UseVisualStyleBackColor = true;
			// 
			// radioPayEnforce
			// 
			this.radioPayEnforce.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayEnforce.Location = new System.Drawing.Point(7, 21);
			this.radioPayEnforce.Name = "radioPayEnforce";
			this.radioPayEnforce.Size = new System.Drawing.Size(76, 18);
			this.radioPayEnforce.TabIndex = 295;
			this.radioPayEnforce.TabStop = true;
			this.radioPayEnforce.Text = "Rigorous";
			this.radioPayEnforce.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayEnforce.UseVisualStyleBackColor = true;
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.BackColor = System.Drawing.Color.White;
			this.groupBoxOD2.Controls.Add(this.label7);
			this.groupBoxOD2.Controls.Add(this.label6);
			this.groupBoxOD2.Controls.Add(this.label8);
			this.groupBoxOD2.Controls.Add(this.radioAdjustDont);
			this.groupBoxOD2.Controls.Add(this.radioAdjustLink);
			this.groupBoxOD2.Controls.Add(this.radioAdjustEnforce);
			this.groupBoxOD2.Location = new System.Drawing.Point(298, 154);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(463, 113);
			this.groupBoxOD2.TabIndex = 318;
			this.groupBoxOD2.Text = "Adjustments";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(93, 82);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(343, 18);
			this.label7.TabIndex = 318;
			this.label7.Text = "Not paying providers by collections.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(93, 55);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(343, 16);
			this.label6.TabIndex = 316;
			this.label6.Text = "Adjustment links are made automatically, but can be edited.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(93, 18);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(343, 30);
			this.label8.TabIndex = 306;
			this.label8.Text = "Adjustments are automatically linked to procedures\r\nand validity is enforced.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioAdjustDont
			// 
			this.radioAdjustDont.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustDont.Location = new System.Drawing.Point(7, 80);
			this.radioAdjustDont.Name = "radioAdjustDont";
			this.radioAdjustDont.Size = new System.Drawing.Size(76, 18);
			this.radioAdjustDont.TabIndex = 297;
			this.radioAdjustDont.TabStop = true;
			this.radioAdjustDont.Text = "Manual";
			this.radioAdjustDont.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustDont.UseVisualStyleBackColor = true;
			// 
			// radioAdjustLink
			// 
			this.radioAdjustLink.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustLink.Location = new System.Drawing.Point(7, 54);
			this.radioAdjustLink.Name = "radioAdjustLink";
			this.radioAdjustLink.Size = new System.Drawing.Size(76, 18);
			this.radioAdjustLink.TabIndex = 296;
			this.radioAdjustLink.TabStop = true;
			this.radioAdjustLink.Text = "Link Only";
			this.radioAdjustLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustLink.UseVisualStyleBackColor = true;
			// 
			// radioAdjustEnforce
			// 
			this.radioAdjustEnforce.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustEnforce.Location = new System.Drawing.Point(7, 21);
			this.radioAdjustEnforce.Name = "radioAdjustEnforce";
			this.radioAdjustEnforce.Size = new System.Drawing.Size(76, 18);
			this.radioAdjustEnforce.TabIndex = 295;
			this.radioAdjustEnforce.TabStop = true;
			this.radioAdjustEnforce.Text = "Rigorous";
			this.radioAdjustEnforce.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAdjustEnforce.UseVisualStyleBackColor = true;
			// 
			// butDefault
			// 
			this.butDefault.Location = new System.Drawing.Point(21, 112);
			this.butDefault.Name = "butDefault";
			this.butDefault.Size = new System.Drawing.Size(109, 24);
			this.butDefault.TabIndex = 320;
			this.butDefault.Text = "Default";
			this.butDefault.Click += new System.EventHandler(this.butDefault_Click);
			// 
			// labelPermission
			// 
			this.labelPermission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPermission.ForeColor = System.Drawing.Color.DarkRed;
			this.labelPermission.Location = new System.Drawing.Point(598, 542);
			this.labelPermission.Name = "labelPermission";
			this.labelPermission.Size = new System.Drawing.Size(116, 16);
			this.labelPermission.TabIndex = 318;
			this.labelPermission.Text = "No Permission";
			this.labelPermission.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox5
			// 
			this.groupBox5.BackColor = System.Drawing.Color.White;
			this.groupBox5.Controls.Add(this.labelRefundable);
			this.groupBox5.Controls.Add(this.checkAllowPrePayToTpProcs);
			this.groupBox5.Controls.Add(this.checkIsRefundable);
			this.groupBox5.Controls.Add(this.label57);
			this.groupBox5.Controls.Add(this.comboTpUnearnedType);
			this.groupBox5.Location = new System.Drawing.Point(26, 274);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(735, 129);
			this.groupBox5.TabIndex = 322;
			this.groupBox5.Text = "Treatment Planned Prepayments";
			// 
			// labelRefundable
			// 
			this.labelRefundable.Location = new System.Drawing.Point(362, 40);
			this.labelRefundable.Name = "labelRefundable";
			this.labelRefundable.Size = new System.Drawing.Size(343, 58);
			this.labelRefundable.TabIndex = 317;
			this.labelRefundable.Text = resources.GetString("labelRefundable.Text");
			this.labelRefundable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAllowPrePayToTpProcs
			// 
			this.checkAllowPrePayToTpProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPrePayToTpProcs.Location = new System.Drawing.Point(3, 21);
			this.checkAllowPrePayToTpProcs.Name = "checkAllowPrePayToTpProcs";
			this.checkAllowPrePayToTpProcs.Size = new System.Drawing.Size(353, 18);
			this.checkAllowPrePayToTpProcs.TabIndex = 5;
			this.checkAllowPrePayToTpProcs.Text = "Allow prepayments to allocate to treatment planned procedures";
			this.checkAllowPrePayToTpProcs.Click += new System.EventHandler(this.checkAllowPrePayToTpProcs_Click);
			// 
			// checkIsRefundable
			// 
			this.checkIsRefundable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsRefundable.Location = new System.Drawing.Point(120, 46);
			this.checkIsRefundable.Name = "checkIsRefundable";
			this.checkIsRefundable.Size = new System.Drawing.Size(236, 17);
			this.checkIsRefundable.TabIndex = 10;
			this.checkIsRefundable.Text = "TP prepayments are non-refundable";
			this.checkIsRefundable.Visible = false;
			// 
			// label57
			// 
			this.label57.Location = new System.Drawing.Point(56, 103);
			this.label57.Name = "label57";
			this.label57.Size = new System.Drawing.Size(281, 18);
			this.label57.TabIndex = 253;
			this.label57.Text = "Default treatment planned procedure unearned type";
			this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTpUnearnedType
			// 
			this.comboTpUnearnedType.Location = new System.Drawing.Point(343, 100);
			this.comboTpUnearnedType.Name = "comboTpUnearnedType";
			this.comboTpUnearnedType.Size = new System.Drawing.Size(163, 21);
			this.comboTpUnearnedType.TabIndex = 15;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(387, 415);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(393, 18);
			this.label4.TabIndex = 323;
			this.label4.Text = "Not recommended.  It\'s good to see the splits even if you don\'t change them.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIncomeTransfersMadeUponClaimReceived
			// 
			this.checkIncomeTransfersMadeUponClaimReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncomeTransfersMadeUponClaimReceived.Location = new System.Drawing.Point(51, 483);
			this.checkIncomeTransfersMadeUponClaimReceived.Name = "checkIncomeTransfersMadeUponClaimReceived";
			this.checkIncomeTransfersMadeUponClaimReceived.Size = new System.Drawing.Size(331, 17);
			this.checkIncomeTransfersMadeUponClaimReceived.TabIndex = 318;
			this.checkIncomeTransfersMadeUponClaimReceived.Text = "Make claim specific income transfers when received";
			this.checkIncomeTransfersMadeUponClaimReceived.ThreeState = true;
			this.checkIncomeTransfersMadeUponClaimReceived.CheckStateChanged += new System.EventHandler(this.checkAutoIncomeTransfer_CheckedStateChanged);
			// 
			// labelIncomeTransfersMadeUponClaimReceivedDesc
			// 
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.Location = new System.Drawing.Point(387, 482);
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.Name = "labelIncomeTransfersMadeUponClaimReceivedDesc";
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.Size = new System.Drawing.Size(460, 16);
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.TabIndex = 324;
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.Text = "(Empty, no transfers), (x, automatic transfers), (square, follows paysplits rigor" +
    "ous)";
			this.labelIncomeTransfersMadeUponClaimReceivedDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAdjustmentsOffset
			// 
			this.checkAdjustmentsOffset.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAdjustmentsOffset.Location = new System.Drawing.Point(127, 506);
			this.checkAdjustmentsOffset.Name = "checkAdjustmentsOffset";
			this.checkAdjustmentsOffset.Size = new System.Drawing.Size(255, 17);
			this.checkAdjustmentsOffset.TabIndex = 325;
			this.checkAdjustmentsOffset.Text = "+/- Adjustments offset each other";
			// 
			// FormAllocationsSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(892, 572);
			this.Controls.Add(this.checkAdjustmentsOffset);
			this.Controls.Add(this.labelIncomeTransfersMadeUponClaimReceivedDesc);
			this.Controls.Add(this.checkIncomeTransfersMadeUponClaimReceived);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.labelPermission);
			this.Controls.Add(this.butDefault);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.checkClaimPayByTotalSplitsAuto);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkShowIncomeTransferManager);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label40);
			this.Controls.Add(this.butLineItem);
			this.Controls.Add(this.checkHidePaysplits);
			this.Controls.Add(this.butSimple);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAllocationsSetup";
			this.Text = "Allocations Setup";
			this.Load += new System.EventHandler(this.FormAllocationsSetup_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.CheckBox checkClaimPayByTotalSplitsAuto;
		private OpenDental.UI.CheckBox checkShowIncomeTransferManager;
		private System.Windows.Forms.Label label40;
		private UI.Button butLineItem;
		private UI.Button butSimple;
		private OpenDental.UI.CheckBox checkHidePaysplits;
		private UI.GroupBox groupBoxOD1;
		private System.Windows.Forms.RadioButton radioPayEnforce;
		private System.Windows.Forms.RadioButton radioPayDont;
		private System.Windows.Forms.RadioButton radioPayAuto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private UI.GroupBox groupBoxOD2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.RadioButton radioAdjustDont;
		private System.Windows.Forms.RadioButton radioAdjustLink;
		private System.Windows.Forms.RadioButton radioAdjustEnforce;
		private UI.Button butDefault;
		private System.Windows.Forms.Label labelPermission;
		private UI.GroupBox groupBox5;
		private OpenDental.UI.CheckBox checkAllowPrePayToTpProcs;
		private OpenDental.UI.CheckBox checkIsRefundable;
		private System.Windows.Forms.Label label57;
		private UI.ComboBox comboTpUnearnedType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelRefundable;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.CheckBox checkIncomeTransfersMadeUponClaimReceived;
		private System.Windows.Forms.Label labelIncomeTransfersMadeUponClaimReceivedDesc;
		private OpenDental.UI.CheckBox checkAdjustmentsOffset;
	}
}