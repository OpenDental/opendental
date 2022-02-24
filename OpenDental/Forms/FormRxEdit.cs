using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;

namespace OpenDental{
///<summary></summary>
	public partial class FormRxEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
    public FormRpPrintPreview pView = new FormRpPrintPreview();
		private Patient PatCur;
		//private User user;
		private RxPat RxPatCur;
		///<summary>If the Rx has already been printed, this will contain the archived sheet. The print button will be not visible, and the view button will be visible.</summary>
		private Sheet sheet;
		private RxPat _rxPatOld;
		private List <Procedure> _listInUseProcs;

		///<summary></summary>
		public FormRxEdit(Patient patCur,RxPat rxPatCur){
			//){//
			InitializeComponent();
			InitializeLayoutManager();
			RxPatCur=rxPatCur;
			PatCur=patCur;
			Lan.F(this);
		}

		private void FormRxEdit_Load(object sender, System.EventArgs e) {
			RxPatCur.IsNew=IsNew;
			_rxPatOld=RxPatCur.Copy();
			if(IsNew){
				butAudit.Visible=false;
				butView.Visible=false;
				labelView.Visible=false;
				sheet=null;
				Provider provUser=Providers.GetProv(Security.CurUser.ProvNum);
				if(provUser!=null && !provUser.IsSecondary) {//Only set the provider on the Rx if the provider is not a hygienist.
					RxPatCur.ProvNum=Security.CurUser.ProvNum;
					if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Is CPOE
						labelCPOE.Visible=true;
						comboProv.Enabled=false;
						butPickProv.Enabled=false;
					}
				}
			}
			else {
				sheet=Sheets.GetRx(RxPatCur.PatNum,RxPatCur.RxNum);
				if(sheet==null){
					butView.Visible=false;
					labelView.Visible=false;
				}
				else{
					butPrint.Visible=false;
				}
				if(!Security.IsAuthorized(Permissions.RxEdit)) {
					textDate.Enabled=false;
					checkControlled.Enabled=false;
					checkProcRequired.Enabled=false;
					comboProcCode.Enabled=false;
					textDaysOfSupply.Enabled=false;
					textDrug.Enabled=false;
					textSig.Enabled=false;
					textDisp.Enabled=false;
					textRefills.Enabled=false;
					textPatInstructions.Enabled=false;
					comboProv.Enabled=false;
					butPickProv.Enabled=false;
					textDosageCode.Enabled=false;
					textNotes.Enabled=false;
					butPick.Enabled=false;
					comboSendStatus.Enabled=false;
					butDelete.Enabled=false;
					comboClinic.Enabled=false;
					butOK.Enabled=false;
				}
			}
			//security is handled on the Rx button click in the Chart module
			textDate.Text=RxPatCur.RxDate.ToString("d");
			checkControlled.Checked=RxPatCur.IsControlled;
			comboProcCode.Items.Clear();
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				checkProcRequired.Checked=RxPatCur.IsProcRequired;
				comboProcCode.Items.Add(Lan.g(this,"none"));
				comboProcCode.SelectedIndex=0;
				List <ProcedureCode> listProcCodes=ProcedureCodes.GetListDeep();
				DateTime rxDate=PIn.Date(textDate.Text);
				if(rxDate.Year < 1880) {
					rxDate=DateTime.Today;
				}
				_listInUseProcs=Procedures.Refresh(RxPatCur.PatNum)
					.FindAll(x => x.ProcNum==RxPatCur.ProcNum
						|| (x.ProcStatus==ProcStat.C && x.DateComplete <= rxDate && x.DateComplete >= rxDate.AddYears(-1))
						|| x.ProcStatus==ProcStat.TP)
					.OrderBy(x => x.ProcStatus.ToString())
					.ThenBy(x => (x.ProcStatus==ProcStat.C)?x.DateComplete:x.DateTP)
					.ToList();
				foreach(Procedure proc in _listInUseProcs) {
					ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum,listProcCodes);
					string itemText=Lan.g("enumProcStat",proc.ProcStatus.ToString());
					if(ProcMultiVisits.IsProcInProcess(proc.ProcNum)) {
						itemText=Lan.g("enumProcStat",ProcStatExt.InProcess);
					}
					if(proc.ProcStatus==ProcStat.C) {
						itemText+=" "+proc.DateComplete.ToShortDateString();
					}
					else {
						itemText+=" "+proc.DateTP.ToShortDateString();
					}
					itemText+=" "+Procedures.GetDescription(proc);
					comboProcCode.Items.Add(itemText);
					if(proc.ProcNum==RxPatCur.ProcNum) {
						comboProcCode.SelectedIndex=comboProcCode.Items.Count-1;
					}
				}
				if(RxPatCur.DaysOfSupply!=0) {
					textDaysOfSupply.Text=POut.Double(RxPatCur.DaysOfSupply);
				}
			}
			else {
				checkProcRequired.Enabled=false;
				labelProcedure.Enabled=false;
				comboProcCode.Enabled=false;
				labelDaysOfSupply.Enabled=false;
				textDaysOfSupply.Enabled=false;
			}
			for(int i=0;i<Enum.GetNames(typeof(RxSendStatus)).Length;i++) {
				comboSendStatus.Items.Add(Enum.GetNames(typeof(RxSendStatus))[i]);
			}
			comboSendStatus.SelectedIndex=(int)RxPatCur.SendStatus;
			textDrug.Text=RxPatCur.Drug;
			textSig.Text=RxPatCur.Sig;
			textDisp.Text=RxPatCur.Disp;
			textRefills.Text=RxPatCur.Refills;
			textPatInstructions.Text=RxPatCur.PatientInstruction;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)){
				textDosageCode.Text=RxPatCur.DosageCode;
			}
			else{
				labelDosageCode.Visible=false;
				textDosageCode.Visible=false;
			}
			textNotes.Text=RxPatCur.Notes;
			textPharmInfo.Text=RxPatCur.ErxPharmacyInfo;
			textPharmacy.Text=Pharmacies.GetDescription(RxPatCur.PharmacyNum);
			comboClinic.SelectedClinicNum=RxPatCur.ClinicNum;
			FillComboProvNum();
		}

		///<summary>Fill the provider combobox with items depending on the clinic selected</summary>
		private void FillComboProvNum() {
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			if(RxPatCur.ProvNum==0) {//If new Rx									 
				if(comboProv.Items.Count==0) {//No items in dropdown
					//Use clinic default prov.  We need some default selection.
					comboProv.SetSelectedProvNum(Providers.GetDefaultProvider(comboClinic.SelectedClinicNum).ProvNum);
				}
				else {
					comboProv.SetSelected(0);//First in list
				}
			}
			else {
				comboProv.SetSelectedProvNum(RxPatCur.ProvNum);//Use prov from Rx
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			FormPP.SelectedProvNum=comboProv.GetSelectedProvNum();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormPP.SelectedProvNum);
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormPharmacies FormP=new FormPharmacies();
			FormP.IsSelectionMode=true;
			FormP.SelectedPharmacyNum=RxPatCur.PharmacyNum;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			RxPatCur.PharmacyNum=FormP.SelectedPharmacyNum;
			textPharmacy.Text=Pharmacies.GetDescription(RxPatCur.PharmacyNum);
		}

		private void butAudit_Click(object sender,EventArgs e) {
			List<Permissions> perms=new List<Permissions>();
			perms.Add(Permissions.RxCreate);
			perms.Add(Permissions.RxEdit);
			using FormAuditOneType FormA=new FormAuditOneType(RxPatCur.PatNum,perms,Lan.g(this,"Audit Trail for Rx"),RxPatCur.RxNum);
			FormA.ShowDialog();
		}

		///<summary>Attempts to save, returning true if successful.</summary>
		private bool SaveRx(){
			if(!textDate.IsValid() 
				|| (textDaysOfSupply.Text!="" && !textDaysOfSupply.IsValid())) 
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			long selectedProvNum=comboProv.GetSelectedProvNum();
			if(selectedProvNum==0) {//should not happen
				MessageBox.Show(Lan.g(this,"Invalid provider."));
				return false;
			}
			//Prevents prescriptions from being added that have a provider selected that is past their term date
			List<long> listInvalidProvNums=Providers.GetInvalidProvsByTermDate(new List<long> { selectedProvNum },DateTime.Now);
			if(listInvalidProvNums.Count > 0) {
				MsgBox.Show(this,"The provider selected has a Term Date prior to today. Please select another provider.");
				return false;
			}
			RxPatCur.ProvNum=selectedProvNum;
			RxPatCur.RxDate=PIn.Date(textDate.Text);
			RxPatCur.Drug=textDrug.Text;
			RxPatCur.IsControlled=checkControlled.Checked;
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				RxPatCur.IsProcRequired=checkProcRequired.Checked;
				if(comboProcCode.SelectedIndex==0) {//none
					RxPatCur.ProcNum=0;
				}
				else {
					RxPatCur.ProcNum=_listInUseProcs[comboProcCode.SelectedIndex-1].ProcNum;
				}
				RxPatCur.DaysOfSupply=PIn.Double(textDaysOfSupply.Text);
			}
			RxPatCur.Sig=textSig.Text;
			RxPatCur.Disp=textDisp.Text;
			RxPatCur.Refills=textRefills.Text;
			RxPatCur.DosageCode=textDosageCode.Text;
			RxPatCur.Notes=textNotes.Text;
			RxPatCur.SendStatus=(RxSendStatus)comboSendStatus.SelectedIndex;
			RxPatCur.PatientInstruction=textPatInstructions.Text;
			RxPatCur.ClinicNum=(comboClinic.SelectedClinicNum==-1 ? RxPatCur.ClinicNum : comboClinic.SelectedClinicNum);//If no selection, don't change the ClinicNum
			// hook for additional authorization before prescription is saved
			bool[] authorized=new bool[1] { false };
			if(Plugins.HookMethod(this,"FormRxEdit.SaveRx_Authorize",authorized,Providers.GetProv(selectedProvNum),RxPatCur,_rxPatOld)) {
				if(!authorized[0]) {
					return false;
				}
			}
			//pharmacy is set when using pick button.
			if(IsNew){
				RxPatCur.RxNum=RxPats.Insert(RxPatCur);
				SecurityLogs.MakeLogEntry(Permissions.RxCreate,RxPatCur.PatNum,"CREATED("+RxPatCur.RxDate.ToShortDateString()+","+RxPatCur.Drug+","
					+RxPatCur.ProvNum+","+RxPatCur.Disp+","+RxPatCur.Refills+")",RxPatCur.RxNum,DateTime.MinValue);//No date previous needed, new Rx Pat
				if(FormProcGroup.IsOpen){
					FormProcGroup.RxNum=RxPatCur.RxNum;
				}
			}
			else{
				if(RxPats.Update(RxPatCur,_rxPatOld)) {
					//The rx has changed, make an edit entry.
					SecurityLogs.MakeLogEntry(Permissions.RxEdit,RxPatCur.PatNum,"FROM("+_rxPatOld.RxDate.ToShortDateString()+","+_rxPatOld.Drug+","
						+_rxPatOld.ProvNum+","+_rxPatOld.Disp+","+_rxPatOld.Refills+")"+"\r\nTO("+RxPatCur.RxDate.ToShortDateString()+","+RxPatCur.Drug+","
						+RxPatCur.ProvNum+","+RxPatCur.Disp+","+RxPatCur.Refills+")",RxPatCur.RxNum,_rxPatOld.DateTStamp);
				}
			}
			//If there is not a link for the current PharmClinic combo, make one.
			if(Pharmacies.GetOne(RxPatCur.PharmacyNum)!=null && PharmClinics.GetOneForPharmacyAndClinic(RxPatCur.PharmacyNum,RxPatCur.ClinicNum)==null) {
				PharmClinics.Insert(new PharmClinic(RxPatCur.PharmacyNum,RxPatCur.ClinicNum));
			}
			IsNew=false;//so that we can save it again after printing if needed.
			return true;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Prescription?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RxEdit,RxPatCur.PatNum,"FROM("+_rxPatOld.RxDate.ToShortDateString()+","+_rxPatOld.Drug+","+_rxPatOld.ProvNum+","+_rxPatOld.Disp+","+_rxPatOld.Refills+")"+"\r\nTO('deleted')",RxPatCur.RxNum,_rxPatOld.DateTStamp);
			RxPats.Delete(RxPatCur.RxNum);
			DialogResult=DialogResult.OK;	
		}
		
		private void butPrintPatInstructions_Click(object sender,EventArgs e) {
			PrintRx(true);
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!PrintRx(false)) {
				return;
			}
			if(RxPatCur.IsNew) {
				AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),PatCur.PatNum,0,new List<RxPat>() { RxPatCur });
			}
			DialogResult=DialogResult.OK;
		}

		///<summary>Prints the prescription.  Returns true if printing was successful.  Otherwise; displays an error message and returns false.</summary>
		private bool PrintRx(bool isInstructions) {
			if(PrinterSettings.InstalledPrinters.Count==0) {
				MsgBox.Show(this,"Error: No Printers Installed\r\n"+
									"If you do have a printer installed, restarting the workstation may solve the problem."
				);
				return false;
			}
			if(!isInstructions) {
				//only visible if sheet==null.
				if(comboSendStatus.SelectedIndex==(int)RxSendStatus.InElectQueue
					|| comboSendStatus.SelectedIndex==(int)RxSendStatus.SentElect) {
					//do not change status
				}
				else {
					comboSendStatus.SelectedIndex=(int)RxSendStatus.Printed;
				}
			}
			if(!SaveRx()){
				return false;
			}
			//This logic is an exact copy of FormRxManage.butPrintSelect_Click()'s logic when 1 Rx is selected.  
			//If this is updated, that method needs to be updated as well.
			SheetDef sheetDef;
			if(isInstructions) {
				sheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.RxInstruction);
			}
			else {
				sheetDef=SheetDefs.GetSheetsDefault(SheetTypeEnum.Rx,Clinics.ClinicNum);
			}
			sheet=SheetUtil.CreateSheet(sheetDef,PatCur.PatNum);
			SheetParameter.SetParameter(sheet,"RxNum",RxPatCur.RxNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			if(!SheetPrinting.PrintRx(sheet,RxPatCur)) {
				return false;
			}
			return true;
		}

		private void butView_Click(object sender,EventArgs e) {
			//only visible if there is already a sheet.
			if(!SaveRx()){
				return;
			}
			SheetFields.GetFieldsAndParameters(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			//When the clinic changes we need to refill the provider combo for the new clinic.
			FillComboProvNum();
		}

		///<summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK) { //if user clicked cancel, then we can just stay in this form.
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!SaveRx()){
				return;
			}
			if(RxPatCur.IsNew) {
				AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),PatCur.PatNum,0,new List<RxPat>() { RxPatCur });
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}
