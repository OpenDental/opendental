using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class RpRouting {

		///<summary>Gets a list of aptNums for one day in the schedule for a given set of providers and clinics.  Will be for all clinics and/or all provs
		///if the corresponding list is null or empty.</summary>
		public static List<long> GetRouting(DateTime date,List<long> listProvNums,List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),date,listProvNums,listClinicNums);
			}
			//Excluding PtNote and PtNoteCompleted per Nathan and Arna, see job 1064
			string command="SELECT AptNum FROM appointment "
				+"WHERE "+DbHelper.DateTConditionColumn("AptDateTime",ConditionOperator.Equals,date)+" "
				+"AND AptStatus NOT IN ("+POut.Int((int)ApptStatus.UnschedList)+","+POut.Int((int)ApptStatus.Planned)+","+POut.Int((int)ApptStatus.PtNote)+","
					+POut.Int((int)ApptStatus.PtNoteCompleted)+") ";
			if(listProvNums!=null && listProvNums.Count>0) {
				command+="AND (ProvNum IN ("+string.Join(",",listProvNums)+") OR ProvHyg IN ("+string.Join(",",listProvNums)+")) ";
			}
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+="AND ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
			}
			command+="ORDER BY AptDateTime";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetListLong(command));
		}
	}
}
