using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile {
	///<summary></summary>
	public class Appointmentms {

		#region Only used on webserver for mobile web.

			///<summary>Gets one Appointmentm from the db. </summary>
			public static Appointmentm GetOne(long customerNum,long aptNum) {
				return Crud.AppointmentmCrud.SelectOne(customerNum,aptNum);
			}

			///<summary>Gets Appointmentm from the db as specified by the date range and provider.</summary>
			public static List<Appointmentm> GetAppointmentms(long customerNum,long provNum,DateTime startDate,DateTime endDate) {
				string command=
					"SELECT * from appointmentm "
					+"WHERE AptDateTime BETWEEN '"+POut.Date(startDate,false)+"' AND '"+POut.Date(endDate.AddDays(1),false)+"' "
					+"AND CustomerNum = "+POut.Long(customerNum)+" "
					+"AND (ProvNum = "+POut.Long(provNum)+" "
					+"OR (IsHygiene = 1 AND ProvHyg = "+POut.Long(provNum)+")) " 
					+"ORDER BY AptDateTime";
				return Crud.AppointmentmCrud.SelectMany(command);
			}

			///<summary>Gets Appointmentm from the db as specified by the date range.</summary>
			public static List<Appointmentm> GetAppointmentms(long customerNum,DateTime startDate,DateTime endDate) {
				string command=
					"SELECT * from appointmentm "
					+"WHERE AptDateTime BETWEEN '"+POut.Date(startDate,false)+"' AND '"+POut.Date(endDate.AddDays(1),false)+"' "
					+"AND CustomerNum = "+POut.Long(customerNum)+" "
					+"ORDER BY AptDateTime";
				return Crud.AppointmentmCrud.SelectMany(command);
			}
			///<summary>Gets all Appointmentm for a single patient.</summary>
			public static List<Appointmentm> GetAppointmentms(long customerNum,long patNum) {
				string command=
					"SELECT * from appointmentm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
				return Crud.AppointmentmCrud.SelectMany(command);
			}
		#endregion

		#region Used only on OD 
			///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
			public static List<Appointmentm> ConvertListToM(List<Appointment> list) {
				List<Appointmentm> retVal=new List<Appointmentm>();
				for(int i=0;i<list.Count;i++) {
					retVal.Add(Crud.AppointmentmCrud.ConvertToM(list[i]));
				}
				return retVal;
			}

			///<summary>The values returned are sent to the webserver.</summary>
			public static List<long> GetChangedSinceAptNums(DateTime changedSince,DateTime excludeOlderThan) {
				return Appointments.GetChangedSinceAptNums(changedSince,excludeOlderThan);
			}

			///<summary>The values returned are sent to the webserver.</summary>
			public static List<Appointmentm> GetMultApts(List<long> aptNums) {
				List<Appointment> aptList=Appointments.GetMultApts(aptNums);
				List<Appointmentm> aptmList=ConvertListToM(aptList);
				return aptmList;
			}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
			///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
			public static void UpdateFromChangeList(List<Appointmentm> list,long customerNum) {
				for(int i=0;i<list.Count;i++) {
					list[i].CustomerNum=customerNum;
					Appointmentm appointmentm=Crud.AppointmentmCrud.SelectOne(customerNum,list[i].AptNum);
					if(appointmentm==null) {//not in db
						Crud.AppointmentmCrud.Insert(list[i],true);
					}
					else {
						Crud.AppointmentmCrud.Update(list[i]);
					}
				}
			}

			///<summary>used in tandem with Full synch</summary>
			public static void DeleteAll(long customerNum) {
				string command= "DELETE FROM appointmentm WHERE CustomerNum = "+POut.Long(customerNum); ;
				Db.NonQ(command);
			}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static long Insert(Appointmentm appointmentm) {
			return Crud.AppointmentmCrud.Insert(appointmentm,true);
		}
		
		///<summary></summary>
		public static void Update(Appointmentm appointmentm) {
			Crud.AppointmentmCrud.Update(appointmentm);
		}
		 
		///<summary></summary>
		public static void Delete(long customerNum,long aptNum) {
			string command= "DELETE FROM appointmentm WHERE CustomerNum = "+POut.Long(customerNum)+" AND AptNum = "+POut.Long(aptNum);
			Db.NonQ(command);
		}

		 
		///<summary></summary>
		public static List<Appointmentm> Refresh(long patNum){
			string command="SELECT * FROM appointmentm WHERE PatNum = "+POut.Long(patNum);
			return Crud.AppointmentmCrud.SelectMany(command);
		}

		
		
		*/


	}
}