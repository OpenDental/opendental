namespace OpenDental{
	partial class FormWebChatSessionNoteEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebChatSessionNoteEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAutoNote = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.textTech = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(468, 317);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(565, 317);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAutoNote
			// 
			this.butAutoNote.Location = new System.Drawing.Point(304, 41);
			this.butAutoNote.Name = "butAutoNote";
			this.butAutoNote.Size = new System.Drawing.Size(80, 22);
			this.butAutoNote.TabIndex = 4;
			this.butAutoNote.Text = "Auto Note";
			this.butAutoNote.Click += new System.EventHandler(this.butAutoNote_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(34, 317);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 3;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(99, 13);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(164, 20);
			this.textDateTime.TabIndex = 166;
			this.textDateTime.TabStop = false;
			// 
			// textTech
			// 
			this.textTech.Location = new System.Drawing.Point(99, 39);
			this.textTech.Name = "textTech";
			this.textTech.ReadOnly = true;
			this.textTech.Size = new System.Drawing.Size(134, 20);
			this.textTech.TabIndex = 164;
			this.textTech.TabStop = false;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(3, 41);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(94, 16);
			this.label16.TabIndex = 165;
			this.label16.Text = "Tech";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.textNote.Location = new System.Drawing.Point(99, 65);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Task;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(541, 218);
			this.textNote.TabIndex = 0;
			this.textNote.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 16);
			this.label4.TabIndex = 163;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(1, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(97, 16);
			this.label2.TabIndex = 162;
			this.label2.Text = "Date / Time";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormWebChatSessionNoteEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(664, 366);
			this.Controls.Add(this.butAutoNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDateTime);
			this.Controls.Add(this.textTech);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebChatSessionNoteEdit";
			this.Text = "Web Chat Session Note Edit";
			this.Load += new System.EventHandler(this.FormWebChatSessionNoteEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butAutoNote;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textDateTime;
		private System.Windows.Forms.TextBox textTech;
		private System.Windows.Forms.Label label16;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
	}
}