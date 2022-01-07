using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Phones {

		///<summary>The asterisk server ip setting can change at any time via the phone tracking server application.</summary>
		public static string PathPhoneMsg {
			get {
				string retVal=@"\\"+PrefC.GetString(PrefName.AsteriskServerIp);
				if(ODBuild.IsDebug()) {
					retVal=@"C:\Users\chris\Documents\VoiceMails";//Change as needed.
				}
				else {
					retVal+=@"\Voicemail\default\998\INBOX";
				}
				return retVal;
			}
		}

		///<summary>Define a color scheme for a phone. FormMapHQ uses DUAL while PhoneTile uses SINGLE.</summary>
		public class PhoneColorScheme {
			#region Pre-Defined Dual Color Scheme
			
			private static Color COLOR_DUAL_FontHere=Color.Black;
			private static Color COLOR_DUAL_FontAway=Color.FromArgb(186,186,186);
			private static Color COLOR_DUAL_InnerHome=Color.FromArgb(245,245,245);
			private static Color COLOR_DUAL_OuterHome=Color.FromArgb(191,191,191);
			private static Color COLOR_DUAL_InnerNeedsHelp=Color.FromArgb(249,233,249);
			private static Color COLOR_DUAL_OuterNeedsHelp=Color.Orchid;
			private static Color COLOR_DUAL_InnerHelpOnTheWay=Color.Lavender;
			private static Color COLOR_DUAL_OuterHelpOnTheWay=Color.FromArgb(220,180,230); 
			private static Color COLOR_DUAL_InnerNoColor=Color.FromArgb(245,245,245);
			private static Color COLOR_DUAL_OuterNoColor=Color.FromArgb(100,100,100);
			private static Color COLOR_DUAL_InnerUnavailable=Color.FromArgb(245,245,245);
			private static Color COLOR_DUAL_OuterUnavailable=Color.FromArgb(100,100,100);
			private static Color COLOR_DUAL_InnerTriageAway=Color.White;
			public static Color COLOR_DUAL_InnerTriageHere=Color.LightCyan;
			public static Color COLOR_DUAL_OuterTriage=Color.Blue;
			private static Color COLOR_DUAL_InnerOnPhone=Color.FromArgb(254,235,233);
			private static Color COLOR_DUAL_OuterOnPhone=Color.Red;
			private static Color COLOR_DUAL_InnerLunchBreak=Color.White;
			private static Color COLOR_DUAL_OuterLunchBreak=Color.LimeGreen;
			private static Color COLOR_DUAL_InnerAvailable=Color.FromArgb(217,255,217);
			private static Color COLOR_DUAL_OuterAvailable=Color.LimeGreen;
			private static Color COLOR_DUAL_InnerTrainingWrap=Color.FromArgb(255,255,135);
			private static Color COLOR_DUAL_OuterTrainingWrap=Color.LimeGreen;
			private static Color COLOR_DUAL_InnerBackup=Color.FromArgb(236,255,236);
			private static Color COLOR_DUAL_OuterBackup=Color.FromArgb(100,100,100);
			private static Color COLOR_DUAL_InnerAssist=Color.FromArgb(255,255,210);
			private static Color COLOR_DUAL_OuterAssist=Color.LimeGreen;

			#endregion

			#region Pre-Defined Single Color Scheme

			private static Color COLOR_SINGLE_Unavailable=Color.FromArgb(191,191,191);
			private static Color COLOR_SINGLE_TriageAway=Color.White;
			private static Color COLOR_SINGLE_TriageHere=Color.SkyBlue;
			private static Color COLOR_SINGLE_OnPhone=Color.Salmon;
			private static Color COLOR_SINGLE_LunchBreak=Color.White;
			private static Color COLOR_SINGLE_Available=Color.FromArgb(153,220,153);
			private static Color COLOR_SINGLE_TrainingWrap=Color.FromArgb(255,255,135);
			private static Color COLOR_SINGLE_Backup=Color.FromArgb(217,255,217);
			private static Color COLOR_SINGLE_Assist=Color.FromArgb(255,255,210);

			#endregion

			#region Public Data

			private bool _forDualColor;
			public bool ForDualColor {
				get {
					return _forDualColor;
				}
				set {
					_forDualColor=value;
					SetColorScheme(_forDualColor);
				}
			}
			public Color ColorFontHere;
			public Color ColorFontAway;
			public Color ColorInnerUnavailable;
			public Color ColorOuterUnavailable;
			public Color ColorInnerHome;
			public Color ColorOuterHome;
			public Color ColorInnerNeedsHelp;
			public Color ColorOuterNeedsHelp;
			public Color ColorInnerHelpOnTheWay;
			public Color ColorOuterHelpOnTheWay;
			public Color ColorInnerNoColor;
			public Color ColorOuterNoColor;
			public Color ColorInnerTriageAway;
			public Color ColorInnerTriageHere;
			public Color ColorOuterTriage;
			public Color ColorInnerOnPhone;
			public Color ColorOuterOnPhone;
			public Color ColorInnerLunchBreak;
			public Color ColorOuterLunchBreak;
			public Color ColorInnerAvailable;
			public Color ColorOuterAvailable;
			public Color ColorInnerTrainingWrap;
			public Color ColorOuterTrainingWrap;
			public Color ColorInnerBackup;
			public Color ColorOuterBackup;
			public Color ColorInnerAssist;
			public Color ColorOuterAssist;

			#endregion

			///<summary>Switch between single and dual color schemes</summary>
			public PhoneColorScheme(bool forDualColor) {
				ForDualColor=forDualColor;
			}

			///<summary>Set public available colors according to user preference</summary>
			private void SetColorScheme(bool forDualColor) {
				//default is dual color scheme	
				ColorFontHere=COLOR_DUAL_FontHere;
				ColorFontAway=COLOR_DUAL_FontAway;
				ColorInnerUnavailable=COLOR_DUAL_InnerUnavailable;
				ColorOuterUnavailable=COLOR_DUAL_OuterUnavailable;
				ColorInnerHome=COLOR_DUAL_InnerHome;
				ColorOuterHome=COLOR_DUAL_OuterHome;
				ColorInnerNeedsHelp=COLOR_DUAL_InnerNeedsHelp;
				ColorOuterNeedsHelp=COLOR_DUAL_OuterNeedsHelp;
				ColorInnerHelpOnTheWay=COLOR_DUAL_InnerHelpOnTheWay;
				ColorOuterHelpOnTheWay=COLOR_DUAL_OuterHelpOnTheWay;
				ColorInnerNoColor=COLOR_DUAL_InnerNoColor;
				ColorOuterNoColor=COLOR_DUAL_OuterNoColor;
				ColorInnerTriageAway=COLOR_DUAL_InnerTriageAway;
				ColorInnerTriageHere=COLOR_DUAL_InnerTriageHere;
				ColorOuterTriage=COLOR_DUAL_OuterTriage;
				ColorInnerOnPhone=COLOR_DUAL_InnerOnPhone;
				ColorOuterOnPhone=COLOR_DUAL_OuterOnPhone;
				ColorInnerLunchBreak=COLOR_DUAL_InnerLunchBreak;
				ColorOuterLunchBreak=COLOR_DUAL_OuterLunchBreak;
				ColorInnerAvailable=COLOR_DUAL_InnerAvailable;
				ColorOuterAvailable=COLOR_DUAL_OuterAvailable;
				ColorInnerTrainingWrap=COLOR_DUAL_InnerTrainingWrap;
				ColorOuterTrainingWrap=COLOR_DUAL_OuterTrainingWrap;
				ColorInnerBackup=COLOR_DUAL_InnerBackup;
				ColorOuterBackup=COLOR_DUAL_OuterBackup;
				ColorInnerAssist=COLOR_DUAL_InnerAssist;
				ColorOuterAssist=COLOR_DUAL_OuterAssist;

				//make any changes necessary for single color scheme
				if(!forDualColor) {
					ColorInnerUnavailable=COLOR_SINGLE_Unavailable;
					ColorOuterUnavailable=COLOR_SINGLE_Unavailable;
					ColorInnerTriageAway=COLOR_SINGLE_TriageAway;
					ColorInnerTriageHere=COLOR_SINGLE_TriageHere;
					ColorOuterTriage=COLOR_SINGLE_TriageHere;
					ColorInnerOnPhone=COLOR_SINGLE_OnPhone;
					ColorOuterOnPhone=COLOR_SINGLE_OnPhone;
					ColorInnerLunchBreak=COLOR_SINGLE_LunchBreak;
					ColorOuterLunchBreak=COLOR_SINGLE_LunchBreak;
					ColorInnerAvailable=COLOR_SINGLE_Available;
					ColorOuterAvailable=COLOR_SINGLE_Available;
					ColorInnerTrainingWrap=COLOR_SINGLE_TrainingWrap;
					ColorOuterTrainingWrap=COLOR_SINGLE_TrainingWrap;
					ColorInnerBackup=COLOR_SINGLE_Backup;
					ColorOuterBackup=COLOR_SINGLE_Backup;
					ColorInnerAssist=COLOR_SINGLE_Assist;
					ColorOuterAssist=COLOR_SINGLE_Assist;
				}
			}
		}

		public static void Insert(Phone phone) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phone);
				return;
			}
			Crud.PhoneCrud.Insert(phone);//db sets the PK
		}

		///<summary>Deletes all the phones with the corresponding PKs.  Used by the PhoneManager (external application).</summary>
		public static void DeletePhones(List<long> listPhoneNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPhoneNums);
				return;
			}
			if(listPhoneNums==null || listPhoneNums.Count < 1) {
				return;
			}
			string command="DELETE FROM phone WHERE PhoneNum IN ("+string.Join(",",listPhoneNums)+")";
			Db.NonQ(command);
		}

		///<summary>Gets every phone entry in the database.  By default the list of phones returned will not include the correspnond web cam images.
		///Set hasWebCamImages to true if the web cam images are needed which is only for the phone tile and phone control small (right now).
		///Set the extensionsNum to an extension other than 0 to only get the web cam image for that extensions.</summary>
		public static List<Phone> GetPhoneList() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Phone>>(MethodBase.GetCurrentMethod());
			}
			string command = "SELECT * FROM phone ORDER BY Extension";
			try {
				return Crud.PhoneCrud.SelectMany(command);
			}
			catch(Exception e) {
				e.DoNothing();
				//Phone table may not even exist. "Is Missing In General"
				return new List<Phone>();
			}
		}

		///<summary>Converts from string to enum and also handles conversion of Working to Available</summary>
		public static ClockStatusEnum GetClockStatusFromEmp(string empClockStatus) {
			//No need to check RemotingRole; no call to db.
			switch(empClockStatus) {
				case "Home":
					return ClockStatusEnum.Home;
				case "Lunch":
					return ClockStatusEnum.Lunch;
				case "Break":
					return ClockStatusEnum.Break;
				case "Working":
					return ClockStatusEnum.Available;
				default:
					return ClockStatusEnum.None;
			}
		}

		public static string ConvertClockStatusToString(ClockStatusEnum status) {
			switch(status) {
				case ClockStatusEnum.Lunch:
					return "Lunch";
				case ClockStatusEnum.Break:
					return "Break";
				case ClockStatusEnum.Available:
					return "Avail";
				case ClockStatusEnum.WrapUp:
					return "WrapU";
				case ClockStatusEnum.Training:
					return "Train";
				case ClockStatusEnum.TeamAssist:
					return "TmAst";
				case ClockStatusEnum.OfflineAssist:
					return "OffAst";
				case ClockStatusEnum.Backup:
					return "BackU";
				case ClockStatusEnum.Unavailable:
					return "UnAv";
				case ClockStatusEnum.NeedsHelp:
					return "Help";
				case ClockStatusEnum.HelpOnTheWay:
					return "OnWay";
				case ClockStatusEnum.TCResponder:
					return "Resp";
				case ClockStatusEnum.Off:
				case ClockStatusEnum.None:
				case ClockStatusEnum.Home:
				default:
					return status.ToString();
			}
		}

		///<summary>this code is similar to code in the phone tracking server.  But here, we frequently only change clockStatus and ColorBar by setting employeeNum=-1.  If employeeNum is not -1, then EmployeeName also gets set.  If employeeNum==0, then clears employee from that row.</summary>
		public static void SetPhoneStatus(ClockStatusEnum clockStatus,int extens,long employeeNum=-1) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clockStatus,extens,employeeNum);
				return;
			}
			string command=@"SELECT phoneempdefault.EmployeeNum,phoneempdefault.IsTriageOperator,Description,phoneempdefault.EmpName,HasColor,phone.ClockStatus "
				+"FROM phone "
				+"LEFT JOIN phoneempdefault ON phone.Extension=phoneempdefault.PhoneExt "
				+"WHERE phone.Extension="+POut.Long(extens);
			DataTable tablePhone=Db.GetTable(command);
			if(tablePhone.Rows.Count==0) {
				//It would be nice if we could create a phone row for this extension.
				return;
			}
			long empNum=PIn.Long(tablePhone.Rows[0]["EmployeeNum"].ToString());
			bool isTriageOperator=PIn.Bool(tablePhone.Rows[0]["IsTriageOperator"].ToString());
			string empName=PIn.String(tablePhone.Rows[0]["EmpName"].ToString());
			string clockStatusDb=PIn.String(tablePhone.Rows[0]["ClockStatus"].ToString());
			Employee emp=Employees.GetEmp(employeeNum);
			if(emp!=null) {//A new employee is going to take over this extension.
				empName=emp.FName;
				empNum=emp.EmployeeNum;
			}
			else if(employeeNum==0) {//Clear the employee from that row.
				empName="";
				empNum=0;
			}
			//if these values are null because of missing phoneempdefault row, they will default to false
			//PhoneEmpStatusOverride statusOverride=(PhoneEmpStatusOverride)PIn.Int(tablePhone.Rows[0]["StatusOverride"].ToString());
			bool hasColor=PIn.Bool(tablePhone.Rows[0]["HasColor"].ToString());
			#region DateTimeStart
			//When a user shows up as a color on the phone panel, we want a timer to be constantly going to show how long they've been off the phone.
			string dateTimeStart="";
			//It's possible that a new user has never clocked in before, therefore their clockStatus will be empty.  Simply set it to the status that they are trying to go to.
			if(clockStatusDb=="") {
				clockStatusDb=clockStatus.ToString();
			}
			if(clockStatus==ClockStatusEnum.Break
					|| clockStatus==ClockStatusEnum.Lunch) {
				//The user is going on Lunch or Break.  Start the DateTimeStart counter so we know how long they have been gone.
				dateTimeStart="DateTimeStart=NOW(), ";
			}
			else if(clockStatus==ClockStatusEnum.Home) 
			{
				//User is going Home.  Always clear the DateTimeStart column no matter what.
				dateTimeStart="DateTimeStart='0001-01-01', ";
			}
			else {//User shows as a color on big phones and is not going to a status of Home, Lunch, or Break.  Example: Available, Training etc.
				//Get the current clock status from the database.
				ClockStatusEnum clockStatusCur=(ClockStatusEnum)Enum.Parse(typeof(ClockStatusEnum),clockStatusDb);
				//Start the clock if the user is going from a break status to any other non-break status.
				if(clockStatusCur==ClockStatusEnum.Home
					|| clockStatusCur==ClockStatusEnum.Lunch
					|| clockStatusCur==ClockStatusEnum.Break) 
				{
					//The user is clocking in from home, lunch, or break.  Start the timer up.
					if(hasColor) {//Only start up the timer when someone with color clocks in.
						dateTimeStart="DateTimeStart=NOW(), ";
					}
					else { //Someone with no color then reset the timer. They are back from break, that's all we need to know.
						dateTimeStart="DateTimeStart='0001-01-01', ";
					}
				}
			}
			string dateTimeNeedsHelpStart;
			if(clockStatus==ClockStatusEnum.NeedsHelp) {
				dateTimeNeedsHelpStart="DateTimeNeedsHelpStart=NOW(), ";
			}
			else {
				dateTimeNeedsHelpStart="DateTimeNeedsHelpStart="+POut.DateT(DateTime.MinValue)+", ";
			}
			#endregion
			//Update the phone row to reflect the new clock status of the user.
			string clockStatusNew=clockStatus.ToString();
			if(clockStatus==ClockStatusEnum.None) {
				clockStatusNew="";
			}
			if(clockStatus==ClockStatusEnum.HelpOnTheWay && clockStatusDb == ClockStatusEnum.HelpOnTheWay.ToString()) {//If HelpOnTheWay already
				clockStatusNew = ClockStatusEnum.Available.ToString();
			}
			command="UPDATE phone SET ClockStatus='"+POut.String(clockStatusNew)+"', "
				+dateTimeStart
				+dateTimeNeedsHelpStart
				//+"ColorBar=-1, " //ColorBar is now determined at runtime by OD using Phones.GetPhoneColor.
				+"EmployeeNum="+POut.Long(empNum)+", "
				+"EmployeeName='"+POut.String(empName)+"' "
				+"WHERE Extension="+extens;
			Db.NonQ(command);
			//Zero out any duplicate phone table rows for this employee. 
			//This is possible if a user logged off and another employee logs into their computer. This would cause duplicate entries in the big phones window.
			UpdatePhoneToEmpty(employeeNum,extens);
		}

		///<summary>Zero out any duplicate phone table rows for this employee. This is possible if a user logged off and another employee logs into their computer. This would cause duplicate entries in the big phones window. If ignoreExtension less than 1 (inavlid) then zero out all entries for this employeeNum.</summary>
		public static void UpdatePhoneToEmpty(long employeeNum,int ignoreExtension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum,ignoreExtension);
				return;
			}
			string command="UPDATE phone SET "
					+"EmployeeName='', "
					+"ClockStatus='', "
					+"Description='', "
					+"EmployeeNum=0 "
					+"WHERE EmployeeNum="+POut.Long(employeeNum)+" "
					+"AND Extension!="+POut.Int(ignoreExtension);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Update(Phone phone,Phone phoneOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phone,phoneOld);
				return;
			}
			Crud.PhoneCrud.Update(phone,phoneOld);
		}

		///<summary>Consider all scenarios for a employee/phone/cubicle and return color and triage information</summary>
		public static void GetPhoneColor(Phone phone,PhoneEmpDefault phoneEmpDefault,bool forDualColorScheme,out Color outerColor,out Color innerColor,out Color fontColor,out bool isTriageOperatorOnTheClock) {
			PhoneColorScheme colorScheme=new PhoneColorScheme(forDualColorScheme);
			isTriageOperatorOnTheClock=false;
			//first set the font color
			if(phone==null
				|| phoneEmpDefault==null
				|| phone.ClockStatus==ClockStatusEnum.Home
				|| phone.ClockStatus==ClockStatusEnum.None
				|| phone.ClockStatus==ClockStatusEnum.Off) 
			{
				fontColor=colorScheme.ColorFontAway;
			}
			else {
				fontColor=colorScheme.ColorFontHere;
			}
			if(phoneEmpDefault==null || (!forDualColorScheme && !phoneEmpDefault.HasColor)) {//smaller color boxes need special colors
				innerColor=Color.Black;
				outerColor=Color.White;
				return;
			}
			//now cover all scenarios and set the inner and out color
			if(phone.ClockStatus==ClockStatusEnum.Home
				|| phone.ClockStatus==ClockStatusEnum.None
				|| phone.ClockStatus==ClockStatusEnum.Off) {
				//No color if employee is not currently working. Trumps all.
				outerColor=colorScheme.ColorOuterHome;
				innerColor=colorScheme.ColorInnerHome;
				return;
			}
			if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { //get this person help now!
				outerColor=colorScheme.ColorOuterNeedsHelp;
				innerColor=colorScheme.ColorInnerNeedsHelp;
				return;
			}
			if(phone.ClockStatus==ClockStatusEnum.HelpOnTheWay) { //getting this person help now!!!
				outerColor=colorScheme.ColorOuterHelpOnTheWay;
				innerColor=colorScheme.ColorInnerHelpOnTheWay;
				return;
			}
			if(!phoneEmpDefault.HasColor) { //not colored (generally an engineer or admin)
				outerColor=colorScheme.ColorOuterNoColor;
				innerColor=colorScheme.ColorInnerNoColor;
				return;
			}
			if(phone.ClockStatus==ClockStatusEnum.Unavailable) { //Unavailable is very rare and must be approved by management. Make them look like admin/engineer.
				outerColor=colorScheme.ColorOuterUnavailable;
				innerColor=colorScheme.ColorInnerUnavailable;
				return;
			}
			//If we get this far then the person is a tech who is working today.
			if(phoneEmpDefault.IsTriageOperator) {
				outerColor=colorScheme.ColorOuterTriage;
				if(phone.ClockStatus==ClockStatusEnum.Break 
					|| phone.ClockStatus==ClockStatusEnum.Lunch) {
					//triage op is working today but currently on break/lunch
					innerColor=colorScheme.ColorInnerTriageAway;
					if(!forDualColorScheme) { //smaller color boxes need special colors
						outerColor=colorScheme.ColorInnerTriageAway;
					}
				}
				else {
					//this is a triage operator who is currently here and on the clock
					isTriageOperatorOnTheClock=true;
					innerColor=colorScheme.ColorInnerTriageHere;
				}
				return;
			}
			if(phone.Description!="") { //Description field only has 'in use' when person is on the phone. That is the only time the field is not empty.
				outerColor=colorScheme.ColorOuterOnPhone;
				innerColor=colorScheme.ColorInnerOnPhone;
				return;
			}
			//We get this far so we are dealing with a tech who is not on a phone call. Handle each state.
			switch(phone.ClockStatus) {
				case ClockStatusEnum.Lunch:
				case ClockStatusEnum.Break:
					outerColor=colorScheme.ColorOuterLunchBreak;
					innerColor=colorScheme.ColorInnerLunchBreak;
					return;
				case ClockStatusEnum.Available:
					outerColor=colorScheme.ColorOuterAvailable;
					innerColor=colorScheme.ColorInnerAvailable;
					return;
				case ClockStatusEnum.WrapUp:
				case ClockStatusEnum.Training:
					outerColor=colorScheme.ColorOuterTrainingWrap;
					innerColor=colorScheme.ColorInnerTrainingWrap;
					return;
				case ClockStatusEnum.TeamAssist:
				case ClockStatusEnum.OfflineAssist:
				case ClockStatusEnum.TCResponder:
					outerColor=colorScheme.ColorOuterAssist;
					innerColor=colorScheme.ColorInnerAssist;
					return;
				case ClockStatusEnum.Backup:
					outerColor=colorScheme.ColorOuterBackup;
					innerColor=colorScheme.ColorInnerBackup;
					return;
				default:
					break;
			}
			throw new Exception("FormMapHQ.GetPhoneColor has a state that is currently unsupported!");
		}

		public static Phone GetPhoneForExtensionDB(int extension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Phone>(MethodBase.GetCurrentMethod(),extension);
			}
			string command = "SELECT * FROM phone WHERE Extension="+POut.Long(extension);
			try {
				return Crud.PhoneCrud.SelectOne(command);
			}
			catch {
				return null;
			}
		}

		public static Phone GetPhoneForExtension(List<Phone> phoneList,int extens) {
			//No need to check RemotingRole; no call to db.
			return phoneList.FirstOrDefault(x => x.Extension==extens);
		}

		public static Phone GetPhoneForEmployeeNum(List<Phone> phoneList,long employeeNum) {
			//No need to check RemotingRole; no call to db.
			return phoneList.FirstOrDefault(x => x.EmployeeNum==employeeNum);
		}

		///<summary>Gets the extension for the employee.  Returns 0 if employee cannot be found.</summary>
		public static int GetExtensionForEmp(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),employeeNum);
			}
			string command="SELECT Extension FROM phone WHERE EmployeeNum="+POut.Long(employeeNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0){
				return PIn.Int(table.Rows[0]["Extension"].ToString());
			}
			return 0;
		}

		/*
		///<summary>Gets the phoneNum which is the primary key, not the phone number.</summary>
		public static long GetPhoneNum(int extension){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),extension);
			}
			string command="SELECT PhoneNum FROM phone WHERE Extension ="+POut.Long(extension);
			string result= Db.GetScalar(command);
			return PIn.Long(result);
		}*/

		///<summary>Sets the employeeName and employeeNum for when someone else logs into another persons computer.</summary>
		public static void SetPhoneForEmp(long empNum,string empName,long extension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),empNum,empName,extension);
				return;
			}
			if(extension==0) {
				return;
			}
			string command="UPDATE phone SET "
				+"EmployeeName   = '"+POut.String(empName)+"', "
				+"EmployeeNum   = "+POut.Long(empNum)+" "
				+"WHERE Extension = "+POut.Long(extension);
			Db.NonQ(command);
		}

		///<summary>Use by the proximity sensors in WebcamOD/ProximityOD</summary>
		public static void SetProximity(bool isProximal,int extension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),isProximal,extension);
				return;
			}
			string command = "UPDATE phone SET DateTProximal = "+DbHelper.Now()+", IsProximal = "+POut.Bool(isProximal)
				+" WHERE Extension = "+POut.Int(extension);
			Db.NonQ(command);
		}

		public static void ClearImages() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="UPDATE phone SET WebCamImage=''";
			Db.NonQ(command);
		}

		///<summary>Gets list of TaskNums for new and viewed tasks within the Triage task list.</summary>
		public static List<long> GetTriageTaskNums() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			List<long> taskNums=new List<long>();
			string command="SELECT * FROM task "
				+"WHERE TaskListNum=1697 "//Triage task list.
				+"AND TaskStatus<>2";//Not done (new or viewed).
			List<Task> triageList=Crud.TaskCrud.SelectMany(command);
			for(int i=0;i<triageList.Count;i++) {
				taskNums.Add(triageList[i].TaskNum);
			}
			return taskNums;
		}

		///<summary>Returns the time of the oldest task within the Triage task list.  Returns 0 if there is no tasks in the list.</summary>
		public static DateTime GetTriageTime() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT GREATEST(IFNULL(task.DateTimeEntry,'0001-01-01'), IFNULL((SELECT MAX(DateTimeNote) FROM tasknote WHERE tasknote.tasknum=task.tasknum),'0001-01-01')) AS triageTime "
				+"FROM task "
				+"WHERE TaskListNum=1697 "//Triage task list.
				+"AND TaskStatus<>2 "//Not done (new or viewed).
				+"AND TaskNum NOT IN (SELECT TaskNum FROM tasknote) "//Not waiting a call back.
				+"AND PriorityDefNum IN(502,501) "//Blue and Red tasks only.
				+"LIMIT 1";
			return PIn.DateT(Db.GetScalar(command));
		}

		///<summary>Get triage metrics to be displayed in phone panels. Will never return null even if there are no triagemetrics in the db.</summary>
		public static TriageMetric GetTriageMetrics() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TriageMetric>(MethodBase.GetCurrentMethod());
			}
			TriageMetric triageMetric=null;
			try {
				triageMetric=TriageMetrics.GetMostRecent();
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			//If there is not a row in the table simply pass back 0's and the current time so the stats shown are all 0's.
			if(triageMetric==null) {
				triageMetric=new TriageMetric();
				triageMetric.CountBlueTasks=0;
				triageMetric.CountWhiteTasks=0;
				triageMetric.CountRedTasks=0;
				triageMetric.DateTimeOldestTriageTaskOrTaskNote=DateTime.MinValue;
				triageMetric.DateTimeOldestUrgentTaskOrTaskNote=DateTime.MinValue;
			}
			return triageMetric;
		}


		/// <summary>sorting class used to sort Phone in various ways</summary>
		public class PhoneComparer:IComparer<Phone> {
			private SortBy SortOn=SortBy.name;

			public PhoneComparer(SortBy sortBy) {
				SortOn=sortBy;
			}

			public int Compare(Phone x,Phone y) {
				int retVal=0;
				switch(SortOn) {
					case SortBy.empNum:
						retVal=x.EmployeeNum.CompareTo(y.EmployeeNum);
						break;
					case SortBy.ext:
						retVal=x.Extension.CompareTo(y.Extension);
						break;
					case SortBy.name:
					default:
						retVal=x.EmployeeName.CompareTo(y.EmployeeName);
						//Shove "vacant" tiles to the end.
						if(string.IsNullOrEmpty(x.EmployeeName) && !string.IsNullOrEmpty(y.EmployeeName)) {
							retVal=1;
						}
						if(!string.IsNullOrEmpty(x.EmployeeName) && string.IsNullOrEmpty(y.EmployeeName)) {
							retVal=-1;
						}
					break;
				}
				if(retVal==0) {//last name is primary tie breaker
					retVal=x.EmployeeName.CompareTo(y.EmployeeName);
					if(retVal==0) {//extension is seconary tie breaker
						retVal=x.Extension.CompareTo(y.Extension);
					}
				}
				//we got here so our sort was successful
				return retVal;
			}

			public enum SortBy {
				///<summary>0 - By Extension.</summary>
				ext,
				///<summary>1 - By EmployeeNum.</summary>
				empNum,
				///<summary>2 - By Name.</summary>
				name
			}
		}
	}


}