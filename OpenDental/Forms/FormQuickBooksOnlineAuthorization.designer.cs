namespace OpenDental{
	partial class FormQuickBooksOnlineAuthorization {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQuickBooksOnlineAuthorization));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textAuthCode = new System.Windows.Forms.TextBox();
			this.labelAuthCode = new System.Windows.Forms.Label();
			this.labelRealmId = new System.Windows.Forms.Label();
			this.textRealmId = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(285, 155);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(366, 155);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textAuthCode
			// 
			this.textAuthCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textAuthCode.Location = new System.Drawing.Point(32, 40);
			this.textAuthCode.MaxLength = 99999;
			this.textAuthCode.Name = "textAuthCode";
			this.textAuthCode.Size = new System.Drawing.Size(385, 20);
			this.textAuthCode.TabIndex = 0;
			// 
			// labelAuthCode
			// 
			this.labelAuthCode.Location = new System.Drawing.Point(32, 2);
			this.labelAuthCode.Name = "labelAuthCode";
			this.labelAuthCode.Size = new System.Drawing.Size(385, 36);
			this.labelAuthCode.TabIndex = 130;
			this.labelAuthCode.Text = "Please enter the Authorization Code from your browser.";
			this.labelAuthCode.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelRealmId
			// 
			this.labelRealmId.Location = new System.Drawing.Point(32, 61);
			this.labelRealmId.Name = "labelRealmId";
			this.labelRealmId.Size = new System.Drawing.Size(385, 36);
			this.labelRealmId.TabIndex = 131;
			this.labelRealmId.Text = "Please enter the Realm Id from your browser.";
			this.labelRealmId.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textRealmId
			// 
			this.textRealmId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRealmId.Location = new System.Drawing.Point(32, 99);
			this.textRealmId.MaxLength = 99999;
			this.textRealmId.Name = "textRealmId";
			this.textRealmId.Size = new System.Drawing.Size(385, 20);
			this.textRealmId.TabIndex = 1;
			// 
			// FormQuickBooksOnlineAuthorization
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(453, 191);
			this.Controls.Add(this.textRealmId);
			this.Controls.Add(this.labelRealmId);
			this.Controls.Add(this.labelAuthCode);
			this.Controls.Add(this.textAuthCode);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQuickBooksOnlineAuthorization";
			this.Text = "QuickBooks Online Authorization";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textAuthCode;
		private System.Windows.Forms.Label labelAuthCode;
		private System.Windows.Forms.Label labelRealmId;
		private System.Windows.Forms.TextBox textRealmId;
	}
}