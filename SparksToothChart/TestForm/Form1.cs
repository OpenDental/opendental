using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;
using SparksToothChart;

namespace TestForm {
	public partial class Form1:Form {

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender,EventArgs e) {
			SetProcs();
		}

		private void SetProcs(){
			chart.SuspendLayout();
			chart.ResetTeeth();
			for(int i=1;i<=13;i++){
				chart.SetInvisible(i.ToString());
				chart.SetImplant(i.ToString(),Color.Red);
			}
			for(int i=2;i<6;i++){
				chart.SetCrown(i.ToString(),Color.Red);
			}
			for(int i=22;i<=32;i++) {
				chart.SetInvisible(i.ToString());
				chart.SetImplant(i.ToString(),Color.Red);
			}
			for(int i=27;i<31;i++) {
				chart.SetCrown(i.ToString(),Color.Red);
			}
			//chart.MoveTooth("2",30,0,0,0,0,0);
			/*chart.HideTooth("31");
			chart.MoveTooth("32",0,30,0,8.5f,0,0);
			chart.SetCrownColor("30",Color.DarkGoldenrod);
			chart.SetSurfaceColors("3","MOD",Color.DarkRed);
			chart.SetBigX("7",Color.Blue);
			chart.SetToPrimary("13");*/
			chart.ResumeLayout();
		}

		private void button1_Click(object sender,EventArgs e) {
			SetProcs();
			/*chart.MoveTooth("13",45,0,0,0f,-2,0);
			chart.SetInvisible("31");
			chart.MoveTooth("32",0,30,0,8.5f,0,0);
			chart.MoveTooth("24",0,0,-10,0,0,0);
			chart.MoveTooth("23",20,0,10,0,0,0);
			chart.MoveTooth("25",0,0,0,0,3,0);
			chart.MoveTooth("8",0,0,0,-1,0,0);
			chart.MoveTooth("9",0,0,0,-1,0,0);
			chart.MoveTooth("11",0,80,0,0,-12,0);
			chart.MoveTooth("1",0,0,30,0,0,2);
			chart.MoveTooth("12",0,15,0,3,0,0);
			*/
			/*for(int i=1;i<=32;i++){
				if(i==3 || i==14 || i==19 || i==30){
					continue;
				}
				chart.SetToPrimary(i.ToString());
			}
			chart.MoveTooth("3",0,0,0,0,-2,0);
			chart.MoveTooth("14",0,0,0,0,-2,0);
			chart.MoveTooth("19",0,0,0,0,-2,0);
			chart.MoveTooth("30",0,0,0,0,-2,0);*/
			/*chart.SetToPrimary("3");
			chart.SetToPrimary("4");
			chart.SetInvisible("31");
			chart.MoveTooth("32",0,30,0,8.5f,0,0);
			chart.SetSurfaceColors("4","B",Color.Red);
			chart.SetSurfaceColors("A","O",Color.Blue);
			chart.SetInvisible("14");
			chart.SetPontic("14",Color.Red);
			chart.SetRCT("12",Color.Red);*/
			//for(int i=1;i<=14;i++){
			//	chart.SetRCT(i.ToString(),Color.Red);
			//}
			
			
		}

		private void butRed_Click(object sender,EventArgs e) {
			//chart.SetSurfaceColors("19","LMOD",Color.Red);
		}

		private void butBlue_Click(object sender,EventArgs e) {
			//chart.SetSurfaceColors("19","LMOD",Color.Blue);
		}

		private void butInvalid_Click(object sender,EventArgs e) {
			//chart.Invalidate();
		}


		
	}
}