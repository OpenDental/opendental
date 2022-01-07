using System;

namespace OpenDentBusiness {

	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class WebChatSurvey:TableBase {

		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebChatSurveyNum;
		///<summary>FK to webchatsession.WebChatSessionNum</summary>
		public long WebChatSessionNum;
		///<summary>Overall rating for technician for the session.</summary>
		public TechSurveyRating TechRating;
		///<summary>Comments from customers.  Similar CRUD special type to commlog notes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string CustomerComment;
		///<summary>Experience notes from customers.  Similar CRUD special type to commlog notes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string CustomerExperience;

		///<summary></summary>
		public WebChatSession Clone() {
			return (WebChatSession)this.MemberwiseClone();
		}
	}

	public enum TechSurveyRating {
		///<summary>0</summary>
		Neutral,
		///<summary>1</summary>
		Positive,
		///<summary>2</summary>
		Negative,
	}
}