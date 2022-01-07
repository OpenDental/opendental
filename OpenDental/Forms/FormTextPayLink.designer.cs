namespace OpenDental{
	partial class FormTextPayLink {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTextPayLink));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textPreview = new OpenDental.ODtextBox();
			this.labelFormattedPreview = new System.Windows.Forms.Label();
			this.checkIncludeInsurance = new System.Windows.Forms.CheckBox();
			this.butPreview = new OpenDental.UI.Button();
			this.grouTemplateOptions = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioAccount = new System.Windows.Forms.RadioButton();
			this.radioAppointment = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBalForFam = new System.Windows.Forms.CheckBox();
			this.labelCheckIns = new System.Windows.Forms.Label();
			this.comboBoxClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.toolBarMain = new OpenDental.UI.ToolBarOD();
			this.grouTemplateOptions.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(261, 385);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(99, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "Prepare to Send";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(180, 385);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textPreview
			// 
			this.textPreview.AcceptsTab = true;
			this.textPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPreview.BackColor = System.Drawing.SystemColors.Control;
			this.textPreview.DetectLinksEnabled = false;
			this.textPreview.DetectUrls = false;
			this.textPreview.Location = new System.Drawing.Point(167, 59);
			this.textPreview.Name = "textPreview";
			this.textPreview.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textPreview.ReadOnly = true;
			this.textPreview.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPreview.Size = new System.Drawing.Size(193, 322);
			this.textPreview.TabIndex = 6;
			this.textPreview.Text = "";
			// 
			// labelFormattedPreview
			// 
			this.labelFormattedPreview.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelFormattedPreview.Location = new System.Drawing.Point(164, 40);
			this.labelFormattedPreview.Name = "labelFormattedPreview";
			this.labelFormattedPreview.Size = new System.Drawing.Size(144, 13);
			this.labelFormattedPreview.TabIndex = 8;
			this.labelFormattedPreview.Text = "Message Preview";
			// 
			// checkIncludeInsurance
			// 
			this.checkIncludeInsurance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeInsurance.Location = new System.Drawing.Point(112, 91);
			this.checkIncludeInsurance.Name = "checkIncludeInsurance";
			this.checkIncludeInsurance.Size = new System.Drawing.Size(14, 21);
			this.checkIncludeInsurance.TabIndex = 11;
			this.checkIncludeInsurance.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.checkIncludeInsurance.UseVisualStyleBackColor = true;
			this.checkIncludeInsurance.Click += new System.EventHandler(this.checkIncludeInsurance_Click);
			// 
			// butPreview
			// 
			this.butPreview.Location = new System.Drawing.Point(12, 208);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(93, 24);
			this.butPreview.TabIndex = 13;
			this.butPreview.Text = "Preview Stmt";
			this.butPreview.UseVisualStyleBackColor = true;
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// grouTemplateOptions
			// 
			this.grouTemplateOptions.Controls.Add(this.groupBox1);
			this.grouTemplateOptions.Controls.Add(this.label1);
			this.grouTemplateOptions.Controls.Add(this.checkBalForFam);
			this.grouTemplateOptions.Controls.Add(this.labelCheckIns);
			this.grouTemplateOptions.Controls.Add(this.checkIncludeInsurance);
			this.grouTemplateOptions.Location = new System.Drawing.Point(12, 59);
			this.grouTemplateOptions.Name = "grouTemplateOptions";
			this.grouTemplateOptions.Size = new System.Drawing.Size(149, 143);
			this.grouTemplateOptions.TabIndex = 14;
			this.grouTemplateOptions.TabStop = false;
			this.grouTemplateOptions.Text = "Template Options";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioAccount);
			this.groupBox1.Controls.Add(this.radioAppointment);
			this.groupBox1.Location = new System.Drawing.Point(20, 19);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(106, 66);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Templates";
			// 
			// radioAccount
			// 
			this.radioAccount.AutoSize = true;
			this.radioAccount.Location = new System.Drawing.Point(6, 42);
			this.radioAccount.Name = "radioAccount";
			this.radioAccount.Size = new System.Drawing.Size(65, 17);
			this.radioAccount.TabIndex = 19;
			this.radioAccount.Text = "Account";
			this.radioAccount.UseVisualStyleBackColor = true;
			this.radioAccount.CheckedChanged += new System.EventHandler(this.radioAccount_CheckedChanged);
			// 
			// radioAppointment
			// 
			this.radioAppointment.AutoSize = true;
			this.radioAppointment.Checked = true;
			this.radioAppointment.Location = new System.Drawing.Point(6, 19);
			this.radioAppointment.Name = "radioAppointment";
			this.radioAppointment.Size = new System.Drawing.Size(84, 17);
			this.radioAppointment.TabIndex = 18;
			this.radioAppointment.TabStop = true;
			this.radioAppointment.Text = "Appointment";
			this.radioAppointment.UseVisualStyleBackColor = true;
			this.radioAppointment.CheckedChanged += new System.EventHandler(this.radioAppointment_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 116);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 21);
			this.label1.TabIndex = 17;
			this.label1.Text = "Show Family Bal";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkBalForFam
			// 
			this.checkBalForFam.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBalForFam.Location = new System.Drawing.Point(112, 112);
			this.checkBalForFam.Name = "checkBalForFam";
			this.checkBalForFam.Size = new System.Drawing.Size(14, 21);
			this.checkBalForFam.TabIndex = 16;
			this.checkBalForFam.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.checkBalForFam.UseVisualStyleBackColor = true;
			this.checkBalForFam.Click += new System.EventHandler(this.checkBalForFam_Click);
			// 
			// labelCheckIns
			// 
			this.labelCheckIns.Location = new System.Drawing.Point(3, 92);
			this.labelCheckIns.Name = "labelCheckIns";
			this.labelCheckIns.Size = new System.Drawing.Size(72, 21);
			this.labelCheckIns.TabIndex = 15;
			this.labelCheckIns.Text = "Include Ins";
			this.labelCheckIns.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBoxClinicPicker
			// 
			this.comboBoxClinicPicker.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.comboBoxClinicPicker.Location = new System.Drawing.Point(167, 12);
			this.comboBoxClinicPicker.Name = "comboBoxClinicPicker";
			this.comboBoxClinicPicker.Size = new System.Drawing.Size(193, 21);
			this.comboBoxClinicPicker.TabIndex = 18;
			this.comboBoxClinicPicker.SelectionChangeCommitted += new System.EventHandler(this.comboBoxClinicPicker_SelectionChangeCommitted);
			// 
			// toolBarMain
			// 
			this.toolBarMain.Location = new System.Drawing.Point(8, 12);
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.Size = new System.Drawing.Size(95, 23);
			this.toolBarMain.TabIndex = 20;
			this.toolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarMain_ButtonClick);
			// 
			// FormTextPayLink
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(392, 421);
			this.Controls.Add(this.toolBarMain);
			this.Controls.Add(this.comboBoxClinicPicker);
			this.Controls.Add(this.grouTemplateOptions);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.labelFormattedPreview);
			this.Controls.Add(this.textPreview);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTextPayLink";
			this.Text = "Payment Link Preview";
			this.Load += new System.EventHandler(this.FormTextPayLink_Load);
			this.grouTemplateOptions.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textPreview;
		private System.Windows.Forms.Label labelFormattedPreview;
		private System.Windows.Forms.CheckBox checkIncludeInsurance;
		private UI.Button butPreview;
		private System.Windows.Forms.GroupBox grouTemplateOptions;
		private System.Windows.Forms.Label labelCheckIns;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBalForFam;
		private UI.ComboBoxClinicPicker comboBoxClinicPicker;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioAccount;
		private System.Windows.Forms.RadioButton radioAppointment;
		private UI.ToolBarOD toolBarMain;
	}
}