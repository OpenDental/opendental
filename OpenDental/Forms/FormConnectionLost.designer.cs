namespace OpenDental{
	partial class FormConnectionLost {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConnectionLost));
			this.labelErrMsg = new System.Windows.Forms.Label();
			this.butRetry = new OpenDental.UI.Button();
			this.butExit = new OpenDental.UI.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// labelErrMsg
			// 
			this.labelErrMsg.Location = new System.Drawing.Point(12, 9);
			this.labelErrMsg.Name = "labelErrMsg";
			this.labelErrMsg.Size = new System.Drawing.Size(377, 144);
			this.labelErrMsg.TabIndex = 4;
			this.labelErrMsg.Text = "Error Message";
			// 
			// butRetry
			// 
			this.butRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRetry.Location = new System.Drawing.Point(232, 164);
			this.butRetry.Name = "butRetry";
			this.butRetry.Size = new System.Drawing.Size(75, 24);
			this.butRetry.TabIndex = 3;
			this.butRetry.Text = "Retry";
			this.butRetry.Click += new System.EventHandler(this.butRetry_Click);
			// 
			// butExit
			// 
			this.butExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExit.Location = new System.Drawing.Point(313, 164);
			this.butExit.Name = "butExit";
			this.butExit.Size = new System.Drawing.Size(75, 24);
			this.butExit.TabIndex = 2;
			this.butExit.Text = "Exit Program";
			this.butExit.Click += new System.EventHandler(this.butExit_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(400, 200);
			this.panel1.TabIndex = 5;
			// 
			// FormConnectionLost
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(400, 200);
			this.Controls.Add(this.labelErrMsg);
			this.Controls.Add(this.butRetry);
			this.Controls.Add(this.butExit);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormConnectionLost";
			this.ShowInTaskbar = false;
			this.Text = "Connection Lost";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConnectionLost_FormClosing);
			this.Load += new System.EventHandler(this.FormConnectionLost_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butRetry;
		private OpenDental.UI.Button butExit;
		private System.Windows.Forms.Label labelErrMsg;
		private System.Windows.Forms.Panel panel1;
	}
}