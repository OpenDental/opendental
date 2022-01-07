using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RefAttaches{
		#region Get Methods
		///<summary>For one patient</summary>
		public static List<RefAttach> Refresh(long patNum) {
			//No need to check RemotingRole; no call to db.
			return RefreshFiltered(patNum,true,0);
		}

		///<summary>For the ReferralsPatient window.  showAll is only used for the referred procs view.</summary>
		public static List<RefAttach> RefreshFiltered(long patNum,bool showAll,long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RefAttach>>(MethodBase.GetCurrentMethod(),patNum,showAll,procNum);
			}
			//Inner join with referral table on ReferralNum to ignore invalid RefAttaches.  DBM removes these invalid rows anyway.
			string command="SELECT refattach.* FROM refattach "
				+"INNER JOIN referral ON refattach.ReferralNum=referral.ReferralNum "
				+"WHERE refattach.PatNum = "+POut.Long(patNum)+" ";
			if(procNum!=0) {//for procedure
				if(!showAll) {//hide regular referrals
					command+="AND refattach.ProcNum="+POut.Long(procNum)+" ";
				}
			}
			command+="ORDER BY refattach.ItemOrder";
			return Crud.RefAttachCrud.SelectMany(command);
		}
		
		///<summary>For FormReferralProckTrack.</summary>
		public static List<RefAttach> RefreshForReferralProcTrack(DateTime dateFrom,DateTime dateTo,bool complete) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RefAttach>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,complete);
			}
			//Inner join with referral table on ReferralNum to ignore invalid RefAttaches.  DBM removes these invalid rows anyway.
			string command="SELECT refattach.* FROM refattach "
				+"INNER JOIN referral ON refattach.ReferralNum=referral.ReferralNum "
				+"INNER JOIN procedurelog ON refattach.ProcNum=procedurelog.ProcNum "
				+"WHERE refattach.RefDate>="+POut.Date(dateFrom)+" "
				+"AND refattach.RefDate<="+POut.Date(dateTo)+" ";
			if(!complete) {
				command+="AND refattach.DateProcComplete="+POut.Date(DateTime.MinValue)+" ";
			}
			command+="ORDER BY refattach.RefDate";
			return Crud.RefAttachCrud.SelectMany(command);
		}

		///<summary>Returns a list of patient names that are attached to this referral. Used to display in the referral edit window.</summary>
		public static string[] GetPats(long refNum,ReferralType refType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod(),refNum,refType);
			}
			string command="SELECT CONCAT(CONCAT(patient.LName,', '),patient.FName) "
				+"FROM patient,refattach,referral " 
				+"WHERE patient.PatNum=refattach.PatNum "
				+"AND refattach.ReferralNum=referral.ReferralNum "
				+"AND refattach.RefType="+POut.Int((int)refType)+" "
				+"AND referral.ReferralNum="+POut.Long(refNum);
			DataTable table=Db.GetTable(command);
			string[] retStr=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++){
				retStr[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return retStr;
		}

		/// <summary>Gets the referral number for this patient.  If multiple, it returns the first one.  If none, it returns 0.  Does not consider referred To.</summary>
		public static long GetReferralNum(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT ReferralNum "
				+"FROM refattach " 
				+"WHERE refattach.PatNum ="+POut.Long(patNum)+" "
				+"AND refattach.RefType="+POut.Int((int)ReferralType.RefFrom)+" "
				+"ORDER BY ItemOrder ";
			command=DbHelper.LimitOrderBy(command,1);
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Gets all RefAttaches for the patients in the list of PatNums.  Returns an empty list if no matches.</summary>
		public static List<RefAttach> GetRefAttaches(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RefAttach>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums.Count==0) {
				return new List<RefAttach>();
			}
			//MySQL can handle duplicate values within the IN criteria more efficiently than removing them in a loop.
			List<long> uniqueNums=new List<long>();
			string command="SELECT * FROM refattach "
				+"WHERE refattach.PatNum IN ("+String.Join<long>(",",listPatNums)+")";
			return Crud.RefAttachCrud.SelectMany(command);
		}

		///<summary>Gets all the possible RefAttaches, for the patient, that are in the denominator of the summary of care measure.</summary>
		public static List<RefAttach> GetRefAttachesForSummaryOfCareForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RefAttach>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM refattach "
				+"WHERE PatNum = "+POut.Long(patNum)+" "
				+"AND RefType="+POut.Int((int)ReferralType.RefTo)+" "
				+"AND IsTransitionOfCare=1 AND ProvNum!=0 "
				+"ORDER BY ItemOrder";
			return Crud.RefAttachCrud.SelectMany(command);
		}
		#endregion
	
		#region Insert
		///<summary></summary>
		public static long Insert(RefAttach attach) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				attach.RefAttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),attach);
				return attach.RefAttachNum;
			}
			return Crud.RefAttachCrud.Insert(attach);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(RefAttach attach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),attach);
				return;
			}
			Crud.RefAttachCrud.Update(attach);
		}
		
		///<summary></summary>
		public static void Update(RefAttach attach,RefAttach attachOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),attach,attachOld);
				return;
			}
			Crud.RefAttachCrud.Update(attach,attachOld);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(RefAttach attach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),attach);
				return;
			}
			string command="UPDATE refattach SET ItemOrder=ItemOrder-1 WHERE PatNum="+POut.Long(attach.PatNum)
				+" AND ItemOrder > "+POut.Int(attach.ItemOrder);
			Db.NonQ(command);
			command= "DELETE FROM refattach "
				+"WHERE refattachnum = "+POut.Long(attach.RefAttachNum);
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary></summary>
		public static bool IsReferralAttached(long referralNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),referralNum);
			}
			string command="SELECT COUNT(*) FROM refattach WHERE ReferralNum = '"+POut.Long(referralNum)+"'";
			return (Db.GetCount(command)!="0");
		}
		#endregion
	}
}