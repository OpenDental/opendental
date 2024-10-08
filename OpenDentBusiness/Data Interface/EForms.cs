using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EForms{
		///<summary>Gets a single eForm from the database.  Then, gets all the fields for it.  So it returns a fully functional eForm. Returns null if the eform isn't found in the database.</summary>
		public static EForm GetEForm(long eFormNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EForm>(MethodBase.GetCurrentMethod(),eFormNum);
			}
			EForm eForm=Crud.EFormCrud.SelectOne(eFormNum);
			if(eForm==null) {
				return null;//eForm was deleted.
			}
			eForm.ListEFormFields=EFormFields.GetForForm(eFormNum);
			return eForm;
		}

		///<summary></summary>
		public static long Insert(EForm eForm){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eForm.EFormNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eForm);
				return eForm.EFormNum;
			}
			return Crud.EFormCrud.Insert(eForm);
		}

		///<summary></summary>
		public static void Update(EForm eForm){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eForm);
				return;
			}
			Crud.EFormCrud.Update(eForm);
		}

		///<summary></summary>
		public static void Delete(long eFormNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormNum);
				return;
			}
			Crud.EFormCrud.Delete(eFormNum);
		}

		/// <summary>The eFormDef passed in must have ListEFormFieldDefs already filled. The resulting EForm will also have its fields already attached. Neither the form nor the fields get inserted into the db here.</summary>
		public static EForm CreateEFormFromEFormDef(EFormDef eFormDef,long patNum) {
			EForm eForm=new EForm();
			eForm.IsNew=true;
			eForm.FormType=eFormDef.FormType;
			eForm.PatNum=patNum;
			eForm.DateTimeShown=DateTime.Now;
			eForm.DateTEdited=DateTime.Now;
			eForm.Description=eFormDef.Description;
			eForm.MaxWidth=eFormDef.MaxWidth;
			eForm.RevID=eFormDef.RevID;
			eForm.ShowLabelsBold=eFormDef.ShowLabelsBold;
			eForm.SpaceBelowEachField=eFormDef.SpaceBelowEachField;
			eForm.SpaceToRightEachField=eFormDef.SpaceToRightEachField;
			eForm.ListEFormFields=EFormFields.FromListDefs(eFormDef.ListEFormFieldDefs);
			return eForm;
		}

		///<summary>Validates all of the fields that have a DbLink from the passed-in eForm. The eForm must contain a list of eFormFields. The maskedSSNOld is used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.  Returns an object that stores the error message and the page number that the problem field is located on. Use the page number to go directly to the problem field. If the return object has an empty error message, everything passed validation.</summary>
		public static EFormValidation Validate(EForm eForm,string maskedSSNOld) {
			EFormValidation eFormValidation=new EFormValidation();
			List<EFormField> listEFormFields=eForm.ListEFormFields;
			int pageNum=1;
			for(int i=0;i<listEFormFields.Count;i++) {
				if(listEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {
					pageNum++;
				}
				//Validating Allergies
				if(listEFormFields[i].DbLink=="allergiesNone") {
					List<EFormField> listEFormFieldsAllergyChecks=listEFormFields.FindAll(x=>x.DbLink.StartsWith("allergy:"));
					List<EFormField> listEFormFieldsAllergiesOther=listEFormFields.FindAll(x=>x.DbLink=="allergiesOther");
					if(listEFormFields[i].ValueString=="X") {
						if(listEFormFieldsAllergyChecks.Any(x=>x.ValueString=="X") || listEFormFieldsAllergiesOther.Any(x=>x.ValueString!="")) {
							eFormValidation.ErrorMsg="You cannot have '"+listEFormFields[i].ValueLabel+"' checked and an allergy checked or written in.";
							eFormValidation.PageNum=pageNum;
							return eFormValidation;
						}
					}
					else if(listEFormFields[i].IsRequired) {
						if(listEFormFieldsAllergyChecks.All(x=>x.ValueString=="") &&  listEFormFieldsAllergiesOther.All(x=>x.ValueString=="")) {
							eFormValidation.ErrorMsg="If you do not have any allergies, you must check '"+listEFormFields[i].ValueLabel+"'";
							eFormValidation.PageNum=pageNum;
							return eFormValidation;
						}
					}
				}
				//Validating Problems
				if(listEFormFields[i].DbLink=="problemsNone") {
					List<EFormField> listEFormFieldsProblemChecks=listEFormFields.FindAll(x=>x.DbLink.StartsWith("problem:"));
					List<EFormField> listEFormFieldsProblemsOther=listEFormFields.FindAll(x=>x.DbLink=="problemsOther");
					if(listEFormFields[i].ValueString=="X") {
						if(listEFormFieldsProblemChecks.Any(x=>x.ValueString=="X") || listEFormFieldsProblemsOther.Any(x=>x.ValueString!="")) {
							eFormValidation.ErrorMsg="You cannot have '"+listEFormFields[i].ValueLabel+"' checked and a problem checked or written in.";
							eFormValidation.PageNum=pageNum;
							return eFormValidation;
						}
					}
					else if(listEFormFields[i].IsRequired) {
						if(listEFormFieldsProblemChecks.All(x=>x.ValueString=="") && listEFormFieldsProblemsOther.All(x=>x.ValueString!="")) {
							eFormValidation.ErrorMsg="If you do not have any problems, you must check '"+listEFormFields[i].ValueLabel+"'";
							eFormValidation.PageNum=pageNum;
							return eFormValidation;
						}
					}
				}
				//Validating State Field
				if(!ValidateStateField(eForm)) {
					eFormValidation.ErrorMsg="The State field must be exactly two characters in length.";
					eFormValidation.PageNum=pageNum;
					return eFormValidation;
				}
				//Validating Phone Numbers
				if(listEFormFields[i].DbLink.Contains("Phone")) {
					if(TelephoneNumbers.IsNumberValidTenDigit(listEFormFields[i].ValueString)) {
						listEFormFields[i].ValueString=TelephoneNumbers.AutoFormat(listEFormFields[i].ValueString);
					}
					else {
						eFormValidation.ErrorMsg="Please fix the format on the entered phone number";
						eFormValidation.PageNum=pageNum;
						return eFormValidation;
					}
				}
				//Validating SSN
				if(!ValidateSSN(listEFormFields[i],maskedSSNOld)) {
					eFormValidation.ErrorMsg="Patient's Social Security Number is invalid.";
					eFormValidation.PageNum=pageNum;
					return eFormValidation;
				}
//todo
//Validate email field always
				//Check IsRequired Fields
				if(listEFormFields[i].DbLink!="allergiesNone" && listEFormFields[i].DbLink!="problemsNone") {
					//This is for all the other fields. This does work with radiobuttons, for example
					if(listEFormFields[i].IsRequired 
						&& listEFormFields[i].ValueString.IsNullOrEmpty()
						&& !listEFormFields[i].IsHiddenCondit) 
					{
						eFormValidation.ErrorMsg=listEFormFields[i].ValueLabel+" is a required field";
						eFormValidation.PageNum=pageNum;
						return eFormValidation;
					}
				}
			}
			return eFormValidation;//If we get to here, this object should still have default values and everything passed validation.
		}

		///<summary>Verify that the InputField of "State" is exactly 2 characters in length. </summary>
		public static bool ValidateStateField(EForm eForm) {
			if(eForm.FormType!=EnumEFormType.PatientForm) {
				return true;
			}
			for(int i=0;i<eForm.ListEFormFields.Count;i++){
				if(eForm.ListEFormFields[i].DbLink!="State"){
					continue;
				}
				if(eForm.ListEFormFields[i].ValueString.Trim().Length!=2 && eForm.ListEFormFields[i].ValueString.Trim().Length>0) {
					return false;
				}
			}
			return true;
		}

		public static bool ValidateSSN(EFormField eFormField,string maskedSSNOld) {
			if(eFormField.DbLink!="SSN") {
				return true;
			}
			string textSSN=eFormField.ValueString;
			if(CultureInfo.CurrentCulture.Name!="en-US"){
				return true;
			}
			//only reformats if in USA and exactly 9 digits.
			if(string.IsNullOrEmpty(textSSN)){
				return true;
			}
			if(PrefC.GetBool(PrefName.PatientSSNMasked) || !Security.IsAuthorized(EnumPermType.PatientSSNView, true)) {
				if(textSSN==maskedSSNOld) {//If SSN hasn't changed, don't validate.  It is masked.
					return true;
				}
			}
			if(!Regex.IsMatch(textSSN,@"^\d{9}$") && !Regex.IsMatch(textSSN,@"^\d{3}-\d{2}-\d{4}$")) {
				return false;
			}
			if(textSSN.Length==9){//if just numbers, try to reformat.
				for(int j=0;j<textSSN.Length;j++){
					if(!Char.IsNumber(textSSN,j)){
						return false;
					}
				}
				eFormField.ValueString=textSSN.Substring(0,3)+"-"
					+textSSN.Substring(3,2)+"-"+textSSN.Substring(5,4);	
			}
			return true;
		}
		
		///<summary>Loops through all the fields and appends together all the ValueStrings. All the ValueStrings must have been filled first, and it excludes all SigBox types. The order is critical.</summary>
		public static string GetSignatureKeyData(List<EFormField> listEFormFields) {
			Meth.NoCheckMiddleTierRole();
			//The fields will already be sorted by ItemOrder
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<listEFormFields.Count;i++) {
				if(listEFormFields[i].FieldType.In(EnumEFormFieldType.SigBox)) {
					continue;
				}
				stringBuilder.Append(listEFormFields[i].ValueString);
			}
			return stringBuilder.ToString();
		}
	}

	public class EFormValidation {
		///<summary>If this is empty, then no error.</summary>
		public string ErrorMsg="";
		///<summary>So that the UI can then jump to the page where the error was found.</summary>
		public int PageNum=0;
	}
}