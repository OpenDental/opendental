using System;
using System.Reflection;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrPatients{
		///<summary>Only call when EHR is enabled.  Creates the ehrpatient record for the patient if a record does not already exist.  Always returns a non-null EhrPatient.</summary>
		public static EhrPatient Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrPatient>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM ehrpatient WHERE patnum='"+POut.Long(patNum)+"'";
			if(Db.GetCount(command)=="0") {//A record does not exist for this patient yet.
				Insert(patNum);//Create a new record.
			}
			command ="SELECT * FROM ehrpatient WHERE patnum ='"+POut.Long(patNum)+"'";
			return Crud.EhrPatientCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(EhrPatient ehrPatient) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrPatient);
				return;
			}
			Crud.EhrPatientCrud.Update(ehrPatient);
		}

		///<summary>Private method only called from Refresh.</summary>
		private static void Insert(long patNum) {
			//No need to check RemotingRole; private static.
			//Random keys not necessary to check because of 1:1 patNum.
			//However, this is a lazy insert, so multiple locations might attempt it.
			//Just in case, we will have it fail silently.
			//try {
			//AD~ Attempted bug fix.  This command was throwing a "Error Code: 1364 Field 'MotherMaidenFname' doesn't have a default value" UE.  We 
			//believe this is caused by a third party changing the sql_mode variable after our startup call to MiscData.SetSqlMode() which clears this 
			//variable if set to a more strict mode.  A more strict sql_mode would cause warnings to be returned as errors.  Our normal pattern is to use 
			//the Crud Insert method, but due to replication delays (see comment below), we use a custom query to update on duplicate keys instead.  
			//Previously, we only specified the PatNum and VacShareOK in the query.  Now, we explicitly set blank values on all other fields.  For now, we
			//will continue to leave the try/catch commented out, so that we can continue to monitor this code for other bugs we haven't observed yet.
			string command="INSERT INTO ehrpatient (PatNum,MotherMaidenFname,MotherMaidenLname,VacShareOk,MedicaidState,SexualOrientation,GenderIdentity,"
				+"SexualOrientationNote,GenderIdentityNote) "
				+"VALUES("+POut.Long(patNum)+",'','',0,'','','','','')";//VacShareOk cannot be NULL for Oracle.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//We may need to do this in Oracle in the future as well.
					//If using Replication, then we need to watch for duplicate errors, because the insert is lazy.
					//Replication servers can insert a patient note with a primary key belonging to another replication server's key range.
					command+=" ON DUPLICATE KEY UPDATE PatNum='"+patNum+"'";
				}
				Db.NonQ(command);
			//}
			//catch (Exception ex){
			//	//Fail Silently.
			//}
		}

		///<summary>Gets one EhrPatient from the db.  Returns null if there is no entry.</summary>
		public static EhrPatient GetOne(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrPatient>(MethodBase.GetCurrentMethod(),patNum);
			}
			return Crud.EhrPatientCrud.SelectOne(patNum);
		}
	
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrPatient> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrPatient>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrpatient WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrPatientCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command= "DELETE FROM ehrpatient WHERE PatNum = "+POut.Long(patNum);
			Db.NonQ(command);
		}
		*/
	}
}