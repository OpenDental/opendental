namespace UpdateFileCopier {
	partial class FormMain {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.labelFile = new System.Windows.Forms.Label();
			this.butRetry = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelFile
			// 
			this.labelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFile.Location = new System.Drawing.Point(12, 9);
			this.labelFile.Name = "labelFile";
			this.labelFile.Size = new System.Drawing.Size(360, 87);
			this.labelFile.TabIndex = 0;
			this.labelFile.Text = "Preparing to Copy Files...";
			this.labelFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRetry
			// 
			this.butRetry.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butRetry.Location = new System.Drawing.Point(155, 106);
			this.butRetry.Name = "butRetry";
			this.butRetry.Size = new System.Drawing.Size(75, 23);
			this.butRetry.TabIndex = 1;
			this.butRetry.Text = "Retry";
			this.butRetry.UseVisualStyleBackColor = true;
			this.butRetry.Visible = false;
			this.butRetry.Click += new System.EventHandler(this.butRetry_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 141);
			this.Controls.Add(this.butRetry);
			this.Controls.Add(this.labelFile);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(318, 151);
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Update File Copier";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.Shown += new System.EventHandler(this.FormMain_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelFile;
		private System.Windows.Forms.Button butRetry;
	}
}

