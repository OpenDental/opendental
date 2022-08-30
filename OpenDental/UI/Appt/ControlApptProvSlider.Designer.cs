namespace OpenDental.UI
{
	partial class ControlApptProvSlider
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				_brushProv?.Dispose();
				_fontCourier?.Dispose();
				_fontTime?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timerCursor = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// timerCursor
			// 
			this.timerCursor.Interval = 600;
			this.timerCursor.Tick += new System.EventHandler(this.TimerCursor_Tick);
			// 
			// ContrApptProvSlider
			// 
			this.Name = "ContrApptProvSlider";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerCursor;
	}
}
