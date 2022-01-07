namespace OpenDental{
	partial class FormWebMailMessageEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebMailMessageEdit));
			this.textTo = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textSubject = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.labelNotification = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboRegardingPatient = new System.Windows.Forms.ComboBox();
			this.listAttachments = new OpenDental.UI.ListBoxOD();
			this.contextMenuAttachments = new System.Windows.Forms.ContextMenu();
			this.menuItemAttachmentPreview = new System.Windows.Forms.MenuItem();
			this.menuItemAttachmentRemove = new System.Windows.Forms.MenuItem();
			this.butDelete = new OpenDental.UI.Button();
			this.butAttach = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.textBody = new OpenDental.ODtextBox();
			this.butSend = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butProvPick = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// textTo
			// 
			this.textTo.Location = new System.Drawing.Point(120, 59);
			this.textTo.Name = "textTo";
			this.textTo.ReadOnly = true;
			this.textTo.Size = new System.Drawing.Size(305, 20);
			this.textTo.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 14);
			this.label1.TabIndex = 11;
			this.label1.Text = "To:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFrom
			// 
			this.textFrom.Location = new System.Drawing.Point(119, 85);
			this.textFrom.Name = "textFrom";
			this.textFrom.ReadOnly = true;
			this.textFrom.Size = new System.Drawing.Size(270, 20);
			this.textFrom.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(102, 14);
			this.label3.TabIndex = 13;
			this.label3.Text = "From:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 139);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 14);
			this.label2.TabIndex = 13;
			this.label2.Text = "Message:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubject
			// 
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.Location = new System.Drawing.Point(119, 113);
			this.textSubject.Name = "textSubject";
			this.textSubject.ReadOnly = true;
			this.textSubject.Size = new System.Drawing.Size(632, 20);
			this.textSubject.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 116);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(102, 14);
			this.label4.TabIndex = 16;
			this.label4.Text = "Subject:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNotification
			// 
			this.labelNotification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNotification.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelNotification.Location = new System.Drawing.Point(17, 355);
			this.labelNotification.Name = "labelNotification";
			this.labelNotification.Size = new System.Drawing.Size(652, 14);
			this.labelNotification.TabIndex = 17;
			this.labelNotification.Text = "Warning: Patient email is not setup properly. No notification email will be sent " +
    "to this patient.";
			this.labelNotification.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(14, 31);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 14);
			this.label5.TabIndex = 19;
			this.label5.Text = "Regarding Patient:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRegardingPatient
			// 
			this.comboRegardingPatient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRegardingPatient.FormattingEnabled = true;
			this.comboRegardingPatient.Location = new System.Drawing.Point(120, 28);
			this.comboRegardingPatient.MaxDropDownItems = 30;
			this.comboRegardingPatient.Name = "comboRegardingPatient";
			this.comboRegardingPatient.Size = new System.Drawing.Size(304, 21);
			this.comboRegardingPatient.TabIndex = 0;
			// 
			// listAttachments
			// 
			this.listAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listAttachments.Location = new System.Drawing.Point(431, 49);
			this.listAttachments.Name = "listAttachments";
			this.listAttachments.Size = new System.Drawing.Size(321, 56);
			this.listAttachments.TabIndex = 20;
			this.listAttachments.TabStop = false;
			this.listAttachments.DoubleClick += new System.EventHandler(this.listAttachments_DoubleClick);
			this.listAttachments.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listAttachments_MouseDown);
			// 
			// contextMenuAttachments
			// 
			this.contextMenuAttachments.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAttachmentPreview,
            this.menuItemAttachmentRemove});
			// 
			// menuItemAttachmentPreview
			// 
			this.menuItemAttachmentPreview.Index = 0;
			this.menuItemAttachmentPreview.Text = "Open";
			this.menuItemAttachmentPreview.Click += new System.EventHandler(this.menuItemAttachmentPreview_Click);
			// 
			// menuItemAttachmentRemove
			// 
			this.menuItemAttachmentRemove.Index = 1;
			this.menuItemAttachmentRemove.Text = "Remove";
			this.menuItemAttachmentRemove.Click += new System.EventHandler(this.menuItemAttachmentRemove_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(39, 372);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 22;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAttach
			// 
			this.butAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAttach.Location = new System.Drawing.Point(678, 27);
			this.butAttach.Name = "butAttach";
			this.butAttach.Size = new System.Drawing.Size(74, 21);
			this.butAttach.TabIndex = 21;
			this.butAttach.Text = "Attach...";
			this.butAttach.Click += new System.EventHandler(this.butAttach_Click);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPreview.Location = new System.Drawing.Point(39, 328);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(74, 24);
			this.butPreview.TabIndex = 5;
			this.butPreview.Text = "&Cancel";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// textBody
			// 
			this.textBody.AcceptsTab = true;
			this.textBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBody.BackColor = System.Drawing.SystemColors.Control;
			this.textBody.DetectLinksEnabled = false;
			this.textBody.DetectUrls = false;
			this.textBody.Location = new System.Drawing.Point(119, 139);
			this.textBody.Name = "textBody";
			this.textBody.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textBody.ReadOnly = true;
			this.textBody.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBody.Size = new System.Drawing.Size(632, 213);
			this.textBody.TabIndex = 4;
			this.textBody.Text = "";
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(595, 372);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 6;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(676, 372);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butProvPick
			// 
			this.butProvPick.Location = new System.Drawing.Point(395, 85);
			this.butProvPick.Name = "butProvPick";
			this.butProvPick.Size = new System.Drawing.Size(29, 20);
			this.butProvPick.TabIndex = 23;
			this.butProvPick.Text = "...";
			this.butProvPick.Click += new System.EventHandler(this.butProvPick_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(780, 24);
			this.menuMain.TabIndex = 24;
			// 
			// FormWebMailMessageEdit
			// 
			this.ClientSize = new System.Drawing.Size(780, 408);
			this.Controls.Add(this.butProvPick);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listAttachments);
			this.Controls.Add(this.butAttach);
			this.Controls.Add(this.comboRegardingPatient);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.labelNotification);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBody);
			this.Controls.Add(this.textFrom);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textTo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebMailMessageEdit";
			this.Text = "Web Mail Message Edit";
			this.Load += new System.EventHandler(this.FormWebMailMessageEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSend;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textTo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFrom;
		private System.Windows.Forms.Label label3;
		private ODtextBox textBody;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textSubject;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelNotification;
		private UI.Button butPreview;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboRegardingPatient;
		private OpenDental.UI.ListBoxOD listAttachments;
		private UI.Button butAttach;
		private System.Windows.Forms.ContextMenu contextMenuAttachments;
		private System.Windows.Forms.MenuItem menuItemAttachmentRemove;
		private System.Windows.Forms.MenuItem menuItemAttachmentPreview;
		private UI.Button butDelete;
		private UI.Button butProvPick;
		private UI.MenuOD menuMain;
	}
}