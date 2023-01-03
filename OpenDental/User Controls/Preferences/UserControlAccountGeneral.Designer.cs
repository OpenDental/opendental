
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
			this.groupBoxFunctionality = new OpenDental.UI.GroupBox();
			this.labelAgingBeginDateT = new System.Windows.Forms.Label();
			this.textAgingBeginDateT = new System.Windows.Forms.TextBox();
			this.labelAutoAgingRunTime = new System.Windows.Forms.Label();
			this.textAutoAgingRunTime = new System.Windows.Forms.TextBox();
			this.checkBalancesDontSubtractIns = new OpenDental.UI.CheckBox();
			this.checkAccountShowPaymentNums = new OpenDental.UI.CheckBox();
			this.checkStatementInvoiceGridShowWriteoffs = new OpenDental.UI.CheckBox();
			this.checkAllowFutureTrans = new OpenDental.UI.CheckBox();
			this.groupCommLogs = new OpenDental.UI.GroupBox();
			this.checkCommLogAutoSave = new OpenDental.UI.CheckBox();
			this.checkShowFamilyCommByDefault = new OpenDental.UI.CheckBox();
			this.labelAllowFutureTransDetails = new System.Windows.Forms.Label();
			this.groupBoxFutureDatedTransactions = new OpenDental.UI.GroupBox();
			this.labelFutureDatedOptions = new System.Windows.Forms.Label();
			this.checkAllowFutureDebits = new OpenDental.UI.CheckBox();
			this.checkAllowFuturePayments = new OpenDental.UI.CheckBox();
			this.groupBoxFunctionality.SuspendLayout();
			this.groupCommLogs.SuspendLayout();
			this.groupBoxFutureDatedTransactions.SuspendLayout();
			this.SuspendLayout();
			// 
			// label62
			// 
			this.label62.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label62.Location = new System.Drawing.Point(172, 413);
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
			this.labelAccountShowPaymentNumsDetails.Location = new System.Drawing.Point(476, 162);
			this.labelAccountShowPaymentNumsDetails.Name = "labelAccountShowPaymentNumsDetails";
			this.labelAccountShowPaymentNumsDetails.Size = new System.Drawing.Size(498, 17);
			this.labelAccountShowPaymentNumsDetails.TabIndex = 325;
			this.labelAccountShowPaymentNumsDetails.Text = "in the payment description, useful for foreign offices";
			this.labelAccountShowPaymentNumsDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butClearAgingBeginDateT
			// 
			this.butClearAgingBeginDateT.Location = new System.Drawing.Point(397, 137);
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
			this.groupBoxFunctionality.Controls.Add(this.checkAccountShowPaymentNums);
			this.groupBoxFunctionality.Controls.Add(this.checkStatementInvoiceGridShowWriteoffs);
			this.groupBoxFunctionality.Location = new System.Drawing.Point(20, 122);
			this.groupBoxFunctionality.Name = "groupBoxFunctionality";
			this.groupBoxFunctionality.Size = new System.Drawing.Size(450, 168);
			this.groupBoxFunctionality.TabIndex = 307;
			this.groupBoxFunctionality.Text = "Functionality";
			// 
			// labelAgingBeginDateT
			// 
			this.labelAgingBeginDateT.Location = new System.Drawing.Point(8, 135);
			this.labelAgingBeginDateT.Name = "labelAgingBeginDateT";
			this.labelAgingBeginDateT.Size = new System.Drawing.Size(230, 27);
			this.labelAgingBeginDateT.TabIndex = 314;
			this.labelAgingBeginDateT.Text = "DateTime the currently running aging started\r\nUsually blank";
			this.labelAgingBeginDateT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAgingBeginDateT
			// 
			this.textAgingBeginDateT.Location = new System.Drawing.Point(241, 138);
			this.textAgingBeginDateT.Name = "textAgingBeginDateT";
			this.textAgingBeginDateT.ReadOnly = true;
			this.textAgingBeginDateT.Size = new System.Drawing.Size(150, 20);
			this.textAgingBeginDateT.TabIndex = 313;
			this.textAgingBeginDateT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelAutoAgingRunTime
			// 
			this.labelAutoAgingRunTime.Location = new System.Drawing.Point(13, 106);
			this.labelAutoAgingRunTime.Name = "labelAutoAgingRunTime";
			this.labelAutoAgingRunTime.Size = new System.Drawing.Size(350, 17);
			this.labelAutoAgingRunTime.TabIndex = 312;
			this.labelAutoAgingRunTime.Text = "Automated aging run time, Leave blank to disable";
			this.labelAutoAgingRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAutoAgingRunTime
			// 
			this.textAutoAgingRunTime.Location = new System.Drawing.Point(366, 103);
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
			// 
			// checkAllowFutureTrans
			// 
			this.checkAllowFutureTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowFutureTrans.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureTrans.Location = new System.Drawing.Point(142, 10);
			this.checkAllowFutureTrans.Name = "checkAllowFutureTrans";
			this.checkAllowFutureTrans.Size = new System.Drawing.Size(298, 15);
			this.checkAllowFutureTrans.TabIndex = 18;
			this.checkAllowFutureTrans.Text = "Allow future dated transactions";
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
			this.checkShowFamilyCommByDefault.Click += new System.EventHandler(this.checkShowFamilyCommByDefault_Click);
			// 
			// labelAllowFutureTransDetails
			// 
			this.labelAllowFutureTransDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAllowFutureTransDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelAllowFutureTransDetails.Location = new System.Drawing.Point(476, 312);
			this.labelAllowFutureTransDetails.Name = "labelAllowFutureTransDetails";
			this.labelAllowFutureTransDetails.Size = new System.Drawing.Size(498, 17);
			this.labelAllowFutureTransDetails.TabIndex = 371;
			this.labelAllowFutureTransDetails.Text = "including patient payments and insurance payments";
			this.labelAllowFutureTransDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxFutureDatedTransactions
			// 
			this.groupBoxFutureDatedTransactions.Controls.Add(this.labelFutureDatedOptions);
			this.groupBoxFutureDatedTransactions.Controls.Add(this.checkAllowFutureDebits);
			this.groupBoxFutureDatedTransactions.Controls.Add(this.checkAllowFuturePayments);
			this.groupBoxFutureDatedTransactions.Controls.Add(this.checkAllowFutureTrans);
			this.groupBoxFutureDatedTransactions.Location = new System.Drawing.Point(20, 304);
			this.groupBoxFutureDatedTransactions.Name = "groupBoxFutureDatedTransactions";
			this.groupBoxFutureDatedTransactions.Size = new System.Drawing.Size(450, 98);
			this.groupBoxFutureDatedTransactions.TabIndex = 372;
			this.groupBoxFutureDatedTransactions.Text = "Future Dated Transactions";
			// 
			// labelFutureDatedOptions
			// 
			this.labelFutureDatedOptions.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelFutureDatedOptions.Location = new System.Drawing.Point(93, 28);
			this.labelFutureDatedOptions.Name = "labelFutureDatedOptions";
			this.labelFutureDatedOptions.Size = new System.Drawing.Size(350, 17);
			this.labelFutureDatedOptions.TabIndex = 329;
			this.labelFutureDatedOptions.Text = "(or enable one of the two options below)";
			this.labelFutureDatedOptions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowFutureDebits
			// 
			this.checkAllowFutureDebits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowFutureDebits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureDebits.Location = new System.Drawing.Point(100, 71);
			this.checkAllowFutureDebits.Name = "checkAllowFutureDebits";
			this.checkAllowFutureDebits.Size = new System.Drawing.Size(340, 17);
			this.checkAllowFutureDebits.TabIndex = 290;
			this.checkAllowFutureDebits.Text = "Allow future dated patient payments";
			// 
			// checkAllowFuturePayments
			// 
			this.checkAllowFuturePayments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFuturePayments.Location = new System.Drawing.Point(73, 48);
			this.checkAllowFuturePayments.Name = "checkAllowFuturePayments";
			this.checkAllowFuturePayments.Size = new System.Drawing.Size(367, 17);
			this.checkAllowFuturePayments.TabIndex = 289;
			this.checkAllowFuturePayments.Text = "Allow future dated insurance payments";
			// 
			// UserControlAccountGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxFutureDatedTransactions);
			this.Controls.Add(this.labelAllowFutureTransDetails);
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
			this.groupBoxFutureDatedTransactions.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBox groupCommLogs;
		private OpenDental.UI.CheckBox checkCommLogAutoSave;
		private OpenDental.UI.CheckBox checkShowFamilyCommByDefault;
		private OpenDental.UI.CheckBox checkBalancesDontSubtractIns;
		private OpenDental.UI.CheckBox checkAllowFutureTrans;
		private OpenDental.UI.CheckBox checkAccountShowPaymentNums;
		private OpenDental.UI.CheckBox checkStatementInvoiceGridShowWriteoffs;
		private System.Windows.Forms.Label label62;
		private UI.GroupBox groupBoxFunctionality;
		private System.Windows.Forms.Label labelCommLogAutoSaveDetails;
		private System.Windows.Forms.Label labelAccountShowPaymentNumsDetails;
		private System.Windows.Forms.Label labelAgingBeginDateT;
		private System.Windows.Forms.TextBox textAgingBeginDateT;
		private System.Windows.Forms.Label labelAutoAgingRunTime;
		private System.Windows.Forms.TextBox textAutoAgingRunTime;
		private UI.Button butClearAgingBeginDateT;
		private System.Windows.Forms.Label labelAllowFutureTransDetails;
		private UI.GroupBox groupBoxFutureDatedTransactions;
		private OpenDental.UI.CheckBox checkAllowFuturePayments;
		private OpenDental.UI.CheckBox checkAllowFutureDebits;
		private System.Windows.Forms.Label labelFutureDatedOptions;
	}
}
