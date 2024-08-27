using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenDentBusiness;

namespace OpenDental {
	public class PhoneUI {
		private static string langThis="Phone";		

		public static void Manage(PhoneTile tile){
			Manage(tile.PhoneCur);
		}

		public static void Manage(Phone phone){
			//if(selectedTile.PhoneCur==null) {//already validated
			long patNum=phone.PatNum;
			if(patNum==0) {
				MsgBox.Show(langThis,"Please attach this number to a patient first.");
				return;
			}
			using FormPhoneNumbersManage FormM=new FormPhoneNumbersManage();
			FormM.PatNum=patNum;
			FormM.ShowDialog();
		}

		public static void Add(PhoneTile tile){
			Add(tile.PhoneCur);
		}

		public static void Add(Phone phone){
			//if(selectedTile.PhoneCur==null) {//already validated
			if(phone.CustomerNumber=="") {
				MsgBox.Show(langThis,"No phone number present.");
				return;
			}
			long patNum=phone.PatNum;
			if(FormOpenDental.PatNumCur==0) {
				MsgBox.Show(langThis,"Please select a patient in the main window first.");
				return;
			}
			if(patNum!=0) {
				MsgBox.Show(langThis,"The current number is already attached to a different customer.");
				return;
				//if(!MsgBox.Show(langThis,MsgBoxButtons.OKCancel,"The current number is already attached to a patient. Attach it to this patient instead?")) {
				//	return;
				//}
				//This crashes because we don't actually know what the number is.  Enhance later by storing actual number in phone grid.
				//PhoneNumber ph=PhoneNumbers.GetByVal(PhoneCur.CustomerNumber);
				//ph.PatNum=FormOpenDental.CurPatNum;
				//PhoneNumbers.Update(ph);
			}
			else {
				string patName=Patients.GetLim(FormOpenDental.PatNumCur).GetNameLF();
				if(MessageBox.Show("Attach this phone number to "+patName+"?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
				PhoneNumber phoneNumber=new PhoneNumber();
				phoneNumber.PatNum=FormOpenDental.PatNumCur;
				phoneNumber.PhoneNumberVal=phone.CustomerNumber;
				PhoneNumbers.Insert(phoneNumber);
			}
			//tell the phone server to refresh this row with the patient name and patnum
			DataValid.SetInvalid(InvalidType.PhoneNumbers);
		}

		///<summary>If this is Security.CurUser's tile then ClockIn. If it is someone else's tile then allow the single case of switching from NeedsHelp to Available.</summary>
		public static void Available(PhoneTile phoneTile,bool isAtHome) {
			Available(phoneTile.PhoneCur,isAtHome);
		}

		public static void Available(Phone phone,bool isAtHome=false) {
			long employeeNum=Security.CurUser.EmployeeNum;
			if(Security.CurUser.EmployeeNum!=phone.EmployeeNum) { //We are on someone else's tile. So Let's do some checks before we assume we can take over this extension.
				if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { 
					//Allow the specific state where we are changing their status back from NeedsHelp to Available.
					//This does not require any security permissions as any tech in can perform this action on behalf of any other tech.
					Phones.SetPhoneStatus(ClockStatusEnum.Available,phone.Extension,phone.EmployeeNum);//green
					return;
				}
				//We are on a tile that is not our own
				//If another employee is occupying this extension then assume we are trying to change that employee's status back to available.
				if(ClockEvents.IsClockedIn(phone.EmployeeNum)) { //This tile is taken by an employee who is clocked in.					
					//Transition the employee back to available.
					ChangeTileStatus(phone,ClockStatusEnum.Available);
					PhoneAsterisks.SetToDefaultQueue(phone.EmployeeNum);
					return;
				}
				if(phone.ClockStatus!=ClockStatusEnum.None
					&& phone.ClockStatus!=ClockStatusEnum.Home) 
				{
					//Another person is still actively using this extension.
					MsgBox.Show(langThis,"Cannot take over this extension as it is currently occuppied by someone who is likely on Break or Lunch.");			
					return;
				}			
				//If another employee is NOT occupying this extension then assume we are trying clock in at this extension.
				if(ClockEvents.IsClockedIn(employeeNum)) { //We are already clocked in at a different extension.
					MsgBox.Show(langThis,"You are already clocked in at a different extension.  You must clock out of the current extension you are logged into before moving to another extension.");
					return;
				}
				//We got this far so fall through and allow user to clock in.
			}
			//We go here so all of our checks passed and we may login at this extension
			if(!ClockIn(isAtHome)) { //Clock in on behalf of yourself
				return;
			}
			//Update the Phone tables accordingly.
			PhoneEmpDefaults.SetAvailable(phone.Extension,employeeNum);
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			PhoneAsterisks.SetToDefaultQueue(phone.EmployeeNum);
			Phones.SetPhoneStatus(ClockStatusEnum.Available,phone.Extension,employeeNum);//green
		}

		public static void Training(PhoneTile tile) {
			Training(tile.PhoneCur);
		}

		public static void Training(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.Training);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void TeamAssist(PhoneTile tile) {
			TeamAssist(tile.PhoneCur);
		}

		public static void TeamAssist(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.TeamAssist);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void NeedsHelp(PhoneTile tile) {
			NeedsHelp(tile.PhoneCur);
		}

		public static void NeedsHelp(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.NeedsHelp);
			//Do not manipulate the queue for this extension on purpose.
		}

		public static void WrapUp(PhoneTile tile) { //this is usually an automatic status
			WrapUp(tile.PhoneCur);
		}

		public static void WrapUp(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.WrapUp);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void OfflineAssist(PhoneTile tile) {
			OfflineAssist(tile.PhoneCur);
		}

		public static void OfflineAssist(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.OfflineAssist);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void TCResponder(PhoneTile tile) {
			TCResponder(tile.PhoneCur);
		}

		public static void TCResponder(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.TCResponder);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		//Queue - None
		public static void Unavailable(PhoneTile tile) {
			Unavailable(tile.PhoneCur);
		}

		public static void Unavailable(Phone phone) {
			if(!ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) { //Employee performing the action must be clocked in.
				MsgBox.Show("PhoneUI","You must clock in before completing this action.");
				return;
			}
			if(!ClockEvents.IsClockedIn(phone.EmployeeNum)) { //Employee having action performed must be clocked in.
				MessageBox.Show(Lan.g("PhoneUI","Target employee must be clocked in before setting this status:")+" "+phone.EmployeeName);
				return;
			}
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			int extension=phone.Extension;
			long employeeNum=phone.EmployeeNum;
			PhoneEmpDefault ped=PhoneEmpDefaults.GetByExtAndEmp(extension,employeeNum);
			if(ped==null) {
				MessageBox.Show("PhoneEmpDefault row not found for Extension "+extension.ToString()+" and EmployeeNum "+employeeNum.ToString());
				return;
			}
			using FormPhoneEmpDefaultEdit formPED=new FormPhoneEmpDefaultEdit();
			formPED.PedCur=ped;
			formPED.PedCur.StatusOverride=PhoneEmpStatusOverride.Unavailable;
			formPED.ShowDialog();//Phone Status and Queue get updated inside of the form when status is set to Unavailable.
		}

		///<summary></summary>
		public static void Backup(PhoneTile tile) {
			Backup(tile.PhoneCur);
		}

		public static void Backup(Phone phone) {
			ChangeTileStatus(phone,ClockStatusEnum.Backup);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.Backup);
		}

		public static void ShowEmployeeSettings(PhoneTile tile) {
			ShowEmployeeSettings(tile.PhoneCur);
		}

		public static void ShowEmployeeSettings(Phone phone) {
			if(phone==null || phone.EmployeeNum < 1) {
				return;
			}
			PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetByExtAndEmp(phone.Extension,phone.EmployeeNum);
			if(phoneEmpDefault==null) {
				MsgBox.Show("Could not find the selected EmployeeNum/Extension pair in database. Please verify that the correct extension is listed for this user in map setup.");
				return;
			}
			using FormPhoneEmpDefaultEdit formPhoneEmpDefaultEdit=new FormPhoneEmpDefaultEdit();
			formPhoneEmpDefaultEdit.PedCur=phoneEmpDefault;
			formPhoneEmpDefaultEdit.ShowDialog();
		}

		//Queues---------------------------------------------------

		public static void QueueTech(PhoneTile tile) {
			QueueTech(tile.PhoneCur);
		}

		public static void QueueTech(Phone phone) {
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.Tech);
		}

		public static void QueueNone(PhoneTile tile) {
			QueueNone(tile.PhoneCur);
		}

		public static void QueueNone(Phone phone) {
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void QueueDefault(PhoneTile tile) {
			QueueDefault(tile.PhoneCur);
		}

		public static void QueueDefault(Phone phone) {
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			PhoneAsterisks.SetToDefaultQueue(phone.EmployeeNum);
		}

		///<summary></summary>
		public static void QueueBackup(PhoneTile tile) {
			QueueBackup(tile.PhoneCur);
		}

		public static void QueueBackup(Phone phone) {
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.Backup);
		}

		//Timecard---------------------------------------------------

		public static void Lunch(PhoneTile tile) {
			Lunch(tile.PhoneCur);
		}

		public static void Lunch(Phone phone) {
			//verify that employee is logged in as user
			int extension=phone.Extension;
			long employeeNum=phone.EmployeeNum;
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				ClockEvents.ClockOut(employeeNum,TimeClockStatus.Lunch);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock in again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(langThis,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//This message will tell user that they are already clocked out.
				return;
			}
			PhoneEmpDefaults.SetAvailable(extension,employeeNum);
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Employee EmpCur=Employees.GetEmpFromDB(employeeNum);
			Employee EmpOld=EmpCur.Copy();
			EmpCur.ClockStatus=Lan.g("enumTimeClockStatus",TimeClockStatus.Lunch.ToString());
			Employees.UpdateChanged(EmpCur,EmpOld, true);
			Phones.SetPhoneStatus(ClockStatusEnum.Lunch,extension);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void Home(PhoneTile tile) {
			Home(tile.PhoneCur);
		}

		public static void Home(Phone phone) {
			//verify that employee is logged in as user
			int extension=phone.Extension;
			long employeeNum=phone.EmployeeNum;
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				ClockEvents.ClockOut(employeeNum,TimeClockStatus.Home);//Update the clock event, phone (HQ only), and phone emp default (HQ only).
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock in again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(langThis,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//This message will tell user that they are already clocked out.
				return;
			}
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Employee EmpCur=Employees.GetEmpFromDB(employeeNum);
			Employee EmpOld=EmpCur.Copy();
			EmpCur.ClockStatus=Lan.g("enumTimeClockStatus",TimeClockStatus.Home.ToString());
			Employees.UpdateChanged(EmpCur,EmpOld, true);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		public static void Break(PhoneTile tile) {
			Break(tile.PhoneCur);
		}

		public static void Break(Phone phone) {
			//verify that employee is logged in as user
			int extension=phone.Extension;
			long employeeNum=phone.EmployeeNum;
			if(!CheckUserCanChangeStatus(phone)) {
				return;
			}
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				ClockEvents.ClockOut(employeeNum,TimeClockStatus.Break);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock in again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(langThis,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//This message will tell user that they are already clocked out.
				return;
			}
			PhoneEmpDefaults.SetAvailable(extension,employeeNum);
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Employee EmpCur=Employees.GetEmpFromDB(employeeNum);
			Employee EmpOld=EmpCur.Copy();
			EmpCur.ClockStatus=Lan.g("enumTimeClockStatus",TimeClockStatus.Break.ToString());
			Employees.UpdateChanged(EmpCur,EmpOld, true);
			Phones.SetPhoneStatus(ClockStatusEnum.Break,extension);
			PhoneAsterisks.SetQueueForExtension(phone.Extension,AsteriskQueues.None);
		}

		//ChanSpy---------------------------------------------------
		public static void ListenToCall(int phoneExtSpy, int phoneExtBeingSpied) {
			string msgValue=StartChanSpy(phoneExtSpy,phoneExtBeingSpied);
			Signalod signalOD=new Signalod();
			signalOD.IType=InvalidType.PhoneAsteriskReload;
			signalOD.FKeyType=KeyType.ChanSpy;
			signalOD.MsgValue=msgValue;
			Signalods.Insert(signalOD);
			SecurityLogs.MakeLogEntry(EnumPermType.ChanSpy,0,"Current user extension: "+POut.Int(phoneExtSpy)+", extension of user listened to: "+POut.Int(phoneExtBeingSpied)+", ChanSpy mode used: "+ChanSpyModesEnum.Listen.ToString()+".");
		}

		private static string StartChanSpy(int phoneExtSpy, int phoneExtBeingSpied) {
			ChanSpy chanSpy=new ChanSpy();
			chanSpy.ExtensionSpy=phoneExtSpy;
			chanSpy.ExtensionBeingSpied=phoneExtBeingSpied;
			chanSpy.SpyMode=ChanSpyModesEnum.Listen;
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(ChanSpy));
			using StringWriter stringWriter=new StringWriter();
			using XmlWriter xlmWriter=XmlWriter.Create(stringWriter,new XmlWriterSettings { Indent=false,NewLineHandling=NewLineHandling.None });
			xmlSerializer.Serialize(xlmWriter,chanSpy);
			return stringWriter.ToString();
		}

		public static void BuildMenuStatus(ContextMenuStrip menuStatus,Phone phoneCur) {
			//Jason - Allowed to be 0 here.  The Security.UserCur.EmpNum will be used when they go to clock in and that is where the 0 check needs to be.
			//if(phoneCur.EmployeeNum==0) {
			//  return;
			//}
			bool allowStatusEdit=ClockEvents.IsClockedIn(phoneCur.EmployeeNum);
			if(phoneCur.EmployeeNum==Security.CurUser.EmployeeNum) { //Always allow status edit for yourself
				allowStatusEdit=true;
			}
			if(phoneCur.ClockStatus==ClockStatusEnum.NeedsHelp) { //Always allow any employee to change any other employee from NeedsAssistance to Available
				allowStatusEdit=true;
			}
			string statusOnBehalfOf=phoneCur.EmployeeName;
			bool allowSetSelfAvailable=false;
			if(!ClockEvents.IsClockedIn(phoneCur.EmployeeNum) //No one is clocked in at this extension.
				&& !ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) //This user is not clocked in either.
			{ 
				//Vacant extension and this user is not clocked in so allow this user to clock in at this extension.
				statusOnBehalfOf=Security.CurUser.UserName;
				allowSetSelfAvailable=true;
			}
			AddToolstripGroup(menuStatus,"menuItemStatusOnBehalf","Status for: "+statusOnBehalfOf);
			AddToolstripGroup(menuStatus,"menuItemRingGroupOnBehalf","Queues for ext: "+phoneCur.Extension.ToString());
			AddToolstripGroup(menuStatus,"menuItemClockOnBehalf","Clock event for: "+phoneCur.EmployeeName);
			SetToolstripItemText(menuStatus,"menuItemAvailable",allowStatusEdit || allowSetSelfAvailable);
			SetToolstripItemText(menuStatus,"menuItemTraining",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemTeamAssist",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemNeedsHelp",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemWrapUp",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemOfflineAssist",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemUnavailable",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemTCResponder",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemBackup",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemLunch",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemHome",allowStatusEdit);
			SetToolstripItemText(menuStatus,"menuItemBreak",allowStatusEdit);
		}

		private static void AddToolstripGroup(ContextMenuStrip menuStatus,string groupName,string itemText) {
			ToolStripItem[] tsiFound=menuStatus.Items.Find(groupName,false);
			if(tsiFound==null || tsiFound.Length<=0) {
				return;
			}
			tsiFound[0].Text=itemText;
		}

		private static void SetToolstripItemText(ContextMenuStrip menuStatus,string toolStripItemName,bool isClockedIn) {
			ToolStripItem[] tsiFound=menuStatus.Items.Find(toolStripItemName,false);
			if(tsiFound==null || tsiFound.Length<=0) {
				return;
			}
			//set back to default
			tsiFound[0].Text=tsiFound[0].Text.Replace(" (Not Clocked In)","");
			if(isClockedIn) {
				tsiFound[0].Enabled=true;				
			}
			else {
				tsiFound[0].Enabled=false;
				tsiFound[0].Text=tsiFound[0].Text+" (Not Clocked In)";
			}			
		}

		///<summary>If already clocked in, this does nothing unless you are clocking in to a different location, home/office.  Returns false if not able to clock in due to security, or true if successful.
		///This method used to take a PhoneTile, but it wasn't being used so it was removed.  If this is needed again someone can bring it back.</summary>
		private static bool ClockIn(bool isAtHome) {
			long employeeNum=Security.CurUser.EmployeeNum;//tile.PhoneCur.EmployeeNum;
			if(employeeNum==0) {//Can happen if logged in as 'admin' user (employeeNum==0). Otherwise should not happen, means the employee trying to clock doesn't exist in the employee table.
				MsgBox.Show(langThis,"Invalid OD User: "+Security.CurUser.UserName);
				return false;
			}
			ClockEvent clockEventLast=ClockEvents.GetLastEvent(employeeNum);
			//If already clocked in, but switching locations between, available at home / available at office, To create a new clock event.
			if(ClockEvents.IsClockedIn(employeeNum) && clockEventLast.IsWorkingHome!=isAtHome) {
				ClockEvents.ClockOut(employeeNum,TimeClockStatus.Home); //clocking out for home to complete clock event.	
				//Needed to maintain that the end of the first clockevent, and the start of the second clockevent doesnt happen at the same second.
				//Otherwise it will break the calc daily functionality and cause the wrong employe status.
				System.Threading.Thread.Sleep(1000); 
			}
			//If already clocked in and not switching locations.
			else if(ClockEvents.IsClockedIn(employeeNum)) {
			  return true;
			}
			//We no longer need to check passwords here because the user HAS to be logged in and physically sitting at the computer.
			/*if(Security.CurUser.EmployeeNum!=employeeNum) {
				if(!Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
					if(!CheckSelectedUserPassword(employeeNum)) {
						return false;
					}
				}
			}*/
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				ClockEvents.ClockIn(employeeNum,isAtHome);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(langThis,"Processing clock event...");
			try {
				progressOD.ShowDialog();
			}
			catch(Exception ex) {
				if(ex.Message.Contains("Already clocked in")) {
					return true;
				}
				MessageBox.Show(ex.Message);
				return false;
			}
			Employee EmpCur=Employees.GetEmpFromDB(employeeNum);
			Employee EmpOld=EmpCur.Copy();
			EmpCur.ClockStatus="Working";
			Employees.UpdateChanged(EmpCur,EmpOld, true);
			return true;
		}

		///<summary>Will ask for password if the current user logged in isn't the user status being manipulated.</summary>
		private static bool CheckSelectedUserPassword(long employeeNum) {
			if(Security.CurUser.EmployeeNum==employeeNum) {
				return true;
			}
			Userod selectedUser=Userods.GetUserByEmployeeNum(employeeNum);
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.TextBox;
			inputBoxParam.LabelText="Please enter password";
			inputBoxParam.IsPassswordCharStar=true;
			InputBox inputBox=new InputBox(inputBoxParam);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return false;
			}
			if(!Authentication.CheckPassword(selectedUser,inputBox.StringResult)) { 
				MsgBox.Show("PhoneUI","Wrong password.");
				return false;
			}
			return true;
		}

		///<summary>Verify... 
		///1) Security.CurUser is clocked in. 
		///2) Target status change employee is clocked in. 
		///3) Security.CurUser has TimecardsEditAll permission.</summary>
		private static bool ChangeTileStatus(Phone phoneCur,ClockStatusEnum newClockStatus) {
			if(!ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) { //Employee performing the action must be clocked in.
				MsgBox.Show(langThis,"You must clock in before completing this action.");
				return false;
			}
			if(!ClockEvents.IsClockedIn(phoneCur.EmployeeNum)) { //Employee having action performed must be clocked in.
				MessageBox.Show(Lan.g(langThis,"Target employee must be clocked in before setting this status:")+" "+phoneCur.EmployeeName);
				return false;
			}
			if(!CheckUserCanChangeStatus(phoneCur)) {
				return false;
			}
			PhoneEmpDefaults.SetAvailable(phoneCur.Extension,phoneCur.EmployeeNum);
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Phones.SetPhoneStatus(newClockStatus,phoneCur.Extension);
			return true;
		}

		///<summary>Verify Security.CurUser is allowed to change this tile's status.</summary>
		private static bool CheckUserCanChangeStatus(PhoneTile tile) {
			return CheckUserCanChangeStatus(tile.PhoneCur);
		}

		///<summary>Verify Security.CurUser is allowed to change this tile's status.</summary>
		private static bool CheckUserCanChangeStatus(Phone phoneCur) {
			if(phoneCur==null) {
				return false;
			}
			if(Security.CurUser.EmployeeNum==phoneCur.EmployeeNum) { //User is changing their own tile. This is always allowed.
				return true;
			}
			if(Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)) { //User has time card edit permission so allow it.
				return true;
			}
			//User must enter target tile's password correctly.
			return CheckSelectedUserPassword(phoneCur.EmployeeNum);
		}

		public class ChanSpy {
			public int ExtensionSpy;
			public int ExtensionBeingSpied;
			public ChanSpyModesEnum SpyMode;
		}

		public enum ChanSpyModesEnum {
			///<summary>Spy hears both operator and customer, but spy cannot be heard.</summary>
			Listen,
			///<summary>Spy hears both operator and customer, only the operator hears the spy.</summary>
			Whisper,
			///<summary>Spy hears both operator and customer, and both operator and customer hear the spy.</summary>
			Barge,
		}



	}
}
