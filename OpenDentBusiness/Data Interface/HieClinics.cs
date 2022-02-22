using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HieClinics{
		#region Methods - Get
		///<summary>Returns all hieclinics.</summary>
		public static List<HieClinic> Refresh(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HieClinic>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hieclinic";
			return Crud.HieClinicCrud.SelectMany(command);
		}
		
		///<summary>Gets one HieClinic from the db.</summary>
		public static HieClinic GetOne(long hieClinicNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<HieClinic>(MethodBase.GetCurrentMethod(),hieClinicNum);
			}
			return Crud.HieClinicCrud.SelectOne(hieClinicNum);
		}

		///<summary>Returns a list where PathExportCCD is not blank and IsEnabled is true.</summary>
		public static List<HieClinic> GetAllEnabled() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HieClinic>>(MethodBase.GetCurrentMethod());
			}
			string command=$"SELECT * FROM hieclinic WHERE PathExportCCD!='' AND IsEnabled=1";
			return Crud.HieClinicCrud.SelectMany(command);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(HieClinic hieClinic){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				hieClinic.HieClinicNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hieClinic);
				return hieClinic.HieClinicNum;
			}
			return Crud.HieClinicCrud.Insert(hieClinic);
		}

		///<summary></summary>
		public static void Update(HieClinic hieClinic){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hieClinic);
				return;
			}
			Crud.HieClinicCrud.Update(hieClinic);
		}

		///<summary></summary>
		public static void Delete(long hieClinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hieClinicNum);
				return;
			}
			Crud.HieClinicCrud.Delete(hieClinicNum);
		}

		///<summary>Syncs the passed in list of hieclinics with all of the hieclinics from the database.</summary>
		public static bool Sync(List<HieClinic> listHieClinics) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listHieClinics);
			}
			List<HieClinic> listHieClinicsDb=Refresh();
			return Crud.HieClinicCrud.Sync(listHieClinics,listHieClinicsDb);
		}
		#endregion
		#region Methods - Misc
		///<summary>Returns true if any of the rows have an PathExportCCD filed out and IsEnabled is true.</summary>
		public static bool IsEnabled() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM hieclinic "
				+"WHERE PathExportCCD !='' AND IsEnabled=1";
			return Db.GetCount(command)!="0";
		}

		#endregion Methods - Misc
	}
}