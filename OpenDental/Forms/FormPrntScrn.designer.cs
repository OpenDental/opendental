using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPrntScrn {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrntScrn));
			this.label1 = new System.Windows.Forms.Label();
			this.printPreviewControl2 = new System.Windows.Forms.PrintPreviewControl();
			this.pd2 = new System.Drawing.Printing.PrintDocument();
			this.butCancel = new OpenDental.UI.Button();			
			this.textMouseX = new System.Windows.Forms.TextBox();
			this.textMouseY = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.butZoomIn = new OpenDental.UI.Button();
			this.butZoomOut = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(190,100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(416,23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Once this is functioning, you can preview the image here before printing";
			// 
			// printPreviewControl2
			// 
			this.printPreviewControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.printPreviewControl2.AutoZoom = false;
			this.printPreviewControl2.Location = new System.Drawing.Point(0,0);
			this.printPreviewControl2.Name = "printPreviewControl2";
			this.printPreviewControl2.Size = new System.Drawing.Size(842,538);
			this.printPreviewControl2.TabIndex = 1;
			this.printPreviewControl2.Zoom = 1;
			this.printPreviewControl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl2_MouseMove);
			this.printPreviewControl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl2_MouseDown);
			this.printPreviewControl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl2_MouseUp);
			// 
			// pd2
			// 
			this.pd2.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.pd2_PrintPage);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(884,759);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textMouseX
			// 
			this.textMouseX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textMouseX.Location = new System.Drawing.Point(274,763);
			this.textMouseX.Name = "textMouseX";
			this.textMouseX.Size = new System.Drawing.Size(56,20);
			this.textMouseX.TabIndex = 7;
			this.textMouseX.Visible = false;
			// 
			// textMouseY
			// 
			this.textMouseY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textMouseY.Location = new System.Drawing.Point(394,763);
			this.textMouseY.Name = "textMouseY";
			this.textMouseY.Size = new System.Drawing.Size(56,20);
			this.textMouseY.TabIndex = 8;
			this.textMouseY.Visible = false;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(209,764);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63,17);
			this.label2.TabIndex = 11;
			this.label2.Text = "Mouse X";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label2.Visible = false;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(334,763);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56,20);
			this.label3.TabIndex = 12;
			this.label3.Text = "MouseY";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label3.Visible = false;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(678,759);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75,26);
			this.butPrint.TabIndex = 13;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butZoomIn
			// 
			this.butZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butZoomIn.Image = global::OpenDental.Properties.Resources.butZoomIn;
			this.butZoomIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butZoomIn.Location = new System.Drawing.Point(502,759);
			this.butZoomIn.Name = "butZoomIn";
			this.butZoomIn.Size = new System.Drawing.Size(77,26);
			this.butZoomIn.TabIndex = 14;
			this.butZoomIn.Text = "&Zoom +";
			this.butZoomIn.Click += new System.EventHandler(this.butZoomIn_Click);
			// 
			// butZoomOut
			// 
			this.butZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butZoomOut.Image = global::OpenDental.Properties.Resources.butZoomOut;
			this.butZoomOut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butZoomOut.Location = new System.Drawing.Point(591,759);
			this.butZoomOut.Name = "butZoomOut";
			this.butZoomOut.Size = new System.Drawing.Size(75,26);
			this.butZoomOut.TabIndex = 15;
			this.butZoomOut.Text = "Zoom -";
			this.butZoomOut.Click += new System.EventHandler(this.butZoomOut_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(765,759);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75,26);
			this.butExport.TabIndex = 16;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// FormPrntScrn
			// 
			this.AcceptButton = this.butPrint;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(976,792);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.butZoomOut);
			this.Controls.Add(this.butZoomIn);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMouseY);
			this.Controls.Add(this.textMouseX);

			this.Controls.Add(this.printPreviewControl2);
			this.Controls.Add(this.label1);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrntScrn";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Prnt Scrn Tool";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormPrntScrn_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormPrntScrn_Layout);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Drawing.Printing.PrintDocument pd2;
		private System.Windows.Forms.PrintPreviewControl printPreviewControl2;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textMouseX;
		private System.Windows.Forms.TextBox textMouseY;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butZoomIn;
		private OpenDental.UI.Button butZoomOut;
		private OpenDental.UI.Button butExport;
	}
}
