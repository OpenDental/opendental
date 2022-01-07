namespace TestToothChart {
	partial class FormFullScreen {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			SparksToothChart.ToothChartData toothChartData1=new SparksToothChart.ToothChartData();
			this.toothChartForBig=new SparksToothChart.ToothChartWrapper();
			this.SuspendLayout();
			// 
			// toothChartForBig
			// 
			this.toothChartForBig.AutoFinish=false;
			this.toothChartForBig.ColorBackground=System.Drawing.Color.FromArgb(((int)(((byte)(150)))),((int)(((byte)(145)))),((int)(((byte)(152)))));
			this.toothChartForBig.Cursor=System.Windows.Forms.Cursors.Default;
			this.toothChartForBig.CursorTool=SparksToothChart.CursorTool.Pointer;
			this.toothChartForBig.Dock=System.Windows.Forms.DockStyle.Fill;
			this.toothChartForBig.DrawMode=OpenDentBusiness.DrawingMode.Simple2D;
			this.toothChartForBig.Location=new System.Drawing.Point(0,0);
			this.toothChartForBig.Name="toothChartForBig";
			this.toothChartForBig.PreferredPixelFormatNumber=0;
			this.toothChartForBig.Size=new System.Drawing.Size(1214,821);
			this.toothChartForBig.TabIndex=196;
			toothChartData1.SizeControl=new System.Drawing.Size(1214,821);
			this.toothChartForBig.TcData=toothChartData1;
			this.toothChartForBig.UseHardware=false;
			// 
			// FormFullScreen
			// 
			this.AutoScaleDimensions=new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize=new System.Drawing.Size(1214,821);
			this.Controls.Add(this.toothChartForBig);
			this.Name="FormFullScreen";
			this.StartPosition=System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text="FormFullScreen";
			this.WindowState=System.Windows.Forms.FormWindowState.Maximized;
			this.Load+=new System.EventHandler(this.FormFullScreen_Load);
			this.FormClosed+=new System.Windows.Forms.FormClosedEventHandler(this.FormFullScreen_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		public SparksToothChart.ToothChartWrapper toothChartForBig;



	}
}