using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental
{
	/// <summary>
	/// Summary description for FormRpApptWithPhones.
	/// </summary>
	public partial class FormRpBirthday : FormODBase {
		private int pagesPrinted;
		private ErrorProvider errorProvider1=new ErrorProvider();
		private DataTable BirthdayTable;
		private int patientsPrinted;

		///<summary></summary>
		public FormRpBirthday()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpBirthday_Load(object sender, System.EventArgs e){
			SetNextMonth();
			textPostcardMsg.Text=PrefC.GetString(PrefName.BirthdayPostcardMsg);
			Plugins.HookAddCode(this,"FormRpBirthday.Load_end");
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(!validDateFrom.IsValid() || !validDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(validDateFrom.Text);
			if(dateFrom.Year < 1880) {
				MsgBox.Show(this,"Please fix the From date first.");
				return;
			}
			DateTime dateTo=PIn.Date(validDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			validDateFrom.Text=dateFrom.AddMonths(-1).ToShortDateString();
			dateTo=dateTo.AddMonths(-1);
			validDateTo.Text=dateTo.ToShortDateString();
			if(toLastDay){
				validDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void butMonth_Click(object sender, System.EventArgs e) {
			SetNextMonth();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(!validDateFrom.IsValid() || !validDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(validDateFrom.Text);
			DateTime dateTo=PIn.Date(validDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			validDateFrom.Text=dateFrom.AddMonths(1).ToShortDateString();
			dateTo=dateTo.AddMonths(1);
			validDateTo.Text=dateTo.ToShortDateString();
			if(toLastDay){
				validDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void SetNextMonth(){
			validDateFrom.Text
				=new DateTime(DateTime.Today.AddMonths(1).Year,DateTime.Today.AddMonths(1).Month,1)
				.ToShortDateString();
			validDateTo.Text
				=new DateTime(DateTime.Today.AddMonths(2).Year,DateTime.Today.AddMonths(2).Month,1).AddDays(-1)
				.ToShortDateString();
			validDateFrom.Validate();
			validDateTo.Validate();
		}
		
		private void butSave_Click(object sender,EventArgs e) {
			if(Prefs.UpdateString(PrefName.BirthdayPostcardMsg,textPostcardMsg.Text)){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butPostcards_Click(object sender,EventArgs e) {
			if(!validDateFrom.IsValid() || !validDateTo.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime dateFrom=PIn.Date(validDateFrom.Text);
			DateTime dateTo=PIn.Date(validDateTo.Text);
			if(dateTo < dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			if(dateFrom.AddYears(1) <= dateTo) {
				MsgBox.Show(this,"Date range must not exceed 1 year.");
				return;
			}
			BirthdayTable=RpBirthday.GetBirthdayTable(dateFrom,dateTo);
			if(BirthdayTable.Rows.Count==0) {
				MsgBox.Show(this,"No postcards to preview.");
				return;
			}
			pagesPrinted=0;
			patientsPrinted=0;
			PaperSize paperSize;
			PrintoutOrientation orient=PrintoutOrientation.Default;
			if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==1) {
				paperSize=new PaperSize("Postcard",400,600);
				orient=PrintoutOrientation.Landscape;
			}
			else if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==3) {
				paperSize=new PaperSize("Postcard",850,1100);
			}
			else {//4
				paperSize=new PaperSize("Postcard",850,1100);
				orient=PrintoutOrientation.Landscape;
			}
			int totalPages=(int)Math.Ceiling((double)BirthdayTable.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet));
			PrinterL.TryPreview(pdCards_PrintPage,
				Lan.g(this,"Birthday report postcards printed"),
				PrintSituation.Postcard,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin,
				paperSize,
				orient,
				totalPages
			);
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender,PrintPageEventArgs ev) {
			int totalPages=(int)Math.Ceiling((double)BirthdayTable.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet));
			Graphics g=ev.Graphics;
			float yPos=0;//these refer to the upper left origin of each postcard
			float xPos=0;
			string str;
			int age;
			DateTime birthdate;
			while(yPos<ev.PageBounds.Height-100 && patientsPrinted<BirthdayTable.Rows.Count) {
				//Return Address--------------------------------------------------------------------------
				if(PrefC.GetBool(PrefName.RecallCardsShowReturnAdd)) {
					str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
					g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
					str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
					if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
						str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
					}
					str+=PrefC.GetString(PrefName.PracticeCity)+",  "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
					string phone=PrefC.GetString(PrefName.PracticePhone);
					if(CultureInfo.CurrentCulture.Name=="en-US"&& phone.Length==10) {
						str+="("+phone.Substring(0,3)+")"+phone.Substring(3,3)+"-"+phone.Substring(6);
					}
					else {//any other phone format
						str+=phone;
					}
					g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,xPos+45,yPos+75);
				}
				//Body text-------------------------------------------------------------------------------
				str=textPostcardMsg.Text;
				if(BirthdayTable.Rows[patientsPrinted]["Preferred"].ToString()!=""){
					str=str.Replace("?FName",BirthdayTable.Rows[patientsPrinted]["Preferred"].ToString());
				}
				else{
					str=str.Replace("?FName",BirthdayTable.Rows[patientsPrinted]["FName"].ToString());
				}
				birthdate=PIn.Date(BirthdayTable.Rows[patientsPrinted]["Birthdate"].ToString());
				//age=Shared.DateToAge(birthdate,PIn.PDate(textDateTo.Text).AddDays(1));//age on the day after the range
				age=PIn.Int(BirthdayTable.Rows[patientsPrinted]["Age"].ToString());
				str=str.Replace("?AgeOrdinal",Shared.NumberToOrdinal(age));
				str=str.Replace("?Age",age.ToString());
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,10),Brushes.Black,new RectangleF(xPos+45,yPos+180,250,190));
				//Patient's Address-----------------------------------------------------------------------
				str=BirthdayTable.Rows[patientsPrinted]["FName"].ToString()+" "
					//+BirthdayTable.Rows[patientsPrinted]["MiddleI"].ToString()+" "
					+BirthdayTable.Rows[patientsPrinted]["LName"].ToString()+"\r\n"
					+BirthdayTable.Rows[patientsPrinted]["Address"].ToString()+"\r\n";
				if(BirthdayTable.Rows[patientsPrinted]["Address2"].ToString()!="") {
					str+=BirthdayTable.Rows[patientsPrinted]["Address2"].ToString()+"\r\n";
				}
				str+=BirthdayTable.Rows[patientsPrinted]["City"].ToString()+", "
					+BirthdayTable.Rows[patientsPrinted]["State"].ToString()+"   "
					+BirthdayTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,xPos+320,yPos+240);
				if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==1) {
					yPos+=400;
				}
				else if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==3) {
					yPos+=366;
				}
				else {//4
					xPos+=550;
					if(xPos>1000) {
						xPos=0;
						yPos+=425;
					}
				}
				patientsPrinted++;
			}//while
			pagesPrinted++;
			if(pagesPrinted==totalPages) {
				ev.HasMorePages=false;
				pagesPrinted=0;
				patientsPrinted=0;
			}
			else {
				ev.HasMorePages=true;
			}
		}

		private void butReport_Click(object sender, System.EventArgs e){
			if(!validDateFrom.IsValid() || !validDateTo.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(validDateFrom.Text=="" || validDateTo.Text=="") {
				MsgBox.Show(this,"Dates can not be blank");
				return;
			}
			DateTime dateFrom=PIn.Date(validDateFrom.Text);
			DateTime dateTo=PIn.Date(validDateTo.Text);
			if(dateTo < dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			if(dateFrom.AddYears(1) <= dateTo) {
				MsgBox.Show(this,"Date range must not exceed 1 year.");
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Birthdays");
			report.AddTitle("Title",Lan.g(this,"Birthdays"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",dateFrom.ToShortDateString()+" - "+dateTo.ToShortDateString(),fontSubTitle);
			QueryObject query=report.AddQuery(RpBirthday.GetBirthdayTable(dateFrom,dateTo),"","",SplitByKind.None,1,true);
			query.AddColumn("LName",90,FieldValueType.String,font);
			query.AddColumn("FName",90,FieldValueType.String,font);
			query.AddColumn("Preferred",90,FieldValueType.String,font);
			query.AddColumn("Address",90,FieldValueType.String,font);
			query.AddColumn("Address2",90,FieldValueType.String,font);
			query.AddColumn("City",75,FieldValueType.String,font);
			query.AddColumn("State",60,FieldValueType.String,font);
			query.AddColumn("Zip",75,FieldValueType.String,font);
			query.AddColumn("Birthdate",75,FieldValueType.Date,font);
			query.GetColumnDetail("Birthdate").StringFormat="d";
			query.AddColumn("Age",45,FieldValueType.Integer,font);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		
	}
}
