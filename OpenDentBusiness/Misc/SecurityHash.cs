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
		public static DateTime DateStart=new DateTime(2024,5,7);
		///<summary>Only set to false for standalone hashing tool. </summary>
		public static bool IsThreaded=true;
		private static bool _arePaySplitsUpdated=false;
		private static bool _areAppointmentsUpdated=false;
		private static bool _arePatientsUpdated=false;
		private static bool _arePayPlansUpdated=false;
		private static bool _areClaimsUpdated=false;
		private static bool _areClaimProcsUpdated=false;

		///<summary>This method is NOT safe to invoke during database conversions. It is only to be called AFTER all conversions have taken place in order to avoid a rare table lockup. 
		///Runs a hashing algorithm over all tables for which Open Dental is enforcing database integrity. Overwrites any existing SecurityHashes with new ones for recent entries. </summary>
		public static void UpdateHashing() {
			RunPaysplit();
			RunAppointment();
			RunPatient();
			RunPayPlan();
			RunClaim();
			RunClaimProc();
			string updating="Updating database.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
		}

		///<summary>Only used one time during a conversion script. Sets the _arePatientsUpdate boolean to false. Allows the hashing logic to run on the patient table a subsequent time during a convert script update. </summary>
		public static void ResetPatientHashing() {
			_arePatientsUpdated=false;
		}

		///<summary>Returns either SecurityHash.DateStart or the date from a month prior to today, whichever is more recent.</summary>
		public static DateTime GetHashingDate() {
			DateTime dateLastMonth=DateTime.Now.Date.AddMonths(-1);
			DateTime dateStartHashing=DateStart;
			if(dateLastMonth>DateStart) {
				dateStartHashing=dateLastMonth;
			}
			return dateStartHashing;
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
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			string command="SELECT SplitNum, PatNum, SplitAmt, DatePay, SecurityHash FROM paysplit WHERE DatePay >= "+POut.Date(dateStartHashing);
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
					hashedTextNew=ex.GetType().Name;
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
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			string command="SELECT AptNum, AptStatus, Confirmed, AptDateTime, SecurityHash FROM appointment WHERE AptDateTime>= "+POut.Date(dateStartHashing);
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
					hashedTextNew=ex.GetType().Name;
				}
				//Only update hashes that changed
				if(hashedTextOld!=hashedTextNew) {
					command=$@"UPDATE appointment SET SecurityHash='{POut.String(hashedTextNew)}' WHERE AptNum={POut.Long(aptNum)}";
					Db.NonQ(command);
				}
			}
			_areAppointmentsUpdated=true;
		}

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
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			string command="SELECT PatNum FROM patient WHERE DateTStamp >= "+POut.Date(dateStartHashing);
			DataTable table=Db.GetTable(command);
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
					hashedText=ex.GetType().Name;
				}
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
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			command="SELECT * FROM payplan WHERE PayPlanDate>= "+POut.Date(dateStartHashing);
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
					hashedText=ex.GetType().Name;
				}
				command=$@"UPDATE payplan SET SecurityHash='{POut.String(hashedText)}' WHERE PayPlanNum={POut.Long(payPlanNum)}";
				Db.NonQ(command);
			}
			_arePayPlansUpdated=true;
		}

		private static void RunClaim() {
			if(_areClaimsUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claim","SecurityHash")
			  || !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claim","ClaimFee")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claim","ClaimStatus")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claim","InsPayEst")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claim","InsPayAmt"))
			{
				return;
			}
			string updating="Hashing claims.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			string command="SELECT ClaimNum, ClaimFee, ClaimStatus, InsPayEst, InsPayAmt, SecurityHash FROM claim WHERE DateService >= "+POut.Date(dateStartHashing);
			DataTable table=Db.GetTable(command);
			long claimNum;
			string unhashedText="";
			string hashedText="";
			string hashedTextOld="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				claimNum=PIn.Long(table.Rows[i]["ClaimNum"].ToString());
				hashedTextOld=PIn.String(table.Rows[i]["SecurityHash"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["ClaimFee"].ToString()).ToString("F2");
				unhashedText+=PIn.String(table.Rows[i]["ClaimStatus"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["InsPayEst"].ToString()).ToString("F2");
				unhashedText+=PIn.Double(table.Rows[i]["InsPayAmt"].ToString()).ToString("F2");
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText=ex.GetType().Name;
				}
				if(hashedTextOld!=hashedText) { //Only update SecurityHash if it's different.
					command=$@"UPDATE claim SET SecurityHash='{POut.String(hashedText)}' WHERE ClaimNum={POut.Long(claimNum)}";
					Db.NonQ(command);
				}
			}
			_areClaimsUpdated=true;
		}

		private static void RunClaimProc() {
			if(_areClaimProcsUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claimproc","SecurityHash")
			  || !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claimproc","ClaimNum")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claimproc","Status")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claimproc","InsPayEst")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"claimproc","InsPayAmt"))
			{
				return;
			}
			string updating="Hashing claimprocs.";
			ODEvent.Fire(ODEventType.ConvertDatabases,updating);
			//Threading because it can take too long and it doesn't matter if user starts working while it's still in progress.
			//They might see integrity triangles until this completes.
			//If they shut down before it completes, then those won't be fixed until the next time they run the hashing (their next update).
			_areClaimProcsUpdated=true;
			if(IsThreaded){
				ThreadStart threadStart=new ThreadStart(ClaimProcWorker);
				Thread thread=new Thread(threadStart);
				thread.IsBackground=true;
				thread.Start();
			}
			else{
				ClaimProcWorker();
			}
		}

		private static void ClaimProcWorker() {
			DateTime dateStartHashing=GetHashingDate();
			//Hash entries made after new date
			string command="SELECT ClaimProcNum, ClaimNum, Status, InsPayEst, InsPayAmt, SecurityHash FROM claimproc WHERE SecDateEntry >= "+POut.Date(dateStartHashing);
			DataTable table=Db.GetTable(command);
			long claimProcNum;
			string unhashedText="";
			string hashedText="";
			string hashedTextOld="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				claimProcNum=PIn.Long(table.Rows[i]["ClaimProcNum"].ToString());
				hashedTextOld=PIn.String(table.Rows[i]["SecurityHash"].ToString());
				unhashedText+=PIn.Long(table.Rows[i]["ClaimNum"].ToString());
				unhashedText+=PIn.Int(table.Rows[i]["Status"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["InsPayEst"].ToString()).ToString("F2");
				unhashedText+=PIn.Double(table.Rows[i]["InsPayAmt"].ToString()).ToString("F2");
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText=ex.GetType().Name;
				}
				if(hashedTextOld!=hashedText) { //Only update SecurityHash if it's different.
					command=$@"UPDATE claimproc SET SecurityHash='{POut.String(hashedText)}' WHERE ClaimProcNum={POut.Long(claimProcNum)}";
					Db.NonQ(command);
				}
			}
		}

	}
}
