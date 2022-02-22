namespace OpenDental {
	partial class FormApptsOther {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components=null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ) {
			if(disposing && (components!=null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptsOther));
			this.checkDone = new System.Windows.Forms.CheckBox();
			this.butCancel = new OpenDental.UI.Button();
			this.textApptModNote = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butGoTo = new OpenDental.UI.Button();
			this.butPin = new OpenDental.UI.Button();
			this.butNew = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listViewFamily = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.butOK = new OpenDental.UI.Button();
			this.butRecall = new OpenDental.UI.Button();
			this.textFinUrg = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butNote = new OpenDental.UI.Button();
			this.butRecallFamily = new OpenDental.UI.Button();
			this.odApptGrid = new OpenDental.DashApptGrid();
			this.checkShowCompletePlanned = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkDone
			// 
			this.checkDone.AutoCheck = false;
			this.checkDone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDone.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkDone.Location = new System.Drawing.Point(12, 145);
			this.checkDone.Name = "checkDone";
			this.checkDone.Size = new System.Drawing.Size(168, 16);
			this.checkDone.TabIndex = 1;
			this.checkDone.TabStop = false;
			this.checkDone.Text = "Planned Appt Done";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butCancel.Location = new System.Drawing.Point(837, 620);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textApptModNote
			// 
			this.textApptModNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptModNote.BackColor = System.Drawing.Color.White;
			this.textApptModNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textApptModNote.ForeColor = System.Drawing.Color.Red;
			this.textApptModNote.Location = new System.Drawing.Point(707, 36);
			this.textApptModNote.Multiline = true;
			this.textApptModNote.Name = "textApptModNote";
			this.textApptModNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textApptModNote.Size = new System.Drawing.Size(202, 36);
			this.textApptModNote.TabIndex = 44;
			this.textApptModNote.Leave += new System.EventHandler(this.textApptModNote_Leave);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(542, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(163, 21);
			this.label1.TabIndex = 45;
			this.label1.Text = "Appointment Module Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butGoTo
			// 
			this.butGoTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGoTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGoTo.Location = new System.Drawing.Point(31, 620);
			this.butGoTo.Name = "butGoTo";
			this.butGoTo.Size = new System.Drawing.Size(125, 24);
			this.butGoTo.TabIndex = 46;
			this.butGoTo.Text = "&Go To Appt";
			this.butGoTo.Click += new System.EventHandler(this.butGoTo_Click);
			// 
			// butPin
			// 
			this.butPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPin.Image = global::OpenDental.Properties.Resources.butPin;
			this.butPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPin.Location = new System.Drawing.Point(165, 620);
			this.butPin.Name = "butPin";
			this.butPin.Size = new System.Drawing.Size(134, 24);
			this.butPin.TabIndex = 47;
			this.butPin.Text = "Copy To &Pinboard";
			this.butPin.Click += new System.EventHandler(this.butPin_Click);
			// 
			// butNew
			// 
			this.butNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNew.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(576, 620);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(125, 24);
			this.butNew.TabIndex = 48;
			this.butNew.Text = "Create &New Appt";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 17);
			this.label2.TabIndex = 57;
			this.label2.Text = "Recall for Family";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listViewFamily
			// 
			this.listViewFamily.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3,
            this.columnHeader5});
			this.listViewFamily.FullRowSelect = true;
			this.listViewFamily.GridLines = true;
			this.listViewFamily.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewFamily.HideSelection = false;
			this.listViewFamily.Location = new System.Drawing.Point(12, 36);
			this.listViewFamily.Name = "listViewFamily";
			this.listViewFamily.Size = new System.Drawing.Size(384, 97);
			this.listViewFamily.TabIndex = 58;
			this.listViewFamily.UseCompatibleStateImageBehavior = false;
			this.listViewFamily.View = System.Windows.Forms.View.Details;
			this.listViewFamily.Click += new System.EventHandler(this.listFamily_Click);
			this.listViewFamily.DoubleClick += new System.EventHandler(this.listFamily_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Family Member";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Age";
			this.columnHeader2.Width = 40;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Gender";
			this.columnHeader4.Width = 50;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Due Date";
			this.columnHeader3.Width = 74;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Scheduled";
			this.columnHeader5.Width = 74;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.butOK.Location = new System.Drawing.Point(751, 620);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 59;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butRecall
			// 
			this.butRecall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRecall.Icon = OpenDental.UI.EnumIcons.Recall;
			this.butRecall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRecall.Location = new System.Drawing.Point(308, 620);
			this.butRecall.Name = "butRecall";
			this.butRecall.Size = new System.Drawing.Size(125, 24);
			this.butRecall.TabIndex = 60;
			this.butRecall.Text = "Schedule Recall";
			this.butRecall.Click += new System.EventHandler(this.butRecall_Click);
			// 
			// textFinUrg
			// 
			this.textFinUrg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFinUrg.BackColor = System.Drawing.Color.White;
			this.textFinUrg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textFinUrg.ForeColor = System.Drawing.Color.Red;
			this.textFinUrg.Location = new System.Drawing.Point(707, 78);
			this.textFinUrg.Multiline = true;
			this.textFinUrg.Name = "textFinUrg";
			this.textFinUrg.ReadOnly = true;
			this.textFinUrg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textFinUrg.Size = new System.Drawing.Size(202, 81);
			this.textFinUrg.TabIndex = 63;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label3.Location = new System.Drawing.Point(542, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(163, 21);
			this.label3.TabIndex = 64;
			this.label3.Text = "Family Urgent Financial Notes";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butNote
			// 
			this.butNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNote.Image = ((System.Drawing.Image)(resources.GetObject("butNote.Image")));
			this.butNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNote.Location = new System.Drawing.Point(442, 620);
			this.butNote.Name = "butNote";
			this.butNote.Size = new System.Drawing.Size(125, 24);
			this.butNote.TabIndex = 65;
			this.butNote.Text = "NO&TE for Patient";
			this.butNote.Click += new System.EventHandler(this.butNote_Click);
			// 
			// butRecallFamily
			// 
			this.butRecallFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRecallFamily.Icon = OpenDental.UI.EnumIcons.Recall;
			this.butRecallFamily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRecallFamily.Location = new System.Drawing.Point(308, 588);
			this.butRecallFamily.Name = "butRecallFamily";
			this.butRecallFamily.Size = new System.Drawing.Size(125, 24);
			this.butRecallFamily.TabIndex = 66;
			this.butRecallFamily.Text = "Entire Family";
			this.butRecallFamily.Click += new System.EventHandler(this.butRecallFamily_Click);
			// 
			// odApptGrid
			// 
			this.odApptGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.odApptGrid.Location = new System.Drawing.Point(12, 168);
			this.odApptGrid.Name = "odApptGrid";
			this.odApptGrid.Size = new System.Drawing.Size(897, 398);
			this.odApptGrid.TabIndex = 67;
			// 
			// checkShowCompletePlanned
			// 
			this.checkShowCompletePlanned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowCompletePlanned.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.checkShowCompletePlanned.Location = new System.Drawing.Point(182, 145);
			this.checkShowCompletePlanned.Name = "checkShowCompletePlanned";
			this.checkShowCompletePlanned.Size = new System.Drawing.Size(251, 16);
			this.checkShowCompletePlanned.TabIndex = 68;
			this.checkShowCompletePlanned.TabStop = false;
			this.checkShowCompletePlanned.Text = "Show Completed Planned Appts";
			this.checkShowCompletePlanned.CheckedChanged += new System.EventHandler(this.checkShowCompletePlanned_CheckedChanged);
			// 
			// FormApptsOther
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(924, 658);
			this.Controls.Add(this.checkShowCompletePlanned);
			this.Controls.Add(this.odApptGrid);
			this.Controls.Add(this.butRecallFamily);
			this.Controls.Add(this.butNote);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textFinUrg);
			this.Controls.Add(this.butRecall);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.listViewFamily);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.butPin);
			this.Controls.Add(this.butGoTo);
			this.Controls.Add(this.textApptModNote);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkDone);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptsOther";
			this.ShowInTaskbar = false;
			this.Text = "Other Appointments";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormApptsOther_Closing);
			this.Load += new System.EventHandler(this.FormApptsOther_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.CheckBox checkDone;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textApptModNote;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butGoTo;
		private OpenDental.UI.Button butPin;
		private OpenDental.UI.Button butNew;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView listViewFamily;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private OpenDental.UI.Button butRecall;
		private System.Windows.Forms.TextBox textFinUrg;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butNote;
		private OpenDental.UI.Button butRecallFamily;
		private DashApptGrid odApptGrid;
		private System.Windows.Forms.CheckBox checkShowCompletePlanned;
	}
}
