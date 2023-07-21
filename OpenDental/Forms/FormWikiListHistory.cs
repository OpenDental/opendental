using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListHistory:FormODBase {
		///<summary>Set from outside this form to load all appropriate data into the form during Form_Load().</summary>
		public string WikiListName;
		public bool IsReverted;

		public FormWikiListHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiListHistory_Load(object sender,EventArgs e) {
			FillGridMain();
			if(gridMain.ListGridRows.Count>0) {
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
				gridMain.ScrollToEnd();
			}
			Text="Wiki List History - "+WikiListName;
			gridOld.Title="Old Revision";
			gridCur.Title="Current Revision";
			FillGridOld();
			FillGridCur();
		}

		private void FormWikiListHistory_Resize(object sender,EventArgs e) {
			int intWorkingWidth=butRevert.Left-gridMain.Right-18;//18 for 3x6 px between gridMain-gridOld, gridOld-gridCur, and gridCur-butRevert
			gridOld.Width=intWorkingWidth/2;
			gridCur.Left=gridOld.Right+6;
			gridCur.Width=intWorkingWidth-gridOld.Width;
		}

		/// <summary></summary>
		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.ListGridRows.Clear();
			gridMain.Columns.AddRange(new[] { new GridColumn(Lan.g(this,"User"),70),new GridColumn(Lan.g(this,"Saved"),80) });
			gridMain.ListGridRows.AddRange(WikiListHists.GetByNameNoContent(WikiListName)
				.Select(x => new GridRow(Userods.GetName(x.UserNum),x.DateTimeSaved.ToString()) { Tag=x }));
			gridMain.EndUpdate();
		}

		/// <summary></summary>
		private void FillGridOld() {
			gridOld.BeginUpdate();
			gridOld.Columns.Clear();
			gridOld.ListGridRows.Clear();
			if(gridMain.GetSelectedIndex()<0) {
				gridOld.EndUpdate();
				return;
			}
			if(string.IsNullOrEmpty(gridMain.SelectedTag<WikiListHist>()?.ListHeaders)) {
				gridMain.SelectedGridRows[0].Tag=WikiListHists.SelectOne(gridMain.SelectedTag<WikiListHist>()?.WikiListHistNum??0);
			}
			List<WikiListHeaderWidth> listWikiListHeaderWidths=WikiListHeaderWidths.GetFromListHist(gridMain.SelectedTag<WikiListHist>());
			DataTable table=new DataTable();
			using StringReader stringReader=new StringReader(gridMain.SelectedTag<WikiListHist>().ListContent);
			using XmlReader xmlReader=XmlReader.Create(stringReader);
			try {
				table.ReadXml(xmlReader);
				xmlReader.Close();
			}
			catch(Exception) {
				MsgBox.Show(this,"Corruption detected in the Old Revision table.  Partial data will be displayed.  Please call us for support.");
				gridOld.EndUpdate();
				return;
			}
			List<DataColumn> listDataColumns=table.Columns.OfType<DataColumn>().ToList();
			for(int i=0;i<listDataColumns.Count;i++) {
				GridColumn col=new GridColumn();
				col.Heading=listDataColumns[i].ColumnName;
				WikiListHeaderWidth wikiListHeaderWidth=listWikiListHeaderWidths.Find(x=>x.ColName==listDataColumns[i].ColumnName);
				col.ColWidth=100;
				if(wikiListHeaderWidth!=null) {
					col.ColWidth=wikiListHeaderWidth.ColWidth;
				}
				gridOld.Columns.Add(col);
			}			
			for(int i=0;i<table.Rows.Count ;i++) {
				GridRow row = new GridRow();
				for(int j = 0;j<table.Columns.Count;j++) {
					row.Cells.Add(table.Rows[i][j].ToString());
				}
				gridOld.ListGridRows.Add(row);
			}
			gridOld.EndUpdate();
		}

		/// <summary></summary>
		private void FillGridCur() {
			gridCur.BeginUpdate();
			gridCur.Columns.Clear();
			gridCur.ListGridRows.Clear();
			List<WikiListHeaderWidth> listWikiListHeaderWidths=WikiListHeaderWidths.GetForList(WikiListName);
			using DataTable table=WikiLists.GetByName(WikiListName);
			List<DataColumn> listDataColumns=table.Columns.OfType<DataColumn>().ToList();
			for(int i=0;i<listDataColumns.Count;i++) {
				GridColumn col=new GridColumn();
				col.Heading=listDataColumns[i].ColumnName;
				WikiListHeaderWidth wikiListHeaderWidth=listWikiListHeaderWidths.Find(x=>x.ColName==listDataColumns[i].ColumnName);
				col.ColWidth=100;
				if(wikiListHeaderWidth!=null) {
					col.ColWidth=wikiListHeaderWidth.ColWidth;
				}
				gridCur.Columns.Add(col);
			}
			for(int i=0;i<table.Rows.Count;i++) {
				GridRow row=new GridRow();
				for(int j=0;j<table.Columns.Count;j++) {
					row.Cells.Add(table.Rows[i][j].ToString());
				}
				gridCur.ListGridRows.Add(row);
			}
			gridCur.EndUpdate();
		}

		private void gridMain_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			FillGridOld();
			gridMain.Focus();
		}

		private void butRevert_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Revert list to currently selected revision?")) {
				return;
			}
			try {
				WikiListHists.RevertFrom(gridMain.SelectedTag<WikiListHist>(),Security.CurUser.UserNum);
			}
			catch(Exception) {
				MsgBox.Show(this,"There was an error when trying to revert changes.  Please call us for support.");
				return;
			}
			FillGridMain();
			gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);//select the new revision.
			gridMain.ScrollToEnd();//in case there are LOTS of revisions. Should this go in the fill grid code? 
			FillGridOld();
			FillGridCur();
			gridMain.Focus();
			IsReverted=true;
		}

	}
}