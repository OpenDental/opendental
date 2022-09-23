namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	partial class FormProgressViewer
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
			this.components = new System.ComponentModel.Container();
			this.butOK = new System.Windows.Forms.Button();
			this.lblMessages = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.butViewLog = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.butCancel = new System.Windows.Forms.Button();
			this.butStart = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(3, 91);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// lblMessages
			// 
			this.lblMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblMessages.Location = new System.Drawing.Point(0, 0);
			this.lblMessages.Name = "lblMessages";
			this.lblMessages.Size = new System.Drawing.Size(409, 122);
			this.lblMessages.TabIndex = 1;
			this.lblMessages.Text = "Messages";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(409, 99);
			this.richTextBox1.TabIndex = 2;
			this.richTextBox1.Text = "";
			this.richTextBox1.Visible = false;
			// 
			// butViewLog
			// 
			this.butViewLog.Location = new System.Drawing.Point(3, 3);
			this.butViewLog.Name = "butViewLog";
			this.butViewLog.Size = new System.Drawing.Size(75, 23);
			this.butViewLog.TabIndex = 4;
			this.butViewLog.Text = "View Log";
			this.butViewLog.UseVisualStyleBackColor = true;
			this.butViewLog.Click += new System.EventHandler(this.butViewLog_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar1.Location = new System.Drawing.Point(0, 99);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(409, 23);
			this.progressBar1.TabIndex = 4;
			// 
			// butCancel
			// 
			this.butCancel.Enabled = false;
			this.butCancel.Location = new System.Drawing.Point(84, 91);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butStart
			// 
			this.butStart.Location = new System.Drawing.Point(3, 66);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(75, 23);
			this.butStart.TabIndex = 3;
			this.butStart.Text = "Start";
			this.butStart.UseVisualStyleBackColor = true;
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bw_ProgressChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.richTextBox1);
			this.panel1.Controls.Add(this.progressBar1);
			this.panel1.Controls.Add(this.lblMessages);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(409, 122);
			this.panel1.TabIndex = 6;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.Controls.Add(this.butViewLog);
			this.panel3.Controls.Add(this.butOK);
			this.panel3.Controls.Add(this.butCancel);
			this.panel3.Controls.Add(this.butStart);
			this.panel3.Location = new System.Drawing.Point(425, 12);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(166, 122);
			this.panel3.TabIndex = 8;
			// 
			// FormProgressViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(596, 144);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel1);
			this.MaximumSize = new System.Drawing.Size(1000, 800);
			this.MinimumSize = new System.Drawing.Size(525, 166);
			this.Name = "FormProgressViewer";
			this.Text = "FormProgressViewer";
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Label lblMessages;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button butViewLog;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butStart;
		private System.Windows.Forms.Timer timer1;
		public System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
	}
}