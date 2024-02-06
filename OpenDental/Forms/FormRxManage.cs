using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;

namespace OpenDental {
	public partial class FormRxManage:FormODBase {
		private UI.GridOD gridMain;
		private OpenDental.UI.Button butPrintSelected;
		private OpenDental.UI.Button butClose;
		private UI.Button butNewRx;
		private Label labelECWerror;
		private Patient _patient;
		private List<RxPat> _listRxPats;

		public FormRxManage(Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patCur;
			Lan.F(this);
		}

		private void FormRxManage_Load(object sender,System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxManage","Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Drug"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Sig"),70){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Disp"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Refills"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Provider"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Notes"),70){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxManage","Missing Info"),70){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			_listRxPats=RxPats.GetAllForPat(_patient.PatNum);
			_listRxPats.Sort(SortByRxDate);
			GridRow row;
			for(int i = 0;i<_listRxPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listRxPats[i].RxDate.ToShortDateString());
				row.Cells.Add(_listRxPats[i].Drug);
				row.Cells.Add(_listRxPats[i].Sig);
				row.Cells.Add(_listRxPats[i].Disp);
				row.Cells.Add(_listRxPats[i].Refills);
				row.Cells.Add(Providers.GetAbbr(_listRxPats[i].ProvNum));
				row.Cells.Add(_listRxPats[i].Notes);
				row.Cells.Add(SheetPrinting.ValidateRxForSheet(_listRxPats[i]));
				row.Tag=_listRxPats[i].Copy();
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Sorts the passed in RxPats by RxDate and then RxNum.</summary>
		private int SortByRxDate(RxPat rxPat1,RxPat rxPat2) {
			if(rxPat1.RxDate!=rxPat2.RxDate) {
				return rxPat2.RxDate.CompareTo(rxPat1.RxDate);
			}
			return rxPat2.RxNum.CompareTo(rxPat1.RxNum);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				//this should never happen
				return;
			}
			RxPat rxPat=_listRxPats[gridMain.GetSelectedIndex()];
			using FormRxEdit formRxEdit=new FormRxEdit(_patient,rxPat);
			formRxEdit.ShowDialog();
			if(formRxEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		///<summary>Prints the selected rx's. If one rx is selected, uses single rx sheet. If more than one is selected, uses multirx sheet</summary>
		private void butPrintSelect_Click(object sender,EventArgs e) {
			List<RxPat> listRxPats=new List<RxPat>();
			string messageSkipped="";
			SheetDef sheetDef;
			Sheet sheet;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				RxPat rxPat=_listRxPats[gridMain.SelectedIndices[i]];
				if(rxPat.ProvNum==0) { //skip any Rx without a provider
					messageSkipped+=rxPat.RxDate.ToShortDateString()+"    "+rxPat.Drug+"\r\n";
					continue;
				}
				listRxPats.Add(rxPat);
			}
			if(listRxPats.Count==0) {
				MsgBox.Show(this,"At least one prescription with a provider must be selected");
				return;
			}
			else if(!String.IsNullOrEmpty(messageSkipped)) {
				messageSkipped="The following prescription(s) were not printed because they do not have a provider: \r\n" + messageSkipped;
				MessageBox.Show(messageSkipped);
			}
			if(PrinterSettings.InstalledPrinters.Count==0) {
				MsgBox.Show(this,"Error: No Printers Installed\r\n"+
									"If you do have a printer installed, restarting the workstation may solve the problem."
				);
				return;
			}
			if(listRxPats.Count==1) {//old way of printing one rx
				//This logic is an exact copy of FormRxEdit.butPrint_Click()'s logic.  If this is updated, that method needs to be updated as well.
				sheetDef=SheetDefs.GetSheetsDefault(SheetTypeEnum.Rx,Clinics.ClinicNum);
				sheet=SheetUtil.CreateSheet(sheetDef,_patient.PatNum);
				SheetParameter.SetParameter(sheet,"RxNum",listRxPats[0].RxNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				SheetPrinting.PrintRx(sheet,listRxPats[0]);
				if(listRxPats[0].SendStatus!=RxSendStatus.InElectQueue && listRxPats[0].SendStatus!=RxSendStatus.SentElect) {
					if(listRxPats[0].SendStatus!=RxSendStatus.Printed) {
						SecurityLogs.MakeLogEntry(Permissions.RxEdit,listRxPats[0].PatNum,"Send Status of Rx "+listRxPats[0].Drug+" changed from "+listRxPats[0].SendStatus+" to Printed",listRxPats[0].RxNum,listRxPats[0].DateTStamp);
					}
					listRxPats[0].SendStatus=RxSendStatus.Printed;
					RxPats.Update(listRxPats[0]);
				}
				SecurityLogs.MakeLogEntry(Permissions.RxEdit,listRxPats[0].PatNum,"Printed as: "+listRxPats[0].RxDate.ToShortDateString()+","+listRxPats[0].Drug+",ProvNum:"+listRxPats[0].ProvNum+",Disp:"+listRxPats[0].Disp+",Refills:"+listRxPats[0].Refills,listRxPats[0].RxNum,listRxPats[0].DateTStamp);
			}
			else { //multiple rx selected
				//Print batch list of rx
				SheetPrinting.PrintMultiRx(listRxPats);
				for(int i=0;i<listRxPats.Count;i++) {
					SecurityLogs.MakeLogEntry(Permissions.RxEdit,listRxPats[i].PatNum,"Printed as: "+listRxPats[i].RxDate.ToShortDateString()+","+listRxPats[i].Drug+",ProvNum:"+listRxPats[i].ProvNum+",Disp:"+listRxPats[i].Disp+",Refills:"+listRxPats[i].Refills,listRxPats[i].RxNum,listRxPats[i].DateTStamp);
					if(listRxPats[i].SendStatus==RxSendStatus.InElectQueue || listRxPats[i].SendStatus==RxSendStatus.SentElect) {
						continue;
					}
					if(listRxPats[i].SendStatus!=RxSendStatus.Printed) {
						SecurityLogs.MakeLogEntry(Permissions.RxEdit,listRxPats[i].PatNum,"Send Status of Rx "+listRxPats[i].Drug+" changed from "+listRxPats[i].SendStatus+" to Printed",listRxPats[i].RxNum,listRxPats[i].DateTStamp);
					}
					listRxPats[i].SendStatus=RxSendStatus.Printed;
					RxPats.Update(listRxPats[i]);
				}
			}
		}

		private void butNewRx_Click(object sender,EventArgs e) {
			//This code is a copy of ContrChart.Tool_Rx_Click().  Any changes to this code need to be changed there too.
			if(!Security.IsAuthorized(Permissions.RxCreate)) {
				return;
			}
			if(Programs.UsingEcwTightOrFullMode() && Bridges.ECW.UserId!=0) {
				VBbridges.Ecw.LoadRxForm((int)Bridges.ECW.UserId,Bridges.ECW.EcwConfigPath,(int)Bridges.ECW.AptNum);
				//refresh the right panel:
				try {
					string strAppServer=VBbridges.Ecw.GetAppServer((int)Bridges.ECW.UserId,Bridges.ECW.EcwConfigPath);
					labelECWerror.Visible=false;
				}
				catch(Exception ex) {
					labelECWerror.Text="Error: "+ex.Message;
					labelECWerror.Visible=true;
				}
				FillGrid();
				return;
			}
			using FormRxSelect formRxSelect=new FormRxSelect(_patient);
			formRxSelect.ShowDialog();
			if(formRxSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RxCreate,_patient.PatNum,"Created prescription.");
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}