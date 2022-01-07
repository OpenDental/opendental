using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class MobileAppDeviceT {
		///<summary>Append ClinicNum EG (0, 1, 100, whatever) to end of this for plain text password of MobileAppClinic.</summary>
		public const string MobileAppDeviceDevicePrefx="CheckinDevice_";
		///<summary>Append ClinicNum EG (0, 1, 100, whatever) to end of this for plain text password of MobileAppClinic.</summary>
		public const string MobileAppDeviceUniqueIdPrefix="CheckinUniqueId_";

		///<summary>Returns a predictable user name for this clinic, eClipboardUser_XXX where XXX = clinicNum.</summary>
		public static string GenerateDeviceName(long clinicNum) {
			return MobileAppDeviceDevicePrefx+clinicNum.ToString();
		}

		///<summary>Returns a predictable user name for this clinic, eClipboardPwd_XXX where XXX = clinicNum.</summary>
		public static string GenerateUniqueId(long clinicNum) {
			return MobileAppDeviceUniqueIdPrefix+clinicNum.ToString();
		}
		///<summary></summary>
		public static MobileAppDevice CreateMobileAppDevice(string deviceName,string uniqueID,long clinicNum,bool isAllowed) {
			return DataAction.GetPractice(() => {
				MobileAppDevice ret=new MobileAppDevice {
					DeviceName=deviceName,UniqueID=uniqueID,ClinicNum=clinicNum,IsAllowed=isAllowed,LastAttempt=DateTime.Now
				};
				OpenDentBusiness.Crud.MobileAppDeviceCrud.Insert(ret);
				return ret;
			});
		}

		///<summary>Deletes everything from the mobileappdevice table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearMobileAppDevice() {
			string command="DELETE FROM mobileappdevice";
			DataAction.RunPractice(() => DataCore.NonQ(command));
		}
	}
}
