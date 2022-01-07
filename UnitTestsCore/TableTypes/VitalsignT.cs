using System;
using System.Collections.Generic;
using OpenDentBusiness;
using OpenDentBusiness.Crud;

namespace UnitTestsCore {
	public class VitalsignT {

		public static Vitalsign CreateVitalsign(long patNum,int height=0,int weight=0,int bpSystolic=0,int bpDiastolic=0,DateTime date=default(DateTime),
				bool hasFollowUpPlan=false,bool isIneligible=false,string documentation="",bool childGotNutrition=false,bool childGotPhysCouns=false,
				WeightUnitType weightCode=default(WeightUnitType),string heightExamCode="",string weightExamCode="",string bmiExamCode="",int ehrNotPerformedNum=0,
				int pregDiseaseNum=0,int bmiPercentile=0,int pulse=0){
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
	}
}
