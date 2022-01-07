namespace OpenDental.UI {
	partial class LightSignalGrid {
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.contextMenuConfKick = new System.Windows.Forms.ContextMenu();
			this.menuItemKick = new System.Windows.Forms.MenuItem();
			this.menuItemKick.Click+=new System.EventHandler(this.menuItemKick_Click);
			this.SuspendLayout();
			// 
			// contextMenuConfKick
			// 
			this.contextMenuConfKick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemKick});
			// 
			// menuItemSeatedNow
			// 
			this.menuItemKick.Index = 0;
			this.menuItemKick.Text = "Kick";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenu contextMenuConfKick;
		private System.Windows.Forms.MenuItem menuItemKick;
	}
}
