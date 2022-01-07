using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	/// <summary>Used to attach procedures to an OrthoCase.</summary>
	[Serializable]
	public class OrthoProcLink:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoProcLinkNum;
		///<summary>FK to orthocase.OrthoCaseNum.  </summary>
		public long OrthoCaseNum;
		///<summary>FK to procedurelog.ProcNum </summary>
		public long ProcNum;
		///<summary>DateTime proclink was added. Not editable by user. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>FK to userod.UserNum. User that added the proc link. </summary>
		public long SecUserNumEntry;
		///<summary>Enum:OrthoProcType Indicates what type of procedure is being associated to Ortho Case in link.</summary>
		public OrthoProcType ProcLinkType;

		///<summary></summary>
		public OrthoProcLink Copy(){
			return (OrthoProcLink)this.MemberwiseClone();
		}

	}

	///<summary>A procedures type as it relates to an Ortho Case</summary>
	public enum OrthoProcType {
		///<summary>0 - Procedure for putting appliance on.</summary>
		Banding,
		///<summary>1 - Procedure for removing appliance.</summary>
		Debond,
		///<summary>2 - All maintenance visits between Banding and Debond procedures.</summary>
		Visit,
	}
}
