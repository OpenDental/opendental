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
		public long ClaimNumCur;
		///<summary></summary>
		public long PatNumCur;
		//<summary>This will be 0 unless the user is trying to print a batch e-claim with a predefined ClaimForm.</summary>
		//public int ClaimFormNum;
		///<summary></summary>
		public bool PrintBlank;
		///<summary></summary>
		public bool PrintImmediately;
		private string[] displayStrings;
		///<summary>The claimprocs for this claim, not including payments and duplicates.</summary>
		private List<ClaimProc> ListClaimProcs;
		private int pagesPrinted;
		private int totalPages;
		//<summary>Set to true if using this class just to generate strings for the Renaissance link.</summary>
		//private bool IsRenaissance;
		private List<ClaimProc> ClaimProcsForClaim;
		private List<ClaimProc> ClaimProcsForPat;
		///<summary>All procedures for the patient.</summary>
		private List<Procedure> ListProc;
		///<summary>This is set externally for Renaissance and generic e-claims.  If it was not set ahead of time, it will set in FillDisplayStrings according to the insPlan.</summary>
		public ClaimForm ClaimFormCur;
		private List <InsPlan> ListInsPlan;
		//private InsPlan[] MedPlanList;
		private List<PatPlan> ListPatPlans;
		private Claim ClaimCur;
		///<summary>Filled with the first four diagnoses codes in the list of procedures associated with the claim. Always length of 4.</summary>
		private string[] diagnoses;
		///<summary>Complete list of all diagnoses.  Maximum unique length of array will be 12 due to the requirements of the medical 1500 (02-12) claim form.</summary>
		private string[] arrayDiagnoses;
		//private Claim[] ClaimsArray;
		//private Claim[] MedClaimsArray;
		private List<ClaimValCodeLog> ListClaimValCodeLog;
		private Referral ClaimReferral;
		private List<InsSub> ListInsSub2;
		private InsSub subCur;
		private InsPlan planCur;
		private Carrier carrier;

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
		public bool PrintImmediate(string auditDescription,PrintSituation printSit,long auditPatNum,bool doUseLastPrinterSettingsIfAvailable=false) {
			if(PrintBlank) {
				//Get the default claim form for the practice.
				ClaimFormCur=ClaimForms.GetClaimForm(PrefC.GetLong(PrefName.DefaultClaimForm));
				if(ClaimFormCur==null) {//Could happen if printing a blank form and no default claim form is set.
					MsgBox.Show(this,"No default claim form set.  Set a default in Setup | Family / Insurance | Claim Forms.");
					return false;
				}
			}
			PrinterSettings pSet=(PrinterSettings)ODprintout.CurPrintout?.PrintDoc?.PrinterSettings?.Clone();
			PageSettings pgSet=(PageSettings)ODprintout.CurPrintout?.PrintDoc?.DefaultPageSettings?.Clone();
			bool? originAtMargins=ODprintout.CurPrintout?.PrintDoc?.OriginAtMargins;
			CreateODprintout(auditDescription,printSit,auditPatNum);
			if(ODprintout.CurPrintout.SettingsErrorCode==PrintoutErrorCode.Success) {
				if(doUseLastPrinterSettingsIfAvailable && pSet!=null && pgSet!=null) {
					ODprintout.CurPrintout.PrintDoc.PrinterSettings=pSet;
					ODprintout.CurPrintout.PrintDoc.DefaultPageSettings=pgSet;
					ODprintout.CurPrintout.PrintDoc.OriginAtMargins=originAtMargins??ODprintout.CurPrintout.PrintDoc.OriginAtMargins;
					return ODprintout.CurPrintout.TryPrint();
				}
				return PrinterL.TryPrint(ODprintout.CurPrintout);
			}
			return false;
		}

		///<summary>Constructs a new ODprintout and sets it to ODprintout.CurPrintout.</summary>
		private void CreateODprintout(string auditDescription,PrintSituation printSit,long auditPatNum) {
			//TODO: Implement ODprintout pattern - print or debug preview control
			pagesPrinted=0;
			ODprintout.InvalidMinDefaultPageHeight=400;//some printers report page size of 0.
			ODprintout.InvalidMinDefaultPageWidth=0;
			PrinterL.CreateODprintout(pd2_PrintPage,
				auditDescription,
				printSit,
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
			if(ListClaimProcs.Count==0){
			totalPages=1;
			//js Some other programmer added this variable and implemented it.  We couldn't figure out why.  For example, in the line above, why would claimprocs ever be zero?  Due to many style and logic issues, we were forced to remove the function of this variable.  It would have to be written from scratch to implement it in the future.  Claims were never designed for multi-page.	
			//Jason - this code we don't understand worked so I added it back until we get time to rewrite it correctly.
			}
			else{
				totalPages=(int)Math.Ceiling((double)ListClaimProcs.Count/(double)procLimit);
			}
			bool HasMedical = false;
			if(!PrintBlank){
				FillProcStrings(pagesPrinted*procLimit,procLimit);
				for(int i=0;i<ListInsPlan.Count;i++){
					if(ListInsPlan[i].IsMedical){
						HasMedical=true;
					}
				}
			}
			if(HasMedical){
				FillMedInsStrings();
				FillMedValueCodes();
				FillMedCondCodes();
			}
			Graphics grfx=ev.Graphics;
			float xPosText;
			for(int i=0;i<ClaimFormCur.Items.Count;i++){
				if(ClaimFormCur.Items[i].ImageFileName==""){//field
					xPosText=ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX;
					if(ListTools.In(ClaimFormCur.Items[i].FieldName,
							"P1Fee","P2Fee","P3Fee","P4Fee","P5Fee","P6Fee","P7Fee","P8Fee","P9Fee","P10Fee","P11Fee","P12Fee","P13Fee","P14Fee","P15Fee","P1Lab",
							"P2Lab","P3Lab","P4Lab","P5Lab","P6Lab","P7Lab","P8Lab","P9Lab","P10Lab","P1FeeMinusLab","P2FeeMinusLab","P3FeeMinusLab",
							"P4FeeMinusLab","P5FeeMinusLab","P6FeeMinusLab","P7FeeMinusLab","P8FeeMinusLab","P9FeeMinusLab","P10FeeMinusLab","TotalFee",
							"MedInsAAmtDue","MedInsBAmtDue","MedInsCAmtDue","MedInsAPriorPmt","MedInsBPriorPmt","MedInsCPriorPmt","MedValAmount39a",
							"MedValAmount39b","MedValAmount39c","MedValAmount39d","MedValAmount40a","MedValAmount40b","MedValAmount40c","MedValAmount40d",
							"MedValAmount41a","MedValAmount41b","MedValAmount41c","MedValAmount41d","ICDindicatorAB","OutsideLabFee"))
					{
						//this aligns it to the right
						xPosText-=grfx.MeasureString(displayStrings[i],new Font(ClaimFormCur.FontName,ClaimFormCur.FontSize)).Width;
					}
					grfx.DrawString(displayStrings[i],
						new Font(ClaimFormCur.FontName,ClaimFormCur.FontSize),
						new SolidBrush(Color.Black),
						new RectangleF(xPosText,ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,ClaimFormCur.Items[i].Width,ClaimFormCur.Items[i].Height));
				}
				else{//image
					if(!ClaimFormCur.PrintImages){
						continue;
					}
					Image thisImage;
					string extension;
					switch(ClaimFormCur.Items[i].ImageFileName) {
						case "ADA2006.gif":
							thisImage=CDT.Class1.GetADA2006();
							extension=".gif";
							break;
						case "ADA2012.gif":
							thisImage=CDT.Class1.GetADA2012();
							extension=".gif";
							break;
						case "ADA2012_J430D.gif":
							thisImage=CDT.Class1.GetADA2012_J430D();
							extension=".gif";
							break;
						case "ADA2018_J432.gif":
							thisImage=CDT.Class1.GetADA2018_J432();
							extension=".gif";
							break;
						case "ADA2019_J430.gif":
							thisImage=CDT.Class1.GetADA2019_J430();
							extension=".gif";
							break;
						case "1500_02_12.gif":
							thisImage=Properties.Resources._1500_02_12;
							extension=".gif";
							break;
						case "DC-217.gif":
							thisImage=CDT.Class1.GetDC217();
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
							thisImage=FileAtoZ.GetImage(fileName);
							if(thisImage==null) {
								continue;
							}
							extension=Path.GetExtension(ClaimFormCur.Items[i].ImageFileName);
							break;
					}
					if(extension==".jpg"){
						grfx.DrawImage(thisImage,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							(int)(thisImage.Width/thisImage.HorizontalResolution*100),
							(int)(thisImage.Height/thisImage.VerticalResolution*100));
					}
					else if(extension==".gif"){
						grfx.DrawImage(thisImage,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							ClaimFormCur.Items[i].Width,
							ClaimFormCur.Items[i].Height);
					}
					else if(extension==".emf"){
						grfx.DrawImage(thisImage,
							ClaimFormCur.Items[i].XPos+ClaimFormCur.OffsetX,
							ClaimFormCur.Items[i].YPos+ClaimFormCur.OffsetY,
							thisImage.Width,thisImage.Height);
					}
				}
			}
			pagesPrinted++;
			if(totalPages==pagesPrinted){
				ev.HasMorePages=false;
				labelTotPages.Text="1 / "+totalPages.ToString();
			}
			else{
				//MessageBox.Show(pagesPrinted.ToString()+","+totalPages.ToString());
				ev.HasMorePages=true;
			}
		}

		///<summary>Only used when the print button is clicked from within this form during print preview.</summary>
		public bool PrintClaim(){
			pagesPrinted=0;
			return PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Claim from")+" "+ClaimCur.DateService.ToShortDateString()+" "+Lan.g(this,"printed"),
				PatNumCur,
				PrintSituation.Claim,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin
			);
		}

		///<summary>Called from Bridges.Renaissance, this takes the supplied ClaimFormItems.ListForForm, and generates an array of strings that will get saved into a text file.  First dimension of array is the pages. Second dimension is the lines in the page.</summary>
		public static string[][] FillRenaissance(long claimNum,long patNum,List<ClaimFormItem> listItems) {
			FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.ClaimNumCur=claimNum;
			FormCP.PatNumCur=patNum;
			FormCP.ClaimFormCur=new ClaimForm();
			FormCP.ClaimFormCur.Items=listItems;
			return FormCP.FillRenaissance();
		}

		private string[][] FillRenaissance() { 
			//IsRenaissance=true;
			int procLimit=10;
			TryFillDisplayStrings(true);//claimprocs is filled in FillDisplayStrings
														//, so this is just a little extra work
			totalPages=(int)Math.Ceiling((double)ListClaimProcs.Count/(double)procLimit);
			string[][] retVal=new string[totalPages][];
			for(int i=0;i<totalPages;i++){
				pagesPrinted=i;
				//not sure if I also need to do FillDisplayStrings here
				FillProcStrings(pagesPrinted*procLimit,procLimit);
				retVal[i]=(string[])displayStrings.Clone();
			}
			return retVal;
		}

		///<summary>Gets all necessary info from db based on ThisPatNum and ThisClaimNum.  Then fills displayStrings with the actual text that will 
		///display on claim.  The isRenaissance flag is very temporary. Returns true if able to successfully fill the display strings.</summary>
		private bool TryFillDisplayStrings(bool isRenaissance){
			if(PrintBlank){
				displayStrings=new string[ClaimFormCur.Items.Count];
				ListClaimProcs=new List<ClaimProc>();
				return true;
			}
			Family FamCur=Patients.GetFamily(PatNumCur);
			Patient PatCur=FamCur.GetPatient(PatNumCur);
			if(PatCur==null) {
				MsgBox.Show(this,"Unable to find patient.");
				butPrint.Enabled=false;
				return false;
			}
			List<Claim> ClaimList=Claims.Refresh(PatCur.PatNum);
			ClaimCur=Claims.GetFromList(ClaimList,ClaimNumCur);
			if(ClaimCur==null) {
				MsgBox.Show(this,"Claim has been deleted by another user.");
				butPrint.Enabled=false;
				return false;
			}
				//((Claim)Claims.HList[ThisClaimNum]).Clone();
			ListInsSub2=InsSubs.RefreshForFam(FamCur);
			ListInsPlan=InsPlans.RefreshForSubList(ListInsSub2);
			ListPatPlans=PatPlans.Refresh(ClaimCur.PatNum);
			InsPlan otherPlan=InsPlans.GetPlan(ClaimCur.PlanNum2,ListInsPlan);
			InsSub otherSub=InsSubs.GetSub(ClaimCur.InsSubNum2,ListInsSub2);
			if(otherPlan==null){
				otherPlan=new InsPlan();//easier than leaving it null
			}
			Carrier otherCarrier=new Carrier();
			if(otherPlan.PlanNum!=0){
				otherCarrier=Carriers.GetCarrier(otherPlan.CarrierNum);
			}
			//Employers.GetEmployer(otherPlan.EmployerNum);
			//Employer otherEmployer=Employers.Cur;//not actually used
			//then get the main plan
			subCur=InsSubs.GetSub(ClaimCur.InsSubNum,ListInsSub2);
			planCur=InsPlans.GetPlan(ClaimCur.PlanNum,ListInsPlan);
			Clinic clinic=PrefC.HasClinicsEnabled ? Clinics.GetClinic(ClaimCur.ClinicNum) : null;//null if clinics are not enabled.
			carrier=Carriers.GetCarrier(planCur.CarrierNum);
			//Employers.GetEmployer(InsPlans.Cur.EmployerNum);
			Patient subsc;
			if(FamCur.GetIndex(subCur.Subscriber)==-1) {//from another family
				subsc=Patients.GetPat(subCur.Subscriber);
				//Patients.Cur;
				//Patients.GetFamily(ThisPatNum);//return to current family
			}
			else{
				subsc=FamCur.ListPats[FamCur.GetIndex(subCur.Subscriber)];
			}
			if(subsc==null) {//Patient for this InsSub could not be found.  Likely db corruption.
				MsgBox.Show(this,"Insurance Plan attached to Claim does not have a valid Subscriber.  Run Database Maintenance (Tools): InsSubInvalidSubscriber.");
				butPrint.Enabled=false;
				return false;
			}
			Patient otherSubsc=new Patient();
			if(otherPlan.PlanNum!=0){//if secondary insurance exists
				if(FamCur.GetIndex(otherSub.Subscriber)==-1) {//from another family
					otherSubsc=Patients.GetPat(otherSub.Subscriber);
					//Patients.Cur;
					//Patients.GetFamily(ThisPatNum);//return to current family
				}
				else{
					otherSubsc=FamCur.ListPats[FamCur.GetIndex(otherSub.Subscriber)];
				}
			}
			if(ClaimCur.ReferringProv>0) {
				Referrals.TryGetReferral(ClaimCur.ReferringProv,out ClaimReferral);
			}
			ListProc=Procedures.Refresh(PatCur.PatNum);
			List<ToothInitial> initialList=ToothInitials.Refresh(PatCur.PatNum);
			//List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			ClaimProcsForPat=ClaimProcs.Refresh(ClaimCur.PatNum);
			ClaimProcsForClaim=ClaimProcs.RefreshForClaim(ClaimCur.ClaimNum);
			//Customers were sometimes getting supplemental payments to show up before a received claim proc. This would cause the FeeBilled to show as $0.
			ClaimProcsForClaim=ClaimProcsForClaim.OrderBy(x => x.LineNumber)
				.ThenBy(x => x.Status==ClaimProcStatus.Supplemental).ToList();//Put Supplementals on the bottom since they don't have the FeeBilled set.
			ListClaimProcs=new List<ClaimProc>();
			bool includeThis;
			Procedure proc;
			for(int i=0;i<ClaimProcsForClaim.Count;i++){//fill the arraylist
				if(ClaimProcsForClaim[i].ProcNum==0){
					continue;//skip payments
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					proc=Procedures.GetProcFromList(ListProc,ClaimProcsForClaim[i].ProcNum);
					if(proc.ProcNumLab!=0) { //This is a lab fee procedure.
						continue;//skip lab fee procedures in Canada, because they will show up on the same line as the procedure that they are attached to.
					}
				}
				includeThis=true;
				for(int j=0;j<ListClaimProcs.Count;j++){//loop through existing claimprocs
					if(ListClaimProcs[j].ProcNum==ClaimProcsForClaim[i].ProcNum){
						includeThis=false;//skip duplicate procedures
					}
				}
				if(includeThis){
					ListClaimProcs.Add(ClaimProcsForClaim[i]);	
				}
			}
			List<string> missingTeeth=ToothInitials.GetMissingOrHiddenTeeth(initialList);
			ProcedureCode procCode;
			for(int j=missingTeeth.Count-1;j>=0;j--) {//loop backwards to keep index accurate as items are removed
				//if the missing tooth is missing because of an extraction being billed here, then exclude it
				for(int p=0;p<ListClaimProcs.Count;p++) {
					proc=Procedures.GetProcFromList(ListProc,ListClaimProcs[p].ProcNum);
					procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
					if(procCode.PaintType==ToothPaintingType.Extraction && proc.ToothNum==missingTeeth[j]) {
						missingTeeth.RemoveAt(j);
						break;
					}
				}
			}
			//diagnoses---------------------------------------------------------------------------------------
			diagnoses=new string[4];
			for(int i=0;i<4;i++){
				diagnoses[i]="";
			}
			for(int i=0;i<ListClaimProcs.Count;i++){
				proc=Procedures.GetProcFromList(ListProc,ListClaimProcs[i].ProcNum);
				if(proc.DiagnosticCode=="" || proc.IcdVersion==9){
					continue;
				}
				for(int d=0;d<4;d++){
					if(diagnoses[d]==proc.DiagnosticCode){
						break;//if it's already been added
					}
					if(diagnoses[d]==""){//we're at the end of the list of existing diagnoses, and no match
						diagnoses[d]=proc.DiagnosticCode;//so add it.
						break;
					}
				}
				//There's still a chance that the diagnosis didn't get added, if there were more than 4.
			}
			arrayDiagnoses=Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(ListClaimProcs),true).ToArray();
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider providerClaimTreat=Providers.GetFirstOrDefault(x => x.ProvNum==ClaimCur.ProvTreat)??providerFirst;
			ProviderClinic provClinicClaimTreat=ProviderClinics.GetOneOrDefault(providerClaimTreat.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			Provider providerClaimBill=Providers.GetFirstOrDefault(x => x.ProvNum==ClaimCur.ProvBill)??providerFirst;
			ProviderClinic provClinicClaimBill=ProviderClinics.GetOneOrDefault(providerClaimBill.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			if(ClaimFormCur==null){
				if(ClaimCur.ClaimForm>0){
					ClaimFormCur=ClaimForms.GetClaimForm(ClaimCur.ClaimForm);
				} 
				else {
					ClaimFormCur=ClaimForms.GetClaimForm(planCur.ClaimFormNum);
				}
			}
			//If we aren't able to find a claimform, or it has no items there's nothing we can do.
			if(ClaimFormCur==null || ClaimFormCur.Items==null) {
				if(!isRenaissance) {
					MsgBox.Show("Could not retrieve claim form.\r\nClose form and try again.");
				}
				return false;
			}
			List<PatPlan> patPlans=null;
			displayStrings=new string[ClaimFormCur.Items.Count];
			//a value is set for every item, but not every case will have a matching claimform item.
			for(int i=0;i<ClaimFormCur.Items.Count;i++){
				if(ClaimFormCur.Items[i]==null){//Renaissance does not use [0]
					displayStrings[i]="";
					continue;
				}
				switch(ClaimFormCur.Items[i].FieldName){
					default://image. or procedure which gets filled in FillProcStrings.
						displayStrings[i]="";
						break;
					case "FixedText":
						displayStrings[i]=ClaimFormCur.Items[i].FormatString;
						break;
					case "IsPreAuth":
						if(ClaimCur.ClaimType=="PreAuth") {
							displayStrings[i]="X";
						}
						break;
					case "IsStandardClaim":
						if(ClaimCur.ClaimType!="PreAuth") {
							displayStrings[i]="X";
						}
						break;
					case "ShowPreauthorizationIfPreauth":
						if(ClaimCur.ClaimType=="PreAuth") {
							displayStrings[i]="Preauthorization";
						}
						break;
					case "IsMedicaidClaim"://this should later be replaced with an insplan field.
						if(PatCur.MedicaidID!="") {
							displayStrings[i]="X";
						}
						break;
					case "IsGroupHealthPlan":
						string eclaimcode=InsFilingCodes.GetEclaimCode(planCur.FilingCode);
						if(PatCur.MedicaidID=="" 
							&& eclaimcode != "MC"//medicaid
							&& eclaimcode != "CH"//champus
							&& eclaimcode != "VA")//veterans
							//&& eclaimcode != ""//medicare?
						{
							displayStrings[i]="X";
						}
						break;
					case "PreAuthString":
						displayStrings[i]=ClaimCur.PreAuthString;
						break;
					case "PriorAuthString":
						displayStrings[i]=ClaimCur.PriorAuthorizationNumber;
						break;
					#region PriIns
					case "PriInsCarrierName":
						displayStrings[i]=carrier.CarrierName;
						break;
					case "PriInsAddress":
						displayStrings[i]=carrier.Address;
						break;
					case "PriInsAddress2":
						displayStrings[i]=carrier.Address2;
						break;
					case "PriInsAddressComplete":
						displayStrings[i]=carrier.Address+" "+carrier.Address2;
						break;
					case "PriInsCity":
						displayStrings[i]=carrier.City;
						break;
					case "PriInsST":
						displayStrings[i]=carrier.State;
						break;
					case "PriInsZip":
						displayStrings[i]=carrier.Zip;
						break;
					#endregion PriIns
					#region OtherIns
					case "OtherInsExists":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsNotExists":
						if(otherPlan.PlanNum==0) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsExistsDent":
						if(otherPlan.PlanNum!=0) {
							if(!otherPlan.IsMedical) {
								displayStrings[i]="X";
							}
						}
						break;
					case "OtherInsExistsMed":
						if(otherPlan.PlanNum!=0) {
							if(otherPlan.IsMedical) {
								displayStrings[i]="X";
							}
						}
						break;
					case "OtherInsSubscrLastFirst":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherSubsc.LName+", "+otherSubsc.FName+" "+otherSubsc.MiddleI;
						}
						break;
					case "OtherInsSubscrDOB":
						if(otherPlan.PlanNum!=0) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=otherSubsc.Birthdate.ToShortDateString();
							}
							else {
								displayStrings[i]=otherSubsc.Birthdate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "OtherInsSubscrIsMale":
						if(otherPlan.PlanNum!=0 && otherSubsc.Gender==PatientGender.Male) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsSubscrIsFemale":
						if(otherPlan.PlanNum!=0 && otherSubsc.Gender==PatientGender.Female) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsSubscrIsGenderUnknown":
						if(otherPlan.PlanNum!=0 && otherSubsc.Gender==PatientGender.Unknown) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsSubscrGender":
						if(otherPlan.PlanNum!=0) {
							if(otherSubsc.Gender==PatientGender.Male) {
								displayStrings[i]="M";
							}
							else if(otherSubsc.Gender==PatientGender.Female) {
								displayStrings[i]="F";
							}
							else {//Unknown
								displayStrings[i]="U";
							}
						}
						break;
					case "OtherInsSubscrID":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherSub.SubscriberID;
						}
						break;
						//if(otherPlan.PlanNum!=0 && otherSubsc.SSN.Length==9){
						//	displayStrings[i]=otherSubsc.SSN.Substring(0,3)
						//		+"-"+otherSubsc.SSN.Substring(3,2)
						//		+"-"+otherSubsc.SSN.Substring(5);
						//}
						//break;
					case "OtherInsGroupNum":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherPlan.GroupNum;
						}
						break;
					case "OtherInsRelatIsSelf":
						if(otherPlan.PlanNum!=0 && ClaimCur.PatRelat2==Relat.Self) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsRelatIsSpouse":
						if(otherPlan.PlanNum!=0 && ClaimCur.PatRelat2==Relat.Spouse) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsRelatIsChild":
						if(otherPlan.PlanNum!=0 && ClaimCur.PatRelat2==Relat.Child) {
							displayStrings[i]="X";
						}
						break;
					case "OtherInsRelatIsOther":
						if(otherPlan.PlanNum!=0 && (
							ClaimCur.PatRelat2==Relat.Dependent
							|| ClaimCur.PatRelat2==Relat.Employee
							|| ClaimCur.PatRelat2==Relat.HandicapDep
							|| ClaimCur.PatRelat2==Relat.InjuredPlaintiff
							|| ClaimCur.PatRelat2==Relat.LifePartner
							|| ClaimCur.PatRelat2==Relat.SignifOther
							))
							displayStrings[i]="X";
						break;
					case "OtherInsCarrierName":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherCarrier.CarrierName;
						}
						break;
					case "OtherInsAddress":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherCarrier.Address;
						}
						break;
					case "OtherInsCity":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherCarrier.City;
						}
						break;
					case "OtherInsST":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherCarrier.State;
						}
						break;
					case "OtherInsZip":
						if(otherPlan.PlanNum!=0) {
							displayStrings[i]=otherCarrier.Zip;
						}
						break;
					#endregion OtherIns
					#region Subscr
					case "SubscrLastFirst":
						displayStrings[i]=subsc.LName+", "+subsc.FName+" "+subsc.MiddleI;
						break;
					case "SubscrAddress":
						displayStrings[i]=subsc.Address;
						break;
					case "SubscrAddress2":
						displayStrings[i]=subsc.Address2;
						break;
					case "SubscrAddressComplete":
						displayStrings[i]=subsc.Address+" "+subsc.Address2;
						break;
					case "SubscrCity":
						displayStrings[i]=subsc.City;
						break;
					case "SubscrST":
						displayStrings[i]=subsc.State;
						break;
					case "SubscrZip":
						displayStrings[i]=subsc.Zip;
						break;
					case "SubscrPhone"://needs work.  Only used for 1500
						if(isRenaissance) {
							//Expecting (XXX)XXX-XXXX
							displayStrings[i]=subsc.HmPhone;
							if(subsc.HmPhone.Length>14) {//Might have a note following the number.
								displayStrings[i]=subsc.HmPhone.Substring(0,14);
							}
						}
						else {
							string phone=subsc.HmPhone.Replace("(","");
							phone=phone.Replace(")","    ");
							phone=phone.Replace("-","  ");
							displayStrings[i]=phone;
						}
						break;
					case "SubscrDOB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=subsc.Birthdate.ToShortDateString();//MM/dd/yyyy
						}
						else {
							displayStrings[i]=subsc.Birthdate.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "SubscrIsMale":
						if(subsc.Gender==PatientGender.Male) {
							displayStrings[i]="X";
						}
						break;
					case "SubscrIsFemale":
						if(subsc.Gender==PatientGender.Female) {
							displayStrings[i]="X";
						}
						break;
					case "SubscrIsGenderUnknown":
						if(subsc.Gender==PatientGender.Unknown) {
							displayStrings[i]="X";
						}
						break;
					case "SubscrGender":
						if(subsc.Gender==PatientGender.Male) {
							displayStrings[i]="M";
						}
						else if(subsc.Gender==PatientGender.Female) {
							displayStrings[i]="F";
						}
						else {//Unknown
							displayStrings[i]="U";
						}
						break;
					case "SubscrIsMarried":
						if(subsc.Position==PatientPosition.Married) {
							displayStrings[i]="X";
						}
						break;
					case "SubscrIsSingle":
						if(subsc.Position==PatientPosition.Single
							|| subsc.Position==PatientPosition.Child
							|| subsc.Position==PatientPosition.Widowed) {
							displayStrings[i]="X";
						}
						break;
					case "SubscrID":
						patPlans=PatPlans.Refresh(PatNumCur);
						string patID=PatPlans.GetPatID(subCur.InsSubNum,patPlans);
						if(patID=="") {
							displayStrings[i]=subCur.SubscriberID;
						}
						else {
							displayStrings[i]=patID;
						}
						break;
					case "SubscrIDStrict":
						displayStrings[i]=subCur.SubscriberID;
						break;
					case "SubscrIsFTStudent":
						if(subsc.StudentStatus=="F") {
							displayStrings[i]="X";
						}
						break;
					case "SubscrIsPTStudent":
						if(subsc.StudentStatus=="P") {
							displayStrings[i]="X";
						}
						break;
					#endregion Subscr
					case "GroupName":
						displayStrings[i]=planCur.GroupName;
						break;
					case "GroupNum":
						displayStrings[i]=planCur.GroupNum;
						break;
					case "DivisionNo":
						displayStrings[i]=planCur.DivisionNo;
						break;
					case "EmployerName":
						displayStrings[i]=Employers.GetEmployer(planCur.EmployerNum).EmpName;;
						break;
					#region Relat
					case "RelatIsSelf":
						if(ClaimCur.PatRelat==Relat.Self) {
							displayStrings[i]="X";
						}
						break;
					case "RelatIsSpouse":
						if(ClaimCur.PatRelat==Relat.Spouse) {
							displayStrings[i]="X";
						}
						break;
					case "RelatIsChild":
						if(ClaimCur.PatRelat==Relat.Child) {
							displayStrings[i]="X";
						}
						break;
					case "RelatIsOther":
						if(ClaimCur.PatRelat==Relat.Dependent
							|| ClaimCur.PatRelat==Relat.Employee
							|| ClaimCur.PatRelat==Relat.HandicapDep
							|| ClaimCur.PatRelat==Relat.InjuredPlaintiff
							|| ClaimCur.PatRelat==Relat.LifePartner
							|| ClaimCur.PatRelat==Relat.SignifOther) {
							displayStrings[i]="X";
						}
						break;
					case "Relationship":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							if(ClaimCur.PatRelat==Relat.Self) {
								displayStrings[i]="Self";
							}
							else if(ClaimCur.PatRelat==Relat.Spouse) {
								displayStrings[i]="Spouse";
							}
							else if(ClaimCur.PatRelat==Relat.Child) {
								displayStrings[i]="Child";
							}
							else if(ClaimCur.PatRelat==Relat.SignifOther || ClaimCur.PatRelat==Relat.LifePartner) {
								displayStrings[i]="Common Law Spouse";
							}
							else {
								displayStrings[i]="Other";
							}
						}
						else {
							displayStrings[i]=ClaimCur.PatRelat.ToString();
						}
						break;
					#endregion Relat
					case "IsFTStudent":
						if(PatCur.StudentStatus=="F") {
							displayStrings[i]="X";
						}
						break;
					case "IsPTStudent":
						if(PatCur.StudentStatus=="P") {
							displayStrings[i]="X";
						}
						break;
					case "IsStudent":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")//Canadian. en-CA or fr-CA
							&& (PatCur.CanadianEligibilityCode==1 || PatCur.CanadianEligibilityCode==3))//Patient is a student
						{
							displayStrings[i]="X";
						}
						else if(PatCur.StudentStatus=="P" || PatCur.StudentStatus=="F") {
							displayStrings[i]="X";
						}
						break;
					case "CollegeName":
						displayStrings[i]=PatCur.SchoolName;
						break;
					#region Patient
					case "PatientLastFirst":
						displayStrings[i]=PatCur.LName+", "+PatCur.FName+" "+PatCur.MiddleI;
						break;
					case "PatientLastFirstMiCommas"://Medical required format for UB04 printed claims
						displayStrings[i]=PatCur.LName+", "+PatCur.FName+(PatCur.MiddleI==""?"":(", "+PatCur.MiddleI[0]));
						break;
					case "PatientFirstMiddleLast":
						displayStrings[i]=PatCur.FName+" "+PatCur.MiddleI+" "+PatCur.LName;
						break;
					case "PatientFirstName":
						displayStrings[i] = PatCur.FName;
						break;
					case "PatientMiddleName":
						displayStrings[i] = PatCur.MiddleI;
						break;
					case "PatientLastName":
						displayStrings[i] = PatCur.LName;
						break;
					case "PatientAddress":
						displayStrings[i]=PatCur.Address;
						break;
					case "PatientAddress2":
						displayStrings[i]=PatCur.Address2;
						break;
					case "PatientAddressComplete":
						displayStrings[i]=PatCur.Address+" "+PatCur.Address2;
						break;
					case "PatientCity":
						displayStrings[i]=PatCur.City;
						break;
					case "PatientST":
						displayStrings[i]=PatCur.State;
						break;
					case "PatientZip":
						displayStrings[i]=PatCur.Zip;
						break;
					case "PatientPhone"://needs work.  Only used for 1500
						if(isRenaissance) {
							//Expecting (XXX)XXX-XXXX
							displayStrings[i]=PatCur.HmPhone;
							if(PatCur.HmPhone.Length>14) {//Might have a note following the number.
								displayStrings[i]=PatCur.HmPhone.Substring(0,14);
							}
						}
						else {
							string phonep=PatCur.HmPhone.Replace("(","");
							phonep=phonep.Replace(")","    ");
							phonep=phonep.Replace("-","  ");
							displayStrings[i]=phonep;
						}
						break;
					case "PatientDOB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=PatCur.Birthdate.ToShortDateString();//MM/dd/yyyy
						}
						else {
							displayStrings[i]=PatCur.Birthdate.ToString
								(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "PatientIsMale":
						if(PatCur.Gender==PatientGender.Male) {
							displayStrings[i]="X";
						}
						break;
					case "PatientIsFemale":
						if(PatCur.Gender==PatientGender.Female) {
							displayStrings[i]="X";
						}
						break;
					case "PatientIsGenderUnknown":
						if(PatCur.Gender==PatientGender.Unknown) {
							displayStrings[i]="X";
						}
						break;
					case "PatientGender":
						if(PatCur.Gender==PatientGender.Male) {
							displayStrings[i]="Male";
						}
						else if(PatCur.Gender==PatientGender.Female) {
							displayStrings[i]="Female";
						}
						else {//Unknown
							displayStrings[i]="Unknown";
						}
						break;
					case "PatientGenderLetter":
						if(PatCur.Gender==PatientGender.Male) {
							displayStrings[i]="M";
						}
						else if(PatCur.Gender==PatientGender.Female) {
							displayStrings[i]="F";
						}
						else {//Unknown
							displayStrings[i]="U";
						}
						break;
					case "PatientIsMarried":
						if(PatCur.Position==PatientPosition.Married) {
							displayStrings[i]="X";
						}
						break;
					case "PatientIsSingle":
						if(PatCur.Position==PatientPosition.Single
							|| PatCur.Position==PatientPosition.Child
							|| PatCur.Position==PatientPosition.Widowed) {
							displayStrings[i]="X";
						}
						break;
					case "PatIDFromPatPlan": //Dependant Code for Canada
						patPlans=PatPlans.Refresh(PatNumCur);
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && carrier.ElectID=="000064") {//Canadian. en-CA or fr-CA and Pacific Blue Cross
							displayStrings[i]=subCur.SubscriberID+"-"+PatPlans.GetPatID(subCur.InsSubNum,patPlans);
						}
						else {
							displayStrings[i]=PatPlans.GetPatID(subCur.InsSubNum,patPlans);
						}
						break;
					case "PatientSSN":
						if(PatCur.SSN.Length==9) {
							displayStrings[i]=PatCur.SSN.Substring(0,3)
								+"-"+PatCur.SSN.Substring(3,2)
								+"-"+PatCur.SSN.Substring(5);
						}
						else {
							displayStrings[i]=PatCur.SSN;
						}
						break;
					case "PatientMedicaidID":
						displayStrings[i]=PatCur.MedicaidID;
						break;
					case "PatientID-MedicaidOrSSN":
						if(PatCur.MedicaidID!="") {
							displayStrings[i]=PatCur.MedicaidID;
						}
						else {
							displayStrings[i]=PatCur.SSN;
						}
						break;
					case "PatientChartNum":
						displayStrings[i]=PatCur.ChartNumber;
						break;
					case "PatientPatNum":
						displayStrings[i]=PatCur.PatNum.ToString();
						break;
					#endregion Patient
					#region Diagnosis
					case "Diagnosis1":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=diagnoses[0];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=diagnoses[0].Replace(".","");
						}
						break;
					case "Diagnosis2":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=diagnoses[1];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=diagnoses[1].Replace(".","");
						}
						break;
					case "Diagnosis3":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=diagnoses[2];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=diagnoses[2].Replace(".","");
						}
						break;
					case "Diagnosis4":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=diagnoses[3];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=diagnoses[3].Replace(".","");
						}
						break;
					case "DiagnosisA":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[0];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[0].Replace(".","");
						}
						break;
					case "DiagnosisB":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[1];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[1].Replace(".","");
						}
						break;
					case "DiagnosisC":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[2];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[2].Replace(".","");
						}
						break;
					case "DiagnosisD":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[3];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[3].Replace(".","");
						}
						break;
					case "DiagnosisE":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[4];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[4].Replace(".","");
						}
						break;
					case "DiagnosisF":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[5];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[5].Replace(".","");
						}
						break;
					case "DiagnosisG":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[6];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[6].Replace(".","");
						}
						break;
					case "DiagnosisH":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[7];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[7].Replace(".","");
						}
						break;
					case "DiagnosisI":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[8];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[8].Replace(".","");
						}
						break;
					case "DiagnosisJ":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[9];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[9].Replace(".","");
						}
						break;
					case "DiagnosisK":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[10];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[10].Replace(".","");
						}
						break;
					case "DiagnosisL":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=arrayDiagnoses[11];
						}
						else if(ClaimFormCur.Items[i].FormatString=="NoDec") {
							displayStrings[i]=arrayDiagnoses[11].Replace(".","");
						}
						break;
					#endregion Diagnosis
			//this is where the procedures used to be
					#region Miss
					case "Miss1":
						if(missingTeeth.Contains("1")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss2":
						if(missingTeeth.Contains("2")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss3":
						if(missingTeeth.Contains("3")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss4":
						if(missingTeeth.Contains("4")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss5":
						if(missingTeeth.Contains("5")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss6":
						if(missingTeeth.Contains("6")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss7":
						if(missingTeeth.Contains("7")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss8":
						if(missingTeeth.Contains("8")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss9":
						if(missingTeeth.Contains("9")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss10":
						if(missingTeeth.Contains("10")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss11":
						if(missingTeeth.Contains("11")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss12":
						if(missingTeeth.Contains("12")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss13":
						if(missingTeeth.Contains("13")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss14":
						if(missingTeeth.Contains("14")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss15":
						if(missingTeeth.Contains("15")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss16":
						if(missingTeeth.Contains("16")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss17":
						if(missingTeeth.Contains("17")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss18":
						if(missingTeeth.Contains("18")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss19":
						if(missingTeeth.Contains("19")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss20":
						if(missingTeeth.Contains("20")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss21":
						if(missingTeeth.Contains("21")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss22":
						if(missingTeeth.Contains("22")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss23":
						if(missingTeeth.Contains("23")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss24":
						if(missingTeeth.Contains("24")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss25":
						if(missingTeeth.Contains("25")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss26":
						if(missingTeeth.Contains("26")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss27":
						if(missingTeeth.Contains("27")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss28":
						if(missingTeeth.Contains("28")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss29":
						if(missingTeeth.Contains("29")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss30":
						if(missingTeeth.Contains("30")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss31":
						if(missingTeeth.Contains("31")) {
							displayStrings[i]="X";
						}
						break;
					case "Miss32":
						if(missingTeeth.Contains("32")) {
							displayStrings[i]="X";
						}
						break;
					#endregion Miss
					case "Remarks":
						displayStrings[i]="";
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							if(ClaimCur.ClaimType=="PreAuth") {
								displayStrings[i]+="Predetermination only."+Environment.NewLine;
							}
							else {
								if(subCur.AssignBen) {
									displayStrings[i]+="Please pay provider."+Environment.NewLine;
								}
								else {
									displayStrings[i]+="Please pay patient."+Environment.NewLine;
								}
							}
						}
						if(ClaimCur.AttachmentID!="" && !ClaimCur.ClaimNote.StartsWith(ClaimCur.AttachmentID)){
							displayStrings[i]=ClaimCur.AttachmentID+" ";
						}
						displayStrings[i]+=ClaimCur.ClaimNote;
						break;
					case "PatientRelease":
						if(subCur.ReleaseInfo) {
							displayStrings[i]="Signature on File";
						}
						break;
					case "PatientReleaseDate":
						if(subCur.ReleaseInfo && ClaimCur.DateSent.Year > 1860) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.DateSent.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						} 
						break;
					case "PatientAssignment":
						if(subCur.AssignBen) {
							displayStrings[i]="Signature on File";
						}
						break;
					case "PatientAssignmentDate":
						if(subCur.AssignBen && ClaimCur.DateSent.Year > 1860) {
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.DateSent.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					#region Place
					case "PlaceIsOffice":
						if(ClaimCur.PlaceService==PlaceOfService.Office) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsHospADA2002":
						if(ClaimCur.PlaceService==PlaceOfService.InpatHospital
							|| ClaimCur.PlaceService==PlaceOfService.OutpatHospital) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsExtCareFacilityADA2002":
						if(ClaimCur.PlaceService==PlaceOfService.CustodialCareFacility
							|| ClaimCur.PlaceService==PlaceOfService.SkilledNursFac) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsOtherADA2002":
						if(ClaimCur.PlaceService==PlaceOfService.PatientsHome
							|| ClaimCur.PlaceService==PlaceOfService.OtherLocation) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsInpatHosp":
						if(ClaimCur.PlaceService==PlaceOfService.InpatHospital) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsOutpatHosp":
						if(ClaimCur.PlaceService==PlaceOfService.OutpatHospital) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsAdultLivCareFac":
						if(ClaimCur.PlaceService==PlaceOfService.CustodialCareFacility) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsSkilledNursFac":
						if(ClaimCur.PlaceService==PlaceOfService.SkilledNursFac) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsPatientsHome":
						if(ClaimCur.PlaceService==PlaceOfService.PatientsHome) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceIsOtherLocation":
						if(ClaimCur.PlaceService==PlaceOfService.OtherLocation) {
							displayStrings[i]="X";
						}
						break;
					case "PlaceNumericCode":
						displayStrings[i]=X12object.GetPlaceService(ClaimCur.PlaceService);
						break;
					#endregion Place
					case "IsRadiographsAttached":
						if(ClaimCur.Radiographs>0) {
							displayStrings[i]="X";
						}
						break;
					case "RadiographsNumAttached":
						displayStrings[i]=ClaimCur.Radiographs.ToString();
						break;
					case "RadiographsNotAttached":
						if(ClaimCur.Radiographs==0) {
							displayStrings[i]="X";
						}
						break;
					case "IsEnclosuresAttached":
						if(ClaimCur.Radiographs>0 || ClaimCur.AttachedImages>0 || ClaimCur.AttachedModels>0) {
							displayStrings[i]="X";
						}
						break;
					case "AttachedImagesNum":
						displayStrings[i]=ClaimCur.AttachedImages.ToString();
						break;
					case "AttachedModelsNum":
						displayStrings[i]=ClaimCur.AttachedModels.ToString();
						break;
					#region Ortho
					case "IsNotOrtho":
						if(!ClaimCur.IsOrtho) {
							displayStrings[i]="X";
						}
						break;
					case "IsOrtho":
						if(ClaimCur.IsOrtho) {
							displayStrings[i]="X";
						}
						break;
					case "DateOrthoPlaced":
						if(ClaimCur.OrthoDate.Year > 1880){
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.OrthoDate.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.OrthoDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "MonthsOrthoRemaining":
						if(ClaimCur.OrthoRemainM > 0) {
							displayStrings[i]=ClaimCur.OrthoRemainM.ToString();
						}
						break;
					case "MonthsOrthoTotal":
						if(ClaimCur.OrthoTotalM > 0) {
							displayStrings[i]=ClaimCur.OrthoTotalM.ToString();
						}
						break;
					#endregion Ortho
					#region Prosth
					case "IsNotProsth":
						if(ClaimCur.IsProsthesis=="N") {
							displayStrings[i]="X";
						}
						break;
					case "IsInitialProsth":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							//Canadian claim form has two sections for initial/replacement prosthesis. Checkbox is only checked if maxiliar and mandibular settings
							//do not conflict, i.e. if user chooses Max=Initial and Mand=Replacment, box left blank.  However, Max=Initial and Mand=NotAProsthesis 
							//will check this box.
							((ClaimCur.CanadianIsInitialUpper=="Y" && ClaimCur.CanadianIsInitialLower!="N") 
								|| (ClaimCur.CanadianIsInitialLower=="Y" && ClaimCur.CanadianIsInitialUpper!="N")))
						{ 
							displayStrings[i]="X";
						}
						else if(ClaimCur.IsProsthesis=="I") {
							displayStrings[i]="X";
						}
						break;
					case "IsNotReplacementProsth":
						if(ClaimCur.IsProsthesis!="R") {//=='I'nitial or 'N'o
							displayStrings[i]="X";
						}
						break;
					case "IsReplacementProsth":
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							//Canadian claim form has two sections for initial/replacement prosthesis. Checkbox is only checked if maxiliar and mandibular settings
							//do not conflict, i.e. if user chooses Max=Initial and Mand=Replacment, box left blank.  However, Max=Replacement and 
							//Mand=NotAProsthesis will check this box.
							((ClaimCur.CanadianIsInitialUpper=="N" && ClaimCur.CanadianIsInitialLower!="Y")
								|| (ClaimCur.CanadianIsInitialLower=="N" && ClaimCur.CanadianIsInitialUpper!="Y")))
						{ 
							displayStrings[i]="X";
						}
						else if(ClaimCur.IsProsthesis=="R") {
							displayStrings[i]="X";
						}
						break;
					case "DatePriorProsthPlaced":
						if(ClaimCur.PriorDate.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.PriorDate.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.PriorDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					#endregion Prosth
					#region Occupational and Accident
					case "IsOccupational":
						if(ClaimCur.AccidentRelated=="E") {
							displayStrings[i]="X";
						}
						break;
					case "IsNotOccupational":
						if(ClaimCur.AccidentRelated!="E") {
							displayStrings[i]="X";
						}
						break;
					case "IsAutoAccident":
						if(ClaimCur.AccidentRelated=="A") {
							displayStrings[i]="X";
						}
						break;
					case "IsNotAutoAccident":
						if(ClaimCur.AccidentRelated!="A") {
							displayStrings[i]="X";
						}
						break;
					case "IsOtherAccident":
						if(ClaimCur.AccidentRelated=="O") {
							displayStrings[i]="X";
						}
						break;
					case "IsNotOtherAccident":
						if(ClaimCur.AccidentRelated!="O") {
							displayStrings[i]="X";
						}
						break;
					case "IsNotAccident":
						if(ClaimCur.AccidentRelated=="" && (!CultureInfo.CurrentCulture.Name.EndsWith("CA") || ClaimCur.AccidentDate.Year <= 1860)) {
							displayStrings[i]="X";
						}
						break;
					case "IsAccident":
						if(ClaimCur.AccidentRelated!="" || (CultureInfo.CurrentCulture.Name.EndsWith("CA") && ClaimCur.AccidentDate.Year > 1860)) {
							displayStrings[i]="X";
						}
						break;
					case "AccidentDate":
						if(ClaimCur.AccidentDate.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.AccidentDate.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.AccidentDate.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "AccidentST":
						displayStrings[i]=ClaimCur.AccidentST;
						break;
					#endregion Occupational and Accident
					#region BillingDentist
					case "BillingDentist":
						if(providerClaimBill.IsNotPerson) {
							displayStrings[i]=providerClaimBill.LName+" "+providerClaimBill.Suffix;
						}
						else {
							displayStrings[i]=providerClaimBill.FName+" "+providerClaimBill.MI+" "+providerClaimBill.LName+" "+providerClaimBill.Suffix;
						}
						break;
					case "BillingDentistMedicaidID":
						displayStrings[i]=providerClaimBill.MedicaidID;
						break;
					case "BillingDentistProviderID":
						ProviderIdent[] provIdents=ProviderIdents.GetForPayor(ClaimCur.ProvBill,carrier.ElectID);
						if(provIdents.Length>0){
							displayStrings[i]=provIdents[0].IDNumber;//just use the first one we find
						}
						break;
					case "BillingDentistNPI":
						displayStrings[i]=providerClaimBill.NationalProvID;
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && //Canadian. en-CA or fr-CA
							carrier.ElectID=="000064" && //Pacific Blue Cross (PBC)
							providerClaimBill.NationalProvID!=providerClaimTreat.NationalProvID && //Billing and treating providers are different
							displayStrings[i].Length==9) { //Only for provider numbers which have been entered correctly (to prevent and indexing exception).
							displayStrings[i]="00"+displayStrings[i].Substring(2,5)+"00";
						}
						break;
					case "BillingDentistLicenseNum":
						displayStrings[i]=(provClinicClaimBill==null ? "" : provClinicClaimBill.StateLicense);
						break;
					case "BillingDentistSSNorTIN":
						displayStrings[i]=providerClaimBill.SSN;
						break;
					case "BillingDentistNumIsSSN":
						if(!providerClaimBill.UsingTIN) {
							displayStrings[i]="X";
						}
						break;
					case "BillingDentistNumIsTIN":
						if(providerClaimBill.UsingTIN) {
							displayStrings[i]="X";
						}
						break;
					case "BillingDentistPh123":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(0,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(0,3);
							}
						}
						break;
					case "BillingDentistPh456":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(3,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(3,3);
							}
						}
						break;
					case "BillingDentistPh78910":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(6);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(6);
							}
						}
						break;
					case "BillingDentistPhoneFormatted":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]="("+PrefC.GetString(PrefName.PracticePhone).Substring(0,3)
									+")"+PrefC.GetString(PrefName.PracticePhone).Substring(3,3)
									+"-"+PrefC.GetString(PrefName.PracticePhone).Substring(6);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]="("+clinic.Phone.Substring(0,3)
									+")"+clinic.Phone.Substring(3,3)
									+"-"+clinic.Phone.Substring(6);
							}
						}
						break;
					case "BillingDentistPhoneRaw":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticePhone);
						}
						else {
							displayStrings[i]=clinic.Phone;
						}
						break;
					#endregion BillingDentist
					#region PayToDentist
					case "PayToDentistAddress": //Behaves just like the old BillingDentistAddress field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							displayStrings[i]=clinic.PayToAddress;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
						  displayStrings[i]=PrefC.GetString(PrefName.PracticePayToAddress);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims) {
							displayStrings[i]=clinic.BillingAddress;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)){
						  displayStrings[i]=PrefC.GetString(PrefName.PracticeBillingAddress);
						}
						else if(clinic==null) {
						  displayStrings[i]=PrefC.GetString(PrefName.PracticeAddress);
						}
						else {
							displayStrings[i]=clinic.Address;
						}
						break;
					case "PayToDentistAddress2": //Behaves just like the old BillingDentistAddress2 field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							displayStrings[i]=clinic.PayToAddress2;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
						  displayStrings[i]=PrefC.GetString(PrefName.PracticePayToAddress2);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims) {
							displayStrings[i]=clinic.BillingAddress2;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)){
						  displayStrings[i]=PrefC.GetString(PrefName.PracticeBillingAddress2);
						}
						else if(clinic==null) {
						  displayStrings[i]=PrefC.GetString(PrefName.PracticeAddress2);
						}
						else {
							displayStrings[i]=clinic.Address2;
						}
						break;
					case "PayToDentistCity": //Behaves just like the old BillingDentistCity field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							displayStrings[i]=clinic.PayToCity;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							displayStrings[i]=PrefC.GetString(PrefName.PracticePayToCity);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims) {
							displayStrings[i]=clinic.BillingCity;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeBillingCity);
						}
						else if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeCity);
						}
						else {
							displayStrings[i]=clinic.City;
						}
						break;
					case "PayToDentistST": //Behaves just like the old BillingDentistST field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							displayStrings[i]=clinic.PayToState;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							displayStrings[i]=PrefC.GetString(PrefName.PracticePayToST);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims) {
							displayStrings[i]=clinic.BillingState;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeBillingST);
						}
						else if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeST);
						}
						else {
							displayStrings[i]=clinic.State;
						}
						break;
					case "PayToDentistZip": //Behaves just like the old BillingDentistZip field, but is overridden by the Pay-To address if the Pay-To address has been specified.
						if(clinic!=null && clinic.PayToAddress!="") {
							displayStrings[i]=clinic.PayToZip;
						}
						else if(PrefC.GetString(PrefName.PracticePayToAddress)!="") { //All Pay-To address fields are used in 5010 eclaims when Pay-To address line 1 is not blank.
							displayStrings[i]=PrefC.GetString(PrefName.PracticePayToZip);
						}
						else if(clinic!=null && clinic.UseBillAddrOnClaims) {
							displayStrings[i]=clinic.BillingZip;
						}
						else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeBillingZip);
						}
						else if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeZip);
						}
						else {
							displayStrings[i]=clinic.Zip;
						}
						break;
					#endregion PayToDentist
					#region TreatingDentist
					case "TreatingDentist":
						if(providerClaimTreat.IsNotPerson) {
							displayStrings[i]=providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
						}
						else {
							displayStrings[i]=providerClaimTreat.FName+" "+providerClaimTreat.MI+" "+providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
						}
						break;
					case "TreatingDentistFName":
						displayStrings[i]=providerClaimTreat.FName;
						break;
					case "TreatingDentistLName":
						displayStrings[i]=providerClaimTreat.LName;
						break;
					case "TreatingDentistSignature":
						if(providerClaimTreat.SigOnFile){
							if(PrefC.GetBool(PrefName.ClaimFormTreatDentSaysSigOnFile)){
								displayStrings[i]="Signature on File";
							}
							else{
								displayStrings[i]=providerClaimTreat.FName+" "+providerClaimTreat.MI+" "+providerClaimTreat.LName+" "+providerClaimTreat.Suffix;
							}
						}
						break;
					case "TreatingDentistSigDate":
						if(providerClaimTreat.SigOnFile && ClaimCur.DateSent.Year > 1860){
							if(ClaimFormCur.Items[i].FormatString=="") {
								displayStrings[i]=ClaimCur.DateSent.ToShortDateString();
							}
							else {
								displayStrings[i]=ClaimCur.DateSent.ToString(ClaimFormCur.Items[i].FormatString);
							}
						}
						break;
					case "TreatingDentistMedicaidID":
						displayStrings[i]=providerClaimTreat.MedicaidID;
						break;
					case "TreatingDentistProviderID":
						provIdents=ProviderIdents.GetForPayor(ClaimCur.ProvTreat,carrier.ElectID);
						if(provIdents.Length>0) {
							displayStrings[i]=provIdents[0].IDNumber;//just use the first one we find
						}
						break;
					case "TreatingDentistNPI":
						displayStrings[i]=providerClaimTreat.NationalProvID;
						break;
					case "TreatingDentistLicense":
						displayStrings[i]=(provClinicClaimTreat==null ? "" : provClinicClaimTreat.StateLicense);
						break;
					case "TreatingDentistAddress":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeAddress);
						}
						else {
							displayStrings[i]=clinic.Address;
						}
						break;
					case "TreatingDentistAddress2":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeAddress2);
						}
						else {
							displayStrings[i]=clinic.Address2;
						}
						break;
					case "TreatingDentistCity":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeCity);
						}
						else {
							displayStrings[i]=clinic.City;
						}
						break;
					case "TreatingDentistST":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeST);
						}
						else {
							displayStrings[i]=clinic.State;
						}
						break;
					case "TreatingDentistZip":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticeZip);
						}
						else {
							displayStrings[i]=clinic.Zip;
						}
						break;
					case "TreatingDentistPh123":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(0,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(0,3);
							}
						}
						break;
					case "TreatingDentistPh456":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(3,3);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(3,3);
							}
						}
						break;
					case "TreatingDentistPh78910":
						if(clinic==null){
							if(PrefC.GetString(PrefName.PracticePhone).Length==10){
								displayStrings[i]=PrefC.GetString(PrefName.PracticePhone).Substring(6);
							}
						}
						else{
							if(clinic.Phone.Length==10){
								displayStrings[i]=clinic.Phone.Substring(6);
							}
						}
						break;
					case "TreatingDentistPhoneRaw":
						if(clinic==null) {
							displayStrings[i]=PrefC.GetString(PrefName.PracticePhone);
						}
						else {
							displayStrings[i]=clinic.Phone;
						}
						break;
					case "TreatingProviderSpecialty":
						displayStrings[i]=X12Generator.GetTaxonomy(providerClaimTreat);
						break;
					#endregion TreatingDentist
					case "TotalPages":
						displayStrings[i]="";//totalPages.ToString();//bugs with this field that we can't fix since we didn't write that code.
						break;
					case "ReferringProvNPI":
						if(ClaimReferral==null){
							displayStrings[i]="";
						}
						else{
							displayStrings[i]=ClaimReferral.NationalProvID;
						}
						break;
					case "ReferringProvNameFL":
						if(ClaimReferral==null){
							displayStrings[i]="";
						}
						else{
							displayStrings[i]=ClaimReferral.GetNameFL();
						}
						break;
					#region Med
					case "MedUniformBillType":
						displayStrings[i]=ClaimCur.UniformBillType;
						break;
					case "MedAdmissionTypeCode":
						displayStrings[i]=ClaimCur.AdmissionTypeCode;
						break;
					case "MedAdmissionSourceCode":
						displayStrings[i]=ClaimCur.AdmissionSourceCode;
						break;
					case "MedPatientStatusCode":
						displayStrings[i]=ClaimCur.PatientStatusCode;
						break;
					case "MedAccidentCode": //For UB04.
						if(ClaimCur.AccidentRelated=="A") { //Auto accident
							displayStrings[i]="01";
						}
						else if(ClaimCur.AccidentRelated=="E") { //Employment related accident
							displayStrings[i]="04";
						}
						break;
					#endregion Med
					case "ICDindicator"://For the 1500_02_12 claim form.
						byte icdVersion=0;
						List<byte> listDxVersions=new List<byte>();
						Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(ListClaimProcs),false,listDxVersions);
						if(listDxVersions.Count>0) {
							icdVersion=listDxVersions[0];
						}
						if(icdVersion==9) {
							displayStrings[i]="9";
						}
						else if(icdVersion==10) {
							displayStrings[i]="0";
						}
						break;
					case "ICDindicatorAB"://For the ADA 2012 claim form.
						icdVersion=0;
						listDxVersions=new List<byte>();
						Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(ListClaimProcs),false,listDxVersions);
						if(listDxVersions.Count>0) {
							icdVersion=listDxVersions[0];
						}
						if(icdVersion==9) {
							displayStrings[i]="B";
						}
						else if(icdVersion==10) {
							displayStrings[i]="AB";
						}
						break;
					case "AcceptAssignmentY":
						displayStrings[i]=subCur.AssignBen?"X":"";
						break;
					case "AcceptAssignmentN":
						displayStrings[i]=subCur.AssignBen?"":"X";
						break;
					case "ClaimIdentifier":
						displayStrings[i]=ClaimCur.ClaimIdentifier;
					break;
					case "OrigRefNum":
						displayStrings[i]=ClaimCur.OrigRefNum;
						break;
					case "CorrectionType":
						displayStrings[i]="";//Original claim
						if(ClaimCur.CorrectionType==ClaimCorrectionType.Replacement) {
							displayStrings[i]="7";
						}
						else if(ClaimCur.CorrectionType==ClaimCorrectionType.Void) {
							displayStrings[i]="8";
						}
						break;
					case "DateIllnessInjuryPreg":
						if(ClaimCur.DateIllnessInjuryPreg.Year>1880) {
							displayStrings[i]=string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)?ClaimCur.DateIllnessInjuryPreg.ToShortDateString():
								ClaimCur.DateIllnessInjuryPreg.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateIllnessInjuryPregQualifier":
						displayStrings[i]=ClaimCur.DateIllnessInjuryPregQualifier==DateIllnessInjuryPregQualifier.None?"":((int)ClaimCur.DateIllnessInjuryPregQualifier).ToString("000");
						break;
					case "DateOther":
						if(ClaimCur.DateOther.Year>1880){
							displayStrings[i]=string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)?ClaimCur.DateOther.ToShortDateString():
								ClaimCur.DateOther.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateOtherQualifier":
						displayStrings[i]=ClaimCur.DateOtherQualifier==DateOtherQualifier.None?"":((int)ClaimCur.DateOtherQualifier).ToString("000");
						break;
					case "IsOutsideLab":
						displayStrings[i]=ClaimCur.IsOutsideLab?"X":"";
						break;
					case "IsNotOutsideLab":
						displayStrings[i]=ClaimCur.IsOutsideLab?"":"X";
						break;
					case "OfficeNumber":
						displayStrings[i]=providerClaimTreat.CanadianOfficeNum;
						break;
					case "OutsideLabFee":
						if(!ClaimCur.IsOutsideLab) {
							displayStrings[i]="";
							break;
						}
						//if this is for an outside lab fee there should only be one procedure on the claim and that should be for the outside fee.  We will use
						//the claim fee, it's up to the user to only include the outside lab fee on the claim.
						if(string.IsNullOrEmpty(ClaimFormCur.Items[i].FormatString)) {
							displayStrings[i]=((decimal)ClaimCur.ClaimFee).ToString("F");
						}
						else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
							displayStrings[i]=((decimal)ClaimCur.ClaimFee).ToString("F").Replace("."," ");
						}
						else {
							displayStrings[i]=((decimal)ClaimCur.ClaimFee).ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
				}//switch
				if(CultureInfo.CurrentCulture.Name=="nl-BE"	&& displayStrings[i]==""){//Dutch Belgium
					displayStrings[i]="*   *   *";
				}
				//Renaissance eclaims only: Remove newlines from display strings to prevent formatting issues, because the .rss file format requires each field on a single line.
				if(isRenaissance && displayStrings[i]!=null) {
					displayStrings[i]=displayStrings[i].Replace("\r","").Replace("\n","");
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
				InsPlan planCur = InsPlans.GetPlan(ClaimCur.PlanNum, ListInsPlan);
				qty = 0;
				switch(ClaimFormCur.Items[i].FieldName){
					//there is no default, because any non-matches will remain as ""
					#region P1
					case "P1SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(1,startProc);
						break;
					case "P1Date":
						displayStrings[i]=GetProcInfo("Date",1+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P1Area":
						displayStrings[i]=GetProcInfo("Area",1+startProc);
						break;
					case "P1System":
						displayStrings[i]=GetProcInfo("System",1+startProc);
						break;
					case "P1ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",1+startProc);
						break;
					case "P1Surface":
						displayStrings[i]=GetProcInfo("Surface",1+startProc);
						break;
					case "P1Code":
						displayStrings[i]=GetProcInfo("Code",1+startProc);
						break;
					case "P1Description":
						displayStrings[i]=GetProcInfo("Desc",1+startProc);
						break;
					case "P1Fee":
						displayStrings[i]=GetProcInfo("Fee",1+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P1TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",1+startProc);
						break;
					case "P1PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",1+startProc);
						break;
					case "P1Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",1+startProc);
						break;
					case "P1DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",1+startProc);
						break;
					case "P1Lab":
						displayStrings[i]=GetProcInfo("Lab",1+startProc);
						break;
					case "P1FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",1+startProc);
						break;
					case "P1ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",1+startProc);
						break;
					case "P1TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",1+startProc);
						break;
					case "P1RevCode":
						displayStrings[i]=GetProcInfo("RevCode",1+startProc);
						break;
					case "P1CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",1+startProc);
						break;
					case "P1CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",1+startProc);
						break;
					case "P1CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",1+startProc);
						break;
					case "P1CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",1+startProc);
						break;
					case "P1Minutes":
						displayStrings[i]=GetProcInfo("Minutes",1+startProc);
						break;
					case "P1UnitQty":
						if(planCur.ShowBaseUnits) {
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
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P1UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",1+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(1,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P1CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",1+startProc) + GetProcInfo("CodeMod1",1+startProc) + GetProcInfo("CodeMod2",1+startProc) + GetProcInfo("CodeMod3",1+startProc) + GetProcInfo("CodeMod4",1+startProc);
						break;
					case "P1IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",1+startProc);
						break;
					case "P1eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",1+startProc);
						break;
					#endregion P1
					#region P2
					case "P2SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(2,startProc);
						break;
					case "P2Date":
						displayStrings[i]=GetProcInfo("Date",2+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P2Area":
						displayStrings[i]=GetProcInfo("Area",2+startProc);
						break;
					case "P2System":
						displayStrings[i]=GetProcInfo("System",2+startProc);
						break;
					case "P2ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",2+startProc);
						break;
					case "P2Surface":
						displayStrings[i]=GetProcInfo("Surface",2+startProc);
						break;
					case "P2Code":
						displayStrings[i]=GetProcInfo("Code",2+startProc);
						break;
					case "P2Description":
						displayStrings[i]=GetProcInfo("Desc",2+startProc);
						break;
					case "P2Fee":
						displayStrings[i]=GetProcInfo("Fee",2+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P2TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",2+startProc);
						break;
					case "P2PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",2+startProc);
						break;
					case "P2Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",2+startProc);
						break;
					case "P2DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",2+startProc);
						break;
					case "P2Lab":
						displayStrings[i]=GetProcInfo("Lab",2+startProc);
						break;
					case "P2FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",2+startProc);
						break;
					case "P2ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",2+startProc);
						break;
					case "P2TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",2+startProc);
						break;
					case "P2RevCode":
						displayStrings[i]=GetProcInfo("RevCode",2+startProc);
						break;
					case "P2CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",2+startProc);
						break;
					case "P2CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",2+startProc);
						break;
					case "P2CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",2+startProc);
						break;
					case "P2CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",2+startProc);
						break;
					case "P2Minutes":
						displayStrings[i]=GetProcInfo("Minutes",2+startProc);
						break;
					case "P2UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",2+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",2+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",2+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",2+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P2UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",2+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(2,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P2CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",2+startProc) 
							+ GetProcInfo("CodeMod1",2+startProc) 
							+ GetProcInfo("CodeMod2",2+startProc) 
							+ GetProcInfo("CodeMod3",2+startProc) 
							+ GetProcInfo("CodeMod4",2+startProc);
						break;
					case "P2IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",2+startProc);
						break;
					case "P2eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",2+startProc);
						break;
					#endregion P2
					#region P3
					case "P3SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(3,startProc);
						break;
					case "P3Date":
						displayStrings[i]=GetProcInfo("Date",3+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P3Area":
						displayStrings[i]=GetProcInfo("Area",3+startProc);
						break;
					case "P3System":
						displayStrings[i]=GetProcInfo("System",3+startProc);
						break;
					case "P3ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",3+startProc);
						break;
					case "P3Surface":
						displayStrings[i]=GetProcInfo("Surface",3+startProc);
						break;
					case "P3Code":
						displayStrings[i]=GetProcInfo("Code",3+startProc);
						break;
					case "P3Description":
						displayStrings[i]=GetProcInfo("Desc",3+startProc);
						break;
					case "P3Fee":
						displayStrings[i]=GetProcInfo("Fee",3+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P3TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",3+startProc);
						break;
					case "P3PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",3+startProc);
						break;
					case "P3Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",3+startProc);
						break;
					case "P3DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",3+startProc);
						break;
					case "P3Lab":
						displayStrings[i]=GetProcInfo("Lab",3+startProc);
						break;
					case "P3FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",3+startProc);
						break;
					case "P3ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",3+startProc);
						break;
					case "P3TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",3+startProc);
						break;
					case "P3RevCode":
						displayStrings[i]=GetProcInfo("RevCode",3+startProc);
						break;
					case "P3CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",3+startProc);
						break;
					case "P3CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",3+startProc);
						break;
					case "P3CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",3+startProc);
						break;
					case "P3CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",3+startProc);
						break;
					case "P3Minutes":
						displayStrings[i]=GetProcInfo("Minutes",3+startProc);
						break;
					case "P3UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",3+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",3+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",3+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",3+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P3UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",3+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(3,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P3CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",3+startProc) 
							+ GetProcInfo("CodeMod1",3+startProc) 
							+ GetProcInfo("CodeMod2",3+startProc) 
							+ GetProcInfo("CodeMod3",3+startProc) 
							+ GetProcInfo("CodeMod4",3+startProc);
						break;
					case "P3IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",3+startProc);
						break;
					case "P3eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",3+startProc);
						break;
					#endregion P3
					#region P4
					case "P4SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(4,startProc);
						break;
					case "P4Date":
						displayStrings[i]=GetProcInfo("Date",4+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P4Area":
						displayStrings[i]=GetProcInfo("Area",4+startProc);
						break;
					case "P4System":
						displayStrings[i]=GetProcInfo("System",4+startProc);
						break;
					case "P4ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",4+startProc);
						break;
					case "P4Surface":
						displayStrings[i]=GetProcInfo("Surface",4+startProc);
						break;
					case "P4Code":
						displayStrings[i]=GetProcInfo("Code",4+startProc);
						break;
					case "P4Description":
						displayStrings[i]=GetProcInfo("Desc",4+startProc);
						break;
					case "P4Fee":
						displayStrings[i]=GetProcInfo("Fee",4+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P4TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",4+startProc);
						break;
					case "P4PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",4+startProc);
						break;
					case "P4Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",4+startProc);
						break;
					case "P4DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",4+startProc);
						break;
					case "P4Lab":
						displayStrings[i]=GetProcInfo("Lab",4+startProc);
						break;
					case "P4FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",4+startProc);
						break;
					case "P4ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",4+startProc);
						break;
					case "P4TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",4+startProc);
						break;
					case "P4RevCode":
						displayStrings[i]=GetProcInfo("RevCode",4+startProc);
						break;
					case "P4CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",4+startProc);
						break;
					case "P4CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",4+startProc);
						break;
					case "P4CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",4+startProc);
						break;
					case "P4CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",4+startProc);
						break;
					case "P4Minutes":
						displayStrings[i]=GetProcInfo("Minutes",4+startProc);
						break;
					case "P4UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",4+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",4+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",4+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",4+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P4UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",4+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(4,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P4CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",4+startProc) 
							+ GetProcInfo("CodeMod1",4+startProc) 
							+ GetProcInfo("CodeMod2",4+startProc) 
							+ GetProcInfo("CodeMod3",4+startProc) 
							+ GetProcInfo("CodeMod4",4+startProc);
						break;
					case "P4IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",4+startProc);
						break;
					case "P4eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",4+startProc);
						break;
					#endregion P4
					#region P5
					case "P5SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(5,startProc);
						break;
					case "P5Date":
						displayStrings[i]=GetProcInfo("Date",5+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P5Area":
						displayStrings[i]=GetProcInfo("Area",5+startProc);
						break;
					case "P5System":
						displayStrings[i]=GetProcInfo("System",5+startProc);
						break;
					case "P5ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",5+startProc);
						break;
					case "P5Surface":
						displayStrings[i]=GetProcInfo("Surface",5+startProc);
						break;
					case "P5Code":
						displayStrings[i]=GetProcInfo("Code",5+startProc);
						break;
					case "P5Description":
						displayStrings[i]=GetProcInfo("Desc",5+startProc);
						break;
					case "P5Fee":
						displayStrings[i]=GetProcInfo("Fee",5+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P5TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",5+startProc);
						break;
					case "P5PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",5+startProc);
						break;
					case "P5Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",5+startProc);
						break;
					case "P5DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",5+startProc);
						break;
					case "P5Lab":
						displayStrings[i]=GetProcInfo("Lab",5+startProc);
						break;
					case "P5FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",5+startProc);
						break;
					case "P5ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",5+startProc);
						break;
					case "P5TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",5+startProc);
						break;
					case "P5RevCode":
						displayStrings[i]=GetProcInfo("RevCode",5+startProc);
						break;
					case "P5CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",5+startProc);
						break;
					case "P5CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",5+startProc);
						break;
					case "P5CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",5+startProc);
						break;
					case "P5CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",5+startProc);
						break;
					case "P5Minutes":
						displayStrings[i]=GetProcInfo("Minutes",5+startProc);
						break;
					case "P5UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",5+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",5+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",5+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",5+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P5UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",5+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(5,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P5CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",5+startProc) 
							+ GetProcInfo("CodeMod1",5+startProc) 
							+ GetProcInfo("CodeMod2",5+startProc) 
							+ GetProcInfo("CodeMod3",5+startProc) 
							+ GetProcInfo("CodeMod4",5+startProc);
						break;
					case "P5IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",5+startProc);
						break;
					case "P5eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",5+startProc);
						break;
					#endregion P5
					#region P6
					case "P6SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(6,startProc);
						break;
					case "P6Date":
						displayStrings[i]=GetProcInfo("Date",6+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P6Area":
						displayStrings[i]=GetProcInfo("Area",6+startProc);
						break;
					case "P6System":
						displayStrings[i]=GetProcInfo("System",6+startProc);
						break;
					case "P6ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",6+startProc);
						break;
					case "P6Surface":
						displayStrings[i]=GetProcInfo("Surface",6+startProc);
						break;
					case "P6Code":
						displayStrings[i]=GetProcInfo("Code",6+startProc);
						break;
					case "P6Description":
						displayStrings[i]=GetProcInfo("Desc",6+startProc);
						break;
					case "P6Fee":
						displayStrings[i]=GetProcInfo("Fee",6+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P6TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",6+startProc);
						break;
					case "P6PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",6+startProc);
						break;
					case "P6Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",6+startProc);
						break;
					case "P6DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",6+startProc);
						break;
					case "P6Lab":
						displayStrings[i]=GetProcInfo("Lab",6+startProc);
						break;
					case "P6FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",6+startProc);
						break;
					case "P6ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",6+startProc);
						break;
					case "P6TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",6+startProc);
						break;
					case "P6RevCode":
						displayStrings[i]=GetProcInfo("RevCode",6+startProc);
						break;
					case "P6CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",6+startProc);
						break;
					case "P6CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",6+startProc);
						break;
					case "P6CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",6+startProc);
						break;
					case "P6CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",6+startProc);
						break;
					case "P6Minutes":
						displayStrings[i]=GetProcInfo("Minutes",6+startProc);
						break;
					case "P6UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",6+startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",6+startProc),out uqty);
							qty=bunit+uqty;
						}
						else if(GetProcInfo("UnitQty",6+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",6+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P6UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",6+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(6,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P6CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",6+startProc) 
							+ GetProcInfo("CodeMod1",6+startProc) 
							+ GetProcInfo("CodeMod2",6+startProc) 
							+ GetProcInfo("CodeMod3",6+startProc) 
							+ GetProcInfo("CodeMod4",6+startProc);
						break;
					case "P6IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",6+startProc);
						break;
					case "P6eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",6+startProc);
						break;
					#endregion P6
					#region P7
					case "P7SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(7,startProc);
						break;
					case "P7Date":
						displayStrings[i]=GetProcInfo("Date",7+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P7Area":
						displayStrings[i]=GetProcInfo("Area",7+startProc);
						break;
					case "P7System":
						displayStrings[i]=GetProcInfo("System",7+startProc);
						break;
					case "P7ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",7+startProc);
						break;
					case "P7Surface":
						displayStrings[i]=GetProcInfo("Surface",7+startProc);
						break;
					case "P7Code":
						displayStrings[i]=GetProcInfo("Code",7+startProc);
						break;
					case "P7Description":
						displayStrings[i]=GetProcInfo("Desc",7+startProc);
						break;
					case "P7Fee":
						displayStrings[i]=GetProcInfo("Fee",7+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P7TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",7+startProc);
						break;
					case "P7PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",7+startProc);
						break;
					case "P7Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",7+startProc);
						break;
					case "P7DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",7+startProc);
						break;
					case "P7Lab":
						displayStrings[i]=GetProcInfo("Lab",7+startProc);
						break;
					case "P7FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",7+startProc);
						break;
					case "P7ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",7+startProc);
						break;
					case "P7TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",7+startProc);
						break;
					case "P7RevCode":
						displayStrings[i]=GetProcInfo("RevCode",7+startProc);
						break;
					case "P7CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",7+startProc);
						break;
					case "P7CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",7+startProc);
						break;
					case "P7CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",7+startProc);
						break;
					case "P7CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",7+startProc);
						break;
					case "P7Minutes":
						displayStrings[i]=GetProcInfo("Minutes",7+startProc);
						break;
					case "P7UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",7 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",7 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",7+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",7+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P7UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",7+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(7,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P7CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",7+startProc) 
							+ GetProcInfo("CodeMod1",7+startProc) 
							+ GetProcInfo("CodeMod2",7+startProc) 
							+ GetProcInfo("CodeMod3",7+startProc) 
							+ GetProcInfo("CodeMod4",7+startProc);
						break;
					case "P7IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",7+startProc);
						break;
					case "P7eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",7+startProc);
						break;
					#endregion P7
					#region P8
					case "P8SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(8,startProc);
						break;
					case "P8Date":
						displayStrings[i]=GetProcInfo("Date",8+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P8Area":
						displayStrings[i]=GetProcInfo("Area",8+startProc);
						break;
					case "P8System":
						displayStrings[i]=GetProcInfo("System",8+startProc);
						break;
					case "P8ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",8+startProc);
						break;
					case "P8Surface":
						displayStrings[i]=GetProcInfo("Surface",8+startProc);
						break;
					case "P8Code":
						displayStrings[i]=GetProcInfo("Code",8+startProc);
						break;
					case "P8Description":
						displayStrings[i]=GetProcInfo("Desc",8+startProc);
						break;
					case "P8Fee":
						displayStrings[i]=GetProcInfo("Fee",8+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P8TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",8+startProc);
						break;
					case "P8PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",8+startProc);
						break;
					case "P8Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",8+startProc);
						break;
					case "P8DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",8+startProc);
						break;
					case "P8Lab":
						displayStrings[i]=GetProcInfo("Lab",8+startProc);
						break;
					case "P8FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",8+startProc);
						break;
					case "P8ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",8+startProc);
						break;
					case "P8TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",8+startProc);
						break;
					case "P8RevCode":
						displayStrings[i]=GetProcInfo("RevCode",8+startProc);
						break;
					case "P8CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",8+startProc);
						break;
					case "P8CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",8+startProc);
						break;
					case "P8CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",8+startProc);
						break;
					case "P8CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",8+startProc);
						break;
					case "P8Minutes":
						displayStrings[i]=GetProcInfo("Minutes",8+startProc);
						break;
					case "P8UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",8 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",8 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",8+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",8+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P8UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",8+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(8,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P8IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",8+startProc);
						break;
					case "P8eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",8+startProc);
						break;
					#endregion P8
					#region P9
					case "P8CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",8+startProc) 
							+ GetProcInfo("CodeMod1",8+startProc) 
							+ GetProcInfo("CodeMod2",8+startProc) 
							+ GetProcInfo("CodeMod3",8+startProc) 
							+ GetProcInfo("CodeMod4",8+startProc);
						break;
					case "P9SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(9,startProc);
						break;
					case "P9Date":
						displayStrings[i]=GetProcInfo("Date",9+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P9Area":
						displayStrings[i]=GetProcInfo("Area",9+startProc);
						break;
					case "P9System":
						displayStrings[i]=GetProcInfo("System",9+startProc);
						break;
					case "P9ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",9+startProc);
						break;
					case "P9Surface":
						displayStrings[i]=GetProcInfo("Surface",9+startProc);
						break;
					case "P9Code":
						displayStrings[i]=GetProcInfo("Code",9+startProc);
						break;
					case "P9Description":
						displayStrings[i]=GetProcInfo("Desc",9+startProc);
						break;
					case "P9Fee":
						displayStrings[i]=GetProcInfo("Fee",9+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P9TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",9+startProc);
						break;
					case "P9PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",9+startProc);
						break;
					case "P9Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",9+startProc);
						break;
					case "P9DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",9+startProc);
						break;
					case "P9Lab":
						displayStrings[i]=GetProcInfo("Lab",9+startProc);
						break;
					case "P9FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",9+startProc);
						break;
					case "P9ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",9+startProc);
						break;
					case "P9TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",9+startProc);
						break;
					case "P9RevCode":
						displayStrings[i]=GetProcInfo("RevCode",9+startProc);
						break;
					case "P9CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",9+startProc);
						break;
					case "P9CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",9+startProc);
						break;
					case "P9CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",9+startProc);
						break;
					case "P9CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",9+startProc);
						break;
					case "P9Minutes":
						displayStrings[i]=GetProcInfo("Minutes",9+startProc);
						break;
					case "P9UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",9 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",9 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",9+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",9+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P9UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",9+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(9,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P9CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",9+startProc) 
							+ GetProcInfo("CodeMod1",9+startProc) 
							+ GetProcInfo("CodeMod2",9+startProc) 
							+ GetProcInfo("CodeMod3",9+startProc) 
							+ GetProcInfo("CodeMod4",9+startProc);
						break;
					case "P9IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",9+startProc);
						break;
					case "P9eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",9+startProc);
						break;
					#endregion P9
					#region P10
					case "P10SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(10,startProc);
						break;
					case "P10Date":
						displayStrings[i]=GetProcInfo("Date",10+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P10Area":
						displayStrings[i]=GetProcInfo("Area",10+startProc);
						break;
					case "P10System":
						displayStrings[i]=GetProcInfo("System",10+startProc);
						break;
					case "P10ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",10+startProc);
						break;
					case "P10Surface":
						displayStrings[i]=GetProcInfo("Surface",10+startProc);
						break;
					case "P10Code":
						displayStrings[i]=GetProcInfo("Code",10+startProc);
						break;
					case "P10Description":
						displayStrings[i]=GetProcInfo("Desc",10+startProc);
						break;
					case "P10Fee":
						displayStrings[i]=GetProcInfo("Fee",10+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P10TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",10+startProc);
						break;
					case "P10PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",10+startProc);
						break;
					case "P10Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",10+startProc);
						break;
					case "P10DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",10+startProc);
						break;
					case "P10Lab":
						displayStrings[i]=GetProcInfo("Lab",10+startProc);
						break;
					case "P10FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",10+startProc);
						break;
					case "P10ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",10+startProc);
						break;
					case "P10TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",10+startProc);
						break;
					case "P10RevCode":
						displayStrings[i]=GetProcInfo("RevCode",10+startProc);
						break;
					case "P10CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",10+startProc);
						break;
					case "P10CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",10+startProc);
						break;
					case "P10CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",10+startProc);
						break;
					case "P10CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",10+startProc);
						break;
					case "P10Minutes":
						displayStrings[i]=GetProcInfo("Minutes",10+startProc);
						break;
					case "P10UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",10 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",10 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",10+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",10+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P10UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",10+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(10,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P10CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",10+startProc) 
							+ GetProcInfo("CodeMod1",10+startProc) 
							+ GetProcInfo("CodeMod2",10+startProc) 
							+ GetProcInfo("CodeMod3",10+startProc) 
							+ GetProcInfo("CodeMod4",10+startProc);
						break;
					case "P10IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",10+startProc);
						break;
					case "P10eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",10+startProc);
						break;
					#endregion P10
					#region P11
					case "P11SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(11,startProc);
						break;
					case "P11Date":
						displayStrings[i]=GetProcInfo("Date",11+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P11Area":
						displayStrings[i]=GetProcInfo("Area",11+startProc);
						break;
					case "P11System":
						displayStrings[i]=GetProcInfo("System",11+startProc);
						break;
					case "P11ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",11+startProc);
						break;
					case "P11Surface":
						displayStrings[i]=GetProcInfo("Surface",11+startProc);
						break;
					case "P11Code":
						displayStrings[i]=GetProcInfo("Code",11+startProc);
						break;
					case "P11Description":
						displayStrings[i]=GetProcInfo("Desc",11+startProc);
						break;
					case "P11Fee":
						displayStrings[i]=GetProcInfo("Fee",11+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P11TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",11+startProc);
						break;
					case "P11PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",11+startProc);
						break;
					case "P11Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",11+startProc);
						break;
					case "P11DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",11+startProc);
						break;
					case "P11Lab":
						displayStrings[i]=GetProcInfo("Lab",11+startProc);
						break;
					case "P11FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",11+startProc);
						break;
					case "P11ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",11+startProc);
						break;
					case "P11TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",11+startProc);
						break;
					case "P11RevCode":
						displayStrings[i]=GetProcInfo("RevCode",11+startProc);
						break;
					case "P11CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",11+startProc);
						break;
					case "P11CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",11+startProc);
						break;
					case "P11CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",11+startProc);
						break;
					case "P11CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",11+startProc);
						break;
					case "P11Minutes":
						displayStrings[i]=GetProcInfo("Minutes",11+startProc);
						break;
					case "P11UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",11 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",11 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",11+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",11+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P11UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",11+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(11,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P11CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",11+startProc)
							+ GetProcInfo("CodeMod1",11+startProc)
							+ GetProcInfo("CodeMod2",11+startProc)
							+ GetProcInfo("CodeMod3",11+startProc)
							+ GetProcInfo("CodeMod4",11+startProc);
						break;
					case "P11IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",11+startProc);
						break;
					case "P11eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",11+startProc);
						break;
					#endregion P11
					#region P12
					case "P12SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(12,startProc);
						break;
					case "P12Date":
						displayStrings[i]=GetProcInfo("Date",12+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P12Area":
						displayStrings[i]=GetProcInfo("Area",12+startProc);
						break;
					case "P12System":
						displayStrings[i]=GetProcInfo("System",12+startProc);
						break;
					case "P12ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",12+startProc);
						break;
					case "P12Surface":
						displayStrings[i]=GetProcInfo("Surface",12+startProc);
						break;
					case "P12Code":
						displayStrings[i]=GetProcInfo("Code",12+startProc);
						break;
					case "P12Description":
						displayStrings[i]=GetProcInfo("Desc",12+startProc);
						break;
					case "P12Fee":
						displayStrings[i]=GetProcInfo("Fee",12+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P12TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",12+startProc);
						break;
					case "P12PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",12+startProc);
						break;
					case "P12Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",12+startProc);
						break;
					case "P12DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",12+startProc);
						break;
					case "P12Lab":
						displayStrings[i]=GetProcInfo("Lab",12+startProc);
						break;
					case "P12FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",12+startProc);
						break;
					case "P12ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",12+startProc);
						break;
					case "P12TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",12+startProc);
						break;
					case "P12RevCode":
						displayStrings[i]=GetProcInfo("RevCode",12+startProc);
						break;
					case "P12CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",12+startProc);
						break;
					case "P12CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",12+startProc);
						break;
					case "P12CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",12+startProc);
						break;
					case "P12CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",12+startProc);
						break;
					case "P12Minutes":
						displayStrings[i]=GetProcInfo("Minutes",12+startProc);
						break;
					case "P12UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",12 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",12 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",12+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",12+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P12UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",12+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(12,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P12CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",12+startProc)
							+ GetProcInfo("CodeMod1",12+startProc)
							+ GetProcInfo("CodeMod2",12+startProc)
							+ GetProcInfo("CodeMod3",12+startProc)
							+ GetProcInfo("CodeMod4",12+startProc);
						break;
					case "P12IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",12+startProc);
						break;
					case "P12eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",12+startProc);
						break;
					#endregion P12
					#region P13
					case "P13SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(13,startProc);
						break;
					case "P13Date":
						displayStrings[i]=GetProcInfo("Date",13+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P13Area":
						displayStrings[i]=GetProcInfo("Area",13+startProc);
						break;
					case "P13System":
						displayStrings[i]=GetProcInfo("System",13+startProc);
						break;
					case "P13ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",13+startProc);
						break;
					case "P13Surface":
						displayStrings[i]=GetProcInfo("Surface",13+startProc);
						break;
					case "P13Code":
						displayStrings[i]=GetProcInfo("Code",13+startProc);
						break;
					case "P13Description":
						displayStrings[i]=GetProcInfo("Desc",13+startProc);
						break;
					case "P13Fee":
						displayStrings[i]=GetProcInfo("Fee",13+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P13TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",13+startProc);
						break;
					case "P13PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",13+startProc);
						break;
					case "P13Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",13+startProc);
						break;
					case "P13DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",13+startProc);
						break;
					case "P13Lab":
						displayStrings[i]=GetProcInfo("Lab",13+startProc);
						break;
					case "P13FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",13+startProc);
						break;
					case "P13ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",13+startProc);
						break;
					case "P13TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",13+startProc);
						break;
					case "P13RevCode":
						displayStrings[i]=GetProcInfo("RevCode",13+startProc);
						break;
					case "P13CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",13+startProc);
						break;
					case "P13CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",13+startProc);
						break;
					case "P13CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",13+startProc);
						break;
					case "P13CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",13+startProc);
						break;
					case "P13Minutes":
						displayStrings[i]=GetProcInfo("Minutes",13+startProc);
						break;
					case "P13UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",13 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",13 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",13+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",13+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P13UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",13+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(13,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P13CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",13+startProc)
							+ GetProcInfo("CodeMod1",13+startProc)
							+ GetProcInfo("CodeMod2",13+startProc)
							+ GetProcInfo("CodeMod3",13+startProc)
							+ GetProcInfo("CodeMod4",13+startProc);
						break;
					case "P13IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",13+startProc);
						break;
					case "P13eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",13+startProc);
						break;
					#endregion P13
					#region P14
					case "P14SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(14,startProc);
						break;
					case "P14Date":
						displayStrings[i]=GetProcInfo("Date",14+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P14Area":
						displayStrings[i]=GetProcInfo("Area",14+startProc);
						break;
					case "P14System":
						displayStrings[i]=GetProcInfo("System",14+startProc);
						break;
					case "P14ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",14+startProc);
						break;
					case "P14Surface":
						displayStrings[i]=GetProcInfo("Surface",14+startProc);
						break;
					case "P14Code":
						displayStrings[i]=GetProcInfo("Code",14+startProc);
						break;
					case "P14Description":
						displayStrings[i]=GetProcInfo("Desc",14+startProc);
						break;
					case "P14Fee":
						displayStrings[i]=GetProcInfo("Fee",14+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P14TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",14+startProc);
						break;
					case "P14PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",14+startProc);
						break;
					case "P14Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",14+startProc);
						break;
					case "P14DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",14+startProc);
						break;
					case "P14Lab":
						displayStrings[i]=GetProcInfo("Lab",14+startProc);
						break;
					case "P14FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",14+startProc);
						break;
					case "P14ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",14+startProc);
						break;
					case "P14TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",14+startProc);
						break;
					case "P14RevCode":
						displayStrings[i]=GetProcInfo("RevCode",14+startProc);
						break;
					case "P14CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",14+startProc);
						break;
					case "P14CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",14+startProc);
						break;
					case "P14CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",14+startProc);
						break;
					case "P14CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",14+startProc);
						break;
					case "P14Minutes":
						displayStrings[i]=GetProcInfo("Minutes",14+startProc);
						break;
					case "P14UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",14 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",14 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",14+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",14+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P14UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",14+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(14,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P14CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",14+startProc)
							+ GetProcInfo("CodeMod1",14+startProc)
							+ GetProcInfo("CodeMod2",14+startProc)
							+ GetProcInfo("CodeMod3",14+startProc)
							+ GetProcInfo("CodeMod4",14+startProc);
						break;
					case "P14IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",14+startProc);
						break;
					case "P14eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",14+startProc);
						break;
					#endregion P14
					#region P15
					case "P15SystemAndTeeth":
						displayStrings[i]=GenerateSystemAndTeethField(15,startProc);
						break;
					case "P15Date":
						displayStrings[i]=GetProcInfo("Date",15+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P15Area":
						displayStrings[i]=GetProcInfo("Area",15+startProc);
						break;
					case "P15System":
						displayStrings[i]=GetProcInfo("System",15+startProc);
						break;
					case "P15ToothNumber":
						displayStrings[i]=GetProcInfo("ToothNum",15+startProc);
						break;
					case "P15Surface":
						displayStrings[i]=GetProcInfo("Surface",15+startProc);
						break;
					case "P15Code":
						displayStrings[i]=GetProcInfo("Code",15+startProc);
						break;
					case "P15Description":
						displayStrings[i]=GetProcInfo("Desc",15+startProc);
						break;
					case "P15Fee":
						displayStrings[i]=GetProcInfo("Fee",15+startProc,ClaimFormCur.Items[i].FormatString);
						break;
					case "P15TreatDentMedicaidID":
						displayStrings[i]=GetProcInfo("TreatDentMedicaidID",15+startProc);
						break;
					case "P15PlaceNumericCode":
						displayStrings[i]=GetProcInfo("PlaceNumericCode",15+startProc);
						break;
					case "P15Diagnosis":
						displayStrings[i]=GetProcInfo("Diagnosis",15+startProc);
						break;
					case "P15DiagnosisPoint":
						displayStrings[i]=GetProcInfo("DiagnosisPoint",15+startProc);
						break;
					case "P15Lab":
						displayStrings[i]=GetProcInfo("Lab",15+startProc);
						break;
					case "P15FeeMinusLab":
						displayStrings[i]=GetProcInfo("FeeMinusLab",15+startProc);
						break;
					case "P15ToothNumOrArea":
						displayStrings[i]=GetProcInfo("ToothNumOrArea",15+startProc);
						break;
					case "P15TreatProvNPI":
						displayStrings[i]=GetProcInfo("TreatProvNPI",15+startProc);
						break;
					case "P15RevCode":
						displayStrings[i]=GetProcInfo("RevCode",15+startProc);
						break;
					case "P15CodeMod1":
						displayStrings[i]=GetProcInfo("CodeMod1",15+startProc);
						break;
					case "P15CodeMod2":
						displayStrings[i]=GetProcInfo("CodeMod2",15+startProc);
						break;
					case "P15CodeMod3":
						displayStrings[i]=GetProcInfo("CodeMod3",15+startProc);
						break;
					case "P15CodeMod4":
						displayStrings[i]=GetProcInfo("CodeMod4",15+startProc);
						break;
					case "P15Minutes":
						displayStrings[i]=GetProcInfo("Minutes",15+startProc);
						break;
					case "P15UnitQty":
						if(planCur.ShowBaseUnits) {
							short bunit;
							System.Int16.TryParse(GetProcInfo("BaseUnits",15 + startProc),out bunit);
							short uqty;
							System.Int16.TryParse(GetProcInfo("UnitQty",15 + startProc),out uqty);
							qty = bunit + uqty;
						}
						else if(GetProcInfo("UnitQty",15+startProc)!="") {
							qty=Int16.Parse(GetProcInfo("UnitQty",15+startProc));
						}
						else {
							qty=0;
						}
						if(qty==0) {
							displayStrings[i]="";
						}
						else {
							displayStrings[i]=qty.ToString();
						}
						break;
					case "P15UnitQtyOrCount":
						toothCount=GetToothRangeCount(GetProcInfo("ToothNum",15+startProc));
						if(toothCount==-1) {//No toothrange specified
							displayStrings[i]=CalculateUnitQtyField(15,startProc,planCur);
						}
						else {
							displayStrings[i]=toothCount.ToString();
						}
						break;
					case "P15CodeAndMods":
						displayStrings[i]=GetProcInfo("Code",15+startProc)
							+ GetProcInfo("CodeMod1",15+startProc)
							+ GetProcInfo("CodeMod2",15+startProc)
							+ GetProcInfo("CodeMod3",15+startProc)
							+ GetProcInfo("CodeMod4",15+startProc);
						break;
					case "P15IsEmergency":
						displayStrings[i]=GetProcInfo("IsEmergency",15+startProc);
						break;
					case "P15eClaimNote":
						displayStrings[i]=GetProcInfo("eClaimNote",15+startProc);
						break;
					#endregion P15
					case "TotalFee":
						decimal fee=0;//fee only for this page. Each page is treated like a separate claim.
						for(int f=startProc;f<startProc+totProcs;f++) {//eg f=0;f<10;f++
							if(f < ListClaimProcs.Count) {
								fee+=(decimal)((ClaimProc)ListClaimProcs[f]).FeeBilled;
								if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
									List<Procedure> labProcs=Procedures.GetCanadianLabFees(ListClaimProcs[f].ProcNum,ListProc);
									for(int j=0;j<labProcs.Count;j++) {
										fee+=(decimal)labProcs[j].ProcFee;
									}
								}
							}
						}
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=fee.ToString("F");
						}
						else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
							displayStrings[i] = fee.ToString("F").Replace("."," ");
						}
						else {
							displayStrings[i]=fee.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
					case "DateService":
						if(ClaimFormCur.Items[i].FormatString=="") {
							displayStrings[i]=ClaimCur.DateService.ToShortDateString();
						}
						else {
							displayStrings[i]=ClaimCur.DateService.ToString(ClaimFormCur.Items[i].FormatString);
						}
						break;
				}//switch
				if(CultureInfo.CurrentCulture.Name=="nl-BE" && displayStrings[i]==""){//Dutch Belgium
					displayStrings[i]="*   *   *";
				}
			}//for i
		}

		private void FillMedValueCodes(){
			ListClaimValCodeLog = ClaimValCodeLogs.GetForClaim(ClaimCur.ClaimNum);
			if(ListClaimValCodeLog.Count>0){
				ClaimValCodeLog[] vcA;
				vcA = new ClaimValCodeLog[12];
				for(int i=0;i<ListClaimValCodeLog.Count;i++){
					vcA[i]=(ClaimValCodeLog)ListClaimValCodeLog[i];
				}
				for(int i=ListClaimValCodeLog.Count;i<12;i++){
					vcA[i]= new ClaimValCodeLog();
				}
				for(int i=0;i<ClaimFormCur.Items.Count;i++){
					switch(ClaimFormCur.Items[i].FieldName){
						case "MedValCode39a":
							displayStrings[i]=vcA[0].ValCode;
							break;
						case "MedValAmount39a":
							if(vcA[0].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i]=vcA[0].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[0].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39b":
							if(vcA[3]!=null)
								displayStrings[i]=vcA[3].ValCode;
							break;
						case "MedValAmount39b":
							if(vcA[3].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[3].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[3].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39c":
							if(vcA[6]!=null)
								displayStrings[i]=vcA[6].ValCode;
							break;
						case "MedValAmount39c":
							if(vcA[6].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[6].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[6].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode39d":
							if(vcA[9]!=null)
								displayStrings[i]=vcA[9].ValCode;
							break;
						case "MedValAmount39d":
							if(vcA[9].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[9].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[9].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40a":
							if(vcA[1]!=null) {
								displayStrings[i]=vcA[1].ValCode;
							}
							break;
						case "MedValAmount40a":
							if(vcA[1].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[1].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[1].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40b":
							if(vcA[4]!=null) {
								displayStrings[i]=vcA[4].ValCode;
							}
							break;
						case "MedValAmount40b":
							if(vcA[4].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[4].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[4].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40c":
							if(vcA[7]!=null) {
								displayStrings[i]=vcA[7].ValCode;
							}
							break;
						case "MedValAmount40c":
							if(vcA[7].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[7].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[7].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode40d":
							if(vcA[10]!=null)
								displayStrings[i]=vcA[10].ValCode;
							break;
						case "MedValAmount40d":
							if(vcA[10].ValAmount==0){
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[10].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[10].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41a":
							if(vcA[2]!=null)
								displayStrings[i]=vcA[2].ValCode;
							break;
						case "MedValAmount41a":
							if(vcA[2].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[2].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[2].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41b":
							if(vcA[5]!=null) {
								displayStrings[i]=vcA[5].ValCode;
							}
							break;
						case "MedValAmount41b":
							if(vcA[5].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[5].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[5].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41c":
							if(vcA[8]!=null) {
								displayStrings[i]=vcA[8].ValCode;
							}
							break;
						case "MedValAmount41c":
							if(vcA[8].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[8].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[8].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
						case "MedValCode41d":
							if(vcA[11]!=null)
								displayStrings[i]=vcA[11].ValCode;
							break;
						case "MedValAmount41d":
							if(vcA[11].ValAmount==0) {
								displayStrings[i]="";
							}
							else if(ClaimFormCur.Items[i].FormatString == "NoDec") {
								displayStrings[i] = vcA[11].ValAmount.ToString("F").Replace("."," ");
							}
							else {
								displayStrings[i]=vcA[11].ValAmount.ToString(ClaimFormCur.Items[i].FormatString);
							}
							break;
					}
				}
			}
		}

		private void FillMedCondCodes(){
			ClaimCondCodeLog claimCondCodeLog=ClaimCondCodeLogs.GetByClaimNum(ClaimCur.ClaimNum);
			if(claimCondCodeLog==null) {
				return;
			}
			for(int i=0;i<ClaimFormCur.Items.Count;i++) {
				switch(ClaimFormCur.Items[i].FieldName) {
					case "MedConditionCode18":
						displayStrings[i]=claimCondCodeLog.Code0;
						break;
					case "MedConditionCode19":
						displayStrings[i]=claimCondCodeLog.Code1;
						break;
					case "MedConditionCode20":
						displayStrings[i]=claimCondCodeLog.Code2;
						break;
					case "MedConditionCode21":
						displayStrings[i]=claimCondCodeLog.Code3;
						break;
					case "MedConditionCode22":
						displayStrings[i]=claimCondCodeLog.Code4;
						break;
					case "MedConditionCode23":
						displayStrings[i]=claimCondCodeLog.Code5;
						break;
					case "MedConditionCode24":
						displayStrings[i]=claimCondCodeLog.Code6;
						break;
					case "MedConditionCode25":
						displayStrings[i]=claimCondCodeLog.Code7;
						break;
					case "MedConditionCode26":
						displayStrings[i]=claimCondCodeLog.Code8;
						break;
					case "MedConditionCode27":
						displayStrings[i]=claimCondCodeLog.Code9;
						break;
					case "MedConditionCode28":
						displayStrings[i]=claimCondCodeLog.Code10;
						break;
				}
			}
		}

		///<summary>These fields are used for the UB04.</summary>
		private void FillMedInsStrings(){
			PatPlan patPlan=null;
			for(int i=0;i<ListPatPlans.Count;i++) {
				if(ListPatPlans[i].InsSubNum==ClaimCur.InsSubNum) {
					patPlan=ListPatPlans[i];
					break;
				}
			}
			string insFilingCodeStr=InsFilingCodes.GetEclaimCode(planCur.FilingCode);
			//Determine the slot (A, B or C) where the medical insurance information belongs.
			string insLine="A";
			if(insFilingCodeStr=="MC") { //Medicaid
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
			for(int i=0;i<ClaimProcsForClaim.Count;i++) {
				for(int j=0;j<ClaimProcsForPat.Count;j++) {
					if(!ClaimProcs.IsValidClaimAdj(ClaimProcsForPat[j],ClaimProcsForClaim[i].ProcNum,ClaimProcsForClaim[i].InsSubNum)) {
						continue;
					}
					if(insFilingCodeStr=="MC") { //Medicaid
						string insFilingCodeClaimProcStr="";
						for(int k=0;k<ListInsPlan.Count;k++) {
							if(ListInsPlan[k].PlanNum==ClaimProcsForPat[j].PlanNum) {
								insFilingCodeClaimProcStr=InsFilingCodes.GetEclaimCode(ListInsPlan[k].FilingCode);
								break;
							}
						}
						if(insFilingCodeClaimProcStr=="16" || insFilingCodeClaimProcStr=="MB") {//HMO_MedicareRisk and MedicarePartB
							priorPaymentsA+=(decimal)ClaimProcsForPat[j].InsPayAmt;
						}
						else {
							priorPaymentsB+=(decimal)ClaimProcsForPat[j].InsPayAmt;
						}
					}
					else {
						for(int k=0;k<ListPatPlans.Count;k++) {
							if(ListPatPlans[k].InsSubNum==ClaimProcsForPat[j].InsSubNum) {
								if(ListPatPlans[k].Ordinal==1) {
									priorPaymentsA+=(decimal)ClaimProcsForPat[j].InsPayAmt;
								}
								else if(ListPatPlans[k].Ordinal==2) {
									priorPaymentsB+=(decimal)ClaimProcsForPat[j].InsPayAmt;
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
					if(insFilingCodeStr=="MB") { //Medicare Part B
						displayStrings[i]="XOVR";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Name") { //MedInsAName, MedInsBName, MedInsCName
					displayStrings[i]=Carriers.GetName(planCur.CarrierNum);
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"PlanID") { //MedInsAPlanID, MedInsBPlanID, MedInsCPlanID
					//Not used. Leave blank.
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"RelInfo") { //MedInsARelInfo, MedInsBRelInfo, MedInsCRelInfo
					displayStrings[i]=subCur.ReleaseInfo?"X":"";
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AssignBen") { //MedInsAAssignBen, MedInsBAssignBen, MedInsCAssignBen
					displayStrings[i]=subCur.AssignBen?"X":"";
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedInsAPriorPmt") {
					if(priorPaymentsA==0) {
						continue;
					}
					if(stringFormat=="") {
						displayStrings[i]=priorPaymentsA.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						displayStrings[i]=priorPaymentsA.ToString("F").Replace("."," ");
					}
					else {
						displayStrings[i]=priorPaymentsA.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedInsBPriorPmt") {
					if(priorPaymentsB==0) {
						continue;
					}
					if(stringFormat=="") {
						displayStrings[i]=priorPaymentsB.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						displayStrings[i]=priorPaymentsB.ToString("F").Replace("."," ");
					}
					else {
						displayStrings[i]=priorPaymentsB.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AmtDue") { //MedInsAAmtDue, MedInsBAmtDue, MedInsCAmtDue
					decimal totalValAmount=(decimal)ClaimValCodeLogs.GetValAmountTotal(ClaimCur.ClaimNum,"23");
					decimal amtDue=((decimal)ClaimCur.ClaimFee-priorPaymentsA-priorPaymentsB-totalValAmount);
					if(stringFormat=="") {
						displayStrings[i]=amtDue.ToString("F");
					}
					else if(stringFormat=="NoDec") {
						displayStrings[i]=amtDue.ToString("F").Replace("."," ");
					}
					else {
						displayStrings[i]=amtDue.ToString(stringFormat);
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"OtherProvID") { //MedInsAOtherProvID, MedInsBOtherProvID, MedInsCOtherProvID
					string CarrierElectID=carrier.ElectID.ToString();
					Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
					Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==ClaimCur.ProvBill)??providerFirst;
					if(prov.ProvNum>0 && CarrierElectID!="" && ProviderIdents.GetForPayor(prov.ProvNum,CarrierElectID).Length>0) {
						ProviderIdent provID=ProviderIdents.GetForPayor(prov.ProvNum,CarrierElectID)[0];
						if(provID.IDNumber != "") {
							displayStrings[i]=provID.IDNumber.ToString();
						}
						else {
							displayStrings[i] = "";
						}
					}
					else {
						displayStrings[i]="";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"InsuredName") { //MedInsAInsuredName, MedInsBInsuredName, MedInsCInsuredName
					displayStrings[i]=Patients.GetPat(subCur.Subscriber).GetNameFLFormal();
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Relation") { //MedInsARelation, MedInsBRelation, MedInsCRelation
					if(ClaimCur.PatRelat==Relat.Spouse) {
						displayStrings[i]="01";
					}
					else if(ClaimCur.PatRelat==Relat.Self) {
						displayStrings[i]="18";
					}
					else if(ClaimCur.PatRelat==Relat.Child) {
						displayStrings[i]="19";
					}
					else if(ClaimCur.PatRelat==Relat.Employee) {
						displayStrings[i]="20";
					}
					else if(ClaimCur.PatRelat==Relat.LifePartner) {
						displayStrings[i]="53";
					}
					else {
						displayStrings[i]="G8";
					}
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"InsuredID") { //MedInsAInsuredID, MedInsBInsuredID, MedInsCInsuredID
					displayStrings[i]=subCur.SubscriberID;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"GroupName") { //MedInsAGroupName, MedInsBGroupName, MedInsCGroupName
					displayStrings[i]=planCur.GroupName;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"GroupNum") { //MedInsAGroupNum, MedInsBGroupNum, MedInsCGroupNum
					displayStrings[i]=planCur.GroupNum;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"AuthCode") { //MedInsAAuthCode, MedInsBAuthCode, MedInsCAuthCode
					displayStrings[i]=ClaimCur.PreAuthString;
				}
				else if(ClaimFormCur.Items[i].FieldName=="MedIns"+insLine+"Employer") { //MedInsAEmployer, MedInsBEmployer, MedInsCEmployer
					displayStrings[i]=Employers.GetName(planCur.EmployerNum);
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
			if(ListClaimProcs.Count <= procIndex){
				return "";
			}
			Procedure ProcCur=Procedures.GetProcFromList(ListProc,ListClaimProcs[procIndex].ProcNum);
			ProcedureCode procCode=ProcedureCodes.GetProcCode(ProcCur.CodeNum);
			switch(field) {
				case "System":
					return "JP";
				case "Code":
					return ListClaimProcs[procIndex].CodeSent;
				case "Fee":
					decimal totalProcFees=(decimal)ListClaimProcs[procIndex].FeeBilled;
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						List<Procedure> labProcs=Procedures.GetCanadianLabFees(ListClaimProcs[procIndex].ProcNum,ListProc);
						for(int i=0;i<labProcs.Count;i++) {
							totalProcFees+=(decimal)labProcs[i].ProcFee;
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
					return ProcCur.RevCode;
				case "CodeMod1":
					return ProcCur.CodeMod1;
				case "CodeMod2":
					return ProcCur.CodeMod2;
				case "CodeMod3":
					return ProcCur.CodeMod3;
				case "CodeMod4":
					return ProcCur.CodeMod4;
				case "Minutes":
					if(ProcCur.ProcTime==TimeSpan.Zero || ProcCur.ProcTimeEnd==TimeSpan.Zero) {
						return "";
					}
					return POut.Int((int)(ProcCur.ProcTimeEnd-ProcCur.ProcTime).TotalMinutes);
				case "UnitQty":
					return ProcCur.UnitQty.ToString();
				case "BaseUnits":
					return ProcCur.BaseUnits.ToString();
				case "Desc":
					if(procCode.DrugNDC!="") {
						//For UB04, we must show the procedure description as a standard drug format so that the drug can be easily recognized.
						//The DrugNDC field is only used when medical features are turned on so this behavior won't take effect in many circumstances.
						string drugUnit="UN";//Unit
						float drugQty=ProcCur.DrugQty;
						switch(ProcCur.DrugUnit) {
							case EnumProcDrugUnit.Gram:
								drugUnit="GR";
								break;
							case EnumProcDrugUnit.InternationalUnit:
								drugUnit="F2";
								break;
							case EnumProcDrugUnit.Milligram:
								drugUnit="GR";
								drugQty=drugQty/1000;
								break;
							case EnumProcDrugUnit.Milliliter:
								drugUnit="ML";
								break;
						}
						return "N4"+procCode.DrugNDC+drugUnit+drugQty.ToString("f3");
					}
					else {
						ProcedureCode procCodeSent=ProcedureCodes.GetProcCode(ListClaimProcs[procIndex].CodeSent);
						string descript=Procedures.GetClaimDescript(ListClaimProcs[procIndex],procCodeSent,ProcCur,procCode,planCur);
						if(procCodeSent.TreatArea==TreatmentArea.Quad) {
							return ProcCur.Surf+" "+descript;
						}
						else {
							return descript;
						}
					}
				case "Date":
					if(ClaimCur.ClaimType=="PreAuth") {//no date on preauth procedures
						return "";
					}
					if(stringFormat=="") {
						return ListClaimProcs[procIndex].ProcDate.ToShortDateString();
					}
					return ListClaimProcs[procIndex].ProcDate.ToString(stringFormat);
				case "TreatDentMedicaidID":
					if(ListClaimProcs[procIndex].ProvNum==0) {
						return "";
					}
					else {
						Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
						Provider providerClaimProc=Providers.GetFirstOrDefault(x => x.ProvNum==ListClaimProcs[procIndex].ProvNum)??providerFirst;
						return providerClaimProc.MedicaidID;
					}
				case "TreatProvNPI":
					if(ListClaimProcs[procIndex].ProvNum==0) {
						return "";
					}
					else {
						Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
						Provider providerClaimProc=Providers.GetFirstOrDefault(x => x.ProvNum==ListClaimProcs[procIndex].ProvNum)??providerFirst;
						return providerClaimProc.NationalProvID;
					}
				case "PlaceNumericCode":
					return X12object.GetPlaceService(ClaimCur.PlaceService);
				case "Diagnosis":
					if(ProcCur.DiagnosticCode==""){
						return "";
					}
					for(int d=0;d<diagnoses.Length;d++){
						if(diagnoses[d]==ProcCur.DiagnosticCode){
							return (d+1).ToString();
						}
					}
					return ProcCur.DiagnosticCode;
				case "DiagnosisPoint":
					if(ProcCur.DiagnosticCode=="") {
						return "";//No diagnosis codes present on procedure.
					}
					string pointer="";//Output will be in alphabetical order.
					for(int d=0;d<arrayDiagnoses.Length;d++) {
						if(arrayDiagnoses[d]=="") {
							continue;
						}
						if(arrayDiagnoses[d]==ProcCur.DiagnosticCode || arrayDiagnoses[d]==ProcCur.DiagnosticCode2 ||
							arrayDiagnoses[d]==ProcCur.DiagnosticCode3 || arrayDiagnoses[d]==ProcCur.DiagnosticCode4) 
						{
							pointer+=(Char)(d+(int)('A'));//Characters A through L.
						}
					}
					return pointer;
				case "Lab":
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						List<Procedure> labProcs=Procedures.GetCanadianLabFees(ListClaimProcs[procIndex].ProcNum,ListProc);
						decimal totalLabFees=0;
						for(int i=0;i<labProcs.Count;i++) {
							totalLabFees+=(decimal)labProcs[i].ProcFee;
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
							return ListClaimProcs[procIndex].FeeBilled.ToString("F");
						}
						else if(stringFormat=="NoDec") {
							double amt = ListClaimProcs[procIndex].FeeBilled * 100;
							return amt.ToString();
						}
						else {
							return ListClaimProcs[procIndex].FeeBilled.ToString(stringFormat);
						}
					}
					return "";//(((ClaimProc)claimprocs[procIndex]).FeeBilled-ProcCur.LabFee).ToString("n");
				case "IsEmergency":
					if(ProcCur.Urgency==ProcUrgency.Emergency) {
						return "Y";
					}
					return "";
				case "eClaimNote":
					return ProcCur.ClaimNote;
				default:
					break;
			}
			string area="";
			string toothNum="";
			string surf="";
			switch(ProcedureCodes.GetProcCode(ProcCur.CodeNum).TreatArea){
				case TreatmentArea.Surf:
					//area blank
					toothNum=Tooth.ToInternat(ProcCur.ToothNum);
					surf=Tooth.SurfTidyForClaims(ProcCur.Surf,ProcCur.ToothNum);
					break;
				case TreatmentArea.Tooth:
					//area blank
					toothNum=Tooth.ToInternat(ProcCur.ToothNum);
					//surf blank
					break;
				case TreatmentArea.Quad:
					area=AreaToCode(ProcCur.Surf);//"UL" etc -> 20 etc
					//num blank
					//surf blank
					break;
				case TreatmentArea.Sextant:
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						//United States Sextant 1 is Canadian sextant 03.
						//United States Sextant 2 is Canadian sextant 04.
						//United States Sextant 3 is Canadian sextant 05.
						//United States Sextant 4 is Canadian sextant 06.
						//United States Sextant 5 is Canadian sextant 07.
						//United States Sextant 6 is Canadian sextant 08.
						//The sextant goes into the "International Tooth Code" column on the claim form, according to the Nova Scotia NIHB fee guide page VII.
						toothNum=(PIn.Int(ProcCur.Surf)+2).ToString().PadLeft(2,'0');//Add 2 to US sextant, then prepend a '0'.
					}
					else {//United States
						area="";//leave it blank.  Never used anyway.
						//area="S"+ProcCur.Surf;//area
						//num blank
						//surf blank
					}
					break;
				case TreatmentArea.Arch:
					area=AreaToCode(ProcCur.Surf);//area "U", etc
					//num blank
					//surf blank
					break;
				case TreatmentArea.ToothRange:
					//area blank
					toothNum=Tooth.FormatRangeForDisplay(ProcCur.ToothRange);
					/*for(int i=0;i<ProcCur.ToothRange.Split(',').Length;i++){
						if(!Tooth.IsValidDB(ProcCur.ToothRange.Split(',')[i])){
							continue;
						}
						if(i>0){
							toothNum+=",";
						}
						toothNum+=Tooth.ToInternat(ProcCur.ToothRange.Split(',')[i]);
					}*/
					//surf blank
					break;
				default://mouth
					//area?
					break;
			}//switch treatarea
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
				FormClaimPrint FormCP=new FormClaimPrint();
				FormCP.PatNumCur=claim.PatNum;
				FormCP.ClaimNumCur=claim.ClaimNum;
				FormCP.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
				if(FormCP.PrintImmediate(Lan.g(nameof(PrinterL),"CDA claim form printed"),PrintSituation.Claim,claim.PatNum)) {
					Etranss.SetClaimSentOrPrinted(claim.ClaimNum,claim.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				}
			}
			catch {
				//Oh well, the user can manually reprint if needed.
			}
		}

		//Todo: Find a better place to put this method.
		public static void ShowCdaClaimForm(Claim claim) {
			using FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.PatNumCur=claim.PatNum;
			FormCP.ClaimNumCur=claim.ClaimNum;
			FormCP.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
			FormCP.ShowDialog();
		}

		///<summary>This method returns -1 if a toothrange was not specified, otherwise returns the count of teeth in the toothrange.</summary>
		private int GetToothRangeCount(string toothNums) {
			List<string> listToothNums=RemoveToothRangeFormat(toothNums);
			if(listToothNums.Count<=1) {//Proc had 0 or 1 toothNum specified
				return -1;
			}
			else {//The procedure has a toothrange, return the number of teeth.
				return listToothNums.Count;
			}
		}

		///<summary>Returns the procedures UnitQty fields.</summary>
		private string CalculateUnitQtyField(int index,int startProc,InsPlan insPlan) {
			int qty;
			if(insPlan.ShowBaseUnits) {
				short bunit;
				System.Int16.TryParse(GetProcInfo("BaseUnits",index+startProc),out bunit);
				short uqty;
				System.Int16.TryParse(GetProcInfo("UnitQty",index+startProc),out uqty);
				qty=bunit+uqty;
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
			string toothNum=GetProcInfo("ToothNum",index+startProc);
			if(String.IsNullOrEmpty(toothNum)) {
				string area=GetProcInfo("Area",index+startProc);
				if(string.IsNullOrEmpty(area)) {
					return "";//Procedure did not specify a toothnum or area
				}
				//JO="ANSI/ADA/ISO Specification No. 3950-1984 Dentistry Designation System for Tooth and Areas of the Oral Cavity" (v. 02-12 1500 claim form)
				return "JO"+area;
			}
			List<string> listToothNums=RemoveToothRangeFormat(toothNum);
			//loop through the list and construct the field
			return "JP"+String.Join(" ",listToothNums);//JP="Universal/National Tooth Designation System" (v. 02-12 1500 claim form)
		}

		///<summary>In GetProcInfo() if the treament area is a ToothRange, the resulting string is formatted with a '-'.
		///E.g.- Input:"1-7", returns "1,2,3,4,5,6,7" in a list.
		///This format will not work with the 1500 claim form per the instruction manual.
		///"Do not enter a space between the qualifier and the number/code/information. Do not enter hyphens or spaces within the number/code".
		///Obligatory "Beware ye who enter here".</summary>
		private List<string> RemoveToothRangeFormat(string toothRange) {
			//There may be multiple toothranges formatted with a '-'. E.g. 1-5,20-25
			//Each must be broken apart and added to the return value
			string[] arrayToothRanges=toothRange.Split(',');
			//List of toothNums
			List<string> retVal=new List<string>();
			foreach(string toothRangeVal in arrayToothRanges) {
				if(toothRangeVal.Contains("-")) {
					string[] arrayRange=toothRangeVal.Split('-');
					int start=PIn.Int(arrayRange[0]);
					int end=PIn.Int(arrayRange[1]);
					//Create a list of ints given the starting number and total count of ints needed. Then comma delimit the list and add it to the return value.
					retVal.AddRange(Enumerable.Range(start,(end-start)+1).Select(x => x.ToString()));
				}
				else if(toothRangeVal.Trim()!="") {
					retVal.Add(toothRangeVal);
				}
			}
			return retVal;
		}

		private void butBack_Click(object sender, System.EventArgs e){
			if(Preview2.StartPage==0) { 
				return; 
			}
			Preview2.StartPage--;
			labelTotPages.Text=(Preview2.StartPage+1).ToString() + " / " + totalPages.ToString();
		}

		private void butFwd_Click(object sender, System.EventArgs e){
			if(Preview2.StartPage==totalPages-1) {
				return;
			}
			Preview2.StartPage++;
			labelTotPages.Text=(Preview2.StartPage+1).ToString() + " / " + totalPages.ToString();
		}

		private void butPrint_Click(object sender, System.EventArgs e){
			if(PrintClaim()){
				Etranss.SetClaimSentOrPrinted(ClaimNumCur,ClaimCur.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				//Claims.UpdateStatus(ThisClaimNum,"P");
				SecurityLogs.MakeLogEntry(Permissions.ClaimSend,ClaimCur.PatNum,Lan.g(this,"Claim printed from Claim Preview window."),
					ClaimCur.ClaimNum,ClaimCur.SecDateTEdit);
				DialogResult=DialogResult.OK;
			}
			else{
				DialogResult=DialogResult.Cancel;
			}
			//Close();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void FormClaimPrint_FormClosing(object sender,FormClosingEventArgs e) {
			if(!butPrint.Enabled) {//if the claim has been deleted and the print button has been disabled.
				DialogResult=DialogResult.Abort;
			}
		}
	}

}
















