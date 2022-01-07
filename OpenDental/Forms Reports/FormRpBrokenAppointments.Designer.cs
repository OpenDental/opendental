namespace OpenDental{
	partial class FormRpBrokenAppointments {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpBrokenAppointments));
			this.checkAllProvs = new System.Windows.Forms.CheckBox();
			this.checkAllClinics = new System.Windows.Forms.CheckBox();
			this.listClinics = new OpenDental.UI.ListBoxOD();
			this.labelClinics = new System.Windows.Forms.Label();
			this.listProvs = new OpenDental.UI.ListBoxOD();
			this.labelProviders = new System.Windows.Forms.Label();
			this.dateEnd = new System.Windows.Forms.MonthCalendar();
			this.dateStart = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.labelDescr = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.radioProcs = new System.Windows.Forms.RadioButton();
			this.radioAdj = new System.Windows.Forms.RadioButton();
			this.radioAptStatus = new System.Windows.Forms.RadioButton();
			this.listOptions = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// checkAllProvs
			// 
			this.checkAllProvs.Checked = true;
			this.checkAllProvs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProvs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProvs.Location = new System.Drawing.Point(31, 222);
			this.checkAllProvs.Name = "checkAllProvs";
			this.checkAllProvs.Size = new System.Drawing.Size(95, 16);
			this.checkAllProvs.TabIndex = 64;
			this.checkAllProvs.Text = "All";
			this.checkAllProvs.Click += new System.EventHandler(this.checkAllProvs_Click);
			// 
			// checkAllClinics
			// 
			this.checkAllClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClinics.Location = new System.Drawing.Point(157, 222);
			this.checkAllClinics.Name = "checkAllClinics";
			this.checkAllClinics.Size = new System.Drawing.Size(120, 16);
			this.checkAllClinics.TabIndex = 63;
			this.checkAllClinics.Text = "All (Includes hidden)";
			this.checkAllClinics.Click += new System.EventHandler(this.checkAllClinics_Click);
			// 
			// listClinics
			// 
			this.listClinics.Location = new System.Drawing.Point(157, 241);
			this.listClinics.Name = "listClinics";
			this.listClinics.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClinics.Size = new System.Drawing.Size(120, 186);
			this.listClinics.TabIndex = 62;
			this.listClinics.Click += new System.EventHandler(this.listClinics_Click);
			// 
			// labelClinics
			// 
			this.labelClinics.Location = new System.Drawing.Point(157, 203);
			this.labelClinics.Name = "labelClinics";
			this.labelClinics.Size = new System.Drawing.Size(104, 16);
			this.labelClinics.TabIndex = 61;
			this.labelClinics.Text = "Clinics";
			this.labelClinics.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listProvs
			// 
			this.listProvs.Location = new System.Drawing.Point(31, 241);
			this.listProvs.Name = "listProvs";
			this.listProvs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProvs.Size = new System.Drawing.Size(120, 186);
			this.listProvs.TabIndex = 60;
			this.listProvs.Click += new System.EventHandler(this.listProvs_Click);
			// 
			// labelProviders
			// 
			this.labelProviders.Location = new System.Drawing.Point(31, 202);
			this.labelProviders.Name = "labelProviders";
			this.labelProviders.Size = new System.Drawing.Size(104, 16);
			this.labelProviders.TabIndex = 59;
			this.labelProviders.Text = "Providers";
			this.labelProviders.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// dateEnd
			// 
			this.dateEnd.Location = new System.Drawing.Point(326, 36);
			this.dateEnd.Name = "dateEnd";
			this.dateEnd.TabIndex = 66;
			// 
			// dateStart
			// 
			this.dateStart.Location = new System.Drawing.Point(64, 36);
			this.dateStart.Name = "dateStart";
			this.dateStart.TabIndex = 65;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(212, 44);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 67;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelDescr
			// 
			this.labelDescr.Location = new System.Drawing.Point(66, 15);
			this.labelDescr.Name = "labelDescr";
			this.labelDescr.Size = new System.Drawing.Size(481, 13);
			this.labelDescr.TabIndex = 68;
			this.labelDescr.Text = "Broken appointments based on appointment status.";
			this.labelDescr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(435, 446);
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
			this.butCancel.Location = new System.Drawing.Point(525, 446);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			// 
			// radioProcs
			// 
			this.radioProcs.Location = new System.Drawing.Point(287, 255);
			this.radioProcs.Name = "radioProcs";
			this.radioProcs.Size = new System.Drawing.Size(143, 18);
			this.radioProcs.TabIndex = 69;
			this.radioProcs.TabStop = true;
			this.radioProcs.Text = "By procedures";
			this.radioProcs.CheckedChanged += new System.EventHandler(this.radioProcs_CheckedChanged);
			// 
			// radioAdj
			// 
			this.radioAdj.Location = new System.Drawing.Point(287, 277);
			this.radioAdj.Name = "radioAdj";
			this.radioAdj.Size = new System.Drawing.Size(143, 18);
			this.radioAdj.TabIndex = 70;
			this.radioAdj.TabStop = true;
			this.radioAdj.Text = "By adjustments";
			this.radioAdj.CheckedChanged += new System.EventHandler(this.radioAdj_CheckedChanged);
			// 
			// radioAptStatus
			// 
			this.radioAptStatus.Location = new System.Drawing.Point(287, 299);
			this.radioAptStatus.Name = "radioAptStatus";
			this.radioAptStatus.Size = new System.Drawing.Size(143, 18);
			this.radioAptStatus.TabIndex = 71;
			this.radioAptStatus.TabStop = true;
			this.radioAptStatus.Text = "By appointment status";
			this.radioAptStatus.CheckedChanged += new System.EventHandler(this.radioAptStatus_CheckedChanged);
			// 
			// listByOptions
			// 
			this.listOptions.Location = new System.Drawing.Point(435, 241);
			this.listOptions.Name = "listByOptions";
			this.listOptions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listOptions.Size = new System.Drawing.Size(152, 186);
			this.listOptions.TabIndex = 72;
			this.listOptions.Visible = false;
			// 
			// FormRpBrokenAppointments
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(612, 482);
			this.Controls.Add(this.listOptions);
			this.Controls.Add(this.radioAptStatus);
			this.Controls.Add(this.radioAdj);
			this.Controls.Add(this.radioProcs);
			this.Controls.Add(this.labelDescr);
			this.Controls.Add(this.dateEnd);
			this.Controls.Add(this.dateStart);
			this.Controls.Add(this.labelTO);
			this.Controls.Add(this.checkAllProvs);
			this.Controls.Add(this.checkAllClinics);
			this.Controls.Add(this.listClinics);
			this.Controls.Add(this.labelClinics);
			this.Controls.Add(this.listProvs);
			this.Controls.Add(this.labelProviders);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpBrokenAppointments";
			this.Text = "Broken Appointments Report";
			this.Load += new System.EventHandler(this.FormRpBrokenAppointments_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkAllProvs;
		private System.Windows.Forms.CheckBox checkAllClinics;
		private OpenDental.UI.ListBoxOD listClinics;
		private System.Windows.Forms.Label labelClinics;
		private OpenDental.UI.ListBoxOD listProvs;
		private System.Windows.Forms.Label labelProviders;
		private System.Windows.Forms.MonthCalendar dateEnd;
		private System.Windows.Forms.MonthCalendar dateStart;
		private System.Windows.Forms.Label labelTO;
		private System.Windows.Forms.Label labelDescr;
		private System.Windows.Forms.RadioButton radioProcs;
		private System.Windows.Forms.RadioButton radioAdj;
		private System.Windows.Forms.RadioButton radioAptStatus;
		private OpenDental.UI.ListBoxOD listOptions;
	}
}