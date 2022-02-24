using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormImageViewer {
		private System.ComponentModel.IContainer components = null;

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
				if(backBuffGraph!=null) {
					backBuffGraph.Dispose();
					backBuffGraph=null;
				}
				if(backBuffer!=null) {
					backBuffer.Dispose();
					backBuffer=null;
				}
				if(renderImage!=null) {
					renderImage.Dispose();
					renderImage=null;
				}
				if(ImageCurrent!=null) {
					ImageCurrent.Dispose();
					ImageCurrent=null;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageViewer));
			this.imageListTools = new System.Windows.Forms.ImageList(this.components);
			this.PictureBox1 = new System.Windows.Forms.PictureBox();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// imageListTools
			// 
			this.imageListTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools.ImageStream")));
			this.imageListTools.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTools.Images.SetKeyName(0,"");
			this.imageListTools.Images.SetKeyName(1,"");
			// 
			// PictureBox1
			// 
			this.PictureBox1.BackColor = System.Drawing.SystemColors.Window;
			this.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PictureBox1.Location = new System.Drawing.Point(0,25);
			this.PictureBox1.Name = "PictureBox1";
			this.PictureBox1.Size = new System.Drawing.Size(903,673);
			this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.PictureBox1.TabIndex = 12;
			this.PictureBox1.TabStop = false;
			this.PictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
			this.PictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
			this.PictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListTools;
			this.ToolBarMain.Location = new System.Drawing.Point(0,0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(903,25);
			this.ToolBarMain.TabIndex = 11;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// FormImageViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(903,698);
			this.Controls.Add(this.PictureBox1);
			this.Controls.Add(this.ToolBarMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Image Viewer";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormImageViewer_Load);
			this.Resize += new System.EventHandler(this.FormImageViewer_Resize);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.PictureBox PictureBox1;
		private System.Windows.Forms.ImageList imageListTools;
	}
}
