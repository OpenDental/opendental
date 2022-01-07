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
		public Patient PatCur;
		///<summary>Usually filled from within the form by using Patcur.PriProv</summary>
		public Provider ProvCur;
		///<summary>Knowledge request objects, possible object types are: DiseaseDef, Medication, LabResult, ICD9, Icd10, Snomed, RxNorm, Loinc, or LabResult.  Should not break if unsupported objects are in list.</summary>
		private List<KnowledgeRequest> _listKnowledgeRequests;
		//public List<DiseaseDef> ListProblems;//should this be named disease or problem? Also snomed/medication
		//public List<Medication> ListMedications;
		//public List<LabResult> ListLabResults;
		/////<summary>Used to add various codes that are not explicitly related to a problem, medication, or allergy.</summary>
		//public List<Snomed> ListSnomed;
		private ActTaskCode ActTC;//may need to make this public later.
		public ObservationInterpretationNormality ObsInterpretation;
		public ActEncounterCode ActEC;
		public bool UseAge;
		public bool UseAgeGroup;
		public bool PerformerIsProvider;
		public bool RecipientIsProvider;
		private CultureInfo[] arrayCultures;

		public FormInfobutton() {
			_listKnowledgeRequests=new List<KnowledgeRequest>();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public FormInfobutton(List<KnowledgeRequest> listKnowledgeRequests) {
			_listKnowledgeRequests=listKnowledgeRequests;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInfobutton_Load(object sender,EventArgs e) {
			fillLanguageCombos();
			fillEncounterCombo();
			fillTaskCombo();
			fillContext();
			fillKnowledgeRequestitems();
		}

		private void fillLanguageCombos() {
			arrayCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			for(int i=0;i<arrayCultures.Length;i++) {
				comboPatLang.Items.Add(arrayCultures[i].DisplayName);
				comboProvLang.Items.Add(arrayCultures[i].DisplayName);
			}
		}

		private void fillEncounterCombo() {
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

		private void fillTaskCombo() {
			comboTask.Items.Add("Order Entry");
			comboTask.Items.Add("Patient Documentation");
			comboTask.Items.Add("Patient Information Review");
		}

		private void fillContext() {
			//Fill Patient-------------------------------------------------------------------------------------------------------------------
			if(PatCur!=null) {
				textPatName.Text=PatCur.GetNameFL();
				if(PatCur.Birthdate!=DateTime.MinValue) {
					textPatBirth.Text=PatCur.Birthdate.ToShortDateString();
				}
				comboPatLang.SelectedIndex=comboPatLang.Items.IndexOf(System.Globalization.CultureInfo.CurrentCulture.DisplayName);
				switch(PatCur.Gender) {
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
			if(ProvCur==null && PatCur!=null) {
				ProvCur=Providers.GetProv(PatCur.PriProv);
			}
			if(ProvCur==null) {
				ProvCur=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			}
			if(ProvCur!=null) {
				textProvName.Text=ProvCur.GetFormalName();
				textProvID.Text=ProvCur.NationalProvID;
				comboProvLang.SelectedIndex=comboPatLang.Items.IndexOf(System.Globalization.CultureInfo.CurrentCulture.DisplayName);
			}
			//Fill Organization--------------------------------------------------------------------------------------------------------------
			textOrgName.Text=PrefC.GetString(PrefName.PracticeTitle);
			//Fill Encounter-----------------------------------------------------------------------------------------------------------------
			ActEC=ActEncounterCode.AMB;
			comboEncType.SelectedIndex=(int)ActEC;//ambulatory
			if(PatCur!=null) {
				textEncLocID.Text=PatCur.ClinicNum.ToString();//do not use to generate message if this value is zero.
			}
			//Fill Requestor/Recievor--------------------------------------------------------------------------------------------------------
			radioReqProv.Checked=PerformerIsProvider;
			radioReqPat.Checked=!PerformerIsProvider;
			radioRecProv.Checked=RecipientIsProvider;
			radioRecPat.Checked=!RecipientIsProvider;
			//Fill Task Type-----------------------------------------------------------------------------------------------------------------
			ActTC=ActTaskCode.PATINFO;//may need to change this later.
			comboTask.SelectedIndex=(int)ActTC;
		}

		private void fillKnowledgeRequestitems() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Type",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("CodeSystem",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",80);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(KnowledgeRequest knowRequest in _listKnowledgeRequests) {
				row=new GridRow();
				row.Cells.Add(knowRequest.Type);
				row.Cells.Add(knowRequest.Code);
				row.Cells.Add(knowRequest.GetCodeSystemDisplay());
				row.Cells.Add(knowRequest.Description);
				row.Tag=knowRequest;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butPreview_Click(object sender,EventArgs e) {
			if(!isValidHL7DataSet()) {
				return;
			}
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(GenerateKnowledgeRequestNotification());
			msgbox.ShowDialog();
		}

		///<summary>Generates message box with all errors. Returns true if data passes validation or if user decides to "continue anyways".</summary>
		private bool isValidHL7DataSet() {
			string warnings="";//additional data that could be used but is not neccesary.
			string errors="";//additional data that must be present in order to be compliant.
			string message="";
			string bullet="  - ";//should be used at the beggining of every warning/error
			//Patient information-------------------------------------------------------------------------------------------------
			if(PatCur==null) {//should never happen
				warnings+=bullet+Lan.g(this,"No patient selected.")+"\r\n";
			}
			else {
				try {
					PatCur.Birthdate=PIn.Date(textPatBirth.Text);
				}
				catch {

					warnings+=bullet+Lan.g(this,"Birthday.")+"\r\n";
				}
				if(PatCur.Birthdate==DateTime.MinValue) {
					warnings+=bullet+Lan.g(this,"Patient does not have a valid birthday.")+"\r\n";
				}
			}
			//Provider information------------------------------------------------------------------------------------------------
			if(ProvCur==null) {
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
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				w.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				w.WriteWhitespace("\r\n");
				w.WriteStartElement("knowledgeRequestNotification");
					w.WriteAttributeString("classCode","ACT");
					w.WriteAttributeString("moodCode","DEF");
					w.WriteStartElement("id");
						w.WriteAttributeString("value",knowledgeRequestIDHelper());
						w.WriteAttributeString("assigningAuthority",OIDInternals.GetForType(IdentifierType.Root).IDRoot);
					w.WriteEndElement();//id
					w.WriteStartElement("effectiveTime");
						w.WriteAttributeString("value",DateTime.Now.ToString("yyyyMMddhhmmss"));
					w.WriteEndElement();//effectiveTime
					w.WriteStartElement("subject1");
						w.WriteAttributeString("typeCode","SBJ");
						w.WriteStartElement("patientContext");
							w.WriteAttributeString("classCode","PAT");
							w.WriteStartElement("patientPerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
								w.WriteStartElement("administrativeGenderCode");
									w.WriteAttributeString("code",administrativeGenderCodeHelper(PatCur.Gender));
									w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.1");
									w.WriteAttributeString("codeSystemName","administrativeGender");
									w.WriteAttributeString("displayName",administrativeGenderNameHelper(PatCur.Gender));
								w.WriteEndElement();//administrativeGenderCode
							w.WriteEndElement();//patientPerson
						if(PatCur.Birthdate!=DateTime.MinValue){
							w.WriteStartElement("subjectOf");
								w.WriteAttributeString("typeCode","SBJ");
							if(UseAge || UseAge==UseAgeGroup) {//if true or both are false; field is required.
								w.WriteStartElement("age");
									w.WriteAttributeString("classCode","OBS");
									w.WriteAttributeString("moodCode","DEF");
									w.WriteStartElement("code");
										w.WriteAttributeString("code","30525-0");
										w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.1");
										w.WriteAttributeString("codeSystemName","LN");
										w.WriteAttributeString("displayName","AGE");
									w.WriteEndElement();//code
									w.WriteStartElement("value");
										w.WriteAttributeString("value",PatCur.Age.ToString());
										w.WriteAttributeString("unit","a");
									w.WriteEndElement();//value
								w.WriteEndElement();//age
							}
							if(UseAgeGroup || UseAge==UseAgeGroup) {//if true or both are false; field is required.
								w.WriteStartElement("ageGroup");
									w.WriteAttributeString("classCode","OBS");
									w.WriteAttributeString("moodCode","DEF");
									w.WriteStartElement("code");
										w.WriteAttributeString("code","46251-5");
										w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.1");
										w.WriteAttributeString("codeSystemName","LN");
										w.WriteAttributeString("displayName","Age Groups");
									w.WriteEndElement();//code
									w.WriteStartElement("value");
										w.WriteAttributeString("code",AgeGroupCodeHelper(PatCur.Birthdate));
										w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.177");
										w.WriteAttributeString("codeSystemName","MSH");
										w.WriteAttributeString("displayName",AgeGroupNameHelper(PatCur.Birthdate));
									w.WriteEndElement();//value
								w.WriteEndElement();//ageGroup
							}
							w.WriteEndElement();//subjectOf
						}
						w.WriteEndElement();//patientContext
					w.WriteEndElement();//subject1
					w.WriteStartElement("holder");
						w.WriteAttributeString("typeCode","HLD");
						w.WriteStartElement("assignedEntity");
							w.WriteAttributeString("classCode","ASSIGNED");
							w.WriteStartElement("name");
								w.WriteString(Security.CurUser.UserName);
							w.WriteEndElement();//name
							w.WriteStartElement("certificateText");
								w.WriteString(Security.CurUser.PasswordHash);
							w.WriteEndElement();//certificateText
							w.WriteStartElement("assignedAuthorizedPerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
							if(textProvID.Text!=""){
								w.WriteStartElement("id");
									w.WriteAttributeString("value",textProvID.Text);
								w.WriteEndElement();//id
								}
							w.WriteEndElement();//assignedAuthorizedPerson
						if(textOrgID.Text!="" && textOrgName.Text!=""){
							w.WriteStartElement("representedOrganization");
								w.WriteAttributeString("classCode","ORG");
								w.WriteAttributeString("determinerCode","INSTANCE");
							if(textOrgID.Text!=""){
								w.WriteStartElement("id");
									w.WriteAttributeString("value",textOrgID.Text);
								w.WriteEndElement();//id
							}
							if(textOrgName.Text!=""){
								w.WriteStartElement("name");
									w.WriteAttributeString("value",textOrgName.Text);
								w.WriteEndElement();//name
							}
							w.WriteEndElement();//representedOrganization
						}
						w.WriteEndElement();//assignedEntity
					w.WriteEndElement();//holder
				//Performer (Requester)--------------------------------------------------------------------------------------------------------------------------
					w.WriteStartElement("performer");
						w.WriteAttributeString("typeCode","PRF");						
					if(radioReqProv.Checked) {//----performer choice-----
						w.WriteStartElement("healthCareProvider");
							w.WriteAttributeString("classCode","PROV");
							w.WriteStartElement("code");
								w.WriteAttributeString("code","120000000X");
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.101");
								w.WriteAttributeString("codeSystemName","NUCC Health Care Provider Taxonomy");
								w.WriteAttributeString("displayName","Dental Providers");
							w.WriteEndElement();//code
						if((comboProvLang.Text!="" && radioReqProv.Checked) 
							|| (comboPatLang.Text=="" && radioReqPat.Checked))
						{//A missing languageCommunication field invalidates the entire Person class.
							w.WriteStartElement("healthCarePerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
								w.WriteStartElement("languageCommunication");
									w.WriteStartElement("languageCommunicationCode");
										w.WriteAttributeString("code",arrayCultures[comboProvLang.SelectedIndex].ThreeLetterISOLanguageName);
										w.WriteAttributeString("codeSytem","1.0.639.2");
										w.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										w.WriteAttributeString("displayName",arrayCultures[comboProvLang.SelectedIndex].DisplayName);
									w.WriteEndElement();//languageCommunicationCode
								w.WriteEndElement();//languageCommunication
							w.WriteEndElement();//healthCarePerson
							}//end if no language selected.
						w.WriteEndElement();//healthCareProvider
					}
					else {//Performer is patient.
						w.WriteStartElement("patient");
							w.WriteAttributeString("classCode","PAT");
						if((comboProvLang.Text!="" && radioRecProv.Checked) 
							|| (comboPatLang.Text=="" && radioRecPat.Checked))
						{//A missing languageCommunication field invalidates the entire Person class.
							w.WriteStartElement("patientPerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
								w.WriteStartElement("languageCommunication");
									w.WriteStartElement("languageCommunicationCode");
										w.WriteAttributeString("code",arrayCultures[comboPatLang.SelectedIndex].ThreeLetterISOLanguageName);
										w.WriteAttributeString("codeSytem","1.0.639.2");
										w.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										w.WriteAttributeString("displayName",arrayCultures[comboPatLang.SelectedIndex].DisplayName);
									w.WriteEndElement();//languageCommunicationCode
								w.WriteEndElement();//languageCommunication
							w.WriteEndElement();//patientPerson
							}//end if no language selected.
						w.WriteEndElement();//patient
					}
					w.WriteEndElement();//performer
				//InformationRecipient--------------------------------------------------------------------------------------------------------------------------
					w.WriteStartElement("informationRecipient");	
					if(radioRecProv.Checked) {//----performer choice-----
						w.WriteStartElement("healthCareProvider");
							w.WriteAttributeString("classCode","PROV");
							w.WriteStartElement("code");
								w.WriteAttributeString("code","120000000X");
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.101");
								w.WriteAttributeString("codeSystemName","NUCC Health Care Provider Taxonomy");
								w.WriteAttributeString("displayName","Dental Providers");
							w.WriteEndElement();//code
							w.WriteStartElement("healthCarePerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
								w.WriteStartElement("languageCommunication");
									w.WriteStartElement("languageCommunicationCode");
										w.WriteAttributeString("code",arrayCultures[comboProvLang.SelectedIndex].ThreeLetterISOLanguageName);
										w.WriteAttributeString("codeSytem","1.0.639.2");
										w.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										w.WriteAttributeString("displayName",arrayCultures[comboProvLang.SelectedIndex].DisplayName);
									w.WriteEndElement();//languageCommunicationCode
								w.WriteEndElement();//languageCommunication
							w.WriteEndElement();//healthCarePerson
						w.WriteEndElement();//healthCareProvider
					}
					else {//Performer is patient.
						w.WriteStartElement("patient");
							w.WriteAttributeString("classCode","PAT");
							w.WriteStartElement("patientPerson");
								w.WriteAttributeString("classCode","PSN");
								w.WriteAttributeString("determinerCode","INSTANCE");
								w.WriteStartElement("languageCommunication");
									w.WriteStartElement("languageCommunicationCode");
										w.WriteAttributeString("code",arrayCultures[comboPatLang.SelectedIndex].ThreeLetterISOLanguageName);
										w.WriteAttributeString("codeSytem","1.0.639.2");
										w.WriteAttributeString("codeSystemName","ISO 639-2: Codes for the representation of names of languages -- Part 2: Alpha-3 code");
										w.WriteAttributeString("displayName",arrayCultures[comboPatLang.SelectedIndex].DisplayName);
									w.WriteEndElement();//languageCommunicationCode
								w.WriteEndElement();//languageCommunication
							w.WriteEndElement();//patientPerson
						w.WriteEndElement();//patient
					}
					w.WriteEndElement();//informationRecipient
					w.WriteStartElement("subject2");
						w.WriteAttributeString("typeCode","SUBJ");
						w.WriteStartElement("taskContext");
							w.WriteAttributeString("classCode","ACT");
							w.WriteAttributeString("moodCode","DEF");
							w.WriteStartElement("code");
								w.WriteAttributeString("code",ActTaskCodeHelper());
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								w.WriteAttributeString("codeSystemName","ActCode");
								w.WriteAttributeString("displayName",ActTaskCodeNameHelper());
							w.WriteEndElement();//code
						w.WriteEndElement();//taskContext
					w.WriteEndElement();//subject2
					w.WriteStartElement("subject3");
						w.WriteAttributeString("typeCode","SUBJ");
						w.WriteStartElement("subTopic");
							w.WriteAttributeString("classCode","OBS");
							w.WriteAttributeString("moodCode","DEF");
							w.WriteStartElement("code");
								w.WriteAttributeString("code","KSUBT");
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								w.WriteAttributeString("codeSystemName","ActCode");
								w.WriteAttributeString("displayName","knowledge subtopic");
							w.WriteEndElement();//code
							//w.WriteStartElement("value");
							//	w.WriteAttributeString("code","TODO");
							//	w.WriteAttributeString("codeSytem","2.16.840.1.113883.6.177");
							//	w.WriteAttributeString("codeSystemName","MSH");
							//	w.WriteAttributeString("displayName","TODO");
							//w.WriteEndElement();//value
						w.WriteEndElement();//subTopic
					w.WriteEndElement();//subject3
					w.WriteStartElement("subject4");
						w.WriteAttributeString("typeCode","SUBJ");
						w.WriteStartElement("mainSearchCriteria");
							w.WriteAttributeString("classCode","OBS");
							w.WriteAttributeString("moodCode","DEF");
							w.WriteStartElement("code");
								w.WriteAttributeString("code","KSUBJ");
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								w.WriteAttributeString("codeSystemName","ActCode");
								w.WriteAttributeString("displayName","knowledge subject");
							w.WriteEndElement();//code
							w.WriteStartElement("value");
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
							w.WriteEndElement();//value
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
						w.WriteEndElement();//mainSearchCriteria
					w.WriteEndElement();//subject4
					w.WriteStartElement("componentOf");
						w.WriteAttributeString("typeCode","COMP");
						w.WriteStartElement("encounter");
							w.WriteAttributeString("classCode","ENC");
							w.WriteAttributeString("moodCode","DEF");
							w.WriteStartElement("code");
								w.WriteAttributeString("code",EncounterCodeHelper(ActEC));
								w.WriteAttributeString("codeSytem","2.16.840.1.113883.5.4");
								w.WriteAttributeString("codeSystemName","ActCode");
								w.WriteAttributeString("displayName",EncounterCode(ActEC));
							w.WriteEndElement();//code
						if(textEncLocID.Text!=""){
							w.WriteStartElement("location");
							w.WriteAttributeString("typeCode","LOC");
								w.WriteStartElement("serviceDeliveryLocation");
									w.WriteAttributeString("typeCode","SDLOC");
									w.WriteAttributeString("id",textEncLocID.Text);
								w.WriteEndElement();//serviceDeliveryLocation
							w.WriteEndElement();//location
							}
						w.WriteEndElement();//encounter
					w.WriteEndElement();//componentOf
				w.WriteEndElement();//knowledgeRequestNotification
			}
			return strBuilder.ToString();
		}

		#region helper Functions Start

		private string knowledgeRequestIDAAHelper() {
			if(PrefC.GetString(PrefName.PracticeTitle)!="") {
				return PrefC.GetString(PrefName.PracticeTitle);
			}
			return "Open Dental Software, version"+PrefC.GetString(PrefName.ProgramVersion);
		}

		private string knowledgeRequestIDHelper() {
			if(PatCur!=null) {
				return "PT"+PatCur.PatNum+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
			}
			else if(ProvCur!=null) {
				return "PV"+ProvCur.ProvNum+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
			}
			else {
				return "OD"+DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss");
			}
		}

		private string EncounterCodeHelper(ActEncounterCode aec) {
			switch(aec) {
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

		private string EncounterCode(ActEncounterCode aec) {
			switch(aec) {
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

		public string ObservationInterpretationCodeHelper(ObservationInterpretationNormality oin) {
			switch(oin) {
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

		public string ObservationInterpretationNameHelper(ObservationInterpretationNormality oin) {
			switch(oin) {
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
			switch(ActTC) {
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
			switch(ActTC) {
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
			if(PatCur.Birthdate.AddMonths(1)>DateTime.Now) {//less than 1mo old, newborn
				return "D007231";
			}
			else if(PatCur.Birthdate.AddYears(2)>DateTime.Now) {//less than 2 yrs old, Infant
				return "D007223";
			}
			else if(PatCur.Birthdate.AddYears(5)>DateTime.Now) {//2 to 5 yrs old, Preschool
				return "D007675";
			}
			else if(PatCur.Birthdate.AddYears(12)>DateTime.Now) {//6 to 12 yrs old, Child
				return "D002648";
			}
			else if(PatCur.Birthdate.AddYears(18)>DateTime.Now) {//13 to 18 yrs old, Adolescent
				return "D000293";
			}
			else if(PatCur.Birthdate.AddYears(44)>DateTime.Now) {//19 to 44 yrs old, Adult
				return "D000328";
			}
			else if(PatCur.Birthdate.AddYears(64)>DateTime.Now) {//45 to 64 yrs old, Middle Aged
				return "D008875";
			}
			else if(PatCur.Birthdate.AddYears(79)>DateTime.Now) {//65 to 79 yrs old, Aged
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
			if(PatCur.Birthdate.AddMonths(1)>DateTime.Now) {//less than 1mo old, newborn
				return "Newborn";
			}
			else if(PatCur.Birthdate.AddYears(2)>DateTime.Now) {//less than 2 yrs old, Infant
				return "Infant";
			}
			else if(PatCur.Birthdate.AddYears(5)>DateTime.Now) {//2 to 5 yrs old, Preschool
				return "Preschool";
			}
			else if(PatCur.Birthdate.AddYears(12)>DateTime.Now) {//6 to 12 yrs old, Child
				return "Child";
			}
			else if(PatCur.Birthdate.AddYears(18)>DateTime.Now) {//13 to 18 yrs old, Adolescent
				return "Adolescent";
			}
			else if(PatCur.Birthdate.AddYears(44)>DateTime.Now) {//19 to 44 yrs old, Adult
				return "Adult";
			}
			else if(PatCur.Birthdate.AddYears(64)>DateTime.Now) {//45 to 64 yrs old, Middle Aged
				return "Middle Aged";
			}
			else if(PatCur.Birthdate.AddYears(79)>DateTime.Now) {//65 to 79 yrs old, Aged
				return "Aged";
			}
			else { //if(PatCur.Birthdate.AddYears(79)>DateTime.Now) {//80 yrs old or older, Aged, 80 and over
				return "Aged, 80 and over";
			}
		}

		///<summary>The gender of a person used for adminstrative purposes (as opposed to clinical gender). Empty string/value is allowed.</summary>
		public string administrativeGenderCodeHelper(PatientGender patientGender) {
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
		public string administrativeGenderNameHelper(PatientGender patientGender) {
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
			ActEC=(ActEncounterCode)comboEncType.SelectedIndex;
		}

		private void comboTask_SelectedIndexChanged(object sender,EventArgs e) {
			ActTC=(ActTaskCode)comboTask.SelectedIndex;
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
			using FormDiseaseDefs FormDD = new FormDiseaseDefs();
			FormDD.IsSelectionMode=true;
			FormDD.ShowDialog();
			if(FormDD.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			EhrTriggers.ConvertToKnowledgeRequests(FormDD.ListDiseaseDefsSelected[0]).ForEach(x=>_listKnowledgeRequests.Add(x));
			fillKnowledgeRequestitems();
		}

		private void butAddSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS = new FormSnomeds();
			FormS.IsMultiSelectMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormS.ListSelectedSnomeds.Count;i++) {
				EhrTriggers.ConvertToKnowledgeRequests(FormS.ListSelectedSnomeds[i]).ForEach(x=>_listKnowledgeRequests.Add(x));
			}			
			fillKnowledgeRequestitems();
		}

		private void butAddRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms FormRXN=new FormRxNorms();
			FormRXN.IsMultiSelectMode=true;
			FormRXN.ShowDialog();
			if(FormRXN.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormRXN.ListSelectedRxNorms.Count;i++) {
				EhrTriggers.ConvertToKnowledgeRequests(FormRXN.ListSelectedRxNorms[i]).ForEach(x=>_listKnowledgeRequests.Add(x));
			}	
			fillKnowledgeRequestitems();
		}

		private void butAddIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s FormI9=new FormIcd9s();
			FormI9.IsSelectionMode=true;
			FormI9.ShowDialog();
			if(FormI9.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggers.ConvertToKnowledgeRequests(FormI9.SelectedIcd9).ForEach(x=>_listKnowledgeRequests.Add(x));
			fillKnowledgeRequestitems();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergySetup FormA=new FormAllergySetup();
			FormA.IsSelectionMode=true;
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggers.ConvertToKnowledgeRequests(AllergyDefs.GetOne(FormA.SelectedAllergyDefNum)).ForEach(x => _listKnowledgeRequests.Add(x));
			fillKnowledgeRequestitems();
		}

		private void butAddIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s FormI10=new FormIcd10s();
			FormI10.IsSelectionMode=true;
			FormI10.ShowDialog();
			if(FormI10.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggers.ConvertToKnowledgeRequests(FormI10.SelectedIcd10).ForEach(x => _listKnowledgeRequests.Add(x));
			fillKnowledgeRequestitems();
		}

		private void butAddLoinc_Click(object sender,EventArgs e) {
			using FormLoincs FormL=new FormLoincs();
			FormL.IsSelectionMode=true;
			FormL.ShowDialog();
			if(FormL.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggers.ConvertToKnowledgeRequests(FormL.SelectedLoinc).ForEach(x => _listKnowledgeRequests.Add(x));
			fillKnowledgeRequestitems();
		}

		private void butPreviewRequest_Click(object sender,EventArgs e) {
			KnowledgeRequestNotification.KnowledgeRequestNotification krn;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(gridMain.ListGridRows[i].Tag==null){
					MsgBox.Show(this,"Cannot search without a valid code.");
					continue;
				}
				krn=new KnowledgeRequestNotification.KnowledgeRequestNotification();
				krn.AddKnowledgeRequest((KnowledgeRequest)gridMain.ListGridRows[i].Tag);
				krn.performerIsPatient=radioReqPat.Checked;
				krn.recipientIsPatient=radioRecPat.Checked;
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste("http://apps.nlm.nih.gov/medlineplus/services/mpconnect.cfm?"+krn.ToUrl());
				msgbox.ShowDialog();
				//msgbox=new MsgBoxCopyPaste("http://apps2.nlm.nih.gov/medlineplus/services/servicedemo.cfm?"+krn.ToUrl());
				//msgbox.ShowDialog();
			}//end gridMain.Rows

		}

		private void butSend_Click(object sender,EventArgs e) {
			KnowledgeRequestNotification.KnowledgeRequestNotification krn;
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==null) {
					MsgBox.Show(this,"Cannot search without a valid code.");
					continue;
				}
				krn=new KnowledgeRequestNotification.KnowledgeRequestNotification();
				krn.AddKnowledgeRequest((KnowledgeRequest)gridMain.ListGridRows[i].Tag);
				krn.performerIsPatient=radioReqPat.Checked;
				krn.recipientIsPatient=radioRecPat.Checked;
				//using FormInfobuttonResponse FormIR = new FormInfobuttonResponse();
				//FormIR.RawResponse=getWebResponse("http://apps2.nlm.nih.gov/medlineplus/services/mpconnect_service.cfm?"+krn.ToUrl());
				//FormIR.ShowDialog();
				//The lines commented out here launch the infobutton in the default browser.
				try {
					System.Diagnostics.Process.Start("http://apps.nlm.nih.gov/medlineplus/services/mpconnect.cfm?"+krn.ToUrl());
					//System.Diagnostics.Process.Start("http://apps2.nlm.nih.gov/medlineplus/services/servicedemo.cfm?"+krn.ToUrl());
				}
				catch(Exception) { }
			}//end gridMain.Rows

		}

		///<summary>For More info goto: http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx </summary>
		private static string getWebResponse(string url) {
			// Create a request for the URL. 
			WebRequest request = WebRequest.Create(url);
			// If required by the server, set the credentials.
			//request.Credentials = CredentialCache.DefaultCredentials;
			// Get the response.
			WebResponse response = request.GetResponse();
			// Display the status.
			Console.WriteLine(((HttpWebResponse)response).StatusDescription);
			// Get the stream containing content returned by the server.
			Stream dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();
			// Clean up the streams and the response.
			reader.Close();
			response.Close();
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
		public static string classCode="ACT";		//1..1
		///<summary>Static field "DEF".  A definition of a service (master).  Cardinality [1..1]</summary>
		public static string moodCode="DEF";						//1..1
		///<summary>List of globally unique identifiers of this knowledge request.  Cardinality [0..*]</summary>
		public List<Id> IdList=new List<Id>();	//0..*
		///<summary>Creation time of the knowledge request.  Must be formatted "yyyyMMddhhmmss" when used.  Cardinality [0..1]</summary>
		public DateTime effectiveTime;					//0..1 "yyyyMMddhhmmss"
		///<summary>Patient context information. Cardinality [0..1]</summary>
		public Subject subject1;								//0..1
		///<summary>Not fully implemented. Implemented enough to work.</summary>
		public bool performerIsPatient;
		///<summary>Not fully implemented. Implemented enough to work.</summary>
		public bool recipientIsPatient;

		//public Performer performer;

		//public InformationRecipient informationRecipient;


		///<summary>Task context information. Cardinality [0..1]</summary>
		public Subject1 subject2;								//0..1
		///<summary>Sub-topic information. Cardinality [0..1]</summary>
		public Subject2 subject3;								//0..1
		///<summary>Conatins a list of MainSearchCriteria: represents the main subject of interest in a knowledge request (e.g., a medication, a lab test result, a disease in the patient's problem list). When multiple multiple search criteria are present, knowledge resources MAY determine whether to join the multiple instances using the AND vs. OR Boolean operator. Cardinality[1..*]</summary>
		public List<Subject3> subject4List;			//1..*
		///<summary>Contains encounter information, type and location.  Cardinality[0..1]</summary>
		public Component1 componentOf;					//0..1

		public KnowledgeRequestNotification() {
			classCode="ACT";
			moodCode="DEF";
			IdList=new List<Id>();
			effectiveTime=DateTime.Now;
			subject1=new Subject();
			subject2=new Subject1();
			subject3=new Subject2();
			subject4List=new List<Subject3>();
			componentOf=new Component1();
		}

		public void AddKnowledgeRequest(KnowledgeRequest knowRequest) {
			switch(knowRequest.CodeSystem) {
				case CodeSyst.Snomed:
					subject4List.Add(new Subject3(new Value(knowRequest.Code,"2.16.840.1.113883.6.96","SNOMEDCT",knowRequest.Description)));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description;
					break;
				case CodeSyst.Icd9:
					subject4List.Add(new Subject3(new Value(knowRequest.Code,"2.16.840.1.113883.6.103","ICD9CM",knowRequest.Description)));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description;
					break;
				case CodeSyst.Icd10:
					subject4List.Add(new Subject3(new Value(knowRequest.Code,"2.16.840.1.113883.6.90","ICD10CM",knowRequest.Description)));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description;
					break;
				case CodeSyst.RxNorm:
					subject4List.Add(new Subject3(new Value(knowRequest.Code,"2.16.840.1.113883.6.88","RxNorm",knowRequest.Description)));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description;
					break;
				case CodeSyst.Loinc:
					subject4List.Add(new Subject3(new Value(knowRequest.Code,"2.16.840.1.113883.6.1","LOINC",knowRequest.Description)));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description;
					break;
				case CodeSyst.AllergyDef:
					subject4List.Add(new Subject3(new Value("","","",knowRequest.Description.Replace("Allergy -",""))));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description.Replace("Allergy -","");
					break;
				case CodeSyst.ProblemDef:
					subject4List.Add(new Subject3(new Value("","","",knowRequest.Description.Replace("Problem -",""))));
					subject4List[subject4List.Count-1].mainSearchCriteria.originalText=knowRequest.Description.Replace("Problem -","");
					break;
					//case "LabResult"://Deprecated
					//	AddLabResult((LabResult)obj);
					//	break;
			}
		}	

		public string ToXml() {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				w.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				w.WriteWhitespace("\r\n");
				//TODO:Implement more fields, this is just
				w.WriteStartElement("knowledgeRequestNotification");
					w.WriteAttributeString("classCode","ACT");
					w.WriteAttributeString("moodCode","DEF");
					//id
					//effectiveTime
					//subject1
					//holder
					//performer
					//informationRecipient
					//subject2
					//subject3
					w.WriteRaw(Subject3.ToXml(subject4List));
					//componentOf
				w.WriteEndElement();//knowledgeRequestNotification
			}
			return strBuilder.ToString();
		}

		public string ToUrl() {
			StringBuilder strB=new StringBuilder();
			strB.Append((effectiveTime.Year>1880?"knowledgeRequestNotification.effectiveTime.v="+effectiveTime.ToString("yyyyMMddhhmmss")+"&":""));
			//holder
			//assignedEntity
			//patientPerson
			//age
			//ageGroup
			//taskContext
			//subTopic
			for(int i=0;i<subject4List.Count;i++) {
				strB.Append("mainSearchCriteria.v.c"+(i==0?"":""+i)+"="+subject4List[i].mainSearchCriteria.value.code+"&");
				strB.Append("mainSearchCriteria.v.cs"+(i==0?"":""+i)+"="+subject4List[i].mainSearchCriteria.value.codeSystem+"&");
				strB.Append("mainSearchCriteria.v.dn"+(i==0?"":""+i)+"="+subject4List[i].mainSearchCriteria.value.displayName+"&");
				if(subject4List[i].mainSearchCriteria.originalText!=subject4List[i].mainSearchCriteria.value.displayName) {//original text only if different than display name.
					strB.Append("mainSearchCriteria.v.ot"+(i==0?"":""+i)+"="+subject4List[i].mainSearchCriteria.originalText+"&");
				}
				//severityObservation
			}
			//informationRecipient
			strB.Append("informationRecipient="+(recipientIsPatient?"PAT&":"PROV&"));
			//performer
			strB.Append("performer="+(performerIsPatient?"PAT&":"PROV&"));
			//encounter
			//serviceDeliveryLocation
			return strB.ToString().Replace(" ","%20");
		}

		public string ToUrl(string xml) {
			StringBuilder strBuilder=new StringBuilder();
			//TODO later, maybe
			//XmlDocument doc=new XmlDocument();
			//doc.LoadXml(xml);
			//XmlNode node=doc.SelectSingleNode("//Error");
			//if(node!=null) {
			//	throw new Exception(node.InnerText);
			//}
			return strBuilder.ToString();
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
		public string typeCode;
		public MainSearchCriteria mainSearchCriteria;

		public Subject3() {
			typeCode="SUBJ";
			mainSearchCriteria=new MainSearchCriteria();
		}

		public Subject3(Value value) {
			typeCode="SUBJ";
			mainSearchCriteria=new MainSearchCriteria(value);
		}

		public static string ToXml(List<Subject3> subject4List) {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				for(int i=0;i<subject4List.Count;i++){
					w.WriteStartElement("subject4");
						w.WriteAttributeString("typeCode",subject4List[i].typeCode);
						w.WriteRaw(subject4List[i].mainSearchCriteria.ToXml());
					w.WriteEndElement();
				}//end subject4List
			}//end using
			return strBuilder.ToString();
		}
	}

	public class MainSearchCriteria {
		///<summary>Static field "OBS".  Observation.  Cardinality [1..1]</summary>
		public static string classCode;
		///<summary>Static field "DEF".  A definition of a service (master).  Cardinality [1..1]</summary>
		public static string moodCode;
		///<summary>Static field.  This defines the value as being a knowledge subject.  Cardinality [1..1]</summary>
		public Code code;
		///<summary>Contains information on the snomed in question, icd9, icd10 ... etc code.  The "value" of the "code".  Cardinality [1..1]</summary>
		public Value value;
		///<summary>Represents the human readable representation of the code as displayed to the user in the CIS and SHOULD be used only if different than the displayName</summary>
		public string originalText;
		///<summary>Contains SeverityObservation:specifies the interpretation of a laboratory test result (e.g., 'high', 'low', 'abnormal', 'normal'). This class MAY be used to support implementations where the MainSearchCriteria consists of a laboratory test result. Supports questions such as "what are the causes of high serum potassium?</summary>
		public Subject4 subject;

		public MainSearchCriteria() {
			classCode="OBS";
			moodCode="DEF";
			code=new Code("KSUBJ","2.16.840.1.113883.6.96","SNOMEDCT","knowledge subject");
			value=new Value();
			subject = new Subject4();
		}

		public MainSearchCriteria(Value val) {
			classCode="OBS";
			moodCode="DEF";
			code=new Code("KSUBJ","2.16.840.1.113883.6.96","SNOMEDCT","knowledge subject");
			value=val;
			subject = new Subject4();
		}

		public string ToXml() {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				w.WriteStartElement("mainSearchCriteria");
					w.WriteAttributeString("classCode",classCode);
					w.WriteAttributeString("moodCode",moodCode);
					w.WriteRaw(code.ToXml());
					w.WriteRaw(value.ToXml());
					if(originalText!="" && originalText!=null && originalText!=value.displayName){
						w.WriteStartElement("originalText");
							w.WriteString(originalText);
						w.WriteEndElement();//originalText
					}
				w.WriteEndElement();//mainSearchCriteria
			}
			return strBuilder.ToString();
		}

	}//MainSearchCriteria class

	public class Subject4 {
		public static string typeCode;
		public SeverityObservation severityObservation;

		public Subject4(){
			typeCode="SUBJ";
			severityObservation=new SeverityObservation();
		}
	}

	public class SeverityObservation {
		public static string classCode;
		public static string moodCode;
		public Code code;
		public ObservationInterpretationNormality observationInterpretationNormality;

		public SeverityObservation() {
			classCode="OBS";
			moodCode="DEF";
			code=new Code("SeverityObservationType","2.16.840.1.113883.5.6","ActClass","SeverityObservationType");
			observationInterpretationNormality=null;
		}
	}

	///<summary>Normality, Abnormality, Alert. Concepts in this category are mutually exclusive, i.e., at most one is allowed. Enum generated from HL7 _ObservationInterpretationNormality [2.16.840.1.113883.1.11.10206] which is a subset of ObservationInterpretation [OID=2.16.840.1.113883.5.83] documentation published 20120831 10:21 AM.</summary>
	public class ObservationInterpretationNormality {
		public string code;
		public static string codeSystem;
		public static string codeSystemName;
		public string displayName;

		public ObservationInterpretationNormality(){
			code="";
			codeSystem="2.16.840.1.113883.5.83";
			codeSystemName="ObservationInterpretation";
			displayName="";
		}

		public void SetInterpretation(LabAbnormalFlag laf) {
			switch(laf) {
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
				code="";
				displayName="";
				return;
			}
			switch(interpretationCode){
				case "A":	///<summary>0 - Abnormal - Abnormal (for nominal observations, all service types) </summary>
					displayName="Abnormal";
					break;
				case "AA":///<summary>1 - Abnormal alert - Abnormal alert (for nominal observations and all service types) </summary>
					displayName="Abnormal alert";
					break;
				case "HH":///<summary>2 - High alert - Above upper alert threshold (for quantitative observations) </summary>
					displayName="High alert";
					break;
				case "LL":///<summary>3 - Low alert - Below lower alert threshold (for quantitative observations) </summary>
					displayName="Low alert";
					break;
				case "H":	///<summary>4 - High - Above high normal (for quantitative observations) </summary>
					displayName="High";
					break;
				case "L":	///<summary>5 - Low - Below low normal (for quantitative observations) </summary>
					displayName="Low";
					break;
				case "N":	///<summary>6 - Normal - Normal (for all service types) </summary>
					displayName="Normal";
					break;
				default:
					code="";
					displayName="";
					return;
			}
			code=interpretationCode;
		}
			
	}
	

	public class Code {
		///<summary>The actual code snomed, icd-9, or incd10, etc code. Example: 95521008</summary>
		public string code;
		///<summary>The HL7-OID of the code system used. Example: 2.16.840.1.113883.6.96 if using SNOMEDCT</summary>
		public string codeSystem;
		///<summary>The human readable name of the code system used. Example: SNOMEDCT</summary>
		public string codeSystemName;
		///<summary>The human readable name of the code.  Example: "Abnormal jaw movement (disorder)"</summary>
		public string displayName;

		public Code(string c,string cs,string csn,string dn) {
			code=c;
			codeSystem=cs;
			codeSystemName=csn;
			displayName=dn;
		}

		public string ToXml() {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				w.WriteStartElement("code");
					w.WriteAttributeString("code",code);
					w.WriteAttributeString("codeSystem",codeSystem);
					w.WriteAttributeString("codeSystemName",codeSystemName);
					w.WriteAttributeString("displayName",displayName);
				w.WriteEndElement();//code
			}
			return strBuilder.ToString(); ;
		}

	}

	public class Value {
		///<summary>The actual code snomed, icd-9, or incd10, etc code. Example: 95521008</summary>
		public string code;
		///<summary>The HL7-OID of the code system used. Example: 2.16.840.1.113883.6.96 if using SNOMEDCT</summary>
		public string codeSystem;
		///<summary>The human readable name of the code system used. Example: SNOMEDCT</summary>
		public string codeSystemName;
		///<summary>The human readable name of the code.  Example: "Abnormal jaw movement (disorder)"</summary>
		public string displayName;

		public Value(string c,string cs,string csn,string dn) {
			code=c;
			codeSystem=cs;
			codeSystemName=csn;
			displayName=dn;
		}

		public Value() {

		}

		public string ToXml() {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="  ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter w=XmlWriter.Create(strBuilder,xmlSettings)) {
				w.WriteStartElement("value");
					w.WriteAttributeString("code",code);
					w.WriteAttributeString("codeSystem",codeSystem);
					w.WriteAttributeString("codeSystemName",codeSystemName);
					w.WriteAttributeString("displayName",displayName);
				w.WriteEndElement();//code
				
			}
			return strBuilder.ToString();
		}

	}//value class

	///<summary>Encounter location and type.</summary>
	public class Component1 {

	}

}