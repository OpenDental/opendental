using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReportCustom {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportCustom));
			this.viewer = new fyiReporting.RdlViewer.RdlViewer();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.menuScrollMode = new System.Windows.Forms.ContextMenu();
			this.menuItemContinuous = new System.Windows.Forms.MenuItem();
			this.menuItemContinuousFacing = new System.Windows.Forms.MenuItem();
			this.menuItemFacing = new System.Windows.Forms.MenuItem();
			this.menuItemSinglePage = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// viewer
			// 
			this.viewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.viewer.Cursor = System.Windows.Forms.Cursors.Default;
			this.viewer.Folder = null;
			this.viewer.Location = new System.Drawing.Point(45,56);
			this.viewer.Name = "viewer";
			this.viewer.PageCurrent = 1;
			this.viewer.Parameters = "XPat="+FormOpenDental.CurPatNum.ToString();
			this.viewer.ReportName = null;
			this.viewer.ScrollMode = fyiReporting.RdlViewer.ScrollModeEnum.Continuous;
			this.viewer.ShowParameterPanel = true;
			this.viewer.Size = new System.Drawing.Size(856,453);
			this.viewer.SourceFile = null;
			this.viewer.SourceRdl = null;
			this.viewer.TabIndex = 2;
			this.viewer.Text = "rdlViewer1";
			this.viewer.Zoom = 0.3662712F;
			this.viewer.ZoomMode = fyiReporting.RdlViewer.ZoomEnum.FitPage;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0,0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(987,25);
			this.ToolBarMain.TabIndex = 5;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0,"");
			this.imageListMain.Images.SetKeyName(1,"");
			this.imageListMain.Images.SetKeyName(2,"");
			this.imageListMain.Images.SetKeyName(3,"");
			this.imageListMain.Images.SetKeyName(4,"");
			this.imageListMain.Images.SetKeyName(5,"");
			this.imageListMain.Images.SetKeyName(6,"");
			// 
			// menuScrollMode
			// 
			this.menuScrollMode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemContinuous,
            this.menuItemContinuousFacing,
            this.menuItemFacing,
            this.menuItemSinglePage});
			// 
			// menuItemContinuous
			// 
			this.menuItemContinuous.Index = 0;
			this.menuItemContinuous.Text = "Continuous";
			this.menuItemContinuous.Click += new System.EventHandler(this.menuItemContinuous_Click);
			// 
			// menuItemContinuousFacing
			// 
			this.menuItemContinuousFacing.Index = 1;
			this.menuItemContinuousFacing.Text = "Continuous Facing";
			this.menuItemContinuousFacing.Click += new System.EventHandler(this.menuItemContinuousFacing_Click);
			// 
			// menuItemFacing
			// 
			this.menuItemFacing.Index = 2;
			this.menuItemFacing.Text = "Facing";
			this.menuItemFacing.Click += new System.EventHandler(this.menuItemFacing_Click);
			// 
			// menuItemSinglePage
			// 
			this.menuItemSinglePage.Index = 3;
			this.menuItemSinglePage.Text = "Single Page";
			this.menuItemSinglePage.Click += new System.EventHandler(this.menuItemSinglePage_Click);
			// 
			// FormReportCustom
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(987,712);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.viewer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportCustom";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Report";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormRDLreport_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormReport_Layout);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.ContextMenu menuScrollMode;
		private System.Windows.Forms.MenuItem menuItemContinuous;
		private System.Windows.Forms.MenuItem menuItemContinuousFacing;
		private System.Windows.Forms.MenuItem menuItemFacing;
		private System.Windows.Forms.MenuItem menuItemSinglePage;
		private fyiReporting.RdlViewer.RdlViewer viewer;
	}
}
