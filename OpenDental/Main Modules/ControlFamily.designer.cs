using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	partial class ControlFamily {

		private IContainer components=null;
		#region Dispose
		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion Dispose

		#region Component Designer generated code
		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlFamily));
			this.imageListToolBar = new System.Windows.Forms.ImageList(this.components);
			this.contextMenuInsurance = new System.Windows.Forms.ContextMenu();
			this.menuItemPlansForFam = new System.Windows.Forms.MenuItem();
			this.contextMenuDiscount = new System.Windows.Forms.ContextMenu();
			this.menuItemRemoveDiscount = new System.Windows.Forms.MenuItem();
			this.gridSuperFam = new OpenDental.UI.GridOD();
			this.gridRecall = new OpenDental.UI.GridOD();
			this.gridFamily = new OpenDental.UI.GridOD();
			this.gridPat = new OpenDental.UI.GridOD();
			this.gridIns = new OpenDental.UI.GridOD();
			this.gridPatientClones = new OpenDental.UI.GridOD();
			this.pictureBoxPat = new OpenDental.UI.ODPictureBox();
			this.toolBarMain = new OpenDental.UI.ToolBarOD();
			this.splitContainerSuperClones = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.splitContainerSuperClones.SuspendLayout();
			this.splitterPanel1.SuspendLayout();
			this.splitterPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListToolBar
			// 
			this.imageListToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListToolBar.ImageStream")));
			this.imageListToolBar.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListToolBar.Images.SetKeyName(0, "");
			this.imageListToolBar.Images.SetKeyName(1, "");
			this.imageListToolBar.Images.SetKeyName(2, "");
			this.imageListToolBar.Images.SetKeyName(3, "");
			this.imageListToolBar.Images.SetKeyName(4, "");
			this.imageListToolBar.Images.SetKeyName(5, "");
			this.imageListToolBar.Images.SetKeyName(6, "Umbrella.gif");
			// 
			// contextMenuInsurance
			// 
			this.contextMenuInsurance.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPlansForFam});
			// 
			// menuItemPlansForFam
			// 
			this.menuItemPlansForFam.Index = 0;
			this.menuItemPlansForFam.Text = "Plans for Family";
			this.menuItemPlansForFam.Click += new System.EventHandler(this.menuPlansForFam_Click);
			// 
			// contextMenuDiscount
			// 
			this.contextMenuDiscount.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemRemoveDiscount});
			// 
			// menuItemRemoveDiscount
			// 
			this.menuItemRemoveDiscount.Index = 0;
			this.menuItemRemoveDiscount.Text = "Drop Discount Plan";
			this.menuItemRemoveDiscount.Click += new System.EventHandler(this.menuItemRemoveDiscount_Click);
			// 
			// gridSuperFam
			// 
			this.gridSuperFam.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridSuperFam.Location = new System.Drawing.Point(0, 0);
			this.gridSuperFam.Name = "gridSuperFam";
			this.gridSuperFam.Size = new System.Drawing.Size(329, 285);
			this.gridSuperFam.TabIndex = 33;
			this.gridSuperFam.Title = "Super Family";
			this.gridSuperFam.TranslationName = "TableSuper";
			this.gridSuperFam.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSuperFam_CellDoubleClick);
			this.gridSuperFam.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSuperFam_CellClick);
			// 
			// gridRecall
			// 
			this.gridRecall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRecall.Location = new System.Drawing.Point(585, 27);
			this.gridRecall.Name = "gridRecall";
			this.gridRecall.Size = new System.Drawing.Size(354, 100);
			this.gridRecall.TabIndex = 32;
			this.gridRecall.Title = "Recall";
			this.gridRecall.TranslationName = "TableRecall";
			this.gridRecall.WrapText = false;
			this.gridRecall.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRecall_CellDoubleClick);
			this.gridRecall.DoubleClick += new System.EventHandler(this.gridRecall_DoubleClick);
			// 
			// gridFamily
			// 
			this.gridFamily.ColorSelectedRow = System.Drawing.Color.DarkSalmon;
			this.gridFamily.Location = new System.Drawing.Point(103, 27);
			this.gridFamily.Name = "gridFamily";
			this.gridFamily.Size = new System.Drawing.Size(480, 100);
			this.gridFamily.TabIndex = 31;
			this.gridFamily.Title = "Family Members";
			this.gridFamily.TranslationName = "TableFamily";
			this.gridFamily.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFamily_CellDoubleClick);
			this.gridFamily.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFamily_CellClick);
			// 
			// gridPat
			// 
			this.gridPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridPat.Location = new System.Drawing.Point(0, 129);
			this.gridPat.Name = "gridPat";
			this.gridPat.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridPat.Size = new System.Drawing.Size(252, 579);
			this.gridPat.TabIndex = 30;
			this.gridPat.Title = "Patient Information";
			this.gridPat.TranslationName = "TablePatient";
			this.gridPat.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellDoubleClick);
			this.gridPat.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellClick);
			this.gridPat.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridPat_MouseDown);
			// 
			// gridIns
			// 
			this.gridIns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridIns.HScrollVisible = true;
			this.gridIns.Location = new System.Drawing.Point(254, 129);
			this.gridIns.Name = "gridIns";
			this.gridIns.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridIns.Size = new System.Drawing.Size(685, 579);
			this.gridIns.TabIndex = 29;
			this.gridIns.Title = "Insurance Plans";
			this.gridIns.TranslationName = "TableCoverage";
			this.gridIns.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridIns_CellDoubleClick);
			// 
			// gridPatientClones
			// 
			this.gridPatientClones.Location = new System.Drawing.Point(0, 0);
			this.gridPatientClones.Name = "gridPatientClones";
			this.gridPatientClones.Size = new System.Drawing.Size(329, 290);
			this.gridPatientClones.TabIndex = 34;
			this.gridPatientClones.Title = "Patient Clones";
			this.gridPatientClones.TranslationName = "TablePatientClones";
			this.gridPatientClones.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatientClone_CellClick);
			// 
			// pictureBoxPat
			// 
			this.pictureBoxPat.Location = new System.Drawing.Point(1, 27);
			this.pictureBoxPat.Name = "pictureBoxPat";
			this.pictureBoxPat.Size = new System.Drawing.Size(100, 100);
			this.pictureBoxPat.TabIndex = 28;
			this.pictureBoxPat.Text = "picturePat";
			this.pictureBoxPat.TextNullImage = "Patient Picture Unavailable";
			this.pictureBoxPat.DoubleClick += new System.EventHandler(this.pictureBoxPat_DoubleClick);
			// 
			// toolBarMain
			// 
			this.toolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarMain.ImageList = this.imageListToolBar;
			this.toolBarMain.Location = new System.Drawing.Point(0, 0);
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.Size = new System.Drawing.Size(939, 25);
			this.toolBarMain.TabIndex = 19;
			this.toolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// splitContainerSuperClones
			// 
			this.splitContainerSuperClones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.splitContainerSuperClones.Controls.Add(this.splitterPanel1);
			this.splitContainerSuperClones.Controls.Add(this.splitterPanel2);
			this.splitContainerSuperClones.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainerSuperClones.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.splitContainerSuperClones.Location = new System.Drawing.Point(254, 129);
			this.splitContainerSuperClones.Name = "splitContainerSuperClones";
			this.splitContainerSuperClones.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainerSuperClones.Panel1 = this.splitterPanel1;
			this.splitContainerSuperClones.Panel1MinSize = 25;
			this.splitContainerSuperClones.Panel2 = this.splitterPanel2;
			this.splitContainerSuperClones.Panel2MinSize = 25;
			this.splitContainerSuperClones.Size = new System.Drawing.Size(329, 579);
			this.splitContainerSuperClones.SplitterDistance = 285;
			this.splitContainerSuperClones.TabIndex = 35;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Controls.Add(this.gridSuperFam);
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(329, 285);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Controls.Add(this.gridPatientClones);
			this.splitterPanel2.Location = new System.Drawing.Point(0, 289);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(329, 290);
			this.splitterPanel2.TabIndex = 14;
			// 
			// ControlFamily
			// 
			this.Controls.Add(this.splitContainerSuperClones);
			this.Controls.Add(this.gridRecall);
			this.Controls.Add(this.gridFamily);
			this.Controls.Add(this.gridPat);
			this.Controls.Add(this.gridIns);
			this.Controls.Add(this.pictureBoxPat);
			this.Controls.Add(this.toolBarMain);
			this.Name = "ControlFamily";
			this.Size = new System.Drawing.Size(939, 708);
			this.splitContainerSuperClones.ResumeLayout(false);
			this.splitterPanel1.ResumeLayout(false);
			this.splitterPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private ContextMenu contextMenuDiscount;
		private ContextMenu contextMenuInsurance;
		private GridOD gridFamily;
		private GridOD gridIns;
		private GridOD gridPat;
		private GridOD gridPatientClones;
		private GridOD gridRecall;
		private GridOD gridSuperFam;
		private ImageList imageListToolBar;
		private MenuItem menuItemRemoveDiscount;
		private MenuItem menuItemPlansForFam;
		private ODPictureBox pictureBoxPat;
		private UI.SplitContainer splitContainerSuperClones;
		private UI.SplitterPanel splitterPanel1;
		private UI.SplitterPanel splitterPanel2;
	}
}