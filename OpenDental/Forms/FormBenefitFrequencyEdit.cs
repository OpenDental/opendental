using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBenefitFrequencyEdit:FormODBase {
		public Benefit BenefitCur;

		public FormBenefitFrequencyEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBenefitFrequencyEdit_Load(object sender,EventArgs e) {
			List<CodeGroup> listCodeGroups=CodeGroups.GetDeepCopy(isShort:true);
			listBoxCodeGroup.Items.AddList<CodeGroup>(listCodeGroups, x=>x.GroupName);
			comboFrequencyOptions.Items.AddEnums<FrequencyOptions>();
			if(BenefitCur.IsNew) { 
				comboFrequencyOptions.SetSelectedEnum(FrequencyOptions.Every_Years);
				return;
			}
			int indexCodeGroup=listCodeGroups.FindIndex(x=>x.CodeGroupNum==BenefitCur.CodeGroupNum);
			listBoxCodeGroup.SetSelected(indexCodeGroup);
			comboFrequencyOptions.SetSelectedEnum(DetermineBenefitFrequencyOption(BenefitCur));
			textNumber.Text=BenefitCur.Quantity.ToString();
		}

		private void SetupFrequencyBenefitForSelectedCodeGroup(long codeGroupNum,FrequencyOptions frequencyOption,byte quantity) {
			BenefitCur.CodeGroupNum=codeGroupNum;
			BenefitCur.TimePeriod=BenefitTimePeriod.None;
			if(frequencyOption==FrequencyOptions.Every_Years) {
				BenefitCur.QuantityQualifier=BenefitQuantity.Years;
			}
			else if(frequencyOption==FrequencyOptions._PerBenefitYear) {
				BenefitCur.QuantityQualifier=BenefitQuantity.NumberOfServices;
				// This logic is the definition of 'IsCalendar' in FormInsBenefits
				if(InsPlans.GetPlan(BenefitCur.PlanNum, null).MonthRenew==0) {
					BenefitCur.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
				else {
					BenefitCur.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			else if(frequencyOption==FrequencyOptions.Every_Months) {
				BenefitCur.QuantityQualifier=BenefitQuantity.Months;
			}
			else if(frequencyOption==FrequencyOptions._InLast12Months) {
				BenefitCur.QuantityQualifier=BenefitQuantity.NumberOfServices;
				BenefitCur.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
			}
			BenefitCur.Quantity=quantity;
			BenefitCur.IsNew=false;
		}

		private FrequencyOptions DetermineBenefitFrequencyOption(Benefit benefit) {
			if(benefit.QuantityQualifier==BenefitQuantity.Years) {
				return FrequencyOptions.Every_Years;
			}
			else if(benefit.TimePeriod==BenefitTimePeriod.ServiceYear || benefit.TimePeriod==BenefitTimePeriod.CalendarYear) {
				return FrequencyOptions._PerBenefitYear;
			}
			else if(benefit.QuantityQualifier==BenefitQuantity.Months) {
				return FrequencyOptions.Every_Months;
			}
			else if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
				return FrequencyOptions._InLast12Months;
			}
			throw new Exception("Frequency Benefit did not have a matching Frequency Option");
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textNumber.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			CodeGroup codeGroup=listBoxCodeGroup.GetSelected<CodeGroup>();
			if(codeGroup==null) {
				MsgBox.Show(this,"Please select a Code Group.");
				return;
			}
			SetupFrequencyBenefitForSelectedCodeGroup(codeGroup.CodeGroupNum,comboFrequencyOptions.GetSelected<FrequencyOptions>(), PIn.Byte(textNumber.Text));
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			BenefitCur=null;
			DialogResult=DialogResult.Cancel;
		}
	}
}