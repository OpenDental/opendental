using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Drawing2D;

namespace OpenDental {
	public partial class FormEhrGrowthCharts:FormODBase {
		public long PatNum;
		private Patient pat;
		private const float border=50f;
		private Pen thinPen=new Pen(Color.LightGray,0.5f);
		private Pen thickPen=new Pen(Color.Black,1.5f);
		private Brush dataBrush=new SolidBrush(Color.CadetBlue);
		private float minYH;
		private float maxYH;
		private float minYW;
		private float maxYW;
		private float minX;
		private float maxX;
		private float pixPerYear;
		private float pixPerInch;
		private float pixPerPound;
		private const int minAge=2;
		private const int maxAge=20;
		private const int maxInches=76;
		private const int maxPounds=230;
		private const int ageRange=maxAge-minAge;
		List<Vitalsign> listVS;

		public FormEhrGrowthCharts() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormGrowthCharts_Load(object sender,EventArgs e) {
			listVS = Vitalsigns.Refresh(PatNum);
			pat=Patients.GetPat(PatNum);
			Text+=" - "+pat.Gender.ToString()+" Growth Chart for "+pat.FName+" "+pat.LName+" age "+pat.Age;
		}

		private void FormGrowthCharts_Paint(object sender,PaintEventArgs e) {
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
			defineBounds(e.Graphics);
			DrawHeightGrid(e.Graphics);
			DrawWeightGrid(e.Graphics);
			PlotHW(e.Graphics);
		}

		///<summary>Defines the bounds of the graphs to be drawn based on size of form.</summary>
		private void defineBounds(Graphics g) {
			minYH=border;
			maxYH=ClientSize.Height/2f-border;
			minYW=ClientSize.Height/2f+border;
			maxYW=ClientSize.Height-border;
			minX=border+g.MeasureString("230lbs",Font).Width;
			maxX=(ClientSize.Width)-border-g.MeasureString("230lbs",Font).Width;
			pixPerYear=(maxX-minX)/(float)(ageRange);
			pixPerInch=(maxYH-minYH)/(float)maxInches;
			pixPerPound=(maxYW-minYW)/(float)maxPounds;
		}

		///<summary></summary>
		private void DrawHeightGrid(Graphics g) {
			float curX;
			float curY;
			SizeF labelPix;
			//draw background
			g.FillRectangle(Brushes.White,minX,minYH,maxX-minX,maxYH-minYH);
			//draw Title
			labelPix=g.MeasureString("Height - ("+pat.Gender.ToString()+")",Font);
			curX=((maxX+minX)/2)-(labelPix.Width/2);
			curY=minYH-labelPix.Height-2f;
			g.DrawString("Height - ("+pat.Gender.ToString()+")",Font,Brushes.Black,curX,curY);
			//Draw thin age lines
			for(int i=0;i<ageRange+1;i++) {
				curX=minX+(float)i*pixPerYear;
				g.DrawLine(thinPen,curX,minYH,curX,maxYH);
				labelPix=g.MeasureString((i+minAge).ToString()+"y",DefaultFont);
				g.DrawString((i+minAge).ToString()+"y",Font,Brushes.Black,curX-(labelPix.Width/2f),maxYH+1f);
			}
			//draw all Horizontal Height lines
			for(int i=0;i<maxInches+1;i++) {
				curY=maxYH-(float)i*pixPerInch;
				g.DrawLine(thinPen,minX,curY,maxX,curY);
				if(i%12==0 || i==maxInches) {//bold line and label every 12"
					g.DrawLine(thickPen,minX,curY,maxX,curY);
					g.DrawString(i.ToString()+"in",Font,Brushes.Black,border-5f,curY-5f);
					g.DrawString(i.ToString()+"in",Font,Brushes.Black,maxX+4f,curY-5f);
				}
			}
			//draw bold vertical age lines
			for(int i=0;i<ageRange+1;i++) {
				curX=minX+(float)i*pixPerYear;
				if((i%5)+minAge==5 || i==0) {
					g.DrawLine(thickPen,curX,minYH,curX,maxYH);
				}
			}
		}

		///<summary></summary>
		private void DrawWeightGrid(Graphics g) {
			float curX;
			float curY;
			SizeF labelPix;
			//draw background
			g.FillRectangle(Brushes.White,minX,minYW,maxX-minX,maxYW-minYW);
			//draw Title
			labelPix=g.MeasureString("Weight - ("+pat.Gender.ToString()+")",Font);
			curX=((maxX+minX)/2)-(labelPix.Width/2);
			curY=minYW-labelPix.Height-2f;
			g.DrawString("Weight - ("+pat.Gender.ToString()+")",Font,Brushes.Black,curX,curY);
			//Draw thin age lines
			for(int i=0;i<ageRange+1;i++) {
				curX=minX+(float)i*pixPerYear;
				g.DrawLine(thinPen,curX,minYW,curX,maxYW);
				labelPix=g.MeasureString((i+minAge).ToString()+"y",Font);
				g.DrawString((i+minAge).ToString()+"y",DefaultFont,Brushes.Black,curX-(labelPix.Width/2f),maxYW+2f);
			}
			//draw all Horizontal pound lines
			for(int i=0;i<maxPounds+1;i++) {
				curY=maxYW-(float)i*pixPerPound;
				if(i%5==0){//gray line every 5 lbs
					g.DrawLine(thinPen,minX,curY,maxX,curY);
				}
				if(i%20==0 || i==maxPounds) {//bold line and label every 20lbs
					g.DrawLine(thickPen,minX,curY,maxX,curY);
					g.DrawString(i.ToString()+"lbs",DefaultFont,Brushes.Black,border-5f,curY-5f);
					g.DrawString(i.ToString()+"lbs",DefaultFont,Brushes.Black,maxX+4f,curY-5f);
				}
			}
			//draw bold vertical age lines
			for(int i=0;i<ageRange+1;i++) {
				curX=minX+(float)i*pixPerYear;
				if((i%5)+minAge==5 || i==0) {
					g.DrawLine(thickPen,curX,minYW,curX,maxYW);
				}
			}
		}

		///<summary></summary>
		private void PlotHW(Graphics g) {
			float curX;
			float curY;
			foreach(Vitalsign vs in listVS) {
				TimeSpan ageOfVitals = vs.DateTaken-pat.Birthdate;
				curX=minX+(float)((ageOfVitals.TotalDays/365)-2)*pixPerYear;//-2 to adjust for age starting at 2yrs old
				curY=maxYH-vs.Height*pixPerInch;
				g.FillEllipse(Brushes.Blue,curX-2.5f,curY-2.5f,5f,5f);
				curY=maxYW-(float)vs.Weight*pixPerPound;
				g.FillEllipse(Brushes.Teal,curX-2.5f,curY-2.5f,5f,5f);
			}
		}
	
	}
}


/* This is a copy of the entire file as of 3/25/11 AM
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormGrowthCharts:ODForm {
		private float patNum;
		private const float border=10f;
		private Pen thinPen=new Pen(Color.Gray,1f);
		private Pen thickPen=new Pen(Color.Black,1f);

		public FormGrowthCharts() {
			InitializeComponent();
		}

		private void FormGrowthCharts_Load(object sender,EventArgs e) {

		}

		private void FormGrowthCharts_Paint(object sender,PaintEventArgs e) {
			//DrawHeightGrid(e.Graphics);
			//DrawWeightGrid(e.Graphics);
			testGraphics(e.Graphics);
			e.Graphics.DrawString(Height.ToString(),DefaultFont,Brushes.Cyan,5,30);
			e.Graphics.DrawString(Width.ToString(),DefaultFont,Brushes.Cyan,30,30);
			//DrawWeightGrid
			//etc...
			//e.Graphics.DrawLine(thinPen,0,0,100,100);
		}

		private void testGraphics(Graphics g){
			Height=200;
			Width=200;
			for(int i=0;i<400;i++){
				g.DrawLine(thinPen,i,0,i,i);
				if(i%10==0){
				g.DrawLine(thickPen,i,0,i,i);					
				}
			}
		}

		///<summary></summary>
		private void DrawHeightGrid(Graphics g) {
			float maxYcoord = (float)(Height-border)/2f;
			float maxXcoord = (float)Width-border;
			float pixPerX = (float)(Width-2f*border)/19f;
			float pixPerY = (float)(maxYcoord-border)/76f;
			float xCur = border;
			float yCur = border;
			
			for(int i=0;i<19;i++) { //Vertical Lines
				xCur=border+(float)i*pixPerX;
				g.DrawLine(thinPen,xCur,border,xCur,maxYcoord);
				g.DrawString((i+2).ToString(),DefaultFont,Brushes.Black,xCur-7f,maxYcoord);
			}
			maxXcoord=xCur;//graphics issue with Width. This ensures the horizontal lines are drawn up to but not past the vertical lines.
			for(int i=0;i<77;i++) {//Horizontal Height Lines
				yCur=maxYcoord-i*pixPerY;
				g.DrawLine(thinPen,border,yCur,maxXcoord,yCur);
				if(i%12==0) {//bold line every 12 inches
					g.DrawLine(thickPen,border,yCur,maxXcoord,yCur);
					g.DrawString(i.ToString(),DefaultFont,Brushes.Black,maxXcoord+4f,yCur-5f);
				}
			}
			for(int i=0;i<19;i++) {//Bold vertical Lines
				xCur=border+(float)i*pixPerX;
				if(i%5==3) {//bold line every 5 years
					g.DrawLine(thickPen,xCur,border,xCur,maxYcoord);
				}
			}
		}

		///<summary></summary>
		private void DrawWeightGrid(Graphics g) {
			float minYcoord = (float)(Height+border)/2f;
			float maxYcoord = (float)(Height-border)*.96f;
			float maxXcoord = (float)Width-border;
			float pixPerX = (float)(Width-2f*border)/19f;
			float pixPerY = (float)(maxYcoord-minYcoord)/231f;
			float xCur = border;
			float yCur = border;

			for(int i=0;i<19;i++) { //Vertical Lines
				xCur=border+(float)i*pixPerX;
				g.DrawLine(thinPen,xCur,minYcoord,xCur,maxYcoord);
				//g.DrawString((i+2).ToString(),DefaultFont,Brushes.Black,xCur-7f,maxYcoord+5f);
			}
			maxXcoord=xCur;//graphics issue with Width. This ensures the horizontal lines are drawn up to but not past the last vertical line.
			for(int i=0;i<231;i++) {//Horizontal Lines
				yCur=maxYcoord-i*pixPerY;
				if(i%2==0) {//lines every inch was too crowded.
					g.DrawLine(thinPen,border,yCur,maxXcoord,yCur);
				}
				if(i%10==0) {//bold line every 10 lbs
					g.DrawLine(thickPen,border,yCur,maxXcoord,yCur);
				}
				if(i%20==0) {//number every 20 lbs
					g.DrawString(i.ToString(),DefaultFont,Brushes.Black,maxXcoord+4f,yCur-5f);
				}
			}
			for(int i=0;i<19;i++) {//Bold vertical Lines
				xCur=border+(float)i*pixPerX;
				if(i%5==3) {//bold line every 5 years
					g.DrawLine(thickPen,xCur,minYcoord,xCur,(float)(Height-border)*.96f);
				}
			}
		}

	}
}
*/