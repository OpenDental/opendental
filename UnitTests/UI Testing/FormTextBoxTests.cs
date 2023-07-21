using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;

namespace UnitTests {
	public partial class FormTextBoxTests:FormODBase {
		public FormTextBoxTests() {
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=20;
		}

		private void FormTextBoxTests_Load(object sender, EventArgs e){
			LayoutManager.LayoutFormBoundsAndFonts(this);
			panel1.Invalidate();
		}

		private void butSetDate_Click(object sender,EventArgs e) {
			//textDate.Text=DateTime.Today.ToShortDateString();
			textDate.Value=DateTime.Today;
			textDate.Validate();
		}

		private void butSetEmpty_Click(object sender,EventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		private void panel1_Paint(object sender,PaintEventArgs e) {
			//Note: This behavior does match my drawing in UI.TextBox when at 96dpi.
			//But when MS is scaled to 150%, they draw differently.
			//TextBox ends up a bit wider.
			//A hint to the possible difference is that this uses LayoutManager.ScaleF, whereas TextBox uses ScaleFontODZoom.
			//Unresolved.
			Graphics g=e.Graphics;
			g.DrawLine(Pens.Blue,LayoutManager.ScaleF(20+100),0,LayoutManager.ScaleF(20+100),200);
			float x=LayoutManager.ScaleF(20);
			float y=LayoutManager.ScaleF(20);
			RectangleF rectangleFText=RectangleF.Empty;
			StringFormat stringFormat=new StringFormat();
			stringFormat.FormatFlags=StringFormatFlags.NoWrap;
			List<RectangleF> listRectangleFsChars=new List<RectangleF>();
			Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
			//We are going to test how the same string draws differently in various situations
			//string str="This is a test";
			string strLong="This is a test that spills over long";
			////1-in rectangle
			//rectangleFText=new RectangleF(x,y,LayoutManager.ScaleF(100),LayoutManager.ScaleF(20));
			//listRectangleFsChars=MeasureRects(g,str,font,rectangleFText,stringFormat);
			//g.DrawString(str,font,Brushes.Black,rectangleFText,stringFormat);
			//g.DrawRectangles(Pens.Red,listRectangleFsChars.ToArray());
			//y+=LayoutManager.ScaleF(25);
			////2-at point
			//rectangleFText=new RectangleF(x,y,LayoutManager.ScaleF(100),LayoutManager.ScaleF(20));
			//listRectangleFsChars=MeasureRects(g,str,font,rectangleFText,stringFormat);
			//g.DrawString(str,font,Brushes.Black,x,y,stringFormat);
			//g.DrawRectangles(Pens.Red,listRectangleFsChars.ToArray());
			//y+=LayoutManager.ScaleF(25);
			////3-long in rectangle
			//rectangleFText=new RectangleF(x,y,LayoutManager.ScaleF(100),LayoutManager.ScaleF(20));
			//listRectangleFsChars=MeasureRects(g,strLong,font,rectangleFText,stringFormat);
			//g.DrawString(strLong,font,Brushes.Black,rectangleFText,stringFormat);
			//g.DrawRectangles(Pens.Red,listRectangleFsChars.ToArray());
			//y+=LayoutManager.ScaleF(25);
			//3-long at point
			rectangleFText=new RectangleF(x,y,LayoutManager.ScaleF(200),LayoutManager.ScaleF(20));
			listRectangleFsChars=MeasureRects(g,strLong,font,rectangleFText,stringFormat);
			g.DrawString(strLong,font,Brushes.Black,x,y,stringFormat);
			g.DrawRectangles(Pens.Red,listRectangleFsChars.ToArray());
			y+=LayoutManager.ScaleF(25);
		}

		private List<RectangleF> MeasureRects(Graphics g,string text,Font font,RectangleF rectangleFText,StringFormat stringFormat){
			List<RectangleF> listRectangleFsChars=new List<RectangleF>();
			int cStart=0;//the starting idx of the group of 32 or less chars that we're about to measure
			while(true){
				if(cStart>text.Length-1){//example 5>4
					break;
				}
				int length=32;
				if(cStart+length>=text.Length){
					length=text.Length-cStart;
				}
				CharacterRange[] characterRangeArray=new CharacterRange[length];//each CharacterRange is just one char
				for(int i=0;i<characterRangeArray.Length;i++){
					characterRangeArray[i]=new CharacterRange(cStart+i,1);
				}
				stringFormat.SetMeasurableCharacterRanges(characterRangeArray);
				Region[] regionArray=g.MeasureCharacterRanges(text,font,rectangleFText,stringFormat);
				for(int r=0;r<regionArray.Length;r++){
					RectangleF rectangleFBoundsChar=regionArray[r].GetBounds(g);
					listRectangleFsChars.Add(rectangleFBoundsChar);
				}//end of r loop
				cStart+=length;
			}//end of while cStart loop
			return listRectangleFsChars;
		}
	}
}
