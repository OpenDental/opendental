namespace CodeBase {
	partial class ODProgressBar {
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelLeftText = new System.Windows.Forms.Label();
			this.labelTopText = new System.Windows.Forms.Label();
			this.labelPercentComplete = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(104, 22);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(286, 23);
			this.progressBar.TabIndex = 0;
			// 
			// labelLeftText
			// 
			this.labelLeftText.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelLeftText.Location = new System.Drawing.Point(3, 19);
			this.labelLeftText.Name = "labelLeftText";
			this.labelLeftText.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelLeftText.Size = new System.Drawing.Size(95, 26);
			this.labelLeftText.TabIndex = 1;
			this.labelLeftText.Text = "title";
			this.labelLeftText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTopText
			// 
			this.labelTopText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTopText.Location = new System.Drawing.Point(6, 3);
			this.labelTopText.Name = "labelTopText";
			this.labelTopText.Size = new System.Drawing.Size(461, 16);
			this.labelTopText.TabIndex = 2;
			this.labelTopText.Text = "topTitle";
			this.labelTopText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelPercentComplete
			// 
			this.labelPercentComplete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelPercentComplete.Location = new System.Drawing.Point(396, 26);
			this.labelPercentComplete.Name = "labelPercentComplete";
			this.labelPercentComplete.Size = new System.Drawing.Size(71, 13);
			this.labelPercentComplete.TabIndex = 4;
			this.labelPercentComplete.Text = "0%";
			this.labelPercentComplete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ODProgressBar
			// 
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.labelPercentComplete);
			this.Controls.Add(this.labelTopText);
			this.Controls.Add(this.labelLeftText);
			this.Name = "ODProgressBar";
			this.Size = new System.Drawing.Size(470, 51);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelLeftText;
		private System.Windows.Forms.Label labelTopText;
		private System.Windows.Forms.Label labelPercentComplete;
	}
}
