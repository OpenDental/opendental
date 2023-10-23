namespace OpenDental{
	partial class FormTimeCardRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCardRuleEdit));
			this.label5 = new System.Windows.Forms.Label();
			this.checkIsOvertimeExempt = new OpenDental.UI.CheckBox();
			this.textClockInMin = new System.Windows.Forms.TextBox();
			this.textOverHoursPerDay = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.but6am = new OpenDental.UI.Button();
			this.textAfterTimeOfDay = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.but5pm = new OpenDental.UI.Button();
			this.textBeforeTimeOfDay = new System.Windows.Forms.TextBox();
			this.listEmp = new OpenDental.UI.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkUseRate3 = new OpenDental.UI.CheckBox();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(34, 163);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(150, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Earliest Clock in Time";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsOvertimeExempt
			// 
			this.checkIsOvertimeExempt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsOvertimeExempt.Location = new System.Drawing.Point(61, 143);
			this.checkIsOvertimeExempt.Name = "checkIsOvertimeExempt";
			this.checkIsOvertimeExempt.Size = new System.Drawing.Size(139, 16);
			this.checkIsOvertimeExempt.TabIndex = 4;
			this.checkIsOvertimeExempt.Text = "Is Overtime Exempt";
			// 
			// textClockInMin
			// 
			this.textClockInMin.Location = new System.Drawing.Point(187, 163);
			this.textClockInMin.Name = "textClockInMin";
			this.textClockInMin.Size = new System.Drawing.Size(62, 20);
			this.textClockInMin.TabIndex = 5;
			this.textClockInMin.Text = "6:00";
			// 
			// textOverHoursPerDay
			// 
			this.textOverHoursPerDay.Location = new System.Drawing.Point(187, 10);
			this.textOverHoursPerDay.Name = "textOverHoursPerDay";
			this.textOverHoursPerDay.Size = new System.Drawing.Size(62, 20);
			this.textOverHoursPerDay.TabIndex = 1;
			this.textOverHoursPerDay.Text = "8:00";
			this.textOverHoursPerDay.TextChanged += new System.EventHandler(this.textOverHoursPerDay_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(179, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Overtime if over Hours Per Day";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.but6am);
			this.groupBox2.Controls.Add(this.textAfterTimeOfDay);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.but5pm);
			this.groupBox2.Controls.Add(this.textBeforeTimeOfDay);
			this.groupBox2.Location = new System.Drawing.Point(34, 41);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(282, 75);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.Text = "Rate2 Hours";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(25, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(125, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Before Time of Day";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// but6am
			// 
			this.but6am.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.but6am.Location = new System.Drawing.Point(221, 17);
			this.but6am.Name = "but6am";
			this.but6am.Size = new System.Drawing.Size(35, 22);
			this.but6am.TabIndex = 0;
			this.but6am.TabStop = false;
			this.but6am.Text = "6 AM";
			this.but6am.Click += new System.EventHandler(this.but6am_Click);
			// 
			// textAfterTimeOfDay
			// 
			this.textAfterTimeOfDay.Location = new System.Drawing.Point(153, 46);
			this.textAfterTimeOfDay.Name = "textAfterTimeOfDay";
			this.textAfterTimeOfDay.Size = new System.Drawing.Size(62, 20);
			this.textAfterTimeOfDay.TabIndex = 2;
			this.textAfterTimeOfDay.Text = "17:00";
			this.textAfterTimeOfDay.TextChanged += new System.EventHandler(this.textAfterTimeOfDay_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(39, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(111, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "After Time of Day";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// but5pm
			// 
			this.but5pm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.but5pm.Location = new System.Drawing.Point(221, 45);
			this.but5pm.Name = "but5pm";
			this.but5pm.Size = new System.Drawing.Size(35, 22);
			this.but5pm.TabIndex = 0;
			this.but5pm.TabStop = false;
			this.but5pm.Text = "5 PM";
			this.but5pm.Click += new System.EventHandler(this.but5pm_Click);
			// 
			// textBeforeTimeOfDay
			// 
			this.textBeforeTimeOfDay.Location = new System.Drawing.Point(153, 19);
			this.textBeforeTimeOfDay.Name = "textBeforeTimeOfDay";
			this.textBeforeTimeOfDay.Size = new System.Drawing.Size(62, 20);
			this.textBeforeTimeOfDay.TabIndex = 1;
			this.textBeforeTimeOfDay.Text = "6:00";
			this.textBeforeTimeOfDay.TextChanged += new System.EventHandler(this.textBeforeTimeOfDay_TextChanged);
			// 
			// listEmp
			// 
			this.listEmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listEmp.Location = new System.Drawing.Point(34, 210);
			this.listEmp.Name = "listEmp";
			this.listEmp.Size = new System.Drawing.Size(282, 212);
			this.listEmp.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 190);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(175, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Employee (can select multiple)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(21, 439);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(191, 439);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = false;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(272, 439);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkUseRate3
			// 
			this.checkUseRate3.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseRate3.Location = new System.Drawing.Point(7, 122);
			this.checkUseRate3.Name = "checkUseRate3";
			this.checkUseRate3.Size = new System.Drawing.Size(193, 16);
			this.checkUseRate3.TabIndex = 3;
			this.checkUseRate3.Text = "Use Rate3 for Weekend Hours";
			// 
			// FormTimeCardRuleEdit
			// 
			this.ClientSize = new System.Drawing.Size(359, 475);
			this.Controls.Add(this.checkUseRate3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkIsOvertimeExempt);
			this.Controls.Add(this.textClockInMin);
			this.Controls.Add(this.textOverHoursPerDay);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.listEmp);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTimeCardRuleEdit";
			this.Text = "Time Card Rule Edit";
			this.Load += new System.EventHandler(this.FormTimeCardRuleEdit_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textOverHoursPerDay;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textAfterTimeOfDay;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.ListBox listEmp;
		private OpenDental.UI.Button but5pm;
		private UI.Button but6am;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBeforeTimeOfDay;
		private OpenDental.UI.GroupBox groupBox2;
		private OpenDental.UI.CheckBox checkIsOvertimeExempt;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textClockInMin;
		private OpenDental.UI.CheckBox checkUseRate3;
	}
}