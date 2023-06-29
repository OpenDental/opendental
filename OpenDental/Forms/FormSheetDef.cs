using System;
using System.Drawing.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using PdfSharp.Drawing;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormSheetDef:FormODBase {
		///<summary>This holds the default values of the sheet dimensions, but once we click ok the users settings will override.</summary>
		public SheetDef SheetDefCur;
		//private List<SheetFieldDef> AvailFields;
		public bool IsReadOnly;
		///<summary>On creation of a new sheetdef, the user must pick a description and a sheettype before allowing to start editing the sheet.  After the initial sheettype selection, this will be false, indicating that the user may not change the type.</summary>
		public bool IsInitial;
		///<summary>The Autosave feature needs to be considered when there is at least one image category flagged to Autosave Forms.</summary>
		private bool _doConsiderAutoSave;
		///<summary>This is the FormSheetDefEdit that is passed into this window when it is opened in order for us to update the fontsize in the background for a preview.</summary>
		public FormSheetDefEdit _formSheetDefEdit;
		///<summary>This is a copy of the SheetDef in order to revert the font size back to what it was in case the customer doesn't press the OK button.</summary>
		private SheetDef _sheetDefCopy;

		public FormSheetDef() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetDef_Load(object sender,EventArgs e) {
			SetHeightWidthMin();
			if(IsReadOnly){
				butOK.Enabled=false;
				butReduceFontSize.Enabled=false;
			}
			if(!IsInitial){
				listSheetType.Enabled=false;
				checkHasMobileLayout.Enabled=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			}
			_doConsiderAutoSave=(Defs.GetImageCat(ImageCategorySpecial.U)>0);//Any are checked to auto save
			textDescription.Text=SheetDefCur.Description;
			Func<SheetTypeEnum,string> funcItemToString=new Func<SheetTypeEnum,string>((sheetType) => { return Lan.g("enumSheetTypeEnum",sheetType.GetDescription()); });
			//not allowed to change sheettype once created.
			List<SheetTypeEnum> listSheetTypeEnums =Enum.GetValues(typeof(SheetTypeEnum)).Cast<SheetTypeEnum>().ToList();
			listSheetTypeEnums=listSheetTypeEnums.FindAll(x => !x.In(SheetTypeEnum.None,SheetTypeEnum.MedLabResults) && !SheetDefs.IsDashboardType(x));
			listSheetTypeEnums=listSheetTypeEnums.OrderBy(x=>funcItemToString(x)).ToList(); //Order alphabetical.
			listSheetType.Items.AddList(listSheetTypeEnums, funcItemToString); //funcItemToString contains the text displayed for each item
			if(!IsInitial) {
				for(int i=0; i<listSheetType.Items.Count;i++) {
					if((SheetTypeEnum)listSheetType.Items.GetObjectAt(i)==SheetDefCur.SheetType) {
						listSheetType.SetSelected(i);
					}
				}
			}
			InstalledFontCollection installedFontCollection=new InstalledFontCollection();
			for(int i=0;i<installedFontCollection.Families.Length;i++){
				comboFontName.Items.Add(installedFontCollection.Families[i].Name);
				if(installedFontCollection.Families[i].Name.ToLower()==SheetDefCur.FontName.ToLower()) {
					comboFontName.SelectedIndex=i;
				}
			}
			if(comboFontName.SelectedIndex==-1) { //The sheetfield's current font is not in the list of installed fonts on this machine
				//Add the font to the combobox and mark it as missing. That way office can decided to either keep or change the missing font used for this field
				comboFontName.Items.Add(SheetDefCur.FontName+" (missing)",SheetDefCur.FontName);
				comboFontName.SetSelected(comboFontName.Items.Count-1);
			}
			checkBypassLockDate.Checked=(SheetDefCur.BypassGlobalLock==BypassLockStatus.BypassAlways);
			textFontSize.Text=SheetDefCur.FontSize.ToString();
			textWidth.Text=SheetDefCur.Width.ToString();
			textHeight.Text=SheetDefCur.Height.ToString();
			checkIsLandscape.Checked=SheetDefCur.IsLandscape;
			checkHasMobileLayout.Checked=SheetDefCur.HasMobileLayout;
			checkAutoSaveCheck.Checked=SheetDefCur.AutoCheckSaveImage;
			comboAutoSaveOverride.Items.AddDefNone();
			comboAutoSaveOverride.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ImageCats,true));
			comboAutoSaveOverride.SetSelectedDefNum(SheetDefCur.AutoCheckSaveImageDocCategory);
			if(SetAutoCheckEnabled(SheetDefCur.SheetType,isLoading:true)) {
				checkAutoSaveCheck.Enabled=true;
				comboAutoSaveOverride.Enabled=true;
				labelAutoSaveOverride.Enabled=true;
			}
			else {
				checkAutoSaveCheck.Enabled=false;
				comboAutoSaveOverride.Enabled=false;
				labelAutoSaveOverride.Enabled=false;
			}
			//Load is done. It is now safe to register for the selection change event.
			listSheetType.SelectedIndexChanged+=new EventHandler(listSheetType_SelectedIndexChanged);
			if(SheetDefs.IsDashboardType(SheetDefCur)) {
				labelSheetType.Visible=false;
				listSheetType.Visible=false;
				checkBypassLockDate.Visible=false;
				checkIsLandscape.Visible=false;
				checkHasMobileLayout.Visible=false;
			}
			_sheetDefCopy=SheetDefCur.Copy();
			if(SheetDefCur.SheetFieldDefs==null) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefs=new List<SheetFieldDef>();
			for(int i=0;i<SheetDefCur.SheetFieldDefs.Count;i++){
				SheetFieldDef sheetFieldDef=SheetDefCur.SheetFieldDefs[i].Copy();
				listSheetFieldDefs.Add(sheetFieldDef);
			}
			_sheetDefCopy.SheetFieldDefs=listSheetFieldDefs;
		}

		private bool SetAutoCheckEnabled(SheetTypeEnum sheetType,bool isLoading=false) {
			if(isLoading && _doConsiderAutoSave) {
				checkAutoSaveCheck.Visible=true;
				comboAutoSaveOverride.Visible=true;
				labelAutoSaveOverride.Visible=true;
			}
			return _doConsiderAutoSave && EnumTools.GetAttributeOrDefault<SheetTypeAttribute>(sheetType).CanAutoSave;
		}

		///<summary>Sets the minimum valid value (used for validation only) of the appropriate Height or Width field based on the bottom of the lowest field. Max values are set in the designer.</summary>
		private void SetHeightWidthMin() {
			textHeight.MinVal=10;//default values
			textWidth.MinVal=10;//default values
			if(SheetDefCur.SheetFieldDefs==null) {
				//New sheet
				return;
			}
			int minVal=int.MaxValue;
			for(int i=0;i<SheetDefCur.SheetFieldDefs.Count;i++) {
				minVal=Math.Min(minVal,SheetDefCur.SheetFieldDefs[i].Bounds.Bottom/SheetDefCur.PageCount);
			}
			if(minVal==int.MaxValue) {
				//Sheet has no sheet fields.
				return;
			}
			if(checkIsLandscape.Checked) {
				//Because Width is used to measure vertical sheet size.
				textWidth.MinVal=minVal;
			}
			else {
				//Because Height is used to measure vertical sheet size.
				textHeight.MinVal=minVal;
			}
		}

		private void checkIsLandscape_Click(object sender,EventArgs e) {
			SetHeightWidthMin();
		}

		private void listSheetType_SelectedIndexChanged(object sender,EventArgs e) {
			if(!IsInitial){
				return;
			}
			if(listSheetType.SelectedIndex==-1){
				return;
			}
			SheetDef sheetDef=null;
			checkHasMobileLayout.Enabled=false;
			SheetTypeEnum sheetTypeEnumSelected=listSheetType.GetSelected<SheetTypeEnum>();
			if(SheetDefs.IsMobileAllowed(sheetTypeEnumSelected)) {
				checkHasMobileLayout.Enabled=true;
			}
			if(SetAutoCheckEnabled(sheetTypeEnumSelected)) {
				checkAutoSaveCheck.Enabled=true;
				comboAutoSaveOverride.Enabled=true;
				labelAutoSaveOverride.Enabled=true;
			}
			else {
				//Put the following controls back to a default state because they will be disabled and possibly visible.
				checkAutoSaveCheck.Enabled=false;
				checkAutoSaveCheck.Checked=false;
				comboAutoSaveOverride.Enabled=false;
				labelAutoSaveOverride.Enabled=false;
				comboAutoSaveOverride.SetSelected(0);//None
			}
			switch(sheetTypeEnumSelected){
				case SheetTypeEnum.LabelCarrier:
				case SheetTypeEnum.LabelPatient:
				case SheetTypeEnum.LabelReferral:
					sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientMail);
					if(textDescription.Text==""){
						textDescription.Text=sheetTypeEnumSelected.GetDescription();
					}
					textFontSize.Text=sheetDef.FontSize.ToString();
					textWidth.Text=sheetDef.Width.ToString();
					textHeight.Text=sheetDef.Height.ToString();
					checkIsLandscape.Checked=sheetDef.IsLandscape;
					break;
				case SheetTypeEnum.ReferralSlip:
					sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.ReferralSlip);
					if(textDescription.Text==""){
						textDescription.Text=sheetTypeEnumSelected.GetDescription();
					}
					textFontSize.Text=sheetDef.FontSize.ToString();
					textWidth.Text=sheetDef.Width.ToString();
					textHeight.Text=sheetDef.Height.ToString();
					checkIsLandscape.Checked=sheetDef.IsLandscape;
					break;
				default://All other sheet types use default values
					ReloadDefaultValues();
					break;
			}
			if(!checkHasMobileLayout.Enabled) { //Only change the check state if the selected form does not allow mobile sheet layout
				checkHasMobileLayout.Checked=false;
			}
		}

		private void CheckHasMobileLayout_CheckedChanged(object sender,EventArgs e) {
			if(!checkHasMobileLayout.Checked && SheetDefCur.SheetDefNum>0 && EClipboardSheetDefs.IsSheetDefInUse(SheetDefCur.SheetDefNum)) {
				MsgBox.Show("This sheet is currently being used by eClipboard, which requires sheets to have a mobile layout. " +
					"You must remove this form from eClipboard rules before you can remove the mobile layout for this sheet.");
				checkHasMobileLayout.Checked=true;
			}
		}

		private void ReloadDefaultValues() {
			if(textDescription.Text==""){
				textDescription.Text=listSheetType.GetSelected<SheetTypeEnum>().GetDescription();
			}
			textFontSize.Text=SheetDefCur.FontSize.ToString();
			textWidth.Text=SheetDefCur.Width.ToString();
			textHeight.Text=SheetDefCur.Height.ToString();
			checkIsLandscape.Checked=SheetDefCur.IsLandscape;
		}

		private void butReduceFontSize_Click(object sender,EventArgs e) {
			float fontSize;
			try {
				fontSize=float.Parse(textFontSize.Text);
			}
			catch{
				MsgBox.Show(this,"Font size is invalid.");
				return;
			}
			if(fontSize<2.5) {
				textFontSize.Text="2";
				return;
			}
			fontSize-=0.5f;
			textFontSize.Text=fontSize.ToString();
			if(_formSheetDefEdit==null) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefs=SheetDefCur.SheetFieldDefs;
			for(int i=0;i<listSheetFieldDefs.Count();i++) {
				if(listSheetFieldDefs[i].FieldType!=SheetFieldType.InputField
					&& listSheetFieldDefs[i].FieldType!=SheetFieldType.OutputText
					&& listSheetFieldDefs[i].FieldType!=SheetFieldType.StaticText)
				{
					continue;
				}
				if(listSheetFieldDefs[i].FontSize<(2.5f)) {
					listSheetFieldDefs[i].FontSize=2;
					continue;
				}
				fontSize=listSheetFieldDefs[i].FontSize-0.5f;
				listSheetFieldDefs[i].FontSize=fontSize;
			}
			_formSheetDefEdit.UpdateSheetDefInBackground();
		}

		private void butAbout_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"When updating to 23.1 from previous versions, the horizontal space between letters within sheets has increased slightly.  If some text no longer fits inside the boxes, there are two ways to fix it:\r\n1. Make the field boundaries bigger.  This can work for just a few fields.\r\n2. Use this tool to reduce the font size for all text fields in the entire sheet. This can be done repeatedly if needed.");
			return;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textWidth.IsValid() || !textHeight.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listSheetType.SelectedIndex==-1 && !SheetDefs.IsDashboardType(SheetDefCur)) {
				MsgBox.Show(this,"Please select a sheet type first.");
				return;
			}
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Description may not be blank.");
				return;
			}
			SheetTypeEnum sheetTypeEnumSelected=listSheetType.GetSelected<SheetTypeEnum>();
			if(sheetTypeEnumSelected==SheetTypeEnum.ExamSheet) {
				//make sure description for exam sheet does not contain a ':' or a ';' because this interferes with pulling the exam sheet fields to fill a patient letter
				if(textDescription.Text.Contains(":") || textDescription.Text.Contains(";")) {
					MsgBox.Show(this,"Description for an Exam Sheet may not contain a ':' or a ';'.");
					return;
				}
			}
			if(comboFontName.GetSelected<string>()=="" || comboFontName.GetSelected<string>()==null){
				//not going to bother testing for validity unless it will cause a crash.
				MsgBox.Show(this,"Please select a font name first.");
				return;
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
			SheetDefCur.Description=textDescription.Text;
			if(!SheetDefs.IsDashboardType(SheetDefCur)) {
				SheetDefCur.SheetType=sheetTypeEnumSelected;
			}
			if(checkBypassLockDate.Checked) {
				SheetDefCur.BypassGlobalLock=BypassLockStatus.BypassAlways;
			}
			else {
				SheetDefCur.BypassGlobalLock=BypassLockStatus.NeverBypass;
			}
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(comboFontName.GetSelected<string>(),fontSize,XFontStyle.Regular);
			}
			catch {
				MsgBox.Show(Lan.g(this,$"Unsupported font: {comboFontName.GetSelected<string>()}. Please choose another font."));
				return;
			}
			SheetDefCur.FontName=comboFontName.GetSelected<string>();
			SheetDefCur.FontSize=fontSize;
			SheetDefCur.Width=PIn.Int(textWidth.Text);
			SheetDefCur.Height=PIn.Int(textHeight.Text);
			SheetDefCur.IsLandscape=checkIsLandscape.Checked;
			SheetDefCur.HasMobileLayout=checkHasMobileLayout.Checked;
			SheetDefCur.AutoCheckSaveImage=SetAutoCheckEnabled(SheetDefCur.SheetType) && checkAutoSaveCheck.Checked;
			SheetDefCur.AutoCheckSaveImageDocCategory=comboAutoSaveOverride.GetSelectedDefNum();
			if(SheetDefCur.SheetFieldDefs==null) { // A new sheet def, so it will not have sheet field defs. 
				DialogResult=DialogResult.OK;
				return;
			}
			//don't save to database here.
			DialogResult=DialogResult.OK;
		}

		private void FormSheetDef_FormClosed(object sender,FormClosedEventArgs e) {
			if(DialogResult==DialogResult.OK
				|| _sheetDefCopy==null
				|| _formSheetDefEdit==null) 
			{
				return;
			}
			_formSheetDefEdit.SheetDef_=_sheetDefCopy;
			_formSheetDefEdit.FillFieldList();
			SheetDefCur=_formSheetDefEdit.SheetDef_;
			_formSheetDefEdit.UpdateSheetDefInBackground();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		
	}
}