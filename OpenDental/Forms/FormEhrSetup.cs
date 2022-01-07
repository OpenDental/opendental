using Ionic.Zip;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental {
	public partial class FormEhrSetup:FormODBase {
		public FormEhrSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrSetup_Load(object sender,EventArgs e) {
			LayoutMenu();
			if(PrefC.GetBool(PrefName.EhrEmergencyNow)) {
				panelEmergencyNow.BackColor=Color.Red;
			}
			else {
				panelEmergencyNow.BackColor=SystemColors.Control;
			}
			if(!Security.IsAuthorized(Permissions.Setup,true)) {
				//Hide all the buttons except Emergency Now and Close.
				//Unhiding all code system buttons since code systems can no longer be edited.
				butAllergies.Visible=false;
				//Forumularies will now be checked through New Crop
				//butFormularies.Visible=false;
				butVaccineDef.Visible=false;
				butDrugManufacturer.Visible=false;
				butDrugUnit.Visible=false;
				butInboundEmail.Visible=false;
				butReminderRules.Visible=false;
				butEducationalResources.Visible=false;
				menuMain.Enabled=false;
				butTimeSynch.Visible=false;
				butEhrTriggers.Visible=false;
			}
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Settings",menuItemSettings_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSettings_Click(object sender,EventArgs e) {
			using FormEhrSettings FormES=new FormEhrSettings();
			FormES.ShowDialog();
		}

		private void butICD9s_Click(object sender,EventArgs e) {
			using FormIcd9s FormE=new FormIcd9s();
			FormE.ShowDialog();
		}

		private void butAllergies_Click(object sender,EventArgs e) {
			using FormAllergySetup FAS=new FormAllergySetup();
			FAS.ShowDialog();
		}

		//Formularies will now be checked through New Crop
		//private void butFormularies_Click(object sender,EventArgs e) {
		//	using FormFormularies FormE=new FormFormularies();
		//	FormE.ShowDialog();
		//}

		private void butVaccineDef_Click(object sender,EventArgs e) {
			using FormVaccineDefSetup FormE=new FormVaccineDefSetup();
			FormE.ShowDialog();
		}

		private void butDrugManufacturer_Click(object sender,EventArgs e) {
			using FormDrugManufacturerSetup FormE=new FormDrugManufacturerSetup();
			FormE.ShowDialog();
		}

		private void butDrugUnit_Click(object sender,EventArgs e) {
			using FormDrugUnitSetup FormE=new FormDrugUnitSetup();
			FormE.ShowDialog();
		}

		private void butInboundEmail_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEA=new FormEmailAddresses();
			formEA.ShowDialog();
		}

		private void butEmergencyNow_Click(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.EhrEmergencyNow)) {
				panelEmergencyNow.BackColor=SystemColors.Control;
				Prefs.UpdateBool(PrefName.EhrEmergencyNow,false);
			}
			else {
				panelEmergencyNow.BackColor=Color.Red;
				Prefs.UpdateBool(PrefName.EhrEmergencyNow,true);
			}
			DataValid.SetInvalid(InvalidType.Prefs);
		}
		
		private void butReminderRules_Click(object sender,EventArgs e) {
			using FormReminderRules FormRR = new FormReminderRules();
			FormRR.ShowDialog();
		}

		private void butEducationalResources_Click(object sender,EventArgs e) {
			using FormEduResourceSetup FormEDUSetup = new FormEduResourceSetup();
			FormEDUSetup.ShowDialog();
		}

		private void butRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms FormR=new FormRxNorms();
			FormR.ShowDialog();
		}

		private void butLoincs_Click(object sender,EventArgs e) {
			using FormLoincs FormL=new FormLoincs();
			FormL.ShowDialog();
		}

		private void butSnomeds_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			FormS.ShowDialog();
		}

		private void butTimeSynch_Click(object sender,EventArgs e) {
			using FormEhrTimeSynch formET = new FormEhrTimeSynch();
			formET.ShowDialog();
		}

		private void butPortalSetup_Click(object sender,EventArgs e) {
			using FormEServicesPatientPortal formESPatPortal=new FormEServicesPatientPortal();
			formESPatPortal.ShowDialog();
		}

		private void butCodeImport_Click(object sender,EventArgs e) {
			using FormCodeSystemsImport FormCSI=new FormCodeSystemsImport();
			FormCSI.ShowDialog();
		}

		private void butProviderKeys_Click(object sender,EventArgs e) {
			using FormEhrProviderKeys formK=new FormEhrProviderKeys();
			formK.ShowDialog();
		}

		private static string downloadFileHelper(string codeSystemURL,string codeSystemName) {
			string zipFileDestination=PrefC.GetRandomTempFile(".tmp");//@"c:\users\ryan\desktop\"+codeSystemName+".tmp";
			File.Delete(zipFileDestination);
			WebRequest wr=WebRequest.Create(codeSystemURL);
			WebResponse webResp=null;
			try {
				webResp=wr.GetResponse();
			}
			catch(Exception ex) {
				ex.DoNothing();
				return null;
			}
			DownloadFileWorker(codeSystemURL,zipFileDestination);
			Thread.Sleep(100);//allow file to be released for use by the unzipper.
			//Unzip the compressed file-----------------------------------------------------------------------------------------------------
			MemoryStream ms=new MemoryStream();
			using(ZipFile unzipped=ZipFile.Read(zipFileDestination)) {
				ZipEntry ze=unzipped[0];
				ze.Extract(PrefC.GetTempFolderPath(),ExtractExistingFileAction.OverwriteSilently);
				return ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),unzipped[0].FileName);
			}
		}

		///<summary>This is the function that the worker thread uses to actually perform the download.  Can also call this method in the ordinary way if the file to be transferred is short.</summary>
		private static void DownloadFileWorker(string downloadUri,string destinationPath) {
			int chunk=10;//KB
			byte[] buffer;
			int i=0;
			WebClient myWebClient=new WebClient();
			Stream readStream=myWebClient.OpenRead(downloadUri);
			BinaryReader br=new BinaryReader(readStream);
			FileStream writeStream=new FileStream(destinationPath,FileMode.Create);
			BinaryWriter bw=new BinaryWriter(writeStream);
			try {
				while(true) {
					buffer=br.ReadBytes(chunk*1024);
					if(buffer.Length==0) {
						break;
					}
					bw.Write(buffer);
					i++;
				}
			}
			catch {//for instance, if abort.
				br.Close();
				bw.Close();
				File.Delete(destinationPath);
			}
			finally {
				br.Close();
				bw.Close();
			}
			//myWebClient.DownloadFile(downloadUri,ODFileUtils.CombinePaths(FormPath.GetPreferredImagePath(),"Setup.exe"));
		}

		private static string SendAndReceiveDownloadXml(string codeSystemName) {
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				//TODO: include more user information
				writer.WriteStartElement("UpdateRequest");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("PracticeTitle");
				writer.WriteString(PrefC.GetString(PrefName.PracticeTitle));
				writer.WriteEndElement();
				writer.WriteStartElement("PracticeAddress");
				writer.WriteString(PrefC.GetString(PrefName.PracticeAddress));
				writer.WriteEndElement();
				writer.WriteStartElement("PracticePhone");
				writer.WriteString(PrefC.GetString(PrefName.PracticePhone));
				writer.WriteEndElement();
				writer.WriteStartElement("ProgramVersion");
				writer.WriteString(PrefC.GetString(PrefName.ProgramVersion));
				writer.WriteEndElement();
				writer.WriteStartElement("CodeSystemRequested");
				writer.WriteString(codeSystemName);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
#if DEBUG
			OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
			updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress) !="") {
				IWebProxy proxy = new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				proxy.Credentials=cred;
				updateService.Proxy=proxy;
			}
			string result="";
			try {
				result=updateService.RequestCodeSystemDownload(strbuild.ToString());//may throw error
			}
			catch(Exception ex) {
				//Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return "";
			}
			return result;
		}

		private void butCdsTriggers_Click(object sender,EventArgs e) {
			using FormCdsTriggers FormET=new FormCdsTriggers();
			FormET.ShowDialog();
		}

		private void butOIDs_Click(object sender,EventArgs e) {
			using FormOIDRegistryInternal FormOIDI=new FormOIDRegistryInternal();
			FormOIDI.ShowDialog();
		}


		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private List<string> getTestMessagesHelper(){
			List<string> retVal=new List<string>();
			#region LRI_1.0-GU_Final
			retVal.Add(@"MSH|^~\&#|NIST Test Lab APP^2.16.840.1.113883.3.72.5.20^ISO|NIST Lab Facility^2.16.840.1.113883.3.72.5.21^ISO||NIST EHR Facility^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-001.00|T|2.5.1|||AL|NE|||||LRI_Common_Component^Profile Component^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^Profile Component^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^Profile Component^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^NIST MPI&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A^JR^^^L||19610615|M||2106-3^White^HL70005^CAUC^Caucasian^L
ORC|RE|ORD723222^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO|R-783274^NIST Lab Filler^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^M^JR^DR^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD723222^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO|R-783274^NIST Lab Filler^2.16.840.1.113883.3.72.5.25^ISO|30341-2^Erythrocyte sedimentation rate^LN^815115^Erythrocyte sedimentation rate^99USI^^^Erythrocyte sedimentation rate|||20110331140551-0800||||L||7520000^fever of unknown origin^SCT^22546000^fever, origin unknown^99USI^^^Fever of unknown origin|||57422^Radon^Nicholas^M^JR^DR^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110331160428-0800|||F|||10092^Hamlin^Pafford^M^Sr.^Dr.^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI|||||||||||||||||||||CC^Carbon Copy^HL70507^C^Send Copy^L^^^Copied Requested
NTE|1||Patient is extremely anxious about needles used for drawing blood.
TQ1|1||||||20110331150028-0800|20110331152028-0800
OBX|1|NM|30341-2^Erythrocyte sedimentation rate^LN^815117^ESR^99USI^^^Erythrocyte sedimentation rate||10|mm/h^millimeter per hour^UCUM|0 to 17|N|||F|||20110331140551-0800|||||20110331150551-0800||||Century Hospital^^^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^USA^B^^06037|2343242^Knowsalot^Phil^J.^III^Dr.^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
SPM|1|||119297000^BLD^SCT^BldSpc^Blood^99USA^^^Blood Specimen|||||||||||||20110331140551-0800|||||||COOL^Cool^HL70493^CL^Cool^99USA^^^Cool");
			#endregion
			#region LRI_1.1-GU_Corrected
			retVal.Add(@"MSH|^~\&#|NIST Test Lab APP^2.16.840.1.113883.3.72.5.20^ISO|NIST Lab Facility^2.16.840.1.113883.3.72.5.21^ISO||NIST EHR Facility^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-001.01|T|2.5.1|||AL|NE|||||LRI_Common_Component^Profile Component^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^Profile Component^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^Profile Component^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^NIST MPI&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A^JR^^^L||19610615|M||2106-3^White^HL70005^CAUC^Caucasian^L
ORC|RE|ORD723222^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO|R-783274^NIST Lab Filler^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^M^JR^DR^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD723222^NIST EHR^2.16.840.1.113883.3.72.5.24^ISO|R-783274^NIST Lab Filler^2.16.840.1.113883.3.72.5.25^ISO|30341-2^Erythrocyte sedimentation rate^LN^815115^Erythrocyte sedimentation rate^99USI^^^Erythrocyte sedimentation rate|||20110331140551-0800||||L||7520000^fever of unknown origin^SCT^22546000^fever, origin unknown^99USI^^^Fever of unknown origin|||57422^Radon^Nicholas^M^JR^DR^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110331160428-0800|||C|||10092^Hamlin^Pafford^M^Sr.^Dr.^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI|||||||||||||||||||||CC^Carbon Copy^HL70507^C^Send Copy^L^^^Copied Requested
NTE|1||Patient is extremely anxious about needles used for drawing blood.
TQ1|1||||||20110331150028-0800|20110331152028-0800
OBX|1|NM|30341-2^Erythrocyte sedimentation rate^LN^815117^ESR^99USI^^^Erythrocyte sedimentation rate||20|mm/h^millimeter per hour^UCUM|0 to 17|H|||C|||20110331140551-0800|||||20110331150551-0800||||Century Hospital^^^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^USA^B^^06037|2343242^Knowsalot^Phil^J.^III^Dr.^^^NIST-AA-1&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
NTE|1||Specimen re-analyzed per request of ordering provider.
SPM|1|||119297000^BLD^SCT^BldSpc^Blood^99USA^^^Blood Specimen|||||||||||||20110331140551-0800|||||||COOL^Cool^HL70493^CL^Cool^99USA^^^Cool");
			#endregion
			#region LRI_1.2-GU_Rejected
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-001.04|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1236^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Anderson^Janet||19860930|F||2106-3^White^HL70005
ORC|RE|ORD723222-1^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-1^^2.16.840.1.113883.3.72.5.25^ISO|GORD874222^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD723222-1^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-1^^2.16.840.1.113883.3.72.5.25^ISO|30341-2^Erythrocyte sedimentation rate^LN^815115^Erythrocyte sedimentation rate^99USI^^^Erythrocyte sedimentation rate|||20110331140551-0800|||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110331160428-0800|||X
SPM|1|||119297000^BLD^SCT^BldSpc^Blood^99USA^^^Blood Specimen|||||||||||||20110331140551-0800||||QS^Quantity not sufficient^HL70490^NSQ^Not sufficient quantity^99USA^^^Quantity not sufficient|||COOL^Cool^HL70493^CL^Cool^99USA^^^Cool");
			#endregion
			#region LRI_2.0-GU_Typ
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-002.00|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A||19610615|M||2106-3^White^HL70005
ORC|RE|ORD666555^^2.16.840.1.113883.3.72.5.24^ISO|R-991133^^2.16.840.1.113883.3.72.5.25^ISO|GORD874233^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD666555^^2.16.840.1.113883.3.72.5.24^ISO|R-991133^^2.16.840.1.113883.3.72.5.25^ISO|57021-8^CBC W Auto Differential panel in Blood^LN^4456544^CBC^99USI^^^CBC W Auto Differential panel in Blood|||20110103143428-0800|||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110104170028-0800|||F|||10093^Deluca^Naddy^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI|||||||||||||||||||||CC^Carbon Copy^HL70507
OBX|1|NM|26453-1^Erythrocytes [#/volume] in Blood^LN^^^^^^Erythrocytes [#/volume] in Blood||4.41|10*6/uL^million per microliter^UCUM|4.3 to 6.2|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|2|NM|718-7^Hemoglobin [Mass/volume] in Blood^LN^^^^^^Hemoglobin [Mass/volume] in Blood||12.5|g/mL^grams per milliliter^UCUM|13 to 18|L|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|3|NM|20570-8^Hematocrit [Volume Fraction] of Blood^LN^^^^^^Hematocrit [Volume Fraction] of Blood||41|%^percent^UCUM|40 to 52|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|4|NM|26464-8^Leukocytes [#/volume] in Blood^LN^^^^^^Leukocytes [#/volume] in Blood||105600|{cells}/uL^cells per microliter^UCUM|4300 to 10800|HH|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|5|NM|26515-7^Platelets [#/volume] in Blood^LN^^^^^^Platelets [#/volume] in Blood||210000|{cells}/uL^cells per microliter^UCUM|150000 to 350000|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|6|NM|30428-7^Erythrocyte mean corpuscular volume [Entitic volume]^LN^^^^^^Erythrocyte mean corpuscular volume [Entitic volume]||91|fL^femtoliter^UCUM|80 to 95|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|7|NM|28539-5^Erythrocyte mean corpuscular hemoglobin [Entitic mass]^LN^^^^^^Erythrocyte mean corpuscular hemoglobin [Entitic mass]||29|pg/{cell}^picograms per cell^UCUM|27 to 31|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|8|NM|28540-3^Erythrocyte mean corpuscular hemoglobin concentration [Mass/volume]^LN^^^^^^Erythrocyte mean corpuscular hemoglobin concentration [Mass/volume]||32.4|g/dL^grams per deciliter^UCUM|32 to 36|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|9|NM|30385-9^Erythrocyte distribution width [Ratio]^LN^^^^^^Erythrocyte distribution width [Ratio]||10.5|%^percent^UCUM|10.2 to 14.5|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|10|NM|26444-0^Basophils [#/volume] in Blood^LN^^^^^^Basophils [#/volume] in Blood||0.1|10*3/uL^thousand per microliter^UCUM|0 to 0.3|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|11|NM|30180-4^Basophils/100 leukocytes in Blood^LN^^^^^^Basophils/100 leukocytes in Blood||0.1|%^percent^UCUM|0 to 2|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|12|NM|26484-6^Monocytes [#/volume] in Blood^LN^^^^^^Monocytes [#/volume] in Blood||3|10*3/uL^thousand per microliter^UCUM|0.0 to 13.0|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|13|NM|26485-3^Monocytes/100 leukocytes in Blood^LN^^^^^^Monocytes/100 leukocytes in Blood||3|%^percent^UCUM|0 to 10|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|14|NM|26449-9^Eosinophils [#/volume] in Blood^LN^^^^^^Eosinophils [#/volume] in Blood||2.1|10*3/uL^thousand per microliter^UCUM|0.0 to 0.45|HH|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|15|NM|26450-7^Eosinophils/100 leukocytes in Blood^LN^^^^^^Eosinophils/100 leukocytes in Blood||2|%^percent^UCUM|0 to 6|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|16|NM|26474-7^Lymphocytes [#/volume] in Blood^LN^^^^^^Lymphocytes [#/volume] in Blood||41.2|10*3/uL^thousand per microliter^UCUM|1.0 to 4.8|HH|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|17|NM|26478-8^Lymphocytes/100 leukocytes in Blood^LN^^^^^^Lymphocytes/100 leukocytes in Blood||39|%^percent^UCUM|15.0 to 45.0|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|18|NM|26499-4^Neutrophils [#/volume] in Blood^LN^^^^^^Neutrophils [#/volume] in Blood||58|10*3/uL^thousand per microliter^UCUM|1.5 to 7.0|HH|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|19|NM|26511-6^Neutrophils/100 leukocytes in Blood^LN^^^^^^Neutrophils/100 leukocytes in Blood||55|%^percent^UCUM|50 to 73|N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|20|CWE|38892-6^Anisocytosis [Presence] in Blood^LN^^^^^^Anisocytosis [Presence] in Blood||260348001^Present ++ out of ++++^SCT^^^^^^Moderate Anisocytosis|||A|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|21|CWE|30400-6^Hypochromia [Presence] in Blood^LN^^^^^^Hypochromia [Presence] in Blood||260415000^not detected^SCT^^^^^^None seen|||N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|22|CWE|30424-6^Macrocytes [Presence] in Blood^LN^^^^^^Macrocytes [Presence] in Blood||260415000^not detected^SCT^^^^^^None seen|||N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|23|CWE|30434-5^Microcytes [Presence] in Blood^LN^^^^^^Microcytes [Presence] in Blood||260415000^not detected^SCT^^^^^^None seen|||N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|24|CWE|779-9^Poikilocytosis [Presence] in Blood by Light microscopy^LN^^^^^^Poikilocytosis [Presence] in Blood by Light microscopy||260415000^not detected^SCT^^^^^^None seen|||N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|25|CWE|10378-8^Polychromasia [Presence] in Blood by Light microscopy^LN^^^^^^Polychromasia [Presence] in Blood by Light microscopy||260415000^not detected^SCT^^^^^^None seen|||N|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|26|TX|6742-1^Erythrocyte morphology finding [Identifier] in Blood^LN^^^^^^Erythrocyte morphology finding [Identifier] in Blood||Many spherocytes present.|||A|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|27|TX|11156-7^Leukocyte morphology finding [Identifier] in Blood^LN^^^^^^Leukocyte morphology finding [Identifier] in Blood||Reactive morphology in lymphoid cells.|||A|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|28|TX|11125-2^Platelet morphology finding [Identifier] in Blood^LN^^^^^^Platelet morphology finding [Identifier] in Blood||Platelets show defective granulation.|||A|||F|||20110103143428-0800|||||20110103163428-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
SPM|1|||119297000^BLD^SCT^^^^^^Blood|||||||||||||20110103143428-0800
");
			#endregion
			#region LRI_3.0-GU_Final
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-003.00|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A||19610615|M||2106-3^White^HL70005
ORC|RE|ORD777888^^2.16.840.1.113883.3.72.5.24^ISO|R-220713^^2.16.840.1.113883.3.72.5.25^ISO|GORD874244^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD777888^^2.16.840.1.113883.3.72.5.24^ISO|R-220713^^2.16.840.1.113883.3.72.5.25^ISO|24331-1^Lipid 1996 panel in Serum or Plasma^LN^345789^Lipid Panel^99USI^^^Lipid 1996 panel in Serum or Plasma|||20110531123551-0800||||||56388000^hyperlipidemia^99USI^3744001^hyperlipoproteinemia^SCT^^^hyperlipoproteinemia|||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110611140428-0800|||F|||10092^Hamlin^Pafford^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI|||||||||||||||||||||BCC^Blind Copy^HL70507
OBX|1|NM|2093-3^Cholesterol [Mass/volume] in Serum or Plasma^LN^^^^^^Cholesterol [Mass/volume] in Serum or Plasma||196|mg/dL^milligrams per deciliter^UCUM|Recommended: <200; Moderate Risk: 200-239 ; High Risk: >240|N|||F|||20110531123551-0800|||||20110601130551-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|2|NM|2571-8^Triglyceride [Mass/volume] in Serum or Plasma^LN^^^^^^Triglyceride [Mass/volume] in Serum or Plasma||100|mg/dL^milligrams per deciliter^UCUM|40 to 160|N|||F|||20110531123551-0800|||||20110601130551-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|3|NM|2085-9^Cholesterol in HDL [Mass/volume] in Serum or Plasma^LN^^^^^^Cholesterol in HDL [Mass/volume] in Serum or Plasma||60|mg/dL^milligrams per deciliter^UCUM|29 to 72|N|||F|||20110531123551-0800|||||20110601130551-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
OBX|4|NM|2089-1^Cholesterol in LDL [Mass/volume] in Serum or Plasma^LN^^^^^^Cholesterol in LDL [Mass/volume] in Serum or Plasma||116|mg/dL^milligrams per deciliter^UCUM|Recommended: <130; Moderate Risk: 130-159; High Risk: >160|N|||F|||20110531123551-0800|||||20110601130551-0800||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|2343242^Knowsalot^Phil^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^DN
SPM|1|||119297000^BLD^SCT^^^^^^Blood|||||||||||||20110531123551-0800
");
			#endregion
			#region LRI_4.0-GU_Parent
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-004.00|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A||19610615|M||2106-3^White^HL70005
ORC|RE|ORD723222-4^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-4^^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD723222-4^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-4^^2.16.840.1.113883.3.72.5.25^ISO|625-4^Bacteria identified in Stool by Culture^LN^3456543^CULTURE STOOL^99USI^^^Stool Culture|||20110530123551-0500||||||787.91^DIARRHEA^I9CDX^^^^^^DIARRHEA|||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110531140428-0500|||P
OBX|1|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|1|103429008^Enterohemorrhagic Escherichia coli, serotype O157:H7^SCT^^^^^^Shiga toxin producing E. coli O157:H7 isolated|||A|||P|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|2|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|2|398567006^Salmonella I, group O:4^SCT^^^^^^Salmonella I, group O:4 isolated|||A|||P|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|3|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|3|85729005^Shigella flexneri^SCT^^^^^^Shigella flexneri isolated|||A|||P|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
SPM|1|||119339001^Stool specimen^SCT^^^^^^Stool|||||||||||||20110530123551-0500
");
			#endregion
			#region LRI_4.1-GU_Parent_Child
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-RU-004.01|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1234^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Jones^William^A||19610615|M||2106-3^White^HL70005
ORC|RE|ORD723222-4^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-4^^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD723222-4^^2.16.840.1.113883.3.72.5.24^ISO|R-783274-4^^2.16.840.1.113883.3.72.5.25^ISO|625-4^Bacteria identified in Stool by Culture^LN^3456543^CULTURE STOOL^99USI^^^Stool Culture|||20110530123551-0500||||||787.91^DIARRHEA^I9CDX^^^^^^DIARRHEA|||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110531140428-0500|||F
OBX|1|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|1|103429008^Enterohemorrhagic Escherichia coli, serotype O157:H7^SCT^^^^^^Shiga toxin producing E. coli O157:H7 isolated|||A|||F|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|2|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|2|398567006^Salmonella I, group O:4^SCT^^^^^^Salmonella I, group O:4 isolated|||A|||F|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|3|CWE|625-4^Bacteria identified in Stool by Culture^LN^^^^^^Stool Culture|3|85729005^Shigella flexneri^SCT^^^^^^Shigella flexneri isolated|||A|||F|||20110530123551-0500|||||20110531130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
SPM|1|||119339001^Stool specimen^SCT^^^^^^Stool|||||||||||||20110530123551-0500
ORC|RE||R-783274-5^^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|2||R-783274-5^^2.16.840.1.113883.3.72.5.25^ISO|50545-3^Bacterial susceptibility panel in Isolate by Minimum inhibitory concentration (MIC)^LN^^^^^^Bacteria susceptibility|||20110530123551-0500||||G|||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110601140428-0500|||F|625-4&Bacteria identified in Stool by Culture&LN&&&&&&Stool Culture^1|||ORD723222-4&&2.16.840.1.113883.3.72.5.24&ISO^R-783274-4&&2.16.840.1.113883.3.72.5.25&ISO
OBX|1|SN|28-1^Ampicillin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^AMPICILLIN|1|<^0.06|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|2|SN|267-5^Gentamicin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^GENTAMICIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|3|SN|185-9^Ciprofloxacin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^CIPROFLOXACIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
ORC|RE||R-783274-6^^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|3||R-783274-6^^2.16.840.1.113883.3.72.5.25^ISO|50545-3^Bacterial susceptibility panel in Isolate by Minimum inhibitory concentration (MIC)^LN^^^^^^Bacteria susceptibility|||20110530123551-0500||||G|||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110601140428-0500|||F|625-4&Bacteria identified in Stool by Culture&LN&&&&&&Stool Culture^2|||ORD723222-4&&2.16.840.1.113883.3.72.5.24&ISO^R-783274-4&&2.16.840.1.113883.3.72.5.25&ISO
OBX|1|SN|28-1^Ampicillin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^AMPICILLIN|1|<^0.06|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|2|SN|267-5^Gentamicin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^GENTAMICIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|3|SN|185-9^Ciprofloxacin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^CIPROFLOXACIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
ORC|RE||R-783274-7^^2.16.840.1.113883.3.72.5.25^ISO|GORD874211^^2.16.840.1.113883.3.72.5.24^ISO||||||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|4||R-783274-7^^2.16.840.1.113883.3.72.5.25^ISO|50545-3^Bacterial susceptibility panel in Isolate by Minimum inhibitory concentration (MIC)^LN^^^^^^Bacteria susceptibility|||20110530123551-0500||||G|||||57422^Radon^Nicholas^^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20110601140428-0500|||F|625-4&Bacteria identified in Stool by Culture&LN&&&&&&Stool Culture^3|||ORD723222-4&&2.16.840.1.113883.3.72.5.24&ISO^R-783274-4&&2.16.840.1.113883.3.72.5.25&ISO
OBX|1|SN|28-1^Ampicillin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^AMPICILLIN|1|<^0.06|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|2|SN|267-5^Gentamicin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^GENTAMICIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBX|3|SN|185-9^Ciprofloxacin [Susceptibility] by Minimum inhibitory concentration (MIC)^LN^^^^^^CIPROFLOXACIN|1|^0.05|ug/mL^^UCUM||S|||F|||20110530123551-0500|||||20110601130655-0500||||Century Hospital^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^987|2070 Test Park^^Los Angeles^CA^90067^^B|9876543^Slide^Stan^S^^^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
");
			#endregion
			#region LRI_5.0-GU-RU_Parent_Child
			retVal.Add(@"MSH|^~\&|^2.16.840.1.113883.3.72.5.20^ISO|^2.16.840.1.113883.3.72.5.21^ISO||^2.16.840.1.113883.3.72.5.23^ISO|20110531140551-0500||ORU^R01^ORU_R01|NIST-LRI-GU-RU-005.00|T|2.5.1|||AL|NE|||||LRI_Common_Component^^2.16.840.1.113883.9.16^ISO~LRI_GU_Component^^2.16.840.1.113883.9.12^ISO~LRI_RU_Component^^2.16.840.1.113883.9.14^ISO
PID|1||PATID1239^^^&2.16.840.1.113883.3.72.5.30.2&ISO^MR||Smirnoff^Peggy^^^^^M||19750401|F||2106-3^White^HL70005^wh^white^L
ORC|RE|ORD448811^^2.16.840.1.113883.3.72.5.24^ISO|R-511^^2.16.840.1.113883.3.72.5.25^ISO|||||||||1234567890^Fine^Larry^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|1|ORD448811^^2.16.840.1.113883.3.72.5.24^ISO|R-511^^2.16.840.1.113883.3.72.5.25^ISO|HepABC Panel^Hepatitis A B C Panel^L|||20120628070100|||||||||1234567890^Fine^Larry^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20120629132900-0500|||F
OBX|1|CWE|22314-9^Hepatitis A virus IgM Ab [Presence] in Serum^LN^HAVM^Hepatitis A IgM antibodies (IgM anti-HAV)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|2|CWE|20575-7^Hepatitis A virus Ab [Presence] in Serum^LN^HAVAB^Hepatitis A antibodies (anti-HAV)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|3|CWE|16933-4^Hepatitis B virus core Ab [Presence] in Serum^LN^HBVcAB^Hepatitis B core antibodies (anti-HBVc)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|4|SN|22316-4^Hepatitis B virus core Ab [Units/volume] in Serum^LN^HBcAbQ^Hepatitis B core antibodies (anti-HBVc) Quant^L||^0.40|[IU]/mL^international unit per milliliter^UCUM^IU/ml^^L|<0.50 IU/mL|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|5|CWE|22320-6^Hepatitis B virus e Ab [Presence] in Serum^LN^HBVeAB^Hepatitis B e antibodies (anti-HBVe)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|6|CWE|5195-3^Hepatitis B virus surface Ag [Presence] in Serum^LN^HBVsAG^Hepatitis B surface antigen (HBsAg)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|7|CWE|22322-2^Hepatitis B virus surface Ab [Presence] in Serum^LN^HBVSAB^Hepatitis B surface antibody (anti-HBVs)^L||260385009^Negative (qualifier value)^SCT^NEG^NEGATIVE^L^^^Negative (qualifier value)||Negative|N|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|8|CWE|16128-1^Hepatitis C virus Ab [Presence] in Serum^LN^HCVAB^Hepatitis C antibody screen  (anti-HCV)^L||10828004^Positive (qualifier value)^SCT^POS^POSITIVE^L^^^Positive (qualifier value)||Negative|A|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
OBX|9|SN|48159-8^Hepatitis C virus Ab Signal/Cutoff in Serum or Plasma by Immunoassay^LN^HCVSCO^Hepatitis C antibodies Signal to Cut-off Ratio^L||^10.8|{s_co_ratio}^Signal to cutoff ratio^UCUM^s/co^^L|0.0-0.9 s/co|H|||F|||20120628070100|||||20120628100500||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.
NTE|1||Negative:   < 0.8; Indeterminate 0.8 - 0.9; Positive:  > 0.9.  In order to reduce the incidence of a false positive result, the CDC recommends that all s/co ratios between 1.0 and 10.9 be confirmed with additional Verification or PCR testing.
SPM|1|||119364003^Serum specimen (specimen)^SCT^SER^Serum^L|||||||||||||20120628070100
ORC|RE||R-512^^2.16.840.1.113883.3.72.5.25^ISO|||||||||1234567890^Fine^Larry^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI
OBR|2||R-512^^2.16.840.1.113883.3.72.5.25^ISO|11011-4^Hepatitis C virus RNA [Units/volume] (viral load) in Serum or Plasma by Probe and target amplification method^LN^HCVRNA^Hepatitis C RNA PCR^L|||20120628070100||||G|||||1234567890^Fine^Larry^^^Dr.^^^&2.16.840.1.113883.3.72.5.30.1&ISO^L^^^NPI||||||20120629132900-0500|||F|16128-1&Hepatitis C virus Ab [Presence] in Serum&LN&HCVAB&Hepatitis C antibody screen  (anti-HCV)&L|||ORD448811&&2.16.840.1.113883.3.72.5.24&ISO^R-511&&2.16.840.1.113883.3.72.5.25&ISO
OBX|1|SN|11011-4^Hepatitis C virus RNA [Units/volume] (viral load) in Serum or Plasma by Probe and target amplification method^LN^HCVRNA^Hepatitis C RNA PCR^L||^7611200|[IU]/mL^international unit per milliliter^UCUM^IU/ml^^L|<43 IU/mL|H|||F|||20120628070100|||||20120629092700||||Princeton Hospital Laboratory^^^^^NIST HCAA-1&2.16.840.1.113883.3.72.5.30.1&ISO^XX^^^34D4567890|123 High Street^^Princeton^NJ^08540^USA^O^^34021|^Martin^Steven^M^^Dr.");
#endregion
			return retVal;
		}
		

	}
}