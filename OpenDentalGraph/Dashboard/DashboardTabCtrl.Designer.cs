namespace OpenDentalGraph {
	partial class DashboardTabCtrl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardTabCtrl));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabImageList = new System.Windows.Forms.ImageList(this.components);
			this.butPrintPage = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.ImageList = this.tabImageList;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(222, 127);
			this.tabControl.TabIndex = 0;
			this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
			this.tabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseClick);
			this.tabControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseDoubleClick);
			this.tabControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseMove);
			// 
			// tabImageList
			// 
			this.tabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabImageList.ImageStream")));
			this.tabImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.tabImageList.Images.SetKeyName(0, "addTab11.png");
			this.tabImageList.Images.SetKeyName(1, "deleteTab11.png");
			this.tabImageList.Images.SetKeyName(2, "deleteTabHighlight11.png");
			// 
			// butPrintPage
			// 
			this.butPrintPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintPage.BackColor = System.Drawing.SystemColors.Control;
			this.butPrintPage.Location = new System.Drawing.Point(142, 1);
			this.butPrintPage.Name = "butPrintPage";
			this.butPrintPage.Size = new System.Drawing.Size(77, 21);
			this.butPrintPage.TabIndex = 9;
			this.butPrintPage.Text = "Print Page";
			this.butPrintPage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.butPrintPage.UseVisualStyleBackColor = false;
			this.butPrintPage.Click += new System.EventHandler(this.butPrintPage_Click);
			// 
			// DashboardTabCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butPrintPage);
			this.Controls.Add(this.tabControl);
			this.Name = "DashboardTabCtrl";
			this.Size = new System.Drawing.Size(222, 127);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.ImageList tabImageList;
		private System.Windows.Forms.Button butPrintPage;
	}
}
