using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ProcedureLogic_Test {
	[TestClass]
	public class ProcedureLogicTest:TestBase {

		#region ProcedureSortForCandianLabs
		///<summary></summary>
		[TestMethod]
		public void ProcedureLogic_SortProcedures_CanadianLabs_MultipleLabs() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			CultureInfo oldCulture=SetCulture(new CultureInfo("en-CA"));
			Patient pat=PatientT.CreatePatient(suffix);
			List<Procedure> listProcs=new List<Procedure>();
			//Parent and Lab match on all fields except ProcStatus
			Procedure parentProc=ProcedureT.CreateProcedure(pat,"04711",ProcStat.TP,"",91.00);
			listProcs.Add(parentProc);
			Procedure parentProc2=ProcedureT.CreateProcedure(pat,"04712",ProcStat.TP,"",182.00);
			listProcs.Add(parentProc2);
			listProcs.AddRange(CreateAssortedProcs(pat));
			Procedure labProc=ProcedureT.CreateProcedure(pat,"99111",ProcStat.TP,"",89.00,parentProc.ProcDate,procNumLab:parentProc.ProcNum);
			listProcs.Add(labProc);
			Procedure labProc2A=ProcedureT.CreateProcedure(pat,"99111",ProcStat.TP,"",23.00,parentProc2.ProcDate,procNumLab:parentProc2.ProcNum);
			listProcs.Add(labProc2A);
			Procedure labProc2B=ProcedureT.CreateProcedure(pat,"99111",ProcStat.TP,"",15.00,parentProc2.ProcDate,procNumLab:parentProc2.ProcNum);
			listProcs.Add(labProc2B);
			Procedure randomProcedure=ProcedureT.CreateProcedure(pat,"01101",ProcStat.TP,"",105.00);
			ProcedureLogic.SortProcedures(ref listProcs);
			//Get the indexes of all of the parent procedures and their corresponding labs.
			int parentProcIndex=listProcs.FindIndex(x => x==parentProc);
			int labProcIndex=listProcs.FindIndex(x => x==labProc);
			int parentProc2Index=listProcs.FindIndex(x => x==parentProc2);
			int labProc2AIndex=listProcs.FindIndex(x => x==labProc2A);
			int labProc2BIndex=listProcs.FindIndex(x => x==labProc2B);
			//Labs should always be directly after their parent procedure.
			Assert.AreEqual(parentProcIndex+1,labProcIndex);
			Assert.IsTrue(ListTools.In(labProc2AIndex,parentProc2Index+1,parentProc2Index+2));
			Assert.IsTrue(ListTools.In(labProc2BIndex,parentProc2Index+1,parentProc2Index+2));
			SetCulture(oldCulture);
		}

		///<summary>Tests that in a list of various procedures, the sorting logic still places a Canadian Lab under its parent Procedure, 
		///even when the ProcStatus is mismatched between them. As this is a Canadian Lab sorting test, Culture is forced to Canada.</summary>
		[TestMethod]
		public void ProcedureLogic_SortProcedures_CanadianLabs_ProcStatusMismatch() {
			CultureInfo oldCulture=SetCulture(new CultureInfo("en-CA"));
			Patient pat=PatientT.CreatePatient("Test");
			List<Procedure> listProcs=new List<Procedure>();
			//Parent and Lab match on all fields except ProcStatus
			Procedure parentProc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"4", 200.00,procDate:DateTime.Now,priority:0,0,0,0,0,"",0);
			listProcs.Add(parentProc);
			Procedure parentProc2=ProcedureT.CreateProcedure(pat,"D1420",ProcStat.C,"6", 200.00,procDate:DateTime.Now,priority:0,0,0,0,0,"",0);
			listProcs.Add(parentProc2);
			listProcs.AddRange(CreateAssortedProcs(pat));
			Procedure labProc=ProcedureT.CreateProcedure(pat,"99333",ProcStat.TP,"4", 200.00,parentProc.ProcDate,priority:0,0,0,0,0,"",parentProc.ProcNum);
			listProcs.Add(labProc);
			Procedure labProc2=ProcedureT.CreateProcedure(pat,"99222",ProcStat.TP,"6", 120.00,parentProc2.ProcDate,priority:0,0,0,0,0,"",parentProc2.ProcNum);
			listProcs.Add(labProc2);
			ProcedureLogic.SortProcedures(ref listProcs);

			int parentProcIndex=listProcs.FindIndex(x => x==parentProc);
			int labProcIndex=listProcs.FindIndex(x => x==labProc);
			int parentProc2Index=listProcs.FindIndex(x => x==parentProc2);
			int labProc2Index=listProcs.FindIndex(x => x==labProc2);

			Assert.AreEqual(parentProcIndex+1,labProcIndex);//A lab should be directly after its parent
			Assert.AreEqual(parentProc2Index+1,labProc2Index);//A lab should be directly after its parent
			SetCulture(oldCulture);
		}

		///<summary>Tests that in a list of various procedures, the sorting logic still places a Canadian Lab under its parent Procedure, 
		///even when the priorities are mismatched between them. As this is a Canadian Lab sorting test, Culture is forced to Canada.</summary>
		[TestMethod]
		public void ProcedureLogic_SortProcedures_CanadianLabs_PriorityMismatch() {
			CultureInfo oldCulture=SetCulture(new CultureInfo("en-CA"));
			Patient pat=PatientT.CreatePatient("Test");
			List<Procedure> listProcs=CreateAssortedProcs(pat);
			//Parent and Lab match on all fields except ProcStatus
			Procedure parentProc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"4", 200.00,procDate:DateTime.Now,priority:4,0,0,0,0,"",0);
			listProcs.Add(parentProc);
			Procedure labProc=ProcedureT.CreateProcedure(pat,"99333",ProcStat.C,"4", 200.00,parentProc.ProcDate,priority:2,0,0,0,0,"",parentProc.ProcNum);
			listProcs.Add(labProc);
			ProcedureLogic.SortProcedures(ref listProcs);

			int parentProcIndex=listProcs.FindIndex(x => x==parentProc);
			int labProcIndex=listProcs.FindIndex(x => x==labProc);

			Assert.AreEqual(parentProcIndex+1,labProcIndex);//A lab should be directly after its parent
			Assert.AreEqual(labProcIndex-1,parentProcIndex);//A parent should be directly before its lab
			SetCulture(oldCulture);
		}

		///<summary>Tests that in a list of various procedures, the sorting logic still places a Canadian Lab under its parent Procedure, 
		///even when the ToothNums are mismatched between them. As this is a Canadian Lab sorting test, Culture is forced to Canada.</summary>
		[TestMethod]
		public void ProcedureLogic_SortProcedures_CanadianLabs_ToothNumMismatch() {
			CultureInfo oldCulture=SetCulture(new CultureInfo("en-CA"));
			Patient pat=PatientT.CreatePatient("Test");
			List<Procedure> listProcs=CreateAssortedProcs(pat);
			//Parent and Lab match on all fields except ProcStatus
			Procedure parentProc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"10", 200.00,procDate:DateTime.Now,priority:0,0,0,0,0,"",0);
			listProcs.Add(parentProc);
			Procedure labProc=ProcedureT.CreateProcedure(pat,"99333",ProcStat.C,"4", 200.00,parentProc.ProcDate,priority:0,0,0,0,0,"",parentProc.ProcNum);
			listProcs.Add(labProc);
			ProcedureLogic.SortProcedures(ref listProcs);

			int parentProcIndex=listProcs.FindIndex(x => x==parentProc);
			int labProcIndex=listProcs.FindIndex(x => x==labProc);

			Assert.AreEqual(parentProcIndex+1,labProcIndex);//A lab should be directly after its parent
			Assert.AreEqual(labProcIndex-1,parentProcIndex);//A parent should be directly before its lab
			SetCulture(oldCulture);
		}

		///<summary>Tests that in a list of various procedures, the sorting logic still places a Canadian Lab under its parent Procedure, 
		///even when the ToothNums are mismatched between them. As this is a Canadian Lab sorting test, Culture is forced to Canada.</summary>
		[TestMethod]
		public void ProcedureLogic_SortProcedures_CanadianLabs_ProcDateMismatch() {
			CultureInfo oldCulture=SetCulture(new CultureInfo("en-CA"));
			Patient pat=PatientT.CreatePatient("Test");
			List<Procedure> listProcs=CreateAssortedProcs(pat);
			//Parent and Lab match on all fields except ProcStatus
			Procedure parentProc=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"10", 200.00,DateTime.Now,priority:0,0,0,0,0,"",0);
			listProcs.Add(parentProc);
			Procedure labProc=ProcedureT.CreateProcedure(pat,"99333",ProcStat.C,"4", 200.00,DateTime.Now.AddDays(5),priority:0,0,0,0,0,"",parentProc.ProcNum);
			listProcs.Add(labProc);
			ProcedureLogic.SortProcedures(ref listProcs);

			int parentProcIndex=listProcs.FindIndex(x => x==parentProc);
			int labProcIndex=listProcs.FindIndex(x => x==labProc);

			Assert.AreEqual(parentProcIndex+1,labProcIndex);//A lab should be directly after its parent
			Assert.AreEqual(labProcIndex-1,parentProcIndex);//A parent should be directly before its lab
			SetCulture(oldCulture);
		}

		///<summary>A helper method to create 10 Procedures for a given Patient, half with ProcStat.C and half with ProcStat.TP, 
		///each with different priorities, toothnums, dates and procfees. This method is not Canada specific and can be used for all Cultures.</summary>
		private List<Procedure> CreateAssortedProcs(Patient pat) {
			List<Procedure> listProcs=new List<Procedure>();
			for(int i=0;i<5;i++) {
				Procedure newProc=ProcedureT.CreateProcedure(pat,"D2220",ProcStat.C,i.ToString(),i*100.00,DateTime.Now.AddDays(i),i,0,0,0,0,"",0);
				listProcs.Add(newProc);
			}
			for(int i=0;i<5;i++) {
				Procedure newProc=ProcedureT.CreateProcedure(pat,"D2220",ProcStat.TP,i.ToString(),i*100.00,DateTime.Now.AddDays(i),i,0,0,0,0,"",0);
				listProcs.Add(newProc);
			}
			return listProcs;
		}

		///<summary>Takes cultureInfo the current culture info, converts it to what you passed in and returns the old culture info you had previously.
		///This is not in a setup/teardown method because others may come to this class to test Procedure Logic in a non-Canada specific manner. 
		///Tests which need Canada logic should not impact them by forcing the entire class into that culture.</summary>
		private CultureInfo SetCulture(CultureInfo targetCulture) {
			CultureInfo oldCulture=Application.CurrentCulture;
			Application.CurrentCulture=targetCulture;
			CultureInfo.DefaultThreadCurrentCulture=Application.CurrentCulture;
			CultureInfo.DefaultThreadCurrentUICulture=Application.CurrentCulture;
			return oldCulture;
		}
		#endregion

	} 
 }
