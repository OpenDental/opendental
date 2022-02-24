using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>This is a list of phrases that need to be translated.  The primary key is a combination of the ClassType and the English phrase.  This table is currently filled dynmically at run time, but the plan is to fill it using a tool that parses the code.</summary>
	[Serializable]
	public class Language:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LanguageNum;
		///<summary>No longer used.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string EnglishComments;
		///<summary>A string representing the class where the translation is used.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ClassType;
		///<summary>The English version of the phrase, case sensitive.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string English;
		///<summary>As this gets more sophisticated, we will use this field to mark some phrases obsolete instead of just deleting them outright.  That way, translators will still have access to them.  For now, this is not used at all.</summary>
		public bool IsObsolete;

		public Language Copy() {
			return (Language)this.MemberwiseClone();
		}
	}




	

	

}













