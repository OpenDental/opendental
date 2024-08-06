using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	public class EFormFiller {
		////Any changes to the list below should be duplicated in EFormFieldsAvailable and EFormImport.

		///<summary>Gets some data from the database and fills the fields. Input should only be new eForm. Fields must already be attached.</summary>
		public static void FillFields(EForm eForm){
			Family family=Patients.GetFamily(eForm.PatNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(eForm.PatNum);
			if(!PatPlans.IsPatPlanListValid(listPatPlans)) {
				listPatPlans=PatPlans.Refresh(eForm.PatNum);
			}
			Patient patient=family.GetPatient(eForm.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			InsPlan insPlan1=null;
			InsSub insSub1=null;
			Carrier carrier1=null;
			if(listPatPlans.Count>0){
				insSub1=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
				insPlan1=InsPlans.GetPlan(insSub1.PlanNum,listInsPlans);
				carrier1=Carriers.GetCarrier(insPlan1.CarrierNum);
			}
			InsPlan insplan2=null;
			InsSub sub2=null;
			Carrier carrier2=null;
			if(listPatPlans.Count>1) {
				sub2=InsSubs.GetSub(listPatPlans[1].InsSubNum,listInsSubs);
				insplan2=InsPlans.GetPlan(sub2.PlanNum,listInsPlans);
				carrier2=Carriers.GetCarrier(insplan2.CarrierNum);
			}
			PatientNote patientNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor);
			List<Allergy> listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//just active allergies
			List<AllergyDef> listAllergyDefs=AllergyDefs.GetAll(includeHidden:true);//from db. AllergyDefs are oddly not cached, so we need the entire list ahead of time
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
			List<Disease> listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);
			for(int i=0;i<eForm.ListEFormFields.Count;i++){
				//EFormField field=eForm.ListEFormFields[i];//not allowed
				if(eForm.ListEFormFields[i].DbLink=="Address") {
					eForm.ListEFormFields[i].ValueString=patient.Address;
				}
				if(eForm.ListEFormFields[i].DbLink=="Address2") {
					eForm.ListEFormFields[i].ValueString=patient.Address2;
				}
				/*
				//if(eForm.ListEFormFields[i].DbLink=="addressAndHmPhoneIsSameEntireFamily") {
				//	bool isSame=true;
				//	for(int i=0;i<family.ListPats.Length;i++){
				//		if(patient.HmPhone!=family.ListPats[i].HmPhone
				//			|| patient.Address!=family.ListPats[i].Address
				//			|| patient.Address2!=family.ListPats[i].Address2
				//			|| patient.City!=family.ListPats[i].City
				//			|| patient.State!=family.ListPats[i].State
				//			|| patient.Zip!=family.ListPats[i].Zip)
				//		{
				//			isSame=false;
				//			continue;
				//		}
				//	}
				//	if(isSame) {
				//		field.FieldValue="X";
				//	}
				//}*/
				if(eForm.ListEFormFields[i].DbLink=="allergiesNone") {
					if(listAllergiesPat.Count==0){
						eForm.ListEFormFields[i].ValueString="X";
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="allergiesOther") {
					//get list of allergies already covered by checkboxes and radiobuttons
					List<string> listStrAllergiesChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("allergy:")).Select(x=>x.DbLink.Substring(8)).ToList();
					List<string> listStrAllergiesToAdd=new List<string>();
					for(int a=0;a<listAllergiesPat.Count;a++){
						AllergyDef allergyDef=listAllergyDefs.Find(x=>x.AllergyDefNum==listAllergiesPat[a].AllergyDefNum);
						string strAllerg=allergyDef.Description;
						if(listStrAllergiesChecks.Contains(strAllerg)) {
							continue;//this allergy is already covered by a checkbox or radiobutton
						}
						if(strAllerg.Contains(",")){
							continue;//ignore allergies that patient has that contain commas. It would mess up our serialization.
						}
						if(strAllerg=="Other"){
							//The allergy attached to patient is "Other", probably from a previous import
							if(listAllergiesPat[a].Reaction.Contains(",")){
								continue;
							}
							listStrAllergiesToAdd.Add(listAllergiesPat[a].Reaction);//this is where we store the actual allergy name for the Others.
							continue;
						}
						listStrAllergiesToAdd.Add(strAllerg);
					}
					eForm.ListEFormFields[i].ValueString=string.Join(", ",listStrAllergiesToAdd);
				}
				if(eForm.ListEFormFields[i].DbLink.StartsWith("allergy:")) {
					string strAllergyName=eForm.ListEFormFields[i].DbLink.Substring(8);
					bool hasAllergy=false;
					for(int a=0;a<listAllergiesPat.Count;a++){
						AllergyDef allergyDef=listAllergyDefs.Find(x=>x.AllergyDefNum==listAllergiesPat[a].AllergyDefNum);
						//Should never be null, but there might be an edge case I haven't thought of
						if(allergyDef==null){
							continue;
						}
						string strAllergDes=allergyDef.Description;
						if(strAllergDes==strAllergyName) {
							hasAllergy=true;
							break;
						}
					}
					if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
						if(hasAllergy){
							eForm.ListEFormFields[i].ValueString="X";
						}
					}
					if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
						if(hasAllergy){
							eForm.ListEFormFields[i].ValueString="Y";
						}
						else{
							eForm.ListEFormFields[i].ValueString="N";
						}
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="Birthdate") {
					eForm.ListEFormFields[i].ValueString=patient.Birthdate.ToShortDateString();
				}
				if(eForm.ListEFormFields[i].DbLink=="City") {
					eForm.ListEFormFields[i].ValueString=patient.City;
				}
				if(eForm.ListEFormFields[i].DbLink=="Email") {
					eForm.ListEFormFields[i].ValueString=patient.Email;
				}
				if(eForm.ListEFormFields[i].DbLink=="FName") {
					eForm.ListEFormFields[i].ValueString=patient.FName;
				}
				if(eForm.ListEFormFields[i].DbLink=="Gender") {
					eForm.ListEFormFields[i].ValueString=patient.Gender.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="HmPhone") {
					eForm.ListEFormFields[i].ValueString=patient.HmPhone;
				}
				if(eForm.ListEFormFields[i].DbLink=="ICEName") {
					eForm.ListEFormFields[i].ValueString=patientNote.ICEName;
				}
				if(eForm.ListEFormFields[i].DbLink=="ICEPhone") {
					eForm.ListEFormFields[i].ValueString=patientNote.ICEPhone;
				}
				#region Insurance
				if(eForm.ListEFormFields[i].DbLink=="ins1CarrierName") {
					if(carrier1!=null){
						eForm.ListEFormFields[i].ValueString=carrier1.CarrierName;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1CarrierPhone") {
					if(carrier1!=null) {
						eForm.ListEFormFields[i].ValueString=carrier1.Phone;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1EmployerName") {
					if(insPlan1!=null) {
						eForm.ListEFormFields[i].ValueString=Employers.GetName(insPlan1.EmployerNum);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1GroupName") {
					if(insPlan1!=null) {
						eForm.ListEFormFields[i].ValueString=insPlan1.GroupName;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1GroupNum") {
					if(insPlan1!=null) {
						eForm.ListEFormFields[i].ValueString=insPlan1.GroupNum;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1Relat") {
					if(listPatPlans.Count==0){
						eForm.ListEFormFields[i].ValueString="";
					}
					else{
						eForm.ListEFormFields[i].ValueString=listPatPlans[0].Relationship.ToString();
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1SubscriberID") {
					if(insPlan1!=null) {
						eForm.ListEFormFields[i].ValueString=insSub1.SubscriberID;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins1SubscriberNameF") {
					if(insPlan1!=null) {
						eForm.ListEFormFields[i].ValueString=family.GetNameInFamFirst(insSub1.Subscriber);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2CarrierName") {
					if(carrier2!=null) {
						eForm.ListEFormFields[i].ValueString=carrier2.CarrierName;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2CarrierPhone") {
					if(carrier2!=null) {
						eForm.ListEFormFields[i].ValueString=carrier2.Phone;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2EmployerName") {
					if(insplan2!=null) {
						eForm.ListEFormFields[i].ValueString=Employers.GetName(insplan2.EmployerNum);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2GroupName") {
					if(insplan2!=null) {
						eForm.ListEFormFields[i].ValueString=insplan2.GroupName;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2GroupNum") {
					if(insplan2!=null) {
						eForm.ListEFormFields[i].ValueString=insplan2.GroupNum;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2Relat") {
					if(listPatPlans.Count<2){
						eForm.ListEFormFields[i].ValueString="";
					}
					else{
						eForm.ListEFormFields[i].ValueString=listPatPlans[1].Relationship.ToString();
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2SubscriberID") {
					if(insplan2!=null) {
						eForm.ListEFormFields[i].ValueString=sub2.SubscriberID;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="ins2SubscriberNameF") {
					if(insplan2!=null) {
						eForm.ListEFormFields[i].ValueString=family.GetNameInFamFirst(sub2.Subscriber);
					}
				}
				#endregion Insurance
				if(eForm.ListEFormFields[i].DbLink=="LName") {
					eForm.ListEFormFields[i].ValueString=patient.LName;
				}
				if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList) {
					EFormMedListLayout eFormMedListLayout=JsonConvert.DeserializeObject<EFormMedListLayout>(eForm.ListEFormFields[i].ValueLabel);
					if(eFormMedListLayout.PrefillCol1){
						List<EFormMed> listEFormMeds=new List<EFormMed>();
						for(int a=0;a<listMedicationPats.Count;a++){
							Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
							string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
							if(medication!=null){
								strMed=medication.MedName;
							}
							EFormMed eFormMed=new EFormMed();
							eFormMed.MedName=strMed;
							if(eFormMedListLayout.PrefillCol2){
								eFormMed.StrengthFreq=listMedicationPats[a].PatNote;
							}
							listEFormMeds.Add(eFormMed);
						}
						eForm.ListEFormFields[i].ValueString=JsonConvert.SerializeObject(listEFormMeds);
					}
				}
				//if(eForm.ListEFormFields[i].DbLink=="medsNone") {
				//	if(listMedicationPats.Count==0){
				//		eForm.ListEFormFields[i].ValueString="X";
				//	}
				//}
				/*if(eForm.ListEFormFields[i].DbLink=="medsOther") {
					//get list of meds already covered by checkboxes
					List<string> listStrMedsChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("med:")).Select(x=>x.DbLink.Substring(4)).ToList();
					List<string> listStrMedsToAdd=new List<string>();
					for(int a=0;a<listMedicationPats.Count;a++){
						Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
						string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
						if(medication!=null){
							strMed=medication.MedName;
						}
						if(listStrMedsChecks.Contains(strMed)) {
							continue;//this med is already covered by a checkbox
						}
						if(strMed.Contains(",")){
							continue;//ignore meds that patient has that contain commas. It would mess up our serialization.
						}
						//unlike allergies and problems, meds doesn't use a dummy "Other" med
						listStrMedsToAdd.Add(strMed);
					}
					eForm.ListEFormFields[i].ValueString=string.Join(", ",listStrMedsToAdd);
				}*/
				//if(eForm.ListEFormFields[i].DbLink.StartsWith("med:")){
				//	//get the checkbox med that we are looking at
				//	string strMedCheck=eForm.ListEFormFields[i].DbLink.Substring(4);
				//	for(int a=0;a<listMedicationPats.Count;a++){
				//		Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
				//		string strMedDes=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForms import and medication is null
				//		if(medication!=null){
				//			strMedDes=medication.MedName;
				//		}
				//		if(strMedDes==strMedCheck) {//if the patient has this med, "check the box"
				//			eForm.ListEFormFields[i].ValueString="X";
				//		}
				//	}
				//}
				if(eForm.ListEFormFields[i].DbLink=="MiddleI") {
					eForm.ListEFormFields[i].ValueString=patient.MiddleI;
				}
				if(eForm.ListEFormFields[i].DbLink=="Position") {
					eForm.ListEFormFields[i].ValueString=patient.Position.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferConfirmMethod") {
					eForm.ListEFormFields[i].ValueString=patient.PreferConfirmMethod.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferContactMethod") {
					eForm.ListEFormFields[i].ValueString=patient.PreferContactMethod.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferRecallMethod") {
					eForm.ListEFormFields[i].ValueString=patient.PreferRecallMethod.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="Preferred") {
					eForm.ListEFormFields[i].ValueString=patient.Preferred;
				}
				if(eForm.ListEFormFields[i].DbLink=="problemsNone") {
					if(listDiseasesPat.Count==0) {
						eForm.ListEFormFields[i].ValueString="X";
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="problemsOther") {
					//get list of diseases already covered by checkboxes or radiobuttons
					List<string> listStrDiseasesChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("problem:")).Select(x=>x.DbLink.Substring(8)).ToList();
					List<string> listStrDiseasesToAdd=new List<string>();
					for(int a=0;a<listDiseasesPat.Count;a++) {
						DiseaseDef diseaseDef=DiseaseDefs.GetItem(listDiseasesPat[a].DiseaseDefNum);
						string strDisease=diseaseDef.DiseaseName;
						if(listStrDiseasesChecks.Contains(strDisease)) {
							continue;//this disease is already covered by a checkbox or radiobutton
						}
						if(strDisease.Contains(",")){
							continue;//ignore diseases that patient has that contain commas. It would mess up our serialization.
						}
						if(strDisease=="Other"){
							//The disease attached to patient is "Other", probably from a previous import
							if(listDiseasesPat[a].PatNote.Contains(",")){
								continue;
							}
							listStrDiseasesToAdd.Add(listDiseasesPat[a].PatNote);//this is where we store the actual disease name for the Others.
							continue;
						}
						listStrDiseasesToAdd.Add(strDisease);
					}
					eForm.ListEFormFields[i].ValueString=string.Join(", ",listStrDiseasesToAdd);
				}
				if(eForm.ListEFormFields[i].DbLink.StartsWith("problem:")) {
					string strProblemName=eForm.ListEFormFields[i].DbLink.Substring(8);
					bool hasProblem=false;
					for(int a=0;a<listDiseasesPat.Count;a++){
						DiseaseDef diseaseDef=DiseaseDefs.GetItem(listDiseasesPat[a].DiseaseDefNum);
						//Should never be null, but there might be an edge case I haven't thought of
						if(diseaseDef==null){
							continue;
						}
						string strDiseaseName=diseaseDef.DiseaseName;
						if(strDiseaseName==strProblemName) {
							hasProblem=true;
							break;
						}
					}
					if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
						if(hasProblem){
							eForm.ListEFormFields[i].ValueString="X";
						}
					}
					if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
						if(hasProblem){
							eForm.ListEFormFields[i].ValueString="Y";
						}
						else{
							eForm.ListEFormFields[i].ValueString="N";
						}
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="referredFrom") {
					Referral referral=Referrals.GetReferralForPat(patient.PatNum);
					if(referral!=null){
						eForm.ListEFormFields[i].ValueString=Referrals.GetNameFL(referral.ReferralNum);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="SSN") {
					if(CultureInfo.CurrentCulture.Name=="en-US" && patient.SSN.Length==9){//and length exactly 9 (no data gets lost in formatting)
						eForm.ListEFormFields[i].ValueString=patient.SSN.Substring(0,3)+"-"+patient.SSN.Substring(3,2)+"-"+patient.SSN.Substring(5,4);
					}
					else {
						eForm.ListEFormFields[i].ValueString=patient.SSN;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="State") {
					eForm.ListEFormFields[i].ValueString=patient.State;
				}
				if(eForm.ListEFormFields[i].DbLink=="StateNoValidation") {
					eForm.ListEFormFields[i].ValueString=patient.State;
				}
				if(eForm.ListEFormFields[i].DbLink=="StudentStatus") {
					eForm.ListEFormFields[i].ValueString=patient.StudentStatus.ToString();
				}
				if(eForm.ListEFormFields[i].DbLink=="WirelessPhone") {
					eForm.ListEFormFields[i].ValueString=patient.WirelessPhone;
				}
				if(eForm.ListEFormFields[i].DbLink=="wirelessCarrier") {
					eForm.ListEFormFields[i].ValueString="";//not implemented
				}
				if(eForm.ListEFormFields[i].DbLink=="WkPhone") {
					eForm.ListEFormFields[i].ValueString=patient.WkPhone;
				}
				if(eForm.ListEFormFields[i].DbLink=="Zip") {
					eForm.ListEFormFields[i].ValueString=patient.Zip;
				}
			}
			List<EnumStaticTextField> listEnumStaticTextFields=GetAllStaticTextFieldsForEForm(eForm);
			StaticTextFieldDependency staticTextFieldDependency=StaticTextData.GetStaticTextDependencies(listEnumStaticTextFields);
			List<StaticTextReplacement> listStaticTextReplacements=SheetFiller.GetStaticTextReplacements(listEnumStaticTextFields,patient,family,staticTextData:null,staticTextFieldDependency,aptNum:0);
			ReplaceStaticTextFields(listStaticTextReplacements,eForm,patient,family);
		}

		public static List<EnumStaticTextField> GetAllStaticTextFieldsForEForm(EForm eForm){
			List<EnumStaticTextField> listEnumStaticTextFields=new List<EnumStaticTextField>();
			for(int i=0;i<eForm.ListEFormFields.Count;i++) {
				if(eForm.ListEFormFields[i].FieldType!=EnumEFormFieldType.Label) {
					continue;
				}
				string pattern = @"\["//beginning square bracket
					+@"("//beginning of group
					+@"\w+"//one or more word characters (letters, digits, or underscores)
					+@")"//end of group
					+@"\]";//ending square bracket
				//group 0 is the entire match, including []
				//group 1 is just the part inside (), so without []
				Regex regex=new Regex(pattern);
				MatchCollection matchCollection=regex.Matches(eForm.ListEFormFields[i].ValueLabel);
				for(int m=0;m<matchCollection.Count;m++){
					EnumStaticTextField enumStaticTextField;
					try{
					 enumStaticTextField=(EnumStaticTextField)Enum.Parse(typeof(EnumStaticTextField),matchCollection[m].Groups[1].Value);
					}
					catch{
						continue;
					}
					listEnumStaticTextFields.Add(enumStaticTextField);
				}
			}
			return listEnumStaticTextFields;
		}

		///<summary>Takes a list of replacements and actually performs the replacement within all the Sheet fields.</summary>
		private static void ReplaceStaticTextFields(List<StaticTextReplacement> listStaticTextReplacements,EForm eForm,Patient patient,Family family){
			for(int f=0;f<eForm.ListEFormFields.Count;f++){
				if(eForm.ListEFormFields[f].FieldType!=EnumEFormFieldType.Label) {
					continue;
				}
				for(int i=0;i<listStaticTextReplacements.Count;i++){
					eForm.ListEFormFields[f].ValueLabel=eForm.ListEFormFields[f].ValueLabel.Replace(
						"["+listStaticTextReplacements[i].StaticTextField.ToString()+"]",
						listStaticTextReplacements[i].NewValue);
				}
			}
		}
	}
}
