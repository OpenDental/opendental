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
		private Userod _user;
		private List<Procedure> _listNonCpoeProcs=new List<Procedure>();
		private List<Appointment> _listAppointments=new List<Appointment>();
		private List<Patient> _listPats=new List<Patient>();
		private List<ProcedureCode> _listProcCodes;

		public FormRadOrderList(Userod user) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=menuRightClick;
			_user=user;
		}

		private void FormProcRadLists_Load(object sender,EventArgs e) {
			RefreshRadOrdersForUser(_user);
		}

		///<summary>Refreshes the list of radiology orders showing with the rad orders associated to the user passed in.</summary>
		public void RefreshRadOrdersForUser(Userod user) {
			_listNonCpoeProcs.Clear();
			_listAppointments.Clear();
			_listPats.Clear();
			if(user==null) {
				FillGrid();//Nothing to show the user.
				return;
			}
			_user=user;
			//Add all non-CPOE radiology procedures
			_listNonCpoeProcs=Procedures.GetProcsNonCpoeAttachedToApptsForProv(_user.ProvNum);
			//Keep a deep copy of the procedure code cache around for ease of use.
			_listProcCodes=ProcedureCodes.GetListDeep();
			//This list of appointments can be enhanced to include many more appointments when needed.
			_listAppointments=Appointments.GetMultApts(_listNonCpoeProcs.Select(x => x.AptNum).Distinct().ToList());
			//This list of patients can be enhanced to include many more pat nums when needed.
			_listPats=Patients.GetLimForPats(_listNonCpoeProcs.Select(x => x.PatNum).Distinct().ToList());
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.AllowSortingByColumn=true;
			GridColumn col=new GridColumn(Lan.g("TableRadiologyOrders","Date"),90,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Name"),220);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Code"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Abbr"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRadiologyOrders","Description"),110){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listNonCpoeProcs.Count;i++) {
				Patient pat=_listPats.FirstOrDefault(x => x.PatNum==_listNonCpoeProcs[i].PatNum);
				ProcedureCode procCode=_listProcCodes.FirstOrDefault(x => x.CodeNum==_listNonCpoeProcs[i].CodeNum);
				Appointment appt=_listAppointments.FirstOrDefault(x => x.AptNum==_listNonCpoeProcs[i].AptNum);
				if(pat==null || procCode==null || appt==null) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(appt.AptDateTime.ToShortDateString());
				row.Cells.Add(Patients.GetNameLF(pat.LName,pat.FName,pat.Preferred,pat.MiddleI));
				row.Cells.Add(procCode.ProcCode);
				row.Cells.Add(procCode.AbbrDesc);
				row.Cells.Add(procCode.Descript);
				row.Tag=_listNonCpoeProcs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butGotoFamily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FamilyModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a radiology order first.");
				return;
			}
			Procedure proc=(Procedure)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			GotoModule.GotoFamily(proc.PatNum);
		}

		private void butGotoChart_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ChartModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a radiology order first.");
				return;
			}
			Procedure proc=(Procedure)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			GotoModule.GotoChart(proc.PatNum);
		}

		private void butSelected_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select at least one radiology order to approve.");
				return;
			}
			List<Procedure> listSelectedProcs=new List<Procedure>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				Procedure proc=(Procedure)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				listSelectedProcs.Add(proc);
				_listNonCpoeProcs.Remove(proc);
			}
			Procedures.UpdateCpoeForProcs(listSelectedProcs.Select(x => x.ProcNum).Distinct().ToList(),true);
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Approve all radiology orders?")) {
				return;
			}
			Procedures.UpdateCpoeForProcs(_listNonCpoeProcs.Select(x => x.ProcNum).Distinct().ToList(),true);
			DialogResult=DialogResult.OK;
			this.Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
		}

	}
}