namespace OpenDental{
	partial class FormTaskOptions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskOptions));
			this.butSave = new OpenDental.UI.Button();
			this.checkShowFinished = new OpenDental.UI.CheckBox();
			this.textStartDate = new OpenDental.ValidDate();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.checkTaskSortApptDateTime = new OpenDental.UI.CheckBox();
			this.checkCollapsed = new OpenDental.UI.CheckBox();
			this.checkShowArchivedTaskLists = new OpenDental.UI.CheckBox();
			this.checkBlockedTaskPlaySound = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(237, 151);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkShowFinished
			// 
			this.checkShowFinished.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkShowFinished.Location = new System.Drawing.Point(12, 12);
			this.checkShowFinished.Name = "checkShowFinished";
			this.checkShowFinished.Size = new System.Drawing.Size(151, 17);
			this.checkShowFinished.TabIndex = 11;
			this.checkShowFinished.Text = "Show Finished Tasks";
			this.checkShowFinished.Click += new System.EventHandler(this.checkShowFinished_Click);
			// 
			// textStartDate
			// 
			this.textStartDate.Location = new System.Drawing.Point(192, 29);
			this.textStartDate.Name = "textStartDate";
			this.textStartDate.Size = new System.Drawing.Size(87, 20);
			this.textStartDate.TabIndex = 13;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(38, 30);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(153, 17);
			this.labelStartDate.TabIndex = 14;
			this.labelStartDate.Text = "Finished Task Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTaskSortApptDateTime
			// 
			this.checkTaskSortApptDateTime.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkTaskSortApptDateTime.Location = new System.Drawing.Point(12, 55);
			this.checkTaskSortApptDateTime.Name = "checkTaskSortApptDateTime";
			this.checkTaskSortApptDateTime.Size = new System.Drawing.Size(299, 17);
			this.checkTaskSortApptDateTime.TabIndex = 15;
			this.checkTaskSortApptDateTime.Text = "Sort appointment type task lists by AptDateTime";
			// 
			// checkCollapsed
			// 
			this.checkCollapsed.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkCollapsed.Location = new System.Drawing.Point(12, 78);
			this.checkCollapsed.Name = "checkCollapsed";
			this.checkCollapsed.Size = new System.Drawing.Size(299, 17);
			this.checkCollapsed.TabIndex = 16;
			this.checkCollapsed.Text = "Default tasks to collapsed state";
			// 
			// checkShowArchivedTaskLists
			// 
			this.checkShowArchivedTaskLists.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkShowArchivedTaskLists.Location = new System.Drawing.Point(12, 101);
			this.checkShowArchivedTaskLists.Name = "checkShowArchivedTaskLists";
			this.checkShowArchivedTaskLists.Size = new System.Drawing.Size(200, 17);
			this.checkShowArchivedTaskLists.TabIndex = 17;
			this.checkShowArchivedTaskLists.Text = "Show Archived Task Lists";
			// 
			// checkBlockedTaskPlaySound
			// 
			this.checkBlockedTaskPlaySound.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBlockedTaskPlaySound.Location = new System.Drawing.Point(12, 124);
			this.checkBlockedTaskPlaySound.Name = "checkBlockedTaskPlaySound";
			this.checkBlockedTaskPlaySound.Size = new System.Drawing.Size(328, 17);
			this.checkBlockedTaskPlaySound.TabIndex = 19;
			this.checkBlockedTaskPlaySound.Text = "Blocked task popups still make notification sound";
			// 
			// FormTaskOptions
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(331, 191);
			this.Controls.Add(this.checkBlockedTaskPlaySound);
			this.Controls.Add(this.checkShowArchivedTaskLists);
			this.Controls.Add(this.checkCollapsed);
			this.Controls.Add(this.checkTaskSortApptDateTime);
			this.Controls.Add(this.textStartDate);
			this.Controls.Add(this.labelStartDate);
			this.Controls.Add(this.checkShowFinished);
			this.Controls.Add(this.butSave);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskOptions";
			this.Text = "Task Options";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkShowFinished;
		private ValidDate textStartDate;
		private System.Windows.Forms.Label labelStartDate;
		private OpenDental.UI.CheckBox checkTaskSortApptDateTime;
		private OpenDental.UI.CheckBox checkCollapsed;
		private OpenDental.UI.CheckBox checkShowArchivedTaskLists;
		private OpenDental.UI.CheckBox checkBlockedTaskPlaySound;
	}
}