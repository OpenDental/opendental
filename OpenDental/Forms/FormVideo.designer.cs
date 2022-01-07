namespace OpenDental{
	partial class FormVideo {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVideo));
			this.butClose = new OpenDental.UI.Button();
			this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
			this.butCapture = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboCameras = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(341, 282);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.TabStop = false;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// pictureBoxCamera
			// 
			this.pictureBoxCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxCamera.Location = new System.Drawing.Point(3, 29);
			this.pictureBoxCamera.Name = "pictureBoxCamera";
			this.pictureBoxCamera.Size = new System.Drawing.Size(413, 250);
			this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxCamera.TabIndex = 6;
			this.pictureBoxCamera.TabStop = false;
			// 
			// butCapture
			// 
			this.butCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCapture.Location = new System.Drawing.Point(3, 282);
			this.butCapture.Name = "butCapture";
			this.butCapture.Size = new System.Drawing.Size(75, 24);
			this.butCapture.TabIndex = 1;
			this.butCapture.Text = "Capture";
			this.butCapture.Click += new System.EventHandler(this.butCapture_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Camera";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCameras
			// 
			this.comboCameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCameras.FormattingEnabled = true;
			this.comboCameras.Location = new System.Drawing.Point(64, 4);
			this.comboCameras.Name = "comboCameras";
			this.comboCameras.Size = new System.Drawing.Size(239, 21);
			this.comboCameras.TabIndex = 2;
			this.comboCameras.SelectionChangeCommitted += new System.EventHandler(this.comboCameras_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(82, 285);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 18);
			this.label2.TabIndex = 10;
			this.label2.Text = "(or Space)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormVideo
			// 
			this.ClientSize = new System.Drawing.Size(419, 309);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboCameras);
			this.Controls.Add(this.butCapture);
			this.Controls.Add(this.pictureBoxCamera);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormVideo";
			this.Text = "Video Capture";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVideo_FormClosing);
			this.Load += new System.EventHandler(this.FormVideo_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormVideo_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.PictureBox pictureBoxCamera;
		private UI.Button butCapture;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboCameras;
		private System.Windows.Forms.Label label2;
	}
}