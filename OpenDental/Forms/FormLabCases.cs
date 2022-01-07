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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLabCases : FormODBase {
		private DataTable table;
		//<summary>Set this to the selected date on the schedule, and date range will start out based on this date.</summary>
		//public DateTime DateViewing;
		///<summary>If this is zero, then it's an ordinary close.</summary>
		public long GoToAptNum;
		public bool headingPrinted;
		public int headingPrintH;
		public int pagesPrinted;

		///<summary></summary>
		public FormLabCases()
		{
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
			table=LabCases.Refresh(PIn.Date(textDateFrom.Text),dateMax,checkShowAll.Checked,checkShowUnattached.Checked);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableLabCases","Appt Date Time"),120);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Procedures"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Patient"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Status"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Lab"),75);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Lab Phone"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCases","Instructions"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<long> operatoryNums = new List<long>();
			if(comboClinic.IsAllSelected && !Security.CurUser.ClinicIsRestricted) {//"All"
				operatoryNums=null;
			}
			else {//"All" that the user has access to or it could be just a single clinic the user has selected
				foreach(long clinicNum in comboClinic.ListSelectedClinicNums) {
					operatoryNums.AddRange(Operatories.GetOpsForClinic(clinicNum).Select(x => x.OperatoryNum));
				}
			}
			for(int i=0;i<table.Rows.Count;i++){
				if(PrefC.HasClinicsEnabled //no filtering for non clinics.
					&& operatoryNums!=null //we don't have "All" selected for an unrestricted user.
					&& table.Rows[i]["AptNum"].ToString()!="0" //show unattached for any clinic 
					&& !operatoryNums.Contains(PIn.Long(table.Rows[i]["OpNum"].ToString()))) //Attached appointment is scheduled in an Op for the clinic
				{
					continue;//appointment scheduled in an operatory for another clinic.
				}
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["aptDateTime"].ToString());
				row.Cells.Add(table.Rows[i]["ProcDescript"].ToString());
				row.Cells.Add(table.Rows[i]["patient"].ToString());
				row.Cells.Add(table.Rows[i]["status"].ToString());
				row.Cells.Add(table.Rows[i]["lab"].ToString());
				row.Cells.Add(table.Rows[i]["phone"].ToString());
				row.Cells.Add(table.Rows[i]["Instructions"].ToString());
				row.Tag=table.Rows[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.AllowSortingByColumn=true;
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow row=(DataRow)gridMain.ListGridRows[e.Row].Tag;
			long selectedLabCase=PIn.Long(row["LabCaseNum"].ToString());
			using FormLabCaseEdit FormL=new FormLabCaseEdit();
			FormL.CaseCur=LabCases.GetOne(selectedLabCase);
			FormL.ShowDialog();
			switch(FormL.DialogResult) {
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
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["LabCaseNum"].ToString()==selectedLabCase.ToString()){
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
			Appointment apt=Appointments.GetOneApt(PIn.Long(row["AptNum"].ToString()));
			if(apt.AptStatus==ApptStatus.UnschedList){
				MsgBox.Show(this,"Cannot go to an unscheduled appointment");
				return;
			}
			GoToAptNum=apt.AptNum;
			Close();
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Lab Cases");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count<1) {
				MsgBox.Show(this,"Nothing to print.");
				return;
			}
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Lab case list printed"),PrintoutOrientation.Landscape);
		}



		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}





















