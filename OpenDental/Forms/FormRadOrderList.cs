using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections;

namespace OpenDental {
	///<summary>This form was originally designed to show providers all radiology procedures that are not flagged as CPOE.
	///It is named generically because it can be enhanced in the future to actually show more than just radiology orders that need action.</summary>
	public partial class FormRadOrderList:FormODBase {
		private Userod _userod;
		private List<Procedure> _listProceduresNonCpoe=new List<Procedure>();
		private List<Appointment> _listAppointments=new List<Appointment>();
		private List<Patient> _listPatients=new List<Patient>();
		private List<ProcedureCode> _listProcedureCodes;

		public FormRadOrderList(Userod userod) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=menuRightClick;
			_userod=userod;
		}

		private void FormProcRadLists_Load(object sender,EventArgs e) {
			RefreshRadOrdersForUser(_userod);
		}

		///<summary>Refreshes the list of radiology orders showing with the rad orders associated to the user passed in.</summary>
		public void RefreshRadOrdersForUser(Userod userod) {
			_listProceduresNonCpoe.Clear();
			_listAppointments.Clear();
			_listPatients.Clear();
			if(userod==null) {
				FillGrid();//Nothing to show the user.
				return;
			}
			_userod=userod;
			//Add all non-CPOE radiology procedures
			_listProceduresNonCpoe=Procedures.GetProcsNonCpoeAttachedToApptsForProv(_userod.ProvNum);
			//Keep a deep copy of the procedure code cache around for ease of use.
			_listProcedureCodes=ProcedureCodes.GetListDeep();
			//This list of appointments can be enhanced to include many more appointments when needed.
			_listAppointments=Appointments.GetMultApts(_listProceduresNonCpoe.Select(x => x.AptNum).Distinct().ToList());
			//This list of patients can be enhanced to include many more pat nums when needed.
			_listPatients=Patients.GetLimForPats(_listProceduresNonCpoe.Select(x => x.PatNum).Distinct().ToList());
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.AllowSortingByColumn=true;
			GridColumn col=new GridColumn(Lan.g("TableRadiologyOrders","Date"),90,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Name"),220);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Code"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Abbr"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Description"),110){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listProceduresNonCpoe.Count;i++) {
				Patient patient=_listPatients.FirstOrDefault(x => x.PatNum==_listProceduresNonCpoe[i].PatNum);
				ProcedureCode procedureCode=_listProcedureCodes.FirstOrDefault(x => x.CodeNum==_listProceduresNonCpoe[i].CodeNum);
				Appointment appointment=_listAppointments.FirstOrDefault(x => x.AptNum==_listProceduresNonCpoe[i].AptNum);
				if(patient==null || procedureCode==null || appointment==null) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(appointment.AptDateTime.ToShortDateString());
				row.Cells.Add(Patients.GetNameLF(patient.LName,patient.FName,patient.Preferred,patient.MiddleI));
				row.Cells.Add(procedureCode.ProcCode);
				row.Cells.Add(procedureCode.AbbrDesc);
				row.Cells.Add(procedureCode.Descript);
				row.Tag=_listProceduresNonCpoe[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butGotoFamily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.FamilyModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a radiology order first.");
				return;
			}
			Procedure procedure=(Procedure)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			GlobalFormOpenDental.GotoFamily(procedure.PatNum);
		}

		private void butGotoChart_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ChartModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a radiology order first.");
				return;
			}
			Procedure procedure=(Procedure)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			GlobalFormOpenDental.GotoChart(procedure.PatNum);
		}

		private void butSelected_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select at least one radiology order to approve.");
				return;
			}
			List<Procedure> listProceduresSelected=new List<Procedure>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				Procedure procedure=(Procedure)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				listProceduresSelected.Add(procedure);
				_listProceduresNonCpoe.Remove(procedure);
			}
			Procedures.UpdateCpoeForProcs(listProceduresSelected.Select(x => x.ProcNum).Distinct().ToList(),true);
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Approve all radiology orders?")) {
				return;
			}
			Procedures.UpdateCpoeForProcs(_listProceduresNonCpoe.Select(x => x.ProcNum).Distinct().ToList(),true);
			DialogResult=DialogResult.OK;
			this.Close();
		}

	}
}