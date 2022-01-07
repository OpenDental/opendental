using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>An actual signal that gets sent out as part of the messaging functionality.</summary>
	[Serializable]
	public class Signal:TableBase,IComparable{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SignalNum;
		///<summary>Text version of 'user' this message was sent from, which can actually be any description of a group or individual.</summary>
		public string FromUser;
		///<summary>Enum:InvalidType List of InvalidType long values separated by commas.  Can be empty.  When Date or Tasks are used, they are used all alone with no other flags present.</summary>
		public string ITypes;
		///<summary>If IType=Date, then this is the affected date in the Appointments module.</summary>
		public DateTime DateViewing;
		///<summary>Enum:SignalType  Button, or Invalid.</summary>
		public SignalType SigType;
		///<summary>This is only used if the type is button, and the user types in some text.  This is the typed portion and does not include any of the text that was on the buttons.  These types of signals are displayed in their own separate list in addition to any light and sound that they may cause.</summary>
		public string SigText;
		///<summary>The exact server time when this signal was entered into db.  This does not need to be set by sender since it's handled automatically.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime SigDateTime;
		///<summary>Text version of 'user' this message was sent to, which can actually be any description of a group or individual.</summary>
		public string ToUser;
		///<summary>If this signal has been acknowledged, then this will contain the date and time.  This is how lights get turned off also.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime AckTime;
		///<summary>FK to task.TaskNum.  If IType=Tasks, then this is the taskNum that was added.</summary>
		public long TaskNum;
		///<summary>Not a database field.  The sounds and lights attached to the signal.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public SigElement[] ElementList;

		///<summary>IComparable.CompareTo implementation.  This is used to order signals.  This is needed because ordering signals is too complex to do with a query.</summary>
		public int CompareTo(object obj) {
			if(!(obj is Signal)) {
				throw new ArgumentException("object is not a Signal");
			}
			Signal sig=(Signal)obj;
			DateTime date1;
			DateTime date2;
			if(AckTime.Year<1880){//if not acknowledged
				date1=SigDateTime;
			}
			else{
				date1=AckTime;
			}
			if(sig.AckTime.Year<1880) {//if not acknowledged
				date2=sig.SigDateTime;
			}
			else {
				date2=sig.AckTime;
			}
			return date1.CompareTo(date2);
		}
		
		///<summary></summary>
		public Signal Copy(){
			Signal s=(Signal)this.MemberwiseClone();
			s.ElementList=new SigElement[ElementList.Length];
			ElementList.CopyTo(s.ElementList,0);
			return s;
		}

	
	}

	

	


}




















