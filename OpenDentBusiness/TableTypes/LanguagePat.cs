using System;

namespace OpenDentBusiness {
	///<summary>Practice-defined translations for text shown to patients. Unlike Language and LanguageForeign, which translate Open Dental's interface to a practice's preferred language, LanguagePat allows practices to customize translations of messages and information displayed to patients in their preferred language. Used right now for about 30 prefs for things like email messages. Also used for EForms. So either PrefName will be empty or EFormFieldDefNum will be 0. Sheets are translated differently, by making a copy of each SheetFieldDef used for each language.</summary>
	[Serializable]
	public class LanguagePat:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LanguagePatNum;
		///<summary>FK to pref.PrefName. There are about 30 of these in use. This allows us to translate the value stored for templates like email, postcard, text, etc. Will be empty string if this is an eForm translation.></summary>
		public string PrefName;
		///<summary>Three-letter language name or custom language name.  The custom language name is the full string name and is not necessarily supported by Microsoft.
		///This will typically be matched to the patient's preferred language to select the appropriate translation.
		///<br>Three-letter language name examples: eng (English), spa (Spanish), fra (French).</br>
		///<br>Custom language name examples: Tahitian, American Sign Language, Morse Code.</br>
		///The LanguagesUsedByPatients preference stores the three-letter names that the practice chooses to support.
		///</summary>
		public string Language;
		///<summary>The translated text. Max 65,000 characters. Might store complex email templates.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Translation;
		///<summary>FK to eformfielddef.EFormFieldDefNum. This is how eForms get translated. Once a def is converted to an eForm, this is not needed. The eForm fields have all the translated text. Will be 0 if this is a pref translation.</summary>
		public long EFormFieldDefNum;
		
		///<summary></summary>
		public LanguagePat Copy(){
			return (LanguagePat)this.MemberwiseClone();
		}
	}
}
