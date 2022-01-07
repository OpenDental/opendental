namespace OpenDental{
	partial class FormWebChatTools {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebChatTools));
			this.butClose = new OpenDental.UI.Button();
			this.gridWebChatSessions = new OpenDental.UI.GridOD();
			this.checkShowEndedSessions = new System.Windows.Forms.CheckBox();
			this.dateRangeWebChat = new OpenDental.UI.ODDateRangePicker();
			this.labelUsers = new System.Windows.Forms.Label();
			this.comboUsers = new OpenDental.UI.ComboBoxOD();
			this.labelChatTextContains = new System.Windows.Forms.Label();
			this.textChatTextContains = new OpenDental.ODtextBox();
			this.textSessionNum = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.labelCount = new System.Windows.Forms.Label();
			this.labelCountValue = new System.Windows.Forms.Label();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1143, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridWebChatSessions
			// 
			this.gridWebChatSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebChatSessions.Location = new System.Drawing.Point(12, 88);
			this.gridWebChatSessions.Name = "gridWebChatSessions";
			this.gridWebChatSessions.Size = new System.Drawing.Size(1206, 566);
			this.gridWebChatSessions.TabIndex = 4;
			this.gridWebChatSessions.Title = "Web Chat Sessions";
			this.gridWebChatSessions.TranslationName = "gridWebChatSessions";
			this.gridWebChatSessions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridWebChatSessions_CellDoubleClick);
			// 
			// checkShowEndedSessions
			// 
			this.checkShowEndedSessions.Location = new System.Drawing.Point(9, 18);
			this.checkShowEndedSessions.Name = "checkShowEndedSessions";
			this.checkShowEndedSessions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkShowEndedSessions.Size = new System.Drawing.Size(168, 20);
			this.checkShowEndedSessions.TabIndex = 5;
			this.checkShowEndedSessions.Text = "Show Ended Sessions";
			this.checkShowEndedSessions.UseVisualStyleBackColor = true;
			// 
			// dateRangeWebChat
			// 
			this.dateRangeWebChat.BackColor = System.Drawing.Color.Transparent;
			this.dateRangeWebChat.Location = new System.Drawing.Point(9, 44);
			this.dateRangeWebChat.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangeWebChat.Name = "dateRangeWebChat";
			this.dateRangeWebChat.Size = new System.Drawing.Size(453, 24);
			this.dateRangeWebChat.TabIndex = 6;
			// 
			// labelUsers
			// 
			this.labelUsers.Location = new System.Drawing.Point(468, 43);
			this.labelUsers.Name = "labelUsers";
			this.labelUsers.Size = new System.Drawing.Size(83, 20);
			this.labelUsers.TabIndex = 7;
			this.labelUsers.Text = "Users";
			this.labelUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUsers
			// 
			this.comboUsers.BackColor = System.Drawing.SystemColors.Window;
			this.comboUsers.Location = new System.Drawing.Point(557, 44);
			this.comboUsers.Name = "comboUsers";
			this.comboUsers.SelectionModeMulti = true;
			this.comboUsers.Size = new System.Drawing.Size(159, 21);
			this.comboUsers.TabIndex = 9;
			// 
			// labelChatTextContains
			// 
			this.labelChatTextContains.Location = new System.Drawing.Point(418, 16);
			this.labelChatTextContains.Name = "labelChatTextContains";
			this.labelChatTextContains.Size = new System.Drawing.Size(138, 20);
			this.labelChatTextContains.TabIndex = 11;
			this.labelChatTextContains.Text = "Chat Text Contains";
			this.labelChatTextContains.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textChatTextContains
			// 
			this.textChatTextContains.AcceptsTab = true;
			this.textChatTextContains.BackColor = System.Drawing.SystemColors.Window;
			this.textChatTextContains.DetectLinksEnabled = false;
			this.textChatTextContains.DetectUrls = false;
			this.textChatTextContains.Location = new System.Drawing.Point(557, 18);
			this.textChatTextContains.Multiline = false;
			this.textChatTextContains.Name = "textChatTextContains";
			this.textChatTextContains.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textChatTextContains.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textChatTextContains.Size = new System.Drawing.Size(159, 20);
			this.textChatTextContains.TabIndex = 12;
			this.textChatTextContains.Text = "";
			// 
			// textSessionNum
			// 
			this.textSessionNum.AcceptsTab = true;
			this.textSessionNum.BackColor = System.Drawing.SystemColors.Window;
			this.textSessionNum.DetectLinksEnabled = false;
			this.textSessionNum.DetectUrls = false;
			this.textSessionNum.Location = new System.Drawing.Point(319, 18);
			this.textSessionNum.Multiline = false;
			this.textSessionNum.Name = "textSessionNum";
			this.textSessionNum.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSessionNum.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSessionNum.Size = new System.Drawing.Size(93, 20);
			this.textSessionNum.TabIndex = 14;
			this.textSessionNum.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(183, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 20);
			this.label1.TabIndex = 13;
			this.label1.Text = "Session Num";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupFilters
			// 
			this.groupFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilters.Controls.Add(this.checkShowEndedSessions);
			this.groupFilters.Controls.Add(this.textSessionNum);
			this.groupFilters.Controls.Add(this.label1);
			this.groupFilters.Controls.Add(this.dateRangeWebChat);
			this.groupFilters.Controls.Add(this.comboUsers);
			this.groupFilters.Controls.Add(this.textChatTextContains);
			this.groupFilters.Controls.Add(this.labelChatTextContains);
			this.groupFilters.Controls.Add(this.labelUsers);
			this.groupFilters.Location = new System.Drawing.Point(12, 4);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(1206, 78);
			this.groupFilters.TabIndex = 15;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Filters";
			// 
			// labelCount
			// 
			this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelCount.Location = new System.Drawing.Point(9, 666);
			this.labelCount.Name = "labelCount";
			this.labelCount.Size = new System.Drawing.Size(38, 13);
			this.labelCount.TabIndex = 16;
			this.labelCount.Text = "Count:";
			// 
			// labelCountValue
			// 
			this.labelCountValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelCountValue.Location = new System.Drawing.Point(53, 666);
			this.labelCountValue.Name = "labelCountValue";
			this.labelCountValue.Size = new System.Drawing.Size(36, 13);
			this.labelCountValue.TabIndex = 17;
			this.labelCountValue.Text = "0";
			// 
			// FormWebChatTools
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.labelCountValue);
			this.Controls.Add(this.labelCount);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.gridWebChatSessions);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebChatTools";
			this.Text = "Web Chat Tools";
			this.Load += new System.EventHandler(this.FormWebChatTools_Load);
			this.groupFilters.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridWebChatSessions;
		private System.Windows.Forms.CheckBox checkShowEndedSessions;
		private UI.ODDateRangePicker dateRangeWebChat;
		private System.Windows.Forms.Label labelUsers;
		private UI.ComboBoxOD comboUsers;
		private System.Windows.Forms.Label labelChatTextContains;
		private ODtextBox textChatTextContains;
		private ODtextBox textSessionNum;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupFilters;
		private System.Windows.Forms.Label labelCount;
		private System.Windows.Forms.Label labelCountValue;
	}
}