using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CommOptOuts{
		///<summary>Returns an entry from the db, or a new instance(not in db yet) if not found.</summary>
		public static CommOptOut Refresh(long patNum) {
			//No need to check MiddleTierRole; no call to db.
			CommOptOut commOptOut=GetForPat(patNum);
			if(commOptOut!=null){
				return commOptOut;
			}
			commOptOut=new CommOptOut();
			commOptOut.PatNum=patNum;
			return commOptOut;
		}

		///<summary>Returns an entry from the db for the given patNum.</summary>
		public static CommOptOut GetForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CommOptOut>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM commoptout WHERE PatNum = "+POut.Long(patNum);
			return Crud.CommOptOutCrud.SelectOne(command);
		}

		public static List<CommOptOut> GetForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<CommOptOut>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CommOptOut>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM commoptout WHERE PatNum IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") ";
			return Crud.CommOptOutCrud.SelectMany(command);
		}


		///<summary></summary>
		public static void InsertMany(List<CommOptOut> listCommOptOuts) {
			if(listCommOptOuts.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCommOptOuts);
				return;
			}
			Crud.CommOptOutCrud.InsertMany(listCommOptOuts);
		}

		public static void DeleteMany(List<CommOptOut> listCommOptOuts) {
			if(listCommOptOuts.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCommOptOuts);
				return;
			}
			string command="DELETE FROM commoptout WHERE CommOptOutNum IN("+string.Join(",",
				listCommOptOuts.Select(x => POut.Long(x.CommOptOutNum)))+")";
			Db.NonQ(command);
		}
		
		///<summary></summary>
		public static void Update(CommOptOut commOptOutNew,CommOptOut commOptOutOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),commOptOutNew,commOptOutOld);
				return;
			}
			Crud.CommOptOutCrud.Update(commOptOutNew,commOptOutOld);
		}

		public static void Upsert(CommOptOut commOptOut) {
			//No need to check MiddleTierRole; no call to db.
			//This assures one-to-one with patient table.  Also safe for concurrent editing.
			CommOptOut commOptOutDb=null;
			if(commOptOut.PatNum>0){
				commOptOutDb=GetForPat(commOptOut.PatNum);
			}
			if(commOptOutDb is null) {
				List<CommOptOut> listCommOptOuts=new List<CommOptOut>();
				listCommOptOuts.Add(commOptOut);
				InsertMany(listCommOptOuts);
				return;
			}
			Update(commOptOut,commOptOutDb);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		#endregion
		#region Modification Methods
			#region Insert
			#endregion
			#region Update
		
			#endregion
			#region Delete
		///<summary></summary>
			#endregion
		#endregion
		#region Misc Methods
		

		
		#endregion
		*/
	}
}