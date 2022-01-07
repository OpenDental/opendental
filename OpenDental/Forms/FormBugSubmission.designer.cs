namespace OpenDental{
	partial class FormBugSubmission {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBugSubmission));
			this.butAddViewJob = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelDateTime = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butAddViewBug = new OpenDental.UI.Button();
			this.bugSubmissionControl = new OpenDental.UI.BugSubmissionControl();
			this.labelHashNum = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butAddViewJob
			// 
			this.butAddViewJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddViewJob.Location = new System.Drawing.Point(514, 611);
			this.butAddViewJob.MinimumSize = new System.Drawing.Size(80, 24);
			this.butAddViewJob.Name = "butAddViewJob";
			this.butAddViewJob.Size = new System.Drawing.Size(80, 24);
			this.butAddViewJob.TabIndex = 3;
			this.butAddViewJob.Text = "&Add Job";
			this.butAddViewJob.Click += new System.EventHandler(this.butAddViewJob_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(600, 611);
			this.butClose.MinimumSize = new System.Drawing.Size(80, 24);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(80, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(81, 7);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(215, 13);
			this.labelName.TabIndex = 32;
			this.labelName.Text = "XXXXX";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 31;
			this.label2.Text = "Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDateTime
			// 
			this.labelDateTime.Location = new System.Drawing.Point(81, 21);
			this.labelDateTime.Name = "labelDateTime";
			this.labelDateTime.Size = new System.Drawing.Size(215, 13);
			this.labelDateTime.TabIndex = 34;
			this.labelDateTime.Text = "XXXXX";
			this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 13);
			this.label4.TabIndex = 33;
			this.label4.Text = "DateTime:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(371, 7);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(215, 13);
			this.labelVersion.TabIndex = 36;
			this.labelVersion.Text = "XXXXX";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(302, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 35;
			this.label3.Text = "Version:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAddViewBug
			// 
			this.butAddViewBug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddViewBug.Location = new System.Drawing.Point(428, 611);
			this.butAddViewBug.MinimumSize = new System.Drawing.Size(80, 24);
			this.butAddViewBug.Name = "butAddViewBug";
			this.butAddViewBug.Size = new System.Drawing.Size(80, 24);
			this.butAddViewBug.TabIndex = 37;
			this.butAddViewBug.Text = "&Add Bug";
			this.butAddViewBug.Click += new System.EventHandler(this.butAddViewBug_Click);
			// 
			// bugSubmissionControl
			// 
			this.bugSubmissionControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bugSubmissionControl.ControlMode = BugSubmissionControlMode.Specific;
			this.bugSubmissionControl.Location = new System.Drawing.Point(4, 37);
			this.bugSubmissionControl.MinimumSize = new System.Drawing.Size(594, 521);
			this.bugSubmissionControl.Name = "bugSubmissionControl";
			this.bugSubmissionControl.Size = new System.Drawing.Size(680, 599);
			this.bugSubmissionControl.TabIndex = 38;
			// 
			// labelHashNum
			// 
			this.labelHashNum.Location = new System.Drawing.Point(371, 21);
			this.labelHashNum.Name = "labelHashNum";
			this.labelHashNum.Size = new System.Drawing.Size(215, 13);
			this.labelHashNum.TabIndex = 40;
			this.labelHashNum.Text = "XXXXX";
			this.labelHashNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(302, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(65, 13);
			this.label5.TabIndex = 39;
			this.label5.Text = "HashNum:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormBugSubmission
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(692, 642);
			this.Controls.Add(this.labelHashNum);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butAddViewBug);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelDateTime);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butAddViewJob);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.bugSubmissionControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBugSubmission";
			this.Text = "Bug Submissions";
			this.Load += new System.EventHandler(this.FormBugSubmission_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butAddViewJob;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelDateTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label label3;
		private UI.Button butAddViewBug;
		private UI.BugSubmissionControl bugSubmissionControl;
		private System.Windows.Forms.Label labelHashNum;
		private System.Windows.Forms.Label label5;
	}
}