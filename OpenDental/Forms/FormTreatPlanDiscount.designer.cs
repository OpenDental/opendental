namespace OpenDental{
	partial class FormTreatmentPlanDiscount {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTreatmentPlanDiscount));
			this.label1 = new System.Windows.Forms.Label();
			this.textPercentage = new System.Windows.Forms.TextBox();
			this.butSave = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(210, 15);
			this.label1.TabIndex = 106;
			this.label1.Text = "Percent";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPercentage
			// 
			this.textPercentage.Location = new System.Drawing.Point(15, 36);
			this.textPercentage.MaxLength = 255;
			this.textPercentage.Name = "textPercentage";
			this.textPercentage.Size = new System.Drawing.Size(58, 20);
			this.textPercentage.TabIndex = 105;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(226, 111);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(74, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(175, 17);
			this.label2.TabIndex = 107;
			this.label2.Text = "0 will clear discounts";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(289, 33);
			this.label3.TabIndex = 108;
			this.label3.Text = "Clicking Save will apply the discount to selected procedures or all procedures if" +
    " none were selected.";
			// 
			// FormTreatmentPlanDiscount
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(313, 147);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPercentage);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTreatmentPlanDiscount";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Treatment Plan Discount";
			this.Load += new System.EventHandler(this.FormTreatmentPlanDiscount_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPercentage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}