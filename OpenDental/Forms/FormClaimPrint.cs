using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental{
	///<summary></summary>
	public partial class FormClaimPrint : FormODBase {
		///<summary></summary>
		public long ClaimNum;
	///<summary>This is set externally for Renaissance and generic e-claims.  If it was not set ahead of time, it will set in FillDisplayStrings according to the insPlan.</summary>
		public ClaimForm ClaimFormCur;
		///<summary></summary>
		public long PatNum;
		//<summary>This will be 0 unless the user is trying to print a batch e-claim with a predefined ClaimForm.</summary>
		//public int ClaimFormNum;
		///<summary></summary>
		public bool DoPrintBlank;
		///<summary></summary>
		public bool DoPrintImmediately;
		///<summary>Set to true if the claim was marked sent.</summary>
		public bool WasSent=false;
		private string[] _stringArrayDisplay;
		///<summary>The claimprocs for this claim, not including payments and duplicates.</summary>
		private List<ClaimProc> _listClaimProcs;
		private int _pagesPrinted;
		private int _totalPages;
		//<summary>Set to true if using this class just to generate strings for the Renaissance link.</summary>
		//private bool IsRenaissance;
		private List<ClaimProc> _listClaimProcsForClaim;
		private List<ClaimProc> _listClaimProcsForPat;
		///<summary>All procedures for the patient.</summary>
		private List<Procedure> _listProcedures;
		///<summary></summary>
		private List <InsPlan> _listInsPlans;
		//private InsPlan[] MedPlanList;
		private List<PatPlan> _listPatPlans;
		private Claim _claim;
		///<summary>Filled with the first four diagnoses codes in the list of procedures associated with the claim. Always length of 4.</summary>
		private string[] _stringArrayDiagnoses;
		///<summary>Complete list of all diagnoses.  Maximum unique length of array will be 12 due to the requirements of the medical 1500 (02-12) claim form.</summary>
		private string[] _stringArrayAllDiagnoses;
		//private Claim[] ClaimsArray;
		//private Claim[] MedClaimsArray;
		private List<ClaimValCodeLog> _listClaimValCodeLog;
		private Referral _referral;
		private List<InsSub> _listInsSubs;
		private InsSub _insSub;
		private InsPlan _insPlan;
		private Carrier _carrier;

		///<summary></summary>
		public FormClaimPrint(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this,new Control[] 
			{
				this.labelTotPages//exclude
			});
		}

		private void FormClaimPrint_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			LayoutManager.MoveHeight(Preview2,ClientRectangle.Height);
			LayoutManager.MoveWidth(Preview2,ClientRectangle.Width-160);
			//butClose.Location=new Point(ClientRectangle.Width-100,ClientRectangle.Height-70);
			//butPrint.Location=new Point(ClientRectangle.Width-100,ClientRectangle.Height-140);
		}

		private void FormClaimPrint_Load(object sender, System.EventArgs e) {
			CreateODprintout("",PrintSituation.Default,0);
			if(ODprintout.CurPrintout.SettingsErrorCode==PrintoutErrorCode.Success) {
				Preview2.Document=ODprintout.CurPrintout.PrintDoc;
				Preview2.InvalidatePreview();
			}
			else {
				butPrint.Enabled=false;
			}
		}

		///<summary>Only called from external forms without ever loading this form.  Prints without showing any print preview.  Returns true if printed 
		///successfully.  Set doUseLastPrinterSettingsIfAvailable=true to skip any printing prompts and use the last used printer settings if exists.
		///</summary>
		public bool PrintImmediate(string auditDescription,PrintSituation printSituation,long auditPatNum,bool doUseLastPrinterSettingsIfAvailable=false) {
			if(DoPrintBlank) {
				//Get the default claim form for the practice.
				ClaimFormCur=ClaimForms.GetClaimForm(PrefC.GetLong(PrefName.DefaultClaimForm));
				if(ClaimFormCur==null) {//Could happen if printing a blank form and no default claim form is set.
					MsgBox.Show(this,"No default claim form set.  Set a default in Setup | Family / Insurance | Claim Forms.");
					return false;
				}
			}
			PrinterSettings printerSettings=(PrinterSettings)ODprintout.CurPrintout?.PrintDoc?.PrinterSettings?.Clone();
			PageSettings pageSettings=(PageSettings)ODprintout.CurPrintout?.PrintDoc?.DefaultPageSettings?.Clone();
			bool? isOriginAtMargins=ODprintout.CurPrintout?.PrintDoc?.OriginAtMargins;
			CreateODprintout(auditDescription,printSituation,auditPatNum);
			if(ODprintout.CurPrintout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				return false;
			}
			if(doUseLastPrinterSettingsIfAvailable && printerSettings!=null && pageSettings!=null) {
				ODprintout.CurPrintout.PrintDoc.PrinterSettings=printerSettings;
				ODprintout.CurPrintout.PrintDoc.DefaultPageSettings=pageSettings;
				ODprintout.CurPrintout.PrintDoc.OriginAtMargins=isOriginAtMargins??ODprintout.CurPrintout.PrintDoc.OriginAtMargins;
				return ODprintout.CurPrintout.TryPrint();
			}
			return PrinterL.TryPrint(ODprintout.CurPrintout);
		}

		///<summary>Constructs a new ODprintout and sets it to ODprintout.CurPrintout.</summary>
		private void CreateODprintout(string auditDescription,PrintSituation printSituation,long auditPatNum) {
			_pagesPrinted=0;
			ODprintout.InvalidMinDefaultPageHeight=400;//some printers report page size of 0.
			ODprintout.InvalidMinDefaultPageWidth=0;
			PrinterL.CreateODprintout(pd2_PrintPage,
				auditDescription,
				printSituation,
				auditPatNum,
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){//raised for each page to be printed.
			if(!TryFillDisplayStrings(false)) {//if failed
				ev.HasMorePages=false;
				return;
			}
			int procLimit=ProcLimitForFormat();
			//claimprocs is filled in FillDisplayStrings
			if(_listClaimProcs.Count==0){
			_totalPages=1;
			//js Some other programmer added this variable and implemented it.  We couldn't figure out why.  For example, in the line above, why would claimprocs ever be zero?  Due to many style and logic issues, we were forced to remove the function of this variable.  It would have to be written from scratch to implement it in the future.  Claims were never designed for multi-page.	
			//Jason - this code we don't understand worked so I added it back until we get time to rewrite it correctly.
			}
			else{
				_totalPages=(int)Math.Ceiling((double)_listClaimProcs.Count/(double)procLimit);
			}
			bool HasMedical = false;
			if(!DoPrintBlank){
				FillProcStrings(_pagesPrinted*procLimit,procLimit);
				for(int i=0;i<_listInsPlans.Count;i++){
					if(_listInsPlans[i].IsMedical){
						HasMedical=true;
					}
				}
			}
			if(HasMedical){
				FillMedInsStrings();
				FillMedValueCodes();
				FillMedCondCodes();
			}
			Graphics g=ev.Graphics;
			float xPosText;
			for(int i=0;i<ClaimFormCur.Items.Count;i++){
				if(ClaimFormCur.Items[i].ImageFileName==""){//field
					xPosText=ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX;
					if(ClaimFormCur.Items[i].FieldName.In(
							"P1Fee","P2Fee","P3Fee","P4Fee","P5Fee","P6Fee","P7Fee","P8Fee","P9Fee","P10Fee","P11Fee","P12Fee","P13Fee","P14Fee","P15Fee","P1Lab",
							"P2Lab","P3Lab","P4Lab","P5Lab","P6Lab","P7Lab","P8Lab","P9Lab","P10Lab","P1FeeMinusLab","P2FeeMinusLab","P3FeeMinusLab",
							"P4FeeMinusLab","P5FeeMinusLab","P6FeeMinusLab","P7FeeMinusLab","P8FeeMinusLab","P9FeeMinusLab","P10FeeMinusLab","TotalFee",
							"MedInsAAmtDue","MedInsBAmtDue","MedInsCAmtDue","MedInsAPriorPmt","MedInsBPriorPmt","MedInsCPriorPmt","MedValAmount39a",
							"MedValAmount39b","MedValAmount39c","MedValAmount39d","MedValAmount40a","MedValAmount40b","MedValAmount40c","MedValAmount40d",
							"MedValAmount41a","MedValAmount41b","MedValAmount41c","MedValAmount41d","ICDindicatorAB","OutsideLabFee"))
					{
						//this aligns it to the right
						xPosText-=g.MeasureString(_stringArrayDisplay[i],new Font(ClaimFormCur.FontName,ClaimFormCur.FontSize)).Width;
					}
					g.DrawString(_stringArrayDisplay[i],
						new Font(ClaimFormCur.FontName,ClaimFormCur.FontSize),
						new SolidBrush(Color.Black),
						new RectangleF(xPosText,ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,ClaimFormCur.Items[i].Width,ClaimFormCur.Items[i].Height));
				}
				else{//image
					if(!ClaimFormCur.PrintImages){
						continue;
					}
					Image image;
					string extension;
					switch(ClaimFormCur.Items[i].ImageFileName) {
						case "ADA2006.gif":
							image=CDT.Class1.GetADA2006();
							extension=".gif";
							break;
						case "ADA2012.gif":
							image=CDT.Class1.GetADA2012();
							extension=".gif";
							break;
						case "ADA2012_J430D.gif":
							image=CDT.Class1.GetADA2012_J430D();
							extension=".gif";
							break;
						case "ADA2018_J432.gif":
							image=CDT.Class1.GetADA2018_J432();
							extension=".gif";
							break;
						case "ADA2019_J430.gif":
							image=CDT.Class1.GetADA2019_J430();
							extension=".gif";
							break;
						case "ADA2024_J430.gif":
							image=CDT.Class1.GetADA2024_J430();
							extension=".gif";
							break;
						case "1500_02_12.gif":
							image=Properties.Resources._1500_02_12;
							extension=".gif";
							break;
						case "DC-217.gif":
							image=CDT.Class1.GetDC217();
							extension=".gif";
							break;
						default:
							//In the case when the A to Z folders are not being used, an invalid form image path is returned
							//and we simply print without the background image (just as if the background image were removed
							//from the A to Z folders where it was expected.
							string fileName=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),ClaimFormCur.Items[i].ImageFileName);
							if(!FileAtoZ.Exists(fileName)) {
								continue;
							}
							image=FileAtoZ.GetImage(fileName);
							if(image==null) {
								continue;
							}
							extension=Path.GetExtension(ClaimFormCur.Items[i].ImageFileName);
							break;
					}
					if(extension==".jpg"){
						g.DrawImage(image,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							(int)(image.Width/image.HorizontalResolution*100),
							(int)(image.Height/image.VerticalResolution*100));
					}
					else if(extension==".gif"){
						g.DrawImage(image,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							ClaimFormCur.Items[i].Width,
							ClaimFormCur.Items[i].Height);
					}
					else if(extension==".emf"){
						g.DrawImage(image,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							image.Width,image.Height);
					}
				}
			}
			_pagesPrinted++;
			if(_totalPages==_pagesPrinted){
				ev.HasMorePages=false;
				labelTotPages.Text="1 / "+_totalPages.ToString();
			}
			else{
				//MessageBox.Show(pagesPrinted.ToString()+","+totalPages.ToString());
				ev.HasMorePages=true;
			}
		}

		///<summary>Only used when the print button is clicked from within this form during print preview.</summary>
		public bool PrintClaim(){
			_pagesPrinted=0;
			return PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Claim from")+" "+_claim.DateService.ToShortDateString()+" "+Lan.g(this,"printed"),
				PatNum,
				PrintSituation.Claim,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin
			);
		}

		///<summary>Called from Bridges.Renaissance, this takes the supplied ClaimFormItems.ListForForm, and generates an array of strings that will get saved into a text file.  First dimension of array is the pages. Second dimension is the lines in the page.</summary>
		public static string[][] FillRenaissance(long claimNum,long patNum,List<ClaimFormItem> listClaimFormItems) {
			FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.ClaimNum=claimNum;
			formClaimPrint.PatNum=patNum;
			formClaimPrint.ClaimFormCur=new ClaimForm();
			formClaimPrint.ClaimFormCur.Items=listClaimFormItems;
			return formClaimPrint.FillRenaissance();
		}

		private string[][] FillRenaissance() { 
			//IsRenaissance=true;
			int procLimit=10;
			TryFillDisplayStrings(true);//claimprocs is filled in FillDisplayStrings
														//, so this is just a little extra work
			_totalPages=(int)Math.Ceiling((double)_listClaimProcs.Count/(double)procLimit);
			string[][] stringDoubleArrayRetVals=new string[_totalPages][];
			for(int i=0;i<_totalPages;i++){
				_pagesPrinted=i;
				//not sure if I also need to do FillDisplayStrings here
				FillProcStrings(_pagesPrinted*procLimit,procLimit);
				stringDoubleArrayRetVals[i]=(string[])_stringArrayDisplay.Clone();
			}
			return stringDoubleArrayRetVals;
		}

		///<summary>Gets all necessary info from db based on ThisPatNum and ThisClaimNum.  Then fills _stringArrayDisplay with the actual text that will 
		///display on claim.  The isRenaissance flag is very temporary. Returns true if able to successfully fill the display strings.</summary>
		private bool TryFillDisplayStrings(bool isRenaissance){
			if(DoPrintBlank){
				_stringArrayDisplay=new string[ClaimFormCur.Items.Count];
				_listClaimProcs=new List<ClaimProc>();
				return true;
			}
			Family family=Patients.GetFamily(PatNum);
			Patient patient=family.GetPatient(PatNum);
			if(patient==null) {
				MsgBox.Show(this,"Unable to find patient.");
				butPrint.Enabled=false;
				return false;
			}
			List<Claim> listClaims=Claims.Refresh(patient.PatNum);
			_claim=Claims.GetFromList(listClaims,ClaimNum);
			if(_claim==null) {
				MsgBox.Show(this,"Claim has been deleted by another user.");
				butPrint.Enabled=false;
				return false;
			}
				//((Claim)Claims.HList[ThisClaimNum]).Clone();
			_listInsSubs=InsSubs.RefreshForFam(family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_listPatPlans=PatPlans.Refresh(_claim.PatNum);
			InsPlan insPlan2=InsPlans.GetPlan(_claim.PlanNum2,_listInsPlans);
			InsSub insSub2=InsSubs.GetSub(_claim.InsSubNum2,_listInsSubs);
			if(insPlan2==null){
				insPlan2=new InsPlan();//easier than leaving it null
			}
			Carrier carrier2=new Carrier();
			if(insPlan2.PlanNum!=0){
				carrier2=Carriers.GetCarrier(insPlan2.CarrierNum);
			}
			//Employers.GetEmployer(otherPlan.EmployerNum);
			//Employer otherEmployer=Employers.Cur;//not actually used
			//then get the main plan
			_insSub=InsSubs.GetSub(_claim.InsSubNum,_listInsSubs);
			_insPlan=InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
			Clinic clinic=PrefC.HasClinicsEnabled ? Clinics.GetClinic(_claim.ClinicNum) : null;//null if clinics are not enabled.
			_carrier=Carriers.GetCarrier(_insPlan.CarrierNum);
			//Employers.GetEmployer(InsPlans.Cur.EmployerNum);
			Patient patientSubscriber;
			if(family.GetIndex(_insSub.Subscriber)==-1) {//from another family
				patientSubscriber=Patients.GetPat(_insSub.Subscriber);
				//Patients.Cur;
				//Patients.GetFamily(ThisPatNum);//return to current family
			}
			else{
				patientSubscriber=family.ListPats[family.GetIndex(_insSub.Subscriber)];
			}
			if(patientSubscriber==null) {//Patient for this InsSub could not be found.  Likely db corruption.
				MsgBox.Show(this,"Insurance Plan attached to Claim does not have a valid Subscriber.  Run Database Maintenance (Tools): InsSubInvalidSubscriber.");
				butPrint.Enabled=false;
				return false;
			}
			Patient patientOtherSubscriber=new Patient();
			if(insPlan2.PlanNum!=0){//if secondary insurance exists
				if(family.GetIndex(insSub2.Subscriber)==-1) {//from another family
					patientOtherSubscriber=Patients.GetPat(insSub2.Subscriber);
					//Patients.Cur;
					//Patients.GetFamily(ThisPatNum);//return to current family
				}
				else{
					patientOtherSubscriber=family.ListPats[family.GetIndex(insSub2.Subscriber)];
				}
			}
			if(_claim.ReferringProv>0) {
				Referrals.TryGetReferral(_claim.ReferringProv,out _referral);
			}
			_listProcedures=Procedures.Refresh(patient.PatNum);
			List<ToothInitial> listToothInitials=ToothInitials.GetPatientData(patient.PatNum);
			//List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			_listClaimProcsForPat=ClaimProcs.Refresh(_claim.PatNum);
			_listClaimProcsForClaim=ClaimProcs.RefreshForClaim(_claim.ClaimNum);
			//Customers were sometimes getting supplemental payments to show up before a received claim proc. This would cause the FeeBilled to show as $0.
			_listClaimProcsForClaim=_listClaimProcsForClaim.OrderBy(x => x.LineNumber)
				.ThenBy(x => x.Status==ClaimProcStatus.Supplemental).ToList();//Put Supplementals on the bottom since they don't have the FeeBilled set.
			_listClaimProcs=new List<ClaimProc>();
			bool doIncludeThis;
			Procedure procedure;
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){//fill the arraylist
				if(_listClaimProcsForClaim[i].ProcNum==0){
					continue;//skip payments
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcsForClaim[i].ProcNum);
					if(procedure.ProcNumLab!=0) { //This is a lab fee procedure.
						continue;//skip lab fee procedures in Canada, because they will show up on the same line as the procedure that they are attached to.
					}
				}
				doIncludeThis=true;
				for(int j=0;j<_listClaimProcs.Count;j++){//loop through existing claimprocs
					if(_listClaimProcs[j].ProcNum==_listClaimProcsForClaim[i].ProcNum){
						doIncludeThis=false;//skip duplicate procedures
					}
				}
				if(doIncludeThis){
					_listClaimProcs.Add(_listClaimProcsForClaim[i]);	
				}
			}
			List<string> listStringsMissingTeeth=ToothInitials.GetMissingOrHiddenTeeth(listToothInitials);
			ProcedureCode procedureCode;
			for(int j=listStringsMissingTeeth.Count-1;j>=0;j--) {//loop backwards to keep index accurate as items are removed
				//if the missing tooth is missing because of an extraction being billed here, then exclude it
				for(int p=0;p<_listClaimProcs.Count;p++) {
					procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcs[p].ProcNum);
					procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
					if(procedureCode.PaintType==ToothPaintingType.Extraction && procedure.ToothNum==listStringsMissingTeeth[j]) {
						listStringsMissingTeeth.RemoveAt(j);
						break;
					}
				}
			}
			//diagnoses---------------------------------------------------------------------------------------
			_stringArrayDiagnoses=new string[4];
			for(int i=0;i<4;i++){
				_stringArrayDiagnoses[i]="";
			}
			for(int i=0;i<_listClaimProcs.Count;i++){
				procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcs[i].ProcNum);
				if(procedure.DiagnosticCode=="" || procedure.IcdVersion==9){
					continue;
				}
				for(int d=0;d<4;d++){
					if(_stringArrayDiagnoses[d]==procedure.DiagnosticCode){
						break;//if it's already been added
					}
					if(_stringArrayDiagnoses[d]==""){//we're at the end of the list of existing diagnoses, and no match
						_stringArrayDiagnoses[d]=procedure.DiagnosticCode;//so add it.
						break;
					}
				}
				//There's still a chance that the diagnosis didn't get added, if there were more than 4.
			}
			_stringArrayAllDiagnoses=Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(_listClaimProcs),true).ToArray();
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider providerClaimTreat=Providers.GetFirstOrDefault(x => x.ProvNum==_claim.ProvTreat)??providerFirst;
			ProviderClinic providerClinicClaimTreat=ProviderClinics.GetOneOrDefault(providerClaimTreat.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			Provider providerClaimBill=Providers.GetFirstOrDefault(x => x.ProvNum==_claim.ProvBill)??providerFirst;
			ProviderClinic provClinicClaimBill=ProviderClinics.GetOneOrDefault(providerClaimBill.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			if(ClaimFormCur==null){
				if(_claim.ClaimForm>0){
					ClaimFormCur=ClaimForms.GetClaimForm(_claim.ClaimForm);
				} 
				else {
					ClaimFormCur=ClaimForms.GetClaimForm(_insPlan.ClaimFormNum);
				}
			}
			//If we aren't able to find a claimform, or it has no items there's nothing we can do.
			if(ClaimFormCur==null || ClaimFormCur.Items==null) {
				if(!isRenaissance) {
					MsgBox.Show("Could not retrieve claim form.\r\nClose form and try again.");
				}
				return false;
			}
			List<PatPlan> listPatPlans=null;
			_stringArrayDisplay=new string[ClaimFormCur.Items.Count];
			//a value is set for every item, but not every case will have a matching claimform item.
			for(int i=0;i<ClaimFormCur.Items.Count;i++){
				if(ClaimFormCur.Items[i]==null){//Renaissance does not use [0]
					_stringArrayDisplay[i]="";
					continue;
				}
				switch(ClaimFormCur.Items[i].FieldName){
					default://image. or procedure which gets filled in FillProcStrings.
						_stringArrayDisplay[i]="";
						break;
					case "FixedText":
						_stringArrayDisplay[i]=ClaimFormCur.Items[i].FormatString;
						break;
					case "IsPreAuth":
						if(_claim.ClaimType=="PreAuth") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsStandardClaim":
						if(_claim.ClaimType!="PreAuth") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "ShowPreauthorizationIfPreauth":
						if(_claim.ClaimType=="PreAuth") {
							_stringArrayDisplay[i]="Preauthorization";
						}
						break;
					case "IsMedicaidClaim"://this should later be replaced with an insplan field.
						if(patient.MedicaidID!="") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsGroupHealthPlan":
						string eclaimCode=InsFilingCodes.GetEclaimCode(_insPlan.FilingCode);
						if(patient.MedicaidID=="" 
							&& eclaimCode != "MC"//medicaid
							&& eclaimCode != "CH"//champus
							&& eclaimCode != "VA")//veterans
							//&& eclaimcode != ""//medicare?
						{
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PreAuthString":
						_stringArrayDisplay[i]=_claim.PreAuthString;
						break;
					case "PriorAuthString":
						_stringArrayDisplay[i]=_claim.PriorAuthorizationNumber;
						break;
					#region PriIns
					case "PriInsCarrierName":
						_stringArrayDisplay[i]=_carrier.CarrierName;
						break;
					case "PriInsAddress":
						_stringArrayDisplay[i]=_carrier.Address;
						break;
					case "PriInsAddress2":
						_stringArrayDisplay[i]=_carrier.Address2;
						break;
					case "PriInsAddressComplete":
						_stringArrayDisplay[i]=_carrier.Address+" "+_carrier.Address2;
						break;
					case "PriInsCity":
						_stringArrayDisplay[i]=_carrier.City;
						break;
					case "PriInsElectID":
						_stringArrayDisplay[i]=_carrier.ElectID;
						break;
					case "PriInsST":
						_stringArrayDisplay[i]=_carrier.State;
						break;
					case "PriInsZip":
						_stringArrayDisplay[i]=_carrier.Zip;
						break;
					#endregion PriIns
					#region OtherIns
					case "OtherInsExists":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsNotExists":
						if(insPlan2.PlanNum==0) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsExistsDent":
						if(insPlan2.PlanNum!=0) {
							if(!insPlan2.IsMedical) {
								_stringArrayDisplay[i]="X";
							}
						}
						break;
					case "OtherInsExistsMed":
						if(insPlan2.PlanNum!=0) {
							if(insPlan2.IsMedical) {
								_stringArrayDisplay[i]="X";
							}
						}
						break;
					case "OtherInsSubscrLastFirst":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=patientOtherSubscriber.LName+", "+patientOtherSubscriber.FName+" "+patientOtherSubscriber.MiddleI;
						}
						break;
					case "OtherInsSubscrDOB":
						if(insPlan2.PlanNum!=0) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=patientOtherSubscriber.Birthdate.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=patientOtherSubscriber.Birthdate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "OtherInsSubscrIsMale":
						if(insPlan2.PlanNum!=0 && patientOtherSubscriber.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsSubscrIsFemale":
						if(insPlan2.PlanNum!=0 && patientOtherSubscriber.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsSubscrIsGenderUnknown":
						if(insPlan2.PlanNum!=0 && (patientOtherSubscriber.Gender==PatientGender.Unknown || patientOtherSubscriber.Gender==PatientGender.Other)) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsSubscrGender":
						if(insPlan2.PlanNum!=0) {
							if(patientOtherSubscriber.Gender==PatientGender.Male) {
								_stringArrayDisplay[i]="M";
							}
							else if(patientOtherSubscriber.Gender==PatientGender.Female) {
								_stringArrayDisplay[i]="F";
							}
							else {//Unknown or Intersex
								_stringArrayDisplay[i]="U";
							}
						}
						break;
					case "OtherInsSubscrID":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=insSub2.SubscriberID;
						}
						break;
						//if(otherPlan.PlanNum!=0 && otherSubsc.SSN.Length==9){
						//	_stringArrayDisplay[i]=otherSubsc.SSN.Substring(0,3)
						//		+"-"+otherSubsc.SSN.Substring(3,2)
						//		+"-"+otherSubsc.SSN.Substring(5);
						//}
						//break;
					case "OtherInsGroupNum":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=insPlan2.GroupNum;
						}
						break;
					case "OtherInsRelatIsSelf":
						if(insPlan2.PlanNum!=0 && _claim.PatRelat2==Relat.Self) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsRelatIsSpouse":
						if(insPlan2.PlanNum!=0 && _claim.PatRelat2==Relat.Spouse) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsRelatIsChild":
						if(insPlan2.PlanNum!=0 && _claim.PatRelat2==Relat.Child) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "OtherInsRelatIsOther":
						if(insPlan2.PlanNum!=0 && (
							_claim.PatRelat2==Relat.Dependent
							|| _claim.PatRelat2==Relat.Employee
							|| _claim.PatRelat2==Relat.HandicapDep
							|| _claim.PatRelat2==Relat.InjuredPlaintiff
							|| _claim.PatRelat2==Relat.LifePartner
							|| _claim.PatRelat2==Relat.SignifOther
							))
							_stringArrayDisplay[i]="X";
						break;
					case "OtherInsCarrierName":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.CarrierName;
						}
						break;
					case "OtherInsAddress":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.Address;
						}
						break;
					case "OtherInsCity":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.City;
						}
						break;
					case "OtherInsElectID":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.ElectID;
						}
						break;
					case "OtherInsST":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.State;
						}
						break;
					case "OtherInsZip":
						if(insPlan2.PlanNum!=0) {
							_stringArrayDisplay[i]=carrier2.Zip;
						}
						break;
					#endregion OtherIns
					#region Subscr
					case "SubscrLastFirst":
						_stringArrayDisplay[i]=patientSubscriber.LName+", "+patientSubscriber.FName+" "+patientSubscriber.MiddleI;
						break;
					case "SubscrAddress":
						_stringArrayDisplay[i]=patientSubscriber.Address;
						break;
					case "SubscrAddress2":
						_stringArrayDisplay[i]=patientSubscriber.Address2;
						break;
					case "SubscrAddressComplete":
						_stringArrayDisplay[i]=patientSubscriber.Address+" "+patientSubscriber.Address2;
						break;
					case "SubscrCity":
						_stringArrayDisplay[i]=patientSubscriber.City;
						break;
					case "SubscrST":
						_stringArrayDisplay[i]=patientSubscriber.State;
						break;
					case "SubscrZip":
						_stringArrayDisplay[i]=patientSubscriber.Zip;
						break;
					case "SubscrPhone"://needs work.  Only used for 1500
						if(isRenaissance) {
							//Expecting (XXX)XXX-XXXX
							_stringArrayDisplay[i]=patientSubscriber.HmPhone;
							if(patientSubscriber.HmPhone.Length>14) {//Might have a note following the number.
								_stringArrayDisplay[i]=patientSubscriber.HmPhone.Substring(0,14);
							}
						}
						else {
							string phone=patientSubscriber.HmPhone.Replace("(","");
							phone=phone.Replace(")","    ");
							phone=phone.Replace("-","  ");
							_stringArrayDisplay[i]=phone;
						}
						break;
					case "SubscrDOB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=patientSubscriber.Birthdate.ToShortDateString();//MM/dd/yyyy
						}
						else {
							_stringArrayDisplay[i]=patientSubscriber.Birthdate.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "SubscrIsMale":
						if(patientSubscriber.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrIsFemale":
						if(patientSubscriber.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrIsGenderUnknown":
						if(patientSubscriber.Gender==PatientGender.Unknown || patientSubscriber.Gender==PatientGender.Other) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrGender":
						if(patientSubscriber.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="M";
						}
						else if(patientSubscriber.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="F";
						}
						else {//Unknown
							_stringArrayDisplay[i]="U";
						}
						break;
					case "SubscrIsMarried":
						if(patientSubscriber.Position==PatientPosition.Married) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrIsSingle":
						if(patientSubscriber.Position==PatientPosition.Single
							|| patientSubscriber.Position==PatientPosition.Child
							|| patientSubscriber.Position==PatientPosition.Widowed) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrID":
						listPatPlans=PatPlans.Refresh(PatNum);
						string strPatID=PatPlans.GetPatID(_insSub.InsSubNum,listPatPlans);
						if(strPatID=="") {
							_stringArrayDisplay[i]=_insSub.SubscriberID;
						}
						else {
							_stringArrayDisplay[i]=strPatID;
						}
						break;
					case "SubscrIDStrict":
						_stringArrayDisplay[i]=_insSub.SubscriberID;
						break;
					case "SubscrIsFTStudent":
						if(patientSubscriber.StudentStatus=="F") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "SubscrIsPTStudent":
						if(patientSubscriber.StudentStatus=="P") {
							_stringArrayDisplay[i]="X";
						}
						break;
					#endregion Subscr
					case "GroupName":
						_stringArrayDisplay[i]=_insPlan.GroupName;
						break;
					case "GroupNum":
						_stringArrayDisplay[i]=_insPlan.GroupNum;
						break;
					case "DivisionNo":
						_stringArrayDisplay[i]=_insPlan.DivisionNo;
						break;
					case "EmployerName":
						_stringArrayDisplay[i]=Employers.GetEmployer(_insPlan.EmployerNum).EmpName;;
						break;
					#region Relat
					case "RelatIsSelf":
						if(_claim.PatRelat==Relat.Self) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "RelatIsSpouse":
						if(_claim.PatRelat==Relat.Spouse) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "RelatIsChild":
						if(_claim.PatRelat==Relat.Child) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "RelatIsOther":
						if(_claim.PatRelat==Relat.Dependent
							|| _claim.PatRelat==Relat.Employee
							|| _claim.PatRelat==Relat.HandicapDep
							|| _claim.PatRelat==Relat.InjuredPlaintiff
							|| _claim.PatRelat==Relat.LifePartner
							|| _claim.PatRelat==Relat.SignifOther) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Relationship":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							if(_claim.PatRelat==Relat.Self) {
								_stringArrayDisplay[i]="Self";
							}
							else if(_claim.PatRelat==Relat.Spouse) {
								_stringArrayDisplay[i]="Spouse";
							}
							else if(_claim.PatRelat==Relat.Child) {
								_stringArrayDisplay[i]="Child";
							}
							else if(_claim.PatRelat==Relat.SignifOther || _claim.PatRelat==Relat.LifePartner) {
								_stringArrayDisplay[i]="Common Law Spouse";
							}
							else {
								_stringArrayDisplay[i]="Other";
							}
						}
						else {
							_stringArrayDisplay[i]=_claim.PatRelat.ToString();
						}
						break;
					#endregion Relat
					case "IsFTStudent":
						if(patient.StudentStatus=="F") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsPTStudent":
						if(patient.StudentStatus=="P") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsStudent":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")//Canadian. en-CA or fr-CA
							&& (patient.CanadianEligibilityCode==1 || patient.CanadianEligibilityCode==3))//Patient is a student
						{
							_stringArrayDisplay[i]="X";
						}
						else if(patient.StudentStatus=="P" || patient.StudentStatus=="F") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "CollegeName":
						_stringArrayDisplay[i]=patient.SchoolName;
						break;
					#region Patient
					case "PatientLastFirst":
						_stringArrayDisplay[i]=patient.LName+", "+patient.FName+" "+patient.MiddleI;
						break;
					case "PatientLastFirstMiCommas"://Medical required format for UB04 printed claims
						_stringArrayDisplay[i]=patient.LName+", "+patient.FName+(patient.MiddleI==""?"":(", "+patient.MiddleI[0]));
						break;
					case "PatientFirstMiddleLast":
						_stringArrayDisplay[i]=patient.FName+" "+patient.MiddleI+" "+patient.LName;
						break;
					case "PatientFirstName":
						_stringArrayDisplay[i] = patient.FName;
						break;
					case "PatientMiddleName":
						_stringArrayDisplay[i] = patient.MiddleI;
						break;
					case "PatientLastName":
						_stringArrayDisplay[i] = patient.LName;
						break;
					case "PatientAddress":
						_stringArrayDisplay[i]=patient.Address;
						break;
					case "PatientAddress2":
						_stringArrayDisplay[i]=patient.Address2;
						break;
					case "PatientAddressComplete":
						_stringArrayDisplay[i]=patient.Address+" "+patient.Address2;
						break;
					case "PatientCity":
						_stringArrayDisplay[i]=patient.City;
						break;
					case "PatientST":
						_stringArrayDisplay[i]=patient.State;
						break;
					case "PatientZip":
						_stringArrayDisplay[i]=patient.Zip;
						break;
					case "PatientPhone"://needs work.  Only used for 1500
						if(isRenaissance) {
							//Expecting (XXX)XXX-XXXX
							_stringArrayDisplay[i]=patient.HmPhone;
							if(patient.HmPhone.Length>14) {//Might have a note following the number.
								_stringArrayDisplay[i]=patient.HmPhone.Substring(0,14);
							}
						}
						else {
							string patientPhone=patient.HmPhone.Replace("(","");
							patientPhone=patientPhone.Replace(")","    ");
							patientPhone=patientPhone.Replace("-","  ");
							_stringArrayDisplay[i]=patientPhone;
						}
						break;
					case "PatientDOB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=patient.Birthdate.ToShortDateString();//MM/dd/yyyy
						}
						else {
							_stringArrayDisplay[i]=patient.Birthdate.ToString
								(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "PatientIsMale":
						if(patient.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PatientIsFemale":
						if(patient.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PatientIsGenderUnknown":
						if(patient.Gender==PatientGender.Unknown || patient.Gender==PatientGender.Other) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PatientGender":
						if(patient.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="Male";
						}
						else if(patient.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="Female";
						}
						else {//Unknown
							_stringArrayDisplay[i]="Unknown";
						}
						break;
					case "PatientGenderLetter":
						if(patient.Gender==PatientGender.Male) {
							_stringArrayDisplay[i]="M";
						}
						else if(patient.Gender==PatientGender.Female) {
							_stringArrayDisplay[i]="F";
						}
						else {//Unknown
							_stringArrayDisplay[i]="U";
						}
						break;
					case "PatientIsMarried":
						if(patient.Position==PatientPosition.Married) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PatientIsSingle":
						if(patient.Position==PatientPosition.Single
							|| patient.Position==PatientPosition.Child
							|| patient.Position==PatientPosition.Widowed) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PatIDFromPatPlan": //Dependant Code for Canada
						listPatPlans=PatPlans.Refresh(PatNum);
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && _carrier.ElectID=="000064") {//Canadian. en-CA or fr-CA and Pacific Blue Cross
							_stringArrayDisplay[i]=_insSub.SubscriberID+"-"+PatPlans.GetPatID(_insSub.InsSubNum,listPatPlans);
						}
						else {
							_stringArrayDisplay[i]=PatPlans.GetPatID(_insSub.InsSubNum,listPatPlans);
						}
						break;
					case "PatientSSN":
						if(patient.SSN.Length==9) {
							_stringArrayDisplay[i]=patient.SSN.Substring(0,3)
								+"-"+patient.SSN.Substring(3,2)
								+"-"+patient.SSN.Substring(5);
						}
						else {
							_stringArrayDisplay[i]=patient.SSN;
						}
						break;
					case "PatientMedicaidID":
						_stringArrayDisplay[i]=patient.MedicaidID;
						break;
					case "PatientID-MedicaidOrSSN":
						if(patient.MedicaidID!="") {
							_stringArrayDisplay[i]=patient.MedicaidID;
						}
						else {
							_stringArrayDisplay[i]=patient.SSN;
						}
						break;
					case "PatientChartNum":
						_stringArrayDisplay[i]=patient.ChartNumber;
						break;
					case "PatientPatNum":
						_stringArrayDisplay[i]=patient.PatNum.ToString();
						break;
					#endregion Patient
					#region Diagnosis
					case "Diagnosis1":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[0];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[0].Replace(".","");
						}
						break;
					case "Diagnosis2":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[1];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[1].Replace(".","");
						}
						break;
					case "Diagnosis3":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[2];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[2].Replace(".","");
						}
						break;
					case "Diagnosis4":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[3];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayDiagnoses[3].Replace(".","");
						}
						break;
					case "DiagnosisA":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[0];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[0].Replace(".","");
						}
						break;
					case "DiagnosisB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[1];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[1].Replace(".","");
						}
						break;
					case "DiagnosisC":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[2];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[2].Replace(".","");
						}
						break;
					case "DiagnosisD":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[3];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[3].Replace(".","");
						}
						break;
					case "DiagnosisE":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[4];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[4].Replace(".","");
						}
						break;
					case "DiagnosisF":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[5];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[5].Replace(".","");
						}
						break;
					case "DiagnosisG":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[6];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[6].Replace(".","");
						}
						break;
					case "DiagnosisH":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[7];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[7].Replace(".","");
						}
						break;
					case "DiagnosisI":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[8];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[8].Replace(".","");
						}
						break;
					case "DiagnosisJ":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[9];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[9].Replace(".","");
						}
						break;
					case "DiagnosisK":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[10];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[10].Replace(".","");
						}
						break;
					case "DiagnosisL":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[11];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							_stringArrayDisplay[i]=_stringArrayAllDiagnoses[11].Replace(".","");
						}
						break;
					#endregion Diagnosis
			//this is where the procedures used to be
					#region Miss
					case "Miss1":
						if(listStringsMissingTeeth.Contains("1")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss2":
						if(listStringsMissingTeeth.Contains("2")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss3":
						if(listStringsMissingTeeth.Contains("3")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss4":
						if(listStringsMissingTeeth.Contains("4")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss5":
						if(listStringsMissingTeeth.Contains("5")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss6":
						if(listStringsMissingTeeth.Contains("6")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss7":
						if(listStringsMissingTeeth.Contains("7")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss8":
						if(listStringsMissingTeeth.Contains("8")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss9":
						if(listStringsMissingTeeth.Contains("9")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss10":
						if(listStringsMissingTeeth.Contains("10")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss11":
						if(listStringsMissingTeeth.Contains("11")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss12":
						if(listStringsMissingTeeth.Contains("12")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss13":
						if(listStringsMissingTeeth.Contains("13")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss14":
						if(listStringsMissingTeeth.Contains("14")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss15":
						if(listStringsMissingTeeth.Contains("15")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss16":
						if(listStringsMissingTeeth.Contains("16")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss17":
						if(listStringsMissingTeeth.Contains("17")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss18":
						if(listStringsMissingTeeth.Contains("18")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss19":
						if(listStringsMissingTeeth.Contains("19")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss20":
						if(listStringsMissingTeeth.Contains("20")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss21":
						if(listStringsMissingTeeth.Contains("21")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss22":
						if(listStringsMissingTeeth.Contains("22")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss23":
						if(listStringsMissingTeeth.Contains("23")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss24":
						if(listStringsMissingTeeth.Contains("24")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss25":
						if(listStringsMissingTeeth.Contains("25")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss26":
						if(listStringsMissingTeeth.Contains("26")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss27":
						if(listStringsMissingTeeth.Contains("27")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss28":
						if(listStringsMissingTeeth.Contains("28")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss29":
						if(listStringsMissingTeeth.Contains("29")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss30":
						if(listStringsMissingTeeth.Contains("30")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss31":
						if(listStringsMissingTeeth.Contains("31")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "Miss32":
						if(listStringsMissingTeeth.Contains("32")) {
							_stringArrayDisplay[i]="X";
						}
						break;
					#endregion Miss
					case "Remarks":
						_stringArrayDisplay[i]="";
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							if(_claim.ClaimType=="PreAuth") {
								_stringArrayDisplay[i]+="Predetermination only."+Environment.NewLine;
							}
							else {
								if(Claims.GetAssignmentOfBenefits(_claim,_insSub)) {
									_stringArrayDisplay[i]+="Please pay provider."+Environment.NewLine;
								}
								else {
									_stringArrayDisplay[i]+="Please pay patient."+Environment.NewLine;
								}
							}
						}
						if(_claim.AttachmentID!="" && !_claim.ClaimNote.StartsWith(_claim.AttachmentID)){
							_stringArrayDisplay[i]=_claim.AttachmentID+" ";
						}
						_stringArrayDisplay[i]+=_claim.ClaimNote;
						break;
					case "PatientRelease":
						if(_insSub.ReleaseInfo) {
							_stringArrayDisplay[i]="Signature on File";
						}
						break;
					case "PatientReleaseDate":
						if(_insSub.ReleaseInfo && _claim.DateSent.Year > 1860) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.DateSent.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						} 
						break;
					case "PatientAssignment":
						if(Claims.GetAssignmentOfBenefits(_claim,_insSub)) {
							_stringArrayDisplay[i]="Signature on File";
						}
						break;
					case "PatientAssignmentDate":
						if(Claims.GetAssignmentOfBenefits(_claim,_insSub) && _claim.DateSent.Year > 1860) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.DateSent.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					#region Place
					case "PlaceIsOffice":
						if(_claim.PlaceService==PlaceOfService.Office) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsHospADA2002":
						if(_claim.PlaceService==PlaceOfService.InpatHospital
							|| _claim.PlaceService==PlaceOfService.OutpatHospital) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsExtCareFacilityADA2002":
						if(_claim.PlaceService==PlaceOfService.CustodialCareFacility
							|| _claim.PlaceService==PlaceOfService.SkilledNursFac) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsOtherADA2002":
						if(_claim.PlaceService==PlaceOfService.PatientsHome
							|| _claim.PlaceService==PlaceOfService.OtherLocation) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsInpatHosp":
						if(_claim.PlaceService==PlaceOfService.InpatHospital) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsOutpatHosp":
						if(_claim.PlaceService==PlaceOfService.OutpatHospital) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsAdultLivCareFac":
						if(_claim.PlaceService==PlaceOfService.CustodialCareFacility) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsSkilledNursFac":
						if(_claim.PlaceService==PlaceOfService.SkilledNursFac) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsPatientsHome":
						if(_claim.PlaceService==PlaceOfService.PatientsHome) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceIsOtherLocation":
						if(_claim.PlaceService==PlaceOfService.OtherLocation) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "PlaceNumericCode":
						_stringArrayDisplay[i]=X12object.GetPlaceService(_claim.PlaceService);
						break;
					#endregion Place
					case "IsRadiographsAttached":
						if(_claim.Radiographs>0) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "RadiographsNumAttached":
						_stringArrayDisplay[i]=_claim.Radiographs.ToString();
						break;
					case "RadiographsNotAttached":
						if(_claim.Radiographs==0) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsEnclosuresAttached":
						if(_claim.Radiographs>0 || _claim.AttachedImages>0 || _claim.AttachedModels>0 || _claim.AttachedFlags.Contains("EoB") 
							|| _claim.AttachedFlags.Contains("Note") || _claim.AttachedFlags.Contains("Perio") || _claim.AttachedFlags.Contains("Misc")) {
							_stringArrayDisplay[i]="Y";
						}
						else {
							_stringArrayDisplay[i]="N";
						}
						break;
					case "AttachedImagesNum":
						_stringArrayDisplay[i]=_claim.AttachedImages.ToString();
						break;
					case "AttachedModelsNum":
						_stringArrayDisplay[i]=_claim.AttachedModels.ToString();
						break;
					#region Ortho
					case "IsNotOrtho":
						if(!_claim.IsOrtho) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsOrtho":
						if(_claim.IsOrtho) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "DateOrthoPlaced":
						if(_claim.OrthoDate.Year > 1880){
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.OrthoDate.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.OrthoDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "MonthsOrthoRemaining":
						if(_claim.OrthoRemainM > 0) {
							_stringArrayDisplay[i]=_claim.OrthoRemainM.ToString();
						}
						break;
					case "MonthsOrthoTotal":
						if(_claim.OrthoTotalM > 0) {
							_stringArrayDisplay[i]=_claim.OrthoTotalM.ToString();
						}
						break;
					#endregion Ortho
					#region Prosth
					case "IsNotProsth":
						if(_claim.IsProsthesis=="N") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsInitialProsth":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							//Canadian claim form has two sections for initial/replacement prosthesis. Checkbox is only checked if maxiliar and mandibular settings
							//do not conflict, i.e. if user chooses Max=Initial and Mand=Replacment, box left blank.  However, Max=Initial and Mand=NotAProsthesis 
							//will check this box.
							((_claim.CanadianIsInitialUpper=="Y" && _claim.CanadianIsInitialLower!="N") 
								|| (_claim.CanadianIsInitialLower=="Y" && _claim.CanadianIsInitialUpper!="N")))
						{ 
							_stringArrayDisplay[i]="X";
						}
						else if(_claim.IsProsthesis=="I") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsNotReplacementProsth":
						if(_claim.IsProsthesis!="R") {//=='I'nitial or 'N'o
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsReplacementProsth":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							//Canadian claim form has two sections for initial/replacement prosthesis. Checkbox is only checked if maxiliar and mandibular settings
							//do not conflict, i.e. if user chooses Max=Initial and Mand=Replacment, box left blank.  However, Max=Replacement and 
							//Mand=NotAProsthesis will check this box.
							((_claim.CanadianIsInitialUpper=="N" && _claim.CanadianIsInitialLower!="Y")
								|| (_claim.CanadianIsInitialLower=="N" && _claim.CanadianIsInitialUpper!="Y")))
						{ 
							_stringArrayDisplay[i]="X";
						}
						else if(_claim.IsProsthesis=="R") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "DatePriorProsthPlaced":
						if(_claim.PriorDate.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.PriorDate.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.PriorDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					#endregion Prosth
					#region Occupational and Accident
					case "IsOccupational":
						if(_claim.AccidentRelated=="E") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsNotOccupational":
						if(_claim.AccidentRelated!="E") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsAutoAccident":
						if(_claim.AccidentRelated=="A") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsNotAutoAccident":
						if(_claim.AccidentRelated!="A") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsOtherAccident":
						if(_claim.AccidentRelated=="O") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsNotOtherAccident":
						if(_claim.AccidentRelated!="O") {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsNotAccident":
						if(_claim.AccidentRelated=="" && (!CultureInfo.CurrentCulture.Name.EndsWith("CA") || _claim.AccidentDate.Year <= 1860)) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "IsAccident":
						if(_claim.AccidentRelated!="" || (CultureInfo.CurrentCulture.Name.EndsWith("CA") && _claim.AccidentDate.Year > 1860)) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "AccidentDate":
						if(_claim.AccidentDate.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.AccidentDate.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.AccidentDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "AccidentST":
						_stringArrayDisplay[i]=_claim.AccidentST;
						break;
					#endregion Occupational and Accident
					#region BillingDentist
					case "BillingDentist":
						if(providerClaimBill.IsNotPerson) {
							_stringArrayDisplay[i]=providerClaimBill.LName+" "+providerClaimBill.Suffix;
						}
						else {
							_stringArrayDisplay[i]=providerClaimBill.FName+" "+providerClaimBill.MI+" "+providerClaimBill.LName+" "+providerClaimBill.Suffix;
						}
						break;
					case "BillingDentistMedicaidID":
						_stringArrayDisplay[i]=providerClaimBill.MedicaidID;
						break;
					case "BillingDentistProviderID":
						ProviderIdent[] providerIdentArray=ProviderIdents.GetForPayor(_claim.ProvBill,_carrier.ElectID);
						if(providerIdentArray.Length>0){
							_stringArrayDisplay[i]=providerIdentArray[0].IDNumber;//just use the first one we find
						}
						break;
					case "BillingDentistNPI":
						_stringArrayDisplay[i]=providerClaimBill.NationalProvID;
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							_carrier.ElectID=="000064" && //Pacific Blue Cross (PBC)
							providerClaimBill.NationalProvID!=providerClaimTreat.NationalProvID && //Billing and treating providers are different
							_stringArrayDisplay[i].Length==9) { //Only for provider numbers which have been entered correctly (to prevent and indexing exception).
							_stringArrayDisplay[i]="00"+_stringArrayDisplay[i].Substring(2,5)+"00";
						}
						break;
					case "BillingDentistLicenseNum":
						_stringArrayDisplay[i]=(provClinicClaimBill==null ? "" : provClinicClaimBill.StateLicense);
						break;
					case "BillingDentistSSNorTIN":
						_stringArrayDisplay[i]=providerClaimBill.SSN;
						break;
					case "BillingDentistNumIsSSN":
						if(!providerClaimBill.UsingTIN) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "BillingDentistNumIsTIN":
						if(providerClaimBill.UsingTIN) {
							_stringArrayDisplay[i]="X";
						}
						break;
					case "BillingDentistPh123":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(0,3);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(0,3);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(0,3);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(0,3);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(0,3);
						}
						break;
					case "BillingDentistPh456":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(3,3);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(3,3);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(3,3);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(3,3);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(3,3);
						}
						break;
					case "BillingDentistPh78910":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(6);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(6);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(6);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(6);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(6);
						}
						break;
					case "BillingDentistPhoneFormatted":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePayToPhone));
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(clinic.Phone);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticeBillingPhone));
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(clinic.Phone);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
						  _stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
						}
						break;
					case "BillingDentistPhoneRaw"://Behaves just like the old BillingDentistAddress field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone);
						}
						break;
					#endregion BillingDentist
					#region PayToDentist
					case "PayToDentistAddress": //Behaves just like the old BillingDentistAddress field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							_stringArrayDisplay[i]=clinic.PayToAddress;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToAddress);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.BillingAddress!="") {
							_stringArrayDisplay[i]=clinic.BillingAddress;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingAddress)!="") {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingAddress);
						}
						else if(clinic!=null && clinic.Address!="") {
							_stringArrayDisplay[i]=clinic.Address;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeAddress);
						}
						break;
					case "PayToDentistAddress2": //Behaves just like the old BillingDentistAddress2 field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							_stringArrayDisplay[i]=clinic.PayToAddress2;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToAddress2);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.BillingAddress!="") {
							_stringArrayDisplay[i]=clinic.BillingAddress2;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingAddress)!="") {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingAddress2);
						}
						else if(clinic!=null && clinic.Address!="") {
							_stringArrayDisplay[i]=clinic.Address2;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeAddress2);
						}
						break;
					case "PayToDentistCity": //Behaves just like the old BillingDentistCity field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							_stringArrayDisplay[i]=clinic.PayToCity;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToCity);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.BillingAddress!="") {
							_stringArrayDisplay[i]=clinic.BillingCity;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingAddress)!="") {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingCity);
						}
						else if(clinic!=null && clinic.Address!="") {
							_stringArrayDisplay[i]=clinic.City;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeCity);
						}
						break;
					case "PayToDentistST": //Behaves just like the old BillingDentistST field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							_stringArrayDisplay[i]=clinic.PayToState;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToST);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.BillingAddress!="") {
							_stringArrayDisplay[i]=clinic.BillingState;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingAddress)!="") {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingST);
						}
						else if(clinic!=null && clinic.Address!="") {
							_stringArrayDisplay[i]=clinic.State;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeST);
						}
						break;
					case "PayToDentistZip": //Behaves just like the old BillingDentistZip field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							_stringArrayDisplay[i]=clinic.PayToZip;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToZip);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.BillingAddress!="") {
							_stringArrayDisplay[i]=clinic.BillingZip;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingAddress)!="") {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingZip);
						}
						else if(clinic!=null && clinic.Address!="") {
							_stringArrayDisplay[i]=clinic.Zip;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeZip);
						}
						break;
					case "PayToDentistPh123":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(0,3);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(0,3);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(0,3);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(0,3);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(0,3);
						}
						break;
					case "PayToDentistPh456":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(3,3);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(3,3);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(3,3);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(3,3);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(3,3);
						}
						break;
					case "PayToDentistPh78910":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone).Substring(6);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(6);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone).Substring(6);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone.Substring(6);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(6);
						}
						break;
					case "PayToDentistPhoneFormatted":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePayToPhone));
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(clinic.Phone);
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
						  _stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticeBillingPhone));
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(clinic.Phone);
						}
						else if(PrefC.GetString(PrefName.PracticePhone).Length==10) {
							_stringArrayDisplay[i]=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
						}
						break;
					case "PayToDentistPhoneRaw":
						if(PrefC.GetString(PrefName.PracticePayToAddress)!="" && PrefC.GetString(PrefName.PracticePayToPhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePayToPhone);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims) && PrefC.GetString(PrefName.PracticeBillingPhone).Length==10) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeBillingPhone);
						}
						else if(clinic!=null && clinic.Phone.Length==10) {
							_stringArrayDisplay[i]=clinic.Phone;
						}
						else {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone);
						}
						break;
					#endregion PayToDentist
					#region TreatingDentist
					case "TreatingDentist":
						if(providerClaimTreat.IsNotPerson) {
							_stringArrayDisplay[i]=providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
						}
						else {
							_stringArrayDisplay[i]=providerClaimTreat.FName+" "+providerClaimTreat.MI+" "+providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
						}
						break;
					case "TreatingDentistFName":
						_stringArrayDisplay[i]=providerClaimTreat.FName;
						break;
					case "TreatingDentistLName":
						_stringArrayDisplay[i]=providerClaimTreat.LName;
						break;
					case "TreatingDentistSignature":
						if(providerClaimTreat.SigOnFile){
							if(PrefC.GetBool(PrefName.ClaimFormTreatDentSaysSigOnFile)){
								_stringArrayDisplay[i]="Signature on File";
							}
							else{
								_stringArrayDisplay[i]=providerClaimTreat.FName+" "+providerClaimTreat.MI+" "+providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
							}
						}
						break;
					case "TreatingDentistSigDate":
						if(providerClaimTreat.SigOnFile && _claim.DateSent.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								_stringArrayDisplay[i]=_claim.DateSent.ToShortDateString();
							}
							else {
								_stringArrayDisplay[i]=_claim.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "TreatingDentistMedicaidID":
						_stringArrayDisplay[i]=providerClaimTreat.MedicaidID;
						break;
					case "TreatingDentistProviderID":
						providerIdentArray=ProviderIdents.GetForPayor(_claim.ProvTreat,_carrier.ElectID);
						if(providerIdentArray.Length>0) {
							_stringArrayDisplay[i]=providerIdentArray[0].IDNumber;//just use the first one we find
						}
						break;
					case "TreatingDentistNPI":
						_stringArrayDisplay[i]=providerClaimTreat.NationalProvID;
						break;
					case "TreatingDentistLicense":
						_stringArrayDisplay[i]=(providerClinicClaimTreat==null ? "" : providerClinicClaimTreat.StateLicense);
						break;
					case "TreatingDentistAddress":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeAddress);
						}
						else {
							_stringArrayDisplay[i]=clinic.Address;
						}
						break;
					case "TreatingDentistAddress2":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeAddress2);
						}
						else {
							_stringArrayDisplay[i]=clinic.Address2;
						}
						break;
					case "TreatingDentistCity":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeCity);
						}
						else {
							_stringArrayDisplay[i]=clinic.City;
						}
						break;
					case "TreatingDentistST":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeST);
						}
						else {
							_stringArrayDisplay[i]=clinic.State;
						}
						break;
					case "TreatingDentistZip":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticeZip);
						}
						else {
							_stringArrayDisplay[i]=clinic.Zip;
						}
						break;
					case "TreatingDentistPh123":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(0,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								_stringArrayDisplay[i]=clinic.Phone.Substring(0,3);
							}
						}
						break;
					case "TreatingDentistPh456":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(3,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								_stringArrayDisplay[i]=clinic.Phone.Substring(3,3);
							}
						}
						break;
					case "TreatingDentistPh78910":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone).Substring(6);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								_stringArrayDisplay[i]=clinic.Phone.Substring(6);
							}
						}
						break;
					case "TreatingDentistPhoneRaw":
						if(clinic==null) {
							_stringArrayDisplay[i]=PrefC.GetString(PrefName.PracticePhone);
						}
						else {
							_stringArrayDisplay[i]=clinic.Phone;
						}
						break;
					case "TreatingProviderSpecialty":
						_stringArrayDisplay[i]=X12Generator.GetTaxonomy(providerClaimTreat);
						break;
					#endregion TreatingDentist
					case "TotalPages":
						_stringArrayDisplay[i]="";//totalPages.ToString();//bugs with this field that we can't fix since we didn't write that code.
						break;
					case "ReferringProvNPI":
						if(_referral==null){
							_stringArrayDisplay[i]="";
						}
						else{
							_stringArrayDisplay[i]=_referral.NationalProvID;
						}
						break;
					case "ReferringProvNameFL":
						if(_referral==null){
							_stringArrayDisplay[i]="";
						}
						else{
							_stringArrayDisplay[i]=_referral.GetNameFL();
						}
						break;
					#region Med
					case "MedUniformBillType":
						_stringArrayDisplay[i]=_claim.UniformBillType;
						break;
					case "MedAdmissionTypeCode":
						_stringArrayDisplay[i]=_claim.AdmissionTypeCode;
						break;
					case "MedAdmissionSourceCode":
						_stringArrayDisplay[i]=_claim.AdmissionSourceCode;
						break;
					case "MedPatientStatusCode":
						_stringArrayDisplay[i]=_claim.PatientStatusCode;
						break;
					case "MedAccidentCode": //For UB04.
						if(_claim.AccidentRelated=="A") { //Auto accident
							_stringArrayDisplay[i]="01";
						}
						else if(_claim.AccidentRelated=="E") { //Employment related accident
							_stringArrayDisplay[i]="04";
						}
						break;
					#endregion Med
					case "ICDindicator"://For the 1500_02_12 claim form.
						byte icdVersion=0;
						List<byte> listBytesDxVersions=new List<byte>();
						Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(_listClaimProcs),false,listBytesDxVersions);
						if(listBytesDxVersions.Count>0) {
							icdVersion=listBytesDxVersions[0];
						}
						if(icdVersion==9) {
							_stringArrayDisplay[i]="9";
						}
						else if(icdVersion==10) {
							_stringArrayDisplay[i]="0";
						}
						break;
					case "ICDindicatorAB"://For the ADA 2012 claim form.
						icdVersion=0;
						listBytesDxVersions=new List<byte>();
						Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(_listClaimProcs),false,listBytesDxVersions);
						if(listBytesDxVersions.Count>0) {
							icdVersion=listBytesDxVersions[0];
						}
						if(icdVersion==9) {
							_stringArrayDisplay[i]="B";
						}
						else if(icdVersion==10) {
							_stringArrayDisplay[i]="AB";
						}
						break;
					case "AcceptAssignmentY":
						_stringArrayDisplay[i]=Claims.GetAssignmentOfBenefits(_claim,_insSub)?"X":"";
						break;
					case "AcceptAssignmentN":
						_stringArrayDisplay[i]=Claims.GetAssignmentOfBenefits(_claim,_insSub)?"":"X";
						break;
					case "ClaimIdentifier":
						_stringArrayDisplay[i]=_claim.ClaimIdentifier;
					break;
					case "OrigRefNum":
						_stringArrayDisplay[i]=_claim.OrigRefNum;
						break;
					case "CorrectionType":
						_stringArrayDisplay[i]="";//Original claim
						if(_claim.CorrectionType==ClaimCorrectionType.Replacement) {
							_stringArrayDisplay[i]="7";
						}
						else if(_claim.CorrectionType==ClaimCorrectionType.Void) {
							_stringArrayDisplay[i]="8";
						}
						break;
					case "DateIllnessInjuryPreg":
						if(_claim.DateIllnessInjuryPreg.Year>1880) {
							_stringArrayDisplay[i]=string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)?_claim.DateIllnessInjuryPreg.ToShortDateString():
								_claim.DateIllnessInjuryPreg.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateIllnessInjuryPregQualifier":
						_stringArrayDisplay[i]=_claim.DateIllnessInjuryPregQualifier==DateIllnessInjuryPregQualifier.None?"":((int)_claim.DateIllnessInjuryPregQualifier).ToString("000");
						break;
					case "DateLastSRP":
						procedure=Procedures.GetMostRecentSRP(_listProcedures);
						if(procedure!=null) {
							_stringArrayDisplay[i]=procedure.ProcDate.ToShortDateString();
						}
						break;
					case "DateOther":
						if(_claim.DateOther.Year>1880){
							_stringArrayDisplay[i]=string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)?_claim.DateOther.ToShortDateString():
								_claim.DateOther.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateOtherQualifier":
						_stringArrayDisplay[i]=_claim.DateOtherQualifier==DateOtherQualifier.None?"":((int)_claim.DateOtherQualifier).ToString("000");
						break;
					case "IsOutsideLab":
						_stringArrayDisplay[i]=_claim.IsOutsideLab?"X":"";
						break;
					case "IsNotOutsideLab":
						_stringArrayDisplay[i]=_claim.IsOutsideLab?"":"X";
						break;
					case "OfficeNumber":
						_stringArrayDisplay[i]=providerClaimTreat.CanadianOfficeNum;
						break;
					case "OutsideLabFee":
						if(!_claim.IsOutsideLab) {
							_stringArrayDisplay[i]="";
							break;
						}
						//if this is for an outside lab fee there should only be one procedure on the claim and that should be for the outside fee.  We will use
						//the claim fee, it's up to the user to only include the outside lab fee on the claim.
						if(string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)) {
							_stringArrayDisplay[i]=((decimal)_claim.ClaimFee).ToString("F");
						}
						else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
							_stringArrayDisplay[i]=((decimal)_claim.ClaimFee).ToString("F").Replace("."," ");
						}
						else {
							_stringArrayDisplay[i]=((decimal)_claim.ClaimFee).ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
				}//switch
				if(CultureInfo.CurrentCulture.Name=="nl-BE"	&& _stringArrayDisplay[i]==""){//Dutch Belgium
					_stringArrayDisplay[i]="*   *   *";
				}
				//Renaissance eclaims only: Remove newlines from display strings to prevent formatting issues, because the .rss file format requires each field on a single line.
				if(isRenaissance && _stringArrayDisplay[i]!=null) {
					_stringArrayDisplay[i]=_stringArrayDisplay[i].Replace("\r","").Replace("\n","");
				}
			}//for
			return true;
		}

		/// <summary></summary>
		/// <param name="startProc">For page 1, this will be 0, otherwise it might be 10, 8, 20, or whatever.  It is the 0-based index of the first proc. Depends on how many procedures this claim format can display and which page we are on.</param>
		/// <param name="totProcs">The number of procedures that can be displayed or printed per claim form.  Depends on the individual claim format. For example, 10 on the ADA2002</param>
		private void FillProcStrings(int startProc,int totProcs){
			int qty;
			int toothCount;
			for(int i=0;i<ClaimFormCur.Items.Count;i++){
				if(ClaimFormCur.Items[i]==null){//Renaissance does not use [0]
					continue;
				}
				InsPlan insPlan = InsPlans.GetPlan(_claim.PlanNum, _listInsPlans);
				qty = 0;
				switch(ClaimFormCur.Items[i].FieldName){
					//there is no default, because any non-matches will remain as ""
					#region P1
					case "P1SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(1,startProc);
						break;
					case "P1SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(1,startProc);
						break;
					case "P1Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",1+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P1Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",1+startProc);
						break;
					case "P1System":
						_stringArrayDisplay[i]=GetProcInfo("System",1+startProc);
						break;
					case "P1ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",1+startProc);
						break;
					case "P1Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",1+startProc);
						break;
					case "P1Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",1+startProc);
						break;
					case "P1Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",1+startProc);
						break;
					case "P1Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",1+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P1TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",1+startProc);
						break;
					case "P1PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",1+startProc);
						break;
					case "P1Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",1+startProc);
						break;
					case "P1DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",1+startProc);
						break;
					case "P1Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",1+startProc);
						break;
					case "P1FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",1+startProc);
						break;
					case "P1ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",1+startProc);
						break;
					case "P1TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",1+startProc);
						break;
					case "P1RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",1+startProc);
						break;
					case "P1CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",1+startProc);
						break;
					case "P1CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",1+startProc);
						break;
					case "P1CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",1+startProc);
						break;
					case "P1CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",1+startProc);
						break;
					case "P1Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",1+startProc);
						break;
					case "P1UnitQty":
						if(insPlan.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",1+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",1+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",1+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",1+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P1UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",1+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(1,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P1CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",1+startProc) + GetProcInfo("CodeMod1",1+startProc) + GetProcInfo("CodeMod2",1+startProc) + GetProcInfo("CodeMod3",1+startProc) + GetProcInfo("CodeMod4",1+startProc);
						break;
					case "P1IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",1+startProc);
						break;
					case "P1eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",1+startProc);
						break;
					#endregion P1
					#region P2
					case "P2SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(2,startProc);
						break;
					case "P2SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(2,startProc);
						break;
					case "P2Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",2+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P2Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",2+startProc);
						break;
					case "P2System":
						_stringArrayDisplay[i]=GetProcInfo("System",2+startProc);
						break;
					case "P2ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",2+startProc);
						break;
					case "P2Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",2+startProc);
						break;
					case "P2Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",2+startProc);
						break;
					case "P2Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",2+startProc);
						break;
					case "P2Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",2+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P2TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",2+startProc);
						break;
					case "P2PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",2+startProc);
						break;
					case "P2Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",2+startProc);
						break;
					case "P2DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",2+startProc);
						break;
					case "P2Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",2+startProc);
						break;
					case "P2FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",2+startProc);
						break;
					case "P2ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",2+startProc);
						break;
					case "P2TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",2+startProc);
						break;
					case "P2RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",2+startProc);
						break;
					case "P2CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",2+startProc);
						break;
					case "P2CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",2+startProc);
						break;
					case "P2CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",2+startProc);
						break;
					case "P2CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",2+startProc);
						break;
					case "P2Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",2+startProc);
						break;
					case "P2UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",2+startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",2+startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",2+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",2+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P2UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",2+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(2,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P2CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",2+startProc) 
							+ GetProcInfo("CodeMod1",2+startProc) 
							+ GetProcInfo("CodeMod2",2+startProc) 
							+ GetProcInfo("CodeMod3",2+startProc) 
							+ GetProcInfo("CodeMod4",2+startProc);
						break;
					case "P2IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",2+startProc);
						break;
					case "P2eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",2+startProc);
						break;
					#endregion P2
					#region P3
					case "P3SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(3,startProc);
						break;
					case "P3SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(3,startProc);
						break;
					case "P3Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",3+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P3Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",3+startProc);
						break;
					case "P3System":
						_stringArrayDisplay[i]=GetProcInfo("System",3+startProc);
						break;
					case "P3ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",3+startProc);
						break;
					case "P3Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",3+startProc);
						break;
					case "P3Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",3+startProc);
						break;
					case "P3Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",3+startProc);
						break;
					case "P3Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",3+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P3TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",3+startProc);
						break;
					case "P3PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",3+startProc);
						break;
					case "P3Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",3+startProc);
						break;
					case "P3DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",3+startProc);
						break;
					case "P3Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",3+startProc);
						break;
					case "P3FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",3+startProc);
						break;
					case "P3ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",3+startProc);
						break;
					case "P3TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",3+startProc);
						break;
					case "P3RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",3+startProc);
						break;
					case "P3CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",3+startProc);
						break;
					case "P3CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",3+startProc);
						break;
					case "P3CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",3+startProc);
						break;
					case "P3CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",3+startProc);
						break;
					case "P3Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",3+startProc);
						break;
					case "P3UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",3+startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",3+startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",3+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",3+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P3UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",3+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(3,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P3CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",3+startProc) 
							+ GetProcInfo("CodeMod1",3+startProc) 
							+ GetProcInfo("CodeMod2",3+startProc) 
							+ GetProcInfo("CodeMod3",3+startProc) 
							+ GetProcInfo("CodeMod4",3+startProc);
						break;
					case "P3IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",3+startProc);
						break;
					case "P3eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",3+startProc);
						break;
					#endregion P3
					#region P4
					case "P4SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(4,startProc);
						break;
					case "P4SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(4,startProc);
						break;
					case "P4Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",4+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P4Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",4+startProc);
						break;
					case "P4System":
						_stringArrayDisplay[i]=GetProcInfo("System",4+startProc);
						break;
					case "P4ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",4+startProc);
						break;
					case "P4Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",4+startProc);
						break;
					case "P4Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",4+startProc);
						break;
					case "P4Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",4+startProc);
						break;
					case "P4Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",4+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P4TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",4+startProc);
						break;
					case "P4PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",4+startProc);
						break;
					case "P4Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",4+startProc);
						break;
					case "P4DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",4+startProc);
						break;
					case "P4Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",4+startProc);
						break;
					case "P4FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",4+startProc);
						break;
					case "P4ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",4+startProc);
						break;
					case "P4TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",4+startProc);
						break;
					case "P4RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",4+startProc);
						break;
					case "P4CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",4+startProc);
						break;
					case "P4CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",4+startProc);
						break;
					case "P4CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",4+startProc);
						break;
					case "P4CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",4+startProc);
						break;
					case "P4Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",4+startProc);
						break;
					case "P4UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",4+startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",4+startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",4+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",4+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P4UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",4+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(4,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P4CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",4+startProc) 
							+ GetProcInfo("CodeMod1",4+startProc) 
							+ GetProcInfo("CodeMod2",4+startProc) 
							+ GetProcInfo("CodeMod3",4+startProc) 
							+ GetProcInfo("CodeMod4",4+startProc);
						break;
					case "P4IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",4+startProc);
						break;
					case "P4eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",4+startProc);
						break;
					#endregion P4
					#region P5
					case "P5SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(5,startProc);
						break;
					case "P5SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(5,startProc);
						break;
					case "P5Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",5+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P5Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",5+startProc);
						break;
					case "P5System":
						_stringArrayDisplay[i]=GetProcInfo("System",5+startProc);
						break;
					case "P5ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",5+startProc);
						break;
					case "P5Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",5+startProc);
						break;
					case "P5Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",5+startProc);
						break;
					case "P5Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",5+startProc);
						break;
					case "P5Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",5+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P5TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",5+startProc);
						break;
					case "P5PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",5+startProc);
						break;
					case "P5Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",5+startProc);
						break;
					case "P5DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",5+startProc);
						break;
					case "P5Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",5+startProc);
						break;
					case "P5FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",5+startProc);
						break;
					case "P5ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",5+startProc);
						break;
					case "P5TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",5+startProc);
						break;
					case "P5RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",5+startProc);
						break;
					case "P5CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",5+startProc);
						break;
					case "P5CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",5+startProc);
						break;
					case "P5CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",5+startProc);
						break;
					case "P5CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",5+startProc);
						break;
					case "P5Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",5+startProc);
						break;
					case "P5UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",5+startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",5+startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",5+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",5+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P5UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",5+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(5,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P5CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",5+startProc) 
							+ GetProcInfo("CodeMod1",5+startProc) 
							+ GetProcInfo("CodeMod2",5+startProc) 
							+ GetProcInfo("CodeMod3",5+startProc) 
							+ GetProcInfo("CodeMod4",5+startProc);
						break;
					case "P5IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",5+startProc);
						break;
					case "P5eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",5+startProc);
						break;
					#endregion P5
					#region P6
					case "P6SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(6,startProc);
						break;
					case "P6SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(6,startProc);
						break;
					case "P6Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",6+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P6Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",6+startProc);
						break;
					case "P6System":
						_stringArrayDisplay[i]=GetProcInfo("System",6+startProc);
						break;
					case "P6ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",6+startProc);
						break;
					case "P6Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",6+startProc);
						break;
					case "P6Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",6+startProc);
						break;
					case "P6Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",6+startProc);
						break;
					case "P6Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",6+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P6TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",6+startProc);
						break;
					case "P6PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",6+startProc);
						break;
					case "P6Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",6+startProc);
						break;
					case "P6DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",6+startProc);
						break;
					case "P6Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",6+startProc);
						break;
					case "P6FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",6+startProc);
						break;
					case "P6ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",6+startProc);
						break;
					case "P6TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",6+startProc);
						break;
					case "P6RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",6+startProc);
						break;
					case "P6CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",6+startProc);
						break;
					case "P6CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",6+startProc);
						break;
					case "P6CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",6+startProc);
						break;
					case "P6CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",6+startProc);
						break;
					case "P6Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",6+startProc);
						break;
					case "P6UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",6+startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",6+startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",6+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",6+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P6UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",6+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(6,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P6CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",6+startProc) 
							+ GetProcInfo("CodeMod1",6+startProc) 
							+ GetProcInfo("CodeMod2",6+startProc) 
							+ GetProcInfo("CodeMod3",6+startProc) 
							+ GetProcInfo("CodeMod4",6+startProc);
						break;
					case "P6IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",6+startProc);
						break;
					case "P6eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",6+startProc);
						break;
					#endregion P6
					#region P7
					case "P7SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(7,startProc);
						break;
					case "P7SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(7,startProc);
						break;
					case "P7Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",7+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P7Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",7+startProc);
						break;
					case "P7System":
						_stringArrayDisplay[i]=GetProcInfo("System",7+startProc);
						break;
					case "P7ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",7+startProc);
						break;
					case "P7Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",7+startProc);
						break;
					case "P7Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",7+startProc);
						break;
					case "P7Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",7+startProc);
						break;
					case "P7Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",7+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P7TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",7+startProc);
						break;
					case "P7PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",7+startProc);
						break;
					case "P7Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",7+startProc);
						break;
					case "P7DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",7+startProc);
						break;
					case "P7Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",7+startProc);
						break;
					case "P7FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",7+startProc);
						break;
					case "P7ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",7+startProc);
						break;
					case "P7TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",7+startProc);
						break;
					case "P7RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",7+startProc);
						break;
					case "P7CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",7+startProc);
						break;
					case "P7CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",7+startProc);
						break;
					case "P7CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",7+startProc);
						break;
					case "P7CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",7+startProc);
						break;
					case "P7Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",7+startProc);
						break;
					case "P7UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",7 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",7 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",7+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",7+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P7UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",7+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(7,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P7CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",7+startProc) 
							+ GetProcInfo("CodeMod1",7+startProc) 
							+ GetProcInfo("CodeMod2",7+startProc) 
							+ GetProcInfo("CodeMod3",7+startProc) 
							+ GetProcInfo("CodeMod4",7+startProc);
						break;
					case "P7IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",7+startProc);
						break;
					case "P7eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",7+startProc);
						break;
					#endregion P7
					#region P8
					case "P8SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(8,startProc);
						break;
					case "P8SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(8,startProc);
						break;
					case "P8Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",8+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P8Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",8+startProc);
						break;
					case "P8System":
						_stringArrayDisplay[i]=GetProcInfo("System",8+startProc);
						break;
					case "P8ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",8+startProc);
						break;
					case "P8Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",8+startProc);
						break;
					case "P8Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",8+startProc);
						break;
					case "P8Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",8+startProc);
						break;
					case "P8Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",8+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P8TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",8+startProc);
						break;
					case "P8PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",8+startProc);
						break;
					case "P8Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",8+startProc);
						break;
					case "P8DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",8+startProc);
						break;
					case "P8Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",8+startProc);
						break;
					case "P8FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",8+startProc);
						break;
					case "P8ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",8+startProc);
						break;
					case "P8TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",8+startProc);
						break;
					case "P8RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",8+startProc);
						break;
					case "P8CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",8+startProc);
						break;
					case "P8CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",8+startProc);
						break;
					case "P8CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",8+startProc);
						break;
					case "P8CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",8+startProc);
						break;
					case "P8Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",8+startProc);
						break;
					case "P8UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",8 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",8 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",8+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",8+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P8UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",8+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(8,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P8IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",8+startProc);
						break;
					case "P8eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",8+startProc);
						break;
					#endregion P8
					#region P9
					case "P8CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",8+startProc) 
							+ GetProcInfo("CodeMod1",8+startProc) 
							+ GetProcInfo("CodeMod2",8+startProc) 
							+ GetProcInfo("CodeMod3",8+startProc) 
							+ GetProcInfo("CodeMod4",8+startProc);
						break;
					case "P9SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(9,startProc);
						break;
					case "P9SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(9,startProc);
						break;
					case "P9Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",9+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P9Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",9+startProc);
						break;
					case "P9System":
						_stringArrayDisplay[i]=GetProcInfo("System",9+startProc);
						break;
					case "P9ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",9+startProc);
						break;
					case "P9Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",9+startProc);
						break;
					case "P9Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",9+startProc);
						break;
					case "P9Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",9+startProc);
						break;
					case "P9Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",9+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P9TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",9+startProc);
						break;
					case "P9PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",9+startProc);
						break;
					case "P9Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",9+startProc);
						break;
					case "P9DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",9+startProc);
						break;
					case "P9Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",9+startProc);
						break;
					case "P9FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",9+startProc);
						break;
					case "P9ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",9+startProc);
						break;
					case "P9TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",9+startProc);
						break;
					case "P9RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",9+startProc);
						break;
					case "P9CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",9+startProc);
						break;
					case "P9CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",9+startProc);
						break;
					case "P9CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",9+startProc);
						break;
					case "P9CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",9+startProc);
						break;
					case "P9Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",9+startProc);
						break;
					case "P9UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",9 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",9 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",9+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",9+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P9UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",9+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(9,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P9CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",9+startProc) 
							+ GetProcInfo("CodeMod1",9+startProc) 
							+ GetProcInfo("CodeMod2",9+startProc) 
							+ GetProcInfo("CodeMod3",9+startProc) 
							+ GetProcInfo("CodeMod4",9+startProc);
						break;
					case "P9IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",9+startProc);
						break;
					case "P9eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",9+startProc);
						break;
					#endregion P9
					#region P10
					case "P10SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(10,startProc);
						break;
					case "P10SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(10,startProc);
						break;
					case "P10Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",10+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P10Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",10+startProc);
						break;
					case "P10System":
						_stringArrayDisplay[i]=GetProcInfo("System",10+startProc);
						break;
					case "P10ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",10+startProc);
						break;
					case "P10Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",10+startProc);
						break;
					case "P10Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",10+startProc);
						break;
					case "P10Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",10+startProc);
						break;
					case "P10Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",10+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P10TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",10+startProc);
						break;
					case "P10PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",10+startProc);
						break;
					case "P10Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",10+startProc);
						break;
					case "P10DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",10+startProc);
						break;
					case "P10Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",10+startProc);
						break;
					case "P10FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",10+startProc);
						break;
					case "P10ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",10+startProc);
						break;
					case "P10TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",10+startProc);
						break;
					case "P10RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",10+startProc);
						break;
					case "P10CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",10+startProc);
						break;
					case "P10CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",10+startProc);
						break;
					case "P10CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",10+startProc);
						break;
					case "P10CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",10+startProc);
						break;
					case "P10Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",10+startProc);
						break;
					case "P10UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",10 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",10 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",10+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",10+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P10UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",10+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(10,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P10CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",10+startProc) 
							+ GetProcInfo("CodeMod1",10+startProc) 
							+ GetProcInfo("CodeMod2",10+startProc) 
							+ GetProcInfo("CodeMod3",10+startProc) 
							+ GetProcInfo("CodeMod4",10+startProc);
						break;
					case "P10IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",10+startProc);
						break;
					case "P10eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",10+startProc);
						break;
					#endregion P10
					#region P11
					case "P11SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(11,startProc);
						break;
					case "P11SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(11,startProc);
						break;
					case "P11Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",11+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P11Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",11+startProc);
						break;
					case "P11System":
						_stringArrayDisplay[i]=GetProcInfo("System",11+startProc);
						break;
					case "P11ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",11+startProc);
						break;
					case "P11Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",11+startProc);
						break;
					case "P11Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",11+startProc);
						break;
					case "P11Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",11+startProc);
						break;
					case "P11Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",11+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P11TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",11+startProc);
						break;
					case "P11PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",11+startProc);
						break;
					case "P11Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",11+startProc);
						break;
					case "P11DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",11+startProc);
						break;
					case "P11Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",11+startProc);
						break;
					case "P11FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",11+startProc);
						break;
					case "P11ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",11+startProc);
						break;
					case "P11TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",11+startProc);
						break;
					case "P11RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",11+startProc);
						break;
					case "P11CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",11+startProc);
						break;
					case "P11CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",11+startProc);
						break;
					case "P11CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",11+startProc);
						break;
					case "P11CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",11+startProc);
						break;
					case "P11Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",11+startProc);
						break;
					case "P11UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",11 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",11 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",11+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",11+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P11UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",11+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(11,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P11CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",11+startProc)
							+ GetProcInfo("CodeMod1",11+startProc)
							+ GetProcInfo("CodeMod2",11+startProc)
							+ GetProcInfo("CodeMod3",11+startProc)
							+ GetProcInfo("CodeMod4",11+startProc);
						break;
					case "P11IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",11+startProc);
						break;
					case "P11eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",11+startProc);
						break;
					#endregion P11
					#region P12
					case "P12SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(12,startProc);
						break;
					case "P12SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(12,startProc);
						break;
					case "P12Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",12+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P12Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",12+startProc);
						break;
					case "P12System":
						_stringArrayDisplay[i]=GetProcInfo("System",12+startProc);
						break;
					case "P12ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",12+startProc);
						break;
					case "P12Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",12+startProc);
						break;
					case "P12Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",12+startProc);
						break;
					case "P12Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",12+startProc);
						break;
					case "P12Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",12+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P12TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",12+startProc);
						break;
					case "P12PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",12+startProc);
						break;
					case "P12Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",12+startProc);
						break;
					case "P12DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",12+startProc);
						break;
					case "P12Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",12+startProc);
						break;
					case "P12FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",12+startProc);
						break;
					case "P12ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",12+startProc);
						break;
					case "P12TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",12+startProc);
						break;
					case "P12RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",12+startProc);
						break;
					case "P12CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",12+startProc);
						break;
					case "P12CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",12+startProc);
						break;
					case "P12CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",12+startProc);
						break;
					case "P12CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",12+startProc);
						break;
					case "P12Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",12+startProc);
						break;
					case "P12UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",12 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",12 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",12+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",12+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P12UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",12+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(12,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P12CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",12+startProc)
							+ GetProcInfo("CodeMod1",12+startProc)
							+ GetProcInfo("CodeMod2",12+startProc)
							+ GetProcInfo("CodeMod3",12+startProc)
							+ GetProcInfo("CodeMod4",12+startProc);
						break;
					case "P12IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",12+startProc);
						break;
					case "P12eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",12+startProc);
						break;
					#endregion P12
					#region P13
					case "P13SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(13,startProc);
						break;
					case "P13SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(13,startProc);
						break;
					case "P13Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",13+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P13Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",13+startProc);
						break;
					case "P13System":
						_stringArrayDisplay[i]=GetProcInfo("System",13+startProc);
						break;
					case "P13ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",13+startProc);
						break;
					case "P13Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",13+startProc);
						break;
					case "P13Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",13+startProc);
						break;
					case "P13Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",13+startProc);
						break;
					case "P13Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",13+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P13TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",13+startProc);
						break;
					case "P13PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",13+startProc);
						break;
					case "P13Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",13+startProc);
						break;
					case "P13DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",13+startProc);
						break;
					case "P13Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",13+startProc);
						break;
					case "P13FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",13+startProc);
						break;
					case "P13ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",13+startProc);
						break;
					case "P13TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",13+startProc);
						break;
					case "P13RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",13+startProc);
						break;
					case "P13CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",13+startProc);
						break;
					case "P13CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",13+startProc);
						break;
					case "P13CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",13+startProc);
						break;
					case "P13CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",13+startProc);
						break;
					case "P13Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",13+startProc);
						break;
					case "P13UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",13 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",13 + startProc),out unitQty);
							qty = baseUnit + unitQty;
						}
						else if(GetProcInfo("UnitQty",13+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",13+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P13UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",13+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(13,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P13CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",13+startProc)
							+ GetProcInfo("CodeMod1",13+startProc)
							+ GetProcInfo("CodeMod2",13+startProc)
							+ GetProcInfo("CodeMod3",13+startProc)
							+ GetProcInfo("CodeMod4",13+startProc);
						break;
					case "P13IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",13+startProc);
						break;
					case "P13eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",13+startProc);
						break;
					#endregion P13
					#region P14
					case "P14SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(14,startProc);
						break;
					case "P14SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(14,startProc);
						break;
					case "P14Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",14+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P14Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",14+startProc);
						break;
					case "P14System":
						_stringArrayDisplay[i]=GetProcInfo("System",14+startProc);
						break;
					case "P14ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",14+startProc);
						break;
					case "P14Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",14+startProc);
						break;
					case "P14Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",14+startProc);
						break;
					case "P14Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",14+startProc);
						break;
					case "P14Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",14+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P14TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",14+startProc);
						break;
					case "P14PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",14+startProc);
						break;
					case "P14Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",14+startProc);
						break;
					case "P14DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",14+startProc);
						break;
					case "P14Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",14+startProc);
						break;
					case "P14FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",14+startProc);
						break;
					case "P14ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",14+startProc);
						break;
					case "P14TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",14+startProc);
						break;
					case "P14RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",14+startProc);
						break;
					case "P14CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",14+startProc);
						break;
					case "P14CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",14+startProc);
						break;
					case "P14CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",14+startProc);
						break;
					case "P14CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",14+startProc);
						break;
					case "P14Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",14+startProc);
						break;
					case "P14UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",14 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",14 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",14+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",14+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P14UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",14+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(14,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P14CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",14+startProc)
							+ GetProcInfo("CodeMod1",14+startProc)
							+ GetProcInfo("CodeMod2",14+startProc)
							+ GetProcInfo("CodeMod3",14+startProc)
							+ GetProcInfo("CodeMod4",14+startProc);
						break;
					case "P14IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",14+startProc);
						break;
					case "P14eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",14+startProc);
						break;
					#endregion P14
					#region P15
					case "P15SystemAndTeeth":
						_stringArrayDisplay[i]=GenerateSystemAndTeethField(15,startProc);
						break;
					case "P15SupplementalInfo":
						_stringArrayDisplay[i]=GenerateSupplementalInfoField(15,startProc);
						break;
					case "P15Date":
						_stringArrayDisplay[i]=GetProcInfo("Date",15+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P15Area":
						_stringArrayDisplay[i]=GetProcInfo("Area",15+startProc);
						break;
					case "P15System":
						_stringArrayDisplay[i]=GetProcInfo("System",15+startProc);
						break;
					case "P15ToothNumber":
						_stringArrayDisplay[i]=GetProcInfo("ToothNum",15+startProc);
						break;
					case "P15Surface":
						_stringArrayDisplay[i]=GetProcInfo("Surface",15+startProc);
						break;
					case "P15Code":
						_stringArrayDisplay[i]=GetProcInfo("Code",15+startProc);
						break;
					case "P15Description":
						_stringArrayDisplay[i]=GetProcInfo("Desc",15+startProc);
						break;
					case "P15Fee":
						_stringArrayDisplay[i]=GetProcInfo("Fee",15+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P15TreatDentMedicaidID":
						_stringArrayDisplay[i]=GetProcInfo("TreatDentMedicaidID",15+startProc);
						break;
					case "P15PlaceNumericCode":
						_stringArrayDisplay[i]=GetProcInfo("PlaceNumericCode",15+startProc);
						break;
					case "P15Diagnosis":
						_stringArrayDisplay[i]=GetProcInfo("Diagnosis",15+startProc);
						break;
					case "P15DiagnosisPoint":
						_stringArrayDisplay[i]=GetProcInfo("DiagnosisPoint",15+startProc);
						break;
					case "P15Lab":
						_stringArrayDisplay[i]=GetProcInfo("Lab",15+startProc);
						break;
					case "P15FeeMinusLab":
						_stringArrayDisplay[i]=GetProcInfo("FeeMinusLab",15+startProc);
						break;
					case "P15ToothNumOrArea":
						_stringArrayDisplay[i]=GetProcInfo("ToothNumOrArea",15+startProc);
						break;
					case "P15TreatProvNPI":
						_stringArrayDisplay[i]=GetProcInfo("TreatProvNPI",15+startProc);
						break;
					case "P15RevCode":
						_stringArrayDisplay[i]=GetProcInfo("RevCode",15+startProc);
						break;
					case "P15CodeMod1":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod1",15+startProc);
						break;
					case "P15CodeMod2":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod2",15+startProc);
						break;
					case "P15CodeMod3":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod3",15+startProc);
						break;
					case "P15CodeMod4":
						_stringArrayDisplay[i]=GetProcInfo("CodeMod4",15+startProc);
						break;
					case "P15Minutes":
						_stringArrayDisplay[i]=GetProcInfo("Minutes",15+startProc);
						break;
					case "P15UnitQty":
						if(insPlan.ShowBaseUnits) {
							short baseUnit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",15 + startProc),out baseUnit);
							short unitQty;
							System.Int16.TryParse(GetProcInfo("UnitQty",15 + startProc),out unitQty);
							qty=baseUnit+unitQty;
						}
						else if(GetProcInfo("UnitQty",15+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",15+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							_stringArrayDisplay[i]="";
						}
						else {
							_stringArrayDisplay[i]=qty.ToString();
						}
						break;
					case "P15UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",15+startProc));
						if(toothCount==-1) {//No toothrange specified
							_stringArrayDisplay[i]=CalculateUnitQtyField(15,startProc,insPlan);
						}
						else {
							_stringArrayDisplay[i]=toothCount.ToString();
						}
						break;
					case "P15CodeAndMods":
						_stringArrayDisplay[i]=GetProcInfo("Code",15+startProc)
							+ GetProcInfo("CodeMod1",15+startProc)
							+ GetProcInfo("CodeMod2",15+startProc)
							+ GetProcInfo("CodeMod3",15+startProc)
							+ GetProcInfo("CodeMod4",15+startProc);
						break;
					case "P15IsEmergency":
						_stringArrayDisplay[i]=GetProcInfo("IsEmergency",15+startProc);
						break;
					case "P15eClaimNote":
						_stringArrayDisplay[i]=GetProcInfo("eClaimNote",15+startProc);
						break;
					#endregion P15
					case "TotalFee":
						decimal fee=0;//fee only for this page. Each page is treated like a separate claim.
						for(int f=startProc;f<startProc+totProcs;f++) {//eg f=0;f<10;f++
							if(f < _listClaimProcs.Count) {
								fee+=(decimal)((ClaimProc)_listClaimProcs[f]).FeeBilled;
								if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
									List<Procedure> listProceduresLab=Procedures.GetCanadianLabFees(_listClaimProcs[f].ProcNum,_listProcedures);
									for(int j=0;j<listProceduresLab.Count;j++) {
										fee+=(decimal)listProceduresLab[j].ProcFee;
									}
								}
							}
						}
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=fee.ToString("F");
						}
						else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
							_stringArrayDisplay[i] = fee.ToString("F").Replace("."," ");
						}
						else {
							_stringArrayDisplay[i]=fee.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateService":
						if(ClaimFormCur.Items[i].FormatString=="") {
							_stringArrayDisplay[i]=_claim.DateService.ToShortDateString();
						}
						else {
							_stringArrayDisplay[i]=_claim.DateService.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
				}//switch
				if(CultureInfo.CurrentCulture.Name=="nl-BE" && _stringArrayDisplay[i]==""){//Dutch Belgium
					_stringArrayDisplay[i]="*   *   *";
				}
			}//for i
		}

		private void FillMedValueCodes(){
			_listClaimValCodeLog = ClaimValCodeLogs.GetForClaim(_claim.ClaimNum);
			if(_listClaimValCodeLog.Count>0){
				ClaimValCodeLog[] claimValCodeLogArray;
				claimValCodeLogArray=new ClaimValCodeLog[12];
				for(int i=0;i<_listClaimValCodeLog.Count;i++){
					claimValCodeLogArray[i]=(ClaimValCodeLog)_listClaimValCodeLog[i];
				}
				for(int i=_listClaimValCodeLog.Count;i<12;i++){
					claimValCodeLogArray[i]=new ClaimValCodeLog();
				}
				for(int i=0;i<ClaimFormCur.Items.Count;i++){
					switch(ClaimFormCur.Items[i].FieldName){
						case "MedValCode39a":
							_stringArrayDisplay[i]=claimValCodeLogArray[0].ValCode;
							break;
						case "MedValAmount39a":
							if(claimValCodeLogArray[0].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i]=claimValCodeLogArray[0].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[0].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39b":
							if(claimValCodeLogArray[3]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[3].ValCode;
							break;
						case "MedValAmount39b":
							if(claimValCodeLogArray[3].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[3].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[3].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39c":
							if(claimValCodeLogArray[6]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[6].ValCode;
							break;
						case "MedValAmount39c":
							if(claimValCodeLogArray[6].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[6].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[6].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39d":
							if(claimValCodeLogArray[9]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[9].ValCode;
							break;
						case "MedValAmount39d":
							if(claimValCodeLogArray[9].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[9].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[9].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40a":
							if(claimValCodeLogArray[1]!=null) {
								_stringArrayDisplay[i]=claimValCodeLogArray[1].ValCode;
							}
							break;
						case "MedValAmount40a":
							if(claimValCodeLogArray[1].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[1].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[1].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40b":
							if(claimValCodeLogArray[4]!=null) {
								_stringArrayDisplay[i]=claimValCodeLogArray[4].ValCode;
							}
							break;
						case "MedValAmount40b":
							if(claimValCodeLogArray[4].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[4].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[4].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40c":
							if(claimValCodeLogArray[7]!=null) {
								_stringArrayDisplay[i]=claimValCodeLogArray[7].ValCode;
							}
							break;
						case "MedValAmount40c":
							if(claimValCodeLogArray[7].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[7].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[7].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40d":
							if(claimValCodeLogArray[10]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[10].ValCode;
							break;
						case "MedValAmount40d":
							if(claimValCodeLogArray[10].ValAmount==0){
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[10].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[10].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41a":
							if(claimValCodeLogArray[2]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[2].ValCode;
							break;
						case "MedValAmount41a":
							if(claimValCodeLogArray[2].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[2].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[2].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41b":
							if(claimValCodeLogArray[5]!=null) {
								_stringArrayDisplay[i]=claimValCodeLogArray[5].ValCode;
							}
							break;
						case "MedValAmount41b":
							if(claimValCodeLogArray[5].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[5].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[5].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41c":
							if(claimValCodeLogArray[8]!=null) {
								_stringArrayDisplay[i]=claimValCodeLogArray[8].ValCode;
							}
							break;
						case "MedValAmount41c":
							if(claimValCodeLogArray[8].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[8].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[8].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41d":
							if(claimValCodeLogArray[11]!=null)
								_stringArrayDisplay[i]=claimValCodeLogArray[11].ValCode;
							break;
						case "MedValAmount41d":
							if(claimValCodeLogArray[11].ValAmount==0) {
								_stringArrayDisplay[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								_stringArrayDisplay[i] = claimValCodeLogArray[11].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								_stringArrayDisplay[i]=claimValCodeLogArray[11].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
					}
				}
			}
		}

		private void FillMedCondCodes(){
			ClaimCondCodeLog claimCondCodeLog=ClaimCondCodeLogs.GetByClaimNum(_claim.ClaimNum);
			if(claimCondCodeLog==null) {
				return;
			}
			for(int i=0;i<ClaimFormCur.Items.Count;i++) {
				switch(ClaimFormCur.Items[i].FieldName) {
					case "MedConditionCode18":
						_stringArrayDisplay[i]=claimCondCodeLog.Code0;
						break;
					case "MedConditionCode19":
						_stringArrayDisplay[i]=claimCondCodeLog.Code1;
						break;
					case "MedConditionCode20":
						_stringArrayDisplay[i]=claimCondCodeLog.Code2;
						break;
					case "MedConditionCode21":
						_stringArrayDisplay[i]=claimCondCodeLog.Code3;
						break;
					case "MedConditionCode22":
						_stringArrayDisplay[i]=claimCondCodeLog.Code4;
						break;
					case "MedConditionCode23":
						_stringArrayDisplay[i]=claimCondCodeLog.Code5;
						break;
					case "MedConditionCode24":
						_stringArrayDisplay[i]=claimCondCodeLog.Code6;
						break;
					case "MedConditionCode25":
						_stringArrayDisplay[i]=claimCondCodeLog.Code7;
						break;
					case "MedConditionCode26":
						_stringArrayDisplay[i]=claimCondCodeLog.Code8;
						break;
					case "MedConditionCode27":
						_stringArrayDisplay[i]=claimCondCodeLog.Code9;
						break;
					case "MedConditionCode28":
						_stringArrayDisplay[i]=claimCondCodeLog.Code10;
						break;
				}
			}
		}

		///<summary>These fields are used for the UB04.</summary>
		private void FillMedInsStrings(){
			PatPlan patPlan=null;
			for(int i=0;i<_listPatPlans.Count;i++) {
				if(_listPatPlans[i].InsSubNum==_claim.InsSubNum) {
					patPlan=_listPatPlans[i];
					break;
				}
			}
			string strInsFilingCode=InsFilingCodes.GetEclaimCode(_insPlan.FilingCode);
			//Determine the slot (A, B or C) where the medical insurance information belongs.
			string insLine="A";
			if(strInsFilingCode=="MC") { //Medicaid
				insLine="C";
			}
			else if(patPlan==null) {//Plan was dropped.
				//We only support one medical plan right now so defaulting to primary should be fine. 
			}
			else {
				//If not Medicaid, then primary is on line A, secondary is on line B, and Tertiary is on line C.
				if(patPlan.Ordinal==2) {
					insLine="B";
				}
				else if(patPlan.Ordinal==3) {
					insLine="C";
				}
			}
			//Determine the prior payments or total payments from other insurance companies.
			decimal priorPaymentsA=0;
			decimal priorPaymentsB=0;
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				for(int j=0;j<_listClaimProcsForPat.Count;j++) {
					if(!ClaimProcs.IsValidClaimAdj(_listClaimProcsForPat[j],_listClaimProcsForClaim[i].ProcNum,_listClaimProcsForClaim[i].InsSubNum)) {
						continue;
					}
					if(strInsFilingCode=="MC") { //Medicaid
						string strInsFilingCodeClaimProc="";
						for(int k=0;k<_listInsPlans.Count;k++) {
							if(_listInsPlans[k].PlanNum==_listClaimProcsForPat[j].PlanNum) {
								strInsFilingCodeClaimProc=InsFilingCodes.GetEclaimCode(_listInsPlans[k].FilingCode);
								break;
							}
						}
						if(strInsFilingCodeClaimProc=="16" || strInsFilingCodeClaimProc=="MB") {//HMO_MedicareRisk and MedicarePartB
							priorPaymentsA+=(decimal)_listClaimProcsForPat[j].InsPayAmt;
						}
						else {
							priorPaymentsB+=(decimal)_listClaimProcsForPat[j].InsPayAmt;
						}
					}
					else {
						for(int k=0;k<_listPatPlans.Count;k++) {
							if(_listPatPlans[k].InsSubNum==_listClaimProcsForPat[j].InsSubNum) {
								if(_listPatPlans[k].Ordinal==1) {
									priorPaymentsA+=(decimal)_listClaimProcsForPat[j].InsPayAmt;
								}
								else if(_listPatPlans[k].Ordinal==2) {
									priorPaymentsB+=(decimal)_listClaimProcsForPat[j].InsPayAmt;
								}
								break;
							}
						}
					}
				}
			}
			for(int i=0;i<ClaimFormCur.Items.Count;i++) {
				string stringFormat=ClaimFormCur.Items[i].FormatString;
				if(ClaimFormCur.Items[i].FieldName=="MedInsCrossoverIndicator") {
					if(strInsFilingCode=="MB") { //Medicare Part B
						_stringArrayDisplay[i]="XOVR";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Name") { //MedInsAName, MedInsBName, MedInsCName
					_stringArrayDisplay[i]=Carriers.GetName(_insPlan.CarrierNum);
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"PlanID") { //MedInsAPlanID, MedInsBPlanID, MedInsCPlanID
					//Not used. Leave blank.
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"RelInfo") { //MedInsARelInfo, MedInsBRelInfo, MedInsCRelInfo
					_stringArrayDisplay[i]=_insSub.ReleaseInfo?"X":"";
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AssignBen") { //MedInsAAssignBen, MedInsBAssignBen, MedInsCAssignBen
					_stringArrayDisplay[i]=Claims.GetAssignmentOfBenefits(_claim,_insSub)?"X":"";
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedInsAPriorPmt") {
					if(priorPaymentsA==0) {
						continue;
					}
					if(stringFormat=="") {
						_stringArrayDisplay[i]=priorPaymentsA.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						_stringArrayDisplay[i]=priorPaymentsA.ToString("F").Replace("."," ");
					}
					else {
						_stringArrayDisplay[i]=priorPaymentsA.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedInsBPriorPmt") {
					if(priorPaymentsB==0) {
						continue;
					}
					if(stringFormat=="") {
						_stringArrayDisplay[i]=priorPaymentsB.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						_stringArrayDisplay[i]=priorPaymentsB.ToString("F").Replace("."," ");
					}
					else {
						_stringArrayDisplay[i]=priorPaymentsB.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AmtDue") { //MedInsAAmtDue, MedInsBAmtDue, MedInsCAmtDue
					decimal totalValAmt=(decimal)ClaimValCodeLogs.GetValAmountTotal(_claim.ClaimNum,"23");
					decimal amtDue=((decimal)_claim.ClaimFee-priorPaymentsA-priorPaymentsB-totalValAmt);
					if(stringFormat=="") {
						_stringArrayDisplay[i]=amtDue.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						_stringArrayDisplay[i]=amtDue.ToString("F").Replace("."," ");
					}
					else {
						_stringArrayDisplay[i]=amtDue.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"OtherProvID") { //MedInsAOtherProvID, MedInsBOtherProvID, MedInsCOtherProvID
					string carrierElectID=_carrier.ElectID.ToString();
					Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
					Provider provider=Providers.GetFirstOrDefault(x => x.ProvNum==_claim.ProvBill)??providerFirst;
					if(provider.ProvNum>0 && carrierElectID!="" && ProviderIdents.GetForPayor(provider.ProvNum,carrierElectID).Length>0) {
						ProviderIdent providerIdent=ProviderIdents.GetForPayor(provider.ProvNum,carrierElectID)[0];
						if(providerIdent.IDNumber != "") {
							_stringArrayDisplay[i]=providerIdent.IDNumber.ToString();
						}
						else {
							_stringArrayDisplay[i] = "";
						}
					}
					else {
						_stringArrayDisplay[i]="";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"InsuredName") { //MedInsAInsuredName, MedInsBInsuredName, MedInsCInsuredName
					_stringArrayDisplay[i]=Patients.GetPat(_insSub.Subscriber).GetNameFLFormal();
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Relation") { //MedInsARelation, MedInsBRelation, MedInsCRelation
					if(_claim.PatRelat==Relat.Spouse) {
						_stringArrayDisplay[i]="01";
					}
					else if(_claim.PatRelat==Relat.Self) {
						_stringArrayDisplay[i]="18";
					}
					else if(_claim.PatRelat==Relat.Child) {
						_stringArrayDisplay[i]="19";
					}
					else if(_claim.PatRelat==Relat.Employee) {
						_stringArrayDisplay[i]="20";
					}
					else if(_claim.PatRelat==Relat.LifePartner) {
						_stringArrayDisplay[i]="53";
					}
					else {
						_stringArrayDisplay[i]="G8";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"InsuredID") { //MedInsAInsuredID, MedInsBInsuredID, MedInsCInsuredID
					_stringArrayDisplay[i]=_insSub.SubscriberID;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"GroupName") { //MedInsAGroupName, MedInsBGroupName, MedInsCGroupName
					_stringArrayDisplay[i]=_insPlan.GroupName;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"GroupNum") { //MedInsAGroupNum, MedInsBGroupNum, MedInsCGroupNum
					_stringArrayDisplay[i]=_insPlan.GroupNum;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AuthCode") { //MedInsAAuthCode, MedInsBAuthCode, MedInsCAuthCode
					_stringArrayDisplay[i]=_claim.PreAuthString;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Employer") { //MedInsAEmployer, MedInsBEmployer, MedInsCEmployer
					_stringArrayDisplay[i]=Employers.GetName(_insPlan.EmployerNum);
				}
			}
		}

		/// <summary>Uses the fee field to determine how many procedures this claim will print.</summary>
		/// <returns></returns>
		private int ProcLimitForFormat(){
			int retVal=0;
			//loop until a match is not found.
			for(int i=0;i<=15;i++){
				for(int j=0;j<ClaimFormCur.Items.Count;j++){
					if(ClaimFormCur.Items[j].FieldName=="P"+i.ToString()+"Fee"){
						retVal=i;
					}
				}//for j
			}
			if(retVal==0){//if claimform doesn't use fees, use procedurecode
				for(int i=0;i<=15;i++){
					for(int j=0;j<ClaimFormCur.Items.Count;j++){
						if(ClaimFormCur.Items[j].FieldName=="P"+i.ToString()+"Code"){
							retVal=i;
						}
					}//for j
				}
			}
			if(retVal==0){//if STILL zero
				retVal=10;
			}
			return retVal;
		}

		/// <summary>Overload that does not need a stringFormat</summary>
		/// <param name="field"></param>
		/// <param name="procIndex"></param>
		/// <returns></returns>
		private string GetProcInfo(string field,int procIndex) {
			return GetProcInfo(field,procIndex,"");
		}

		/// <summary>Gets the string to be used for this field and index.</summary>
		/// <param name="field"></param>
		/// <param name="procIndex"></param>
		/// <param name="stringFormat"></param>
		/// <returns></returns>
		private string GetProcInfo(string field,int procIndex, string stringFormat) {
			//remember that procIndex is 1 based, not 0 based, 
			procIndex--;//so convert to 0 based
			if(_listClaimProcs.Count <= procIndex){
				return "";
			}
			Procedure procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcs[procIndex].ProcNum);
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
			switch(field) {
				case "System":
					return "JP";
				case "Code":
					return _listClaimProcs[procIndex].CodeSent;
				case "Fee":
					decimal totalProcFees=(decimal)_listClaimProcs[procIndex].FeeBilled;
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						List<Procedure> listProceduresLab=Procedures.GetCanadianLabFees(_listClaimProcs[procIndex].ProcNum,_listProcedures);
						for(int i=0;i<listProceduresLab.Count;i++) {
							totalProcFees+=(decimal)listProceduresLab[i].ProcFee;
						}
					}
					if(stringFormat=="") {
						return totalProcFees.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						return totalProcFees.ToString("F").Replace("."," ");
					}
					return totalProcFees.ToString(stringFormat);
				case "RevCode":
					return procedure.RevCode;
				case "CodeMod1":
					return procedure.CodeMod1;
				case "CodeMod2":
					return procedure.CodeMod2;
				case "CodeMod3":
					return procedure.CodeMod3;
				case "CodeMod4":
					return procedure.CodeMod4;
				case "Minutes":
					if(procedure.ProcTime==TimeSpan.Zero || procedure.ProcTimeEnd==TimeSpan.Zero) {
						return "";
					}
					return POut.Int((int)(procedure.ProcTimeEnd-procedure.ProcTime).TotalMinutes);
				case "UnitQty":
					return procedure.UnitQty.ToString();
				case "BaseUnits":
					return procedure.BaseUnits.ToString();
				case "Desc":
					if(procedureCode.DrugNDC!="") {
						//For UB04, we must show the procedure description as a standard drug format so that the drug can be easily recognized.
						//The DrugNDC field is only used when medical features are turned on so this behavior won't take effect in many circumstances.
						string strDrugUnit="UN";//Unit
						float drugQty=procedure.DrugQty;
						switch(procedure.DrugUnit) {
							case EnumProcDrugUnit.Gram:
								strDrugUnit="GR";
								break;
							case EnumProcDrugUnit.InternationalUnit:
								strDrugUnit="F2";
								break;
							case EnumProcDrugUnit.Milligram:
								strDrugUnit="GR";
								drugQty=drugQty/1000;
								break;
							case EnumProcDrugUnit.Milliliter:
								strDrugUnit="ML";
								break;
						}
						return "N4"+procedureCode.DrugNDC+strDrugUnit+drugQty.ToString("f3");
					}
					else {
						ProcedureCode procedureCodeSent=ProcedureCodes.GetProcCode(_listClaimProcs[procIndex].CodeSent);
						string descript=Procedures.GetClaimDescript(_listClaimProcs[procIndex],procedureCodeSent,procedure,procedureCode,_insPlan);
						if(procedureCodeSent.TreatArea==TreatmentArea.Quad) {
							return procedure.Surf+" "+descript;
						}
						else {
							return descript;
						}
					}
				case "Date":
					if(_claim.ClaimType=="PreAuth") {//no date on preauth procedures
						return "";
					}
					if(stringFormat=="") {
						return _listClaimProcs[procIndex].ProcDate.ToShortDateString();
					}
					return _listClaimProcs[procIndex].ProcDate.ToString(stringFormat);
				case "TreatDentMedicaidID":
					if(_listClaimProcs[procIndex].ProvNum==0) {
						return "";
					}
					else {
						Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
						Provider providerClaimProc=Providers.GetFirstOrDefault(x => x.ProvNum==_listClaimProcs[procIndex].ProvNum)??providerFirst;
						return providerClaimProc.MedicaidID;
					}
				case "TreatProvNPI":
					if(_listClaimProcs[procIndex].ProvNum==0) {
						return "";
					}
					else {
						Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
						Provider providerClaimProc=Providers.GetFirstOrDefault(x => x.ProvNum==_listClaimProcs[procIndex].ProvNum)??providerFirst;
						return providerClaimProc.NationalProvID;
					}
				case "PlaceNumericCode":
					return X12object.GetPlaceService(_claim.PlaceService);
				case "Diagnosis":
					if(procedure.DiagnosticCode==""){
						return "";
					}
					for(int d=0;d<_stringArrayDiagnoses.Length;d++){
						if(_stringArrayDiagnoses[d]==procedure.DiagnosticCode){
							return (d+1).ToString();
						}
					}
					return procedure.DiagnosticCode;
				case "DiagnosisPoint":
					if(procedure.DiagnosticCode=="") {
						return "";//No diagnosis codes present on procedure.
					}
					string strPointer="";//Output will be in alphabetical order.
					for(int d=0;d<_stringArrayAllDiagnoses.Length;d++) {
						if(_stringArrayAllDiagnoses[d]=="") {
							continue;
						}
						if(_stringArrayAllDiagnoses[d]==procedure.DiagnosticCode || _stringArrayAllDiagnoses[d]==procedure.DiagnosticCode2 ||
							_stringArrayAllDiagnoses[d]==procedure.DiagnosticCode3 || _stringArrayAllDiagnoses[d]==procedure.DiagnosticCode4) 
						{
							strPointer+=(Char)(d+(int)('A'));//Characters A through L.
						}
					}
					return strPointer;
				case "Lab":
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						List<Procedure> listProceduresLab=Procedures.GetCanadianLabFees(_listClaimProcs[procIndex].ProcNum,_listProcedures);
						decimal totalLabFees=0;
						for(int i=0;i<listProceduresLab.Count;i++) {
							totalLabFees+=(decimal)listProceduresLab[i].ProcFee;
						}
						if(stringFormat=="") {
							return totalLabFees.ToString("F");
						}
						else if(stringFormat=="NoDec") {
							return totalLabFees.ToString("F").Replace("."," ");
						}
						return totalLabFees.ToString(stringFormat);
					}
					return "";
				case "FeeMinusLab":
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(stringFormat=="") {
							return _listClaimProcs[procIndex].FeeBilled.ToString("F");
						}
						else if(stringFormat=="NoDec") {
							double amt = _listClaimProcs[procIndex].FeeBilled * 100;
							return amt.ToString();
						}
						else {
							return _listClaimProcs[procIndex].FeeBilled.ToString(stringFormat);
						}
					}
					return "";//(((ClaimProc)claimprocs[procIndex]).FeeBilled-ProcCur.LabFee).ToString("n");
				case "IsEmergency":
					if(procedure.Urgency==ProcUrgency.Emergency) {
						return "Y";
					}
					return "";
				case "eClaimNote":
					return procedure.ClaimNote;
				case "DrugNDC":
					if(procedureCode.DrugNDC!="") {
						//For UB04, we must show the procedure description as a standard drug format so that the drug can be easily recognized.
						//The DrugNDC field is only used when medical features are turned on so this behavior won't take effect in many circumstances.
						string strDrugUnit="UN";//Unit
						float drugQty=procedure.DrugQty;
						switch(procedure.DrugUnit) {
							case EnumProcDrugUnit.Gram:
								strDrugUnit="GR";
								break;
							case EnumProcDrugUnit.InternationalUnit:
								strDrugUnit="F2";
								break;
							case EnumProcDrugUnit.Milligram:
								strDrugUnit="GR";
								drugQty=drugQty/1000;
								break;
							case EnumProcDrugUnit.Milliliter:
								strDrugUnit="ML";
								break;
						}
						return "N4"+procedureCode.DrugNDC+" "+strDrugUnit+drugQty.ToString("f3");
					}
					return "";
				default:
					break;
			}
			string area="";
			string toothNum="";
			string surf="";
			TreatmentArea treatmentArea=ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea;
			if(treatmentArea==TreatmentArea.Surf){
				//area blank
				toothNum=Tooth.Display(procedure.ToothNum);
				surf=Tooth.SurfTidyForClaims(procedure.Surf,procedure.ToothNum);
			}
			if(treatmentArea==TreatmentArea.Tooth){
				//area blank
				toothNum=Tooth.Display(procedure.ToothNum);
				//surf blank
			}
			if(treatmentArea==TreatmentArea.Quad){
				area=AreaToCode(procedure.Surf);//"UL" etc -> 20 etc
				//toothNum blank
				//surf blank
			}
			if(treatmentArea==TreatmentArea.Sextant){
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					//United States Sextant 1 is Canadian sextant 03.
					//United States Sextant 2 is Canadian sextant 04.
					//United States Sextant 3 is Canadian sextant 05.
					//United States Sextant 4 is Canadian sextant 06.
					//United States Sextant 5 is Canadian sextant 07.
					//United States Sextant 6 is Canadian sextant 08.
					//The sextant goes into the "International Tooth Code" column on the claim form, according to the Nova Scotia NIHB fee guide page VII.
					toothNum=(PIn.Int(procedure.Surf)+2).ToString().PadLeft(2,'0');//Add 2 to US sextant, then prepend a '0'.
				}
				else {//United States
					area="";//leave it blank.  Never used anyway.
					//area="S"+ProcCur.Surf;//area
					//toothNum blank
					//surf blank
				}
			}
			if(treatmentArea==TreatmentArea.Arch){
				area=AreaToCode(procedure.Surf);//area "U", etc
				//toothNum blank
				//surf blank
			}
			if(treatmentArea==TreatmentArea.ToothRange || ProcedureCodes.GetProcCode(procedure.CodeNum).AreaAlsoToothRange){
				//area blank. Or, for AreaAlsoToothRange, already handled above for arch or quad
				toothNum=Tooth.DisplayRange(procedure.ToothRange);
				//surf blank
			}
			if(treatmentArea==TreatmentArea.Mouth){
				area="00";
			}
      switch(field){
				case "Area":
					return area;
				case "ToothNum":
					return toothNum;
				case "Surface":
					return surf;
				case "ToothNumOrArea":
					if(toothNum!=""){
						return toothNum;
					}
					else{
						return area;
					}
			}
			MessageBox.Show("error in getprocinfo");
			return "";//should never get to here
		}

		private string AreaToCode(string area){
			switch(area){
				case "U":
					return "01";
				case "L":
					return "02";
				case "UR":
					return "10";
				case "UL":
					return "20";
				case "LL":
					return "30";
				case "LR":
					return "40";
			}
			return "";
		}

		//Todo: Find a better place to put this method.
		///<summary>Prints the claim to a Canadian Dental Association (CDA) claim form.  Tries to print to the printer chosen by the user in File | Printers | Claim.</summary>
		public static void PrintCdaClaimForm(Claim claim) {
			try {
				FormClaimPrint formClaimPrint=new FormClaimPrint();
				formClaimPrint.PatNum=claim.PatNum;
				formClaimPrint.ClaimNum=claim.ClaimNum;
				formClaimPrint.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
				if(formClaimPrint.PrintImmediate(Lan.g(nameof(PrinterL),"CDA claim form printed"),PrintSituation.Claim,claim.PatNum)) {
					Etranss.SetClaimSentOrPrinted(claim.ClaimNum,claim.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				}
			}
			catch {
				//Oh well, the user can manually reprint if needed.
			}
		}

		//Todo: Find a better place to put this method.
		public static void ShowCdaClaimForm(Claim claim) {
			using FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.PatNum=claim.PatNum;
			formClaimPrint.ClaimNum=claim.ClaimNum;
			formClaimPrint.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
			formClaimPrint.ShowDialog();
		}

		///<summary>This method returns -1 if a toothrange was not specified, otherwise returns the count of teeth in the toothrange.</summary>
		private int GetToothRangeCount(string toothNums) {
			List<string> listStringsToothNums=RemoveToothRangeFormat(toothNums);
			if(listStringsToothNums.Count<=1) {//Proc had 0 or 1 toothNum specified
				return -1;
			}
			else {//The procedure has a toothrange, return the number of teeth.
				return listStringsToothNums.Count;
			}
		}

		///<summary>Returns the procedures UnitQty fields.</summary>
		private string CalculateUnitQtyField(int index,int startProc,InsPlan insPlan) {
			int qty;
			if(insPlan.ShowBaseUnits) {
				short bUnit;
				System.Int16.TryParse(GetProcInfo("BaseUnits",index+startProc),out bUnit);
				short uQty;
				System.Int16.TryParse(GetProcInfo("UnitQty",index+startProc),out uQty);
				qty=bUnit+uQty;
			}
			else if(GetProcInfo("UnitQty",index+startProc)!="") {
				qty=Int16.Parse(GetProcInfo("UnitQty",index+startProc));
			}
			else {
				qty=0;
			}
			if(qty==0) {
				return "";
			}
			return qty.ToString();
		}

		///<summary>Constructs the SystemAndTeeth field as specified by the 1500 claim form manual.
		///Returns an empty string if the procedure did not specify a toothNum or tooth range.</summary>
		private string GenerateSystemAndTeethField(int index,int startProc) {
			string strToothNum=GetProcInfo("ToothNum",index+startProc);
			if(String.IsNullOrEmpty(strToothNum)) {
				string area=GetProcInfo("Area",index+startProc);
				if(string.IsNullOrEmpty(area)) {
					return "";//Procedure did not specify a toothnum or area
				}
				//JO="ANSI/ADA/ISO Specification No. 3950-1984 Dentistry Designation System for Tooth and Areas of the Oral Cavity" (v. 02-12 1500 claim form)
				return "JO"+area;
			}
			List<string> listStringsToothNums=RemoveToothRangeFormat(strToothNum);
			//loop through the list and construct the field
			return "JP"+String.Join(" ",listStringsToothNums);//JP="Universal/National Tooth Designation System" (v. 02-12 1500 claim form)
		}

		private string GenerateSupplementalInfoField(int index,int startProc) {
			List<string> listSuppInfo=new List<string>();
			string systemAndTeeth=GenerateSystemAndTeethField(index,startProc);
			if(systemAndTeeth=="") { 
				string strDrugNDC=GetProcInfo("DrugNDC",index+startProc);
				if(!strDrugNDC.IsNullOrEmpty()) {
					listSuppInfo.Add(strDrugNDC);
				}
			}
			else {
				listSuppInfo.Add(systemAndTeeth);
			}
			string eClaimNote=GetProcInfo("eClaimNote",index+startProc);
			if(!eClaimNote.IsNullOrEmpty()) {
				listSuppInfo.Add("ZZ "+eClaimNote);
			}
			return StringTools.Truncate(String.Join("   ",listSuppInfo),61);
		}

		///<summary>In GetProcInfo() if the treament area is a ToothRange, the resulting string is formatted with a '-'.
		///E.g.- Input:"1-7", returns "1,2,3,4,5,6,7" in a list.
		///This format will not work with the 1500 claim form per the instruction manual.
		///"Do not enter a space between the qualifier and the number/code/information. Do not enter hyphens or spaces within the number/code".
		///Obligatory "Beware ye who enter here".</summary>
		private List<string> RemoveToothRangeFormat(string toothRange) {
			//There may be multiple toothranges formatted with a '-'. E.g. 1-5,20-25
			//Each must be broken apart and added to the return value
			string[] stringArrayToothRanges=toothRange.Split(',');
			//List of toothNums
			List<string> listStringsRetVals=new List<string>();
			foreach(string toothRangeVal in stringArrayToothRanges) {
				if(toothRangeVal.Contains("-")) {
					string[] arrayRange=toothRangeVal.Split('-');
					int start=PIn.Int(arrayRange[0]);
					int end=PIn.Int(arrayRange[1]);
					//Create a list of ints given the starting number and total count of ints needed. Then comma delimit the list and add it to the return value.
					listStringsRetVals.AddRange(Enumerable.Range(start,(end-start)+1).Select(x => x.ToString()));
				}
				else if(toothRangeVal.Trim()!="") {
					listStringsRetVals.Add(toothRangeVal);
				}
			}
			return listStringsRetVals;
		}

		private void butBack_Click(object sender, System.EventArgs e){
			if(Preview2.StartPage==0) { 
				return; 
			}
			Preview2.StartPage--;
			labelTotPages.Text=(Preview2.StartPage+1).ToString() + " / " + _totalPages.ToString();
		}

		private void butFwd_Click(object sender, System.EventArgs e){
			if(Preview2.StartPage==_totalPages-1) {
				return;
			}
			Preview2.StartPage++;
			labelTotPages.Text=(Preview2.StartPage+1).ToString() + " / " + _totalPages.ToString();
		}

		private void butPrint_Click(object sender, System.EventArgs e){
			if(PrintClaim()) {
				Etranss.SetClaimSentOrPrinted(ClaimNum,_claim.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				WasSent=true;
				//Claims.UpdateStatus(ThisClaimNum,"P");
				SecurityLogs.MakeLogEntry(EnumPermType.ClaimSend,_claim.PatNum,Lan.g(this,"Claim printed from Claim Preview window."),
					_claim.ClaimNum,_claim.SecDateTEdit);
				DialogResult=DialogResult.OK;
			}
			else{
				DialogResult=DialogResult.Cancel;
			}
			//Close();
		}

		private void FormClaimPrint_FormClosing(object sender,FormClosingEventArgs e) {
			if(!butPrint.Enabled) {//if the claim has been deleted and the print button has been disabled.
				DialogResult=DialogResult.Abort;
			}
		}

		private void FormClaimPrint_CloseXClicked(object sender,CancelEventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}