namespace SparksToothChart {
	partial class ToothChartWrapper {
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
			this.toothChart2D = new SparksToothChart.ToothChart2D();
			this.SuspendLayout();
			// 
			// toothChart2D
			// 
			this.toothChart2D.Location = new System.Drawing.Point(0,0);
			this.toothChart2D.Name = "toothChart2D";
			this.toothChart2D.Size = new System.Drawing.Size(410,307);
			this.toothChart2D.TabIndex = 0;
			// 
			// ToothChartWrapper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toothChart2D);
			this.Name = "ToothChartWrapper";
			this.Size = new System.Drawing.Size(544,351);
			this.ResumeLayout(false);

		}

		#endregion

		private ToothChart2D toothChart2D;

		

	}
}
