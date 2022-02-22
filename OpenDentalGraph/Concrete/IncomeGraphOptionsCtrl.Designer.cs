namespace OpenDentalGraph {
	partial class IncomeGraphOptionsCtrl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkIncludeInsuranceClaimPayments = new System.Windows.Forms.CheckBox();
			this.checkIncludePaySplits = new System.Windows.Forms.CheckBox();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkIncludeInsuranceClaimPayments);
			this.groupBox2.Controls.Add(this.checkIncludePaySplits);
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(214, 56);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Included Income Sources";
			// 
			// checkIncludeInsuranceClaimPayments
			// 
			this.checkIncludeInsuranceClaimPayments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeInsuranceClaimPayments.Checked = true;
			this.checkIncludeInsuranceClaimPayments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIncludeInsuranceClaimPayments.Location = new System.Drawing.Point(6, 35);
			this.checkIncludeInsuranceClaimPayments.Name = "checkIncludeInsuranceClaimPayments";
			this.checkIncludeInsuranceClaimPayments.Size = new System.Drawing.Size(199, 18);
			this.checkIncludeInsuranceClaimPayments.TabIndex = 3;
			this.checkIncludeInsuranceClaimPayments.Text = "Insurance Claim Payments";
			this.checkIncludeInsuranceClaimPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeInsuranceClaimPayments.UseVisualStyleBackColor = true;
			this.checkIncludeInsuranceClaimPayments.Click += new System.EventHandler(this.OnIncomeGraphInputsChanged);
			// 
			// checkIncludePaySplits
			// 
			this.checkIncludePaySplits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludePaySplits.Checked = true;
			this.checkIncludePaySplits.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIncludePaySplits.Location = new System.Drawing.Point(6, 14);
			this.checkIncludePaySplits.Name = "checkIncludePaySplits";
			this.checkIncludePaySplits.Size = new System.Drawing.Size(199, 20);
			this.checkIncludePaySplits.TabIndex = 5;
			this.checkIncludePaySplits.Text = "Pay Splits";
			this.checkIncludePaySplits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludePaySplits.UseVisualStyleBackColor = true;
			this.checkIncludePaySplits.Click += new System.EventHandler(this.OnIncomeGraphInputsChanged);
			// 
			// IncomeGraphOptionsCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Name = "IncomeGraphOptionsCtrl";
			this.Size = new System.Drawing.Size(226, 67);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkIncludeInsuranceClaimPayments;
		private System.Windows.Forms.CheckBox checkIncludePaySplits;
	}
}
