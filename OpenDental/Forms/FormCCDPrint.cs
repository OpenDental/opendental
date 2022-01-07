using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	public partial class FormCCDPrint:FormODBase {
		//Jordan - I approved ternary conditional operators for short phrase translations in this file.

		#region Fields - Private
		private PrintDocument _printDocument=null;
		///<summary>Total pages in the printed document.</summary>
		private int _totalPages;
		///<summary>Keeps track of the number of pages which have already been completely printed.</summary>
		private int _pagesPrinted;
		///<summary>Set to true to print page number in upper right-hand corner of the screen.</summary>
		private bool _doPrintPageNumbers;
		///<summary>Set to true when the document has not been renderd into the local container.</summary>
		private bool _isDirty=true;
		///<summary>English by default (represented by false). Set to true later if using French.</summary>
		private bool _isFrench=false;
		private string _formatVersionNumber;
		private bool _isAutoPrint;
		private int _copiesToPrint;
		private bool _hasPredetermination;
		private bool _doPrintPatientCopy=true;
		private Font _fontHeading=new Font(FontFamily.GenericMonospace,10,FontStyle.Bold);
		private Font _fontStandardUnderline=new Font(FontFamily.GenericMonospace,10,FontStyle.Underline);
		private Font _fontStandardSmall=new Font(FontFamily.GenericMonospace,8);
		private DocumentGenerator _documentGenerator=new DocumentGenerator();
		private string _transactionCode;
		///<summary>Used to represent a single line break (the maximum line hight for the standard font).</summary>
		private float _verticalLine;
		///<summary>Represents the maximum width of any character in our standard font.</summary>
		private float _maxCharWidth;
		///<summary>Contains the x-value for the center of the in-doc.bounds print page values.</summary>
		private float _center;
		///<summary>the x position where the current printing is happening.</summary>
		private float _x;
		///<summary>A value is assigned to text sometimes so that it can be measured or set to French. Not always needed.</summary>
		private string _text;
		///<summary>Used to render numbered bullet lists.</summary>
		private int _bullet=1;
		///<summary>Used to hold a single group of graphical primitives, so that they can be moved together in case they align with the bottom of a page.</summary>
		private Pen _penBreakLine=new Pen(Pens.Gray.Brush);
		///<summary>The parsed elements/fields of the etrans.MessageText string or embeded message.</summary>
		private CCDFieldInputter _ccdFieldInputterFormData;
		///<summary>Attained from field G42. Tells which form to print.</summary>
		private string _formId;
		private Etrans _etrans;
		private Patient _patient;
		private Patient _patientSubscriber;
		private Patient _patientSubscriber2;
		private Carrier _carrier;
		private Carrier _carrierOther;
		private Claim _claim;
		private Provider _providerTreat;
		private Provider _providerBill;
		private InsPlan _insplan;
		private InsPlan _insplan2;
		private InsSub _insSub;
		private InsSub _insSub2;
		private List<ClaimProc> _listClaimProcs;
		private List<Procedure> _listProcedureExtracted;
		private string _messageText;
		private List<PatPlan> _listPatPlansForPat;
		private PatPlan _patPlanPri;
		private PatPlan _patPlanSec;
		private Clinic _clinic;
		private string _responseStatus;
		#endregion Fields - Private

		#region Constructors and System Print Handlers

		///<summary>Called externally to display and/or print the messageText as a form.</summary>
		public FormCCDPrint(Etrans eTrans,string messageText,bool isPAutoPrint) {
			_etrans=eTrans;
			_messageText=messageText;
			_copiesToPrint=0;
			_isAutoPrint=isPAutoPrint;
			Init();
		}

		///<summary>Only called internally.</summary>
		protected FormCCDPrint(Etrans eTrans,string messageText,int pCopiesToPrint,bool isPAutoPrint,bool isPPatientCopy) {
			_etrans=eTrans;
			_messageText=messageText;
			_copiesToPrint=pCopiesToPrint;
			_isAutoPrint=isPAutoPrint;
			_doPrintPatientCopy=isPPatientCopy;
			Init();
		}

		///<summary>Simply calls the constructor. Useful when passing around as a delegate.</summary>
		public static void PrintCCD(Etrans eTrans,string messageText,bool isPAutoPrint) {
			new FormCCDPrint(eTrans,messageText,isPAutoPrint);
		}

		protected override void OnFormClosed(FormClosedEventArgs e) {
			if(_printDocument!=null) {
				_printDocument.Dispose();
			}
		}

		private void Init(){
			InitializeComponent();
			InitializeLayoutManager();
			_penBreakLine.Width=2;
			if(_etrans.PatNum!=0) { //Some transactions are not patient specific.
				_patient=Patients.GetPat(_etrans.PatNum);
				_listPatPlansForPat=PatPlans.Refresh(_etrans.PatNum);
				_claim=Claims.GetClaim(_etrans.ClaimNum);
				_carrier=Carriers.GetCarrier(_etrans.CarrierNum);
				if(_claim==null) {//for eligibility or when the claim was deleted by the customer before receiving the response from the mailbox.
					//Get primary info
					_insSub=InsSubs.GetSub(_etrans.InsSubNum,new List<InsSub>());
					_patientSubscriber=Patients.GetPat(_insSub.Subscriber);
					_insplan=InsPlans.GetPlan(_etrans.PlanNum,new List<InsPlan>());
					_patPlanPri=PatPlans.GetFromList(_listPatPlansForPat,_insSub.InsSubNum);
				}
				else {
					//Get primary info
					_insSub=InsSubs.GetSub(_claim.InsSubNum,new List<InsSub>());
					_patientSubscriber=Patients.GetPat(_insSub.Subscriber);
					_insplan=InsPlans.GetPlan(_claim.PlanNum,new List<InsPlan>());
					_patPlanPri=PatPlans.GetFromList(_listPatPlansForPat,_insSub.InsSubNum);
					//Get secondary info
					if(_claim.InsSubNum2!=0) {
						_patPlanSec=PatPlans.GetFromList(_listPatPlansForPat,_claim.InsSubNum2);
						_insSub2=InsSubs.GetSub(_claim.InsSubNum2,new List<InsSub>());
						_patientSubscriber2=Patients.GetPat(_insSub2.Subscriber);
						_insplan2=InsPlans.GetPlan(_claim.PlanNum2,new List<InsPlan>());
						_carrierOther=Carriers.GetCarrier(_insplan2.CarrierNum);
					}
					//Provider info
					_providerTreat=Providers.GetProv(_claim.ProvTreat);
					_providerBill=Providers.GetProv(_claim.ProvBill);
					//Claim related info
					_listClaimProcs=ClaimProcs.RefreshForClaim(_claim.ClaimNum);
					long clinicNum=0;
					for(int i=0;i<_listClaimProcs.Count;i++) {
						if(_listClaimProcs[i].ClinicNum!=0) {
							clinicNum=_listClaimProcs[i].ClinicNum;
							break;
						}
					}
					if(clinicNum!=0) {
						_clinic=Clinics.GetClinic(clinicNum);
					}
					else if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0) {
						_clinic=Clinics.GetFirst();
					}
				}
				if(_providerTreat==null) {
					_providerTreat=Providers.GetProv(Patients.GetProvNum(_patient));
				}
				if(_providerBill==null) {
					_providerBill=Providers.GetProv(Patients.GetProvNum(_patient));
				}
				List<Procedure> listProcedures=Procedures.Refresh(_etrans.PatNum);
				_listProcedureExtracted=Procedures.GetCanadianExtractedTeeth(listProcedures);
			}
			if(_messageText==null || _messageText.Length<23) {
				MessageBox.Show(Lan.g(this,"CCD message format too short")+": "+_messageText);
				Close();
				return;
			}
			_ccdFieldInputterFormData=new CCDFieldInputter(_messageText);//Input the fields of the given message.
			CCDField ccdFieldLanguageOfInsured=_ccdFieldInputterFormData.GetFieldById("G27");
			if(ccdFieldLanguageOfInsured!=null) {
				if(ccdFieldLanguageOfInsured.valuestr=="F") {
					_isFrench=true;
				}
			}
			else if(_patientSubscriber!=null && _patientSubscriber.Language=="fr") {
				_isFrench=true;
			}
			_formatVersionNumber=_ccdFieldInputterFormData.GetFieldById("A03").valuestr;//Must always exist so no error checking here.
			_transactionCode=_ccdFieldInputterFormData.GetFieldById("A04").valuestr;//Must always exist so no error checking here.
			if(_formatVersionNumber=="04") {//FormId field does not exist in version 02 in any of the message texts.
				CCDField ccdFieldFormId=_ccdFieldInputterFormData.GetFieldById("G42");//Usually exists in version 04 response messages.
				//Only a few response transactions don't define field G42. So far, those are transactions 15 (Summary Reconciliation), 16 (Payment Reconciliation) and 24 (Email).
				//In these cases, we simply do not use the formId field later on in the display code.
				if(ccdFieldFormId!=null) {
					_formId=ccdFieldFormId.valuestr;
				}
			}
			else {//Version 02
				//Since there is no FormID field in version 02, we figure out what the formId should be based on the transaction type.
				if(_transactionCode=="10") {//Eligibility Response.
					_formId="08";//Eligibility Form
				}
				else if(_transactionCode=="11") {//Claim Response.
					_formId="03";//Claim Acknowledgement Form
				}
				else if(_transactionCode=="21") {//EOB
					_formId="01";//EOB Form
					CCDField ccdFieldG02=_ccdFieldInputterFormData.GetFieldById("G02");
					if(ccdFieldG02!=null && ccdFieldG02.valuestr=="Y") {
						_formId="04";//Employer Certified.
					}
				}
				else if(_transactionCode=="13") {//Response to Pre-Determination.
					_formId="06";//Pre-Determination Acknowledgement Form
				}
				else if(_transactionCode=="12") { //Reversal response
					//There is no standard form for a reversal response, but we print the reversal response later on based on the transactioncode so we don't need to do anything here.
				}
				else {
					MessageBox.Show(Lan.g(this,"Unhandled transactionCode")+" '"+_transactionCode+"' "+Lan.g(this,"for version 02 message."));
					Close();
					return;
				}
			}
			CCDField ccdFieldStatus=_ccdFieldInputterFormData.GetFieldById("G05");
			if(ccdFieldStatus!=null && ccdFieldStatus.valuestr!=null) {
				_responseStatus=ccdFieldStatus.valuestr.ToUpper();
			}
			_transactionCode=_ccdFieldInputterFormData.GetFieldById("A04").valuestr;
			_hasPredetermination=(_transactionCode=="23"||_transactionCode=="13");//Be sure to list all predetermination response types here!
			if(_copiesToPrint<=0) { //Show the form on screen if there are no copies to print.
				ShowDisplayMessages();
				CCDField ccdFieldPayTo=_ccdFieldInputterFormData.GetFieldById("F01");
				if(ccdFieldPayTo!=null) {
					bool doPaySubscriber=(ccdFieldPayTo.valuestr=="1");//same for version 02 and version 04
					//Typically, insurance companies in Canada prefer to pay the subscriber instead of the dentist.
					if(AssignmentOfBenefits()) {//The insurance plan is set to pay the dentist
						if(doPaySubscriber) {//The carrier has decided to pay the subscriber.
							MsgBox.Show("Canadian","INFORMATION: The carrier changed the payee from the dentist to the subscriber.");//This check was required for certification.
						}
					}
					else {//The insurance plan is set to pay the subscriber
						if(!doPaySubscriber) {//The carrier has decided to pay the dentist.
							MsgBox.Show("Canadian","INFORMATION: The carrier changed the payee from the subscriber to the dentist.");//This check was required for certification.
						}
					}
				}
				CCDField ccdFieldPaymentAdjustmentAmount=_ccdFieldInputterFormData.GetFieldById("G33");
				if(ccdFieldPaymentAdjustmentAmount!=null) {
					if(ccdFieldPaymentAdjustmentAmount.valuestr.Substring(1)!="000000") {
						MessageBox.Show(Lan.g(this,"Payment adjustment amount")+": "+RawMoneyStrToDisplayMoney(ccdFieldPaymentAdjustmentAmount.valuestr));
					}
				}
				if(_isAutoPrint) {
					if(_responseStatus!="R") { //We are not required to automatically print rejection notices.
						//Automatically print a patient copy only. We are never required to autoprint a dentist copy, but it can be done manually instead.
						new FormCCDPrint(_etrans.Copy(),_messageText,1,false,true);
					}
				}
				if(_responseStatus!="R" && _formId=="05") { //Manual claim form
					FormClaimPrint.ShowCdaClaimForm(_claim);
					Close();
				}
				else {
					_printDocument=CreatePrintDocument();
					printPreviewControl1.Document=_printDocument;//Setting the document causes system to call pd_PrintPage, which will print the document in the preview window.
					ShowDialog();
				}
			}
			else {
				if(_responseStatus!="R" && _formId=="05") { //Manual claim form (CDA claim form)
					if(ODBuild.IsDebug()) {
						FormClaimPrint.ShowCdaClaimForm(_claim);//In debug mode, show the form on screen to save paper. Do not print to printer.
					}
					else {
						FormClaimPrint.PrintCdaClaimForm(_claim);//Send the print job to the physical printer.
					}
				}
				else { //All other Canadian forms
					if(ODBuild.IsDebug()) {
						new FormCCDPrint(_etrans.Copy(),_messageText,0,false,_doPrintPatientCopy);//In debug mode, show the form on screen to save paper. Do not print to printer.
					}
					else {
						//Print to the printer in Release mode.
						string strAuditDesc="";
						switch(_formId) {
							default:
								strAuditDesc="Default form printed";
								break;
							case "01"://CDA EOB Form
								strAuditDesc="EOB form printed";
								break;
							case "02"://Dentaide Form
								strAuditDesc="Dentaide form printed";
								break;
							case "03"://Claim Acknowledgement Form
								strAuditDesc="Claim acknowledgement form printed";
								break;
							case "04"://Employer Certified Form
								strAuditDesc="Employer certified form printed";
								break;
							case "05"://Plan Paper Claim Form (CDA form)
								//Printed in an earlier step. This line should never be hit.
								strAuditDesc="CDA claim form printed";
								break;
							case "06"://Predetermination Acknowledgement Form
								strAuditDesc="Predetermination acknowledgement form printed";
								break;
							case "07"://Predetermination EOB Form
								strAuditDesc="Predetermination EOB form printed";
								break;
							case "08"://Eligibility Form
								strAuditDesc="Eligibility form printed";
								break;
						}
						//Tries to print to the printer chosen by the user in File | Printers | Claim.
						PrinterL.TryPrint(pd_PrintPage,strAuditDesc,_etrans.PatNum,PrintSituation.Claim,new Margins(50,50,50,50)/*Half-inch all around*/,
							duplex: Duplex.Horizontal/*Print double sided when possible, since forms are usually 1-2 pages.*/);
					}
				}
				//Print the remaining copies recursively.
				if(_copiesToPrint>=2) {
					new FormCCDPrint(_etrans.Copy(),_messageText,_copiesToPrint-1,false,_doPrintPatientCopy);
				}
			}
			CCDField ccdFieldEmbeddedTransaction=_ccdFieldInputterFormData.GetFieldById("G40");
			if(ccdFieldEmbeddedTransaction!=null) {
				new FormCCDPrint(_etrans.Copy(),ccdFieldEmbeddedTransaction.valuestr,_copiesToPrint,_isAutoPrint,_doPrintPatientCopy);
			}
		}

		private PrintDocument CreatePrintDocument() {
			//have any signatures on the same piece of paper as the rest of the info.
			PrintDocument printDocument=new PrintDocument();
			printDocument.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
			printDocument.DefaultPageSettings.Margins=new Margins(50,50,50,50);//Half-inch all around.
			//This prevents a bug caused by some printer drivers not reporting their papersize.
			//But remember that other countries use A4 paper instead of 8 1/2 x 11.
			if(printDocument.DefaultPageSettings.PrintableArea.Height==0) {
				printDocument.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			printDocument.PrinterSettings.Duplex=Duplex.Horizontal;//Print double sided when possible, since forms are usually 1-2 pages.
			return printDocument;
		}

		private void FormCCDPrint_Resize(object sender,EventArgs e) {
			LayoutManager.MoveLocation(labelPage,new Point((Width-butPrint.Width-labelPage.Width)/2,labelPage.Location.Y));
			LayoutManager.MoveLocation(butBack,new Point(labelPage.Left-butBack.Width-6,butBack.Location.Y));
			LayoutManager.MoveLocation(butForward,new Point(labelPage.Right+6,butForward.Location.Y));
		}
		
		private void butOverride_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {
				return;
			}
			//The following 2 checks mimic logic found in Canadian.EOBImportHelper(...)
			if(!ListTools.In(_transactionCode,"21","23")) {
				MsgBox.Show(this,"You can only write claim amounts with either EOBs or Predetermination EOBs.");
				return;
			}
			if(_listClaimProcs.Exists(x => ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapComplete) || x.ClaimPaymentNum!=0)) {
				MsgBox.Show(this,"Cannot import because the claim has previously been recieved or is attached to a payment. Set the claim not recieved and try again.");
				return;
			}
			if(!_ccdFieldInputterFormData.HasValidPaymentLines()) {
				MsgBox.Show(this,"Cannot import because the number of procedures reported in the response does not match the total count of procedures listed in the response.");
				return;
			}
			if(_claim==null) {//When claim is null then claimprocs is also null.  See Init() for when claim can be null.
				MsgBox.Show(this,"The claim associated to this form could not be found.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Overwrite amounts on the claim with values from this form?\r\nCannot be reversed except manually.")) {
				return;
			}
			List<Procedure> listProcedures=Procedures.Refresh(_patient.PatNum);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			EraBehaviors eraBehaviors;
			if(Clearinghouses.GetClearinghouse(_etrans.ClearingHouseNum)!=null) {
				eraBehaviors=Clearinghouses.GetClearinghouse(_etrans.ClearingHouseNum).IsEraDownloadAllowed;//use etrans clearinghouse, 
			}
			else if(Clearinghouses.GetDefaultDental()!=null){
				eraBehaviors=Clearinghouses.GetDefaultDental().IsEraDownloadAllowed;//ifnull use HQ default clearinghouse
			}
			else {//Something wrong with the database.
				MsgBox.Show(this,"No default clearinghouse set.");
				return;
			}
			if(eraBehaviors==EraBehaviors.None) {
				eraBehaviors=EraBehaviors.DownloadDoNotReceive;
			}
			Canadian.EOBImportHelper(_ccdFieldInputterFormData,_listClaimProcs,listProcedures,listClaimProcs,_claim,false,FormClaimEdit.ShowProviderTransferWindow,eraBehaviors,_patient);
			SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,_claim.PatNum
				,"Claim for service date "+POut.Date(_claim.DateService)+" amounts overwritten manually using recieved EOB amounts.");
			MsgBox.Show(this,"Done");
		}

		private void butBack_Click(object sender,EventArgs e) {
			if(printPreviewControl1.StartPage==0)
				return;
			printPreviewControl1.StartPage--;
			labelPage.Text=(printPreviewControl1.StartPage+1).ToString()+" / "+_totalPages.ToString();
		}

		private void butFwd_Click(object sender,EventArgs e) {
			if(printPreviewControl1.StartPage==_totalPages-1)
				return;
			printPreviewControl1.StartPage++;
			labelPage.Text=(printPreviewControl1.StartPage+1).ToString()+" / "+_totalPages.ToString();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			new FormCCDPrint(_etrans.Copy(),_messageText,1,false,true);
		}

		private void butPrintDentist_Click(object sender,EventArgs e) {
			new FormCCDPrint(_etrans.Copy(),_messageText,1,false,false);
		}

		public void ShowDisplayMessages(){
			StringBuilder stringBuilderMessage=new StringBuilder();
			CCDField[] ccdFieldArrayDisplayMessage=_ccdFieldInputterFormData.GetFieldsById("G32");
			for(int i=0;i<ccdFieldArrayDisplayMessage.Length;i++){
				if(stringBuilderMessage.Length>0){
					stringBuilderMessage.Append(Environment.NewLine);
				}
				stringBuilderMessage.Append(ccdFieldArrayDisplayMessage[i].valuestr);
			}
			CCDField[] ccdFieldArrayNoteOutputFlags=_ccdFieldInputterFormData.GetFieldsById("G41");
			CCDField[] ccdFieldArrayNoteNumbers=_ccdFieldInputterFormData.GetFieldsById("G45");
			CCDField[] ccdFieldArrayNoteTexts=_ccdFieldInputterFormData.GetFieldsById("G26");
			List<string> listStringDisplayMessages=new List<string>();
			List<int> listDisplayMessageNumbers=new List<int>();
			for(int i=0;i<ccdFieldArrayNoteOutputFlags.Length;i++) {
				//We display notes on screen only if they are marked with output flag 1 (display notes on screen). Output flag 0 (prompt) is ignored here because such notes are printed on the physical printout.
				if(PIn.Int(ccdFieldArrayNoteOutputFlags[i].valuestr)!=1) { 
					continue;
				}
				listStringDisplayMessages.Add(ccdFieldArrayNoteTexts[i].valuestr);
				if(i<ccdFieldArrayNoteNumbers.Length) {
					listDisplayMessageNumbers.Add(PIn.Int(ccdFieldArrayNoteNumbers[i].valuestr));
				}
				else {
					listDisplayMessageNumbers.Add(i+1);
				}
			}
			while(listStringDisplayMessages.Count>0) {
				int indexOfMinVal=0;
				for(int j=1;j<listDisplayMessageNumbers.Count;j++) {
					if(listDisplayMessageNumbers[j]<listDisplayMessageNumbers[indexOfMinVal]) {
						indexOfMinVal=j;
					}
				}
				_documentGenerator.StartElement();
				if(stringBuilderMessage.Length>0) {
					stringBuilderMessage.Append(Environment.NewLine);
				}
				stringBuilderMessage.Append(listStringDisplayMessages[indexOfMinVal]);
				listStringDisplayMessages.RemoveAt(indexOfMinVal);
				listDisplayMessageNumbers.RemoveAt(indexOfMinVal);
			}
			string msg=stringBuilderMessage.ToString();
			if(msg.Length>0){
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
				msgBoxCopyPaste.Text=Lan.g(this,"Information from Insurance Carrier");
				msgBoxCopyPaste.ShowDialog();
			}
		}

		///<summary>Called for each page to be printed.</summary>
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e){
			for(byte i=0;i<255;i++){
				SizeF sizeF=e.Graphics.MeasureString(""+Canadian.GetCanadianChar(i),_documentGenerator.standardFont);
				_verticalLine=Math.Max(_verticalLine,(float)Math.Ceiling(sizeF.Height));
				_maxCharWidth=Math.Max(_maxCharWidth,(float)Math.Ceiling(sizeF.Width));
			}
			if(_isDirty){//Only render the document containers the first time through.
				_isDirty=false;
				_documentGenerator.bounds=e.MarginBounds;
				_center=_documentGenerator.bounds.X+_documentGenerator.bounds.Width/2;
				_x=_documentGenerator.StartElement();//Every printed page always starts on the first row and can choose to skip rows later if desired.
				try {
					if(_responseStatus=="R") {
						PrintRejection(e.Graphics);
					}
					else if(_transactionCode=="12") {
						PrintReversalResponse_12(e.Graphics);
					}
					else if(_transactionCode=="15") {
						PrintSummaryReconciliation_15(e.Graphics);
					}
					else if(_transactionCode=="16") {
						PrintPaymentReconciliation_16(e.Graphics);
					}
					else if(_transactionCode=="24") {
						PrintEmail_24(e.Graphics);
					}
					else {
						switch(_formId) {
							default:
								DefaultPrint(e.Graphics);
								break;
							case null:
								MsgBox.Show(this,"Missing form ID in response.\r\n"
									+"This usually indicates an issue with the claim sent,\r\n"
									+"such as an incorrect carrier ID.");
								Close();//Since we did not print anything, the form is blank.
								break;
							case "01"://CDA EOB Form
								PrintEOB(e.Graphics);
								break;
							case "02"://Dentaide Form
								PrintDentaide(e.Graphics);
								break;
							case "03"://Claim Acknowledgement Form
								PrintClaimAck(e.Graphics);
								break;
							case "04"://Employer Certified Form
								PrintEmployerCertified(e.Graphics);
								break;
							case "05"://Plan Paper Claim Form
								//Printed in an earlier step. This line should never be hit.
								break;
							case "06"://Predetermination Acknowledgement Form
								PrintPredeterminationAck(e.Graphics);
								break;
							case "07"://Predetermination EOB Form
								PrintEOB(e.Graphics);
								break;
							case "08"://Eligibility Form
								PrintEligibility(e.Graphics);
								break;
						}
					}
					_x=_documentGenerator.StartElement();//Be sure to end last element always.
					_totalPages=_documentGenerator.CalcTotalPages(e.Graphics);
				}
				catch {
					//Printing will fail if the user switched to Open Dental from another software system and Open Dental gets a response from ITRANS with 
					//regards to a claim from their old system. In this situation we just want to show what information we have because we will not be able 
					//to look up the old claim necessarily. This is also a more elegant way to show other printing errors than allowing an unhandled exception. 
					DefaultPrint(e.Graphics);
				}
			}
			e.Graphics.DrawRectangle(Pens.LightGray,e.MarginBounds);//Draw light border for context.
			_pagesPrinted++;
			_documentGenerator.PrintPage(e.Graphics,_pagesPrinted);
			if(_doPrintPageNumbers){
				_text="Page "+_pagesPrinted.ToString()+(_isFrench?" de ":" of ")+_totalPages;
				e.Graphics.DrawString(_text,_documentGenerator.standardFont,Pens.Black.Brush,
					e.MarginBounds.Right-e.Graphics.MeasureString(_text,_documentGenerator.standardFont).Width-4,e.MarginBounds.Top);
			}
			if(_pagesPrinted<_totalPages){					
				e.HasMorePages=true;
			}
			else{
				e.HasMorePages=false;
				labelPage.Text=(printPreviewControl1.StartPage+1).ToString()+" / "+_totalPages.ToString();
			}
		}

		#endregion

		#region Individual Form Printers

		private void PrintRejection(Graphics g) {
			PrintClaimAck(g);//The rejection form is almost exactly the same as the claim ack so the same function is used, at least for now.
		}

		private void PrintEmail_24(Graphics g){
			if(_isFrench) {
				_text="RÉPONSE PAR COURRIER ÉLECTRONIQUE";
			}
			else {
				_text="E-MAIL";
			}		
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement(_verticalLine);
			PrintTransactionDate(g,_x,0);
			PrintTreatmentProviderOfficeNumber(g,_x+250,0);
			CCDField ccdField=_ccdFieldInputterFormData.GetFieldById("G54");
			_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x+500,0);//REFERENCE
			_x=_documentGenerator.StartElement(_verticalLine);
			ccdField=_ccdFieldInputterFormData.GetFieldById("G49");
			_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);//TO
			_x=_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G50");
			_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);//FROM
			_x=_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G51");
			_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);//SUBJECT
			_x=_documentGenerator.StartElement(_verticalLine);
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"LIGNE":"LINE",_x,0);
			_text="MESSAGE";
			float lineCol=_x+55;
			_documentGenerator.DrawString(g,_text,lineCol,0);
			_x=_documentGenerator.StartElement(_verticalLine);
			CCDField[] ccdFieldArrayNoteLines=_ccdFieldInputterFormData.GetFieldsById("G53");//BODY OF MESSAGE
			if(ccdFieldArrayNoteLines!=null){
				for(int i=0;i<ccdFieldArrayNoteLines.Length;i++){
					_x=_documentGenerator.StartElement();
					_documentGenerator.DrawString(g,(i+1).ToString().PadLeft(2,'0'),_x,0);
					if(ccdFieldArrayNoteLines[i]!=null && ccdFieldArrayNoteLines[i].valuestr!=null) {
						_documentGenerator.DrawString(g,ccdFieldArrayNoteLines[i].valuestr,lineCol,0,_documentGenerator.standardFont);
					}
				}
			}
		}

		private void PrintSummaryReconciliation_15(Graphics g){
			if(_isFrench) {
				_text="RÉSUMÉ DE RÉCONCILIATION";
			}
			else {
				_text="SUMMARY RECONCILIATION";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_documentGenerator.StartElement(_verticalLine);
			PrintOfficeSequenceNumber(g,_x,0);
			_documentGenerator.StartElement();
			_documentGenerator.StartElement();
			PrintTransactionReferenceNumber(g,_x,0);
			_documentGenerator.StartElement();
			CCDField ccdField=_ccdFieldInputterFormData.GetFieldById("G34");//Payment reference
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G35");//Payment date
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),DateNumToPrintDate(ccdField.valuestr),true,_x,0);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G36");//Payment amount
			if(ccdField!=null && ccdField.valuestr!=null) {
				_documentGenerator.DrawString(g,ccdField.GetFieldName(_isFrench)+": "+RawMoneyStrToDisplayMoney(ccdField.valuestr),_x,0,_fontHeading);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G33");//Payment adjustment amount
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),RawMoneyStrToDisplayMoney(ccdField.valuestr),true,_x,0);
			}
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			CCDField[] ccdFieldArrayCdaProviderNumbers=_ccdFieldInputterFormData.GetFieldsById("B01");
			CCDField[] ccdFieldArrayCarrierIdentificationNumbers=_ccdFieldInputterFormData.GetFieldsById("A05");
			CCDField[] ccdFieldArrayOfficeSequenceNumbers=_ccdFieldInputterFormData.GetFieldsById("A02");
			CCDField[] ccdFieldArrayTransactionReferenceNumbers=_ccdFieldInputterFormData.GetFieldsById("G01");
			CCDField[] ccdFieldArrayTransactionPayments=_ccdFieldInputterFormData.GetFieldsById("G38");
			float cdaProviderNumCol=_x;
			float cdaProviderNumColWidth=75;
			float carrierIdentificationNumCol=cdaProviderNumCol+cdaProviderNumColWidth;
			float carrierIdentificationNumColWidth=125;
			float officeSequenceNumCol=carrierIdentificationNumCol+carrierIdentificationNumColWidth;
			float officeSequenceNumColWidth=140;
			float transactionReferenceNumCol=officeSequenceNumCol+officeSequenceNumColWidth;
			float transactionReferenceNumColWidth=125;
			float transactionPaymentCol=transactionReferenceNumCol+transactionReferenceNumColWidth;
			Font font=_documentGenerator.standardFont;
			_documentGenerator.standardFont=new Font(_fontStandardSmall.FontFamily,9,FontStyle.Bold);
			if(_isFrench) {
				_documentGenerator.DrawString(g,"NO DU\nDENTISTE",cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,"IDENTIFICATION\nDE PORTEUR",carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,"NO DE TRANSACTION\nDU CABINET",officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,"NO DE RÉFÉRENCE\nDE TRANSACTION",transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,"PAIEMENT DE\nTRANSACTION",transactionPaymentCol,0);
			}
			else {
				_documentGenerator.DrawString(g,"UNIQUE\nID NO",cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,"CARRIER\nIDENTIFICATION",carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,"DENTAL OFFICE\nCLAIM REFERENCE",officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,"CARRIER CLAIM\nNUMBER",transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,"TRANSACTION\nPAYMENT",transactionPaymentCol,0);
			}
			_documentGenerator.standardFont=_fontStandardSmall;
			for(int i=0;i<ccdFieldArrayCdaProviderNumbers.Length;i++) {
				_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,ccdFieldArrayCdaProviderNumbers[i].valuestr,cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayCarrierIdentificationNumbers[i].valuestr,carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayOfficeSequenceNumbers[i+1].valuestr,officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayTransactionReferenceNumbers[i+1].valuestr,transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,RawMoneyStrToDisplayMoney(ccdFieldArrayTransactionPayments[i].valuestr),transactionPaymentCol,0);
			}
			_documentGenerator.standardFont=font;
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			PrintNoteList(g);
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			PrintErrorList(g);
		}
		private void PrintPaymentReconciliation_16(Graphics g){
			if(_isFrench) {
				_text="RÉCONCILIATION DE PAIEMENT";
			}
			else {
				_text="PAYMENT RECONCILIATION";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_documentGenerator.StartElement(_verticalLine);
			PrintOfficeSequenceNumber(g,_x,0);
			_documentGenerator.StartElement();
			CCDField ccdField=_ccdFieldInputterFormData.GetFieldById("B04");
			if(ccdField!=null){
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);
			}
			_documentGenerator.StartElement();
			PrintTransactionReferenceNumber(g,_x,0);
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G34");//Payment reference
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x,0);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G35");//Payment date
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),DateNumToPrintDate(ccdField.valuestr),true,_x,0);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G36");//Payment amount
			if(ccdField!=null && ccdField.valuestr!=null) {
				_documentGenerator.DrawString(g,ccdField.GetFieldName(_isFrench)+": "+RawMoneyStrToDisplayMoney(ccdField.valuestr),_x,0,_fontHeading);
			}
			_documentGenerator.StartElement();
			ccdField=_ccdFieldInputterFormData.GetFieldById("G33");//Payment adjustment amount
			if(ccdField!=null) {
				_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),RawMoneyStrToDisplayMoney(ccdField.valuestr),true,_x,0);
			}
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			CCDField[] ccdFieldArraycdaProviderNumbers=_ccdFieldInputterFormData.GetFieldsById("B01");
			CCDField[] ccdFieldArrayProviderOfficeNumbers=_ccdFieldInputterFormData.GetFieldsById("B02");
			CCDField[] ccdFieldArrayBillingProviderNumbers=_ccdFieldInputterFormData.GetFieldsById("B03");
			CCDField[] ccdFieldArrayCarrierIdentificationNumbers=_ccdFieldInputterFormData.GetFieldsById("A05");
			CCDField[] ccdFieldArrayOfficeSequenceNumbers=_ccdFieldInputterFormData.GetFieldsById("A02");
			CCDField[] ccdFieldArrayTransactionReferenceNumbers=_ccdFieldInputterFormData.GetFieldsById("G01");
			CCDField[] ccdFieldArrayTransactionPayments=_ccdFieldInputterFormData.GetFieldsById("G38");
			float cdaProviderNumCol=_x;
			float cdaProviderNumColWidth=75;
			float providerOfficeNumCol=cdaProviderNumCol+cdaProviderNumColWidth;
			float providerOfficeNumColWidth=75;
			float billingProviderNumCol=providerOfficeNumCol+providerOfficeNumColWidth;
			float billingProviderNumColWidth=120;
			float carrierIdentificationNumCol=billingProviderNumCol+billingProviderNumColWidth;
			float carrierIdentificationNumColWidth=125;
			float officeSequenceNumCol=carrierIdentificationNumCol+carrierIdentificationNumColWidth;
			float officeSequenceNumColWidth=140;
			float transactionReferenceNumCol=officeSequenceNumCol+officeSequenceNumColWidth;
			float transactionReferenceNumColWidth=125;
			float transactionPaymentCol=transactionReferenceNumCol+transactionReferenceNumColWidth;
			Font font=_documentGenerator.standardFont;
			_documentGenerator.standardFont=new Font(_fontStandardSmall.FontFamily,9,FontStyle.Bold);
			if(_isFrench) {
				_documentGenerator.DrawString(g,"NO DU\nDENTISTE",cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,"NOMBRE\nD'OFFICE",providerOfficeNumCol,0);
				_documentGenerator.DrawString(g,"FOURNISSEUR\nDE FACTURATION",billingProviderNumCol,0);
				_documentGenerator.DrawString(g,"IDENTIFICATION\nDE PORTEUR",carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,"NO DE TRANSACTION\nDU CABINET",officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,"NO DE RÉFÉRENCE\nDE TRANSACTION",transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,"PAIEMENT DE\nTRANSACTION",transactionPaymentCol,0);
			}
			else {
				_documentGenerator.DrawString(g,"UNIQUE\nID NO",cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,"OFFICE\nNUMBER",providerOfficeNumCol,0);
				_documentGenerator.DrawString(g,"BILLING\nPROVIDER",billingProviderNumCol,0);
				_documentGenerator.DrawString(g,"CARRIER\nIDENTIFICATION",carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,"DENTAL OFFICE\nCLAIM REFERENCE",officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,"CARRIER CLAIM\nNUMBER",transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,"TRANSACTION\nPAYMENT",transactionPaymentCol,0);
			}
			_documentGenerator.standardFont=_fontStandardSmall;
			for(int i=0;i<ccdFieldArraycdaProviderNumbers.Length;i++){
				_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,ccdFieldArraycdaProviderNumbers[i].valuestr,cdaProviderNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayProviderOfficeNumbers[i].valuestr,providerOfficeNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayBillingProviderNumbers[i].valuestr,billingProviderNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayCarrierIdentificationNumbers[i].valuestr,carrierIdentificationNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayOfficeSequenceNumbers[i+1].valuestr,officeSequenceNumCol,0);
				_documentGenerator.DrawString(g,ccdFieldArrayTransactionReferenceNumbers[i+1].valuestr,transactionReferenceNumCol,0);
				_documentGenerator.DrawString(g,RawMoneyStrToDisplayMoney(ccdFieldArrayTransactionPayments[i].valuestr),transactionPaymentCol,0);
			}
			_documentGenerator.standardFont=font;
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			PrintNoteList(g);
			_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_documentGenerator.StartElement();
			PrintErrorList(g);
		}

		private void PrintEligibility(Graphics g){
			PrintCarrier(g);
			_x=_documentGenerator.StartElement();
			if(_doPrintPatientCopy) {
				if(_isFrench) {
					_text="ACCUSÉ DE RÉCEPTION D'UNE DEMANDE D'ÉLIGIBILITÉ - COPIE DU PATIENT";
				}
				else {
					_text="ELIGIBILITY RESPONSE - PATIENT COPY";
				}
			}
			else {
				if(_isFrench) {
					_text="ACCUSÉ DE RÉCEPTION D'UNE DEMANDE D'ÉLIGIBILITÉ - COPIE DU DENTISTE";
				}
				else {
					_text="ELIGIBILITY RESPONSE - DENTIST COPY";
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="Nous avons utilisé les renseignements du présent formulaire pour traiter votre demande par ordinateur. Veuillez en vérifier l'exactitude et aviser votre dentiste en cas d'erreur. Prière de ne pas poster à l'assureur/administrateur du régime.";
			}
			else {
				_text="The information contained on this form has been used to process your claim electronically. Please verify the accuracy of this data and report any discrepancies to your dental office. Do not mail this form to the insurer/plan administrator.";
			}
			PrintClaimAckBody(g,_text);
			if(_isFrench) {
				_text="La présente demande de prestations a été transmise par ordinateur.".ToUpper();
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Elle sert de reçu seulement.".ToUpper();
			}
			else {
				_text="THIS CLAIM HAS BEEN SUBMITTED ELECTRONICALLY - THIS IS A RECEIPT ONLY";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
		}

		///<summary>For printing basic information about unknown/unsupported message formats (for debugging, etc.).</summary>
		private void DefaultPrint(Graphics g) {
			_x=_documentGenerator.StartElement(_verticalLine);
			if(_isFrench) {
				_text="DONNÉES BRUTES POUR UNE RÉPONSE INVALIDE OU INCONNU";
			}
			else {
				_text="RAW DATA FOR INVALID OR UNKNOWN RESPONSE";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement(_verticalLine);
			CCDField[] ccdFieldArrayLoadedFields=_ccdFieldInputterFormData.GetLoadedFields();
			if(ccdFieldArrayLoadedFields!=null){
				for(int i=0;i<ccdFieldArrayLoadedFields.Length;i++){
					if(ccdFieldArrayLoadedFields[i]!=null){
						_x=_documentGenerator.StartElement();
						if(ccdFieldArrayLoadedFields[i].fieldId!=null && ccdFieldArrayLoadedFields[i].fieldId.Length>0){
							_text=ccdFieldArrayLoadedFields[i].fieldId;
							_documentGenerator.DrawString(g,_text,_x,0);
						}
						CCDField ccdField=ccdFieldArrayLoadedFields[i];
						_documentGenerator.DrawField(g,ccdField.GetFieldName(_isFrench),ccdField.valuestr,true,_x+30,0);
					}
				}
			}
		}

		private void PrintDentaide(Graphics g){
			_documentGenerator.standardFont=_fontStandardSmall;
			_doPrintPageNumbers=true;
			int headerHeight=(int)_verticalLine;
			_documentGenerator.bounds=new Rectangle(_documentGenerator.bounds.X,_documentGenerator.bounds.Y+headerHeight,_documentGenerator.bounds.Width,
				_documentGenerator.bounds.Height-headerHeight);//Reset the doc.bounds so that the page numbers are on a row alone.
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="FORMULAIRE DENTAIDE";
			}
			else {
				_text="DENTAIDE FORM";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			_text=DateToString(_etrans.DateTimeTrans);
			SizeF sizeF1;
			if(_isFrench) {
				sizeF1=_documentGenerator.DrawString(g,"DATE: ",_x,0);
			}
			else {
				sizeF1=_documentGenerator.DrawString(g,"DATE SUBMITTED: ",_x,0);
			}
			_documentGenerator.DrawString(g,_text,_x+sizeF1.Width,0);
			_text="";
			CCDField ccdFieldG01=_ccdFieldInputterFormData.GetFieldById("G01");
			if(ccdFieldG01!=null) {
				_text=ccdFieldG01.valuestr;
			}
			float rightMidCol=400.0f;
			if(_isFrench) {
				_documentGenerator.DrawField(g,"NO DE TRANSACTION DE DENTAIDE",_text,true,rightMidCol,0);
			}
			else {
				_documentGenerator.DrawField(g,"DENTAIDE TRANSACTION NO",_text,true,rightMidCol,0);
			}
			_x=_documentGenerator.StartElement();
			string fieldText;
			if(_isFrench) {
				fieldText="NO DU PLAN DE TRAITEMENT";
			}
			else {
				fieldText="PREDETERMINATION NO";
			}
			if(_transactionCode=="13") {//predetermination ACK
				_text="";//We are not supposed to show a predetermination number for this transaction type.
			}
			else if(_transactionCode=="23") {//predetermination EOB
				//The predetermination number is the same as the dentaide number. Don't change the text value.
			}
			else if(_claim!=null) { //Claim ACK or EOB
				//Retreive the predetermination number for this claim from the transaction history.
				_text="";//There may not be a predetermination, so we first clear the text.
				List<Etrans> listEtrans=Etranss.GetAllForOneClaim(_claim.ClaimNum);
				Etrans eTransPredetermEob=null;
				for(int i=0;i<listEtrans.Count;i++) {//Etrans entries are chronological, so we find the most recent one starting at the end of the list.
					if(listEtrans[i].Etype==EtransType.PredetermEOB_CA) {//Predeterm EOBs only not Predeterm Acks, because only EOBs have been truely processed by the carrier.
						if(eTransPredetermEob==null || listEtrans[i].DateTimeTrans>eTransPredetermEob.DateTimeTrans) {
							eTransPredetermEob=listEtrans[i];
						}
					}
				}
				if(eTransPredetermEob!=null) {
					string predetermResponseMessage=EtransMessageTexts.GetMessageText(eTransPredetermEob.EtransMessageTextNum,false);
					CCDFieldInputter ccdFieldInputterPredetermResponseFields=new CCDFieldInputter(predetermResponseMessage);
					CCDField predetermFieldG01=ccdFieldInputterPredetermResponseFields.GetFieldById("G01");
					if(predetermFieldG01!=null) {
						_text=predetermFieldG01.valuestr;//finally retreive the predetermination number.
					}
				}
			}
			else { //For some reason in the tests they want us to print elegibilities on the Dentaide form. In this case there is not predetermination number.
				_text="";
			}
			_documentGenerator.DrawField(g,fieldText,_text,false,_x,0);
			PrintTreatmentProviderID(g,rightMidCol,0);
			_x=_documentGenerator.StartElement();
			PrintDentistName(g,_x,0);
			PrintTreatmentProviderOfficeNumber(g,rightMidCol,0);
			_x=_documentGenerator.StartElement();
			PrintDentistAddress(g,_x,0,0);
			sizeF1=PrintDentistPhone(g,rightMidCol,0);
			//Dependant NO. should be same for both primary and secondary insurance, because it is the patient account number from within Open Dental.
			SizeF sizeF2=PrintPrimaryDependantNo(g,rightMidCol,sizeF1.Height,"PATIENT'S OFFICE ACCOUNT NO","NO DE DOSSIER DU PATIENT");
			SizeF sizeF3=PrintOfficeSequenceNumber(g,rightMidCol,sizeF1.Height+sizeF2.Height);
			PrintPatientBirthday(g,rightMidCol,sizeF1.Height+sizeF2.Height+sizeF3.Height);
			_x=_documentGenerator.StartElement();
			PrintPatientName(g,_x,0);			
			PrintPatientSex(g,rightMidCol,0);
			_x=_documentGenerator.StartElement();
			PrintComment(g,_x,0);
			_x=_documentGenerator.StartElement();
			float leftMidCol=290f;
			if(_isFrench) {
				_text="RENSEIGNEMENTS SUR L'ASSURANCE";
			}
			else {
				_text="INSURANCE INFORMATION";
			}
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Underline);
			sizeF1=g.MeasureString(_text,_documentGenerator.standardFont);
			_documentGenerator.DrawString(g,_text,_x+(leftMidCol-_x-sizeF1.Width)/2,0);
			if(_isFrench) {
				_text="PREMIER ASSUREUR";
			}
			else {
				_text="PRIMARY COVERAGE";
			}
			sizeF1=g.MeasureString(_text,_documentGenerator.standardFont);
			float rightCol=leftMidCol+260f;
			_documentGenerator.DrawString(g,_text,leftMidCol+(rightCol-leftMidCol-sizeF1.Width)/2,0);
			if(_isFrench) {
				_text="SECOND ASSUREUR";
			}
			else {
				_text="SECONDARY COVERAGE";
			}
			sizeF1=g.MeasureString(_text,_documentGenerator.standardFont);
			_documentGenerator.DrawString(g,_text,rightCol+(_documentGenerator.bounds.Right-rightCol-sizeF1.Width)/2,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Regular);
			if(_isFrench) {
				_text="ASSUREUR/ADMINIST. RÉGIME:";
			}
			else {
				_text="CARRIER/PLAN ADMINISTRATOR:";
			}
			_documentGenerator.DrawString(g,_text,_x,0);
			_text=_carrier.CarrierName.ToUpper();//Field A05
			_documentGenerator.DrawString(g,_text,leftMidCol,0);
			if(_claim!=null && _insplan2!=null) {
				_text=_carrierOther.CarrierName.ToUpper();
				_documentGenerator.DrawString(g,_text,rightCol,0);
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="NO DE POLICE:";
			}
			else {
				_text="POLICY#:";
			}
			_documentGenerator.DrawString(g,_text,_x,0);
			_text=_insplan.GroupNum;//Field C01
			_documentGenerator.DrawString(g,_text,leftMidCol,0);
			if(_isFrench) {
				_text="DIV/SECTION:";
			}
			else {
				_text="DIV/SECTION NO:";
			}
			_documentGenerator.DrawString(g,_insplan.DivisionNo,leftMidCol+190,0);
			if(_claim!=null && _insplan2!=null) {
				_text=_insplan2.GroupNum;//Field E02
				_documentGenerator.DrawString(g,_text,rightCol,0);
			}
			if(_isFrench) {
				_documentGenerator.DrawString(g,"DIV/SECTION:",rightCol+86,0);			
			}
			else {
				_documentGenerator.DrawString(g,"DIV/SECTION NO:",rightCol+86,0);			
			}
			if(_claim!=null && _insplan2!=null) {
				_documentGenerator.DrawString(g,_insplan2.DivisionNo,rightCol+190,0);
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="TITULAIRE:";
			}
			else {
				_text="INSURED/MEMBER NAME:";
			}
			_documentGenerator.DrawString(g,_text,_x,0);
			_text=_patientSubscriber.GetNameFLFormal();
			_documentGenerator.DrawString(g,_text,leftMidCol,0);
			if(_claim!=null && _insplan2!=null) {
				_text=_patientSubscriber2.GetNameFLFormal();
				_documentGenerator.DrawString(g,_text,rightCol,0);
			}
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"ADRESSE:":"ADDRESS:",_x,0);
			PrintSubscriberAddress(g,leftMidCol,0,true,rightCol-leftMidCol-10f);
			PrintSubscriberAddress(g,rightCol,0,false,_documentGenerator.bounds.Right-rightCol-10f);
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"DATE DE NAISSANCE:":"BIRTHDATE:",_x,0);
			_documentGenerator.DrawString(g,DateToString(_patientSubscriber.Birthdate),leftMidCol,0);//Field D01
			if(_patientSubscriber2!=null){
				_documentGenerator.DrawString(g,DateToString(_patientSubscriber2.Birthdate),rightCol,0);//Field E04
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_documentGenerator.DrawString(g,"NO DU TITULAIRE/CERTIFICAT:",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"INSURANCE/CERTIFICATE NO:",_x,0);
			}
			_documentGenerator.DrawString(g,_insSub.SubscriberID,leftMidCol,0);//Fields D01 and D11
			_documentGenerator.DrawString(g,_isFrench?"- SÉQUENCE:":"- SEQUENCE:",leftMidCol+86,0);
			_documentGenerator.DrawString(g,_insplan.DentaideCardSequence.ToString().PadLeft(2,'0'),leftMidCol+162,0);
			if(_insplan2!=null){
				_documentGenerator.DrawString(g,_insSub2.SubscriberID,rightCol,0);//Fields E04 and E07
			}
			_documentGenerator.DrawString(g,_isFrench?"- SÉQUENCE:":"- SEQUENCE:",rightCol+86,0);
			if(_insplan2!=null){
				_documentGenerator.DrawString(g,_insplan2.DentaideCardSequence.ToString().PadLeft(2,'0'),rightCol+162,0);
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_documentGenerator.DrawString(g,"PARENTÉ AVEC PATIENT:",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"RELATIONSHIP TO PATIENT:",_x,0);
			}
			_text="";
			Relat relatPri=GetRelationshipToSubscriber();
			switch(Canadian.GetRelationshipCode(relatPri)){//Field C03
				case "1":
					_text=_isFrench?"Soi-même":"Self";
					break;
				case "2":
					_text=_isFrench?"Époux(se)":"Spouse";
					break;
				case "3":
					_text="Parent";//Same in French and English.
					break;
				case "4":
					_text=_isFrench?"Conjoint(e)":"Common Law Spouse";
					break;
				case "5":
					_text=_isFrench?"Autre":"Other";
					break;
				default:
					break;
			}
			_documentGenerator.DrawString(g,_text,leftMidCol,0);
			if(_claim!=null && _insplan2!=null) {
				_text="";
				switch(Canadian.GetRelationshipCode(_patPlanSec.Relationship)){//Field E06
					case "1":
						_text=_isFrench?"Époux(se)":"Spouse";
						break;
					case "2":
						_text=_isFrench?"Époux(se)":"Spouse";
						break;
					case "3":
						_text="Parent";//Same in French and English.
						break;
					//"4" does not apply.
					case "5":
						_text=_isFrench?"Autre":"Other";
						break;
					default:
						break;
				}
				_documentGenerator.DrawString(g,_text,rightCol,0);
			}
			//Spaces don't show up with underline, so we will have to underline manually.
			float underlineWidth=g.MeasureString("***",_documentGenerator.standardFont).Width;
			string isStudent="   ";
			string isHandicapped="   ";
			if(relatPri==Relat.Child){
				_text="";
				switch(_patient.CanadianEligibilityCode){//Field C09
					case 1://Patient is a full-time student.
						isStudent=_isFrench?"Oui":"Yes";
						_text=_patient.SchoolName;
						break;
					case 2://Patient is disabled.
						isHandicapped=_isFrench?"Oui":"Yes";
						break;
					case 3://Patient is a disabled student.
						isStudent=_isFrench?"Oui":"Yes";
						_text=_patient.SchoolName;
						isHandicapped=_isFrench?"Oui":"Yes";
						break;
					default://Not applicable
						break;
				}
			}
			else if(_patient.CanadianEligibilityCode==2) {
				isHandicapped=_isFrench?"Oui":"Yes";
			}
			else if(_patient.CanadianEligibilityCode==3) {
				isStudent=_isFrench?"Oui":"Yes";
				isHandicapped=_isFrench?"Oui":"Yes";
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_documentGenerator.DrawString(g,"PERSONNE À CHARGE: HANDICAPÉ     ÉTUDIANT À PLEIN TEMPS     ; NOM DE L'INSTITUTION:",_x,0);
				_documentGenerator.DrawString(g,isHandicapped,(_x+200),0);
				_documentGenerator.DrawString(g,isStudent,(_x+385),0);
			}
			else {
				_documentGenerator.DrawString(g,"IF DEPENDANT, INDICATE: HANDICAPPED     FULL-TIME STUDENT    ;NAME OF STUDENT'S SCHOOL:",_x,0);
				_documentGenerator.DrawString(g,isHandicapped,(_x+248),0);
				_documentGenerator.DrawString(g,isStudent,(_x+397),0);
			}
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_patient.SchoolName,_x,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Underline);
			if(_isFrench) {
				_documentGenerator.DrawString(g,"RENSEIGNEMENTS SUR LES SOINS:",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"TREATMENT INFORMATION:",_x,0);
			}
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Regular);
			_x=_documentGenerator.StartElement();
			_bullet=1;
			if(_isFrench) {
				PrintAccidentBullet(g,"Le traitement résulte-t-il d'un accident?");
			}
			else {
				PrintAccidentBullet(g,"Is treatment resulting from an accident?");
			}
			_x=_documentGenerator.StartElement();
			CCDField ccdFieldInitialPlacementUpper=_ccdFieldInputterFormData.GetFieldById("F15");
			string initialPlacementUpperStr="X";
			if(ccdFieldInitialPlacementUpper!=null) {
				initialPlacementUpperStr=ccdFieldInitialPlacementUpper.valuestr;
			}
			CCDField ccdFieldInitialPlacementLower=_ccdFieldInputterFormData.GetFieldById("F18");
			string initialPlacementLowerStr="X";
			if(ccdFieldInitialPlacementLower!=null) {
				initialPlacementLowerStr=ccdFieldInitialPlacementLower.valuestr;
			}			
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Begin indentation.
			string initialPlacementUpperReadable;
			string initialPlacementLowerReadable;
			if(_isFrench) {
				if(initialPlacementUpperStr=="X") {
					initialPlacementUpperReadable=" ";
				}
				else {
					if(initialPlacementUpperStr=="N") {
						initialPlacementUpperReadable="Non";
					}
					else{
						initialPlacementUpperReadable="Oui";
					}
				}
				if(initialPlacementLowerStr=="X") {
					initialPlacementLowerReadable=" ";
				}
				else {
					if(initialPlacementLowerStr=="N") {
						initialPlacementLowerReadable="Non";
					}
					else{
						initialPlacementLowerReadable="Oui";
					}
				}
			}
			else {
				if(initialPlacementUpperStr=="X") {
					initialPlacementUpperReadable=" ";
				}
				else {
					if(initialPlacementUpperStr=="N") {
						initialPlacementUpperReadable="No";
					}
					else{
						initialPlacementUpperReadable="Yes";
					}
				}
				if(initialPlacementLowerStr=="X") {
					initialPlacementLowerReadable=" ";
				}
				else {
					if(initialPlacementLowerStr=="N") {
						initialPlacementLowerReadable="No";
					}
					else {
						initialPlacementLowerReadable="Yes";
					}
				}
			}
			if(_isFrench) {
				_documentGenerator.DrawString(g,"S'agit-il de la première mise en bouche d'une couronne, prothèse ou pont? Maxillaire:      Mandibule:",_x,0);
				_documentGenerator.DrawString(g,initialPlacementUpperReadable,(_x+555),0);
				_documentGenerator.DrawString(g,initialPlacementLowerReadable,(_x+670),0);
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,"Si Non, indiquer le matériau de l'ancienne prothèse et la date de mise en bouche:",_x,0);
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,"Maxillaire:      Matériau:                              Date:",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"Is this an initial placement for crown, denture or bridge?      Maxillary:      Mandibular:",_x,0);
				_documentGenerator.DrawString(g,initialPlacementUpperReadable,(_x+510),0);
				_documentGenerator.DrawString(g,initialPlacementLowerReadable,(_x+625),0);
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,"If no, indicate the material used for the previous prosthesis and the date of insertion:",_x,0);
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,"Maxillary:       Material:                              Date:",_x,0);
			}
			_documentGenerator.DrawString(g,initialPlacementUpperReadable,_x+75,0);
			if(initialPlacementUpperStr=="N") {//Field F15 - Avoid invalid upper initial placement data.
				_text=GetMaterialDescription(_claim.CanadianMaxProsthMaterial);//Field F20
				_documentGenerator.DrawString(g,_text,_x+180,0);
				_text=DateToString(_claim.CanadianDateInitialUpper);//Field F04
				_documentGenerator.DrawString(g,_text,_x+420,0);
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_documentGenerator.DrawString(g,"Mandibule:       Matériau:                              Date:",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"Mandibular:      Material:                              Date:",_x,0);
			}
			_documentGenerator.DrawString(g,initialPlacementLowerReadable,_x+75,0);
			if(initialPlacementLowerStr=="N") {//Field F18 - Avoid invalid lower initial placement data.				
				_text=GetMaterialDescription(_claim.CanadianMandProsthMaterial);//Field F21
				_documentGenerator.DrawString(g,_text,_x+180,0);
				_text=DateToString(_claim.CanadianDateInitialLower);//Field F19
				_documentGenerator.DrawString(g,_text,_x+420,0);
			}
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_documentGenerator.DrawString(g,"Si Oui, indiquer le numéro des dents manquantes et la date des extractions.",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"If yes, indicate the missing teeth numbers and the date(s) on which they were removed.",_x,0);
			}
			_x=_documentGenerator.StartElement();
			PrintMissingToothList(g);
			_x=_documentGenerator.PopX();//End first indentation.			
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				sizeF1=_documentGenerator.DrawString(g,_bullet.ToString()+". "+("S'agit-il d'un traitement en vue de soins d'orthodontie?"),_x,0);
			}
			else {
				sizeF1=_documentGenerator.DrawString(g,_bullet.ToString()+". "+("Is treatment for orthodontic purposes? "),_x,0);
			}
			_bullet++;
			if(_claim!=null){
				if(_claim.IsOrtho){//Field F05
					_documentGenerator.DrawString(g,_isFrench?"Oui":"Yes",_x+sizeF1.Width,0);
				}
				else{
					_documentGenerator.DrawString(g,_isFrench?"Non":"No",_x+sizeF1.Width,0);
				}
			}
			CCDField ccdFieldOrthodonticRecordFlag=_ccdFieldInputterFormData.GetFieldById("F25");
			if(_hasPredetermination && ccdFieldOrthodonticRecordFlag!=null && ccdFieldOrthodonticRecordFlag.valuestr=="1"){
				_x=_documentGenerator.StartElement();
				_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
				_bullet++;
				_documentGenerator.PushX(_x);//Begin indentation.
				if(_isFrench) {
					_documentGenerator.DrawString(g,"S'il s'agit d'un plan de traitement d'orthodontie, indiquer",_x,0);			
				}
				else {
					_documentGenerator.DrawString(g,"For orthodontic treatment plan, please indicate:",_x,0);			
				}
				_x=_documentGenerator.StartElement();
				_text=_ccdFieldInputterFormData.GetFieldById("F30").valuestr;//Duration of treatment in months.
				if(_text!="00"){
					_text=_text.TrimStart('0');
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Durée du traitement: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Duration of treatment: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F26").valuestr;//First examination fee in raw form.
				if(_text!="000000"){
					_text=RawMoneyStrToDisplayMoney(_text);
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Tarif du premier examen: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"First examination fee: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F27").valuestr;//Diagnostic Phase Fee in raw form.
				if(_text!="000000"){
					_text=RawMoneyStrToDisplayMoney(_text);
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Tarif de la phase diagnostique: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Diagnostic phase fee: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F28").valuestr;//Initial fee in raw form.
				if(_text!="000000"){
					_text=RawMoneyStrToDisplayMoney(_text);
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Paiement initial: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Initial fee: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F29").valuestr;//Payment mode or expected payment cycle as an enumeration value.
				if(_text!="0"){
					if(_text=="1"){
						_text=_isFrench?"Mensuel":"Monthly";
					}
					else if(_text=="2"){
						_text=_isFrench?"Bimestriel":"Bimonthly";
					}
					else if(_text=="3"){
						_text=_isFrench?"Trimestriel":"Quarterly";
					}
					else{//4
						if(_isFrench) {
							_text="Aux quatre mois";
						}
						else {
							_text="Every four months";
						}
					}
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Mode de paiement: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Payment mode: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F31").valuestr;//Number of anticipated payments
				if(_text!="00"){
					_text=_text.TrimStart('0');
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Nombre prévu de paiements: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Number of anticipated payments: ",_text,true,_x,0);
					}
				}
				_text=_ccdFieldInputterFormData.GetFieldById("F32").valuestr;
				if(_text!="000000"){
					_text=RawMoneyStrToDisplayMoney(_text);
					if(_isFrench) {
						_documentGenerator.DrawField(g,"Montant prévu du paiement: ",_text,true,_x,0);
					}
					else {
						_documentGenerator.DrawField(g,"Anticipated payment amount: ",_text,true,_x,0);
					}
				}				
				_x=_documentGenerator.StartElement(_verticalLine);
				_x=_documentGenerator.PopX();//End indentation.
			}
			_x=_documentGenerator.StartElement();
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Underline);
			if(_isFrench) {
				_documentGenerator.DrawString(g,"Validation du titulaire/employeur",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,"Policy holder / employer certification",_x,0);
			}
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Regular);
			_x=_documentGenerator.StartElement();
			underlineWidth=g.MeasureString("**************",_documentGenerator.standardFont).Width;
			if(_isFrench) {
				sizeF1=_documentGenerator.DrawString(g,"1. Entrée en vigueur de couverture:",_x,0);
			}
			else {
				sizeF1=_documentGenerator.DrawString(g,"1. Date coverage commenced:",_x,0);
			}
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+sizeF1.Width,_x+sizeF1.Width+underlineWidth,sizeF1.Height);
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				sizeF1=_documentGenerator.DrawString(g,"2. Entrée en vigueur de personne à charge:",_x,0);
			}
			else {
				sizeF1=_documentGenerator.DrawString(g,"2. Date dependant covered:",_x,0);
			}
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+sizeF1.Width,_x+sizeF1.Width+underlineWidth,sizeF1.Height);
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				sizeF1=_documentGenerator.DrawString(g,"3. Terminaison:",_x,0);
			}
			else {
				sizeF1=_documentGenerator.DrawString(g,"3. Date terminated:",_x,0);
			}
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+sizeF1.Width,_x+sizeF1.Width+underlineWidth,sizeF1.Height);
			_x=_documentGenerator.StartElement(_verticalLine);
			_documentGenerator.PushX(_x);
			if(_isFrench) {
				_x+=_documentGenerator.DrawString(g,"Signature de personne autorisée:",_x,0).Width;
			}
			else {
				_x+=_documentGenerator.DrawString(g,"Authorized signature:",_x,0).Width;
			}
			_x+=_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+150,_verticalLine).Width+10;
			_x+=_documentGenerator.DrawString(g,_isFrench?"Fonction:":"Position:",_x,0).Width;
			_x+=_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+100,_verticalLine).Width+10;
			_x+=_documentGenerator.DrawString(g,"Date:",_x,0).Width;
			_x+=_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+150,_verticalLine).Width+10;
			_x=_documentGenerator.PopX();
			_x=_documentGenerator.StartElement();
			float yOffset=0;
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Underline);
			if(_isFrench) {
				yOffset+=_documentGenerator.DrawString(g,"Signature du patient et du dentiste",_x,yOffset).Height;
			}
			else {
				yOffset+=_documentGenerator.DrawString(g,"Patient's and Dentist's signature",_x,yOffset).Height;
			}
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size,FontStyle.Regular);
			if(_isFrench) {
				yOffset+=_documentGenerator.DrawString(g,"Je déclare qu’à ma connaissance les renseignements donnés sont "+
					"véridiques, exacts et complets. J’autorise la divulgation à l’assureur, ou au Centre "+
					"Dentaide, ou à leurs mandataires de tout renseignement ou dossier relatif à cette "+
					"demande de prestations. Je conviens de rebourser à l’assureur toute somme "+
					"débourséeindûment à mon égard. Je m;engage à verser au dentiste la portion non "+
					"assurée des frais et honoraires demandés pour les soins décrits ci-après.",_x,yOffset).Height;
			}
			else {
				yOffset+=_documentGenerator.DrawString(g,
					"I certify that to my knowledge this information is truthful, accurate and complete. "+
					"I authorize the disclosure to the insurer, or Centre Dentaide, or their agents of any "+
					"information or file related to this claim. I agree to repay the insurer for any sum "+
					"paid on my behalf, and to pay the dentist the required fees for the uninsured portion "+
					"of the treatment described hereinafter.",_x,yOffset).Height;
			}
			yOffset+=2*_verticalLine;
			yOffset+=_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+400,yOffset).Height;
			if(_isFrench) {
				yOffset+=_documentGenerator.DrawString(g,"Signature du patient (ou parent/tuteur)",_x,yOffset).Height;
			}
			else {
				yOffset+=_documentGenerator.DrawString(g,"Signature of patient (or parent/guardian)",_x,yOffset).Height;
			}
			yOffset+=_verticalLine;
			if(_isFrench) {
				yOffset+=_documentGenerator.DrawString(g,"La présente constitue une description exacte des soins exécutés "+
					"et des honoraires demandés ou, dans le cas d'un plan de traitement, des traitements "+
					"devant être exécutés et des honoraires s'y rapportant, sauf erreur ou omission.",_x,yOffset).Height;
			}
			else {
				yOffset+=_documentGenerator.DrawString(g,"The above and the treatments described below are an accurate statement of services "+
					"performed and fees charged, or of services to be performed and fees to be charged in "+
					"the case of a treatment plan, except errors and omissions.",_x,yOffset).Height;
			}
			yOffset+=_verticalLine*2;
			yOffset+=_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+400,yOffset).Height;
			if(_isFrench) {
				yOffset+=_documentGenerator.DrawString(g,"Signature du dentiste",_x,yOffset).Height;
				_x=_documentGenerator.StartElement();
				sizeF1=_documentGenerator.DrawString(g,"Date du traitement: ",_x,0);
			}
			else {
				yOffset+=_documentGenerator.DrawString(g,"Dentist's signature",_x,yOffset).Height;
				_x=_documentGenerator.StartElement();
				sizeF1=_documentGenerator.DrawString(g,"Date of Service: ",_x,0);
			}
			_text=DateToString(_etrans.DateTimeTrans);
			_documentGenerator.DrawString(g,_text,_x+sizeF1.Width,0);
			_x=_documentGenerator.StartElement(_verticalLine);
			bool isEOB=_transactionCode=="21" || _transactionCode=="23";
			CCDField[] ccdFieldArrayNoteNumbers=_ccdFieldInputterFormData.GetFieldsById("G45");//Used to looking up note reference numbers.
			if(_listClaimProcs!=null) {
				List <Procedure> listProcedures=Procedures.Refresh(_claim.PatNum);
				float amountWidth=g.MeasureString("****.**",_documentGenerator.standardFont).Width;
				//The rest of the CCDField[] object should all be the same length, since they come bundled together as part 
				//of each procedure.
				CCDField[] ccdFieldsArrayProcedureLineNumbers=_ccdFieldInputterFormData.GetFieldsById("F07");
				CCDField[] ccdFieldsArrayEligibleAmounts=_ccdFieldInputterFormData.GetFieldsById("G12");
				CCDField[] ccdFieldsArrayDeductibleAmounts=_ccdFieldInputterFormData.GetFieldsById("G13");
				CCDField[] ccdFieldsArrayEligiblePercentages=_ccdFieldInputterFormData.GetFieldsById("G14");
				CCDField[] ccdFieldsArrayDentaidePayAmounts=_ccdFieldInputterFormData.GetFieldsById("G15");
				CCDField[] ccdFieldsArrayExplainationNoteNumbers1=_ccdFieldInputterFormData.GetFieldsById("G16");
				CCDField[] ccdFieldsArrayExplainationNoteNumbers2=_ccdFieldInputterFormData.GetFieldsById("G17");
				CCDField[] ccdFieldsArrayEligibleLabAmounts1=_ccdFieldInputterFormData.GetFieldsById("G43");
				CCDField[] ccdFieldsArrayDeductibleLabAmounts1=_ccdFieldInputterFormData.GetFieldsById("G56");
				CCDField[] ccdFieldsArrayEligibleLabPercentage1=_ccdFieldInputterFormData.GetFieldsById("G57");
				CCDField[] ccdFieldsArrayBenefitLabAmount1=_ccdFieldInputterFormData.GetFieldsById("G58");
				CCDField[] ccdFieldsArrayEligibleLabAmounts2=_ccdFieldInputterFormData.GetFieldsById("G02");
				CCDField[] ccdFieldsArrayDeductibleLabAmounts2=_ccdFieldInputterFormData.GetFieldsById("G59");
				CCDField[] ccdFieldsArrayEligibleLabPercentage2=_ccdFieldInputterFormData.GetFieldsById("G60");
				CCDField[] ccdFieldsArrayBenefitLabAmount2=_ccdFieldInputterFormData.GetFieldsById("G61");
				float noteColumn=_x;
				float noteColumnWidth=(isEOB?55:0);
				float procedureColumn=noteColumn+noteColumnWidth;
				float procedureColumnWidth=50;
				float toothColumn=procedureColumn+procedureColumnWidth;
				float toothColumnWidth=45;
				float surfaceColumn=toothColumn+toothColumnWidth;
				float surfaceColumnWidth=55;
				float feeColumn=surfaceColumn+surfaceColumnWidth;
				float feeColumnWidth=75;
				float eligibleFeeColumn=feeColumn+feeColumnWidth;
				float eligibleFeeColumnWidth=(isEOB?75:0);
				float labColumn=eligibleFeeColumn+eligibleFeeColumnWidth;
				float labColumnWidth=80;
				float eligibleLabColumn=labColumn+labColumnWidth;
				float eligibleLabColumnWidth=(isEOB?75:0);
				float deductibleColumn=eligibleLabColumn+eligibleLabColumnWidth;
				float deductibleColumnWidth=(isEOB?75:0);
				float percentCoveredColumn=deductibleColumn+deductibleColumnWidth;
				float percentCoveredColumnWidth=(isEOB?90:0);
				float dentaidePaysColumn=percentCoveredColumn+percentCoveredColumnWidth;
				float dentaidePaysColumnWidth=(isEOB?65:0);
				float endNoteColumn=dentaidePaysColumn+dentaidePaysColumnWidth;
				Font font=_documentGenerator.standardFont;
				_documentGenerator.standardFont=_fontStandardSmall;
				double totalFee=0;
				double totalLab=0;
				double totalPaid=0;
				int numProcsPrinted=0;
				if(isEOB) {
					_documentGenerator.DrawString(g,"Note",noteColumn,0);
					_documentGenerator.DrawString(g,_isFrench?"Admissible":"Eligible",eligibleFeeColumn,0);
					_documentGenerator.DrawString(g,_isFrench?"Admissible":"Eligible",eligibleLabColumn,0);
					_documentGenerator.DrawString(g,_isFrench?"Franchise":"Deductible",deductibleColumn,0);
					_documentGenerator.DrawString(g,_isFrench?"%Couvertrem.":"Percent\nCovered",percentCoveredColumn,0);
					_documentGenerator.DrawString(g,_isFrench?"Dentaide":"Detaide\nPays",dentaidePaysColumn,0);
				}
				_documentGenerator.DrawString(g,_isFrench?"Acte":"Proc",procedureColumn,0);
				_documentGenerator.DrawString(g,_isFrench?"Dent":"Tooth",toothColumn,0);
				_documentGenerator.DrawString(g,"Surface",surfaceColumn,0);
				_documentGenerator.DrawString(g,_isFrench?"Honoraires":"Fee",feeColumn,0);
				_documentGenerator.DrawString(g,_isFrench?"Labo.":"Lab",labColumn,0);
				for(int p=1;p<=_listClaimProcs.Count;p++) {
					_x=_documentGenerator.StartElement();
					int procLineNum=numProcsPrinted+p;
					if(procLineNum>_listClaimProcs.Count) {
						continue;
					}
					ClaimProc claimProc=null;
					for(int i=0;i<_listClaimProcs.Count;i++) {
						if(_listClaimProcs[i].LineNumber==procLineNum) {
							claimProc=_listClaimProcs[i];
							break;
						}
					}
					Procedure procedure=Procedures.GetOneProc(claimProc.ProcNum,true);
					_text=claimProc.CodeSent.PadLeft(6,' ');//Field F08
					_documentGenerator.DrawString(g,_text,procedureColumn,0);
					_text=Tooth.ToInternat(procedure.ToothNum);//Field F10
					_documentGenerator.DrawString(g,_text,toothColumn,0);
					_text=Tooth.SurfTidyForClaims(procedure.Surf,procedure.ToothNum);//Field F11
					_documentGenerator.DrawString(g,_text,surfaceColumn,0);
					_text=procedure.ProcFee.ToString("F");//Field F12
					_documentGenerator.DrawString(g,_text,feeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					totalFee+=procedure.ProcFee;
					if(isEOB) {
						List <Procedure> listProceduresLab=Procedures.GetCanadianLabFees(procedure.ProcNum,listProcedures);
						for(int j=0;j<ccdFieldsArrayProcedureLineNumbers.Length;j++) {
							if(Convert.ToInt32(ccdFieldsArrayProcedureLineNumbers[j].valuestr)==procLineNum) {
								//Display the procedure information on its own line.
								//For any procLineNum>0, there will only be one matching carrier procedure, by definition.
								sizeF1=new SizeF(0,0);
								int noteIndex=Convert.ToInt32(ccdFieldsArrayExplainationNoteNumbers1[j].valuestr);
								if(noteIndex>0) {
									sizeF1=_documentGenerator.DrawString(g,ccdFieldArrayNoteNumbers[noteIndex].valuestr,noteColumn,0);
								}
								noteIndex=Convert.ToInt32(ccdFieldsArrayExplainationNoteNumbers2[j].valuestr);
								if(noteIndex>0) {
									_documentGenerator.DrawString(g,Environment.NewLine+ccdFieldArrayNoteNumbers[noteIndex].valuestr,noteColumn+sizeF1.Width,0);
								}
								_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleAmounts[j].valuestr);
								_documentGenerator.DrawString(g,_text,eligibleFeeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
								_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleAmounts[j].valuestr);
								_documentGenerator.DrawString(g,_text,deductibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
								_text=RawPercentToDisplayPercent(ccdFieldsArrayEligiblePercentages[j].valuestr);
								_documentGenerator.DrawString(g,_text,percentCoveredColumn,0);
								_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDentaidePayAmounts[j].valuestr);
								_documentGenerator.DrawString(g,_text,dentaidePaysColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
								totalPaid+=Convert.ToDouble(_text);
								if(listProceduresLab.Count>0) {
									//Display the Lab1 information on its own line.
									_x=_documentGenerator.StartElement();
									_text=ProcedureCodes.GetProcCodeFromDb(listProceduresLab[0].CodeNum).ProcCode.PadLeft(6,' ');
									_documentGenerator.DrawString(g,_text,procedureColumn,0);
									_text=listProceduresLab[0].ProcFee.ToString("F");
									totalLab+=listProceduresLab[0].ProcFee;
									_documentGenerator.DrawString(g,_text,labColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleLabAmounts1[j].valuestr);
									_documentGenerator.DrawString(g,_text,eligibleLabColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleLabAmounts1[j].valuestr);
									_documentGenerator.DrawString(g,_text,deductibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawPercentToDisplayPercent(ccdFieldsArrayEligibleLabPercentage1[j].valuestr);
									_documentGenerator.DrawString(g,_text,percentCoveredColumn,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayBenefitLabAmount1[j].valuestr);
									_documentGenerator.DrawString(g,_text,dentaidePaysColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
								}
								if(listProceduresLab.Count>1) {
									//Display the Lab2 information on its own line.
									_x=_documentGenerator.StartElement();
									_text=ProcedureCodes.GetProcCodeFromDb(listProceduresLab[1].CodeNum).ProcCode.PadLeft(6,' ');
									_documentGenerator.DrawString(g,_text,procedureColumn,0);
									_text=listProceduresLab[1].ProcFee.ToString("F");
									totalLab+=listProceduresLab[1].ProcFee;
									_documentGenerator.DrawString(g,_text,labColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleLabAmounts2[j].valuestr);
									_documentGenerator.DrawString(g,_text,eligibleLabColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleLabAmounts2[j].valuestr);
									_documentGenerator.DrawString(g,_text,deductibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
									_text=RawPercentToDisplayPercent(ccdFieldsArrayEligibleLabPercentage2[j].valuestr);
									_documentGenerator.DrawString(g,_text,percentCoveredColumn,0);
									_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayBenefitLabAmount2[j].valuestr);
									_documentGenerator.DrawString(g,_text,dentaidePaysColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
								}
							}
						}
					}
				}
				if(isEOB) {
					//List the carrier inserted procedures into the procedure list.
					CCDField[] ccdFieldsArrayProcs=_ccdFieldInputterFormData.GetFieldsById("G19");
					CCDField[] ccdFieldsArrayCarrierEligibleAmts=_ccdFieldInputterFormData.GetFieldsById("G20");
					CCDField[] ccdFieldsArrayCarrierEligibleLabAmts=_ccdFieldInputterFormData.GetFieldsById("G44");
					CCDField[] ccdFieldsArrayCarrierDeductAmts=_ccdFieldInputterFormData.GetFieldsById("G21");
					CCDField[] ccdFieldsArrayCarrierAts=_ccdFieldInputterFormData.GetFieldsById("G22");
					CCDField[] ccdFieldsArrayCarrierBenefitAmts=_ccdFieldInputterFormData.GetFieldsById("G23");
					CCDField[] ccdFieldsArrayCarrierNotes1=_ccdFieldInputterFormData.GetFieldsById("G24");
					CCDField[] ccdFieldsArrayCarrierNotes2=_ccdFieldInputterFormData.GetFieldsById("G25");
					for(int p=0;p<ccdFieldsArrayProcs.Length;p++) {
						//Display the eligible proc info.
						_x=_documentGenerator.StartElement();
						_text=ccdFieldsArrayProcs[p].valuestr.PadLeft(6,' ');//Field G19
						_documentGenerator.DrawString(g,_text,procedureColumn,0);
						_text=ProcedureCodes.GetProcCode(ProcedureCodes.GetCodeNum(_text)).Descript;
						_documentGenerator.DrawString(g,_text,procedureColumn+procedureColumnWidth,0,_documentGenerator.standardFont,(int)(toothColumn-procedureColumn-procedureColumnWidth-10));
						_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierEligibleAmts[p].valuestr);//Field G20
						_documentGenerator.DrawString(g,_text,eligibleFeeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
						_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierDeductAmts[p].valuestr);//Field G21
						_documentGenerator.DrawString(g,_text,deductibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
						_text=RawPercentToDisplayPercent(ccdFieldsArrayCarrierAts[p].valuestr);//Field G22
						_documentGenerator.DrawString(g,_text,percentCoveredColumn,0);
						_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierBenefitAmts[p].valuestr);//Field G23
						_documentGenerator.DrawString(g,_text,dentaidePaysColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
						_text="";
						if(ccdFieldsArrayCarrierNotes1[p].valuestr!="00") {
							_text+=ccdFieldsArrayCarrierNotes1[p].valuestr;
						}
						if(ccdFieldsArrayCarrierNotes2[p].valuestr!="00") {
							if(_text.Length>0) {
								_text+=",";
							}
							_text+=ccdFieldsArrayCarrierNotes2[p].valuestr;
						}
						_documentGenerator.DrawString(g,_text,endNoteColumn,0);
						//Display the eligible lab info for the proc but on a separate line.
						_x=_documentGenerator.StartElement();
						_text="LAB(S)";
						_documentGenerator.DrawString(g,_text,procedureColumn,0);
						_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierEligibleLabAmts[p].valuestr);
						_documentGenerator.DrawString(g,_text,eligibleLabColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					}
				}
				_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,_documentGenerator.standardFont.Size+1,FontStyle.Bold);
				_x=_documentGenerator.StartElement(_verticalLine);
				_documentGenerator.DrawString(g,"TOTAL:",toothColumn,0);
				if(isEOB) {
					CCDField ccdFieldTotalPayable=_ccdFieldInputterFormData.GetFieldById("G55");
					if(ccdFieldTotalPayable!=null) {
						totalPaid=PIn.Double(RawMoneyStrToDisplayMoney(ccdFieldTotalPayable.valuestr));
					}
					_documentGenerator.DrawString(g,totalPaid.ToString("F"),dentaidePaysColumn,0);
				}
				_documentGenerator.DrawString(g,totalFee.ToString("F"),feeColumn,0);
				_documentGenerator.DrawString(g,totalLab.ToString("F"),labColumn,0);
				_documentGenerator.standardFont=font;
			}
			if(isEOB){
				_documentGenerator.StartElement();
				_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
				PrintNoteList(g);
			}
		}

		///<summary>Contains different header and footer based on wether or not this is a patient copy.</summary>
		private void PrintClaimAck(Graphics g){
			PrintCarrier(g);
			_x=_documentGenerator.StartElement();
			if(_responseStatus=="R") {
				if(_isFrench) {
					_text="REFUS D'UNE DEMANDE DE PRESTATIONS";
				}
				else {
					_text="CLAIM REJECTION NOTICE";
				}
			}
			else {
				if(_doPrintPatientCopy) {
					if(_isFrench) {
						_text="ACCUSÉ DE RÉCEPTION D'UNE DEMANDE DE PRESTATIONS - COPIE DU PATIENT";
					}
					else {
						_text="CLAIM ACKNOWLEDGEMENT - PATIENT COPY";
					}
				}
				else {
					if(_isFrench) {
						_text="ACCUSÉ DE RÉCEPTION D'UNE DEMANDE DE PRESTATIONS - COPIE DU DENTISTE";
					}
					else {
						_text="CLAIM ACKNOWLEDGEMENT - DENTIST COPY";				
					}
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="Nous avons utilisé les renseignements du présent formulaire pour traiter votre demande par ordinateur. Veuillez en vérifier l'exactitude et aviser votre dentiste en cas d'erreur. Prière de ne pas poster à l'assureur/administrateur du régime.";
			}
			else {
				_text="The information contained on this form has been used to process your claim electronically. Please verify the accuracy of this data and report any discrepancies to your dental office. Do not mail this form to the insurer/plan administrator.";
			}
			PrintClaimAckBody(g,_text);
			if(_responseStatus=="R") {
				if(_isFrench) {
					_text="VEUILLEZ CORRIGER LES ERREURS AVANT DE RESOUMETTRE LA DEMANDE.";				
				}
				else {
					_text="PLEASE CORRECT ERRORS AS SHOWN, PRIOR TO RE-SUBMITTING THE CLAIM.";
				}
			}
			else {
				if(_isFrench) {
					_text="La présente demande de prestations a été transmise par ordinateur.".ToUpper();
					_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
					_x=_documentGenerator.StartElement();
					_text="Elle sert de reçu seulement.".ToUpper();
				}
				else {
					_text="THIS CLAIM HAS BEEN SUBMITTED ELECTRONICALLY - THIS IS A RECEIPT ONLY";
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
		}

		private void PrintPredeterminationAck(Graphics g){
			PrintCarrier(g);
			_x=_documentGenerator.StartElement(_verticalLine);
			if(_doPrintPatientCopy){
				if(_isFrench) {
					_text="ACCUSÉ DE RÉCEPTION D'UN PRÉDÉTERMINATION - COPIE DU PATIENT";
				}
				else {
					_text="PREDETERMINATION ACKNOWLEDGMENT - PATIENT COPY";
				}
			}
			else{
				if(_isFrench) {
					_text="ACCUSÉ DE RÉCEPTION D'UN PRÉDÉTERMINATION - COPIE DU DENTISTE";
				}
				else {
					_text="PREDETERMINATION ACKNOWLEDGMENT - DENTIST COPY";
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement(_verticalLine);
			if(_isFrench) {
				_text="Nous avons utilisé les renseignements du présent formulaire pour traiter votre demande par"+
					"ordinateur. Veuillez en vérifier l'exactitude et aviser votre dentiste en cas d'erreur. "+
					"Prière de ne pas poster à l'assureur/administrateur du régime.";
			}
			else {
				_text="The information contained on this form has been used to process your claim electronically. "+
					"Please verify the accuracy of this data and report any discrepancies to your dental office. "+
					"Do not mail this form to the insurer/plan administrator.";
			}
			PrintClaimAckBody(g,_text);
			if(_isFrench){
				_text="La présente demande de prestations a été transmise par ordinateur.".ToUpper();
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Elle sert de reçu seulement.".ToUpper();
			}
			else{
				_text="THIS PREDETERMINATION HAS BEEN SUBMITTED ELECTRONICALLY - THIS IS A RECEIPT ONLY";
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
		}

		private void PrintEmployerCertified(Graphics g){
			PrintCarrier(g);
			_x=_documentGenerator.StartElement(_verticalLine);
			if(_doPrintPatientCopy){
				if(_isFrench) {
					_text="VALIDATION PAR L'EMPLOYEUR/ADMINISTRATEUR DU RÉGIME - COPIE DU PATIENT";
				}
				else {
					_text="EMPLOYER/PLAN ADMINISTRATOR CERTIFIED FORM - PATIENT COPY";
				}
			}
			else{

				if(_isFrench) {
					_text="VALIDATION PAR L'EMPLOYEUR/ADMINISTRATEUR DU RÉGIME - COPIE DE DENTISTE";
				}
				else {
					_text="EMPLOYER/PLAN ADMINISTRATOR CERTIFIED FORM - DENTIST COPY";
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement(_verticalLine);
			_text=_isFrench?
				"Nous avons utilisé les renseignements du présent formulaire pour traiter votre demande par"+
				"ordinateur. Veuillez en vérifier l'exactitude et aviser votre dentiste en cas d'erreur.":
				"The information contained on this form has been used to process your claim electronically. "+
				"Please verify the accuracy of this data and report any discrepancies to your dental office.";
			PrintClaimAckBody(g,_text);
			_documentGenerator.DrawString(g,_isFrench?"VALIDATION DU TITULAIRE/EMPLOYEUR":"POLICYHOLDER/EMPLOYER - CERTIFICATION",_x,0);
			_x=_documentGenerator.StartElement(_verticalLine);
			SizeF size=_documentGenerator.DrawString(g,_isFrench?"EMPLOYEUR: ":"EMPLOYER: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine/2);
			size=_documentGenerator.DrawString(g,_isFrench?"ENTRÉE EN VIGUEUR DE COUVERTURE: ":"DATE COVERAGE COMMENCED: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine/2);
			size=_documentGenerator.DrawString(g,_isFrench?"ENTRÉE EN VIGUEUR DE COUVERTURE DE PERSONNE À CHARGE: ":"DATE DEPENDANT COVERED: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine/2);
			size=_documentGenerator.DrawString(g,_isFrench?"DATE DE TERMINAISON: ":"DATE TERMINATED: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine/2);
			size=_documentGenerator.DrawString(g,_isFrench?"SIGNATURE DE PERSONNE AUTORISÉE: ":"SIGNATURE OF AUTHORIZED OFFICIAL: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine/2);
			size=_documentGenerator.DrawString(g,_isFrench?"DATE D'AUTORISATION: ":"AUTHORIZATION DATE: ",_x,0);
			_documentGenerator.HorizontalLine(g,Pens.Black,_x+size.Width,_documentGenerator.bounds.Right,size.Height);
			_x=_documentGenerator.StartElement(_verticalLine);
			_text=_isFrench?"LA PRÉSENTE DEMANDE A ÉTÉ TRANSMISE PAR ORDINATEUR À:":"THIS CLAIM HAS BEEN SUBMITTED ELECTRONICALLY TO:";
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			PrintCarrier(g);
			_x=_documentGenerator.StartElement();
			_text=_isFrench?"VEUILLEZ LA FAIRE VALIDER PAR VOTRE. EMPLOYEUR":"PLEASE TAKE THIS FORM TO YOUR EMPLOYER FOR CERTIFICATION";
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
		}

		///<summary>Does the actual work for printing claims. When graphicObjects is null, returns the required graphicObjects after calculating them. In normal use, this function should be called twice for each time the form is rendered, once to calculate the graphical primitives, then once more to actually render to form for printing.</summary>
		private void PrintClaimAckBody(Graphics g,string centralDisclaimer){
			_documentGenerator.standardFont=new Font(_fontStandardSmall.FontFamily,8,FontStyle.Regular);
			PrintTransactionDate(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintStatus(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintDisposition(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintDentistName(g,_x,0);
			PrintDentistPhone(g,_x+250,0);
			PrintTreatmentProviderID(g,_x+500,0);
			_x=_documentGenerator.StartElement();
			PrintOfficeSequenceNumber(g,_x,0);
			PrintTreatmentProviderOfficeNumber(g,_x+500,0);
			_x=_documentGenerator.StartElement();
			PrintPolicyNo(g,_x,0,true);
			PrintDivisionSectionNo(g,_x+500,0);
			_x=_documentGenerator.StartElement();
			PrintCertificateNo(g,_x,0,true);
			PrintPrimaryDependantNo(g,_x+250,0);
			PrintCardNo(g,_x+500,0);
			_x=_documentGenerator.StartElement();
			PrintPatientName(g,_x,0);
			PrintPatientBirthday(g,_x+500,0);			
			_x=_documentGenerator.StartElement();
			PrintInsuredMember(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintInsuredAddress(g,_x,0,true,0);
			PrintTransactionReferenceNumber(g,_x+400,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_x=_documentGenerator.StartElement();
			//Payee not visible in predetermination
			PrintProcedureListAck(g,_hasPredetermination?"":GetPayableToString(AssignmentOfBenefits()));
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_x=_documentGenerator.StartElement();
			if(_responseStatus=="R") {
				PrintErrorList(g);
			}
			else {
				_documentGenerator.standardFont=new Font(_fontStandardSmall.FontFamily,10,FontStyle.Bold);
				_documentGenerator.DrawString(g,centralDisclaimer,_x,0);
				_documentGenerator.standardFont=new Font(_fontStandardSmall.FontFamily,8,FontStyle.Regular);
				_x=_documentGenerator.StartElement();
				_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,_isFrench?"RENSEIGNEMENTS SUR LE PATIENT":"PATIENT INFORMATION",_x,0);
				_x=_documentGenerator.StartElement();
				_bullet=1;
				PrintDependencyBullet(g);
				_x=_documentGenerator.StartElement();
				PrintSecondaryPolicyBullet(g);
				_x=_documentGenerator.StartElement();
				PrintAccidentBullet(g);
				_x=_documentGenerator.StartElement();
				PrintInitialPlacementBullet(g);
				_x=_documentGenerator.StartElement();
				PrintToothExtractionBullet(g);
				_x=_documentGenerator.StartElement();
				_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			}
			_x=_documentGenerator.StartElement(_verticalLine);
		}

		///<summary>Prints the list of procedures performed for the patient on the document in question.</summary>
		private void PrintProcedureListAck(Graphics g,string payableToStr) {
			if(_claim==null){//for eligibility response.
				return;
			}
			float amountWidth=g.MeasureString("****.**",_documentGenerator.standardFont).Width;
			float procedureCodeColumn=_x;
			float procedureToothColumn=procedureCodeColumn+470;
			float procedureSurfaceColumn=procedureToothColumn+25;
			float procedureDateColumn=procedureSurfaceColumn+52;
			float procedureDateColumnWidth=_hasPredetermination?0:80;
			float procedureChargeColumn=procedureDateColumn+procedureDateColumnWidth;
			float procedureTotalColumn=procedureChargeColumn+amountWidth+10;//charge col width is amount width.
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"ACTE":"PROCEDURE",procedureCodeColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"D#":"TH#",procedureToothColumn,0);
			_documentGenerator.DrawString(g,"SURF",procedureSurfaceColumn,0);//Same in both languages.
			if(!_hasPredetermination){
				_documentGenerator.DrawString(g,"DATE",procedureDateColumn,0);//Same in both languages.
			}
			_documentGenerator.DrawString(g,_isFrench?"HONS":"CHARGE",procedureChargeColumn,0);
			_documentGenerator.DrawString(g,"TOTAL",procedureTotalColumn,0);//Same in both languages.
			_x=_documentGenerator.StartElement();
			Procedure procedure;
			float procCodeWidth=g.MeasureString("*******",_documentGenerator.standardFont).Width;
			List<ClaimProc> listClaimProcsOrdered=new List<ClaimProc>();
			for(int i=0;i<_listClaimProcs.Count;i++) {
				ClaimProc claimProc=_listClaimProcs[i];
				if(claimProc.ProcNum==0) {
					continue;
				}
				int j=0;
				for(;j<listClaimProcsOrdered.Count;j++) {
					if(listClaimProcsOrdered[j].LineNumber>claimProc.LineNumber) {
						break;
					}
				}
				listClaimProcsOrdered.Insert(j,claimProc);
			}
			List <Procedure> listProcedures=Procedures.Refresh(_claim.PatNum);
			decimal totalSubmitted=0;
			for(int i=0;i<listClaimProcsOrdered.Count;i++) {
				ClaimProc claimProc=listClaimProcsOrdered[i];
				procedure=Procedures.GetOneProc(claimProc.ProcNum,true);
				_text=claimProc.CodeSent.PadLeft(6,' ');
				_documentGenerator.DrawString(g,_text,_x,0);
				_text=ProcedureCodes.GetProcCode(procedure.CodeNum).Descript;
				_documentGenerator.DrawString(g,_text,procedureCodeColumn+procCodeWidth,0,_documentGenerator.standardFont,(int)(procedureToothColumn-procedureCodeColumn-procCodeWidth-10));
				_text=Tooth.ToInternat(procedure.ToothNum);//Field F10
				_documentGenerator.DrawString(g,_text,procedureToothColumn,0);
				_text=Tooth.SurfTidyForClaims(procedure.Surf,procedure.ToothNum);//Field F11
				_documentGenerator.DrawString(g,_text,procedureSurfaceColumn,0);
				if(!_hasPredetermination) {//Used to remove service dates in a predetermination ack.
					_text=procedure.ProcDate.ToShortDateString();//Field F09
					_documentGenerator.DrawString(g,_text,procedureDateColumn,0);
				}
				_text=claimProc.FeeBilled.ToString("F");//Field F12
				_documentGenerator.DrawString(g,_text,procedureChargeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				decimal feeBilledTotal=(decimal)claimProc.FeeBilled;
				List<Procedure> listProceduresLab=Procedures.GetCanadianLabFees(claimProc.ProcNum,listProcedures);
				for(int j=0;j<listProceduresLab.Count;j++) {
					feeBilledTotal+=(decimal)listProceduresLab[j].ProcFee;
				}				
				_text=feeBilledTotal.ToString("F");
				_documentGenerator.DrawString(g,_text,procedureTotalColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				totalSubmitted+=feeBilledTotal;
				_x=_documentGenerator.StartElement();
			}
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawField(g,_isFrench?"DESTINATAIRE DU PAIEMENT":"BENEFIT AMOUNT PAYABLE TO",payableToStr,true,_x,0);
			_text=_isFrench?"TOTAL DEMANDÉ ":"TOTAL SUBMITTED ";
			_documentGenerator.DrawString(g,_text,procedureTotalColumn-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
			_text=totalSubmitted.ToString("F");
			_documentGenerator.DrawString(g,_text,procedureTotalColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
		}

		///<summary>Prints the EOB header. Left in its own function, since the header is expected to be printed on each respective page of output.</summary>
		private void PrintEOBHeader(Graphics g){
			PrintCarrier(g);
			_x=_documentGenerator.StartElement();
			if(_hasPredetermination){
				if(_doPrintPatientCopy) {
					_text=_isFrench?"DÉTAIL DES PRESTATIONS D'UN PLAN DE TRAITEMENT - COPIE DU PATIENT":"PREDETERMINATION EXPLANATION OF BENEFITS - PATIENT COPY";
				}else{
					_text=_isFrench?"DÉTAIL DES PRESTATIONS D'UN PLAN DE TRAITEMENT - COPIE DE DENTISTE":"PREDETERMINATION EXPLANATION OF BENEFITS - DENTIST COPY";
				}
			}else{
				if(_doPrintPatientCopy){
					_text=_isFrench?"DÉTAIL DES PRESTATIONS - COPIE DU PATIENT":"EXPLANATION OF BENEFITS - PATIENT COPY";
				}else{
					_text=_isFrench?"DÉTAIL DES PRESTATIONS - COPIE DE DENTISTE":"EXPLANATION OF BENEFITS - DENTIST COPY";
				}
			}
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			float rightColumn=450;
			PrintVertificationNo(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintDentistName(g,_x,0);
			PrintTreatmentProviderID(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintOfficeSequenceNumber(g,_x,0);
			PrintTreatmentProviderOfficeNumber(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintPolicyNo(g,_x,0,true);
			PrintDivisionSectionNo(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintCertificateNo(g,_x,0,true);
			float midColumn=240;
			PrintPrimaryDependantNo(g,_x+midColumn,0);
			PrintCardNo(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintInsuredMember(g,_x,0);
			PrintSubscriberBirthday(g,_x+rightColumn,0,true);
			_x=_documentGenerator.StartElement();
			PrintPatientName(g,_x,0);
			PrintPatientBirthday(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintRelationshipToSubscriber(g,_x,0,true);
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
		}

		private void PrintEOB(Graphics g){
			_documentGenerator.standardFont=new Font(_documentGenerator.standardFont.FontFamily,8,FontStyle.Regular);
			PrintEOBHeader(g);
			_x=_documentGenerator.StartElement();
			PrintTransactionReferenceNumber(g,_x,0);
			PrintTransactionDate(g,_x+450,0);
			_x=_documentGenerator.StartElement();
			PrintProcedureListEOB(g);
			_x=_documentGenerator.StartElement();
			PrintPaymentSummary(g);
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_x=_documentGenerator.StartElement();
			if(_isFrench) {
				_text="Nous avons utilisé les renseignements du présent formulaire pour traiter votre demande par ordinateur.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
				_x=_documentGenerator.StartElement();
				_text="Veuillez en vérifier l'exactitude et aviser votre dentiste en cas d'erreur.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
				_x=_documentGenerator.StartElement();
				_text="Prière de ne pas poster à l'assureur/administrateur du régime.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
			}
			else {
				_text="The information contained on this form has been used to process your claim electronically.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
				_x=_documentGenerator.StartElement();
				_text="Please verify the accuracy of this data and report any discrepancies to your dental office.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
				_x=_documentGenerator.StartElement();
				_text="Do not mail this form to the insurer/plan administrator.";
				_documentGenerator.DrawString(g,_text,_documentGenerator.bounds.Left+_documentGenerator.bounds.Width/2-g.MeasureString(_text,_documentGenerator.standardFont).Width/2,0);
			}
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			if(!_hasPredetermination){
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,_isFrench?"RENSEIGNEMENTS SUR LE PATIENT:":"PATIENT INFORMATION:",_x,0);
				_x=_documentGenerator.StartElement();
				_bullet=1;
				PrintDependencyBullet(g);
				_x=_documentGenerator.StartElement();
				PrintSecondaryPolicyBullet(g);
				_x=_documentGenerator.StartElement();
				PrintAccidentBullet(g);
				_x=_documentGenerator.StartElement();
				PrintInitialPlacementBullet(g);
				_x=_documentGenerator.StartElement();
				PrintToothExtractionBullet(g);
				_x=_documentGenerator.StartElement();
				_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			}
			_x=_documentGenerator.StartElement();
			PrintNoteList(g);
			_x=_documentGenerator.StartElement();
			_documentGenerator.HorizontalLine(g,_penBreakLine,_documentGenerator.bounds.Left,_documentGenerator.bounds.Right,0);
			_x=_documentGenerator.StartElement();
			if(_isFrench){
				_text="La présente nous a été transmise par ordinateur par votre dentiste.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Veuillez la conserver.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Pour tout renseignement, veuillez vous adresser à "+
					(_hasPredetermination?"votre assureur.":"l'assureur/administrateur du régime.");
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Les honoraires non remboursables sont déductibles de l'impôt.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
			}
			else{
				_text="This "+(_hasPredetermination?"Predetermination":"Claim")+
					" Has Been Submitted Electronically on Your Behalf By Your Dentist.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Please Direct Any Inquiries to Your Insurer/Plan Administrator.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Expenses Not Payable May be Considered for Income Tax Purposes.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
				_text="Please Retain Copy.";
				_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
				_x=_documentGenerator.StartElement();
			}
		}

		private void PrintProcedureListEOB(Graphics g) {
			List<Procedure> listProcedures=Procedures.Refresh(_claim.PatNum);
			Font font=_documentGenerator.standardFont;
			_documentGenerator.standardFont=_fontStandardSmall;
			float procedureCodeColumn=_x;
			float procedureToothColumn=procedureCodeColumn+270;
			float procedureDateColumn=procedureToothColumn+25;
			float procedureDateColumnWidth=_hasPredetermination?0:75;
			float procedureChargeColumn=procedureDateColumn+procedureDateColumnWidth;
			float procedureEligibleColumn=procedureChargeColumn+60;
			float procedureDeductColumn=procedureEligibleColumn+60;
			float procedureAtColumn=procedureDeductColumn+60;
			float procedureBenefitColumn=procedureAtColumn+30;
			float procedureNotesColumn=procedureBenefitColumn+60;
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"ACTE":"PROCEDURE",procedureCodeColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"D#":"TH#",procedureToothColumn,0);
			if(!_hasPredetermination) {
				_documentGenerator.DrawString(g,"DATE",procedureDateColumn,0);//Same in both languages.
			}
			_documentGenerator.DrawString(g,_isFrench?"HONS":"CHARGE",procedureChargeColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"ADMIS":"ELIG",procedureEligibleColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"FRANCH":"DEDUCT",procedureDeductColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"%":"AT",procedureAtColumn,0);
			_documentGenerator.DrawString(g,_isFrench?"PREST":"BENEFIT",procedureBenefitColumn,0);
			_documentGenerator.DrawString(g,"NOTES",procedureNotesColumn,0);
			//First start by listing the procedures originally attached to the claim.
			CCDField[] ccdFieldsArrayProcedureLineNumbers=_ccdFieldInputterFormData.GetFieldsById("F07");
			CCDField[] ccdFieldsArrayEligibleAmounts=_ccdFieldInputterFormData.GetFieldsById("G12");
			CCDField[] ccdFieldsArrayDeductibleAmounts=_ccdFieldInputterFormData.GetFieldsById("G13");
			CCDField[] ccdFieldsArrayEligiblePercentage=_ccdFieldInputterFormData.GetFieldsById("G14");
			CCDField[] ccdFieldsArrayBenefitAmountForTheProcedures=_ccdFieldInputterFormData.GetFieldsById("G15");
			CCDField[] ccdFieldsArrayExplainationNotes1=_ccdFieldInputterFormData.GetFieldsById("G16");
			CCDField[] ccdFieldsArrayExplainationNotes2=_ccdFieldInputterFormData.GetFieldsById("G17");
			CCDField[] ccdFieldsArrayEligibleLabAmounts1=_ccdFieldInputterFormData.GetFieldsById("G43");
			CCDField[] ccdFieldsArrayDeductibleLabAmounts1=_ccdFieldInputterFormData.GetFieldsById("G56");
			CCDField[] ccdFieldsArrayEligibleLabPercentage1=_ccdFieldInputterFormData.GetFieldsById("G57");
			CCDField[] ccdFieldsArrayBenefitLabAmount1=_ccdFieldInputterFormData.GetFieldsById("G58");
			CCDField[] ccdFieldsArrayEligibleLabAmounts2=_ccdFieldInputterFormData.GetFieldsById("G02");
			CCDField[] ccdFieldsArrayDeductibleLabAmounts2=_ccdFieldInputterFormData.GetFieldsById("G59");
			CCDField[] ccdFieldsArrayEligibleLabPercentage2=_ccdFieldInputterFormData.GetFieldsById("G60");
			CCDField[] ccdFieldsArrayBenefitLabAmount2=_ccdFieldInputterFormData.GetFieldsById("G61");
			Procedure procedure;
			float amountWidth=g.MeasureString("****.**",_documentGenerator.standardFont).Width;
			float procCodeWidth=g.MeasureString("*******",_documentGenerator.standardFont).Width;
			for(int p=0;p<ccdFieldsArrayProcedureLineNumbers.Length;p++){
				int procedureLineNumber=Convert.ToInt32(ccdFieldsArrayProcedureLineNumbers[p].valuestr);
				int i=0;
				while(i<_listClaimProcs.Count && _listClaimProcs[i].LineNumber!=procedureLineNumber){
					i++;
				}
				//List the current procedure.
				ClaimProc claimproc=_listClaimProcs[i];
				_x=_documentGenerator.StartElement();
				procedure=Procedures.GetOneProc(claimproc.ProcNum,true);
				_text=claimproc.CodeSent.PadLeft(6,' ');
				_documentGenerator.DrawString(g,_text,_x,0);//procedure code
				_text=ProcedureCodes.GetProcCode(procedure.CodeNum).Descript;
				_documentGenerator.DrawString(g,_text,procedureCodeColumn+procCodeWidth,0,_documentGenerator.standardFont,(int)(procedureToothColumn-procedureCodeColumn-procCodeWidth-10));//proc descript
				_text=Tooth.ToInternat(procedure.ToothNum);//Field F10
				_documentGenerator.DrawString(g,_text,procedureToothColumn,0);//Tooth number
				if(!_hasPredetermination) {//Used to remove service dates in a predetermination ack.
					_text=procedure.ProcDate.ToShortDateString();//Field F09
					_documentGenerator.DrawString(g,_text,procedureDateColumn,0);//Procedure date.
				}
				_text=claimproc.FeeBilled.ToString("F");
				_documentGenerator.DrawString(g,_text,procedureChargeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleAmounts[p].valuestr);//Field G12
				_documentGenerator.DrawString(g,_text,procedureEligibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleAmounts[p].valuestr);//Field G13
				_documentGenerator.DrawString(g,_text,procedureDeductColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text=RawPercentToDisplayPercent(ccdFieldsArrayEligiblePercentage[p].valuestr);//Field G14
				_documentGenerator.DrawString(g,_text,procedureAtColumn,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayBenefitAmountForTheProcedures[p].valuestr);//Field G15
				_documentGenerator.DrawString(g,_text,procedureBenefitColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text="";
				if(ccdFieldsArrayExplainationNotes1[p].valuestr!="00"){
					_text+=ccdFieldsArrayExplainationNotes1[p].valuestr;
				}
				if(ccdFieldsArrayExplainationNotes2[p].valuestr!="00"){
					if(_text.Length>0){
						_text+=",";
					}
					_text+=ccdFieldsArrayExplainationNotes2[p].valuestr;
				}
				_documentGenerator.DrawString(g,_text,procedureNotesColumn,0,_documentGenerator.standardFont,(int)(_documentGenerator.bounds.Right-procedureNotesColumn));
				List<Procedure> listProceduresLab=Procedures.GetCanadianLabFees(procedure.ProcNum,listProcedures);
				if(listProceduresLab.Count > 0 && ccdFieldsArrayEligibleLabAmounts1.Length > 0) {//In version 2 the lab fee is rolled into the procedure amount and so there is no lab section.
					//List the first lab info for the current procedure on its own line.
					_x=_documentGenerator.StartElement();
					_text=ProcedureCodes.GetProcCodeFromDb(listProceduresLab[0].CodeNum).ProcCode.PadLeft(6,' ');
					_documentGenerator.DrawString(g,_text,procedureCodeColumn,0);//proc code
					_text=listProceduresLab[0].ProcFee.ToString("F");
					_documentGenerator.DrawString(g,_text,procedureChargeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);//proc fee
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleLabAmounts1[i].valuestr);//G43
					_documentGenerator.DrawString(g,_text,procedureEligibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleLabAmounts1[i].valuestr);//G56
					_documentGenerator.DrawString(g,_text,procedureDeductColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					_text=RawPercentToDisplayPercent(ccdFieldsArrayEligibleLabPercentage1[i].valuestr);//G57
					_documentGenerator.DrawString(g,_text,procedureAtColumn,0);
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayBenefitLabAmount1[i].valuestr);//G58
					_documentGenerator.DrawString(g,_text,procedureBenefitColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				}
				if(listProceduresLab.Count > 1 && ccdFieldsArrayEligibleLabAmounts2.Length > 0) {//In version 2 the lab fee is rolled into the procedure amount and so there is no lab section.
					//List the second lab info for the current procedure on its own line.
					_x=_documentGenerator.StartElement();
					_text=ProcedureCodes.GetProcCodeFromDb(listProceduresLab[1].CodeNum).ProcCode.PadLeft(6,' ');
					_documentGenerator.DrawString(g,_text,procedureCodeColumn,0);//proc code
					_text=listProceduresLab[1].ProcFee.ToString("F");
					_documentGenerator.DrawString(g,_text,procedureChargeColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);//proc fee
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayEligibleLabAmounts2[i].valuestr);//G02
					_documentGenerator.DrawString(g,_text,procedureEligibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayDeductibleLabAmounts2[i].valuestr);//G59
					_documentGenerator.DrawString(g,_text,procedureDeductColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
					_text=RawPercentToDisplayPercent(ccdFieldsArrayEligibleLabPercentage2[i].valuestr);//G60
					_documentGenerator.DrawString(g,_text,procedureAtColumn,0);
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayBenefitLabAmount2[i].valuestr);//G61
					_documentGenerator.DrawString(g,_text,procedureBenefitColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				}
			}
			//List the carrier inserted procedures into the procedure list.
			CCDField[] ccdFieldsArrayCarrierProcs=_ccdFieldInputterFormData.GetFieldsById("G19");
			CCDField[] ccdFieldsArrayCarrierEligibleAmts=_ccdFieldInputterFormData.GetFieldsById("G20");
			CCDField[] ccdFieldsArrayCarrierEligibleLabAmts=_ccdFieldInputterFormData.GetFieldsById("G44");
			CCDField[] ccdFieldsArrayCarrierDeductAmts=_ccdFieldInputterFormData.GetFieldsById("G21");
			CCDField[] ccdFieldsArrayCarrierAts=_ccdFieldInputterFormData.GetFieldsById("G22");
			CCDField[] ccdFieldsArrayCarrierBenefitAmts=_ccdFieldInputterFormData.GetFieldsById("G23");
			CCDField[] ccdFieldsArrayCarrierNotes1=_ccdFieldInputterFormData.GetFieldsById("G24");
			CCDField[] ccdFieldsArrayCarrierNotes2=_ccdFieldInputterFormData.GetFieldsById("G25");
			for(int p=0;p<ccdFieldsArrayCarrierProcs.Length;p++){
				//Display the eligible proc info.
				_x=_documentGenerator.StartElement();
				_text=ccdFieldsArrayCarrierProcs[p].valuestr.PadLeft(6,' ');//Field G19
				_documentGenerator.DrawString(g,_text,_x,0);
				_text=ProcedureCodes.GetProcCode(ProcedureCodes.GetCodeNum(_text)).Descript;
				_documentGenerator.DrawString(g,_text,procedureCodeColumn+procCodeWidth,0,_documentGenerator.standardFont,(int)(procedureToothColumn-procedureCodeColumn-procCodeWidth-10));
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierEligibleAmts[p].valuestr);//Field G20
				_documentGenerator.DrawString(g,_text,procedureEligibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierDeductAmts[p].valuestr);//Field G21
				_documentGenerator.DrawString(g,_text,procedureDeductColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text=RawPercentToDisplayPercent(ccdFieldsArrayCarrierAts[p].valuestr);//Field G22
				_documentGenerator.DrawString(g,_text,procedureAtColumn,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierBenefitAmts[p].valuestr);//Field G23
				_documentGenerator.DrawString(g,_text,procedureBenefitColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_text="";
				if(ccdFieldsArrayCarrierNotes1[p].valuestr!="00"){
					_text+=ccdFieldsArrayCarrierNotes1[p].valuestr;
				}
				if(ccdFieldsArrayCarrierNotes2[p].valuestr!="00"){
					if(_text.Length>0){
						_text+=",";
					}
					_text+=ccdFieldsArrayCarrierNotes2[p].valuestr;
				}
				_documentGenerator.DrawString(g,_text,procedureNotesColumn,0);
				//Display the eligible lab info for the proc but on a separate line.
				_x=_documentGenerator.StartElement();
				if(ccdFieldsArrayCarrierEligibleLabAmts.Length>0) {
					_text="LAB(S)";
					_documentGenerator.DrawString(g,_text,procedureCodeColumn,0);
					_text=RawMoneyStrToDisplayMoney(ccdFieldsArrayCarrierEligibleLabAmts[p].valuestr);
					_documentGenerator.DrawString(g,_text,procedureEligibleColumn+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				}
			}
			//Handle the unallocated deductible amount if it exists. 
			//This happens when a carrier will not supply deductibles on a procedural basis.
			string unallocatedDeductible=_ccdFieldInputterFormData.GetFieldById("G29").valuestr;
			if(unallocatedDeductible!="000000") {
				_x=_documentGenerator.StartElement();
				_text=_isFrench?"Total Franchise":"Total Deductible";
				_documentGenerator.DrawString(g,_text,_x+70,0);
				_text=RawMoneyStrToDisplayMoney(unallocatedDeductible);
				_documentGenerator.DrawString(g,_text,procedureDeductColumn,0);
				_text="-"+_text;
				_documentGenerator.DrawString(g,_text,procedureBenefitColumn,0);
			}
			_documentGenerator.standardFont=font;
		}

		private void PrintReversalResponse_12(Graphics g) {
			PrintCarrier(g);
			_x=_documentGenerator.StartElement();
			_text=(_isFrench?"Réponse d'annulation de réclamation":"CLAIM REVERSAL RESPONSE").ToUpper();
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
			_x=_documentGenerator.StartElement();
			PrintStatus(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintDisposition(g,_x,0);
			_x=_documentGenerator.StartElement();
			PrintTransactionReferenceNumber(g,_x,0);
			_x=_documentGenerator.StartElement();
			_text=_isFrench?"MONTANT TOTAL DE SERVICE: ":"TOTAL AMOUNT OF SERVICE: ";
			_x+=_documentGenerator.DrawString(g,_text,_x,0).Width+10;
			_text=RawMoneyStrToDisplayMoney(_ccdFieldInputterFormData.GetFieldById("G04").valuestr);
			_documentGenerator.DrawString(g,_text,_x,0);
			_x=_documentGenerator.StartElement();
			float rightColumn=450;
			PrintDentistName(g,_x,0);
			PrintTreatmentProviderID(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			PrintOfficeSequenceNumber(g,_x,0);
			PrintTreatmentProviderOfficeNumber(g,_x+rightColumn,0);
			_x=_documentGenerator.StartElement();
			_x=_documentGenerator.StartElement();	
			PrintErrorList(g);	
		}

		#endregion

		#region Printing Helper Functions

		///<summary>Prints carrier name centered on current form row followed by a space.</summary>
		private void PrintCarrier(Graphics g){
			_text=_carrier.CarrierName;
			_documentGenerator.DrawString(g,_text,_center-g.MeasureString(_text,_fontHeading).Width/2,0,_fontHeading);
		}

		///<summary>For EOBs only.</summary>
		private SizeF PrintVertificationNo(Graphics g,float X,float Y){
			CCDField ccdFieldVertificationNo=_ccdFieldInputterFormData.GetFieldById("G30");//Present in EOBs.
			return _documentGenerator.DrawField(g,ccdFieldVertificationNo.GetFieldName(_isFrench),ccdFieldVertificationNo.valuestr,false,X,Y);
		}

		private SizeF PrintTransactionDate(Graphics g,float X,float Y){
			_text=DateToString(_etrans.DateTimeTrans);
			return _documentGenerator.DrawField(g,_isFrench?"DATE":"DATE SUBMITTED",_text,true,X,Y);
		}

		///<summary>Corresponds to field G01.</summary>
		private SizeF PrintTransactionReferenceNumber(Graphics g,float X,float Y){
			CCDField[] ccdFieldCarrierClaimNos=_ccdFieldInputterFormData.GetFieldsById("G01");
			if(ccdFieldCarrierClaimNos==null || ccdFieldCarrierClaimNos.Length==0){
				throw new Exception("Field G01 does not exist in transaction, cannot print carrier claim number.");
			}
			return _documentGenerator.DrawField(g,_isFrench?"NO DE RÉFÉRENCE DE TRANSACTION":"CARRIER CLAIM NO",ccdFieldCarrierClaimNos[0].valuestr,false,X,Y);
		}

		private SizeF PrintDisposition(Graphics g,float X,float Y) {
			CCDField ccdFieldDisposition=_ccdFieldInputterFormData.GetFieldById("G07");
			return _documentGenerator.DrawField(g,ccdFieldDisposition.GetFieldName(_isFrench),ccdFieldDisposition.valuestr,false,X,Y);
		}

		private SizeF PrintStatus(Graphics g,float X,float Y) {
			CCDField ccdFieldStatus=_ccdFieldInputterFormData.GetFieldById("G05");
			string statusStr="";
			if(ccdFieldStatus!=null) {
				if(_isFrench) {
					switch(ccdFieldStatus.valuestr) {
						default:
							statusStr="";
							break;
						case ("A"):
							statusStr="La transaction a été accepté.";
							break;
						case ("E"):
							statusStr="Le patient est éligible.";
							break;
						case ("R"):
							statusStr="Transaction rejeté due aux erreurs.";
							break;
						case ("H"):
							statusStr="Transaction a été reçu par l'assureur et est détenu pour un traitement ultérieur. Réponse ne sera pas renvoyé chez le dentiste par voie électronique.";
							break;
						case ("B"):
							statusStr="Transaction a été reçu par le réseau et sera transmit en lot à l'assureur pour traitement ultérieur. Réponse ne sera pas renvoyé chez le dentiste par voie électronique.";
							break;
						case ("C"):
							statusStr="Transaction a été reçu par l'assureur et est détenu pour un traitement ultérieur. Réponse sera peut-être renvoyé chez le dentiste par voie électronique et obtenu par une demande pour les réponses en suspend.";
							break;
						case ("N"):
							statusStr="Transaction a été reçu par le réseau et sera transmit en lot à l'assureur pour traitement ultérieur.  Réponse sera peut-être renvoyé chez le dentiste par voie électronique et obtenu par une demande pour les réponses en suspend.";
							break;
						case ("M"):
							statusStr="Un formulaire de transaction manuelle devrait être soumise par le patient ou le cabinet dentaire.";
							break;
						case ("X"):
							statusStr="Aucunes réponses en suspend à suivre.";
							break;
					}
				}
				else {
					switch(ccdFieldStatus.valuestr) {
						default:
							statusStr="";
							break;
						case ("A"):
							statusStr="Transaction has been accepted.";
							break;
						case ("E"):
							statusStr="The patient is eligible.";
							break;
						case ("R"):
							statusStr="Claim is rejected because of errors.";
							break;
						case ("H"):
							statusStr="Claim is received successfully by the carrier and is held for further processing. Response will NOT be sent back to the dentist electronically.";
							break;
						case ("B"):
							statusStr="Claim is received successfully by the network and will be batch-forwarded on to the carrier for further processing. Response will NOT be sent back to the dentist electronically.";
							break;
						case ("C"):
							statusStr="Claim is received successfully by the carrier and is held for further processing. Response may be sent back to the dentist electronically and retrievable via ROT.";
							break;
						case ("N"):
							statusStr="Claim is received successfully by the network and will be batch forwarded onto the carrier for further processing. Response may be sent back to the dentist electronically and retrievable via ROT.";
							break;
						case ("M"):
							statusStr="Manual claim form should be submitted by the patient or the dental office.";
							break;
						case ("X"):
							statusStr="No more outstanding responses to follow.";
							break;
					}
				}
			}
			return _documentGenerator.DrawField(g,_isFrench?"STATUT":"STATUS",statusStr,true,X,Y);
		}

		private SizeF PrintComment(Graphics g,float X,float Y) {
			string comment="";
			CCDField ccdFieldComment=_ccdFieldInputterFormData.GetFieldById("G07");
			if(ccdFieldComment!=null){//The disposition message is not always present.
				comment=ccdFieldComment.valuestr;
			}
			return _documentGenerator.DrawField(g,_isFrench?"COMMENTAIRES":"COMMENT",comment,false,X,Y);
		}

		private SizeF PrintDentistName(Graphics g,float X,float Y) {
			//Treatment provider should match that retrieved from the CDA provider number in field B01.
			_text=_providerTreat.LName+", "+_providerTreat.FName+" "+_providerTreat.MI+" "+_providerTreat.Suffix;
			return _documentGenerator.DrawField(g,_isFrench?"DENTISTE":"DENTIST",_text,false,X,Y);
		}

		private SizeF PrintDentistPhone(Graphics g,float X,float Y){
			_text=PrefC.GetString(PrefName.PracticePhone);
			if(_text.Length==10) {//May need to format for nice appearance.
				_text=_text.Substring(0,3)+"-"+_text.Substring(3,3)+"-"+_text.Substring(6,4);
			}
			return _documentGenerator.DrawField(g,_isFrench?"NO DE TÉLÉPHONE":"TELEPHONE",_text,true,X,Y);
		}

		///<summary>The output will be no wider than maxWidthInPixels, unless maxWidthInPixels<=0, in which case there is no maximum width.</summary>
		private SizeF PrintDentistAddress(Graphics g,float X,float Y,float maxWidthInPixels){
			SizeF sizeF1=_documentGenerator.DrawString(g,_isFrench?"ADRESSE: ":"ADDRESS: ",X,Y);
			SizeF sizeF2=PrintAddress(g,X+sizeF1.Width,Y,PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),
				PrefC.GetString(PrefName.PracticeCity)+", "+PrefC.GetString(PrefName.PracticeST)+" "+PrefC.GetString(PrefName.PracticeZip),150f,maxWidthInPixels);
			return new SizeF(sizeF1.Width+sizeF2.Width,Math.Max(sizeF1.Height,sizeF2.Height));
		}

		///<summary>Corresponds to field B01.</summary>
		private SizeF PrintTreatmentProviderID(Graphics g,float X,float Y) {
			CCDField ccdFieldTreatmentProviderID=_ccdFieldInputterFormData.GetFieldById("B01");
			return _documentGenerator.DrawField(g,ccdFieldTreatmentProviderID.GetFieldName(_isFrench),ccdFieldTreatmentProviderID.valuestr,false,X,Y);
		}

		///<summary>Corresponds to field B02.</summary>
		private SizeF PrintTreatmentProviderOfficeNumber(Graphics g,float X,float Y) {
			CCDField ccdFieldCdaOfficeNumber=_ccdFieldInputterFormData.GetFieldById("B02");
			return _documentGenerator.DrawField(g,ccdFieldCdaOfficeNumber.GetFieldName(_isFrench),ccdFieldCdaOfficeNumber.valuestr,false,X,Y);
		}

		///<summary>Corresponds to field A02.</summary>
		private SizeF PrintOfficeSequenceNumber(Graphics g,float X,float Y) {
			CCDField[] ccdFieldOfficeSequenceNumbers=_ccdFieldInputterFormData.GetFieldsById("A02");
			if(ccdFieldOfficeSequenceNumbers==null || ccdFieldOfficeSequenceNumbers.Length==0){
				throw new Exception("There are no instances of field A02 to read, cannot print dental office claim reference number.");
			}
			return _documentGenerator.DrawField(g,ccdFieldOfficeSequenceNumbers[0].GetFieldName(_isFrench),
				ccdFieldOfficeSequenceNumbers[0].valuestr,false,X,Y);
		}

		private SizeF PrintPatientName(Graphics g,float X,float Y) {
			return _documentGenerator.DrawField(g,"PATIENT",_patient.GetNameFLFormal(),true,X,Y);//Fields C06,C07,C08
		}

		private SizeF PrintPatientBirthday(Graphics g,float X,float Y) {
			_text=DateToString(_patient.Birthdate);
			return _documentGenerator.DrawField(g,_isFrench?"DATE DE NAISSANCE":"BIRTHDATE",_text,true,X,Y);//Field C05
		}

		private SizeF PrintPatientSex(Graphics g,float X,float Y){
			switch(_patient.Gender){
				case PatientGender.Male:
					_text="M";
					break;
				default:
					_text="F";
					break;
			}
			return _documentGenerator.DrawField(g,_isFrench?"SEXE":"SEX",_text,true,X,Y);
		}

		private SizeF PrintPolicyNo(Graphics g,float X,float Y,bool isPrimary){
			_text="";
			if(isPrimary){
				_text=_insplan.GroupNum;//Field C01
			}
			else if(_insplan2!=null){
				_text=_insplan2.GroupNum;//Field E02
			}
			return _documentGenerator.DrawField(g,_isFrench?"NO DE POLICE":"POLICY#",_text,true,X,Y);
		}

		private SizeF PrintDivisionSectionNo(Graphics g,float X,float Y){
			return _documentGenerator.DrawField(g,_isFrench?"NO DE DIVISION/SECTION":"DIVISION/SECTION NO",_insplan.DivisionNo,true,X,Y);//Field C11
		}

		private SizeF PrintCertificateNo(Graphics g,float X,float Y,bool isPrimary) {
			if(isPrimary){
				_text=_insSub.SubscriberID;//Field C02
			}
			else if(_insplan2!=null) {
				_text=_insSub2.SubscriberID;//Field E03
			}
			//The instructions for how to deal with NIHB plans came from the version 4.1 Message Formats Document, described in the data dictionary near field C02.
			CCDField ccdFieldC12=_ccdFieldInputterFormData.GetFieldById("C12");//Plan flag. 'N' for NIHB, 'A' for Newfoundland MCP Plan - Provincial Medical Plan, 'V' for Veteran's Affairs Plan. There may be other values.
			CCDField ccdFieldC13=_ccdFieldInputterFormData.GetFieldById("C13");//Band number for NIHB plans.
			CCDField ccdFieldC14=_ccdFieldInputterFormData.GetFieldById("C14");//Family number for NIHB plans.
			//NIHB stands for "Non-Insured Health Benefits" as defined at http://www.hc-sc.gc.ca/fniah-spnia/nihb-ssna/index-eng.php. Government based program.
			//For NIHB claims, print the Band (Field C13) and Family (Field C14) numbers as required.
			//If they have NIHB, then it is probably their primary and they probably don't have any other plan.
			if(ccdFieldC12!=null && ccdFieldC12.valuestr=="N" && ccdFieldC13!=null && ccdFieldC13.valuestr.Trim()!="" && ccdFieldC14!=null && ccdFieldC14.valuestr.Trim()!="") {
				return _documentGenerator.DrawString(g,_isFrench?("BANDE: "+ccdFieldC13.valuestr+"  FAMILLE: "+ccdFieldC14.valuestr):("BAND: "+ccdFieldC13.valuestr+"  FAMILY: "+ccdFieldC14.valuestr),X,Y);
			}
			return _documentGenerator.DrawField(g,_isFrench?"NO DE CERTIFICAT":"CERTIFICATE NO",_text,true,X,Y);
		}

		///<summary>Print "sequence" number.</summary>
		private SizeF PrintCardNo(Graphics g,float X,float Y) {
			_text=(_insplan.DentaideCardSequence.ToString());//Field D11
			return _documentGenerator.DrawField(g,_isFrench?"NO DE CARTE":"CARD NO",_text=="0"?"":_text,true,X,Y);
		}

		private SizeF PrintPrimaryDependantNo(Graphics g,float X,float Y) {
			return PrintPrimaryDependantNo(g,X,Y,"DEPENDANT NO","NO DE PERSONNE À CHARGE");
		}

		private SizeF PrintPrimaryDependantNo(Graphics g,float X,float Y,string fieldText,string frenchFieldText){
			string patId="";
			if(_patPlanPri!=null) {
				patId=_patPlanPri.PatID;
			}
			return _documentGenerator.DrawField(g,_isFrench?frenchFieldText:fieldText,patId,true,X,Y);
		}

		private SizeF PrintSecondaryDependantNo(Graphics g,float X,float Y) {
			return PrintSecondaryDependantNo(g,X,Y,"DEPENDANT NO","NO DE PERSONNE À CHARGE");
		}

		private SizeF PrintSecondaryDependantNo(Graphics g,float X,float Y,string fieldText,string frenchFieldText){
			return _documentGenerator.DrawField(g,_isFrench?frenchFieldText:fieldText,_patPlanSec.PatID,true,X,Y);
		}

		private SizeF PrintInsuredMember(Graphics g,float X,float Y){
			_text=_patientSubscriber.GetNameFLFormal();
			return _documentGenerator.DrawField(g,_isFrench?"TITULAIRE":"INSURED/MEMBER",_text,true,X,Y);//Fields D02,D03,D04
		}

		///<summary>The output will be no wider than maxWidthInPixels, unless maxWidthInPixels<=0, in which case there is no maximum width.</summary>
		private SizeF PrintSubscriberAddress(Graphics g,float X,float Y,bool isPrimary,float maxWidthInPixels) {
			string line1="";
			string line2="";
			string line3="";
			Patient patientSubscriber=isPrimary?_patientSubscriber:_patientSubscriber2;
			if(patientSubscriber!=null){
				//Primary: Fields D05,D06,D07,D08,D09
				//Secondary: Fields E11,E12,E13,E14,E15
				line1=patientSubscriber.Address;
				line2=patientSubscriber.Address2;
				line3=patientSubscriber.City+", "+patientSubscriber.State+" "+patientSubscriber.Zip;
			}
			return PrintAddress(g,X,Y,line1,line2,line3,maxWidthInPixels,maxWidthInPixels);
		}

		///<summary>If maxCharsPerLine>0, then the lines which are excess in length are truncated to the value specified.</summary>
		private SizeF PrintInsuredAddress(Graphics g,float X,float Y,bool isPrimary,int maxCharsPerLine) {
			SizeF sizeF1=_documentGenerator.DrawString(g,_isFrench?"ADRESSE DU TITULAIRE: ":"INSURED/MEMBER ADDRESS: ",X,Y);
			SizeF sizeF2=PrintSubscriberAddress(g,X+sizeF1.Width,Y,isPrimary,maxCharsPerLine);
			return new SizeF(sizeF1.Width+sizeF2.Width,Math.Max(sizeF1.Height,sizeF2.Height));
		}

		///<summary>Pulls the relationship from the claim if not null. Otherwise pulls the claim from the primary patinet plan. If both are null, Self is returned.</summary>
		private Relat GetRelationshipToSubscriber() {
			if(_claim!=null) {
				return _claim.PatRelat;
			}
			else if(_patPlanPri!=null) {
				return _patPlanPri.Relationship;
			}
			return Relat.Self;
		}

		///<summary>Corresponds to field C03.</summary>
		private SizeF PrintRelationshipToSubscriber(Graphics g,float X,float Y,bool doUseCaps) {
			_text=GetPatientRelationshipString(GetRelationshipToSubscriber());
			string engStr="RELATIONSHIP TO INSURED/MEMBER";
			string frStr="PARENTÉ AVEC TITULAIRE";
			string label=_isFrench?frStr:engStr;
			return _documentGenerator.DrawField(g,doUseCaps?label.ToUpper():label,_text,true,X,Y);
		}

		private SizeF PrintSubscriberBirthday(Graphics g,float X,float Y,bool doUseCaps) {
			_text=DateToString(_patientSubscriber.Birthdate);
			string engStr="Birthdate";
			string frStr="Date de naissance";
			string label=_isFrench?frStr:engStr;
			return _documentGenerator.DrawField(g,doUseCaps?label.ToUpper():label,_text,true,X,Y);
		}

		///<summary>Prints a three-line address. Each line is underlined and the address is printed without a label. 
		///The output will be no wider than maxWidthInPixels, unless maxWidthInPixels<=0, in which case there is no maximum width.</summary>
		private SizeF PrintAddress(Graphics g,float X,float Y,string line1,string line2,string line3,float minWidthInPixels,float maxWidthInPixels) {
			line1=GetTruncatedString(g,_documentGenerator.standardFont,line1,maxWidthInPixels);
			line2=GetTruncatedString(g,_documentGenerator.standardFont,line2,maxWidthInPixels);
			line3=GetTruncatedString(g,_documentGenerator.standardFont,line3,maxWidthInPixels);
			string address=line1+"\n"+line2+"\n"+line3;
			float lineWidth=Math.Max(minWidthInPixels,g.MeasureString(address,_documentGenerator.standardFont).Width);
			float yoff=0;
			_documentGenerator.DrawString(g,line1,X,Y+yoff,_documentGenerator.standardFont);
			yoff+=_verticalLine;
			yoff+=_documentGenerator.HorizontalLine(g,Pens.Black,X,X+lineWidth,yoff).Height;
			_documentGenerator.DrawString(g,line2,X,Y+yoff,_documentGenerator.standardFont);
			yoff+=_verticalLine;
			yoff+=_documentGenerator.HorizontalLine(g,Pens.Black,X,X+lineWidth,yoff).Height;
			_documentGenerator.DrawString(g,line3,X,Y+yoff,_documentGenerator.standardFont);
			yoff+=_verticalLine;
			yoff+=_documentGenerator.HorizontalLine(g,Pens.Black,X,X+lineWidth,yoff).Height;
			return new SizeF(lineWidth,yoff);
		}

		///<summary>If the specified string is wider than maxWidthInPixels on graphics object g in the specified font, 
		///then the longest substring of str is returned which is less than or equal to maxWidthInPixels in width, 
		///starting from the first character in str. If maxWidthInPixels<=0 then str is returned without modification.
		///In all other cases, str is returned without modification.</summary>
		private string GetTruncatedString(Graphics g,Font font,string str,float maxWidthInPixels){
			if(maxWidthInPixels<=0){
				return str;
			}
			string result=str;
			SizeF sizeFStr=g.MeasureString(result,font);
			//Reduce the size of the string until it fits within maxWidthInPixels.
			while(result.Length>0 && sizeFStr.Width>maxWidthInPixels){
				//Remove the last character from the string.
				result=result.Substring(0,result.Length-1);
				//Set the last 3 characters in the string to '.' if possible.
				int chrsToReplace=Math.Min(3,result.Length);
				result=result.Substring(0,result.Length-chrsToReplace)+("".PadRight(chrsToReplace,'.'));
				//Remeasure the new result and check again.
				sizeFStr=g.MeasureString(result,font);
			}
			return result;
		}

		private bool PrintDependencies(Graphics g,bool doFillOut){
			string strIsStudent="   ";
			string isHandicapped="   ";
			bool isStudent=false;
			_text="";//Used for school name.
			if(doFillOut) {
				switch(_patient.CanadianEligibilityCode) {//Field C09
					case 1://Patient is a full-time student.
						strIsStudent=_isFrench?"Oui":"Yes";
						isStudent=true;
						_text=_patient.SchoolName;
						break;
					case 2://Patient is disabled.
						isHandicapped=_isFrench?"Oui":"Yes";
						break;
					case 3://Patient is a disabled student.
						strIsStudent=_isFrench?"Oui":"Yes";
						isStudent=true;
						_text=_patient.SchoolName;
						isHandicapped=_isFrench?"Oui":"Yes";
						break;
					default:
						break;
				}
			}
			_documentGenerator.PushX(_x);
			_x+=_documentGenerator.DrawString(g,_isFrench?"Personne à charge: Étudiant":"If dependant, indicate: Student",_x,0).Width;
			float isStudentHeight=_documentGenerator.DrawString(g,strIsStudent,_x,0).Height;
			//Spaces don't show up with underline, so we will have to underline manually.
			float underlineWidth=g.MeasureString("***",_documentGenerator.standardFont).Width;
			_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+underlineWidth,isStudentHeight);
			_x+=underlineWidth;
			_x+=_documentGenerator.DrawString(g,_isFrench?" Handicapé":" Handicapped",_x,0).Width;
			float isHandicappedHeight=_documentGenerator.DrawString(g,isHandicapped,_x,0).Height;
			_documentGenerator.HorizontalLine(g,Pens.Black,_x,_x+underlineWidth,isHandicappedHeight);
			_x=_documentGenerator.PopX();
			return isStudent;
		}

		private void PrintDependencyBullet(Graphics g) {
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Save indentation x-value for this list number.
			PrintRelationshipToSubscriber(g,_x,0,false);
			PrintSubscriberBirthday(g,_x+360,0,false);
			_x=_documentGenerator.StartElement();
			PrintDependencies(g,true);
			_documentGenerator.DrawField(g,_isFrench?"Si étudiant nom de l'école":"If Student, Name of student's school",_patient.SchoolName,true,_x+360,0);
			_x=_documentGenerator.PopX();//End indentation.
		}

		private void PrintSecondaryPolicyBullet(Graphics g){
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Save indentation spot for this bullet point.
			if(_isFrench) {
				_text="A-t-il droit à des prestations ou services dans un autre régime dentaire, régime collectif ou gouvernemental? ";
			}
			else {
				_text="Are any Dental Benefits or services provided under any other group insurance or dental plan, WCB or gov’t plan? ";
			}
			//Only print secondary coverage information on the primary claim report.
			if(ThisIsPrimary() && _insplan2!=null){
				_documentGenerator.DrawString(g,_text+(_isFrench?"Oui":"Yes"),_x,0);
				_x=_documentGenerator.StartElement();
				PrintPolicyNo(g,_x,0,false);
				_text=_carrierOther.CarrierName;
				_documentGenerator.DrawField(g,_isFrench?"Nom de l'assureur/administrateur":"Name of Insurer/Plan Administrator",_text,true,_x+200,0);
				_x=_documentGenerator.StartElement();
				PrintCertificateNo(g,_x,0,false);
				PrintSecondaryDependantNo(g,_x+200,0);
				_text=DateToString(_patientSubscriber2.Birthdate);//Field E04
				_documentGenerator.DrawField(g,_isFrench?"Date de naissance du titulaire":"Insured/Member Date of Birth",_text,true,_x+400,0);			
				_x=_documentGenerator.StartElement();
				_text=GetPatientRelationshipString(_patPlanSec.Relationship);//Field E06
				_documentGenerator.DrawField(g,_isFrench?"Parenté avec patient":"Relationship to Patient",_text,true,_x,0);
			}
			else{
				_documentGenerator.DrawString(g,_text+(_isFrench?"Non":"No"),_x,0);
			}
			_x=_documentGenerator.PopX();//End indentation.
		}

		private void PrintAccidentBullet(Graphics g){
			PrintAccidentBullet(g,_isFrench?"Y-a-t-il un traitement requis par suite d'un accident?":"Is any treatment required as the result of an accident?");
		}

		private void PrintAccidentBullet(Graphics g,string questionStr){
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Begin indentation.
			_x+=_documentGenerator.DrawString(g,questionStr+" ",_x,0).Width;
			bool isAccident=(_claim!=null && IsValidDate(_claim.AccidentDate));
			if(!isAccident) {//Field F02 - No accident claimed.
				_documentGenerator.DrawString(g,_isFrench?"Non":"No",_x,0);
			}
			else {
				_documentGenerator.DrawString(g,_isFrench?"Oui":"Yes",_x,0);
			}
			_x=_documentGenerator.StartElement();
			_x+=_documentGenerator.DrawField(g,_isFrench?"Si Oui, donner date":"If yes, give date",(isAccident?DateToString(_claim.AccidentDate):"________")+" ",true,_x,0).Width;
			_documentGenerator.DrawString(g,_isFrench?"et détails à part:":"and details separately:",_x,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,isAccident?_claim.ClaimNote:"",_x,0);
			_x=_documentGenerator.PopX();//End indentation.
		}

		private void PrintInitialPlacementBullet(Graphics g){
			string initialPlacementUpper="X";
			CCDField ccdFieldInitialPlacementUpper=_ccdFieldInputterFormData.GetFieldById("F15");
			if(ccdFieldInitialPlacementUpper!=null) {
				initialPlacementUpper=ccdFieldInitialPlacementUpper.valuestr;
			}
			else if(_claim!=null) {
				initialPlacementUpper=_claim.CanadianIsInitialUpper;
			}
			string initialPlacementLower="X";			
			CCDField ccdFieldInitialPlacementLower=_ccdFieldInputterFormData.GetFieldById("F18");
			if(ccdFieldInitialPlacementLower!=null) {
				initialPlacementLower=ccdFieldInitialPlacementLower.valuestr;
			}
			else if(_claim!=null) {
				initialPlacementLower=_claim.CanadianIsInitialLower;
			}
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Begin indentation.
			_documentGenerator.DrawString(g,_isFrench?"Prothèse, couronne ou pont: est-ce la première mise en bouche?":"If Denture, crown or bridge, Is this the initial placement?",_x,0);
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"Maxillaire: ":"Upper: ",_x,0);
			_x+=80;
			_documentGenerator.PushX(_x);//Begin second indentation.
			if(initialPlacementUpper=="N") {
				_documentGenerator.DrawString(g,_isFrench?"Non":"No",_x,0);
				_x=_documentGenerator.StartElement();
				_text=GetMaterialDescription(_claim.CanadianMaxProsthMaterial);//Field F20
				_documentGenerator.DrawField(g,_isFrench?"Matériau initial":"Initial Material",_text,true,_x,0);
				_x=_documentGenerator.StartElement();
				_text=DateToString(_claim.CanadianDateInitialUpper);//Field F04
				_documentGenerator.DrawField(g,_isFrench?"Date de mise en bouche":"Placement Date",_text,true,_x,0);
				_x=_documentGenerator.StartElement();
				_text=GetAllProcedureTypeDescriptions();
				_documentGenerator.DrawField(g,_isFrench?"Motif du remplacement":"Reason for replacement",_text,true,_x,0);
			}
			else if(initialPlacementUpper=="Y") {
				_documentGenerator.DrawString(g,_isFrench?"Oui":"Yes",_x,0);
			}
			_x=_documentGenerator.PopX();//End second indentation.
			_x=_documentGenerator.StartElement();
			_documentGenerator.DrawString(g,_isFrench?"Mandibule: ":"Lower: ",_x,0);
			_x+=80;
			_documentGenerator.PushX(_x);//Begin second indentation.
			if(initialPlacementLower=="N") {
				_documentGenerator.DrawString(g,_isFrench?"Non":"No",_x,0);
				_x=_documentGenerator.StartElement();
				_text=GetMaterialDescription(_claim.CanadianMandProsthMaterial);//Field F21
				_documentGenerator.DrawField(g,_isFrench?"Matériau initial":"Initial Material",_text,true,_x,0);
				_x=_documentGenerator.StartElement();
				_text=DateToString(_claim.CanadianDateInitialLower);//Field F19
				_documentGenerator.DrawField(g,_isFrench?"Date de mise en bouche":"Placement Date",_text,true,_x,0);
				_x=_documentGenerator.StartElement();
				_text=GetAllProcedureTypeDescriptions();
				_documentGenerator.DrawField(g,_isFrench?"Motif du remplacement":"Reason for replacement",_text,true,_x,0);
			}
			else if(initialPlacementLower=="Y") {
				_documentGenerator.DrawString(g,_isFrench?"Oui":"Yes",_x,0);
			}
			_x=_documentGenerator.PopX();//End second indentation.
			_x=_documentGenerator.PopX();//End first indentation.
		}

		private void PrintToothExtractionBullet(Graphics g) {
			_x+=_documentGenerator.DrawString(g,_bullet.ToString()+". ",_x,0).Width;
			_bullet++;
			_documentGenerator.PushX(_x);//Begin indentation.
			_x+=_documentGenerator.DrawString(g,_isFrench?"S'agit-il d'un traitement en vue de soins d'orthodontie? ":"Is any treatment provided for orthodontic purposes? ",_x,0).Width;
			if(_claim!=null && _claim.IsOrtho){//Field F05
				_documentGenerator.DrawString(g,_isFrench?"Oui":"Yes",_x,0);
				_x=_documentGenerator.StartElement();
				PrintMissingToothList(g);
			}
			else {
				_documentGenerator.DrawString(g,_isFrench?"Non":"No",_x,0);
			}
			_x=_documentGenerator.PopX();//End indentation.
		}

		private void PrintPaymentSummary(Graphics g) {
			float amountWidth=(float)Math.Ceiling((double)g.MeasureString("******.**",_documentGenerator.standardFont).Width);
			float valuesBlockOffset=_x+566;
			_text=(_isFrench?"TOTAL DEMANDÉ:":"TOTAL DENTIST CHARGES:");
			_documentGenerator.DrawString(g,_text,valuesBlockOffset-g.MeasureString(_text,_documentGenerator.standardFont).Width-5,0);
			_text=RawMoneyStrToDisplayMoney(_ccdFieldInputterFormData.GetFieldById("G04").valuestr);
			_documentGenerator.DrawString(g,_text,valuesBlockOffset+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
			_x=_documentGenerator.StartElement();
			_text=_isFrench?"TOTAL DEDUCTIBLES NON-ALLOCER:":"DEDUCTIBLE NOT ALLOCATED:";
			_documentGenerator.DrawString(g,_text,valuesBlockOffset-g.MeasureString(_text,_documentGenerator.standardFont).Width-5,0);
			_text=RawMoneyStrToDisplayMoney(_ccdFieldInputterFormData.GetFieldById("G29").valuestr);
			_documentGenerator.DrawString(g,_text,valuesBlockOffset+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
			_x=_documentGenerator.StartElement();
			string expPayDateStr="";
			CCDField ccdFieldG03=_ccdFieldInputterFormData.GetFieldById("G03");
			if(ccdFieldG03!=null && ccdFieldG03.valuestr!="00000000") {
				expPayDateStr=DateNumToPrintDate(ccdFieldG03.valuestr);
				_documentGenerator.DrawField(g,_isFrench?"DATE PRÉVUE DU PAIEMENT":"EXPECTED PAYMENT DATE",expPayDateStr,true,_x,0);
			}
			CCDField ccdFieldF01=_ccdFieldInputterFormData.GetFieldById("F01");
			//G55 exists in version 04 sometimes, but never in version 02 messages. When G55 is available, it includes adjustments that G28 does not include.
			CCDField ccdFieldTotalPayable=_ccdFieldInputterFormData.GetFieldById("G55");
			if(ccdFieldTotalPayable==null) {
				ccdFieldTotalPayable=_ccdFieldInputterFormData.GetFieldById("G28");
			}
			//For cases when field f01 is not present, we are supposed to grab the value determining who the payment is for from the original claim, 
			//but we must instead rely on the assignment of benefits flag associated with the primary insurance subscriber because there is no such field
			//in the claim object itself.
			string payableTo=(ccdFieldF01==null)?(AssignmentOfBenefits()?"4":"1"):ccdFieldF01.valuestr;
			if(payableTo=="1") {//Pay the subscriber.
				_text=_isFrench?"TOTAL REMBOURSABLE AU TITULAIRE:":"TOTAL PAYABLE TO INSURED:";
				_documentGenerator.DrawString(g,_text,valuesBlockOffset-g.MeasureString(_text,_documentGenerator.standardFont).Width-5,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldTotalPayable.valuestr);
				_documentGenerator.DrawString(g,_text,valuesBlockOffset+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_x=_documentGenerator.StartElement();
				_text=_isFrench?"ADRESSE DU DESTINATAIRE DU PAIEMENT:":"PAYEE'S ADDRESS:";
				SizeF sizeF1=_documentGenerator.DrawString(g,_text,_x,0);
				_text=Patients.GetAddressFull(_patient.Address,_patient.Address2,_patient.City,_patient.State,_patient.Zip);
				_documentGenerator.DrawString(g,_text,_x+sizeF1.Width+10,0);
			}
			else if(payableTo=="2") {//Pay other party.
				_text=_isFrench?"TOTAL REMBOURSABLE AU AUTRES:":"TOTAL PAYABLE TO OTHER:";
				_documentGenerator.DrawString(g,_text,valuesBlockOffset-g.MeasureString(_text,_documentGenerator.standardFont).Width-5,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldTotalPayable.valuestr);
				_documentGenerator.DrawString(g,_text,valuesBlockOffset+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_x=_documentGenerator.StartElement();
			}
			else if(payableTo=="3") {//Reserved
			}
			else if(payableTo=="4" || payableTo=="0") {//Dentist
				_text=_isFrench?"TOTAL REMBOURSABLE AU DENTISTE:":"TOTAL PAYABLE TO DENTIST:";
				_documentGenerator.DrawString(g,_text,valuesBlockOffset-g.MeasureString(_text,_documentGenerator.standardFont).Width-5,0);
				_text=RawMoneyStrToDisplayMoney(ccdFieldTotalPayable.valuestr);
				_documentGenerator.DrawString(g,_text,valuesBlockOffset+amountWidth-g.MeasureString(_text,_documentGenerator.standardFont).Width,0);
				_x=_documentGenerator.StartElement();
				_text=_isFrench?"ADRESSE DU DESTINATAIRE DU PAIEMENT:":"PAYEE'S ADDRESS:";
				SizeF sizeF1=_documentGenerator.DrawString(g,_text,_x,0);
				PrintPracticeAddress(g,_x+sizeF1.Width+10);
			}
			_x=_documentGenerator.StartElement();
		}

		private void PrintPracticeAddress(Graphics g,float xPos){
			if(_clinic==null){
				if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)){
					_text=Patients.GetAddressFull(PrefC.GetString(PrefName.PracticeBillingAddress),PrefC.GetString(PrefName.PracticeBillingAddress2),
						PrefC.GetString(PrefName.PracticeBillingCity),PrefC.GetString(PrefName.PracticeBillingST),PrefC.GetString(PrefName.PracticeBillingZip));
				}else{
					_text=Patients.GetAddressFull(PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),
						PrefC.GetString(PrefName.PracticeCity),PrefC.GetString(PrefName.PracticeST),PrefC.GetString(PrefName.PracticeZip));
				}
			}else{
				_text=Patients.GetAddressFull(_clinic.Address,_clinic.Address2,_clinic.City,_clinic.State,_clinic.Zip);
			}
			_documentGenerator.DrawString(g,_text,xPos,0);
		}

		private void PrintMissingToothList(Graphics g){
			CCDField ccdFieldInitialPlacementUpper=_ccdFieldInputterFormData.GetFieldById("F15");
			CCDField ccdFieldInitialPlacementLower=_ccdFieldInputterFormData.GetFieldById("F18");
			if(ccdFieldInitialPlacementUpper!=null && ccdFieldInitialPlacementLower!=null &&
				(ccdFieldInitialPlacementUpper.valuestr=="Y" || ccdFieldInitialPlacementUpper.valuestr=="O") &&
				(ccdFieldInitialPlacementLower.valuestr=="Y" || ccdFieldInitialPlacementLower.valuestr=="O")) {//Only print extractions when F15 or F18 are "Yes"
				string title=(_isFrench?"D#":"TH")+"  DATE(YYYYMMDD)\t";
				float titleWidth=g.MeasureString(title,_documentGenerator.standardFont).Width;
				int cycleOrthoDateCount=(int)((_documentGenerator.bounds.Right-_x)/titleWidth);
				for(int i=0;i<Math.Min(cycleOrthoDateCount,_listProcedureExtracted.Count);i++) {
					_x+=_documentGenerator.DrawString(g,title,_x,0).Width;
				}
				int j=0;
				for(int i=0;i<_listProcedureExtracted.Count;i++) {//Count specified by field F22
					if(j%cycleOrthoDateCount==0) {
						_x=_documentGenerator.StartElement();
					}
					if(IsValidDate(_listProcedureExtracted[i].ProcDate)) {//Tooth is considered unextracted if it doesn't have a date.
						float thWidth=_documentGenerator.DrawString(g,Tooth.ToInternat(_listProcedureExtracted[i].ToothNum).PadLeft(2,' ')+" ",_x,0).Width;//Field F23
						_text=" "+DateToString(_listProcedureExtracted[i].ProcDate);
						_documentGenerator.DrawString(g,_text,_x+thWidth,0);
						_x+=titleWidth;
						j++;
					}
				}
			}
		}

		private void PrintNoteList(Graphics g) {
			CCDField[] ccdFieldsArrayNoteOutputFlags=_ccdFieldInputterFormData.GetFieldsById("G41");
			CCDField[] ccdFieldsArrayNoteNumbers=_ccdFieldInputterFormData.GetFieldsById("G45");
			CCDField[] ccdFieldsArrayNoteTexts=_ccdFieldInputterFormData.GetFieldsById("G26");
			List<string> listStringsDisplayMessages=new List<string>();
			List<int> listDisplayMessageNumbers=new List<int>();
			_documentGenerator.DrawString(g,"NOTES: ",_x,0,_fontHeading);
			_documentGenerator.StartElement(_verticalLine);
			for(int i=0;i<ccdFieldsArrayNoteTexts.Length;i++) {//noteTexts.Length<=32
				if(i<ccdFieldsArrayNoteOutputFlags.Length) {//Sometimes G26 exists without the output flags or the note numbers.
					if(PIn.Int(ccdFieldsArrayNoteOutputFlags[i].valuestr)==1) {
						continue;//We will print the notes if either the output flag is 2 (print) or 0 (prompt), but will not print notes with output flag 1 (display notes on screen).
					}
				}
				if(i<ccdFieldsArrayNoteNumbers.Length) {
					listDisplayMessageNumbers.Add(PIn.Int(ccdFieldsArrayNoteNumbers[i].valuestr));
				}
				else {
					listDisplayMessageNumbers.Add(i+1);
				}
				listStringsDisplayMessages.Add(ccdFieldsArrayNoteTexts[i].valuestr);
			}
			while(listStringsDisplayMessages.Count>0) {
				int indexOfMinVal=0;
				for(int j=1;j<listDisplayMessageNumbers.Count;j++) {
					if(listDisplayMessageNumbers[j]<listDisplayMessageNumbers[indexOfMinVal]) {
						indexOfMinVal=j;
					}
				}
				_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,listDisplayMessageNumbers[indexOfMinVal].ToString().PadLeft(2,'0'),_x,0);
				_documentGenerator.DrawString(g,listStringsDisplayMessages[indexOfMinVal],_x+50,0);
				listStringsDisplayMessages.RemoveAt(indexOfMinVal);
				listDisplayMessageNumbers.RemoveAt(indexOfMinVal);
			}
		}

		///<summary>Returns the number of errors displayed.</summary>
		private int PrintErrorList(Graphics g) {
			CCDField[] ccdFieldArrayErrors=_ccdFieldInputterFormData.GetFieldsById("G08");
			if(ccdFieldArrayErrors==null){
				return 0;
			}
			_documentGenerator.DrawString(g,(_isFrench?"ERREURS (":"ERRORS (")+ccdFieldArrayErrors.Length+")",_x,0,_fontHeading);
			for(int i=0;i<ccdFieldArrayErrors.Length;i++){
				_x=_documentGenerator.StartElement();
				_documentGenerator.DrawString(g,ccdFieldArrayErrors[i].valuestr.PadLeft(3,'0'),_x,0);
				_documentGenerator.DrawString(g,CCDerror.message(Convert.ToInt32(ccdFieldArrayErrors[i].valuestr),_isFrench),_x+80,0);
			}
			return ccdFieldArrayErrors.Length;
		}

		private bool AssignmentOfBenefits() {
			if(_claim!=null) {
				if(_claim.ClaimType=="S") {
					if(_insSub2!=null) {
						return Claims.GetAssignmentOfBenefits(_claim,_insSub2);
					}
				}
				if(_insSub!=null) {
					return Claims.GetAssignmentOfBenefits(_claim,_insSub);
				}
			}
			return false;
		}

		#endregion

		#region Printing Information Translators

		private bool ThisIsPrimary(){
			string strCarrierIdentificationNumber=_ccdFieldInputterFormData.GetFieldById("A05").valuestr;//Exists in all formats but 24-Email, and 16-Payment Reconciliation Response
			return _carrier!=null && _carrier.ElectID==strCarrierIdentificationNumber;
		}

		///<summary>The rawPercent string should be of length 3 and should be numerical digits only.</summary>
		private string RawPercentToDisplayPercent(string rawPercent){
			return Canadian.RawPercentToDisplayPercent(rawPercent)+"%";
		}

		private bool IsValidDate(DateTime dt){
			return (dt.Year>1880);
		}

		private string DateToString(DateTime dt){
			if(IsValidDate(dt)){
				return dt.ToShortDateString();
			}
			return "";//Invalid date.
		}

		///<summary>Input string is expected to have the form 'YYYYMMDD'.</summary>
		private string DateNumToPrintDate(string number){
			if(number=="00000000") {
				return DateToString(DateTime.MinValue);
			}
			DateTime dateTime=new DateTime(Convert.ToInt32(number.Substring(0,4)),Convert.ToInt32(number.Substring(4,2)),Convert.ToInt32(number.Substring(6,2)));
			return DateToString(dateTime);
		}

		///<summary>The given number must be in the format of: [+-]?[0-9]*</summary>
		private string RawMoneyStrToDisplayMoney(string number){
			return Canadian.RawMoneyStrToDisplayMoney(number);
		}

		private string GetPayableToString(bool doAssignBenefits) {
			if(doAssignBenefits) {
				return _isFrench?"DENTISTE":"DENTIST";
			}
			else {
				return _isFrench?"TITULAIRE":"INSURED/MEMBER";
			}
		}

		///<summary>Convert a patient relationship enum value into a human-readable, CDA required string.</summary>
		private string GetPatientRelationshipString(Relat relat){
			switch(Canadian.GetRelationshipCode(relat)){
				case "1":
					return _isFrench?"Soi-même":"Self";
				case "2":
					return _isFrench?"Époux(se)":"Spouse";
				case "3":
					return _isFrench?"Enfant":"Child";
				case "4":
					return _isFrench?"Conjoint(e)":"Common Law Spouse";
				case "5":
					return _isFrench?"Autre":"Other";
				default:
					break;
			}
			return "";
		}

		///<summary>Convet a code from fields F20 and F21 into a human-readable string.</summary>
		private string GetMaterialDescription(int materialCode) {
			switch(materialCode) {
				case 1:
					return _isFrench?"Pont fixe":"Fixed Bridge";
				case 2:
					return _isFrench?"Pont Maryland":"Maryland Bridge";
				case 3:
					return _isFrench?"Prothèse (acrylique)":"Denture (Acrylic)";
				case 4:
					return _isFrench?"Prothèse (chrome cobalt)":"Denture (Chrome Cobalt)";
				case 5:
					return _isFrench?"Implant (fixe)":"Implant (Fixed)";
				case 6:
					return _isFrench?"Implant (démontable)":"Implant (Removable)";
				case 7:
					return _isFrench?"Implant (amovible)":"Crown";
				default:
					break;
			}
			return "";
		}

		///<summary>Convert one of the type codes from field F16 into a human-readable string.</summary>
		private string GetProcedureTypeCodeDescription(char procedureTypeCode) {
			switch(procedureTypeCode) {
				case 'A':
					return _isFrench?"Réparation d’un traitement ou appareil; si non spécifié, il s’agit d’une mise en bouche initiale.":
						"Repair of a prior service or installation.";
				case 'B':
					return _isFrench?"Mise en bouche ou traitement temporaire; si non spécifié, il s’agit d’une mise en bouche ou traitement permanent.":"Temporary placement or service.";
				case 'C':
					return _isFrench?"Correction d’un appareil ATM.":"Service for correction of TMJ.";
				case 'E':
					return _isFrench?"Traitement est un implant ou est exécuté conjointement avec un implant.":
						"Service is an implant or is performed in conjunction with implants.";
				case 'L':
					return _isFrench?"Appareil perdu.":"Appliance lost.";
				case 'S':
					return _isFrench?"Appareil volé.":"Appliance stolen.";
				case 'X':
					return _isFrench?"Aucun de ces choix.":"Abnormal circumstances.";
				default:
					break;
			}
			return "";
		}

		///<summary>If field F16 is defined for the current message, then the codes within it are transcribed into a paragraph of text.</summary>
		private string GetAllProcedureTypeDescriptions(){
			string text="";
			CCDField ccdFieldProcedureTypeCodes=_ccdFieldInputterFormData.GetFieldById("F16");
			if(ccdFieldProcedureTypeCodes!=null){
				for(int c=0;c<ccdFieldProcedureTypeCodes.valuestr.Length;c++){
					if(ccdFieldProcedureTypeCodes.valuestr[c]!=' '){
						if(text!=""){
							text+=Environment.NewLine;
						}
						text+=GetProcedureTypeCodeDescription(ccdFieldProcedureTypeCodes.valuestr[c]);
					}
				}
			}
			return text;
		}

		#endregion

	}
}