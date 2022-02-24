using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using PdfSharp.Drawing;

namespace OpenDental {
	public partial class FormSheetFieldInput:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		private List<SheetFieldDef> AvailFields;
		public bool IsEditMobile;
		public bool IsReadOnly;

		public FormSheetFieldInput() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldInput_Load(object sender,EventArgs e) {
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			labelReportableName.Visible=false;
			textReportableName.Visible=false;
			if(SheetFieldDefCur.FieldName.StartsWith("misc")) {
				labelReportableName.Visible=true;
				textReportableName.Visible=true;
				textReportableName.Text=SheetFieldDefCur.ReportableName;
			}
			if(IsEditMobile) {
				groupBox1.Enabled=false;
				comboGrowthBehavior.Enabled=false;
				textTabOrder.Enabled=false;
			}
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			textUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			//not allowed to change sheettype or fieldtype once created.  So get all avail fields for this sheettype
			AvailFields=SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.In);
			listFields.Items.Clear();
			for(int i=0;i<AvailFields.Count;i++){
				//static text is not one of the options.
				listFields.Items.Add(AvailFields[i].FieldName);
				if(SheetFieldDefCur.FieldName.StartsWith(AvailFields[i].FieldName)){
					listFields.SelectedIndex=i;
				}
			}
			InstalledFontCollection fColl=new InstalledFontCollection();
			for(int i=0;i<fColl.Families.Length;i++){
				comboFontName.Items.Add(fColl.Families[i].Name);
			}
			comboFontName.Text=SheetFieldDefCur.FontName;
			textFontSize.Text=SheetFieldDefCur.FontSize.ToString();
			checkFontIsBold.Checked=SheetFieldDefCur.FontIsBold;
			SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior,SheetFieldDefCur.GrowthBehavior);
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			checkRequired.Checked=SheetFieldDefCur.IsRequired;
			textTabOrder.Text=SheetFieldDefCur.TabOrder.ToString();
			if(!string.IsNullOrEmpty(SheetFieldDefCur.UiLabelMobile)) { //Already has a value that user has setup previously.
				textUiLabelMobile.Text=SheetFieldDefCur.UiLabelMobile;
			}
			labelReportableName.Visible=false;
			textReportableName.Visible=false;
			textReportableName.Text="";
		}

		private void listFields_DoubleClick(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void listFields_SelectionChangeCommitted(object sender,EventArgs e) {
			if(listFields.SelectedIndex==-1) {
				return;
			}
			string fieldName=AvailFields[listFields.SelectedIndex].FieldName;
			if(fieldName=="misc") {
				labelReportableName.Visible=true;
				textReportableName.Visible=true;
				textReportableName.Text=SheetFieldDefCur.ReportableName;//will either be "" or saved ReportableName.
				textUiLabelMobile.Text="Misc";
			}
			else {
				labelReportableName.Visible=false;
				textReportableName.Visible=false;
				textReportableName.Text="";
			}
			if(fieldName.StartsWith("inputMed")) {
				int inputMedNum=0;
				if(int.TryParse(fieldName.Replace("inputMed",""),out inputMedNum)) {
					textUiLabelMobile.Text="Input Medication "+inputMedNum.ToString();
				}
			}
			else if(fieldName=="Birthdate") {
				textUiLabelMobile.Text="Birthdate";
			}
			else if(fieldName=="FName") {
				textUiLabelMobile.Text="First Name";
			}
			else if(fieldName=="LName") {
				textUiLabelMobile.Text="Last Name";
			}
			else if(fieldName=="ICEName") {
				textUiLabelMobile.Text="Emergency Contact Name";
			}
			else if(fieldName=="ICEPhone") {
				textUiLabelMobile.Text="Emergency Phone";
			}
			else if(fieldName=="toothNum") {
				textUiLabelMobile.Text="Tooth Number(s)";
			}
			else if(fieldName=="MiddleI") {
				textUiLabelMobile.Text="Middle Initial";
			}
			else if(fieldName=="Preferred") {
				textUiLabelMobile.Text="Preferred Name";
			}
			else {
				textUiLabelMobile.Text=AvailFields[listFields.SelectedIndex].FieldName;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void SaveAndClose(){
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid()
				|| !textTabOrder.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listFields.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a field name first.");
				return;
			}
			if(comboFontName.Text==""){
				//not going to bother testing for validity unless it will cause a crash.
				MsgBox.Show(this,"Please select a font name first.");
				return;
			}
			if(SheetDefCur.SheetType==SheetTypeEnum.ExamSheet) {
				if(textReportableName.Text.Contains(";") || textReportableName.Text.Contains(":")) {
					MsgBox.Show(this,"Reportable name for Exam Sheet fields may not contain a ':' or a ';'.");
					return;
				}
			}
			float fontSize;
			try{
				fontSize=float.Parse(textFontSize.Text);
				if(fontSize<2){
					MsgBox.Show(this,"Font size is invalid.");
					return;
				}
			}
			catch{
				MsgBox.Show(this,"Font size is invalid.");
				return;
			}
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(comboFontName.Text,fontSize,XFontStyle.Regular);
			}
			catch {
				MsgBox.Show(Lan.g(this,$"Unsupported font: {comboFontName.Text}. Please choose another font."));
				return;
			}
			SheetFieldDefCur.FieldName=AvailFields[listFields.SelectedIndex].FieldName;
			SheetFieldDefCur.ReportableName=textReportableName.Text;//always safe even if not a misc field or if textReportableName is blank.
			SheetFieldDefCur.FontName=comboFontName.Text;
			SheetFieldDefCur.FontSize=fontSize;
			SheetFieldDefCur.FontIsBold=checkFontIsBold.Checked;
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			SheetFieldDefCur.GrowthBehavior=comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
			SheetFieldDefCur.IsRequired=checkRequired.Checked;
			SheetFieldDefCur.TabOrder=PIn.Int(textTabOrder.Text);
			SheetFieldDefCur.UiLabelMobile=textUiLabelMobile.Text;
			//don't save to database here.
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		
	}
}