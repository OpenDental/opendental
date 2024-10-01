namespace OpenDental{
	partial class FormPreferences {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPreferences));
			this.butSave = new OpenDental.UI.Button();
			this.treeMain = new System.Windows.Forms.TreeView();
			this.panelMain = new OpenDental.UI.PanelOD();
			this.labelCategories = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(663, 669);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// treeMain
			// 
			this.treeMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.treeMain.HideSelection = false;
			this.treeMain.Location = new System.Drawing.Point(12, 52);
			this.treeMain.Name = "treeMain";
			this.treeMain.ShowPlusMinus = false;
			this.treeMain.Size = new System.Drawing.Size(225, 612);
			this.treeMain.TabIndex = 0;
			this.treeMain.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeMain_BeforeCollapse);
			this.treeMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMain_AfterSelect);
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelMain.Location = new System.Drawing.Point(244, 4);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(494, 660);
			this.panelMain.TabIndex = 5;
			// 
			// labelCategories
			// 
			this.labelCategories.Location = new System.Drawing.Point(11, 33);
			this.labelCategories.Name = "labelCategories";
			this.labelCategories.Size = new System.Drawing.Size(225, 18);
			this.labelCategories.TabIndex = 236;
			this.labelCategories.Text = "Categories";
			this.labelCategories.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 18);
			this.label1.TabIndex = 237;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(62, 5);
			this.textSearch.MaxLength = 100;
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(130, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
			// 
			// FormPreferences
			// 
			this.ClientSize = new System.Drawing.Size(750, 696);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelCategories);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.treeMain);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPreferences";
			this.Text = "Preferences";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPreferences_FormClosing);
			this.Load += new System.EventHandler(this.FormModulePrefs_Load);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormPreferences_MouseMove);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.TreeView treeMain;
		private UI.PanelOD panelMain;
		private System.Windows.Forms.Label labelCategories;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSearch;
	}
}