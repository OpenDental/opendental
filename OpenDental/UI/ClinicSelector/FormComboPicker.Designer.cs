namespace OpenDental.UI {
	partial class FormComboPicker {
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
			this.SuspendLayout();
			// 
			// FormComboPicker
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(29, 73);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Name = "FormComboPicker";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Deactivate += new System.EventHandler(this.FormComboPicker_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormComboPicker_FormClosing);
			this.Load += new System.EventHandler(this.FormComboPicker_Load);
			this.Shown += new System.EventHandler(this.FormComboPicker_Shown);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormComboPicker_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormComboPicker_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormComboPicker_KeyUp);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormComboPicker_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormComboPicker_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormComboPicker_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion
	}
}