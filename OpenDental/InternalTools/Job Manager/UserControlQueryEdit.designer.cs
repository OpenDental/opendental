namespace OpenDental.InternalTools.Job_Manager {
	partial class UserControlQueryEdit {
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
			this.gridFiles = new OpenDental.UI.GridOD();
			this.gridAppointments = new OpenDental.UI.GridOD();
			this.gridTasks = new OpenDental.UI.GridOD();
			this.timerTitle = new System.Windows.Forms.Timer(this.components);
			this.timerVersion = new System.Windows.Forms.Timer(this.components);
			this.panel3 = new System.Windows.Forms.Panel();
			this.butParentPick = new OpenDental.UI.Button();
			this.textParent = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butParentRemove = new OpenDental.UI.Button();
			this.labelRelatedJobs = new System.Windows.Forms.Label();
			this.treeRelatedJobs = new System.Windows.Forms.TreeView();
			this.gridRoles = new OpenDental.UI.GridOD();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label9 = new System.Windows.Forms.Label();
			this.textQuoteDate = new System.Windows.Forms.TextBox();
			this.checkApproved = new System.Windows.Forms.CheckBox();
			this.textQuoteAmount = new System.Windows.Forms.TextBox();
			this.textQuoteHours = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboPhase = new OpenDental.UI.ComboBoxOD();
			this.comboPriority = new OpenDental.UI.ComboBoxOD();
			this.textCustomer = new System.Windows.Forms.TextBox();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.textDateEntry = new System.Windows.Forms.TextBox();
			this.textJobNum = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.textHoursLeft = new OpenDental.ValidDouble();
			this.butAddTime = new OpenDental.UI.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.butTimeLog = new OpenDental.UI.Button();
			this.textActualHours = new OpenDental.ValidDouble();
			this.textEstHours = new OpenDental.ValidDouble();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.butChangeEst = new OpenDental.UI.Button();
			this.butPhoneNums = new OpenDental.UI.Button();
			this.label16 = new System.Windows.Forms.Label();
			this.text0_30 = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.text31_60 = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.text61_90 = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textOver90 = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textZip = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textBillingType = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.butEmail = new OpenDental.UI.Button();
			this.butCommlog = new OpenDental.UI.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.textSchedDate = new System.Windows.Forms.TextBox();
			this.butActions = new OpenDental.UI.Button();
			this.panel5 = new System.Windows.Forms.Panel();
			this.splitContainerNoFlicker2 = new OpenDental.SplitContainerNoFlicker();
			this.textEditorMain = new OpenDental.OdtextEditor();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabMain = new System.Windows.Forms.TabPage();
			this.gridNotes = new OpenDental.UI.GridOD();
			this.tabReviews = new System.Windows.Forms.TabPage();
			this.gridReview = new OpenDental.UI.GridOD();
			this.splitContainerNoFlicker1 = new OpenDental.SplitContainerNoFlicker();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panel3.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker2)).BeginInit();
			this.splitContainerNoFlicker2.Panel1.SuspendLayout();
			this.splitContainerNoFlicker2.Panel2.SuspendLayout();
			this.splitContainerNoFlicker2.SuspendLayout();
			this.tabControlMain.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.tabReviews.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker1)).BeginInit();
			this.splitContainerNoFlicker1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridFiles
			// 
			this.gridFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridFiles.HasAddButton = true;
			this.gridFiles.Location = new System.Drawing.Point(3, 322);
			this.gridFiles.Name = "gridFiles";
			this.gridFiles.Size = new System.Drawing.Size(235, 155);
			this.gridFiles.TabIndex = 260;
			this.gridFiles.Title = "Files";
			this.gridFiles.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFiles_CellDoubleClick);
			this.gridFiles.TitleAddClick += new System.EventHandler(this.gridFiles_TitleAddClick);
			this.gridFiles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridFiles_MouseMove);
			// 
			// gridAppointments
			// 
			this.gridAppointments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridAppointments.HasAddButton = true;
			this.gridAppointments.Location = new System.Drawing.Point(3, 162);
			this.gridAppointments.Name = "gridAppointments";
			this.gridAppointments.Size = new System.Drawing.Size(235, 154);
			this.gridAppointments.TabIndex = 228;
			this.gridAppointments.Title = "Appointments";
			this.gridAppointments.TranslationName = "FormTaskEdit";
			this.gridAppointments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAppointments_CellDoubleClick);
			this.gridAppointments.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAppointments_CellClick);
			this.gridAppointments.TitleAddClick += new System.EventHandler(this.gridAppointments_TitleAddClick);
			// 
			// gridTasks
			// 
			this.gridTasks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridTasks.HasAddButton = true;
			this.gridTasks.Location = new System.Drawing.Point(3, 3);
			this.gridTasks.Name = "gridTasks";
			this.gridTasks.Size = new System.Drawing.Size(235, 153);
			this.gridTasks.TabIndex = 227;
			this.gridTasks.Title = "Tasks";
			this.gridTasks.TranslationName = "FormTaskEdit";
			this.gridTasks.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellDoubleClick);
			this.gridTasks.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellClick);
			this.gridTasks.TitleAddClick += new System.EventHandler(this.gridTasks_TitleAddClick);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.butParentPick);
			this.panel3.Controls.Add(this.textParent);
			this.panel3.Controls.Add(this.label11);
			this.panel3.Controls.Add(this.butParentRemove);
			this.panel3.Controls.Add(this.labelRelatedJobs);
			this.panel3.Controls.Add(this.treeRelatedJobs);
			this.panel3.Controls.Add(this.gridRoles);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(241, 234);
			this.panel3.TabIndex = 305;
			// 
			// butParentPick
			// 
			this.butParentPick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butParentPick.Location = new System.Drawing.Point(190, 105);
			this.butParentPick.Name = "butParentPick";
			this.butParentPick.Size = new System.Drawing.Size(23, 20);
			this.butParentPick.TabIndex = 315;
			this.butParentPick.Text = "...";
			this.butParentPick.Click += new System.EventHandler(this.butParentPick_Click);
			// 
			// textParent
			// 
			this.textParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textParent.Location = new System.Drawing.Point(101, 105);
			this.textParent.MaxLength = 100;
			this.textParent.Name = "textParent";
			this.textParent.ReadOnly = true;
			this.textParent.Size = new System.Drawing.Size(89, 20);
			this.textParent.TabIndex = 312;
			this.textParent.TabStop = false;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.Location = new System.Drawing.Point(6, 105);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(93, 20);
			this.label11.TabIndex = 313;
			this.label11.Text = "Parent Job Num";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butParentRemove
			// 
			this.butParentRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butParentRemove.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butParentRemove.Location = new System.Drawing.Point(213, 105);
			this.butParentRemove.Name = "butParentRemove";
			this.butParentRemove.Size = new System.Drawing.Size(23, 20);
			this.butParentRemove.TabIndex = 314;
			this.butParentRemove.Click += new System.EventHandler(this.butParentRemove_Click);
			// 
			// labelRelatedJobs
			// 
			this.labelRelatedJobs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRelatedJobs.Location = new System.Drawing.Point(2, 128);
			this.labelRelatedJobs.Name = "labelRelatedJobs";
			this.labelRelatedJobs.Size = new System.Drawing.Size(235, 20);
			this.labelRelatedJobs.TabIndex = 311;
			this.labelRelatedJobs.Text = "Related Jobs";
			this.labelRelatedJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// treeRelatedJobs
			// 
			this.treeRelatedJobs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeRelatedJobs.Indent = 9;
			this.treeRelatedJobs.Location = new System.Drawing.Point(2, 151);
			this.treeRelatedJobs.Name = "treeRelatedJobs";
			this.treeRelatedJobs.Size = new System.Drawing.Size(235, 80);
			this.treeRelatedJobs.TabIndex = 310;
			this.treeRelatedJobs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeRelatedJobs_AfterSelect);
			this.treeRelatedJobs.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeRelatedJobs_NodeMouseClick);
			// 
			// gridRoles
			// 
			this.gridRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRoles.Location = new System.Drawing.Point(5, 0);
			this.gridRoles.Name = "gridRoles";
			this.gridRoles.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridRoles.Size = new System.Drawing.Size(232, 100);
			this.gridRoles.TabIndex = 304;
			this.gridRoles.Title = "Query Roles";
			this.gridRoles.TranslationName = "FormTaskEdit";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gridFiles, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.gridTasks, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gridAppointments, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 243);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(241, 480);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(325, 10);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(104, 13);
			this.label9.TabIndex = 328;
			this.label9.Text = "Approved Date";
			// 
			// textQuoteDate
			// 
			this.textQuoteDate.Location = new System.Drawing.Point(328, 26);
			this.textQuoteDate.Name = "textQuoteDate";
			this.textQuoteDate.Size = new System.Drawing.Size(140, 20);
			this.textQuoteDate.TabIndex = 327;
			this.textQuoteDate.Leave += new System.EventHandler(this.textQuoteDate_Leave);
			// 
			// checkApproved
			// 
			this.checkApproved.Location = new System.Drawing.Point(474, 23);
			this.checkApproved.Name = "checkApproved";
			this.checkApproved.Size = new System.Drawing.Size(74, 24);
			this.checkApproved.TabIndex = 326;
			this.checkApproved.Text = "Approved";
			this.checkApproved.UseVisualStyleBackColor = true;
			this.checkApproved.CheckedChanged += new System.EventHandler(this.checkApproved_CheckedChanged);
			// 
			// textQuoteAmount
			// 
			this.textQuoteAmount.Location = new System.Drawing.Point(254, 63);
			this.textQuoteAmount.Name = "textQuoteAmount";
			this.textQuoteAmount.Size = new System.Drawing.Size(67, 20);
			this.textQuoteAmount.TabIndex = 324;
			this.textQuoteAmount.Leave += new System.EventHandler(this.textQuoteAmount_Leave);
			// 
			// textQuoteHours
			// 
			this.textQuoteHours.Location = new System.Drawing.Point(254, 26);
			this.textQuoteHours.Name = "textQuoteHours";
			this.textQuoteHours.Size = new System.Drawing.Size(67, 20);
			this.textQuoteHours.TabIndex = 323;
			this.textQuoteHours.Leave += new System.EventHandler(this.textQuoteHours_Leave);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(251, 49);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(70, 13);
			this.label8.TabIndex = 322;
			this.label8.Text = "Quote Amt";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(251, 11);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 13);
			this.label7.TabIndex = 321;
			this.label7.Text = "Quote Hours";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(123, 47);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(124, 13);
			this.label6.TabIndex = 320;
			this.label6.Text = "Phase";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(125, 10);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(122, 13);
			this.label5.TabIndex = 319;
			this.label5.Text = "Priority";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(548, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(122, 13);
			this.label4.TabIndex = 318;
			this.label4.Text = "Customer";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(471, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 13);
			this.label3.TabIndex = 317;
			this.label3.Text = "Title";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 13);
			this.label2.TabIndex = 316;
			this.label2.Text = "Date Entry";
			// 
			// comboPhase
			// 
			this.comboPhase.Location = new System.Drawing.Point(126, 63);
			this.comboPhase.Name = "comboPhase";
			this.comboPhase.Size = new System.Drawing.Size(121, 21);
			this.comboPhase.TabIndex = 315;
			this.comboPhase.SelectionChangeCommitted += new System.EventHandler(this.comboPhase_SelectionChangeCommitted);
			// 
			// comboPriority
			// 
			this.comboPriority.Location = new System.Drawing.Point(126, 26);
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(121, 21);
			this.comboPriority.TabIndex = 314;
			this.comboPriority.SelectionChangeCommitted += new System.EventHandler(this.comboPriority_SelectionChangeCommitted);
			// 
			// textCustomer
			// 
			this.textCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCustomer.Location = new System.Drawing.Point(551, 26);
			this.textCustomer.Name = "textCustomer";
			this.textCustomer.ReadOnly = true;
			this.textCustomer.Size = new System.Drawing.Size(144, 20);
			this.textCustomer.TabIndex = 311;
			this.textCustomer.Click += new System.EventHandler(this.textCustomer_Click);
			// 
			// textTitle
			// 
			this.textTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTitle.Location = new System.Drawing.Point(474, 63);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(221, 20);
			this.textTitle.TabIndex = 309;
			this.textTitle.Leave += new System.EventHandler(this.textTitle_Leave);
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(14, 64);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(100, 20);
			this.textDateEntry.TabIndex = 307;
			// 
			// textJobNum
			// 
			this.textJobNum.Location = new System.Drawing.Point(14, 26);
			this.textJobNum.Name = "textJobNum";
			this.textJobNum.ReadOnly = true;
			this.textJobNum.Size = new System.Drawing.Size(100, 20);
			this.textJobNum.TabIndex = 305;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 13);
			this.label1.TabIndex = 304;
			this.label1.Text = "JobNum";
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.textHoursLeft);
			this.panel4.Controls.Add(this.butAddTime);
			this.panel4.Controls.Add(this.label15);
			this.panel4.Controls.Add(this.butTimeLog);
			this.panel4.Controls.Add(this.textActualHours);
			this.panel4.Controls.Add(this.textEstHours);
			this.panel4.Controls.Add(this.label21);
			this.panel4.Controls.Add(this.label22);
			this.panel4.Controls.Add(this.butChangeEst);
			this.panel4.Controls.Add(this.butPhoneNums);
			this.panel4.Controls.Add(this.label16);
			this.panel4.Controls.Add(this.text0_30);
			this.panel4.Controls.Add(this.label17);
			this.panel4.Controls.Add(this.text31_60);
			this.panel4.Controls.Add(this.label18);
			this.panel4.Controls.Add(this.text61_90);
			this.panel4.Controls.Add(this.label19);
			this.panel4.Controls.Add(this.textOver90);
			this.panel4.Controls.Add(this.label20);
			this.panel4.Controls.Add(this.textState);
			this.panel4.Controls.Add(this.label14);
			this.panel4.Controls.Add(this.textZip);
			this.panel4.Controls.Add(this.label13);
			this.panel4.Controls.Add(this.textBillingType);
			this.panel4.Controls.Add(this.label12);
			this.panel4.Controls.Add(this.butEmail);
			this.panel4.Controls.Add(this.butCommlog);
			this.panel4.Controls.Add(this.label10);
			this.panel4.Controls.Add(this.textSchedDate);
			this.panel4.Controls.Add(this.butActions);
			this.panel4.Controls.Add(this.comboPhase);
			this.panel4.Controls.Add(this.comboPriority);
			this.panel4.Controls.Add(this.label9);
			this.panel4.Controls.Add(this.textTitle);
			this.panel4.Controls.Add(this.label5);
			this.panel4.Controls.Add(this.label1);
			this.panel4.Controls.Add(this.textQuoteDate);
			this.panel4.Controls.Add(this.textJobNum);
			this.panel4.Controls.Add(this.label6);
			this.panel4.Controls.Add(this.textDateEntry);
			this.panel4.Controls.Add(this.textCustomer);
			this.panel4.Controls.Add(this.label7);
			this.panel4.Controls.Add(this.label4);
			this.panel4.Controls.Add(this.checkApproved);
			this.panel4.Controls.Add(this.label3);
			this.panel4.Controls.Add(this.label8);
			this.panel4.Controls.Add(this.label2);
			this.panel4.Controls.Add(this.textQuoteHours);
			this.panel4.Controls.Add(this.textQuoteAmount);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(766, 151);
			this.panel4.TabIndex = 329;
			// 
			// textHoursLeft
			// 
			this.textHoursLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textHoursLeft.Location = new System.Drawing.Point(647, 127);
			this.textHoursLeft.MaxVal = 1000000D;
			this.textHoursLeft.MinVal = 0D;
			this.textHoursLeft.Name = "textHoursLeft";
			this.textHoursLeft.ReadOnly = true;
			this.textHoursLeft.Size = new System.Drawing.Size(44, 20);
			this.textHoursLeft.TabIndex = 357;
			// 
			// butAddTime
			// 
			this.butAddTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddTime.Location = new System.Drawing.Point(693, 106);
			this.butAddTime.Name = "butAddTime";
			this.butAddTime.Size = new System.Drawing.Size(71, 20);
			this.butAddTime.TabIndex = 355;
			this.butAddTime.Text = "Add Time";
			this.butAddTime.Click += new System.EventHandler(this.butAddTime_Click);
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.Location = new System.Drawing.Point(597, 128);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(49, 20);
			this.label15.TabIndex = 356;
			this.label15.Text = "Hrs. Left";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butTimeLog
			// 
			this.butTimeLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTimeLog.Location = new System.Drawing.Point(693, 127);
			this.butTimeLog.Name = "butTimeLog";
			this.butTimeLog.Size = new System.Drawing.Size(71, 20);
			this.butTimeLog.TabIndex = 359;
			this.butTimeLog.Text = "Log";
			this.butTimeLog.Click += new System.EventHandler(this.butTimeLog_Click);
			// 
			// textActualHours
			// 
			this.textActualHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textActualHours.Location = new System.Drawing.Point(647, 106);
			this.textActualHours.MaxVal = 1000000D;
			this.textActualHours.MinVal = 0D;
			this.textActualHours.Name = "textActualHours";
			this.textActualHours.ReadOnly = true;
			this.textActualHours.Size = new System.Drawing.Size(44, 20);
			this.textActualHours.TabIndex = 354;
			// 
			// textEstHours
			// 
			this.textEstHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textEstHours.Location = new System.Drawing.Point(647, 85);
			this.textEstHours.MaxVal = 1000000D;
			this.textEstHours.MinVal = 0D;
			this.textEstHours.Name = "textEstHours";
			this.textEstHours.ReadOnly = true;
			this.textEstHours.Size = new System.Drawing.Size(44, 20);
			this.textEstHours.TabIndex = 353;
			// 
			// label21
			// 
			this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label21.Location = new System.Drawing.Point(581, 106);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(65, 20);
			this.label21.TabIndex = 352;
			this.label21.Text = "Hrs. So Far";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label22
			// 
			this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label22.Location = new System.Drawing.Point(597, 85);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(49, 20);
			this.label22.TabIndex = 351;
			this.label22.Text = "Hrs. Est.";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butChangeEst
			// 
			this.butChangeEst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeEst.Location = new System.Drawing.Point(693, 85);
			this.butChangeEst.Name = "butChangeEst";
			this.butChangeEst.Size = new System.Drawing.Size(71, 20);
			this.butChangeEst.TabIndex = 358;
			this.butChangeEst.Text = "Change Est.";
			this.butChangeEst.Click += new System.EventHandler(this.butChangeEst_Click);
			// 
			// butPhoneNums
			// 
			this.butPhoneNums.Location = new System.Drawing.Point(506, 116);
			this.butPhoneNums.Name = "butPhoneNums";
			this.butPhoneNums.Size = new System.Drawing.Size(75, 23);
			this.butPhoneNums.TabIndex = 350;
			this.butPhoneNums.Text = "Phone Nums";
			this.butPhoneNums.UseVisualStyleBackColor = true;
			this.butPhoneNums.Click += new System.EventHandler(this.butPhoneNums_Click);
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(45, 104);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(55, 13);
			this.label16.TabIndex = 349;
			this.label16.Text = "0-30";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// text0_30
			// 
			this.text0_30.Location = new System.Drawing.Point(43, 117);
			this.text0_30.Name = "text0_30";
			this.text0_30.ReadOnly = true;
			this.text0_30.Size = new System.Drawing.Size(55, 20);
			this.text0_30.TabIndex = 348;
			this.text0_30.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(98, 104);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(55, 13);
			this.label17.TabIndex = 347;
			this.label17.Text = "31-60";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// text31_60
			// 
			this.text31_60.Location = new System.Drawing.Point(98, 117);
			this.text31_60.Name = "text31_60";
			this.text31_60.ReadOnly = true;
			this.text31_60.Size = new System.Drawing.Size(55, 20);
			this.text31_60.TabIndex = 346;
			this.text31_60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(153, 104);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(55, 13);
			this.label18.TabIndex = 345;
			this.label18.Text = "61-90";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// text61_90
			// 
			this.text61_90.Location = new System.Drawing.Point(153, 117);
			this.text61_90.Name = "text61_90";
			this.text61_90.ReadOnly = true;
			this.text61_90.Size = new System.Drawing.Size(55, 20);
			this.text61_90.TabIndex = 344;
			this.text61_90.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(208, 104);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(55, 13);
			this.label19.TabIndex = 343;
			this.label19.Text = "over 90";
			this.label19.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textOver90
			// 
			this.textOver90.Location = new System.Drawing.Point(208, 117);
			this.textOver90.Name = "textOver90";
			this.textOver90.ReadOnly = true;
			this.textOver90.Size = new System.Drawing.Size(55, 20);
			this.textOver90.TabIndex = 342;
			this.textOver90.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label20
			// 
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label20.Location = new System.Drawing.Point(3, 104);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(37, 33);
			this.label20.TabIndex = 341;
			this.label20.Text = "Family\r\nAging";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(376, 118);
			this.textState.Name = "textState";
			this.textState.ReadOnly = true;
			this.textState.Size = new System.Drawing.Size(35, 20);
			this.textState.TabIndex = 337;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(373, 103);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(38, 13);
			this.label14.TabIndex = 338;
			this.label14.Text = "State";
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(417, 118);
			this.textZip.Name = "textZip";
			this.textZip.ReadOnly = true;
			this.textZip.Size = new System.Drawing.Size(83, 20);
			this.textZip.TabIndex = 335;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(417, 103);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(83, 13);
			this.label13.TabIndex = 336;
			this.label13.Text = "Zip Code";
			// 
			// textBillingType
			// 
			this.textBillingType.Location = new System.Drawing.Point(270, 118);
			this.textBillingType.Name = "textBillingType";
			this.textBillingType.ReadOnly = true;
			this.textBillingType.Size = new System.Drawing.Size(100, 20);
			this.textBillingType.TabIndex = 333;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(267, 103);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(103, 13);
			this.label12.TabIndex = 334;
			this.label12.Text = "Billing Type";
			// 
			// butEmail
			// 
			this.butEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEmail.Location = new System.Drawing.Point(705, 47);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(58, 23);
			this.butEmail.TabIndex = 332;
			this.butEmail.Text = "Email";
			this.butEmail.UseVisualStyleBackColor = true;
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butCommlog
			// 
			this.butCommlog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCommlog.Location = new System.Drawing.Point(705, 24);
			this.butCommlog.Name = "butCommlog";
			this.butCommlog.Size = new System.Drawing.Size(58, 23);
			this.butCommlog.TabIndex = 331;
			this.butCommlog.Text = "Commlog";
			this.butCommlog.UseVisualStyleBackColor = true;
			this.butCommlog.Click += new System.EventHandler(this.butCommlog_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(325, 49);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(70, 13);
			this.label10.TabIndex = 330;
			this.label10.Text = "Sched Date";
			// 
			// textSchedDate
			// 
			this.textSchedDate.Location = new System.Drawing.Point(328, 63);
			this.textSchedDate.Name = "textSchedDate";
			this.textSchedDate.Size = new System.Drawing.Size(140, 20);
			this.textSchedDate.TabIndex = 329;
			this.textSchedDate.Leave += new System.EventHandler(this.textSchedDate_Leave);
			// 
			// butActions
			// 
			this.butActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butActions.Image = global::OpenDental.Properties.Resources.arrowDownTriangle;
			this.butActions.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butActions.Location = new System.Drawing.Point(669, 0);
			this.butActions.Name = "butActions";
			this.butActions.Size = new System.Drawing.Size(95, 23);
			this.butActions.TabIndex = 303;
			this.butActions.Text = "Job Actions";
			this.butActions.Click += new System.EventHandler(this.butActions_Click);
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.splitContainerNoFlicker2);
			this.panel5.Controls.Add(this.splitContainerNoFlicker1);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel5.Location = new System.Drawing.Point(0, 151);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(766, 575);
			this.panel5.TabIndex = 330;
			// 
			// splitContainerNoFlicker2
			// 
			this.splitContainerNoFlicker2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerNoFlicker2.Location = new System.Drawing.Point(0, 0);
			this.splitContainerNoFlicker2.Name = "splitContainerNoFlicker2";
			// 
			// splitContainerNoFlicker2.Panel1
			// 
			this.splitContainerNoFlicker2.Panel1.Controls.Add(this.textEditorMain);
			// 
			// splitContainerNoFlicker2.Panel2
			// 
			this.splitContainerNoFlicker2.Panel2.Controls.Add(this.tabControlMain);
			this.splitContainerNoFlicker2.Size = new System.Drawing.Size(766, 575);
			this.splitContainerNoFlicker2.SplitterDistance = 496;
			this.splitContainerNoFlicker2.TabIndex = 308;
			// 
			// textEditorMain
			// 
			this.textEditorMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEditorMain.HasEditorOptions = true;
			this.textEditorMain.HasSaveButton = true;
			this.textEditorMain.Location = new System.Drawing.Point(0, 0);
			this.textEditorMain.MainFont = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textEditorMain.MainRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 " +
    "Consolas;}}\r\n{\\*\\generator Riched20 10.0.17763}\\viewkind4\\uc1 \r\n\\pard\\f0\\fs18\\pa" +
    "r\r\n}\r\n";
			this.textEditorMain.MainText = "";
			this.textEditorMain.MinimumSize = new System.Drawing.Size(450, 120);
			this.textEditorMain.Name = "textEditorMain";
			this.textEditorMain.ReadOnly = false;
			this.textEditorMain.Size = new System.Drawing.Size(496, 575);
			this.textEditorMain.TabIndex = 306;
			this.textEditorMain.SaveClick += new OpenDental.ODtextEditorSaveEventHandler(this.textEditor_SaveClick);
			this.textEditorMain.OnTextEdited += new OpenDental.OdtextEditor.textChangedEventHandler(this.textEditorMain_OnTextEdited);
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabMain);
			this.tabControlMain.Controls.Add(this.tabReviews);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(266, 575);
			this.tabControlMain.TabIndex = 261;
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.gridNotes);
			this.tabMain.Location = new System.Drawing.Point(4, 22);
			this.tabMain.Name = "tabMain";
			this.tabMain.Padding = new System.Windows.Forms.Padding(3);
			this.tabMain.Size = new System.Drawing.Size(258, 549);
			this.tabMain.TabIndex = 0;
			this.tabMain.Text = "Discussion";
			this.tabMain.UseVisualStyleBackColor = true;
			// 
			// gridNotes
			// 
			this.gridNotes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridNotes.HasAddButton = true;
			this.gridNotes.Location = new System.Drawing.Point(3, 3);
			this.gridNotes.Name = "gridNotes";
			this.gridNotes.Size = new System.Drawing.Size(252, 543);
			this.gridNotes.TabIndex = 194;
			this.gridNotes.Title = "Discussion";
			this.gridNotes.TranslationName = "FormTaskEdit";
			this.gridNotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridNotes_CellDoubleClick);
			this.gridNotes.TitleAddClick += new System.EventHandler(this.gridNotes_TitleAddClick);
			// 
			// tabReviews
			// 
			this.tabReviews.Controls.Add(this.gridReview);
			this.tabReviews.Location = new System.Drawing.Point(4, 22);
			this.tabReviews.Name = "tabReviews";
			this.tabReviews.Padding = new System.Windows.Forms.Padding(3);
			this.tabReviews.Size = new System.Drawing.Size(258, 549);
			this.tabReviews.TabIndex = 4;
			this.tabReviews.Text = "Reviews";
			this.tabReviews.UseVisualStyleBackColor = true;
			// 
			// gridReview
			// 
			this.gridReview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridReview.HasAddButton = true;
			this.gridReview.HScrollVisible = true;
			this.gridReview.Location = new System.Drawing.Point(3, 3);
			this.gridReview.Name = "gridReview";
			this.gridReview.Size = new System.Drawing.Size(252, 543);
			this.gridReview.TabIndex = 22;
			this.gridReview.TabStop = false;
			this.gridReview.Title = "Reviews";
			this.gridReview.TranslationName = "TableReviews";
			this.gridReview.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridReview_CellDoubleClick);
			this.gridReview.TitleAddClick += new System.EventHandler(this.gridReview_TitleAddClick);
			// 
			// splitContainerNoFlicker1
			// 
			this.splitContainerNoFlicker1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerNoFlicker1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerNoFlicker1.Name = "splitContainerNoFlicker1";
			this.splitContainerNoFlicker1.Size = new System.Drawing.Size(766, 575);
			this.splitContainerNoFlicker1.SplitterDistance = 254;
			this.splitContainerNoFlicker1.TabIndex = 307;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(766, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(247, 726);
			this.tableLayoutPanel2.TabIndex = 307;
			// 
			// UserControlQueryEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Name = "UserControlQueryEdit";
			this.Size = new System.Drawing.Size(1013, 726);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.splitContainerNoFlicker2.Panel1.ResumeLayout(false);
			this.splitContainerNoFlicker2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker2)).EndInit();
			this.splitContainerNoFlicker2.ResumeLayout(false);
			this.tabControlMain.ResumeLayout(false);
			this.tabMain.ResumeLayout(false);
			this.tabReviews.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker1)).EndInit();
			this.splitContainerNoFlicker1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridTasks;
		private UI.GridOD gridRoles;
		private System.Windows.Forms.Timer timerTitle;
		private System.Windows.Forms.Timer timerVersion;
		private UI.GridOD gridFiles;
		private UI.GridOD gridAppointments;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabMain;
		private UI.GridOD gridNotes;
		private OdtextEditor textEditorMain;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textQuoteDate;
		private System.Windows.Forms.CheckBox checkApproved;
		private System.Windows.Forms.TextBox textQuoteAmount;
		private System.Windows.Forms.TextBox textQuoteHours;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboPhase;
		private UI.ComboBoxOD comboPriority;
		private System.Windows.Forms.TextBox textCustomer;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.TextBox textDateEntry;
		private System.Windows.Forms.TextBox textJobNum;
		private System.Windows.Forms.Label label1;
		private UI.Button butActions;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private SplitContainerNoFlicker splitContainerNoFlicker2;
		private SplitContainerNoFlicker splitContainerNoFlicker1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textSchedDate;
		private System.Windows.Forms.TabPage tabReviews;
		private UI.GridOD gridReview;
		private UI.Button butEmail;
		private UI.Button butCommlog;
		private System.Windows.Forms.Label labelRelatedJobs;
		private System.Windows.Forms.TreeView treeRelatedJobs;
		private UI.Button butParentPick;
		private System.Windows.Forms.TextBox textParent;
		private System.Windows.Forms.Label label11;
		private UI.Button butParentRemove;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBillingType;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox text0_30;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox text31_60;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox text61_90;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox textOver90;
		private System.Windows.Forms.Label label20;
		private UI.Button butPhoneNums;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private ValidDouble textHoursLeft;
		private UI.Button butAddTime;
		private System.Windows.Forms.Label label15;
		private UI.Button butTimeLog;
		private ValidDouble textActualHours;
		private ValidDouble textEstHours;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private UI.Button butChangeEst;
	}
}
