namespace OpenDental{
	partial class FormCommReferral {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCommReferral));
			this.butSave = new OpenDental.UI.Button();
			this.labelDateTimeCreated = new System.Windows.Forms.Label();
			this.textDateTimeCreated = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.listSentOrReceived = new OpenDental.UI.ListBox();
			this.labelSentorReceived = new System.Windows.Forms.Label();
			this.listMode = new OpenDental.UI.ListBox();
			this.labelMode = new System.Windows.Forms.Label();
			this.checkHidden = new OpenDental.UI.CheckBox();
			this.checkAnchored = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(523, 356);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 5;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelDateTimeCreated
			// 
			this.labelDateTimeCreated.Location = new System.Drawing.Point(2, 58);
			this.labelDateTimeCreated.Name = "labelDateTimeCreated";
			this.labelDateTimeCreated.Size = new System.Drawing.Size(106, 18);
			this.labelDateTimeCreated.TabIndex = 148;
			this.labelDateTimeCreated.Text = "Date/Time Created";
			this.labelDateTimeCreated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTimeCreated
			// 
			this.textDateTimeCreated.Location = new System.Drawing.Point(108, 57);
			this.textDateTimeCreated.Name = "textDateTimeCreated";
			this.textDateTimeCreated.ReadOnly = true;
			this.textDateTimeCreated.Size = new System.Drawing.Size(205, 20);
			this.textDateTimeCreated.TabIndex = 147;
			// 
			// textUser
			// 
			this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUser.Location = new System.Drawing.Point(479, 12);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(119, 20);
			this.textUser.TabIndex = 150;
			// 
			// labelUser
			// 
			this.labelUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelUser.Location = new System.Drawing.Point(413, 13);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(60, 16);
			this.labelUser.TabIndex = 149;
			this.labelUser.Text = "User";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.HasAutoNotes = true;
			this.textNote.Location = new System.Drawing.Point(108, 240);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(451, 105);
			this.textNote.TabIndex = 0;
			this.textNote.Text = "";
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(105, 223);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(82, 16);
			this.labelNote.TabIndex = 152;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listSentOrReceived
			// 
			this.listSentOrReceived.Location = new System.Drawing.Point(246, 107);
			this.listSentOrReceived.Name = "listSentOrReceived";
			this.listSentOrReceived.Size = new System.Drawing.Size(101, 43);
			this.listSentOrReceived.TabIndex = 2;
			// 
			// labelSentorReceived
			// 
			this.labelSentorReceived.Location = new System.Drawing.Point(250, 89);
			this.labelSentorReceived.Name = "labelSentorReceived";
			this.labelSentorReceived.Size = new System.Drawing.Size(142, 16);
			this.labelSentorReceived.TabIndex = 155;
			this.labelSentorReceived.Text = "Sent or Received";
			this.labelSentorReceived.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listMode
			// 
			this.listMode.Location = new System.Drawing.Point(108, 107);
			this.listMode.Name = "listMode";
			this.listMode.Size = new System.Drawing.Size(125, 108);
			this.listMode.TabIndex = 1;
			// 
			// labelMode
			// 
			this.labelMode.Location = new System.Drawing.Point(107, 90);
			this.labelMode.Name = "labelMode";
			this.labelMode.Size = new System.Drawing.Size(82, 16);
			this.labelMode.TabIndex = 153;
			this.labelMode.Text = "Mode";
			this.labelMode.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkHidden
			// 
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(5, 9);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkHidden.Size = new System.Drawing.Size(115, 18);
			this.checkHidden.TabIndex = 3;
			this.checkHidden.Text = "Hidden  ";
			// 
			// checkAnchored
			// 
			this.checkAnchored.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAnchored.Location = new System.Drawing.Point(5, 33);
			this.checkAnchored.Name = "checkAnchored";
			this.checkAnchored.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAnchored.Size = new System.Drawing.Size(115, 18);
			this.checkAnchored.TabIndex = 4;
			this.checkAnchored.Text = "Anchored to top";
			// 
			// FormCommReferral
			// 
			this.ClientSize = new System.Drawing.Size(610, 392);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.checkAnchored);
			this.Controls.Add(this.listSentOrReceived);
			this.Controls.Add(this.labelSentorReceived);
			this.Controls.Add(this.listMode);
			this.Controls.Add(this.labelMode);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.labelUser);
			this.Controls.Add(this.labelDateTimeCreated);
			this.Controls.Add(this.textDateTimeCreated);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCommReferral";
			this.Text = "Communication Item for Referral";
			this.Load += new System.EventHandler(this.FormCommReferral_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label labelDateTimeCreated;
		private System.Windows.Forms.TextBox textDateTimeCreated;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label labelUser;
		private ODtextBox textNote;
		private System.Windows.Forms.Label labelNote;
		private UI.ListBox listSentOrReceived;
		private System.Windows.Forms.Label labelSentorReceived;
		private UI.ListBox listMode;
		private System.Windows.Forms.Label labelMode;
		private UI.CheckBox checkHidden;
		private UI.CheckBox checkAnchored;
	}
}