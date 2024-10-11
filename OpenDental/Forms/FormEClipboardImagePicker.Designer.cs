namespace OpenDental{
	partial class FormEClipboardImagePicker {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardImagePicker));
			this.gridImages = new OpenDental.UI.GridOD();
			this.butSave = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridImages
			// 
			this.gridImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridImages.Location = new System.Drawing.Point(12, 12);
			this.gridImages.Name = "gridImages";
			this.gridImages.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridImages.Size = new System.Drawing.Size(377, 467);
			this.gridImages.TabIndex = 0;
			this.gridImages.Title = "Images";
			this.gridImages.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridImages_CellDoubleClick);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(422, 455);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&OK";
			this.butSave.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormEClipboardImagePicker
			// 
			this.ClientSize = new System.Drawing.Size(509, 491);
			this.Controls.Add(this.gridImages);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEClipboardImagePicker";
			this.Text = "eClipboard Image Picker";
			this.Load += new System.EventHandler(this.FormEClipboardImagePicker_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.GridOD gridImages;
	}
}