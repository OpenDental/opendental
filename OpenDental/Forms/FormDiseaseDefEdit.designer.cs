using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDiseaseDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiseaseDefEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textICD9 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textSnomed = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butSnomed = new OpenDental.UI.Button();
			this.butIcd9 = new OpenDental.UI.Button();
			this.butIcd10 = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textIcd10 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(372, 172);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(372, 136);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(118, 98);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(308, 20);
			this.textName.TabIndex = 0;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(30, 172);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 25);
			this.butDelete.TabIndex = 3;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(28, 132);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(104, 24);
			this.checkIsHidden.TabIndex = 1;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 5;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textICD9
			// 
			this.textICD9.Location = new System.Drawing.Point(118, 20);
			this.textICD9.Name = "textICD9";
			this.textICD9.ReadOnly = true;
			this.textICD9.Size = new System.Drawing.Size(273, 20);
			this.textICD9.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "ICD-9 Code";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSnomed
			// 
			this.textSnomed.Location = new System.Drawing.Point(118, 72);
			this.textSnomed.Name = "textSnomed";
			this.textSnomed.ReadOnly = true;
			this.textSnomed.Size = new System.Drawing.Size(273, 20);
			this.textSnomed.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(14, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "SNOMED CT Code";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSnomed
			// 
			this.butSnomed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSnomed.Location = new System.Drawing.Point(397, 70);
			this.butSnomed.Name = "butSnomed";
			this.butSnomed.Size = new System.Drawing.Size(29, 25);
			this.butSnomed.TabIndex = 10;
			this.butSnomed.Text = "...";
			this.butSnomed.Click += new System.EventHandler(this.butSnomed_Click);
			// 
			// butIcd9
			// 
			this.butIcd9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butIcd9.Location = new System.Drawing.Point(397, 17);
			this.butIcd9.Name = "butIcd9";
			this.butIcd9.Size = new System.Drawing.Size(29, 25);
			this.butIcd9.TabIndex = 6;
			this.butIcd9.Text = "...";
			this.butIcd9.Click += new System.EventHandler(this.butIcd9_Click);
			// 
			// butIcd10
			// 
			this.butIcd10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butIcd10.Location = new System.Drawing.Point(397, 43);
			this.butIcd10.Name = "butIcd10";
			this.butIcd10.Size = new System.Drawing.Size(29, 25);
			this.butIcd10.TabIndex = 8;
			this.butIcd10.Text = "...";
			this.butIcd10.Click += new System.EventHandler(this.butIcd10_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 44);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "ICD-10 Code";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textIcd10
			// 
			this.textIcd10.Location = new System.Drawing.Point(118, 46);
			this.textIcd10.Name = "textIcd10";
			this.textIcd10.ReadOnly = true;
			this.textIcd10.Size = new System.Drawing.Size(273, 20);
			this.textIcd10.TabIndex = 7;
			// 
			// FormDiseaseDefEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(460, 208);
			this.Controls.Add(this.butIcd10);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textIcd10);
			this.Controls.Add(this.butIcd9);
			this.Controls.Add(this.butSnomed);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textSnomed);
			this.Controls.Add(this.textICD9);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDiseaseDefEdit";
			this.ShowInTaskbar = false;
			this.Text = "Problem Def Edit";
			this.Load += new System.EventHandler(this.FormDiseaseDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textName;
		private OpenDental.UI.Button butDelete;
		private CheckBox checkIsHidden;
		private Label label1;
		private TextBox textICD9;
		private Label label2;
		private TextBox textSnomed;
		private Label label3;
		private UI.Button butSnomed;
		private UI.Button butIcd9;
		private UI.Button butIcd10;
		private Label label4;
		private TextBox textIcd10;
	}
}
