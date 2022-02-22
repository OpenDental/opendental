using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	/// <summary>PT Dental.  www.gopaperlessnow.com.  This bridge only works on Windows and in English, so some shortcuts were taken.</summary>
	public class PaperlessTechnology{
		private static string exportAddCsv="addpatient_OD.csv";
		private static string exportUpdateCsv="updatepatient_OD.csv";
		private static string importCsv="patientinfo_PT.csv";
		private static string importMedCsv="patientmedalerts.csv";
		private static string exportAddExe="addpatient_OD.exe";
		private static string exportUpdateExe="updatepatient_OD.exe";
		private static string dir=@"C:\PT\USI";
		private static FileSystemWatcher watcher;
		private static string[] fieldNames;
		private static string[] fieldVals;

		/// <summary></summary>
		public PaperlessTechnology(){
			
		}

		///<Summary>There might be incoming files that we have to watch for.  They will get processed and deleted.  There is no user interface for this function.  This method is called when OD first starts up.</Summary>
		public static void InitializeFileWatcher(){
			if(ODBuild.IsWeb()) {
				return;//bridge is not enabled for WEB mode.
			}
			if(!Directory.Exists(dir)){
				if(watcher!=null){
					watcher.Dispose();
				}
				return;
			}
			watcher = new FileSystemWatcher();
			watcher.Path =dir;
			//watcher.NotifyFilter = NotifyFilters.CreationTime;
			string importFileName=importCsv;
			if(File.Exists(dir+"\\"+importCsv)) {
				File.Delete(dir+"\\"+importCsv);
			}
			watcher.Filter=importFileName;
			watcher.Created+=new FileSystemEventHandler(OnCreated);
			watcher.EnableRaisingEvents = true;			
		}

		private static void OnCreated(object source,FileSystemEventArgs e) {
			//MessageBox.Show("File created.  It will now be deleted.");
			Thread.Sleep(200);//just to make sure the other process is done writing.
			string[] lines=File.ReadAllLines(e.FullPath);
			File.Delete(e.FullPath);
			if(lines.Length!=1){
				MessageBox.Show(e.FullPath+" was supposed to have exactly one line.  Invalid file.");
				return;
			}
			string rawFieldNames="PAT_PK,PAT_LOGFK,PAT_LANFK,PAT_TITLE,PAT_FNAME,PAT_MI,PAT_LNAME,PAT_CALLED,PAT_ADDR1,PAT_ADDR2,PAT_CITY,PAT_ST,PAT_ZIP,PAT_HPHN,PAT_WPHN,PAT_EXT,PAT_FAX,PAT_PAGER,PAT_CELL,PAT_EMAIL,PAT_SEX,PAT_EDOCS,PAT_STATUS,PAT_TYPE,PAT_BIRTH,PAT_SSN,PAT_NOCALL,PAT_NOCORR,PAT_DISRES,PAT_LSTUPD,PAT_INSNM,PAT_INSGPL,PAT_INSAD1,PAT_INSAD2,PAT_INSCIT,PAT_INSST,PAT_INSZIP,PAT_INSPHN,PAT_INSEXT,PAT_INSCON,PAT_INSGNO,PAT_EMPNM,PAT_EMPAD1,PAT_EMPAD2,PAT_EMPCIT,PAT_EMPST,PAT_EMPZIP,PAT_EMPPHN,PAT_REFLNM,PAT_REFFNM,PAT_REFMI,PAT_REFPHN,PAT_REFEML,PAT_REFSPE,PAT_NOTES,PAT_NOTE1,PAT_NOTE2,PAT_NOTE3,PAT_NOTE4,PAT_NOTE5,PAT_NOTE6,PAT_NOTE7,PAT_NOTE8,PAT_NOTE9,PAT_NOTE10,PAT_FPSCAN,PAT_PREMED,PAT_MEDS,PAT_FTSTUD,PAT_PTSTUD,PAT_COLLEG,PAT_CHRTNO,PAT_OTHID,PAT_RESPRT,PAT_POLHLD,PAT_CUSCD,PAT_PMPID";
			fieldNames=rawFieldNames.Split(',');
			fieldVals=lines[0].Split(',');
			if(fieldNames.Length!=fieldVals.Length){
				MessageBox.Show(e.FullPath+" contains "+fieldNames.Length.ToString()+" field names, but "+fieldVals.Length.ToString()+" field values.  Invalid file.");
				return;
			}
			for(int i=0;i<fieldVals.Length;i++) {
				fieldVals[i]=fieldVals[i].Replace("\"","");//remove quotes
			}
			long patNum=PIn.Long(GetVal("PAT_OTHID"));
			if(patNum==0){
				MessageBox.Show(patNum.ToString()+" is not a recognized PatNum.");
				return;
			}
			Family fam=Patients.GetFamily(patNum);
			if(fam==null){
				MessageBox.Show("Could not find patient based on PatNum "+patNum.ToString());
				return;
			}
			Patient pat=fam.GetPatient(patNum);
			Patient patOld=pat.Copy();
			string txt;
			string note="PT Dental import processed.  Some information is shown below which was too complex to import automatically.\r\n";
			txt=GetVal("PAT_FNAME");
			if(txt!=""){
				pat.FName=txt;
			}
			txt=GetVal("PAT_MI");
			if(txt!=""){
				pat.MiddleI=txt;
			}
			txt=GetVal("PAT_LNAME");
			if(txt!=""){
				pat.LName=txt;
			}
			txt=GetVal("PAT_CALLED");
			if(txt!=""){
				pat.Preferred=txt;
			}
			txt=GetVal("PAT_ADDR1");
			if(txt!=""){
				pat.Address=txt;
			}
			txt=GetVal("PAT_ADDR2");
			if(txt!=""){
				pat.Address2=txt;
			}
			txt=GetVal("PAT_CITY");
			if(txt!=""){
				pat.City=txt;
			}
			txt=GetVal("PAT_ST");
			if(txt!=""){
				pat.State=txt;
			}
			txt=GetVal("PAT_ZIP");
			if(txt!=""){
				pat.Zip=txt;
			}
			txt=GetVal("PAT_HPHN");//No punct
			if(txt!=""){
				pat.HmPhone=TelephoneNumbers.ReFormat(txt);
			}
			txt=GetVal("PAT_WPHN");
			if(txt!=""){
				pat.WkPhone=TelephoneNumbers.ReFormat(txt);
			}
			//no matching fields for these three:
			txt=GetVal("PAT_EXT");
			if(txt!="") {
				note+="Ph extension: "+txt+"\r\n";
			}
			txt=GetVal("PAT_FAX");
			if(txt!="") {
				note+="Fax: "+txt+"\r\n";
			}
			txt=GetVal("PAT_PAGER");
			if(txt!="") {
				note+="Pager: "+txt+"\r\n";
			}
			txt=GetVal("PAT_CELL");
			if(txt!=""){
				pat.WirelessPhone=TelephoneNumbers.ReFormat(txt);
			}
			txt=GetVal("PAT_EMAIL");
			if(txt!=""){
				pat.Email=txt;
			}
			txt=GetVal("PAT_SEX");//M or F
			if(txt=="M"){
				pat.Gender=PatientGender.Male;
			}
			if(txt=="F"){
				pat.Gender=PatientGender.Male;
			}
			txt=GetVal("PAT_STATUS");//our patStatus, Any text allowed
			switch(txt){
				case "Archived": pat.PatStatus=PatientStatus.Archived; break;
				case "Deceased": pat.PatStatus=PatientStatus.Deceased; break;
				//case "Archived": pat.PatStatus=PatientStatus.Deleted; break;
				case "Inactive": pat.PatStatus=PatientStatus.Inactive; break;
				case "NonPatient": pat.PatStatus=PatientStatus.NonPatient; break;
				case "Patient": pat.PatStatus=PatientStatus.Patient; break;
			}
			txt=GetVal("PAT_TYPE");//our Position, Any text allowed
			switch(txt){
				case "Child": pat.Position=PatientPosition.Child; break;
				case "Divorced": pat.Position=PatientPosition.Divorced; break;
				case "Married": pat.Position=PatientPosition.Married; break;
				case "Single": pat.Position=PatientPosition.Single; break;
				case "Widowed": pat.Position=PatientPosition.Widowed; break;
			}
			txt=GetVal("PAT_BIRTH");// yyyyMMdd
			if(txt!=""){
				pat.Birthdate=PIn.Date(txt);
			}
			txt=GetVal("PAT_SSN");// No punct
			if(txt!=""){
				pat.SSN=txt;
			}
			txt=GetVal("PAT_NOCALL");// T if no call
			if(txt!=""){
				note+="No Call Patient: "+txt+"\r\n";
			}
			txt=GetVal("PAT_NOCORR");// T/F
			if(txt!="") {
				note+="No Correspondence: "+txt+"\r\n";
			}
			txt=GetVal("PAT_NOTES");// No limits.
			if(txt!=""){
				note+=txt+"\r\n";
			}
			txt=GetVal("PAT_PREMED");// F or T
			//I don't like giving the patient control of this field, but I guess the office has the option of not showing this on forms.
			if(txt=="T") {
				pat.Premed=true;
			}
			if(txt=="F") {
				pat.Premed=false;
			}
			txt=GetVal("PAT_MEDS");// The meds that they must premedicate with.
			if(txt!="") {
				note+="Patient Meds: "+txt+"\r\n";
			}
			string ft=GetVal("PAT_FTSTUD");// T/F
			string pt=GetVal("PAT_PTSTUD");//parttime
			if(ft=="T"){
				pat.StudentStatus="F";//fulltime
			}
			else if(pt=="T"){
				pat.StudentStatus="P";//parttime
			}
			else if(ft=="F" && pt=="F"){
				pat.StudentStatus="";//nonstudent
			}
			else if(ft=="F" && pat.StudentStatus=="F"){
				pat.StudentStatus="";
			}
			else if(pt=="F" && pat.StudentStatus=="P"){
				pat.StudentStatus="";
			}
			txt=GetVal("PAT_COLLEG");
			if(txt!="") {
				pat.SchoolName=txt;
			}
			txt=GetVal("PAT_CHRTNO");
			//I don't think patient should have control of this field.
			if(txt!="") {
				pat.ChartNumber=txt;
			}
			txt=GetVal("PAT_RESPRT");// Responsible party checkbox T/F
			if(txt=="T" && pat.PatNum!=pat.Guarantor) {
				note+="Responsible party: True\r\n";
			}
			if(txt=="F" && pat.PatNum==pat.Guarantor) {
				note+="Responsible party: False\r\n";
			}
			txt=GetVal("PAT_POLHLD");// Policy holder checkbox T/F
			if(txt=="T") {
				note+="Policy holder: True\r\n";
			}
			if(txt=="F") {
				note+="Policy holder: False\r\n";
			}
			txt=GetVal("PAT_INSNM");
			if(txt!="") {
				note+="Insurance name: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSGPL");
			if(txt!="") {
				note+="Ins group plan name: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSAD1");
			if(txt!="") {
				note+="Ins address: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSAD2");
			if(txt!="") {
				note+="Ins address2: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSCIT");
			if(txt!="") {
				note+="Ins city: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSST");
			if(txt!="") {
				note+="Ins state: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSZIP");
			if(txt!="") {
				note+="Ins zip: "+txt+"\r\n";
			}
			txt=GetVal("PAT_INSPHN");
			if(txt!="") {
				note+="Ins phone: "+TelephoneNumbers.ReFormat(txt)+"\r\n";
			}
			txt=GetVal("PAT_INSGNO");// Ins group number
			if(txt!="") {
				note+="Ins group number: "+txt+"\r\n";
			}
			txt=GetVal("PAT_EMPNM");
			if(txt!="") {
				note+="Employer name: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFLNM");
			if(txt!="") {
				note+="Referral last name: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFFNM");
			if(txt!="") {
				note+="Referral first name: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFMI");
			if(txt!="") {
				note+="Referral middle init: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFPHN");
			if(txt!="") {
				note+="Referral phone: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFEML");// Referral source email
			if(txt!="") {
				note+="Referral email: "+txt+"\r\n";
			}
			txt=GetVal("PAT_REFSPE");// Referral specialty. Customizable, so any allowed
			if(txt!="") {
				note+="Referral specialty: "+txt+"\r\n";
			}
			Patients.Update(pat,patOld);
			if(File.Exists(dir+"\\"+importMedCsv)){
				lines=File.ReadAllLines(dir+"\\"+importMedCsv);
				File.Delete(dir+"\\"+importMedCsv);
				if(lines.Length<1) {
					MessageBox.Show(e.FullPath+" was supposed to have at least one line.  Invalid file.");
					return;
				}
				fieldNames=lines[0].Split(',');
				long diseaseDefNum;
				Disease disease;
				string diseaseNote;
				for(int i=1;i<lines.Length;i++){
					fieldVals=lines[i].Split(',');
					txt=GetVal("PMA_MALDES");
					diseaseNote=GetVal("PMA_NOTES");
					if(txt==""){
						continue;
					}
					diseaseDefNum=DiseaseDefs.GetNumFromName(txt);
					if(diseaseDefNum==0){
						note+="Disease: "+txt+", "+diseaseNote+"\r\n";
					}
					disease=Diseases.GetSpecificDiseaseForPatient(patNum,diseaseDefNum);
					if(disease==null){
						disease=new Disease();
						disease.DiseaseDefNum=diseaseDefNum;
						disease.PatNum=patNum;
						disease.PatNote=diseaseNote;
						Diseases.Insert(disease);
					}
					else{
						if(txt!=""){
							if(disease.PatNote!=""){
								disease.PatNote+="  ";
							}
							disease.PatNote+=diseaseNote;
							Diseases.Update(disease);
						}
					}
				}
			}
			Commlog comm=new Commlog();
			comm.PatNum=patNum;
			comm.SentOrReceived=CommSentOrReceived.Received;
			comm.CommDateTime=DateTime.Now;
			comm.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			comm.Mode_=CommItemMode.None;
			comm.Note=note;
			comm.UserNum=Security.CurUser.UserNum;
			Commlogs.Insert(comm);
			MessageBox.Show("PT Dental import complete.");
		}

		private static string GetVal(string fieldname){
			for(int i=0;i<fieldNames.Length;i++){
				if(fieldNames[i]==fieldname){
					if(fieldVals.Length!=fieldNames.Length){
						return "";
					}
					return fieldVals[i];
				}
			}
			return "";
		}

		///<summary>Sends data for Patient.Cur to an export file and then launches an exe to notify PT.  If patient exists, this simply opens the patient.  If patient does not exist, then this triggers creation of the patient in PT Dental.  If isUpdate is true, then the export file and exe will have different names. In PT, update is a separate programlink with a separate button.</summary>
		public static void SendData(Program ProgramCur, Patient pat,bool isUpdate){
			//ArrayList ForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			//ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "InfoFile path");
			//string infoFile=PPCur.PropertyValue;
			if(!Directory.Exists(dir)){
				MessageBox.Show(dir+" does not exist.  PT Dental doesn't seem to be properly installed on this computer.");
				return;
			}
			if(pat==null){
				MessageBox.Show("No patient is selected.");
				return;
			}
			if(!File.Exists(dir+"\\"+exportAddExe)) {
				MessageBox.Show(dir+"\\"+exportAddExe+" does not exist.  PT Dental doesn't seem to be properly installed on this computer.");
				return;
			}
			if(!File.Exists(dir+"\\"+exportUpdateExe)) {
				MessageBox.Show(dir+"\\"+exportUpdateExe+" does not exist.  PT Dental doesn't seem to be properly installed on this computer.");
				return;
			}
			string filename=dir+"\\"+exportAddCsv;
			if(isUpdate){
				filename=dir+"\\"+exportUpdateCsv;
			}
			using(StreamWriter sw=new StreamWriter(filename,false)){//overwrites if it already exists.
				sw.WriteLine("PAT_PK,PAT_LOGFK,PAT_LANFK,PAT_TITLE,PAT_FNAME,PAT_MI,PAT_LNAME,PAT_CALLED,PAT_ADDR1,PAT_ADDR2,PAT_CITY,PAT_ST,PAT_ZIP,PAT_HPHN,PAT_WPHN,PAT_EXT,PAT_FAX,PAT_PAGER,PAT_CELL,PAT_EMAIL,PAT_SEX,PAT_EDOCS,PAT_STATUS,PAT_TYPE,PAT_BIRTH,PAT_SSN,PAT_NOCALL,PAT_NOCORR,PAT_DISRES,PAT_LSTUPD,PAT_INSNM,PAT_INSGPL,PAT_INSAD1,PAT_INSAD2,PAT_INSCIT,PAT_INSST,PAT_INSZIP,PAT_INSPHN,PAT_INSEXT,PAT_INSCON,PAT_INSGNO,PAT_EMPNM,PAT_EMPAD1,PAT_EMPAD2,PAT_EMPCIT,PAT_EMPST,PAT_EMPZIP,PAT_EMPPHN,PAT_REFLNM,PAT_REFFNM,PAT_REFMI,PAT_REFPHN,PAT_REFEML,PAT_REFSPE,PAT_NOTES,PAT_FPSCAN,PAT_PREMED,PAT_MEDS,PAT_FTSTUD,PAT_PTSTUD,PAT_COLLEG,PAT_CHRTNO,PAT_OTHID,PAT_RESPRT,PAT_POLHLD,PAT_CUSCD,PAT_PMPID");
				sw.Write(",");//PAT_PK  Primary key. Long alphanumeric. We do not use.
				sw.Write(",");//PAT_LOGFK Internal PT logical, it can be ignored.
				sw.Write(",");//PAT_LANFK Internal PT logical, it can be ignored.
				sw.Write(",");//PAT_TITLE We do not have this field yet
				sw.Write(Tidy(pat.FName)+",");//PAT_FNAME
				sw.Write(Tidy(pat.MiddleI)+",");//PAT_MI
				sw.Write(Tidy(pat.LName)+",");//PAT_LNAME
				sw.Write(Tidy(pat.Preferred)+",");//PAT_CALLED Nickname
				sw.Write(Tidy(pat.Address)+",");//PAT_ADDR1
				sw.Write(Tidy(pat.Address2)+",");//PAT_ADDR2
				sw.Write(Tidy(pat.City)+",");//PAT_CITY
				sw.Write(Tidy(pat.State)+",");//PAT_ST
				sw.Write(Tidy(pat.Zip)+",");//PAT_ZIP
				sw.Write(TelephoneNumbers.FormatNumbersOnly(pat.HmPhone)+",");//PAT_HPHN No punct
				sw.Write(TelephoneNumbers.FormatNumbersOnly(pat.WkPhone)+",");//PAT_WPHN
				sw.Write(",");//PAT_EXT
				sw.Write(",");//PAT_FAX
				sw.Write(",");//PAT_PAGER
				sw.Write(TelephoneNumbers.FormatNumbersOnly(pat.WirelessPhone)+",");//PAT_CELL
				sw.Write(Tidy(pat.Email)+",");//PAT_EMAIL
				if(pat.Gender==PatientGender.Female){
					sw.Write("Female");
				}
				else if(pat.Gender==PatientGender.Male) {
					sw.Write("Male");
				}
				sw.Write(",");//PAT_SEX might be blank if unknown
				sw.Write(",");//PAT_EDOCS Internal PT logical, it can be ignored.
				sw.Write(pat.PatStatus.ToString()+",");//PAT_STATUS Any text allowed
				sw.Write(pat.Position.ToString()+",");//PAT_TYPE Any text allowed
				if(pat.Birthdate.Year>1880){
					sw.Write(pat.Birthdate.ToString("MM/dd/yyyy"));//PAT_BIRTH MM/dd/yyyy
				}
				sw.Write(",");
				sw.Write(Tidy(pat.SSN)+",");//PAT_SSN No punct
				if(pat.PreferContactMethod==ContactMethod.DoNotCall
					|| pat.PreferConfirmMethod==ContactMethod.DoNotCall
					|| pat.PreferRecallMethod==ContactMethod.DoNotCall)
				{
					sw.Write("1");
				}
				sw.Write(",");//PAT_NOCALL T if no call
				sw.Write(",");//PAT_NOCORR No correspondence HIPAA
				sw.Write(",");//PAT_DISRES Internal PT logical, it can be ignored.
				sw.Write(",");//PAT_LSTUPD Internal PT logical, it can be ignored.
				List <PatPlan> patPlanList=PatPlans.Refresh(pat.PatNum);
				Family fam=Patients.GetFamily(pat.PatNum);
				List<InsSub> subList=InsSubs.RefreshForFam(fam);
				List <InsPlan> planList=InsPlans.RefreshForSubList(subList);
				PatPlan patplan=null;
				InsSub sub=null;
				InsPlan plan=null;
				Carrier carrier=null;
				Employer emp=null;
				if(patPlanList.Count>0){
					patplan=patPlanList[0];
					sub=InsSubs.GetSub(patplan.InsSubNum,subList);
					plan=InsPlans.GetPlan(sub.PlanNum,planList);
					carrier=Carriers.GetCarrier(plan.CarrierNum);
					if(plan.EmployerNum!=0){
						emp=Employers.GetEmployer(plan.EmployerNum);
					}
				}
				if(plan==null){
					sw.Write(",");//PAT_INSNM
					sw.Write(",");//PAT_INSGPL Ins group plan name
					sw.Write(",");//PAT_INSAD1
					sw.Write(",");//PAT_INSAD2
					sw.Write(",");//PAT_INSCIT
					sw.Write(",");//PAT_INSST
					sw.Write(",");//PAT_INSZIP
					sw.Write(",");//PAT_INSPHN
				}
				else{
					sw.Write(Tidy(carrier.CarrierName)+",");//PAT_INSNM
					sw.Write(Tidy(plan.GroupName)+",");//PAT_INSGPL Ins group plan name
					sw.Write(Tidy(carrier.Address)+",");//PAT_INSAD1
					sw.Write(Tidy(carrier.Address2)+",");//PAT_INSAD2
					sw.Write(Tidy(carrier.City)+",");//PAT_INSCIT
					sw.Write(Tidy(carrier.State)+",");//PAT_INSST
					sw.Write(Tidy(carrier.Zip)+",");//PAT_INSZIP
					sw.Write(TelephoneNumbers.FormatNumbersOnly(carrier.Phone)+",");//PAT_INSPHN
				}
				sw.Write(",");//PAT_INSEXT
				sw.Write(",");//PAT_INSCON Ins contact person
				if(plan==null){
					sw.Write(",");
				}
				else{
					sw.Write(Tidy(plan.GroupNum)+",");//PAT_INSGNO Ins group number
				}
				if(emp==null){
					sw.Write(",");//PAT_EMPNM
				}
				else{
					sw.Write(Tidy(emp.EmpName)+",");//PAT_EMPNM
				}
				sw.Write(",");//PAT_EMPAD1
				sw.Write(",");//PAT_EMPAD2
				sw.Write(",");//PAT_EMPCIT
				sw.Write(",");//PAT_EMPST
				sw.Write(",");//PAT_EMPZIP
				sw.Write(",");//PAT_EMPPHN
				/*we don't support employer info yet.
				sw.Write(Tidy(emp.Address)+",");//PAT_EMPAD1
				sw.Write(Tidy(emp.Address2)+",");//PAT_EMPAD2
				sw.Write(Tidy(emp.City)+",");//PAT_EMPCIT
				sw.Write(Tidy(emp.State)+",");//PAT_EMPST
				sw.Write(Tidy(emp.State)+",");//PAT_EMPZIP
				sw.Write(TelephoneNumbers.FormatNumbersOnly(emp.Phone)+",");//PAT_EMPPHN*/
				Referral referral=Referrals.GetReferralForPat(pat.PatNum);//could be null
				if(referral==null){
					sw.Write(",");//PAT_REFLNM
					sw.Write(",");//PAT_REFFNM
					sw.Write(",");//PAT_REFMI
					sw.Write(",");//PAT_REFPHN
					sw.Write(",");//PAT_REFEML Referral source email
					sw.Write(",");//PAT_REFSPE Referral specialty. Customizable, so any allowed
				}
				else{
					sw.Write(Tidy(referral.LName)+",");//PAT_REFLNM
					sw.Write(Tidy(referral.FName)+",");//PAT_REFFNM
					sw.Write(Tidy(referral.MName)+",");//PAT_REFMI
					sw.Write(referral.Telephone+",");//PAT_REFPHN
					sw.Write(Tidy(referral.EMail)+",");//PAT_REFEML Referral source email
					if(referral.PatNum==0 && !referral.NotPerson){//not a patient, and is a person
						sw.Write(Defs.GetName(DefCat.ProviderSpecialties,referral.Specialty));
					}
					sw.Write(",");//PAT_REFSPE Referral specialty. Customizable, so any allowed
				}
				sw.Write(",");//PAT_NOTES No limits.  We won't use this right now for exports.
				//sw.Write(",");//PAT_NOTE1-PAT_NOTE10 skipped
				sw.Write(",");//PAT_FPSCAN Internal PT logical, it can be ignored.
				if(pat.Premed){
					sw.Write("1");
				}
				else{
					sw.Write("0");
				}
				sw.Write(",");//PAT_PREMED F or T
				sw.Write(Tidy(pat.MedUrgNote)+",");//PAT_MEDS The meds that they must premedicate with.
				if(pat.StudentStatus=="F"){//fulltime
					sw.Write("1");
				}
				else{
					sw.Write("0");
				}
				sw.Write(",");//PAT_FTSTUD T/F
				if(pat.StudentStatus=="P") {//parttime
					sw.Write("1");
				}
				else {
					sw.Write("0");
				}
				sw.Write(",");//PAT_PTSTUD
				sw.Write(Tidy(pat.SchoolName)+",");//PAT_COLLEG Name of college
				sw.Write(Tidy(pat.ChartNumber)+",");//PAT_CHRTNO
				sw.Write(pat.PatNum.ToString()+",");//PAT_OTHID The primary key in Open Dental ************IMPORTANT***************
				if(pat.PatNum==pat.Guarantor){
					sw.Write("1");
				}
				else {
					sw.Write("0");
				}
				sw.Write(",");//PAT_RESPRT Responsible party checkbox T/F
				if(plan!=null && pat.PatNum==sub.Subscriber) {//if current patient is the subscriber on their primary plan
					sw.Write("1");
				}
				else {
					sw.Write("0");
				}
				sw.Write(",");//PAT_POLHLD Policy holder checkbox T/F
				sw.Write(",");//PAT_CUSCD Web sync folder, used internally this can be ignored.
				sw.Write("");//PAT_PMPID Practice Management Program ID. Can be ignored
				sw.WriteLine();
			}
			try{
				if(isUpdate){
					ODFileUtils.ProcessStart(dir+"\\"+exportUpdateExe);//already validated to exist
				}
				else{
					ODFileUtils.ProcessStart(dir+"\\"+exportAddExe);//already validated to exist
				}
				//MessageBox.Show("done");
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>removes commas.</summary>
		private static string Tidy(string str){
			string retval=str.Replace(",","");
			retval=retval.Replace("\r","");
			retval=retval.Replace("\n","");
			retval=retval.Replace("\t","");
			return retval;
		}

		/*
		///<summary>removes commas, dashes, parentheses, and spaces.  It would be better to use regex to strip all non-numbers.</summary>
		private static string TidyNumber(string str) {
			//Regex.
			str=str.Replace(",","");
			str=str.Replace("-","");
			str=str.Replace("(","");
			str=str.Replace(")","");
			str=str.Replace(" ","");
			return str;
		}*/

	



	}
}










