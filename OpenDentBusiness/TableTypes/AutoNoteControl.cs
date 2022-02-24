using System;
using System.Collections.Generic;
using System.Text;
namespace OpenDentBusiness {

	///<summary>In the program, this is now called an autonote prompt.</summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class AutoNoteControl:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutoNoteControlNum;
		///<summary>The description of the prompt as it will be referred to from other windows.</summary>
		public string Descript;
		///<summary>'Text', 'OneResponse', or 'MultiResponse'.  More types to be added later.</summary>
		public string ControlType;
		///<summary>The prompt text.</summary>
		public string ControlLabel;
		///<summary>For TextBox, this is the default text.  For a ComboBox, this is the list of possible responses, one per line.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ControlOptions;
		

		///<summary></summary>
		public AutoNoteControl Copy() {
			return (AutoNoteControl)this.MemberwiseClone();
		}
	}
}
