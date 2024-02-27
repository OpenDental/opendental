namespace OpenDental{
	partial class FormTeamSummary {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTeamSummary));
			this.butDraftEmail = new OpenDental.UI.Button();
			this.tabControlSummary = new OpenDental.UI.TabControl();
			this.SuspendLayout();
			// 
			// butDraftEmail
			// 
			this.butDraftEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDraftEmail.Location = new System.Drawing.Point(845, 660);
			this.butDraftEmail.Name = "butDraftEmail";
			this.butDraftEmail.Size = new System.Drawing.Size(75, 24);
			this.butDraftEmail.TabIndex = 3;
			this.butDraftEmail.Text = "Draft Email";
			this.butDraftEmail.Click += new System.EventHandler(this.butDraftEmail_Click);
			// 
			// tabControlSummary
			// 
			this.tabControlSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlSummary.Location = new System.Drawing.Point(0, 12);
			this.tabControlSummary.Name = "tabControlSummary";
			this.tabControlSummary.PaddingTabPages = 4;
			this.tabControlSummary.Size = new System.Drawing.Size(932, 642);
			this.tabControlSummary.TabIndex = 74;
			// 
			// FormJobTeamSummary
			// 
			this.ClientSize = new System.Drawing.Size(932, 696);
			this.Controls.Add(this.butDraftEmail);
			this.Controls.Add(this.tabControlSummary);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobTeamSummary";
			this.Text = "Team Weekly Summary";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butDraftEmail;
		private UI.TabControl tabControlSummary;
	}
}