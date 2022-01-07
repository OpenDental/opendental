using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEmailMessageEdit {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailMessageEdit));
			this.labelTemplate = new System.Windows.Forms.Label();
			this.listTemplates = new System.Windows.Forms.ListBox();
			this.panelTemplates = new System.Windows.Forms.Panel();
			this.butEditTemplate = new OpenDental.UI.Button();
			this.butInsertTemplate = new OpenDental.UI.Button();
			this.butDeleteTemplate = new OpenDental.UI.Button();
			this.butAddTemplate = new OpenDental.UI.Button();
			this.labelDecrypt = new System.Windows.Forms.Label();
			this.panelAutographs = new System.Windows.Forms.Panel();
			this.butEditAutograph = new OpenDental.UI.Button();
			this.butInsertAutograph = new OpenDental.UI.Button();
			this.butDeleteAutograph = new OpenDental.UI.Button();
			this.butAddAutograph = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listAutographs = new OpenDental.UI.ListBoxOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.butRawMessage = new OpenDental.UI.Button();
			this.butDecrypt = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.emailPreview = new OpenDental.EmailPreviewControl();
			this.toolTipMessage = new System.Windows.Forms.ToolTip(this.components);
			this.butEditHtml = new OpenDental.UI.Button();
			this.butEditText = new OpenDental.UI.Button();
			this.toolBarSend = new OpenDental.UI.ToolBarOD();
			this.panelTemplates.SuspendLayout();
			this.panelAutographs.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTemplate
			// 
			this.labelTemplate.Location = new System.Drawing.Point(8, 7);
			this.labelTemplate.Name = "labelTemplate";
			this.labelTemplate.Size = new System.Drawing.Size(166, 14);
			this.labelTemplate.TabIndex = 18;
			this.labelTemplate.Text = "E-mail Template";
			this.labelTemplate.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listTemplates
			// 
			this.listTemplates.HorizontalScrollbar = true;
			this.listTemplates.Location = new System.Drawing.Point(10, 26);
			this.listTemplates.Name = "listTemplates";
			this.listTemplates.Size = new System.Drawing.Size(164, 173);
			this.listTemplates.TabIndex = 0;
			this.listTemplates.TabStop = false;
			this.listTemplates.DoubleClick += new System.EventHandler(this.listTemplates_DoubleClick);
			this.listTemplates.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listTemplates_MouseMove);
			// 
			// panelTemplates
			// 
			this.panelTemplates.Controls.Add(this.butEditTemplate);
			this.panelTemplates.Controls.Add(this.butInsertTemplate);
			this.panelTemplates.Controls.Add(this.butDeleteTemplate);
			this.panelTemplates.Controls.Add(this.butAddTemplate);
			this.panelTemplates.Controls.Add(this.labelTemplate);
			this.panelTemplates.Controls.Add(this.listTemplates);
			this.panelTemplates.Location = new System.Drawing.Point(8, 9);
			this.panelTemplates.Name = "panelTemplates";
			this.panelTemplates.Size = new System.Drawing.Size(180, 268);
			this.panelTemplates.TabIndex = 0;
			// 
			// butEditTemplate
			// 
			this.butEditTemplate.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditTemplate.Location = new System.Drawing.Point(102, 236);
			this.butEditTemplate.Name = "butEditTemplate";
			this.butEditTemplate.Size = new System.Drawing.Size(75, 26);
			this.butEditTemplate.TabIndex = 19;
			this.butEditTemplate.Text = "Edit";
			this.butEditTemplate.Click += new System.EventHandler(this.butEditTemplate_Click);
			// 
			// butInsertTemplate
			// 
			this.butInsertTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butInsertTemplate.Image = global::OpenDental.Properties.Resources.Right;
			this.butInsertTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butInsertTemplate.Location = new System.Drawing.Point(102, 202);
			this.butInsertTemplate.Name = "butInsertTemplate";
			this.butInsertTemplate.Size = new System.Drawing.Size(74, 26);
			this.butInsertTemplate.TabIndex = 2;
			this.butInsertTemplate.Text = "Insert";
			this.butInsertTemplate.Click += new System.EventHandler(this.butInsertTemplate_Click);
			// 
			// butDeleteTemplate
			// 
			this.butDeleteTemplate.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteTemplate.Location = new System.Drawing.Point(7, 236);
			this.butDeleteTemplate.Name = "butDeleteTemplate";
			this.butDeleteTemplate.Size = new System.Drawing.Size(75, 26);
			this.butDeleteTemplate.TabIndex = 3;
			this.butDeleteTemplate.Text = "Delete";
			this.butDeleteTemplate.Click += new System.EventHandler(this.butDeleteTemplate_Click);
			// 
			// butAddTemplate
			// 
			this.butAddTemplate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTemplate.Location = new System.Drawing.Point(7, 202);
			this.butAddTemplate.Name = "butAddTemplate";
			this.butAddTemplate.Size = new System.Drawing.Size(75, 26);
			this.butAddTemplate.TabIndex = 1;
			this.butAddTemplate.Text = "&Add";
			this.butAddTemplate.Click += new System.EventHandler(this.butAddTemplate_Click);
			// 
			// labelDecrypt
			// 
			this.labelDecrypt.Location = new System.Drawing.Point(5, 548);
			this.labelDecrypt.Name = "labelDecrypt";
			this.labelDecrypt.Size = new System.Drawing.Size(267, 59);
			this.labelDecrypt.TabIndex = 31;
			this.labelDecrypt.Text = "Previous attempts to decrypt this message have failed.\r\nDecryption usually fails " +
    "when your private decryption key is not installed on the local computer.\r\nUse th" +
    "e Decrypt button to try again.";
			this.labelDecrypt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelDecrypt.Visible = false;
			// 
			// panelAutographs
			// 
			this.panelAutographs.Controls.Add(this.butEditAutograph);
			this.panelAutographs.Controls.Add(this.butInsertAutograph);
			this.panelAutographs.Controls.Add(this.butDeleteAutograph);
			this.panelAutographs.Controls.Add(this.butAddAutograph);
			this.panelAutographs.Controls.Add(this.label2);
			this.panelAutographs.Controls.Add(this.listAutographs);
			this.panelAutographs.Location = new System.Drawing.Point(8, 279);
			this.panelAutographs.Name = "panelAutographs";
			this.panelAutographs.Size = new System.Drawing.Size(180, 268);
			this.panelAutographs.TabIndex = 19;
			// 
			// butEditAutograph
			// 
			this.butEditAutograph.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditAutograph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditAutograph.Location = new System.Drawing.Point(101, 236);
			this.butEditAutograph.Name = "butEditAutograph";
			this.butEditAutograph.Size = new System.Drawing.Size(75, 26);
			this.butEditAutograph.TabIndex = 20;
			this.butEditAutograph.Text = "Edit";
			this.butEditAutograph.Click += new System.EventHandler(this.butEditAutograph_Click);
			// 
			// butInsertAutograph
			// 
			this.butInsertAutograph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butInsertAutograph.Image = global::OpenDental.Properties.Resources.Right;
			this.butInsertAutograph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butInsertAutograph.Location = new System.Drawing.Point(102, 202);
			this.butInsertAutograph.Name = "butInsertAutograph";
			this.butInsertAutograph.Size = new System.Drawing.Size(74, 26);
			this.butInsertAutograph.TabIndex = 2;
			this.butInsertAutograph.Text = "Insert";
			this.butInsertAutograph.Click += new System.EventHandler(this.butInsertAutograph_Click);
			// 
			// butDeleteAutograph
			// 
			this.butDeleteAutograph.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteAutograph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteAutograph.Location = new System.Drawing.Point(7, 236);
			this.butDeleteAutograph.Name = "butDeleteAutograph";
			this.butDeleteAutograph.Size = new System.Drawing.Size(75, 26);
			this.butDeleteAutograph.TabIndex = 3;
			this.butDeleteAutograph.Text = "Delete";
			this.butDeleteAutograph.Click += new System.EventHandler(this.butDeleteAutograph_Click);
			// 
			// butAddAutograph
			// 
			this.butAddAutograph.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAutograph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAutograph.Location = new System.Drawing.Point(7, 202);
			this.butAddAutograph.Name = "butAddAutograph";
			this.butAddAutograph.Size = new System.Drawing.Size(75, 26);
			this.butAddAutograph.TabIndex = 1;
			this.butAddAutograph.Text = "&Add";
			this.butAddAutograph.Click += new System.EventHandler(this.butAddAutograph_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(166, 14);
			this.label2.TabIndex = 18;
			this.label2.Text = "E-mail Autograph";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listAutographs
			// 
			this.listAutographs.Location = new System.Drawing.Point(10, 26);
			this.listAutographs.Name = "listAutographs";
			this.listAutographs.Size = new System.Drawing.Size(164, 173);
			this.listAutographs.TabIndex = 0;
			this.listAutographs.TabStop = false;
			this.listAutographs.DoubleClick += new System.EventHandler(this.listAutographs_DoubleClick);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRefresh.Location = new System.Drawing.Point(358, 660);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 25);
			this.butRefresh.TabIndex = 37;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butRawMessage
			// 
			this.butRawMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRawMessage.Location = new System.Drawing.Point(138, 659);
			this.butRawMessage.Name = "butRawMessage";
			this.butRawMessage.Size = new System.Drawing.Size(89, 26);
			this.butRawMessage.TabIndex = 36;
			this.butRawMessage.Text = "Raw Message";
			this.butRawMessage.Visible = false;
			this.butRawMessage.Click += new System.EventHandler(this.butRawMessage_Click);
			// 
			// butDecrypt
			// 
			this.butDecrypt.Location = new System.Drawing.Point(8, 610);
			this.butDecrypt.Name = "butDecrypt";
			this.butDecrypt.Size = new System.Drawing.Size(75, 25);
			this.butDecrypt.TabIndex = 7;
			this.butDecrypt.Text = "Decrypt";
			this.butDecrypt.Visible = false;
			this.butDecrypt.Click += new System.EventHandler(this.butDecrypt_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(8, 659);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSave.Location = new System.Drawing.Point(278, 660);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 25);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// emailPreview
			// 
			this.emailPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.emailPreview.BccAddress = "";
			this.emailPreview.BodyText = "";
			this.emailPreview.CcAddress = "";
			this.emailPreview.IsPreview = false;
			this.emailPreview.Location = new System.Drawing.Point(189, 11);
			this.emailPreview.Name = "emailPreview";
			this.emailPreview.Size = new System.Drawing.Size(771, 642);
			this.emailPreview.Subject = "";
			this.emailPreview.TabIndex = 38;
			this.emailPreview.ToAddress = "";
			// 
			// toolTipMessage
			// 
			this.toolTipMessage.AutoPopDelay = 0;
			this.toolTipMessage.InitialDelay = 10;
			this.toolTipMessage.ReshowDelay = 100;
			// 
			// butEditHtml
			// 
			this.butEditHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditHtml.Location = new System.Drawing.Point(520, 660);
			this.butEditHtml.Name = "butEditHtml";
			this.butEditHtml.Size = new System.Drawing.Size(75, 25);
			this.butEditHtml.TabIndex = 39;
			this.butEditHtml.Text = "Edit HTML";
			this.butEditHtml.Click += new System.EventHandler(this.butEditHtml_Click);
			// 
			// butEditText
			// 
			this.butEditText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditText.Location = new System.Drawing.Point(439, 660);
			this.butEditText.Name = "butEditText";
			this.butEditText.Size = new System.Drawing.Size(75, 25);
			this.butEditText.TabIndex = 40;
			this.butEditText.Text = "Edit Text";
			this.butEditText.Click += new System.EventHandler(this.butEditText_Click);
			// 
			// toolBarSend
			// 
			this.toolBarSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.toolBarSend.Location = new System.Drawing.Point(846, 660);
			this.toolBarSend.Name = "toolBarSend";
			this.toolBarSend.Size = new System.Drawing.Size(113, 25);
			this.toolBarSend.TabIndex = 41;
			this.toolBarSend.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarSend_ButtonClick);
			// 
			// FormEmailMessageEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.butEditText);
			this.Controls.Add(this.butEditHtml);
			this.Controls.Add(this.toolBarSend);
			this.Controls.Add(this.panelAutographs);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butRawMessage);
			this.Controls.Add(this.butDecrypt);
			this.Controls.Add(this.labelDecrypt);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.panelTemplates);
			this.Controls.Add(this.emailPreview);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailMessageEdit";
			this.Text = "Edit Email Message";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEmailMessageEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormEmailMessageEdit_Load);
			this.panelTemplates.ResumeLayout(false);
			this.panelAutographs.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butDeleteTemplate;
		private OpenDental.UI.Button butAddTemplate;
		private System.Windows.Forms.Label labelTemplate;
		private System.Windows.Forms.ListBox listTemplates;
		private OpenDental.UI.Button butInsertTemplate;
		private UI.Button butRefresh;
		private System.Windows.Forms.Panel panelTemplates;
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.Button butDelete;
		private Label labelDecrypt;
		private UI.Button butDecrypt;
		private UI.Button butRawMessage;
		private Panel panelAutographs;
		private UI.Button butInsertAutograph;
		private UI.Button butDeleteAutograph;
		private UI.Button butAddAutograph;
		private Label label2;
		private UI.ListBoxOD listAutographs;
		private UI.Button butEditTemplate;
		private UI.Button butEditAutograph;
		private UI.Button butEditHtml;
		private ToolTip toolTipMessage;
		private UI.Button butEditText;
		private EmailPreviewControl emailPreview;
		private UI.ToolBarOD toolBarSend;
	}
}
