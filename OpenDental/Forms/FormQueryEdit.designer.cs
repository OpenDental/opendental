using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormQueryEdit {
		private System.ComponentModel.IContainer components = null;
	
		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQueryEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textFileName = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textQuery = new OpenDental.ODtextBox();
			this.checkReleased = new System.Windows.Forms.CheckBox();
			this.checkIsPromptSetup = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Title";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textTitle
			// 
			this.textTitle.Location = new System.Drawing.Point(16, 18);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(315, 20);
			this.textTitle.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Query Text";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(13, 620);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Export File Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textFileName
			// 
			this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textFileName.Location = new System.Drawing.Point(16, 637);
			this.textFileName.Name = "textFileName";
			this.textFileName.Size = new System.Drawing.Size(326, 20);
			this.textFileName.TabIndex = 2;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(509, 633);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(594, 633);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textQuery
			// 
			this.textQuery.AcceptsTab = true;
			this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textQuery.BackColor = System.Drawing.SystemColors.Window;
			this.textQuery.DetectLinksEnabled = false;
			this.textQuery.DetectUrls = false;
			this.textQuery.Location = new System.Drawing.Point(16, 82);
			this.textQuery.Name = "textQuery";
			this.textQuery.QuickPasteType = OpenDentBusiness.QuickPasteType.Query;
			this.textQuery.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textQuery.Size = new System.Drawing.Size(653, 536);
			this.textQuery.SpellCheckIsEnabled = false;
			this.textQuery.TabIndex = 5;
			this.textQuery.Text = "";
			// 
			// checkReleased
			// 
			this.checkReleased.Location = new System.Drawing.Point(16, 41);
			this.checkReleased.Name = "checkReleased";
			this.checkReleased.Size = new System.Drawing.Size(203, 21);
			this.checkReleased.TabIndex = 6;
			this.checkReleased.Text = "Released";
			this.checkReleased.UseVisualStyleBackColor = true;
			// 
			// checkIsPromptSetup
			// 
			this.checkIsPromptSetup.Location = new System.Drawing.Point(219, 44);
			this.checkIsPromptSetup.Name = "checkIsPromptSetup";
			this.checkIsPromptSetup.Size = new System.Drawing.Size(239, 21);
			this.checkIsPromptSetup.TabIndex = 7;
			this.checkIsPromptSetup.Text = "Prompt for SET statements";
			this.checkIsPromptSetup.UseVisualStyleBackColor = true;
			// 
			// FormQueryEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(682, 670);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkIsPromptSetup);
			this.Controls.Add(this.checkReleased);
			this.Controls.Add(this.textQuery);
			this.Controls.Add(this.textFileName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTitle);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormQueryEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Favorite";
			this.Load += new System.EventHandler(this.FormQueryEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textFileName;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.ODtextBox textQuery;// Required designer variable.
		private CheckBox checkReleased;
		private CheckBox checkIsPromptSetup;
	}
}
