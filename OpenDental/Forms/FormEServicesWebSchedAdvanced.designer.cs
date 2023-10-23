namespace OpenDental{
	partial class FormEServicesWebSchedAdvanced {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesWebSchedAdvanced));
			this.gridClinics = new OpenDental.UI.GridOD();
			this.butSave = new OpenDental.UI.Button();
			this.panelHostedURLs = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// gridClinics
			// 
			this.gridClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridClinics.Location = new System.Drawing.Point(7, 25);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.Size = new System.Drawing.Size(281, 391);
			this.gridClinics.TabIndex = 5;
			this.gridClinics.Title = "Clinics";
			this.gridClinics.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(972, 422);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// panelHostedURLs
			// 
			this.panelHostedURLs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelHostedURLs.AutoScroll = true;
			this.panelHostedURLs.Location = new System.Drawing.Point(291, 25);
			this.panelHostedURLs.Name = "panelHostedURLs";
			this.panelHostedURLs.Size = new System.Drawing.Size(756, 391);
			this.panelHostedURLs.TabIndex = 5;
			// 
			// FormEServicesWebSchedAdvanced
			// 
			this.ClientSize = new System.Drawing.Size(1069, 457);
			this.Controls.Add(this.panelHostedURLs);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesWebSchedAdvanced";
			this.Text = "eServices Web Sched - Advanced";
			this.Load += new System.EventHandler(this.FormEServicesWebSchedAdvanced_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.GridOD gridClinics;
		private System.Windows.Forms.Panel panelHostedURLs;
	}
}