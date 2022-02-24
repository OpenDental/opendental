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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClaimSnapshot>(MethodBase.GetCurrentMethod(),claimSnapshotNum);
			}
			return Crud.ClaimSnapshotCrud.SelectOne(claimSnapshotNum);
		}

		///<summary>Gets a list of ClaimSnapShot for all of the passed in ClaimProcNums.</summary>
		public static List<ClaimSnapshot> GetByClaimProcNums(List<long> listClaimProcNums) {
			if(listClaimProcNums==null || listClaimProcNums.Count==0) {
				return new List<ClaimSnapshot>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			//No need to check RemotingRole; no call to db.
			if(!PrefC.GetBool(PrefName.ClaimSnapshotEnabled)
				|| PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true)!=triggerType) 
			{
				return;
			}
			if(triggerType == ClaimSnapshotTrigger.Service) {
				CreateClaimSnapShotService();
				return;
			}
			Dictionary<long,double> dictCompletedProcFees=Procedures.GetProcsFromClaimProcs(listClaimProcs).ToDictionary(x => x.ProcNum,x => x.ProcFee);
			//This list will be used to check for existing claimsnapshots for the claimprocs passed in. We will update exisiting snapshots. 
			List<ClaimSnapshot> listClaimSnapshotsOld=GetByClaimProcNums(listClaimProcs.Select(x => x.ClaimProcNum).ToList());
			//Loop through all the claimprocs and create a claimsnapshot entry for each.
			foreach(ClaimProc cp in listClaimProcs) {
				//only create snapshots for 0=NotReceived, 1=Received, 4=Supplemental, 5=CapClaim, 6=Estimate (only if triggerType=Service), 
				//7=CapComplete, and 8=CapEstimate (only if triggerType=Service)
				if(ListTools.In(cp.Status,ClaimProcStatus.Preauth,ClaimProcStatus.Adjustment,ClaimProcStatus.Estimate,ClaimProcStatus.CapEstimate)) {
					continue;
				}
				//get the procfee
				double procFee;
				if(!dictCompletedProcFees.TryGetValue(cp.ProcNum,out procFee)) {
					procFee=0;
				}
				//If there is an existing claimsnapshot created Today for the current cp.ProcNum, cp.ClaimProcNum, claimType, and the ClaimSnapshotTrigger 
				//is not Service, then update it. Otherwise, create a new one. 
				//This fixes an issue with reports not showing the correct writeoffs. Ex. A procedure was completed, a claim was created, the claim was deleted, 
				//the writeoff was modified on the claimproc, then a new claim was created.
				ClaimSnapshot existingSnapshot=listClaimSnapshotsOld.FirstOrDefault(x => x.DateTEntry.Date==DateTime.Today.Date && x.ProcNum==cp.ProcNum 
					&& x.ClaimProcNum==cp.ClaimProcNum && x.ClaimType==claimType && x.SnapshotTrigger!=ClaimSnapshotTrigger.Service);
				if(existingSnapshot!=null) {
					SetSnapshotFields(existingSnapshot,cp,procFee,triggerType,claimType);
					ClaimSnapshots.Update(existingSnapshot);
					continue;
				}
				ClaimSnapshot snapshot=new ClaimSnapshot();
				SetSnapshotFields(snapshot,cp,procFee,triggerType,claimType);
				ClaimSnapshots.Insert(snapshot);
			}
		}

		private static void SetSnapshotFields(ClaimSnapshot snapshot,ClaimProc cp,double procFee,ClaimSnapshotTrigger triggerType,string claimType) {
			snapshot.ProcNum=cp.ProcNum;
			snapshot.Writeoff=cp.WriteOff;
			snapshot.InsPayEst=cp.InsEstTotal;
			snapshot.Fee=procFee;
			snapshot.ClaimProcNum=cp.ClaimProcNum;
			snapshot.SnapshotTrigger=triggerType;
			snapshot.ClaimType=claimType;
		}

		///<summary>Users using the OpenDentalService to create claim snapshots only get primary claim snap shots created.</summary>
		private static void CreateClaimSnapShotService() {
			//No need to check RemotingRole; no call to db.
			List<Procedure> listCompletedProcs = Procedures.GetCompletedByDateCompleteForDateRange(DateTime.Today,DateTime.Today);
			List<ClaimProc> listClaimProcs = ClaimProcs.GetForProcsWithOrdinal(listCompletedProcs.Select(x => x.ProcNum).ToList(),1).Where(x => !ListTools.In(x.Status,ClaimProcStatus.Preauth,ClaimProcStatus.Adjustment)).ToList();
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
				ClaimProc cpCur=listClaimProcs[i];
				if(cpCur.Status==ClaimProcStatus.CapClaim
					|| cpCur.Status==ClaimProcStatus.CapComplete
					|| cpCur.Status==ClaimProcStatus.CapEstimate
					|| cpCur.Status==ClaimProcStatus.Preauth
					|| cpCur.Status==ClaimProcStatus.Supplemental
					|| cpCur.Status==ClaimProcStatus.InsHist) 
				{
					continue;
				}
				//get the procfee
				double procFee = 0;
				Procedure procCur = listCompletedProcs.Find(x => x.ProcNum==cpCur.ProcNum);
				if(procCur != null) {
					procFee=procCur.ProcFee;
				}
				//get the writeoff
				double writeoffAmt = cpCur.WriteOff;
				//For the Service, only use the WriteOff amount on the claimproc if the claimproc is associated to a claim, 
				//as this means that value has been set.
				if(cpCur.Status!=ClaimProcStatus.NotReceived && cpCur.Status!=ClaimProcStatus.Received) {
					if(cpCur.WriteOffEstOverride!=-1) {
						writeoffAmt=cpCur.WriteOffEstOverride;
					}
					else {
						writeoffAmt=cpCur.WriteOffEst;
					}
				}
				//create the snapshot
				ClaimSnapshot snapshot = new ClaimSnapshot();
				snapshot.ProcNum=cpCur.ProcNum;
				snapshot.Writeoff=writeoffAmt;
				snapshot.InsPayEst=cpCur.InsEstTotal;
				snapshot.Fee=procFee;
				snapshot.ClaimProcNum=cpCur.ClaimProcNum;
				snapshot.SnapshotTrigger=ClaimSnapshotTrigger.Service;
				ClaimSnapshots.Insert(snapshot);
			}
		}

		///<summary></summary>
		public static long Insert(ClaimSnapshot claimSnapshot) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimSnapshot);
				return;
			}
			Crud.ClaimSnapshotCrud.Update(claimSnapshot);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long claimSnapshotNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimSnapshotNum);
				return;
			}
			Crud.ClaimSnapshotCrud.Delete(claimSnapshotNum);
		}

		public static void DeleteForClaimProcs(List<long> listClaimProcNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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