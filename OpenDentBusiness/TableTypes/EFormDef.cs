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

Conditional Logic
To implement this properly, we must include as many examples as possible:
Do you have insurance?, Yes or No
I have insurance, checkbox
Gender, Female
Our conditional logic will be placed on the field that we want to show or hide.
1. Parent field ... User can type it in or pick from a list, based on the ValueLabels. This means that ValueLabels are required for now.
2. Has value... checked, Yes, Female, etc. The value showing, not the db value. Checkboxes will use checked/unchecked for now.
If all the fields on a page are hidden, that page gets hidden.
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

		///<Summary>This is needed for serialization/deserialization of internal EForms. We also leave this list attached to internal EForms for a while because any list would not have a FK way to associate with the EFormDef.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<EFormFieldDef> ListEFormFieldDefs;
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsInternal;

		public EFormDef Copy() {
			return (EFormDef)this.MemberwiseClone();
		}		
	}
}

/*
Flex has multiple videos on how they do their setup. Here's one:
https://www.youtube.com/watch?v=4V-wbDuVtFg 
Ours are similar.

Next:
Locking an EForm should not make it dim and should not disable nav buttons.
ClearSigs whenever we change a field
Permission to edit complete EForm, use Sheet permission?
Required fields never actually got implemented
When filling forms, changing window width does not perform another layout.
Enhance FormWebForms to include eForms
Slight enhancements to FormPatientPickWebForm for wording
What about FormPatientPickWebForm?
and .IsWebForm
WebForms_Preference.ColorBorder
Background color
Conditional dates/ages

Widths in a future version: We might add:
 Proportional widths, either * or %.
 Minimum widths are required to make this work, probably at the form level instead of field level.
Because of the minimum widths, fixed and proportional could be intermingled.
Once they hit min widths, they would start wrapping.
Default would be proportional.
Auto is not needed because we already know how wide each element needs to be
and textboxes don't really ever need auto width.
Another nagging detail is that these aren't really min widths 
because screen could be narrower than specified min width and it must be ignored in that case.

Future Allergies, Problems, Medications:
There are many possible future improvements. Some are listed below in order of priority, with allergies as an example:
A series of YesNo boxes to force them to answer each question.
A current list of allergies along with a formal way to add/remove.
This list control could be combined with the custom checkboxes or yes/no boxes to avoid the need for an "other" textbox.
Flex uses categories that get expanded. Those don't look useful to me.
Medications are more complex than Allergies and Problems, so we use a special control.

Future enhancements, in order of priority:
Add functionality in FormSheetImport to allow EForms.
Branding, using colors, logos, and images, using MobileBrandingProfile.
Signatures
	Unlock button
	SignatureBoxWrapper.GetIsTypedFromWebForms()
	DateTimeSig: ??
	CanElectronicallySign: ??
	IsSigProvRestricted: not needed because provs don't sign patient forms

Language Translation. 
	Not sure what we'll translate exactly, maybe labels, picklists for comboboxes and radiobuttons, meds, problems, allergies, etc.
	Because we use dynamic layout, we don't need to maintain that awful layout syncing that sheets use for languages.
	We also want to support unlimited languages tacked onto the fields of a single form.
	This will clearly require another db table, with one row for each translation.
	When we do implement translation, it will not include everything listed above, probably just labels on the first pass.
	Automate translations somehow?
	Convert all translations from mobile sheets 
Lists for Allergies, Problems
YesNo for Allergies, Problems, Medications
Allergies and meds, more elegant options than Other.
Tools to combine meds and probs, like we already do with allergies.
Columns of checkboxes
TextArea specify rows showing initially
Carryover when filling multiple forms in one session.
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

Enhancements we will not add:
Multiselect radioButtons. Use a multiselect listbox instead
Checkbox to indicate import. No need because that's what DbLink indicates.

*/
