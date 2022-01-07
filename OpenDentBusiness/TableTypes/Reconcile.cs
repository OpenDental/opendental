using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used in the Accounting section.  Each row represents one reconcile.  Transactions will be attached to it.</summary>
	[Serializable]
	public class Reconcile:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReconcileNum;
		///<summary>FK to account.AccountNum</summary>
		public long AccountNum;
		///<summary>User enters starting balance here.</summary>
		public double StartingBal;
		///<summary>User enters ending balance here.</summary>
		public double EndingBal;
		///<summary>The date that the reconcile was performed.</summary>
		public DateTime DateReconcile;
		///<summary>If StartingBal + sum of entries selected = EndingBal, then user can lock.  Unlock requires special permission, which nobody will have by default.</summary>
		public bool IsLocked;

		///<summary></summary>
		public Reconcile Copy() {
			Reconcile r=new Reconcile();
			r.ReconcileNum=ReconcileNum;
			r.AccountNum=AccountNum;
			r.StartingBal=StartingBal;
			r.EndingBal=EndingBal;
			r.DateReconcile=DateReconcile;
			r.IsLocked=IsLocked;
			return r;
		}


	}

	
}




