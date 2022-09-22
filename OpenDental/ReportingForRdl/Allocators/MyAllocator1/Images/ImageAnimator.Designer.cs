namespace OpenDental.Reporting.Allocators.MyAllocator1.Images
{
	partial class ImageAnimator
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
			this.LoopTimer = new System.Windows.Forms.Timer(this.components);
			this.ImageBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
			this.SuspendLayout();
			// 
			// LoopTimer
			// 
			this.LoopTimer.Tick += new System.EventHandler(this.LoopTimer_Tick);
			// 
			// ImageBox
			// 
			this.ImageBox.Location = new System.Drawing.Point(0, 0);
			this.ImageBox.Margin = new System.Windows.Forms.Padding(0);
			this.ImageBox.Name = "ImageBox";
			this.ImageBox.Size = new System.Drawing.Size(50, 50);
			this.ImageBox.TabIndex = 0;
			this.ImageBox.TabStop = false;
			// 
			// ImageAnimator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ImageBox);
			this.Name = "ImageAnimator";
			this.Size = new System.Drawing.Size(50, 50);
			this.SizeChanged += new System.EventHandler(this.ImageAnimator_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer LoopTimer;
		private System.Windows.Forms.PictureBox ImageBox;
	}
}
