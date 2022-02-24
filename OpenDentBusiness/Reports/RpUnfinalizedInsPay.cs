using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class RpUnfinalizedInsPay {
		///<summary>Gets a list of unfinalized insurance payments.</summary>
		public static List<UnfinalizedInsPay> GetUnfinalizedInsPay(string carrierName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UnfinalizedInsPay>>(MethodBase.GetCurrentMethod(),carrierName);
			}
			string command=@"
				SELECT partialpay.PayType,partialpay.PatNum,partialpay.ClaimPaymentNum,partialpay.ClinicNum,partialpay.CarrierName,partialpay.Date,
				partialpay.DOS,partialpay.Amount,partialpay.ClaimNum,partialpay.CountPats
				FROM (	
						SELECT 'PartialPayment' PayType,COALESCE(MAX(claimproc.PatNum),0) PatNum,MAX(claimpayment.ClaimPaymentNum) ClaimPaymentNum,
						COUNT(DISTINCT claimproc.PatNum) CountPats,MAX(claimpayment.ClinicNum) ClinicNum,MAX(claimpayment.CarrierName) CarrierName,
						MAX(claimpayment.CheckDate) Date,COALESCE(MAX(claimproc.ProcDate),'0001-01-01') DOS,MAX(claimpayment.CheckAmt) Amount,0 ClaimNum
						FROM claimpayment 
						LEFT JOIN claimproc ON claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum
						WHERE claimpayment.IsPartial = 1 
						AND claimpayment.CarrierName LIKE '%"+POut.String(carrierName.Trim())+"%' "+@"
						GROUP BY claimpayment.ClaimPaymentNum	
						UNION ALL	
						SELECT 'UnfinalizedPayment' PayType,MAX(claimproc.PatNum) PatNum,0 ClaimPaymentNum,1 CountPats,MAX(claimproc.ClinicNum) ClinicNum,
						MAX(carrier.CarrierName) CarrierName,MAX(claimproc.DateCP) Date,MAX(claimproc.ProcDate) DOS,SUM(claimproc.InsPayAmt) Amount,
						claimproc.ClaimNum
						FROM claimproc
						INNER JOIN insplan ON insplan.PlanNum=claimproc.PlanNum
						INNER JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum	
							AND carrier.CarrierName LIKE '%"+POut.String(carrierName.Trim())+"%' "
						//Filter logic here mimics batch payments in ClaimProcs.AttachAllOutstandingToPayment().
						+@"WHERE claimproc.ClaimPaymentNum = 0 AND claimproc.InsPayAmt != 0 
							AND claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Received)+","
							+POut.Int((int)ClaimProcStatus.Supplemental)+","+POut.Int((int)ClaimProcStatus.CapClaim)+@") 
							AND claimproc.IsTransfer=0 
						GROUP BY claimproc.ClaimNum	
			) partialpay";
			DataTable table=ReportsComplex.RunFuncOnReportServer(()=> Db.GetTable(command));
			List<Patient> listPats=Patients.GetMultPats(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList()).ToList();
			List<Claim> listClaims=Claims.GetClaimsFromClaimNums(table.Select().Select(x => PIn.Long(x["ClaimNum"].ToString())).ToList());
			List<ClaimPayment> listPayments=ClaimPayments.GetByClaimPaymentNums(table.Select().Select(x => PIn.Long(x["ClaimPaymentNum"].ToString()))
				.ToList());
			List<UnfinalizedInsPay> listUnfinalizedInsPay=new List<UnfinalizedInsPay>();
			for(int i=0;i<table.Rows.Count;i++) {
				listUnfinalizedInsPay.Add(new UnfinalizedInsPay(table.Rows[i]["PayType"].ToString(),
					listPats.FirstOrDefault(x => x.PatNum==PIn.Long(table.Rows[i]["PatNum"].ToString())),
					PIn.Long(table.Rows[i]["ClinicNum"].ToString()),
					table.Rows[i]["CarrierName"].ToString(),
					PIn.Date(table.Rows[i]["Date"].ToString()),
					PIn.Date(table.Rows[i]["DOS"].ToString()),
					PIn.Double(table.Rows[i]["Amount"].ToString()),
					listPayments.FirstOrDefault(x => x.ClaimPaymentNum==PIn.Long(table.Rows[i]["ClaimPaymentNum"].ToString())),
					listClaims.FirstOrDefault(x => x.ClaimNum==PIn.Long(table.Rows[i]["ClaimNum"].ToString())),
					PIn.Int(table.Rows[i]["CountPats"].ToString())
				));
			}
			return listUnfinalizedInsPay;
		}

		///<summary>Class that contains a singular unfinalized insurance payment and all relevant information. Could be a partial payment or 
		///a paid claim with no check attached.</summary>
		public class UnfinalizedInsPay {
			public UnfinalizedPaymentType Type;
			public Patient PatientCur;
			public Clinic ClinicCur;
			public Carrier CarrierCur;
			public ClaimPayment ClaimPaymentCur;
			public Claim ClaimCur;
			public DateTime Date;
			public DateTime DateOfService;
			public double Amount;
			public int CountPats;

			public UnfinalizedInsPay() {
				//Need for serialization
			}

			public UnfinalizedInsPay(string type,Patient patCur,long clinicNumCur,string carrierCur,DateTime date,DateTime dateOfService,double amt,
				ClaimPayment claimPay,Claim claim,int countPats) 
			{
				//assign passed-in values
				Type=(UnfinalizedPaymentType)Enum.Parse(typeof(UnfinalizedPaymentType),type);
				if(patCur == null) {
					PatientCur=new Patient {
						FName="",
						LName="",
						MiddleI=""
					};
				}
				else {
					PatientCur=patCur;
				}
				ClinicCur=Clinics.GetClinic(clinicNumCur)??
					new Clinic {//creating new clinic with Unassigned as description. The clinic will not get inserted into the db.
					ClinicNum=0,
					Description="Unassigned",
					Abbr="Unassigned"
				};
				//find CarrierCur. GetCarrier uses the H List if possible.
				CarrierCur=Carriers.GetCarrierByName(carrierCur)??new Carrier() { CarrierName=carrierCur };
				ClaimPaymentCur=claimPay;
				ClaimCur=claim;
				Date=date;
				DateOfService=dateOfService;
				Amount=amt;
				CountPats=countPats;
			}

			public enum UnfinalizedPaymentType {
				[Description("Partial Payment")]
				PartialPayment,
				[Description("Unfinalized Payment")]
				UnfinalizedPayment
			}
		}
	}
}
