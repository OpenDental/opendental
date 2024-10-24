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
		public static void Delete(long eFormNum,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormNum);
				return;
			}
//todo: mark deleted instead, just like sheets
			Crud.EFormCrud.Delete(eFormNum);
			//This triggers removal from eClipboard.
			MobileNotifications.CI_RemoveEForm(patNum,eFormNum);
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
			eForm.SaveImageCategory=eFormDef.SaveImageCategory;
			eForm.ListEFormFields=EFormFields.FromListDefs(eFormDef.ListEFormFieldDefs);
			return eForm;
		}

		///<summary>Validates a few fields like phone numbers and state format. The eForm must contain a list of eFormFields. The maskedSSNOld is used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.  Returns an object that stores the error message and the page number that the problem field is located on. Use the page number to go directly to the problem field. If the return object has an empty error message, everything passed validation.</summary>
		public static EFormValidation Validate(EForm eForm,string maskedSSNOld) {
			EFormValidation eFormValidation=new EFormValidation();
			List<EFormField> listEFormFields=eForm.ListEFormFields;
			int pageNum=1;
			for(int i=0;i<listEFormFields.Count;i++) {
				if(listEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {
					pageNum++;
					continue;
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
			}
			return eFormValidation;//If we get to here, this object should still have default values and everything passed validation.
		}

		///<summary>This is not called from OD proper. Required fields are only enforced in eClipboard. Returns an object that stores the error message and the page number that the problem field is located on. Use the page number to go directly to the problem field. If the return object has an empty error message, everything passed validation.</summary>
		public static EFormValidation ValidateRequired(EForm eForm){
			EFormValidation eFormValidation=new EFormValidation();
			List<EFormField> listEFormFields=eForm.ListEFormFields;
			int pageNum=1;
			for(int i=0;i<listEFormFields.Count;i++) {
				if(listEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {
					pageNum++;
					continue;
				}
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

		///<summary>Language will be empty string if the patient does not have a language set.</summary>
		public static void TranslateFields(EForm eForm,string langIso3){
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<eForm.ListEFormFields.Count;i++){
				if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
					//Language translations are stored as comma delimited string like this: "label,button1,button2"
					//Our setup window, FrmEFormDefEdit, rigorously ensures that the number of items in the translation exactly matches label+buttons.
					//List<string> listEnglishs=
					int numRadioBtns=eForm.ListEFormFields[i].PickListVis.Split(',').ToList().Count();
					string strLabels=eForm.ListEFormFields[i].ValueLabel+","+eForm.ListEFormFields[i].PickListVis;
					string strTranslations=LanguagePats.TranslateEFormField(eForm.ListEFormFields[i].EFormFieldDefNum,langDisplay:"",strLabels,langIso3:langIso3);
					List<string> listTranslations=strTranslations.Split(',').ToList();//Ex: [label,button1,button2]
					//int numTranslations=strTranslations.Split(',').ToList().Count()-1;
					if(listTranslations.Count-1!=numRadioBtns){//subtract 1 because of the label at idx 0.
						continue;//should never happen
					}
					eForm.ListEFormFields[i].ValueLabel=listTranslations[0];
					listTranslations.RemoveAt(0);
					eForm.ListEFormFields[i].PickListVis=string.Join(",",listTranslations);
					continue;
				}
				//checkbox,date,label,sigbox,textbox:
				eForm.ListEFormFields[i].ValueLabel=LanguagePats.TranslateEFormField(eForm.ListEFormFields[i].EFormFieldDefNum,langDisplay:"",eForm.ListEFormFields[i].ValueLabel,langIso3:langIso3);
				//still todo: medlist
			}
		}

		///<summary>Used by both FrmEFormDefs and FormEServicesEClipboard. If there are no EFormDefs in the db when this method is called, then this immediately copies all our internal forms into the db. This could have been done in the ConvertDb script, but it's easier here and we don't every run complex methods when updating versions. Returns true if internal forms were inserted, and returns false if eFormDefs already exist in the db.</summary>
		public static bool InsertInternalToDb(){
			Meth.NoCheckMiddleTierRole();
			List<EFormDef> listEFormDefsCustom=EFormDefs.GetDeepCopy();
			if(listEFormDefsCustom.Count>0){
				return false;//eFormDefs already exist in the db, no need to copy internal into db.
			}
			List<EFormDef> listEFormDefsInternal=EFormInternal.GetAllInternal();
			for(int i=0;i<listEFormDefsInternal.Count;i++){
				listEFormDefsInternal[i].DateTCreated=DateTime.Now;
				EFormDefs.Insert(listEFormDefsInternal[i]);
				for(int f=0;f<listEFormDefsInternal[i].ListEFormFieldDefs.Count;f++){
					listEFormDefsInternal[i].ListEFormFieldDefs[f].EFormDefNum=listEFormDefsInternal[i].EFormDefNum;
					EFormFieldDefs.Insert(listEFormDefsInternal[i].ListEFormFieldDefs[f]);
				}
			}
			return true;
		}

	}

	public class EFormValidation {
		///<summary>If this is empty, then no error.</summary>
		public string ErrorMsg="";
		///<summary>So that the UI can then jump to the page where the error was found.</summary>
		public int PageNum=0;
	}
}