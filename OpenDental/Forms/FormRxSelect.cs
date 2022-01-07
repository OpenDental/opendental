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
		private Patient PatCur;
		///<summary>A list of all RxDefs.  Filled on load and on butRefresh click.</summary>
		private RxDef[] _arrayRxDefs;

		/// <summary>This is set for any medical orders that are selected.</summary>
		public long _medOrderNum;

		///<summary></summary>
		public FormRxSelect(Patient patCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			PatCur=patCur;
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
				string[] arraySearchTerms=textDrug.Text.Split(' ');
				foreach(string searchTerm in arraySearchTerms) {
					arrayRxDefs=arrayRxDefs.Where(x => x.Drug.ToLower().Contains(searchTerm.ToLower())).ToArray();
				}
			}
			if(textDisp.Text!="") {
				string[] arraySearchTerms=textDisp.Text.Split(' ');
				foreach(string searchTerm in arraySearchTerms) {
					arrayRxDefs=arrayRxDefs.Where(x => x.Disp.ToLower().Contains(searchTerm.ToLower())).ToArray();
				}
			}
			if(checkControlledOnly.Checked) {
				arrayRxDefs=arrayRxDefs.Where(x => x.IsControlled).ToArray();
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxSetup","Drug"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Controlled"),70,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Sig"),250);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Disp"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Refills"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Notes"),300);
			gridMain.ListGridColumns.Add(col);
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
			RxDef RxDefCur=(RxDef)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			if(PrefC.GetBool(PrefName.ShowFeatureEhr) && RxDefCur.RxCui==0) {
				string strMsgText=Lan.g(this,"The selected prescription is missing an RxNorm")+".\r\n"
					+Lan.g(this,"Prescriptions without RxNorms cannot be exported in EHR documents")+".\r\n";
				if(!Security.IsAuthorized(Permissions.RxEdit,true)) {
					//Show the message but don't allow to edit. Continue creating rx
					MessageBox.Show(strMsgText);
				}
				else if(MessageBox.Show(strMsgText+Lan.g(this,"Edit RxNorm in Rx Template?"),"",MessageBoxButtons.YesNo)==DialogResult.Yes) {
					using FormRxDefEdit form=new FormRxDefEdit(RxDefCur);
					form.ShowDialog();
					RxDefCur=RxDefs.GetOne(RxDefCur.RxDefNum);//FormRxDefEdit does not modify the RxDefCur object, so we must get the updated RxCui from the db.
				}
			}
			if(RxDefCur==null) {//Can occur if the RxDef is deleted. Refresh list and fill grid
				_arrayRxDefs=RxDefs.Refresh();
				FillGrid();
				return;
			}
			//Alert
			if(!RxAlertL.DisplayAlerts(PatCur.PatNum,RxDefCur.RxDefNum)){
				return;
			}
			//User OK with alert
			RxPat RxPatCur=new RxPat();
			RxPatCur.RxDate=DateTime.Today;
			RxPatCur.PatNum=PatCur.PatNum;
			RxPatCur.ClinicNum=PatCur.ClinicNum;
			RxPatCur.Drug=RxDefCur.Drug;
			RxPatCur.IsControlled=RxDefCur.IsControlled;
			if(PrefC.GetBool(PrefName.RxHasProc) && (Clinics.ClinicNum==0 || Clinics.GetClinic(Clinics.ClinicNum).HasProcOnRx)) {
				RxPatCur.IsProcRequired=RxDefCur.IsProcRequired;
			}
			RxPatCur.Sig=RxDefCur.Sig;
			RxPatCur.Disp=RxDefCur.Disp;
			RxPatCur.Refills=RxDefCur.Refills;
			if(PrefC.GetBool(PrefName.RxSendNewToQueue)) {
				RxPatCur.SendStatus=RxSendStatus.InElectQueue;
			}
			else {
				RxPatCur.SendStatus=RxSendStatus.Unsent;
			}
			RxPatCur.PatientInstruction=RxDefCur.PatientInstruction;
			//Notes not copied: we don't want these kinds of notes cluttering things
			using FormRxEdit FormE=new FormRxEdit(PatCur,RxPatCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			bool isProvOrder=false;
			if(Security.CurUser.ProvNum!=0) {//The user who is currently logged in is a provider.
				isProvOrder=true;
			}
			_medOrderNum=MedicationPats.InsertOrUpdateMedOrderForRx(RxPatCur,RxDefCur.RxCui,isProvOrder);//RxDefCur.RxCui can be 0.
			EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.EventType=EhrMeasureEventType.CPOE_MedOrdered;
			newMeasureEvent.PatNum=PatCur.PatNum;
			newMeasureEvent.MoreInfo="";
			newMeasureEvent.FKey=_medOrderNum;
			EhrMeasureEvents.Insert(newMeasureEvent);
			DialogResult=DialogResult.OK;
		}

		private void butBlank_Click(object sender, System.EventArgs e) {
			RxPat RxPatCur=new RxPat();
			RxPatCur.RxDate=DateTime.Today;
			RxPatCur.PatNum=PatCur.PatNum;
			RxPatCur.ClinicNum=PatCur.ClinicNum;
			if(PrefC.GetBool(PrefName.RxSendNewToQueue)) {
				RxPatCur.SendStatus=RxSendStatus.InElectQueue;
			}
			else {
				RxPatCur.SendStatus=RxSendStatus.Unsent;
			}
			using FormRxEdit FormE=new FormRxEdit(PatCur,RxPatCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			//We do not need to make a medical order here, because butBlank is not visible in EHR mode.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
					MsgBox.Show(this,"Please select Rx first.");
				}
				else {
					MsgBox.Show(this,"Please select Rx first or click Blank");
				}
				return;
			}
			RxSelected();
		}

	}
}
