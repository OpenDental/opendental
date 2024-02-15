using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AutoCodeItems{
		#region Cache Pattern

		///<summary>Utilizes the NonPkAbs version of CacheDict because it uses CodeNum as the Key instead of the PK AutoCodeItemNum.</summary>
		private class AutoCodeItemCache : CacheDictNonPkAbs<AutoCodeItem,long,AutoCodeItem> {
			protected override List<AutoCodeItem> GetCacheFromDb() {
				string command="SELECT * FROM autocodeitem";
				return Crud.AutoCodeItemCrud.SelectMany(command);
			}
			protected override List<AutoCodeItem> TableToList(DataTable table) {
				return Crud.AutoCodeItemCrud.TableToList(table);
			}
			protected override AutoCodeItem Copy(AutoCodeItem autoCodeItem) {
				return autoCodeItem.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,AutoCodeItem> dictAutoCodeItem) {
				return Crud.AutoCodeItemCrud.ListToTable(dictAutoCodeItem.Values.ToList(),"AutoCodeItem");
			}
			protected override void FillCacheIfNeeded() {
				AutoCodeItems.GetTableFromCache(false);
			}
			protected override long GetDictKey(AutoCodeItem autoCodeItem) {
				return autoCodeItem.CodeNum;
			}
			protected override AutoCodeItem GetDictValue(AutoCodeItem autoCodeItem) {
				return autoCodeItem;
			}
			protected override AutoCodeItem CopyDictValue(AutoCodeItem autoCodeItem) {
				return autoCodeItem.Copy();
			}

			protected override DataTable ListToTable(List<AutoCodeItem> listAllItems) {
				return Crud.AutoCodeItemCrud.ListToTable(listAllItems);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoCodeItemCache _autoCodeItemCache=new AutoCodeItemCache();

		public static AutoCodeItem GetOne(long codeNum) {
			return _autoCodeItemCache.GetOne(codeNum);
		}

		public static bool GetContainsKey(long codeNum) {
			return _autoCodeItemCache.GetContainsKey(codeNum);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoCodeItemCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoCodeItemCache.FillCacheFromTable(table);
				return table;
			}
			return _autoCodeItemCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_autoCodeItemCache.ClearCache();
		}
		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(AutoCodeItem autoCodeItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				autoCodeItem.AutoCodeItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),autoCodeItem);
				return autoCodeItem.AutoCodeItemNum;
			}
			return Crud.AutoCodeItemCrud.Insert(autoCodeItem);
		}

		///<summary></summary>
		public static void Update(AutoCodeItem autoCodeItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoCodeItem);
				return;
			}
			Crud.AutoCodeItemCrud.Update(autoCodeItem);
		}

		///<summary></summary>
		public static void Delete(AutoCodeItem autoCodeItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoCodeItem);
				return;
			}
			string command= "DELETE FROM autocodeitem WHERE AutoCodeItemNum = '"
				+POut.Long(autoCodeItem.AutoCodeItemNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Delete(long autoCodeNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoCodeNum);
				return;
			}
			string command= "DELETE FROM autocodeitem WHERE AutoCodeNum = '"
				+POut.Long(autoCodeNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Gets from cache.  No call to db.</summary>
		public static List<AutoCodeItem> GetListForCode(long autoCodeNum) {
			//No need to check MiddleTierRole; no call to db.
			return _autoCodeItemCache.GetWhereFromList(x => x.AutoCodeNum==autoCodeNum);
		}

		//-----

		///<summary>Only called from ContrChart.listProcButtons_Click.  Called once for each tooth selected and for each autocode item attached to the button.</summary>
		public static long GetCodeNum(long autoCodeNum,string toothNum,string surf,bool isAdditional,long patNum,int age,bool willBeMissing) {
			//No need to check MiddleTierRole; no call to db.
			bool areAllCondsMet;
			List<AutoCodeItem> listAutoCodeItemsForCode=AutoCodeItems.GetListForCode(autoCodeNum);
			if(listAutoCodeItemsForCode.Count==0) {
				return 0;
			}
			//bool willBeMissing=Procedures.WillBeMissing(toothNum,patNum);//moved this out so that this method has no db call
			List<AutoCodeCond> listAutoCodeConds;
			for(int i=0;i<listAutoCodeItemsForCode.Count;i++) {
				listAutoCodeConds=AutoCodeConds.GetListForItem(listAutoCodeItemsForCode[i].AutoCodeItemNum);
				areAllCondsMet=true;
				for(int j=0;j<listAutoCodeConds.Count;j++) {
					if(!AutoCodeConds.ConditionIsMet(listAutoCodeConds[j].Cond,toothNum,surf,isAdditional,willBeMissing,age)) {
						areAllCondsMet=false;
					}
				}
				if(areAllCondsMet) {
					return listAutoCodeItemsForCode[i].CodeNum;
				}
			}
			return listAutoCodeItemsForCode[0].CodeNum;//if couldn't find a better match
		}

		///<summary>Only called when closing the procedure edit window. Usually returns the supplied CodeNum, unless a better match is found.</summary>
		public static long VerifyCode(long codeNum,string toothNum,string surf,bool isAdditional,long patNum,int age) {
			//No need to check MiddleTierRole; no call to db.
			bool areAllCondsMet;
			AutoCode autoCode;
			if(!GetContainsKey(codeNum)) {
				return codeNum;
			}
			if(!AutoCodes.GetContainsKey(GetOne(codeNum).AutoCodeNum)) {
				return codeNum;//just in case.
			}
			autoCode=AutoCodes.GetOne(GetOne(codeNum).AutoCodeNum);
			if(autoCode.LessIntrusive) {
				return codeNum;
			}
			bool willBeMissing=Procedures.WillBeMissing(toothNum,patNum);
			List<AutoCodeItem> listAutoCodeItems=AutoCodeItems.GetListForCode(GetOne(codeNum).AutoCodeNum);
			List<AutoCodeCond> listAutoCodeConds;
			for(int i=0;i<listAutoCodeItems.Count;i++) {
				listAutoCodeConds=AutoCodeConds.GetListForItem(listAutoCodeItems[i].AutoCodeItemNum);
				areAllCondsMet=true;
				for(int j=0;j<listAutoCodeConds.Count;j++) {
					if(!AutoCodeConds.ConditionIsMet(listAutoCodeConds[j].Cond,toothNum,surf,isAdditional,willBeMissing,age)) {
						areAllCondsMet=false;
					}
				}
				if(areAllCondsMet) {
					return listAutoCodeItems[i].CodeNum;
				}
			}
			return codeNum;//if couldn't find a better match
		}

		///<summary>Checks inputs and returns either the same code or a slightly different code that is a better fit for the situation.</summary>
		public static long GetRecommendedCodeNum(Procedure procedure,ProcedureCode procedureCode,Patient patient,bool isMandibular,
			List<ClaimProc> claimProcsForProc) 
		{
			//No remoting role check; no call to db.
			long codeNumRecommended=procedure.CodeNum;
			//these areas have no autocodes
			if(procedureCode.TreatArea==TreatmentArea.Mouth
				|| procedureCode.TreatArea==TreatmentArea.None
				|| procedureCode.TreatArea==TreatmentArea.Quad
				|| procedureCode.TreatArea==TreatmentArea.Sextant
				|| Procedures.IsAttachedToClaim(procedure,claimProcsForProc)) {
				return codeNumRecommended;
			}
			//this represents the suggested code based on the autocodes set up.
			if(procedureCode.TreatArea==TreatmentArea.Arch) {
				if(string.IsNullOrEmpty(procedure.Surf)) {
					return codeNumRecommended;
				}
				if(procedure.Surf=="U") {
					codeNumRecommended=AutoCodeItems.VerifyCode(procedureCode.CodeNum,"1","",procedure.IsAdditional,patient.PatNum,patient.Age);//max
				}
				else {
					codeNumRecommended=AutoCodeItems.VerifyCode(procedureCode.CodeNum,"32","",procedure.IsAdditional,patient.PatNum,patient.Age);//mand
				}
			}
			else if(procedureCode.TreatArea==TreatmentArea.ToothRange) {
				if(string.IsNullOrEmpty(procedure.ToothRange)) {
					return codeNumRecommended;
				}
				//test for max or mand.
				codeNumRecommended=AutoCodeItems.VerifyCode(procedureCode.CodeNum,(isMandibular) ? "32" : "1","",procedure.IsAdditional,patient.PatNum,patient.Age);
			}
			else {//surf or tooth
				string claimSurf=Tooth.SurfTidyForClaims(procedure.Surf,procedure.ToothNum);
				codeNumRecommended=AutoCodeItems.VerifyCode(procedureCode.CodeNum,procedure.ToothNum,claimSurf,procedure.IsAdditional,patient.PatNum,patient.Age);
			}
			return codeNumRecommended;
		}
	}
}