namespace OpenDental{
	partial class FormRpActivePatients {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpActivePatients));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listBillingTypes = new OpenDental.UI.ListBoxOD();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.dateEnd = new System.Windows.Forms.MonthCalendar();
			this.dateStart = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkAllBilling = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkAllPatStatus = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listPatientStatuses = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(511, 454);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(592, 454);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listBillingTypes
			// 
			this.listBillingTypes.Location = new System.Drawing.Point(12, 240);
			this.listBillingTypes.Name = "listBillingTypes";
			this.listBillingTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillingTypes.Size = new System.Drawing.Size(163, 199);
			this.listBillingTypes.TabIndex = 69;
			this.listBillingTypes.Click += new System.EventHandler(this.listBillingTypes_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(519, 221);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 68;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.CheckedChanged += new System.EventHandler(this.checkAllClin_CheckedChanged);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(519, 240);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 199);
			this.listClin.TabIndex = 67;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(516, 203);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 66;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(350, 221);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 61;
			this.checkAllProv.Text = "All";
			this.checkAllProv.CheckedChanged += new System.EventHandler(this.checkAllProv_CheckedChanged);
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(350, 240);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(163, 199);
			this.listProv.TabIndex = 60;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(347, 203);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 59;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// dateEnd
			// 
			this.dateEnd.Location = new System.Drawing.Point(277, 32);
			this.dateEnd.Name = "dateEnd";
			this.dateEnd.TabIndex = 57;
			// 
			// dateStart
			// 
			this.dateStart.Location = new System.Drawing.Point(12, 32);
			this.dateStart.Name = "dateStart";
			this.dateStart.TabIndex = 56;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(220, 40);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 58;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 203);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(110, 16);
			this.label2.TabIndex = 70;
			this.label2.Text = "Billing Types";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllBilling
			// 
			this.checkAllBilling.Checked = true;
			this.checkAllBilling.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllBilling.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllBilling.Location = new System.Drawing.Point(12, 222);
			this.checkAllBilling.Name = "checkAllBilling";
			this.checkAllBilling.Size = new System.Drawing.Size(95, 16);
			this.checkAllBilling.TabIndex = 71;
			this.checkAllBilling.Text = "All";
			this.checkAllBilling.CheckedChanged += new System.EventHandler(this.checkAllBilling_CheckedChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(492, 16);
			this.label3.TabIndex = 72;
			this.label3.Text = "Used to get a list of all patients that have had a completed procedure within the" +
    " date range.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllPatStatus
			// 
			this.checkAllPatStatus.Checked = true;
			this.checkAllPatStatus.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllPatStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllPatStatus.Location = new System.Drawing.Point(181, 222);
			this.checkAllPatStatus.Name = "checkAllPatStatus";
			this.checkAllPatStatus.Size = new System.Drawing.Size(95, 16);
			this.checkAllPatStatus.TabIndex = 75;
			this.checkAllPatStatus.Text = "All";
			this.checkAllPatStatus.CheckedChanged += new System.EventHandler(this.checkAllPatStatus_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(178, 203);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110, 16);
			this.label4.TabIndex = 74;
			this.label4.Text = "Patient Status";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPatStatus
			// 
			this.listPatientStatuses.Location = new System.Drawing.Point(181, 240);
			this.listPatientStatuses.Name = "listPatStatus";
			this.listPatientStatuses.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPatientStatuses.Size = new System.Drawing.Size(163, 199);
			this.listPatientStatuses.TabIndex = 73;
			this.listPatientStatuses.Click += new System.EventHandler(this.listPatientStatuses_Click);
			// 
			// FormRpActivePatients
			// 
			this.ClientSize = new System.Drawing.Size(679, 490);
			this.Controls.Add(this.checkAllPatStatus);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listPatientStatuses);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkAllBilling);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listBillingTypes);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dateEnd);
			this.Controls.Add(this.dateStart);
			this.Controls.Add(this.labelTO);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpActivePatients";
			this.Text = "Active Patients Report";
			this.Load += new System.EventHandler(this.FormRpActivePatients_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listBillingTypes;
		private System.Windows.Forms.CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private System.Windows.Forms.Label labelClin;
		private System.Windows.Forms.CheckBox checkAllProv;
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MonthCalendar dateEnd;
		private System.Windows.Forms.MonthCalendar dateStart;
		private System.Windows.Forms.Label labelTO;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkAllBilling;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkAllPatStatus;
		private System.Windows.Forms.Label label4;
		private UI.ListBoxOD listPatientStatuses;
	}
}