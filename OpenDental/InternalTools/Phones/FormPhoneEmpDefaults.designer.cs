using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPhoneEmpDefaults {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneEmpDefaults));
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butPhoneComps = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(879, 546);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 11;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Location = new System.Drawing.Point(115, 546);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 12;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(8, 14);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(946, 524);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Phone Settings";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butPhoneComps
			// 
			this.butPhoneComps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPhoneComps.Location = new System.Drawing.Point(8, 546);
			this.butPhoneComps.Name = "butPhoneComps";
			this.butPhoneComps.Size = new System.Drawing.Size(101, 24);
			this.butPhoneComps.TabIndex = 13;
			this.butPhoneComps.Text = "Phone Comps";
			this.butPhoneComps.Visible = false;
			this.butPhoneComps.Click += new System.EventHandler(this.butPhoneComps_Click);
			// 
			// FormPhoneEmpDefaults
			// 
			this.ClientSize = new System.Drawing.Size(966, 582);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPhoneComps);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPhoneEmpDefaults";
			this.Text = "Phone Employee Defaults";
			this.Load += new System.EventHandler(this.FormAccountPick_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butClose;
		private UI.Button butAdd;
		private UI.Button butPhoneComps;
	}
}
