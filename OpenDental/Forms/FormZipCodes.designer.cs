using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormZipCodes {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormZipCodes));
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.gridZipCode = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(615, 374);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(76, 26);
			this.butAdd.TabIndex = 28;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(615, 513);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(76, 26);
			this.butClose.TabIndex = 26;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Location = new System.Drawing.Point(615, 410);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(76, 26);
			this.butDelete.TabIndex = 31;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridZipCode
			// 
			this.gridZipCode.AllowSortingByColumn = true;
			this.gridZipCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridZipCode.Location = new System.Drawing.Point(12, 17);
			this.gridZipCode.Name = "gridZipCode";
			this.gridZipCode.Size = new System.Drawing.Size(581, 522);
			this.gridZipCode.TabIndex = 32;
			this.gridZipCode.Title = "Zip Codes";
			this.gridZipCode.TranslationName = "TableZipCodes";
			this.gridZipCode.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridZipCode_CellDoubleClick);
			// 
			// FormZipCodes
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(715, 563);
			this.Controls.Add(this.gridZipCode);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormZipCodes";
			this.ShowInTaskbar = false;
			this.Text = "Zip Codes";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormZipCodes_Closing);
			this.Load += new System.EventHandler(this.FormZipCodes_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridZipCode;
	}
}
