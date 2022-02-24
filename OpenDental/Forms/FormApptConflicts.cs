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
		private List<Appointment> _listAppts;
		///<summary>All unique PatNums via the list of appointments.</summary>
		private List<Patient> _listPatients;

		public FormApptConflicts(List<Appointment> listAppts) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listAppts=listAppts.Select(x => x.Copy()).ToList();
		}

		private void FormApptConflicts_Load(object sender,EventArgs e) {
			gridConflicts.ContextMenu=contextRightClick;
			FillGrid();
		}
		
		private void FillGrid(){
			this.Cursor=Cursors.WaitCursor;
			_listPatients=Patients.GetLimForPats(_listAppts.Select(x => x.PatNum).Distinct().ToList());
			gridConflicts.BeginUpdate();
			gridConflicts.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptConflicts","Patient"),140);
			gridConflicts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Date"),120);
			gridConflicts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Op"),110);
			gridConflicts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Prov"),50);
			gridConflicts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Procedures"),150);
			gridConflicts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptConflicts","Notes"),200);
			gridConflicts.ListGridColumns.Add(col);
			gridConflicts.ListGridRows.Clear();
			GridRow row;
			foreach(Appointment apptCur in _listAppts) {
				row=new GridRow();
				Patient patCur=_listPatients.First(x => x.PatNum==apptCur.PatNum);
				row.Cells.Add(patCur.GetNameLF());
				if(apptCur.AptDateTime.Year < 1880){//shouldn't be possible.
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(apptCur.AptDateTime.ToShortDateString()+"  "+apptCur.AptDateTime.ToShortTimeString());
				}
				row.Cells.Add(Operatories.GetAbbrev(apptCur.Op));
				if(apptCur.IsHygiene) {
					row.Cells.Add(Providers.GetAbbr(apptCur.ProvHyg));
				}
				else {
					row.Cells.Add(Providers.GetAbbr(apptCur.ProvNum));
				}
				row.Cells.Add(apptCur.ProcDescript);
				row.Cells.Add(apptCur.Note);
				row.Tag=apptCur;
				gridConflicts.ListGridRows.Add(row);
			}
			gridConflicts.EndUpdate();
			Cursor=Cursors.Default;
		}
		
		private void gridConflicts_DoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;
			int currentScroll=gridConflicts.ScrollValue;
			Appointment apptCur=(Appointment)gridConflicts.ListGridRows[e.Row].Tag;
			long SelectedPatNum=apptCur.PatNum;
			Patient pat=_listPatients.First(x => x.PatNum==SelectedPatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormAE=new FormApptEdit(apptCur.AptNum);
			FormAE.PinIsVisible=true;
			FormAE.ShowDialog();
			if(FormAE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormAE.PinClicked) {
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
			_listAppts.RemoveAll(x => listSelectedAptNums.Contains(x.AptNum));
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
			Patient pat=_listPatients.First(x => x.PatNum==_listAppts[gridConflicts.SelectedIndices[gridConflicts.SelectedIndices.Length-1]].PatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
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
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int y=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			int headingPrintH=0;
			if(!_hasHeadingPrinted) {
				text=Lan.g(this,"Operatory Merge - Conflict Appointment List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,y);
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
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}
