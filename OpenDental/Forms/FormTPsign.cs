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
	public class FormTPsign : FormODBase {
		private System.ComponentModel.IContainer components;
		///<summary></summary>
		public int TotalPages;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.PrintPreviewControl previewContr;
		///<summary></summary>
		public PrintDocument Document;
		private Panel panelSig;
		private Label labelSig;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private bool _sigChanged;
		public TreatPlan TPcur;
		///<summary>Must be sorted by primary key.</summary>
		private List<ProcTP> proctpList;
				//private bool allowTopaz;
		///<summary>Should be set to ContrTreat.SaveTPAsDocument(). Can save multiple copies if multiple TP image categories are defined.</summary>
		public SaveFileAsDocDelegate SaveDocDelegate;
		///<summary>Currently the size of this control is (331,79). If this is ever changed then eClipboard needs to be considered.</summary>
		private SignatureBoxWrapper signatureBoxWrapper;
		public Sheet SheetTP;
		///<summary>True if printing with sheets, false if printing with classic view.</summary>
		public bool DoPrintUsingSheets;
		///<summary>Currently the size of this control is (331,79). If this is ever changed then eClipboard needs to be considered.</summary>
		private SignatureBoxWrapper signatureBoxWrapperPractice;
		private Label labelSigPractice;
		///<summary>True if sheetTP has a practice signature box.</summary>
		private bool _hasSigPractice;
		private TextBox textTypeSig;
		private Label labelTypeSig;
		private TextBox textTypeSigPractice;
		private Label labelTypeSigPractice;
		private bool _hasSigPracticeChanged;

		///<summary></summary>
		public FormTPsign(){
			InitializeComponent();//Required for Windows Form Designer support
			InitializeLayoutManager();
		}

		/// <summary>Clean up any resources being used.</summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTPsign));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.textTypeSigPractice = new System.Windows.Forms.TextBox();
			this.labelTypeSigPractice = new System.Windows.Forms.Label();
			this.signatureBoxWrapperPractice = new OpenDental.UI.SignatureBoxWrapper();
			this.labelSigPractice = new System.Windows.Forms.Label();
			this.panelSig = new System.Windows.Forms.Panel();
			this.textTypeSig = new System.Windows.Forms.TextBox();
			this.labelTypeSig = new System.Windows.Forms.Label();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelSig = new System.Windows.Forms.Label();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.previewContr = new System.Windows.Forms.PrintPreviewControl();
			this.panelSig.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "");
			this.imageListMain.Images.SetKeyName(1, "");
			this.imageListMain.Images.SetKeyName(2, "");
			// 
			// textTypeSigPractice
			// 
			this.textTypeSigPractice.Location = new System.Drawing.Point(651, 86);
			this.textTypeSigPractice.Name = "textTypeSigPractice";
			this.textTypeSigPractice.Size = new System.Drawing.Size(331, 20);
			this.textTypeSigPractice.TabIndex = 228;
			this.textTypeSigPractice.Visible = false;
			this.textTypeSigPractice.TextChanged += new System.EventHandler(this.textTypeSigPractice_TextChanged);
			// 
			// labelTypeSigPractice
			// 
			this.labelTypeSigPractice.Location = new System.Drawing.Point(502, 87);
			this.labelTypeSigPractice.Name = "labelTypeSigPractice";
			this.labelTypeSigPractice.Size = new System.Drawing.Size(147, 17);
			this.labelTypeSigPractice.TabIndex = 229;
			this.labelTypeSigPractice.Text = "Type name here";
			this.labelTypeSigPractice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTypeSigPractice.Visible = false;
			// 
			// signatureBoxWrapperPractice
			// 
			this.signatureBoxWrapperPractice.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapperPractice.Location = new System.Drawing.Point(651, 3);
			this.signatureBoxWrapperPractice.Name = "signatureBoxWrapperPractice";
			this.signatureBoxWrapperPractice.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapperPractice.Size = new System.Drawing.Size(331, 79);
			this.signatureBoxWrapperPractice.TabIndex = 182;
			this.signatureBoxWrapperPractice.UserSig = null;
			this.signatureBoxWrapperPractice.Visible = false;
			this.signatureBoxWrapperPractice.SignatureChanged += new System.EventHandler(this.signatureBoxWrapperPractice_SignatureChanged);
			// 
			// labelSigPractice
			// 
			this.labelSigPractice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSigPractice.Location = new System.Drawing.Point(496, 4);
			this.labelSigPractice.Name = "labelSigPractice";
			this.labelSigPractice.Size = new System.Drawing.Size(153, 41);
			this.labelSigPractice.TabIndex = 92;
			this.labelSigPractice.Text = "Practice Sign Here --->";
			this.labelSigPractice.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelSigPractice.Visible = false;
			// 
			// panelSig
			// 
			this.panelSig.Controls.Add(this.textTypeSigPractice);
			this.panelSig.Controls.Add(this.textTypeSig);
			this.panelSig.Controls.Add(this.labelTypeSigPractice);
			this.panelSig.Controls.Add(this.labelTypeSig);
			this.panelSig.Controls.Add(this.signatureBoxWrapperPractice);
			this.panelSig.Controls.Add(this.labelSigPractice);
			this.panelSig.Controls.Add(this.signatureBoxWrapper);
			this.panelSig.Controls.Add(this.butCancel);
			this.panelSig.Controls.Add(this.butOK);
			this.panelSig.Controls.Add(this.labelSig);
			this.panelSig.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelSig.Location = new System.Drawing.Point(0, 581);
			this.panelSig.Name = "panelSig";
			this.panelSig.Size = new System.Drawing.Size(1159, 115);
			this.panelSig.TabIndex = 92;
			// 
			// textTypeSig
			// 
			this.textTypeSig.Location = new System.Drawing.Point(162, 88);
			this.textTypeSig.Name = "textTypeSig";
			this.textTypeSig.Size = new System.Drawing.Size(331, 20);
			this.textTypeSig.TabIndex = 230;
			this.textTypeSig.Visible = false;
			this.textTypeSig.TextChanged += new System.EventHandler(this.textTypeSig_TextChanged);
			// 
			// labelTypeSig
			// 
			this.labelTypeSig.Location = new System.Drawing.Point(13, 89);
			this.labelTypeSig.Name = "labelTypeSig";
			this.labelTypeSig.Size = new System.Drawing.Size(147, 17);
			this.labelTypeSig.TabIndex = 231;
			this.labelTypeSig.Text = "Type name here";
			this.labelTypeSig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTypeSig.Visible = false;
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(162, 3);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(331, 79);
			this.signatureBoxWrapper.TabIndex = 182;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.SignatureChanged += new System.EventHandler(this.signatureBoxWrapper_SignatureChanged);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1012, 70);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 94;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1012, 38);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 93;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelSig
			// 
			this.labelSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSig.Location = new System.Drawing.Point(7, 4);
			this.labelSig.Name = "labelSig";
			this.labelSig.Size = new System.Drawing.Size(153, 41);
			this.labelSig.TabIndex = 92;
			this.labelSig.Text = "Please Sign Here --->";
			this.labelSig.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1159, 25);
			this.ToolBarMain.TabIndex = 5;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// previewContr
			// 
			this.previewContr.AutoZoom = false;
			this.previewContr.Location = new System.Drawing.Point(10, 41);
			this.previewContr.Name = "previewContr";
			this.previewContr.Size = new System.Drawing.Size(806, 423);
			this.previewContr.TabIndex = 6;
			// 
			// FormTPsign
			// 
			this.ClientSize = new System.Drawing.Size(1099, 696);
			this.Controls.Add(this.panelSig);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.previewContr);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTPsign";
			this.Text = "Report";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTPsign_FormClosing);
			this.Load += new System.EventHandler(this.FormTPsign_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormReport_Layout);
			this.panelSig.ResumeLayout(false);
			this.panelSig.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void FormTPsign_Load(object sender, System.EventArgs e) {
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(Permissions.TreatPlanSign,TPcur.DateTP)) {
				butOK.Enabled=false;
				signatureBoxWrapper.Enabled=false;
				signatureBoxWrapperPractice.Enabled=false;
				textTypeSig.Enabled=false;
				textTypeSigPractice.Enabled=false;
			}
			_hasSigPractice=(SheetTP==null ? false : (SheetTP.SheetFields.Any(x => x.FieldType==SheetFieldType.SigBoxPractice) && DoPrintUsingSheets));
			LayoutToolBar();
			ToolBarMain.Buttons["FullPage"].Pushed=true;
			LayoutManager.MoveLocation(previewContr,new Point(0,ToolBarMain.Bottom));
			LayoutManager.MoveSize(previewContr,new Size(ClientRectangle.Width,ClientRectangle.Height-ToolBarMain.Height-panelSig.Height));
			if(Document==null) {//Only set when not pringing using sheets, shet via a MigraDoc.
				//TODO:Implement ODprintout pattern - MigraDoc
				//Just signing the TP, there is no way to print a Treat' Plan from the Sign TP window so suppress the printer dialogs.
				//Users will click the Print TP button from the Treat' Plan module when they want to print.
				PrinterL.ControlPreviewOverride=previewContr;//Sets the printdoc to previewContr.Document after validation. Otherwise shows error.
				SheetPrinting.Print(SheetTP,isPrintDocument:false,isPreviewMode:true);
				if(ODprintout.CurPrintout.SettingsErrorCode!=PrintoutErrorCode.Success) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				Document=ODprintout.CurPrintout.PrintDoc;
			}
			else {//MigraDoc
				if(Document.DefaultPageSettings.PrintableArea.Height==0) {
					Document.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
				}
				previewContr.Document=Document;
			}
			SetSize();
			ToolBarMain.Buttons["PageNum"].Text=(previewContr.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			proctpList=ProcTPs.RefreshForTP(TPcur.TreatPlanNum);
			//Fill TP signature
			signatureBoxWrapper.SignatureMode=UI.SignatureBoxWrapper.SigMode.TreatPlan;
			string keyData= TreatPlans.GetKeyDataForSignatureHash(TPcur,proctpList);
			signatureBoxWrapper.FillSignature(TPcur.SigIsTopaz,keyData,TPcur.Signature);
			SheetField sheetField;
			if(SheetTP!=null) {
				sheetField=SheetTP.SheetFields.FirstOrDefault(x => x.FieldType==SheetFieldType.SigBox);
				if(sheetField!=null && !string.IsNullOrEmpty(sheetField.FieldName)) {
					labelSig.Text=$"{sheetField.FieldName} sign here --->";
				}
				sheetField=SheetTP.GetSheetFieldByName("SignatureText");
				if(sheetField!=null) {
					textTypeSig.Text=TPcur.SignatureText;
					labelTypeSig.Visible=true;
					textTypeSig.Visible=true;
				}
			}
			//Fill TP practice signature if printing using sheets
			if(_hasSigPractice) {
				signatureBoxWrapperPractice.Visible=true;
				labelSigPractice.Visible=true;
				signatureBoxWrapperPractice.SignatureMode=UI.SignatureBoxWrapper.SigMode.TreatPlan;
				signatureBoxWrapperPractice.FillSignature(TPcur.SigIsTopaz,keyData,TPcur.SignaturePractice);
				sheetField=SheetTP.SheetFields.FirstOrDefault(x => x.FieldType==SheetFieldType.SigBoxPractice);
				if(sheetField!=null && !string.IsNullOrEmpty(sheetField.FieldName)) {
					labelSigPractice.Text=$"{sheetField.FieldName} sign here --->";
				}
				sheetField=SheetTP.GetSheetFieldByName("SignaturePracticeText");
				if(sheetField!=null) {
					textTypeSigPractice.Text=TPcur.SignaturePracticeText;
					labelTypeSigPractice.Visible=true;//defaulted to be hidden
					textTypeSigPractice.Visible=true;
				}
			}
		}

		private void SetSize(){
			if(ToolBarMain.Buttons["FullPage"].Pushed){
				//if document fits within window, then don't zoom it bigger; leave it at 100%
				if(Document.DefaultPageSettings.PaperSize.Height<previewContr.ClientSize.Height
					&& Document.DefaultPageSettings.PaperSize.Width<previewContr.ClientSize.Width) {
					previewContr.Zoom=1;
				}
				//if document ratio is taller than screen ratio, shrink by height.
				else if(Document.DefaultPageSettings.PaperSize.Height
					/Document.DefaultPageSettings.PaperSize.Width
					> previewContr.ClientSize.Height / previewContr.ClientSize.Width) {
					previewContr.Zoom=((double)previewContr.ClientSize.Height
						/(double)Document.DefaultPageSettings.PaperSize.Height);
				}
				//otherwise, shrink by width
				else {
					previewContr.Zoom=((double)previewContr.ClientSize.Width
						/(double)Document.DefaultPageSettings.PaperSize.Width);
				}
			}
			else{//100%
				previewContr.Zoom=1;
			}
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,"Go Back One Page","Back"));
			ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			button.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			button=new ODToolBarButton(Lan.g(this,"FullPage"),-1,Lan.g(this,"FullPage"),"FullPage");
			button.Style=ODToolBarButtonStyle.ToggleButton;
			ToolBarMain.Buttons.Add(button);
			button=new ODToolBarButton(Lan.g(this,"100%"),-1,Lan.g(this,"100%"),"100");
			button.Style=ODToolBarButtonStyle.ToggleButton;
			ToolBarMain.Buttons.Add(button);
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
			if(!PrinterL.SetPrinter(Document,PrintSituation.TPPerio,TPcur.PatNum,"Signed treatment plan from "+TPcur.DateTP.ToShortDateString()+" printed")){
				return;
			}
			if(Document.OriginAtMargins){
				//In the sheets framework,we had to set margins to 20 because of a bug in their preview control.
				//We now need to set it back to 0 for the actual printing.
				//Hopefully, this doesn't break anything else.
				Document.DefaultPageSettings.Margins=new Margins(0,0,0,0);
			}
			try{
				Document.Print();
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
			if(previewContr.StartPage==0) return;
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
			TPcur.SignatureText=textTypeSig.Text;
			TPcur.SignaturePracticeText=textTypeSigPractice.Text;
			SheetField sheetField;
			if(_sigChanged) {
				string keyData = TreatPlans.GetKeyDataForSignatureSaving(TPcur,proctpList);
				TPcur.Signature=signatureBoxWrapper.GetSignature(keyData);
				TPcur.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
				TPcur.DateTSigned=MiscData.GetNowDateTime();
				sheetField=SheetTP?.GetSheetFieldByName("SignatureText");
				if(sheetField!=null) {
					sheetField.FieldValue=TPcur.SignatureText;
				}
				sheetField=SheetTP?.GetSheetFieldByName("DateTSigned");
				if(sheetField!=null) {
					sheetField.FieldValue=TPcur.DateTSigned.ToShortDateString();
				}
			}
			if(_hasSigPractice && _hasSigPracticeChanged) {
				string keyData = TreatPlans.GetKeyDataForSignatureSaving(TPcur,proctpList);
				TPcur.SignaturePractice=signatureBoxWrapperPractice.GetSignature(keyData);
				TPcur.SigIsTopaz=signatureBoxWrapperPractice.GetSigIsTopaz();
				TPcur.DateTPracticeSigned=MiscData.GetNowDateTime();
				sheetField=SheetTP.GetSheetFieldByName("SignaturePracticeText");
				if(sheetField!=null) {
					sheetField.FieldValue=TPcur.SignaturePracticeText;
				}
				sheetField=SheetTP.GetSheetFieldByName("DateTPracticeSigned");
				if(sheetField!=null) {
					sheetField.FieldValue=TPcur.DateTPracticeSigned.ToShortDateString();
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveSignature();//"saves" signature to TPCur, does not save to DB.
			TreatPlans.Update(TPcur);//save signature to DB.
			TPcur.ListProcTPs=ProcTPs.RefreshForTP(TPcur.TreatPlanNum);
			if(DoPrintUsingSheets) {
				SheetParameter.SetParameter(SheetTP,"TreatPlan",TPcur); //update TP on sheet to have new signature for generating pdfs
			}
			if(TPcur.SignaturePractice.Length>0 && TPcur.DocNum==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				_hasSigPracticeChanged=true;
			}
			if(TPcur.Signature.Length>0 && TPcur.DocNum==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				_sigChanged=true;
			}
			else if(TPcur.DocNum>0 && !Documents.DocExists(TPcur.DocNum) && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				//Setting SigChanged to True will resave document below.
				bool doResave=MsgBox.Show(this,MsgBoxButtons.YesNo,"Cannot find saved copy of signed PDF, would you like to resave the document?");
				_sigChanged=doResave;
				_hasSigPracticeChanged=doResave;
			}
			if(PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf) && SaveDocDelegate!=null
				&& ((_sigChanged && TPcur.Signature.Length>0) || (_hasSigPracticeChanged && TPcur.SignaturePractice.Length>0))) 
			{
				List<Document> docs=SaveDocDelegate(true,SheetTP);
				if(docs.Count>0) {
					TPcur.DocNum=docs[0].DocNum;//attach first Doc to TP.
					TreatPlans.Update(TPcur); //update docnum. must be called after signature is updated.
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,TPcur.PatNum,"Sign TP");
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
