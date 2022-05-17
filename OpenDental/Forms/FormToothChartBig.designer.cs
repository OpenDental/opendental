using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SparksToothChart;

namespace OpenDental {
	public partial class FormToothChartingBig {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			SparksToothChart.ToothChartData toothChartData1 = new SparksToothChart.ToothChartData();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToothChartingBig));
			this.toothChartWrapper = new SparksToothChart.ToothChartWrapper();
			this.SuspendLayout();
			// 
			// toothChartWrapper
			// 
			this.toothChartWrapper.AutoFinish = false;
			this.toothChartWrapper.ColorBackground = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(145)))), ((int)(((byte)(152)))));
			this.toothChartWrapper.Cursor = System.Windows.Forms.Cursors.Default;
			this.toothChartWrapper.CursorTool = SparksToothChart.CursorTool.Pointer;
			this.toothChartWrapper.DeviceFormat = null;
			this.toothChartWrapper.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toothChartWrapper.DrawMode = OpenDentBusiness.DrawingMode.Simple2D;
			this.toothChartWrapper.Location = new System.Drawing.Point(0, 0);
			this.toothChartWrapper.Name = "toothChartWrapper";
			this.toothChartWrapper.PerioMode = false;
			this.toothChartWrapper.PreferredPixelFormatNumber = 0;
			this.toothChartWrapper.Size = new System.Drawing.Size(926, 858);
			this.toothChartWrapper.TabIndex = 0;
			toothChartData1.SizeControl = new System.Drawing.Size(926, 858);
			this.toothChartWrapper.TcData = toothChartData1;
			this.toothChartWrapper.UseHardware = false;
			this.toothChartWrapper.Visible = false;
			// 
			// FormToothChartingBig
			// 
			this.ClientSize = new System.Drawing.Size(926, 858);
			this.Controls.Add(this.toothChartWrapper);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormToothChartingBig";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormToothChartingBig_FormClosed);
			this.Load += new System.EventHandler(this.FormToothChartingBig_Load);
			this.ResizeEnd += new System.EventHandler(this.FormToothChartingBig_ResizeEnd);
			this.ResumeLayout(false);

		}
		#endregion

		private ToothChartWrapper toothChartWrapper;
	}
}
