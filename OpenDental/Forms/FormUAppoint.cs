using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using System.Linq;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormUAppoint:FormODBase {
		/// <summary>This Program link is new.</summary>
		public bool IsNew;
		public Program ProgramCur;
		private List<ProgramProperty> _listProgramProperties;
		private static Thread _thread;
		private static string _logfile="UAppointLog.txt";

		///<summary></summary>
		public FormUAppoint() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUAppoint_Load(object sender, System.EventArgs e) {
			ODThread odThreadUpdateText=new ODThread(300,(o) => {
				string syncStatus=ProgramProperties.GetValFromDb(ProgramCur.ProgramNum,"SynchStatus");
				ODException.SwallowAnyException(() => { this.Invoke(() => { textSynchStatus.Text=syncStatus; }); });
			});
			odThreadUpdateText.Start();
			this.FormClosed+=new FormClosedEventHandler((o,e1) => { odThreadUpdateText.QuitAsync(); });
			FillForm();
		}

		private void FillForm(){
			ProgramProperties.RefreshCache();
			textProgName.Text=ProgramCur.ProgName;
			textProgDesc.Text=ProgramCur.ProgDesc;
			checkEnabled.Checked=ProgramCur.Enabled;
			textPath.Text=ProgramCur.Path;
			_listProgramProperties=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			textUsername.Text=GetProp("Username");
			textPassword.Text=GetProp("Password");
			for(int i=0;i<_listProgramProperties.Count;i++) {
				if(_listProgramProperties[i].IsHighSecurity){
					_listProgramProperties[i].TagOD=_listProgramProperties[i].PropertyValue;
				}
			}
			if(!ProgramProperties.CanEditProperties(_listProgramProperties)) {
				textPassword.ReadOnly=true;
			}
			textWorkstationName.Text=GetProp("WorkstationName");
			textIntervalSeconds.Text=GetProp("IntervalSeconds");
			DateTime dateTime=PIn.DateT(GetProp("DateTimeLastUploaded"));
			if(dateTime.Year>1880){
				textDateTimeLastUploaded1.Text=dateTime.ToShortDateString()+"  "+dateTime.ToShortTimeString();
			}
			//textSynchStatus.Text=GetProp("SynchStatus");//auto
			textNote.Text=ProgramCur.Note;
			textPassword.UseSystemPasswordChar=textPassword.Text.Trim().Length>0;
		}

		private string GetProp(string desc){
			for(int i=0;i<_listProgramProperties.Count;i++){
				if(_listProgramProperties[i].PropertyDesc==desc){
					if(_listProgramProperties[i].IsMasked) {
						return CDT.Class1.TryDecrypt((_listProgramProperties[i].PropertyValue));
					}
					return _listProgramProperties[i].PropertyValue;
				}
			}
			throw new ApplicationException("Property not found: "+desc);
		}

		private void butStart_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return;
			}
			if(!SaveToDb()){
				return;
			}
			string propVal=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"SynchStatus");
			if(ProgramCur.Enabled 
				&& propVal=="Initial synchronization running.")
			{
				MessageBox.Show("Initial synchronization is running.  Not allowed to restart.  You could uncheck the Enabled box and then click this button to stop the sychronization.");
				return;
			}
			propVal=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"DateTimeLastUploaded");
			DateTime dateT=PIn.DateT(propVal);
			if(dateT.Year<1880){
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This is an initial synchronization.  It could take a while.  You can probably continue to work on this computer, but you will need to leave the program running on this workstation until the synch is done.  Begin initial synchronization?"))
				{
					return;
				}
				File.AppendAllText(_logfile,DateTime.Now.ToString()+"  Initial synchronization running.\r\n");
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"SynchStatus","Initial synchronization running.");
			}
			StartThreadIfEnabled();
		}

		private void butViewLog_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return;
			}
			Process.Start(_logfile);
		}

		///<summary>Spawns a thread that handled uploading data to UAppoint in real time.  If the thread is already running, then this restarts it.  If the uploading should no longer happen, then this aborts the thread and exits.</summary>
		public static void StartThreadIfEnabled(){
			if(_thread!=null){
				_thread.Abort();
			}
			Program program=Programs.GetCur(ProgramName.UAppoint);
			if(program==null){
				return;
			}
			if(!Programs.IsEnabledByHq(program,out _) || !program.Enabled || ODBuild.IsWeb()){
				return;
			}
			//get current time and use delta from now on?
			_thread=new Thread(ThreadStartTarget);
			_thread.Start(program);
		}

		public static void AbortThread(){
			if(_thread!=null){
				_thread.Abort();
			}
		}

		private static void ThreadStartTarget(object data){
			File.WriteAllText(_logfile,DateTime.Now.ToString()+"  Synch thread started.\r\n");//creates or clears the log
			Program program=(Program)data;
			int intervalSec=PIn.Int(ProgramProperties.GetPropVal(program.ProgramNum,"IntervalSeconds"));
			int intervalSecError=intervalSec*4;
			string username=ProgramProperties.GetPropVal(program.ProgramNum,"Username");
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(program.ProgramNum,"Password"));
			DateTime dateTimeLastUploaded=PIn.DateT(ProgramProperties.GetPropVal(program.ProgramNum,"DateTimeLastUploaded"));
			//track delta here
			DateTime nowServer=MiscData.GetNowDateTime();
			TimeSpan deltaTimeSpan=nowServer-DateTime.Now;//this was tested to work by adding delta to local time
			DateTime timeStartSynch=DateTime.MinValue;
			string serverName=program.Path;
			HttpWebRequest httpWebRequest;
			string postData;//data just for the current post.
			List<Patient> listPatientsToSync=new List<Patient>();
			List<Provider> listProvidersToSync=new List<Provider>();
			List<Appointment> listAppointmentsToSync=new List<Appointment>();
			List<string> listApptProcsToSync=new List<string>();
			List<DeletedObject> listDeletedObjectsToSync=new List<DeletedObject>();
			List<Schedule> listSchedulesToSync=new List<Schedule>();
			List<Operatory> listOperatoriesToSync=new List<Operatory>();
			List<Recall> listRecallsToSync=new List<Recall>();
			List<ProcedureCode> listProcedureCodesToSync=new List<ProcedureCode>();
			int totalObjectsToSynch=0;
			string synchstatus="";
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.ConformanceLevel=ConformanceLevel.Fragment;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="   ";
			StringBuilder stringBuilder_;
			Version version=new Version(Application.ProductVersion);
			int objectsInThisPost;
			Patient patient;
			Provider provider;
			Appointment appointment;
			DeletedObject deletedObject;
			Schedule schedule;
			Operatory operatory;
			Recall recall;
			ProcedureCode procedureCode;
			int patsInThisPost=0;
			int provsInThisPost=0;
			int apptsInThisPost=0;
			int delObjInThisPost=0;
			int schedsInThisPost=0;
			int opsInThisPost=0;
			int recallsInThisPost=0;
			int codesInThisPost=0;
			string str;
			do{
				#region firstPart
				objectsInThisPost=0;
				totalObjectsToSynch
					=listPatientsToSync.Count
					+listProvidersToSync.Count
					+listAppointmentsToSync.Count
					+listDeletedObjectsToSync.Count
					+listSchedulesToSync.Count
					+listOperatoriesToSync.Count
					+listRecallsToSync.Count
					+listProcedureCodesToSync.Count;
				if(totalObjectsToSynch==0){//if there are no objects ready to upload
					timeStartSynch=DateTime.Now;
					//get various objects from the database.
					listPatientsToSync=Patients.GetChangedSince(dateTimeLastUploaded);//datetime will be handled better soon with delta
					listProvidersToSync=Providers.GetChangedSince(dateTimeLastUploaded);
					listAppointmentsToSync=Appointments.GetChangedSince(dateTimeLastUploaded,DateTime.MinValue);
					listApptProcsToSync=Appointments.GetUAppointProcs(listAppointmentsToSync);
					listDeletedObjectsToSync=DeletedObjects.GetDeletedSince(dateTimeLastUploaded);
					listSchedulesToSync=Schedules.GetChangedSince(dateTimeLastUploaded);
					listOperatoriesToSync=Operatories.GetChangedSince(dateTimeLastUploaded);
					listRecallsToSync=Recalls.GetChangedSince(dateTimeLastUploaded);
					listProcedureCodesToSync=ProcedureCodes.GetChangedSince(dateTimeLastUploaded);
				}
				totalObjectsToSynch
					=listPatientsToSync.Count
					+listProvidersToSync.Count
					+listAppointmentsToSync.Count
					+listDeletedObjectsToSync.Count
					+listSchedulesToSync.Count
					+listOperatoriesToSync.Count
					+listRecallsToSync.Count
					+listProcedureCodesToSync.Count;
				if(totalObjectsToSynch==0){//if there are still no objects
					File.AppendAllText(_logfile,DateTime.Now.ToString()+"  Current.  Sleeping between synch.\r\n");
					ProgramProperties.SetProperty(program.ProgramNum,"SynchStatus","Current.  Sleeping between synch.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSec));//sleep for a while
					continue;
				}
				synchstatus="Synching.  Objects remaining: ";
				if(listPatientsToSync.Count>0){
					synchstatus+=listPatientsToSync.Count.ToString()+" patients, ";
				}
				if(listProvidersToSync.Count>0){
					synchstatus+=listProvidersToSync.Count.ToString()+" providers, ";
				}
				if(listAppointmentsToSync.Count>0){
					synchstatus+=listAppointmentsToSync.Count.ToString()+" appts, ";
				}
				if(listDeletedObjectsToSync.Count>0){
					synchstatus+=listDeletedObjectsToSync.Count.ToString()+" deletions, ";
				}
				if(listSchedulesToSync.Count>0){
					synchstatus+=listSchedulesToSync.Count.ToString()+" schedules, ";
				}
				if(listOperatoriesToSync.Count>0){
					synchstatus+=listOperatoriesToSync.Count.ToString()+" operatories, ";
				}
				if(listRecallsToSync.Count>0){
					synchstatus+=listRecallsToSync.Count.ToString()+" recalls, ";
				}
				if(listProcedureCodesToSync.Count>0){
					synchstatus+=listProcedureCodesToSync.Count.ToString()+" codes, ";
				}
				File.AppendAllText(_logfile,DateTime.Now.ToString()+"  "+synchstatus+"\r\n");
				ProgramProperties.SetProperty(program.ProgramNum,"SynchStatus",synchstatus);
				stringBuilder_=new StringBuilder();
				XmlWriter xmlWriter=XmlWriter.Create(stringBuilder_,xmlWriterSettings);//manual dispose
				xmlWriter.WriteStartElement("PracticeClient");
				xmlWriter.WriteAttributeString("user",username);
				xmlWriter.WriteAttributeString("client-version","OD-"+version.Major.ToString()+"."+version.Minor.ToString()+"."+version.Build.ToString());
				xmlWriter.WriteAttributeString("pass-md5",password);
				#endregion firstPart
				#region patient
				//patient-------------------------------------------------------------------------------------------------
				patsInThisPost=0;
				for(int i=0;i<listPatientsToSync.Count;i++){
					if(objectsInThisPost>=50){//0, some, or all of them might be patients
						break;
					}
					patient=listPatientsToSync[i];
					//patient:
					xmlWriter.WriteStartElement("patient");
					if(patient.PatStatus==PatientStatus.Deleted){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",patient.PatNum.ToString());
					xmlWriter.WriteAttributeString("name-title","");//(Dr., Mrs., etc.) optional 
					xmlWriter.WriteAttributeString("name-first",patient.FName);
					xmlWriter.WriteAttributeString("name-middle",patient.MiddleI);
					xmlWriter.WriteAttributeString("name-last",patient.LName);
					//writer.WriteAttributeString("name-suffix","");//(Jr., TDS, etc)  optional 
					xmlWriter.WriteAttributeString("email",patient.Email);
					if(patient.Birthdate.Year>1880){//notice that there's no way to clear a birthdate
						xmlWriter.WriteAttributeString("birthdate",patient.Birthdate.ToString("yyyy-MM-dd"));
					}
					xmlWriter.WriteAttributeString("provider-id",patient.PriProv.ToString());
					xmlWriter.WriteAttributeString("address-id",patient.PatNum.ToString());
					//writer.WriteAttributeString("status",);
					xmlWriter.WriteEndElement();
					//Address--------------------------------------------------------------------------------------------------
					xmlWriter.WriteStartElement("address");
					if(patient.PatStatus==PatientStatus.Deleted){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",patient.PatNum.ToString());
					xmlWriter.WriteAttributeString("street1",patient.Address);
					xmlWriter.WriteAttributeString("street2",patient.Address2);
					xmlWriter.WriteAttributeString("city",patient.City);
					xmlWriter.WriteAttributeString("state",patient.State);
					xmlWriter.WriteAttributeString("zip",patient.Zip);
					xmlWriter.WriteEndElement();
					//Phone--------------------------------------------------------------------------------------------------
					//primary key is the id + type
					//home
					xmlWriter.WriteStartElement("phone");
					if(patient.PatStatus==PatientStatus.Deleted){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("patient-id",patient.PatNum.ToString());
					xmlWriter.WriteAttributeString("type","home");
					xmlWriter.WriteAttributeString("number",patient.HmPhone);
					xmlWriter.WriteEndElement();
					//cell
					xmlWriter.WriteStartElement("phone");
					if(patient.PatStatus==PatientStatus.Deleted){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("patient-id",patient.PatNum.ToString());
					xmlWriter.WriteAttributeString("type","cell");
					xmlWriter.WriteAttributeString("number",patient.WirelessPhone);
					xmlWriter.WriteEndElement();
					//work
					xmlWriter.WriteStartElement("phone");
					if(patient.PatStatus==PatientStatus.Deleted){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("patient-id",patient.PatNum.ToString());
					xmlWriter.WriteAttributeString("type","work");
					xmlWriter.WriteAttributeString("number",patient.WkPhone);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					patsInThisPost=i+1;
				}
				#endregion patient
				#region provider
				//provider-------------------------------------------------------------------------------------------------
				provsInThisPost=0;
				for(int i=0;i<listProvidersToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					provider=listProvidersToSync[i];
					xmlWriter.WriteStartElement("provider");
					if(provider.IsHidden){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",provider.ProvNum.ToString());
					if(provider.IsSecondary){
						xmlWriter.WriteAttributeString("type","hygienist");
					}
					//writer.WriteAttributeString("name-title",prov);
					xmlWriter.WriteAttributeString("name-first",provider.FName);
					xmlWriter.WriteAttributeString("name-middle",provider.MI);
					xmlWriter.WriteAttributeString("name-last",provider.LName);
					xmlWriter.WriteAttributeString("name-suffix",provider.Suffix);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					provsInThisPost=i+1;
				}
				#endregion provider
				#region appt
				//appointment-------------------------------------------------------------------------------------------------
				apptsInThisPost=0;
				for(int i=0;i<listAppointmentsToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					appointment=listAppointmentsToSync[i];
					xmlWriter.WriteStartElement("appointment");
					if(appointment.AptStatus==ApptStatus.Broken
						|| appointment.AptStatus==ApptStatus.Planned
						|| appointment.AptStatus==ApptStatus.PtNote
						|| appointment.AptStatus==ApptStatus.PtNoteCompleted
						|| appointment.AptStatus==ApptStatus.UnschedList)
					{
						xmlWriter.WriteAttributeString("action","delete");
					}
					else if(appointment.AptStatus==ApptStatus.Complete
						|| appointment.AptStatus==ApptStatus.None
						|| appointment.AptStatus==ApptStatus.Scheduled)
					{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",appointment.AptNum.ToString());
					xmlWriter.WriteAttributeString("patient-id",appointment.PatNum.ToString());
					xmlWriter.WriteAttributeString("provider-id",appointment.ProvNum.ToString());
					xmlWriter.WriteAttributeString("operatory-id",appointment.Op.ToString());
					xmlWriter.WriteAttributeString("start",appointment.AptDateTime.ToString("yyyy-MM-dd HH:mm"));
					xmlWriter.WriteAttributeString("length",(appointment.Pattern.Length*5).ToString());
					xmlWriter.WriteAttributeString("description",appointment.ProcDescript);
					//A comma-separated list of procedure code ids:
					xmlWriter.WriteAttributeString("procedure-code-ids",listApptProcsToSync[i]);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					apptsInThisPost=i+1;
				}
				#endregion appt
				#region delobj
				//deleted objects-------------------------------------------------------------------------------------------------
				delObjInThisPost=0;
				for(int i=0;i<listDeletedObjectsToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					deletedObject=listDeletedObjectsToSync[i];
					if(deletedObject.ObjectType==DeletedObjectType.Appointment){
						xmlWriter.WriteStartElement("appointment");
					}
					else if(deletedObject.ObjectType==DeletedObjectType.ScheduleProv){
						xmlWriter.WriteStartElement("schedule");
					}
					else if(deletedObject.ObjectType==DeletedObjectType.RecallPatNum){
						xmlWriter.WriteStartElement("recall");
					}
					xmlWriter.WriteAttributeString("action","delete");
					if(deletedObject.ObjectType==DeletedObjectType.RecallPatNum){
						xmlWriter.WriteAttributeString("patient-id",deletedObject.ObjectNum.ToString());
					}
					else{
						xmlWriter.WriteAttributeString("id",deletedObject.ObjectNum.ToString());
					}
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					delObjInThisPost=i+1;
				}
				#endregion delobj
				#region sched
				//schedules-------------------------------------------------------------------------------------------------
				schedsInThisPost=0;
				for(int i=0;i<listSchedulesToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					schedule=listSchedulesToSync[i];
					xmlWriter.WriteStartElement("schedule");
					xmlWriter.WriteAttributeString("action","");
					xmlWriter.WriteAttributeString("id",schedule.ScheduleNum.ToString());
					xmlWriter.WriteAttributeString("provider-id",schedule.ProvNum.ToString());
					xmlWriter.WriteAttributeString("recur","none");
					xmlWriter.WriteAttributeString("date",schedule.SchedDate.ToString("yyyy-MM-dd"));
					xmlWriter.WriteAttributeString("start-time",schedule.StartTime.ToString("HH:mm"));
					xmlWriter.WriteAttributeString("finish-time",schedule.StopTime.ToString("HH:mm"));
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					schedsInThisPost=i+1;
				}
				#endregion sched
				#region operatories
				//operatories-------------------------------------------------------------------------------------------------
				opsInThisPost=0;
				for(int i=0;i<listOperatoriesToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					operatory=listOperatoriesToSync[i];
					xmlWriter.WriteStartElement("operatory");
					if(operatory.IsHidden){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",operatory.OperatoryNum.ToString());
					xmlWriter.WriteAttributeString("name",operatory.OpName);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					opsInThisPost=i+1;
				}
				#endregion operatories
				#region recalls
				//recalls-------------------------------------------------------------------------------------------------
				recallsInThisPost=0;
				for(int i=0;i<listRecallsToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					recall=listRecallsToSync[i];
					xmlWriter.WriteStartElement("recall");
					if(recall.IsDisabled){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("patient-id",recall.PatNum.ToString());
					xmlWriter.WriteAttributeString("type","prophy");
					//writer.WriteAttributeString("length","");//missing so use practice default
					//if(recall.IsDisabled){
					//	writer.WriteAttributeString("eligible","false");
					//}
					//else{
						xmlWriter.WriteAttributeString("eligible","true");
					//}
					str="";
					if(recall.RecallInterval.Years>0){
						str=recall.RecallInterval.Years.ToString()+" year";
						if(recall.RecallInterval.Years>1){
							str+="s";
						}
					}
					else if(recall.RecallInterval.Months>0){
						str=recall.RecallInterval.Months.ToString()+" month";
						if(recall.RecallInterval.Months>1){
							str+="s";
						}
					}
					else if(recall.RecallInterval.Days>0){
						str=recall.RecallInterval.Days.ToString()+" day";
						if(recall.RecallInterval.Days>1){
							str+="s";
						}
					}
					xmlWriter.WriteAttributeString("freq",str);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					recallsInThisPost=i+1;
				}
				#endregion recalls
				#region procedure codes
				//procedure codes-------------------------------------------------------------------------------------------------
				codesInThisPost=0;
				for(int i=0;i<listProcedureCodesToSync.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					procedureCode=listProcedureCodesToSync[i];
					xmlWriter.WriteStartElement("procedure-code");
					if(Defs.GetHidden(DefCat.ProcCodeCats,procedureCode.ProcCat)){
						xmlWriter.WriteAttributeString("action","delete");
					}
					else{
						xmlWriter.WriteAttributeString("action","");
					}
					xmlWriter.WriteAttributeString("id",procedureCode.CodeNum.ToString());
					xmlWriter.WriteAttributeString("ada-code",procedureCode.ProcCode);
					xmlWriter.WriteAttributeString("abbrev",procedureCode.AbbrDesc);
					xmlWriter.WriteAttributeString("description",procedureCode.LaymanTerm);
					xmlWriter.WriteAttributeString("long-description",procedureCode.Descript);
					xmlWriter.WriteEndElement();
					objectsInThisPost++;
					codesInThisPost=i+1;
				}
				#endregion procedure codes
				xmlWriter.WriteEndElement();//PracticeClient
				xmlWriter.Dispose();//also flushes
				//File.AppendAllText(@"E:\My Documents\Bridge Info\UAppoint\Output.txt",strBuild.ToString());
				//Thread.Sleep(TimeSpan.FromMinutes(10));
				postData=stringBuilder_.ToString();
				httpWebRequest=(HttpWebRequest)WebRequest.Create(serverName);
				httpWebRequest.KeepAlive=false;
				httpWebRequest.Method="POST";
				httpWebRequest.ContentType="application/x-www-form-urlencoded";
				httpWebRequest.ContentLength=postData.Length;
				ASCIIEncoding aSCIIEncoding=new ASCIIEncoding();
				byte[] byteArray=aSCIIEncoding.GetBytes(postData);
				Stream streamOut=httpWebRequest.GetRequestStream();
				streamOut.Write(byteArray,0,byteArray.Length);
				streamOut.Dispose();//also flushes
				WebResponse webResponse;
				try{
					webResponse=httpWebRequest.GetResponse();
				}
				catch(Exception ex){
					//typical error is: Bad gateway
					//This can happen even during a normal upload sequence soon after a successful post.
					File.AppendAllText(_logfile,DateTime.Now.ToString()+"  Error:"+ex.Message+"   Sleeping for "+intervalSecError.ToString()+" seconds."+"\r\n");
					ProgramProperties.SetProperty(program.ProgramNum,"SynchStatus","Error:"+ex.Message+"   Sleeping for "+intervalSecError.ToString()+" seconds.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSecError));
					continue;
				}
				//Process the response:
				StreamReader streamReader=new StreamReader(webResponse.GetResponseStream(),Encoding.ASCII);
				string responseStr=streamReader.ReadToEnd();
				streamReader.Dispose();
				if(responseStr!="<server error=\"false\" />\r\n"){
					File.AppendAllText(_logfile,DateTime.Now.ToString()+"  ServerError.  "+responseStr+"  Sleeping for "+intervalSecError.ToString()+" seconds.\r\n");
					ProgramProperties.SetProperty(program.ProgramNum,"SynchStatus","ServerError.  "+responseStr+"  Sleeping for "+intervalSecError.ToString()+" seconds.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSecError));
					continue;
				}
				//success, so adjust all the lists--------------------------------------------------------------------------------------------
				if(patsInThisPost>0){//if at least some of them are patients
					if(patsInThisPost==listPatientsToSync.Count-1){//if we grabbed all the patients
						listPatientsToSync.Clear();
					}
					else{
						listPatientsToSync=listPatientsToSync.GetRange(patsInThisPost,listPatientsToSync.Count-patsInThisPost);
					}
				}
				if(provsInThisPost>0){
					if(provsInThisPost==listProvidersToSync.Count-1){
						listProvidersToSync.Clear();
					}
					else{
						listProvidersToSync=listProvidersToSync.GetRange(provsInThisPost,listProvidersToSync.Count-provsInThisPost);
					}
				}
				if(apptsInThisPost>0){
					if(apptsInThisPost==listAppointmentsToSync.Count-1){
						listAppointmentsToSync.Clear();
						listApptProcsToSync.Clear();
					}
					else{
						listAppointmentsToSync=listAppointmentsToSync.GetRange(apptsInThisPost,listAppointmentsToSync.Count-apptsInThisPost);
						listApptProcsToSync=listApptProcsToSync.GetRange(apptsInThisPost,listApptProcsToSync.Count-apptsInThisPost);
					}
				}
				if(delObjInThisPost>0){
					if(delObjInThisPost==listDeletedObjectsToSync.Count-1){
						listDeletedObjectsToSync.Clear();
					}
					else{
						listDeletedObjectsToSync=listDeletedObjectsToSync.GetRange(delObjInThisPost,listDeletedObjectsToSync.Count-delObjInThisPost);
					}
				}
				if(schedsInThisPost>0){
					if(schedsInThisPost==listSchedulesToSync.Count-1){
						listSchedulesToSync.Clear();
					}
					else{
						listSchedulesToSync=listSchedulesToSync.GetRange(schedsInThisPost,listSchedulesToSync.Count-schedsInThisPost);
					}
				}
				if(opsInThisPost>0){
					if(opsInThisPost==listOperatoriesToSync.Count-1){
						listOperatoriesToSync.Clear();
					}
					else{
						listOperatoriesToSync=listOperatoriesToSync.GetRange(opsInThisPost,listOperatoriesToSync.Count-opsInThisPost);
					}
				}
				if(recallsInThisPost>0){
					if(recallsInThisPost==listRecallsToSync.Count-1){
						listRecallsToSync.Clear();
					}
					else{
						listRecallsToSync=listRecallsToSync.GetRange(recallsInThisPost,listRecallsToSync.Count-recallsInThisPost);
					}
				}
				if(codesInThisPost>0){
					if(codesInThisPost==listProcedureCodesToSync.Count-1){
						listProcedureCodesToSync.Clear();
					}
					else{
						listProcedureCodesToSync=listProcedureCodesToSync.GetRange(codesInThisPost,listProcedureCodesToSync.Count-codesInThisPost);
					}
				}
				if(totalObjectsToSynch==objectsInThisPost){
					dateTimeLastUploaded=timeStartSynch+deltaTimeSpan;
					ProgramProperties.SetProperty(program.ProgramNum,"DateTimeLastUploaded",POut.DateT(dateTimeLastUploaded,false));
					//POut.PDateT(MiscData.GetNowDateTime()));
				}
				//there are still objects to upload.
				//This sleep is only for debugging so we don't swamp the server.
				//Thread.Sleep(TimeSpan.FromMilliseconds(500));
			}
			while(true);
		}

		private bool SaveToDb(){
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.UAppoint,out string err)) {
				MessageBox.Show(err);
				return false;
			}
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return false;
			}
			if(textProgDesc.Text==""){
				MessageBox.Show("Description may not be blank.");
				return false;
			}
			if(checkEnabled.Checked && textPath.Text==""){
				MessageBox.Show("URL may not be blank.");
				return false;
			}
			//check for valid url?
			if(checkEnabled.Checked && textUsername.Text==""){
				MessageBox.Show("Username may not be blank.");
				return false;
			}
			if(checkEnabled.Checked && textPassword.Text==""){
				MessageBox.Show("Password may not be blank.");
				return false;
			}
			if(checkEnabled.Checked && textWorkstationName.Text==""){
				MessageBox.Show("Workstation name may not be blank.");
				return false;
			}
			if(checkEnabled.Checked && Environment.MachineName!=textWorkstationName.Text.ToUpper()){
				MessageBox.Show("This workstation is: "+Environment.MachineName+".  The workstation entered does not match.\r\n"
					+"UAppoint setup should only be performed from the workstation responsible for synch.");
				return false;
			}
			int intervalSec=0;
			try{
				intervalSec=PIn.Int(textIntervalSeconds.Text);//"" is handled fine here
			}
			catch{
				MessageBox.Show("Please fix the interval in seconds.");
				return false;
			}
			if(checkEnabled.Checked && intervalSec<1){
				MessageBox.Show("Interval in seconds must be greater than zero.");
				return false;
			}
			DateTime datetime=DateTime.MinValue;
			if(textDateTimeLastUploaded2.Text!=""){
				try{
					datetime=DateTime.Parse(textDateTimeLastUploaded2.Text);
				}
				catch{
					MessageBox.Show("Please fix the DateTime last uploaded.");
					return false;
				}
			}
			ProgramCur.ProgDesc=textProgDesc.Text;
			ProgramCur.Enabled=checkEnabled.Checked;
			ProgramCur.Path=textPath.Text;
			ProgramCur.Note=textNote.Text;
			Programs.Update(ProgramCur);
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"Username",textUsername.Text);
			string passwordEncrypted=CDT.Class1.TryEncrypt(textPassword.Text);
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"Password",passwordEncrypted);
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"WorkstationName",textWorkstationName.Text.ToUpper());
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"IntervalSeconds",intervalSec.ToString());
			if(textDateTimeLastUploaded2.Text!=""){
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"DateTimeLastUploaded",POut.DateT(datetime,false));
			}
			DataValid.SetInvalid(InvalidType.Programs);
			for(int i=0;i<_listProgramProperties.Count;i++) {
				if(_listProgramProperties[i].IsHighSecurity && ProgramProperties.GetPropVal(ProgramCur.ProgramNum,_listProgramProperties[i].PropertyDesc)!=_listProgramProperties[i].TagOD.ToString()){
					string logText=$"{ProgramCur.ProgDesc}+'s {_listProgramProperties[i].PropertyDesc} for headquarters was altered.";
					SecurityLogs.MakeLogEntry(Permissions.ManageHighSecurityProgProperties,0,logText,ProgramCur.ProgramNum,DateTime.Now);
				}
			}
			return true;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!SaveToDb()){
				return;
			}
			if(MessageBox.Show("Restart synchronization?  You should not restart if you are in the middle of a big synchronization and you have not changed anything since opening this window or clicking Restart.","",MessageBoxButtons.YesNo)==DialogResult.Yes)
			{
				StartThreadIfEnabled();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			//Let the users see what they are typing if they clear out the password field completely
			if(textPassword.Text.Trim().Length==0) {
				textPassword.UseSystemPasswordChar=false;
			}
		}
	}
}