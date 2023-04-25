using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class FeeSchedGroups{
		#region Get Methods
		/// <summary>There will be at most one result for a FeeSched/Clinic combination.  Can return NULL.</summary>
		public static FeeSchedGroup GetOneForFeeSchedAndClinic(long feeSchedNum, long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<FeeSchedGroup>(MethodBase.GetCurrentMethod(),feeSchedNum,clinicNum);
			}
			//ClinicNums are stored as a comma delimited list requiring a LIKE condition.
			string command=$"SELECT * FROM feeschedgroup WHERE FeeSchedNum={feeSchedNum} AND FIND_IN_SET('{clinicNum}',ClinicNums)";
			return Crud.FeeSchedGroupCrud.SelectOne(command);
		}

		///<summary>Returns a list of every single FeeSchedGroup in the database.</summary>
		public static List<FeeSchedGroup> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FeeSchedGroup>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM feeschedgroup";
			return Crud.FeeSchedGroupCrud.SelectMany(command);
		}

		///<summary>Returns a list of all FeeSchedGroups for a given FeeSched.  A feeSchedNum of 0 will return all feeschedgroups.</summary>
		public static List<FeeSchedGroup> GetAllForFeeSched(long feeSchedNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FeeSchedGroup>>(MethodBase.GetCurrentMethod(),feeSchedNum);
			}
			string command=$"SELECT * FROM feeschedgroup {(feeSchedNum>0? $"WHERE FeeSchedNum={feeSchedNum}":"")}";
			return Crud.FeeSchedGroupCrud.SelectMany(command);
		}

		#endregion

		#region Insert

		///<summary></summary>
		public static long Insert(FeeSchedGroup feeSchedGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				feeSchedGroup.FeeSchedGroupNum=Meth.GetLong(MethodBase.GetCurrentMethod(),feeSchedGroup);
				return feeSchedGroup.FeeSchedGroupNum;
			}
			return Crud.FeeSchedGroupCrud.Insert(feeSchedGroup);
		}
		#endregion

		#region Update

		///<summary></summary>
		public static void Update(FeeSchedGroup feeSchedGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSchedGroup);
				return;
			}
			Crud.FeeSchedGroupCrud.Update(feeSchedGroup);
		}

		#endregion

		#region Delete

		///<summary></summary>
		public static void Delete(long feeSchedGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSchedGroupNum);
				return;
			}
			Crud.FeeSchedGroupCrud.Delete(feeSchedGroupNum);
		}
		#endregion

		#region Fee Operations
		///<summary>Takes a list of fees that have been inserted/updated and copies those changes to the rest of the clinics in the feeschedgroup.
		///listFeesOld only sent in from SyncGroupFees.</summary>
		public static void UpsertGroupFees(List<Fee> listFees,List<Fee> listFeesOld=null) {
			//No need to check MiddleTierRole; no call to db.
			if(listFees.IsNullOrEmpty()) {
				return;
			}
			List<Fee> listFeesDb=new List<Fee>();
			if(!listFeesOld.IsNullOrEmpty()) {
				listFeesOld.Select(x => x.Copy()).ToList();//local copy because we don't want the list to be changed for fee sync later
			}
			//list of all FeeSchedGroups
			List<FeeSchedGroup> listFeeSchedGroups=GetListFeeSchedGroups(listFees.Select(x => x.FeeSched).ToList());
			for(int i=0;i<listFees.Count;i++){
				List<FeeSchedGroup> listFeeSchedGroupsForSched=listFeeSchedGroups.FindAll(x => x.FeeSchedNum==listFees[i].FeeSched);
				if(listFeeSchedGroupsForSched.Count==0) {//if FeeSched is not part of a group
					continue;
				}
				//first group with f.ClinicNum that also has a ClinicNum other than f.ClinicNum
				FeeSchedGroup feeSchedGroup=listFeeSchedGroupsForSched.Find(x => x.ListClinicNumsAll.Contains(listFees[i].ClinicNum) && x.ListClinicNumsAll.Any(y => y!=listFees[i].ClinicNum));
				if(feeSchedGroup==null) {
					continue;
				}
				//the fees in listFees will be updated outside of this method.  So skip clinics in the group where a fee already exists in listFees for this
				//CodeNum, FeeSched, ProvNum, and ClinicNum with matching Amount
				List<long> listClinicNums=feeSchedGroup.ListClinicNumsAll
					.FindAll(x => !listFees.Exists(y => y.CodeNum==listFees[i].CodeNum && y.FeeSched==listFees[i].FeeSched && y.ProvNum==listFees[i].ProvNum && y.ClinicNum==x && y.Amount==listFees[i].Amount));
				if(listClinicNums.Count==0) {
					continue;
				}
				List<Fee> listFeesForClinics;
				if(listFeesDb.IsNullOrEmpty()) {
					listFeesForClinics=Fees.GetListExactForClinicsFromDb(listFees[i].CodeNum,listFees[i].FeeSched,listFees[i].ProvNum,listClinicNums);
				}
				else {
					listFeesForClinics=listFeesDb.FindAll(x => x.CodeNum==listFees[i].CodeNum && x.FeeSched==listFees[i].FeeSched && x.ProvNum==listFees[i].ProvNum && listClinicNums.Contains(x.ClinicNum));
				}
				List<Fee> listFeesToInsert=listClinicNums.FindAll(x => listFeesForClinics.All(y => y.ClinicNum!=x))
					.Select(x => new Fee { Amount=listFees[i].Amount,FeeSched=listFees[i].FeeSched,CodeNum=listFees[i].CodeNum,ClinicNum=x,ProvNum=listFees[i].ProvNum }).ToList();
				List<Fee> listFeesToUpdate=listFeesForClinics.FindAll(x => !CompareDouble.IsEqual(x.Amount,listFees[i].Amount));
				if(listFeesToInsert.Count>0) {
					Fees.InsertMany(listFeesToInsert,false);//Don't call FeeSchedGroup logic or we'll be sucked into an infinite loop.
					listFeesDb.AddRange(listFeesToInsert);
				}
				if(listFeesToUpdate.Count>0) {
					UpdateFeeAmounts(listFeesToUpdate.Select(x => x.FeeNum).ToList(),listFees[i].Amount);
					for(int j=0;j<listFeesToUpdate.Count;j++) {
						listFeesToUpdate[j].Amount=listFees[i].Amount;
					}
				}
			}
		}

		///<summary>Takes a list of FeeSchedNums and returns a List of FeeSchedGroups.</summary>
		public static List<FeeSchedGroup> GetListFeeSchedGroups(List<long> listFeeSchedNums) {
			if(listFeeSchedNums.IsNullOrEmpty()) {
				return new List<FeeSchedGroup>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FeeSchedGroup>>(MethodBase.GetCurrentMethod(),listFeeSchedNums);
			}
			string command=$"SELECT * FROM feeschedgroup WHERE FeeSchedNum IN ({string.Join(",",listFeeSchedNums.Distinct())})";
			return Crud.FeeSchedGroupCrud.SelectMany(command);
		}

		///<summary>Takes a list of fees to be deleted and deletes them from rest of the clinics in the feeschedgroup.</summary>
		public static void DeleteGroupFees(List<long> listFeeNums) {
			//No need to check MiddleTierRole; no call to db.
			DeleteGroupFees(Fees.GetManyByFeeNum(listFeeNums));
		}

		///<summary>Takes a list of fees to be deleted and deletes the fees for the other clinics in the feeschedgroup.</summary>
		public static void DeleteGroupFees(List<Fee> listFees) {
			//No need to check MiddleTierRole; no call to db.
			if(listFees.IsNullOrEmpty()) {
				return;
			}
			//List of FeeSchedGroups 
			List<FeeSchedGroup> listFeeSchedGroups=GetListFeeSchedGroups(listFees.Select(x => x.FeeSched).ToList());
			List<long> listFeeNumsToDelete=new List<long>();
			FeeSchedGroup feeSchedGroup;
			for(int i=0;i<listFees.Count;i++){
				if(listFeeNumsToDelete.Contains(listFees[i].FeeNum)) {//part of a group of fees already added to the list to be deleted, skip it
					continue;
				}
				List<FeeSchedGroup> listFeeSchedGroupsClinic=listFeeSchedGroups.FindAll(x => x.FeeSchedNum==listFees[i].FeeSched);
				if(listFeeSchedGroupsClinic.Count==0) {//FeeSched is not part of a group
					continue;
				}
				//first group with f.ClinicNum that also has a ClinicNum other than f.ClinicNum
				feeSchedGroup=listFeeSchedGroupsClinic.Find(x => x.ListClinicNumsAll.Contains(listFees[i].ClinicNum) && x.ListClinicNumsAll.Any(y => y!=listFees[i].ClinicNum));
				if(feeSchedGroup==null) {
					continue;
				}
				//list of all fees with the same CodeNum, FeeSched, ProvNum, and with ClinicNum in the group with the current fee.
				listFeeNumsToDelete.AddRange(Fees.GetListExactForClinicsFromDb(listFees[i].CodeNum,listFees[i].FeeSched,listFees[i].ProvNum,feeSchedGroup.ListClinicNumsAll)
					.Select(x => x.FeeNum));
			}
			//Remove any fees where the FeeNum is in listFees, since those are handled outside this method.
			listFeeNumsToDelete.RemoveAll(x => listFees.Any(y => x==y.FeeNum));
			if(listFeeNumsToDelete.Count==0) {
				return;
			}
			Fees.DeleteMany(listFeeNumsToDelete.Distinct().ToList(),false);
		}

		///<summary>Only used by Fees.SynchList, this is basically a copy of the CRUD generated sync method with slight tweaks to work with FeeSchedGroups.
		///Only calls the group helper methods that only modify the other fees in the group, the fess in listFeesNew and listFeesOld will be left to the
		///fees.cs sync method to handle.</summary>
		public static void SyncGroupFees(List<Fee> listFeesNew,List<Fee> listFeesOld) {
			//No need to check MiddleTierRole; no call to db.
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<Fee> listUpsert=new List<Fee>();
			List<Fee> listDel=new List<Fee>();
			listFeesNew.Sort((Fee x,Fee y) => { return x.FeeNum.CompareTo(y.FeeNum); });//Anonymous function, sorts by compairing PK.
			listFeesOld.Sort((Fee x,Fee y) => { return x.FeeNum.CompareTo(y.FeeNum); });//Anonymous function, sorts by compairing PK.
			int idxNew=0;
			int idxDB=0;
			Fee fieldNew;
			Fee fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element
			//based on PrimaryKey.  If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.
			//If both lists contain the next item, the item will be updated.
			while(idxNew<listFeesNew.Count || idxDB<listFeesOld.Count) {
				fieldNew=null;
				if(idxNew<listFeesNew.Count) {
					fieldNew=listFeesNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listFeesOld.Count) {
					fieldDB=listFeesOld[idxDB];
				}
				if(fieldNew!=null && (fieldDB==null || fieldNew.FeeNum<fieldDB.FeeNum)) {
					//listNew has more items or newPK is less than dbPK, update required if fee is part of group
					listUpsert.Add(fieldNew);
					idxNew++;
					continue;
				}
				if(fieldDB!=null && (fieldNew==null || fieldDB.FeeNum<fieldNew.FeeNum)) {
					//listDB has more items or dbPK less than newPK, delete dbItem if fee is part of group 
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				if(Crud.FeeCrud.UpdateComparison(fieldNew,fieldDB)) {
					listUpsert.Add(fieldNew);
				}
				idxNew++;
				idxDB++;
			}
			//Check for groups to update with changes.
			UpsertGroupFees(listUpsert,listFeesOld);
			DeleteGroupFees(listDel);
		}

		///<summary>Only called from the fee sync when updating feeschedgroups, therefore does not check feeschedgroups here and is a private method so
		///it can't be called from outside this class.  Private is intentional!</summary>
		public static void UpdateFeeAmounts(List<long> listFeeNumsToUpdate,double newAmount) {
			if(listFeeNumsToUpdate.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFeeNumsToUpdate,newAmount);
				return;
			}
			string command=$@"UPDATE fee SET Amount={POut.Double(newAmount)}
				WHERE fee.FeeNum IN({string.Join(",",listFeeNumsToUpdate.Select(x => POut.Long(x)))})";
			Db.NonQ(command);
		}
		#endregion Fee Operations

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<FeeSchedGroup> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FeeSchedGroup>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM feeschedgroup WHERE PatNum = "+POut.Long(patNum);
			return Crud.FeeSchedGroupCrud.SelectMany(command);
		}
		
		///<summary>Gets one FeeSchedGroup from the db.</summary>
		public static FeeSchedGroup GetOne(long feeSchedGroupNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<FeeSchedGroup>(MethodBase.GetCurrentMethod(),feeSchedGroupNum);
			}
			return Crud.FeeSchedGroupCrud.SelectOne(feeSchedGroupNum);
		}
		#endregion Get Methods
		#region Modification Methods
		
		///<summary></summary>
		public static void Update(FeeSchedGroup feeSchedGroup){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSchedGroup);
				return;
			}
			Crud.FeeSchedGroupCrud.Update(feeSchedGroup);
		}
		
		#endregion Modification Methods
		
		*/
	}
}