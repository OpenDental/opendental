namespace OpenDental {
	partial class FormPhoneGraphEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneGraphEdit));
			this.checkIsGraphed = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDateEntry = new OpenDental.ValidDate();
			this.textSchedStart1 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textEmployee = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textSchedStop2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textSchedStart2 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textSchedStop1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupOverrides = new System.Windows.Forms.GroupBox();
			this.radioNotTracked = new System.Windows.Forms.RadioButton();
			this.radioShortNotice = new System.Windows.Forms.RadioButton();
			this.radioPrescheduled = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.textStop2 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textStart2 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textStop1 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textStart1 = new System.Windows.Forms.TextBox();
			this.checkAbsent = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.butCopy = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.checkGraphDefault = new System.Windows.Forms.CheckBox();
			this.checkPrescheduledOff = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.groupProv = new System.Windows.Forms.GroupBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textProvider = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textProvStop2 = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textProvStart2 = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textProvStop1 = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textProvStart1 = new System.Windows.Forms.TextBox();
			this.butCopyEmp = new OpenDental.UI.Button();
			this.butCopyOverride = new OpenDental.UI.Button();
			this.butSign = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupOverrides.SuspendLayout();
			this.groupProv.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkIsGraphed
			// 
			this.checkIsGraphed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGraphed.Location = new System.Drawing.Point(7, 60);
			this.checkIsGraphed.Name = "checkIsGraphed";
			this.checkIsGraphed.Size = new System.Drawing.Size(109, 17);
			this.checkIsGraphed.TabIndex = 0;
			this.checkIsGraphed.Text = "Is Graphed";
			this.checkIsGraphed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGraphed.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 427);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(632, 427);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(716, 427);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(102, 15);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(82, 20);
			this.textDateEntry.TabIndex = 10;
			// 
			// textSchedStart1
			// 
			this.textSchedStart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSchedStart1.Location = new System.Drawing.Point(86, 80);
			this.textSchedStart1.Name = "textSchedStart1";
			this.textSchedStart1.ReadOnly = true;
			this.textSchedStart1.Size = new System.Drawing.Size(83, 20);
			this.textSchedStart1.TabIndex = 79;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.textEmployee);
			this.groupBox1.Controls.Add(this.label17);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textSchedStop2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textSchedStart2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textSchedStop1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textSchedStart1);
			this.groupBox1.Location = new System.Drawing.Point(13, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(180, 190);
			this.groupBox1.TabIndex = 80;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Employee Schedule";
			// 
			// label18
			// 
			this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label18.Location = new System.Drawing.Point(10, 20);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(74, 18);
			this.label18.TabIndex = 94;
			this.label18.Text = "Employee";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmployee
			// 
			this.textEmployee.Location = new System.Drawing.Point(86, 19);
			this.textEmployee.Name = "textEmployee";
			this.textEmployee.ReadOnly = true;
			this.textEmployee.Size = new System.Drawing.Size(83, 20);
			this.textEmployee.TabIndex = 93;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(17, 45);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(142, 33);
			this.label17.TabIndex = 92;
			this.label17.Text = "Default schedule.\r\nUsually not changed.";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(8, 158);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(74, 18);
			this.label5.TabIndex = 87;
			this.label5.Text = "Stop Time 2";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSchedStop2
			// 
			this.textSchedStop2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSchedStop2.Location = new System.Drawing.Point(86, 158);
			this.textSchedStop2.Name = "textSchedStop2";
			this.textSchedStop2.ReadOnly = true;
			this.textSchedStop2.Size = new System.Drawing.Size(83, 20);
			this.textSchedStop2.TabIndex = 86;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(8, 132);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 18);
			this.label4.TabIndex = 85;
			this.label4.Text = "Start Time 2";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSchedStart2
			// 
			this.textSchedStart2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSchedStart2.Location = new System.Drawing.Point(86, 132);
			this.textSchedStart2.Name = "textSchedStart2";
			this.textSchedStart2.ReadOnly = true;
			this.textSchedStart2.Size = new System.Drawing.Size(83, 20);
			this.textSchedStart2.TabIndex = 84;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(8, 106);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 18);
			this.label3.TabIndex = 83;
			this.label3.Text = "Stop Time 1";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSchedStop1
			// 
			this.textSchedStop1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSchedStop1.Location = new System.Drawing.Point(86, 106);
			this.textSchedStop1.Name = "textSchedStop1";
			this.textSchedStop1.ReadOnly = true;
			this.textSchedStop1.Size = new System.Drawing.Size(83, 20);
			this.textSchedStop1.TabIndex = 82;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(8, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 18);
			this.label2.TabIndex = 81;
			this.label2.Text = "Start Time 1";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupOverrides
			// 
			this.groupOverrides.Controls.Add(this.radioNotTracked);
			this.groupOverrides.Controls.Add(this.radioShortNotice);
			this.groupOverrides.Controls.Add(this.radioPrescheduled);
			this.groupOverrides.Controls.Add(this.label6);
			this.groupOverrides.Controls.Add(this.textStop2);
			this.groupOverrides.Controls.Add(this.label7);
			this.groupOverrides.Controls.Add(this.textStart2);
			this.groupOverrides.Controls.Add(this.label8);
			this.groupOverrides.Controls.Add(this.textStop1);
			this.groupOverrides.Controls.Add(this.label9);
			this.groupOverrides.Controls.Add(this.textStart1);
			this.groupOverrides.Location = new System.Drawing.Point(287, 96);
			this.groupOverrides.Name = "groupOverrides";
			this.groupOverrides.Size = new System.Drawing.Size(180, 191);
			this.groupOverrides.TabIndex = 88;
			this.groupOverrides.TabStop = false;
			this.groupOverrides.Text = "Overrides";
			// 
			// radioNotTracked
			// 
			this.radioNotTracked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioNotTracked.Location = new System.Drawing.Point(6, 54);
			this.radioNotTracked.Name = "radioNotTracked";
			this.radioNotTracked.Size = new System.Drawing.Size(142, 18);
			this.radioNotTracked.TabIndex = 90;
			this.radioNotTracked.Text = "Not tracked (slow, etc)";
			this.radioNotTracked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioNotTracked.UseVisualStyleBackColor = true;
			// 
			// radioShortNotice
			// 
			this.radioShortNotice.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioShortNotice.Location = new System.Drawing.Point(11, 36);
			this.radioShortNotice.Name = "radioShortNotice";
			this.radioShortNotice.Size = new System.Drawing.Size(137, 18);
			this.radioShortNotice.TabIndex = 89;
			this.radioShortNotice.Text = "Short notice, tracked";
			this.radioShortNotice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioShortNotice.UseVisualStyleBackColor = true;
			// 
			// radioPrescheduled
			// 
			this.radioPrescheduled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPrescheduled.Checked = true;
			this.radioPrescheduled.Location = new System.Drawing.Point(44, 18);
			this.radioPrescheduled.Name = "radioPrescheduled";
			this.radioPrescheduled.Size = new System.Drawing.Size(104, 18);
			this.radioPrescheduled.TabIndex = 88;
			this.radioPrescheduled.TabStop = true;
			this.radioPrescheduled.Text = "Prescheduled";
			this.radioPrescheduled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPrescheduled.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(8, 159);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(74, 18);
			this.label6.TabIndex = 87;
			this.label6.Text = "Stop Time 2";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStop2
			// 
			this.textStop2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textStop2.Location = new System.Drawing.Point(86, 159);
			this.textStop2.Name = "textStop2";
			this.textStop2.Size = new System.Drawing.Size(83, 20);
			this.textStop2.TabIndex = 86;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(8, 133);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(74, 18);
			this.label7.TabIndex = 85;
			this.label7.Text = "Start Time 2";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStart2
			// 
			this.textStart2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textStart2.Location = new System.Drawing.Point(86, 133);
			this.textStart2.Name = "textStart2";
			this.textStart2.Size = new System.Drawing.Size(83, 20);
			this.textStart2.TabIndex = 84;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(8, 107);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(74, 18);
			this.label8.TabIndex = 83;
			this.label8.Text = "Stop Time 1";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStop1
			// 
			this.textStop1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textStop1.Location = new System.Drawing.Point(86, 107);
			this.textStop1.Name = "textStop1";
			this.textStop1.Size = new System.Drawing.Size(83, 20);
			this.textStop1.TabIndex = 82;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label9.Location = new System.Drawing.Point(8, 81);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(74, 18);
			this.label9.TabIndex = 81;
			this.label9.Text = "Start Time 1";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStart1
			// 
			this.textStart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textStart1.Location = new System.Drawing.Point(86, 81);
			this.textStart1.Name = "textStart1";
			this.textStart1.Size = new System.Drawing.Size(83, 20);
			this.textStart1.TabIndex = 79;
			// 
			// checkAbsent
			// 
			this.checkAbsent.AutoCheck = false;
			this.checkAbsent.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAbsent.Location = new System.Drawing.Point(268, 42);
			this.checkAbsent.Name = "checkAbsent";
			this.checkAbsent.Size = new System.Drawing.Size(153, 17);
			this.checkAbsent.TabIndex = 95;
			this.checkAbsent.Text = "Absent, short notice";
			this.checkAbsent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAbsent.UseVisualStyleBackColor = true;
			this.checkAbsent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkAbsent_MouseDown);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(96, 314);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(234, 17);
			this.label10.TabIndex = 90;
			this.label10.Text = "Note: usually just name and date";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textNote
			// 
			this.textNote.AcceptsReturn = true;
			this.textNote.Location = new System.Drawing.Point(99, 334);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(231, 56);
			this.textNote.TabIndex = 89;
			// 
			// butCopy
			// 
			this.butCopy.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(207, 202);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(63, 24);
			this.butCopy.TabIndex = 92;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butClear
			// 
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(373, 293);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(83, 24);
			this.butClear.TabIndex = 93;
			this.butClear.Text = "Clear Times";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// checkGraphDefault
			// 
			this.checkGraphDefault.AutoCheck = false;
			this.checkGraphDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGraphDefault.Enabled = false;
			this.checkGraphDefault.Location = new System.Drawing.Point(7, 41);
			this.checkGraphDefault.Name = "checkGraphDefault";
			this.checkGraphDefault.Size = new System.Drawing.Size(109, 17);
			this.checkGraphDefault.TabIndex = 94;
			this.checkGraphDefault.Text = "Graph Default";
			this.checkGraphDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGraphDefault.UseVisualStyleBackColor = true;
			// 
			// checkPrescheduledOff
			// 
			this.checkPrescheduledOff.AutoCheck = false;
			this.checkPrescheduledOff.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrescheduledOff.Location = new System.Drawing.Point(302, 22);
			this.checkPrescheduledOff.Name = "checkPrescheduledOff";
			this.checkPrescheduledOff.Size = new System.Drawing.Size(119, 17);
			this.checkPrescheduledOff.TabIndex = 96;
			this.checkPrescheduledOff.Text = "Prescheduled Off";
			this.checkPrescheduledOff.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrescheduledOff.UseVisualStyleBackColor = true;
			this.checkPrescheduledOff.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkPrescheduledOff_MouseDown);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(198, 229);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(81, 34);
			this.label12.TabIndex = 97;
			this.label12.Text = "Only for late or leaving early";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// groupProv
			// 
			this.groupProv.Controls.Add(this.label19);
			this.groupProv.Controls.Add(this.textProvider);
			this.groupProv.Controls.Add(this.label16);
			this.groupProv.Controls.Add(this.label11);
			this.groupProv.Controls.Add(this.textProvStop2);
			this.groupProv.Controls.Add(this.label13);
			this.groupProv.Controls.Add(this.textProvStart2);
			this.groupProv.Controls.Add(this.label14);
			this.groupProv.Controls.Add(this.textProvStop1);
			this.groupProv.Controls.Add(this.label15);
			this.groupProv.Controls.Add(this.textProvStart1);
			this.groupProv.Location = new System.Drawing.Point(602, 97);
			this.groupProv.Name = "groupProv";
			this.groupProv.Size = new System.Drawing.Size(180, 191);
			this.groupProv.TabIndex = 90;
			this.groupProv.TabStop = false;
			this.groupProv.Text = "Provider Schedule";
			// 
			// label19
			// 
			this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label19.Location = new System.Drawing.Point(9, 20);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(74, 18);
			this.label19.TabIndex = 95;
			this.label19.Text = "Provider";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvider
			// 
			this.textProvider.Location = new System.Drawing.Point(86, 19);
			this.textProvider.Name = "textProvider";
			this.textProvider.ReadOnly = true;
			this.textProvider.Size = new System.Drawing.Size(83, 20);
			this.textProvider.TabIndex = 94;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(9, 45);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(166, 33);
			this.label16.TabIndex = 91;
			this.label16.Text = "Shows on Ops.  Frequently different than Employee Sched";
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.Location = new System.Drawing.Point(8, 159);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(74, 18);
			this.label11.TabIndex = 87;
			this.label11.Text = "Stop Time 2";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvStop2
			// 
			this.textProvStop2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textProvStop2.Location = new System.Drawing.Point(86, 159);
			this.textProvStop2.Name = "textProvStop2";
			this.textProvStop2.Size = new System.Drawing.Size(83, 20);
			this.textProvStop2.TabIndex = 86;
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label13.Location = new System.Drawing.Point(8, 133);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(74, 18);
			this.label13.TabIndex = 85;
			this.label13.Text = "Start Time 2";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvStart2
			// 
			this.textProvStart2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textProvStart2.Location = new System.Drawing.Point(86, 133);
			this.textProvStart2.Name = "textProvStart2";
			this.textProvStart2.Size = new System.Drawing.Size(83, 20);
			this.textProvStart2.TabIndex = 84;
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label14.Location = new System.Drawing.Point(8, 107);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(74, 18);
			this.label14.TabIndex = 83;
			this.label14.Text = "Stop Time 1";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvStop1
			// 
			this.textProvStop1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textProvStop1.Location = new System.Drawing.Point(86, 107);
			this.textProvStop1.Name = "textProvStop1";
			this.textProvStop1.Size = new System.Drawing.Size(83, 20);
			this.textProvStop1.TabIndex = 82;
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label15.Location = new System.Drawing.Point(8, 81);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(74, 18);
			this.label15.TabIndex = 81;
			this.label15.Text = "Start Time 1";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvStart1
			// 
			this.textProvStart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textProvStart1.Location = new System.Drawing.Point(86, 81);
			this.textProvStart1.Name = "textProvStart1";
			this.textProvStart1.Size = new System.Drawing.Size(83, 20);
			this.textProvStart1.TabIndex = 79;
			// 
			// butCopyEmp
			// 
			this.butCopyEmp.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopyEmp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopyEmp.Location = new System.Drawing.Point(480, 195);
			this.butCopyEmp.Name = "butCopyEmp";
			this.butCopyEmp.Size = new System.Drawing.Size(107, 24);
			this.butCopyEmp.TabIndex = 98;
			this.butCopyEmp.Text = "Copy Emp";
			this.butCopyEmp.Click += new System.EventHandler(this.butCopyEmp_Click);
			// 
			// butCopyOverride
			// 
			this.butCopyOverride.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopyOverride.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopyOverride.Location = new System.Drawing.Point(480, 231);
			this.butCopyOverride.Name = "butCopyOverride";
			this.butCopyOverride.Size = new System.Drawing.Size(107, 24);
			this.butCopyOverride.TabIndex = 100;
			this.butCopyOverride.Text = "Copy Override";
			this.butCopyOverride.Click += new System.EventHandler(this.butCopyOverride_Click);
			// 
			// butSign
			// 
			this.butSign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSign.Location = new System.Drawing.Point(268, 396);
			this.butSign.Name = "butSign";
			this.butSign.Size = new System.Drawing.Size(62, 24);
			this.butSign.TabIndex = 101;
			this.butSign.Text = "Sign";
			this.butSign.Click += new System.EventHandler(this.butSign_Click);
			// 
			// FormPhoneGraphEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(803, 463);
			this.Controls.Add(this.butSign);
			this.Controls.Add(this.butCopyOverride);
			this.Controls.Add(this.butCopyEmp);
			this.Controls.Add(this.groupProv);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.checkAbsent);
			this.Controls.Add(this.checkPrescheduledOff);
			this.Controls.Add(this.checkGraphDefault);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.groupOverrides);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkIsGraphed);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneGraphEdit";
			this.Text = "Phone Graph Edit";
			this.Load += new System.EventHandler(this.FormPhoneGraphCreate_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupOverrides.ResumeLayout(false);
			this.groupOverrides.PerformLayout();
			this.groupProv.ResumeLayout(false);
			this.groupProv.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkIsGraphed;
		private ValidDate textDateEntry;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textSchedStart1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textSchedStop2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textSchedStart2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textSchedStop1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupOverrides;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textStop2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textStart2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textStop1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textStart1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textNote;
		private UI.Button butCopy;
		private UI.Button butClear;
		private System.Windows.Forms.CheckBox checkGraphDefault;
		private System.Windows.Forms.CheckBox checkAbsent;
		private System.Windows.Forms.CheckBox checkPrescheduledOff;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.RadioButton radioShortNotice;
		private System.Windows.Forms.RadioButton radioPrescheduled;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textEmployee;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.RadioButton radioNotTracked;
		private System.Windows.Forms.GroupBox groupProv;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox textProvider;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textProvStop2;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textProvStart2;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textProvStop1;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textProvStart1;
		private UI.Button butCopyEmp;
		private UI.Button butCopyOverride;
		private UI.Button butSign;
	}
}