namespace OpenDental{
	partial class FormMassEmailTemplates {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailTemplates));
			this.userControlEmailTemplate = new OpenDental.UserControlEmailTemplate();
			this.butEditTemplate = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butNewTemplate = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.butSendEmails = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelNotSignedUp = new System.Windows.Forms.Label();
			this.butExamples = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// userControlEmailTemplate
			// 
			this.userControlEmailTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.userControlEmailTemplate.Location = new System.Drawing.Point(349, 50);
			this.userControlEmailTemplate.Name = "userControlEmailTemplate";
			this.userControlEmailTemplate.Size = new System.Drawing.Size(816, 539);
			this.userControlEmailTemplate.TabIndex = 138;
			// 
			// butEditTemplate
			// 
			this.butEditTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditTemplate.Location = new System.Drawing.Point(237, 12);
			this.butEditTemplate.Name = "butEditTemplate";
			this.butEditTemplate.Size = new System.Drawing.Size(101, 25);
			this.butEditTemplate.TabIndex = 134;
			this.butEditTemplate.Text = "Edit Template";
			this.butEditTemplate.Click += new System.EventHandler(this.butEditTemplate_Click);
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(140, 12);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(83, 25);
			this.butImport.TabIndex = 137;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butNewTemplate
			// 
			this.butNewTemplate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNewTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNewTemplate.Location = new System.Drawing.Point(12, 12);
			this.butNewTemplate.Name = "butNewTemplate";
			this.butNewTemplate.Size = new System.Drawing.Size(114, 25);
			this.butNewTemplate.TabIndex = 136;
			this.butNewTemplate.Text = "New Template";
			this.butNewTemplate.Click += new System.EventHandler(this.butNewTemplate_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 50);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(327, 569);
			this.gridMain.TabIndex = 131;
			this.gridMain.Title = "Saved Templates";
			this.gridMain.TranslationName = "TableSavedTemplates";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1085, 627);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(80, 24);
			this.butClose.TabIndex = 133;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSendEmails
			// 
			this.butSendEmails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendEmails.Location = new System.Drawing.Point(951, 684);
			this.butSendEmails.Name = "butSendEmails";
			this.butSendEmails.Size = new System.Drawing.Size(113, 24);
			this.butSendEmails.TabIndex = 139;
			this.butSendEmails.Text = "Send Emails";
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopy.Location = new System.Drawing.Point(264, 624);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 25);
			this.butCopy.TabIndex = 135;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(965, 11);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 22);
			this.comboClinic.TabIndex = 217;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinicPatient_SelectionChangeCommitted);
			// 
			// labelNotSignedUp
			// 
			this.labelNotSignedUp.ForeColor = System.Drawing.Color.Red;
			this.labelNotSignedUp.Location = new System.Drawing.Point(796, 12);
			this.labelNotSignedUp.Name = "labelNotSignedUp";
			this.labelNotSignedUp.Size = new System.Drawing.Size(163, 17);
			this.labelNotSignedUp.TabIndex = 326;
			this.labelNotSignedUp.Text = "* Clinic is not signed up";
			this.labelNotSignedUp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butExamples
			// 
			this.butExamples.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExamples.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExamples.Location = new System.Drawing.Point(12, 624);
			this.butExamples.Name = "butExamples";
			this.butExamples.Size = new System.Drawing.Size(75, 25);
			this.butExamples.TabIndex = 327;
			this.butExamples.Text = "Examples";
			this.butExamples.UseVisualStyleBackColor = true;
			this.butExamples.Click += new System.EventHandler(this.butExamples_Click);
			// 
			// FormMassEmailTemplates
			// 
			this.ClientSize = new System.Drawing.Size(1177, 661);
			this.Controls.Add(this.butExamples);
			this.Controls.Add(this.labelNotSignedUp);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.userControlEmailTemplate);
			this.Controls.Add(this.butEditTemplate);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.butNewTemplate);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butSendEmails);
			this.Controls.Add(this.butCopy);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailTemplates";
			this.Text = "Mass Email Templates";
			this.Load += new System.EventHandler(this.FormMassEmailTemplates_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UserControlEmailTemplate userControlEmailTemplate;
		private UI.Button butEditTemplate;
		private UI.Button butImport;
		private UI.Button butNewTemplate;
		private UI.GridOD gridMain;
		private UI.Button butClose;
		private UI.Button butSendEmails;
		private UI.Button butCopy;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label labelNotSignedUp;
		private UI.Button butExamples;
	}
}