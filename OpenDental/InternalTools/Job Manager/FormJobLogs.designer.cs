namespace OpenDental{
	partial class FormJobLogs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobLogs));
			this.butClose = new OpenDental.UI.Button();
			this.gridLog = new OpenDental.UI.GridOD();
			this.checkShowHistoryText = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(736, 339);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 16;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridLog
			// 
			this.gridLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLog.Location = new System.Drawing.Point(0, 17);
			this.gridLog.Name = "gridLog";
			this.gridLog.Size = new System.Drawing.Size(823, 316);
			this.gridLog.TabIndex = 246;
			this.gridLog.TabStop = false;
			this.gridLog.Title = "Log Events";
			this.gridLog.TranslationName = "TableHistoryEvents";
			this.gridLog.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridLog_CellDoubleClick);
			// 
			// checkShowHistoryText
			// 
			this.checkShowHistoryText.AutoSize = true;
			this.checkShowHistoryText.Dock = System.Windows.Forms.DockStyle.Top;
			this.checkShowHistoryText.Location = new System.Drawing.Point(0, 0);
			this.checkShowHistoryText.Name = "checkShowHistoryText";
			this.checkShowHistoryText.Size = new System.Drawing.Size(823, 17);
			this.checkShowHistoryText.TabIndex = 247;
			this.checkShowHistoryText.Text = "Show Full Job Descriptions";
			this.checkShowHistoryText.UseVisualStyleBackColor = true;
			this.checkShowHistoryText.CheckedChanged += new System.EventHandler(this.checkShowHistoryText_CheckedChanged);
			// 
			// FormJobLogs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(823, 375);
			this.Controls.Add(this.gridLog);
			this.Controls.Add(this.checkShowHistoryText);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobLogs";
			this.Text = "Job Logs";
			this.Load += new System.EventHandler(this.FormJobLogs_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridLog;
		private System.Windows.Forms.CheckBox checkShowHistoryText;
	}
}