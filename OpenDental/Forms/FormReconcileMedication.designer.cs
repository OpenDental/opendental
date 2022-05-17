namespace OpenDental{
	partial class FormReconcileMedication {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReconcileMedication));
			this.gridMedExisting = new OpenDental.UI.GridOD();
			this.gridMedImport = new OpenDental.UI.GridOD();
			this.gridMedReconcile = new OpenDental.UI.GridOD();
			this.butRemoveRec = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butAddNew = new OpenDental.UI.Button();
			this.butAddExist = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelBatch = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMedExisting
			// 
			this.gridMedExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMedExisting.Location = new System.Drawing.Point(4, 12);
			this.gridMedExisting.Name = "gridMedExisting";
			this.gridMedExisting.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMedExisting.Size = new System.Drawing.Size(480, 245);
			this.gridMedExisting.TabIndex = 65;
			this.gridMedExisting.Title = "Current Medications";
			this.gridMedExisting.TranslationName = "GridMedExisting";
			// 
			// gridMedImport
			// 
			this.gridMedImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMedImport.Location = new System.Drawing.Point(497, 12);
			this.gridMedImport.Name = "gridMedImport";
			this.gridMedImport.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMedImport.Size = new System.Drawing.Size(480, 245);
			this.gridMedImport.TabIndex = 77;
			this.gridMedImport.Title = "Transition of Care/Referral Summary";
			this.gridMedImport.TranslationName = "GridMedImport";
			// 
			// gridMedReconcile
			// 
			this.gridMedReconcile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMedReconcile.Location = new System.Drawing.Point(4, 293);
			this.gridMedReconcile.Name = "gridMedReconcile";
			this.gridMedReconcile.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMedReconcile.Size = new System.Drawing.Size(973, 300);
			this.gridMedReconcile.TabIndex = 67;
			this.gridMedReconcile.Title = "Reconciled Medications";
			this.gridMedReconcile.TranslationName = "gridMedReconcile";
			// 
			// butRemoveRec
			// 
			this.butRemoveRec.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butRemoveRec.Location = new System.Drawing.Point(437, 599);
			this.butRemoveRec.Name = "butRemoveRec";
			this.butRemoveRec.Size = new System.Drawing.Size(99, 24);
			this.butRemoveRec.TabIndex = 82;
			this.butRemoveRec.Text = "Remove Selected";
			this.butRemoveRec.Click += new System.EventHandler(this.butRemoveRec_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(821, 640);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 81;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAddNew
			// 
			this.butAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddNew.Image = global::OpenDental.Properties.Resources.down;
			this.butAddNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNew.Location = new System.Drawing.Point(712, 263);
			this.butAddNew.Name = "butAddNew";
			this.butAddNew.Size = new System.Drawing.Size(51, 24);
			this.butAddNew.TabIndex = 80;
			this.butAddNew.Text = "Add";
			this.butAddNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butAddNew.Click += new System.EventHandler(this.butAddNew_Click);
			// 
			// butAddExist
			// 
			this.butAddExist.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butAddExist.Image = global::OpenDental.Properties.Resources.down;
			this.butAddExist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddExist.Location = new System.Drawing.Point(218, 263);
			this.butAddExist.Name = "butAddExist";
			this.butAddExist.Size = new System.Drawing.Size(51, 24);
			this.butAddExist.TabIndex = 79;
			this.butAddExist.Text = "Add";
			this.butAddExist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butAddExist.Click += new System.EventHandler(this.butAddExist_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(902, 640);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelBatch
			// 
			this.labelBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelBatch.Location = new System.Drawing.Point(76, 640);
			this.labelBatch.Name = "labelBatch";
			this.labelBatch.Size = new System.Drawing.Size(739, 24);
			this.labelBatch.TabIndex = 153;
			this.labelBatch.Text = "Clicking OK updates the patient\'s medications to match the reconciled list.";
			this.labelBatch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormReconcileMedication
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(982, 676);
			this.Controls.Add(this.labelBatch);
			this.Controls.Add(this.gridMedExisting);
			this.Controls.Add(this.gridMedImport);
			this.Controls.Add(this.butRemoveRec);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butAddNew);
			this.Controls.Add(this.butAddExist);
			this.Controls.Add(this.gridMedReconcile);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReconcileMedication";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Reconcile Medication";
			this.Load += new System.EventHandler(this.FormReconcileMedication_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMedReconcile;
		private UI.GridOD gridMedImport;
		private UI.GridOD gridMedExisting;
		private UI.Button butAddExist;
		private UI.Button butAddNew;
		private UI.Button butOK;
		private UI.Button butRemoveRec;
		private System.Windows.Forms.Label labelBatch;
	}
}