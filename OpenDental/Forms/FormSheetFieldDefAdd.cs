using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSheetFieldDefAdd:FormODBase {
		///<summary>Upon closing this might be set to true.</summary>
		public bool DoRefreshDoubleBuffer;
		public string SelectedLanguageThreeLetters;
		public SheetDef SheetDefCur;
		///<summary>If DialogResult.OK, then this is the new sheetFieldDef that should be added to the SheetDef.</summary>
		public SheetFieldDef SheetFieldDefNew;
		public SheetFieldLayoutMode SheetFieldLayoutMode_;

		public FormSheetFieldDefAdd() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldAdd_Load(object sender,EventArgs e) {
			List<SheetFieldType> listSheetFieldTypes=SheetDefs.GetVisibleButtons(SheetDefCur.SheetType);
			butOutputText.Visible=listSheetFieldTypes.Contains(SheetFieldType.OutputText);
			butInputField.Visible=listSheetFieldTypes.Contains(SheetFieldType.InputField);
			butStaticText.Visible=listSheetFieldTypes.Contains(SheetFieldType.StaticText);
			butCheckBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.CheckBox);
			butComboBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.ComboBox);
			butImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.Image);
			butPatImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.PatImage);
			butLine.Visible=listSheetFieldTypes.Contains(SheetFieldType.Line);
			butRectangle.Visible=listSheetFieldTypes.Contains(SheetFieldType.Rectangle);
			butSigBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBox);
			butSigBoxPractice.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBoxPractice);
			butSpecial.Visible=listSheetFieldTypes.Contains(SheetFieldType.Special);
			butGrid.Visible=listSheetFieldTypes.Contains(SheetFieldType.Grid);
			butScreenChart.Visible=listSheetFieldTypes.Contains(SheetFieldType.ScreenChart);
		}

		private void butOutputText_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.Out).Count==0) {
				MsgBox.Show(this,"There are no output fields available for this type of sheet.");
				return;
			}
			Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldOutput formSheetFieldOutput=new FormSheetFieldOutput();
			formSheetFieldOutput.IsNew=true;
			formSheetFieldOutput.SheetDefCur=SheetDefCur;
			formSheetFieldOutput.SheetFieldDefCur=SheetFieldDef.NewOutput("",SheetDefCur.FontSize,SheetDefCur.FontName,false,0,0,100,font.Height);
			formSheetFieldOutput.ShowDialog();
			if(formSheetFieldOutput.DialogResult!=DialogResult.OK  || formSheetFieldOutput.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldOutput.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butInputField_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.In).Count==0) {
				MsgBox.Show(this,"There are no input fields available for this type of sheet.");
				return;
			}
			Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldInput formSheetFieldInput=new FormSheetFieldInput();
			formSheetFieldInput.SheetDefCur=SheetDefCur;
			formSheetFieldInput.SheetFieldDefCur=SheetFieldDef.NewInput("",SheetDefCur.FontSize,SheetDefCur.FontName,false,0,0,100,font.Height);
			formSheetFieldInput.ShowDialog();
			if(formSheetFieldInput.DialogResult!=DialogResult.OK  || formSheetFieldInput.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldInput.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butStaticText_Click(object sender,EventArgs e) {
			Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldStatic formSheetFieldStatic=new FormSheetFieldStatic();
			formSheetFieldStatic.SheetDefCur=SheetDefCur;
			formSheetFieldStatic.SheetFieldDefCur=SheetFieldDef.NewStaticText("",SheetDefCur.FontSize,SheetDefCur.FontName,false,0,0,100,font.Height);
			formSheetFieldStatic.ShowDialog();
			if(formSheetFieldStatic.DialogResult!=DialogResult.OK  || formSheetFieldStatic.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldStatic.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butCheckBox_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.Check).Count==0) {
				MsgBox.Show(this,"There are no checkbox fields available for this type of sheet.");
				return;
			}
			using FormSheetFieldCheckBox formSheetFieldCheckBox=new FormSheetFieldCheckBox();
			formSheetFieldCheckBox.SheetFieldDefCur=SheetFieldDef.NewCheckBox("",0,0,11,11);
			formSheetFieldCheckBox.SheetFieldDefCur.Language=SelectedLanguageThreeLetters;
			formSheetFieldCheckBox.SheetDefCur=SheetDefCur;
			formSheetFieldCheckBox.ShowDialog();
			if(formSheetFieldCheckBox.DialogResult!=DialogResult.OK  || formSheetFieldCheckBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldCheckBox.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butComboBox_Click(object sender,EventArgs e) {
			using FormSheetFieldComboBox formSheetFieldComboBox=new FormSheetFieldComboBox();
			formSheetFieldComboBox.SheetDefCur=SheetDefCur;
			formSheetFieldComboBox.SheetFieldDefCur=SheetFieldDef.NewComboBox("","",0,0);
			formSheetFieldComboBox.ShowDialog();
			if(formSheetFieldComboBox.DialogResult!=DialogResult.OK || formSheetFieldComboBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldComboBox.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butImage_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Not allowed because not using AtoZ folder");
				return;
			}
			//Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldImage formSheetFieldImage=new FormSheetFieldImage();
			formSheetFieldImage.SheetDefCur=SheetDefCur;
			formSheetFieldImage.SheetFieldDefCur=SheetFieldDef.NewImage("",0,0,100,100);
			formSheetFieldImage.SheetFieldDefCur.Language=SelectedLanguageThreeLetters;
			formSheetFieldImage.ShowDialog();
			if(formSheetFieldImage.DialogResult!=DialogResult.OK  || formSheetFieldImage.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldImage.SheetFieldDefCur;
			DoRefreshDoubleBuffer=true;
			DialogResult=DialogResult.OK;
		}

		private void butPatImage_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Not allowed because not using AtoZ folder");
				return;
			}
			//Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldPatImage formSheetFieldPatImage=new FormSheetFieldPatImage();
			formSheetFieldPatImage.SheetDefCur=SheetDefCur;
			formSheetFieldPatImage.SheetFieldDefCur=SheetFieldDef.NewPatImage(0,0,100,100);
			formSheetFieldPatImage.SheetFieldDefCur.FieldType=SheetFieldType.PatImage;
			formSheetFieldPatImage.ShowDialog();
			if(formSheetFieldPatImage.DialogResult!=DialogResult.OK  || formSheetFieldPatImage.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldPatImage.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butLine_Click(object sender,EventArgs e) {
			using FormSheetFieldLine formSheetFieldLine=new FormSheetFieldLine();
			formSheetFieldLine.SheetDefCur=SheetDefCur;
			formSheetFieldLine.SheetFieldDefCur=SheetFieldDef.NewLine(0,0,0,0);
			formSheetFieldLine.ShowDialog();
			if(formSheetFieldLine.DialogResult!=DialogResult.OK  || formSheetFieldLine.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldLine.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butRectangle_Click(object sender,EventArgs e) {
			using FormSheetFieldRect formSheetFieldRect=new FormSheetFieldRect();
			formSheetFieldRect.SheetDefCur=SheetDefCur;
			formSheetFieldRect.SheetFieldDefCur=SheetFieldDef.NewRect(0,0,0,0);
			formSheetFieldRect.ShowDialog();
			if(formSheetFieldRect.DialogResult!=DialogResult.OK  || formSheetFieldRect.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldRect.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butSigBox_Click(object sender,EventArgs e) {
			AddSignatureBox(SheetFieldType.SigBox);
		}

		private void butSigBoxPractice_Click(object sender,EventArgs e) {
			AddSignatureBox(SheetFieldType.SigBoxPractice);
		}

		private void AddSignatureBox(SheetFieldType sheetFieldType) {
			using FormSheetFieldSigBox formSheetFieldSigBox=new FormSheetFieldSigBox();
			formSheetFieldSigBox.SheetDefCur=SheetDefCur;
			formSheetFieldSigBox.SheetFieldDefCur=SheetFieldDef.NewSigBox(0,0,364,81,sigBox:sheetFieldType);
			formSheetFieldSigBox.ShowDialog();
			if(formSheetFieldSigBox.DialogResult!=DialogResult.OK  || formSheetFieldSigBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			SheetFieldDefNew=formSheetFieldSigBox.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butSpecial_Click(object sender,EventArgs e) {
			using FormSheetFieldSpecial formSheetFieldSpecial=new FormSheetFieldSpecial();
			formSheetFieldSpecial.SheetDefCur=SheetDefCur;
			formSheetFieldSpecial.SheetFieldDefCur=new SheetFieldDef(){IsNew=true };
			formSheetFieldSpecial.LayoutMode=SheetFieldLayoutMode_;
			formSheetFieldSpecial.ShowDialog();
			if(formSheetFieldSpecial.DialogResult!=DialogResult.OK) {
				return;
			}
			bool isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic;
			List<SheetFieldDef> listSheetFieldDefsPertinent=SheetDefCur.SheetFieldDefs.FindAll(x => x.LayoutMode==SheetFieldLayoutMode_ && x.Language==SelectedLanguageThreeLetters);
			if(isDynamicSheetType && listSheetFieldDefsPertinent.Any(x => x.FieldName==formSheetFieldSpecial.SheetFieldDefCur.FieldName)) {
				MsgBox.Show(this,"Field already exists.");
				return;
			}
			SheetFieldDefNew=formSheetFieldSpecial.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butGrid_Click(object sender,EventArgs e) {
			bool isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(SheetDefCur.SheetType).IsDynamic;
			using FormSheetFieldGrid formSheetFieldGrid=new FormSheetFieldGrid();
			formSheetFieldGrid.SheetDefCur=SheetDefCur;
			if(SheetDefs.IsDashboardType(SheetDefCur)) {
				//is resized from dialog window.
				formSheetFieldGrid.SheetFieldDefCur=SheetFieldDef.NewGrid(DashApptGrid.SheetFieldName,0,0,100,150,growthBehavior:GrowthBehaviorEnum.None); 
			}
			else {
				using FormSheetFieldGridType formSheetFieldGridType=new FormSheetFieldGridType();
				formSheetFieldGridType.SheetDefCur=SheetDefCur;
				formSheetFieldGridType.LayoutMode=SheetFieldLayoutMode_;
				formSheetFieldGridType.ShowDialog();
				if(formSheetFieldGridType.DialogResult!=DialogResult.OK) {
					return;
				}
				formSheetFieldGrid.SheetFieldDefCur=SheetFieldDef.NewGrid(formSheetFieldGridType.SelectedSheetGridType,0,0,100,100); //is resized from dialog window.
				if(isDynamicSheetType) {
					//Grid dimensions in dynamic sheetDefs should be static by default because easiest to understand.
					formSheetFieldGrid.SheetFieldDefCur.GrowthBehavior=GrowthBehaviorEnum.None;
				}
			}
			formSheetFieldGrid.ShowDialog();
			if(formSheetFieldGrid.DialogResult!=DialogResult.OK  || formSheetFieldGrid.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsPertinent=SheetDefCur.SheetFieldDefs.FindAll(x => x.LayoutMode==SheetFieldLayoutMode_ && x.Language==SelectedLanguageThreeLetters);
			if(isDynamicSheetType && listSheetFieldDefsPertinent.Any(x => x.FieldName==formSheetFieldGrid.SheetFieldDefCur.FieldName)) {
				MsgBox.Show(this,"Grid already exists.");
				return;
			}
			SheetFieldDefNew=formSheetFieldGrid.SheetFieldDefCur;
			DialogResult=DialogResult.OK;
		}

		private void butScreenChart_Click(object sender,EventArgs e) {
			string fieldValue="0;d,m,ling;d,m,ling;,,;,,;,,;,,;m,d,ling;m,d,ling;m,d,buc;m,d,buc;,,;,,;,,;,,;d,m,buc;d,m,buc";
			SheetFieldDef sheetFieldDef=null;
			if(!HasChartSealantComplete()) {
				sheetFieldDef=SheetFieldDef.NewScreenChart("ChartSealantComplete",fieldValue,0,0);
			}
			else if(!HasChartSealantTreatment()) {
				sheetFieldDef=SheetFieldDef.NewScreenChart("ChartSealantTreatment",fieldValue,0,0);
			}
			else {
				MsgBox.Show(this,"Only two charts are allowed per screening sheet.");
				return;
			}
			SheetFieldDefNew=sheetFieldDef;
			DialogResult=DialogResult.OK;
		}

		private bool HasScreeningChart(bool isTreatmentChart) {
			if(SheetDefCur.SheetType!=SheetTypeEnum.Screening) {
				return false;
			}
			List<SheetFieldDef> listSheetFieldDefsPertinent=SheetDefCur.SheetFieldDefs.FindAll( x => x.LayoutMode==SheetFieldLayoutMode_ && x.Language==SelectedLanguageThreeLetters);
			List<SheetFieldDef> listSheetFieldDefs=listSheetFieldDefsPertinent;
			if(listSheetFieldDefs.Count==0) {
				return false;
			}
			string chartName;
			if(isTreatmentChart) {
				chartName="ChartSealantTreatment";
			}
			else {
				chartName="ChartSealantComplete";
			}
			return listSheetFieldDefs.Any(x => x.FieldType==SheetFieldType.ScreenChart && x.FieldName==chartName);
		}

		private bool HasChartSealantComplete() {
			return HasScreeningChart(isTreatmentChart:false);
		}

		private bool HasChartSealantTreatment() {
			return HasScreeningChart(isTreatmentChart:true);
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}