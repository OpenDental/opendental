using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimFormItemEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimFormItemEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelImageFileName = new System.Windows.Forms.Label();
			this.textImageFileName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelFieldName = new System.Windows.Forms.Label();
			this.listFieldName = new System.Windows.Forms.ListBox();
			this.textFormatString = new System.Windows.Forms.TextBox();
			this.labelFormatString = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(852, 607);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(852, 576);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelImageFileName
			// 
			this.labelImageFileName.Location = new System.Drawing.Point(12, 12);
			this.labelImageFileName.Name = "labelImageFileName";
			this.labelImageFileName.Size = new System.Drawing.Size(236, 17);
			this.labelImageFileName.TabIndex = 2;
			this.labelImageFileName.Text = "Image File Name";
			this.labelImageFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textImageFileName
			// 
			this.textImageFileName.Location = new System.Drawing.Point(12, 30);
			this.textImageFileName.Name = "textImageFileName";
			this.textImageFileName.Size = new System.Drawing.Size(236, 20);
			this.textImageFileName.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(236, 45);
			this.label2.TabIndex = 4;
			this.label2.Text = "This file must be present in the A to Z folder.  \r\nIt should be a gif, jpg, or em" +
    "f.";
			// 
			// labelFieldName
			// 
			this.labelFieldName.Location = new System.Drawing.Point(254, 12);
			this.labelFieldName.Name = "labelFieldName";
			this.labelFieldName.Size = new System.Drawing.Size(156, 17);
			this.labelFieldName.TabIndex = 5;
			this.labelFieldName.Text = "or Field Name";
			this.labelFieldName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listFieldName
			// 
			this.listFieldName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listFieldName.Location = new System.Drawing.Point(254, 30);
			this.listFieldName.MultiColumn = true;
			this.listFieldName.Name = "listFieldName";
			this.listFieldName.Size = new System.Drawing.Size(592, 602);
			this.listFieldName.TabIndex = 6;
			this.listFieldName.DoubleClick += new System.EventHandler(this.listFieldName_DoubleClick);
			// 
			// textFormatString
			// 
			this.textFormatString.Location = new System.Drawing.Point(12, 161);
			this.textFormatString.Name = "textFormatString";
			this.textFormatString.Size = new System.Drawing.Size(236, 20);
			this.textFormatString.TabIndex = 8;
			// 
			// labelFormatString
			// 
			this.labelFormatString.Location = new System.Drawing.Point(12, 106);
			this.labelFormatString.Name = "labelFormatString";
			this.labelFormatString.Size = new System.Drawing.Size(236, 52);
			this.labelFormatString.TabIndex = 7;
			this.labelFormatString.Text = "Format String.  All dates must have a format.  Valid entries would include MM/dd/" +
    "yyyy, MM-dd-yy, and M d y as examples.";
			this.labelFormatString.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 607);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 25);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormClaimFormItemEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(939, 644);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textFormatString);
			this.Controls.Add(this.textImageFileName);
			this.Controls.Add(this.labelFormatString);
			this.Controls.Add(this.listFieldName);
			this.Controls.Add(this.labelFieldName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelImageFileName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "FormClaimFormItemEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Claim Form Item";
			this.Load += new System.EventHandler(this.FormClaimFormItemEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelImageFileName;
		private System.Windows.Forms.TextBox textImageFileName;
		private System.Windows.Forms.Label labelFieldName;
		private System.Windows.Forms.ListBox listFieldName;
		private System.Windows.Forms.TextBox textFormatString;
		private System.Windows.Forms.Label labelFormatString;
		private OpenDental.UI.Button butDelete;
	}
}
