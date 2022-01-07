using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Etrans835s{

		#region Methods - Get
		///<summary></summary>
		public static List<Etrans835> GetByEtransNums(params long[] arrayEtransNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans835>>(MethodBase.GetCurrentMethod(),arrayEtransNums);
			}
			if(arrayEtransNums.Length==0) {
				return new List<Etrans835>();
			}
			string command="SELECT * FROM etrans835 WHERE EtransNum IN("+string.Join(",",arrayEtransNums)+")";
			return Crud.Etrans835Crud.SelectMany(command);
		}

		///<summary>All parameters are optional and will be excluded from the query if not set.
		///Strings are considered not set if blank, dates are considered not set if equal to DateTime.MinVal, decimals are not set if negative.</summary>
		public static List<Etrans835> GetFiltered(DateTime dateFrom,DateTime dateTo,string carrierName,string checkTraceNum,decimal insPaidMin,decimal insPaidMax,
			string controlId,params X835Status[] arrayStatuses)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans835>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,carrierName,checkTraceNum,insPaidMin,insPaidMax,controlId,arrayStatuses);
			}
			string command="SELECT * FROM etrans835 "
				+"INNER JOIN etrans on etrans.EtransNum=etrans835.EtransNum ";
			List<string> listJoinClauses=new List<string>();
			if(carrierName!="") {
				listJoinClauses.Add("LOWER(TRIM(etrans835.PayerName)) LIKE '%"+POut.String(carrierName.ToLower().Trim())+"%'");
			}
			if(checkTraceNum!="") {
				listJoinClauses.Add("LOWER(TRIM(etrans835.TransRefNum)) LIKE '%"+POut.String(checkTraceNum.ToLower().Trim())+"%'");
			}
			if(insPaidMin >= 0) {
				listJoinClauses.Add("etrans835.InsPaid >= "+POut.Decimal(insPaidMin));
			}
			if(insPaidMax >= 0) {
				listJoinClauses.Add("etrans835.InsPaid <= "+POut.Decimal(insPaidMax));
			}
			if(controlId!="") {
				listJoinClauses.Add("LOWER(TRIM(etrans835.ControlId)) LIKE '%"+POut.String(controlId.ToLower().Trim())+"%'");
			}
			if(arrayStatuses.Length > 0) {
				listJoinClauses.Add("etrans835.Status IN ("+string.Join(",",arrayStatuses.Select(x => (int)x))+")");
			}
			if(listJoinClauses.Count > 0) {
				command+=" AND "+string.Join(" AND ",listJoinClauses);
			}
			List<string> listWhereClauses=new List<string>();
			if(dateFrom!=DateTime.MinValue) {
				listWhereClauses.Add(DbHelper.DtimeToDate("etrans.DateTimeTrans")+" >= "+POut.Date(dateFrom));
			}
			if(dateTo!=DateTime.MinValue) {
				listWhereClauses.Add(DbHelper.DtimeToDate("etrans.DateTimeTrans")+" <= "+POut.Date(dateTo));
			}
			if(listWhereClauses.Count > 0) {
				command+=" WHERE "+string.Join(" AND ",listWhereClauses);
			}
			return Crud.Etrans835Crud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(Etrans835 etrans835){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				etrans835.Etrans835Num=Meth.GetLong(MethodBase.GetCurrentMethod(),etrans835);
				return etrans835.Etrans835Num;
			}
			return Crud.Etrans835Crud.Insert(etrans835);
		}

		///<summary></summary>
		public static void Update(Etrans835 etrans835,Etrans835 oldEtrans835){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etrans835,oldEtrans835);
				return;
			}
			Crud.Etrans835Crud.Update(etrans835,oldEtrans835);
		}

		public static void Upsert(Etrans835 etrans835,X835 x835) {
			Etrans835 oldEtrans835=etrans835.Copy();
			etrans835.PayerName=x835.PayerName;
			etrans835.TransRefNum=x835.TransRefNum;
			etrans835.InsPaid=(double)x835.InsPaid;
			etrans835.ControlId=x835.ControlId;
			etrans835.PaymentMethodCode=x835._paymentMethodCode;
			List<string> listPatNames=x835.ListClaimsPaid.Select(x => x.PatientName.ToString()).Distinct().ToList();
			etrans835.PatientName=(listPatNames.Count>0 ? listPatNames[0] : "");
			if(listPatNames.Count>1) {
				etrans835.PatientName="("+POut.Long(listPatNames.Count)+")";
			}
			etrans835.Status=x835.GetStatus();
			if(etrans835.Etrans835Num==0) {
				Insert(etrans835);
			}
			else {
				Update(etrans835,oldEtrans835);
			}
		}

		#endregion Methods - Modify
		#region Methods - Misc		
		#endregion Methods - Misc
	}
}