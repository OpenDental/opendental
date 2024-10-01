namespace OpenDental{
	partial class FormPayPlanChargeSelection {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanChargeSelection));
			this.listBoxPayPlanCharges = new OpenDental.UI.ListBox();
			this.butZeroOutCharges = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listBoxPayPlanCharges
			// 
			this.listBoxPayPlanCharges.Location = new System.Drawing.Point(12, 12);
			this.listBoxPayPlanCharges.Name = "listBoxPayPlanCharges";
			this.listBoxPayPlanCharges.Size = new System.Drawing.Size(264, 342);
			this.listBoxPayPlanCharges.TabIndex = 3;
			this.listBoxPayPlanCharges.Text = "listBox1";
			this.listBoxPayPlanCharges.DoubleClick += new System.EventHandler(this.listBoxPayPlanCharges_DoubleClick);
			// 
			// butZeroOutCharges
			// 
			this.butZeroOutCharges.Location = new System.Drawing.Point(12, 374);
			this.butZeroOutCharges.Name = "butZeroOutCharges";
			this.butZeroOutCharges.Size = new System.Drawing.Size(102, 24);
			this.butZeroOutCharges.TabIndex = 4;
			this.butZeroOutCharges.Text = "Zero out charges";
			this.butZeroOutCharges.UseVisualStyleBackColor = true;
			this.butZeroOutCharges.Click += new System.EventHandler(this.butZeroOutCharges_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(201, 372);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormPayPlanChargeSelection
			// 
			this.ClientSize = new System.Drawing.Size(288, 410);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butZeroOutCharges);
			this.Controls.Add(this.listBoxPayPlanCharges);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayPlanChargeSelection";
			this.Text = "Pay Plan Charge Selection";
			this.Load += new System.EventHandler(this.FormPayPlanChargeSelection_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ListBox listBoxPayPlanCharges;
		private UI.Button butZeroOutCharges;
		private UI.Button butOK;
	}
}