namespace OpenDental{
	partial class FormCloudSessionLimit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloudSessionLimit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.validNumNewSessions = new OpenDental.ValidNum();
			this.labelMonthlyCost = new System.Windows.Forms.Label();
			this.labelDesiredSessions = new System.Windows.Forms.Label();
			this.labelCurrentSessions = new System.Windows.Forms.Label();
			this.textCurrentSessions = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(170, 133);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(251, 133);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// validNumNewSessions
			// 
			this.validNumNewSessions.Location = new System.Drawing.Point(215, 96);
			this.validNumNewSessions.MinVal = 1;
			this.validNumNewSessions.Name = "validNumNewSessions";
			this.validNumNewSessions.ShowZero = false;
			this.validNumNewSessions.Size = new System.Drawing.Size(40, 20);
			this.validNumNewSessions.TabIndex = 5;
			this.validNumNewSessions.Text = "1";
			// 
			// labelMonthlyCost
			// 
			this.labelMonthlyCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMonthlyCost.Location = new System.Drawing.Point(13, 13);
			this.labelMonthlyCost.Name = "labelMonthlyCost";
			this.labelMonthlyCost.Size = new System.Drawing.Size(313, 14);
			this.labelMonthlyCost.TabIndex = 8;
			this.labelMonthlyCost.Text = "Your current agreement includes XX  concurrent sessions.";
			this.labelMonthlyCost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDesiredSessions
			// 
			this.labelDesiredSessions.Location = new System.Drawing.Point(13, 98);
			this.labelDesiredSessions.Name = "labelDesiredSessions";
			this.labelDesiredSessions.Size = new System.Drawing.Size(196, 20);
			this.labelDesiredSessions.TabIndex = 10;
			this.labelDesiredSessions.Text = "Change Maximum Concurrent Sessions to";
			// 
			// labelCurrentSessions
			// 
			this.labelCurrentSessions.Location = new System.Drawing.Point(13, 73);
			this.labelCurrentSessions.Name = "labelCurrentSessions";
			this.labelCurrentSessions.Size = new System.Drawing.Size(196, 23);
			this.labelCurrentSessions.TabIndex = 11;
			this.labelCurrentSessions.Text = "Current maximum concurrent sessions";
			// 
			// textCurrentSessions
			// 
			this.textCurrentSessions.Location = new System.Drawing.Point(215, 71);
			this.textCurrentSessions.Name = "textCurrentSessions";
			this.textCurrentSessions.ReadOnly = true;
			this.textCurrentSessions.Size = new System.Drawing.Size(40, 20);
			this.textCurrentSessions.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(13, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(313, 31);
			this.label1.TabIndex = 13;
			this.label1.Text = "Each additional session costs $15/month. You will only be charged for sessions ac" +
    "tually used each month.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormCloudSessionLimit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(338, 169);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCurrentSessions);
			this.Controls.Add(this.labelCurrentSessions);
			this.Controls.Add(this.labelDesiredSessions);
			this.Controls.Add(this.labelMonthlyCost);
			this.Controls.Add(this.validNumNewSessions);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCloudSessionLimit";
			this.Text = "Change Session Limit";
			this.Load += new System.EventHandler(this.FormCloudSessionLimit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidNum validNumNewSessions;
		private System.Windows.Forms.Label labelMonthlyCost;
		private System.Windows.Forms.Label labelDesiredSessions;
		private System.Windows.Forms.Label labelCurrentSessions;
		private System.Windows.Forms.TextBox textCurrentSessions;
		private System.Windows.Forms.Label label1;
	}
}