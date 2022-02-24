using EhrLaboratories;
using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, May either be a note attached to an EhrLab or an EhrLabResult.  NTE.*</summary>
	[Serializable]
	public class EhrLabNote:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabNoteNum;
		///<summary>FK to ehrlab.EhrLabNum.  Should never be zero.</summary>
		public long EhrLabNum;
		///<summary>FK to ehrlabresult.EhrLabResult.  May be 0 if this is a Lab Note, will be valued if this is an Ehr Lab Result Note.</summary>
		public long EhrLabResultNum;
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		///<summary>Carret delimited list of comments.  Comments must be formatted text and cannot contain the following 6 characters |^&amp;~\#  NTE.*.*</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Comments;


		///<summary></summary>
		public EhrLabNote Copy() {
			return (EhrLabNote)MemberwiseClone();
		}

		public EhrLabNote() {
			Comments="";
		}

	}

}