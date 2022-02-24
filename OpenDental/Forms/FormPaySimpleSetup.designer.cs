namespace OpenDental{
	partial class FormPaySimpleSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPaySimpleSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.groupPaySettings = new System.Windows.Forms.GroupBox();
			this.checkPreventSavingNewCC = new System.Windows.Forms.CheckBox();
			this.textKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboPaymentTypeCC = new UI.ComboBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPaymentTypeACH = new UI.ComboBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.groupPaySettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(222, 306);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(307, 306);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(12, 37);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(226, 18);
			this.checkEnabled.TabIndex = 4;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(28, 38);
			this.linkLabel1.Location = new System.Drawing.Point(4, 9);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(389, 16);
			this.linkLabel1.TabIndex = 5;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The PaySimple website is at www.paysimple.com/partner/open-dental";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Location = new System.Drawing.Point(70, 58);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(218, 40);
			this.labelClinicEnable.TabIndex = 6;
			this.labelClinicEnable.Text = "To enable PaySimple for a clinic, set the Username and Key for that clinic.";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(163, 118);
			this.comboClinic.MaxDropDownItems = 30;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(175, 21);
			this.comboClinic.TabIndex = 8;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(43, 120);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(119, 16);
			this.labelClinic.TabIndex = 7;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPaySettings
			// 
			this.groupPaySettings.Controls.Add(this.comboPaymentTypeACH);
			this.groupPaySettings.Controls.Add(this.label4);
			this.groupPaySettings.Controls.Add(this.checkPreventSavingNewCC);
			this.groupPaySettings.Controls.Add(this.textKey);
			this.groupPaySettings.Controls.Add(this.label3);
			this.groupPaySettings.Controls.Add(this.textUsername);
			this.groupPaySettings.Controls.Add(this.label2);
			this.groupPaySettings.Controls.Add(this.comboPaymentTypeCC);
			this.groupPaySettings.Controls.Add(this.label1);
			this.groupPaySettings.Location = new System.Drawing.Point(12, 145);
			this.groupPaySettings.Name = "groupPaySettings";
			this.groupPaySettings.Size = new System.Drawing.Size(370, 155);
			this.groupPaySettings.TabIndex = 9;
			this.groupPaySettings.TabStop = false;
			this.groupPaySettings.Text = "Clinic Payment Settings";
			// 
			// checkPreventSavingNewCC
			// 
			this.checkPreventSavingNewCC.Location = new System.Drawing.Point(151, 124);
			this.checkPreventSavingNewCC.Name = "checkPreventSavingNewCC";
			this.checkPreventSavingNewCC.Size = new System.Drawing.Size(217, 18);
			this.checkPreventSavingNewCC.TabIndex = 5;
			this.checkPreventSavingNewCC.Text = "Prevent saving new cards";
			this.checkPreventSavingNewCC.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkPreventSavingNewCC.UseVisualStyleBackColor = true;
			// 
			// textKey
			// 
			this.textKey.Location = new System.Drawing.Point(151, 98);
			this.textKey.Name = "textKey";
			this.textKey.Size = new System.Drawing.Size(175, 20);
			this.textKey.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Key";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(151, 73);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(175, 20);
			this.textUsername.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(110, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Username";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentTypeCC
			// 
			this.comboPaymentTypeCC.Location = new System.Drawing.Point(151, 19);
			this.comboPaymentTypeCC.Name = "comboPaymentTypeCC";
			this.comboPaymentTypeCC.Size = new System.Drawing.Size(175, 21);
			this.comboPaymentTypeCC.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(147, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Payment Type Credit Card";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentTypeACH
			// 
			this.comboPaymentTypeACH.Location = new System.Drawing.Point(151, 46);
			this.comboPaymentTypeACH.Name = "comboPaymentTypeACH";
			this.comboPaymentTypeACH.Size = new System.Drawing.Size(175, 21);
			this.comboPaymentTypeACH.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 49);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Payment Type ACH";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormPaySimpleSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(394, 342);
			this.Controls.Add(this.groupPaySettings);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.labelClinicEnable);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPaySimpleSetup";
			this.Text = "PaySimple Setup";
			this.Load += new System.EventHandler(this.FormPaySimpleSetup_Load);
			this.groupPaySettings.ResumeLayout(false);
			this.groupPaySettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label labelClinicEnable;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.GroupBox groupPaySettings;
		private System.Windows.Forms.TextBox textKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboPaymentTypeCC;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkPreventSavingNewCC;
		private UI.ComboBoxOD comboPaymentTypeACH;
		private System.Windows.Forms.Label label4;
	}
}