using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsPlanPreferences{
		
		#region Methods - Get
		///<summary>Gets single InsPlanPreference entry from the db by insPlanPrefNum.</summary>
		public static InsPlanPreference GetOne(long insPlanPrefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<InsPlanPreference>(MethodBase.GetCurrentMethod(),insPlanPrefNum);
			}
			return Crud.InsPlanPreferenceCrud.SelectOne(insPlanPrefNum);
		}

		///<summary>Gets single InsPlanPreference entry from the db using a combination of FkeyType/Fkey/PlanNum.</summary>
		public static InsPlanPreference GetOne(long fKey,InsPlanPrefFKeyType fKeyType,long planNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<InsPlanPreference>(MethodBase.GetCurrentMethod(),fKey,fKeyType,planNum);
			}
			string command=$@"SELECT * FROM insplanpreference 
				WHERE FKey={POut.Long(fKey)}
				AND FKeyType={POut.Int((int)fKeyType)}
				AND PlanNum={POut.Long(planNum)}";
			return Crud.InsPlanPreferenceCrud.SelectOne(command);
		}

		///<summary>Retrieves InsPlanPreference entries for multiple insurance plans based on FkeyType and FKey.</summary>
		public static List<InsPlanPreference> GetManyForPlanNums(long fKey,InsPlanPrefFKeyType fKeyType,List<long> listPlanNums){
			if(listPlanNums.Count==0) {
				return new List<InsPlanPreference>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<InsPlanPreference>>(MethodBase.GetCurrentMethod(),fKey,fKeyType,listPlanNums);
			}
			string command=$@"SELECT * FROM insplanpreference
				WHERE FKey={POut.Long(fKey)} 
				AND FKeyType={POut.Int((int)fKeyType)} 
				AND PlanNum IN ({string.Join(",",listPlanNums)})";
			return Crud.InsPlanPreferenceCrud.SelectMany(command);
		}

		///<summary>Retrieves InsPlanPreference entries for multiple FKeys based on FkeyType and planNum.</summary>
		public static List<InsPlanPreference> GetManyForFKeys(List<long> listFKeys,InsPlanPrefFKeyType fKeyType,InsPlan insPlan){
			if(listFKeys.Count==0 || insPlan==null) {
				return new List<InsPlanPreference>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<InsPlanPreference>>(MethodBase.GetCurrentMethod(),listFKeys,fKeyType,insPlan);
			}
			string command=$@"SELECT * FROM insplanpreference 
				WHERE PlanNum={POut.Long(insPlan.PlanNum)}
				AND FKeyType={POut.Int((int)fKeyType)}
				AND FKey IN ({string.Join(",",listFKeys)})";
			return Crud.InsPlanPreferenceCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(InsPlanPreference insPlanPreference){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				insPlanPreference.InsPlanPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insPlanPreference);
				return insPlanPreference.InsPlanPrefNum;
			}
			return Crud.InsPlanPreferenceCrud.Insert(insPlanPreference);
		}

		///<summary></summary>
		public static void Update(InsPlanPreference insPlanPreference){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insPlanPreference);
				return;
			}
			Crud.InsPlanPreferenceCrud.Update(insPlanPreference);
		}

		///<summary>Updates or inserts a InsPlanPreference object for every PlanNum provided.</summary>
		public static void UpsertMany(long fKey,InsPlanPrefFKeyType fKeyType,List<long> listPlanNums,string valueString){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fKey,fKeyType,listPlanNums,valueString);
				return;
			}
			List<InsPlanPreference> listInsPlanPreferences=GetManyForPlanNums(fKey,fKeyType,listPlanNums);
			for(int i=0;i<listPlanNums.Count;i++) {
				InsPlanPreference insPlanPreference=listInsPlanPreferences.Find(x => x.PlanNum==listPlanNums[i]);
				if(insPlanPreference==null) {
					insPlanPreference=new InsPlanPreference();
					insPlanPreference.PlanNum=listPlanNums[i];
					insPlanPreference.FKey=fKey;
					insPlanPreference.FKeyType=fKeyType;
					insPlanPreference.ValueString=valueString;
					Crud.InsPlanPreferenceCrud.Insert(insPlanPreference);
					continue;
				}
				InsPlanPreference insPlanPreferenceOld=insPlanPreference.Copy();
				insPlanPreference.ValueString=valueString;
				Crud.InsPlanPreferenceCrud.Update(insPlanPreference,insPlanPreferenceOld);
			}
		}

		///<summary>Deletes a single InsPlanPreference entry in the database using insPlanPrefNum</summary>
		public static void Delete(long insPlanPrefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insPlanPrefNum);
				return;
			}
			Crud.InsPlanPreferenceCrud.Delete(insPlanPrefNum);
		}
		
		
		///<summary>Deletes all of the rows that match the fKey and fKeyType combination for the ins plans passed in.</summary>
		public static void DeleteMany(long fKey,InsPlanPrefFKeyType fKeyType,List<long> listPlanNums) {
			if(listPlanNums.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fKey,fKeyType,listPlanNums);
				return;
			}
			string command=$@"DELETE FROM insplanpreference 
				WHERE FKey={POut.Long(fKey)} 
				AND FKeyType={POut.Int((int)fKeyType)} 
				AND PlanNum IN ({string.Join(",",listPlanNums)})";
			Db.NonQ(command);
		}
		#endregion Methods - Modify

		#region Methods - Misc
		///<summary>Makes a call to the db. Checks if Insurance plan has NoBillIns override for the passed in procedureCode. If no, returns the procedure code NoBillIns value.
		///If insurance plan has an override, then returns the insurance plan NoBillIns override value.</summary>
		public static bool NoBillIns(ProcedureCode procedureCode,InsPlan insPlan){
			//No need to check MiddleTierRole; no call to db.
			if(insPlan==null) {
				return procedureCode.NoBillIns;
			}
			InsPlanPreference insPlanPreference=GetOne(procedureCode.CodeNum,InsPlanPrefFKeyType.ProcCodeNoBillIns,insPlan.PlanNum);
			if(insPlanPreference==null) {
				return procedureCode.NoBillIns;
			}
			NoBillInsOverride noBillInsOverride=PIn.Enum<NoBillInsOverride>(insPlanPreference.ValueString);
			switch(noBillInsOverride) {
				case NoBillInsOverride.BillToIns:
					return false;
				case NoBillInsOverride.DoNotUsuallyBillToIns:
					return true;
			} 
			return procedureCode.NoBillIns;
		}

		///<summary>Does not make a call to the db. Checks if Insurance plan has NoBillIns override in the passed in list of InsPlanPreferences for the specified procedureCode.
		///If no, returns the procedure code NoBillIns value. If insurance plan has an override, then returns the insurance plan NoBillIns override value.
		///Use this to avoid making multiple calls to db. </summary>
		public static bool NoBillIns(ProcedureCode procedureCode,List<InsPlanPreference> listInsPlanPreferences){
			//No need to check MiddleTierRole; no call to db.
			if(listInsPlanPreferences.IsNullOrEmpty()) {
				return procedureCode.NoBillIns;
			}
			InsPlanPreference insPlanPreference=listInsPlanPreferences.Find(x => x.FKey==procedureCode.CodeNum && x.FKeyType==InsPlanPrefFKeyType.ProcCodeNoBillIns);
			if(insPlanPreference==null) {
				return procedureCode.NoBillIns;
			}
			NoBillInsOverride noBillInsOverride=PIn.Enum<NoBillInsOverride>(insPlanPreference.ValueString);
			switch(noBillInsOverride) {
				case NoBillInsOverride.BillToIns:
					return false;
				case NoBillInsOverride.DoNotUsuallyBillToIns:
					return true;
			} 
			return procedureCode.NoBillIns;
		}
		#endregion Methods - Misc
	
	}
}