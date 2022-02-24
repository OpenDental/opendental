using System;
using System.Collections;

namespace OpenDentBusiness {

	/// <summary>This defines the light buttons on the left of the main screen.</summary>
	[Serializable]
	public class SigButDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SigButDefNum;
		///<summary>The text on the button</summary>
		public string ButtonText;
		///<summary>0-based index defines the order of the buttons.</summary>
		public int ButtonIndex;
		///<summary>0=none, or 1-9. The cell in the 3x3 tic-tac-toe main program icon that is to be synched with this button.  It will light up or clear whenever this button lights or clears.</summary>
		public byte SynchIcon;
		///<summary>Blank for the default buttons.  Or contains the computer name for the buttons that override the defaults.</summary>
		public string ComputerName;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumUser;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumExtra;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumMsg;

		///<summary></summary>
		public SigButDef Copy() {
			return (SigButDef)this.MemberwiseClone();
		}

		
	}

		



		
	

	

	


}










