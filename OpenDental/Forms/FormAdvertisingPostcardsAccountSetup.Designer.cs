namespace OpenDental{
	partial class FormAdvertisingPostcardsAccountSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdvertisingPostcardsAccountSetup));
			this.butAdd = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelFName = new System.Windows.Forms.Label();
			this.textLastName = new System.Windows.Forms.TextBox();
			this.textFirstName = new System.Windows.Forms.TextBox();
			this.labelLName = new System.Windows.Forms.Label();
			this.textPhone = new System.Windows.Forms.TextBox();
			this.labelPhone = new System.Windows.Forms.Label();
			this.textMobile = new System.Windows.Forms.TextBox();
			this.labelMobile = new System.Windows.Forms.Label();
			this.labelAccountTitle = new System.Windows.Forms.Label();
			this.textAccountTitle = new System.Windows.Forms.TextBox();
			this.butViewAccount = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.labelRequiredFields = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Location = new System.Drawing.Point(269, 205);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 3;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(350, 205);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(15, 41);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(105, 15);
			this.labelEmail.TabIndex = 19;
			this.labelEmail.Text = "Email*";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(126, 38);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(299, 20);
			this.textEmail.TabIndex = 20;
			// 
			// labelFName
			// 
			this.labelFName.Location = new System.Drawing.Point(15, 67);
			this.labelFName.Name = "labelFName";
			this.labelFName.Size = new System.Drawing.Size(105, 15);
			this.labelFName.TabIndex = 23;
			this.labelFName.Text = "First Name*";
			this.labelFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLastName
			// 
			this.textLastName.Location = new System.Drawing.Point(126, 90);
			this.textLastName.Name = "textLastName";
			this.textLastName.Size = new System.Drawing.Size(299, 20);
			this.textLastName.TabIndex = 26;
			// 
			// textFirstName
			// 
			this.textFirstName.Location = new System.Drawing.Point(126, 64);
			this.textFirstName.Name = "textFirstName";
			this.textFirstName.Size = new System.Drawing.Size(299, 20);
			this.textFirstName.TabIndex = 24;
			// 
			// labelLName
			// 
			this.labelLName.Location = new System.Drawing.Point(15, 93);
			this.labelLName.Name = "labelLName";
			this.labelLName.Size = new System.Drawing.Size(105, 15);
			this.labelLName.TabIndex = 25;
			this.labelLName.Text = "Last Name*";
			this.labelLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(126, 116);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(299, 20);
			this.textPhone.TabIndex = 30;
			// 
			// labelPhone
			// 
			this.labelPhone.Location = new System.Drawing.Point(15, 119);
			this.labelPhone.Name = "labelPhone";
			this.labelPhone.Size = new System.Drawing.Size(105, 15);
			this.labelPhone.TabIndex = 29;
			this.labelPhone.Text = "Phone ";
			this.labelPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMobile
			// 
			this.textMobile.Location = new System.Drawing.Point(126, 142);
			this.textMobile.Name = "textMobile";
			this.textMobile.Size = new System.Drawing.Size(299, 20);
			this.textMobile.TabIndex = 32;
			// 
			// labelMobile
			// 
			this.labelMobile.Location = new System.Drawing.Point(15, 145);
			this.labelMobile.Name = "labelMobile";
			this.labelMobile.Size = new System.Drawing.Size(105, 15);
			this.labelMobile.TabIndex = 31;
			this.labelMobile.Text = "Mobile ";
			this.labelMobile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAccountTitle
			// 
			this.labelAccountTitle.Location = new System.Drawing.Point(15, 15);
			this.labelAccountTitle.Name = "labelAccountTitle";
			this.labelAccountTitle.Size = new System.Drawing.Size(105, 15);
			this.labelAccountTitle.TabIndex = 33;
			this.labelAccountTitle.Text = "Account Title*";
			this.labelAccountTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAccountTitle
			// 
			this.textAccountTitle.Location = new System.Drawing.Point(126, 12);
			this.textAccountTitle.Name = "textAccountTitle";
			this.textAccountTitle.Size = new System.Drawing.Size(299, 20);
			this.textAccountTitle.TabIndex = 34;
			// 
			// butViewAccount
			// 
			this.butViewAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butViewAccount.Location = new System.Drawing.Point(18, 205);
			this.butViewAccount.Name = "butViewAccount";
			this.butViewAccount.Size = new System.Drawing.Size(122, 24);
			this.butViewAccount.TabIndex = 35;
			this.butViewAccount.Text = "View Account Details";
			this.butViewAccount.Visible = false;
			this.butViewAccount.Click += new System.EventHandler(this.butViewAccount_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(123, 165);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 15);
			this.label1.TabIndex = 36;
			this.label1.Text = "* Indicates required field";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRequiredFields
			// 
			this.labelRequiredFields.ForeColor = System.Drawing.Color.Red;
			this.labelRequiredFields.Location = new System.Drawing.Point(15, 169);
			this.labelRequiredFields.Name = "labelRequiredFields";
			this.labelRequiredFields.Size = new System.Drawing.Size(105, 15);
			this.labelRequiredFields.TabIndex = 36;
			this.labelRequiredFields.Text = "Required*";
			this.labelRequiredFields.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormAdvertisingPostcardsAccountSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(437, 241);
			this.Controls.Add(this.labelRequiredFields);
			this.Controls.Add(this.butViewAccount);
			this.Controls.Add(this.labelAccountTitle);
			this.Controls.Add(this.textAccountTitle);
			this.Controls.Add(this.textMobile);
			this.Controls.Add(this.labelMobile);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.labelPhone);
			this.Controls.Add(this.labelEmail);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.labelFName);
			this.Controls.Add(this.textLastName);
			this.Controls.Add(this.textFirstName);
			this.Controls.Add(this.labelLName);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdvertisingPostcardsAccountSetup";
			this.Text = "Advertising - Postcards Add Account";
			this.Load += new System.EventHandler(this.FormMassPostcardSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label labelFName;
		private System.Windows.Forms.TextBox textLastName;
		private System.Windows.Forms.TextBox textFirstName;
		private System.Windows.Forms.Label labelLName;
		private System.Windows.Forms.TextBox textPhone;
		private System.Windows.Forms.Label labelPhone;
		private System.Windows.Forms.TextBox textMobile;
		private System.Windows.Forms.Label labelMobile;
		private System.Windows.Forms.Label labelAccountTitle;
		private System.Windows.Forms.TextBox textAccountTitle;
		private UI.Button butViewAccount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelRequiredFields;
	}
}