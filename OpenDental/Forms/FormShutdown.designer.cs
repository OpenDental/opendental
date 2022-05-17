namespace OpenDental{
	partial class FormShutdown {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShutdown));
			this.gridActiveInstances = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butCloseSessions = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butShutdown = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridActiveInstances
			// 
			this.gridActiveInstances.AllowSortingByColumn = true;
			this.gridActiveInstances.Location = new System.Drawing.Point(12, 24);
			this.gridActiveInstances.Name = "gridActiveInstances";
			this.gridActiveInstances.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridActiveInstances.Size = new System.Drawing.Size(509, 431);
			this.gridActiveInstances.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(510, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "List of known sessions connected to this database. This session is highlighted.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butCloseSessions
			// 
			this.butCloseSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCloseSessions.Location = new System.Drawing.Point(529, 351);
			this.butCloseSessions.Name = "butCloseSessions";
			this.butCloseSessions.Size = new System.Drawing.Size(107, 24);
			this.butCloseSessions.TabIndex = 8;
			this.butCloseSessions.Text = "Shutdown Selected";
			this.butCloseSessions.UseVisualStyleBackColor = true;
			this.butCloseSessions.Click += new System.EventHandler(this.butCloseSessions_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(529, 431);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(107, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butShutdown
			// 
			this.butShutdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butShutdown.Location = new System.Drawing.Point(529, 391);
			this.butShutdown.Name = "butShutdown";
			this.butShutdown.Size = new System.Drawing.Size(107, 24);
			this.butShutdown.TabIndex = 6;
			this.butShutdown.Text = "Shutdown All";
			this.butShutdown.Click += new System.EventHandler(this.butShutdown_Click);
			// 
			// FormShutdown
			// 
			this.ClientSize = new System.Drawing.Size(648, 467);
			this.Controls.Add(this.butCloseSessions);
			this.Controls.Add(this.gridActiveInstances);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butShutdown);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormShutdown";
			this.Text = "Shutdown Workstations";
			this.Load += new System.EventHandler(this.FormShutdown_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butShutdown;
		private UI.GridOD gridActiveInstances;
		private UI.Button butCloseSessions;
	}
}