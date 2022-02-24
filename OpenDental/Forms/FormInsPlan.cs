using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using OpenDentBusiness.Crud;
using System.Linq;
using OpenDentBusiness.Eclaims;

namespace OpenDental{
///<summary></summary>
	public partial class FormInsPlan : FormODBase {
		///<summary>The InsPlan is always inserted before opening this form.</summary>
		public bool IsNewPlan;
		///<summary>The PatPlan is always inserted before opening this form.</summary>
		public bool IsNewPatPlan;
		/// <summary>used in the emp dropdown logic</summary>
		private string empOriginal;
		private bool mouseIsInListEmps;
		private List<Carrier> similarCars;
		private string carOriginal;
		private bool mouseIsInListCars;
		private Carrier _carrierCur;
		private PatPlan PatPlanCur;
		private PatPlan _patPlanOld;
		private ArrayList AdjAL;
		///<summary>This is the current benefit list that displays on the form.  It does not get saved to the database until this form closes.</summary>
		private List<Benefit> benefitList;//each item is a Benefit
		private List<Benefit> benefitListOld;
		//<summary>Set to true if called from the list of insurance plans.  In this case, the planNum will be 0.  There will be no subscriber.  Benefits will be 'typical' rather than from one specific plan.  Upon saving, all similar plans will be set to be exactly the same as PlanCur.</summary>
		//public bool IsForAll;//Instead, just pass in a null subscriber.
		///<summary>Set to true from FormInsPlansMerge.  In this case, the insplan is read only, because it's much more complicated to allow user to change.</summary>
		//public bool IsReadOnly;
		private List<FeeSched> FeeSchedsStandard;
		private List<FeeSched> FeeSchedsCopay;
		private List<FeeSched> _listFeeSchedsOutOfNetwork;
		private List<FeeSched> _listFeeSchedsManualBlueBook;
		private bool _hasDropped=false;
		private bool _hasOrdinalChanged=false;
		private bool _hasCarrierChanged=false;
		private bool _hasDeleted=false;
		private InsSub _subOld;
		private DateTime _dateInsPlanLastVerified;
		private DateTime _datePatPlanLastVerified;
		///<summary>The carrier num when the window was loaded.  Used to track if carrier has been changed.</summary>
		private long _carrierNumOrig;
		///<summary>The employer num when the window was loaded.  Used to track if the employer has been changed.</summary>
		private string _employerNameOrig;
		private string _employerNameCur;
		private string _electIdCur;
		private ProcedureCode _orthoAutoProc;
		private List<InsFilingCode> _listInsFilingCodes;
		private List<ClaimForm> _listClaimForms;
		//<summary>This is a field that is accessed only by clicking on the button because there's not room for it otherwise.  This variable should be treated just as if it was a visible textBox.</summary>
		//private string BenefitNotes;
		///<summary>Currently selected plan in the window.</summary>
		private InsPlan _planCur;
		///<summary>This is a copy of PlanCur as it was originally when this form was opened.  
		///This is needed to determine whether plan was changed.  However, this is also reset when 'pick from list' is used.</summary>
		private InsPlan _planCurOriginal;
		///<summary>Ins sub for the currently selected plan.</summary>
		private InsSub _subCur;
		private bool _didAddInsHistCP;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listEmps;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listCars;
		///<summary>The plan type that is selected in comboPlanType</summary>
		private InsPlanTypeComboItem _selectedPlanType;

		///<summary>The original plan that was passed into this form. Assigned in the constructor and can never be modified.  
		///This allows intelligent decisions about how to save changes.</summary>
		private InsPlan _planOld {
			get;
		}

		public long PlanCurNum {
			get {
				return _planCur.PlanNum;
			}
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsPlanCanada";
			}
			return "FormInsPlan";
		}

		///<summary>Called from ContrFamily and FormInsPlans. Must pass in the plan, patPlan, and sub, although patPlan and sub can be null.</summary>
		public FormInsPlan(InsPlan planCur,PatPlan patPlanCur,InsSub subCur){
			Cursor=Cursors.WaitCursor;
			InitializeComponent();
			InitializeLayoutManager();
			_planCur=planCur;
			_planOld=_planCur.Copy();
			PatPlanCur=patPlanCur;
			_patPlanOld=patPlanCur?.Copy();
			_subCur=subCur;
			listEmps=new ListBox();//Instead of ListBoxOD for consistency with listCars.
			listEmps.Location=new Point(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+textEmployer.Left,
				tabPageInsPlanInfo.Top+tabControlInsPlan.Top+panelPlan.Top+groupPlan.Top+textEmployer.Bottom);
			listEmps.Size=new Size(231,100);
			listEmps.Visible=false;
			listEmps.Click += new System.EventHandler(listEmps_Click);
			listEmps.DoubleClick += new System.EventHandler(listEmps_DoubleClick);
			listEmps.MouseEnter += new System.EventHandler(listEmps_MouseEnter);
			listEmps.MouseLeave += new System.EventHandler(listEmps_MouseLeave);
			LayoutManager.Add(listEmps,this);
			listEmps.BringToFront();
			listCars=new ListBox();//Instead of ListBoxOD, for horiz scroll on a dropdown.
			listCars.Location=new Point(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+groupCarrier.Left+textCarrier.Left,
				tabControlInsPlan.Top+tabPageInsPlanInfo.Top+panelPlan.Top+groupPlan.Top+groupCarrier.Top+textCarrier.Bottom);
			listCars.Size=new Size(700,100);
			listCars.HorizontalScrollbar=true;
			listCars.Visible=false;
			listCars.Click += new System.EventHandler(listCars_Click);
			listCars.DoubleClick += new System.EventHandler(listCars_DoubleClick);
			listCars.MouseEnter += new System.EventHandler(listCars_MouseEnter);
			listCars.MouseLeave += new System.EventHandler(listCars_MouseLeave);
			LayoutManager.Add(listCars,this);
			listCars.BringToFront();
			//tbPercentPlan.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbPercentPlan_CellClicked);
			//tbPercentPat.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbPercentPat_CellClicked);
			Lan.F(this);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelPatID.Text=Lan.g(this,"Dependant Code");
				labelCitySTZip.Text=Lan.g(this,"City,Prov,Post");   //Postal Code";
				butSearch.Visible=false;
				labelElectronicID.Text="EDI Code";
				comboElectIDdescript.Visible=false;
				labelGroupNum.Text=Lan.g(this,"Plan Number");
				checkIsPMP.Checked=(planCur.CanadianPlanFlag!=null && planCur.CanadianPlanFlag!="");
			}
			else{
				labelDivisionDash.Visible=false;
				textDivisionNo.Visible=false;
				//groupCanadian.Visible=false;
				tabControlInsPlan.TabPages.Remove(tabPageCanadian);
			}
			if(CultureInfo.CurrentCulture.Name.Length>=4 && CultureInfo.CurrentCulture.Name.Substring(3)=="GB"){//en-GB
				labelCitySTZip.Text=Lan.g(this,"City,Postcode");
			}
			panelPat.BackColor=Defs.GetFirstForCategory(DefCat.MiscColors).ItemColor;
			//labelViewRequestDocument.Text="         ";
			//if(!PrefC.GetBool(PrefName.CustomizedForPracticeWeb")) {
			//	butEligibility.Visible=false;
			//	labelViewRequestDocument.Visible=false;
			//}
			Cursor=Cursors.Default;
		}

		private void FormInsPlan_Load(object sender,System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			_planCurOriginal=_planCur.Copy();
			_listInsFilingCodes=InsFilingCodes.GetDeepCopy();
			if(_subCur!=null) {
				_subOld=_subCur.Copy();
			}
			long patPlanNum=0;
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				Label labelNoPermission=new Label();
				labelNoPermission.Text=Lan.g(this,"No Insurance Plan Edit permission.  Patient and Subscriber Information can still be saved.");
				labelNoPermission.Location=new Point(groupChanges.Location.X,groupChanges.Location.Y+10);
				labelNoPermission.Size=new Size(groupChanges.Size.Width+0,groupChanges.Size.Height);
				labelNoPermission.Visible=true;
				LayoutManager.Add(labelNoPermission,this);
				groupChanges.Visible=false;
				//It was decided by Nathan that restricting users from pressing the "Pick From List" button 
				//was an oversight and doesn't actually modify insurance information.
				//butPick.Enabled=false;
				butPickCarrier.Enabled=false;
				comboElectIDdescript.Enabled=false;
				checkIsMedical.Enabled=false;
				textEmployer.Enabled=false;
				textCarrier.Enabled=false;
				textPhone.Enabled=false;
				textAddress.Enabled=false;
				textAddress2.Enabled=false;
				textCity.Enabled=false;
				textState.Enabled=false;
				textZip.Enabled=false;
				textElectID.Enabled=false;
				butSearch.Enabled=false;
				comboSendElectronically.Enabled=false;
				textGroupName.Enabled=false;
				textGroupNum.Enabled=false;
				textLinkedNum.Enabled=false;
				textBIN.Enabled=false;
				textDivisionNo.Enabled=false;
				comboPlanType.Enabled=false;
				butSubstCodes.Enabled=false;
				checkAlternateCode.Enabled=false;
				checkCodeSubst.Enabled=false;
				checkClaimsUseUCR.Enabled=false;
				checkIsHidden.Enabled=false;
				comboFeeSched.Enabled=false;
				comboClaimForm.Enabled=false;
				comboCopay.Enabled=false;
				comboOutOfNetwork.Enabled=false;
				comboManualBlueBook.Enabled=false;
				comboCobRule.Enabled=false;
				comboFilingCode.Enabled=false;
				comboFilingCodeSubtype.Enabled=false;
				comboBillType.Enabled=false;
				comboExclusionFeeRule.Enabled=false;
				checkShowBaseUnits.Enabled=false;
				textDentaide.Enabled=false;
				textPlanFlag.Enabled=false;
				checkIsPMP.Enabled=false;
				textCanadianDiagCode.Enabled=false;
				textCanadianInstCode.Enabled=false;
				textPlanNote.Enabled=false;
				butGetElectronic.Enabled=false;
				butHistoryElect.Enabled=false;
				butImportTrojan.Enabled=false;
				butIapFind.Enabled=false;
				butBenefitNotes.Enabled=false;
				checkDontVerify.Enabled=false;
				textTrojanID.Enabled=false;
				//Allow users to verify that the current insurance plan information is correct.  Since this doesn't affect the insurance plan itself,
				//it is acceptable to allow them to acknowledge correct plans.
				//butVerifyBenefits.Enabled=false;
				//textDateLastVerifiedBenefits.Enabled=false;
				butDelete.Enabled=false;
			}
			if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook) {
				comboOutOfNetwork.Enabled=false;
				comboManualBlueBook.Enabled=true;
			}
			else {
				comboOutOfNetwork.Enabled=true;
				comboManualBlueBook.Enabled=false;
			}
			if(!Security.IsAuthorized(Permissions.InsuranceVerification,true)) {
				//Disable buttons that set UI to now.
				butVerifyPatPlan.Visible=false;//Using Visible instead of Enabled, Enabled makes the button background transparent and it looks strange.
				butVerifyBenefits.Visible=false;
				//Disable manual modification too.
				textDateLastVerifiedPatPlan.ReadOnly=true;
				textDateLastVerifiedBenefits.ReadOnly=true;
			}
			if(PatPlanCur!=null) {
				patPlanNum=PatPlanCur.PatPlanNum;
			}
			if(_subCur==null) {//editing from big list
				butPick.Visible=false;//This prevents an infinite loop
				//groupRequestBen.Visible=false;//might try to make this functional later, but not now.
				//groupRequestBen:---------------------------------------------
				butGetElectronic.Visible=false;
				butHistoryElect.Visible=false;
				labelHistElect.Visible=false;
				textElectBenLastDate.Visible=false;
				butImportTrojan.Visible=false;
				butIapFind.Visible=false;
				textTrojanID.Enabled=false;//view only
				butBenefitNotes.Visible=false;
				//end of groupRequestBen
				groupSubscriber.Visible=false;
				//radioChangeAll.Checked=true;//this logic needs to be repeated in OK.
				groupChanges.Visible=false;
				//benefitList=Benefits.RefreshForAll(PlanCur);
				//if(IsReadOnly) {
				//	butOK.Enabled=false;
				//}
				butDelete.Visible=false;
			}
			else {//editing from a patient
				if(PrefC.GetBool(PrefName.InsurancePlansShared)) {
					radioChangeAll.Checked=true;
				}
			}
			checkDontVerify.Checked=_planCur.HideFromVerifyList;
			InsVerify insVerifyBenefitsCur=InsVerifies.GetOneByFKey(_planCur.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefitsCur!=null && insVerifyBenefitsCur.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefitsCur.DateLastVerified.ToShortDateString();
			}
			if(IsNewPlan) {//Regardless of whether from big list or from individual patient.  Overrides above settings.
				//radioCreateNew.Checked=true;//this logic needs to be repeated in OK.
				//groupChanges.Visible=false;//because it wouldn't make sense to apply anything to "all"
				if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
					_planCur.PlanType="p";
				}
				_planCur.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
				textDateLastVerifiedBenefits.Text="";
			}
			benefitList=Benefits.RefreshForPlan(_planCur.PlanNum,patPlanNum);
			benefitListOld=new List<Benefit>();
			for(int i=0;i<benefitList.Count;i++){
				benefitListOld.Add(benefitList[i].Copy());
			}
			if(_planCur.PlanNum!=0) {
				textInsPlanNum.Text=_planCur.PlanNum.ToString();
			}
			if(PrefC.GetBool(PrefName.EasyHideCapitation)) {
				//groupCoPay.Visible=false;
				//comboCopay.Visible=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
				checkAlternateCode.Visible=false;
			}
			Program ProgramCur=Programs.GetCur(ProgramName.Trojan);
			if(ProgramCur!=null && ProgramCur.Enabled) {
				textTrojanID.Text=_planCur.TrojanID;
			}
			else {
				//labelTrojan.Visible=false;
				labelTrojanID.Visible=false;
				butImportTrojan.Visible=false;
				textTrojanID.Visible=false;
			}
			ProgramCur=Programs.GetCur(ProgramName.IAP);
			if(ProgramCur==null || !ProgramCur.Enabled) {
				//labelIAP.Visible=false;
				butIapFind.Visible=false;
			}
			if(!butIapFind.Visible && !butImportTrojan.Visible) {
				butBenefitNotes.Visible=false;
			}
			//FillPatData------------------------------
			if(PatPlanCur==null) {
				panelPat.Visible=false;
				//PatPlanCur is sometimes null
				butGetElectronic.Visible=false;
				butHistoryElect.Visible=false;
			}
			else {
				comboRelationship.Items.Clear();
				for(int i=0;i<Enum.GetNames(typeof(Relat)).Length;i++) {
					comboRelationship.Items.Add(Lan.g("enumRelat",Enum.GetNames(typeof(Relat))[i]));
					if((int)PatPlanCur.Relationship==i) {
						comboRelationship.SelectedIndex=i;
					}
				}
				if(PatPlanCur.PatPlanNum!=0) {
					textPatPlanNum.Text=PatPlanCur.PatPlanNum.ToString();
					if(IsNewPatPlan) {
						//Relationship is set to Self,  but the subscriber for the plan is not set to the current patient.
						if(comboRelationship.SelectedIndex==0 && _subCur.Subscriber!=PatPlanCur.PatNum) {
								comboRelationship.SelectedIndex=-1;
						}
					}
					else {
						InsVerify insVerifyPatPlanCur=InsVerifies.GetOneByFKey(PatPlanCur.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerifyPatPlanCur!=null && insVerifyPatPlanCur.DateLastVerified.Year>1880) {
							textDateLastVerifiedPatPlan.Text=insVerifyPatPlanCur.DateLastVerified.ToShortDateString();
						}
					}
				}
				textOrdinal.Text=PatPlanCur.Ordinal.ToString();
				checkIsPending.Checked=PatPlanCur.IsPending;
				textPatID.Text=PatPlanCur.PatID;
				FillPatientAdjustments();
			}
			if(_subCur!=null) {
				textSubscriber.Text=Patients.GetLim(_subCur.Subscriber).GetNameLF();
				textSubscriberID.Text=_subCur.SubscriberID;
				if(_subCur.DateEffective.Year < 1880) {
					textDateEffect.Text="";
				}
				else {
					textDateEffect.Text=_subCur.DateEffective.ToString("d");
				}
				if(_subCur.DateTerm.Year < 1880) {
					textDateTerm.Text="";
				}
				else {
					textDateTerm.Text=_subCur.DateTerm.ToString("d");
				}
				checkRelease.Checked=_subCur.ReleaseInfo;
				checkAssign.Checked=_subCur.AssignBen;
				textSubscNote.Text=_subCur.SubscNote;
			}
			FeeSchedsStandard=FeeScheds.GetListForType(FeeScheduleType.Normal,false);
			FeeSchedsCopay=FeeScheds.GetListForType(FeeScheduleType.CoPay,false)
				.Union(FeeScheds.GetListForType(FeeScheduleType.FixedBenefit,false))
				.ToList();
			_listFeeSchedsOutOfNetwork=FeeScheds.GetListForType(FeeScheduleType.OutNetwork,false);
			_listFeeSchedsManualBlueBook=FeeScheds.GetListForType(FeeScheduleType.ManualBlueBook,false);
			//Clearinghouse clearhouse=Clearinghouses.GetDefault();
			//if(clearhouse==null || clearhouse.CommBridge!=EclaimsCommBridge.ClaimConnect) {
			//	butEligibility.Visible=false;
			//}
			_employerNameOrig=Employers.GetName(_planCur.EmployerNum);
			_employerNameCur=Employers.GetName(_planCur.EmployerNum);
			_carrierNumOrig=_planCur.CarrierNum;
			_listClaimForms=ClaimForms.GetDeepCopy(false);
			comboSendElectronically.Items.AddEnums<NoSendElectType>();//selected index set in FillFormWithPlanCur -> FillCarrier
			FillFormWithPlanCur(false);
			FillBenefits();
			DateTime dateLast270=Etranss.GetLastDate270(_planCur.PlanNum);
			if(dateLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateLast270.ToShortDateString();
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkCodeSubst.Visible=false;
			}
			_datePatPlanLastVerified=PIn.Date(textDateLastVerifiedPatPlan.Text);
			_orthoAutoProc=_planCur.OrthoAutoProcCodeNumOverride==0 ? null : ProcedureCodes.GetProcCode(_planCur.OrthoAutoProcCodeNumOverride);
			FillOrtho();
			Cursor=Cursors.Default;
		}

		///<summary>Fills controls with ortho information.  Also hides the controls if needed.</summary>
		private void FillOrtho() {
			if(!PrefC.GetBool(PrefName.OrthoEnabled)) {
				butPatOrtho.Visible=false;
				tabControlInsPlan.TabPages.Remove(tabPageOrtho);
				return;
			}
			comboOrthoClaimType.Items.Clear();
			foreach(OrthoClaimType type in Enum.GetValues(typeof(OrthoClaimType))) {
				comboOrthoClaimType.Items.Add(Lan.g("enumOrthoClaimType",type.GetDescription()));
				if(_planCur.OrthoType==type) {
					comboOrthoClaimType.SelectedIndex = (int)type;
				}
			}
			comboOrthoAutoProcPeriod.Items.Clear();
			foreach(OrthoAutoProcFrequency type in Enum.GetValues(typeof(OrthoAutoProcFrequency))) {
				comboOrthoAutoProcPeriod.Items.Add(Lan.g("enumOrthoAutoProcFrequency",type.GetDescription()));
				if(_planCur.OrthoAutoProcFreq==type) {
					comboOrthoAutoProcPeriod.SelectedIndex = (int)type;
				}
			}
			textOrthoAutoFee.Text=_planCur.OrthoAutoFeeBilled.ToString();
			checkOrthoWaitDays.Checked=_planCur.OrthoAutoClaimDaysWait > 0;
			if(_orthoAutoProc!=null) {
				textOrthoAutoProc.Text=_orthoAutoProc.ProcCode;
			}
			else {
				textOrthoAutoProc.Text=ProcedureCodes.GetProcCode(PrefC.GetLong(PrefName.OrthoAutoProcCodeNum)).ProcCode +" ("+ Lan.g(this,"Default")+")";
			}
			SetEnabledOrtho();
		}

		private void SetEnabledOrtho() {
			if(!Security.IsAuthorized(Permissions.InsPlanOrthoEdit,true)) {
				//Disable every control within the Ortho tab.
				foreach(Control control in panelOrtho.Controls) {
					ODException.SwallowAnyException(() => { control.Enabled=false; });
				}
				return;
			}
			if(comboOrthoClaimType.SelectedIndex!=(int)OrthoClaimType.InitialPlusPeriodic) {
				comboOrthoAutoProcPeriod.Enabled=false;
				checkOrthoWaitDays.Checked=false;
				checkOrthoWaitDays.Enabled=false;
				labelAutoOrthoProcPeriod.Enabled=false;
				butPickOrthoProc.Enabled=false;
				labelOrthoAutoFee.Enabled=false;
				textOrthoAutoFee.Enabled=false;
				butDefaultAutoOrthoProc.Enabled=false;
			}
			else {
				comboOrthoAutoProcPeriod.Enabled=true;
				checkOrthoWaitDays.Enabled=true;
				labelAutoOrthoProcPeriod.Enabled=true;
				butPickOrthoProc.Enabled=true;
				labelOrthoAutoFee.Enabled=true;
				textOrthoAutoFee.Enabled=true;
				butDefaultAutoOrthoProc.Enabled=true;
			}
			if(comboOrthoClaimType.SelectedIndex==-1) {
				comboOrthoClaimType.SelectedIndex=0;
			}
			if(comboOrthoAutoProcPeriod.SelectedIndex==-1) {
				comboOrthoAutoProcPeriod.SelectedIndex=0;
			}	
		}

		///<summary>Uses PlanCur to fill out the information on the form.  Called once on startup and also if user picks a plan from template list.  This does not fill from SubCur, unlike FillPlanCurFromForm().</summary>
		private void FillFormWithPlanCur(bool isPicked) {
			Cursor=Cursors.WaitCursor;
			textEmployer.Text=Employers.GetName(_planCur.EmployerNum);
			_employerNameCur=textEmployer.Text;
			textGroupName.Text=_planCur.GroupName;
			textGroupNum.Text=_planCur.GroupNum;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				textBIN.Text=_planCur.RxBIN;
			}
			else{
				labelBIN.Visible=false;
				textBIN.Visible=false;
			}
			textDivisionNo.Text=_planCur.DivisionNo;//only visible in Canada
			textTrojanID.Text=_planCur.TrojanID;
			comboPlanType.Items.Clear();
			//Items must be added in the same order in which they are listed in InsPlanTypeComboItem.
			comboPlanType.Items.Add(Lan.g(this,"Category Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Fixed Benefit"));
			comboPlanType.Items.Add(Lan.g(this,"Medicaid or Flat Co-pay"));
			//Capitation must always be last, since it is sometimes hidden.
			if(!PrefC.GetBool(PrefName.EasyHideCapitation)) {
				comboPlanType.Items.Add(Lan.g(this,"Capitation"));
				if(_planCur.PlanType=="c") {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.Capitation;
				}
			}
			if(_planCur.PlanType=="") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.CategoryPercentage;
			}
			if(_planCur.PlanType=="p") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPO;
				FeeSched copayFeeSched=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==_planCur.CopayFeeSched 
					&& x.FeeSchedType==FeeScheduleType.FixedBenefit);
				if(copayFeeSched!=null) {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPOFixedBenefit;
				}
			}
			if(_planCur.PlanType=="f") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.MedicaidOrFlatCopay;
			}
			_selectedPlanType=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			checkAlternateCode.Checked=_planCur.UseAltCode;
			checkCodeSubst.Checked=_planCur.CodeSubstNone;
			checkPpoSubWo.Checked=_planCur.HasPpoSubstWriteoffs;
			checkIsMedical.Checked=_planCur.IsMedical;
			if(!PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				checkIsMedical.Visible=false;//This line prevents most users from modifying the Medical Insurance checkbox by accident, because most offices are dental only.
			}
			checkClaimsUseUCR.Checked=_planCur.ClaimsUseUCR;
			if(IsNewPlan && _planCur.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims) && !isPicked) {
				checkClaimsUseUCR.Checked=true;
			}
			if(IsNewPlan && !PrefC.GetBool(PrefName.InsDefaultAssignBen) && !isPicked) {
				checkAssign.Checked=false;
			}
			checkIsHidden.Checked=_planCur.IsHidden;
			checkShowBaseUnits.Checked=_planCur.ShowBaseUnits;
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.AddNone<FeeSched>();
			comboFeeSched.Items.AddList(FeeSchedsStandard,x=>x.Description);
			comboFeeSched.SetSelectedKey<FeeSched>(_planCur.FeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
			comboClaimForm.Items.Clear();
			foreach(ClaimForm claimForm in _listClaimForms) {
				//The default claim form will always show even if hidden.
				if(claimForm.IsHidden && claimForm.ClaimFormNum!=_planCur.ClaimFormNum && claimForm.ClaimFormNum!=PrefC.GetLong(PrefName.DefaultClaimForm)) {
					continue;
				}
				comboClaimForm.Items.Add(claimForm.Description+(claimForm.IsHidden?" (hidden)":""),claimForm);
				if(claimForm.ClaimFormNum==_planCur.ClaimFormNum) {
					comboClaimForm.SelectedIndex=comboClaimForm.Items.Count-1;
				}
			}
			if(comboClaimForm.SelectedIndex==-1) {//Select the default claim form if no selection (always for new plans)
				comboClaimForm.SetSelectedKey<ClaimForm>(PrefC.GetLong(PrefName.DefaultClaimForm), x => x.ClaimFormNum);
			}
			FillComboCoPay();
			comboOutOfNetwork.Items.Clear();
			comboOutOfNetwork.Items.AddNone<FeeSched>();
			comboOutOfNetwork.Items.AddList(_listFeeSchedsOutOfNetwork,x=>x.Description);
			comboOutOfNetwork.SetSelectedKey<FeeSched>(_planCur.AllowedFeeSched,x=>x.FeeSchedNum);
			comboManualBlueBook.Items.Clear();
			comboManualBlueBook.Items.AddNone<FeeSched>();
			comboManualBlueBook.Items.AddList(_listFeeSchedsManualBlueBook,x => x.Description);
			comboManualBlueBook.SetSelectedKey<FeeSched>(_planCur.ManualFeeSchedNum,x => x.FeeSchedNum);
			comboCobRule.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumCobRule)).Length;i++) {
				comboCobRule.Items.Add(Lan.g("enumEnumCobRule",Enum.GetNames(typeof(EnumCobRule))[i]));
			}
			comboCobRule.SelectedIndex=(int)_planCur.CobRule;			
			long selectedFilingCodeNum=_planCur.FilingCode;
			if(comboFilingCode.GetSelected<InsFilingCode>()!=null) {
				selectedFilingCodeNum=comboFilingCode.GetSelected<InsFilingCode>().InsFilingCodeNum;
			}
			comboFilingCode.Items.Clear();
			for(int i=0;i<_listInsFilingCodes.Count;i++) {
				comboFilingCode.Items.Add(_listInsFilingCodes[i].Descript, _listInsFilingCodes[i]);
				if(_listInsFilingCodes[i].InsFilingCodeNum==selectedFilingCodeNum) {
					comboFilingCode.SelectedIndex=i;
				}
			}
			FillComboFilingSubtype(selectedFilingCodeNum);
			comboBillType.Items.Clear();
			comboBillType.Items.AddDefNone();
			comboBillType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes,true));
			comboBillType.SetSelectedDefNum(_planCur.BillingType); 
			comboExclusionFeeRule.Items.Clear();
			Enum.GetValues(typeof(ExclusionRule)).Cast<ExclusionRule>().ForEach(x => comboExclusionFeeRule.Items.Add(x.GetDescription()));
			comboExclusionFeeRule.SelectedIndex=(int)_planCur.ExclusionFeeRule;
			FillCarrier(_planCur.CarrierNum);
			if(!Security.IsAuthorized(Permissions.CarrierCreate,true)) {
				textCarrier.Enabled=false;
				textPhone.Enabled=false;
				textAddress.Enabled=false;
				textAddress2.Enabled=false;
				textCity.Enabled=false;
				textState.Enabled=false;
				textZip.Enabled=false;
				textElectID.Enabled=false;
				butSearch.Enabled=false;
				comboSendElectronically.Enabled=false;
			}
			FillOtherSubscribers();
			textPlanNote.Text=_planCur.PlanNote;
			if(_planCur.DentaideCardSequence==0){
				textDentaide.Text="";
			}
			else{
				textDentaide.Text=_planCur.DentaideCardSequence.ToString();
			}
			textPlanFlag.Text=_planCur.CanadianPlanFlag;
			textCanadianDiagCode.Text=_planCur.CanadianDiagnosticCode;
			textCanadianInstCode.Text=_planCur.CanadianInstitutionCode;
			checkDontVerify.Checked=_planCur.HideFromVerifyList;
			InsVerify insVerifyBenefitsCur=InsVerifies.GetOneByFKey(_planCur.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefitsCur!=null && insVerifyBenefitsCur.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefitsCur.DateLastVerified.ToShortDateString();
				_dateInsPlanLastVerified=PIn.Date(textDateLastVerifiedBenefits.Text);
			}
			//if(PlanCur.BenefitNotes==""){
			//	butBenefitNotes.Enabled=false;
			//}
			Cursor=Cursors.Default;
		}

		private List<FeeSched> GetFilteredCopayFeeSched(List<FeeSched> listFeeSchedCopays) {
			if(_selectedPlanType==InsPlanTypeComboItem.PPOFixedBenefit) {
				labelCopayFeeSched.Text=Lan.g(this,"Fixed Benefit Amounts");
			}
			else {
				labelCopayFeeSched.Text=Lan.g(this,"Patient Co-pay Amounts");
			}
			List<FeeSched> listFeeSchedFiltered=new List<FeeSched>();
			foreach(FeeSched feeSchedCur in listFeeSchedCopays) {
				if(!IsFixedBenefitMismatch(feeSchedCur)) {
					listFeeSchedFiltered.Add(feeSchedCur.Copy());
				}
			}
			return listFeeSchedFiltered;
		}

		private void FillOtherSubscribers() {
			long excludeSub=-1;
			if(_subCur!=null){
				excludeSub=_subCur.InsSubNum;
			}
			//Even though this sub hasn't been updated to the database, this still works because SubCur.InsSubNum is valid and won't change.
			int countSubs=InsSubs.GetSubscriberCountForPlan(_planCur.PlanNum,excludeSub!=-1);
			textLinkedNum.Text=countSubs.ToString();
			if(countSubs>10000) {//10,000 per Nathan.
				comboLinked.Visible=false;
				butOtherSubscribers.Visible=true;
				butOtherSubscribers.Location=comboLinked.Location;
			}
			else {
				comboLinked.Visible=true;
				butOtherSubscribers.Visible=false;
				List<string> listSubs=InsSubs.GetSubscribersForPlan(_planCur.PlanNum,excludeSub);
				comboLinked.Items.Clear();
				comboLinked.Items.AddRange(listSubs.ToArray());
				if(listSubs.Count>0){
					comboLinked.SelectedIndex=0;
				}
			}
		}

		private void butOtherSubscribers_Click(object sender,EventArgs e) {
			using FormODBase form=new FormODBase() {
				Size=new Size(500,400),
				Text="Other Subscribers List",
				FormBorderStyle=FormBorderStyle.FixedSingle
			};
			GridOD grid=new GridOD() {
				Size=new Size(475,300),
				Location=new Point(5,5),
				Title="Subscribers",
				TranslationName=""
			};
			UI.Button butClose=new UI.Button() {
				Size=new Size(75,23),
				Text="Close",
				Location=new Point(form.ClientSize.Width-80,form.ClientSize.Height-28),//subtract the button's size plus 5 pixel buffer.
			};
			butClose.Click+=(s,ex) => form.Close();//When butClose is pressed, simply close the form.  If more functionality is needed, make a method below.
			form.Controls.Add(grid);
			form.Controls.Add(butClose);
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),20){ IsWidthDynamic=true });
			grid.ListGridRows.Clear();
			long excludeSub=-1;
			if(_subCur!=null){
				excludeSub=_subCur.InsSubNum;
			}
			List<string> listSubs=InsSubs.GetSubscribersForPlan(_planCur.PlanNum,excludeSub);
			foreach(string subName in listSubs) {
				grid.ListGridRows.Add(new GridRow(subName));
			}
			grid.EndUpdate();
			form.ShowDialog();
		}
		
		private void FillPatientAdjustments() {
			List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(PatPlanCur.PatNum);
			AdjAL=new ArrayList();//move selected claimprocs into ALAdj
			for(int i=0;i<ClaimProcList.Count;i++) {
				if(ClaimProcList[i].InsSubNum==_subCur.InsSubNum
					&& ClaimProcList[i].Status==ClaimProcStatus.Adjustment) {
					AdjAL.Add(ClaimProcList[i]);
				}
			}
			listAdj.Items.Clear();
			string s;
			for(int i=0;i<AdjAL.Count;i++) {
				s=((ClaimProc)AdjAL[i]).ProcDate.ToShortDateString()+"       Ins Used:  "
					+((ClaimProc)AdjAL[i]).InsPayAmt.ToString("F")+"       Ded Used:  "
					+((ClaimProc)AdjAL[i]).DedApplied.ToString("F");
				listAdj.Items.Add(s);
			}
		}

		///<summary>Fills the carrier fields on the form based on the specified carrierNum.</summary>
		private void FillCarrier(long carrierNum) {
			_carrierCur=Carriers.GetCarrier(carrierNum);
			textCarrier.Text=_carrierCur.CarrierName;
			textPhone.Text=_carrierCur.Phone;
			textAddress.Text=_carrierCur.Address;
			textAddress2.Text=_carrierCur.Address2;
			textCity.Text=_carrierCur.City;
			textState.Text=_carrierCur.State;
			textZip.Text=_carrierCur.Zip;
			textElectID.Text=_carrierCur.ElectID;
			_electIdCur=textElectID.Text;
			FillPayor();
			comboSendElectronically.SetSelectedEnum(_carrierCur.NoSendElect);
		}

		///<summary>Only called from FillCarrier and textElectID_Validating. Fills comboElectIDdescript as appropriate.</summary>
		private void FillPayor() {
			//textElectIDdescript.Text=ElectIDs.GetDescript(textElectID.Text);
			comboElectIDdescript.Items.Clear();
			string[] payorNames=ElectIDs.GetDescripts(textElectID.Text);
			if(payorNames.Length>1) {
				comboElectIDdescript.Items.Add("multiple payors use this ID");
			}
			for(int i=0;i<payorNames.Length;i++) {
				comboElectIDdescript.Items.Add(payorNames[i]);
			}
			if(payorNames.Length>0) {
				comboElectIDdescript.SelectedIndex=0;
			}
		}

		private void comboElectIDdescript_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(comboElectIDdescript.Items.Count>0) {
				comboElectIDdescript.SelectedIndex=0;//always show the first item in the list
			}
		}

		private void comboPlanType_SelectionChangeCommitted(object sender,System.EventArgs e) {
			//MessageBox.Show(InsPlans.Cur.PlanType+","+listPlanType.SelectedIndex.ToString());
			if((_planCur.PlanType=="" || _planCur.PlanType=="p")
				&& ListTools.In(comboPlanType.SelectedIndex,(int)InsPlanTypeComboItem.MedicaidOrFlatCopay,(int)InsPlanTypeComboItem.Capitation)) 
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear all percentages. Continue?")) {
					comboPlanType.SelectedIndex=(int)_selectedPlanType;//Undo the selection change.
					return;
				}
				//Loop through the list backwards so i will be valid.
				for(int i=benefitList.Count-1;i>=0;i--) {
					if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.CoInsurance) {
						benefitList.RemoveAt(i);
					}
				}
				//benefitList=new ArrayList();
				FillBenefits();
			}
			else if(comboPlanType.SelectedIndex==(int)InsPlanTypeComboItem.PPOFixedBenefit) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will set all percentages to 100%. Continue?")) {
					comboPlanType.SelectedIndex=(int)_selectedPlanType;//Undo the selection change.
					return;
				}
				foreach(Benefit benefit in benefitList) {
					if(benefit.BenefitType==InsBenefitType.CoInsurance) {
						benefit.Percent=100;
					}
				}
				FillBenefits();
			}
			InsPlanTypeComboItem prevSelection=_selectedPlanType;
			_selectedPlanType=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			switch(_selectedPlanType) {
				case InsPlanTypeComboItem.CategoryPercentage:
					_planCur.PlanType="";
					break;
				case InsPlanTypeComboItem.PPO:
				case InsPlanTypeComboItem.PPOFixedBenefit:
					_planCur.PlanType="p";
					break;
				case InsPlanTypeComboItem.MedicaidOrFlatCopay:
					_planCur.PlanType="f";
					break;
				case InsPlanTypeComboItem.Capitation:
					_planCur.PlanType="c";
					break;
				default:
					break;
			}
			if(PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {//otherwise, no automation on this field.
				if(_planCur.PlanType=="") {
					checkClaimsUseUCR.Checked=true;
				}
				else {
					checkClaimsUseUCR.Checked=false;
				}
			}
			if(prevSelection!=_selectedPlanType//Selection has actually changed
				&& (prevSelection==InsPlanTypeComboItem.PPOFixedBenefit || _selectedPlanType==InsPlanTypeComboItem.PPOFixedBenefit))//Is or was Fixed Benefit
			{
				//Fix the comboCopay list to match the new selection type if changing to or from PPO Fixed Benefits only.
				//Changing between non-fixed benefit types shouldn't refill the list because that will try to change the current selection
				FillComboCoPay();
			}
		}

		///<summary></summary>
		private bool IsFixedBenefitMismatch(FeeSched feeSched) {
			if(feeSched is null) {
				return false;
			}			
			bool isFixedBenefitSched=(feeSched.FeeSchedType==FeeScheduleType.FixedBenefit);
			bool isFixedBenefitPlanType=(_selectedPlanType==InsPlanTypeComboItem.PPOFixedBenefit);
			return(isFixedBenefitPlanType!=isFixedBenefitSched);
		}

		private void FillComboCoPay() {
			List<FeeSched> listFilteredCopayFeeSched=GetFilteredCopayFeeSched(FeeSchedsCopay);
			comboCopay.Items.Clear();
			comboCopay.Items.AddNone<FeeSched>();
			comboCopay.Items.AddList(listFilteredCopayFeeSched,x=>x.Description);
			comboCopay.SetSelectedKey<FeeSched>(_planCur.CopayFeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
		}

		private void butAdjAdd_Click(object sender,System.EventArgs e) {
			ClaimProc ClaimProcCur=ClaimProcs.CreateInsPlanAdjustment(PatPlanCur.PatNum,_planCur.PlanNum,_subCur.InsSubNum);
			using FormInsAdj FormIA=new FormInsAdj(ClaimProcCur);
			FormIA.IsNew=true;
			FormIA.ShowDialog();
			FillPatientAdjustments();
		}

		private void butHistory_Click(object sender,EventArgs e) {
			using FormInsHistSetup FormIHS=new FormInsHistSetup(PatPlanCur.PatNum,_subCur);
			if(FormIHS.ShowDialog()==DialogResult.Cancel) {
				return;
			}
			_didAddInsHistCP=true;
		}

		private void listAdj_DoubleClick(object sender,System.EventArgs e) {
			if(listAdj.SelectedIndex==-1) {
				return;
			}
			using FormInsAdj FormIA=new FormInsAdj((ClaimProc)AdjAL[listAdj.SelectedIndex]);
			FormIA.ShowDialog();
			FillPatientAdjustments();
		}

		///<summary>Button not visible if SubCur=null, editing from big list.</summary>
		private void butPick_Click(object sender,EventArgs e) {
			if(!IsNewPlan && !Security.IsAuthorized(Permissions.InsPlanPickListExisting,true)) {
				MsgBox.Show(this,"Permission required: 'Change existing Ins Plan using Pick From List'.\r\n"
					+"Alternatively, the Ins Plan can be dropped and a new plan may be added.");
				return;
			}
			using FormInsPlans FormIP=new FormInsPlans();
			FormIP.empText=textEmployer.Text;
			FormIP.carrierText=textCarrier.Text;
			FormIP.IsSelectMode=true;
			FormIP.ShowDialog();
			if(FormIP.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(!IsNewPlan && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to use the selected plan?  You should NOT use this if the patient is changing insurance.  Use the Drop button instead.")) {
				return;
			}
			if(FormIP.SelectedPlan.PlanNum==0) {//user clicked Blank
				_planCur=new InsPlan();
				_planCur.PlanNum=_planOld.PlanNum;
			}
			else {//user selected an existing plan
				_planCur=FormIP.SelectedPlan;
				textInsPlanNum.Text=FormIP.SelectedPlan.PlanNum.ToString();
			}
			FillFormWithPlanCur(true);
			//We need to pass patPlanNum in to RefreshForPlan to get patient level benefits:
			long patPlanNum=0;
			if(PatPlanCur!=null){
				patPlanNum=PatPlanCur.PatPlanNum;
			}
			if(FormIP.SelectedPlan.PlanNum==0){//user clicked blank
				benefitList=new List<Benefit>();
			}
			else {//user selected an existing plan
				benefitList=Benefits.RefreshForPlan(_planCur.PlanNum,patPlanNum);
			}
			FillBenefits();
			if(IsNewPlan || FormIP.SelectedPlan.PlanNum==0) {//New plan or user clicked blank.
				//Leave benefitListOld alone so that it will trigger deletion of the orphaned benefits later.
			}
			else{
				//Replace benefitListOld so that we only cause changes to be save that are made after this point.
				benefitListOld=new List<Benefit>();
				for(int i=0;i<benefitList.Count;i++) {
					benefitListOld.Add(benefitList[i].Copy());
				}
			}
			//benefitListOld=new List<Benefit>(benefitList);//this was not the proper way to make a shallow copy.
			_planCurOriginal=_planCur.Copy();
			FillOtherSubscribers();
			FillOrtho();
			//PlanNumOriginal is NOT reset here.
			//It's now similar to if we'd just opened a new form, except for SubCur still needs to be changed.
		}

		private void textEmployer_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return) {
				listEmps.Visible=false;
				textGroupName.Focus();
				return;
			}
			if(textEmployer.Text=="") {
				listEmps.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listEmps.Items.Count==0) {
					return;
				}
				if(listEmps.SelectedIndex==-1) {
					listEmps.SelectedIndex=0;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				else if(listEmps.SelectedIndex==listEmps.Items.Count-1) {
					listEmps.SelectedIndex=-1;
					textEmployer.Text=empOriginal;
				}
				else {
					listEmps.SelectedIndex++;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listEmps.Items.Count==0) {
					return;
				}
				if(listEmps.SelectedIndex==-1) {
					listEmps.SelectedIndex=listEmps.Items.Count-1;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				else if(listEmps.SelectedIndex==0) {
					listEmps.SelectedIndex=-1;
					textEmployer.Text=empOriginal;
				}
				else {
					listEmps.SelectedIndex--;
					textEmployer.Text=listEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(textEmployer.Text.Length==1) {
				textEmployer.Text=textEmployer.Text.ToUpper();
				textEmployer.SelectionStart=1;
			}
			empOriginal=textEmployer.Text;//the original text is preserved when using up and down arrows
			listEmps.Items.Clear();
			List<Employer> similarEmps=Employers.GetSimilarNames(textEmployer.Text);
			for(int i=0;i<similarEmps.Count;i++) {
				listEmps.Items.Add(similarEmps[i].EmpName);
			}
			int h=13*similarEmps.Count+5;
			if(h > ClientSize.Height-listEmps.Top){
				h=ClientSize.Height-listEmps.Top;
			}
			listEmps.Size=new Size(231,h);
			listEmps.Visible=true;
		}

		private void textEmployer_Leave(object sender,System.EventArgs e) {
			if(mouseIsInListEmps) {
				return;
			}
			listEmps.Visible=false;
		}

		private void listEmps_Click(object sender,System.EventArgs e) {
			if(listEmps.SelectedItem==null) {
				return;
			}
			textEmployer.Text=listEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listEmps.Visible=false;
		}

		private void listEmps_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
			textEmployer.Text=listEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listEmps.Visible=false;
		}

		private void listEmps_MouseEnter(object sender,System.EventArgs e) {
			mouseIsInListEmps=true;
		}

		private void listEmps_MouseLeave(object sender,System.EventArgs e) {
			mouseIsInListEmps=false;
		}

		private void butPickCarrier_Click(object sender,EventArgs e) {
			using FormCarriers formc=new FormCarriers();
			formc.IsSelectMode=true;
			formc.ShowDialog();
			if(formc.DialogResult!=DialogResult.OK) {
				return;
			}
			FillCarrier(formc.SelectedCarrier.CarrierNum);
		}

		private void textCarrier_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Return) {
				if(listCars.SelectedIndex==-1) {
					textPhone.Focus();
				}
				else {
					FillCarrier(similarCars[listCars.SelectedIndex].CarrierNum);
					textCarrier.Focus();
					textCarrier.SelectionStart=textCarrier.Text.Length;
				}
				listCars.Visible=false;
				return;
			}
			if(textCarrier.Text=="") {
				listCars.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listCars.Items.Count==0) {
					return;
				}
				if(listCars.SelectedIndex==-1) {
					listCars.SelectedIndex=0;
					textCarrier.Text=similarCars[listCars.SelectedIndex].CarrierName;
				}
				else if(listCars.SelectedIndex==listCars.Items.Count-1) {
					listCars.SelectedIndex=-1;
					textCarrier.Text=carOriginal;
				}
				else {
					listCars.SelectedIndex++;
					textCarrier.Text=similarCars[listCars.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listCars.Items.Count==0) {
					return;
				}
				if(listCars.SelectedIndex==-1) {
					listCars.SelectedIndex=listCars.Items.Count-1;
					textCarrier.Text=similarCars[listCars.SelectedIndex].CarrierName;
				}
				else if(listCars.SelectedIndex==0) {
					listCars.SelectedIndex=-1;
					textCarrier.Text=carOriginal;
				}
				else {
					listCars.SelectedIndex--;
					textCarrier.Text=similarCars[listCars.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(textCarrier.Text.Length==1) {
				textCarrier.Text=textCarrier.Text.ToUpper();
				textCarrier.SelectionStart=1;
			}
			carOriginal=textCarrier.Text;//the original text is preserved when using up and down arrows
			listCars.Items.Clear();
			similarCars=Carriers.GetSimilarNames(textCarrier.Text);
			for(int i=0;i<similarCars.Count;i++) {
				listCars.Items.Add(similarCars[i].CarrierName+", "
					+similarCars[i].Phone+", "
					+similarCars[i].Address+", "
					+similarCars[i].Address2+", "
					+similarCars[i].City+", "
					+similarCars[i].State+", "
					+similarCars[i].Zip);
			}
			int h=13*similarCars.Count+5;
			if(h > ClientSize.Height-listCars.Top){
				h=ClientSize.Height-listCars.Top;
			}
			listCars.Size=new Size(listCars.Width,h);
			listCars.Visible=true;
		}

		private void textCarrier_Leave(object sender,System.EventArgs e) {
			if(mouseIsInListCars) {
				return;
			}
			//or if user clicked on a different text box.
			if(listCars.SelectedIndex!=-1) {
				FillCarrier(similarCars[listCars.SelectedIndex].CarrierNum);
			}
			listCars.Visible=false;
		}

		private void listCars_Click(object sender,System.EventArgs e) {
			if(listCars.SelectedIndex==-1) {
				return;
			}
			FillCarrier(similarCars[listCars.SelectedIndex].CarrierNum);
			textCarrier.Focus();
			textCarrier.SelectionStart=textCarrier.Text.Length;
			listCars.Visible=false;
		}

		private void listCars_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
		}

		private void listCars_MouseEnter(object sender,System.EventArgs e) {
			mouseIsInListCars=true;
		}

		private void listCars_MouseLeave(object sender,System.EventArgs e) {
			mouseIsInListCars=false;
		}

		private void textAddress_TextChanged(object sender,System.EventArgs e) {
			if(textAddress.Text.Length==1) {
				textAddress.Text=textAddress.Text.ToUpper();
				textAddress.SelectionStart=1;
			}
		}

		private void textAddress2_TextChanged(object sender,System.EventArgs e) {
			if(textAddress2.Text.Length==1) {
				textAddress2.Text=textAddress2.Text.ToUpper();
				textAddress2.SelectionStart=1;
			}
		}

		private void textCity_TextChanged(object sender,System.EventArgs e) {
			if(textCity.Text.Length==1) {
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender,System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textState.Text.Length==1 || textState.Text.Length==2) {
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=2;
				}
			}
			else {
				if(textState.Text.Length==1) {
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=1;
				}
			}
		}

		private void textElectID_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
			if(textElectID.Text=="" || textElectID.Text==_electIdCur) {
				return;
			}
			if(CultureInfo.CurrentCulture.Name.Length>=4 && CultureInfo.CurrentCulture.Name.Substring(3)=="CA"){//en-CA or fr-CA
				if(!Regex.IsMatch(textElectID.Text,@"^[0-9]{6}$")) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Carrier ID should be six digits long.  Continue anyway?")){
						textElectID.Text=_electIdCur;//They clicked Cancel, set it back to what it was.
						e.Cancel=true;
						return;
					}
				}
			}
			//else{//anyplace including Canada
			string[] electIDs=ElectIDs.GetDescripts(textElectID.Text);
			if(electIDs.Length==0) {//if none found in the predefined list
				if(!Carriers.ElectIdInUse(textElectID.Text)){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Electronic ID not found. Continue anyway?")) {
						textElectID.Text=_electIdCur;//They clicked Cancel, set it back to what it was.
						e.Cancel=true;
						return;
					}
				}
			}
			_electIdCur=textElectID.Text;
			FillPayor();
			//}
		}
		
		private void butChange_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanChangeSubsc)) {
				return;
			}
			try {
				InsSubs.ValidateNoKeys(_subCur.InsSubNum,false);
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change subscriber?  This should not normally be needed.")) {
					return;
				}
			}
			catch(Exception ex){
				if(PrefC.GetBool(PrefName.SubscriberAllowChangeAlways)) {
					DialogResult dres=MessageBox.Show(Lan.g(this,"Warning!  Do not change unless fixing database corruption.  ")+"\r\n"+ex.Message);
					if(dres!=DialogResult.OK) {
						return;
					}
				}
				else {
					MessageBox.Show(Lan.g(this,"Not allowed to change.")+"\r\n"+ex.Message);
					return;
				}
			}
			Family fam=Patients.GetFamily(_subCur.Subscriber);
			using FormFamilyMemberSelect FormF=new FormFamilyMemberSelect(fam,true);
			FormF.ShowDialog();
			if(FormF.DialogResult!=DialogResult.OK) {
				return;
			}
			_subCur.Subscriber=FormF.SelectedPatNum;
			Patient subsc=Patients.GetLim(FormF.SelectedPatNum);
			textSubscriber.Text=subsc.GetNameLF();
			textSubscriberID.Text="";
		}

		private void CheckAssign_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign)) {
				checkAssign.Checked=!checkAssign.Checked;
			}
		}

		private void butSearch_Click(object sender,System.EventArgs e) {
			using FormElectIDs FormE=new FormElectIDs();
			FormE.IsSelectMode=true;
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK) {
				return;
			}
			textElectID.Text=FormE.selectedID.PayorID;
			_electIdCur=textElectID.Text;
			FillPayor();
			//textElectIDdescript.Text=FormE.selectedID.CarrierName;
		}

		private void FillComboFilingSubtype(long selectedFilingCode) {
			comboFilingCodeSubtype.Items.Clear();
			List<InsFilingCodeSubtype> subtypeListt=InsFilingCodeSubtypes.GetForInsFilingCode(selectedFilingCode);
			for(int i=0;i<subtypeListt.Count;i++) {
				comboFilingCodeSubtype.Items.Add(subtypeListt[i].Descript,subtypeListt[i]);
				if(_planCur.FilingCodeSubtype==subtypeListt[i].InsFilingCodeSubtypeNum) {
					comboFilingCodeSubtype.SelectedIndex=i;
				}
			}
		}
		
		private void comboFilingCode_SelectionChangeCommitted(object sender,EventArgs e) {
			InsFilingCode insFilingCode=comboFilingCode.GetSelected<InsFilingCode>();
			if(insFilingCode==null) {
				return;
			}
			FillComboFilingSubtype(insFilingCode.InsFilingCodeNum);
		}

		private void comboBillType_SelectionChangeCommitted(object sender,EventArgs e) {
			_planCur.BillingType=comboBillType.GetSelectedDefNum();
		}

		private void comboExclusionFeeRule_SelectionChangeCommitted(object sender,EventArgs e) {
			_planCur.ExclusionFeeRule=(ExclusionRule)comboExclusionFeeRule.SelectedIndex;
		}

		private void butImportTrojan_Click(object sender,System.EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return;//bridge is not yet available for web users.
			}
			//If SubCur is null, this button is not visible to click.
			if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Restorative)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Endodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Periodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Crowns)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.OralSurgery)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Orthodontics)==null) 
			{
				MsgBox.Show(this,"You must first set up your insurance categories with corresponding electronic benefit categories: Diagnostic,RoutinePreventive, Restorative, Endodontics, Periodontics, Crowns, OralSurgery, Orthodontics, and Prosthodontics");
				return;
			}
			string file="";
			if(ODBuild.IsDebug()) {
				file=@"C:\Trojan\ETW\Planout.txt";
			}
			else {
				RegistryKey regKey=Registry.LocalMachine.OpenSubKey("Software\\TROJAN BENEFIT SERVICE");
				if(regKey==null) {//dmg Unix OS will exit here.
					MessageBox.Show("Trojan not installed properly.");
					return;
				}
				//C:\ETW
				if(regKey.GetValue("INSTALLDIR")==null) {
					MessageBox.Show(@"Registry entry is missing and should be added manually.  LocalMachine\Software\TROJAN BENEFIT SERVICE. StringValue.  Name='INSTALLDIR',	value= path where the Trojan program is located.  Full path to directory, without trailing slash.");
					return;
				}
				file=ODFileUtils.CombinePaths(regKey.GetValue("INSTALLDIR").ToString(),"Planout.txt");
			}
			if(!File.Exists(file)) {
				MessageBox.Show(file+" not found.  You should export from Trojan first.");
				return;
			}
			TrojanObject troj=Trojan.ProcessTextToObject(File.ReadAllText(file));
			textTrojanID.Text=troj.TROJANID;
			textEmployer.Text=troj.ENAME;
			textGroupName.Text=troj.PLANDESC;
			textPhone.Text=troj.ELIGPHONE;
			textGroupNum.Text=troj.POLICYNO;
			//checkNoSendElect.Checked=!troj.ECLAIMS;//Ignore this.  Even if Trojan says paper, most offices still send by clearinghouse.
			textElectID.Text=troj.PAYERID;
			_electIdCur=textElectID.Text;
			textCarrier.Text=troj.MAILTO;
			textAddress.Text=troj.MAILTOST;
			textCity.Text=troj.MAILCITYONLY;
			textState.Text=troj.MAILSTATEONLY;
			textZip.Text=troj.MAILZIPONLY;
			_planCur.MonthRenew=(byte)troj.MonthRenewal;
			if(_subCur.BenefitNotes!="") {
				_subCur.BenefitNotes+="\r\n--------------------------------\r\n";
			}
			_subCur.BenefitNotes+=troj.BenefitNotes;
			if(troj.PlanNote!=""){
				if(textPlanNote.Text=="") {
					textPlanNote.Text=troj.PlanNote;
				}
				else {//must let user pick final note
					string[] noteArray=new string[2];
					noteArray[0]=textPlanNote.Text;
					noteArray[1]=troj.PlanNote;
					using FormNotePick FormN=new FormNotePick(noteArray);
					FormN.UseTrojanImportDescription=true;
					FormN.ShowDialog();
					if(FormN.DialogResult==DialogResult.OK) {
						textPlanNote.Text=FormN.SelectedNote;
					}
				}
			}
			//clear exising benefits from screen, not db:
			List<Benefit> listBensToKeep=new List<Benefit>();
			//Go through all current benefits, keep all limitation or age limit benefits.
			foreach(Benefit ben in benefitList) {
				if(Benefits.IsBitewingFrequency(ben) 
					|| Benefits.IsCancerScreeningFrequency(ben)
					|| Benefits.IsCrownFrequency(ben)
					|| Benefits.IsDenturesFrequency(ben)
					|| Benefits.IsExamFrequency(ben)
					|| Benefits.IsFlourideAgeLimit(ben)
					|| Benefits.IsFlourideFrequency(ben)
					|| Benefits.IsFullDebridementFrequency(ben)
					|| Benefits.IsImplantFrequency(ben)
					|| Benefits.IsPanoFrequency(ben)
					|| Benefits.IsPerioMaintFrequency(ben)
					|| Benefits.IsProphyFrequency(ben)
					|| Benefits.IsSealantAgeLimit(ben)
					|| Benefits.IsSealantFrequency(ben)
					|| Benefits.IsSRPFrequency(ben))
				{
					listBensToKeep.Add(ben);
				}
			}
			benefitList=new List<Benefit>();
			benefitList.AddRange(listBensToKeep);
			for(int i=0;i<troj.BenefitList.Count;i++){
				//if(fields[2]=="Anniversary year") {
				//	usesAnnivers=true;
				//	MessageBox.Show("Warning.  Plan uses Anniversary year rather than Calendar year.  Please verify the Plan Start Date.");
				//}
				troj.BenefitList[i].PlanNum=_planCur.PlanNum;
				benefitList.Add(troj.BenefitList[i].Copy());
			}
			if(!ODBuild.IsDebug()) {
				File.Delete(file);
			}
			butBenefitNotes.Enabled=true;
			FillBenefits();
			/*if(resetFeeSched){
				FeeSchedsStandard=FeeScheds.GetListForType(FeeScheduleType.Normal,false);
				FeeSchedsCopay=FeeScheds.GetListForType(FeeScheduleType.CoPay,false);
				FeeSchedsAllowed=FeeScheds.GetListForType(FeeScheduleType.Allowed,false);
				//if managed care, then do it a bit differently
				comboFeeSched.Items.Clear();
				comboFeeSched.Items.Add(Lan.g(this,"none"));
				comboFeeSched.SelectedIndex=0;
				for(int i=0;i<FeeSchedsStandard.Count;i++) {
					comboFeeSched.Items.Add(FeeSchedsStandard[i].Description);
					if(FeeSchedsStandard[i].FeeSchedNum==feeSchedNum)
						comboFeeSched.SelectedIndex=i+1;
				}
				comboCopay.Items.Clear();
				comboCopay.Items.Add(Lan.g(this,"none"));
				comboCopay.SelectedIndex=0;
				for(int i=0;i<FeeSchedsCopay.Count;i++) {
					comboCopay.Items.Add(FeeSchedsCopay[i].Description);
					//This will get set for managed care
					//if(FeeSchedsCopay[i].DefNum==PlanCur.CopayFeeSched)
					//	comboCopay.SelectedIndex=i+1;
				}
				comboAllowedFeeSched.Items.Clear();
				comboAllowedFeeSched.Items.Add(Lan.g(this,"none"));
				comboAllowedFeeSched.SelectedIndex=0;
				for(int i=0;i<FeeSchedsAllowed.Count;i++) {
					comboAllowedFeeSched.Items.Add(FeeSchedsAllowed[i].Description);
					//I would have set allowed for PPO, but we are probably going to deprecate this when we do coverage tables.
					//if(FeeSchedsAllowed[i].DefNum==PlanCur.AllowedFeeSched)
					//	comboAllowedFeeSched.SelectedIndex=i+1;
				}
			}*/
		}

		private void butIapFind_Click(object sender,System.EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Bridge is not available while viewing through the web.");
				return;
			}
			//If SubCur is null, this button is not visible to click.
			using FormIap FormI=new FormIap();
			FormI.ShowDialog();
			if(FormI.DialogResult==DialogResult.Cancel) {
				return;
			}
			Benefit ben;
			//clear exising benefits from screen, not db:
			benefitList=new List<Benefit>();
			string plan=FormI.selectedPlan;
			string field=null;
			string[] splitField;//if a field is a sentence with more than one word, we can split it for analysis
			int percent;
			try {
				Iap.ReadRecord(plan);
				for(int i=1;i<122;i++) {
					field=Iap.ReadField(i);
					if(field==null){
						field="";
					}
					switch(i) {
						default:
							//do nothing
							break;
						case Iap.Employer:
							if(_subCur.BenefitNotes!="") {
								_subCur.BenefitNotes+="\r\n";
							}
							_subCur.BenefitNotes+="Employer: "+field;
							textEmployer.Text=field;
							break;
						case Iap.Phone:
							_subCur.BenefitNotes+="\r\n"+"Phone: "+field;
							break;
						case Iap.InsUnder:
							_subCur.BenefitNotes+="\r\n"+"InsUnder: "+field;
							break;
						case Iap.Carrier:
							_subCur.BenefitNotes+="\r\n"+"Carrier: "+field;
							textCarrier.Text=field;
							break;
						case Iap.CarrierPh:
							_subCur.BenefitNotes+="\r\n"+"CarrierPh: "+field;
							textPhone.Text=field;
							break;
						case Iap.Group://seems to be used as groupnum
							_subCur.BenefitNotes+="\r\n"+"Group: "+field;
							textGroupNum.Text=field;
							break;
						case Iap.MailTo://the carrier name again
							_subCur.BenefitNotes+="\r\n"+"MailTo: "+field;
							break;
						case Iap.MailTo2://address
							_subCur.BenefitNotes+="\r\n"+"MailTo2: "+field;
							textAddress.Text=field;
							break;
						case Iap.MailTo3://address2
							_subCur.BenefitNotes+="\r\n"+"MailTo3: "+field;
							textAddress2.Text=field;
							break;
						case Iap.EClaims:
							_subCur.BenefitNotes+="\r\n"+"EClaims: "+field;//this contains the PayorID at the end, but also a bunch of other drivel.
							int payorIDloc=field.LastIndexOf("Payor ID#:");
							if(payorIDloc!=-1 && field.Length>payorIDloc+10) {
								textElectID.Text=field.Substring(payorIDloc+10);
								_electIdCur=textElectID.Text;
							}
							break;
						case Iap.FAXClaims:
							_subCur.BenefitNotes+="\r\n"+"FAXClaims: "+field;
							break;
						case Iap.DMOOption:
							_subCur.BenefitNotes+="\r\n"+"DMOOption: "+field;
							break;
						case Iap.Medical:
							_subCur.BenefitNotes+="\r\n"+"Medical: "+field;
							break;
						case Iap.GroupNum://not used.  They seem to use the group field instead
							_subCur.BenefitNotes+="\r\n"+"GroupNum: "+field;
							break;
						case Iap.Phone2://?
							_subCur.BenefitNotes+="\r\n"+"Phone2: "+field;
							break;
						case Iap.Deductible:
							_subCur.BenefitNotes+="\r\n"+"Deductible: "+field;
							if(field.StartsWith("$")) {
								splitField=field.Split(new char[] { ' ' });
								ben=new Benefit();
								ben.BenefitType=InsBenefitType.Deductible;
								ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								ben.PlanNum=_planCur.PlanNum;
								ben.TimePeriod=BenefitTimePeriod.CalendarYear;
								ben.MonetaryAmt=PIn.Double(splitField[0].Remove(0,1));//removes the $
								benefitList.Add(ben.Copy());
							}
							break;
						case Iap.FamilyDed:
							_subCur.BenefitNotes+="\r\n"+"FamilyDed: "+field;
							break;
						case Iap.Maximum:
							_subCur.BenefitNotes+="\r\n"+"Maximum: "+field;
							if(field.StartsWith("$")) {
								splitField=field.Split(new char[] { ' ' });
								ben=new Benefit();
								ben.BenefitType=InsBenefitType.Limitations;
								ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								ben.PlanNum=_planCur.PlanNum;
								ben.TimePeriod=BenefitTimePeriod.CalendarYear;
								ben.MonetaryAmt=PIn.Double(splitField[0].Remove(0,1));//removes the $
								benefitList.Add(ben.Copy());
							}
							break;
						case Iap.BenefitYear://text is too complex to parse
							_subCur.BenefitNotes+="\r\n"+"BenefitYear: "+field;
							break;
						case Iap.DependentAge://too complex to parse
							_subCur.BenefitNotes+="\r\n"+"DependentAge: "+field;
							break;
						case Iap.Preventive:
							_subCur.BenefitNotes+="\r\n"+"Preventive: "+field;
							splitField=field.Split(new char[] { ' ' });
							if(splitField.Length==0 || !splitField[0].EndsWith("%")) {
								break;
							}
							splitField[0]=splitField[0].Remove(splitField[0].Length-1,1);//remove %
							percent=PIn.Int(splitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							break;
						case Iap.Basic:
							_subCur.BenefitNotes+="\r\n"+"Basic: "+field;
							splitField=field.Split(new char[] { ' ' });
							if(splitField.Length==0 || !splitField[0].EndsWith("%")) {
								break;
							}
							splitField[0]=splitField[0].Remove(splitField[0].Length-1,1);//remove %
							percent=PIn.Int(splitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							break;
						case Iap.Major:
							_subCur.BenefitNotes+="\r\n"+"Major: "+field;
							splitField=field.Split(new char[] { ' ' });
							if(splitField.Length==0 || !splitField[0].EndsWith("%")) {
								break;
							}
							splitField[0]=splitField[0].Remove(splitField[0].Length-1,1);//remove %
							percent=PIn.Int(splitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;//includes crowns?
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							break;
						case Iap.InitialPlacement:
							_subCur.BenefitNotes+="\r\n"+"InitialPlacement: "+field;
							break;
						case Iap.ExtractionClause:
							_subCur.BenefitNotes+="\r\n"+"ExtractionClause: "+field;
							break;
						case Iap.Replacement:
							_subCur.BenefitNotes+="\r\n"+"Replacement: "+field;
							break;
						case Iap.Other:
							_subCur.BenefitNotes+="\r\n"+"Other: "+field;
							break;
						case Iap.Orthodontics:
							_subCur.BenefitNotes+="\r\n"+"Orthodontics: "+field;
							splitField=field.Split(new char[] { ' ' });
							if(splitField.Length==0 || !splitField[0].EndsWith("%")) {
								break;
							}
							splitField[0]=splitField[0].Remove(splitField[0].Length-1,1);//remove %
							percent=PIn.Int(splitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.CoInsurance;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
							ben.PlanNum=_planCur.PlanNum;
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.Percent=percent;
							benefitList.Add(ben.Copy());
							break;
						case Iap.Deductible2:
							_subCur.BenefitNotes+="\r\n"+"Deductible2: "+field;
							break;
						case Iap.Maximum2://ortho Max
							_subCur.BenefitNotes+="\r\n"+"Maximum2: "+field;
							if(field.StartsWith("$")) {
								splitField=field.Split(new char[] { ' ' });
								ben=new Benefit();
								ben.BenefitType=InsBenefitType.Limitations;
								ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
								ben.PlanNum=_planCur.PlanNum;
								ben.TimePeriod=BenefitTimePeriod.CalendarYear;
								ben.MonetaryAmt=PIn.Double(splitField[0].Remove(0,1));//removes the $
								benefitList.Add(ben.Copy());
							}
							break;
						case Iap.PymtSchedule:
							_subCur.BenefitNotes+="\r\n"+"PymtSchedule: "+field;
							break;
						case Iap.AgeLimit:
							_subCur.BenefitNotes+="\r\n"+"AgeLimit: "+field;
							break;
						case Iap.SignatureonFile:
							_subCur.BenefitNotes+="\r\n"+"SignatureonFile: "+field;
							break;
						case Iap.StandardADAForm:
							_subCur.BenefitNotes+="\r\n"+"StandardADAForm: "+field;
							break;
						case Iap.CoordinationRule:
							_subCur.BenefitNotes+="\r\n"+"CoordinationRule: "+field;
							break;
						case Iap.CoordinationCOB:
							_subCur.BenefitNotes+="\r\n"+"CoordinationCOB: "+field;
							break;
						case Iap.NightguardsforBruxism:
							_subCur.BenefitNotes+="\r\n"+"NightguardsforBruxism: "+field;
							break;
						case Iap.OcclusalAdjustments:
							_subCur.BenefitNotes+="\r\n"+"OcclusalAdjustments: "+field;
							break;
						case Iap.XXXXXX:
							_subCur.BenefitNotes+="\r\n"+"XXXXXX: "+field;
							break;
						case Iap.TMJNonSurgical:
							_subCur.BenefitNotes+="\r\n"+"TMJNonSurgical: "+field;
							break;
						case Iap.Implants:
							_subCur.BenefitNotes+="\r\n"+"Implants: "+field;
							break;
						case Iap.InfectionControl:
							_subCur.BenefitNotes+="\r\n"+"InfectionControl: "+field;
							break;
						case Iap.Cleanings:
							_subCur.BenefitNotes+="\r\n"+"Cleanings: "+field;
							break;
						case Iap.OralEvaluation:
							_subCur.BenefitNotes+="\r\n"+"OralEvaluation: "+field;
							break;
						case Iap.Fluoride1200s:
							_subCur.BenefitNotes+="\r\n"+"Fluoride1200s: "+field;
							break;
						case Iap.Code0220:
							_subCur.BenefitNotes+="\r\n"+"Code0220: "+field;
							break;
						case Iap.Code0272_0274:
							_subCur.BenefitNotes+="\r\n"+"Code0272_0274: "+field;
							break;
						case Iap.Code0210:
							_subCur.BenefitNotes+="\r\n"+"Code0210: "+field;
							break;
						case Iap.Code0330:
							_subCur.BenefitNotes+="\r\n"+"Code0330: "+field;
							break;
						case Iap.SpaceMaintainers:
							_subCur.BenefitNotes+="\r\n"+"SpaceMaintainers: "+field;
							break;
						case Iap.EmergencyExams:
							_subCur.BenefitNotes+="\r\n"+"EmergencyExams: "+field;
							break;
						case Iap.EmergencyTreatment:
							_subCur.BenefitNotes+="\r\n"+"EmergencyTreatment: "+field;
							break;
						case Iap.Sealants1351:
							_subCur.BenefitNotes+="\r\n"+"Sealants1351: "+field;
							break;
						case Iap.Fillings2100:
							_subCur.BenefitNotes+="\r\n"+"Fillings2100: "+field;
							break;
						case Iap.Extractions:
							_subCur.BenefitNotes+="\r\n"+"Extractions: "+field;
							break;
						case Iap.RootCanals:
							_subCur.BenefitNotes+="\r\n"+"RootCanals: "+field;
							break;
						case Iap.MolarRootCanal:
							_subCur.BenefitNotes+="\r\n"+"MolarRootCanal: "+field;
							break;
						case Iap.OralSurgery:
							_subCur.BenefitNotes+="\r\n"+"OralSurgery: "+field;
							break;
						case Iap.ImpactionSoftTissue:
							_subCur.BenefitNotes+="\r\n"+"ImpactionSoftTissue: "+field;
							break;
						case Iap.ImpactionPartialBony:
							_subCur.BenefitNotes+="\r\n"+"ImpactionPartialBony: "+field;
							break;
						case Iap.ImpactionCompleteBony:
							_subCur.BenefitNotes+="\r\n"+"ImpactionCompleteBony: "+field;
							break;
						case Iap.SurgicalProceduresGeneral:
							_subCur.BenefitNotes+="\r\n"+"SurgicalProceduresGeneral: "+field;
							break;
						case Iap.PerioSurgicalPerioOsseous:
							_subCur.BenefitNotes+="\r\n"+"PerioSurgicalPerioOsseous: "+field;
							break;
						case Iap.SurgicalPerioOther:
							_subCur.BenefitNotes+="\r\n"+"SurgicalPerioOther: "+field;
							break;
						case Iap.RootPlaning:
							_subCur.BenefitNotes+="\r\n"+"RootPlaning: "+field;
							break;
						case Iap.Scaling4345:
							_subCur.BenefitNotes+="\r\n"+"Scaling4345: "+field;
							break;
						case Iap.PerioPx:
							_subCur.BenefitNotes+="\r\n"+"PerioPx: "+field;
							break;
						case Iap.PerioComment:
							_subCur.BenefitNotes+="\r\n"+"PerioComment: "+field;
							break;
						case Iap.IVSedation:
							_subCur.BenefitNotes+="\r\n"+"IVSedation: "+field;
							break;
						case Iap.General9220:
							_subCur.BenefitNotes+="\r\n"+"General9220: "+field;
							break;
						case Iap.Relines5700s:
							_subCur.BenefitNotes+="\r\n"+"Relines5700s: "+field;
							break;
						case Iap.StainlessSteelCrowns:
							_subCur.BenefitNotes+="\r\n"+"StainlessSteelCrowns: "+field;
							break;
						case Iap.Crowns2700s:
							_subCur.BenefitNotes+="\r\n"+"Crowns2700s: "+field;
							break;
						case Iap.Bridges6200:
							_subCur.BenefitNotes+="\r\n"+"Bridges6200: "+field;
							break;
						case Iap.Partials5200s:
							_subCur.BenefitNotes+="\r\n"+"Partials5200s: "+field;
							break;
						case Iap.Dentures5100s:
							_subCur.BenefitNotes+="\r\n"+"Dentures5100s: "+field;
							break;
						case Iap.EmpNumberXXX:
							_subCur.BenefitNotes+="\r\n"+"EmpNumberXXX: "+field;
							break;
						case Iap.DateXXX:
							_subCur.BenefitNotes+="\r\n"+"DateXXX: "+field;
							break;
						case Iap.Line4://city state
							_subCur.BenefitNotes+="\r\n"+"Line4: "+field;
							field=field.Replace("  "," ");//get rid of double space before zip
							splitField=field.Split(new char[] { ' ' });
							if(splitField.Length<3) {
								break;
							}
							textCity.Text=splitField[0].Replace(",","");//gets rid of the comma on the end of city
							textState.Text=splitField[1];
							textZip.Text=splitField[2];
							break;
						case Iap.Note:
							_subCur.BenefitNotes+="\r\n"+"Note: "+field;
							break;
						case Iap.Plan://?
							_subCur.BenefitNotes+="\r\n"+"Plan: "+field;
							break;
						case Iap.BuildUps:
							_subCur.BenefitNotes+="\r\n"+"BuildUps: "+field;
							break;
						case Iap.PosteriorComposites:
							_subCur.BenefitNotes+="\r\n"+"PosteriorComposites: "+field;
							break;
					}
				}
				Iap.CloseDatabase();
				butBenefitNotes.Enabled=true;
			}
			catch(ApplicationException ex) {
				Iap.CloseDatabase();
				MessageBox.Show(ex.Message);
			}
			catch(Exception ex) {
				Iap.CloseDatabase();
				MessageBox.Show("Error: "+ex.Message);
			}
			FillBenefits();
		}

		private void EligibilityCheckCanada() {
			if(!FillPlanCurFromForm()) {
				return;
			}
			Carrier carrier=Carriers.GetCarrier(_planCur.CarrierNum);
			if(!carrier.IsCDA){
				MsgBox.Show(this,"Eligibility only supported for CDAnet carriers.");
				return;
			}
			if((carrier.CanadianSupportedTypes & CanSupTransTypes.EligibilityTransaction_08) != CanSupTransTypes.EligibilityTransaction_08) {
				MsgBox.Show(this,"Eligibility not supported by this carrier.");
				return;
			}
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			Cursor=Cursors.WaitCursor;
			//string result="";
			DateTime date=DateTime.Today;
			if(ODBuild.IsDebug()) {
				date=new DateTime(1999,1,4);//TODO: Remove after Canadian claim certification is complete.
			}
			Relat relat=(Relat)comboRelationship.SelectedIndex;
			string patID=textPatID.Text;
			try {
				CanadianOutput.SendElegibility(clearinghouseClin,PatPlanCur.PatNum,_planCur,date,relat,patID,true,_subCur,false,FormCCDPrint.PrintCCD);
				//textSubscriberID.Text,textPatID.Text,(Relat)comboRelationship.SelectedIndex,PlanCur.Subscriber,textDentaide.Text);
				//printout will happen in the line above.
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			//PlanCur.BenefitNotes+=result;
			//butBenefitNotes.Enabled=true;
			Cursor=Cursors.Default;
			DateTime dateLast270=Etranss.GetLastDate270(_planCur.PlanNum);
			if(dateLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateLast270.ToShortDateString();
			}
		}

		///<summary>This button is only visible if Trojan or IAP is enabled.  Always active.  Button not visible if SubCur==null.</summary>
		private void butBenefitNotes_Click(object sender,System.EventArgs e) {
			string otherBenNote="";
			if(_subCur.BenefitNotes=="") {
				//try to find some other similar notes. Never includes the current subscriber.
				//List<long> samePlans=InsPlans.GetPlanNumsOfSamePlans(textEmployer.Text,textGroupName.Text,textGroupNum.Text,
				//	textDivisionNo.Text,textCarrier.Text,checkIsMedical.Checked,PlanCur.PlanNum,false);
				otherBenNote=InsSubs.GetBenefitNotes(_planCur.PlanNum,_subCur.InsSubNum);
				if(otherBenNote=="") {
					MsgBox.Show(this,"No benefit note found.  Benefit notes are created when importing Trojan or IAP benefit information and are frequently read-only.  Store your own notes in the subscriber note instead.");
					return;
				}
				MsgBox.Show(this,"This plan does not have a benefit note, but a note was found for another subsriber of this plan.  You will be able to view this note, but not change it.");
			}
			using FormInsBenefitNotes FormI=new FormInsBenefitNotes();
			if(_subCur.BenefitNotes!="") {
				FormI.BenefitNotes=_subCur.BenefitNotes;
			}
			else {
				FormI.BenefitNotes=otherBenNote;
			}
			FormI.ShowDialog();
			if(FormI.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(_subCur.BenefitNotes!="") {
				_subCur.BenefitNotes=FormI.BenefitNotes;
			}
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			string logText="";
			//this is a dual purpose button.  It sometimes deletes subscribers (inssubs), and sometimes the plan itself. 
			if(IsNewPlan) {
				DialogResult=DialogResult.Cancel;//original plan will get deleted in closing event.
				return;
			}
			string warningMsg="This plan doesn't have a carrier attached and probably is being created by another user right now.  Click OK to delete plan anyway.";
			if(_carrierCur.CarrierNum==0 && !MsgBox.Show(this,MsgBoxButtons.YesNo,warningMsg)) {
				return;
			}
			//1. Delete Subscriber---------------------------------------------------------------------------------------------------
			//Can only do this if there are other subscribers present.  If this is the last subscriber, then it attempts to delete the plan itself, down below.
			if(textLinkedNum.Text!="0") {//Other subscribers are present.  
				if(_subCur==null) {//viewing from big list
					MsgBox.Show(this,"Subscribers must be removed individually before deleting plan.");//by dropping, then using this same delete button.
					return;
				}
				else {//Came into here through a patient.
					DateTime dateSubChange=_subCur.SecDateTEdit;
					if(PatPlanCur!=null) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All patients attached to this subscription will be dropped and the subscription for this plan will be deleted. Continue?")) {
							return;
						}
					}
					//drop the plan


					//detach subscriber.
					try {
						InsSubs.Delete(_subCur.InsSubNum);//Checks dependencies first;  If none, deletes the inssub, claimprocs, patplans, and recomputes all estimates.
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					logText=Lan.g(this,"The subscriber")+" "+Patients.GetPat(_subCur.Subscriber).GetNameFLnoPref()+" "
						+Lan.g(this,"with the Subscriber ID")+" "+_subCur.SubscriberID+" "+Lan.g(this,"was deleted.");
					_hasDeleted=true;
					//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null)?0:PatPlanCur.PatNum,logText,(_planCur==null)?0:_planCur.PlanNum,
						_planCur.SecDateTEdit);
					DialogResult=DialogResult.OK;
					return;
				}
			}
			//or
			//2. Delete the plan itself-------------------------------------------------------------------------------------------------
			//This is the only subscriber, so delete inssub and insplan
			//Or this is the big list and there are no subscribers, so just delete the insplan.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Plan?")) {
				return;
			}
			DateTime datePrevious=_planCur.SecDateTEdit;
			try {
				InsPlans.Delete(_planCur);//Checks dependencies first;  If none, deletes insplan, inssub, benefits, claimprocs, patplans, and recomputes all estimates.
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_planCur.CarrierNum).CarrierName+" "+Lan.g(this,"was deleted.");
			_hasDeleted=true;
			//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null)?0:PatPlanCur.PatNum,logText,(_planCur==null)?0:_planCur.PlanNum,
				datePrevious);
			DialogResult=DialogResult.OK;
		}

		private void butDrop_Click(object sender,System.EventArgs e) {
			DropClickHelper();
		}

		///<summary>Returns true when successfully dropped.</summary>
		private bool DropClickHelper() {
			//Treat the Drop button just like the Delete and Cancel buttons if this is a new plan.
			if(IsNewPlan) {
				DialogResult=DialogResult.Cancel;//original plan will get deleted in closing event.
				return false;
			}
			string warningMsg="This plan doesn't have a carrier attached and probably is being created by another user right now.  Click OK to drop plan anyway.";
			if(_carrierCur.CarrierNum==0 && !MsgBox.Show(this,MsgBoxButtons.YesNo,warningMsg)) {
				return false;
			}
			//should we save the plan info first?  Probably not.
			//--
			//If they have a claim for this ins with today's date, don't let them drop.
			//We already have code in place to delete claimprocs when we drop ins, but the claimprocs attached to claims are protected.
			//The claim clearly needs to be deleted if they are dropping.  We need the user to delete the claim before they drop the plan.
			//We also have code in place to add new claimprocs when they add the correct insurance.
			List<Claim> claimList=Claims.Refresh(PatPlanCur.PatNum);
			for(int j=0;j<claimList.Count;j++) {
				if(claimList[j].PlanNum!=_planCur.PlanNum) {//different insplan
					continue;
				}
				if(claimList[j].DateService!=DateTime.Today) {//not today
					continue;
				}
				//Patient currently has a claim for the insplan they are trying to drop
				MsgBox.Show(this,"Please delete all of today's claims for this patient before dropping this plan.");
				return false;
			}
			PatPlans.Delete(PatPlanCur.PatPlanNum);//Estimates recomputed within Delete()
			//PlanCur.ComputeEstimatesForCur();
			_hasDropped=true;
			string logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_planCur.CarrierNum).CarrierName+" "+Lan.g(this,"was dropped.");
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null)?0:PatPlanCur.PatNum,logText,(_planCur==null)?0:_planCur.PlanNum,
				_planCur.SecDateTEdit);
			InsEditPatLogs.MakeLogEntry(null,PatPlanCur,InsEditPatLogType.PatPlan);
			DialogResult=DialogResult.OK;
			return true;
		}

		private void butLabel_Click(object sender,System.EventArgs e) {//TODO: Implement ODprintout pattern
			//LabelSingle label=new LabelSingle();
			PrintDocument pd=new PrintDocument();//only used to pass printerName
			long patNumCur = PatPlanCur!=null ? PatPlanCur.PatNum : 0;
			if(!PrinterL.SetPrinter(pd,PrintSituation.LabelSingle,patNumCur,textCarrier.Text+" insurance plan label printed")) {
				return;
			}
			Carrier carrier=new Carrier();
			carrier.CarrierName=textCarrier.Text;
			carrier.Phone=textPhone.Text;
			carrier.Address=textAddress.Text;
			carrier.Address2=textAddress2.Text;
			carrier.City=textCity.Text;
			carrier.State=textState.Text;
			carrier.Zip=textZip.Text;
			carrier.ElectID=textElectID.Text;
			carrier.NoSendElect=comboSendElectronically.GetSelected<NoSendElectType>();
			try {
				carrier=Carriers.GetIdentical(carrier);
			}
			catch(ApplicationException ex) {
				//the catch is just to display a message to the user.  It doesn't affect the success of the function.
				MessageBox.Show(ex.Message);
			}	
			LabelSingle.PrintCarrier(carrier.CarrierNum);//,pd.PrinterSettings.PrinterName);
		}

		///<summary>This only fills the grid on the screen.  It does not get any data from the database.</summary>
		private void FillBenefits() {
			benefitList.Sort();
			gridBenefits.BeginUpdate();
			gridBenefits.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Pat",28);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Level",60);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Type",70);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Category",72);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("%",30);//,HorizontalAlignment.Right);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Amt",40);//,HorizontalAlignment.Right);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Time Period",80);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Quantity",115);
			gridBenefits.ListGridColumns.Add(col);
			gridBenefits.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<benefitList.Count;i++) {
				row=new GridRow();
				if(benefitList[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(benefitList[i].CoverageLevel==BenefitCoverageLevel.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",benefitList[i].CoverageLevel.ToString()));
				}
				if(benefitList[i].BenefitType==InsBenefitType.CoInsurance && benefitList[i].Percent != -1) {
					row.Cells.Add("%");
				}
				else if(benefitList[i].BenefitType==InsBenefitType.WaitingPeriod) {
					row.Cells.Add(Lan.g(this,"Waiting Period"));
				}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",benefitList[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(benefitList[i])); //already translated
				if(benefitList[i].Percent==-1 ) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(benefitList[i].Percent.ToString());
				}
				if(benefitList[i].MonetaryAmt == -1) {
					//if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.Deductible) {
					//	row.Cells.Add(((Benefit)benefitList[i]).MonetaryAmt.ToString("n0"));
					//}
					//else {
					row.Cells.Add("");
					//}
				}
				else {
					row.Cells.Add(benefitList[i].MonetaryAmt.ToString("n0"));
				}
				if(benefitList[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",benefitList[i].TimePeriod.ToString()));
				}
				if(benefitList[i].Quantity>0) {
					if(benefitList[i].QuantityQualifier==BenefitQuantity.NumberOfServices
						&&(benefitList[i].TimePeriod==BenefitTimePeriod.ServiceYear
						|| benefitList[i].TimePeriod==BenefitTimePeriod.CalendarYear))
					{
						row.Cells.Add(benefitList[i].Quantity.ToString()+" "+Lan.g(this,"times per year")+" ");
					}
					else if(benefitList[i].QuantityQualifier==BenefitQuantity.NumberOfServices 
						&& benefitList[i].TimePeriod==BenefitTimePeriod.NumberInLast12Months) 
					{
						row.Cells.Add(benefitList[i].Quantity.ToString()+" "+Lan.g(this,"times in the last 12 months")+" ");
					}
					else {
						row.Cells.Add(benefitList[i].Quantity.ToString()+" "
							+Lan.g("enumBenefitQuantity",benefitList[i].QuantityQualifier.ToString()));
					}
				}
				else {
					row.Cells.Add("");
				}
				gridBenefits.ListGridRows.Add(row);
			}
			gridBenefits.EndUpdate();
			/*if(allCalendarYear){
				checkCalendarYear.CheckState=CheckState.Checked;
			}
			else if(allServiceYear){
				checkCalendarYear.CheckState=CheckState.Unchecked;
			}
			else{
				checkCalendarYear.CheckState=CheckState.Indeterminate;
			}*/
		}

		private void gridBenefits_DoubleClick(object sender,EventArgs e) {
			if(IsNewPlan && _planCur.PlanNum != _planOld.PlanNum) {  //If adding a new plan and picked existing plan from list
				//==Travis 05/06/2015:  Allowing users to edit insurance benefits for new plans that were picked from the list was causing problems with 
				//	duplicating benefits.  This was the fix we decided to go with, as the issue didn't seem to be affecting existing plans for a patient.
				MessageBox.Show(Lan.g(this,"You have picked an existing insurance plan and changes cannot be made to benefits until you have saved the plan for this new subscriber.")
					+"\r\n"+Lan.g(this,"To edit, click OK and then open the edit insurance plan window again."));
				return;
			}
			long patPlanNum=0;
			if(PatPlanCur!=null) {
				patPlanNum=PatPlanCur.PatPlanNum;
			}
			using FormInsBenefits FormI=new FormInsBenefits(_planCur.PlanNum,patPlanNum);
			FormI.OriginalBenList=benefitList;
			FormI.Note=textSubscNote.Text;
			FormI.MonthRenew=_planCur.MonthRenew;
			FormI.SubCur=_subCur;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK) {
				return;
			}
			FillBenefits();
			textSubscNote.Text=FormI.Note;
			_planCur.MonthRenew=FormI.MonthRenew;
		}

		///<summary>Gets an employerNum based on the name entered. Called from FillCur</summary>
		private void GetEmployerNum() {
			if(_planCur.EmployerNum==0) {//no employer was previously entered.
				if(textEmployer.Text=="") {
					//no change - Use what's in the database if they truly didn't change anything (PlanCur has no emp, text is blank, and text was always blank, they didn't switch insplans)
					if(PatPlanCur!=null && _employerNameOrig=="" && _planCur.PlanNum==_planCurOriginal.PlanNum) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_planCur.EmployerNum=insPlanDB.EmployerNum;
						_planCurOriginal.EmployerNum=insPlanDB.EmployerNum;
					}
				}
				else {
					_planCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
			else {//an employer was previously entered
				if(textEmployer.Text=="") {
					_planCur.EmployerNum=0;
				}
				//if text has changed - 
				else if(_employerNameOrig!=textEmployer.Text) {
					_planCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
				else {
					//no change - Use what's in the database
					if(PatPlanCur!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_planCur.EmployerNum=insPlanDB.EmployerNum;
						_planCurOriginal.EmployerNum=insPlanDB.EmployerNum;
					}
				}
			}
		}

		private void butGetElectronic_Click(object sender,EventArgs e) {
			if(IsPatPlanRemoved()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			//button not visible if SubCur is null
			if(PrefC.GetBool(PrefName.CustomizedForPracticeWeb)) {
				EligibilityCheckDentalXchange();
				return;
			}
			//Visible for everyone.
			Clearinghouse clearinghouseHq=Clearinghouses.GetDefaultEligibility();
			if(clearinghouseHq==null){
				MsgBox.Show(this,"No clearinghouse is set as default.");
				return;
			}
			if(clearinghouseHq.CommBridge!=EclaimsCommBridge.ClaimConnect 
				&& clearinghouseHq.CommBridge!=EclaimsCommBridge.EDS
				&& clearinghouseHq.CommBridge!=EclaimsCommBridge.WebMD
				&& clearinghouseHq.Eformat!=ElectronicClaimFormat.Canadian
				&& !Plugins.HookMethod(this,"FormInsPlan.butGetElectronic_Click_is270Supported",clearinghouseHq))
			{
				string error="So far, eligibility checks only work with ClaimConnect, EDS, WebMD (Emdeon Dental), and CDAnet.";
				object[] parameters={error};
				Plugins.HookAddCode(this,"FormInsPlan.butGetElectronic_Click_270NotSupportedError",parameters);
				error=(string)parameters[0];
				MsgBox.Show(this,error);
				return;
			}
			if(clearinghouseHq.Eformat==ElectronicClaimFormat.Canadian) {
				EligibilityCheckCanada();
				return;
			}
			//Validate the 271 settings before sending the request, otherwise the request might take 10-20 seconds to run, then the user might be blocked after waiting.
			//It is nicer to the user to not make them wait when they can fix the settings beforehand.
			string settingErrors271=X271.ValidateSettings();
			if(settingErrors271!="") {
				MessageBox.Show(settingErrors271);
				return;
			}
			if(!FillPlanCurFromForm()) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				string error;
				Etrans etrans=x270Controller.RequestBenefits(clearinghouseClin,_planCur,PatPlanCur.PatNum,_carrierCur,_subCur,out error);
				if(etrans != null) {
					//show the user a list of benefits to pick from for import--------------------------
					bool isDependentRequest=(PatPlanCur.PatNum!=_subCur.Subscriber);
					Carrier carrierCur=Carriers.GetCarrier(_planCur.CarrierNum);
					using FormEtrans270Edit formE=new FormEtrans270Edit(PatPlanCur.PatPlanNum,_planCur.PlanNum,_subCur.InsSubNum,isDependentRequest,_subCur.Subscriber,carrierCur.IsCoinsuranceInverted);
					formE.EtransCur=etrans;
					formE.IsInitialResponse=true;
					formE.benList=benefitList;
					if(formE.ShowDialog()==DialogResult.OK) {
						EB271.SetInsuranceHistoryDates(formE.ListImportedEbs,PatPlanCur.PatNum,_subCur);
						#region Plan Notes
						string patName=Patients.GetNameLF(PatPlanCur.PatNum);
						DateTime planEndDate=DateTime.MinValue;
						List<DTP271> listDates=formE.ListDTP;
						foreach(DTP271 date in listDates) {
							string dtpDateStr=DTP271.GetDateStr(date.Segment.Get(2),date.Segment.Get(3));
							if(date.Segment.Get(1)=="347") {//347 => Plan End
								planEndDate=X12Parse.ToDate(date.Segment.Get(3));
								if(!isDependentRequest) {
									textDateTerm.Text=dtpDateStr;
								}
							}
							if(isDependentRequest || date.Segment.Get(1)!="347"){
								string dtpDescript=DTP271.GetQualifierDescript(date.Segment.Get(1));
								string note="As of "+DateTime.Today.ToShortDateString()+" - "+patName+": "+Lan.g(this,dtpDescript)+", "+dtpDateStr+"\n";
								textSubscNote.Text=textSubscNote.Text.Insert(0,note);
							}
						}
						#endregion Plan Notes
						#region Drop plan and add popup
						if(isDependentRequest
							&& planEndDate.Year > 1900 && planEndDate < DateTime.Today
							&& MsgBox.Show(this,MsgBoxButtons.YesNo,"The plan has ended.  Would you like to drop this plan?"))
						{
							if(DropClickHelper()
								&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to add a popup to collect new insurance information from patient?"))
							{
								Popup popup=new Popup();
								popup.PatNum=PatPlanCur.PatNum;
								popup.PopupLevel=EnumPopupLevel.Patient;
								popup.IsNew=true;
								popup.Description=Lan.g(this,"Insurance expired.  Collect new insurance information.");
								using FormPopupEdit FormPE=new FormPopupEdit();
								FormPE.PopupCur=popup;
								FormPE.ShowDialog();
							}
						}
						#endregion Drop plan and add popup
					}
				}
				else if(!error.IsNullOrEmpty()) {
					MessageBox.Show(error);
				}
			}
			catch(Exception ex) {//although many errors will be caught and result in a response etrans.
				//this also catches validation errors such as missing info.
				Cursor=Cursors.Default;
				if(ex.Message.Contains("AAA*N**79*")){
					MsgBox.Show(this,"There is a problem with your benefits request. Check with your clearinghouse to ensure"
						+" they support Real Time Eligibility for this carrier and verify that the correct electronic ID is entered.");
				}
				else{
					using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(ex.Message);
					msgbox.ShowDialog();
				}
			}
			Cursor=Cursors.Default;
			DateTime dateLast270=Etranss.GetLastDate270(_planCur.PlanNum);
			if(dateLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateLast270.ToShortDateString();
			}
			FillBenefits();
		}

		private void butHistoryElect_Click(object sender,EventArgs e) {
			//button not visible if SubCur is null
			using FormBenefitElectHistory formB=new FormBenefitElectHistory(_planCur.PlanNum,PatPlanCur.PatPlanNum,_subCur.InsSubNum,_subCur.Subscriber,_planCur.CarrierNum);
			formB.BenList=benefitList;
			formB.ShowDialog();
			DateTime dateLast270=Etranss.GetLastDate270(_planCur.PlanNum);
			if(dateLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateLast270.ToShortDateString();
			}
			FillBenefits();
		}

		#region EligibilityCheckDentalXchange
		//This is not our code.   Added SPK/AAD 10/06 for eligibility check.-------------------------------------------------------------------------
		private void EligibilityCheckDentalXchange() {
			Cursor = Cursors.WaitCursor;
			OpenDental.com.dentalxchange.webservices.WebServiceService DCIService 
				= new OpenDental.com.dentalxchange.webservices.WebServiceService();
			OpenDental.com.dentalxchange.webservices.Credentials DCICredential 
				= new OpenDental.com.dentalxchange.webservices.Credentials();
			OpenDental.com.dentalxchange.webservices.Request DCIRequest = new OpenDental.com.dentalxchange.webservices.Request();
			OpenDental.com.dentalxchange.webservices.Response DCIResponse = new OpenDental.com.dentalxchange.webservices.Response();
			string loginID;
			string passWord;
			// Get Login / Password
			Clearinghouse clearinghouseHq=Clearinghouses.GetDefaultDental();
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			if(clearinghouseClin!=null) {
				loginID=clearinghouseClin.LoginID;
				passWord=clearinghouseClin.Password;
			}
			else {
				loginID = "";
				passWord = "";
			}
			if(loginID == "") {
				MessageBox.Show("ClaimConnect login ID and password are required to check eligibility.");
				Cursor = Cursors.Default;
				return;
			}
			// Set Credentials
			DCICredential.serviceID = "DCI Web Service ID: 001513";
			DCICredential.username = loginID;   // ABCuser
			DCICredential.password = passWord;  // testing1
			DCICredential.client = "Practice-Web";
			DCICredential.version = "1";
			// Set Request Document
			//textAddress.Text = PrepareEligibilityRequest();
			DCIRequest.content = PrepareEligibilityRequestDentalXchange(loginID,passWord);
			try {
				DCIResponse = DCIService.lookupEligibility(DCICredential,DCIRequest);
				//DisplayEligibilityStatus();
				ProcessEligibilityResponseDentalXchange(DCIResponse.content.ToString());
			}
			catch{//Exception ex) {
				// SPK /AAD 8/16/08 Display more user friendly error message
				MessageBox.Show("Error : Inadequate data for response. Payer site may be unavailable.");
			}
			Cursor = Cursors.Default;
		}

		private string PrepareEligibilityRequestDentalXchange(string loginID,string passWord) {
			DataTable table;
			string infoReceiverLastName;
			string infoReceiverFirstName;
			string practiceAddress1;
			string practiceAddress2;
			string practicePhone;
			string practiceCity;
			string practiceState;
			string practiceZip;
			string renderingProviderLastName;
			string renderingProviderFirstName;
			string GenderCode;
			string TaxoCode;
			string RelationShip;
			XmlDocument doc = new XmlDocument();
			XmlNode EligNode = doc.CreateNode(XmlNodeType.Element,"EligRequest","");
			doc.AppendChild(EligNode);
			// Prepare Namespace Attribute
			XmlAttribute nameSpaceAttribute = doc.CreateAttribute("xmlns","xsi","http://www.w3.org/2000/xmlns/");
			nameSpaceAttribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
			doc.DocumentElement.SetAttributeNode(nameSpaceAttribute);
			// Prepare noNamespace Schema Location Attribute
			XmlAttribute noNameSpaceSchemaLocation = doc.CreateAttribute("xsi","noNamespaceSchemaLocation","http://www.w3.org/2001/XMLSchema-instance");
			//dmg Not sure what this is for. This path will not exist on Unix and will fail. In fact, this path
			//will either not exist or be read-only on most Windows boxes, so this path specification is probably
			//a bug, but has not caused any user complaints thus far.
			noNameSpaceSchemaLocation.Value = @"D:\eligreq.xsd";
			doc.DocumentElement.SetAttributeNode(noNameSpaceSchemaLocation);
			//  Prepare AuthInfo Node
			XmlNode AuthInfoNode = doc.CreateNode(XmlNodeType.Element,"AuthInfo","");
			//  Create UserName / Password ChildNode for AuthInfoNode
			XmlNode UserName = doc.CreateNode(XmlNodeType.Element,"UserName","");
			XmlNode Password = doc.CreateNode(XmlNodeType.Element,"Password","");
			//  Set Value of UserID / Password
			UserName.InnerText = loginID;
			Password.InnerText = passWord;
			//  Append UserName / Password to AuthInfoNode
			AuthInfoNode.AppendChild(UserName);
			AuthInfoNode.AppendChild(Password);
			//  Append AuthInfoNode To EligNode
			EligNode.AppendChild(AuthInfoNode);
			//  Prepare Information Receiver Node
			XmlNode InfoReceiver = doc.CreateNode(XmlNodeType.Element,"InformationReceiver","");
			XmlNode InfoAddress = doc.CreateNode(XmlNodeType.Element,"Address","");
			XmlNode InfoAddressName = doc.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode InfoAddressFirstName = doc.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode InfoAddressLastName = doc.CreateNode(XmlNodeType.Element,"LastName","");
			// Get Provider Information
			table = Providers.GetDefaultPracticeProvider2();
			if(table.Rows.Count != 0) {
				infoReceiverFirstName = PIn.String(table.Rows[0][0].ToString());
				infoReceiverLastName = PIn.String(table.Rows[0][1].ToString());
				// Case statement for TaxoCode
				switch(PIn.Long(table.Rows[0][2].ToString())) {
					case 1:
						TaxoCode = "124Q00000X";
						break;
					case 2:
						TaxoCode = "1223D0001X";
						break;
					case 3:
						TaxoCode = "1223E0200X";
						break;
					case 4:
						TaxoCode = "1223P0106X";
						break;
					case 5:
						TaxoCode = "1223D0008X";
						break;
					case 6:
						TaxoCode = "1223S0112X";
						break;
					case 7:
						TaxoCode = "1223X0400X";
						break;
					case 8:
						TaxoCode = "1223P0221X";
						break;
					case 9:
						TaxoCode = "1223P0300X";
						break;
					case 10:
						TaxoCode = "1223P0700X";
						break;
					default:
						TaxoCode = "1223G0001X";
						break;
				}
			}
			else {
				infoReceiverFirstName = "Unknown";
				infoReceiverLastName = "Unknown";
				TaxoCode = "Unknown";
			};
			InfoAddressFirstName.InnerText = infoReceiverLastName;
			InfoAddressLastName.InnerText = infoReceiverFirstName;
			InfoAddressName.AppendChild(InfoAddressFirstName);
			InfoAddressName.AppendChild(InfoAddressLastName);
			XmlNode InfoAddressLine1 = doc.CreateNode(XmlNodeType.Element,"AddressLine1","");
			XmlNode InfoAddressLine2 = doc.CreateNode(XmlNodeType.Element,"AddressLine2","");
			XmlNode InfoPhone = doc.CreateNode(XmlNodeType.Element,"Phone","");
			XmlNode InfoCity = doc.CreateNode(XmlNodeType.Element,"City","");
			XmlNode InfoState = doc.CreateNode(XmlNodeType.Element,"State","");
			XmlNode InfoZip = doc.CreateNode(XmlNodeType.Element,"Zip","");
			//  Populate Practioner demographic from hash table
			practiceAddress1 = PrefC.GetString(PrefName.PracticeAddress);
			practiceAddress2 = PrefC.GetString(PrefName.PracticeAddress2);
			// Format Phone
			if(PrefC.GetString(PrefName.PracticePhone).Length == 10) {
				practicePhone = PrefC.GetString(PrefName.PracticePhone).Substring(0,3)
                                    + "-" + PrefC.GetString(PrefName.PracticePhone).Substring(3,3)
                                    + "-" + PrefC.GetString(PrefName.PracticePhone).Substring(6);
			}
			else {
				practicePhone = PrefC.GetString(PrefName.PracticePhone);
			}
			practiceCity = PrefC.GetString(PrefName.PracticeCity);
			practiceState = PrefC.GetString(PrefName.PracticeST);
			practiceZip = PrefC.GetString(PrefName.PracticeZip);
			InfoAddressLine1.InnerText = practiceAddress1;
			InfoAddressLine2.InnerText = practiceAddress2;
			InfoPhone.InnerText = practicePhone;
			InfoCity.InnerText = practiceCity;
			InfoState.InnerText = practiceState;
			InfoZip.InnerText = practiceZip;
			InfoAddress.AppendChild(InfoAddressName);
			InfoAddress.AppendChild(InfoAddressLine1);
			InfoAddress.AppendChild(InfoAddressLine2);
			InfoAddress.AppendChild(InfoPhone);
			InfoAddress.AppendChild(InfoCity);
			InfoAddress.AppendChild(InfoState);
			InfoAddress.AppendChild(InfoZip);
			InfoReceiver.AppendChild(InfoAddress);
			//SPK / AAD 8/13/08 Add NPI -- Begin
			XmlNode InfoReceiverProviderNPI = doc.CreateNode(XmlNodeType.Element,"NPI","");
			//Get Provider NPI #
			table = Providers.GetDefaultPracticeProvider3();
			if(table.Rows.Count != 0) {
				InfoReceiverProviderNPI.InnerText = PIn.String(table.Rows[0][0].ToString());
			};
			InfoReceiver.AppendChild(InfoReceiverProviderNPI);
			//SPK / AAD 8/13/08 Add NPI -- End
			XmlNode InfoCredential = doc.CreateNode(XmlNodeType.Element,"Credential","");
			XmlNode InfoCredentialType = doc.CreateNode(XmlNodeType.Element,"Type","");
			XmlNode InfoCredentialValue = doc.CreateNode(XmlNodeType.Element,"Value","");
			InfoCredentialType.InnerText = "TJ";
			InfoCredentialValue.InnerText = "123456789";
			InfoCredential.AppendChild(InfoCredentialType);
			InfoCredential.AppendChild(InfoCredentialValue);
			InfoReceiver.AppendChild(InfoCredential);
			XmlNode InfoTaxonomyCode = doc.CreateNode(XmlNodeType.Element,"TaxonomyCode","");
			InfoTaxonomyCode.InnerText = TaxoCode;
			InfoReceiver.AppendChild(InfoTaxonomyCode);
			//  Append InfoReceiver To EligNode
			EligNode.AppendChild(InfoReceiver);
			//  Payer Info
			XmlNode InfoPayer = doc.CreateNode(XmlNodeType.Element,"Payer","");
			XmlNode InfoPayerNEIC = doc.CreateNode(XmlNodeType.Element,"PayerNEIC","");
			InfoPayerNEIC.InnerText = textElectID.Text;
			InfoPayer.AppendChild(InfoPayerNEIC);
			EligNode.AppendChild(InfoPayer);
			//  Patient
			XmlNode Patient = doc.CreateNode(XmlNodeType.Element,"Patient","");
			XmlNode PatientName = doc.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode PatientFirstName = doc.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode PatientLastName = doc.CreateNode(XmlNodeType.Element,"LastName","");
			XmlNode PatientDOB = doc.CreateNode(XmlNodeType.Element,"DOB","");
			XmlNode PatientSubscriber = doc.CreateNode(XmlNodeType.Element,"SubscriberID","");
			XmlNode PatientRelationship = doc.CreateNode(XmlNodeType.Element,"RelationshipCode","");
			XmlNode PatientGender = doc.CreateNode(XmlNodeType.Element,"Gender","");
			// Read Patient FName,LName,DOB, and Gender from Patient Table
			table = Patients.GetPartialPatientData(PatPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				PatientFirstName.InnerText = PIn.String(table.Rows[0][0].ToString());
				PatientLastName.InnerText = PIn.String(table.Rows[0][1].ToString());
				PatientDOB.InnerText = PIn.String(table.Rows[0][2].ToString());
				switch(comboRelationship.Text) {
					case "Self":
						RelationShip = "18";
						break;
					case "Spouse":
						RelationShip = "01";
						break;
					case "Child":
						RelationShip = "19";
						break;
					default:
						RelationShip = "34";
						break;
				}
				switch(PIn.String(table.Rows[0][3].ToString())) {
					case "1":
						GenderCode = "F";
						break;
					default:
						GenderCode = "M";
						break;
				}
			}
			else {
				PatientFirstName.InnerText = "Unknown";
				PatientLastName.InnerText = "Unknown";
				PatientDOB.InnerText = "99/99/9999";
				RelationShip = "??";
				GenderCode = "?";
			}
			PatientName.AppendChild(PatientFirstName);
			PatientName.AppendChild(PatientLastName);
			PatientSubscriber.InnerText = textSubscriberID.Text;
			PatientRelationship.InnerText = RelationShip;
			PatientGender.InnerText = GenderCode;
			Patient.AppendChild(PatientName);
			Patient.AppendChild(PatientDOB);
			Patient.AppendChild(PatientSubscriber);
			Patient.AppendChild(PatientRelationship);
			Patient.AppendChild(PatientGender);
			EligNode.AppendChild(Patient);
			//  Subscriber
			XmlNode Subscriber = doc.CreateNode(XmlNodeType.Element,"Subscriber","");
			XmlNode SubscriberName = doc.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode SubscriberFirstName = doc.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode SubscriberLastName = doc.CreateNode(XmlNodeType.Element,"LastName","");
			XmlNode SubscriberDOB = doc.CreateNode(XmlNodeType.Element,"DOB","");
			XmlNode SubscriberSubscriber = doc.CreateNode(XmlNodeType.Element,"SubscriberID","");
			XmlNode SubscriberRelationship = doc.CreateNode(XmlNodeType.Element,"RelationshipCode","");
			XmlNode SubscriberGender = doc.CreateNode(XmlNodeType.Element,"Gender","");
			// Read Subscriber FName,LName,DOB, and Gender from Patient Table
			table=Patients.GetPartialPatientData2(PatPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				SubscriberFirstName.InnerText = PIn.String(table.Rows[0][0].ToString());
				SubscriberLastName.InnerText = PIn.String(table.Rows[0][1].ToString());
				SubscriberDOB.InnerText = PIn.String(table.Rows[0][2].ToString());
				switch(PIn.String(table.Rows[0][3].ToString())) {
					case "1":
						GenderCode = "F";
						break;
					default:
						GenderCode = "M";
						break;
				}
			}
			else {
				SubscriberFirstName.InnerText = "Unknown";
				SubscriberLastName.InnerText = "Unknown";
				SubscriberDOB.InnerText = "99/99/9999";
				GenderCode = "?";
			}
			SubscriberName.AppendChild(SubscriberFirstName);
			SubscriberName.AppendChild(SubscriberLastName);
			SubscriberSubscriber.InnerText = textSubscriberID.Text;
			SubscriberRelationship.InnerText = RelationShip;
			SubscriberGender.InnerText = GenderCode;
			Subscriber.AppendChild(SubscriberName);
			Subscriber.AppendChild(SubscriberDOB);
			Subscriber.AppendChild(SubscriberSubscriber);
			Subscriber.AppendChild(SubscriberRelationship);
			Subscriber.AppendChild(SubscriberGender);
			EligNode.AppendChild(Subscriber);
			//  Prepare Information Receiver Node
			XmlNode RenderingProvider = doc.CreateNode(XmlNodeType.Element,"RenderingProvider","");
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- Start
			XmlNode RenderingProviderNPI = doc.CreateNode(XmlNodeType.Element,"NPI","");
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- End
			XmlNode RenderingAddress = doc.CreateNode(XmlNodeType.Element,"Address","");
			XmlNode RenderingAddressName = doc.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode RenderingAddressFirstName = doc.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode RenderingAddressLastName = doc.CreateNode(XmlNodeType.Element,"LastName","");
			// Get Rendering Provider first and lastname
			// Read Patient FName,LName,DOB, and Gender from Patient Table
			table=Providers.GetPrimaryProviders(PatPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				renderingProviderFirstName = PIn.String(table.Rows[0][0].ToString());
				renderingProviderLastName = PIn.String(table.Rows[0][1].ToString());
			}
			else {
				renderingProviderFirstName = infoReceiverFirstName;
				renderingProviderLastName = infoReceiverLastName;
			};
			RenderingAddressFirstName.InnerText = renderingProviderFirstName;
			RenderingAddressLastName.InnerText = renderingProviderLastName;
			RenderingAddressName.AppendChild(RenderingAddressFirstName);
			RenderingAddressName.AppendChild(RenderingAddressLastName);
			XmlNode RenderingAddressLine1 = doc.CreateNode(XmlNodeType.Element,"AddressLine1","");
			XmlNode RenderingAddressLine2 = doc.CreateNode(XmlNodeType.Element,"AddressLine2","");
			XmlNode RenderingPhone = doc.CreateNode(XmlNodeType.Element,"Phone","");
			XmlNode RenderingCity = doc.CreateNode(XmlNodeType.Element,"City","");
			XmlNode RenderingState = doc.CreateNode(XmlNodeType.Element,"State","");
			XmlNode RenderingZip = doc.CreateNode(XmlNodeType.Element,"Zip","");
			RenderingProviderNPI.InnerText = InfoReceiverProviderNPI.InnerText;
			RenderingAddressLine1.InnerText = practiceAddress1;
			RenderingAddressLine2.InnerText = practiceAddress2;
			RenderingPhone.InnerText = practicePhone;
			RenderingCity.InnerText = practiceCity;
			RenderingState.InnerText = practiceState;
			RenderingZip.InnerText = practiceZip;
			RenderingAddress.AppendChild(RenderingAddressName);
			RenderingAddress.AppendChild(RenderingAddressLine1);
			RenderingAddress.AppendChild(RenderingAddressLine2);
			RenderingAddress.AppendChild(RenderingPhone);
			RenderingAddress.AppendChild(RenderingCity);
			RenderingAddress.AppendChild(RenderingState);
			RenderingAddress.AppendChild(RenderingZip);
			XmlNode RenderingCredential = doc.CreateNode(XmlNodeType.Element,"Credential","");
			XmlNode RenderingCredentialType = doc.CreateNode(XmlNodeType.Element,"Type","");
			XmlNode RenderingCredentialValue = doc.CreateNode(XmlNodeType.Element,"Value","");
			RenderingCredentialType.InnerText = "TJ";
			RenderingCredentialValue.InnerText = "123456789";
			RenderingCredential.AppendChild(RenderingCredentialType);
			RenderingCredential.AppendChild(RenderingCredentialValue);
			XmlNode RenderingTaxonomyCode = doc.CreateNode(XmlNodeType.Element,"TaxonomyCode","");
			RenderingTaxonomyCode.InnerText = TaxoCode;
			RenderingProvider.AppendChild(RenderingAddress);
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- Start
			RenderingProvider.AppendChild(RenderingProviderNPI);
			// SPK / AAD 8/13/08 Add NPI -- End
			RenderingProvider.AppendChild(RenderingCredential);
			RenderingProvider.AppendChild(RenderingTaxonomyCode);
			//  Append RenderingProvider To EligNode
			EligNode.AppendChild(RenderingProvider);
			return doc.OuterXml;
		}

		private void ProcessEligibilityResponseDentalXchange(string DCIResponse) {
			XmlDocument doc = new XmlDocument();
			XmlNode IsEligibleNode;
			string IsEligibleStatus;
			doc.LoadXml(DCIResponse);
			IsEligibleNode = doc.SelectSingleNode("EligBenefitResponse/isEligible");
			switch(IsEligibleNode.InnerText) {
				case "0": // SPK
					// HINA Added 9/2. 
					// Open new form to display complete response Detail
					using(Form formDisplayEligibilityResponse = new FormEligibilityResponseDisplay(doc,PatPlanCur.PatNum)) {
						formDisplayEligibilityResponse.ShowDialog();
					}
					break;
				case "1": // SPK
					// Process Error code and Message Node AAD
					XmlNode ErrorCode;
					XmlNode ErrorMessage;
					ErrorCode = doc.SelectSingleNode("EligBenefitResponse/Response/ErrorCode");
					ErrorMessage = doc.SelectSingleNode("EligBenefitResponse/Response/ErrorMsg");
					IsEligibleStatus = textSubscriber.Text + " is Not Eligible. Error Code:";
					IsEligibleStatus += ErrorCode.InnerText + " Error Description:" + ErrorMessage.InnerText;
					MessageBox.Show(IsEligibleStatus);
					break;
				default:
					IsEligibleStatus = textSubscriber.Text + " Eligibility status is Unknown";
					MessageBox.Show(IsEligibleStatus);
					break;
			}
		}

		#endregion

		private bool IsEmployerValid() {
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
			InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
			InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
			bool hasExistingEmployerChanged=(insPlanDB.CarrierNum!=0 && insPlanDB.EmployerNum!=_planCur.EmployerNum && insPlanDB.PlanNum==_planCur.PlanNum);//not new insplan and employer db not same as selection and insplan still used.
			if(_employerNameOrig=="") {//no employer was previously entered.
				if(textEmployer.Text=="") {
					//no change
				}
				else {
					if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//Employer was changed and they don't have perms to make new insplan (they picked plan from list).
						//Validate plan's employer in DB
						Employer employerDB=Employers.GetByName(textEmployer.Text);
						if(employerDB==null) {
							MsgBox.Show(this,"The Employer for this insurance plan has been combined or deleted since the plan was loaded.  Please choose another insurance plan.");
							return false;
						}
						if(hasExistingEmployerChanged) {//not a new insplan, and the employer was changed compared to what's in the DB.
							MsgBox.Show(this,"The Employer for this insurance plan has been changed since the plan was loaded.  Please choose another insurance plan.");
							return false;
						}
					}
					else { 
						//They do have perms and they chose an insurance plan or entered employer manually.  Could have entered in emp manually or picked from list, we don't care
					}
				}
			}
			else {//an employer was previously entered
				if(textEmployer.Text=="") {
					if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//Employer is now empty.  Need to see if the insplan in DB also has empty employer, or if someone else put one on it.
						if(hasExistingEmployerChanged) {//Not a new insplan and employer was changed
							MsgBox.Show(this,"The Employer for this insurance plan has been changed since the plan was loaded.  Please choose another insurance plan.");
							return false;
						}
					}
				}
				//if text has changed
				else if(_employerNameOrig!=textEmployer.Text) {//Employer text was changed since the window was loaded (picked from list or manually edited)
					if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//Without permission, they must have picked from list.  Verify employer still exists.  If it does, verify the insplan still has same employer.
						Employer employerDB=Employers.GetByName(textEmployer.Text);
						if(employerDB==null) {
							MsgBox.Show(this,"The Employer for this insurance plan has been combined or deleted since the plan was loaded.  Please choose another insurance plan.");
							return false;
						}						
						if(hasExistingEmployerChanged) {//Not a new insplan and employer was changed.
							MsgBox.Show(this,"The Employer for this insurance plan has been changed since the plan was loaded.  Please choose another insurance plan.");
							return false;
						}
					}
					else {//Are authorized
						if(_employerNameCur==textEmployer.Text) { //They picked from list and didn't change it manually.
							Employer employerDB=Employers.GetByName(textEmployer.Text);
							if(employerDB==null && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The Employer entered for this insurance plan has been combined or deleted since the plan was loaded.  Do you want to override those changes?")) {
								return false;
							}
						
							if(hasExistingEmployerChanged && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The Employer for this insurance plan has been changed since the plan was loaded.  Do you want to override those changes?")) {
								return false;
							}
						}
						else {
							//They changed it manually we don't care if it exists or not.
						}
					}
				}
				else {
					//Employer wasn't changed.
				}
			}
			return true;
		}

		private bool IsCarrierValid() {
			Carrier carrierForm=GetCarrierFromForm();
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
			InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
			InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);//Can have CarrierNum==0 if this is a new plan.
			bool hasExistingCarrierChanged=(insPlanDB.CarrierNum!=_carrierCur.CarrierNum && insPlanDB.CarrierNum!=0 && insPlanDB.CarrierNum!=_carrierNumOrig);
			if(_carrierCur.CarrierNum!=_carrierNumOrig && Carriers.Compare(carrierForm,_carrierCur)) {//Carrier was changed via "Pick From List" and not edited manually
				if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
					if(Carriers.GetCarrierDB(_carrierCur.CarrierNum)==null && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The Carrier selected has been combined or deleted since it was last picked.  Would you like to override those changes?")) {//Someone deleted/combined the carrier while the window was open.
						return false;
					}
					if(hasExistingCarrierChanged && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {//Someone changed this insplan's carrier while the window was open.
						return false;
					}
				}
				else {//Not authorized
					if(Carriers.GetCarrierDB(_carrierCur.CarrierNum)==null) {
						MsgBox.Show(this,"The selected insurance plan has had its carrier combined or deleted since it was last picked.  Please choose another.");
						return false;
					}
					if(hasExistingCarrierChanged) {//Someone changed this insplan's carrier while the window was open.
						MsgBox.Show(this,"The selected insurance plan has had its Carrier changed since it was loaded.  Please choose another.");
						return false;
					}
				}
			}
			else if(!Carriers.Compare(carrierForm,_carrierCur)) {//Carrier edited manually (doesn't matter if it was picked from list or not, user without perms can't edit manually)
				if(hasExistingCarrierChanged && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {//Someone changed this insplan's carrier while the window was open.
					return false;
				}
				//No need to look up if the carrier entered manually exists.  We can't tell if it doesn't exist or if it was deleted while the form was open.
				//If we look up a carrier using the info in the form, if it exists, then fine.  If it doesn't exist, is it because it was manually edited, or because someone else deleted it.
			}
			else if(_planOld.PlanNum!=_planCur.PlanNum) {//Plan was picked from list
				if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
					if(insPlanDB.CarrierNum!=_carrierCur.CarrierNum && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {
						return false;
					}
				}
				else {//Not authorized
					if(insPlanDB.CarrierNum!=_carrierCur.CarrierNum) {
						MsgBox.Show(this,"The selected insurance plan has had its Carrier changed since it was loaded.  Please choose another.");
						return false;
					}
				}
			}
			else {//Carrier information is the same from when it was loaded into the form and "Pick From List" wasn't used to change information.
				//Always use what's in the DB, no warnings
			}
			return true;
		}

		private Carrier GetCarrierFromForm() {
			Carrier carrier=new Carrier();
			carrier.CarrierName=textCarrier.Text;
			carrier.Phone=textPhone.Text;
			carrier.Address=textAddress.Text;
			carrier.Address2=textAddress2.Text;
			carrier.City=textCity.Text;
			carrier.State=textState.Text;
			carrier.Zip=textZip.Text;
			carrier.ElectID=textElectID.Text;
			carrier.NoSendElect=comboSendElectronically.GetSelected<NoSendElectType>();
			return carrier;
		}

		///<summary>Used from butGetElectronic_Click and from butOK_Click.  Returns false if unable to complete.  Also fills SubCur if not null.</summary>
		private bool FillPlanCurFromForm(){
			if(!textDateEffect.IsValid()
				|| !textDateTerm.IsValid()
				|| !textDentaide.IsValid()
				|| !textDateLastVerifiedBenefits.IsValid()
				|| !textDateLastVerifiedPatPlan.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textPlanFlag.Text!="" && textPlanFlag.Text!="A" && textPlanFlag.Text!="V" && textPlanFlag.Text!="N") {
					MsgBox.Show(this,"Plan flag must be A, V, N, or blank.");
					return false;
				}
				if(textPlanFlag.Text=="") {
					if(checkIsPMP.Checked) {
						MsgBox.Show(this,"The provincial medical plan checkbox must be unchecked when the plan flag is blank.");
						return false;
					}
				}
				else {
					if(!checkIsPMP.Checked) {
						MsgBox.Show(this,"The provincial medical plan checkbox must be checked when the plan flag is not blank.");
						return false;
					}
					if(textPlanFlag.Text=="A") {
						if(textCanadianDiagCode.Text=="" || textCanadianDiagCode.Text!=Canadian.TidyAN(textCanadianDiagCode.Text,textCanadianDiagCode.Text.Length,true)) {
							MsgBox.Show(this,"When plan flag is set to A, diagnostic code must be set and must be 6 characters or less in length.");
							return false;
						}
						if(textCanadianInstCode.Text=="" || textCanadianInstCode.Text!=Canadian.TidyAN(textCanadianInstCode.Text,textCanadianInstCode.Text.Length,true)) {
							MsgBox.Show(this,"When plan flag is set to A, institution code must be set and must be 6 characters or less in length.");
							return false;
						}
					}
				}
			}
			if(textSubscriberID.Text=="" && _subCur!=null) {
				MsgBox.Show(this,"Subscriber ID not allowed to be blank.");
				return false;
			}
			if(textCarrier.Text=="") {
				MsgBox.Show(this,"Carrier not allowed to be blank.");
				return false;
			}
			if(PatPlanCur!=null && !textOrdinal.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(comboRelationship.SelectedIndex==-1 && comboRelationship.Items.Count>0) {
				MsgBox.Show(this,"Relationship to Subscriber is not allowed to be blank.");
				return false;
			}
			if(PatPlanCur!=null && !IsEmployerValid()) {
				return false;
			}
			if(PatPlanCur!=null && !IsCarrierValid()) {
				return false;
			}
			if(_subCur!=null) {
				//Subscriber: Only changed when user clicks change button.
				_subCur.SubscriberID=textSubscriberID.Text;
				_subCur.DateEffective=PIn.Date(textDateEffect.Text);
				_subCur.DateTerm=PIn.Date(textDateTerm.Text);
				_subCur.ReleaseInfo=checkRelease.Checked;
				_subCur.AssignBen=checkAssign.Checked;
				_subCur.SubscNote=textSubscNote.Text;
				//MonthRenew already handled inside benefit window.
			}
			GetEmployerNum();
			_planCur.GroupName=textGroupName.Text;
			_planCur.GroupNum=textGroupNum.Text;
			_planCur.RxBIN=textBIN.Text;
			_planCur.DivisionNo=textDivisionNo.Text;//only visible in Canada
			//carrier-----------------------------------------------------------------------------------------------------
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//User has the ability to edit carrier information.  Check for matches, create new Carrier if applicable.
				Carrier carrierForm=GetCarrierFromForm();
				Carrier carrierOld=_carrierCur.Copy();
				if(_carrierCur.CarrierNum==_carrierNumOrig && Carriers.Compare(carrierForm,_carrierCur) && _planCur.PlanNum==_planOld.PlanNum) {
					//carrier is the same as it was originally, use what's in db if editing a patient's patplan.
					if(PatPlanCur!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_carrierCur=Carriers.GetCarrier(insPlanDB.CarrierNum);
						_planCurOriginal.CarrierNum=_carrierCur.CarrierNum;
					}
					else {
						//Someone could have changed the insplan while the user was editing this window, do not overwrite the other users changes.
						InsPlan insPlanDB=InsPlans.GetPlan(_planCur.PlanNum,null);
						if(insPlanDB.PlanNum==0) {
							MsgBox.Show(this,"Insurance plan has been combined or deleted since the window was opened.  Please press Cancel to continue and refresh the list of insurance plans.");
							return false;
						}
						_carrierCur=Carriers.GetCarrier(insPlanDB.CarrierNum);
						_planCurOriginal.CarrierNum=_carrierCur.CarrierNum;
					}
				}
				else {
					_carrierCur=carrierForm;
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						bool carrierFound=true;
						try {
							_carrierCur=Carriers.GetIdentical(_carrierCur);
						}
						catch {//match not found
							carrierFound=false;
						}
						if(!carrierFound) {
							if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Carrier not found.  Create new carrier?")) {
								return false;
							}
							using FormCarrierEdit formCE=new FormCarrierEdit();
							formCE.IsNew=true;
							formCE.CarrierCur=_carrierCur;
							formCE.ShowDialog();
							if(formCE.DialogResult!=DialogResult.OK) {
								return false;
							}
						}
					}
					else {
						_carrierCur=Carriers.GetIdentical(_carrierCur,carrierOld: carrierOld);
					}
				}
				_planCur.CarrierNum=_carrierCur.CarrierNum;
			}
			else {//User does not have permission to edit carrier information.  
				//We don't care if carrier info is changed, only if it's removed.  
				//If it's removed, have them choose another.  If it's simply changed, just use the same prikey.
				if(Carriers.GetCarrier(_carrierCur.CarrierNum).CarrierName=="" && _planCur.PlanNum!=_planOld.PlanNum) {//Carrier not found, it must have been deleted or combined
					MsgBox.Show(this,"Selected carrier has been combined or deleted.  Please choose another insurance plan.");
					return false;
				}
				else if(_planCur.PlanNum==_planOld.PlanNum) {//Didn't switch insplan, they were only viewing.
					long planNumDb=_planOld.PlanNum;
					if(PatPlanCur!=null) {
						//Another user could have edited this patient's plan at the same time and could have changed something about the pat plan so we need
						//to go to the database an make sure that we are "not changing anything" by saving potentially stale data to the db.
						//If we don't do this, then we would end up overriding any changes that other users did while we were in this edit window.
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						planNumDb=insSubDB.PlanNum;
					}
					InsPlan insPlanDB=InsPlans.GetPlan(planNumDb,null);
					_carrierCur=Carriers.GetCarrier(insPlanDB.CarrierNum);
					_planCurOriginal.CarrierNum=_carrierCur.CarrierNum;
					_planCur.CarrierNum=_carrierCur.CarrierNum;
				}
				else { 
					_planCur.CarrierNum=_carrierCur.CarrierNum;
				}
			}
			//plantype already handled.
			if(comboClaimForm.SelectedIndex!=-1){
				_planCur.ClaimFormNum=comboClaimForm.GetSelected<ClaimForm>().ClaimFormNum;
			}
			_planCur.UseAltCode=checkAlternateCode.Checked;
			_planCur.CodeSubstNone=checkCodeSubst.Checked;
			_planCur.HasPpoSubstWriteoffs=checkPpoSubWo.Checked;
			_planCur.IsMedical=checkIsMedical.Checked;
			_planCur.ClaimsUseUCR=checkClaimsUseUCR.Checked;
			_planCur.IsHidden=checkIsHidden.Checked;
			_planCur.ShowBaseUnits=checkShowBaseUnits.Checked;
			if(comboFeeSched.SelectedIndex==0) {
				_planCur.FeeSched=0;
			}
			else if(comboFeeSched.SelectedIndex == -1) {//Hidden fee schedule selected in comboFeeSched
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Fee Schedule has been hidden. Are you sure you want to continue?")) {
					return false;
				}
				_planCur.FeeSched=_planCurOriginal.FeeSched;
			}
			else{
				_planCur.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboCopay.SelectedIndex==0){
				_planCur.CopayFeeSched=0;//none
			}
			else if(comboCopay.SelectedIndex==-1) {//CopayFeeSched is hidden or switching to/from PPO Fixed Benefits plan
				if(IsFixedBenefitMismatch(FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==comboCopay.GetSelectedKey<FeeSched>(x=>x.FeeSchedNum)))) {
					MessageBox.Show(Lans.g(this,"PPO Fixed Benefits Fee Schedules can only be assigned to PPO Fixed Benefits plan types. "
						+"Please make a valid selection from ")+labelCopayFeeSched.Text);
					return false;	
				}
				if(MessageBox.Show(this,Lans.g(this,"The selected ")+labelCopayFeeSched.Text
					+Lan.g(this," fee schedule has been hidden. Are you sure you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No) 
				{
					return false;
				}
				_planCur.CopayFeeSched=_planCurOriginal.CopayFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_planCur.CopayFeeSched=comboCopay.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboOutOfNetwork.SelectedIndex==0){
				if(IsNewPlan
					&& _planCur.PlanType==""//percentage
					&& PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.LegacyBlueBook){
					//add a fee schedule if needed
					FeeSched sched=FeeScheds.GetByExactName(_carrierCur.CarrierName,FeeScheduleType.OutNetwork);
					if(sched==null){
						sched=new FeeSched();
						sched.Description=_carrierCur.CarrierName;
						sched.FeeSchedType=FeeScheduleType.OutNetwork;
						//sched.IsNew=true;
						sched.IsGlobal=true;
						sched.ItemOrder=FeeScheds.GetCount();
						FeeScheds.Insert(sched);
						DataValid.SetInvalid(InvalidType.FeeScheds);
					}
					_planCur.AllowedFeeSched=sched.FeeSchedNum;
				}
				else{
					_planCur.AllowedFeeSched=0;
				}
			}
			else if(comboOutOfNetwork.SelectedIndex==-1) {//Hidden fee schedule selected in comboAllowedFeeSched
				if(comboOutOfNetwork.Enabled 
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Out Of Network fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_planCur.AllowedFeeSched=_planCurOriginal.AllowedFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_planCur.AllowedFeeSched=comboOutOfNetwork.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboManualBlueBook.SelectedIndex==-1) {
				if(comboManualBlueBook.Enabled
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Manual Blue book fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_planCur.ManualFeeSchedNum=_planCurOriginal.ManualFeeSchedNum;//No change, maintain hidden feesched.
			}
			else {
				_planCur.ManualFeeSchedNum=comboManualBlueBook.GetSelected<FeeSched>().FeeSchedNum;
			}
			_planCur.CobRule=(EnumCobRule)comboCobRule.SelectedIndex;
			//Canadian------------------------------------------------------------------------------------------
			_planCur.DentaideCardSequence=PIn.Byte(textDentaide.Text);
			_planCur.CanadianPlanFlag=textPlanFlag.Text;//validated
			_planCur.CanadianDiagnosticCode=textCanadianDiagCode.Text;//validated
			_planCur.CanadianInstitutionCode=textCanadianInstCode.Text;//validated
			//Canadian end---------------------------------------------------------------------------------------
			_planCur.TrojanID=textTrojanID.Text;
			_planCur.PlanNote=textPlanNote.Text;
			_planCur.HideFromVerifyList=checkDontVerify.Checked;
			//Ortho----------------------------------------------------------------------------------------------
			_planCur.OrthoType=comboOrthoClaimType.SelectedIndex==-1 ? 0 : (OrthoClaimType)Enum.GetValues(typeof(OrthoClaimType)).GetValue(comboOrthoClaimType.SelectedIndex);
			if(_orthoAutoProc!=null) {
				_planCur.OrthoAutoProcCodeNumOverride=_orthoAutoProc.CodeNum;
			}
			else {
				_planCur.OrthoAutoProcCodeNumOverride=0; //overridden by practice default.
			}
			_planCur.OrthoAutoProcFreq=comboOrthoAutoProcPeriod.SelectedIndex==-1 ? 0 : (OrthoAutoProcFrequency)Enum.GetValues(typeof(OrthoAutoProcFrequency)).GetValue(comboOrthoAutoProcPeriod.SelectedIndex);
			_planCur.OrthoAutoClaimDaysWait=checkOrthoWaitDays.Checked ? 30 : 0;
			_planCur.OrthoAutoFeeBilled=PIn.Double(textOrthoAutoFee.Text);
			return true;
		}

		///<summary>Warns user if there are received claims for this plan.  Returns true if user wants to proceed, or if there are no received claims for this plan.  Returns false if the user aborts.</summary>
		private bool CheckForReceivedClaims() {
			long patNum=0;
			if(PatPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
				patNum=PatPlanCur.PatNum;
			}
			int claimCount=0;
			if(patNum==0) {//Editing insurance plans from Lists > Insurance Plans.
				//Check all claims for plan
				claimCount=Claims.GetCountReceived(_planCurOriginal.PlanNum);
				if(claimCount!=0) {
					if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return false; //abort
					}
				}
			}
			else {//Editing insurance plans from Family module.
				if(radioChangeAll.Checked==true) {//Check radio button
					claimCount=Claims.GetCountReceived(_planCurOriginal.PlanNum);
					if(claimCount!=0) {//Check all claims for plan
						if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false; //abort
						}
					}
				}
				else {//Check claims for plan and patient only
					claimCount=Claims.GetCountReceived(_planCurOriginal.PlanNum,PatPlanCur.InsSubNum);
					if(claimCount!=0) {
						if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false; //abort
						}
					}
				}
			}
			return true;
		}

		private void butSubstCodes_Click(object sender,EventArgs e) {
			InsPlan planCurCopy=_planCur.Copy();
			planCurCopy.CodeSubstNone=checkCodeSubst.Checked;
			using FormInsPlanSubstitution FormInsSubst=new FormInsPlanSubstitution(planCurCopy);
			if(FormInsSubst.ShowDialog()==DialogResult.OK) {
				checkCodeSubst.Checked=planCurCopy.CodeSubstNone;//Since the user can change this flag in the other window.
			}
		}

		private void butVerifyPatPlan_Click(object sender,EventArgs e) {
			textDateLastVerifiedPatPlan.Text=DateTime.Today.ToShortDateString();
		}

		private void butVerifyBenefits_Click(object sender,EventArgs e) {
			textDateLastVerifiedBenefits.Text=DateTime.Today.ToShortDateString();
		}

		private void butAuditPat_Click(object sender,EventArgs e) {
			if(PatPlanCur==null) {
				return;
			}
			using FormInsEditPatLog formInsEditPatLog=new FormInsEditPatLog(PatPlanCur);
			formInsEditPatLog.ShowDialog();
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(IsPatPlanRemoved()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			GetEmployerNum();
			using FormInsEditLogs FormIEL = new FormInsEditLogs(_planCur,benefitList);
			FormIEL.ShowDialog();
		}

		private void comboOrthoClaimType_SelectionChangeCommitted(object sender,EventArgs e) {
			SetEnabledOrtho();
		}

		private void butPatOrtho_Click(object sender,EventArgs e) {
			if(comboOrthoClaimType.SelectedIndex != (int)OrthoClaimType.InitialPlusPeriodic) {
				MsgBox.Show(this,"To view this setup window, the insurance plan must be set to have an Ortho Claim Type of Initial Plus Periodic.");
				return;
			}
			double defaultFee=PIn.Double(textOrthoAutoFee.Text);
			string carrierName=PIn.String(textCarrier.Text);
			string subID=PIn.String(textSubscriberID.Text);
			if(defaultFee==0) {
				defaultFee=_planCur.OrthoAutoFeeBilled;
			}
			using FormOrthoPat FormOP = new FormOrthoPat(PatPlanCur,_planCur,carrierName,subID,defaultFee);
			FormOP.ShowDialog();
		}

		private void butPickOrthoProc_Click(object sender,EventArgs e) {
			using FormProcCodes FormPC = new FormProcCodes();
			FormPC.IsSelectionMode=true;
			FormPC.ShowDialog();
			if(FormPC.DialogResult == DialogResult.OK) {
				_orthoAutoProc=ProcedureCodes.GetProcCode(FormPC.SelectedCodeNum);
				textOrthoAutoProc.Text=_orthoAutoProc.ProcCode;
			}
		}

		private void butDefaultAutoOrthoProc_Click(object sender,EventArgs e) {
			_orthoAutoProc=null;
			textOrthoAutoProc.Text=ProcedureCodes.GetProcCode(PrefC.GetLong(PrefName.OrthoAutoProcCodeNum)).ProcCode+" ("+Lan.g(this,"Default")+")";
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			bool removeLogs=false;
			#region Validation
			if(IsPatPlanRemoved()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if((radioChangeAll.Checked || (radioCreateNew.Checked && textLinkedNum.Text=="0")) //These are the two scenarios in which InsPlans.Update will be called instead of Insert.
				&& (_planCur==null || InsPlans.GetPlan(_planCur.PlanNum,new List<InsPlan>())==null)) 
			{
				MsgBox.Show(this,"The selected insurance plan was removed by another user and no longer exists.  Open insurance plan again to edit.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_subCur!=null && InsPlans.GetPlan(_subCur.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"The subscriber's insurance plan was merged by another user and no longer exists.  Open insurance plan again to edit.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(DoShowBluebookDeletionMsgBox()) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,Lan.g(this,"Changing a plan's PlanType from Category Percentage to another type or adding a Fee "
					+"Schedule will make the plan ineligible for Blue Book estimates and will delete all Blue Book data for the plan. Would you like to continue?")))
				{
					return;
				}
			}
			long selectedFilingCodeNum=0;
			if(comboFilingCode.GetSelected<InsFilingCode>()!=null) {
				selectedFilingCodeNum=comboFilingCode.GetSelected<InsFilingCode>().InsFilingCodeNum;
			}
			_planCur.FilingCode=selectedFilingCodeNum;
			_planCur.FilingCodeSubtype=0;
			if(comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>()!=null) {
				_planCur.FilingCodeSubtype=comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>().InsFilingCodeSubtypeNum;
			}
			#endregion Validation
			#region 1 - Validate Carrier Received Claims
			try {
			if(!FillPlanCurFromForm()) {//also fills SubCur if not null
				return;
			}
			if(_planCur.CarrierNum!=_planCurOriginal.CarrierNum) {
				long patNum=0;
				if(PatPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
					patNum=PatPlanCur.PatNum;
				}
				string carrierNameOrig=Carriers.GetCarrier(_planCurOriginal.CarrierNum).CarrierName;
				string carrierNameNew=Carriers.GetCarrier(_planCur.CarrierNum).CarrierName;
				if(carrierNameOrig!=carrierNameNew) {//The CarrierNum could have changed but the CarrierName might not have changed.  Only warn the name changed.
					if(!CheckForReceivedClaims()) {
						return;
					}
				}
			}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 1")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion 1 - Validate Carrier Received Claims
			#region 2 - InsPlanChangeAssign Permission Check
			try {
			//We do not want to block users from creating new plans for subscribers if they do not have the InsPlanChangeAssign permission.
			//Therefore, we will only check the permission if they are editing an old plan.
			if(_subOld!=null) {//Editing an old plan for a subscriber.
				if(_subOld.AssignBen!=checkAssign.Checked) {
					if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign)) {
						return;
					}
					//It is very possible that the user changed the patient associated to the ins sub.
					//We need to make a security log for the most recent patient (_subCur.Subscriber) instead of the original patient (_subOld.Subscriber) that was passed in.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeAssign,_subCur.Subscriber,Lan.g(this,"Assignment of Benefits (pay dentist) changed from")
						+" "+(_subOld.AssignBen?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" "
						+Lan.g(this,"to")
						+" "+(checkAssign.Checked?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" for plan "
						+Carriers.GetCarrier(_planCur.CarrierNum).CarrierName);
				}
			}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 2")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion 2 - InsPlanChangeAssign Permission Check
			#region 3 - PatPlan
			try {
				//Validation is finished at this point.
				//PatPlan-------------------------------------------------------------------------------------------
				if(PatPlanCur!=null) {
					if(PIn.Long(textOrdinal.Text)!=PatPlanCur.Ordinal) {//Ordinal changed by user
						PatPlanCur.Ordinal=(byte)(PatPlans.SetOrdinal(PatPlanCur.PatPlanNum,PIn.Int(textOrdinal.Text)));
						_hasOrdinalChanged=true;
					}
					else if(PIn.Long(textOrdinal.Text)!=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum).Ordinal) {
						//PatPlan's ordinal changed by somebody else and not this user, set it to what's in the DB for this update.
						PatPlanCur.Ordinal=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum).Ordinal;
					}
					PatPlanCur.IsPending=checkIsPending.Checked;
					PatPlanCur.Relationship=(Relat)comboRelationship.SelectedIndex;
					PatPlanCur.PatID=textPatID.Text;
					if(_patPlanOld!=null) {
						_patPlanOld.PatID=_patPlanOld.PatID??"";
					}
					PatPlans.Update(PatPlanCur,_patPlanOld);
					if(!PIn.Date(textDateLastVerifiedPatPlan.Text).Date.Equals(_datePatPlanLastVerified.Date)) {
						InsVerify insVerify=InsVerifies.GetOneByFKey(PatPlanCur.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerify!=null) {
							insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedPatPlan.Text);
							InsVerifyHists.InsertFromInsVerify(insVerify);
						}
					}
					if(!IsNewPatPlan) {//Updated
						InsEditPatLogs.MakeLogEntry(PatPlanCur,_patPlanOld,InsEditPatLogType.PatPlan);
					}
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 3")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			//It is okay to set the plan num on the subscriber object at this point.
			//This is mainly for users that do not have the InsPlanEdit permission which should be allowed to manipulate subscribers.
			//The plan num could change again farther down if the user actually has permission to manipulate the ins plan.
			if(_subCur!=null) {
				_subCur.PlanNum=_planCur.PlanNum;
			}
			#endregion 3 - PatPlan
			//Sections 4 - 10 all deal with manipulating the insurance plan so make sure the user has permission to do so.
			#region InsPlan Edit
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				if(_subCur==null) {//editing from big list.  No subscriber.  'pick from list' button not visible, making logic easier.
					#region 4 - InsPlan Null Subscriber
					try {
						if(InsPlans.AreEqualValue(_planCur,_planCurOriginal)) {//If no changes
							//pick button doesn't complicate things.  Simply nothing to do.  Also, no SubCur, so just close the form.
							DialogResult=DialogResult.OK;
						}
						else {//changes were made
							InsPlans.Update(_planCur);
							DialogResult=DialogResult.OK;
						}
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Error Code 4")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
						return;
					}
					#endregion 4 - InsPlan Null Subscriber
				}//end if(_subCur==null)
				else {//(subCur!=null) editing from within patient
					//Be very careful here.  User could have clicked 'pick from list' button, which would have changed PlanNum.
					//So we always compare with PlanNumOriginal.
					if(IsNewPlan) {
						if(InsPlans.AreEqualValue(_planCur,_planCurOriginal)) {//New plan, no changes
							#region 5 - InsPlan Non-Null Subscriber, New Plan, No Changes Made
							//If the logic in this region changes, then also change region 5a below.
							try {
								if(_planCur.PlanNum != _planOld.PlanNum) {//clicked 'pick from list' button
									//No need to update PlanCur because no changes, delete original plan.
									try {
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(PatPlanCur.PatNum,_planOld.PlanNum,_planCur.PlanNum);
										}
										//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
										InsPlans.Delete(_planOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
										benefitListOld=new List<Benefit>();
										removeLogs=true;
									}
									catch(ApplicationException ex) {
										MessageBox.Show(ex.Message);
										//do not need to update PlanCur because no changes were made.
										SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null) ? 0 : PatPlanCur.PatNum
											,Lan.g(this,"FormInsPlan region 5 delete validation failed.  Plan was not deleted."),_planOld.PlanNum,
											DateTime.MinValue); //new plan, no date needed.
										Close();
										return;
									}
									_subCur.PlanNum=_planCur.PlanNum;
								}
								else {//new plan, no changes, not picked from list.
									//do not need to update PlanCur because no changes were made.
								}
							}
							catch(Exception ex) { //catch any other exceptions and display
								MessageBox.Show(Lan.g(this,"Error Code 5")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
								return;
							}
							#endregion 5 - InsPlan Non-Null Subscriber, New Plan, No Changes Made
						}
						else {//new plan, changes were made
							#region 6 - InsPlan Non-Null Subscriber, New Plan, Changes Made
							if(_planCur.PlanNum != _planOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								//If the logic in this region changes, then also change region 6a below.
								try {
									if(radioChangeAll.Checked) {
										InsPlans.Update(_planCur);//they might not realize that they would be changing an existing plan. Oh well.
										try {
											if(_didAddInsHistCP) {
												//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
												ClaimProcs.UpdatePlanNumForInsHist(PatPlanCur.PatNum,_planOld.PlanNum,_planCur.PlanNum);
											}
											//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
											InsPlans.Delete(_planOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
											benefitListOld=new List<Benefit>();
											removeLogs=true;
										}
										catch(ApplicationException ex) {
											MessageBox.Show(ex.Message);
											SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null) ? 0 : PatPlanCur.PatNum
												,Lan.g(this,"FormInsPlan region 6 delete validation failed.  Plan was not deleted."),_planOld.PlanNum,
												DateTime.MinValue); //new plan, no date needed.
											Close();
											return;
										}
										_subCur.PlanNum=_planCur.PlanNum;
									}
									else {//option is checked for "create new plan if needed"
										_planCur.PlanNum=_planOld.PlanNum;
										InsPlans.Update(_planCur);
										_subCur.PlanNum=_planCur.PlanNum;
										//no need to update PatPlan.  Same old PlanNum.  When 'pick from list' button was pushed, benfitList was filled with benefits from
										//the picked plan.  benefitListOld was not touched and still contains the old benefits.  So the original benefits will be
										//automatically deleted.  We force copies to be made in the database, but with different PlanNums.  Any other changes will be preserved.
										for(int i = 0;i<benefitList.Count;i++) {
											if(benefitList[i].PlanNum>0) {
												benefitList[i].PlanNum=_planCur.PlanNum;
												benefitList[i].BenefitNum=0;//triggers insert during synch.
											}
										}
									}
								}
								catch(Exception ex) {
									MessageBox.Show(Lan.g(this,"Error Code 6")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
									return;
								}
							}
							else {//new plan, changes made, not picked from list.
								InsPlans.Update(_planCur);
							}
							#endregion 6 - InsPlan Non-Null Subscriber, New Plan, Changes Made
						}//end else of if(InsPlans.AreEqual...
					}//end if(IsNewPlan)
					else {//editing an existing plan from within patient
						if(InsPlans.AreEqualValue(_planCur,_planCurOriginal)) {//If no changes
							#region 7 - InsPlan Non-Null Subscriber, Not a New Plan, No Changes Made
							try {
								if(_planCur.PlanNum != _planOld.PlanNum) {//clicked 'pick from list' button, then made no changes
									//do not need to update PlanCur because no changes were made.
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(PatPlanCur.PatNum,_planOld.PlanNum,_planCur.PlanNum);
									}
									_subCur.PlanNum=_planCur.PlanNum;
									//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
									//So if any benefits were changed, the synch further down will trigger updates for the benefits on the picked plan.
								}
								else {//existing plan, no changes, not picked from list.
									//do not need to update PlanCur because no changes were made.
								}
							}
							catch(Exception ex) {
								MessageBox.Show(Lan.g(this,"Error Code 7")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
								return;
							}
							#endregion 7 - InsPlan Non-Null Subscriber, Not a New Plan, No Changes Made
						}//end if(InsPlans.AreEqual...
						else {//changes were made
							if(_planCur.PlanNum != _planOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								if(radioChangeAll.Checked) {
									#region 8 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Change All Checked
									try {
										//warn user here?
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(PatPlanCur.PatNum,_planOld.PlanNum,_planCur.PlanNum);
										}
										InsPlans.Update(_planCur);
										_subCur.PlanNum=_planCur.PlanNum;
										//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
										//So if any benefits were changed, the synch further down will trigger updates for the benefits on the picked plan.
									}
									catch(Exception ex) {
										MessageBox.Show(Lan.g(this,"Error Code 8")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
										return;
									}
									#endregion 8 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Change All Checked
								}
								else {//option is checked for "create new plan if needed"
									#region 9 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Create New Plan Checked
									try {
										if(textLinkedNum.Text=="0") {//if this is the only subscriber
											InsPlans.Update(_planCur);
											_subCur.PlanNum=_planCur.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//So if any benefits were changed, the synch further down will trigger updates for the benefits on the picked plan.
										}
										else {//if there are other subscribers
											InsPlans.Insert(_planCur);//this gives it a new primary key.
											_subCur.PlanNum=_planCur.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//We must clear the benefitListOld to prevent deletion of those benefits.
											benefitListOld=new List<Benefit>();
											//Force copies to be made in the database, but with different PlanNum;
											for(int i = 0;i<benefitList.Count;i++) {
												if(benefitList[i].PlanNum>0) {
													benefitList[i].PlanNum=_planCur.PlanNum;
													benefitList[i].BenefitNum=0;//triggers insert during synch.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_planCur.PlanNum,_planOld.PlanNum);
										}
									}
									catch(Exception ex) {
										MessageBox.Show(Lan.g(this,"Error Code 9")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
										return;
									}
									#endregion 9 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Create New Plan Checked
								}
							}
							else {//existing plan, changes made, not picked from list.
								#region 10 - InsPlan Non-Null Subscriber, Not a New Plan, Not Picked From List, Changes Made
								try {
									if(radioChangeAll.Checked) {
										InsPlans.Update(_planCur);
									}
									else {//option is checked for "create new plan if needed"
										if(textLinkedNum.Text=="0") {//if this is the only subscriber
											InsPlans.Update(_planCur);
										}
										else {//if there are other subscribers
											InsPlans.Insert(_planCur);//this gives it a new primary key.
											_subCur.PlanNum=_planCur.PlanNum;
											//PatPlanCur.PlanNum=PlanCur.PlanNum;
											//PatPlans.Update(PatPlanCur);
											//make copies of all the benefits
											benefitListOld=new List<Benefit>();
											for(int i = 0;i<benefitList.Count;i++) {
												if(benefitList[i].PlanNum>0) {
													benefitList[i].PlanNum=_planCur.PlanNum;
													benefitList[i].BenefitNum=0;//triggers insert.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_planCur.PlanNum,_planOld.PlanNum);
										}
									}
								}
								catch(Exception ex) {
									MessageBox.Show(Lan.g(this,"Error Code 10")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
									return;
								}
								#endregion 10 - InsPlan Non-Null Subscriber, Not a New Plan, Not Picked From List, Changes Made
							}
						}//end else of if(InsPlans.AreEqual...
					}//end else of if(IsNewPlan)
				}//end else of if(_subCur==null)
			}//End InsPlanEdit permission check
			else {//User does not have the InsPlanEdit permission.
				if(_subCur!=null) {
					if(IsNewPlan) {
						#region 5a - User Without Permissions, InsPlan Non-Null Subscriber, New Plan
						//If the logic in this region changes, then also change region 5 above.
						try {
							if(_planCur.PlanNum != _planOld.PlanNum) {//user clicked the "pick from list" button. 
								//In a previous version, a user could still change some things about the plan even if they had no permissions to do so.
								//This was causing empty insurance plans to get saved to the db.
								//Even if they somehow managed to change something about the insurance plan they picked, we always just want to do the following:
								//1. Update the inssub to be the current insplan. (which happens above)
								//2. Delete the empty insurance plan (which happens here)
								try {
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(PatPlanCur.PatNum,_planOld.PlanNum,_planCur.PlanNum);
									}
									//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
									InsPlans.Delete(_planOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
									benefitListOld=new List<Benefit>();
									removeLogs=true;
								}
								catch(ApplicationException ex) {
									MessageBox.Show(ex.Message);
									SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null) ? 0 : PatPlanCur.PatNum
										,Lan.g(this,"FormInsPlan region 5a delete validation failed.  Plan was not deleted."),
										_planOld.PlanNum,DateTime.MinValue); //new plan, no date needed.
									Close();
									return;
								}
							}
						}
						catch(Exception ex) {
							MessageBox.Show(Lan.g(this,"Error Code 5a")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
							return;
						}
						#endregion 5a - User Without Permissions, InsPlan Non-Null Subscriber, New Plan
					}
				}
			}
			#endregion InsPlan Edit
			#region InsSub and Benefit Sync
			try {
				if(!PIn.Date(textDateLastVerifiedBenefits.Text).Date.Equals(_dateInsPlanLastVerified.Date)) {
					InsVerify insVerify=InsVerifies.GetOneByFKey(_planCur.PlanNum,VerifyTypes.InsuranceBenefit);
					insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedBenefits.Text);
					InsVerifyHists.InsertFromInsVerify(insVerify);
				}
				//PatPlanCur.InsSubNum is already set before opening this window.  There is no possible way to change it from within this window.  Even if PlanNum changes, it's still the same inssub.  And even if inssub.Subscriber changes, it's still the same inssub.  So no change to PatPlanCur.InsSubNum is ever require from within this window.
				if(benefitListOld.Count>0 || benefitList.Count>0) {//Synch benefits
					Benefits.UpdateList(benefitListOld,benefitList);
				}
				if(removeLogs) {
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_planOld.PlanNum);
				}
				if(_subCur!=null) {//Update SubCur if needed
					InsSubs.Update(_subCur);//also saves the other fields besides PlanNum
					if(_subOld.Subscriber!=_subCur.Subscriber) {//If the subscriber was changed, include an audit trail entry
						Dictionary<long,string> dictPatNames=Patients.GetPatientNames(new List<long>() { _subOld.Subscriber,_subCur.Subscriber });
						//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
						//However, the Change button is invisible from List > Insurance Plans, so we can count on PatPlanCur not null.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,PatPlanCur.PatNum,
							Lan.g(this,"Subscriber Changed from")+" "+dictPatNames[_subOld.Subscriber]+" #"+_subOld.Subscriber+" "
							+Lan.g(this,"to")+" "+dictPatNames[_subCur.Subscriber]+" #"+_subCur.Subscriber);
					}
					//Udate all claims, claimprocs, payplans, and etrans that are pointing at the inssub.InsSubNum since it may now be pointing at a new insplan.PlanNum.
					InsSubs.SynchPlanNumsForNewPlan(_subCur);
					InsPlans.ComputeEstimatesForSubscriber(_subCur.Subscriber);
					InsEditPatLogs.MakeLogEntry(_subCur,_subOld,InsEditPatLogType.Subscriber);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 11")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion InsSub and Benefit Sync
			#region Carrier
			try {
				//Check for changes in the carrier
				if(_planCur.CarrierNum!=_planCurOriginal.CarrierNum) {
					_hasCarrierChanged=true;
					long patNum=0;
					if(PatPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
						patNum=PatPlanCur.PatNum;
					}
					string carrierNameOrig=Carriers.GetCarrier(_planCurOriginal.CarrierNum).CarrierName;
					string carrierNameNew=Carriers.GetCarrier(_planCur.CarrierNum).CarrierName;
					if(carrierNameOrig!=carrierNameNew) {//The CarrierNum could have changed but the CarrierName might not have changed.  Only make an audit entry if the name changed.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeCarrierName,patNum,Lan.g(this,"Carrier name changed in Edit Insurance Plan window from")+" "
							+(string.IsNullOrEmpty(carrierNameOrig)?"blank":carrierNameOrig)+" "+Lan.g(this,"to")+" "
							+(string.IsNullOrEmpty(carrierNameNew)?"blank":carrierNameNew),_planCur.PlanNum,_planCurOriginal.SecDateTEdit);
					}
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 12")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion Carrier
			#region Carrier FeeSched
			try {
				Carrier carrierCur=Carriers.GetCarrier(_planCur.CarrierNum);
				if(_planCurOriginal.FeeSched!=0 && _planCurOriginal.FeeSched!=_planCur.FeeSched) {
					string feeSchedOld=FeeScheds.GetDescription(_planCurOriginal.FeeSched);
					string feeSchedNew=FeeScheds.GetDescription(_planCur.FeeSched);
					string logText=Lan.g(this,"The fee schedule associated with insurance plan number")+" "+_planCur.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"was changed from")+" "+feeSchedOld+" "+Lan.g(this,"to")+" "+feeSchedNew;
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,PatPlanCur==null?0:PatPlanCur.PatNum,logText,(_planCur==null)?0:_planCur.PlanNum,
						_planCurOriginal.SecDateTEdit);
				}
				if(InsPlanCrud.UpdateComparison(_planCurOriginal,_planCur)) {
					string logText=Lan.g(this,"Insurance plan")+" "+_planCur.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"has changed.");
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,PatPlanCur==null?0:PatPlanCur.PatNum,logText,(_planCur==null)?0:_planCur.PlanNum,
						_planCurOriginal.SecDateTEdit);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 13")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion Carrier FeeSched
			//InsBlueBook entries should only exist for category percentage plans.
			//Delete if no longer blue book eligible, otherwise update.
			if(_planCur.PlanType!="" || _planCur.FeeSched!=0) {
				InsBlueBooks.DeleteByPlanNums(_planCur.PlanNum);
			}
			else {
				InsBlueBooks.UpdateByInsPlan(_planCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		///<summary>Check to see if we should show confirmation message to user about deleting bluebook data for the currently populated form. Returns true if the criteria are met to show, false otherwise.</summary>
		private bool DoShowBluebookDeletionMsgBox() {
			bool isBluebookOn=PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook;
			if(!isBluebookOn) {//Bluebook feature must be on to warn user.
				return false;
			}
			if(IsNewPlan && _planOld.PlanNum==_planCurOriginal.PlanNum) {//Never have to warn user because the plan is new and was not picked from the list.
				return false;
			}
			if(_planCurOriginal.PlanType!="") {//Not Category Percentage plan so not bluebook eligible regardless if fee schedule changes.
				return false;
			}
			if((_planCurOriginal.FeeSched==0 && comboFeeSched.SelectedIndex!=0)	|| (_planCurOriginal.PlanType=="" && _planCur.PlanType!="")) {
				//Warn user because we have changed to use a fee schedule or from Category Percentage plan to a non-bluebook eligible plan.
				return true;
			}
			return false;
		}

		private void FormInsPlan_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				if(PatPlanCur!=null && (_hasDropped || _hasOrdinalChanged || _hasCarrierChanged || IsNewPatPlan || IsNewPlan || _hasDeleted || 
					_planCur.IsMedical!=_planOld.IsMedical)) 
				{
					Appointments.UpdateInsPlansForPat(PatPlanCur.PatNum);
				}
				if(IsNewPatPlan//Only when assigning new insurance
					&& PatPlanCur.Ordinal==1//Primary insurance.
					&& _planCur.BillingType!=0//Selection made.
					&& Security.IsAuthorized(Permissions.PatientBillingEdit,true)
					&& PrefC.GetBool(PrefName.PatInitBillingTypeFromPriInsPlan))
				{
					Patient patOld=Patients.GetPat(PatPlanCur.PatNum);
					if(patOld.BillingType!=_planCur.BillingType) {
						Patient patNew=patOld.Copy();
						patNew.BillingType=_planCur.BillingType;
						Patients.Update(patNew,patOld);
						//This needs to be the last call due to automation possibily leaving the form in a closing limbo.
						AutomationL.Trigger(AutomationTrigger.SetBillingType,null,patNew.PatNum);
						Patients.InsertBillTypeChangeSecurityLogEntry(patOld,patNew);
					}
				}
				if(_subCur!=null && _hasDeleted) {
					List<PatPlan> listPatPlansForSub=PatPlans.GetListByInsSubNums(new List<long> { _subCur.InsSubNum });
					foreach(PatPlan patPlan in listPatPlansForSub) {
						Appointments.UpdateInsPlansForPat(patPlan.PatNum);
					}
				}
				return;
			}
			//So, user cancelled a new entry
			if(IsNewPlan){//this would also be new coverage
				//warning: If user clicked 'pick from list' button, then we don't want to delete an existing plan used by others
				try {
					if(_subCur!=null) {
						InsSubs.Delete(_subCur.InsSubNum);
					}
					InsPlans.Delete(_planOld,canDeleteInsSub:false,doInsertInsEditLogs:false);//does dependency checking.
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_planOld.PlanNum);
					//Ok to delete these adjustments because we deleted the benefits in Benefits.DeleteForPlan().
					ClaimProcs.DeleteMany(AdjAL.ToArray().Cast<ClaimProc>().ToList());
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(PatPlanCur==null)?0:PatPlanCur.PatNum
						,Lan.g(this,"FormInsPlan_Closing delete validation failed.  Plan was not deleted."),_planOld.PlanNum,DateTime.MinValue);//new plan, no date needed.
					return;
				}
			}
			if(IsNewPatPlan){
				PatPlans.Delete(PatPlanCur.PatPlanNum);//no need to check dependencies.  Maintains ordinals and recomputes estimates.
			}
		}

		///<summary>Check if PatPlan was dropped since window was opened.</summary>
		private bool IsPatPlanRemoved() {
			if(PatPlanCur!=null) {
				PatPlan ppExists=PatPlans.GetByPatPlanNum(PatPlanCur.PatPlanNum);
				if(ppExists==null) {
					MsgBox.Show(this,"This plan was removed by another user and no longer exists.");
					return true;
				}
			}
			return false;
		}

		///<summary>This is related to insplan.PlanType, but that column is a string.
		///We should have used an enum instead of string values for insplan.PlanType to begin with.
		///However, too late to change now.  This enum makes this form more human readable.</summary>
		private enum InsPlanTypeComboItem {
			///<summary>0</summary>
			CategoryPercentage,
			///<summary>1</summary>
			PPO,
			///<summary>2</summary>
			PPOFixedBenefit,
			///<summary>3</summary>
			MedicaidOrFlatCopay,
			///<summary>4</summary>
			Capitation,
		}
	}
}
