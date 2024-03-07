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
		public const string MobileAppDevicePayloadTag="MADPayload";
		#region Get Methods

		///<summary>Gets one MobileAppDevice from the database.</summary>
		public static MobileAppDevice GetOne(long mobileAppDeviceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MobileAppDevice>(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
			}
			return Crud.MobileAppDeviceCrud.SelectOne(mobileAppDeviceNum);
		}

		///<summary>Gets all MobileAppDevices from the database. If patNum is provided then filters by patNum. PatNum of 0 get unoccupied devices.</summary>
		public static List<MobileAppDevice> GetAll(long patNum=-1) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MobileAppDevice>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM mobileappdevice";
			if(patNum>-1) {
				command+=$" WHERE PatNum={POut.Long(patNum)}";
			}
			return Crud.MobileAppDeviceCrud.SelectMany(command);
		}

		public static List<MobileAppDevice> GetForUser(Userod user,bool doIncludeHQ=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MobileAppDevice>>(MethodBase.GetCurrentMethod(),user,doIncludeHQ);
			}
			string command=$"SELECT * FROM mobileappdevice ";
			if(PrefC.HasClinicsEnabled) {
				List<Clinic> listClinicsForUser=Clinics.GetForUserod(user,doIncludeHQ);
				if(listClinicsForUser.Count==0) {
					return new List<MobileAppDevice>();
				}
				command+=$"WHERE ClinicNum in ({ string.Join(",",listClinicsForUser.Select(x => x.ClinicNum))})";
			}
			//Get valid BYOD devices and all other non BYOD MADs
			return Crud.MobileAppDeviceCrud.SelectMany(command).Where(x => !x.IsBYODDevice || (x.IsBYODDevice && x.PatNum!=0)).ToList();
		}

		public static MobileAppDevice GetForPat(long patNum,MADPage page=MADPage.Undefined) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MobileAppDevice>(MethodBase.GetCurrentMethod(),patNum,page);
			}
			string command=$"SELECT * FROM mobileappdevice WHERE PatNum={POut.Long(patNum)} ";
			if(page!=MADPage.Undefined) {
				command=$"AND DevicePage={POut.Enum<MADPage>(page)} ";
			}
			return Crud.MobileAppDeviceCrud.SelectOne(command);
		}

		public static List<MobileAppDevice> GetForClinic(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MobileAppDevice>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command=$"SELECT * FROM mobileappdevice WHERE ClinicNum={POut.Long(clinicNum)}";
			return Crud.MobileAppDeviceCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Update

		public static void Update(MobileAppDevice device) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),device);
				return;
			}
			Crud.MobileAppDeviceCrud.Update(device);			
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		public static void Update(MobileAppDevice mobileAppDeviceNew,MobileAppDevice mobileAppDeviceOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceNew,mobileAppDeviceOld);
				return;
			}
			Crud.MobileAppDeviceCrud.Update(mobileAppDeviceNew,mobileAppDeviceOld);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary>Keeps MobileAppDevice table current so we know which patient is on which device and for how long.</summary>
		public static void SetPatNum(long mobileAppDeviceNum,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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

		///<summary>Keeps MobileAppDevice table current so we know which patient is on which device and for how long.</summary>
		public static void SetUserNum(string mobileAppDeviceID, long userNum) {
			if(RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(), mobileAppDeviceID, userNum);
				return;
			}
			string command;
				command = $"UPDATE mobileappdevice SET UserNum={POut.Long(userNum)},LastCheckInActivity={POut.DateT(DateTime.Now)}"
				+ $" WHERE UniqueID='{POut.String(mobileAppDeviceID)}'";
				Db.NonQ(command);
				Signalods.SetInvalid(InvalidType.EClipboard);
			}

		///<summary>Sets the page state for the passed in MAD. </summary>
		public static void SetPageName(string mobileAppDeviceUniqueID,MADPage pageName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceUniqueID,pageName);
				return;
			}
			string command;
			command=$"UPDATE mobileappdevice SET DevicePage={POut.Int((int)pageName)} "
			+$"WHERE UniqueID=\"{POut.String(mobileAppDeviceUniqueID)}\" ";
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		///<summary>Syncs the two lists in the database.</summary>
		public static void Sync(List<MobileAppDevice> listDevicesNew,List<MobileAppDevice> listDevicesDb) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
				return;
			}
			MobileNotifications.CI_IsAllowedChanged(mobileAppDeviceNum,false);//deleting so always false
			Crud.MobileAppDeviceCrud.Delete(mobileAppDeviceNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		public static void DeleteMany(List<MobileAppDevice> listMobileAppDevices) {
			if(listMobileAppDevices.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMobileAppDevices);
				return;
			}
			for(int i=0;i<listMobileAppDevices.Count;i++) {
				if(listMobileAppDevices[i].IsEclipboardEnabled) {
					MobileNotifications.CI_IsAllowedChanged(listMobileAppDevices[i].MobileAppDeviceNum,false); //deleting so always false
				}
			}
			Crud.MobileAppDeviceCrud.DeleteMany(listMobileAppDevices.Select(x => x.MobileAppDeviceNum).ToList());
			Signalods.SetInvalid(InvalidType.EClipboard);
		}

		#endregion Delete

		///<summary>For any device whose clinic num is not in the list passed in, sets IsAllowed to false.</summary>
		public static void UpdateIsEClipboardAllowed(List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClinicNums);
				return;
			}
			string command="UPDATE mobileappdevice SET IsEclipboardEnabled=0";
			if(!listClinicNums.IsNullOrEmpty()) {
				command+=" WHERE ClinicNum NOT IN("+string.Join(",",listClinicNums)+")";
			}
			Db.NonQ(command);
		}

		///<summary>Returns true if this PatNum is currently linked to a MobileAppDevice row.</summary>
		public static bool PatientIsAlreadyUsingDevice(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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

		///<summary>Returns true if the primary key for the given keytype is set for a device, false otherwise.</summary>
		public static bool IsInUse(long patNum=0,params MADPage[] devicePage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum,devicePage);
			}
			string command=$"SELECT COUNT(*) FROM mobileappdevice WHERE DevicePage IN ({string.Join(",",devicePage.Select(x=> POut.Enum<MADPage>(x)).ToList())})";
			if(patNum>0) {
				command+=$"AND PatNum={POut.Long(patNum)} ";
			}
			return PIn.Long(Db.GetCount(command))>0;
		}

		/// <summary>Gets the most recently used device that has the passed in patient set. If UserNum is set or device does not exist, returns false, should not create mboile
		/// notification. If no user is set or device row does not exist, create mobile notification. Also fills in passed in MobileAppDevice object so that the caller does not
		/// have to do the same code to get it. Centralizes all of the code needed to answer this question and provides the needed data to take action (ie. creating a mobile
		/// notification for a given device). When result is true we know that the out paramater device is not null, so it is safe to use. If not creating a mobile notification
		/// no need for the device.</summary>
		public static bool ShouldCreateMobileNotification(long patNum, out MobileAppDevice device) {
			List<MobileAppDevice> listMobileAppDevices = MobileAppDevices.GetAll(patNum);
			device = listMobileAppDevices.OrderByDescending(x => x.LastCheckInActivity).FirstOrDefault();
			if(device != null && device.LastCheckInActivity>DateTime.Now.AddHours(-1) && device.UserNum == 0) {
				return true;
			}
			return false;
		}


	}
}