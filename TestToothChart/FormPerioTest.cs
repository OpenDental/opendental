using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace TestToothChart {
	public partial class FormPerioTest:Form {
		public FormPerioTest() {
			InitializeComponent();
			//Assume that a hardware format on the default adapter using 16 bit color and 16 bit depth buffer
			//with 4X antialiasing will work for testing purposes.
			toothChart.DeviceFormat=new SparksToothChart.ToothChartDirectX.DirectXDeviceFormat("0;Hardware;32;D16;R5G6B5;FourSamples");
			toothChart.DrawMode=DrawingMode.DirectX;
		}

		private void FormPerioTest_Load(object sender,EventArgs e) {
			toothChart.ColorBackground=Color.White;
			toothChart.ColorText=Color.Black;
			toothChart.PerioMode=true;
			toothChart.ColorBleeding=butColorBleed.BackColor;
			toothChart.ColorSuppuration=butColorPus.BackColor;
			toothChart.ColorGingivalMargin=butColorGM.BackColor;
			toothChart.ColorCAL=butColorCAL.BackColor;
			toothChart.ColorMGJ=butColorMGJ.BackColor;
			toothChart.ColorProbing=butColorProbing.BackColor;
			toothChart.ColorProbingRed=butColorProbingRed.BackColor;
			toothChart.ColorFurcations=butColorFurc.BackColor;
			toothChart.ColorFurcationsRed=butColorFurcRed.BackColor;
			toothChart.RedLimitProbing=PIn.Int(labelRedLimitProbing.Text);
			toothChart.RedLimitFurcations=PIn.Int(labelRedLimitFurcations.Text);
			toothChart.SetMissing("13");
			toothChart.SetMissing("14");
			toothChart.SetMissing("18");
			toothChart.SetMissing("25");
			toothChart.SetMissing("26");
			toothChart.SetImplant("14",Color.Gray);
			//Movements are too low of a priority to test right now.  We might not even want to implement them.
			//toothChart.MoveTooth("4",0,0,0,0,-5,0);
			//toothChart.MoveTooth("16",0,20,0,-3,0,0);
			//toothChart.MoveTooth("24",15,2,0,0,0,0);
			toothChart.SetMobility("2","3",Color.Red);
			toothChart.SetMobility("7","2",Color.Red);
			toothChart.SetMobility("8","2",Color.Red);
			toothChart.SetMobility("9","2",Color.Red);
			toothChart.SetMobility("10","2",Color.Red);
			toothChart.SetMobility("16","3",Color.Red);
			toothChart.SetMobility("24","2",Color.Red);
			toothChart.SetMobility("31","3",Color.Red);
			toothChart.AddPerioMeasure(1,PerioSequenceType.Furcation,-1,2,-1,1,-1,-1);
			toothChart.AddPerioMeasure(2,PerioSequenceType.Furcation,-1,2,-1,1,-1,-1);
			toothChart.AddPerioMeasure(3,PerioSequenceType.Furcation,-1,2,-1,1,-1,-1);
			toothChart.AddPerioMeasure(5,PerioSequenceType.Furcation,1,-1,-1,-1,-1,-1);
			toothChart.AddPerioMeasure(30,PerioSequenceType.Furcation,-1,-1,-1,-1,3,-1);
			for(int i=1;i<=32;i++) {
				//string tooth_id=i.ToString();
				//bleeding and suppuration on all MB sites
				//bleeding only all DL sites
				//suppuration only all B sites
				//blood=1, suppuration=2, both=3
				toothChart.AddPerioMeasure(i,PerioSequenceType.Bleeding,  3,2,-1,-1,-1,1);
				toothChart.AddPerioMeasure(i,PerioSequenceType.GingMargin,0,1,1,1,0,0);
				toothChart.AddPerioMeasure(i,PerioSequenceType.Probing,   3,2,3,4,2,3);
				toothChart.AddPerioMeasure(i,PerioSequenceType.CAL,       3,3,4,5,2,3);//basically GingMargin+Probing, unless one of them is -1
				toothChart.AddPerioMeasure(i,PerioSequenceType.MGJ,       5,5,5,6,6,6);
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrintDocument pd2=new PrintDocument();
			pd2.PrintPage+=new PrintPageEventHandler(this.pd2_PrintPage);
			pd2.OriginAtMargins=true;
			pd2.DefaultPageSettings.Margins=new Margins(0,0,0,0);
			pd2.Print();
		}

		private void pd2_PrintPage(object sender,PrintPageEventArgs ev) {//raised for each page to be printed.
			Graphics g=ev.Graphics;
			Bitmap bitmap=toothChart.GetBitmap();
			g.DrawImage(bitmap,75,75,bitmap.Width,bitmap.Height);
		}

		private void butColorGM_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorCAL_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorMGJ_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorProbing_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorProbingRed_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorFurc_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void butColorFurcRed_Click(object sender,EventArgs e) {
			ShowColor(sender);
		}

		private void ShowColor(object sender) {
			Color color=((Button)sender).BackColor;
			int colorint=color.ToArgb();
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(colorint.ToString());
			msgbox.ShowDialog();
		}
	}
}
