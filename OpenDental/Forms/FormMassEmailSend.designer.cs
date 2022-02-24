namespace OpenDental{
	partial class FormMassEmailSend {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailSend));
			this.butSendEmails = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.userControlEmailTemplate1 = new OpenDental.UserControlEmailTemplate();
			this.textSubject = new OpenDental.ODtextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelSendingPatients = new System.Windows.Forms.Label();
			this.checkDisplay = new System.Windows.Forms.CheckBox();
			this.textPatient = new OpenDental.ODtextBox();
			this.butPatientSelect = new OpenDental.UI.Button();
			this.labelReplacedData = new System.Windows.Forms.Label();
			this.textboxAlias = new OpenDental.ODtextBox();
			this.labelEmailAlias = new System.Windows.Forms.Label();
			this.textSenderAddress = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSelectSender = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textEmailGroup = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupSender = new System.Windows.Forms.GroupBox();
			this.butVerifications = new OpenDental.UI.Button();
			this.radioReturnAddress = new System.Windows.Forms.RadioButton();
			this.radioNoReply = new System.Windows.Forms.RadioButton();
			this.groupSender.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSendEmails
			// 
			this.butSendEmails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendEmails.Location = new System.Drawing.Point(818, 660);
			this.butSendEmails.Name = "butSendEmails";
			this.butSendEmails.Size = new System.Drawing.Size(88, 24);
			this.butSendEmails.TabIndex = 3;
			this.butSendEmails.Text = "Send Emails";
			this.butSendEmails.Click += new System.EventHandler(this.butSendEmails_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(912, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// userControlEmailTemplate1
			// 
			this.userControlEmailTemplate1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.userControlEmailTemplate1.Location = new System.Drawing.Point(12, 252);
			this.userControlEmailTemplate1.Name = "userControlEmailTemplate1";
			this.userControlEmailTemplate1.Size = new System.Drawing.Size(975, 402);
			this.userControlEmailTemplate1.TabIndex = 4;
			// 
			// textSubject
			// 
			this.textSubject.AcceptsTab = true;
			this.textSubject.BackColor = System.Drawing.SystemColors.Control;
			this.textSubject.DetectLinksEnabled = false;
			this.textSubject.DetectUrls = false;
			this.textSubject.Location = new System.Drawing.Point(25, 186);
			this.textSubject.Multiline = false;
			this.textSubject.Name = "textSubject";
			this.textSubject.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSubject.ReadOnly = true;
			this.textSubject.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSubject.Size = new System.Drawing.Size(470, 20);
			this.textSubject.TabIndex = 101;
			this.textSubject.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(22, 165);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 18);
			this.label2.TabIndex = 100;
			this.label2.Text = "Subject";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSendingPatients
			// 
			this.labelSendingPatients.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSendingPatients.ForeColor = System.Drawing.Color.LimeGreen;
			this.labelSendingPatients.Location = new System.Drawing.Point(22, 9);
			this.labelSendingPatients.Name = "labelSendingPatients";
			this.labelSendingPatients.Size = new System.Drawing.Size(473, 18);
			this.labelSendingPatients.TabIndex = 102;
			this.labelSendingPatients.Text = "Sending the following email template to ### patients";
			this.labelSendingPatients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkDisplay
			// 
			this.checkDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkDisplay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDisplay.Location = new System.Drawing.Point(710, 10);
			this.checkDisplay.Name = "checkDisplay";
			this.checkDisplay.Size = new System.Drawing.Size(277, 18);
			this.checkDisplay.TabIndex = 214;
			this.checkDisplay.Text = "Display rendering with replaced data";
			this.checkDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDisplay.UseVisualStyleBackColor = true;
			this.checkDisplay.Click += new System.EventHandler(this.checkDisplay_Click);
			// 
			// textPatient
			// 
			this.textPatient.AcceptsTab = true;
			this.textPatient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textPatient.BackColor = System.Drawing.SystemColors.Control;
			this.textPatient.DetectLinksEnabled = false;
			this.textPatient.DetectUrls = false;
			this.textPatient.Location = new System.Drawing.Point(794, 34);
			this.textPatient.Multiline = false;
			this.textPatient.Name = "textPatient";
			this.textPatient.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textPatient.ReadOnly = true;
			this.textPatient.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPatient.Size = new System.Drawing.Size(112, 20);
			this.textPatient.TabIndex = 215;
			this.textPatient.Text = "";
			// 
			// butPatientSelect
			// 
			this.butPatientSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPatientSelect.Location = new System.Drawing.Point(912, 33);
			this.butPatientSelect.Name = "butPatientSelect";
			this.butPatientSelect.Size = new System.Drawing.Size(36, 20);
			this.butPatientSelect.TabIndex = 216;
			this.butPatientSelect.Text = "...";
			this.butPatientSelect.Click += new System.EventHandler(this.butPatientSelect_Click);
			// 
			// labelReplacedData
			// 
			this.labelReplacedData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelReplacedData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelReplacedData.ForeColor = System.Drawing.Color.LimeGreen;
			this.labelReplacedData.Location = new System.Drawing.Point(791, 57);
			this.labelReplacedData.Name = "labelReplacedData";
			this.labelReplacedData.Size = new System.Drawing.Size(196, 18);
			this.labelReplacedData.TabIndex = 217;
			this.labelReplacedData.Text = "With replaced data";
			this.labelReplacedData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textboxAlias
			// 
			this.textboxAlias.AcceptsTab = true;
			this.textboxAlias.BackColor = System.Drawing.SystemColors.Window;
			this.textboxAlias.DetectLinksEnabled = false;
			this.textboxAlias.DetectUrls = false;
			this.textboxAlias.Location = new System.Drawing.Point(9, 37);
			this.textboxAlias.Multiline = false;
			this.textboxAlias.Name = "textboxAlias";
			this.textboxAlias.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textboxAlias.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textboxAlias.Size = new System.Drawing.Size(470, 20);
			this.textboxAlias.TabIndex = 219;
			this.textboxAlias.Text = "";
			// 
			// labelEmailAlias
			// 
			this.labelEmailAlias.Location = new System.Drawing.Point(6, 16);
			this.labelEmailAlias.Name = "labelEmailAlias";
			this.labelEmailAlias.Size = new System.Drawing.Size(182, 18);
			this.labelEmailAlias.TabIndex = 218;
			this.labelEmailAlias.Text = "Alias of sender";
			this.labelEmailAlias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSenderAddress
			// 
			this.textSenderAddress.AcceptsTab = true;
			this.textSenderAddress.BackColor = System.Drawing.SystemColors.Control;
			this.textSenderAddress.DetectLinksEnabled = false;
			this.textSenderAddress.DetectUrls = false;
			this.textSenderAddress.Location = new System.Drawing.Point(25, 142);
			this.textSenderAddress.Multiline = false;
			this.textSenderAddress.Name = "textSenderAddress";
			this.textSenderAddress.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSenderAddress.ReadOnly = true;
			this.textSenderAddress.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSenderAddress.Size = new System.Drawing.Size(428, 20);
			this.textSenderAddress.TabIndex = 221;
			this.textSenderAddress.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 121);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 18);
			this.label1.TabIndex = 220;
			this.label1.Text = "Return email address";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSelectSender
			// 
			this.butSelectSender.Location = new System.Drawing.Point(459, 139);
			this.butSelectSender.Name = "butSelectSender";
			this.butSelectSender.Size = new System.Drawing.Size(36, 24);
			this.butSelectSender.TabIndex = 222;
			this.butSelectSender.Text = "...";
			this.butSelectSender.UseVisualStyleBackColor = true;
			this.butSelectSender.Click += new System.EventHandler(this.butSelectSender_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(558, 666);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(254, 18);
			this.label3.TabIndex = 223;
			this.label3.Text = "(this will first send a test email to the sender)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmailGroup
			// 
			this.textEmailGroup.AcceptsTab = true;
			this.textEmailGroup.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailGroup.DetectLinksEnabled = false;
			this.textEmailGroup.DetectUrls = false;
			this.textEmailGroup.Location = new System.Drawing.Point(25, 230);
			this.textEmailGroup.Multiline = false;
			this.textEmailGroup.Name = "textEmailGroup";
			this.textEmailGroup.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailGroup.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailGroup.Size = new System.Drawing.Size(470, 20);
			this.textEmailGroup.TabIndex = 224;
			this.textEmailGroup.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(22, 209);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(151, 18);
			this.label4.TabIndex = 225;
			this.label4.Text = "Mass email group name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(501, 231);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(327, 18);
			this.label5.TabIndex = 226;
			this.label5.Tag = "";
			this.label5.Text = "This will help identify the emails when looking at analytics.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSender
			// 
			this.groupSender.Controls.Add(this.butVerifications);
			this.groupSender.Controls.Add(this.radioReturnAddress);
			this.groupSender.Controls.Add(this.radioNoReply);
			this.groupSender.Controls.Add(this.labelEmailAlias);
			this.groupSender.Controls.Add(this.textboxAlias);
			this.groupSender.Location = new System.Drawing.Point(17, 30);
			this.groupSender.Name = "groupSender";
			this.groupSender.Size = new System.Drawing.Size(490, 90);
			this.groupSender.TabIndex = 228;
			this.groupSender.TabStop = false;
			this.groupSender.Text = "Sender";
			// 
			// butVerifications
			// 
			this.butVerifications.Location = new System.Drawing.Point(376, 60);
			this.butVerifications.Name = "butVerifications";
			this.butVerifications.Size = new System.Drawing.Size(102, 24);
			this.butVerifications.TabIndex = 223;
			this.butVerifications.Text = "Sender Addresses";
			this.butVerifications.UseVisualStyleBackColor = true;
			this.butVerifications.Click += new System.EventHandler(this.butVerifications_Click);
			// 
			// radioReturnAddress
			// 
			this.radioReturnAddress.Location = new System.Drawing.Point(119, 60);
			this.radioReturnAddress.Name = "radioReturnAddress";
			this.radioReturnAddress.Size = new System.Drawing.Size(259, 24);
			this.radioReturnAddress.TabIndex = 221;
			this.radioReturnAddress.TabStop = true;
			this.radioReturnAddress.Text = "Return Address";
			this.radioReturnAddress.UseVisualStyleBackColor = true;
			// 
			// radioNoReply
			// 
			this.radioNoReply.Location = new System.Drawing.Point(9, 60);
			this.radioNoReply.Name = "radioNoReply";
			this.radioNoReply.Size = new System.Drawing.Size(104, 24);
			this.radioNoReply.TabIndex = 220;
			this.radioNoReply.TabStop = true;
			this.radioNoReply.Text = "\'NoReply\'";
			this.radioNoReply.UseVisualStyleBackColor = true;
			// 
			// FormMassEmailSend
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(999, 696);
			this.Controls.Add(this.groupSender);
			this.Controls.Add(this.butSelectSender);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textEmailGroup);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textSenderAddress);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butSendEmails);
			this.Controls.Add(this.labelReplacedData);
			this.Controls.Add(this.butPatientSelect);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.checkDisplay);
			this.Controls.Add(this.labelSendingPatients);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.userControlEmailTemplate1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailSend";
			this.Text = "Sending Mass Emails";
			this.Load += new System.EventHandler(this.FormMassEmailSend_Load);
			this.groupSender.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSendEmails;
		private OpenDental.UI.Button butCancel;
		private UserControlEmailTemplate userControlEmailTemplate1;
		private ODtextBox textSubject;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelSendingPatients;
		private System.Windows.Forms.CheckBox checkDisplay;
		private ODtextBox textPatient;
		private UI.Button butPatientSelect;
		private System.Windows.Forms.Label labelReplacedData;
		private ODtextBox textboxAlias;
		private System.Windows.Forms.Label labelEmailAlias;
		private ODtextBox textSenderAddress;
		private System.Windows.Forms.Label label1;
		private UI.Button butSelectSender;
		private System.Windows.Forms.Label label3;
		private ODtextBox textEmailGroup;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupSender;
		private UI.Button butVerifications;
		private System.Windows.Forms.RadioButton radioReturnAddress;
		private System.Windows.Forms.RadioButton radioNoReply;
	}
}