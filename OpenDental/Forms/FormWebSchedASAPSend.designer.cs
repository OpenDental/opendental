namespace OpenDental{
	partial class FormWebSchedASAPSend {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedASAPSend));
			this.butSend = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupSendMode = new System.Windows.Forms.GroupBox();
			this.radioEmail = new System.Windows.Forms.RadioButton();
			this.radioTextEmail = new System.Windows.Forms.RadioButton();
			this.radioPreferred = new System.Windows.Forms.RadioButton();
			this.radioText = new System.Windows.Forms.RadioButton();
			this.textTextTemplate = new OpenDental.ODtextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textEmailSubject = new OpenDental.ODtextBox();
			this.labelAnticipated = new System.Windows.Forms.Label();
			this.timerUpdateDetails = new System.Windows.Forms.Timer(this.components);
			this.gridSendDetails = new OpenDental.UI.GridOD();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmailText = new System.Windows.Forms.WebBrowser();
			this.groupSendMode.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(506, 580);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 3;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(587, 580);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupSendMode
			// 
			this.groupSendMode.Controls.Add(this.radioEmail);
			this.groupSendMode.Controls.Add(this.radioTextEmail);
			this.groupSendMode.Controls.Add(this.radioPreferred);
			this.groupSendMode.Controls.Add(this.radioText);
			this.groupSendMode.Location = new System.Drawing.Point(24, 13);
			this.groupSendMode.Name = "groupSendMode";
			this.groupSendMode.Size = new System.Drawing.Size(438, 98);
			this.groupSendMode.TabIndex = 74;
			this.groupSendMode.TabStop = false;
			this.groupSendMode.Text = "Send Mode";
			// 
			// radioEmail
			// 
			this.radioEmail.Location = new System.Drawing.Point(15, 52);
			this.radioEmail.Name = "radioEmail";
			this.radioEmail.Size = new System.Drawing.Size(422, 18);
			this.radioEmail.TabIndex = 1;
			this.radioEmail.Text = "Email";
			this.radioEmail.UseVisualStyleBackColor = true;
			this.radioEmail.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioTextEmail
			// 
			this.radioTextEmail.Location = new System.Drawing.Point(15, 16);
			this.radioTextEmail.Name = "radioTextEmail";
			this.radioTextEmail.Size = new System.Drawing.Size(422, 18);
			this.radioTextEmail.TabIndex = 77;
			this.radioTextEmail.Text = "Text Message and Email";
			this.radioTextEmail.UseVisualStyleBackColor = true;
			this.radioTextEmail.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioPreferred
			// 
			this.radioPreferred.Location = new System.Drawing.Point(15, 71);
			this.radioPreferred.Name = "radioPreferred";
			this.radioPreferred.Size = new System.Drawing.Size(422, 18);
			this.radioPreferred.TabIndex = 74;
			this.radioPreferred.Text = "Preferred contact method";
			this.radioPreferred.UseVisualStyleBackColor = true;
			this.radioPreferred.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioText
			// 
			this.radioText.Location = new System.Drawing.Point(15, 34);
			this.radioText.Name = "radioText";
			this.radioText.Size = new System.Drawing.Size(422, 18);
			this.radioText.TabIndex = 0;
			this.radioText.Text = "Text Message";
			this.radioText.UseVisualStyleBackColor = true;
			this.radioText.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// textTextTemplate
			// 
			this.textTextTemplate.AcceptsTab = true;
			this.textTextTemplate.BackColor = System.Drawing.SystemColors.Window;
			this.textTextTemplate.DetectLinksEnabled = false;
			this.textTextTemplate.DetectUrls = false;
			this.textTextTemplate.Location = new System.Drawing.Point(24, 143);
			this.textTextTemplate.Name = "textTextTemplate";
			this.textTextTemplate.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textTextTemplate.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTextTemplate.Size = new System.Drawing.Size(438, 67);
			this.textTextTemplate.TabIndex = 75;
			this.textTextTemplate.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 222);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 14);
			this.label2.TabIndex = 77;
			this.label2.Text = "Email Message Text";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 122);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(190, 18);
			this.label1.TabIndex = 78;
			this.label1.Text = "Text Message Text";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 341);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 14);
			this.label3.TabIndex = 80;
			this.label3.Text = "Email Subject";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textEmailSubject
			// 
			this.textEmailSubject.AcceptsTab = true;
			this.textEmailSubject.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailSubject.DetectLinksEnabled = false;
			this.textEmailSubject.DetectUrls = false;
			this.textEmailSubject.Location = new System.Drawing.Point(24, 358);
			this.textEmailSubject.Name = "textEmailSubject";
			this.textEmailSubject.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailSubject.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailSubject.Size = new System.Drawing.Size(438, 20);
			this.textEmailSubject.TabIndex = 79;
			this.textEmailSubject.Text = "";
			// 
			// labelAnticipated
			// 
			this.labelAnticipated.Location = new System.Drawing.Point(486, 68);
			this.labelAnticipated.Name = "labelAnticipated";
			this.labelAnticipated.Size = new System.Drawing.Size(171, 178);
			this.labelAnticipated.TabIndex = 81;
			this.labelAnticipated.Text = "101 texts will be sent out over the next 42 minutes.\r\n99 emails will be sent once" +
    " Send is clicked.";
			// 
			// timerUpdateDetails
			// 
			this.timerUpdateDetails.Interval = 60000;
			this.timerUpdateDetails.Tick += new System.EventHandler(this.timerUpdateDetails_Tick);
			// 
			// gridSendDetails
			// 
			this.gridSendDetails.AllowSortingByColumn = true;
			this.gridSendDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridSendDetails.HScrollVisible = true;
			this.gridSendDetails.Location = new System.Drawing.Point(24, 383);
			this.gridSendDetails.Name = "gridSendDetails";
			this.gridSendDetails.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridSendDetails.Size = new System.Drawing.Size(638, 191);
			this.gridSendDetails.TabIndex = 82;
			this.gridSendDetails.Title = "Send Details";
			this.gridSendDetails.TranslationName = "FormWebSchedASAPSend";
			// 
			// butEditEmail
			// 
			this.butEditEmail.Location = new System.Drawing.Point(391, 218);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(70, 20);
			this.butEditEmail.TabIndex = 322;
			this.butEditEmail.Text = "Edit";
			this.butEditEmail.UseVisualStyleBackColor = true;
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// browserEmailText
			// 
			this.browserEmailText.AllowWebBrowserDrop = false;
			this.browserEmailText.Location = new System.Drawing.Point(24, 239);
			this.browserEmailText.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailText.Name = "browserEmailText";
			this.browserEmailText.Size = new System.Drawing.Size(437, 99);
			this.browserEmailText.TabIndex = 321;
			this.browserEmailText.WebBrowserShortcutsEnabled = false;
			// 
			// FormWebSchedASAPSend
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(674, 616);
			this.Controls.Add(this.butEditEmail);
			this.Controls.Add(this.browserEmailText);
			this.Controls.Add(this.gridSendDetails);
			this.Controls.Add(this.labelAnticipated);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textEmailSubject);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTextTemplate);
			this.Controls.Add(this.groupSendMode);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedASAPSend";
			this.Text = "Send Web Sched ASAP Alerts";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWebSchedASAPSend_FormClosing);
			this.Load += new System.EventHandler(this.FormWebSchedASAPSend_Load);
			this.groupSendMode.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSend;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupSendMode;
		private System.Windows.Forms.RadioButton radioEmail;
		private System.Windows.Forms.RadioButton radioTextEmail;
		private System.Windows.Forms.RadioButton radioPreferred;
		private System.Windows.Forms.RadioButton radioText;
		private ODtextBox textTextTemplate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private ODtextBox textEmailSubject;
		private System.Windows.Forms.Label labelAnticipated;
		private System.Windows.Forms.Timer timerUpdateDetails;
		private UI.GridOD gridSendDetails;
		private UI.Button butEditEmail;
		private System.Windows.Forms.WebBrowser browserEmailText;
	}
}