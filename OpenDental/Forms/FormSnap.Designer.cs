namespace OpenDental{
	partial class FormSnap {
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
				_graphicsPathHalf_L?.Dispose();
				_graphicsPathQuarter_UL?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSnap));
			this.timerClose = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// timerClose
			// 
			this.timerClose.Interval = 800;
			this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
			// 
			// FormSnap
			// 
			this.ClientSize = new System.Drawing.Size(625, 358);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSnap";
			this.ShowInTaskbar = false;
			this.Text = "Basic Template";
			this.Load += new System.EventHandler(this.FormSnap_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormSnap_Paint);
			this.MouseEnter += new System.EventHandler(this.FormSnap_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.FormSnap_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormSnap_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerClose;
	}
}