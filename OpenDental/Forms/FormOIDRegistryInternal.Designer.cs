namespace OpenDental {
	partial class FormOIDRegistryInternal {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOIDRegistryInternal));
			this.labelRetrieveStatus = new System.Windows.Forms.Label();
			this.butRetrieveOIDs = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// labelRetrieveStatus
			// 
			this.labelRetrieveStatus.ForeColor = System.Drawing.Color.Red;
			this.labelRetrieveStatus.Location = new System.Drawing.Point(103, 16);
			this.labelRetrieveStatus.Name = "labelRetrieveStatus";
			this.labelRetrieveStatus.Size = new System.Drawing.Size(468, 17);
			this.labelRetrieveStatus.TabIndex = 10;
			this.labelRetrieveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRetrieveOIDs
			// 
			this.butRetrieveOIDs.Location = new System.Drawing.Point(12, 12);
			this.butRetrieveOIDs.Name = "butRetrieveOIDs";
			this.butRetrieveOIDs.Size = new System.Drawing.Size(85, 24);
			this.butRetrieveOIDs.TabIndex = 9;
			this.butRetrieveOIDs.Text = "Retrieve OIDs";
			this.butRetrieveOIDs.UseVisualStyleBackColor = true;
			this.butRetrieveOIDs.Click += new System.EventHandler(this.butRetrieveOIDs_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(497, 326);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 7;
			this.butSave.Text = "&Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(560, 278);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Object Identifiers";
			this.gridMain.TranslationName = "TableObjectIdentifiers";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormOIDRegistryInternal
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(584, 362);
			this.Controls.Add(this.labelRetrieveStatus);
			this.Controls.Add(this.butRetrieveOIDs);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOIDRegistryInternal";
			this.Text = "OID Registry Internal";
			this.Load += new System.EventHandler(this.FormReminders_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private UI.Button butSave;
		private UI.Button butRetrieveOIDs;
		private System.Windows.Forms.Label labelRetrieveStatus;
	}
}