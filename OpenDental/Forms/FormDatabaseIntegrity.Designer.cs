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
			this.butOK = new OpenDental.UI.Button();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelUrl = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(362, 182);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// linkLabel
			// 
			this.linkLabel.Location = new System.Drawing.Point(139, 121);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(230, 13);
			this.linkLabel.TabIndex = 4;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "https://www.opendental.com/site/integrity.html";
			this.linkLabel.Click += new System.EventHandler(this.linkLabel1_Click);
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(12, 21);
			this.labelMessage.MaximumSize = new System.Drawing.Size(450, 0);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(368, 13);
			this.labelMessage.TabIndex = 5;
			this.labelMessage.Text = "This Object was modified outside of Open Dental.";
			// 
			// labelUrl
			// 
			this.labelUrl.Location = new System.Drawing.Point(12, 121);
			this.labelUrl.Name = "labelUrl";
			this.labelUrl.Size = new System.Drawing.Size(126, 13);
			this.labelUrl.TabIndex = 6;
			this.labelUrl.Text = "For more information visit:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(405, 59);
			this.label1.TabIndex = 8;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.UseMnemonic = false;
			// 
			// FormDatabaseIntegrity
			// 
			this.ClientSize = new System.Drawing.Size(448, 215);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelUrl);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.linkLabel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDatabaseIntegrity";
			this.Text = "Database Integrity";
			this.Load += new System.EventHandler(this.FormDatabaseIntegrity_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.LinkLabel linkLabel;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelUrl;
		private System.Windows.Forms.Label label1;
	}
}