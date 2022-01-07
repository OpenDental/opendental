namespace OpenDental{
	partial class FormJobQuoteEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobQuoteEdit));
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textQuoteHours = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textApprovedAmount = new System.Windows.Forms.TextBox();
			this.checkIsApproved = new System.Windows.Forms.CheckBox();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butPatPicker = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(12, 123);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(60, 20);
			this.label9.TabIndex = 0;
			this.label9.Text = "Note";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(9, 52);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(134, 20);
			this.label10.TabIndex = 0;
			this.label10.Text = "Quote Amount";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.textNote.Location = new System.Drawing.Point(12, 146);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(389, 152);
			this.textNote.TabIndex = 11;
			this.textNote.Text = "";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 304);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 17;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(245, 304);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 15;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(326, 304);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 16;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(144, 52);
			this.textAmount.MaxLength = 100;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(183, 20);
			this.textAmount.TabIndex = 0;
			this.textAmount.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 20);
			this.label2.TabIndex = 19;
			this.label2.Text = "Quote Hours";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textQuoteHours
			// 
			this.textQuoteHours.Location = new System.Drawing.Point(144, 30);
			this.textQuoteHours.MaxLength = 100;
			this.textQuoteHours.Name = "textQuoteHours";
			this.textQuoteHours.Size = new System.Drawing.Size(183, 20);
			this.textQuoteHours.TabIndex = 20;
			this.textQuoteHours.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(134, 20);
			this.label3.TabIndex = 21;
			this.label3.Text = "Approved Amount";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textApprovedAmount
			// 
			this.textApprovedAmount.Location = new System.Drawing.Point(144, 74);
			this.textApprovedAmount.MaxLength = 100;
			this.textApprovedAmount.Name = "textApprovedAmount";
			this.textApprovedAmount.Size = new System.Drawing.Size(183, 20);
			this.textApprovedAmount.TabIndex = 22;
			this.textApprovedAmount.TabStop = false;
			// 
			// checkIsApproved
			// 
			this.checkIsApproved.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsApproved.Location = new System.Drawing.Point(13, 95);
			this.checkIsApproved.Name = "checkIsApproved";
			this.checkIsApproved.Size = new System.Drawing.Size(145, 24);
			this.checkIsApproved.TabIndex = 23;
			this.checkIsApproved.Text = "Has Customer Approval?";
			this.checkIsApproved.UseVisualStyleBackColor = true;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(144, 8);
			this.textPatient.MaxLength = 100;
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(183, 20);
			this.textPatient.TabIndex = 0;
			this.textPatient.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Patient";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPatPicker
			// 
			this.butPatPicker.Location = new System.Drawing.Point(328, 7);
			this.butPatPicker.Name = "butPatPicker";
			this.butPatPicker.Size = new System.Drawing.Size(20, 20);
			this.butPatPicker.TabIndex = 18;
			this.butPatPicker.Text = "...";
			this.butPatPicker.Click += new System.EventHandler(this.butPatPicker_Click);
			// 
			// FormJobQuoteEdit
			// 
			this.ClientSize = new System.Drawing.Size(413, 341);
			this.Controls.Add(this.checkIsApproved);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textApprovedAmount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textQuoteHours);
			this.Controls.Add(this.butPatPicker);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobQuoteEdit";
			this.Text = "Job Quote Edit";
			this.Load += new System.EventHandler(this.FormJobQuoteEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private UI.Button butDelete;
		private ODtextBox textNote;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textQuoteHours;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textApprovedAmount;
		private System.Windows.Forms.CheckBox checkIsApproved;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label label1;
		private UI.Button butPatPicker;
	}
}