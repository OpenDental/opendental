using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBenefitFrequencies:FormODBase {
		///<summary>All benefits for this patient.</summary>
		private List<Benefit> _listBenefitsAll;
		///<summary>The frequency benefits that are displayed in this window when the form loads.</summary>
		private List<Benefit> _listFrequencyBenefits=new List<Benefit>();
		///<summary>The current insurance plan PlanNum.</summary>
		private long _planNum;
		///<summary>Indicates if the time period will be using the calendar year or service year if applicable.</summary>
		private bool _useCalendarYear;

		///<summary>All benefits for the current plan, including the new ones added in this form.</summary>
		public List<Benefit> ListBenefits {
			get {
				return _listBenefitsAll;
			}
		}

		public FormBenefitFrequencies(List<Benefit> listBenefits,long planNum,bool useCalendarYear) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listBenefitsAll=listBenefits;
			_planNum=planNum;
			_useCalendarYear=useCalendarYear;
		}

		private void FormBenefitFrequencies_Load(object sender,EventArgs e) {
			foreach(ComboBox comboBox in UIHelper.GetAllControls(this).OfType<ComboBox>()) {
				comboBox.SelectedIndex=0;//Every # years
			}
			FillFrequencies();
		}

		private void FillFrequencies() {
			foreach(Benefit ben in _listBenefitsAll) {
				bool isFrequencyBen=true;
				//Diagnostic group
				if(Benefits.IsBitewingFrequency(ben)) {
					FillBenefit(ben,textBW,comboBW);
				}
				else if(Benefits.IsPanoFrequency(ben)) {
					FillBenefit(ben,textPano,comboPano);
				}
				else if(Benefits.IsExamFrequency(ben)) {
					FillBenefit(ben,textExams,comboExams);
				}
				else if(Benefits.IsCancerScreeningFrequency(ben)) {
					FillBenefit(ben,textCancerScreenings,comboCancerScreenings);
				}
				//Preventive group
				else if(Benefits.IsProphyFrequency(ben)) {
					FillBenefit(ben,textProphy,comboProphy);
				}
				else if(Benefits.IsFlourideFrequency(ben)) {
					FillBenefit(ben,textFlouride,comboFlouride);
				}
				else if(Benefits.IsSealantFrequency(ben)) {
					FillBenefit(ben,textSealants,comboSealants);
				}
				//Restorative group
				else if(Benefits.IsCrownFrequency(ben)) {
					FillBenefit(ben,textCrown,comboCrown);
				}
				//Periodontal group
				else if(Benefits.IsSRPFrequency(ben)) {
					FillBenefit(ben,textSRP,comboSRP);
				}
				else if(Benefits.IsFullDebridementFrequency(ben)) {
					FillBenefit(ben,textDebridement,comboDebridement);
				}
				else if(Benefits.IsPerioMaintFrequency(ben)) {
					FillBenefit(ben,textPerioMaint,comboPerioMaint);
				}
				//Prosthodontics group
				else if(Benefits.IsDenturesFrequency(ben)) {
					FillBenefit(ben,textDentures,comboDentures);
				}
				//Implants group
				else if(Benefits.IsImplantFrequency(ben)) {
					FillBenefit(ben,textImplant,comboImplant);
				}
				else {
					isFrequencyBen=false;
				}
				if(isFrequencyBen) {
					_listFrequencyBenefits.Add(ben);
				}
			}
		}

		private void FillBenefit(Benefit ben,ValidNum textBoxBenefit,ComboBox comboBenefit) {
			textBoxBenefit.Text=ben.Quantity.ToString();
			if(ben.QuantityQualifier==BenefitQuantity.Years) {
				comboBenefit.SelectedIndex=0;//Every # Years
			}
			else if(ben.QuantityQualifier==BenefitQuantity.NumberOfServices) {
				if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
					comboBenefit.SelectedIndex=3;//# Per 12 Months
				}
				else {//BenefitTimePeriod CalendarYear or ServiceYear
					comboBenefit.SelectedIndex=1;//# Per Year
				}
			}
			else {
				comboBenefit.SelectedIndex=2;//Every # Months
			}
		}

		private void AddBenefits() {
			try {
				CreateFrequencyBenefit(textBW,comboBW,ProcedureCodes.BitewingCode);
				CreateFrequencyBenefit(textPano,comboPano,ProcedureCodes.PanoCode);
				if(PIn.Byte(textExams.Text,false)!=0) {
					Benefit ben=Benefits.CreateFrequencyBenefit(EbenefitCategory.RoutinePreventive,PIn.Byte(textExams.Text),GetQuantityQualifier(comboExams),
						_planNum,GetTimePeriod(comboExams.SelectedIndex));
					_listBenefitsAll.Add(ben);
				}
				CreateFrequencyBenefit(textCancerScreenings,comboCancerScreenings,ProcedureCodes.CancerScreeningCode);
				CreateFrequencyBenefit(textProphy,comboProphy,ProcedureCodes.ProphyCode);
				CreateFrequencyBenefit(textFlouride,comboFlouride,ProcedureCodes.FlourideCode);
				CreateFrequencyBenefit(textSealants,comboSealants,ProcedureCodes.SealantCode);
				CreateFrequencyBenefit(textCrown,comboCrown,ProcedureCodes.CrownCode);
				CreateFrequencyBenefit(textSRP,comboSRP,ProcedureCodes.SRPCode);
				CreateFrequencyBenefit(textDebridement,comboDebridement,ProcedureCodes.FullDebridementCode);
				CreateFrequencyBenefit(textPerioMaint,comboPerioMaint,ProcedureCodes.PerioMaintCode);
				CreateFrequencyBenefit(textDentures,comboDentures,ProcedureCodes.DenturesCode);
				CreateFrequencyBenefit(textImplant,comboImplant,ProcedureCodes.ImplantCode);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"An error occurred"),ex);
			}
		}

		private void CreateFrequencyBenefit(ValidNum textBox,ComboBox comboBox,string procCodeStr) {
			if(PIn.Byte(textBox.Text,false)==0 || ProcedureCodes.GetCodeNum(procCodeStr)==0) {
				return;
			}
			Benefit ben=Benefits.CreateFrequencyBenefit(ProcedureCodes.GetCodeNum(procCodeStr),PIn.Byte(textBox.Text),GetQuantityQualifier(comboBox),
				_planNum,GetTimePeriod(comboBox.SelectedIndex));
			_listBenefitsAll.Add(ben);
		}

		private BenefitQuantity GetQuantityQualifier(ComboBox comboBox) {
			switch(comboBox.SelectedIndex) {
				case 0:
					return BenefitQuantity.Years;
				case 1:
					return BenefitQuantity.NumberOfServices;
				case 2:
					return BenefitQuantity.Months;
				case 3:
					return BenefitQuantity.NumberOfServices;
				default:
					throw new ApplicationException("Invalid combo box selection");
			}
		}

		///<summary>Returns the BenefitTimePeriod for the given combobox.</summary>
		private BenefitTimePeriod GetTimePeriod(int selectedIndex) {
			if(selectedIndex==3) {
				return BenefitTimePeriod.NumberInLast12Months;
			}
			else if(selectedIndex==1) {
				if(_useCalendarYear) {//If not # per 12 months, either calendar year or service year depending on what was passed in.
					return BenefitTimePeriod.CalendarYear;
				}
				else {
					return BenefitTimePeriod.ServiceYear;
				}
			}
			return BenefitTimePeriod.None;	
		}

		///<summary>Recursive needed for nested controls.</summary>
		private IEnumerable<Control> GetAllControls(Control control) {
			IEnumerable<Control> controls=control.Controls.OfType<Control>();
			return controls.SelectMany(GetAllControls).Concat(controls);
		}

		private void butOK_Click(object sender,EventArgs e) {
			IEnumerable<Control> controls=GetAllControls(this);
			if(controls.OfType<ValidNum>().Any(x => !x.IsValid())) {
				MsgBox.Show(this,"Please fix data errors first.");
				return;
			}
			_listFrequencyBenefits.ForEach(x => _listBenefitsAll.Remove(x));
			AddBenefits();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}