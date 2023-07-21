using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using PdfSharp.Drawing;

namespace OpenDental {
	public partial class FormSheetFieldOutput:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		private List<SheetFieldDef> _listSheetFieldDefsAvail;
		public bool IsEditMobile;
		public bool IsReadOnly;
		public bool IsNew;

		public FormSheetFieldOutput() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldDefEdit_Load(object sender,EventArgs e) {
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			if(IsEditMobile) {
				comboGrowthBehavior.Enabled=false;
				groupBox1.Enabled=false;
			}
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			textUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			//not allowed to change sheettype or fieldtype once created.  So get all avail fields for this sheettype
			_listSheetFieldDefsAvail=SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.Out);
			listFields.Items.Clear();
			for(int i=0;i<_listSheetFieldDefsAvail.Count;i++){
				//static text is not one of the options.
				listFields.Items.Add(_listSheetFieldDefsAvail[i].FieldName);
				if(SheetFieldDefCur.FieldName==_listSheetFieldDefsAvail[i].FieldName){
					listFields.SelectedIndex=i;
				}
			}
			InstalledFontCollection installedFontCollection=new InstalledFontCollection();
			for(int i=0;i<installedFontCollection.Families.Length;i++){
				comboFontName.Items.Add(installedFontCollection.Families[i].Name);
				if(!SheetFieldDefCur.FontName.IsNullOrEmpty() && SheetFieldDefCur.FontName==installedFontCollection.Families[i].Name) {
					comboFontName.SetSelected(i);
				}
			}
			if(comboFontName.SelectedIndex==-1) { //The sheetfield's current font is not in the list of installed fonts on this machine
				//Add the font to the combobox and mark it as missing. That way office can decided to either keep or change the missing font used for this field
				comboFontName.Items.Add(SheetFieldDefCur.FontName+" (missing)",SheetFieldDefCur.FontName);
				comboFontName.SetSelected(comboFontName.Items.Count-1);
			}
			textFontSize.Text=SheetFieldDefCur.FontSize.ToString();
			checkFontIsBold.Checked=SheetFieldDefCur.FontIsBold;
			SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior,SheetFieldDefCur.GrowthBehavior);
			for(int i=0;i<Enum.GetNames(typeof(System.Windows.Forms.HorizontalAlignment)).Length;i++) {
				comboTextAlign.Items.Add(Enum.GetNames(typeof(System.Windows.Forms.HorizontalAlignment))[i]);
				if((int)SheetFieldDefCur.TextAlign==i) {
					comboTextAlign.SelectedIndex=i;
				}
			}
			checkIsLocked.Checked=IsNew ? true : SheetFieldDefCur.IsLocked;
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			butColor.BackColor=SheetFieldDefCur.ItemColor;
			if(!string.IsNullOrEmpty(SheetFieldDefCur.UiLabelMobile)) { //Already has a value that user has setup previously.
				textUiLabelMobile.Text=SheetFieldDefCur.UiLabelMobile;
			}
		}

		private void listFields_SelectedIndexChanged(object sender,EventArgs e) {
			string fieldName=_listSheetFieldDefsAvail[listFields.SelectedIndex].FieldName;
			if(fieldName=="dateTime.Today") {
				textUiLabelMobile.Text="Date";
			}
			else if(fieldName=="patient.nameFL") {
				textUiLabelMobile.Text="Full Name";
			}
		}

		private void listFields_DoubleClick(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog1=new ColorDialog();
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
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
				|| !textHeight.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listFields.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a field name first.");
				return;
			}
			if(comboFontName.GetSelected<string>()=="" || comboFontName.GetSelected<string>()==null){
				//not going to bother testing for validity unless it will cause a crash.
				MsgBox.Show(this,"Please select a font name first.");
				return;
			}
			float fontSize;
			try{
				fontSize=float.Parse(textFontSize.Text);
			}
			catch{
				MsgBox.Show(this,"Font size is invalid.");
				return;
			}
			if(fontSize<2){
				MsgBox.Show(this,"Font size must be greater than 2.");
				return;
			}
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(comboFontName.GetSelected<string>(),fontSize,XFontStyle.Regular);
			}
			catch {
				MsgBox.Show(Lan.g(this,$"Unsupported font: {comboFontName.GetSelected<string>()}. Please choose another font."));
				return;
			}
			SheetFieldDefCur.FieldName=_listSheetFieldDefsAvail[listFields.SelectedIndex].FieldName;
			SheetFieldDefCur.FontName=comboFontName.GetSelected<string>();
			SheetFieldDefCur.FontSize=fontSize;
			SheetFieldDefCur.FontIsBold=checkFontIsBold.Checked;
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			SheetFieldDefCur.GrowthBehavior=comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
			SheetFieldDefCur.TextAlign=(System.Windows.Forms.HorizontalAlignment)comboTextAlign.SelectedIndex;
			SheetFieldDefCur.ItemColor=butColor.BackColor;
			SheetFieldDefCur.IsLocked=checkIsLocked.Checked;
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