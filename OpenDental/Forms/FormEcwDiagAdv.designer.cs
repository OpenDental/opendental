namespace OpenDental{
	partial class FormEcwDiagAdv {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEcwDiagAdv));
			this.textQuery = new System.Windows.Forms.TextBox();
			this.textConnString = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridTables = new OpenDental.UI.GridOD();
			this.butRunQ = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listQuery = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// textQuery
			// 
			this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textQuery.Location = new System.Drawing.Point(246, 58);
			this.textQuery.Multiline = true;
			this.textQuery.Name = "textQuery";
			this.textQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textQuery.Size = new System.Drawing.Size(728, 99);
			this.textQuery.TabIndex = 3;
			this.textQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textQuery_KeyDown);
			// 
			// textConnString
			// 
			this.textConnString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textConnString.Location = new System.Drawing.Point(12, 12);
			this.textConnString.Multiline = true;
			this.textConnString.Name = "textConnString";
			this.textConnString.Size = new System.Drawing.Size(962, 40);
			this.textConnString.TabIndex = 4;
			// 
			// gridMain
			// 
			this.gridMain.AllowSelection = false;
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(246, 163);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(728, 505);
			this.gridMain.TabIndex = 7;
			this.gridMain.Title = "Query Results";
			this.gridMain.TranslationName = "TableQueryResults";
			// 
			// gridTables
			// 
			this.gridTables.AllowSelection = false;
			this.gridTables.AllowSortingByColumn = true;
			this.gridTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridTables.AutoScroll = true;
			this.gridTables.HScrollVisible = true;
			this.gridTables.Location = new System.Drawing.Point(12, 163);
			this.gridTables.Name = "gridTables";
			this.gridTables.Size = new System.Drawing.Size(228, 505);
			this.gridTables.TabIndex = 8;
			this.gridTables.Title = "Tables Available";
			this.gridTables.TranslationName = "TableAvailable";
			this.gridTables.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTables_CellDoubleClick);
			this.gridTables.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridTables_KeyPress);
			// 
			// butRunQ
			// 
			this.butRunQ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRunQ.Image = global::OpenDental.Properties.Resources.butGoto;
			this.butRunQ.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRunQ.Location = new System.Drawing.Point(986, 58);
			this.butRunQ.Name = "butRunQ";
			this.butRunQ.Size = new System.Drawing.Size(75, 24);
			this.butRunQ.TabIndex = 6;
			this.butRunQ.Text = "Run";
			this.butRunQ.Click += new System.EventHandler(this.butRunQ_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(986, 644);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listQuery
			// 
			this.listQuery.Location = new System.Drawing.Point(12, 58);
			this.listQuery.Name = "listQuery";
			this.listQuery.Size = new System.Drawing.Size(228, 95);
			this.listQuery.TabIndex = 9;
			this.listQuery.SelectedIndexChanged += new System.EventHandler(this.listQuery_SelectedIndexChanged);
			// 
			// FormEcwDiagAdv
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1073, 680);
			this.Controls.Add(this.listQuery);
			this.Controls.Add(this.gridTables);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butRunQ);
			this.Controls.Add(this.textConnString);
			this.Controls.Add(this.textQuery);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEcwDiagAdv";
			this.Text = "eClinical Works Diagnostic";
			this.Load += new System.EventHandler(this.FormEcwDiagAdv_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textQuery;
		private System.Windows.Forms.TextBox textConnString;
		private UI.Button butRunQ;
		private UI.GridOD gridMain;
		private UI.GridOD gridTables;
		private OpenDental.UI.ListBoxOD listQuery;
	}
}