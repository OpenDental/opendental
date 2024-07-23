using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Individual fields for the EForm. Each field generally includes a label and a value. Links to a EFormDef by FKey to eformdef.EFormDefNum.
	///NOTE: If any new fields get added to this class and EFormField, make sure to add them to the methods EFormFields.FromDef and EFormFields.ToDef</summary>
	[Serializable]
	public class EFormFieldDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EFormFieldDefNum;
		///<summary>FKey to eformdef.EFormDefNum</summary>
		public long EFormDefNum;
		///<summary>Enum:EnumEFormFieldType 0-TextField, 1-Label, 2-CheckBox, etc.</summary>
		public EnumEFormFieldType FieldType;
		///<summary>If this field is importable, then this links to a db field. The list of available fields for each type is in EFormFieldsAvailable. Users can pick from that list. Identical list as in Sheets. It's string-based instead of enum, just like Sheets, because it's too complex to use an enum, even for our reduced number of items. None is always represented in UI as "None" and in db as empty string. All DbLinks are available on all form types to give users more flexibility. Checkboxes can have DBLinks that look like "allergy:...", "med:...", or "problem:..."</summary>
		public string DbLink;
		///<summary>Used differently for different types:
		///<para>TextField, DateField, CheckBox: The label next to or above the textbox, or checkbox.</para>
		///<para>RadioButtons: The label above the group of radiobuttons. Labels on each radiobutton are in SelectionList.</para>
		///<para>Label: This label is the only thing that shows. A label is always a WPF FlowDocument, which is an XML format. This allows extensive rich text formatting, like bold, color, paragraph formatting, etc. This format can be used directly in OD proper, but it will need to be converted for some other languages using external tools. BUT, prior to that, it must be run through a method that adjusts all the font sizes. FlowDocuments only support absolute font sizes instead of relative font sizes. We use 11.5 as the base font size and all other fonts are considered to be relative to this base. So if a font size of 13.8 is present in the FlowDocument, that does not mean to use 13.8; it instead means to use 120%. If your chosen base font size on a mobile device is 16, then the conversion method needs to convert the 13.8 to 19.2 prior to using the FlowDocument.</para>
		///<para>PageBreak: Not used.</para>
		///<para>SigBox: Optional label above sig box.</para>
		///<para>MedicationList: This holds an EFormMedListLayout object, serialized as json, including the Title, column headers, column widths, etc.</para>
		///</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string ValueLabel;
		///<summary>0 based.</summary>
		public int ItemOrder;
		///<summary>Comma delimited list of strings, used for radioButtons, future comboBoxes, etc. This is the list of items that are visible to the patient. Setup enforces same number of items in PickListDb for 1:1 match. This list allows customization of what the patient sees vs what's in the db. Example: Vis=Hispanic, Db=2135-2. Example: Vis=Do Not Call, Db=DoNotCall. For radiobuttons, the number of items in the lists determines the number of radiobuttons to show to the patient. These editable lists also allow excluding some db options from being visible to patient. Example: Ins Relationship has 9 options, but only 4 of them are really used in dentistry. Just leave the other 5 off and force them to pick one of the 4. But it is also not required for them to pick one. Example: For Marital Status, you might only show Married and Child, excluding Divorced and Single from the pick list. The unselected state then represents no change, so an existing patient could leave both radio buttons unchecked and their status would remain Divorced or Single. However, we currently lack a feature to let them uncheck a radiobutton that is already checked. This is a rare edge case that nearly nobody will care about. You can also have a row with no db value. For example, a visible value of Separated might have no corresponding db value entered. In that case, an import would not cause any change to the existing db value. These lists also allow two radioButtons to represent one db item. Example: Gender Other in db can be expanded to show patient both Nonbinary and Other. When patient picks either of these, it goes into the db as Other. The lists also allow any or all items to be empty with no label. Example: Y/N radiobuttons for a series of allergies. Y/N label at top, but none of the radiobuttons need labels. When translation is added later, it will translate this list, not the PickListDb. PickListVis will, by default, simply be exactly the same as PickListDb. In this state, what the patient sees is the same as what's in the db. Must have at least two items for now. </summary>
		public string PickListVis;
		///<summary>Comma delimited list of strings, used for radioButtons, future comboBoxes, etc. This is the list of items as they would be stored in the database. See PickListVis above for examples of how to use. The value chosen from this list is what will be stored in the ValueString field. Never show this value to the patient.</summary>
		public string PickListDb;
		///<summary>Typically false. Set to true to cause this field to get stacked horizontally compared to its previous sibling. Example might be to set State and Zip fields to true. This request will be ignored if screen is too small, like on a phone. We don't allow this option for RadioButtons because they already stack horizontally, and that would be confusing. The following types are not allowed to stack: RadioButtons, SigBox, PageBreak, MedicationList.</summary>
		public bool IsHorizStacking;
		///<summary>Only applies when this is a TextField. Default is false, which creates a single row textbox that scrolls horizontally if text is too long. Set to true to cause text to wrap instead. This will cause the box to grow to fit the text.</summary>
		public bool IsTextWrap;
		///<summary>If this is blank/0, then width will be 100% of what's available. If fields are stacked horizontally, then they will wrap when they hit screen width. So horizontally stacked fields may end up vertically stacked on a small screen. But if a single field is still set to be wider than the current screen, it will shrink to fit the screen. This width uses WPF DIPs which are 1/96". For Android phones that use 1/160" per DIP, we must scale it while taking into account the font size used for 100% FontScale. So assuming we use 16 Android DIPs for 100% font vs 11.5 in WPF, the conversion would look like this: Width/11.5*16. Notice that we are only converting based on font size. This makes our converted width a near perfect fit for the same text as the original.</summary>
		public int Width;
		///<summary>Applies to both the label on the field and the field itself. Never 0. Does not apply to Label types, though, since those are only handled by editing the rich text. Always has a valid value between 50 and 300. Default is 100, indicating normal size. WPF defines a DIP as 1/96". Open Dental uses 11.5 DIPs for nearly all fonts on desktop version. Old Microsoft font sizes were based on 1/72", so 11.5 converts to old 8.6. Android defines a DIP as 1/160". Typical recommended font size on Android seems to be about 16, which translates to 9.6 MS DIPs or 7.2 old Windows font. In other words, recommended phone fonts are physically slightly smaller than desktop fonts. EForms uses font sizes based on 100% being a standard normal size. 100% equates to 11.5 on desktop, probably about 16 on Android phones, and whatever our engineers come up with for tablets. By doing it this way, we do not have to explain anything complicated to users, and they also have very good control over font sizes.</summary>
		public int FontScale;
		///<summary>False by default. If this is set to true, the patient will be required to fill out the field. If conditional logic causes a required field to not show, it will not enforce the requirement.</summary>
		public bool IsRequired;
		///<summary>This string is the label of the field that acts as the parent for conditional logic. Empty string by default indicates no parent. Truncated to the first 255 characters. </summary>
		public string ConditionalParent;
		///<summary>When this field has a conditional parent, it will only show if the value of this field matches the value of the parent. For radio buttons, it matches the value of one of the radiobuttons. For checkboxes, a match is "X". If radiobutton text is too long, it matches the first 255 characters.</summary>
		public string ConditionalValue;

		public EFormFieldDef(){
			//We frequently create these from scratch instead of from db.
			//We don't want these to be null.
			DbLink="";
			ValueLabel="";
			PickListVis="";
			PickListDb="";
			ConditionalParent="";
			ConditionalValue="";
		}

		public EFormFieldDef Copy() {
			return (EFormFieldDef)this.MemberwiseClone();
		}		
	}
}

