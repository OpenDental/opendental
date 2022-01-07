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
		private bool _isDynamicSheetType;
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
			textGridType.Text=SheetFieldDefCur.FieldName;
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			_isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic;			
			if(_isDynamicSheetType || SheetDefs.IsDashboardType(SheetDefCur)) {
				//Allow user to set dimensions of grids in dynamic layout defs.
				//These values define the min width and height.
				textHeight.Enabled=true;
				textWidth.Enabled=true;
				if(_isDynamicSheetType) {
					comboGrowthBehavior.Enabled=true;
				}
			}
			else {
				List<DisplayField> Columns=SheetUtil.GetGridColumnsAvailable(SheetFieldDefCur.FieldName);
				SheetFieldDefCur.Width=0;
				foreach(DisplayField f in Columns) {
					SheetFieldDefCur.Width+=f.ColumnWidth;
				}
			}
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			GridOD grid=new GridOD();
			grid.TranslationName="";
			using(Graphics g=Graphics.FromImage(new Bitmap(100,100))) {
				if(SheetFieldDefCur.FieldName=="EraClaimsPaid" || SheetDefs.IsDashboardType(SheetDefCur) || _isDynamicSheetType) {
					//Do not modify grid heights for Eras, Appt grid and dynamic layouts as the heights are calculated elsewhere.
				}
				else {
					//Why do we change the grid title height here?  The heights are also set elsewhere...
					SheetFieldDefCur.Height=0;
					//These grids display a title.
					if(new[] {"StatementPayPlan","StatementDynamicPayPlan","StatementInvoicePayment","TreatPlanBenefitsFamily","TreatPlanBenefitsIndividual"}.Contains(SheetFieldDefCur.FieldName)) {
						SheetFieldDefCur.Height+=18;//grid.TitleHeight;
					}
					SheetFieldDefCur.Height+=15//grid.HeaderHeight
						+(int)g.MeasureString("Any",grid.Font,100,StringFormat.GenericTypographic).Height+3;
				}
				textHeight.Text=SheetFieldDefCur.Height.ToString();
			}
			SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior,SheetFieldDefCur.GrowthBehavior,_isDynamicSheetType,true);
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
			if(_isDynamicSheetType || SheetDefs.IsDashboardType(SheetDefCur)) {
				SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
				SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
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