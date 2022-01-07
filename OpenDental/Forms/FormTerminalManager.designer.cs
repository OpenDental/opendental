namespace OpenDental {
	partial class FormTerminalManager {
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

		///<summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTerminalManager));
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBoxPassword = new System.Windows.Forms.GroupBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.groupBoxPatient = new System.Windows.Forms.GroupBox();
			this.butRemoveTreatPlan = new OpenDental.UI.Button();
			this.listTreatPlans = new OpenDental.UI.ListBoxOD();
			this.labelTreatPlans = new System.Windows.Forms.Label();
			this.butPatForms = new OpenDental.UI.Button();
			this.labelSheets = new System.Windows.Forms.Label();
			this.labelPatient = new System.Windows.Forms.Label();
			this.listSheets = new OpenDental.UI.ListBoxOD();
			this.butByod = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.contrClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.butClose = new OpenDental.UI.Button();
			this.groupBoxPassword.SuspendLayout();
			this.groupBoxPatient.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(21, 92);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(727, 418);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Active Kiosks";
			this.gridMain.TranslationName = "TableTerminals";
			this.gridMain.SelectionCommitted += new System.EventHandler(this.gridMain_SelectionCommitted);
			// 
			// groupBoxPassword
			// 
			this.groupBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxPassword.Controls.Add(this.textPassword);
			this.groupBoxPassword.Controls.Add(this.label2);
			this.groupBoxPassword.Controls.Add(this.butSave);
			this.groupBoxPassword.Location = new System.Drawing.Point(21, 517);
			this.groupBoxPassword.Name = "groupBoxPassword";
			this.groupBoxPassword.Size = new System.Drawing.Size(506, 80);
			this.groupBoxPassword.TabIndex = 12;
			this.groupBoxPassword.TabStop = false;
			this.groupBoxPassword.Text = "Password";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(10, 50);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(129, 20);
			this.textPassword.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(327, 31);
			this.label2.TabIndex = 4;
			this.label2.Text = "To close a kiosk, go to that computer and click the hidden button at the lower ri" +
    "ght.  You will need to enter this password:";
			// 
			// butSave
			// 
			this.butSave.Location = new System.Drawing.Point(145, 48);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(97, 24);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "Save Password";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// groupBoxPatient
			// 
			this.groupBoxPatient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxPatient.Controls.Add(this.butRemoveTreatPlan);
			this.groupBoxPatient.Controls.Add(this.listTreatPlans);
			this.groupBoxPatient.Controls.Add(this.labelTreatPlans);
			this.groupBoxPatient.Controls.Add(this.butPatForms);
			this.groupBoxPatient.Controls.Add(this.labelSheets);
			this.groupBoxPatient.Controls.Add(this.labelPatient);
			this.groupBoxPatient.Controls.Add(this.listSheets);
			this.groupBoxPatient.Location = new System.Drawing.Point(754, 90);
			this.groupBoxPatient.Name = "groupBoxPatient";
			this.groupBoxPatient.Size = new System.Drawing.Size(180, 390);
			this.groupBoxPatient.TabIndex = 11;
			this.groupBoxPatient.TabStop = false;
			this.groupBoxPatient.Text = "Current Patient";
			// 
			// butRemoveTreatPlan
			// 
			this.butRemoveTreatPlan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemoveTreatPlan.Location = new System.Drawing.Point(11, 357);
			this.butRemoveTreatPlan.Name = "butRemoveTreatPlan";
			this.butRemoveTreatPlan.Size = new System.Drawing.Size(158, 24);
			this.butRemoveTreatPlan.TabIndex = 19;
			this.butRemoveTreatPlan.Text = "Remove Treatment Plan";
			this.butRemoveTreatPlan.UseVisualStyleBackColor = true;
			this.butRemoveTreatPlan.Click += new System.EventHandler(this.butRemoveTreatPlan_Click);
			// 
			// listTreatPlans
			// 
			this.listTreatPlans.Location = new System.Drawing.Point(11, 233);
			this.listTreatPlans.Name = "listTreatPlans";
			this.listTreatPlans.Size = new System.Drawing.Size(158, 121);
			this.listTreatPlans.TabIndex = 18;
			this.listTreatPlans.SelectedIndexChanged += new System.EventHandler(this.listTreatPlans_SelectedIndexChanged);
			// 
			// labelTreatPlans
			// 
			this.labelTreatPlans.Location = new System.Drawing.Point(11, 213);
			this.labelTreatPlans.Name = "labelTreatPlans";
			this.labelTreatPlans.Size = new System.Drawing.Size(155, 17);
			this.labelTreatPlans.TabIndex = 17;
			this.labelTreatPlans.Text = "Treatment Plans on Device";
			this.labelTreatPlans.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butPatForms
			// 
			this.butPatForms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.butPatForms.Location = new System.Drawing.Point(11, 186);
			this.butPatForms.Name = "butPatForms";
			this.butPatForms.Size = new System.Drawing.Size(158, 24);
			this.butPatForms.TabIndex = 16;
			this.butPatForms.Text = "Add or Remove Forms";
			this.butPatForms.Click += new System.EventHandler(this.butPatForms_Click);
			// 
			// labelSheets
			// 
			this.labelSheets.Location = new System.Drawing.Point(11, 41);
			this.labelSheets.Name = "labelSheets";
			this.labelSheets.Size = new System.Drawing.Size(146, 17);
			this.labelSheets.TabIndex = 10;
			this.labelSheets.Text = "Forms for Kiosk";
			this.labelSheets.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(11, 19);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(147, 18);
			this.labelPatient.TabIndex = 9;
			this.labelPatient.Text = "Fname Lname";
			// 
			// listSheets
			// 
			this.listSheets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listSheets.Location = new System.Drawing.Point(11, 62);
			this.listSheets.Name = "listSheets";
			this.listSheets.Size = new System.Drawing.Size(158, 121);
			this.listSheets.TabIndex = 8;
			// 
			// butByod
			// 
			this.butByod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butByod.Location = new System.Drawing.Point(765, 486);
			this.butByod.Name = "butByod";
			this.butByod.Size = new System.Drawing.Size(158, 24);
			this.butByod.TabIndex = 17;
			this.butByod.Text = "Send eClipboard BYOD Text";
			this.butByod.Click += new System.EventHandler(this.butByod_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(580, 49);
			this.label1.TabIndex = 3;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// contrClinicPicker
			// 
			this.contrClinicPicker.IncludeAll = true;
			this.contrClinicPicker.IncludeUnassigned = true;
			this.contrClinicPicker.Location = new System.Drawing.Point(90, 60);
			this.contrClinicPicker.Name = "contrClinicPicker";
			this.contrClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.contrClinicPicker.TabIndex = 16;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(859, 573);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 15;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormTerminalManager
			// 
			this.ClientSize = new System.Drawing.Size(946, 609);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butByod);
			this.Controls.Add(this.contrClinicPicker);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBoxPassword);
			this.Controls.Add(this.groupBoxPatient);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTerminalManager";
			this.Text = "Kiosk Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTerminalManager_FormClosing);
			this.Load += new System.EventHandler(this.FormTerminalManager_Load);
			this.groupBoxPassword.ResumeLayout(false);
			this.groupBoxPassword.PerformLayout();
			this.groupBoxPatient.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.GroupBox groupBoxPatient;
		private OpenDental.UI.ListBoxOD listSheets;
		private System.Windows.Forms.Label labelSheets;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.GroupBox groupBoxPassword;
		private OpenDental.UI.Button butClose;
		private UI.Button butPatForms;
		private UI.ComboBoxClinicPicker contrClinicPicker;
		private UI.Button butByod;
		private UI.ListBoxOD listTreatPlans;
		private System.Windows.Forms.Label labelTreatPlans;
		private UI.Button butRemoveTreatPlan;
	}
}
