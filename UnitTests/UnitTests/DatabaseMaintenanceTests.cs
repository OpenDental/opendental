using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.DatabaseMaintenance_Tests {
	[TestClass]
	public class DatabaseMaintenanceTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		private static List<MethodInfo> _listMethodInfoDbms => (typeof(DatabaseMaintenances)).GetMethods().ToList();

		private static MethodInfo _methodInfoPatientMissing => _listMethodInfoDbms.FirstOrDefault(x => x.Name==nameof(DatabaseMaintenances.PatientMissing));

		///<summary>Runs a single DBM method.  Updates the DateLastRun column in the database for the method passed in if modeCur is set to Fix.</summary>
		private bool RunMethod(MethodInfo method) {
			try {
				List<object> parameters=GetParametersForMethod(method,DbmMode.Fix);
				string resultDoNothing=(string)method.Invoke(null,parameters.ToArray());
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		///<summary>Returns a list of parameters for the corresponding DBM method.  The order of these parameters is critical.</summary>
		private List<object> GetParametersForMethod(MethodInfo method,DbmMode modeCur) {
			long patNum=0;
			DbmMethodAttr methodAttributes=(DbmMethodAttr)Attribute.GetCustomAttribute(method,typeof(DbmMethodAttr));
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			//We always send verbose and modeCur into all DBM methods first.
			List<object> parameters=new List<object>() { false,modeCur };
			//Followed by an optional PatNum for patient specific DBM methods.
			if(methodAttributes.HasPatNum) {
				parameters.Add(patNum);
			}
			return parameters;
		}

		[TestMethod]
		public void DBMTests_PatientMissing_AppiontmentWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum);
			//Create appointment 
			Appointment aptPat=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,opScheduling.OperatoryNum,provNum);
			Appointment aptPatOld=aptPat.Copy();
			aptPat.PatNum=0;
			Appointments.Update(aptPat,aptPatOld);
			//Next, perform the thing you're trying to test.
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			aptPat=Appointments.GetOneApt(aptPat.AptNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,aptPat.PatNum);
			Assert.AreNotEqual(pat.PatNum,aptPat.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_ClaimWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			InsuranceT.AddInsurance(pat,suffix);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan plan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,0);
			BenefitT.CreateDeductible(plan.PlanNum,"D0220",50);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",30);//proc1 - Intraoral - periapical first film
			Claim claim=ClaimT.CreateClaim("P",listPatPlans,listPlans,new List<ClaimProc>(),new List<Procedure> { proc1 },pat,new List<Procedure> { proc1 },
				listBens,listSubs);
			claim.PatNum=0;
			Claims.Update(claim);
			//Next, perform the thing you're trying to test.
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			claim=Claims.GetClaim(claim.ClaimNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,claim.PatNum);
			Assert.AreNotEqual(pat.PatNum,claim.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_ClaimprocWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			InsuranceT.AddInsurance(pat,suffix);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan plan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,0);
			BenefitT.CreateDeductible(plan.PlanNum,"D0220",50);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",30);//proc1 - Intraoral - periapical first film
			Claim claim=ClaimT.CreateClaim("P",listPatPlans,listPlans,new List<ClaimProc>(),new List<Procedure> { proc1 },pat,new List<Procedure> { proc1 },
				listBens,listSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForProc(proc1.ProcNum);
			//Set the PatNum=0 and update the claimproc
			ClaimProc claimProc=listClaimProcs.FirstOrDefault();
			ClaimProc claimProcOld=claimProc.Copy();
			claimProc.PatNum=0;
			ClaimProcs.Update(claimProc,claimProcOld);
			//Next, perform the thing you're trying to test.
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			//Get the updated claimproc
			listClaimProcs=ClaimProcs.RefreshForProc(claimProc.ProcNum);
			claimProc=ClaimProcs.GetFromList(listClaimProcs,claimProc.ClaimProcNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,claimProc.PatNum);
			Assert.AreNotEqual(pat.PatNum,claimProc.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_CommlogWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			Commlog commlog=CommlogT.CreateCommlog(pat.PatNum);
			//Next, perform the thing you're trying to test.
			Commlog commlogOld=commlog.Copy();
			commlog.PatNum=0;
			Commlogs.Update(commlog,commlogOld);
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			commlog=Commlogs.GetOne(commlog.CommlogNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,commlog.PatNum);
			Assert.AreNotEqual(pat.PatNum,commlog.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_DocumentWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			Document doc=Documents.InsertAndGet(new Document() { PatNum=pat.PatNum,Description=$"Test {suffix}" },pat);
			//Next, perform the thing you're trying to test.
			Document procOld=doc.Copy();
			doc.PatNum=0;
			Documents.Update(doc,procOld);
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			doc=Documents.GetByNum(doc.DocNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,doc.PatNum);
			Assert.AreNotEqual(pat.PatNum,doc.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_ProcedureWithZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",125);
			//Next, perform the thing you're trying to test.
			Procedure procOld=proc.Copy();
			proc.PatNum=0;
			Procedures.Update(proc,procOld);
			//Then, get anything necessary from the database to see if the test is correct.
			DatabaseMaintenances.PatientMissing();
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			//Finally, use one or more asserts to verify the results.
			Assert.AreNotEqual(0,proc.PatNum);
			Assert.AreNotEqual(pat.PatNum,proc.PatNum);
		}

		[TestMethod]
		public void DBMTests_PatientMissing_ProcedureWithNonZeroPatNum() {
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",125);
			//Next, perform the thing you're trying to test.
			string command=$"DELETE FROM patient WHERE PatNum={pat.PatNum}";
			DataCore.NonQ(command);
			Patient patProc=Patients.GetPat(proc.PatNum);
			//Then, get anything necessary from the database to see if the test is correct.
			Assert.IsNull(patProc);
			DatabaseMaintenances.PatientMissing();
			patProc=Patients.GetPat(proc.PatNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(pat.PatNum,patProc.PatNum);
		}

		[TestMethod]
		public void DMBTests_FixSpecialChars() {
			//Arrange
			char invalidChar=(char)1;
			char nullChar=(char)0;
			string apptNoteExpected="a note without invalid chars.";
			string patAddrNoteExpected="Address Note without invalid chars.";
			string apptProcDescriptExpected="D0110";
			string procSurfExpected="O";
			Patient pat=PatientT.CreatePatient();
			Patient patOld=pat.Copy();
			pat.AddrNote=patAddrNoteExpected+invalidChar;
			Patients.Update(pat,patOld);
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,1,1,aptNote:apptNoteExpected+invalidChar+nullChar);
			Appointment apptOld=appt.Copy();
			appt.ProcDescript=apptProcDescriptExpected+invalidChar;
			Appointments.Update(appt,apptOld);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"1",0,surf:procSurfExpected+invalidChar);
			Dictionary<string,string> dictExpected=new Dictionary<string,string>();
			Random rnd=new Random();
			for(int i = 0;i<DatabaseMaintenances.ListTableAndColumns.Count;i+=2) {
				string tableName=DatabaseMaintenances.ListTableAndColumns[i];
				string columnName=DatabaseMaintenances.ListTableAndColumns[i+1];
				string guid=Guid.NewGuid().ToString().Replace("-","").Substring(0,30);//No -'s, max 30 chars (patient.WirelessPhone)
				//patientnote doesn't have an auto-increment primary key.
				string pkCol=tableName=="patientnote" ? "PatNum," : "";
				string pkVal=tableName=="patientnote" ? $"{rnd.Next(1,int.MaxValue)}," : "";
				string command="INSERT INTO "+tableName+" ("+pkCol+columnName+") VALUES("+pkVal+"CONCAT('"+POut.String(guid)+"',CHAR(0)))";
				DataCore.NonQ(command);
				dictExpected[tableName+columnName]=guid;
			}
			//Act
			DatabaseMaintenances.FixSpecialCharacters();
			pat=Patients.GetPat(pat.PatNum);
			appt=Appointments.GetOneApt(appt.AptNum);
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			//Assert
			Assert.AreEqual(apptNoteExpected,appt.Note);
			Assert.AreEqual(apptProcDescriptExpected,appt.ProcDescript);
			Assert.AreEqual(patAddrNoteExpected,pat.AddrNote);
			Assert.AreEqual(procSurfExpected,proc.Surf);
			for(int i = 0;i<DatabaseMaintenances.ListTableAndColumns.Count;i+=2) {
				string tableName=DatabaseMaintenances.ListTableAndColumns[i];
				string columnName=DatabaseMaintenances.ListTableAndColumns[i+1];
				string command="SELECT COUNT(*) FROM "+tableName+" WHERE "+columnName+"='"+POut.String(dictExpected[tableName+columnName])+"'";
				int count=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
				Assert.AreEqual(1,count,$"{tableName}.{columnName}={dictExpected[tableName+columnName]} not found.");
			}
		}

		[TestMethod]
		public void DBMTest_ToothChartInvalidDrawingSegments_RemoveRowsNoPatNum() {
			ToothInitialT.ClearTable();
			Patient pat=PatientT.CreatePatient();
			//valid drawing segments
			ToothInitialT.CreateToothInitial(pat,"366,105;366,106;366,107;366,108;366,109;367,110;368,110;367,110");
			ToothInitialT.CreateToothInitial(pat,"22.5,15.78;1.1,2.2");
			ToothInitialT.CreateToothInitial(pat,"20,25");
			ToothInitialT.CreateToothInitial(pat,"-1,-3;0,-1");
			ToothInitialT.CreateToothInitial(pat,"-1.1,-2.3;4.3,2.2");
			//invalid drawing segments
			ToothInitialT.CreateToothInitial(pat,"83,;366,106;366,107;366,108");//first y missing
			ToothInitialT.CreateToothInitial(pat,",22;366,106;366,107;366,108");//first x missing
			ToothInitialT.CreateToothInitial(pat,"366,105;366,106;366,107;366,108;,109;367,110;368,110;367,110");//one middle coordinate missing
			ToothInitialT.CreateToothInitial(pat,"112,273;111,;110,;,273;,273;,273;105,;,273;103,;102,273");//many middle coordinates missing
			ToothInitialT.CreateToothInitial(pat,",273;111,273.5;,273;109,;108.2,273;,273;105,;104,;,273;102,273");//first x missing and many middle
			ToothInitialT.CreateToothInitial(pat,"112,;111,273;,273;,273;,273;107,;105,273;104,;103,273;102,273");//first y missing and many middle
			ToothInitialT.CreateToothInitial(pat,"108,273;107,273.77;105,273;104,273.12;103,273;,273.6");//last x missing
			ToothInitialT.CreateToothInitial(pat,"108,273;107,273;105,273;104,273;103,273;201,");//last y missing
			ToothInitialT.CreateToothInitial(pat,"108,273;,273;,273;,273;103,;,273");//last x missing and many middle
			ToothInitialT.CreateToothInitial(pat,"366,109;,110;108,273;107,273;,273;,273;103,273;102,");//last y missing and many middle
			ToothInitialT.CreateToothInitial(pat,",105;366,106;366,;366,;366,;367,;368,110;367,");//first x, last y, many middle missing
			string command="SELECT COUNT(*) FROM toothinitial " +
				"WHERE DrawingSegment LIKE '%,;%' OR DrawingSegment LIKE '%;,%'" +//middle coordinates are incorrect
				"OR DrawingSegment LIKE ',%'" +//first coordinate is incorrect
				"OR DrawingSegment LIKE '%,'";//last coordinate is incorrect
			int numBadRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(11,numBadRows);
			DatabaseMaintenances.ToothChartInvalidDrawingSegments(false,DbmMode.Fix);
			numBadRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(0,numBadRows);
			command="SELECT COUNT(*) FROM toothinitial";
			int numGoodRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(5,numGoodRows);
			ToothInitialT.ClearTable();
			PatientT.ClearPatientTable();
		}

		[TestMethod]
		public void DBMTest_ToothChartInvalidDrawingSegments_RemoveRowsWithPatNum() {
			ToothInitialT.ClearTable();
			Patient pat1=PatientT.CreatePatient();
			Patient pat2=PatientT.CreatePatient();
			//valid drawing segments
			ToothInitialT.CreateToothInitial(pat1,"366,105;366,106;366,107;366,108;366,109;367,110;368,110;367,110");
			ToothInitialT.CreateToothInitial(pat1,"22.5,15.78;1.1,2.2");
			ToothInitialT.CreateToothInitial(pat1,"20,25");
			ToothInitialT.CreateToothInitial(pat2,"-1,-3;0,-1");
			ToothInitialT.CreateToothInitial(pat2,"-1.1,-2.3;4.3,2.2");
			//invalid drawing segments
			ToothInitialT.CreateToothInitial(pat1,"83,;366,106;366,107;366,108");//first y missing
			ToothInitialT.CreateToothInitial(pat1,",22;366,106;366,107;366,108");//first x missing
			ToothInitialT.CreateToothInitial(pat1,"366,105;366,106;366,107;366,108;,109;367,110;368,110;367,110");//one middle coordinate missing
			ToothInitialT.CreateToothInitial(pat1,"112,273;111,;110,;,273;,273;,273;105,;,273;103,;102,273");//many middle coordinates missing
			ToothInitialT.CreateToothInitial(pat1,",273;111,273.5;,273;109,;108.2,273;,273;105,;104,;,273;102,273");//first x missing and many middle
			ToothInitialT.CreateToothInitial(pat2,"112,;111,273;,273;,273;,273;107,;105,273;104,;103,273;102,273");//first y missing and many middle
			ToothInitialT.CreateToothInitial(pat2,"108,273;107,273.77;105,273;104,273.12;103,273;,273.6");//last x missing
			ToothInitialT.CreateToothInitial(pat2,"108,273;107,273;105,273;104,273;103,273;201,");//last y missing
			ToothInitialT.CreateToothInitial(pat2,"108,273;,273;,273;,273;103,;,273");//last x missing and many middle
			ToothInitialT.CreateToothInitial(pat2,"366,109;,110;108,273;107,273;,273;,273;103,273;102,");//last y missing and many middle
			ToothInitialT.CreateToothInitial(pat2,",105;366,106;366,;366,;366,;367,;368,110;367,");//first x, last y, many middle missing
			string command="SELECT COUNT(*) FROM toothinitial " +
				"WHERE (DrawingSegment LIKE '%,;%' OR DrawingSegment LIKE '%;,%'" +//middle coordinates are incorrect
				"OR DrawingSegment LIKE ',%'" +//first coordinate is incorrect
				"OR DrawingSegment LIKE '%,')" +//last coordinate is incorrect
				"AND PatNum="+pat2.PatNum+" ";
			int numBadRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(6,numBadRows);
			DatabaseMaintenances.ToothChartInvalidDrawingSegments(false,DbmMode.Fix,pat2.PatNum);
			numBadRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(0,numBadRows);
			command="SELECT COUNT(*) FROM toothinitial WHERE PatNum="+pat2.PatNum;
			int numGoodRows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(2,numGoodRows);
			command="SELECT COUNT(*) FROM toothinitial WHERE PatNum="+pat1.PatNum;
			int numPat1Rows=PIn.Int(DataCore.GetTable(command).Rows[0][0].ToString());
			Assert.AreEqual(8,numPat1Rows);
			ToothInitialT.ClearTable();
			PatientT.ClearPatientTable();
		}

		#region NotesWithTooMuchWhiteSpace Tests
		//This list is from the DatabaseMaintenance method these test are covering. Advise any future coders to check that this list
		//still matches in the event these tests fail and to add to it if we need to
		//var listTablesAndColumns=new[] {
		//	new { tableName="appointment",columnName="Note",key="AptNum" },
		//	new { tableName="commlog",columnName="Note",key="CommlogNum" },
		//	new { tableName="procnote",columnName="Note",key="ProcNoteNum" },
		//	new { tableName="patient",columnName="FamFinUrgNote",key="PatNum" },
		//	new { tableName="patfield",columnName="FieldValue",key="PatFieldNum" },
		//	new { tableName="insplan",columnName="PlanNote",key="PlanNum"},
		//};
		

		[TestMethod]
		public void DBMTests_NotesWithTooMuchWhiteSpace_ConsecutiveSpaces() {
			string valid="valid data ";//space left at the end for how we maintain keeping spaces between words when needed
			string command="INSERT INTO commlog (Note) VALUES('"+valid+string.Join("",Enumerable.Repeat(string.Join("",Enumerable.Repeat("\t",25))+"\n",25))+"')";
			long pk=DataCore.NonQ(command,getInsertID:true);
			Commlog commlog=Commlogs.GetOne(pk);
			Assert.AreNotEqual(valid,commlog.Note);
			DatabaseMaintenances.NotesWithTooMuchWhiteSpace(false,DbmMode.Fix);
			commlog=Commlogs.GetOne(pk);
			Assert.AreEqual(valid,commlog.Note);
		}

		[TestMethod]
		public void DBMTests_NotesWithTooMuchWhiteSpace_Tabs() {
			string valid="valid data";
			string command="INSERT INTO appointment (Note) VALUES('"+valid+string.Join("",Enumerable.Repeat("\t",50))+"')";
			long pk=DataCore.NonQ(command,getInsertID:true);
			Appointment appt=Appointments.GetOneApt(pk);
			Assert.AreNotEqual(valid,appt.Note);
			DatabaseMaintenances.NotesWithTooMuchWhiteSpace(false,DbmMode.Fix);
			appt=Appointments.GetOneApt(pk);
			Assert.AreEqual(valid,appt.Note);
		}

		[TestMethod]
		public void DBMTests_NotesWithTooMuchWhiteSpace_NewLinesAndCarriageReturns() {
			string valid="valid data";
			string command="INSERT INTO patient (FamFinUrgNote) VALUES('"+valid+string.Join("",Enumerable.Repeat("\r\n",50))+"')";
			long pkForCarriageReturn=DataCore.NonQ(command,getInsertID:true);
			Patient patNoteWithCarriageReturns=Patients.GetPat(pkForCarriageReturn);
			command="INSERT INTO patient (FamFinUrgNote) VALUES('"+valid+string.Join("",Enumerable.Repeat("\n",50))+"')";
			long pkForNewLine=DataCore.NonQ(command,getInsertID:true);
			Patient patNoteWithNewLines=Patients.GetPat(pkForNewLine);
			Assert.AreNotEqual(valid,patNoteWithCarriageReturns.FamFinUrgNote);
			Assert.AreNotEqual(valid,patNoteWithNewLines.FamFinUrgNote);
			DatabaseMaintenances.NotesWithTooMuchWhiteSpace(false,DbmMode.Fix);
			patNoteWithNewLines=Patients.GetPat(pkForCarriageReturn);
			patNoteWithCarriageReturns=Patients.GetPat(pkForNewLine);
			Assert.AreEqual(valid+"\r\n",patNoteWithNewLines.FamFinUrgNote);//expecting a carraige return and a new line
			Assert.AreEqual(valid+"\r\n",patNoteWithCarriageReturns.FamFinUrgNote);//expecting one carriage return and a new line
		}

		[TestMethod]
		public void DBMTests_NotesWithTooMuchWhiteSpace_TooManySpaces() {
			string valid="valid data";
			string command="INSERT INTO appointment (Note) VALUES ('"+valid+string.Join("",Enumerable.Repeat(@" ",500))+"')";
			long pk=DataCore.NonQ(command,getInsertID:true);
			Appointment apptWithSpaces=Appointments.GetOneApt(pk);
			Assert.AreNotEqual(valid,apptWithSpaces.Note);
			DatabaseMaintenances.NotesWithTooMuchWhiteSpace(false,DbmMode.Fix);
			apptWithSpaces=Appointments.GetOneApt(pk);
			Assert.AreEqual(valid,apptWithSpaces.Note);
		}

		[TestMethod]
		public void DBMTests_ProcedureLogWithInvalidAptNum_HappyPath() {
			Patient pat=PatientT.CreatePatient();
			Appointment schedAptToDelete=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(2022,2,12),0,0,aptStatus:ApptStatus.Scheduled);
			Appointment plannedAptNoPlannedStatus=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(2022,2,12),0,0,aptStatus:ApptStatus.Scheduled);
			Procedure procWithInvalidPlannedApt=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.TP,"30",0,plannedAptNum:plannedAptNoPlannedStatus.AptNum,doInsert:false);
			procWithInvalidPlannedApt.AptNum=schedAptToDelete.AptNum;
			Procedures.Insert(procWithInvalidPlannedApt);
			Assert.IsTrue(procWithInvalidPlannedApt.PlannedAptNum!=0);
			string command="SELECT ProcNum "
						+"FROM procedurelog "
						+"WHERE (AptNum NOT IN(SELECT AptNum FROM appointment) AND AptNum!=0) "
						+"OR ("
							+"(PlannedAptNum NOT IN(SELECT AptNum FROM appointment)  "
							+"OR (PlannedAptNum IN(SELECT AptNum FROM appointment WHERE AptStatus!=6))"
						+") "
						+"AND PlannedAptNum!=0)";
			DataTable problemRows=DataCore.GetTable(command);
			Assert.IsTrue(problemRows.Rows.Count==1);//right now the only problem is that the PlannedAptNum is linked to an appoint whose status is not 'Planned'
			DatabaseMaintenances.ProcedurelogWithInvalidAptNum(false,DbmMode.Fix);
			problemRows=DataCore.GetTable(command);
			Assert.IsTrue(problemRows.Rows.Count==0);
			Procedure procAfterDbm=Procedures.GetOneProc(procWithInvalidPlannedApt.ProcNum,false);
			Assert.IsTrue(procAfterDbm.PlannedAptNum==0);
			Assert.IsTrue(procAfterDbm.AptNum==schedAptToDelete.AptNum);
			Appointments.Delete(schedAptToDelete.AptNum);//This method is supposed to set any aptnums to 0 if an appointment is deleted so we need to reset the aptnum
			Procedure procWithInvalidAptOld=Procedures.GetOneProc(procWithInvalidPlannedApt.ProcNum,false);
			procWithInvalidPlannedApt.AptNum=schedAptToDelete.AptNum;
			Procedures.Update(procWithInvalidPlannedApt,procWithInvalidAptOld);
			problemRows=DataCore.GetTable(command);
			Assert.IsTrue(problemRows.Rows.Count==1);//expect the PlannedAptNum to be invalid now that we have deleted it from the database. This should fix that.
			DatabaseMaintenances.ProcedurelogWithInvalidAptNum(false,DbmMode.Fix);
			problemRows=DataCore.GetTable(command);
			Assert.IsTrue(problemRows.Rows.Count==0);
		}
		#endregion
	}
}
