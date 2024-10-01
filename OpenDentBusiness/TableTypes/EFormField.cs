using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Individual fields for EForm. Each field generally includes a label and a value. Links to a EForm by FKey to eform.EFormNum.
	///NOTE: If any new fields get added to this class and EFormFieldDef, make sure to add them to the methods EFormFields.FromDef and EFormFields.ToDef</summary>
	[Serializable]
	public class EFormField:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EFormFieldNum;
		///<summary>FKey to eform.EFormNum</summary>
		public long EFormNum;
		///<summary>FKey to patient.PatNum to let us quickly grab all for a patient, and then loop later.</summary>
		public long PatNum;
		///<summary>Enum:EFormFieldType 0-TextField, 1-Label, 2-DateField, etc.</summary>
		public EnumEFormFieldType FieldType;
		///<summary>If this field is importable, then this links to a db field. The list of available fields for each type is in EFormFieldsAvailable. Users can pick from that list. Identical list as in Sheets. It's string-based instead of enum, just like Sheets, because it's too complex to use an enum, even for our reduced number of items. None is always represented in UI as "None" and in db as empty string. All DbLinks are available on all form types to give users more flexibility. Checkboxes can have DBLinks that look like "allergy:..." or "problem:..."</summary>
		public string DbLink;
		///<summary>Used differently for different types:
		///<para>TextField, DateField, CheckBox: The label next to or above the textbox, or checkbox.</para>
		///<para>RadioButtons: This label next to or above the group of radiobuttons. Labels on each radiobutton are in PickListVis.</para>
		///<para>Label: This label is the only thing that shows. A label is always a WPF FlowDocument, which is an XML format. This allows extensive rich text formatting, like bold, color, paragraph formatting, etc. This format can be used directly in OD proper, but it will need to be converted for some other programming languages using external tools. BUT, prior to that, it must be run through a method that adjusts all the font sizes. FlowDocuments only support absolute font sizes instead of relative font sizes. We use 11.5 as the base font size and all other fonts are considered to be relative to this base. So if a font size of 13.8 is present in the FlowDocument, that does not mean to use 13.8; it instead means to use 120%. If your chosen base font size on a mobile device is 16, then the conversion method needs to convert the 13.8 to 19.2 prior to using the FlowDocument.</para>
		///<para>PageBreak: Not used.</para>
		///<para>SigBox: Optional label above sig box.</para>
		///<para>MedicationList: This holds an EFormMedListLayout object, serialized as json, including the Title, column headers, column widths, etc.</para>
		///</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string ValueLabel;
		///<summary>The data as entered by patient or pulled from the db. We do not need this in EFormFieldDef because that has no patient or db data. Used differently for different types:
		///<para>TextField: value in textBox. For allergiesOther, medsOther, and problemsOther, this is a comma-delimited list. Spaces by commas are ok. Like this: "Aspirin, Iodine, Latex"</para>
		///<para>Label: Not used because no patient input.</para>
		///<para>DateField: date in culture format, like 4/25/2024.</para>
		///<para>Checkbox: "X" or blank "".</para>
		///<para>RadioButton: String value chosen by patient. Pulled from PickListDb, not PickListVis. When importing, empty signifies that patient did not enter any choice, so do not import.</para>
		///<para>ComboBox (not yet added): String value chosen by patient.</para>
		///<para>SigBox: First char is 0 for our sigbox and 1 for Topaz. The remainder is the signature string. See OpenDentBusiness\UI\SignatureBoxWrapper.cs for details. Same format as used for all signatures in OD. Does not get imported.</para>
		///<para>MedicationList: This holds a list of EFormMed objects, serialized as json.</para>
		/// </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string ValueString;
		///<summary>0 based.</summary>
		public int ItemOrder;
		///<summary>Comma delimited list of strings, used for radioButtons, future comboBoxes, etc. This is the list of items that are visible to the patient. Setup enforces same number of items in PickListDb for 1:1 match. This list allows customization of what the patient sees vs what's in the db. Example: Vis=Hispanic, Db=2135-2. Example: Vis=Do Not Call, Db=DoNotCall. For radiobuttons, the number of items in the lists determines the number of radiobuttons to show to the patient. These editable lists also allow excluding some db options from being visible to patient. Example: Ins Relationship has 9 options, but only 4 of them are really used in dentistry. Just leave the other 5 off and force them to pick one of the 4. But it is also not required for them to pick one. Example: For Marital Status, you might only show Married and Child, excluding Divorced and Single from the pick list. The unselected state then represents no change, so an existing patient could leave both radio buttons unchecked and their status would remain Divorced or Single. However, we currently lack a feature to let them uncheck a radiobutton that is already checked. This is a rare edge case that nearly nobody will care about. You can also have a row with no db value. For example, a visible value of Separated might have no corresponding db value entered. In that case, an import would not cause any change to the existing db value. These lists also allow two radioButtons to represent one db item. Example: Gender Other in db can be expanded to show patient both Nonbinary and Other. When patient picks either of these, it goes into the db as Other. The lists also allow any or all items to be empty with no label. Example: Y/N radiobuttons for a series of allergies. Y/N label at top, but none of the radiobuttons need labels. When translation is added later, it will translate this list, not the PickListDb. PickListVis will, by default, simply be exactly the same as PickListDb. In this state, what the patient sees is the same as what's in the db. Must have at least two items for now.</summary>
		public string PickListVis;
		///<summary>Comma delimited list of strings, used for radioButtons, future comboBoxes, etc. This is the list of items as they would be stored in the database. See PickListVis above for examples of how to use. The value chosen from this list is what will be stored in the ValueString field. Never show this value to the patient.</summary>
		public string PickListDb;
		///<summary>Typically false. Set to true to cause this field to get stacked horizontally compared to its previous sibling. Example might be to set State and Zip fields to true. This request will be ignored if screen is too small, like on a phone. The following types are not allowed to stack: RadioButtons, SigBox, PageBreak, MedicationList.</summary>
		public bool IsHorizStacking;
		///<summary>Only applies when this is a TextField. Default is false, which creates a single row textbox that scrolls horizontally if text is too long. Set to true to cause text to wrap instead. This will cause the box to grow to fit the text.</summary>
		public bool IsTextWrap;
		///<summary>This stores either pixel width or percentage width, depending on IsWidthPercentage. In either case, if this is blank/0, then width will be 100% of what's available. The discussion here is for fixed widths. See IsWidthPercentage for discussion of percentage widths. If fields are stacked horizontally, then they will wrap when they hit screen width. So horizontally stacked fields may end up vertically stacked on a small screen. But if a single field is still set to be wider than the current screen, it will shrink to fit the screen. This width uses WPF DIPs which are 1/96". Android phones define DIPs differently; they use 1/160" per DIP. But if you are using a language like Flutter, they are handling that conversion for you in the background. Regardless, we will be ignoring DIPs and scaling based solely on font size. The reason for this is to make fonts and boxes all look proportionally the same on both OD proper and in eClipboard. So assuming you use 14 flutter logical pixels for 100% font vs 11.5 in WPF, the conversion would look like this: Width/11.5*14. Notice that we are only converting based on font size. This makes our converted width a near perfect fit for the same text as the original. Width is only available on the field types that are h-stackable. Width is available on RadioButtons, but it only refers to the width of the label which is left aligned in order to allow stacking of radioButtons like in allergies and problems.</summary>
		public int Width;
		///<summary>Applies to both the label on the field and the field itself. Never 0. Does not apply to Label types, though, since those are only handled by editing the rich text. Always has a valid value between 50 and 300. Default is 100, indicating normal size. WPF defines a DIP as 1/96". Open Dental uses 11.5 DIPs for nearly all fonts on desktop version. Old Microsoft font sizes were based on 1/72", so 11.5 converts to old 8.6. Android defines a DIP as 1/160". Typical recommended font size on Android seems to be about 16, which translates to 9.6 MS DIPs or 7.2 old Windows font. In other words, recommended phone fonts are physically slightly smaller than desktop fonts. EForms uses font sizes based on 100% being a standard normal size. 100% equates to 11.5 on desktop, probably about 16 on Android phones, and whatever our engineers come up with for tablets. By doing it this way, we do not have to explain anything complicated to users, and they also have very good control over font sizes.</summary>
		public int FontScale;
		///<summary>False by default. If this is set to true, the patient will be required to fill out the field. If conditional logic causes a required field to not show, it will not enforce the requirement.</summary>
		public bool IsRequired;
		///<summary>This string is the label of the field that acts as the parent for conditional logic. Empty string by default indicates no parent. Truncated to the first 255 characters. </summary>
		public string ConditionalParent;
		///<summary>When this field has a conditional parent, it will only show if the value of this field matches the value of the parent. For radio buttons, it matches the value of one of the radiobuttons. It matches the value in PickListVis, but if there is a DbLink and the PickListVis is empty string, then it will match the value from the PickListDb. For checkboxes, a match is "X". If radiobutton text is too long, it matches the first 255 characters. For age conditions, it must start with greaterthan or lessthan. Example ">14".</summary>
		public string ConditionalValue;
		///<summary>Enum:EnumEFormLabelAlign 0-TopLeft, 1-LeftLeft, 2-Right. Only used in RadioButtons for now.</summary>
		public EnumEFormLabelAlign LabelAlign;
		///<summary>The amount of space below each field. Overrides the global default. -1 will indicate to use that default. That way, 0 means 0 space. If multiple fields are stacked horizontally, then only the right-most field can have this field set.</summary>
		public int SpaceBelow;
		///<summary>Allows reporting on fields that don't have DbLink.</summary>
		public string ReportableName;
		///<summary>If a field is locked, it stops a patient from editing the text when presented to them. Example is a consent form. Only available for TextField and CheckBox. This flag inherits from EFormFieldDef and there's no UI to change it once it's in EFormField. In FrmEFormFillEdit, this field has no effect because offices still need to be able to edit. So the field only has an effect when filling out by a patient on the web or in eClipboard. Patients can still use FrmEFormFillEdit, and in that case, it's assumed that office staff is watching to make sure they don't change the text.</summary>
		public bool IsLocked;
		///<summary>Enum:EnumEFormBorder 0-none, 1-3D. Shaded borders are optional on each field. They are on by default when most fields are added. But they don't make sense in some cases, like labels and stacks of Y/N radio buttons for allergies. When a border is present, any single row textbox inside it gets shown as a single underline instead of a rectangle. If the textbox has text wrapping turned on, it will always be a rectangle.</summary>
		public EnumEFormBorder Border;
		///<summary>False=DIPs / pixels at 96 dpi, True=Percentage. There is no mechanisms for "fill remainder" or "auto size to contents". There is no allowed mixing of fixed and percentage on the same row. Wrap won't happen until all columns have hit their MinWidth. If someone specifies percentages that add up to more than 100, that's ok. We will proportionally adapt. So in addition to expected percentages like 30-30-40, the user would get the same behavior by using 150-150-200. Let's use the example of 150-150-200 and assume MinWidths were 110-100-100. If available width was 400, then the widths would be 120-120-160. If available width was 330, then the widths would be 110-94-126, or (minWidth)-3/7-4/7. Below 310, they would start wrapping. If percentages add up to less than 100, then they might stop short of 100%. For example, 25-25-25 would come up short. They would continue to occupy 75% of available space until the space got so small that they started to hit their MinWidths. Let's assume MinWidths in that example were 50-100-100. If available width was 600, then the widths would be 150-150-150 (still 75%). If available width was 300, then the widths would be 75-100-100 (only 92%). Below 250, they would start wrapping.</summary>
		public bool IsWidthPercentage;
		///<summary>Only used with IsWidthPercentage.  If left blank/0, then no minimum width. A number might be present here but will be ignored if IsWidthPercentage is false.</summary>
		public int MinWidth;

		[CrudColumn(IsNotDbColumn=true)]
		public int Page;
		///<summary>This tells you if the field is hidden based on conditional logic. False by default. Set to true if the field has a parent field set for decision purposes. Has nothing to do with hiding pages.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsHiddenCondit;
		///<summary>This is only used when converting from fieldDefs to fields because we must preserve the PK.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public long EFormFieldDefNum;
		///<summary>During setup, we need to keep track of this because we don't take action on it until the user clicks Save. Although if the user just added it, we will immediately delete so that if they click Cancel, we don't end up with orphans.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsDeleted;

		public EFormField() {
			DbLink="";
			ValueLabel="";
			ValueString="";
			PickListVis="";
			PickListDb="";
			ConditionalParent="";
			ConditionalValue="";
			SpaceBelow=-1;
			ReportableName="";
		}

		public EFormField Copy(){
			return (EFormField)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EnumEFormFieldType {
		///<Summary>0-A textbox that the user can type into. Frequently tied to a database field. Can frequently be prefilled from database if desired. In Sheets, this was two different field types: InputField and OutputField.</Summary>
		TextField,
		///<Summary>1-This can be used for a label, heading, title, paragraph, etc. These also support the exact same replacement fields as in sheets StaticText. See the extensive comments on the ValueLabel field.</Summary>
		Label,
		///<summary>2-Some sort of textbox that's optimized for date input.</summary>
		[Description("Date")]
		DateField,
		///<summary>3-Simple checkbox that can be tied to a db field.</summary>
		[Description("Check Box")]
		CheckBox,
		///<summary>4-Not a single radiobutton, but a group of them.</summary>
		[Description("Radio Button Group")]
		RadioButtons,
		///<summary>5-A signature box, directly on the screen with stylus/mouse. Just the drawing, no encryption to tie it to the data yet.</summary>
		[Description("Signature")]
		SigBox,
		///<summary>6-.</summary>
		[Description("Page Break")]
		PageBreak,
		///<summary>7-A Medication List is a complex field. It consists of a list of medications with an optional second column for strength and frequency. Each medication has a Delete button to its right. There is also an Add button and a None checkbox at the bottom. The None checkbox only shows when the list is empty and allows satisfying a 'required' flag. There is no way to indicate 'no changes', but the office is free to add a separate No Changes checkbox below this list which doesn't actually do anything but which can serve as a visual indicator.</summary>
		[Description("Medication List")]
		MedicationList
	}

	///<summary>This object is serialized as json and stored in EFormField.ValueLabel. It includes the Title, column headers, column widths, etc. No actual medication info, just layout.</summary>
	public class EFormMedListLayout{
		///<summary>The overall title above the list.</summary>
		public string Title="Medication List";
		///<summary>Defaults to 'Medication'</summary>
		public string HeaderCol1="Medication";
		///<summary>Defaults to 'Strength/Freq'</summary>
		public string HeaderCol2="Strength/Freq";
		///<summary>If one of these is left blank, that column will fill remaining space. If both are left blank, they will split 50/50.</summary>
		public int WidthCol1=0;
		///<summary></summary>
		public int WidthCol2=0;
		///<summary>Setting this to false will hide the second column so it only shows the medication names.</summary>
		public bool IsCol2Visible=true;
		///<summary>If set to false, then the patient would enter their meds from scratch each time.</summary>
		public bool PrefillCol1=true;
		///<summary>This gets filled from the med patient note field, which you might not want the patient to see.</summary>
		public bool PrefillCol2=true;
		///<summary>Whether meds are prefilled or not, if this is true, then meds will be imported them back into the database after patient fills them out.</summary>
		public bool ImportCol1=true;
		///<summary>It's hard to import strength and frequency back into the database, so this is false by default. If turned on, the value that the patient fills out would overwrite whatever was originally in the database.</summary>
		public bool ImportCol2Overwrite=false;
		///<summary>If turned on, the database would be overwritten with a date followed by what the patient fills out.</summary>
		public bool ImportCol2OverwriteDate=false;
		///<summary>If turned on, the value that the patient fills out would be appended to the end of the existing value in the database, after a carriage return. Don't use with Prefill.</summary>
		public bool ImportCol2Append=false;
		///<summary>If turned on, the value that the patient fills out would be appended to the end of the existing value in the database, after a carriage return and a date. Don't use with Prefill.</summary>
		public bool ImportCol2AppendDate=false;
	}

	///<summary>A list of these get serialized onto an eForm.</summary>
	public class EFormMed{
		public string MedName="";
		public string StrengthFreq="";
	}

	///<summary>Describes where the label is in relation to the rest of the field.</summary>
	public enum EnumEFormLabelAlign{
		///<summary>0-Default</summary>
		TopLeft,
		///<summary>1-Left of the remainder of the field, and left aligned within that space.</summary>
		LeftLeft,
		///<summary>2-Right of the remainder of the field, and left aligned within that space.</summary>
		Right
		//LeftRight: Left of the remainder of the field, and right aligned within that space.
	}

	public enum EnumEFormBorder{
		///<summary>0-No border</summary>
		None,
		///<summary>1-3D border with gradient shadows and rounded corners</summary>
		ThreeD
	}


}
