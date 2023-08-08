using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This stores log entries for debugging the orthochart.  Logging gets turned on and off with the pref. This table will go away once the bug is found. </summary>
	[Serializable]
	public class OrthoChartLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoChartLogNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary></summary>
		public string ComputerName;
		///<summary>DateTime that this log entry was made</summary>	
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeLog;
		///<summary>DateTime of the chart row.</summary>	
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeService;
		///<summary>FK to userod.UserNum.  The user that created or last edited an ortho chart field.</summary>
		public long UserNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to orthochartrow.OrthoChartRowNum.</summary>
		public long OrthoChartRowNum;
		///<summary>This can be long and complex -- whatever you want. MediumText, so max length=16M.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string LogData;
	}
}