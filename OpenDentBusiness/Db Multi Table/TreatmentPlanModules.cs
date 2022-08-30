using CodeBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class TreatmentPlanModules {
		private static string _translationString="TableTP";

		///<summary>Gets a good chunk of the data used in the TP Module.</summary>
		public static TPModuleData GetModuleData(long patNum,bool doMakeSecLog) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<TPModuleData>(MethodBase.GetCurrentMethod(),patNum,doMakeSecLog);
			}
			TPModuleData tpData=new TPModuleData();
			tpData.Fam=Patients.GetFamily(patNum);
			tpData.Pat=tpData.Fam.GetPatient(patNum);
			tpData.PatPlanList=PatPlans.Refresh(patNum);
			if(!PatPlans.IsPatPlanListValid(tpData.PatPlanList)) {
				//PatPlans had invalid references and need to be refreshed.
				tpData.PatPlanList=PatPlans.Refresh(patNum);
			}
			tpData.SubList=InsSubs.RefreshForFam(tpData.Fam);
			tpData.InsPlanList=InsPlans.RefreshForSubList(tpData.SubList);
			tpData.BenefitList=Benefits.Refresh(tpData.PatPlanList,tpData.SubList);
			tpData.ClaimList=Claims.Refresh(tpData.Pat.PatNum);
			tpData.HistList=ClaimProcs.GetHistList(tpData.Pat.PatNum,tpData.BenefitList,tpData.PatPlanList,tpData.InsPlanList,DateTime.Today,
				tpData.SubList);
			tpData.ListSubstLinks=SubstitutionLinks.GetAllForPlans(tpData.InsPlanList);
			tpData.DiscountPlanSub=DiscountPlanSubs.GetSubForPat(patNum);
			tpData.DiscountPlan=DiscountPlans.GetForPats(new List<long>{ patNum }).FirstOrDefault();
			TreatPlanType tpTypeCur=(tpData.DiscountPlanSub==null?TreatPlanType.Insurance:TreatPlanType.Discount);
			TreatPlans.AuditPlans(patNum,tpTypeCur);
			tpData.ListProcedures=Procedures.Refresh(patNum);
			tpData.ListTreatPlans=TreatPlans.GetAllForPat(patNum);
			tpData.ListProcTPs=ProcTPs.Refresh(patNum);
			if(doMakeSecLog) {
				SecurityLogs.MakeLogEntry(Permissions.TPModule,patNum,"");
			}
			return tpData;
		}

		///<summary>Gets most of the data needed to load the active treatment plan.</summary>
		///<param name="doFillHistList">If false, then LoadActiveTPData.HistList will be null.</param>
		public static LoadActiveTPData GetLoadActiveTpData(Patient pat,long treatPlanNum,List<Benefit> listBenefits,List<PatPlan> listPatPlans,
			List<InsPlan> listInsPlans,DateTime dateTimeTP,List<InsSub> listInsSubs,bool doFillHistList,bool isTreatPlanSortByTooth,
			List<SubstitutionLink> listSubstLinks) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check here to reduce round-trips to the server.
				return Meth.GetObject<LoadActiveTPData>(MethodBase.GetCurrentMethod(),pat,treatPlanNum,listBenefits,listPatPlans,listInsPlans,dateTimeTP,
					listInsSubs,doFillHistList,isTreatPlanSortByTooth,listSubstLinks);
			}
			LoadActiveTPData data=new LoadActiveTPData();
			data.ListTreatPlanAttaches=TreatPlanAttaches.GetAllForTreatPlan(treatPlanNum);
			List<Procedure> listProcs=Procedures.GetManyProc(data.ListTreatPlanAttaches.Select(x=>x.ProcNum).ToList(),false);
			data.listProcForTP=Procedures.SortListByTreatPlanPriority(listProcs.FindAll(x => x.ProcStatus==ProcStat.TP || x.ProcStatus==ProcStat.TPi)
				,isTreatPlanSortByTooth,data.ListTreatPlanAttaches).ToList();
			//One thing to watch out for here is that we must be absolutely sure to include all claimprocs for the procedures listed,
			//regardless of status.  Needed for Procedures.ComputeEstimates.  This should be fine.
			data.ClaimProcList=ClaimProcs.RefreshForTP(pat.PatNum);
			if(doFillHistList) {
				data.HistList=ClaimProcs.GetHistList(pat.PatNum,listBenefits,listPatPlans,listInsPlans,-1,dateTimeTP,listInsSubs);
			}
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			foreach(Procedure procedure in listProcs) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(procedure.CodeNum));
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum);
			data.ListFees=Fees.GetListFromObjects(listProcedureCodes,listProcs.Select(x=>x.MedicalCode).ToList(),listProcs.Select(x=>x.ProvNum).ToList(),
				pat.PriProv,pat.SecProv,pat.FeeSched,listInsPlans,listProcs.Select(x=>x.ClinicNum).ToList(),null,//appts can be null because provs already set
				listSubstLinks,discountPlanNum);
			data.BlueBookEstimateData=new BlueBookEstimateData(listInsPlans,listInsSubs,listPatPlans,listProcs,listSubstLinks);
			return data;
		}

		public static List<TpRow> GetArchivedTpRows(bool showSubTotals,bool showTotals,TreatPlan treatPlanTemp,List<ProcTP> _listProcTPsSelect,List<Procedure> _listProcedures,bool doPrioritizeTotals=false) {
			decimal subfee=0;
			decimal suballowed=0;
			decimal subpriIns=0;
			decimal subsecIns=0;
			decimal subdiscount=0;
			decimal subpat=0;
			decimal subTaxEst=0;
			decimal subCatPercUCR=0;
			decimal totFee=0;
			decimal totPriIns=0;
			decimal totSecIns=0;
			decimal totDiscount=0;
			decimal totPat=0;
			decimal totAllowed=0;
			decimal totTaxEst=0;
			decimal totCatPercUCR=0;
			List<TpRow> listTpRows=new List<TpRow>();
			//Archived TPs below this point==============================================================================
			List<Appointment> listAppointments=Appointments.GetAppointmentsForProcs(_listProcedures);
			bool isDone;
			decimal priIns,secIns,allowed,taxAmt,subTaxAmt,totTaxAmt;
			subfee=suballowed=totFee=priIns=secIns=subpriIns=allowed=totPriIns=subsecIns=totSecIns=subdiscount=totDiscount=subpat=totPat=totAllowed=
				taxAmt=subTaxAmt=totTaxAmt=subCatPercUCR=totCatPercUCR=0;
			TpRow row;
			for(int i=0;i<_listProcTPsSelect.Count;i++) {
				row=new TpRow();
				isDone=false;
				long plannedAptNumCur=0;
				long aptNumCur=0;
				ApptStatus apptStatus=ApptStatus.None;
				if(_listProcTPsSelect[i].TagOD==null) {
					_listProcTPsSelect[i].TagOD=0L;
				}
				for(int j=0;j<_listProcedures.Count;j++) {
					if(_listProcedures[j].ProcNum==_listProcTPsSelect[i].ProcNumOrig) {
						if((long)_listProcTPsSelect[i].TagOD==0) {
							_listProcTPsSelect[i].TagOD=_listProcedures[j].ProcNumLab;
						}
						if(_listProcedures[j].ProcStatus==ProcStat.C) {
							isDone=true;
						}
						plannedAptNumCur=_listProcedures[j].PlannedAptNum;
						aptNumCur=_listProcedures[j].AptNum;
						Appointment appointment=listAppointments.FirstOrDefault(x => x.AptNum==aptNumCur);
						if(appointment!=null) {
							apptStatus=appointment.AptStatus;
						}
					}
				}
				if(isDone) {
					row.Done="X";
				}
				row.ProcAbbr=_listProcTPsSelect[i].ProcAbbr;
				row.Priority=Defs.GetName(DefCat.TxPriorities,_listProcTPsSelect[i].Priority);
				row.Tth=_listProcTPsSelect[i].ToothNumTP;
				row.Surf=_listProcTPsSelect[i].Surf;
				row.Code=_listProcTPsSelect[i].ProcCode;
				row.Description=_listProcTPsSelect[i].Descript;
				row.Fee=(decimal)_listProcTPsSelect[i].FeeAmt; //Fee
				row.PriIns=(decimal)_listProcTPsSelect[i].PriInsAmt; //PriIns or DiscountPlan
				row.SecIns=(decimal)_listProcTPsSelect[i].SecInsAmt; //SecIns
				row.FeeAllowed=(decimal)_listProcTPsSelect[i].FeeAllowed;
				row.TaxEst=(decimal)_listProcTPsSelect[i].TaxAmt; //Estimated Tax
				row.ProvNum=_listProcTPsSelect[i].ProvNum;
				row.DateTP=_listProcTPsSelect[i].DateTP;
				row.ClinicNum=_listProcTPsSelect[i].ClinicNum;
				row.Appt=row.Appt=TreatmentPlanModules.GetApptStatusAbbr(plannedAptNumCur,aptNumCur,apptStatus);
				row.CatPercUCR=(decimal)_listProcTPsSelect[i].CatPercUCR;
				//Totals
				subfee+=row.Fee;
				if(CompareDecimal.IsGreaterThan(row.FeeAllowed,-1)) {//-1 means the proc is DoNotBillIns
					suballowed+=row.FeeAllowed;
					totAllowed+=(decimal)_listProcTPsSelect[i].FeeAllowed;
				}
				totFee+=row.Fee;
				subpriIns+=(decimal)_listProcTPsSelect[i].PriInsAmt;
				totPriIns+=treatPlanTemp.TPType==TreatPlanType.Insurance ? (decimal)_listProcTPsSelect[i].PriInsAmt : row.PriIns;
				subsecIns+=(decimal)_listProcTPsSelect[i].SecInsAmt;
				totSecIns+=(decimal)_listProcTPsSelect[i].SecInsAmt;
				row.Discount=(decimal)_listProcTPsSelect[i].Discount; //Discount
				subdiscount+=(decimal)_listProcTPsSelect[i].Discount;
				totDiscount+=(decimal)_listProcTPsSelect[i].Discount;
				row.Pat=(decimal)_listProcTPsSelect[i].PatAmt; //Pat
				subpat+=(decimal)_listProcTPsSelect[i].PatAmt;
				totPat+=(decimal)_listProcTPsSelect[i].PatAmt;
				subTaxEst+=(decimal)_listProcTPsSelect[i].TaxAmt;
				totTaxEst+=(decimal)_listProcTPsSelect[i].TaxAmt;
				subCatPercUCR+=(decimal)_listProcTPsSelect[i].CatPercUCR;
				totCatPercUCR+=(decimal)_listProcTPsSelect[i].CatPercUCR;
				row.Prognosis=_listProcTPsSelect[i].Prognosis; //Prognosis
				row.Dx=_listProcTPsSelect[i].Dx;
				row.ColorText=Defs.GetColor(DefCat.TxPriorities,_listProcTPsSelect[i].Priority);
				if(row.ColorText==System.Drawing.Color.White) {
					row.ColorText=System.Drawing.Color.Black;
				}
				row.Tag=_listProcTPsSelect[i].Copy();
				row.RowType=TpRowType.TpRow;
				listTpRows.Add(row);
				if(showSubTotals &&
					 (i==_listProcTPsSelect.Count-1 || _listProcTPsSelect[i+1].Priority!=_listProcTPsSelect[i].Priority)) {
					row=new TpRow();
					row.Description=Lans.g(_translationString,"Subtotal");
					row.Fee=subfee;
					row.PriIns=subpriIns;
					row.SecIns=subsecIns;
					row.Discount=subdiscount;
					row.Pat=subpat;
					row.FeeAllowed=suballowed;
					row.TaxEst=subTaxEst;
					row.CatPercUCR=subCatPercUCR;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,_listProcTPsSelect[i].Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					row.Bold=true;
					row.ColorLborder=System.Drawing.Color.Black;
					row.RowType=TpRowType.SubTotal;
					if(doPrioritizeTotals) {
						row.Priority=Defs.GetName(DefCat.TxPriorities,_listProcTPsSelect[i].Priority);
					}
					listTpRows.Add(row);
					subfee=0;
					subpriIns=0;
					subsecIns=0;
					subdiscount=0;
					subpat=0;
					suballowed=0;
					subTaxEst=0;
					subCatPercUCR=0;
				}
			}
			if(showTotals) {
				row=new TpRow();
				row.Description=Lans.g(_translationString,"Total");
				row.Fee=totFee;
				row.PriIns=totPriIns;
				row.SecIns=totSecIns;
				row.Discount=totDiscount;
				row.Pat=totPat;
				row.FeeAllowed=totAllowed;
				row.TaxEst=totTaxEst;
				row.CatPercUCR=totCatPercUCR;
				row.Bold=true;
				row.ColorText=System.Drawing.Color.Black;
				row.RowType=TpRowType.Total;
				listTpRows.Add(row);
			}
			return listTpRows;
		}

		///<summary>Gets the active treatment plan as a list of ProcTP.  
		///Uses the static variable 'PrefC.IsTreatPlanSortByTooth' to determine if procedures should be sorted by tooth order.</summary>
		public static List<TpRow> GetActiveTpPlanTpRows(bool showMaxDeductions,bool showDiscounts,bool showSubTotals,bool showTotals,TreatPlan treatPlan,Patient PatientCur,DateTime dateTimeTP,LoadActiveTPData _loadActiveTPData, List<InsPlan> _listInsPlans,
			List<Benefit> _listBenefits,List<PatPlan> _listPatPlans,List<SubstitutionLink> _listSubstitutionLinks,List<InsSub> _listInsSubs, DiscountPlanSub discountPlanSub, 
			DiscountPlan discountPlan, List<Procedure> _listProcedures, ref List<ClaimProc> _listClaimProcs,List<ClaimProcHist> _listClaimProcHists,bool doProritizeTotals=false) 
		{
			//No remoting role check. No call to db.
			List<TpRow> listTpRows=new List<TpRow>();
			List<TreatPlanAttach> listTreatPlanAttaches=_loadActiveTPData.ListTreatPlanAttaches;
			List<Procedure> listProceduresForTPs=_loadActiveTPData.listProcForTP;
			List<Appointment> listAppointments=Appointments.GetAppointmentsForProcs(_listProcedures);
			Lookup<FeeKey2,Fee> lookupFees=null;
			if(_loadActiveTPData.ListFees!=null){
				lookupFees=(Lookup<FeeKey2,Fee>)_loadActiveTPData.ListFees.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
			}
			InsPlan insPlanPri=null;
			if(_listPatPlans.Count>0) { //primary
				InsSub insSubPri=InsSubs.GetSub(_listPatPlans[0].InsSubNum,_listInsSubs);
				insPlanPri=InsPlans.GetPlan(insSubPri.PlanNum,_listInsPlans);
			}
			InsPlan insPlanSec=null;
			if(_listPatPlans.Count>1) { //secondary
				InsSub insSubSec=InsSubs.GetSub(_listPatPlans[1].InsSubNum,_listInsSubs);
				insPlanSec=InsPlans.GetPlan(insSubSec.PlanNum,_listInsPlans);
			}
			List<ClaimProc> listClaimProcsOld=_listClaimProcs.Select(x => x.Copy()).ToList();
			List<ClaimProcHist> listClaimProcHistsLoops=new List<ClaimProcHist>();
			//foreach(Procedure tpProc in listProcForTP){
			//Get data for any OrthoCases that may be linked to procs in listProcForTP
			List<OrthoProcLink> listOrthoProcLinksAll=new List<OrthoProcLink>();
			Dictionary<long,OrthoProcLink> dictionaryOrthoProcLinksForProcList=new Dictionary<long,OrthoProcLink>();
			Dictionary<long,OrthoCase> dictionaryOrthoCases=new Dictionary<long,OrthoCase>();
			Dictionary<long,OrthoSchedule> dictionaryOrthoSchedules=new Dictionary<long,OrthoSchedule>();
			OrthoCases.GetDataForListProcs(ref listOrthoProcLinksAll,ref dictionaryOrthoProcLinksForProcList,ref dictionaryOrthoCases,ref dictionaryOrthoSchedules,_listProcedures);
			OrthoProcLink orthoProcLink=null;
			OrthoCase orthoCase=null;
			OrthoSchedule orthoSchedule=null;
			List<OrthoProcLink> listOrthoProcLinksForOrthoCases=null;
			if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
				for(int i=0;i<listProceduresForTPs.Count;i++) {
					listProceduresForTPs[i].ProcDate=dateTimeTP;
					if(listProceduresForTPs[i].ProcNumLab!=0){
							//Lab fees will be calculated and added to looplist when its parent is calculated.
							continue;
					}
					OrthoCases.FillOrthoCaseObjectsForProc(listProceduresForTPs[i].ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,
						ref listOrthoProcLinksForOrthoCases,dictionaryOrthoProcLinksForProcList,dictionaryOrthoCases,dictionaryOrthoSchedules,listOrthoProcLinksAll);
					Procedures.ComputeEstimates(listProceduresForTPs[i],PatientCur.PatNum,ref _listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,
						_listClaimProcHists,listClaimProcHistsLoops,false,
						PatientCur.Age,_listInsSubs,
						null,false,true,_listSubstitutionLinks,false,
						_loadActiveTPData.ListFees,lookupFees,
						orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCases,_loadActiveTPData.BlueBookEstimateData);
					//then, add this information to loopList so that the next procedure is aware of it.
					listClaimProcHistsLoops.AddRange(ClaimProcs.GetHistForProc(_listClaimProcs,listProceduresForTPs[i].ProcNum,listProceduresForTPs[i].CodeNum));
				}
				SyncCanadianLabs(_listClaimProcs,listProceduresForTPs);
				//We don't want to save the claimprocs if it's a date other than DateTime.Today, since they are calculated using modified date information.
				//Only save to db if this is the active TP.  Inactive TPs should not change what is stored in the db, only what is displayed in the grid.
				if(dateTimeTP==DateTime.Today && treatPlan.TPStatus==TreatPlanStatus.Active) {
					ClaimProcs.Synch(ref _listClaimProcs,listClaimProcsOld);
				}
			}
			else { 
				for(int i=0;i<listProceduresForTPs.Count;i++) {
					if(listProceduresForTPs[i].ProcNumLab!=0){
							//Lab fees will be calculated and added to looplist when its parent is calculated.
							continue;
					}
					OrthoCases.FillOrthoCaseObjectsForProc(listProceduresForTPs[i].ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,
						ref listOrthoProcLinksForOrthoCases,dictionaryOrthoProcLinksForProcList,dictionaryOrthoCases,dictionaryOrthoSchedules,listOrthoProcLinksAll);
					Procedures.ComputeEstimates(listProceduresForTPs[i],PatientCur.PatNum,ref _listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,_listClaimProcHists,listClaimProcHistsLoops
						,false,PatientCur.Age,_listInsSubs,
						listSubstLinks:_listSubstitutionLinks,lookupFees:lookupFees,
						orthoProcLink:orthoProcLink,orthoCase:orthoCase,orthoSchedule:orthoSchedule,listOrthoProcLinksForOrthoCase:listOrthoProcLinksForOrthoCases,
						blueBookEstimateData:_loadActiveTPData.BlueBookEstimateData);
					//then, add this information to loopList so that the next procedure is aware of it.
					listClaimProcHistsLoops.AddRange(ClaimProcs.GetHistForProc(_listClaimProcs,listProceduresForTPs[i].ProcNum,listProceduresForTPs[i].CodeNum));
				}
				SyncCanadianLabs(_listClaimProcs,listProceduresForTPs);
				//Only save to db if this is the active TP.  Inactive TPs should not change what is stored in the db, only what is displayed in the grid.
				if(treatPlan.TPStatus==TreatPlanStatus.Active) {
					ClaimProcs.Synch(ref _listClaimProcs,listClaimProcsOld);
				}
			}
			//claimProcList=ClaimProcs.RefreshForTP(PatCur.PatNum);
			string estimateNote;
			decimal subfee,suballowed,totFee,priIns,secIns,subpriIns,allowed,totPriIns,subsecIns,totSecIns,subdiscount,totDiscount,subpat,totPat,totAllowed
				,taxAmt,subTaxAmt,totTaxAmt,subCatPercUCR,totCatPercUCR;
			subfee=suballowed=totFee=priIns=secIns=subpriIns=allowed=totPriIns=subsecIns=totSecIns=subdiscount=totDiscount=subpat=totPat=totAllowed=
				taxAmt=subTaxAmt=totTaxAmt=subCatPercUCR=totCatPercUCR=0;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			bool doShowDiscountForCatPercent=listDisplayFields.Any(x => x.InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR)
				&& listDisplayFields.Any(x => x.InternalName==DisplayFields.InternalNames.TreatmentPlanModule.Fee);
			InsPlan insPlanPrimary=null;
			if(_listPatPlans.Count>0) {
				InsSub insSubPrimary=InsSubs.GetSub(_listPatPlans[0].InsSubNum,_listInsSubs);
				insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,_listInsPlans);
			}
			List<ProcTP> listProcTPsRetVals=new List<ProcTP>();
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			if(discountPlanSub!=null && !listProceduresForTPs.IsNullOrEmpty()) {
				listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(listProceduresForTPs,discountPlanSub:discountPlanSub,discountPlan:discountPlan);
			}
			List<long> listCodeNums=listProceduresForTPs.Select(x => x.CodeNum).Distinct().ToList(); //List of all CodeNums in the treatmentplan
			List<InsPlanPreference> listInsPlanPreferences=InsPlanPreferences.GetManyForFKeys(listCodeNums,InsPlanPrefFKeyType.ProcCodeNoBillIns,insPlanPrimary);
			for(int i=0;i<listProceduresForTPs.Count;i++) {
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresForTPs[i].CodeNum);
				TpRow row=new TpRow();
				row.ProcAbbr=procedureCode.AbbrDesc;
				row.ProvNum=listProceduresForTPs[i].ProvNum;
				row.DateTP=listProceduresForTPs[i].DateTP;
				row.ClinicNum=listProceduresForTPs[i].ClinicNum;
				ApptStatus apptStatus=ApptStatus.None;
				Appointment appointment=listAppointments.FirstOrDefault(x => x.AptNum==listProceduresForTPs[i].AptNum);
				if(appointment!=null) {
					apptStatus=appointment.AptStatus;
				}
				row.Appt=GetApptStatusAbbr(listProceduresForTPs[i].PlannedAptNum,listProceduresForTPs[i].AptNum, apptStatus);
				//Passing in empty lists to simulate what the fee would be if the patient did not have any insurance.
				row.CatPercUCR=(decimal)Procedures.GetProcFee(PatientCur,new List<PatPlan>(),new List<InsSub>(),new List<InsPlan>(),listProceduresForTPs[i].CodeNum,
					listProceduresForTPs[i].ProvNum,listProceduresForTPs[i].ClinicNum,listProceduresForTPs[i].MedicalCode,listFees:_loadActiveTPData.ListFees);
				decimal fee=(decimal)listProceduresForTPs[i].ProcFeeTotal;
				subfee+=fee;
				totFee+=fee;
				#region ShowMaxDed
				string showPriDeduct="";
				string showSecDeduct="";
				ClaimProc claimProc; //holds the estimate.
				DiscountPlanProc discountPlanProc=null;
				priIns=0; //We need to clear out priIns in this loop, it's not reset anywhere.
				if(discountPlanSub!=null) {
					discountPlanProc=listDiscountPlanProcs.First(x=>x.ProcNum==listProceduresForTPs[i].ProcNum);
					priIns=(decimal)discountPlanProc.DiscountPlanAmt;
				}
				if(_listPatPlans.Count>0) { //Primary
					claimProc=ClaimProcs.GetEstimate(_listClaimProcs,listProceduresForTPs[i].ProcNum,insPlanPri.PlanNum,_listPatPlans[0].InsSubNum);
					if(claimProc==null || claimProc.EstimateNote.Contains("Frequency Limitation")) {
						if(claimProc!=null && claimProc.InsEstTotalOverride!=-1) {
							priIns=(decimal)claimProc.InsEstTotalOverride;
						}
						else { 
							priIns=0;
						}
					}
					else {
						if(showMaxDeductions) { //whether visible or not
							priIns=(decimal)ClaimProcs.GetInsEstTotal(claimProc);
							double ded=ClaimProcs.GetDeductibleDisplay(claimProc);
							if(ded>0) {
								showPriDeduct="\r\n"+Lans.g(_translationString,"Pri Deduct Applied: ")+ded.ToString("c");
							}
						}
						else {
							priIns=(decimal)claimProc.BaseEst;
						}
					}
					bool noBillIns=InsPlanPreferences.NoBillIns(procedureCode,listInsPlanPreferences); 
					if((claimProc!=null && claimProc.NoBillIns) || (claimProc==null && noBillIns)) {
						allowed=-1;
					}
					else {
						Appointment appointmentForProc=listAppointments.Find(x=>x.AptNum==listProceduresForTPs[i].AptNum);
						if(appointmentForProc==null) {
							appointmentForProc=listAppointments.Find(x=>x.AptNum==listProceduresForTPs[i].PlannedAptNum); //If a scheduled appt doesn't exist, check for a planned one.
						}
						allowed=ComputeAllowedAmount(listProceduresForTPs[i],claimProc,_listSubstitutionLinks,lookupFees,_listInsPlans,_loadActiveTPData.BlueBookEstimateData,appointmentForProc);
					}
				}
				if(_listPatPlans.Count>1) { //Secondary
					claimProc=ClaimProcs.GetEstimate(_listClaimProcs,listProceduresForTPs[i].ProcNum,insPlanSec.PlanNum,_listPatPlans[1].InsSubNum);
					if(claimProc==null) {
						secIns=0;
					}
					else {
						if(showMaxDeductions) {
							secIns=(decimal)ClaimProcs.GetInsEstTotal(claimProc);
							decimal ded=(decimal)ClaimProcs.GetDeductibleDisplay(claimProc);
							if(ded>0) {
								showSecDeduct="\r\n"+Lans.g(_translationString,"Sec Deduct Applied: ")+ded.ToString("c");
							}
						}
						else {
							secIns=(decimal)claimProc.BaseEst;
						}
					}
				} //secondary
				else { //no secondary ins
					secIns=0;
				}
				#endregion ShowMaxDed
				subpriIns+=priIns;
				totPriIns+=priIns;
				subsecIns+=secIns;
				totSecIns+=secIns;
				if(CompareDecimal.IsGreaterThan(allowed,-1)) {//-1 means the proc is DoNotBillIns
					suballowed+=allowed;
					totAllowed+=allowed;
				}
				taxAmt=(decimal)listProceduresForTPs[i].TaxAmt;
				subTaxAmt+=taxAmt;
				totTaxAmt+=taxAmt;
				subCatPercUCR+=row.CatPercUCR;
				totCatPercUCR+=row.CatPercUCR;
				decimal discount=(decimal)ClaimProcs.GetTotalWriteOffEstimateDisplay(_listClaimProcs,listProceduresForTPs[i].ProcNum);
				decimal writeOffDiscount=discount;
				if(doShowDiscountForCatPercent && insPlanPrimary!=null && insPlanPrimary.PlanType=="") {//plan is category percentage
					discount+=Math.Max(row.CatPercUCR-fee,0);
				}
				decimal procDiscount=(decimal)listProceduresForTPs[i].Discount;
				discount+=procDiscount;
				subdiscount+=discount;
				totDiscount+=discount;
				decimal pat=fee-priIns-secIns-writeOffDiscount-procDiscount+taxAmt;
				if(pat<0) {
					pat=0;
				}
				subpat+=pat;
				totPat+=pat;
				//Fill TpRow object with information.
				row.Priority=Defs.GetName(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProceduresForTPs[i].ProcNum).Priority);//(Defs.GetName(DefCat.TxPriorities,listProcForTP[i].Priority));
				row.Tth=(Tooth.Display(listProceduresForTPs[i].ToothNum));
				if(ProcedureCodes.GetProcCode(listProceduresForTPs[i].CodeNum).TreatArea==TreatmentArea.Surf) {
					row.Surf=(Tooth.SurfTidyFromDbToDisplay(listProceduresForTPs[i].Surf,listProceduresForTPs[i].ToothNum));
				}
				else if(ProcedureCodes.GetProcCode(listProceduresForTPs[i].CodeNum).TreatArea==TreatmentArea.Sextant) {
					row.Surf=Tooth.GetSextant(listProceduresForTPs[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					row.Surf=(listProceduresForTPs[i].Surf); //I think this will properly allow UR, L, etc.
				}
				row.Code=procedureCode.ProcCode;
				string descript=ProcedureCodes.GetLaymanTerm(listProceduresForTPs[i].CodeNum);
				if(listProceduresForTPs[i].ToothRange!="") {
					descript+=" #"+Tooth.DisplayRange(listProceduresForTPs[i].ToothRange);
				}
				if(showMaxDeductions) {
					estimateNote=ClaimProcs.GetEstimateNotes(listProceduresForTPs[i].ProcNum,_listClaimProcs);
					if(estimateNote!="") {
						descript+="\r\n"+estimateNote;
					}
				}
				row.Description=(descript);
				if(listProceduresForTPs[i].ProcNumLab!=0) {
					row.Description=$"^ ^ {descript}";
				}
				if(showPriDeduct!="") {
					row.Description+=showPriDeduct;
				}
				if(showSecDeduct!="") {
					row.Description+=showSecDeduct;
				}
				if(discountPlanProc!=null) {
					if(discountPlanProc.doesExceedFreqLimit) {
						row.Description+=" - "+Lans.g(_translationString,"Frequency Limitation");
					}
					else if(discountPlanProc.doesExceedAnnualMax) {
						row.Description+=" - "+Lans.g(_translationString,"Over Annual Max");
					}
				}
				row.Prognosis=Defs.GetName(DefCat.Prognosis,PIn.Long(listProceduresForTPs[i].Prognosis.ToString()));
				row.Dx=Defs.GetValue(DefCat.Diagnosis,PIn.Long(listProceduresForTPs[i].Dx.ToString()));
				row.Fee=fee;
				row.PriIns=priIns;
				row.SecIns=secIns;
				row.Discount=discount;
				row.Pat=pat;
				row.FeeAllowed=allowed;
				row.TaxEst=taxAmt;
				row.ColorText=Defs.GetColor(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==listProceduresForTPs[i].ProcNum).Priority);
				if(row.ColorText==System.Drawing.Color.White) {
					row.ColorText=System.Drawing.Color.Black;
				}
				Procedure procedure=listProceduresForTPs[i].Copy();
				long priority=listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==procedure.ProcNum).Priority;
				row.RowType=TpRowType.TpRow;
				listTpRows.Add(row);
				#region subtotal
				if(showSubTotals &&
					 (i==listProceduresForTPs.Count-1 || listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProceduresForTPs[i+1].ProcNum).Priority!=priority)) {
					row=new TpRow();
					row.Description=Lans.g(_translationString,"Subtotal");
					row.Fee=subfee;
					row.PriIns=subpriIns;
					row.SecIns=subsecIns;
					row.Discount=subdiscount;
					row.Pat=subpat;
					row.FeeAllowed=suballowed;
					row.TaxEst=subTaxAmt;
					row.CatPercUCR=subCatPercUCR;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==listProceduresForTPs[i].ProcNum).Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					row.Bold=true;
					row.ColorLborder=System.Drawing.Color.Black;
					row.RowType=TpRowType.SubTotal;
					if(doProritizeTotals) {
						row.Priority=Defs.GetName(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProceduresForTPs[i].ProcNum).Priority);
					}
					listTpRows.Add(row);
					subfee=0;
					subpriIns=0;
					subsecIns=0;
					subdiscount=0;
					subpat=0;
					suballowed=0;
					subTaxAmt=0;
					subCatPercUCR=0;
				}
				#endregion subtotal
			}
			#region Totals
			if(showTotals) {
				TpRow row=new TpRow();
				row.Description=Lans.g(_translationString,"Total");
				row.Fee=totFee;
				row.PriIns=totPriIns;
				row.SecIns=totSecIns;
				row.Discount=totDiscount;
				row.Pat=totPat;
				row.FeeAllowed=totAllowed;
				row.TaxEst=totTaxAmt;
				row.CatPercUCR=totCatPercUCR;
				row.Bold=true;
				row.ColorText=System.Drawing.Color.Black;
				row.RowType=TpRowType.Total;
				listTpRows.Add(row);
			}
			#endregion Totals
			return listTpRows;
		}



		///<summary>Refreshes Canadian Lab Fee ClaimProcs in listClaimProcs from the Db, and adds any new Canadian Lab Fee ClaimProcs that were 
		///added to Db when computing estimates</summary>
		private static void SyncCanadianLabs(List<ClaimProc> listClaimProcs,List<Procedure> listProceduresTps) {
			//1. Get all lab Cp for lab proc nums from Db
			//2. Copy Db lab Cp to original listClaimProcs (Db updates in ClaimProcs.ComputeBaseEst())
			//3. Add any new lab Cp (Db inserts in ClaimProcs.ComputeBaseEst())
			List<long> listProcNums=listProceduresTps.Where(x => x.ProcNumLab!=0).Select(x => x.ProcNum).ToList();
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") || listProcNums.Count==0 ){//Not Canada or no labs to consider.
				return;
			}
			List<long> listClaimProcNumsOrig=listClaimProcs.Where(x => listProcNums.Contains(x.ProcNum)).Select(x => x.ClaimProcNum).ToList();
			//Contains CPs we want to refresh and any that were added. Only look at estimates and cap estimates, see ClaimProcs.RefreshForTP(...)
			List<ClaimProc> listClaimProcsDbLabs=ClaimProcs.RefreshForProcs(listProcNums)
				.Where(x => x.Status.In(ClaimProcStatus.Estimate,ClaimProcStatus.CapEstimate)).ToList();
			for(int i=0;i<listClaimProcs.Count;i++) {
				long claimProcNum=listClaimProcs[i].ClaimProcNum;
				if(!listClaimProcNumsOrig.Contains(claimProcNum)) {
					continue;//listClaimProcs[i] is not associated to a lab
				}
				listClaimProcs[i]=ClaimProcs.GetFromList(listClaimProcsDbLabs,claimProcNum);//Update listClaimProcs to reflect changed values.
			}
			//New estimates could have been added in ClaimProcs.CanadianLabBaseEstHelper(...).
			listClaimProcs.AddRange(listClaimProcsDbLabs.Where(x => !listClaimProcNumsOrig.Contains(x.ClaimProcNum)).ToList());
		}

		private static decimal ComputeAllowedAmount(Procedure procedure,ClaimProc claimProc,List<SubstitutionLink> listSubstitutionLinks,Lookup<FeeKey2,Fee> lookupFees,
			List<InsPlan> listInsPlans,BlueBookEstimateData blueBookEstimateData,Appointment appointment=null)
		{
			//List<Fee> listFees) {
			decimal allowed=0;
			if(claimProc!=null) {				
				if(claimProc.AllowedOverride!=-1) {//check for allowed override
					allowed=(decimal)claimProc.AllowedOverride;
				}
				else {
					allowed=InsPlans.GetAllowedForProc(procedure,claimProc,listInsPlans,listSubstitutionLinks,lookupFees,blueBookEstimateData,appointment);
					if(allowed==-1) {//Carrier does not have an allowed fee entered
						allowed=0;
					}
				}
			}
			return allowed;
		}
		
		///<summary>Returns string that will occupy the procedure's 'Appt' cell in the main grid. 'X' if procedure is on scheduled appointment,
		///'P' if on an unscheduled planned appointment, or a blank string if on no appointments.</summary>
		public static string GetApptStatusAbbr(long plannedAptNum,long aptNum, ApptStatus apptStatus) {
			string result="";
			if(plannedAptNum>0) {
				result="P";
			}
			if(aptNum>0) {
				if(apptStatus==ApptStatus.UnschedList) {
					result="U";
				}
				else {
					result="X";
				}
			}
			return result;
		}
	}

	[Serializable]
	public class TPModuleData {
		public Family Fam;
		public Patient Pat;
		public List<InsSub> SubList;
		public List<InsPlan> InsPlanList;
		public List<PatPlan> PatPlanList;
		public List<Benefit> BenefitList;
		public List<Claim> ClaimList;
		public List<ClaimProcHist> HistList;
		public List<SubstitutionLink> ListSubstLinks;
		public List<Procedure> ListProcedures;
		public List<TreatPlan> ListTreatPlans;
		public List<ProcTP>	ListProcTPs;
		public DiscountPlan DiscountPlan;
		public DiscountPlanSub DiscountPlanSub;
	}

	[Serializable]
	public class LoadActiveTPData {
		public List<TreatPlanAttach> ListTreatPlanAttaches;
		public List<Procedure> listProcForTP;
		public List<ClaimProc> ClaimProcList;
		public List<ClaimProcHist> HistList;
		//public List<SubstitutionLink> ListSubstLinks;//already handled above, in TPModuleData
		///<summary>Includes fees for all procedurecodes on this plan. Includes fee schedules for providers on this plan, insurance, etc.  Very concise list.</summary>
		public List<Fee> ListFees;
		public BlueBookEstimateData BlueBookEstimateData;
	}

	public class TpRow {
		public string Done;
		public string Priority;
		public string Tth;
		public string Surf;
		public string Code;
		public string Description;
		public string Prognosis;
		public string Dx;
		public string ProcAbbr;
		public decimal Fee;
		public decimal PriIns;
		public decimal SecIns;
		public decimal Discount;
		public decimal Pat;
		public decimal FeeAllowed;
		public System.Drawing.Color ColorText;
		public System.Drawing.Color ColorLborder;
		public bool Bold;
		public object Tag;
		public decimal TaxEst;
		public long ProvNum;
		public DateTime DateTP;
		public long ClinicNum;
		public string Appt;
		public decimal CatPercUCR;
		public TpRowType RowType;
	}

	public enum TpRowType {
		///<summary>TpRow is linked to a ProcTP.</summary>
		TpRow,
		///<summary>Tp Row is a row simply showing a total.</summary>
		Total,
		///<summary>Tp Row is a row detailing the sub total for a priority group of ProcTPs.</summary>
		SubTotal,
	}
}
