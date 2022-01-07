using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEtrans835ClaimPay {
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;

		///<summary>Clean up any resources being used.</summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components!=null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans835ClaimPay));
			this.textInsPayAllowed = new System.Windows.Forms.TextBox();
			this.textClaimFee = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.gridClaimAdjustments = new OpenDental.UI.GridOD();
			this.gridProcedureBreakdown = new OpenDental.UI.GridOD();
			this.gridPayments = new OpenDental.UI.GridOD();
			this.textDedApplied = new System.Windows.Forms.TextBox();
			this.textInsPayAmt = new System.Windows.Forms.TextBox();
			this.textEobInsPayAmt = new System.Windows.Forms.TextBox();
			this.textEobDedApplied = new System.Windows.Forms.TextBox();
			this.textEobInsPayAllowed = new System.Windows.Forms.TextBox();
			this.textEobClaimFee = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butViewEobDetails = new OpenDental.UI.Button();
			this.butWriteOff = new OpenDental.UI.Button();
			this.butDeductible = new OpenDental.UI.Button();
			this.textWriteOff = new OpenDental.ValidDouble();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkIncludeWOPercCoPay = new System.Windows.Forms.CheckBox();
			this.butSplitProcs = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textInsPayAllowed
			// 
			this.textInsPayAllowed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textInsPayAllowed.Location = new System.Drawing.Point(485, 608);
			this.textInsPayAllowed.Name = "textInsPayAllowed";
			this.textInsPayAllowed.ReadOnly = true;
			this.textInsPayAllowed.Size = new System.Drawing.Size(62, 20);
			this.textInsPayAllowed.TabIndex = 116;
			this.textInsPayAllowed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textClaimFee
			// 
			this.textClaimFee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textClaimFee.Location = new System.Drawing.Point(361, 608);
			this.textClaimFee.Name = "textClaimFee";
			this.textClaimFee.ReadOnly = true;
			this.textClaimFee.Size = new System.Drawing.Size(62, 20);
			this.textClaimFee.TabIndex = 118;
			this.textClaimFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(207, 611);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 16);
			this.label1.TabIndex = 117;
			this.label1.Text = "Totals";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(496, 658);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(311, 39);
			this.label2.TabIndex = 122;
			this.label2.Text = "Before you click OK, the Deductible and the Ins Pay amounts should exactly match " +
    "the insurance EOB.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(20, 622);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(116, 34);
			this.label3.TabIndex = 123;
			this.label3.Text = "Assign to selected payment line:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(164, 627);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(108, 29);
			this.label4.TabIndex = 124;
			this.label4.Text = "On all unpaid procedure amounts:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridClaimAdjustments
			// 
			this.gridClaimAdjustments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClaimAdjustments.Location = new System.Drawing.Point(9, 12);
			this.gridClaimAdjustments.Name = "gridClaimAdjustments";
			this.gridClaimAdjustments.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridClaimAdjustments.Size = new System.Drawing.Size(956, 100);
			this.gridClaimAdjustments.TabIndex = 200;
			this.gridClaimAdjustments.TabStop = false;
			this.gridClaimAdjustments.Title = "EOB Claim Adjustments";
			this.gridClaimAdjustments.TranslationName = "FormEtrans835Edit";
			this.gridClaimAdjustments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClaimAdjustments_CellDoubleClick);
			// 
			// gridProcedureBreakdown
			// 
			this.gridProcedureBreakdown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProcedureBreakdown.Location = new System.Drawing.Point(9, 118);
			this.gridProcedureBreakdown.Name = "gridProcedureBreakdown";
			this.gridProcedureBreakdown.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProcedureBreakdown.Size = new System.Drawing.Size(956, 168);
			this.gridProcedureBreakdown.TabIndex = 199;
			this.gridProcedureBreakdown.TabStop = false;
			this.gridProcedureBreakdown.Title = "EOB Procedure Breakdown";
			this.gridProcedureBreakdown.TranslationName = "FormEtrans835Edit";
			this.gridProcedureBreakdown.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProcedureBreakdown_CellDoubleClick);
			// 
			// gridPayments
			// 
			this.gridPayments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPayments.Location = new System.Drawing.Point(9, 345);
			this.gridPayments.Name = "gridPayments";
			this.gridPayments.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridPayments.Size = new System.Drawing.Size(956, 257);
			this.gridPayments.TabIndex = 125;
			this.gridPayments.Title = "Enter Payments";
			this.gridPayments.TranslationName = "TableClaimProc";
			this.gridPayments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridPayments.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPayments_CellClick);
			this.gridPayments.CellTextChanged += new System.EventHandler(this.gridMain_CellTextChanged);
			// 
			// textDedApplied
			// 
			this.textDedApplied.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textDedApplied.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textDedApplied.Location = new System.Drawing.Point(423, 608);
			this.textDedApplied.Name = "textDedApplied";
			this.textDedApplied.ReadOnly = true;
			this.textDedApplied.Size = new System.Drawing.Size(62, 20);
			this.textDedApplied.TabIndex = 202;
			this.textDedApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textInsPayAmt
			// 
			this.textInsPayAmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textInsPayAmt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textInsPayAmt.Location = new System.Drawing.Point(547, 608);
			this.textInsPayAmt.Name = "textInsPayAmt";
			this.textInsPayAmt.ReadOnly = true;
			this.textInsPayAmt.Size = new System.Drawing.Size(62, 20);
			this.textInsPayAmt.TabIndex = 203;
			this.textInsPayAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textEobInsPayAmt
			// 
			this.textEobInsPayAmt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textEobInsPayAmt.Location = new System.Drawing.Point(517, 292);
			this.textEobInsPayAmt.Name = "textEobInsPayAmt";
			this.textEobInsPayAmt.ReadOnly = true;
			this.textEobInsPayAmt.Size = new System.Drawing.Size(62, 20);
			this.textEobInsPayAmt.TabIndex = 209;
			this.textEobInsPayAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textEobDedApplied
			// 
			this.textEobDedApplied.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textEobDedApplied.Location = new System.Drawing.Point(393, 292);
			this.textEobDedApplied.Name = "textEobDedApplied";
			this.textEobDedApplied.ReadOnly = true;
			this.textEobDedApplied.Size = new System.Drawing.Size(62, 20);
			this.textEobDedApplied.TabIndex = 208;
			this.textEobDedApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textEobInsPayAllowed
			// 
			this.textEobInsPayAllowed.Location = new System.Drawing.Point(455, 292);
			this.textEobInsPayAllowed.Name = "textEobInsPayAllowed";
			this.textEobInsPayAllowed.ReadOnly = true;
			this.textEobInsPayAllowed.Size = new System.Drawing.Size(62, 20);
			this.textEobInsPayAllowed.TabIndex = 204;
			this.textEobInsPayAllowed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textEobClaimFee
			// 
			this.textEobClaimFee.Location = new System.Drawing.Point(331, 292);
			this.textEobClaimFee.Name = "textEobClaimFee";
			this.textEobClaimFee.ReadOnly = true;
			this.textEobClaimFee.Size = new System.Drawing.Size(62, 20);
			this.textEobClaimFee.TabIndex = 206;
			this.textEobClaimFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(177, 295);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(150, 16);
			this.label5.TabIndex = 205;
			this.label5.Text = "EOB Totals";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butViewEobDetails
			// 
			this.butViewEobDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butViewEobDetails.Location = new System.Drawing.Point(331, 659);
			this.butViewEobDetails.Name = "butViewEobDetails";
			this.butViewEobDetails.Size = new System.Drawing.Size(135, 25);
			this.butViewEobDetails.TabIndex = 201;
			this.butViewEobDetails.Text = "EOB Claim Details";
			this.butViewEobDetails.Click += new System.EventHandler(this.butViewEobDetails_Click);
			// 
			// butWriteOff
			// 
			this.butWriteOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butWriteOff.Location = new System.Drawing.Point(163, 659);
			this.butWriteOff.Name = "butWriteOff";
			this.butWriteOff.Size = new System.Drawing.Size(90, 25);
			this.butWriteOff.TabIndex = 121;
			this.butWriteOff.Text = "&Write Off";
			this.butWriteOff.Click += new System.EventHandler(this.butWriteOff_Click);
			// 
			// butDeductible
			// 
			this.butDeductible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeductible.Location = new System.Drawing.Point(23, 659);
			this.butDeductible.Name = "butDeductible";
			this.butDeductible.Size = new System.Drawing.Size(92, 25);
			this.butDeductible.TabIndex = 120;
			this.butDeductible.Text = "&Deductible";
			this.butDeductible.Click += new System.EventHandler(this.butDeductible_Click);
			// 
			// textWriteOff
			// 
			this.textWriteOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textWriteOff.Location = new System.Drawing.Point(609, 608);
			this.textWriteOff.MaxVal = 100000000D;
			this.textWriteOff.MinVal = -100000000D;
			this.textWriteOff.Name = "textWriteOff";
			this.textWriteOff.ReadOnly = true;
			this.textWriteOff.Size = new System.Drawing.Size(62, 20);
			this.textWriteOff.TabIndex = 119;
			this.textWriteOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(890, 659);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(809, 659);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkIncludeWOPercCoPay
			// 
			this.checkIncludeWOPercCoPay.Location = new System.Drawing.Point(9, 322);
			this.checkIncludeWOPercCoPay.Name = "checkIncludeWOPercCoPay";
			this.checkIncludeWOPercCoPay.Size = new System.Drawing.Size(570, 19);
			this.checkIncludeWOPercCoPay.TabIndex = 210;
			this.checkIncludeWOPercCoPay.Text = "Include WriteOffs for Category Percentage and Medicaid/Flat CoPay plans";
			this.checkIncludeWOPercCoPay.UseVisualStyleBackColor = true;
			this.checkIncludeWOPercCoPay.CheckedChanged += new System.EventHandler(this.checkIncludeWOPercCoPay_CheckedChanged);
			// 
			// butSplitProcs
			// 
			this.butSplitProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSplitProcs.Location = new System.Drawing.Point(868, 318);
			this.butSplitProcs.Name = "butSplitProcs";
			this.butSplitProcs.Size = new System.Drawing.Size(97, 25);
			this.butSplitProcs.TabIndex = 210;
			this.butSplitProcs.Text = "Split Claim";
			this.butSplitProcs.Click += new System.EventHandler(this.butSplitProcs_Click);
			// 
			// FormEtrans835ClaimPay
			// 
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkIncludeWOPercCoPay);
			this.Controls.Add(this.butSplitProcs);
			this.Controls.Add(this.textEobInsPayAmt);
			this.Controls.Add(this.textEobDedApplied);
			this.Controls.Add(this.textEobInsPayAllowed);
			this.Controls.Add(this.textEobClaimFee);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textInsPayAmt);
			this.Controls.Add(this.textDedApplied);
			this.Controls.Add(this.butViewEobDetails);
			this.Controls.Add(this.gridClaimAdjustments);
			this.Controls.Add(this.gridProcedureBreakdown);
			this.Controls.Add(this.gridPayments);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butWriteOff);
			this.Controls.Add(this.butDeductible);
			this.Controls.Add(this.textWriteOff);
			this.Controls.Add(this.textInsPayAllowed);
			this.Controls.Add(this.textClaimFee);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEtrans835ClaimPay";
			this.ShowInTaskbar = false;
			this.Text = " Verify and Enter Payment";
			this.Load += new System.EventHandler(this.FormEtrans835ClaimPay_Load);
			this.Shown += new System.EventHandler(this.FormEtrans835ClaimPay_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.ValidDouble textWriteOff;
		private System.Windows.Forms.TextBox textInsPayAllowed;
		private System.Windows.Forms.TextBox textClaimFee;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDeductible;
		private OpenDental.UI.Button butWriteOff;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.GridOD gridPayments;
		private OpenDental.UI.GridOD gridClaimAdjustments;
		private OpenDental.UI.GridOD gridProcedureBreakdown;
		private UI.Button butViewEobDetails;
		private TextBox textDedApplied;
		private TextBox textInsPayAmt;
		private TextBox textEobInsPayAmt;
		private TextBox textEobDedApplied;
		private TextBox textEobInsPayAllowed;
		private TextBox textEobClaimFee;
		private Label label5;
		private CheckBox checkIncludeWOPercCoPay;
		private UI.Button butSplitProcs;
	}
}
