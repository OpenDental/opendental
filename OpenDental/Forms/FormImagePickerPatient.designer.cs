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
			this.butCancel = new OpenDental.UI.Button();
			this.imageSelector = new OpenDental.UI.ImageSelector();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1143, 630);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 19;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1143, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 18;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// imageSelector
			// 
			this.imageSelector.Location = new System.Drawing.Point(12, 12);
			this.imageSelector.Name = "imageSelector";
			this.imageSelector.Size = new System.Drawing.Size(236, 672);
			this.imageSelector.TabIndex = 20;
			this.imageSelector.Text = "imageSelector1";
			this.imageSelector.ItemDoubleClick += new System.EventHandler(this.imageSelector_ItemDoubleClick);
			this.imageSelector.SelectionChangeCommitted += new System.EventHandler(this.imageSelector_SelectionChangeCommitted);
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
			// FormImagePickerPatient
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.imageSelector);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImagePickerPatient";
			this.Text = "Select Image";
			this.Load += new System.EventHandler(this.FormImagePickerPatient_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.ImageSelector imageSelector;
		private System.Windows.Forms.PictureBox pictureBox;
	}
}