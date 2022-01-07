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
		DataTable Table;
		List<RefAttach> RefAttachList;
		DateTime DateFrom;
		DateTime DateTo;
		int pagesPrinted;
		bool headingPrinted;
		int headingPrintH;

		public FormReferralProcTrack() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReferralProcTrack_Load(object sender,EventArgs e) {
			DateFrom=DateTime.Now.AddMonths(-1);
			textDateFrom.Text=DateFrom.ToShortDateString();
			DateTo=DateTime.Now;
			textDateTo.Text=DateTo.ToShortDateString();
			FillGrid();
		}

		private void FillGrid() {
			if(!textDateTo.IsValid() || !textDateFrom.IsValid()) {//Test To and From dates
				MsgBox.Show(this,"Please enter valid To and From dates.");
				return;
			}
			DateFrom=PIn.Date(textDateFrom.Text);
			DateTo=PIn.Date(textDateTo.Text);
			if(DateTo<DateFrom) {
				MsgBox.Show(this,"Date To cannot be before Date From.");
				return;
			}
//todo: checkbox
			RefAttachList=RefAttaches.RefreshForReferralProcTrack(DateFrom,DateTo,checkComplete.Checked);
			Table=Procedures.GetReferred(DateFrom,DateTo,checkComplete.Checked);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),125);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Referred To"),125);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),125);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),125);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Referred"),86);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Done"),86);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),84);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime date;
			for(int i=0;i<Table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Patients.GetPat(PIn.Long(Table.Rows[i]["PatNum"].ToString())).GetNameLF());
				row.Cells.Add(Table.Rows[i]["LName"].ToString()+", "+Table.Rows[i]["FName"].ToString()+" "+Table.Rows[i]["MName"].ToString());
				row.Cells.Add(ProcedureCodes.GetLaymanTerm(PIn.Long(Table.Rows[i]["CodeNum"].ToString())));
				row.Cells.Add(Table.Rows[i]["Note"].ToString());
				date=PIn.Date(Table.Rows[i]["RefDate"].ToString());
				if(date.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(date.ToShortDateString());
				}
				date=PIn.Date(Table.Rows[i]["DateProcComplete"].ToString());
				if(date.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(date.ToShortDateString());
				}
				ReferralToStatus refStatus=(ReferralToStatus)PIn.Int(Table.Rows[i]["RefToStatus"].ToString());
				if(refStatus==ReferralToStatus.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(refStatus.ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormRefAttachEdit FormRAE2=new FormRefAttachEdit();
			RefAttach refattach=RefAttachList[e.Row].Copy();
			FormRAE2.RefAttachCur=refattach;
			FormRAE2.ShowDialog();
			FillGrid();
			//reselect
			for(int i=0;i<RefAttachList.Count;i++){
				if(RefAttachList[i].RefAttachNum==refattach.RefAttachNum) {
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
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Referred procedure tracking list printed"),PrintoutOrientation.Portrait);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Referred Procedure Tracking");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(checkComplete.Checked) {
					text="Including Incomplete Procedures";
				}
				else {
					text="Including Only Complete Procedures";
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				text="From "+DateFrom.ToShortDateString()+" to "+DateTo.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
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
				text="Total Referrals: "+RefAttachList.Count;
				g.DrawString(text,subHeadingFont,Brushes.Black,center+gridMain.Width/2-g.MeasureString(text,subHeadingFont).Width-10,yPos);
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}