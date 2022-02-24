using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Will usually only contain translations for a single foreign language, although more are allowed.  The primary key is a combination of the ClassType and the English phrase and the culture.</summary>
	[Serializable]
	public class LanguageForeign:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LanguageForeignNum;
		///<summary>A string representing the class where the translation is used.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ClassType;
		///<summary>The English version of the phrase.  Case sensitive.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string English;
		///<summary>The specific culture name.  Almost always in 5 digit format like this: en-US.</summary>
		public string Culture;
		///<summary>The foreign translation.  Remember we use Unicode-8, so this translation can be in any language, including Russian, Hebrew, and Chinese.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Translation;
		///<summary>Comments for other translators for the foreign language.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Comments;

		///<summary></summary>
		public LanguageForeign Copy(){
			return (LanguageForeign)this.MemberwiseClone();
		}

		

	}

	

	

}













