namespace OpenDental
{
	partial class ControlImagesJ
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlImagesJ));
			this.imageListTools2 = new System.Windows.Forms.ImageList(this.components);
			this.menuForms = new System.Windows.Forms.ContextMenu();
			this.menuMounts = new System.Windows.Forms.ContextMenu();
			this.menuTree = new System.Windows.Forms.ContextMenu();
			this.menuItemTreePrint = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.panelNote = new System.Windows.Forms.Panel();
			this.labelInvalidSig = new System.Windows.Forms.Label();
			this.sigBox = new OpenDental.UI.SignatureBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.menuMainPanel = new System.Windows.Forms.ContextMenu();
			this.menuItemCopy = new System.Windows.Forms.MenuItem();
			this.menuItemPaste = new System.Windows.Forms.MenuItem();
			this.menuItemFlipHoriz = new System.Windows.Forms.MenuItem();
			this.menuItemRotateLeft = new System.Windows.Forms.MenuItem();
			this.menuItemRotateRight = new System.Windows.Forms.MenuItem();
			this.menuItemRotate180 = new System.Windows.Forms.MenuItem();
			this.menuItemDelete = new System.Windows.Forms.MenuItem();
			this.menuItemInfo = new System.Windows.Forms.MenuItem();
			this.timerTreeClick = new System.Windows.Forms.Timer(this.components);
			this.panelAcquire = new System.Windows.Forms.Panel();
			this.labelAcquireNotifications = new System.Windows.Forms.Label();
			this.panelAcquireColor = new System.Windows.Forms.Panel();
			this.comboDevice = new OpenDental.UI.ComboBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butStart = new OpenDental.UI.Button();
			this.panelImportAuto = new System.Windows.Forms.Panel();
			this.butGoTo = new OpenDental.UI.Button();
			this.butImportStart = new OpenDental.UI.Button();
			this.butImportClose = new OpenDental.UI.Button();
			this.butBrowse = new OpenDental.UI.Button();
			this.textImportAuto = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.imageSelector = new OpenDental.UI.ImageSelectorTemp();
			this.panelSplitter = new OpenDental.UI.PanelOD();
			this.toolBarPaint = new OpenDental.UI.ToolBarOD();
			this.zoomSlider = new OpenDental.UI.ZoomSlider();
			this.panelMain = new OpenDental.UI.ControlDoubleBuffered();
			this.windowingSlider = new OpenDental.UI.WindowingSlider();
			this.toolBarMain = new OpenDental.UI.ToolBarOD();
			this.panelNote.SuspendLayout();
			this.panelAcquire.SuspendLayout();
			this.panelImportAuto.SuspendLayout();
			this.SuspendLayout();
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
			this.imageListTools2.Images.SetKeyName(20, "arrowsAll.png");
			// 
			// menuTree
			// 
			this.menuTree.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemTreePrint,
            this.menuItem9,
            this.menuItem10});
			// 
			// menuItemTreePrint
			// 
			this.menuItemTreePrint.Index = 0;
			this.menuItemTreePrint.Text = "Print";
			this.menuItemTreePrint.Click += new System.EventHandler(this.menuTree_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 1;
			this.menuItem9.Text = "Delete";
			this.menuItem9.Click += new System.EventHandler(this.menuTree_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 2;
			this.menuItem10.Text = "Info";
			this.menuItem10.Click += new System.EventHandler(this.menuTree_Click);
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
			// panelNote
			// 
			this.panelNote.Controls.Add(this.labelInvalidSig);
			this.panelNote.Controls.Add(this.sigBox);
			this.panelNote.Controls.Add(this.label15);
			this.panelNote.Controls.Add(this.label1);
			this.panelNote.Controls.Add(this.textNote);
			this.panelNote.Location = new System.Drawing.Point(230, 523);
			this.panelNote.Name = "panelNote";
			this.panelNote.Size = new System.Drawing.Size(834, 64);
			this.panelNote.TabIndex = 23;
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
			// 
			// menuMainPanel
			// 
			this.menuMainPanel.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemFlipHoriz,
            this.menuItemRotateLeft,
            this.menuItemRotateRight,
            this.menuItemRotate180,
            this.menuItemDelete,
            this.menuItemInfo});
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Index = 0;
			this.menuItemCopy.Text = "Copy";
			this.menuItemCopy.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemPaste
			// 
			this.menuItemPaste.Index = 1;
			this.menuItemPaste.Text = "Paste";
			this.menuItemPaste.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemFlipHoriz
			// 
			this.menuItemFlipHoriz.Index = 2;
			this.menuItemFlipHoriz.Text = "Flip Horizontally";
			this.menuItemFlipHoriz.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemRotateLeft
			// 
			this.menuItemRotateLeft.Index = 3;
			this.menuItemRotateLeft.Text = "Rotate Left";
			this.menuItemRotateLeft.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemRotateRight
			// 
			this.menuItemRotateRight.Index = 4;
			this.menuItemRotateRight.Text = "Rotate Right";
			this.menuItemRotateRight.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemRotate180
			// 
			this.menuItemRotate180.Index = 5;
			this.menuItemRotate180.Text = "Rotate 180";
			this.menuItemRotate180.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemDelete
			// 
			this.menuItemDelete.Index = 6;
			this.menuItemDelete.Text = "Delete";
			this.menuItemDelete.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// menuItemInfo
			// 
			this.menuItemInfo.Index = 7;
			this.menuItemInfo.Text = "Info";
			this.menuItemInfo.Click += new System.EventHandler(this.menuPanelMain_Click);
			// 
			// timerTreeClick
			// 
			this.timerTreeClick.Interval = 1000;
			this.timerTreeClick.Tick += new System.EventHandler(this.timerTreeClick_Tick);
			// 
			// panelAcquire
			// 
			this.panelAcquire.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panelAcquire.Controls.Add(this.labelAcquireNotifications);
			this.panelAcquire.Controls.Add(this.panelAcquireColor);
			this.panelAcquire.Controls.Add(this.comboDevice);
			this.panelAcquire.Controls.Add(this.label2);
			this.panelAcquire.Controls.Add(this.butCancel);
			this.panelAcquire.Controls.Add(this.butStart);
			this.panelAcquire.Location = new System.Drawing.Point(262, 69);
			this.panelAcquire.Name = "panelAcquire";
			this.panelAcquire.Size = new System.Drawing.Size(790, 23);
			this.panelAcquire.TabIndex = 33;
			this.panelAcquire.Visible = false;
			// 
			// labelAcquireNotifications
			// 
			this.labelAcquireNotifications.Location = new System.Drawing.Point(383, 2);
			this.labelAcquireNotifications.Name = "labelAcquireNotifications";
			this.labelAcquireNotifications.Size = new System.Drawing.Size(404, 18);
			this.labelAcquireNotifications.TabIndex = 39;
			this.labelAcquireNotifications.Text = "Notifications";
			this.labelAcquireNotifications.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelAcquireColor
			// 
			this.panelAcquireColor.BackColor = System.Drawing.Color.LimeGreen;
			this.panelAcquireColor.Location = new System.Drawing.Point(347, 1);
			this.panelAcquireColor.Name = "panelAcquireColor";
			this.panelAcquireColor.Size = new System.Drawing.Size(30, 21);
			this.panelAcquireColor.TabIndex = 35;
			// 
			// comboDevice
			// 
			this.comboDevice.Location = new System.Drawing.Point(66, 1);
			this.comboDevice.Name = "comboDevice";
			this.comboDevice.Size = new System.Drawing.Size(148, 21);
			this.comboDevice.TabIndex = 38;
			this.comboDevice.Text = "comboBoxOD1";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "Device";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Location = new System.Drawing.Point(283, 1);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(57, 20);
			this.butCancel.TabIndex = 32;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butStart
			// 
			this.butStart.Location = new System.Drawing.Point(220, 1);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(57, 20);
			this.butStart.TabIndex = 31;
			this.butStart.Text = "Start";
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// panelImportAuto
			// 
			this.panelImportAuto.Controls.Add(this.butGoTo);
			this.panelImportAuto.Controls.Add(this.butImportStart);
			this.panelImportAuto.Controls.Add(this.butImportClose);
			this.panelImportAuto.Controls.Add(this.butBrowse);
			this.panelImportAuto.Controls.Add(this.textImportAuto);
			this.panelImportAuto.Controls.Add(this.label3);
			this.panelImportAuto.Location = new System.Drawing.Point(262, 102);
			this.panelImportAuto.Name = "panelImportAuto";
			this.panelImportAuto.Size = new System.Drawing.Size(813, 44);
			this.panelImportAuto.TabIndex = 34;
			this.panelImportAuto.Visible = false;
			// 
			// butGoTo
			// 
			this.butGoTo.Location = new System.Drawing.Point(610, 21);
			this.butGoTo.Name = "butGoTo";
			this.butGoTo.Size = new System.Drawing.Size(60, 20);
			this.butGoTo.TabIndex = 43;
			this.butGoTo.Text = "Go To";
			this.butGoTo.Click += new System.EventHandler(this.butGoTo_Click);
			// 
			// butImportStart
			// 
			this.butImportStart.Location = new System.Drawing.Point(670, 21);
			this.butImportStart.Name = "butImportStart";
			this.butImportStart.Size = new System.Drawing.Size(60, 20);
			this.butImportStart.TabIndex = 42;
			this.butImportStart.Text = "Start";
			this.butImportStart.Click += new System.EventHandler(this.butImportStart_Click);
			// 
			// butImportClose
			// 
			this.butImportClose.Location = new System.Drawing.Point(730, 21);
			this.butImportClose.Name = "butImportClose";
			this.butImportClose.Size = new System.Drawing.Size(60, 20);
			this.butImportClose.TabIndex = 41;
			this.butImportClose.Text = "Close";
			this.butImportClose.Click += new System.EventHandler(this.butImportClose_Click);
			// 
			// butBrowse
			// 
			this.butBrowse.Location = new System.Drawing.Point(550, 21);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(60, 20);
			this.butBrowse.TabIndex = 40;
			this.butBrowse.Text = "Change";
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// textImportAuto
			// 
			this.textImportAuto.Location = new System.Drawing.Point(0, 22);
			this.textImportAuto.Name = "textImportAuto";
			this.textImportAuto.Size = new System.Drawing.Size(548, 20);
			this.textImportAuto.TabIndex = 39;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(1, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(616, 16);
			this.label3.TabIndex = 38;
			this.label3.Text = "Automatically import files from the following folder as they appear.  Existing fi" +
    "les will be ignored.  Imported files will be deleted.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// imageSelector
			// 
			this.imageSelector.AllowDrop = true;
			this.imageSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.imageSelector.Location = new System.Drawing.Point(15, 102);
			this.imageSelector.Name = "imageSelector";
			this.imageSelector.Size = new System.Drawing.Size(200, 434);
			this.imageSelector.TabIndex = 35;
			this.imageSelector.DragDropImport += new System.EventHandler<OpenDental.UI.DragDropImportEventArgs>(this.imageSelector_DragDropImport);
			this.imageSelector.DraggedToCategory += new System.EventHandler<OpenDental.UI.DragEventArgsOD>(this.imageSelector_DraggedToCategory);
			this.imageSelector.ItemDoubleClick += new System.EventHandler(this.imageSelector_ItemDoubleClick);
			this.imageSelector.SelectionChangeCommitted += new System.EventHandler(this.imageSelector_SelectionChangeCommitted);
			// 
			// panelSplitter
			// 
			this.panelSplitter.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panelSplitter.Location = new System.Drawing.Point(233, 69);
			this.panelSplitter.Name = "panelSplitter";
			this.panelSplitter.Size = new System.Drawing.Size(8, 434);
			this.panelSplitter.TabIndex = 24;
			this.panelSplitter.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSplitter_Paint);
			this.panelSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseDown);
			this.panelSplitter.MouseLeave += new System.EventHandler(this.panelSplitter_MouseLeave);
			this.panelSplitter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseMove);
			this.panelSplitter.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseUp);
			// 
			// toolBarPaint
			// 
			this.toolBarPaint.ImageList = this.imageListTools2;
			this.toolBarPaint.Location = new System.Drawing.Point(393, 25);
			this.toolBarPaint.Name = "toolBarPaint";
			this.toolBarPaint.Size = new System.Drawing.Size(674, 25);
			this.toolBarPaint.TabIndex = 15;
			this.toolBarPaint.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarPaint_ButtonClick);
			// 
			// zoomSlider
			// 
			this.zoomSlider.Location = new System.Drawing.Point(162, 25);
			this.zoomSlider.Name = "zoomSlider";
			this.zoomSlider.Size = new System.Drawing.Size(231, 25);
			this.zoomSlider.TabIndex = 21;
			this.zoomSlider.Text = "zoomSlider1";
			this.zoomSlider.FitPressed += new System.EventHandler(this.zoomSlider_FitPressed);
			this.zoomSlider.Zoomed += new System.EventHandler(this.zoomSlider_Zoomed);
			// 
			// panelMain
			// 
			this.panelMain.AllowDrop = true;
			this.panelMain.BackColor = System.Drawing.Color.White;
			this.panelMain.ContextMenu = this.menuMainPanel;
			this.panelMain.Location = new System.Drawing.Point(252, 183);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(813, 298);
			this.panelMain.TabIndex = 20;
			this.panelMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelMain_DragDrop);
			this.panelMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMain_DragEnter);
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
			this.panelMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDoubleClick);
			this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
			this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
			this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
			// 
			// windowingSlider
			// 
			this.windowingSlider.Enabled = false;
			this.windowingSlider.Location = new System.Drawing.Point(4, 27);
			this.windowingSlider.MaxVal = 255;
			this.windowingSlider.MinVal = 0;
			this.windowingSlider.Name = "windowingSlider";
			this.windowingSlider.Size = new System.Drawing.Size(154, 20);
			this.windowingSlider.TabIndex = 16;
			this.windowingSlider.Text = "contrWindowingSlider1";
			this.windowingSlider.Scroll += new System.EventHandler(this.windowingSlider_Scroll);
			this.windowingSlider.ScrollComplete += new System.EventHandler(this.windowingSlider_ScrollComplete);
			// 
			// toolBarMain
			// 
			this.toolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarMain.ImageList = this.imageListTools2;
			this.toolBarMain.Location = new System.Drawing.Point(0, 0);
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.Size = new System.Drawing.Size(1067, 25);
			this.toolBarMain.TabIndex = 11;
			this.toolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarMain_ButtonClick);
			// 
			// ControlImagesJ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.imageSelector);
			this.Controls.Add(this.panelImportAuto);
			this.Controls.Add(this.panelAcquire);
			this.Controls.Add(this.panelSplitter);
			this.Controls.Add(this.panelNote);
			this.Controls.Add(this.toolBarPaint);
			this.Controls.Add(this.zoomSlider);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.windowingSlider);
			this.Controls.Add(this.toolBarMain);
			this.DoubleBuffered = true;
			this.Name = "ControlImagesJ";
			this.Size = new System.Drawing.Size(1067, 595);
			this.Resize += new System.EventHandler(this.ContrImages_Resize);
			this.panelNote.ResumeLayout(false);
			this.panelNote.PerformLayout();
			this.panelAcquire.ResumeLayout(false);
			this.panelImportAuto.ResumeLayout(false);
			this.panelImportAuto.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ToolBarOD toolBarMain;
		private System.Windows.Forms.ContextMenu menuForms;
		private UI.ToolBarOD toolBarPaint;
		private System.Windows.Forms.ContextMenu menuMounts;
		private UI.WindowingSlider windowingSlider;
		private System.Windows.Forms.ImageList imageListTools2;
		private OpenDental.UI.ControlDoubleBuffered panelMain;
		private System.Windows.Forms.ContextMenu menuTree;
		private System.Windows.Forms.MenuItem menuItemTreePrint;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.ImageList imageListTree;
		private UI.ZoomSlider zoomSlider;
		private System.Windows.Forms.Panel panelNote;
		private System.Windows.Forms.Label labelInvalidSig;
		private UI.SignatureBox sigBox;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Timer timerTreeClick;
		private System.Windows.Forms.ContextMenu menuMainPanel;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemPaste;
		private System.Windows.Forms.MenuItem menuItemFlipHoriz;
		private System.Windows.Forms.MenuItem menuItemRotateLeft;
		private System.Windows.Forms.MenuItem menuItemRotateRight;
		private System.Windows.Forms.MenuItem menuItemRotate180;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemInfo;
		private UI.PanelOD panelSplitter;
		private System.Windows.Forms.Panel panelAcquire;
		private UI.Button butCancel;
		private UI.Button butStart;
		private System.Windows.Forms.Label labelAcquireNotifications;
		private System.Windows.Forms.Panel panelAcquireColor;
		private UI.ComboBoxOD comboDevice;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panelImportAuto;
		private UI.Button butImportClose;
		private UI.Button butBrowse;
		private System.Windows.Forms.TextBox textImportAuto;
		private System.Windows.Forms.Label label3;
		private UI.Button butImportStart;
		private UI.Button butGoTo;
		private UI.ImageSelectorTemp imageSelector;
	}
}
