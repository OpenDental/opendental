namespace OpenDental {
	partial class FormRxManage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRxManage));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butNewRx = new OpenDental.UI.Button();
			this.butPrintSelected = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelECWerror = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(22, 25);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(990, 410);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Prescriptions";
			this.gridMain.TranslationName = "TableRxManage";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butNewRx
			// 
			this.butNewRx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNewRx.Location = new System.Drawing.Point(82, 455);
			this.butNewRx.Name = "butNewRx";
			this.butNewRx.Size = new System.Drawing.Size(75, 24);
			this.butNewRx.TabIndex = 0;
			this.butNewRx.Text = "&New Rx";
			this.butNewRx.Click += new System.EventHandler(this.butNewRx_Click);
			// 
			// butPrintSelected
			// 
			this.butPrintSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintSelected.Location = new System.Drawing.Point(219, 455);
			this.butPrintSelected.Name = "butPrintSelected";
			this.butPrintSelected.Size = new System.Drawing.Size(87, 24);
			this.butPrintSelected.TabIndex = 1;
			this.butPrintSelected.Text = "&Print Selected";
			this.butPrintSelected.Click += new System.EventHandler(this.butPrintSelect_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(937, 455);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelECWerror
			// 
			this.labelECWerror.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelECWerror.AutoSize = true;
			this.labelECWerror.Location = new System.Drawing.Point(332, 461);
			this.labelECWerror.MinimumSize = new System.Drawing.Size(314, 14);
			this.labelECWerror.Name = "labelECWerror";
			this.labelECWerror.Size = new System.Drawing.Size(314, 14);
			this.labelECWerror.TabIndex = 5;
			this.labelECWerror.Text = "Error:";
			this.labelECWerror.Visible = false;
			// 
			// FormRxManage
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1034, 491);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelECWerror);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butNewRx);
			this.Controls.Add(this.butPrintSelected);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRxManage";
			this.Text = "Rx Manage";
			this.Load += new System.EventHandler(this.FormRxManage_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

	}
}