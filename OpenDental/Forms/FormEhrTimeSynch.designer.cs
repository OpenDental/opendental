namespace OpenDental{
	partial class FormEhrTimeSynch {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrTimeSynch));
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.timerSendingLimit = new System.Windows.Forms.Timer(this.components);
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textMessage = new OpenDental.ODtextBox();
			this.textLocalTime = new OpenDental.ODtextBox();
			this.textServerTime = new OpenDental.ODtextBox();
			this.textNistTime = new OpenDental.ODtextBox();
			this.butRefreshTime = new OpenDental.UI.Button();
			this.textNistUrl = new OpenDental.ODtextBox();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(25, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 20);
			this.label2.TabIndex = 81;
			this.label2.Text = "NIST server address";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(109, 20);
			this.label1.TabIndex = 82;
			this.label1.Text = "NIST server";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(25, 130);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 20);
			this.label3.TabIndex = 83;
			this.label3.Text = "Database server";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(28, 156);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 20);
			this.label4.TabIndex = 84;
			this.label4.Text = "Local machine";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// timerSendingLimit
			// 
			this.timerSendingLimit.Interval = 4000;
			this.timerSendingLimit.Tick += new System.EventHandler(this.timerSendingLimit_Tick);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(25, 21);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(456, 44);
			this.label6.TabIndex = 87;
			this.label6.Text = resources.GetString("label6.Text");
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(28, 182);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(103, 20);
			this.label7.TabIndex = 92;
			this.label7.Text = "Message";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMessage
			// 
			this.textMessage.AcceptsTab = true;
			this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessage.DetectUrls = false;
			this.textMessage.ForeColor = System.Drawing.Color.DarkRed;
			this.textMessage.Location = new System.Drawing.Point(137, 182);
			this.textMessage.Name = "textMessage";
			this.textMessage.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textMessage.ReadOnly = true;
			this.textMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessage.Size = new System.Drawing.Size(255, 84);
			this.textMessage.TabIndex = 91;
			this.textMessage.Text = "";
			// 
			// textLocalTime
			// 
			this.textLocalTime.AcceptsTab = true;
			this.textLocalTime.DetectUrls = false;
			this.textLocalTime.Location = new System.Drawing.Point(137, 156);
			this.textLocalTime.Multiline = false;
			this.textLocalTime.Name = "textLocalTime";
			this.textLocalTime.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textLocalTime.ReadOnly = true;
			this.textLocalTime.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textLocalTime.Size = new System.Drawing.Size(111, 20);
			this.textLocalTime.TabIndex = 79;
			this.textLocalTime.Text = "";
			// 
			// textServerTime
			// 
			this.textServerTime.AcceptsTab = true;
			this.textServerTime.DetectUrls = false;
			this.textServerTime.Location = new System.Drawing.Point(137, 130);
			this.textServerTime.Multiline = false;
			this.textServerTime.Name = "textServerTime";
			this.textServerTime.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textServerTime.ReadOnly = true;
			this.textServerTime.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textServerTime.Size = new System.Drawing.Size(111, 20);
			this.textServerTime.TabIndex = 78;
			this.textServerTime.Text = "";
			// 
			// textNistTime
			// 
			this.textNistTime.AcceptsTab = true;
			this.textNistTime.DetectUrls = false;
			this.textNistTime.Location = new System.Drawing.Point(137, 104);
			this.textNistTime.Multiline = false;
			this.textNistTime.Name = "textNistTime";
			this.textNistTime.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textNistTime.ReadOnly = true;
			this.textNistTime.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNistTime.Size = new System.Drawing.Size(111, 20);
			this.textNistTime.TabIndex = 77;
			this.textNistTime.Text = "";
			// 
			// butRefreshTime
			// 
			this.butRefreshTime.Location = new System.Drawing.Point(398, 76);
			this.butRefreshTime.Name = "butRefreshTime";
			this.butRefreshTime.Size = new System.Drawing.Size(83, 24);
			this.butRefreshTime.TabIndex = 76;
			this.butRefreshTime.Text = "Synch Time";
			this.butRefreshTime.Click += new System.EventHandler(this.butRefreshTime_Click);
			// 
			// textNistUrl
			// 
			this.textNistUrl.AcceptsTab = true;
			this.textNistUrl.DetectUrls = false;
			this.textNistUrl.Location = new System.Drawing.Point(137, 78);
			this.textNistUrl.Multiline = false;
			this.textNistUrl.Name = "textNistUrl";
			this.textNistUrl.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textNistUrl.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNistUrl.Size = new System.Drawing.Size(255, 20);
			this.textNistUrl.TabIndex = 75;
			this.textNistUrl.Text = "";
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(438, 242);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormEhrTimeSynch
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(538, 293);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textLocalTime);
			this.Controls.Add(this.textServerTime);
			this.Controls.Add(this.textNistTime);
			this.Controls.Add(this.butRefreshTime);
			this.Controls.Add(this.textNistUrl);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrTimeSynch";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Time Synchronization";
			this.Load += new System.EventHandler(this.FormEhrTime_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private ODtextBox textNistUrl;
		private UI.Button butRefreshTime;
		private ODtextBox textNistTime;
		private ODtextBox textServerTime;
		private ODtextBox textLocalTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Timer timerSendingLimit;
		private System.Windows.Forms.Label label6;
		private ODtextBox textMessage;
		private System.Windows.Forms.Label label7;
	}
}