using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Used to store preferences specific to clinics.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class ClinicPref:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClinicPrefNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>Enum: </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PrefName PrefName;
		///<summary>The stored value.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ValueString;

		public ClinicPref() {
			
		}

		public ClinicPref(long clinicNum, PrefName prefName, bool valueBool) {
			this.ClinicNum=clinicNum;
			this.PrefName=prefName;
			this.ValueString=POut.Bool(valueBool);
		}

		public ClinicPref(long clinicNum, PrefName prefName, string valueString) {
			this.ClinicNum=clinicNum;
			this.PrefName=prefName;
			this.ValueString=valueString;
		}

		///<summary></summary>
		public ClinicPref Clone() {
			return (ClinicPref)this.MemberwiseClone();
		}

	}

	
}



