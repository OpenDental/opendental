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
			string importFilePath;
			if(ODCloudClient.IsAppStream) {
				importFilePath=ODCloudClient.ImportFileForCloud();
				if(importFilePath.IsNullOrEmpty()) {
					return; //User cancelled out of OpenFileDialog
				}
			}
			else {
				using OpenFileDialog openFileDialog=new OpenFileDialog();
				if(Directory.Exists(@"C:\X-Charge\")) {
					openFileDialog.InitialDirectory=@"C:\X-Charge\";
				}
				else if(Directory.Exists(@"C:\")) {
					openFileDialog.InitialDirectory=@"C:\";
				}
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
				importFilePath=openFileDialog.FileName;
			}
			if(!File.Exists(importFilePath)) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"File not found");
				return;
			}
			XChargeTransaction xChargeTransaction=new XChargeTransaction();
			string[] stringArrayfields;
			XChargeTransaction xChargeTransactionCheck;
			using StreamReader streamReader=new StreamReader(importFilePath);
			Cursor=Cursors.WaitCursor;
			string line=streamReader.ReadLine();
			while(line!=null) {
				stringArrayfields=line.Split(new string[1] { "," },StringSplitOptions.None);
				if(stringArrayfields.Length < 16 || stringArrayfields.All(x => x=="")){
					//This occurs when the XCharge transaction is blank. For example, ,,,,,,,,,,,,,,,, which is what is outputted by XCharge 
					//when no transactions occurred for a day.
					line=streamReader.ReadLine();
					continue;
				}
				xChargeTransaction.TransType=stringArrayfields[0];
				stringArrayfields[1]=stringArrayfields[1].Replace("$","");
				if(stringArrayfields[1].Contains("(")) {
					stringArrayfields[1]=stringArrayfields[1].TrimStart('(');
					stringArrayfields[1]=stringArrayfields[1].TrimEnd(')');
					stringArrayfields[1]=stringArrayfields[1].Insert(0,"-");
				}
				xChargeTransaction.Amount=PIn.Double(stringArrayfields[1]);
				xChargeTransaction.CCEntry=stringArrayfields[2];
				xChargeTransaction.PatNum=0;
				if(!string.IsNullOrWhiteSpace(stringArrayfields[3]) && stringArrayfields[3].Length > 3) {
					xChargeTransaction.PatNum=PIn.Long(stringArrayfields[3].Substring(3));//remove "PAT" from the beginning of the string
				}
				xChargeTransaction.Result=stringArrayfields[4];
				xChargeTransaction.ClerkID=stringArrayfields[5];
				stringArrayfields[6]=stringArrayfields[6].Replace("$","");
				if(stringArrayfields[6].Contains("(")) {
					stringArrayfields[6]=stringArrayfields[6].TrimStart('(');
					stringArrayfields[6]=stringArrayfields[6].TrimEnd(')');
					stringArrayfields[6]=stringArrayfields[6].Insert(0,"-");
				}
				xChargeTransaction.BatchTotal=PIn.Double(stringArrayfields[6]);
				xChargeTransaction.ResultCode=stringArrayfields[7];
				xChargeTransaction.Expiration=stringArrayfields[8];
				xChargeTransaction.CCType=stringArrayfields[9];
				xChargeTransaction.CreditCardNum=stringArrayfields[10];
				xChargeTransaction.BatchNum=stringArrayfields[11];
				xChargeTransaction.ItemNum=stringArrayfields[12];
				xChargeTransaction.ApprCode=stringArrayfields[13];
				xChargeTransaction.TransactionDateTime=PIn.Date(stringArrayfields[14])
					.AddHours(PIn.Double(stringArrayfields[15].Substring(0,2)))
					.AddMinutes(PIn.Double(stringArrayfields[15].Substring(3,2)))
					.AddSeconds(PIn.Double(stringArrayfields[15].Substring(6,2)));
				xChargeTransactionCheck=XChargeTransactions.GetOneMatch(xChargeTransaction.BatchNum,xChargeTransaction.ItemNum,xChargeTransaction.PatNum,xChargeTransaction.TransactionDateTime,xChargeTransaction.TransType);
				if(xChargeTransactionCheck!=null && xChargeTransaction.Result!="AP DUPE" 
					&& CompareDouble.IsEqual(xChargeTransactionCheck.Amount,xChargeTransaction.Amount)
					//Before 17.1.19, we did not store the seconds value of the TransactionDateTime, so we consider it a match if Second==0.
					&& (xChargeTransactionCheck.TransactionDateTime==xChargeTransaction.TransactionDateTime || xChargeTransactionCheck.TransactionDateTime.Second==0))						
				{
					XChargeTransactions.Delete(xChargeTransactionCheck.XChargeTransactionNum);
					XChargeTransactions.Insert(xChargeTransaction);
				}
				else {
					XChargeTransactions.Insert(xChargeTransaction);
				}
				line=streamReader.ReadLine();
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done.");
		}

		private void butViewImported_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			ReportSimpleGrid reportSimpleGrid=new ReportSimpleGrid();
			reportSimpleGrid.Query="SELECT TransactionDateTime,TransType,ClerkID,ItemNum,PatNum,CreditCardNum,Expiration,Result,"
				+"CASE WHEN ResultCode IN('000','010') THEN Amount ELSE 0 END AS Amount "
				+"FROM xchargetransaction "
				+"WHERE "+DbHelper.DtimeToDate("TransactionDateTime")+" BETWEEN "+POut.Date(date1.SelectionStart)+" AND "+POut.Date(date2.SelectionStart);
			using FormQuery formQuery2=new FormQuery(reportSimpleGrid);
			formQuery2.IsReport=true;
			formQuery2.SubmitReportQuery();
			reportSimpleGrid.Title="XCharge Transactions From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString();
			reportSimpleGrid.SubTitle.Add(PrefC.GetString(PrefName.PracticeTitle));
			reportSimpleGrid.SetColumn(this,0,"Transaction Date/Time",170);
			reportSimpleGrid.SetColumn(this,1,"Transaction Type",120);
			reportSimpleGrid.SetColumn(this,2,"Clerk ID",80);
			reportSimpleGrid.SetColumn(this,3,"Item#",50);
			reportSimpleGrid.SetColumn(this,4,"Pat",50);//This name is used to ensure FormQuery does not replace the patnum with the patient name.
			reportSimpleGrid.SetColumn(this,5,"Credit Card Num",140);
			reportSimpleGrid.SetColumn(this,6,"Exp",50);
			reportSimpleGrid.SetColumn(this,7,"Result",50);
			reportSimpleGrid.SetColumn(this,8,"Amount",60,HorizontalAlignment.Right);
			Cursor=Cursors.Default;
			formQuery2.ShowDialog();
		}

		private void butPayments_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			ReportSimpleGrid reportSimpleGrid=new ReportSimpleGrid();
			reportSimpleGrid.Query="SET @pos=0; "
				+"SELECT @pos:=@pos+1 AS 'Count',patient.PatNum,LName,FName,DateEntry,PayDate,PayNote,PayAmt "
				+"FROM patient INNER JOIN payment ON payment.PatNum=patient.PatNum "
				+"INNER JOIN ("
					+"SELECT ClinicNum,PropertyValue AS PaymentType FROM programproperty "
					+"WHERE ProgramNum="+POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+" AND PropertyDesc='PaymentType'"
				+") paytypes ON paytypes.ClinicNum=payment.ClinicNum AND paytypes.PaymentType=payment.PayType "
				//Must be DateEntry here. PayDate will not work with recurring charges
				+"WHERE DateEntry BETWEEN "+POut.Date(date1.SelectionStart)+" AND "+POut.Date(date2.SelectionStart)+" "
				+"ORDER BY Count ASC";
			using FormQuery formQuery2=new FormQuery(reportSimpleGrid);
			formQuery2.IsReport=true;
			formQuery2.SubmitReportQuery();
			reportSimpleGrid.Title="Payments From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString();
			reportSimpleGrid.SubTitle.Add(PrefC.GetString(PrefName.PracticeTitle));
			reportSimpleGrid.SetColumn(this,0,"Count",50);
			reportSimpleGrid.SetColumn(this,1,"Pat",50);//This name is used to ensure FormQuery does not replace the patnum with the patient name.
			reportSimpleGrid.SetColumn(this,2,"LName",100);
			reportSimpleGrid.SetColumn(this,3,"FName",100);
			reportSimpleGrid.SetColumn(this,4,"DateEntry",100);
			reportSimpleGrid.SetColumn(this,5,"PayDate",100);
			reportSimpleGrid.SetColumn(this,6,"PayNote",150);
			reportSimpleGrid.SetColumn(this,7,"PayAmt",70,HorizontalAlignment.Right);
			Cursor=Cursors.Default;
			formQuery2.ShowDialog();
		}

		private void butMissing_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex reportComplex=new ReportComplex(true,false);
			reportComplex.ReportName="Missing";
			reportComplex.AddTitle("Title","XCharge Transactions From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString(),fontTitle);
			reportComplex.GetTitle("Title").IsUnderlined=true;
			reportComplex.AddSubTitle("SubTitle","No Matching Transaction Found in Open Dental",fontSubTitle);
			QueryObject queryObject;
			DataTable table=XChargeTransactions.GetMissingPaymentsTable(date1.SelectionStart,date2.SelectionStart);
			queryObject=reportComplex.AddQuery(table,"Missing Payments","",SplitByKind.None,1,true);//Valid entries to count have result code 0
			queryObject.AddColumn("Transaction Date/Time",170,FieldValueType.String,font);
			queryObject.AddColumn("Transaction Type",120,FieldValueType.String,font);
			queryObject.AddColumn("Clerk ID",80,FieldValueType.String,font);
			queryObject.AddColumn("Item#",50,FieldValueType.String,font);
			queryObject.AddColumn("Pat",50,FieldValueType.String,font);
			queryObject.AddColumn("Credit Card Num",140,FieldValueType.String,font);
			queryObject.AddColumn("Exp",50,FieldValueType.String,font);
			queryObject.AddColumn("Result",50,FieldValueType.String,font);
			queryObject.AddColumn("Amount",60,FieldValueType.Number,font);
			queryObject.GetColumnHeader("Amount").ContentAlignment=ContentAlignment.MiddleRight;
			Cursor=Cursors.Default;
			if(!reportComplex.SubmitQueries(isShowMessage:true)) {
				return;
			}
			// display report
			using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
			//FormR.MyReport=report;
			formReportComplex.ShowDialog();
		}

		private void butExtra_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex reportComplex=new ReportComplex(true,false);
			reportComplex.ReportName="Extra Payments";
			reportComplex.AddTitle("Title","Payments From "+date1.SelectionStart.ToShortDateString()+" To "+date2.SelectionStart.ToShortDateString(),fontTitle);
			reportComplex.GetTitle("Title").IsUnderlined=true;
			reportComplex.AddSubTitle("SubTitle","No Matching X-Charge Transactions for these Payments",fontSubTitle);
			QueryObject queryObject;
			DataTable table=XChargeTransactions.GetMissingXTransTable(date1.SelectionStart,date2.SelectionStart);
			queryObject=reportComplex.AddQuery(table,"Extra Payments","",SplitByKind.None,1,true);
			queryObject.AddColumn("Pat",50,FieldValueType.String,font);
			queryObject.AddColumn("LName",100,FieldValueType.String,font);
			queryObject.AddColumn("FName",100,FieldValueType.String,font);
			queryObject.AddColumn("DateEntry",90,FieldValueType.Date,font);
			queryObject.AddColumn("PayDate",90,FieldValueType.Date,font);
			queryObject.AddColumn("PayNote",210,FieldValueType.String,font);
			queryObject.AddColumn("PayAmt",70,FieldValueType.Number,font);
			queryObject.GetColumnHeader("PayAmt").ContentAlignment=ContentAlignment.MiddleRight;
			queryObject.GetColumnDetail("PayAmt").ContentAlignment=ContentAlignment.MiddleRight;
			Cursor=Cursors.Default;
			if(!reportComplex.SubmitQueries(isShowMessage:true)) {
				return;
			}
			// display report
			using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
			//FormR.MyReport=report;
			formReportComplex.ShowDialog();
		}

		private void butValidate_Click(object sender,EventArgs e) {
			validateBatch();
		}

		private void validateBatch() {
			string message="";
			DataTable table = XChargeTransactions.GetXChargeTransactionValidateBatchData(date1.SelectionStart, date2.SelectionStart);
			for(int i = 0;i<table.Rows.Count;++i) {
				if(table.Rows[i]["Difference"].ToString() != "0") {
					message+=$"Batch:{table.Rows[i]["BatchNum"]} | XC:{table.Rows[i]["Total"]} | Summed:{table.Rows[i]["Summed"]} | Diff:{table.Rows[i]["Difference"]}\n";
				}
			}
			if(message.Length==0) {
				message="All Batch totals match.";
			}
			MsgBox.Show(this,message);
		}

	}
}