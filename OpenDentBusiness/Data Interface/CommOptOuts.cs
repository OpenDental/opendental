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
			return GetForPat(patNum)??new CommOptOut() { PatNum=patNum };
		}

		///<summary>Returns an entry from the db for the given patNum.</summary>
		public static CommOptOut GetForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<CommOptOut>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM commoptout WHERE PatNum = "+POut.Long(patNum);
			return Crud.CommOptOutCrud.SelectOne(command);
		}

		public static List<CommOptOut> GetForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<CommOptOut>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCommOptOuts);
				return;
			}
			Crud.CommOptOutCrud.InsertMany(listCommOptOuts);
		}

		public static void DeleteMany(List<CommOptOut> listCommOptOuts) {
			if(listCommOptOuts.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCommOptOuts);
				return;
			}
			string command="DELETE FROM commoptout WHERE CommOptOutNum IN("+string.Join(",",
				listCommOptOuts.Select(x => POut.Long(x.CommOptOutNum)))+")";
			Db.NonQ(command);
		}
		
		///<summary></summary>
		public static void Update(CommOptOut commOptOutNew,CommOptOut commOptOutOld){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),commOptOutNew,commOptOutOld);
				return;
			}
			Crud.CommOptOutCrud.Update(commOptOutNew,commOptOutOld);
		}

		public static void Upsert(CommOptOut commOptOut) {
			//This assures one-to-one with patient table.  Also safe for concurrent editing.
			CommOptOut commOptOutDb=commOptOut.PatNum>0 ? GetForPat(commOptOut.PatNum) : null;
			if(commOptOutDb is null) {
				InsertMany(new List<CommOptOut> { commOptOut });
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