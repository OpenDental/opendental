using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class TreatPlanT {

		///<summary>Creates a TreatPlan (Saved, Active, or Inactive). Also creates TreatPlanAttaches for "Active" and "Inactive" TreatPlanStatus or ProcTPs for "Saved" TreatPlanStatus for each procedures in listProcedures. daysPrevious updates SecDateTEdit. Ex: daysPrevious=7 will be a week ago.</summary>
		public static TreatPlan CreateTreatPlan(long patNum,string heading="Active Treatment Plan",List<Procedure> listProcedures=null,TreatPlanStatus treatPlanStatus=TreatPlanStatus.Active,int daysPrevious=0,string note=null) {
			TreatPlan treatplan=new TreatPlan() {
				PatNum=patNum,
				DateTP=DateTime.MinValue,
				Heading=heading,
				Note=note??PrefC.GetString(PrefName.TreatmentPlanNote),
				TPStatus=treatPlanStatus,
				TPType=DiscountPlanSubs.HasDiscountPlan(patNum) ? TreatPlanType.Discount : TreatPlanType.Insurance
			};
			treatplan.TreatPlanNum=TreatPlans.Insert(treatplan);
			treatplan=TreatPlans.GetOne(treatplan.TreatPlanNum);//Make sure treatplans matches Db. Avoids problems with DateTime columns.
			//Create TreatPlanAttach for each procedure when TreatPlanStatus is Active or Inactive
			if(treatPlanStatus!=TreatPlanStatus.Saved && listProcedures!=null) {
				for(int i=0;i<listProcedures.Count;i++) {
					TreatPlanAttaches.Insert(new TreatPlanAttach() { TreatPlanNum=treatplan.TreatPlanNum,ProcNum=listProcedures[i].ProcNum,Priority=0 });
				}
			}
			//Create ProcTPs for each procedure when TreatPlanStatus Saved.
			if(treatPlanStatus==TreatPlanStatus.Saved && listProcedures!=null) {
				ProcTpT.CreateProcTpsForSavedTreatPlan(treatplan.TreatPlanNum,listProcedures);
			}
			if(daysPrevious>0) { //prevents a double negative.
				SetSecDateTEditToDaysPast(treatplan,daysPrevious);
			}
			return treatplan;
		}

		///<summary>Deletes everything from the TreatPlan table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTreatPlanTable() {
			string command="DELETE FROM treatplan";
			DataCore.NonQ(command);
		}

		///<summary>Deletes everything from the TreatPlanAttach table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTreatPlanAttachTable() {
			string command="DELETE FROM treatplanattach";
			DataCore.NonQ(command);
		}

		///<summary>Creates and returns a list of 3 TreatPlans for each patient in listPatients with all fields utilized and varied. Useful for testing a TreatPlan search for different fields.</summary>
		public static List<TreatPlan> CreateVariedTreatPlanSet(List<OpenDentBusiness.Patient> listPatients) {
			ProcTpT.ClearProcTpTable();
			ClearTreatPlanTable();
			ClearTreatPlanAttachTable();
			long provNum=ProviderT.CreateProvider("T'Ana");
			List<TreatPlan> listTreatPlan=new List<TreatPlan>();
			int daysPrevious=14;
			for(int i=0;i<listPatients.Count;i++) {
				#region Create Procedures for Active TreatPlan
				List<OpenDentBusiness.Procedure>listOdbProcedures=new List<OpenDentBusiness.Procedure>();
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"Pano",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:35.99,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"SpMRemUni",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:79.56,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"MaxImmDent",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:567.99,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				#endregion+
				//Create Active TreatPlan
				listTreatPlan.Add(TreatPlanT.CreateTreatPlan(patNum:listPatients[i].PatNum,heading:"Active Treatment Plan",listOdbProcedures,TreatPlanStatus.Active,daysPrevious++));
				#region Create Procedures for Inactive TreatPlan
				listOdbProcedures=new List<OpenDentBusiness.Procedure>();
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"Sentient Tooth",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:350,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"Q-damage",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:5350,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				#endregion
				//Create InActive TreatPlan
				listTreatPlan.Add(TreatPlanT.CreateTreatPlan(patNum:listPatients[i].PatNum,heading:"Inactive Treatment Plan",listOdbProcedures,TreatPlanStatus.Inactive,daysPrevious++));
				#region Create Procedures for Saved TreatPlan
				listOdbProcedures=new List<OpenDentBusiness.Procedure>();
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"AddTooth",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:996.12,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				listOdbProcedures.Add(ProcedureT.CreateProcedure(
					pat:listPatients[i],
					procCodeStr:"DentAttach",
					procStatus:OpenDentBusiness.ProcStat.TP,
					toothNum:"",
					procFee:857.54,
					procDate:DateTime.Today,
					priority:0,
					plannedAptNum:0,
					provNum:provNum,
					aptNum:0,
					baseUnits:0,
					surf:"",
					procNumLab:0,
					doInsert:true,
					discount:0,
					clinicNum:0
				));
				#endregion
				//Create Saved TreatPlan
				listTreatPlan.Add(TreatPlanT.CreateTreatPlan(patNum:listPatients[i].PatNum,heading:"Saved Treatment Plan",listOdbProcedures,TreatPlanStatus.Saved,daysPrevious++));
			}
			return listTreatPlan;
		}

		///<summary>Updates the SecDateTEdit of a TreatPlan to the number of daysPrevious. Unable to set the SecDateTEdit to anything but DateTime.Now in the CRUD Layer.</summary>
		public static void SetSecDateTEditToDaysPast(TreatPlan treatPlan,int daysPrevious) {
			string command="UPDATE treatplan SET SecDateTEdit = "+POut.Date(DateTime.Now.AddDays(-daysPrevious))
				+" WHERE TreatPlanNum = "+POut.Long(treatPlan.TreatPlanNum);
			DataCore.NonQ(command);
    }


	}
}
