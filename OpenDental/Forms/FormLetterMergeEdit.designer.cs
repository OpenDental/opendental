using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLetterMergeEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLetterMergeEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textTemplateName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textDataFileName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboCategory = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butEditPaths = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.labelPatient = new System.Windows.Forms.Label();
			this.listPatSelect = new OpenDental.UI.ListBoxOD();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butBrowse = new OpenDental.UI.Button();
			this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
			this.labelReferredFrom = new System.Windows.Forms.Label();
			this.listReferral = new OpenDental.UI.ListBoxOD();
			this.butNew = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.listOther = new OpenDental.UI.ListBoxOD();
			this.labelImageCategory = new System.Windows.Forms.Label();
			this.comboImageFolder = new OpenDental.UI.ComboBoxOD();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(799, 649);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(799, 608);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(20, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(250, 14);
			this.label2.TabIndex = 3;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(272, 7);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(221, 20);
			this.textDescription.TabIndex = 0;
			// 
			// textTemplateName
			// 
			this.textTemplateName.Location = new System.Drawing.Point(272, 54);
			this.textTemplateName.Name = "textTemplateName";
			this.textTemplateName.Size = new System.Drawing.Size(346, 20);
			this.textTemplateName.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(20, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(250, 14);
			this.label1.TabIndex = 5;
			this.label1.Text = "Template File Name (eg MyTemplate.doc)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDataFileName
			// 
			this.textDataFileName.Location = new System.Drawing.Point(272, 77);
			this.textDataFileName.Name = "textDataFileName";
			this.textDataFileName.Size = new System.Drawing.Size(346, 20);
			this.textDataFileName.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(20, 82);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(250, 14);
			this.label3.TabIndex = 7;
			this.label3.Text = "Datafile Name (eg MyTemplate.txt)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCategory
			// 
			this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCategory.Location = new System.Drawing.Point(272, 100);
			this.comboCategory.MaxDropDownItems = 30;
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(222, 21);
			this.comboCategory.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(20, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(250, 14);
			this.label4.TabIndex = 9;
			this.label4.Text = "Category";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPath
			// 
			this.textPath.Location = new System.Drawing.Point(272, 30);
			this.textPath.Name = "textPath";
			this.textPath.ReadOnly = true;
			this.textPath.Size = new System.Drawing.Size(346, 20);
			this.textPath.TabIndex = 23;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(21, 34);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(250, 14);
			this.label5.TabIndex = 24;
			this.label5.Text = "Letter Merge Path";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEditPaths
			// 
			this.butEditPaths.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditPaths.Location = new System.Drawing.Point(622, 27);
			this.butEditPaths.Name = "butEditPaths";
			this.butEditPaths.Size = new System.Drawing.Size(87, 25);
			this.butEditPaths.TabIndex = 25;
			this.butEditPaths.Text = "Edit Paths";
			this.butEditPaths.Click += new System.EventHandler(this.butEditPaths_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 649);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(87, 26);
			this.butDelete.TabIndex = 26;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(12, 142);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(170, 14);
			this.labelPatient.TabIndex = 28;
			this.labelPatient.Text = "Patient Fields";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPatSelect
			// 
			this.listPatSelect.Location = new System.Drawing.Point(12, 160);
			this.listPatSelect.Name = "listPatSelect";
			this.listPatSelect.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listPatSelect.Size = new System.Drawing.Size(170, 472);
			this.listPatSelect.TabIndex = 27;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(190, 158);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(622, 23);
			this.textBox1.TabIndex = 29;
			this.textBox1.Text = "Hint: Use the Ctrl key when clicking.  Also you can simply drag the pointer acros" +
    "s muliple rows to select quickly.";
			// 
			// butBrowse
			// 
			this.butBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBrowse.Location = new System.Drawing.Point(622, 53);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(87, 25);
			this.butBrowse.TabIndex = 30;
			this.butBrowse.Text = "Browse";
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// labelReferredFrom
			// 
			this.labelReferredFrom.Location = new System.Drawing.Point(206, 182);
			this.labelReferredFrom.Name = "labelReferredFrom";
			this.labelReferredFrom.Size = new System.Drawing.Size(170, 14);
			this.labelReferredFrom.TabIndex = 32;
			this.labelReferredFrom.Text = "Referred From";
			this.labelReferredFrom.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listReferral
			// 
			this.listReferral.Location = new System.Drawing.Point(206, 199);
			this.listReferral.Name = "listReferral";
			this.listReferral.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listReferral.Size = new System.Drawing.Size(170, 433);
			this.listReferral.TabIndex = 31;
			// 
			// butNew
			// 
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(714, 53);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(68, 25);
			this.butNew.TabIndex = 33;
			this.butNew.Text = "New";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(400, 182);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(170, 14);
			this.label6.TabIndex = 35;
			this.label6.Text = "Other";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listOther
			// 
			this.listOther.Location = new System.Drawing.Point(400, 199);
			this.listOther.Name = "listOther";
			this.listOther.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listOther.Size = new System.Drawing.Size(170, 433);
			this.listOther.TabIndex = 34;
			// 
			// labelImageCategory
			// 
			this.labelImageCategory.Location = new System.Drawing.Point(20, 128);
			this.labelImageCategory.Name = "labelImageCategory";
			this.labelImageCategory.Size = new System.Drawing.Size(250, 14);
			this.labelImageCategory.TabIndex = 37;
			this.labelImageCategory.Text = "Save to Image Folder";
			this.labelImageCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboImageFolder
			// 
			this.comboImageFolder.Location = new System.Drawing.Point(272, 124);
			this.comboImageFolder.Name = "comboImageFolder";
			this.comboImageFolder.Size = new System.Drawing.Size(222, 21);
			this.comboImageFolder.TabIndex = 36;
			// 
			// FormLetterMergeEdit
			// 
			this.ClientSize = new System.Drawing.Size(894, 683);
			this.Controls.Add(this.labelImageCategory);
			this.Controls.Add(this.comboImageFolder);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.listOther);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.labelReferredFrom);
			this.Controls.Add(this.listReferral);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.listPatSelect);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butEditPaths);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboCategory);
			this.Controls.Add(this.textDataFileName);
			this.Controls.Add(this.textTemplateName);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLetterMergeEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Letter Merge";
			this.Load += new System.EventHandler(this.FormLetterMergeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textTemplateName;
		private System.Windows.Forms.TextBox textDataFileName;
		private System.Windows.Forms.ComboBox comboCategory;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textPath;
		private OpenDental.UI.Button butEditPaths;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelPatient;
		private OpenDental.UI.ListBoxOD listPatSelect;
		private OpenDental.UI.Button butBrowse;
		private System.Windows.Forms.OpenFileDialog openFileDlg;
		private System.Windows.Forms.Label labelReferredFrom;
		private OpenDental.UI.ListBoxOD listReferral;
		private OpenDental.UI.Button butNew;
		private Label label6;
		private OpenDental.UI.ListBoxOD listOther;
		private System.Windows.Forms.TextBox textBox1;
		private Label labelImageCategory;
		private UI.ComboBoxOD comboImageFolder;
	}
}
