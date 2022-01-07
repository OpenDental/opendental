using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness {
	///<summary>Handles database commands related to the statementprod table in the db.</summary>
	public class StatementProds {
		#region Get Methods
		///<summary>Gets all of the StatementProds for a list of StatementNums.</summary>
		public static List<StatementProd> GetManyForStatements(List<long> listStatementNums) {
			if(listStatementNums.IsNullOrEmpty()) {
				return new List<StatementProd>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<StatementProd>>(MethodBase.GetCurrentMethod(),listStatementNums);
			}
			string command=$"SELECT * FROM statementprod WHERE StatementNum IN({String.Join(",",listStatementNums.Select(x => POut.Long(x)))})";
			return Crud.StatementProdCrud.SelectMany(command);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary>Takes a list of statement prods for statements and a list of statements prods for the same statements that exist in DB, then performs inserts, updates, and deletes to sync the new list to the DB.</summary>
		public static void Sync(List<StatementProd> listNewStatementProds,List<StatementProd>listStatementProdsInDb) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNewStatementProds,listStatementProdsInDb);
				return;
			}
			Crud.StatementProdCrud.Sync(listNewStatementProds,listStatementProdsInDb);
		}

		///<summary>Creates statement prods for a statement based off of the dataSet passed in, then syncs this list with the existing statementprods for the statement in the DB.</summary>
		public static void SyncForStatement(DataSet dataSet,long statementNum,long docNum) {
			//No need to check RemotingRole; no call to db.
			Dictionary<long,StatementProdSyncData> dictStatementProdSyncData=StatementProdSyncData.GetDictSyncData(new List<long>(){statementNum});
			if(!dictStatementProdSyncData.TryGetValue(statementNum,out StatementProdSyncData statementProdSyncData)) {
				statementProdSyncData=new StatementProdSyncData();//Initialize an empty one if it isn't present in the dictionary.
			}
			List<StatementProd> listStatementProdsFromDb=statementProdSyncData.GetListSatementProds();
			Sync(CreateManyForStatement(dataSet,statementNum,docNum,statementProdSyncData),listStatementProdsFromDb);
		}

		///<summary>Pass in a dictionary for which the keys are StatementNums and the values are statement DataSets. Creates statement prods for the statements based off of their dataSets, then syncs these statementprods with the existing statementprods for the statements in the DB.</summary>
		public static void SyncForMultipleStatements(Dictionary<long,StatementData> dictStmtData) {
			//No need to check RemotingRole; no call to db.
			List<StatementProd> listNewStatementProds=new List<StatementProd>();
			List<StatementProd> listStatementProdsFromDb=new List<StatementProd>();
			Dictionary<long,StatementProdSyncData> dictStatementProdSyncData=StatementProdSyncData.GetDictSyncData(dictStmtData.Keys.ToList());
			foreach(KeyValuePair<long,StatementData> stmtDataSetCur in dictStmtData) {
				if(!dictStatementProdSyncData.TryGetValue(stmtDataSetCur.Key,out StatementProdSyncData statementProdSyncData)) {
					statementProdSyncData=new StatementProdSyncData();//Initialize an empty one if it isn't present in the dictionary.
				}
				listStatementProdsFromDb.AddRange(statementProdSyncData.GetListSatementProds());
				listNewStatementProds.AddRange(
					CreateManyForStatement(stmtDataSetCur.Value.StmtDataSet,stmtDataSetCur.Key,stmtDataSetCur.Value.DocNum,statementProdSyncData));
			}
			Sync(listNewStatementProds,listStatementProdsFromDb);
		}

		///<summary>Creates StatementProds for all of the production items on a statement. Any StatementProds that already exist in the DB for the statement that would be created again, get copied from the DB so that the sync method maintains them. All new table and column requirements from the DataSet passed in need to be added to StatementData.StmtDataSet.</summary>
		private static List<StatementProd> CreateManyForStatement(DataSet dataSet,long statementNum,long docNum
			,StatementProdSyncData statementProdSyncData)
		{
			//No need to check RemotingRole; no call to db.
			List<StatementProd> listStatementProds=new List<StatementProd>();
			for(int i=0;i<dataSet.Tables.Count;i++) {
				DataTable tableCur=dataSet.Tables[i];
				//Each family member will have their own account table, so only consider tables that start with 'account'.
				if(!tableCur.TableName.StartsWith("account")) {
					continue;
				}
				for(int j=0;j<tableCur.Rows.Count;j++) {
					DataRow rowCur=tableCur.Rows[j];
					long procNum=PIn.Long(rowCur["ProcNum"].ToString());
					long adjNum=PIn.Long(rowCur["AdjNum"].ToString());
					long payPlanChargeNum=PIn.Long(rowCur["PayPlanChargeNum"].ToString());
					double creditsDouble=PIn.Double(rowCur["CreditsDouble"].ToString());
					long fKey;
					ProductionType prodType;
					if(procNum!=0) {
						fKey=procNum;
						prodType=ProductionType.Procedure;
					}
					else if(adjNum!=0 && PIn.Long(rowCur["ProcsOnObj"].ToString())==0) {
						fKey=adjNum;
						prodType=ProductionType.Adjustment;
					}
					else if(payPlanChargeNum!=0 && CompareDouble.IsZero(creditsDouble)) {
						fKey=payPlanChargeNum;
						prodType=ProductionType.PayPlanCharge;
					}
					else {
						continue;
					}
					StatementProd statementProd=new StatementProd();
					if(statementProdSyncData.TryGetStatementProd(fKey,prodType,out StatementProd existingStatementProd)) {
						statementProd=existingStatementProd.Copy();
					}
					else {
						statementProd.StatementNum=statementNum;
						statementProd.FKey=fKey;
						statementProd.ProdType=prodType;
						statementProd.LateChargeAdjNum=0;
					}
					statementProd.DocNum=docNum;//DocNum is the only thing that should change for existing statementprods that are being updated.
					listStatementProds.Add(statementProd);
					statementProdSyncData.RemoveStatementProd(fKey,prodType);
				}
			}
			//We have removed any StatementProds from our Db collection that are for production items that are still on the statement.
			//We are only left with StatementProds for production items that aren't on the statement anymore.
			//These may be associated to a late charge, so we should keep them.
			listStatementProds.AddRange(statementProdSyncData.GetListSatementProds());
			return listStatementProds;
		}

		///<summary>Used when creating late charge adjustments. All of the StatementProds for statements sent on or before the date passed in that have an FKey in the lists passed in get assigned the AdjNum, even those that aren't on the statement that the late charge is being made for.</summary>
		public static void UpdateLateChargeAdjNumForMany(long adjNum,List<long> listProcNums,List<long> listAdjNums
			,List<long> listPayPlanChargeNums,DateTime dateMaxUpdateStmtProd)
		{
			bool isOrStatementNeeded=false;
			if(listProcNums.IsNullOrEmpty() && listAdjNums.IsNullOrEmpty() && listPayPlanChargeNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adjNum,listProcNums,listAdjNums,listPayPlanChargeNums,dateMaxUpdateStmtProd);
				return;
			}
			string command=$@"UPDATE statementprod
				INNER JOIN statement
					ON statementprod.StatementNum=statement.StatementNum
					AND statement.DateSent<={POut.Date(dateMaxUpdateStmtProd)}
				SET statementprod.LateChargeAdjNum={POut.Long(adjNum)}
				WHERE statementprod.LateChargeAdjNum=0 
				AND ";//One of the lists below must be non-null and have an FKey in it.
			if(!listProcNums.IsNullOrEmpty()) {
				command+=@$"(statementprod.ProdType={POut.Int((int)ProductionType.Procedure)}
					AND FKey IN({string.Join(",",listProcNums.Select(x => POut.Long(x)).ToList())})) ";
				isOrStatementNeeded=true;
			}
			if(!listAdjNums.IsNullOrEmpty()) {
				if(isOrStatementNeeded) {
					command+="OR ";
				}
				command+=@$"(statementprod.ProdType={POut.Int((int)ProductionType.Adjustment)}
					AND FKey IN({string.Join(",",listAdjNums.Select(x => POut.Long(x)).ToList())})) ";
				isOrStatementNeeded=true;
			}
			if(!listPayPlanChargeNums.IsNullOrEmpty()) {
				if(isOrStatementNeeded) {
					command+="OR ";
				}
				command+=@$"(statementprod.ProdType={POut.Int((int)ProductionType.PayPlanCharge)}
					AND FKey IN({string.Join(",",listPayPlanChargeNums.Select(x => POut.Long(x)).ToList())}))";
			}
			Db.NonQ(command);
		}

		///<summary>Typically only used when setting StatementProd.LateChargeAdjNum back to 0 when the late charge Adjustment is deleted.</summary>
		public static void UpdateLateChargeAdjNumForMany(long adjNumNew,params long[] arrayOldAdjNums) {
			if(arrayOldAdjNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adjNumNew,arrayOldAdjNums);
				return;
			}
			string command=$@"UPDATE statementprod SET statementprod.LateChargeAdjNum={POut.Long(adjNumNew)}
				WHERE statementprod.LateChargeAdjNum IN ({string.Join(",",arrayOldAdjNums.Select(x => POut.Long(x)).ToList())})";
			Db.NonQ(command);
		}
		#endregion Modification Methods
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		///<summary>Gets one StatementProd from the db.</summary>
		public static StatementProd GetOne(long statementProdNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<StatementProd>(MethodBase.GetCurrentMethod(),statementProdNum);
			}
			return Crud.StatementProdCrud.SelectOne(statementProdNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(StatementProd statementProd){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				statementProd.StatementProdNum=Meth.GetLong(MethodBase.GetCurrentMethod(),statementProd);
				return statementProd.StatementProdNum;
			}
			return Crud.StatementProdCrud.Insert(statementProd);
		}
		///<summary></summary>
		public static void Update(StatementProd statementProd){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementProd);
				return;
			}
			Crud.StatementProdCrud.Update(statementProd);
		}
		///<summary></summary>
		public static void Delete(long statementProdNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementProdNum);
				return;
			}
			Crud.StatementProdCrud.Delete(statementProdNum);
		}
		#endregion Methods - Modify
		*/

	}

	///<summary>Used to hold all of the statementprods from the DB for a group of statements to facilitate synchronizatoin between new StatementProds for the group of statements and the StatementProds that already exist for them in the DB.</summary>
	public class StatementProdSyncData {
		///<summary>Key is ProcNum. Only used when syncing StatementProds for statements.</summary>
		private Dictionary<long,StatementProd> DictStatementProdProcs=new Dictionary<long, StatementProd>();
		///<summary>Key is AdjNum. Only used when syncing StatementProds for statements.</summary>
		private Dictionary<long,StatementProd> DictStatementProdAdjs=new Dictionary<long, StatementProd>();
		///<summary>Key is PayPlanChargeNum. Only used when syncing StatementProds for statements.</summary>
		private Dictionary<long,StatementProd> DictStatementProdPayPlanCharges=new Dictionary<long, StatementProd>();

		///<summary></summary>
		public StatementProdSyncData() {
		}

		///<summary>Returns a dictionary of StatementProdSyncData sets for each StatementNum. The key is StatementNum.</summary>
		public static Dictionary<long,StatementProdSyncData> GetDictSyncData(List<long> listStatementNums) {
			//No need to check RemotingRole; no call to db.
			Dictionary<long,StatementProdSyncData> dictSyncData=new Dictionary<long,StatementProdSyncData>();
			List<StatementProd> listStatementProds=StatementProds.GetManyForStatements(listStatementNums);
			for(int i=0;i<listStatementProds.Count;i++) {
				StatementProd statementProdCur=listStatementProds[i];
				if(!dictSyncData.TryGetValue(statementProdCur.StatementNum,out StatementProdSyncData syncDataCur)) {
					//Dictionary entry for this StatementProds statement does not exist yet, so add it.
					syncDataCur=new StatementProdSyncData();
					dictSyncData.Add(statementProdCur.StatementNum,syncDataCur);
				}
				switch(statementProdCur.ProdType) {
					case ProductionType.Procedure:
						syncDataCur.DictStatementProdProcs.Add(statementProdCur.FKey,statementProdCur);
						break;
					case ProductionType.Adjustment:
						syncDataCur.DictStatementProdAdjs.Add(statementProdCur.FKey,statementProdCur);
						break;
					case ProductionType.PayPlanCharge:
						syncDataCur.DictStatementProdPayPlanCharges.Add(statementProdCur.FKey,statementProdCur);
						break;
				}
			}
			return dictSyncData;
		}

		///<summary>Sets statementProd out param to null and returns false if it doesn't exist in any of the three dictionaries, otherwise returns true and sets the out param to a StatementProd from one of the three dictionaries.</summary>
		public bool TryGetStatementProd(long fKey,ProductionType prodType,out StatementProd statementProd) {
			//No need to check RemotingRole; no call to db.
			statementProd=null;
			switch(prodType) {
				case ProductionType.Procedure:
					DictStatementProdProcs.TryGetValue(fKey,out statementProd);
					break;
				case ProductionType.Adjustment:
					DictStatementProdAdjs.TryGetValue(fKey,out statementProd);
					break;
				case ProductionType.PayPlanCharge:
					DictStatementProdPayPlanCharges.TryGetValue(fKey,out statementProd);
					break;
			}
			if(statementProd==null) {
				return false;
			}
			else {
				return true;
			}
		}

		///<summary>Removes a StatementProd from the appropriate dictionary.</summary>
		public void RemoveStatementProd(long fKey,ProductionType prodType) {
			//No need to check RemotingRole; no call to db.
			switch(prodType) {
				case ProductionType.Procedure:
					DictStatementProdProcs.Remove(fKey);
					break;
				case ProductionType.Adjustment:
					DictStatementProdAdjs.Remove(fKey);
					break;
				case ProductionType.PayPlanCharge:
					DictStatementProdPayPlanCharges.Remove(fKey);
					break;
			}
		}

		///<summary>Places all the StatementProds of all ProductionTypes into one list and returns it.</summary>
		public List<StatementProd> GetListSatementProds() {
			//No need to check RemotingRole; no call to db.
			List<StatementProd> listStatementProds=new List<StatementProd>();
			listStatementProds.AddRange(DictStatementProdProcs.Values);
			listStatementProds.AddRange(DictStatementProdAdjs.Values);
			listStatementProds.AddRange(DictStatementProdPayPlanCharges.Values);
			return listStatementProds;
		}
	}

}