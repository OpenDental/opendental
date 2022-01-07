using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OpenDental.UI;
using CodeBase;
using System.Linq;

namespace OpenDental{
	public partial class FormRpProcNote:FormODBase {
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;

		///<summary></summary>
		public FormRpProcNote() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpProcNote_Load(object sender,System.EventArgs e) {
			checkNoNotes.Checked=PrefC.GetBool(PrefName.ReportsIncompleteProcsNoNotes);
			checkUnsignedNote.Checked=PrefC.GetBool(PrefName.ReportsIncompleteProcsUnsigned);
			FillProvs();
			dateRangePicker.SetDateTimeFrom(DateTime.Today);
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			FillGrid();
		}

		private void FillProvs() {
			comboProvs.IncludeAll=true;
			comboProvs.Items.AddProvsFull(Providers.GetListReports());
			comboProvs.IsAllSelected=true;
		}

		private void FillGrid() {
			DataTable table=GetIncompleteProcNotes();
			bool includeNoNotes=checkNoNotes.Checked;
			bool includeUnsignedNotes=checkUnsignedNote.Checked;
			gridMain.BeginUpdate();
			//Columns
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient Name"),120);
			gridMain.ListGridColumns.Add(col);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g(this,"Code"),150);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Description"),220);
				gridMain.ListGridColumns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g(this,"Code"),150);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Description"),220);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Tth"),30);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Surf"),40);
				gridMain.ListGridColumns.Add(col);
			}
			if(includeUnsignedNotes || includeNoNotes) {
				col=new GridColumn(Lan.g(this,"Incomplete"),80);
				gridMain.ListGridColumns.Add(col);
			}
			if(includeNoNotes) {
				col=new GridColumn(Lan.g(this,"No Note"),60);
				gridMain.ListGridColumns.Add(col);
			}
			if(includeUnsignedNotes) {
				col=new GridColumn(Lan.g(this,"Unsigned"),70);
				gridMain.ListGridColumns.Add(col);
			}
			//Rows
			gridMain.ListGridRows.Clear();
			foreach(DataRow row in table.Rows) {
				GridRow newRow=new GridRow();
				newRow.Cells.Add(PIn.Date(row["ProcDate"].ToString()).ToString("d"));
				newRow.Cells.Add(row["PatName"].ToString());
				if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					newRow.Cells.Add(row["ProcCode"].ToString());
					newRow.Cells.Add(row["Descript"].ToString());
				}
				else {
					newRow.Cells.Add(row["ProcCode"].ToString());
					newRow.Cells.Add(row["Descript"].ToString());
					newRow.Cells.Add(row["ToothNum"].ToString());
					newRow.Cells.Add(row["Surf"].ToString());
				}
				if(includeUnsignedNotes || includeNoNotes) {
					newRow.Cells.Add(row["Incomplete"].ToString());
				}
				if(includeNoNotes) {
					newRow.Cells.Add(row["HasNoNote"].ToString());
				}
				if(includeUnsignedNotes) {
					newRow.Cells.Add(row["HasUnsignedNote"].ToString());
				}
				newRow.Tag=row["PatNum"].ToString();
				gridMain.ListGridRows.Add(newRow);
			}
			gridMain.EndUpdate();
		}

		private DataTable GetIncompleteProcNotes() {
			RpProcNote.ProcNoteGroupBy groupBy;
			if(radioPatient.Checked) {
				groupBy=RpProcNote.ProcNoteGroupBy.Patient;
			}
			else if(radioProcDate.Checked) {
				groupBy=RpProcNote.ProcNoteGroupBy.DateAndPatient;
			}
			else {
				groupBy=RpProcNote.ProcNoteGroupBy.Procedure;
			}
			return RpProcNote.GetData(GetListSelectedProvNums(),comboClinics.ListSelectedClinicNums,dateRangePicker.GetDateTimeFrom(),
				dateRangePicker.GetDateTimeTo(),checkNoNotes.Checked,checkUnsignedNote.Checked,
				(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers),groupBy,showExcludedCodes:checkShowExcludedCodes.Checked);
		}

		///<summary></summary>
		private List<long> GetListSelectedProvNums (){
			return comboProvs.GetSelectedProvNums();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.ChartModule)) {
				return;
			}
			long goToPatNum=PIn.Long(gridMain.ListGridRows[e.Row].Tag.ToString());
			SendToBack();
			GotoModule.GotoChart(goToPatNum);
		}

		private void goToChartToolStripMenuItem_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.ChartModule)) {
				return;
			}
			long patNum=PIn.Long(gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag.ToString());
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			SendToBack();
			GotoModule.GotoChart(pat.PatNum);
		}

		private void radioProc_MouseCaptureChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioPatient_MouseCaptureChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioProcDate_MouseCaptureChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Incomplete Procedure Notes report printed"),PrintoutOrientation.Landscape);
		}  
		
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Incomplete Procedure Notes");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=PrefC.GetString(PrefName.PracticeTitle);
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(comboProvs.IsAllSelected){
					text=Lan.g(this,"All Providers");
				}
				else {
					text=Lan.g(this,"For Providers:")+" "+string.Join(",",GetListSelectedProvNums().Select(provNum => Providers.GetAbbr(provNum)));
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(comboClinics.ListSelectedClinicNums.Count==0 || comboClinics.IsAllSelected) {//No clinics selected or 'All' selected
					text=Lan.g(this,"All Clinics");
				}
				else {
					text="For Clinics: "+comboClinics.GetStringSelectedClinics();
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}
		
		private void butExport_Click(object sender,System.EventArgs e) {
			gridMain.Export("Incomplete Procedure Notes");
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}




















