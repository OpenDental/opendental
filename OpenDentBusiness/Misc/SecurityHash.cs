/*Any changes to this file should be also done in the SecurityHashingTool solution, including changing DateStart.*/
using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDentBusiness.Misc {
	public class SecurityHash {
		///<summary>The date Open Dental started hashing fields into paysplit.SecurityHash. Used to determine if hashing is required. </summary>
		public static DateTime DateStart=new DateTime(2022,11,2);
		///<summary>Only set to false for standalone hashing tool. </summary>
		public static bool IsThreaded=true;
		private static bool _arePaySplitsUpdated=false;
		private static bool _areAppointmentsUpdated=false;
		private static bool _arePatientsUpdated=false;
		private static bool _arePayPlansUpdated=false;

		///<summary>This method is allowed anywhere in the ConvertDatabase scripts.  It's specially written to never crash. It does not affect the schema. It can be called multiple times during a conversion and will only run once.  Updates the SecurityHash fields of all tables for which Open Dental is enforcing database integrity. First clears out all existing SecurityHashes, then creates new ones for recent entries. </summary>
		public static void UpdateHashing() {
			RunPaysplit();
			RunAppointment();
			RunPatient();
			RunPayPlan();
			string updating="Updating database.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
		}

		///<summary>Only used one time during a conversion script. Sets the _arePatientsUpdate boolean to false. Allows the hashing logic to run on the patient table a subsequent time during a convert script update. </summary>
		public static void ResetPatientHashing() {
			_arePatientsUpdated=false;
		}

		private static void RunPaysplit() {
			if(_arePaySplitsUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"paysplit","SecurityHash")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"paysplit","PatNum")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"paysplit","SplitAmt")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"paysplit","DatePay"))
			{
				return;
			}
			string updating="Hashing paysplits.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			//Threading because it can take too long and it doesn't matter if user starts working while it's still in progress.
			//They might see integrity triangles until this completes.
			//If they shut down before it completes, then those won't be fixed until the next time they run the hashing (their next update).
			//This is only a problem for a recent db conversion, where there are thousands of splits with the same date.  Otherwise, it's really fast.
			_arePaySplitsUpdated=true;
			if(IsThreaded){
				ThreadStart threadStart=new ThreadStart(PaysplitWorker);
				Thread thread=new Thread(threadStart);
				thread.IsBackground=true;
				thread.Start();
			}
			else{
				PaysplitWorker();
			}
		}

		private static void PaysplitWorker() {
			//Hash entries made after new date
			string command="SELECT SplitNum, PatNum, SplitAmt, DatePay, SecurityHash FROM paysplit WHERE DatePay >= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long splitNum;
			string unhashedText="";
			string hashedTextOld="";
			string hashedTextNew="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				hashedTextOld=PIn.String(table.Rows[i]["SecurityHash"].ToString());
				splitNum=PIn.Long(table.Rows[i]["SplitNum"].ToString());
				unhashedText+=PIn.Long(table.Rows[i]["PatNum"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["SplitAmt"].ToString()).ToString("F2");
				unhashedText+=PIn.Date(table.Rows[i]["DatePay"].ToString()).ToString("yyyy-MM-dd");
				try {
					hashedTextNew=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedTextNew="";
				}
				//Only update hashes that changed
				if(hashedTextOld!=hashedTextNew) {
					command=$@"UPDATE paysplit SET SecurityHash='{POut.String(hashedTextNew)}' WHERE SplitNum={POut.Long(splitNum)}";
					Db.NonQ(command);
				}
			}
		}

		private static void RunAppointment() {
			if(_areAppointmentsUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"appointment","SecurityHash")
			  || !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"appointment","AptStatus")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"appointment","Confirmed")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"appointment","AptDateTime"))
			{
				return;
			}
			string updating="Hashing appointments.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			//Hash entries made after new date
			string command="SELECT AptNum, AptStatus, Confirmed, AptDateTime, SecurityHash FROM appointment WHERE AptDateTime>= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long aptNum;
			string unhashedText="";
			string hashedTextOld="";
			string hashedTextNew="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				hashedTextOld=PIn.String(table.Rows[i]["SecurityHash"].ToString());
				aptNum=PIn.Long(table.Rows[i]["AptNum"].ToString());
				unhashedText+=PIn.Int(table.Rows[i]["AptStatus"].ToString()).ToString();
				unhashedText+=PIn.Long(table.Rows[i]["Confirmed"].ToString()).ToString();
				unhashedText+=PIn.DateT(table.Rows[i]["AptDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
				try {
					hashedTextNew=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedTextNew="";
				}
				//Only update hashes that changed
				if(hashedTextOld!=hashedTextNew) {
					command=$@"UPDATE appointment SET SecurityHash='{POut.String(hashedTextNew)}' WHERE AptNum={POut.Long(aptNum)}";
					Db.NonQ(command);
				}
			}
			_areAppointmentsUpdated=true;
		}

		///<summary>Hashes ALL patients with an empty SecurityHash field. Does not use the DateStart like other table hashing methods. </summary>
		private static void RunPatient() {
			if(_arePatientsUpdated) {
				return;
			}
			//Check if SecurityHash column is present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","SecurityHash")) {
				return;
			}
			string updating="Hashing patients.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			_arePatientsUpdated=true;
			//Threading because it can take too long and it doesn't matter if user starts working while it's still in progress.
			//They might see integrity triangles until this completes.
			//If they shut down before it completes, then those won't be fixed until the next time they run the hashing (their next update).
			if(IsThreaded){
				ThreadStart threadStart=new ThreadStart(PatientWorker);
				Thread thread=new Thread(threadStart);
				thread.IsBackground=true;
				thread.Start();
			}
			else{
				PatientWorker();
			}
		}

		private static void PatientWorker() {
			//Get all ALL unhashed patients
			string command="SELECT PatNum FROM patient WHERE SecurityHash=''";
			DataTable table=Db.GetTable(command);
			//Do not clear current hashes
			long patNum;
			string unhashedText="";
			string hashedText="";
			for(int i=0;i<table.Rows.Count;i++) {
				patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				unhashedText=patNum.ToString();
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText="";
				}
				//This loop takes 7 minutes for 1M patients when run locally, but it's taking too long for customers.
				command=$@"UPDATE patient SET SecurityHash='{POut.String(hashedText)}' WHERE PatNum={POut.Long(patNum)}";
				Db.NonQ(command);
			}
		}

		private static void RunPayPlan() {
			if(_arePayPlansUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"payplan","SecurityHash")
			  || !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"payplan","Guarantor")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"payplan","PayAmt")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"payplan","IsClosed")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"payplan","IsLocked"))
			{
				return;
			}
			string updating="Hashing payplans.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			//Clear old hashes
			string command="UPDATE payplan SET SecurityHash=''";
			Db.NonQ(command);
			//Hash entries made after new date
			command="SELECT * FROM payplan WHERE PayPlanDate>= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long payPlanNum;
			string unhashedText="";
			string hashedText="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				payPlanNum=PIn.Long(table.Rows[i]["PayPlanNum"].ToString());
				unhashedText+=PIn.Long(table.Rows[i]["Guarantor"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["PayAmt"].ToString()).ToString("F2");
				unhashedText+=PIn.Bool(table.Rows[i]["IsClosed"].ToString());
				unhashedText+=PIn.Bool(table.Rows[i]["IsLocked"].ToString());
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText="";
				}
				command=$@"UPDATE payplan SET SecurityHash='{POut.String(hashedText)}' WHERE PayPlanNum={POut.Long(payPlanNum)}";
				Db.NonQ(command);
			}
			_arePayPlansUpdated=true;
		}

	}
}
