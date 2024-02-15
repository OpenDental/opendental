using OpenDentBusiness;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	///<summary></summary>
	public partial class FormBenefitEdit : FormODBase {
		///<summary>Set this to the benefit that is going to be edited before loading the window. This object will be manipulated if the user makes changes and clicks OK. Gets set to null if the user clicks Delete.</summary>
		public Benefit BenefitCur;
		private long _planNum;
		private long _patPlanNum;

		///<summary></summary>
		public FormBenefitEdit(long patPlanNum,long planNum) {
			InitializeComponent();
			InitializeLayoutManager();
			_patPlanNum=patPlanNum;
			_planNum=planNum;
			Lan.F(this);
		}

		private void FormBenefitEdit_Load(object sender, System.EventArgs e) {
			if(BenefitCur==null) {
				MessageBox.Show("Benefit cannot be null.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit)) {
				butSave.Enabled=false;
			}
			if(BenefitCur.PatPlanNum==0) {//attached to insplan
				checkPat.Checked=false;
			}
			else{
				checkPat.Checked=true;
			}
			if(!Benefits.IsFrequencyLimitation(BenefitCur) || BenefitCur.CodeGroupNum==0) {
				labelCodeGroup.Visible=false;
				comboCodeGroup.Visible=false;
			}
			List<CovCat> listCovCats=CovCats.GetDeepCopy(true);
			listCategory.Items.Clear();
			listCategory.Items.AddNone<CovCat>();
			listCategory.Items.AddList<CovCat>(listCovCats,x => x.Description);
			int selectedIndex=listCovCats.FindIndex(x => x.CovCatNum==BenefitCur.CovCatNum);
			listCategory.SelectedIndex=selectedIndex+1;//Add one due to the 'none' option always being first.
			textProcCode.Text=ProcedureCodes.GetStringProcCode(BenefitCur.CodeNum);
			CodeGroup codeGroupNone=new CodeGroup();
			codeGroupNone.CodeGroupNum=0;
			codeGroupNone.ProcCodes="";
			codeGroupNone.GroupName=Lans.g(this,"None");
			List<CodeGroup> listCodeGroups=new List<CodeGroup>();
			listCodeGroups.Add(codeGroupNone);
			listCodeGroups.AddRange(CodeGroups.GetDeepCopy(isShort:true));
			comboCodeGroup.Items.Clear();
			comboCodeGroup.Items.AddList(listCodeGroups,x => CodeGroups.GetGroupName(x));
			comboCodeGroup.SetSelectedKey<CodeGroup>(BenefitCur.CodeGroupNum,x => x.CodeGroupNum,x => CodeGroups.GetGroupName(x));
			listBenefitType.Items.Clear();
			listBenefitType.Items.AddEnums<InsBenefitType>();
			listBenefitType.SetSelectedEnum(BenefitCur.BenefitType);
			if(BenefitCur.Percent==-1) {
				textPercent.Text="";
			}
			else {
				textPercent.Text=BenefitCur.Percent.ToString();
			}
			textAmount.Text=BenefitCur.GetDisplayMonetaryAmt();
			listTimePeriod.Items.Clear();
			listTimePeriod.Items.AddEnums<BenefitTimePeriod>();
			listTimePeriod.SetSelectedEnum(BenefitCur.TimePeriod);
			textQuantity.Text=BenefitCur.Quantity.ToString();
			listQuantityQualifier.Items.Clear();
			listQuantityQualifier.Items.AddEnums<BenefitQuantity>();
			listQuantityQualifier.SetSelectedEnum(BenefitCur.QuantityQualifier);
			listCoverageLevel.Items.Clear();
			listCoverageLevel.Items.AddEnums<BenefitCoverageLevel>();
			listCoverageLevel.SetSelectedEnum(BenefitCur.CoverageLevel);
			listTreatArea.Items.Clear();
			listTreatArea.Items.Add("Default",TreatmentArea.None);
			listTreatArea.Items.AddEnums<TreatmentArea>();
			listTreatArea.Items.RemoveAt(1); // There are now two instances of TreatmentArea.None, remove the one that will display as "None"
			listTreatArea.SetSelectedEnum(BenefitCur.TreatArea);
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(BenefitCur.QuantityQualifier==BenefitQuantity.AgeLimit && PIn.Int(textQuantity.Text,false)==0)  {
				string messageText=Lan.g(this,"field is invalid.\r\n"
					+"Enter an age greater than 0 to denote coverage through that year, or click delete to remove this benefit.");
				MessageBox.Show(this,groupQuantity.Text+" "+messageText);
				return;
			}
			if(!textPercent.IsValid() || !textAmount.IsValid() || !textQuantity.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//Read the UI into variables for validation.
			long covCatNum=listCategory.GetSelectedKey<CovCat>(x => x.CovCatNum);
			long codeNum=ProcedureCodes.GetCodeNum(textProcCode.Text);
			long codeGroupNum=comboCodeGroup.GetSelectedKey<CodeGroup>(x => x.CodeGroupNum);
			InsBenefitType benefitType=listBenefitType.GetSelected<InsBenefitType>();
			BenefitQuantity quantityQualifier=listQuantityQualifier.GetSelected<BenefitQuantity>();
			int percent;
			if(textPercent.Text=="") {
				percent=-1;
			}
			else {
				percent=PIn.Int(textPercent.Text);
			}
			double monetaryAmt;
			if(textAmount.Text=="") {
				monetaryAmt=-1;
			}
			else {
				monetaryAmt=PIn.Double(textAmount.Text);
			}
			byte quantity=PIn.Byte(textQuantity.Text);
			BenefitTimePeriod timePeriod=listTimePeriod.GetSelected<BenefitTimePeriod>();
			BenefitCoverageLevel coverageLevel=listCoverageLevel.GetSelected<BenefitCoverageLevel>();
			TreatmentArea treatArea=(TreatmentArea)listTreatArea.SelectedIndex;
			int countCatProcCodeGroup=0;
			if(covCatNum!=0) {
				countCatProcCodeGroup++;
			}
			if(textProcCode.Text!="") {
				countCatProcCodeGroup++;
			}
			if(codeGroupNum > 0) {
				countCatProcCodeGroup++;
			}
			if(countCatProcCodeGroup > 1) {
				MsgBox.Show(this,"You may only enter one of Category, or Proc Code, or Code Group.");
				return;
			}
			//not allowed to set to pat if editing plan only, and no patplanNum is available
			if(_patPlanNum==0 && checkPat.Checked) {
				MsgBox.Show(this,"Not allowed to check the Pat box because no patient is available.");
				return;
			}
			if(benefitType!=InsBenefitType.CoInsurance && percent > 0) {//-1 or 0 is allowed for other types
				//We must allow Exclusion with 0 (this "if" must fail) to remain backward compatible for NADG
				MsgBox.Show(this,"Not allowed to enter a percentage unless type is CoInsurance.");
				return;
			}
			if(textProcCode.Text!="" && codeNum==0) {
				MsgBox.Show(this,"Invalid procedure code.");
				return;
			}
			if(benefitType==InsBenefitType.WaitingPeriod
				&& quantityQualifier!=BenefitQuantity.Months
				&& quantityQualifier!=BenefitQuantity.Years)
			{
				MsgBox.Show(this,"Waiting period must be months or years.");
				return;
			}
			if(benefitType==InsBenefitType.WaitingPeriod 
					&& covCatNum==0
					&& codeNum==0)
			{
				MsgBox.Show(this,"Waiting period must have a category or a procedure code.");
				return;
			}
			long patPlanNum;
			long planNum;
			if(checkPat.Checked) {
				patPlanNum=_patPlanNum;
				planNum=0;
			}
			else {
				patPlanNum=0;
				planNum=_planNum;
			}
			//Create a new benefit object out of the fields from the UI in order to invoke helper methods.
			//Validate seven extremely specific fields from the UI for limitation benefits in general.
			Benefit benefit=new Benefit();
			benefit.BenefitType=benefitType;
			benefit.MonetaryAmt=monetaryAmt;
			benefit.Percent=percent;
			benefit.QuantityQualifier=quantityQualifier;
			benefit.TimePeriod=timePeriod;
			benefit.CoverageLevel=coverageLevel;
			benefit.TreatArea=treatArea;
			benefit.Quantity=quantity;
			//Fluoride and sealant age limitations need to validate the code, code group, and patPlanNum.
			benefit.CodeGroupNum=codeGroupNum;
			benefit.CodeNum=codeNum;
			benefit.PatPlanNum=patPlanNum;
			if(Benefits.IsFrequencyLimitation(benefit) || Benefits.IsAgeLimit(benefit))
			{
				if(covCatNum > 0) {
					MsgBox.Show(this,"Category cannot be used for this limitation benefit.\r\nUse Proc Code or Code Group instead.");
					return;
				}
				//We check for an age limit above in if statement but there will never be a case where IsAgeLimit() is true and quantity is less than/equal to zero.
				if(quantity<=0) {
					MsgBox.Show(this,"Frequency Limitations cannot have a quantity of zero.");
					return;
				}
			}
			else {
				//Code Groups support fluoride and sealant age limitations along with frequency limitations.
				if(codeGroupNum > 0) {
					MsgBox.Show(this,"Code Group can only be used for fluoride age, sealant age, or frequency limitations.");
					return;
				}
			}
			if(!Benefits.IsFrequencyLimitation(benefit) && treatArea!=TreatmentArea.None) {
				MsgBox.Show(this,"Treatment Area can only be used for limitation benefits.");
				return;
			}
			//End of validation. Manipulate BenefitCur with the values from the UI.
			BenefitCur.PatPlanNum=patPlanNum;
			BenefitCur.PlanNum=planNum;
			BenefitCur.CovCatNum=covCatNum;
			BenefitCur.CodeNum=codeNum;
			BenefitCur.CodeGroupNum=codeGroupNum;
			BenefitCur.BenefitType=benefitType;
			BenefitCur.Percent=percent;
			BenefitCur.MonetaryAmt=monetaryAmt;
			BenefitCur.TimePeriod=timePeriod;
			BenefitCur.Quantity=quantity;
			BenefitCur.QuantityQualifier=quantityQualifier;
			BenefitCur.CoverageLevel=coverageLevel;
			BenefitCur.TreatArea=treatArea;
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(BenefitCur.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				DialogResult=DialogResult.OK;
			}
			BenefitCur=null;
		}
	}
}
