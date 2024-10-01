namespace OpenDental{
	partial class FormImageFloat {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageFloat));
			this.SuspendLayout();
			// 
			// FormImageFloat
			// 
			this.ClientSize = new System.Drawing.Size(431, 376);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageFloat";
			this.ShowIcon = false;
			this.Text = "Image Float";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormImageFloat_FormClosed);
			this.Load += new System.EventHandler(this.FormImageFloat_Load);
			this.LocationChanged += new System.EventHandler(this.FormImageFloat_LocationChanged);
			this.ResumeLayout(false);

		}

		#endregion
	}
}