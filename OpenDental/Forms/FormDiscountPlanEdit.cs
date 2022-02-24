using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;
using System.Drawing;

namespace OpenDental {
	public partial class FormDiscountPlanEdit:FormODBase {
		public DiscountPlan DiscountPlanCur;
		///<summary>FeeSched for the current DiscountPlan.  May be null if the DiscountPlan is new.</summary>
		private FeeSched _feeSchedCur;
		private List<Def> _listAdjTypeDefs;
		private List<string> _listPatNames;
		private int _countPats;

		public FormDiscountPlanEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDiscountPlanEdit_Load(object sender,EventArgs e) {
			textDescript.Text=DiscountPlanCur.Description;
			textPlanNum.Text=DiscountPlanCur.DiscountPlanNum.ToString();
			if(DiscountPlanCur.AnnualMax > -1 && !DiscountPlanCur.IsNew) {
				textAnnualMax.Text=DiscountPlanCur.AnnualMax.ToString();
			}
			else {
				textAnnualMax.Text="";
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
			_listAdjTypeDefs=Defs.GetDiscountPlanAdjTypes().ToList();
			for(int i=0;i<_listAdjTypeDefs.Count;i++) {
				comboBoxAdjType.Items.Add(_listAdjTypeDefs[i].ItemName);
				if(_listAdjTypeDefs[i].DefNum==DiscountPlanCur.DefNum) {
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
			using FormFeeScheds FormFS=new FormFeeScheds(true);
			FormFS.SelectedFeeSchedNum=(_feeSchedCur==null ? 0 : _feeSchedCur.FeeSchedNum);
			if(FormFS.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_feeSchedCur=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==FormFS.SelectedFeeSchedNum,true);//Hidden FeeSched are invalid selections.
			textFeeSched.Text=(_feeSchedCur?.Description??"");//Null check on OK click will force the user to make a FeeSched selection if null.
		}

		private void checkHidden_Click(object sender,EventArgs e) {
			if(checkHidden.Checked) {
				if(_countPats!=0) {
					string msgText=Lan.g(this,"Specified Discount Plan will be hidden.  "+
						"It will no longer be available for assigning, but existing patients on plan will remain");
					if(MessageBox.Show(this,msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						checkHidden.Checked=false;
					}
				}
			}
		}

		private void butListPatients_Click(object sender,EventArgs e) {
			if(_listPatNames==null) {
				_listPatNames=DiscountPlans.GetPatNamesForPlan(DiscountPlanCur.DiscountPlanNum)
					.Distinct()
					.OrderBy(x => x)
					.ToList();
			}
			using FormODBase form=new FormODBase() {
				Size=new Size(500,400),
				Text="Other Patients List",
				FormBorderStyle=FormBorderStyle.FixedSingle
			};
			form.LayoutManager=new LayoutManagerForms(form,true,false);
			GridOD grid=new GridOD() {
				Size=new Size(475,300),
				Location=new Point(5,5),
				Title="Patients",
				TranslationName=""
			};
			UI.Button butClose=new UI.Button() {
				Size=new Size(75,23),
				Text="Close",
				Location=new Point(form.ClientSize.Width-80,form.ClientSize.Height-28),//Subtract the button's size plus 5 pixel buffer.
			};
			butClose.Click+=(s,ex) => form.Close();//When butClose is pressed, simply close the form.  If more functionality is needed, make a method below.
			form.LayoutManager.Add(grid,form);
			form.LayoutManager.Add(butClose,form);
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),100){ IsWidthDynamic=true });
			grid.ListGridRows.Clear();
			foreach(string patName in _listPatNames) {
				grid.ListGridRows.Add(new GridRow(patName));
			}
			grid.EndUpdate();
			form.ShowDialog();
		}

		private bool IsValidFrequencyLimitation(string textBoxString) {
			int value;
			if(textBoxString.IsNullOrEmpty()) {
				return true;
			}
			try {
				value=Int32.Parse(textBoxString);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
			if(value < -1) {
				return false;
			}
			return true;
		}

		private int FormatUnlimitedValueToInt(string textBoxValue) {
			if(string.IsNullOrWhiteSpace(textBoxValue)) {
				return -1;
			}
			else {
				return PIn.Int(textBoxValue);
			}
		}

		private string FormatUnlimitedValueToString(int value) {
			if(value<0) {
				return "";
			}
			else {
				return value.ToString();
			}
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
			//Validate that the frequency limitations are integers and greater than or equal to -2.
			if(!IsValidFrequencyLimitation(textDiscountExamFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountProphyFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountFluorideFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountPerioFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountLimitedFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountXrayFreq.Text)
				||!IsValidFrequencyLimitation(textDiscountPAFreq.Text))
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textAnnualMax.Text)) {
				DiscountPlanCur.AnnualMax=-1;
			}
			else {
				DiscountPlanCur.AnnualMax=PIn.Double(textAnnualMax.Text);
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
			DiscountPlanCur.DefNum=_listAdjTypeDefs[comboBoxAdjType.SelectedIndex].DefNum;
			DiscountPlanCur.IsHidden=checkHidden.Checked;
			DiscountPlanCur.PlanNote=textPlanNote.Text;
			if(DiscountPlanCur.IsNew) {
				DiscountPlans.Insert(DiscountPlanCur);
			}
			else {
				DiscountPlans.Update(DiscountPlanCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}