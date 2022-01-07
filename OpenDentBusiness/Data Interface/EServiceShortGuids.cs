using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Health.Direct.Common.Extensions;
using MySql.Data.MySqlClient;
using Word;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EServiceShortGuids{

		#region Get Methods
		
		///<summary>Gets many EServiceShortGuid from the db.</summary>
		public static List<EServiceShortGuid> GetByShortGuid(string shortGuid,bool doIncludeExpired=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EServiceShortGuid>>(MethodBase.GetCurrentMethod(),shortGuid,doIncludeExpired);
			}
			string command=$"SELECT * FROM eserviceshortguid WHERE eserviceshortguid.ShortGuid='{POut.String(shortGuid)}' " +
				(doIncludeExpired ? "" : "AND eserviceshortguid.DateTimeExpiration > "+DbHelper.Now());
			return Crud.EServiceShortGuidCrud.SelectMany(command);
		}

		///<summary>Gets many EServiceShortGuid from the db.</summary>
		public static List<EServiceShortGuid> GetByFKey(EServiceShortGuidKeyType keyType,List<long> listFKeys,bool doIncludeExpired=false) {
			if(listFKeys.IsNullOrEmpty()) {
				return new List<EServiceShortGuid>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EServiceShortGuid>>(MethodBase.GetCurrentMethod(),keyType,listFKeys,doIncludeExpired);
			}
			//KeyType is EnumAsString
			string command=$"SELECT * FROM eserviceshortguid WHERE eserviceshortguid.FKeyType='{POut.String(keyType.ToString())}' " +
				$"AND eserviceshortguid.FKey IN ({string.Join(",",listFKeys.Select(x => POut.Long(x)))}) " +
				(doIncludeExpired ? "" : "AND eserviceshortguid.DateTimeExpiration > "+DbHelper.Now());
			return Crud.EServiceShortGuidCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary>Inserts one EServiceShortGuid into the db.</summary>
		public static long Insert(EServiceShortGuid eServiceShortGuid){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				eServiceShortGuid.EServiceShortGuidNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eServiceShortGuid);
				return eServiceShortGuid.EServiceShortGuidNum;
			}
			return Crud.EServiceShortGuidCrud.Insert(eServiceShortGuid);
		}	

		///<summary>Inserts many EServiceShortGuid into the db.</summary>
		public static void InsertMany(List<EServiceShortGuid> listEServiceShortGuids){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEServiceShortGuids);
				return;
			}
			Crud.EServiceShortGuidCrud.InsertMany(listEServiceShortGuids);
		}
		
		#endregion Modification Methods

		///<summary>Generates a ShortGuid via WebServiceHQ, inserts a corresponding entry into EServiceShortGuid, and returns the result.
		///DateTimeExpiration is set to midnight the day after each appointment.</summary>
		public static List<EServiceShortGuid> GenerateShortGuid(List<Appointment> listAppts,eServiceCode code,EServiceShortGuidKeyType keyType) {
			List<EServiceShortGuid> listShortGuids=new List<EServiceShortGuid>();
			//Group by Appointment.ClinicNum if clinics enabled, otherwise one big group using 0 as the key, which will be used as 
			//ShortGuidLookup.ClinicNum at HQ.
			var groups=PrefC.HasClinicsEnabled ? listAppts.GroupBy(x => x.ClinicNum) : listAppts.GroupBy(x => (long)0);
			foreach(var apptsInClinic in listAppts.GroupBy(x => x.ClinicNum)) {
				List<Appointment> listApptsPerClinic=apptsInClinic.ToList();
				List<WebServiceMainHQProxy.ShortGuidResult> listShorties=WebServiceMainHQProxy.GetShortGUIDs(listApptsPerClinic.Count,listApptsPerClinic.Count
					,apptsInClinic.Key,code);
				for(int i=0;i<listApptsPerClinic.Count;i++) {
					Appointment appt=listApptsPerClinic[i];
					WebServiceMainHQProxy.ShortGuidResult shorty=listShorties[i];
					listShortGuids.Add(new EServiceShortGuid {
						EServiceCode=code,
						ShortGuid=shorty.ShortGuid,
						ShortURL=shorty.ShortURL,
						DateTimeExpiration=appt.AptDateTime.Date.AddDays(1),
						FKey=appt.AptNum,
						FKeyType=keyType,
					});
				}
			}
			InsertMany(listShortGuids);
			return listShortGuids;	
		}

		/*		 
		///<summary></summary>
		public static List<EServiceShortGuid> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EServiceShortGuid>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eserviceshortguid WHERE PatNum = "+POut.Long(patNum);
			return Crud.EServiceShortGuidCrud.SelectMany(command);
		} 

		///<summary></summary>
		public static void Delete(long eServiceShortGuidNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eServiceShortGuidNum);
				return;
			}
			Crud.EServiceShortGuidCrud.Delete(eServiceShortGuidNum);
		}

		///<summary></summary>
		public static void Update(EServiceShortGuid eServiceShortGuid){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eServiceShortGuid);
				return;
			}
			Crud.EServiceShortGuidCrud.Update(eServiceShortGuid);
		}
		*/

	}
}