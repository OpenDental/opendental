using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Represent a row in the ortho chart UI grid.</summary>
	[Serializable()]
	public class OrthoChartRow:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoChartRowNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>DateTime of service.</summary>	
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeService;
		///<summary>FK to userod.UserNum.  The user that created or last edited an ortho chart field.</summary>
		public long UserNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>Signature.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Signature;

		[CrudColumn(IsNotDbColumn=true)]
		///<summary>List of ortho chart cells that are associated to ortho chart row.</summary>
		public List<OrthoChart> ListOrthoCharts=new List<OrthoChart>();
	}
}
