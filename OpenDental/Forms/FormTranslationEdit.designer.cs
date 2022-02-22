using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTranslationEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTranslationEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textEnglish = new System.Windows.Forms.TextBox();
			this.textTranslation = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textComments = new System.Windows.Forms.TextBox();
			this.textOtherTranslation = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(786,594);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(786,628);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(43,36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82,23);
			this.label1.TabIndex = 2;
			this.label1.Text = "English";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(38,168);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88,16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Translation";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEnglish
			// 
			this.textEnglish.AcceptsReturn = true;
			this.textEnglish.Location = new System.Drawing.Point(127,34);
			this.textEnglish.Multiline = true;
			this.textEnglish.Name = "textEnglish";
			this.textEnglish.ReadOnly = true;
			this.textEnglish.Size = new System.Drawing.Size(672,130);
			this.textEnglish.TabIndex = 6;
			// 
			// textTranslation
			// 
			this.textTranslation.AcceptsReturn = true;
			this.textTranslation.Location = new System.Drawing.Point(127,166);
			this.textTranslation.Multiline = true;
			this.textTranslation.Name = "textTranslation";
			this.textTranslation.Size = new System.Drawing.Size(672,130);
			this.textTranslation.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(44,434);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82,14);
			this.label4.TabIndex = 10;
			this.label4.Text = "Comments";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textComments
			// 
			this.textComments.AcceptsReturn = true;
			this.textComments.Location = new System.Drawing.Point(127,432);
			this.textComments.Multiline = true;
			this.textComments.Name = "textComments";
			this.textComments.Size = new System.Drawing.Size(672,130);
			this.textComments.TabIndex = 11;
			// 
			// textOtherTranslation
			// 
			this.textOtherTranslation.Location = new System.Drawing.Point(127,299);
			this.textOtherTranslation.Multiline = true;
			this.textOtherTranslation.Name = "textOtherTranslation";
			this.textOtherTranslation.ReadOnly = true;
			this.textOtherTranslation.Size = new System.Drawing.Size(671,130);
			this.textOtherTranslation.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4,301);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(121,16);
			this.label3.TabIndex = 12;
			this.label3.Text = "Other Translation";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormTranslationEdit
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(880,668);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textOtherTranslation);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textComments);
			this.Controls.Add(this.textTranslation);
			this.Controls.Add(this.textEnglish);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTranslationEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Translation Edit";
			this.Load += new System.EventHandler(this.FormTranslationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textEnglish;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textComments;
		private System.Windows.Forms.TextBox textTranslation;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textOtherTranslation;
	}
}
