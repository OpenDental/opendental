using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
/*
EForms is a complete replacement for the mobile version of sheets for patient forms. 
It's also designed to be used on larger devices so that it can replace sheets being used for webforms.
We will probably not be able to completely deprecate mobile sheets immediately, but we will try very hard for most customers.
EForms are fillable from within OD, although this is not really intended for patients. 
Patients will eventually be able to fill out EForms online in the web version of eClipboard and also on eClipboard in the office.

Relationship to Sheets:
We did a good job on sheets, but we never should have tried to adapt sheets for mobile dynamic layout. We should have just started over. Now we are.
Once EForms is mature and powerful, we might add a way to convert a mobile sheet into a more powerful EForm.
Since EForms is intended to replace Mobile Sheets, we need to explain how to do the same things here that we did there.
Sheets are laid out in absolute x,y coords, with some additional growth behavior built in.
EForms are entirely dynamic
Mobile sheets had no positional control but were instead just stacked vertical scrolling.
EForms is still stacked vertical scrolling, but there are also horizontal stacking options mixed in.
Internal EFormDefs are hard coded in C#, just like internal sheets.
If a user makes a copy of in internal EFormDef, they will now have a custom EFormDef that is identical but stored in the database.
A custom EFormDef is editable. User can add/remove/rearrange EFormFields.
Sheets stored database link as FieldName. EForms uses DbLink.

Comparison of field types:
Sheet OutputText and InputField are combined into a single type called TextField.
StaticText has been renamed Label.
Nearly identical in both: Checkbox, ComboBox, and Signature.
Not supported in EForms: StaticImage, PatImage, Line, Rect.
Additional in EForms: PageBreak, DateField, RadioButton

Allergies and Problems:
These are particularly difficult.
Medications is not included here because it's even harder and requires its own special control.
Allergies and Problems are handled similarly. I'll use allergy as an example.
In the first version, we support a series of checkboxes that get prechecked if the patient has that allergy.
All of these text boxes will have DbLinks that look like "allergy:..."
We also provide an "allergyOther" textbox to enter allergies not in the main list.
The "allergyOther" textbox is comma delimited and importable.
The "allergyOther" textbox gets prefilled for all existing allergies that are not handled by checkboxes.
Any import not matching an existing allergy in OD gets imported as an allergy called "Other", with the name stored in the note field.
A "none of the above" box works as a group with the checkboxes and allergyOther to support IsRequired.

Insurance
Import is extremely difficult, so we will mostly not automatically import, and then just add features to help with that.
The import process of sheets and eForms in general will just need a lot of attention later.
We might add a button just for Ins import since everything else will be done automatically.
We need to consider where to put various import notes. Flex uses Service Notes for some of it. Commlog entry?
We might also recommend a checkbox to indicate if insurance has changed. Maybe that should trigger some sort of warning for the office.

Language Translation
(still working on this documentation)
For radiobuttons, we must concat label and all radiobutton labels.
PickListVis holds the radiobutton labels for the English version.
We ignore PickListDb because those won't change with different languages.
Our delimiter will be comma. Like this:
Example Label,Radio1Span,Radio2Span
Radiobuttons are already validated to not contain commas.
Do the same for radiobutton labels, English version.
Neither labels nor radiobuttons require visible labels.
So PickListVis might already look like ",,"
and ValueLabel might also look like ""
The translation allows empty strings as well, regardless of whether English has empty strings.
A translation could also look like ",,,".
The number of items in the translation is critical and must match.
Rename VisDb to VisDbLang and add a third field.
Add another column to the grid if this is not default language.
They are allowed to change both the English and foreign translations in that grid at any time.

*/

	///<summary>EForms are a way for patients to fill out forms. This is similar to sheets, but optimized for dynamic layout instead of fixed layout. The office sets up templates, EFormDefs, which get copied to EForms. Since this is a template EForm, it does not link to a patient. It can be freely changed without affecting any EForms. We also supply internal EFormDefs, which are hard coded as XML rather than being in any office database.</summary>
	[Serializable]
	public class EFormDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EFormDefNum;
		///<summary>Enum:EnumEFormType 0=PatientForm, 1=MedicalHistory, 2=Consent. This doesn't actually do anything, and all fields are available for all types, but that might eventually change if more types are added.</summary>
		public EnumEFormType FormType;
		///<summary>The title of the EFormDef. Set by the user.</summary>
		public string Description;
		///<summary>The date and time when the EFormDef was created. Not editable by the user in the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTCreated;
		///<summary>This is set to true when a user "deletes" an internal form. That way, it won't show anymore. This is the only way to hide an internal form. It gets done automatically when a user "edits" an internal form so that only the custom form will show. The linkage to the internal form is done by matching the Description. So a row where this was set to true would have a description and nothing else. No EFormFieldDefs. Once the user hides an internal eForm in this manner, they can't ever get it back other than by using Add.</summary>
		public bool IsInternalHidden;
		///<summary>Required. Can be any value between 50 and 1000. On wide screens, this limits the width of the form. This is needed on pretty much anything other than a phone. Makes it look consistent across devices and prevents useless white space. Default 450.</summary>
		public int MaxWidth;
		///<summary>Revision ID. Gets updated any time an eForm field is added or deleted from an eFormDef (this includes any time a translation is changed). Used to determine in conjunction with PrefillStatus for eClipboardSheetDef to determine whether to show a patient a new form or have them update their last filled out form. Must match up with EForm RevID to show a previously filled out form.</summary>
		public int RevID;
		///<summary>If true, then this form will show labels at 95% and slightly bold. This looks good, but some users might not want it for certain forms, so it's an option. This applies to text, date, radiobuttons, and sigBox. It does not apply to types label, checkbox, or medicationList.</summary>
		public bool ShowLabelsBold;
			///<summary>The amount of space below each field. Overrides the global default and can be overridden by field.SpaceBelow. -1 indicates to use default. That way, 0 means 0 space.</summary>
		public int SpaceBelowEachField;
		///<summary>The amount of space to the right of each field. Overrides the global default and can be overridden by field.SpaceToRight. -1 indicates to use default. That way, 0 means 0 space.</summary>
		public int SpaceToRightEachField;
		///<summary>FK to definition.DefNum. There is a global setting to save forms to the image category which has ItemVal set to "U". This global setting is only used to set this field for new forms. If this is 0, it will not save to images. Any other number is an override to a different category from global. Copied to EForm child.</summary>
		public long SaveImageCategory;

		///<Summary>This is needed for serialization/deserialization of internal EForms. We also leave this list attached to internal EForms for a while for convenience.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<EFormFieldDef> ListEFormFieldDefs;
		///<summary>Not a db field.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsInternal;
		///<summary>Not a db field.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsDeleted;

		public EFormDef Copy() {
			return (EFormDef)this.MemberwiseClone();
		}		
	}
}

/*
Flex has multiple videos on how they do their setup. Here's one:
https://www.youtube.com/watch?v=4V-wbDuVtFg 
Ours are similar.

Jordan:
So something about when a user tries to set a stacked field to be full width. Right now we silently set a width. Probably a msgbox instead.

RyanH:
Language translation for all other types, using textBox as example
Dates should be single lines instead of rectangles when inside border boxes
Default border=true for new text, date, checkbox, and radiobuttons 
Refine the 3 built-in forms according to my interative instructions

RyanR:
Overhaul to follow layout strategy at top of CtrlEFormFill
Figure out the details for font scaling and size scaling based on font, as described
Show me screenshots about every day from now on so that we make sure we are both on the same page
Implement IsLocked in eClipboard UI

Enhance FormWebForms to include eForms. I don't think eForms get retrieved, but do they still show on that form?
	For sheets, they show if DateTimeSheet is within search range and IsWebForm is true.
	We could just mirror that for eForms. Or IsWebForm probably doesn't even matter.
	For eForms, DateTimeShown is the field that is analagous to DateTimeSheet. 
	Both are the dates that show at the lower left of the fill window,
	and both are the dates used for showing in commlog and also indicate when they were filled out.
(done) Slight enhancements to FormPatientPickWebForm for wording
and .IsWebForm
WebForms_Preference.ColorBorder
Background color
Conditional dates/ages
Permission to edit complete EForm, use Sheet permission?

Future Allergies, Problems, Medications:
There are many possible future improvements. Some are listed below in order of priority, with allergies as an example:
A current list of allergies along with a formal way to add/remove.
This list control could be combined with the custom checkboxes or yes/no boxes to avoid the need for an "other" textbox.
Flex uses categories that get expanded. Those don't look useful to me.
Medications are more complex than Allergies and Problems, so we use a special control.

Future enhancements, in order of priority:
Address same for entire family checkbox
Carryover when filling multiple forms in one session. (not needed if Address same for family is done right)
Branding, using colors, logos, and images, using MobileBrandingProfile.
Signatures
	SignatureBoxWrapper.GetIsTypedFromWebForms()
	DateTimeSig: ??
	CanElectronicallySign: ??
	IsSigProvRestricted: not needed because provs don't sign patient forms

Automate language translations somehow?
Convert all translations from mobile sheets 
Lists for Allergies, Problems
Allergies and meds, more elegant options than Other.
Tools to combine meds and probs, like we already do with allergies.
Columns of checkboxes, probably possible now. Test
TextArea specify rows showing initially
Validator and formatter for zip, state, phone, email, etc.
Fontname
SigBox encryption rather than just the drawing.
ComboBox
Listbox for lists that are shorter than a comboBox, but longer than a radioButton group.
Multiselect listbox. Probably done with visual aid by adding a checkbox to each item because they can't use Ctrl key to multiselect.
We already support unselecting all radiobuttons to represent another state in the db. Example: For Marital Status, you might only show Married and Child, excluding Divorced and Single from the pick list. The unselected state then represents no change, so an existing patient could leave both radiobuttons unchecked and their status would remain Divorced or Single. The enhancement is that we would allow them to uncheck if they accidentally checked or if already checked, and the result would be unchanged or default. This will clearly require a few unit tests, or at least specific written scenarios.
Global EForms FontSize adjustment which would multiply all fields by this percentage.
Misc fields that are not importable also prefill for subsequent updates.
In label edit window, rich text box. The following are technically very difficult, so we don't have time to do them yet. They can be listed as annoyances.
	Text remain highlighted when in font window.
	Text selection follow pointer better instead of automatically selecting entire word
	Support mixed formatting in the selection, font window being aware of that third state.
Label edit window, icons and fancier toolbar functionality like pick font size in toolbar.
Radiobuttons for referral sources in addition to the textbox

Enhancements we will not add:
Multiselect radioButtons. Use a multiselect listbox instead
Checkbox to indicate import. No need because that's what DbLink indicates.

*/
