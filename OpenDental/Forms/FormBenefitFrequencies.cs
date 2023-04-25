using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormBenefitFrequencies:FormODBase {
		///<summary>All benefits for the current plan, including the new ones added in this form.</summary>
		public List<Benefit> ListBenefits;
		///<summary>The frequency benefits that are displayed in this window when the form loads.</summary>
		private List<Benefit> _listBenefitsFrequency=new List<Benefit>();
		///<summary>The current insurance plan PlanNum.</summary>
		private long _planNum;
		///<summary>Indicates if the time period will be using the calendar year or service year if applicable.</summary>
		private bool _useCalendarYear;

		public FormBenefitFrequencies(List<Benefit> listBenefits,long planNum,bool useCalendarYear) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ListBenefits=listBenefits;
			_planNum=planNum;
			_useCalendarYear=useCalendarYear;
		}

		private void FormBenefitFrequencies_Load(object sender,EventArgs e) {
			List<string> listFrequencyOptions=new List<string>() {
				"Every # Years",
				"# Per Benefit Year",
				"Every # Months",
				"# in Last 12 Months"
			};
			List<UI.ComboBox> listComboBoxes = UIHelper.GetAllControls(this).OfType<UI.ComboBox>().ToList();
			for(int i=0;i<listComboBoxes.Count;++i) {
				listComboBoxes[i].Items.AddList(listFrequencyOptions);
				listComboBoxes[i].SelectedIndex=0;//Every # years
			}
			FillFrequencies();
		}

		private void FillFrequencies() {
			for(int i = 0;i<ListBenefits.Count;++i) {
				bool isFrequencyBen=true;
				//Diagnostic group
				if(string.IsNullOrWhiteSpace(textBW.Text)//Jordan: Why is it testing this?
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenBWCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsBitewingFrequency(ListBenefits[i]))
				{
					FillBenefit(ListBenefits[i],textBW,comboBW);
				}
				else if(string.IsNullOrWhiteSpace(textPano.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenPanoCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsPanoFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textPano,comboPano);
				}
				//Exam frequency limitations have codeNum 0. Distinguished by covcat.
				else if(string.IsNullOrWhiteSpace(textExams.Text)
					&& Benefits.IsExamFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textExams,comboExams);
				}
				else if(string.IsNullOrWhiteSpace(textCancerScreenings.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenCancerScreeningCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsCancerScreeningFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textCancerScreenings,comboCancerScreenings);
				}
				//Preventive group
				else if(string.IsNullOrWhiteSpace(textProphy.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenProphyCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsProphyFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textProphy,comboProphy);
				}
				else if(string.IsNullOrWhiteSpace(textFlouride.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenFlourideCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsFlourideFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textFlouride,comboFlouride);
				}
				else if(string.IsNullOrWhiteSpace(textSealants.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenSealantCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsSealantFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textSealants,comboSealants);
				}
				//Restorative group
				else if(string.IsNullOrWhiteSpace(textCrown.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenCrownCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsCrownFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textCrown,comboCrown);
				}
				//Periodontal group
				else if(string.IsNullOrWhiteSpace(textSRP.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenSRPCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsSRPFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textSRP,comboSRP);
				}
				else if(string.IsNullOrWhiteSpace(textDebridement.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenFullDebridementCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsFullDebridementFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textDebridement,comboDebridement);
				}
				else if(string.IsNullOrWhiteSpace(textPerioMaint.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenPerioMaintCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsPerioMaintFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textPerioMaint,comboPerioMaint);
				}
				//Prosthodontics group
				else if(string.IsNullOrWhiteSpace(textDentures.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenDenturesCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsDenturesFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textDentures,comboDentures);
				}
				//Implants group
				else if(string.IsNullOrWhiteSpace(textImplant.Text)
					&& ProcedureCodes.GetCodeNumForPref(PrefName.InsBenImplantCodes)==ListBenefits[i].CodeNum 
					&& Benefits.IsImplantFrequency(ListBenefits[i])) 
				{
					FillBenefit(ListBenefits[i],textImplant,comboImplant);
				}
				else {
					isFrequencyBen=false;
				}
				if(isFrequencyBen) {
					_listBenefitsFrequency.Add(ListBenefits[i]);
				}
			}
			Benefit benefit;
			if(string.IsNullOrWhiteSpace(textBW.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsBitewingFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textBW,comboBW);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textPano.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsPanoFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textPano,comboPano);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textCancerScreenings.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsCancerScreeningFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textCancerScreenings,comboCancerScreenings);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textProphy.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsProphyFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textProphy,comboProphy);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textFlouride.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsFlourideFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textFlouride,comboFlouride);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textSealants.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsSealantFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textSealants,comboSealants);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textCrown.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsCrownFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textCrown,comboCrown);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textSRP.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsSRPFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textSRP,comboSRP);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textDebridement.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsFullDebridementFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textDebridement,comboDebridement);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textDentures.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsDenturesFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textDentures,comboDentures);
					_listBenefitsFrequency.Add(benefit);
				}
			}
			if(string.IsNullOrWhiteSpace(textImplant.Text)) {
				benefit=ListBenefits.FirstOrDefault(x => Benefits.IsImplantFrequency(x));
				if(benefit!=null) {
					FillBenefit(benefit,textImplant,comboImplant);
					_listBenefitsFrequency.Add(benefit);
				}
			}
		}

		private void FillBenefit(Benefit ben,ValidNum textBoxBenefit,UI.ComboBox comboBenefit) {
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
				CreateFrequencyBenefit(textBW,comboBW,ProcedureCodes.GetProcCode(PrefName.InsBenBWCodes));
				CreateFrequencyBenefit(textPano,comboPano,ProcedureCodes.GetProcCode(PrefName.InsBenPanoCodes));
				if(PIn.Byte(textExams.Text,false)!=0) {
					Benefit ben=Benefits.CreateFrequencyBenefit(EbenefitCategory.RoutinePreventive,PIn.Byte(textExams.Text),GetQuantityQualifier(comboExams),
						_planNum,GetTimePeriod(comboExams.SelectedIndex));
					ListBenefits.Add(ben);
				}
				CreateFrequencyBenefit(textCancerScreenings,comboCancerScreenings,ProcedureCodes.GetProcCode(PrefName.InsBenCancerScreeningCodes));
				CreateFrequencyBenefit(textProphy,comboProphy,ProcedureCodes.GetProcCode(PrefName.InsBenProphyCodes));
				CreateFrequencyBenefit(textFlouride,comboFlouride,ProcedureCodes.GetProcCode(PrefName.InsBenFlourideCodes));
				CreateFrequencyBenefit(textSealants,comboSealants,ProcedureCodes.GetProcCode(PrefName.InsBenSealantCodes));
				CreateFrequencyBenefit(textCrown,comboCrown,ProcedureCodes.GetProcCode(PrefName.InsBenCrownCodes));
				CreateFrequencyBenefit(textSRP,comboSRP,ProcedureCodes.GetProcCode(PrefName.InsBenSRPCodes));
				CreateFrequencyBenefit(textDebridement,comboDebridement,ProcedureCodes.GetProcCode(PrefName.InsBenFullDebridementCodes));
				CreateFrequencyBenefit(textPerioMaint,comboPerioMaint,ProcedureCodes.GetProcCode(PrefName.InsBenPerioMaintCodes));
				CreateFrequencyBenefit(textDentures,comboDentures,ProcedureCodes.GetProcCode(PrefName.InsBenDenturesCodes));
				CreateFrequencyBenefit(textImplant,comboImplant,ProcedureCodes.GetProcCode(PrefName.InsBenImplantCodes));
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"An error occurred"),ex);
			}
		}

		private void CreateFrequencyBenefit(ValidNum textBox,UI.ComboBox comboBox,string procCodeStr) {
			if(PIn.Byte(textBox.Text,false)==0 || ProcedureCodes.GetCodeNum(procCodeStr)==0) {
				return;
			}
			Benefit ben=Benefits.CreateFrequencyBenefit(ProcedureCodes.GetCodeNum(procCodeStr),PIn.Byte(textBox.Text),GetQuantityQualifier(comboBox),
				_planNum,GetTimePeriod(comboBox.SelectedIndex));
			ListBenefits.Add(ben);
		}

		private BenefitQuantity GetQuantityQualifier(UI.ComboBox comboBox) {
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
			_listBenefitsFrequency.ForEach(x => ListBenefits.Remove(x));
			AddBenefits();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}