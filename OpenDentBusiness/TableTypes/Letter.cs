using System;

namespace OpenDentBusiness{

	///<summary>These are templates that are used to send simple letters to patients.</summary>
	[Serializable]
	public class Letter:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LetterNum;
		///<summary>Description of the Letter.</summary>
		public string Description;
		///<summary>Text of the letter</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BodyText;

		public Letter Copy() {
			return (Letter)this.MemberwiseClone();
		}
	}
	
	
	

}













