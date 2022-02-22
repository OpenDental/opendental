namespace OpenDental{
	partial class FormUpdateInProgress {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdateInProgress));
			this.labelWarning = new System.Windows.Forms.Label();
			this.butTryAgain = new OpenDental.UI.Button();
			this.butOverride = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.Location = new System.Drawing.Point(15, 29);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(539, 122);
			this.labelWarning.TabIndex = 3;
			this.labelWarning.Text = "labelWarning";
			// 
			// butTryAgain
			// 
			this.butTryAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTryAgain.Location = new System.Drawing.Point(398, 160);
			this.butTryAgain.Name = "butTryAgain";
			this.butTryAgain.Size = new System.Drawing.Size(75, 24);
			this.butTryAgain.TabIndex = 1;
			this.butTryAgain.Text = "Try Again";
			this.butTryAgain.Click += new System.EventHandler(this.butTryAgain_Click);
			// 
			// butOverride
			// 
			this.butOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butOverride.Location = new System.Drawing.Point(15, 160);
			this.butOverride.Name = "butOverride";
			this.butOverride.Size = new System.Drawing.Size(75, 24);
			this.butOverride.TabIndex = 4;
			this.butOverride.Text = "Override";
			this.butOverride.Visible = false;
			this.butOverride.Click += new System.EventHandler(this.butOverride_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(479, 160);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormUpdateInProgress
			// 
			this.AcceptButton = this.butTryAgain;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(566, 195);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.butTryAgain);
			this.Controls.Add(this.butOverride);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUpdateInProgress";
			this.Text = "Update In Progress";
			this.Load += new System.EventHandler(this.FormUpdateInProgress_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butTryAgain;
		private OpenDental.UI.Button butOverride;
		private System.Windows.Forms.Label labelWarning;
		private UI.Button butCancel;
	}
}