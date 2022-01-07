using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	public class EDS_Statements {
		
		///<summary>Generates all the xml up to the point where the first statement would go.</summary>
		public static void GeneratePracticeInfo(XmlWriter writer,long clinicNum) {
			Clinic clinic=Clinics.GetClinic(clinicNum);
			Ebill eBillClinic=Ebills.GetForClinic(clinicNum);
			if(eBillClinic==null) {//Clinic specific Ebill doesn't exist, use the defaults.
				eBillClinic=Ebills.GetForClinic(0);
			}
			writer.WriteProcessingInstruction("xml","version = \"1.0\" standalone=\"yes\"");
			writer.WriteStartElement("StatementFile");
			//sender address----------------------------------------------------------
			writer.WriteStartElement("SenderAddress");
			if(clinic==null) {
				writer.WriteElementString("Name",PrefC.GetString(PrefName.PracticeTitle));
			}
			else {
				writer.WriteElementString("Name",clinic.Description);
			}
			WriteAddress(writer,eBillClinic.PracticeAddress,clinic);
			writer.WriteEndElement();//SenderAddress
			writer.WriteStartElement("Statements");
		}

		private static void WriteAddress(XmlWriter writer,EbillAddress eBillAddress,Clinic clinic) {
			//If using practice information or using the default (no clinic) Ebill and a clinic enum is specified, use the practice level information.
			if(eBillAddress==EbillAddress.PracticePhysical || (clinic==null && eBillAddress==EbillAddress.ClinicPhysical)) {
				writer.WriteElementString("Address1",PrefC.GetString(PrefName.PracticeAddress));
				writer.WriteElementString("Address2",PrefC.GetString(PrefName.PracticeAddress2));
				writer.WriteElementString("City",PrefC.GetString(PrefName.PracticeCity));
				writer.WriteElementString("State",PrefC.GetString(PrefName.PracticeST));
				writer.WriteElementString("Zip",PrefC.GetString(PrefName.PracticeZip));
				writer.WriteElementString("Phone",PrefC.GetString(PrefName.PracticePhone));//enforced to be 10 digit fairly rigidly by the UI
			}
			else if(eBillAddress==EbillAddress.PracticePayTo || (clinic==null && eBillAddress==EbillAddress.ClinicPayTo)) {
				writer.WriteElementString("Address1",PrefC.GetString(PrefName.PracticePayToAddress));
				writer.WriteElementString("Address2",PrefC.GetString(PrefName.PracticePayToAddress2));
				writer.WriteElementString("City",PrefC.GetString(PrefName.PracticePayToCity));
				writer.WriteElementString("State",PrefC.GetString(PrefName.PracticePayToST));
				writer.WriteElementString("Zip",PrefC.GetString(PrefName.PracticePayToZip));
				writer.WriteElementString("Phone",PrefC.GetString(PrefName.PracticePayToPhone));//enforced to be 10 digit fairly rigidly by the UI
			}
			else if(eBillAddress==EbillAddress.PracticeBilling || (clinic==null && eBillAddress==EbillAddress.ClinicBilling)) {
				writer.WriteElementString("Address1",PrefC.GetString(PrefName.PracticeBillingAddress));
				writer.WriteElementString("Address2",PrefC.GetString(PrefName.PracticeBillingAddress2));
				writer.WriteElementString("City",PrefC.GetString(PrefName.PracticeBillingCity));
				writer.WriteElementString("State",PrefC.GetString(PrefName.PracticeBillingST));
				writer.WriteElementString("Zip",PrefC.GetString(PrefName.PracticeBillingZip));
				writer.WriteElementString("Phone",PrefC.GetString(PrefName.PracticeBillingPhone));//enforced to be 10 digit fairly rigidly by the UI
			}
			else if(eBillAddress==EbillAddress.ClinicPhysical) {
				writer.WriteElementString("Address1",clinic.Address);
				writer.WriteElementString("Address2",clinic.Address2);
				writer.WriteElementString("City",clinic.City);
				writer.WriteElementString("State",clinic.State);
				writer.WriteElementString("Zip",clinic.Zip);
				writer.WriteElementString("Phone",clinic.Phone);//enforced to be 10 digit fairly rigidly by the UI
			}
			else if(eBillAddress==EbillAddress.ClinicPayTo) {
				writer.WriteElementString("Address1",clinic.PayToAddress);
				writer.WriteElementString("Address2",clinic.PayToAddress2);
				writer.WriteElementString("City",clinic.PayToCity);
				writer.WriteElementString("State",clinic.PayToState);
				writer.WriteElementString("Zip",clinic.PayToZip);
				writer.WriteElementString("Phone",clinic.Phone);//enforced to be 10 digit fairly rigidly by the UI
			}
			else if(eBillAddress==EbillAddress.ClinicBilling) {
				writer.WriteElementString("Address1",clinic.BillingAddress);
				writer.WriteElementString("Address2",clinic.BillingAddress2);
				writer.WriteElementString("City",clinic.BillingCity);
				writer.WriteElementString("State",clinic.BillingState);
				writer.WriteElementString("Zip",clinic.BillingZip);
				writer.WriteElementString("Phone",clinic.Phone);//enforced to be 10 digit fairly rigidly by the UI
			}
		}

		///<summary>Adds the xml for one statement.</summary>
		public static void GenerateOneStatement(XmlWriter writer,Statement stmt,Patient pat,Family fam,DataSet dataSet){
			writer.WriteStartElement("Statement");
			writer.WriteStartElement("RecipientAddress");
			Patient guar=fam.ListPats[0];
			writer.WriteElementString("Name",guar.GetNameFLFormal());
			if(PrefC.GetBool(PrefName.StatementAccountsUseChartNumber)) {
				writer.WriteElementString("Account",guar.ChartNumber);
			} else {
				writer.WriteElementString("Account",POut.Long(guar.PatNum));
			}
			writer.WriteElementString("Address1",guar.Address);
			writer.WriteElementString("Address2",guar.Address2);
			writer.WriteElementString("City",guar.City);
			writer.WriteElementString("State",guar.State);
			writer.WriteElementString("Zip",guar.Zip);
			string email="";
			Def billingDef=Defs.GetDef(DefCat.BillingTypes,guar.BillingType);
			if(billingDef.ItemValue=="E") {
				email=guar.Email;
			}
			writer.WriteElementString("EMail",email);
			writer.WriteEndElement();//RecipientAddress
			//Account summary-----------------------------------------------------------------------
			if(stmt.DateRangeFrom.Year<1880) {//make up a statement date.
				writer.WriteElementString("PriorStatementDate",DateTime.Today.AddMonths(-1).ToString("MM/dd/yyyy"));
			}
			else {
				writer.WriteElementString("PriorStatementDate",stmt.DateRangeFrom.AddDays(-1).ToString("MM/dd/yyyy"));
			}
			DateTime dueDate;
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)==-1){
				dueDate=DateTime.Today.AddDays(10);
			}
			else{
				dueDate=DateTime.Today.AddDays(PrefC.GetLong(PrefName.StatementsCalcDueDate));
			}
			writer.WriteElementString("DueDate",dueDate.ToString("MM/dd/yyyy"));
			writer.WriteElementString("StatementDate",stmt.DateSent.ToString("MM/dd/yyyy"));
			double balanceForward=0;
			for(int r=0;r<dataSet.Tables["misc"].Rows.Count;r++){
				if(dataSet.Tables["misc"].Rows[r]["descript"].ToString()=="balanceForward"){
					balanceForward=PIn.Double(dataSet.Tables["misc"].Rows[r]["value"].ToString());
				}
			}
			writer.WriteElementString("PriorBalance",balanceForward.ToString("F2"));
			DataTable tableAccount=null;
			for(int i=0;i<dataSet.Tables.Count;i++) {
				if(dataSet.Tables[i].TableName.StartsWith("account")) {
					tableAccount=dataSet.Tables[i];
				}
			}
			double credits=0;
			for(int i=0;i<tableAccount.Rows.Count;i++) {
				credits+=PIn.Double(tableAccount.Rows[i]["creditsDouble"].ToString());
			}
			writer.WriteElementString("Credits",credits.ToString("F2"));
			decimal payPlanDue=0;
			double amountDue=guar.BalTotal;
			for(int m=0;m<dataSet.Tables["misc"].Rows.Count;m++) {
				if(dataSet.Tables["misc"].Rows[m]["descript"].ToString()=="payPlanDue") {
					payPlanDue+=PIn.Decimal(dataSet.Tables["misc"].Rows[m]["value"].ToString());//This will be an option once more users are using it.
				}
			}
			writer.WriteElementString("PayPlanDue",payPlanDue.ToString("F2"));
			if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
				writer.WriteElementString("EstInsPayments","");//optional.
			}
			else {//this is typical
				writer.WriteElementString("EstInsPayments",guar.InsEst.ToString("F2"));//optional.
				amountDue-=guar.InsEst;
			}
			InstallmentPlan installPlan=InstallmentPlans.GetOneForFam(guar.PatNum);
			if(installPlan!=null){
				//show lesser of normal total balance or the monthly payment amount.
				if(installPlan.MonthlyPayment < amountDue) {
					amountDue=installPlan.MonthlyPayment;
				}
			}
			writer.WriteElementString("AmountDue",amountDue.ToString("F2"));
			writer.WriteElementString("PastDue30",guar.Bal_31_60.ToString("F2"));//optional
			writer.WriteElementString("PastDue60",guar.Bal_61_90.ToString("F2"));//optional
			writer.WriteElementString("PastDue90",guar.BalOver90.ToString("F2"));//optional
			//Notes-----------------------------------------------------------------------------------
			writer.WriteStartElement("Notes");
			if(stmt.NoteBold!="") {
				writer.WriteStartElement("Note");
				writer.WriteAttributeString("FgColor","Red");
				writer.WriteCData(stmt.NoteBold);
				writer.WriteEndElement();//Note
			}
			if(stmt.Note!="") {
				writer.WriteStartElement("Note");
				writer.WriteCData(stmt.Note);
				writer.WriteEndElement();//Note
			}
			writer.WriteEndElement();//Notes
			//Detail items------------------------------------------------------------------------------
			writer.WriteStartElement("DetailItems");
			//string note;
			string descript;
			string fulldesc;
			string procCode;
			string tth;
			//string linedesc;
			string[] lineArray;
			List<string> lines;
			DateTime date;
			int seq=0;
			for(int i=0;i<tableAccount.Rows.Count;i++) {
				procCode=tableAccount.Rows[i]["ProcCode"].ToString();
				tth=tableAccount.Rows[i]["tth"].ToString();
				descript=tableAccount.Rows[i]["description"].ToString();
				fulldesc=procCode+" "+tth+" "+descript;
				lineArray=fulldesc.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
				lines=new List<string>(lineArray);
				//The specs say that the line limit is 30 char.  But in testing, it will take 50 char.
				//We will use 40 char to be safe.
				if(lines[0].Length>40) {
					string newline=lines[0].Substring(40);
					lines[0]=lines[0].Substring(0,40);//first half
					lines.Insert(1,newline);//second half
				}
				for(int li=0;li<lines.Count;li++) {
					writer.WriteStartElement("DetailItem");//has a child item. We won't add optional child note
					writer.WriteAttributeString("sequence",seq.ToString());
					writer.WriteStartElement("Item");
					if(li==0) {
						date=(DateTime)tableAccount.Rows[i]["DateTime"];
						writer.WriteElementString("Date",date.ToString("MM/dd/yyyy"));
						writer.WriteElementString("PatientName",tableAccount.Rows[i]["patient"].ToString());
					}
					else {
						writer.WriteElementString("Date","");
						writer.WriteElementString("PatientName","");
					}
					writer.WriteElementString("Description",lines[li]);
					if(li==0) {
						writer.WriteElementString("Charges",tableAccount.Rows[i]["charges"].ToString());
						writer.WriteElementString("Credits",tableAccount.Rows[i]["credits"].ToString());
						writer.WriteElementString("Balance",tableAccount.Rows[i]["balance"].ToString());
					}
					else {
						writer.WriteElementString("Charges","");
						writer.WriteElementString("Credits","");
						writer.WriteElementString("Balance","");
					}
					writer.WriteEndElement();//Item
					writer.WriteEndElement();//DetailItem
					seq++;
				}
			}
			writer.WriteEndElement();//DetailItems
			writer.WriteEndElement();//Statement
		}

		///<summary>After statements are added, this adds the necessary closing xml elements.</summary>
		public static void GenerateWrapUp(XmlWriter writer) {
			writer.WriteEndElement();//Statements
			writer.WriteEndElement();//StatementFile
		}

		//<summary>Surround with try catch.  The "data" is the previously constructed xml.</summary>
		//public static void Send(string data) {
			//do this in the UI
		//}

	


	}
}
