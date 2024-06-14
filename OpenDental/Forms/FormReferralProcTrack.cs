using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReferralProcTrack:FormODBase {
		private DataTable _tableReferrals;
		private List<RefAttach> _listRefAttaches;
		private DateTime _dateFrom;
		private DateTime _dateTo;
		private int _pagesPrinted;
		private bool _isHeadPrinted;
		private int _heightHeadingPrint;

		public FormReferralProcTrack() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReferralProcTrack_Load(object sender,EventArgs e) {
			_dateFrom=DateTime.Now.AddMonths(-1);
			textDateFrom.Text=_dateFrom.ToShortDateString();
			_dateTo=DateTime.Now;
			textDateTo.Text=_dateTo.ToShortDateString();
			FillGrid();
		}

		private void FillGrid() {
			if(!textDateTo.IsValid() || !textDateFrom.IsValid()) {//Test To and From dates
				MsgBox.Show(this,"Please enter valid To and From dates.");
				return;
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			_dateTo=PIn.Date(textDateTo.Text);
			if(_dateTo<_dateFrom) {
				MsgBox.Show(this,"Date To cannot be before Date From.");
				return;
			}
//todo: checkbox
			_listRefAttaches=RefAttaches.RefreshForReferralProcTrack(_dateFrom,_dateTo,checkComplete.Checked);
			_tableReferrals=Procedures.GetReferred(_dateFrom,_dateTo,checkComplete.Checked);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),125);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Referred To"),125);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),125);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),125);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Referred"),86);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Done"),86);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),84);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime date;
			for(int i=0;i<_tableReferrals.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Patients.GetPat(PIn.Long(_tableReferrals.Rows[i]["PatNum"].ToString())).GetNameLF());
				row.Cells.Add(_tableReferrals.Rows[i]["LName"].ToString()+", "+_tableReferrals.Rows[i]["FName"].ToString()+" "+_tableReferrals.Rows[i]["MName"].ToString());
				row.Cells.Add(ProcedureCodes.GetLaymanTerm(PIn.Long(_tableReferrals.Rows[i]["CodeNum"].ToString())));
				row.Cells.Add(_tableReferrals.Rows[i]["Note"].ToString());
				date=PIn.Date(_tableReferrals.Rows[i]["RefDate"].ToString());
				if(date.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(date.ToShortDateString());
				}
				date=PIn.Date(_tableReferrals.Rows[i]["DateProcComplete"].ToString());
				if(date.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(date.ToShortDateString());
				}
				ReferralToStatus referalToStatus=(ReferralToStatus)PIn.Int(_tableReferrals.Rows[i]["RefToStatus"].ToString());
				if(referalToStatus==ReferralToStatus.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(referalToStatus.ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FrmRefAttachEdit frmRefAttachEdit=new FrmRefAttachEdit();
			RefAttach refAttach=_listRefAttaches[e.Row].Copy();
			frmRefAttachEdit.RefAttachCur=refAttach;
			frmRefAttachEdit.ShowDialog();
			FillGrid();
			//reselect
			for(int i=0;i<_listRefAttaches.Count;i++){
				if(_listRefAttaches[i].RefAttachNum==refAttach.RefAttachNum) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkComplete_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Referred procedure tracking list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadPrinted) {
				text=Lan.g(this,"Referred Procedure Tracking");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				if(checkComplete.Checked) {
					text="Including Incomplete Procedures";
				}
				else {
					text="Including Only Complete Procedures";
				}
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				text="From "+_dateFrom.ToShortDateString()+" to "+_dateTo.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangleBounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				text="Total Referrals: "+_listRefAttaches.Count;
				g.DrawString(text,fontSubHeading,Brushes.Black,center+gridMain.Width/2-g.MeasureString(text,fontSubHeading).Width-10,yPos);
			}
			g.Dispose();
		}

	}
}