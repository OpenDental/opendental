using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Misc {
	public class SecurityHash {
		///<summary>The date Open Dental started hashing fields into paysplit.SecurityHash. Used to determine if hashing is required. </summary>
		public static DateTime DateStart=new DateTime(2021,12,9);
		private static bool _arePaySplitsUpdated=false;
		private static bool _areAppointmentsUpdated=false;
		private static bool _arePatientsUpdated=false;
		private static bool _arePayPlansUpdated=false;

		///<summary>This method is allowed anywhere in the ConvertDatabase scripts.  It's specially written to never crash. It does not affect the schema. It can be called multiple times and will only run once.  Updates the SecurityHash field of the tables for which Open Dental is enforcing database integrity. First clears out all existing SecurityHashes, then creates new ones. </summary>
		public static void UpdateHashing() {
			RunPaysplit();
			RunAppointment();
			RunPatient();
			RunPayPlan();
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
			//Clear old hashes
			string command="UPDATE paysplit SET SecurityHash=''";
			Db.NonQ(command);
			//Hash entries made after new date
			command="SELECT * FROM paysplit WHERE SecDateTEdit >= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long splitNum;
			string unhashedText="";
			string hashedText="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				splitNum=PIn.Long(table.Rows[i]["SplitNum"].ToString());
				unhashedText+=PIn.Long(table.Rows[i]["PatNum"].ToString());
				unhashedText+=PIn.Double(table.Rows[i]["SplitAmt"].ToString()).ToString("F2");
				unhashedText+=PIn.Date(table.Rows[i]["DatePay"].ToString()).ToString("yyyy-MM-dd");
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText="";
				}
				command=$@"UPDATE paysplit SET SecurityHash='{POut.String(hashedText)}' WHERE SplitNum={POut.Long(splitNum)}";
				Db.NonQ(command);
			}
			_arePaySplitsUpdated=true;
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
			//Clear old hashes
			string command="UPDATE appointment SET SecurityHash=''";
			Db.NonQ(command);
			//Hash entries made after new date
			command="SELECT * FROM appointment WHERE SecDateTEntry>= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long aptNum;
			string unhashedText="";
			string hashedText="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				aptNum=PIn.Long(table.Rows[i]["AptNum"].ToString());
				unhashedText+=PIn.Int(table.Rows[i]["AptStatus"].ToString()).ToString();
				unhashedText+=PIn.Long(table.Rows[i]["Confirmed"].ToString()).ToString();
				unhashedText+=PIn.DateT(table.Rows[i]["AptDateTime"].ToString()).ToString("yyyy-MM-dd");
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText="";
				}
				command=$@"UPDATE appointment SET SecurityHash='{POut.String(hashedText)}' WHERE AptNum={POut.Long(aptNum)}";
				Db.NonQ(command);
			}
			_areAppointmentsUpdated=true;
		}

		private static void RunPatient() {
			if(_arePatientsUpdated) {
				return;
			}
			//Check if columns are present
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","SecurityHash")
			  || !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","Guarantor")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","FamFinUrgNote")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","ApptModNote")
				|| !LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"patient","Email"))
			{
				return;
			}
			//Clear old hashes
			string command="UPDATE patient SET SecurityHash=''";
			Db.NonQ(command);
			//Hash entries made after new date
			command="SELECT * FROM patient WHERE SecDateEntry>= "+POut.Date(DateStart);
			DataTable table=Db.GetTable(command);
			long patNum;
			string unhashedText="";
			string hashedText="";
			for(int i=0;i<table.Rows.Count;i++) {
				unhashedText="";
				patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				unhashedText+=PIn.Long(table.Rows[i]["Guarantor"].ToString());
				unhashedText+=PIn.String(table.Rows[i]["FamFinUrgNote"].ToString());
				unhashedText+=PIn.String(table.Rows[i]["ApptModNote"].ToString());
				unhashedText+=PIn.String(table.Rows[i]["Email"].ToString());
				try {
					hashedText=CDT.Class1.CreateSaltedHash(unhashedText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					hashedText="";
				}
				command=$@"UPDATE patient SET SecurityHash='{POut.String(hashedText)}' WHERE PatNum={POut.Long(patNum)}";
				Db.NonQ(command);
			}
			_arePatientsUpdated=true;
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
