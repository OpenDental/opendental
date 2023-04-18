using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary</summary>
	public partial class FormLabCases : FormODBase {
		//public DateTime DateViewing;
		///<summary>If this is zero, then it's an ordinary close.</summary>
		public long GoToAptNum;
		public bool IsHeadingPrinted;
		public int HeadingPrintH;
		public int PagesPrinted;
		//<summary>Set this to the selected date on the schedule, and date range will start out based on this date.</summary>
		private DataTable _table;

		///<summary></summary>
		public FormLabCases(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLabCases_Load(object sender,EventArgs e) {
			if(Clinics.ClinicNum==0) {
				comboClinic.IsAllSelected=true;
			}
			else {
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
			}
			gridMain.ContextMenu=contextMenu1;
			textDateFrom.Text="";//DateViewing.ToShortDateString();
			textDateTo.Text="";//DateViewing.AddDays(5).ToShortDateString();
			//checkShowAll.Checked=false
			FillGrid();
		}

		private void FillGrid() {
			if(!textDateFrom.IsValid() || !textDateFrom.IsValid()) {
				//MsgBox.Show(this,"Please fix errors first.");
				return;
			}
			DateTime dateMax = new DateTime(2100,1,1);
			if(textDateTo.Text!="") {
				dateMax=PIn.Date(textDateTo.Text);
			}
			_table=LabCases.Refresh(PIn.Date(textDateFrom.Text),dateMax,checkShowAll.Checked,checkShowUnattached.Checked);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableLabCases","Appt Date Time"),120);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Appt Status"),75);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Procedures"),200);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Patient"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Status"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Lab"),75);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Lab Phone"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Instructions"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<long> operatoryNums = new List<long>();
			if(comboClinic.IsAllSelected && !Security.CurUser.ClinicIsRestricted) {//"All"
				operatoryNums=null;
			}
			else {//"All" that the user has access to or it could be just a single clinic the user has selected
				for(int i=0;i<comboClinic.ListSelectedClinicNums.Count;i++) {
					operatoryNums.AddRange(Operatories.GetOpsForClinic(comboClinic.ListSelectedClinicNums[i]).Select(x => x.OperatoryNum));
				}
			}
			for(int i=0;i<_table.Rows.Count;i++){
				if(PrefC.HasClinicsEnabled //no filtering for non clinics.
					&& operatoryNums!=null //we don't have "All" selected for an unrestricted user.
					&& _table.Rows[i]["AptNum"].ToString()!="0" //show unattached for any clinic 
					&& !operatoryNums.Contains(PIn.Long(_table.Rows[i]["OpNum"].ToString()))) //Attached appointment is scheduled in an Op for the clinic
				{
					continue;//appointment scheduled in an operatory for another clinic.
				}
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["aptDateTime"].ToString());
				row.Cells.Add(_table.Rows[i]["aptStatus"].ToString());
				row.Cells.Add(_table.Rows[i]["ProcDescript"].ToString());
				row.Cells.Add(_table.Rows[i]["patient"].ToString());
				row.Cells.Add(_table.Rows[i]["status"].ToString());
				row.Cells.Add(_table.Rows[i]["lab"].ToString());
				row.Cells.Add(_table.Rows[i]["phone"].ToString());
				row.Cells.Add(_table.Rows[i]["Instructions"].ToString());
				row.Tag=_table.Rows[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.AllowSortingByColumn=true;
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow row=(DataRow)gridMain.ListGridRows[e.Row].Tag;
			long selectedLabCase=PIn.Long(row["LabCaseNum"].ToString());
			using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
			formLabCaseEdit.LabCaseCur=LabCases.GetOne(selectedLabCase);
			formLabCaseEdit.ShowDialog();
			switch(formLabCaseEdit.DialogResult) {
				default:
				case DialogResult.Cancel://==Jordan don't refresh unless we have to.  It messes up the user's ordering.
					return;
				case DialogResult.Abort://User was forced out of window due to a null object, refresh grid to remove missing row.
					FillGrid();
					return;
				case DialogResult.OK:
					//Intentionally blank
					break;
			}
			FillGrid();
			for(int i=0;i<_table.Rows.Count;i++){
				if(_table.Rows[i]["LabCaseNum"].ToString()==selectedLabCase.ToString()){
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void comboClinic_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateFrom.IsValid()) {
				MsgBox.Show(this,"Please fix errors first.");
				return;
			}
			FillGrid();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a lab case first.");
				return;
			}
			DataRow row=gridMain.SelectedTag<DataRow>();
			if(row["AptNum"].ToString()=="0") {
				MsgBox.Show(this,"There are no appointments for unattached lab cases.");
				return;
			}
			Appointment appointment=Appointments.GetOneApt(PIn.Long(row["AptNum"].ToString()));
			if(appointment.AptStatus==ApptStatus.UnschedList){
				MsgBox.Show(this,"Cannot go to an unscheduled appointment");
				return;
			}
			GoToAptNum=appointment.AptNum;
			Close();
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!IsHeadingPrinted) {
				text=Lan.g(this,"Lab Cases");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				IsHeadingPrinted=true;
				HeadingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,PagesPrinted,rectangleBounds,HeadingPrintH);
			PagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
				return;
			}
			e.HasMorePages=false;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count<1) {
				MsgBox.Show(this,"Nothing to print.");
				return;
			}
			PagesPrinted=0;
			IsHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Lab case list printed"),PrintoutOrientation.Landscape);
		}



		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}





















