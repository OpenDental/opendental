using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RxPats {
		///<summary>Returns a list of RxPats containing the passed in PatNum.</summary>
		public static List<RxPat> GetAllForPat(long patNum,RxTypes rxTypes=RxTypes.Rx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxPat>>(MethodBase.GetCurrentMethod(),patNum,rxTypes);
			}
			string command="SELECT * FROM rxpat WHERE PatNum="+POut.Long(patNum)+" AND RxType="+POut.Enum(rxTypes);
			return Crud.RxPatCrud.SelectMany(command);
		}

		///<summary>Used in Ehr.  Excludes controlled substances.</summary>
		public static List<RxPat> GetPermissableForDateRange(long patNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxPat>>(MethodBase.GetCurrentMethod(),patNum,dateStart,dateStop);
			}
			string command="SELECT * FROM rxpat WHERE PatNum="+POut.Long(patNum)+" "
				+"AND RxDate >= "+POut.Date(dateStart)+" "
				+"AND RxDate <= "+POut.Date(dateStop)+" "
				+"AND IsControlled = 0 "
				+"AND RxType="+POut.Enum(RxTypes.Rx);
			return Crud.RxPatCrud.SelectMany(command);
		}

		///<summary></summary>
		public static RxPat GetRx(long rxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<RxPat>(MethodBase.GetCurrentMethod(),rxNum);
			}
			return Crud.RxPatCrud.SelectOne(rxNum);
		}

		///<summary></summary>
		public static void Update(RxPat rxPat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxPat);
				return;
			}
			Crud.RxPatCrud.Update(rxPat);
		}

		public static bool Update(RxPat rxPat,RxPat rxPatOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),rxPat,rxPatOld);
			}
			return Crud.RxPatCrud.Update(rxPat,rxPatOld);
		}

		///<summary></summary>
		public static long Insert(RxPat rxPat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				rxPat.RxNum=Meth.GetLong(MethodBase.GetCurrentMethod(),rxPat);
				return rxPat.RxNum;
			}
			return Crud.RxPatCrud.Insert(rxPat);
		}

		///<summary></summary>
		public static void Delete(long rxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxNum);
				return;
			}
			Crud.RxPatCrud.Delete(rxNum);
		}

		public static List<long> GetChangedSinceRxNums(DateTime dateTChangedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTChangedSince);
			}
			string command="SELECT RxNum FROM rxpat WHERE DateTStamp > "+POut.DateT(dateTChangedSince)+" AND RxType="+POut.Enum(RxTypes.Rx);
			DataTable tableRxNums=Db.GetTable(command);
			List<long> listRxNums = new List<long>(tableRxNums.Rows.Count);
			for(int i=0;i<tableRxNums.Rows.Count;i++) {
				listRxNums.Add(PIn.Long(tableRxNums.Rows[i]["RxNum"].ToString()));
			}
			return listRxNums;
		}

		///<summary>Used along with GetChangedSinceRxNums</summary>
		public static List<RxPat> GetMultRxPats(List<long> listRxNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxPat>>(MethodBase.GetCurrentMethod(),listRxNums);
			}
			string strRxNums="";
			DataTable tableRxPats;
			if(listRxNums.Count>0) {
				for(int i=0;i<listRxNums.Count;i++) {
					if(i>0) {
						strRxNums+="OR ";
					}
					strRxNums+="RxNum='"+listRxNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM rxpat WHERE "+strRxNums;
				tableRxPats=Db.GetTable(command);
			}
			else {
				tableRxPats=new DataTable();
			}
			RxPat[] arrayRxPats=Crud.RxPatCrud.TableToList(tableRxPats).ToArray();
			List<RxPat> listRxPats=new List<RxPat>(arrayRxPats);
			return listRxPats;
		}

		///<summary>Used in FormRxSend to fill electronic queue.</summary>
		public static List<RxPat> GetQueue() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxPat>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM rxpat WHERE SendStatus=1 AND RxType="+POut.Enum(RxTypes.Rx);
			return Crud.RxPatCrud.SelectMany(command);
		}

		///<summary></summary>
		public static RxPat GetErxByIdForPat(string erxGuid,long patNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<RxPat>(MethodBase.GetCurrentMethod(),erxGuid,patNum);
			}
			string command="SELECT * FROM rxpat WHERE ErxGuid='"+POut.String(erxGuid)+"' AND RxType="+POut.Enum(RxTypes.Rx);
			if(patNum!=0) {
				command+=" AND PatNum="+POut.Long(patNum);
			}
			List<RxPat> listRxPats=Crud.RxPatCrud.SelectMany(command);
			if(listRxPats.Count==0) {
				return null;
			}
			return listRxPats[0];
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching rxNum as FKey and are related to RxPat.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the RxPat table type.</summary>
		public static void ClearFkey(long rxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxNum);
				return;
			}
			Crud.RxPatCrud.ClearFkey(rxNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching rxNums as FKey and are related to RxPat.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the RxPat table type.</summary>
		public static void ClearFkey(List<long> listRxNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRxNums);
				return;
			}
			Crud.RxPatCrud.ClearFkey(listRxNums);
		}

		///<summary>Creates an RxPat when pdmp bridge is used and inserts it into RxPat</summary>
		public static void CreatePdmpAccessLog(Patient patient,Userod userod,Program program) {
			RxPat rxPat=new RxPat();
			rxPat.PatNum=patient.PatNum;
			rxPat.UserNum=userod.UserNum;
			rxPat.ProvNum=userod.ProvNum;
			rxPat.ClinicNum=PrefC.HasClinicsEnabled ? patient.ClinicNum : 0;
			rxPat.RxDate=DateTime.Today;
			if(program.ProgName==ProgramName.PDMP.ToString()) {
				rxPat.RxType=RxTypes.LogicoyAccess;
			}
			else {
				rxPat.RxType=RxTypes.ApprisAccess;
			}
			Insert(rxPat);
		}
	}

}













