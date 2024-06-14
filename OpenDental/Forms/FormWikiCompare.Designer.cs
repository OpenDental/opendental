namespace OpenDental{
	partial class FormWikiCompare {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiCompare));
			this.textContentRight = new OpenDental.ODcodeBox();
			this.textContentLeft = new OpenDental.ODcodeBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTitleNew = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelHelpNew = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textContentRight
			// 
			this.textContentRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textContentRight.BackColor = System.Drawing.SystemColors.Control;
			this.textContentRight.Location = new System.Drawing.Point(585, 43);
			this.textContentRight.Name = "textContentRight";
			this.textContentRight.ReadOnly = true;
			this.textContentRight.Size = new System.Drawing.Size(567, 606);
			this.textContentRight.TabIndex = 7;
			this.textContentRight.Text = "";
			// 
			// textContentLeft
			// 
			this.textContentLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textContentLeft.BackColor = System.Drawing.SystemColors.Control;
			this.textContentLeft.Location = new System.Drawing.Point(12, 43);
			this.textContentLeft.Name = "textContentLeft";
			this.textContentLeft.ReadOnly = true;
			this.textContentLeft.Size = new System.Drawing.Size(567, 606);
			this.textContentLeft.TabIndex = 6;
			this.textContentLeft.Text = "";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(26, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(553, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "Title of Wikipage Might be Long 12/27/2022 08:00:00";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTitleNew
			// 
			this.labelTitleNew.BackColor = System.Drawing.Color.White;
			this.labelTitleNew.Location = new System.Drawing.Point(600, 24);
			this.labelTitleNew.Name = "labelTitleNew";
			this.labelTitleNew.Size = new System.Drawing.Size(552, 17);
			this.labelTitleNew.TabIndex = 12;
			this.labelTitleNew.Text = "Title of Wikipage Might be Long 12/28/2022 08:00:00";
			this.labelTitleNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.label3.Location = new System.Drawing.Point(26, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(217, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Older version (deleted or moved changes).";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelHelpNew
			// 
			this.labelHelpNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.labelHelpNew.Location = new System.Drawing.Point(600, 6);
			this.labelHelpNew.Name = "labelHelpNew";
			this.labelHelpNew.Size = new System.Drawing.Size(219, 16);
			this.labelHelpNew.TabIndex = 7;
			this.labelHelpNew.Text = "Newer version (added or moved changes).";
			this.labelHelpNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormWikiCompare
			// 
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.ClientSize = new System.Drawing.Size(1164, 661);
			this.Controls.Add(this.labelTitleNew);
			this.Controls.Add(this.labelHelpNew);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textContentRight);
			this.Controls.Add(this.textContentLeft);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiCompare";
			this.Text = "Wiki Compare";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormWikiCompare_Load);
			this.SizeChanged += new System.EventHandler(this.FormWikiCompare_SizeChange);
			this.ResumeLayout(false);

		}

		#endregion
		private ODcodeBox textContentRight;
		private ODcodeBox textContentLeft;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelTitleNew;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelHelpNew;
	}
}