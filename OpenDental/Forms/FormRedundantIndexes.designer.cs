namespace OpenDental{
	partial class FormRedundantIndexes {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRedundantIndexes));
			this.butDropIndexes = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAll = new OpenDental.UI.Button();
			this.butNone = new OpenDental.UI.Button();
			this.checkLogAddStatements = new System.Windows.Forms.CheckBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butDropIndexes
			// 
			this.butDropIndexes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDropIndexes.Location = new System.Drawing.Point(259, 454);
			this.butDropIndexes.Name = "butDropIndexes";
			this.butDropIndexes.Size = new System.Drawing.Size(75, 24);
			this.butDropIndexes.TabIndex = 4;
			this.butDropIndexes.Text = "Drop";
			this.butDropIndexes.Click += new System.EventHandler(this.butDrop_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1143, 454);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 43);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1206, 405);
			this.gridMain.TabIndex = 0;
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAll.Location = new System.Drawing.Point(12, 454);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 24);
			this.butAll.TabIndex = 1;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNone.Location = new System.Drawing.Point(93, 454);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 24);
			this.butNone.TabIndex = 2;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// checkLogAddStatements
			// 
			this.checkLogAddStatements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkLogAddStatements.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLogAddStatements.Checked = true;
			this.checkLogAddStatements.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkLogAddStatements.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLogAddStatements.Location = new System.Drawing.Point(984, 19);
			this.checkLogAddStatements.Name = "checkLogAddStatements";
			this.checkLogAddStatements.Size = new System.Drawing.Size(234, 18);
			this.checkLogAddStatements.TabIndex = 3;
			this.checkLogAddStatements.Text = "Log statements to add indexes back";
			this.checkLogAddStatements.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLogAddStatements.UseVisualStyleBackColor = true;
			this.checkLogAddStatements.CheckedChanged += new System.EventHandler(this.checkLogAddStatements_CheckedChanged);
			// 
			// labelDescription
			// 
			this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDescription.Location = new System.Drawing.Point(12, 9);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(966, 28);
			this.labelDescription.TabIndex = 6;
			this.labelDescription.Text = resources.GetString("labelDescription.Text");
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormRedundantIndexes
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1230, 490);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.checkLogAddStatements);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDropIndexes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRedundantIndexes";
			this.Text = "Drop Redundant Indexes";
			this.Load += new System.EventHandler(this.FormRedundantIndexes_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butDropIndexes;
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.Button butAll;
		private UI.Button butNone;
		private System.Windows.Forms.CheckBox checkLogAddStatements;
		private System.Windows.Forms.Label labelDescription;
	}
}