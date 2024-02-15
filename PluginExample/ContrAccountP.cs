using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;

namespace PluginExample {
	public partial class ContrAccountP:UserControl {
		private static ContrAccountP contrAccountP;
		public Family FamilyCur;
		public Patient PatientCur;

		public ContrAccountP() {
			InitializeComponent();
		}

		public static void InitializeOnStartup_end(OpenDental.ControlAccount sender) {
			contrAccountP=new ContrAccountP();
			sender.Controls.Add(contrAccountP.panelInsInfoDetail);
			//any control could be used here:
			Label label2=(Label)sender.Controls.Find("label2",true)[0];
			label2.MouseHover+=new EventHandler(contrAccountP.label2_MouseHover);
			label2.MouseLeave+=new EventHandler(contrAccountP.label2_MouseLeave);
		}

		public static void RefreshModuleData_end(object sender,Family family,Patient patient) {
			contrAccountP.FamilyCur=family;
			contrAccountP.PatientCur=patient;
		}

		public void label2_MouseHover(object sender,EventArgs e) {
			if(PatientCur==null) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			panelInsInfoDetail.Visible = true;
			panelInsInfoDetail.BringToFront();
			FillInsInfo();
			Cursor.Current=Cursors.Default;
		}

		private void label2_MouseLeave(object sender,EventArgs e) {
			panelInsInfoDetail.Visible = false;
		}

		public void FillInsInfo() {
			//Broken during overhaul of 6.7, fixed in 6.8 by DrTech
			textPriMax.Text = "";
			textPriDed.Text = "";
			textPriDedFam.Text = "";
			textPriDedRem.Text = "";
			textPriUsed.Text = "";
			textPriPend.Text = "";
			textPriRem.Text = "";
			textSecMax.Text = "";
			textSecDed.Text = "";
			textSecDedFam.Text = "";
			textSecDedRem.Text = "";
			textSecUsed.Text = "";
			textSecPend.Text = "";
			textSecRem.Text = "";
			if(PatientCur == null) {//redundant
				return;
			}
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(FamilyCur);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(PatientCur.PatNum);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(PatientCur.PatNum);
			List<ClaimProcHist> listClaimProcHists=ClaimProcs.GetHistList(PatientCur.PatNum,listBenefits,listPatPlans,listInsPlans,DateTime.Today,listInsSubs);
			double max = 0;
			double ded = 0;
			double dedFam = 0;
			double dedUsed = 0;
			double remain = 0;
			double pend = 0;
			double used = 0;
			InsPlan insPlan;//=new InsPlan();
			if(listPatPlans.Count > 0) {
				insPlan = InsPlans.GetPlan(InsSubs.GetOne(listPatPlans[0].InsSubNum).PlanNum,listInsPlans);
				pend = InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[0].PatPlanNum,-1,PatientCur.PatNum,listPatPlans[0].InsSubNum,listBenefits);
				used = InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,
					-1,listInsPlans,listBenefits,PatientCur.PatNum,listPatPlans[0].InsSubNum);
				textPriPend.Text = pend.ToString("F");
				textPriUsed.Text = used.ToString("F");
				max = Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,false);
				if(max == -1) {//if annual max is blank
					textPriMax.Text = "";
					textPriRem.Text = "";
				}
				else {
					remain = max - used - pend;
					if(remain < 0) {
						remain = 0;
					}
					textPriMax.Text = max.ToString("F");
					textPriRem.Text = remain.ToString("F");
				}
				//deductible:
				ded = Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam = Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded != -1) {
					textPriDed.Text = ded.ToString("F");
					dedUsed = InsPlans.GetDedUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,-1,listInsPlans,
							BenefitCoverageLevel.Individual,PatientCur.PatNum);
					textPriDedRem.Text = (ded - dedUsed).ToString("F");
				}
				if(dedFam != -1) {
					textPriDedFam.Text = dedFam.ToString("F");
				}
			}
			if(listPatPlans.Count > 1) {
				insPlan = InsPlans.GetPlan(InsSubs.GetOne(listPatPlans[0].InsSubNum).PlanNum,listInsPlans);
				pend = InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[1].PatPlanNum,-1,PatientCur.PatNum,listPatPlans[1].InsSubNum,listBenefits);
				textSecPend.Text = pend.ToString("F");
				used = InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,-1,
					listInsPlans,listBenefits,PatientCur.PatNum,listPatPlans[1].InsSubNum);
				textSecUsed.Text = used.ToString("F");
				max = Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,false);
				if(max == -1) {
					textSecMax.Text = "";
					textSecRem.Text = "";
				}
				else {
					remain = max - used - pend;
					if(remain < 0) {
						remain = 0;
					}
					textSecMax.Text = max.ToString("F");
					textSecRem.Text = remain.ToString("F");
				}
				ded = Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam = Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded != -1) {
					textSecDed.Text = ded.ToString("F");
					dedUsed = InsPlans.GetDedUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,-1,listInsPlans,
							BenefitCoverageLevel.Individual,PatientCur.PatNum);
					textSecDedRem.Text = (ded - dedUsed).ToString("F");
				}
				if(dedFam != -1) {
					textSecDedFam.Text = dedFam.ToString("F");
				}
			}
			/*
			//**only different line from tx pl routine fillsummary
			if(PatPlanList.Count == 0) {
				labelInsLeft.Text = Lan.g(this,"No Ins.");
				labelInsLeftAmt.Text = "";
			}
			else {
				labelInsLeft.Text = Lan.g(this,"Ins. Left");
				labelInsLeftAmt.Text = textPriRem.Text;
			}*/

		}



	}
}
