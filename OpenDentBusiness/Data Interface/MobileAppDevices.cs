using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MobileAppDevices{
		#region Get Methods

		///<summary>Gets one MobileAppDevice from the database.</summary>
		public static MobileAppDevice GetOne(long mobileAppDeviceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MobileAppDevice>(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
			}
			return Crud.MobileAppDeviceCrud.SelectOne(mobileAppDeviceNum);
		}

		///<summary>Gets all MobileAppDevices from the database. If patNum is provided then filters by patNum. PatNum of 0 get unoccupied devices.</summary>
		public static List<MobileAppDevice> GetAll(long patNum=-1) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileAppDevice>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM mobileappdevice";
			if(patNum>-1) {
				command+=$" WHERE PatNum={POut.Long(patNum)}";
			}
			return Crud.MobileAppDeviceCrud.SelectMany(command);
		}

		public static List<MobileAppDevice> GetForUser(Userod user) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MobileAppDevice>>(MethodBase.GetCurrentMethod(),user);
			}
			string command=$"SELECT * FROM mobileappdevice ";
			if(PrefC.HasClinicsEnabled) {
				List<Clinic> listClinicsForUser=Clinics.GetForUserod(user);
				if(listClinicsForUser.Count==0) {
					return new List<MobileAppDevice>();
				}
				command+=$"WHERE ClinicNum in ({ string.Join(",",listClinicsForUser.Select(x => x.ClinicNum))})";
			}
			//Get valid BYOD devices and all other non BYOD MADs
			return Crud.MobileAppDeviceCrud.SelectMany(command).Where(x => !x.IsBYODDevice || (x.IsBYODDevice && x.PatNum!=0)).ToList();
		}

		#endregion Get Methods

		#region Update

		public static void Update(MobileAppDevice device) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),device);
				return;
			}
			Crud.MobileAppDeviceCrud.Update(device);			
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary>Keeps MobileAppDevice table current so we know which patient is on which device and for how long.</summary>
		public static void SetPatNum(long mobileAppDeviceNum,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceNum,patNum);
				return;
			}
			string command;
			if(patNum==-1) {
				if((GetOne(mobileAppDeviceNum)??new MobileAppDevice { IsBYODDevice=false }).IsBYODDevice) {
					Crud.MobileAppDeviceCrud.Delete(mobileAppDeviceNum);
					Signalods.SetInvalid(InvalidType.EClipboard);
				}
				else {
					command="UPDATE mobileappdevice SET PatNum="+POut.Long(0)+",LastCheckInActivity="+POut.DateT(DateTime.Now)
					+" WHERE MobileAppDeviceNum="+POut.Long(mobileAppDeviceNum);
					Db.NonQ(command);
					Signalods.SetInvalid(InvalidType.EClipboard);
				}
			}
			else {
				command="UPDATE mobileappdevice SET PatNum="+POut.Long(patNum)+",LastCheckInActivity="+POut.DateT(DateTime.Now)
				+" WHERE MobileAppDeviceNum="+POut.Long(mobileAppDeviceNum);
				Db.NonQ(command);
				Signalods.SetInvalid(InvalidType.EClipboard);
			}
		}

		///<summary>Syncs the two lists in the database.</summary>
		public static void Sync(List<MobileAppDevice> listDevicesNew,List<MobileAppDevice> listDevicesDb) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDevicesNew,listDevicesDb);
				return;
			}
			if(Crud.MobileAppDeviceCrud.Sync(listDevicesNew,listDevicesDb)) {
				Signalods.SetInvalid(InvalidType.EClipboard);
			}
		}

		#endregion Update

		#region Delete

		public static void Delete(long mobileAppDeviceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
				return;
			}
			WebTypes.PushNotificationUtils.CI_IsAllowedChanged(mobileAppDeviceNum,false); //deleting so always false
			Crud.MobileAppDeviceCrud.Delete(mobileAppDeviceNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		#endregion Delete

		///<summary>For any device whose clinic num is not in the list passed in, sets IsAllowed to false.</summary>
		public static void UpdateIsAllowed(List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClinicNums);
				return;
			}
			string command="UPDATE mobileappdevice SET IsAllowed=0";
			if(!listClinicNums.IsNullOrEmpty()) {
				command+=" WHERE ClinicNum NOT IN("+string.Join(",",listClinicNums)+")";
			}
			Db.NonQ(command);
		}

		///<summary>Returns true if this PatNum is currently linked to a MobileAppDevice row.</summary>
		public static bool PatientIsAlreadyUsingDevice(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT COUNT(*) FROM mobileappdevice WHERE PatNum={POut.Long(patNum)}";
			return PIn.Long(Db.GetCount(command))>0;
		}

		///<summary>Returns true if this clinicNum is subscribed for eClipboard.</summary>
		public static bool IsClinicSignedUpForEClipboard(long clinicNum) {
			//No remoting role check needed.
			return PrefC.GetString(PrefName.EClipboardClinicsSignedUp)
				.Split(',')
				.Where(x => x!="")
				.Select(x => PIn.Long(x))
				.Any(x => x==clinicNum);
		}

		///<summary>Returns true if this clinicNum is subscribed for MobileWeb.</summary>
		public static bool IsClinicSignedUpForMobileWeb(long clinicNum) {
			//No remoting role check needed.
			return GetClinicSignedUpForMobileWeb().Any(x => x==clinicNum);
		}

		///<summary>Returns list of ClinicNum(s) which are currently subscribed for MobileWeb. 
		///Will include ClinicNum 0 if not using clinics and practice is enabled.</summary>
		public static List<long> GetClinicSignedUpForMobileWeb() {
			//No remoting role check needed.
			return PrefC.GetString(PrefName.MobileWebClinicsSignedUp)
				.Split(',')
				.Where(x => x!="")
				.Select(x => PIn.Long(x)).ToList();
		}
	}
}