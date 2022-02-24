namespace OpenDental{
	partial class FormWebFormSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebFormSetup));
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupConstructURL = new System.Windows.Forms.GroupBox();
			this.butCopyToClipboard = new OpenDental.UI.Button();
			this.butNavigateTo = new OpenDental.UI.Button();
			this.checkDisableTypedSig = new System.Windows.Forms.CheckBox();
			this.checkAutoFillNameAndBirthdate = new System.Windows.Forms.CheckBox();
			this.textURLs = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butNextForms = new OpenDental.UI.Button();
			this.textNextForms = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textRedirectURL = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.butChange = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butWebformBorderColor = new System.Windows.Forms.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.labelBorderColor = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textboxWebHostAddress = new System.Windows.Forms.TextBox();
			this.labelWebhostURL = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butUpdate = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			this.checkDisableWebFormSignatures = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupConstructURL.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupConstructURL
			// 
			this.groupConstructURL.Controls.Add(this.butCopyToClipboard);
			this.groupConstructURL.Controls.Add(this.butNavigateTo);
			this.groupConstructURL.Controls.Add(this.checkDisableTypedSig);
			this.groupConstructURL.Controls.Add(this.checkAutoFillNameAndBirthdate);
			this.groupConstructURL.Controls.Add(this.textURLs);
			this.groupConstructURL.Controls.Add(this.label2);
			this.groupConstructURL.Controls.Add(this.comboClinic);
			this.groupConstructURL.Controls.Add(this.butNextForms);
			this.groupConstructURL.Controls.Add(this.textNextForms);
			this.groupConstructURL.Controls.Add(this.label1);
			this.groupConstructURL.Controls.Add(this.textRedirectURL);
			this.groupConstructURL.Controls.Add(this.label3);
			this.groupConstructURL.Location = new System.Drawing.Point(308, 135);
			this.groupConstructURL.Name = "groupConstructURL";
			this.groupConstructURL.Size = new System.Drawing.Size(615, 334);
			this.groupConstructURL.TabIndex = 75;
			this.groupConstructURL.TabStop = false;
			this.groupConstructURL.Text = "Construct URL";
			// 
			// butCopyToClipboard
			// 
			this.butCopyToClipboard.Location = new System.Drawing.Point(212, 304);
			this.butCopyToClipboard.Name = "butCopyToClipboard";
			this.butCopyToClipboard.Size = new System.Drawing.Size(115, 24);
			this.butCopyToClipboard.TabIndex = 78;
			this.butCopyToClipboard.Text = "Copy to Clipboard";
			this.butCopyToClipboard.Click += new System.EventHandler(this.butCopyToClipboard_Click);
			// 
			// butNavigateTo
			// 
			this.butNavigateTo.Location = new System.Drawing.Point(89, 304);
			this.butNavigateTo.Name = "butNavigateTo";
			this.butNavigateTo.Size = new System.Drawing.Size(117, 24);
			this.butNavigateTo.TabIndex = 79;
			this.butNavigateTo.Text = "Navigate to URL(s)";
			this.butNavigateTo.Click += new System.EventHandler(this.butNavigateTo_Click);
			// 
			// checkDisableTypedSig
			// 
			this.checkDisableTypedSig.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDisableTypedSig.Location = new System.Drawing.Point(89, 137);
			this.checkDisableTypedSig.Name = "checkDisableTypedSig";
			this.checkDisableTypedSig.Size = new System.Drawing.Size(444, 18);
			this.checkDisableTypedSig.TabIndex = 44;
			this.checkDisableTypedSig.Text = "Disable Typed Signature";
			this.checkDisableTypedSig.UseVisualStyleBackColor = true;
			this.checkDisableTypedSig.CheckedChanged += new System.EventHandler(this.checkDisableTypedSig_CheckChanged);
			// 
			// checkAutoFillNameAndBirthdate
			// 
			this.checkAutoFillNameAndBirthdate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAutoFillNameAndBirthdate.Location = new System.Drawing.Point(89, 113);
			this.checkAutoFillNameAndBirthdate.Name = "checkAutoFillNameAndBirthdate";
			this.checkAutoFillNameAndBirthdate.Size = new System.Drawing.Size(444, 18);
			this.checkAutoFillNameAndBirthdate.TabIndex = 43;
			this.checkAutoFillNameAndBirthdate.Text = "Inherit (Auto Fill) Name and Birthdate from Previous Form";
			this.checkAutoFillNameAndBirthdate.UseVisualStyleBackColor = true;
			this.checkAutoFillNameAndBirthdate.CheckedChanged += new System.EventHandler(this.checkAutoFillNameAndBirthdate_CheckedChanged);
			// 
			// textURLs
			// 
			this.textURLs.Location = new System.Drawing.Point(89, 161);
			this.textURLs.MaxLength = 100000;
			this.textURLs.Multiline = true;
			this.textURLs.Name = "textURLs";
			this.textURLs.ReadOnly = true;
			this.textURLs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textURLs.Size = new System.Drawing.Size(444, 137);
			this.textURLs.TabIndex = 42;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 162);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 17);
			this.label2.TabIndex = 41;
			this.label2.Text = "URLs";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(52, 87);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(217, 21);
			this.comboClinic.TabIndex = 38;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// butNextForms
			// 
			this.butNextForms.Location = new System.Drawing.Point(539, 60);
			this.butNextForms.Name = "butNextForms";
			this.butNextForms.Size = new System.Drawing.Size(23, 20);
			this.butNextForms.TabIndex = 19;
			this.butNextForms.Text = "...";
			this.butNextForms.Click += new System.EventHandler(this.butNextForms_Click);
			// 
			// textNextForms
			// 
			this.textNextForms.Location = new System.Drawing.Point(89, 60);
			this.textNextForms.MaxLength = 100;
			this.textNextForms.Name = "textNextForms";
			this.textNextForms.ReadOnly = true;
			this.textNextForms.Size = new System.Drawing.Size(444, 20);
			this.textNextForms.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 14);
			this.label1.TabIndex = 5;
			this.label1.Text = "Next Forms";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRedirectURL
			// 
			this.textRedirectURL.Location = new System.Drawing.Point(89, 33);
			this.textRedirectURL.MaxLength = 100;
			this.textRedirectURL.Name = "textRedirectURL";
			this.textRedirectURL.Size = new System.Drawing.Size(444, 20);
			this.textRedirectURL.TabIndex = 1;
			this.textRedirectURL.TextChanged += new System.EventHandler(this.textRedirectURL_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 14);
			this.label3.TabIndex = 2;
			this.label3.Text = "Redirect URL";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSave
			// 
			this.butSave.Location = new System.Drawing.Point(511, 68);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 74;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(172, 45);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 72;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(218, 288);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 58;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butWebformBorderColor
			// 
			this.butWebformBorderColor.BackColor = System.Drawing.Color.RoyalBlue;
			this.butWebformBorderColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butWebformBorderColor.Location = new System.Drawing.Point(141, 45);
			this.butWebformBorderColor.Name = "butWebformBorderColor";
			this.butWebformBorderColor.Size = new System.Drawing.Size(24, 24);
			this.butWebformBorderColor.TabIndex = 71;
			this.butWebformBorderColor.UseVisualStyleBackColor = false;
			this.butWebformBorderColor.Click += new System.EventHandler(this.butWebformBorderColor_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(218, 203);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 57;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// labelBorderColor
			// 
			this.labelBorderColor.Location = new System.Drawing.Point(24, 45);
			this.labelBorderColor.Name = "labelBorderColor";
			this.labelBorderColor.Size = new System.Drawing.Size(111, 19);
			this.labelBorderColor.TabIndex = 48;
			this.labelBorderColor.Text = "Border Color";
			this.labelBorderColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.HasLinkDetect = false;
			this.gridMain.Location = new System.Drawing.Point(12, 29);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(200, 454);
			this.gridMain.TabIndex = 56;
			this.gridMain.Title = "Available Web Forms";
			this.gridMain.TranslationName = "TableSheetDefs";
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// textboxWebHostAddress
			// 
			this.textboxWebHostAddress.Location = new System.Drawing.Point(141, 19);
			this.textboxWebHostAddress.Name = "textboxWebHostAddress";
			this.textboxWebHostAddress.Size = new System.Drawing.Size(445, 20);
			this.textboxWebHostAddress.TabIndex = 45;
			this.textboxWebHostAddress.TextChanged += new System.EventHandler(this.textboxWebHostAddress_TextChanged);
			// 
			// labelWebhostURL
			// 
			this.labelWebhostURL.Location = new System.Drawing.Point(6, 19);
			this.labelWebhostURL.Name = "labelWebhostURL";
			this.labelWebhostURL.Size = new System.Drawing.Size(131, 19);
			this.labelWebhostURL.TabIndex = 46;
			this.labelWebhostURL.Text = "Host Server Address";
			this.labelWebhostURL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(847, 480);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butUpdate
			// 
			this.butUpdate.Location = new System.Drawing.Point(218, 246);
			this.butUpdate.Name = "butUpdate";
			this.butUpdate.Size = new System.Drawing.Size(75, 24);
			this.butUpdate.TabIndex = 76;
			this.butUpdate.Text = "Update";
			this.butUpdate.Click += new System.EventHandler(this.butUpdate_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(766, 480);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 77;
			this.butOk.Text = "&OK";
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// checkDisableWebFormSignatures
			// 
			this.checkDisableWebFormSignatures.AutoSize = true;
			this.checkDisableWebFormSignatures.Location = new System.Drawing.Point(141, 75);
			this.checkDisableWebFormSignatures.Name = "checkDisableWebFormSignatures";
			this.checkDisableWebFormSignatures.Size = new System.Drawing.Size(166, 17);
			this.checkDisableWebFormSignatures.TabIndex = 78;
			this.checkDisableWebFormSignatures.Text = "Disable Web Form Signatures";
			this.checkDisableWebFormSignatures.UseVisualStyleBackColor = true;
			this.checkDisableWebFormSignatures.CheckedChanged += new System.EventHandler(this.checkDisableWebFormSignatures_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textboxWebHostAddress);
			this.groupBox1.Controls.Add(this.butSave);
			this.groupBox1.Controls.Add(this.labelBorderColor);
			this.groupBox1.Controls.Add(this.checkDisableWebFormSignatures);
			this.groupBox1.Controls.Add(this.butWebformBorderColor);
			this.groupBox1.Controls.Add(this.butChange);
			this.groupBox1.Controls.Add(this.labelWebhostURL);
			this.groupBox1.Location = new System.Drawing.Point(308, 29);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(615, 100);
			this.groupBox1.TabIndex = 79;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preferences";
			// 
			// FormWebFormSetup
			// 
			this.ClientSize = new System.Drawing.Size(934, 514);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butUpdate);
			this.Controls.Add(this.groupConstructURL);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebFormSetup";
			this.Text = "Web Form Setup";
			this.Load += new System.EventHandler(this.FormWebFormSetup_Load);
			this.Shown += new System.EventHandler(this.FormWebFormSetup_Shown);
			this.groupConstructURL.ResumeLayout(false);
			this.groupConstructURL.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textboxWebHostAddress;
		private System.Windows.Forms.Label labelWebhostURL;
		private System.Windows.Forms.Label labelBorderColor;
		private System.Windows.Forms.Button butWebformBorderColor;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private OpenDental.UI.Button butChange;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private UI.Button butSave;
		private System.Windows.Forms.GroupBox groupConstructURL;
		private System.Windows.Forms.TextBox textRedirectURL;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textNextForms;
		private System.Windows.Forms.Label label1;
		private UI.Button butNextForms;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textURLs;
		private UI.Button butUpdate;
		private System.Windows.Forms.CheckBox checkAutoFillNameAndBirthdate;
		private UI.Button butOk;
		private System.Windows.Forms.CheckBox checkDisableTypedSig;
		private UI.Button butCopyToClipboard;
		private UI.Button butNavigateTo;
		private System.Windows.Forms.CheckBox checkDisableWebFormSignatures;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}