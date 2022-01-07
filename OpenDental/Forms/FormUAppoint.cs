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
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormUAppoint:FormODBase {
		/// <summary>This Program link is new.</summary>
		public bool IsNew;
		public Program ProgramCur;
		private List<ProgramProperty> PropertyList;
		private static Thread thread;
		private static string logfile="UAppointLog.txt";

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
			PropertyList=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			textUsername.Text=GetProp("Username");
			textPassword.Text=GetProp("Password");
			textWorkstationName.Text=GetProp("WorkstationName");
			textIntervalSeconds.Text=GetProp("IntervalSeconds");
			DateTime datet=PIn.DateT(GetProp("DateTimeLastUploaded"));
			if(datet.Year>1880){
				textDateTimeLastUploaded1.Text=datet.ToShortDateString()+"  "+datet.ToShortTimeString();
			}
			//textSynchStatus.Text=GetProp("SynchStatus");//auto
			textNote.Text=ProgramCur.Note;
			textPassword.UseSystemPasswordChar=textPassword.Text.Trim().Length>0;
		}

		private string GetProp(string desc){
			for(int i=0;i<PropertyList.Count;i++){
				if(PropertyList[i].PropertyDesc==desc){
					if(PropertyList[i].IsMasked) {
						return CDT.Class1.TryDecrypt((PropertyList[i].PropertyValue));
					}
					return PropertyList[i].PropertyValue;
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
			DateTime datet=PIn.DateT(propVal);
			if(datet.Year<1880){
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This is an initial synchronization.  It could take a while.  You can probably continue to work on this computer, but you will need to leave the program running on this workstation until the synch is done.  Begin initial synchronization?"))
				{
					return;
				}
				File.AppendAllText(logfile,DateTime.Now.ToString()+"  Initial synchronization running.\r\n");
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"SynchStatus","Initial synchronization running.");
			}
			StartThreadIfEnabled();
		}

		private void butViewLog_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return;
			}
			Process.Start(logfile);
		}

		///<summary>Spawns a thread that handled uploading data to UAppoint in real time.  If the thread is already running, then this restarts it.  If the uploading should no longer happen, then this aborts the thread and exits.</summary>
		public static void StartThreadIfEnabled(){
			if(thread!=null){
				thread.Abort();
			}
			Program prog=Programs.GetCur(ProgramName.UAppoint);
			if(prog==null){
				return;
			}
			if(!Programs.IsEnabledByHq(prog,out _) || !prog.Enabled || ODBuild.IsWeb()){
				return;
			}
			//get current time and use delta from now on?
			thread=new Thread(ThreadStartTarget);
			thread.Start(prog);
		}

		public static void AbortThread(){
			if(thread!=null){
				thread.Abort();
			}
		}

		private static void ThreadStartTarget(object data){
			File.WriteAllText(logfile,DateTime.Now.ToString()+"  Synch thread started.\r\n");//creates or clears the log
			Program prog=(Program)data;
			int intervalSec=PIn.Int(ProgramProperties.GetPropVal(prog.ProgramNum,"IntervalSeconds"));
			int intervalSecError=intervalSec*4;
			string username=ProgramProperties.GetPropVal(prog.ProgramNum,"Username");
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(prog.ProgramNum,"Password"));
			DateTime dateTimeLastUploaded=PIn.DateT(ProgramProperties.GetPropVal(prog.ProgramNum,"DateTimeLastUploaded"));
			//track delta here
			DateTime nowServer=MiscData.GetNowDateTime();
			TimeSpan deltaTimeSpan=nowServer-DateTime.Now;//this was tested to work by adding delta to local time
			DateTime timeStartSynch=DateTime.MinValue;
			string serverName=prog.Path;
			HttpWebRequest webReq;
			string postData;//data just for the current post.
			List<Patient> patientsToSynch=new List<Patient>();
			List<Provider> provsToSynch=new List<Provider>();
			List<Appointment> apptsToSynch=new List<Appointment>();
			List<string> apptProcsToSynch=new List<string>();
			List<DeletedObject> delObjToSynch=new List<DeletedObject>();
			List<Schedule> schedsToSynch=new List<Schedule>();
			List<Operatory> opsToSynch=new List<Operatory>();
			List<Recall> recallsToSynch=new List<Recall>();
			List<ProcedureCode> codesToSynch=new List<ProcedureCode>();
			int totalObjectsToSynch=0;
			string synchstatus="";
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.ConformanceLevel=ConformanceLevel.Fragment;
			settings.Indent=true;
			settings.IndentChars="   ";
			StringBuilder strBuild;
			XmlWriter writer;
			Version version=new Version(Application.ProductVersion);
			int objectsInThisPost;
			Patient pat;
			Provider prov;
			Appointment appt;
			DeletedObject delObj;
			Schedule sched;
			Operatory op;
			Recall recall;
			ProcedureCode code;
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
					=patientsToSynch.Count
					+provsToSynch.Count
					+apptsToSynch.Count
					+delObjToSynch.Count
					+schedsToSynch.Count
					+opsToSynch.Count
					+recallsToSynch.Count
					+codesToSynch.Count;
				if(totalObjectsToSynch==0){//if there are no objects ready to upload
					timeStartSynch=DateTime.Now;
					//get various objects from the database.
					patientsToSynch=Patients.GetChangedSince(dateTimeLastUploaded);//datetime will be handled better soon with delta
					provsToSynch=Providers.GetChangedSince(dateTimeLastUploaded);
					apptsToSynch=Appointments.GetChangedSince(dateTimeLastUploaded,DateTime.MinValue);
					apptProcsToSynch=Appointments.GetUAppointProcs(apptsToSynch);
					delObjToSynch=DeletedObjects.GetDeletedSince(dateTimeLastUploaded);
					schedsToSynch=Schedules.GetChangedSince(dateTimeLastUploaded);
					opsToSynch=Operatories.GetChangedSince(dateTimeLastUploaded);
					recallsToSynch=Recalls.GetChangedSince(dateTimeLastUploaded);
					codesToSynch=ProcedureCodes.GetChangedSince(dateTimeLastUploaded);
				}
				totalObjectsToSynch
					=patientsToSynch.Count
					+provsToSynch.Count
					+apptsToSynch.Count
					+delObjToSynch.Count
					+schedsToSynch.Count
					+opsToSynch.Count
					+recallsToSynch.Count
					+codesToSynch.Count;
				if(totalObjectsToSynch==0){//if there are still no objects
					File.AppendAllText(logfile,DateTime.Now.ToString()+"  Current.  Sleeping between synch.\r\n");
					ProgramProperties.SetProperty(prog.ProgramNum,"SynchStatus","Current.  Sleeping between synch.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSec));//sleep for a while
					continue;
				}
				synchstatus="Synching.  Objects remaining: ";
				if(patientsToSynch.Count>0){
					synchstatus+=patientsToSynch.Count.ToString()+" patients, ";
				}
				if(provsToSynch.Count>0){
					synchstatus+=provsToSynch.Count.ToString()+" providers, ";
				}
				if(apptsToSynch.Count>0){
					synchstatus+=apptsToSynch.Count.ToString()+" appts, ";
				}
				if(delObjToSynch.Count>0){
					synchstatus+=delObjToSynch.Count.ToString()+" deletions, ";
				}
				if(schedsToSynch.Count>0){
					synchstatus+=schedsToSynch.Count.ToString()+" schedules, ";
				}
				if(opsToSynch.Count>0){
					synchstatus+=opsToSynch.Count.ToString()+" operatories, ";
				}
				if(recallsToSynch.Count>0){
					synchstatus+=recallsToSynch.Count.ToString()+" recalls, ";
				}
				if(codesToSynch.Count>0){
					synchstatus+=codesToSynch.Count.ToString()+" codes, ";
				}
				File.AppendAllText(logfile,DateTime.Now.ToString()+"  "+synchstatus+"\r\n");
				ProgramProperties.SetProperty(prog.ProgramNum,"SynchStatus",synchstatus);
				strBuild=new StringBuilder();
				writer=XmlWriter.Create(strBuild,settings);
				writer.WriteStartElement("PracticeClient");
				writer.WriteAttributeString("user",username);
				writer.WriteAttributeString("client-version","OD-"+version.Major.ToString()+"."+version.Minor.ToString()+"."+version.Build.ToString());
				writer.WriteAttributeString("pass-md5",password);
				#endregion firstPart
				#region patient
				//patient-------------------------------------------------------------------------------------------------
				patsInThisPost=0;
				for(int i=0;i<patientsToSynch.Count;i++){
					if(objectsInThisPost>=50){//0, some, or all of them might be patients
						break;
					}
					pat=patientsToSynch[i];
					//patient:
					writer.WriteStartElement("patient");
					if(pat.PatStatus==PatientStatus.Deleted){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",pat.PatNum.ToString());
					writer.WriteAttributeString("name-title","");//(Dr., Mrs., etc.) optional 
					writer.WriteAttributeString("name-first",pat.FName);
					writer.WriteAttributeString("name-middle",pat.MiddleI);
					writer.WriteAttributeString("name-last",pat.LName);
					//writer.WriteAttributeString("name-suffix","");//(Jr., TDS, etc)  optional 
					writer.WriteAttributeString("email",pat.Email);
					if(pat.Birthdate.Year>1880){//notice that there's no way to clear a birthdate
						writer.WriteAttributeString("birthdate",pat.Birthdate.ToString("yyyy-MM-dd"));
					}
					writer.WriteAttributeString("provider-id",pat.PriProv.ToString());
					writer.WriteAttributeString("address-id",pat.PatNum.ToString());
					//writer.WriteAttributeString("status",);
					writer.WriteEndElement();
					//Address--------------------------------------------------------------------------------------------------
					writer.WriteStartElement("address");
					if(pat.PatStatus==PatientStatus.Deleted){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",pat.PatNum.ToString());
					writer.WriteAttributeString("street1",pat.Address);
					writer.WriteAttributeString("street2",pat.Address2);
					writer.WriteAttributeString("city",pat.City);
					writer.WriteAttributeString("state",pat.State);
					writer.WriteAttributeString("zip",pat.Zip);
					writer.WriteEndElement();
					//Phone--------------------------------------------------------------------------------------------------
					//primary key is the id + type
					//home
					writer.WriteStartElement("phone");
					if(pat.PatStatus==PatientStatus.Deleted){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("patient-id",pat.PatNum.ToString());
					writer.WriteAttributeString("type","home");
					writer.WriteAttributeString("number",pat.HmPhone);
					writer.WriteEndElement();
					//cell
					writer.WriteStartElement("phone");
					if(pat.PatStatus==PatientStatus.Deleted){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("patient-id",pat.PatNum.ToString());
					writer.WriteAttributeString("type","cell");
					writer.WriteAttributeString("number",pat.WirelessPhone);
					writer.WriteEndElement();
					//work
					writer.WriteStartElement("phone");
					if(pat.PatStatus==PatientStatus.Deleted){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("patient-id",pat.PatNum.ToString());
					writer.WriteAttributeString("type","work");
					writer.WriteAttributeString("number",pat.WkPhone);
					writer.WriteEndElement();
					objectsInThisPost++;
					patsInThisPost=i+1;
				}
				#endregion patient
				#region provider
				//provider-------------------------------------------------------------------------------------------------
				provsInThisPost=0;
				for(int i=0;i<provsToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					prov=provsToSynch[i];
					writer.WriteStartElement("provider");
					if(prov.IsHidden){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",prov.ProvNum.ToString());
					if(prov.IsSecondary){
						writer.WriteAttributeString("type","hygienist");
					}
					//writer.WriteAttributeString("name-title",prov);
					writer.WriteAttributeString("name-first",prov.FName);
					writer.WriteAttributeString("name-middle",prov.MI);
					writer.WriteAttributeString("name-last",prov.LName);
					writer.WriteAttributeString("name-suffix",prov.Suffix);
					writer.WriteEndElement();
					objectsInThisPost++;
					provsInThisPost=i+1;
				}
				#endregion provider
				#region appt
				//appointment-------------------------------------------------------------------------------------------------
				apptsInThisPost=0;
				for(int i=0;i<apptsToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					appt=apptsToSynch[i];
					writer.WriteStartElement("appointment");
					if(appt.AptStatus==ApptStatus.Broken
						|| appt.AptStatus==ApptStatus.Planned
						|| appt.AptStatus==ApptStatus.PtNote
						|| appt.AptStatus==ApptStatus.PtNoteCompleted
						|| appt.AptStatus==ApptStatus.UnschedList)
					{
						writer.WriteAttributeString("action","delete");
					}
					else if(appt.AptStatus==ApptStatus.Complete
						|| appt.AptStatus==ApptStatus.None
						|| appt.AptStatus==ApptStatus.Scheduled)
					{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",appt.AptNum.ToString());
					writer.WriteAttributeString("patient-id",appt.PatNum.ToString());
					writer.WriteAttributeString("provider-id",appt.ProvNum.ToString());
					writer.WriteAttributeString("operatory-id",appt.Op.ToString());
					writer.WriteAttributeString("start",appt.AptDateTime.ToString("yyyy-MM-dd HH:mm"));
					writer.WriteAttributeString("length",(appt.Pattern.Length*5).ToString());
					writer.WriteAttributeString("description",appt.ProcDescript);
					//A comma-separated list of procedure code ids:
					writer.WriteAttributeString("procedure-code-ids",apptProcsToSynch[i]);
					writer.WriteEndElement();
					objectsInThisPost++;
					apptsInThisPost=i+1;
				}
				#endregion appt
				#region delobj
				//deleted objects-------------------------------------------------------------------------------------------------
				delObjInThisPost=0;
				for(int i=0;i<delObjToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					delObj=delObjToSynch[i];
					if(delObj.ObjectType==DeletedObjectType.Appointment){
						writer.WriteStartElement("appointment");
					}
					else if(delObj.ObjectType==DeletedObjectType.ScheduleProv){
						writer.WriteStartElement("schedule");
					}
					else if(delObj.ObjectType==DeletedObjectType.RecallPatNum){
						writer.WriteStartElement("recall");
					}
					writer.WriteAttributeString("action","delete");
					if(delObj.ObjectType==DeletedObjectType.RecallPatNum){
						writer.WriteAttributeString("patient-id",delObj.ObjectNum.ToString());
					}
					else{
						writer.WriteAttributeString("id",delObj.ObjectNum.ToString());
					}
					writer.WriteEndElement();
					objectsInThisPost++;
					delObjInThisPost=i+1;
				}
				#endregion delobj
				#region sched
				//schedules-------------------------------------------------------------------------------------------------
				schedsInThisPost=0;
				for(int i=0;i<schedsToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					sched=schedsToSynch[i];
					writer.WriteStartElement("schedule");
					writer.WriteAttributeString("action","");
					writer.WriteAttributeString("id",sched.ScheduleNum.ToString());
					writer.WriteAttributeString("provider-id",sched.ProvNum.ToString());
					writer.WriteAttributeString("recur","none");
					writer.WriteAttributeString("date",sched.SchedDate.ToString("yyyy-MM-dd"));
					writer.WriteAttributeString("start-time",sched.StartTime.ToString("HH:mm"));
					writer.WriteAttributeString("finish-time",sched.StopTime.ToString("HH:mm"));
					writer.WriteEndElement();
					objectsInThisPost++;
					schedsInThisPost=i+1;
				}
				#endregion sched
				#region operatories
				//operatories-------------------------------------------------------------------------------------------------
				opsInThisPost=0;
				for(int i=0;i<opsToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					op=opsToSynch[i];
					writer.WriteStartElement("operatory");
					if(op.IsHidden){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",op.OperatoryNum.ToString());
					writer.WriteAttributeString("name",op.OpName);
					writer.WriteEndElement();
					objectsInThisPost++;
					opsInThisPost=i+1;
				}
				#endregion operatories
				#region recalls
				//recalls-------------------------------------------------------------------------------------------------
				recallsInThisPost=0;
				for(int i=0;i<recallsToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					recall=recallsToSynch[i];
					writer.WriteStartElement("recall");
					if(recall.IsDisabled){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("patient-id",recall.PatNum.ToString());
					writer.WriteAttributeString("type","prophy");
					//writer.WriteAttributeString("length","");//missing so use practice default
					//if(recall.IsDisabled){
					//	writer.WriteAttributeString("eligible","false");
					//}
					//else{
						writer.WriteAttributeString("eligible","true");
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
					writer.WriteAttributeString("freq",str);
					writer.WriteEndElement();
					objectsInThisPost++;
					recallsInThisPost=i+1;
				}
				#endregion recalls
				#region procedure codes
				//procedure codes-------------------------------------------------------------------------------------------------
				codesInThisPost=0;
				for(int i=0;i<codesToSynch.Count;i++){
					if(objectsInThisPost>=50){
						break;
					}
					code=codesToSynch[i];
					writer.WriteStartElement("procedure-code");
					if(Defs.GetHidden(DefCat.ProcCodeCats,code.ProcCat)){
						writer.WriteAttributeString("action","delete");
					}
					else{
						writer.WriteAttributeString("action","");
					}
					writer.WriteAttributeString("id",code.CodeNum.ToString());
					writer.WriteAttributeString("ada-code",code.ProcCode);
					writer.WriteAttributeString("abbrev",code.AbbrDesc);
					writer.WriteAttributeString("description",code.LaymanTerm);
					writer.WriteAttributeString("long-description",code.Descript);
					writer.WriteEndElement();
					objectsInThisPost++;
					codesInThisPost=i+1;
				}
				#endregion procedure codes
				writer.WriteEndElement();//PracticeClient
				writer.Close();
				//File.AppendAllText(@"E:\My Documents\Bridge Info\UAppoint\Output.txt",strBuild.ToString());
				//Thread.Sleep(TimeSpan.FromMinutes(10));
				postData=strBuild.ToString();
				webReq=(HttpWebRequest)WebRequest.Create(serverName);
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="application/x-www-form-urlencoded";
				webReq.ContentLength=postData.Length;
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] bytes=encoding.GetBytes(postData);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(bytes,0,bytes.Length);
				streamOut.Close();
				WebResponse response;
				try{
					response=webReq.GetResponse();
				}
				catch(Exception ex){
					//typical error is: Bad gateway
					//This can happen even during a normal upload sequence soon after a successful post.
					File.AppendAllText(logfile,DateTime.Now.ToString()+"  Error:"+ex.Message+"   Sleeping for "+intervalSecError.ToString()+" seconds."+"\r\n");
					ProgramProperties.SetProperty(prog.ProgramNum,"SynchStatus","Error:"+ex.Message+"   Sleeping for "+intervalSecError.ToString()+" seconds.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSecError));
					continue;
				}
				//Process the response:
				StreamReader readStream=new StreamReader(response.GetResponseStream(),Encoding.ASCII);
				string responseStr=readStream.ReadToEnd();
				readStream.Close();
				if(responseStr!="<server error=\"false\" />\r\n"){
					File.AppendAllText(logfile,DateTime.Now.ToString()+"  ServerError.  "+responseStr+"  Sleeping for "+intervalSecError.ToString()+" seconds.\r\n");
					ProgramProperties.SetProperty(prog.ProgramNum,"SynchStatus","ServerError.  "+responseStr+"  Sleeping for "+intervalSecError.ToString()+" seconds.");
					Thread.Sleep(TimeSpan.FromSeconds(intervalSecError));
					continue;
				}
				//success, so adjust all the lists--------------------------------------------------------------------------------------------
				if(patsInThisPost>0){//if at least some of them are patients
					if(patsInThisPost==patientsToSynch.Count-1){//if we grabbed all the patients
						patientsToSynch.Clear();
					}
					else{
						patientsToSynch=patientsToSynch.GetRange(patsInThisPost,patientsToSynch.Count-patsInThisPost);
					}
				}
				if(provsInThisPost>0){
					if(provsInThisPost==provsToSynch.Count-1){
						provsToSynch.Clear();
					}
					else{
						provsToSynch=provsToSynch.GetRange(provsInThisPost,provsToSynch.Count-provsInThisPost);
					}
				}
				if(apptsInThisPost>0){
					if(apptsInThisPost==apptsToSynch.Count-1){
						apptsToSynch.Clear();
						apptProcsToSynch.Clear();
					}
					else{
						apptsToSynch=apptsToSynch.GetRange(apptsInThisPost,apptsToSynch.Count-apptsInThisPost);
						apptProcsToSynch=apptProcsToSynch.GetRange(apptsInThisPost,apptProcsToSynch.Count-apptsInThisPost);
					}
				}
				if(delObjInThisPost>0){
					if(delObjInThisPost==delObjToSynch.Count-1){
						delObjToSynch.Clear();
					}
					else{
						delObjToSynch=delObjToSynch.GetRange(delObjInThisPost,delObjToSynch.Count-delObjInThisPost);
					}
				}
				if(schedsInThisPost>0){
					if(schedsInThisPost==schedsToSynch.Count-1){
						schedsToSynch.Clear();
					}
					else{
						schedsToSynch=schedsToSynch.GetRange(schedsInThisPost,schedsToSynch.Count-schedsInThisPost);
					}
				}
				if(opsInThisPost>0){
					if(opsInThisPost==opsToSynch.Count-1){
						opsToSynch.Clear();
					}
					else{
						opsToSynch=opsToSynch.GetRange(opsInThisPost,opsToSynch.Count-opsInThisPost);
					}
				}
				if(recallsInThisPost>0){
					if(recallsInThisPost==recallsToSynch.Count-1){
						recallsToSynch.Clear();
					}
					else{
						recallsToSynch=recallsToSynch.GetRange(recallsInThisPost,recallsToSynch.Count-recallsInThisPost);
					}
				}
				if(codesInThisPost>0){
					if(codesInThisPost==codesToSynch.Count-1){
						codesToSynch.Clear();
					}
					else{
						codesToSynch=codesToSynch.GetRange(codesInThisPost,codesToSynch.Count-codesInThisPost);
					}
				}
				if(totalObjectsToSynch==objectsInThisPost){
					dateTimeLastUploaded=timeStartSynch+deltaTimeSpan;
					ProgramProperties.SetProperty(prog.ProgramNum,"DateTimeLastUploaded",POut.DateT(dateTimeLastUploaded,false));
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





















