using System;

namespace OpenDentBusiness {

	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class WebChatNote:TableBase {

		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebChatNoteNum;
		///<summary>FK to webchatsession.</summary>
		public long WebChatSessionNum;
		///<summary>When the note was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeNote;
		///<summary>Optional. The tech who created the note.
		///Name as string not userod.UserNum because the WebChat project does not have access to the customers database to look up user names.
		///Copied from userod.UserName.  OD enforces userod.UserName to be unique.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string TechName;
		///<summary>Optional. Note about the particular webchatsession.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText | CrudSpecialColType.CleanText)]
		public string Note;

		///<summary></summary>
		public WebChatNote Clone() {
			return (WebChatNote)this.MemberwiseClone();
		}
	}
}