using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeBase;
using OpenDentBusiness.UI;
using OpenDentBusiness.WebTypes;
using PdfSharp.Pdf;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TreatPlans {
		#region Update
		///<summmary>Updates all active and inactive TP's to match the patients current treatment plan type.</summmary>
		public static void UpdateTreatmentPlanType(Patient patient) {
			//No need to check MiddleTierRole; no call to db.
			List<TreatPlan> listTreatPlans=TreatPlans.GetAllForPat(patient.PatNum);
			listTreatPlans.RemoveAll(x => x.TPStatus==TreatPlanStatus.Saved);	//keep active and inactive tp's, not saved ones.
			TreatPlanType treatPlanType=TreatPlanType.Insurance;
			if(DiscountPlanSubs.HasDiscountPlan(patient.PatNum)) {
				treatPlanType=TreatPlanType.Discount;
			}
			for(int i=0;i<listTreatPlans.Count;i++) {
				if(listTreatPlans[i].TPType!=treatPlanType) {
					listTreatPlans[i].TPType=treatPlanType;
					TreatPlans.Update(listTreatPlans[i]);
				}
			}
		}

		///<summary>Used to set or clear out the mobile app device the treatment plan is being added or removed from.</summary>
		public static void UpdateMobileAppDeviceNum(TreatPlan treatPlan,long mobileAppDeviceNum) {
			treatPlan.MobileAppDeviceNum=mobileAppDeviceNum;
			Update(treatPlan);
		}
		#endregion

		///<summary>Gets all Saved TreatPlans for a given Patient, ordered by date.</summary>
		public static List<TreatPlan> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplan "
			  +"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND TPStatus=0 "//Saved
				+"ORDER BY DateTP";
			return Crud.TreatPlanCrud.SelectMany(command);
		}

		public static List<TreatPlan> GetAllForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplan "
				+"WHERE PatNum="+POut.Long(patNum)+" ";
			return Crud.TreatPlanCrud.SelectMany(command);
		}

		public static List<TreatPlan> GetAllCurrentForPat(long patNum) {
			List<TreatPlan> listTreatPlans=GetAllForPat(patNum).Where(x => x.TPStatus!=TreatPlanStatus.Saved)
				.OrderBy(x => x.TPStatus!=TreatPlanStatus.Active)
				.ThenBy(x => x.DateTP).ToList();
			return listTreatPlans;
		}

		///<summary>A single treatplan from the DB.</summary>
		public static TreatPlan GetOne(long treatPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TreatPlan>(MethodBase.GetCurrentMethod(),treatPlanNum);
			}
			return Crud.TreatPlanCrud.SelectOne(treatPlanNum);
		}

		///<summary>Gets the first Active TP from the DB for the patient.  Returns null if no Active TP is found for this patient.</summary>
		public static TreatPlan GetActiveForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TreatPlan>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplan WHERE PatNum="+POut.Long(patNum)+" AND TPStatus="+POut.Int((int)TreatPlanStatus.Active);
			return Crud.TreatPlanCrud.SelectOne(command);
		}

		public static List<TreatPlan> GetAllWithMobileAppDeviceNum(long mobileAppDeviceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
			}
			string command="SELECT * FROM treatplan WHERE MobileAppDeviceNum="+POut.Long(mobileAppDeviceNum)+";";
			return Crud.TreatPlanCrud.SelectMany(command);
		}

		///<summary>Gets TreatPlans from database. Returns null if not found.</summary>
		public static List<TreatPlan> GetTreatPlansForApi(int limit,int offset,long patNum,DateTime dateTime,int tPStatus) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),limit,offset,patNum,dateTime,tPStatus);
			}
			string command="SELECT * FROM treatplan WHERE SecDateTEdit >= "+POut.DateT(dateTime)+" ";
			if(patNum>-1) {
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			if(tPStatus>-1) {
				command+="AND TPStatus="+POut.Long(tPStatus)+" ";
			}
			command+="ORDER BY TreatPlanNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.TreatPlanCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(TreatPlan treatPlan){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlan);
				return;
			}
			Crud.TreatPlanCrud.Update(treatPlan);
		}

		///<summary></summary>
		public static void Update(TreatPlan treatPlan,TreatPlan treatPlanOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlan,treatPlanOld);
				return;
			}
			Crud.TreatPlanCrud.Update(treatPlan,treatPlanOld);
		}

		///<summary></summary>
		public static long Insert(TreatPlan treatPlan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				treatPlan.TreatPlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),treatPlan);
				return treatPlan.TreatPlanNum;
			}
			treatPlan.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.TreatPlanCrud.Insert(treatPlan);
		}

		///<summary>Dependencies checked first and throws an exception if any found. So surround by try catch</summary>
		public static void Delete(TreatPlan treatPlan){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlan);
				return;
			}
			//check proctp for dependencies
			string command="SELECT * FROM proctp WHERE TreatPlanNum ="+POut.Long(treatPlan.TreatPlanNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				//this should never happen
				throw new ApplicationException(Lans.g("TreatPlans","Cannot delete treatment plan because it has ProcTP's attached"));
			}
			command= "DELETE from treatplan WHERE TreatPlanNum = '"+POut.Long(treatPlan.TreatPlanNum)+"'";
 			Db.NonQ(command);
			if(!treatPlan.TPStatus.In(TreatPlanStatus.Saved)) {
			 return;
			}
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll(treatPlan.PatNum);
			if(listMobileAppDevices.Count>0) {
				MobileNotifications.CI_RemoveTreatmentPlan(listMobileAppDevices.First().MobileAppDeviceNum,treatPlan);
			}
		}

		///<summary>Inserts the passed in Treatment plan. Inserts all ProcTP's passed in.</summary>
		public static long CreateArchivedTreatPlan(TreatPlan treatPlan,Patient patient,List<ProcTP> listProcTPsSelected,List<TreatPlanAttach> listTreatPlanAttaches) {
			//No need to check MiddleTierRole; no call to db.
			long retVal=TreatPlans.Insert(treatPlan);
			ProcTP procTP;
			Procedure procedure;
			int itemNo=0;
			for(int i=0;i<listProcTPsSelected.Count;i++) {
				if(listProcTPsSelected[i]==null){
					//user must have highlighted a subtotal row.
					continue;
				}
				procedure=Procedures.GetOneProc(listProcTPsSelected[i].ProcNumOrig,true);
				procTP=new ProcTP();
				procTP.TreatPlanNum=treatPlan.TreatPlanNum;
				procTP.PatNum=patient.PatNum;
				procTP.ProcNumOrig=procedure.ProcNum;
				procTP.ItemOrder=itemNo;
				TreatPlanAttach treatPlanAttach=listTreatPlanAttaches.FirstOrDefault(x=>x.ProcNum==procedure.ProcNum);
				if(treatPlanAttach==null) {
					//This could happen if another workstation completed this procedure just now.
					procTP.Priority=0;
				}
				else {
					procTP.Priority=treatPlanAttach.Priority;
				}
				procTP.ToothNumTP=Tooth.Display(procedure.ToothNum);
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				if(procedureCode.TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum);
				}
				else if(procedureCode.TreatArea==TreatmentArea.Sextant) {
					procTP.Surf=Tooth.GetSextant(procedure.Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					procTP.Surf=procedure.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				procTP.Descript=listProcTPsSelected[i].Descript;
				procTP.FeeAmt=listProcTPsSelected[i].FeeAmt;
				procTP.PriInsAmt=listProcTPsSelected[i].PriInsAmt;
				procTP.SecInsAmt=listProcTPsSelected[i].SecInsAmt;
				procTP.Discount=listProcTPsSelected[i].Discount;
				procTP.PatAmt=listProcTPsSelected[i].PatAmt;
				procTP.Prognosis=listProcTPsSelected[i].Prognosis;
				procTP.Dx=listProcTPsSelected[i].Dx;
				procTP.ProcAbbr=listProcTPsSelected[i].ProcAbbr;
				procTP.FeeAllowed=listProcTPsSelected[i].FeeAllowed;
				procTP.TaxAmt=listProcTPsSelected[i].TaxAmt;
				procTP.ProvNum=listProcTPsSelected[i].ProvNum;
				procTP.DateTP=listProcTPsSelected[i].DateTP;
				procTP.ClinicNum=listProcTPsSelected[i].ClinicNum;
				procTP.CatPercUCR=listProcTPsSelected[i].CatPercUCR;
				ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;
				#region Canadian Lab Fees
				/*
				proc=(Procedure)gridMain.Rows[gridMain.SelectedIndices[i]].Tag;
				procTP=new ProcTP();
				procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatCur.PatNum;
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ItemOrder=itemNo;
				procTP.Priority=proc.Priority;
				procTP.ToothNumTP="";
				procTP.Surf="";
				procTP.Code=proc.LabProcCode;
				procTP.Descript=gridMain.Rows[gridMain.SelectedIndices[i]]
					.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Description"))].Text;
				if(checkShowFees.Checked) {
					procTP.FeeAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Fee"))].Text);
				}
				if(checkShowIns.Checked) {
					procTP.PriInsAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Pri Ins"))].Text);
					procTP.SecInsAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Sec Ins"))].Text);
					procTP.PatAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Pat"))].Text);
				}
				ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;*/
				#endregion Canadian Lab Fees
			}
			return retVal;
		}

		///<summary>Gets the hashstring for generating signatures.
		///Should only be used when saving signatures, for validating see GetKeyDataForSignatureHash() and GetHashStringForSignature()</summary>
		public static string GetKeyDataForSignatureSaving(TreatPlan treatPlan,List<ProcTP> listProcTPs) {
			//No need to check MiddleTierRole; no call to db.
			string keyData = GetKeyDataForSignatureHash(treatPlan,listProcTPs);
			return GetHashStringForSignature(keyData);
		}

		///<summary>Gets the key data string needed to create a hashstring to be used later when filling the signature.
		///This is done seperate of the hashing so that new line replacements can be done when validating signatures before hashing.</summary>
		public static string GetKeyDataForSignatureHash(TreatPlan treatPlan,List<ProcTP> listProcTPs) {
			//No need to check MiddleTierRole; no call to db.
			//the key data is a concatenation of the following:
			//tp: Note, DateTP, SignatureText, SignaturePracticeText
			//each proctp: Descript,PatAmt
			//The procedures MUST be in the correct order, and we'll use ItemOrder to order them.
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(treatPlan.Note);
			stringBuilder.Append(treatPlan.DateTP.ToString("yyyyMMdd"));
			stringBuilder.Append(treatPlan.SignatureText);
			stringBuilder.Append(treatPlan.SignaturePracticeText);
			for(int i = 0;i<listProcTPs.Count;i++) {
				stringBuilder.Append(listProcTPs[i].Descript);
				stringBuilder.Append(listProcTPs[i].PatAmt.ToString("F2"));
			}
			return stringBuilder.ToString();
		}

		///<summary>Used to fill the grid on a TP sheet.</summary>
		public static TreatPlan GetTreatPlanListProcTP(TreatPlan treatPlan){
			TPModuleData tpModuleData=TreatmentPlanModules.GetModuleData(treatPlan.PatNum,true);
			LoadActiveTPData loadActiveTPData=TreatmentPlanModules.GetLoadActiveTpData(tpModuleData.Pat,
				treatPlan.TreatPlanNum,
				tpModuleData.BenefitList,
				tpModuleData.PatPlanList,
				tpModuleData.InsPlanList,
				treatPlan.DateTP,
				tpModuleData.SubList,
				PrefC.GetBool(PrefName.InsChecksFrequency),
				isTreatPlanSortByTooth:false,
				tpModuleData.ListSubstLinks);
			if(treatPlan.TPStatus==TreatPlanStatus.Saved){
				treatPlan.ListProcTPs=tpModuleData.ListProcTPs;
			}
			else {
				List<TpRow> listTpRows=TreatmentPlanModules.GetActiveTpPlanTpRows(
					showMaxDeductions:true,
					showDiscounts:true,
					showSubTotals:true,
					showTotals:true,
					treatPlan,
					tpModuleData.Pat,
					treatPlan.DateTP,
					loadActiveTPData,
					tpModuleData.InsPlanList,
					tpModuleData.BenefitList,
					tpModuleData.PatPlanList,
					tpModuleData.ListSubstLinks,
					tpModuleData.SubList,
					tpModuleData.DiscountPlanSub,
					tpModuleData.DiscountPlan,
					tpModuleData.ListProcedures,
					ref loadActiveTPData.ClaimProcList,
					loadActiveTPData.HistList,
					doProritizeTotals:true);
				treatPlan.ListProcTPs=ProcTPs.GetProcTPsFromTpRows(
					tpModuleData.Pat.PatNum,
					listTpRows.FindAll(x=>x.RowType==TpRowType.TpRow),
					loadActiveTPData.listProcForTP,
					loadActiveTPData.ListTreatPlanAttaches);
			}
			return treatPlan;
		}


		///<summary>Gets the hashstring from the provided string that is typically generated from GetStringForSignatureHash().
		///This is done seperate of building the string so that new line replacements can be done when validating signatures before hashing.</summary>
		public static string GetHashStringForSignature(string str) {
			//No need to check MiddleTierRole; no call to db.
			return Encoding.ASCII.GetString(ODCrypt.MD5.Hash(Encoding.UTF8.GetBytes(str)));
		}

		///<summary>This is the automation behind keeping treatplans correct.  Many calls to DB, consider optimizing or calling sparingly.
		///<para>Ensures patients only have one active treatplan, marks extras inactive and creates an active if necessary.</para>
		///<para>Attaches procedures to the active plan if the proc status is TP or status is TPi and the proc is attached to a sched/planned appt.</para>
		///<para>Creates an unassigned treatplan if necessary and attaches any unassigned procedures to it.</para>
		///<para>Also maintains priorities of treatplanattaches and procedures and updates the procstatus of TP and TPi procs if necessary.</para></summary>
		public static void AuditPlans(long patNum,TreatPlanType treatPlanType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,treatPlanType);
				return;
			}
			#region Pseudo Code
			//Get all treatplans for the patient
			//Find active TP if it already exists
			//If more than one active TP, update all but the first to Inactive
			//Find unassigned TP if it already exists
			//Get all treatplanattaches for the treatplans
			//Get all TP and TPi procs for the patient
			//Get list of procs for the active plan, i.e. TPA exists linking to active plan or ProcStatus is TP or attached to sched/planned appt
			//Get list of inactive procs, i.e. ProcStatus is TPi and AptNum is 0 and PlannedAptNum is 0 and no TPA exists linking it to the active plan
			//Create an active plan if one doesn't exist and there are procs that need to be attached to it
			//Create an unassigned plan if one doesn't exist and there are unassigned TPi procs
			//For each proc that should be attached to the active plan
			//  update status from TPi to TP
			//  if TPA exists linking to active plan, update priority to TPA priority
			//  delete any TPA that links the proc to the unassigned plan
			//  if TPA linking the proc to the active plan does not exist, insert one with TPA priority set to the proc priority
			//For each proc that is not attached to the active plan (ProcStatus is TPi)
			//  set proc priority to 0
			//  if TPA does not exist, insert one linking the proc to the unassigned plan with TPA priority 0
			//  if multiple TPAs exist with one linking the proc to the unassigned plan, delete the TPA linking to the unassigned plan
			//Foreach TPA
			//  if TPA links proc to the unassigned plan and TPA exists linking the proc to any other plan, delete the link to the unassigned plan
			//If an unassigned plan exists and there are no TPAs pointing to it, delete the unassigned plan
			#endregion Pseudo Code
			#region Variables
			List<TreatPlan> listTreatPlans=TreatPlans.GetAllForPat(patNum);//All treatplans for the pat. [([Includes Saved Plans]}};
			TreatPlan treatPlanActive=listTreatPlans.FirstOrDefault(x => x.TPStatus==TreatPlanStatus.Active);//can be null
			TreatPlan treatPlanUnassigned=listTreatPlans.FirstOrDefault(x => x.TPStatus==TreatPlanStatus.Inactive && x.Heading==Lans.g("TreatPlans","Unassigned"));//can be null
			List<TreatPlanAttach> listTreatPlanAttaches=TreatPlanAttaches.GetAllForTPs(listTreatPlans.Select(x => x.TreatPlanNum).ToList());
			List<Procedure> listProceduresTpTpi=Procedures.GetProcsByStatusForPat(patNum,new[] { ProcStat.TP,ProcStat.TPi });//All TP and TPi procs for the pat.
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				//Previously we have had Canadian users add labs to TPi parent procs, this is rare and odd.
				//When this happens we must ensure that there are matching TPAs for the labs.
				List<long> listTPAProcNums=listTreatPlanAttaches.Select(x=>x.ProcNum).ToList();
				for(int i=0;i<listProceduresTpTpi.Count();i++) {
					Procedure procedure=listProceduresTpTpi[i];
					if(procedure.ProcNumLab!=0  && //proc is a Canadian lab
						!listTPAProcNums.Contains(procedure.ProcNum) && //Lab does not have a TreatPlanAttach
						listTPAProcNums.Contains(procedure.ProcNumLab))//Parent proc has a TreatPlanAttach
					{
						TreatPlanAttach treatPlanAttach=new TreatPlanAttach();
						treatPlanAttach.ProcNum=procedure.ProcNum;
						treatPlanAttach.TreatPlanNum=listTreatPlanAttaches.First(x=>x.ProcNum==procedure.ProcNumLab).TreatPlanNum;
						listTreatPlanAttaches.Add(treatPlanAttach);
					}
				}
			}
			List<Procedure> listProceduresForActive=new List<Procedure>();//All procs that should be linked to the active plan (can be linked to inactive plans as well)
			List<Procedure> listProceduresForInactive=new List<Procedure>();//All procs that should not be linked to the active plan (linked to inactive or unnasigned)
			long[] arrayProcNumsTpa=listTreatPlanAttaches.Select(x => x.ProcNum).ToArray();//All procnums from listTPAs, makes it easier to see if a TPA exists for a proc
			DiscountPlanSub discountPlanSub=DiscountPlanSubs.GetSubForPat(patNum);
			DiscountPlan discountPlan=DiscountPlans.GetForPats(new List<long>{ patNum }).FirstOrDefault();
			#endregion Variables
			#region Fill Proc Lists and Create Active and Unassigned Plans
			for(int i=0;i<listProceduresTpTpi.Count;i++) {//puts each procedure in listProcsForActive or listProcsForInactive
				if(listProceduresTpTpi[i].ProcStatus==ProcStat.TP	//all TP procs should be linked to active plan
					|| listProceduresTpTpi[i].AptNum>0								//all procs attached to an appt should be linked to active plan
					|| listProceduresTpTpi[i].PlannedAptNum>0				//all procs attached to a planned appt should be linked to active plan
					|| (treatPlanActive!=null							//if active plan exists and proc is linked to it, add to list
					&& listTreatPlanAttaches.Any(x => x.ProcNum==listProceduresTpTpi[i].ProcNum && x.TreatPlanNum==treatPlanActive.TreatPlanNum)))
				{
					listProceduresForActive.Add(listProceduresTpTpi[i]);
				}
				else {//TPi status, AptNum=0, PlannedAptNum=0, and not attached to active plan
					listProceduresForInactive.Add(listProceduresTpTpi[i]);
				}
			}
			//Create active plan if needed
			if(treatPlanActive==null && listProceduresForActive.Count>0) {
				treatPlanActive=new TreatPlan(); 
				treatPlanActive.Heading=Lans.g("TreatPlans","Active Treatment Plan");
				treatPlanActive.Note=PrefC.GetString(PrefName.TreatmentPlanNote);
				treatPlanActive.TPStatus=TreatPlanStatus.Active;
				treatPlanActive.PatNum=patNum;
				//treatPlanActive.UserNumPresenter=userNum;
				//treatPlanActive.Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				treatPlanActive.SecUserNumEntry=Security.CurUser.UserNum;
				treatPlanActive.TPType=treatPlanType;
				TreatPlans.Insert(treatPlanActive);
				listTreatPlans.Add(treatPlanActive);
			}
			//Update extra active plans to Inactive status, should only ever be one Active status plan
			//All TP procs are linked to the active plan, so proc statuses won't have to change to TPi for procs attached to an "extra" active plan
			List<TreatPlan> listTreatPlansExtra=listTreatPlans.FindAll(x => x.TPStatus==TreatPlanStatus.Active && treatPlanActive!=null 
				&& x.TreatPlanNum!=treatPlanActive.TreatPlanNum);
			for(int i=0;i<listTreatPlansExtra.Count;i++) {
				listTreatPlansExtra[i].TPStatus=TreatPlanStatus.Inactive;
				TreatPlans.Update(listTreatPlansExtra[i]);
			}
			//Create unassigned plan if needed
			if(treatPlanUnassigned==null && listProceduresForInactive.Any(x => !arrayProcNumsTpa.Contains(x.ProcNum))) {
				treatPlanUnassigned=new TreatPlan(); 
				treatPlanUnassigned.Heading=Lans.g("TreatPlans","Unassigned");
				treatPlanUnassigned.Note=PrefC.GetString(PrefName.TreatmentPlanNote);
				treatPlanUnassigned.TPStatus=TreatPlanStatus.Inactive;
				treatPlanUnassigned.PatNum=patNum;
				//treatPlanUnassigned.UserNumPresenter=userNum;
				//treatPlanUnassigned.Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				treatPlanUnassigned.SecUserNumEntry=Security.CurUser.UserNum;
				treatPlanUnassigned.TPType=treatPlanType;
				TreatPlans.Insert(treatPlanUnassigned);
				listTreatPlans.Add(treatPlanUnassigned);
			}
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			if(discountPlanSub!=null && !listProceduresForActive.IsNullOrEmpty()) {
				listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(listProceduresForActive,discountPlanSub:discountPlanSub,discountPlan:discountPlan);
			}
			#endregion Fill Proc Lists and Create Active and Unassigned Plans
			#region Procs for Active Plan
			//Update proc status to TP (from TPi) for all procs that should be linked to the active plan.
			//For procs with an existing TPA linking it to the active plan, update proc priority to the TPA priority.
			//Remove any TPAs linking the proc to the unassigned plan.
			//Create TPAs linking the proc to the active plan if needed with TPA priority set to the proc priority.
			for(int i=0;i<listProceduresForActive.Count();i++){
				Procedure procedureOld=listProceduresForActive[i].Copy();
				listProceduresForActive[i].ProcStatus=ProcStat.TP;
				//checking the array of ProcNums for an existing TPA is fast, so check the list first
				if(arrayProcNumsTpa.Contains(listProceduresForActive[i].ProcNum)) {
					if(treatPlanUnassigned!=null) {//remove any TPAs linking the proc to the unassigned plan
						listTreatPlanAttaches.RemoveAll(x => x.ProcNum==listProceduresForActive[i].ProcNum && x.TreatPlanNum==treatPlanUnassigned.TreatPlanNum);
					}
					TreatPlanAttach treatPlanAttachActivePlan=listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProceduresForActive[i].ProcNum && x.TreatPlanNum==treatPlanActive.TreatPlanNum);
					if(treatPlanAttachActivePlan==null) {//no TPA linking the proc to the active plan, create one with priority equal to the proc priority
						TreatPlanAttach treatPlanAttach=new TreatPlanAttach();
						treatPlanAttach.ProcNum=listProceduresForActive[i].ProcNum;
						treatPlanAttach.TreatPlanNum=treatPlanActive.TreatPlanNum;
						treatPlanAttach.Priority=listProceduresForActive[i].Priority;
						listTreatPlanAttaches.Add(treatPlanAttach);
					}
					else {//TPA linking this proc to the active plan exists, update proc priority to equal TPA priority
						listProceduresForActive[i].Priority=treatPlanAttachActivePlan.Priority;
					}
				}
				else {//no TPAs exist for this proc, add one linking the proc to the active plan and set the TPA priority equal to the proc priority
					TreatPlanAttach treatPlanAttach=new TreatPlanAttach();
					treatPlanAttach.ProcNum=listProceduresForActive[i].ProcNum;
					treatPlanAttach.TreatPlanNum=treatPlanActive.TreatPlanNum;
					treatPlanAttach.Priority=listProceduresForActive[i].Priority;
					listTreatPlanAttaches.Add(treatPlanAttach);
				}
				if(discountPlanSub!=null) {
					listProceduresForActive[i].DiscountPlanAmt=listDiscountPlanProcs.First(x=>x.ProcNum==listProceduresForActive[i].ProcNum).DiscountPlanAmt;
				}
				else {
					listProceduresForActive[i].DiscountPlanAmt=0;
				}
				Procedures.Update(listProceduresForActive[i],procedureOld,isSilent:true); //We want to suppress AvaTax errors here or we could end up with a bunch
			}
			#endregion Procs for Active Plan
			#region Procs for Inactive and Unassigned Plans
			//Update proc priority to 0 for all inactive procs.
			//If no TPA exists for the proc, create a TPA with priority 0 linking the proc to the unassigned plan.
			for(int i=0;i<listProceduresForInactive.Count;i++) {
				Procedure procedureOld=listProceduresForInactive[i].Copy();
				listProceduresForInactive[i].Priority=0;
				Procedures.Update(listProceduresForInactive[i],procedureOld);
				if(treatPlanUnassigned!=null && !arrayProcNumsTpa.Contains(listProceduresForInactive[i].ProcNum)) {
					//no TPAs for this proc, add a new one to the list linking proc to the unassigned plan
					TreatPlanAttach treatPlanAttach=new TreatPlanAttach();
					treatPlanAttach.TreatPlanNum=treatPlanUnassigned.TreatPlanNum;
					treatPlanAttach.ProcNum=listProceduresForInactive[i].ProcNum;
					treatPlanAttach.Priority=0;
					listTreatPlanAttaches.Add(treatPlanAttach);
				}
			}
			#endregion Procs for Inactive and Unassigned Plans
			#region Sync and Clean-Up TreatPlanAttach List
			//Remove any TPAs if the proc isn't in listProcsTpTpi, status could've changed or possibly proc is for a different pat.
			listTreatPlanAttaches.RemoveAll(x => !listProceduresTpTpi.Select(y => y.ProcNum).Contains(x.ProcNum));
			if(treatPlanUnassigned!=null) {//if an unassigned plan exists
				//Remove any TPAs from the list that link a proc to the unassigned plan if there is a TPA that links the proc to any other plan
				listTreatPlanAttaches.RemoveAll(x => x.TreatPlanNum==treatPlanUnassigned.TreatPlanNum
					&& listTreatPlanAttaches.Any(y => y.ProcNum==x.ProcNum && y.TreatPlanNum!=treatPlanUnassigned.TreatPlanNum));
			}
			listTreatPlans.ForEach(x => TreatPlanAttaches.Sync(listTreatPlanAttaches.FindAll(y => y.TreatPlanNum==x.TreatPlanNum),x.TreatPlanNum));
			if(treatPlanUnassigned!=null) {//Must happen after Sync. Delete the unassigned plan if it exists and there are no TPAs pointing to it.
				listTreatPlanAttaches=TreatPlanAttaches.GetAllForTreatPlan(treatPlanUnassigned.TreatPlanNum);//from DB.
				if(listTreatPlanAttaches.Count==0) {//nothing attached to unassigned anymore
					Crud.TreatPlanCrud.Delete(treatPlanUnassigned.TreatPlanNum);
				}
			}
			#endregion Sync and Clean-Up TreatPlanAttach List
		}

		/// <summary>Syncs various related tables with treat plan status. When isMarkingActive is true, will set all other TPs inactive.</summary>
		public static void SyncTreatPlanStatusWithProcs(TreatPlan TreatPlan, bool isMarkingActive, List<TreatPlanAttach> listTreatPlanAttaches, List<TreatPlanAttach> listTreatPlanAttachesAll,List<Procedure> listProceduresTpProcs) {
			//No need to check MiddleTierRole; no call to db.
			//get all TPAttaches for this TP where there is either a procedure with a TPAttach linking it to this TP
			//or, if this TP is active, a procedure linked to an appt by AptNum or PlannedAptNun
			List<TreatPlanAttach> listTreatPlanAttachesNew=listTreatPlanAttaches.FindAll(x => listProceduresTpProcs.Any(y => x.ProcNum==y.ProcNum));
			listProceduresTpProcs.FindAll(x => !listTreatPlanAttachesNew.Any(y => x.ProcNum==y.ProcNum))
				.ForEach(x => listTreatPlanAttachesNew.Add(new TreatPlanAttach() { TreatPlanNum=TreatPlan.TreatPlanNum,ProcNum=x.ProcNum,Priority=0 }));
			TreatPlanAttaches.Sync(listTreatPlanAttachesNew,TreatPlan.TreatPlanNum);
			if(isMarkingActive) {
				TreatPlans.SetOtherActiveTPsToInactive(TreatPlan);
			}
			if(TreatPlan.TPStatus!=TreatPlanStatus.Active) {
				return;
			}
			//we have to this whether we just made this the active or it was already active, otherwise any procs we move off of the active plan will
			//retain the TP status and AuditPlans will throw them back on this TP.
			//Changing the status to TPi of any procs that are not on this plan prevents that from happening.
			List<long> listProcNumsActive=listTreatPlanAttachesNew.Select(x=>x.ProcNum).ToList();
			List<TreatPlanAttach> listTreatPlanAttachesInactive=listTreatPlanAttachesAll.FindAll(x=>!listProcNumsActive.Contains(x.ProcNum));
			Procedures.SetTPActive(TreatPlan.PatNum,listTreatPlanAttachesNew.Select(x => x.ProcNum).ToList());
			for(int i=0;i<listTreatPlanAttachesNew.Count;i++) {
				ProcMultiVisits.UpdateGroupForProc(listTreatPlanAttachesNew[i].ProcNum,ProcStat.TP);
			}
			for(int i=0;i<listTreatPlanAttachesInactive.Count;i++) {
				ProcMultiVisits.UpdateGroupForProc(listTreatPlanAttachesInactive[i].ProcNum,ProcStat.TPi);
			}
		}

		///<summary>Sets the appropriate priority for a given list of procnums corresponding ProcTP or TreatPlanAttaches.</summary>
		public static void SetPriorityForProcs(TreatPlan treatPlan,long priorityDefNum,List<long> listProcNums,int treatPlanCount,bool suppressSecMessage=false) {
			//No remoting role needed. No call to DB.
			if(treatPlanCount>0
				 && (treatPlan.TPStatus==TreatPlanStatus.Active || treatPlan.TPStatus==TreatPlanStatus.Inactive)) 
			{
				TreatPlanAttaches.SetPriorityForTreatPlanProcs(priorityDefNum,treatPlan.TreatPlanNum,listProcNums);
				return;
			}
			//any Saved TP
			if(!Security.IsAuthorized(EnumPermType.TreatPlanEdit,treatPlan.DateTP,suppressSecMessage)) {
				return;
			}
			ProcTPs.SetPriorityForTreatPlanProcs(priorityDefNum,treatPlan.TreatPlanNum,listProcNums);
		}

		public static TreatPlan GetUnassigned(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TreatPlan>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplan "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND TPStatus="+POut.Int((int)TreatPlanStatus.Inactive)+" "
				+"AND Heading='"+POut.String(Lans.g("TreatPlans","Unassigned"))+"'";
			return Crud.TreatPlanCrud.SelectOne(command)??new TreatPlan();
		}

		///<summary>Called after setting the status to treatPlanCur to Active.
		///Updates the status of any other plan with Active status to Inactive.
		///If the original heading of the other plan is "Active Treatment Plan" it will be updated to "Inactive Treatment Plan".</summary>
		public static void SetOtherActiveTPsToInactive(TreatPlan treatPlan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlan);
				return;
			}
			string command="SELECT * FROM treatplan "
				+"WHERE PatNum="+POut.Long(treatPlan.PatNum)+" "
				+"AND TPStatus="+POut.Int((int)TreatPlanStatus.Active)+" "
				+"AND TreatPlanNum!="+POut.Long(treatPlan.TreatPlanNum);
			//Make Active TP's inactive. Rename if TP's still have default name.
			List<TreatPlan> listTreatPlansActive=Crud.TreatPlanCrud.SelectMany(command);
			for(int i=0;i<listTreatPlansActive.Count;i++) {//should only ever be one, but just in case there are multiple this will rectify the problem.
				if(listTreatPlansActive[i].Heading==Lans.g("TreatPlans","Active Treatment Plan")) {
					listTreatPlansActive[i].Heading=Lans.g("TreatPlans","Inactive Treatment Plan");
				}
				listTreatPlansActive[i].TPStatus=TreatPlanStatus.Inactive;
				TreatPlans.Update(listTreatPlansActive[i]);
			}
			//Heading is changed from within the form, if they have changed it back to Inactive Treatment Plan it was deliberate.
			//if(treatPlanCur.Heading==Lans.g("TreatPlans","Inactive Treatment Plan")) {
			//	treatPlanCur.Heading=Lans.g("TreatPlans","Active Treatment Plan");
			//}
			//Not necessary, treatPlanCur should be set to Active prior to calling this function.
			//treatPlanCur.TPStatus=TreatPlanStatus.Active;
			//TreatPlans.Update(treatPlanCur);
		}

		///<summary>May not return correct values if notes are stored with newline characters.</summary>
		public static List<long> GetNumsByNote(string noteOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),noteOld);
			}
			noteOld=noteOld.Replace("\r","");
			//oldNote=oldNote.Replace("\r","").Replace("\n","\r\n");
			//oldNote=oldNote.Replace("\r","").Replace("\n","*?");
			string command="SELECT TreatPlanNum FROM treatplan WHERE REPLACE(Note,'\\r','')='"+POut.String(noteOld)+"' "+
			"AND TPStatus IN ("+POut.Int((int)TreatPlanStatus.Active)+","+POut.Int((int)TreatPlanStatus.Inactive)+")";
			//string command="SELECT TreatPlanNum FROM treatplan WHERE Note='"+POut.String(oldNote)+"' "+
			//	"AND TPStatus IN ("+POut.Int((int)TreatPlanStatus.Active)+","+POut.Int((int)TreatPlanStatus.Inactive)+")";
			return Db.GetListLong(command);
		}

		/// <summary>	Updates the default note on active/inactive treatment plans with new note</summary>
		public static void UpdateNotes(string noteNew, List<long> listTreatPlanNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),noteNew,listTreatPlanNums);
				return;
			}
			if(listTreatPlanNums==null || listTreatPlanNums.Count==0) {
				return;
			}
			string command="UPDATE treatplan SET Note='"+POut.String(noteNew)+"' "
				+"WHERE TreatPlanNum IN ("+string.Join(",",listTreatPlanNums)+")";
 			Db.NonQ(command);
		}

		public static List<TreatPlan> GetFromProcTPs(List<ProcTP> listProcTPs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),listProcTPs);
			}
			List<TreatPlan> listTreatPlans = new List<TreatPlan>();
			if(listProcTPs.Count == 0) {
				return listTreatPlans;
			}
			string command = "SELECT * FROM treatplan WHERE treatplan.TreatPlanNum IN ("+string.Join(",",listProcTPs.Select(x => x.TreatPlanNum))+")";
			listTreatPlans = Crud.TreatPlanCrud.SelectMany(command);
			for(int i=0;i<listTreatPlans.Count;i++) {
				listTreatPlans[i].ListProcTPs = listProcTPs.Where(x => x.TreatPlanNum == listTreatPlans[i].TreatPlanNum).ToList();
			}
			return listTreatPlans;
		}

		///<summary>Returns only 5 columns for all saved treatment plans.</summary>
		public static List<TreatPlan> GetAllSavedLim(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TreatPlan>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command="SELECT TreatPlanNum, PatNum, DateTP, SecUserNumEntry, UserNumPresenter "
				+" FROM treatplan WHERE treatplan.TPStatus="+POut.Int((int)TreatPlanStatus.Saved)+" "
				+"AND DateTP>="+POut.Date(dateStart)+" "
				+"AND DateTP<="+POut.Date(dateEnd)+" ";
			DataTable table=Db.GetTable(command);
			List<TreatPlan> listTreatPlansSavedLim=new List<TreatPlan>();
			for(int i=0;i<table.Rows.Count;i++) {
				TreatPlan treatPlan = new TreatPlan();
				treatPlan.TreatPlanNum=PIn.Long(table.Rows[i]["TreatPlanNum"].ToString());
				treatPlan.PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				treatPlan.DateTP=PIn.Date(table.Rows[i]["DateTP"].ToString());
				treatPlan.SecUserNumEntry=PIn.Long(table.Rows[i]["SecUserNumEntry"].ToString());
				treatPlan.UserNumPresenter=PIn.Long(table.Rows[i]["UserNumPresenter"].ToString());
				listTreatPlansSavedLim.Add(treatPlan);
			}
			return listTreatPlansSavedLim;
		}

		///<summary>Sets every TreatPlan MobileAppDeviceNum to 0 if it matches the passed in mobileAppDeviceNum.</summary>
		public static void RemoveMobileAppDeviceNum(long mobileAppDeviceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mobileAppDeviceNum);
				return;
			}
			string command=$@"
				UPDATE treatplan
				SET MobileAppDeviceNum=0
				WHERE MobileAppDeviceNum={mobileAppDeviceNum}";
			Db.NonQ(command);
		}

		#region Xam TP methods
		///<summary>Returns a PDF for the currently selected TreatmentPlan and sets out TreatmentPlan to selected TreatmentPlan.
		///If nothing is selected in gridPlans then returns null and out TreatPlan is set to null.</summary>
		public static string ReturnBytesOfTreatPlanPDF(TreatPlan treatPlan) {
			//No need to check MiddleTierRole; no call to db.
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			Sheet sheetTP=TreatPlans.CreateSheetFromTreatmentPlan(treatPlan);
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for TreatPlan sheets.
			PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheetTP);
			SheetDrawingJob.SavePdfToFile(pdfDocument,tempFile);
			//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for TreatPlan sheets.
			//Convert the pdf into its raw bytes
			string rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
			//clean up after oursevles
			try {
				File.Delete(tempFile);
			}
			catch(Exception e) {
				e.DoNothing();//clean up on a best attempt basis. Don't want to throw just because we can't delete the temp file for some reason.
			}
			return rawBase64;
		}

		///<summary>To be used when you need a sheet for a TreatPlan and you don't have a reference to OpenDental.</summary>
		public static Sheet CreateSheetFromTreatmentPlan(TreatPlan treatPlan) {
			//No need to check MiddleTierRole; no call to db.
			treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
			//Get the TreatPlanParams associated with the current treatment plan and then delete it. It is only used once to set the parameters here.
			TreatPlanParam treatPlanParamSheet=TreatPlanParams.GetOneByTreatPlanNum(treatPlan.TreatPlanNum);
			TreatPlanParams.Delete(treatPlanParamSheet.TreatPlanParamNum);
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetSheetsDefault(SheetTypeEnum.TreatmentPlan,Clinics.ClinicNum),treatPlan.PatNum);
			//These are all of the different sheet parameters that can be added to a treatment plan
			sheet.Parameters.Add(new SheetParameter(true,"TreatPlan") { ParamValue=treatPlan });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowDiscountNotAutomatic") { ParamValue=treatPlanParamSheet.ShowDiscount });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowDiscount") { ParamValue=treatPlanParamSheet.ShowDiscount });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowMaxDed") { ParamValue=treatPlanParamSheet.ShowMaxDed });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowSubTotals") { ParamValue=treatPlanParamSheet.ShowSubTotals });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowTotals") { ParamValue=treatPlanParamSheet.ShowTotals });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowCompleted") { ParamValue=treatPlanParamSheet.ShowCompleted });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowFees") { ParamValue=treatPlanParamSheet.ShowFees });
			sheet.Parameters.Add(new SheetParameter(true,"checkShowIns") { ParamValue=treatPlanParamSheet.ShowIns });
			sheet.Parameters.Add(new SheetParameter(true,"toothChartImg") { ParamValue=SheetFramework.ToothChartHelper.GetImage(treatPlan.PatNum,
				PrefC.GetBool(PrefName.TreatPlanShowCompleted),treatPlan) 
			});
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			return sheet;
		}

		///<summary>Attempts to sign a treamentplan with the provided signatures. If signaturePractice is not needed (as defined by the TP sheet) then set to null.
		///Returns true if there are no errors, otherwise returns false and sets out error param.</summary>
		public static bool TrySignTreatmentPlan(TreatPlan treatPlan,string signaturePatient,string signaturePractice,out string error) {
			//No need to check MiddleTierRole; no call to db.
			if(!TryValidateSignatures(treatPlan,signaturePatient,signaturePractice,out string patientSignature,out string practiceSignature,out error)) {
				return false;
			}
			UpdateTreatmentPlanSignatures(treatPlan,patientSignature,practiceSignature);
			return true;
			}
		
		///<summary>Returns true if given treatPlan and signatures are valid for DB, provides decrypted signatures when true, practiceSignature can be null if not needed.
		///Otherwise returns false and sets out error.</summary>
		public static bool TryValidateSignatures(TreatPlan treatPlan,string signaturePatient,string signaturePractice,
			out string patientSignature,out string practiceSignature,out string error)
		{
			//No need to check MiddleTierRole; no call to db.
			error=null;
			patientSignature=null;
			practiceSignature=null;
			if(treatPlan==null){
				error="This Treatment Plan no longer exists. Please select and sign a new Treatment Plan and try again.";
			}
			List<ProcTP> listProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
			string keyData=GetKeyDataForSignatureSaving(treatPlan,listProcTPs);
			UTF8Encoding uTF8Encoding=new UTF8Encoding();
			byte[] byteArrayHash=uTF8Encoding.GetBytes(keyData);
			//331 and 79 are the width and height of the signature box in FormTPsign.cs
			patientSignature=SigBox.EncryptSigString(byteArrayHash,GetScaledSignature(signaturePatient));
			if(patientSignature.IsNullOrEmpty()) {
				error="Error occurred when encrypting the patient signature.";
			}
			if(!signaturePractice.IsNullOrEmpty()) {
				practiceSignature=SigBox.EncryptSigString(byteArrayHash,GetScaledSignature(signaturePractice));
				if(practiceSignature.IsNullOrEmpty()) {
					error="Error occurred when encrypting the practice signature.";
				}
			}
			return (error.IsNullOrEmpty());
		}

		///<summary>Updates the given treatPlans signatures in the DB.
		///Both given signatures should be decrypted.</summary>
		public static void UpdateTreatmentPlanSignatures(TreatPlan treatPlan,string patientSignature,string practiceSignature=null) {
			//No need to check MiddleTierRole; no call to db.
			if(!practiceSignature.IsNullOrEmpty()) {
				treatPlan.SignaturePractice=practiceSignature;
				treatPlan.DateTPracticeSigned=DateTime.Now;
			}
			treatPlan.Signature=patientSignature;
			treatPlan.DateTSigned=DateTime.Now;
			Update(treatPlan);
		}

		///<summary>Given a string of points separated by ';' this returns the scaled coordinates.</summary>
		public static string GetScaledSignature(string originalPoints,int signatureBoxWidth=331,int signatureBoxHeight=79){
			//No need to check MiddleTierRole; no call to db.
			string[] stringArrayPoints=originalPoints.Split(new char[] {';'},StringSplitOptions.RemoveEmptyEntries);
			if(stringArrayPoints.IsNullOrEmpty()) {
				return "";
			}
			List<Point> listPointsOrig=new List<Point>();
			for(int i=0;i<stringArrayPoints.Length;i++) {
				string[] stringArrayCoords=stringArrayPoints[i].Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
				Point point=new Point(Convert.ToInt32(stringArrayCoords[0]),Convert.ToInt32(stringArrayCoords[1]));
				listPointsOrig.Add(point);
			}
			//Get the maximum X value, and the corresponding Y value ising the 9:2 aspect ratio.
			int xMax=Math.Max(listPointsOrig.Select(x => x.X).Max()+1,signatureBoxWidth);
			Point pointUseX=new Point(xMax,(int)(xMax/(double)9*2));
			//Get the maximum Y value, and the corresponding X value ising the 9:2 aspect ratio.
			int yMax=Math.Max(listPointsOrig.Select(y => y.Y).Max()+1,signatureBoxHeight);
			Point pointUseY=new Point((int)(yMax*(double)9/2),yMax);
			//Use the larger valued point to make the largest scaling factor to ensure the the signature looks as similar to the original as possible.
			//This is not exact, because we are using the largest points on the signature box, 
			//instead of the size of the signature box that was signed on, since we don't have that information available from the device.
			Point pointUse=pointUseY;
			if(pointUseX.X >= pointUseY.X) {
				pointUse=pointUseX;
			}
			List<Point> listPointsScaled=new List<Point>();
			//Apply the scaling factor to each point that was saved from the device to shrink the signature box down to the size that the sheetfield expects.
			listPointsOrig.ForEach(x => {
				listPointsScaled.Add(new Point(
					(int)Math.Floor(x.X/(pointUse.X/(decimal)signatureBoxWidth)),(int)Math.Floor(x.Y/(pointUse.Y/(decimal)signatureBoxHeight))
				));
			});
			return string.Join(";",listPointsScaled.Select(x => $"{x.X},{x.Y}"));
		}
		#endregion
	}

	

	


}