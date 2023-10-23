namespace OpenDental{
	partial class FormDatabaseIntegrity {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseIntegrity));
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelUrl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// linkLabel
			// 
			this.linkLabel.Location = new System.Drawing.Point(139, 172);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(303, 13);
			this.linkLabel.TabIndex = 4;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "https://www.opendental.com/site/integrity.html";
			this.linkLabel.Click += new System.EventHandler(this.linkLabel1_Click);
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(12, 18);
			this.labelMessage.MaximumSize = new System.Drawing.Size(450, 0);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(430, 152);
			this.labelMessage.TabIndex = 5;
			this.labelMessage.Text = "Message";
			// 
			// labelUrl
			// 
			this.labelUrl.Location = new System.Drawing.Point(12, 172);
			this.labelUrl.Name = "labelUrl";
			this.labelUrl.Size = new System.Drawing.Size(126, 13);
			this.labelUrl.TabIndex = 6;
			this.labelUrl.Text = "For more information visit:";
			// 
			// FormDatabaseIntegrity
			// 
			this.ClientSize = new System.Drawing.Size(464, 201);
			this.Controls.Add(this.labelUrl);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.linkLabel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDatabaseIntegrity";
			this.Text = "Database Integrity";
			this.Load += new System.EventHandler(this.FormDatabaseIntegrity_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.LinkLabel linkLabel;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelUrl;
	}
}