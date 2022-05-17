using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	
	///<summary></summary>
	public class ReportSimpleGrid{
		///<summary></summary>
		public DataTable TableQ;
		///<summary></summary>
		public string Query;
		///<summary></summary>
		public string Title;
		///<summary></summary>
		public List<string> SubTitle;
		///<summary></summary>
		private int[] colPos;
		///<summary></summary>
		private string[] colCaption;
		///<summary></summary>
		private HorizontalAlignment[] colAlign;
		///<summary></summary>
		public decimal[] ColTotal;
		///<summary></summary>
		private int[] colWidth;
		///<summary></summary>
		public List<string> Summary;
		///<summary>This is a quick hack to allow printing account numbers on deposit slips in a different font.  It will typically be null.  It will be gone soon.</summary>
		public string SummaryFont;
		///<summary>If there are just too many columns for portrait.</summary>
		public bool IsLandscape;
		///<summary>Set to true if the Query on this object has already been validated.</summary>
		public bool IsSqlValidated;

		public ReportSimpleGrid() {
			SubTitle=new List<string>();
			Summary=new List<string>();
		}

		public int[] ColWidth {
			get {
				return colWidth;
			}
		}

		///<summary>This is a little flakey and will be improved later.  The first value is always 0.  They represent the left edge of each column, so there must be one more than the number of columns in order to record the position of the right edge of the last column.</summary>
		public int[] ColPos {
			get {
				return colPos;
			}
		}

		public string[] ColCaption {
			get {
				return colCaption;
			}
		}

		public HorizontalAlignment[] ColAlign {
			get {
				return colAlign;
			}
		}

		///<summary>Sends the query to the database to retrieve the table.  Then initializes the column objects.</summary>
		public void SubmitQuery(){
			TableQ=Reports.GetTable(Query);
			InitializeColumns();
		}

		///<summary>This is typically used when the data table is to be manually created.  Before calling this, be sure to create TableQ and add columns to it.</summary>
		public void InitializeColumns() {
			colWidth=new int[TableQ.Columns.Count];
			colPos=new int[TableQ.Columns.Count+1];
			colPos[0]=0;
			colCaption=new string[TableQ.Columns.Count];
			colAlign=new HorizontalAlignment[TableQ.Columns.Count];
			ColTotal=new decimal[TableQ.Columns.Count];
		}

		///<summary>DataTable can't sort on columns with commas in the name, so clean up any un-aliased report columns with a comma in the name.
		///https://stackoverflow.com/questions/22097203/datatable-sorting-with-datacolumn-name-with-comma </summary>
		public void FixColumnNames() {
			for(int i=0;i<TableQ.Columns.Count;i++) {
				//Replace any unacceptable characters with valid ones.
				TableQ.Columns[i].ColumnName=TableQ.Columns[i].ColumnName.Replace(',','.');
			}
		}

		///<summary>Runs the query and returns the result.  An improvement would be to pass in the query, but no time to rewrite.</summary>
		public DataTable GetTempTable(){
			return Reports.GetTable(Query);
		}

		///<summary>Assumes that the columns will be set in sequential order.  Automatically runs the language translation.</summary>
		public void SetColumn(object sender,int idx,string colCaption,int colWidth) {
			SetColumn(sender,idx,colCaption,colWidth,HorizontalAlignment.Left);
		}

		///<summary>Assumes that the columns will be set in sequential order.  Automatically runs the language translation.</summary>
		public void SetColumn(object sender,int idx,string colCaption,int colWidth,HorizontalAlignment colAlign) {
			this.colCaption[idx]=Lan.g(sender,colCaption);
			this.colWidth[idx]=colWidth;
			this.colPos[idx+1]=this.colPos[idx]+colWidth;
			this.colAlign[idx]=colAlign;
		}

		///<summary>This is an alternative to SetColumn.  Used when we want to set absolute column positions instead of widths.  Mostly used for older reports so that we don't have to sit down with a calculator and refigure each column width.  SetColumn is the newer better way to do it.  When using this, set the RIGHT position of each column.  Column 1 always has a left position of 0.</summary>
		public void SetColumnPos(object sender,int idx,string colCaption,int colPos) {
			SetColumnPos(this,idx,colCaption,colPos,HorizontalAlignment.Left);
		}

		///<summary>This is an alternative to SetColumn.  Used when we want to set absolute column positions instead of widths.  Mostly used for older reports so that we don't have to sit down with a calculator and refigure each column width.  SetColumn is the newer better way to do it.  When using this, set the RIGHT position of each column.  Column 1 always has a left position of 0.</summary>
		public void SetColumnPos(object sender,int idx,string colCaption,int colPos,HorizontalAlignment colAlign) {
			this.colCaption[idx]=Lan.g(sender,colCaption);
			this.colPos[idx+1]=colPos;
			this.colWidth[idx]=this.colPos[idx+1]-this.colPos[idx];
			this.colAlign[idx]=colAlign;
		}

	}


}













