namespace OpenDental{
	partial class FormTaskAttachmentEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskAttachmentEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelTaskNum = new System.Windows.Forms.Label();
			this.labelDocNum = new System.Windows.Forms.Label();
			this.labelDescription = new System.Windows.Forms.Label();
			this.butImport = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textDocNum = new System.Windows.Forms.TextBox();
			this.textTaskNum = new System.Windows.Forms.TextBox();
			this.textValue = new OpenDental.ODtextBox();
			this.labelText = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butViewDoc = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1062, 660);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1143, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelTaskNum
			// 
			this.labelTaskNum.Location = new System.Drawing.Point(21, 66);
			this.labelTaskNum.Name = "labelTaskNum";
			this.labelTaskNum.Size = new System.Drawing.Size(67, 19);
			this.labelTaskNum.TabIndex = 15;
			this.labelTaskNum.Text = "TaskNum";
			this.labelTaskNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDocNum
			// 
			this.labelDocNum.Location = new System.Drawing.Point(21, 40);
			this.labelDocNum.Name = "labelDocNum";
			this.labelDocNum.Size = new System.Drawing.Size(67, 19);
			this.labelDocNum.TabIndex = 17;
			this.labelDocNum.Text = "DocNum";
			this.labelDocNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(7, 13);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(83, 19);
			this.labelDescription.TabIndex = 58;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(172, 37);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(69, 24);
			this.butImport.TabIndex = 1;
			this.butImport.Text = "Import";
			this.butImport.UseVisualStyleBackColor = true;
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// textDescription
			// 
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.Location = new System.Drawing.Point(91, 12);
			this.textDescription.MaxLength = 255;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(382, 20);
			this.textDescription.TabIndex = 0;
			// 
			// textDocNum
			// 
			this.textDocNum.BackColor = System.Drawing.SystemColors.Window;
			this.textDocNum.Location = new System.Drawing.Point(91, 39);
			this.textDocNum.Name = "textDocNum";
			this.textDocNum.ReadOnly = true;
			this.textDocNum.Size = new System.Drawing.Size(75, 20);
			this.textDocNum.TabIndex = 61;
			this.textDocNum.TabStop = false;
			// 
			// textTaskNum
			// 
			this.textTaskNum.BackColor = System.Drawing.SystemColors.Window;
			this.textTaskNum.Location = new System.Drawing.Point(91, 65);
			this.textTaskNum.Name = "textTaskNum";
			this.textTaskNum.ReadOnly = true;
			this.textTaskNum.Size = new System.Drawing.Size(75, 20);
			this.textTaskNum.TabIndex = 60;
			this.textTaskNum.TabStop = false;
			// 
			// textValue
			// 
			this.textValue.AcceptsTab = true;
			this.textValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textValue.BackColor = System.Drawing.SystemColors.Window;
			this.textValue.DetectLinksEnabled = true;
			this.textValue.DetectUrls = true;
			this.textValue.HasAutoNotes = true;
			this.textValue.QuickPasteType = OpenDentBusiness.QuickPasteType.Task;
			this.textValue.Location = new System.Drawing.Point(12, 118);
			this.textValue.MaxLength = 65535;
			this.textValue.Multiline = true;
			this.textValue.Name = "textValue";
			this.textValue.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textValue.Size = new System.Drawing.Size(1174, 519);
			this.textValue.TabIndex = 3;
			this.textValue.Text = "";
			// 
			// labelText
			// 
			this.labelText.Location = new System.Drawing.Point(12, 96);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(66, 19);
			this.labelText.TabIndex = 63;
			this.labelText.Text = "Text";
			this.labelText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 660);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butViewDoc
			// 
			this.butViewDoc.Location = new System.Drawing.Point(250, 37);
			this.butViewDoc.Name = "butViewDoc";
			this.butViewDoc.Size = new System.Drawing.Size(69, 24);
			this.butViewDoc.TabIndex = 2;
			this.butViewDoc.Text = "View Doc";
			this.butViewDoc.UseVisualStyleBackColor = true;
			this.butViewDoc.Click += new System.EventHandler(this.butViewDoc_Click);
			// 
			// FormTaskAttachmentEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.butViewDoc);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.labelText);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textDocNum);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textTaskNum);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelTaskNum);
			this.Controls.Add(this.labelDocNum);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskAttachmentEdit";
			this.Text = "Edit Task Attachment";
			this.Load += new System.EventHandler(this.FormTaskAttachmentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelTaskNum;
		private System.Windows.Forms.Label labelDocNum;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.TextBox textDocNum;
		private System.Windows.Forms.TextBox textTaskNum;
		private ODtextBox textValue;
		private System.Windows.Forms.Label labelText;
		private UI.Button butImport;
		private UI.Button butDelete;
		private UI.Button butViewDoc;
	}
}