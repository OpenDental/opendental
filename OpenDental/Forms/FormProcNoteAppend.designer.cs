namespace OpenDental{
	partial class FormProcNoteAppend {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcNoteAppend));
			this.butSave = new OpenDental.UI.Button();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.label15 = new System.Windows.Forms.Label();
			this.buttonUseAutoNote = new OpenDental.UI.Button();
			this.textUser = new System.Windows.Forms.TextBox();
			this.textNotes = new OpenDental.ODtextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textAppended = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelPermAlert = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(487, 447);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(112, 340);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(364, 81);
			this.signatureBoxWrapper.TabIndex = 107;
			this.signatureBoxWrapper.UserSig = null;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(109, 319);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(136, 18);
			this.label15.TabIndex = 108;
			this.label15.Text = "Signature / Initials";
			this.label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// buttonUseAutoNote
			// 
			this.buttonUseAutoNote.Location = new System.Drawing.Point(230, 17);
			this.buttonUseAutoNote.Name = "buttonUseAutoNote";
			this.buttonUseAutoNote.Size = new System.Drawing.Size(80, 22);
			this.buttonUseAutoNote.TabIndex = 113;
			this.buttonUseAutoNote.Text = "Auto Note";
			this.buttonUseAutoNote.Click += new System.EventHandler(this.buttonUseAutoNote_Click);
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(112, 18);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(116, 20);
			this.textUser.TabIndex = 112;
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Control;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.Location = new System.Drawing.Point(112, 38);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Procedure;
			this.textNotes.ReadOnly = true;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(450, 164);
			this.textNotes.TabIndex = 110;
			this.textNotes.Text = "";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(37, 19);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(73, 16);
			this.label16.TabIndex = 111;
			this.label16.Text = "User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(24, 42);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 16);
			this.label7.TabIndex = 109;
			this.label7.Text = "Original Note";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAppended
			// 
			this.textAppended.AcceptsTab = true;
			this.textAppended.BackColor = System.Drawing.SystemColors.Window;
			this.textAppended.DetectLinksEnabled = false;
			this.textAppended.DetectUrls = false;
			this.textAppended.HasAutoNotes = true;
			this.textAppended.Location = new System.Drawing.Point(112, 208);
			this.textAppended.Name = "textAppended";
			this.textAppended.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Procedure;
			this.textAppended.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAppended.Size = new System.Drawing.Size(450, 108);
			this.textAppended.TabIndex = 115;
			this.textAppended.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 212);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 16);
			this.label1.TabIndex = 114;
			this.label1.Text = "Appended Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelPermAlert
			// 
			this.labelPermAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPermAlert.ForeColor = System.Drawing.Color.DarkRed;
			this.labelPermAlert.Location = new System.Drawing.Point(113, 425);
			this.labelPermAlert.Name = "labelPermAlert";
			this.labelPermAlert.Size = new System.Drawing.Size(282, 20);
			this.labelPermAlert.TabIndex = 212;
			this.labelPermAlert.Text = "Notes can only be signed by providers.";
			this.labelPermAlert.Visible = false;
			// 
			// FormProcNoteAppend
			// 
			this.ClientSize = new System.Drawing.Size(572, 483);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.labelPermAlert);
			this.Controls.Add(this.textAppended);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonUseAutoNote);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.signatureBoxWrapper);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcNoteAppend";
			this.Text = "Procedure Note Append";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProcNoteAppend_FormClosing);
			this.Load += new System.EventHandler(this.FormProcNoteAppend_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private System.Windows.Forms.Label label15;
		private UI.Button buttonUseAutoNote;
		private System.Windows.Forms.TextBox textUser;
		private ODtextBox textNotes;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label7;
		private ODtextBox textAppended;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelPermAlert;
	}
}