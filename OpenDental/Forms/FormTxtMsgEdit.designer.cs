namespace OpenDental{
	partial class FormTxtMsgEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTxtMsgEdit));
			this.textWirelessPhone = new OpenDental.ValidPhone();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.radioOther = new System.Windows.Forms.RadioButton();
			this.groupRecipient = new System.Windows.Forms.GroupBox();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butPatFind = new OpenDental.UI.Button();
			this.textMessage = new OpenDental.ODtextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelCharCount = new System.Windows.Forms.Label();
			this.labelMsgCount = new System.Windows.Forms.Label();
			this.textCharCount = new OpenDental.ODtextBox();
			this.textMsgCount = new OpenDental.ODtextBox();
			this.groupRecipient.SuspendLayout();
			this.SuspendLayout();
			// 
			// textWirelessPhone
			// 
			this.textWirelessPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textWirelessPhone.Location = new System.Drawing.Point(219, 91);
			this.textWirelessPhone.Name = "textWirelessPhone";
			this.textWirelessPhone.Size = new System.Drawing.Size(140, 20);
			this.textWirelessPhone.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(216, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Wireless Phone Number";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(25, 158);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 20);
			this.label2.TabIndex = 6;
			this.label2.Text = "Text Message";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(6, 18);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioPatient.Size = new System.Drawing.Size(120, 17);
			this.radioPatient.TabIndex = 1;
			this.radioPatient.TabStop = true;
			this.radioPatient.Text = "Patient";
			this.radioPatient.UseVisualStyleBackColor = true;
			this.radioPatient.Click += new System.EventHandler(this.radioPatient_Click);
			// 
			// radioOther
			// 
			this.radioOther.Location = new System.Drawing.Point(132, 18);
			this.radioOther.Name = "radioOther";
			this.radioOther.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioOther.Size = new System.Drawing.Size(120, 17);
			this.radioOther.TabIndex = 2;
			this.radioOther.TabStop = true;
			this.radioOther.Text = "Another Person";
			this.radioOther.UseVisualStyleBackColor = true;
			this.radioOther.Click += new System.EventHandler(this.radioOther_Click);
			// 
			// groupRecipient
			// 
			this.groupRecipient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupRecipient.Controls.Add(this.radioPatient);
			this.groupRecipient.Controls.Add(this.radioOther);
			this.groupRecipient.Location = new System.Drawing.Point(28, 13);
			this.groupRecipient.Name = "groupRecipient";
			this.groupRecipient.Size = new System.Drawing.Size(331, 43);
			this.groupRecipient.TabIndex = 9;
			this.groupRecipient.TabStop = false;
			this.groupRecipient.Text = "Choose one of the following options:";
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(28, 91);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(185, 20);
			this.textPatient.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(25, 67);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 20);
			this.label4.TabIndex = 163;
			this.label4.Text = "Patient";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPatFind
			// 
			this.butPatFind.Location = new System.Drawing.Point(28, 117);
			this.butPatFind.Name = "butPatFind";
			this.butPatFind.Size = new System.Drawing.Size(63, 24);
			this.butPatFind.TabIndex = 5;
			this.butPatFind.Text = "Find";
			this.butPatFind.Click += new System.EventHandler(this.butPatFind_Click);
			// 
			// textMessage
			// 
			this.textMessage.AcceptsTab = true;
			this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textMessage.DetectLinksEnabled = false;
			this.textMessage.DetectUrls = false;
			this.textMessage.Location = new System.Drawing.Point(28, 181);
			this.textMessage.Name = "textMessage";
			this.textMessage.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessage.Size = new System.Drawing.Size(331, 113);
			this.textMessage.TabIndex = 6;
			this.textMessage.Text = "";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(209, 303);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&Send";
			this.butOK.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(290, 303);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelCharCount
			// 
			this.labelCharCount.Location = new System.Drawing.Point(190, 135);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new System.Drawing.Size(115, 13);
			this.labelCharCount.TabIndex = 164;
			this.labelCharCount.Text = "Character Count";
			this.labelCharCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelMsgCount
			// 
			this.labelMsgCount.Location = new System.Drawing.Point(190, 162);
			this.labelMsgCount.Name = "labelMsgCount";
			this.labelMsgCount.Size = new System.Drawing.Size(115, 13);
			this.labelMsgCount.TabIndex = 165;
			this.labelMsgCount.Text = "Message Count";
			this.labelMsgCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCharCount
			// 
			this.textCharCount.AcceptsTab = true;
			this.textCharCount.BackColor = System.Drawing.SystemColors.Control;
			this.textCharCount.DetectLinksEnabled = false;
			this.textCharCount.DetectUrls = false;
			this.textCharCount.Location = new System.Drawing.Point(306, 132);
			this.textCharCount.Name = "textCharCount";
			this.textCharCount.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textCharCount.ReadOnly = true;
			this.textCharCount.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCharCount.Size = new System.Drawing.Size(53, 20);
			this.textCharCount.TabIndex = 166;
			this.textCharCount.TabStop = false;
			this.textCharCount.Text = "0";
			// 
			// textMsgCount
			// 
			this.textMsgCount.AcceptsTab = true;
			this.textMsgCount.BackColor = System.Drawing.SystemColors.Control;
			this.textMsgCount.DetectLinksEnabled = false;
			this.textMsgCount.DetectUrls = false;
			this.textMsgCount.Location = new System.Drawing.Point(306, 159);
			this.textMsgCount.Name = "textMsgCount";
			this.textMsgCount.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textMsgCount.ReadOnly = true;
			this.textMsgCount.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMsgCount.Size = new System.Drawing.Size(53, 20);
			this.textMsgCount.TabIndex = 167;
			this.textMsgCount.TabStop = false;
			this.textMsgCount.Text = "0";
			// 
			// FormTxtMsgEdit
			// 
			this.ClientSize = new System.Drawing.Size(377, 339);
			this.Controls.Add(this.textMsgCount);
			this.Controls.Add(this.textCharCount);
			this.Controls.Add(this.labelMsgCount);
			this.Controls.Add(this.labelCharCount);
			this.Controls.Add(this.butPatFind);
			this.Controls.Add(this.groupRecipient);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textWirelessPhone);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTxtMsgEdit";
			this.Text = "Text Message";
			this.Load += new System.EventHandler(this.FormTxtMsgEdit_Load);
			this.groupRecipient.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textMessage;
		private ValidPhone textWirelessPhone;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioPatient;
		private System.Windows.Forms.RadioButton radioOther;
		private System.Windows.Forms.GroupBox groupRecipient;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label label4;
		private UI.Button butPatFind;
		private System.Windows.Forms.Label labelCharCount;
		private System.Windows.Forms.Label labelMsgCount;
		private ODtextBox textCharCount;
		private ODtextBox textMsgCount;
	}
}