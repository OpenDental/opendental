using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary>The two lists get refreshed the first time they are needed rather than at startup.</summary>
	public class Reconciles {
		///<summary></summary>
		public static Reconcile[] GetList(long accountNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Reconcile[]>(MethodBase.GetCurrentMethod(),accountNum);
			}
			string command="SELECT * FROM reconcile WHERE AccountNum="+POut.Long(accountNum)
				+" ORDER BY DateReconcile";
			return Crud.ReconcileCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets one reconcile directly from the database.  Program will crash if reconcile not found.</summary>
		public static Reconcile GetOne(long reconcileNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Reconcile>(MethodBase.GetCurrentMethod(),reconcileNum);
			}
			string command="SELECT * FROM reconcile WHERE ReconcileNum="+POut.Long(reconcileNum);
			return Crud.ReconcileCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(Reconcile reconcile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				reconcile.ReconcileNum=Meth.GetLong(MethodBase.GetCurrentMethod(),reconcile);
				return reconcile.ReconcileNum;
			}
			return Crud.ReconcileCrud.Insert(reconcile);
		}

		///<summary></summary>
		public static void Update(Reconcile reconcile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reconcile);
				return;
			}
			Crud.ReconcileCrud.Update(reconcile);
		}

		///<summary>Throws exception if Reconcile is in use.</summary>
		public static void Delete(Reconcile reconcile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reconcile);
				return;
			}
			//check to see if any journal entries are attached to this Reconcile
			string command="SELECT COUNT(*) FROM journalentry WHERE ReconcileNum="+POut.Long(reconcile.ReconcileNum);
			if(Db.GetCount(command)!="0"){
				throw new ApplicationException(Lans.g("FormReconcileEdit",
					"Not allowed to delete a Reconcile with existing journal entries."));
			}
			command="DELETE FROM reconcile WHERE ReconcileNum = "+POut.Long(reconcile.ReconcileNum);
			Db.NonQ(command);
		}
	}

}




