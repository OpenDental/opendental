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
		private Patient _patient;
		public PatientNote PatientNoteCur;
		public Family FamilyCur;
		/// <summary>If EhrFormResultOnClose=RxEdit, then this specifies which Rx to open.</summary>
		public long RxNum;
		///<summary>After this form closes, this can trigger other things to happen.  DialogResult is not checked.  After the other actions are taken, FormEHR opens back up again for seamless user experience.</summary>
		public EhrFormResult EhrFormResultOnClose;
		private List<EhrMu> _listEhrMus;
		/// <summary>If EhrFormResultOnClose=MedicationPatEdit, then this specifies which MedicationPat to open.</summary>
		public long MedicationPatNum;
		///<summary>If set to true, this will cause medical orders window to come up.</summary>
		public bool DoShowOrders;
		///<summary>This is set every time the form is shown.  It is used to determine if there is an Ehr key for the patient's primary provider.  If not, then The main grid will show empty.</summary>
		private Provider _provider;
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
			byte[] byteArrayResource=null;
			resourceReader.GetResourceData(strResourceName,out strResourceType,out byteArrayResource);
			resourceReader.Dispose();
			stream.Dispose();
			MemoryStream memoryStream=new MemoryStream(byteArrayResource);
			BinaryReader binaryReader=new BinaryReader(memoryStream);
			string retVal=binaryReader.ReadString();//Removes the leading binary characters from the string.
			memoryStream.Dispose();
			binaryReader.Dispose();
			return retVal;
		}

		private void FormEHR_Load(object sender,EventArgs e) {
			//Can't really use this, because it's only loaded once at the very beginning of OD starting up.
		}

		private void FormEHR_Shown(object sender,EventArgs e) {
			EhrFormResultOnClose=EhrFormResult.None;
			_patient=Patients.GetPat(PatNum);
			_provider=Providers.GetProv(_patient.PriProv);
			labelProvPat.Text=_provider.GetLongDesc();
			if(EhrProvKeys.GetKeysByFLName(_provider.LName,_provider.FName).Count==0) {
				labelProvPat.Text+=" (no ehr provider key entered)";
			}
			if(Security.CurUser.ProvNum==0) {
				labelProvUser.Text="none";
			}
			else {
				Provider provider=Providers.GetProv(Security.CurUser.ProvNum);
				labelProvUser.Text=Providers.GetLongDesc(provider.ProvNum);
				if(EhrProvKeys.GetKeysByFLName(provider.LName,provider.FName).Count==0) {
					labelProvUser.Text+=" (no ehr provider key entered)";
				}
			}
			FillGridMu();
			if(DoShowOrders) {
				//LaunchOrdersWindow();
				DoShowOrders=false;
			}
			//We already indicate that the patient's provider does not have an ehr key entered in labelProvPat.  No need for a popup.
			//This is so that non-ehr providers can still use many of our ehr features.  E.g. vital signs.
			//if(ProvPat.EhrKey=="") {
			//	MessageBox.Show("No ehr provider key entered for this patient's primary provider.");
			//}
		}

		private void FillGridMu() {
			gridMu.BeginUpdate();
			gridMu.Columns.Clear();
			GridColumn col=new GridColumn("MeasureType",145);
			gridMu.Columns.Add(col);
			col=new GridColumn("Met",35,HorizontalAlignment.Center);
			gridMu.Columns.Add(col);
			col=new GridColumn("Details",170);
			gridMu.Columns.Add(col);
			col=new GridColumn("Click to Take Action",168);
			gridMu.Columns.Add(col);
			col=new GridColumn("Related Actions",142);
			gridMu.Columns.Add(col);
			//Always fill the grid regardless if the patient's provider has a valid ehr key.
			//TODO: ProvPat may not be the primary provider in the future.
			if(_provider.EhrMuStage==0) {//Use the global preference
				if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==2) {/*Modified Stage 2*/
					gridMu.Title="Modified Stage 2 Meaningful Use for this patient";
					_listEhrMus=EhrMeasures.GetMu2Mod(_patient);
				}
				else if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==1) {/*Stage 2*/
					gridMu.Title="Stage 2 Meaningful Use for this patient";
					_listEhrMus=EhrMeasures.GetMu2(_patient);
				}
				else {/*Stage 1*/
					gridMu.Title="Stage 1 Meaningful Use for this patient";
					_listEhrMus=EhrMeasures.GetMu(_patient);
				}
			}
			else if(_provider.EhrMuStage==1) {
				gridMu.Title="Stage 1 Meaningful Use for this patient";
				_listEhrMus=EhrMeasures.GetMu(_patient);
			}
			else if(_provider.EhrMuStage==2) {
				gridMu.Title="Stage 2 Meaningful Use for this patient";
				_listEhrMus=EhrMeasures.GetMu2(_patient);
			}
			else if(_provider.EhrMuStage==3) {
				gridMu.Title="Modified Stage 2 Meaningful Use for this patient";
				_listEhrMus=EhrMeasures.GetMu2Mod(_patient);
			}
			gridMu.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEhrMus.Count;i++) {
				row=new GridRow();
				if(_provider.EhrMuStage==3) {//Stage 2 Modified
					row.Cells.Add(EhrMeasures.MeasureTypeTitleHelper(_listEhrMus[i].MeasureType));
				}
				else {
					row.Cells.Add(_listEhrMus[i].MeasureType.ToString());
				}
				if(_listEhrMus[i].Met==MuMet.True) {
					row.Cells.Add("X");
					row.ColorBackG=Color.FromArgb(178,255,178);
				}
				else if(_listEhrMus[i].Met==MuMet.NA) {
					row.Cells.Add("N/A");
					row.ColorBackG=Color.FromArgb(178,255,178);
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(_listEhrMus[i].Details);
				row.Cells.Add(_listEhrMus[i].Action);
				row.Cells.Add(_listEhrMus[i].Action2);
				gridMu.ListGridRows.Add(row);
			}
			gridMu.EndUpdate();
		}

		private void gridMu_CellClick(object sender,ODGridClickEventArgs e) {
			 
			if(e.Col==3) {
				switch(_listEhrMus[e.Row].MeasureType) {
					case EhrMeasureType.ProblemList:
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabProblems")) {
							formMedical.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.MedicationList:
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabMedications")) {
							formMedical.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.AllergyList:
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabAllergies")) {
							formMedical.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Demographics:
						if(!Security.IsAuthorized(Permissions.PatientEdit)) {
							return;
						}
						using(FormPatientEdit formPatientEdit=new FormPatientEdit(_patient,FamilyCur)) {
							formPatientEdit.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.Education:
						using(FormEhrEduResourcesPat formEhrEduResourcesPat = new FormEhrEduResourcesPat()) {
							formEhrEduResourcesPat.patCur=_patient;
							formEhrEduResourcesPat.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.TimelyAccess:
					case EhrMeasureType.ElectronicCopyAccess:
						using(FormPatientPortal formPatientPortal=new FormPatientPortal(_patient)) {
							formPatientPortal.ShowDialog();
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
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabVitalSigns")) {
							formMedical.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Smoking:
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabTobaccoUse")) {
							formMedical.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.Lab:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						using(FormEhrLabOrders formEhrLabOrders=new FormEhrLabOrders()) {
							formEhrLabOrders.PatCur=_patient;
							formEhrLabOrders.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.ElectronicCopy:
						if(_listEhrMus[e.Row].Action=="Provide elect copy to Pt") {
							using(FormEhrElectronicCopy formEhrElectronicCopy=new FormEhrElectronicCopy()) {
								formEhrElectronicCopy.PatCur=_patient;
								formEhrElectronicCopy.ShowDialog();
							}
							FillGridMu();
						}
						break;
					case EhrMeasureType.ClinicalSummaries:
						using(FormEhrClinicalSummary formEhrClinicalSummary=new FormEhrClinicalSummary()) {
							formEhrClinicalSummary.PatCur=_patient;
							formEhrClinicalSummary.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.Reminders:
						using(FormEhrReminders formEhrReminders=new FormEhrReminders()) {
							formEhrReminders.PatCur=_patient;
							formEhrReminders.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.MedReconcile:
						int compare=EhrMeasures.CompareReferralsToReconciles(PatNum);
						if(compare==1 || compare==0) {//Referral count is less than reconcile count or both are zero.
							using FormReferralSelect formReferralSelect=new FormReferralSelect();
							formReferralSelect.IsDoctorSelectionMode=true;
							formReferralSelect.IsSelectionMode=true;
							formReferralSelect.ShowDialog();
							if(formReferralSelect.DialogResult!=DialogResult.OK) {
								return;
							}
							List<RefAttach> listRefAttaches=RefAttaches.RefreshFiltered(PatNum,false,0);
							RefAttach refAttach=new RefAttach();
							refAttach.ReferralNum=formReferralSelect.SelectedReferral.ReferralNum;
							refAttach.PatNum=PatNum;
							refAttach.RefType=ReferralType.RefFrom;
							refAttach.RefDate=DateTime.Today;
							if(formReferralSelect.SelectedReferral.IsDoctor) {//whether using ehr or not
								refAttach.IsTransitionOfCare=true;
							}
							int order=0;
							for(int i=0;i<listRefAttaches.Count;i++) {
								if(listRefAttaches[i].ItemOrder > order) {
									order=listRefAttaches[i].ItemOrder;
								}
							}
							refAttach.ItemOrder=order+1;
							RefAttaches.Insert(refAttach);
							SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
						}
						else if(compare==-1) {//The referral count is greater than the reconcile count.
							//So we do not need to show the referral window, we just need to reconcile below.
						}
						using(FormEhrSummaryOfCare formEhrSummaryOfCare=new FormEhrSummaryOfCare()) {
							formEhrSummaryOfCare.PatCur=_patient;
							formEhrSummaryOfCare.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SummaryOfCare:
						using(FormEhrSummaryOfCare formEhrSummaryOfCare=new FormEhrSummaryOfCare()) {
							formEhrSummaryOfCare.PatCur=_patient;
							formEhrSummaryOfCare.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SummaryOfCareElectronic:
						using(FormEhrSummaryOfCare formEhrSummaryOfCare=new FormEhrSummaryOfCare()) {
							formEhrSummaryOfCare.PatCur=_patient;
							formEhrSummaryOfCare.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.SecureMessaging:
						if(PrefC.GetInt(PrefName.MeaningfulUseTwo)==2) {/*Modified Stage 2*/
							using FormWebMailMessageEdit formWebMailMessageEdit=new FormWebMailMessageEdit(_patient.PatNum);
							formWebMailMessageEdit.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.FamilyHistory:
						using(FormMedical formMedical=new FormMedical(PatientNoteCur,_patient,"tabFamHealthHist")) {
							formMedical.ShowDialog();
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
						using(FormEhrLabOrders formEhrLabOrders=new FormEhrLabOrders()) {
							formEhrLabOrders.PatCur=_patient;
							formEhrLabOrders.ShowDialog();
							FillGridMu();
						}
						break;
					case EhrMeasureType.LabImages:
						if(DataConnection.DBtype==DatabaseType.Oracle) {
							MsgBox.Show(this,"Labs not supported with Oracle");
							break;
						}
						using(FormEhrLabOrders formEhrLabOrders=new FormEhrLabOrders()) {
							formEhrLabOrders.PatCur=_patient;
							formEhrLabOrders.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.DrugDrugInteractChecking:
						EhrMeasureEvent ehrMeasureEventDDIC=new EhrMeasureEvent();
						ehrMeasureEventDDIC.DateTEvent=DateTime.Now;
						ehrMeasureEventDDIC.EventType=EhrMeasureEventType.DrugDrugInteractChecking;
						ehrMeasureEventDDIC.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.DrugDrugInteractChecking);
						ehrMeasureEventDDIC.IsNew=true;
						using(FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit(ehrMeasureEventDDIC)) {
							formEhrMeasureEventEdit.MeasureDescript="Explain how you have enabled Drug-Drug Interaction Checking";
							formEhrMeasureEventEdit.ShowDialog();
							if(formEhrMeasureEventEdit.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.DrugFormularyChecking:
						EhrMeasureEvent ehrMeasureEventDFC=new EhrMeasureEvent();
						ehrMeasureEventDFC.DateTEvent=DateTime.Now;
						ehrMeasureEventDFC.EventType=EhrMeasureEventType.DrugFormularyChecking;
						ehrMeasureEventDFC.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.DrugFormularyChecking);
						ehrMeasureEventDFC.IsNew=true;
						using(FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit(ehrMeasureEventDFC)) {
							formEhrMeasureEventEdit.MeasureDescript="Explain how you have enabled Drug Formulary Checks";
							formEhrMeasureEventEdit.ShowDialog();
							if(formEhrMeasureEventEdit.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.ProtectElectHealthInfo:
						EhrMeasureEvent ehrMeasureEventPEHI=new EhrMeasureEvent();
						ehrMeasureEventPEHI.DateTEvent=DateTime.Now;
						ehrMeasureEventPEHI.EventType=EhrMeasureEventType.ProtectElectHealthInfo;
						ehrMeasureEventPEHI.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.ProtectElectHealthInfo);
						ehrMeasureEventPEHI.IsNew=true;
						using(FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit(ehrMeasureEventPEHI)) {
							formEhrMeasureEventEdit.MeasureDescript="Have you performed your security risk analysis?  Explain.";
							formEhrMeasureEventEdit.ShowDialog();
							if(formEhrMeasureEventEdit.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.ImmunizationRegistries:
						EhrMeasureEvent ehrMeasureEventIR=new EhrMeasureEvent();
						ehrMeasureEventIR.DateTEvent=DateTime.Now;
						ehrMeasureEventIR.EventType=EhrMeasureEventType.ImmunizationRegistries;
						ehrMeasureEventIR.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.ImmunizationRegistries);
						ehrMeasureEventIR.IsNew=true;
						using(FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit(ehrMeasureEventIR)) {
							formEhrMeasureEventEdit.MeasureDescript="Check with your state agency for guidance and recommendations.  Usually excluded.  Explain.";
							formEhrMeasureEventEdit.ShowDialog();
							if(formEhrMeasureEventEdit.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.SyndromicSurveillance:
						EhrMeasureEvent ehrMeasureEventSS=new EhrMeasureEvent();
						ehrMeasureEventSS.DateTEvent=DateTime.Now;
						ehrMeasureEventSS.EventType=EhrMeasureEventType.SyndromicSurveillance;
						ehrMeasureEventSS.MoreInfo=EhrMeasureEvents.GetLatestInfoByType(EhrMeasureEventType.SyndromicSurveillance);
						ehrMeasureEventSS.IsNew=true;
						using(FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit(ehrMeasureEventSS)) {
							formEhrMeasureEventEdit.MeasureDescript="Check with your state agency for guidance and recommendations.  Usually excluded.  Explain.";
							formEhrMeasureEventEdit.ShowDialog();
							if(formEhrMeasureEventEdit.DialogResult==DialogResult.OK) {
								FillGridMu();
							}
						}
						break;
					case EhrMeasureType.PatientList:
						using(FormPatListEHR2014 formPatListEHR2014=new FormPatListEHR2014()){
							formPatListEHR2014.ShowDialog();
						}
						FillGridMu();
						break;
					case EhrMeasureType.ClinicalInterventionRules:
						using(FormCdsTriggers formCdsTriggers=new FormCdsTriggers()) {
							formCdsTriggers.ShowDialog();
						}
						FillGridMu();
						break;
				}
			}
			if(e.Col==4) {
				switch(_listEhrMus[e.Row].MeasureType) {
					case EhrMeasureType.MedReconcile:
						int compare=EhrMeasures.CompareReferralsToReconciles(PatNum);
						if(compare==1 || compare==0) {
							using FormReferralSelect formReferralSelect=new FormReferralSelect();
							formReferralSelect.IsDoctorSelectionMode=true;
							formReferralSelect.IsSelectionMode=true;
							formReferralSelect.ShowDialog();
							if(formReferralSelect.DialogResult==DialogResult.OK) {
								List<RefAttach> listRefAttaches=RefAttaches.RefreshFiltered(PatNum,false,0);
								RefAttach refAttach=new RefAttach();
								refAttach.ReferralNum=formReferralSelect.SelectedReferral.ReferralNum;
								refAttach.PatNum=PatNum;
								refAttach.RefType=ReferralType.RefFrom;
								refAttach.RefDate=DateTime.Today;
								if(formReferralSelect.SelectedReferral.IsDoctor) {//whether using ehr or not
									//we're not going to ask.  That's stupid.
									//if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is this an incoming transition of care from another provider?")){
									refAttach.IsTransitionOfCare=true;
								}
								int order=0;
								for(int i=0;i<listRefAttaches.Count;i++) {
									if(listRefAttaches[i].ItemOrder > order) {
										order=listRefAttaches[i].ItemOrder;
									}
								}
								refAttach.ItemOrder=order+1;
								RefAttaches.Insert(refAttach);
								SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
								using FormMedicationReconcile formMedicationReconcile=new FormMedicationReconcile();
								formMedicationReconcile.PatCur=_patient;
								formMedicationReconcile.ShowDialog();
							}
						}
						else if(compare==-1) {
							using FormMedicationReconcile formMedicationReconcile=new FormMedicationReconcile();
							formMedicationReconcile.PatCur=_patient;
							formMedicationReconcile.ShowDialog();
						}
						FillGridMu();
						//ResultOnClosing=EhrFormResult.Referrals;
						//Close();
						break;
					case EhrMeasureType.SummaryOfCare:
					case EhrMeasureType.SummaryOfCareElectronic:
						using(FormReferralsPatient formReferralsPatient=new FormReferralsPatient()) {
							formReferralsPatient.PatNum=_patient.PatNum;
							formReferralsPatient.ShowDialog();
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
						using(FormRadOrderList formRadOrderList=new FormRadOrderList(Security.CurUser)) {
							formRadOrderList.ShowDialog();//Do not use a non-modal window in this case due to needing to refresh the grid after closing.
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
			using FormEhrMedicalOrders formEhrMedicalOrders = new FormEhrMedicalOrders();
			formEhrMedicalOrders._patCur=_patient;
			formEhrMedicalOrders.ShowDialog();
			//if(formEhrMedicalOrders.DialogResult!=DialogResult.OK) {//There is no ok button
			//	return;
			//}
			/*not currently used, but might be if we let users generate Rx from med order.
			if(formEhrMedicalOrders.LaunchRx) {
				if(formEhrMedicalOrders.LaunchRxNum==0) {
					ResultOnClosing=EhrFormResult.RxSelect;
				}
				else {
					ResultOnClosing=EhrFormResult.RxEdit;
					LaunchRxNum=formEhrMedicalOrders.LaunchRxNum;
				}
				Close();
			}
			else*/
			if(formEhrMedicalOrders.LaunchMedicationPat) {
				//if(formEhrMedicalOrders.LaunchMedicationPatNum==0) {
				//	ResultOnClosing=EhrFormResult.MedicationPatNew;//This cannot happen unless a provider is logged in with a valid ehr key
				//}
				//else {
				using FormMedPat formMedPat=new FormMedPat();
				formMedPat.MedicationPatCur=MedicationPats.GetOne(formEhrMedicalOrders.LaunchMedicationPatNum);
				formMedPat.ShowDialog();
					//ResultOnClosing=EhrFormResult.MedicationPatEdit;
					//LaunchMedicationPatNum=formEhrMedicalOrders.LaunchMedicationPatNum;
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
				using Form form=(Form)type.InvokeMember("FormEhrMeasures",System.Reflection.BindingFlags.CreateInstance,null,ObjFormEhrMeasures,null);
				//AssemblyEHR.GetModule("FormEhrMeasures");
				form.ShowDialog();
				long patNum=0;
				try {
					patNum=(long)type.InvokeMember("SelectedPatNum",System.Reflection.BindingFlags.GetProperty,null,form,null);
				}
				catch { }
				if(form.DialogResult==DialogResult.OK && patNum!=0) {
					PatNum=patNum;
					EhrFormResultOnClose=EhrFormResult.PatientSelect;
					DialogResult=DialogResult.OK;
					Close();
					return;
				}
				//long patNum;
			#endif
			FillGridMu();
		}

		private void butHash_Click(object sender,EventArgs e) {
			using FormEhrHash formEhrHash=new FormEhrHash();
			formEhrHash.ShowDialog();
		}

		private void butEncryption_Click(object sender,EventArgs e) {
			using FormEhrEncryption formEhrEncryption=new FormEhrEncryption();
			formEhrEncryption.ShowDialog();
		}

		private void butQuality_Click(object sender,EventArgs e) {
			using FormEhrQualityMeasures formEhrQualityMeasures=new FormEhrQualityMeasures();
			formEhrQualityMeasures.ShowDialog();
			FillGridMu();
		}

		private void but2014CQM_Click(object sender,EventArgs e) {
			using FormEhrQualityMeasures2014 formEhrQualityMeasures2014=new FormEhrQualityMeasures2014();
			formEhrQualityMeasures2014.ShowDialog();
			if(formEhrQualityMeasures2014.DialogResult==DialogResult.OK && formEhrQualityMeasures2014.selectedPatNum!=0) {
				PatNum=formEhrQualityMeasures2014.selectedPatNum;
				EhrFormResultOnClose=EhrFormResult.PatientSelect;
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			FillGridMu();
		}

		private void butVaccines_Click(object sender,EventArgs e) {
			using FormEhrVaccines formEhrVaccines = new FormEhrVaccines();
			formEhrVaccines.PatCur=_patient;
			formEhrVaccines.ShowDialog();
		}

		private void butPatList_Click(object sender,EventArgs e) {
			using FormPatListEHR2014 formPatListEHR2014=new FormPatListEHR2014();
			formPatListEHR2014.ShowDialog();
		}

		private void butPatList14_Click(object sender,EventArgs e) {
			MessageBox.Show("This form was moved to the OpenDental project and should be launched from Reports ?");
			//using OpenDental.FormPatList2014 FormPL14=new OpenDental.FormPatList2014();
			//FormPL14.ShowDialog();
		}

		private void butAmendments_Click(object sender,EventArgs e) {
			using FormEhrAmendments formEhrAmendments=new FormEhrAmendments();
			formEhrAmendments.PatientCur=_patient;
			formEhrAmendments.ShowDialog();
		}

		private void butEhrNotPerformed_Click(object sender,EventArgs e) {
			using FormEhrNotPerformed formEhrNotPerformed=new FormEhrNotPerformed();
			formEhrNotPerformed.PatCur=_patient;
			formEhrNotPerformed.ShowDialog();
		}

		private void butEncounters_Click(object sender,EventArgs e) {
			using FormEncounters formEncounters=new FormEncounters();
			formEncounters.PatientCur=_patient;
			formEncounters.ShowDialog();
		}

		private void butInterventions_Click(object sender,EventArgs e) {
			using FormInterventions formInterventions=new FormInterventions();
			formInterventions.PatientCur=_patient;
			formInterventions.ShowDialog();
		}

		private void butCarePlans_Click(object sender,EventArgs e) {
			using FormEhrCarePlans formEhrCarePlans=new FormEhrCarePlans(_patient);
			formEhrCarePlans.ShowDialog();
		}

		private void butClinicalSummary_Click(object sender,EventArgs e) {
			using FormEhrClinicalSummary formEhrClinicalSummary=new FormEhrClinicalSummary();
			formEhrClinicalSummary.PatCur=_patient;
			formEhrClinicalSummary.ShowDialog();
		}

		private void but2011Labs_Click(object sender,EventArgs e) {
			LaunchOrdersWindow();
		}

		private void butMeasureEvent_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EhrMeasureEventEdit)) {
				return;
			}
			using FormEhrMeasureEvents formEhrMeasureEvents=new FormEhrMeasureEvents();
			formEhrMeasureEvents.ShowDialog();
		}

		public static bool ProvKeyIsValid(string lName,string fName,int year,string provKey) {
			try {
				#if EHRTEST //This pattern allows the code to compile without having the EHR code available.
				return FormEhrMeasures.ProvKeyIsValid(lName,fName,yearValue,provKey);
				#else
				constructObjFormEhrMeasuresHelper();
				Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
				object[] objectArray=new object[] { lName,fName,year,provKey };
				return (bool)type.InvokeMember("ProvKeyIsValid",System.Reflection.BindingFlags.InvokeMethod,null,ObjFormEhrMeasures,objectArray);
				#endif
			}
			catch {
				return false;
			}
		}

		public static bool QuarterlyKeyIsValid(string year,string quarter,string practiceTitle,string qKey) {
			try{
				#if EHRTEST //This pattern allows the code to compile without having the EHR code available.
					return FormEhrMeasures.QuarterlyKeyIsValid(year,quarter,practiceTitle,qkey);
				#else
					constructObjFormEhrMeasuresHelper();
					Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
					object[] objectArray=new object[] { year,quarter,practiceTitle,qKey };
					return (bool)type.InvokeMember("QuarterlyKeyIsValid",System.Reflection.BindingFlags.InvokeMethod,null,ObjFormEhrMeasures,objectArray);
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
