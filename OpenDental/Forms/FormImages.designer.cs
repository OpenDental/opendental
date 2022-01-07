namespace OpenDental{
	partial class FormImages {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImages));
			this.contrImagesMain = new OpenDental.ControlImages();
			this.SuspendLayout();
			// 
			// contrImagesMain
			// 
			this.contrImagesMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contrImagesMain.Location = new System.Drawing.Point(0, 0);
			this.contrImagesMain.Name = "contrImagesMain";
			this.contrImagesMain.Size = new System.Drawing.Size(1166, 778);
			this.contrImagesMain.TabIndex = 0;
			// 
			// FormImages
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1166, 778);
			this.Controls.Add(this.contrImagesMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImages";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Images";
			this.Load += new System.EventHandler(this.FormImages_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private ControlImages contrImagesMain;

	}
}