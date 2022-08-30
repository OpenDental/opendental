using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparksToothChart;
using OpenDentBusiness;

namespace TestToothChart {
	public partial class Form1:Form {
		public Form1() {
			InitializeComponent();
			toothChart2D.DrawMode=DrawingMode.Simple2D;
			toothChartOpenGL.PreferredPixelFormatNumber=7;
			toothChartOpenGL.DrawMode=DrawingMode.OpenGL;
			toothChartDirectX.DeviceFormat=new SparksToothChart.ToothChartDirectX.DirectXDeviceFormat("0;Hardware;32;D16;R5G6B5;FourSamples");
			toothChartDirectX.DrawMode=DrawingMode.DirectX;
		}

		private void Form1_Load(object sender,EventArgs e) {

		}

		private void butReset_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
		}

		private void butAllPrimary_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetPrimary("1");
			toothChart2D.SetPrimary("2");
			toothChart2D.SetPrimary("3");
			toothChart2D.SetPrimary("4");
			toothChart2D.SetPrimary("5");
			toothChart2D.SetPrimary("6");
			toothChart2D.SetPrimary("7");
			toothChart2D.SetPrimary("8");
			toothChart2D.SetPrimary("9");
			toothChart2D.SetPrimary("10");
			toothChart2D.SetPrimary("11");
			toothChart2D.SetPrimary("12");
			toothChart2D.SetPrimary("13");
			toothChart2D.SetPrimary("14");
			toothChart2D.SetPrimary("15");
			toothChart2D.SetPrimary("16");
			toothChart2D.SetPrimary("17");
			toothChart2D.SetPrimary("18");
			toothChart2D.SetPrimary("19");
			toothChart2D.SetPrimary("20");
			toothChart2D.SetPrimary("21");
			toothChart2D.SetPrimary("22");
			toothChart2D.SetPrimary("23");
			toothChart2D.SetPrimary("24");
			toothChart2D.SetPrimary("25");
			toothChart2D.SetPrimary("26");
			toothChart2D.SetPrimary("27");
			toothChart2D.SetPrimary("28");
			toothChart2D.SetPrimary("29");
			toothChart2D.SetPrimary("30");
			toothChart2D.SetPrimary("31");
			toothChart2D.SetPrimary("32");
			//
			toothChartOpenGL.SetPrimary("1");
			toothChartOpenGL.SetPrimary("2");
			toothChartOpenGL.SetPrimary("3");
			toothChartOpenGL.SetPrimary("4");
			toothChartOpenGL.SetPrimary("5");
			toothChartOpenGL.SetPrimary("6");
			toothChartOpenGL.SetPrimary("7");
			toothChartOpenGL.SetPrimary("8");
			toothChartOpenGL.SetPrimary("9");
			toothChartOpenGL.SetPrimary("10");
			toothChartOpenGL.SetPrimary("11");
			toothChartOpenGL.SetPrimary("12");
			toothChartOpenGL.SetPrimary("13");
			toothChartOpenGL.SetPrimary("14");
			toothChartOpenGL.SetPrimary("15");
			toothChartOpenGL.SetPrimary("16");
			toothChartOpenGL.SetPrimary("17");
			toothChartOpenGL.SetPrimary("18");
			toothChartOpenGL.SetPrimary("19");
			toothChartOpenGL.SetPrimary("20");
			toothChartOpenGL.SetPrimary("21");
			toothChartOpenGL.SetPrimary("22");
			toothChartOpenGL.SetPrimary("23");
			toothChartOpenGL.SetPrimary("24");
			toothChartOpenGL.SetPrimary("25");
			toothChartOpenGL.SetPrimary("26");
			toothChartOpenGL.SetPrimary("27");
			toothChartOpenGL.SetPrimary("28");
			toothChartOpenGL.SetPrimary("29");
			toothChartOpenGL.SetPrimary("30");
			toothChartOpenGL.SetPrimary("31");
			toothChartOpenGL.SetPrimary("32");
			//
			toothChartDirectX.SetPrimary("1");
			toothChartDirectX.SetPrimary("2");
			toothChartDirectX.SetPrimary("3");
			toothChartDirectX.SetPrimary("4");
			toothChartDirectX.SetPrimary("5");
			toothChartDirectX.SetPrimary("6");
			toothChartDirectX.SetPrimary("7");
			toothChartDirectX.SetPrimary("8");
			toothChartDirectX.SetPrimary("9");
			toothChartDirectX.SetPrimary("10");
			toothChartDirectX.SetPrimary("11");
			toothChartDirectX.SetPrimary("12");
			toothChartDirectX.SetPrimary("13");
			toothChartDirectX.SetPrimary("14");
			toothChartDirectX.SetPrimary("15");
			toothChartDirectX.SetPrimary("16");
			toothChartDirectX.SetPrimary("17");
			toothChartDirectX.SetPrimary("18");
			toothChartDirectX.SetPrimary("19");
			toothChartDirectX.SetPrimary("20");
			toothChartDirectX.SetPrimary("21");
			toothChartDirectX.SetPrimary("22");
			toothChartDirectX.SetPrimary("23");
			toothChartDirectX.SetPrimary("24");
			toothChartDirectX.SetPrimary("25");
			toothChartDirectX.SetPrimary("26");
			toothChartDirectX.SetPrimary("27");
			toothChartDirectX.SetPrimary("28");
			toothChartDirectX.SetPrimary("29");
			toothChartDirectX.SetPrimary("30");
			toothChartDirectX.SetPrimary("31");
			toothChartDirectX.SetPrimary("32");
		}

		private void butMixed_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetPrimary("1");
			toothChart2D.SetPrimary("2");
			toothChart2D.SetPrimary("4");
			toothChart2D.SetPrimary("5");
			toothChart2D.SetPrimary("6");
			toothChart2D.SetPrimary("11");
			toothChart2D.SetPrimary("12");
			toothChart2D.SetPrimary("13");
			toothChart2D.SetPrimary("15");
			toothChart2D.SetPrimary("16");
			toothChart2D.SetPrimary("17");
			toothChart2D.SetPrimary("18");
			toothChart2D.SetPrimary("20");
			toothChart2D.SetPrimary("21");
			toothChart2D.SetPrimary("22");
			toothChart2D.SetPrimary("27");
			toothChart2D.SetPrimary("28");
			toothChart2D.SetPrimary("29");
			toothChart2D.SetPrimary("31");
			toothChart2D.SetPrimary("32");
			//
			toothChartOpenGL.SetPrimary("1");
			toothChartOpenGL.SetPrimary("2");
			toothChartOpenGL.SetPrimary("4");
			toothChartOpenGL.SetPrimary("5");
			toothChartOpenGL.SetPrimary("6");
			toothChartOpenGL.SetPrimary("11");
			toothChartOpenGL.SetPrimary("12");
			toothChartOpenGL.SetPrimary("13");
			toothChartOpenGL.SetPrimary("15");
			toothChartOpenGL.SetPrimary("16");
			toothChartOpenGL.SetPrimary("17");
			toothChartOpenGL.SetPrimary("18");
			toothChartOpenGL.SetPrimary("20");
			toothChartOpenGL.SetPrimary("21");
			toothChartOpenGL.SetPrimary("22");
			toothChartOpenGL.SetPrimary("27");
			toothChartOpenGL.SetPrimary("28");
			toothChartOpenGL.SetPrimary("29");
			toothChartOpenGL.SetPrimary("31");
			toothChartOpenGL.SetPrimary("32");
			//
			toothChartDirectX.SetPrimary("1");
			toothChartDirectX.SetPrimary("2");
			toothChartDirectX.SetPrimary("4");
			toothChartDirectX.SetPrimary("5");
			toothChartDirectX.SetPrimary("6");
			toothChartDirectX.SetPrimary("11");
			toothChartDirectX.SetPrimary("12");
			toothChartDirectX.SetPrimary("13");
			toothChartDirectX.SetPrimary("15");
			toothChartDirectX.SetPrimary("16");
			toothChartDirectX.SetPrimary("17");
			toothChartDirectX.SetPrimary("18");
			toothChartDirectX.SetPrimary("20");
			toothChartDirectX.SetPrimary("21");
			toothChartDirectX.SetPrimary("22");
			toothChartDirectX.SetPrimary("27");
			toothChartDirectX.SetPrimary("28");
			toothChartDirectX.SetPrimary("29");
			toothChartDirectX.SetPrimary("31");
			toothChartDirectX.SetPrimary("32");
		}

		private void panelColorBackgroundGray_Click(object sender,EventArgs e) {
			toothChartOpenGL.ColorBackground=panelColorBackgroundGray.BackColor;
			toothChartDirectX.ColorBackground=panelColorBackgroundGray.BackColor;
		}

		private void panelColorBackgroundLtGray_Click(object sender,EventArgs e) {
			toothChartOpenGL.ColorBackground=panelColorBackgroundLtGray.BackColor;
			toothChartDirectX.ColorBackground=panelColorBackgroundLtGray.BackColor;
		}

		private void panelColorBackgroundBlack_Click(object sender,EventArgs e) {
			toothChartOpenGL.ColorBackground=panelColorBackgroundBlack.BackColor;
			toothChartDirectX.ColorBackground=panelColorBackgroundBlack.BackColor;
		}

		private void panelColorBackgroundWhite_Click(object sender,EventArgs e) {
			toothChartOpenGL.ColorBackground=panelColorBackgroundWhite.BackColor;
			toothChartDirectX.ColorBackground=panelColorBackgroundWhite.BackColor;
		}

		private void panelColorBackgroundBlue_Click(object sender,EventArgs e) {
			toothChartOpenGL.ColorBackground=panelColorBackgroundBlue.BackColor;
			toothChartDirectX.ColorBackground=panelColorBackgroundBlue.BackColor;
		}

		private void panelColorTextGray_Click(object sender,EventArgs e) {
			toothChart2D.ColorText=panelColorTextGray.BackColor;
			toothChartOpenGL.ColorText=panelColorTextGray.BackColor;
			toothChartDirectX.ColorText=panelColorTextGray.BackColor;
		}

		private void panelColorTextBlack_Click(object sender,EventArgs e) {
			toothChart2D.ColorText=panelColorTextBlack.BackColor;
			toothChartOpenGL.ColorText=panelColorTextBlack.BackColor;
			toothChartDirectX.ColorText=panelColorTextBlack.BackColor;
		}

		private void panelColorTextWhite_Click(object sender,EventArgs e) {
			toothChart2D.ColorText=panelColorTextWhite.BackColor;
			toothChartOpenGL.ColorText=panelColorTextWhite.BackColor;
			toothChartDirectX.ColorText=panelColorTextWhite.BackColor;
		}

		private void panelColorTextHighlightGray_Click(object sender,EventArgs e) {
			toothChart2D.ColorTextHighlight=panelColorTextHighlightGray.BackColor;
			toothChartOpenGL.ColorTextHighlight=panelColorTextHighlightGray.BackColor;
			toothChartDirectX.ColorTextHighlight=panelColorTextHighlightGray.BackColor;
		}

		private void panelColorTextHighlightBlack_Click(object sender,EventArgs e) {
			toothChart2D.ColorTextHighlight=panelColorTextHighlightBlack.BackColor;
			toothChartOpenGL.ColorTextHighlight=panelColorTextHighlightBlack.BackColor;
			toothChartDirectX.ColorTextHighlight=panelColorTextHighlightBlack.BackColor;
		}

		private void panelColorTextHighlightWhite_Click(object sender,EventArgs e) {
			toothChart2D.ColorTextHighlight=panelColorTextHighlightWhite.BackColor;
			toothChartOpenGL.ColorTextHighlight=panelColorTextHighlightWhite.BackColor;
			toothChartDirectX.ColorTextHighlight=panelColorTextHighlightWhite.BackColor;
		}

		private void panelColorTextHighlightRed_Click(object sender,EventArgs e) {
			toothChart2D.ColorTextHighlight=panelColorTextHighlightRed.BackColor;
			toothChartOpenGL.ColorTextHighlight=panelColorTextHighlightRed.BackColor;
			toothChartDirectX.ColorTextHighlight=panelColorTextHighlightRed.BackColor;
		}

		private void panelColorBackHighlightGray_Click(object sender,EventArgs e) {
			toothChart2D.ColorBackHighlight=panelColorBackHighlightGray.BackColor;
			toothChartOpenGL.ColorBackHighlight=panelColorBackHighlightGray.BackColor;
			toothChartDirectX.ColorBackHighlight=panelColorBackHighlightGray.BackColor;
		}

		private void panelColorBackHighlightBlack_Click(object sender,EventArgs e) {
			toothChart2D.ColorBackHighlight=panelColorBackHighlightBlack.BackColor;
			toothChartOpenGL.ColorBackHighlight=panelColorBackHighlightBlack.BackColor;
			toothChartDirectX.ColorBackHighlight=panelColorBackHighlightBlack.BackColor;
		}

		private void panelColorBackHighlightWhite_Click(object sender,EventArgs e) {
			toothChart2D.ColorBackHighlight=panelColorBackHighlightWhite.BackColor;
			toothChartOpenGL.ColorBackHighlight=panelColorBackHighlightWhite.BackColor;
			toothChartDirectX.ColorBackHighlight=panelColorBackHighlightWhite.BackColor;
		}

		private void panelColorBackHighlightBlue_Click(object sender,EventArgs e) {
			toothChart2D.ColorBackHighlight=panelColorBackHighlightBlue.BackColor;
			toothChartOpenGL.ColorBackHighlight=panelColorBackHighlightBlue.BackColor;
			toothChartDirectX.ColorBackHighlight=panelColorBackHighlightBlue.BackColor;
		}

		private void butSizeNormal_Click(object sender,EventArgs e) {
			toothChart2D.Size=new Size(410,307);
			toothChartOpenGL.Size=new Size(410,307);
			toothChartDirectX.Size=new Size(410,307);
			toothChart2D.Location=new Point(8,28);
			toothChartOpenGL.Location=new Point(424,28);
			toothChartDirectX.Location=new Point(840,28);
		}

		private void butSizeTall_Click(object sender,EventArgs e) {
			toothChart2D.Size=new Size(250,307);
			toothChartOpenGL.Size=new Size(250,307);
			toothChartDirectX.Size=new Size(250,307);
		}

		private void butSizeWide_Click(object sender,EventArgs e) {
			toothChart2D.Size=new Size(410,190);
			toothChartOpenGL.Size=new Size(410,190);
			toothChartDirectX.Size=new Size(410,190);
		}

		private void butVeryTall_Click(object sender,EventArgs e) {
			toothChart2D.Size=new Size(295,700);
			toothChartOpenGL.Size=new Size(295,700);
			toothChartDirectX.Size=new Size(295,700);
			toothChart2D.Location=new Point(200,28);
			toothChartOpenGL.Location=new Point(500,28);
			toothChartDirectX.Location=new Point(800,28);
		}

		private void butVeryWide_Click(object sender,EventArgs e) {
			toothChart2D.Size=new Size(1200,200);
			toothChartOpenGL.Size=new Size(1200,200);
			toothChartDirectX.Size=new Size(1200,200);
			toothChart2D.Location=new Point(5,5);
			toothChartOpenGL.Location=new Point(5,210);
			toothChartDirectX.Location=new Point(5,415);
		}

		private void butMissing_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//toothChart2D.  pointless
			toothChartOpenGL.SetMissing("1");
			toothChartOpenGL.SetMissing("3");
			toothChartOpenGL.SetMissing("5");
			toothChartOpenGL.SetMissing("7");
			toothChartOpenGL.SetMissing("9");
			toothChartOpenGL.SetMissing("11");
			toothChartOpenGL.SetMissing("13");
			toothChartOpenGL.SetMissing("15");
			toothChartOpenGL.SetMissing("17");
			toothChartOpenGL.SetMissing("19");
			toothChartOpenGL.SetMissing("21");
			toothChartOpenGL.SetMissing("23");
			toothChartOpenGL.SetMissing("25");
			toothChartOpenGL.SetMissing("27");
			toothChartOpenGL.SetMissing("29");
			toothChartOpenGL.SetMissing("31");
			//
			toothChartDirectX.SetMissing("1");
			toothChartDirectX.SetMissing("3");
			toothChartDirectX.SetMissing("5");
			toothChartDirectX.SetMissing("7");
			toothChartDirectX.SetMissing("9");
			toothChartDirectX.SetMissing("11");
			toothChartDirectX.SetMissing("13");
			toothChartDirectX.SetMissing("15");
			toothChartDirectX.SetMissing("17");
			toothChartDirectX.SetMissing("19");
			toothChartDirectX.SetMissing("21");
			toothChartDirectX.SetMissing("23");
			toothChartDirectX.SetMissing("25");
			toothChartDirectX.SetMissing("27");
			toothChartDirectX.SetMissing("29");
			toothChartDirectX.SetMissing("31");
		}

		private void butHidden_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetHidden("4");
			toothChart2D.SetHidden("5");
			toothChart2D.SetHidden("12");
			toothChart2D.SetHidden("13");
			toothChart2D.SetHidden("20");
			toothChart2D.SetHidden("21");
			toothChart2D.SetHidden("28");
			toothChart2D.SetHidden("29");
			//
			toothChartOpenGL.SetHidden("4");
			toothChartOpenGL.SetHidden("5");
			toothChartOpenGL.SetHidden("12");
			toothChartOpenGL.SetHidden("13");
			toothChartOpenGL.SetHidden("20");
			toothChartOpenGL.SetHidden("21");
			toothChartOpenGL.SetHidden("28");
			toothChartOpenGL.SetHidden("29");
			//
			toothChartDirectX.SetHidden("4");
			toothChartDirectX.SetHidden("5");
			toothChartDirectX.SetHidden("12");
			toothChartDirectX.SetHidden("13");
			toothChartDirectX.SetHidden("20");
			toothChartDirectX.SetHidden("21");
			toothChartDirectX.SetHidden("28");
			toothChartDirectX.SetHidden("29");
		}

		private void butMissingHiddenComplex_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetPrimary("4");
			toothChart2D.SetPrimary("5");
			toothChart2D.SetPrimary("6");
			toothChart2D.SetPrimary("7");
			toothChart2D.SetPrimary("8");
			toothChart2D.SetPrimary("9");
			toothChart2D.SetPrimary("10");
			toothChart2D.SetPrimary("11");
			toothChart2D.SetPrimary("12");
			toothChart2D.SetPrimary("13");
			toothChart2D.SetPrimary("14");
			toothChart2D.SetPrimary("15");
			toothChart2D.SetPrimary("16");
			toothChart2D.SetMissing("4");
			toothChart2D.SetMissing("B");
			toothChart2D.SetMissing("6");
			toothChart2D.SetMissing("D");
			toothChart2D.SetHidden("G");
			toothChart2D.SetHidden("11");
			toothChart2D.SetHidden("I");
			toothChart2D.SetHidden("13");
			toothChart2D.SetHidden("14");
			//bottom
			toothChart2D.SetPrimary("29");
			toothChart2D.SetPrimary("28");
			toothChart2D.SetPrimary("27");
			toothChart2D.SetPrimary("26");
			toothChart2D.SetPrimary("25");
			toothChart2D.SetPrimary("24");
			toothChart2D.SetPrimary("23");
			toothChart2D.SetPrimary("22");
			toothChart2D.SetPrimary("21");
			toothChart2D.SetPrimary("20");
			toothChart2D.SetPrimary("19");
			toothChart2D.SetPrimary("18");
			toothChart2D.SetPrimary("17");
			toothChart2D.SetMissing("29");
			toothChart2D.SetMissing("S");
			toothChart2D.SetMissing("27");
			toothChart2D.SetMissing("Q");
			toothChart2D.SetHidden("N");
			toothChart2D.SetHidden("22");
			toothChart2D.SetHidden("L");
			toothChart2D.SetHidden("20");
			toothChart2D.SetHidden("19");
			//
			toothChartOpenGL.SetPrimary("4");
			toothChartOpenGL.SetPrimary("5");
			toothChartOpenGL.SetPrimary("6");
			toothChartOpenGL.SetPrimary("7");
			toothChartOpenGL.SetPrimary("8");
			toothChartOpenGL.SetPrimary("9");
			toothChartOpenGL.SetPrimary("10");
			toothChartOpenGL.SetPrimary("11");
			toothChartOpenGL.SetPrimary("12");
			toothChartOpenGL.SetPrimary("13");
			toothChartOpenGL.SetPrimary("14");
			toothChartOpenGL.SetPrimary("15");
			toothChartOpenGL.SetPrimary("16");
			toothChartOpenGL.SetMissing("4");
			toothChartOpenGL.SetMissing("B");
			toothChartOpenGL.SetMissing("6");
			toothChartOpenGL.SetMissing("D");
			toothChartOpenGL.SetHidden("G");
			toothChartOpenGL.SetHidden("11");
			toothChartOpenGL.SetHidden("I");
			toothChartOpenGL.SetHidden("13");
			toothChartOpenGL.SetHidden("14");
			//mirror image on the bottom:
			toothChartOpenGL.SetPrimary("29");
			toothChartOpenGL.SetPrimary("28");
			toothChartOpenGL.SetPrimary("27");
			toothChartOpenGL.SetPrimary("26");
			toothChartOpenGL.SetPrimary("25");
			toothChartOpenGL.SetPrimary("24");
			toothChartOpenGL.SetPrimary("23");
			toothChartOpenGL.SetPrimary("22");
			toothChartOpenGL.SetPrimary("21");
			toothChartOpenGL.SetPrimary("20");
			toothChartOpenGL.SetPrimary("19");
			toothChartOpenGL.SetPrimary("18");
			toothChartOpenGL.SetPrimary("17");
			toothChartOpenGL.SetMissing("29");
			toothChartOpenGL.SetMissing("S");
			toothChartOpenGL.SetMissing("27");
			toothChartOpenGL.SetMissing("Q");
			toothChartOpenGL.SetHidden("N");
			toothChartOpenGL.SetHidden("22");
			toothChartOpenGL.SetHidden("L");
			toothChartOpenGL.SetHidden("20");
			toothChartOpenGL.SetHidden("19");
			//
			toothChartDirectX.SetPrimary("4");
			toothChartDirectX.SetPrimary("5");
			toothChartDirectX.SetPrimary("6");
			toothChartDirectX.SetPrimary("7");
			toothChartDirectX.SetPrimary("8");
			toothChartDirectX.SetPrimary("9");
			toothChartDirectX.SetPrimary("10");
			toothChartDirectX.SetPrimary("11");
			toothChartDirectX.SetPrimary("12");
			toothChartDirectX.SetPrimary("13");
			toothChartDirectX.SetPrimary("14");
			toothChartDirectX.SetPrimary("15");
			toothChartDirectX.SetPrimary("16");
			toothChartDirectX.SetMissing("4");
			toothChartDirectX.SetMissing("B");
			toothChartDirectX.SetMissing("6");
			toothChartDirectX.SetMissing("D");
			toothChartDirectX.SetHidden("G");
			toothChartDirectX.SetHidden("11");
			toothChartDirectX.SetHidden("I");
			toothChartDirectX.SetHidden("13");
			toothChartDirectX.SetHidden("14");
			//bottom
			toothChartDirectX.SetPrimary("29");
			toothChartDirectX.SetPrimary("28");
			toothChartDirectX.SetPrimary("27");
			toothChartDirectX.SetPrimary("26");
			toothChartDirectX.SetPrimary("25");
			toothChartDirectX.SetPrimary("24");
			toothChartDirectX.SetPrimary("23");
			toothChartDirectX.SetPrimary("22");
			toothChartDirectX.SetPrimary("21");
			toothChartDirectX.SetPrimary("20");
			toothChartDirectX.SetPrimary("19");
			toothChartDirectX.SetPrimary("18");
			toothChartDirectX.SetPrimary("17");
			toothChartDirectX.SetMissing("29");
			toothChartDirectX.SetMissing("S");
			toothChartDirectX.SetMissing("27");
			toothChartDirectX.SetMissing("Q");
			toothChartDirectX.SetHidden("N");
			toothChartDirectX.SetHidden("22");
			toothChartDirectX.SetHidden("L");
			toothChartDirectX.SetHidden("20");
			toothChartDirectX.SetHidden("19");
		}

		private void butWatches_Click(object sender,EventArgs e) {
			//toothChart2D.ResetTeeth();
			//toothChartOpenGL.ResetTeeth();
			//toothChartDirectX.ResetTeeth();
			for(int i=1;i<=32;i++){
				string toothNum=i.ToString();
				toothChart2D.SetWatch(toothNum,Color.Red);
				toothChartOpenGL.SetWatch(toothNum,Color.Red);
				toothChartDirectX.SetWatch(toothNum,Color.Red);
			}
		}

		private void butFillings_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetCrown("1",Color.DarkGreen);
			toothChart2D.SetSurfaceColors("3","MOD",Color.DarkRed);
			toothChart2D.SetSurfaceColors("4","V",Color.Green);
			toothChart2D.SetSurfaceColors("5","B",Color.Green);
			toothChart2D.SetSurfaceColors("6","FIL",Color.DarkRed);
			toothChart2D.SetPrimary("11");
			toothChart2D.SetMissing("11");
			toothChart2D.SetSurfaceColors("H","MOD",Color.DarkRed);//some invalid surfaces
			toothChart2D.SetPrimary("12");
			toothChart2D.SetSurfaceColors("I","MOD",Color.DarkRed);
			toothChart2D.SetSurfaceColors("J","O",Color.DarkRed);//should not show
			toothChart2D.SetHidden("14");
			toothChart2D.SetSurfaceColors("14","MOD",Color.DarkRed);//should not show
			toothChart2D.SetCrown("15",Color.DarkRed);
			toothChart2D.SetCrown("17",Color.DarkBlue);
			toothChart2D.SetCrown("18",Color.DarkBlue);
			toothChart2D.SetCrown("19",Color.DarkBlue);
			toothChart2D.SetCrown("20",Color.LightBlue);
			toothChart2D.SetCrown("21",Color.LightBlue);
			toothChart2D.SetCrown("22",Color.LightBlue);
			toothChart2D.SetCrown("31",Color.White);
			toothChart2D.SetCrown("32",Color.Black);
			//
			toothChartOpenGL.SetCrown("1",Color.DarkGreen);
			toothChartOpenGL.SetSurfaceColors("3","MOD",Color.DarkRed);
			toothChartOpenGL.SetSurfaceColors("4","V",Color.Green);
			toothChartOpenGL.SetSurfaceColors("5","B",Color.Green);
			toothChartOpenGL.SetSurfaceColors("6","FIL",Color.DarkRed);
			toothChartOpenGL.SetPrimary("11");
			toothChartOpenGL.SetMissing("11");
			toothChartOpenGL.SetSurfaceColors("H","MOD",Color.DarkRed);//some invalid surfaces
			toothChartOpenGL.SetPrimary("12");
			toothChartOpenGL.SetSurfaceColors("I","MOD",Color.DarkRed);
			toothChartOpenGL.SetSurfaceColors("J","O",Color.DarkRed);//should not show
			toothChartOpenGL.SetHidden("14");
			toothChartOpenGL.SetSurfaceColors("14","MOD",Color.DarkRed);//should not show
			toothChartOpenGL.SetCrown("15",Color.DarkRed);
			toothChartOpenGL.SetCrown("17",Color.DarkBlue);
			toothChartOpenGL.SetCrown("18",Color.DarkBlue);
			toothChartOpenGL.SetCrown("19",Color.DarkBlue);
			toothChartOpenGL.SetCrown("20",Color.LightBlue);
			toothChartOpenGL.SetCrown("21",Color.LightBlue);
			toothChartOpenGL.SetCrown("22",Color.LightBlue);
			toothChartOpenGL.SetCrown("31",Color.White);
			toothChartOpenGL.SetCrown("32",Color.Black);
			//
			toothChartDirectX.SetCrown("1",Color.DarkGreen);
			toothChartDirectX.SetSurfaceColors("3","MOD",Color.DarkRed);
			toothChartDirectX.SetSurfaceColors("4","V",Color.Green);
			toothChartDirectX.SetSurfaceColors("5","B",Color.Green);
			toothChartDirectX.SetSurfaceColors("6","FIL",Color.DarkRed);
			toothChartDirectX.SetPrimary("11");
			toothChartDirectX.SetMissing("11");
			toothChartDirectX.SetSurfaceColors("H","MOD",Color.DarkRed);//some invalid surfaces
			toothChartDirectX.SetPrimary("12");
			toothChartDirectX.SetSurfaceColors("I","MOD",Color.DarkRed);
			toothChartDirectX.SetSurfaceColors("J","O",Color.DarkRed);//should not show
			toothChartDirectX.SetHidden("14");
			toothChartDirectX.SetSurfaceColors("14","MOD",Color.DarkRed);//should not show
			toothChartDirectX.SetCrown("15",Color.DarkRed);
			toothChartDirectX.SetCrown("17",Color.DarkBlue);
			toothChartDirectX.SetCrown("18",Color.DarkBlue);
			toothChartDirectX.SetCrown("19",Color.DarkBlue);
			toothChartDirectX.SetCrown("20",Color.LightBlue);
			toothChartDirectX.SetCrown("21",Color.LightBlue);
			toothChartDirectX.SetCrown("22",Color.LightBlue);
			toothChartDirectX.SetCrown("31",Color.White);
			toothChartDirectX.SetCrown("32",Color.Black);
		}

		private void butMouse_Click(object sender,EventArgs e) {
			//toothChartOpenGL.
		}

		private void butRCT_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			string tooth_id;
			for(int i=1;i<=32;i++) {
				tooth_id=i.ToString();
				toothChart2D.SetRCT(tooth_id,Color.DarkRed);
				toothChart2D.SetBU(tooth_id,Color.DarkRed);
				toothChartOpenGL.SetRCT(tooth_id,Color.DarkRed);
				toothChartOpenGL.SetBU(tooth_id,Color.DarkRed);
				toothChartDirectX.SetRCT(tooth_id,Color.DarkRed);
				toothChartDirectX.SetBU(tooth_id,Color.DarkRed);
			}
		}

		private void butPrimaryBU_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			string tooth_id;
			for(int i=1;i<=32;i++) {
				tooth_id=i.ToString();
				toothChart2D.SetPrimary(tooth_id);
				toothChartOpenGL.SetPrimary(tooth_id);
				toothChartDirectX.SetPrimary(tooth_id);
			}
			toothChart2D.SetBU("A",Color.DarkRed);
			toothChart2D.SetBU("B",Color.DarkRed);
			toothChart2D.SetBU("C",Color.DarkRed);
			toothChart2D.SetBU("D",Color.DarkRed);
			toothChart2D.SetBU("E",Color.DarkRed);
			toothChart2D.SetBU("F",Color.DarkRed);
			toothChart2D.SetBU("G",Color.DarkRed);
			toothChart2D.SetBU("H",Color.DarkRed);
			toothChart2D.SetBU("I",Color.DarkRed);
			toothChart2D.SetBU("J",Color.DarkRed);
			toothChart2D.SetBU("K",Color.DarkRed);
			toothChart2D.SetBU("L",Color.DarkRed);
			toothChart2D.SetBU("M",Color.DarkRed);
			toothChart2D.SetBU("N",Color.DarkRed);
			toothChart2D.SetBU("O",Color.DarkRed);
			toothChart2D.SetBU("P",Color.DarkRed);
			toothChart2D.SetBU("Q",Color.DarkRed);
			toothChart2D.SetBU("R",Color.DarkRed);
			toothChart2D.SetBU("S",Color.DarkRed);
			toothChart2D.SetBU("T",Color.DarkRed);
			//
			toothChartOpenGL.SetBU("A",Color.DarkRed);
			toothChartOpenGL.SetBU("B",Color.DarkRed);
			toothChartOpenGL.SetBU("C",Color.DarkRed);
			toothChartOpenGL.SetBU("D",Color.DarkRed);
			toothChartOpenGL.SetBU("E",Color.DarkRed);
			toothChartOpenGL.SetBU("F",Color.DarkRed);
			toothChartOpenGL.SetBU("G",Color.DarkRed);
			toothChartOpenGL.SetBU("H",Color.DarkRed);
			toothChartOpenGL.SetBU("I",Color.DarkRed);
			toothChartOpenGL.SetBU("J",Color.DarkRed);
			toothChartOpenGL.SetBU("K",Color.DarkRed);
			toothChartOpenGL.SetBU("L",Color.DarkRed);
			toothChartOpenGL.SetBU("M",Color.DarkRed);
			toothChartOpenGL.SetBU("N",Color.DarkRed);
			toothChartOpenGL.SetBU("O",Color.DarkRed);
			toothChartOpenGL.SetBU("P",Color.DarkRed);
			toothChartOpenGL.SetBU("Q",Color.DarkRed);
			toothChartOpenGL.SetBU("R",Color.DarkRed);
			toothChartOpenGL.SetBU("S",Color.DarkRed);
			toothChartOpenGL.SetBU("T",Color.DarkRed);
			//
			toothChartDirectX.SetBU("A",Color.DarkRed);
			toothChartDirectX.SetBU("B",Color.DarkRed);
			toothChartDirectX.SetBU("C",Color.DarkRed);
			toothChartDirectX.SetBU("D",Color.DarkRed);
			toothChartDirectX.SetBU("E",Color.DarkRed);
			toothChartDirectX.SetBU("F",Color.DarkRed);
			toothChartDirectX.SetBU("G",Color.DarkRed);
			toothChartDirectX.SetBU("H",Color.DarkRed);
			toothChartDirectX.SetBU("I",Color.DarkRed);
			toothChartDirectX.SetBU("J",Color.DarkRed);
			toothChartDirectX.SetBU("K",Color.DarkRed);
			toothChartDirectX.SetBU("L",Color.DarkRed);
			toothChartDirectX.SetBU("M",Color.DarkRed);
			toothChartDirectX.SetBU("N",Color.DarkRed);
			toothChartDirectX.SetBU("O",Color.DarkRed);
			toothChartDirectX.SetBU("P",Color.DarkRed);
			toothChartDirectX.SetBU("Q",Color.DarkRed);
			toothChartDirectX.SetBU("R",Color.DarkRed);
			toothChartDirectX.SetBU("S",Color.DarkRed);
			toothChartDirectX.SetBU("T",Color.DarkRed);
		}

		private void butBigX_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			string tooth_id;
			for(int i=1;i<=32;i++) {
				tooth_id=i.ToString();
				toothChart2D.SetBigX(tooth_id,Color.DarkRed);
				toothChartOpenGL.SetBigX(tooth_id,Color.DarkRed);
				toothChartDirectX.SetBigX(tooth_id,Color.DarkRed);
			}
		}

		private void butBridges_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetMissing("3");
			toothChart2D.SetCrown("2",Color.SteelBlue);
			toothChart2D.SetPontic("3",Color.SteelBlue);
			toothChart2D.SetCrown("4",Color.SteelBlue);
			toothChart2D.SetMissing("8");
			toothChart2D.SetMissing("9");
			toothChart2D.SetCrown("7",Color.SteelBlue);
			toothChart2D.SetPontic("8",Color.SteelBlue);
			toothChart2D.SetPontic("9",Color.SteelBlue);
			toothChart2D.SetCrown("10",Color.SteelBlue);
			toothChart2D.SetMissing("19");
			toothChart2D.SetCrown("18",Color.SteelBlue);
			toothChart2D.SetPontic("19",Color.SteelBlue);
			toothChart2D.SetCrown("20",Color.SteelBlue);
			toothChart2D.SetMissing("26");
			toothChart2D.SetMissing("27");
			toothChart2D.SetCrown("25",Color.SteelBlue);
			toothChart2D.SetPontic("26",Color.SteelBlue);
			toothChart2D.SetPontic("27",Color.SteelBlue);
			toothChart2D.SetCrown("28",Color.SteelBlue);
			//	
			toothChartOpenGL.SetMissing("3");
			toothChartOpenGL.SetCrown("2",Color.SteelBlue);
			toothChartOpenGL.SetPontic("3",Color.SteelBlue);
			toothChartOpenGL.SetCrown("4",Color.SteelBlue);
			toothChartOpenGL.SetMissing("8");
			toothChartOpenGL.SetMissing("9");
			toothChartOpenGL.SetCrown("7",Color.SteelBlue);
			toothChartOpenGL.SetPontic("8",Color.SteelBlue);
			toothChartOpenGL.SetPontic("9",Color.SteelBlue);
			toothChartOpenGL.SetCrown("10",Color.SteelBlue);
			toothChartOpenGL.SetMissing("19");
			toothChartOpenGL.SetCrown("18",Color.SteelBlue);
			toothChartOpenGL.SetPontic("19",Color.SteelBlue);
			toothChartOpenGL.SetCrown("20",Color.SteelBlue);
			toothChartOpenGL.SetMissing("26");
			toothChartOpenGL.SetMissing("27");
			toothChartOpenGL.SetCrown("25",Color.SteelBlue);
			toothChartOpenGL.SetPontic("26",Color.SteelBlue);
			toothChartOpenGL.SetPontic("27",Color.SteelBlue);
			toothChartOpenGL.SetCrown("28",Color.SteelBlue);
			//	
			toothChartDirectX.SetMissing("3");
			toothChartDirectX.SetCrown("2",Color.SteelBlue);
			toothChartDirectX.SetPontic("3",Color.SteelBlue);
			toothChartDirectX.SetCrown("4",Color.SteelBlue);
			toothChartDirectX.SetMissing("8");
			toothChartDirectX.SetMissing("9");
			toothChartDirectX.SetCrown("7",Color.SteelBlue);
			toothChartDirectX.SetPontic("8",Color.SteelBlue);
			toothChartDirectX.SetPontic("9",Color.SteelBlue);
			toothChartDirectX.SetCrown("10",Color.SteelBlue);
			toothChartDirectX.SetMissing("19");
			toothChartDirectX.SetCrown("18",Color.SteelBlue);
			toothChartDirectX.SetPontic("19",Color.SteelBlue);
			toothChartDirectX.SetCrown("20",Color.SteelBlue);
			toothChartDirectX.SetMissing("26");
			toothChartDirectX.SetMissing("27");
			toothChartDirectX.SetCrown("25",Color.SteelBlue);
			toothChartDirectX.SetPontic("26",Color.SteelBlue);
			toothChartDirectX.SetPontic("27",Color.SteelBlue);
			toothChartDirectX.SetCrown("28",Color.SteelBlue);
		}

		private void butImplants_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetMissing("3");
			toothChart2D.SetImplant("3",Color.Silver);
			toothChart2D.SetCrown("3",Color.SteelBlue);
			toothChart2D.SetMissing("7");
			toothChart2D.SetMissing("8");
			toothChart2D.SetMissing("9");
			toothChart2D.SetMissing("10");
			toothChart2D.SetImplant("7",Color.Silver);
			toothChart2D.SetImplant("10",Color.Silver);
			toothChart2D.SetCrown("7",Color.SteelBlue);
			toothChart2D.SetPontic("8",Color.SteelBlue);
			toothChart2D.SetPontic("9",Color.SteelBlue);
			toothChart2D.SetCrown("10",Color.SteelBlue);
			toothChart2D.SetMissing("19");
			toothChart2D.SetMissing("20");
			toothChart2D.SetImplant("19",Color.Silver);
			toothChart2D.SetImplant("20",Color.Silver);
			toothChart2D.SetCrown("19",Color.SteelBlue);
			toothChart2D.SetCrown("20",Color.SteelBlue);
			//
			toothChartOpenGL.SetMissing("3");
			toothChartOpenGL.SetImplant("3",Color.Silver);
			toothChartOpenGL.SetCrown("3",Color.SteelBlue);
			toothChartOpenGL.SetMissing("7");
			toothChartOpenGL.SetMissing("8");
			toothChartOpenGL.SetMissing("9");
			toothChartOpenGL.SetMissing("10");
			toothChartOpenGL.SetImplant("7",Color.Silver);
			toothChartOpenGL.SetImplant("10",Color.Silver);
			toothChartOpenGL.SetCrown("7",Color.SteelBlue);
			toothChartOpenGL.SetPontic("8",Color.SteelBlue);
			toothChartOpenGL.SetPontic("9",Color.SteelBlue);
			toothChartOpenGL.SetCrown("10",Color.SteelBlue);
			toothChartOpenGL.SetMissing("19");
			toothChartOpenGL.SetMissing("20");
			toothChartOpenGL.SetImplant("19",Color.Silver);
			toothChartOpenGL.SetImplant("20",Color.Silver);
			toothChartOpenGL.SetCrown("19",Color.SteelBlue);
			toothChartOpenGL.SetCrown("20",Color.SteelBlue);
			//
			toothChartDirectX.SetMissing("3");
			toothChartDirectX.SetImplant("3",Color.Silver);
			toothChartDirectX.SetCrown("3",Color.SteelBlue);
			toothChartDirectX.SetMissing("7");
			toothChartDirectX.SetMissing("8");
			toothChartDirectX.SetMissing("9");
			toothChartDirectX.SetMissing("10");
			toothChartDirectX.SetImplant("7",Color.Silver);
			toothChartDirectX.SetImplant("10",Color.Silver);
			toothChartDirectX.SetCrown("7",Color.SteelBlue);
			toothChartDirectX.SetPontic("8",Color.SteelBlue);
			toothChartDirectX.SetPontic("9",Color.SteelBlue);
			toothChartDirectX.SetCrown("10",Color.SteelBlue);
			toothChartDirectX.SetMissing("19");
			toothChartDirectX.SetMissing("20");
			toothChartDirectX.SetImplant("19",Color.Silver);
			toothChartDirectX.SetImplant("20",Color.Silver);
			toothChartDirectX.SetCrown("19",Color.SteelBlue);
			toothChartDirectX.SetCrown("20",Color.SteelBlue);
		}

		private void butSealants_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetSealant("2",Color.Red);
			toothChart2D.SetSealant("3",Color.Red);
			toothChart2D.SetSealant("4",Color.Red);
			toothChart2D.SetSealant("13",Color.Red);
			toothChart2D.SetSealant("14",Color.Red);
			toothChart2D.SetSealant("15",Color.Red);
			toothChart2D.SetSealant("18",Color.Red);
			toothChart2D.SetSealant("19",Color.Red);
			toothChart2D.SetSealant("20",Color.Red);
			toothChart2D.SetSealant("29",Color.Red);
			toothChart2D.SetSealant("30",Color.Red);
			toothChart2D.SetSealant("31",Color.Red);
			//
			toothChartOpenGL.SetSealant("2",Color.Red);
			toothChartOpenGL.SetSealant("3",Color.Red);
			toothChartOpenGL.SetSealant("4",Color.Red);
			toothChartOpenGL.SetSealant("13",Color.Red);
			toothChartOpenGL.SetSealant("14",Color.Red);
			toothChartOpenGL.SetSealant("15",Color.Red);
			toothChartOpenGL.SetSealant("18",Color.Red);
			toothChartOpenGL.SetSealant("19",Color.Red);
			toothChartOpenGL.SetSealant("20",Color.Red);
			toothChartOpenGL.SetSealant("29",Color.Red);
			toothChartOpenGL.SetSealant("30",Color.Red);
			toothChartOpenGL.SetSealant("31",Color.Red);
			//
			toothChartDirectX.SetSealant("2",Color.Red);
			toothChartDirectX.SetSealant("3",Color.Red);
			toothChartDirectX.SetSealant("4",Color.Red);
			toothChartDirectX.SetSealant("13",Color.Red);
			toothChartDirectX.SetSealant("14",Color.Red);
			toothChartDirectX.SetSealant("15",Color.Red);
			toothChartDirectX.SetSealant("18",Color.Red);
			toothChartDirectX.SetSealant("19",Color.Red);
			toothChartDirectX.SetSealant("20",Color.Red);
			toothChartDirectX.SetSealant("29",Color.Red);
			toothChartDirectX.SetSealant("30",Color.Red);
			toothChartDirectX.SetSealant("31",Color.Red);
		}

		private void butVeneers_Click(object sender,EventArgs e) {
			toothChart2D.ResetTeeth();
			toothChartOpenGL.ResetTeeth();
			toothChartDirectX.ResetTeeth();
			//
			toothChart2D.SetVeneer("4",Color.DarkRed);
			toothChart2D.SetVeneer("5",Color.DarkRed);
			toothChart2D.SetVeneer("6",Color.DarkRed);
			toothChart2D.SetVeneer("7",Color.DarkRed);
			toothChart2D.SetVeneer("8",Color.DarkRed);
			toothChart2D.SetVeneer("9",Color.DarkRed);
			toothChart2D.SetVeneer("10",Color.DarkRed);
			toothChart2D.SetVeneer("11",Color.DarkRed);
			toothChart2D.SetVeneer("12",Color.DarkRed);
			toothChart2D.SetVeneer("13",Color.DarkRed);
			toothChart2D.SetVeneer("20",Color.DarkRed);
			toothChart2D.SetVeneer("21",Color.DarkRed);
			toothChart2D.SetVeneer("22",Color.DarkRed);
			toothChart2D.SetVeneer("23",Color.DarkRed);
			toothChart2D.SetVeneer("24",Color.DarkRed);
			toothChart2D.SetVeneer("25",Color.DarkRed);
			toothChart2D.SetVeneer("26",Color.DarkRed);
			toothChart2D.SetVeneer("27",Color.DarkRed);
			toothChart2D.SetVeneer("28",Color.DarkRed);
			toothChart2D.SetVeneer("29",Color.DarkRed);
			//
			toothChartOpenGL.SetVeneer("4",Color.DarkRed);
			toothChartOpenGL.SetVeneer("5",Color.DarkRed);
			toothChartOpenGL.SetVeneer("6",Color.DarkRed);
			toothChartOpenGL.SetVeneer("7",Color.DarkRed);
			toothChartOpenGL.SetVeneer("8",Color.DarkRed);
			toothChartOpenGL.SetVeneer("9",Color.DarkRed);
			toothChartOpenGL.SetVeneer("10",Color.DarkRed);
			toothChartOpenGL.SetVeneer("11",Color.DarkRed);
			toothChartOpenGL.SetVeneer("12",Color.DarkRed);
			toothChartOpenGL.SetVeneer("13",Color.DarkRed);
			toothChartOpenGL.SetVeneer("20",Color.DarkRed);
			toothChartOpenGL.SetVeneer("21",Color.DarkRed);
			toothChartOpenGL.SetVeneer("22",Color.DarkRed);
			toothChartOpenGL.SetVeneer("23",Color.DarkRed);
			toothChartOpenGL.SetVeneer("24",Color.DarkRed);
			toothChartOpenGL.SetVeneer("25",Color.DarkRed);
			toothChartOpenGL.SetVeneer("26",Color.DarkRed);
			toothChartOpenGL.SetVeneer("27",Color.DarkRed);
			toothChartOpenGL.SetVeneer("28",Color.DarkRed);
			toothChartOpenGL.SetVeneer("29",Color.DarkRed);
			//
			toothChartDirectX.SetVeneer("4",Color.DarkRed);
			toothChartDirectX.SetVeneer("5",Color.DarkRed);
			toothChartDirectX.SetVeneer("6",Color.DarkRed);
			toothChartDirectX.SetVeneer("7",Color.DarkRed);
			toothChartDirectX.SetVeneer("8",Color.DarkRed);
			toothChartDirectX.SetVeneer("9",Color.DarkRed);
			toothChartDirectX.SetVeneer("10",Color.DarkRed);
			toothChartDirectX.SetVeneer("11",Color.DarkRed);
			toothChartDirectX.SetVeneer("12",Color.DarkRed);
			toothChartDirectX.SetVeneer("13",Color.DarkRed);
			toothChartDirectX.SetVeneer("20",Color.DarkRed);
			toothChartDirectX.SetVeneer("21",Color.DarkRed);
			toothChartDirectX.SetVeneer("22",Color.DarkRed);
			toothChartDirectX.SetVeneer("23",Color.DarkRed);
			toothChartDirectX.SetVeneer("24",Color.DarkRed);
			toothChartDirectX.SetVeneer("25",Color.DarkRed);
			toothChartDirectX.SetVeneer("26",Color.DarkRed);
			toothChartDirectX.SetVeneer("27",Color.DarkRed);
			toothChartDirectX.SetVeneer("28",Color.DarkRed);
			toothChartDirectX.SetVeneer("29",Color.DarkRed);
		}

		private void radioPointer_Click(object sender,EventArgs e) {
			toothChart2D.CursorTool=CursorTool.Pointer;
			toothChartOpenGL.CursorTool=CursorTool.Pointer;
			toothChartDirectX.CursorTool=CursorTool.Pointer;
		}

		private void radioPen_Click(object sender,EventArgs e) {
			toothChart2D.CursorTool=CursorTool.Pen;
			toothChartOpenGL.CursorTool=CursorTool.Pen;
			toothChartDirectX.CursorTool=CursorTool.Pen;
		}

		private void radioEraser_Click(object sender,EventArgs e) {
			toothChart2D.CursorTool=CursorTool.Eraser;
			toothChartOpenGL.CursorTool=CursorTool.Eraser;
			toothChartDirectX.CursorTool=CursorTool.Eraser;
		}

		private void radioColorChanger_Click(object sender,EventArgs e) {
			toothChart2D.CursorTool=CursorTool.ColorChanger;
			toothChartOpenGL.CursorTool=CursorTool.ColorChanger;
			toothChartDirectX.CursorTool=CursorTool.ColorChanger;
		}

		private void panelColorDrawBlack_Click(object sender,EventArgs e) {
			panelColorDraw.BackColor=panelColorDrawBlack.BackColor;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void panelColorDrawRed_Click(object sender,EventArgs e) {
			panelColorDraw.BackColor=panelColorDrawRed.BackColor;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void panelColorDrawBlue_Click(object sender,EventArgs e) {
			panelColorDraw.BackColor=panelColorDrawBlue.BackColor;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void panelColorDrawGreen_Click(object sender,EventArgs e) {
			panelColorDraw.BackColor=panelColorDrawGreen.BackColor;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void panelColorDrawWhite_Click(object sender,EventArgs e) {
			panelColorDraw.BackColor=panelColorDrawWhite.BackColor;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void butColorDrawOther_Click(object sender,EventArgs e) {
			ColorDialog cd=new ColorDialog();
			cd.Color=panelColorDraw.BackColor;
			if(cd.ShowDialog()!=DialogResult.OK) {
				return;
			}
			panelColorDraw.BackColor=cd.Color;
			toothChart2D.ColorDrawing=panelColorDraw.BackColor;
			toothChartOpenGL.ColorDrawing=panelColorDraw.BackColor;
			toothChartDirectX.ColorDrawing=panelColorDraw.BackColor;
		}

		private void butShowDrawing_Click(object sender,EventArgs e) {
			ToothInitial ti=new ToothInitial();
			ti.ColorDraw=Color.Blue;
			ti.DrawingSegment="30,30;70,30;70,70;50,80;50,90;30,70;30,30";
			ti.InitialType=ToothInitialType.Drawing;
			toothChart2D.AddDrawingSegment(ti);
			toothChartOpenGL.AddDrawingSegment(ti);
			toothChartDirectX.AddDrawingSegment(ti);
		}

		private void butGetBitmap2D_Click(object sender,EventArgs e) {
			textScreenshotBox.Visible=true;
			Application.DoEvents();
			FormImageViewer form=new FormImageViewer();
			form.Bmp=toothChart2D.GetBitmap();
			form.Show();
			textScreenshotBox.Visible=false;
		}

		private void butGetBitmapOpenGL_Click(object sender,EventArgs e) {
			textScreenshotBox.Visible=true;
			Application.DoEvents();
			FormImageViewer form=new FormImageViewer();
			form.Bmp=toothChartOpenGL.GetBitmap();
			form.Show();
			textScreenshotBox.Visible=false;
		}

		private void butGetBitmapDirectX_Click(object sender,EventArgs e) {
			textScreenshotBox.Visible=true;
			Application.DoEvents();
			FormImageViewer form=new FormImageViewer();
			form.Bmp=toothChartDirectX.GetBitmap();
			form.Show();
			textScreenshotBox.Visible=false;
		}

		private void butFullscreen2D_Click(object sender,EventArgs e) {
			FormFullScreen form=new FormFullScreen();
			form.toothChartForBig.DrawMode=DrawingMode.Simple2D;
			form.toothChartForBig.TcData=toothChartOpenGL.TcData.Copy();
			form.ShowDialog();
		}

		private void butFullscreenOpenGL_Click(object sender,EventArgs e) {
			FormFullScreen form=new FormFullScreen();
			form.toothChartForBig.DrawMode=DrawingMode.OpenGL;
			form.toothChartForBig.TcData=toothChartOpenGL.TcData.Copy();
			form.ShowDialog();
		}

		private void butFullscreenDirectX_Click(object sender,EventArgs e) {
			FormFullScreen form=new FormFullScreen();
			form.toothChartForBig.DrawMode=DrawingMode.DirectX;
			form.toothChartForBig.TcData=toothChartDirectX.TcData.Copy();
			form.ShowDialog();
		}

		private void butUniversal_Click(object sender,EventArgs e) {
			toothChart2D.SetToothNumberingNomenclature(ToothNumberingNomenclature.Universal);
			toothChartOpenGL.SetToothNumberingNomenclature(ToothNumberingNomenclature.Universal);
			toothChartDirectX.SetToothNumberingNomenclature(ToothNumberingNomenclature.Universal);
		}

		private void butFDI_Click(object sender,EventArgs e) {
			toothChart2D.SetToothNumberingNomenclature(ToothNumberingNomenclature.FDI);
			toothChartOpenGL.SetToothNumberingNomenclature(ToothNumberingNomenclature.FDI);
			toothChartDirectX.SetToothNumberingNomenclature(ToothNumberingNomenclature.FDI);
		}

		private void butHaderup_Click(object sender,EventArgs e) {
			toothChart2D.SetToothNumberingNomenclature(ToothNumberingNomenclature.Haderup);
			toothChartOpenGL.SetToothNumberingNomenclature(ToothNumberingNomenclature.Haderup);
			toothChartDirectX.SetToothNumberingNomenclature(ToothNumberingNomenclature.Haderup);
		}

		private void butPalmer_Click(object sender,EventArgs e) {
			toothChart2D.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
			toothChartOpenGL.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
			toothChartDirectX.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
		}

	

		
	

		

		





	}
}
