namespace OpenDental.UI.Progress
{
	partial class FormProgressAuto
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.butCancel = new OpenDental.UI.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelMsg = new System.Windows.Forms.Label();
			this.textHistoryMsg = new System.Windows.Forms.TextBox();
			this.butCopy = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(495, 362);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(51, 66);
			this.progressBar.MarqueeAnimationSpeed = 50;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(363, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 7;
			// 
			// labelMsg
			// 
			this.labelMsg.Location = new System.Drawing.Point(48, 25);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(366, 36);
			this.labelMsg.TabIndex = 6;
			this.labelMsg.Text = "Please Wait...";
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textHistoryMsg
			// 
			this.textHistoryMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textHistoryMsg.Location = new System.Drawing.Point(51, 108);
			this.textHistoryMsg.Multiline = true;
			this.textHistoryMsg.Name = "textHistoryMsg";
			this.textHistoryMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textHistoryMsg.Size = new System.Drawing.Size(418, 277);
			this.textHistoryMsg.TabIndex = 10;
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy.Location = new System.Drawing.Point(495, 314);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 23);
			this.butCopy.TabIndex = 11;
			this.butCopy.Text = "Copy to CB";
			this.butCopy.UseVisualStyleBackColor = true;
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// FormProgressAuto
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(597, 406);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.textHistoryMsg);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.labelMsg);
			this.Name = "FormProgressAuto";
			this.Text = "Progress";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormProgressAuto_CloseXClicked);
			this.Load += new System.EventHandler(this.FormProgressAuto_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelMsg;
		private System.Windows.Forms.TextBox textHistoryMsg;
		private Button butCopy;
	}
}