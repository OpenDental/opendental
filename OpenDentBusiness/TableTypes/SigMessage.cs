using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	public class SigMessage:TableBase,IComparable {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SigMessageNum;
		///<summary>The text on the button</summary>
		public string ButtonText;
		///<summary>0-based index defines the order of the buttons.</summary>
		public int ButtonIndex;
		///<summary>0=none, or 1-9. The cell in the 3x3 tic-tac-toe main program icon that is to be synched with this button.
		///It will light up or clear whenever this button lights or clears.</summary>
		public byte SynchIcon;
		///<summary>Text version of 'user' this message was sent from, which can actually be any description of a group or individual.</summary>
		public string FromUser;
		///<summary>Text version of 'user' this message was sent to, which can actually be any description of a group or individual.</summary>
		public string ToUser;
		///<summary>Automatically set to the date and time upon insert.  Uses server time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime MessageDateTime;
		///<summary>This date time will get set as soon as this message has been acknowledged.  How lights get turned off.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime AckDateTime;
		///<summary>The text that shows for the element, like the user name or the two word message.  No long text is stored here.</summary>
		public string SigText;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumUser;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumExtra;
		///<summary>FK to sigelementdef.SigElementDefNum</summary>
		public long SigElementDefNumMsg;

		///<summary></summary>
		public SigMessage Copy() {
			return (SigMessage)this.MemberwiseClone();
		}

		///<summary>IComparable.CompareTo implementation.  This is used to order sigmessages.
		///This is needed because ordering SigMessages is too complex to do with a query.</summary>
		public int CompareTo(object obj) {
			if(!(obj is SigMessage)) {
				throw new ArgumentException("object is not a SigMessage");
			}
			SigMessage sig=(SigMessage)obj;
			DateTime date1;
			DateTime date2;
			if(AckDateTime.Year < 1880) {//if not acknowledged
				date1=MessageDateTime;
			}
			else {
				date1=AckDateTime;
			}
			if(sig.AckDateTime.Year < 1880) {//if not acknowledged
				date2=sig.MessageDateTime;
			}
			else {
				date2=sig.AckDateTime;
			}
			return date1.CompareTo(date2);
		}
	}
}
