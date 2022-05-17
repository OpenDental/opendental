namespace OpenDental{
	partial class FormFeeSchedPickRemote {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedPickRemote));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridFeeSchedFiles = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(277,525);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(358,525);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridFeeSchedFiles
			// 
			this.gridFeeSchedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridFeeSchedFiles.Location = new System.Drawing.Point(12,12);
			this.gridFeeSchedFiles.Name = "gridFeeSchedFiles";
			this.gridFeeSchedFiles.Size = new System.Drawing.Size(421,503);
			this.gridFeeSchedFiles.TabIndex = 140;
			this.gridFeeSchedFiles.Title = "Fee Sched Files";
			this.gridFeeSchedFiles.TranslationName = "TableApptProcs";
			this.gridFeeSchedFiles.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFeeSchedFiles_CellDoubleClick);
			this.gridFeeSchedFiles.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFeeSchedFiles_CellClick);
			// 
			// FormFeeSchedPickRemote
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(445,561);
			this.Controls.Add(this.gridFeeSchedFiles);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeeSchedPickRemote";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Fee Sched Pick Remote";
			this.Load += new System.EventHandler(this.FormFeeSchedPickRemote_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridFeeSchedFiles;
	}
}