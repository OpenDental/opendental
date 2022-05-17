using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>These are copies of procedures that are attached to saved treatment plans.  The ProcNumOrig points to the actual procedurelog row.</summary>
	[Serializable]
	[CrudTable(IsSecurityStamped=true)]
	public class ProcTP:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProcTPNum;
		///<summary>FK to treatplan.TreatPlanNum.  The treatment plan to which this proc is attached.</summary>
		public long TreatPlanNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to procedurelog.ProcNum.  It is very common for the referenced procedure to be missing.  This procNum is only here to compare and test the existence of the referenced procedure.  If present, it will check to see whether the procedure is still status TP.</summary>
		public long ProcNumOrig;
		///<summary>The order of this proc within its tp.  This is set when the tp is first created and can't be changed.  Drastically simplifies loading the tp.</summary>
		public int ItemOrder;
		///<summary>FK to definition.DefNum which contains the text of the priority.</summary>
		public long Priority;
		///<summary>A simple string displaying the tooth number.  If international tooth numbers are used, then this will be in international format already.  For Canadian users, using FDI nomenclature, we use 51 as a placeholder for supernumerary teeth, which is tooth number 99 according to CDHA standards (2/17/2014).  Logic for this is handled in the tooth logic class.</summary>
		public string ToothNumTP;
		///<summary>Tooth surfaces or area.  This is already converted for international use.  If arch or quad, then it will have U,LR, etc.</summary>
		public string Surf;
		///<summary>Not a foreign key.  Simply display text.  Can be changed by user at any time.</summary>
		public string ProcCode;
		///<summary>Description is originally copied from procedurecode.Descript, but user can change it.</summary>
		public string Descript;
		///<summary>The fee charged to the patient. Never gets automatically updated.</summary>
		public double FeeAmt;
		///<summary>The amount primary insurance is expected to pay. Never gets automatically updated.</summary>
		public double PriInsAmt;
		///<summary>The amount secondary insurance is expected to pay. Never gets automatically updated.</summary>
		public double SecInsAmt;
		///<summary>The amount the patient is expected to pay. Never gets automatically updated.</summary>
		public double PatAmt;
		///<summary>The amount of discount.  Used for PPOs and procedure level discounts.</summary>
		public double Discount;
		///<summary>Text from prognosis definition.  Can be changed by user at any time.</summary>
		public string Prognosis;
		///<summary>Text from diagnosis definition.  Can be changed by user at any time.</summary>
		public string Dx;
		///<summary>The ProcedureCode abbreviation.  Can be changed by user at any time.</summary>
		public string ProcAbbr;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>The amount primary insurance allows. Should be the exact amount in the FormClaimProc allowed amount field. May be either the PPO fee
		///or the out of network allowed fee.</summary>
		public double FeeAllowed;
		///<summary>Holds the Sales Tax estimate for this procedure.  Used to review history when being reviewed by accounting.
		///In the Treatment Plan, this represents an estimate and a record for pre-payments.</summary>
		public double TaxAmt;
		///<summary>FK to provider.ProvNum.  Holds the ProvNum for this procedure's provider.</summary>
		public long ProvNum;
		///<summary>Holds the DateTP for this procedure.</summary>
		public DateTime DateTP;
		///<summary>FK to clinic.ClinicNum.  Holds the ClinicNum for this procedure's clinic.</summary>
		public long ClinicNum;
		///<summary>The UCR fee for the procedure. Cannot be changed by the user.</summary>
		public double CatPercUCR;

		///<summary></summary>
		public ProcTP Copy(){
			return (ProcTP)MemberwiseClone();
		}

	
	}

	

	


}




















