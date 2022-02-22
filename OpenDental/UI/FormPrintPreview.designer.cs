using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class FormPrintPreview {
		private System.ComponentModel.IContainer components=null;

		/// <summary>Clean up any resources being used.</summary>
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrintPreview));
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.printPreviewControl2 = new OpenDental.UI.ODPrintPreviewControl();
			this.SuspendLayout();
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(831, 25);
			this.ToolBarMain.TabIndex = 5;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "");
			this.imageListMain.Images.SetKeyName(1, "");
			this.imageListMain.Images.SetKeyName(2, "");
			// 
			// printPreviewControl2
			// 
			this.printPreviewControl2.AutoZoom = false;
			this.printPreviewControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.printPreviewControl2.Location = new System.Drawing.Point(0, 25);
			this.printPreviewControl2.Name = "printPreviewControl2";
			this.printPreviewControl2.Size = new System.Drawing.Size(831, 545);
			this.printPreviewControl2.TabIndex = 6;
			// 
			// FormPrintPreview
			// 
			this.ClientSize = new System.Drawing.Size(831, 570);
			this.Controls.Add(this.printPreviewControl2);
			this.Controls.Add(this.ToolBarMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrintPreview";
			this.ShowInTaskbar = false;
			this.Text = "Print Preview";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormPrintPreview_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private OpenDental.UI.ODPrintPreviewControl printPreviewControl2;
	}
}
