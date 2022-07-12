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
	/// <summary></summary>
	public partial class FormInsBenefits : FormODBase {
		///<summary>This needs to be set externally.  It will only be altered when user clicks OK and form closes.</summary>
		public List<Benefit> ListBenefits;
		private long _planNum;
		private long _patPlanNum;
		///<summary>The subscriber note.  Gets set before form opens.</summary>
		public string Note;
		///<summary>This is the list used to display on this form.</summary>
		private List<Benefit> _listBenefits;
		///<summary>This is the list of all benefits to display on this form.  Some will be in the simple view, and the rest will be transferred to benefitList for display in the grid.</summary>
		private List<Benefit> _listBenefitsAll;
		private bool _dontAllowSimplified;
		///<summary>Set this externally before opening the form.  0 indicates calendar year, otherwise 1-12.  This forces all benefits to conform to this setting.  User can change as a whole.</summary>
		public byte MonthRenew;
		private bool _isResettingQuickText=false;
		///<summary>Set this externally before opening the form.</summary>
		public InsSub InsSub_;

		///<summary></summary>
		public FormInsBenefits(long planNum,long patPlanNum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patPlanNum=patPlanNum;
			_planNum=planNum;
		}

		private void FormInsBenefits_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				butOK.Enabled=false;
				butAdd.Enabled=false;
				butDelete.Enabled=false;
			}
			List<string> listFrequencyOptions=new List<string>() {
				"Every # Years",
				"# Per Benefit Year",
				"Every # Months",
				"# in Last 12 Months"
			};
			comboBW.Items.AddList(listFrequencyOptions);
			comboPano.Items.AddList(listFrequencyOptions);
			comboExams.Items.AddList(listFrequencyOptions);
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
			if(checkSimplified.Checked){
				if(_dontAllowSimplified){
					checkSimplified.Checked=false;
					MsgBox.Show(this,"Not allowed to use simplified view until you fix your Insurance Categories from the setup menu.  At least one of each e-benefit category must be present.");
					return;
				}
				gridBenefits.Title=Lan.g(this,"Other Benefits");
				panelSimple.Visible=true;
				LayoutManager.MoveLocation(gridBenefits,new Point(gridBenefits.Left,panelSimple.Bottom+4));
				LayoutManager.MoveHeight(gridBenefits,butAdd.Top-gridBenefits.Top-5);
				//FillSimple handles all further logic.
				FillSimple();
				FillGrid();
				return;
			}
			//Not checked
			if(!ConvertFormToBenefits()){
				checkSimplified.Checked=true;
				return;
			}
			gridBenefits.Title=Lan.g(this,"Benefits");
			panelSimple.Visible=false;
			LayoutManager.MoveLocation(gridBenefits,new Point(gridBenefits.Left,groupYear.Bottom+3));
			LayoutManager.MoveHeight(gridBenefits,butAdd.Top-gridBenefits.Top-5);
			FillSimple();
			FillGrid();
		}

		///<summary>This will only be run when the form first opens or if user switches to simple view.  FillGrid should always be run after this.</summary>
		private void FillSimple(){
			if(!panelSimple.Visible){
				_listBenefits=new List<Benefit>(_listBenefitsAll);
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
			_listBenefits=new List<Benefit>();
			Benefit benefit;
			for(int i=0;i<_listBenefitsAll.Count;i++){
				#region Loop
				benefit=_listBenefitsAll[i];
				string benProcCode=ProcedureCodes.GetStringProcCode(benefit.CodeNum);
				//annual max individual
				if(Benefits.IsAnnualMax(benefit,BenefitCoverageLevel.Individual)) {
					textAnnualMax.Text=benefit.MonetaryAmt.ToString("n");
				}
				//annual max family
				else if(Benefits.IsAnnualMax(benefit,BenefitCoverageLevel.Family)) {
					textAnnualMaxFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible individual
				else if(Benefits.IsGeneralDeductible(benefit,BenefitCoverageLevel.Individual)) {
					textDeductible.Text=benefit.MonetaryAmt.ToString("n");
				}
				//deductible family
				else if(Benefits.IsGeneralDeductible(benefit,BenefitCoverageLevel.Family)) {
					textDeductibleFam.Text=benefit.MonetaryAmt.ToString("n");
				}
				//Flo
				else if(Benefits.IsFlourideAgeLimit(benefit)) {
					textFlo.Text=benefit.Quantity.ToString();
				}
				else if(Benefits.IsSealantAgeLimit(benefit)) {
					textSealantAge.Text=benefit.Quantity.ToString();
				}
				//Canadian Flo
				else if(CultureInfo.CurrentCulture.Name.EndsWith("CA") &&
					((Canadian.IsQuebec() && benProcCode=="12400")//The proc code is different for Quebec!
					|| (!Canadian.IsQuebec() && benProcCode=="12101"))//The rest of Canada conforms to a standard.
					&& benefit.BenefitType==InsBenefitType.Limitations
					//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Percent==-1
					&& benefit.QuantityQualifier==BenefitQuantity.AgeLimit)
				{
					textFlo.Text=benefit.Quantity.ToString();
				}
				//Checks if benefit codenum is the first codenum in limitation preference, then checks if benefit is a frequency limitation.
				#region frequency limitations
				//Canadian BWs
				else if(Benefits.IsBitewingFrequency(benefit)
					&& CultureInfo.CurrentCulture.Name.EndsWith("CA")//All of Canada, including Quebec (the proc codes are the same in this instance).
					&& textBW.Text==""
					&& ProcedureCodes.GetStringProcCode(benefit.CodeNum)=="02144"//4BW
					&& benefit.BenefitType==InsBenefitType.Limitations
					//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
					&& benefit.MonetaryAmt==-1
					&& benefit.PatPlanNum==0
					&& benefit.Percent==-1
					&& (benefit.QuantityQualifier==BenefitQuantity.Months
						|| benefit.QuantityQualifier==BenefitQuantity.Years
						|| benefit.QuantityQualifier==BenefitQuantity.NumberOfServices))
				{
					textBW.Text=benefit.Quantity.ToString();
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						comboBW.SelectedIndex=2;
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						comboBW.SelectedIndex=0;
					}
					else {
						if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
				}
				//BWs group
				else if(textBW.Text=="" 
					&& ProcedureCodes.GetStringProcCode(benefit.CodeNum)==ProcedureCodes.BitewingCode 
					&& Benefits.IsBitewingFrequency(benefit))
				{
					textBW.Text=benefit.Quantity.ToString();
					if(benefit.QuantityQualifier==BenefitQuantity.Months){
						comboBW.SelectedIndex=2;
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years){
						comboBW.SelectedIndex=0;
					}
					else{
						if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
				}
				//Pano
				else if(Benefits.IsPanoFrequency(benefit)
					&& ProcedureCodes.GetStringProcCode(benefit.CodeNum)==ProcedureCodes.PanoCode 
					&& textPano.Text=="") 
				{
					textPano.Text=benefit.Quantity.ToString();
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						comboPano.SelectedIndex=2;
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						comboPano.SelectedIndex=0;
					}
					else {
						if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboPano.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboPano.SelectedIndex=1;//# per year
						}
					}
				}
				//Exam group
				else if(Benefits.IsExamFrequency(benefit) && textExams.Text=="") {
					textExams.Text=benefit.Quantity.ToString();
					if(benefit.QuantityQualifier==BenefitQuantity.Months) {
						comboExams.SelectedIndex=2;
					}
					else if(benefit.QuantityQualifier==BenefitQuantity.Years) {
						comboExams.SelectedIndex=0;
					}
					else {
						if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboExams.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboExams.SelectedIndex=1;//# per year
						}
					}
				}
				#endregion
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
				//Stand1
				//Stand2
				//Stand4
				//Diagnostic
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
				//any benefit that didn't get handled above
				else {
					_listBenefits.Add(benefit);
				}
				#endregion Loop
			}
			//Canadian BWs
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")//All of Canada, including Quebec (the proc codes are the same in this instance).
				&& string.IsNullOrWhiteSpace(textBW.Text)) 
			{ 
				Benefit benefitBW=_listBenefits.FirstOrDefault(x => Benefits.IsBitewingFrequency(x)
					&& ProcedureCodes.GetStringProcCode(x.CodeNum)=="02144"//4BW
					&& x.BenefitType==InsBenefitType.Limitations
					//&& ben.CovCatNum==CovCats.GetForEbenCat(EbenefitCategory.Db).CovCatNum//ignored
					&& x.MonetaryAmt==-1
					&& x.PatPlanNum==0
					&& x.Percent==-1
					&& (x.QuantityQualifier==BenefitQuantity.Months
						|| x.QuantityQualifier==BenefitQuantity.Years
						|| x.QuantityQualifier==BenefitQuantity.NumberOfServices));
				if(benefitBW!=null) {
					if(benefitBW.QuantityQualifier==BenefitQuantity.Months) {
						comboBW.SelectedIndex=2;
					}
					else if(benefitBW.QuantityQualifier==BenefitQuantity.Years) {
						comboBW.SelectedIndex=0;
					}
					else {
						if(benefitBW.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
					_listBenefits.Remove(benefitBW);
				}
			}
			else if(string.IsNullOrWhiteSpace(textBW.Text)) {
				Benefit benefitBW=_listBenefits.FirstOrDefault(x => Benefits.IsBitewingFrequency(x));
				if(benefitBW!=null) {
					textBW.Text=benefitBW.Quantity.ToString();
					if(benefitBW.QuantityQualifier==BenefitQuantity.Months){
						comboBW.SelectedIndex=2;
					}
					else if(benefitBW.QuantityQualifier==BenefitQuantity.Years){
						comboBW.SelectedIndex=0;
					}
					else{
						if(benefitBW.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboBW.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboBW.SelectedIndex=1;//# per year
						}
					}
					_listBenefits.Remove(benefitBW);
				}
			}
			if(string.IsNullOrWhiteSpace(textPano.Text)) {
				Benefit benefitPano=_listBenefits.FirstOrDefault(x => Benefits.IsPanoFrequency(x));
				if(benefitPano!=null) {
					textPano.Text=benefitPano.Quantity.ToString();
					if(benefitPano.QuantityQualifier==BenefitQuantity.Months){
						comboPano.SelectedIndex=2;
					}
					else if(benefitPano.QuantityQualifier==BenefitQuantity.Years){
						comboPano.SelectedIndex=0;
					}
					else{
						if(benefitPano.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboPano.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboPano.SelectedIndex=1;//# per year
						}
					}
					_listBenefits.Remove(benefitPano);
				}
			}
			else if(string.IsNullOrWhiteSpace(textExams.Text)) {
				Benefit benefitExam=_listBenefits.FirstOrDefault(x => Benefits.IsExamFrequency(x));
				if(benefitExam!=null) {
					textExams.Text=benefitExam.Quantity.ToString();
					if(benefitExam.QuantityQualifier==BenefitQuantity.Months) {
						comboExams.SelectedIndex=2;
					}
					else if(benefitExam.QuantityQualifier==BenefitQuantity.Years) {
						comboExams.SelectedIndex=0;
					}
					else {
						if(benefitExam.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
							comboExams.SelectedIndex=3;//# in last 12 months
						}
						else {
							comboExams.SelectedIndex=1;//# per year
						}
					}
					_listBenefits.Remove(benefitExam);
				}
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
		private void TryResetQuickText(TextBox textBox,TextBox textBoxStand) {
			if(textBox.Text!=textBoxStand.Text) {
				_isResettingQuickText=true;
				textBoxStand.Text="";
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
			for(int i=0;i<_listBenefitsAll.Count;i++) {
				if(_listBenefitsAll[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
					_listBenefitsAll[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				if(_listBenefitsAll[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
					_listBenefitsAll[i].TimePeriod=BenefitTimePeriod.CalendarYear;
				}
			}
			if(isCalendar) {
				checkCalendarYear.Checked=true;
				textMonth.Text="";
				textMonth.Enabled=false;
				return;
			}
			checkCalendarYear.Checked=false;
			textMonth.Text=MonthRenew.ToString();
			textMonth.Enabled=true;
		}

		///<summary>This only fills the grid on the screen.  It does not get any data from the database.</summary>
		private void FillGrid() {
			_listBenefits.Sort();
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
			col=new GridColumn("%",35);//,HorizontalAlignment.Right);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Amt",50);//,HorizontalAlignment.Right);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Time Period",80);
			gridBenefits.Columns.Add(col);
			col=new GridColumn("Quantity",115);
			gridBenefits.Columns.Add(col);
			gridBenefits.ListGridRows.Clear();
			GridRow row;
			//bool isCalendarYear=true;
			for(int i=0;i<_listBenefits.Count;i++) {
				row=new GridRow();
				if(_listBenefits[i].PatPlanNum==0) {//attached to plan
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");
				}
				if(_listBenefits[i].CoverageLevel==BenefitCoverageLevel.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(Lan.g("enumBenefitCoverageLevel",_listBenefits[i].CoverageLevel.ToString()));
				}
				if(_listBenefits[i].BenefitType==InsBenefitType.CoInsurance && _listBenefits[i].Percent != -1) {
					row.Cells.Add("%");
				}
				//else if(((Benefit)benefitList[i]).BenefitType==InsBenefitType.Limitations
				//	&& (((Benefit)benefitList[i]).TimePeriod==BenefitTimePeriod.ServiceYear
				//	|| ((Benefit)benefitList[i]).TimePeriod==BenefitTimePeriod.CalendarYear)
				//	&& ((Benefit)benefitList[i]).QuantityQualifier==BenefitQuantity.None) {//annual max
				//	row.Cells.Add(Lan.g(this,"Annual Max"));
				//}
				else {
					row.Cells.Add(Lan.g("enumInsBenefitType",_listBenefits[i].BenefitType.ToString()));
				}
				row.Cells.Add(Benefits.GetCategoryString(_listBenefits[i]));
				if(_listBenefits[i].Percent == -1) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listBenefits[i].Percent.ToString());
				}
				row.Cells.Add(_listBenefits[i].GetDisplayMonetaryAmt());
				if(_listBenefits[i].TimePeriod==BenefitTimePeriod.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("enumBenefitTimePeriod",_listBenefits[i].TimePeriod.ToString()));
				}
				if(_listBenefits[i].Quantity>0) {
					row.Cells.Add(_listBenefits[i].Quantity.ToString()+" "
						+Lan.g("enumBenefitQuantity",_listBenefits[i].QuantityQualifier.ToString()));
				}
				else {
					row.Cells.Add("");
				}
				gridBenefits.ListGridRows.Add(row);
			}
			gridBenefits.EndUpdate();
		}

		private void gridBenefits_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e){
			int benefitListI=_listBenefits.IndexOf(_listBenefits[e.Row]);
			int benefitListAllI=_listBenefitsAll.IndexOf(_listBenefits[e.Row]);
			using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(_patPlanNum,_planNum);
			formBenefitEdit.BenefitCur=_listBenefits[e.Row];
			formBenefitEdit.ShowDialog();
			if(formBenefitEdit.BenefitCur==null){//user deleted
				_listBenefits.RemoveAt(benefitListI);
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
			for(int i=0;i<_listBenefits.Count;i++) {
				if(_listBenefits[i].TimePeriod==BenefitTimePeriod.CalendarYear && !isCalendar) {
					return true;
				}
				if(_listBenefits[i].TimePeriod==BenefitTimePeriod.ServiceYear && isCalendar) {
					return true;
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
				for(int i=0;i<_listBenefits.Count;i++){
					if(_listBenefits[i].TimePeriod==BenefitTimePeriod.ServiceYear){
						_listBenefits[i].TimePeriod=BenefitTimePeriod.CalendarYear;
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
			for(int i=0;i<_listBenefits.Count;i++) {
				if(_listBenefits[i].TimePeriod==BenefitTimePeriod.CalendarYear) {
					_listBenefits[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			FillGrid();
		}

		private void butMoreFrequencies_Click(object sender,EventArgs e) {
			if(!ConvertFormToBenefits()) {
				return;
			}
			using FormBenefitFrequencies formBenefitFrequencies=new FormBenefitFrequencies(_listBenefitsAll,_planNum,checkCalendarYear.Checked);
			if(formBenefitFrequencies.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_listBenefitsAll=formBenefitFrequencies.ListBenefits;
			FillSimple();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Benefit benefit=new Benefit();
			benefit.PlanNum=_planNum;
			if(checkCalendarYear.CheckState==CheckState.Checked) {
				benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
			}
			if(checkCalendarYear.CheckState==CheckState.Unchecked) {
				benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
			}
			if(CovCats.GetCount(true) > 0){
				benefit.CovCatNum=CovCats.GetFirst(true).CovCatNum;
			}
			benefit.BenefitType=InsBenefitType.CoInsurance;
			using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(_patPlanNum,_planNum);
			formBenefitEdit.IsNew=true;
			formBenefitEdit.BenefitCur=benefit;
			formBenefitEdit.ShowDialog();
			if(formBenefitEdit.DialogResult!=DialogResult.OK) {
				FillGrid();
				return;
			}
			_listBenefits.Add(formBenefitEdit.BenefitCur);
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
				listBenefits.Add(_listBenefits[gridBenefits.SelectedIndices[i]]);
			}
			for(int i=0;i<listBenefits.Count;i++) {
				_listBenefits.Remove(listBenefits[i]);
				_listBenefitsAll.Remove(listBenefits[i]);
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
			_listBenefitsAll=new List<Benefit>(_listBenefits);
			Benefit benefit;
			//annual max individual
			if(textAnnualMax.Text !=""){
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(checkCalendarYear.Checked){
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
			//deductible individual
			if(textDeductible.Text !=""){
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
			//Flo
			if(textFlo.Text !=""){
				benefit=Benefits.CreateAgeLimitationBenefit(ProcedureCodes.GetCodeNum(ProcedureCodes.FlourideCode),PIn.Byte(textFlo.Text),_planNum);
				_listBenefitsAll.Add(benefit);
			}
			if(textSealantAge.Text!="") {
				benefit=Benefits.CreateAgeLimitationBenefit(ProcedureCodes.GetCodeNum(ProcedureCodes.SealantCode),PIn.Byte(textSealantAge.Text),_planNum);
				_listBenefitsAll.Add(benefit);
			}
			//Frequency BW group
			if(textBW.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=ProcedureCodes.GetCodeNum(ProcedureCodes.BitewingCode);
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(comboBW.SelectedIndex==0){
					benefit.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboBW.SelectedIndex==1){
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboBW.SelectedIndex==2){
					benefit.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboBW.SelectedIndex==3) {
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					benefit.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				benefit.Quantity=PIn.Byte(textBW.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				_listBenefitsAll.Add(benefit);
			}
			//Frequency pano/FMX group
			if(textPano.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=ProcedureCodes.GetCodeNum(ProcedureCodes.PanoCode);
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=0;
				benefit.PlanNum=_planNum;
				if(comboPano.SelectedIndex==0) {
					benefit.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboPano.SelectedIndex==1) {
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboPano.SelectedIndex==2) {
					benefit.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboPano.SelectedIndex==3) {
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					benefit.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				benefit.Quantity=PIn.Byte(textPano.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				_listBenefitsAll.Add(benefit);
			}
			//Frequency in Exams group
			if(textExams.Text !="") {
				benefit=new Benefit();
				benefit.CodeNum=0;
				benefit.BenefitType=InsBenefitType.Limitations;
				benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
				benefit.PlanNum=_planNum;
				if(comboExams.SelectedIndex==0) {
					benefit.QuantityQualifier=BenefitQuantity.Years;
				}
				else if(comboExams.SelectedIndex==1) {
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(checkCalendarYear.Checked) {
						benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
				}
				else if(comboExams.SelectedIndex==2) {
					benefit.QuantityQualifier=BenefitQuantity.Months;
				}
				else if(comboExams.SelectedIndex==3) {
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					benefit.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
				}
				benefit.Quantity=PIn.Byte(textExams.Text);
				//ben.TimePeriod is none for years or months, although calYear, or ServiceYear, or Years might work too
				_listBenefitsAll.Add(benefit);
			}
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
			return true;
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
				for(int i=0;i<_listBenefits.Count;i++) {
					ListBenefits.Add(_listBenefits[i]);
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





















