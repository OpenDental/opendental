namespace OpenDental{
	partial class FormOrthoPat {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoPat));
			this.label6 = new System.Windows.Forms.Label();
			this.textSubID = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelNextClaim = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.butSave = new OpenDental.UI.Button();
			this.textFee = new OpenDental.ValidDouble();
			this.checkUseDefaultFee = new OpenDental.UI.CheckBox();
			this.textDateNextClaim = new OpenDental.ValidDate();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 76);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(108, 20);
			this.label6.TabIndex = 19;
			this.label6.Text = "Sub ID";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubID
			// 
			this.textSubID.Location = new System.Drawing.Point(112, 76);
			this.textSubID.Name = "textSubID";
			this.textSubID.ReadOnly = true;
			this.textSubID.Size = new System.Drawing.Size(229, 20);
			this.textSubID.TabIndex = 18;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(108, 20);
			this.label5.TabIndex = 17;
			this.label5.Text = "Carrier";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(112, 50);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.ReadOnly = true;
			this.textCarrier.Size = new System.Drawing.Size(229, 20);
			this.textCarrier.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 20);
			this.label1.TabIndex = 13;
			this.label1.Text = "Patient";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNextClaim
			// 
			this.labelNextClaim.Location = new System.Drawing.Point(3, 128);
			this.labelNextClaim.Name = "labelNextClaim";
			this.labelNextClaim.Size = new System.Drawing.Size(108, 20);
			this.labelNextClaim.TabIndex = 12;
			this.labelNextClaim.Text = "Next Claim Date";
			this.labelNextClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 102);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 20);
			this.label2.TabIndex = 8;
			this.label2.Text = "Fee";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(112, 24);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(229, 20);
			this.textPatient.TabIndex = 5;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(278, 164);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 4;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textFee
			// 
			this.textFee.Location = new System.Drawing.Point(112, 102);
			this.textFee.Name = "textFee";
			this.textFee.ReadOnly = true;
			this.textFee.Size = new System.Drawing.Size(73, 20);
			this.textFee.TabIndex = 20;
			// 
			// checkUseDefaultFee
			// 
			this.checkUseDefaultFee.Checked = true;
			this.checkUseDefaultFee.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkUseDefaultFee.Location = new System.Drawing.Point(187, 104);
			this.checkUseDefaultFee.Name = "checkUseDefaultFee";
			this.checkUseDefaultFee.Size = new System.Drawing.Size(152, 18);
			this.checkUseDefaultFee.TabIndex = 21;
			this.checkUseDefaultFee.Text = "Use Default Fee";
			this.checkUseDefaultFee.CheckedChanged += new System.EventHandler(this.checkUseDefaultFee_CheckedChanged);
			// 
			// textDateNextClaim
			// 
			this.textDateNextClaim.Location = new System.Drawing.Point(112, 128);
			this.textDateNextClaim.Name = "textDateNextClaim";
			this.textDateNextClaim.Size = new System.Drawing.Size(103, 20);
			this.textDateNextClaim.TabIndex = 22;
			this.textDateNextClaim.Validated += new System.EventHandler(this.textDateNextClaim_Validated);
			// 
			// FormOrthoPat
			// 
			this.ClientSize = new System.Drawing.Size(365, 200);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.textDateNextClaim);
			this.Controls.Add(this.checkUseDefaultFee);
			this.Controls.Add(this.textFee);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textSubID);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelNextClaim);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoPat";
			this.Text = "Ortho Patient Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butSave;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelNextClaim;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textCarrier;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textSubID;
		private ValidDouble textFee;
		private OpenDental.UI.CheckBox checkUseDefaultFee;
		private ValidDate textDateNextClaim;
	}
}