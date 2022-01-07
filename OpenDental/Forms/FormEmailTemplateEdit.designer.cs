using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEmailTemplateEdit {
		/// <summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailTemplateEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBodyText = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butBodyFields = new OpenDental.UI.Button();
			this.textSubject = new OpenDental.ODtextBox();
			this.textDescription = new OpenDental.ODtextBox();
			this.butSubjectFields = new OpenDental.UI.Button();
			this.butAttach = new OpenDental.UI.Button();
			this.gridAttachments = new OpenDental.UI.GridOD();
			this.contextMenuAttachments = new System.Windows.Forms.ContextMenu();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemRename = new System.Windows.Forms.MenuItem();
			this.menuItemRemove = new System.Windows.Forms.MenuItem();
			this.butEditHtml = new OpenDental.UI.Button();
			this.butEditText = new OpenDental.UI.Button();
			this.webBrowserHtml = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(883, 656);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(802, 656);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "Subject";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBodyText
			// 
			this.textBodyText.AcceptsTab = true;
			this.textBodyText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBodyText.BackColor = System.Drawing.SystemColors.Window;
			this.textBodyText.DetectLinksEnabled = false;
			this.textBodyText.DetectUrls = false;
			this.textBodyText.Location = new System.Drawing.Point(97, 86);
			this.textBodyText.Name = "textBodyText";
			this.textBodyText.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textBodyText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBodyText.Size = new System.Drawing.Size(861, 564);
			this.textBodyText.TabIndex = 3;
			this.textBodyText.Text = "";
			this.textBodyText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBodyText_KeyDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Body";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 20);
			this.label3.TabIndex = 0;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBodyFields
			// 
			this.butBodyFields.Location = new System.Drawing.Point(182, 23);
			this.butBodyFields.Name = "butBodyFields";
			this.butBodyFields.Size = new System.Drawing.Size(82, 20);
			this.butBodyFields.TabIndex = 4;
			this.butBodyFields.Text = "Body Fields";
			this.butBodyFields.Click += new System.EventHandler(this.butBodyFields_Click);
			// 
			// textSubject
			// 
			this.textSubject.AcceptsTab = true;
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.BackColor = System.Drawing.SystemColors.Window;
			this.textSubject.DetectLinksEnabled = false;
			this.textSubject.DetectUrls = false;
			this.textSubject.Location = new System.Drawing.Point(97, 65);
			this.textSubject.Multiline = false;
			this.textSubject.Name = "textSubject";
			this.textSubject.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textSubject.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSubject.Size = new System.Drawing.Size(635, 20);
			this.textSubject.TabIndex = 2;
			this.textSubject.Text = "";
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.DetectLinksEnabled = false;
			this.textDescription.DetectUrls = false;
			this.textDescription.Location = new System.Drawing.Point(97, 44);
			this.textDescription.Multiline = false;
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(635, 20);
			this.textDescription.TabIndex = 1;
			this.textDescription.Text = "";
			// 
			// butSubjectFields
			// 
			this.butSubjectFields.Location = new System.Drawing.Point(97, 23);
			this.butSubjectFields.Name = "butSubjectFields";
			this.butSubjectFields.Size = new System.Drawing.Size(82, 20);
			this.butSubjectFields.TabIndex = 7;
			this.butSubjectFields.Text = "Subject Fields";
			this.butSubjectFields.Click += new System.EventHandler(this.butSubjectFields_Click);
			// 
			// butAttach
			// 
			this.butAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAttach.Location = new System.Drawing.Point(738, 2);
			this.butAttach.Name = "butAttach";
			this.butAttach.Size = new System.Drawing.Size(82, 20);
			this.butAttach.TabIndex = 9;
			this.butAttach.Text = "Attach...";
			this.butAttach.Click += new System.EventHandler(this.butAttach_Click);
			// 
			// gridAttachments
			// 
			this.gridAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAttachments.Location = new System.Drawing.Point(738, 23);
			this.gridAttachments.Name = "gridAttachments";
			this.gridAttachments.Size = new System.Drawing.Size(220, 62);
			this.gridAttachments.TabIndex = 8;
			this.gridAttachments.TabStop = false;
			this.gridAttachments.Title = "Attachments";
			this.gridAttachments.TranslationName = "TableAttachments";
			this.gridAttachments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAttachments_CellDoubleClick);
			this.gridAttachments.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridAttachments_MouseDown);
			// 
			// contextMenuAttachments
			// 
			this.contextMenuAttachments.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOpen,
            this.menuItemRename,
            this.menuItemRemove});
			this.contextMenuAttachments.Popup += new System.EventHandler(this.contextMenuAttachments_Popup);
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.Text = "Open";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemRename
			// 
			this.menuItemRename.Index = 1;
			this.menuItemRename.Text = "Rename";
			this.menuItemRename.Click += new System.EventHandler(this.menuItemRename_Click);
			// 
			// menuItemRemove
			// 
			this.menuItemRemove.Index = 2;
			this.menuItemRemove.Text = "Remove";
			this.menuItemRemove.Click += new System.EventHandler(this.menuItemRemove_Click);
			// 
			// butEditHtml
			// 
			this.butEditHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditHtml.Location = new System.Drawing.Point(178, 656);
			this.butEditHtml.Name = "butEditHtml";
			this.butEditHtml.Size = new System.Drawing.Size(75, 25);
			this.butEditHtml.TabIndex = 40;
			this.butEditHtml.Text = "Edit HTML";
			this.butEditHtml.Click += new System.EventHandler(this.butEditHtml_Click);
			// 
			// butEditText
			// 
			this.butEditText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditText.Location = new System.Drawing.Point(97, 656);
			this.butEditText.Name = "butEditText";
			this.butEditText.Size = new System.Drawing.Size(75, 25);
			this.butEditText.TabIndex = 41;
			this.butEditText.Text = "Edit Text";
			this.butEditText.Click += new System.EventHandler(this.butEditText_Click);
			// 
			// webBrowserHtml
			// 
			this.webBrowserHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserHtml.Location = new System.Drawing.Point(97, 86);
			this.webBrowserHtml.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserHtml.Name = "webBrowserHtml";
			this.webBrowserHtml.Size = new System.Drawing.Size(861, 564);
			this.webBrowserHtml.TabIndex = 42;
			this.webBrowserHtml.Visible = false;
			this.webBrowserHtml.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserHtml_Navigated);
			this.webBrowserHtml.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserHtml_Navigating);
			// 
			// FormEmailTemplateEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 695);
			this.Controls.Add(this.butEditText);
			this.Controls.Add(this.butEditHtml);
			this.Controls.Add(this.butAttach);
			this.Controls.Add(this.gridAttachments);
			this.Controls.Add(this.butSubjectFields);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.butBodyFields);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBodyText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.webBrowserHtml);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailTemplateEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Email Template";
			this.Load += new System.EventHandler(this.FormEmailTemplateEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private OpenDental.ODtextBox textBodyText;
		private Label label1;
		private Label label3;
		private UI.Button butBodyFields;
		private ODtextBox textSubject;
		private ODtextBox textDescription;
		private UI.Button butAttach;
		private UI.GridOD gridAttachments;
		private UI.Button butSubjectFields;
		private ContextMenu contextMenuAttachments;
		private MenuItem menuItemOpen;
		private MenuItem menuItemRename;
		private MenuItem menuItemRemove;
		private UI.Button butEditHtml;
		private UI.Button butEditText;
		private WebBrowser webBrowserHtml;
	}
}
