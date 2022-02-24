namespace SparksToothChart {
	partial class ToothChart2D {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToothChart2D));
			this.pictBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictBox
			// 
			this.pictBox.Image = ((System.Drawing.Image)(resources.GetObject("pictBox.Image")));
			this.pictBox.Location = new System.Drawing.Point(0,0);
			this.pictBox.Name = "pictBox";
			this.pictBox.Size = new System.Drawing.Size(410,307);
			this.pictBox.TabIndex = 1;
			this.pictBox.TabStop = false;
			this.pictBox.Visible = false;
			// 
			// ToothChart2D
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictBox);
			this.Name = "ToothChart2D";
			this.Size = new System.Drawing.Size(410,307);
			((System.ComponentModel.ISupportInitialize)(this.pictBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictBox;
	}
}
