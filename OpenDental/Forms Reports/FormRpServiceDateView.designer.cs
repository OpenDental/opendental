namespace OpenDental{
	partial class FormRpServiceDateView {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpServiceDateView));
			this.butClose = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSavePDFToImages = new OpenDental.UI.Button();
			this.groupFilter = new System.Windows.Forms.GroupBox();
			this.checkDetailedView = new System.Windows.Forms.CheckBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(900, 444);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(12, 442);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 4;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 63);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMain.Size = new System.Drawing.Size(963, 373);
			this.gridMain.TabIndex = 29;
			this.gridMain.Title = "Service View";
			this.gridMain.TranslationName = "TableServiceView";
			// 
			// butSavePDFToImages
			// 
			this.butSavePDFToImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSavePDFToImages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSavePDFToImages.Location = new System.Drawing.Point(107, 442);
			this.butSavePDFToImages.Name = "butSavePDFToImages";
			this.butSavePDFToImages.Size = new System.Drawing.Size(88, 26);
			this.butSavePDFToImages.TabIndex = 84;
			this.butSavePDFToImages.TabStop = false;
			this.butSavePDFToImages.Text = "Save to Images";
			this.butSavePDFToImages.Click += new System.EventHandler(this.butSavePDFToImages_Click);
			// 
			// groupFilter
			// 
			this.groupFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilter.Controls.Add(this.checkDetailedView);
			this.groupFilter.Location = new System.Drawing.Point(707, 12);
			this.groupFilter.Name = "groupFilter";
			this.groupFilter.Size = new System.Drawing.Size(169, 45);
			this.groupFilter.TabIndex = 85;
			this.groupFilter.TabStop = false;
			this.groupFilter.Text = "Filters";
			// 
			// checkDetailedView
			// 
			this.checkDetailedView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDetailedView.Checked = true;
			this.checkDetailedView.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkDetailedView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDetailedView.Location = new System.Drawing.Point(4, 15);
			this.checkDetailedView.Name = "checkDetailedView";
			this.checkDetailedView.Size = new System.Drawing.Size(142, 17);
			this.checkDetailedView.TabIndex = 62;
			this.checkDetailedView.Text = "Show detailed view";
			this.checkDetailedView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(884, 27);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 86;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// FormRpServiceDateView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(987, 480);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.groupFilter);
			this.Controls.Add(this.butSavePDFToImages);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butPrint);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpServiceDateView";
			this.Text = "Service Date View";
			this.Load += new System.EventHandler(this.FormRpServiceDate_Load);
			this.groupFilter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.Button butPrint;
		private UI.GridOD gridMain;
		private UI.Button butSavePDFToImages;
		private System.Windows.Forms.GroupBox groupFilter;
		private UI.Button butRefresh;
		private System.Windows.Forms.CheckBox checkDetailedView;
	}
}