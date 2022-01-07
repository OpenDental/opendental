using System;

namespace OpenDentBusiness{

	///<summary>A list of query favorites that users can run.</summary>
	[Serializable]
	public class UserQuery:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long QueryNum;
		///<summary>Description.</summary>
		public string Description;
		///<summary>The name of the file to export to.</summary>
		public string FileName;
		///<summary>The text of the query.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string QueryText;
		///<summary>Determines whether the query is safe for users with lower permissions.  Also causes this user query to be available in the Main Menu, Reports, Query Favorites Filtered.</summary>
		public bool IsReleased;
		///<summary>Determines whether the Query Favorites window should prompt for query values via FormQueryParser/'SET Fields' popup when running query.</summary>
		public bool IsPromptSetup;

		public UserQuery Copy() {
			return (UserQuery)this.MemberwiseClone();
		}
	}





}













