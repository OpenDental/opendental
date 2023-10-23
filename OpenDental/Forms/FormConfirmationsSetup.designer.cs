namespace OpenDental{
	partial class FormConfirmationSetup {
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
			this.butSave = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.butSetup = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.comboStatusTextMessagedConfirm = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboStatusEmailedConfirm = new OpenDental.UI.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkGroupFamilies = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(777, 511);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 27);
			this.butSave.TabIndex = 97;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(297, 10);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(555, 28);
			this.label7.TabIndex = 96;
			this.label7.Text = "Automated eConfirmation and eReminders are alternatives to using manual confirmat" +
    "ions and reminders.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSetup
			// 
			this.butSetup.Location = new System.Drawing.Point(13, 12);
			this.butSetup.Name = "butSetup";
			this.butSetup.Size = new System.Drawing.Size(279, 24);
			this.butSetup.TabIndex = 95;
			this.butSetup.Text = "Automated eConfirmation && eReminder Setup";
			this.butSetup.Click += new System.EventHandler(this.butSetup_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 44);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(840, 435);
			this.gridMain.TabIndex = 87;
			this.gridMain.Title = "Messages";
			this.gridMain.TranslationName = "TableConfirmMsgs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// comboStatusTextMessagedConfirm
			// 
			this.comboStatusTextMessagedConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusTextMessagedConfirm.Location = new System.Drawing.Point(291, 517);
			this.comboStatusTextMessagedConfirm.Name = "comboStatusTextMessagedConfirm";
			this.comboStatusTextMessagedConfirm.Size = new System.Drawing.Size(233, 21);
			this.comboStatusTextMessagedConfirm.TabIndex = 91;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(13, 521);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(276, 19);
			this.label6.TabIndex = 89;
			this.label6.Text = "Status for text messaged confirmation";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboStatusEmailedConfirm
			// 
			this.comboStatusEmailedConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEmailedConfirm.Location = new System.Drawing.Point(291, 495);
			this.comboStatusEmailedConfirm.Name = "comboStatusEmailedConfirm";
			this.comboStatusEmailedConfirm.Size = new System.Drawing.Size(233, 21);
			this.comboStatusEmailedConfirm.TabIndex = 90;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(13, 499);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(276, 19);
			this.label5.TabIndex = 88;
			this.label5.Text = "Status for e-mailed confirmation";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkGroupFamilies
			// 
			this.checkGroupFamilies.Location = new System.Drawing.Point(696, 485);
			this.checkGroupFamilies.Name = "checkGroupFamilies";
			this.checkGroupFamilies.Size = new System.Drawing.Size(110, 20);
			this.checkGroupFamilies.TabIndex = 99;
			this.checkGroupFamilies.Text = "Group Families";
			// 
			// FormConfirmationSetup
			// 
			this.ClientSize = new System.Drawing.Size(864, 563);
			this.Controls.Add(this.checkGroupFamilies);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butSetup);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.comboStatusTextMessagedConfirm);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboStatusEmailedConfirm);
			this.Controls.Add(this.label5);
			this.Name = "FormConfirmationSetup";
			this.Text = "Setup Confirmation";
			this.Load += new System.EventHandler(this.FormConfirmationSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion
		
		private System.Windows.Forms.Label label7;
		private UI.Button butSetup;
		private UI.GridOD gridMain;
		private OpenDental.UI.ComboBox comboStatusTextMessagedConfirm;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.ComboBox comboStatusEmailedConfirm;
		private System.Windows.Forms.Label label5;
		private UI.Button butSave;
		private OpenDental.UI.CheckBox checkGroupFamilies;
	}
}