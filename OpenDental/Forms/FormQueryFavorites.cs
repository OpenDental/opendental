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
		private List<UserQuery> _listUserQueries;
		public UserQuery UserQueryCur;
		public bool IsBlockingDangerous;

		///<summary></summary>
		public FormQueryFavorites() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQueryFormulate_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.UserQueryAdmin,true)) { //disable controls for users without permission
				butEdit.Enabled=false;
				butDelete.Enabled=false;
				butAdd.Enabled=false;
			}
			//hide the query text by default.
			Width=LayoutManager.Scale(500);
			splitContainer.Panel2Collapsed=true;
			FillGrid();
			textSearch.Select();
		}

		private void FormQueryFavorites_SizeChanged(object sender,EventArgs e) {
			splitContainer.SplitterDistance=LayoutManager.Scale(454);
		}

		private void FillGrid(bool doRefreshList=true,bool isScrollToSelection=true) {
			if(doRefreshList) {
				_listUserQueries=UserQueries.GetDeepCopy();
			}
			bool isAuthorized=Security.IsAuthorized(EnumPermType.UserQueryAdmin,true);
			string[] stringArraySearchTerms = Regex.Split(textSearch.Text,@"\W");//matches any non-word character
			//get all queries that contain ALL of the search terms entered, either in the query text or the query description.
			List<UserQuery> listUserQueryDisplay = _listUserQueries
				.Where(x => stringArraySearchTerms.All(y => 
					x.QueryText.ToLowerInvariant().Contains(y.ToLowerInvariant()) || x.Description.ToLowerInvariant().Contains(y.ToLowerInvariant())
				)).ToList();
			//attempt to preserve the currently selected query.
			long selectedQueryNum=0;
			if(gridMain.GetSelectedIndex() != -1) {
				selectedQueryNum=gridMain.SelectedTag<UserQuery>().QueryNum;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Query"),350));
			if(isAuthorized) {
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Released"),55,HorizontalAlignment.Center));
			}
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listUserQueryDisplay.Count;i++) {
				if(!isAuthorized && !listUserQueryDisplay[i].IsReleased) {//not released for reg users
					continue;
				}
				if(IsBlockingDangerous){
					if(!UserQueries.ValidateQueryForMassEmail(listUserQueryDisplay[i].QueryText)){
						continue;
					}
				}
				GridRow row = new GridRow();
				row.Cells.Add(listUserQueryDisplay[i].Description);
				if(isAuthorized) {
					row.Cells.Add(listUserQueryDisplay[i].IsReleased ? "X" : "");
				}
				row.Tag = listUserQueryDisplay[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			int indexSelected=gridMain.ListGridRows.Select(x => (UserQuery)x.Tag).ToList().FindIndex(y => y.QueryNum==selectedQueryNum);
			if(indexSelected>-1) {
				gridMain.SetSelected(indexSelected,true);
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
			if(gridMain.GetSelectedIndex() == -1){
				return;
			}
			UserQueryCur = (UserQuery)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			textQuery.Text=UserQueryCur.QueryText.Replace("\n","\r\n");
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==1) {//Released Column
				UserQuery userQuery=gridMain.SelectedTag<UserQuery>();
				userQuery.IsReleased=(!userQuery.IsReleased);
				UserQueries.Update(userQuery);
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
			if(UserQueryCur.IsPromptSetup && UserQueries.ParseSetStatements(UserQueryCur.QueryText).Count > 0) {
				//if the user is not a query admin, they will not have the ability to edit 
				//the query before it is run, so show them the SET statement edit window.
				using FormQueryParser formQueryParser = new FormQueryParser(UserQueryCur);
				formQueryParser.ShowDialog();
				if(formQueryParser.DialogResult==DialogResult.OK) {
					FormUserQuery formUserQuery = new FormUserQuery(UserQueryCur.QueryText,submitQueryOnLoad:true);
					DialogResult=DialogResult.OK;
				}
				return;
			}
			//user has permission to edit the query, so just run the query.
			DialogResult=DialogResult.OK;
		}

		private void butEdit_Click(object sender,EventArgs e) {
			//button is disabled for users without Query Admin permission.
			if(UserQueryCur==null) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			using FormQueryEdit formQueryEdit=new FormQueryEdit();
			formQueryEdit.UserQueryCur=UserQueryCur;
			formQueryEdit.UserQueryCur.IsNew=false;
			formQueryEdit.UserQueryCur.IsPromptSetup=false;
			formQueryEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//button is disabled for users without Query Admin permission.
			using FormQueryEdit formQueryEdit=new FormQueryEdit();
			formQueryEdit.UserQueryCur=new UserQuery();
			formQueryEdit.UserQueryCur.IsNew=true;
			formQueryEdit.UserQueryCur.IsPromptSetup=true;
			formQueryEdit.UserQueryCur.DefaultFormatRaw=PrefC.GetBool(PrefName.UserQueryDefaultRaw);
			formQueryEdit.ShowDialog();
			if(formQueryEdit.DialogResult==DialogResult.OK){
				FillGrid();
			}
		}

		private void butShowHide_Click(object sender,EventArgs e) {
			splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
			splitContainer.SplitterDistance=LayoutManager.Scale(454);
			if(splitContainer.Panel2Collapsed) {
				Width = LayoutManager.Scale(500);
				butShowHide.Text=Lan.g(this,"Show Text") +" >";
				return;
			}
			Width = LayoutManager.Scale(1100);
			butShowHide.Text=Lan.g(this,"Hide Text") + " <";
			
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
			RefreshQueryCur();
			if(UserQueryCur == null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(UserQueryCur.IsPromptSetup && UserQueries.ParseSetStatements(UserQueryCur.QueryText).Count > 0) {
				//if the user is not a query admin, they will not have the ability to edit 
				//the query before it is run, so show them the SET statement edit window.
				using FormQueryParser formQueryParser = new FormQueryParser(UserQueryCur);
				formQueryParser.ShowDialog();
				if(formQueryParser.DialogResult==DialogResult.OK) {
					FormUserQuery formUserQuery = new FormUserQuery(UserQueryCur.QueryText,submitQueryOnLoad:true);
					DialogResult=DialogResult.OK;
				}
				return;
			}
			//user has permission to edit the query, so just run the query.
			DialogResult=DialogResult.OK;
		}

	}
}