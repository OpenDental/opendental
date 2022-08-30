
namespace OpenDental {
	partial class UserControlAccountRecAndRepCharges {
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
			this.groupRecurringCharges = new OpenDental.UI.GroupBoxOD();
			this.checkRecurringChargesShowInactive = new System.Windows.Forms.CheckBox();
			this.checkRecurringChargesInactivateDeclinedCards = new System.Windows.Forms.CheckBox();
			this.checkRecurPatBal0 = new System.Windows.Forms.CheckBox();
			this.label56 = new System.Windows.Forms.Label();
			this.comboRecurringChargePayType = new OpenDental.UI.ComboBoxOD();
			this.labelRecurringChargesAutomatedTime = new System.Windows.Forms.Label();
			this.textRecurringChargesTime = new OpenDental.ValidTime();
			this.checkRecurringChargesAutomated = new System.Windows.Forms.CheckBox();
			this.checkRecurringChargesUseTransDate = new System.Windows.Forms.CheckBox();
			this.checkRecurChargPriProv = new System.Windows.Forms.CheckBox();
			this.groupRepeatingCharges = new OpenDental.UI.GroupBoxOD();
			this.labelRepeatingChargesAutomatedTime = new System.Windows.Forms.Label();
			this.textRepeatingChargesAutomatedTime = new OpenDental.ValidTime();
			this.checkRepeatingChargesRunAging = new System.Windows.Forms.CheckBox();
			this.checkRepeatingChargesAutomated = new System.Windows.Forms.CheckBox();
			this.labelRecurChargPriProvDetails = new System.Windows.Forms.Label();
			this.labelRecurringChargesTimeDetails = new System.Windows.Forms.Label();
			this.labelRecurringChargesInactivateDeclinedCardsDetails = new System.Windows.Forms.Label();
			this.labelRepeatingChargesAutomatedTimeDetails = new System.Windows.Forms.Label();
			this.groupRecurringCharges.SuspendLayout();
			this.groupRepeatingCharges.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupRecurringCharges
			// 
			this.groupRecurringCharges.BackColor = System.Drawing.Color.White;
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesShowInactive);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesInactivateDeclinedCards);
			this.groupRecurringCharges.Controls.Add(this.checkRecurPatBal0);
			this.groupRecurringCharges.Controls.Add(this.label56);
			this.groupRecurringCharges.Controls.Add(this.comboRecurringChargePayType);
			this.groupRecurringCharges.Controls.Add(this.labelRecurringChargesAutomatedTime);
			this.groupRecurringCharges.Controls.Add(this.textRecurringChargesTime);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesAutomated);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesUseTransDate);
			this.groupRecurringCharges.Controls.Add(this.checkRecurChargPriProv);
			this.groupRecurringCharges.Location = new System.Drawing.Point(20, 40);
			this.groupRecurringCharges.Name = "groupRecurringCharges";
			this.groupRecurringCharges.Size = new System.Drawing.Size(450, 271);
			this.groupRecurringCharges.TabIndex = 14;
			this.groupRecurringCharges.Text = "Recurring Charges (payments)";
			// 
			// checkRecurringChargesShowInactive
			// 
			this.checkRecurringChargesShowInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesShowInactive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesShowInactive.Location = new System.Drawing.Point(140, 82);
			this.checkRecurringChargesShowInactive.Name = "checkRecurringChargesShowInactive";
			this.checkRecurringChargesShowInactive.Size = new System.Drawing.Size(300, 17);
			this.checkRecurringChargesShowInactive.TabIndex = 241;
			this.checkRecurringChargesShowInactive.Text = "Recurring charges show inactive charges by default";
			this.checkRecurringChargesShowInactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurringChargesInactivateDeclinedCards
			// 
			this.checkRecurringChargesInactivateDeclinedCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesInactivateDeclinedCards.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesInactivateDeclinedCards.Location = new System.Drawing.Point(214, 244);
			this.checkRecurringChargesInactivateDeclinedCards.Name = "checkRecurringChargesInactivateDeclinedCards";
			this.checkRecurringChargesInactivateDeclinedCards.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesInactivateDeclinedCards.TabIndex = 254;
			this.checkRecurringChargesInactivateDeclinedCards.Text = "Automatically inactivate declined cards";
			this.checkRecurringChargesInactivateDeclinedCards.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurPatBal0
			// 
			this.checkRecurPatBal0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurPatBal0.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurPatBal0.Location = new System.Drawing.Point(91, 213);
			this.checkRecurPatBal0.Name = "checkRecurPatBal0";
			this.checkRecurPatBal0.Size = new System.Drawing.Size(349, 17);
			this.checkRecurPatBal0.TabIndex = 246;
			this.checkRecurPatBal0.Text = "Allow recurring charges to run even if no family balance present";
			this.checkRecurPatBal0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurPatBal0.UseVisualStyleBackColor = true;
			// 
			// label56
			// 
			this.label56.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label56.Location = new System.Drawing.Point(184, 181);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(90, 17);
			this.label56.TabIndex = 253;
			this.label56.Text = "Pay type for CC";
			this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRecurringChargePayType
			// 
			this.comboRecurringChargePayType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboRecurringChargePayType.Location = new System.Drawing.Point(277, 178);
			this.comboRecurringChargePayType.Name = "comboRecurringChargePayType";
			this.comboRecurringChargePayType.Size = new System.Drawing.Size(163, 21);
			this.comboRecurringChargePayType.TabIndex = 252;
			// 
			// labelRecurringChargesAutomatedTime
			// 
			this.labelRecurringChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRecurringChargesAutomatedTime.Enabled = false;
			this.labelRecurringChargesAutomatedTime.Location = new System.Drawing.Point(210, 147);
			this.labelRecurringChargesAutomatedTime.Name = "labelRecurringChargesAutomatedTime";
			this.labelRecurringChargesAutomatedTime.Size = new System.Drawing.Size(159, 17);
			this.labelRecurringChargesAutomatedTime.TabIndex = 243;
			this.labelRecurringChargesAutomatedTime.Text = "Recurring charges run time";
			this.labelRecurringChargesAutomatedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRecurringChargesTime
			// 
			this.textRecurringChargesTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRecurringChargesTime.Enabled = false;
			this.textRecurringChargesTime.Location = new System.Drawing.Point(372, 144);
			this.textRecurringChargesTime.Name = "textRecurringChargesTime";
			this.textRecurringChargesTime.Size = new System.Drawing.Size(68, 20);
			this.textRecurringChargesTime.TabIndex = 242;
			this.textRecurringChargesTime.Leave += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRecurringChargesAutomated
			// 
			this.checkRecurringChargesAutomated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesAutomated.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesAutomated.Location = new System.Drawing.Point(214, 113);
			this.checkRecurringChargesAutomated.Name = "checkRecurringChargesAutomated";
			this.checkRecurringChargesAutomated.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesAutomated.TabIndex = 240;
			this.checkRecurringChargesAutomated.Text = "Recurring charges run automatically";
			this.checkRecurringChargesAutomated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesAutomated.CheckedChanged += new System.EventHandler(this.checkRecurringChargesAutomated_CheckedChanged);
			this.checkRecurringChargesAutomated.Click += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRecurringChargesUseTransDate
			// 
			this.checkRecurringChargesUseTransDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesUseTransDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesUseTransDate.Location = new System.Drawing.Point(214, 51);
			this.checkRecurringChargesUseTransDate.Name = "checkRecurringChargesUseTransDate";
			this.checkRecurringChargesUseTransDate.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesUseTransDate.TabIndex = 239;
			this.checkRecurringChargesUseTransDate.Text = "Recurring charges use transaction date";
			this.checkRecurringChargesUseTransDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurChargPriProv
			// 
			this.checkRecurChargPriProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurChargPriProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurChargPriProv.Location = new System.Drawing.Point(102, 20);
			this.checkRecurChargPriProv.Name = "checkRecurChargPriProv";
			this.checkRecurChargPriProv.Size = new System.Drawing.Size(338, 17);
			this.checkRecurChargPriProv.TabIndex = 238;
			this.checkRecurChargPriProv.Text = "Recurring charges use primary provider instead of FIFO";
			this.checkRecurChargPriProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupRepeatingCharges
			// 
			this.groupRepeatingCharges.BackColor = System.Drawing.Color.White;
			this.groupRepeatingCharges.Controls.Add(this.labelRepeatingChargesAutomatedTime);
			this.groupRepeatingCharges.Controls.Add(this.textRepeatingChargesAutomatedTime);
			this.groupRepeatingCharges.Controls.Add(this.checkRepeatingChargesRunAging);
			this.groupRepeatingCharges.Controls.Add(this.checkRepeatingChargesAutomated);
			this.groupRepeatingCharges.Location = new System.Drawing.Point(20, 325);
			this.groupRepeatingCharges.Name = "groupRepeatingCharges";
			this.groupRepeatingCharges.Size = new System.Drawing.Size(450, 102);
			this.groupRepeatingCharges.TabIndex = 15;
			this.groupRepeatingCharges.Text = "Repeating Charges (procedures)";
			// 
			// labelRepeatingChargesAutomatedTime
			// 
			this.labelRepeatingChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRepeatingChargesAutomatedTime.Enabled = false;
			this.labelRepeatingChargesAutomatedTime.Location = new System.Drawing.Point(215, 75);
			this.labelRepeatingChargesAutomatedTime.Name = "labelRepeatingChargesAutomatedTime";
			this.labelRepeatingChargesAutomatedTime.Size = new System.Drawing.Size(154, 17);
			this.labelRepeatingChargesAutomatedTime.TabIndex = 243;
			this.labelRepeatingChargesAutomatedTime.Text = "Repeating charges run time";
			this.labelRepeatingChargesAutomatedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRepeatingChargesAutomatedTime
			// 
			this.textRepeatingChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRepeatingChargesAutomatedTime.Enabled = false;
			this.textRepeatingChargesAutomatedTime.Location = new System.Drawing.Point(372, 72);
			this.textRepeatingChargesAutomatedTime.Name = "textRepeatingChargesAutomatedTime";
			this.textRepeatingChargesAutomatedTime.Size = new System.Drawing.Size(68, 20);
			this.textRepeatingChargesAutomatedTime.TabIndex = 243;
			this.textRepeatingChargesAutomatedTime.Leave += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRepeatingChargesRunAging
			// 
			this.checkRepeatingChargesRunAging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRepeatingChargesRunAging.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesRunAging.Location = new System.Drawing.Point(187, 10);
			this.checkRepeatingChargesRunAging.Name = "checkRepeatingChargesRunAging";
			this.checkRepeatingChargesRunAging.Size = new System.Drawing.Size(253, 17);
			this.checkRepeatingChargesRunAging.TabIndex = 239;
			this.checkRepeatingChargesRunAging.Text = "Run aging after posting charges";
			this.checkRepeatingChargesRunAging.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRepeatingChargesAutomated
			// 
			this.checkRepeatingChargesAutomated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRepeatingChargesAutomated.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesAutomated.Location = new System.Drawing.Point(214, 41);
			this.checkRepeatingChargesAutomated.Name = "checkRepeatingChargesAutomated";
			this.checkRepeatingChargesAutomated.Size = new System.Drawing.Size(226, 17);
			this.checkRepeatingChargesAutomated.TabIndex = 238;
			this.checkRepeatingChargesAutomated.Text = "Repeating charges run automatically";
			this.checkRepeatingChargesAutomated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesAutomated.CheckedChanged += new System.EventHandler(this.checkRepeatingChargesAutomated_CheckedChanged);
			this.checkRepeatingChargesAutomated.Click += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// labelRecurChargPriProvDetails
			// 
			this.labelRecurChargPriProvDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRecurChargPriProvDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelRecurChargPriProvDetails.Location = new System.Drawing.Point(476, 59);
			this.labelRecurChargPriProvDetails.Name = "labelRecurChargPriProvDetails";
			this.labelRecurChargPriProvDetails.Size = new System.Drawing.Size(498, 17);
			this.labelRecurChargPriProvDetails.TabIndex = 334;
			this.labelRecurChargPriProvDetails.Text = "only used when Paysplit allocations is set to \'Rigorous\'";
			this.labelRecurChargPriProvDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelRecurringChargesTimeDetails
			// 
			this.labelRecurringChargesTimeDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRecurringChargesTimeDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelRecurringChargesTimeDetails.Location = new System.Drawing.Point(476, 187);
			this.labelRecurringChargesTimeDetails.Name = "labelRecurringChargesTimeDetails";
			this.labelRecurringChargesTimeDetails.Size = new System.Drawing.Size(498, 17);
			this.labelRecurringChargesTimeDetails.TabIndex = 335;
			this.labelRecurringChargesTimeDetails.Text = "this should be set to run after Repeating charges";
			this.labelRecurringChargesTimeDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelRecurringChargesInactivateDeclinedCardsDetails
			// 
			this.labelRecurringChargesInactivateDeclinedCardsDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRecurringChargesInactivateDeclinedCardsDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelRecurringChargesInactivateDeclinedCardsDetails.Location = new System.Drawing.Point(476, 283);
			this.labelRecurringChargesInactivateDeclinedCardsDetails.Name = "labelRecurringChargesInactivateDeclinedCardsDetails";
			this.labelRecurringChargesInactivateDeclinedCardsDetails.Size = new System.Drawing.Size(498, 17);
			this.labelRecurringChargesInactivateDeclinedCardsDetails.TabIndex = 336;
			this.labelRecurringChargesInactivateDeclinedCardsDetails.Text = "3 states. If filled, behaves as unchecked. Recommend unchecked.";
			this.labelRecurringChargesInactivateDeclinedCardsDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelRepeatingChargesAutomatedTimeDetails
			// 
			this.labelRepeatingChargesAutomatedTimeDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRepeatingChargesAutomatedTimeDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelRepeatingChargesAutomatedTimeDetails.Location = new System.Drawing.Point(476, 400);
			this.labelRepeatingChargesAutomatedTimeDetails.Name = "labelRepeatingChargesAutomatedTimeDetails";
			this.labelRepeatingChargesAutomatedTimeDetails.Size = new System.Drawing.Size(498, 17);
			this.labelRepeatingChargesAutomatedTimeDetails.TabIndex = 337;
			this.labelRepeatingChargesAutomatedTimeDetails.Text = "this should be set to run before Recurring charges";
			this.labelRepeatingChargesAutomatedTimeDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlAccountRecAndRepCharges
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelRepeatingChargesAutomatedTimeDetails);
			this.Controls.Add(this.labelRecurringChargesInactivateDeclinedCardsDetails);
			this.Controls.Add(this.labelRecurringChargesTimeDetails);
			this.Controls.Add(this.labelRecurChargPriProvDetails);
			this.Controls.Add(this.groupRepeatingCharges);
			this.Controls.Add(this.groupRecurringCharges);
			this.Name = "UserControlAccountRecAndRepCharges";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupRecurringCharges.ResumeLayout(false);
			this.groupRecurringCharges.PerformLayout();
			this.groupRepeatingCharges.ResumeLayout(false);
			this.groupRepeatingCharges.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBoxOD groupRecurringCharges;
		private System.Windows.Forms.CheckBox checkRecurringChargesShowInactive;
		private System.Windows.Forms.CheckBox checkRecurringChargesInactivateDeclinedCards;
		private System.Windows.Forms.CheckBox checkRecurPatBal0;
		private System.Windows.Forms.Label label56;
		private UI.ComboBoxOD comboRecurringChargePayType;
		private System.Windows.Forms.Label labelRecurringChargesAutomatedTime;
		private ValidTime textRecurringChargesTime;
		private System.Windows.Forms.CheckBox checkRecurringChargesAutomated;
		private System.Windows.Forms.CheckBox checkRecurringChargesUseTransDate;
		private System.Windows.Forms.CheckBox checkRecurChargPriProv;
		private UI.GroupBoxOD groupRepeatingCharges;
		private System.Windows.Forms.Label labelRepeatingChargesAutomatedTime;
		private ValidTime textRepeatingChargesAutomatedTime;
		private System.Windows.Forms.CheckBox checkRepeatingChargesRunAging;
		private System.Windows.Forms.CheckBox checkRepeatingChargesAutomated;
		private System.Windows.Forms.Label labelRecurChargPriProvDetails;
		private System.Windows.Forms.Label labelRecurringChargesTimeDetails;
		private System.Windows.Forms.Label labelRecurringChargesInactivateDeclinedCardsDetails;
		private System.Windows.Forms.Label labelRepeatingChargesAutomatedTimeDetails;
	}
}
