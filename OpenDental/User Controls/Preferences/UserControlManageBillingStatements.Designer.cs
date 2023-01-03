
namespace OpenDental {
	partial class UserControlManageBillingStatements {
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
			this.checkStatementsAlphabetically = new OpenDental.UI.CheckBox();
			this.checkBillingShowProgress = new OpenDental.UI.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.textBillingElectBatchMax = new OpenDental.ValidNum();
			this.checkStatementShowAdjNotes = new OpenDental.UI.CheckBox();
			this.checkIntermingleDefault = new OpenDental.UI.CheckBox();
			this.checkStatementShowReturnAddress = new OpenDental.UI.CheckBox();
			this.checkStatementShowProcBreakdown = new OpenDental.UI.CheckBox();
			this.checkStatementShowNotes = new OpenDental.UI.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboUseChartNum = new OpenDental.UI.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textStatementsCalcDueDate = new OpenDental.ValidNum();
			this.textPayPlansBillInAdvanceDays = new OpenDental.ValidNum();
			this.groupBoxBilling = new OpenDental.UI.GroupBox();
			this.groupBoxStatements = new OpenDental.UI.GroupBox();
			this.labelStatementShowProcBreakdownDetails = new System.Windows.Forms.Label();
			this.labelPayPlansBillInAdvanceDaysDetails = new System.Windows.Forms.Label();
			this.labelStatementsCalcDueDateDetails = new System.Windows.Forms.Label();
			this.labelBillingElectBatchMaxDetails = new System.Windows.Forms.Label();
			this.groupBoxBilling.SuspendLayout();
			this.groupBoxStatements.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkStatementsAlphabetically
			// 
			this.checkStatementsAlphabetically.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementsAlphabetically.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementsAlphabetically.Location = new System.Drawing.Point(63, 104);
			this.checkStatementsAlphabetically.Name = "checkStatementsAlphabetically";
			this.checkStatementsAlphabetically.Size = new System.Drawing.Size(377, 17);
			this.checkStatementsAlphabetically.TabIndex = 10;
			this.checkStatementsAlphabetically.Text = "Print statements alphabetically";
			// 
			// checkBillingShowProgress
			// 
			this.checkBillingShowProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBillingShowProgress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillingShowProgress.Location = new System.Drawing.Point(63, 74);
			this.checkBillingShowProgress.Name = "checkBillingShowProgress";
			this.checkBillingShowProgress.Size = new System.Drawing.Size(377, 17);
			this.checkBillingShowProgress.TabIndex = 9;
			this.checkBillingShowProgress.Text = "Show progress when sending statements";
			// 
			// label24
			// 
			this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label24.Location = new System.Drawing.Point(61, 43);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(316, 17);
			this.label24.TabIndex = 217;
			this.label24.Text = "Max number of statements per batch";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingElectBatchMax
			// 
			this.textBillingElectBatchMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBillingElectBatchMax.Location = new System.Drawing.Point(380, 40);
			this.textBillingElectBatchMax.Name = "textBillingElectBatchMax";
			this.textBillingElectBatchMax.Size = new System.Drawing.Size(60, 20);
			this.textBillingElectBatchMax.TabIndex = 8;
			this.textBillingElectBatchMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkStatementShowAdjNotes
			// 
			this.checkStatementShowAdjNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementShowAdjNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowAdjNotes.Location = new System.Drawing.Point(72, 72);
			this.checkStatementShowAdjNotes.Name = "checkStatementShowAdjNotes";
			this.checkStatementShowAdjNotes.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowAdjNotes.TabIndex = 2;
			this.checkStatementShowAdjNotes.Text = "Show notes for adjustments";
			// 
			// checkIntermingleDefault
			// 
			this.checkIntermingleDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIntermingleDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIntermingleDefault.Location = new System.Drawing.Point(63, 10);
			this.checkIntermingleDefault.Name = "checkIntermingleDefault";
			this.checkIntermingleDefault.Size = new System.Drawing.Size(377, 17);
			this.checkIntermingleDefault.TabIndex = 7;
			this.checkIntermingleDefault.Text = "Account Module statements default to intermingled mode";
			// 
			// checkStatementShowReturnAddress
			// 
			this.checkStatementShowReturnAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementShowReturnAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowReturnAddress.Location = new System.Drawing.Point(165, 10);
			this.checkStatementShowReturnAddress.Name = "checkStatementShowReturnAddress";
			this.checkStatementShowReturnAddress.Size = new System.Drawing.Size(275, 17);
			this.checkStatementShowReturnAddress.TabIndex = 0;
			this.checkStatementShowReturnAddress.Text = "Show return address";
			// 
			// checkStatementShowProcBreakdown
			// 
			this.checkStatementShowProcBreakdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementShowProcBreakdown.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowProcBreakdown.Location = new System.Drawing.Point(72, 103);
			this.checkStatementShowProcBreakdown.Name = "checkStatementShowProcBreakdown";
			this.checkStatementShowProcBreakdown.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowProcBreakdown.TabIndex = 3;
			this.checkStatementShowProcBreakdown.Text = "Show procedure breakdown";
			// 
			// checkStatementShowNotes
			// 
			this.checkStatementShowNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementShowNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowNotes.Location = new System.Drawing.Point(72, 41);
			this.checkStatementShowNotes.Name = "checkStatementShowNotes";
			this.checkStatementShowNotes.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowNotes.TabIndex = 1;
			this.checkStatementShowNotes.Text = "Show notes for payments";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(59, 172);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(318, 17);
			this.label2.TabIndex = 204;
			this.label2.Text = "Days to calculate due date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUseChartNum
			// 
			this.comboUseChartNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboUseChartNum.Location = new System.Drawing.Point(310, 134);
			this.comboUseChartNum.Name = "comboUseChartNum";
			this.comboUseChartNum.Size = new System.Drawing.Size(130, 21);
			this.comboUseChartNum.TabIndex = 4;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(112, 137);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(195, 17);
			this.label10.TabIndex = 208;
			this.label10.Text = "Account Numbers use";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label18.Location = new System.Drawing.Point(32, 206);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(345, 17);
			this.label18.TabIndex = 209;
			this.label18.Text = "Days in advance to bill payment plan amounts due";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStatementsCalcDueDate
			// 
			this.textStatementsCalcDueDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textStatementsCalcDueDate.Location = new System.Drawing.Point(380, 169);
			this.textStatementsCalcDueDate.Name = "textStatementsCalcDueDate";
			this.textStatementsCalcDueDate.ShowZero = false;
			this.textStatementsCalcDueDate.Size = new System.Drawing.Size(60, 20);
			this.textStatementsCalcDueDate.TabIndex = 5;
			this.textStatementsCalcDueDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPayPlansBillInAdvanceDays
			// 
			this.textPayPlansBillInAdvanceDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textPayPlansBillInAdvanceDays.Location = new System.Drawing.Point(380, 203);
			this.textPayPlansBillInAdvanceDays.Name = "textPayPlansBillInAdvanceDays";
			this.textPayPlansBillInAdvanceDays.Size = new System.Drawing.Size(60, 20);
			this.textPayPlansBillInAdvanceDays.TabIndex = 6;
			this.textPayPlansBillInAdvanceDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBoxBilling
			// 
			this.groupBoxBilling.Controls.Add(this.checkIntermingleDefault);
			this.groupBoxBilling.Controls.Add(this.textBillingElectBatchMax);
			this.groupBoxBilling.Controls.Add(this.checkStatementsAlphabetically);
			this.groupBoxBilling.Controls.Add(this.label24);
			this.groupBoxBilling.Controls.Add(this.checkBillingShowProgress);
			this.groupBoxBilling.Location = new System.Drawing.Point(20, 287);
			this.groupBoxBilling.Name = "groupBoxBilling";
			this.groupBoxBilling.Size = new System.Drawing.Size(450, 130);
			this.groupBoxBilling.TabIndex = 218;
			this.groupBoxBilling.Text = "Billing";
			// 
			// groupBoxStatements
			// 
			this.groupBoxStatements.Controls.Add(this.checkStatementShowReturnAddress);
			this.groupBoxStatements.Controls.Add(this.checkStatementShowNotes);
			this.groupBoxStatements.Controls.Add(this.checkStatementShowAdjNotes);
			this.groupBoxStatements.Controls.Add(this.checkStatementShowProcBreakdown);
			this.groupBoxStatements.Controls.Add(this.comboUseChartNum);
			this.groupBoxStatements.Controls.Add(this.textPayPlansBillInAdvanceDays);
			this.groupBoxStatements.Controls.Add(this.label18);
			this.groupBoxStatements.Controls.Add(this.label10);
			this.groupBoxStatements.Controls.Add(this.textStatementsCalcDueDate);
			this.groupBoxStatements.Controls.Add(this.label2);
			this.groupBoxStatements.Location = new System.Drawing.Point(20, 40);
			this.groupBoxStatements.Name = "groupBoxStatements";
			this.groupBoxStatements.Size = new System.Drawing.Size(450, 233);
			this.groupBoxStatements.TabIndex = 219;
			this.groupBoxStatements.Text = "Statements";
			// 
			// labelStatementShowProcBreakdownDetails
			// 
			this.labelStatementShowProcBreakdownDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatementShowProcBreakdownDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelStatementShowProcBreakdownDetails.Location = new System.Drawing.Point(476, 142);
			this.labelStatementShowProcBreakdownDetails.Name = "labelStatementShowProcBreakdownDetails";
			this.labelStatementShowProcBreakdownDetails.Size = new System.Drawing.Size(498, 17);
			this.labelStatementShowProcBreakdownDetails.TabIndex = 364;
			this.labelStatementShowProcBreakdownDetails.Text = "shows in the Description column as patient portion, insurance paid, write-off, ad" +
    "justment, etc.";
			this.labelStatementShowProcBreakdownDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPayPlansBillInAdvanceDaysDetails
			// 
			this.labelPayPlansBillInAdvanceDaysDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPayPlansBillInAdvanceDaysDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPayPlansBillInAdvanceDaysDetails.Location = new System.Drawing.Point(476, 240);
			this.labelPayPlansBillInAdvanceDaysDetails.Name = "labelPayPlansBillInAdvanceDaysDetails";
			this.labelPayPlansBillInAdvanceDaysDetails.Size = new System.Drawing.Size(498, 28);
			this.labelPayPlansBillInAdvanceDaysDetails.TabIndex = 365;
			this.labelPayPlansBillInAdvanceDaysDetails.Text = "usually 10 or 15, causes statements to be triggered to print, cannot be used with" +
    " Dynamic Payment Plans";
			this.labelPayPlansBillInAdvanceDaysDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelStatementsCalcDueDateDetails
			// 
			this.labelStatementsCalcDueDateDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatementsCalcDueDateDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelStatementsCalcDueDateDetails.Location = new System.Drawing.Point(476, 210);
			this.labelStatementsCalcDueDateDetails.Name = "labelStatementsCalcDueDateDetails";
			this.labelStatementsCalcDueDateDetails.Size = new System.Drawing.Size(498, 17);
			this.labelStatementsCalcDueDateDetails.TabIndex = 366;
			this.labelStatementsCalcDueDateDetails.Text = "usually 10 or 15, leave blank to show \"Due on Receipt\"";
			this.labelStatementsCalcDueDateDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBillingElectBatchMaxDetails
			// 
			this.labelBillingElectBatchMaxDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelBillingElectBatchMaxDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelBillingElectBatchMaxDetails.Location = new System.Drawing.Point(476, 330);
			this.labelBillingElectBatchMaxDetails.Name = "labelBillingElectBatchMaxDetails";
			this.labelBillingElectBatchMaxDetails.Size = new System.Drawing.Size(498, 17);
			this.labelBillingElectBatchMaxDetails.TabIndex = 367;
			this.labelBillingElectBatchMaxDetails.Text = "0 for no limit";
			this.labelBillingElectBatchMaxDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlManageBillingStatements
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelBillingElectBatchMaxDetails);
			this.Controls.Add(this.labelStatementsCalcDueDateDetails);
			this.Controls.Add(this.labelPayPlansBillInAdvanceDaysDetails);
			this.Controls.Add(this.labelStatementShowProcBreakdownDetails);
			this.Controls.Add(this.groupBoxStatements);
			this.Controls.Add(this.groupBoxBilling);
			this.Name = "UserControlManageBillingStatements";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxBilling.ResumeLayout(false);
			this.groupBoxBilling.PerformLayout();
			this.groupBoxStatements.ResumeLayout(false);
			this.groupBoxStatements.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.CheckBox checkStatementsAlphabetically;
		private OpenDental.UI.CheckBox checkBillingShowProgress;
		private System.Windows.Forms.Label label24;
		private ValidNum textBillingElectBatchMax;
		private OpenDental.UI.CheckBox checkStatementShowAdjNotes;
		private OpenDental.UI.CheckBox checkIntermingleDefault;
		private OpenDental.UI.CheckBox checkStatementShowReturnAddress;
		private OpenDental.UI.CheckBox checkStatementShowProcBreakdown;
		private OpenDental.UI.CheckBox checkStatementShowNotes;
		private System.Windows.Forms.Label label2;
		private UI.ComboBox comboUseChartNum;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label18;
		private ValidNum textStatementsCalcDueDate;
		private ValidNum textPayPlansBillInAdvanceDays;
		private UI.GroupBox groupBoxBilling;
		private UI.GroupBox groupBoxStatements;
		private System.Windows.Forms.Label labelStatementShowProcBreakdownDetails;
		private System.Windows.Forms.Label labelPayPlansBillInAdvanceDaysDetails;
		private System.Windows.Forms.Label labelStatementsCalcDueDateDetails;
		private System.Windows.Forms.Label labelBillingElectBatchMaxDetails;
	}
}
