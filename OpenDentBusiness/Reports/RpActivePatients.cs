using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpActivePatients {

		///<summary>If not using clinics then supply an empty list of clinicNums. dateStart and dateEnd can be MinVal/MaxVal to indicate "forever".</summary>
		public static DataTable GetActivePatientTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums,List<long> listBillingTypes,List<long> listPatientStatuses,bool hasAllProvs,bool hasAllClinics,bool hasAllBilling) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,listBillingTypes,listPatientStatuses,hasAllProvs,hasAllClinics,hasAllBilling);
			}
			bool hasClinicsEnabled=ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			List<Provider> listProvs=ReportsComplex.RunFuncOnReportServer(() => Providers.GetAll());
			List<Def> listDefs=ReportsComplex.RunFuncOnReportServer(() => Defs.GetDefsNoCache(DefCat.BillingTypes));
			List<Clinic> listClinics=ReportsComplex.RunFuncOnReportServer(() => Clinics.GetClinicsNoCache());
			DataTable table=new DataTable();
			table.Columns.Add("name");
			table.Columns.Add("priProv");
			table.Columns.Add("Address");
			table.Columns.Add("Address2");
			table.Columns.Add("City");
			table.Columns.Add("State");
			table.Columns.Add("Zip");
			table.Columns.Add("carrier");
			table.Columns.Add("HmPhone");
			table.Columns.Add("WkPhone");
			table.Columns.Add("WirelessPhone");
			table.Columns.Add("billingType");
			table.Columns.Add("secProv");
			table.Columns.Add("clinic");
			DataRow row;
			string command=$@"
				SELECT patient.PatNum,patient.LName,patient.FName,patient.MiddleI,patient.Preferred,carrier.CarrierName,patient.BillingType,patient.PriProv,patient.SecProv,
							patient.HmPhone,patient.WkPhone,patient.WirelessPhone,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip,patient.ClinicNum,provider.Abbr
				FROM procedurelog 
				INNER JOIN patient ON patient.PatNum=procedurelog.PatNum AND PatStatus IN ({String.Join(",",listPatientStatuses)})
				LEFT JOIN patplan ON patplan.PatNum=patient.PatNum AND patplan.Ordinal=1
				LEFT JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum
				LEFT JOIN insplan ON insplan.PlanNum=inssub.PlanNum
				LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum
				LEFT JOIN provider ON provider.ProvNum=patient.PriProv 
				WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@"
					AND procedurelog.ProcDate BETWEEN "+POut.DateT(dateStart)+@" AND "+POut.DateT(dateEnd);
			if(!hasAllProvs) {
				command+=@" AND (patient.PriProv IN("+String.Join(",",listProvNums)+") OR patient.SecProv IN("+String.Join(",",listProvNums)+")) ";
			}
			if(listClinicNums.Count>0) {
				command+="AND patient.ClinicNum IN("+String.Join(",",listClinicNums)+") ";
			}
			command+="AND patient.BillingType IN("+String.Join(",",listBillingTypes)+") ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY patient.PatNum";
			}
			else {//Oracle
				command+=@"GROUP BY patient.PatNum,patient.LName,patient.FName,patient.MiddleI,patient.Preferred,carrier.CarrierName
					,patient.BillingType,patient.PriProv,patient.SecProv,patient.HmPhone,patient.WkPhone,patient.WirelessPhone
					,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip,patient.ClinicNum";
			}
			if(!hasClinicsEnabled) {
				command+=" ORDER BY provider.Abbr,patient.LName,patient.FName";
			}
			else {//Using clinics
				command+=" ORDER BY patient.ClinicNum,provider.Abbr,patient.LName,patient.FName";
			}
			DataTable raw=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			Patient pat;
			for(int i=0;i<raw.Rows.Count;i++) {
				Def billingType=listDefs.FirstOrDefault(x => x.DefNum==PIn.Long(raw.Rows[i]["BillingType"].ToString()));
				row=table.NewRow();
				pat=new Patient();
				pat.LName=raw.Rows[i]["LName"].ToString();
				pat.FName=raw.Rows[i]["FName"].ToString();
				pat.MiddleI=raw.Rows[i]["MiddleI"].ToString();
				pat.Preferred=raw.Rows[i]["Preferred"].ToString();
				row["name"]=pat.GetNameLF();
				row["priProv"]=Providers.GetAbbr(PIn.Long(raw.Rows[i]["PriProv"].ToString()));
				row["Address"]=raw.Rows[i]["Address"].ToString();
				row["Address2"]=raw.Rows[i]["Address2"].ToString();
				row["City"]=raw.Rows[i]["City"].ToString();
				row["State"]=raw.Rows[i]["State"].ToString();
				row["Zip"]=raw.Rows[i]["Zip"].ToString();
				row["Carrier"]=raw.Rows[i]["CarrierName"].ToString();
				row["HmPhone"]=raw.Rows[i]["HmPhone"].ToString();
				row["WkPhone"]=raw.Rows[i]["WkPhone"].ToString();
				row["WirelessPhone"]=raw.Rows[i]["WirelessPhone"].ToString();
				row["billingType"]=(billingType==null) ? "" : billingType.ItemValue;
				row["secProv"]=Providers.GetLName(PIn.Long(raw.Rows[i]["SecProv"].ToString()),listProvs);
				if(hasClinicsEnabled) {//Using clinics
					string clinicDesc=Clinics.GetDesc(PIn.Long(raw.Rows[i]["ClinicNum"].ToString()),listClinics);
					row["clinic"]=(clinicDesc=="")?Lans.g("FormRpPayPlans","Unassigned"):clinicDesc;
				}
				table.Rows.Add(row);
			}
			return table;
		}

	}
}
