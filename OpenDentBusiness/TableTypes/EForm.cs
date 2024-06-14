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
		///<summary>Enum:EnumEFormType 0=None, 1=PatientForm, 2=MedicalHistory, 3=Consent.</summary>
		public EnumEFormType FormType;
		///<summary>FKey to patient.PatNum.</summary>
		public long PatNum;
		///<summary>The date and time that show in the UI. It will be editable at some point.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeShown;
		///<summary>The title of the EForm. Copied from EFormDef.Description.</summary>
		public string Description;
		///<summary>The date and time when the EForm was lasted edited. Not editable by the user in the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTEdited;

		///<Summary>Used sparingly prior to db insertion, when we have no other choice.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<EFormField> ListEFormFields;

	}

	///<summary>Different types of EForms that can be used. They don't actually do anything, and all fields are available from all types, but that might eventually change if more types are added.</summary>
	public enum EnumEFormType {
		//<summary>0 - Not used yet, but just in case.</summary>//this didn't make any sense.
		//None,
		///<summary>0 - Includes patient information and insurance information.</summary>
		[Description("Patient Form")]
		PatientForm,
		///<summary>1 - </summary>
		[Description("Medical History")]
		MedicalHistory,
		///<summary>2 - .</summary>
		Consent
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
