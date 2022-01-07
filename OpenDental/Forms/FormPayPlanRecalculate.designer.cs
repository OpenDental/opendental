namespace OpenDental{
	partial class FormPayPlanRecalculate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanRecalculate));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkRecalculateInterest = new System.Windows.Forms.CheckBox();
			this.radioPrepay = new System.Windows.Forms.RadioButton();
			this.radioPrepayWithoutInterest = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(199, 191);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(292, 191);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkRecalculateInterest
			// 
			this.checkRecalculateInterest.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecalculateInterest.Location = new System.Drawing.Point(18, 143);
			this.checkRecalculateInterest.Name = "checkRecalculateInterest";
			this.checkRecalculateInterest.Size = new System.Drawing.Size(264, 24);
			this.checkRecalculateInterest.TabIndex = 45;
			this.checkRecalculateInterest.Text = "Recalculate Interest";
			this.checkRecalculateInterest.UseVisualStyleBackColor = true;
			// 
			// radioPrepay
			// 
			this.radioPrepay.Location = new System.Drawing.Point(6, 17);
			this.radioPrepay.Name = "radioPrepay";
			this.radioPrepay.Size = new System.Drawing.Size(63, 17);
			this.radioPrepay.TabIndex = 43;
			this.radioPrepay.TabStop = true;
			this.radioPrepay.Text = "Prepay";
			this.radioPrepay.UseVisualStyleBackColor = true;
			// 
			// radioPrepayWithoutInterest
			// 
			this.radioPrepayWithoutInterest.AutoSize = true;
			this.radioPrepayWithoutInterest.Location = new System.Drawing.Point(6, 39);
			this.radioPrepayWithoutInterest.Name = "radioPrepayWithoutInterest";
			this.radioPrepayWithoutInterest.Size = new System.Drawing.Size(100, 17);
			this.radioPrepayWithoutInterest.TabIndex = 44;
			this.radioPrepayWithoutInterest.TabStop = true;
			this.radioPrepayWithoutInterest.Text = "Pay on principal";
			this.radioPrepayWithoutInterest.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioPrepayWithoutInterest);
			this.groupBox1.Controls.Add(this.radioPrepay);
			this.groupBox1.Location = new System.Drawing.Point(12, 73);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(270, 64);
			this.groupBox1.TabIndex = 44;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Payment Allocation Method";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(349, 57);
			this.label1.TabIndex = 46;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// FormPayPlanRecalculate
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(379, 227);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkRecalculateInterest);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayPlanRecalculate";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Payment Plan Recalculate";
			this.Load += new System.EventHandler(this.FormPayPlanRecalculate_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkRecalculateInterest;
		private System.Windows.Forms.RadioButton radioPrepay;
		private System.Windows.Forms.RadioButton radioPrepayWithoutInterest;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
	}
}