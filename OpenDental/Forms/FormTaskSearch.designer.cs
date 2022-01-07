namespace OpenDental{
	partial class FormTaskSearch {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskSearch));
			this.groupBox2 = new OpenDental.UI.GroupBoxOD();
			this.groupUsers = new OpenDental.UI.GroupBoxOD();
			this.checkShowHiddenUsers = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboUsers = new OpenDental.UI.ComboBoxOD();
			this.butUserPicker = new OpenDental.UI.Button();
			this.checkBoxIncludeCompleted = new System.Windows.Forms.CheckBox();
			this.checkBoxIncludesTaskNotes = new System.Windows.Forms.CheckBox();
			this.comboPriority = new OpenDental.UI.ComboBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox3 = new OpenDental.UI.GroupBoxOD();
			this.butClearCompleted = new OpenDental.UI.Button();
			this.dateCompletedTo = new System.Windows.Forms.DateTimePicker();
			this.dateCompletedFrom = new System.Windows.Forms.DateTimePicker();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.butClearCreated = new OpenDental.UI.Button();
			this.dateCreatedTo = new System.Windows.Forms.DateTimePicker();
			this.dateCreatedFrom = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butPatPicker = new OpenDental.UI.Button();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textTaskNum = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textTaskList = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkLimit = new System.Windows.Forms.CheckBox();
			this.gridTasks = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butNewTask = new OpenDental.UI.Button();
			this.checkReportServer = new System.Windows.Forms.CheckBox();
			this.groupBox2.SuspendLayout();
			this.groupUsers.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.groupUsers);
			this.groupBox2.Controls.Add(this.checkBoxIncludeCompleted);
			this.groupBox2.Controls.Add(this.checkBoxIncludesTaskNotes);
			this.groupBox2.Controls.Add(this.comboPriority);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.groupBox3);
			this.groupBox2.Controls.Add(this.groupBox1);
			this.groupBox2.Controls.Add(this.butPatPicker);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.textDescription);
			this.groupBox2.Controls.Add(this.textTaskNum);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textTaskList);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(1018, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(210, 411);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.Text = "Search by:";
			// 
			// groupUsers
			// 
			this.groupUsers.Controls.Add(this.checkShowHiddenUsers);
			this.groupUsers.Controls.Add(this.label1);
			this.groupUsers.Controls.Add(this.comboUsers);
			this.groupUsers.Controls.Add(this.butUserPicker);
			this.groupUsers.Location = new System.Drawing.Point(12, 18);
			this.groupUsers.Name = "groupUsers";
			this.groupUsers.Size = new System.Drawing.Size(186, 70);
			this.groupUsers.TabIndex = 0;
			this.groupUsers.Text = "Users";
			// 
			// checkShowHiddenUsers
			// 
			this.checkShowHiddenUsers.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHiddenUsers.Location = new System.Drawing.Point(14, 17);
			this.checkShowHiddenUsers.Name = "checkShowHiddenUsers";
			this.checkShowHiddenUsers.Size = new System.Drawing.Size(159, 18);
			this.checkShowHiddenUsers.TabIndex = 0;
			this.checkShowHiddenUsers.Text = "Show Hidden Users";
			this.checkShowHiddenUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHiddenUsers.UseVisualStyleBackColor = true;
			this.checkShowHiddenUsers.Click += new System.EventHandler(this.checkShowHiddenUsers_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "User";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUsers
			// 
			this.comboUsers.Location = new System.Drawing.Point(67, 41);
			this.comboUsers.Name = "comboUsers";
			this.comboUsers.Size = new System.Drawing.Size(90, 21);
			this.comboUsers.TabIndex = 1;
			// 
			// butUserPicker
			// 
			this.butUserPicker.Location = new System.Drawing.Point(159, 41);
			this.butUserPicker.Name = "butUserPicker";
			this.butUserPicker.Size = new System.Drawing.Size(20, 20);
			this.butUserPicker.TabIndex = 2;
			this.butUserPicker.Text = "...";
			this.butUserPicker.UseVisualStyleBackColor = true;
			this.butUserPicker.Click += new System.EventHandler(this.butUserPicker_Click);
			// 
			// checkBoxIncludeCompleted
			// 
			this.checkBoxIncludeCompleted.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxIncludeCompleted.Checked = true;
			this.checkBoxIncludeCompleted.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxIncludeCompleted.Location = new System.Drawing.Point(19, 389);
			this.checkBoxIncludeCompleted.Name = "checkBoxIncludeCompleted";
			this.checkBoxIncludeCompleted.Size = new System.Drawing.Size(179, 17);
			this.checkBoxIncludeCompleted.TabIndex = 10;
			this.checkBoxIncludeCompleted.Text = "Include Completed";
			this.checkBoxIncludeCompleted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxIncludeCompleted.UseVisualStyleBackColor = true;
			// 
			// checkBoxIncludesTaskNotes
			// 
			this.checkBoxIncludesTaskNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxIncludesTaskNotes.Checked = true;
			this.checkBoxIncludesTaskNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxIncludesTaskNotes.Location = new System.Drawing.Point(19, 368);
			this.checkBoxIncludesTaskNotes.Name = "checkBoxIncludesTaskNotes";
			this.checkBoxIncludesTaskNotes.Size = new System.Drawing.Size(179, 17);
			this.checkBoxIncludesTaskNotes.TabIndex = 9;
			this.checkBoxIncludesTaskNotes.Text = "Include Task Notes";
			this.checkBoxIncludesTaskNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxIncludesTaskNotes.UseVisualStyleBackColor = true;
			// 
			// comboPriority
			// 
			this.comboPriority.Location = new System.Drawing.Point(90, 172);
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(90, 21);
			this.comboPriority.TabIndex = 6;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(14, 174);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(74, 17);
			this.label12.TabIndex = 33;
			this.label12.Text = "Priority";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.butClearCompleted);
			this.groupBox3.Controls.Add(this.dateCompletedTo);
			this.groupBox3.Controls.Add(this.dateCompletedFrom);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Location = new System.Drawing.Point(12, 278);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(186, 84);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.Text = "Date Completed";
			// 
			// butClearCompleted
			// 
			this.butClearCompleted.Location = new System.Drawing.Point(112, 58);
			this.butClearCompleted.Name = "butClearCompleted";
			this.butClearCompleted.Size = new System.Drawing.Size(56, 20);
			this.butClearCompleted.TabIndex = 2;
			this.butClearCompleted.Text = "Clear";
			this.butClearCompleted.UseVisualStyleBackColor = true;
			this.butClearCompleted.Click += new System.EventHandler(this.butClearCompleted_Click);
			// 
			// dateCompletedTo
			// 
			this.dateCompletedTo.CustomFormat = " ";
			this.dateCompletedTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateCompletedTo.Location = new System.Drawing.Point(67, 36);
			this.dateCompletedTo.Name = "dateCompletedTo";
			this.dateCompletedTo.Size = new System.Drawing.Size(101, 20);
			this.dateCompletedTo.TabIndex = 1;
			this.dateCompletedTo.ValueChanged += new System.EventHandler(this.dateCompletedTo_ValueChanged);
			// 
			// dateCompletedFrom
			// 
			this.dateCompletedFrom.CustomFormat = " ";
			this.dateCompletedFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateCompletedFrom.Location = new System.Drawing.Point(67, 16);
			this.dateCompletedFrom.Name = "dateCompletedFrom";
			this.dateCompletedFrom.Size = new System.Drawing.Size(101, 20);
			this.dateCompletedFrom.TabIndex = 0;
			this.dateCompletedFrom.ValueChanged += new System.EventHandler(this.dateCompletedFrom_ValueChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(34, 38);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(32, 15);
			this.label6.TabIndex = 5;
			this.label6.Text = "To";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(34, 18);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 15);
			this.label7.TabIndex = 4;
			this.label7.Text = "From";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butClearCreated);
			this.groupBox1.Controls.Add(this.dateCreatedTo);
			this.groupBox1.Controls.Add(this.dateCreatedFrom);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 194);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(186, 84);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.Text = "Date Created";
			// 
			// butClearCreated
			// 
			this.butClearCreated.Location = new System.Drawing.Point(112, 58);
			this.butClearCreated.Name = "butClearCreated";
			this.butClearCreated.Size = new System.Drawing.Size(56, 20);
			this.butClearCreated.TabIndex = 2;
			this.butClearCreated.Text = "Clear";
			this.butClearCreated.UseVisualStyleBackColor = true;
			this.butClearCreated.Click += new System.EventHandler(this.butClearCreated_Click);
			// 
			// dateCreatedTo
			// 
			this.dateCreatedTo.CustomFormat = " ";
			this.dateCreatedTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateCreatedTo.Location = new System.Drawing.Point(67, 36);
			this.dateCreatedTo.Name = "dateCreatedTo";
			this.dateCreatedTo.Size = new System.Drawing.Size(101, 20);
			this.dateCreatedTo.TabIndex = 1;
			this.dateCreatedTo.ValueChanged += new System.EventHandler(this.dateCreatedTo_ValueChanged);
			// 
			// dateCreatedFrom
			// 
			this.dateCreatedFrom.CustomFormat = " ";
			this.dateCreatedFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateCreatedFrom.Location = new System.Drawing.Point(67, 16);
			this.dateCreatedFrom.Name = "dateCreatedFrom";
			this.dateCreatedFrom.Size = new System.Drawing.Size(101, 20);
			this.dateCreatedFrom.TabIndex = 0;
			this.dateCreatedFrom.ValueChanged += new System.EventHandler(this.dateCreatedFrom_ValueChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(34, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(32, 15);
			this.label5.TabIndex = 1;
			this.label5.Text = "To";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(34, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "From";
			// 
			// butPatPicker
			// 
			this.butPatPicker.Location = new System.Drawing.Point(182, 152);
			this.butPatPicker.Name = "butPatPicker";
			this.butPatPicker.Size = new System.Drawing.Size(20, 20);
			this.butPatPicker.TabIndex = 5;
			this.butPatPicker.Text = "...";
			this.butPatPicker.UseVisualStyleBackColor = true;
			this.butPatPicker.Click += new System.EventHandler(this.butPatPicker_Click);
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(90, 152);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(90, 20);
			this.textPatNum.TabIndex = 4;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 135);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(73, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "Description";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(14, 155);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(75, 12);
			this.label9.TabIndex = 18;
			this.label9.Text = "PatNum";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(90, 132);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(90, 20);
			this.textDescription.TabIndex = 3;
			// 
			// textTaskNum
			// 
			this.textTaskNum.Location = new System.Drawing.Point(90, 112);
			this.textTaskNum.Name = "textTaskNum";
			this.textTaskNum.Size = new System.Drawing.Size(90, 20);
			this.textTaskNum.TabIndex = 2;
			this.textTaskNum.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textTaskNum_KeyUp);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "Task Num";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTaskList
			// 
			this.textTaskList.Location = new System.Drawing.Point(90, 92);
			this.textTaskList.Name = "textTaskList";
			this.textTaskList.Size = new System.Drawing.Size(90, 20);
			this.textTaskList.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 95);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "Task List";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkLimit
			// 
			this.checkLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkLimit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLimit.Location = new System.Drawing.Point(1083, 503);
			this.checkLimit.Name = "checkLimit";
			this.checkLimit.Size = new System.Drawing.Size(133, 18);
			this.checkLimit.TabIndex = 4;
			this.checkLimit.Text = "Limit Results (50)";
			this.checkLimit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLimit.UseVisualStyleBackColor = true;
			// 
			// gridTasks
			// 
			this.gridTasks.AllowSortingByColumn = true;
			this.gridTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridTasks.Location = new System.Drawing.Point(12, 12);
			this.gridTasks.Name = "gridTasks";
			this.gridTasks.NoteSpanStart = 2;
			this.gridTasks.NoteSpanStop = 2;
			this.gridTasks.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridTasks.Size = new System.Drawing.Size(1000, 672);
			this.gridTasks.TabIndex = 0;
			this.gridTasks.Title = "Task Results";
			this.gridTasks.TranslationName = "TableProg";
			this.gridTasks.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellDoubleClick);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(1145, 428);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1143, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 6;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butNewTask
			// 
			this.butNewTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butNewTask.Location = new System.Drawing.Point(1018, 428);
			this.butNewTask.Name = "butNewTask";
			this.butNewTask.Size = new System.Drawing.Size(75, 24);
			this.butNewTask.TabIndex = 2;
			this.butNewTask.Text = "&New Task";
			this.butNewTask.Click += new System.EventHandler(this.butNewTask_Click);
			// 
			// checkReportServer
			// 
			this.checkReportServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkReportServer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReportServer.Location = new System.Drawing.Point(1059, 524);
			this.checkReportServer.Name = "checkReportServer";
			this.checkReportServer.Size = new System.Drawing.Size(157, 18);
			this.checkReportServer.TabIndex = 5;
			this.checkReportServer.Text = "Run on Report Server";
			this.checkReportServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReportServer.UseVisualStyleBackColor = true;
			// 
			// FormTaskSearch
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.checkReportServer);
			this.Controls.Add(this.butNewTask);
			this.Controls.Add(this.checkLimit);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.gridTasks);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskSearch";
			this.Text = "Task Search";
			this.Load += new System.EventHandler(this.FormTaskSearch_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupUsers.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridTasks;
		private UI.Button butRefresh;
		private OpenDental.UI.GroupBoxOD groupBox2;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textTaskNum;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textTaskList;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkLimit;
		private UI.Button butPatPicker;
		private OpenDental.UI.ComboBoxOD comboUsers;
		private UI.Button butUserPicker;
		private OpenDental.UI.ComboBoxOD comboPriority;
		private System.Windows.Forms.Label label12;
		private OpenDental.UI.GroupBoxOD groupBox3;
		private OpenDental.UI.GroupBoxOD groupBox1;
		private System.Windows.Forms.DateTimePicker dateCreatedFrom;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dateCompletedTo;
		private System.Windows.Forms.DateTimePicker dateCompletedFrom;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.DateTimePicker dateCreatedTo;
		private UI.Button butClearCompleted;
		private UI.Button butClearCreated;
		private UI.Button butNewTask;
		private System.Windows.Forms.CheckBox checkBoxIncludeCompleted;
		private System.Windows.Forms.CheckBox checkBoxIncludesTaskNotes;
		private System.Windows.Forms.CheckBox checkReportServer;
		private System.Windows.Forms.CheckBox checkShowHiddenUsers;
		private UI.GroupBoxOD groupUsers;
	}
}