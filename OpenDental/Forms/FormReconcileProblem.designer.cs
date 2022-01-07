namespace OpenDental{
	partial class FormReconcileProblem {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReconcileProblem));
			this.gridProbExisting = new OpenDental.UI.GridOD();
			this.gridProbImport = new OpenDental.UI.GridOD();
			this.butRemoveRec = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butAddNew = new OpenDental.UI.Button();
			this.butAddExist = new OpenDental.UI.Button();
			this.gridProbReconcile = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.labelBatch = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridProbExisting
			// 
			this.gridProbExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProbExisting.Location = new System.Drawing.Point(4, 12);
			this.gridProbExisting.Name = "gridProbExisting";
			this.gridProbExisting.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProbExisting.Size = new System.Drawing.Size(477, 245);
			this.gridProbExisting.TabIndex = 65;
			this.gridProbExisting.Title = "Current Problems";
			this.gridProbExisting.TranslationName = "GridProbExisting";
			// 
			// gridProbImport
			// 
			this.gridProbImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProbImport.Location = new System.Drawing.Point(497, 12);
			this.gridProbImport.Name = "gridProbImport";
			this.gridProbImport.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProbImport.Size = new System.Drawing.Size(480, 245);
			this.gridProbImport.TabIndex = 77;
			this.gridProbImport.Title = "Transition of Care/Referral Summary";
			this.gridProbImport.TranslationName = "GridProbImport";
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
			// gridProbReconcile
			// 
			this.gridProbReconcile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProbReconcile.Location = new System.Drawing.Point(4, 293);
			this.gridProbReconcile.Name = "gridProbReconcile";
			this.gridProbReconcile.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProbReconcile.Size = new System.Drawing.Size(973, 300);
			this.gridProbReconcile.TabIndex = 67;
			this.gridProbReconcile.Title = "Reconciled Problem";
			this.gridProbReconcile.TranslationName = "gridProbReconcile";
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
			this.labelBatch.Text = "Clicking OK updates the patient\'s problems to match the reconciled list.";
			this.labelBatch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormReconcileProblem
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(982, 676);
			this.Controls.Add(this.labelBatch);
			this.Controls.Add(this.gridProbExisting);
			this.Controls.Add(this.gridProbImport);
			this.Controls.Add(this.butRemoveRec);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butAddNew);
			this.Controls.Add(this.butAddExist);
			this.Controls.Add(this.gridProbReconcile);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReconcileProblem";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Reconcile Problems";
			this.Load += new System.EventHandler(this.FormReconcileProblem_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridProbReconcile;
		private UI.GridOD gridProbImport;
		private UI.GridOD gridProbExisting;
		private UI.Button butAddExist;
		private UI.Button butAddNew;
		private UI.Button butOK;
		private UI.Button butRemoveRec;
		private System.Windows.Forms.Label labelBatch;
	}
}