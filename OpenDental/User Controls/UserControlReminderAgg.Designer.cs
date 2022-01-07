namespace OpenDental {
	partial class UserControlReminderAgg {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tabTemplates = new System.Windows.Forms.TabControl();
			this.tabEmailTemplate = new System.Windows.Forms.TabPage();
			this.groupBoxEmailSubjAggShared = new System.Windows.Forms.GroupBox();
			this.labelEmailSubjAggShared = new System.Windows.Forms.Label();
			this.textEmailSubjAggShared = new OpenDental.ODtextBox();
			this.groupBoxEmailAggShared = new System.Windows.Forms.GroupBox();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmailBody = new System.Windows.Forms.WebBrowser();
			this.labelEmailAggShared = new System.Windows.Forms.Label();
			this.groupBoxEmailAggPerAppt = new System.Windows.Forms.GroupBox();
			this.labelEmailAggPerAppt = new System.Windows.Forms.Label();
			this.textEmailAggPerAppt = new OpenDental.ODtextBox();
			this.tabSMSTemplate = new System.Windows.Forms.TabPage();
			this.groupBoxSMSAggPerAppt = new System.Windows.Forms.GroupBox();
			this.labelSMSAggPerAppt = new System.Windows.Forms.Label();
			this.textSMSAggPerAppt = new OpenDental.ODtextBox();
			this.groupBoxSMSAggShared = new System.Windows.Forms.GroupBox();
			this.labelSMSAggShared = new System.Windows.Forms.Label();
			this.textSMSAggShared = new OpenDental.ODtextBox();
			this.tabAutoReplyTemplate = new System.Windows.Forms.TabPage();
			this.groupAggregateAutoReplyTemplate = new System.Windows.Forms.GroupBox();
			this.labelAggregateAutoReply = new System.Windows.Forms.Label();
			this.textAggregateAutoReply = new OpenDental.ODtextBox();
			this.groupAutoReplySingle = new System.Windows.Forms.GroupBox();
			this.labelSingleAutoReply = new System.Windows.Forms.Label();
			this.textSingleAutoReply = new OpenDental.ODtextBox();
			this.tabArrivalTemplate = new System.Windows.Forms.TabPage();
			this.groupArrivedReply = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textArrivalResponse = new OpenDental.ODtextBox();
			this.groupComeIn = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textComeIn = new OpenDental.ODtextBox();
			this.groupBoxTags = new System.Windows.Forms.GroupBox();
			this.labelTags = new System.Windows.Forms.Label();
			this.tabTemplates.SuspendLayout();
			this.tabEmailTemplate.SuspendLayout();
			this.groupBoxEmailSubjAggShared.SuspendLayout();
			this.groupBoxEmailAggShared.SuspendLayout();
			this.groupBoxEmailAggPerAppt.SuspendLayout();
			this.tabSMSTemplate.SuspendLayout();
			this.groupBoxSMSAggPerAppt.SuspendLayout();
			this.groupBoxSMSAggShared.SuspendLayout();
			this.tabAutoReplyTemplate.SuspendLayout();
			this.groupAggregateAutoReplyTemplate.SuspendLayout();
			this.groupAutoReplySingle.SuspendLayout();
			this.tabArrivalTemplate.SuspendLayout();
			this.groupArrivedReply.SuspendLayout();
			this.groupComeIn.SuspendLayout();
			this.groupBoxTags.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabTemplates
			// 
			this.tabTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabTemplates.Controls.Add(this.tabEmailTemplate);
			this.tabTemplates.Controls.Add(this.tabSMSTemplate);
			this.tabTemplates.Controls.Add(this.tabAutoReplyTemplate);
			this.tabTemplates.Controls.Add(this.tabArrivalTemplate);
			this.tabTemplates.Location = new System.Drawing.Point(1, 76);
			this.tabTemplates.MinimumSize = new System.Drawing.Size(639, 460);
			this.tabTemplates.Name = "tabTemplates";
			this.tabTemplates.SelectedIndex = 0;
			this.tabTemplates.Size = new System.Drawing.Size(639, 460);
			this.tabTemplates.TabIndex = 11;
			this.tabTemplates.SelectedIndexChanged += new System.EventHandler(this.tabTemplates_SelectedIndexChanged);
			// 
			// tabEmailTemplate
			// 
			this.tabEmailTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.tabEmailTemplate.Controls.Add(this.groupBoxEmailSubjAggShared);
			this.tabEmailTemplate.Controls.Add(this.groupBoxEmailAggShared);
			this.tabEmailTemplate.Controls.Add(this.groupBoxEmailAggPerAppt);
			this.tabEmailTemplate.Location = new System.Drawing.Point(4, 22);
			this.tabEmailTemplate.Name = "tabEmailTemplate";
			this.tabEmailTemplate.Padding = new System.Windows.Forms.Padding(3);
			this.tabEmailTemplate.Size = new System.Drawing.Size(631, 434);
			this.tabEmailTemplate.TabIndex = 1;
			this.tabEmailTemplate.Text = "Email Templates";
			// 
			// groupBoxEmailSubjAggShared
			// 
			this.groupBoxEmailSubjAggShared.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxEmailSubjAggShared.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxEmailSubjAggShared.Controls.Add(this.labelEmailSubjAggShared);
			this.groupBoxEmailSubjAggShared.Controls.Add(this.textEmailSubjAggShared);
			this.groupBoxEmailSubjAggShared.Location = new System.Drawing.Point(4, 2);
			this.groupBoxEmailSubjAggShared.Name = "groupBoxEmailSubjAggShared";
			this.groupBoxEmailSubjAggShared.Size = new System.Drawing.Size(625, 66);
			this.groupBoxEmailSubjAggShared.TabIndex = 12;
			this.groupBoxEmailSubjAggShared.TabStop = false;
			this.groupBoxEmailSubjAggShared.Text = "Aggregated Email Subject";
			// 
			// labelEmailSubjAggShared
			// 
			this.labelEmailSubjAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelEmailSubjAggShared.Name = "labelEmailSubjAggShared";
			this.labelEmailSubjAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelEmailSubjAggShared.TabIndex = 13;
			this.labelEmailSubjAggShared.Text = "The subject heading template.";
			// 
			// textEmailSubjAggShared
			// 
			this.textEmailSubjAggShared.AcceptsTab = true;
			this.textEmailSubjAggShared.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailSubjAggShared.DetectLinksEnabled = false;
			this.textEmailSubjAggShared.DetectUrls = false;
			this.textEmailSubjAggShared.Location = new System.Drawing.Point(6, 39);
			this.textEmailSubjAggShared.Name = "textEmailSubjAggShared";
			this.textEmailSubjAggShared.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailSubjAggShared.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailSubjAggShared.Size = new System.Drawing.Size(615, 20);
			this.textEmailSubjAggShared.TabIndex = 3;
			this.textEmailSubjAggShared.Text = "";
			// 
			// groupBoxEmailAggShared
			// 
			this.groupBoxEmailAggShared.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxEmailAggShared.Controls.Add(this.butEditEmail);
			this.groupBoxEmailAggShared.Controls.Add(this.browserEmailBody);
			this.groupBoxEmailAggShared.Controls.Add(this.labelEmailAggShared);
			this.groupBoxEmailAggShared.Location = new System.Drawing.Point(5, 68);
			this.groupBoxEmailAggShared.Name = "groupBoxEmailAggShared";
			this.groupBoxEmailAggShared.Size = new System.Drawing.Size(625, 227);
			this.groupBoxEmailAggShared.TabIndex = 14;
			this.groupBoxEmailAggShared.TabStop = false;
			this.groupBoxEmailAggShared.Text = "Aggregated Email Template";
			// 
			// butEditEmail
			// 
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Location = new System.Drawing.Point(544, 196);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(75, 26);
			this.butEditEmail.TabIndex = 127;
			this.butEditEmail.Text = "&Edit";
			this.butEditEmail.UseVisualStyleBackColor = true;
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// browserEmailBody
			// 
			this.browserEmailBody.AllowWebBrowserDrop = false;
			this.browserEmailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browserEmailBody.Location = new System.Drawing.Point(10, 36);
			this.browserEmailBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailBody.Name = "browserEmailBody";
			this.browserEmailBody.Size = new System.Drawing.Size(609, 156);
			this.browserEmailBody.TabIndex = 115;
			this.browserEmailBody.WebBrowserShortcutsEnabled = false;
			// 
			// labelEmailAggShared
			// 
			this.labelEmailAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelEmailAggShared.Name = "labelEmailAggShared";
			this.labelEmailAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelEmailAggShared.TabIndex = 16;
			this.labelEmailAggShared.Text = "The message body template. Used once per aggregate message.";
			// 
			// groupBoxEmailAggPerAppt
			// 
			this.groupBoxEmailAggPerAppt.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxEmailAggPerAppt.Controls.Add(this.labelEmailAggPerAppt);
			this.groupBoxEmailAggPerAppt.Controls.Add(this.textEmailAggPerAppt);
			this.groupBoxEmailAggPerAppt.Location = new System.Drawing.Point(4, 301);
			this.groupBoxEmailAggPerAppt.Name = "groupBoxEmailAggPerAppt";
			this.groupBoxEmailAggPerAppt.Size = new System.Drawing.Size(625, 134);
			this.groupBoxEmailAggPerAppt.TabIndex = 16;
			this.groupBoxEmailAggPerAppt.TabStop = false;
			this.groupBoxEmailAggPerAppt.Text = "Aggregated Email Template Per Appointment";
			// 
			// labelEmailAggPerAppt
			// 
			this.labelEmailAggPerAppt.Location = new System.Drawing.Point(7, 18);
			this.labelEmailAggPerAppt.Name = "labelEmailAggPerAppt";
			this.labelEmailAggPerAppt.Size = new System.Drawing.Size(471, 18);
			this.labelEmailAggPerAppt.TabIndex = 17;
			this.labelEmailAggPerAppt.Text = "A single appointment template. Formats each appointment listed in the aggregate m" +
    "essage.";
			this.labelEmailAggPerAppt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textEmailAggPerAppt
			// 
			this.textEmailAggPerAppt.AcceptsTab = true;
			this.textEmailAggPerAppt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEmailAggPerAppt.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailAggPerAppt.DetectLinksEnabled = false;
			this.textEmailAggPerAppt.DetectUrls = false;
			this.textEmailAggPerAppt.Location = new System.Drawing.Point(6, 39);
			this.textEmailAggPerAppt.Name = "textEmailAggPerAppt";
			this.textEmailAggPerAppt.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailAggPerAppt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailAggPerAppt.Size = new System.Drawing.Size(615, 89);
			this.textEmailAggPerAppt.TabIndex = 5;
			this.textEmailAggPerAppt.Text = "";
			// 
			// tabSMSTemplate
			// 
			this.tabSMSTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.tabSMSTemplate.Controls.Add(this.groupBoxSMSAggPerAppt);
			this.tabSMSTemplate.Controls.Add(this.groupBoxSMSAggShared);
			this.tabSMSTemplate.Location = new System.Drawing.Point(4, 22);
			this.tabSMSTemplate.Name = "tabSMSTemplate";
			this.tabSMSTemplate.Padding = new System.Windows.Forms.Padding(3);
			this.tabSMSTemplate.Size = new System.Drawing.Size(631, 434);
			this.tabSMSTemplate.TabIndex = 0;
			this.tabSMSTemplate.Text = "SMS Templates";
			// 
			// groupBoxSMSAggPerAppt
			// 
			this.groupBoxSMSAggPerAppt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSMSAggPerAppt.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxSMSAggPerAppt.Controls.Add(this.labelSMSAggPerAppt);
			this.groupBoxSMSAggPerAppt.Controls.Add(this.textSMSAggPerAppt);
			this.groupBoxSMSAggPerAppt.Location = new System.Drawing.Point(2, 4);
			this.groupBoxSMSAggPerAppt.Name = "groupBoxSMSAggPerAppt";
			this.groupBoxSMSAggPerAppt.Size = new System.Drawing.Size(625, 155);
			this.groupBoxSMSAggPerAppt.TabIndex = 10;
			this.groupBoxSMSAggPerAppt.TabStop = false;
			this.groupBoxSMSAggPerAppt.Text = "Aggregated SMS Template Per Appointment";
			// 
			// labelSMSAggPerAppt
			// 
			this.labelSMSAggPerAppt.Location = new System.Drawing.Point(7, 20);
			this.labelSMSAggPerAppt.Name = "labelSMSAggPerAppt";
			this.labelSMSAggPerAppt.Size = new System.Drawing.Size(471, 13);
			this.labelSMSAggPerAppt.TabIndex = 11;
			this.labelSMSAggPerAppt.Text = "A single appointment template. Formats each appointment listed in the aggregate m" +
    "essage.";
			// 
			// textSMSAggPerAppt
			// 
			this.textSMSAggPerAppt.AcceptsTab = true;
			this.textSMSAggPerAppt.BackColor = System.Drawing.SystemColors.Window;
			this.textSMSAggPerAppt.DetectLinksEnabled = false;
			this.textSMSAggPerAppt.DetectUrls = false;
			this.textSMSAggPerAppt.Location = new System.Drawing.Point(6, 39);
			this.textSMSAggPerAppt.Name = "textSMSAggPerAppt";
			this.textSMSAggPerAppt.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textSMSAggPerAppt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSMSAggPerAppt.Size = new System.Drawing.Size(615, 110);
			this.textSMSAggPerAppt.TabIndex = 2;
			this.textSMSAggPerAppt.Text = "";
			// 
			// groupBoxSMSAggShared
			// 
			this.groupBoxSMSAggShared.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSMSAggShared.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxSMSAggShared.Controls.Add(this.labelSMSAggShared);
			this.groupBoxSMSAggShared.Controls.Add(this.textSMSAggShared);
			this.groupBoxSMSAggShared.Location = new System.Drawing.Point(3, 165);
			this.groupBoxSMSAggShared.Name = "groupBoxSMSAggShared";
			this.groupBoxSMSAggShared.Size = new System.Drawing.Size(624, 211);
			this.groupBoxSMSAggShared.TabIndex = 8;
			this.groupBoxSMSAggShared.TabStop = false;
			this.groupBoxSMSAggShared.Text = "Aggregated SMS Template";
			// 
			// labelSMSAggShared
			// 
			this.labelSMSAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelSMSAggShared.Name = "labelSMSAggShared";
			this.labelSMSAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelSMSAggShared.TabIndex = 9;
			this.labelSMSAggShared.Text = "The message body template. Used once per aggregate message.";
			// 
			// textSMSAggShared
			// 
			this.textSMSAggShared.AcceptsTab = true;
			this.textSMSAggShared.BackColor = System.Drawing.SystemColors.Window;
			this.textSMSAggShared.DetectLinksEnabled = false;
			this.textSMSAggShared.DetectUrls = false;
			this.textSMSAggShared.Location = new System.Drawing.Point(6, 39);
			this.textSMSAggShared.Name = "textSMSAggShared";
			this.textSMSAggShared.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textSMSAggShared.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSMSAggShared.Size = new System.Drawing.Size(615, 166);
			this.textSMSAggShared.TabIndex = 1;
			this.textSMSAggShared.Text = "";
			// 
			// tabAutoReplyTemplate
			// 
			this.tabAutoReplyTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.tabAutoReplyTemplate.Controls.Add(this.groupAggregateAutoReplyTemplate);
			this.tabAutoReplyTemplate.Controls.Add(this.groupAutoReplySingle);
			this.tabAutoReplyTemplate.Location = new System.Drawing.Point(4, 22);
			this.tabAutoReplyTemplate.Name = "tabAutoReplyTemplate";
			this.tabAutoReplyTemplate.Size = new System.Drawing.Size(631, 434);
			this.tabAutoReplyTemplate.TabIndex = 2;
			this.tabAutoReplyTemplate.Text = "Auto Reply Templates";
			// 
			// groupAggregateAutoReplyTemplate
			// 
			this.groupAggregateAutoReplyTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAggregateAutoReplyTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.groupAggregateAutoReplyTemplate.Controls.Add(this.labelAggregateAutoReply);
			this.groupAggregateAutoReplyTemplate.Controls.Add(this.textAggregateAutoReply);
			this.groupAggregateAutoReplyTemplate.Location = new System.Drawing.Point(2, 141);
			this.groupAggregateAutoReplyTemplate.Name = "groupAggregateAutoReplyTemplate";
			this.groupAggregateAutoReplyTemplate.Size = new System.Drawing.Size(628, 155);
			this.groupAggregateAutoReplyTemplate.TabIndex = 12;
			this.groupAggregateAutoReplyTemplate.TabStop = false;
			this.groupAggregateAutoReplyTemplate.Text = "Aggregate Auto Reply Template";
			// 
			// labelAggregateAutoReply
			// 
			this.labelAggregateAutoReply.Location = new System.Drawing.Point(7, 20);
			this.labelAggregateAutoReply.Name = "labelAggregateAutoReply";
			this.labelAggregateAutoReply.Size = new System.Drawing.Size(471, 13);
			this.labelAggregateAutoReply.TabIndex = 11;
			this.labelAggregateAutoReply.Text = "Aggregate appointment confirmation auto reply template.";
			// 
			// textAggregateAutoReply
			// 
			this.textAggregateAutoReply.AcceptsTab = true;
			this.textAggregateAutoReply.BackColor = System.Drawing.SystemColors.Window;
			this.textAggregateAutoReply.DetectLinksEnabled = false;
			this.textAggregateAutoReply.DetectUrls = false;
			this.textAggregateAutoReply.Location = new System.Drawing.Point(6, 39);
			this.textAggregateAutoReply.Name = "textAggregateAutoReply";
			this.textAggregateAutoReply.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textAggregateAutoReply.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAggregateAutoReply.Size = new System.Drawing.Size(615, 110);
			this.textAggregateAutoReply.TabIndex = 2;
			this.textAggregateAutoReply.Text = "";
			// 
			// groupAutoReplySingle
			// 
			this.groupAutoReplySingle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAutoReplySingle.BackColor = System.Drawing.SystemColors.Control;
			this.groupAutoReplySingle.Controls.Add(this.labelSingleAutoReply);
			this.groupAutoReplySingle.Controls.Add(this.textSingleAutoReply);
			this.groupAutoReplySingle.Location = new System.Drawing.Point(2, 3);
			this.groupAutoReplySingle.Name = "groupAutoReplySingle";
			this.groupAutoReplySingle.Size = new System.Drawing.Size(628, 155);
			this.groupAutoReplySingle.TabIndex = 11;
			this.groupAutoReplySingle.TabStop = false;
			this.groupAutoReplySingle.Text = "Single Auto Reply Template";
			// 
			// labelSingleAutoReply
			// 
			this.labelSingleAutoReply.Location = new System.Drawing.Point(7, 20);
			this.labelSingleAutoReply.Name = "labelSingleAutoReply";
			this.labelSingleAutoReply.Size = new System.Drawing.Size(471, 13);
			this.labelSingleAutoReply.TabIndex = 11;
			this.labelSingleAutoReply.Text = "A single appointment confirmation auto reply template.";
			// 
			// textSingleAutoReply
			// 
			this.textSingleAutoReply.AcceptsTab = true;
			this.textSingleAutoReply.BackColor = System.Drawing.SystemColors.Window;
			this.textSingleAutoReply.DetectLinksEnabled = false;
			this.textSingleAutoReply.DetectUrls = false;
			this.textSingleAutoReply.Location = new System.Drawing.Point(6, 39);
			this.textSingleAutoReply.Name = "textSingleAutoReply";
			this.textSingleAutoReply.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textSingleAutoReply.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSingleAutoReply.Size = new System.Drawing.Size(615, 110);
			this.textSingleAutoReply.TabIndex = 2;
			this.textSingleAutoReply.Text = "";
			// 
			// tabArrivalTemplate
			// 
			this.tabArrivalTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.tabArrivalTemplate.Controls.Add(this.groupArrivedReply);
			this.tabArrivalTemplate.Controls.Add(this.groupComeIn);
			this.tabArrivalTemplate.Location = new System.Drawing.Point(4, 22);
			this.tabArrivalTemplate.Name = "tabArrivalTemplate";
			this.tabArrivalTemplate.Size = new System.Drawing.Size(631, 434);
			this.tabArrivalTemplate.TabIndex = 3;
			this.tabArrivalTemplate.Text = "Arrival Templates";
			// 
			// groupArrivedReply
			// 
			this.groupArrivedReply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupArrivedReply.BackColor = System.Drawing.SystemColors.Control;
			this.groupArrivedReply.Controls.Add(this.label2);
			this.groupArrivedReply.Controls.Add(this.textArrivalResponse);
			this.groupArrivedReply.Location = new System.Drawing.Point(2, 3);
			this.groupArrivedReply.Name = "groupArrivedReply";
			this.groupArrivedReply.Size = new System.Drawing.Size(628, 155);
			this.groupArrivedReply.TabIndex = 14;
			this.groupArrivedReply.TabStop = false;
			this.groupArrivedReply.Text = "Arrival SMS Response Template";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(614, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Automatic response when arrival message is received from patient.  Leave blank to" +
    " disable.";
			// 
			// textArrivalResponse
			// 
			this.textArrivalResponse.AcceptsTab = true;
			this.textArrivalResponse.BackColor = System.Drawing.SystemColors.Window;
			this.textArrivalResponse.DetectLinksEnabled = false;
			this.textArrivalResponse.DetectUrls = false;
			this.textArrivalResponse.Location = new System.Drawing.Point(6, 39);
			this.textArrivalResponse.Name = "textArrivalResponse";
			this.textArrivalResponse.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textArrivalResponse.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textArrivalResponse.Size = new System.Drawing.Size(615, 110);
			this.textArrivalResponse.TabIndex = 2;
			this.textArrivalResponse.Text = "";
			// 
			// groupComeIn
			// 
			this.groupComeIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupComeIn.BackColor = System.Drawing.SystemColors.Control;
			this.groupComeIn.Controls.Add(this.label1);
			this.groupComeIn.Controls.Add(this.textComeIn);
			this.groupComeIn.Location = new System.Drawing.Point(2, 162);
			this.groupComeIn.Name = "groupComeIn";
			this.groupComeIn.Size = new System.Drawing.Size(628, 155);
			this.groupComeIn.TabIndex = 13;
			this.groupComeIn.TabStop = false;
			this.groupComeIn.Text = "Come In SMS Message Template";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(614, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "Message sent when office is ready for the patient.  Accessed by right-clicking on" +
    " appointment.";
			// 
			// textComeIn
			// 
			this.textComeIn.AcceptsTab = true;
			this.textComeIn.BackColor = System.Drawing.SystemColors.Window;
			this.textComeIn.DetectLinksEnabled = false;
			this.textComeIn.DetectUrls = false;
			this.textComeIn.Location = new System.Drawing.Point(6, 39);
			this.textComeIn.Name = "textComeIn";
			this.textComeIn.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textComeIn.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textComeIn.Size = new System.Drawing.Size(615, 110);
			this.textComeIn.TabIndex = 2;
			this.textComeIn.Text = "";
			// 
			// groupBoxTags
			// 
			this.groupBoxTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxTags.Controls.Add(this.labelTags);
			this.groupBoxTags.Location = new System.Drawing.Point(1, 2);
			this.groupBoxTags.Name = "groupBoxTags";
			this.groupBoxTags.Size = new System.Drawing.Size(639, 73);
			this.groupBoxTags.TabIndex = 21;
			this.groupBoxTags.TabStop = false;
			this.groupBoxTags.Text = "Template Replacement Tags";
			// 
			// labelTags
			// 
			this.labelTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTags.Location = new System.Drawing.Point(3, 16);
			this.labelTags.Name = "labelTags";
			this.labelTags.Size = new System.Drawing.Size(633, 54);
			this.labelTags.TabIndex = 19;
			this.labelTags.Text = "Use template tags to create dynamic messages.";
			// 
			// UserControlReminderAgg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxTags);
			this.Controls.Add(this.tabTemplates);
			this.MinimumSize = new System.Drawing.Size(641, 537);
			this.Name = "UserControlReminderAgg";
			this.Size = new System.Drawing.Size(641, 537);
			this.tabTemplates.ResumeLayout(false);
			this.tabEmailTemplate.ResumeLayout(false);
			this.groupBoxEmailSubjAggShared.ResumeLayout(false);
			this.groupBoxEmailAggShared.ResumeLayout(false);
			this.groupBoxEmailAggPerAppt.ResumeLayout(false);
			this.tabSMSTemplate.ResumeLayout(false);
			this.groupBoxSMSAggPerAppt.ResumeLayout(false);
			this.groupBoxSMSAggShared.ResumeLayout(false);
			this.tabAutoReplyTemplate.ResumeLayout(false);
			this.groupAggregateAutoReplyTemplate.ResumeLayout(false);
			this.groupAutoReplySingle.ResumeLayout(false);
			this.tabArrivalTemplate.ResumeLayout(false);
			this.groupArrivedReply.ResumeLayout(false);
			this.groupComeIn.ResumeLayout(false);
			this.groupBoxTags.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabTemplates;
		private System.Windows.Forms.TabPage tabEmailTemplate;
		private System.Windows.Forms.GroupBox groupBoxEmailSubjAggShared;
		private System.Windows.Forms.Label labelEmailSubjAggShared;
		private ODtextBox textEmailSubjAggShared;
		private System.Windows.Forms.GroupBox groupBoxEmailAggShared;
		private UI.Button butEditEmail;
		private System.Windows.Forms.WebBrowser browserEmailBody;
		private System.Windows.Forms.Label labelEmailAggShared;
		private System.Windows.Forms.GroupBox groupBoxEmailAggPerAppt;
		private System.Windows.Forms.Label labelEmailAggPerAppt;
		private ODtextBox textEmailAggPerAppt;
		private System.Windows.Forms.TabPage tabSMSTemplate;
		private System.Windows.Forms.GroupBox groupBoxSMSAggPerAppt;
		private System.Windows.Forms.Label labelSMSAggPerAppt;
		private ODtextBox textSMSAggPerAppt;
		private System.Windows.Forms.GroupBox groupBoxSMSAggShared;
		private System.Windows.Forms.Label labelSMSAggShared;
		private ODtextBox textSMSAggShared;
		private System.Windows.Forms.TabPage tabAutoReplyTemplate;
		private System.Windows.Forms.GroupBox groupAggregateAutoReplyTemplate;
		private System.Windows.Forms.Label labelAggregateAutoReply;
		private ODtextBox textAggregateAutoReply;
		private System.Windows.Forms.GroupBox groupAutoReplySingle;
		private System.Windows.Forms.Label labelSingleAutoReply;
		private ODtextBox textSingleAutoReply;
		private System.Windows.Forms.GroupBox groupBoxTags;
		private System.Windows.Forms.Label labelTags;
		private System.Windows.Forms.TabPage tabArrivalTemplate;
		private System.Windows.Forms.GroupBox groupArrivedReply;
		private System.Windows.Forms.Label label2;
		private ODtextBox textArrivalResponse;
		private System.Windows.Forms.GroupBox groupComeIn;
		private System.Windows.Forms.Label label1;
		private ODtextBox textComeIn;
	}
}
