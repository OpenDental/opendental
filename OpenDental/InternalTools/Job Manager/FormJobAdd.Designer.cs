namespace OpenDental{
	partial class FormJobAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobAdd));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.tabControlExtra = new System.Windows.Forms.TabControl();
			this.tabConcept = new System.Windows.Forms.TabPage();
			this.textConcept = new OpenDental.OdtextEditor();
			this.tabCustomers = new System.Windows.Forms.TabPage();
			this.gridCustomers = new OpenDental.UI.GridOD();
			this.tabSubscribers = new System.Windows.Forms.TabPage();
			this.gridSubscribers = new OpenDental.UI.GridOD();
			this.tabRequests = new System.Windows.Forms.TabPage();
			this.gridFeatureReq = new OpenDental.UI.GridOD();
			this.tabBugs = new System.Windows.Forms.TabPage();
			this.gridBugs = new OpenDental.UI.GridOD();
			this.tabTasks = new System.Windows.Forms.TabPage();
			this.gridTasks = new OpenDental.UI.GridOD();
			this.tabQuotes = new System.Windows.Forms.TabPage();
			this.gridQuotes = new OpenDental.UI.GridOD();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.labelVersion = new System.Windows.Forms.Label();
			this.comboProposedVersion = new System.Windows.Forms.ComboBox();
			this.labelPriority = new System.Windows.Forms.Label();
			this.comboPriority = new OpenDental.UI.ComboBoxOD();
			this.comboCategory = new System.Windows.Forms.ComboBox();
			this.labelCategory = new System.Windows.Forms.Label();
			this.tabControlExtra.SuspendLayout();
			this.tabConcept.SuspendLayout();
			this.tabCustomers.SuspendLayout();
			this.tabSubscribers.SuspendLayout();
			this.tabRequests.SuspendLayout();
			this.tabBugs.SuspendLayout();
			this.tabTasks.SuspendLayout();
			this.tabQuotes.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(395, 2);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 22);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(476, 2);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(67, 22);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// tabControlExtra
			// 
			this.tabControlExtra.Controls.Add(this.tabConcept);
			this.tabControlExtra.Controls.Add(this.tabCustomers);
			this.tabControlExtra.Controls.Add(this.tabSubscribers);
			this.tabControlExtra.Controls.Add(this.tabRequests);
			this.tabControlExtra.Controls.Add(this.tabBugs);
			this.tabControlExtra.Controls.Add(this.tabTasks);
			this.tabControlExtra.Controls.Add(this.tabQuotes);
			this.tabControlExtra.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlExtra.HotTrack = true;
			this.tabControlExtra.Location = new System.Drawing.Point(3, 93);
			this.tabControlExtra.Name = "tabControlExtra";
			this.tabControlExtra.SelectedIndex = 0;
			this.tabControlExtra.Size = new System.Drawing.Size(553, 380);
			this.tabControlExtra.TabIndex = 4;
			// 
			// tabConcept
			// 
			this.tabConcept.Controls.Add(this.textConcept);
			this.tabConcept.Location = new System.Drawing.Point(4, 22);
			this.tabConcept.Name = "tabConcept";
			this.tabConcept.Padding = new System.Windows.Forms.Padding(3);
			this.tabConcept.Size = new System.Drawing.Size(545, 354);
			this.tabConcept.TabIndex = 0;
			this.tabConcept.Text = "Concept";
			this.tabConcept.UseVisualStyleBackColor = true;
			// 
			// textConcept
			// 
			this.textConcept.BackColor = System.Drawing.SystemColors.Control;
			this.textConcept.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textConcept.HasEditorOptions = true;
			this.textConcept.HasSaveButton = false;
			this.textConcept.Location = new System.Drawing.Point(3, 3);
			this.textConcept.MainFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textConcept.MainRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 " +
    "Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.19041}\\viewkind4\\uc1 \r\n\\par" +
    "d\\f0\\fs17\\par\r\n}\r\n";
			this.textConcept.MainText = "";
			this.textConcept.Name = "textConcept";
			this.textConcept.ReadOnly = false;
			this.textConcept.Size = new System.Drawing.Size(539, 348);
			this.textConcept.TabIndex = 0;
			// 
			// tabCustomers
			// 
			this.tabCustomers.Controls.Add(this.gridCustomers);
			this.tabCustomers.Location = new System.Drawing.Point(4, 22);
			this.tabCustomers.Name = "tabCustomers";
			this.tabCustomers.Padding = new System.Windows.Forms.Padding(3);
			this.tabCustomers.Size = new System.Drawing.Size(545, 354);
			this.tabCustomers.TabIndex = 1;
			this.tabCustomers.Text = "Customers";
			this.tabCustomers.UseVisualStyleBackColor = true;
			// 
			// gridCustomers
			// 
			this.gridCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridCustomers.HasAddButton = true;
			this.gridCustomers.Location = new System.Drawing.Point(3, 3);
			this.gridCustomers.Name = "gridCustomers";
			this.gridCustomers.Size = new System.Drawing.Size(539, 348);
			this.gridCustomers.TabIndex = 263;
			this.gridCustomers.Title = "Customers";
			this.gridCustomers.TranslationName = "FormTaskEdit";
			this.gridCustomers.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCustomers_CellClick);
			this.gridCustomers.TitleAddClick += new System.EventHandler(this.gridCustomers_TitleAddClick);
			// 
			// tabSubscribers
			// 
			this.tabSubscribers.Controls.Add(this.gridSubscribers);
			this.tabSubscribers.Location = new System.Drawing.Point(4, 22);
			this.tabSubscribers.Name = "tabSubscribers";
			this.tabSubscribers.Padding = new System.Windows.Forms.Padding(3);
			this.tabSubscribers.Size = new System.Drawing.Size(545, 354);
			this.tabSubscribers.TabIndex = 2;
			this.tabSubscribers.Text = "Subscribers";
			this.tabSubscribers.UseVisualStyleBackColor = true;
			// 
			// gridSubscribers
			// 
			this.gridSubscribers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridSubscribers.HasAddButton = true;
			this.gridSubscribers.Location = new System.Drawing.Point(3, 3);
			this.gridSubscribers.Name = "gridSubscribers";
			this.gridSubscribers.Size = new System.Drawing.Size(539, 348);
			this.gridSubscribers.TabIndex = 226;
			this.gridSubscribers.Title = "Subscribers";
			this.gridSubscribers.TranslationName = "FormTaskEdit";
			this.gridSubscribers.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubscribers_CellClick);
			this.gridSubscribers.TitleAddClick += new System.EventHandler(this.gridSubscribers_TitleAddClick);
			// 
			// tabRequests
			// 
			this.tabRequests.Controls.Add(this.gridFeatureReq);
			this.tabRequests.Location = new System.Drawing.Point(4, 22);
			this.tabRequests.Name = "tabRequests";
			this.tabRequests.Padding = new System.Windows.Forms.Padding(3);
			this.tabRequests.Size = new System.Drawing.Size(545, 354);
			this.tabRequests.TabIndex = 3;
			this.tabRequests.Text = "Requests";
			this.tabRequests.UseVisualStyleBackColor = true;
			// 
			// gridFeatureReq
			// 
			this.gridFeatureReq.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridFeatureReq.HasAddButton = true;
			this.gridFeatureReq.Location = new System.Drawing.Point(3, 3);
			this.gridFeatureReq.Name = "gridFeatureReq";
			this.gridFeatureReq.Size = new System.Drawing.Size(539, 348);
			this.gridFeatureReq.TabIndex = 229;
			this.gridFeatureReq.Title = "Feature Requests";
			this.gridFeatureReq.TranslationName = "FormTaskEdit";
			this.gridFeatureReq.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFeatureReq_CellClick);
			this.gridFeatureReq.TitleAddClick += new System.EventHandler(this.gridFeatureReq_TitleAddClick);
			// 
			// tabBugs
			// 
			this.tabBugs.Controls.Add(this.gridBugs);
			this.tabBugs.Location = new System.Drawing.Point(4, 22);
			this.tabBugs.Name = "tabBugs";
			this.tabBugs.Padding = new System.Windows.Forms.Padding(3);
			this.tabBugs.Size = new System.Drawing.Size(545, 354);
			this.tabBugs.TabIndex = 4;
			this.tabBugs.Text = "Bugs";
			this.tabBugs.UseVisualStyleBackColor = true;
			// 
			// gridBugs
			// 
			this.gridBugs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridBugs.HasAddButton = true;
			this.gridBugs.Location = new System.Drawing.Point(3, 3);
			this.gridBugs.Name = "gridBugs";
			this.gridBugs.Size = new System.Drawing.Size(539, 348);
			this.gridBugs.TabIndex = 261;
			this.gridBugs.Title = "Bugs/Enhancements";
			this.gridBugs.TranslationName = "FormTaskEdit";
			this.gridBugs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBugs_CellDoubleClick);
			this.gridBugs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBugs_CellClick);
			this.gridBugs.TitleAddClick += new System.EventHandler(this.gridBugs_TitleAddClick);
			// 
			// tabTasks
			// 
			this.tabTasks.Controls.Add(this.gridTasks);
			this.tabTasks.Location = new System.Drawing.Point(4, 22);
			this.tabTasks.Name = "tabTasks";
			this.tabTasks.Padding = new System.Windows.Forms.Padding(3);
			this.tabTasks.Size = new System.Drawing.Size(545, 354);
			this.tabTasks.TabIndex = 5;
			this.tabTasks.Text = "Tasks";
			this.tabTasks.UseVisualStyleBackColor = true;
			// 
			// gridTasks
			// 
			this.gridTasks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridTasks.HasAddButton = true;
			this.gridTasks.Location = new System.Drawing.Point(3, 3);
			this.gridTasks.Name = "gridTasks";
			this.gridTasks.Size = new System.Drawing.Size(539, 348);
			this.gridTasks.TabIndex = 228;
			this.gridTasks.Title = "Tasks";
			this.gridTasks.TranslationName = "FormTaskEdit";
			this.gridTasks.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellDoubleClick);
			this.gridTasks.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTasks_CellClick);
			this.gridTasks.TitleAddClick += new System.EventHandler(this.gridTasks_TitleAddClick);
			// 
			// tabQuotes
			// 
			this.tabQuotes.Controls.Add(this.gridQuotes);
			this.tabQuotes.Location = new System.Drawing.Point(4, 22);
			this.tabQuotes.Name = "tabQuotes";
			this.tabQuotes.Padding = new System.Windows.Forms.Padding(3);
			this.tabQuotes.Size = new System.Drawing.Size(545, 354);
			this.tabQuotes.TabIndex = 6;
			this.tabQuotes.Text = "Quotes";
			this.tabQuotes.UseVisualStyleBackColor = true;
			// 
			// gridQuotes
			// 
			this.gridQuotes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridQuotes.HasAddButton = true;
			this.gridQuotes.Location = new System.Drawing.Point(3, 3);
			this.gridQuotes.Name = "gridQuotes";
			this.gridQuotes.Size = new System.Drawing.Size(539, 348);
			this.gridQuotes.TabIndex = 227;
			this.gridQuotes.Title = "Quotes";
			this.gridQuotes.TranslationName = "FormTaskEdit";
			this.gridQuotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQuotes_CellDoubleClick);
			this.gridQuotes.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQuotes_CellClick);
			this.gridQuotes.TitleAddClick += new System.EventHandler(this.gridCustomerQuotes_TitleAddClick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tabControlExtra, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(559, 509);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.butOK);
			this.panel1.Controls.Add(this.butCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 479);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(553, 27);
			this.panel1.TabIndex = 5;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(553, 84);
			this.tableLayoutPanel2.TabIndex = 6;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textTitle);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(547, 25);
			this.panel2.TabIndex = 0;
			// 
			// textTitle
			// 
			this.textTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTitle.Location = new System.Drawing.Point(36, 3);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(508, 20);
			this.textTitle.TabIndex = 1;
			this.textTitle.TextChanged += new System.EventHandler(this.textTitle_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Title";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel3.Controls.Add(this.labelVersion, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.comboProposedVersion, 2, 1);
			this.tableLayoutPanel3.Controls.Add(this.labelPriority, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.comboPriority, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.comboCategory, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.labelCategory, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 34);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(547, 47);
			this.tableLayoutPanel3.TabIndex = 2;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(367, 0);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(89, 16);
			this.labelVersion.TabIndex = 323;
			this.labelVersion.Text = "Est. Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboProposedVersion
			// 
			this.comboProposedVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboProposedVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProposedVersion.FormattingEnabled = true;
			this.comboProposedVersion.Location = new System.Drawing.Point(367, 22);
			this.comboProposedVersion.Name = "comboProposedVersion";
			this.comboProposedVersion.Size = new System.Drawing.Size(177, 21);
			this.comboProposedVersion.TabIndex = 322;
			// 
			// labelPriority
			// 
			this.labelPriority.Location = new System.Drawing.Point(185, 0);
			this.labelPriority.Name = "labelPriority";
			this.labelPriority.Size = new System.Drawing.Size(57, 16);
			this.labelPriority.TabIndex = 309;
			this.labelPriority.Text = "Priority";
			this.labelPriority.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboPriority
			// 
			this.comboPriority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPriority.Location = new System.Drawing.Point(185, 22);
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(176, 21);
			this.comboPriority.TabIndex = 311;
			this.comboPriority.SelectionChangeCommitted += new System.EventHandler(this.comboPriority_SelectionChangeCommitted);
			// 
			// comboCategory
			// 
			this.comboCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCategory.FormattingEnabled = true;
			this.comboCategory.Location = new System.Drawing.Point(3, 22);
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(176, 21);
			this.comboCategory.TabIndex = 312;
			this.comboCategory.SelectionChangeCommitted += new System.EventHandler(this.comboCategory_SelectionChangeCommitted);
			// 
			// labelCategory
			// 
			this.labelCategory.Location = new System.Drawing.Point(3, 0);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(105, 16);
			this.labelCategory.TabIndex = 310;
			this.labelCategory.Text = "Category";
			this.labelCategory.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormJobAdd
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(559, 509);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobAdd";
			this.Text = "Add Job";
			this.Load += new System.EventHandler(this.FormJobAdd_Load);
			this.tabControlExtra.ResumeLayout(false);
			this.tabConcept.ResumeLayout(false);
			this.tabCustomers.ResumeLayout(false);
			this.tabSubscribers.ResumeLayout(false);
			this.tabRequests.ResumeLayout(false);
			this.tabBugs.ResumeLayout(false);
			this.tabTasks.ResumeLayout(false);
			this.tabQuotes.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TabControl tabControlExtra;
		private System.Windows.Forms.TabPage tabConcept;
		private System.Windows.Forms.TabPage tabCustomers;
		private System.Windows.Forms.TabPage tabSubscribers;
		private System.Windows.Forms.TabPage tabRequests;
		private System.Windows.Forms.TabPage tabBugs;
		private System.Windows.Forms.TabPage tabTasks;
		private System.Windows.Forms.TabPage tabQuotes;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelPriority;
		private System.Windows.Forms.Label labelCategory;
		private UI.ComboBoxOD comboPriority;
		private System.Windows.Forms.ComboBox comboCategory;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private OdtextEditor textConcept;
		private UI.GridOD gridSubscribers;
		private UI.GridOD gridFeatureReq;
		private UI.GridOD gridBugs;
		private UI.GridOD gridTasks;
		private UI.GridOD gridQuotes;
		private UI.GridOD gridCustomers;
		private System.Windows.Forms.ComboBox comboProposedVersion;
		private System.Windows.Forms.Label labelVersion;
	}
}