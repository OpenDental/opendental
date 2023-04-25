using Microsoft.CSharp;
//using Microsoft.Vsa;
using System.CodeDom.Compiler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;

namespace OpenDental{
	public delegate List<Document> SaveFileAsDocDelegate(bool isSigSave,Sheet sheetTP);

	///<summary></summary>
	public partial class FormTPsign : FormODBase {
		///<summary></summary>
		public int TotalPages;
		///<summary></summary>
		public PrintDocument PrintDocumentCur;
		private bool _sigChanged;
		public TreatPlan TreatPlanCur;
		///<summary>Must be sorted by primary key.</summary>
		private List<ProcTP> _listProcTPs;
		//private bool allowTopaz;
		///<summary>Should be set to ContrTreat.SaveTPAsDocument(). Can save multiple copies if multiple TP image categories are defined.</summary>
		public SaveFileAsDocDelegate SaveFileAsDocDelegate_;
		public Sheet SheetTP;
		///<summary>True if printing with sheets, false if printing with classic view.</summary>
		public bool DoPrintUsingSheets;
		///<summary>True if sheetTP has a practice signature box.</summary>
		private bool _hasSigPractice;
		private bool _hasSigPracticeChanged;

		///<summary></summary>
		public FormTPsign(){
			InitializeComponent();//Required for Windows Form Designer support
			InitializeLayoutManager();
		}

		private void FormTPsign_Load(object sender, System.EventArgs e) {
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(Permissions.TreatPlanSign,TreatPlanCur.DateTP)) {
				butOK.Enabled=false;
				signatureBoxWrapper.Enabled=false;
				signatureBoxWrapperPractice.Enabled=false;
				textTypeSig.Enabled=false;
				textTypeSigPractice.Enabled=false;
			}
			_hasSigPractice=false;
			if(SheetTP!=null) {
				_hasSigPractice=(SheetTP.SheetFields.Any(x => x.FieldType==SheetFieldType.SigBoxPractice) && DoPrintUsingSheets);
			}
			LayoutToolBar();
			ToolBarMain.Buttons["FullPage"].Pushed=true;
			LayoutManager.MoveLocation(previewContr,new Point(0,ToolBarMain.Bottom));
			LayoutManager.MoveSize(previewContr,new Size(ClientRectangle.Width,ClientRectangle.Height-ToolBarMain.Height-panelSig.Height));
			if(PrintDocumentCur==null) {//Only set when not pringing using sheets, shet via a MigraDoc.
				//TODO:Implement ODprintout pattern - MigraDoc
				//Just signing the TP, there is no way to print a Treat' Plan from the Sign TP window so suppress the printer dialogs.
				//Users will click the Print TP button from the Treat' Plan module when they want to print.
				PrinterL.ControlPreviewOverride=previewContr;//Sets the printdoc to previewContr.Document after validation. Otherwise shows error.
				SheetPrinting.Print(SheetTP,isPrintDocument:false,isPreviewMode:true);
				if(ODprintout.CurPrintout.SettingsErrorCode!=PrintoutErrorCode.Success) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				PrintDocumentCur=ODprintout.CurPrintout.PrintDoc;
			}
			else {//MigraDoc
				if(PrintDocumentCur.DefaultPageSettings.PrintableArea.Height==0) {
					PrintDocumentCur.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
				}
				previewContr.Document=PrintDocumentCur;
			}
			SetSize();
			ToolBarMain.Buttons["PageNum"].Text=(previewContr.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			_listProcTPs=ProcTPs.RefreshForTP(TreatPlanCur.TreatPlanNum);
			//Fill TP signature
			signatureBoxWrapper.SignatureMode=UI.SignatureBoxWrapper.SigMode.TreatPlan;
			string keyData= TreatPlans.GetKeyDataForSignatureHash(TreatPlanCur,_listProcTPs);
			signatureBoxWrapper.FillSignature(TreatPlanCur.SigIsTopaz,keyData,TreatPlanCur.Signature);
			//There are two signature boxes and only one SigIsTopaz column.
			//The patient and the practice could have signed the treatment plan using different mediums so attempt to load both just in case.
			if(!signatureBoxWrapper.IsValid) {
				signatureBoxWrapper.FillSignature(!TreatPlanCur.SigIsTopaz,keyData,TreatPlanCur.Signature);
			}
			SheetField sheetField;
			if(SheetTP!=null) {
				sheetField=SheetTP.SheetFields.FirstOrDefault(x => x.FieldType==SheetFieldType.SigBox);
				if(!string.IsNullOrEmpty(sheetField?.FieldName)) {
					labelSig.Text=$"{sheetField.FieldName} sign here --->";
				}
				SignatureBoxSheetCheck(sheetField,signatureBoxWrapper);
				sheetField=SheetTP.GetSheetFieldByName("SignatureText");
				if(sheetField!=null) {
					textTypeSig.Text=TreatPlanCur.SignatureText;
					labelTypeSig.Visible=true;
					textTypeSig.Visible=true;
				}
			}
			//Fill TP practice signature if printing using sheets
			if(!_hasSigPractice) {
				return;
			}
			signatureBoxWrapperPractice.Visible=true;
			labelSigPractice.Visible=true;
			signatureBoxWrapperPractice.SignatureMode=UI.SignatureBoxWrapper.SigMode.TreatPlan;
			signatureBoxWrapperPractice.FillSignature(TreatPlanCur.SigIsTopaz,keyData,TreatPlanCur.SignaturePractice);
			//There are two signature boxes and only one SigIsTopaz column.
			//The patient and the practice could have signed the treatment plan using different mediums so attempt to load both just in case.
			if(!signatureBoxWrapperPractice.IsValid) {
				signatureBoxWrapperPractice.FillSignature(!TreatPlanCur.SigIsTopaz,keyData,TreatPlanCur.SignaturePractice);
			}
			sheetField=SheetTP.SheetFields.FirstOrDefault(x => x.FieldType==SheetFieldType.SigBoxPractice);
			if(sheetField!=null && !string.IsNullOrEmpty(sheetField.FieldName)) {
				labelSigPractice.Text=$"{sheetField.FieldName} sign here --->";
			}
			SignatureBoxSheetCheck(sheetField,signatureBoxWrapperPractice);
			sheetField=SheetTP.GetSheetFieldByName("SignaturePracticeText");
			if(sheetField==null) {
				return;
			}
			textTypeSigPractice.Text=TreatPlanCur.SignaturePracticeText;
			labelTypeSigPractice.Visible=true;//defaulted to be hidden
			textTypeSigPractice.Visible=true;
		}

		private void SetSize(){
			if(!ToolBarMain.Buttons["FullPage"].Pushed) {//100%
				previewContr.Zoom=1;
				return;
			}
			//if document fits within window, then don't zoom it bigger; leave it at 100%
			if(PrintDocumentCur.DefaultPageSettings.PaperSize.Height<previewContr.ClientSize.Height
				&& PrintDocumentCur.DefaultPageSettings.PaperSize.Width<previewContr.ClientSize.Width)
			{
				previewContr.Zoom=1;
				return;
			}
			//if document ratio is taller than screen ratio, shrink by height.
			if(PrintDocumentCur.DefaultPageSettings.PaperSize.Height
				/PrintDocumentCur.DefaultPageSettings.PaperSize.Width
				> previewContr.ClientSize.Height / previewContr.ClientSize.Width)
			{
				previewContr.Zoom=previewContr.ClientSize.Height
					/(double)PrintDocumentCur.DefaultPageSettings.PaperSize.Height;
				return;
			}
			//otherwise, shrink by width
			previewContr.Zoom=previewContr.ClientSize.Width
				/(double)PrintDocumentCur.DefaultPageSettings.PaperSize.Width;
		}

		///<summary>SignatureBoxes can be restricted to providers, and are allowed to be signed electronically if enabled within sheets.</summary>
		private void SignatureBoxSheetCheck(SheetField sheetField,SignatureBoxWrapper signatureBoxWrapper) {
			if(sheetField==null) {
				return;
			}
			if(sheetField.IsSigProvRestricted && (Security.CurUser==null || Security.CurUser.UserNum<1 || Security.CurUser.ProvNum<1)) {
				signatureBoxWrapper.Enabled=false;
			}
			if(Security.CurUser==null) {
				return;
			}
			if (Security.CurUser.UserNum>0 //Is currently a logged in user
				&& sheetField.CanElectronicallySign) //If the field allows for electronic signature
			{
				signatureBoxWrapper.SetAllowDigitalSig(true,true);
			}
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,"Go Back One Page","Back"));
			ODToolBarButton toolBarButton=new ODToolBarButton("",-1,"","PageNum");
			toolBarButton.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(toolBarButton);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			toolBarButton=new ODToolBarButton(Lan.g(this,"FullPage"),-1,Lan.g(this,"FullPage"),"FullPage");
			toolBarButton.Style=ODToolBarButtonStyle.ToggleButton;
			ToolBarMain.Buttons.Add(toolBarButton);
			toolBarButton=new ODToolBarButton(Lan.g(this,"100%"),-1,Lan.g(this,"100%"),"100");
			toolBarButton.Style=ODToolBarButtonStyle.ToggleButton;
			ToolBarMain.Buttons.Add(toolBarButton);
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private void FormReport_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			LayoutManager.MoveWidth(previewContr,ClientSize.Width);
			LayoutManager.MoveHeight(previewContr,ClientSize.Height-panelSig.Height-ToolBarMain.Height);
		}

		/*//I don't think we need this:
		///<summary></summary>
		private void FillSignature() {
			textNote.Text="";
			sigBox.ClearTablet();
			if(!panelNote.Visible) {
				return;
			}
			DataRow obj=(DataRow)TreeDocuments.SelectedNode.Tag;
			textNote.Text=DocSelected.Note;
			sigBox.Visible=true;
			sigBox.SetTabletState(0);//never accepts input here
			labelInvalidSig.Visible=false;
			//Topaz box is not supported in Unix, since the required dll is Windows native.
			if(Environment.OSVersion.Platform!=PlatformID.Unix) {
				sigBoxTopaz.Location=sigBox.Location;//this puts both boxes in the same spot.
				sigBoxTopaz.Visible=false;
				((Topaz.SigPlusNET)sigBoxTopaz).SetTabletState(0);
			}
			//A machine running Unix will have DocSelected.SigIsTopaz set to false here, because the visibility of the panelNote
			//will be set to false in the case of Unix and SigIsTopaz. Therefore, the else part of this if-else clause is always
			//run on Unix systems.
			if(DocSelected.SigIsTopaz) {
				if(DocSelected.Signature!=null && DocSelected.Signature!="") {
					sigBox.Visible=false;
					sigBoxTopaz.Visible=true;
					((Topaz.SigPlusNET)sigBoxTopaz).ClearTablet();
					((Topaz.SigPlusNET)sigBoxTopaz).SetSigCompressionMode(0);
					((Topaz.SigPlusNET)sigBoxTopaz).SetEncryptionMode(0);
					((Topaz.SigPlusNET)sigBoxTopaz).SetKeyString(GetHashString(DocSelected));
					((Topaz.SigPlusNET)sigBoxTopaz).SetEncryptionMode(2);//high encryption
					((Topaz.SigPlusNET)sigBoxTopaz).SetSigCompressionMode(2);//high compression
					((Topaz.SigPlusNET)sigBoxTopaz).SetSigString(DocSelected.Signature);
					if(((Topaz.SigPlusNET)sigBoxTopaz).NumberOfTabletPoints() == 0) {
						labelInvalidSig.Visible=true;
					}
				}
			}
			else {
				sigBox.ClearTablet();
				if(DocSelected.Signature!=null && DocSelected.Signature!="") {
					sigBox.Visible=true;
					sigBoxTopaz.Visible=false;
					sigBox.SetKeyString(GetHashString(DocSelected));
					sigBox.SetSigString(DocSelected.Signature);
					if(sigBox.NumberOfTabletPoints()==0) {
						labelInvalidSig.Visible=true;
					}
					sigBox.SetTabletState(0);//not accepting input.
				}
			}
		}*/

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//MessageBox.Show(e.Button.Tag.ToString());
			switch(e.Button.Tag.ToString()){
				//case "Print":
				//	ToolBarPrint_Click();
				//	break;
				case "Back":
					OnBack_Click();
					break;
				case "Fwd":
					OnFwd_Click();
					break;
				case "FullPage":
					OnFullPage_Click();
					break;
				case "100":
					On100_Click();
					break;
				//case "Close":
				//	OnClose_Click();
				//	break;
			}
		}

		private void OnPrint_Click() {
			//TODO: Implement ODprintout pattern
			if(!PrinterL.SetPrinter(PrintDocumentCur,PrintSituation.TPPerio,TreatPlanCur.PatNum,"Signed treatment plan from "+TreatPlanCur.DateTP.ToShortDateString()+" printed")){
				return;
			}
			if(PrintDocumentCur.OriginAtMargins){
				//In the sheets framework,we had to set margins to 20 because of a bug in their preview control.
				//We now need to set it back to 0 for the actual printing.
				//Hopefully, this doesn't break anything else.
				PrintDocumentCur.DefaultPageSettings.Margins=new Margins(0,0,0,0);
			}
			try{
				PrintDocumentCur.Print();
			}
			catch(Exception e){
				MessageBox.Show(Lan.g(this,"Error: ")+e.Message);
			}
			DialogResult=DialogResult.OK;
		}

		private void OnClose_Click() {
			this.Close();
		}

		private void OnBack_Click(){
			if(previewContr.StartPage==0) {
				return;
			}
			previewContr.StartPage--;
			ToolBarMain.Buttons["PageNum"].Text=(previewContr.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			ToolBarMain.Invalidate();
		}

		private void OnFwd_Click(){
			//if(printPreviewControl2.StartPage==totalPages-1) return;
			previewContr.StartPage++;
			ToolBarMain.Buttons["PageNum"].Text=(previewContr.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			ToolBarMain.Invalidate();
		}

		private void OnFullPage_Click(){
			ToolBarMain.Buttons["100"].Pushed=!ToolBarMain.Buttons["FullPage"].Pushed;
			ToolBarMain.Invalidate();
			SetSize();
		}

		private void On100_Click(){
			ToolBarMain.Buttons["FullPage"].Pushed=!ToolBarMain.Buttons["100"].Pushed;
			ToolBarMain.Invalidate();
			SetSize();
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_sigChanged=true;
		}

		private void signatureBoxWrapperPractice_SignatureChanged(object sender,EventArgs e) {
			_hasSigPracticeChanged=true;
		}

		private void textTypeSig_TextChanged(object sender,EventArgs e) {
			_sigChanged=true;
		}

		private void textTypeSigPractice_TextChanged(object sender,EventArgs e) {
			_hasSigPracticeChanged=true;
		}

		private void SaveSignature() {
			//We need to set the typed signature name to the Tpcur object for both signature boxes before we get the GetKeyDataForSignatureSaving(...).
			//SignatureText and SignaturePracticeText are used to get the hash string.
			TreatPlanCur.SignatureText=textTypeSig.Text;
			TreatPlanCur.SignaturePracticeText=textTypeSigPractice.Text;
			SheetField sheetField;
			if(_sigChanged) {
				string keyData = TreatPlans.GetKeyDataForSignatureSaving(TreatPlanCur,_listProcTPs);
				TreatPlanCur.Signature=signatureBoxWrapper.GetSignature(keyData);
				TreatPlanCur.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
				TreatPlanCur.DateTSigned=MiscData.GetNowDateTime();
				sheetField=SheetTP?.GetSheetFieldByName("SignatureText");
				if(sheetField!=null) {
					sheetField.FieldValue=TreatPlanCur.SignatureText;
				}
				sheetField=SheetTP?.GetSheetFieldByName("DateTSigned");
				if(sheetField!=null) {
					sheetField.FieldValue=TreatPlanCur.DateTSigned.ToShortDateString();
				}
			}
			if(!_hasSigPractice || !_hasSigPracticeChanged) {
				return;
			}
			string keyData2 = TreatPlans.GetKeyDataForSignatureSaving(TreatPlanCur,_listProcTPs);
			TreatPlanCur.SignaturePractice=signatureBoxWrapperPractice.GetSignature(keyData2);
			TreatPlanCur.SigIsTopaz=signatureBoxWrapperPractice.GetSigIsTopaz();
			TreatPlanCur.DateTPracticeSigned=MiscData.GetNowDateTime();
			sheetField=SheetTP?.GetSheetFieldByName("SignaturePracticeText");
			if(sheetField!=null) {
				sheetField.FieldValue=TreatPlanCur.SignaturePracticeText;
			}
			sheetField=SheetTP?.GetSheetFieldByName("DateTPracticeSigned");
			if(sheetField!=null) {
				sheetField.FieldValue=TreatPlanCur.DateTPracticeSigned.ToShortDateString();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveSignature();//"saves" signature to TPCur, does not save to DB.
			TreatPlans.Update(TreatPlanCur);//save signature to DB.
			TreatPlanCur.ListProcTPs=ProcTPs.RefreshForTP(TreatPlanCur.TreatPlanNum);
			if(DoPrintUsingSheets) {
				SheetParameter.SetParameter(SheetTP,"TreatPlan",TreatPlanCur); //update TP on sheet to have new signature for generating pdfs
			}
			if(TreatPlanCur.SignaturePractice.Length>0 && TreatPlanCur.DocNum==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				_hasSigPracticeChanged=true;
			}
			if(TreatPlanCur.Signature.Length>0 && TreatPlanCur.DocNum==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				_sigChanged=true;
			}
			else if(TreatPlanCur.DocNum>0 && !Documents.DocExists(TreatPlanCur.DocNum) && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				//Setting SigChanged to True will resave document below.
				bool doResave=MsgBox.Show(this,MsgBoxButtons.YesNo,"Cannot find saved copy of signed PDF, would you like to resave the document?");
				_sigChanged=doResave;
				_hasSigPracticeChanged=doResave;
			}
			if(PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf) && SaveFileAsDocDelegate_!=null
				&& ((_sigChanged && TreatPlanCur.Signature.Length>0) || (_hasSigPracticeChanged && TreatPlanCur.SignaturePractice.Length>0)))
			{
				List<Document> docs=SaveFileAsDocDelegate_(true,SheetTP);
				if(docs.Count>0) {
					TreatPlanCur.DocNum=docs[0].DocNum;//attach first Doc to TP.
					TreatPlans.Update(TreatPlanCur); //update docnum. must be called after signature is updated.
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,TreatPlanCur.PatNum,"Sign TP");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormTPsign_FormClosing(object sender,FormClosingEventArgs e) {
			signatureBoxWrapperPractice?.SetTabletState(0);
			signatureBoxWrapper?.SetTabletState(0);
		}
	}
}