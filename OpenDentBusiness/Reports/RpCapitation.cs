using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpCapitation {
		///<summary></summary>
		public static DataTable GetCapitationTable(DateTime dateStart,DateTime dateEnd,string textCarrier,bool isMedicalOrClinic) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,textCarrier,isMedicalOrClinic);
			}			
			string queryString=@"SELECT carrier.CarrierName,CONCAT(CONCAT(patSub.LName,', '),patSub.FName) 
				,patSub.SSN,CONCAT(CONCAT(patPat.LName,', '),patPat.FName)
				,patPat.Birthdate,procedurecode.ProcCode,procedurecode.Descript";
			if(!isMedicalOrClinic) {
				queryString+=",procedurelog.ToothNum,procedurelog.Surf";
			}
			queryString+=@",procedurelog.ProcDate,procedurelog.ProcFee
				,procedurelog.ProcFee-claimproc.WriteOff
				FROM procedurelog,patient AS patSub,patient AS patPat
				,insplan,inssub,carrier,procedurecode,claimproc
				WHERE procedurelog.PatNum = patPat.PatNum
				AND claimproc.InsSubNum = inssub.InsSubNum
				AND procedurelog.ProcNum = claimproc.ProcNum
				AND claimproc.PlanNum = insplan.PlanNum
				AND claimproc.Status = "+POut.Int((int)ClaimProcStatus.CapComplete)+@"
				AND claimproc.NoBillIns = 0 
				AND inssub.Subscriber = patSub.PatNum
				AND insplan.CarrierNum = carrier.CarrierNum	
				AND procedurelog.CodeNum = procedurecode.CodeNum "
				+"AND carrier.CarrierName LIKE '%"+POut.String(textCarrier)+"%' "
				+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
				+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
				+"AND insplan.PlanType = 'c' "
				+"AND procedurelog.ProcStatus = "+POut.Int((int)ProcStat.C);
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(queryString));
		}	
	}

}
