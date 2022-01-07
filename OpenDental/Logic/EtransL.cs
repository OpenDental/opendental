using System;
using System.Collections.Generic;
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
			List<Etrans835Attach> listAttaches=Etrans835Attaches.GetForClaimNums(listClaimNums.ToArray());
			List<Etrans> listEtrans=Etranss.GetMany(listAttaches.Select(x => x.EtransNum).ToArray());
			ViewEra(listEtrans,listAttaches);
		}
		
		///<summary>Print previews a specific claim on an ERA.</summary>
		public static void ViewEra(Claim claim) {
			List<Etrans835Attach> listAttaches=Etrans835Attaches.GetForClaimNums(claim.ClaimNum);
			List<Etrans> listEtrans=new List<Etrans>();
			if(listAttaches.Count==0) {
				listEtrans=Etranss.GetErasOneClaim(claim.ClaimIdentifier,claim.DateService);
			}
			else {
				listEtrans=Etranss.GetMany(listAttaches.Select(x => x.EtransNum).ToArray());
			}
			ViewEra(listEtrans,listAttaches,claim.ClaimNum);
		}
		
		///<summary></summary>
		private static void ViewEra(List<Etrans> listEtrans,List<Etrans835Attach> listAttaches,long specificClaimNum=0) {
			if(listEtrans.Count==0) {
				MsgBox.Show("X835","No matching ERAs could be located based on the claim identifier.");
			}
			else if(listEtrans.Count==1) {//This also takes care of older 835s such that there are multiple 835s in 1 X12 msg but only 1 etrans row.
				ViewFormForEra(listEtrans[0],specificClaimNum:specificClaimNum);
			}
			else {//Happens for new 835s when there are multiple 835s in the same X12 msg. There will be 1 etrans row for each 835 in the X12 msg.
				FormEtrans835PickEra FormEPE=new FormEtrans835PickEra(listEtrans,specificClaimNum);
				FormEPE.Show();
			}
		}

		///<summary>etrans must be the entire object due to butOK_Click(...) calling Etranss.Update(...).
		///Anywhere we pull etrans from Etranss.RefreshHistory(...) will need to pull full object using an additional query.
		///Eventually we should enhance Etranss.RefreshHistory(...) to return full objects.</summary>
		public static void ViewFormForEra(Etrans etrans,Form formParent=null,long specificClaimNum=0) {
			string messageText835=EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum,false);
			X12object x835=new X12object(messageText835);
			List<string> listTranSetIds=x835.GetTranSetIds();
			if(listTranSetIds.Count>=2 && etrans.TranSetId835=="") {//More than one EOB in the 835 and we do not know which one to pick.
				using FormEtrans835PickEob formPickEob=new FormEtrans835PickEob(listTranSetIds,messageText835,etrans);
				formPickEob.ShowDialog();
			}
			else {
				FormEtrans835Edit Form835=new FormEtrans835Edit(specificClaimNum);
				Form835.EtransCur=etrans;
				Form835.MessageText835=messageText835;
				Form835.TranSetId835=etrans.TranSetId835;//Empty or null string will cause the first EOB in the 835 to display.
				Form835.Show(formParent);//Non-modal but belongs to the parent form passed in so that it never shows behind.
			}
		}
		
		///<summary>Attempts to delete all etrans data for the given x835, returns true if successful otherwise false.
		///Prompts user in either case.</summary>
		public static bool TryDeleteEra(X835 x835,List<Hx835_ShortClaim> listClaimsFor835,List<Hx835_ShortClaimProc> listClaimProcs, List<Etrans835Attach> listAttaches) {
			X835Status status=x835.GetStatus(listClaimsFor835,listClaimProcs,listAttaches);
			switch(status) {
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
		public static void ImportEraClaimData(X835 x835,Hx835_Claim claimPaid,Claim claim,Patient pat,List<ClaimProc> listClaimProcsForClaim) {
			bool isSupplementalPay=(claim.ClaimStatus=="R" || listClaimProcsForClaim.All(x => ListTools.In(x.Status,
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
				else if(listPayPlans.Count>1) {
					//More than one valid PayPlan.
					using FormPayPlanSelect FormPPS=new FormPayPlanSelect(listPayPlans);
					FormPPS.ShowDialog();//Modal because this form allows editing of information.
					if(FormPPS.DialogResult==DialogResult.OK) {
						insPayPlanNum=FormPPS.SelectedPayPlanNum;
					}
				}
			}
			List<ClaimProc> listClaimProcsForClaimModified=listClaimProcsForClaim.Select(x => x.Copy()).ToList();
			Etranss.TryImportEraClaimData(x835,claimPaid,claim,pat,isAutomatic:false,listClaimProcsForClaimModified,insPayPlanNum);
			Family fam=Patients.GetFamily(claim.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(fam);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(claim.PatNum);
			using FormEtrans835ClaimPay formEtrans835ClaimPay=new FormEtrans835ClaimPay(x835,claimPaid,claim,pat,fam,listInsPlans,listPatPlans,listInsSubs,isSupplementalPay);
			formEtrans835ClaimPay.ListClaimProcsForClaim=listClaimProcsForClaimModified;
			if(formEtrans835ClaimPay.ShowDialog()!=DialogResult.OK) {//Modal because this window can edit information
				//Supplemental Claimprocs are pre-inserted in TryImportEraClaimData, so we must delete any new claim procs if cancel was clicked.
				List<long> listClaimProcNumsForClaim=listClaimProcsForClaim.Select(x => x.ClaimProcNum).ToList();
				//Any claimprocs in the modified list that were not in the original list, were inserted by TryImportEraClaimData() above.
				List<ClaimProc> listClaimProcsToDelete=listClaimProcsForClaimModified.FindAll(x => !ListTools.In(x.ClaimProcNum,listClaimProcNumsForClaim));
				ClaimProcs.DeleteMany(listClaimProcsToDelete);
			}
		}

		///<summary>Returns false if an error is encountered and payment is not finalized.
		///Attempts to finalize the batch insurance payment for an ERA. The user will finish this process in FormClaimPayBatch.
		///Warns the user if there are preauths that need to be detached, there are unreceived claims,
		///there are claimprocs that are not recieved, or all claims are detached and no payment can be created.</summary>
		public static bool FinalizeBatchPayment(X835 x835) {
			List<Hx835_Claim> listSkippedPreauths=x835.ListClaimsPaid.FindAll(x => x.IsPreauth && !x.IsAttachedToClaim);
			if(listSkippedPreauths.Count>0
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
				Dictionary<long,string> dictPatNames=Claims.GetAllUniquePatNamesForClaims(listClaims);
				int maxNamesLength=Math.Max(7,dictPatNames.Values.Max(x => x.Length));//Minimum of 7 to account for column title width "Patient".
				int maxX835NameLength=0;
				if(listClaims.Exists(x => x.ClaimNum==0)) {//There is a claim that could not be found in the DB. Must consider Hx835_Claims.PatientName lengths.
					maxX835NameLength=listClaims
					.Where(x => x.ClaimNum==0)
					.Max(x => ((Hx835_Claim)x.TagOD).PatientName.ToString().Length);
				}
				int maxColumnLength=Math.Max(maxNamesLength,maxX835NameLength);
				#endregion
				#region Construct msg
				string msg="One or more claims are not recieved.\r\n"
				+"You must receive all of the following claims before finializing payment:\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				msg+="PatNum".PadRight(patNumColumnLength)+"\t"+"Patient".PadRight(maxColumnLength)+"\tDOS       \tTotal Fee\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				for(int i = 0;i<listClaims.Count;i++) {
					if(listClaims[i].ClaimNum==0) {//Current claim was not found in DB and was not detached, so we will use the Hx835_Claim object to get name.
						Hx835_Claim xClaimCur=(Hx835_Claim)listClaims[i].TagOD;
						msg+="".PadRight(patNumColumnLength)+"\t"//Blank PatNum because unknown.
							+xClaimCur.PatientName.ToString().PadRight(maxColumnLength)+"\t"
							+xClaimCur.DateServiceStart.ToShortDateString()+"\t"
							+POut.Decimal(xClaimCur.ClaimFee)+"\r\n";
						continue;
					}
					//Current claim was found in DB, so we will use Claim object
					Claim claim=listClaims[i];
					if(claim.ClaimStatus=="R") {
						continue;
					}
					msg+=claim.PatNum.ToString().PadRight(patNumColumnLength).Substring(0,patNumColumnLength)+"\t"
						+dictPatNames[claim.PatNum].PadRight(maxColumnLength).Substring(0,maxColumnLength)+"\t"
						+claim.DateService.ToShortDateString()+"\t"
						+POut.Double(claim.ClaimFee)+"\r\n";
				}
				#endregion
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForClaims(listClaims.Where(x => x.ClaimNum!=0).Select(x=>x.ClaimNum).ToList());
			//Dictionary such that the key is a claimNum and the value is a list of associated claimProcs.
			Dictionary<long,List<ClaimProc>> dictClaimProcs=listClaimProcsAll.GroupBy(x => x.ClaimNum)
				.ToDictionary(
					x => x.Key,//ClaimNum
					x=>listClaimProcsAll.FindAll(y => y.ClaimNum==x.Key)//List of claimprocs associated to current claimNum
			);
			if(listClaimProcsAll.Exists(x => !ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
				int patNumColumnLength=Math.Max(6,listClaimProcsAll.Max(x => x.PatNum.ToString().Length));//PatNum column length
				SerializableDictionary<long,string> dictPatNames=Claims.GetAllUniquePatNamesForClaims(listClaims);
				int maxNamesLength=dictPatNames.Values.Max(x => x.Length);
				#region Construct msg
				string msg="One or more claim procedures are set to the wrong status and are not ready to be finalized.\r\n"
				+"The acceptable claim procedure statuses are Received, Supplemental and CapClaim.\r\n"
				+"The following claims have claim procedures which need to be modified before finalizing:\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				msg+="PatNum".PadRight(patNumColumnLength)+"\t"+"Patient".PadRight(maxNamesLength)+"\tDOS       \tTotal Fee\r\n";
				msg+="-------------------------------------------------------------------\r\n";
				foreach(Claim claim in listClaims) {
					List <ClaimProc> listClaimProcs=dictClaimProcs[claim.ClaimNum];
					if(listClaimProcs.All(x => ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
						continue;
					}
					msg+=claim.PatNum.ToString().PadRight(patNumColumnLength).Substring(0,patNumColumnLength)+"\t"
						+dictPatNames[claim.PatNum].PadRight(maxNamesLength).Substring(0,maxNamesLength)+"\t"
						+claim.DateService.ToShortDateString()+"\t"
						+POut.Double(claim.ClaimFee)+"\r\n";
				}
				#endregion
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			ClaimPayment claimPay=new ClaimPayment();
			Patient pat=Patients.GetPat(listClaims[0].PatNum);
			if(!Etranss.TryFinalizeBatchPayment(x835,listClaims,listClaimProcsAll,pat.ClinicNum,claimPay)) {
				return false;
			}
			Family fam=Patients.GetFamily(pat.PatNum);
			FormClaimEdit.FormFinalizePaymentHelper(claimPay,listClaims[0],pat,Patients.GetFamily(pat.PatNum));
			return true;
		}

		#region Import Insurance Plans

		///<summary>For the given x834, imports all of the information about insurance plans.</summary>
		///<param name="x834">The x834 object. This was generated by taking the .834 and parsing the text.</param>
		///<param name="listPatients">A list of all patients from the database.</param>
		///<param name="doCreatePatient">Indicates if a new patient should be made automatically when no patients are found. Correlates to 
		///PrefName.Ins834IsPatientCreate.</param>
		///<param name="doDropExistingInsurance">Indicates that all patient plans that are not in the 834 will be dropped. Correlated to 
		///PrefName.Ins834DropExistingPatPlans</param>
		///<param name="sbErrorMessages">A stringbuilder that contains any errors.</param>
		///<param name="showStatus">An optional action to take that shows the status of the process to the user via the UI. The current row/index of
		///the patient is the first parameter to the action. The second is the current patient.</param>
		public static void ImportInsurancePlans(X834 x834,List<Patient> listPatients,bool doCreatePatient,bool doCreateEmployer,bool doDropExistingInsurance,
			out int createdPatsCount,out int updatedPatsCount,out int skippedPatsCount,out int createdCarrierCount,out int createdInsPlanCount,
			out int updatedInsPlanCount,out int createdInsSubCount,out int updatedInsSubCount,out int createdPatPlanCount,out int droppedPatPlanCount,
			out int updatedPatPlanCount,out int createdEmployerCount,out StringBuilder sbErrorMessages,Action<int,Patient> showStatus=null) 
		{
			int rowIndex=1;
			createdPatsCount=0;
			updatedPatsCount=0;
			skippedPatsCount=0;
			createdCarrierCount=0;
			createdInsPlanCount=0;
			updatedInsPlanCount=0;
			createdInsSubCount=0;
			updatedInsSubCount=0;
			createdPatPlanCount=0;
			createdEmployerCount=0;
			droppedPatPlanCount=0;
			updatedPatPlanCount=0;
			sbErrorMessages=new StringBuilder();
			List<int> listImportedSegments=new List<int> ();//Used to reconstruct the 834 with imported patients removed.
			for(int i=0;i<x834.ListTransactions.Count;i++) {
				Hx834_Tran tran=x834.ListTransactions[i];
				for(int j=0;j<tran.ListMembers.Count;j++) {
					Hx834_Member member=tran.ListMembers[j];
					showStatus?.Invoke(rowIndex,member.Pat);
					Employer localEmployer=null;
					if(doCreateEmployer) {
						Hx834_Employer importEmployer=member.ListMemberEmployers.FirstOrDefault();
						if(importEmployer!=null && importEmployer.MemberEmployer!=null && !importEmployer.MemberEmployer.GetNameLF().IsNullOrEmpty()) {
							string employerPhone="";
							string employerAddr="";
							string employerCity="";
							string employerState="";
							string employerPostal="";
							if(importEmployer.MemberEmployerCommunicationsNumbers!=null
								&& !importEmployer.MemberEmployerCommunicationsNumbers.CommunicationNumber1.IsNullOrEmpty())
							{
								employerPhone=importEmployer.MemberEmployerCommunicationsNumbers.CommunicationNumber1;
							}
							if(importEmployer.MemberEmployerStreetAddress!=null
								&& !importEmployer.MemberEmployerStreetAddress.AddressInformation1.IsNullOrEmpty())
							{
								employerAddr=importEmployer.MemberEmployerStreetAddress.AddressInformation1;
							}
							if(importEmployer.MemberEmployerCityStateZipCode!=null) {
								if(!importEmployer.MemberEmployerCityStateZipCode.CityName.IsNullOrEmpty()) {
									employerCity=importEmployer.MemberEmployerCityStateZipCode.CityName;
								}
								if(!importEmployer.MemberEmployerCityStateZipCode.PostalCode.IsNullOrEmpty()) {
									employerPostal=importEmployer.MemberEmployerCityStateZipCode.PostalCode;
								}
								if(!importEmployer.MemberEmployerCityStateZipCode.StateOrProvinceCode.IsNullOrEmpty()) {
									employerState=importEmployer.MemberEmployerCityStateZipCode.StateOrProvinceCode;
								}
							}
							string employerName=importEmployer.MemberEmployer.GetNameLF();
							List<Employer> listSimilarEmployers=Employers.GetAllByName(employerName);
							localEmployer=listSimilarEmployers.FindAll(x=>Regex.Replace(x.Phone,"[^0-9]","")==Regex.Replace(employerPhone,"[^0-9]","") 
								&& x.Address==employerAddr).FirstOrDefault(); //Check for employers with the imported name, phone, and addr
							if(localEmployer==null) { //If none exist, create one.
								localEmployer=new Employer();
								localEmployer.EmpName=importEmployer.MemberEmployer.GetNameLF();
								localEmployer.Address=employerAddr;
								localEmployer.City=employerCity;
								localEmployer.State=employerState;
								localEmployer.Zip=employerPostal;
								localEmployer.Phone=employerPhone;
								Employers.Insert(localEmployer);
								Employers.MakeLog(localEmployer,LogSources.EmployerImport834);
								Employers.RefreshCache();
								createdEmployerCount++;
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
					if(member.Pat.PatNum==0) {
						//The patient may need to be created below.  However, the patient may have already been inserted in a pervious iteration of this loop
						//in following scenario: Two different 834s include updates for the same patient and both documents are being imported at the same time.
						//If the patient was already inserted, then they would show up in _listPatients and also in the database.  Attempt to locate the patient
						//in _listPatients again before inserting.
						List <Patient> listPatientMatches=Patients.GetPatientsByNameAndBirthday(member.Pat,listPatients);
						if(listPatientMatches.Count==1) {
							member.Pat.PatNum=listPatientMatches[0].PatNum;
						}
						else if(listPatientMatches.Count > 1) {
							isMultiMatch=true;
						}
					}
					if(isMultiMatch) {
						skippedPatsCount++;
					}
					else if(member.Pat.PatNum==0 && doCreatePatient) {
						//The code here mimcs the behavior of FormPatientSelect.butAddPt_Click().
						Patients.Insert(member.Pat,false);
						Patient memberPatOld=member.Pat.Copy();
						member.Pat.PatStatus=PatientStatus.Patient;
						member.Pat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
						if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							//Set the patients primary provider to the practice default provider.
							member.Pat.PriProv=Providers.GetDefaultProvider().ProvNum;
						}
						member.Pat.ClinicNum=Clinics.ClinicNum;
						member.Pat.Guarantor=member.Pat.PatNum;
						Patients.Update(member.Pat,memberPatOld);
						int patIdx=listPatients.BinarySearch(member.Pat);//Preserve sort order by locating the index in which to insert the newly added patient.
						int insertIdx=~patIdx;//According to MSDN, the index returned by BinarySearch() is a "bitwise compliment", since not currently in list.
						listPatients.Insert(insertIdx,member.Pat);
						SecurityLogs.MakeLogEntry(Permissions.PatientCreate,member.Pat.PatNum,"Created from Import Ins Plans 834.",LogSources.InsPlanImport834);
						isMemberImported=true;
						createdPatsCount++;
					}
					else if(member.Pat.PatNum==0 && !doCreatePatient) {
						skippedPatsCount++;
					}
					else {//member.Pat.PatNum!=0
						Patient patDb=listPatients.FirstOrDefault(x => x.PatNum==member.Pat.PatNum);//Locate by PatNum, in case user selected manually.
						if(patDb==null) {
							listPatients=Patients.GetAllPatients();
						}
						listPatients.FirstOrDefault(x => x.PatNum==member.Pat.PatNum);//If its still null, the patient really doesn't exist in the DB.
						if(patDb!=null) {
							member.MergePatientIntoDbPatient(patDb);//Also updates the patient to the database and makes log entry.
							listPatients.Remove(patDb);//Remove patient from list so we can add it back in the correct location (in case name or bday changed).
							int patIdx=listPatients.BinarySearch(patDb);//Preserve sort order by locating the index in which to insert the newly added patient.
							//patIdx could be positive if the user manually selected a patient when there were multiple matches found.
							//If patIdx is negative, then according to MSDN, the index returned by BinarySearch() is a "bitwise compliment".
							//If there were mult instances of patDb BinarySearch() would return 0, which should not be complimented (OutOfRangeException)
							int insertIdx=(patIdx>=0)?patIdx:~patIdx; 
							listPatients.Insert(insertIdx,patDb);
							isMemberImported=true;
							updatedPatsCount++;
						}
					}
					if(isMemberImported) {
						//A list of pat plans that should be removed. Only after going through the list of health coverages do we actually drop the plans.
						//Fill this list once and mark all plans to be removed. As the plans appear in the 834, they will be removed from the list.
						List<PatPlan> listPatPlanNumsToRemove=(doDropExistingInsurance ? PatPlans.Refresh(member.Pat.PatNum) : new List<PatPlan>());
						//Import insurance changes for patient.
						for(int k=0;k<member.ListHealthCoverage.Count;k++) {
							Hx834_HealthCoverage healthCoverage=member.ListHealthCoverage[k];
							if(k > 0) {
								rowIndex++;
							}
							List<Carrier> listCarriers=Carriers.GetByNameAndTin(tran.Payer.Name,tran.Payer.IdentificationCode);							
							if(listCarriers.Count==0) {
								Carrier carrier=new Carrier();
								carrier.CarrierName=tran.Payer.Name;
								carrier.TIN=tran.Payer.IdentificationCode;
								Carriers.Insert(carrier);
								DataValid.SetInvalid(InvalidType.Carriers);
								listCarriers.Add(carrier);
								SecurityLogs.MakeLogEntry(Permissions.CarrierCreate,0,"Carrier '"+carrier.CarrierName
									+"' created from Import Ins Plans 834.",LogSources.InsPlanImport834);
								createdCarrierCount++;
							}
							//Update insurance plans.  Match based on carrier and SubscriberId. If the maintenance type code is 002 drop the matching
							//ins plan.
							bool is834ExplicitlyDropping=(healthCoverage.HealthCoverage.MaintenanceTypeCode=="002");
							//The insPlanNew will only be inserted if necessary.  Created temporarily in order to determine if insert is needed.
							InsPlan insPlanNew=InsertOrUpdateInsPlan(null,member,listCarriers[0],localEmployer,false);
							//Since the insurance plans in the 834 do not include very much information, it is likely that many patients will share the same exact plan.
							//We look for an existing plan being used by any other patinents which match the fields we typically import.
							List<InsPlan> listInsPlans=InsPlans.GetAllByCarrierNum(listCarriers[0].CarrierNum);
							InsPlan insPlanMatch=null;
							for(int p=0;p<listInsPlans.Count;p++) {
								//Set the PlanNums equal so that AreEqualValue() will ignore this field.  We must ignore PlanNum, since we do not know the PlanNum.
								insPlanNew.PlanNum=listInsPlans[p].PlanNum;
								if(InsPlans.AreEqualValue(listInsPlans[p],insPlanNew)) {
									insPlanMatch=listInsPlans[p];
									break;
								}
							}
							Family fam=Patients.GetFamily(member.Pat.PatNum);
							List<InsSub> listInsSubs=InsSubs.RefreshForFam(fam);							
							List<PatPlan> listPatPlans=PatPlans.Refresh(member.Pat.PatNum);
							List<InsPlan> listUpdatedInsPlans=new List<InsPlan>();
							List<InsSub> listUpdatedInsSubs=new List<InsSub>();
							List<PatPlan> listUpdatedPatPlans=new List<PatPlan>();
							InsSub insSubMatch=null;							
							PatPlan patPlanMatch=null;
							//Get InsPlans for listInsSubs so we can check the Carrier name. Limits Db calls.
							List<InsPlan> listPlansForSubs=InsPlans.GetByInsSubs(listInsSubs.Select(x=>x.InsSubNum).ToList());
							for(int p=0;p<listInsSubs.Count;p++) {
								InsSub insSub=listInsSubs[p];
								Carrier carrier=Carriers.GetCarrier(listPlansForSubs.First(x=>x.PlanNum==insSub.PlanNum).CarrierNum);
								//According to section 1.4.3 of the standard, the preferred method of matching a dependent to a subscriber is to use the subscriberId.
								if(insSub.SubscriberID.Trim()!=member.SubscriberId.Trim()
									|| carrier.CarrierName.Trim().ToLower()!=tran.Payer.Name.Trim().ToLower())
								{
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
								droppedPatPlanCount++;
							}
							else {//Not dropping plan per 834. Update or Insert the plan.
								bool isInsPlanUpdate=insPlanMatch!=null;//InsPlan actually exists, so the plan itself is just updating.
								insPlanMatch=InsertOrUpdateInsPlan(insPlanMatch,member,listCarriers[0],localEmployer,doCreateEmployer:doCreateEmployer);
								if(isInsPlanUpdate && !listUpdatedInsPlans.Contains(insPlanMatch)) {
									listUpdatedInsPlans.Add(insPlanMatch);
									updatedInsPlanCount++;
								}
								else {
									createdInsPlanCount++;
								}
								bool isInsSubUpdate=insSubMatch!=null;//InsSub actually exists, so the ins sub itself is just updating.
								insSubMatch=InsertOrUpdateInsSub(insSubMatch,insPlanMatch,member,healthCoverage,listCarriers[0]);
								if(isInsSubUpdate && !listUpdatedInsSubs.Contains(insSubMatch)) {
									listUpdatedInsSubs.Add(insSubMatch);
									updatedInsSubCount++;
								}
								else {
									createdInsSubCount++;
								}
								bool isPatPlanUpdate=patPlanMatch!=null;//PatPlan actually exists, so the pat plan itself is just updating.
								patPlanMatch=InsertOrUpdatePatPlan(patPlanMatch,insSubMatch,insPlanMatch,member,listCarriers[0],listPatPlans);
								if(isPatPlanUpdate && !listUpdatedPatPlans.Contains(patPlanMatch)) {
									listUpdatedPatPlans.Add(patPlanMatch);
									updatedPatPlanCount++;
								}
								else {
									createdPatPlanCount++;
								}
								//We know this plan appears in the 834. If the user is dropping all patient plans, remove this from the list so it
								//does not get removed. 
								listPatPlanNumsToRemove.RemoveAll(x => x.PatPlanNum==patPlanMatch.PatPlanNum);
							}
						}//end loop k
						//Drop patient plans that were marked to be dropped. This list will only contain something if doDropExistingPlans was marked as
						//true.
						if(doDropExistingInsurance && !listPatPlanNumsToRemove.IsNullOrEmpty()) {
							List<InsSub> listInsSubsForLog=InsSubs.GetMany(listPatPlanNumsToRemove.Select(x => x.InsSubNum).ToList());
							List<InsPlan> listInsPlansForLog=InsPlans.GetPlans(listInsSubsForLog.Select(x => x.PlanNum).ToList());
							foreach(PatPlan plan in listPatPlanNumsToRemove) {
								InsSub insSub=listInsSubsForLog.FirstOrDefault(x => x.InsSubNum==plan.InsSubNum);
								InsPlan insPlan=listInsPlansForLog.FirstOrDefault(x => x.PlanNum==insSub.PlanNum);
								Carrier carrier=Carriers.GetFirstOrDefault(x => x.CarrierNum==insPlan.CarrierNum);
								DropPlan(plan,insPlan,insSub,carrier);
								droppedPatPlanCount++;
							}
						}
						//Remove the member from the X834.
						int endSegIndex=0;
						if(j<tran.ListMembers.Count-1) {
							endSegIndex=tran.ListMembers[j+1].MemberLevelDetail.SegmentIndex-1;
						}
						else {
							X12Segment segSe=x834.GetNextSegmentById(member.MemberLevelDetail.SegmentIndex+1,"SE");//SE segment is required.
							endSegIndex=segSe.SegmentIndex-1;
						}
						for(int s=member.MemberLevelDetail.SegmentIndex;s<=endSegIndex;s++) {
							listImportedSegments.Add(s);
						}
					}
					rowIndex++;
				}//end loop j
			}//end loop i
			if(listImportedSegments.Count > 0 && skippedPatsCount > 0) {//Some patients imported, while others did not.
				if(MoveFileToArchiveFolder(x834)) {
					//Save the unprocessed members back to the import directory, so the user can try to import them again.
					File.WriteAllText(x834.FilePath,x834.ReconstructRaw(listImportedSegments));
				}
			}
			else if(listImportedSegments.Count > 0) {//All patinets imported.
				MoveFileToArchiveFolder(x834);
			}
			else if(skippedPatsCount > 0) {//No patients imported.  All patients were skipped.
				//Leave the raw file unaltered and where it is, so it can be processed again.
			}
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
			try {
				string dir=Path.GetDirectoryName(x834.FilePath);
				string dirArchive=ODFileUtils.CombinePaths(dir,"Archive");
				if(!Directory.Exists(dirArchive)) {
					Directory.CreateDirectory(dirArchive);
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
		private static InsPlan InsertOrUpdateInsPlan(InsPlan insPlan,Hx834_Member member,Carrier carrier,Employer localEmployer,bool isInsertAllowed=true,bool doCreateEmployer=false) {
			//The code below mimics how insurance plans are created in ContrFamily.ToolButIns_Click().
			if(insPlan==null) {
				insPlan=new InsPlan();
				if(member.InsFiling!=null) {
					insPlan.FilingCode=member.InsFiling.InsFilingCodeNum;
				}
				insPlan.GroupName="";
				insPlan.GroupNum=member.GroupNum;
				insPlan.PlanNote="";
				insPlan.FeeSched=0;
				insPlan.PlanType="";
				insPlan.ClaimFormNum=PrefC.GetLong(PrefName.DefaultClaimForm);
				insPlan.UseAltCode=false;
				insPlan.ClaimsUseUCR=false;
				insPlan.CopayFeeSched=0;
				insPlan.EmployerNum=0;
				if(doCreateEmployer && localEmployer!=null) {
					insPlan.EmployerNum=localEmployer.EmployerNum;
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
			}
			else {
				InsPlan insPlanOld=insPlan.Copy();
				if(member.InsFiling!=null) {
					insPlan.FilingCode=member.InsFiling.InsFilingCodeNum;
				}
				insPlan.GroupNum=member.GroupNum;
				if(doCreateEmployer && localEmployer!=null) {
					insPlan.EmployerNum=localEmployer.EmployerNum;
				}
				if(OpenDentBusiness.Crud.InsPlanCrud.UpdateComparison(insPlan,insPlanOld)) {
					InsPlans.Update(insPlan,insPlanOld);
					InsBlueBooks.UpdateByInsPlan(insPlan);//Update insbluebook entries for insplan because GroupNum has changed.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,0,"Insurance plan for carrier '"+carrier.CarrierName+"' and groupnum "
						+"'"+insPlan.GroupNum+"' edited from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlanOld.SecDateTEdit);
				}
			}
			return insPlan;
		}

		///<summary>For the given information creates an insSub if insSub is null. If one is passed in, it updates the InsSub. Returns the
		///inserted/updated InsSub object.</summary>
		private static InsSub InsertOrUpdateInsSub(InsSub insSub,InsPlan insPlan,Hx834_Member member,Hx834_HealthCoverage healthCoverage,Carrier carrier) {
			if(insSub==null) {
				insSub=new InsSub();
				insSub.PlanNum=insPlan.PlanNum;
				//According to section 1.4.3 of the 834 standard, subscribers must be sent in the 834 before dependents.
				//This requirement facilitates easier linking of dependents to their subscribers.
				//Since we were not able to locate a subscriber within the family above, we can safely assume that the insurance plan in the 834
				//is for the subscriber.  Thus we can set the Subscriber PatNum to the same PatNum as the patient.
				insSub.Subscriber=member.Pat.PatNum;
				insSub.SubscriberID=member.SubscriberId;
				insSub.ReleaseInfo=member.IsReleaseInfo;
				insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub.DateEffective=healthCoverage.DateEffective;
				insSub.DateTerm=healthCoverage.DateTerm;
				InsSubs.Insert(insSub);
				SecurityLogs.MakeLogEntry(Permissions.InsPlanCreateSub,insSub.Subscriber,
					"Insurance subscriber created for carrier '"+carrier.CarrierName+"' and groupnum "
					+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
					+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,DateTime.MinValue);
			}
			else {
				InsSub insSubOld=insSub.Copy();
				insSub.DateEffective=healthCoverage.DateEffective;
				insSub.DateTerm=healthCoverage.DateTerm;
				insSub.ReleaseInfo=member.IsReleaseInfo;
				if(OpenDentBusiness.Crud.InsSubCrud.UpdateComparison(insSub,insSubOld)) {
					InsSubs.Update(insSub);
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEditSub,insSub.Subscriber,
						"Insurance subscriber edited for carrier '"+carrier.CarrierName+"' and groupnum "
						+"'"+insPlan.GroupNum+"' and subscriber ID '"+insSub.SubscriberID+"' "
						+"from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insSubOld.SecDateTEdit);
				}
			}
			return insSub;
		}

		///<summary>For the given information creates an patPlan if patPlan is null. If one is passed in, it updates the patPlan. Returns the
		///inserted/updated patPlan object.</summary>
		private static PatPlan InsertOrUpdatePatPlan(PatPlan patPlan,InsSub insSub,InsPlan insPlan,Hx834_Member member,
			Carrier carrier,List <PatPlan> listOtherPatPlans)
		{
			if(patPlan==null) {
				patPlan=new PatPlan();
				patPlan.Ordinal=0;
				for(int p=0;p<listOtherPatPlans.Count;p++) {
					if(listOtherPatPlans[p].Ordinal > patPlan.Ordinal) {
						patPlan.Ordinal=listOtherPatPlans[p].Ordinal;
					}
				}
				patPlan.Ordinal++;//Greatest ordinal for patient.
				patPlan.PatNum=member.Pat.PatNum;
				patPlan.InsSubNum=insSub.InsSubNum;
				patPlan.Relationship=member.PlanRelat;
				if(member.PlanRelat!=Relat.Self) {
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
			}
			else {
				PatPlan patPlanOld=patPlan.Copy();
				patPlan.Relationship=member.PlanRelat;
				if(OpenDentBusiness.Crud.PatPlanCrud.UpdateComparison(patPlan,patPlanOld)) {
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,patPlan.PatNum,"Insurance plan relationship changed from "
						+member.PlanRelat+" to "+patPlan.Relationship+" for carrier '"+carrier.CarrierName+"' and groupnum "
						+"'"+insPlan.GroupNum+"' from Import Ins Plans 834.",insPlan.PlanNum,LogSources.InsPlanImport834,insPlan.SecDateTEdit);
					PatPlans.Update(patPlan);
				}
			}
			return patPlan;
		}
		#endregion

	}
}
