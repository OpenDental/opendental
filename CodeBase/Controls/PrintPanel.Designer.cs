namespace CodeBase {
	partial class PrintPanel {
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
			this.panelSurface = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelSurface
			// 
			this.panelSurface.Location = new System.Drawing.Point(0,0);
			this.panelSurface.Name = "panelSurface";
			this.panelSurface.Size = new System.Drawing.Size(850,1100);
			this.panelSurface.TabIndex = 0;
			this.panelSurface.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSurface_Paint);
			this.panelSurface.SizeChanged += new System.EventHandler(this.panelSurface_SizeChanged);
			// 
			// PrintPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.panelSurface);
			this.DoubleBuffered = true;
			this.Name = "PrintPanel";
			this.Size = new System.Drawing.Size(850,1100);
			this.SizeChanged += new System.EventHandler(this.PrintPanel_SizeChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelSurface;
	}
}
