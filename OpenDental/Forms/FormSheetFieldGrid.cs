using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSheetFieldGrid:FormODBase {
		private bool _isChartModuleSheetType;
		private bool _isTreatmentPlanGrid;
		public SheetDef SheetDefCur;
		public SheetFieldDef SheetFieldDefCur;
		///<summary>Ignored. Not available for mobile</summary>
		public bool IsEditMobile;
		public bool IsReadOnly;

		public FormSheetFieldGrid() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldGrid_Load(object sender,EventArgs e) {
			if(IsReadOnly) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			labelEnableGrowthBehavior.Visible=false;
			checkBoxGrowthBehavior.Visible=false;
			textGridType.Text=SheetFieldDefCur.FieldName;
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			_isChartModuleSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsChartModule;			
			if(_isChartModuleSheetType || SheetDefs.IsDashboardType(SheetDefCur)) {
				//Allow user to set dimensions of grids in dynamic layout defs.
				//These values define the min width and height.
				textHeight.Enabled=true;
				textWidth.Enabled=true;
				if(_isChartModuleSheetType) {
					comboGrowthBehavior.Enabled=true;
				}
			}
			else {
				List<DisplayField> listDisplayFieldsCols=SheetUtil.GetGridColumnsAvailable(SheetFieldDefCur.FieldName);
				SheetFieldDefCur.Width=0;
				for(int i=0;i<listDisplayFieldsCols.Count;i++){
					SheetFieldDefCur.Width+=listDisplayFieldsCols[i].ColumnWidth;
				}
			}
			//Only enable label/checkbox selection for TreatPlanBenefitsFamily and TreatPlanBenefitsIndividual grids.
			if(SheetFieldDefCur.FieldName=="TreatPlanBenefitsFamily" || SheetFieldDefCur.FieldName=="TreatPlanBenefitsIndividual") {
				_isTreatmentPlanGrid=true;
				labelEnableGrowthBehavior.Visible=true;
				checkBoxGrowthBehavior.Visible=true;
			}
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			if(SheetFieldDefCur.FieldName=="EraClaimsPaid" || SheetDefs.IsDashboardType(SheetDefCur) || _isChartModuleSheetType) {
				//Do not modify grid heights for Eras, Appt grid and dynamic layouts as the heights are calculated elsewhere.
			}
			else {
				SheetFieldDefCur.Height=0;
				//These grids display a title.
				if(SheetFieldDefCur.FieldName.In("StatementPayPlan","StatementDynamicPayPlan","StatementInvoicePayment","TreatPlanBenefitsFamily","TreatPlanBenefitsIndividual")) {
					SheetFieldDefCur.Height+=18;//grid.TitleHeight;
				}
				SheetFieldDefCur.Height+=15;//grid.HeaderHeight
				Font font=new Font(FontFamily.GenericSansSerif,8.5f);
				int heightFont=font.Height;//unscaled 96 dpi value is good
				SheetFieldDefCur.Height+=heightFont+2;
			}
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior,SheetFieldDefCur.GrowthBehavior,_isChartModuleSheetType,true);
		}

		///<summary>checkBoxGrowthBehavior is only enabled for TreatPlanBenefitsFamily and TreatPlanBenefitsIndividual grids.</summary>
		private void checkBoxGrowthBehavior_CheckedChanged(object sender,EventArgs e) {
			if(checkBoxGrowthBehavior.Checked) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Changing the growth behavior of a grid element might produce unexpected results. Do you want to proceed?")) {
					comboGrowthBehavior.Enabled=true;
				}
				else {
					checkBoxGrowthBehavior.Checked=false;
				}
				return;
			}
			comboGrowthBehavior.Enabled=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private bool IsValid(out string error) {
			error="";
			if(comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>()==GrowthBehaviorEnum.FillDownFitColumns 
				&& SheetFieldDefCur.FieldName!="ProgressNotes")
			{ 
				error="FillDownFitColumns can only be selected for the ProgressNotes grid.";
			}
			return error.IsNullOrEmpty();
		}

		private void butOK_Click(object sender,EventArgs e) {
			string error;
			if(!IsValid(out error)) {
				MsgBox.Show(error);
				return;
			}
			if(!textXPos.IsValid() 
				|| !textYPos.IsValid()
				|| !textHeight.IsValid()
				|| !textWidth.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//don't save to database here.
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			//Only enabled for grids related to a dynamic sheetType, and Dashboard Appointment Grid.
			if(_isChartModuleSheetType || SheetDefs.IsDashboardType(SheetDefCur)) {
				SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
				SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
				SheetFieldDefCur.GrowthBehavior=comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
			}
			//User has selected a growth behavior on a treatment plan grid.
			if(_isTreatmentPlanGrid) {
				SheetFieldDefCur.GrowthBehavior=comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
			}
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}