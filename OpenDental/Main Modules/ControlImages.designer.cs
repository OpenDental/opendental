namespace OpenDental {
	partial class ControlImages {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				_suniDeviceControl.KillXRayThread();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlImages));
			this.treeMain = new System.Windows.Forms.TreeView();
			this.contextTree = new System.Windows.Forms.ContextMenu();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.imageListTools2 = new System.Windows.Forms.ImageList(this.components);
			this.pictureBoxMain = new System.Windows.Forms.PictureBox();
			this.menuMountItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.menuPrefs = new System.Windows.Forms.MenuItem();
			this.panelNote = new System.Windows.Forms.Panel();
			this.labelInvalidSig = new System.Windows.Forms.Label();
			this.sigBox = new OpenDental.UI.SignatureBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.panelUnderline = new System.Windows.Forms.Panel();
			this.panelVertLine = new System.Windows.Forms.Panel();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.ToolBarPaint = new OpenDental.UI.ToolBarOD();
			this.windowingSlider = new OpenDental.UI.WindowingSlider();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
			this.panelNote.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeMain
			// 
			this.treeMain.AllowDrop = true;
			this.treeMain.ContextMenu = this.contextTree;
			this.treeMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeMain.FullRowSelect = true;
			this.treeMain.HideSelection = false;
			this.treeMain.ImageIndex = 2;
			this.treeMain.ImageList = this.imageListTree;
			this.treeMain.Indent = 20;
			this.treeMain.Location = new System.Drawing.Point(0, 29);
			this.treeMain.Name = "treeMain";
			this.treeMain.SelectedImageIndex = 2;
			this.treeMain.Size = new System.Drawing.Size(228, 519);
			this.treeMain.TabIndex = 0;
			this.treeMain.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TreeDocuments_AfterCollapse);
			this.treeMain.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TreeDocuments_AfterExpand);
			this.treeMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeDocuments_DragDrop);
			this.treeMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeDocuments_DragEnter);
			this.treeMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TreeDocuments_MouseDoubleClick);
			this.treeMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeDocuments_MouseDown);
			this.treeMain.MouseLeave += new System.EventHandler(this.TreeDocuments_MouseLeave);
			this.treeMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreeDocuments_MouseMove);
			this.treeMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TreeDocuments_MouseUp);
			// 
			// contextTree
			// 
			this.contextTree.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem3,
            this.menuItem4});
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Print";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Delete";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "Info";
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "");
			this.imageListTree.Images.SetKeyName(1, "");
			this.imageListTree.Images.SetKeyName(2, "");
			this.imageListTree.Images.SetKeyName(3, "");
			this.imageListTree.Images.SetKeyName(4, "");
			this.imageListTree.Images.SetKeyName(5, "");
			this.imageListTree.Images.SetKeyName(6, "");
			this.imageListTree.Images.SetKeyName(7, "");
			// 
			// imageListTools2
			// 
			this.imageListTools2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools2.ImageStream")));
			this.imageListTools2.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTools2.Images.SetKeyName(0, "Pat.gif");
			this.imageListTools2.Images.SetKeyName(1, "print.gif");
			this.imageListTools2.Images.SetKeyName(2, "deleteX.gif");
			this.imageListTools2.Images.SetKeyName(3, "info.gif");
			this.imageListTools2.Images.SetKeyName(4, "scan.gif");
			this.imageListTools2.Images.SetKeyName(5, "import.gif");
			this.imageListTools2.Images.SetKeyName(6, "paste.gif");
			this.imageListTools2.Images.SetKeyName(7, "");
			this.imageListTools2.Images.SetKeyName(8, "ZoomIn.gif");
			this.imageListTools2.Images.SetKeyName(9, "ZoomOut.gif");
			this.imageListTools2.Images.SetKeyName(10, "Hand.gif");
			this.imageListTools2.Images.SetKeyName(11, "flip.gif");
			this.imageListTools2.Images.SetKeyName(12, "rotateL.gif");
			this.imageListTools2.Images.SetKeyName(13, "rotateR.gif");
			this.imageListTools2.Images.SetKeyName(14, "scanDoc.gif");
			this.imageListTools2.Images.SetKeyName(15, "scanPhoto.gif");
			this.imageListTools2.Images.SetKeyName(16, "scanXray.gif");
			this.imageListTools2.Images.SetKeyName(17, "copy.gif");
			this.imageListTools2.Images.SetKeyName(18, "ScanMulti.gif");
			this.imageListTools2.Images.SetKeyName(19, "Export.gif");
			// 
			// pictureBoxMain
			// 
			this.pictureBoxMain.BackColor = System.Drawing.SystemColors.Window;
			this.pictureBoxMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBoxMain.ContextMenuStrip = this.menuMountItem;
			this.pictureBoxMain.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBoxMain.InitialImage = null;
			this.pictureBoxMain.Location = new System.Drawing.Point(233, 54);
			this.pictureBoxMain.Name = "pictureBoxMain";
			this.pictureBoxMain.Size = new System.Drawing.Size(703, 370);
			this.pictureBoxMain.TabIndex = 6;
			this.pictureBoxMain.TabStop = false;
			this.pictureBoxMain.SizeChanged += new System.EventHandler(this.pictureBoxMain_SizeChanged);
			this.pictureBoxMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseDown);
			this.pictureBoxMain.MouseHover += new System.EventHandler(this.pictureBoxMain_MouseHover);
			this.pictureBoxMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseMove);
			this.pictureBoxMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseUp);
			// 
			// menuMountItem
			// 
			this.menuMountItem.Name = "MountMenu";
			this.menuMountItem.Size = new System.Drawing.Size(61, 4);
			this.menuMountItem.Opening += new System.ComponentModel.CancelEventHandler(this.menuMountItem_Opening);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuPrefs});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuExit});
			this.menuItem1.Text = "File";
			// 
			// menuExit
			// 
			this.menuExit.Index = 0;
			this.menuExit.Text = "Exit";
			// 
			// menuPrefs
			// 
			this.menuPrefs.Index = 1;
			this.menuPrefs.Text = "Preferences";
			// 
			// panelNote
			// 
			this.panelNote.Controls.Add(this.labelInvalidSig);
			this.panelNote.Controls.Add(this.sigBox);
			this.panelNote.Controls.Add(this.label15);
			this.panelNote.Controls.Add(this.label1);
			this.panelNote.Controls.Add(this.textNote);
			this.panelNote.Location = new System.Drawing.Point(234, 485);
			this.panelNote.Name = "panelNote";
			this.panelNote.Size = new System.Drawing.Size(705, 64);
			this.panelNote.TabIndex = 11;
			this.panelNote.Visible = false;
			this.panelNote.DoubleClick += new System.EventHandler(this.panelNote_DoubleClick);
			// 
			// labelInvalidSig
			// 
			this.labelInvalidSig.BackColor = System.Drawing.SystemColors.Window;
			this.labelInvalidSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInvalidSig.Location = new System.Drawing.Point(398, 31);
			this.labelInvalidSig.Name = "labelInvalidSig";
			this.labelInvalidSig.Size = new System.Drawing.Size(196, 59);
			this.labelInvalidSig.TabIndex = 94;
			this.labelInvalidSig.Text = "Invalid Signature -  Document or note has changed since it was signed.";
			this.labelInvalidSig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelInvalidSig.DoubleClick += new System.EventHandler(this.labelInvalidSig_DoubleClick);
			// 
			// sigBox
			// 
			this.sigBox.Location = new System.Drawing.Point(308, 20);
			this.sigBox.Name = "sigBox";
			this.sigBox.Size = new System.Drawing.Size(362, 79);
			this.sigBox.TabIndex = 90;
			this.sigBox.DoubleClick += new System.EventHandler(this.sigBox_DoubleClick);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(305, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(63, 18);
			this.label15.TabIndex = 87;
			this.label15.Text = "Signature";
			this.label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label15.DoubleClick += new System.EventHandler(this.label15_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label1.DoubleClick += new System.EventHandler(this.label1_DoubleClick);
			// 
			// textNote
			// 
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.Location = new System.Drawing.Point(0, 20);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ReadOnly = true;
			this.textNote.Size = new System.Drawing.Size(302, 79);
			this.textNote.TabIndex = 0;
			this.textNote.DoubleClick += new System.EventHandler(this.textNote_DoubleClick);
			this.textNote.MouseHover += new System.EventHandler(this.textNote_MouseHover);
			// 
			// panelUnderline
			// 
			this.panelUnderline.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelUnderline.Location = new System.Drawing.Point(234, 49);
			this.panelUnderline.Name = "panelUnderline";
			this.panelUnderline.Size = new System.Drawing.Size(704, 2);
			this.panelUnderline.TabIndex = 15;
			// 
			// panelVertLine
			// 
			this.panelVertLine.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelVertLine.Location = new System.Drawing.Point(233, 25);
			this.panelVertLine.Name = "panelVertLine";
			this.panelVertLine.Size = new System.Drawing.Size(2, 25);
			this.panelVertLine.TabIndex = 16;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListTools2;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(939, 25);
			this.ToolBarMain.TabIndex = 10;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// ToolBarPaint
			// 
			this.ToolBarPaint.ImageList = this.imageListTools2;
			this.ToolBarPaint.Location = new System.Drawing.Point(440, 25);
			this.ToolBarPaint.Name = "ToolBarPaint";
			this.ToolBarPaint.Size = new System.Drawing.Size(499, 25);
			this.ToolBarPaint.TabIndex = 14;
			this.ToolBarPaint.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarPaint_ButtonClick);
			// 
			// windowingSlider
			// 
			this.windowingSlider.Enabled = false;
			this.windowingSlider.Location = new System.Drawing.Point(240, 32);
			this.windowingSlider.MaxVal = 255;
			this.windowingSlider.MinVal = 0;
			this.windowingSlider.Name = "windowingSlider";
			this.windowingSlider.Size = new System.Drawing.Size(194, 14);
			this.windowingSlider.TabIndex = 12;
			this.windowingSlider.Text = "contrWindowingSlider1";
			this.windowingSlider.Scroll += new System.EventHandler(this.windowingSlider_Scroll);
			this.windowingSlider.ScrollComplete += new System.EventHandler(this.windowingSlider_ScrollComplete);
			// 
			// ContrImages
			// 
			this.AllowDrop = true;
			this.Controls.Add(this.panelVertLine);
			this.Controls.Add(this.panelUnderline);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.ToolBarPaint);
			this.Controls.Add(this.windowingSlider);
			this.Controls.Add(this.panelNote);
			this.Controls.Add(this.pictureBoxMain);
			this.Controls.Add(this.treeMain);
			this.Name = "ContrImages";
			this.Size = new System.Drawing.Size(939, 585);
			this.Resize += new System.EventHandler(this.ContrImages_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
			this.panelNote.ResumeLayout(false);
			this.panelNote.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ImageList imageListTree;
		private System.Windows.Forms.ImageList imageListTools2;
		private System.Windows.Forms.TreeView treeMain;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.MenuItem menuPrefs;
		private UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ContextMenu contextTree;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.Panel panelNote;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textNote;
		private UI.SignatureBox sigBox;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label labelInvalidSig;
		private UI.WindowingSlider windowingSlider;
		private UI.ToolBarOD ToolBarPaint;
		private System.Windows.Forms.Panel panelUnderline;
		private System.Windows.Forms.Panel panelVertLine;
		private System.Windows.Forms.PictureBox pictureBoxMain;
		private System.Windows.Forms.ContextMenu menuForms;
		private System.Windows.Forms.ContextMenuStrip menuMountItem;
		private System.Windows.Forms.ContextMenu menuMounts;
	}
}
