using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimSnapshots{
		#region Get Methods
		///<summary>Gets one ClaimSnapshot from the db.</summary>
		public static ClaimSnapshot GetOne(long claimSnapshotNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ClaimSnapshot>(MethodBase.GetCurrentMethod(),claimSnapshotNum);
			}
			return Crud.ClaimSnapshotCrud.SelectOne(claimSnapshotNum);
		}

		///<summary>Gets a list of ClaimSnapShot for all of the passed in ClaimProcNums.</summary>
		public static List<ClaimSnapshot> GetByClaimProcNums(List<long> listClaimProcNums) {
			if(listClaimProcNums==null || listClaimProcNums.Count==0) {
				return new List<ClaimSnapshot>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClaimSnapshot>>(MethodBase.GetCurrentMethod(),listClaimProcNums);
			}
			string command="SELECT * FROM claimsnapshot WHERE ClaimProcNum IN("+string.Join(",",listClaimProcNums.Select(x => POut.Long(x)))+")";
			return Crud.ClaimSnapshotCrud.SelectMany(command);
		}
		#endregion

		#region Insert
		///<summary>Creates a snapshot for the claimprocs passed in.  Used for reporting purposes.
		///If called from Open Dental Service, ignore passed in claimprocs and make snapshots for the entire day of completed procedures in a different method.
		///When passing in claimprocs, the implementor will need to ensure that only primary claimprocs are being saved.
		///Only creates snapshots if the feature is enabled and if the claimproc is of certain statuses.</summary>
		public static void CreateClaimSnapshot(List<ClaimProc> listClaimProcs,ClaimSnapshotTrigger triggerType,string claimType) {
			Meth.NoCheckMiddleTierRole();
			if(!PrefC.GetBool(PrefName.ClaimSnapshotEnabled)
				|| PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true)!=triggerType) 
			{
				return;
			}
			if(triggerType == ClaimSnapshotTrigger.Service) {
				CreateClaimSnapShotService();
				return;
			}
			Dictionary<long,double> dictionaryCompletedProcFees=Procedures.GetProcsFromClaimProcs(listClaimProcs).ToDictionary(x => x.ProcNum,x => x.ProcFee);
			//This list will be used to check for existing claimsnapshots for the claimprocs passed in. We will update exisiting snapshots. 
			List<ClaimSnapshot> listClaimSnapshotsOld=GetByClaimProcNums(listClaimProcs.Select(x => x.ClaimProcNum).ToList());
			//Loop through all the claimprocs and create a claimsnapshot entry for each.
			for(int i=0;i<listClaimProcs.Count;i++){
				//only create snapshots for 0=NotReceived, 1=Received, 4=Supplemental, 5=CapClaim, 6=Estimate (only if triggerType=Service), 
				//7=CapComplete, and 8=CapEstimate (only if triggerType=Service)
				if(listClaimProcs[i].Status.In(ClaimProcStatus.Preauth,ClaimProcStatus.Adjustment,ClaimProcStatus.Estimate,ClaimProcStatus.CapEstimate)) {
					continue;
				}
				//get the procfee
				double procFee;
				if(!dictionaryCompletedProcFees.TryGetValue(listClaimProcs[i].ProcNum,out procFee)) {
					procFee=0;
				}
				//If there is an existing claimsnapshot created Today for the current listClaimProcs[i].ProcNum, listClaimProcs[i].ClaimProcNum, claimType, and the ClaimSnapshotTrigger 
				//is not Service, then update it. Otherwise, create a new one. 
				//This fixes an issue with reports not showing the correct writeoffs. Ex. A procedure was completed, a claim was created, the claim was deleted, 
				//the writeoff was modified on the claimproc, then a new claim was created.
				ClaimSnapshot claimSnapshotExisting=listClaimSnapshotsOld.Find(x => x.DateTEntry.Date==DateTime.Today.Date 
					&& x.ProcNum==listClaimProcs[i].ProcNum 
					&& x.ClaimProcNum==listClaimProcs[i].ClaimProcNum 
					&& x.ClaimType==claimType 
					&& x.SnapshotTrigger!=ClaimSnapshotTrigger.Service);
				if(claimSnapshotExisting!=null) {
					SetSnapshotFields(claimSnapshotExisting,listClaimProcs[i],procFee,triggerType,claimType);
					ClaimSnapshots.Update(claimSnapshotExisting);
					continue;
				}
				ClaimSnapshot claimSnapshot=new ClaimSnapshot();
				SetSnapshotFields(claimSnapshot,listClaimProcs[i],procFee,triggerType,claimType);
				ClaimSnapshots.Insert(claimSnapshot);
			}
		}

		private static void SetSnapshotFields(ClaimSnapshot claimSnapshot,ClaimProc claimProc,double procFee,ClaimSnapshotTrigger claimSnapshotTrigger,string claimType) {
			claimSnapshot.ProcNum=claimProc.ProcNum;
			claimSnapshot.Writeoff=claimProc.WriteOff;
			claimSnapshot.InsPayEst=claimProc.InsEstTotal;
			claimSnapshot.Fee=procFee;
			claimSnapshot.ClaimProcNum=claimProc.ClaimProcNum;
			claimSnapshot.SnapshotTrigger=claimSnapshotTrigger;
			claimSnapshot.ClaimType=claimType;
		}

		///<summary>Users using the OpenDentalService to create claim snapshots only get primary claim snap shots created.</summary>
		private static void CreateClaimSnapShotService() {
			Meth.NoCheckMiddleTierRole();
			List<Procedure> listProceduresCompleted = Procedures.GetCompletedByDateCompleteForDateRange(DateTime.Today,DateTime.Today);
			List<ClaimProc> listClaimProcs = ClaimProcs.GetForProcsWithOrdinal(listProceduresCompleted.Select(x => x.ProcNum).ToList(),1)
				.FindAll(x => !x.Status.In(ClaimProcStatus.Preauth,ClaimProcStatus.Adjustment));
			List<PatPlan> listPatPlans = PatPlans.GetListByInsSubNums(listClaimProcs.Select(x => x.InsSubNum).ToList());
			listClaimProcs=listClaimProcs
				.OrderByDescending(x => x.ClaimNum)//order by claim num
				.ThenByDescending(x => x.SecDateEntry) //then by creation date
					//group by procnum and ordinal
				.GroupBy(x => new { ProcNum = x.ProcNum,Ordinal = PatPlans.GetOrdinal(x.InsSubNum,listPatPlans.Where(y => y.PatNum == x.PatNum).ToList()) }) 
				.Select(x => x.First())//get the first for each group
				.ToList();
			//Loop through all the claimprocs and create a claimsnapshot entry for each.
			for(int i = 0;i<listClaimProcs.Count;i++) {
				ClaimProc claimProc=listClaimProcs[i];
				if(claimProc.Status==ClaimProcStatus.CapClaim
					|| claimProc.Status==ClaimProcStatus.CapComplete
					|| claimProc.Status==ClaimProcStatus.CapEstimate
					|| claimProc.Status==ClaimProcStatus.Preauth
					|| claimProc.Status==ClaimProcStatus.Supplemental
					|| claimProc.Status==ClaimProcStatus.InsHist) 
				{
					continue;
				}
				//get the procfee
				double procFee = 0;
				Procedure procedure = listProceduresCompleted.Find(x => x.ProcNum==claimProc.ProcNum);
				if(procedure != null) {
					procFee=procedure.ProcFee;
				}
				//get the writeoff
				double writeoffAmt = claimProc.WriteOff;
				//For the Service, only use the WriteOff amount on the claimproc if the claimproc is associated to a claim, 
				//as this means that value has been set.
				if(claimProc.Status!=ClaimProcStatus.NotReceived && claimProc.Status!=ClaimProcStatus.Received) {
					if(claimProc.WriteOffEstOverride!=-1) {
						writeoffAmt=claimProc.WriteOffEstOverride;
					}
					else {
						writeoffAmt=claimProc.WriteOffEst;
					}
				}
				//create the snapshot
				ClaimSnapshot claimSnapshot = new ClaimSnapshot();
				claimSnapshot.ProcNum=claimProc.ProcNum;
				claimSnapshot.Writeoff=writeoffAmt;
				claimSnapshot.InsPayEst=claimProc.InsEstTotal;
				claimSnapshot.Fee=procFee;
				claimSnapshot.ClaimProcNum=claimProc.ClaimProcNum;
				claimSnapshot.SnapshotTrigger=ClaimSnapshotTrigger.Service;
				ClaimSnapshots.Insert(claimSnapshot);
			}
		}

		///<summary></summary>
		public static long Insert(ClaimSnapshot claimSnapshot) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				claimSnapshot.ClaimSnapshotNum=Meth.GetLong(MethodBase.GetCurrentMethod(),claimSnapshot);
				return claimSnapshot.ClaimSnapshotNum;
			}
			string command="SELECT COUNT(*) FROM claimsnapshot WHERE ProcNum="+POut.Long(claimSnapshot.ProcNum)+" AND ClaimProcNum='"+claimSnapshot.ClaimProcNum+"'";
			if(Db.GetCount(command)!="0") {
				return 0;//Do nothing.
			}
			return Crud.ClaimSnapshotCrud.Insert(claimSnapshot);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(ClaimSnapshot claimSnapshot) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimSnapshot);
				return;
			}
			Crud.ClaimSnapshotCrud.Update(claimSnapshot);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long claimSnapshotNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimSnapshotNum);
				return;
			}
			Crud.ClaimSnapshotCrud.Delete(claimSnapshotNum);
		}

		public static void DeleteForClaimProcs(List<long> listClaimProcNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClaimProcNums);
				return;
			}
			if(listClaimProcNums==null || listClaimProcNums.Count < 1) {
				return;
			}
			string command="DELETE FROM claimsnapshot WHERE ClaimProcNum IN ("+string.Join(",",listClaimProcNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}
		#endregion
	}
}