using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPrintPreview {
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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPrintPreview));
			this._printPreviewControl2 = new OpenDental.UI.ODPrintPreviewControl();
			this.butNext = new OpenDental.UI.Button();
			this.butPrev = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// _printPreviewControl2
			// 
			this._printPreviewControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._printPreviewControl2.AutoZoom = false;
			this._printPreviewControl2.Location = new System.Drawing.Point(0, 0);
			this._printPreviewControl2.Name = "_printPreviewControl2";
			this._printPreviewControl2.Size = new System.Drawing.Size(842, 538);
			this._printPreviewControl2.TabIndex = 7;
			this._printPreviewControl2.Zoom = 1D;
			// 
			// butNext
			// 
			this.butNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNext.Location = new System.Drawing.Point(91, 716);
			this.butNext.Name = "butNext";
			this.butNext.Size = new System.Drawing.Size(75, 23);
			this.butNext.TabIndex = 8;
			this.butNext.Text = "Next Page";
			this.butNext.Click += new System.EventHandler(this.butNext_Click);
			// 
			// butPrev
			// 
			this.butPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrev.Location = new System.Drawing.Point(10, 716);
			this.butPrev.Name = "butPrev";
			this.butPrev.Size = new System.Drawing.Size(75, 23);
			this.butPrev.TabIndex = 9;
			this.butPrev.Text = "Prev. Page";
			this.butPrev.Click += new System.EventHandler(this.butPrev_Click);
			// 
			// FormRpPrintPreview
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(842, 746);
			this.Controls.Add(this.butPrev);
			this.Controls.Add(this.butNext);
			this.Controls.Add(this._printPreviewControl2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpPrintPreview";
			this.ShowInTaskbar = false;
			this.Text = "FormRpPrintPreview";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormRpPrintPreview_Load);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary></summary>
		private OpenDental.UI.ODPrintPreviewControl _printPreviewControl2;
		private OpenDental.UI.Button butNext;
		private OpenDental.UI.Button butPrev;
	}
}
