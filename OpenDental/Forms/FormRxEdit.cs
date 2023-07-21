using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormRxEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		public FormRpPrintPreview FormRpPrintPreview_ = new FormRpPrintPreview();
		private Patient _patient;
		//private User user;
		private RxPat _rxPat;
		///<summary>If the Rx has already been printed, this will contain the archived sheet. The print button will be not visible, and the view button will be visible.</summary>
		private Sheet _sheet;
		private RxPat _rxPatOld;
		private List <Procedure> _listProceduresInUse;
		private Provider _userProvider=null;

		///<summary></summary>
		public FormRxEdit(Patient patient,RxPat rxPat){
			//){//
			InitializeComponent();
			InitializeLayoutManager();
			_rxPat=rxPat;
			_patient=patient;
			Lan.F(this);
		}

		private void FormRxEdit_Load(object sender, System.EventArgs e) {
			_rxPat.IsNew=IsNew;
			_rxPatOld=_rxPat.Copy();
			if(IsNew){
				butAudit.Visible=false;
				butView.Visible=false;
				labelView.Visible=false;
				_sheet=null;
				_userProvider=Providers.GetProv(Security.CurUser.ProvNum);
				comboClinic.SelectedClinicNum=_patient.ClinicNum;
				if(PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)){
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
				if(_userProvider!=null && !_userProvider.IsSecondary) {//Only set the provider on the Rx if the provider is not a hygienist.
					if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {//Is CPOE
						labelCPOE.Visible=true;
						comboProv.Enabled=false;
						butPickProv.Enabled=false;
					}
				}
			}
			else {
				_sheet=Sheets.GetRx(_rxPat.PatNum,_rxPat.RxNum);
				comboClinic.SelectedClinicNum=_rxPat.ClinicNum;
				if(_sheet==null){
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
			textDate.Text=_rxPat.RxDate.ToString("d");
			checkControlled.Checked=_rxPat.IsControlled;
			comboProcCode.Items.Clear();
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				checkProcRequired.Checked=_rxPat.IsProcRequired;
				comboProcCode.Items.Add(Lan.g(this,"none"));
				comboProcCode.SelectedIndex=0;
				List <ProcedureCode> listProcedureCodes=ProcedureCodes.GetListDeep();
				DateTime dateRx=PIn.Date(textDate.Text);
				if(dateRx.Year < 1880) {
					dateRx=DateTime.Today;
				}
				_listProceduresInUse=Procedures.Refresh(_rxPat.PatNum)
					.FindAll(x => x.ProcNum==_rxPat.ProcNum
						|| (x.ProcStatus==ProcStat.C && x.DateComplete <= dateRx && x.DateComplete >= dateRx.AddYears(-1))
						|| x.ProcStatus==ProcStat.TP)
					.OrderBy(x => x.ProcStatus.ToString())
					.ThenBy(x => (x.ProcStatus==ProcStat.C)?x.DateComplete:x.DateTP)
					.ToList();
				for(int i=0;i<_listProceduresInUse.Count;i++) {
					ProcedureCode procedureCode = ProcedureCodes.GetProcCode(_listProceduresInUse[i].CodeNum,listProcedureCodes);
					string itemText=Lan.g("enumProcStat",_listProceduresInUse[i].ProcStatus.ToString());
					if(ProcMultiVisits.IsProcInProcess(_listProceduresInUse[i].ProcNum)) {
						itemText=Lan.g("enumProcStat",ProcStatExt.InProcess);
					}
					if(_listProceduresInUse[i].ProcStatus==ProcStat.C) {
						itemText+=" "+_listProceduresInUse[i].DateComplete.ToShortDateString();
					}
					else {
						itemText+=" "+_listProceduresInUse[i].DateTP.ToShortDateString();
					}
					itemText+=" "+Procedures.GetDescription(_listProceduresInUse[i]);
					comboProcCode.Items.Add(itemText);
					if(_listProceduresInUse[i].ProcNum==_rxPat.ProcNum) {
						comboProcCode.SelectedIndex=comboProcCode.Items.Count-1;
					}
				}
				if(_rxPat.DaysOfSupply!=0) {
					textDaysOfSupply.Text=POut.Double(_rxPat.DaysOfSupply);
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
			comboSendStatus.SelectedIndex=(int)_rxPat.SendStatus;
			textDrug.Text=_rxPat.Drug;
			textSig.Text=_rxPat.Sig;
			textDisp.Text=_rxPat.Disp;
			textRefills.Text=_rxPat.Refills;
			textPatInstructions.Text=_rxPat.PatientInstruction;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)){
				textDosageCode.Text=_rxPat.DosageCode;
			}
			else{
				labelDosageCode.Visible=false;
				textDosageCode.Visible=false;
			}
			textNotes.Text=_rxPat.Notes;
			textPharmInfo.Text=_rxPat.ErxPharmacyInfo;
			textPharmacy.Text=Pharmacies.GetDescription(_rxPat.PharmacyNum);
			FillComboProvNum();
			if(comboProv.Items.Count==0) {
				MsgBox.Show(this,"No providers available from clinic with a DEA#");
				DialogResult=DialogResult.Cancel;
			}
		}

		///<summary>Fill the provider combobox with items depending on the clinic selected</summary>
		private void FillComboProvNum() { 
			comboProv.Items.Clear();
			List<Provider> listProviders = Providers.GetProvsForClinicList(new List<long>{0, comboClinic.SelectedClinicNum}).Where(x => !x.IsNotPerson).ToList();
			if(_userProvider!=null && !listProviders.Any(x => x.ProvNum==_userProvider.ProvNum) && !_userProvider.IsNotPerson) {
				listProviders.Add(_userProvider);
			}
			if(PrefC.GetBool(PrefName.RxHideProvsWithoutDEA)) {
				List<long> listProvNums=listProviders.Select(x => x.ProvNum).ToList();
				List<long> listProviderNumsToKeep = ProviderClinics.GetByProvNumsAndClinicNum(listProvNums,comboClinic.SelectedClinicNum,includeUnsassigned:true)
				.Where(x => !x.DEANum.IsNullOrEmpty())
				.Select(x => x.ProvNum)
				.ToList();
				listProviders = listProviders.Where(x => listProviderNumsToKeep.Contains(x.ProvNum)).ToList();
				if(_rxPat.ProvNum != 0 && listProviders.FindIndex(x=> x.ProvNum==_rxPat.ProvNum) == -1) {//because List<T>.Contains can't manage to find a provider that is in the list
					Provider provider = Providers.GetProv(_rxPat.ProvNum);
					if(!provider.IsNotPerson) {
						listProviders.Add(provider);
					}
				}
			}
			comboProv.Items.AddProvsAbbr(listProviders);
			if(_rxPat.ProvNum==0) {//If new Rx 
				if(_userProvider!=null && listProviders.Any(x=>x.ProvNum==_userProvider.ProvNum)) {
					comboProv.SetSelectedProvNum(_userProvider.ProvNum);
				}
				else if(listProviders.Any(x=>x.ProvNum==_patient.PriProv)) {//Select the patient's primary provider, if possible.
					comboProv.SetSelectedProvNum(_patient.PriProv);
				}
				else if(listProviders.Count==1) {
					comboProv.SetSelectedProvNum(listProviders[0].ProvNum);
				}
				else {
					//no provider will be selected.  This will be caught at OK click.
				}
			}
			else {
				comboProv.SetSelectedProvNum(_rxPat.ProvNum);//Use prov from Rx
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formProviderPick.ProvNumSelected);
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormPharmacies formPharmacies=new FormPharmacies();
			formPharmacies.IsSelectionMode=true;
			formPharmacies.PharmacyNumSelected=_rxPat.PharmacyNum;
			formPharmacies.ShowDialog();
			if(formPharmacies.DialogResult!=DialogResult.OK){
				return;
			}
			_rxPat.PharmacyNum=formPharmacies.PharmacyNumSelected;
			textPharmacy.Text=Pharmacies.GetDescription(_rxPat.PharmacyNum);
		}

		private void butAudit_Click(object sender,EventArgs e) {
			List<Permissions> listPermissionss=new List<Permissions>();
			listPermissionss.Add(Permissions.RxCreate);
			listPermissionss.Add(Permissions.RxEdit);
			using FormAuditOneType formAuditOneType=new FormAuditOneType(_rxPat.PatNum,listPermissionss,Lan.g(this,"Audit Trail for Rx"),_rxPat.RxNum);
			formAuditOneType.ShowDialog();
		}

		///<summary>Attempts to save, returning true if successful.</summary>
		private bool SaveRx(){
			if(!textDate.IsValid() 
				|| (textDaysOfSupply.Text!="" && !textDaysOfSupply.IsValid())) 
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			long selectedProvNum=comboProv.GetSelectedProvNum();//zero if nothing selected
			if(selectedProvNum==0) {//Happens if the patient's primary provider is not in the list of providers or the logged in user is not a provider. 
				MessageBox.Show(Lan.g(this,"Please select a provider."));
				return false;
			}
			//Prevents prescriptions from being added that have a provider selected that is past their term date
			List<long> listInvalidProvNums=Providers.GetInvalidProvsByTermDate(new List<long> { selectedProvNum },DateTime.Now);
			if(listInvalidProvNums.Count > 0) {
				MsgBox.Show(this,"The provider selected has a Term Date prior to today. Please select another provider.");
				return false;
			}
			_rxPat.ProvNum=selectedProvNum;
			_rxPat.RxDate=PIn.Date(textDate.Text);
			_rxPat.Drug=textDrug.Text;
			_rxPat.IsControlled=checkControlled.Checked;
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				_rxPat.IsProcRequired=checkProcRequired.Checked;
				if(comboProcCode.SelectedIndex==0) {//none
					_rxPat.ProcNum=0;
				}
				else {
					_rxPat.ProcNum=_listProceduresInUse[comboProcCode.SelectedIndex-1].ProcNum;
				}
				_rxPat.DaysOfSupply=PIn.Double(textDaysOfSupply.Text);
			}
			_rxPat.Sig=textSig.Text;
			_rxPat.Disp=textDisp.Text;
			_rxPat.Refills=textRefills.Text;
			_rxPat.DosageCode=textDosageCode.Text;
			_rxPat.Notes=textNotes.Text;
			_rxPat.SendStatus=(RxSendStatus)comboSendStatus.SelectedIndex;
			_rxPat.PatientInstruction=textPatInstructions.Text;
			_rxPat.ClinicNum=(comboClinic.SelectedClinicNum==-1 ? _rxPat.ClinicNum : comboClinic.SelectedClinicNum);//If no selection, don't change the ClinicNum
			// hook for additional authorization before prescription is saved
			bool[] boolArrayAuthorized=new bool[1] { false };
			if(Plugins.HookMethod(this,"FormRxEdit.SaveRx_Authorize",boolArrayAuthorized,Providers.GetProv(selectedProvNum),_rxPat,_rxPatOld)) {
				if(!boolArrayAuthorized[0]) {
					return false;
				}
			}
			//pharmacy is set when using pick button.
			if(IsNew){
				_rxPat.RxNum=RxPats.Insert(_rxPat);
				SecurityLogs.MakeLogEntry(Permissions.RxCreate,_rxPat.PatNum,"CREATED("+_rxPat.RxDate.ToShortDateString()+","+_rxPat.Drug+",ProvNum:"
					+_rxPat.ProvNum+",Disp:"+_rxPat.Disp+",Refills:"+_rxPat.Refills+")",_rxPat.RxNum,DateTime.MinValue);//No date previous needed, new Rx Pat
				if(FormProcGroup.IsOpen){
					FormProcGroup.RxNum=_rxPat.RxNum;
				}
			}
			else{
				if(RxPats.Update(_rxPat,_rxPatOld)) {
					//The rx has changed, make an edit entry.
					SecurityLogs.MakeLogEntry(Permissions.RxEdit,_rxPat.PatNum,"FROM("+_rxPatOld.RxDate.ToShortDateString()+","+_rxPatOld.Drug+","
						+_rxPatOld.ProvNum+","+_rxPatOld.Disp+","+_rxPatOld.Refills+")"+"\r\nTO("+_rxPat.RxDate.ToShortDateString()+","+_rxPat.Drug+","
						+_rxPat.ProvNum+","+_rxPat.Disp+","+_rxPat.Refills+")",_rxPat.RxNum,_rxPatOld.DateTStamp);
				}
			}
			//If there is not a link for the current PharmClinic combo, make one.
			if(Pharmacies.GetOne(_rxPat.PharmacyNum)!=null && PharmClinics.GetOneForPharmacyAndClinic(_rxPat.PharmacyNum,_rxPat.ClinicNum)==null) {
				PharmClinics.Insert(new PharmClinic(_rxPat.PharmacyNum,_rxPat.ClinicNum));
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
			SecurityLogs.MakeLogEntry(Permissions.RxEdit,_rxPat.PatNum,"FROM("+_rxPatOld.RxDate.ToShortDateString()+","+_rxPatOld.Drug+","+_rxPatOld.ProvNum+","+_rxPatOld.Disp+","+_rxPatOld.Refills+")"+"\r\nTO('deleted')",_rxPat.RxNum,_rxPatOld.DateTStamp);
			RxPats.Delete(_rxPat.RxNum);
			DialogResult=DialogResult.OK;	
		}
		
		private void butPrintPatInstructions_Click(object sender,EventArgs e) {
			PrintRx(true);
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!PrintRx(false)) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RxEdit,_rxPat.PatNum,"Printed as: "+_rxPat.RxDate.ToShortDateString()+","+_rxPat.Drug+",ProvNum:"+_rxPat.ProvNum+",Disp:"+_rxPat.Disp+",Refills:"+_rxPat.Refills+")",_rxPat.RxNum, _rxPat.DateTStamp);
			if(_rxPat.IsNew) {
				AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),_patient.PatNum,0,new List<RxPat>() { _rxPat });
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
			_sheet=SheetUtil.CreateSheet(sheetDef,_patient.PatNum);
			SheetParameter.SetParameter(_sheet,"RxNum",_rxPat.RxNum);
			SheetFiller.FillFields(_sheet);
			SheetUtil.CalculateHeights(_sheet);
			if(!SheetPrinting.PrintRx(_sheet,_rxPat)) {
				return false;
			}
			return true;
		}

		private void butView_Click(object sender,EventArgs e) {
			//only visible if there is already a sheet.
			if(!SaveRx()){
				return;
			}
			SheetFields.GetFieldsAndParameters(_sheet);
			FormSheetFillEdit.ShowForm(_sheet,FormSheetFillEdit_FormClosing);
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
			if(_rxPat.IsNew) {
				AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),_patient.PatNum,0,new List<RxPat>() { _rxPat });
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}
