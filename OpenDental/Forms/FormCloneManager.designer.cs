namespace OpenDental {
	partial class FormCloneManager {
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
			this.butCancel = new OpenDental.UI.Button();
			this.butRun = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.menuRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemSeeFamily = new System.Windows.Forms.MenuItem();
			this.butRefresh = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(861,663);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butRun.Location = new System.Drawing.Point(780,663);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75,24);
			this.butRun.TabIndex = 4;
			this.butRun.Text = "Run";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSelection = false;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(40,82);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(895,572);
			this.gridMain.TabIndex = 19;
			this.gridMain.Title = "Possible Clones";
			this.gridMain.TranslationName = "TableCloneList";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// menuRightClick
			// 
			this.menuRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSeeFamily});
			// 
			// menuItemSeeFamily
			// 
			this.menuItemSeeFamily.Index = 0;
			this.menuItemSeeFamily.Text = "See Family";
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(860,52);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75,24);
			this.butRefresh.TabIndex = 20;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13,13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(717,63);
			this.label1.TabIndex = 21;
			this.label1.Text = "What this does:";
			// 
			// FormCloneFix
			// 
			this.ClientSize = new System.Drawing.Size(974,696);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCancel);
			this.Name = "FormCloneFix";
			this.Text = "Clone Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCloneFix_FormClosing);
			this.Load += new System.EventHandler(this.FormCloneFix_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butCancel;
		private UI.Button butRun;
		private UI.GridOD gridMain;
		private System.Windows.Forms.ContextMenu menuRightClick;
		private System.Windows.Forms.MenuItem menuItemSeeFamily;
		private UI.Button butRefresh;
		private System.Windows.Forms.Label label1;
	}
}