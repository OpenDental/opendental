using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDental;
using OpenDentBusiness;
using OpenDental.Eclaims;

namespace TestCanada {
	public class ClaimTC {
		///<summary>Remember that this is 0-based.  So subtract 1 from the script number to get the index in this list.</summary>
		public static List<long> ClaimNums;

		public static string CreateAllClaims() {
			ClaimNums=new List<long>();
			CreateOne();
			CreateTwo();
			CreateThree();
			CreateFour();
			CreateFive();
			CreateSix();
			CreateSeven();
			CreateEight();
			CreateNine();
			CreateTen();
			CreateEleven();
			CreateTwelve();
			return "Procedure objects set.\r\nClaim objects set.\r\n";
		}

		private static void CreateOne() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum1);//patient#1, Lisa Fête"
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"26","MOD",107.6,"X",provNum));
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="X";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateTwo() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum1);//patient#1, Lisa Fête"
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"25","MIV",107.6,"X",provNum));//impossible surfaces
			proc=ProcTC.AddProc("27211",pat.PatNum,new DateTime(1999,1,1),"24","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"24","",238,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			proc=ProcTC.AddProc("27213",pat.PatNum,new DateTime(1999,1,1),"26","",450,"E",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"26","",210,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"26","",35,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procList.Add(ProcTC.AddProc("32222",pat.PatNum,new DateTime(1999,1,1),"36","",65,"X",provNum));
			procList.Add(ProcTC.AddProc("39202",pat.PatNum,new DateTime(1999,1,1),"36","",67.5,"X",provNum));
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateThree() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum2);//patient#2, John Smith
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"46","DOV",107.6,"X",provNum));
			proc=ProcTC.AddProc("56112",pat.PatNum,new DateTime(1999,1,1),"","L",217.2,"S",provNum);//lower
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",315,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="N";
			claim.CanadianDateInitialLower=new DateTime(1984,4,7);
			claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateFour() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum4);//patient#4, John Smith, Jr.
			//Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21113",pat.PatNum,new DateTime(1999,1,1),"52","MIV",107.6,"A",provNum));//the date in the script is a typo.
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			pat.SchoolName="Wilson Elementary School";
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=new DateTime(1998,11,2);
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateFive() {
			long provNum=ProviderC.ListShort[1].ProvNum;//dentist#2
			Patient pat=Patients.GetPat(PatientTC.PatNum5);//patient#5, Bob Howard
			//Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"26","MOD",107.6,"A",provNum));
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="081234500";//missing from documentation
			claim.CanadianReferralReason=4;
			pat.SchoolName="";
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSix() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum5);//patient#5, Bob Howard
			//Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",37.5,"X",provNum));//wrong code in documentation
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21213",pat.PatNum,new DateTime(1999,1,1),"22","DIV",107.6,"X",provNum));
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="081234500";
			claim.CanadianReferralReason=4;
			pat.SchoolName="";
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSeven() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum5);//patient#5, Bob Howard
			//Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01202",pat.PatNum,new DateTime(1999,1,1),"","",37.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			procList.Add(ProcTC.AddProc("21213",pat.PatNum,new DateTime(1999,1,1),"22","DIV",107.6,"X",provNum));//wrong code in documentation
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="081234500";
			claim.CanadianReferralReason=4;
			pat.SchoolName="";
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="PD78901234";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateEight() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum6);//patient#6, Martha West
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67211",pat.PatNum,new DateTime(1999,1,1),"10","",450.6,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",487.3,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="Y";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateNine() {
			long provNum=ProviderC.ListShort[1].ProvNum;//dentist#2
			Patient pat=Patients.GetPat(PatientTC.PatNum7);//patient#7, Madeleine Arpege
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"11","",412.6,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",380,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="Y";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=true;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateTen() {
			long provNum=ProviderC.ListShort[1].ProvNum;//dentist#2
			Patient pat=Patients.GetPat(PatientTC.PatNum8);//patient#8, Fred Jones
			Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("21222",pat.PatNum,new DateTime(1999,1,1),"15","MD",102,"X",provNum);
			procList.Add(proc);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateEleven() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum9);//patient#9, Fred Smith
			Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"",provNum));
			proc=ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"26","MOD",107.6,"",provNum);
			procList.Add(proc);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="Y";//Example documentation suggests 'X', but c11.txt and the test environment suggest this value should be 'Y', with a blank date. Very strange.
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateTwelve() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum9);//patient#9, Fred Smith
			Procedure proc;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"",provNum));
			proc=ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"27","MOD",107.6,"",provNum);
			procList.Add(proc);
			Claim claim=CreateClaim(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="";
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=DateTime.MinValue;
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="Y";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="X";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			//claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static Claim CreateClaim(Patient pat,List<Procedure> procList,long provTreat) {
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlanList=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlanList,subList);
			List<ClaimProc> claimProcList=ClaimProcs.Refresh(pat.PatNum);
			List<Procedure> procsForPat=Procedures.Refresh(pat.PatNum);
			InsSub sub=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,1),subList);
			InsPlan insPlan=InsPlans.GetPlan(sub.PlanNum,planList);
			InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,2),subList);
			InsPlan insPlan2=null;
			if(sub2.InsSubNum!=0) {
				insPlan2=InsPlans.GetPlan(sub2.PlanNum,planList);
			}
			Claim claim=new Claim();
			Claims.Insert(claim);//to retreive a key for new Claim.ClaimNum
			claim.PatNum=pat.PatNum;
			claim.DateService=procList[0].ProcDate;
			claim.DateSent=DateTime.Today;
			claim.ClaimStatus="W";
			claim.PlanNum=insPlan.PlanNum;
			if(insPlan2!=null) {
				claim.PlanNum2=insPlan2.PlanNum;
			}
			claim.InsSubNum=PatPlans.GetInsSubNum(patPlanList,1);
			claim.InsSubNum2=PatPlans.GetInsSubNum(patPlanList,2);
			claim.PatRelat=PatPlans.GetRelat(patPlanList,1);
			claim.PatRelat2=PatPlans.GetRelat(patPlanList,2);
			//if(ordinal==1) {
			claim.ClaimType="P";
			//}
			//else {
			//	claim.ClaimType="S";
			//}
			claim.ProvTreat=provTreat;
			claim.IsProsthesis="N";
			claim.ProvBill=Providers.GetBillingProvNum(claim.ProvTreat,0);
			claim.EmployRelated=YN.No;
			ClaimProc cp;
			List<Procedure> procListClaim=new List<Procedure>();//this list will exclude lab fees
			for(int i=0;i<procList.Count;i++) {
				if(procList[i].ProcNumLab==0) {
					procListClaim.Add(procList[i]);
				}
			}
			for(int i=0;i<procListClaim.Count;i++) {
				cp=new ClaimProc();
				ClaimProcs.CreateEst(cp,procListClaim[i],insPlan,sub);
				cp.ClaimNum=claim.ClaimNum;
				cp.Status=ClaimProcStatus.NotReceived;
				cp.CodeSent=ProcedureCodes.GetProcCode(procListClaim[i].CodeNum).ProcCode;
				cp.LineNumber=(byte)(i+1);
				ClaimProcs.Update(cp);
			}
			claimProcList=ClaimProcs.Refresh(pat.PatNum);
			Claims.CalculateAndUpdate(procsForPat,planList,claim,patPlanList,benefitList,pat.Age,subList);
			return claim;
		}

		public static string Run(int scriptNum,string responseExpected,string responseTypeExpected,Claim claim,bool showForms) {
			string retVal="";
			ClaimSendQueueItem queueItem=Claims.GetQueueList(claim.ClaimNum,claim.ClinicNum,0)[0];
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(queueItem.ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			Eclaims.GetMissingData(clearinghouseClin,queueItem);//,out warnings);
			if(queueItem.MissingData!="") {
				return "Cannot send claim until missing data is fixed:\r\n"+queueItem.MissingData+"\r\n";
			}
#if DEBUG
			Canadian.testNumber=scriptNum;
#endif
			long etransNum=Canadian.SendClaim(clearinghouseClin,queueItem,showForms);
			Etrans etrans=Etranss.GetEtrans(etransNum);
			string message=EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum);
			CCDFieldInputter formData=new CCDFieldInputter(message);
			string responseType=formData.GetValue("A04");
			if(responseType!=responseTypeExpected) {
				return "Form type should be "+responseTypeExpected+"\r\n";
			}
			string responseStatus=formData.GetValue("G05");
			if(responseStatus!=responseExpected) {
				return "G05 should be "+responseExpected+"\r\n";
			}
			if(responseExpected=="R" && responseTypeExpected=="11") {
				//so far, only for #6.  We need some other way to test if successful transaction
				string errorMsgCount=formData.GetValue("G06");
				if(errorMsgCount=="00") {
					return "Wrong message count.\r\n";
				}
			}
			retVal+="Claim #"+scriptNum.ToString()+" successful.\r\n";
			return retVal;
		}

		public static string RunOne(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[0]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);
			return Run(1,"C","11",claim,showForms);
		}

		public static string RunTwo(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[1]);
			InsSubTC.SetAssignBen(true,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(2,"","21",claim,showForms);
		}

		public static string RunThree(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[2]);
			InsSubTC.SetAssignBen(true,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);//Even though the test says 1, the example message uses 2
			return Run(3,"","21",claim,showForms);//expecting EOB
		}

		public static string RunFour(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[3]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);
			return Run(4,"","21",claim,showForms);//expecting EOB
		}

		public static string RunFive(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[4]);
			InsSubTC.SetAssignBen(true,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);
			return Run(5,"C","11",claim,showForms);
		}

		public static string RunSix(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[5]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(6,"R","11",claim,showForms);
		}

		public static string RunSeven(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[6]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(7,"","21",claim,showForms);
		}

		public static string RunEight(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[7]);
			InsSubTC.SetAssignBen(true,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);
			return Run(8,"","21",claim,showForms);
		}

		public static string RunNine(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[8]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(9,"","21",claim,showForms);//test the result of the COB
		}

		public static string RunTen(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[9]);
			InsSubTC.SetAssignBen(true,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(10,"","21",claim,showForms);//test the result of the COB
		}

		public static string RunEleven(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[10]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			string oldVersion=CarrierTC.SetCDAnetVersion(claim.PlanNum,"02");
			string retval=Run(11,"","11",claim,showForms);
			CarrierTC.SetCDAnetVersion(claim.PlanNum,oldVersion);
			return retval;
		}

		public static string RunTwelve(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[11]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			string oldVersion=CarrierTC.SetCDAnetVersion(claim.PlanNum,"02");
			string retval=Run(12,"","21",claim,showForms);
			CarrierTC.SetCDAnetVersion(claim.PlanNum,oldVersion);
			return retval;
		}


	}
}
