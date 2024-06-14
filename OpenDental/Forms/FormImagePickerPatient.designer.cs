namespace OpenDental{
	partial class FormImagePickerPatient {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImagePickerPatient));
			this.butOK = new OpenDental.UI.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.elementHostImageSelector = new System.Windows.Forms.Integration.ElementHost();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1143, 660);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 19;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(254, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(883, 672);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 22;
			this.pictureBox.TabStop = false;
			// 
			// elementHostImageSelector
			// 
			this.elementHostImageSelector.Location = new System.Drawing.Point(11, 12);
			this.elementHostImageSelector.Name = "elementHostImageSelector";
			this.elementHostImageSelector.Size = new System.Drawing.Size(237, 672);
			this.elementHostImageSelector.TabIndex = 39;
			this.elementHostImageSelector.Text = "elementHost1";
			this.elementHostImageSelector.Child = null;
			// 
			// FormImagePickerPatient
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.elementHostImageSelector);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImagePickerPatient";
			this.Text = "Select Image";
			this.Load += new System.EventHandler(this.FormImagePickerPatient_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butOK;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Integration.ElementHost elementHostImageSelector;
	}
}