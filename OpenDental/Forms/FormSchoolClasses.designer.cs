using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSchoolClasses {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchoolClasses));
			this.listMain = new OpenDental.UI.ListBox();
			this.butAdd = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listMain
			// 
			this.listMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listMain.Location = new System.Drawing.Point(16, 12);
			this.listMain.Name = "listMain";
			this.listMain.Size = new System.Drawing.Size(265, 381);
			this.listMain.TabIndex = 4;
			this.listMain.DoubleClick += new System.EventHandler(this.listMain_DoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(16, 416);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// FormSchoolClasses
			// 
			this.ClientSize = new System.Drawing.Size(300, 459);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.listMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSchoolClasses";
			this.ShowInTaskbar = false;
			this.Text = "Dental School Classes";
			this.Load += new System.EventHandler(this.FormSchoolClasses_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.ListBox listMain;
	}
}
