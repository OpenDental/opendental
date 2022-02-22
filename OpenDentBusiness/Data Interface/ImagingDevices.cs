using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{
	public class ImagingDevices{ 
		#region CachePattern
		private class ImagingDeviceCache : CacheListAbs<ImagingDevice> {
			protected override List<ImagingDevice> GetCacheFromDb() {
				string command="SELECT * FROM imagingdevice ORDER BY ItemOrder";
				return Crud.ImagingDeviceCrud.SelectMany(command);
			}
			protected override List<ImagingDevice> TableToList(DataTable table) {
				return Crud.ImagingDeviceCrud.TableToList(table);
			}
			protected override ImagingDevice Copy(ImagingDevice imagingDevice) {
				return imagingDevice.Copy();
			}
			protected override DataTable ListToTable(List<ImagingDevice> listImagingDevices) {
				return Crud.ImagingDeviceCrud.ListToTable(listImagingDevices,"ImagingDevice");
			}
			protected override void FillCacheIfNeeded() {
				ImagingDevices.GetTableFromCache(false);
			}
			protected override bool IsInListShort(ImagingDevice imagingDevice) {
				return true;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ImagingDeviceCache _imagingDeviceCache=new ImagingDeviceCache();

		public static List<ImagingDevice> GetDeepCopy(bool isShort=false) {
			return _imagingDeviceCache.GetDeepCopy(isShort);
		}

		public static bool GetExists(Predicate<ImagingDevice> match,bool isShort=false) {
			return _imagingDeviceCache.GetExists(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_imagingDeviceCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_imagingDeviceCache.FillCacheFromTable(table);
				return table;
			}
			return _imagingDeviceCache.GetTableFromCache(doRefreshCache);
		}
		#endregion

		///<summary></summary>
		public static void Update(ImagingDevice imagingDevice) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),imagingDevice);
				return;
			}
			Crud.ImagingDeviceCrud.Update(imagingDevice);
		}

		///<summary></summary>
		public static long Insert(ImagingDevice imagingDevice) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				imagingDevice.ImagingDeviceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),imagingDevice);
				return imagingDevice.ImagingDeviceNum;
			}
			return Crud.ImagingDeviceCrud.Insert(imagingDevice);
		}

		///<summary>No need to surround with try/catch, because all deletions are allowed.</summary>
		public static void Delete(long imagingDeviceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),imagingDeviceNum);
				return;
			}
			string command="DELETE FROM imagingdevice WHERE ImagingDeviceNum="+POut.Long(imagingDeviceNum);
			Db.NonQ(command);
		}

		



	}
}
