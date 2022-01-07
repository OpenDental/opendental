using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormInsBenefits : FormODBase {
		///<summary>This needs to be set externally.  It will only be altered when user clicks OK and form closes.</summary>
		public List<Benefit> OriginalBenList;
		private long PlanNum;
		private long PatPlanNum;
		///<summary>The subscriber note.  Gets set before form opens.</summary>
		public string Note;
		///<summary>This is the list used to display on this form.</summary>
		private List<Benefit> benefitList;
		///<summary>This is the list of all benefits to display on this form.  Some will be in the simple view, and the rest will be transferred to benefitList for display in the grid.</summary>
		private List<Benefit> benefitListAll;
		private bool dontAllowSimplified;
		///<summary>Set this externally before opening the form.  0 indicates calendar year, otherwise 1-12.  This forces all benefits to conform to this setting.  User can change as a whole.</summary>
		public byte MonthRenew;
		private bool _isResettingQuickText=false;
		///<summary>Set this externally before opening the form.</summary>
		public InsSub SubCur;

		///<summary></summary>
		public FormInsBenefits(long planNum,long patPlanNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PatPlanNum=patPlanNum;
			PlanNum=planNum;
		}

		private void FormInsBenefits_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				butOK.Enabled=false;
				butAdd.Enabled=false;
				butDelete.Enabled=false;
			}
			benefitListAll=new List<Benefit>(OriginalBenList);
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
				dontAllowSimplified=true;
				checkSimplified.Checked=false;
				panelSimple.Visible=false;
				gridBenefits.Location=new Point(gridBenefits.Left,groupYear.Bottom+3);
				gridBenefits.Height=butAdd.Top-gridBenefits.Top-5;
			}
			FillCalendarYear();
			FillSimple();
			FillGrid();
			textSubscNote.Text=Note;
			if(SubCur==null) {
				textSubscNote.Visible=false;
				labelSubscNote.Visible=false;
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkSimplified.Checked=false;
				checkSimplified.Visible=false;
			}
		}

		private void checkSimplified_Click(object sender,EventArgs e) {
			if(checkSimplified.Checked){
				if(dontAllowSimplified){
					checkSimplified.Checked=false;
					MsgBox.Show(this,"Not allowed to use simplified view until you fix your Insurance Categories from the setup menu.  At least one of each e-benefit category must be present.");
					return;
				}
				gridBenefits.Title=Lan.g(this,"Other Benefits");
				panelSimple.Visible=true;
				gridBenefits.Location=new Point(gridBenefits.Left,panelSimple.Bottom+4);
				gridBenefits.Height=butAdd.Top-gridBenefits.Top-5;
				//FillSimple handles all further logic.
			}
			else{
				if(!ConvertFormToBenefits()){
					checkSimplified.Checked=true;
					return;
				}
				gridBenefits.Title=Lan.g(this,"Benefits");
				panelSimple.Visible=false;
				gridBenefits.Location=new Point(gridBenefits.Left,groupYear.Bottom+3);
				gridBenefits.Height=butAdd.Top-gridBenefits.Top-5;
			}
			FillSimple();
			FillGrid();
		}

		///<summary>This will only be run when the form first opens or if user switches to simple view.  FillGrid should always be run after this.</summary>
		private void FillSimple(){
			if(!panelSimple.Visible){
				benefitList=new List<Benefit>(benefitListAll);
				return;
			}
			textAnnualMax.Text="";
			textDeductible.Text="";
			textAnnualMaxFam.Text="";
			textDeductibleFam.Text="";
			textFlo.Text="";
			textBW.Text="";
			textPano.Text="";
			textExams.Text="";
			comboBW.SelectedIndex=0;
			comboPano.SelectedIndex=0;
			comboExams.SelectedIndex=0;
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
			benefitList=new List<Benefit>();
			Benefit ben;
			for(int i=0;i<benefitListAll.Count;i++){
				#region Loop
				ben=benefitListAll[i];
				string benProcCode=ProcedureCodes.GetStringProcCode(ben.CodeNum);
				//annual max individual
				if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Individual)) {
					textAnnualMax.Text=ben.MonetaryAmt.ToString("n");
				}
				//annual max family
				else if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Family)) {
					textAnnualMaxFam.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible individual
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Individual)) {
					textDeductible.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible family
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Family)) {
					textDeductibleFam.Text=ben.MonetaryAmt.ToString("n");
				}
				//Flo
				else if(Benefits.IsFlourideAgeLimit(ben)) {
					textFlo.Text=ben.Quantity.ToString();
				}
				else if(Benefits.IsSealantAgeLimit(ben)) {
					textSealantAge.Text=ben.Quantity.ToString();
				}
				//Canadian Flo
				else if(CultureInfo.CurrentCulture.Name.EndsWith("CA") &&
					((Canadian.IsQuebec() && benProcCode=="12400")//The proc code is different for Quebec!
					|| (!Canadian.IsQuebec() && benProcCode=="12101"))//The rest of Canada conforms to a standard.
					&& ben.BenefitType==InsBenefitType.Limitations
					//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Percent==-1
					&& ben.QuantityQualifier==BenefitQuantity.AgeLimit)
				{
					textFlo.Text=ben.Quantity.ToString();
				}
				//BWs group
				else if(Benefits.IsBitewingFrequency(ben)) {
					textBW.Text=ben.Quantity.ToString();
					if(ben.QuantityQualifier==BenefitQuantity.Months){
						comboBW.SelectedIndex=2;
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years){
						comboBW.SelectedIndex=0;
					}
					else{
						if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
				}
				//Canadian BWs
				else if(CultureInfo.CurrentCulture.Name.EndsWith("CA")//All of Canada, including Quebec (the proc codes are the same in this instance).
					&& ProcedureCodes.GetStringProcCode(ben.CodeNum)=="02144"//4BW
					&& ben.BenefitType==InsBenefitType.Limitations
					//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Percent==-1
					&& (ben.QuantityQualifier==BenefitQuantity.Months
						|| ben.QuantityQualifier==BenefitQuantity.Years
						|| ben.QuantityQualifier==BenefitQuantity.NumberOfServices))
				{
					textBW.Text=ben.Quantity.ToString();
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						comboBW.SelectedIndex=2;
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						comboBW.SelectedIndex=0;
					}
					else {
						if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
				}
				//Pano
				else if(Benefits.IsPanoFrequency(ben)) {
					textPano.Text=ben.Quantity.ToString();
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						comboPano.SelectedIndex=2;
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						comboPano.SelectedIndex=0;
					}
					else {
						if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboPano.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboPano.SelectedIndex=1;//# per year
						}
					}
				}
				//Exam group
				else if(Benefits.IsExamFrequency(ben)) {
					textExams.Text=ben.Quantity.ToString();
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						comboExams.SelectedIndex=2;
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						comboExams.SelectedIndex=0;
					}
					else {
						if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboExams.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboExams.SelectedIndex=1;//# per year
						}
					}
				}
				//Ortho Age
				else if(ben.BenefitType==InsBenefitType.Limitations
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Percent==-1
					&& ben.QuantityQualifier==BenefitQuantity.AgeLimit)
				{
					textOrthoAge.Text=ben.Quantity.ToString();
				}
				//OrthoMax
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Limitations
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Percent==-1
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& ben.CoverageLevel!=BenefitCoverageLevel.Family
					&& ben.TimePeriod==BenefitTimePeriod.Lifetime)
				{
					textOrthoMax.Text=ben.MonetaryAmt.ToString("n");
				}
				//OrthoPercent
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear))
				{
					textOrthoPercent.Text=ben.Percent.ToString();
				}
				//deductible diagnostic individual
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductDiag.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible diagnostic family
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductDiagFam.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible xray individual
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductXray.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible xray family
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductXrayFam.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible preventive individual
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Individual) {
					textDeductPrevent.Text=ben.MonetaryAmt.ToString("n");
				}
				//deductible preventive family
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.Deductible
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)
					&& ben.CoverageLevel==BenefitCoverageLevel.Family) {
					textDeductPreventFam.Text=ben.MonetaryAmt.ToString("n");
				}
				//Stand1
				//Stand2
				//Stand4
				//Diagnostic
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear))
				{
					textDiagnostic.Text=ben.Percent.ToString();
				}
				//X-Ray
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textXray.Text=ben.Percent.ToString();
				}
				//RoutinePreventive
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textRoutinePrev.Text=ben.Percent.ToString();
				}
				//Restorative
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textRestorative.Text=ben.Percent.ToString();
				}
				//Endo
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) 
				{
					textEndo.Text=ben.Percent.ToString();
				}
				//Perio
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textPerio.Text=ben.Percent.ToString();
				}
				//OralSurg
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textOralSurg.Text=ben.Percent.ToString();
				}
				//Crowns
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textCrowns.Text=ben.Percent.ToString();
				}
				//Prosth
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textProsth.Text=ben.Percent.ToString();
				}
				//MaxProsth
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textMaxProsth.Text=ben.Percent.ToString();
				}
				//Accident
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.CoInsurance
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Accident).CovCatNum
					&& ben.MonetaryAmt==-1
					&& ben.PatPlanNum==0
					&& ben.Quantity==0
					&& ben.QuantityQualifier==BenefitQuantity.None
					&& (ben.TimePeriod==BenefitTimePeriod.CalendarYear	|| ben.TimePeriod==BenefitTimePeriod.ServiceYear)) {
					textAccident.Text=ben.Percent.ToString();
				}
				//Restorative Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitRestorative.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitRestorative.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//Endo Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitEndo.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitEndo.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//Perio Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitPerio.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitPerio.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//OralSurg Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitOralSurg.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitOralSurg.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//Crowns Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitCrowns.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitCrowns.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//Prosthodontics Waiting Period
				else if(ben.CodeNum==0
					&& ben.BenefitType==InsBenefitType.WaitingPeriod
					&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum
					&& ben.PatPlanNum==0)
				{
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						textWaitProsth.Text=ben.Quantity.ToString();
					}
					else if(ben.QuantityQualifier==BenefitQuantity.Years) {
						textWaitProsth.Text=(ben.Quantity*12).ToString();//Convert to months
					}
				}
				//any benefit that didn't get handled above
				else {
					benefitList.Add(ben);
				}
				#endregion Loop
			}
			if(textDiagnostic.Text !="" && textDiagnostic.Text==textRoutinePrev.Text
				&& textDiagnostic.Text==textXray.Text)
			{
				textStand1.Text=textDiagnostic.Text;
			}
			if(textRestorative.Text !="" && textRestorative.Text==textEndo.Text 
				&& textRestorative.Text==textPerio.Text	&& textRestorative.Text==textOralSurg.Text)
			{
				textStand2.Text=textRestorative.Text;
			}
			if(textCrowns.Text !="" && textCrowns.Text==textProsth.Text) {
				textStand4.Text=textCrowns.Text;
			}
		}

		///<summary>Clears Quick% textbox associated to textCur if newly entered value in textCur no longer matches the existing Quick% value.</summary>
		private void TryResetQuickText(TextBox textCur,TextBox textStand) {
			if(textCur.Text!=textStand.Text) {
				_isResettingQuickText=true;
				textStand.Text="";
				_isResettingQuickText=false;
			}
		}

		private void textDiagnostic_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand1);
		}

		private void textXray_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand1);
		}

		private void textRoutinePrev_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand1);
		}

		private void textRestorative_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand2);
		}

		private void textEndo_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand2);
		}

		private void textPerio_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand2);
		}

		private void textOralSurg_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand2);
		}

		private void textCrowns_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand4);
		}

		private void textProsth_Leave(object sender,EventArgs e) {
			TryResetQuickText((TextBox)sender,textStand4);
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
			bool isCalendar= MonthRenew==0;
			for(int i=0;i<benefitListAll.Count;i++) {
				if(benefitListAll[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
					benefitListAll[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				if(benefitListAll[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
					benefitListAll[i].TimePeriod=BenefitTimePeriod.CalendarYear;
				}
			}
			if(isCalendar) {
				checkCalendarYear.Checked=true;
				textMonth.Text="";
				textMonth.Enabled=false;
			}
			else {
				checkCalendarYear.Checked=false;
				textMonth.Text=MonthRenew.ToString();
				textMonth.Enabled=true;
			}
		}

		///<summary>This only fills the grid on the screen.  It does not get any data from the database.</summary>
		private void FillGrid() {
			benefitList.Sort();
			gridBenefits.BeginUpdate();
			gridBenefits.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Pat",35);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Level",60);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Type",90);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Category",90);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("%",35);//,HorizontalAlignment.Right);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Amt",50);//,HorizontalAlignment.Right);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Time Period",80);
			gridBenefits.ListGridColumns.Add(col);
			col=new GridColumn("Quantity",115);
			gridBenefits.ListGridColumns.Add(col);
			gridBenefits.ListGridRows.Clear();
			GridRow row;
			//bool isCalendarYear=true;
			for(int i=0;i<benefitList.Count;i++) {
				row=new GridRow();
				if(benefitList[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(benefitList[i].CoverageLevel==BenefitCoverageLevel.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",benefitList[i].CoverageLevel.ToString()));
				}
				if(benefitList[i].BenefitType==InsBenefitType.CoInsurance && benefitList[i].Percent != -1) {
					row.Cells.Add("%");
				}
				//else if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.Limitations
				//	&& (((Benefit)benefitList[i]).TimePeriod==BenefitTimePeriod.ServiceYear
				//	|| ((Benefit)benefitList[i]).TimePeriod==BenefitTimePeriod.CalendarYear)
				//	&& ((Benefit)benefitList[i]).QuantityQualifier==BenefitQuantity.None) {//annual max
				//	row.Cells.Add(Lan.g(this,"Annual Max"));
				//}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",benefitList[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(benefitList[i]));
				if(benefitList[i].Percent == -1) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(benefitList[i].Percent.ToString());
				}
				row.Cells.Add(benefitList[i].GetDisplayMonetaryAmt());
				if(benefitList[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",benefitList[i].TimePeriod.ToString()));
				}
				if(benefitList[i].Quantity>0) {
					row.Cells.Add(benefitList[i].Quantity.ToString()+" "
						+Lan.g("enumBenefitQuantity",benefitList[i].QuantityQualifier.ToString()));
				}
				else {
					row.Cells.Add("");
				}
				gridBenefits.ListGridRows.Add(row);
			}
			gridBenefits.EndUpdate();
		}

		private void gridBenefits_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e){
			int benefitListI=benefitList.IndexOf(benefitList[e.Row]);
			int benefitListAllI=benefitListAll.IndexOf(benefitList[e.Row]);
			using FormBenefitEdit FormB=new FormBenefitEdit(PatPlanNum,PlanNum);
			FormB.BenefitCur=benefitList[e.Row];
			FormB.ShowDialog();
			if(FormB.BenefitCur==null){//user deleted
				benefitList.RemoveAt(benefitListI);
				benefitListAll.RemoveAt(benefitListAllI);
			}
			FillGrid();
		}

		///<summary>Returns true if there are a mixture of benefits with calendar and service year time periods.</summary>
		private bool HasInvalidTimePeriods() {
			//Similar code can be found in FillCalendarYear()
			bool isCalendar=(!textMonth.Enabled);
			if(panelSimple.Visible) {
				for(int i=0;i<benefitListAll.Count;i++) {
					if(benefitListAll[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
						return true;
					}
					if(benefitListAll[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
						return true;
					}
				}
			}
			else {
				for(int i=0;i<benefitList.Count;i++) {
					if(benefitList[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
						return true;
					}
					if(benefitList[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
						return true;
					}
				}
			}
			return false;
		}

		private void checkCalendarYear_Click(object sender,EventArgs e) {
			//checkstate will have already changed.
			//right now, change any benefits in the grid.  Upon closing, the ones in simple view will be changed.
			if(checkCalendarYear.CheckState==CheckState.Checked){//change all to calendarYear
				textMonth.Text="";
				textMonth.Enabled=false;
				for(int i=0;i<benefitList.Count;i++){
					if(benefitList[i].TimePeriod==BenefitTimePeriod.ServiceYear){
						benefitList[i].TimePeriod=BenefitTimePeriod.CalendarYear;
					}
				}
			}
			else if(checkCalendarYear.CheckState==CheckState.Unchecked) {//change all to serviceYear
				textMonth.Enabled=true;
				for(int i=0;i<benefitList.Count;i++) {
					if(benefitList[i].TimePeriod==BenefitTimePeriod.CalendarYear) {
						benefitList[i].TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
			}
			FillGrid();
		}

		private void butMoreFrequencies_Click(object sender,EventArgs e) {
			if(!ConvertFormToBenefits()) {
				return;
			}
			using FormBenefitFrequencies formBenefitFrequencies=new FormBenefitFrequencies(benefitListAll,PlanNum,checkCalendarYear.Checked);
			if(formBenefitFrequencies.ShowDialog()!=DialogResult.OK) {
				return;
			}
			benefitListAll=formBenefitFrequencies.ListBenefits;
			FillSimple();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Benefit ben=new Benefit();
			ben.PlanNum=PlanNum;
			if(checkCalendarYear.CheckState==CheckState.Checked) {
				ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			}
			if(checkCalendarYear.CheckState==CheckState.Unchecked) {
				ben.TimePeriod=BenefitTimePeriod.ServiceYear;
			}
			if(CovCats.GetCount(true) > 0){
				ben.CovCatNum=CovCats.GetFirst(true).CovCatNum;
			}
			ben.BenefitType=InsBenefitType.CoInsurance;
			using FormBenefitEdit FormB=new FormBenefitEdit(PatPlanNum,PlanNum);
			FormB.IsNew=true;
			FormB.BenefitCur=ben;
			FormB.ShowDialog();
			if(FormB.DialogResult==DialogResult.OK) {
				bool doFillSimple=false;
				if(panelSimple.Visible && ConvertFormToBenefits(isSilent:true)) {
					doFillSimple=true;
				}
				benefitList.Add(FormB.BenefitCur);
				benefitListAll.Add(FormB.BenefitCur);
				if(doFillSimple) {
					FillSimple();
				}
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
			List<Benefit> listToClear=new List<Benefit>();
			for(int i=0;i<gridBenefits.SelectedIndices.Length;i++) {
				listToClear.Add(benefitList[gridBenefits.SelectedIndices[i]]);
			}
			for(int i=0;i<listToClear.Count;i++) {
				benefitList.Remove(listToClear[i]);
				benefitListAll.Remove(listToClear[i]);
			}
			FillGrid();
		}

		///<summary>Only called if in simple view.  This takes all the data on the form and converts it to benefit items.  A new benefitListAll is created based on a combination of benefitList and the new items from the form.  This is used when clicking OK from simple view, or when switching from simple view to complex view.</summary>
		private bool ConvertFormToBenefits(bool isSilent=false){
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
			/*bool hasIndivid=false;
			if(textAnnualMax.Text != "" || textDeductible.Text != "" || textDeductPrev.Text != "") {
				hasIndivid=true;
			}
			bool hasFam=false;
			if(textAnnualMaxFam.Text != "" || textDeductibleFam.Text != "" || textDeductPrevFam.Text != "") {
				hasFam=true;
			}
			if(hasIndivid && hasFam) {
				MsgBox.Show(this,"You can enter either Individual or Family benefits, but not both.");
				return false;
			}*/
			benefitListAll=new List<Benefit>(benefitList);
			Benefit ben;
			//annual max individual
			if(textAnnualMax.Text !=""){
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked){
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else{
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;					
				}
				ben.MonetaryAmt=PIn.Double(textAnnualMax.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Individual;
				benefitListAll.Add(ben);
			}
			//annual max family
			if(textAnnualMaxFam.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textAnnualMaxFam.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Family;
				benefitListAll.Add(ben);
			}
			//deductible individual
			if(textDeductible.Text !=""){
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else{
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;					
				}
				ben.MonetaryAmt=PIn.Double(textDeductible.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Individual;
				benefitListAll.Add(ben);
			}
			//deductible family
			if(textDeductibleFam.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductibleFam.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Family;
				benefitListAll.Add(ben);
			}
			//Flo
			if(textFlo.Text !=""){
				ben=Benefits.CreateAgeLimitationBenefit(ProcedureCodes.GetCodeNum(ProcedureCodes.FlourideCode),PIn.Byte(textFlo.Text),PlanNum);
				benefitListAll.Add(ben);
			}
			if(textSealantAge.Text!="") {
				ben=Benefits.CreateAgeLimitationBenefit(ProcedureCodes.GetCodeNum(ProcedureCodes.SealantCode),PIn.Byte(textSealantAge.Text),PlanNum);
				benefitListAll.Add(ben);
			}
			//Frequency BW group
			if(textBW.Text !="") {
				ben=new Benefit();
				ben.CodeNum=ProcedureCodes.GetCodeNum(ProcedureCodes.BitewingCode);
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(comboBW.SelectedIndex==0){
					ben.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboBW.SelectedIndex==1){
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						ben.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboBW.SelectedIndex==2){
					ben.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboBW.SelectedIndex==3) {
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					ben.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				ben.Quantity=PIn.Byte(textBW.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				benefitListAll.Add(ben);
			}
			//Frequency pano/FMX group
			if(textPano.Text !="") {
				ben=new Benefit();
				ben.CodeNum=ProcedureCodes.GetCodeNum(ProcedureCodes.PanoCode);
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=0;
				ben.PlanNum=PlanNum;
				if(comboPano.SelectedIndex==0) {
					ben.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboPano.SelectedIndex==1) {
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						ben.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboPano.SelectedIndex==2) {
					ben.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboPano.SelectedIndex==3) {
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					ben.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				ben.Quantity=PIn.Byte(textPano.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				benefitListAll.Add(ben);
			}
			//Frequency in Exams group
			if(textExams.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				ben.PlanNum=PlanNum;
				if(comboExams.SelectedIndex==0) {
					ben.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboExams.SelectedIndex==1) {
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						ben.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboExams.SelectedIndex==2) {
					ben.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboExams.SelectedIndex==3) {
					ben.QuantityQualifier=BenefitQuantity.NumberOfServices;
					ben.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				ben.Quantity=PIn.Byte(textExams.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				benefitListAll.Add(ben);
			}
			//ortho age
			if(textOrthoAge.Text!="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				ben.PlanNum=PlanNum;
				ben.QuantityQualifier=BenefitQuantity.AgeLimit;
				ben.Quantity=PIn.Byte(textOrthoAge.Text);
				benefitListAll.Add(ben);
			}
			//ortho max
			if(textOrthoMax.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Limitations;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				ben.PlanNum=PlanNum;
				ben.TimePeriod=BenefitTimePeriod.Lifetime;
				ben.MonetaryAmt=PIn.Double(textOrthoMax.Text);
				benefitListAll.Add(ben);
			}
			//ortho percent
			if(textOrthoPercent.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
				ben.Percent=PIn.Int(textOrthoPercent.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//deductible diagnostic individual
			if(textDeductDiag.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductDiag.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Individual;
				benefitListAll.Add(ben);
			}
			//deductible diagnostic family
			if(textDeductDiagFam.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductDiagFam.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Family;
				benefitListAll.Add(ben);
			}
			//deductible xray individual
			if(textDeductXray.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductXray.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Individual;
				benefitListAll.Add(ben);
			}
			//deductible xray family
			if(textDeductXrayFam.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductXrayFam.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Family;
				benefitListAll.Add(ben);
			}
			//deductible preventive individual
			if(textDeductPrevent.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductPrevent.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Individual;
				benefitListAll.Add(ben);
			}
			//deductible preventive family
			if(textDeductPreventFam.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.Deductible;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				ben.MonetaryAmt=PIn.Double(textDeductPreventFam.Text);
				ben.CoverageLevel=BenefitCoverageLevel.Family;
				benefitListAll.Add(ben);
			}
			//Diagnostic
			if(textDiagnostic.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
				ben.Percent=PIn.Int(textDiagnostic.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//X-Ray
			if(textXray.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.DiagnosticXRay).CovCatNum;
				ben.Percent=PIn.Int(textXray.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//RoutinePreventive
			if(textRoutinePrev.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				ben.Percent=PIn.Int(textRoutinePrev.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Restorative
			if(textRestorative.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
				ben.Percent=PIn.Int(textRestorative.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Endo
			if(textEndo.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
				ben.Percent=PIn.Int(textEndo.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Perio
			if(textPerio.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
				ben.Percent=PIn.Int(textPerio.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//OralSurg
			if(textOralSurg.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
				ben.Percent=PIn.Int(textOralSurg.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Crowns
			if(textCrowns.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
				ben.Percent=PIn.Int(textCrowns.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Prosth
			if(textProsth.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
				ben.Percent=PIn.Int(textProsth.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//MaxProsth
			if(textMaxProsth.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.MaxillofacialProsth).CovCatNum;
				ben.Percent=PIn.Int(textMaxProsth.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Accident
			if(textAccident.Text !="") {
				ben=new Benefit();
				ben.CodeNum=0;
				ben.BenefitType=InsBenefitType.CoInsurance;
				ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Accident).CovCatNum;
				ben.Percent=PIn.Int(textAccident.Text);
				ben.PlanNum=PlanNum;
				if(checkCalendarYear.Checked) {
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					ben.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				benefitListAll.Add(ben);
			}
			//Waiting Periods
			//Restorative
			MakeWaitingPeriodBenefitIfNeeded(textWaitRestorative,EbenefitCategory.Restorative,benefitListAll);
			//Endo
			MakeWaitingPeriodBenefitIfNeeded(textWaitEndo,EbenefitCategory.Endodontics,benefitListAll);
			//Perio
			MakeWaitingPeriodBenefitIfNeeded(textWaitPerio,EbenefitCategory.Periodontics,benefitListAll);
			//OralSurg
			MakeWaitingPeriodBenefitIfNeeded(textWaitOralSurg,EbenefitCategory.OralSurgery,benefitListAll);
			//Crowns
			MakeWaitingPeriodBenefitIfNeeded(textWaitCrowns,EbenefitCategory.Crowns,benefitListAll);
			//Prosth
			MakeWaitingPeriodBenefitIfNeeded(textWaitProsth,EbenefitCategory.Prosthodontics,benefitListAll);
			return true;
		}

		private void MakeWaitingPeriodBenefitIfNeeded(ValidNum textBox,EbenefitCategory category,List<Benefit> listAllBenefits) {
			if(string.IsNullOrWhiteSpace(textBox.Text)) {
				return;
			}
			Benefit ben=new Benefit();
			ben.CodeNum=0;
			ben.BenefitType=InsBenefitType.WaitingPeriod;
			ben.CovCatNum=CovCats.GetForEbenCat(category).CovCatNum;
			ben.PlanNum=PlanNum;
			ben.QuantityQualifier=BenefitQuantity.Months;
			ben.Quantity=PIn.Byte(textBox.Text);
			if(checkCalendarYear.Checked) {
				ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			}
			else {
				ben.TimePeriod=BenefitTimePeriod.ServiceYear;
			}
			listAllBenefits.Add(ben);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkCalendarYear.Checked && textMonth.Text=="") {
				MsgBox.Show(this,"Please enter a starting month for the benefit year.");
				return;
			}
			if(panelSimple.Visible) {
				if(!ConvertFormToBenefits()) {
					return;
				}
				if(HasInvalidTimePeriods()) {
					MsgBox.Show(this,"A mixture of calendar and service year time periods are not allowed.");
					return;
				}
				//OriginalBenList.Clear();
				//for(int i=0;i<benefitListAll.Count;i++){
				//	OriginalBenList.Add(benefitListAll[i]);
				//}
				//We can't just clear the list.  Then, we wouldn't be able to test it for most efficient db queries.
				for(int i=OriginalBenList.Count-1;i>=0;i--) {//loop through the old list, backwards.
					bool matchFound=false;
					for(int j=0;j<benefitListAll.Count;j++) {
						if(benefitListAll[j].IsSimilar(OriginalBenList[i])) {
							matchFound=true;
						}
					}
					if(!matchFound) {//If no match is found in the new list
						//delete the entry from the old list.  That will cause a deletion from the db later.
						OriginalBenList.RemoveAt(i);
					}
				}
				for(int j=0;j<benefitListAll.Count;j++) {//loop through the new list.
					bool matchFound=false;
					for(int i=0;i<OriginalBenList.Count;i++) {
						if(benefitListAll[j].IsSimilar(OriginalBenList[i])) {
							matchFound=true;
						}
					}
					if(!matchFound) {//If no match is found in the old list
						//add the entry to the old list.  This will cause an insert because BenefitNum will be 0.
						OriginalBenList.Add(benefitListAll[j]);
					}
				}
			}
			else {//not simple view.  Will optimize this later for speed.  Should be easier.
				if(HasInvalidTimePeriods()) {
					MsgBox.Show(this,"A mixture of calendar and service year time periods are not allowed.");
					return;
				}
				OriginalBenList.Clear();
				for(int i=0;i<benefitList.Count;i++) {
					OriginalBenList.Add(benefitList[i]);
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





















