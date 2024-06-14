namespace OpenDental.InternalTools.Job_Manager {
	partial class UserControlProjectEdit {
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
			this.labelTitle = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gridConcept = new OpenDental.UI.GridOD();
			this.gridComplete = new OpenDental.UI.GridOD();
			this.gridDefinition = new OpenDental.UI.GridOD();
			this.gridDevelopment = new OpenDental.UI.GridOD();
			this.gridDocumentation = new OpenDental.UI.GridOD();
			this.gridPendingReview = new OpenDental.UI.GridOD();
			this.labelJobTeam = new System.Windows.Forms.Label();
			this.textJobNum = new System.Windows.Forms.TextBox();
			this.labelJobNum = new System.Windows.Forms.Label();
			this.textProjectManager = new System.Windows.Forms.TextBox();
			this.labelProjectManager = new System.Windows.Forms.Label();
			this.textGitBranchName = new System.Windows.Forms.TextBox();
			this.labelGitBranchName = new System.Windows.Forms.Label();
			this.labelPriority = new System.Windows.Forms.Label();
			this.labelPhase = new System.Windows.Forms.Label();
			this.labelDateEntry = new System.Windows.Forms.Label();
			this.textDateEntry = new System.Windows.Forms.TextBox();
			this.gridDiscussion = new OpenDental.UI.GridOD();
			this.gridProjects = new OpenDental.UI.GridOD();
			this.labelProjectDescription = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.textVersion = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.textEditorProjectDescription = new OpenDental.OdtextEditor();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.textTitle = new OpenDental.ODtextBox();
			this.textSubmitter = new System.Windows.Forms.TextBox();
			this.labelSubmitter = new System.Windows.Forms.Label();
			this.labelHoursEstimate = new System.Windows.Forms.Label();
			this.labelHoursEstimateSum = new System.Windows.Forms.Label();
			this.labelHoursLeftSum = new System.Windows.Forms.Label();
			this.labelHoursActual = new System.Windows.Forms.Label();
			this.textCheckedOut = new System.Windows.Forms.TextBox();
			this.labelCheckedOut = new System.Windows.Forms.Label();
			this.timerTitle = new System.Windows.Forms.Timer(this.components);
			this.timerVersion = new System.Windows.Forms.Timer(this.components);
			this.comboPriority = new System.Windows.Forms.ComboBox();
			this.butAddExistingJob = new OpenDental.UI.Button();
			this.textHoursActualDescendants = new OpenDental.ValidDouble();
			this.butChangeEst = new OpenDental.UI.Button();
			this.textHoursLeftDescendants = new OpenDental.ValidDouble();
			this.butCalculateSums = new OpenDental.UI.Button();
			this.textEstHours = new OpenDental.ValidDouble();
			this.textHoursEstDescendants = new OpenDental.ValidDouble();
			this.butParentPick = new OpenDental.UI.Button();
			this.butParentRemove = new OpenDental.UI.Button();
			this.butVersionPrompt = new OpenDental.UI.Button();
			this.groupGridFilters = new OpenDental.UI.GroupBox();
			this.checkShowAllChildJobs = new OpenDental.UI.CheckBox();
			this.checkIncludeCancelled = new OpenDental.UI.CheckBox();
			this.checkIncludeComplete = new OpenDental.UI.CheckBox();
			this.butActions = new OpenDental.UI.Button();
			this.checkIsActive = new OpenDental.UI.CheckBox();
			this.comboPhase = new OpenDental.UI.ComboBox();
			this.comboJobTeam = new OpenDental.UI.ComboBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.groupGridFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTitle
			// 
			this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelTitle.Location = new System.Drawing.Point(3, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(29, 26);
			this.labelTitle.TabIndex = 289;
			this.labelTitle.Text = "Title";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Controls.Add(this.gridConcept, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gridComplete, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.gridDefinition, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.gridDevelopment, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.gridDocumentation, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.gridPendingReview, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 639);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1470, 226);
			this.tableLayoutPanel1.TabIndex = 334;
			// 
			// gridConcept
			// 
			this.gridConcept.AutoSize = true;
			this.gridConcept.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridConcept.HScrollVisible = true;
			this.gridConcept.Location = new System.Drawing.Point(3, 3);
			this.gridConcept.Name = "gridConcept";
			this.gridConcept.ShowContextMenu = false;
			this.gridConcept.Size = new System.Drawing.Size(484, 107);
			this.gridConcept.TabIndex = 316;
			this.gridConcept.Title = "Concept";
			this.gridConcept.TranslationName = "FormJobManager";
			this.gridConcept.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// gridComplete
			// 
			this.gridComplete.AutoSize = true;
			this.gridComplete.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridComplete.HScrollVisible = true;
			this.gridComplete.Location = new System.Drawing.Point(983, 116);
			this.gridComplete.Name = "gridComplete";
			this.gridComplete.ShowContextMenu = false;
			this.gridComplete.Size = new System.Drawing.Size(484, 107);
			this.gridComplete.TabIndex = 325;
			this.gridComplete.Title = "Complete";
			this.gridComplete.TranslationName = "FormJobManager";
			this.gridComplete.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// gridDefinition
			// 
			this.gridDefinition.AutoSize = true;
			this.gridDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridDefinition.HScrollVisible = true;
			this.gridDefinition.Location = new System.Drawing.Point(493, 3);
			this.gridDefinition.Name = "gridDefinition";
			this.gridDefinition.ShowContextMenu = false;
			this.gridDefinition.Size = new System.Drawing.Size(484, 107);
			this.gridDefinition.TabIndex = 317;
			this.gridDefinition.Title = "Definition";
			this.gridDefinition.TranslationName = "FormJobManager";
			this.gridDefinition.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// gridDevelopment
			// 
			this.gridDevelopment.AutoSize = true;
			this.gridDevelopment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridDevelopment.HScrollVisible = true;
			this.gridDevelopment.Location = new System.Drawing.Point(983, 3);
			this.gridDevelopment.Name = "gridDevelopment";
			this.gridDevelopment.ShowContextMenu = false;
			this.gridDevelopment.Size = new System.Drawing.Size(484, 107);
			this.gridDevelopment.TabIndex = 318;
			this.gridDevelopment.Title = "Development";
			this.gridDevelopment.TranslationName = "FormJobManager";
			this.gridDevelopment.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// gridDocumentation
			// 
			this.gridDocumentation.AutoSize = true;
			this.gridDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridDocumentation.HScrollVisible = true;
			this.gridDocumentation.Location = new System.Drawing.Point(493, 116);
			this.gridDocumentation.Name = "gridDocumentation";
			this.gridDocumentation.ShowContextMenu = false;
			this.gridDocumentation.Size = new System.Drawing.Size(484, 107);
			this.gridDocumentation.TabIndex = 324;
			this.gridDocumentation.Title = "Documentation";
			this.gridDocumentation.TranslationName = "FormJobManager";
			this.gridDocumentation.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// gridPendingReview
			// 
			this.gridPendingReview.AutoSize = true;
			this.gridPendingReview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridPendingReview.HScrollVisible = true;
			this.gridPendingReview.Location = new System.Drawing.Point(3, 116);
			this.gridPendingReview.Name = "gridPendingReview";
			this.gridPendingReview.ShowContextMenu = false;
			this.gridPendingReview.Size = new System.Drawing.Size(484, 107);
			this.gridPendingReview.TabIndex = 323;
			this.gridPendingReview.Title = "Pending Review";
			this.gridPendingReview.TranslationName = "FormJobManager";
			this.gridPendingReview.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJob_CellDoubleClick);
			// 
			// labelJobTeam
			// 
			this.labelJobTeam.Location = new System.Drawing.Point(5, 309);
			this.labelJobTeam.Name = "labelJobTeam";
			this.labelJobTeam.Size = new System.Drawing.Size(91, 16);
			this.labelJobTeam.TabIndex = 338;
			this.labelJobTeam.Text = "Team";
			this.labelJobTeam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textJobNum
			// 
			this.textJobNum.Location = new System.Drawing.Point(96, 28);
			this.textJobNum.MaxLength = 100;
			this.textJobNum.Name = "textJobNum";
			this.textJobNum.ReadOnly = true;
			this.textJobNum.Size = new System.Drawing.Size(117, 20);
			this.textJobNum.TabIndex = 337;
			this.textJobNum.TabStop = false;
			// 
			// labelJobNum
			// 
			this.labelJobNum.Location = new System.Drawing.Point(19, 30);
			this.labelJobNum.Name = "labelJobNum";
			this.labelJobNum.Size = new System.Drawing.Size(78, 16);
			this.labelJobNum.TabIndex = 336;
			this.labelJobNum.Text = "JobNum";
			this.labelJobNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProjectManager
			// 
			this.textProjectManager.Location = new System.Drawing.Point(96, 71);
			this.textProjectManager.MaxLength = 100;
			this.textProjectManager.Name = "textProjectManager";
			this.textProjectManager.ReadOnly = true;
			this.textProjectManager.Size = new System.Drawing.Size(117, 20);
			this.textProjectManager.TabIndex = 344;
			this.textProjectManager.TabStop = false;
			// 
			// labelProjectManager
			// 
			this.labelProjectManager.Location = new System.Drawing.Point(6, 72);
			this.labelProjectManager.Name = "labelProjectManager";
			this.labelProjectManager.Size = new System.Drawing.Size(90, 16);
			this.labelProjectManager.TabIndex = 343;
			this.labelProjectManager.Text = "Project Manager";
			this.labelProjectManager.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGitBranchName
			// 
			this.textGitBranchName.Location = new System.Drawing.Point(96, 286);
			this.textGitBranchName.MaxLength = 100;
			this.textGitBranchName.Name = "textGitBranchName";
			this.textGitBranchName.ReadOnly = true;
			this.textGitBranchName.Size = new System.Drawing.Size(117, 20);
			this.textGitBranchName.TabIndex = 346;
			this.textGitBranchName.TabStop = false;
			// 
			// labelGitBranchName
			// 
			this.labelGitBranchName.Location = new System.Drawing.Point(5, 286);
			this.labelGitBranchName.Name = "labelGitBranchName";
			this.labelGitBranchName.Size = new System.Drawing.Size(91, 16);
			this.labelGitBranchName.TabIndex = 345;
			this.labelGitBranchName.Text = "Git Branch Name";
			this.labelGitBranchName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPriority
			// 
			this.labelPriority.Location = new System.Drawing.Point(37, 159);
			this.labelPriority.Name = "labelPriority";
			this.labelPriority.Size = new System.Drawing.Size(57, 16);
			this.labelPriority.TabIndex = 347;
			this.labelPriority.Text = "Priority";
			this.labelPriority.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPhase
			// 
			this.labelPhase.Location = new System.Drawing.Point(26, 136);
			this.labelPhase.Name = "labelPhase";
			this.labelPhase.Size = new System.Drawing.Size(67, 16);
			this.labelPhase.TabIndex = 349;
			this.labelPhase.Text = "Phase";
			this.labelPhase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateEntry
			// 
			this.labelDateEntry.Location = new System.Drawing.Point(33, 51);
			this.labelDateEntry.Name = "labelDateEntry";
			this.labelDateEntry.Size = new System.Drawing.Size(63, 16);
			this.labelDateEntry.TabIndex = 353;
			this.labelDateEntry.Text = "Date Entry";
			this.labelDateEntry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(96, 49);
			this.textDateEntry.MaxLength = 100;
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(117, 20);
			this.textDateEntry.TabIndex = 354;
			this.textDateEntry.TabStop = false;
			// 
			// gridDiscussion
			// 
			this.gridDiscussion.AutoSize = true;
			this.gridDiscussion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridDiscussion.HasAddButton = true;
			this.gridDiscussion.Location = new System.Drawing.Point(3, 474);
			this.gridDiscussion.Name = "gridDiscussion";
			this.gridDiscussion.Size = new System.Drawing.Size(1244, 154);
			this.gridDiscussion.TabIndex = 327;
			this.gridDiscussion.Title = "Discussion";
			this.gridDiscussion.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridDiscussion_CellDoubleClick);
			this.gridDiscussion.TitleAddClick += new System.EventHandler(this.gridDiscussion_TitleAddClick);
			// 
			// gridProjects
			// 
			this.gridProjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridProjects.Location = new System.Drawing.Point(3, 38);
			this.gridProjects.Name = "gridProjects";
			this.gridProjects.ShowContextMenu = false;
			this.gridProjects.Size = new System.Drawing.Size(1244, 242);
			this.gridProjects.TabIndex = 241;
			this.gridProjects.Title = "Projects";
			this.gridProjects.TranslationName = "FormJobManager";
			this.gridProjects.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProjects_CellDoubleClick);
			this.gridProjects.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProjects_CellClick);
			// 
			// labelProjectDescription
			// 
			this.labelProjectDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelProjectDescription.Location = new System.Drawing.Point(3, 283);
			this.labelProjectDescription.Name = "labelProjectDescription";
			this.labelProjectDescription.Size = new System.Drawing.Size(1244, 26);
			this.labelProjectDescription.TabIndex = 357;
			this.labelProjectDescription.Text = "Project Description";
			this.labelProjectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(32, 265);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(63, 16);
			this.labelVersion.TabIndex = 359;
			this.labelVersion.Text = "Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVersion
			// 
			this.textVersion.Location = new System.Drawing.Point(95, 264);
			this.textVersion.MaxLength = 100;
			this.textVersion.Name = "textVersion";
			this.textVersion.Size = new System.Drawing.Size(92, 20);
			this.textVersion.TabIndex = 360;
			this.textVersion.TextChanged += new System.EventHandler(this.textVersion_TextChanged);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.gridDiscussion, 0, 4);
			this.tableLayoutPanel2.Controls.Add(this.textEditorProjectDescription, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.gridProjects, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.labelProjectDescription, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(220, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 5;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.59542F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.40458F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 162F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 159F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1250, 631);
			this.tableLayoutPanel2.TabIndex = 364;
			// 
			// textEditorProjectDescription
			// 
			this.textEditorProjectDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEditorProjectDescription.HasEditorOptions = true;
			this.textEditorProjectDescription.HasSaveButton = true;
			this.textEditorProjectDescription.Location = new System.Drawing.Point(3, 312);
			this.textEditorProjectDescription.MainFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textEditorProjectDescription.MainRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 " +
    "Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.22621}\\viewkind4\\uc1 \r\n\\par" +
    "d\\f0\\fs17\\par\r\n}\r\n";
			this.textEditorProjectDescription.MainText = "";
			this.textEditorProjectDescription.MinimumSize = new System.Drawing.Size(450, 120);
			this.textEditorProjectDescription.Name = "textEditorProjectDescription";
			this.textEditorProjectDescription.ReadOnly = false;
			this.textEditorProjectDescription.Size = new System.Drawing.Size(1244, 156);
			this.textEditorProjectDescription.TabIndex = 333;
			this.textEditorProjectDescription.SaveClick += new OpenDental.ODtextEditorSaveEventHandler(this.textEditor_SaveClick);
			this.textEditorProjectDescription.OnTextEdited += new OpenDental.OdtextEditor.textChangedEventHandler(this.textEditor_OnTextEdited);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1208F));
			this.tableLayoutPanel3.Controls.Add(this.textTitle, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.labelTitle, 0, 0);
			this.tableLayoutPanel3.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(1243, 26);
			this.tableLayoutPanel3.TabIndex = 365;
			// 
			// textTitle
			// 
			this.textTitle.AcceptsTab = true;
			this.textTitle.BackColor = System.Drawing.SystemColors.Window;
			this.textTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textTitle.DetectLinksEnabled = false;
			this.textTitle.DetectUrls = false;
			this.textTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textTitle.Location = new System.Drawing.Point(38, 3);
			this.textTitle.Multiline = false;
			this.textTitle.Name = "textTitle";
			this.textTitle.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.JobManager;
			this.textTitle.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTitle.Size = new System.Drawing.Size(1202, 20);
			this.textTitle.TabIndex = 309;
			this.textTitle.Text = "";
			this.textTitle.WordWrap = false;
			this.textTitle.TextChanged += new System.EventHandler(this.textTitle_TextChanged);
			// 
			// textSubmitter
			// 
			this.textSubmitter.Location = new System.Drawing.Point(96, 91);
			this.textSubmitter.MaxLength = 100;
			this.textSubmitter.Name = "textSubmitter";
			this.textSubmitter.ReadOnly = true;
			this.textSubmitter.Size = new System.Drawing.Size(117, 20);
			this.textSubmitter.TabIndex = 366;
			this.textSubmitter.TabStop = false;
			// 
			// labelSubmitter
			// 
			this.labelSubmitter.Location = new System.Drawing.Point(5, 93);
			this.labelSubmitter.Name = "labelSubmitter";
			this.labelSubmitter.Size = new System.Drawing.Size(90, 16);
			this.labelSubmitter.TabIndex = 365;
			this.labelSubmitter.Text = "Submitter";
			this.labelSubmitter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHoursEstimate
			// 
			this.labelHoursEstimate.Location = new System.Drawing.Point(45, 177);
			this.labelHoursEstimate.Name = "labelHoursEstimate";
			this.labelHoursEstimate.Size = new System.Drawing.Size(49, 20);
			this.labelHoursEstimate.TabIndex = 367;
			this.labelHoursEstimate.Text = "Hrs. Est.";
			this.labelHoursEstimate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHoursEstimateSum
			// 
			this.labelHoursEstimateSum.Location = new System.Drawing.Point(8, 198);
			this.labelHoursEstimateSum.Name = "labelHoursEstimateSum";
			this.labelHoursEstimateSum.Size = new System.Drawing.Size(86, 20);
			this.labelHoursEstimateSum.TabIndex = 368;
			this.labelHoursEstimateSum.Text = "Hrs. Est Sum";
			this.labelHoursEstimateSum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHoursLeftSum
			// 
			this.labelHoursLeftSum.Location = new System.Drawing.Point(11, 243);
			this.labelHoursLeftSum.Name = "labelHoursLeftSum";
			this.labelHoursLeftSum.Size = new System.Drawing.Size(83, 20);
			this.labelHoursLeftSum.TabIndex = 372;
			this.labelHoursLeftSum.Text = "Hrs. Left Sum";
			this.labelHoursLeftSum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHoursActual
			// 
			this.labelHoursActual.Location = new System.Drawing.Point(11, 221);
			this.labelHoursActual.Name = "labelHoursActual";
			this.labelHoursActual.Size = new System.Drawing.Size(83, 20);
			this.labelHoursActual.TabIndex = 375;
			this.labelHoursActual.Text = "Hrs. Actual Sum";
			this.labelHoursActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCheckedOut
			// 
			this.textCheckedOut.Location = new System.Drawing.Point(95, 112);
			this.textCheckedOut.MaxLength = 100;
			this.textCheckedOut.Name = "textCheckedOut";
			this.textCheckedOut.ReadOnly = true;
			this.textCheckedOut.Size = new System.Drawing.Size(117, 20);
			this.textCheckedOut.TabIndex = 378;
			this.textCheckedOut.TabStop = false;
			// 
			// labelCheckedOut
			// 
			this.labelCheckedOut.Location = new System.Drawing.Point(4, 114);
			this.labelCheckedOut.Name = "labelCheckedOut";
			this.labelCheckedOut.Size = new System.Drawing.Size(90, 16);
			this.labelCheckedOut.TabIndex = 377;
			this.labelCheckedOut.Text = "Checked Out";
			this.labelCheckedOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// timerTitle
			// 
			this.timerTitle.Interval = 3000;
			this.timerTitle.Tick += new System.EventHandler(this.timerTitle_Tick);
			// 
			// timerVersion
			// 
			this.timerVersion.Interval = 3000;
			this.timerVersion.Tick += new System.EventHandler(this.timerVersion_Tick);
			// 
			// comboPriority
			// 
			this.comboPriority.FormattingEnabled = true;
			this.comboPriority.IntegralHeight = false;
			this.comboPriority.Location = new System.Drawing.Point(95, 155);
			this.comboPriority.MaxDropDownItems = 10;
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(117, 21);
			this.comboPriority.TabIndex = 348;
			this.comboPriority.SelectionChangeCommitted += new System.EventHandler(this.comboPriority_SelectionChangeCommitted);
			this.comboPriority.Leave += new System.EventHandler(this.comboPriority_Leave);
			// 
			// butAddExistingJob
			// 
			this.butAddExistingJob.Location = new System.Drawing.Point(8, 334);
			this.butAddExistingJob.Name = "butAddExistingJob";
			this.butAddExistingJob.Size = new System.Drawing.Size(204, 22);
			this.butAddExistingJob.TabIndex = 322;
			this.butAddExistingJob.Text = "Add Existing Job to Project";
			this.butAddExistingJob.Click += new System.EventHandler(this.butAddExistingJob_Click);
			// 
			// textHoursActualDescendants
			// 
			this.textHoursActualDescendants.Location = new System.Drawing.Point(95, 220);
			this.textHoursActualDescendants.MaxVal = 1000000D;
			this.textHoursActualDescendants.MinVal = 0D;
			this.textHoursActualDescendants.Name = "textHoursActualDescendants";
			this.textHoursActualDescendants.ReadOnly = true;
			this.textHoursActualDescendants.Size = new System.Drawing.Size(44, 20);
			this.textHoursActualDescendants.TabIndex = 376;
			// 
			// butChangeEst
			// 
			this.butChangeEst.Location = new System.Drawing.Point(141, 177);
			this.butChangeEst.Name = "butChangeEst";
			this.butChangeEst.Size = new System.Drawing.Size(71, 20);
			this.butChangeEst.TabIndex = 374;
			this.butChangeEst.Text = "Change Est.";
			this.butChangeEst.Click += new System.EventHandler(this.butChangeEst_Click);
			// 
			// textHoursLeftDescendants
			// 
			this.textHoursLeftDescendants.Location = new System.Drawing.Point(95, 242);
			this.textHoursLeftDescendants.MaxVal = 1000000D;
			this.textHoursLeftDescendants.MinVal = 0D;
			this.textHoursLeftDescendants.Name = "textHoursLeftDescendants";
			this.textHoursLeftDescendants.ReadOnly = true;
			this.textHoursLeftDescendants.Size = new System.Drawing.Size(44, 20);
			this.textHoursLeftDescendants.TabIndex = 373;
			// 
			// butCalculateSums
			// 
			this.butCalculateSums.Location = new System.Drawing.Point(141, 198);
			this.butCalculateSums.Name = "butCalculateSums";
			this.butCalculateSums.Size = new System.Drawing.Size(71, 20);
			this.butCalculateSums.TabIndex = 371;
			this.butCalculateSums.Text = "Calculate";
			this.butCalculateSums.Click += new System.EventHandler(this.butCalculateSums_Click);
			// 
			// textEstHours
			// 
			this.textEstHours.Location = new System.Drawing.Point(95, 177);
			this.textEstHours.MaxVal = 1000000D;
			this.textEstHours.MinVal = 0D;
			this.textEstHours.Name = "textEstHours";
			this.textEstHours.ReadOnly = true;
			this.textEstHours.Size = new System.Drawing.Size(44, 20);
			this.textEstHours.TabIndex = 369;
			// 
			// textHoursEstDescendants
			// 
			this.textHoursEstDescendants.Location = new System.Drawing.Point(95, 198);
			this.textHoursEstDescendants.MaxVal = 1000000D;
			this.textHoursEstDescendants.MinVal = 0D;
			this.textHoursEstDescendants.Name = "textHoursEstDescendants";
			this.textHoursEstDescendants.ReadOnly = true;
			this.textHoursEstDescendants.Size = new System.Drawing.Size(44, 20);
			this.textHoursEstDescendants.TabIndex = 370;
			// 
			// butParentPick
			// 
			this.butParentPick.Location = new System.Drawing.Point(8, 614);
			this.butParentPick.Name = "butParentPick";
			this.butParentPick.Size = new System.Drawing.Size(75, 20);
			this.butParentPick.TabIndex = 363;
			this.butParentPick.Text = "Set Parent";
			this.butParentPick.Click += new System.EventHandler(this.butParentPick_Click);
			// 
			// butParentRemove
			// 
			this.butParentRemove.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butParentRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butParentRemove.Location = new System.Drawing.Point(89, 614);
			this.butParentRemove.Name = "butParentRemove";
			this.butParentRemove.Size = new System.Drawing.Size(123, 20);
			this.butParentRemove.TabIndex = 362;
			this.butParentRemove.Text = "Remove parent";
			this.butParentRemove.Click += new System.EventHandler(this.butParentRemove_Click);
			// 
			// butVersionPrompt
			// 
			this.butVersionPrompt.Location = new System.Drawing.Point(190, 264);
			this.butVersionPrompt.Name = "butVersionPrompt";
			this.butVersionPrompt.Size = new System.Drawing.Size(23, 20);
			this.butVersionPrompt.TabIndex = 361;
			this.butVersionPrompt.Text = "...";
			this.butVersionPrompt.Click += new System.EventHandler(this.butVersionPrompt_Click);
			// 
			// groupGridFilters
			// 
			this.groupGridFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.groupGridFilters.Controls.Add(this.checkShowAllChildJobs);
			this.groupGridFilters.Controls.Add(this.checkIncludeCancelled);
			this.groupGridFilters.Controls.Add(this.checkIncludeComplete);
			this.groupGridFilters.Location = new System.Drawing.Point(8, 528);
			this.groupGridFilters.Name = "groupGridFilters";
			this.groupGridFilters.Size = new System.Drawing.Size(204, 83);
			this.groupGridFilters.TabIndex = 358;
			this.groupGridFilters.Text = "Grid Filters";
			// 
			// checkShowAllChildJobs
			// 
			this.checkShowAllChildJobs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAllChildJobs.Location = new System.Drawing.Point(6, 55);
			this.checkShowAllChildJobs.Name = "checkShowAllChildJobs";
			this.checkShowAllChildJobs.Size = new System.Drawing.Size(150, 18);
			this.checkShowAllChildJobs.TabIndex = 328;
			this.checkShowAllChildJobs.Text = "Show all descendent jobs";
			this.checkShowAllChildJobs.Click += new System.EventHandler(this.checkShowAllChildJobs_Click);
			// 
			// checkIncludeCancelled
			// 
			this.checkIncludeCancelled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeCancelled.Location = new System.Drawing.Point(42, 35);
			this.checkIncludeCancelled.Name = "checkIncludeCancelled";
			this.checkIncludeCancelled.Size = new System.Drawing.Size(114, 18);
			this.checkIncludeCancelled.TabIndex = 327;
			this.checkIncludeCancelled.Text = "Include Cancelled";
			this.checkIncludeCancelled.CheckedChanged += new System.EventHandler(this.checkIncludeCancelled_CheckedChanged);
			// 
			// checkIncludeComplete
			// 
			this.checkIncludeComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeComplete.Location = new System.Drawing.Point(42, 17);
			this.checkIncludeComplete.Name = "checkIncludeComplete";
			this.checkIncludeComplete.Size = new System.Drawing.Size(114, 18);
			this.checkIncludeComplete.TabIndex = 326;
			this.checkIncludeComplete.Text = "Include Complete";
			this.checkIncludeComplete.CheckedChanged += new System.EventHandler(this.checkIncludeComplete_CheckedChanged);
			// 
			// butActions
			// 
			this.butActions.Image = global::OpenDental.Properties.Resources.downArrowWinForm;
			this.butActions.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butActions.Location = new System.Drawing.Point(3, 3);
			this.butActions.Name = "butActions";
			this.butActions.Size = new System.Drawing.Size(106, 24);
			this.butActions.TabIndex = 356;
			this.butActions.Text = "Project Actions";
			this.butActions.Click += new System.EventHandler(this.butActions_Click);
			// 
			// checkIsActive
			// 
			this.checkIsActive.Location = new System.Drawing.Point(147, 3);
			this.checkIsActive.Name = "checkIsActive";
			this.checkIsActive.Size = new System.Drawing.Size(80, 24);
			this.checkIsActive.TabIndex = 355;
			this.checkIsActive.Text = "Is Active";
			// 
			// comboPhase
			// 
			this.comboPhase.Location = new System.Drawing.Point(95, 133);
			this.comboPhase.Name = "comboPhase";
			this.comboPhase.Size = new System.Drawing.Size(117, 21);
			this.comboPhase.TabIndex = 350;
			this.comboPhase.SelectionChangeCommitted += new System.EventHandler(this.comboPhase_SelectionChangeCommitted);
			// 
			// comboJobTeam
			// 
			this.comboJobTeam.Location = new System.Drawing.Point(96, 307);
			this.comboJobTeam.Name = "comboJobTeam";
			this.comboJobTeam.Size = new System.Drawing.Size(117, 21);
			this.comboJobTeam.TabIndex = 339;
			this.comboJobTeam.SelectionChangeCommitted += new System.EventHandler(this.comboJobTeam_SelectionChangeCommitted);
			// 
			// UserControlProjectEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.butAddExistingJob);
			this.Controls.Add(this.textCheckedOut);
			this.Controls.Add(this.labelCheckedOut);
			this.Controls.Add(this.textHoursActualDescendants);
			this.Controls.Add(this.labelHoursActual);
			this.Controls.Add(this.butChangeEst);
			this.Controls.Add(this.labelHoursEstimate);
			this.Controls.Add(this.textHoursLeftDescendants);
			this.Controls.Add(this.labelHoursEstimateSum);
			this.Controls.Add(this.butCalculateSums);
			this.Controls.Add(this.textEstHours);
			this.Controls.Add(this.textHoursEstDescendants);
			this.Controls.Add(this.labelHoursLeftSum);
			this.Controls.Add(this.textSubmitter);
			this.Controls.Add(this.labelSubmitter);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.butParentPick);
			this.Controls.Add(this.butParentRemove);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.textVersion);
			this.Controls.Add(this.butVersionPrompt);
			this.Controls.Add(this.groupGridFilters);
			this.Controls.Add(this.butActions);
			this.Controls.Add(this.checkIsActive);
			this.Controls.Add(this.labelDateEntry);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.comboPhase);
			this.Controls.Add(this.labelPhase);
			this.Controls.Add(this.comboPriority);
			this.Controls.Add(this.labelPriority);
			this.Controls.Add(this.textGitBranchName);
			this.Controls.Add(this.labelGitBranchName);
			this.Controls.Add(this.textProjectManager);
			this.Controls.Add(this.labelProjectManager);
			this.Controls.Add(this.comboJobTeam);
			this.Controls.Add(this.labelJobTeam);
			this.Controls.Add(this.textJobNum);
			this.Controls.Add(this.labelJobNum);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "UserControlProjectEdit";
			this.Size = new System.Drawing.Size(1476, 865);
			this.Leave += new System.EventHandler(this.comboPriority_Leave);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.groupGridFilters.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelTitle;
		private ODtextBox textTitle;
		private UI.GridOD gridConcept;
		private UI.GridOD gridDefinition;
		private UI.GridOD gridDevelopment;
		private UI.Button butAddExistingJob;
		private UI.GridOD gridComplete;
		private UI.GridOD gridDocumentation;
		private UI.GridOD gridPendingReview;
		private UI.CheckBox checkIncludeComplete;
		private UI.GridOD gridProjects;
		private UI.GridOD gridDiscussion;
		private OdtextEditor textEditorProjectDescription;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private UI.ComboBox comboJobTeam;
		private System.Windows.Forms.Label labelJobTeam;
		private System.Windows.Forms.TextBox textJobNum;
		private System.Windows.Forms.Label labelJobNum;
		private System.Windows.Forms.TextBox textProjectManager;
		private System.Windows.Forms.Label labelProjectManager;
		private System.Windows.Forms.TextBox textGitBranchName;
		private System.Windows.Forms.Label labelGitBranchName;
		private System.Windows.Forms.ComboBox comboPriority;
		private System.Windows.Forms.Label labelPriority;
		private UI.ComboBox comboPhase;
		private System.Windows.Forms.Label labelPhase;
		private System.Windows.Forms.Label labelDateEntry;
		private System.Windows.Forms.TextBox textDateEntry;
		private UI.CheckBox checkIsActive;
		private UI.Button butActions;
		private System.Windows.Forms.Label labelProjectDescription;
		private UI.GroupBox groupGridFilters;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.TextBox textVersion;
		private UI.Button butVersionPrompt;
		private UI.Button butParentPick;
		private UI.Button butParentRemove;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TextBox textSubmitter;
		private System.Windows.Forms.Label labelSubmitter;
		private UI.CheckBox checkIncludeCancelled;
		private UI.Button butChangeEst;
		private System.Windows.Forms.Label labelHoursEstimate;
		private ValidDouble textHoursLeftDescendants;
		private System.Windows.Forms.Label labelHoursEstimateSum;
		private UI.Button butCalculateSums;
		private ValidDouble textEstHours;
		private ValidDouble textHoursEstDescendants;
		private System.Windows.Forms.Label labelHoursLeftSum;
		private ValidDouble textHoursActualDescendants;
		private System.Windows.Forms.Label labelHoursActual;
		private System.Windows.Forms.TextBox textCheckedOut;
		private System.Windows.Forms.Label labelCheckedOut;
		private System.Windows.Forms.Timer timerTitle;
		private System.Windows.Forms.Timer timerVersion;
		private UI.CheckBox checkShowAllChildJobs;
	}
}
