namespace OpenDental{
	partial class FormWebChatSurveys {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebChatSurveys));
			this.butClose = new OpenDental.UI.Button();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.textSessionNum = new OpenDental.ODtextBox();
			this.labelSessionNum = new System.Windows.Forms.Label();
			this.dateRangeWebChat = new OpenDental.UI.ODDateRangePicker();
			this.comboUsers = new OpenDental.UI.ComboBoxOD();
			this.textSurveyTextContains = new OpenDental.ODtextBox();
			this.labelSurveyTextContains = new System.Windows.Forms.Label();
			this.labelUsers = new System.Windows.Forms.Label();
			this.gridWebChatSurveys = new OpenDental.UI.GridOD();
			this.butPrint = new OpenDental.UI.Button();
			this.labelTotals = new System.Windows.Forms.Label();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(922, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilters.Controls.Add(this.textSessionNum);
			this.groupFilters.Controls.Add(this.labelSessionNum);
			this.groupFilters.Controls.Add(this.dateRangeWebChat);
			this.groupFilters.Controls.Add(this.comboUsers);
			this.groupFilters.Controls.Add(this.textSurveyTextContains);
			this.groupFilters.Controls.Add(this.labelSurveyTextContains);
			this.groupFilters.Controls.Add(this.labelUsers);
			this.groupFilters.Location = new System.Drawing.Point(12, 12);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(985, 78);
			this.groupFilters.TabIndex = 17;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Filters";
			// 
			// textSessionNum
			// 
			this.textSessionNum.AcceptsTab = true;
			this.textSessionNum.BackColor = System.Drawing.SystemColors.Window;
			this.textSessionNum.DetectLinksEnabled = false;
			this.textSessionNum.DetectUrls = false;
			this.textSessionNum.Location = new System.Drawing.Point(98, 18);
			this.textSessionNum.Multiline = false;
			this.textSessionNum.Name = "textSessionNum";
			this.textSessionNum.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSessionNum.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSessionNum.Size = new System.Drawing.Size(103, 20);
			this.textSessionNum.TabIndex = 14;
			this.textSessionNum.Text = "";
			// 
			// labelSessionNum
			// 
			this.labelSessionNum.Location = new System.Drawing.Point(5, 18);
			this.labelSessionNum.Name = "labelSessionNum";
			this.labelSessionNum.Size = new System.Drawing.Size(93, 20);
			this.labelSessionNum.TabIndex = 13;
			this.labelSessionNum.Text = "Session Num";
			this.labelSessionNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateRangeWebChat
			// 
			this.dateRangeWebChat.BackColor = System.Drawing.Color.Transparent;
			this.dateRangeWebChat.Location = new System.Drawing.Point(33, 44);
			this.dateRangeWebChat.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangeWebChat.Name = "dateRangeWebChat";
			this.dateRangeWebChat.Size = new System.Drawing.Size(453, 24);
			this.dateRangeWebChat.TabIndex = 6;
			// 
			// comboUsers
			// 
			this.comboUsers.BackColor = System.Drawing.SystemColors.Window;
			this.comboUsers.Location = new System.Drawing.Point(581, 44);
			this.comboUsers.Name = "comboUsers";
			this.comboUsers.IncludeAll = true;
			this.comboUsers.SelectionModeMulti = true;
			this.comboUsers.Size = new System.Drawing.Size(159, 21);
			this.comboUsers.TabIndex = 9;
			// 
			// textSurveyTextContains
			// 
			this.textSurveyTextContains.AcceptsTab = true;
			this.textSurveyTextContains.BackColor = System.Drawing.SystemColors.Window;
			this.textSurveyTextContains.DetectLinksEnabled = false;
			this.textSurveyTextContains.DetectUrls = false;
			this.textSurveyTextContains.Location = new System.Drawing.Point(581, 18);
			this.textSurveyTextContains.Multiline = false;
			this.textSurveyTextContains.Name = "textSurveyTextContains";
			this.textSurveyTextContains.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSurveyTextContains.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSurveyTextContains.Size = new System.Drawing.Size(159, 20);
			this.textSurveyTextContains.TabIndex = 12;
			this.textSurveyTextContains.Text = "";
			// 
			// labelSurveyTextContains
			// 
			this.labelSurveyTextContains.Location = new System.Drawing.Point(442, 16);
			this.labelSurveyTextContains.Name = "labelSurveyTextContains";
			this.labelSurveyTextContains.Size = new System.Drawing.Size(138, 20);
			this.labelSurveyTextContains.TabIndex = 11;
			this.labelSurveyTextContains.Text = "Survey Text Contains";
			this.labelSurveyTextContains.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelUsers
			// 
			this.labelUsers.Location = new System.Drawing.Point(492, 43);
			this.labelUsers.Name = "labelUsers";
			this.labelUsers.Size = new System.Drawing.Size(83, 20);
			this.labelUsers.TabIndex = 7;
			this.labelUsers.Text = "Users";
			this.labelUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridWebChatSurveys
			// 
			this.gridWebChatSurveys.AllowSortingByColumn = true;
			this.gridWebChatSurveys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebChatSurveys.Location = new System.Drawing.Point(12, 116);
			this.gridWebChatSurveys.Name = "gridWebChatSurveys";
			this.gridWebChatSurveys.Size = new System.Drawing.Size(985, 538);
			this.gridWebChatSurveys.TabIndex = 16;
			this.gridWebChatSurveys.Title = "Web Chat Surveys";
			this.gridWebChatSurveys.TranslationName = "gridWebChatSessions";
			this.gridWebChatSurveys.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridWebChatSurveys_CellDoubleClick);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(467, 660);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 18;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// labelTotals
			// 
			this.labelTotals.Location = new System.Drawing.Point(12, 93);
			this.labelTotals.Name = "labelTotals";
			this.labelTotals.Size = new System.Drawing.Size(985, 20);
			this.labelTotals.TabIndex = 19;
			this.labelTotals.Text = "Totals: Positive (0) Neutral (0) Negative (0)";
			this.labelTotals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormWebChatSurveys
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1009, 696);
			this.Controls.Add(this.labelTotals);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.gridWebChatSurveys);
			this.Controls.Add(this.butClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormWebChatSurveys";
			this.Text = "Web Chat Surveys";
			this.Load += new System.EventHandler(this.FormWebChatSurveys_Load);
			this.groupFilters.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.GroupBox groupFilters;
		private ODtextBox textSessionNum;
		private System.Windows.Forms.Label labelSessionNum;
		private UI.ODDateRangePicker dateRangeWebChat;
		private UI.ComboBoxOD comboUsers;
		private ODtextBox textSurveyTextContains;
		private System.Windows.Forms.Label labelSurveyTextContains;
		private System.Windows.Forms.Label labelUsers;
		private UI.GridOD gridWebChatSurveys;
		private UI.Button butPrint;
		private System.Windows.Forms.Label labelTotals;
	}
}