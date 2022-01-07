namespace OpenDental{
	partial class FormFHIRAPIKeyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFHIRAPIKeyEdit));
			this.butClose = new OpenDental.UI.Button();
			this.textKey = new System.Windows.Forms.TextBox();
			this.textPhone = new OpenDental.ValidPhone();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.textName = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.labelMiddleI = new System.Windows.Forms.Label();
			this.labelFName = new System.Windows.Forms.Label();
			this.labelLName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDateDisabled = new System.Windows.Forms.TextBox();
			this.textStatus = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDisable = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(433, 237);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textKey
			// 
			this.textKey.Location = new System.Drawing.Point(193, 39);
			this.textKey.MaxLength = 100;
			this.textKey.Name = "textKey";
			this.textKey.ReadOnly = true;
			this.textKey.Size = new System.Drawing.Size(228, 20);
			this.textKey.TabIndex = 4;
			this.textKey.TabStop = false;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(193, 147);
			this.textPhone.MaxLength = 100;
			this.textPhone.Name = "textPhone";
			this.textPhone.ReadOnly = true;
			this.textPhone.Size = new System.Drawing.Size(106, 20);
			this.textPhone.TabIndex = 11;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(193, 119);
			this.textEmail.MaxLength = 100;
			this.textEmail.Name = "textEmail";
			this.textEmail.ReadOnly = true;
			this.textEmail.Size = new System.Drawing.Size(228, 20);
			this.textEmail.TabIndex = 10;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(193, 91);
			this.textName.MaxLength = 100;
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(228, 20);
			this.textName.TabIndex = 9;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(38, 43);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(154, 14);
			this.label19.TabIndex = 5;
			this.label19.Text = "API Key";
			this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelMiddleI
			// 
			this.labelMiddleI.Location = new System.Drawing.Point(38, 151);
			this.labelMiddleI.Name = "labelMiddleI";
			this.labelMiddleI.Size = new System.Drawing.Size(154, 14);
			this.labelMiddleI.TabIndex = 6;
			this.labelMiddleI.Text = "Developer Phone";
			this.labelMiddleI.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFName
			// 
			this.labelFName.Location = new System.Drawing.Point(38, 123);
			this.labelFName.Name = "labelFName";
			this.labelFName.Size = new System.Drawing.Size(154, 14);
			this.labelFName.TabIndex = 7;
			this.labelFName.Text = "Developer Email";
			this.labelFName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelLName
			// 
			this.labelLName.Location = new System.Drawing.Point(38, 95);
			this.labelLName.Name = "labelLName";
			this.labelLName.Size = new System.Drawing.Size(154, 14);
			this.labelLName.TabIndex = 8;
			this.labelLName.Text = "Developr Name";
			this.labelLName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 177);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 14);
			this.label1.TabIndex = 12;
			this.label1.Text = "Date Disabled";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateDisabled
			// 
			this.textDateDisabled.Location = new System.Drawing.Point(193, 174);
			this.textDateDisabled.MaxLength = 100;
			this.textDateDisabled.Name = "textDateDisabled";
			this.textDateDisabled.ReadOnly = true;
			this.textDateDisabled.Size = new System.Drawing.Size(106, 20);
			this.textDateDisabled.TabIndex = 14;
			this.textDateDisabled.TabStop = false;
			// 
			// textStatus
			// 
			this.textStatus.Location = new System.Drawing.Point(193, 65);
			this.textStatus.MaxLength = 100;
			this.textStatus.Name = "textStatus";
			this.textStatus.ReadOnly = true;
			this.textStatus.Size = new System.Drawing.Size(228, 20);
			this.textStatus.TabIndex = 16;
			this.textStatus.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(38, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 14);
			this.label2.TabIndex = 15;
			this.label2.Text = "Key Status";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butDisable
			// 
			this.butDisable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDisable.Location = new System.Drawing.Point(193, 207);
			this.butDisable.Name = "butDisable";
			this.butDisable.Size = new System.Drawing.Size(85, 24);
			this.butDisable.TabIndex = 17;
			this.butDisable.Text = "&Disable Key";
			this.butDisable.Click += new System.EventHandler(this.ButDisable_Click);
			// 
			// FormFHIRAPIKeyEdit
			// 
			this.ClientSize = new System.Drawing.Size(520, 273);
			this.Controls.Add(this.butDisable);
			this.Controls.Add(this.textStatus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDateDisabled);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textKey);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.labelMiddleI);
			this.Controls.Add(this.labelFName);
			this.Controls.Add(this.labelLName);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFHIRAPIKeyEdit";
			this.Text = "API Key Edit";
			this.Load += new System.EventHandler(this.FormFHIRAPIKeyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textKey;
		private ValidPhone textPhone;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label labelMiddleI;
		private System.Windows.Forms.Label labelFName;
		private System.Windows.Forms.Label labelLName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDateDisabled;
		private System.Windows.Forms.TextBox textStatus;
		private System.Windows.Forms.Label label2;
		private UI.Button butDisable;
	}
}