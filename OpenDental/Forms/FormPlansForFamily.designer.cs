using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPlansForFamily {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlansForFamily));
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(12,57);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(538,181);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Insurance Plans for Family";
			this.gridMain.TranslationName = "TableInsPlans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12,9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(481,35);
			this.label1.TabIndex = 2;
			this.label1.Text = "This is a list of all insurance plans for the family.  The main purpose is to vie" +
    "w inactive plans that have been dropped.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormPlansForFamily
			// 
			this.ClientSize = new System.Drawing.Size(564,253);
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPlansForFamily";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Insurance Plans for Family";
			this.Load += new System.EventHandler(this.FormPlansForFamily_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private UI.GridOD gridMain;
		private Label label1;
	}
}
