namespace OpenDental {
	partial class ControlImageDisplay {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
				_bitmapRaw?.Dispose();
				if(_bitmapArrayShowing!=null && _bitmapArrayShowing.Length>0){
					for(int i=0;i<_bitmapArrayShowing.Length;i++){
						_bitmapArrayShowing[i]?.Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.menuPanelMain = new System.Windows.Forms.ContextMenu();
			this.menuItemCopy = new System.Windows.Forms.MenuItem();
			this.menuItemPaste = new System.Windows.Forms.MenuItem();
			this.menuItemFlipHoriz = new System.Windows.Forms.MenuItem();
			this.menuItemRotateLeft = new System.Windows.Forms.MenuItem();
			this.menuItemRotateRight = new System.Windows.Forms.MenuItem();
			this.menuItemRotate180 = new System.Windows.Forms.MenuItem();
			this.menuItemDelete = new System.Windows.Forms.MenuItem();
			this.menuItemInfo = new System.Windows.Forms.MenuItem();
			this.panelMain = new OpenDental.UI.ControlDoubleBuffered();
			this.SuspendLayout();
			// 
			// menuPanelMain
			// 
			this.menuPanelMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
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
			// panelMain
			// 
			this.panelMain.AllowDrop = true;
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(150, 170);
			this.panelMain.TabIndex = 1;
			this.panelMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelMain_DragDrop);
			this.panelMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMain_DragEnter);
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
			this.panelMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDoubleClick);
			this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
			this.panelMain.MouseLeave += new System.EventHandler(this.panelMain_MouseLeave);
			this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
			this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
			// 
			// ControlImageDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelMain);
			this.Name = "ControlImageDisplay";
			this.Size = new System.Drawing.Size(150, 170);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlImageDisplay_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ControlImageDisplay_KeyUp);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ContextMenu menuPanelMain;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemPaste;
		private System.Windows.Forms.MenuItem menuItemFlipHoriz;
		private System.Windows.Forms.MenuItem menuItemRotateLeft;
		private System.Windows.Forms.MenuItem menuItemRotateRight;
		private System.Windows.Forms.MenuItem menuItemRotate180;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemInfo;
		public UI.ControlDoubleBuffered panelMain;
	}
}
