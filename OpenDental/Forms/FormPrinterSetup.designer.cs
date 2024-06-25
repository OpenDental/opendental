using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPrinterSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrinterSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.checkSimple = new OpenDental.UI.CheckBox();
			this.gridPrinters = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(19, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(438, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "These settings only apply to this workstation";
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(554, 368);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 2;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkSimple
			// 
			this.checkSimple.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSimple.Location = new System.Drawing.Point(12, 367);
			this.checkSimple.Name = "checkSimple";
			this.checkSimple.Size = new System.Drawing.Size(440, 24);
			this.checkSimple.TabIndex = 33;
			this.checkSimple.Text = "This is too complicated.  Show me the simple interface.";
			this.checkSimple.Click += new System.EventHandler(this.checkSimple_Click);
			// 
			// gridPrinters
			// 
			this.gridPrinters.Location = new System.Drawing.Point(12, 27);
			this.gridPrinters.Name = "gridPrinters";
			this.gridPrinters.Size = new System.Drawing.Size(617, 334);
			this.gridPrinters.TabIndex = 42;
			this.gridPrinters.Title = "Print Situations";
			this.gridPrinters.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPrinters_CellDoubleClick);
			// 
			// FormPrinterSetup
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(641, 404);
			this.Controls.Add(this.gridPrinters);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkSimple);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPrinterSetup";
			this.ShowInTaskbar = false;
			this.Text = "Printer Setup";
			this.Load += new System.EventHandler(this.FormPrinterSetup_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkSimple;
		private UI.GridOD gridPrinters;
	}
}
