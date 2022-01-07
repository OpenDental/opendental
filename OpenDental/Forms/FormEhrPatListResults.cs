using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;

namespace OpenDental {
	public partial class FormEhrPatListResults:FormODBase {
		private List<EhrPatListElement> elementList;
		private DataTable table;
		private bool headingPrinted;
		private int pagesPrinted;
		/// <summary>Used to sort and keep the ASC/DESC UI intact, mostly useless.</summary>
		private int orderByColumn=-1;

		public FormEhrPatListResults(List<EhrPatListElement> ElementList) {
			InitializeComponent();
			InitializeLayoutManager();
			elementList=ElementList;
		}

		private void FormPatListResults_Load(object sender,EventArgs e) {
			FillGrid(true);
		}

		///<summary>Deprecated.  We no longer need to pass in a bool.</summary>
		private void FillGrid(bool isAsc) {
			FillGrid();
		}

		private void FillGrid() {
			table=EhrPatListElements.GetListOrderBy2014Retro(elementList);
			int colWidth=0;
			Graphics g=CreateGraphics();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("PatNum",60,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.AmountParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Full Name",200);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridMain.ListGridColumns.Add(col);
			for(int i=0;i<elementList.Count;i++) {
				if(orderByColumn==-1 && elementList[i].OrderBy) {
					//There can be 0 to 1 elements that have OrderBy set to true. 
					//we will use this to determine how to use the ASC/DESC buttons.
					//some elements add one column, others add two, this selects the column that is to be added next.
					orderByColumn=gridMain.ListGridColumns.Count;
				}
				switch(elementList[i].Restriction) {
					case EhrRestrictionType.Birthdate:
						col=new GridColumn("Birthdate",80,HorizontalAlignment.Center);
						col.SortingStrategy=GridSortingStrategy.DateParse;
						gridMain.ListGridColumns.Add(col);
						break;
					case EhrRestrictionType.Gender:
						col=new GridColumn("Gender",80,HorizontalAlignment.Center);
						gridMain.ListGridColumns.Add(col);
						break;
					case EhrRestrictionType.LabResult:
						colWidth=System.Convert.ToInt32(g.MeasureString("Lab Value: "+elementList[i].CompareString,this.Font).Width);
						col.SortingStrategy=GridSortingStrategy.AmountParse;
						colWidth=colWidth+(colWidth/10);//Add 10%
						col=new GridColumn("Lab Value: "+elementList[i].CompareString,colWidth,HorizontalAlignment.Center);
						gridMain.ListGridColumns.Add(col);
						break;
					case EhrRestrictionType.Medication:
						col=new GridColumn("Medication",90,HorizontalAlignment.Center);
						col.SortingStrategy=GridSortingStrategy.StringCompare;
						gridMain.ListGridColumns.Add(col);
						break;
					case EhrRestrictionType.Problem:
						col=new GridColumn("Disease",160,HorizontalAlignment.Center);
						col.SortingStrategy=GridSortingStrategy.StringCompare;
						gridMain.ListGridColumns.Add(col);
						break;
					default:
						//should not happen.
						break;
				}
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["PatNum"].ToString());
				row.Cells.Add(table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString());
				//Add 3 to j to compensate for PatNum, LName and FName.
				for(int j=0;j<elementList.Count;j++) {//sometimes one element might pull two columns, Lab Results for instance.//<elementList.Count;j++) {
					switch(elementList[j].Restriction) {
						case EhrRestrictionType.Medication:
						case EhrRestrictionType.Problem:
							row.Cells.Add(table.Rows[i][j+3].ToString());
							break;
						case EhrRestrictionType.Birthdate:
							row.Cells.Add(table.Rows[i][j+3].ToString().Replace(" 12:00:00 AM",""));
							break;
						case EhrRestrictionType.LabResult:
							row.Cells.Add(table.Rows[i][j+3].ToString());//obsVal
							break;
						case EhrRestrictionType.Gender:
							switch(table.Rows[i][j+3].ToString()) {
								case "0"://Male
									row.Cells.Add("Male");
									break;
								case "1"://Female
									row.Cells.Add("Female");
									break;
								case "2"://Unknown
								default:
									row.Cells.Add("Unknown");
									break;
							}
							break;
					}
					
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			g.Dispose();
		}

		////private void FillGrid(bool isAsc) {
		////	table=EhrPatListElements.GetListOrderBy(elementList,isAsc);
		////	int colWidth=0;
		////	Graphics g=CreateGraphics();
		////	gridMain.BeginUpdate();
		////	gridMain.Columns.Clear();
		////	ODGridColumn col;
		////	col=new ODGridColumn("PatNum",60,HorizontalAlignment.Center);
		////	gridMain.Columns.Add(col);
		////	col=new ODGridColumn("Full Name",110);
		////	gridMain.Columns.Add(col);
		////	for(int i=0;i<elementList.Count;i++) {
		////		if(elementList[i].Restriction==EhrRestrictionType.Birthdate) {
		////			col=new ODGridColumn("Birthdate",80,HorizontalAlignment.Center);
		////			gridMain.Columns.Add(col);
		////		}
		////		else if(elementList[i].Restriction==EhrRestrictionType.Gender) {
		////			col=new ODGridColumn("Gender",70,HorizontalAlignment.Center);
		////			gridMain.Columns.Add(col);
		////		}
		////		else if(elementList[i].Restriction==EhrRestrictionType.Problem) {
		////			col=new ODGridColumn("Disease",160,HorizontalAlignment.Center);
		////			gridMain.Columns.Add(col);
		////		}
		////		else {
		////			colWidth=System.Convert.ToInt32(g.MeasureString(elementList[i].CompareString,this.Font).Width);
		////			colWidth=colWidth+(colWidth/10);//Add 10%
		////			if(colWidth<90) {
		////				colWidth=90;//Minimum of 90 width.
		////			}
		////			col=new ODGridColumn(elementList[i].CompareString,colWidth,HorizontalAlignment.Center);
		////			gridMain.Columns.Add(col);
		////		}
		////	}
		////	gridMain.Rows.Clear();
		////	ODGridRow row;
		////	string icd9Desc;
		////	for(int i=0;i<table.Rows.Count;i++) {
		////		row=new ODGridRow();
		////		row.Cells.Add(table.Rows[i]["PatNum"].ToString());
		////		row.Cells.Add(table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString());
		////		//Add 3 to j to compensate for PatNum, LName and FName.
		////		for(int j=0;j<elementList.Count;j++) {
		////			if(elementList[j].Restriction==EhrRestrictionType.Problem) {
		////				ICD9 icd;
		////				try {
		////					icd=ICD9s.GetOne(PIn.Long(table.Rows[i][j+3].ToString()));
		////					icd9Desc="("+icd.ICD9Code+")-"+icd.Description;
		////					row.Cells.Add(icd9Desc);
		////				}
		////				catch {//Graceful fail just in case.
		////					row.Cells.Add("X");
		////				}
		////				continue;
		////			}
		////			if(elementList[j].Restriction==EhrRestrictionType.Medication)	{
		////				row.Cells.Add("X");
		////				continue;
		////			}
		////			if(elementList[j].Restriction==EhrRestrictionType.Birthdate) {
		////				row.Cells.Add(PIn.Date(table.Rows[i][j+3].ToString()).ToShortDateString());
		////				continue;
		////			}
		////			if(elementList[j].Restriction==EhrRestrictionType.Gender)	{
		////				switch(table.Rows[i][j+3].ToString()) {
		////					case "0":
		////						row.Cells.Add("Male");
		////						break;
		////					case "1":
		////						row.Cells.Add("Female");
		////						break;
		////					default:
		////						row.Cells.Add("Unknown");
		////						break;
		////				}
		////				continue;
		////			}
		////			row.Cells.Add(table.Rows[i][j+3].ToString());
		////		}
		////		gridMain.Rows.Add(row);
		////	}
		////	gridMain.EndUpdate();
		////	g.Dispose();
		////}

		//private void radioAsc_CheckedChanged(object sender,EventArgs e) {
		//	if(orderByColumn==-1 || orderByColumn>gridMain.Columns.Count-1){
		//		return;//no order by collumn set, or index out of bounds.
		//	}
		//	gridMain.SortForced(orderByColumn,true);
		//}

		//private void radioDesc_CheckedChanged(object sender,EventArgs e) {
		//	if(orderByColumn==-1 || orderByColumn>gridMain.Columns.Count-1) {
		//		return;//no order by collumn set, or index out of bounds.
		//	}
		//	gridMain.SortForced(orderByColumn,false);
		//}

		private void radioOrderBy_CheckedChanged(object sender,EventArgs e) {
			if(orderByColumn==-1 || orderByColumn>gridMain.ListGridColumns.Count-1) {
				return;//no order by collumn set, or index out of bounds.
			}
			gridMain.SortForced(orderByColumn,radioAsc.Checked);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Patient List Printed"));
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
				text="Patient List";
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				headingPrinted=true;
				//headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,yPos);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
