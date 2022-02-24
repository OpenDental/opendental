namespace OpenDental.UI {
	partial class BugSubmissionControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.labelLastCall = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.labelSubNum = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.labelCustomerNum = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.labelCustomerPhone = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.labelCustomerState = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.labelRegKey = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butCompare = new OpenDental.UI.Button();
			this.butBugTask = new OpenDental.UI.Button();
			this.butGoToAccount = new OpenDental.UI.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabStackTrace = new System.Windows.Forms.TabPage();
			this.textStack = new OpenDental.ODtextBox();
			this.tabOdStackSig = new System.Windows.Forms.TabPage();
			this.textODStack = new OpenDental.ODtextBox();
			this.tabSimpleStack = new System.Windows.Forms.TabPage();
			this.textStrippedStack = new OpenDental.ODtextBox();
			this.tabCategoryTags = new System.Windows.Forms.TabPage();
			this.butDeleteCategory = new OpenDental.UI.Button();
			this.butAddCategory = new OpenDental.UI.Button();
			this.listBoxCategories = new OpenDental.UI.ListBoxOD();
			this.textDevNote = new OpenDental.ODtextBox();
			this.gridCustomerSubs = new OpenDental.UI.GridOD();
			this.gridOfficeInfo = new OpenDental.UI.GridOD();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabStackTrace.SuspendLayout();
			this.tabOdStackSig.SuspendLayout();
			this.tabSimpleStack.SuspendLayout();
			this.tabCategoryTags.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelLastCall
			// 
			this.labelLastCall.Location = new System.Drawing.Point(78, 106);
			this.labelLastCall.Name = "labelLastCall";
			this.labelLastCall.Size = new System.Drawing.Size(215, 13);
			this.labelLastCall.TabIndex = 29;
			this.labelLastCall.Text = "XXXXX";
			this.labelLastCall.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(7, 106);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(65, 13);
			this.label12.TabIndex = 28;
			this.label12.Text = "Last Call:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSubNum
			// 
			this.labelSubNum.Location = new System.Drawing.Point(78, 88);
			this.labelSubNum.Name = "labelSubNum";
			this.labelSubNum.Size = new System.Drawing.Size(215, 13);
			this.labelSubNum.TabIndex = 27;
			this.labelSubNum.Text = "XXXXX";
			this.labelSubNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(7, 88);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(65, 13);
			this.label10.TabIndex = 26;
			this.label10.Text = "Sub Num:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCustomerNum
			// 
			this.labelCustomerNum.Location = new System.Drawing.Point(78, 16);
			this.labelCustomerNum.Name = "labelCustomerNum";
			this.labelCustomerNum.Size = new System.Drawing.Size(215, 13);
			this.labelCustomerNum.TabIndex = 25;
			this.labelCustomerNum.Text = "XXXXX";
			this.labelCustomerNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(7, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(65, 13);
			this.label11.TabIndex = 24;
			this.label11.Text = "Pat Num:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCustomerPhone
			// 
			this.labelCustomerPhone.Location = new System.Drawing.Point(78, 70);
			this.labelCustomerPhone.Name = "labelCustomerPhone";
			this.labelCustomerPhone.Size = new System.Drawing.Size(215, 13);
			this.labelCustomerPhone.TabIndex = 23;
			this.labelCustomerPhone.Text = "(555)555-5555";
			this.labelCustomerPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(7, 70);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 13);
			this.label9.TabIndex = 22;
			this.label9.Text = "Work Phone:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCustomerState
			// 
			this.labelCustomerState.Location = new System.Drawing.Point(78, 52);
			this.labelCustomerState.Name = "labelCustomerState";
			this.labelCustomerState.Size = new System.Drawing.Size(215, 13);
			this.labelCustomerState.TabIndex = 21;
			this.labelCustomerState.Text = "OR";
			this.labelCustomerState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(7, 52);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 13);
			this.label8.TabIndex = 20;
			this.label8.Text = "State:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRegKey
			// 
			this.labelRegKey.Location = new System.Drawing.Point(78, 34);
			this.labelRegKey.Name = "labelRegKey";
			this.labelRegKey.Size = new System.Drawing.Size(215, 13);
			this.labelRegKey.TabIndex = 19;
			this.labelRegKey.Text = "XXXX XXXX XXXX XXXX";
			this.labelRegKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(7, 34);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(65, 13);
			this.label6.TabIndex = 18;
			this.label6.Text = "RegKey:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.butCompare);
			this.groupBox2.Controls.Add(this.butBugTask);
			this.groupBox2.Controls.Add(this.labelLastCall);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.labelSubNum);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.butGoToAccount);
			this.groupBox2.Controls.Add(this.labelCustomerNum);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.labelCustomerPhone);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.labelCustomerState);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.labelRegKey);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Location = new System.Drawing.Point(3, 410);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(411, 148);
			this.groupBox2.TabIndex = 41;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Submitter Info.";
			// 
			// butCompare
			// 
			this.butCompare.Location = new System.Drawing.Point(225, 122);
			this.butCompare.Name = "butCompare";
			this.butCompare.Size = new System.Drawing.Size(104, 24);
			this.butCompare.TabIndex = 31;
			this.butCompare.Text = "&Compare";
			this.butCompare.Click += new System.EventHandler(this.butCompare_Click);
			// 
			// butBugTask
			// 
			this.butBugTask.Location = new System.Drawing.Point(115, 122);
			this.butBugTask.Name = "butBugTask";
			this.butBugTask.Size = new System.Drawing.Size(104, 24);
			this.butBugTask.TabIndex = 30;
			this.butBugTask.Text = "&Create Task";
			this.butBugTask.Click += new System.EventHandler(this.butBugTask_Click);
			// 
			// butGoToAccount
			// 
			this.butGoToAccount.Location = new System.Drawing.Point(6, 122);
			this.butGoToAccount.Name = "butGoToAccount";
			this.butGoToAccount.Size = new System.Drawing.Size(103, 24);
			this.butGoToAccount.TabIndex = 21;
			this.butGoToAccount.Text = "&Go To Account";
			this.butGoToAccount.Click += new System.EventHandler(this.butGoToAccount_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.textDevNote);
			this.splitContainer1.Size = new System.Drawing.Size(410, 402);
			this.splitContainer1.SplitterDistance = 360;
			this.splitContainer1.TabIndex = 43;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabStackTrace);
			this.tabControl1.Controls.Add(this.tabOdStackSig);
			this.tabControl1.Controls.Add(this.tabSimpleStack);
			this.tabControl1.Controls.Add(this.tabCategoryTags);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(407, 358);
			this.tabControl1.TabIndex = 41;
			// 
			// tabStackTrace
			// 
			this.tabStackTrace.Controls.Add(this.textStack);
			this.tabStackTrace.Location = new System.Drawing.Point(4, 22);
			this.tabStackTrace.Margin = new System.Windows.Forms.Padding(0);
			this.tabStackTrace.Name = "tabStackTrace";
			this.tabStackTrace.Size = new System.Drawing.Size(399, 332);
			this.tabStackTrace.TabIndex = 0;
			this.tabStackTrace.Text = "Stack Trace";
			this.tabStackTrace.UseVisualStyleBackColor = true;
			// 
			// textStack
			// 
			this.textStack.AcceptsTab = true;
			this.textStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textStack.BackColor = System.Drawing.SystemColors.Window;
			this.textStack.DetectLinksEnabled = false;
			this.textStack.DetectUrls = false;
			this.textStack.Location = new System.Drawing.Point(0, 0);
			this.textStack.Name = "textStack";
			this.textStack.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textStack.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textStack.Size = new System.Drawing.Size(399, 332);
			this.textStack.SpellCheckIsEnabled = false;
			this.textStack.TabIndex = 5;
			this.textStack.Text = "";
			// 
			// tabOdStackSig
			// 
			this.tabOdStackSig.Controls.Add(this.textODStack);
			this.tabOdStackSig.Location = new System.Drawing.Point(4, 22);
			this.tabOdStackSig.Name = "tabOdStackSig";
			this.tabOdStackSig.Size = new System.Drawing.Size(399, 332);
			this.tabOdStackSig.TabIndex = 1;
			this.tabOdStackSig.Text = "Stack Sig";
			this.tabOdStackSig.UseVisualStyleBackColor = true;
			// 
			// textODStack
			// 
			this.textODStack.AcceptsTab = true;
			this.textODStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textODStack.BackColor = System.Drawing.SystemColors.Window;
			this.textODStack.DetectLinksEnabled = false;
			this.textODStack.DetectUrls = false;
			this.textODStack.Location = new System.Drawing.Point(0, 0);
			this.textODStack.Name = "textODStack";
			this.textODStack.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textODStack.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textODStack.Size = new System.Drawing.Size(399, 332);
			this.textODStack.SpellCheckIsEnabled = false;
			this.textODStack.TabIndex = 6;
			this.textODStack.Text = "";
			// 
			// tabSimpleStack
			// 
			this.tabSimpleStack.Controls.Add(this.textStrippedStack);
			this.tabSimpleStack.Location = new System.Drawing.Point(4, 22);
			this.tabSimpleStack.Name = "tabSimpleStack";
			this.tabSimpleStack.Size = new System.Drawing.Size(399, 332);
			this.tabSimpleStack.TabIndex = 2;
			this.tabSimpleStack.Text = "Stack Simple";
			this.tabSimpleStack.UseVisualStyleBackColor = true;
			// 
			// textStrippedStack
			// 
			this.textStrippedStack.AcceptsTab = true;
			this.textStrippedStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textStrippedStack.BackColor = System.Drawing.SystemColors.Window;
			this.textStrippedStack.DetectLinksEnabled = false;
			this.textStrippedStack.DetectUrls = false;
			this.textStrippedStack.Location = new System.Drawing.Point(0, 0);
			this.textStrippedStack.Name = "textStrippedStack";
			this.textStrippedStack.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textStrippedStack.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textStrippedStack.Size = new System.Drawing.Size(399, 332);
			this.textStrippedStack.SpellCheckIsEnabled = false;
			this.textStrippedStack.TabIndex = 6;
			this.textStrippedStack.Text = "";
			// 
			// tabCategoryTags
			// 
			this.tabCategoryTags.Controls.Add(this.butDeleteCategory);
			this.tabCategoryTags.Controls.Add(this.butAddCategory);
			this.tabCategoryTags.Controls.Add(this.listBoxCategories);
			this.tabCategoryTags.Location = new System.Drawing.Point(4, 22);
			this.tabCategoryTags.Name = "tabCategoryTags";
			this.tabCategoryTags.Size = new System.Drawing.Size(399, 332);
			this.tabCategoryTags.TabIndex = 3;
			this.tabCategoryTags.Text = "Categories";
			this.tabCategoryTags.UseVisualStyleBackColor = true;
			// 
			// butDeleteCategory
			// 
			this.butDeleteCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteCategory.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteCategory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteCategory.Location = new System.Drawing.Point(77, 306);
			this.butDeleteCategory.Name = "butDeleteCategory";
			this.butDeleteCategory.Size = new System.Drawing.Size(69, 23);
			this.butDeleteCategory.TabIndex = 2;
			this.butDeleteCategory.Text = "Delete";
			this.butDeleteCategory.UseVisualStyleBackColor = true;
			this.butDeleteCategory.Click += new System.EventHandler(this.butDeleteCategory_Click);
			// 
			// butAddCategory
			// 
			this.butAddCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddCategory.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCategory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCategory.Location = new System.Drawing.Point(2, 306);
			this.butAddCategory.Name = "butAddCategory";
			this.butAddCategory.Size = new System.Drawing.Size(69, 23);
			this.butAddCategory.TabIndex = 1;
			this.butAddCategory.Text = "Add";
			this.butAddCategory.UseVisualStyleBackColor = true;
			this.butAddCategory.Click += new System.EventHandler(this.butAddCategory_Click);
			// 
			// listBoxCategories
			// 
			this.listBoxCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxCategories.Location = new System.Drawing.Point(0, 0);
			this.listBoxCategories.Name = "listBoxCategories";
			this.listBoxCategories.Size = new System.Drawing.Size(399, 303);
			this.listBoxCategories.TabIndex = 0;
			// 
			// textDevNote
			// 
			this.textDevNote.AcceptsTab = true;
			this.textDevNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDevNote.BackColor = System.Drawing.SystemColors.Window;
			this.textDevNote.DetectLinksEnabled = false;
			this.textDevNote.DetectUrls = false;
			this.textDevNote.Location = new System.Drawing.Point(0, 0);
			this.textDevNote.Name = "textDevNote";
			this.textDevNote.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textDevNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDevNote.Size = new System.Drawing.Size(407, 35);
			this.textDevNote.SpellCheckIsEnabled = false;
			this.textDevNote.TabIndex = 6;
			this.textDevNote.Text = "";
			this.textDevNote.Leave += new System.EventHandler(this.textDevNote_Leave);
			// 
			// gridCustomerSubs
			// 
			this.gridCustomerSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.gridCustomerSubs.HasLinkDetect = false;
			this.gridCustomerSubs.HeadersVisible = false;
			this.gridCustomerSubs.Location = new System.Drawing.Point(420, 411);
			this.gridCustomerSubs.Name = "gridCustomerSubs";
			this.gridCustomerSubs.Size = new System.Drawing.Size(255, 118);
			this.gridCustomerSubs.TabIndex = 44;
			this.gridCustomerSubs.Title = "Customer Submissions";
			this.gridCustomerSubs.TranslationName = "TableSubmissions";
			this.gridCustomerSubs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCustomerSubs_CellDoubleClick);
			this.gridCustomerSubs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCustomerSubs_CellClick);
			// 
			// gridOfficeInfo
			// 
			this.gridOfficeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOfficeInfo.Location = new System.Drawing.Point(420, 3);
			this.gridOfficeInfo.Name = "gridOfficeInfo";
			this.gridOfficeInfo.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOfficeInfo.Size = new System.Drawing.Size(255, 402);
			this.gridOfficeInfo.TabIndex = 42;
			this.gridOfficeInfo.Title = "Office Info";
			this.gridOfficeInfo.TranslationName = "TableOfficeInfo";
			// 
			// BugSubmissionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridCustomerSubs);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.gridOfficeInfo);
			this.Controls.Add(this.groupBox2);
			this.MinimumSize = new System.Drawing.Size(594, 521);
			this.Name = "BugSubmissionControl";
			this.Size = new System.Drawing.Size(679, 561);
			this.Load += new System.EventHandler(this.BugSubmissionControl_Load);
			this.groupBox2.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabStackTrace.ResumeLayout(false);
			this.tabOdStackSig.ResumeLayout(false);
			this.tabSimpleStack.ResumeLayout(false);
			this.tabCategoryTags.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butBugTask;
		private System.Windows.Forms.Label labelLastCall;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label labelSubNum;
		private System.Windows.Forms.Label label10;
		private UI.Button butGoToAccount;
		private System.Windows.Forms.Label labelCustomerNum;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label labelCustomerPhone;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TabPage tabStackTrace;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Label labelCustomerState;
		private UI.GridOD gridOfficeInfo;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labelRegKey;
		private System.Windows.Forms.Label label6;
		private ODtextBox textDevNote;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox groupBox2;
		private ODtextBox textStack;
		private UI.GridOD gridCustomerSubs;
		private Button butCompare;
		private System.Windows.Forms.TabPage tabOdStackSig;
		private ODtextBox textODStack;
		private System.Windows.Forms.TabPage tabSimpleStack;
		private ODtextBox textStrippedStack;
		private System.Windows.Forms.TabPage tabCategoryTags;
		private OpenDental.UI.ListBoxOD listBoxCategories;
		private Button butAddCategory;
		private Button butDeleteCategory;
	}
}
