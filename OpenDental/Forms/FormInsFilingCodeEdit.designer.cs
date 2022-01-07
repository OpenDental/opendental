using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsFilingCodeEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components=null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsFilingCodeEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textEclaimCode = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.gridInsFilingCodeSubtypes = new OpenDental.UI.GridOD();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.comboGroup = new OpenDental.UI.ComboBoxOD();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(160, 20);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(291, 20);
			this.textDescription.TabIndex = 0;
			this.textDescription.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			// 
			// textEclaimCode
			// 
			this.textEclaimCode.Location = new System.Drawing.Point(160, 45);
			this.textEclaimCode.MaxLength = 255;
			this.textEclaimCode.Name = "textEclaimCode";
			this.textEclaimCode.Size = new System.Drawing.Size(157, 20);
			this.textEclaimCode.TabIndex = 1;
			this.textEclaimCode.TextChanged += new System.EventHandler(this.textEclaimCode_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(151, 17);
			this.label2.TabIndex = 99;
			this.label2.Text = "Eclaim Code";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridInsFilingCodeSubtypes
			// 
			this.gridInsFilingCodeSubtypes.Location = new System.Drawing.Point(160, 97);
			this.gridInsFilingCodeSubtypes.Name = "gridInsFilingCodeSubtypes";
			this.gridInsFilingCodeSubtypes.Size = new System.Drawing.Size(291, 160);
			this.gridInsFilingCodeSubtypes.TabIndex = 100;
			this.gridInsFilingCodeSubtypes.Title = "Insurance Filing Code Subtypes";
			this.gridInsFilingCodeSubtypes.TranslationName = "TableInsFilingCodeSubtypes";
			this.gridInsFilingCodeSubtypes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInsFilingCodeSubtypes_CellDoubleClick);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(27, 302);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 26);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(412, 302);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(503, 302);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAdd
			// 
			this.butAdd.Enabled = false;
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(160, 263);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 101;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(151, 17);
			this.label3.TabIndex = 103;
			this.label3.Text = "Group";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboGroup
			// 
			this.comboGroup.Location = new System.Drawing.Point(160, 70);
			this.comboGroup.Name = "comboGroup";
			this.comboGroup.Size = new System.Drawing.Size(157, 21);
			this.comboGroup.TabIndex = 104;
			// 
			// FormInsFilingCodeEdit
			// 
			this.ClientSize = new System.Drawing.Size(604, 346);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.comboGroup);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridInsFilingCodeSubtypes);
			this.Controls.Add(this.textEclaimCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInsFilingCodeEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Claim Filing Code";
			this.Load += new System.EventHandler(this.FormInsFilingCodeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textEclaimCode;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.GridOD gridInsFilingCodeSubtypes;
		private OpenDental.UI.Button butAdd;
		private Label label3;
		private UI.ComboBoxOD comboGroup;
	}
}
