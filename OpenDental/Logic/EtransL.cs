﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public class EtransL {
		
		///<summary>Shows a print preview of the given x835.
		///Provide the a valid claimNum to preview a single claim from the x835.</summary>
		public static void PrintPreview835(X835 x835, long claimNum=0) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.ERA));
			X835 x835Copy=x835.Copy();
			if(claimNum!=0 && x835Copy.ListClaimsPaid.Any(x => x.ClaimNum==claimNum)) {
				x835Copy.ListClaimsPaid.RemoveAll(x => x.ClaimNum!=claimNum);
			}
			SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue=x835Copy;//Required param
			SheetParameter.GetParamByName(sheet.Parameters,"IsSingleClaimPaid").ParamValue=true;//Value is null if not set
			SheetFiller.FillFields(sheet);
			SheetPrinting.Print(sheet,isPreviewMode:true);
		}

		///<summary>Either shows a single ERA if all claimNums share a single ERA.
		///Otherwise shows FormEtrans835PickEra to get user selection and show an ERA.</summary>
		public static void ViewEra(List<long> listClaimNums) {
			List<Etrans835Attach> listEtrans835Attaches=Etrans835Attaches.GetForClaimNums(listClaimNums.ToArray());
			List<Etrans> listEtranss=Etranss.GetMany(listEtrans835Attaches.Select(x => x.EtransNum).ToArray());
			ViewEra(listEtranss,listEtrans835Attaches);
		}
		
		///<summary>Print previews a specific claim on an ERA.</summary>
		public static void ViewEra(Claim claim) {
			List<Etrans835Attach> listAttaches=Etrans835Attaches.GetForClaimNums(claim.ClaimNum);
			List<Etrans> listEtranss=new List<Etrans>();
			if(listAttaches.Count==0) {
				listEtranss=Etranss.GetErasOneClaim(claim.ClaimIdentifier,claim.DateService);
			}
			else {
				listEtranss=Etranss.GetMany(listAttaches.Select(x => x.EtransNum).ToArray());
			}
			ViewEra(listEtranss,listAttaches,claim.ClaimNum);
		}
		
		///<summary></summary>
		private static void ViewEra(List<Etrans> listEtrans,List<Etrans835Attach> listEtrans835Attaches,long claimNumSpecific=0) {
			if(listEtrans.Count==0) {
				MsgBox.Show("X835","No matching ERAs could be located based on the claim identifier.");
				return;
			}
			if(listEtrans.Count==1) {//This also takes care of older 835s such that there are multiple 835s in 1 X12 msg but only 1 etrans row.
				ViewFormForEra(listEtrans[0],specificClaimNum:claimNumSpecific);
				return;
			}
			//Happens for new 835s when there are multiple 835s in the same X12 msg. There will be 1 etrans row for each 835 in the X12 msg.
			FormEtrans835PickEra formEtrans835PickEra=new FormEtrans835PickEra(listEtrans,claimNumSpecific);
			formEtrans835PickEra.Show();
		}

		///<summary>etrans must be the entire object due to butOK_Click(...) calling Etranss.Update(...).
		///Anywhere we pull etrans from Etranss.RefreshHistory(...) will need to pull full object using an additional query.
		///Eventually we should enhance Etranss.RefreshHistory(...) to return full objects.</summary>
		public static void ViewFormForEra(Etrans etrans,Form formParent=null,long specificClaimNum=0) {
			string messageText835=EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum);
			X12object x12Object835=new X12object(messageText835);
			List<string> listTranSetIds=x12Object835.GetTranSetIds();
			if(listTranSetIds.Count>=2 && etrans.TranSetId835=="") {//More than one EOB in the 835 and we do not know which one to pick.
				using FormEtrans835PickEob formEtrans835PickEob=new FormEtrans835PickEob(listTranSetIds,messageText835,etrans,doOpenEtrans835:true);
				formEtrans835PickEob.ShowDialog();
				return;
			}
			FormEtrans835Edit formEtrans835Edit=new FormEtrans835Edit(specificClaimNum);
			formEtrans835Edit.EtransCur=etrans;
			formEtrans835Edit.MessageText835=messageText835;
			formEtrans835Edit.TranSetId835=etrans.TranSetId835;//Empty or null string will cause the first EOB in the 835 to display.
			formEtrans835Edit.Show(formParent);//Non-modal but belongs to the parent form passed in so that it never shows behind.
		}

		///<summary>Returns an EraData which contains lists of Etrans, Etrans835s, and Etrans835Attaches limited by the passed in filters.</summary>
		public static EraData GetEraDataFiltered(bool showStatusAndClinics,List<X835Status> listX835Statuses,List<long> listClinicNumsSelected,
			string amountMin,string amountMax,DateTime dateFrom,DateTime dateTo,string carrierName,string checkTraceNum,string controlId,bool includeAutomatableCarriersOnly,
			bool areAllClinicsSelected,List<X835AutoProcessed> listX835AutoProcessedsStatuses=null,bool includeAcknowledged=true) 
		{
			ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g("FormEtrans835s","Refreshing history."));
			decimal insPaidMin=-1;
			if(amountMin!="") {
				insPaidMin=PIn.Decimal(amountMin);
			}
			decimal insPaidMax=-1;
			if(amountMax!="") {
				insPaidMax=PIn.Decimal(amountMax);
			}
			EraData eraData=new EraData();
			eraData.ListEtrans835s=Etrans835s.GetFiltered(dateFrom,dateTo,carrierName,checkTraceNum,insPaidMin,insPaidMax,controlId,
				listX835AutoProcessedsStatuses,includeAcknowledged,listX835Statuses.ToArray());
			eraData.ListEtrans=Etranss.GetMany(eraData.ListEtrans835s.Select(x => x.EtransNum).ToArray());
			eraData.ListAttached=Etrans835Attaches.GetForEtrans(false,eraData.ListEtrans.Select(x => x.EtransNum).ToArray());//Gets non-db columns also.
			if(showStatusAndClinics && PrefC.HasClinicsEnabled && !areAllClinicsSelected) {//Filter by selected clinics if applicable.
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g("FormEtrans835s","Filtering by clinic."));
				eraData.ListAttached=eraData.ListAttached.FindAll(x => listClinicNumsSelected.Contains(x.ClinicNum));
				List<long> listEtransNums=eraData.ListAttached.Select(x => x.EtransNum).Distinct().ToList();
				eraData.ListEtrans=eraData.ListEtrans.FindAll(x => listEtransNums.Contains(x.EtransNum));
			}
			//Ensure listFilteredEtrans835s are listed in same order and with same total count as the listEtransFiltered.
			eraData.ListEtrans835s=eraData.ListEtrans.Select(x => eraData.ListEtrans835s.FirstOrDefault(y => y.EtransNum==x.EtransNum)).ToList();
			if(includeAutomatableCarriersOnly) {
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g("FormEtrans835s","Filtering by automatable carriers."));
				FilterAutomatableERAs(eraData.ListEtrans,eraData.ListEtrans835s,eraData.ListAttached);
			}
			return eraData;
		}

		///<summary>Removes Etrans and their associated Etrans835s from the lists passed in if they are not for automatable carriers.</summary>
		private static void FilterAutomatableERAs(List<Etrans> listEtranss,List<Etrans835> listEtrans835sFiltered,List<Etrans835Attach> listEtrans835Attaches) {
			List<Carrier> listCarriersForAllEtrans=Carriers.GetCarriers(listEtrans835Attaches.Select(x => x.CarrierNum).Distinct().ToList());
			for(int i=listEtranss.Count-1;i>=0;i--) {
				List<long> listCarrierNumsForEtrans=listEtrans835Attaches.FindAll(x => x.EtransNum==listEtranss[i].EtransNum).Select(x => x.CarrierNum).ToList();
				List<Carrier> listCarriersForEtrans=listCarriersForAllEtrans.FindAll(x => listCarrierNumsForEtrans.Contains(x.CarrierNum));
				Etrans835 etrans835=listEtrans835sFiltered.FirstOrDefault(x => x.EtransNum==listEtranss[i].EtransNum);
				bool isEtransAutomatable=Etranss.IsEtransAutomatable(listCarriersForEtrans,etrans835.PayerName,isFullyAutomatic:false);
				if(isEtransAutomatable) {
					continue;//Keep the etrans and its status in the lists.
				}
				//Otherwise, remove the etrans and its etrans835 from the lists if it cannot be automated.
				listEtranss.RemoveAt(i);
				listEtrans835sFiltered.RemoveAt(i);
			}
		}

		///<summary>Inserts Etrans835s for etrans that don't have them between(inclusive) the dateFrom and dateTo passed in.</summary>
		public static void AddMissingEtrans835s(DateTime dateFrom,DateTime dateTo) {
			List<long> listEtransNumsMissingEtrans835s=Etranss.GetErasMissingEtrans835(dateFrom,dateTo);
			List<Etrans> listEtransMissingEtrans835s=Etranss.GetMany(listEtransNumsMissingEtrans835s.ToArray());
			//Running a set of queries/commands here for each ERA is fine because this will only happen once for each ERA and we have a progress bar.
			for(int i=0;i<listEtransMissingEtrans835s.Count;i++) {
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g("FormEtrans835s","Creating Etrans835 records")+" "+(i+1)+"/"+listEtransMissingEtrans835s.Count);
				string messageText835=EtransMessageTexts.GetMessageText(listEtransMissingEtrans835s[i].EtransMessageTextNum);
				List<Etrans835Attach> listEtrans835Attaches=Etrans835Attaches.GetForEtrans(listEtransMissingEtrans835s[i].EtransNum);
				X835 x835=new X835(listEtransMissingEtrans835s[i],messageText835,listEtransMissingEtrans835s[i].TranSetId835,listEtrans835Attaches);
				Etrans835 etrans835=new Etrans835();
				etrans835.EtransNum=listEtransMissingEtrans835s[i].EtransNum;
				Etrans835s.Upsert(etrans835,x835);
			}
		}

		///<summary>Try to navigate to the account of the patient on an Hx835_Claim.</summary>
		public static void GoToAccountForHx835_Claim(Hx835_Claim hx835_ClaimPaid) {
			if(hx835_ClaimPaid.ClaimNum!=0) {
				Claim claim=Claims.GetClaim(hx835_ClaimPaid.ClaimNum);
				if(claim!=null) {
					GotoModule.GotoAccount(claim.PatNum);
					return;
				}
			}
			PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(false,hx835_ClaimPaid.PatientName.Lname,hx835_ClaimPaid.PatientName.Fname,"","",false,
				"","","","","",0,false,false,DateTime.MinValue,0,"","","","","","","");
			DataTable tablePt=Patients.GetPtDataTable(ptTableSearchParams);//Mimics FormPatientSelect.cs
			if(tablePt.Rows.Count==0) {
				MsgBox.Show("X835","Patient could not be found.  Please manually attach to a claim and try again.");
				return;
			}
			if(tablePt.Rows.Count==1) {
				GotoModule.GotoAccount(PIn.Long(tablePt.Rows[0]["PatNum"].ToString()));
				return;
			}
			MsgBox.Show("X835","Multiple patients with same name found.  Please manually attach to a claim and try again.");
		}
		
		///<summary>Attempts to delete all etrans data for the given x835, returns true if successful otherwise false.
		///Prompts user in either case.</summary>
		public static bool TryDeleteEra(X835 x835,List<Hx835_ShortClaim> listHx835_ShortClaims,List<Hx835_ShortClaimProc> listHx835_ShortClaimProcs, List<Etrans835Attach> listEtrans835Attaches) {
			X835Status x835Status=x835.GetStatus(listHx835_ShortClaims,listHx835_ShortClaimProcs,listEtrans835Attaches);
			switch(x835Status) {
				case X835Status.FinalizedAllDetached:
				case X835Status.Unprocessed:
					if(!MsgBox.Show("X835",MsgBoxButtons.YesNo,"You are about to delete this ERA.\r\nContinue?")) {
						return false;
					}
					break;
				default://Includes X835Status.None, although this None should never happen.
				case X835Status.Partial:
				case X835Status.NotFinalized:
				case X835Status.FinalizedSomeDetached:
				case X835Status.Finalized:
					MsgBox.Show("X835","You cannot delete an ERA with received claims attached.  Manually change status of attached claims before trying again.");
					return false;
			}
			Etranss.Delete835(x835.EtransSource);
			return true;
		}

		///<summary>This function creates the payment claimprocs and displays the payment entry window.
		///Warns the user if a supplemental payment is going to be made. Prompts user to choose an insurance payment plan to associate the payment to
		///if multiple plans are available.</summary>
		public static void ImportEraClaimData(X835 x835,Hx835_Claim hx835_ClaimPaid,Claim claim,Patient patient,List<ClaimProc> listClaimProcsForClaim) {
			bool isSupplementalPay=(claim.ClaimStatus=="R" || listClaimProcsForClaim.All(x => x.Status.In(
			ClaimProcStatus.Received,ClaimProcStatus.Supplemental)));
			//Warn user if they selected a claim which is already received, so they do not accidentally enter a supplemental payment when they meant to enter a new payment.
			if(isSupplementalPay && !MsgBox.Show("FormEtrans835Edit",MsgBoxButtons.YesNo,"You are about to post supplemental payments to this claim.\r\nContinue?")) {
				return;
			}
			long insPayPlanNum=0;
			if(claim.ClaimType!="PreAuth" && claim.ClaimType!="Cap") {//By definition, capitation insurance pays in one lump-sum, not over an extended period of time.
				//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
				List<PayPlan> listPayPlans=PayPlans.GetValidInsPayPlans(claim.PatNum,claim.PlanNum,claim.InsSubNum,claim.ClaimNum);
				if(listPayPlans.Count==1) {
					insPayPlanNum=listPayPlans[0].PayPlanNum;
				}
				if(listPayPlans.Count>1) {
					//More than one valid PayPlan.
					using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(listPayPlans);
					formPayPlanSelect.ShowDialog();//Modal because this form allows editing of information.
					if(formPayPlanSelect.DialogResult==DialogResult.OK) {
						insPayPlanNum=formPayPlanSelect.PayPlanNumSelected;
					}
				}
			}
			List<ClaimProc> listClaimProcsForClaimModified=listClaimProcsForClaim.Select(x => x.Copy()).ToList();
			Etranss.TryImportEraClaimData(x835,hx835_ClaimPaid,claim,patient,isAutomatic:false,listClaimProcsForClaimModified,insPayPlanNum);
			Family family=Patients.GetFamily(claim.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(claim.PatNum);
			using FormEtrans835ClaimPay formEtrans835ClaimPay=new FormEtrans835ClaimPay(x835,hx835_ClaimPaid,claim,patient,family,listInsPlans,listPatPlans,listInsSubs,isSupplementalPay);
			formEtrans835ClaimPay.ListClaimProcs=listClaimProcsForClaimModified;
			if(formEtrans835ClaimPay.ShowDialog()!=DialogResult.OK) {//Modal because this window can edit information
				//Supplemental Claimprocs are pre-inserted in TryImportEraClaimData, so we must delete any new claim procs if cancel was clicked.
				List<long> listClaimProcNumsForClaim=listClaimProcsForClaim.Select(x => x.ClaimProcNum).ToList();
				//Any claimprocs in the modified list that were not in the original list, were inserted by TryImportEraClaimData() above.
				List<ClaimProc> listClaimProcsToDelete=listClaimProcsForClaimModified.FindAll(x => !listClaimProcNumsForClaim.Contains(x.ClaimProcNum));
				ClaimProcs.DeleteMany(listClaimProcsToDelete);
			}
		}

		///<summary>Returns false if an error is encountered and payment is not finalized.
		///Attempts to finalize the batch insurance payment for an ERA. The user will finish this process in FormClaimPayBatch.
		///Warns the user if there are preauths that need to be detached, there are unreceived claims,
		///there are claimprocs that are not recieved, or all claims are detached and no payment can be created.</summary>
		public static bool FinalizeBatchPayment(X835 x835) {
			List<Hx835_Claim> listHx835_ClaimsSkippedPreauths=x835.ListClaimsPaid.FindAll(x => x.IsPreauth && !x.IsAttachedToClaim);
			if(listHx835_ClaimsSkippedPreauths.Count>0
				&& !MsgBox.Show("X835",MsgBoxButtons.YesNo,
				"There are preauths that have not been attached to an internal claim or have not been detached from the ERA.  "
				+"Would you like to automatically detach and ignore these preauths?")) {
				return false;
			}
			List<Claim> listClaims=x835.GetClaimsForFinalization();
			if(listClaims.Count==0) {
				MsgBox.Show("X835","All claims have been detached from this ERA or are preauths (there is no payment).  Click OK to close the ERA instead.");
				return false;
			}
			if(listClaims.Exists(x => x.ClaimNum==0 || x.ClaimStatus!="R")) {
				#region Column width: PatNum
				int patNumColumnLength=6;//Minimum of 6 because that is the width of the column name "PatNum"
				if(listClaims.Exists(x => x.ClaimNum!=0)) {//There are claims that were found in DB, need to consider claim.PatNum.ToString() lengths.
					patNumColumnLength=Math.Min(8,Math.Max(patNumColumnLength,listClaims.Max(x => x.PatNum.ToString().Length)));
				}
				#endregion
				#region Column width: Patient
				List<long> listPatNums=listClaims.Select(x=>x.PatNum).Distinct().ToList();
				List<Patient> listPatientsLims=Patients.GetLimForPats(listPatNums);
				int maxLen=0;
				//To determine what the longest patient name is, loop through the list and set maxLen to the longest name.
				for(int i=0;i<listPatientsLims.Count;i++){
					string strName=Patients.GetNameLF(listPatientsLims[i]);
					if(strName.Length>maxLen){
						maxLen=strName.Length;
					}
				}
				int maxNamesLength=Math.Max(7,maxLen);//Minimum of 7 to account for column title width "Patient".
				int maxX835NameLength=0;
				if(listClaims.Exists(x => x.ClaimNum==0)) {//There is a claim that could not be found in the DB. Must consider Hx835_Claims.PatientName lengths.
					maxX835NameLength=listClaims
					.Where(x => x.ClaimNum==0)
					.Max(x => ((Hx835_Claim)x.TagOD).PatientName.ToString().Length);
				}
				int maxColumnLength=Math.Max(maxNamesLength,maxX835NameLength);
				#endregion
				#region Construct msg
				string msg="One or more claims are not received.\r\n"
				+"You must receive all of the following claims before finializing payment:\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				msg+="PatNum".PadRight(patNumColumnLength)+"\t"+"Patient".PadRight(maxColumnLength)+"\tDOS       \tTotal Fee\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				for(int i = 0;i<listClaims.Count;i++) {
					if(listClaims[i].ClaimNum==0) {//Current claim was not found in DB and was not detached, so we will use the Hx835_Claim object to get name.
						Hx835_Claim hx835_Claim=(Hx835_Claim)listClaims[i].TagOD;
						msg+="".PadRight(patNumColumnLength)+"\t"//Blank PatNum because unknown.
							+hx835_Claim.PatientName.ToString().PadRight(maxColumnLength)+"\t"
							+hx835_Claim.DateServiceStart.ToShortDateString()+"\t"
							+POut.Decimal(hx835_Claim.ClaimFee)+"\r\n";
						continue;
					}
					//Current claim was found in DB, so we will use Claim object
					Claim claim=listClaims[i];
					if(claim.ClaimStatus=="R") {
						continue;
					}
					string name=listPatientsLims.Find(x=>x.PatNum==claim.PatNum).GetNameLF().PadRight(maxColumnLength);//why the padRight?
					msg+=claim.PatNum.ToString().PadRight(patNumColumnLength).Substring(0,patNumColumnLength)+"\t"//and especially why the useless substring? 
						+name.Substring(0,maxColumnLength)+"\t"
						+claim.DateService.ToShortDateString()+"\t"
						+POut.Double(claim.ClaimFee)+"\r\n";
				}
				#endregion
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForClaims(listClaims.Where(x => x.ClaimNum!=0).Select(x=>x.ClaimNum).ToList());
			if(listClaimProcsAll.Exists(x => !x.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
				int patNumColumnLength=Math.Max(6,listClaimProcsAll.Max(x => x.PatNum.ToString().Length));//PatNum column length
				List<long> listPatNums=listClaims.Select(x=>x.PatNum).Distinct().ToList();
				List<Patient> listPatientsLims=Patients.GetLimForPats(listPatNums);
				int maxNamesLength=0;
				//To determine what the longest patient name is, loop through the list and set maxLen to the longest name.
				for(int i=0;i<listPatientsLims.Count;i++){
					string strName=Patients.GetNameLF(listPatientsLims[i]);
					if(strName.Length>maxNamesLength){
						maxNamesLength=strName.Length;
					}
				}
				#region Construct msg
				string msg="One or more claim procedures are set to the wrong status and are not ready to be finalized.\r\n"
				+"The acceptable claim procedure statuses are Received, Supplemental and CapClaim.\r\n"
				+"The following claims have claim procedures which need to be modified before finalizing:\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				msg+="PatNum".PadRight(patNumColumnLength)+"\t"+"Patient".PadRight(maxNamesLength)+"\tDOS       \tTotal Fee\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				for(int i=0;i<listClaims.Count;i++){
					List<ClaimProc> listClaimProcs=listClaimProcsAll.FindAll(x=>x.ClaimNum==listClaims[i].ClaimNum);
					if(listClaimProcs.All(x => x.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
						continue;
					}
					string name=listPatientsLims.Find(x=>x.PatNum==listClaims[i].PatNum).GetNameLF().PadRight(maxNamesLength);
					msg+=listClaims[i].PatNum.ToString().PadRight(patNumColumnLength).Substring(0,patNumColumnLength)+"\t"
						+name.Substring(0,maxNamesLength)+"\t"
						+listClaims[i].DateService.ToShortDateString()+"\t"
						+POut.Double(listClaims[i].ClaimFee)+"\r\n";
				}
				#endregion
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			ClaimPayment claimPayment=new ClaimPayment();
			Patient patient=Patients.GetPat(listClaims[0].PatNum);
			if(!Etranss.TryFinalizeBatchPayment(x835,listClaims,listClaimProcsAll,patient.ClinicNum,claimPayment)) {
				return false;
			}
			Family family=Patients.GetFamily(patient.PatNum);
			FormClaimEdit.FormFinalizePaymentHelper(claimPayment,listClaims[0],patient,Patients.GetFamily(patient.PatNum));
			return true;
		}

		#region Import Insurance Plans

		///<summary>For the given x834, imports all of the information about insurance plans.</summary>
		///<param name="x834">The x834 object. This was generated by taking the .834 and parsing the text.</param>
		///<param name="listPatientsFor834ImportsLimited">A list of all patients from the database.</param>
		///<param name="doCreatePatient">Indicates if a new patient should be made automatically when no patients are found. Correlates to 
		///PrefName.Ins834IsPatientCreate.</param>
		///<param name="doDropExistingInsurance">Indicates that all patient plans that are not in the 834 will be dropped. Correlated to 
		///PrefName.Ins834DropExistingPatPlans</param>
		///<param name="stringBuilderErrorMessages">A stringbuilder that contains any errors.</param>
		///<param name="actionShowStatus">An optional action to take that shows the status of the process to the user via the UI. The current row/index of
		///the patient is the first parameter to the action. The second is the current patient.</param>
		public static ImportInsurancePlansReturnData ImportInsurancePlansUsingPatientLimited(X834 x834,List<PatientFor834Import> listPatientsFor834ImportsLimited,bool doCreatePatient,bool doCreateEmployer,bool doDropExistingInsurance, Action<int,Patient> actionShowStatus = null){
			ImportInsurancePlansReturnData importInsurancePlansReturnData=new ImportInsurancePlansReturnData();
			int rowIndex=1;
			importInsurancePlansReturnData.stringBuilderErrorMessages=new StringBuilder();
			List<int> listImportedSegments=new List<int> ();//Used to reconstruct the 834 with imported patients removed.
			for(int i = 0;i<x834.ListTransactions.Count;i++) {
				Hx834_Tran hx834_Tran=x834.ListTransactions[i];
				for(int j = 0;j<hx834_Tran.ListMembers.Count;j++) {
					Hx834_Member hx834_Member=hx834_Tran.ListMembers[j];
					actionShowStatus?.Invoke(rowIndex,hx834_Member.Pat);
					Employer employerLocal=null;
					if(doCreateEmployer) {
						Hx834_Employer hx834_EmployerImport=hx834_Member.ListMemberEmployers.FirstOrDefault();
						if(hx834_EmployerImport!=null && hx834_EmployerImport.MemberEmployer!=null && !hx834_EmployerImport.MemberEmployer.GetNameLF().IsNullOrEmpty()) {
							string employerPhone="";
							string employerAddr="";
							string employerCity="";
							string employerState="";
							string employerPostal="";
							if(hx834_EmployerImport.MemberEmployerCommunicationsNumbers!=null
								&& !hx834_EmployerImport.MemberEmployerCommunicationsNumbers.CommunicationNumber1.IsNullOrEmpty()) {
								employerPhone=hx834_EmployerImport.MemberEmployerCommunicationsNumbers.CommunicationNumber1;
							}
							if(hx834_EmployerImport.MemberEmployerStreetAddress!=null
								&& !hx834_EmployerImport.MemberEmployerStreetAddress.AddressInformation1.IsNullOrEmpty()) {
								employerAddr=hx834_EmployerImport.MemberEmployerStreetAddress.AddressInformation1;
							}
							if(hx834_EmployerImport.MemberEmployerCityStateZipCode!=null) {
								if(!hx834_EmployerImport.MemberEmployerCityStateZipCode.CityName.IsNullOrEmpty()) {
									employerCity=hx834_EmployerImport.MemberEmployerCityStateZipCode.CityName;
								}
								if(!hx834_EmployerImport.MemberEmployerCityStateZipCode.PostalCode.IsNullOrEmpty()) {
									employerPostal=hx834_EmployerImport.MemberEmployerCityStateZipCode.PostalCode;
								}
								if(!hx834_EmployerImport.MemberEmployerCityStateZipCode.StateOrProvinceCode.IsNullOrEmpty()) {
									employerState=hx834_EmployerImport.MemberEmployerCityStateZipCode.StateOrProvinceCode;
								}
							}
							string employerName=hx834_EmployerImport.MemberEmployer.GetNameLF();
							List<Employer> listEmployersSimilar=Employers.GetAllByName(employerName);
							employerLocal=listEmployersSimilar.FindAll(x => Regex.Replace(x.Phone,"[^0-9]","")==Regex.Replace(employerPhone,"[^0-9]","")
								&& x.Address==employerAddr).FirstOrDefault(); //Check for employers with the imported name, phone, and addr
							if(employerLocal==null) { //If none exist, create one.
								employerLocal=new Employer();
								employerLocal.EmpName=hx834_EmployerImport.MemberEmployer.GetNameLF();
								employerLocal.Address=employerAddr;
								employerLocal.City=employerCity;
								employerLocal.State=employerState;
								employerLocal.Zip=employerPostal;
								employerLocal.Phone=employerPhone;
								Employers.Insert(employerLocal);
								Employers.MakeLog(employerLocal,LogSources.EmployerImport834);
								Employers.RefreshCache();
								importInsurancePlansReturnData.createdEmployerCount++;
							}
						}
					}
					//The patient's status is not affected by the maintenance code.  After reviewing all of the possible maintenance codes in 
					//member.MemberLevelDetail.MaintenanceTypeCode, we believe that all statuses suggest either insert or update, except for "Cancel".
					//Nathan and Derek feel that archiving the patinet in response to a Cancel request is a bit drastic.
					//Thus we ignore the patient maintenance code and always assume insert/update.
					//Even if the status was "Cancel", then updating the patient does not hurt.
					bool isMemberImported=false;
					bool isMultiMatch=false;
					if(hx834_Member.Pat.PatNum==0) {
						//The patient may need to be created below.  However, the patient may have already been inserted in a pervious iteration of this loop
						//in following scenario: Two different 834s include updates for the same patient and both documents are being imported at the same time.
						//If the patient was already inserted, then they would show up in _listPatients and also in the database.  Attempt to locate the patient
						//in _listPatients again before inserting.
						List <PatientFor834Import> listPatientFor834ImportsMatches=PatientFor834Import.GetPatientLimitedsByNameAndBirthday(hx834_Member.Pat,listPatientsFor834ImportsLimited);
						PatientFor834Import.FilterMatchingList(ref listPatientFor834ImportsMatches);
						if(listPatientFor834ImportsMatches.Count==1) {
							hx834_Member.Pat.PatNum=listPatientFor834ImportsMatches[0].PatNum;
						}
						else if(listPatientFor834ImportsMatches.Count > 1) {
							isMultiMatch=true;
						}
					}
					if(isMultiMatch) {
						importInsurancePlansReturnData.skippedPatsCount++;
					}
					else if(hx834_Member.Pat.PatNum==0 && doCreatePatient) {
						//The code here mimcs the behavior of FormPatientSelect.butAddPt_Click().
						Patients.Insert(hx834_Member.Pat,false);
						Patient patientOldMember=hx834_Member.Pat.Copy();
						hx834_Member.Pat.PatStatus=PatientStatus.Patient;
						hx834_Member.Pat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
						if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							//Set the patients primary provider to the practice default provider.
							hx834_Member.Pat.PriProv=Providers.GetDefaultProvider().ProvNum;
						}
						hx834_Member.Pat.ClinicNum=Clinics.ClinicNum;
						hx834_Member.Pat.Guarantor=hx834_Member.Pat.PatNum;
						Patients.Update(hx834_Member.Pat,patientOldMember);
						PatientFor834Import patientFor834ImportLimited = new PatientFor834Import(hx834_Member.Pat);
						int patIdx=listPatientsFor834ImportsLimited.BinarySearch(patientFor834ImportLimited);//Preserve sort order by locating the index in which to insert the newly added patient.
						int insertIdx=~patIdx;//According to MSDN, the index returned by BinarySearch() is a "bitwise compliment", since not currently in list.
						listPatientsFor834ImportsLimited.Insert(insertIdx,patientFor834ImportLimited);
						SecurityLogs.MakeLogEntry(Permissions.PatientCreate,hx834_Member.Pat.PatNum,"Created from Import Ins Plans 834.",LogSources.InsPlanImport834);
						isMemberImported=true;
						importInsurancePlansReturnData.createdPatsCount++;
					}
					else if(hx834_Member.Pat.PatNum==0 && !doCreatePatient) {
						importInsurancePlansReturnData.skippedPatsCount++;
					}
					else {//member.Pat.PatNum!=0
						PatientFor834Import patientFor834ImportDb=listPatientsFor834ImportsLimited.FirstOrDefault(x => x.PatNum==hx834_Member.Pat.PatNum);//Locate by PatNum, in case user selected manually.
						if(patientFor834ImportDb==null) {
							listPatientsFor834ImportsLimited=Patients.GetAllPatsFor834Imports();
						}
						listPatientsFor834ImportsLimited.FirstOrDefault(x => x.PatNum==hx834_Member.Pat.PatNum);//If its still null, the patient really doesn't exist in the DB.
						if(patientFor834ImportDb!=null) {
							Patient patientDB = Patients.GetPat(patientFor834ImportDb.PatNum);
							hx834_Member.MergePatientIntoDbPatient(patientDB);//Also updates the patient to the database and makes log entry.
							listPatientsFor834ImportsLimited.Remove(patientFor834ImportDb);//Remove patient from list so we can add it back in the correct location (in case name or bday changed).
							int patIdx=listPatientsFor834ImportsLimited.BinarySearch(patientFor834ImportDb);//Preserve sort order by locating the index in which to insert the newly added patient.
																													//patIdx could be positive if the user manually selected a patient when there were multiple matches found.
																													//If patIdx is negative, then according to MSDN, the index returned by BinarySearch() is a "bitwise compliment".
																													//If there were mult instances of patDb BinarySearch() would return 0, which should not be complimented (OutOfRangeException)
							int insertIdx=(patIdx>=0)?patIdx:~patIdx;
							listPatientsFor834ImportsLimited.Insert(insertIdx,patientFor834ImportDb);
							isMemberImported=true;
							importInsurancePlansReturnData.updatedPatsCount++;
						}
					}
					if(isMemberImported) {
						//A list of pat plans that should be removed. Only after going through the list of health coverages do we actually drop the plans.
						//Fill this list once and mark all plans to be removed. As the plans appear in the 834, they will be removed from the list.
						List<PatPlan> listPatPlansNumsToRemove=(doDropExistingInsurance ? PatPlans.Refresh(hx834_Member.Pat.PatNum) : new List<PatPlan>());
						//Import insurance changes for patient.
						for(int k = 0;k<hx834_Member.ListHealthCoverage.Count;k++) {
							Hx834_HealthCoverage hx834_HealthCoverage=hx834_Member.ListHealthCoverage[k];
							if(k > 0) {
								rowIndex++;
							}
							List<Carrier> listCarriers=Carriers.GetByNameAndTin(hx834_Tran.Payer.Name,hx834_Tran.Payer.IdentificationCode);
							if(listCarriers.Count==0) {
								Carrier carrier=new Carrier();
								carrier.CarrierName=hx834_Tran.Payer.Name;
								carrier.TIN=hx834_Tran.Payer.IdentificationCode;
								Carriers.Insert(carrier);
								DataValid.SetInvalid(InvalidType.Carriers);
								listCarriers.Add(carrier);
								SecurityLogs.MakeLogEntry(Permissions.CarrierCreate,0,"Carrier '"+carrier.CarrierName
									+"' created from Import Ins Plans 834.",LogSources.InsPlanImport834);
								importInsurancePlansReturnData.createdCarrierCount++;
							}
							//Update insurance plans.  Match based on carrier and SubscriberId. If the maintenance type code is 002 drop the matching
							//ins plan.
							bool is834ExplicitlyDropping=(hx834_HealthCoverage.HealthCoverage.MaintenanceTypeCode=="002");
							//The insPlanNew will only be inserted if necessary.  Created temporarily in order to determine if insert is needed.
							InsPlan insPlanNew=InsertOrUpdateInsPlan(null,hx834_Member,listCarriers[0],employerLocal,false);
							//Since the insurance plans in the 834 do not include very much information, it is likely that many patients will share the same exact plan.
							//We look for an existing plan being used by any other patinents which match the fields we typically import.
							List<InsPlan> listInsPlans=InsPlans.GetAllByCarrierNum(listCarriers[0].CarrierNum);
							InsPlan insPlanMatch=null;
							for(int p = 0;p<listInsPlans.Count;p++) {
								//Set the PlanNums equal so that AreEqualValue() will ignore this field.  We must ignore PlanNum, since we do not know the PlanNum.
								insPlanNew.PlanNum=listInsPlans[p].PlanNum;
								if(InsPlans.AreEqualValue(listInsPlans[p],insPlanNew)) {
									insPlanMatch=listInsPlans[p];
									break;
								}
							}
							Family family=Patients.GetFamily(hx834_Member.Pat.PatNum);
							List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
							List<PatPlan> listPatPlans=PatPlans.Refresh(hx834_Member.Pat.PatNum);
							List<InsPlan> listInsPlansUpdated=new List<InsPlan>();
							List<InsSub> listInsSubsUpdated=new List<InsSub>();
							List<PatPlan> listPatPlansUpdated=new List<PatPlan>();
							InsSub insSubMatch=null;
							PatPlan patPlanMatch=null;
							//Get InsPlans for listInsSubs so we can check the Carrier name. Limits Db calls.
							List<InsPlan> listInsPlansForSubs=InsPlans.GetByInsSubs(listInsSubs.Select(x=>x.InsSubNum).ToList());
							for(int p = 0;p<listInsSubs.Count;p++) {
								InsSub insSub=listInsSubs[p];
								Carrier carrier=Carriers.GetCarrier(listInsPlansForSubs.First(x=>x.PlanNum==insSub.PlanNum).CarrierNum);
								//According to section 1.4.3 of the standard, the preferred method of matching a dependent to a subscriber is to use the subscriberId.
								if(insSub.SubscriberID.Trim()!=hx834_Member.SubscriberId.Trim()
									|| carrier.CarrierName.Trim().ToLower()!=hx834_Tran.Payer.Name.Trim().ToLower()) {
									continue;
								}
								insSubMatch=insSub;
								patPlanMatch=PatPlans.GetFromList(listPatPlans,insSub.InsSubNum);
								break;
							}
							if(is834ExplicitlyDropping) {//Dropping plan.
								if(patPlanMatch==null) {
									//Nothing to do.  The plan either never existed or is already dropped.
									continue;
								}
								DropPlan(patPlanMatch,insPlanMatch,insSubMatch,listCarriers[0]);
								importInsurancePlansReturnData.droppedPatPlanCount++;
							}
							else {//Not dropping plan per 834. Update or Insert the plan.
								bool isInsPlanUpdate=insPlanMatch!=null;//InsPlan actually exists, so the plan itself is just updating.
								insPlanMatch=InsertOrUpdateInsPlan(insPlanMatch,hx834_Member,listCarriers[0],employerLocal,doCreateEmployer: doCreateEmployer);
								if(isInsPlanUpdate && !listInsPlansUpdated.Contains(insPlanMatch)) {
									listInsPlansUpdated.Add(insPlanMatch);
									importInsurancePlansReturnData.updatedInsPlanCount++;
								}
								else {
									importInsurancePlansReturnData.createdInsPlanCount++;
								}
								bool isInsSubUpdate=insSubMatch!=null;//InsSub actually exists, so the ins sub itself is just updating.
								insSubMatch=InsertOrUpdateInsSub(insSubMatch,insPlanMatch,hx834_Member,hx834_HealthCoverage,listCarriers[0]);
								if(isInsSubUpdate && !listInsSubsUpdated.Contains(insSubMatch)) {
									listInsSubsUpdated.Add(insSubMatch);
									importInsurancePlansReturnData.updatedInsSubCount++;
								}
								else {
									importInsurancePlansReturnData.createdInsSubCount++;
								}
								bool isPatPlanUpdate=patPlanMatch!=null;//PatPlan actually exists, so the pat plan itself is just updating.
								patPlanMatch=InsertOrUpdatePatPlan(patPlanMatch,insSubMatch,insPlanMatch,hx834_Member,listCarriers[0],listPatPlans);
								if(isPatPlanUpdate && !listPatPlansUpdated.Contains(patPlanMatch)) {
									listPatPlansUpdated.Add(patPlanMatch);
									importInsurancePlansReturnData.updatedPatPlanCount++;
								}
								else {
									importInsurancePlansReturnData.createdPatPlanCount++;
								}
								//We know this plan appears in the 834. If the user is dropping all patient plans, remove this from the list so it
								//does not get removed. 
								listPatPlansNumsToRemove.RemoveAll(x => x.PatPlanNum==patPlanMatch.PatPlanNum);
							}
						}//end loop k
						 //Drop patient plans that were marked to be dropped. This list will only contain something if doDropExistingPlans was marked as
						 //true.
						if(doDropExistingInsurance && !listPatPlansNumsToRemove.IsNullOrEmpty()) {
							List<InsSub> listInsSubsForLog=InsSubs.GetMany(listPatPlansNumsToRemove.Select(x => x.InsSubNum).ToList());
							List<InsPlan> listInsPlansForLog=InsPlans.GetPlans(listInsSubsForLog.Select(x => x.PlanNum).ToList());
							for(int p=0;p<listPatPlansNumsToRemove.Count;p++){
								InsSub insSub=listInsSubsForLog.FirstOrDefault(x => x.InsSubNum==listPatPlansNumsToRemove[p].InsSubNum);
								InsPlan insPlan=listInsPlansForLog.FirstOrDefault(x => x.PlanNum==insSub.PlanNum);
								Carrier carrier=Carriers.GetFirstOrDefault(x => x.CarrierNum==insPlan.CarrierNum);
								DropPlan(listPatPlansNumsToRemove[p],insPlan,insSub,carrier);
								importInsurancePlansReturnData.droppedPatPlanCount++;
							}
						}
						//Remove the member from the X834.
						int endSegIndex=0;
						if(j<hx834_Tran.ListMembers.Count-1) {
							endSegIndex=hx834_Tran.ListMembers[j+1].MemberLevelDetail.SegmentIndex-1;
						}
						else {
							X12Segment x12Segment=x834.GetNextSegmentById(hx834_Member.MemberLevelDetail.SegmentIndex+1,"SE");//SE segment is required.
							endSegIndex=x12Segment.SegmentIndex-1;
						}
						for(int s = hx834_Member.MemberLevelDetail.SegmentIndex;s<=endSegIndex;s++) {
							listImportedSegments.Add(s);
						}
					}
					rowIndex++;
				}//end loop j
			}//end loop i
			if(listImportedSegments.Count > 0 && importInsurancePlansReturnData.skippedPatsCount > 0) {//Some patients imported, while others did not.
				if(MoveFileToArchiveFolder(x834)) {
					//Save the unprocessed members back to the import directory, so the user can try to import them again.
					File.WriteAllText(x834.FilePath,x834.ReconstructRaw(listImportedSegments));
				}
			}
			else if(listImportedSegments.Count > 0) {//All patinets imported.
				MoveFileToArchiveFolder(x834);
			}
			else if(importInsurancePlansReturnData.skippedPatsCount > 0) {//No patients imported.  All patients were skipped.
																		 //Leave the raw file unaltered and where it is, so it can be processed again.
			}
			return importInsurancePlansReturnData;
		}

		///<summary>Drops the given patplan. Makes a security log.</summary>
		private static void DropPlan(PatPlan patPlan,InsPlan insPlan,InsSub insSub,Carrier carrier) {
			//This code mimics the behavior of FormInsPlan.butDrop_Click(), except here we do not care if there are claims for this plan today.
			//We need this feature to be as streamlined as possible so that it might become an automated process later.
			//Testing for claims on today's date does not seem that useful anyway, or at least not as useful as checking for any claims
			//associated to the plan, instead of just today's date.
			PatPlans.Delete(patPlan.PatPlanNum);//Estimates recomputed within Delete()
			SecurityLogs.MakeLogEntry(Permissions.InsPlanDropPat,patPlan.PatNum,
				"Insurance plan dropped from patient for carrier '"+carrier+"' and groupnum "
				+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
				+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlan.SecDateTEdit);
		}

		///<summary>For the given x834, tries to move the file to the archive folder. Will return if this succeeded or not.</summary>
		private static bool MoveFileToArchiveFolder(X834 x834) {
			string dir=Path.GetDirectoryName(x834.FilePath);
			string dirArchive=ODFileUtils.CombinePaths(dir,"Archive");
			if(!Directory.Exists(dirArchive)) {
				try{
					Directory.CreateDirectory(dirArchive);
				}
				catch(Exception ex){
					if(!ODInitialize.IsRunningInUnitTest) {
						MessageBox.Show(Lan.g("FormEtrans834Preview","Failed to move file")+" '"+x834.FilePath+"' "
							+Lan.g("FormEtrans834Preview","to archive, probably due to a permission issue.")+"  "+ex.Message);
					}
					return false;
				}
			}
			string destPathBasic=ODFileUtils.CombinePaths(dirArchive,Path.GetFileName(x834.FilePath));
			string destPathExt=Path.GetExtension(destPathBasic);
			string destPathBasicRoot=destPathBasic.Substring(0,destPathBasic.Length-destPathExt.Length);
			string destPath=destPathBasic;
			int attemptCount=1;
			while(File.Exists(destPath)) {
				attemptCount++;
				destPath=destPathBasicRoot+"_"+attemptCount+destPathExt;
			}
			try {
				File.Move(x834.FilePath,destPath);
			}
			catch(Exception ex) {
				if(!ODInitialize.IsRunningInUnitTest) {
					MessageBox.Show(Lan.g("FormEtrans834Preview","Failed to move file")+" '"+x834.FilePath+"' "
						+Lan.g("FormEtrans834Preview","to archive, probably due to a permission issue.")+"  "+ex.Message);
				}
				return false;
			}
			return true;
		}

		///<summary>For the given information creates an insurance plan if insPlan is null. If one is passed in, it updates the plan. Returns the
		///inserted/updated InsPlan object. localEmployer can be null.</summary>
		private static InsPlan InsertOrUpdateInsPlan(InsPlan insPlan,Hx834_Member hx834_Member,Carrier carrier,Employer employerLocal,bool isInsertAllowed=true,bool doCreateEmployer=false) {
			//The code below mimics how insurance plans are created in ContrFamily.ToolButIns_Click().
			if(insPlan!=null){
				InsPlan insPlanOld=insPlan.Copy();
				if(hx834_Member.InsFiling!=null) {
					insPlan.FilingCode=hx834_Member.InsFiling.InsFilingCodeNum;
				}
				insPlan.GroupNum=hx834_Member.GroupNum;
				if(doCreateEmployer && employerLocal!=null) {
					insPlan.EmployerNum=employerLocal.EmployerNum;
				}
				if(OpenDentBusiness.Crud.InsPlanCrud.UpdateComparison(insPlan,insPlanOld)) {
					InsPlans.Update(insPlan,insPlanOld);
					InsBlueBooks.UpdateByInsPlan(insPlan);//Update insbluebook entries for insplan because GroupNum has changed.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,0,"Insurance plan for carrier '"+carrier.CarrierName+"' and groupnum "
						+"'"+insPlan.GroupNum+"' edited from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlanOld.SecDateTEdit);
				}
				return insPlan;
			}
			insPlan=new InsPlan();
			if(hx834_Member.InsFiling!=null) {
				insPlan.FilingCode=hx834_Member.InsFiling.InsFilingCodeNum;
			}
			insPlan.GroupName="";
			insPlan.GroupNum=hx834_Member.GroupNum;
			insPlan.PlanNote="";
			insPlan.FeeSched=0;
			insPlan.PlanType="";
			insPlan.ClaimFormNum=PrefC.GetLong(PrefName.DefaultClaimForm);
			insPlan.UseAltCode=false;
			insPlan.ClaimsUseUCR=false;
			insPlan.CopayFeeSched=0;
			insPlan.EmployerNum=0;
			if(doCreateEmployer && employerLocal!=null) {
				insPlan.EmployerNum=employerLocal.EmployerNum;
			}
			insPlan.CarrierNum=carrier.CarrierNum;
			insPlan.AllowedFeeSched=0;
			insPlan.TrojanID="";
			insPlan.DivisionNo="";
			insPlan.IsMedical=false;
			insPlan.FilingCode=0;
			insPlan.DentaideCardSequence=0;
			insPlan.ShowBaseUnits=false;
			insPlan.CodeSubstNone=false;
			insPlan.IsHidden=false;
			insPlan.MonthRenew=0;
			insPlan.FilingCodeSubtype=0;
			insPlan.CanadianPlanFlag="";
			insPlan.CobRule=EnumCobRule.Basic;
			insPlan.HideFromVerifyList=false;
			if(isInsertAllowed) {
				InsPlans.Insert(insPlan);
				SecurityLogs.MakeLogEntry(Permissions.InsPlanCreate,0,"Insurance plan for carrier '"+carrier.CarrierName+"' and groupnum "
					+"'"+insPlan.GroupNum+"' created from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,
					DateTime.MinValue); //new insplan, no date needed
			}
			return insPlan;
		}

		///<summary>For the given information creates an insSub if insSub is null. If one is passed in, it updates the InsSub. Returns the
		///inserted/updated InsSub object.</summary>
		private static InsSub InsertOrUpdateInsSub(InsSub insSub,InsPlan insPlan,Hx834_Member hx834_Member,Hx834_HealthCoverage hx834_HealthCoverage,Carrier carrier) {
			if(insSub==null) {
				insSub=new InsSub();
				insSub.PlanNum=insPlan.PlanNum;
				//According to section 1.4.3 of the 834 standard, subscribers must be sent in the 834 before dependents.
				//This requirement facilitates easier linking of dependents to their subscribers.
				//Since we were not able to locate a subscriber within the family above, we can safely assume that the insurance plan in the 834
				//is for the subscriber.  Thus we can set the Subscriber PatNum to the same PatNum as the patient.
				insSub.Subscriber=hx834_Member.Pat.PatNum;
				insSub.SubscriberID=hx834_Member.SubscriberId;
				insSub.ReleaseInfo=hx834_Member.IsReleaseInfo;
				insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub.DateEffective=hx834_HealthCoverage.DateEffective;
				insSub.DateTerm=hx834_HealthCoverage.DateTerm;
				InsSubs.Insert(insSub);
				SecurityLogs.MakeLogEntry(Permissions.InsPlanCreateSub,insSub.Subscriber,
					"Insurance subscriber created for carrier '"+carrier.CarrierName+"' and groupnum "
					+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
					+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,DateTime.MinValue);
				return insSub;
			}
			InsSub insSubOld=insSub.Copy();
			insSub.DateEffective=hx834_HealthCoverage.DateEffective;
			insSub.DateTerm=hx834_HealthCoverage.DateTerm;
			insSub.ReleaseInfo=hx834_Member.IsReleaseInfo;
			if(OpenDentBusiness.Crud.InsSubCrud.UpdateComparison(insSub,insSubOld)) {
				InsSubs.Update(insSub);
				SecurityLogs.MakeLogEntry(Permissions.InsPlanEditSub,insSub.Subscriber,
					"Insurance subscriber edited for carrier '"+carrier.CarrierName+"' and groupnum "
					+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
					+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insSubOld.SecDateTEdit);
			}
			return insSub;
		}

		///<summary>For the given information creates an patPlan if patPlan is null. If one is passed in, it updates the patPlan. Returns the
		///inserted/updated patPlan object.</summary>
		private static PatPlan InsertOrUpdatePatPlan(PatPlan patPlan,InsSub insSub,InsPlan insPlan,Hx834_Member hx834_Member,
			Carrier carrier,List <PatPlan> listPatPlansOther)
		{
			if(patPlan!=null){
				PatPlan patPlanOld=patPlan.Copy();
				patPlan.Relationship=hx834_Member.PlanRelat;
				if(OpenDentBusiness.Crud.PatPlanCrud.UpdateComparison(patPlan,patPlanOld)) {
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,patPlan.PatNum,"Insurance plan relationship changed from "
						+hx834_Member.PlanRelat+" to "+patPlan.Relationship+" for carrier '"+carrier.CarrierName+"' and groupnum "
						+"'"+insPlan.GroupNum+"' from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlan.SecDateTEdit);
					PatPlans.Update(patPlan);
				}
				return patPlan;
			}
			patPlan=new PatPlan();
			patPlan.Ordinal=0;
			for(int p=0;p<listPatPlansOther.Count;p++) {
				if(listPatPlansOther[p].Ordinal > patPlan.Ordinal) {
					patPlan.Ordinal=listPatPlansOther[p].Ordinal;
				}
			}
			patPlan.Ordinal++;//Greatest ordinal for patient.
			patPlan.PatNum=hx834_Member.Pat.PatNum;
			patPlan.InsSubNum=insSub.InsSubNum;
			patPlan.Relationship=hx834_Member.PlanRelat;
			if(hx834_Member.PlanRelat!=Relat.Self) {
				//This is not needed yet.  If we do this in the future, then we need to mimic the Move tool in the Family module.
				//member.Pat.Guarantor=insSubMatch.Subscriber;
				//Patient memberPatOld=member.Pat.Copy();
				//Patients.Update(member.Pat,memberPatOld);
			}
			PatPlans.Insert(patPlan);
			SecurityLogs.MakeLogEntry(Permissions.InsPlanAddPat,patPlan.PatNum,
				"Insurance plan added to patient for carrier '"+carrier.CarrierName+"' and groupnum "
				+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
				+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlan.SecDateTEdit);

			return patPlan;
		}
		#endregion

		public class ImportInsurancePlansReturnData{
			public int createdPatsCount;
			public int updatedPatsCount;
			public int skippedPatsCount;
			public int createdCarrierCount;
			public int createdInsPlanCount;
			public int updatedInsPlanCount;
			public int createdInsSubCount;
			public int updatedInsSubCount;
			public int createdPatPlanCount;
			public int droppedPatPlanCount;
			public int updatedPatPlanCount;
			public int createdEmployerCount;
			public StringBuilder stringBuilderErrorMessages;
		}
	}
}
