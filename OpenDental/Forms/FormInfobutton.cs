using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;
using System.Xml.XPath;
using System.IO;
using OpenDental.UI;
using System.Net;

namespace OpenDental {
	public partial class FormInfobutton:FormODBase {
		public Patient PatientCur;
		///<summary>Usually filled from within the form by using Patcur.PriProv</summary>
		public Provider ProviderCur;
		///<summary>Knowledge request objects, possible object types are: DiseaseDef, Medication, LabResult, ICD9, Icd10, Snomed, RxNorm, Loinc, or LabResult.  Should not break if unsupported objects are in list.</summary>
		private List<KnowledgeRequest> _listKnowledgeRequests;
		//public List<DiseaseDef> ListProblems;//should this be named disease or problem? Also snomed/medication
		//public List<Medication> ListMedications;
		//public List<LabResult> ListLabResults;
		/////<summary>Used to add various codes that are not explicitly related to a problem, medication, or allergy.</summary>
		//public List<Snomed> ListSnomed;
		private ActTaskCode _actTaskCode;//may need to make this public later.
		public ObservationInterpretationNormality ObservationInterpretationNormality_;
		public ActEncounterCode ActEncounterCode_;
		public bool UseAge;
		public bool UseAgeGroup;
		public bool IsPerformerProvider;
		public bool IsRecipientProvider;
		private CultureInfo[] _cultureInfoArray;

		public FormInfobutton(List<KnowledgeRequest> listKnowledgeRequests=null) {
			if(listKnowledgeRequests==null){
				_listKnowledgeRequests=new List<KnowledgeRequest>();
			}
			else {
				_listKnowledgeRequests=listKnowledgeRequests;
			}
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInfobutton_Load(object sender,EventArgs e) {
			FillLanguageCombos();
			FillEncounterCombo();
			FillTaskCombo();
			FillContext();
			FillKnowledgeRequestitems();
		}

		private void FillLanguageCombos() {
			_cultureInfoArray = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			for(int i=0;i<_cultureInfoArray.Length;i++) {
				comboPatLang.Items.Add(_cultureInfoArray[i].DisplayName);
				comboProvLang.Items.Add(_cultureInfoArray[i].DisplayName);
			}
		}

		private void FillEncounterCombo() {
			//for(int i=0;i<Enum.GetValues(typeof(ActEncounterCode)).Length;i++){
			//  comboEncType.Items.Add(Enum.GetName(typeof(ActEncounterCode),i));
			//}
			comboEncType.Items.Add("ambulatory");
			comboEncType.Items.Add("emergency");
			comboEncType.Items.Add("field");
			comboEncType.Items.Add("home health");
			comboEncType.Items.Add("inpatient encounter");
			comboEncType.Items.Add("short stay");
			comboEncType.Items.Add("virtual");
		}

		private void FillTaskCombo() {
			comboTask.Items.Add("Order Entry");
			comboTask.Items.Add("Patient Documentation");
			comboTask.Items.Add("Patient Information Review");
		}

		private void FillContext() {
			//Fill Patient-------------------------------------------------------------------------------------------------------------------
			if(PatientCur!=null) {
				textPatName.Text=PatientCur.GetNameFL();
				if(PatientCur.Birthdate!=DateTime.MinValue) {
					textPatBirth.Text=PatientCur.Birthdate.ToShortDateString();
				}
				comboPatLang.SelectedIndex=comboPatLang.Items.IndexOf(System.Globalization.CultureInfo.CurrentCulture.DisplayName);
				switch(PatientCur.Gender) {
					case PatientGender.Female:
						radioPatGenFem.Checked=true;
						break;
					case PatientGender.Male:
						radioPatGenMale.Checked=true;
						break;
					case PatientGender.Unknown:
					default:
						radioPatGenUn.Checked=true;
						break;
				}
			}
			//Fill Provider------------------------------------------------------------------------------------------------------------------
			if(ProviderCur==null && PatientCur!=null) {
				ProviderCur=Providers.GetProv(PatientCur.PriProv);
			}
			if(ProviderCur==null) {
				ProviderCur=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			}
			if(ProviderCur!=null) {
				textProvName.Text=ProviderCur.GetFormalName();
				textProvID.Text=ProviderCur.NationalProvID;
				comboProvLang.SelectedIndex=comboPatLang.Items.IndexOf(System.Globalization.CultureInfo.CurrentCulture.DisplayName);
			}
			//Fill Organization--------------------------------------------------------------------------------------------------------------
			textOrgName.Text=PrefC.GetString(PrefName.PracticeTitle);
			//Fill Encounter-----------------------------------------------------------------------------------------------------------------
			ActEncounterCode_=ActEncounterCode.AMB;
			comboEncType.SelectedIndex=(int)ActEncounterCode_;//ambulatory
			if(PatientCur!=null) {
				textEncLocID.Text=PatientCur.ClinicNum.ToString();//do not use to generate message if this value is zero.
			}
			//Fill Requestor/Recievor--------------------------------------------------------------------------------------------------------
			radioReqProv.Checked=IsPerformerProvider;
			radioReqPat.Checked=!IsPerformerProvider;
			radioRecProv.Checked=IsRecipientProvider;
			radioRecPat.Checked=!IsRecipientProvider;
			//Fill Task Type-----------------------------------------------------------------------------------------------------------------
			_actTaskCode=ActTaskCode.PATINFO;//may need to change this later.
			comboTask.SelectedIndex=(int)_actTaskCode;
		}

		private void FillKnowledgeRequestitems() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Type",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Code",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("CodeSystem",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",80);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listKnowledgeRequests.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listKnowledgeRequests[i].Type);
				row.Cells.Add(_listKnowledgeRequests[i].Code);
				row.Cells.Add(_listKnowledgeRequests[i].GetCodeSystemDisplay());
				row.Cells.Add(_listKnowledgeRequests[i].Description);
				row.Tag=_listKnowledgeRequests[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butPreview_Click(object sender,EventArgs e) {
			if(!IsValidHL7DataSet()) {
				return;
			}
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(GenerateKnowledgeRequestNotification());
			msgBoxCopyPaste.ShowDialog();
		}

		///<summary>Generates message box with all errors. Returns true if data passes validation or if user decides to "continue anyways".</summary>
		private bool IsValidHL7DataSet() {
			string warnings="";//additional data that could be used but is not neccesary.
			string errors="";//additional data that must be present in order to be compliant.
			string message="";
			string bullet="  - ";//should be used at the beggining of every warning/error
			//Patient information-------------------------------------------------------------------------------------------------
			if(PatientCur==null) {//should never happen
				warnings+=bullet+Lan.g(this,"No patient selected.")+"\r\n";
			}
			else {
				try {
					PatientCur.Birthdate=PIn.Date(textPatBirth.Text);
				}
				catch {

					warnings+=bullet+Lan.g(this,"Birthday.")+"\r\n";
				}
				if(PatientCur.Birthdate==DateTime.MinValue) {
					warnings+=bullet+Lan.g(this,"Patient does not have a valid birthday.")+"\r\n";
				}
			}
			//Provider information------------------------------------------------------------------------------------------------
			if(ProviderCur==null) {
				warnings+=bullet+Lan.g(this,"No provider selected.")+"\r\n";
			}
			else {
				if(textProvID.Text=="") {
					warnings+=bullet+Lan.g(this,"No povider ID.")+"\r\n";
				}
			}
			//Organization information--------------------------------------------------------------------------------------------
			if(textOrgName.Text=="") {
				warnings+=bullet+Lan.g(this,"No organization name.")+"\r\n";
			}
			if(textOrgID.Text=="") {
				warnings+=bullet+Lan.g(this,"No organization ID.")+"\r\n";
			}
			//Encounter information-----------------------------------------------------------------------------------------------
			if(textEncLocID.Text=="") {
				warnings+=bullet+Lan.g(this,"No encounter location ID.")+"\r\n";
			}
			//Requestor information-----------------------------------------------------------------------------------------------
			if(radioReqPat.Checked && radioRecProv.Checked) {
				warnings+=bullet+Lan.g(this,"It is uncommon for the requestor to be the patient and the recipient to be the provider.")+"\r\n";
			}
			//Recipient information-----------------------------------------------------------------------------------------------
			//Problem, Medication, Lab Result information-------------------------------------------------------------------------
			//switch(""){//tabControl1.SelectedTab.Name) {
			//	case "tabProblem"://------------------------------------------------------------------------------------------------
			//		if(ProblemCur==null) {
			//			errors+=bullet+Lan.g(this,"No problem is selected.")+"\r\n";
			//		}
			//		else {
			//			if(textProbSnomedCode.Text=="") {
			//				errors+=bullet+Lan.g(this,"No SNOMED CT problem code.")+"\r\n";
			//				break;
			//			}
			//			if(textProbSnomedCode.Text!=ProblemCur.SnomedCode) {
			//				warnings+=bullet+Lan.g(this,"SNOMED CT problem code has been manualy altered.")+"\r\n";
			//			}
			//			if(Snomeds.GetByCode(textProbSnomedCode.Text)==null) {
			//				errors+=bullet+Lan.g(this,"SNOMED CT problem code does not exist in database.")+"\r\n";
			//			}
			//		}
			//		break;
			//	case "tabMedication"://---------------------------------------------------------------------------------------------
			//		if(MedicationCur==null) {
			//			errors+=bullet+Lan.g(this,"No medication is selected.")+"\r\n";
			//		}
			//		else {
			//			if(textMedSnomedCode.Text=="") {
			//				errors+=bullet+Lan.g(this,"No SNOMED CT medication code.")+"\r\n";
			//			}
			//			//if(textProbSnomedCode.Text!=MedicationCur.SnomedCode) {
			//			//  warnings+=bullet+Lan.g(this,"SNOMED CT medication code has been manualy altered.")+"\r\n";
			//			//}
			//		}
			//		break;
			//	case "tabLabResult"://----------------------------------------------------------------------------------------------
			//		if(LabCur==null) {
			//			errors+=bullet+Lan.g(this,"No lab result is selected.")+"\r\n";
			//		}
			//		else {
			//			if(textMedSnomedCode.Text=="") {
			//				errors+=bullet+Lan.g(this,"No SNOMED CT lab result code.")+"\r\n";
			//			}
			//			//if(textProbSnomedCode.Text!=LabCur.SnomedCode) {
			//			//  warnings+=bullet+Lan.g(this,"SNOMED CT lab result code has been manualy altered.")+"\r\n";
			//			//}
			//		}
			//		break;
			//	default://----------------------------------------------------------------------------------------------------------
			//		errors+=bullet+Lan.g(this,"Problem, medication, or lab result not selected.")+"\r\n";
			//		break;
			//}
			//Generate messagebox-------------------------------------------------------------------------------------------------
			if(errors!="") {
				message+=Lan.g(this,"The following errors must be corrected in order to comply with HL7 standard:")+"\r\n";
				message+=errors;
				message+="\r\n";
			}
			if(warnings!="") {
				message+=Lan.g(this,"Fixing the following warnings may provide better knowledge request results:")+"\r\n";
				message+=warnings;
				message+="\r\n";
			}
			if(message!="") {
				message+=Lan.g(this,"Would you like to continue anyways?");
				if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return false;
				}
			}
			return true;
		}

		private string GenerateKnowledgeRequestNotification() {
//		KnowledgeRequestNotification.KnowledgeRequestNotification krn = new KnowledgeRequestNotification.KnowledgeRequestNotification();
//		if(ODBuild.IsDebug()) {
//			krn.subject4List.Add(
//				new KnowledgeRequestNotification.Subject3(
//					new KnowledgeRequestNotification.Value("191166001","2.16.840.1.113883.6.96","SNOMEDCT","[X]Megaloblastic anemia NOS (disorder)"
//			)));
//			krn.subject4List[0].mainSearchCriteria.originalText="Anemia";
//			return krn.ToXml();
//		}
			//old code below this line.
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				xmlWriter.WriteWhitespace("\r\n");
				xmlWriter.WriteStartElement("knowledgeRequestNotification");
					xmlWriter.WriteAttributeString("classCode","ACT");
					xmlWriter.WriteAttributeString("moodCode","DEF");
					xmlWriter.WriteStartElement("id");
						xmlWriter.WriteAttributeString("value",KnowledgeRequestIDHelper());
						xmlWriter.WriteAttributeString("assigningAuthority",OIDInternals.GetForType(IdentifierType.Root).IDRoot);
					xmlWriter.WriteEndElement();//id
					xmlWriter.WriteStartElement("effectiveTime");
						xmlWriter.WriteAttributeString("value",DateTime.Now.ToString("yyyyMMddhhmmss"));
					xmlWriter.WriteEndElement();//effectiveTime
					xmlWriter.WriteStartElement("subject1");
						xmlWriter.WriteAttributeString("typeCode","SBJ");
						xmlWriter.WriteStartElement("patientContext");
							xmlWriter.WriteAttributeString("classCode","PAT");
							xmlWriter.WriteStartElement("patientPerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
								xmlWriter.WriteStartElement("administrativeGenderCode");
									xmlWriter.WriteAttributeString("code",AdministrativeGenderCodeHelper(PatientCur.Gender));
									xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.5.1");
									xmlWriter.WriteAttributeString("codeSystemName","administrativeGender");
									xmlWriter.WriteAttributeString("displayName",AdministrativeGenderNameHelper(PatientCur.Gender));
								xmlWriter.WriteEndElement();//administrativeGenderCode
							xmlWriter.WriteEndElement();//patientPerson
						if(PatientCur.Birthdate!=DateTime.MinValue){
							xmlWriter.WriteStartElement("subjectOf");
								xmlWriter.WriteAttributeString("typeCode","SBJ");
							if(UseAge || UseAge==UseAgeGroup) {//if true or both are false; field is required.
								xmlWriter.WriteStartElement("age");
									xmlWriter.WriteAttributeString("classCode","OBS");
									xmlWriter.WriteAttributeString("moodCode","DEF");
									xmlWriter.WriteStartElement("code");
										xmlWriter.WriteAttributeString("code","30525-0");
										xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.6.1");
										xmlWriter.WriteAttributeString("codeSystemName","LN");
										xmlWriter.WriteAttributeString("displayName","AGE");
									xmlWriter.WriteEndElement();//code
									xmlWriter.WriteStartElement("value");
										xmlWriter.WriteAttributeString("value",PatientCur.Age.ToString());
										xmlWriter.WriteAttributeString("unit","a");
									xmlWriter.WriteEndElement();//value
								xmlWriter.WriteEndElement();//age
							}
							if(UseAgeGroup || UseAge==UseAgeGroup) {//if true or both are false; field is required.
								xmlWriter.WriteStartElement("ageGroup");
									xmlWriter.WriteAttributeString("classCode","OBS");
									xmlWriter.WriteAttributeString("moodCode","DEF");
									xmlWriter.WriteStartElement("code");
										xmlWriter.WriteAttributeString("code","46251-5");
										xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.6.1");
										xmlWriter.WriteAttributeString("codeSystemName","LN");
										xmlWriter.WriteAttributeString("displayName","Age Groups");
									xmlWriter.WriteEndElement();//code
									xmlWriter.WriteStartElement("value");
										xmlWriter.WriteAttributeString("code",AgeGroupCodeHelper(PatientCur.Birthdate));
										xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.6.177");
										xmlWriter.WriteAttributeString("codeSystemName","MSH");
										xmlWriter.WriteAttributeString("displayName",AgeGroupNameHelper(PatientCur.Birthdate));
									xmlWriter.WriteEndElement();//value
								xmlWriter.WriteEndElement();//ageGroup
							}
							xmlWriter.WriteEndElement();//subjectOf
						}
						xmlWriter.WriteEndElement();//patientContext
					xmlWriter.WriteEndElement();//subject1
					xmlWriter.WriteStartElement("holder");
						xmlWriter.WriteAttributeString("typeCode","HLD");
						xmlWriter.WriteStartElement("assignedEntity");
							xmlWriter.WriteAttributeString("classCode","ASSIGNED");
							xmlWriter.WriteStartElement("name");
								xmlWriter.WriteString(Security.CurUser.UserName);
							xmlWriter.WriteEndElement();//name
							xmlWriter.WriteStartElement("certificateText");
								xmlWriter.WriteString(Security.CurUser.PasswordHash);
							xmlWriter.WriteEndElement();//certificateText
							xmlWriter.WriteStartElement("assignedAuthorizedPerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
							if(textProvID.Text!=""){
								xmlWriter.WriteStartElement("id");
									xmlWriter.WriteAttributeString("value",textProvID.Text);
								xmlWriter.WriteEndElement();//id
								}
							xmlWriter.WriteEndElement();//assignedAuthorizedPerson
						if(textOrgID.Text!="" && textOrgName.Text!=""){
							xmlWriter.WriteStartElement("representedOrganization");
								xmlWriter.WriteAttributeString("classCode","ORG");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
							if(textOrgID.Text!=""){
								xmlWriter.WriteStartElement("id");
									xmlWriter.WriteAttributeString("value",textOrgID.Text);
								xmlWriter.WriteEndElement();//id
							}
							if(textOrgName.Text!=""){
								xmlWriter.WriteStartElement("name");
									xmlWriter.WriteAttributeString("value",textOrgName.Text);
								xmlWriter.WriteEndElement();//name
							}
							xmlWriter.WriteEndElement();//representedOrganization
						}
						xmlWriter.WriteEndElement();//assignedEntity
					xmlWriter.WriteEndElement();//holder
				//Performer (Requester)--------------------------------------------------------------------------------------------------------------------------
					xmlWriter.WriteStartElement("performer");
						xmlWriter.WriteAttributeString("typeCode","PRF");						
					if(radioReqProv.Checked) {//----performer choice-----
						xmlWriter.WriteStartElement("healthCareProvider");
							xmlWriter.WriteAttributeString("classCode","PROV");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code","120000000X");
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.6.101");
								xmlWriter.WriteAttributeString("codeSystemName","NUCC Health Care Provider Taxonomy");
								xmlWriter.WriteAttributeString("displayName","Dental Providers");
							xmlWriter.WriteEndElement();//code
						if((comboProvLang.Text!="" && radioReqProv.Checked) 
							|| (comboPatLang.Text=="" && radioReqPat.Checked))
						{//A missing languageCommunication field invalidates the entire Person class.
							xmlWriter.WriteStartElement("healthCarePerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
								xmlWriter.WriteStartElement("languageCommunication");
									xmlWriter.WriteStartElement("languageCommunicationCode");
										xmlWriter.WriteAttributeString("code",_cultureInfoArray[comboProvLang.SelectedIndex].ThreeLetterISOLanguageName);
										xmlWriter.WriteAttributeString("codeSytem","1.0.639.2");
										xmlWriter.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										xmlWriter.WriteAttributeString("displayName",_cultureInfoArray[comboProvLang.SelectedIndex].DisplayName);
									xmlWriter.WriteEndElement();//languageCommunicationCode
								xmlWriter.WriteEndElement();//languageCommunication
							xmlWriter.WriteEndElement();//healthCarePerson
							}//end if no language selected.
						xmlWriter.WriteEndElement();//healthCareProvider
					}
					else {//Performer is patient.
						xmlWriter.WriteStartElement("patient");
							xmlWriter.WriteAttributeString("classCode","PAT");
						if((comboProvLang.Text!="" && radioRecProv.Checked) 
							|| (comboPatLang.Text=="" && radioRecPat.Checked))
						{//A missing languageCommunication field invalidates the entire Person class.
							xmlWriter.WriteStartElement("patientPerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
								xmlWriter.WriteStartElement("languageCommunication");
									xmlWriter.WriteStartElement("languageCommunicationCode");
										xmlWriter.WriteAttributeString("code",_cultureInfoArray[comboPatLang.SelectedIndex].ThreeLetterISOLanguageName);
										xmlWriter.WriteAttributeString("codeSytem","1.0.639.2");
										xmlWriter.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										xmlWriter.WriteAttributeString("displayName",_cultureInfoArray[comboPatLang.SelectedIndex].DisplayName);
									xmlWriter.WriteEndElement();//languageCommunicationCode
								xmlWriter.WriteEndElement();//languageCommunication
							xmlWriter.WriteEndElement();//patientPerson
							}//end if no language selected.
						xmlWriter.WriteEndElement();//patient
					}
					xmlWriter.WriteEndElement();//performer
				//InformationRecipient--------------------------------------------------------------------------------------------------------------------------
					xmlWriter.WriteStartElement("informationRecipient");	
					if(radioRecProv.Checked) {//----performer choice-----
						xmlWriter.WriteStartElement("healthCareProvider");
							xmlWriter.WriteAttributeString("classCode","PROV");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code","120000000X");
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.6.101");
								xmlWriter.WriteAttributeString("codeSystemName","NUCC Health Care Provider Taxonomy");
								xmlWriter.WriteAttributeString("displayName","Dental Providers");
							xmlWriter.WriteEndElement();//code
							xmlWriter.WriteStartElement("healthCarePerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
								xmlWriter.WriteStartElement("languageCommunication");
									xmlWriter.WriteStartElement("languageCommunicationCode");
										xmlWriter.WriteAttributeString("code",_cultureInfoArray[comboProvLang.SelectedIndex].ThreeLetterISOLanguageName);
										xmlWriter.WriteAttributeString("codeSytem","1.0.639.2");
										xmlWriter.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										xmlWriter.WriteAttributeString("displayName",_cultureInfoArray[comboProvLang.SelectedIndex].DisplayName);
									xmlWriter.WriteEndElement();//languageCommunicationCode
								xmlWriter.WriteEndElement();//languageCommunication
							xmlWriter.WriteEndElement();//healthCarePerson
						xmlWriter.WriteEndElement();//healthCareProvider
					}
					else {//Performer is patient.
						xmlWriter.WriteStartElement("patient");
							xmlWriter.WriteAttributeString("classCode","PAT");
							xmlWriter.WriteStartElement("patientPerson");
								xmlWriter.WriteAttributeString("classCode","PSN");
								xmlWriter.WriteAttributeString("determinerCode","INSTANCE");
								xmlWriter.WriteStartElement("languageCommunication");
									xmlWriter.WriteStartElement("languageCommunicationCode");
										xmlWriter.WriteAttributeString("code",_cultureInfoArray[comboPatLang.SelectedIndex].ThreeLetterISOLanguageName);
										xmlWriter.WriteAttributeString("codeSytem","1.0.639.2");
										xmlWriter.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										xmlWriter.WriteAttributeString("displayName",_cultureInfoArray[comboPatLang.SelectedIndex].DisplayName);
									xmlWriter.WriteEndElement();//languageCommunicationCode
								xmlWriter.WriteEndElement();//languageCommunication
							xmlWriter.WriteEndElement();//patientPerson
						xmlWriter.WriteEndElement();//patient
					}
					xmlWriter.WriteEndElement();//informationRecipient
					xmlWriter.WriteStartElement("subject2");
						xmlWriter.WriteAttributeString("typeCode","SUBJ");
						xmlWriter.WriteStartElement("taskContext");
							xmlWriter.WriteAttributeString("classCode","ACT");
							xmlWriter.WriteAttributeString("moodCode","DEF");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code",ActTaskCodeHelper());
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								xmlWriter.WriteAttributeString("codeSystemName","ActCode");
								xmlWriter.WriteAttributeString("displayName",ActTaskCodeNameHelper());
							xmlWriter.WriteEndElement();//code
						xmlWriter.WriteEndElement();//taskContext
					xmlWriter.WriteEndElement();//subject2
					xmlWriter.WriteStartElement("subject3");
						xmlWriter.WriteAttributeString("typeCode","SUBJ");
						xmlWriter.WriteStartElement("subTopic");
							xmlWriter.WriteAttributeString("classCode","OBS");
							xmlWriter.WriteAttributeString("moodCode","DEF");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code","KSUBT");
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								xmlWriter.WriteAttributeString("codeSystemName","ActCode");
								xmlWriter.WriteAttributeString("displayName","knowledge subtopic");
							xmlWriter.WriteEndElement();//code
							//w.WriteStartElement("value");
							//	w.WriteAttributeString("code","TODO");
							//	w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.177");
							//	w.WriteAttributeString("codeSystemName","MSH");
							//	w.WriteAttributeString("displayName","TODO");
							//w.WriteEndElement();//value
						xmlWriter.WriteEndElement();//subTopic
					xmlWriter.WriteEndElement();//subject3
					xmlWriter.WriteStartElement("subject4");
						xmlWriter.WriteAttributeString("typeCode","SUBJ");
						xmlWriter.WriteStartElement("mainSearchCriteria");
							xmlWriter.WriteAttributeString("classCode","OBS");
							xmlWriter.WriteAttributeString("moodCode","DEF");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code","KSUBJ");
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								xmlWriter.WriteAttributeString("codeSystemName","ActCode");
								xmlWriter.WriteAttributeString("displayName","knowledge subject");
							xmlWriter.WriteEndElement();//code
							xmlWriter.WriteStartElement("value");
						//switch(tabControl1.SelectedTab.Name) {
						//	case "tabProblem"://------------------------------------------------------------------------------------------------
						//		w.WriteAttributeString("code","TODO:SNOMED CT Problem Code.");
						//		w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.96");//HL7 OID for SNOMED Clinical Terms
						//		w.WriteAttributeString("codeSystemName","snomed-CT");//HL7 name for SNOMED Clinical Terms
						//		w.WriteAttributeString("displayName","TODO:SNOMED CT Problem Name");
						//		break;
						//	case "tabMedication"://---------------------------------------------------------------------------------------------
						//		w.WriteAttributeString("code","TODO:SNOMED CT Medication Code.");
						//		w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.96");//HL7 OID for SNOMED Clinical Terms
						//		w.WriteAttributeString("codeSystemName","snomed-CT");//HL7 name for SNOMED Clinical Terms
						//		w.WriteAttributeString("displayName","TODO: SNOMED CT Medication Name.");
						//		break;
						//	case "tabLabResult"://----------------------------------------------------------------------------------------------
						//		w.WriteAttributeString("code","TODO: SNOMED CT Lab Results Code??");
						//		w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.96");//HL7 OID for SNOMED Clinical Terms
						//		w.WriteAttributeString("codeSystemName","snomed-CT");//HL7 name for SNOMED Clinical Terms
						//		w.WriteAttributeString("displayName","TODO: SNOMED CT Lab Results Name??");
						//		break;
						//	default://----------------------------------------------------------------------------------------------------------
						//		//either no tab is selected or the tab names above are misspelled.
						//		//w.WriteAttributeString("code","TODO: ");
						//		//w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.96");//HL7 OID for SNOMED Clinical Terms
						//		//w.WriteAttributeString("codeSystemName","snomed-CT");//HL7 name for SNOMED Clinical Terms
						//		//w.WriteAttributeString("displayName","TODO: ");
						//		break;
						//}
							xmlWriter.WriteEndElement();//value
						//if(tabControl1.SelectedTab.Name=="tabLabResult"){
						//	w.WriteStartElement("subject");
						//		w.WriteAttributeString("typeCode","SUBJ");
						//		w.WriteStartElement("severityObservation");
						//			w.WriteAttributeString("classCode","OBS");
						//			w.WriteAttributeString("moodCode","DEF");
						//			w.WriteStartElement("code");
						//				w.WriteAttributeString("code","SEV");
						//				w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
						//				w.WriteAttributeString("codeSystemName","ActCode");
						//				w.WriteAttributeString("displayName","Severity Observation");
						//			w.WriteEndElement();//code
						//			w.WriteStartElement("interpretationCode");
						//				w.WriteAttributeString("code",ObservationInterpretationCodeHelper(ObsInterpretation));
						//				w.WriteAttributeString("codeSytem","");
						//				w.WriteAttributeString("codeSystemName","");
						//				w.WriteAttributeString("displayName",ObservationInterpretationNameHelper(ObsInterpretation));
						//			w.WriteEndElement();//value
						//		w.WriteEndElement();//severityObservation
						//	w.WriteEndElement();//subject
						//}
						xmlWriter.WriteEndElement();//mainSearchCriteria
					xmlWriter.WriteEndElement();//subject4
					xmlWriter.WriteStartElement("componentOf");
						xmlWriter.WriteAttributeString("typeCode","COMP");
						xmlWriter.WriteStartElement("encounter");
							xmlWriter.WriteAttributeString("classCode","ENC");
							xmlWriter.WriteAttributeString("moodCode","DEF");
							xmlWriter.WriteStartElement("code");
								xmlWriter.WriteAttributeString("code",EncounterCodeHelper(ActEncounterCode_));
								xmlWriter.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								xmlWriter.WriteAttributeString("codeSystemName","ActCode");
								xmlWriter.WriteAttributeString("displayName",EncounterCode(ActEncounterCode_));
							xmlWriter.WriteEndElement();//code
						if(textEncLocID.Text!=""){
							xmlWriter.WriteStartElement("location");
							xmlWriter.WriteAttributeString("typeCode","LOC");
								xmlWriter.WriteStartElement("serviceDeliveryLocation");
									xmlWriter.WriteAttributeString("typeCode","SDLOC");
									xmlWriter.WriteAttributeString("id",textEncLocID.Text);
								xmlWriter.WriteEndElement();//serviceDeliveryLocation
							xmlWriter.WriteEndElement();//location
							}
						xmlWriter.WriteEndElement();//encounter
					xmlWriter.WriteEndElement();//componentOf
				xmlWriter.WriteEndElement();//knowledgeRequestNotification
			}
			return stringBuilder.ToString();
		}

		#region helper Functions Start

		private string KnowledgeRequestIDAAHelper() {
			if(PrefC.GetString(PrefName.PracticeTitle)!="") {
				return PrefC.GetString(PrefName.PracticeTitle);
			}
			return "Open Dental Software, version"+PrefC.GetString(PrefName.ProgramVersion);
		}

		private string KnowledgeRequestIDHelper() {
			if(PatientCur!=null) {
				return "PT"+PatientCur.PatNum+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
			}
			else if(ProviderCur!=null) {
				return "PV"+ProviderCur.ProvNum+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
			}
			return "OD"+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
		}

		private string EncounterCodeHelper(ActEncounterCode actEncounterCode) {
			switch(actEncounterCode) {
				case ActEncounterCode.AMB:
					return "AMB";
				case ActEncounterCode.EMER:
					return "EMER";
				case ActEncounterCode.FLD:
					return "FLD";
				case ActEncounterCode.HH:
					return "HH";
				case ActEncounterCode.IMP:
					return "IMP";
				case ActEncounterCode.SS:
					return "SS";
				case ActEncounterCode.VR:
					return "VR";
				default:
					return "";
			}
		}

		private string EncounterCode(ActEncounterCode actEncounterCode) {
			switch(actEncounterCode) {
				case ActEncounterCode.AMB:
					return "ambulatory";
				case ActEncounterCode.EMER:
					return "emergency";
				case ActEncounterCode.FLD:
					return "field";
				case ActEncounterCode.HH:
					return "home health";
				case ActEncounterCode.IMP:
					return "inpatient encounter";
				case ActEncounterCode.SS:
					return "short stay";
				case ActEncounterCode.VR:
					return "virtual";
				default:
					return "";
			}
		}

		public string ObservationInterpretationCodeHelper(ObservationInterpretationNormality observationInterpretationNormality) {
			switch(observationInterpretationNormality) {
				case ObservationInterpretationNormality.A:
					return "A";
				case ObservationInterpretationNormality.AA:
					return "AA";
				case ObservationInterpretationNormality.HH:
					return "HH";
				case ObservationInterpretationNormality.LL:
					return "LL";
				case ObservationInterpretationNormality.H:
					return "H";
				case ObservationInterpretationNormality.L:
					return "L";
				case ObservationInterpretationNormality.N:
					return "N";
				default:
					return "";
			}
		}

		public string ObservationInterpretationNameHelper(ObservationInterpretationNormality observationInterpretationNormality) {
			switch(observationInterpretationNormality) {
				case ObservationInterpretationNormality.A:
					return "Abnormal";
				case ObservationInterpretationNormality.AA:
					return "Abnormal alert";
				case ObservationInterpretationNormality.HH:
					return "High alert";
				case ObservationInterpretationNormality.LL:
					return "Low alert";
				case ObservationInterpretationNormality.H:
					return "High";
				case ObservationInterpretationNormality.L:
					return "Low";
				case ObservationInterpretationNormality.N:
					return "Normal";
				default:
					return "";
			}
		}

		/// <summary>Returns thefirst level of ActTaskCode. OE, PATDOC, or PATINFO there are 35 total ActTaskCodes available.</summary>
		public string ActTaskCodeHelper() {
			switch(_actTaskCode) {
				case ActTaskCode.OE:
					return "OE";
				case ActTaskCode.PATDOC:
					return "PATDOC";
				case ActTaskCode.PATINFO:
					return "PATINFO";
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>Returns thefirst level of ActTaskCode. OE, PATDOC, or PATINFO there are 35 total ActTaskCodes available.</summary>
		public string ActTaskCodeNameHelper() {
			switch(_actTaskCode) {
				case ActTaskCode.OE:
					return "order entry task";
				case ActTaskCode.PATDOC:
					return "patient documentation task";
				case ActTaskCode.PATINFO:
					return "patient information review task";
				default:
					throw new NotImplementedException();
			}
		}

		///<summary>Returns MeSH age group code based on birthdate. i.e. &lt;2yrs==Infant==D007231</summary>
		public string AgeGroupCodeHelper(DateTime dateTime) {
			#region MeSH (Medical Subject Headers) codes used for age groups.
			//*NEWRECORD
			//RECTYPE = D
			//MH = Infant, NewbornGM = birth to 1 month age group
			//MS = An infant during the first month after birth.
			//UI = D007231
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Infant
			//GM = 1 month to 2 year age group; + includes birth to 2 years; for birth to 1 month, use Infant, Newborn +
			//MS = A child between 1 and 23 months of age.
			//UI = D007223
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Child, Preschool
			//GM = 2-5 age group; for 1 month to 2 years use Infant +
			//MS = A child between the ages of 2 and 5.
			//UI = D002675
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Child
			//MH = ChildGM = 6-12 age group; for 2-5 use Child, Preschool; + includes birth to 18 year age group
			//MS = A person 6 to 12 years of age. An individual 2 to 5 years old is CHILD, PRESCHOOL.
			//UI = D002648
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Adolescent
			//AN = age 13-18 yr; IM as psychol & sociol entity; check tag ADOLESCENT for NIM; Manual 18.5.12, 34.9.5
			//MS = A person 13 to 18 years of age.
			//UI = D000293
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Adult
			//GM = 19-44 age group; older than 44, use Middle Age, Aged +, or + for all
			//MS = A person having attained full growth or maturity. Adults are of 19 through 44 years of age. For a person between 19 and 24 years of age, YOUNG ADULT is available.
			//UI = D000328
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Middle Aged
			//AN = age 45-64; IM as psychol, sociol entity: Manual 18.5.12; NIM as check tag; Manual 34.10 for indexing examples
			//UI = D008875
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Aged
			//GM = 65 and older; consider also Aged, 80 and over
			//MS = A person 65 through 79 years of age. For a person older than 79 years, AGED, 80 AND OVER is available.
			//UI = D000368
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Aged, 80 and over
			//GM = consider also Aged + (65 and older)
			//MS = A person 80 years of age and older.
			//UI = D000369
			#endregion
			if(PatientCur.Birthdate.AddMonths(1)>DateTime.Now) {//less than 1mo old, newborn
				return "D007231";
			}
			else if(PatientCur.Birthdate.AddYears(2)>DateTime.Now) {//less than 2 yrs old, Infant
				return "D007223";
			}
			else if(PatientCur.Birthdate.AddYears(5)>DateTime.Now) {//2 to 5 yrs old, Preschool
				return "D007675";
			}
			else if(PatientCur.Birthdate.AddYears(12)>DateTime.Now) {//6 to 12 yrs old, Child
				return "D002648";
			}
			else if(PatientCur.Birthdate.AddYears(18)>DateTime.Now) {//13 to 18 yrs old, Adolescent
				return "D000293";
			}
			else if(PatientCur.Birthdate.AddYears(44)>DateTime.Now) {//19 to 44 yrs old, Adult
				return "D000328";
			}
			else if(PatientCur.Birthdate.AddYears(64)>DateTime.Now) {//45 to 64 yrs old, Middle Aged
				return "D008875";
			}
			else if(PatientCur.Birthdate.AddYears(79)>DateTime.Now) {//65 to 79 yrs old, Aged
				return "D000368";
			}
			else { //if(PatCur.Birthdate.AddYears(79)>DateTime.Now) {//80 yrs old or older, Aged, 80 and over
				return "D000369";
			}
		}

		///<summary>Returns MeSH age group name based on birthdate. i.e. &lt;2yrs==Infant.</summary>
		public string AgeGroupNameHelper(DateTime dateTime) {
			#region MeSH (Medical Subject Headers) codes used for age groups.
			//*NEWRECORD
			//RECTYPE = D
			//MH = Infant, NewbornGM = birth to 1 month age group
			//MS = An infant during the first month after birth.
			//UI = D007231
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Infant
			//GM = 1 month to 2 year age group; + includes birth to 2 years; for birth to 1 month, use Infant, Newborn +
			//MS = A child between 1 and 23 months of age.
			//UI = D007223
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Child, Preschool
			//GM = 2-5 age group; for 1 month to 2 years use Infant +
			//MS = A child between the ages of 2 and 5.
			//UI = D002675
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Child
			//MH = ChildGM = 6-12 age group; for 2-5 use Child, Preschool; + includes birth to 18 year age group
			//MS = A person 6 to 12 years of age. An individual 2 to 5 years old is CHILD, PRESCHOOL.
			//UI = D002648
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Adolescent
			//AN = age 13-18 yr; IM as psychol & sociol entity; check tag ADOLESCENT for NIM; Manual 18.5.12, 34.9.5
			//MS = A person 13 to 18 years of age.
			//UI = D000293
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Adult
			//GM = 19-44 age group; older than 44, use Middle Age, Aged +, or + for all
			//MS = A person having attained full growth or maturity. Adults are of 19 through 44 years of age. For a person between 19 and 24 years of age, YOUNG ADULT is available.
			//UI = D000328
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Middle Aged
			//AN = age 45-64; IM as psychol, sociol entity: Manual 18.5.12; NIM as check tag; Manual 34.10 for indexing examples
			//UI = D008875
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Aged
			//GM = 65 and older; consider also Aged, 80 and over
			//MS = A person 65 through 79 years of age. For a person older than 79 years, AGED, 80 AND OVER is available.
			//UI = D000368
			//
			//*NEWRECORD
			//RECTYPE = D
			//MH = Aged, 80 and over
			//GM = consider also Aged + (65 and older)
			//MS = A person 80 years of age and older.
			//UI = D000369
			#endregion
			if(PatientCur.Birthdate.AddMonths(1)>DateTime.Now) {//less than 1mo old, newborn
				return "Newborn";
			}
			else if(PatientCur.Birthdate.AddYears(2)>DateTime.Now) {//less than 2 yrs old, Infant
				return "Infant";
			}
			else if(PatientCur.Birthdate.AddYears(5)>DateTime.Now) {//2 to 5 yrs old, Preschool
				return "Preschool";
			}
			else if(PatientCur.Birthdate.AddYears(12)>DateTime.Now) {//6 to 12 yrs old, Child
				return "Child";
			}
			else if(PatientCur.Birthdate.AddYears(18)>DateTime.Now) {//13 to 18 yrs old, Adolescent
				return "Adolescent";
			}
			else if(PatientCur.Birthdate.AddYears(44)>DateTime.Now) {//19 to 44 yrs old, Adult
				return "Adult";
			}
			else if(PatientCur.Birthdate.AddYears(64)>DateTime.Now) {//45 to 64 yrs old, Middle Aged
				return "Middle Aged";
			}
			else if(PatientCur.Birthdate.AddYears(79)>DateTime.Now) {//65 to 79 yrs old, Aged
				return "Aged";
			}
			else { //if(PatCur.Birthdate.AddYears(79)>DateTime.Now) {//80 yrs old or older, Aged, 80 and over
				return "Aged, 80 and over";
			}
		}

		///<summary>The gender of a person used for adminstrative purposes (as opposed to clinical gender). Empty string/value is allowed.</summary>
		public string AdministrativeGenderCodeHelper(PatientGender patientGender) {
			switch(patientGender) {
				case PatientGender.Female:
					return "F";
				case PatientGender.Male:
					return "M";
				case PatientGender.Unknown:
					return "UN";
				default://should never happen
					return " ";
			}
		} 

		///<summary>The gender of a person used for adminstrative purposes (as opposed to clinical gender). Empty string/value is allowed.</summary>
		public string AdministrativeGenderNameHelper(PatientGender patientGender) {
			switch(patientGender) {
				case PatientGender.Female:
					return "Female";
				case PatientGender.Male:
					return "Male";
				case PatientGender.Unknown:
					return "Undifferentiated";
				default://should never happen
					return "";
			}
		}

		#endregion

		private void comboEncType_SelectedIndexChanged(object sender,EventArgs e) {
			ActEncounterCode_=(ActEncounterCode)comboEncType.SelectedIndex;
		}

		private void comboTask_SelectedIndexChanged(object sender,EventArgs e) {
			_actTaskCode=(ActTaskCode)comboTask.SelectedIndex;
		}

		//private void butProbPick_Click(object sender,EventArgs e) {
		//	using FormDiseaseDefs FormDD = new FormDiseaseDefs();
		//	FormDD.IsSelectionMode=true;
		//	FormDD.ShowDialog();
		//	if(FormDD.DialogResult!=DialogResult.OK) {
		//		return;
		//	}
		//	//ProblemCur=DiseaseDefs.GetItem(FormDD.SelectedDiseaseDefNum);
		//	//fillProblem();
		//}

		private void butAddDisease_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs = new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formDiseaseDefs.ListDiseaseDefsSelected[0]);
			_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			//EhrTriggers.ConvertToKnowledgeRequests(formDiseaseDefs.ListDiseaseDefsSelected[0]).ForEach(x=>_listKnowledgeRequests.Add(x));
			FillKnowledgeRequestitems();
		}

		private void butAddSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds = new FormSnomeds();
			formSnomeds.IsMultiSelectMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formSnomeds.ListSnomedsSelected.Count;i++) {
				List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formSnomeds.ListSnomedsSelected[i]);
				_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			}			
			FillKnowledgeRequestitems();
		}

		private void butAddRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms formRxNorms=new FormRxNorms();
			formRxNorms.IsMultiSelectMode=true;
			formRxNorms.ShowDialog();
			if(formRxNorms.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formRxNorms.ListSelectedRxNorms.Count;i++) {
				List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formRxNorms.ListSelectedRxNorms[i]);
				_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			}	
			FillKnowledgeRequestitems();
		}

		private void butAddIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s formIcd9s=new FormIcd9s();
			formIcd9s.IsSelectionMode=true;
			formIcd9s.ShowDialog();
			if(formIcd9s.DialogResult!=DialogResult.OK) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formIcd9s.ICD9Selected);
			_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			FillKnowledgeRequestitems();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergySetup formAllergySetup=new FormAllergySetup();
			formAllergySetup.IsSelectionMode=true;
			formAllergySetup.ShowDialog();
			if(formAllergySetup.DialogResult!=DialogResult.OK) {
				return;
			}
			AllergyDef allergyDef = AllergyDefs.GetOne(formAllergySetup.AllergyDefNumSelected);
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(allergyDef);
			_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			FillKnowledgeRequestitems();
		}

		private void butAddIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s formIcd10s=new FormIcd10s();
			formIcd10s.IsSelectionMode=true;
			formIcd10s.ShowDialog();
			if(formIcd10s.DialogResult!=DialogResult.OK) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formIcd10s.Icd10Selected);
			_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			FillKnowledgeRequestitems();
		}

		private void butAddLoinc_Click(object sender,EventArgs e) {
			using FormLoincs formLoincs=new FormLoincs();
			formLoincs.IsSelectionMode=true;
			formLoincs.ShowDialog();
			if(formLoincs.DialogResult!=DialogResult.OK) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(formLoincs.LoincSelected);
			_listKnowledgeRequests.AddRange(listKnowledgeRequests);
			FillKnowledgeRequestitems();
		}

		private void butPreviewRequest_Click(object sender,EventArgs e) {
			KnowledgeRequestNotification.KnowledgeRequestNotification knowledgeRequestNotification;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(gridMain.ListGridRows[i].Tag==null){
					MsgBox.Show(this,"Cannot search without a valid code.");
					continue;
				}
				knowledgeRequestNotification=new KnowledgeRequestNotification.KnowledgeRequestNotification();
				knowledgeRequestNotification.AddKnowledgeRequest((KnowledgeRequest)gridMain.ListGridRows[i].Tag);
				knowledgeRequestNotification.IsPerformerPatient=radioReqPat.Checked;
				knowledgeRequestNotification.IsRecipientPatient=radioRecPat.Checked;
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste("http://apps.nlm.nih.gov/medlineplus/services/mpconnect.cfm?"+knowledgeRequestNotification.ToUrl());
				msgBoxCopyPaste.ShowDialog();
				//msgbox=new MsgBoxCopyPaste("http://apps2.nlm.nih.gov/medlineplus/services/servicedemo.cfm?"+krn.ToUrl());
				//msgbox.ShowDialog();
			}//end gridMain.Rows

		}

		private void butSend_Click(object sender,EventArgs e) {
			KnowledgeRequestNotification.KnowledgeRequestNotification knowledgeRequestNotification;
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==null) {
					MsgBox.Show(this,"Cannot search without a valid code.");
					continue;
				}
				knowledgeRequestNotification=new KnowledgeRequestNotification.KnowledgeRequestNotification();
				knowledgeRequestNotification.AddKnowledgeRequest((KnowledgeRequest)gridMain.ListGridRows[i].Tag);
				knowledgeRequestNotification.IsPerformerPatient=radioReqPat.Checked;
				knowledgeRequestNotification.IsRecipientPatient=radioRecPat.Checked;
				//using FormInfobuttonResponse FormIR = new FormInfobuttonResponse();
				//FormIR.RawResponse=getWebResponse("http://apps2.nlm.nih.gov/medlineplus/services/mpconnect_service.cfm?"+krn.ToUrl());
				//FormIR.ShowDialog();
				//The lines commented out here launch the infobutton in the default browser.
				try {
					System.Diagnostics.Process.Start("http://apps.nlm.nih.gov/medlineplus/services/mpconnect.cfm?"+knowledgeRequestNotification.ToUrl());
					//System.Diagnostics.Process.Start("http://apps2.nlm.nih.gov/medlineplus/services/servicedemo.cfm?"+krn.ToUrl());
				}
				catch(Exception) { }
			}//end gridMain.Rows

		}

		///<summary>For More info goto: http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx </summary>
		private static string GetWebResponse(string url) {
			// Create a request for the URL. 
			WebRequest webRequest = WebRequest.Create(url);
			// If required by the server, set the credentials.
			//request.Credentials = CredentialCache.DefaultCredentials;
			// Get the response.
			WebResponse webResponse = webRequest.GetResponse();
			// Display the status.
			Console.WriteLine(((HttpWebResponse)webResponse).StatusDescription);
			// Get the stream containing content returned by the server.
			Stream stream = webResponse.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader streamReader = new StreamReader(stream);
			// Read the content.
			string responseFromServer = streamReader.ReadToEnd();
			// Clean up the streams and the response.
			streamReader.Close();
			webResponse.Close();
			return responseFromServer;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void groupBoxContext_Enter(object sender,EventArgs e) {

		}

	}

	///<summary>Only enumerating the highest level task codes, OE, PATDOC, and PATINFO., Enum generated from HL7 ActTaskCode [2.16.840.1.113883.1.11.19846] which is a subset of ActCode [OID=2.16.840.1.113883.5.4] documentation published 20120831 10:21 AM.</summary>
	public enum ActTaskCode {
		///<summary>0 - order entry task</summary>
		OE,
		/////<summary>1 - laboratory test order entry task</summary>
		//LABOE,
		/////<summary>2 - medication order entry task</summary>
		//MEDOE,
		///<summary>1 - patient documentation task</summary>
		PATDOC,
		/////<summary>4 - allergy list review</summary>
		//ALLERLREV,
		/////<summary>5 - clinical note entry task</summary>
		//CLINNOTEE,
		/////<summary>6 - diagnosis list entry task</summary>
		//DIAGLISTE,
		/////<summary>7 - discharge summary entry task</summary>
		//DISCHSUME,
		/////<summary>8 - pathology report entry task</summary>
		//PATREPE,
		/////<summary>9 - problem list entry task</summary>
		//PROBLISTE,
		/////<summary>10 - radiology report entry task</summary>
		//RADREPE,
		/////<summary>11 - immunization list review</summary>
		//IMMLREV,
		/////<summary>12 - reminder list review</summary>
		//REMLREV,
		/////<summary>13 - wellness reminder list review</summary>
		//WELLREMLREV,
		///<summary>2 - patient information review task</summary>
		PATINFO
		/////<summary>15 - allergy list entry</summary>
		//ALLERLE,
		/////<summary>16 - clinical note review task</summary>
		//CLINNOTEREV,
		/////<summary>17 - discharge summary review task</summary>
		//DISCHSUMREV,
		/////<summary>18 - diagnosis list review task</summary>
		//DIAGLISTREV,
		/////<summary>19 - immunization list entry</summary>
		//IMMLE,
		/////<summary>20 - laboratory results review task</summary>
		//LABRREV,
		/////<summary>21 - microbiology results review task</summary>
		//MICRORREV,
		/////<summary>22 - microbiology organisms results review task</summary>
		//MICROORGRREV,
		/////<summary>23 - microbiology sensitivity test results review task</summary>
		//MICROSENSRREV,
		/////<summary>24 - medication list review task</summary>
		//MLREV,
		/////<summary>25 - medication administration record work list review task</summary>
		//MARWLREV,
		/////<summary>26 - orders review task</summary>
		//OREV,
		/////<summary>27 - pathology report review task</summary>
		//PATREPREV,
		/////<summary>28 - problem list review task</summary>
		//PROBLISTREV,
		/////<summary>29 - radiology report review task</summary>
		//RADREPREV,
		/////<summary>30 - reminder list entry</summary>
		//REMLE,
		/////<summary>31 - wellness reminder list entry</summary>
		//WELLREMLE,
		/////<summary>32 - risk assessment instrument task</summary>
		//RISKASSESS,
		/////<summary>33 - falls risk assessment instrument task</summary>
		//FALLRISK
	}

	///<summary>Enum generated from HL7 ActEncounterCode [2.16.840.1.113883.1.11.13955] which is a subset of ActCode [OID=2.16.840.1.113883.5.4] documentation published 20120831 10:21 AM.</summary>
	public enum ActEncounterCode {
		///<summary>0 - ambulatory</summary>
		AMB,
		///<summary>1 - emergency</summary>
		EMER,
		///<summary>2 - field</summary>
		FLD,
		///<summary>3 - home health</summary>
		HH,
		///<summary>4 - inpatient encounter</summary>
		IMP,
		///<summary>5 - short stay</summary>
		SS,
		///<summary>6 - virtual</summary>
		VR
	}

	///<summary>Normality, Abnormality, Alert. Concepts in this category are mutually exclusive, i.e., at most one is allowed. Enum generated from HL7 _ObservationInterpretationNormality [2.16.840.1.113883.1.11.10206] which is a subset of ObservationInterpretation [OID=2.16.840.1.113883.5.83] documentation published 20120831 10:21 AM.</summary>
	public enum ObservationInterpretationNormality {
		///<summary>0 - Abnormal - Abnormal (for nominal observations, all service types) </summary>
		A,
		///<summary>1 - Abnormal alert - Abnormal alert (for nominal observations and all service types) </summary>
		AA,
		///<summary>2 - High alert - Above upper alert threshold (for quantitative observations) </summary>
		HH,
		///<summary>3 - Low alert - Below lower alert threshold (for quantitative observations) </summary>
		LL,
		///<summary>4 - High - Above high normal (for quantitative observations) </summary>
		H,
		///<summary>5 - Low - Below low normal (for quantitative observations) </summary>
		L,
		///<summary>6 - Normal - Normal (for all service types) </summary>
		N 
	}

}

namespace KnowledgeRequestNotification {

	///<summary>This class represents the root of the KnowledgeRequestNotificatio.</summary>
	public class KnowledgeRequestNotification {
		///<summary>Classification code.  Static field "ACT".  A record of something that is being done, has been done, can be done, or is intended or requested to be done.  Cardinality [1..1]</summary>
		public static string ClassCode="ACT";		//1..1
		///<summary>Static field "DEF".  A definition of a service (master).  Cardinality [1..1]</summary>
		public static string MoodCode="DEF";						//1..1
		///<summary>List of globally unique identifiers of this knowledge request.  Cardinality [0..*]</summary>
		public List<Id> ListIds=new List<Id>();	//0..*
		///<summary>Creation time of the knowledge request.  Must be formatted "yyyyMMddhhmmss" when used.  Cardinality [0..1]</summary>
		public DateTime DateTimeEffective;					//0..1 "yyyyMMddhhmmss"
		///<summary>Patient context information. Cardinality [0..1]</summary>
		public Subject Subject_;								//0..1
		///<summary>Not fully implemented. Implemented enough to work.</summary>
		public bool IsPerformerPatient;
		///<summary>Not fully implemented. Implemented enough to work.</summary>
		public bool IsRecipientPatient;

		//public Performer performer;

		//public InformationRecipient informationRecipient;


		///<summary>Task context information. Cardinality [0..1]</summary>
		public Subject1 Subject1_;								//0..1
		///<summary>Sub-topic information. Cardinality [0..1]</summary>
		public Subject2 Subject2_;								//0..1
		///<summary>Conatins a list of MainSearchCriteria: represents the main subject of interest in a knowledge request (e.g., a medication, a lab test result, a disease in the patient's problem list). When multiple multiple search criteria are present, knowledge resources MAY determine whether to join the multiple instances using the AND vs. OR Boolean operator. Cardinality[1..*]</summary>
		public List<Subject3> ListSubject3s;			//1..*
		///<summary>Contains encounter information, type and location.  Cardinality[0..1]</summary>
		public Component1 Component1_;					//0..1

		public KnowledgeRequestNotification() {
			ClassCode="ACT";
			MoodCode="DEF";
			ListIds=new List<Id>();
			DateTimeEffective=DateTime.Now;
			Subject_=new Subject();
			Subject1_=new Subject1();
			Subject2_=new Subject2();
			ListSubject3s=new List<Subject3>();
			Component1_=new Component1();
		}

		public void AddKnowledgeRequest(KnowledgeRequest knowledgeRequest) {
			switch(knowledgeRequest.CodeSystem) {
				case CodeSyst.Snomed:
					ListSubject3s.Add(new Subject3(new Value(knowledgeRequest.Code,"2.16.840.1.113883.6.96","SNOMEDCT",knowledgeRequest.Description)));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description;
					break;
				case CodeSyst.Icd9:
					ListSubject3s.Add(new Subject3(new Value(knowledgeRequest.Code,"2.16.840.1.113883.6.103","ICD9CM",knowledgeRequest.Description)));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description;
					break;
				case CodeSyst.Icd10:
					ListSubject3s.Add(new Subject3(new Value(knowledgeRequest.Code,"2.16.840.1.113883.6.90","ICD10CM",knowledgeRequest.Description)));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description;
					break;
				case CodeSyst.RxNorm:
					ListSubject3s.Add(new Subject3(new Value(knowledgeRequest.Code,"2.16.840.1.113883.6.88","RxNorm",knowledgeRequest.Description)));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description;
					break;
				case CodeSyst.Loinc:
					ListSubject3s.Add(new Subject3(new Value(knowledgeRequest.Code,"2.16.840.1.113883.6.1","LOINC",knowledgeRequest.Description)));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description;
					break;
				case CodeSyst.AllergyDef:
					ListSubject3s.Add(new Subject3(new Value("","","",knowledgeRequest.Description.Replace("Allergy -",""))));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description.Replace("Allergy -","");
					break;
				case CodeSyst.ProblemDef:
					ListSubject3s.Add(new Subject3(new Value("","","",knowledgeRequest.Description.Replace("Problem -",""))));
					ListSubject3s[ListSubject3s.Count-1].MainSearchCriteria_.OriginalText=knowledgeRequest.Description.Replace("Problem -","");
					break;
					//case "LabResult"://Deprecated
					//	AddLabResult((LabResult)obj);
					//	break;
			}
		}	

		public string ToXml() {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				xmlWriter.WriteWhitespace("\r\n");
				//TODO:Implement more fields, this is just
				xmlWriter.WriteStartElement("knowledgeRequestNotification");
					xmlWriter.WriteAttributeString("classCode","ACT");
					xmlWriter.WriteAttributeString("moodCode","DEF");
					//id
					//effectiveTime
					//subject1
					//holder
					//performer
					//informationRecipient
					//subject2
					//subject3
					xmlWriter.WriteRaw(global::KnowledgeRequestNotification.Subject3.ToXml(ListSubject3s));
					//componentOf
				xmlWriter.WriteEndElement();//knowledgeRequestNotification
			}
			return stringBuilder.ToString();
		}

		public string ToUrl() {
			StringBuilder stringBuilder=new StringBuilder();
			if(DateTimeEffective.Year>1880) {
				stringBuilder.Append("knowledgeRequestNotification.effectiveTime.v="+DateTimeEffective.ToString("yyyyMMddhhmmss")+"&");
			}
			else {
				stringBuilder.Append("");
			}
			//holder
			//assignedEntity
			//patientPerson
			//age
			//ageGroup
			//taskContext
			//subTopic
			for(int i=0;i<ListSubject3s.Count;i++) {
				stringBuilder.Append("mainSearchCriteria.v.c");
				if(i==0) {
					stringBuilder.Append("");
				}
				else {
					stringBuilder.Append(""+i);
				}
				stringBuilder.Append("="+ListSubject3s[i].MainSearchCriteria_.Value_.Code+"&");			
				stringBuilder.Append("mainSearchCriteria.v.cs");
				if(i==0) {
					stringBuilder.Append("");
				}
				else {
					stringBuilder.Append(""+i);
				}
				stringBuilder.Append("="+ListSubject3s[i].MainSearchCriteria_.Value_.CodeSystem+"&");
				stringBuilder.Append("mainSearchCriteria.v.dn");
				if(i==0) {
					stringBuilder.Append("");
				}
				else {
					stringBuilder.Append(""+i);
				}
				stringBuilder.Append("="+ListSubject3s[i].MainSearchCriteria_.Value_.DisplayName+"&");
				if(ListSubject3s[i].MainSearchCriteria_.OriginalText!=ListSubject3s[i].MainSearchCriteria_.Value_.DisplayName) {//original text only if different than display name.
					stringBuilder.Append("mainSearchCriteria.v.ot");
					if(i==0) {
					stringBuilder.Append("");
					}
					else {
						stringBuilder.Append(""+i);
					}
					stringBuilder.Append("="+ListSubject3s[i].MainSearchCriteria_.OriginalText+"&");
				}
				//severityObservation
			}
			//informationRecipient
			stringBuilder.Append("informationRecipient=");
			if(IsRecipientPatient) {
				stringBuilder.Append("PAT&");
			}
			else {
				stringBuilder.Append("PROV&");
			}
			//performer
			stringBuilder.Append("performer=");
			if(IsPerformerPatient) {
				stringBuilder.Append("PAT&");
			}
			else {
				stringBuilder.Append("PROV&");
			}
			//encounter
			//serviceDeliveryLocation
			return stringBuilder.ToString().Replace(" ","%20");
		}

		public string ToUrl(string xml) {
			StringBuilder stringBuilder=new StringBuilder();
			//TODO later, maybe
			//XmlDocument doc=new XmlDocument();
			//doc.LoadXml(xml);
			//XmlNode node=doc.SelectSingleNode("//Error");
			//if(node!=null) {
			//	throw new Exception(node.InnerText);
			//}
			return stringBuilder.ToString();
		}

	}

	///<summary>Represents the globally unique instance identifier of a knowledge request.  0..*</summary>
	public class Id {

	}

	///<summary>Patient context information. Cardinality [0..1]</summary>
	public class Subject {

	}

	/////<summary>Patient context information. Cardinality [0..1]</summary>
	//public class Performer {
	//	public string typeCode;
	//}

	/////<summary>Patient context information. Cardinality [0..1]</summary>
	//public class InformationRecipient {

	//}

	///<summary>Task context information. Cardinality [0..1]</summary>
	public class Subject1 {

	}

	///<summary>Sub-topic information. Cardinality [0..1]</summary>
	public class Subject2 {

	}

	///<summary>Mostly just a main search criteria.</summary>
	public class Subject3 {
		public string TypeCode;
		public MainSearchCriteria MainSearchCriteria_;

		public Subject3() {
			TypeCode="SUBJ";
			MainSearchCriteria_=new MainSearchCriteria();
		}

		public Subject3(Value value) {
			TypeCode="SUBJ";
			MainSearchCriteria_=new MainSearchCriteria(value);
		}

		public static string ToXml(List<Subject3> listSubject3s) {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				for(int i=0;i<listSubject3s.Count;i++){
					xmlWriter.WriteStartElement("subject4");
						xmlWriter.WriteAttributeString("typeCode",listSubject3s[i].TypeCode);
						xmlWriter.WriteRaw(listSubject3s[i].MainSearchCriteria_.ToXml());
					xmlWriter.WriteEndElement();
				}//end subject4List
			}//end using
			return stringBuilder.ToString();
		}
	}

	public class MainSearchCriteria {
		///<summary>Static field "OBS".  Observation.  Cardinality [1..1]</summary>
		public static string ClassCode;
		///<summary>Static field "DEF".  A definition of a service (master).  Cardinality [1..1]</summary>
		public static string MoodCode;
		///<summary>Static field.  This defines the value as being a knowledge subject.  Cardinality [1..1]</summary>
		public Code Code_;
		///<summary>Contains information on the snomed in question, icd9, icd10 ... etc code.  The "value" of the "code".  Cardinality [1..1]</summary>
		public Value Value_;
		///<summary>Represents the human readable representation of the code as displayed to the user in the CIS and SHOULD be used only if different than the displayName</summary>
		public string OriginalText;
		///<summary>Contains SeverityObservation:specifies the interpretation of a laboratory test result (e.g., 'high', 'low', 'abnormal', 'normal'). This class MAY be used to support implementations where the MainSearchCriteria consists of a laboratory test result. Supports questions such as "what are the causes of high serum potassium?</summary>
		public Subject4 Subject4_;

		public MainSearchCriteria() {
			ClassCode="OBS";
			MoodCode="DEF";
			Code_=new Code("KSUBJ","2.16.840.1.113883.6.96","SNOMEDCT","knowledge subject");
			Value_=new Value();
			Subject4_ = new Subject4();
		}

		public MainSearchCriteria(Value value) {
			ClassCode="OBS";
			MoodCode="DEF";
			Code_=new Code("KSUBJ","2.16.840.1.113883.6.96","SNOMEDCT","knowledge subject");
			Value_=value;
			Subject4_ = new Subject4();
		}

		public string ToXml() {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteStartElement("mainSearchCriteria");
					xmlWriter.WriteAttributeString("classCode",ClassCode);
					xmlWriter.WriteAttributeString("moodCode",MoodCode);
					xmlWriter.WriteRaw(Code_.ToXml());
					xmlWriter.WriteRaw(Value_.ToXml());
					if(OriginalText!="" && OriginalText!=null && OriginalText!=Value_.DisplayName){
						xmlWriter.WriteStartElement("originalText");
							xmlWriter.WriteString(OriginalText);
						xmlWriter.WriteEndElement();//originalText
					}
				xmlWriter.WriteEndElement();//mainSearchCriteria
			}
			return stringBuilder.ToString();
		}

	}//MainSearchCriteria class

	public class Subject4 {
		public static string TypeCode;
		public SeverityObservation SeverityObservation_;

		public Subject4(){
			TypeCode="SUBJ";
			SeverityObservation_=new SeverityObservation();
		}
	}

	public class SeverityObservation {
		public static string ClassCode;
		public static string MoodCode;
		public Code Code_;
		public ObservationInterpretationNormality ObservationInterpretationNormality_;

		public SeverityObservation() {
			ClassCode="OBS";
			MoodCode="DEF";
			Code_=new Code("SeverityObservationType","2.16.840.1.113883.5.6","ActClass","SeverityObservationType");
			ObservationInterpretationNormality_=null;
		}
	}

	///<summary>Normality, Abnormality, Alert. Concepts in this category are mutually exclusive, i.e., at most one is allowed. Enum generated from HL7 _ObservationInterpretationNormality [2.16.840.1.113883.1.11.10206] which is a subset of ObservationInterpretation [OID=2.16.840.1.113883.5.83] documentation published 20120831 10:21 AM.</summary>
	public class ObservationInterpretationNormality {
		public string Code;
		public static string CodeSystem;
		public static string CodeSystemName;
		public string DisplayName;

		public ObservationInterpretationNormality(){
			Code="";
			CodeSystem="2.16.840.1.113883.5.83";
			CodeSystemName="ObservationInterpretation";
			DisplayName="";
		}

		public void SetInterpretation(LabAbnormalFlag labAbnormalFlag) {
			switch(labAbnormalFlag) {
				case LabAbnormalFlag.Above:
					SetInterpretation("H");
					return;
				case LabAbnormalFlag.Below:
					SetInterpretation("L");
					return;
				case LabAbnormalFlag.Normal:
					SetInterpretation("N");
					return;
				case LabAbnormalFlag.None:
				default:
					SetInterpretation("");
					return;
			}
		}

		///<summary>Set InterpretaionCodes Normal, Abnormal, High, Low, Alert.</summary>
		/// <param name="code">Allowed values: A, AA, HH, LL, H, L, N.</param>
		public void SetInterpretation(string interpretationCode){
			if(interpretationCode==null){
				Code="";
				DisplayName="";
				return;
			}
			switch(interpretationCode){
				case "A":	///<summary>0 - Abnormal - Abnormal (for nominal observations, all service types) </summary>
					DisplayName="Abnormal";
					break;
				case "AA":///<summary>1 - Abnormal alert - Abnormal alert (for nominal observations and all service types) </summary>
					DisplayName="Abnormal alert";
					break;
				case "HH":///<summary>2 - High alert - Above upper alert threshold (for quantitative observations) </summary>
					DisplayName="High alert";
					break;
				case "LL":///<summary>3 - Low alert - Below lower alert threshold (for quantitative observations) </summary>
					DisplayName="Low alert";
					break;
				case "H":	///<summary>4 - High - Above high normal (for quantitative observations) </summary>
					DisplayName="High";
					break;
				case "L":	///<summary>5 - Low - Below low normal (for quantitative observations) </summary>
					DisplayName="Low";
					break;
				case "N":	///<summary>6 - Normal - Normal (for all service types) </summary>
					DisplayName="Normal";
					break;
				default:
					Code="";
					DisplayName="";
					return;
			}
			Code=interpretationCode;
		}
			
	}
	

	public class Code {
		///<summary>The actual code snomed, icd-9, or incd10, etc code. Example: 95521008</summary>
		public string Code_;
		///<summary>The HL7-OID of the code system used. Example: 2.16.840.1.113883.6.96 if using SNOMEDCT</summary>
		public string CodeSystem;
		///<summary>The human readable name of the code system used. Example: SNOMEDCT</summary>
		public string CodeSystemName;
		///<summary>The human readable name of the code.  Example: "Abnormal jaw movement (disorder)"</summary>
		public string DisplayName;

		public Code(string c,string cs,string csn,string dn) {
			Code_=c;
			CodeSystem=cs;
			CodeSystemName=csn;
			DisplayName=dn;
		}

		public string ToXml() {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteStartElement("code");
					xmlWriter.WriteAttributeString("code",Code_);
					xmlWriter.WriteAttributeString("codeSystem",CodeSystem);
					xmlWriter.WriteAttributeString("codeSystemName",CodeSystemName);
					xmlWriter.WriteAttributeString("displayName",DisplayName);
				xmlWriter.WriteEndElement();//code
			}
			return stringBuilder.ToString(); ;
		}

	}

	public class Value {
		///<summary>The actual code snomed, icd-9, or incd10, etc code. Example: 95521008</summary>
		public string Code;
		///<summary>The HL7-OID of the code system used. Example: 2.16.840.1.113883.6.96 if using SNOMEDCT</summary>
		public string CodeSystem;
		///<summary>The human readable name of the code system used. Example: SNOMEDCT</summary>
		public string CodeSystemName;
		///<summary>The human readable name of the code.  Example: "Abnormal jaw movement (disorder)"</summary>
		public string DisplayName;

		public Value(string c,string cs,string csn,string dn) {
			Code=c;
			CodeSystem=cs;
			CodeSystemName=csn;
			DisplayName=dn;
		}

		public Value() {

		}

		public string ToXml() {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="  ";
			StringBuilder stringBuilder=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings)) {
				xmlWriter.WriteStartElement("value");
					xmlWriter.WriteAttributeString("code",Code);
					xmlWriter.WriteAttributeString("codeSystem",CodeSystem);
					xmlWriter.WriteAttributeString("codeSystemName",CodeSystemName);
					xmlWriter.WriteAttributeString("displayName",DisplayName);
				xmlWriter.WriteEndElement();//code
				
			}
			return stringBuilder.ToString();
		}

	}//value class

	///<summary>Encounter location and type.</summary>
	public class Component1 {

	}

}