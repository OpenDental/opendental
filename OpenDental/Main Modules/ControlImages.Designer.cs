namespace OpenDental
{
	partial class ControlImages
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlImages));
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
			this.panelImportAuto = new System.Windows.Forms.Panel();
			this.butGoTo = new OpenDental.UI.Button();
			this.butImportStart = new OpenDental.UI.Button();
			this.butImportClose = new OpenDental.UI.Button();
			this.butBrowse = new OpenDental.UI.Button();
			this.textImportAuto = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panelDraw = new System.Windows.Forms.Panel();
			this.toolBarDraw = new OpenDental.UI.ToolBarOD();
			this.elementHostImageSelector = new System.Windows.Forms.Integration.ElementHost();
			this.elementHostZoomSlider = new System.Windows.Forms.Integration.ElementHost();
			this.elementHostWindowingSlider = new System.Windows.Forms.Integration.ElementHost();
			this.panelSplitter = new OpenDental.UI.PanelOD();
			this.toolBarPaint = new OpenDental.UI.ToolBarOD();
			this.panelMain = new OpenDental.UI.ControlDoubleBuffered();
			this.elementHostUnmountedBar = new System.Windows.Forms.Integration.ElementHost();
			this.elementHostToolBarMain = new System.Windows.Forms.Integration.ElementHost();
			this.panelNote.SuspendLayout();
			this.panelImportAuto.SuspendLayout();
			this.panelDraw.SuspendLayout();
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
			this.panelNote.Location = new System.Drawing.Point(230, 494);
			this.panelNote.Name = "panelNote";
			this.panelNote.Size = new System.Drawing.Size(834, 93);
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
			// panelDraw
			// 
			this.panelDraw.Controls.Add(this.toolBarDraw);
			this.panelDraw.Location = new System.Drawing.Point(250, 161);
			this.panelDraw.Name = "panelDraw";
			this.panelDraw.Size = new System.Drawing.Size(1056, 25);
			this.panelDraw.TabIndex = 36;
			this.panelDraw.Visible = false;
			// 
			// toolBarDraw
			// 
			this.toolBarDraw.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarDraw.ImageList = this.imageListTools2;
			this.toolBarDraw.Location = new System.Drawing.Point(0, 0);
			this.toolBarDraw.Name = "toolBarDraw";
			this.toolBarDraw.Size = new System.Drawing.Size(817, 25);
			this.toolBarDraw.TabIndex = 53;
			this.toolBarDraw.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarDraw_ButtonClick);
			// 
			// elementHostImageSelector
			// 
			this.elementHostImageSelector.Location = new System.Drawing.Point(15, 123);
			this.elementHostImageSelector.Name = "elementHostImageSelector";
			this.elementHostImageSelector.Size = new System.Drawing.Size(200, 409);
			this.elementHostImageSelector.TabIndex = 38;
			this.elementHostImageSelector.Text = "elementHost1";
			this.elementHostImageSelector.Child = null;
			// 
			// elementHostZoomSlider
			// 
			this.elementHostZoomSlider.Location = new System.Drawing.Point(162, 25);
			this.elementHostZoomSlider.Name = "elementHostZoomSlider";
			this.elementHostZoomSlider.Size = new System.Drawing.Size(231, 25);
			this.elementHostZoomSlider.TabIndex = 39;
			this.elementHostZoomSlider.Text = "elementHost1";
			this.elementHostZoomSlider.Child = null;
			// 
			// elementHostWindowingSlider
			// 
			this.elementHostWindowingSlider.Location = new System.Drawing.Point(4, 27);
			this.elementHostWindowingSlider.Name = "elementHostWindowingSlider";
			this.elementHostWindowingSlider.Size = new System.Drawing.Size(154, 20);
			this.elementHostWindowingSlider.TabIndex = 169;
			this.elementHostWindowingSlider.Text = "elementHost1";
			this.elementHostWindowingSlider.Child = null;
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
			this.toolBarPaint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarPaint.ImageList = this.imageListTools2;
			this.toolBarPaint.Location = new System.Drawing.Point(393, 25);
			this.toolBarPaint.Name = "toolBarPaint";
			this.toolBarPaint.Size = new System.Drawing.Size(674, 25);
			this.toolBarPaint.TabIndex = 15;
			this.toolBarPaint.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarPaint_ButtonClick);
			// 
			// panelMain
			// 
			this.panelMain.AllowDrop = true;
			this.panelMain.BackColor = System.Drawing.Color.White;
			this.panelMain.Location = new System.Drawing.Point(254, 192);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(813, 240);
			this.panelMain.TabIndex = 20;
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
			// 
			// elementHostUnmountedBar
			// 
			this.elementHostUnmountedBar.Location = new System.Drawing.Point(250, 438);
			this.elementHostUnmountedBar.Name = "elementHostUnmountedBar";
			this.elementHostUnmountedBar.Size = new System.Drawing.Size(923, 50);
			this.elementHostUnmountedBar.TabIndex = 170;
			this.elementHostUnmountedBar.Text = "elementHost1";
			this.elementHostUnmountedBar.Child = null;
			// 
			// elementHostToolBarMain
			// 
			this.elementHostToolBarMain.Location = new System.Drawing.Point(3, -6);
			this.elementHostToolBarMain.Name = "elementHostToolBarMain";
			this.elementHostToolBarMain.Size = new System.Drawing.Size(1318, 25);
			this.elementHostToolBarMain.TabIndex = 171;
			this.elementHostToolBarMain.Text = "elementHost1";
			this.elementHostToolBarMain.Child = null;
			// 
			// ControlImages
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.elementHostToolBarMain);
			this.Controls.Add(this.elementHostUnmountedBar);
			this.Controls.Add(this.elementHostWindowingSlider);
			this.Controls.Add(this.elementHostZoomSlider);
			this.Controls.Add(this.elementHostImageSelector);
			this.Controls.Add(this.panelDraw);
			this.Controls.Add(this.panelImportAuto);
			this.Controls.Add(this.panelSplitter);
			this.Controls.Add(this.panelNote);
			this.Controls.Add(this.toolBarPaint);
			this.Controls.Add(this.panelMain);
			this.DoubleBuffered = true;
			this.Name = "ControlImages";
			this.Size = new System.Drawing.Size(1321, 595);
			this.Resize += new System.EventHandler(this.ContrImages_Resize);
			this.panelNote.ResumeLayout(false);
			this.panelNote.PerformLayout();
			this.panelImportAuto.ResumeLayout(false);
			this.panelImportAuto.PerformLayout();
			this.panelDraw.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ContextMenu menuForms;
		private UI.ToolBarOD toolBarPaint;
		private System.Windows.Forms.ContextMenu menuMounts;
		private System.Windows.Forms.ImageList imageListTools2;
		private OpenDental.UI.ControlDoubleBuffered panelMain;
		private System.Windows.Forms.ContextMenu menuTree;
		private System.Windows.Forms.MenuItem menuItemTreePrint;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.ImageList imageListTree;
		private System.Windows.Forms.Panel panelNote;
		private System.Windows.Forms.Label labelInvalidSig;
		private UI.SignatureBox sigBox;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textNote;
		private UI.PanelOD panelSplitter;
		private System.Windows.Forms.Panel panelImportAuto;
		private UI.Button butImportClose;
		private UI.Button butBrowse;
		private System.Windows.Forms.TextBox textImportAuto;
		private System.Windows.Forms.Label label3;
		private UI.Button butImportStart;
		private UI.Button butGoTo;
		private System.Windows.Forms.Panel panelDraw;
		private UI.ToolBarOD toolBarDraw;
		private System.Windows.Forms.Integration.ElementHost elementHostImageSelector;
		private System.Windows.Forms.Integration.ElementHost elementHostZoomSlider;
		private System.Windows.Forms.Integration.ElementHost elementHostWindowingSlider;
		private System.Windows.Forms.Integration.ElementHost elementHostUnmountedBar;
		private System.Windows.Forms.Integration.ElementHost elementHostToolBarMain;
	}
}
