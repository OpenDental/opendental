using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>EForms is a way for patients to fill out forms. This is similar to sheets, but optimized for dynamic layout instead of fixed layout. The office sets up templates, EFormDefs, which get copied to EForms. Each EForm is linked to one patient.</summary>
	[Serializable]
	public class EForm:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EFormNum;
		///<summary>Enum:EnumEFormType 0=PatientForm, 1=MedicalHistory, 2=Consent. This doesn't actually do anything, and all fields are available for all types, but that might eventually change if more types are added.</summary>
		public EnumEFormType FormType;
		///<summary>FKey to patient.PatNum.</summary>
		public long PatNum;
		///<summary>The date and time that show in the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeShown;
		///<summary>The title of the EForm. Copied from EFormDef.Description.</summary>
		public string Description;
		///<summary>The date and time when the EForm was lasted edited. Not editable by the user in the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTEdited;
		///<summary>Required. Can be any value between 50 and 1000. On wide screens, this limits the width of the form. This is needed on pretty much anything other than a phone. Makes it look consistent across devices and prevents useless white space. Default 450.</summary>
		public int MaxWidth;
		///<summary>FKey to eformdef.EFormDefNum. This is only used alongside the eClipboardSheetDef table to determine if the patient has filled out that form yet. It's also used with the RevID to prefill fields that don't use a DbLink by pulling from a previous form. I think this also requires using the ReportableName for those fields.</summary>
		public long EFormDefNum;
		///<summary>Enum:EnumEFormStatus 0-None, 1-ReadyForPatientFill, 2-Filled, 3-Imported. In the None status, the office might be filling in the tooth number on a consent form. In the ReadyForPatientFill status, it will show in eClipboard. Sheets uses ShowInTerminal for this purpose. Once the patient has filled it, the status is changed to Filled. Sheets uses IsWebForm for this purpose. A Filled eForm will then be imported automatically and the status changed to Imported. If the import process could not match to a known patient, then this status will be UnmatchedToPatient and the PatNum will remain 0 until user matches it.</summary>
		public EnumEFormStatus Status;
		///<summary>Revision ID. Used to determine in conjunction with PrefillStatus for eClipboardSheetDef to determine whether to show a patient a new form or have them update their last filled out form. Must match up with EFormDef RevID to show a previously filled out form.</summary>
		public int RevID;
		///<summary>If true, then this form will show labels at 95% and slightly bold. This looks good, but some users might not want it for certain forms, so it's an option. This applies to text, date, radiobuttons, and sigBox. It does not apply to types label, checkbox, or medicationList.</summary>
		public bool ShowLabelsBold;
		///<summary>The amount of space below each field. Overrides the global default and can be overridden by field.SpaceBelow. -1 indicates to use default. That way, 0 means 0 space.</summary>
		public int SpaceBelowEachField;
		///<summary>The amount of space to the right of each field. Overrides the global default and can be overridden by field.SpaceToRight. -1 indicates to use default. That way, 0 means 0 space.</summary>
		public int SpaceToRightEachField;
		///<summary>FK to definition.DefNum. This gets set when an EForm is created from an EFormDef. 0 for none or else it's set to an image category. User can change it. It only saves during the import process or if user saves manually.</summary>
		public long SaveImageCategory;

		///<Summary>Used sparingly prior to db insertion, when we have no other choice.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<EFormField> ListEFormFields;

	}

	///<summary>Different types of EForms that can be used.</summary>
	public enum EnumEFormType {
		///<summary>0 - Includes patient information and insurance information.</summary>
		[Description("Patient Form")]
		PatientForm,
		///<summary>1 - </summary>
		[Description("Medical History")]
		MedicalHistory,
		///<summary>2 - .</summary>
		Consent
	}

	public enum EnumEFormStatus{
		///<summary>0 - If a form is added manually in OD proper and then filled out by the patient in OD proper, it will remain this status unless it gets imported.</summary>
		None,
		///<summary>1 - Forms with this status will show in eClipboard. Unlike sheets, there is no way to set order.</summary>
		ShowInEClipboard,
		///<summary>2 - This status gets set from eClipboard after filling.</summary>
		Filled,
		///<summary>3 - .</summary>
		Imported,
		///<summary>4 - Import was attempted, but could not match a known patient in the database. Once it's matched to a patient, it gets converted to Imported status.</summary>
		UnmatchedToPatient
	}

	//<summary>These are the different internal eForm types that show up on the left grid of FrmEFormDefs. For example, we might have two different internal eForms called PatientRegistration and PatientLetter but they would both have an EnumEFormType set to PatientForm.</summary>
	//public enum EnumEFormInternalType {
		//<summary>0 - Patient Registration Form. eformdef.EnumEFormType will be set to PatientForm.</summary>
		//[Description("Patient Registration")]
		//PatientRegistration,
		/* Will be added when we actually have these forms saved in the resource folder.
		///<summary>1 - Medical History Form. eformdef.EnumEFormType will be set to MedicalHistory.</summary>
		[Description("Medical History")]
		MedicalHist,
		///<summary>2 - Consent Form. eformdef.EnumEFormType will be set to Consent.</summary>
		Consent
		*/
	//}
}
