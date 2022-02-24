namespace OpenDental{
	partial class FormEtrans834Import {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans834Import));
			this.label1 = new System.Windows.Forms.Label();
			this.folderBrowserImportPath = new OpenDental.FolderBrowserDialog();
			this.gridInsPlanFiles = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.butImportPathPick = new OpenDental.UI.Button();
			this.textImportPath = new OpenDental.ODtextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelProgress = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 93);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.TabIndex = 5;
			this.label1.Text = "Import Path";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridInsPlanFiles
			// 
			this.gridInsPlanFiles.AllowSortingByColumn = true;
			this.gridInsPlanFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridInsPlanFiles.Location = new System.Drawing.Point(12, 142);
			this.gridInsPlanFiles.Name = "gridInsPlanFiles";
			this.gridInsPlanFiles.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridInsPlanFiles.Size = new System.Drawing.Size(950, 512);
			this.gridInsPlanFiles.TabIndex = 8;
			this.gridInsPlanFiles.Title = "Ins Plan Files To Import";
			this.gridInsPlanFiles.TranslationName = "TableImport";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(950, 84);
			this.label2.TabIndex = 10;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(887, 116);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 20);
			this.butRefresh.TabIndex = 9;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butImportPathPick
			// 
			this.butImportPathPick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butImportPathPick.Location = new System.Drawing.Point(854, 116);
			this.butImportPathPick.Name = "butImportPathPick";
			this.butImportPathPick.Size = new System.Drawing.Size(30, 20);
			this.butImportPathPick.TabIndex = 6;
			this.butImportPathPick.Text = "...";
			this.butImportPathPick.Click += new System.EventHandler(this.butImportPathPick_Click);
			// 
			// textImportPath
			// 
			this.textImportPath.AcceptsTab = true;
			this.textImportPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textImportPath.BackColor = System.Drawing.SystemColors.Window;
			this.textImportPath.DetectLinksEnabled = false;
			this.textImportPath.DetectUrls = false;
			this.textImportPath.Location = new System.Drawing.Point(12, 116);
			this.textImportPath.Multiline = false;
			this.textImportPath.Name = "textImportPath";
			this.textImportPath.QuickPasteType = OpenDentBusiness.QuickPasteType.FilePath;
			this.textImportPath.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textImportPath.Size = new System.Drawing.Size(836, 20);
			this.textImportPath.SpellCheckIsEnabled = false;
			this.textImportPath.TabIndex = 4;
			this.textImportPath.Text = "";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(809, 660);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(887, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelProgress
			// 
			this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelProgress.Location = new System.Drawing.Point(12, 662);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(788, 20);
			this.labelProgress.TabIndex = 11;
			this.labelProgress.Text = " ";
			this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormEtrans834Import
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.labelProgress);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.gridInsPlanFiles);
			this.Controls.Add(this.butImportPathPick);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textImportPath);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEtrans834Import";
			this.Text = "Import Ins Plans 834";
			this.Load += new System.EventHandler(this.FormEtrans834Import_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textImportPath;
		private System.Windows.Forms.Label label1;
		private UI.Button butImportPathPick;
		private OpenDental.FolderBrowserDialog folderBrowserImportPath;
		private UI.GridOD gridInsPlanFiles;
		private UI.Button butRefresh;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelProgress;
	}
}