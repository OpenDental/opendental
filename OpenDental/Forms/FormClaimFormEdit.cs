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
		private bool controlIsDown;
		private bool mouseIsDown;
		private PointF mouseDownLoc;
		private PointF[] oldItemLocs;
		private string[] displayStrings;
		///<summary>A deep copy of the claim form passed into the constructor.  This is safe to modify.</summary>
		private ClaimForm _claimFormCur;
		///<summary>Don't modify.  A shallow copy of the claim form passed into the constructor.</summary>
		private ClaimForm _claimFormOld;
		///<summary>The maximum width of the claim form that will be visible.</summary>
		private const int _claimFormVisibleWidth=850;
		///<summary>Stores the image from its corresponding file name.
		///Key: File Name
		///Value: Image from the file, could be null.</summary>
		private Dictionary<string,Image> _dictImages=new Dictionary<string, Image>();

		///<summary>You must pass in the claimform to show.</summary>
		public FormClaimFormEdit(ClaimForm claimFormCur) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_claimFormOld=claimFormCur;
			_claimFormCur=claimFormCur.Copy();
		}

		private void FormClaimFormEdit_Load(object sender, System.EventArgs e) {
			if(_claimFormCur.IsInternal) {
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
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
				panelClaimForm.Enabled=false;
				labelInternal.Visible=true;
			}
			textDescription.Text=_claimFormCur.Description;
			checkIsHidden.Checked=_claimFormCur.IsHidden;
			checkPrintImages.Checked=_claimFormCur.PrintImages;
			textOffsetX.Text=_claimFormCur.OffsetX.ToString();
			textOffsetY.Text=_claimFormCur.OffsetY.ToString();
			textFormWidth.Text=_claimFormCur.Width.ToString();
			textFormHeight.Text=_claimFormCur.Height.ToString();
			if(_claimFormCur.FontName=="" || _claimFormCur.FontSize==0) {
				_claimFormCur.FontName="Arial";
				_claimFormCur.FontSize=8;
			}
			panelClaimForm.Width=_claimFormCur.Width;
			panelClaimForm.Height=_claimFormCur.Height;
			FillItems();
			panelClaimForm.Invalidate();
			SetHorizontalScrollBar();
			//panelClaimForm.Location=new Point(0,0);//set panel back to the top.
		}

		private void FormClaimFormEdit_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			if(_claimFormCur==null){
				return;
			}
			LayoutManager.MoveHeight(vScrollBar1,this.ClientSize.Height);
			vScrollBar1.Minimum=0;
			vScrollBar1.Maximum=_claimFormCur.Width > _claimFormVisibleWidth ? panelClaimForm.Height+hScrollBar.Height : panelClaimForm.Height;//1100;
			vScrollBar1.LargeChange=ClientSize.Height;
			SetHorizontalScrollBar();
		}

		///<summary>Sets the horizontal scroll bar's visibility, size, and location.</summary>
		private void SetHorizontalScrollBar() {
			if(panelClaimForm.Width > _claimFormVisibleWidth) {//If they set the width to greater than the default, show the scroll bar
				hScrollBar.Width=Math.Min(_claimFormVisibleWidth,Math.Min(panelClaimForm.Width,ClientSize.Width));
				hScrollBar.Location=new Point(0,ClientSize.Height-hScrollBar.Height);//Move to bottom of screen
				hScrollBar.Minimum=0;
				hScrollBar.Maximum=panelClaimForm.Width-_claimFormVisibleWidth;
				bool hScrollBarWasVisible= hScrollBar.Visible;
				hScrollBar.Visible=true;
				if(!hScrollBarWasVisible) {
					panelClaimForm.Location=new Point(panelClaimForm.Location.X,panelClaimForm.Location.Y-hScrollBar.Height);
				}
			}
			else {
				bool hScrollBarWasVisible=hScrollBar.Visible;
				hScrollBar.Visible=false;
				if(hScrollBarWasVisible) {
					panelClaimForm.Location=new Point(panelClaimForm.Location.X,Math.Min(panelClaimForm.Location.Y+hScrollBar.Height,0));
				}
			}
		}

		private void FillForm(){
			if(listItems.SelectedIndices.Count==0) {
				textXPos.Text="";
				textYPos.Text="";
				textWidth.Text="";
				textHeight.Text="";
			}
			else if(listItems.SelectedIndices.Count==1) {
				textXPos.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].XPos.ToString();
				textYPos.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].YPos.ToString();
				textWidth.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].Width.ToString();
				textHeight.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].Height.ToString();
			}
			else {//2 or more selected
				//only shows a value if all are the same
				bool xSame=true;
				bool ySame=true;
				bool wSame=true;
				bool hSame=true;
				for(int i=1;i<listItems.SelectedIndices.Count;i++) {//loop starts with second items to compare
					if(_claimFormCur.Items[listItems.SelectedIndices[i]].XPos!=
						_claimFormCur.Items[listItems.SelectedIndices[i-1]].XPos)
					{
						xSame=false;
					}
					if(_claimFormCur.Items[listItems.SelectedIndices[i]].YPos!=
						_claimFormCur.Items[listItems.SelectedIndices[i-1]].YPos)
					{
						ySame=false;
					}
					if(_claimFormCur.Items[listItems.SelectedIndices[i]].Width!=
						_claimFormCur.Items[listItems.SelectedIndices[i-1]].Width)
					{
						wSame=false;
					}
					if(_claimFormCur.Items[listItems.SelectedIndices[i]].Height!=
						_claimFormCur.Items[listItems.SelectedIndices[i-1]].Height)
					{
						hSame=false;
					}
				}
				if(xSame) {
					textXPos.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].XPos.ToString();
				}
				else {
					textXPos.Text="";
				}
				if(ySame) {
					textYPos.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].YPos.ToString();
				}
				else {
					textYPos.Text="";
				}
				if(wSame) {
					textWidth.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].Width.ToString();
				}
				else {
					textWidth.Text="";
				}
				if(hSame) {
					textHeight.Text=_claimFormCur.Items[listItems.SelectedIndices[0]].Height.ToString();
				}
				else {
					textHeight.Text="";
				}
			}
		}

		private void FillItems() {
			listItems.Items.Clear();
			for(int i=0;i<_claimFormCur.Items.Count;i++) {
				if(_claimFormCur.Items[i].ImageFileName=="") {//field
					listItems.Items.Add(_claimFormCur.Items[i].FieldName);
				}
				else {//image
					listItems.Items.Add(_claimFormCur.Items[i].ImageFileName);
				}
			}
			if(_claimFormCur.Items.Count>0) {
				listItems.ColumnWidth=_claimFormCur.Items.Max(x => TextRenderer.MeasureText(x.FieldName,listItems.Font).Width);
			}
		}

		private void panelClaimForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			//could make this much faster if invalidated only certain areas, but no time
			FillDisplayStrings();
			Graphics grfx=e.Graphics;
			Color myColor;
			float xPosRect;
			float xPosText;
			for(int i=0;i<_claimFormCur.Items.Count;i++){
				if(_claimFormCur.Items[i].ImageFileName==""){//field
					if(listItems.SelectedIndices.Contains(i))
						myColor=Color.Red;
					else myColor=Color.Blue;
					xPosRect=_claimFormCur.Items[i].XPos;
					xPosText=xPosRect;
					if(displayStrings[i]=="1234.00" || displayStrings[i]=="AB") {//right aligned fields: any amount field, or ICDindicatorAB
						xPosRect-=_claimFormCur.Items[i].Width;//this aligns it to the right
						xPosText-=grfx.MeasureString(displayStrings[i]
							,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)).Width;
					}
					grfx.DrawRectangle(new Pen(myColor)
						,xPosRect,_claimFormCur.Items[i].YPos
						,_claimFormCur.Items[i].Width,_claimFormCur.Items[i].Height);
					grfx.DrawString(displayStrings[i]
						,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)
						,new SolidBrush(myColor)
						,new RectangleF(xPosText,_claimFormCur.Items[i].YPos
						,_claimFormCur.Items[i].Width,_claimFormCur.Items[i].Height));
				}
				else{//image
					string extension="";
					Image thisImage;
					if(_dictImages.ContainsKey(_claimFormCur.Items[i].ImageFileName)) {
						extension=Path.GetExtension(_claimFormCur.Items[i].ImageFileName);
						thisImage=_dictImages[_claimFormCur.Items[i].ImageFileName];
					}
					else {
						thisImage=GetImage(_claimFormCur.Items[i],out extension);
						if(!_dictImages.ContainsKey(_claimFormCur.Items[i].ImageFileName)) {
							_dictImages.Add(_claimFormCur.Items[i].ImageFileName,thisImage);
						}
					}
					if(thisImage==null) {
						grfx.DrawString("IMAGE FILE NOT FOUND",new Font(FontFamily.GenericSansSerif,12,FontStyle.Bold)
							,Brushes.DarkRed,0,0);
						continue;
					}					
					if(extension==".jpg"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos
							,_claimFormCur.Items[i].YPos
							,(int)(thisImage.Width/thisImage.HorizontalResolution*100)
							,(int)(thisImage.Height/thisImage.VerticalResolution*100));
					}
					else if(extension==".gif"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos
							,_claimFormCur.Items[i].YPos
							,_claimFormCur.Items[i].Width
							,_claimFormCur.Items[i].Height);
					}
					else if(extension==".emf"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos
							,_claimFormCur.Items[i].YPos
							,thisImage.Width
							,thisImage.Height);
					}
				}
			}
		}

		///<summary>Gets the image from the A to Z folder. Will return null if the file is not found.</summary>
		private Image GetImage(ClaimFormItem claimFormItem,out string extension) {
			extension="";
			Image img=null;
			if(claimFormItem.ImageFileName=="ADA2006.gif") {
				img=CDT.Class1.GetADA2006();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2012.gif") {
				img=CDT.Class1.GetADA2012();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2012_J430D.gif") {
				img=CDT.Class1.GetADA2012_J430D();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2018_J432.gif") {
				img=CDT.Class1.GetADA2018_J432();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="ADA2019_J430.gif") {
				img=CDT.Class1.GetADA2019_J430();
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="1500_02_12.gif") {
				img=Properties.Resources._1500_02_12;
				extension=".gif";
			}
			else if(claimFormItem.ImageFileName=="DC-217.gif") {
				img=CDT.Class1.GetDC217();
				extension=".gif";
			}
			else {
				string fileName=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),claimFormItem.ImageFileName);
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					if(!File.Exists(fileName)) {
						return null;
					}
					img=Image.FromFile(fileName);
					extension=Path.GetExtension(fileName);
				}
				else if(CloudStorage.IsCloudStorage) {
					using FormProgress FormP=new FormProgress();
					FormP.DisplayText="Downloading...";
					FormP.NumberFormat="F";
					FormP.NumberMultiplication=1;
					FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					FormP.TickMS=1000;
					OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(ImageStore.GetPreferredAtoZpath()
								,claimFormItem.ImageFileName
								,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
					FormP.ShowDialog();
					if(FormP.DialogResult==DialogResult.Cancel) {
						state.DoCancel=true;
						return null;
					}
					//Download was successful
					using(MemoryStream stream=new MemoryStream(state.FileContent)) {
						img=Image.FromStream(stream);
						extension=Path.GetExtension(fileName);
					}
				}
			}
			return img;
		}

		private void FillDisplayStrings(){
			displayStrings=new string[_claimFormCur.Items.Count];
			for(int i=0;i<_claimFormCur.Items.Count;i++){
				switch(_claimFormCur.Items[i].FieldName){
					default://image="", or most fields = name of field
						displayStrings[i]=_claimFormCur.Items[i].FieldName;
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
						displayStrings[i]="X";
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
						displayStrings[i]="Y";
						break;
					case "PriInsST":
					case "OtherInsST":
						displayStrings[i]="ST";
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
						if(_claimFormCur.Items[i].FormatString=="")
							displayStrings[i]="";//DateTime.Today.ToShortDateString();
						else
							displayStrings[i]=DateTime.Today.ToString(_claimFormCur.Items[i].FormatString);
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
						displayStrings[i]="1234.00";
						break;
					case "MedUniformBillType":
						displayStrings[i]="831";
						break;
					case "MedAdmissionTypeCode":
					case "MedAdmissionSourceCode":
						displayStrings[i]="1";
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
						displayStrings[i]="01";
						break;
					case "ICDindicatorAB":
						displayStrings[i]="AB";
						break;
					case "Remarks":
						displayStrings[i]="This is a test of the remarks section of the claim form.";
						break;
					case "FixedText":
						displayStrings[i]=_claimFormCur.Items[i].FormatString;
						break;
					case "DateIllnessInjuryPregQualifier":
					case "DateOtherQualifier":
						displayStrings[i]="000";
						break;
					case "CorrectionType":
						displayStrings[i]="0";
						break;
				}//switch
			}//for
		}
		
		private void ScrollBars_Scroll(object sender,ScrollEventArgs e) {
			panelClaimForm.Location=new Point(-hScrollBar.Value,-vScrollBar1.Value);
		}

		private void textFormWidth_Validated(object sender,EventArgs e) {
			if(!textFormWidth.IsValid()) {
				return;
			}
			_claimFormCur.Width=PIn.Int(textFormWidth.Text);
			panelClaimForm.Width=_claimFormCur.Width;
			SetHorizontalScrollBar();
		}

		private void textFormHeight_Validated(object sender,EventArgs e) {
			if(!textFormHeight.IsValid()) {
				return;
			}
			_claimFormCur.Height=PIn.Int(textFormHeight.Text);
			panelClaimForm.Height=_claimFormCur.Height;
		}

		private void listItems_DoubleClick(object sender, System.EventArgs e) {
			if(listItems.SelectedIndices.Count==0) {
				return;
			}
			int index=listItems.SelectedIndices[0];
			ClaimFormItem selectedClaimFormItem= _claimFormCur.Items[index];
			using FormClaimFormItemEdit FormCFIE=new FormClaimFormItemEdit();
			FormCFIE.CFIcur=_claimFormCur.Items[index];
			FormCFIE.ShowDialog();
			if(FormCFIE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormCFIE.IsDeleted) {
				_claimFormCur.Items.RemoveAll(x => x.ClaimFormItemNum == selectedClaimFormItem.ClaimFormItemNum);
			}
			FillItems();
			panelClaimForm.Invalidate();
			//Reselect the claim form item that was edited if it still exists.  Do nothing if it was deleted and was the last in the list.
			if(listItems.Items.Count > index) {
				listItems.SetSelected(index,true);
			}
			FillForm();
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
				_claimFormCur.Items[listItems.SelectedIndices[i]].XPos=xPos;
			}
			FillForm();
			panelClaimForm.Invalidate();
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
				_claimFormCur.Items[listItems.SelectedIndices[i]].YPos=yPos;
			}
			FillForm();
			panelClaimForm.Invalidate();
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
				_claimFormCur.Items[listItems.SelectedIndices[i]].Width=width;
			}
			FillForm();
			panelClaimForm.Invalidate();
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
				_claimFormCur.Items[listItems.SelectedIndices[i]].Height=height;
			}
			FillForm();
			panelClaimForm.Invalidate();
		}

		private void listItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			FillForm();
			panelClaimForm.Invalidate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormClaimFormItemEdit FormCFIE=new FormClaimFormItemEdit();
			FormCFIE.CFIcur=new ClaimFormItem();
			FormCFIE.CFIcur.ClaimFormNum=_claimFormCur.ClaimFormNum;
			FormCFIE.CFIcur.YPos=540;
			FormCFIE.IsNew=true;
			FormCFIE.ShowDialog();
			if(FormCFIE.DialogResult!=DialogResult.OK || FormCFIE.IsDeleted) {
				return;
			}
			_claimFormCur.Items.Add(FormCFIE.CFIcur);
			FillItems();//also gets ListForForm
			listItems.ClearSelected();
			if(listItems.Items.Count > 0) {
				listItems.SetSelected(listItems.Items.Count-1,true);
			}
			panelClaimForm.Invalidate();//also Fills displayStrings
			FillForm();
		}

		private void butFont_Click(object sender, System.EventArgs e) {
			Font myFont=new Font(_claimFormCur.FontName,_claimFormCur.FontSize);
			fontDialog1.Font=myFont;
			if(fontDialog1.ShowDialog()!=DialogResult.OK){
				return;
			}
			if(fontDialog1.Font.Style!=FontStyle.Regular){
				MessageBox.Show(Lan.g(this,"Only regular font style allowed."));
			}
			_claimFormCur.FontName=fontDialog1.Font.Name;
			_claimFormCur.FontSize=fontDialog1.Font.Size;
			fontDialog1.Dispose();
			panelClaimForm.Invalidate();
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
				controlIsDown=true;
			}
			if(e.KeyCode==Keys.ShiftKey){
				return;
			}
			if(e.KeyCode==Keys.ControlKey){
				return;
			}
			//loop through all items selected and change them
			ClaimFormItem curItem;
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				curItem=_claimFormCur.Items[listItems.SelectedIndices[i]];
				switch(e.KeyCode){
					case Keys.Up:
						if(e.Shift)
							curItem.YPos-=10;
						else
							curItem.YPos-=1;
						break;
					case Keys.Down:
						if(e.Shift)
							curItem.YPos+=10;
						else
							curItem.YPos+=1;
						break;
					case Keys.Left:
						if(e.Shift)
							curItem.XPos-=10;
						else
							curItem.XPos-=1;
						break;
					case Keys.Right:
						if(e.Shift)
							curItem.XPos+=10;
						else
							curItem.XPos+=1;
						break;
				}
				curItem.XPos=Math.Max(curItem.XPos,0);
				curItem.XPos=Math.Min(curItem.XPos,_claimFormCur.Width);
				curItem.YPos=Math.Max(curItem.YPos,0);
				curItem.YPos=Math.Min(curItem.YPos,_claimFormCur.Height);
			}
			FillForm();
			panelClaimForm.Invalidate();
		}

		private void FormClaimFormEdit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			controlIsDown=false;
		}

		private void panelClaimForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=true;
			mouseDownLoc=new Point(e.X,e.Y);
			//find the item and select it in the list
			float width;//of item
			float height;
			Graphics grfx=panelClaimForm.CreateGraphics();
			//start at the end of the list and work backwards until match
			for(int i=_claimFormCur.Items.Count-1;i>=0;i--) {
				if(_claimFormCur.Items[i].Width==0 || _claimFormCur.Items[i].Height==0) {
					width=grfx.MeasureString(displayStrings[i]
						,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)).Width;
					height=grfx.MeasureString(displayStrings[i]
						,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)).Height;
				}
				else {//a width and height are available, so use them
					width=_claimFormCur.Items[i].Width;
					height=_claimFormCur.Items[i].Height;
				}
				bool rightAligned = displayStrings[i]=="1234.00" || displayStrings[i]=="AB";
				if((rightAligned ? e.X<_claimFormCur.Items[i].XPos : e.X>_claimFormCur.Items[i].XPos)
					&& (rightAligned ? e.X>_claimFormCur.Items[i].XPos-width : e.X<_claimFormCur.Items[i].XPos+width)
					&& e.Y>_claimFormCur.Items[i].YPos
					&& e.Y<_claimFormCur.Items[i].YPos+height) 
				{
					if(controlIsDown) {
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
			}
			grfx.Dispose();
			FillForm();//also sets the oldItemLocs
			oldItemLocs=new PointF[listItems.SelectedIndices.Count];
			for(int i=0;i<listItems.SelectedIndices.Count;i++) {//then a normal loop to set oldlocs for dragging
				oldItemLocs[i]=new PointF((float)_claimFormCur.Items[listItems.SelectedIndices[i]].XPos
					,(float)_claimFormCur.Items[listItems.SelectedIndices[i]].YPos);
			}
			panelClaimForm.Invalidate();
		}

		private void panelClaimForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!mouseIsDown || listItems.SelectedIndices.Count==0){
				return;
			}
			for(int i=0;i<listItems.SelectedIndices.Count;i++){
				ClaimFormItem claimFormItem=_claimFormCur.Items[listItems.SelectedIndices[i]];
				claimFormItem.XPos=oldItemLocs[i].X+e.X-mouseDownLoc.X;
				claimFormItem.YPos=oldItemLocs[i].Y+e.Y-mouseDownLoc.Y;
				claimFormItem.XPos=Math.Max(claimFormItem.XPos,0);
				claimFormItem.XPos=Math.Min(claimFormItem.XPos,_claimFormCur.Width);
				claimFormItem.YPos=Math.Max(claimFormItem.YPos,0);
				claimFormItem.YPos=Math.Min(claimFormItem.YPos,_claimFormCur.Height);
			}
			FillForm();
			panelClaimForm.Invalidate();
		}

		private void panelClaimForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=false;
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Claim form")+" "+_claimFormCur.Description+" "+Lan.g(this,"printed"),
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){//raised for each page to be printed.
			Graphics grfx=ev.Graphics;
			Color myColor;
			float xPosRect;
			float xPosText;
			for(int i=0;i<_claimFormCur.Items.Count;i++){
				if(_claimFormCur.Items[i].ImageFileName==""){//field
					myColor=Color.Blue;
					xPosRect=_claimFormCur.Items[i].XPos+_claimFormCur.OffsetX;
					xPosText=xPosRect;
					if(displayStrings[i]=="1234.00" || displayStrings[i]=="AB") {//right aligned fields: any amount field, or ICDindicatorAB
						xPosRect-=_claimFormCur.Items[i].Width;//this aligns it to the right
						xPosText-=grfx.MeasureString(displayStrings[i]
							,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)).Width;
					}
					grfx.DrawRectangle(new Pen(myColor)
						,xPosRect,_claimFormCur.Items[i].YPos+_claimFormCur.OffsetY
						,_claimFormCur.Items[i].Width,_claimFormCur.Items[i].Height);
					grfx.DrawString(displayStrings[i]
						,new Font(_claimFormCur.FontName,_claimFormCur.FontSize)
						,new SolidBrush(myColor)
						,new RectangleF(xPosText,_claimFormCur.Items[i].YPos+_claimFormCur.OffsetY
						,_claimFormCur.Items[i].Width,_claimFormCur.Items[i].Height));
				}
				else{//image
					if(!_claimFormCur.PrintImages){
						continue;
					}
					string fileName=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),_claimFormCur.Items[i].ImageFileName);
					Image thisImage=null;
					switch(_claimFormCur.Items[i].ImageFileName) {
						case "ADA2006.gif":
							thisImage=CDT.Class1.GetADA2006();
							break;
						case "ADA2012.gif":
							thisImage=CDT.Class1.GetADA2012();
							break;
						case "ADA2012_J430D.gif":
							thisImage=CDT.Class1.GetADA2012_J430D();
							break;
						case "ADA2018_J432.gif":
							thisImage=CDT.Class1.GetADA2018_J432();
							break;
						case "ADA2019_J430.gif":
							thisImage=CDT.Class1.GetADA2019_J430();
							break;
						case "1500_02_12.gif":
							thisImage=Properties.Resources._1500_02_12;
							break;
						default:
							if(!FileAtoZ.Exists(fileName)) {
								MsgBox.Show(this,"File not found.");
								continue;
							}
							thisImage=FileAtoZ.GetImage(fileName);
							if(thisImage==null) {
								continue;
							}
							break;
					}
					if(fileName.Substring(fileName.Length-3)=="jpg"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos+_claimFormCur.OffsetX
							,_claimFormCur.Items[i].YPos+_claimFormCur.OffsetY
							,(int)(thisImage.Width/thisImage.HorizontalResolution*100)
							,(int)(thisImage.Height/thisImage.VerticalResolution*100));
					}
					else if(fileName.Substring(fileName.Length-3)=="gif"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos+_claimFormCur.OffsetX
							,_claimFormCur.Items[i].YPos+_claimFormCur.OffsetY
							,_claimFormCur.Items[i].Width
							,_claimFormCur.Items[i].Height);
					}
					else if(fileName.Substring(fileName.Length-3)=="emf"){
						grfx.DrawImage(thisImage
							,_claimFormCur.Items[i].XPos+_claimFormCur.OffsetX
							,_claimFormCur.Items[i].YPos+_claimFormCur.OffsetY
							,thisImage.Width
							,thisImage.Height);
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
			_claimFormCur.Description=textDescription.Text;
			_claimFormCur.IsHidden=checkIsHidden.Checked;
			_claimFormCur.PrintImages=checkPrintImages.Checked;
			_claimFormCur.OffsetX=PIn.Int(textOffsetX.Text);
			_claimFormCur.OffsetY=PIn.Int(textOffsetY.Text);
			return true;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			ClaimForms.Update(_claimFormCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormClaimFormEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			foreach(Image img in _dictImages.Values) {
				img?.Dispose();
			}
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(_claimFormOld.IsNew) {
				ClaimForms.Delete(_claimFormCur);
			}
		}

		

		

		


		

		

		

		

		

		

		

		

		

		

		

		


	}
}





















