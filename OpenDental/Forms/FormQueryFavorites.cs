using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenDental{
///<summary></summary>
	public partial class FormQueryFavorites:FormODBase {
		private List<UserQuery> _listQueries;
		public UserQuery UserQueryCur;

		///<summary></summary>
		public FormQueryFavorites() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQueryFormulate_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQueryAdmin,true)) { //disable controls for users without permission
				butEdit.Enabled=false;
				butDelete.Enabled=false;
				butAdd.Enabled=false;
			}
			//hide the query text by default.
			Width=LayoutManager.Scale(500);
			splitContainer1.Panel2Collapsed=true;
			FillGrid();
			textSearch.Select();
		}

		private void FormQueryFavorites_SizeChanged(object sender,EventArgs e) {
			splitContainer1.SplitterDistance=LayoutManager.Scale(454);
		}

		private void FillGrid(bool refreshList=true,bool isScrollToSelection=true) {
			if(refreshList) {
				_listQueries=UserQueries.GetDeepCopy();
			}
			string[] strSearchTerms = Regex.Split(textSearch.Text,@"\W");//matches any non-word character
			//get all queries that contain ALL of the search terms entered, either in the query text or the query description.
			List<UserQuery> listDisplayQueries = _listQueries
				.Where(x => strSearchTerms.All(y => 
					x.QueryText.ToLowerInvariant().Contains(y.ToLowerInvariant()) || x.Description.ToLowerInvariant().Contains(y.ToLowerInvariant())
				)).ToList();
			//attempt to preserve the currently selected query.
			long selectedQueryNum=0;
			if(gridMain.GetSelectedIndex() != -1) {
				selectedQueryNum=gridMain.SelectedTag<UserQuery>().QueryNum;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Query"),350));
			if(Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Released"),55,HorizontalAlignment.Center));
			}
			gridMain.ListGridRows.Clear();
			foreach(UserQuery queryCur in listDisplayQueries) {
				if(!Security.IsAuthorized(Permissions.UserQueryAdmin,true) && !queryCur.IsReleased) {//not released for reg users
					continue;
				}
				GridRow row = new GridRow();
				row.Cells.Add(queryCur.Description);
				if(Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
					row.Cells.Add(queryCur.IsReleased ? "X" : "");
				}
				row.Tag = queryCur;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			int selectedIdx=gridMain.ListGridRows.Select(x => (UserQuery)x.Tag).ToList().FindIndex(y => y.QueryNum==selectedQueryNum);
			if(selectedIdx>-1) {
				gridMain.SetSelected(selectedIdx,true);
			}
			if(gridMain.GetSelectedIndex()==-1) {
				gridMain.SetSelected(0,true); //can handle values outside of the row count (so if there are no rows, this will not fail)
			}
			if(isScrollToSelection) {
				gridMain.ScrollToIndex(gridMain.GetSelectedIndex()); //can handle values outside of the row count
			}
			RefreshQueryCur();
		}

		///<summary>Refreshes UserQueryCur and fills the textbox.</summary>
		private void RefreshQueryCur() {
			UserQueryCur = null;
			textQuery.Text="";
			if(gridMain.GetSelectedIndex() != -1) {
				UserQueryCur = (UserQuery)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
				textQuery.Text=UserQueryCur.QueryText;
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==1) {//Released Column
				UserQuery query=gridMain.SelectedTag<UserQuery>();
				query.IsReleased=(!query.IsReleased);
				UserQueries.Update(query);
				DataValid.SetInvalid(InvalidType.UserQueries);
				FillGrid(true,false);//Results in RefreshQueryCur()
				return;
			}
			RefreshQueryCur();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(UserQueryCur == null) {
				MsgBox.Show(this,"Please select an item first."); //should never happen.
				return;
			}
			ReportSimpleGrid report=new ReportSimpleGrid();
			if(UserQueryCur.IsPromptSetup && UserQueries.ParseSetStatements(UserQueryCur.QueryText).Count > 0) {
				//if the user is not a query admin, they will not have the ability to edit 
				//the query before it is run, so show them the SET statement edit window.
				using FormQueryParser FormQP = new FormQueryParser(UserQueryCur);
				FormQP.ShowDialog();
				if(FormQP.DialogResult==DialogResult.OK) {
					report.Query=UserQueryCur.QueryText;
					DialogResult=DialogResult.OK;
				}
			}
			else {
				//user has permission to edit the query, so just run the query.
				DialogResult=DialogResult.OK;
			}
		}

		private void butEdit_Click(object sender,EventArgs e) {
			//button is disabled for users without Query Admin permission.
			if(UserQueryCur==null) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			using FormQueryEdit FormQE=new FormQueryEdit();
			FormQE.UserQueryCur=UserQueryCur;
			FormQE.IsNew=false;
			FormQE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//button is disabled for users without Query Admin permission.
			using FormQueryEdit FormQE=new FormQueryEdit();
			FormQE.IsNew=true;
			FormQE.UserQueryCur=new UserQuery();
			FormQE.ShowDialog();
			if(FormQE.DialogResult==DialogResult.OK){
				FillGrid();
			}
		}

		private void butShowHide_Click(object sender,EventArgs e) {
			splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
			splitContainer1.SplitterDistance=LayoutManager.Scale(454);
			if(splitContainer1.Panel2Collapsed) {
				Width = LayoutManager.Scale(500);
				butShowHide.Text=Lan.g(this,"Show Text") +" >";
			}
			else {
				Width = LayoutManager.Scale(1100);
				butShowHide.Text=Lan.g(this,"Hide Text") + " <";
			}

		}

		private void checkWrapText_CheckedChanged(object sender,EventArgs e) {
			textQuery.WordWrap = checkWrapText.Checked;
		}		

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillGrid(false);
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//button is disabled for users without Query Admin permission.
			if(UserQueryCur==null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Item?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			UserQueries.Delete(UserQueryCur);
			DataValid.SetInvalid(InvalidType.UserQueries);
			gridMain.SetAll(false);
			UserQueryCur=null;
			FillGrid();
			textQuery.Text="";
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(UserQueryCur == null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ReportSimpleGrid report=new ReportSimpleGrid();
			if(UserQueryCur.IsPromptSetup && UserQueries.ParseSetStatements(UserQueryCur.QueryText).Count > 0) {
				//if the user is not a query admin, they will not have the ability to edit 
				//the query before it is run, so show them the SET statement edit window.
				using FormQueryParser FormQP = new FormQueryParser(UserQueryCur);
				FormQP.ShowDialog();
				if(FormQP.DialogResult==DialogResult.OK) {
					report.Query=UserQueryCur.QueryText;
					DialogResult=DialogResult.OK;
				}
			}
			else {
				//user has permission to edit the query, so just run the query.
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	}
}
