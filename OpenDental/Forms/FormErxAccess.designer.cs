namespace OpenDental{
	partial class FormErxAccess {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormErxAccess));
			this.label1 = new System.Windows.Forms.Label();
			this.gridProviders = new OpenDental.UI.GridOD();
			this.butNotIdpd = new OpenDental.UI.Button();
			this.butIdpd = new OpenDental.UI.Button();
			this.butEnable = new OpenDental.UI.Button();
			this.butDisable = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butNotEPCS = new OpenDental.UI.Button();
			this.butEPCS = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(446, 118);
			this.label1.TabIndex = 9;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridProviders
			// 
			this.gridProviders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProviders.Location = new System.Drawing.Point(12, 257);
			this.gridProviders.Name = "gridProviders";
			this.gridProviders.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProviders.Size = new System.Drawing.Size(446, 242);
			this.gridProviders.TabIndex = 4;
			this.gridProviders.Title = "Providers";
			this.gridProviders.TranslationName = "TableProviders";
			// 
			// butNotIdpd
			// 
			this.butNotIdpd.Location = new System.Drawing.Point(198, 227);
			this.butNotIdpd.Name = "butNotIdpd";
			this.butNotIdpd.Size = new System.Drawing.Size(75, 24);
			this.butNotIdpd.TabIndex = 8;
			this.butNotIdpd.Text = "Not IDP\'d";
			this.butNotIdpd.Click += new System.EventHandler(this.butNotIdpd_Click);
			// 
			// butIdpd
			// 
			this.butIdpd.Location = new System.Drawing.Point(198, 202);
			this.butIdpd.Name = "butIdpd";
			this.butIdpd.Size = new System.Drawing.Size(75, 24);
			this.butIdpd.TabIndex = 7;
			this.butIdpd.Text = "IDP\'d";
			this.butIdpd.Click += new System.EventHandler(this.butIdpd_Click);
			// 
			// butEnable
			// 
			this.butEnable.Location = new System.Drawing.Point(12, 202);
			this.butEnable.Name = "butEnable";
			this.butEnable.Size = new System.Drawing.Size(75, 24);
			this.butEnable.TabIndex = 6;
			this.butEnable.Text = "Enable";
			this.butEnable.Click += new System.EventHandler(this.butEnable_Click);
			// 
			// butDisable
			// 
			this.butDisable.Location = new System.Drawing.Point(12, 227);
			this.butDisable.Name = "butDisable";
			this.butDisable.Size = new System.Drawing.Size(75, 24);
			this.butDisable.TabIndex = 5;
			this.butDisable.Text = "Disable";
			this.butDisable.Click += new System.EventHandler(this.butDisable_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(302, 505);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(383, 505);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butNotEPCS
			// 
			this.butNotEPCS.Location = new System.Drawing.Point(383, 227);
			this.butNotEPCS.Name = "butNotEPCS";
			this.butNotEPCS.Size = new System.Drawing.Size(75, 24);
			this.butNotEPCS.TabIndex = 11;
			this.butNotEPCS.Text = "Not EPCS";
			this.butNotEPCS.Click += new System.EventHandler(this.butNotEPCS_Click);
			// 
			// butEPCS
			// 
			this.butEPCS.Location = new System.Drawing.Point(383, 202);
			this.butEPCS.Name = "butEPCS";
			this.butEPCS.Size = new System.Drawing.Size(75, 24);
			this.butEPCS.TabIndex = 10;
			this.butEPCS.Text = "EPCS";
			this.butEPCS.Click += new System.EventHandler(this.butEPCS_Click);
			// 
			// FormErxAccess
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(470, 541);
			this.Controls.Add(this.butNotEPCS);
			this.Controls.Add(this.butEPCS);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butNotIdpd);
			this.Controls.Add(this.butIdpd);
			this.Controls.Add(this.butEnable);
			this.Controls.Add(this.butDisable);
			this.Controls.Add(this.gridProviders);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormErxAccess";
			this.Text = "Edit Erx Access";
			this.Load += new System.EventHandler(this.FormErxAccess_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridProviders;
		private UI.Button butDisable;
		private UI.Button butEnable;
		private UI.Button butIdpd;
		private UI.Button butNotIdpd;
		private System.Windows.Forms.Label label1;
		private UI.Button butNotEPCS;
		private UI.Button butEPCS;
	}
}