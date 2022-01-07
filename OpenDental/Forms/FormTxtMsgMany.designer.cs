namespace OpenDental{
	partial class FormTxtMsgMany {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTxtMsgMany));
			this.label2 = new System.Windows.Forms.Label();
			this.textMessage = new OpenDental.ODtextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.labelCharCount = new System.Windows.Forms.Label();
			this.labelMsgCount = new System.Windows.Forms.Label();
			this.textCharCount = new OpenDental.ODtextBox();
			this.textMsgCountPerPatient = new OpenDental.ODtextBox();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(25, 183);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 20);
			this.label2.TabIndex = 6;
			this.label2.Text = "Text Message";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.textMessage.Location = new System.Drawing.Point(28, 206);
			this.textMessage.Name = "textMessage";
			this.textMessage.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessage.Size = new System.Drawing.Size(331, 141);
			this.textMessage.TabIndex = 6;
			this.textMessage.Text = "";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(209, 372);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&Send";
			this.butOK.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(290, 372);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(28, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMain.Size = new System.Drawing.Size(331, 143);
			this.gridMain.TabIndex = 9;
			this.gridMain.Title = "Recipients";
			this.gridMain.TranslationName = "TableTextMany";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(25, 350);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(334, 20);
			this.label1.TabIndex = 10;
			this.label1.Text = "Available tags: [NameF]";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCharCount
			// 
			this.labelCharCount.Location = new System.Drawing.Point(188, 163);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new System.Drawing.Size(115, 13);
			this.labelCharCount.TabIndex = 11;
			this.labelCharCount.Text = "Character Count";
			this.labelCharCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelMsgCount
			// 
			this.labelMsgCount.Location = new System.Drawing.Point(130, 187);
			this.labelMsgCount.Name = "labelMsgCount";
			this.labelMsgCount.Size = new System.Drawing.Size(173, 13);
			this.labelMsgCount.TabIndex = 12;
			this.labelMsgCount.Text = "Message Count Per Patient";
			this.labelMsgCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCharCount
			// 
			this.textCharCount.AcceptsTab = true;
			this.textCharCount.BackColor = System.Drawing.SystemColors.Control;
			this.textCharCount.DetectLinksEnabled = false;
			this.textCharCount.DetectUrls = false;
			this.textCharCount.Location = new System.Drawing.Point(306, 160);
			this.textCharCount.Name = "textCharCount";
			this.textCharCount.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textCharCount.ReadOnly = true;
			this.textCharCount.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCharCount.Size = new System.Drawing.Size(53, 20);
			this.textCharCount.TabIndex = 13;
			this.textCharCount.TabStop = false;
			this.textCharCount.Text = "0";
			// 
			// textMsgCountPerPatient
			// 
			this.textMsgCountPerPatient.AcceptsTab = true;
			this.textMsgCountPerPatient.BackColor = System.Drawing.SystemColors.Control;
			this.textMsgCountPerPatient.DetectLinksEnabled = false;
			this.textMsgCountPerPatient.DetectUrls = false;
			this.textMsgCountPerPatient.Location = new System.Drawing.Point(306, 184);
			this.textMsgCountPerPatient.Name = "textMsgCountPerPatient";
			this.textMsgCountPerPatient.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textMsgCountPerPatient.ReadOnly = true;
			this.textMsgCountPerPatient.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMsgCountPerPatient.Size = new System.Drawing.Size(53, 20);
			this.textMsgCountPerPatient.TabIndex = 14;
			this.textMsgCountPerPatient.TabStop = false;
			this.textMsgCountPerPatient.Text = "0";
			// 
			// FormTxtMsgMany
			// 
			this.ClientSize = new System.Drawing.Size(377, 408);
			this.Controls.Add(this.textMsgCountPerPatient);
			this.Controls.Add(this.textCharCount);
			this.Controls.Add(this.labelMsgCount);
			this.Controls.Add(this.labelCharCount);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMessage);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTxtMsgMany";
			this.Text = "Text Message";
			this.Load += new System.EventHandler(this.FormTxtMsgMany_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textMessage;
		private System.Windows.Forms.Label label2;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCharCount;
		private System.Windows.Forms.Label labelMsgCount;
		private ODtextBox textCharCount;
		private ODtextBox textMsgCountPerPatient;
	}
}