namespace CentralManager {
	partial class FormCentralPatientTransfer {
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
			this.gridPatients = new OpenDental.UI.GridOD();
			this.gridDatabasesTo = new OpenDental.UI.GridOD();
			this.labelSourceDb = new System.Windows.Forms.Label();
			this.butTransfer = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butAddPatients = new OpenDental.UI.Button();
			this.butAddDatabases = new OpenDental.UI.Button();
			this.butRemovePats = new OpenDental.UI.Button();
			this.butRemoveDb = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridPatients
			// 
			this.gridPatients.AllowSortingByColumn = true;
			this.gridPatients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridPatients.Location = new System.Drawing.Point(12, 59);
			this.gridPatients.Name = "gridPatients";
			this.gridPatients.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridPatients.Size = new System.Drawing.Size(450, 369);
			this.gridPatients.TabIndex = 6;
			this.gridPatients.Title = "Patients to Export";
			this.gridPatients.TranslationName = "";
			// 
			// gridDatabasesTo
			// 
			this.gridDatabasesTo.AllowSortingByColumn = true;
			this.gridDatabasesTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridDatabasesTo.Location = new System.Drawing.Point(485, 59);
			this.gridDatabasesTo.Name = "gridDatabasesTo";
			this.gridDatabasesTo.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridDatabasesTo.Size = new System.Drawing.Size(450, 369);
			this.gridDatabasesTo.TabIndex = 7;
			this.gridDatabasesTo.Title = "Database to Export patients to";
			this.gridDatabasesTo.TranslationName = "";
			// 
			// labelSourceDb
			// 
			this.labelSourceDb.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelSourceDb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSourceDb.Location = new System.Drawing.Point(12, 36);
			this.labelSourceDb.Name = "labelSourceDb";
			this.labelSourceDb.Size = new System.Drawing.Size(453, 12);
			this.labelSourceDb.TabIndex = 228;
			this.labelSourceDb.Text = "Source database:   ";
			this.labelSourceDb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butTransfer
			// 
			this.butTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTransfer.Location = new System.Drawing.Point(953, 388);
			this.butTransfer.Name = "butTransfer";
			this.butTransfer.Size = new System.Drawing.Size(75, 24);
			this.butTransfer.TabIndex = 229;
			this.butTransfer.Text = "Export";
			this.butTransfer.UseVisualStyleBackColor = true;
			this.butTransfer.Click += new System.EventHandler(this.butTransfer_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(953, 448);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 230;
			this.butClose.Text = "&Close";
			// 
			// butAddPatients
			// 
			this.butAddPatients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddPatients.Location = new System.Drawing.Point(12, 447);
			this.butAddPatients.Name = "butAddPatients";
			this.butAddPatients.Size = new System.Drawing.Size(75, 24);
			this.butAddPatients.TabIndex = 231;
			this.butAddPatients.Text = "Add";
			this.butAddPatients.UseVisualStyleBackColor = true;
			this.butAddPatients.Click += new System.EventHandler(this.butAddPatients_Click);
			// 
			// butAddDatabases
			// 
			this.butAddDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddDatabases.Location = new System.Drawing.Point(485, 447);
			this.butAddDatabases.Name = "butAddDatabases";
			this.butAddDatabases.Size = new System.Drawing.Size(75, 24);
			this.butAddDatabases.TabIndex = 232;
			this.butAddDatabases.Text = "Add";
			this.butAddDatabases.UseVisualStyleBackColor = true;
			this.butAddDatabases.Click += new System.EventHandler(this.butAddDatabases_Click);
			// 
			// butRemovePats
			// 
			this.butRemovePats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRemovePats.Location = new System.Drawing.Point(93, 447);
			this.butRemovePats.Name = "butRemovePats";
			this.butRemovePats.Size = new System.Drawing.Size(75, 24);
			this.butRemovePats.TabIndex = 233;
			this.butRemovePats.Text = "Remove";
			this.butRemovePats.UseVisualStyleBackColor = true;
			this.butRemovePats.Click += new System.EventHandler(this.butRemovePats_Click);
			// 
			// butRemoveDb
			// 
			this.butRemoveDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRemoveDb.Location = new System.Drawing.Point(566, 447);
			this.butRemoveDb.Name = "butRemoveDb";
			this.butRemoveDb.Size = new System.Drawing.Size(75, 24);
			this.butRemoveDb.TabIndex = 234;
			this.butRemoveDb.Text = "Remove";
			this.butRemoveDb.UseVisualStyleBackColor = true;
			this.butRemoveDb.Click += new System.EventHandler(this.butRemoveDb_Click);
			// 
			// FormCentralPatientTransfer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1050, 489);
			this.Controls.Add(this.butRemoveDb);
			this.Controls.Add(this.butRemovePats);
			this.Controls.Add(this.butAddDatabases);
			this.Controls.Add(this.butAddPatients);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butTransfer);
			this.Controls.Add(this.labelSourceDb);
			this.Controls.Add(this.gridDatabasesTo);
			this.Controls.Add(this.gridPatients);
			this.Name = "FormCentralPatientTransfer";
			this.Text = "Patient Transfer";
			this.Load += new System.EventHandler(this.FormCentralConnectionPatientTransfer_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridPatients;
		private OpenDental.UI.GridOD gridDatabasesTo;
		private System.Windows.Forms.Label labelSourceDb;
		private OpenDental.UI.Button butTransfer;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAddPatients;
		private OpenDental.UI.Button butAddDatabases;
		private OpenDental.UI.Button butRemovePats;
		private OpenDental.UI.Button butRemoveDb;
	}
}