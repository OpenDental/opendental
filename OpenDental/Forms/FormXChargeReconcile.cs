using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormXChargeReconcile:FormODBase {

		public FormXChargeReconcile() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butImport_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			using OpenFileDialog Dlg=new OpenFileDialog();
			if(Directory.Exists(@"C:\X-Charge\")) {
				Dlg.InitialDirectory=@"C:\X-Charge\";
			}
			else if(Directory.Exists(@"C:\")) {
				Dlg.InitialDirectory=@"C:\";
			}
			if(Dlg.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			if(!File.Exists(Dlg.FileName)) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"File not found");
				return;
			}
			XChargeTransaction trans=new XChargeTransaction();
			string[] fields;
			XChargeTransaction transCheck;
			using(StreamReader sr=new StreamReader(Dlg.FileName)) {
				Cursor=Cursors.WaitCursor;
				string line=sr.ReadLine();
				while(line!=null) {
					fields=line.Split(new string[1] { "," },StringSplitOptions.None);
					if(fields.Length < 16 || fields.All(x => x=="")){
						//This occurs when the XCharge transaction is blank. For example, ,,,,,,,,,,,,,,,, which is what is outputted by XCharge 
						//when no transactions occurred for a day.
						line=sr.ReadLine();
						continue;
					}
					trans.TransType=fields[0];
					fields[1]=fields[1].Replace("$","");
					if(fields[1].Contains("(")) {
						fields[1]=fields[1].TrimStart('(');
						fields[1]=fields[1].TrimEnd(')');
						fields[1]=fields[1].Insert(0,"-");
					}
					trans.Amount=PIn.Double(fields[1]);
					trans.CCEntry=fields[2];
					trans.PatNum=0;
					if(!string.IsNullOrWhiteSpace(fields[3])) {
						trans.PatNum=PIn.Long(fields[3].Substring(3));//remove "PAT" from the beginning of the string
					}
					trans.Result=fields[4];
					trans.ClerkID=fields[5];
					trans.ResultCode=fields[7];
					trans.Expiration=fields[8];
					trans.CCType=fields[9];
					trans.CreditCardNum=fields[10];
					trans.BatchNum=fields[11];
					trans.ItemNum=fields[12];
					trans.ApprCode=fields[13];
					trans.TransactionDateTime=PIn.Date(fields[14])
						.AddHours(PIn.Double(fields[15].Substring(0,2)))
						.AddMinutes(PIn.Double(fields[15].Substring(3,2)))
						.AddSeconds(PIn.Double(fields[15].Substring(6,2)));
					transCheck=XChargeTransactions.GetOneMatch(trans.BatchNum,trans.ItemNum,trans.PatNum,trans.TransactionDateTime,trans.TransType);
					if(transCheck!=null && trans.Result!="AP DUPE" 
						&& CompareDouble.IsEqual(transCheck.Amount,trans.Amount)
						//Before 17.1.19, we did not store the seconds value of the TransactionDateTime, so we consider it a match if Second==0.
						&& (transCheck.TransactionDateTime==trans.TransactionDateTime || transCheck.TransactionDateTime.Second==0))						
					{
						XChargeTransactions.Delete(transCheck.XChargeTransactionNum);
						XChargeTransactions.Insert(trans);
					}
					else {
						XChargeTransactions.Insert(trans);
					}
					line=sr.ReadLine();
				}
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
		}

		private void butViewImported_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query="SELECT TransactionDateTime,TransType,ClerkID,ItemNum,PatNum,CreditCardNum,Expiration,Result,"
				+"CASE WHEN ResultCode IN('000','010') THEN Amount ELSE 0 END AS Amount "
				+"FROM xchargetransaction "
				+"WHERE "+DbHelper.DtimeToDate("TransactionDateTime")+" BETWEEN "+POut.Date(date1.SelectionStart)+" AND "+POut.Date(date2.SelectionStart);
			using FormQuery FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=true;
			FormQuery2.SubmitReportQuery();
			report.Title="XCharge Transactions From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString();
			report.SubTitle.Add(PrefC.GetString(PrefName.PracticeTitle));
			report.SetColumn(this,0,"Transaction Date/Time",170);
			report.SetColumn(this,1,"Transaction Type",120);
			report.SetColumn(this,2,"Clerk ID",80);
			report.SetColumn(this,3,"Item#",50);
			report.SetColumn(this,4,"Pat",50);//This name is used to ensure FormQuery does not replace the patnum with the patient name.
			report.SetColumn(this,5,"Credit Card Num",140);
			report.SetColumn(this,6,"Exp",50);
			report.SetColumn(this,7,"Result",50);
			report.SetColumn(this,8,"Amount",60,HorizontalAlignment.Right);
			Cursor=Cursors.Default;
			FormQuery2.ShowDialog();
		}

		private void butPayments_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query="SET @pos=0; "
				+"SELECT @pos:=@pos+1 AS 'Count',patient.PatNum,LName,FName,DateEntry,PayDate,PayNote,PayAmt "
				+"FROM patient INNER JOIN payment ON payment.PatNum=patient.PatNum "
				+"INNER JOIN ("
					+"SELECT ClinicNum,PropertyValue AS PaymentType FROM programproperty "
					+"WHERE ProgramNum="+POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+" AND PropertyDesc='PaymentType'"
				+") paytypes ON paytypes.ClinicNum=payment.ClinicNum AND paytypes.PaymentType=payment.PayType "
				//Must be DateEntry here. PayDate will not work with recurring charges
				+"WHERE DateEntry BETWEEN "+POut.Date(date1.SelectionStart)+" AND "+POut.Date(date2.SelectionStart)+" "
				+"ORDER BY Count ASC";
			using FormQuery FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=true;
			FormQuery2.SubmitReportQuery();
			report.Title="Payments From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString();
			report.SubTitle.Add(PrefC.GetString(PrefName.PracticeTitle));
			report.SetColumn(this,0,"Count",50);
			report.SetColumn(this,1,"Pat",50);//This name is used to ensure FormQuery does not replace the patnum with the patient name.
			report.SetColumn(this,2,"LName",100);
			report.SetColumn(this,3,"FName",100);
			report.SetColumn(this,4,"DateEntry",100);
			report.SetColumn(this,5,"PayDate",100);
			report.SetColumn(this,6,"PayNote",150);
			report.SetColumn(this,7,"PayAmt",70,HorizontalAlignment.Right);
			Cursor=Cursors.Default;
			FormQuery2.ShowDialog();
		}

		private void butMissing_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex report=new ReportComplex(true,false);
			report.ReportName="Missing";
			report.AddTitle("Title","XCharge Transactions From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString(),fontTitle);
			report.GetTitle("Title").IsUnderlined=true;
			report.AddSubTitle("SubTitle","No Matching Transaction Found in Open Dental",fontSubTitle);
			QueryObject query;
			DataTable dt=XChargeTransactions.GetMissingPaymentsTable(date1.SelectionStart,date2.SelectionStart);
			query=report.AddQuery(dt,"Missing Payments","",SplitByKind.None,1,true);//Valid entries to count have result code 0
			query.AddColumn("Transaction Date/Time",170,FieldValueType.String,font);
			query.AddColumn("Transaction Type",120,FieldValueType.String,font);
			query.AddColumn("Clerk ID",80,FieldValueType.String,font);
			query.AddColumn("Item#",50,FieldValueType.String,font);
			query.AddColumn("Pat",50,FieldValueType.String,font);
			query.AddColumn("Credit Card Num",140,FieldValueType.String,font);
			query.AddColumn("Exp",50,FieldValueType.String,font);
			query.AddColumn("Result",50,FieldValueType.String,font);
			query.AddColumn("Amount",60,FieldValueType.Number,font);
			query.GetColumnHeader("Amount").ContentAlignment=ContentAlignment.MiddleRight;
			Cursor=Cursors.Default;
			if(!report.SubmitQueries(isShowMessage:true)) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
		}

		private void butExtra_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex report=new ReportComplex(true,false);
			report.ReportName="Extra Payments";
			report.AddTitle("Title","Payments From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString(),fontTitle);
			report.GetTitle("Title").IsUnderlined=true;
			report.AddSubTitle("SubTitle","No Matching X-Charge Transactions for these Payments",fontSubTitle);
			QueryObject query;
			DataTable dt=XChargeTransactions.GetMissingXTransTable(date1.SelectionStart,date2.SelectionStart);
			query=report.AddQuery(dt,"Extra Payments","",SplitByKind.None,1,true);
			query.AddColumn("Pat",50,FieldValueType.String,font);
			query.AddColumn("LName",100,FieldValueType.String,font);
			query.AddColumn("FName",100,FieldValueType.String,font);
			query.AddColumn("DateEntry",90,FieldValueType.Date,font);
			query.AddColumn("PayDate",90,FieldValueType.Date,font);
			query.AddColumn("PayNote",210,FieldValueType.String,font);
			query.AddColumn("PayAmt",70,FieldValueType.Number,font);
			query.GetColumnHeader("PayAmt").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("PayAmt").ContentAlignment=ContentAlignment.MiddleRight;
			Cursor=Cursors.Default;
			if(!report.SubmitQueries(isShowMessage:true)) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}