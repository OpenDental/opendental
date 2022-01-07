using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using SparksToothChart;

namespace OpenDental {
	public partial class FormSheetFieldSpecial:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		//private List<SheetFieldDef> AvailFields;
		///<summary>Ignored. Not available for mobile</summary>
		public bool IsEditMobile;
		public bool IsReadOnly;
		private List<SheetFieldDef> _listFieldDefsAvailable;
		///<summary>Only SheetFieldDefs that are associated to this SheetFieldLayoutMode will be shown in the listBoxAvailable.</summary>
		public SheetFieldLayoutMode LayoutMode=SheetFieldLayoutMode.Default;

		public FormSheetFieldSpecial() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldDefEdit_Load(object sender,EventArgs e) {
			base.SetFilterControlsAndAction(()=>SheetFilterChanged(),0,textWidth);
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			_listFieldDefsAvailable=SheetFieldsAvailable.GetSpecial(SheetDefCur.SheetType,LayoutMode);
			listBoxAvailable.Items.AddList(_listFieldDefsAvailable,x => x.FieldName);
			if(SheetFieldDefCur.IsNew) {
				listBoxAvailable.SetSelected(0);
				SheetFieldDefCur=_listFieldDefsAvailable[0];
			}
			else {
				listBoxAvailable.SetSelected(_listFieldDefsAvailable.FindIndex(x => x.FieldName==SheetFieldDefCur.FieldName),true);
				listBoxAvailable.Enabled=false;
			}
			if(ListTools.In(SheetFieldDefCur.FieldName,"SetPriorityListBox","PanelEcw")) {//Dynamic special controls which have growth/fill logic.
				comboGrowthBehavior.Visible=true;
				labelGrowth.Visible=true;
			}
			FillFields();
		}

		private void SheetFilterChanged() {
			if(EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic && SheetFieldDefCur.FieldName=="toothChart") {
				int height=(int)Math.Round(PIn.Double(textWidth.Text)*ToothChartData.SizeOriginalDrawing.Height)/ToothChartData.SizeOriginalDrawing.Width;
				textHeight.Text=POut.Int(height);
			}
		}

		///<summary>Each special field type is a little bit different, this allows each field to fill the form in its own way.</summary>
		private void FillFields() {
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			labelSpecialInfo.Text="";
			//textXPos.Enabled=true;
			//textYPos.Enabled=true;
			textHeight.Enabled=true;
			textWidth.Enabled=true;
			textHeight.ReadOnly=false;
			bool isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic;
			SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior,SheetFieldDefCur.GrowthBehavior,isDynamicSheetType);
			textWidth.MinVal=1;//Default for control in this window.
			textHeight.MinVal=1;//Default for control in this window.
			switch(((SheetFieldDef)listBoxAvailable.SelectedItem).FieldName){
				case "toothChart":
					labelSpecialInfo.Text=Lan.g(this,"The tooth chart will display a graphical toothchart based on which patient and treatment plan is selected. "+
					                                 "Fixed aspect ratio of 410/307");
					if(EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic) {
						//Allow user to edit toothChart width.
						textHeight.ReadOnly=true;
						textWidth.MinVal=410;
						textHeight.MinVal=307;
					}
					break;
				case "toothChartLegend":
					labelSpecialInfo.Text=Lan.g(this,"The tooth chart legend shows what the colors on the tooth chart mean.");
					textWidth.Text=POut.Int(DashToothChartLegend.DefaultWidth);
					textHeight.Text=POut.Int(DashToothChartLegend.DefaultHeight);
					if(!SheetDefs.IsDashboardType(SheetDefCur)) {
						textWidth.Enabled=false;
						textHeight.Enabled=false;
					}
					break;
				case "toothGrid"://not used
				default:
					break;
			}
		}

		private void listBoxAvailable_SelectedIndexChanged(object sender,EventArgs e) {
			if(!SheetFieldDefCur.IsNew) {
				//We don't allow this to be changed once the field is no longer "new"
				//If this method was like the ChangeCommited() this would not be needed but
				//the setting of the index on a existing sheetfield comes into this method.
				return;
			}
			if(listBoxAvailable.SelectedIndex==-1) {
				return;
			}
			SheetFieldDefCur=_listFieldDefsAvailable[listBoxAvailable.SelectedIndex];
			FillFields();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SheetFieldDefCur.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				SheetFieldDefCur=null; //null this out so the calling form knows to delete this sheetfield
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void SaveAndClose(){
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			SheetFieldDefCur.GrowthBehavior=comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
			//don't save to database here.
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		
	}
}