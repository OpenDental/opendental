using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDocInfo {
			private System.ComponentModel.IContainer components = null;//required by designer

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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDocInfo));
            this.listCategory = new OpenDental.UI.ListBoxOD();
            this.labelCategory = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textDescript = new System.Windows.Forms.TextBox();
            this.butOK = new OpenDental.UI.Button();
            this.butCancel = new OpenDental.UI.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelFileName = new System.Windows.Forms.Label();
            this.textFileName = new System.Windows.Forms.TextBox();
            this.textDate = new OpenDental.ValidDate();
            this.labelType = new System.Windows.Forms.Label();
            this.listType = new OpenDental.UI.ListBoxOD();
            this.textSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textToothNumbers = new System.Windows.Forms.TextBox();
            this.labelToothNums = new System.Windows.Forms.Label();
            this.butOpen = new OpenDental.UI.Button();
            this.butAudit = new OpenDental.UI.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textTime = new System.Windows.Forms.TextBox();
            this.labelMountItem = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboProv = new OpenDental.UI.ComboBoxOD();
            this.SuspendLayout();
            // 
            // listCategory
            // 
            this.listCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listCategory.Location = new System.Drawing.Point(12, 30);
            this.listCategory.Name = "listCategory";
            this.listCategory.Size = new System.Drawing.Size(104, 342);
            this.listCategory.TabIndex = 2;
            // 
            // labelCategory
            // 
            this.labelCategory.Location = new System.Drawing.Point(12, 14);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(100, 16);
            this.labelCategory.TabIndex = 1;
            this.labelCategory.Text = "Category";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(117, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textDescript
            // 
            this.textDescript.Location = new System.Drawing.Point(245, 146);
            this.textDescript.MaxLength = 255;
            this.textDescript.Name = "textDescript";
            this.textDescript.Size = new System.Drawing.Size(299, 20);
            this.textDescript.TabIndex = 0;
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(664, 351);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 24);
            this.butOK.TabIndex = 3;
            this.butOK.Text = "OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(756, 351);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 24);
            this.butCancel.TabIndex = 4;
            this.butCancel.Text = "Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(144, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Date";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFileName
            // 
            this.labelFileName.Location = new System.Drawing.Point(144, 33);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(100, 18);
            this.labelFileName.TabIndex = 8;
            this.labelFileName.Text = "File Name";
            this.labelFileName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textFileName
            // 
            this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileName.Location = new System.Drawing.Point(245, 30);
            this.textFileName.Name = "textFileName";
            this.textFileName.ReadOnly = true;
            this.textFileName.Size = new System.Drawing.Size(586, 20);
            this.textFileName.TabIndex = 9;
            // 
            // textDate
            // 
            this.textDate.Location = new System.Drawing.Point(245, 76);
            this.textDate.Name = "textDate";
            this.textDate.Size = new System.Drawing.Size(104, 20);
            this.textDate.TabIndex = 1;
            // 
            // labelType
            // 
            this.labelType.Location = new System.Drawing.Point(144, 171);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(100, 18);
            this.labelType.TabIndex = 11;
            this.labelType.Text = "Type";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // listType
            // 
            this.listType.Location = new System.Drawing.Point(245, 169);
            this.listType.Name = "listType";
            this.listType.Size = new System.Drawing.Size(104, 69);
            this.listType.TabIndex = 10;
            // 
            // textSize
            // 
            this.textSize.Location = new System.Drawing.Point(245, 53);
            this.textSize.Name = "textSize";
            this.textSize.ReadOnly = true;
            this.textSize.Size = new System.Drawing.Size(134, 20);
            this.textSize.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(144, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 18);
            this.label6.TabIndex = 12;
            this.label6.Text = "File Size";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textToothNumbers
            // 
            this.textToothNumbers.Location = new System.Drawing.Point(245, 241);
            this.textToothNumbers.Name = "textToothNumbers";
            this.textToothNumbers.Size = new System.Drawing.Size(134, 20);
            this.textToothNumbers.TabIndex = 15;
            // 
            // labelToothNums
            // 
            this.labelToothNums.Location = new System.Drawing.Point(144, 243);
            this.labelToothNums.Name = "labelToothNums";
            this.labelToothNums.Size = new System.Drawing.Size(100, 18);
            this.labelToothNums.TabIndex = 14;
            this.labelToothNums.Text = "Tooth Numbers";
            this.labelToothNums.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // butOpen
            // 
            this.butOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butOpen.Location = new System.Drawing.Point(756, 55);
            this.butOpen.Name = "butOpen";
            this.butOpen.Size = new System.Drawing.Size(75, 24);
            this.butOpen.TabIndex = 16;
            this.butOpen.Text = "Open Folder";
            this.butOpen.Click += new System.EventHandler(this.butOpen_Click);
            // 
            // butAudit
            // 
            this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butAudit.Location = new System.Drawing.Point(245, 351);
            this.butAudit.Name = "butAudit";
            this.butAudit.Size = new System.Drawing.Size(92, 24);
            this.butAudit.TabIndex = 126;
            this.butAudit.Text = "Audit Trail";
            this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(149, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 16);
            this.label4.TabIndex = 128;
            this.label4.Text = "Time";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textTime
            // 
            this.textTime.Location = new System.Drawing.Point(245, 99);
            this.textTime.Name = "textTime";
            this.textTime.Size = new System.Drawing.Size(104, 20);
            this.textTime.TabIndex = 129;
            // 
            // labelMountItem
            // 
            this.labelMountItem.Location = new System.Drawing.Point(242, 294);
            this.labelMountItem.Name = "labelMountItem";
            this.labelMountItem.Size = new System.Drawing.Size(271, 34);
            this.labelMountItem.TabIndex = 135;
            this.labelMountItem.Text = "This information is only for one individual mount item.  The mount has a separate" +
    " information window.";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(149, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 16);
            this.label1.TabIndex = 136;
            this.label1.Text = "Provider";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // comboProv
            // 
            this.comboProv.Location = new System.Drawing.Point(245, 122);
            this.comboProv.Name = "comboProv";
            this.comboProv.Size = new System.Drawing.Size(173, 21);
            this.comboProv.TabIndex = 137;
            this.comboProv.Text = "comboBoxOD1";
            // 
            // FormDocInfo
            // 
            this.AcceptButton = this.butOK;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(852, 387);
            this.Controls.Add(this.comboProv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelMountItem);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.textTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.butAudit);
            this.Controls.Add(this.butOpen);
            this.Controls.Add(this.textToothNumbers);
            this.Controls.Add(this.labelToothNums);
            this.Controls.Add(this.textSize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.listType);
            this.Controls.Add(this.textDate);
            this.Controls.Add(this.textDescript);
            this.Controls.Add(this.textFileName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCategory);
            this.Controls.Add(this.listCategory);
            this.Controls.Add(this.labelFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDocInfo";
            this.ShowInTaskbar = false;
            this.Text = "Item Info";
            this.Load += new System.EventHandler(this.FormDocInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelCategory;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listCategory;
		private System.Windows.Forms.TextBox textDescript;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelFileName;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.TextBox textFileName;
		private System.Windows.Forms.Label labelType;
		private OpenDental.UI.ListBoxOD listType;
		private System.Windows.Forms.TextBox textSize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textToothNumbers;
		private System.Windows.Forms.Label labelToothNums;
		private UI.Button butOpen;
		private UI.Button butAudit;
		private Label label4;
		private TextBox textTime;
		private Label labelMountItem;
		private Label label1;
		private UI.ComboBoxOD comboProv;
	}
}
