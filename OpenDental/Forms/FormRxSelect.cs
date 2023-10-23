using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRxSelect : FormODBase {
		private Patient _patient;
		///<summary>A list of all RxDefs.  Filled on load and on butRefresh click.</summary>
		private RxDef[] _arrayRxDefs;

		/// <summary>This is set for any medical orders that are selected.</summary>
		public long MedOrderNum;

		///<summary></summary>
		public FormRxSelect(Patient patient){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patient=patient;
			Lan.F(this);
		}

		private void FormRxSelect_Load(object sender, System.EventArgs e) {
			_arrayRxDefs=RxDefs.Refresh();
			FillGrid();
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				//We cannot allow blank prescription when using EHR, because each prescription created in this window must have an RxCui.
				//If we allowed blank, we would not know where to pull the RxCui from.
				butBlank.Visible=false;
				labelInstructions.Text=Lan.g(this,"Please select a Prescription from the list.");
			}
		}

		private void FillGrid() {
			RxDef[] arrayRxDefs=_arrayRxDefs;
			if(textDrug.Text!="") {
				string[] stringArraySearchTerms=textDrug.Text.Split(' ');
				for(int i=0;i<stringArraySearchTerms.Length;i++) {
					arrayRxDefs=arrayRxDefs.Where(x => x.Drug.ToLower().Contains(stringArraySearchTerms[i].ToLower())).ToArray();
				}
			}
			if(textDisp.Text!="") {
				string[] stringArraySearchTerms=textDisp.Text.Split(' ');
				for(int i=0;i<stringArraySearchTerms.Length;i++) {
					arrayRxDefs=arrayRxDefs.Where(x => x.Disp.ToLower().Contains(stringArraySearchTerms[i].ToLower())).ToArray();
				}
			}
			if(checkControlledOnly.Checked) {
				arrayRxDefs=arrayRxDefs.Where(x => x.IsControlled).ToArray();
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxSetup","Drug"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Controlled"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Sig"),250);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Disp"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Refills"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Notes"),300);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<arrayRxDefs.Length;i++) {
				row=new GridRow();
				row.Cells.Add(arrayRxDefs[i].Drug);
				if(arrayRxDefs[i].IsControlled){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(arrayRxDefs[i].Sig);
				row.Cells.Add(arrayRxDefs[i].Disp);
				row.Cells.Add(arrayRxDefs[i].Refills);
				row.Cells.Add(arrayRxDefs[i].Notes);
				gridMain.ListGridRows.Add(row);
				row.Tag=arrayRxDefs[i];
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			RxSelected();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_arrayRxDefs=RxDefs.Refresh();
			FillGrid();
		}

		private void textDrug_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Enter) {
				return;
			}
			e.Handled=true;
			e.SuppressKeyPress=true;
			FillGrid();
		}

		private void textDisp_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Enter) {
				return;
			}
			e.Handled=true;
			e.SuppressKeyPress=true;
			FillGrid();
		}

		private void RxSelected(){
			if(gridMain.GetSelectedIndex()==-1) {
				//this should never happen
				return;
			}
			RxDef rxDef=(RxDef)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr) && rxDef.RxCui==0) {
				string strMsgText=Lan.g(this,"The selected prescription is missing an RxNorm")+".\r\n"
					+Lan.g(this,"Prescriptions without RxNorms cannot be exported in EHR documents")+".\r\n";
				if(!Security.IsAuthorized(EnumPermType.RxEdit,true)) {
					//Show the message but don't allow to edit. Continue creating rx
					MessageBox.Show(strMsgText);
				}
				else if(MessageBox.Show(strMsgText+Lan.g(this,"Edit RxNorm in Rx Template?"),"",MessageBoxButtons.YesNo)==DialogResult.Yes) {
					using FormRxDefEdit formRxDefEdit=new FormRxDefEdit(rxDef);
					formRxDefEdit.ShowDialog();
					rxDef=RxDefs.GetOne(rxDef.RxDefNum);//FormRxDefEdit does not modify the RxDefCur object, so we must get the updated RxCui from the db.
				}
			}
			if(rxDef==null) {//Can occur if the RxDef is deleted. Refresh list and fill grid
				_arrayRxDefs=RxDefs.Refresh();
				FillGrid();
				return;
			}
			//Alert
			if(!RxAlertL.DisplayAlerts(_patient.PatNum,rxDef.RxDefNum)){
				return;
			}
			//User OK with alert
			RxPat rxPat=new RxPat();
			rxPat.RxDate=DateTime.Today;
			rxPat.PatNum=_patient.PatNum;
			rxPat.ClinicNum=_patient.ClinicNum;
			rxPat.Drug=rxDef.Drug;
			rxPat.IsControlled=rxDef.IsControlled;
			if(PrefC.GetBool(PrefName.RxHasProc) && (Clinics.ClinicNum==0 || Clinics.GetClinic(Clinics.ClinicNum).HasProcOnRx)) {
				rxPat.IsProcRequired=rxDef.IsProcRequired;
			}
			rxPat.Sig=rxDef.Sig;
			rxPat.Disp=rxDef.Disp;
			rxPat.Refills=rxDef.Refills;
			if(PrefC.GetBool(PrefName.RxSendNewToQueue)) {
				rxPat.SendStatus=RxSendStatus.InElectQueue;
			}
			else {
				rxPat.SendStatus=RxSendStatus.Unsent;
			}
			rxPat.PatientInstruction=rxDef.PatientInstruction;
			//Notes not copied: we don't want these kinds of notes cluttering things
			using FormRxEdit formRxEdit=new FormRxEdit(_patient,rxPat);
			formRxEdit.IsNew=true;
			formRxEdit.ShowDialog();
			if(formRxEdit.DialogResult!=DialogResult.OK){
				return;
			}
			bool isProvOrder=false;
			if(Security.CurUser.ProvNum!=0) {//The user who is currently logged in is a provider.
				isProvOrder=true;
			}
			MedOrderNum=MedicationPats.InsertOrUpdateMedOrderForRx(rxPat,rxDef.RxCui,isProvOrder);//RxDefCur.RxCui can be 0.
			EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=DateTime.Now;
			ehrMeasureEvent.EventType=EhrMeasureEventType.CPOE_MedOrdered;
			ehrMeasureEvent.PatNum=_patient.PatNum;
			ehrMeasureEvent.MoreInfo="";
			ehrMeasureEvent.FKey=MedOrderNum;
			EhrMeasureEvents.Insert(ehrMeasureEvent);
			DialogResult=DialogResult.OK;
		}

		private void butBlank_Click(object sender, System.EventArgs e) {
			RxPat rxPat=new RxPat();
			rxPat.RxDate=DateTime.Today;
			rxPat.PatNum=_patient.PatNum;
			rxPat.ClinicNum=_patient.ClinicNum;
			if(PrefC.GetBool(PrefName.RxSendNewToQueue)) {
				rxPat.SendStatus=RxSendStatus.InElectQueue;
			}
			else {
				rxPat.SendStatus=RxSendStatus.Unsent;
			}
			using FormRxEdit formRxEdit=new FormRxEdit(_patient,rxPat);
			formRxEdit.IsNew=true;
			formRxEdit.ShowDialog();
			if(formRxEdit.DialogResult!=DialogResult.OK){
				return;
			}
			//We do not need to make a medical order here, because butBlank is not visible in EHR mode.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()!=-1){ 
				RxSelected();
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				MsgBox.Show(this,"Please select Rx first.");
				return;
			}
			MsgBox.Show(this,"Please select Rx first or click Blank");
			return;
		}
	}
}
