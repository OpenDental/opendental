using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormApptConflicts:FormODBase {
		private bool _hasHeadingPrinted;
		private int _pagesPrinted;
		///<summary>Passed in list of Appts to show.</summary>
		private List<Appointment> _listAppointments;
		///<summary>All unique PatNums via the list of appointments.</summary>
		private List<Patient> _listPatients;

		public FormApptConflicts(List<Appointment> listAppointments) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listAppointments=listAppointments.Select(x => x.Copy()).ToList();
		}

		private void FormApptConflicts_Load(object sender,EventArgs e) {
			gridConflicts.ContextMenu=contextRightClick;
			FillGrid();
		}
		
		private void FillGrid(){
			this.Cursor=Cursors.WaitCursor;
			_listPatients=Patients.GetLimForPats(_listAppointments.Select(x => x.PatNum).Distinct().ToList());
			gridConflicts.BeginUpdate();
			gridConflicts.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptConflicts","Patient"),140);
			gridConflicts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Date"),120);
			gridConflicts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Op"),110);
			gridConflicts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Prov"),50);
			gridConflicts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Procedures"),150);
			gridConflicts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Notes"),200);
			gridConflicts.Columns.Add(col);
			gridConflicts.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAppointments.Count;i++) {
				row=new GridRow();
				Patient patient=_listPatients.First(x => x.PatNum==_listAppointments[i].PatNum);
				row.Cells.Add(patient.GetNameLF());
				if(_listAppointments[i].AptDateTime.Year < 1880){//shouldn't be possible.
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listAppointments[i].AptDateTime.ToShortDateString()+"  "+_listAppointments[i].AptDateTime.ToShortTimeString());
				}
				row.Cells.Add(Operatories.GetAbbrev(_listAppointments[i].Op));
				if(_listAppointments[i].IsHygiene) {
					row.Cells.Add(Providers.GetAbbr(_listAppointments[i].ProvHyg));
				}
				else {
					row.Cells.Add(Providers.GetAbbr(_listAppointments[i].ProvNum));
				}
				row.Cells.Add(_listAppointments[i].ProcDescript);
				row.Cells.Add(_listAppointments[i].Note);
				row.Tag=_listAppointments[i];
				gridConflicts.ListGridRows.Add(row);
			}
			gridConflicts.EndUpdate();
			Cursor=Cursors.Default;
		}
		
		private void gridConflicts_DoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;
			int currentScroll=gridConflicts.ScrollValue;
			Appointment appointment=(Appointment)gridConflicts.ListGridRows[e.Row].Tag;
			long selectedPatNum=appointment.PatNum;
			Patient patient=_listPatients.First(x => x.PatNum==selectedPatNum);
			FormOpenDental.S_Contr_PatientSelected(patient,true);
			using FormApptEdit formApptEdit=new FormApptEdit(appointment.AptNum);
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formApptEdit.PinClicked) {
				SendPinboard_Click(); //Whatever they double clicked on will still be selected, just fire the event to send it to the pinboard.
			}
			gridConflicts.SetSelected(currentSelection,true);
			gridConflicts.ScrollValue=currentScroll;
		}

		private void menuItemPin_Click(object sender,EventArgs e) {
			SendPinboard_Click();
		}
		
		///<summary>Removes the selected appoinments from the class wide list of appointments, sends the appointments to the pinboard,
		///and then refreshes the grid so that the user can see that they are "taking care" of the conflicts.</summary>
		private void SendPinboard_Click() {
			if(gridConflicts.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listSelectedAptNums=new List<long>();
			for(int i=0;i<gridConflicts.SelectedIndices.Length;i++) {
				listSelectedAptNums.Add(((Appointment)gridConflicts.ListGridRows[gridConflicts.SelectedIndices[i]].Tag).AptNum);
			}
			_listAppointments.RemoveAll(x => listSelectedAptNums.Contains(x.AptNum));
			FillGrid();
			GotoModule.PinToAppt(listSelectedAptNums,0); //Pins all appointments to the pinboard that were in listAptSelected.
		}

		private void menuItemSelectPatient_Click(object sender,EventArgs e) {
			SelectPatient_Click();
		}
		
		private void SelectPatient_Click() {
			if(gridConflicts.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			Patient patient=_listPatients.First(x => x.PatNum==_listAppointments[gridConflicts.SelectedIndices[gridConflicts.SelectedIndices.Length-1]].PatNum);
			FormOpenDental.S_Contr_PatientSelected(patient,true);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_hasHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Operatory Merge - conflict appointment List printed."));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int y=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			int headingPrintH=0;
			if(!_hasHeadingPrinted) {
				text=Lan.g(this,"Operatory Merge - Conflict Appointment List");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,y);
				y+=25;
				_hasHeadingPrinted=true;
				headingPrintH=y;
			}
			#endregion
			y=gridConflicts.PrintPage(g,_pagesPrinted,bounds,headingPrintH);
			_pagesPrinted++;
			if(y==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}
