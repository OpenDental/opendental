using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>A central table for all accural systems. </summary>
	[Serializable]
	public class OrthoCase:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoCaseNum;
		///<summary>FK to patient.PatNum.  The patient on this ortho case.</summary>
		public long PatNum;
		///<summary>FK to provider.ProvNum. </summary>
		public long ProvNum;
		///<summary>FK to clinic.ClinicNum. </summary>
		public long ClinicNum;
		///<summary>Total amount of procedure fees. Is editable by user.</summary>
		public double Fee;
		///<summary>The amount that primary insurance will cover for the entire ortho case.</summary>
		public double FeeInsPrimary;
		///<summary>Calculated from Fee - FeeIns. </summary>
		public double FeePat;
		///<summary>Date of Banding. </summary>
		public DateTime BandingDate;
		///<summary>Date of Debond. </summary>
		public DateTime DebondDate;
		///<summary>Date of expected Debond.</summary>
		public DateTime DebondDateExpected;
		///<summary>Used to denote that the banding date is used as the transfer date instead.</summary>
		public bool IsTransfer;
		///<summary>FK to definition.DefNum</summary>
		public long OrthoType;
		///<summary>DateTime ortho case was added. Not editable by user. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>FK to userod.usernum. The usernum that added the OrthoCase. </summary>
		public long SecUserNumEntry;
		///<summary>Timestamp of the last modification to the ortho case. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>Determines whether or not this is an active ortho case</summary>
		public bool IsActive;
		///<summary>The amount that secondary insurance will cover for the entire ortho case.
		///Will be set to zero if patient doesn't have secondary insurance</summary>
		public double FeeInsSecondary;


		///<summary></summary>
		public OrthoCase Copy(){
			return (OrthoCase)this.MemberwiseClone();
		}

	}
}