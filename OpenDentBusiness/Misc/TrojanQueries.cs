using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class TrojanQueries {

		public static DateTime GetMaxProcedureDate(long PatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),PatNum);
			}
			string command=$@"SELECT MAX(ProcDate) FROM procedurelog,patient
				WHERE patient.PatNum=procedurelog.PatNum
				AND procedurelog.ProcStatus={POut.Int((int)ProcStat.C)}
				AND patient.Guarantor={POut.Long(PatNum)}";
			return PIn.Date(Db.GetScalar(command));
		}

		public static DateTime GetMaxPaymentDate(long PatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),PatNum);
			}
			string command=$@"SELECT MAX(DatePay) FROM paysplit,patient
				WHERE patient.PatNum=paysplit.PatNum
				AND patient.Guarantor={POut.Long(PatNum)}";
			return PIn.Date(Db.GetScalar(command));
		}

		///<summary>Increments the PreviousFileNumber program property to the next available int and returns that new file number.</summary>
		public static int GetUniqueFileNum(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			long progNum=Programs.GetProgramNum(ProgramName.TrojanExpressCollect);
			int fileNum=PIn.Int(ProgramProperties.GetValFromDb(progNum,"PreviousFileNumber"),false)+1;
			while(ProgramProperties.SetProperty(progNum,"PreviousFileNumber",fileNum.ToString())<1) {
				fileNum++;
			}
			return fileNum;
		}

		///<summary>Get the list of records for the pending plan deletion report for plans that need to be brought to the patient's attention.</summary>
		public static DataTable GetPendingDeletionTable(Collection<string[]> deletePatientRecords) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),deletePatientRecords);
			}
			string whereTrojanID="";
			for(int i=0;i<deletePatientRecords.Count;i++) {
				if(i>0) {
					whereTrojanID+="OR ";
				}
				whereTrojanID+="i.TrojanID='"+deletePatientRecords[i][0]+"' ";
			}
			string command="SELECT DISTINCT "+
					"p.FName,"+
					"p.LName,"+
					"p.FName,"+
					"p.LName,"+
					"p.SSN,"+
					"p.Birthdate,"+
					"i.GroupNum,"+
					"s.SubscriberID,"+
					"i.TrojanID,"+
					"CASE i.EmployerNum WHEN 0 THEN '' ELSE e.EmpName END,"+
					"CASE i.EmployerNum WHEN 0 THEN '' ELSE e.Phone END,"+
					"c.CarrierName,"+
					"c.Phone "+
					"FROM patient p,insplan i,employer e,carrier c,inssub s "+
					"WHERE p.PatNum=s.Subscriber AND "+
					"("+whereTrojanID+") AND "+
					"i.CarrierNum=c.CarrierNum AND "+
					"s.PlanNum=i.PlanNum AND "+
					"(i.EmployerNum=e.EmployerNum OR i.EmployerNum=0) AND "+
					"(SELECT COUNT(*) FROM patplan a WHERE a.PatNum=p.PatNum AND a.InsSubNum=s.InsSubNum) > 0 "+
					"ORDER BY i.TrojanID,p.LName,p.FName";
			return Db.GetTable(command);
		}

		///<summary>Get the list of records for the pending plan deletion report for plans which need to be bought to Trojan's attention.</summary>
		public static DataTable GetPendingDeletionTableTrojan(Collection<string[]> deleteTrojanRecords) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),deleteTrojanRecords);
			}
			string whereTrojanID="";
			for(int i=0;i<deleteTrojanRecords.Count;i++) {
				if(i>0) {
					whereTrojanID+="OR ";
				}
				whereTrojanID+="i.TrojanID='"+deleteTrojanRecords[i][0]+"' ";
			}
			string command="SELECT DISTINCT "+
					"p.FName,"+
					"p.LName,"+
					"p.FName,"+
					"p.LName,"+
					"p.SSN,"+
					"p.Birthdate,"+
					"i.GroupNum,"+
					"s.SubscriberID,"+
					"i.TrojanID,"+
					"CASE i.EmployerNum WHEN 0 THEN '' ELSE e.EmpName END,"+
					"CASE i.EmployerNum WHEN 0 THEN '' ELSE e.Phone END,"+
					"c.CarrierName,"+
					"c.Phone "+
					"FROM patient p,insplan i,employer e,carrier c,inssub s "+
					"WHERE p.PatNum=s.Subscriber AND "+
					"("+whereTrojanID+") AND "+
					"i.CarrierNum=c.CarrierNum AND "+
					"s.PlanNum=i.PlanNum AND "+
					"(i.EmployerNum=e.EmployerNum OR i.EmployerNum=0) AND "+
					"(SELECT COUNT(*) FROM patplan a WHERE a.PatNum=p.PatNum AND a.InsSubNum=s.InsSubNum) > 0 "+
					"ORDER BY i.TrojanID,p.LName,p.FName";
			return Db.GetTable(command);
		}

		public static InsPlan GetPlanWithTrojanID(string trojanID){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsPlan>(MethodBase.GetCurrentMethod(),trojanID);
			}
			string command="SELECT * FROM insplan WHERE TrojanID = '"+POut.String(trojanID)+"'";
			return Crud.InsPlanCrud.SelectOne(command);
		}

		///<summary>This returns the number of plans affected.</summary>
		public static void UpdatePlan(TrojanObject troj,long planNum,bool updateBenefits) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),troj,planNum,updateBenefits);
				return;
			}
			long employerNum=Employers.GetEmployerNum(troj.ENAME);
			string command;
			//for(int i=0;i<planNums.Count;i++) {
			command="UPDATE insplan SET "
				+"EmployerNum="  +POut.Long  (employerNum)+", "
				+"GroupName='"   +POut.String(troj.PLANDESC)+"', "
				+"GroupNum='"    +POut.String(troj.POLICYNO)+"', "
				+"CarrierNum= "  +POut.Long  (troj.CarrierNum)+" "
				+"WHERE PlanNum="+POut.Long  (planNum);
			Db.NonQ(command);
			command=$@"UPDATE insbluebook SET
				insbluebook.GroupNum='{POut.String(troj.POLICYNO)}',
				insbluebook.CarrierNum={POut.Long(troj.CarrierNum)}
				WHERE insbluebook.PlanNum={POut.Long(planNum)}";
			Db.NonQ(command);
			command="UPDATE inssub SET "
				+"BenefitNotes='"+POut.String(troj.BenefitNotes)+"' "
				+"WHERE PlanNum="+POut.Long(planNum);
			Db.NonQ(command);
			if(updateBenefits) {
				//clear benefits
				command="DELETE FROM benefit WHERE PlanNum="+POut.Long(planNum);
				Db.NonQ(command);
				//benefitList
				for(int j=0;j<troj.BenefitList.Count;j++) {
					troj.BenefitList[j].PlanNum=planNum;
					Benefits.Insert(troj.BenefitList[j]);
				}
				InsPlans.ComputeEstimatesForTrojanPlan(planNum);
			}
		}

	}

	///<summary>This is used as a container for plan and benefit info coming in from Trojan.</summary>
	[Serializable()]
	public class TrojanObject {
		///<summary>TrojanID</summary>
		public string TROJANID;
		///<summary>Employer name</summary>
		public string ENAME;
		///<summary>GroupName</summary>
		public string PLANDESC;
		///<summary>Carrier phone</summary>
		public string ELIGPHONE;
		///<summary>GroupNum</summary>
		public string POLICYNO;
		///<summary>Accepts eclaims</summary>
		public bool ECLAIMS;
		///<summary>ElectID</summary>
		public string PAYERID;
		///<summary>CarrierName</summary>
		public string MAILTO;
		///<summary>Address</summary>
		public string MAILTOST;
		///<summary>City</summary>
		public string MAILCITYONLY;
		///<summary>State</summary>
		public string MAILSTATEONLY;
		///<summary>Zip</summary>
		public string MAILZIPONLY;
		///<summary>The only thing that will be missing from these benefits is the PlanNum.</summary>
		public List<Benefit> BenefitList;
		///<summary>This can be filled at some point based on the carrier fields.</summary>
		public long CarrierNum;
		///<summary></summary>
		public string BenefitNotes;
		///<summary></summary>
		public string PlanNote;
		///<summary></summary>
		public int MonthRenewal;
	}


}
