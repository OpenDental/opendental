using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	///<summary>This window was created as part of a concept that spawned from request #4221.
	///Nathan wants to keep this window around for future versions when we introduce more "things" that need action to be taken by users.
	///It will serve as a window to show (one item in the grid at a time) that they can double click on to get to other sections of the program.</summary>
	public partial class FormActionNeeded:FormODBase {
		//private List<ActionObj> _listActionObjs=new List<ActionObj>();

		///<summary></summary>
		public FormActionNeeded() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormActionNeeded_Load(object sender,EventArgs e) {
			FillActionObjs();
			FillGrid();
		}

		private void FillActionObjs() {
			//_listActionObjs.Clear();
			//here will be where we can fill the action objs list with random things that "need action taken"
		}

		private void FillGrid() {
			//gridMain.BeginUpdate();
			//gridMain.Columns.Clear();
			//ODGridColumn col=new ODGridColumn(Lan.g("TableActionNeeded","Type"),90);
			//gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("TableActionNeeded","Description"),0);
			//gridMain.Columns.Add(col);
			//gridMain.Rows.Clear();
			//ODGridRow row;
			//for(int i=0;i<_listActionObjs.Count;i++) {
			//	row=new ODGridRow();
			//	row.Cells.Add(_listActionObjs[i].ActionType.ToString());
			//	row.Cells.Add(_listActionObjs[i].Description);
			//	gridMain.Rows.Add(row);
			//}
			//gridMain.EndUpdate();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

	//public class ActionObj {
	//	public long FKey;
	//	public ActionTypes ActionType;
	//	public string Description;

	//	public ActionObj(long fKey,ActionTypes actionType,string description) {
	//		FKey=fKey;
	//		ActionType=actionType;
	//		Description=description;
	//	}
	//}
}