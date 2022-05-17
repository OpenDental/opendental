namespace OpenDental {
	partial class FormWikiListAdvancedSearch {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiListAdvancedSearch));
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelSummary = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 49);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(310, 371);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Columns";
			this.gridMain.TranslationName = "TableWikiListAdvancedSearch";
			// 
			// labelSummary
			// 
			this.labelSummary.Location = new System.Drawing.Point(12, 9);
			this.labelSummary.Name = "labelSummary";
			this.labelSummary.Size = new System.Drawing.Size(310, 37);
			this.labelSummary.TabIndex = 1;
			this.labelSummary.Text = "Choose one or more columns to be used within the search of the Wiki List Edit win" +
    "dow.\r\n";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(166, 426);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(247, 426);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			// 
			// FormWikiListAdvancedSearch
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(334, 461);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelSummary);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiListAdvancedSearch";
			this.Text = "Wiki List Advanced Search";
			this.Load += new System.EventHandler(this.FormWikiListAdvancedSearch_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelSummary;
		private UI.Button butOK;
		private UI.Button butCancel;
	}
}