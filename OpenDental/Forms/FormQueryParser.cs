using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using System.Text.RegularExpressions;
using System.Drawing;

namespace OpenDental {
	public partial class FormQueryParser:FormODBase {
		private UserQuery _userQuery;
		private UserQuery _userQueryOld;

		public FormQueryParser(UserQuery userQuery) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_userQuery=userQuery;
			_userQueryOld=userQuery.Copy();
			splitContainer.Panel2Collapsed=true;
			this.Text +=" - " + _userQuery.Description;
		}

		private void FormQueryParser_Load(object sender,EventArgs e) {
			Height = 325;//hide the query text by default.
			Action action = () => {
				_userQuery.QueryText=textQuery.Text;
				//No cell is currently selected, so the user must be typing in textQuery.  If a cell is selected, we got here because we made a change in a 
				//cell.  So we want to maintain our selection.
				bool isTypingInQuery=(textQuery.Focused || gridMain.SelectedCell.Y==-1);
				FillGrid(isTypingInQuery);
			};
			SetFilterControlsAndAction(action,(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,textQuery);
			textQuery.Text=_userQuery.QueryText;
			FillGrid();
			if(!Security.IsAuthorized(EnumPermType.UserQueryAdmin,true)) {
				textQuery.ReadOnly=true;
			}
		}

		///<summary>This method takes care of parsing the query by pulling out SET statements and finding the variables with their assigned values.
		///Puts all of this information into the grid.</summary>
		private void FillGrid(bool isTypingText = false) {
			Point pointSelectedCell = gridMain.SelectedCell;
			List<string> listSetStmts = UserQueries.ParseSetStatements(_userQuery.QueryText);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Variable"),200));
			gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Value"),200,true));
			gridMain.ListGridRows.Clear();
			for(int i=0;i< listSetStmts.Count;i++) { //for each SET statement
				List<QuerySetStmtObject> listQuerySetStmtObjects = UserQueries.GetListQuerySetStmtObjs(listSetStmts[i]); //find the variable name
				for(int j=0; j<listQuerySetStmtObjects.Count; j++) {
					GridRow row = new GridRow();
					row.Cells.Add(listQuerySetStmtObjects[j].Variable);
					row.Cells.Add(listQuerySetStmtObjects[j].Value);
					row.Tag=listQuerySetStmtObjects[j];
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
			if(isTypingText){
				return;
			}
			try {
				gridMain.SetSelected(pointSelectedCell);
			}
			catch {
				//suppress if the row doesn't exist (such as filling the grid for the first time)
			}
			
		}
		
		///<summary>When a row is double clicked, bring up an input box that allows the user to change the variable's value.</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			QuerySetStmtObject querySetStmtObject = (QuerySetStmtObject)gridMain.SelectedGridRows[0].Tag;
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.TextBox;
			inputBoxParam.LabelText=Lan.g(this,"Set value for")+" "+ querySetStmtObject.Variable;
			inputBoxParam.Text=querySetStmtObject.Value;
			InputBox inputBox=new InputBox(inputBoxParam);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			string stmtOld = querySetStmtObject.Stmt;
			//Regular expression for the expression @Variable = Value.
			Regex regex=new Regex(Regex.Escape(querySetStmtObject.Variable)+@"\s*=\S*?[;|,]*"+Regex.Escape(querySetStmtObject.Value));
			string stmtNew=regex.Replace(stmtOld,querySetStmtObject.Variable+"="+inputBox.StringResult,1);
			_userQuery.QueryText=_userQuery.QueryText.Replace(stmtOld,stmtNew);
			if(stmtOld == stmtNew) {
				return; //don't bother refilling the grid if the value didn't change.
			}
			textQuery.Text=_userQuery.QueryText;
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			SelectTextForSelectedRow();
		}

		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			SelectTextForSelectedRow();
		}

		private void SelectTextForSelectedRow() {
			QuerySetStmtObject querySetStmtObject = (QuerySetStmtObject)gridMain.SelectedGridRows[0].Tag;
			int startidx = textQuery.Text.IndexOf(querySetStmtObject.Stmt);
			if(startidx == -1) {
				//the query object does not have comments in it, while the text area does.
				//this can cause finding the index of the set statement to fail if the user 
				//has comments contained within their set statements (which should never happen unless the query is malformed).
				//In this case, we'll just suppress any failures.
				return;
			}
			textQuery.Select(startidx,querySetStmtObject.Stmt.Length);
		}

		private void butShowHide_Click(object sender,EventArgs e) {
			splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
			if(splitContainer.Panel2Collapsed) {
				butShowHide.Text=Lan.g(this,"Show Text");
				Height = 325;
				this.butShowHide.Image = global::OpenDental.Properties.Resources.arrowDownTriangle;
				LayoutManager.LayoutControlBoundsAndFonts(splitContainer);
				return;
			}
			butShowHide.Text=Lan.g(this,"Hide Text");
			Height = 720;
			butShowHide.Image = global::OpenDental.Properties.Resources.arrowUpTriangle;
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			QuerySetStmtObject querySetStmtObject;
			GridRow rowLeaving;
			try{
				//Cannot use gridMain.SelectedGridRows due to leave being hit multiple times by input texboxes.
				//Base grid logic fires this leave function multiple times.
				rowLeaving=gridMain.ListGridRows[e.Row];
				querySetStmtObject=(QuerySetStmtObject)rowLeaving.Tag;
			}
			catch {//Has occured when user types SHIFT+ENTER on the keyboard.
				return;
			}
			string stmtOld = querySetStmtObject.Stmt;
			string varOld = querySetStmtObject.Variable;
			string valOld = querySetStmtObject.Value;
			string valNew = rowLeaving.Cells[1].Text.ToString();
			if(UserQueries.SplitQuery(valNew,true,";").Count > 1) {
				Point pointSelectedCell = gridMain.SelectedCell;
				MsgBox.Show(this,"You may not include semicolons in the value text. Please remove all semicolons.");
				gridMain.SelectedGridRows[0].Cells[1].Text = valOld;
				gridMain.SetSelected(pointSelectedCell); //this just refreshes the cell that is being left. Is there an easy way to cancel the CellLeave action?
				return;
			}
			if(valOld == valNew) {
				return; //don't bother doing any of the logic below if nothing changed.
			}
			if(HasQuotes(valOld)) {
				valNew = "'"+valNew.Trim().Trim('\"').Trim('\'')+"'";
			}
			//Regular expression for the expression @Variable = Value.
			Regex regex=new Regex(Regex.Escape(varOld)+@"\s*=\S*?[;|,]*"+Regex.Escape(valOld));
			string stmtNew=regex.Replace(stmtOld,varOld+"="+valNew);
			_userQuery.QueryText=regex.Replace(_userQuery.QueryText,varOld+"="+valNew,1);
			textQuery.Text=_userQuery.QueryText;
			querySetStmtObject.Stmt=stmtNew;
			querySetStmtObject.Value=valNew;
			gridMain.ListGridRows.OfType<GridRow>().Where(x => ((QuerySetStmtObject)x.Tag).Stmt == stmtOld).ToList().ForEach(y => {
				((QuerySetStmtObject)y.Tag).Stmt = stmtNew;
			});
		}

		///<summary>Returns true if the string starts and ends with quotes.</summary>
		private bool HasQuotes(string val) {
			return (val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("\'") && val.EndsWith("\'"));
		}

		private void butOk_Click(object sender,EventArgs e) {
			this.DialogResult=DialogResult.OK;
		}

		private void FormQueryParser_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult != DialogResult.OK) {
				_userQuery.QueryText = _userQueryOld.QueryText; //change the text back to what it used to be if the users cancels.
				return;
			}
			_userQuery.QueryText=textQuery.Text;
		}

	}
}