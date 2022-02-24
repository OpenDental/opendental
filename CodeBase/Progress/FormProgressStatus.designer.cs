namespace CodeBase {
	partial class FormProgressStatus {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgressStatus));
			this.labelMsg = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.textHistoryMsg = new System.Windows.Forms.TextBox();
			this.butCopyToClipboard = new System.Windows.Forms.Button();
			this.butClose = new System.Windows.Forms.Button();
			this.panelMinimize = new System.Windows.Forms.Panel();
			this.labelMinimize = new System.Windows.Forms.Label();
			this.panelMinimize.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMsg
			// 
			this.labelMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMsg.Location = new System.Drawing.Point(52, 36);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(366, 36);
			this.labelMsg.TabIndex = 1;
			this.labelMsg.Text = "Please Wait...";
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.progressBar.Location = new System.Drawing.Point(55, 75);
			this.progressBar.MarqueeAnimationSpeed = 50;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(363, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 2;
			// 
			// textHistoryMsg
			// 
			this.textHistoryMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textHistoryMsg.Location = new System.Drawing.Point(12, 23);
			this.textHistoryMsg.Multiline = true;
			this.textHistoryMsg.Name = "textHistoryMsg";
			this.textHistoryMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textHistoryMsg.Size = new System.Drawing.Size(445, 49);
			this.textHistoryMsg.TabIndex = 3;
			this.textHistoryMsg.Visible = false;
			// 
			// butCopyToClipboard
			// 
			this.butCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopyToClipboard.Location = new System.Drawing.Point(12, 75);
			this.butCopyToClipboard.Name = "butCopyToClipboard";
			this.butCopyToClipboard.Size = new System.Drawing.Size(100, 23);
			this.butCopyToClipboard.TabIndex = 5;
			this.butCopyToClipboard.Text = "Copy to Clipboard";
			this.butCopyToClipboard.UseVisualStyleBackColor = true;
			this.butCopyToClipboard.Visible = false;
			this.butCopyToClipboard.Click += new System.EventHandler(this.butCopyToClipboard_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(382, 75);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Visible = false;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// panelMinimize
			// 
			this.panelMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMinimize.Controls.Add(this.labelMinimize);
			this.panelMinimize.Location = new System.Drawing.Point(445, 1);
			this.panelMinimize.Name = "panelMinimize";
			this.panelMinimize.Size = new System.Drawing.Size(22, 22);
			this.panelMinimize.TabIndex = 7;
			// 
			// labelMinimize
			// 
			this.labelMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMinimize.Location = new System.Drawing.Point(0, 0);
			this.labelMinimize.Name = "labelMinimize";
			this.labelMinimize.Size = new System.Drawing.Size(22, 22);
			this.labelMinimize.TabIndex = 0;
			this.labelMinimize.Text = "_";
			this.labelMinimize.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
			this.labelMinimize.MouseEnter += new System.EventHandler(this.labelMinimize_MouseEnter);
			this.labelMinimize.MouseLeave += new System.EventHandler(this.labelMinimize_MouseLeave);
			// 
			// FormProgressStatus
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(469, 117);
			this.Controls.Add(this.panelMinimize);
			this.Controls.Add(this.butCopyToClipboard);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.labelMsg);
			this.Controls.Add(this.textHistoryMsg);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "FormProgressStatus";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.panelMinimize.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelMsg;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.TextBox textHistoryMsg;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butCopyToClipboard;
		private System.Windows.Forms.Panel panelMinimize;
		private System.Windows.Forms.Label labelMinimize;
	}
}