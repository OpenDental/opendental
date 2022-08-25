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
		private string _stringEmpOriginal;
		private bool _isMouseInListEmps;
		private List<Carrier> _listCarriersSimilar;
		private string _carrierOriginal;
		private bool _isMouseInListCarriers;
		private Carrier _carrierCur;
		private PatPlan _patPlanCur;
		private PatPlan _patPlanOld;
		private ArrayList _arrayListAdj;
		///<summary>This is the current benefit list that displays on the form.  It does not get saved to the database until this form closes.</summary>
		private List<Benefit> _listBenefit;//each item is a Benefit
		private List<Benefit> _listBenefitOld;
		//<summary>Set to true if called from the list of insurance plans.  In this case, the planNum will be 0.  There will be no subscriber.  Benefits will be 'typical' rather than from one specific plan.  Upon saving, all similar plans will be set to be exactly the same as PlanCur.</summary>
		//public bool IsForAll;//Instead, just pass in a null subscriber.
		///<summary>Set to true from FormInsPlansMerge.  In this case, the insplan is read only, because it's much more complicated to allow user to change.</summary>
		//public bool IsReadOnly;
		private List<FeeSched> _listFeeSchedsStandard;
		private List<FeeSched> _listFeeSchedsCopay;
		private List<FeeSched> _listFeeSchedsOutOfNetwork;
		private List<FeeSched> _listFeeSchedsManualBlueBook;
		private bool _hasDropped=false;
		private bool _hasOrdinalChanged=false;
		private bool _hasCarrierChanged=false;
		private bool _hasDeleted=false;
		private InsSub _subOld;
		private DateTime _dateTimeInsPlanLastVerified;
		private DateTime _dateTimePatPlanLastVerified;
		///<summary>The carrier num when the window was loaded.  Used to track if carrier has been changed.</summary>
		private long _carrierNumOrig;
		///<summary>The employer num when the window was loaded.  Used to track if the employer has been changed.</summary>
		private string _employerNameOrig;
		private string _employerNameCur;
		private string _electIdCur;
		private ProcedureCode _procedureCodeOrthoAuto;
		private List<InsFilingCode> _listInsFilingCodes;
		private List<ClaimForm> _listClaimForms;
		//<summary>This is a field that is accessed only by clicking on the button because there's not room for it otherwise.  This variable should be treated just as if it was a visible textBox.</summary>
		//private string BenefitNotes;
		///<summary>Currently selected plan in the window.</summary>
		private InsPlan _insPlanCur;
		///<summary>This is a copy of PlanCur as it was originally when this form was opened.  
		///This is needed to determine whether plan was changed.  However, this is also reset when 'pick from list' is used.</summary>
		private InsPlan _planCurOriginal;
		///<summary>Ins sub for the currently selected plan.</summary>
		private InsSub _insSubCur;
		private bool _didAddInsHistCP;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listBoxEmps;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox listBoxCarriers;
		///<summary>The plan type that is selected in comboPlanType</summary>
		private InsPlanTypeComboItem _insPlanTypeComboItemSelected;

		///<summary>The original plan that was passed into this form. Assigned in the constructor and can never be modified.  
		///This allows intelligent decisions about how to save changes.</summary>
		private InsPlan _insPlanOld {
			get;
		}

		public long PlanCurNum {
			get {
				return _insPlanCur.PlanNum;
			}
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsPlanCanada";
			}
			return "FormInsPlan";
		}

		///<summary>Called from ContrFamily and FormInsPlans. Must pass in the plan, patPlan, and sub, although patPlan and sub can be null.</summary>
		public FormInsPlan(InsPlan InsPlanCur,PatPlan patPlanCur,InsSub insSubCur){
			Cursor=Cursors.WaitCursor;
			InitializeComponent();
			InitializeLayoutManager();
			_insPlanCur=InsPlanCur;
			_insPlanOld=_insPlanCur.Copy();
			_patPlanCur=patPlanCur;
			_patPlanOld=patPlanCur?.Copy();
			_insSubCur=insSubCur;
			listBoxEmps=new ListBox();//Instead of ListBoxOD for consistency with listCars.
			listBoxEmps.Location=new Point(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+textEmployer.Left,
				tabPageInsPlanInfo.Top+tabControlInsPlan.Top+panelPlan.Top+groupPlan.Top+textEmployer.Bottom);
			listBoxEmps.Size=new Size(231,100);
			listBoxEmps.Visible=false;
			listBoxEmps.Click += new System.EventHandler(listBoxEmps_Click);
			listBoxEmps.DoubleClick += new System.EventHandler(listBoxEmps_DoubleClick);
			listBoxEmps.MouseEnter += new System.EventHandler(listBoxEmps_MouseEnter);
			listBoxEmps.MouseLeave += new System.EventHandler(listBoxEmps_MouseLeave);
			LayoutManager.Add(listBoxEmps,this);
			listBoxEmps.BringToFront();
			listBoxCarriers=new ListBox();//Instead of ListBoxOD, for horiz scroll on a dropdown.
			listBoxCarriers.Location=new Point(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+groupCarrier.Left+textCarrier.Left,
				tabControlInsPlan.Top+tabPageInsPlanInfo.Top+panelPlan.Top+groupPlan.Top+groupCarrier.Top+textCarrier.Bottom);
			listBoxCarriers.Size=new Size(700,100);
			listBoxCarriers.HorizontalScrollbar=true;
			listBoxCarriers.Visible=false;
			listBoxCarriers.Click += new System.EventHandler(listBoxCarriers_Click);
			listBoxCarriers.DoubleClick += new System.EventHandler(listBoxCarriers_DoubleClick);
			listBoxCarriers.MouseEnter += new System.EventHandler(listBoxCarriers_MouseEnter);
			listBoxCarriers.MouseLeave += new System.EventHandler(listBoxCarriers_MouseLeave);
			LayoutManager.Add(listBoxCarriers,this);
			listBoxCarriers.BringToFront();
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
				checkIsPMP.Checked=(InsPlanCur.CanadianPlanFlag!=null && InsPlanCur.CanadianPlanFlag!="");
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
			_planCurOriginal=_insPlanCur.Copy();
			_listInsFilingCodes=InsFilingCodes.GetDeepCopy();
			if(_insSubCur!=null) {
				_subOld=_insSubCur.Copy();
			}
			long patPlanNum=0;
			checkUseBlueBook.Visible=false; // hidden by default, shown only if bluebook feature is turned on and plan is category%
			comboOutOfNetwork.Enabled=true;
			comboManualBlueBook.Enabled=false;
			if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook && _insPlanCur.PlanType=="") {
				if(_insPlanCur.IsBlueBookEnabled) {
					comboOutOfNetwork.Enabled=false;
					comboManualBlueBook.Enabled=true;
				}
				checkUseBlueBook.Visible=true; // only show when bluebook is enabled and plan is Cat%. ""== Cat%
			}
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
				//Job E31830 allows users without the InsPlanEdit permission to request electronic benefits and view request history.
				//butGetElectronic.Enabled=false;
				//butHistoryElect.Enabled=false;
				butImportTrojan.Enabled=false;
				butIapFind.Enabled=false;
				butBenefitNotes.Enabled=false;
				checkDontVerify.Enabled=false;
				textTrojanID.Enabled=false;
				checkUseBlueBook.Enabled=false;
				//Allow users to verify that the current insurance plan information is correct.  Since this doesn't affect the insurance plan itself,
				//it is acceptable to allow them to acknowledge correct plans.
				//butVerifyBenefits.Enabled=false;
				//textDateLastVerifiedBenefits.Enabled=false;
				butDelete.Enabled=false;
			}
			if(!Security.IsAuthorized(Permissions.InsuranceVerification,true)) {
				//Disable buttons that set UI to now.
				butVerifyPatPlan.Visible=false;//Using Visible instead of Enabled, Enabled makes the button background transparent and it looks strange.
				butVerifyBenefits.Visible=false;
				//Disable manual modification too.
				textDateLastVerifiedPatPlan.ReadOnly=true;
				textDateLastVerifiedBenefits.ReadOnly=true;
			}
			if(_patPlanCur!=null) {
				patPlanNum=_patPlanCur.PatPlanNum;
			}
			if(_insSubCur==null) {//editing from big list
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
			checkDontVerify.Checked=_insPlanCur.HideFromVerifyList;
			InsVerify insVerifyBenefitsCur=InsVerifies.GetOneByFKey(_insPlanCur.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefitsCur!=null && insVerifyBenefitsCur.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefitsCur.DateLastVerified.ToShortDateString();
			}
			if(IsNewPlan) {//Regardless of whether from big list or from individual patient.  Overrides above settings.
				//radioCreateNew.Checked=true;//this logic needs to be repeated in OK.
				//groupChanges.Visible=false;//because it wouldn't make sense to apply anything to "all"
				if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
					_insPlanCur.PlanType="p";
					checkUseBlueBook.Visible=false;
				}
				_insPlanCur.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
				textDateLastVerifiedBenefits.Text="";
			}
			_listBenefit=Benefits.RefreshForPlan(_insPlanCur.PlanNum,patPlanNum);
			_listBenefitOld=new List<Benefit>();
			for(int i=0;i<_listBenefit.Count;i++){
				_listBenefitOld.Add(_listBenefit[i].Copy());
			}
			if(_insPlanCur.PlanNum!=0) {
				textInsPlanNum.Text=_insPlanCur.PlanNum.ToString();
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
				textTrojanID.Text=_insPlanCur.TrojanID;
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
			if(_patPlanCur==null) {
				panelPat.Visible=false;
				//PatPlanCur is sometimes null
				butGetElectronic.Visible=false;
				butHistoryElect.Visible=false;
			}
			else {
				comboRelationship.Items.Clear();
				for(int i=0;i<Enum.GetNames(typeof(Relat)).Length;i++) {
					comboRelationship.Items.Add(Lan.g("enumRelat",Enum.GetNames(typeof(Relat))[i]));
					if((int)_patPlanCur.Relationship==i) {
						comboRelationship.SelectedIndex=i;
					}
				}
				if(_patPlanCur.PatPlanNum!=0) {
					textPatPlanNum.Text=_patPlanCur.PatPlanNum.ToString();
					if(IsNewPatPlan) {
						//Relationship is set to Self,  but the subscriber for the plan is not set to the current patient.
						if(comboRelationship.SelectedIndex==0 && _insSubCur.Subscriber!=_patPlanCur.PatNum) {
								comboRelationship.SelectedIndex=-1;
						}
					}
					else {
						InsVerify insVerifyPatPlanCur=InsVerifies.GetOneByFKey(_patPlanCur.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerifyPatPlanCur!=null && insVerifyPatPlanCur.DateLastVerified.Year>1880) {
							textDateLastVerifiedPatPlan.Text=insVerifyPatPlanCur.DateLastVerified.ToShortDateString();
						}
					}
				}
				textOrdinal.Text=_patPlanCur.Ordinal.ToString();
				checkIsPending.Checked=_patPlanCur.IsPending;
				textPatID.Text=_patPlanCur.PatID;
				FillPatientAdjustments();
			}
			if(_insSubCur!=null) {
				textSubscriber.Text=Patients.GetLim(_insSubCur.Subscriber).GetNameLF();
				textSubscriberID.Text=_insSubCur.SubscriberID;
				if(_insSubCur.DateEffective.Year < 1880) {
					textDateEffect.Text="";
				}
				else {
					textDateEffect.Text=_insSubCur.DateEffective.ToString("d");
				}
				if(_insSubCur.DateTerm.Year < 1880) {
					textDateTerm.Text="";
				}
				else {
					textDateTerm.Text=_insSubCur.DateTerm.ToString("d");
				}
				checkRelease.Checked=_insSubCur.ReleaseInfo;
				checkAssign.Checked=_insSubCur.AssignBen;
				textSubscNote.Text=_insSubCur.SubscNote;
			}
			_listFeeSchedsStandard=FeeScheds.GetListForType(FeeScheduleType.Normal,false);
			_listFeeSchedsCopay=FeeScheds.GetListForType(FeeScheduleType.CoPay,false)
				.Union(FeeScheds.GetListForType(FeeScheduleType.FixedBenefit,false))
				.ToList();
			_listFeeSchedsOutOfNetwork=FeeScheds.GetListForType(FeeScheduleType.OutNetwork,false);
			_listFeeSchedsManualBlueBook=FeeScheds.GetListForType(FeeScheduleType.ManualBlueBook,false);
			//Clearinghouse clearhouse=Clearinghouses.GetDefault();
			//if(clearhouse==null || clearhouse.CommBridge!=EclaimsCommBridge.ClaimConnect) {
			//	butEligibility.Visible=false;
			//}
			_employerNameOrig=Employers.GetName(_insPlanCur.EmployerNum);
			_employerNameCur=Employers.GetName(_insPlanCur.EmployerNum);
			_carrierNumOrig=_insPlanCur.CarrierNum;
			_listClaimForms=ClaimForms.GetDeepCopy(false);
			comboSendElectronically.Items.AddEnums<NoSendElectType>();//selected index set in FillFormWithPlanCur -> FillCarrier
			FillFormWithPlanCur(false);
			FillBenefits();
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlanCur.PlanNum);
			if(dateTimeLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateTimeLast270.ToShortDateString();
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkCodeSubst.Visible=false;
			}
			_dateTimePatPlanLastVerified=PIn.Date(textDateLastVerifiedPatPlan.Text);
			_procedureCodeOrthoAuto=_insPlanCur.OrthoAutoProcCodeNumOverride==0 ? null : ProcedureCodes.GetProcCode(_insPlanCur.OrthoAutoProcCodeNumOverride);
			FillOrtho();
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true) || !Security.IsAuthorized(Permissions.CarrierEdit,true)) {
				groupCarrier.Enabled=false;
			}
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
				if(_insPlanCur.OrthoType==type) {
					comboOrthoClaimType.SelectedIndex = (int)type;
				}
			}
			comboOrthoAutoProcPeriod.Items.Clear();
			foreach(OrthoAutoProcFrequency type in Enum.GetValues(typeof(OrthoAutoProcFrequency))) {
				comboOrthoAutoProcPeriod.Items.Add(Lan.g("enumOrthoAutoProcFrequency",type.GetDescription()));
				if(_insPlanCur.OrthoAutoProcFreq==type) {
					comboOrthoAutoProcPeriod.SelectedIndex = (int)type;
				}
			}
			textOrthoAutoFee.Text=_insPlanCur.OrthoAutoFeeBilled.ToString();
			checkOrthoWaitDays.Checked=_insPlanCur.OrthoAutoClaimDaysWait > 0;
			if(_procedureCodeOrthoAuto!=null) {
				textOrthoAutoProc.Text=_procedureCodeOrthoAuto.ProcCode;
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
			textEmployer.Text=Employers.GetName(_insPlanCur.EmployerNum);
			_employerNameCur=textEmployer.Text;
			textGroupName.Text=_insPlanCur.GroupName;
			textGroupNum.Text=_insPlanCur.GroupNum;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				textBIN.Text=_insPlanCur.RxBIN;
			}
			else{
				labelBIN.Visible=false;
				textBIN.Visible=false;
			}
			textDivisionNo.Text=_insPlanCur.DivisionNo;//only visible in Canada
			textTrojanID.Text=_insPlanCur.TrojanID;
			comboPlanType.Items.Clear();
			//Items must be added in the same order in which they are listed in InsPlanTypeComboItem.
			comboPlanType.Items.Add(Lan.g(this,"Category Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Fixed Benefit"));
			comboPlanType.Items.Add(Lan.g(this,"Medicaid or Flat Co-pay"));
			//Capitation must always be last, since it is sometimes hidden.
			if(!PrefC.GetBool(PrefName.EasyHideCapitation)) {
				comboPlanType.Items.Add(Lan.g(this,"Capitation"));
				if(_insPlanCur.PlanType=="c") {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.Capitation;
				}
			}
			if(_insPlanCur.PlanType=="") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.CategoryPercentage;
			}
			if(_insPlanCur.PlanType=="p") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPO;
				FeeSched feeSchedCopay=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==_insPlanCur.CopayFeeSched 
					&& x.FeeSchedType==FeeScheduleType.FixedBenefit);
				if(feeSchedCopay!=null) {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPOFixedBenefit;
				}
			}
			if(_insPlanCur.PlanType=="f") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.MedicaidOrFlatCopay;
			}
			_insPlanTypeComboItemSelected=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			checkAlternateCode.Checked=_insPlanCur.UseAltCode;
			checkCodeSubst.Checked=_insPlanCur.CodeSubstNone;
			checkPpoSubWo.Checked=_insPlanCur.HasPpoSubstWriteoffs;
			checkIsMedical.Checked=_insPlanCur.IsMedical;
			if(!PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				checkIsMedical.Visible=false;//This line prevents most users from modifying the Medical Insurance checkbox by accident, because most offices are dental only.
			}
			checkClaimsUseUCR.Checked=_insPlanCur.ClaimsUseUCR;
			if(IsNewPlan && _insPlanCur.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims) && !isPicked) {
				checkClaimsUseUCR.Checked=true;
			}
			if(IsNewPlan && !PrefC.GetBool(PrefName.InsDefaultAssignBen) && !isPicked) {
				checkAssign.Checked=false;
			}
			checkIsHidden.Checked=_insPlanCur.IsHidden;
			checkShowBaseUnits.Checked=_insPlanCur.ShowBaseUnits;
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.AddNone<FeeSched>();
			comboFeeSched.Items.AddList(_listFeeSchedsStandard,x=>x.Description);
			comboFeeSched.SetSelectedKey<FeeSched>(_insPlanCur.FeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
			comboClaimForm.Items.Clear();
			foreach(ClaimForm claimForm in _listClaimForms) {
				//The default claim form will always show even if hidden.
				if(claimForm.IsHidden && claimForm.ClaimFormNum!=_insPlanCur.ClaimFormNum && claimForm.ClaimFormNum!=PrefC.GetLong(PrefName.DefaultClaimForm)) {
					continue;
				}
				comboClaimForm.Items.Add(claimForm.Description+(claimForm.IsHidden?" (hidden)":""),claimForm);
				if(claimForm.ClaimFormNum==_insPlanCur.ClaimFormNum) {
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
			comboOutOfNetwork.SetSelectedKey<FeeSched>(_insPlanCur.AllowedFeeSched,x=>x.FeeSchedNum);
			comboManualBlueBook.Items.Clear();
			comboManualBlueBook.Items.AddNone<FeeSched>();
			comboManualBlueBook.Items.AddList(_listFeeSchedsManualBlueBook,x => x.Description);
			comboManualBlueBook.SetSelectedKey<FeeSched>(_insPlanCur.ManualFeeSchedNum,x => x.FeeSchedNum);
			comboCobRule.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumCobRule)).Length;i++) {
				comboCobRule.Items.Add(Lan.g("enumEnumCobRule",Enum.GetNames(typeof(EnumCobRule))[i]));
			}
			comboCobRule.SelectedIndex=(int)_insPlanCur.CobRule;			
			long selectedFilingCodeNum=_insPlanCur.FilingCode;
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
			comboBillType.SetSelectedDefNum(_insPlanCur.BillingType); 
			comboExclusionFeeRule.Items.Clear();
			Enum.GetValues(typeof(ExclusionRule)).Cast<ExclusionRule>().ForEach(x => comboExclusionFeeRule.Items.Add(x.GetDescription()));
			comboExclusionFeeRule.SelectedIndex=(int)_insPlanCur.ExclusionFeeRule;
			FillCarrier(_insPlanCur.CarrierNum);
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
			textPlanNote.Text=_insPlanCur.PlanNote;
			if(_insPlanCur.DentaideCardSequence==0){
				textDentaide.Text="";
			}
			else{
				textDentaide.Text=_insPlanCur.DentaideCardSequence.ToString();
			}
			textPlanFlag.Text=_insPlanCur.CanadianPlanFlag;
			textCanadianDiagCode.Text=_insPlanCur.CanadianDiagnosticCode;
			textCanadianInstCode.Text=_insPlanCur.CanadianInstitutionCode;
			checkDontVerify.Checked=_insPlanCur.HideFromVerifyList;
			checkUseBlueBook.Checked=_insPlanCur.IsBlueBookEnabled;
			InsVerify insVerifyBenefitsCur=InsVerifies.GetOneByFKey(_insPlanCur.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefitsCur!=null && insVerifyBenefitsCur.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefitsCur.DateLastVerified.ToShortDateString();
				_dateTimeInsPlanLastVerified=PIn.Date(textDateLastVerifiedBenefits.Text);
			}
			//if(PlanCur.BenefitNotes==""){
			//	butBenefitNotes.Enabled=false;
			//}
			Cursor=Cursors.Default;
		}

		private List<FeeSched> GetFilteredCopayFeeSched(List<FeeSched> listFeeSchedCopays) {
			if(_insPlanTypeComboItemSelected==InsPlanTypeComboItem.PPOFixedBenefit) {
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
			if(_insSubCur!=null){
				excludeSub=_insSubCur.InsSubNum;
			}
			//Even though this sub hasn't been updated to the database, this still works because SubCur.InsSubNum is valid and won't change.
			int countSubs=InsSubs.GetSubscriberCountForPlan(_insPlanCur.PlanNum,excludeSub!=-1);
			textLinkedNum.Text=countSubs.ToString();
			if(countSubs>10000) {//10,000 per Nathan.
				comboLinked.Visible=false;
				butOtherSubscribers.Visible=true;
				butOtherSubscribers.Location=comboLinked.Location;
			}
			else {
				comboLinked.Visible=true;
				butOtherSubscribers.Visible=false;
				List<string> listStringSubs=InsSubs.GetSubscribersForPlan(_insPlanCur.PlanNum,excludeSub);
				comboLinked.Items.Clear();
				comboLinked.Items.AddList(listStringSubs.ToArray());
				if(listStringSubs.Count>0){
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
			grid.Columns.Clear();
			grid.Columns.Add(new GridColumn(Lan.g(this,"Name"),20){ IsWidthDynamic=true });
			grid.ListGridRows.Clear();
			long excludeSub=-1;
			if(_insSubCur!=null){
				excludeSub=_insSubCur.InsSubNum;
			}
			List<string> listSubs=InsSubs.GetSubscribersForPlan(_insPlanCur.PlanNum,excludeSub);
			foreach(string subName in listSubs) {
				grid.ListGridRows.Add(new GridRow(subName));
			}
			grid.EndUpdate();
			form.ShowDialog();
		}
		
		private void FillPatientAdjustments() {
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patPlanCur.PatNum);
			_arrayListAdj=new ArrayList();//move selected claimprocs into ALAdj
			for(int i=0;i<listClaimProcs.Count;i++) {
				if(listClaimProcs[i].InsSubNum==_insSubCur.InsSubNum
					&& listClaimProcs[i].Status==ClaimProcStatus.Adjustment) {
					_arrayListAdj.Add(listClaimProcs[i]);
				}
			}
			listAdj.Items.Clear();
			string s;
			for(int i=0;i<_arrayListAdj.Count;i++) {
				s=((ClaimProc)_arrayListAdj[i]).ProcDate.ToShortDateString()+"       Ins Used:  "
					+((ClaimProc)_arrayListAdj[i]).InsPayAmt.ToString("F")+"       Ded Used:  "
					+((ClaimProc)_arrayListAdj[i]).DedApplied.ToString("F");
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

		private string GetSecurityLogMessage(Carrier carrierNew,Carrier carrierOld) {
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine("Carrier changes:");
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.CarrierName,carrierOld.CarrierName,"name"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.Phone,carrierOld.Phone,"phone"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.Address,carrierOld.Address,"address"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.Address2,carrierOld.Address2,"address2"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.City,carrierOld.City,"city"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.State,carrierOld.State,"state"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.Zip,carrierOld.Zip,"zip"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.ElectID,carrierOld.ElectID,"electID"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.NoSendElect.ToString(),carrierOld.NoSendElect.ToString(),"send electronically"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.CobInsPaidBehaviorOverride.ToString(),carrierOld.CobInsPaidBehaviorOverride.ToString(),"send paid by other insurance"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.EraAutomationOverride.ToString(),carrierOld.EraAutomationOverride.ToString(),"ERA automation"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.OrthoInsPayConsolidate.ToString(),carrierOld.OrthoInsPayConsolidate.ToString(),"consolidate ortho ins payments"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.CarrierGroupName.ToString(),carrierOld.CarrierGroupName.ToString(),"carrier group"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.IsHidden.ToString(),carrierOld.IsHidden.ToString(),"is hidden"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.TrustedEtransFlags.ToString(),carrierOld.TrustedEtransFlags.ToString(),"is trusted"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.IsCDA.ToString(),carrierOld.IsCDA.ToString(),"is CDA"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.ApptTextBackColor.ToString(),carrierOld.ApptTextBackColor.ToString(),"text back color"));
			stringBuilder.Append(SecurityLogMessageHelper(carrierNew.IsCoinsuranceInverted.ToString(),carrierOld.IsCoinsuranceInverted.ToString(),"import benefit coinsurance inverted"));
			return stringBuilder.ToString();
		}

		private string SecurityLogMessageHelper(string newVal,string oldVal,string colVal) {
			if(oldVal!=newVal) {
				return $"Carrier {colVal} changed from '{oldVal}' to '{newVal}'\r\n";
			}
			return "";
		}

		///<summary>Only called from FillCarrier and textElectID_Validating. Fills comboElectIDdescript as appropriate.</summary>
		private void FillPayor() {
			//textElectIDdescript.Text=ElectIDs.GetDescript(textElectID.Text);
			comboElectIDdescript.Items.Clear();
			string[] stringArrayPayorNames=ElectIDs.GetDescripts(textElectID.Text);
			if(stringArrayPayorNames.Length>1) {
				comboElectIDdescript.Items.Add("multiple payors use this ID");
			}
			for(int i=0;i<stringArrayPayorNames.Length;i++) {
				comboElectIDdescript.Items.Add(stringArrayPayorNames[i]);
			}
			if(stringArrayPayorNames.Length>0) {
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
			if((_insPlanCur.PlanType=="" || _insPlanCur.PlanType=="p")
				&& ListTools.In(comboPlanType.SelectedIndex,(int)InsPlanTypeComboItem.MedicaidOrFlatCopay,(int)InsPlanTypeComboItem.Capitation)) 
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear all percentages. Continue?")) {
					comboPlanType.SelectedIndex=(int)_insPlanTypeComboItemSelected;//Undo the selection change.
					return;
				}
				//Loop through the list backwards so i will be valid.
				for(int i=_listBenefit.Count-1;i>=0;i--) {
					if(((Benefit)_listBenefit[i]).BenefitType==InsBenefitType.CoInsurance) {
						_listBenefit.RemoveAt(i);
					}
				}
				//benefitList=new ArrayList();
				FillBenefits();
			}
			else if(comboPlanType.SelectedIndex==(int)InsPlanTypeComboItem.PPOFixedBenefit) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will set all percentages to 100%. Continue?")) {
					comboPlanType.SelectedIndex=(int)_insPlanTypeComboItemSelected;//Undo the selection change.
					return;
				}
				foreach(Benefit benefit in _listBenefit) {
					if(benefit.BenefitType==InsBenefitType.CoInsurance) {
						benefit.Percent=100;
					}
				}
				FillBenefits();
			}
			InsPlanTypeComboItem InsPlanTypeComboItemPrevSelection=_insPlanTypeComboItemSelected;
			_insPlanTypeComboItemSelected=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			switch(_insPlanTypeComboItemSelected) {
				case InsPlanTypeComboItem.CategoryPercentage:
					_insPlanCur.PlanType="";
					if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook) {
						checkUseBlueBook.Visible=true; //Only show checkbox if Blue Book is enabled.
					}
					break;
				case InsPlanTypeComboItem.PPO:
				case InsPlanTypeComboItem.PPOFixedBenefit:
					_insPlanCur.PlanType="p";
					checkUseBlueBook.Visible=false;
					break;
				case InsPlanTypeComboItem.MedicaidOrFlatCopay:
					_insPlanCur.PlanType="f";
					checkUseBlueBook.Visible=false;
					break;
				case InsPlanTypeComboItem.Capitation:
					_insPlanCur.PlanType="c";
					checkUseBlueBook.Visible=false;
					break;
				default:
					break;
			}
			SetAllowedFeeScheduleControls();
			if(PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {//otherwise, no automation on this field.
				if(_insPlanCur.PlanType=="") {
					checkClaimsUseUCR.Checked=true;
				}
				else {
					checkClaimsUseUCR.Checked=false;
				}
			}
			if(InsPlanTypeComboItemPrevSelection!=_insPlanTypeComboItemSelected//Selection has actually changed
				&& (InsPlanTypeComboItemPrevSelection==InsPlanTypeComboItem.PPOFixedBenefit || _insPlanTypeComboItemSelected==InsPlanTypeComboItem.PPOFixedBenefit))//Is or was Fixed Benefit
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
			bool isFixedBenefitPlanType=(_insPlanTypeComboItemSelected==InsPlanTypeComboItem.PPOFixedBenefit);
			return(isFixedBenefitPlanType!=isFixedBenefitSched);
		}

		private void FillComboCoPay() {
			List<FeeSched> listFilteredCopayFeeSched=GetFilteredCopayFeeSched(_listFeeSchedsCopay);
			comboCopay.Items.Clear();
			comboCopay.Items.AddNone<FeeSched>();
			comboCopay.Items.AddList(listFilteredCopayFeeSched,x=>x.Description);
			comboCopay.SetSelectedKey<FeeSched>(_insPlanCur.CopayFeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
		}

		private void butAdjAdd_Click(object sender,System.EventArgs e) {
			ClaimProc ClaimProcCur=ClaimProcs.CreateInsPlanAdjustment(_patPlanCur.PatNum,_insPlanCur.PlanNum,_insSubCur.InsSubNum);
			using FormInsAdj formInsAdj=new FormInsAdj(ClaimProcCur);
			formInsAdj.IsNew=true;
			formInsAdj.ShowDialog();
			FillPatientAdjustments();
		}

		private void butHistory_Click(object sender,EventArgs e) {
			using FormInsHistSetup formInsHistSetup=new FormInsHistSetup(_patPlanCur.PatNum,_insSubCur);
			if(formInsHistSetup.ShowDialog()==DialogResult.Cancel) {
				return;
			}
			_didAddInsHistCP=true;
		}

		private void listAdj_DoubleClick(object sender,System.EventArgs e) {
			if(listAdj.SelectedIndex==-1) {
				return;
			}
			using FormInsAdj formInsAdj=new FormInsAdj((ClaimProc)_arrayListAdj[listAdj.SelectedIndex]);
			formInsAdj.ShowDialog();
			FillPatientAdjustments();
		}

		///<summary>Button not visible if SubCur=null, editing from big list.</summary>
		private void butPick_Click(object sender,EventArgs e) {
			if(!IsNewPlan && !Security.IsAuthorized(Permissions.InsPlanPickListExisting,true)) {
				MsgBox.Show(this,"Permission required: 'Change existing Ins Plan using Pick From List'.\r\n"
					+"Alternatively, the Ins Plan can be dropped and a new plan may be added.");
				return;
			}
			using FormInsPlans formInsPlans=new FormInsPlans();
			formInsPlans.empText=textEmployer.Text;
			formInsPlans.carrierText=textCarrier.Text;
			formInsPlans.IsSelectMode=true;
			formInsPlans.ShowDialog();
			if(formInsPlans.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(!IsNewPlan && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to use the selected plan?  You should NOT use this if the patient is changing insurance.  Use the Drop button instead.")) {
				return;
			}
			if(formInsPlans.SelectedPlan.PlanNum==0) {//user clicked Blank
				_insPlanCur=new InsPlan();
				_insPlanCur.PlanNum=_insPlanOld.PlanNum;
			}
			else {//user selected an existing plan
				_insPlanCur=formInsPlans.SelectedPlan;
				textInsPlanNum.Text=formInsPlans.SelectedPlan.PlanNum.ToString();
			}
			FillFormWithPlanCur(true);
			//We need to pass patPlanNum in to RefreshForPlan to get patient level benefits:
			long patPlanNum=0;
			if(_patPlanCur!=null){
				patPlanNum=_patPlanCur.PatPlanNum;
			}
			if(formInsPlans.SelectedPlan.PlanNum==0){//user clicked blank
				_listBenefit=new List<Benefit>();
			}
			else {//user selected an existing plan
				_listBenefit=Benefits.RefreshForPlan(_insPlanCur.PlanNum,patPlanNum);
			}
			FillBenefits();
			if(IsNewPlan || formInsPlans.SelectedPlan.PlanNum==0) {//New plan or user clicked blank.
				//Leave benefitListOld alone so that it will trigger deletion of the orphaned benefits later.
			}
			else{
				//Replace benefitListOld so that we only cause changes to be save that are made after this point.
				_listBenefitOld=new List<Benefit>();
				for(int i=0;i<_listBenefit.Count;i++) {
					_listBenefitOld.Add(_listBenefit[i].Copy());
				}
			}
			//benefitListOld=new List<Benefit>(benefitList);//this was not the proper way to make a shallow copy.
			_planCurOriginal=_insPlanCur.Copy();
			FillOtherSubscribers();
			FillOrtho();
			//PlanNumOriginal is NOT reset here.
			//It's now similar to if we'd just opened a new form, except for SubCur still needs to be changed.
		}

		private void textEmployer_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return) {
				listBoxEmps.Visible=false;
				textGroupName.Focus();
				return;
			}
			if(textEmployer.Text=="") {
				listBoxEmps.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listBoxEmps.Items.Count==0) {
					return;
				}
				if(listBoxEmps.SelectedIndex==-1) {
					listBoxEmps.SelectedIndex=0;
					textEmployer.Text=listBoxEmps.SelectedItem.ToString();
				}
				else if(listBoxEmps.SelectedIndex==listBoxEmps.Items.Count-1) {
					listBoxEmps.SelectedIndex=-1;
					textEmployer.Text=_stringEmpOriginal;
				}
				else {
					listBoxEmps.SelectedIndex++;
					textEmployer.Text=listBoxEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listBoxEmps.Items.Count==0) {
					return;
				}
				if(listBoxEmps.SelectedIndex==-1) {
					listBoxEmps.SelectedIndex=listBoxEmps.Items.Count-1;
					textEmployer.Text=listBoxEmps.SelectedItem.ToString();
				}
				else if(listBoxEmps.SelectedIndex==0) {
					listBoxEmps.SelectedIndex=-1;
					textEmployer.Text=_stringEmpOriginal;
				}
				else {
					listBoxEmps.SelectedIndex--;
					textEmployer.Text=listBoxEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(textEmployer.Text.Length==1) {
				textEmployer.Text=textEmployer.Text.ToUpper();
				textEmployer.SelectionStart=1;
			}
			_stringEmpOriginal=textEmployer.Text;//the original text is preserved when using up and down arrows
			listBoxEmps.Items.Clear();
			List<Employer> listEmployersSimilar=Employers.GetSimilarNames(textEmployer.Text);
			for(int i=0;i<listEmployersSimilar.Count;i++) {
				listBoxEmps.Items.Add(listEmployersSimilar[i].EmpName);
			}
			int h=13*listEmployersSimilar.Count+5;
			if(h > ClientSize.Height-listBoxEmps.Top){
				h=ClientSize.Height-listBoxEmps.Top;
			}
			listBoxEmps.Size=new Size(231,h);
			listBoxEmps.Visible=true;
		}

		private void textEmployer_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListEmps) {
				return;
			}
			listBoxEmps.Visible=false;
		}

		private void listBoxEmps_Click(object sender,System.EventArgs e) {
			if(listBoxEmps.SelectedItem==null) {
				return;
			}
			textEmployer.Text=listBoxEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listBoxEmps.Visible=false;
		}

		private void listBoxEmps_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
			if(listBoxEmps.SelectedIndex==-1) {
				return;
			}
			textEmployer.Text=listBoxEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			listBoxEmps.Visible=false;
		}

		private void listBoxEmps_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListEmps=true;
		}

		private void listBoxEmps_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListEmps=false;
		}

		private void butPickCarrier_Click(object sender,EventArgs e) {
			using FormCarriers formCarriers=new FormCarriers();
			formCarriers.IsSelectMode=true;
			formCarriers.ShowDialog();
			if(formCarriers.DialogResult!=DialogResult.OK) {
				return;
			}
			FillCarrier(formCarriers.CarrierSelected.CarrierNum);
		}

		private void textCarrier_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Return) {
				if(listBoxCarriers.SelectedIndex==-1) {
					textPhone.Focus();
				}
				else {
					FillCarrier(_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierNum);
					textCarrier.Focus();
					textCarrier.SelectionStart=textCarrier.Text.Length;
				}
				listBoxCarriers.Visible=false;
				return;
			}
			if(textCarrier.Text=="") {
				listBoxCarriers.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(listBoxCarriers.Items.Count==0) {
					return;
				}
				if(listBoxCarriers.SelectedIndex==-1) {
					listBoxCarriers.SelectedIndex=0;
					textCarrier.Text=_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierName;
				}
				else if(listBoxCarriers.SelectedIndex==listBoxCarriers.Items.Count-1) {
					listBoxCarriers.SelectedIndex=-1;
					textCarrier.Text=_carrierOriginal;
				}
				else {
					listBoxCarriers.SelectedIndex++;
					textCarrier.Text=_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(listBoxCarriers.Items.Count==0) {
					return;
				}
				if(listBoxCarriers.SelectedIndex==-1) {
					listBoxCarriers.SelectedIndex=listBoxCarriers.Items.Count-1;
					textCarrier.Text=_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierName;
				}
				else if(listBoxCarriers.SelectedIndex==0) {
					listBoxCarriers.SelectedIndex=-1;
					textCarrier.Text=_carrierOriginal;
				}
				else {
					listBoxCarriers.SelectedIndex--;
					textCarrier.Text=_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(textCarrier.Text.Length==1) {
				textCarrier.Text=textCarrier.Text.ToUpper();
				textCarrier.SelectionStart=1;
			}
			_carrierOriginal=textCarrier.Text;//the original text is preserved when using up and down arrows
			listBoxCarriers.Items.Clear();
			_listCarriersSimilar=Carriers.GetSimilarNames(textCarrier.Text);
			for(int i=0;i<_listCarriersSimilar.Count;i++) {
				listBoxCarriers.Items.Add(_listCarriersSimilar[i].CarrierName+", "
					+_listCarriersSimilar[i].Phone+", "
					+_listCarriersSimilar[i].Address+", "
					+_listCarriersSimilar[i].Address2+", "
					+_listCarriersSimilar[i].City+", "
					+_listCarriersSimilar[i].State+", "
					+_listCarriersSimilar[i].Zip);
			}
			int h=13*_listCarriersSimilar.Count+5;
			if(h > ClientSize.Height-listBoxCarriers.Top){
				h=ClientSize.Height-listBoxCarriers.Top;
			}
			listBoxCarriers.Size=new Size(listBoxCarriers.Width,h);
			listBoxCarriers.Visible=true;
		}

		private void textCarrier_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListCarriers) {
				return;
			}
			//or if user clicked on a different text box.
			if(listBoxCarriers.SelectedIndex!=-1) {
				FillCarrier(_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierNum);
			}
			listBoxCarriers.Visible=false;
		}

		private void listBoxCarriers_Click(object sender,System.EventArgs e) {
			if(listBoxCarriers.SelectedIndex==-1) {
				return;
			}
			FillCarrier(_listCarriersSimilar[listBoxCarriers.SelectedIndex].CarrierNum);
			textCarrier.Focus();
			textCarrier.SelectionStart=textCarrier.Text.Length;
			listBoxCarriers.Visible=false;
		}

		private void listBoxCarriers_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
		}

		private void listBoxCarriers_MouseEnter(object sender,System.EventArgs e) {
			_isMouseInListCarriers=true;
		}

		private void listBoxCarriers_MouseLeave(object sender,System.EventArgs e) {
			_isMouseInListCarriers=false;
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
			string[] StringArrayElectIDs=ElectIDs.GetDescripts(textElectID.Text);
			if(StringArrayElectIDs.Length==0) {//if none found in the predefined list
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
				InsSubs.ValidateNoKeys(_insSubCur.InsSubNum,false);
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change subscriber?  This should not normally be needed.")) {
					return;
				}
			}
			catch(Exception ex){
				if(PrefC.GetBool(PrefName.SubscriberAllowChangeAlways)) {
					DialogResult dialogResult=MessageBox.Show(Lan.g(this,"Warning!  Do not change unless fixing database corruption.  ")+"\r\n"+ex.Message);
					if(dialogResult!=DialogResult.OK) {
						return;
					}
				}
				else {
					MessageBox.Show(Lan.g(this,"Not allowed to change.")+"\r\n"+ex.Message);
					return;
				}
			}
			Family family=Patients.GetFamily(_insSubCur.Subscriber);
			using FormFamilyMemberSelect formFamilyMemberSelect=new FormFamilyMemberSelect(family,true);
			formFamilyMemberSelect.ShowDialog();
			if(formFamilyMemberSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_insSubCur.Subscriber=formFamilyMemberSelect.SelectedPatNum;
			Patient patientSubscriber=Patients.GetLim(formFamilyMemberSelect.SelectedPatNum);
			textSubscriber.Text=patientSubscriber.GetNameLF();
			textSubscriberID.Text="";
		}

		private void CheckAssign_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign)) {
				checkAssign.Checked=!checkAssign.Checked;
			}
		}

		private void butSearch_Click(object sender,System.EventArgs e) {
			using FormElectIDs formElectIDs=new FormElectIDs();
			formElectIDs.IsSelectMode=true;
			formElectIDs.ShowDialog();
			if(formElectIDs.DialogResult!=DialogResult.OK) {
				return;
			}
			textElectID.Text=formElectIDs.ElectIDSelected.PayorID;
			_electIdCur=textElectID.Text;
			FillPayor();
			//textElectIDdescript.Text=FormE.selectedID.CarrierName;
		}

		private void FillComboFilingSubtype(long selectedFilingCode) {
			comboFilingCodeSubtype.Items.Clear();
			List<InsFilingCodeSubtype> listInsFilingCodeSubtype=InsFilingCodeSubtypes.GetForInsFilingCode(selectedFilingCode);
			for(int i=0;i<listInsFilingCodeSubtype.Count;i++) {
				comboFilingCodeSubtype.Items.Add(listInsFilingCodeSubtype[i].Descript,listInsFilingCodeSubtype[i]);
				if(_insPlanCur.FilingCodeSubtype==listInsFilingCodeSubtype[i].InsFilingCodeSubtypeNum) {
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
			_insPlanCur.BillingType=comboBillType.GetSelectedDefNum();
		}

		private void comboExclusionFeeRule_SelectionChangeCommitted(object sender,EventArgs e) {
			_insPlanCur.ExclusionFeeRule=(ExclusionRule)comboExclusionFeeRule.SelectedIndex;
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
				RegistryKey registryKey=Registry.LocalMachine.OpenSubKey("Software\\TROJAN BENEFIT SERVICE");
				if(registryKey==null) {//dmg Unix OS will exit here.
					MessageBox.Show("Trojan not installed properly.");
					return;
				}
				//C:\ETW
				if(registryKey.GetValue("INSTALLDIR")==null) {
					MessageBox.Show(@"Registry entry is missing and should be added manually.  LocalMachine\Software\TROJAN BENEFIT SERVICE. StringValue.  Name='INSTALLDIR',	value= path where the Trojan program is located.  Full path to directory, without trailing slash.");
					return;
				}
				file=ODFileUtils.CombinePaths(registryKey.GetValue("INSTALLDIR").ToString(),"Planout.txt");
			}
			if(!File.Exists(file)) {
				MessageBox.Show(file+" not found.  You should export from Trojan first.");
				return;
			}
			TrojanObject trojanObject=Trojan.ProcessTextToObject(File.ReadAllText(file));
			textTrojanID.Text=trojanObject.TROJANID;
			textEmployer.Text=trojanObject.ENAME;
			textGroupName.Text=trojanObject.PLANDESC;
			textPhone.Text=trojanObject.ELIGPHONE;
			textGroupNum.Text=trojanObject.POLICYNO;
			//checkNoSendElect.Checked=!troj.ECLAIMS;//Ignore this.  Even if Trojan says paper, most offices still send by clearinghouse.
			textElectID.Text=trojanObject.PAYERID;
			_electIdCur=textElectID.Text;
			textCarrier.Text=trojanObject.MAILTO;
			textAddress.Text=trojanObject.MAILTOST;
			textCity.Text=trojanObject.MAILCITYONLY;
			textState.Text=trojanObject.MAILSTATEONLY;
			textZip.Text=trojanObject.MAILZIPONLY;
			_insPlanCur.MonthRenew=(byte)trojanObject.MonthRenewal;
			if(_insSubCur.BenefitNotes!="") {
				_insSubCur.BenefitNotes+="\r\n--------------------------------\r\n";
			}
			_insSubCur.BenefitNotes+=trojanObject.BenefitNotes;
			if(trojanObject.PlanNote!=""){
				if(textPlanNote.Text=="") {
					textPlanNote.Text=trojanObject.PlanNote;
				}
				else {//must let user pick final note
					string[] stringArrayNote=new string[2];
					stringArrayNote[0]=textPlanNote.Text;
					stringArrayNote[1]=trojanObject.PlanNote;
					using FormNotePick formNotePick=new FormNotePick(stringArrayNote);
					formNotePick.UseTrojanImportDescription=true;
					formNotePick.ShowDialog();
					if(formNotePick.DialogResult==DialogResult.OK) {
						textPlanNote.Text=formNotePick.SelectedNote;
					}
				}
			}
			//clear exising benefits from screen, not db:
			List<Benefit> listBenefitsToKeep=new List<Benefit>();
			//Go through all current benefits, keep all limitation or age limit benefits.
			foreach(Benefit benefit in _listBenefit) {
				if(Benefits.IsBitewingFrequency(benefit) 
					|| Benefits.IsCancerScreeningFrequency(benefit)
					|| Benefits.IsCrownFrequency(benefit)
					|| Benefits.IsDenturesFrequency(benefit)
					|| Benefits.IsExamFrequency(benefit)
					|| Benefits.IsFlourideAgeLimit(benefit)
					|| Benefits.IsFlourideFrequency(benefit)
					|| Benefits.IsFullDebridementFrequency(benefit)
					|| Benefits.IsImplantFrequency(benefit)
					|| Benefits.IsPanoFrequency(benefit)
					|| Benefits.IsPerioMaintFrequency(benefit)
					|| Benefits.IsProphyFrequency(benefit)
					|| Benefits.IsSealantAgeLimit(benefit)
					|| Benefits.IsSealantFrequency(benefit)
					|| Benefits.IsSRPFrequency(benefit))
				{
					listBenefitsToKeep.Add(benefit);
				}
			}
			_listBenefit=new List<Benefit>();
			_listBenefit.AddRange(listBenefitsToKeep);
			for(int i=0;i<trojanObject.BenefitList.Count;i++){
				//if(fields[2]=="Anniversary year") {
				//	usesAnnivers=true;
				//	MessageBox.Show("Warning.  Plan uses Anniversary year rather than Calendar year.  Please verify the Plan Start Date.");
				//}
				trojanObject.BenefitList[i].PlanNum=_insPlanCur.PlanNum;
				_listBenefit.Add(trojanObject.BenefitList[i].Copy());
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
			using FormIap formIap=new FormIap();
			formIap.ShowDialog();
			if(formIap.DialogResult==DialogResult.Cancel) {
				return;
			}
			Benefit benefit;
			//clear exising benefits from screen, not db:
			_listBenefit=new List<Benefit>();
			string plan=formIap.selectedPlan;
			string field=null;
			string[] stringArraySplitField;//if a field is a sentence with more than one word, we can split it for analysis
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
							if(_insSubCur.BenefitNotes!="") {
								_insSubCur.BenefitNotes+="\r\n";
							}
							_insSubCur.BenefitNotes+="Employer: "+field;
							textEmployer.Text=field;
							break;
						case Iap.Phone:
							_insSubCur.BenefitNotes+="\r\n"+"Phone: "+field;
							break;
						case Iap.InsUnder:
							_insSubCur.BenefitNotes+="\r\n"+"InsUnder: "+field;
							break;
						case Iap.Carrier:
							_insSubCur.BenefitNotes+="\r\n"+"Carrier: "+field;
							textCarrier.Text=field;
							break;
						case Iap.CarrierPh:
							_insSubCur.BenefitNotes+="\r\n"+"CarrierPh: "+field;
							textPhone.Text=field;
							break;
						case Iap.Group://seems to be used as groupnum
							_insSubCur.BenefitNotes+="\r\n"+"Group: "+field;
							textGroupNum.Text=field;
							break;
						case Iap.MailTo://the carrier name again
							_insSubCur.BenefitNotes+="\r\n"+"MailTo: "+field;
							break;
						case Iap.MailTo2://address
							_insSubCur.BenefitNotes+="\r\n"+"MailTo2: "+field;
							textAddress.Text=field;
							break;
						case Iap.MailTo3://address2
							_insSubCur.BenefitNotes+="\r\n"+"MailTo3: "+field;
							textAddress2.Text=field;
							break;
						case Iap.EClaims:
							_insSubCur.BenefitNotes+="\r\n"+"EClaims: "+field;//this contains the PayorID at the end, but also a bunch of other drivel.
							int payorIDloc=field.LastIndexOf("Payor ID#:");
							if(payorIDloc!=-1 && field.Length>payorIDloc+10) {
								textElectID.Text=field.Substring(payorIDloc+10);
								_electIdCur=textElectID.Text;
							}
							break;
						case Iap.FAXClaims:
							_insSubCur.BenefitNotes+="\r\n"+"FAXClaims: "+field;
							break;
						case Iap.DMOOption:
							_insSubCur.BenefitNotes+="\r\n"+"DMOOption: "+field;
							break;
						case Iap.Medical:
							_insSubCur.BenefitNotes+="\r\n"+"Medical: "+field;
							break;
						case Iap.GroupNum://not used.  They seem to use the group field instead
							_insSubCur.BenefitNotes+="\r\n"+"GroupNum: "+field;
							break;
						case Iap.Phone2://?
							_insSubCur.BenefitNotes+="\r\n"+"Phone2: "+field;
							break;
						case Iap.Deductible:
							_insSubCur.BenefitNotes+="\r\n"+"Deductible: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Deductible;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								benefit.PlanNum=_insPlanCur.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefit.Add(benefit.Copy());
							}
							break;
						case Iap.FamilyDed:
							_insSubCur.BenefitNotes+="\r\n"+"FamilyDed: "+field;
							break;
						case Iap.Maximum:
							_insSubCur.BenefitNotes+="\r\n"+"Maximum: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Limitations;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								benefit.PlanNum=_insPlanCur.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefit.Add(benefit.Copy());
							}
							break;
						case Iap.BenefitYear://text is too complex to parse
							_insSubCur.BenefitNotes+="\r\n"+"BenefitYear: "+field;
							break;
						case Iap.DependentAge://too complex to parse
							_insSubCur.BenefitNotes+="\r\n"+"DependentAge: "+field;
							break;
						case Iap.Preventive:
							_insSubCur.BenefitNotes+="\r\n"+"Preventive: "+field;
							stringArraySplitField=field.Split(new char[] { ' ' });
							if(stringArraySplitField.Length==0 || !stringArraySplitField[0].EndsWith("%")) {
								break;
							}
							stringArraySplitField[0]=stringArraySplitField[0].Remove(stringArraySplitField[0].Length-1,1);//remove %
							percent=PIn.Int(stringArraySplitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							break;
						case Iap.Basic:
							_insSubCur.BenefitNotes+="\r\n"+"Basic: "+field;
							stringArraySplitField=field.Split(new char[] { ' ' });
							if(stringArraySplitField.Length==0 || !stringArraySplitField[0].EndsWith("%")) {
								break;
							}
							stringArraySplitField[0]=stringArraySplitField[0].Remove(stringArraySplitField[0].Length-1,1);//remove %
							percent=PIn.Int(stringArraySplitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							break;
						case Iap.Major:
							_insSubCur.BenefitNotes+="\r\n"+"Major: "+field;
							stringArraySplitField=field.Split(new char[] { ' ' });
							if(stringArraySplitField.Length==0 || !stringArraySplitField[0].EndsWith("%")) {
								break;
							}
							stringArraySplitField[0]=stringArraySplitField[0].Remove(stringArraySplitField[0].Length-1,1);//remove %
							percent=PIn.Int(stringArraySplitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;//includes crowns?
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							break;
						case Iap.InitialPlacement:
							_insSubCur.BenefitNotes+="\r\n"+"InitialPlacement: "+field;
							break;
						case Iap.ExtractionClause:
							_insSubCur.BenefitNotes+="\r\n"+"ExtractionClause: "+field;
							break;
						case Iap.Replacement:
							_insSubCur.BenefitNotes+="\r\n"+"Replacement: "+field;
							break;
						case Iap.Other:
							_insSubCur.BenefitNotes+="\r\n"+"Other: "+field;
							break;
						case Iap.Orthodontics:
							_insSubCur.BenefitNotes+="\r\n"+"Orthodontics: "+field;
							stringArraySplitField=field.Split(new char[] { ' ' });
							if(stringArraySplitField.Length==0 || !stringArraySplitField[0].EndsWith("%")) {
								break;
							}
							stringArraySplitField[0]=stringArraySplitField[0].Remove(stringArraySplitField[0].Length-1,1);//remove %
							percent=PIn.Int(stringArraySplitField[0]);
							if(percent<0 || percent>100) {
								break;
							}
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
							benefit.PlanNum=_insPlanCur.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefit.Add(benefit.Copy());
							break;
						case Iap.Deductible2:
							_insSubCur.BenefitNotes+="\r\n"+"Deductible2: "+field;
							break;
						case Iap.Maximum2://ortho Max
							_insSubCur.BenefitNotes+="\r\n"+"Maximum2: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Limitations;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
								benefit.PlanNum=_insPlanCur.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefit.Add(benefit.Copy());
							}
							break;
						case Iap.PymtSchedule:
							_insSubCur.BenefitNotes+="\r\n"+"PymtSchedule: "+field;
							break;
						case Iap.AgeLimit:
							_insSubCur.BenefitNotes+="\r\n"+"AgeLimit: "+field;
							break;
						case Iap.SignatureonFile:
							_insSubCur.BenefitNotes+="\r\n"+"SignatureonFile: "+field;
							break;
						case Iap.StandardADAForm:
							_insSubCur.BenefitNotes+="\r\n"+"StandardADAForm: "+field;
							break;
						case Iap.CoordinationRule:
							_insSubCur.BenefitNotes+="\r\n"+"CoordinationRule: "+field;
							break;
						case Iap.CoordinationCOB:
							_insSubCur.BenefitNotes+="\r\n"+"CoordinationCOB: "+field;
							break;
						case Iap.NightguardsforBruxism:
							_insSubCur.BenefitNotes+="\r\n"+"NightguardsforBruxism: "+field;
							break;
						case Iap.OcclusalAdjustments:
							_insSubCur.BenefitNotes+="\r\n"+"OcclusalAdjustments: "+field;
							break;
						case Iap.XXXXXX:
							_insSubCur.BenefitNotes+="\r\n"+"XXXXXX: "+field;
							break;
						case Iap.TMJNonSurgical:
							_insSubCur.BenefitNotes+="\r\n"+"TMJNonSurgical: "+field;
							break;
						case Iap.Implants:
							_insSubCur.BenefitNotes+="\r\n"+"Implants: "+field;
							break;
						case Iap.InfectionControl:
							_insSubCur.BenefitNotes+="\r\n"+"InfectionControl: "+field;
							break;
						case Iap.Cleanings:
							_insSubCur.BenefitNotes+="\r\n"+"Cleanings: "+field;
							break;
						case Iap.OralEvaluation:
							_insSubCur.BenefitNotes+="\r\n"+"OralEvaluation: "+field;
							break;
						case Iap.Fluoride1200s:
							_insSubCur.BenefitNotes+="\r\n"+"Fluoride1200s: "+field;
							break;
						case Iap.Code0220:
							_insSubCur.BenefitNotes+="\r\n"+"Code0220: "+field;
							break;
						case Iap.Code0272_0274:
							_insSubCur.BenefitNotes+="\r\n"+"Code0272_0274: "+field;
							break;
						case Iap.Code0210:
							_insSubCur.BenefitNotes+="\r\n"+"Code0210: "+field;
							break;
						case Iap.Code0330:
							_insSubCur.BenefitNotes+="\r\n"+"Code0330: "+field;
							break;
						case Iap.SpaceMaintainers:
							_insSubCur.BenefitNotes+="\r\n"+"SpaceMaintainers: "+field;
							break;
						case Iap.EmergencyExams:
							_insSubCur.BenefitNotes+="\r\n"+"EmergencyExams: "+field;
							break;
						case Iap.EmergencyTreatment:
							_insSubCur.BenefitNotes+="\r\n"+"EmergencyTreatment: "+field;
							break;
						case Iap.Sealants1351:
							_insSubCur.BenefitNotes+="\r\n"+"Sealants1351: "+field;
							break;
						case Iap.Fillings2100:
							_insSubCur.BenefitNotes+="\r\n"+"Fillings2100: "+field;
							break;
						case Iap.Extractions:
							_insSubCur.BenefitNotes+="\r\n"+"Extractions: "+field;
							break;
						case Iap.RootCanals:
							_insSubCur.BenefitNotes+="\r\n"+"RootCanals: "+field;
							break;
						case Iap.MolarRootCanal:
							_insSubCur.BenefitNotes+="\r\n"+"MolarRootCanal: "+field;
							break;
						case Iap.OralSurgery:
							_insSubCur.BenefitNotes+="\r\n"+"OralSurgery: "+field;
							break;
						case Iap.ImpactionSoftTissue:
							_insSubCur.BenefitNotes+="\r\n"+"ImpactionSoftTissue: "+field;
							break;
						case Iap.ImpactionPartialBony:
							_insSubCur.BenefitNotes+="\r\n"+"ImpactionPartialBony: "+field;
							break;
						case Iap.ImpactionCompleteBony:
							_insSubCur.BenefitNotes+="\r\n"+"ImpactionCompleteBony: "+field;
							break;
						case Iap.SurgicalProceduresGeneral:
							_insSubCur.BenefitNotes+="\r\n"+"SurgicalProceduresGeneral: "+field;
							break;
						case Iap.PerioSurgicalPerioOsseous:
							_insSubCur.BenefitNotes+="\r\n"+"PerioSurgicalPerioOsseous: "+field;
							break;
						case Iap.SurgicalPerioOther:
							_insSubCur.BenefitNotes+="\r\n"+"SurgicalPerioOther: "+field;
							break;
						case Iap.RootPlaning:
							_insSubCur.BenefitNotes+="\r\n"+"RootPlaning: "+field;
							break;
						case Iap.Scaling4345:
							_insSubCur.BenefitNotes+="\r\n"+"Scaling4345: "+field;
							break;
						case Iap.PerioPx:
							_insSubCur.BenefitNotes+="\r\n"+"PerioPx: "+field;
							break;
						case Iap.PerioComment:
							_insSubCur.BenefitNotes+="\r\n"+"PerioComment: "+field;
							break;
						case Iap.IVSedation:
							_insSubCur.BenefitNotes+="\r\n"+"IVSedation: "+field;
							break;
						case Iap.General9220:
							_insSubCur.BenefitNotes+="\r\n"+"General9220: "+field;
							break;
						case Iap.Relines5700s:
							_insSubCur.BenefitNotes+="\r\n"+"Relines5700s: "+field;
							break;
						case Iap.StainlessSteelCrowns:
							_insSubCur.BenefitNotes+="\r\n"+"StainlessSteelCrowns: "+field;
							break;
						case Iap.Crowns2700s:
							_insSubCur.BenefitNotes+="\r\n"+"Crowns2700s: "+field;
							break;
						case Iap.Bridges6200:
							_insSubCur.BenefitNotes+="\r\n"+"Bridges6200: "+field;
							break;
						case Iap.Partials5200s:
							_insSubCur.BenefitNotes+="\r\n"+"Partials5200s: "+field;
							break;
						case Iap.Dentures5100s:
							_insSubCur.BenefitNotes+="\r\n"+"Dentures5100s: "+field;
							break;
						case Iap.EmpNumberXXX:
							_insSubCur.BenefitNotes+="\r\n"+"EmpNumberXXX: "+field;
							break;
						case Iap.DateXXX:
							_insSubCur.BenefitNotes+="\r\n"+"DateXXX: "+field;
							break;
						case Iap.Line4://city state
							_insSubCur.BenefitNotes+="\r\n"+"Line4: "+field;
							field=field.Replace("  "," ");//get rid of double space before zip
							stringArraySplitField=field.Split(new char[] { ' ' });
							if(stringArraySplitField.Length<3) {
								break;
							}
							textCity.Text=stringArraySplitField[0].Replace(",","");//gets rid of the comma on the end of city
							textState.Text=stringArraySplitField[1];
							textZip.Text=stringArraySplitField[2];
							break;
						case Iap.Note:
							_insSubCur.BenefitNotes+="\r\n"+"Note: "+field;
							break;
						case Iap.Plan://?
							_insSubCur.BenefitNotes+="\r\n"+"Plan: "+field;
							break;
						case Iap.BuildUps:
							_insSubCur.BenefitNotes+="\r\n"+"BuildUps: "+field;
							break;
						case Iap.PosteriorComposites:
							_insSubCur.BenefitNotes+="\r\n"+"PosteriorComposites: "+field;
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
			Carrier carrier=Carriers.GetCarrier(_insPlanCur.CarrierNum);
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
			DateTime dateTime=DateTime.Today;
			if(ODBuild.IsDebug()) {
				dateTime=new DateTime(1999,1,4);//TODO: Remove after Canadian claim certification is complete.
			}
			Relat relat=(Relat)comboRelationship.SelectedIndex;
			string patID=textPatID.Text;
			try {
				CanadianOutput.SendElegibility(clearinghouseClin,_patPlanCur.PatNum,_insPlanCur,dateTime,relat,patID,true,_insSubCur,false,FormCCDPrint.PrintCCD);
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
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlanCur.PlanNum);
			if(dateTimeLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateTimeLast270.ToShortDateString();
			}
		}

		///<summary>This button is only visible if Trojan or IAP is enabled.  Always active.  Button not visible if SubCur==null.</summary>
		private void butBenefitNotes_Click(object sender,System.EventArgs e) {
			string otherBenefitNote="";
			if(_insSubCur.BenefitNotes=="") {
				//try to find some other similar notes. Never includes the current subscriber.
				//List<long> samePlans=InsPlans.GetPlanNumsOfSamePlans(textEmployer.Text,textGroupName.Text,textGroupNum.Text,
				//	textDivisionNo.Text,textCarrier.Text,checkIsMedical.Checked,PlanCur.PlanNum,false);
				otherBenefitNote=InsSubs.GetBenefitNotes(_insPlanCur.PlanNum,_insSubCur.InsSubNum);
				if(otherBenefitNote=="") {
					MsgBox.Show(this,"No benefit note found.  Benefit notes are created when importing Trojan or IAP benefit information and are frequently read-only.  Store your own notes in the subscriber note instead.");
					return;
				}
				MsgBox.Show(this,"This plan does not have a benefit note, but a note was found for another subsriber of this plan.  You will be able to view this note, but not change it.");
			}
			using FormInsBenefitNotes formInsBenefitNotes=new FormInsBenefitNotes();
			if(_insSubCur.BenefitNotes!="") {
				formInsBenefitNotes.BenefitNotes=_insSubCur.BenefitNotes;
			}
			else {
				formInsBenefitNotes.BenefitNotes=otherBenefitNote;
			}
			formInsBenefitNotes.ShowDialog();
			if(formInsBenefitNotes.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(_insSubCur.BenefitNotes!="") {
				_insSubCur.BenefitNotes=formInsBenefitNotes.BenefitNotes;
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
				if(_insSubCur==null) {//viewing from big list
					MsgBox.Show(this,"Subscribers must be removed individually before deleting plan.");//by dropping, then using this same delete button.
					return;
				}
				else {//Came into here through a patient.
					DateTime dateTimeSubChange=_insSubCur.SecDateTEdit;
					if(_patPlanCur!=null) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All patients attached to this subscription will be dropped and the subscription for this plan will be deleted. Continue?")) {
							return;
						}
					}
					//drop the plan


					//detach subscriber.
					try {
						InsSubs.Delete(_insSubCur.InsSubNum);//Checks dependencies first;  If none, deletes the inssub, claimprocs, patplans, and recomputes all estimates.
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					logText=Lan.g(this,"The subscriber")+" "+Patients.GetPat(_insSubCur.Subscriber).GetNameFLnoPref()+" "
						+Lan.g(this,"with the Subscriber ID")+" "+_insSubCur.SubscriberID+" "+Lan.g(this,"was deleted.");
					_hasDeleted=true;
					//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null)?0:_patPlanCur.PatNum,logText,(_insPlanCur==null)?0:_insPlanCur.PlanNum,
						_insPlanCur.SecDateTEdit);
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
			DateTime dateTimePrevious=_insPlanCur.SecDateTEdit;
			try {
				InsPlans.Delete(_insPlanCur);//Checks dependencies first;  If none, deletes insplan, inssub, benefits, claimprocs, patplans, and recomputes all estimates.
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_insPlanCur.CarrierNum).CarrierName+" "+Lan.g(this,"was deleted.");
			_hasDeleted=true;
			//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null)?0:_patPlanCur.PatNum,logText,(_insPlanCur==null)?0:_insPlanCur.PlanNum,
				dateTimePrevious);
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
			List<Claim> listClaims=Claims.Refresh(_patPlanCur.PatNum);
			for(int j=0;j<listClaims.Count;j++) {
				if(listClaims[j].PlanNum!=_insPlanCur.PlanNum) {//different insplan
					continue;
				}
				if(listClaims[j].DateService!=DateTime.Today) {//not today
					continue;
				}
				//Patient currently has a claim for the insplan they are trying to drop
				MsgBox.Show(this,"Please delete all of today's claims for this patient before dropping this plan.");
				return false;
			}
			PatPlans.Delete(_patPlanCur.PatPlanNum);//Estimates recomputed within Delete()
			//PlanCur.ComputeEstimatesForCur();
			_hasDropped=true;
			string logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_insPlanCur.CarrierNum).CarrierName+" "+Lan.g(this,"was dropped.");
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null)?0:_patPlanCur.PatNum,logText,(_insPlanCur==null)?0:_insPlanCur.PlanNum,
				_insPlanCur.SecDateTEdit);
			InsEditPatLogs.MakeLogEntry(null,_patPlanCur,InsEditPatLogType.PatPlan);
			DialogResult=DialogResult.OK;
			return true;
		}

		private void butLabel_Click(object sender,System.EventArgs e) {//TODO: Implement ODprintout pattern
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
			_listBenefit.Sort();
			gridBenefits.BeginUpdate();
			gridBenefits.Columns.Clear();
			GridColumn col=new GridColumn("Pat",28);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Level",60);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Type",70);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Category",72);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("%",30);//,HorizontalAlignment.Right);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Amt",40);//,HorizontalAlignment.Right);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Time Period",80);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Quantity",115);
			gridBenefits.Columns.Add(col);
			gridBenefits.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listBenefit.Count;i++) {
				row=new GridRow();
				if(_listBenefit[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(_listBenefit[i].CoverageLevel==BenefitCoverageLevel.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",_listBenefit[i].CoverageLevel.ToString()));
				}
				if(_listBenefit[i].BenefitType==InsBenefitType.CoInsurance && _listBenefit[i].Percent != -1) {
					row.Cells.Add("%");
				}
				else if(_listBenefit[i].BenefitType==InsBenefitType.WaitingPeriod) {
					row.Cells.Add(Lan.g(this,"Waiting Period"));
				}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",_listBenefit[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(_listBenefit[i])); //already translated
				if(_listBenefit[i].Percent==-1 ) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listBenefit[i].Percent.ToString());
				}
				if(_listBenefit[i].MonetaryAmt == -1) {
					//if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.Deductible) {
					//	row.Cells.Add(((Benefit)benefitList[i]).MonetaryAmt.ToString("n0"));
					//}
					//else {
					row.Cells.Add("");
					//}
				}
				else {
					row.Cells.Add(_listBenefit[i].MonetaryAmt.ToString("n0"));
				}
				if(_listBenefit[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",_listBenefit[i].TimePeriod.ToString()));
				}
				if(_listBenefit[i].Quantity>0) {
					if(_listBenefit[i].QuantityQualifier==BenefitQuantity.NumberOfServices
						&&(_listBenefit[i].TimePeriod==BenefitTimePeriod.ServiceYear
						|| _listBenefit[i].TimePeriod==BenefitTimePeriod.CalendarYear))
					{
						row.Cells.Add(_listBenefit[i].Quantity.ToString()+" "+Lan.g(this,"times per year")+" ");
					}
					else if(_listBenefit[i].QuantityQualifier==BenefitQuantity.NumberOfServices 
						&& _listBenefit[i].TimePeriod==BenefitTimePeriod.NumberInLast12Months) 
					{
						row.Cells.Add(_listBenefit[i].Quantity.ToString()+" "+Lan.g(this,"times in the last 12 months")+" ");
					}
					else {
						row.Cells.Add(_listBenefit[i].Quantity.ToString()+" "
							+Lan.g("enumBenefitQuantity",_listBenefit[i].QuantityQualifier.ToString()));
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
			if(IsNewPlan && _insPlanCur.PlanNum != _insPlanOld.PlanNum) {  //If adding a new plan and picked existing plan from list
				//==Travis 05/06/2015:  Allowing users to edit insurance benefits for new plans that were picked from the list was causing problems with 
				//	duplicating benefits.  This was the fix we decided to go with, as the issue didn't seem to be affecting existing plans for a patient.
				MessageBox.Show(Lan.g(this,"You have picked an existing insurance plan and changes cannot be made to benefits until you have saved the plan for this new subscriber.")
					+"\r\n"+Lan.g(this,"To edit, click OK and then open the edit insurance plan window again."));
				return;
			}
			long patPlanNum=0;
			if(_patPlanCur!=null) {
				patPlanNum=_patPlanCur.PatPlanNum;
			}
			using FormInsBenefits formInsBenefits=new FormInsBenefits(_insPlanCur.PlanNum,patPlanNum);
			formInsBenefits.OriginalBenList=_listBenefit;
			formInsBenefits.Note=textSubscNote.Text;
			formInsBenefits.MonthRenew=_insPlanCur.MonthRenew;
			formInsBenefits.SubCur=_insSubCur;
			formInsBenefits.ShowDialog();
			if(formInsBenefits.DialogResult!=DialogResult.OK) {
				return;
			}
			FillBenefits();
			textSubscNote.Text=formInsBenefits.Note;
			_insPlanCur.MonthRenew=formInsBenefits.MonthRenew;
		}

		///<summary>Gets an employerNum based on the name entered. Called from FillCur</summary>
		private void GetEmployerNum() {
			if(_insPlanCur.EmployerNum==0) {//no employer was previously entered.
				if(textEmployer.Text=="") {
					//no change - Use what's in the database if they truly didn't change anything (PlanCur has no emp, text is blank, and text was always blank, they didn't switch insplans)
					if(_patPlanCur!=null && _employerNameOrig=="" && _insPlanCur.PlanNum==_planCurOriginal.PlanNum) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_insPlanCur.EmployerNum=insPlanDB.EmployerNum;
						_planCurOriginal.EmployerNum=insPlanDB.EmployerNum;
					}
				}
				else {
					_insPlanCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
			else {//an employer was previously entered
				if(textEmployer.Text=="") {
					_insPlanCur.EmployerNum=0;
				}
				//if text has changed - 
				else if(_employerNameOrig!=textEmployer.Text) {
					_insPlanCur.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
				else {
					//no change - Use what's in the database
					if(_patPlanCur!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_insPlanCur.EmployerNum=insPlanDB.EmployerNum;
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
				object[] objArrayParameters={error};
				Plugins.HookAddCode(this,"FormInsPlan.butGetElectronic_Click_270NotSupportedError",objArrayParameters);
				error=(string)objArrayParameters[0];
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
				Etrans etrans=x270Controller.RequestBenefits(clearinghouseClin,_insPlanCur,_patPlanCur.PatNum,_carrierCur,_insSubCur,out error);
				if(etrans != null) {
					//show the user a list of benefits to pick from for import--------------------------
					bool isDependentRequest=(_patPlanCur.PatNum!=_insSubCur.Subscriber);
					Carrier carrierCur=Carriers.GetCarrier(_insPlanCur.CarrierNum);
					using FormEtrans270Edit formEtrans270Edit=new FormEtrans270Edit(_patPlanCur.PatPlanNum,_insPlanCur.PlanNum,_insSubCur.InsSubNum,isDependentRequest,_insSubCur.Subscriber,carrierCur.IsCoinsuranceInverted);
					formEtrans270Edit.EtransCur=etrans;
					formEtrans270Edit.IsInitialResponse=true;
					formEtrans270Edit.ListBenefits=_listBenefit;
					if(formEtrans270Edit.ShowDialog()==DialogResult.OK) {
						EB271.SetInsuranceHistoryDates(formEtrans270Edit.ListEB271sImported,_patPlanCur.PatNum,_insSubCur);
						#region Plan Notes
						string patName=Patients.GetNameLF(_patPlanCur.PatNum);
						DateTime dateTimePlanEnd=DateTime.MinValue;
						List<DTP271> listDTP271Dates=formEtrans270Edit.ListDTP271s;
						foreach(DTP271 date in listDTP271Dates) {
							string dtpDateStr=DTP271.GetDateStr(date.Segment.Get(2),date.Segment.Get(3));
							if(date.Segment.Get(1)=="347") {//347 => Plan End
								dateTimePlanEnd=X12Parse.ToDate(date.Segment.Get(3));
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
							&& dateTimePlanEnd.Year > 1900 && dateTimePlanEnd < DateTime.Today
							&& MsgBox.Show(this,MsgBoxButtons.YesNo,"The plan has ended.  Would you like to drop this plan?"))
						{
							if(DropClickHelper()
								&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to add a popup to collect new insurance information from patient?"))
							{
								Popup popup=new Popup();
								popup.PatNum=_patPlanCur.PatNum;
								popup.PopupLevel=EnumPopupLevel.Patient;
								popup.IsNew=true;
								popup.Description=Lan.g(this,"Insurance expired.  Collect new insurance information.");
								using FormPopupEdit formPopupEdit=new FormPopupEdit();
								formPopupEdit.PopupCur=popup;
								formPopupEdit.ShowDialog();
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
					using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(ex.Message);
					msgBoxCopyPaste.ShowDialog();
				}
			}
			Cursor=Cursors.Default;
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlanCur.PlanNum);
			if(dateTimeLast270.Year<1880) {
				textElectBenLastDate.Text="";
			}
			else {
				textElectBenLastDate.Text=dateTimeLast270.ToShortDateString();
			}
			FillBenefits();
		}

		private void butHistoryElect_Click(object sender,EventArgs e) {
			//button not visible if SubCur is null
			using FormBenefitElectHistory formBenefitElectHistory=new FormBenefitElectHistory(_insPlanCur.PlanNum,_patPlanCur.PatPlanNum,_insSubCur.InsSubNum,_insSubCur.Subscriber,_insPlanCur.CarrierNum);
			formBenefitElectHistory.ListBenefits=_listBenefit;
			formBenefitElectHistory.ShowDialog();
			DateTime dateLast270=Etranss.GetLastDate270(_insPlanCur.PlanNum);
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
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNodeElig = xmlDocument.CreateNode(XmlNodeType.Element,"EligRequest","");
			xmlDocument.AppendChild(xmlNodeElig);
			// Prepare Namespace Attribute
			XmlAttribute xmlAttributeNameSpace = xmlDocument.CreateAttribute("xmlns","xsi","http://www.w3.org/2000/xmlns/");
			xmlAttributeNameSpace.Value = "http://www.w3.org/2001/XMLSchema-instance";
			xmlDocument.DocumentElement.SetAttributeNode(xmlAttributeNameSpace);
			// Prepare noNamespace Schema Location Attribute
			XmlAttribute xmlAttributeNoNameSpaceSchemaLocation = xmlDocument.CreateAttribute("xsi","noNamespaceSchemaLocation","http://www.w3.org/2001/XMLSchema-instance");
			//dmg Not sure what this is for. This path will not exist on Unix and will fail. In fact, this path
			//will either not exist or be read-only on most Windows boxes, so this path specification is probably
			//a bug, but has not caused any user complaints thus far.
			xmlAttributeNoNameSpaceSchemaLocation.Value = @"D:\eligreq.xsd";
			xmlDocument.DocumentElement.SetAttributeNode(xmlAttributeNoNameSpaceSchemaLocation);
			//  Prepare AuthInfo Node
			XmlNode xmlNodeAuthInfo = xmlDocument.CreateNode(XmlNodeType.Element,"AuthInfo","");
			//  Create UserName / Password ChildNode for AuthInfoNode
			XmlNode xmlNodeUserName = xmlDocument.CreateNode(XmlNodeType.Element,"UserName","");
			XmlNode xmlNodePassword = xmlDocument.CreateNode(XmlNodeType.Element,"Password","");
			//  Set Value of UserID / Password
			xmlNodeUserName.InnerText = loginID;
			xmlNodePassword.InnerText = passWord;
			//  Append UserName / Password to AuthInfoNode
			xmlNodeAuthInfo.AppendChild(xmlNodeUserName);
			xmlNodeAuthInfo.AppendChild(xmlNodePassword);
			//  Append AuthInfoNode To EligNode
			xmlNodeElig.AppendChild(xmlNodeAuthInfo);
			//  Prepare Information Receiver Node
			XmlNode xmlNodeInfoReceiver = xmlDocument.CreateNode(XmlNodeType.Element,"InformationReceiver","");
			XmlNode xmlNodeInfoAddress = xmlDocument.CreateNode(XmlNodeType.Element,"Address","");
			XmlNode xmlNodeInfoAddressName = xmlDocument.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode xmlNodeInfoAddressFirstName = xmlDocument.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode xmlNodeInfoAddressLastName = xmlDocument.CreateNode(XmlNodeType.Element,"LastName","");
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
			xmlNodeInfoAddressFirstName.InnerText = infoReceiverLastName;
			xmlNodeInfoAddressLastName.InnerText = infoReceiverFirstName;
			xmlNodeInfoAddressName.AppendChild(xmlNodeInfoAddressFirstName);
			xmlNodeInfoAddressName.AppendChild(xmlNodeInfoAddressLastName);
			XmlNode xmlNodeInfoAddressLine1 = xmlDocument.CreateNode(XmlNodeType.Element,"AddressLine1","");
			XmlNode xmlNodeInfoAddressLine2 = xmlDocument.CreateNode(XmlNodeType.Element,"AddressLine2","");
			XmlNode xmlNodeInfoPhone = xmlDocument.CreateNode(XmlNodeType.Element,"Phone","");
			XmlNode xmlNodeInfoCity = xmlDocument.CreateNode(XmlNodeType.Element,"City","");
			XmlNode xmlNodeInfoState = xmlDocument.CreateNode(XmlNodeType.Element,"State","");
			XmlNode xmlNodeInfoZip = xmlDocument.CreateNode(XmlNodeType.Element,"Zip","");
			//  Populate Practioner demographic from hash table
			practiceAddress1 = PrefC.GetString(PrefName.PracticeAddress);
			practiceAddress2 = PrefC.GetString(PrefName.PracticeAddress2);
			// Format Phone
			if(PrefC.GetString(PrefName.PracticePhone).Length == 10 && TelephoneNumbers.IsFormattingAllowed) {
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
			xmlNodeInfoAddressLine1.InnerText = practiceAddress1;
			xmlNodeInfoAddressLine2.InnerText = practiceAddress2;
			xmlNodeInfoPhone.InnerText = practicePhone;
			xmlNodeInfoCity.InnerText = practiceCity;
			xmlNodeInfoState.InnerText = practiceState;
			xmlNodeInfoZip.InnerText = practiceZip;
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoAddressName);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoAddressLine1);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoAddressLine2);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoPhone);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoCity);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoState);
			xmlNodeInfoAddress.AppendChild(xmlNodeInfoZip);
			xmlNodeInfoReceiver.AppendChild(xmlNodeInfoAddress);
			//SPK / AAD 8/13/08 Add NPI -- Begin
			XmlNode xmlNodeInfoReceiverProviderNPI = xmlDocument.CreateNode(XmlNodeType.Element,"NPI","");
			//Get Provider NPI #
			table = Providers.GetDefaultPracticeProvider3();
			if(table.Rows.Count != 0) {
				xmlNodeInfoReceiverProviderNPI.InnerText = PIn.String(table.Rows[0][0].ToString());
			};
			xmlNodeInfoReceiver.AppendChild(xmlNodeInfoReceiverProviderNPI);
			//SPK / AAD 8/13/08 Add NPI -- End
			XmlNode xmlNodeInfoCredential = xmlDocument.CreateNode(XmlNodeType.Element,"Credential","");
			XmlNode xmlNodeInfoCredentialType = xmlDocument.CreateNode(XmlNodeType.Element,"Type","");
			XmlNode xmlNodeInfoCredentialValue = xmlDocument.CreateNode(XmlNodeType.Element,"Value","");
			xmlNodeInfoCredentialType.InnerText = "TJ";
			xmlNodeInfoCredentialValue.InnerText = "123456789";
			xmlNodeInfoCredential.AppendChild(xmlNodeInfoCredentialType);
			xmlNodeInfoCredential.AppendChild(xmlNodeInfoCredentialValue);
			xmlNodeInfoReceiver.AppendChild(xmlNodeInfoCredential);
			XmlNode xmlNodeInfoTaxonomyCode = xmlDocument.CreateNode(XmlNodeType.Element,"TaxonomyCode","");
			xmlNodeInfoTaxonomyCode.InnerText = TaxoCode;
			xmlNodeInfoReceiver.AppendChild(xmlNodeInfoTaxonomyCode);
			//  Append InfoReceiver To EligNode
			xmlNodeElig.AppendChild(xmlNodeInfoReceiver);
			//  Payer Info
			XmlNode xmlNodeInfoPayer = xmlDocument.CreateNode(XmlNodeType.Element,"Payer","");
			XmlNode xmlNodeInfoPayerNEIC = xmlDocument.CreateNode(XmlNodeType.Element,"PayerNEIC","");
			xmlNodeInfoPayerNEIC.InnerText = textElectID.Text;
			xmlNodeInfoPayer.AppendChild(xmlNodeInfoPayerNEIC);
			xmlNodeElig.AppendChild(xmlNodeInfoPayer);
			//  Patient
			XmlNode xmlNodePatient = xmlDocument.CreateNode(XmlNodeType.Element,"Patient","");
			XmlNode xmlNodePatientName = xmlDocument.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode xmlNodePatientFirstName = xmlDocument.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode xmlNodePatientLastName = xmlDocument.CreateNode(XmlNodeType.Element,"LastName","");
			XmlNode xmlNodePatientDOB = xmlDocument.CreateNode(XmlNodeType.Element,"DOB","");
			XmlNode xmlNodePatientSubscriber = xmlDocument.CreateNode(XmlNodeType.Element,"SubscriberID","");
			XmlNode xmlNodePatientRelationship = xmlDocument.CreateNode(XmlNodeType.Element,"RelationshipCode","");
			XmlNode xmlNodePatientGender = xmlDocument.CreateNode(XmlNodeType.Element,"Gender","");
			// Read Patient FName,LName,DOB, and Gender from Patient Table
			table = Patients.GetPartialPatientData(_patPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				xmlNodePatientFirstName.InnerText = PIn.String(table.Rows[0][0].ToString());
				xmlNodePatientLastName.InnerText = PIn.String(table.Rows[0][1].ToString());
				xmlNodePatientDOB.InnerText = PIn.String(table.Rows[0][2].ToString());
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
				xmlNodePatientFirstName.InnerText = "Unknown";
				xmlNodePatientLastName.InnerText = "Unknown";
				xmlNodePatientDOB.InnerText = "99/99/9999";
				RelationShip = "??";
				GenderCode = "?";
			}
			xmlNodePatientName.AppendChild(xmlNodePatientFirstName);
			xmlNodePatientName.AppendChild(xmlNodePatientLastName);
			xmlNodePatientSubscriber.InnerText = textSubscriberID.Text;
			xmlNodePatientRelationship.InnerText = RelationShip;
			xmlNodePatientGender.InnerText = GenderCode;
			xmlNodePatient.AppendChild(xmlNodePatientName);
			xmlNodePatient.AppendChild(xmlNodePatientDOB);
			xmlNodePatient.AppendChild(xmlNodePatientSubscriber);
			xmlNodePatient.AppendChild(xmlNodePatientRelationship);
			xmlNodePatient.AppendChild(xmlNodePatientGender);
			xmlNodeElig.AppendChild(xmlNodePatient);
			//  Subscriber
			XmlNode xmlNodeSubscriber = xmlDocument.CreateNode(XmlNodeType.Element,"Subscriber","");
			XmlNode xmlNodeSubscriberName = xmlDocument.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode xmlNodeSubscriberFirstName = xmlDocument.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode xmlNodeSubscriberLastName = xmlDocument.CreateNode(XmlNodeType.Element,"LastName","");
			XmlNode xmlNodeSubscriberDOB = xmlDocument.CreateNode(XmlNodeType.Element,"DOB","");
			XmlNode xmlNodeSubscriberSubscriber = xmlDocument.CreateNode(XmlNodeType.Element,"SubscriberID","");
			XmlNode xmlNodeSubscriberRelationship = xmlDocument.CreateNode(XmlNodeType.Element,"RelationshipCode","");
			XmlNode xmlNodeSubscriberGender = xmlDocument.CreateNode(XmlNodeType.Element,"Gender","");
			// Read Subscriber FName,LName,DOB, and Gender from Patient Table
			table=Patients.GetPartialPatientData2(_patPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				xmlNodeSubscriberFirstName.InnerText = PIn.String(table.Rows[0][0].ToString());
				xmlNodeSubscriberLastName.InnerText = PIn.String(table.Rows[0][1].ToString());
				xmlNodeSubscriberDOB.InnerText = PIn.String(table.Rows[0][2].ToString());
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
				xmlNodeSubscriberFirstName.InnerText = "Unknown";
				xmlNodeSubscriberLastName.InnerText = "Unknown";
				xmlNodeSubscriberDOB.InnerText = "99/99/9999";
				GenderCode = "?";
			}
			xmlNodeSubscriberName.AppendChild(xmlNodeSubscriberFirstName);
			xmlNodeSubscriberName.AppendChild(xmlNodeSubscriberLastName);
			xmlNodeSubscriberSubscriber.InnerText = textSubscriberID.Text;
			xmlNodeSubscriberRelationship.InnerText = RelationShip;
			xmlNodeSubscriberGender.InnerText = GenderCode;
			xmlNodeSubscriber.AppendChild(xmlNodeSubscriberName);
			xmlNodeSubscriber.AppendChild(xmlNodeSubscriberDOB);
			xmlNodeSubscriber.AppendChild(xmlNodeSubscriberSubscriber);
			xmlNodeSubscriber.AppendChild(xmlNodeSubscriberRelationship);
			xmlNodeSubscriber.AppendChild(xmlNodeSubscriberGender);
			xmlNodeElig.AppendChild(xmlNodeSubscriber);
			//  Prepare Information Receiver Node
			XmlNode xmlNodeRenderingProvider = xmlDocument.CreateNode(XmlNodeType.Element,"RenderingProvider","");
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- Start
			XmlNode xmlNodeRenderingProviderNPI = xmlDocument.CreateNode(XmlNodeType.Element,"NPI","");
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- End
			XmlNode xmlNodeRenderingAddress = xmlDocument.CreateNode(XmlNodeType.Element,"Address","");
			XmlNode xmlNodeRenderingAddressName = xmlDocument.CreateNode(XmlNodeType.Element,"Name","");
			XmlNode xmlNodeRenderingAddressFirstName = xmlDocument.CreateNode(XmlNodeType.Element,"FirstName","");
			XmlNode xmlNodeRenderingAddressLastName = xmlDocument.CreateNode(XmlNodeType.Element,"LastName","");
			// Get Rendering Provider first and lastname
			// Read Patient FName,LName,DOB, and Gender from Patient Table
			table=Providers.GetPrimaryProviders(_patPlanCur.PatNum);
			if(table.Rows.Count != 0) {
				renderingProviderFirstName = PIn.String(table.Rows[0][0].ToString());
				renderingProviderLastName = PIn.String(table.Rows[0][1].ToString());
			}
			else {
				renderingProviderFirstName = infoReceiverFirstName;
				renderingProviderLastName = infoReceiverLastName;
			};
			xmlNodeRenderingAddressFirstName.InnerText = renderingProviderFirstName;
			xmlNodeRenderingAddressLastName.InnerText = renderingProviderLastName;
			xmlNodeRenderingAddressName.AppendChild(xmlNodeRenderingAddressFirstName);
			xmlNodeRenderingAddressName.AppendChild(xmlNodeRenderingAddressLastName);
			XmlNode xmlNodeRenderingAddressLine1 = xmlDocument.CreateNode(XmlNodeType.Element,"AddressLine1","");
			XmlNode xmlNodeRenderingAddressLine2 = xmlDocument.CreateNode(XmlNodeType.Element,"AddressLine2","");
			XmlNode xmlNodeRenderingPhone = xmlDocument.CreateNode(XmlNodeType.Element,"Phone","");
			XmlNode xmlNodeRenderingCity = xmlDocument.CreateNode(XmlNodeType.Element,"City","");
			XmlNode xmlNodeRenderingState = xmlDocument.CreateNode(XmlNodeType.Element,"State","");
			XmlNode xmlNodeRenderingZip = xmlDocument.CreateNode(XmlNodeType.Element,"Zip","");
			xmlNodeRenderingProviderNPI.InnerText = xmlNodeInfoReceiverProviderNPI.InnerText;
			xmlNodeRenderingAddressLine1.InnerText = practiceAddress1;
			xmlNodeRenderingAddressLine2.InnerText = practiceAddress2;
			xmlNodeRenderingPhone.InnerText = practicePhone;
			xmlNodeRenderingCity.InnerText = practiceCity;
			xmlNodeRenderingState.InnerText = practiceState;
			xmlNodeRenderingZip.InnerText = practiceZip;
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingAddressName);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingAddressLine1);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingAddressLine2);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingPhone);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingCity);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingState);
			xmlNodeRenderingAddress.AppendChild(xmlNodeRenderingZip);
			XmlNode xmlNodeRenderingCredential = xmlDocument.CreateNode(XmlNodeType.Element,"Credential","");
			XmlNode xmlNodeRenderingCredentialType = xmlDocument.CreateNode(XmlNodeType.Element,"Type","");
			XmlNode xmlNodeRenderingCredentialValue = xmlDocument.CreateNode(XmlNodeType.Element,"Value","");
			xmlNodeRenderingCredentialType.InnerText = "TJ";
			xmlNodeRenderingCredentialValue.InnerText = "123456789";
			xmlNodeRenderingCredential.AppendChild(xmlNodeRenderingCredentialType);
			xmlNodeRenderingCredential.AppendChild(xmlNodeRenderingCredentialValue);
			XmlNode xmlNodeRenderingTaxonomyCode = xmlDocument.CreateNode(XmlNodeType.Element,"TaxonomyCode","");
			xmlNodeRenderingTaxonomyCode.InnerText = TaxoCode;
			xmlNodeRenderingProvider.AppendChild(xmlNodeRenderingAddress);
			// SPK / AAD 8/13/08 Add Rendering Provider NPI It is same as Info Receiver NPI -- Start
			xmlNodeRenderingProvider.AppendChild(xmlNodeRenderingProviderNPI);
			// SPK / AAD 8/13/08 Add NPI -- End
			xmlNodeRenderingProvider.AppendChild(xmlNodeRenderingCredential);
			xmlNodeRenderingProvider.AppendChild(xmlNodeRenderingTaxonomyCode);
			//  Append RenderingProvider To EligNode
			xmlNodeElig.AppendChild(xmlNodeRenderingProvider);
			return xmlDocument.OuterXml;
		}

		private void ProcessEligibilityResponseDentalXchange(string DCIResponse) {
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNodeIsEligible;
			string IsEligibleStatus;
			xmlDocument.LoadXml(DCIResponse);
			xmlNodeIsEligible = xmlDocument.SelectSingleNode("EligBenefitResponse/isEligible");
			switch(xmlNodeIsEligible.InnerText) {
				case "0": // SPK
					// HINA Added 9/2. 
					// Open new form to display complete response Detail
					using(Form formDisplayEligibilityResponse = new FormEligibilityResponseDisplay(xmlDocument,_patPlanCur.PatNum)) {
						formDisplayEligibilityResponse.ShowDialog();
					}
					break;
				case "1": // SPK
					// Process Error code and Message Node AAD
					XmlNode xmlNodeErrorCode;
					XmlNode xmlNodeErrorMessage;
					xmlNodeErrorCode = xmlDocument.SelectSingleNode("EligBenefitResponse/Response/ErrorCode");
					xmlNodeErrorMessage = xmlDocument.SelectSingleNode("EligBenefitResponse/Response/ErrorMsg");
					IsEligibleStatus = textSubscriber.Text + " is Not Eligible. Error Code:";
					IsEligibleStatus += xmlNodeErrorCode.InnerText + " Error Description:" + xmlNodeErrorMessage.InnerText;
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
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
			InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
			InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
			bool hasExistingEmployerChanged=(insPlanDB.CarrierNum!=0 && insPlanDB.EmployerNum!=_insPlanCur.EmployerNum && insPlanDB.PlanNum==_insPlanCur.PlanNum);//not new insplan and employer db not same as selection and insplan still used.
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
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
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
			else if(_insPlanOld.PlanNum!=_insPlanCur.PlanNum) {//Plan was picked from list
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
			if(textSubscriberID.Text=="" && _insSubCur!=null) {
				MsgBox.Show(this,"Subscriber ID not allowed to be blank.");
				return false;
			}
			if(textCarrier.Text=="") {
				MsgBox.Show(this,"Carrier not allowed to be blank.");
				return false;
			}
			if(_patPlanCur!=null && !textOrdinal.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(comboRelationship.SelectedIndex==-1 && comboRelationship.Items.Count>0) {
				MsgBox.Show(this,"Relationship to Subscriber is not allowed to be blank.");
				return false;
			}
			if(_patPlanCur!=null && !IsEmployerValid()) {
				return false;
			}
			if(_patPlanCur!=null && !IsCarrierValid()) {
				return false;
			}
			if(_insSubCur!=null) {
				//Subscriber: Only changed when user clicks change button.
				_insSubCur.SubscriberID=textSubscriberID.Text;
				_insSubCur.DateEffective=PIn.Date(textDateEffect.Text);
				_insSubCur.DateTerm=PIn.Date(textDateTerm.Text);
				_insSubCur.ReleaseInfo=checkRelease.Checked;
				_insSubCur.AssignBen=checkAssign.Checked;
				_insSubCur.SubscNote=textSubscNote.Text;
				//MonthRenew already handled inside benefit window.
			}
			GetEmployerNum();
			_insPlanCur.GroupName=textGroupName.Text;
			_insPlanCur.GroupNum=textGroupNum.Text;
			_insPlanCur.RxBIN=textBIN.Text;
			_insPlanCur.DivisionNo=textDivisionNo.Text;//only visible in Canada
			//carrier-----------------------------------------------------------------------------------------------------
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//User has the ability to edit carrier information.  Check for matches, create new Carrier if applicable.
				Carrier carrierForm=GetCarrierFromForm();
				Carrier carrierOld=_carrierCur.Copy();
				if(_carrierCur.CarrierNum==_carrierNumOrig && Carriers.Compare(carrierForm,_carrierCur) && _insPlanCur.PlanNum==_insPlanOld.PlanNum) {
					//carrier is the same as it was originally, use what's in db if editing a patient's patplan.
					if(_patPlanCur!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_carrierCur=Carriers.GetCarrier(insPlanDB.CarrierNum);
						_planCurOriginal.CarrierNum=_carrierCur.CarrierNum;
					}
					else {
						//Someone could have changed the insplan while the user was editing this window, do not overwrite the other users changes.
						InsPlan insPlanDB=InsPlans.GetPlan(_insPlanCur.PlanNum,null);
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
							using FormCarrierEdit formCarrierEdit=new FormCarrierEdit();
							formCarrierEdit.IsNew=true;
							formCarrierEdit.CarrierCur=_carrierCur;
							formCarrierEdit.ShowDialog();
							if(formCarrierEdit.DialogResult!=DialogResult.OK) {
								return false;
							}
						}
					}
					else {
						_carrierCur=Carriers.GetIdentical(_carrierCur,carrierOld: carrierOld);
					}
				}
				_insPlanCur.CarrierNum=_carrierCur.CarrierNum;
			}
			else {//User does not have permission to edit carrier information.  
				//We don't care if carrier info is changed, only if it's removed.  
				//If it's removed, have them choose another.  If it's simply changed, just use the same prikey.
				if(Carriers.GetCarrier(_carrierCur.CarrierNum).CarrierName=="" && _insPlanCur.PlanNum!=_insPlanOld.PlanNum) {//Carrier not found, it must have been deleted or combined
					MsgBox.Show(this,"Selected carrier has been combined or deleted.  Please choose another insurance plan.");
					return false;
				}
				else if(_insPlanCur.PlanNum==_insPlanOld.PlanNum) {//Didn't switch insplan, they were only viewing.
					long planNumDb=_insPlanOld.PlanNum;
					if(_patPlanCur!=null) {
						//Another user could have edited this patient's plan at the same time and could have changed something about the pat plan so we need
						//to go to the database an make sure that we are "not changing anything" by saving potentially stale data to the db.
						//If we don't do this, then we would end up overriding any changes that other users did while we were in this edit window.
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						planNumDb=insSubDB.PlanNum;
					}
					InsPlan insPlanDB=InsPlans.GetPlan(planNumDb,null);
					_carrierCur=Carriers.GetCarrier(insPlanDB.CarrierNum);
					_planCurOriginal.CarrierNum=_carrierCur.CarrierNum;
					_insPlanCur.CarrierNum=_carrierCur.CarrierNum;
				}
				else { 
					_insPlanCur.CarrierNum=_carrierCur.CarrierNum;
				}
			}
			//plantype already handled.
			if(comboClaimForm.SelectedIndex!=-1){
				_insPlanCur.ClaimFormNum=comboClaimForm.GetSelected<ClaimForm>().ClaimFormNum;
			}
			_insPlanCur.UseAltCode=checkAlternateCode.Checked;
			_insPlanCur.CodeSubstNone=checkCodeSubst.Checked;
			_insPlanCur.HasPpoSubstWriteoffs=checkPpoSubWo.Checked;
			_insPlanCur.IsMedical=checkIsMedical.Checked;
			_insPlanCur.ClaimsUseUCR=checkClaimsUseUCR.Checked;
			_insPlanCur.IsHidden=checkIsHidden.Checked;
			_insPlanCur.ShowBaseUnits=checkShowBaseUnits.Checked;
			if(comboFeeSched.SelectedIndex==0) {
				_insPlanCur.FeeSched=0;
			}
			else if(comboFeeSched.SelectedIndex == -1) {//Hidden fee schedule selected in comboFeeSched
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Fee Schedule has been hidden. Are you sure you want to continue?")) {
					return false;
				}
				_insPlanCur.FeeSched=_planCurOriginal.FeeSched;
			}
			else{
				_insPlanCur.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboCopay.SelectedIndex==0){
				_insPlanCur.CopayFeeSched=0;//none
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
				_insPlanCur.CopayFeeSched=_planCurOriginal.CopayFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_insPlanCur.CopayFeeSched=comboCopay.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboOutOfNetwork.SelectedIndex==0){
				if(IsNewPlan
					&& _insPlanCur.PlanType==""//percentage
					&& PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.LegacyBlueBook){
					//add a fee schedule if needed
					FeeSched feeSched=FeeScheds.GetByExactName(_carrierCur.CarrierName,FeeScheduleType.OutNetwork);
					if(feeSched==null){
						feeSched=new FeeSched();
						feeSched.Description=_carrierCur.CarrierName;
						feeSched.FeeSchedType=FeeScheduleType.OutNetwork;
						//sched.IsNew=true;
						feeSched.IsGlobal=true;
						feeSched.ItemOrder=FeeScheds.GetCount();
						FeeScheds.Insert(feeSched);
						DataValid.SetInvalid(InvalidType.FeeScheds);
					}
					_insPlanCur.AllowedFeeSched=feeSched.FeeSchedNum;
				}
				else{
					_insPlanCur.AllowedFeeSched=0;
				}
			}
			else if(comboOutOfNetwork.SelectedIndex==-1) {//Hidden fee schedule selected in comboAllowedFeeSched
				if(comboOutOfNetwork.Enabled 
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Out Of Network fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_insPlanCur.AllowedFeeSched=_planCurOriginal.AllowedFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_insPlanCur.AllowedFeeSched=comboOutOfNetwork.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboManualBlueBook.SelectedIndex==-1) {
				if(comboManualBlueBook.Enabled
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Manual Blue book fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_insPlanCur.ManualFeeSchedNum=_planCurOriginal.ManualFeeSchedNum;//No change, maintain hidden feesched.
			}
			else {
				_insPlanCur.ManualFeeSchedNum=comboManualBlueBook.GetSelected<FeeSched>().FeeSchedNum;
			}
			_insPlanCur.CobRule=(EnumCobRule)comboCobRule.SelectedIndex;
			//Canadian------------------------------------------------------------------------------------------
			_insPlanCur.DentaideCardSequence=PIn.Byte(textDentaide.Text);
			_insPlanCur.CanadianPlanFlag=textPlanFlag.Text;//validated
			_insPlanCur.CanadianDiagnosticCode=textCanadianDiagCode.Text;//validated
			_insPlanCur.CanadianInstitutionCode=textCanadianInstCode.Text;//validated
			//Canadian end---------------------------------------------------------------------------------------
			_insPlanCur.TrojanID=textTrojanID.Text;
			_insPlanCur.PlanNote=textPlanNote.Text;
			_insPlanCur.HideFromVerifyList=checkDontVerify.Checked;
			//Ortho----------------------------------------------------------------------------------------------
			_insPlanCur.OrthoType=comboOrthoClaimType.SelectedIndex==-1 ? 0 : (OrthoClaimType)Enum.GetValues(typeof(OrthoClaimType)).GetValue(comboOrthoClaimType.SelectedIndex);
			if(_procedureCodeOrthoAuto!=null) {
				_insPlanCur.OrthoAutoProcCodeNumOverride=_procedureCodeOrthoAuto.CodeNum;
			}
			else {
				_insPlanCur.OrthoAutoProcCodeNumOverride=0; //overridden by practice default.
			}
			_insPlanCur.OrthoAutoProcFreq=comboOrthoAutoProcPeriod.SelectedIndex==-1 ? 0 : (OrthoAutoProcFrequency)Enum.GetValues(typeof(OrthoAutoProcFrequency)).GetValue(comboOrthoAutoProcPeriod.SelectedIndex);
			_insPlanCur.OrthoAutoClaimDaysWait=checkOrthoWaitDays.Checked ? 30 : 0;
			_insPlanCur.OrthoAutoFeeBilled=PIn.Double(textOrthoAutoFee.Text);
			return true;
		}

		///<summary>Warns user if there are received claims for this plan.  Returns true if user wants to proceed, or if there are no received claims for this plan.  Returns false if the user aborts.</summary>
		private bool CheckForReceivedClaims() {
			long patNum=0;
			if(_patPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
				patNum=_patPlanCur.PatNum;
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
					claimCount=Claims.GetCountReceived(_planCurOriginal.PlanNum,_patPlanCur.InsSubNum);
					if(claimCount!=0) {
						if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false; //abort
						}
					}
				}
			}
			return true;
		}

		///<summary>Enables/disables Out of network and Manual Blue Book ComboBoxes depending on if the blue book checkbox is visible and checked.</summary>
		private void SetAllowedFeeScheduleControls() {
			if(checkUseBlueBook.Checked && checkUseBlueBook.Visible) {
				comboOutOfNetwork.Enabled=false;
				comboManualBlueBook.Enabled=true;
			}
			else {
				comboOutOfNetwork.Enabled=true;
				comboManualBlueBook.Enabled=false;
			}
		}

		private void checkUseBlueBook_CheckedChanged(object sender,EventArgs e) {
			_insPlanCur.IsBlueBookEnabled=checkUseBlueBook.Checked;
			SetAllowedFeeScheduleControls();
		}

		private void comboFeeSched_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_insPlanCur.PlanType=="" && comboFeeSched.SelectedIndex > 0) {
				checkUseBlueBook.Checked=false; // started with no fee sched, selected one
			}
			if(_insPlanCur.PlanType=="" && comboFeeSched.SelectedIndex <= 0) {
				checkUseBlueBook.Checked=true; // had a fee sched, selected none
			}
		}

		private void butSubstCodes_Click(object sender,EventArgs e) {
			InsPlan insPlanCurCopy=_insPlanCur.Copy();
			insPlanCurCopy.CodeSubstNone=checkCodeSubst.Checked;
			using FormInsPlanSubstitution formInsPlanSubstitution=new FormInsPlanSubstitution(insPlanCurCopy);
			if(formInsPlanSubstitution.ShowDialog()==DialogResult.OK) {
				checkCodeSubst.Checked=insPlanCurCopy.CodeSubstNone;//Since the user can change this flag in the other window.
			}
		}

		private void butVerifyPatPlan_Click(object sender,EventArgs e) {
			textDateLastVerifiedPatPlan.Text=DateTime.Today.ToShortDateString();
		}

		private void butVerifyBenefits_Click(object sender,EventArgs e) {
			textDateLastVerifiedBenefits.Text=DateTime.Today.ToShortDateString();
		}

		private void butAuditPat_Click(object sender,EventArgs e) {
			if(_patPlanCur==null) {
				return;
			}
			using FormInsEditPatLog formInsEditPatLog=new FormInsEditPatLog(_patPlanCur);
			formInsEditPatLog.ShowDialog();
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(IsPatPlanRemoved()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			GetEmployerNum();
			using FormInsEditLogs formInsEditLogs = new FormInsEditLogs(_insPlanCur,_listBenefit);
			formInsEditLogs.ShowDialog();
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
				defaultFee=_insPlanCur.OrthoAutoFeeBilled;
			}
			using FormOrthoPat formOrthoPat = new FormOrthoPat(_patPlanCur,_insPlanCur,carrierName,subID,defaultFee);
			formOrthoPat.ShowDialog();
		}

		private void butPickOrthoProc_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes = new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult == DialogResult.OK) {
				_procedureCodeOrthoAuto=ProcedureCodes.GetProcCode(formProcCodes.SelectedCodeNum);
				textOrthoAutoProc.Text=_procedureCodeOrthoAuto.ProcCode;
			}
		}

		private void butDefaultAutoOrthoProc_Click(object sender,EventArgs e) {
			_procedureCodeOrthoAuto=null;
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
				&& (_insPlanCur==null || InsPlans.GetPlan(_insPlanCur.PlanNum,new List<InsPlan>())==null)) 
			{
				MsgBox.Show(this,"The selected insurance plan was removed by another user and no longer exists.  Open insurance plan again to edit.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_insSubCur!=null && InsPlans.GetPlan(_insSubCur.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"The subscriber's insurance plan was merged by another user and no longer exists.  Open insurance plan again to edit.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(DoShowBluebookDeletionMsgBox()) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,Lan.g(this,"Changing a plan's PlanType from Category Percentage to another type or having 'Use Blue Book' not checked will make the plan ineligible for Blue Book estimates and will delete all Blue Book data for the plan. Would you like to continue?")))
				{
					return;
				}
			}
			long selectedFilingCodeNum=0;
			if(comboFilingCode.GetSelected<InsFilingCode>()!=null) {
				selectedFilingCodeNum=comboFilingCode.GetSelected<InsFilingCode>().InsFilingCodeNum;
			}
			_insPlanCur.FilingCode=selectedFilingCodeNum;
			_insPlanCur.FilingCodeSubtype=0;
			if(comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>()!=null) {
				_insPlanCur.FilingCodeSubtype=comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>().InsFilingCodeSubtypeNum;
			}
			#endregion Validation
			#region 1 - Validate Carrier Received Claims
			try {
			if(!FillPlanCurFromForm()) {//also fills SubCur if not null
				return;
			}
			if(_insPlanCur.CarrierNum!=_planCurOriginal.CarrierNum) {
				long patNum=0;
				if(_patPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
					patNum=_patPlanCur.PatNum;
				}
				string carrierNameOrig=Carriers.GetCarrier(_planCurOriginal.CarrierNum).CarrierName;
				string carrierNameNew=Carriers.GetCarrier(_insPlanCur.CarrierNum).CarrierName;
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
					SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeAssign,_insSubCur.Subscriber,Lan.g(this,"Assignment of Benefits (pay dentist) changed from")
						+" "+(_subOld.AssignBen?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" "
						+Lan.g(this,"to")
						+" "+(checkAssign.Checked?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" for plan "
						+Carriers.GetCarrier(_insPlanCur.CarrierNum).CarrierName);
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
				if(_patPlanCur!=null) {
					if(PIn.Long(textOrdinal.Text)!=_patPlanCur.Ordinal) {//Ordinal changed by user
						_patPlanCur.Ordinal=(byte)(PatPlans.SetOrdinal(_patPlanCur.PatPlanNum,PIn.Int(textOrdinal.Text)));
						_hasOrdinalChanged=true;
					}
					else if(PIn.Long(textOrdinal.Text)!=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum).Ordinal) {
						//PatPlan's ordinal changed by somebody else and not this user, set it to what's in the DB for this update.
						_patPlanCur.Ordinal=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum).Ordinal;
					}
					_patPlanCur.IsPending=checkIsPending.Checked;
					_patPlanCur.Relationship=(Relat)comboRelationship.SelectedIndex;
					_patPlanCur.PatID=textPatID.Text;
					if(_patPlanOld!=null) {
						_patPlanOld.PatID=_patPlanOld.PatID??"";
					}
					PatPlans.Update(_patPlanCur,_patPlanOld);
					if(!PIn.Date(textDateLastVerifiedPatPlan.Text).Date.Equals(_dateTimePatPlanLastVerified.Date)) {
						InsVerify insVerify=InsVerifies.GetOneByFKey(_patPlanCur.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerify!=null) {
							insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedPatPlan.Text);
							InsVerifyHists.InsertFromInsVerify(insVerify);
						}
					}
					if(!IsNewPatPlan) {//Updated
						InsEditPatLogs.MakeLogEntry(_patPlanCur,_patPlanOld,InsEditPatLogType.PatPlan);
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
			if(_insSubCur!=null) {
				_insSubCur.PlanNum=_insPlanCur.PlanNum;
			}
			#endregion 3 - PatPlan
			//Sections 4 - 10 all deal with manipulating the insurance plan so make sure the user has permission to do so.
			#region InsPlan Edit
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				if(_insSubCur==null) {//editing from big list.  No subscriber.  'pick from list' button not visible, making logic easier.
					#region 4 - InsPlan Null Subscriber
					try {
						if(InsPlans.AreEqualValue(_insPlanCur,_planCurOriginal)) {//If no changes
							//pick button doesn't complicate things.  Simply nothing to do.  Also, no SubCur, so just close the form.
							DialogResult=DialogResult.OK;
						}
						else {//changes were made
							InsPlans.Update(_insPlanCur);
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
						if(InsPlans.AreEqualValue(_insPlanCur,_planCurOriginal)) {//New plan, no changes
							#region 5 - InsPlan Non-Null Subscriber, New Plan, No Changes Made
							//If the logic in this region changes, then also change region 5a below.
							try {
								if(_insPlanCur.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button
									//No need to update PlanCur because no changes, delete original plan.
									try {
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(_patPlanCur.PatNum,_insPlanOld.PlanNum,_insPlanCur.PlanNum);
										}
										//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
										InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
										_listBenefitOld=new List<Benefit>();
										removeLogs=true;
									}
									catch(ApplicationException ex) {
										MessageBox.Show(ex.Message);
										//do not need to update PlanCur because no changes were made.
										SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null) ? 0 : _patPlanCur.PatNum
											,Lan.g(this,"FormInsPlan region 5 delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,
											DateTime.MinValue); //new plan, no date needed.
										Close();
										return;
									}
									_insSubCur.PlanNum=_insPlanCur.PlanNum;
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
							if(_insPlanCur.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								//If the logic in this region changes, then also change region 6a below.
								try {
									if(radioChangeAll.Checked) {
										InsPlans.Update(_insPlanCur);//they might not realize that they would be changing an existing plan. Oh well.
										try {
											if(_didAddInsHistCP) {
												//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
												ClaimProcs.UpdatePlanNumForInsHist(_patPlanCur.PatNum,_insPlanOld.PlanNum,_insPlanCur.PlanNum);
											}
											//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
											InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
											_listBenefitOld=new List<Benefit>();
											removeLogs=true;
										}
										catch(ApplicationException ex) {
											MessageBox.Show(ex.Message);
											SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null) ? 0 : _patPlanCur.PatNum
												,Lan.g(this,"FormInsPlan region 6 delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,
												DateTime.MinValue); //new plan, no date needed.
											Close();
											return;
										}
										_insSubCur.PlanNum=_insPlanCur.PlanNum;
									}
									else {//option is checked for "create new plan if needed"
										_insPlanCur.PlanNum=_insPlanOld.PlanNum;
										InsPlans.Update(_insPlanCur);
										_insSubCur.PlanNum=_insPlanCur.PlanNum;
										//no need to update PatPlan.  Same old PlanNum.  When 'pick from list' button was pushed, benfitList was filled with benefits from
										//the picked plan.  benefitListOld was not touched and still contains the old benefits.  So the original benefits will be
										//automatically deleted.  We force copies to be made in the database, but with different PlanNums.  Any other changes will be preserved.
										for(int i = 0;i<_listBenefit.Count;i++) {
											if(_listBenefit[i].PlanNum>0) {
												_listBenefit[i].PlanNum=_insPlanCur.PlanNum;
												_listBenefit[i].BenefitNum=0;//triggers insert during synch.
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
								InsPlans.Update(_insPlanCur);
							}
							#endregion 6 - InsPlan Non-Null Subscriber, New Plan, Changes Made
						}//end else of if(InsPlans.AreEqual...
					}//end if(IsNewPlan)
					else {//editing an existing plan from within patient
						if(InsPlans.AreEqualValue(_insPlanCur,_planCurOriginal)) {//If no changes
							#region 7 - InsPlan Non-Null Subscriber, Not a New Plan, No Changes Made
							try {
								if(_insPlanCur.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, then made no changes
									//do not need to update PlanCur because no changes were made.
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(_patPlanCur.PatNum,_insPlanOld.PlanNum,_insPlanCur.PlanNum);
									}
									_insSubCur.PlanNum=_insPlanCur.PlanNum;
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
							if(_insPlanCur.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								if(radioChangeAll.Checked) {
									#region 8 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Change All Checked
									try {
										//warn user here?
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(_patPlanCur.PatNum,_insPlanOld.PlanNum,_insPlanCur.PlanNum);
										}
										InsPlans.Update(_insPlanCur);
										_insSubCur.PlanNum=_insPlanCur.PlanNum;
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
											InsPlans.Update(_insPlanCur);
											_insSubCur.PlanNum=_insPlanCur.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//So if any benefits were changed, the synch further down will trigger updates for the benefits on the picked plan.
										}
										else {//if there are other subscribers
											InsPlans.Insert(_insPlanCur);//this gives it a new primary key.
											_insSubCur.PlanNum=_insPlanCur.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//We must clear the benefitListOld to prevent deletion of those benefits.
											_listBenefitOld=new List<Benefit>();
											//Force copies to be made in the database, but with different PlanNum;
											for(int i = 0;i<_listBenefit.Count;i++) {
												if(_listBenefit[i].PlanNum>0) {
													_listBenefit[i].PlanNum=_insPlanCur.PlanNum;
													_listBenefit[i].BenefitNum=0;//triggers insert during synch.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_insPlanCur.PlanNum,_insPlanOld.PlanNum);
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
										InsPlans.Update(_insPlanCur);
									}
									else {//option is checked for "create new plan if needed"
										if(textLinkedNum.Text=="0") {//if this is the only subscriber
											InsPlans.Update(_insPlanCur);
										}
										else {//if there are other subscribers
											InsPlans.Insert(_insPlanCur);//this gives it a new primary key.
											_insSubCur.PlanNum=_insPlanCur.PlanNum;
											//PatPlanCur.PlanNum=PlanCur.PlanNum;
											//PatPlans.Update(PatPlanCur);
											//make copies of all the benefits
											_listBenefitOld=new List<Benefit>();
											for(int i = 0;i<_listBenefit.Count;i++) {
												if(_listBenefit[i].PlanNum>0) {
													_listBenefit[i].PlanNum=_insPlanCur.PlanNum;
													_listBenefit[i].BenefitNum=0;//triggers insert.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_insPlanCur.PlanNum,_insPlanOld.PlanNum);
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
				if(_insSubCur!=null) {
					if(IsNewPlan) {
						#region 5a - User Without Permissions, InsPlan Non-Null Subscriber, New Plan
						//If the logic in this region changes, then also change region 5 above.
						try {
							if(_insPlanCur.PlanNum != _insPlanOld.PlanNum) {//user clicked the "pick from list" button. 
								//In a previous version, a user could still change some things about the plan even if they had no permissions to do so.
								//This was causing empty insurance plans to get saved to the db.
								//Even if they somehow managed to change something about the insurance plan they picked, we always just want to do the following:
								//1. Update the inssub to be the current insplan. (which happens above)
								//2. Delete the empty insurance plan (which happens here)
								try {
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(_patPlanCur.PatNum,_insPlanOld.PlanNum,_insPlanCur.PlanNum);
									}
									//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
									InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
									_listBenefitOld=new List<Benefit>();
									removeLogs=true;
								}
								catch(ApplicationException ex) {
									MessageBox.Show(ex.Message);
									SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null) ? 0 : _patPlanCur.PatNum
										,Lan.g(this,"FormInsPlan region 5a delete validation failed.  Plan was not deleted."),
										_insPlanOld.PlanNum,DateTime.MinValue); //new plan, no date needed.
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
				if(!PIn.Date(textDateLastVerifiedBenefits.Text).Date.Equals(_dateTimeInsPlanLastVerified.Date)) {
					InsVerify insVerify=InsVerifies.GetOneByFKey(_insPlanCur.PlanNum,VerifyTypes.InsuranceBenefit);
					insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedBenefits.Text);
					InsVerifyHists.InsertFromInsVerify(insVerify);
				}
				//PatPlanCur.InsSubNum is already set before opening this window.  There is no possible way to change it from within this window.  Even if PlanNum changes, it's still the same inssub.  And even if inssub.Subscriber changes, it's still the same inssub.  So no change to PatPlanCur.InsSubNum is ever require from within this window.
				if(_listBenefitOld.Count>0 || _listBenefit.Count>0) {//Synch benefits
					Benefits.UpdateList(_listBenefitOld,_listBenefit);
				}
				if(removeLogs) {
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_insPlanOld.PlanNum);
				}
				if(_insSubCur!=null) {//Update SubCur if needed
					InsSubs.Update(_insSubCur);//also saves the other fields besides PlanNum
					if(_subOld.Subscriber!=_insSubCur.Subscriber) {//If the subscriber was changed, include an audit trail entry
						Dictionary<long,string> dictPatNames=Patients.GetPatientNames(new List<long>() { _subOld.Subscriber,_insSubCur.Subscriber });
						//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
						//However, the Change button is invisible from List > Insurance Plans, so we can count on PatPlanCur not null.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,_patPlanCur.PatNum,
							Lan.g(this,"Subscriber Changed from")+" "+dictPatNames[_subOld.Subscriber]+" #"+_subOld.Subscriber+" "
							+Lan.g(this,"to")+" "+dictPatNames[_insSubCur.Subscriber]+" #"+_insSubCur.Subscriber);
					}
					//Udate all claims, claimprocs, payplans, and etrans that are pointing at the inssub.InsSubNum since it may now be pointing at a new insplan.PlanNum.
					InsSubs.SynchPlanNumsForNewPlan(_insSubCur);
					InsPlans.ComputeEstimatesForSubscriber(_insSubCur.Subscriber);
					InsEditPatLogs.MakeLogEntry(_insSubCur,_subOld,InsEditPatLogType.Subscriber);
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
				Carrier carrierOrig=Carriers.GetCarrier(_planCurOriginal.CarrierNum);
				Carrier carrierNew=Carriers.GetCarrier(_insPlanCur.CarrierNum);
				if(_insPlanCur.CarrierNum!=_planCurOriginal.CarrierNum) {
					_hasCarrierChanged=true;
					long patNum=0;
					if(_patPlanCur!=null) {//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
						patNum=_patPlanCur.PatNum;
					}
					string carrierNameOrig=carrierOrig.CarrierName;
					string carrierNameNew=carrierNew.CarrierName;
					if(carrierNameOrig!=carrierNameNew) {//The CarrierNum could have changed but the CarrierName might not have changed.  Only make an audit entry if the name changed.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeCarrierName,patNum,Lan.g(this,"Carrier name changed in Edit Insurance Plan window from")+" "
							+(string.IsNullOrEmpty(carrierNameOrig)?"blank":carrierNameOrig)+" "+Lan.g(this,"to")+" "
							+(string.IsNullOrEmpty(carrierNameNew)?"blank":carrierNameNew),_insPlanCur.PlanNum,_planCurOriginal.SecDateTEdit);
					}
				}
				string message=GetSecurityLogMessage(carrierNew,carrierOrig);
				SecurityLogs.MakeLogEntry(Permissions.CarrierEdit,0,message);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 12")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion Carrier
			#region Carrier FeeSched
			try {
				Carrier carrierCur=Carriers.GetCarrier(_insPlanCur.CarrierNum);
				if(_planCurOriginal.FeeSched!=0 && _planCurOriginal.FeeSched!=_insPlanCur.FeeSched) {
					string feeSchedOld=FeeScheds.GetDescription(_planCurOriginal.FeeSched);
					string feeSchedNew=FeeScheds.GetDescription(_insPlanCur.FeeSched);
					string logText=Lan.g(this,"The fee schedule associated with insurance plan number")+" "+_insPlanCur.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"was changed from")+" "+feeSchedOld+" "+Lan.g(this,"to")+" "+feeSchedNew;
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlanCur==null?0:_patPlanCur.PatNum,logText,(_insPlanCur==null)?0:_insPlanCur.PlanNum,
						_planCurOriginal.SecDateTEdit);
				}
				if(InsPlanCrud.UpdateComparison(_planCurOriginal,_insPlanCur)) {
					string logText=Lan.g(this,"Insurance plan")+" "+_insPlanCur.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"has changed.");
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlanCur==null?0:_patPlanCur.PatNum,logText,(_insPlanCur==null)?0:_insPlanCur.PlanNum,
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
			if(_insPlanCur.PlanType!="" || !_insPlanCur.IsBlueBookEnabled) {
				InsBlueBooks.DeleteByPlanNums(_insPlanCur.PlanNum);
			}
			else {
				InsBlueBooks.UpdateByInsPlan(_insPlanCur);
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
			if(IsNewPlan && _insPlanOld.PlanNum==_planCurOriginal.PlanNum) {//Never have to warn user because the plan is new and was not picked from the list.
				return false;
			}
			if(_planCurOriginal.PlanType!="") {//Not Category Percentage plan so not bluebook eligible regardless if fee schedule changes.
				return false;
			}
			if((_planCurOriginal.IsBlueBookEnabled && !checkUseBlueBook.Checked)	|| (_planCurOriginal.PlanType=="" && _insPlanCur.PlanType!="")) {
				//Warn user because we have changed to use a fee schedule or from Category Percentage plan to a non-bluebook eligible plan.
				return true;
			}
			return false;
		}

		private void FormInsPlan_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				if(_patPlanCur!=null && (_hasDropped || _hasOrdinalChanged || _hasCarrierChanged || IsNewPatPlan || IsNewPlan || _hasDeleted || 
					_insPlanCur.IsMedical!=_insPlanOld.IsMedical)) 
				{
					Appointments.UpdateInsPlansForPat(_patPlanCur.PatNum);
				}
				if(IsNewPatPlan//Only when assigning new insurance
					&& _patPlanCur.Ordinal==1//Primary insurance.
					&& _insPlanCur.BillingType!=0//Selection made.
					&& Security.IsAuthorized(Permissions.PatientBillingEdit,true)
					&& PrefC.GetBool(PrefName.PatInitBillingTypeFromPriInsPlan))
				{
					Patient patOld=Patients.GetPat(_patPlanCur.PatNum);
					if(patOld.BillingType!=_insPlanCur.BillingType) {
						Patient patNew=patOld.Copy();
						patNew.BillingType=_insPlanCur.BillingType;
						Patients.Update(patNew,patOld);
						//This needs to be the last call due to automation possibily leaving the form in a closing limbo.
						AutomationL.Trigger(AutomationTrigger.SetBillingType,null,patNew.PatNum);
						Patients.InsertBillTypeChangeSecurityLogEntry(patOld,patNew);
					}
				}
				if(_insSubCur!=null && _hasDeleted) {
					List<PatPlan> listPatPlansForSub=PatPlans.GetListByInsSubNums(new List<long> { _insSubCur.InsSubNum });
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
					if(_insSubCur!=null) {
						InsSubs.Delete(_insSubCur.InsSubNum);
					}
					InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);//does dependency checking.
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_insPlanOld.PlanNum);
					//Ok to delete these adjustments because we deleted the benefits in Benefits.DeleteForPlan().
					ClaimProcs.DeleteMany(_arrayListAdj.ToArray().Cast<ClaimProc>().ToList());
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,(_patPlanCur==null)?0:_patPlanCur.PatNum
						,Lan.g(this,"FormInsPlan_Closing delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,DateTime.MinValue);//new plan, no date needed.
					return;
				}
			}
			if(IsNewPatPlan){
				PatPlans.Delete(_patPlanCur.PatPlanNum);//no need to check dependencies.  Maintains ordinals and recomputes estimates.
			}
		}

		///<summary>Check if PatPlan was dropped since window was opened.</summary>
		private bool IsPatPlanRemoved() {
			if(_patPlanCur!=null) {
				PatPlan patPlanExists=PatPlans.GetByPatPlanNum(_patPlanCur.PatPlanNum);
				if(patPlanExists==null) {
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
