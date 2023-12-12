using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.Crud;

namespace OpenDental {
	public partial class FormBenefitFrequencyEdit:FormODBase {
		public Benefit BenefitCur;
		public long PatPlanNum;

		public FormBenefitFrequencyEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBenefitFrequencyEdit_Load(object sender,EventArgs e) {
			List<CodeGroup> listCodeGroups=CodeGroups.GetDeepCopy(isShort:true);
			listBoxCodeGroup.Items.AddList<CodeGroup>(listCodeGroups, x=>x.GroupName);
			if(BenefitCur.CodeGroupNum>0) {
				listBoxCodeGroup.SetSelectedKey<CodeGroup>(BenefitCur.CodeGroupNum, x=>x.CodeGroupNum);
			}
			textNumber.Text=BenefitCur.Quantity.ToString();
			listBoxTimePeriod.Items.AddEnums<FrequencyOptions>();
			listBoxTimePeriod.SetSelectedEnum(BenefitCur.GetFrequencyOption());
			listBoxTreatArea.Items.Add("All",TreatmentArea.None);//replaces None for ui
			List<TreatmentArea> listTreatmentAreas =typeof(TreatmentArea).GetEnumValues()
				.Cast<TreatmentArea>()
				.Where(x => x!=TreatmentArea.None)
				.ToList();
			listBoxTreatArea.Items.AddList(listTreatmentAreas,x=>x.ToString());
			listBoxTreatArea.SetSelected((int)BenefitCur.TreatArea);
			checkPat.Checked=BenefitCur.PatPlanNum!=0;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textNumber.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listBoxCodeGroup.GetSelected<CodeGroup>()==null) {
				MsgBox.Show(this,"Please select a Code Group.");
				return;
			}
			CodeGroup codeGroup=listBoxCodeGroup.GetSelected<CodeGroup>();
			BenefitCur.CodeGroupNum=codeGroup.CodeGroupNum;
			FrequencyOptions frequencyOptions=listBoxTimePeriod.GetSelected<FrequencyOptions>();
			bool isCalendarYr=InsPlans.GetPlan(BenefitCur.PlanNum, null).MonthRenew==0;
			BenefitCur.SetFrequencyOption(frequencyOptions,isCalendarYr);
			BenefitCur.Quantity=PIn.Byte(textNumber.Text);
			BenefitCur.TreatArea=listBoxTreatArea.GetSelected<TreatmentArea>();
			BenefitCur.IsNew=false;
			BenefitCur.PatPlanNum=0;
			if(checkPat.Checked) {
				BenefitCur.PatPlanNum=PatPlanNum;
				BenefitCur.PlanNum=0;
			}
			DialogResult=DialogResult.OK;
		}

		//private void butDelete_Click(object sender,EventArgs e) {
		//	BenefitCur=null;
		//	DialogResult=DialogResult.Cancel;
		//}
	}
}