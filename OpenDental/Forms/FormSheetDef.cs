using System;
using System.Drawing.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using PdfSharp.Drawing;

namespace OpenDental {
	public partial class FormSheetDef:FormODBase {
		///<summary></summary>
		public SheetDef SheetDefCur;
		//private List<SheetFieldDef> AvailFields;
		public bool IsReadOnly;
		///<summary>On creation of a new sheetdef, the user must pick a description and a sheettype before allowing to start editing the sheet.  After the initial sheettype selection, this will be false, indicating that the user may not change the type.</summary>
		public bool IsInitial;
		///<summary>Currently selected SheetType in the list box.</summary>
		private SheetTypeEnum _sheetTypeSelected {
			get {
				return listSheetType.GetSelected<SheetTypeEnum>();
			}
		}

		public FormSheetDef() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetDef_Load(object sender,EventArgs e) {
			setHeightWidthMin();
			if(IsReadOnly){
				butOK.Enabled=false;
			}
			if(!IsInitial){
				listSheetType.Enabled=false;
				checkHasMobileLayout.Enabled=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			}
			textDescription.Text=SheetDefCur.Description;
			Func<SheetTypeEnum,string> fItemToString=new Func<SheetTypeEnum,string>((sheetType) => { return Lan.g("enumSheetTypeEnum",sheetType.GetDescription()); });
			//not allowed to change sheettype once created.
			listSheetType.Items.AddList(
				Enum.GetValues(typeof(SheetTypeEnum)).Cast<SheetTypeEnum>()
					//MedLabResults not allowed here.
					.Where(x => !ListTools.In(x,SheetTypeEnum.None,SheetTypeEnum.MedLabResults)
						&& !SheetDefs.IsDashboardType(x)
					)
					//Order alphabetical.
					.OrderBy(x => fItemToString(x)),
				//Text displayed for each item.
				fItemToString);
			if(!IsInitial) {
				for(int i=0; i<listSheetType.Items.Count;i++) {
					if((SheetTypeEnum)listSheetType.Items.GetObjectAt(i)==SheetDefCur.SheetType) {
						listSheetType.SetSelected(i);
					}
				}
			}
			InstalledFontCollection fColl=new InstalledFontCollection();
			for(int i=0;i<fColl.Families.Length;i++){
				comboFontName.Items.Add(fColl.Families[i].Name);
			}
			checkBypassLockDate.Checked=(SheetDefCur.BypassGlobalLock==BypassLockStatus.BypassAlways);
			comboFontName.Text=SheetDefCur.FontName;
			textFontSize.Text=SheetDefCur.FontSize.ToString();
			textWidth.Text=SheetDefCur.Width.ToString();
			textHeight.Text=SheetDefCur.Height.ToString();
			checkIsLandscape.Checked=SheetDefCur.IsLandscape;
			checkHasMobileLayout.Checked=SheetDefCur.HasMobileLayout;
			//Load is done. It is now safe to register for the selection change event.
			listSheetType.SelectedIndexChanged+=new EventHandler(listSheetType_SelectedIndexChanged);
			if(SheetDefs.IsDashboardType(SheetDefCur)) {
				labelSheetType.Visible=false;
				listSheetType.Visible=false;
				checkBypassLockDate.Visible=false;
				checkIsLandscape.Visible=false;
				checkHasMobileLayout.Visible=false;
			}
		}

		///<summary>Sets the minimum valid value (used for validation only) of the appropriate Height or Width field based on the bottom of the lowest field. Max values are set in the designer.</summary>
		private void setHeightWidthMin() {
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
			setHeightWidthMin();
		}

		private void listSheetType_SelectedIndexChanged(object sender,EventArgs e) {
			if(!IsInitial){
				return;
			}
			if(listSheetType.SelectedIndex==-1){
				return;
			}
			SheetDef sheetdef=null;
			checkHasMobileLayout.Enabled=false;
			if(SheetDefs.IsMobileAllowed(_sheetTypeSelected)) {
				checkHasMobileLayout.Enabled=true;
			}
			switch(_sheetTypeSelected){
				case SheetTypeEnum.LabelCarrier:
				case SheetTypeEnum.LabelPatient:
				case SheetTypeEnum.LabelReferral:
					sheetdef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientMail);
					if(textDescription.Text==""){
						textDescription.Text=_sheetTypeSelected.GetDescription();
					}
					comboFontName.Text=sheetdef.FontName;
					textFontSize.Text=sheetdef.FontSize.ToString();
					textWidth.Text=sheetdef.Width.ToString();
					textHeight.Text=sheetdef.Height.ToString();
					checkIsLandscape.Checked=sheetdef.IsLandscape;
					break;
				case SheetTypeEnum.ReferralSlip:
					sheetdef=SheetsInternal.GetSheetDef(SheetInternalType.ReferralSlip);
					if(textDescription.Text==""){
						textDescription.Text=_sheetTypeSelected.GetDescription();
					}
					comboFontName.Text=sheetdef.FontName;
					textFontSize.Text=sheetdef.FontSize.ToString();
					textWidth.Text=sheetdef.Width.ToString();
					textHeight.Text=sheetdef.Height.ToString();
					checkIsLandscape.Checked=sheetdef.IsLandscape;
					break;
				case SheetTypeEnum.PatientForm:
				case SheetTypeEnum.MedicalHistory:
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
			if(_sheetTypeSelected==SheetTypeEnum.ExamSheet) {
				//make sure description for exam sheet does not contain a ':' or a ';' because this interferes with pulling the exam sheet fields to fill a patient letter
				if(textDescription.Text.Contains(":") || textDescription.Text.Contains(";")) {
					MsgBox.Show(this,"Description for an Exam Sheet may not contain a ':' or a ';'.");
					return;
				}
			}
			if(comboFontName.Text==""){
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
				SheetDefCur.SheetType=_sheetTypeSelected;
			}
			if(checkBypassLockDate.Checked) {
				SheetDefCur.BypassGlobalLock=BypassLockStatus.BypassAlways;
			}
			else {
				SheetDefCur.BypassGlobalLock=BypassLockStatus.NeverBypass;
			}
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(comboFontName.Text,fontSize,XFontStyle.Regular);
			}
			catch {
				MsgBox.Show(Lan.g(this,$"Unsupported font: {comboFontName.Text}. Please choose another font."));
				return;
			}
			SheetDefCur.FontName=comboFontName.Text;
			SheetDefCur.FontSize=fontSize;
			SheetDefCur.Width=PIn.Int(textWidth.Text);
			SheetDefCur.Height=PIn.Int(textHeight.Text);
			SheetDefCur.IsLandscape=checkIsLandscape.Checked;
			SheetDefCur.HasMobileLayout=checkHasMobileLayout.Checked;
			//don't save to database here.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		
	}
}