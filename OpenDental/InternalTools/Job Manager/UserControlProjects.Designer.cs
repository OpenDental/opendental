namespace OpenDental {
	partial class UserControlProjects {
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
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlProjects));
			this.tabContr = new System.Windows.Forms.TabControl();
			this.tabMain = new System.Windows.Forms.TabPage();
			this.tree = new System.Windows.Forms.TreeView();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.menuEdit = new System.Windows.Forms.ContextMenu();
			this.menuItemDone = new System.Windows.Forms.MenuItem();
			this.menuItemEdit = new System.Windows.Forms.MenuItem();
			this.menuItemDelete = new System.Windows.Forms.MenuItem();
			this.checkShowFinished = new System.Windows.Forms.CheckBox();
			this.timerDoneTaskListRefresh = new System.Windows.Forms.Timer(this.components);
			this.gridMain = new OpenDental.UI.ODGrid();
			this.ToolBarMain = new OpenDental.UI.ODToolBar();
			this.tabContr.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabContr
			// 
			this.tabContr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabContr.Controls.Add(this.tabMain);
			this.tabContr.Location = new System.Drawing.Point(0, 26);
			this.tabContr.Name = "tabContr";
			this.tabContr.SelectedIndex = 0;
			this.tabContr.Size = new System.Drawing.Size(804, 23);
			this.tabContr.TabIndex = 5;
			this.tabContr.Click += new System.EventHandler(this.tabContr_Click);
			// 
			// tabMain
			// 
			this.tabMain.Location = new System.Drawing.Point(4, 22);
			this.tabMain.Name = "tabMain";
			this.tabMain.Size = new System.Drawing.Size(796, 0);
			this.tabMain.TabIndex = 0;
			this.tabMain.Text = "Main";
			this.tabMain.UseVisualStyleBackColor = true;
			// 
			// tree
			// 
			this.tree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tree.HideSelection = false;
			this.tree.ImageIndex = 0;
			this.tree.ImageList = this.imageListTree;
			this.tree.ItemHeight = 18;
			this.tree.Location = new System.Drawing.Point(0, 48);
			this.tree.Name = "tree";
			this.tree.Scrollable = false;
			this.tree.SelectedImageIndex = 0;
			this.tree.ShowPlusMinus = false;
			this.tree.Size = new System.Drawing.Size(804, 98);
			this.tree.TabIndex = 7;
			this.tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDown);
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "TaskList.gif");
			this.imageListTree.Images.SetKeyName(1, "checkBoxChecked.gif");
			this.imageListTree.Images.SetKeyName(2, "checkBoxUnchecked.gif");
			this.imageListTree.Images.SetKeyName(3, "TaskListHighlight.gif");
			this.imageListTree.Images.SetKeyName(4, "checkBoxNew.gif");
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "TaskListAdd.gif");
			this.imageListMain.Images.SetKeyName(1, "Add.gif");
			// 
			// menuEdit
			// 
			this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDone,
            this.menuItemEdit,
            this.menuItemDelete});
			// 
			// menuItemDone
			// 
			this.menuItemDone.Index = 0;
			this.menuItemDone.Text = "Set Complete";
			this.menuItemDone.Click += new System.EventHandler(this.menuItemDone_Click);
			// 
			// menuItemEdit
			// 
			this.menuItemEdit.Index = 1;
			this.menuItemEdit.Text = "Edit";
			this.menuItemEdit.Click += new System.EventHandler(this.menuItemEdit_Click);
			// 
			// menuItemDelete
			// 
			this.menuItemDelete.Index = 2;
			this.menuItemDelete.Text = "Delete";
			this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
			// 
			// checkShowFinished
			// 
			this.checkShowFinished.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkShowFinished.Location = new System.Drawing.Point(2, 149);
			this.checkShowFinished.Name = "checkShowFinished";
			this.checkShowFinished.Size = new System.Drawing.Size(151, 15);
			this.checkShowFinished.TabIndex = 10;
			this.checkShowFinished.Text = "Show Finished";
			this.checkShowFinished.UseVisualStyleBackColor = true;
			this.checkShowFinished.Click += new System.EventHandler(this.checkShowFinished_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasMultilineHeaders = false;
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(0, 147);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.Size = new System.Drawing.Size(804, 403);
			this.gridMain.TabIndex = 9;
			this.gridMain.Title = "Projects";
			this.gridMain.TranslationName = "TableJobs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseDown);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(804, 25);
			this.ToolBarMain.TabIndex = 0;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// UserControlProjects
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.checkShowFinished);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.tree);
			this.Controls.Add(this.tabContr);
			this.Name = "UserControlProjects";
			this.Size = new System.Drawing.Size(804, 550);
			this.Load += new System.EventHandler(this.UserControlProjects_Load);
			this.tabContr.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.ODToolBar ToolBarMain;
		private System.Windows.Forms.TabControl tabContr;
		private System.Windows.Forms.TabPage tabMain;
		private System.Windows.Forms.TreeView tree;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.ContextMenu menuEdit;
		private System.Windows.Forms.MenuItem menuItemEdit;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.ImageList imageListTree;
		private OpenDental.UI.ODGrid gridMain;
		private System.Windows.Forms.CheckBox checkShowFinished;
		private System.Windows.Forms.MenuItem menuItemDone;
		private System.Windows.Forms.Timer timerDoneTaskListRefresh;
		private UI.ODToolBar odToolBar1;
	}
}
