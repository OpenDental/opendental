using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDental;
using OpenDentBusiness;
using OpenDental.Eclaims;

namespace TestCanada {
	class PredeterminationTC {

		///<summary>Remember that this is 0-based.  So subtract 1 from the script number to get the index in this list.</summary>
		public static List<long> ClaimNums;

		public static string CreateAllPredeterminations() {
			ClaimNums=new List<long>();
			CreateOne();
			CreateTwo();
			CreateThree();
			CreateFour();
			CreateFive();
			CreateSix_1();
			CreateSix_2();
			CreateSeven_1();
			CreateSeven_2();
			CreateEight();
			return "Predetermination objects set.\r\n";
		}

		private static void CreateOne() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum1);//patient#1, Lisa Fête"
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"11","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="X";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateTwo() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum2);//patient#2, John Smith
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"41","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="XI";//N=XI
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
			claim.CanadianDateInitialLower=new DateTime(1984,04,07);
			claim.CanadianMandProsthMaterial=4;
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateThree() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum4);//patient#4, John Smith
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"41","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="MI";//L=MI
			//billing prov already handled
			claim.CanadianReferralProviderNum="";
			claim.CanadianReferralReason=0;
			//pat.SchoolName
			//assignBen can't be set here because it changes per claim in the scripts
			claim.AccidentDate=new DateTime(1997,03,02);
			claim.PreAuthString="";
			claim.CanadianIsInitialUpper="X";
			claim.CanadianDateInitialUpper=DateTime.MinValue;
			claim.CanadianIsInitialLower="Y";
			claim.CanadianDateInitialLower=DateTime.MinValue;
			claim.IsOrtho=true;
			claim.CanadaEstTreatStartDate=new DateTime(1999,04,01);
			claim.CanadaInitialPayment=1000;
			claim.CanadaPaymentMode=3;
			claim.CanadaTreatDuration=48;
			claim.CanadaNumAnticipatedPayments=16;
			claim.CanadaAnticipatedPayAmount=200;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateFour() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum5);//patient#5, Bob L Howard
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"21","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="I";
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
			claim.IsOrtho=false;
			claim.CanadianReferralProviderNum="081234500";
			claim.CanadianReferralReason=4;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateFive() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum7);//patient#7, Madeleine Arpege
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"21","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="M";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSix_1() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum7);//patient#7, Madeleine Arpege
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"21","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"25","MIV",107.6,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"14","DIV",107.6,"X",provNum));
			proc=ProcTC.AddProc("27211",pat.PatNum,new DateTime(1999,1,1),"24","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",238,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			proc=ProcTC.AddProc("27213",pat.PatNum,new DateTime(1999,1,1),"26","",450,"E",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",210,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",35,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="M";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSix_2() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum7);//patient#7, Madeleine Arpege
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("39202",pat.PatNum,new DateTime(1999,1,1),"36","",67.5,"X",provNum));
			procList.Add(ProcTC.AddProc("32222",pat.PatNum,new DateTime(1999,1,1),"32","",65,"X",provNum));
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="M";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSeven_1() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum3);//patient#3, Mary Walls
			Procedure proc;
			Procedure procLab;
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"X",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"X",provNum));
			proc=ProcTC.AddProc("67301",pat.PatNum,new DateTime(1999,1,1),"21","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",300,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",40,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"25","MIV",107.6,"X",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"14","DIV",107.6,"X",provNum));
			proc=ProcTC.AddProc("27211",pat.PatNum,new DateTime(1999,1,1),"24","",450,"X",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",238,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			proc=ProcTC.AddProc("27213",pat.PatNum,new DateTime(1999,1,1),"26","",450,"E",provNum);
			procList.Add(proc);
			procLab=ProcTC.AddProc("99111",pat.PatNum,new DateTime(1999,1,1),"","",210,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			procLab=ProcTC.AddProc("99222",pat.PatNum,new DateTime(1999,1,1),"","",35,"",provNum);
			ProcTC.AttachLabProc(proc.ProcNum,procLab);
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="M";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateSeven_2() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum3);//patient#3, Mary Walls
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("39202",pat.PatNum,new DateTime(1999,1,1),"36","",67.5,"X",provNum));
			procList.Add(ProcTC.AddProc("32222",pat.PatNum,new DateTime(1999,1,1),"32","",65,"X",provNum));
			Claim claim=CreatePredetermination(pat,procList,provNum);
			claim.CanadianMaterialsForwarded="M";
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static void CreateEight() {
			long provNum=ProviderC.ListShort[0].ProvNum;//dentist#1
			Patient pat=Patients.GetPat(PatientTC.PatNum9);//patient#9, Fred Smith
			List<Procedure> procList=new List<Procedure>();
			procList.Add(ProcTC.AddProc("01201",pat.PatNum,new DateTime(1999,1,1),"","",27.5,"",provNum));
			procList.Add(ProcTC.AddProc("02102",pat.PatNum,new DateTime(1999,1,1),"","",87.25,"",provNum));
			procList.Add(ProcTC.AddProc("21223",pat.PatNum,new DateTime(1999,1,1),"25","MIV",107.60,"",provNum));
			Claim claim=CreatePredetermination(pat,procList,provNum);
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
			claim.IsOrtho=false;
			Claims.Update(claim);
			ClaimNums.Add(claim.ClaimNum);
		}

		private static Claim CreatePredetermination(Patient pat,List<Procedure> procList,long provTreat) {
		  Family fam=Patients.GetFamily(pat.PatNum);
		  List<InsSub> subList=InsSubs.RefreshForFam(fam);
		  List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
		  List<PatPlan> patPlanList=PatPlans.Refresh(pat.PatNum);
		  List<Benefit> benefitList=Benefits.Refresh(patPlanList,subList);
		  List<ClaimProc> claimProcList=ClaimProcs.Refresh(pat.PatNum);
		  List<Procedure> procsForPat=Procedures.Refresh(pat.PatNum);
			InsSub sub=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,1),subList);
		  InsPlan insPlan=InsPlans.GetPlan(sub.PlanNum,planList);
		  Claim claim=new Claim();
		  Claims.Insert(claim);//to retreive a key for new Claim.ClaimNum
		  claim.PatNum=pat.PatNum;
		  claim.DateService=procList[0].ProcDate;
		  claim.DateSent=DateTime.Today;
		  claim.ClaimStatus="W";
			claim.InsSubNum=PatPlans.GetInsSubNum(patPlanList,1);
		  claim.InsSubNum2=PatPlans.GetInsSubNum(patPlanList,2);
			InsSub sub1=InsSubs.GetSub(claim.InsSubNum,subList);
			InsSub sub2=InsSubs.GetSub(claim.InsSubNum,subList);
		  claim.PlanNum=sub1.PlanNum;
			claim.PlanNum2=sub2.PlanNum;
		  claim.PatRelat=PatPlans.GetRelat(patPlanList,1);
		  claim.PatRelat2=PatPlans.GetRelat(patPlanList,2);
		  claim.ClaimType="PreAuth";
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

		public static string Run(int scriptNum,string responseExpected,string responseTypeExpected,Claim claim,bool showForms,int pageNumber,int lastPageNumber,double firstExamFee,double diagnosticPhaseFee) {
		  string retVal="";
		  ClaimSendQueueItem queueItem=Claims.GetQueueList(claim.ClaimNum,claim.ClinicNum,0)[0];
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(queueItem.ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			Eclaims.GetMissingData(clearinghouseClin,queueItem);//,out warnings);
			if(queueItem.MissingData!="") {
				return "Cannot send predetermination until missing data is fixed:\r\n"+queueItem.MissingData+"\r\n";
		  }
#if DEBUG
			Canadian.testNumber=scriptNum;
			claim.PreAuthString=""+pageNumber+","+lastPageNumber+","+firstExamFee+","+diagnosticPhaseFee;
#endif
		  long etransNum=Canadian.SendClaim(clearinghouseClin,queueItem,showForms);
		  Etrans etrans=Etranss.GetEtrans(etransNum);
		  string message=EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum);
		  CCDFieldInputter formData=new CCDFieldInputter(message);
		  string responseType=formData.GetValue("A04");
		  if(responseType!=responseTypeExpected) {
		    return "Form type is '"+responseType+"' but should be '"+responseTypeExpected+"'\r\n";
		  }
		  string responseStatus=formData.GetValue("G05");
		  if(responseStatus!=responseExpected) {
		    return "G05 is '"+responseStatus+"' but should be '"+responseExpected+"'\r\n";
		  }
		  if(responseExpected=="R" && responseTypeExpected=="11") {
		    //so far, only for #6.  We need some other way to test if successful transaction
		    string errorMsgCount=formData.GetValue("G06");
		    if(errorMsgCount=="00") {
		      return "Wrong message count.\r\n";
		    }
		  }
		  retVal+="Predetermination #"+scriptNum+" page "+pageNumber+" of "+lastPageNumber+" successful.\r\n";
		  return retVal;
		}

		public static string RunOne(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[0]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(1,"C","13",claim,showForms,1,1,0,0);
		}

		public static string RunTwo(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[1]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(2,"","23",claim,showForms,1,1,0,0);
		}

		public static string RunThree(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[2]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(3,"","23",claim,showForms,1,1,350,250);
		}

		public static string RunFour(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[3]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,2);
			return Run(4,"","23",claim,showForms,1,1,0,0);
		}

		public static string RunFive(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[4]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(5,"","13",claim,showForms,1,1,0,0);
		}

		public static string RunSix(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[5]);//Claim 6 page 1
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			string result=Run(6,"","13",claim,showForms,1,2,0,0);
			claim=Claims.GetClaim(ClaimNums[6]);//Claim 6 page 2
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			result+=Environment.NewLine+Run(6,"","13",claim,showForms,2,2,0,0);
			return result;
		}

		public static string RunSeven(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[7]);//Claim 7 page 1
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			string result=Run(7,"","13",claim,showForms,1,2,0,0);
			claim=Claims.GetClaim(ClaimNums[8]);//Claim 7 page 2
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			result+=Environment.NewLine+Run(7,"","13",claim,showForms,2,2,0,0);
			return result;
		}

		public static string RunEight(bool showForms) {
			Claim claim=Claims.GetClaim(ClaimNums[9]);
			InsSubTC.SetAssignBen(false,claim.InsSubNum);
			CarrierTC.SetEncryptionMethod(claim.PlanNum,1);
			return Run(8,"C","13",claim,showForms,1,1,0,0);
		}

	}
}
