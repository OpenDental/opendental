namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizSchedule {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlSetupWizSchedule));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.butPrevProv = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butNextProv = new OpenDental.UI.Button();
			this.checkSaturday = new System.Windows.Forms.CheckBox();
			this.labelProv = new System.Windows.Forms.Label();
			this.checkWednesday = new System.Windows.Forms.CheckBox();
			this.checkThursday = new System.Windows.Forms.CheckBox();
			this.checkFriday = new System.Windows.Forms.CheckBox();
			this.checkTuesday = new System.Windows.Forms.CheckBox();
			this.checkSunday = new System.Windows.Forms.CheckBox();
			this.groupMon = new System.Windows.Forms.GroupBox();
			this.gridMonday = new OpenDental.UI.GridOD();
			this.butAddMonday = new OpenDental.UI.Button();
			this.checkMonday = new System.Windows.Forms.CheckBox();
			this.groupSun = new System.Windows.Forms.GroupBox();
			this.gridSunday = new OpenDental.UI.GridOD();
			this.butAddSunday = new OpenDental.UI.Button();
			this.groupSat = new System.Windows.Forms.GroupBox();
			this.gridSaturday = new OpenDental.UI.GridOD();
			this.butAddSaturday = new OpenDental.UI.Button();
			this.groupFri = new System.Windows.Forms.GroupBox();
			this.gridFriday = new OpenDental.UI.GridOD();
			this.butAddFriday = new OpenDental.UI.Button();
			this.groupThu = new System.Windows.Forms.GroupBox();
			this.gridThursday = new OpenDental.UI.GridOD();
			this.butAddThursday = new OpenDental.UI.Button();
			this.groupWed = new System.Windows.Forms.GroupBox();
			this.gridWednesday = new OpenDental.UI.GridOD();
			this.butAddWednesday = new OpenDental.UI.Button();
			this.groupTue = new System.Windows.Forms.GroupBox();
			this.gridTuesday = new OpenDental.UI.GridOD();
			this.butAddTuesday = new OpenDental.UI.Button();
			this.groupMon.SuspendLayout();
			this.groupSun.SuspendLayout();
			this.groupSat.SuspendLayout();
			this.groupFri.SuspendLayout();
			this.groupThu.SuspendLayout();
			this.groupWed.SuspendLayout();
			this.groupTue.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "iButton_Blue.png");
			// 
			// butPrevProv
			// 
			this.butPrevProv.Location = new System.Drawing.Point(774, 8);
			this.butPrevProv.Name = "butPrevProv";
			this.butPrevProv.Size = new System.Drawing.Size(75, 19);
			this.butPrevProv.TabIndex = 105;
			this.butPrevProv.Text = "Prev Prov";
			this.butPrevProv.UseVisualStyleBackColor = true;
			this.butPrevProv.Click += new System.EventHandler(this.butPrevProv_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(432, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(342, 20);
			this.label2.TabIndex = 104;
			this.label2.Text = "When you are done with this provider, click \'Next Prov\'.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNextProv
			// 
			this.butNextProv.Location = new System.Drawing.Point(851, 8);
			this.butNextProv.Name = "butNextProv";
			this.butNextProv.Size = new System.Drawing.Size(75, 19);
			this.butNextProv.TabIndex = 103;
			this.butNextProv.Text = "Next Prov";
			this.butNextProv.UseVisualStyleBackColor = true;
			this.butNextProv.Click += new System.EventHandler(this.butNextProv_Click);
			// 
			// checkSaturday
			// 
			this.checkSaturday.Location = new System.Drawing.Point(368, 280);
			this.checkSaturday.Name = "checkSaturday";
			this.checkSaturday.Size = new System.Drawing.Size(150, 20);
			this.checkSaturday.TabIndex = 99;
			this.checkSaturday.Text = "Saturday";
			this.checkSaturday.UseVisualStyleBackColor = true;
			this.checkSaturday.Click += new System.EventHandler(this.checkSaturday_CheckedChanged);
			// 
			// labelProv
			// 
			this.labelProv.Location = new System.Drawing.Point(3, 7);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(243, 20);
			this.labelProv.TabIndex = 76;
			this.labelProv.Text = "Let\'s set up schedules for ";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkWednesday
			// 
			this.checkWednesday.Location = new System.Drawing.Point(476, 58);
			this.checkWednesday.Name = "checkWednesday";
			this.checkWednesday.Size = new System.Drawing.Size(150, 20);
			this.checkWednesday.TabIndex = 60;
			this.checkWednesday.Text = "Wednesday";
			this.checkWednesday.UseVisualStyleBackColor = true;
			this.checkWednesday.Click += new System.EventHandler(this.checkWednesday_CheckedChanged);
			// 
			// checkThursday
			// 
			this.checkThursday.Location = new System.Drawing.Point(709, 58);
			this.checkThursday.Name = "checkThursday";
			this.checkThursday.Size = new System.Drawing.Size(150, 20);
			this.checkThursday.TabIndex = 59;
			this.checkThursday.Text = "Thursday";
			this.checkThursday.UseVisualStyleBackColor = true;
			this.checkThursday.Click += new System.EventHandler(this.checkThursday_CheckedChanged);
			// 
			// checkFriday
			// 
			this.checkFriday.Location = new System.Drawing.Point(109, 280);
			this.checkFriday.Name = "checkFriday";
			this.checkFriday.Size = new System.Drawing.Size(150, 20);
			this.checkFriday.TabIndex = 58;
			this.checkFriday.Text = "Friday";
			this.checkFriday.UseVisualStyleBackColor = true;
			this.checkFriday.Click += new System.EventHandler(this.checkFriday_CheckedChanged);
			// 
			// checkTuesday
			// 
			this.checkTuesday.Location = new System.Drawing.Point(243, 58);
			this.checkTuesday.Name = "checkTuesday";
			this.checkTuesday.Size = new System.Drawing.Size(150, 20);
			this.checkTuesday.TabIndex = 57;
			this.checkTuesday.Text = "Tuesday";
			this.checkTuesday.UseVisualStyleBackColor = true;
			this.checkTuesday.Click += new System.EventHandler(this.checkTuesday_CheckedChanged);
			// 
			// checkSunday
			// 
			this.checkSunday.Location = new System.Drawing.Point(628, 280);
			this.checkSunday.Name = "checkSunday";
			this.checkSunday.Size = new System.Drawing.Size(150, 20);
			this.checkSunday.TabIndex = 55;
			this.checkSunday.Text = "Sunday";
			this.checkSunday.UseVisualStyleBackColor = true;
			this.checkSunday.Click += new System.EventHandler(this.checkSunday_CheckedChanged);
			// 
			// groupMon
			// 
			this.groupMon.Controls.Add(this.gridMonday);
			this.groupMon.Controls.Add(this.butAddMonday);
			this.groupMon.Location = new System.Drawing.Point(4, 58);
			this.groupMon.Name = "groupMon";
			this.groupMon.Size = new System.Drawing.Size(222, 211);
			this.groupMon.TabIndex = 106;
			this.groupMon.TabStop = false;
			this.groupMon.Visible = false;
			// 
			// gridMonday
			// 
			this.gridMonday.Location = new System.Drawing.Point(7, 22);
			this.gridMonday.Name = "gridMonday";
			this.gridMonday.Size = new System.Drawing.Size(210, 160);
			this.gridMonday.TabIndex = 70;
			this.gridMonday.Title = null;
			this.gridMonday.TranslationName = "TableMonday";
			// 
			// butAddMonday
			// 
			this.butAddMonday.Location = new System.Drawing.Point(7, 183);
			this.butAddMonday.Name = "butAddMonday";
			this.butAddMonday.Size = new System.Drawing.Size(75, 23);
			this.butAddMonday.TabIndex = 94;
			this.butAddMonday.Text = "Add";
			this.butAddMonday.UseVisualStyleBackColor = true;
			this.butAddMonday.Click += new System.EventHandler(this.butAddMonday_Click);
			// 
			// checkMonday
			// 
			this.checkMonday.Location = new System.Drawing.Point(10, 59);
			this.checkMonday.Name = "checkMonday";
			this.checkMonday.Size = new System.Drawing.Size(150, 20);
			this.checkMonday.TabIndex = 56;
			this.checkMonday.Text = "Monday";
			this.checkMonday.UseVisualStyleBackColor = true;
			this.checkMonday.Click += new System.EventHandler(this.checkMonday_CheckedChanged);
			// 
			// groupSun
			// 
			this.groupSun.Controls.Add(this.gridSunday);
			this.groupSun.Controls.Add(this.butAddSunday);
			this.groupSun.Location = new System.Drawing.Point(622, 280);
			this.groupSun.Name = "groupSun";
			this.groupSun.Size = new System.Drawing.Size(222, 211);
			this.groupSun.TabIndex = 110;
			this.groupSun.TabStop = false;
			this.groupSun.Visible = false;
			// 
			// gridSunday
			// 
			this.gridSunday.Location = new System.Drawing.Point(7, 22);
			this.gridSunday.Name = "gridSunday";
			this.gridSunday.Size = new System.Drawing.Size(210, 160);
			this.gridSunday.TabIndex = 69;
			this.gridSunday.Title = null;
			this.gridSunday.TranslationName = "TableSunday";
			// 
			// butAddSunday
			// 
			this.butAddSunday.Location = new System.Drawing.Point(7, 183);
			this.butAddSunday.Name = "butAddSunday";
			this.butAddSunday.Size = new System.Drawing.Size(75, 23);
			this.butAddSunday.TabIndex = 93;
			this.butAddSunday.Text = "Add";
			this.butAddSunday.UseVisualStyleBackColor = true;
			this.butAddSunday.Click += new System.EventHandler(this.butAddSunday_Click);
			// 
			// groupSat
			// 
			this.groupSat.Controls.Add(this.gridSaturday);
			this.groupSat.Controls.Add(this.butAddSaturday);
			this.groupSat.Location = new System.Drawing.Point(362, 280);
			this.groupSat.Name = "groupSat";
			this.groupSat.Size = new System.Drawing.Size(222, 211);
			this.groupSat.TabIndex = 110;
			this.groupSat.TabStop = false;
			this.groupSat.Visible = false;
			// 
			// gridSaturday
			// 
			this.gridSaturday.Location = new System.Drawing.Point(7, 22);
			this.gridSaturday.Name = "gridSaturday";
			this.gridSaturday.Size = new System.Drawing.Size(210, 160);
			this.gridSaturday.TabIndex = 100;
			this.gridSaturday.Title = null;
			this.gridSaturday.TranslationName = "TableSaturday";
			// 
			// butAddSaturday
			// 
			this.butAddSaturday.Location = new System.Drawing.Point(7, 183);
			this.butAddSaturday.Name = "butAddSaturday";
			this.butAddSaturday.Size = new System.Drawing.Size(75, 23);
			this.butAddSaturday.TabIndex = 101;
			this.butAddSaturday.Text = "Add";
			this.butAddSaturday.UseVisualStyleBackColor = true;
			this.butAddSaturday.Click += new System.EventHandler(this.butAddSaturday_Click);
			// 
			// groupFri
			// 
			this.groupFri.Controls.Add(this.gridFriday);
			this.groupFri.Controls.Add(this.butAddFriday);
			this.groupFri.Location = new System.Drawing.Point(102, 280);
			this.groupFri.Name = "groupFri";
			this.groupFri.Size = new System.Drawing.Size(222, 211);
			this.groupFri.TabIndex = 110;
			this.groupFri.TabStop = false;
			this.groupFri.Visible = false;
			// 
			// gridFriday
			// 
			this.gridFriday.Location = new System.Drawing.Point(7, 22);
			this.gridFriday.Name = "gridFriday";
			this.gridFriday.Size = new System.Drawing.Size(210, 160);
			this.gridFriday.TabIndex = 74;
			this.gridFriday.Title = null;
			this.gridFriday.TranslationName = "TableFriday";
			// 
			// butAddFriday
			// 
			this.butAddFriday.Location = new System.Drawing.Point(7, 183);
			this.butAddFriday.Name = "butAddFriday";
			this.butAddFriday.Size = new System.Drawing.Size(75, 23);
			this.butAddFriday.TabIndex = 98;
			this.butAddFriday.Text = "Add";
			this.butAddFriday.UseVisualStyleBackColor = true;
			this.butAddFriday.Click += new System.EventHandler(this.butAddFriday_Click);
			// 
			// groupThu
			// 
			this.groupThu.Controls.Add(this.gridThursday);
			this.groupThu.Controls.Add(this.butAddThursday);
			this.groupThu.Location = new System.Drawing.Point(703, 58);
			this.groupThu.Name = "groupThu";
			this.groupThu.Size = new System.Drawing.Size(222, 211);
			this.groupThu.TabIndex = 109;
			this.groupThu.TabStop = false;
			this.groupThu.Visible = false;
			// 
			// gridThursday
			// 
			this.gridThursday.Location = new System.Drawing.Point(7, 22);
			this.gridThursday.Name = "gridThursday";
			this.gridThursday.Size = new System.Drawing.Size(210, 160);
			this.gridThursday.TabIndex = 73;
			this.gridThursday.Title = null;
			this.gridThursday.TranslationName = "TableThursday";
			// 
			// butAddThursday
			// 
			this.butAddThursday.Location = new System.Drawing.Point(7, 183);
			this.butAddThursday.Name = "butAddThursday";
			this.butAddThursday.Size = new System.Drawing.Size(75, 23);
			this.butAddThursday.TabIndex = 97;
			this.butAddThursday.Text = "Add";
			this.butAddThursday.UseVisualStyleBackColor = true;
			this.butAddThursday.Click += new System.EventHandler(this.butAddThursday_Click);
			// 
			// groupWed
			// 
			this.groupWed.Controls.Add(this.gridWednesday);
			this.groupWed.Controls.Add(this.butAddWednesday);
			this.groupWed.Location = new System.Drawing.Point(470, 58);
			this.groupWed.Name = "groupWed";
			this.groupWed.Size = new System.Drawing.Size(222, 211);
			this.groupWed.TabIndex = 108;
			this.groupWed.TabStop = false;
			this.groupWed.Visible = false;
			// 
			// gridWednesday
			// 
			this.gridWednesday.Location = new System.Drawing.Point(7, 22);
			this.gridWednesday.Name = "gridWednesday";
			this.gridWednesday.Size = new System.Drawing.Size(210, 160);
			this.gridWednesday.TabIndex = 72;
			this.gridWednesday.Title = null;
			this.gridWednesday.TranslationName = "TableWednesday";
			// 
			// butAddWednesday
			// 
			this.butAddWednesday.Location = new System.Drawing.Point(7, 183);
			this.butAddWednesday.Name = "butAddWednesday";
			this.butAddWednesday.Size = new System.Drawing.Size(75, 23);
			this.butAddWednesday.TabIndex = 96;
			this.butAddWednesday.Text = "Add";
			this.butAddWednesday.UseVisualStyleBackColor = true;
			this.butAddWednesday.Click += new System.EventHandler(this.butAddWednesday_Click);
			// 
			// groupTue
			// 
			this.groupTue.Controls.Add(this.gridTuesday);
			this.groupTue.Controls.Add(this.butAddTuesday);
			this.groupTue.Location = new System.Drawing.Point(237, 58);
			this.groupTue.Name = "groupTue";
			this.groupTue.Size = new System.Drawing.Size(222, 211);
			this.groupTue.TabIndex = 107;
			this.groupTue.TabStop = false;
			this.groupTue.Visible = false;
			// 
			// gridTuesday
			// 
			this.gridTuesday.Location = new System.Drawing.Point(7, 22);
			this.gridTuesday.Name = "gridTuesday";
			this.gridTuesday.Size = new System.Drawing.Size(210, 160);
			this.gridTuesday.TabIndex = 71;
			this.gridTuesday.Title = null;
			this.gridTuesday.TranslationName = "TableTuesday";
			// 
			// butAddTuesday
			// 
			this.butAddTuesday.Location = new System.Drawing.Point(7, 183);
			this.butAddTuesday.Name = "butAddTuesday";
			this.butAddTuesday.Size = new System.Drawing.Size(75, 23);
			this.butAddTuesday.TabIndex = 95;
			this.butAddTuesday.Text = "Add";
			this.butAddTuesday.UseVisualStyleBackColor = true;
			this.butAddTuesday.Click += new System.EventHandler(this.butAddTuesday_Click);
			// 
			// UserControlSetupWizSchedule
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butPrevProv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkMonday);
			this.Controls.Add(this.butNextProv);
			this.Controls.Add(this.checkSaturday);
			this.Controls.Add(this.labelProv);
			this.Controls.Add(this.checkWednesday);
			this.Controls.Add(this.checkThursday);
			this.Controls.Add(this.checkFriday);
			this.Controls.Add(this.checkTuesday);
			this.Controls.Add(this.checkSunday);
			this.Controls.Add(this.groupMon);
			this.Controls.Add(this.groupSun);
			this.Controls.Add(this.groupSat);
			this.Controls.Add(this.groupFri);
			this.Controls.Add(this.groupThu);
			this.Controls.Add(this.groupWed);
			this.Controls.Add(this.groupTue);
			this.Name = "UserControlSetupWizSchedule";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizClinic_Load);
			this.groupMon.ResumeLayout(false);
			this.groupSun.ResumeLayout(false);
			this.groupSat.ResumeLayout(false);
			this.groupFri.ResumeLayout(false);
			this.groupThu.ResumeLayout(false);
			this.groupWed.ResumeLayout(false);
			this.groupTue.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox checkSunday;
		private System.Windows.Forms.CheckBox checkMonday;
		private System.Windows.Forms.CheckBox checkTuesday;
		private System.Windows.Forms.CheckBox checkFriday;
		private System.Windows.Forms.CheckBox checkThursday;
		private System.Windows.Forms.CheckBox checkWednesday;
		private UI.GridOD gridSunday;
		private UI.GridOD gridMonday;
		private UI.GridOD gridTuesday;
		private UI.GridOD gridWednesday;
		private UI.GridOD gridThursday;
		private UI.GridOD gridFriday;
		private System.Windows.Forms.ImageList imageList1;
		private UI.Button butAddSunday;
		private UI.Button butAddMonday;
		private UI.Button butAddTuesday;
		private UI.Button butAddWednesday;
		private UI.Button butAddThursday;
		private UI.Button butAddFriday;
		private UI.Button butAddSaturday;
		private UI.GridOD gridSaturday;
		private System.Windows.Forms.CheckBox checkSaturday;
		private System.Windows.Forms.Label labelProv;
		private UI.Button butNextProv;
		private System.Windows.Forms.Label label2;
		private UI.Button butPrevProv;
		private System.Windows.Forms.GroupBox groupMon;
		private System.Windows.Forms.GroupBox groupTue;
		private System.Windows.Forms.GroupBox groupWed;
		private System.Windows.Forms.GroupBox groupThu;
		private System.Windows.Forms.GroupBox groupFri;
		private System.Windows.Forms.GroupBox groupSat;
		private System.Windows.Forms.GroupBox groupSun;
	}
}
