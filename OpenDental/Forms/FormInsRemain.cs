using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;
using System.Globalization;

namespace OpenDental {
	public partial class FormInsRemain:FormODBase {

		private Patient _patient;
		private Family _family;

		protected override string GetHelpOverride() {
			return "FormInsRemain";//Canada-only page
		}


		public FormInsRemain(long selectedPatNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_family=Patients.GetFamily(selectedPatNum);
			_patient=_family.GetPatient(selectedPatNum);
		}

		private void FormInsRemain_Load(object sender,EventArgs e) {
			SetGridCols();
			FillGrid();
			FillSummary();
		}

		/// <summary>Column sizes can be changed as needed</summary>
		private void SetGridCols() { 
			GridColumn col;
			gridRemainTimeUnits.BeginUpdate();
			gridRemainTimeUnits.Columns.Clear();
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Carrier"),60){ IsWidthDynamic=true };
			gridRemainTimeUnits.Columns.Add(col);
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Subscriber"),60){ IsWidthDynamic=true };
			gridRemainTimeUnits.Columns.Add(col);
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Category"),60){ IsWidthDynamic=true };
			gridRemainTimeUnits.Columns.Add(col);
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Qty"),60,HorizontalAlignment.Center);
			gridRemainTimeUnits.Columns.Add(col);
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Used"),60,HorizontalAlignment.Center);
			gridRemainTimeUnits.Columns.Add(col);
			col=new GridColumn(Lan.g(gridRemainTimeUnits.TranslationName,"Remaining"),67,HorizontalAlignment.Center);
			gridRemainTimeUnits.Columns.Add(col);
			gridRemainTimeUnits.EndUpdate();
		}

		private void FillGrid() {
			gridRemainTimeUnits.BeginUpdate();
			gridRemainTimeUnits.ListGridRows.Clear();
			List<PatPlan> listPatPlans=PatPlans.Refresh(_patient.PatNum);
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			if(listBenefits.IsNullOrEmpty()) {
				gridRemainTimeUnits.EndUpdate();
				return;
			}
			List<InsPlan> listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
			List<Carrier> listCarriers=Carriers.GetCarriers(listInsPlans.Select( x => x.CarrierNum).ToList());
			//Get the LIM information for all potential subscribers.
			List<Patient> listPatientsSubscribers=Patients.GetLimForPats(listInsSubs.Select(x => x.Subscriber).ToList());
			GridRow gridRow;
			//Get the last year of completed procedures because there is no current TimePeriod for benefits that will care about older procedures.
			//A subset of these procedures will be used for each specific benefit in order to correctly represent the time units remaining.
			List<Procedure> listProceduresCompleted=Procedures.GetCompletedForDateRange(
				DateTime.Today.AddYears(-1),
				DateTime.Today,
				listPatNums:new List<long> { _patient.PatNum });
			//Get all of the claimprocs associated to the completed procedures in order to link procedures to insurance plans.
			List<ClaimProc> listClaimProcs=ClaimProcs.GetForProcs(listProceduresCompleted.Select(x => x.ProcNum).ToList());
			for(int i=0;i<listBenefits.Count;i++) {
				if(listBenefits[i].CovCatNum==0 //no category
					|| listBenefits[i].BenefitType != InsBenefitType.Limitations //benefit type is not limitations
					|| (listBenefits[i].TimePeriod != BenefitTimePeriod.CalendarYear //neither calendar year, serviceyear, or 12 months
						&& listBenefits[i].TimePeriod != BenefitTimePeriod.ServiceYear
						&& listBenefits[i].TimePeriod != BenefitTimePeriod.NumberInLast12Months)
					|| listBenefits[i].Quantity < 0//quantity is negative (negatives are allowed in FormBenefitEdit)
					|| listBenefits[i].QuantityQualifier != BenefitQuantity.NumberOfServices//qualifier us not the number of services
					|| (listBenefits[i].CoverageLevel != BenefitCoverageLevel.Family //neither individual nor family coverage level
						&& listBenefits[i].CoverageLevel != BenefitCoverageLevel.Individual)) 
				{
					continue;
				}
				List<Procedure> listProcedures;
				//for calendar year, get completed procs from January.01.CurYear ~ Curdate
				if(listBenefits[i].TimePeriod==BenefitTimePeriod.CalendarYear) {
					//01/01/CurYear. is there a better way?
					listProcedures=listProceduresCompleted.FindAll(x => x.ProcDate>=new DateTime(DateTime.Today.Year,1,1));
				}
				else if(listBenefits[i].TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
					//today - 12 months - 1 day. Procedures exactly 1 year ago are not counted in the range
					listProcedures=listProceduresCompleted.FindAll(x => x.ProcDate>=DateTime.Today.AddYears(-1).AddDays(1));
				}
				else { //if not calendar year, then it must be service year
					int monthRenew=InsPlans.RefreshOne(listBenefits[i].PlanNum).MonthRenew; //monthrenew only stores the month as an int.
					if(DateTime.Today.Month >= monthRenew) {//if the the current date is past the renewal month, use the current year
						listProcedures=listProceduresCompleted.FindAll(x => x.ProcDate>=new DateTime(DateTime.Today.Year,monthRenew,1));
					}
					else { //otherwise use the previous year
						listProcedures=listProceduresCompleted.FindAll(x => x.ProcDate>=new DateTime(DateTime.Today.Year-1,monthRenew,1));
					}
				}
				Dictionary<long,List<ClaimProc>> dictionaryListClaimProcsPerSub;
				if(listBenefits[i].PatPlanNum!=0) {
					//The list of benefits that we are looping through was filled via listPatPlans so this will never fail.
					//If this line fails then it means that there was a valid PlanNum AND a valid PatPlanNum set on the benefit which is invalid ATM.
					dictionaryListClaimProcsPerSub=listClaimProcs.FindAll(x => x.InsSubNum==listPatPlans.First(y => y.PatPlanNum==listBenefits[i].PatPlanNum).InsSubNum)
						.GroupBy(x => x.InsSubNum)
						.ToDictionary(x => x.Key,x => x.ToList());
				}
				else {//benefit.PatPlanNum was not set so benefit.PlanNum must be set.
					dictionaryListClaimProcsPerSub=listClaimProcs.FindAll(x => x.PlanNum==listBenefits[i].PlanNum)
						.GroupBy(x => x.InsSubNum)
						.ToDictionary(x => x.Key,x => x.ToList());
				}
				foreach(long insSubNum in dictionaryListClaimProcsPerSub.Keys) {
					//The insSubNum should have a corresponding entry within listInsSubs.
					InsSub insSub=listInsSubs.FirstOrDefault(x => x.InsSubNum==insSubNum);
					if(insSub==null) {
						continue;//If not found then there are claimprocs associated to an inssub that is associated to a dropped or missing plan.
					}
					InsPlan insPlan=listInsPlans.FirstOrDefault(x => x.PlanNum==insSub.PlanNum);
					Carrier carrier=listCarriers.FirstOrDefault(x => x.CarrierNum==insPlan.CarrierNum);
					Patient patientSubscriber=listPatientsSubscribers.FirstOrDefault(x => x.PatNum==insSub.Subscriber);
					CovCat covCat=CovCats.GetCovCat(listBenefits[i].CovCatNum);
					//Filter out any procedures that are not associated to the insurance plan of the current benefit.
					List <Procedure> listProceduresFilter=listProcedures.FindAll(x => dictionaryListClaimProcsPerSub[insSubNum].Select(y => y.ProcNum).Contains(x.ProcNum));
					//Calculate the amount used for one benefit.
					double amtUsed=CovCats.GetAmtUsedForCat(listProceduresFilter,covCat);
					double amtRemain=listBenefits[i].Quantity-amtUsed;
					gridRow=new GridRow((carrier==null) ? "Unknown" : carrier.CarrierName,
						(patientSubscriber==null) ? "Unknown" : patientSubscriber.GetNameFL(),
						covCat.Description.ToString(),
						listBenefits[i].Quantity.ToString(),
						amtUsed.ToString("F"),
						(amtRemain > 0) ? amtRemain.ToString("F") : "0");
					gridRemainTimeUnits.ListGridRows.Add(gridRow);
				}
			}
			gridRemainTimeUnits.EndUpdate();
		}

		///<summary>All of the code from this method is copied directly from the account module, ContrAccount.FillSummary().</summary>
		private void FillSummary() {
			textFamPriMax.Text="";
			textFamPriDed.Text="";
			textFamSecMax.Text="";
			textFamSecDed.Text="";
			textPriMax.Text="";
			textPriDed.Text="";
			textPriDedRem.Text="";
			textPriUsed.Text="";
			textPriPend.Text="";
			textPriRem.Text="";
			textSecMax.Text="";
			textSecDed.Text="";
			textSecDedRem.Text="";
			textSecUsed.Text="";
			textSecPend.Text="";
			textSecRem.Text="";
			if(_patient==null) {
				return;
			}
			double maxFam=0;
			double maxInd=0;
			double ded=0;
			double dedFam=0;
			double dedRem=0;
			double remain=0;
			double pend=0;
			double used=0;
			InsPlan insPlan;
			InsSub insSub;
			List<PatPlan> listPatPlans=PatPlans.Refresh(_patient.PatNum);
			if(!PatPlans.IsPatPlanListValid(listPatPlans)) {
				//PatPlans had invalid references and need to be refreshed.
				listPatPlans=PatPlans.Refresh(_patient.PatNum);
			}
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			List<Claim> listClaims=Claims.Refresh(_patient.PatNum);
			List<ClaimProcHist> listClaimProcHists=ClaimProcs.GetHistList(_patient.PatNum,listBenefits,listPatPlans,listInsPlans,DateTime.Today,listInsSubs);
			if(listPatPlans.Count>0) {
				insSub=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
				insPlan=InsPlans.GetPlan(insSub.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[0].PatPlanNum,
					-1,_patient.PatNum,listPatPlans[0].InsSubNum,listBenefits);
				used=InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,
					-1,listInsPlans,listBenefits,_patient.PatNum,listPatPlans[0].InsSubNum);
				textPriPend.Text=pend.ToString("F");
				textPriUsed.Text=used.ToString("F");
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,false);
				if(maxFam==-1) {
					textFamPriMax.Text="";
				}
				else {
					textFamPriMax.Text=maxFam.ToString("F");
				}
				if(maxInd==-1) {//if annual max is blank
					textPriMax.Text="";
					textPriRem.Text="";
				}
				else {
					remain=maxInd-used-pend;
					if(remain<0) {
						remain=0;
					}
					//textFamPriMax.Text=max.ToString("F");
					textPriMax.Text=maxInd.ToString("F");
					textPriRem.Text=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1) {
					textPriDed.Text=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,
						-1,listInsPlans,_patient.PatNum,ded,dedFam);
					textPriDedRem.Text=dedRem.ToString("F");
				}
				if(dedFam!=-1) {
					textFamPriDed.Text=dedFam.ToString("F");
				}
			}
			if(listPatPlans.Count>1) {
				insSub=InsSubs.GetSub(listPatPlans[1].InsSubNum,listInsSubs);
				insPlan=InsPlans.GetPlan(insSub.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[1].PatPlanNum,
					-1,_patient.PatNum,listPatPlans[1].InsSubNum,listBenefits);
				textSecPend.Text=pend.ToString("F");
				used=InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,
					-1,listInsPlans,listBenefits,_patient.PatNum,listPatPlans[1].InsSubNum);
				textSecUsed.Text=used.ToString("F");
				//max=Benefits.GetAnnualMaxDisplay(BenefitList,PlanCur.PlanNum,PatPlanList[1].PatPlanNum);
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,false);
				if(maxFam==-1) {
					textFamSecMax.Text="";
				}
				else {
					textFamSecMax.Text=maxFam.ToString("F");
				}
				if(maxInd==-1) {//if annual max is blank
					textSecMax.Text="";
					textSecRem.Text="";
				}
				else {
					remain=maxInd-used-pend;
					if(remain<0) {
						remain=0;
					}
					//textFamSecMax.Text=max.ToString("F");
					textSecMax.Text=maxInd.ToString("F");
					textSecRem.Text=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1) {
					textSecDed.Text=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,
						-1,listInsPlans,_patient.PatNum,ded,dedFam);
					textSecDedRem.Text=dedRem.ToString("F");
				}
				if(dedFam!=-1) {
					textFamSecDed.Text=dedFam.ToString("F");
				}
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}