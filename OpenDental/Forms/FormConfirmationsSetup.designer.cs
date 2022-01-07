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
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.butSetup = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.comboStatusTextMessagedConfirm = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboStatusEmailedConfirm = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkGroupFamilies = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(696, 505);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 27);
			this.butOK.TabIndex = 97;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(777, 505);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 27);
			this.butCancel.TabIndex = 98;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
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
			this.gridMain.Size = new System.Drawing.Size(840, 429);
			this.gridMain.TabIndex = 87;
			this.gridMain.Title = "Messages";
			this.gridMain.TranslationName = "TableConfirmMsgs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// comboStatusTextMessagedConfirm
			// 
			this.comboStatusTextMessagedConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusTextMessagedConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatusTextMessagedConfirm.FormattingEnabled = true;
			this.comboStatusTextMessagedConfirm.Location = new System.Drawing.Point(291, 511);
			this.comboStatusTextMessagedConfirm.MaxDropDownItems = 20;
			this.comboStatusTextMessagedConfirm.Name = "comboStatusTextMessagedConfirm";
			this.comboStatusTextMessagedConfirm.Size = new System.Drawing.Size(233, 21);
			this.comboStatusTextMessagedConfirm.TabIndex = 91;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(13, 515);
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
			this.comboStatusEmailedConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatusEmailedConfirm.FormattingEnabled = true;
			this.comboStatusEmailedConfirm.Location = new System.Drawing.Point(291, 489);
			this.comboStatusEmailedConfirm.MaxDropDownItems = 20;
			this.comboStatusEmailedConfirm.Name = "comboStatusEmailedConfirm";
			this.comboStatusEmailedConfirm.Size = new System.Drawing.Size(233, 21);
			this.comboStatusEmailedConfirm.TabIndex = 90;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(13, 493);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(276, 19);
			this.label5.TabIndex = 88;
			this.label5.Text = "Status for e-mailed confirmation";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkGroupFamilies
			// 
			this.checkGroupFamilies.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGroupFamilies.Location = new System.Drawing.Point(696, 479);
			this.checkGroupFamilies.Name = "checkGroupFamilies";
			this.checkGroupFamilies.Size = new System.Drawing.Size(110, 20);
			this.checkGroupFamilies.TabIndex = 99;
			this.checkGroupFamilies.Text = "Group Families";
			this.checkGroupFamilies.UseVisualStyleBackColor = true;
			// 
			// FormConfirmationSetup
			// 
			this.ClientSize = new System.Drawing.Size(864, 563);
			this.Controls.Add(this.checkGroupFamilies);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
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
		private System.Windows.Forms.ComboBox comboStatusTextMessagedConfirm;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboStatusEmailedConfirm;
		private System.Windows.Forms.Label label5;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkGroupFamilies;
	}
}