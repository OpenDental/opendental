using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using OpenDental;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EtransT {
	
		///<summary></summary>
		private static Etrans835Attach Insert835Attach(long etransNum,long claimNum,int clpSegmentIndex,DateTime dateTimeTrans) {
			Etrans835Attach attach=new Etrans835Attach() {
				EtransNum=etransNum,
				ClaimNum=claimNum,
				ClpSegmentIndex=clpSegmentIndex
			};
			Etrans835Attaches.Insert(attach);
			attach.DateTimeTrans=dateTimeTrans;
			return attach;
		}
		
		///<summary></summary>
		public static Etrans Insert835Etrans(string etransMessageText,DateTime dateTimeTrans) {
			Etrans etrans835=Etranss.CreateEtrans(DateTime.Now,0,etransMessageText,0);//Todo: T create class.
			etrans835.Etype=EtransType.ERA_835;
			Etranss.Insert(etrans835);
			etrans835.DateTimeTrans=dateTimeTrans;
			return etrans835;
		}

		///<summary>Builds the X835 for given etrans835 and etransMessageText.
		///Also attempts to create attaches for patients in listPatients using their first/last name and the claim num in their tuple entry.</summary>
		public static X835 Construct835(Etrans etrans835,string etransMessageText,List<ODTuple<Patient,long>> listPatients,out List<Etrans835Attach>listEtrans835Attaches)
		{
			listEtrans835Attaches=new List<Etrans835Attach>();
			//Construct the inital X835 so that we can find the ERA claim we are testing against, needed for attach information.
			X835 x835=new X835(etrans835,etransMessageText,etrans835.TranSetId835);
			foreach(ODTuple<Patient,long> data in listPatients) {
				Patient pat=data.Item1;
				long claimNum=data.Item2;
				//Then using that ERA claim we will spoof the Etrans835Attach logic that would run when associating an OD claim to the ERA claim.
				Hx835_Claim eraClaim=x835.ListClaimsPaid.First(x => x.PatientName.Fname.ToLower()==pat.FName.ToLower() && x.PatientName.Lname.ToLower()==pat.LName.ToLower());
				//Spoof ERA claim attachement.
				Etrans835Attach attach=Insert835Attach(etrans835.EtransNum,claimNum,eraClaim.ClpSegmentIndex,etrans835.DateTimeTrans);
				listEtrans835Attaches.Add(attach);
			}
			//Finally we must reconstruct the X835 with the new spoofed attach.
			return new X835(etrans835,etransMessageText,etrans835.TranSetId835,listEtrans835Attaches);
		}
		
		///<summary>Creates insurance and completed procedures prior to creating a claim with given claimType and claimIdentifier.</summary>
		public static Claim SetupEraClaim(Patient pat,List<EraTestProcCodeData> listProcCodes,string claimType,string claimIdentifier,out List<InsuranceInfo> listInsuranceInfo) {
			listInsuranceInfo=new List<InsuranceInfo>();
			//Create Insurance
			InsuranceInfo insuranceInfoPercentage=InsuranceT.AddInsurance(pat,"PrimaryCarrier");//non-ppo
			InsuranceInfo insuranceInfoPPO=InsuranceT.AddInsurance(pat,"SecondaryCarrier","p",ordinal:2);//PPO, currently not used.
			listInsuranceInfo.Add(insuranceInfoPercentage);
			listInsuranceInfo.Add(insuranceInfoPPO);
			//Create and complete procedures.
			List<Procedure> listProcs=new List<Procedure>();
			foreach(EraTestProcCodeData data in listProcCodes) {
				Procedure proc=ProcedureT.CreateProcedure(pat,data.ProcCode,data.ProcStatus,"0",data.ProcFee,data.ProcDateTime);
				ProcedureT.SetComplete(proc,pat,insuranceInfoPercentage);//Does not create claimProcs if they are backdated.
				listProcs.Add(proc);
			}
			//Create claim
			Claim claim=ClaimT.CreateClaim(listProcs,insuranceInfoPercentage,claimType,claimIdentifier);//Creates missing claimProcs, uses procDate, claimIdentifier from ERA.
			claim.ClaimStatus="U";//Set the claim status to "Unsent" to match the attached claimproc.Status of "Estimate"
			Claims.Update(claim);
			//This function performs vital updates to various claim and claimproc fields which are needed for ERA matching to work properly.
			Claims.CalculateAndUpdate(listProcs,
				listInsuranceInfo.SelectMany(x => x.ListInsPlans).ToList(),
				claim,
				listInsuranceInfo.SelectMany(x => x.ListPatPlans).ToList(),
				listInsuranceInfo.SelectMany(x => x.ListBenefits).ToList(),
				pat,listInsuranceInfo.SelectMany(x => x.ListInsSubs).ToList());
			return claim;
		}

		///<summary>Deletes everything from the etrans table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEtransTable() {
			string command="DELETE FROM etrans";
			DataCore.NonQ(command);
		}
	}

	public class EraTestProcCodeData {
		public string ProcCode;
		public ProcStat ProcStatus;
		public double ProcFee;
		public DateTime ProcDateTime;

		public EraTestProcCodeData(string procCode,ProcStat procStatus,double procFee,DateTime procDateTime) {
			ProcCode=procCode;
			ProcStatus=procStatus;
			ProcFee=procFee;
			ProcDateTime=procDateTime;
		}
	}

}
