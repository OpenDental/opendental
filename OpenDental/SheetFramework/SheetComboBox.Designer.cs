namespace OpenDental {
	partial class SheetComboBox {
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
			this.SuspendLayout();
			// 
			// SheetComboBox
			// 
			this.Enter += new System.EventHandler(this.SheetComboBox_Enter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SheetComboBox_KeyDown);
			this.Leave += new System.EventHandler(this.SheetComboBox_Leave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SheetComboBox_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
