using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormInsBenefits : FormODBase {
		///<summary>Set this externally before opening the form.</summary>
		public InsSub InsSub_;
		///<summary>Set this externally before opening the form. Provide a deep copy of the benefits.</summary>
		public List<Benefit> ListBenefits;
		///<summary>Set this externally before opening the form. 0 indicates calendar year, otherwise 1-12. This forces all benefits to conform to this setting. User can change as a whole.</summary>
		public byte MonthRenew;
		///<summary>Set this externally before opening the form. The subscriber note.</summary>
		public string Note;
		private long _planNum;
		private long _patPlanNum;
		///<summary>A shallow list of all benefits passed into this form along with new benefits added by the user. Some of the benefits in this list will be displayed within the simple view and the rest will be displayed within the grid. This list gets recreated sometimes by starting with the list of benefits showing in the grid and then making new benefit objects out of the current state of the simple view UI.</summary>
		private List<Benefit> _listBenefitsAll;
		///<summary>A shallow, subset list of the benefits that were passed into this form. These benefits are the ones displaying within the grid on this form.</summary>
		private List<Benefit> _listBenefitsGrid;
		private bool _dontAllowSimplified;
		private bool _isResettingQuickText=false;

		///<summary></summary>
		public FormInsBenefits(long planNum,long patPlanNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patPlanNum=patPlanNum;
			_planNum=planNum;
		}

		private void FormInsBenefits_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				butOK.Enabled=false;
				butAdd.Enabled=false;
				butDelete.Enabled=false;
			}
			comboBW.Items.AddEnums<FrequencyOptions>();
			comboPano.Items.AddEnums<FrequencyOptions>();
			comboExams.Items.AddEnums<FrequencyOptions>();
			_listBenefitsAll=new List<Benefit>(ListBenefits);
			if(CovCats.GetForEbenCat(EbenefitCategory.Accident)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Crowns)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Endodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.General)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.MaxillofacialProsth)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.OralSurgery)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Orthodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Periodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.Restorative)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)==null
				|| CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay)==null
				|| Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)
				)
			{
				_dontAllowSimplified=true;
				checkSimplified.Checked=false;
				panelSimple.Visible=false;
				LayoutManager.MoveLocation(gridBenefits,new Point(gridBenefits.Left,groupYear.Bottom+3));
				LayoutManager.MoveHeight(gridBenefits,butAdd.Top-gridBenefits.Top-5);
			}
			FillCalendarYear();
			FillSimple();
			FillGrid();
			textSubscNote.Text=Note;
			if(InsSub_==null) {
				textSubscNote.Visible=false;
				labelSubscNote.Visible=false;
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkSimplified.Checked=false;
				checkSimplified.Visible=false;
			}
		}

		private void checkSimplified_Click(object sender,EventArgs e) {
			if(checkSimplified.Checked) {
				if(_dontAllowSimplified) {
					checkSimplified.Checked=false;
					MsgBox.Show(this,"Not allowed to use simplified view until you fix your Insurance Categories from the setup menu.  At least one of each e-benefit category must be present.");
					return;
				}
				gridBenefits.Title=Lan.g(this,"Other Benefits");
				panelSimple.Visible=true;
				groupCategories.Visible=true;
				LayoutManager.MoveLocation(gridBenefits,new Point(gridBenefits.Left,panelSimple.Bottom+4));
				LayoutManager.MoveHeight(gridBenefits,textSubscNote.Top-gridBenefits.Top-5);
				LayoutManager.MoveLocation(butAdd,new Point(gridBenefits.Right+5,gridBenefits.Top));
				LayoutManager.MoveLocation(butDelete,new Point(butAdd.Left,butAdd.Bottom+6));
				//FillSimple handles all further logic.
				FillSimple();
				FillGrid();
				return;
			}
			//Not checked
			if(!ConvertFormToBenefits()) {
				checkSimplified.Checked=true;
				return;
			}
			gridBenefits.Title=Lan.g(this,"Benefits");
			panelSimple.Visible=false;
			groupCategories.Visible=false;
			LayoutManager.MoveLocation(gridBenefits,new Point(gridBenefits.Left,groupYear.Bottom+3));
			LayoutManager.MoveHeight(gridBenefits,textSubscNote.Top-gridBenefits.Top-5);
			LayoutManager.MoveLocation(butAdd,new Point(gridBenefits.Right+5,gridBenefits.Top));
			LayoutManager.MoveLocation(butDelete,new Point(butAdd.Left,butAdd.Bottom+6));
			FillSimple();
			FillGrid();
		}

		///<summary>This will only be run when the form first opens or if user switches to simple view.  FillGrid should always be run after this.</summary>
		private void FillSimple() {
			if(!panelSimple.Visible) {
				//Show all of the benefits when the simple view is not showing to the user.
				_listBenefitsGrid=new List<Benefit>(_listBenefitsAll);
				return;
			}
			#region Reset UI
			textAnnualMax.Text="";
			textDeductible.Text="";
			textAnnualMaxFam.Text="";
			textDeductibleFam.Text="";
			textFlo.Text="";
			textBW.Text="";
			textPano.Text="";
			textExams.Text="";
			comboBW.SetSelectedEnum(FrequencyOptions.Every_Years);
			comboPano.SetSelectedEnum(FrequencyOptions.Every_Years);
			comboExams.SetSelectedEnum(FrequencyOptions.Every_Years);
			textOrthoAge.Text="";
			textOrthoMax.Text="";
			textOrthoPercent.Text="";
			textDeductDiag.Text="";
			textDeductXray.Text="";
			textDeductPrevent.Text="";
			textDeductDiagFam.Text="";
			textDeductXrayFam.Text="";
			textDeductPreventFam.Text="";
			textStand1.Text="";
			textStand2.Text="";
			textStand4.Text="";
			textDiagnostic.Text="";
			textXray.Text="";
			textRoutinePrev.Text="";
			textRestorative.Text="";
			textEndo.Text="";
			textPerio.Text="";
			textOralSurg.Text="";
			textCrowns.Text="";
			textProsth.Text="";
			textMaxProsth.Text="";
			textAccident.Text="";
			textWaitRestorative.Text="";
			textWaitProsth.Text="";
			textWaitPerio.Text="";
			textWaitOralSurg.Text="";
			textWaitEndo.Text="";
			textWaitCrowns.Text="";
			#endregion
			_listBenefitsGrid=new List<Benefit>();
			Benefit benefit;
			for(int i=0;i<_listBenefitsAll.Count;i++) {
				benefit=_listBenefitsAll[i];
				FrequencyOptions frequencyOption=FrequencyOptions._PerBenefitYear;
				if(benefit.QuantityQualifier==BenefitQuantity.Months) {
					frequencyOption=FrequencyOptions.Every_Months;
				}
				else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
					frequencyOption=FrequencyOptions.Every_Years;
				}
				else if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
					frequencyOption=FrequencyOptions._InLast12Months;
				}
				#region Annual Max
				//annual max individual
				if(Benefits.IsAnnualMax(benefit,BenefitCoverageLevel.Individual)) {
					textAnnualMax.Text=benefit.MonetaryAmt.ToString("n");
				}
				//annual max family
				else if(Benefits.IsAnnualMax(benefit,BenefitCoverageLevel.Family)) {
					textAnnualMaxFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				#endregion
				#region General Deductibles
				//deductible individual
				else if(Benefits.IsGeneralDeductible(benefit,BenefitCoverageLevel.Individual)) {
					textDeductible.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible family
				else if(Benefits.IsGeneralDeductible(benefit,BenefitCoverageLevel.Family)) {
					textDeductibleFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				#endregion
				#region Age Limitations
				//Flo
				else if(Benefits.IsFluorideAgeLimit(benefit)) {
					textFlo.Text=benefit.Quantity.ToString();
				}
				else if(Benefits.IsSealantAgeLimit(benefit)) {
					textSealantAge.Text=benefit.Quantity.ToString();
				}
				#endregion
				#region Frequencies
				//BWs group
				else if(textBW.Text=="" && Benefits.IsBitewingFrequency(benefit)) {
					textBW.Text=benefit.Quantity.ToString();
					comboBW.SetSelectedEnum(frequencyOption);
				}
				//Pano
				else if(textPano.Text=="" && Benefits.IsPanoFrequency(benefit)) {
					textPano.Text=benefit.Quantity.ToString();
					comboPano.SetSelectedEnum(frequencyOption);
				}
				//Exam group
				else if(textExams.Text=="" && Benefits.IsExamFrequency(benefit)) {
					textExams.Text=benefit.Quantity.ToString();
					comboExams.SetSelectedEnum(frequencyOption);
				}
				else if(checkSimplified.Checked && Benefits.IsFrequencyLimitation(benefit) && benefit.CodeGroupNum!=0) {
					// Will be shown in Form opened by 'More' button (butFrequencies)
				}
				#endregion
				#region Ortho
				//Ortho Age
				else if(benefit.BenefitType==InsBenefitType.Limitations
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Percent==-1
					&& benefit.QuantityQualifier==BenefitQuantity.AgeLimit)
				{
					textOrthoAge.Text=benefit.Quantity.ToString();
				}
				//OrthoMax
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Limitations
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Percent==-1
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& benefit.CoverageLevel!=BenefitCoverageLevel.Family
					&& benefit.TimePeriod==BenefitTimePeriod.Lifetime)
				{
					textOrthoMax.Text=benefit.MonetaryAmt.ToString("n");
				}
				//OrthoPercent
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear))
				{
					textOrthoPercent.Text=benefit.Percent.ToString();
				}
				#endregion
				#region Diagnostic
				#region Deductible - Diagnostic
				//deductible diagnostic individual
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductDiag.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible diagnostic family
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductDiagFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				#endregion
				#region Deductible - DiagnosticXRay
				//deductible xray individual
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductXray.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible xray family
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductXrayFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				#endregion
				#region Deductible - RoutinePreventive
				//deductible preventive individual
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductPrevent.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible preventive family
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.Deductible
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& benefit.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductPreventFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				#endregion
				#endregion
				#region CoInsurance
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear))
				{
					textDiagnostic.Text=benefit.Percent.ToString();
				}
				//X-Ray
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textXray.Text=benefit.Percent.ToString();
				}
				//RoutinePreventive
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textRoutinePrev.Text=benefit.Percent.ToString();
				}
				//Restorative
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textRestorative.Text=benefit.Percent.ToString();
				}
				//Endo
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textEndo.Text=benefit.Percent.ToString();
				}
				//Perio
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textPerio.Text=benefit.Percent.ToString();
				}
				//OralSurg
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textOralSurg.Text=benefit.Percent.ToString();
				}
				//Crowns
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textCrowns.Text=benefit.Percent.ToString();
				}
				//Prosth
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textProsth.Text=benefit.Percent.ToString();
				}
				//MaxProsth
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textMaxProsth.Text=benefit.Percent.ToString();
				}
				//Accident
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.CoInsurance
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Accident).CovCatNum
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Quantity==0
					&& benefit.QuantityQualifier==BenefitQuantity.None
					&& (benefit.TimePeriod==BenefitTimePeriod.CalendarYear	|| benefit.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textAccident.Text=benefit.Percent.ToString();
				}
				#endregion
				#region WaitingPeriod
				//Restorative Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitRestorative.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitRestorative.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				//Endo Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitEndo.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitEndo.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				//Perio Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitPerio.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitPerio.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				//OralSurg Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitOralSurg.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitOralSurg.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				//Crowns Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitCrowns.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitCrowns.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				//Prosthodontics Waiting Period
				else if(benefit.CodeNum==0
					&& benefit.BenefitType==InsBenefitType.WaitingPeriod
					&& benefit.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum
					&& benefit.PatPlanNum==0)
				{
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						textWaitProsth.Text=benefit.Quantity.ToString();
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						textWaitProsth.Text=(benefit.Quantity*12).ToString();//Convert to months
					}
				}
				#endregion
				else {//any benefit that didn't get handled above should show in the grid.
					_listBenefitsGrid.Add(benefit);
				}
			}
			if(textDiagnostic.Text !="" 
				&& textDiagnostic.Text==textRoutinePrev.Text
				&& textDiagnostic.Text==textXray.Text)
			{
				textStand1.Text=textDiagnostic.Text;
			}
			if(textRestorative.Text !="" 
				&& textRestorative.Text==textEndo.Text 
				&& textRestorative.Text==textPerio.Text	
				&& textRestorative.Text==textOralSurg.Text)
			{
				textStand2.Text=textRestorative.Text;
			}
			if(textCrowns.Text !="" && textCrowns.Text==textProsth.Text) {
				textStand4.Text=textCrowns.Text;
			}
		}

		///<summary>Clears Quick% textbox associated to textCur if newly entered value in textCur no longer matches the existing Quick% value.</summary>
		private void TryResetQuickText(System.Windows.Forms.TextBox textBox,System.Windows.Forms.TextBox textBoxStand) {
			if(textBox.Text!=textBoxStand.Text) {
				_isResettingQuickText=true;
				textBoxStand.Text="";
				_isResettingQuickText=false;
			}
		}

		private void textDiagnostic_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand1);
		}

		private void textXray_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand1);
		}

		private void textRoutinePrev_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand1);
		}

		private void textRestorative_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand2);
		}

		private void textEndo_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand2);
		}

		private void textPerio_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand2);
		}

		private void textOralSurg_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand2);
		}

		private void textCrowns_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand4);
		}

		private void textProsth_Leave(object sender,EventArgs e) {
			TryResetQuickText((System.Windows.Forms.TextBox)sender,textStand4);
		}

		private void textStand1_TextChanged(object sender,EventArgs e) {
			if(_isResettingQuickText) {
				return;
			}
			textDiagnostic.Text=textStand1.Text;
			textXray.Text=textStand1.Text;
			textRoutinePrev.Text=textStand1.Text;
		}

		private void textStand2_TextChanged(object sender,EventArgs e) {
			if(_isResettingQuickText) {
				return;
			}
			textRestorative.Text=textStand2.Text;
			textEndo.Text=textStand2.Text;
			textPerio.Text=textStand2.Text;
			textOralSurg.Text=textStand2.Text;
		}

		private void textStand4_TextChanged(object sender,EventArgs e) {
			if(_isResettingQuickText) {
				return;
			}
			textCrowns.Text=textStand4.Text;
			textProsth.Text=textStand4.Text;
		}

		///<summary>only run once at startup.</summary>
		private void FillCalendarYear() {
			//Convert all of the CalendarYear or ServiceYear benefits over to the correct TimePeriod based on the value of MonthRenew.
			bool isCalendar=(MonthRenew==0);
			List<Benefit> listBenefits;
			BenefitTimePeriod benefitTimePeriod=BenefitTimePeriod.None;
			if(isCalendar) {
				//Convert all ServiceYear benefits over to CalendarYear.
				listBenefits=_listBenefitsAll.FindAll(x => x.TimePeriod==BenefitTimePeriod.ServiceYear);
				benefitTimePeriod=BenefitTimePeriod.CalendarYear;
				checkCalendarYear.Checked=true;
				textMonth.Text="";
				textMonth.Enabled=false;
			}
			else {
				//Convert all CalendarYear benefits over to ServiceYear.
				listBenefits=_listBenefitsAll.FindAll(x => x.TimePeriod==BenefitTimePeriod.CalendarYear);
				benefitTimePeriod=BenefitTimePeriod.ServiceYear;
				checkCalendarYear.Checked=false;
				textMonth.Text=MonthRenew.ToString();
				textMonth.Enabled=true;
			}
			for(int i=0;i<listBenefits.Count;i++) {
				_listBenefitsAll[i].TimePeriod=benefitTimePeriod;
			}
		}

		private void butFrequencies_Click(object sender,EventArgs e) {
			using FormBenefitFrequencies formBenefitFrequencies = new FormBenefitFrequencies();
			if(!ConvertFormToBenefits()) {
				return;
			}
			formBenefitFrequencies.ListBenefitsAll=_listBenefitsAll.Select(x=>x.Copy()).ToList();
			formBenefitFrequencies.PlanNum=_planNum;
			formBenefitFrequencies.PatPlanNum=_patPlanNum;
			bool isCalendar=(MonthRenew==0);
			formBenefitFrequencies.IsCalendar=isCalendar;
			formBenefitFrequencies.ShowDialog();
			if(formBenefitFrequencies.DialogResult!=DialogResult.OK){
				return;
			}
			_listBenefitsAll=formBenefitFrequencies.ListBenefitsAll;
			FillSimple();
			FillGrid();
		}

		///<summary>This only fills the grid on the screen.  It does not get any data from the database.</summary>
		private void FillGrid() {
			_listBenefitsGrid.Sort();
			gridBenefits.BeginUpdate();
			gridBenefits.Columns.Clear();
			GridColumn col=new GridColumn("Pat",35);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Level",60);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Type",90);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Category",90);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("%",35);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Amt",50);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Time Period",80);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Quantity",115);
			gridBenefits.Columns.Add(col);
			gridBenefits.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listBenefitsGrid.Count;i++) {
				row=new GridRow();
				if(_listBenefitsGrid[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(_listBenefitsGrid[i].CoverageLevel==BenefitCoverageLevel.None) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",_listBenefitsGrid[i].CoverageLevel.ToString()));
				}
				if(_listBenefitsGrid[i].BenefitType==InsBenefitType.CoInsurance && _listBenefitsGrid[i].Percent != -1) {
					row.Cells.Add("%");
				}
				else if(Benefits.IsFrequencyLimitation(_listBenefitsGrid[i])) {
					row.Cells.Add(Lan.g(this,"Frequency"));
				}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",_listBenefitsGrid[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(_listBenefitsGrid[i]));
				if(_listBenefitsGrid[i].Percent == -1) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listBenefitsGrid[i].Percent.ToString());
				}
				row.Cells.Add(_listBenefitsGrid[i].GetDisplayMonetaryAmt());
				if(_listBenefitsGrid[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",_listBenefitsGrid[i].TimePeriod.ToString()));
				}
				if(_listBenefitsGrid[i].Quantity>0) {
					row.Cells.Add(_listBenefitsGrid[i].Quantity.ToString()+" "
						+Lan.g("enumBenefitQuantity",_listBenefitsGrid[i].QuantityQualifier.ToString()));
				}
				else {
					row.Cells.Add("");
				}
				gridBenefits.ListGridRows.Add(row);
			}
			gridBenefits.EndUpdate();
		}

		private void gridBenefits_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			int benefitListI=_listBenefitsGrid.IndexOf(_listBenefitsGrid[e.Row]);
			int benefitListAllI=_listBenefitsAll.IndexOf(_listBenefitsGrid[e.Row]);
			using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(_patPlanNum,_planNum);
			formBenefitEdit.BenefitCur=_listBenefitsGrid[e.Row];
			formBenefitEdit.ShowDialog();
			if(formBenefitEdit.BenefitCur==null) {//user deleted
				_listBenefitsGrid.RemoveAt(benefitListI);
				_listBenefitsAll.RemoveAt(benefitListAllI);
			}
			FillGrid();
		}

		///<summary>Returns true if there are a mixture of benefits with calendar and service year time periods.</summary>
		private bool HasInvalidTimePeriods() {
			//Similar code can be found in FillCalendarYear()
			bool isCalendar=(!textMonth.Enabled);
			if(panelSimple.Visible) {
				for(int i=0;i<_listBenefitsAll.Count;i++) {
					if(_listBenefitsAll[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
						return true;
					}
					if(_listBenefitsAll[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
						return true;
					}
				}
				return false;
			}
			for(int i=0;i<_listBenefitsGrid.Count;i++) {
				if(_listBenefitsGrid[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
					return true;
				}
				if(_listBenefitsGrid[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
					return true;
				}
			}
			return false;
		}

		private void checkCalendarYear_Click(object sender,EventArgs e) {
			//checkstate will have already changed.
			//right now, change any benefits in the grid.  Upon closing, the ones in simple view will be changed.
			if(checkCalendarYear.CheckState==CheckState.Checked) {//change all to calendarYear
				textMonth.Text="";
				textMonth.Enabled=false;
				for(int i=0;i<_listBenefitsGrid.Count;i++) {
					if(_listBenefitsGrid[i].TimePeriod==BenefitTimePeriod.ServiceYear) {
						_listBenefitsGrid[i].TimePeriod=BenefitTimePeriod.CalendarYear;
					}
				}
				FillGrid();
				return;
			}
			if(checkCalendarYear.CheckState!=CheckState.Unchecked) {
				FillGrid();
				return;
			}
			//change all to serviceYear
			textMonth.Enabled=true;
			for(int i=0;i<_listBenefitsGrid.Count;i++) {
				if(_listBenefitsGrid[i].TimePeriod==BenefitTimePeriod.CalendarYear) {
					_listBenefitsGrid[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Benefit benefit=new Benefit();
			benefit.IsNew=true;
			benefit.PlanNum=_planNum;
			if(checkCalendarYear.CheckState==CheckState.Checked) {
				benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
			}
			if(checkCalendarYear.CheckState==CheckState.Unchecked) {
				benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
			}
			if(CovCats.GetCount(true) > 0) {
				benefit.CovCatNum=CovCats.GetFirst(true).CovCatNum;
			}
			benefit.BenefitType=InsBenefitType.CoInsurance;
			using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(_patPlanNum,_planNum);
			formBenefitEdit.BenefitCur=benefit;
			formBenefitEdit.ShowDialog();
			if(formBenefitEdit.DialogResult!=DialogResult.OK) {
				FillGrid();
				return;
			}
			_listBenefitsGrid.Add(formBenefitEdit.BenefitCur);
			_listBenefitsAll.Add(formBenefitEdit.BenefitCur);
			if(panelSimple.Visible && ConvertFormToBenefits(isSilent:true)) {
				FillSimple();
			}
			FillGrid();
		}

		private void butClear_Click(object sender,EventArgs e) {
			if(gridBenefits.SelectedIndices.Length==0) {
				gridBenefits.SetAll(true);
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete all benefits in list?")) {
					return;
				}
			}
			List<Benefit> listBenefits=new List<Benefit>();
			for(int i=0;i<gridBenefits.SelectedIndices.Length;i++) {
				listBenefits.Add(_listBenefitsGrid[gridBenefits.SelectedIndices[i]]);
			}
			for(int i=0;i<listBenefits.Count;i++) {
				_listBenefitsGrid.Remove(listBenefits[i]);
				_listBenefitsAll.Remove(listBenefits[i]);
			}
			FillGrid();
		}

		///<summary>Only called if in simple view.  This takes all the data on the form and converts it to benefit items.  A new benefitListAll is created based on a combination of benefitList and the new items from the form.  This is used when clicking OK from simple view, or when switching from simple view to complex view, or when getting ready to view more frequencies, or when adding a new benefit.</summary>
		private bool ConvertFormToBenefits(bool isSilent=false) {
			#region Validation
			string messageText=Lan.g(this,"field is invalid.\r\n"
				+"Leave the field blank or enter an age to denote coverage through that year before clicking OK.");
			if(!textFlo.IsValid()) { 
				MessageBox.Show(this,label4.Text+" "+messageText);
				return false;
			}
			if(!textSealantAge.IsValid()) {
				MessageBox.Show(this,label24.Text+" "+messageText);
				return false;
			}
			if(!textOrthoAge.IsValid()) {
				MessageBox.Show(this,labelOrthoThroughAge.Text+" "+messageText);
				return false;
			}
			if(!textAnnualMax.IsValid()
				|| !textDeductible.IsValid()
				|| !textAnnualMaxFam.IsValid()
				|| !textDeductibleFam.IsValid()
				|| !textFlo.IsValid()
				|| !textBW.IsValid()
				|| !textPano.IsValid()
				|| !textExams.IsValid()
				|| !textOrthoAge.IsValid()
				|| !textOrthoMax.IsValid()
				|| !textOrthoPercent.IsValid()
				|| !textDeductDiag.IsValid()
				|| !textDeductXray.IsValid()
				|| !textDeductPrevent.IsValid()
				|| !textDeductDiagFam.IsValid()
				|| !textDeductXrayFam.IsValid()
				|| !textDeductPreventFam.IsValid()
				|| !textStand1.IsValid()
				|| !textStand2.IsValid()
				|| !textStand4.IsValid()
				|| !textDiagnostic.IsValid()
				|| !textXray.IsValid()
				|| !textRoutinePrev.IsValid()
				|| !textRestorative.IsValid()
				|| !textEndo.IsValid()
				|| !textPerio.IsValid()
				|| !textOralSurg.IsValid()
				|| !textCrowns.IsValid()
				|| !textProsth.IsValid()
				|| !textMaxProsth.IsValid()
				|| !textAccident.IsValid()
				|| !textMonth.IsValid()
				|| !textWaitRestorative.IsValid()
				|| !textWaitEndo.IsValid()
				|| !textWaitPerio.IsValid()
				|| !textWaitOralSurg.IsValid()
				|| !textWaitCrowns.IsValid()
				|| !textWaitProsth.IsValid())
			{
				if(!isSilent) {
					MsgBox.Show(this,"Please fix data entry errors first.");
				}
				return false;
			}
			if(!checkCalendarYear.Checked && textMonth.Text=="") {
				if(!isSilent) {
					MsgBox.Show(this,"Please enter a starting month for the benefit year.");
				}
				return false;
			}
			#endregion Validation
			//We need to pull from the grid, from textboxes, and from frequency limitations.
			//Pull from frequency limitations========================================================================
			List<Benefit> listBenefitsFreqLimits=_listBenefitsAll.FindAll(x=>Benefits.IsFrequencyLimitation(x) && x.CodeGroupNum!=0);
			//these three are already present as textboxes, so they can be excluded:
			long codeGroupNumBW=CodeGroups.GetCodeGroupNumForCodeGroupFixed(EnumCodeGroupFixed.BW);
			long codeGroupNumPano=CodeGroups.GetCodeGroupNumForCodeGroupFixed(EnumCodeGroupFixed.PanoFMX);
			long codeGroupNumExams=CodeGroups.GetCodeGroupNumForCodeGroupFixed(EnumCodeGroupFixed.Exam);
			listBenefitsFreqLimits=listBenefitsFreqLimits.FindAll(x=>!x.CodeGroupNum.In(codeGroupNumBW,codeGroupNumPano,codeGroupNumExams));
			//Pull from grid==========================================================================================
			_listBenefitsAll=new List<Benefit>(_listBenefitsGrid);
			_listBenefitsAll.AddRange(listBenefitsFreqLimits);
			//Pull from textboxes=======================================================================================
			Benefit benefit; 
			#region Annual Max
			//annual max individual
			if(textAnnualMax.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else{
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;					
				}
				benefit.MonetaryAmt=PIn.Double(textAnnualMax.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Individual;
				_listBenefitsAll.Add(benefit);
			}
			//annual max family
			if(textAnnualMaxFam.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textAnnualMaxFam.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Family;
				_listBenefitsAll.Add(benefit);
			}
			#endregion Annual Max
			#region Deductible
			//deductible individual
			if(textDeductible.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else{
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;					
				}
				benefit.MonetaryAmt=PIn.Double(textDeductible.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Individual;
				_listBenefitsAll.Add(benefit);
			}
			//deductible family
			if(textDeductibleFam.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductibleFam.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Family;
				_listBenefitsAll.Add(benefit);
			}
			#endregion
			#region Age Limitation
			//Flo
			if(textFlo.Text!="") {
				//Old code would assume a valid fluoride procedure code was present within PrefName.InsBenFlourideCodes so assume there is a valid fluoride code group.
				long codeGroupNum=CodeGroups.GetCodeGroupNumForCodeGroupFixed(EnumCodeGroupFixed.Fluoride);
				benefit=Benefits.CreateAgeLimitationBenefit(codeGroupNum,PIn.Byte(textFlo.Text),_planNum);
				_listBenefitsAll.Add(benefit);
			}
			if(textSealantAge.Text!="") {
				//Old code would assume a valid sealant procedure code was present within PrefName.InsBenSealantCodes so assume there is a valid sealant code group.
				long codeGroupNum=CodeGroups.GetCodeGroupNumForCodeGroupFixed(EnumCodeGroupFixed.Sealant);
				benefit=Benefits.CreateAgeLimitationBenefit(codeGroupNum,PIn.Byte(textSealantAge.Text),_planNum);
				_listBenefitsAll.Add(benefit);
			}
			#endregion Age Limitation
			#region Frequency
			//Frequency BW group
			if(textBW.Text!="") {
				benefit=MakeFrequencyBenefitForCodeGroupFixed(EnumCodeGroupFixed.BW,comboBW.GetSelected<FrequencyOptions>(),PIn.Byte(textBW.Text));
				_listBenefitsAll.Add(benefit);
			}
			//Frequency pano/FMX group
			if(textPano.Text !="") {
				benefit=MakeFrequencyBenefitForCodeGroupFixed(EnumCodeGroupFixed.PanoFMX,comboPano.GetSelected<FrequencyOptions>(),PIn.Byte(textPano.Text));
				_listBenefitsAll.Add(benefit);
			}
			//Frequency in Exams group
			if(textExams.Text !="") {
				benefit=MakeFrequencyBenefitForCodeGroupFixed(EnumCodeGroupFixed.Exam,comboExams.GetSelected<FrequencyOptions>(),PIn.Byte(textExams.Text));
				_listBenefitsAll.Add(benefit);
			}
			#endregion Frequencies
			#region Ortho
			//ortho age
			if(textOrthoAge.Text!="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				benefit.PlanNum=_planNum;
				benefit.QuantityQualifier=BenefitQuantity.AgeLimit;
				benefit.Quantity=PIn.Byte(textOrthoAge.Text);
				_listBenefitsAll.Add(benefit);
			}
			//ortho max
			if(textOrthoMax.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				benefit.PlanNum=_planNum;
				benefit.TimePeriod=BenefitTimePeriod.Lifetime;
				benefit.MonetaryAmt=PIn.Double(textOrthoMax.Text);
				_listBenefitsAll.Add(benefit);
			}
			//ortho percent
			if(textOrthoPercent.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				benefit.Percent=PIn.Int(textOrthoPercent.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			#endregion Ortho
			#region Deductible
			//deductible diagnostic individual
			if(textDeductDiag.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductDiag.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Individual;
				_listBenefitsAll.Add(benefit);
			}
			//deductible diagnostic family
			if(textDeductDiagFam.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductDiagFam.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Family;
				_listBenefitsAll.Add(benefit);
			}
			//deductible xray individual
			if(textDeductXray.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductXray.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Individual;
				_listBenefitsAll.Add(benefit);
			}
			//deductible xray family
			if(textDeductXrayFam.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductXrayFam.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Family;
				_listBenefitsAll.Add(benefit);
			}
			//deductible preventive individual
			if(textDeductPrevent.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductPrevent.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Individual;
				_listBenefitsAll.Add(benefit);
			}
			//deductible preventive family
			if(textDeductPreventFam.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Deductible;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefit.MonetaryAmt=PIn.Double(textDeductPreventFam.Text);
				benefit.CoverageLevel=BenefitCoverageLevel.Family;
				_listBenefitsAll.Add(benefit);
			}
			//Diagnostic
			if(textDiagnostic.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				benefit.Percent=PIn.Int(textDiagnostic.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//X-Ray
			if(textXray.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				benefit.Percent=PIn.Int(textXray.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//RoutinePreventive
			if(textRoutinePrev.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				benefit.Percent=PIn.Int(textRoutinePrev.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Restorative
			if(textRestorative.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
				benefit.Percent=PIn.Int(textRestorative.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Endo
			if(textEndo.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
				benefit.Percent=PIn.Int(textEndo.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Perio
			if(textPerio.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
				benefit.Percent=PIn.Int(textPerio.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//OralSurg
			if(textOralSurg.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
				benefit.Percent=PIn.Int(textOralSurg.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Crowns
			if(textCrowns.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
				benefit.Percent=PIn.Int(textCrowns.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Prosth
			if(textProsth.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
				benefit.Percent=PIn.Int(textProsth.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//MaxProsth
			if(textMaxProsth.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum;
				benefit.Percent=PIn.Int(textMaxProsth.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			//Accident
			if(textAccident.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.CoInsurance;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Accident).CovCatNum;
				benefit.Percent=PIn.Int(textAccident.Text);
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				_listBenefitsAll.Add(benefit);
			}
			#endregion Deductible
			#region Waiting Period
			//Waiting Periods
			//Restorative
			MakeWaitingPeriodBenefitIfNeeded(textWaitRestorative,EbenefitCategory.Restorative,_listBenefitsAll);
			//Endo
			MakeWaitingPeriodBenefitIfNeeded(textWaitEndo,EbenefitCategory.Endodontics,_listBenefitsAll);
			//Perio
			MakeWaitingPeriodBenefitIfNeeded(textWaitPerio,EbenefitCategory.Periodontics,_listBenefitsAll);
			//OralSurg
			MakeWaitingPeriodBenefitIfNeeded(textWaitOralSurg,EbenefitCategory.OralSurgery,_listBenefitsAll);
			//Crowns
			MakeWaitingPeriodBenefitIfNeeded(textWaitCrowns,EbenefitCategory.Crowns,_listBenefitsAll);
			//Prosth
			MakeWaitingPeriodBenefitIfNeeded(textWaitProsth,EbenefitCategory.Prosthodontics,_listBenefitsAll);
			#endregion Waiting Period
			return true;
		}

		private Benefit MakeFrequencyBenefitForCodeGroupFixed(EnumCodeGroupFixed codeGroupFixed,FrequencyOptions frequencyOption,byte quantity) {
			Benefit benefit=new Benefit();
			benefit.CodeNum=0;
			benefit.CodeGroupNum=CodeGroups.GetCodeGroupNumForCodeGroupFixed(codeGroupFixed);
			benefit.BenefitType=InsBenefitType.Limitations;
			benefit.CovCatNum=0;
			benefit.PlanNum=_planNum;
			if(frequencyOption==FrequencyOptions.Every_Years) {
				benefit.QuantityQualifier=BenefitQuantity.Years;
			}
			else if(frequencyOption==FrequencyOptions._PerBenefitYear) {
				benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
				if(checkCalendarYear.Checked) {
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			else if(frequencyOption==FrequencyOptions.Every_Months) {
				benefit.QuantityQualifier=BenefitQuantity.Months;
			}
			else if(frequencyOption==FrequencyOptions._InLast12Months) {
				benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
				benefit.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
			}
			benefit.Quantity=quantity;
			return benefit;
		}

		private void MakeWaitingPeriodBenefitIfNeeded(ValidNum validNum,EbenefitCategory ebenefitCategory,List<Benefit> listBenefits) {
			if(string.IsNullOrWhiteSpace(validNum.Text)) {
				return;
			}
			Benefit benefit=new Benefit();
			benefit.CodeNum=0;
			benefit.BenefitType=InsBenefitType.WaitingPeriod;
			benefit.CovCatNum=CovCats.GetForEbenCat(ebenefitCategory).CovCatNum;
			benefit.PlanNum=_planNum;
			benefit.QuantityQualifier=BenefitQuantity.Months;
			benefit.Quantity=PIn.Byte(validNum.Text);
			if(checkCalendarYear.Checked) {
				benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
			}
			else {
				benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
			}
			listBenefits.Add(benefit);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkCalendarYear.Checked && textMonth.Text=="") {
				MsgBox.Show(this,"Please enter a starting month for the benefit year.");
				return;
			}
			if(panelSimple.Visible) {
				List<string> listCodeGroupErrors=new List<string>();
				if(!string.IsNullOrWhiteSpace(textFlo.Text) && !CodeGroups.HasValidCodeGroupFixed(EnumCodeGroupFixed.Fluoride)) {
					listCodeGroupErrors.Add(Lan.g(this,$"'Fluoride Through Age' requires the '{EnumCodeGroupFixed.Fluoride.GetDescription()}' Fixed Group."));
				}
				if(!string.IsNullOrWhiteSpace(textSealantAge.Text) && !CodeGroups.HasValidCodeGroupFixed(EnumCodeGroupFixed.Sealant)) {
					listCodeGroupErrors.Add(Lan.g(this,$"'Sealants Through Age' requires the '{EnumCodeGroupFixed.Sealant.GetDescription()}' Fixed Group."));
				}
				if(!string.IsNullOrWhiteSpace(textBW.Text) && !CodeGroups.HasValidCodeGroupFixed(EnumCodeGroupFixed.BW)) {
					listCodeGroupErrors.Add(Lan.g(this,$"'BWs' Frequencies requires the '{EnumCodeGroupFixed.BW.GetDescription()}' Fixed Group."));
				}
				if(!string.IsNullOrWhiteSpace(textPano.Text) && !CodeGroups.HasValidCodeGroupFixed(EnumCodeGroupFixed.PanoFMX)) {
					listCodeGroupErrors.Add(Lan.g(this,$"'Pano/FMX' Frequencies requires the '{EnumCodeGroupFixed.PanoFMX.GetDescription()}' Fixed Group."));
				}
				if(!string.IsNullOrWhiteSpace(textExams.Text) && !CodeGroups.HasValidCodeGroupFixed(EnumCodeGroupFixed.Exam)) {
					listCodeGroupErrors.Add(Lan.g(this,$"'Exams' Frequencies requires the '{EnumCodeGroupFixed.Exam.GetDescription()}' Fixed Group."));
				}
				if(listCodeGroupErrors.Count > 0) {
					string error=Lan.g(this,"The following fields have invalid or missing Fixed Code Group(s):")
						+"\r\n  "+string.Join("\r\n  ",listCodeGroupErrors)
						+"\r\n"+Lan.g(this,"Go to Setup | Code Groups to fix or create the Fixed Code Group(s).")
						+"\r\n"+Lan.g(this,"Uncheck Simplified View to save changes anyway.");
					MsgBox.Show(error);
					return;
				}
				if(!ConvertFormToBenefits()) {
					return;
				}
				if(HasInvalidTimePeriods()) {
					MsgBox.Show(this,"A mixture of calendar and service year time periods are not allowed.");
					return;
				}
				//OriginalBenList.Clear();
				//for(int i=0;i<benefitListAll.Count;i++) {
				//	OriginalBenList.Add(benefitListAll[i]);
				//}
				//We can't just clear the list.  Then, we wouldn't be able to test it for most efficient db queries.
				for(int i=ListBenefits.Count-1;i>=0;i--) {//loop through the old list, backwards.
					bool matchFound=false;
					for(int j=0;j<_listBenefitsAll.Count;j++) {
						if(_listBenefitsAll[j].IsSimilar(ListBenefits[i])) {
							matchFound=true;
						}
					}
					if(!matchFound) {//If no match is found in the new list
						//delete the entry from the old list.  That will cause a deletion from the db later.
						ListBenefits.RemoveAt(i);
					}
				}
				for(int j=0;j<_listBenefitsAll.Count;j++) {//loop through the new list.
					bool matchFound=false;
					for(int i=0;i<ListBenefits.Count;i++) {
						if(_listBenefitsAll[j].IsSimilar(ListBenefits[i])) {
							matchFound=true;
						}
					}
					if(!matchFound) {//If no match is found in the old list
						//add the entry to the old list.  This will cause an insert because BenefitNum will be 0.
						ListBenefits.Add(_listBenefitsAll[j]);
					}
				}
			}
			else {//not simple view.  Will optimize this later for speed.  Should be easier.
				if(HasInvalidTimePeriods()) {
					MsgBox.Show(this,"A mixture of calendar and service year time periods are not allowed.");
					return;
				}
				ListBenefits.Clear();
				for(int i=0;i<_listBenefitsGrid.Count;i++) {
					ListBenefits.Add(_listBenefitsGrid[i]);
				}
			}
			if(checkCalendarYear.Checked) {
				MonthRenew=0;
			}
			else {
				MonthRenew=PIn.Byte(textMonth.Text);
			}
			Note=textSubscNote.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















