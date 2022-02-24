using System;

namespace OpenDentBusiness{
	//Nathan and Mark discussed this and determined it doesn't matter if rows exist for completed/deleted procs between the time when a proc is completed/deleted and the modules are
	//visited to run the sync method.  07/14/2021 - see task #3681792

	///<summary>Links active and inactive treatment plans to procedurelog rows. When the treatment plan or chart modules are selected, any treatplanattach rows that are linked to
	///completed or deleted procedures will be deleted.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class TreatPlanAttach:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TreatPlanAttachNum;
		///<summary>FK to treatplan.TreatPlanNum.</summary>
		public long TreatPlanNum;
		///<summary>FK to procedurelog.ProcNum.</summary>
		public long ProcNum;
		///<summary>FK to definition.DefNum, which contains the text of the priority. Identical to Procedure.Priority but used to allow different priorities
		/// for the same procedure depending on which TP it is a part of.</summary>
		public long Priority;

		///<summary></summary>
		public TreatPlanAttach Copy() {
			return (TreatPlanAttach)this.MemberwiseClone();
		}

	}

	

	


}