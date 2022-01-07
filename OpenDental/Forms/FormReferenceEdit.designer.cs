namespace OpenDental{
	partial class FormReferenceEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReferenceEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.checkBadRef = new System.Windows.Forms.CheckBox();
			this.textName = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textRecentDate = new OpenDental.ValidDate();
			this.label12 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butToday = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(630, 283);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(711, 283);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(127, 107);
			this.textNote.MaxLength = 255;
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Payment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(239, 158);
			this.textNote.TabIndex = 4;
			this.textNote.Text = "";
			// 
			// checkBadRef
			// 
			this.checkBadRef.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBadRef.Location = new System.Drawing.Point(20, 79);
			this.checkBadRef.Name = "checkBadRef";
			this.checkBadRef.Size = new System.Drawing.Size(122, 18);
			this.checkBadRef.TabIndex = 129;
			this.checkBadRef.Text = "Bad Reference";
			this.checkBadRef.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBadRef.UseVisualStyleBackColor = true;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(127, 24);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(242, 20);
			this.textName.TabIndex = 130;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(28, 26);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(97, 16);
			this.label11.TabIndex = 131;
			this.label11.Text = "Name";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textRecentDate
			// 
			this.textRecentDate.Location = new System.Drawing.Point(127, 50);
			this.textRecentDate.Name = "textRecentDate";
			this.textRecentDate.Size = new System.Drawing.Size(100, 20);
			this.textRecentDate.TabIndex = 132;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(10, 54);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(115, 16);
			this.label12.TabIndex = 133;
			this.label12.Text = "Most Recently Used";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25, 108);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 134;
			this.label1.Text = "Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(383, 24);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(403, 241);
			this.gridMain.TabIndex = 135;
			this.gridMain.Title = "Used as a Reference for These New Customers";
			this.gridMain.TranslationName = "FormPatientSelect";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(233, 50);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(48, 20);
			this.butToday.TabIndex = 2;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// FormReferenceEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(798, 319);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textRecentDate);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.checkBadRef);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butToday);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReferenceEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Reference Edit";
			this.Load += new System.EventHandler(this.FormReferenceEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textNote;
		private System.Windows.Forms.CheckBox checkBadRef;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label11;
		private ValidDate textRecentDate;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label1;
		private UI.GridOD gridMain;
		private UI.Button butToday;
	}
}