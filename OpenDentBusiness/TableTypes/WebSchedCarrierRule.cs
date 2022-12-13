using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
  [Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class WebSchedCarrierRule:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebSchedCarrierRuleNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>Name of the carrier.</summary>
		public string CarrierName;
		///<summary>Set by the user. This is what is shown as a selection for the patient in the WebSched UI.</summary>
		public string DisplayName;
		///<summary>Return message sent back to patients through WebSched. Set by the office after a patient has made a carrier selection.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Message;
		///<summary>Enum:RuleType Allow, AllowWithInput, AllowWithMessage, Block.</summary>
		public RuleType Rule;

		public WebSchedCarrierRule Copy() {
			return (WebSchedCarrierRule)MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum RuleType {
		///<summary>0</summary>
		Allow,
		///<summary>1</summary>
		AllowWithInput,
		///<summary>2</summary>
		AllowWithMessage,
		///<summary>3</summary>
		Block,
	}
}
