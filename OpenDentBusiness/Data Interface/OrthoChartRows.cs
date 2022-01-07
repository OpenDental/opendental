using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class OrthoChartRows{
		#region Methods - Get
		///<summary>Returns a list of all OrthoChartRows for the patnum passed in. Includes the list of orthochart rows by default.</summary>
		public static List<OrthoChartRow> GetAllForPatient(long patNum,bool doIncludeOrthoCharts=true){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChartRow>>(MethodBase.GetCurrentMethod(),patNum,doIncludeOrthoCharts);
			}
			string command="SELECT * FROM orthochartrow WHERE PatNum = "+POut.Long(patNum);
			List<OrthoChartRow> listOrthoChartRows=Crud.OrthoChartRowCrud.SelectMany(command);
			if(doIncludeOrthoCharts) {
				List<OrthoChart> listOrthoChartAll=OrthoCharts.GetByOrthoChartRowNums(listOrthoChartRows.Select(x => x.OrthoChartRowNum).ToList())
					.OrderBy(x => x.OrthoChartNum).ToList();
				for(int i=0;i<listOrthoChartRows.Count;i++) {
					List<OrthoChart> listOrthoChartCur=listOrthoChartAll.FindAll(x => x.OrthoChartRowNum==listOrthoChartRows[i].OrthoChartRowNum);
					if(listOrthoChartCur.IsNullOrEmpty()) {
						continue;
					}
					listOrthoChartRows[i].ListOrthoCharts=listOrthoChartCur;
				}
			}
			return listOrthoChartRows;
		}
		
		///<summary>Gets one OrthoChartRow from the db.</summary>
		public static OrthoChartRow GetOne(long orthoChartRowNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OrthoChartRow>(MethodBase.GetCurrentMethod(),orthoChartRowNum);
			}
			return Crud.OrthoChartRowCrud.SelectOne(orthoChartRowNum);
		}
		#endregion
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(OrthoChartRow orthoChartRow){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				orthoChartRow.OrthoChartRowNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoChartRow);
				return orthoChartRow.OrthoChartRowNum;
			}
			return Crud.OrthoChartRowCrud.Insert(orthoChartRow);
		}
		///<summary></summary>
		public static void Update(OrthoChartRow orthoChartRow){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartRow);
				return;
			}
			Crud.OrthoChartRowCrud.Update(orthoChartRow);
		}
		///<summary></summary>
		public static void Delete(long orthoChartRowNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartRowNum);
				return;
			}
			Crud.OrthoChartRowCrud.Delete(orthoChartRowNum);
		}
		#endregion
		#region Methods - Misc
		public static bool Sync(List<OrthoChartRow> listNew,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,patNum);
			}
			List<OrthoChartRow> listDB=GetAllForPatient(patNum,doIncludeOrthoCharts:false);
			//This code is just a straight copy of the Crud sync.  It's here in preparation for possibly adding logging for signature.
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<OrthoChartRow> listIns    =new List<OrthoChartRow>();
			List<OrthoChartRow> listUpdNew =new List<OrthoChartRow>();
			List<OrthoChartRow> listUpdDB  =new List<OrthoChartRow>();
			List<OrthoChartRow> listDel    =new List<OrthoChartRow>();
			listNew.Sort((OrthoChartRow x,OrthoChartRow y) => { return x.OrthoChartRowNum.CompareTo(y.OrthoChartRowNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((OrthoChartRow x,OrthoChartRow y) => { return x.OrthoChartRowNum.CompareTo(y.OrthoChartRowNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			OrthoChartRow fieldNew;
			OrthoChartRow fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.OrthoChartRowNum<fieldDB.OrthoChartRowNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.OrthoChartRowNum>fieldDB.OrthoChartRowNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				if(listIns[i].UserNum==0) {
					listIns[i].UserNum=Security.CurUser.UserNum;
				}
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Crud.OrthoChartRowCrud.Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
					//TODO add securitylog?
				}
			}
			Crud.OrthoChartRowCrud.DeleteMany(listDel.Select(x => x.OrthoChartRowNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}		
		#endregion
	}
}