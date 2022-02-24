using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using System.Drawing;
using System.Text;

namespace OpenDental{
	public class FormEmailJobs : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butSend;
		public bool IsNew;
		private ODtextBox textSubject;
		private ODtextBox textFromAddress;
		private ODtextBox textBodyTextTemplate;
		private Label label3;
		private Label label2;
		private UI.GridOD gridRecipients;
		private ComboBox comboEmailFrom;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem setupToolStripMenuItem;
		private ToolStripMenuItem editTemplateToolStripMenuItem;
		private List<EmailAddress> _listEmailAddresses;
		public Job JobCur;
		private ODtextBox textDescriptions;
		private ODtextBox textPledgeTemplate;
		private Label label1;
		private Label label4;
		private ODtextBox textVersions;
		private Label label5;
		private List<Jobs.JobEmail> _listJobEmails;
		private List<FeatureRequest> _listFeatureReqs;
		private UI.Button butExport;
		private bool _isSent;

		///<summary></summary>
		public FormEmailJobs(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailJobs));
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboEmailFrom = new System.Windows.Forms.ComboBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.gridRecipients = new OpenDental.UI.GridOD();
			this.textVersions = new OpenDental.ODtextBox();
			this.textDescriptions = new OpenDental.ODtextBox();
			this.textPledgeTemplate = new OpenDental.ODtextBox();
			this.textSubject = new OpenDental.ODtextBox();
			this.textFromAddress = new OpenDental.ODtextBox();
			this.textBodyTextTemplate = new OpenDental.ODtextBox();
			this.butSend = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(12, 285);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 14);
			this.label3.TabIndex = 38;
			this.label3.Text = "From:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(12, 306);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 14);
			this.label2.TabIndex = 40;
			this.label2.Text = "Subject:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEmailFrom
			// 
			this.comboEmailFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.comboEmailFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFrom.Location = new System.Drawing.Point(645, 281);
			this.comboEmailFrom.MaxDropDownItems = 40;
			this.comboEmailFrom.Name = "comboEmailFrom";
			this.comboEmailFrom.Size = new System.Drawing.Size(359, 21);
			this.comboEmailFrom.TabIndex = 227;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1018, 24);
			this.menuStrip1.TabIndex = 229;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// setupToolStripMenuItem
			// 
			this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editTemplateToolStripMenuItem});
			this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
			this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.setupToolStripMenuItem.Text = "Setup";
			// 
			// editTemplateToolStripMenuItem
			// 
			this.editTemplateToolStripMenuItem.Name = "editTemplateToolStripMenuItem";
			this.editTemplateToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.editTemplateToolStripMenuItem.Text = "Edit Templates";
			this.editTemplateToolStripMenuItem.Click += new System.EventHandler(this.editTemplateToolStripMenuItem_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(11, 559);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 31);
			this.label1.TabIndex = 232;
			this.label1.Text = "Feature Description(s):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(12, 528);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 14);
			this.label4.TabIndex = 233;
			this.label4.Text = "Pledge:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(771, 533);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 14);
			this.label5.TabIndex = 234;
			this.label5.Text = "Version(s):";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridRecipients
			// 
			this.gridRecipients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRecipients.HasMultilineHeaders = true;
			this.gridRecipients.Location = new System.Drawing.Point(98, 27);
			this.gridRecipients.Name = "gridRecipients";
			this.gridRecipients.Size = new System.Drawing.Size(906, 250);
			this.gridRecipients.TabIndex = 226;
			this.gridRecipients.Title = "Recipients";
			this.gridRecipients.TranslationName = "FormTaskEdit";
			this.gridRecipients.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRecipients_CellClick);
			// 
			// textVersions
			// 
			this.textVersions.AcceptsTab = true;
			this.textVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textVersions.BackColor = System.Drawing.SystemColors.Window;
			this.textVersions.DetectLinksEnabled = false;
			this.textVersions.DetectUrls = false;
			this.textVersions.Location = new System.Drawing.Point(857, 529);
			this.textVersions.Multiline = false;
			this.textVersions.Name = "textVersions";
			this.textVersions.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textVersions.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.textVersions.Size = new System.Drawing.Size(147, 22);
			this.textVersions.SpellCheckIsEnabled = false;
			this.textVersions.TabIndex = 235;
			this.textVersions.Text = "";
			this.textVersions.WordWrap = false;
			// 
			// textDescriptions
			// 
			this.textDescriptions.AcceptsTab = true;
			this.textDescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescriptions.BackColor = System.Drawing.SystemColors.Window;
			this.textDescriptions.DetectLinksEnabled = false;
			this.textDescriptions.DetectUrls = false;
			this.textDescriptions.Location = new System.Drawing.Point(98, 557);
			this.textDescriptions.Name = "textDescriptions";
			this.textDescriptions.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textDescriptions.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescriptions.Size = new System.Drawing.Size(667, 127);
			this.textDescriptions.SpellCheckIsEnabled = false;
			this.textDescriptions.TabIndex = 230;
			this.textDescriptions.Text = "";
			// 
			// textPledgeTemplate
			// 
			this.textPledgeTemplate.AcceptsTab = true;
			this.textPledgeTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPledgeTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.textPledgeTemplate.DetectLinksEnabled = false;
			this.textPledgeTemplate.DetectUrls = false;
			this.textPledgeTemplate.Location = new System.Drawing.Point(98, 526);
			this.textPledgeTemplate.Name = "textPledgeTemplate";
			this.textPledgeTemplate.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textPledgeTemplate.ReadOnly = true;
			this.textPledgeTemplate.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPledgeTemplate.Size = new System.Drawing.Size(667, 25);
			this.textPledgeTemplate.SpellCheckIsEnabled = false;
			this.textPledgeTemplate.TabIndex = 231;
			this.textPledgeTemplate.Text = "";
			// 
			// textSubject
			// 
			this.textSubject.AcceptsTab = true;
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.BackColor = System.Drawing.SystemColors.Window;
			this.textSubject.DetectLinksEnabled = false;
			this.textSubject.DetectUrls = false;
			this.textSubject.Location = new System.Drawing.Point(98, 304);
			this.textSubject.Multiline = false;
			this.textSubject.Name = "textSubject";
			this.textSubject.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSubject.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.textSubject.Size = new System.Drawing.Size(906, 22);
			this.textSubject.SpellCheckIsEnabled = false;
			this.textSubject.TabIndex = 44;
			this.textSubject.Text = "";
			this.textSubject.WordWrap = false;
			// 
			// textFromAddress
			// 
			this.textFromAddress.AcceptsTab = true;
			this.textFromAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textFromAddress.BackColor = System.Drawing.SystemColors.Window;
			this.textFromAddress.DetectLinksEnabled = false;
			this.textFromAddress.DetectUrls = false;
			this.textFromAddress.Location = new System.Drawing.Point(98, 281);
			this.textFromAddress.Multiline = false;
			this.textFromAddress.Name = "textFromAddress";
			this.textFromAddress.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textFromAddress.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.textFromAddress.Size = new System.Drawing.Size(544, 22);
			this.textFromAddress.SpellCheckIsEnabled = false;
			this.textFromAddress.TabIndex = 41;
			this.textFromAddress.Text = "";
			this.textFromAddress.WordWrap = false;
			// 
			// textBodyTextTemplate
			// 
			this.textBodyTextTemplate.AcceptsTab = true;
			this.textBodyTextTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBodyTextTemplate.BackColor = System.Drawing.SystemColors.Control;
			this.textBodyTextTemplate.DetectLinksEnabled = false;
			this.textBodyTextTemplate.DetectUrls = false;
			this.textBodyTextTemplate.Location = new System.Drawing.Point(98, 327);
			this.textBodyTextTemplate.Name = "textBodyTextTemplate";
			this.textBodyTextTemplate.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textBodyTextTemplate.ReadOnly = true;
			this.textBodyTextTemplate.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBodyTextTemplate.Size = new System.Drawing.Size(906, 193);
			this.textBodyTextTemplate.SpellCheckIsEnabled = false;
			this.textBodyTextTemplate.TabIndex = 46;
			this.textBodyTextTemplate.Text = "";
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(848, 659);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 25);
			this.butSend.TabIndex = 9;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(929, 659);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(17, 253);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75, 24);
			this.butExport.TabIndex = 236;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// FormEmailJobs
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1018, 696);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.textVersions);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDescriptions);
			this.Controls.Add(this.textPledgeTemplate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboEmailFrom);
			this.Controls.Add(this.gridRecipients);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.textFromAddress);
			this.Controls.Add(this.textBodyTextTemplate);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormEmailJobs";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Email Message";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEmailJobs_FormClosing);
			this.Load += new System.EventHandler(this.FormEmailJobs_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormEmailJobs_Load(object sender, System.EventArgs e) {
			List<long> frNums = JobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request).Select(x => x.FKey).ToList();
			_listFeatureReqs=FeatureRequests.GetAll(frNums);
			_listJobEmails = Jobs.GetCustomerEmails(JobCur)
				.OrderByDescending(x => x.IsQuote)
				.ThenByDescending(x => x.IsTask)
				.ThenByDescending(x => x.IsFeatureReq)
				.ThenByDescending(x => x.PledgeAmount)
				.ThenByDescending(x => x.Votes)
				.ThenBy(x => x.Pat.PatNum).ToList();
			FillComboEmail();
			FillGridRecipients();
			textBodyTextTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultEmail);
			textPledgeTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultBillingMsg);
			textVersions.Text=JobCur.JobVersion;
			textDescriptions.Text=JobCur.Title;
			_listFeatureReqs.ForEach(x => textDescriptions.Text+="\r\n\r\nFeature Request #"+x.FeatReqNum+"\r\n"+x.Description);
		}

		private void FillComboEmail() {
			_listEmailAddresses=EmailAddresses.GetDeepCopy();//Does not include user specific email addresses.
			List<Clinic> listClinicsAll = Clinics.GetDeepCopy();
			_listEmailAddresses.RemoveAll(x => listClinicsAll.Any(y => x.EmailAddressNum==y.EmailAddressNum));//Exclude any email addresses that are associated to a clinic.
			//Exclude default practice email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum));
			//Exclude web mail notification email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			comboEmailFrom.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFrom.SelectedIndex=0;
			textFromAddress.Text=EmailAddresses.GetByClinic(Clinics.ClinicNum).EmailUsername;
			//Add all email addresses which are not associated to a user, a clinic, or either of the default email addresses.
			for(int i = 0;i<_listEmailAddresses.Count;i++) {
				comboEmailFrom.Items.Add(_listEmailAddresses[i].EmailUsername);
			}
			//Add user specific email address if present.
			EmailAddress emailAddressMe = EmailAddresses.GetForUser(Security.CurUser.UserNum);//can be null
			if(emailAddressMe!=null) {
				_listEmailAddresses.Insert(0,emailAddressMe);
				comboEmailFrom.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
				comboEmailFrom.SelectedIndex=1;
				textFromAddress.Text=emailAddressMe.EmailUsername;
			}
		}

		private void FillGridRecipients() {
			gridRecipients.BeginUpdate();
			gridRecipients.ListGridColumns.Clear();
			gridRecipients.ListGridColumns.Add(new GridColumn("Pat\r\nNum",50));
			gridRecipients.ListGridColumns.Add(new GridColumn("Pat\r\nName",200));
			gridRecipients.ListGridColumns.Add(new GridColumn("Email",170));
			gridRecipients.ListGridColumns.Add(new GridColumn("Phone",120));
			gridRecipients.ListGridColumns.Add(new GridColumn("Quote",50) { TextAlign=HorizontalAlignment.Center });
			gridRecipients.ListGridColumns.Add(new GridColumn("Task",50) { TextAlign=HorizontalAlignment.Center });
			gridRecipients.ListGridColumns.Add(new GridColumn("Feature",50) { TextAlign=HorizontalAlignment.Center });
			gridRecipients.ListGridColumns.Add(new GridColumn("Pledge\r\nAmount",75) { TextAlign=HorizontalAlignment.Right });
			gridRecipients.ListGridColumns.Add(new GridColumn("Votes",50) { TextAlign=HorizontalAlignment.Right });
			gridRecipients.ListGridColumns.Add(new GridColumn("Send",50) { TextAlign=HorizontalAlignment.Center });
			gridRecipients.ListGridColumns.Add(new GridColumn("Status",50) { TextAlign=HorizontalAlignment.Center });
			gridRecipients.ListGridRows.Clear();
			foreach(Jobs.JobEmail jobEmail in _listJobEmails) {
				GridRow row = new GridRow();
				row.Cells.Add(jobEmail.Pat.PatNum.ToString());
				row.Cells.Add(jobEmail.Pat.GetNameFL());
				row.Cells.Add(jobEmail.EmailAddress);
				row.Cells.Add(jobEmail.PhoneNums);
				row.Cells.Add(jobEmail.IsQuote ? "X" : "");
				row.Cells.Add(jobEmail.IsTask ? "X" : "");
				row.Cells.Add(jobEmail.IsFeatureReq ? "X" : "");
				row.Cells.Add(jobEmail.PledgeAmount>0 ? jobEmail.PledgeAmount.ToString("c") : "");
				row.Cells.Add(jobEmail.Votes.ToString());
				if(jobEmail.IsSend) {
					row.Cells.Add(new GridCell("X") { ColorBackG=Color.FromArgb(236,255,236) });//light green;
				}
				else {
					row.Cells.Add(new GridCell("") { ColorBackG=Color.FromArgb(222,222,222) });//light gray;
				}
				if(string.IsNullOrEmpty(jobEmail.StatusMsg)) {
					//do nothing
				}
				else if(jobEmail.StatusMsg=="Sent") {
					row.ColorBackG=Color.FromArgb(236,255,236);//light green
				}
				else {
					row.ColorBackG=Color.FromArgb(254,235,233);//light red;
				}
				row.Cells.Add(jobEmail.StatusMsg);
				gridRecipients.ListGridRows.Add(row);
			}
			gridRecipients.EndUpdate();
		}

		private void gridRecipients_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridRecipients.ListGridColumns[e.Col].Heading=="Send") {
				_listJobEmails[e.Row].IsSend=!_listJobEmails[e.Row].IsSend;
				FillGridRecipients();
			}
		}

		///<summary></summary>
		private void butSend_Click(object sender, System.EventArgs e) {
			if(comboEmailFrom.SelectedIndex<0) {
				MessageBox.Show("Select an email address to send from first.");
			}
			EmailAddress emailAddress;
			if(comboEmailFrom.SelectedIndex==0) { //clinic/practice default
				emailAddress = EmailAddresses.GetByClinic(Clinics.ClinicNum);//practice or clinic.
			}
			else { //me or static email address
				emailAddress= _listEmailAddresses[comboEmailFrom.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" and items in combobox
			}
			if(emailAddress==null) {
				MessageBox.Show("Unable to find email address.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textFromAddress.Text)) {
				MsgBox.Show(this,"Please enter a sender address.");
				return;
			}
			if(_listJobEmails.All(x=>!x.IsSend)) {
				MsgBox.Show(this,"Please select at least one recipient.");
				return;
			}
			if(emailAddress.SMTPserver=="") {
				MsgBox.Show(this,"The email address in email setup must have an SMTP server.");
				return;
			}
			if(string.IsNullOrEmpty(textVersions.Text)) {
				if(MessageBox.Show("No version specified, continue?","",MessageBoxButtons.YesNoCancel)!=DialogResult.Yes) {
					return;
				}
			}
			if(string.IsNullOrEmpty(textDescriptions.Text)) {
				if(MessageBox.Show("No description specified, continue?","",MessageBoxButtons.YesNoCancel)!=DialogResult.Yes) {
					return;
				}
			}
			if(string.IsNullOrEmpty(textSubject.Text)) {
				if(MessageBox.Show("No subject specified, continue?","",MessageBoxButtons.YesNoCancel)!=DialogResult.Yes) {
					return;
				}
			}
			StringBuilder sb = new StringBuilder();
			Cursor=Cursors.WaitCursor;
			string template = PrefC.GetString(PrefName.JobManagerDefaultEmail);
			foreach(Jobs.JobEmail je in _listJobEmails) {
				if(!je.IsSend) {
					continue;
				}
				if(ODBuild.IsDebug()) {
					je.EmailAddress="ryan@opendental.com";//send all emails to Ryan for debugging.
				}
				EmailMessage emailMessage = GetJobEmailMessage(je);
				emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
				try {
					EmailMessages.SendEmail(emailMessage,emailAddress);
					je.StatusMsg="Sent";
					je.IsSend=false;
				}
				catch(Exception ex) {
					je.StatusMsg=ex.Message;
					sb.AppendLine(ex.Message+"\r\n");
				}
			}
			Cursor=Cursors.Default;
			_isSent=true;
			butSend.Text="Resend";
			butCancel.Text="Close";
			if(!string.IsNullOrEmpty(sb.ToString())) {
				using MsgBoxCopyPaste msgBox = new MsgBoxCopyPaste("The following errors occurred while sending emails:\r\n"+sb.ToString());
				msgBox.ShowDialog();
			}
			FillGridRecipients();
		}

		private EmailMessage GetJobEmailMessage(Jobs.JobEmail je) {
			EmailMessage retVal = new EmailMessage();
			retVal.PatNum=je.Pat.PatNum;
			retVal.PatNumSubj=je.Pat.PatNum;
			retVal.FromAddress=textFromAddress.Text;
			retVal.RecipientAddress=je.EmailAddress;
			retVal.Subject=textSubject.Text.Replace("[Versions]",textVersions.Text);
			retVal.ToAddress=je.EmailAddress;
			retVal.BodyText=PrefC.GetString(PrefName.JobManagerDefaultEmail);
			retVal.BodyText=retVal.BodyText.Replace("[PledgePara]",je.PledgeAmount==0?"":PrefC.GetString(PrefName.JobManagerDefaultBillingMsg));
			retVal.BodyText=retVal.BodyText.Replace("[PledgeAmount]",je.PledgeAmount.ToString("C"));
			retVal.BodyText=retVal.BodyText.Replace("[Descriptions]",textDescriptions.Text);
			retVal.BodyText=retVal.BodyText.Replace("[Versions]",textVersions.Text);
			retVal.MsgDateTime=DateTime.Now;
			retVal.MsgType=EmailMessageSource.JobManager;
			return retVal;
		}

		private void editTemplateToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormEmailJobTemplateEdit FormEJTE = new FormEmailJobTemplateEdit();
			FormEJTE.ShowDialog();
			if(FormEJTE.DialogResult!=DialogResult.OK) {
				return;
			}
			textBodyTextTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultEmail);
			textPledgeTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultBillingMsg);
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}
		private void butExport_Click(object sender,EventArgs e) {
			if(_listJobEmails.Count==0) {
				MsgBox.Show(this,"Recipient list is empty.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			SaveFileDialog Dlg=new SaveFileDialog();
			if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				Dlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			else if(Directory.Exists("C:\\")) {
				Dlg.InitialDirectory="C:\\";
			}
			Dlg.FileName="RecipientEmailList.txt";
			if(Dlg.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			using(StreamWriter sr = File.CreateText(Dlg.FileName)) {
				sr.Write("Job Description: "+JobCur.Title);
				sr.Write("\r\n\r\n"); 
				foreach(Jobs.JobEmail jobEmail in _listJobEmails) {
					sr.Write(jobEmail.Pat.PatNum.ToString()+"\t");
					sr.Write(jobEmail.Pat.GetNameFL()+"\t");
					sr.Write(jobEmail.EmailAddress);
					sr.Write("\r\n");
				}
				sr.Write("\r\n");
				sr.Write("FeatureReqNum\tDescription");
				sr.Write("\r\n\r\n");
				foreach(FeatureRequest req in _listFeatureReqs) {
					sr.Write(req.FeatReqNum+"\t");
					sr.Write(req.Description);
				}
			}
			try {
				System.Diagnostics.Process.Start(Dlg.FileName);
			}
			catch(Exception ex) { ex.DoNothing(); }
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Recipient list exported.");
		}

		private void FormEmailJobs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isSent) {
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

	}
}