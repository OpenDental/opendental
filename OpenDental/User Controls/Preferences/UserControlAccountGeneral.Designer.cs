
namespace OpenDental {
	partial class UserControlAccountGeneral {
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
			this.label62 = new System.Windows.Forms.Label();
			this.labelCommLogAutoSaveDetails = new System.Windows.Forms.Label();
			this.labelAccountShowPaymentNumsDetails = new System.Windows.Forms.Label();
			this.butClearAgingBeginDateT = new OpenDental.UI.Button();
			this.groupBoxFunctionality = new OpenDental.UI.GroupBoxOD();
			this.labelAgingBeginDateT = new System.Windows.Forms.Label();
			this.textAgingBeginDateT = new System.Windows.Forms.TextBox();
			this.labelAutoAgingRunTime = new System.Windows.Forms.Label();
			this.textAutoAgingRunTime = new System.Windows.Forms.TextBox();
			this.checkBalancesDontSubtractIns = new System.Windows.Forms.CheckBox();
			this.checkAgingProcLifo = new System.Windows.Forms.CheckBox();
			this.checkAccountShowPaymentNums = new System.Windows.Forms.CheckBox();
			this.checkAllowFutureTrans = new System.Windows.Forms.CheckBox();
			this.checkStatementInvoiceGridShowWriteoffs = new System.Windows.Forms.CheckBox();
			this.groupCommLogs = new OpenDental.UI.GroupBoxOD();
			this.checkCommLogAutoSave = new System.Windows.Forms.CheckBox();
			this.checkShowFamilyCommByDefault = new System.Windows.Forms.CheckBox();
			this.butAgingProcLifoDetails = new OpenDental.UI.Button();
			this.groupBoxFunctionality.SuspendLayout();
			this.groupCommLogs.SuspendLayout();
			this.SuspendLayout();
			// 
			// label62
			// 
			this.label62.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label62.Location = new System.Drawing.Point(172, 366);
			this.label62.Name = "label62";
			this.label62.Size = new System.Drawing.Size(288, 48);
			this.label62.TabIndex = 306;
			this.label62.Text = "Allocation options for Line Item Accounting have been moved to a different window" +
    ".\r\nSee Main Menu, Setup, Account, Allocations.";
			this.label62.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCommLogAutoSaveDetails
			// 
			this.labelCommLogAutoSaveDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCommLogAutoSaveDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelCommLogAutoSaveDetails.Location = new System.Drawing.Point(476, 49);
			this.labelCommLogAutoSaveDetails.Name = "labelCommLogAutoSaveDetails";
			this.labelCommLogAutoSaveDetails.Size = new System.Drawing.Size(498, 17);
			this.labelCommLogAutoSaveDetails.TabIndex = 321;
			this.labelCommLogAutoSaveDetails.Text = "every ten seconds";
			this.labelCommLogAutoSaveDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAccountShowPaymentNumsDetails
			// 
			this.labelAccountShowPaymentNumsDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAccountShowPaymentNumsDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelAccountShowPaymentNumsDetails.Location = new System.Drawing.Point(473, 162);
			this.labelAccountShowPaymentNumsDetails.Name = "labelAccountShowPaymentNumsDetails";
			this.labelAccountShowPaymentNumsDetails.Size = new System.Drawing.Size(498, 17);
			this.labelAccountShowPaymentNumsDetails.TabIndex = 325;
			this.labelAccountShowPaymentNumsDetails.Text = "in the payment description, useful for foreign offices";
			this.labelAccountShowPaymentNumsDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butClearAgingBeginDateT
			// 
			this.butClearAgingBeginDateT.Location = new System.Drawing.Point(397, 198);
			this.butClearAgingBeginDateT.Name = "butClearAgingBeginDateT";
			this.butClearAgingBeginDateT.Size = new System.Drawing.Size(43, 21);
			this.butClearAgingBeginDateT.TabIndex = 328;
			this.butClearAgingBeginDateT.Text = "Clear";
			this.butClearAgingBeginDateT.Click += new System.EventHandler(this.butClearAgingBeginDateT_Click);
			// 
			// groupBoxFunctionality
			// 
			this.groupBoxFunctionality.Controls.Add(this.butClearAgingBeginDateT);
			this.groupBoxFunctionality.Controls.Add(this.labelAgingBeginDateT);
			this.groupBoxFunctionality.Controls.Add(this.textAgingBeginDateT);
			this.groupBoxFunctionality.Controls.Add(this.labelAutoAgingRunTime);
			this.groupBoxFunctionality.Controls.Add(this.textAutoAgingRunTime);
			this.groupBoxFunctionality.Controls.Add(this.checkBalancesDontSubtractIns);
			this.groupBoxFunctionality.Controls.Add(this.checkAgingProcLifo);
			this.groupBoxFunctionality.Controls.Add(this.checkAccountShowPaymentNums);
			this.groupBoxFunctionality.Controls.Add(this.checkAllowFutureTrans);
			this.groupBoxFunctionality.Controls.Add(this.checkStatementInvoiceGridShowWriteoffs);
			this.groupBoxFunctionality.Location = new System.Drawing.Point(20, 122);
			this.groupBoxFunctionality.Name = "groupBoxFunctionality";
			this.groupBoxFunctionality.Size = new System.Drawing.Size(450, 233);
			this.groupBoxFunctionality.TabIndex = 307;
			this.groupBoxFunctionality.Text = "Functionality";
			// 
			// labelAgingBeginDateT
			// 
			this.labelAgingBeginDateT.Location = new System.Drawing.Point(6, 196);
			this.labelAgingBeginDateT.Name = "labelAgingBeginDateT";
			this.labelAgingBeginDateT.Size = new System.Drawing.Size(230, 27);
			this.labelAgingBeginDateT.TabIndex = 314;
			this.labelAgingBeginDateT.Text = "DateTime the currently running aging started\r\nUsually blank";
			this.labelAgingBeginDateT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAgingBeginDateT
			// 
			this.textAgingBeginDateT.Location = new System.Drawing.Point(239, 199);
			this.textAgingBeginDateT.Name = "textAgingBeginDateT";
			this.textAgingBeginDateT.ReadOnly = true;
			this.textAgingBeginDateT.Size = new System.Drawing.Size(150, 20);
			this.textAgingBeginDateT.TabIndex = 313;
			this.textAgingBeginDateT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelAutoAgingRunTime
			// 
			this.labelAutoAgingRunTime.Location = new System.Drawing.Point(13, 168);
			this.labelAutoAgingRunTime.Name = "labelAutoAgingRunTime";
			this.labelAutoAgingRunTime.Size = new System.Drawing.Size(350, 17);
			this.labelAutoAgingRunTime.TabIndex = 312;
			this.labelAutoAgingRunTime.Text = "Automated aging run time, Leave blank to disable";
			this.labelAutoAgingRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAutoAgingRunTime
			// 
			this.textAutoAgingRunTime.Location = new System.Drawing.Point(366, 165);
			this.textAutoAgingRunTime.Name = "textAutoAgingRunTime";
			this.textAutoAgingRunTime.Size = new System.Drawing.Size(74, 20);
			this.textAutoAgingRunTime.TabIndex = 21;
			this.textAutoAgingRunTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkBalancesDontSubtractIns
			// 
			this.checkBalancesDontSubtractIns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBalancesDontSubtractIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBalancesDontSubtractIns.Location = new System.Drawing.Point(90, 10);
			this.checkBalancesDontSubtractIns.Name = "checkBalancesDontSubtractIns";
			this.checkBalancesDontSubtractIns.Size = new System.Drawing.Size(350, 17);
			this.checkBalancesDontSubtractIns.TabIndex = 12;
			this.checkBalancesDontSubtractIns.Text = "Balances don\'t subtract insurance estimate";
			this.checkBalancesDontSubtractIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAgingProcLifo
			// 
			this.checkAgingProcLifo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAgingProcLifo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingProcLifo.Location = new System.Drawing.Point(5, 134);
			this.checkAgingProcLifo.Name = "checkAgingProcLifo";
			this.checkAgingProcLifo.Size = new System.Drawing.Size(435, 17);
			this.checkAgingProcLifo.TabIndex = 19;
			this.checkAgingProcLifo.Text = "Transactions attached to a procedure offset each other before aging";
			this.checkAgingProcLifo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingProcLifo.ThreeState = true;
			// 
			// checkAccountShowPaymentNums
			// 
			this.checkAccountShowPaymentNums.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAccountShowPaymentNums.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAccountShowPaymentNums.Location = new System.Drawing.Point(89, 41);
			this.checkAccountShowPaymentNums.Name = "checkAccountShowPaymentNums";
			this.checkAccountShowPaymentNums.Size = new System.Drawing.Size(351, 17);
			this.checkAccountShowPaymentNums.TabIndex = 14;
			this.checkAccountShowPaymentNums.Text = "Show payment numbers in Account Module";
			this.checkAccountShowPaymentNums.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowFutureTrans
			// 
			this.checkAllowFutureTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowFutureTrans.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureTrans.Location = new System.Drawing.Point(89, 103);
			this.checkAllowFutureTrans.Name = "checkAllowFutureTrans";
			this.checkAllowFutureTrans.Size = new System.Drawing.Size(351, 17);
			this.checkAllowFutureTrans.TabIndex = 18;
			this.checkAllowFutureTrans.Text = "Allow future dated transactions";
			this.checkAllowFutureTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStatementInvoiceGridShowWriteoffs
			// 
			this.checkStatementInvoiceGridShowWriteoffs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStatementInvoiceGridShowWriteoffs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementInvoiceGridShowWriteoffs.Location = new System.Drawing.Point(89, 72);
			this.checkStatementInvoiceGridShowWriteoffs.Name = "checkStatementInvoiceGridShowWriteoffs";
			this.checkStatementInvoiceGridShowWriteoffs.Size = new System.Drawing.Size(351, 17);
			this.checkStatementInvoiceGridShowWriteoffs.TabIndex = 16;
			this.checkStatementInvoiceGridShowWriteoffs.Text = "Invoice payments grid shows write-offs\r\n";
			this.checkStatementInvoiceGridShowWriteoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupCommLogs
			// 
			this.groupCommLogs.BackColor = System.Drawing.Color.White;
			this.groupCommLogs.Controls.Add(this.checkCommLogAutoSave);
			this.groupCommLogs.Controls.Add(this.checkShowFamilyCommByDefault);
			this.groupCommLogs.Location = new System.Drawing.Point(20, 40);
			this.groupCommLogs.Name = "groupCommLogs";
			this.groupCommLogs.Size = new System.Drawing.Size(450, 68);
			this.groupCommLogs.TabIndex = 4;
			this.groupCommLogs.Text = "Commlogs";
			// 
			// checkCommLogAutoSave
			// 
			this.checkCommLogAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCommLogAutoSave.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCommLogAutoSave.Location = new System.Drawing.Point(235, 10);
			this.checkCommLogAutoSave.Name = "checkCommLogAutoSave";
			this.checkCommLogAutoSave.Size = new System.Drawing.Size(205, 17);
			this.checkCommLogAutoSave.TabIndex = 225;
			this.checkCommLogAutoSave.Text = "Commlogs auto save";
			this.checkCommLogAutoSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCommLogAutoSave.UseVisualStyleBackColor = true;
			// 
			// checkShowFamilyCommByDefault
			// 
			this.checkShowFamilyCommByDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowFamilyCommByDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFamilyCommByDefault.Location = new System.Drawing.Point(210, 41);
			this.checkShowFamilyCommByDefault.Name = "checkShowFamilyCommByDefault";
			this.checkShowFamilyCommByDefault.Size = new System.Drawing.Size(230, 17);
			this.checkShowFamilyCommByDefault.TabIndex = 75;
			this.checkShowFamilyCommByDefault.Text = "Show family commlog entries by default";
			this.checkShowFamilyCommByDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFamilyCommByDefault.Click += new System.EventHandler(this.checkShowFamilyCommByDefault_Click);
			// 
			// butAgingProcLifoDetails
			// 
			this.butAgingProcLifoDetails.ForeColor = System.Drawing.Color.Black;
			this.butAgingProcLifoDetails.Location = new System.Drawing.Point(476, 253);
			this.butAgingProcLifoDetails.Name = "butAgingProcLifoDetails";
			this.butAgingProcLifoDetails.Size = new System.Drawing.Size(64, 21);
			this.butAgingProcLifoDetails.TabIndex = 370;
			this.butAgingProcLifoDetails.Text = "Details";
			this.butAgingProcLifoDetails.Click += new System.EventHandler(this.butAgingProcLifoDetails_Click);
			// 
			// UserControlAccountGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.butAgingProcLifoDetails);
			this.Controls.Add(this.labelAccountShowPaymentNumsDetails);
			this.Controls.Add(this.labelCommLogAutoSaveDetails);
			this.Controls.Add(this.groupBoxFunctionality);
			this.Controls.Add(this.label62);
			this.Controls.Add(this.groupCommLogs);
			this.Name = "UserControlAccountGeneral";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxFunctionality.ResumeLayout(false);
			this.groupBoxFunctionality.PerformLayout();
			this.groupCommLogs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBoxOD groupCommLogs;
		private System.Windows.Forms.CheckBox checkCommLogAutoSave;
		private System.Windows.Forms.CheckBox checkShowFamilyCommByDefault;
		private System.Windows.Forms.CheckBox checkAgingProcLifo;
		private System.Windows.Forms.CheckBox checkBalancesDontSubtractIns;
		private System.Windows.Forms.CheckBox checkAllowFutureTrans;
		private System.Windows.Forms.CheckBox checkAccountShowPaymentNums;
		private System.Windows.Forms.CheckBox checkStatementInvoiceGridShowWriteoffs;
		private System.Windows.Forms.Label label62;
		private UI.GroupBoxOD groupBoxFunctionality;
		private System.Windows.Forms.Label labelCommLogAutoSaveDetails;
		private System.Windows.Forms.Label labelAccountShowPaymentNumsDetails;
		private System.Windows.Forms.Label labelAgingBeginDateT;
		private System.Windows.Forms.TextBox textAgingBeginDateT;
		private System.Windows.Forms.Label labelAutoAgingRunTime;
		private System.Windows.Forms.TextBox textAutoAgingRunTime;
		private UI.Button butClearAgingBeginDateT;
		private UI.Button butAgingProcLifoDetails;
	}
}
