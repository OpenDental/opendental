using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpAging {
		public static DataTable GetAgingTable(RpAgingParamObject rpo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),rpo);
			}
			string queryAg=GetQueryString(rpo);
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(queryAg));
		}

		public static string GetQueryString(RpAgingParamObject rpo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),rpo);
			}
			//patient aging---------------------------------------------------------------------------
			//The aging report always shows historical numbers based on the date entered.
			//the selected columns have to remain in this order due to the way the report complex populates the returned sheet
			string queryAg = "SELECT ";
			if(rpo.IsForInsAging) { //get patNum for insAgingReport only
				queryAg+="patient.PatNum, ";
			}
			if(ReportsComplex.RunFuncOnReportServer(() => (Prefs.GetBoolNoCache(PrefName.ReportsShowPatNum)))) {
				queryAg+=DbHelper.Concat("patient.PatNum","' - '","patient.LName","', '","patient.FName","' '","patient.MiddleI");
			}
			else {
				queryAg+=DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI");
			}
			queryAg+=" patName,guarAging.Bal_0_30,guarAging.Bal_31_60,guarAging.Bal_61_90,guarAging.BalOver90,guarAging.BalTotal,"
				+"guarAging.InsWoEst,guarAging.InsPayEst,guarAging.BalTotal-guarAging.InsPayEst-guarAging.InsWoEst AS ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				queryAg+="$pat";
			}
			else { //Oracle needs quotes.
				queryAg+="\"$pat\"";
			}
			//Must select "blankCol" for use with reportComplex to fix spacing of final column
			queryAg+=(rpo.HasDateLastPay ? ",'' blankCol,guarAging.DateLastPay " : " ")
				+"FROM ("
					+ReportsComplex.RunFuncOnReportServer(() => Ledgers.GetAgingQueryString(asOfDate:rpo.AsOfDate,isHistoric:rpo.IsHistoric,
						isInsPayWoCombined:rpo.IsInsPayWoCombined,hasDateLastPay:rpo.HasDateLastPay,isGroupByGuar:rpo.IsGroupByFam,isWoAged:rpo.IsWoAged,
						doAgePatPayPlanPayments:rpo.DoAgePatPayPlanPayments,doExcludeIncomeTransfers:rpo.doExcludeIncomeTransfers))
				+") guarAging "
				+"INNER JOIN patient ON patient.PatNum=guarAging.PatNum ";
			List<string> listWhereAnds=new List<string>();
			//InsAging will filter for age, but we need to return all in here order for the filtering to be correct
			if(!rpo.IsForInsAging) {
				List<string> listAgeOrs=new List<string>();
				if(rpo.IsIncludeNeg || rpo.IsOnlyNeg) {
					listAgeOrs.Add("guarAging.BalTotal <= -0.005");
				}
				if(rpo.IsIncludeInsNoBal || rpo.IsOnlyInsNoBal) {
					listAgeOrs.Add("((ABS(guarAging.InsPayEst) >= 0.005 OR ABS(guarAging.InsWoEst) >= 0.005) "
						+"AND guarAging.Bal_0_30 < 0.005 AND guarAging.Bal_31_60 < 0.005 AND guarAging.Bal_61_90 < 0.005 AND guarAging.BalOver90 < 0.005)");
				}
				if(!rpo.IsOnlyNeg && !rpo.IsOnlyInsNoBal) {
					listAgeOrs.Add("guarAging.BalOver90 >= 0.005");//applies to all ages
					if(rpo.AccountAge<=AgeOfAccount.Over60) {
						listAgeOrs.Add("guarAging.Bal_61_90 >= 0.005");
					}
					if(rpo.AccountAge<=AgeOfAccount.Over30) {
						listAgeOrs.Add("guarAging.Bal_31_60 >= 0.005");
					}
					if(rpo.AccountAge==AgeOfAccount.Any) {//only applies to Any age
						listAgeOrs.Add("guarAging.Bal_0_30 >= 0.005");
					}
				}
				listWhereAnds.Add("("+string.Join(" OR ",listAgeOrs)+")");
			}
			if(rpo.IsExcludeInactive) {
				listWhereAnds.Add("patient.PatStatus != "+ (int)PatientStatus.Inactive);
			}
			if(rpo.IsExcludeArchive) {
				listWhereAnds.Add("patient.PatStatus != "+ (int)PatientStatus.Archived);
			}
			if(rpo.IsExcludeBadAddress) {
				listWhereAnds.Add("patient.Zip != ''");
			}
			if(rpo.ListBillTypes.Count>0) {//if all bill types is selected, list will be empty
				listWhereAnds.Add("patient.BillingType IN ("+string.Join(",",rpo.ListBillTypes.Select(x => POut.Long(x)))+")");
			}
			if(rpo.ListProvNums.Count>0) {//if all provs is selected, list will be empty
				listWhereAnds.Add("patient.PriProv IN ("+string.Join(",",rpo.ListProvNums.Select(x => POut.Long(x)))+")");
			}
			if(ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache)) //if clinics enabled, at least one clinic will be selected
			{
				//listClin may contain "Unassigned" clinic with ClinicNum 0, in which case it will also be in the query string
				listWhereAnds.Add("patient.ClinicNum IN ("+string.Join(",",rpo.ListClinicNums.Select(x => POut.Long(x)))+")");
			}
			if(listWhereAnds.Count>0) {
				queryAg+="WHERE "+string.Join(" AND ",listWhereAnds)+" ";
			}
			queryAg+="ORDER BY patient.LName,patient.FName";
			return queryAg;
		}

	}

	public enum AgeOfAccount {
		///<summary>0</summary>
		Any,
		///<summary>1</summary>
		Over30,
		///<summary>2</summary>
		Over60,
		///<summary>3</summary>
		Over90,
	}

	[Serializable]
	public class RpAgingParamObject {
		public DateTime AsOfDate=DateTime.Today;
		public bool IsWoAged=false;
		public bool HasDateLastPay=false;
		public bool IsGroupByFam=true;
		public bool IsOnlyNeg=false;
		public AgeOfAccount AccountAge;
		public bool IsIncludeNeg=false;
		public bool IsExcludeInactive=false;
		public bool IsExcludeBadAddress=false;
		public List<long> ListProvNums=new List<long>();
		public List<long> ListClinicNums=new List<long>();
		public List<long> ListBillTypes=new List<long>();
		public bool IsExcludeArchive=false;
		public bool IsIncludeInsNoBal=false;
		public bool IsOnlyInsNoBal=false;
		public bool IsForInsAging=false;
		public bool DoAgePatPayPlanPayments=false;
		public bool IsInsPayWoCombined=true;
		public bool IsHistoric=false;
		public bool IsDetailedBreakdown=false;
		public bool GroupByCarrier=false;
		public bool GroupByGroupName=false;
		public bool doExcludeIncomeTransfers=false;
		public string CarrierNameFilter="";
		public string GroupNameFilter="";

		public RpAgingParamObject Copy() {
			RpAgingParamObject retval=(RpAgingParamObject)this.MemberwiseClone();
			retval.ListProvNums=this.ListProvNums.ToList();
			retval.ListClinicNums=this.ListClinicNums.ToList();
			retval.ListBillTypes=this.ListBillTypes.ToList();
			return retval;
		}
	}
}
