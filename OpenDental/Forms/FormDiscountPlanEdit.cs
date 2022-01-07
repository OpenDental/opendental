using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;
using System.Drawing;
using System.Text;

namespace OpenDental {
	public partial class FormDiscountPlanEdit:FormODBase {
		public DiscountPlan DiscountPlanCur;
		private DiscountPlan _discountPlanOld;
		///<summary>FeeSched for the current DiscountPlan.  May be null if the DiscountPlan is new.</summary>
		private FeeSched _feeSchedCur;
		private List<Def> _listDefsAdjTypes;
		private List<string> _listPatNames;
		private int _countPats;

		public FormDiscountPlanEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDiscountPlanEdit_Load(object sender,EventArgs e) {
			_discountPlanOld=DiscountPlanCur.Copy();
			textDescript.Text=DiscountPlanCur.Description;
			textPlanNum.Text=DiscountPlanCur.DiscountPlanNum.ToString();
			textAnnualMax.Text="";
			if(DiscountPlanCur.AnnualMax > -1 && !DiscountPlanCur.IsNew) {
				textAnnualMax.Text=DiscountPlanCur.AnnualMax.ToString();
			}
			textDiscountExamFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.ExamFreqLimit);
			textDiscountXrayFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.XrayFreqLimit);
			textDiscountProphyFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.ProphyFreqLimit);
			textDiscountFluorideFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.FluorideFreqLimit);
			textDiscountPerioFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.PerioFreqLimit);
			textDiscountLimitedFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.LimitedExamFreqLimit);
			textDiscountPAFreq.Text=FormatUnlimitedValueToString(DiscountPlanCur.PAFreqLimit);
			_feeSchedCur=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==DiscountPlanCur.FeeSchedNum,true);
			textFeeSched.Text=_feeSchedCur!=null ? _feeSchedCur.Description : "";
			_listDefsAdjTypes=Defs.GetDiscountPlanAdjTypes().ToList();
			for(int i=0;i<_listDefsAdjTypes.Count;i++) {
				comboBoxAdjType.Items.Add(_listDefsAdjTypes[i].ItemName);
				if(_listDefsAdjTypes[i].DefNum==DiscountPlanCur.DefNum) {
					comboBoxAdjType.SelectedIndex=i;
				}
			}
			//populate patient information
			_countPats=DiscountPlans.GetPatCountForPlan(DiscountPlanCur.DiscountPlanNum);
			textNumPatients.Text=_countPats.ToString();
			if(_countPats>10000) {//10,000 per Nathan. copied from FormInsPlan.cs
				comboPatient.Visible=false;
				butListPatients.Visible=true;
				butListPatients.Location=comboPatient.Location;
			}
			else {
				_listPatNames=DiscountPlans.GetPatNamesForPlan(DiscountPlanCur.DiscountPlanNum)
					.Distinct()
					.OrderBy(x => x)
					.ToList();
				comboPatient.Visible=true;
				butListPatients.Visible=false;
				comboPatient.Items.Clear();
				comboPatient.Items.AddRange(_listPatNames.ToArray());
				if(_listPatNames.Count>0) {
					comboPatient.SelectedIndex=0;
				}
			}
			checkHidden.Checked=DiscountPlanCur.IsHidden;
			if(!Security.IsAuthorized(Permissions.InsPlanEdit,true)) {//User may be able to get here if FormDiscountPlans is not in selection mode.
				textDescript.ReadOnly=true;
				comboBoxAdjType.Enabled=false;
				butFeeSched.Enabled=false;
				butOK.Enabled=false;
				checkHidden.Enabled=false;
			}
			textPlanNote.Text=DiscountPlanCur.PlanNote;
		}

		private void butFeeSched_Click(object sender,EventArgs e) {
			//No need to check security because we are launching the form in selection mode.
			using FormFeeScheds formFeeScheds=new FormFeeScheds(true);
			formFeeScheds.SelectedFeeSchedNum=(_feeSchedCur==null ? 0 : _feeSchedCur.FeeSchedNum);
			if(formFeeScheds.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_feeSchedCur=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==formFeeScheds.SelectedFeeSchedNum,true);//Hidden FeeSched are invalid selections.
			textFeeSched.Text=(_feeSchedCur?.Description??"");//Null check on OK click will force the user to make a FeeSched selection if null.
		}

		private void checkHidden_Click(object sender,EventArgs e) {
			if(!checkHidden.Checked || _countPats==0) {
				return;
			}
			string msgText=Lan.g(this,"Specified Discount Plan will be hidden.  It will no longer be available for assigning, but existing patients on plan will remain.");
			if(MessageBox.Show(this,msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
				checkHidden.Checked=false;
			}
		}

		private void butListPatients_Click(object sender,EventArgs e) {
			if(_listPatNames==null) {
				_listPatNames=DiscountPlans.GetPatNamesForPlan(DiscountPlanCur.DiscountPlanNum)
					.Distinct()
					.OrderBy(x => x)
					.ToList();
			}
			using FormODBase formPatientList=new FormODBase() {
				Size=new Size(500,400),
				Text="Other Patients List",
				FormBorderStyle=FormBorderStyle.FixedSingle
			};
			formPatientList.LayoutManager=new LayoutManagerForms(formPatientList,true,false);
			GridOD grid=new GridOD() {
				Size=new Size(475,300),
				Location=new Point(5,5),
				Title="Patients",
				TranslationName=""
			};
			UI.Button butClose=new UI.Button() {
				Size=new Size(75,23),
				Text="Close",
				Location=new Point(formPatientList.ClientSize.Width-80,formPatientList.ClientSize.Height-28),//Subtract the button's size plus 5 pixel buffer.
			};
			butClose.Click+=(s,ex) => formPatientList.Close();//When butClose is pressed, simply close the form.  If more functionality is needed, make a method below.
			formPatientList.LayoutManager.Add(grid,formPatientList);
			formPatientList.LayoutManager.Add(butClose,formPatientList);
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Name"),100){ IsWidthDynamic=true };
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			for(int i=0;i<_listPatNames.Count;i++) {
				GridRow row=new GridRow(_listPatNames[i]);
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			formPatientList.ShowDialog();
		}

		private int FormatUnlimitedValueToInt(string textBoxValue) {
			if(string.IsNullOrWhiteSpace(textBoxValue)) {
				return -1;
			}
			return PIn.Int(textBoxValue);
		}

		private string FormatUnlimitedValueToString(int value) {
			if(value<0) {
				return "";
			}
			return value.ToString();
		}

		///<summary>Returns the securitylog message for the discount plan.</summary>
		private string GetSecurityLogMessage() {
			StringBuilder stringBuilder=new StringBuilder();
			string feeSchedDescriptionCur=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==DiscountPlanCur.FeeSchedNum,true)?.Description??"";
			string adjustmentTypeCur=_listDefsAdjTypes.FirstOrDefault(x => x.DefNum==DiscountPlanCur.DefNum)?.ItemName??"";
			string feeSchedDescriptionOld=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==_discountPlanOld.FeeSchedNum,true)?.Description??"";
			string adjustmentTypeOld=_listDefsAdjTypes.FirstOrDefault(x => x.DefNum==_discountPlanOld.DefNum)?.ItemName??"";
			bool isNew=DiscountPlanCur.IsNew;
			if(isNew) {
				stringBuilder.AppendLine("New discount plan added:");
			}
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.Description,_discountPlanOld.Description,"description",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(feeSchedDescriptionCur,feeSchedDescriptionOld,"fee schedule",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(adjustmentTypeCur,adjustmentTypeOld,"adjustment type",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.IsHidden.ToString(),_discountPlanOld.IsHidden.ToString(),"is hidden",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.PlanNote,_discountPlanOld.PlanNote,"plan note",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.ExamFreqLimit.ToString(),
				_discountPlanOld.ExamFreqLimit.ToString(),"exam frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.XrayFreqLimit.ToString(),
				_discountPlanOld.XrayFreqLimit.ToString(),"xray frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.ProphyFreqLimit.ToString(),
				_discountPlanOld.ProphyFreqLimit.ToString(),"prophy frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.FluorideFreqLimit.ToString(),
				_discountPlanOld.FluorideFreqLimit.ToString(),"fluoride frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.PerioFreqLimit.ToString(),
				_discountPlanOld.PerioFreqLimit.ToString(),"perio frequency limit,isNew",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.LimitedExamFreqLimit.ToString(),
				_discountPlanOld.LimitedExamFreqLimit.ToString(),"limited exam frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.PAFreqLimit.ToString(),
				_discountPlanOld.PAFreqLimit.ToString(),"PA exam frequency limit",isNew));
			stringBuilder.Append(SecurityLogMessageHelper(DiscountPlanCur.AnnualMax.ToString(),_discountPlanOld.AnnualMax.ToString(),"annual max",isNew));
			return stringBuilder.ToString();
		}

		private string SecurityLogMessageHelper(string newVal,string oldVal,string columnVal,bool isNew) {
			if(isNew) {
				return $"{columnVal} initialized with value '{newVal}'\r\n";
			}
			else if(oldVal!=newVal) {
				return $"Discount plan {columnVal} changed from '{oldVal}' to '{newVal}'\r\n";
			}
			return "";
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescript.Text.Trim()=="") {
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			if(_feeSchedCur==null) {
				MsgBox.Show(this,"Please select a fee schedule.");
				return;
			}
			if(comboBoxAdjType.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an adjustment type.\r\nYou may need to create discount plan adjustment types within definition setup.");
				return;
			}
			if(!textAnnualMax.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//Validate that the frequency limitations are integers and greater than or equal to -2. Frequencies that do not have a limitation display as blank and are stored as -1.
			if(!textDiscountExamFreq.IsValid()
				|| !textDiscountProphyFreq.IsValid()
				|| !textDiscountFluorideFreq.IsValid()
				|| !textDiscountPerioFreq.IsValid()
				|| !textDiscountLimitedFreq.IsValid()
				|| !textDiscountXrayFreq.IsValid()
				|| !textDiscountPAFreq.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DiscountPlanCur.AnnualMax=PIn.Double(textAnnualMax.Text);
			if(string.IsNullOrWhiteSpace(textAnnualMax.Text)) {
				DiscountPlanCur.AnnualMax=-1;
			}
			DiscountPlanCur.ExamFreqLimit=FormatUnlimitedValueToInt(textDiscountExamFreq.Text);
			DiscountPlanCur.XrayFreqLimit=FormatUnlimitedValueToInt(textDiscountXrayFreq.Text);
			DiscountPlanCur.ProphyFreqLimit=FormatUnlimitedValueToInt(textDiscountProphyFreq.Text);
			DiscountPlanCur.FluorideFreqLimit=FormatUnlimitedValueToInt(textDiscountFluorideFreq.Text);
			DiscountPlanCur.PerioFreqLimit=FormatUnlimitedValueToInt(textDiscountPerioFreq.Text);
			DiscountPlanCur.LimitedExamFreqLimit=FormatUnlimitedValueToInt(textDiscountLimitedFreq.Text);
			DiscountPlanCur.PAFreqLimit=FormatUnlimitedValueToInt(textDiscountPAFreq.Text);
			DiscountPlanCur.Description=textDescript.Text;
			DiscountPlanCur.FeeSchedNum=_feeSchedCur.FeeSchedNum;
			DiscountPlanCur.DefNum=_listDefsAdjTypes[comboBoxAdjType.SelectedIndex].DefNum;
			DiscountPlanCur.IsHidden=checkHidden.Checked;
			DiscountPlanCur.PlanNote=textPlanNote.Text;
			Permissions permissionsType;
			if(DiscountPlanCur.IsNew) {
				DiscountPlans.Insert(DiscountPlanCur);
				permissionsType=Permissions.DiscountPlanAdd;
			}
			else {
				DiscountPlans.Update(DiscountPlanCur);
				permissionsType=Permissions.DiscountPlanEdit;
			}
			string logMessage=GetSecurityLogMessage();
			if(!string.IsNullOrEmpty(logMessage)) {
				SecurityLogs.MakeLogEntry(permissionsType,0,GetSecurityLogMessage());
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}