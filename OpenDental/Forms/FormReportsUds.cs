using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormReportsUds:FormODBase {
		private DateTime _dateFrom;
		private DateTime _dateTo;

		public FormReportsUds() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReportsUds_Load(object sender,EventArgs e) {
			textFrom.Text=DateTime.Now.AddYears(-1).ToShortDateString();
			textTo.Text=DateTime.Now.ToShortDateString();
		}

		private void butPatByZip_Click(object sender,EventArgs e) {
			if(!DateIsValid()) {
				return;
			}
			ReportSimpleGrid reportSimpleGrid=new ReportSimpleGrid();
			reportSimpleGrid.Query="SELECT SUBSTR(Zip,1,5) 'Zip Code',COUNT(*) 'Patients' "//Column headings "Zip Code" and "Patients" are provided by the USD 2010 Manual.
				+"FROM patient pat "
				+"WHERE "+DbHelper.Regexp("Zip","^[0-9]{5}")+" "//Starts with five numbers
				+"AND PatNum IN ( "
					+"SELECT PatNum FROM procedurelog "
					+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
					+"AND DateEntryC >= "+POut.Date(_dateFrom)+" "
					+"AND DateEntryC <= "+POut.Date(_dateTo)+") "
				+"GROUP BY Zip "
				+"HAVING COUNT(*) >= 10 "//Has more than 10 patients in that zip code for the given time frame.
				+"ORDER BY Zip";
			using FormQuery formQuery=new FormQuery(reportSimpleGrid);
			formQuery.IsReport=true;
			formQuery.SubmitQuery();
			formQuery.textQuery.Text=reportSimpleGrid.Query;
			reportSimpleGrid.Title="Patients By ZIP CODE";
			reportSimpleGrid.SubTitle.Add("From "+_dateFrom.ToShortDateString()+" to "+_dateTo.ToShortDateString());
			reportSimpleGrid.Summary.Add("Other Zip Codes: "+Patients.GetZipOther(_dateFrom,_dateTo));
			reportSimpleGrid.Summary.Add("Unknown Residence: "+Patients.GetZipUnknown(_dateFrom,_dateTo));
			reportSimpleGrid.Summary.Add("TOTAL: "+Patients.GetPatCount(_dateFrom,_dateTo));
			formQuery.ShowDialog();
		}

		private void but3A_Click(object sender,EventArgs e) {//TODO: Implement ODprintout pattern
			if(!DateIsValid()) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			PrintDocument printDocument=new PrintDocument();
			printDocument.PrintPage+=new PrintPageEventHandler(this.pdAgeGender_PrintPage);
			//using FormPrintPreview printPreview=new FormPrintPreview(PrintSituation.Default,pd,1,0,"UDS reporting 3A-AgeGender printed");
			//printPreview.ShowDialog();
		}

		private void pdAgeGender_PrintPage(object sender,PrintPageEventArgs e){
			if(!DateIsValid()) {
				return;
			}
			Graphics g=e.Graphics;
			int width=e.PageBounds.Width;
			int height=e.PageBounds.Height;
			int[,] table3A=new int[39,2];
			for(int i=0;i<25;i++) {//fields 1-25, index 0-24
				table3A[i,0]=Patients.GetAgeGenderCount(i,i+1,PatientGender.Male,_dateFrom,_dateTo);
				table3A[i,1]=Patients.GetAgeGenderCount(i,i+1,PatientGender.Female,_dateFrom,_dateTo);
			}
			int age;
			for(int i=0;i<13;i++) {//fields 26-37, index 25-36
				age=25+5*i;
				table3A[25+i,0]=Patients.GetAgeGenderCount(age,age+5,PatientGender.Male,_dateFrom,_dateTo);//For i=0 give qty male ages 25-29
				table3A[25+i,1]=Patients.GetAgeGenderCount(age,age+5,PatientGender.Female,_dateFrom,_dateTo);//For i=0 give qty female ages 25-29
			}
			table3A[37,0]=Patients.GetAgeGenderCount(85,200,PatientGender.Male,_dateFrom,_dateTo);
			table3A[37,1]=Patients.GetAgeGenderCount(85,200,PatientGender.Female,_dateFrom,_dateTo);
			table3A[38,0]=Patients.GetAgeGenderCount(0,200,PatientGender.Male,_dateFrom,_dateTo);
			table3A[38,1]=Patients.GetAgeGenderCount(0,200,PatientGender.Female,_dateFrom,_dateTo);
			using Bitmap bitmap=Properties.Resources.UDS3a;
			int xPos=(width - bitmap.Width)/2;
			int yPos=(height - bitmap.Height)/2;
			g.DrawImage(bitmap,xPos,yPos,bitmap.Width,bitmap.Height);
			xPos=540;
			string qty;
			Font font=new Font(FontFamily.GenericSansSerif,9);
			for(int i=0;i<table3A.GetLength(0);i++){
				if(i==table3A.GetLength(0)-1) {
					font=new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold);
				}
				yPos=245+(int)(17.72*i);
				qty=table3A[i,0].ToString();
				g.DrawString(qty,font,Brushes.Black,xPos-g.MeasureString(qty,font).Width,yPos);//Aligns right
				qty=table3A[i,1].ToString();
				g.DrawString(qty,font,Brushes.Black,xPos-g.MeasureString(qty,font).Width+125,yPos);//Aligns right
			}
		}

		private bool DateIsValid() {
			_dateFrom=PIn.Date(textFrom.Text);
			_dateTo=PIn.Date(textTo.Text);
			if(_dateFrom==DateTime.MinValue || _dateTo==DateTime.MinValue) {
				MsgBox.Show(this,"Please enter valid To and From dates.");
				return false;
			}
			return true;
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}







	}
}