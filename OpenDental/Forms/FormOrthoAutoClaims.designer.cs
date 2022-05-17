namespace OpenDental{
	partial class FormOrthoAutoClaims {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoAutoClaims));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butGenerate = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 39);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(916, 429);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Claims to be Created";
			this.gridMain.TranslationName = "TableAutoOrthoClaims";
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butSelectAll.Location = new System.Drawing.Point(427, 474);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(84, 24);
			this.butSelectAll.TabIndex = 5;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(853, 474);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butGenerate
			// 
			this.butGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGenerate.Location = new System.Drawing.Point(12, 474);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(120, 24);
			this.butGenerate.TabIndex = 2;
			this.butGenerate.Text = "Generate Claims";
			this.butGenerate.Click += new System.EventHandler(this.butGenerateClaims_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinics.Location = new System.Drawing.Point(723, 9);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(205, 21);
			this.comboClinics.TabIndex = 6;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.comboClinics_SelectionChangeCommitted);
			// 
			// FormOrthoAutoClaims
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(939, 510);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butGenerate);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoAutoClaims";
			this.Text = "Ortho Auto Claims";
			this.Load += new System.EventHandler(this.FormAutoOrtho_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butGenerate;
		private UI.GridOD gridMain;
		private UI.Button butSelectAll;
		private UI.ComboBoxClinicPicker comboClinics;
	}
}