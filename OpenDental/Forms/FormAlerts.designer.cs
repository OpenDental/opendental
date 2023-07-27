namespace OpenDental{
	partial class FormAlerts {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlerts));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelOpenForm = new System.Windows.Forms.Label();
			this.butViewDetails = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOpenForm = new OpenDental.UI.Button();
			this.butMarkAsRead = new OpenDental.UI.Button();
			this.butAcknowledge = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(828, 557);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 6;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(13, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(750, 569);
			this.gridMain.TabIndex = 1;
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAlerts_CellClick);
			// 
			// labelOpenForm
			// 
			this.labelOpenForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelOpenForm.Location = new System.Drawing.Point(779, 130);
			this.labelOpenForm.Name = "labelOpenForm";
			this.labelOpenForm.Size = new System.Drawing.Size(117, 34);
			this.labelOpenForm.TabIndex = 7;
			this.labelOpenForm.Text = "eServices eConnector Service";
			// 
			// butViewDetails
			// 
			this.butViewDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butViewDetails.Enabled = false;
			this.butViewDetails.Location = new System.Drawing.Point(779, 42);
			this.butViewDetails.Name = "butViewDetails";
			this.butViewDetails.Size = new System.Drawing.Size(80, 24);
			this.butViewDetails.TabIndex = 3;
			this.butViewDetails.Text = "View Details";
			this.butViewDetails.UseVisualStyleBackColor = true;
			this.butViewDetails.Click += new System.EventHandler(this.butViewDetails_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Enabled = false;
			this.butDelete.Location = new System.Drawing.Point(779, 72);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOpenForm
			// 
			this.butOpenForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butOpenForm.Enabled = false;
			this.butOpenForm.Location = new System.Drawing.Point(779, 102);
			this.butOpenForm.Name = "butOpenForm";
			this.butOpenForm.Size = new System.Drawing.Size(80, 24);
			this.butOpenForm.TabIndex = 5;
			this.butOpenForm.Text = "Open";
			this.butOpenForm.UseVisualStyleBackColor = true;
			this.butOpenForm.Click += new System.EventHandler(this.butOpenForm_Click);
			// 
			// butMarkAsRead
			// 
			this.butMarkAsRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMarkAsRead.Enabled = false;
			this.butMarkAsRead.Location = new System.Drawing.Point(779, 12);
			this.butMarkAsRead.Name = "butMarkAsRead";
			this.butMarkAsRead.Size = new System.Drawing.Size(80, 24);
			this.butMarkAsRead.TabIndex = 2;
			this.butMarkAsRead.Text = "Mark Read";
			this.butMarkAsRead.UseVisualStyleBackColor = true;
			this.butMarkAsRead.Click += new System.EventHandler(this.butMarkAsRead_Click);
			// 
			// butAcknowledge
			// 
			this.butAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAcknowledge.Location = new System.Drawing.Point(779, 224);
			this.butAcknowledge.Name = "butAcknowledge";
			this.butAcknowledge.Size = new System.Drawing.Size(80, 24);
			this.butAcknowledge.TabIndex = 8;
			this.butAcknowledge.Text = "Acknowledge";
			this.butAcknowledge.UseVisualStyleBackColor = true;
			this.butAcknowledge.Click += new System.EventHandler(this.butAcknowledge_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(776, 252);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "(Delete or Mark Read)";
			// 
			// FormAlerts
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(915, 593);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAcknowledge);
			this.Controls.Add(this.labelOpenForm);
			this.Controls.Add(this.butViewDetails);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butOpenForm);
			this.Controls.Add(this.butMarkAsRead);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAlerts";
			this.Text = "Alerts";
			this.Load += new System.EventHandler(this.FormAlerts_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.Button butDelete;
		private UI.Button butOpenForm;
		private UI.Button butMarkAsRead;
		private UI.Button butViewDetails;
		private System.Windows.Forms.Label labelOpenForm;
		private UI.Button butAcknowledge;
		private System.Windows.Forms.Label label1;
	}
}