using System;

namespace OpenDentBusiness{

	///<summary>Some program links (bridges), have properties that need to be set.  The property names are always hard coded.  User can change the value.  The property is usually retrieved based on its name.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true,HasBatchWriteMethods=true)]
	public class ProgramProperty:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProgramPropertyNum;
		///<summary>FK to program.ProgramNum</summary>
		public long ProgramNum;
		///<summary>The description or prompt for this property.  Blank for workstation overrides of program path.
		///Many bridges use this description as an "internal description". This way it can act like a FK in order to look up this particular property.  Users cannot edit.</summary>
		public string PropertyDesc;
		///<summary>The value.  </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PropertyValue;
		///<summary>The human-readable name of the computer on the network (not the IP address).  Only used when overriding program path.  Blank for typical Program Properties.</summary>
		public string ComputerName;
		///<summary>FK to clinic.ClinicNum.  This is only used by a few bridges.  Set to 0 for most bridges.</summary>
		public long ClinicNum;
		///<summary>Is true if the program property is sensitive information that would need to be masked in the UI. False by default.</summary>
		public bool IsMasked;

		///<summary>Returns a copy of this program property.</summary>
		public ProgramProperty Copy() {
			return (ProgramProperty)MemberwiseClone();
		}
	}


}










