using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPopupEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPopupEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsDisabled = new System.Windows.Forms.CheckBox();
			this.comboPopupLevel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescription = new OpenDental.ODtextBox();
			this.textCreateDate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butAudit = new OpenDental.UI.Button();
			this.textEditDate = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.labelNoPerms = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 160);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 19);
			this.label1.TabIndex = 3;
			this.label1.Text = "Popup Message";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsDisabled
			// 
			this.checkIsDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDisabled.Location = new System.Drawing.Point(6, 137);
			this.checkIsDisabled.Name = "checkIsDisabled";
			this.checkIsDisabled.Size = new System.Drawing.Size(158, 18);
			this.checkIsDisabled.TabIndex = 4;
			this.checkIsDisabled.Text = "Permanently Disabled";
			this.checkIsDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDisabled.UseVisualStyleBackColor = true;
			// 
			// comboPopupLevel
			// 
			this.comboPopupLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPopupLevel.FormattingEnabled = true;
			this.comboPopupLevel.Location = new System.Drawing.Point(149, 110);
			this.comboPopupLevel.Name = "comboPopupLevel";
			this.comboPopupLevel.Size = new System.Drawing.Size(159, 21);
			this.comboPopupLevel.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(28, 110);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Level";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(142, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "Patient";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Enabled = false;
			this.textPatient.Location = new System.Drawing.Point(149, 84);
			this.textPatient.Name = "textPatient";
			this.textPatient.Size = new System.Drawing.Size(271, 20);
			this.textPatient.TabIndex = 9;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(28, 274);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 26);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(302, 274);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(77, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(385, 274);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.DetectLinksEnabled = false;
			this.textDescription.DetectUrls = false;
			this.textDescription.Location = new System.Drawing.Point(149, 160);
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.QuickPasteType.Popup;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(271, 91);
			this.textDescription.TabIndex = 1;
			this.textDescription.Text = "";
			// 
			// textCreateDate
			// 
			this.textCreateDate.Enabled = false;
			this.textCreateDate.Location = new System.Drawing.Point(149, 33);
			this.textCreateDate.Name = "textCreateDate";
			this.textCreateDate.ReadOnly = true;
			this.textCreateDate.Size = new System.Drawing.Size(159, 20);
			this.textCreateDate.TabIndex = 16;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(15, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(134, 15);
			this.label5.TabIndex = 15;
			this.label5.Text = "Date Created";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Enabled = false;
			this.textUser.Location = new System.Drawing.Point(149, 7);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(159, 20);
			this.textUser.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(18, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 15);
			this.label4.TabIndex = 13;
			this.label4.Text = "User";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAudit
			// 
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Location = new System.Drawing.Point(149, 274);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(77, 26);
			this.butAudit.TabIndex = 17;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// textEditDate
			// 
			this.textEditDate.Enabled = false;
			this.textEditDate.Location = new System.Drawing.Point(149, 59);
			this.textEditDate.Name = "textEditDate";
			this.textEditDate.ReadOnly = true;
			this.textEditDate.Size = new System.Drawing.Size(159, 20);
			this.textEditDate.TabIndex = 18;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(15, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(134, 15);
			this.label6.TabIndex = 19;
			this.label6.Text = "Date Edited";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNoPerms
			// 
			this.labelNoPerms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNoPerms.Location = new System.Drawing.Point(144, 255);
			this.labelNoPerms.Name = "labelNoPerms";
			this.labelNoPerms.Size = new System.Drawing.Size(311, 16);
			this.labelNoPerms.TabIndex = 20;
			this.labelNoPerms.Text = "Some fields disabled due to lack of Edit Popup permission.";
			this.labelNoPerms.Visible = false;
			// 
			// FormPopupEdit
			// 
			this.ClientSize = new System.Drawing.Size(492, 314);
			this.Controls.Add(this.labelNoPerms);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textEditDate);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.textCreateDate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboPopupLevel);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkIsDisabled);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPopupEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Popup";
			this.Load += new System.EventHandler(this.FormPopupEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private CheckBox checkIsDisabled;
		private OpenDental.UI.Button butDelete;
		private ComboBox comboPopupLevel;
		private Label label2;
		private Label label3;
		private TextBox textPatient;
		private ODtextBox textDescription;
		private TextBox textCreateDate;
		private Label label5;
		private TextBox textUser;
		private Label label4;
		private UI.Button butAudit;
		private TextBox textEditDate;
		private Label label6;
		private Label labelNoPerms;
	}
}
