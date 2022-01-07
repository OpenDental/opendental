using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using DataConnectionBase;
using System.Reflection;
#if EHRTEST
using EHR;
#endif

namespace OpenDental {
	public partial class FormEHR:FormODBase {
		public long PatNum;
		private Patient PatCur;
		public PatientNote PatNotCur;
		public Family PatFamCur;
		/// <summary>If ResultOnClosing=RxEdit, then this specifies which Rx to open.</summary>
		public long LaunchRxNum;
		///<summary>After this form closes, this can trigger other things to happen.  DialogResult is not checked.  After the other actions are taken, FormEHR opens back up again for seamless user experience.</summary>
		public EhrFormResult ResultOnClosing;
		private List<EhrMu> listMu;
		/// <summary>If ResultOnClosing=MedicationPatEdit, then this specifies which MedicationPat to open.</summary>
		public long LaunchMedicationPatNum;
		///<summary>If set to true, this will cause medical orders window to come up.</summary>
		public bool OnShowLaunchOrders;
		///<summary>This is set every time the form is shown.  It is used to determine if there is an Ehr key for the patient's primary provider.  If not, then The main grid will show empty.</summary>
		private Provider ProvPat;
		///<summary>This will be null if EHR didn't load up.  EHRTEST conditional compilation constant is used because the EHR project is only part of the solution here at HQ.  We need to use late binding in a few places so that it will still compile for people who download our sourcecode.  But late binding prevents us from stepping through for debugging, so the EHRTEST lets us switch to early binding.</summary>
		public static object ObjFormEhrMeasures;
		///<summary>This will be null if EHR didn't load up.</summary>
		public static Assembly AssemblyEHR;

		public FormEHR() {
			InitializeComponent();
			InitializeLayoutManager();
			if(PrefC.GetBoolSilent(PrefName.ShowFeatureEhr,false)) {
				constructObjFormEhrMeasuresHelper();
			}
		}

		///<summary>Constructs the ObjFormEhrMeasures fro use with late binding.</summary>
		private static void constructObjFormEhrMeasuresHelper() {
			string dllPathEHR=ODFileUtils.CombinePaths(Application.StartupPath,"EHR.dll");
			ObjFormEhrMeasures=null;
			AssemblyEHR=null;
			if(File.Exists(dllPathEHR)) {//EHR.dll is available, so load it up
				AssemblyEHR=Assembly.LoadFile(dllPathEHR);
				Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
				ObjFormEhrMeasures=Activator.CreateInstance(type);
				return;
			}
			#if EHRTEST
				ObjFormEhrMeasures=new FormEhrMeasures();
			#endif
		}

		///<summary>Loads a resource file from the EHR assembly and returns the file text as a string.
		///Returns "" is the EHR assembly did not load. strResourceName can be either "CCD" or "CCR".
		///This function performs a late binding to the EHR.dll, because resellers do not have EHR.dll necessarily.</summary>
		public static string GetEhrResource(string strResourceName) {
			if(AssemblyEHR==null) {
				constructObjFormEhrMeasuresHelper();
				if(AssemblyEHR==null) {
					return "";
				}
			}
			Stream stream=AssemblyEHR.GetManifestResourceStream("EHR.Properties.Resources.resources");
			System.Resources.ResourceReader resourceReader=new System.Resources.ResourceReader(stream);
			string strResourceType="";
			byte[] arrayResourceBytes=null;
			resourceReader.GetResourceData(strResourceName,out strResourceType,out arrayResourceBytes);
			resourceReader.Dispose();
			stream.Dispose();
			MemoryStream ms=new MemoryStream(arrayResourceBytes);
			BinaryReader br=new BinaryReader(ms);
			string retVal=br.ReadString();//Removes the leading binary characters from the string.
			ms.Dispose();
			br.Dispose();
			return retVal;
		}

		private void FormEHR_Load(object sender,EventArgs e) {
			//Can't really use this, because it's only loaded once at the very beginning of OD starting up.
		}

		private void FormEHR_Shown(object sender,EventArgs e) {
			ResultOnClosing=EhrFormResult.None;
			PatCur=Patients.GetPat(PatNum);
			ProvPat=Providers.GetProv(PatCur.PriProv);
			labelProvPat.Text=ProvPat.GetLongDesc();
			if(EhrProvKeys.GetKeysByFLName(ProvPat.LName,ProvPat.FName).Count==0) {
				labelProvPat.Text+=" (no ehr provider key entered)";
			}
			if(Security.CurUser.ProvNum==0) {
				labelProvUser.Text="none";
			}
			else {
				Provider provUser=Providers.GetProv(Security.CurUser.ProvNum);
				labelProvUser.Text=Providers.GetLongDesc(provUser.ProvNum);
				if(EhrProvKeys.GetKeysByFLName(provUser.LName,provUser.FName).Count==0) {
					labelProvUser.Text+=" (no ehr provider key entered)";
				}
			}
			FillGridMu();
			if(OnShowLaunchOrders) {
				//LaunchOrdersWindow();
				OnShowLaunchOrders=false;
			}
			//We already indicate that the patient's provider does not have an ehr key entered in labelProvPat.  No need for a popup.
			//This is so that non-ehr providers can still use many of our ehr features.  E.g. vital signs.
			//if(ProvPat.EhrKey=="") {
			//	MessageBox.Show("No ehr provider key entered for this patient's primary provider.");
			//}
		}

		private void FillGridMu() {
			gridMu.BeginUpdate();
			gridMu.ListGridColumns.Clear();
			GridColumn col=new GridColumn("MeasureType",145);
			gridMu.ListGridColumns.Add(col);
			col=new GridColumn("Met",35,HorizontalAlignment.Center);
			gridMu.ListGridColumns.Add(col);
			col=new GridColumn("Details",170);
			gridMu.ListGridColumns.Add(col);
			col=new GridColumn("Click to Take Action",168);
			gridMu.ListGridColumns.Add(col);
			col=new GridColumn("Related Actions",142);
			gridMu.ListGridColumns.Add(col);
			//Always fill the grid regardless if the patient's provider has a valid ehr key.
			//TODO: ProvPat may not be the primary provider in the future.
			if(ProvPat.EhrMuStage==0) {//Use the global preference
				if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==2) {/*Modified Stage 2*/
					gridMu.Title="Modified Stage 2 Meaningful Use for this patient";
					listMu=EhrMeasures.GetMu2Mod(PatCur);
				}
				else if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==1) {/*Stage 2*/
					gridMu.Title="Stage 2 Meaningful Use for this patient";
					listMu=EhrMeasures.GetMu2(PatCur);
				}
				else {/*Stage 1*/
					gridMu.Title="Stage 1 Meaningful Use for this patient";
					listMu=EhrMeasures.GetMu(PatCur);
				}
			}
			else if(ProvPat.EhrMuStage==1) {
				gridMu.Title="Stage 1 Meaningful Use for this patient";
				listMu=EhrMeasures.GetMu(PatCur);
			}
			else if(ProvPat.EhrMuStage==2) {
				gridMu.Title="Stage 2 Meaningful Use for this patient";
				listMu=EhrMeasures.GetMu2(PatCur);
			}
			else if(ProvPat.EhrMuStage==3) {
				gridMu.Title="Modified Stage 2 Meaningful Use for this patient";
				listMu=EhrMeasures.GetMu2Mod(PatCur);
			}
			gridMu.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listMu.Count;i++) {
				row=new GridRow();
				if(ProvPat.EhrMuStage==3) {//Stage 2 Modified
					row.Cells.Add(EhrMeasures.MeasureTypeTitleHelper(listMu[i].MeasureType));
				}
				else {
					row.Cells.Add(listMu[i].MeasureType.ToString());
				}
				if(listMu[i].Met==MuMet.True) {
					row.Cells.Add("X");
					row.ColorBackG=Color.FromArgb(178,255,178);
				}
				else if(listMu[i].Met==MuMet.NA) {
					row.Cells.Add("N/A");
					row.ColorBackG=Color.FromArgb(178,255,178);
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(listMu[i].Details);
				row.Cells.Add(listMu[i].Action);
				row.Cells.Add(listMu[i].Action2);
				gridMu.ListGridRows.Add(row);
			}
			gridMu.EndUpdate();
		}

		private void gridMu_CellClick(object sender,ODGridClickEventArgs e) {
			 
			if(e.Col==3) {
				switch(listMu[e.Row].MeasureType) {
					case EhrMeasureType.ProblemList:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabProblems")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.MedicationList:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabMedications")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.AllergyList:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabAllergies")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Demographics:
						using(FormPatientEdit FormPatEdit=new FormPatientEdit(PatCur,PatFamCur)) {
							FormPatEdit.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.Education:
						using(FormEhrEduResourcesPat FormEDUPat = new FormEhrEduResourcesPat()) {
							FormEDUPat.patCur=PatCur;
							FormEDUPat.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.TimelyAccess:
					case EhrMeasureType.ElectronicCopyAccess:
						using(FormPatientPortal FormPatPort=new FormPatientPortal(PatCur)) {
							FormPatPort.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.ProvOrderEntry:
					case EhrMeasureType.CPOE_MedOrdersOnly:
					case EhrMeasureType.CPOE_PreviouslyOrdered:
						//LaunchOrdersWindow();
						break;
					case EhrMeasureType.Rx:
						//no action available
						break;
					case EhrMeasureType.VitalSigns:
					case EhrMeasureType.VitalSignsBMIOnly:
					case EhrMeasureType.VitalSignsBPOnly:
					case EhrMeasureType.VitalSigns2014:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabVitalSigns")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Smoking:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabTobaccoUse")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Lab:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						using(FormEhrLabOrders FormLP=new FormEhrLabOrders()) {
							FormLP.PatCur=PatCur;
							FormLP.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.ElectronicCopy:
						if(listMu[e.Row].Action=="Provide elect copy to Pt") {
							using(FormEhrElectronicCopy FormE=new FormEhrElectronicCopy()) {
								FormE.PatCur=PatCur;
								FormE.ShowDialog();
							}
							FillGridMu();
						}
						break;
					case EhrMeasureType.ClinicalSummaries:
						using(FormEhrClinicalSummary FormCS=new FormEhrClinicalSummary()) {
							FormCS.PatCur=PatCur;
							FormCS.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.Reminders:
						using(FormEhrReminders FormRem=new FormEhrReminders()) {
							FormRem.PatCur=PatCur;
							FormRem.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.MedReconcile:
						int compare=EhrMeasures.CompareReferralsToReconciles(PatNum);
						if(compare==1 || compare==0) {//Referral count is less than reconcile count or both are zero.
							using FormReferralSelect FormRS=new FormReferralSelect();
							FormRS.IsDoctorSelectionMode=true;
							FormRS.IsSelectionMode=true;
							FormRS.ShowDialog();
							if(FormRS.DialogResult!=DialogResult.OK) {
								return;
							}
							List<RefAttach> RefAttachList=RefAttaches.RefreshFiltered(PatNum,false,0);
							RefAttach refattach=new RefAttach();
							refattach.ReferralNum=FormRS.SelectedReferral.ReferralNum;
							refattach.PatNum=PatNum;
							refattach.RefType=ReferralType.RefFrom;
							refattach.RefDate=DateTime.Today;
							if(FormRS.SelectedReferral.IsDoctor) {//whether using ehr or not
								refattach.IsTransitionOfCare=true;
							}
							int order=0;
							for(int i=0;i<RefAttachList.Count;i++) {
								if(RefAttachList[i].ItemOrder > order) {
									order=RefAttachList[i].ItemOrder;
								}
							}
							refattach.ItemOrder=order+1;
							RefAttaches.Insert(refattach);
							SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refattach.ReferralNum));
						}
						else if(compare==-1) {//The referral count is greater than the reconcile count.
							//So we do not need to show the referral window, we just need to reconcile below.
						}
						using(FormEhrSummaryOfCare FormMedRec=new FormEhrSummaryOfCare()) {
							FormMedRec.PatCur=PatCur;
							FormMedRec.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SummaryOfCare:
						using(FormEhrSummaryOfCare FormSoC=new FormEhrSummaryOfCare()) {
							FormSoC.PatCur=PatCur;
							FormSoC.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SummaryOfCareElectronic:
						using(FormEhrSummaryOfCare FormSoCE=new FormEhrSummaryOfCare()) {
							FormSoCE.PatCur=PatCur;
							FormSoCE.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SecureMessaging:
						if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==2) {/*Modified Stage 2*/
							using FormWebMailMessageEdit FormWMME=new FormWebMailMessageEdit(PatCur.PatNum);
							FormWMME.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.FamilyHistory:
						using(FormMedical FormMed=new FormMedical(PatNotCur,PatCur,"tabFamHealthHist")) {
							FormMed.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.ElectronicNote:
						//Sign a Note
						break;
					case EhrMeasureType.CPOE_RadiologyOrdersOnly:
					case EhrMeasureType.CPOE_LabOrdersOnly:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						using(FormEhrLabOrders FormLab=new FormEhrLabOrders()) {
							FormLab.PatCur=PatCur;
							FormLab.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.LabImages:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						using(FormEhrLabOrders FormLO=new FormEhrLabOrders()) {
							FormLO.PatCur=PatCur;
							FormLO.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.DrugDrugInteractChecking:
						EhrMeasureEvent measureEventDDIC=new EhrMeasureEvent();
						measureEventDDIC.DateTEvent=DateTime.Now;
						measureEventDDIC.EventType=EhrMeasureEventType.DrugDrugInteractChecking;
						measureEventDDIC.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.DrugDrugInteractChecking);
						measureEventDDIC.IsNew=true;
						using(FormEhrMeasureEventEdit FormDDIC=new FormEhrMeasureEventEdit(measureEventDDIC)) {
							FormDDIC.MeasureDescript="Explain how you have enabled Drug-Drug Interaction Checking";
							FormDDIC.ShowDialog();
							if(FormDDIC.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.DrugFormularyChecking:
						EhrMeasureEvent measureEventDFC=new EhrMeasureEvent();
						measureEventDFC.DateTEvent=DateTime.Now;
						measureEventDFC.EventType=EhrMeasureEventType.DrugFormularyChecking;
						measureEventDFC.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.DrugFormularyChecking);
						measureEventDFC.IsNew=true;
						using(FormEhrMeasureEventEdit FormDFC=new FormEhrMeasureEventEdit(measureEventDFC)) {
							FormDFC.MeasureDescript="Explain how you have enabled Drug Formulary Checks";
							FormDFC.ShowDialog();
							if(FormDFC.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.ProtectElectHealthInfo:
						EhrMeasureEvent measureEventPEHI=new EhrMeasureEvent();
						measureEventPEHI.DateTEvent=DateTime.Now;
						measureEventPEHI.EventType=EhrMeasureEventType.ProtectElectHealthInfo;
						measureEventPEHI.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.ProtectElectHealthInfo);
						measureEventPEHI.IsNew=true;
						using(FormEhrMeasureEventEdit FormPEHI=new FormEhrMeasureEventEdit(measureEventPEHI)) {
							FormPEHI.MeasureDescript="Have you performed your security risk analysis?  Explain.";
							FormPEHI.ShowDialog();
							if(FormPEHI.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.ImmunizationRegistries:
						EhrMeasureEvent measureEventIR=new EhrMeasureEvent();
						measureEventIR.DateTEvent=DateTime.Now;
						measureEventIR.EventType=EhrMeasureEventType.ImmunizationRegistries;
						measureEventIR.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.ImmunizationRegistries);
						measureEventIR.IsNew=true;
						using(FormEhrMeasureEventEdit FormIR=new FormEhrMeasureEventEdit(measureEventIR)) {
							FormIR.MeasureDescript="Check with your state agency for guidance and recommendations.  Usually excluded.  Explain.";
							FormIR.ShowDialog();
							if(FormIR.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.SyndromicSurveillance:
						EhrMeasureEvent measureEventSS=new EhrMeasureEvent();
						measureEventSS.DateTEvent=DateTime.Now;
						measureEventSS.EventType=EhrMeasureEventType.SyndromicSurveillance;
						measureEventSS.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.SyndromicSurveillance);
						measureEventSS.IsNew=true;
						using(FormEhrMeasureEventEdit FormSS=new FormEhrMeasureEventEdit(measureEventSS)) {
							FormSS.MeasureDescript="Check with your state agency for guidance and recommendations.  Usually excluded.  Explain.";
							FormSS.ShowDialog();
							if(FormSS.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.PatientList:
						using(FormPatListEHR2014 FormPL=new FormPatListEHR2014()){
							FormPL.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.ClinicalInterventionRules:
						using(FormCdsTriggers FormET=new FormCdsTriggers()) {
							FormET.ShowDialog();
						}
						FillGridMu();
						break;
				}
			}
			if(e.Col==4) {
				switch(listMu[e.Row].MeasureType) {
					case EhrMeasureType.MedReconcile:
						int compare=EhrMeasures.CompareReferralsToReconciles(PatNum);
						if(compare==1 || compare==0) {
							using FormReferralSelect FormRS=new FormReferralSelect();
							FormRS.IsDoctorSelectionMode=true;
							FormRS.IsSelectionMode=true;
							FormRS.ShowDialog();
							if(FormRS.DialogResult==DialogResult.OK) {
								List<RefAttach> RefAttachList=RefAttaches.RefreshFiltered(PatNum,false,0);
								RefAttach refattach=new RefAttach();
								refattach.ReferralNum=FormRS.SelectedReferral.ReferralNum;
								refattach.PatNum=PatNum;
								refattach.RefType=ReferralType.RefFrom;
								refattach.RefDate=DateTime.Today;
								if(FormRS.SelectedReferral.IsDoctor) {//whether using ehr or not
									//we're not going to ask.  That's stupid.
									//if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is this an incoming transition of care from another provider?")){
									refattach.IsTransitionOfCare=true;
								}
								int order=0;
								for(int i=0;i<RefAttachList.Count;i++) {
									if(RefAttachList[i].ItemOrder > order) {
										order=RefAttachList[i].ItemOrder;
									}
								}
								refattach.ItemOrder=order+1;
								RefAttaches.Insert(refattach);
								SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refattach.ReferralNum));
								using FormMedicationReconcile FormMedRec=new FormMedicationReconcile();
								FormMedRec.PatCur=PatCur;
								FormMedRec.ShowDialog();
							}
						}
						else if(compare==-1) {
							using FormMedicationReconcile FormMedRec=new FormMedicationReconcile();
							FormMedRec.PatCur=PatCur;
							FormMedRec.ShowDialog();
						}
						FillGridMu();
						//ResultOnClosing=EhrFormResult.Referrals;
						//Close();
						break;
					case EhrMeasureType.SummaryOfCare:
					case EhrMeasureType.SummaryOfCareElectronic:
						using(FormReferralsPatient FormRefSum=new FormReferralsPatient()) {
							FormRefSum.PatNum=PatCur.PatNum;
							FormRefSum.ShowDialog();
						}
						FillGridMu();
						//ResultOnClosing=EhrFormResult.Referrals;
						//Close();
						break;
					case EhrMeasureType.Lab:
						//Redundant now that everything is done from one window
						break;
					case EhrMeasureType.CPOE_RadiologyOrdersOnly:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						//As of v15.4 we started storing radiology orders at the procedure level by flagging the procedure itself as IsCpoe.
						//Show the radiology order window which will be the best way for the provider to mark "radiology orders" as CPOE.
						using(FormRadOrderList FormROL=new FormRadOrderList(Security.CurUser)) {
							FormROL.ShowDialog();//Do not use a non-modal window in this case due to needing to refresh the grid after closing.
						}
						//using(FormEhrLabOrders FormRad=new FormEhrLabOrders()){
						//	FormRad.PatCur=PatCur;
						//	FormRad.ShowDialog();
						//}
						FillGridMu();
						break;
				}
			}
		}

		///<summary>This can happen when clicking in the grid, or when the form is Shown.  The latter would happen after user unknowingly exited ehr in order to use FormMedPat.  Popping back to the Orders window makes the experience seamless.  This can be recursive if the user edits a series of medicationpats.</summary>
		private void LaunchOrdersWindow() {
			using FormEhrMedicalOrders FormOrd = new FormEhrMedicalOrders();
			FormOrd._patCur=PatCur;
			FormOrd.ShowDialog();
			//if(FormOrd.DialogResult!=DialogResult.OK) {//There is no ok button
			//	return;
			//}
			/*not currently used, but might be if we let users generate Rx from med order.
			if(FormOrd.LaunchRx) {
				if(FormOrd.LaunchRxNum==0) {
					ResultOnClosing=EhrFormResult.RxSelect;
				}
				else {
					ResultOnClosing=EhrFormResult.RxEdit;
					LaunchRxNum=FormOrd.LaunchRxNum;
				}
				Close();
			}
			else*/
			if(FormOrd.LaunchMedicationPat) {
				//if(FormOrd.LaunchMedicationPatNum==0) {
				//	ResultOnClosing=EhrFormResult.MedicationPatNew;//This cannot happen unless a provider is logged in with a valid ehr key
				//}
				//else {
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=MedicationPats.GetOne(FormOrd.LaunchMedicationPatNum);
				FormMP.ShowDialog();
					//ResultOnClosing=EhrFormResult.MedicationPatEdit;
					//LaunchMedicationPatNum=FormOrd.LaunchMedicationPatNum;
				//}
				//Close();
			//}
			//else {
				FillGridMu();
			}
		}

		private void butMeasures_Click(object sender,EventArgs e) {
			#if EHRTEST
				ObjFormEhrMeasures=new EHR.FormEhrMeasures();
				((EHR.FormEhrMeasures)ObjFormEhrMeasures).ShowDialog();

			#else
				if(ObjFormEhrMeasures==null) {
					return;
				}
				Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
				//type.InvokeMember("ShowDialog",System.Reflection.BindingFlags.InvokeMethod,null,ObjFormEhrMeasures,null);
				using Form FormEM=(Form)type.InvokeMember("FormEhrMeasures",System.Reflection.BindingFlags.CreateInstance,null,ObjFormEhrMeasures,null);
				//AssemblyEHR.GetModule("FormEhrMeasures");
				FormEM.ShowDialog();
				long patNum=0;
				try {
					patNum=(long)type.InvokeMember("SelectedPatNum",System.Reflection.BindingFlags.GetProperty,null,FormEM,null);
				}
				catch { }
				if(FormEM.DialogResult==DialogResult.OK && patNum!=0) {
					PatNum=patNum;
					ResultOnClosing=EhrFormResult.PatientSelect;
					DialogResult=DialogResult.OK;
					Close();
					return;
				}
				//long patNum;
			#endif
			FillGridMu();
		}

		private void butHash_Click(object sender,EventArgs e) {
			using FormEhrHash FormH=new FormEhrHash();
			FormH.ShowDialog();
		}

		private void butEncryption_Click(object sender,EventArgs e) {
			using FormEhrEncryption FormE=new FormEhrEncryption();
			FormE.ShowDialog();
		}

		private void butQuality_Click(object sender,EventArgs e) {
			using FormEhrQualityMeasures FormQ=new FormEhrQualityMeasures();
			FormQ.ShowDialog();
			FillGridMu();
		}

		private void but2014CQM_Click(object sender,EventArgs e) {
			using FormEhrQualityMeasures2014 FormQ=new FormEhrQualityMeasures2014();
			FormQ.ShowDialog();
			if(FormQ.DialogResult==DialogResult.OK && FormQ.selectedPatNum!=0) {
				PatNum=FormQ.selectedPatNum;
				ResultOnClosing=EhrFormResult.PatientSelect;
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			FillGridMu();
		}

		private void butVaccines_Click(object sender,EventArgs e) {
			using FormEhrVaccines FormVac = new FormEhrVaccines();
			FormVac.PatCur=PatCur;
			FormVac.ShowDialog();
		}

		private void butPatList_Click(object sender,EventArgs e) {
			using FormPatListEHR2014 FormPL=new FormPatListEHR2014();
			FormPL.ShowDialog();
		}

		private void butPatList14_Click(object sender,EventArgs e) {
			MessageBox.Show("This form was moved to the OpenDental project and should be launched from Reports ?");
			//using OpenDental.FormPatList2014 FormPL14=new OpenDental.FormPatList2014();
			//FormPL14.ShowDialog();
		}

		private void butAmendments_Click(object sender,EventArgs e) {
			using FormEhrAmendments FormAmd=new FormEhrAmendments();
			FormAmd.PatCur=PatCur;
			FormAmd.ShowDialog();
		}

		private void butEhrNotPerformed_Click(object sender,EventArgs e) {
			using FormEhrNotPerformed FormNP=new FormEhrNotPerformed();
			FormNP.PatCur=PatCur;
			FormNP.ShowDialog();
		}

		private void butEncounters_Click(object sender,EventArgs e) {
			using FormEncounters FormEnc=new FormEncounters();
			FormEnc.PatCur=PatCur;
			FormEnc.ShowDialog();
		}

		private void butInterventions_Click(object sender,EventArgs e) {
			using FormInterventions FormInt=new FormInterventions();
			FormInt.PatCur=PatCur;
			FormInt.ShowDialog();
		}

		private void butCarePlans_Click(object sender,EventArgs e) {
			using FormEhrCarePlans FormCP=new FormEhrCarePlans(PatCur);
			FormCP.ShowDialog();
		}

		private void butClinicalSummary_Click(object sender,EventArgs e) {
			using FormEhrClinicalSummary FormECS=new FormEhrClinicalSummary();
			FormECS.PatCur=PatCur;
			FormECS.ShowDialog();
		}

		private void but2011Labs_Click(object sender,EventArgs e) {
			LaunchOrdersWindow();
		}

		private void butMeasureEvent_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EhrMeasureEventEdit)) {
				return;
			}
			using FormEhrMeasureEvents FormEME=new FormEhrMeasureEvents();
			FormEME.ShowDialog();
		}

		public static bool ProvKeyIsValid(string lName,string fName,int yearValue,string provKey) {
			try {
				#if EHRTEST //This pattern allows the code to compile without having the EHR code available.
				return FormEhrMeasures.ProvKeyIsValid(lName,fName,yearValue,provKey);
				#else
				constructObjFormEhrMeasuresHelper();
				Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
				object[] args=new object[] { lName,fName,yearValue,provKey };
				return (bool)type.InvokeMember("ProvKeyIsValid",System.Reflection.BindingFlags.InvokeMethod,null,ObjFormEhrMeasures,args);
				#endif
			}
			catch {
				return false;
			}
		}

		public static bool QuarterlyKeyIsValid(string year,string quarter,string practiceTitle,string qkey) {
			try{
				#if EHRTEST //This pattern allows the code to compile without having the EHR code available.
					return FormEhrMeasures.QuarterlyKeyIsValid(year,quarter,practiceTitle,qkey);
				#else
					constructObjFormEhrMeasuresHelper();
					Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
					object[] args=new object[] { year,quarter,practiceTitle,qkey };
					return (bool)type.InvokeMember("QuarterlyKeyIsValid",System.Reflection.BindingFlags.InvokeMethod,null,ObjFormEhrMeasures,args);
				#endif
			}
			catch{
				return false;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
