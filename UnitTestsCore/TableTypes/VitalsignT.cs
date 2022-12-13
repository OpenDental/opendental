using System;
using System.Collections.Generic;
using OpenDentBusiness;
using OpenDentBusiness.Crud;

namespace UnitTestsCore {
	public class VitalsignT {

		public static void ClearVitalsignTable() {
			string command="DELETE from vitalsign";
			DataCore.NonQ(command);
		}

		public static Vitalsign CreateAndInsertVitalSign(long patNum,int height=0,int weight=0,int bpSystolic=0,int bpDiastolic=0,DateTime date=default(DateTime),
				bool hasFollowUpPlan=false,bool isIneligible=false,string documentation="",bool childGotNutrition=false,bool childGotPhysCouns=false,
				WeightUnitType weightCode=default(WeightUnitType),string heightExamCode="",string weightExamCode="",string bmiExamCode="",int ehrNotPerformedNum=0,
				long pregDiseaseNum=0,int bmiPercentile=0,int pulse=0) {
			Vitalsign vitalsign=CreateVitalsign(patNum,height,weight,bpSystolic,bpDiastolic,date,
				hasFollowUpPlan,isIneligible,documentation,childGotNutrition,childGotPhysCouns,
				weightCode,heightExamCode,weightExamCode,bmiExamCode,ehrNotPerformedNum,
				pregDiseaseNum,bmiPercentile,pulse);
			vitalsign.VitalsignNum=Vitalsigns.Insert(vitalsign);
			return vitalsign;
		}

		public static Vitalsign CreateVitalsign(long patNum,int height=0,int weight=0,int bpSystolic=0,int bpDiastolic=0,DateTime date=default(DateTime),
				bool hasFollowUpPlan=false,bool isIneligible=false,string documentation="",bool childGotNutrition=false,bool childGotPhysCouns=false,
				WeightUnitType weightCode=default(WeightUnitType),string heightExamCode="",string weightExamCode="",string bmiExamCode="",int ehrNotPerformedNum=0,
				long pregDiseaseNum=0,int bmiPercentile=0,int pulse=0){
			Vitalsign vitals=new Vitalsign();
			vitals.PatNum=patNum;
			vitals.Height=height;
			vitals.Weight=weight;
			vitals.BpSystolic=bpSystolic;
			vitals.BpDiastolic=bpDiastolic;
			if(date==default(DateTime)) {
				vitals.DateTaken=DateTime.Today;
			}
			vitals.DateTaken=date;
			vitals.HasFollowupPlan=hasFollowUpPlan;
			vitals.IsIneligible=isIneligible;
			vitals.Documentation=documentation;
			vitals.ChildGotNutrition=childGotNutrition;
			vitals.ChildGotPhysCouns=childGotPhysCouns;
			if(weightCode==default(WeightUnitType)){
				weightCode=WeightUnitType.lb;
			}
			vitals.WeightCode=weightCode.ToString();
			vitals.HeightExamCode=heightExamCode;
			vitals.WeightExamCode=weightExamCode;
			vitals.BMIExamCode=bmiExamCode;
			vitals.EhrNotPerformedNum=ehrNotPerformedNum;
			vitals.PregDiseaseNum=pregDiseaseNum;
			vitals.BMIPercentile=bmiPercentile;
			vitals.Pulse=pulse;
			return vitals;
		}

		public static List<Vitalsign> Update(Vitalsign vitals,long patNum) {
			VitalsignCrud.Insert(vitals);
			Vitalsign currVitals=Vitalsigns.GetOne(vitals.VitalsignNum);
			VitalsignCrud.Update(vitals);
			return Vitalsigns.Refresh(patNum);
		}

		public static void InsertHeightWeightBMICodes(bool clearVitalsignTable=false) {
			if(clearVitalsignTable) {
				ClearVitalsignTable();
			}
			Loinc loinc=new Loinc();
			loinc.LoincCode="59574-4";
			Loincs.Insert(loinc);
			loinc.LoincCode="59575-1";
			Loincs.Insert(loinc);
			loinc.LoincCode="59576-9";
			Loincs.Insert(loinc);
			loinc.LoincCode="8302-2";
			Loincs.Insert(loinc);
			loinc.LoincCode="3137-7";
			Loincs.Insert(loinc);
			loinc.LoincCode="3138-5";
			Loincs.Insert(loinc);
			loinc.LoincCode="8306-3";
			Loincs.Insert(loinc);
			loinc.LoincCode="8307-1";
			Loincs.Insert(loinc);
			loinc.LoincCode="8308-9";
			Loincs.Insert(loinc);
			loinc.LoincCode="29463-7";
			Loincs.Insert(loinc);
			loinc.LoincCode="18833-4";
			Loincs.Insert(loinc);
			loinc.LoincCode="3141-9";
			Loincs.Insert(loinc);
			loinc.LoincCode="3142-7";
			Loincs.Insert(loinc);
			loinc.LoincCode="8350-1";
			Loincs.Insert(loinc);
			loinc.LoincCode="8351-9";
		}
	}
}
