namespace OpenDental{
	partial class FormDefaultCCProcs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefaultCCProcs));
			this.listProcs = new OpenDental.UI.ListBox();
			this.label15 = new System.Windows.Forms.Label();
			this.butRemoveProc = new OpenDental.UI.Button();
			this.butAddProc = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.butSync = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listProcs
			// 
			this.listProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listProcs.Location = new System.Drawing.Point(15, 58);
			this.listProcs.Name = "listProcs";
			this.listProcs.Size = new System.Drawing.Size(220, 407);
			this.listProcs.TabIndex = 134;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(12, 12);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(304, 33);
			this.label15.TabIndex = 133;
			this.label15.Text = "Procedures that will be authorized for recurring charges when a new credit card i" +
    "s added.";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRemoveProc
			// 
			this.butRemoveProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemoveProc.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemoveProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemoveProc.Location = new System.Drawing.Point(241, 88);
			this.butRemoveProc.Name = "butRemoveProc";
			this.butRemoveProc.Size = new System.Drawing.Size(78, 24);
			this.butRemoveProc.TabIndex = 136;
			this.butRemoveProc.Text = "Remove";
			this.butRemoveProc.Click += new System.EventHandler(this.butRemoveProc_Click);
			// 
			// butAddProc
			// 
			this.butAddProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddProc.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddProc.Location = new System.Drawing.Point(241, 58);
			this.butAddProc.Name = "butAddProc";
			this.butAddProc.Size = new System.Drawing.Size(78, 24);
			this.butAddProc.TabIndex = 135;
			this.butAddProc.Text = "Add";
			this.butAddProc.Click += new System.EventHandler(this.butAddProc_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(241, 437);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(78, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butSync
			// 
			this.butSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSync.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSync.Location = new System.Drawing.Point(241, 118);
			this.butSync.Name = "butSync";
			this.butSync.Size = new System.Drawing.Size(78, 24);
			this.butSync.TabIndex = 139;
			this.butSync.Text = "Sync Procs";
			this.butSync.Click += new System.EventHandler(this.butSync_Click);
			// 
			// FormDefaultCCProcs
			// 
			this.ClientSize = new System.Drawing.Size(332, 473);
			this.Controls.Add(this.butSync);
			this.Controls.Add(this.listProcs);
			this.Controls.Add(this.butRemoveProc);
			this.Controls.Add(this.butAddProc);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label15);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDefaultCCProcs";
			this.Text = "Default Procedures";
			this.Load += new System.EventHandler(this.FormDefaultCCProcs_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.ListBox listProcs;
		private UI.Button butRemoveProc;
		private System.Windows.Forms.Label label15;
		private UI.Button butAddProc;
		private UI.Button butSync;
	}
}