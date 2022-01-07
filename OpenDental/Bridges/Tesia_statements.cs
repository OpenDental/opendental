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
	///<summary>No longer used.</summary>
	public class Tesia_statements {

		///<summary>Generates all the xml up to the point where the first statement would go.</summary>
		public static void GeneratePracticeInfo(XmlWriter writer) {
			writer.WriteProcessingInstruction("xml","version = \"1.0\" standalone=\"yes\"");
			writer.WriteStartElement("Statements");			
		}

		///<summary>Adds the xml for one statement.</summary>
		public static void GenerateOneStatement(XmlWriter writer,Statement stmt,Patient pat,Family fam,DataSet dataSet){
			Patient guar=fam.ListPats[0];
			writer.WriteStartElement("Statement");
			//writer.WriteAttributeString("CreditCardChoice",PrefC.GetString(PrefName.BillingElectCreditCardChoices"));
			//remit address----------------------------------------------------------
			writer.WriteStartElement("RemitAddress");
			if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
				&& Clinics.GetClinic(guar.ClinicNum)!=null)//and this guar is assigned to a clinic
			{
				Clinic clinic=Clinics.GetClinic(guar.ClinicNum);
				writer.WriteElementString("Name",clinic.Description);
				writer.WriteElementString("Address",clinic.Address);
				writer.WriteElementString("Address2",clinic.Address2);
				writer.WriteElementString("City",clinic.City);
				writer.WriteElementString("State",clinic.State);
				writer.WriteElementString("Zip",clinic.Zip);
				string phone="";
				if(clinic.Phone.Length==10) {
					phone="("+clinic.Phone.Substring(0,3)+")"+clinic.Phone.Substring(3,3)+"-"+clinic.Phone.Substring(6);
				}
				writer.WriteElementString("Phone",phone);
			}
			else{//not using clinics
				writer.WriteElementString("Name",PrefC.GetString(PrefName.PracticeTitle));
				writer.WriteElementString("Address",PrefC.GetString(PrefName.PracticeAddress));
				writer.WriteElementString("Address2",PrefC.GetString(PrefName.PracticeAddress2));
				writer.WriteElementString("City",PrefC.GetString(PrefName.PracticeCity));
				writer.WriteElementString("State",PrefC.GetString(PrefName.PracticeST));
				writer.WriteElementString("Zip",PrefC.GetString(PrefName.PracticeZip));
				writer.WriteElementString("Phone",PrefC.GetString(PrefName.PracticePhone));
			}
			writer.WriteEndElement();//RemitAddress
			//Patient-------------------------------------------------------------------------------
			writer.WriteStartElement("Patient");
			writer.WriteElementString("Name",guar.GetNameFLFormal());
			writer.WriteElementString("AccountNum",guar.PatNum.ToString());
			writer.WriteElementString("Address",guar.Address);
			writer.WriteElementString("Address2",guar.Address2);
			writer.WriteElementString("City",guar.City);
			writer.WriteElementString("State",guar.State);
			writer.WriteElementString("Zip",guar.Zip);
			writer.WriteEndElement();//Patient
			//Account summary-----------------------------------------------------------------------
			writer.WriteStartElement("AccountSummary");
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)==-1){
				writer.WriteElementString("DueDate",Lan.g("FormRpStatement","Upon Receipt"));
			}
			else{
				DateTime dueDate=DateTime.Today.AddDays(PrefC.GetLong(PrefName.StatementsCalcDueDate));
				writer.WriteElementString("DueDate",dueDate.ToString("MM/dd/yyyy"));
			}
			writer.WriteElementString("StatementDate",stmt.DateSent.ToString("MM/dd/yyyy"));
			DataTable tableAccount=null;
			for(int i=0;i<dataSet.Tables.Count;i++) {
				if(dataSet.Tables[i].TableName.StartsWith("account")) {
					tableAccount=dataSet.Tables[i];
				}
			}
			//on a regular printed statement, the amount due at the top might be different from the balance at the middle right.
			//This is because of payment plan balances.
			//But in e-bills, there is only one amount due.
			//Insurance estimate is already subtracted, and payment plan balance is already added.
			double amountDue=guar.BalTotal;
			//add payplan due amt:
			for(int m=0;m<dataSet.Tables["misc"].Rows.Count;m++) {
				if(dataSet.Tables["misc"].Rows[m]["descript"].ToString()=="payPlanDue") {
					amountDue+=PIn.Double(dataSet.Tables["misc"].Rows[m]["value"].ToString());
				}
			}
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
			writer.WriteElementString("Bal_0_30",guar.Bal_0_30.ToString("F2"));
			writer.WriteElementString("Bal_31_60",guar.Bal_31_60.ToString("F2"));
			writer.WriteElementString("Bal_61_90",guar.Bal_61_90.ToString("F2"));
			writer.WriteElementString("BalOver90",guar.BalOver90.ToString("F2"));
			writer.WriteEndElement();//AccountSummary
			//Notes-----------------------------------------------------------------------------------
			writer.WriteStartElement("Notes");
			if(stmt.NoteBold!="") {
				writer.WriteStartElement("Note");
				writer.WriteAttributeString("FgColor",ColorToHexString(Color.DarkRed));
				writer.WriteCData(stmt.NoteBold);
				writer.WriteEndElement();//Note
			}
			if(stmt.Note!="") {
				writer.WriteStartElement("Note");
				writer.WriteAttributeString("FgColor",ColorToHexString(Color.Black));
				writer.WriteCData(stmt.Note);
				writer.WriteEndElement();//Note
			}
			writer.WriteEndElement();//Notes
			//Detail items------------------------------------------------------------------------------
			writer.WriteStartElement("DetailItems");
			string descript;
			string fulldesc;
			string procCode;
			string tth;
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
				//We assume that the line limit is 40 char.
				if(lines[0].Length>40) {
					string newline=lines[0].Substring(40);
					lines[0]=lines[0].Substring(0,40);//first half
					lines.Insert(1,newline);//second half
				}
				for(int li=0;li<lines.Count;li++) {
					writer.WriteStartElement("Item");
					writer.WriteAttributeString("sequence",seq.ToString());
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
					seq++;
				}
				
			}
			writer.WriteEndElement();//DetailItems
			writer.WriteEndElement();//Statement
		}

		///<summary>Converts a .net color to a hex string.  Includes the #.</summary>
		private static string ColorToHexString(Color color) {
			char[] hexDigits={'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
			byte[] bytes = new byte[3];
			bytes[0] = color.R;
			bytes[1] = color.G;
			bytes[2] = color.B;
			char[] chars=new char[bytes.Length * 2];
			for(int i=0;i<bytes.Length;i++){
				int b=bytes[i];
				chars[i*2]=hexDigits[b >> 4];
				chars[i*2+1]=hexDigits[b & 0xF];
			}
			string retVal=new string(chars);
			retVal="#"+retVal;
			return retVal;
		}

		///<summary>After statements are added, this adds the necessary closing xml elements.</summary>
		public static void GenerateWrapUp(XmlWriter writer) {
			writer.WriteEndElement();//Statements
		}

		///<summary>Surround with try catch.  The "data" is the previously constructed xml.</summary>
		public static void Send(string data) {
			//not built yet.
		}

		


	}
}
