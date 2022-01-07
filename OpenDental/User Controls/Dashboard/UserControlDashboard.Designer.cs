namespace OpenDental {
	partial class UserControlDashboard {
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
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemClose,
            this.menuItemRefresh});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(114, 48);
			// 
			// menuItemClose
			// 
			this.menuItemClose.Name = "menuItemClose";
			this.menuItemClose.Size = new System.Drawing.Size(113, 22);
			this.menuItemClose.Text = "Close";
			this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
			// 
			// menuItemRefresh
			// 
			this.menuItemRefresh.Name = "menuItemRefresh";
			this.menuItemRefresh.Size = new System.Drawing.Size(113, 22);
			this.menuItemRefresh.Text = "Refresh";
			this.menuItemRefresh.Click += new System.EventHandler(this.MenuItemRefresh_Click);
			// 
			// UserControlDashboard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Name = "UserControlDashboard";
			this.Size = new System.Drawing.Size(148, 148);
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemClose;
		private System.Windows.Forms.ToolStripMenuItem menuItemRefresh;
	}
}
