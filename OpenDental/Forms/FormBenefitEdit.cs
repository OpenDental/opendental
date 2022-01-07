/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	///<summary></summary>
	public partial class FormBenefitEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private ArrayList PosIndex=new ArrayList();
		private ArrayList NegIndex=new ArrayList();
		///<summary></summary>
		public Benefit BenefitCur;
		private long _planNum;
		private long _patPlanNum;
		private List<CovCat> _listCovCats;

		///<summary></summary>
		public FormBenefitEdit(long patPlanNum,long planNum) {
			InitializeComponent();
			InitializeLayoutManager();
			//BenCur=benCur.Clone();
			_patPlanNum=patPlanNum;
			_planNum=planNum;
			Lan.F(this);
		}

		private void FormBenefitEdit_Load(object sender, System.EventArgs e) {
			if(BenefitCur==null){
				MessageBox.Show("Benefit cannot be null.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!Security.IsAuthorized(Permissions.InsPlanEdit)) {
				butOK.Enabled=false;
			}
			if(BenefitCur.PatPlanNum==0){//attached to insplan
				checkPat.Checked=false;
			}
			else{
				checkPat.Checked=true;
			}
			listCategory.Items.Clear();
			listCategory.Items.Add(Lan.g(this,"None"));
			listCategory.SelectedIndex=0;
			_listCovCats=CovCats.GetDeepCopy(true);
			for(int i=0;i<_listCovCats.Count;i++) {
				listCategory.Items.Add(_listCovCats[i].Description);
				if(_listCovCats[i].CovCatNum==BenefitCur.CovCatNum){
					listCategory.SelectedIndex=i+1;
				}
			}
			textProcCode.Text=ProcedureCodes.GetStringProcCode(BenefitCur.CodeNum);
			listBenefitType.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(InsBenefitType)).Length;i++){
				listBenefitType.Items.Add(Lan.g("enumInsBenefitType",Enum.GetNames(typeof(InsBenefitType))[i]));
				if((int)BenefitCur.BenefitType==i){
					listBenefitType.SelectedIndex=i;
				}
			}
			if(BenefitCur.Percent==-1) {
				textPercent.Text="";
			}
			else {
				textPercent.Text=BenefitCur.Percent.ToString();
			}
			textAmount.Text=BenefitCur.GetDisplayMonetaryAmt();
			listTimePeriod.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(BenefitTimePeriod)).Length;i++) {
				listTimePeriod.Items.Add(Lan.g("enumBenefitTimePeriod",Enum.GetNames(typeof(BenefitTimePeriod))[i]));
				if((int)BenefitCur.TimePeriod==i) {
					listTimePeriod.SelectedIndex=i;
				}
			}
			textQuantity.Text=BenefitCur.Quantity.ToString();
			listQuantityQualifier.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(BenefitQuantity)).Length;i++) {
				listQuantityQualifier.Items.Add(Lan.g("enumBenefitQuantity",Enum.GetNames(typeof(BenefitQuantity))[i]));
				if((int)BenefitCur.QuantityQualifier==i) {
					listQuantityQualifier.SelectedIndex=i;
				}
			}
			for(int i=0;i<Enum.GetNames(typeof(BenefitCoverageLevel)).Length;i++){
				listCoverageLevel.Items.Add(Lan.g("enumBenefitCoverageLevel",Enum.GetNames(typeof(BenefitCoverageLevel))[i]));
				if((int)BenefitCur.CoverageLevel==i) {
					listCoverageLevel.SelectedIndex=i;
				}
			}
			//determine if this is an annual max
			/*if(textCode.Text==""
				&& listType.SelectedIndex==(int)InsBenefitType.Limitations
				&& (listTimePeriod.SelectedIndex==(int)BenefitTimePeriod.CalendarYear
				|| listTimePeriod.SelectedIndex==(int)BenefitTimePeriod.ServiceYear)
				&& listQuantityQualifier.SelectedIndex==(int)BenefitQuantity.None)
			{
				isAnnualMax=true;
			}*/
		}

		private void listType_Click(object sender,EventArgs e) {
			//selected index will already have changed
			//SetVisibilities();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(BenefitCur.QuantityQualifier==BenefitQuantity.AgeLimit && PIn.Int(textQuantity.Text,false)==0)  {
				string messageText=Lan.g(this," field is invalid.\r\n"
					+"Enter an age greater than 0 to denote coverage through that year, or click delete to remove this benefit.");
				MessageBox.Show(this,groupQuantity.Text+messageText);
				return;
			}
			if(!textPercent.IsValid() || !textAmount.IsValid() || !textQuantity.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textProcCode.Text != "" && listCategory.SelectedIndex != 0){
				MsgBox.Show(this,"If a procedure code is entered, category must be None.");
				return;
			}
			//not allowed to set to pat if editing plan only, and no patplanNum is available
			if(_patPlanNum==0 && checkPat.Checked){
				MsgBox.Show(this,"Not allowed to check the Pat box because no patient is available.");
				return;
			}
			if(listBenefitType.SelectedIndex != (int)InsBenefitType.CoInsurance && PIn.Long(textPercent.Text)!=0){
				MsgBox.Show(this,"Not allowed to enter a percentage unless type is CoInsurance.");
				return;
			}
			if(textProcCode.Text!="" && ProcedureCodes.GetCodeNum(textProcCode.Text)==0){
				MsgBox.Show(this,"Invalid procedure code.");
				return;
			}
			if(listBenefitType.SelectedIndex==(int)InsBenefitType.WaitingPeriod
				&& listQuantityQualifier.SelectedIndex!=(int)BenefitQuantity.Months
				&& listQuantityQualifier.SelectedIndex!=(int)BenefitQuantity.Years)
			{
				MsgBox.Show(this,"Waiting period must be months or years.");
				return;
			}
			if((InsBenefitType)listBenefitType.SelectedIndex==InsBenefitType.WaitingPeriod 
					&& listCategory.SelectedIndex==0 
					&& textProcCode.Text=="") 
			{
				MsgBox.Show(this,"Waiting period must have a category or a procedure code.");
				return;
			}
			if(checkPat.Checked){
				BenefitCur.PatPlanNum=_patPlanNum;
				BenefitCur.PlanNum=0;
			}
			else{
				BenefitCur.PatPlanNum=0;
				BenefitCur.PlanNum=_planNum;
			}
			if(listCategory.SelectedIndex==0) {
				BenefitCur.CovCatNum=0;
			}
			else {
				BenefitCur.CovCatNum=_listCovCats[listCategory.SelectedIndex-1].CovCatNum;
			}
			BenefitCur.CodeNum=ProcedureCodes.GetCodeNum(textProcCode.Text);
			BenefitCur.BenefitType=(InsBenefitType)listBenefitType.SelectedIndex;
			if(textPercent.Text=="") {
				BenefitCur.Percent=-1;
			}
			else {
				BenefitCur.Percent=PIn.Int(textPercent.Text);
			}
			if(textAmount.Text=="") {
				BenefitCur.MonetaryAmt=-1;
			}
			else {
				BenefitCur.MonetaryAmt=PIn.Double(textAmount.Text);
			}
			/*if(isAnnualMax){
				if(listTimePeriod.SelectedIndex==0){
					BenCur.TimePeriod=BenefitTimePeriod.ServiceYear;
				}
				if(listTimePeriod.SelectedIndex==1){
					BenCur.TimePeriod=BenefitTimePeriod.CalendarYear;
				}
			}
			else{*/
			BenefitCur.TimePeriod=(BenefitTimePeriod)listTimePeriod.SelectedIndex;
			//}
			BenefitCur.Quantity=PIn.Byte(textQuantity.Text);
			BenefitCur.QuantityQualifier=(BenefitQuantity)listQuantityQualifier.SelectedIndex;
			BenefitCur.CoverageLevel=(BenefitCoverageLevel)listCoverageLevel.SelectedIndex;
			//if(IsNew){
			//	BenCur.Insert();
			//}
			//else{
			//	BenCur.Update();
			//}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			BenefitCur=null;
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(IsNew){
				BenefitCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}
	}
}
