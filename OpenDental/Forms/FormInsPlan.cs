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
		private Carrier _carrier;
		private PatPlan _patPlan;
		private PatPlan _patPlanOld;
		private ArrayList _arrayListAdj;
		///<summary>This is the current benefit list that displays on the form.  It does not get saved to the database until this form closes.</summary>
		private List<Benefit> _listBenefits;//each item is a Benefit
		private List<Benefit> _listBenefitsOld;
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
		private InsSub _insSubOld;
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
		private InsPlan _insPlan;
		///<summary>This is a copy of PlanCur as it was originally when this form was opened.  
		///This is needed to determine whether plan was changed.  However, this is also reset when 'pick from list' is used.</summary>
		private InsPlan _insPlanOriginal;
		///<summary>Ins sub for the currently selected plan.</summary>
		private InsSub _insSub;
		private bool _didAddInsHistCP;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox _listBoxEmps;
		/// <summary>displayed from within code, not designer</summary>
		private System.Windows.Forms.ListBox _listBoxCarriers;
		///<summary>The plan type that is selected in comboPlanType</summary>
		private InsPlanTypeComboItem _insPlanTypeComboItemSelected;

		///<summary>The original plan that was passed into this form. Assigned in the constructor and should never be modified.  
		///This allows intelligent decisions about how to save changes.</summary>
		private InsPlan _insPlanOld;

		public long GetPlanCurNum() {
				return _insPlan.PlanNum;
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsPlanCanada";
			}
			return "FormInsPlan";
		}

		///<summary>Called from ContrFamily and FormInsPlans. Must pass in the plan, patPlan, and sub, although patPlan and sub can be null.</summary>
		public FormInsPlan(InsPlan insPlan,PatPlan patPlan,InsSub insSub){
			Cursor=Cursors.WaitCursor;
			InitializeComponent();
			InitializeLayoutManager();
			_insPlan=insPlan;
			_insPlanOld=_insPlan.Copy();
			_patPlan=patPlan;
			_patPlanOld=patPlan?.Copy();
			_insSub=insSub;
			_listBoxEmps=new ListBox();//Instead of ListBoxOD for consistency with listCars.
			_listBoxEmps.Location=new Point(LayoutManager.Scale(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+textEmployer.Left),
				LayoutManager.Scale(tabPageInsPlanInfo.Top+tabControlInsPlan.Top+panelPlan.Top+groupPlan.Top+textEmployer.Bottom));
			_listBoxEmps.Size=LayoutManager.ScaleSize(new Size(231,100));
			_listBoxEmps.Font=new Font(Font.FontFamily,LayoutManager.ScaleF(Font.Size));
			_listBoxEmps.Visible=false;
			_listBoxEmps.Click += new System.EventHandler(listBoxEmps_Click);
			_listBoxEmps.DoubleClick += new System.EventHandler(listBoxEmps_DoubleClick);
			_listBoxEmps.MouseEnter += new System.EventHandler(listBoxEmps_MouseEnter);
			_listBoxEmps.MouseLeave += new System.EventHandler(listBoxEmps_MouseLeave);
			LayoutManager.Add(_listBoxEmps,this);
			_listBoxEmps.BringToFront();
			_listBoxCarriers=new ListBox();//Instead of ListBoxOD, for horiz scroll on a dropdown.
			_listBoxCarriers.Location=new Point(LayoutManager.Scale(tabControlInsPlan.Left+tabPageInsPlanInfo.Left+panelPlan.Left+groupPlan.Left+groupCarrier.Left+textCarrier.Left),
				LayoutManager.Scale(tabControlInsPlan.Top+tabPageInsPlanInfo.Top+panelPlan.Top+groupPlan.Top+groupCarrier.Top+textCarrier.Bottom));
			_listBoxCarriers.Size=LayoutManager.ScaleSize(new Size(700,100));
			_listBoxCarriers.Font=new Font(Font.FontFamily,LayoutManager.ScaleF(Font.Size));
			_listBoxCarriers.HorizontalScrollbar=true;
			_listBoxCarriers.Visible=false;
			_listBoxCarriers.Click += new System.EventHandler(listBoxCarriers_Click);
			_listBoxCarriers.DoubleClick += new System.EventHandler(listBoxCarriers_DoubleClick);
			_listBoxCarriers.MouseEnter += new System.EventHandler(listBoxCarriers_MouseEnter);
			_listBoxCarriers.MouseLeave += new System.EventHandler(listBoxCarriers_MouseLeave);
			LayoutManager.Add(_listBoxCarriers,this);
			_listBoxCarriers.BringToFront();
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
				checkIsPMP.Checked=(insPlan.CanadianPlanFlag!=null && insPlan.CanadianPlanFlag!="");
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
			_insPlanOriginal=_insPlan.Copy();
			_listInsFilingCodes=InsFilingCodes.GetDeepCopy();
			if(_insSub!=null) {
				_insSubOld=_insSub.Copy();
			}
			long patPlanNum=0;
			checkUseBlueBook.Visible=false; // hidden by default, shown only if bluebook feature is turned on and plan is category%
			comboOutOfNetwork.Enabled=true;
			comboManualBlueBook.Enabled=false;
			if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook && _insPlan.PlanType=="") {
				if(_insPlan.IsBlueBookEnabled) {
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
			if(_patPlan!=null) {
				patPlanNum=_patPlan.PatPlanNum;
			}
			if(_insSub==null) {//editing from big list
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
			checkDontVerify.Checked=_insPlan.HideFromVerifyList;
			InsVerify insVerifyBenefits=InsVerifies.GetOneByFKey(_insPlan.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefits!=null && insVerifyBenefits.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefits.DateLastVerified.ToShortDateString();
			}
			if(IsNewPlan) {//Regardless of whether from big list or from individual patient.  Overrides above settings.
				//radioCreateNew.Checked=true;//this logic needs to be repeated in OK.
				//groupChanges.Visible=false;//because it wouldn't make sense to apply anything to "all"
				if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
					_insPlan.PlanType="p";
					checkUseBlueBook.Visible=false;
				}
				_insPlan.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
				textDateLastVerifiedBenefits.Text="";
			}
			_listBenefits=Benefits.RefreshForPlan(_insPlan.PlanNum,patPlanNum);
			_listBenefitsOld=new List<Benefit>();
			for(int i=0;i<_listBenefits.Count;i++){
				_listBenefitsOld.Add(_listBenefits[i].Copy());
			}
			if(_insPlan.PlanNum!=0) {
				textInsPlanNum.Text=_insPlan.PlanNum.ToString();
			}
			if(PrefC.GetBool(PrefName.EasyHideCapitation)) {
				//groupCoPay.Visible=false;
				//comboCopay.Visible=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
				checkAlternateCode.Visible=false;
			}
			Program program=Programs.GetCur(ProgramName.Trojan);
			if(program!=null && program.Enabled) {
				textTrojanID.Text=_insPlan.TrojanID;
			}
			else {
				//labelTrojan.Visible=false;
				labelTrojanID.Visible=false;
				butImportTrojan.Visible=false;
				textTrojanID.Visible=false;
			}
			program=Programs.GetCur(ProgramName.IAP);
			if(program==null || !program.Enabled) {
				//labelIAP.Visible=false;
				butIapFind.Visible=false;
			}
			if(!butIapFind.Visible && !butImportTrojan.Visible) {
				butBenefitNotes.Visible=false;
			}
			//FillPatData------------------------------
			if(_patPlan==null) {
				panelPat.Visible=false;
				//PatPlanCur is sometimes null
				butGetElectronic.Visible=false;
				butHistoryElect.Visible=false;
			}
			else {
				comboRelationship.Items.Clear();
				for(int i=0;i<Enum.GetNames(typeof(Relat)).Length;i++) {
					comboRelationship.Items.Add(Lan.g("enumRelat",Enum.GetNames(typeof(Relat))[i]));
					if((int)_patPlan.Relationship==i) {
						comboRelationship.SelectedIndex=i;
					}
				}
				if(_patPlan.PatPlanNum!=0) {
					textPatPlanNum.Text=_patPlan.PatPlanNum.ToString();
					if(IsNewPatPlan) {
						//Relationship is set to Self,  but the subscriber for the plan is not set to the current patient.
						if(comboRelationship.SelectedIndex==0 && _insSub.Subscriber!=_patPlan.PatNum) {
								comboRelationship.SelectedIndex=-1;
						}
					}
					else {
						InsVerify insVerifyPatPlan=InsVerifies.GetOneByFKey(_patPlan.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerifyPatPlan!=null && insVerifyPatPlan.DateLastVerified.Year>1880) {
							textDateLastVerifiedPatPlan.Text=insVerifyPatPlan.DateLastVerified.ToShortDateString();
						}
					}
				}
				textOrdinal.Text=_patPlan.Ordinal.ToString();
				checkIsPending.Checked=_patPlan.IsPending;
				textPatID.Text=_patPlan.PatID;
				FillPatientAdjustments();
			}
			if(_insSub!=null) {
				textSubscriber.Text=Patients.GetLim(_insSub.Subscriber).GetNameLF();
				textSubscriberID.Text=_insSub.SubscriberID;
				if(_insSub.DateEffective.Year < 1880) {
					textDateEffect.Text="";
				}
				else {
					textDateEffect.Text=_insSub.DateEffective.ToString("d");
				}
				if(_insSub.DateTerm.Year < 1880) {
					textDateTerm.Text="";
				}
				else {
					textDateTerm.Text=_insSub.DateTerm.ToString("d");
				}
				checkRelease.Checked=_insSub.ReleaseInfo;
				checkAssign.Checked=_insSub.AssignBen;
				textSubscNote.Text=_insSub.SubscNote;
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
			_employerNameOrig=Employers.GetName(_insPlan.EmployerNum);
			_employerNameCur=Employers.GetName(_insPlan.EmployerNum);
			_carrierNumOrig=_insPlan.CarrierNum;
			_listClaimForms=ClaimForms.GetDeepCopy(false);
			comboSendElectronically.Items.AddEnums<NoSendElectType>();//selected index set in FillFormWithPlanCur -> FillCarrier
			FillFormWithPlanCur(false);
			FillBenefits();
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlan.PlanNum);
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
			_procedureCodeOrthoAuto=null;
			if(_insPlan.OrthoAutoProcCodeNumOverride!=0) {
				_procedureCodeOrthoAuto=ProcedureCodes.GetProcCode(_insPlan.OrthoAutoProcCodeNumOverride);
			}
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
			for(int i = 0;i<Enum.GetValues(typeof(OrthoClaimType)).Length;i++) {
				comboOrthoClaimType.Items.Add(Lan.g("enumOrthoClaimType",((OrthoClaimType)i).GetDescription()));
				if((int)_insPlan.OrthoType==i) {
					comboOrthoClaimType.SelectedIndex = i;
				}
			}
			comboOrthoAutoProcPeriod.Items.Clear();
			for(int i=0;i<Enum.GetValues(typeof(OrthoAutoProcFrequency)).Length;i++) {
				comboOrthoAutoProcPeriod.Items.Add(Lan.g("enumOrthoAutoProcFrequency",((OrthoAutoProcFrequency)i).GetDescription()));
				if(i==(int)_insPlan.OrthoAutoProcFreq) {
					comboOrthoAutoProcPeriod.SelectedIndex=i;
				}
			}
			textOrthoAutoFee.Text=_insPlan.OrthoAutoFeeBilled.ToString();
			checkOrthoWaitDays.Checked=_insPlan.OrthoAutoClaimDaysWait > 0;
			if(_procedureCodeOrthoAuto==null) {
				textOrthoAutoProc.Text=ProcedureCodes.GetProcCode(PrefC.GetLong(PrefName.OrthoAutoProcCodeNum)).ProcCode +" ("+ Lan.g(this,"Default")+")";
			}
			else {
				textOrthoAutoProc.Text=_procedureCodeOrthoAuto.ProcCode;
			}
			SetEnabledOrtho();
		}

		private void SetEnabledOrtho() {
			if(!Security.IsAuthorized(Permissions.InsPlanOrthoEdit,true)) {
				//Disable every control within the Ortho tab.
				for(int i=0;i<panelOrtho.Controls.Count;i++) {
					ODException.SwallowAnyException(() => { panelOrtho.Controls[i].Enabled=false; });
				}
				return;
			}
			if(comboOrthoClaimType.SelectedIndex==(int)OrthoClaimType.InitialPlusPeriodic) {
				comboOrthoAutoProcPeriod.Enabled=true;
				checkOrthoWaitDays.Enabled=true;
				labelAutoOrthoProcPeriod.Enabled=true;
				butPickOrthoProc.Enabled=true;
				labelOrthoAutoFee.Enabled=true;
				textOrthoAutoFee.Enabled=true;
				butDefaultAutoOrthoProc.Enabled=true;
			}
			else {
				comboOrthoAutoProcPeriod.Enabled=false;
				checkOrthoWaitDays.Checked=false;
				checkOrthoWaitDays.Enabled=false;
				labelAutoOrthoProcPeriod.Enabled=false;
				butPickOrthoProc.Enabled=false;
				labelOrthoAutoFee.Enabled=false;
				textOrthoAutoFee.Enabled=false;
				butDefaultAutoOrthoProc.Enabled=false;
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
			textEmployer.Text=Employers.GetName(_insPlan.EmployerNum);
			_employerNameCur=textEmployer.Text;
			textGroupName.Text=_insPlan.GroupName;
			textGroupNum.Text=_insPlan.GroupNum;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				textBIN.Text=_insPlan.RxBIN;
			}
			else{
				labelBIN.Visible=false;
				textBIN.Visible=false;
			}
			textDivisionNo.Text=_insPlan.DivisionNo;//only visible in Canada
			textTrojanID.Text=_insPlan.TrojanID;
			comboPlanType.Items.Clear();
			//Items must be added in the same order in which they are listed in InsPlanTypeComboItem.
			comboPlanType.Items.Add(Lan.g(this,"Category Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Percentage"));
			comboPlanType.Items.Add(Lan.g(this,"PPO Fixed Benefit"));
			comboPlanType.Items.Add(Lan.g(this,"Medicaid or Flat Co-pay"));
			//Capitation must always be last, since it is sometimes hidden.
			if(!PrefC.GetBool(PrefName.EasyHideCapitation)) {
				comboPlanType.Items.Add(Lan.g(this,"Capitation"));
				if(_insPlan.PlanType=="c") {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.Capitation;
				}
			}
			if(_insPlan.PlanType=="") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.CategoryPercentage;
			}
			if(_insPlan.PlanType=="p") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPO;
				FeeSched feeSchedCopay=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==_insPlan.CopayFeeSched 
					&& x.FeeSchedType==FeeScheduleType.FixedBenefit);
				if(feeSchedCopay!=null) {
					comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.PPOFixedBenefit;
				}
			}
			if(_insPlan.PlanType=="f") {
				comboPlanType.SelectedIndex=(int)InsPlanTypeComboItem.MedicaidOrFlatCopay;
			}
			_insPlanTypeComboItemSelected=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			checkAlternateCode.Checked=_insPlan.UseAltCode;
			checkCodeSubst.Checked=_insPlan.CodeSubstNone;
			checkPpoSubWo.Checked=_insPlan.HasPpoSubstWriteoffs;
			checkIsMedical.Checked=_insPlan.IsMedical;
			if(!PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				checkIsMedical.Visible=false;//This line prevents most users from modifying the Medical Insurance checkbox by accident, because most offices are dental only.
				labelMedicalInsurance.Visible=false;
			}
			checkClaimsUseUCR.Checked=_insPlan.ClaimsUseUCR;
			if(IsNewPlan && _insPlan.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims) && !isPicked) {
				checkClaimsUseUCR.Checked=true;
			}
			if(IsNewPlan && !PrefC.GetBool(PrefName.InsDefaultAssignBen) && !isPicked) {
				checkAssign.Checked=false;
			}
			checkIsHidden.Checked=_insPlan.IsHidden;
			checkShowBaseUnits.Checked=_insPlan.ShowBaseUnits;
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.AddNone<FeeSched>();
			comboFeeSched.Items.AddList(_listFeeSchedsStandard,x=>x.Description);
			comboFeeSched.SetSelectedKey<FeeSched>(_insPlan.FeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
			comboClaimForm.Items.Clear();
			for(int i=0;i<_listClaimForms.Count;i++) {
				//The default claim form will always show even if hidden.
				if(_listClaimForms[i].IsHidden && _listClaimForms[i].ClaimFormNum!=_insPlan.ClaimFormNum && _listClaimForms[i].ClaimFormNum!=PrefC.GetLong(PrefName.DefaultClaimForm)) {
					continue;
				}
				string descript=_listClaimForms[i].Description;
				if(_listClaimForms[i].IsHidden) {
					descript+= " (hidden)";
				}
				comboClaimForm.Items.Add(descript,_listClaimForms[i]);
				if(_listClaimForms[i].ClaimFormNum==_insPlan.ClaimFormNum) {
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
			comboOutOfNetwork.SetSelectedKey<FeeSched>(_insPlan.AllowedFeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
			comboManualBlueBook.Items.Clear();
			comboManualBlueBook.Items.AddNone<FeeSched>();
			comboManualBlueBook.Items.AddList(_listFeeSchedsManualBlueBook,x => x.Description);
			comboManualBlueBook.SetSelectedKey<FeeSched>(_insPlan.ManualFeeSchedNum,x => x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
			comboCobRule.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumCobRule)).Length;i++) {
				comboCobRule.Items.Add(Lan.g("enumEnumCobRule",Enum.GetNames(typeof(EnumCobRule))[i]));
			}
			comboCobRule.SelectedIndex=(int)_insPlan.CobRule;			
			long selectedFilingCodeNum=_insPlan.FilingCode;
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
			comboBillType.SetSelectedDefNum(_insPlan.BillingType); 
			comboExclusionFeeRule.Items.Clear();
			comboExclusionFeeRule.Items.AddEnums<ExclusionRule>();
			comboExclusionFeeRule.SetSelectedEnum(_insPlan.ExclusionFeeRule);
			FillCarrier(_insPlan.CarrierNum);
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
			textPlanNote.Text=_insPlan.PlanNote;
			if(_insPlan.DentaideCardSequence==0){
				textDentaide.Text="";
			}
			else{
				textDentaide.Text=_insPlan.DentaideCardSequence.ToString();
			}
			textPlanFlag.Text=_insPlan.CanadianPlanFlag;
			textCanadianDiagCode.Text=_insPlan.CanadianDiagnosticCode;
			textCanadianInstCode.Text=_insPlan.CanadianInstitutionCode;
			checkDontVerify.Checked=_insPlan.HideFromVerifyList;
			checkUseBlueBook.Checked=_insPlan.IsBlueBookEnabled;
			InsVerify insVerifyBenefits=InsVerifies.GetOneByFKey(_insPlan.PlanNum,VerifyTypes.InsuranceBenefit);
			if(insVerifyBenefits!=null && insVerifyBenefits.DateLastVerified.Year>1880) {//Only show a date if this insurance has ever been verified
				textDateLastVerifiedBenefits.Text=insVerifyBenefits.DateLastVerified.ToShortDateString();
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
			for(int i=0;i<listFeeSchedCopays.Count;i++) {
				if(!IsFixedBenefitMismatch(listFeeSchedCopays[i])) {
					listFeeSchedFiltered.Add(listFeeSchedCopays[i].Copy());
				}
			}
			return listFeeSchedFiltered;
		}

		private void FillOtherSubscribers() {
			long excludeSub=-1;
			if(_insSub!=null){
				excludeSub=_insSub.InsSubNum;
			}
			//Even though this sub hasn't been updated to the database, this still works because SubCur.InsSubNum is valid and won't change.
			int countSubs=InsSubs.GetSubscriberCountForPlan(_insPlan.PlanNum,excludeSub!=-1);
			textLinkedNum.Text=countSubs.ToString();
			if(countSubs>10000) {//10,000 per Nathan.
				comboLinked.Visible=false;
				butOtherSubscribers.Visible=true;
				butOtherSubscribers.Location=comboLinked.Location;
				return;
			}
			comboLinked.Visible=true;
			butOtherSubscribers.Visible=false;
			List<string> listStringSubs=InsSubs.GetSubscribersForPlan(_insPlan.PlanNum,excludeSub);
			comboLinked.Items.Clear();
			comboLinked.Items.AddList(listStringSubs.ToArray());
			if(listStringSubs.Count>0){
				comboLinked.SelectedIndex=0;
			}
		}

		private void butOtherSubscribers_Click(object sender,EventArgs e) {
			using FormSubscribersList formSubscribersList=new FormSubscribersList();
			formSubscribersList.InsPlanCur = _insPlan;
			formSubscribersList.InsSubCur = _insSub;
			formSubscribersList.ShowDialog();
		}
		
		private void FillPatientAdjustments() {
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patPlan.PatNum);
			_arrayListAdj=new ArrayList();//move selected claimprocs into ALAdj
			for(int i=0;i<listClaimProcs.Count;i++) {
				if(listClaimProcs[i].InsSubNum==_insSub.InsSubNum
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
			_carrier=Carriers.GetCarrier(carrierNum);
			textCarrier.Text=_carrier.CarrierName;
			textPhone.Text=_carrier.Phone;
			textAddress.Text=_carrier.Address;
			textAddress2.Text=_carrier.Address2;
			textCity.Text=_carrier.City;
			textState.Text=_carrier.State;
			textZip.Text=_carrier.Zip;
			textElectID.Text=_carrier.ElectID;
			_electIdCur=textElectID.Text;
			FillPayor();
			comboSendElectronically.SetSelectedEnum(_carrier.NoSendElect);
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
			if((_insPlan.PlanType=="" || _insPlan.PlanType=="p")
				&& comboPlanType.SelectedIndex.In((int)InsPlanTypeComboItem.MedicaidOrFlatCopay,(int)InsPlanTypeComboItem.Capitation)) 
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear all percentages. Continue?")) {
					comboPlanType.SelectedIndex=(int)_insPlanTypeComboItemSelected;//Undo the selection change.
					return;
				}
				//Loop through the list backwards so i will be valid.
				for(int i=_listBenefits.Count-1;i>=0;i--) {
					if(((Benefit)_listBenefits[i]).BenefitType==InsBenefitType.CoInsurance) {
						_listBenefits.RemoveAt(i);
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
				for(int i=0;i<_listBenefits.Count;i++) {
					if(_listBenefits[i].BenefitType==InsBenefitType.CoInsurance) {
						_listBenefits[i].Percent=100;
					}
				}
				FillBenefits();
			}
			InsPlanTypeComboItem InsPlanTypeComboItemPrevSelection=_insPlanTypeComboItemSelected;
			_insPlanTypeComboItemSelected=PIn.Enum<InsPlanTypeComboItem>(comboPlanType.SelectedIndex);
			switch(_insPlanTypeComboItemSelected) {
				case InsPlanTypeComboItem.CategoryPercentage:
					_insPlan.PlanType="";
					if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook) {
						checkUseBlueBook.Visible=true; //Only show checkbox if Blue Book is enabled.
					}
					break;
				case InsPlanTypeComboItem.PPO:
				case InsPlanTypeComboItem.PPOFixedBenefit:
					_insPlan.PlanType="p";
					checkUseBlueBook.Visible=false;
					break;
				case InsPlanTypeComboItem.MedicaidOrFlatCopay:
					_insPlan.PlanType="f";
					checkUseBlueBook.Visible=false;
					break;
				case InsPlanTypeComboItem.Capitation:
					_insPlan.PlanType="c";
					checkUseBlueBook.Visible=false;
					break;
				default:
					break;
			}
			SetAllowedFeeScheduleControls();
			if(PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {//otherwise, no automation on this field.
				if(_insPlan.PlanType=="") {
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

		/// <summary>Helper function that returns true if text width is longer than controler width</summary>
		private bool IsHorizontalScrollBarVisibleForListBox(ListBox listBox) {
			if(!listBox.HorizontalScrollbar) {
				return false;
			}
			for(int i=0;i<listBox.Items.Count;i++) {
				int textWidth=TextRenderer.MeasureText(listBox.Items[i].ToString(),listBox.Font).Width;
				if(textWidth>listBox.Width) {
					return true;
				}
			}
			return false;
		}

		/// <summary>Calculates the list box height and retuns the value</summary>
		private int CalculateListBoxHeight<T>(ListBox listBox, List<T> listItems) {
			int horizontalScrollBarHeight=0;
			if(IsHorizontalScrollBarVisibleForListBox(listBox)) {
				horizontalScrollBarHeight=17;
			}
			//Base line height is 13. The box height is not tall enough without the +5.
			//Scroll bar height is always 17 and doesn't need to be scaled.
			int h=LayoutManager.Scale(13*listItems.Count+5)+horizontalScrollBarHeight;
			if(h > ClientSize.Height-listBox.Top){
				h=ClientSize.Height-listBox.Top;
			}
			return h;
		}

		private void FillComboCoPay() {
			List<FeeSched> listFilteredCopayFeeSched=GetFilteredCopayFeeSched(_listFeeSchedsCopay);
			comboCopay.Items.Clear();
			comboCopay.Items.AddNone<FeeSched>();
			comboCopay.Items.AddList(listFilteredCopayFeeSched,x=>x.Description);
			comboCopay.SetSelectedKey<FeeSched>(_insPlan.CopayFeeSched,x=>x.FeeSchedNum,x=>FeeScheds.GetDescription(x));
		}

		private void butAdjAdd_Click(object sender,System.EventArgs e) {
			ClaimProc claimProc=ClaimProcs.CreateInsPlanAdjustment(_patPlan.PatNum,_insPlan.PlanNum,_insSub.InsSubNum);
			using FormInsAdj formInsAdj=new FormInsAdj(claimProc);
			formInsAdj.IsNew=true;
			formInsAdj.ShowDialog();
			FillPatientAdjustments();
		}

		private void butHistory_Click(object sender,EventArgs e) {
			using FormInsHistSetup formInsHistSetup=new FormInsHistSetup(_patPlan.PatNum,_insSub);
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
			if(formInsPlans.InsPlanSelected.PlanNum==0) {//user clicked Blank
				_insPlan=new InsPlan();
				_insPlan.PlanNum=_insPlanOld.PlanNum;
			}
			else {//user selected an existing plan
				_insPlan=formInsPlans.InsPlanSelected;
				textInsPlanNum.Text=formInsPlans.InsPlanSelected.PlanNum.ToString();
			}
			FillFormWithPlanCur(true);
			//We need to pass patPlanNum in to RefreshForPlan to get patient level benefits:
			long patPlanNum=0;
			if(_patPlan!=null){
				patPlanNum=_patPlan.PatPlanNum;
			}
			if(formInsPlans.InsPlanSelected.PlanNum==0){//user clicked blank
				_listBenefits=new List<Benefit>();
			}
			else {//user selected an existing plan
				_listBenefits=Benefits.RefreshForPlan(_insPlan.PlanNum,patPlanNum);
			}
			FillBenefits();
			if(IsNewPlan || formInsPlans.InsPlanSelected.PlanNum==0) {//New plan or user clicked blank.
				//Leave benefitListOld alone so that it will trigger deletion of the orphaned benefits later.
			}
			else{
				//Replace benefitListOld so that we only cause changes to be save that are made after this point.
				_listBenefitsOld=new List<Benefit>();
				for(int i=0;i<_listBenefits.Count;i++) {
					_listBenefitsOld.Add(_listBenefits[i].Copy());
				}
			}
			//benefitListOld=new List<Benefit>(benefitList);//this was not the proper way to make a shallow copy.
			_insPlanOriginal=_insPlan.Copy();
			FillOtherSubscribers();
			FillOrtho();
			//PlanNumOriginal is NOT reset here.
			//It's now similar to if we'd just opened a new form, except for SubCur still needs to be changed.
		}

		private void textEmployer_KeyUp(object sender,System.Windows.Forms.KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.KeyCode==Keys.Return) {
				_listBoxEmps.Visible=false;
				textGroupName.Focus();
				return;
			}
			if(textEmployer.Text=="") {
				_listBoxEmps.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(_listBoxEmps.Items.Count==0) {
					return;
				}
				if(_listBoxEmps.SelectedIndex==-1) {
					_listBoxEmps.SelectedIndex=0;
					textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
				}
				else if(_listBoxEmps.SelectedIndex==_listBoxEmps.Items.Count-1) {
					_listBoxEmps.SelectedIndex=-1;
					textEmployer.Text=_stringEmpOriginal;
				}
				else {
					_listBoxEmps.SelectedIndex++;
					textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(_listBoxEmps.Items.Count==0) {
					return;
				}
				if(_listBoxEmps.SelectedIndex==-1) {
					_listBoxEmps.SelectedIndex=_listBoxEmps.Items.Count-1;
					textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
				}
				else if(_listBoxEmps.SelectedIndex==0) {
					_listBoxEmps.SelectedIndex=-1;
					textEmployer.Text=_stringEmpOriginal;
				}
				else {
					_listBoxEmps.SelectedIndex--;
					textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
				}
				textEmployer.SelectionStart=textEmployer.Text.Length;
				return;
			}
			if(textEmployer.Text.Length==1) {
				textEmployer.Text=textEmployer.Text.ToUpper();
				textEmployer.SelectionStart=1;
			}
			_stringEmpOriginal=textEmployer.Text;//the original text is preserved when using up and down arrows
			_listBoxEmps.Items.Clear();
			List<Employer> listEmployersSimilar=Employers.GetSimilarNames(textEmployer.Text);
			for(int i=0;i<listEmployersSimilar.Count;i++) {
				_listBoxEmps.Items.Add(listEmployersSimilar[i].EmpName);
			}
			int height=CalculateListBoxHeight<Employer>(_listBoxEmps,listEmployersSimilar);
			LayoutManager.MoveSize(_listBoxEmps,new Size(_listBoxEmps.Width,height));
			_listBoxEmps.Visible=true;
		}

		private void textEmployer_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListEmps) {
				return;
			}
			_listBoxEmps.Visible=false;
		}

		private void listBoxEmps_Click(object sender,System.EventArgs e) {
			if(_listBoxEmps.SelectedItem==null) {
				return;
			}
			textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			_listBoxEmps.Visible=false;
		}

		private void listBoxEmps_DoubleClick(object sender,System.EventArgs e) {
			//no longer used
			if(_listBoxEmps.SelectedIndex==-1) {
				return;
			}
			textEmployer.Text=_listBoxEmps.SelectedItem.ToString();
			textEmployer.Focus();
			textEmployer.SelectionStart=textEmployer.Text.Length;
			_listBoxEmps.Visible=false;
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
				if(_listBoxCarriers.SelectedIndex==-1) {
					textPhone.Focus();
				}
				else {
					FillCarrier(_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierNum);
					textCarrier.Focus();
					textCarrier.SelectionStart=textCarrier.Text.Length;
				}
				_listBoxCarriers.Visible=false;
				return;
			}
			if(textCarrier.Text=="") {
				_listBoxCarriers.Visible=false;
				return;
			}
			if(e.KeyCode==Keys.Down) {
				if(_listBoxCarriers.Items.Count==0) {
					return;
				}
				if(_listBoxCarriers.SelectedIndex==-1) {
					_listBoxCarriers.SelectedIndex=0;
					textCarrier.Text=_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierName;
				}
				else if(_listBoxCarriers.SelectedIndex==_listBoxCarriers.Items.Count-1) {
					_listBoxCarriers.SelectedIndex=-1;
					textCarrier.Text=_carrierOriginal;
				}
				else {
					_listBoxCarriers.SelectedIndex++;
					textCarrier.Text=_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(e.KeyCode==Keys.Up) {
				if(_listBoxCarriers.Items.Count==0) {
					return;
				}
				if(_listBoxCarriers.SelectedIndex==-1) {
					_listBoxCarriers.SelectedIndex=_listBoxCarriers.Items.Count-1;
					textCarrier.Text=_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierName;
				}
				else if(_listBoxCarriers.SelectedIndex==0) {
					_listBoxCarriers.SelectedIndex=-1;
					textCarrier.Text=_carrierOriginal;
				}
				else {
					_listBoxCarriers.SelectedIndex--;
					textCarrier.Text=_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierName;
				}
				textCarrier.SelectionStart=textCarrier.Text.Length;
				return;
			}
			if(textCarrier.Text.Length==1) {
				textCarrier.Text=textCarrier.Text.ToUpper();
				textCarrier.SelectionStart=1;
			}
			_carrierOriginal=textCarrier.Text;//the original text is preserved when using up and down arrows
			_listBoxCarriers.Items.Clear();
			_listCarriersSimilar=Carriers.GetSimilarNames(textCarrier.Text);
			for(int i=0;i<_listCarriersSimilar.Count;i++) {
				_listBoxCarriers.Items.Add(_listCarriersSimilar[i].CarrierName+", "
					+_listCarriersSimilar[i].Phone+", "
					+_listCarriersSimilar[i].Address+", "
					+_listCarriersSimilar[i].Address2+", "
					+_listCarriersSimilar[i].City+", "
					+_listCarriersSimilar[i].State+", "
					+_listCarriersSimilar[i].Zip);
			}
			int height=CalculateListBoxHeight<Carrier>(_listBoxCarriers,_listCarriersSimilar);
			LayoutManager.MoveSize(_listBoxCarriers,new Size(_listBoxCarriers.Width,height));
			_listBoxCarriers.Visible=true;
		}

		private void textCarrier_Leave(object sender,System.EventArgs e) {
			if(_isMouseInListCarriers) {
				return;
			}
			//or if user clicked on a different text box.
			if(_listBoxCarriers.SelectedIndex!=-1) {
				FillCarrier(_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierNum);
			}
			_listBoxCarriers.Visible=false;
		}

		private void listBoxCarriers_Click(object sender,System.EventArgs e) {
			if(_listBoxCarriers.SelectedIndex==-1) {
				return;
			}
			FillCarrier(_listCarriersSimilar[_listBoxCarriers.SelectedIndex].CarrierNum);
			textCarrier.Focus();
			textCarrier.SelectionStart=textCarrier.Text.Length;
			_listBoxCarriers.Visible=false;
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
				InsSubs.ValidateNoKeys(_insSub.InsSubNum,false);
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
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change subscriber?  This should not normally be needed.")) {
				return;
			}
			Family family=Patients.GetFamily(_insSub.Subscriber);
			using FormFamilyMemberSelect formFamilyMemberSelect=new FormFamilyMemberSelect(family,true);
			formFamilyMemberSelect.ShowDialog();
			if(formFamilyMemberSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_insSub.Subscriber=formFamilyMemberSelect.SelectedPatNum;
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
				if(_insPlan.FilingCodeSubtype==listInsFilingCodeSubtype[i].InsFilingCodeSubtypeNum) {
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
			_insPlan.BillingType=comboBillType.GetSelectedDefNum();
		}

		private void comboExclusionFeeRule_SelectionChangeCommitted(object sender,EventArgs e) {
			_insPlan.ExclusionFeeRule=(ExclusionRule)comboExclusionFeeRule.SelectedIndex;
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
			_insPlan.MonthRenew=(byte)trojanObject.MonthRenewal;
			if(_insSub.BenefitNotes!="") {
				_insSub.BenefitNotes+="\r\n--------------------------------\r\n";
			}
			_insSub.BenefitNotes+=trojanObject.BenefitNotes;
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
			for(int i=0;i<_listBenefits.Count;i++) {
				if(Benefits.IsBitewingFrequency(_listBenefits[i]) 
					|| Benefits.IsCancerScreeningFrequency(_listBenefits[i])
					|| Benefits.IsCrownFrequency(_listBenefits[i])
					|| Benefits.IsDenturesFrequency(_listBenefits[i])
					|| Benefits.IsExamFrequency(_listBenefits[i])
					|| Benefits.IsFlourideAgeLimit(_listBenefits[i])
					|| Benefits.IsFlourideFrequency(_listBenefits[i])
					|| Benefits.IsFullDebridementFrequency(_listBenefits[i])
					|| Benefits.IsImplantFrequency(_listBenefits[i])
					|| Benefits.IsPanoFrequency(_listBenefits[i])
					|| Benefits.IsPerioMaintFrequency(_listBenefits[i])
					|| Benefits.IsProphyFrequency(_listBenefits[i])
					|| Benefits.IsSealantAgeLimit(_listBenefits[i])
					|| Benefits.IsSealantFrequency(_listBenefits[i])
					|| Benefits.IsSRPFrequency(_listBenefits[i]))
				{
					listBenefitsToKeep.Add(_listBenefits[i]);
				}
			}
			_listBenefits=new List<Benefit>();
			_listBenefits.AddRange(listBenefitsToKeep);
			for(int i=0;i<trojanObject.BenefitList.Count;i++){
				//if(fields[2]=="Anniversary year") {
				//	usesAnnivers=true;
				//	MessageBox.Show("Warning.  Plan uses Anniversary year rather than Calendar year.  Please verify the Plan Start Date.");
				//}
				trojanObject.BenefitList[i].PlanNum=_insPlan.PlanNum;
				_listBenefits.Add(trojanObject.BenefitList[i].Copy());
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
			_listBenefits=new List<Benefit>();
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
							if(_insSub.BenefitNotes!="") {
								_insSub.BenefitNotes+="\r\n";
							}
							_insSub.BenefitNotes+="Employer: "+field;
							textEmployer.Text=field;
							break;
						case Iap.Phone:
							_insSub.BenefitNotes+="\r\n"+"Phone: "+field;
							break;
						case Iap.InsUnder:
							_insSub.BenefitNotes+="\r\n"+"InsUnder: "+field;
							break;
						case Iap.Carrier:
							_insSub.BenefitNotes+="\r\n"+"Carrier: "+field;
							textCarrier.Text=field;
							break;
						case Iap.CarrierPh:
							_insSub.BenefitNotes+="\r\n"+"CarrierPh: "+field;
							textPhone.Text=field;
							break;
						case Iap.Group://seems to be used as groupnum
							_insSub.BenefitNotes+="\r\n"+"Group: "+field;
							textGroupNum.Text=field;
							break;
						case Iap.MailTo://the carrier name again
							_insSub.BenefitNotes+="\r\n"+"MailTo: "+field;
							break;
						case Iap.MailTo2://address
							_insSub.BenefitNotes+="\r\n"+"MailTo2: "+field;
							textAddress.Text=field;
							break;
						case Iap.MailTo3://address2
							_insSub.BenefitNotes+="\r\n"+"MailTo3: "+field;
							textAddress2.Text=field;
							break;
						case Iap.EClaims:
							_insSub.BenefitNotes+="\r\n"+"EClaims: "+field;//this contains the PayorID at the end, but also a bunch of other drivel.
							int payorIDloc=field.LastIndexOf("Payor ID#:");
							if(payorIDloc!=-1 && field.Length>payorIDloc+10) {
								textElectID.Text=field.Substring(payorIDloc+10);
								_electIdCur=textElectID.Text;
							}
							break;
						case Iap.FAXClaims:
							_insSub.BenefitNotes+="\r\n"+"FAXClaims: "+field;
							break;
						case Iap.DMOOption:
							_insSub.BenefitNotes+="\r\n"+"DMOOption: "+field;
							break;
						case Iap.Medical:
							_insSub.BenefitNotes+="\r\n"+"Medical: "+field;
							break;
						case Iap.GroupNum://not used.  They seem to use the group field instead
							_insSub.BenefitNotes+="\r\n"+"GroupNum: "+field;
							break;
						case Iap.Phone2://?
							_insSub.BenefitNotes+="\r\n"+"Phone2: "+field;
							break;
						case Iap.Deductible:
							_insSub.BenefitNotes+="\r\n"+"Deductible: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Deductible;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								benefit.PlanNum=_insPlan.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefits.Add(benefit.Copy());
							}
							break;
						case Iap.FamilyDed:
							_insSub.BenefitNotes+="\r\n"+"FamilyDed: "+field;
							break;
						case Iap.Maximum:
							_insSub.BenefitNotes+="\r\n"+"Maximum: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Limitations;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
								benefit.PlanNum=_insPlan.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefits.Add(benefit.Copy());
							}
							break;
						case Iap.BenefitYear://text is too complex to parse
							_insSub.BenefitNotes+="\r\n"+"BenefitYear: "+field;
							break;
						case Iap.DependentAge://too complex to parse
							_insSub.BenefitNotes+="\r\n"+"DependentAge: "+field;
							break;
						case Iap.Preventive:
							_insSub.BenefitNotes+="\r\n"+"Preventive: "+field;
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
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							break;
						case Iap.Basic:
							_insSub.BenefitNotes+="\r\n"+"Basic: "+field;
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
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							benefit=new Benefit();
							benefit.BenefitType=InsBenefitType.CoInsurance;
							benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							break;
						case Iap.Major:
							_insSub.BenefitNotes+="\r\n"+"Major: "+field;
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
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							break;
						case Iap.InitialPlacement:
							_insSub.BenefitNotes+="\r\n"+"InitialPlacement: "+field;
							break;
						case Iap.ExtractionClause:
							_insSub.BenefitNotes+="\r\n"+"ExtractionClause: "+field;
							break;
						case Iap.Replacement:
							_insSub.BenefitNotes+="\r\n"+"Replacement: "+field;
							break;
						case Iap.Other:
							_insSub.BenefitNotes+="\r\n"+"Other: "+field;
							break;
						case Iap.Orthodontics:
							_insSub.BenefitNotes+="\r\n"+"Orthodontics: "+field;
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
							benefit.PlanNum=_insPlan.PlanNum;
							benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
							benefit.Percent=percent;
							_listBenefits.Add(benefit.Copy());
							break;
						case Iap.Deductible2:
							_insSub.BenefitNotes+="\r\n"+"Deductible2: "+field;
							break;
						case Iap.Maximum2://ortho Max
							_insSub.BenefitNotes+="\r\n"+"Maximum2: "+field;
							if(field.StartsWith("$")) {
								stringArraySplitField=field.Split(new char[] { ' ' });
								benefit=new Benefit();
								benefit.BenefitType=InsBenefitType.Limitations;
								benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
								benefit.PlanNum=_insPlan.PlanNum;
								benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
								benefit.MonetaryAmt=PIn.Double(stringArraySplitField[0].Remove(0,1));//removes the $
								_listBenefits.Add(benefit.Copy());
							}
							break;
						case Iap.PymtSchedule:
							_insSub.BenefitNotes+="\r\n"+"PymtSchedule: "+field;
							break;
						case Iap.AgeLimit:
							_insSub.BenefitNotes+="\r\n"+"AgeLimit: "+field;
							break;
						case Iap.SignatureonFile:
							_insSub.BenefitNotes+="\r\n"+"SignatureonFile: "+field;
							break;
						case Iap.StandardADAForm:
							_insSub.BenefitNotes+="\r\n"+"StandardADAForm: "+field;
							break;
						case Iap.CoordinationRule:
							_insSub.BenefitNotes+="\r\n"+"CoordinationRule: "+field;
							break;
						case Iap.CoordinationCOB:
							_insSub.BenefitNotes+="\r\n"+"CoordinationCOB: "+field;
							break;
						case Iap.NightguardsforBruxism:
							_insSub.BenefitNotes+="\r\n"+"NightguardsforBruxism: "+field;
							break;
						case Iap.OcclusalAdjustments:
							_insSub.BenefitNotes+="\r\n"+"OcclusalAdjustments: "+field;
							break;
						case Iap.XXXXXX:
							_insSub.BenefitNotes+="\r\n"+"XXXXXX: "+field;
							break;
						case Iap.TMJNonSurgical:
							_insSub.BenefitNotes+="\r\n"+"TMJNonSurgical: "+field;
							break;
						case Iap.Implants:
							_insSub.BenefitNotes+="\r\n"+"Implants: "+field;
							break;
						case Iap.InfectionControl:
							_insSub.BenefitNotes+="\r\n"+"InfectionControl: "+field;
							break;
						case Iap.Cleanings:
							_insSub.BenefitNotes+="\r\n"+"Cleanings: "+field;
							break;
						case Iap.OralEvaluation:
							_insSub.BenefitNotes+="\r\n"+"OralEvaluation: "+field;
							break;
						case Iap.Fluoride1200s:
							_insSub.BenefitNotes+="\r\n"+"Fluoride1200s: "+field;
							break;
						case Iap.Code0220:
							_insSub.BenefitNotes+="\r\n"+"Code0220: "+field;
							break;
						case Iap.Code0272_0274:
							_insSub.BenefitNotes+="\r\n"+"Code0272_0274: "+field;
							break;
						case Iap.Code0210:
							_insSub.BenefitNotes+="\r\n"+"Code0210: "+field;
							break;
						case Iap.Code0330:
							_insSub.BenefitNotes+="\r\n"+"Code0330: "+field;
							break;
						case Iap.SpaceMaintainers:
							_insSub.BenefitNotes+="\r\n"+"SpaceMaintainers: "+field;
							break;
						case Iap.EmergencyExams:
							_insSub.BenefitNotes+="\r\n"+"EmergencyExams: "+field;
							break;
						case Iap.EmergencyTreatment:
							_insSub.BenefitNotes+="\r\n"+"EmergencyTreatment: "+field;
							break;
						case Iap.Sealants1351:
							_insSub.BenefitNotes+="\r\n"+"Sealants1351: "+field;
							break;
						case Iap.Fillings2100:
							_insSub.BenefitNotes+="\r\n"+"Fillings2100: "+field;
							break;
						case Iap.Extractions:
							_insSub.BenefitNotes+="\r\n"+"Extractions: "+field;
							break;
						case Iap.RootCanals:
							_insSub.BenefitNotes+="\r\n"+"RootCanals: "+field;
							break;
						case Iap.MolarRootCanal:
							_insSub.BenefitNotes+="\r\n"+"MolarRootCanal: "+field;
							break;
						case Iap.OralSurgery:
							_insSub.BenefitNotes+="\r\n"+"OralSurgery: "+field;
							break;
						case Iap.ImpactionSoftTissue:
							_insSub.BenefitNotes+="\r\n"+"ImpactionSoftTissue: "+field;
							break;
						case Iap.ImpactionPartialBony:
							_insSub.BenefitNotes+="\r\n"+"ImpactionPartialBony: "+field;
							break;
						case Iap.ImpactionCompleteBony:
							_insSub.BenefitNotes+="\r\n"+"ImpactionCompleteBony: "+field;
							break;
						case Iap.SurgicalProceduresGeneral:
							_insSub.BenefitNotes+="\r\n"+"SurgicalProceduresGeneral: "+field;
							break;
						case Iap.PerioSurgicalPerioOsseous:
							_insSub.BenefitNotes+="\r\n"+"PerioSurgicalPerioOsseous: "+field;
							break;
						case Iap.SurgicalPerioOther:
							_insSub.BenefitNotes+="\r\n"+"SurgicalPerioOther: "+field;
							break;
						case Iap.RootPlaning:
							_insSub.BenefitNotes+="\r\n"+"RootPlaning: "+field;
							break;
						case Iap.Scaling4345:
							_insSub.BenefitNotes+="\r\n"+"Scaling4345: "+field;
							break;
						case Iap.PerioPx:
							_insSub.BenefitNotes+="\r\n"+"PerioPx: "+field;
							break;
						case Iap.PerioComment:
							_insSub.BenefitNotes+="\r\n"+"PerioComment: "+field;
							break;
						case Iap.IVSedation:
							_insSub.BenefitNotes+="\r\n"+"IVSedation: "+field;
							break;
						case Iap.General9220:
							_insSub.BenefitNotes+="\r\n"+"General9220: "+field;
							break;
						case Iap.Relines5700s:
							_insSub.BenefitNotes+="\r\n"+"Relines5700s: "+field;
							break;
						case Iap.StainlessSteelCrowns:
							_insSub.BenefitNotes+="\r\n"+"StainlessSteelCrowns: "+field;
							break;
						case Iap.Crowns2700s:
							_insSub.BenefitNotes+="\r\n"+"Crowns2700s: "+field;
							break;
						case Iap.Bridges6200:
							_insSub.BenefitNotes+="\r\n"+"Bridges6200: "+field;
							break;
						case Iap.Partials5200s:
							_insSub.BenefitNotes+="\r\n"+"Partials5200s: "+field;
							break;
						case Iap.Dentures5100s:
							_insSub.BenefitNotes+="\r\n"+"Dentures5100s: "+field;
							break;
						case Iap.EmpNumberXXX:
							_insSub.BenefitNotes+="\r\n"+"EmpNumberXXX: "+field;
							break;
						case Iap.DateXXX:
							_insSub.BenefitNotes+="\r\n"+"DateXXX: "+field;
							break;
						case Iap.Line4://city state
							_insSub.BenefitNotes+="\r\n"+"Line4: "+field;
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
							_insSub.BenefitNotes+="\r\n"+"Note: "+field;
							break;
						case Iap.Plan://?
							_insSub.BenefitNotes+="\r\n"+"Plan: "+field;
							break;
						case Iap.BuildUps:
							_insSub.BenefitNotes+="\r\n"+"BuildUps: "+field;
							break;
						case Iap.PosteriorComposites:
							_insSub.BenefitNotes+="\r\n"+"PosteriorComposites: "+field;
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
			Carrier carrier=Carriers.GetCarrier(_insPlan.CarrierNum);
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
			DateTime dateToday=DateTime.Today;
			if(ODBuild.IsDebug()) {
				dateToday=new DateTime(1999,1,4);//TODO: Remove after Canadian claim certification is complete.
			}
			Relat relat=(Relat)comboRelationship.SelectedIndex;
			string patID=textPatID.Text;
			try {
				CanadianOutput.SendElegibility(clearinghouseClin,_patPlan.PatNum,_insPlan,dateToday,relat,patID,true,_insSub,false,FormCCDPrint.PrintCCD);
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
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlan.PlanNum);
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
			if(_insSub.BenefitNotes=="") {
				//try to find some other similar notes. Never includes the current subscriber.
				//List<long> samePlans=InsPlans.GetPlanNumsOfSamePlans(textEmployer.Text,textGroupName.Text,textGroupNum.Text,
				//	textDivisionNo.Text,textCarrier.Text,checkIsMedical.Checked,PlanCur.PlanNum,false);
				otherBenefitNote=InsSubs.GetBenefitNotes(_insPlan.PlanNum,_insSub.InsSubNum);
				if(otherBenefitNote=="") {
					MsgBox.Show(this,"No benefit note found.  Benefit notes are created when importing Trojan or IAP benefit information and are frequently read-only.  Store your own notes in the subscriber note instead.");
					return;
				}
				MsgBox.Show(this,"This plan does not have a benefit note, but a note was found for another subsriber of this plan.  You will be able to view this note, but not change it.");
			}
			using FormInsBenefitNotes formInsBenefitNotes=new FormInsBenefitNotes();
			if(_insSub.BenefitNotes!="") {
				formInsBenefitNotes.BenefitNotes=_insSub.BenefitNotes;
			}
			else {
				formInsBenefitNotes.BenefitNotes=otherBenefitNote;
			}
			formInsBenefitNotes.ShowDialog();
			if(formInsBenefitNotes.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(_insSub.BenefitNotes!="") {
				_insSub.BenefitNotes=formInsBenefitNotes.BenefitNotes;
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
			if(_carrier.CarrierNum==0 && !MsgBox.Show(this,MsgBoxButtons.YesNo,warningMsg)) {
				return;
			}
			//1. Delete Subscriber---------------------------------------------------------------------------------------------------
			//Can only do this if there are other subscribers present.  If this is the last subscriber, then it attempts to delete the plan itself, down below.
			if(textLinkedNum.Text!="0") {//Other subscribers are present.  
				if(_insSub==null) {//viewing from big list
					MsgBox.Show(this,"Subscribers must be removed individually before deleting plan.");//by dropping, then using this same delete button.
					return;
				}
				else {//Came into here through a patient.
					DateTime dateTimeSubChange=_insSub.SecDateTEdit;
					if(_patPlan!=null) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All patients attached to this subscription will be dropped and the subscription for this plan will be deleted. Continue?")) {
							return;
						}
					}
					//drop the plan


					//detach subscriber.
					try {
						InsSubs.Delete(_insSub.InsSubNum);//Checks dependencies first;  If none, deletes the inssub, claimprocs, patplans, and recomputes all estimates.
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					logText=Lan.g(this,"The subscriber")+" "+Patients.GetPat(_insSub.Subscriber).GetNameFLnoPref()+" "
						+Lan.g(this,"with the Subscriber ID")+" "+_insSub.SubscriberID+" "+Lan.g(this,"was deleted.");
					_hasDeleted=true;
					//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0,logText,_insPlan?.PlanNum??0,
						_insPlan.SecDateTEdit);
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
			DateTime dateTimePrevious=_insPlan.SecDateTEdit;
			try {
				InsPlans.Delete(_insPlan);//Checks dependencies first;  If none, deletes insplan, inssub, benefits, claimprocs, patplans, and recomputes all estimates.
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_insPlan.CarrierNum).CarrierName+" "+Lan.g(this,"was deleted.");
			_hasDeleted=true;
			//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0,logText,_insPlan?.PlanNum??0,
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
			if(_carrier.CarrierNum==0 && !MsgBox.Show(this,MsgBoxButtons.YesNo,warningMsg)) {
				return false;
			}
			//should we save the plan info first?  Probably not.
			//--
			//If they have a claim for this ins with today's date, don't let them drop.
			//We already have code in place to delete claimprocs when we drop ins, but the claimprocs attached to claims are protected.
			//The claim clearly needs to be deleted if they are dropping.  We need the user to delete the claim before they drop the plan.
			//We also have code in place to add new claimprocs when they add the correct insurance.
			List<Claim> listClaims=Claims.Refresh(_patPlan.PatNum);
			for(int j=0;j<listClaims.Count;j++) {
				if(listClaims[j].PlanNum!=_insPlan.PlanNum) {//different insplan
					continue;
				}
				if(listClaims[j].DateService!=DateTime.Today) {//not today
					continue;
				}
				//Patient currently has a claim for the insplan they are trying to drop
				MsgBox.Show(this,"Please delete all of today's claims for this patient before dropping this plan.");
				return false;
			}
			PatPlans.Delete(_patPlan.PatPlanNum);//Estimates recomputed within Delete()
			//PlanCur.ComputeEstimatesForCur();
			_hasDropped=true;
			string logText=Lan.g(this,"The insurance plan for the carrier")+" "+Carriers.GetCarrier(_insPlan.CarrierNum).CarrierName+" "+Lan.g(this,"was dropped.");
			SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0,logText,_insPlan?.PlanNum??0,
				_insPlan.SecDateTEdit);
			InsEditPatLogs.MakeLogEntry(null,_patPlan,InsEditPatLogType.PatPlan);
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
			_listBenefits.Sort();
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
			for(int i=0;i<_listBenefits.Count;i++) {
				row=new GridRow();
				if(_listBenefits[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(_listBenefits[i].CoverageLevel==BenefitCoverageLevel.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",_listBenefits[i].CoverageLevel.ToString()));
				}
				if(_listBenefits[i].BenefitType==InsBenefitType.CoInsurance && _listBenefits[i].Percent != -1) {
					row.Cells.Add("%");
				}
				else if(_listBenefits[i].BenefitType==InsBenefitType.WaitingPeriod) {
					row.Cells.Add(Lan.g(this,"Waiting Period"));
				}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",_listBenefits[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(_listBenefits[i])); //already translated
				if(_listBenefits[i].Percent==-1 ) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listBenefits[i].Percent.ToString());
				}
				if(_listBenefits[i].MonetaryAmt == -1) {
					//if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.Deductible) {
					//	row.Cells.Add(((Benefit)benefitList[i]).MonetaryAmt.ToString("n0"));
					//}
					//else {
					row.Cells.Add("");
					//}
				}
				else {
					row.Cells.Add(_listBenefits[i].MonetaryAmt.ToString("n0"));
				}
				if(_listBenefits[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",_listBenefits[i].TimePeriod.ToString()));
				}
				if(_listBenefits[i].Quantity>0) {
					if(_listBenefits[i].QuantityQualifier==BenefitQuantity.NumberOfServices
						&&(_listBenefits[i].TimePeriod==BenefitTimePeriod.ServiceYear
						|| _listBenefits[i].TimePeriod==BenefitTimePeriod.CalendarYear))
					{
						row.Cells.Add(_listBenefits[i].Quantity.ToString()+" "+Lan.g(this,"times per year")+" ");
					}
					else if(_listBenefits[i].QuantityQualifier==BenefitQuantity.NumberOfServices 
						&& _listBenefits[i].TimePeriod==BenefitTimePeriod.NumberInLast12Months) 
					{
						row.Cells.Add(_listBenefits[i].Quantity.ToString()+" "+Lan.g(this,"times in the last 12 months")+" ");
					}
					else {
						row.Cells.Add(_listBenefits[i].Quantity.ToString()+" "
							+Lan.g("enumBenefitQuantity",_listBenefits[i].QuantityQualifier.ToString()));
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
			if(IsNewPlan && _insPlan.PlanNum != _insPlanOld.PlanNum) {  //If adding a new plan and picked existing plan from list
				//==Travis 05/06/2015:  Allowing users to edit insurance benefits for new plans that were picked from the list was causing problems with 
				//	duplicating benefits.  This was the fix we decided to go with, as the issue didn't seem to be affecting existing plans for a patient.
				MessageBox.Show(Lan.g(this,"You have picked an existing insurance plan and changes cannot be made to benefits until you have saved the plan for this new subscriber.")
					+"\r\n"+Lan.g(this,"To edit, click OK and then open the edit insurance plan window again."));
				return;
			}
			long patPlanNum=0;
			if(_patPlan!=null) {
				patPlanNum=_patPlan.PatPlanNum;
			}
			using FormInsBenefits formInsBenefits=new FormInsBenefits(_insPlan.PlanNum,patPlanNum);
			formInsBenefits.ListBenefits=_listBenefits;
			formInsBenefits.Note=textSubscNote.Text;
			formInsBenefits.MonthRenew=_insPlan.MonthRenew;
			formInsBenefits.InsSub_=_insSub;
			formInsBenefits.ShowDialog();
			if(formInsBenefits.DialogResult!=DialogResult.OK) {
				return;
			}
			FillBenefits();
			textSubscNote.Text=formInsBenefits.Note;
			_insPlan.MonthRenew=formInsBenefits.MonthRenew;
		}

		///<summary>Gets an employerNum based on the name entered. Called from FillCur</summary>
		private void GetEmployerNum() {
			if(_insPlan.EmployerNum==0) {//no employer was previously entered.
				if(textEmployer.Text=="") {
					//no change - Use what's in the database if they truly didn't change anything (PlanCur has no emp, text is blank, and text was always blank, they didn't switch insplans)
					if(_patPlan!=null && _employerNameOrig=="" && _insPlan.PlanNum==_insPlanOriginal.PlanNum) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_insPlan.EmployerNum=insPlanDB.EmployerNum;
						_insPlanOriginal.EmployerNum=insPlanDB.EmployerNum;
					}
				}
				else {
					_insPlan.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
			}
			else {//an employer was previously entered
				if(textEmployer.Text=="") {
					_insPlan.EmployerNum=0;
				}
				//if text has changed - 
				else if(_employerNameOrig!=textEmployer.Text) {
					_insPlan.EmployerNum=Employers.GetEmployerNum(textEmployer.Text);
				}
				else {
					//no change - Use what's in the database
					if(_patPlan!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_insPlan.EmployerNum=insPlanDB.EmployerNum;
						_insPlanOriginal.EmployerNum=insPlanDB.EmployerNum;
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
				Etrans etrans=x270Controller.RequestBenefits(clearinghouseClin,_insPlan,_patPlan.PatNum,_carrier,_insSub,out error);
				if(etrans != null) {
					//show the user a list of benefits to pick from for import--------------------------
					bool isDependentRequest=(_patPlan.PatNum!=_insSub.Subscriber);
					Carrier carrier=Carriers.GetCarrier(_insPlan.CarrierNum);
					using FormEtrans270Edit formEtrans270Edit=new FormEtrans270Edit(_patPlan.PatPlanNum,_insPlan.PlanNum,_insSub.InsSubNum,isDependentRequest,_insSub.Subscriber,carrier.IsCoinsuranceInverted);
					formEtrans270Edit.EtransCur=etrans;
					formEtrans270Edit.IsInitialResponse=true;
					formEtrans270Edit.ListBenefits=_listBenefits;
					if(formEtrans270Edit.ShowDialog()==DialogResult.OK) {
						EB271.SetInsuranceHistoryDates(formEtrans270Edit.ListEB271sImported,_patPlan.PatNum,_insSub);
						#region Plan Notes
						string patName=Patients.GetNameLF(_patPlan.PatNum);
						DateTime dateTimePlanEnd=DateTime.MinValue;
						List<DTP271> listDTP271Dates=formEtrans270Edit.ListDTP271s;
						for(int i=0;i<listDTP271Dates.Count;i++) {
							string dtpDateStr=DTP271.GetDateStr(listDTP271Dates[i].Segment.Get(2),listDTP271Dates[i].Segment.Get(3));
							if(listDTP271Dates[i].Segment.Get(1)=="347") {//347 => Plan End
								dateTimePlanEnd=X12Parse.ToDate(listDTP271Dates[i].Segment.Get(3));
								if(!isDependentRequest) {
									textDateTerm.Text=dtpDateStr;
								}
							}
							if(isDependentRequest || listDTP271Dates[i].Segment.Get(1)!="347"){
								string dtpDescript=DTP271.GetQualifierDescript(listDTP271Dates[i].Segment.Get(1));
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
								popup.PatNum=_patPlan.PatNum;
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
			DateTime dateTimeLast270=Etranss.GetLastDate270(_insPlan.PlanNum);
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
			using FormBenefitElectHistory formBenefitElectHistory=new FormBenefitElectHistory(_insPlan.PlanNum,_patPlan.PatPlanNum,_insSub.InsSubNum,_insSub.Subscriber,_insPlan.CarrierNum);
			formBenefitElectHistory.ListBenefits=_listBenefits;
			formBenefitElectHistory.ShowDialog();
			DateTime dateLast270=Etranss.GetLastDate270(_insPlan.PlanNum);
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
			if(table.Rows.Count == 0) {
				infoReceiverFirstName = "Unknown";
				infoReceiverLastName = "Unknown";
				TaxoCode = "Unknown";
			}
			else {
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
			table = Patients.GetPartialPatientData(_patPlan.PatNum);
			if(table.Rows.Count == 0) {
				xmlNodePatientFirstName.InnerText = "Unknown";
				xmlNodePatientLastName.InnerText = "Unknown";
				xmlNodePatientDOB.InnerText = "99/99/9999";
				RelationShip = "??";
				GenderCode = "?";
			}
			else {
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
			table=Patients.GetPartialPatientData2(_patPlan.PatNum);
			if(table.Rows.Count == 0) {
				xmlNodeSubscriberFirstName.InnerText = "Unknown";
				xmlNodeSubscriberLastName.InnerText = "Unknown";
				xmlNodeSubscriberDOB.InnerText = "99/99/9999";
				GenderCode = "?";
			}
			else {
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
			table=Providers.GetPrimaryProviders(_patPlan.PatNum);
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
					using(Form formDisplayEligibilityResponse = new FormEligibilityResponseDisplay(xmlDocument,_patPlan.PatNum)) {
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
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
			InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
			InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
			bool hasExistingEmployerChanged=(insPlanDB.CarrierNum!=0 && insPlanDB.EmployerNum!=_insPlan.EmployerNum && insPlanDB.PlanNum==_insPlan.PlanNum);//not new insplan and employer db not same as selection and insplan still used.
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
			PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
			InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
			InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);//Can have CarrierNum==0 if this is a new plan.
			bool hasExistingCarrierChanged=(insPlanDB.CarrierNum!=_carrier.CarrierNum && insPlanDB.CarrierNum!=0 && insPlanDB.CarrierNum!=_carrierNumOrig);
			if(_carrier.CarrierNum!=_carrierNumOrig && Carriers.Compare(carrierForm,_carrier)) {//Carrier was changed via "Pick From List" and not edited manually
				if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
					if(Carriers.GetCarrierDB(_carrier.CarrierNum)==null && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The Carrier selected has been combined or deleted since it was last picked.  Would you like to override those changes?")) {//Someone deleted/combined the carrier while the window was open.
						return false;
					}
					if(hasExistingCarrierChanged && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {//Someone changed this insplan's carrier while the window was open.
						return false;
					}
				}
				else {//Not authorized
					if(Carriers.GetCarrierDB(_carrier.CarrierNum)==null) {
						MsgBox.Show(this,"The selected insurance plan has had its carrier combined or deleted since it was last picked.  Please choose another.");
						return false;
					}
					if(hasExistingCarrierChanged) {//Someone changed this insplan's carrier while the window was open.
						MsgBox.Show(this,"The selected insurance plan has had its Carrier changed since it was loaded.  Please choose another.");
						return false;
					}
				}
			}
			else if(!Carriers.Compare(carrierForm,_carrier)) {//Carrier edited manually (doesn't matter if it was picked from list or not, user without perms can't edit manually)
				if(hasExistingCarrierChanged && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {//Someone changed this insplan's carrier while the window was open.
					return false;
				}
				//No need to look up if the carrier entered manually exists.  We can't tell if it doesn't exist or if it was deleted while the form was open.
				//If we look up a carrier using the info in the form, if it exists, then fine.  If it doesn't exist, is it because it was manually edited, or because someone else deleted it.
			}
			else if(_insPlanOld.PlanNum!=_insPlan.PlanNum) {//Plan was picked from list
				if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
					if(insPlanDB.CarrierNum!=_carrier.CarrierNum && !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected insurance plan has had its Carrier changed since it was loaded.  Would you like to override those changes?")) {
						return false;
					}
				}
				else {//Not authorized
					if(insPlanDB.CarrierNum!=_carrier.CarrierNum) {
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
			if(textSubscriberID.Text=="" && _insSub!=null) {
				MsgBox.Show(this,"Subscriber ID not allowed to be blank.");
				return false;
			}
			if(textCarrier.Text=="") {
				MsgBox.Show(this,"Carrier not allowed to be blank.");
				return false;
			}
			if(_patPlan!=null && !textOrdinal.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(comboRelationship.SelectedIndex==-1 && comboRelationship.Items.Count>0) {
				MsgBox.Show(this,"Relationship to Subscriber is not allowed to be blank.");
				return false;
			}
			if(_patPlan!=null && !IsEmployerValid()) {
				return false;
			}
			if(_patPlan!=null && !IsCarrierValid()) {
				return false;
			}
			if(_insSub!=null) {
				//Subscriber: Only changed when user clicks change button.
				_insSub.SubscriberID=textSubscriberID.Text;
				_insSub.DateEffective=PIn.Date(textDateEffect.Text);
				_insSub.DateTerm=PIn.Date(textDateTerm.Text);
				_insSub.ReleaseInfo=checkRelease.Checked;
				_insSub.AssignBen=checkAssign.Checked;
				_insSub.SubscNote=textSubscNote.Text;
				//MonthRenew already handled inside benefit window.
			}
			GetEmployerNum();
			_insPlan.GroupName=textGroupName.Text;
			_insPlan.GroupNum=textGroupNum.Text;
			_insPlan.RxBIN=textBIN.Text;
			_insPlan.DivisionNo=textDivisionNo.Text;//only visible in Canada
			//carrier-----------------------------------------------------------------------------------------------------
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//User has the ability to edit carrier information.  Check for matches, create new Carrier if applicable.
				Carrier carrierForm=GetCarrierFromForm();
				Carrier carrierOld=_carrier.Copy();
				if(_carrier.CarrierNum==_carrierNumOrig && Carriers.Compare(carrierForm,_carrier) && _insPlan.PlanNum==_insPlanOld.PlanNum) {
					//carrier is the same as it was originally, use what's in db if editing a patient's patplan.
					if(_patPlan!=null) {
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						InsPlan insPlanDB=InsPlans.GetPlan(insSubDB.PlanNum,null);
						_carrier=Carriers.GetCarrier(insPlanDB.CarrierNum);
						_insPlanOriginal.CarrierNum=_carrier.CarrierNum;
					}
					else {
						//Someone could have changed the insplan while the user was editing this window, do not overwrite the other users changes.
						InsPlan insPlanDB=InsPlans.GetPlan(_insPlan.PlanNum,null);
						if(insPlanDB.PlanNum==0) {
							MsgBox.Show(this,"Insurance plan has been combined or deleted since the window was opened.  Please press Cancel to continue and refresh the list of insurance plans.");
							return false;
						}
						_carrier=Carriers.GetCarrier(insPlanDB.CarrierNum);
						_insPlanOriginal.CarrierNum=_carrier.CarrierNum;
					}
				}
				else {
					_carrier=carrierForm;
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						bool carrierFound=true;
						try {
							_carrier=Carriers.GetIdentical(_carrier);
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
							formCarrierEdit.CarrierCur=_carrier;
							formCarrierEdit.ShowDialog();
							if(formCarrierEdit.DialogResult!=DialogResult.OK) {
								return false;
							}
						}
					}
					else {
						_carrier=Carriers.GetIdentical(_carrier,carrierOld: carrierOld);
					}
				}
				_insPlan.CarrierNum=_carrier.CarrierNum;
			}
			else {//User does not have permission to edit carrier information.  
				//We don't care if carrier info is changed, only if it's removed.  
				//If it's removed, have them choose another.  If it's simply changed, just use the same prikey.
				if(Carriers.GetCarrier(_carrier.CarrierNum).CarrierName=="" && _insPlan.PlanNum!=_insPlanOld.PlanNum) {//Carrier not found, it must have been deleted or combined
					MsgBox.Show(this,"Selected carrier has been combined or deleted.  Please choose another insurance plan.");
					return false;
				}
				else if(_insPlan.PlanNum==_insPlanOld.PlanNum) {//Didn't switch insplan, they were only viewing.
					long planNumDb=_insPlanOld.PlanNum;
					if(_patPlan!=null) {
						//Another user could have edited this patient's plan at the same time and could have changed something about the pat plan so we need
						//to go to the database an make sure that we are "not changing anything" by saving potentially stale data to the db.
						//If we don't do this, then we would end up overriding any changes that other users did while we were in this edit window.
						PatPlan patPlanDB=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
						InsSub insSubDB=InsSubs.GetOne(patPlanDB.InsSubNum);
						planNumDb=insSubDB.PlanNum;
					}
					InsPlan insPlanDB=InsPlans.GetPlan(planNumDb,null);
					_carrier=Carriers.GetCarrier(insPlanDB.CarrierNum);
					_insPlanOriginal.CarrierNum=_carrier.CarrierNum;
					_insPlan.CarrierNum=_carrier.CarrierNum;
				}
				else { 
					_insPlan.CarrierNum=_carrier.CarrierNum;
				}
			}
			//plantype already handled.
			if(comboClaimForm.SelectedIndex!=-1){
				_insPlan.ClaimFormNum=comboClaimForm.GetSelected<ClaimForm>().ClaimFormNum;
			}
			_insPlan.UseAltCode=checkAlternateCode.Checked;
			_insPlan.CodeSubstNone=checkCodeSubst.Checked;
			_insPlan.HasPpoSubstWriteoffs=checkPpoSubWo.Checked;
			_insPlan.IsMedical=checkIsMedical.Checked;
			_insPlan.ClaimsUseUCR=checkClaimsUseUCR.Checked;
			_insPlan.IsHidden=checkIsHidden.Checked;
			_insPlan.ShowBaseUnits=checkShowBaseUnits.Checked;
			if(comboFeeSched.SelectedIndex==0) {
				_insPlan.FeeSched=0;
			}
			else if(comboFeeSched.SelectedIndex == -1) {//Hidden fee schedule selected in comboFeeSched
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Fee Schedule has been hidden. Are you sure you want to continue?")) {
					return false;
				}
				_insPlan.FeeSched=_insPlanOriginal.FeeSched;
			}
			else{
				_insPlan.FeeSched=comboFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboCopay.SelectedIndex==0){
				_insPlan.CopayFeeSched=0;//none
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
				_insPlan.CopayFeeSched=_insPlanOriginal.CopayFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_insPlan.CopayFeeSched=comboCopay.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboOutOfNetwork.SelectedIndex==0){
				if(IsNewPlan
					&& _insPlan.PlanType==""//percentage
					&& PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.LegacyBlueBook){
					//add a fee schedule if needed
					FeeSched feeSched=FeeScheds.GetByExactName(_carrier.CarrierName,FeeScheduleType.OutNetwork);
					if(feeSched==null){
						feeSched=new FeeSched();
						feeSched.Description=_carrier.CarrierName;
						feeSched.FeeSchedType=FeeScheduleType.OutNetwork;
						//sched.IsNew=true;
						feeSched.IsGlobal=true;
						feeSched.ItemOrder=FeeScheds.GetCount();
						FeeScheds.Insert(feeSched);
						DataValid.SetInvalid(InvalidType.FeeScheds);
					}
					_insPlan.AllowedFeeSched=feeSched.FeeSchedNum;
				}
				else{
					_insPlan.AllowedFeeSched=0;
				}
			}
			else if(comboOutOfNetwork.SelectedIndex==-1) {//Hidden fee schedule selected in comboAllowedFeeSched
				if(comboOutOfNetwork.Enabled 
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Out Of Network fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_insPlan.AllowedFeeSched=_insPlanOriginal.AllowedFeeSched;//No change, maintain hidden feesched.
			}
			else{
				_insPlan.AllowedFeeSched=comboOutOfNetwork.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(comboManualBlueBook.SelectedIndex==-1) {
				if(comboManualBlueBook.Enabled
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected Manual Blue book fee schedule has been hidden. Are you sure you want to continue?"))
				{
					return false;
				}
				_insPlan.ManualFeeSchedNum=_insPlanOriginal.ManualFeeSchedNum;//No change, maintain hidden feesched.
			}
			else {
				_insPlan.ManualFeeSchedNum=comboManualBlueBook.GetSelected<FeeSched>().FeeSchedNum;
			}
			_insPlan.CobRule=(EnumCobRule)comboCobRule.SelectedIndex;
			//Canadian------------------------------------------------------------------------------------------
			_insPlan.DentaideCardSequence=PIn.Byte(textDentaide.Text);
			_insPlan.CanadianPlanFlag=textPlanFlag.Text;//validated
			_insPlan.CanadianDiagnosticCode=textCanadianDiagCode.Text;//validated
			_insPlan.CanadianInstitutionCode=textCanadianInstCode.Text;//validated
			//Canadian end---------------------------------------------------------------------------------------
			_insPlan.TrojanID=textTrojanID.Text;
			_insPlan.PlanNote=textPlanNote.Text;
			_insPlan.HideFromVerifyList=checkDontVerify.Checked;
			//Ortho----------------------------------------------------------------------------------------------
			OrthoClaimType[] OrthoClaimTypeArray=(OrthoClaimType[])Enum.GetValues(typeof(OrthoAutoProcFrequency));
			_insPlan.OrthoType=0;
			if(comboOrthoClaimType.SelectedIndex!=-1) {
				_insPlan.OrthoType=(OrthoClaimType)OrthoClaimTypeArray.GetValue(comboOrthoClaimType.SelectedIndex);
			}
			_insPlan.OrthoAutoProcCodeNumOverride=0;//overridden by practice default.
			if(_procedureCodeOrthoAuto!=null) {
				_insPlan.OrthoAutoProcCodeNumOverride=_procedureCodeOrthoAuto.CodeNum;
			}
			OrthoAutoProcFrequency[] orthoAutoProcFrequencyArray=(OrthoAutoProcFrequency[])Enum.GetValues(typeof(OrthoAutoProcFrequency));
			_insPlan.OrthoAutoProcFreq=0;
			if(comboOrthoAutoProcPeriod.SelectedIndex!=-1) {
				_insPlan.OrthoAutoProcFreq=(OrthoAutoProcFrequency)OrthoClaimTypeArray.GetValue(comboOrthoAutoProcPeriod.SelectedIndex);
			}
			_insPlan.OrthoAutoClaimDaysWait=0;
			if(checkOrthoWaitDays.Checked) {
				_insPlan.OrthoAutoClaimDaysWait=30;
			}
			_insPlan.OrthoAutoFeeBilled=PIn.Double(textOrthoAutoFee.Text);
			return true;
		}

		///<summary>Warns user if there are received claims for this plan.  Returns true if user wants to proceed, or if there are no received claims for this plan.  Returns false if the user aborts.</summary>
		private bool CheckForReceivedClaims() {
			//_patPlan will be null if editing insurance plans from Lists > Insurance Plans.
			long patNum=_patPlan?.PatNum??0;
			int claimCount=0;
			if(patNum==0) {//Editing insurance plans from Lists > Insurance Plans.
				//Check all claims for plan
				claimCount=Claims.GetCountReceived(_insPlanOriginal.PlanNum);
				if(claimCount!=0) {
					if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return false; //abort
					}
				}
			}
			else {//Editing insurance plans from Family module.
				if(radioChangeAll.Checked==true) {//Check radio button
					claimCount=Claims.GetCountReceived(_insPlanOriginal.PlanNum);
					if(claimCount!=0) {//Check all claims for plan
						if(MessageBox.Show(Lan.g(this,"There are")+" "+claimCount+" "+Lan.g(this,"received claims for this insurance plan that will have the carrier changed")+".  "+Lan.g(this,"You should NOT do this if the patient is changing insurance")+".  "+Lan.g(this,"Use the Drop button instead")+".  "+Lan.g(this,"Continue")+"?","",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false; //abort
						}
					}
				}
				else {//Check claims for plan and patient only
					claimCount=Claims.GetCountReceived(_insPlanOriginal.PlanNum,_patPlan.InsSubNum);
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
			_insPlan.IsBlueBookEnabled=checkUseBlueBook.Checked;
			SetAllowedFeeScheduleControls();
		}

		private void comboFeeSched_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_insPlan.PlanType=="" && comboFeeSched.SelectedIndex > 0) {
				checkUseBlueBook.Checked=false; // started with no fee sched, selected one
			}
			if(_insPlan.PlanType=="" && comboFeeSched.SelectedIndex <= 0) {
				checkUseBlueBook.Checked=true; // had a fee sched, selected none
			}
		}

		private void butSubstCodes_Click(object sender,EventArgs e) {
			InsPlan insPlanCurCopy=_insPlan.Copy();
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
			if(_patPlan==null) {
				return;
			}
			using FormInsEditPatLog formInsEditPatLog=new FormInsEditPatLog(_patPlan);
			formInsEditPatLog.ShowDialog();
		}

		private void butAudit_Click(object sender,EventArgs e) {
			if(IsPatPlanRemoved()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			GetEmployerNum();
			using FormInsEditLogs formInsEditLogs = new FormInsEditLogs(_insPlan,_listBenefits);
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
				defaultFee=_insPlan.OrthoAutoFeeBilled;
			}
			using FormOrthoPat formOrthoPat = new FormOrthoPat(_patPlan,_insPlan,carrierName,subID,defaultFee);
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
				&& (_insPlan==null || InsPlans.GetPlan(_insPlan.PlanNum,new List<InsPlan>())==null)) 
			{
				MsgBox.Show(this,"The selected insurance plan was removed by another user and no longer exists.  Open insurance plan again to edit.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_insSub!=null && InsPlans.GetPlan(_insSub.PlanNum,new List<InsPlan>())==null) {
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
			_insPlan.FilingCode=selectedFilingCodeNum;
			_insPlan.FilingCodeSubtype=0;
			if(comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>()!=null) {
				_insPlan.FilingCodeSubtype=comboFilingCodeSubtype.GetSelected<InsFilingCodeSubtype>().InsFilingCodeSubtypeNum;
			}
			#endregion Validation
			#region 1 - Validate Carrier Received Claims
			try {
				if(!FillPlanCurFromForm()) {//also fills SubCur if not null
					return;
				}
				if(_insPlan.CarrierNum!=_insPlanOriginal.CarrierNum) {
					string carrierNameOrig=Carriers.GetCarrier(_insPlanOriginal.CarrierNum).CarrierName;
					string carrierNameNew=Carriers.GetCarrier(_insPlan.CarrierNum).CarrierName;
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
				if(_insSubOld!=null) {//Editing an old plan for a subscriber.
					if(_insSubOld.AssignBen!=checkAssign.Checked) {
						if(!Security.IsAuthorized(Permissions.InsPlanChangeAssign)) {
							return;
						}
						//It is very possible that the user changed the patient associated to the ins sub.
						//We need to make a security log for the most recent patient (_subCur.Subscriber) instead of the original patient (_subOld.Subscriber) that was passed in.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeAssign,_insSub.Subscriber,Lan.g(this,"Assignment of Benefits (pay dentist) changed from")
							+" "+(_insSubOld.AssignBen?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" "
							+Lan.g(this,"to")
							+" "+(checkAssign.Checked?Lan.g(this,"checked"):Lan.g(this,"unchecked"))+" for plan "
							+Carriers.GetCarrier(_insPlan.CarrierNum).CarrierName);
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
				if(_patPlan!=null) {
					if(PIn.Long(textOrdinal.Text)!=_patPlan.Ordinal) {//Ordinal changed by user
						_patPlan.Ordinal=(byte)(PatPlans.SetOrdinal(_patPlan.PatPlanNum,PIn.Int(textOrdinal.Text)));
						_hasOrdinalChanged=true;
					}
					else if(PIn.Long(textOrdinal.Text)!=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum).Ordinal) {
						//PatPlan's ordinal changed by somebody else and not this user, set it to what's in the DB for this update.
						_patPlan.Ordinal=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum).Ordinal;
					}
					_patPlan.IsPending=checkIsPending.Checked;
					_patPlan.Relationship=(Relat)comboRelationship.SelectedIndex;
					_patPlan.PatID=textPatID.Text;
					if(_patPlanOld!=null) {
						_patPlanOld.PatID=_patPlanOld.PatID??"";
					}
					PatPlans.Update(_patPlan,_patPlanOld);
					if(!PIn.Date(textDateLastVerifiedPatPlan.Text).Date.Equals(_dateTimePatPlanLastVerified.Date)) {
						InsVerify insVerify=InsVerifies.GetOneByFKey(_patPlan.PatPlanNum,VerifyTypes.PatientEnrollment);
						if(insVerify!=null) {
							insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedPatPlan.Text);
							InsVerifyHists.InsertFromInsVerify(insVerify);
						}
					}
					if(!IsNewPatPlan) {//Updated
						InsEditPatLogs.MakeLogEntry(_patPlan,_patPlanOld,InsEditPatLogType.PatPlan);
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
			if(_insSub!=null) {
				_insSub.PlanNum=_insPlan.PlanNum;
			}
			#endregion 3 - PatPlan
			//Sections 4 - 10 all deal with manipulating the insurance plan so make sure the user has permission to do so.
			#region InsPlan Edit
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				if(_insSub==null) {//editing from big list.  No subscriber.  'pick from list' button not visible, making logic easier.
					#region 4 - InsPlan Null Subscriber
					try {
						if(InsPlans.AreEqualValue(_insPlan,_insPlanOriginal)) {//If no changes
							//pick button doesn't complicate things.  Simply nothing to do.  Also, no SubCur, so just close the form.
							DialogResult=DialogResult.OK;
						}
						else {//changes were made
							InsPlans.Update(_insPlan);
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
						if(InsPlans.AreEqualValue(_insPlan,_insPlanOriginal)) {//New plan, no changes
							#region 5 - InsPlan Non-Null Subscriber, New Plan, No Changes Made
							//If the logic in this region changes, then also change region 5a below.
							try {
								if(_insPlan.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button
									//No need to update PlanCur because no changes, delete original plan.
									try {
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(_patPlan.PatNum,_insPlanOld.PlanNum,_insPlan.PlanNum);
										}
										//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
										InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
										_listBenefitsOld=new List<Benefit>();
										removeLogs=true;
									}
									catch(ApplicationException ex) {
										MessageBox.Show(ex.Message);
										//do not need to update PlanCur because no changes were made.
										SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0
											,Lan.g(this,"FormInsPlan region 5 delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,
											DateTime.MinValue); //new plan, no date needed.
										Close();
										return;
									}
									_insSub.PlanNum=_insPlan.PlanNum;
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
							if(_insPlan.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								//If the logic in this region changes, then also change region 6a below.
								try {
									if(radioChangeAll.Checked) {
										InsPlans.Update(_insPlan);//they might not realize that they would be changing an existing plan. Oh well.
										try {
											if(_didAddInsHistCP) {
												//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
												ClaimProcs.UpdatePlanNumForInsHist(_patPlan.PatNum,_insPlanOld.PlanNum,_insPlan.PlanNum);
											}
											//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
											InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
											_listBenefitsOld=new List<Benefit>();
											removeLogs=true;
										}
										catch(ApplicationException ex) {
											MessageBox.Show(ex.Message);
											SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0
												,Lan.g(this,"FormInsPlan region 6 delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,
												DateTime.MinValue); //new plan, no date needed.
											Close();
											return;
										}
										_insSub.PlanNum=_insPlan.PlanNum;
									}
									else {//option is checked for "create new plan if needed"
										_insPlan.PlanNum=_insPlanOld.PlanNum;
										InsPlans.Update(_insPlan);
										_insSub.PlanNum=_insPlan.PlanNum;
										//no need to update PatPlan.  Same old PlanNum.  When 'pick from list' button was pushed, benfitList was filled with benefits from
										//the picked plan.  benefitListOld was not touched and still contains the old benefits.  So the original benefits will be
										//automatically deleted.  We force copies to be made in the database, but with different PlanNums.  Any other changes will be preserved.
										for(int i = 0;i<_listBenefits.Count;i++) {
											if(_listBenefits[i].PlanNum>0) {
												_listBenefits[i].PlanNum=_insPlan.PlanNum;
												_listBenefits[i].BenefitNum=0;//triggers insert during synch.
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
								InsPlans.Update(_insPlan);
							}
							#endregion 6 - InsPlan Non-Null Subscriber, New Plan, Changes Made
						}//end else of if(InsPlans.AreEqual...
					}//end if(IsNewPlan)
					else {//editing an existing plan from within patient
						if(InsPlans.AreEqualValue(_insPlan,_insPlanOriginal)) {//If no changes
							#region 7 - InsPlan Non-Null Subscriber, Not a New Plan, No Changes Made
							try {
								if(_insPlan.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, then made no changes
									//do not need to update PlanCur because no changes were made.
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(_patPlan.PatNum,_insPlanOld.PlanNum,_insPlan.PlanNum);
									}
									_insSub.PlanNum=_insPlan.PlanNum;
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
							if(_insPlan.PlanNum != _insPlanOld.PlanNum) {//clicked 'pick from list' button, and then made changes
								if(radioChangeAll.Checked) {
									#region 8 - InsPlan Non-Null Subscriber, Not a New Plan, Pick From List, Changes Made, Change All Checked
									try {
										//warn user here?
										if(_didAddInsHistCP) {
											//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
											ClaimProcs.UpdatePlanNumForInsHist(_patPlan.PatNum,_insPlanOld.PlanNum,_insPlan.PlanNum);
										}
										InsPlans.Update(_insPlan);
										_insSub.PlanNum=_insPlan.PlanNum;
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
											InsPlans.Update(_insPlan);
											_insSub.PlanNum=_insPlan.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//So if any benefits were changed, the synch further down will trigger updates for the benefits on the picked plan.
										}
										else {//if there are other subscribers
											InsPlans.Insert(_insPlan);//this gives it a new primary key.
											_insSub.PlanNum=_insPlan.PlanNum;
											//When 'pick from list' button was pushed, benefitListOld was filled with a shallow copy of the benefits from the picked list.
											//We must clear the benefitListOld to prevent deletion of those benefits.
											_listBenefitsOld=new List<Benefit>();
											//Force copies to be made in the database, but with different PlanNum;
											for(int i = 0;i<_listBenefits.Count;i++) {
												if(_listBenefits[i].PlanNum>0) {
													_listBenefits[i].PlanNum=_insPlan.PlanNum;
													_listBenefits[i].BenefitNum=0;//triggers insert during synch.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_insPlan.PlanNum,_insPlanOld.PlanNum);
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
										InsPlans.Update(_insPlan);
									}
									else {//option is checked for "create new plan if needed"
										if(textLinkedNum.Text=="0") {//if this is the only subscriber
											InsPlans.Update(_insPlan);
										}
										else {//if there are other subscribers
											InsPlans.Insert(_insPlan);//this gives it a new primary key.
											_insSub.PlanNum=_insPlan.PlanNum;
											//PatPlanCur.PlanNum=PlanCur.PlanNum;
											//PatPlans.Update(PatPlanCur);
											//make copies of all the benefits
											_listBenefitsOld=new List<Benefit>();
											for(int i = 0;i<_listBenefits.Count;i++) {
												if(_listBenefits[i].PlanNum>0) {
													_listBenefits[i].PlanNum=_insPlan.PlanNum;
													_listBenefits[i].BenefitNum=0;//triggers insert.
												}
											}
											//Insert new sub links for the new insurance plan created above. This will maintain the sub links of the old insplan. 
											SubstitutionLinks.CopyLinksToNewPlan(_insPlan.PlanNum,_insPlanOld.PlanNum);
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
				if(_insSub!=null) {
					if(IsNewPlan) {
						#region 5a - User Without Permissions, InsPlan Non-Null Subscriber, New Plan
						//If the logic in this region changes, then also change region 5 above.
						try {
							if(_insPlan.PlanNum != _insPlanOld.PlanNum) {//user clicked the "pick from list" button. 
								//In a previous version, a user could still change some things about the plan even if they had no permissions to do so.
								//This was causing empty insurance plans to get saved to the db.
								//Even if they somehow managed to change something about the insurance plan they picked, we always just want to do the following:
								//1. Update the inssub to be the current insplan. (which happens above)
								//2. Delete the empty insurance plan (which happens here)
								try {
									if(_didAddInsHistCP) {
										//Need to update InsHist with new PlanNum since user clicked 'pick from list' button
										ClaimProcs.UpdatePlanNumForInsHist(_patPlan.PatNum,_insPlanOld.PlanNum,_insPlan.PlanNum);
									}
									//does dependency checking, throws if dependencies exist. the inssub should NOT be deleted as it is used below.
									InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);
									_listBenefitsOld=new List<Benefit>();
									removeLogs=true;
								}
								catch(ApplicationException ex) {
									MessageBox.Show(ex.Message);
									SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0
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
					InsVerify insVerify=InsVerifies.GetOneByFKey(_insPlan.PlanNum,VerifyTypes.InsuranceBenefit);
					insVerify.DateLastVerified=PIn.Date(textDateLastVerifiedBenefits.Text);
					InsVerifyHists.InsertFromInsVerify(insVerify);
				}
				//PatPlanCur.InsSubNum is already set before opening this window.  There is no possible way to change it from within this window.  Even if PlanNum changes, it's still the same inssub.  And even if inssub.Subscriber changes, it's still the same inssub.  So no change to PatPlanCur.InsSubNum is ever require from within this window.
				if(_listBenefitsOld.Count>0 || _listBenefits.Count>0) {//Synch benefits
					Benefits.UpdateList(_listBenefitsOld,_listBenefits);
				}
				if(removeLogs) {
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_insPlanOld.PlanNum);
				}
				if(_insSub!=null) {//Update SubCur if needed
					InsSubs.Update(_insSub);//also saves the other fields besides PlanNum
					if(_insSubOld.Subscriber!=_insSub.Subscriber) {//If the subscriber was changed, include an audit trail entry
						Dictionary<long,string> dictPatNames=Patients.GetPatientNames(new List<long>() { _insSubOld.Subscriber,_insSub.Subscriber });
						//PatPlanCur will be null if editing insurance plans from Lists > Insurance Plans.
						//However, the Change button is invisible from List > Insurance Plans, so we can count on PatPlanCur not null.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,_patPlan.PatNum,
							Lan.g(this,"Subscriber Changed from")+" "+dictPatNames[_insSubOld.Subscriber]+" #"+_insSubOld.Subscriber+" "
							+Lan.g(this,"to")+" "+dictPatNames[_insSub.Subscriber]+" #"+_insSub.Subscriber);
					}
					//Udate all claims, claimprocs, payplans, and etrans that are pointing at the inssub.InsSubNum since it may now be pointing at a new insplan.PlanNum.
					InsSubs.SynchPlanNumsForNewPlan(_insSub);
					InsPlans.ComputeEstimatesForSubscriber(_insSub.Subscriber);
					InsEditPatLogs.MakeLogEntry(_insSub,_insSubOld,InsEditPatLogType.Subscriber);
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
				Carrier carrierOrig=Carriers.GetCarrier(_insPlanOriginal.CarrierNum);
				Carrier carrierNew=Carriers.GetCarrier(_insPlan.CarrierNum);
				if(_insPlan.CarrierNum!=_insPlanOriginal.CarrierNum) {
					_hasCarrierChanged=true;
					//_patPlan will be null if editing insurance plans from Lists > Insurance Plans.				
					string carrierNameOrig=carrierOrig.CarrierName;
					string carrierNameNew=carrierNew.CarrierName;
					if(carrierNameOrig!=carrierNameNew) {//The CarrierNum could have changed but the CarrierName might not have changed.  Only make an audit entry if the name changed.
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeCarrierName,_patPlan?.PatNum??0,Lan.g(this,"Carrier name changed in Edit Insurance Plan window from")+" "
							+(string.IsNullOrEmpty(carrierNameOrig)?"blank":carrierNameOrig)+" "+Lan.g(this,"to")+" "
							+(string.IsNullOrEmpty(carrierNameNew)?"blank":carrierNameNew),_insPlan.PlanNum,_insPlanOriginal.SecDateTEdit);
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
				Carrier carrierCur=Carriers.GetCarrier(_insPlan.CarrierNum);
				if(_insPlanOriginal.FeeSched!=0 && _insPlanOriginal.FeeSched!=_insPlan.FeeSched) {
					string feeSchedOld=FeeScheds.GetDescription(_insPlanOriginal.FeeSched);
					string feeSchedNew=FeeScheds.GetDescription(_insPlan.FeeSched);
					string logText=Lan.g(this,"The fee schedule associated with insurance plan number")+" "+_insPlan.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"was changed from")+" "+feeSchedOld+" "+Lan.g(this,"to")+" "+feeSchedNew;
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0,logText,_insPlan?.PlanNum??0,
						_insPlanOriginal.SecDateTEdit);
				}
				if(InsPlanCrud.UpdateComparison(_insPlanOriginal,_insPlan)) {
					string logText=Lan.g(this,"Insurance plan")+" "+_insPlan.PlanNum.ToString()+" "+Lan.g(this,"for the carrier")+" "+carrierCur.CarrierName+" "+Lan.g(this,"has changed.");
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0,logText,_insPlan?.PlanNum??0,
						_insPlanOriginal.SecDateTEdit);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error Code 13")+".  "+Lan.g(this,"Please contact support")+"\r\n"+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				return;
			}
			#endregion Carrier FeeSched
			//InsBlueBook entries should only exist for category percentage plans.
			//Delete if no longer blue book eligible, otherwise update.
			if(_insPlan.PlanType!="" || !_insPlan.IsBlueBookEnabled) {
				InsBlueBooks.DeleteByPlanNums(_insPlan.PlanNum);
			}
			else {
				InsBlueBooks.UpdateByInsPlan(_insPlan);
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
			if(IsNewPlan && _insPlanOld.PlanNum==_insPlanOriginal.PlanNum) {//Never have to warn user because the plan is new and was not picked from the list.
				return false;
			}
			if(_insPlanOriginal.PlanType!="") {//Not Category Percentage plan so not bluebook eligible regardless if fee schedule changes.
				return false;
			}
			if((_insPlanOriginal.IsBlueBookEnabled && !checkUseBlueBook.Checked)	|| (_insPlanOriginal.PlanType=="" && _insPlan.PlanType!="")) {
				//Warn user because we have changed to use a fee schedule or from Category Percentage plan to a non-bluebook eligible plan.
				return true;
			}
			return false;
		}

		private void FormInsPlan_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				if(_patPlan!=null && (_hasDropped || _hasOrdinalChanged || _hasCarrierChanged || IsNewPatPlan || IsNewPlan || _hasDeleted || 
					_insPlan.IsMedical!=_insPlanOld.IsMedical)) 
				{
					Appointments.UpdateInsPlansForPat(_patPlan.PatNum);
				}
				if(IsNewPatPlan//Only when assigning new insurance
					&& _patPlan.Ordinal==1//Primary insurance.
					&& _insPlan.BillingType!=0//Selection made.
					&& Security.IsAuthorized(Permissions.PatientBillingEdit,true)
					&& PrefC.GetBool(PrefName.PatInitBillingTypeFromPriInsPlan))
				{
					Patient patient=Patients.GetPat(_patPlan.PatNum);
					if(patient.BillingType!=_insPlan.BillingType) {
						Patient patNew=patient.Copy();
						patNew.BillingType=_insPlan.BillingType;
						Patients.Update(patNew,patient);
						//This needs to be the last call due to automation possibily leaving the form in a closing limbo.
						AutomationL.Trigger(AutomationTrigger.SetBillingType,null,patNew.PatNum);
						Patients.InsertBillTypeChangeSecurityLogEntry(patient,patNew);
					}
				}
				if(_insSub!=null && _hasDeleted) {
					List<PatPlan> listPatPlansForSub=PatPlans.GetListByInsSubNums(new List<long> { _insSub.InsSubNum });
					for(int i=0;i<listPatPlansForSub.Count;i++) {
						Appointments.UpdateInsPlansForPat(listPatPlansForSub[i].PatNum);
					}
				}
				return;
			}
			//So, user cancelled a new entry
			if(IsNewPlan){//this would also be new coverage
				//warning: If user clicked 'pick from list' button, then we don't want to delete an existing plan used by others
				try {
					if(_insSub!=null) {
						InsSubs.Delete(_insSub.InsSubNum);
					}
					InsPlans.Delete(_insPlanOld,canDeleteInsSub:false,doInsertInsEditLogs:false);//does dependency checking.
					InsEditLogs.DeletePreInsertedLogsForPlanNum(_insPlanOld.PlanNum);
					//Ok to delete these adjustments because we deleted the benefits in Benefits.DeleteForPlan().
					ClaimProcs.DeleteMany(_arrayListAdj.ToArray().Cast<ClaimProc>().ToList());
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					SecurityLogs.MakeLogEntry(Permissions.InsPlanEdit,_patPlan?.PatNum??0
						,Lan.g(this,"FormInsPlan_Closing delete validation failed.  Plan was not deleted."),_insPlanOld.PlanNum,DateTime.MinValue);//new plan, no date needed.
					return;
				}
			}
			if(IsNewPatPlan){
				PatPlans.Delete(_patPlan.PatPlanNum);//no need to check dependencies.  Maintains ordinals and recomputes estimates.
			}
		}

		///<summary>Check if PatPlan was dropped since window was opened.</summary>
		private bool IsPatPlanRemoved() {
			if(_patPlan!=null) {
				PatPlan patPlanExists=PatPlans.GetByPatPlanNum(_patPlan.PatPlanNum);
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
