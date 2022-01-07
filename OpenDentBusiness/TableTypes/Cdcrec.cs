using System;
//using System.Collections;
//using System.Drawing;

namespace OpenDentBusiness {
	///<summary>CDC Race and Ethnicity.  About 200 rows.  This table is not used anywhere right now.</summary>
	[Serializable]
	public class Cdcrec:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CdcrecNum;
		///<summary>CDCREC Code.  Example: 1002-5.  Not allowed to edit this column once saved in the database.</summary>
		public string CdcrecCode;
		///<summary>Heirarchical Code. Example:
		///<para>R1       =="American Indian or alaska Native"</para>
		///<para>R1.01    =="American Indian"</para>
		///<para>R1.01.001=="Abenaki"</para>
		///<para>Not allowed to edit this column once saved in the database. </para></summary>
		public string HeirarchicalCode;
		///<summary>Description.</summary>
		public string Description;

		///<summary></summary>
		public Cdcrec Copy() {
			return (Cdcrec)this.MemberwiseClone();
		}

	}
}