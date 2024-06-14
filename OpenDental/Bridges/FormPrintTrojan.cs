using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary>This class is pretty much only coded for 8.5 inch X 11 inch sheets of paper, but with a little work could be made for more general sizes of paper, perhaps sometime in the future.</summary>
	public partial class FormPrintTrojan:FormODBase {

		///<summary>If pageNumberFont is set, then the page number is displayed using the page number information.</summary>
		private Font pageNumberFont=null;
		private int totalPages=0;
		///Is set to a non-null value only during printing to a physical printer.
		private Graphics printerGraph=null;
		private Rectangle printerMargins;
		private int pageHeight=900;
		private int curPrintPage=0;
		public delegate void PrintCallback(FormPrintTrojan fpr);
		public PrintCallback printGenerator=null;
		private bool landscape=false;
		private int numTimesPrinted=0;
		private int minimumTimesToPrint=0;

		public FormPrintTrojan() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPrintReport_Load(object sender,EventArgs e) {
			PrintCustom();
			CheckWindowSize();
		}

		public void PrintDelForPatients(DataTable dataTablePatient,Collection<string[]> collectionStringArraysPatient){
			Text="Trojan Plans Pending Deletion: Contact Patients";
			ScrollAmount=10;
			UsePageNumbers(new Font(FontFamily.GenericMonospace,8));
			MinimumTimesToPrint=1;
			//Print the header on the report.
			Font font=new Font(FontFamily.GenericMonospace,12);
			string text=PrefC.GetString(PrefName.PracticeTitle);
			SizeF size=Graph.MeasureString(text,font);
			float y=20;
			Graph.DrawString(text,font,Brushes.Black,GraphWidth/2-size.Width/2,y);
			text=DateTime.Today.ToShortDateString();
			size=Graph.MeasureString(text,font);
			Graph.DrawString(text,font,Brushes.Black,GraphWidth-size.Width,y);
			y+=size.Height;
			text="PLANS PENDING DELETION WHICH REQUIRE YOUR ATTENTION";
			size=Graph.MeasureString(text,font);
			Graph.DrawString(text,font,Brushes.Black,GraphWidth/2-Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			y+=20;//Skip a line or so.
			text="INSTRUCTIONS: These plans no longer exist, please do not contact Trojan. Please contact your patient for current benefit information.";
			Graph.DrawString(text,new Font(font,FontStyle.Bold),Brushes.Black,new RectangleF(0,y,650,500));
			y+=70;//Skip a line or so.
			text="Patient&Insured";
			font=new Font(font.FontFamily,9);
			Graph.DrawString(text,font,Brushes.Black,20,y);
			text="TrojanID";
			Graph.DrawString(text,font,Brushes.Black,240,y);
			text="Employer";
			Graph.DrawString(text,font,Brushes.Black,330,y);
			text="Carrier";
			Graph.DrawString(text,font,Brushes.Black,500,y);
			y+=20;
			//Use a static height for the records, to keep the math simple.
			float recordHeight=140;
			float recordSpacing=10;
			//Calculate the total number of pages in the report the first time this function is called only.
			if(TotalPages==0){
				TotalPages=(int)Math.Ceiling((y+recordHeight*dataTablePatient.Rows.Count+
					((dataTablePatient.Rows.Count>1)?dataTablePatient.Rows.Count-1:0)*recordSpacing)/PageHeight);
			}
			float pageBoundry=PageHeight;
			for(int i=0;i<dataTablePatient.Rows.Count;i++){
				//Draw the outlines around this record.
				Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(GraphWidth-1,y));
				Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight),new PointF(GraphWidth-1,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(0,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(GraphWidth-1,y),new PointF(GraphWidth-1,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight-40),new PointF(GraphWidth-1,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(235,y),new PointF(235,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(325,y),new PointF(325,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(500,y),new PointF(500,y+recordHeight-40));
				//Install the information for the record into the outline box.
				//Patient name, Guarantor name, guarantor SSN, guarantor birthdate, insurance plan group number,
				//and reason for pending deletion.
				Graph.DrawString(
					PIn.String(dataTablePatient.Rows[i][0].ToString())+" "+PIn.String(dataTablePatient.Rows[i][1].ToString())+Environment.NewLine+
					PIn.String(dataTablePatient.Rows[i][2].ToString())+" "+PIn.String(dataTablePatient.Rows[i][3].ToString())+Environment.NewLine+
					" SSN: "+PIn.String(dataTablePatient.Rows[i][4].ToString())+Environment.NewLine+
					" Birth: "+PIn.Date(dataTablePatient.Rows[i][5].ToString()).ToShortDateString()+Environment.NewLine+
					" Group: "+PIn.String(dataTablePatient.Rows[i][6].ToString()),font,Brushes.Black,
					new RectangleF(20,y+5,215,95));
				//Pending deletion reason.
				for(int j=0;j<collectionStringArraysPatient.Count;j++) {
					if(collectionStringArraysPatient[j][0]==PIn.String(dataTablePatient.Rows[i][8].ToString())) {
						text="REASON FOR DELETION: "+collectionStringArraysPatient[j][7];
						if(collectionStringArraysPatient[j][1].ToUpper()=="F"){
							text="FINALLY DELETED"+Environment.NewLine+text;
						}
						Graph.DrawString(text,font,Brushes.Black,
							new RectangleF(20,y+100,GraphWidth-40,40));
						break;
					}
				}
				//Trojan ID.
				Graph.DrawString(PIn.String(dataTablePatient.Rows[i][8].ToString()),font,Brushes.Black,new RectangleF(240,y+5,85,95));
				//Employer Name and Phone.
				Graph.DrawString(PIn.String(dataTablePatient.Rows[i][9].ToString())+Environment.NewLine+
					PIn.String(dataTablePatient.Rows[i][10].ToString()),font,Brushes.Black,new RectangleF(330,y+5,170,95));
				//Carrier Name and Phone
				Graph.DrawString(PIn.String(dataTablePatient.Rows[i][11].ToString())+Environment.NewLine+
					PIn.String(dataTablePatient.Rows[i][12].ToString()),font,Brushes.Black,
					new RectangleF(500,y+5,150,95));
				//Leave space between records.
				y+=recordHeight+recordSpacing;
				//Watch out for the bottom of each page for the next record.
				if(y+recordHeight>pageBoundry) {
					y=pageBoundry+MarginBottom+20;
					pageBoundry+=PageHeight+MarginBottom;
					text="Patient&Insured";
					font=new Font(font.FontFamily,9);
					Graph.DrawString(text,font,Brushes.Black,20,y);
					text="TrojanID";
					Graph.DrawString(text,font,Brushes.Black,240,y);
					text="Employer";
					Graph.DrawString(text,font,Brushes.Black,330,y);
					text="Carrier";
					Graph.DrawString(text,font,Brushes.Black,500,y);
					y+=20;
				}
			}
		}

		public void PrintDelForTrojan(DataTable dataTableTrojan,Collection<string[]> collectionStringArraysTrojan){
			Text="Trojan Plans Pending Deletion: Contact Trojan";
			ScrollAmount=10;
			UsePageNumbers(new Font(FontFamily.GenericMonospace,8));
			MinimumTimesToPrint=1;
			Landscape=true;
			//Print the header on the report.
			Font font=new Font(FontFamily.GenericMonospace,12);
			string text=PrefC.GetString(PrefName.PracticeTitle);
			SizeF size=Graph.MeasureString(text,font);
			float y=20;
			Graph.DrawString(text,font,Brushes.Black,GraphWidth/2-size.Width/2,y);
			text=DateTime.Today.ToShortDateString();
			size=Graph.MeasureString(text,font);
			Graph.DrawString(text,font,Brushes.Black,GraphWidth-size.Width,y);
			y+=size.Height;
			text="PLANS PENDING DELETION: Please Fax or Mail to Trojan";
			size=Graph.MeasureString(text,font);
			Graph.DrawString(text,font,Brushes.Black,GraphWidth/2-Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			text="Fax: 800-232-9788";
			size=Graph.MeasureString(text,font);
			Graph.DrawString(text,font,Brushes.Black,GraphWidth/2-Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			y+=20;//Skip a line or so.
			text="INSTRUCTIONS: Please complete the information requested below to help Trojan research these plans.\n"+
				"Active Patient: \"Yes\" means the patient has been in the office within the past 6 to 8 months.\n"+
				"Correct Employer: \"Yes\" means the insured currently is insured through this employer.\n"+
				"Correct Carrier: \"Yes\" means the insured currently has coverage with this carrier.";
			Graph.DrawString(text,new Font(new Font(font.FontFamily,10),FontStyle.Bold),Brushes.Black,new RectangleF(0,y,900,500));
			y+=85;//Skip a line or so.
			font=new Font(font.FontFamily,9);
			text="Active\nPatient?";
			Graph.DrawString(text,font,Brushes.Black,5,y);
			text="\nPatient&Insured";
			Graph.DrawString(text,font,Brushes.Black,80,y);
			text="\nTrojanID";
			Graph.DrawString(text,font,Brushes.Black,265,y);
			text="Correct\nEmployer?";
			Graph.DrawString(text,font,Brushes.Black,345,y);
			text="\nEmployer";
			Graph.DrawString(text,font,Brushes.Black,420,y);
			text="Correct\nCarrier?";
			Graph.DrawString(text,font,Brushes.Black,600,y);
			text="\nCarrier";
			Graph.DrawString(text,font,Brushes.Black,670,y);
			y+=30;
			//Use a static height for the records, to keep the math simple.
			float recordHeight=200;
			float recordSpacing=10;
			//Calculate the total number of pages in the report the first time this function is called only.
			if(TotalPages==0) {
				TotalPages=(int)Math.Ceiling((y+recordHeight*dataTableTrojan.Rows.Count+
					((dataTableTrojan.Rows.Count>1)?dataTableTrojan.Rows.Count-1:0)*recordSpacing)/PageHeight);
			}
			float pageBoundry=PageHeight;
			for(int i=0;i<dataTableTrojan.Rows.Count;i++) {
				//Draw the outlines around this record.
				Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(GraphWidth-1,y));
				Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight),new PointF(GraphWidth-1,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(0,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(GraphWidth-1,y),new PointF(GraphWidth-1,y+recordHeight));
				Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight-40),new PointF(GraphWidth-1,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(260,y),new PointF(260,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(340,y),new PointF(340,y+recordHeight-40));
				Graph.DrawLine(Pens.Black,new PointF(595,y),new PointF(595,y+recordHeight-40));
				//Patient active boxes.
				text="Yes No";
				Graph.DrawString(text,font,Brushes.Black,10,y);
				Graph.DrawRectangle(Pens.Black,new Rectangle(15,(int)(y+15),15,15));
				Graph.DrawRectangle(Pens.Black,new Rectangle(40,(int)(y+15),15,15));
				//Install the information for the record into the outline box.
				//Patient name, Guarantor name, guarantor SSN, guarantor birthdate, insurance plan group number,
				//and reason for pending deletion.
				Graph.DrawString(
					PIn.String(dataTableTrojan.Rows[i][0].ToString())+" "+PIn.String(dataTableTrojan.Rows[i][1].ToString())+Environment.NewLine+
					PIn.String(dataTableTrojan.Rows[i][2].ToString())+" "+PIn.String(dataTableTrojan.Rows[i][3].ToString())+Environment.NewLine+
					" SSN: "+PIn.String(dataTableTrojan.Rows[i][4].ToString())+Environment.NewLine+
					" Birth: "+PIn.Date(dataTableTrojan.Rows[i][5].ToString()).ToShortDateString()+Environment.NewLine+
					" Group: "+PIn.String(dataTableTrojan.Rows[i][6].ToString()),font,Brushes.Black,
					new RectangleF(80,y+5,185,95));
				//Pending deletion reason.
				for(int j=0;j<collectionStringArraysTrojan.Count;j++) {
					if(collectionStringArraysTrojan[j][0]==PIn.String(dataTableTrojan.Rows[i][8].ToString())) {
						text="REASON FOR DELETION: "+collectionStringArraysTrojan[j][7];
						if(collectionStringArraysTrojan[j][1].ToUpper()=="F"){
							text="FINALLY DELETED"+Environment.NewLine+text;
						}
						Graph.DrawString(text,font,Brushes.Black,
							new RectangleF(5,y+recordHeight-40,GraphWidth-10,40));
						break;
					}
				}
				//Trojan ID.
				Graph.DrawString(PIn.String(dataTableTrojan.Rows[i][8].ToString()),font,Brushes.Black,new RectangleF(265,y+5,85,95));
				//Correct Employer boxes and arrow.
				text="Yes No";
				Graph.DrawString(text,font,Brushes.Black,345,y);
				Graph.DrawRectangle(Pens.Black,new Rectangle(350,(int)(y+15),15,15));
				Graph.DrawRectangle(Pens.Black,new Rectangle(375,(int)(y+15),15,15));
				//Employer Name and Phone.
				Graph.DrawString(PIn.String(dataTableTrojan.Rows[i][9].ToString())+Environment.NewLine+
					PIn.String(dataTableTrojan.Rows[i][10].ToString()),font,Brushes.Black,new RectangleF(420,y+5,175,95));
				//New employer information if necessary.
				text="New\nEmployer:";
				Graph.DrawString(text,font,Brushes.Black,345,y+85);
				Graph.DrawLine(Pens.Black,415,y+110,590,y+110);
				Graph.DrawLine(Pens.Black,415,y+125,590,y+125);
				text="Phone:";
				Graph.DrawString(text,font,Brushes.Black,345,y+130);
				Graph.DrawLine(Pens.Black,415,y+140,590,y+140);
				//Correct Carrier boxes and arrow.
				text="Yes No";
				Graph.DrawString(text,font,Brushes.Black,600,y);
				Graph.DrawRectangle(Pens.Black,new Rectangle(605,(int)(y+15),15,15));
				Graph.DrawRectangle(Pens.Black,new Rectangle(630,(int)(y+15),15,15));
				//Carrier Name and Phone
				Graph.DrawString(PIn.String(dataTableTrojan.Rows[i][11].ToString())+Environment.NewLine+
					PIn.String(dataTableTrojan.Rows[i][12].ToString()),font,Brushes.Black,
					new RectangleF(670,y+5,225,95));
				//New carrier information if necessary.
				text="New\nCarrier:";
				Graph.DrawString(text,font,Brushes.Black,600,y+85);
				Graph.DrawLine(Pens.Black,670,y+110,895,y+110);
				Graph.DrawLine(Pens.Black,670,y+125,895,y+125);
				text="Phone:";
				Graph.DrawString(text,font,Brushes.Black,600,y+130);
				Graph.DrawLine(Pens.Black,670,y+140,895,y+140);
				//Leave space between records.
				y+=recordHeight+recordSpacing;
				//Watch out for the bottom of each page for the next record.
				if(y+recordHeight>pageBoundry) {
					y=pageBoundry+MarginBottom+20;
					pageBoundry+=PageHeight+MarginBottom;
					text="Active\nPatient?";
					Graph.DrawString(text,font,Brushes.Black,5,y);
					text="\nPatient&Insured";
					Graph.DrawString(text,font,Brushes.Black,80,y);
					text="\nTrojanID";
					Graph.DrawString(text,font,Brushes.Black,265,y);
					text="Correct\nEmployer?";
					Graph.DrawString(text,font,Brushes.Black,345,y);
					text="\nEmployer";
					Graph.DrawString(text,font,Brushes.Black,420,y);
					text="Correct\nCarrier?";
					Graph.DrawString(text,font,Brushes.Black,600,y);
					text="\nCarrier";
					Graph.DrawString(text,font,Brushes.Black,670,y);
					y+=30;
				}
			}
		}

		public void UsePageNumbers(Font font){
			pageNumberFont=font;
		}

		private void PrintCustom(){
			if(printGenerator==null){
				return;
			}
			printPanel.Clear();
			Invoke(printGenerator,new object[] { this });//Call the custom printing code.
			//Print page numbers.
			if(pageNumberFont!=null){
				for(int i=0;i<totalPages;i++){
					string text="Page "+(i+1);
					SizeF size=Graph.MeasureString(text,pageNumberFont);
					Graph.DrawString(text,pageNumberFont,Brushes.Black,
						new PointF(GraphWidth-size.Width,i*(pageHeight+MarginBottom)));
				}
			}
		}

		private int CurPage(){
			return (vScroll.Value-vScroll.Minimum+1)/pageHeight;
		}

		private void CalculatePageOffset(){
			printPanel.Origin=new Point(0,-vScroll.Value);
			labPageNum.Text="Page: "+(CurPage()+1)+"\\"+totalPages;
		}

		private void CalculateVScrollMax(){
			if(totalPages>1){
				vScroll.Maximum=pageHeight*(totalPages-1)-1+vScroll.Minimum;
			}else{
				vScroll.Maximum=vScroll.Minimum;
			}
		}

		private void MoveScrollBar(int amount){
			int val=vScroll.Value+amount;
			if(val<vScroll.Minimum){
				val=vScroll.Minimum;
			}else if(val>vScroll.Maximum){
				val=vScroll.Maximum;
			}
			vScroll.Value=val;
		}

		public int ScrollAmount{
			get{ return vScroll.SmallChange; }
			set{ vScroll.SmallChange=value; }
		}

		///<summary>Window size can change based on landscape mode and possibly other things in the future (perhaps different paper sizes).</summary>
		private void CheckWindowSize(){
			if(landscape){
				this.Width=942;
				PageHeight=650;
			}else{
				this.Width=692;
				PageHeight=900;
			}
			printPanel.Width=this.Width-42;
			vScroll.Location=new Point(this.Width-29,33);
		}

		public bool Landscape{
			get{ return landscape; }
			set{ landscape=value; CheckWindowSize(); }
		}

		public int MarginBottom{
			get{ return (printerGraph!=null)?(1100-printerMargins.Bottom):0; }
		}

		public Graphics Graph{
			get{ return (printerGraph!=null)?printerGraph:printPanel.backBuffer; }
		}

		public int PageHeight {
			get { return pageHeight; }
			set { pageHeight=value; CalculateVScrollMax(); }
		}

		///<summary>Must be set by the external printing algorithm in order to get page numbers working properly.</summary>
		public int TotalPages{
			get{ return totalPages; }
			set { totalPages=value; CalculatePageOffset(); labPageNum.Visible=(totalPages>0); CalculateVScrollMax(); }
		}

		public int GraphWidth{
			get{ return (printerGraph!=null)?printerMargins.Width:printPanel.Width; }
			set{ printPanel.Width=value; }
		}

		public int MinimumTimesToPrint{
			get{ return minimumTimesToPrint; }
			set{ minimumTimesToPrint=value; }
		}

		private void Display(){
			CalculatePageOffset();
			PrintCustom();
			printPanel.Invalidate(true);
		}

		private void butNextPage_Click(object sender,EventArgs e) {
			MoveScrollBar((CurPage()+1)*pageHeight-vScroll.Value);
			Display();
		}

		private void butPreviousPage_Click(object sender,EventArgs e) {
			if(vScroll.Value%pageHeight==0){
				MoveScrollBar(-pageHeight);
			}else{
				MoveScrollBar(CurPage()*pageHeight-vScroll.Value);
			}
			Display();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			curPrintPage=0;
			PrintoutOrientation orient=(landscape?PrintoutOrientation.Landscape:PrintoutOrientation.Portrait);
			if(PrinterL.TryPrint(pd1_PrintPage,printoutOrigin:PrintoutOrigin.AtMargin,printoutOrientation:orient)) {
				numTimesPrinted++;
			}
			printerGraph=null;
			Display();
		}

		private void pd1_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			printerGraph=e.Graphics;
			printerMargins=e.MarginBounds;
			printerGraph.TranslateTransform(0,-curPrintPage*(pageHeight+MarginBottom));
			PrintCustom();
			curPrintPage++;
			e.HasMorePages=(printGenerator!=null)&&(curPrintPage<totalPages);
		}

		private void vScroll_Scroll(object sender,ScrollEventArgs e) {
			Display();
		}

		private void FormPrintReport_FormClosing(object sender,FormClosingEventArgs e) {
			if(numTimesPrinted<minimumTimesToPrint){
				if(MessageBox.Show("WARNING: You should print this document at least "+
					(minimumTimesToPrint==1?"one time.":(minimumTimesToPrint+" times."))+
					"You may not be able to print this document again if you close it now. Are you sure you want to close this document?","",
					MessageBoxButtons.YesNo)==DialogResult.No){
					e.Cancel=true;
					return;
				}
			}
		}

	}
}