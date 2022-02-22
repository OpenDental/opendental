namespace OpenDental {
	partial class FormScreenshotBrowse {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScreenshotBrowse));
			this.listFiles = new OpenDental.UI.ListBoxOD();
			this.pictureBoxMain = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
			this.SuspendLayout();
			// 
			// listFiles
			// 
			this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listFiles.IntegralHeight = false;
			this.listFiles.Location = new System.Drawing.Point(0, 0);
			this.listFiles.Name = "listFiles";
			this.listFiles.Size = new System.Drawing.Size(157, 707);
			this.listFiles.TabIndex = 0;
			this.listFiles.SelectedIndexChanged += new System.EventHandler(this.listFiles_SelectedIndexChanged);
			// 
			// pictureBoxMain
			// 
			this.pictureBoxMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxMain.Location = new System.Drawing.Point(159, 0);
			this.pictureBoxMain.Name = "pictureBoxMain";
			this.pictureBoxMain.Size = new System.Drawing.Size(829, 707);
			this.pictureBoxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxMain.TabIndex = 1;
			this.pictureBoxMain.TabStop = false;
			// 
			// FormScreenshotBrowse
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(988, 707);
			this.Controls.Add(this.pictureBoxMain);
			this.Controls.Add(this.listFiles);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormScreenshotBrowse";
			this.Text = "FormScreenshotBrowse";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormScreenshotBrowse_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.ListBoxOD listFiles;
		private System.Windows.Forms.PictureBox pictureBoxMain;
	}
}