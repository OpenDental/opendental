using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class ScreenToothChart:UserControl {
		private string _toothValues;
		public bool IsPrimary;
		///<summary>Returns a list of tooth controls, listed from left to right, then right to left taking first top, then bottom as you are looking at the tooth chart.</summary>
		public List<UserControlScreenTooth> GetTeeth {
			get {
				return new List<UserControlScreenTooth>() {controlTooth2,controlTooth3,controlTooth4,controlTooth5,controlTooth12,controlTooth13,controlTooth14,
					controlTooth15,controlTooth18,controlTooth19,controlTooth20,controlTooth21,controlTooth28,controlTooth29,controlTooth30,controlTooth31 };
			}
		}

		///<summary>Returns a list of tooth controls for primary teeth, listed from left to right, then right to left taking first top, then bottom as you are looking at the tooth chart.</summary>
		public List<UserControlScreenTooth> GetPrimaryTeeth {
			get {
				return new List<UserControlScreenTooth>() {controlToothA,controlTooth2,controlTooth3,controlTooth4,controlTooth5,controlTooth12,controlTooth13,
					controlTooth14,controlTooth15,controlToothJ,controlToothK,controlTooth18,controlTooth19,controlTooth20,controlTooth21,controlTooth28,controlTooth29,controlTooth30,
					controlTooth31,controlToothT };
			}
		}

		public void SetChartMode() {
			if(IsPrimary) {
				//Change labels to primary teeth
				labelA.Text="A";
				labelA.Visible=true;
				label2.Text="B";
				label3.Text="C";
				label4.Text="D";
				label5.Text="E";
				label12.Text="F";
				label13.Text="G";
				label14.Text="H";
				label15.Text="I";
				labelJ.Text="J";
				labelJ.Visible=true;
				labelK.Text="K";
				labelK.Visible=true;
				label18.Text="L";
				label19.Text="M";
				label20.Text="N";
				label21.Text="O";
				label28.Text="P";
				label29.Text="Q";
				label30.Text="R";
				label31.Text="S";
				labelT.Text="T";
				labelT.Visible=true;
				//Change teeth to all having one surface
				controlToothA.Visible=true;
				controlToothA.IsPrimary=true;
				controlToothA.Width=72;
				controlToothA.IsMolar=false;
				labelA.Width=72;
				controlToothJ.Visible=true;
				controlToothJ.IsPrimary=true;
				controlToothJ.Width=72;
				controlToothJ.IsMolar=false;
				labelJ.Width=72;
				controlToothK.Visible=true;
				controlToothK.IsPrimary=true;
				controlToothK.Width=72;
				controlToothK.IsMolar=false;
				labelK.Width=72;
				controlToothT.Visible=true;
				controlToothT.IsPrimary=true;
				controlToothT.Width=72;
				controlToothT.IsMolar=false;
				labelT.Width=72;
				controlTooth2.IsPrimary=true;
				controlTooth2.Width=72;
				controlTooth2.IsMolar=false;
				label2.Width=72;
				controlTooth3.IsPrimary=true;
				controlTooth3.Width=72;
				controlTooth3.IsMolar=false;
				label3.Width=72;
				controlTooth4.IsPrimary=true;
				controlTooth4.Width=72;
				controlTooth4.IsMolar=false;
				label4.Width=72;
				controlTooth5.IsPrimary=true;
				controlTooth5.Width=72;
				controlTooth5.IsMolar=false;
				label5.Width=72;
				controlTooth12.IsPrimary=true;
				controlTooth12.Width=72;
				controlTooth12.IsMolar=false;
				label12.Width=72;
				controlTooth13.IsPrimary=true;
				controlTooth13.Width=72;
				controlTooth13.IsMolar=false;
				label13.Width=72;
				controlTooth14.IsPrimary=true;
				controlTooth14.Width=72;
				controlTooth14.IsMolar=false;
				label14.Width=72;
				controlTooth15.IsPrimary=true;
				controlTooth15.Width=72;
				controlTooth15.IsMolar=false;
				label15.Width=72;
				controlTooth18.IsPrimary=true;
				controlTooth18.Width=72;
				controlTooth18.IsMolar=false;
				label18.Width=72;
				controlTooth19.IsPrimary=true;
				controlTooth19.Width=72;
				controlTooth19.IsMolar=false;
				label19.Width=72;
				controlTooth20.IsPrimary=true;
				controlTooth20.Width=72;
				controlTooth20.IsMolar=false;
				label20.Width=72;
				controlTooth21.IsPrimary=true;
				controlTooth21.Width=72;
				controlTooth21.IsMolar=false;
				label21.Width=72;
				controlTooth28.IsPrimary=true;
				controlTooth28.Width=72;
				controlTooth28.IsMolar=false;
				label28.Width=72;
				controlTooth29.IsPrimary=true;
				controlTooth29.Width=72;
				controlTooth29.IsMolar=false;
				label29.Width=72;
				controlTooth30.IsPrimary=true;
				controlTooth30.Width=72;
				controlTooth30.IsMolar=false;
				label30.Width=72;
				controlTooth31.IsPrimary=true;
				controlTooth31.Width=72;
				controlTooth31.IsMolar=false;
				label31.Width=72;
				//Resize the tooth chart to allow for four more teeth than the normal chart
				controlToothA.Location=controlTooth2.Location;
				labelA.Location=label2.Location;
				controlTooth2.Location=new Point(controlToothA.Location.X+72,controlTooth2.Location.Y);
				label2.Location=new Point(labelA.Location.X+72,label2.Location.Y);
				controlTooth3.Location=new Point(controlTooth2.Location.X+72,controlTooth3.Location.Y);
				label3.Location=new Point(label2.Location.X+72,label3.Location.Y);
				controlTooth4.Location=new Point(controlTooth3.Location.X+72,controlTooth4.Location.Y);
				label4.Location=new Point(label3.Location.X+72,label4.Location.Y);
				controlTooth5.Location=new Point(controlTooth4.Location.X+72,controlTooth5.Location.Y);
				label5.Location=new Point(label4.Location.X+72,label5.Location.Y);
				controlTooth12.Location=new Point(controlTooth5.Location.X+72,controlTooth12.Location.Y);
				label12.Location=new Point(label5.Location.X+72,label12.Location.Y);
				controlTooth13.Location=new Point(controlTooth12.Location.X+72,controlTooth13.Location.Y);
				label13.Location=new Point(label12.Location.X+72,label13.Location.Y);
				controlTooth14.Location=new Point(controlTooth13.Location.X+72,controlTooth14.Location.Y);
				label14.Location=new Point(label13.Location.X+72,label14.Location.Y);
				controlTooth15.Location=new Point(controlTooth14.Location.X+72,controlTooth15.Location.Y);
				label15.Location=new Point(label14.Location.X+72,label15.Location.Y);
				controlToothJ.Location=new Point(controlTooth15.Location.X+72,controlTooth15.Location.Y);
				labelJ.Location=new Point(label15.Location.X+72,label15.Location.Y);
				controlToothT.Location=controlTooth31.Location;
				labelT.Location=label31.Location;
				controlTooth31.Location=new Point(controlToothT.Location.X+72,controlTooth31.Location.Y);
				label31.Location=new Point(labelT.Location.X+72,label31.Location.Y);
				controlTooth30.Location=new Point(controlTooth31.Location.X+72,controlTooth30.Location.Y);
				label30.Location=new Point(label31.Location.X+72,label30.Location.Y);
				controlTooth29.Location=new Point(controlTooth30.Location.X+72,controlTooth29.Location.Y);
				label29.Location=new Point(label30.Location.X+72,label29.Location.Y);
				controlTooth28.Location=new Point(controlTooth29.Location.X+72,controlTooth28.Location.Y);
				label28.Location=new Point(label29.Location.X+72,label28.Location.Y);
				controlTooth21.Location=new Point(controlTooth28.Location.X+72,controlTooth21.Location.Y);
				label21.Location=new Point(label28.Location.X+72,label21.Location.Y);
				controlTooth20.Location=new Point(controlTooth21.Location.X+72,controlTooth20.Location.Y);
				label20.Location=new Point(label21.Location.X+72,label20.Location.Y);
				controlTooth19.Location=new Point(controlTooth20.Location.X+72,controlTooth19.Location.Y);
				label19.Location=new Point(label20.Location.X+72,label19.Location.Y);
				controlTooth18.Location=new Point(controlTooth19.Location.X+72,controlTooth18.Location.Y);
				label18.Location=new Point(label19.Location.X+72,label18.Location.Y);
				controlToothK.Location=new Point(controlTooth18.Location.X+72,controlTooth18.Location.Y);
				labelK.Location=new Point(label18.Location.X+72,label18.Location.Y);
			}
			else {
				label2.Text="2";
				label3.Text="3";
				label4.Text="4";
				label5.Text="5";
				label12.Text="12";
				label13.Text="13";
				label14.Text="14";
				label15.Text="15";
				label18.Text="18";
				label19.Text="19";
				label20.Text="20";
				label21.Text="21";
				label28.Text="28";
				label29.Text="29";
				label30.Text="30";
				label31.Text="31";
			}
		}

		public ScreenToothChart(string toothValues,bool isPrimary) {
			//InitializeComponent creates and loads the teeth.  Teeth need to be set.
			InitializeComponent();
			IsPrimary=isPrimary;
			SetChartMode();
			_toothValues=toothValues;
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}

		private void ScreenToothChart_Load(object sender,EventArgs e) {
			string[] teethValues=_toothValues.Split(';');
			//teethValues[0] is the indicator of if it's a primary chart or not.
			if(!IsPrimary) {
				controlTooth2.SetSelected(teethValues[1].Split(','));
				controlTooth3.SetSelected(teethValues[2].Split(','));
				controlTooth4.SetSelected(teethValues[3].Split(','));
				controlTooth5.SetSelected(teethValues[4].Split(','));
				controlTooth12.SetSelected(teethValues[5].Split(','));
				controlTooth13.SetSelected(teethValues[6].Split(','));
				controlTooth14.SetSelected(teethValues[7].Split(','));
				controlTooth15.SetSelected(teethValues[8].Split(','));
				controlTooth18.SetSelected(teethValues[9].Split(','));
				controlTooth19.SetSelected(teethValues[10].Split(','));
				controlTooth20.SetSelected(teethValues[11].Split(','));
				controlTooth21.SetSelected(teethValues[12].Split(','));
				controlTooth28.SetSelected(teethValues[13].Split(','));
				controlTooth29.SetSelected(teethValues[14].Split(','));
				controlTooth30.SetSelected(teethValues[15].Split(','));
				controlTooth31.SetSelected(teethValues[16].Split(','));
			}
			else {
				//There are 4 additional teeth with primary teeth.
				controlToothA.SetSelected(teethValues[1].Split(','));//A
				controlTooth2.SetSelected(teethValues[2].Split(','));//B
				controlTooth3.SetSelected(teethValues[3].Split(','));//C
				controlTooth4.SetSelected(teethValues[4].Split(','));//D
				controlTooth5.SetSelected(teethValues[5].Split(','));//E
				controlTooth12.SetSelected(teethValues[6].Split(','));//F
				controlTooth13.SetSelected(teethValues[7].Split(','));//G
				controlTooth14.SetSelected(teethValues[8].Split(','));//H
				controlTooth15.SetSelected(teethValues[9].Split(','));//I
				controlToothJ.SetSelected(teethValues[10].Split(','));//J
				controlToothK.SetSelected(teethValues[11].Split(','));//K
				controlTooth18.SetSelected(teethValues[12].Split(','));//L
				controlTooth19.SetSelected(teethValues[13].Split(','));//M
				controlTooth20.SetSelected(teethValues[14].Split(','));//N
				controlTooth21.SetSelected(teethValues[15].Split(','));//O
				controlTooth28.SetSelected(teethValues[16].Split(','));//P
				controlTooth29.SetSelected(teethValues[17].Split(','));//Q
				controlTooth30.SetSelected(teethValues[18].Split(','));//R
				controlTooth31.SetSelected(teethValues[19].Split(','));//S
				controlToothT.SetSelected(teethValues[20].Split(','));//T
			}
			Invalidate();
		}

	}
}
