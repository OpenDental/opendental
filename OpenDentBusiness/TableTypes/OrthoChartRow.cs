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
		///<summary>Examples: "0:ritwq/wV8vlrgUYahhK+RH5UeBFA6W4jCkZdo0cDWd63aZb1S/W3Z4eW5LmchqfgniG23" and "1:52222559445999975122111500485555". The 1st character is whether or not the signature is Topaz. The 2nd character is a separator. The rest of the string is the hashed signature data. Raw signature data is the concatenation of the FieldName and FieldValue of all cells (orthocharts), ordered by FieldName.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Signature;

		[CrudColumn(IsNotDbColumn=true)]
		///<summary>List of ortho chart cells that are associated to ortho chart row.</summary>
		public List<OrthoChart> ListOrthoCharts=new List<OrthoChart>();
	}
}
