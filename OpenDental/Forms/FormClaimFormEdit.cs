using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using CDT;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClaimFormEdit : FormODBase {
		//private bool shiftIsDown;
		private bool _isControlDown;
		private bool _isMouseDown;
		///<summary>In control coords.</summary>
		private PointF _pointFMouseDownLoc;
		///<summary>In doc coords.</summary>
		private PointF[] _pointFArrayOldItemLocs;
		///<summary>1 to 1 with _claimForm.Items</summary>
		private string[] _stringArrayDisplays;
		///<summary>A deep copy of the claim form passed into the constructor.  This is safe to modify.</summary>
		private ClaimForm _claimForm;
		///<summary>Don't modify.  A shallow copy of the claim form passed into the constructor.</summary>
		private ClaimForm _claimFormOld;
		///<summary>Stores the image from its corresponding file name.
		///Key: File Name
		///Value: Image from the file, could be null.</summary>
		private Dictionary<string,Image> _dictImages=new Dictionary<string, Image>();

		///<summary>You must pass in the claimform to show.</summary>
		public FormClaimFormEdit(ClaimForm claimForm) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_claimFormOld=claimForm;
			_claimForm=claimForm.Copy();
		}

		private void FormClaimFormEdit_Load(object sender, System.EventArgs e) {
			if(_claimForm.IsInternal) {
				textDescription.ReadOnly=true;
				checkIsHidden.Enabled=false;
				checkPrintImages.Enabled=false;
				textOffsetX.Enabled=false;
				textOffsetY.Enabled=false;
				butFont.Enabled=false;
				butAdd.Enabled=false;
				//Viewing the list of items can be useful when viewing an internal Claim Form.
				//This way they can view and highligh the fields they might care about.
				listItems.DoubleClick-=listItems_DoubleClick;
				textXPos.Enabled=false;
				textYPos.Enabled=false;
				textWidth.Enabled=false;
				textHeight.Enabled=false;
				butPrint.Visible=false;
				textFormWidth.Enabled=false;
				textFormHeight.Enabled=false;
				butSave.Visible=false;
				pictureBoxClaimForm.Enabled=false;
				labelInternal.Visible=true;
			}
			textDescription.Text=_claimForm.Description;
			checkIsHidden.Checked=_claimForm.IsHidden;
			checkPrintImages.Checked=_claimForm.PrintImages;
			textOffsetX.Text=_claimForm.OffsetX.ToString();
			textOffsetY.Text=_claimForm.OffsetY.ToString();
			textFormWidth.Text=_claimForm.Width.ToString();
			textFormHeight.Text=_claimForm.Height.ToString();
			if(_claimForm.FontName=="" || _claimForm.FontSize==0) {
				_claimForm.FontName="Arial";
				_claimForm.FontSize=8;
			}
			Size size=new Size(LayoutManager.Scale(_claimForm.Width),LayoutManager.Scale(_claimForm.Height));
			LayoutManager.MoveSize(pictureBoxClaimForm,size);
			FillListItems();
			pictureBoxClaimForm.Invalidate();
		}

		///<summary>Fills the textBoxes a the right with the X,Y,W,and H of the selected item(s).</summary>
		private void FillSelectedXYWH(){
			if(listItems.SelectedIndices.Count==0) {
				textXPos.Text="";
				textYPos.Text="";
				textWidth.Text="";
				textHeight.Text="";
			}
			else if(listItems.SelectedIndices.Count==1) {
				textXPos.Text=_claimForm.Items[listItems.SelectedIndices[0]].XPos.ToString();
				textYPos.Text=_claimForm.Items[listItems.SelectedIndices[0]].YPos.ToString();
				textWidth.Text=_claimForm.Items[listItems.SelectedIndices[0]].Width.ToString();
				textHeight.Text=_claimForm.Items[listItems.SelectedIndices[0]].Height.ToString();
			}
			else {//2 or more selected
				//only shows a value if all are the same
				bool isXSame=true;
				bool isYSame=true;
				bool isWSame=true;
				bool isHSame=true;
				for(int i=1;i<listItems.SelectedIndices.Count;i++) {//loop starts with second items to compare
					if(_claimForm.Items[listItems.SelectedIndices[i]].XPos!=
						_claimForm.Items[listItems.SelectedIndices[i-1]].XPos)
					{
						isXSame=false;
					}
					if(_claimForm.Items[listItems.SelectedIndices[i]].YPos!=
						_claimForm.Items[listItems.SelectedIndices[i-1]].YPos)
					{
						isYSame=false;
					}
					if(_claimForm.Items[listItems.SelectedIndices[i]].Width!=
						_claimForm.Items[listItems.SelectedIndices[i-1]].Width)
					{
						isWSame=false;
					}
					if(_claimForm.Items[listItems.SelectedIndices[i]].Height!=
						_claimForm.Items[listItems.SelectedIndices[i-1]].Height)
					{
						isHSame=false;
					}
				}
				if(isXSame) {
					textXPos.Text=_claimForm.Items[listItems.SelectedIndices[0]].XPos.ToString();
				}
				else {
					textXPos.Text="";
				}
				if(isYSame) {
					textYPos.Text=_claimForm.Items[listItems.SelectedIndices[0]].YPos.ToString();
				}
				else {
					textYPos.Text="";
				}
				if(isWSame) {
					textWidth.Text=_claimForm.Items[listItems.SelectedIndices[0]].Width.ToString();
				}
				else {
					textWidth.Text="";
				}
				if(isHSame) {
					textHeight.Text=_claimForm.Items[listItems.SelectedIndices[0]].Height.ToString();
				}
				else {
					textHeight.Text="";
				}
			}
		}

		private void FillListItems() {
			listItems.Items.Clear();
			for(int i=0;i<_claimForm.Items.Count;i++) {
				if(_claimForm.Items[i].ImageFileName=="") {//field
					listItems.Items.Add(_claimForm.Items[i].FieldName);
				}
				else {//image
					listItems.Items.Add(_claimForm.Items[i].ImageFileName);
				}
			}
		}

		private void panelClaimForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			//could make this much faster if invalidated only certain areas, but no time
			FillDisplayStrings();
			Graphics g=e.Graphics;
			//First, images
			for(int i=0;i<_claimForm.Items.Count;i++){
				if(_claimForm.Items[i].ImageFileName==""){
					continue;
				}
				string extension="";
				Image image;
				if(_dictImages.ContainsKey(_claimForm.Items[i].ImageFileName)) {
					extension=Path.GetExtension(_claimForm.Items[i].ImageFileName);
					image=_dictImages[_claimForm.Items[i].ImageFileName];
				}
				else {
					image=GetImage(_claimForm.Items[i],out extension);
					if(!_dictImages.ContainsKey(_claimForm.Items[i].ImageFileName)) {
						_dictImages.Add(_claimForm.Items[i].ImageFileName,image);
					}
				}
				if(image==null) {
					g.DrawString("IMAGE FILE NOT FOUND",new Font(FontFamily.GenericSansSerif,12,FontStyle.Bold)
						,Brushes.DarkRed,0,0);
					continue;
				}					
				if(extension==".jpg"){
					g.DrawImage(image
						,LayoutManager.ScaleF(_claimForm.Items[i].XPos)
						,LayoutManager.ScaleF(_claimForm.Items[i].YPos)
						,LayoutManager.ScaleF(image.Width/image.HorizontalResolution*100)
						,LayoutManager.ScaleF(image.Height/image.VerticalResolution*100));
				}
				else if(extension==".gif"){
					g.DrawImage(image
						,LayoutManager.ScaleF(_claimForm.Items[i].XPos)
						,LayoutManager.ScaleF(_claimForm.Items[i].YPos)
						,LayoutManager.ScaleF(_claimForm.Items[i].Width)
						,LayoutManager.ScaleF(_claimForm.Items[i].Height));
				}
				else if(extension==".emf"){
					g.DrawImage(image
						,LayoutManager.ScaleF(_claimForm.Items[i].XPos)
						,LayoutManager.ScaleF(_claimForm.Items[i].YPos)
						,LayoutManager.ScaleF(image.Width)
						,LayoutManager.ScaleF(image.Height));
				}
			}
			//Then text
			using Font font=new Font(_claimForm.FontName,LayoutManager.ScaleFontODZoom(_claimForm.FontSize));
			for(int i=0;i<_claimForm.Items.Count;i++){
				if(_claimForm.Items[i].ImageFileName!=""){
					continue;
				}
				Color colorPanel=Color.Blue;
				if(listItems.SelectedIndices.Contains(i)){
					colorPanel=Color.Red;
				}
				using SolidBrush solidBrush=new SolidBrush(colorPanel);
				using Pen pen=new Pen(solidBrush.Color);
				float xPosRect=LayoutManager.ScaleF(_claimForm.Items[i].XPos);
				float xPosText=xPosRect;
				if(_stringArrayDisplays[i]=="1234.00" || _stringArrayDisplays[i]=="AB") {//right aligned fields: any amount field, or ICDindicatorAB
					xPosRect-=LayoutManager.ScaleF(_claimForm.Items[i].Width);//this aligns it to the right
					xPosText-=g.MeasureString(_stringArrayDisplays[i],font).Width;
				}
				g.DrawRectangle(pen,xPosRect,LayoutManager.ScaleF(_claimForm.Items[i].YPos),
					LayoutManager.ScaleF(_claimForm.Items[i].Width),LayoutManager.ScaleF(_claimForm.Items[i].Height));
				g.DrawString(_stringArrayDisplays[i],font,solidBrush
					,new RectangleF(xPosText,LayoutManager.ScaleF(_claimForm.Items[i].YPos),
					LayoutManager.ScaleF(_claimForm.Items[i].Width),LayoutManager.ScaleF(_claimForm.Items[i].Height)));
			}
		}

		///<summary>Gets the image from the A to Z folder. Will return null if the file is not found.</summary>
		private Image GetImage(ClaimFormItem claimFormItem,out string extension) {
			extension="";
			Image image=null;
			if(claimFormItem.ImageFileName=="ADA2006.gif") {
				image=CDT.Class1.GetADA2006();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2012.gif") {
				image=CDT.Class1.GetADA2012();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2012_J430D.gif") {
				image=CDT.Class1.GetADA2012_J430D();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2018_J432.gif") {
				image=CDT.Class1.GetADA2018_J432();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2019_J430.gif") {
				image=CDT.Class1.GetADA2019_J430();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2024_J430.gif") {
				image=CDT.Class1.GetADA2024_J430();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="1500_02_12.gif") {
				image=Properties.Resources._1500_02_12;
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="DC-217.gif") {
				image=CDT.Class1.GetDC217();
				extension=".gif";
			}
			else {
				string fileName=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),claimFormItem.ImageFileName);
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					if(!File.Exists(fileName)) {
						return null;
					}
					image=Image.FromFile(fileName);
					extension=Path.GetExtension(fileName);
				}
				else if(CloudStorage.IsCloudStorage) {
					UI.ProgressWin progressWin=new UI.ProgressWin();
					progressWin.StartingMessage="Downloading...";
					byte[] byteArray=null;
					progressWin.ActionMain=() => {
						byteArray=CloudStorage.Download(ImageStore.GetPreferredAtoZpath(),claimFormItem.ImageFileName);
					};
					progressWin.ShowDialog();
					if(byteArray==null || byteArray.Length==0){
						return null;
					}
					//Download was successful
					using(MemoryStream memoryStream=new MemoryStream(byteArray)) {
						image=Image.FromStream(memoryStream);
						extension=Path.GetExtension(fileName);
					}
				}
			}
			return image;
		}

		private void FillDisplayStrings(){
			_stringArrayDisplays=new string[_claimForm.Items.Count];
			for(int i=0;i<_claimForm.Items.Count;i++){
				switch(_claimForm.Items[i].FieldName){
					default://image="", or most fields = name of field
						_stringArrayDisplays[i]=_claimForm.Items[i].FieldName;
						break;
					//bool
					case "IsPreAuth":
					case "IsStandardClaim":
					case "IsMedicaidClaim":
					case "IsGroupHealthPlan":
					case "OtherInsExists":
					case "OtherInsNotExists":
					case "OtherInsExistsDent":
					case "OtherInsExistsMed":
					case "OtherInsSubscrIsMale":
					case "OtherInsSubscrIsFemale":
					case "OtherInsSubscrIsGenderUnknown":
					case "OtherInsRelatIsSelf":
					case "OtherInsRelatIsSpouse":
					case "OtherInsRelatIsChild":
					case "OtherInsRelatIsOther":
					case "SubscrIsMale":
					case "SubscrIsFemale":
					case "SubscrIsGenderUnknown":
					case "SubscrIsMarried":
					case "SubscrIsSingle":
					case "SubscrIsFTStudent":
					case "SubscrIsPTStudent":
					case "RelatIsSelf":
					case "RelatIsSpouse":
					case "RelatIsChild":
					case "RelatIsOther":
					case "IsFTStudent":
					case "IsPTStudent":
					case "IsStudent":
					case "PatientIsMale":
					case "PatientIsFemale":
					case "PatientIsGenderUnknown":
					case "PatientIsMarried":
					case "PatientIsSingle":
					case "Miss1":
					case "Miss2":
					case "Miss3":
					case "Miss4":
					case "Miss5":
					case "Miss6":
					case "Miss7":
					case "Miss8":
					case "Miss9":
					case "Miss10":
					case "Miss11":
					case "Miss12":
					case "Miss13":
					case "Miss14":
					case "Miss15":
					case "Miss16":
					case "Miss17":
					case "Miss18":
					case "Miss19":
					case "Miss20":
					case "Miss21":
					case "Miss22":
					case "Miss23":
					case "Miss24":
					case "Miss25":
					case "Miss26":
					case "Miss27":
					case "Miss28":
					case "Miss29":
					case "Miss30":
					case "Miss31":
					case "Miss32":
					case "PlaceIsOffice":
					case "PlaceIsHospADA2002":
					case "PlaceIsExtCareFacilityADA2002":
					case "PlaceIsOtherADA2002":
					case "PlaceIsInpatHosp":
					case "PlaceIsOutpatHosp":
					case "PlaceIsAdultLivCareFac":
					case "PlaceIsSkilledNursFac":
					case "PlaceIsPatientsHome":
					case "PlaceIsOtherLocation":
					case "IsRadiographsAttached":
					case "RadiographsNotAttached":
					case "IsEnclosuresAttached":
					case "IsNotOrtho":
					case "IsOrtho":
					case "IsNotProsth":
					case "IsInitialProsth":
					case "IsNotReplacementProsth":
					case "IsReplacementProsth":
					case "IsOccupational":
					case "IsNotOccupational":
					case "IsAutoAccident":
					case "IsNotAutoAccident":
					case "IsOtherAccident":
					case "IsNotOtherAccident":
					case "IsNotAccident"://of either kind
					case "IsAccident":
					case "BillingDentistNumIsSSN":
					case "BillingDentistNumIsTIN":
					case "AcceptAssignmentY":
					case "AcceptAssignmentN":
					case "IsOutsideLab":
					case "IsNotOutsideLab":
						_stringArrayDisplays[i]="X";
						break;
					//short strings custom
					case "P1IsEmergency":
					case "P2IsEmergency":
					case "P3IsEmergency":
					case "P4IsEmergency":
					case "P5IsEmergency":
					case "P6IsEmergency":
					case "P7IsEmergency":
					case "P8IsEmergency":
					case "P9IsEmergency":
					case "P10IsEmergency":
					case "P11IsEmergency":
					case "P12IsEmergency":
					case "P13IsEmergency":
					case "P14IsEmergency":
					case "P15IsEmergency":
						_stringArrayDisplays[i]="Y";
						break;
					case "PriInsST":
					case "OtherInsST":
						_stringArrayDisplays[i]="ST";
						break;
					//date
					case "PatientDOB":
					case "SubscrDOB":
					case "OtherInsSubscrDOB":
					case "P1Date":
					case "P2Date":
					case "P3Date":
					case "P4Date":
					case "P5Date":
					case "P6Date":
					case "P7Date":
					case "P8Date":
					case "P9Date":
					case "P10Date":
					case "P11Date":
					case "P12Date":
					case "P13Date":
					case "P14Date":
					case "P15Date":
					case "PatientReleaseDate":
					case "PatientAssignmentDate":
					case "DateOrthoPlaced":
					case "DatePriorProsthPlaced":
					case "AccidentDate":
					case "TreatingDentistSigDate":
					case "DateIllnessInjuryPreg":
					case "DateOther":
						if(_claimForm.Items[i].FormatString=="")
							_stringArrayDisplays[i]="";//DateTime.Today.ToShortDateString();
						else
							_stringArrayDisplays[i]=DateTime.Today.ToString(_claimForm.Items[i].FormatString);
						break;
					case "P1Fee":
					case "P2Fee":
					case "P3Fee":
					case "P4Fee":
					case "P5Fee":
					case "P6Fee":
					case "P7Fee":
					case "P8Fee":
					case "P9Fee":
					case "P10Fee":
					case "P11Fee":
					case "P12Fee":
					case "P13Fee":
					case "P14Fee":
					case "P15Fee":
					case "P1Lab":
					case "P2Lab":
					case "P3Lab":
					case "P4Lab":
					case "P5Lab":
					case "P6Lab":
					case "P7Lab":
					case "P8Lab":
					case "P9Lab":
					case "P10Lab":
					case "P1FeeMinusLab":
					case "P2FeeMinusLab":
					case "P3FeeMinusLab":
					case "P4FeeMinusLab":
					case "P5FeeMinusLab":
					case "P6FeeMinusLab":
					case "P7FeeMinusLab":
					case "P8FeeMinusLab":
					case "P9FeeMinusLab":
					case "P10FeeMinusLab":
					case "MedInsAAmtDue":
					case "MedInsBAmtDue":
					case "MedInsCAmtDue":
					case "MedInsAPriorPmt":
					case "MedInsBPriorPmt":
					case "MedInsCPriorPmt":
					case "TotalFee":
					case "MedValAmount39a":
					case "MedValAmount39b":
					case "MedValAmount39c":
					case "MedValAmount39d":
					case "MedValAmount40a":
					case "MedValAmount40b":
					case "MedValAmount40c":
					case "MedValAmount40d":
					case "MedValAmount41a":
					case "MedValAmount41b":
					case "MedValAmount41c":
					case "MedValAmount41d":
					case "OutsideLabFee":
						_stringArrayDisplays[i]="1234.00";
						break;
					case "MedUniformBillType":
						_stringArrayDisplays[i]="831";
						break;
					case "MedAdmissionTypeCode":
					case "MedAdmissionSourceCode":
						_stringArrayDisplays[i]="1";
						break;
					case "MedPatientStatusCode":
					case "MedConditionCode18":
					case "MedConditionCode19":
					case "MedConditionCode20":
					case "MedConditionCode21":
					case "MedConditionCode22":
					case "MedConditionCode23":
					case "MedConditionCode24":
					case "MedConditionCode25":
					case "MedConditionCode26":
					case "MedConditionCode27":
					case "MedConditionCode28":
						_stringArrayDisplays[i]="01";
						break;
					case "ICDindicatorAB":
						_stringArrayDisplays[i]="AB";
						break;
					case "Remarks":
						_stringArrayDisplays[i]="This is a test of the remarks section of the claim form.";
						break;
					case "FixedText":
						_stringArrayDisplays[i]=_claimForm.Items[i].FormatString;
						break;
					case "DateIllnessInjuryPregQualifier":
					case "DateOtherQualifier":
						_stringArrayDisplays[i]="000";
						break;
					case "CorrectionType":
						_stringArrayDisplays[i]="0";
						break;
				}//switch
			}//for
		}

		private void textFormWidth_Validated(object sender,EventArgs e) {
			if(!textFormWidth.IsValid()) {
				return;
			}
			_claimForm.Width=PIn.Int(textFormWidth.Text);
			Size size=new Size(LayoutManager.Scale(_claimForm.Width),LayoutManager.Scale(_claimForm.Height));
			LayoutManager.MoveSize(pictureBoxClaimForm,size);
		}

		private void textFormHeight_Validated(object sender,EventArgs e) {
			if(!textFormHeight.IsValid()) {
				return;
			}
			_claimForm.Height=PIn.Int(textFormHeight.Text);
			Size size=new Size(LayoutManager.Scale(_claimForm.Width),LayoutManager.Scale(_claimForm.Height));
			LayoutManager.MoveSize(pictureBoxClaimForm,size);
		}

		private void listItems_DoubleClick(object sender, System.EventArgs e) {
			if(listItems.SelectedIndices.Count==0) {
				return;
			}
			int index=listItems.SelectedIndices[0];
			ClaimFormItem claimFormItemSelected= _claimForm.Items[index];
			using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
			formClaimFormItemEdit.ClaimFormItemCur=_claimForm.Items[index];
			formClaimFormItemEdit.ShowDialog();
			if(formClaimFormItemEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formClaimFormItemEdit.IsDeleted) {
				_claimForm.Items.RemoveAll(x => x.ClaimFormItemNum == claimFormItemSelected.ClaimFormItemNum);
			}
			FillListItems();
			pictureBoxClaimForm.Invalidate();
			//Reselect the claim form item that was edited if it still exists.  Do nothing if it was deleted and was the last in the list.
			if(listItems.Items.Count > index) {
				listItems.SetSelected(index,true);
			}
			FillSelectedXYWH();
		}

		private void textXPos_Validated(object sender, System.EventArgs e) {
			if(listItems.SelectedIndices.Count==0)
				return;
			if(listItems.SelectedIndices.Count > 1 && textXPos.Text=="")
				//blank means that the values for the selected items are not the same
				return;//so disregard unless you put in an actual value
			float xPos;
			try{xPos=Convert.ToSingle(textXPos.Text);}
			catch{xPos=0;}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				_claimForm.Items[listItems.SelectedIndices[i]].XPos=xPos;
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void textYPos_Validated(object sender, System.EventArgs e) {
			if(listItems.SelectedIndices.Count==0)
				return;
			if(listItems.SelectedIndices.Count > 1 && textYPos.Text=="")
				return;
			float yPos;
			try{yPos=Convert.ToSingle(textYPos.Text);}
			catch{yPos=0;}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				_claimForm.Items[listItems.SelectedIndices[i]].YPos=yPos;
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void textWidth_Validated(object sender, System.EventArgs e) {
			//MessageBox.Show("width");
			if(listItems.SelectedIndices.Count==0)
				return;
			if(listItems.SelectedIndices.Count > 1 && textWidth.Text=="")
				return;
			float width;
			try{width=Convert.ToSingle(textWidth.Text);}
			catch{width=0;}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				_claimForm.Items[listItems.SelectedIndices[i]].Width=width;
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void textHeight_Validated(object sender, System.EventArgs e) {
			if(listItems.SelectedIndices.Count==0)
				return;
			if(listItems.SelectedIndices.Count > 1 && textHeight.Text=="")
				return;
			float height;
			try{height=Convert.ToSingle(textHeight.Text);}
			catch{height=0;}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				_claimForm.Items[listItems.SelectedIndices[i]].Height=height;
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void listItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
			formClaimFormItemEdit.ClaimFormItemCur=new ClaimFormItem();
			formClaimFormItemEdit.ClaimFormItemCur.ClaimFormNum=_claimForm.ClaimFormNum;
			formClaimFormItemEdit.ClaimFormItemCur.YPos=540;
			formClaimFormItemEdit.IsNew=true;
			formClaimFormItemEdit.ShowDialog();
			if(formClaimFormItemEdit.DialogResult!=DialogResult.OK || formClaimFormItemEdit.IsDeleted) {
				return;
			}
			_claimForm.Items.Add(formClaimFormItemEdit.ClaimFormItemCur);
			FillListItems();//also gets ListForForm
			listItems.ClearSelected();
			if(listItems.Items.Count > 0) {
				listItems.SetSelected(listItems.Items.Count-1,true);
			}
			pictureBoxClaimForm.Invalidate();//also Fills _stringArrayDisplays
			FillSelectedXYWH();
		}

		private void butFont_Click(object sender, System.EventArgs e) {
			Font font=new Font(_claimForm.FontName,_claimForm.FontSize);
			fontDialog1.Font=font;
			if(fontDialog1.ShowDialog()!=DialogResult.OK){
				return;
			}
			if(fontDialog1.Font.Style!=FontStyle.Regular){
				MessageBox.Show(Lan.g(this,"Only regular font style allowed."));
			}
			_claimForm.FontName=fontDialog1.Font.Name;
			_claimForm.FontSize=fontDialog1.Font.Size;
			fontDialog1.Dispose();
			pictureBoxClaimForm.Invalidate();
		}

		private void FormClaimFormEdit_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			e.Handled=true;
			if(e.KeyCode!=Keys.Up
				&& e.KeyCode!=Keys.Down
				&& e.KeyCode!=Keys.Left
				&& e.KeyCode!=Keys.Right
				&& e.KeyCode!=Keys.ShiftKey
				&& e.KeyCode!=Keys.ControlKey){
				return;
			}
			if(e.Control){
				_isControlDown=true;
			}
			if(e.KeyCode==Keys.ShiftKey){
				return;
			}
			if(e.KeyCode==Keys.ControlKey){
				return;
			}
			//loop through all items selected and change them
			ClaimFormItem claimFormItem;
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				claimFormItem=_claimForm.Items[listItems.SelectedIndices[i]];
				switch(e.KeyCode){
					case Keys.Up:
						if(e.Shift)
							claimFormItem.YPos-=10;
						else
							claimFormItem.YPos-=1;
						break;
					case Keys.Down:
						if(e.Shift)
							claimFormItem.YPos+=10;
						else
							claimFormItem.YPos+=1;
						break;
					case Keys.Left:
						if(e.Shift)
							claimFormItem.XPos-=10;
						else
							claimFormItem.XPos-=1;
						break;
					case Keys.Right:
						if(e.Shift)
							claimFormItem.XPos+=10;
						else
							claimFormItem.XPos+=1;
						break;
				}
				claimFormItem.XPos=Math.Max(claimFormItem.XPos,0);
				claimFormItem.XPos=Math.Min(claimFormItem.XPos,_claimForm.Width);
				claimFormItem.YPos=Math.Max(claimFormItem.YPos,0);
				claimFormItem.YPos=Math.Min(claimFormItem.YPos,_claimForm.Height);
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void FormClaimFormEdit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			_isControlDown=false;
		}

		private void pictureBoxClaimForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=true;
			_pointFMouseDownLoc=new Point(e.X,e.Y);
			//find the item and select it in the list
			Graphics g=pictureBoxClaimForm.CreateGraphics();
			//start at the end of the list and work backwards until match
			for(int i=_claimForm.Items.Count-1;i>=0;i--) {
				float width=LayoutManager.ScaleF(_claimForm.Items[i].Width);
				float height=LayoutManager.ScaleF(_claimForm.Items[i].Height);
				using Font font=new Font(_claimForm.FontName,LayoutManager.ScaleFontODZoom(_claimForm.FontSize));
				if(_claimForm.Items[i].Width==0 || _claimForm.Items[i].Height==0) {
					width=g.MeasureString(_stringArrayDisplays[i],font).Width;
					height=g.MeasureString(_stringArrayDisplays[i],font).Height;
				}
				bool isRightAligned = _stringArrayDisplays[i]=="1234.00" || _stringArrayDisplays[i]=="AB";
				if(e.Y<LayoutManager.ScaleF(_claimForm.Items[i].YPos)){
					continue;
				}
				if(e.Y>LayoutManager.ScaleF(_claimForm.Items[i].YPos)+height){
					continue;
				}
				if(isRightAligned){
					if(e.X>LayoutManager.ScaleF(_claimForm.Items[i].XPos)){
						continue;
					}
					if(e.X<LayoutManager.ScaleF(_claimForm.Items[i].XPos)-width){
						continue;
					}
				}
				else{
					if(e.X<LayoutManager.ScaleF(_claimForm.Items[i].XPos)){
						continue;
					}
					if(e.X>LayoutManager.ScaleF(_claimForm.Items[i].XPos)+width){
						continue;
					}
				}
				if(_isControlDown) {
					if(listItems.SelectedIndices.Contains(i)) {//if this item already selected
						listItems.SetSelected(i,false);//unselect it
					}
					else {//if not selected
						listItems.SetSelected(i,true);//select it
					}
				}
				else {//control not down
					if(listItems.SelectedIndices.Count>1//if multiple items already selected
						&& listItems.SelectedIndices.Contains(i)) {//and this is one of them
						//don't do anything.  The user is getting ready to drag a group
					}
					else {
						listItems.ClearSelected();
						listItems.SetSelected(i,true);
					}
				}
				break;
			}
			g.Dispose();
			FillSelectedXYWH();
			_pointFArrayOldItemLocs=new PointF[listItems.SelectedIndices.Count];
			for(int i=0;i<listItems.SelectedIndices.Count;i++) {//then a normal loop to set _pointFArrayOldItemLocs for dragging
				_pointFArrayOldItemLocs[i]=new PointF(_claimForm.Items[listItems.SelectedIndices[i]].XPos
					,_claimForm.Items[listItems.SelectedIndices[i]].YPos);
			}
			pictureBoxClaimForm.Invalidate();
		}

		private void panelClaimForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDown || listItems.SelectedIndices.Count==0){
				return;
			}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				ClaimFormItem claimFormItem=_claimForm.Items[listItems.SelectedIndices[i]];
				claimFormItem.XPos=_pointFArrayOldItemLocs[i].X+LayoutManager.UnscaleF(e.X-_pointFMouseDownLoc.X);
				claimFormItem.YPos=_pointFArrayOldItemLocs[i].Y+LayoutManager.UnscaleF(e.Y-_pointFMouseDownLoc.Y);
				claimFormItem.XPos=Math.Max(claimFormItem.XPos,0);
				claimFormItem.XPos=Math.Min(claimFormItem.XPos,_claimForm.Width);//these are both in doc coords, so no scaling
				claimFormItem.YPos=Math.Max(claimFormItem.YPos,0);
				claimFormItem.YPos=Math.Min(claimFormItem.YPos,_claimForm.Height);
			}
			FillSelectedXYWH();
			pictureBoxClaimForm.Invalidate();
		}

		private void panelClaimForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=false;
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Claim form")+" "+_claimForm.Description+" "+Lan.g(this,"printed"),
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){//raised for each page to be printed.
			Graphics grfx=ev.Graphics;
			Color myColor;
			float xPosRect;
			float xPosText;
			for(int i=0;i<_claimForm.Items.Count;i++){
				if(_claimForm.Items[i].ImageFileName==""){//field
					myColor=Color.Blue;
					xPosRect=_claimForm.Items[i].XPos+_claimForm.OffsetX;
					xPosText=xPosRect;
					if(_stringArrayDisplays[i]=="1234.00" || _stringArrayDisplays[i]=="AB") {//right aligned fields: any amount field, or ICDindicatorAB
						xPosRect-=_claimForm.Items[i].Width;//this aligns it to the right
						xPosText-=grfx.MeasureString(_stringArrayDisplays[i]
							,new Font(_claimForm.FontName,_claimForm.FontSize)).Width;
					}
					grfx.DrawRectangle(new Pen(myColor)
						,xPosRect,_claimForm.Items[i].YPos+_claimForm.OffsetY
						,_claimForm.Items[i].Width,_claimForm.Items[i].Height);
					grfx.DrawString(_stringArrayDisplays[i]
						,new Font(_claimForm.FontName,_claimForm.FontSize)
						,new SolidBrush(myColor)
						,new RectangleF(xPosText,_claimForm.Items[i].YPos+_claimForm.OffsetY
						,_claimForm.Items[i].Width,_claimForm.Items[i].Height));
				}
				else{//image
					if(!_claimForm.PrintImages){
						continue;
					}
					string fileName=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),_claimForm.Items[i].ImageFileName);
					Image image=null;
					switch(_claimForm.Items[i].ImageFileName) {
						case "ADA2006.gif":
							image=CDT.Class1.GetADA2006();
							break;
						case "ADA2012.gif":
							image=CDT.Class1.GetADA2012();
							break;
						case "ADA2012_J430D.gif":
							image=CDT.Class1.GetADA2012_J430D();
							break;
						case "ADA2018_J432.gif":
							image=CDT.Class1.GetADA2018_J432();
							break;
						case "ADA2019_J430.gif":
							image=CDT.Class1.GetADA2019_J430();
							break;
						case "ADA2024_J430.gif":
							image=CDT.Class1.GetADA2024_J430();
							break;
						case "1500_02_12.gif":
							image=Properties.Resources._1500_02_12;
							break;
						default:
							if(!FileAtoZ.Exists(fileName)) {
								MsgBox.Show(this,"File not found.");
								continue;
							}
							image=FileAtoZ.GetImage(fileName);
							if(image==null) {
								continue;
							}
							break;
					}
					if(fileName.Substring(fileName.Length-3)=="jpg"){
						grfx.DrawImage(image
							,_claimForm.Items[i].XPos+_claimForm.OffsetX
							,_claimForm.Items[i].YPos+_claimForm.OffsetY
							,(int)(image.Width/image.HorizontalResolution*100)
							,(int)(image.Height/image.VerticalResolution*100));
					}
					else if(fileName.Substring(fileName.Length-3)=="gif"){
						grfx.DrawImage(image
							,_claimForm.Items[i].XPos+_claimForm.OffsetX
							,_claimForm.Items[i].YPos+_claimForm.OffsetY
							,_claimForm.Items[i].Width
							,_claimForm.Items[i].Height);
					}
					else if(fileName.Substring(fileName.Length-3)=="emf"){
						grfx.DrawImage(image
							,_claimForm.Items[i].XPos+_claimForm.OffsetX
							,_claimForm.Items[i].YPos+_claimForm.OffsetY
							,image.Width
							,image.Height);
					}
				}
			}
			ev.HasMorePages=false;
		}

		private bool ValidateFields(){
			if(!textOffsetX.IsValid() || !textOffsetY.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(textDescription.Text=="") {
				MessageBox.Show(Lan.g(this,"You must enter a description first."));
				return false;
			}
			_claimForm.Description=textDescription.Text;
			_claimForm.IsHidden=checkIsHidden.Checked;
			_claimForm.PrintImages=checkPrintImages.Checked;
			_claimForm.OffsetX=PIn.Int(textOffsetX.Text);
			_claimForm.OffsetY=PIn.Int(textOffsetY.Text);
			return true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			ClaimForms.Update(_claimForm);
			DialogResult=DialogResult.OK;
		}

		private void FormClaimFormEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			foreach(Image img in _dictImages.Values) {
				img?.Dispose();
			}
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(_claimFormOld.IsNew) {
				ClaimForms.Delete(_claimForm);
			}
		}

	}
}