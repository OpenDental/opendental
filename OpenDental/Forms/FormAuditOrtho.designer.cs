namespace OpenDental{
	partial class FormAuditOrtho {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAuditOrtho));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridHist = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(834, 565);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(178, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(731, 547);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Audit Trail";
			this.gridMain.TranslationName = "TableOrthoAudit";
			// 
			// gridHist
			// 
			this.gridHist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridHist.Location = new System.Drawing.Point(12, 12);
			this.gridHist.Name = "gridHist";
			this.gridHist.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridHist.Size = new System.Drawing.Size(160, 547);
			this.gridHist.TabIndex = 6;
			this.gridHist.Title = "Date Service";
			this.gridHist.TranslationName = "OrthoAudit";
			this.gridHist.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridHist_CellClick);
			// 
			// FormAuditOrtho
			// 
			this.ClientSize = new System.Drawing.Size(921, 601);
			this.Controls.Add(this.gridHist);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAuditOrtho";
			this.Text = "Audit Trail";
			this.Load += new System.EventHandler(this.FormAuditOrtho_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.GridOD gridHist;
	}
}