using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>This table is not part of the general release.  User would have to add it manually.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class PhoneMetric:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneMetricNum;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEntry;
		///<summary>Smallint -32768 to 32767. -1 means was unable to reach the server.</summary>
		public int VoiceMails;
		///<summary>Smallint -32768 to 32767</summary>
		public int Triages;
		///<summary>Smallint -32768 to 32767</summary>
		public int MinutesBehind;

		///<summary></summary>
		public PhoneEmpDefault Clone() {
			return (PhoneEmpDefault)this.MemberwiseClone();
		}
	}

	
}



/*
						CREATE TABLE phonemetric (
						PhoneMetricNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateTimeEntry datetime NOT NULL,
						VoiceMails smallint NOT NULL,
						Triages smallint NOT NULL,
						MinutesBehind smallint NOT NULL
						) DEFAULT CHARSET=utf8;
*/