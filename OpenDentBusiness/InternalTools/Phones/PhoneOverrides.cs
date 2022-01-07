using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	/*
	///<summary></summary>
	public class PhoneOverrides{

		public static PhoneOverride GetPhoneOverride(long phoneOverrideNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PhoneOverride>(MethodBase.GetCurrentMethod(),phoneOverrideNum);
			}
			string command="SELECT * FROM phoneoverride WHERE PhoneOverrideNum="+POut.Long(phoneOverrideNum);
			List<PhoneOverride> list=SubmitAndFill(Db.GetTable(command));
			if(list.Count==0){
				return null;
			}
			return list[0];
		}

		///<summary>Could easily return null.</summary>
		public static PhoneOverride GetByExtAndEmp(int extension,long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PhoneOverride>(MethodBase.GetCurrentMethod(),extension,employeeNum);
			}
			string command="SELECT * FROM phoneoverride "
				+"WHERE Extension="+POut.Long(extension)+" "
				+"AND EmpCurrent="+POut.Long(employeeNum);
			List<PhoneOverride> list=SubmitAndFill(Db.GetTable(command));
			if(list.Count==0){
				return null;
			}
			return list[0];
		}

		private static List<PhoneOverride> SubmitAndFill(DataTable table){
			//No need to check RemotingRole; no call to db.
			List<PhoneOverride> list=new List<PhoneOverride>();
			PhoneOverride phoneCur;
			for(int i=0;i<table.Rows.Count;i++){
				phoneCur=new PhoneOverride();
				phoneCur.PhoneOverrideNum=PIn.Int   (table.Rows[0]["PhoneOverrideNum"].ToString());
				phoneCur.Extension       =PIn.Int   (table.Rows[0]["Extension"].ToString());
				phoneCur.EmpCurrent      =PIn.Int   (table.Rows[0]["EmpCurrent"].ToString());
				phoneCur.IsAvailable     =PIn.Bool  (table.Rows[0]["IsAvailable"].ToString());
				phoneCur.Explanation     =PIn.String(table.Rows[0]["Explanation"].ToString());
				list.Add(phoneCur);
			}
			return list;
		}

		///<summary></summary>
		public static long Insert(PhoneOverride phoneCur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				phoneCur.PhoneOverrideNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneCur);
				return phoneCur.PhoneOverrideNum;
			}
			if(PrefC.RandomKeys) {
				phoneCur.PhoneOverrideNum=ReplicationServers.GetKey("","PhoneOverrideNum");
			}
			string command="INSERT INTO phoneoverride (";
			if(PrefC.RandomKeys) {
				command+="PhoneOverrideNum,";
			}
			command+="Extension,EmpCurrent,IsAvailable,Explanation) VALUES(";
			if(PrefC.RandomKeys) {
				command+=POut.Long(phoneCur.PhoneOverrideNum)+", ";
			}
			command+=
				 POut.Long(phoneCur.Extension)+","
				+POut.Long(phoneCur.EmpCurrent)+","
				+POut.Bool(phoneCur.IsAvailable)+","
				+"'"+POut.String(phoneCur.Explanation)+"')";
			if(PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else{
				phoneCur.PhoneOverrideNum=Db.NonQ(command,true);
			}
			//Now do some work on the phone table to display the change just made.
			//First the new overridden extension
			if(phoneCur.IsAvailable){
				command="SELECT ClockStatus FROM employee WHERE EmployeeNum="+POut.Long(phoneCur.EmpCurrent);
				DataTable tableEmp=Db.GetTable(command);
				if(tableEmp.Rows.Count>0){
					ClockStatusEnum status=Phones.GetClockStatusFromEmp(tableEmp.Rows[0][0].ToString());
					Phones.SetPhoneStatus(status,phoneCur.Extension,phoneCur.EmpCurrent);
				}
				//incomplete: there is no check here for whether person is already clocked out when they set this.
				//but it's no big deal if their phone keeps ringing.
				//PhoneAsterisks.SetToDefaultRingGroups(phoneCur.Extension,phoneCur.EmpCurrent);
			}
			else{
				Phones.SetPhoneStatus(ClockStatusEnum.Unavailable,phoneCur.Extension,phoneCur.EmpCurrent);
				//PhoneAsterisks.SetRingGroups(phoneCur.Extension,AsteriskRingGroups.None);
			}
			//then the old extension for the emp.  But only if it's different.
			long defaultExtension=Employees.GetEmp(phoneCur.EmpCurrent).PhoneExt;
			if(defaultExtension > 0 && defaultExtension != phoneCur.Extension){
				Phones.SetPhoneStatus(ClockStatusEnum.None,(int)defaultExtension,0);//clear it out.
				//PhoneAsterisks.SetRingGroups((int)defaultExtension,AsteriskRingGroups.None);
			}
			return phoneCur.PhoneOverrideNum;
		}

		///<summary></summary>
		public static void Update(PhoneOverride phoneCur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneCur);
				return;
			}
			string command="UPDATE phoneoverride SET "
				+"Extension="+phoneCur.Extension.ToString()+","
				+"EmpCurrent="+phoneCur.EmpCurrent.ToString()+","
				+"IsAvailable="+POut.Bool(phoneCur.IsAvailable)+","
				+"Explanation='"+POut.String(phoneCur.Explanation)+"' "
				+"WHERE PhoneOverrideNum="+POut.Long(phoneCur.PhoneOverrideNum);
			Db.NonQ(command);
			//The only change that the UI allows is to change IsAvailable.  Extension and emp can't have changed.
			//Change the phone table.
			if(phoneCur.IsAvailable){
				command="SELECT ClockStatus FROM employee WHERE EmployeeNum="+POut.Long(phoneCur.EmpCurrent);
				DataTable tableEmp=Db.GetTable(command);
				if(tableEmp.Rows.Count>0){
					ClockStatusEnum status=Phones.GetClockStatusFromEmp(tableEmp.Rows[0][0].ToString());
					Phones.SetPhoneStatus(status,phoneCur.Extension,phoneCur.EmpCurrent);
				}
				//PhoneAsterisks.SetToDefaultRingGroups(phoneCur.Extension,phoneCur.EmpCurrent);
			}
			else{
				Phones.SetPhoneStatus(ClockStatusEnum.Unavailable,phoneCur.Extension,phoneCur.EmpCurrent);
				//PhoneAsterisks.SetRingGroups(phoneCur.Extension,AsteriskRingGroups.None);
			}
		}

		public static void Delete(PhoneOverride phoneCur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneCur);
				return;
			}
			string command="DELETE FROM phoneoverride WHERE PhoneOverrideNum="+POut.Long(phoneCur.PhoneOverrideNum);
			Db.NonQ(command);
			//Now do some work on the phone table to display the change just made.
			//First, reset the overridden extension
			long empNumOriginal=Employees.GetEmpNumAtExtension(phoneCur.Extension);//determine what employee belongs at that extension
			if(empNumOriginal>0){
				command="SELECT ClockStatus FROM employee WHERE EmployeeNum="+POut.Long(empNumOriginal);
				DataTable tableEmp=Db.GetTable(command);
				if(tableEmp.Rows.Count>0){
					ClockStatusEnum status=Phones.GetClockStatusFromEmp(tableEmp.Rows[0][0].ToString());
					Phones.SetPhoneStatus(status,phoneCur.Extension,empNumOriginal);
					//PhoneAsterisks.SetToDefaultRingGroups(phoneCur.Extension,phoneCur.EmpCurrent);
				}
			}
			else{
				//not sure what would happen here.  If no emp is assigned that extension by default, then, I guess clear it?
				Phones.SetPhoneStatus(ClockStatusEnum.None,phoneCur.Extension,0);
			}
			//then reset the default extension for the emp.  But only if it's different.
			long defaultExtension=Employees.GetEmp(phoneCur.EmpCurrent).PhoneExt;
			if(defaultExtension > 0 && defaultExtension != phoneCur.Extension){
				command="SELECT ClockStatus FROM employee WHERE EmployeeNum="+POut.Long(phoneCur.EmpCurrent);
				DataTable tableEmp=Db.GetTable(command);
				if(tableEmp.Rows.Count>0){
					ClockStatusEnum status=Phones.GetClockStatusFromEmp(tableEmp.Rows[0][0].ToString());
					Phones.SetPhoneStatus(status,(int)defaultExtension,phoneCur.EmpCurrent);
					//PhoneAsterisks.SetToDefaultRingGroups((int)defaultExtension,phoneCur.EmpCurrent);
				}
			}
		}

		///<summary>If an existing override changes the extension of an employee, then this just changes IsAvailable to true.  But if the existing override has no effect on the extension, then it just gets deleted.</summary>
		public static void SetAvailable(int extension,long empNum) {
			//No need to check RemotingRole; no call to db.
			PhoneOverride phoneOR=GetByExtAndEmp(extension,empNum);
			if(phoneOR==null){
				return;//no override exists.
			}
			Employee emp=Employees.GetEmp(empNum);
			//if(PhoneEmpDefaults.IsNoColor(phoneExclusionList,empNum)){
			//	phoneOR.IsAvailable=true;
			//	phoneOR.Explanation="";
			//	Update(phoneOR);
			//	return;
			//}
			if(emp.PhoneExt==extension){
				Delete(phoneOR);
				return;
			}
			//phone extension doesn't match:
			phoneOR.IsAvailable=true;
			phoneOR.Explanation="";
			Update(phoneOR);
		}

		public static DataTable GetAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM phoneoverride";
			return Db.GetTable(command);
		}


	}*/

}









