namespace OpenDental {
	partial class UserControlSecurityTree {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlSecurityTree));
			this.treePermissions = new System.Windows.Forms.TreeView();
			this.imageListPerm = new System.Windows.Forms.ImageList(this.components);
			this.textXpos = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// treePermissions
			// 
			this.treePermissions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treePermissions.HideSelection = false;
			this.treePermissions.ImageIndex = 0;
			this.treePermissions.ImageList = this.imageListPerm;
			this.treePermissions.ItemHeight = 15;
			this.treePermissions.Location = new System.Drawing.Point(0, 0);
			this.treePermissions.Name = "treePermissions";
			this.treePermissions.SelectedImageIndex = 0;
			this.treePermissions.Size = new System.Drawing.Size(384, 662);
			this.treePermissions.TabIndex = 8;
			this.treePermissions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treePermissions_AfterSelect);
			this.treePermissions.DoubleClick += new System.EventHandler(this.treePermissions_DoubleClick);
			this.treePermissions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treePermissions_MouseDown);
			this.treePermissions.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treePermissions_MouseMove);
			// 
			// imageListPerm
			// 
			this.imageListPerm.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPerm.ImageStream")));
			this.imageListPerm.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListPerm.Images.SetKeyName(0, "grayBox.gif");
			this.imageListPerm.Images.SetKeyName(1, "checkBoxUnchecked.gif");
			this.imageListPerm.Images.SetKeyName(2, "checkBoxChecked.gif");
			this.imageListPerm.Images.SetKeyName(3, "checkBoxGreen.gif");
			// 
			// textXpos
			// 
			this.textXpos.Location = new System.Drawing.Point(300, 22);
			this.textXpos.Name = "textXpos";
			this.textXpos.Size = new System.Drawing.Size(68, 20);
			this.textXpos.TabIndex = 9;
			this.textXpos.Visible = false;
			// 
			// UserControlSecurityTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textXpos);
			this.Controls.Add(this.treePermissions);
			this.Name = "UserControlSecurityTree";
			this.Size = new System.Drawing.Size(384, 662);
			this.Load += new System.EventHandler(this.UserControlSecurityTree_Load);
			this.SizeChanged += new System.EventHandler(this.UserControlSecurityTree_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treePermissions;
		private System.Windows.Forms.ImageList imageListPerm;
		private System.Windows.Forms.TextBox textXpos;
	}
}
