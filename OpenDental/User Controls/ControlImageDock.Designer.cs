namespace OpenDental {
	partial class ControlImageDock {
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
			this.elementHostImageDockHeader = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// elementHostImageDockHeader
			// 
			this.elementHostImageDockHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.elementHostImageDockHeader.Location = new System.Drawing.Point(0, 0);
			this.elementHostImageDockHeader.Name = "elementHostImageDockHeader";
			this.elementHostImageDockHeader.Size = new System.Drawing.Size(664, 23);
			this.elementHostImageDockHeader.TabIndex = 39;
			this.elementHostImageDockHeader.Text = "elementHost1";
			this.elementHostImageDockHeader.Child = null;
			// 
			// ControlImageDock
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.elementHostImageDockHeader);
			this.Name = "ControlImageDock";
			this.Size = new System.Drawing.Size(664, 427);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost elementHostImageDockHeader;
	}
}
