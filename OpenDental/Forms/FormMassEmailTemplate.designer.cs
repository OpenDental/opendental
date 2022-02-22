namespace OpenDental{
	partial class FormMassEmailTemplate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailTemplate));
			this.butSubjectFields = new OpenDental.UI.Button();
			this.textSubject = new OpenDental.ODtextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textTemplateName = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butBodyFieldsPlainText = new OpenDental.UI.Button();
			this.textboxPlainText = new OpenDental.ODtextBox();
			this.butEditTemplate = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDeleteTemplate = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butSubjectFields
			// 
			this.butSubjectFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSubjectFields.Location = new System.Drawing.Point(765, 71);
			this.butSubjectFields.Name = "butSubjectFields";
			this.butSubjectFields.Size = new System.Drawing.Size(98, 20);
			this.butSubjectFields.TabIndex = 6;
			this.butSubjectFields.Text = "Subject Fields";
			this.butSubjectFields.Click += new System.EventHandler(this.butSubjectFields_Click);
			// 
			// textSubject
			// 
			this.textSubject.AcceptsTab = true;
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.BackColor = System.Drawing.SystemColors.Window;
			this.textSubject.DetectLinksEnabled = false;
			this.textSubject.DetectUrls = false;
			this.textSubject.Location = new System.Drawing.Point(153, 73);
			this.textSubject.Multiline = false;
			this.textSubject.Name = "textSubject";
			this.textSubject.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSubject.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSubject.Size = new System.Drawing.Size(606, 20);
			this.textSubject.TabIndex = 1;
			this.textSubject.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(42, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 18);
			this.label2.TabIndex = 9;
			this.label2.Text = "Subject";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTemplateName
			// 
			this.textTemplateName.AcceptsTab = true;
			this.textTemplateName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTemplateName.BackColor = System.Drawing.SystemColors.Window;
			this.textTemplateName.DetectLinksEnabled = false;
			this.textTemplateName.DetectUrls = false;
			this.textTemplateName.Location = new System.Drawing.Point(153, 37);
			this.textTemplateName.Multiline = false;
			this.textTemplateName.Name = "textTemplateName";
			this.textTemplateName.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textTemplateName.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTemplateName.Size = new System.Drawing.Size(606, 20);
			this.textTemplateName.TabIndex = 0;
			this.textTemplateName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 18);
			this.label1.TabIndex = 8;
			this.label1.Text = "Template Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(19, 113);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 18);
			this.label6.TabIndex = 10;
			this.label6.Text = "Plain Text Body";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBodyFieldsPlainText
			// 
			this.butBodyFieldsPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBodyFieldsPlainText.Location = new System.Drawing.Point(765, 113);
			this.butBodyFieldsPlainText.Name = "butBodyFieldsPlainText";
			this.butBodyFieldsPlainText.Size = new System.Drawing.Size(101, 20);
			this.butBodyFieldsPlainText.TabIndex = 7;
			this.butBodyFieldsPlainText.Text = "Body Fields";
			this.butBodyFieldsPlainText.Click += new System.EventHandler(this.butBodyFieldsPlainText_Click);
			// 
			// textboxPlainText
			// 
			this.textboxPlainText.AcceptsTab = true;
			this.textboxPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textboxPlainText.BackColor = System.Drawing.SystemColors.Window;
			this.textboxPlainText.DetectLinksEnabled = false;
			this.textboxPlainText.DetectUrls = false;
			this.textboxPlainText.Location = new System.Drawing.Point(150, 113);
			this.textboxPlainText.Name = "textboxPlainText";
			this.textboxPlainText.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textboxPlainText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textboxPlainText.Size = new System.Drawing.Size(609, 334);
			this.textboxPlainText.TabIndex = 2;
			this.textboxPlainText.Text = "";
			// 
			// butEditTemplate
			// 
			this.butEditTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditTemplate.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditTemplate.Location = new System.Drawing.Point(545, 489);
			this.butEditTemplate.Name = "butEditTemplate";
			this.butEditTemplate.Size = new System.Drawing.Size(132, 26);
			this.butEditTemplate.TabIndex = 3;
			this.butEditTemplate.Text = "Edit HTML Body";
			this.butEditTemplate.Click += new System.EventHandler(this.butEditTemplate_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butOk.Location = new System.Drawing.Point(721, 489);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 26);
			this.butOk.TabIndex = 4;
			this.butOk.Text = "OK";
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCancel.Location = new System.Drawing.Point(806, 489);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDeleteTemplate
			// 
			this.butDeleteTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteTemplate.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteTemplate.Location = new System.Drawing.Point(12, 489);
			this.butDeleteTemplate.Name = "butDeleteTemplate";
			this.butDeleteTemplate.Size = new System.Drawing.Size(75, 26);
			this.butDeleteTemplate.TabIndex = 133;
			this.butDeleteTemplate.Text = "Delete";
			this.butDeleteTemplate.Click += new System.EventHandler(this.butDeleteTemplate_Click);
			// 
			// FormMassEmailTemplate
			// 
			this.ClientSize = new System.Drawing.Size(893, 527);
			this.Controls.Add(this.butDeleteTemplate);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butEditTemplate);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butBodyFieldsPlainText);
			this.Controls.Add(this.textboxPlainText);
			this.Controls.Add(this.textTemplateName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSubjectFields);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailTemplate";
			this.Text = "Mass Email Template";
			this.Load += new System.EventHandler(this.FormMassEmailTemplate_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butSubjectFields;
		private ODtextBox textSubject;
		private System.Windows.Forms.Label label2;
		private ODtextBox textTemplateName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label6;
		private UI.Button butBodyFieldsPlainText;
		private ODtextBox textboxPlainText;
		private UI.Button butEditTemplate;
		private UI.Button butOk;
		private UI.Button butCancel;
		private UI.Button butDeleteTemplate;
	}
}