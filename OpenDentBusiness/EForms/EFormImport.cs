using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	public class EFormImport {
		///<summary>Always succeeds silently.</summary>
		public static void Import(EForm eForm){
			//Any changes to this list should also be duplicated in EFormFiller and EFormFieldsAvailable.
			//This code was copied from FormSheetImport
			Patient patientOld=Patients.GetPat(eForm.PatNum);
			Patient patient=patientOld.Copy();
			PatientNote patientNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor);
			List<Allergy> listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//just active allergies
			List<AllergyDef> listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);//from db. AllergyDefs are oddly not cached.
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
			List<Disease> listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);
			for(int i=0;i<eForm.ListEFormFields.Count;i++) {
				if(eForm.ListEFormFields[i].DbLink==""){
				}
				if(eForm.ListEFormFields[i].DbLink=="Address") {
					patient.Address=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="Address2") {
					patient.Address2=eForm.ListEFormFields[i].ValueString;
				}
				//if(eForm.ListEFormFields[i].DbLink=="addressAndHmPhoneIsSameEntireFamily") {

				//}
				if(eForm.ListEFormFields[i].DbLink=="allergiesNone") {
					//change the StatusIsActive to false since they checked "None" for allergies
					if(listAllergiesPat.Count==0){
						continue;
					}
					if(eForm.ListEFormFields[i].ValueString=="X") {
						for(int a=0;a<listAllergiesPat.Count;a++) {
							listAllergiesPat[a].StatusIsActive=false;
							Allergies.Update(listAllergiesPat[a]);
						}
					}
					listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//refresh if needed for a future loop
				}
				if(eForm.ListEFormFields[i].DbLink=="allergiesOther") {
					List<string> listStrsInput=eForm.ListEFormFields[i].ValueString.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToList();
					//get list of allergies already covered by checkboxes
					List<string> listStrAllergiesChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("allergy:")).Select(x=>x.DbLink.Substring(8)).ToList();
					for(int a=0;a<listStrsInput.Count;a++){
						if(listStrAllergiesChecks.Contains(listStrsInput[a])) {
							continue;//this allergy is already covered by a checkbox
						}
						AllergyDef allergyDef=listAllergyDefs.Find(x=>x.Description.ToLower()==listStrsInput[a].ToLower());
						if(allergyDef==null){
							//this allergy that they typed in does not have any matching allergyDef, so we will use "Other".
							AllergyDef allergyDefOther=listAllergyDefs.Find(x=>x.Description=="Other");
							if(allergyDefOther==null){//this will only happen once, and then all patients will reuse it for years.
								allergyDefOther=new AllergyDef();
								allergyDefOther.Description="Other";
								AllergyDefs.Insert(allergyDefOther);
								listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);
								//DataValid.SetInvalid would be used instead if this was cached.
							}
							//allergyDefOther is now guaranteed to be valid
							Allergy allergyOther=listAllergiesPat.Find(x=>x.AllergyDefNum==allergyDefOther.AllergyDefNum && x.Reaction==listStrsInput[a]);
							if(allergyOther==null){
								//This patient does not have an active "Other" allergy representing the allergy that was typed in
								allergyOther=new Allergy();
								allergyOther.PatNum=patient.PatNum;
								allergyOther.AllergyDefNum=allergyDefOther.AllergyDefNum;
								allergyOther.Reaction=listStrsInput[a];
								allergyOther.StatusIsActive=true;
								Allergies.Insert(allergyOther);
								listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//refresh if needed for a future loop
								continue;
							}
							//This patient already has an active "Other" allergy representing the allergy that was typed in
							//so db will not need to change.
							continue;
						}
						//we found an allergyDef with the exact spelling that user typed in.
						Allergy allergyMatch=listAllergiesPat.Find(x=>x.AllergyDefNum==allergyDef.AllergyDefNum);
						if(allergyMatch==null){
							//But this patient does not have it attached as one of their allergies
							allergyMatch=new Allergy();
							allergyMatch.PatNum=patient.PatNum;
							allergyMatch.AllergyDefNum=allergyDef.AllergyDefNum;
							allergyMatch.StatusIsActive=true;
							Allergies.Insert(allergyMatch);
							listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//refresh if needed for a future loop
							continue;
						}
						//This patient already has an active allergy matching the allergy that was typed in
						//so db will not need to change.
					}
				}
				if(eForm.ListEFormFields[i].DbLink.StartsWith("allergy:")){
					string strAllergyCheck=eForm.ListEFormFields[i].DbLink.Substring(8);
					AllergyDef allergyDef=listAllergyDefs.Find(x=>x.Description.ToLower()==strAllergyCheck.ToLower());
					if(allergyDef==null){
						//This shouldn't normally happen, but it certainly is possible because user could change an allergyDef name after adding the checkbox,
						//or this could be an imported form with no allergyDef in the db representing the value in this checkbox.
						//We will just add this for them. That way, all imported forms will correctly work.
						allergyDef=new AllergyDef();
						allergyDef.Description=strAllergyCheck;
						AllergyDefs.Insert(allergyDef);
						listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);
						//DataValid.SetInvalid would be used instead if this was cached.
					}
					//allergyDef is now guaranteed to have a value
					Allergy allergyPat=listAllergiesPat.Find(x=>x.AllergyDefNum==allergyDef.AllergyDefNum);
					if(allergyPat==null){
						//This patient does not have an active allergy matching the allergy checked.
						//Notice that we do not look at inactive allergies and consider flipping them back to active.
						//Once someone has an allergy, it doesn't go away. We don't need to consider that fancy edge case.
						//The worst consequence is that they might end up with one active and one inactive. No big deal.
						if(eForm.ListEFormFields[i].ValueString=="X"){
							allergyPat=new Allergy();
							allergyPat.PatNum=patient.PatNum;
							allergyPat.AllergyDefNum=allergyDef.AllergyDefNum;
							allergyPat.StatusIsActive=true;
							Allergies.Insert(allergyPat);
							listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//refresh if needed for a future loop
						}
						else{
							//db is already accurate
						}
					}
					else{
						//This patient does have an active allergy matching the allergy checked
						if(eForm.ListEFormFields[i].ValueString=="X"){
							//db is already accurate
						}
						else{
							allergyPat.StatusIsActive=false;//make it inactive
							Allergies.Update(allergyPat);
							listAllergiesPat=Allergies.GetAll(eForm.PatNum,showInactive:false);//refresh if needed for a future loop
						}
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="Birthdate") {
					patient.Birthdate=PIn.Date(eForm.ListEFormFields[i].ValueString);
				}
				if(eForm.ListEFormFields[i].DbLink=="City") {
					patient.City=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="Email") {
					patient.Email=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="FName") {
					patient.FName=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="Gender") {
					if(eForm.ListEFormFields[i].ValueString!=""){
						patient.Gender=(PatientGender)Enum.Parse(typeof(PatientGender),eForm.ListEFormFields[i].ValueString);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="HmPhone") {
					patient.HmPhone=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="ICEName") {
					patientNote.ICEName=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="ICEPhone") {
					patientNote.ICEPhone=eForm.ListEFormFields[i].ValueString;
				}
					/*
				//We're not even going to try to import insurance at this point
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
				}*/
				if(eForm.ListEFormFields[i].DbLink=="LName") {
					patient.LName=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList) {
					EFormMedListLayout eFormMedListLayout=JsonConvert.DeserializeObject<EFormMedListLayout>(eForm.ListEFormFields[i].ValueLabel);
					if(eFormMedListLayout.ImportCol1){
						List<EFormMed> listEFormMeds=new List<EFormMed>();
						if(!String.IsNullOrEmpty(eForm.ListEFormFields[i].ValueString)){
							listEFormMeds=JsonConvert.DeserializeObject<List<EFormMed>>(eForm.ListEFormFields[i].ValueString);
						}
						//Remove meds from patient that are not in the import list.
						bool isRemoved=false;
						for(int a=0;a<listMedicationPats.Count;a++){
							Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
							string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
							if(medication!=null){
								strMed=medication.MedName;
							}
							EFormMed eFormMed=listEFormMeds.Find(x=>x.MedName==strMed);
							if(eFormMed!=null){//in the import list
								continue;
							}
							listMedicationPats[a].DateStop=DateTime.Today.AddDays(-1);
							MedicationPats.Update(listMedicationPats[a]);
							isRemoved=true;
						}
						if(isRemoved){
							listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
						}
						//Add any missing meds to patient
						//If the med is already present, import the freq & quant.
						for(int m=0;m<listEFormMeds.Count;m++){
							MedicationPat medicationPat=null;
							for(int a=0;a<listMedicationPats.Count;a++){
								Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
								string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
								if(medication!=null){
									strMed=medication.MedName;
								}
								if(strMed==listEFormMeds[m].MedName){
									medicationPat=listMedicationPats[a];
									break;
								}
							}
							if(medicationPat!=null){
								//already present, but we need to import col2
								bool imported2=false;
								if(eFormMedListLayout.ImportCol2AppendDate){
									if(medicationPat.PatNote!=""){
										medicationPat.PatNote+="\r\n";
									}
									medicationPat.PatNote+=DateTime.Today.ToShortDateString()+"-"+listEFormMeds[m].StrengthFreq;
									imported2=true;
								}
								if(eFormMedListLayout.ImportCol2OverwriteDate){
									medicationPat.PatNote=DateTime.Today.ToShortDateString()+"-"+listEFormMeds[m].StrengthFreq;
									imported2=true;
								}
								if(eFormMedListLayout.ImportCol2Append){
									if(medicationPat.PatNote!=""){
										medicationPat.PatNote+="\r\n";
									}
									medicationPat.PatNote+=listEFormMeds[m].StrengthFreq;
									imported2=true;
								}
								if(eFormMedListLayout.ImportCol2Overwrite){
									medicationPat.PatNote=listEFormMeds[m].StrengthFreq;
									imported2=true;
								}
								if(imported2){
									MedicationPats.Update(medicationPat);
									listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
								}
								continue;
							}
							//missing in pat
							medicationPat=new MedicationPat();
							medicationPat.PatNum=patient.PatNum;
							medicationPat.MedDescript=listEFormMeds[m].MedName;
							if(eFormMedListLayout.ImportCol2AppendDate
								|| eFormMedListLayout.ImportCol2OverwriteDate)
							{
								medicationPat.PatNote=DateTime.Today.ToShortDateString()+"-"+listEFormMeds[m].StrengthFreq;
							}
							if(eFormMedListLayout.ImportCol2Append
								|| eFormMedListLayout.ImportCol2Overwrite)
							{
								medicationPat.PatNote=listEFormMeds[m].StrengthFreq;
							}
							MedicationPats.Insert(medicationPat);
							listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
						}
						//for(int a=0;a<listMedicationPats.Count;a++){
						//	Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
						//	string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
						//	if(medication!=null){
						//		strMed=medication.MedName;
						//	}
						//	EFormMed eFormMed=new EFormMed();
						//	eFormMed.MedName=strMed;
						//	if(eFormMedListLayout.PrefillCol2){
						//		eFormMed.StrengthFreq=listMedicationPats[a].PatNote;
						//	}
						//	listEFormMeds.Add(eFormMed);
						//}
						//eForm.ListEFormFields[i].ValueString=JsonConvert.SerializeObject(listEFormMeds);
					}
				}
				/*
				//if(eForm.ListEFormFields[i].DbLink=="medsNone") {
				//	//change the date discontinued since they checked "None" for meds
				//	if(listMedicationPats.Count==0){
				//		continue;
				//	}
				//	if(eForm.ListEFormFields[i].ValueString=="X") {
				//		for(int a=0;a<listMedicationPats.Count;a++) {
				//			listMedicationPats[a].DateStop=DateTime.Today.AddDays(-1);//yesterday so it shows as discontinued
				//			MedicationPats.Update(listMedicationPats[a]);
				//		}
				//	}
				//	listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
				//}*/
				/*if(eForm.ListEFormFields[i].DbLink=="medsOther") {
					List<string> listStrsInput=eForm.ListEFormFields[i].ValueString.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToList();
					//get list of meds already covered by checkboxes
					List<string> listStrMedsChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("med:")).Select(x=>x.DbLink.Substring(4)).ToList();
					for(int a=0;a<listStrsInput.Count;a++){
						if(listStrMedsChecks.Contains(listStrsInput[a])) {
							continue;//this med is already covered by a checkbox
						}
						Medication medication=Medications.GetFirstOrDefault(x=>x.MedName.ToLower()==listStrsInput[a].ToLower());
						if(medication==null){
							//This will be very common.
							//The description that they typed in does not have any matching med in the master list.
							//Unlike allergies and problems, meds don't use a dummy "Other" med.
							//Instead, we leave MedicationNum=0, and set MedDescript.
							MedicationPat medicationPat=listMedicationPats.Find(x=>x.MedDescript==listStrsInput[a]);
							if(medicationPat==null){
								//This patient does not have an active med representing the med that was typed in
								medicationPat=new MedicationPat();
								medicationPat.PatNum=patient.PatNum;
								medicationPat.MedDescript=listStrsInput[a];
								MedicationPats.Insert(medicationPat);
								listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
								continue;
							}
							//This patient already has an active med matching the med that was typed in,
							//so db will not need to change.
							continue;
						}
						//we found a medication with the exact spelling that user typed in.
						MedicationPat medicationPatMatch=listMedicationPats.Find(x=>x.MedicationNum==medication.MedicationNum);
						if(medicationPatMatch==null){
							//But this patient does not have it attached as one of their meds
							medicationPatMatch=new MedicationPat();
							medicationPatMatch.PatNum=patient.PatNum;
							medicationPatMatch.MedicationNum=medication.MedicationNum;
							MedicationPats.Insert(medicationPatMatch);
							listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
							continue;
						}
						//This patient already has an active med matching the med that was typed in
						//so db will not need to change.
					}
				}*/
				/*if(eForm.ListEFormFields[i].DbLink.StartsWith("med:")){
					string strMedicationPatCheck=eForm.ListEFormFields[i].DbLink.Substring(4);
					Medication medication=Medications.GetFirstOrDefault(x=>x.MedName.ToLower()==strMedicationPatCheck.ToLower());
					if(medication==null){
						//This shouldn't normally happen, but it certainly is possible because user could change a medication name after adding the checkbox,
						//or this could be an imported form with no medication in the db representing the value in this checkbox.
						//Because this was added by a dental office user instead of a patient, we treat it as official and accurate.
						//We will just add this for them. That way, all imported forms will correctly work.
						medication=new Medication();
						medication.MedName=strMedicationPatCheck;
						Medications.Insert(medication);
						medication.GenericNum=medication.MedicationNum;//quirk of meds
						Medications.Update(medication);
						Signalods.SetInvalid(InvalidType.Medications);
						Medications.RefreshCache();
					}
					//medication is now guaranteed to have a value
					MedicationPat medicationPat=listMedicationPats.Find(x=>x.MedicationNum==medication.MedicationNum);
					if(medicationPat==null){
						//This patient does not have an active med matching the med checked and the master list.
						//But they might have a medicationPat with  MedicationNum=0 and a matching MedDescript.
						medicationPat=listMedicationPats.Find(x=>x.MedDescript.ToLower()==strMedicationPatCheck.ToLower());
						if(medicationPat==null){
							//This patient does not have any active med matching the med checked.
							//Notice that we do not look at inactive meds and consider flipping them back to active.
							//We don't need to consider that fancy edge case.
							//The worst consequence is that they might end up with one active and one inactive. No big deal.
							if(eForm.ListEFormFields[i].ValueString=="X"){
								medicationPat=new MedicationPat();
								medicationPat.PatNum=patient.PatNum;
								//We do link to an official medication instead of using MedDescript because this is a checkbox, not a typed med.
								medicationPat.MedicationNum=medication.MedicationNum;
								MedicationPats.Insert(medicationPat);
								listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
							}
							else{
								//db is already accurate
							}
						}
						else{
							//This patient does have an active med matching the med checked
							if(eForm.ListEFormFields[i].ValueString=="X"){
								//db is already accurate
							}
							else{
								medicationPat.DateStop=DateTime.Today.AddDays(-1);//yesterday so it shows as discontinued
								MedicationPats.Update(medicationPat);
								listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
							}
						}
					}
					else{
						//This patient does have an active med matching the med checked
						if(eForm.ListEFormFields[i].ValueString=="X"){
							//db is already accurate
						}
						else{
							medicationPat.DateStop=DateTime.Today.AddDays(-1);//yesterday so it shows as discontinued
							MedicationPats.Update(medicationPat);
							listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);//refresh if needed for a future loop
						}
					}
				}*/
				if(eForm.ListEFormFields[i].DbLink=="MiddleI") {
					patient.MiddleI=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="Position") {
					if(eForm.ListEFormFields[i].ValueString!=""){
						patient.Position=(PatientPosition)Enum.Parse(typeof(PatientPosition),eForm.ListEFormFields[i].ValueString);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferConfirmMethod") {
					if(eForm.ListEFormFields[i].ValueString!=""){
						patient.PreferConfirmMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),eForm.ListEFormFields[i].ValueString);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferContactMethod") {
					if(eForm.ListEFormFields[i].ValueString!=""){
						patient.PreferContactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),eForm.ListEFormFields[i].ValueString);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="PreferRecallMethod") {
					if(eForm.ListEFormFields[i].ValueString!=""){
						patient.PreferRecallMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),eForm.ListEFormFields[i].ValueString);
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="Preferred") {
					patient.Preferred=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="problemsNone") {
					//change to inactive since they checked "None" for problems
					if(listDiseasesPat.Count==0){
						continue;
					}
					if(eForm.ListEFormFields[i].ValueString=="X") {
						for(int a=0;a<listDiseasesPat.Count;a++) {
							listDiseasesPat[a].ProbStatus=ProblemStatus.Inactive;
							listDiseasesPat[a].DateStop=DateTime.Today.AddDays(-1);//yesterday
							Diseases.Update(listDiseasesPat[a]);
						}
					}
					listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);//refresh if needed for a future loop
				}
				if(eForm.ListEFormFields[i].DbLink=="problemsOther") {
					List<string> listStrsInput=eForm.ListEFormFields[i].ValueString.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToList();
					//get list of problems already covered by checkboxes
					List<string> listStrProbsChecks=eForm.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("problem:")).Select(x=>x.DbLink.Substring(8)).ToList();
					for(int a=0;a<listStrsInput.Count;a++){
						if(listStrProbsChecks.Contains(listStrsInput[a])) {
							continue;//this problem is already covered by a checkbox
						}
						DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName.ToLower()==listStrsInput[a].ToLower());
						if(diseaseDef==null){
							//the disease that they typed in does not have any matching diseaseDef, so we will use "Other".
							DiseaseDef diseaseDefOther=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName=="Other");
							if(diseaseDefOther==null){//this will only happen once, and then all patients will reuse it for years.
								diseaseDefOther=new DiseaseDef();
								diseaseDefOther.DiseaseName="Other";
								DiseaseDefs.Insert(diseaseDefOther);
								Signalods.SetInvalid(InvalidType.Diseases);
								DiseaseDefs.RefreshCache();
							}
							//diseaseDefOther is now guaranteed to be valid
							Disease diseaseOther=listDiseasesPat.Find(x=>x.DiseaseDefNum==diseaseDefOther.DiseaseDefNum && x.PatNote==listStrsInput[a]);
							if(diseaseOther==null){
								//This patient does not have an active "Other" disease representing the disease that was typed in
								diseaseOther=new Disease();
								diseaseOther.PatNum=patient.PatNum;
								diseaseOther.DiseaseDefNum=diseaseDefOther.DiseaseDefNum;
								diseaseOther.PatNote=listStrsInput[a];
								diseaseOther.ProbStatus=ProblemStatus.Active;
								Diseases.Insert(diseaseOther);
								listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);//refresh if needed for a future loop
								continue;
							}
							//This patient already has an active "Other" disease representing the disease that was typed in
							//so db will not need to change.
							continue;
						}
						//we found a disease with the exact spelling that user typed in.
						Disease diseaseMatch=listDiseasesPat.Find(x=>x.DiseaseDefNum==diseaseDef.DiseaseDefNum);
						if(diseaseMatch==null){
							//But this patient does not have it attached as one of their diseases
							diseaseMatch=new Disease();
							diseaseMatch.PatNum=patient.PatNum;
							diseaseMatch.DiseaseDefNum=diseaseDef.DiseaseDefNum;
							diseaseMatch.ProbStatus=ProblemStatus.Active;
							Diseases.Insert(diseaseMatch);
							listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);//refresh if needed for a future loop
							continue;
						}
						//This patient already has an active disease matching the disease that was typed in
						//so db will not need to change.
					}
				}
				if(eForm.ListEFormFields[i].DbLink.StartsWith("problem:")){
					string strDiseaseCheck=eForm.ListEFormFields[i].DbLink.Substring(8);
					DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName.ToLower()==strDiseaseCheck.ToLower());
					if(diseaseDef==null){
						//This shouldn't normally happen, but it certainly is possible because user could change a diseaseDef name after adding the checkbox,
						//or this could be an imported form with no diseaseDef in the db representing the value in this checkbox.
						//We will just add this for them. That way, all imported forms will correctly work.
						diseaseDef=new DiseaseDef();
						diseaseDef.DiseaseName=strDiseaseCheck;
						DiseaseDefs.Insert(diseaseDef);
						Signalods.SetInvalid(InvalidType.Diseases);
						DiseaseDefs.RefreshCache();
					}
					//diseaseDef is now guaranteed to have a value
					Disease diseasePat=listDiseasesPat.Find(x=>x.DiseaseDefNum==diseaseDef.DiseaseDefNum);
					if(diseasePat==null){
						//This patient does not have an active disease matching the disease checked.
						//Notice that we do not look at inactive diseases and consider flipping them back to active.
						//We don't need to consider that fancy edge case.
						//The worst consequence is that they might end up with one active and one inactive. No big deal.
						if(eForm.ListEFormFields[i].ValueString=="X"){
							diseasePat=new Disease();
							diseasePat.PatNum=patient.PatNum;
							diseasePat.DiseaseDefNum=diseaseDef.DiseaseDefNum;
							diseasePat.ProbStatus=ProblemStatus.Active;
							Diseases.Insert(diseasePat);
							listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);//refresh if needed for a future loop
						}
						else{
							//db is already accurate
						}
					}
					else{
						//This patient does have an active disease matching the disease checked
						if(eForm.ListEFormFields[i].ValueString=="X"){
							//db is already accurate
						}
						else{
							diseasePat.ProbStatus=ProblemStatus.Inactive;
							diseasePat.DateStop=DateTime.Today.AddDays(-1);//yesterday
							Diseases.Update(diseasePat);
							listDiseasesPat=Diseases.Refresh(patient.PatNum,showActiveOnly:true);//refresh if needed for a future loop
						}
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="referredFrom") {
					Referral referral=Referrals.GetOther();
					RefAttach refAttach=new RefAttach();
					refAttach.RefType=ReferralType.RefFrom;
					refAttach.ItemOrder=1;
					refAttach.PatNum=patient.PatNum;
					refAttach.RefDate=DateTime.Today;
					refAttach.ReferralNum=referral.ReferralNum;
					refAttach.Note=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="SSN") {
					if(CultureInfo.CurrentCulture.Name=="en-US"){
						patient.SSN=eForm.ListEFormFields[i].ValueString.Replace("-","");//strip any dashes
					}
					else{
						patient.SSN=eForm.ListEFormFields[i].ValueString;
					}
				}
				if(eForm.ListEFormFields[i].DbLink=="State") {
					patient.State=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="StateNoValidation") {
					patient.State=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="StudentStatus") {
					patient.StudentStatus=eForm.ListEFormFields[i].ValueString;//N, P, or F
				}
				if(eForm.ListEFormFields[i].DbLink=="WirelessPhone") {
					patient.WirelessPhone=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="WkPhone") {
					patient.WkPhone=eForm.ListEFormFields[i].ValueString;
				}
				if(eForm.ListEFormFields[i].DbLink=="Zip") {
					patient.Zip=eForm.ListEFormFields[i].ValueString;
				}
			}
			Patients.Update(patient,patientOld);
			PatientNotes.Update(patientNote,patient.Guarantor);
			for(int i=0;i<listAllergiesPat.Count;i++) {
				Allergies.Update(listAllergiesPat[i]);
			}
		}
	}
}
