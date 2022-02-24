namespace OpenDental.UI {
	partial class ODCodeRangeFilter {
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
			this.textCodeRange = new System.Windows.Forms.TextBox();
			this.labelExample = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textCodeRange
			// 
			this.textCodeRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCodeRange.Location = new System.Drawing.Point(0, 0);
			this.textCodeRange.Name = "textCodeRange";
			this.textCodeRange.Size = new System.Drawing.Size(150, 20);
			this.textCodeRange.TabIndex = 46;
			// 
			// labelExample
			// 
			this.labelExample.Location = new System.Drawing.Point(2, 23);
			this.labelExample.Name = "labelExample";
			this.labelExample.Size = new System.Drawing.Size(108, 16);
			this.labelExample.TabIndex = 48;
			this.labelExample.Text = "Ex: D2140-D2394";
			// 
			// ODCodeRangeFilter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelExample);
			this.Controls.Add(this.textCodeRange);
			this.Name = "ODCodeRangeFilter";
			this.Size = new System.Drawing.Size(150, 37);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textCodeRange;
		private System.Windows.Forms.Label labelExample;
	}
}
