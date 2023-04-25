using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiTableViewEdit:FormODBase {
		public WikiView WikiViewCur;
		///<summary>Passed in.  Some will show in the listbox and some in the grid.</summary>
		public List<string> ListColsAll;
		public bool IsNew;

		public FormWikiTableViewEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiTableViewEdit_Load(object sender,EventArgs e) {
			textViewName.Text=WikiViewCur.ViewName;
			FillLists(false);
		}

		///<summary>Except on startup, the existing orderby should be preserved.</summary>
		private void FillLists(bool preserveOrderBy) {
			if(preserveOrderBy) {
				WikiViewCur.OrderBy="";
				if(comboOrderBy.SelectedIndex>0) {
					WikiViewCur.OrderBy=comboOrderBy.SelectedItem.ToString();
				}
			}
			//------------------------------
			List<string> listColsAvail=new List<string>();
			for(int a=0;a<ListColsAll.Count;a++) {
				if(!WikiViewCur.ListColumns.Contains(ListColsAll[a])) {
					listColsAvail.Add(ListColsAll[a]);
				}
			}
			listAvail.Items.Clear();
			listAvail.Items.AddList(listColsAvail,x => x.ToString());
			listShowing.Items.Clear();
			listShowing.Items.AddList(WikiViewCur.ListColumns,x => x.ToString());
			comboOrderBy.Items.Clear();
			comboOrderBy.Items.Add(Lan.g(this,"none"));
			for(int i=0;i<WikiViewCur.ListColumns.Count;i++) {
				comboOrderBy.Items.Add(WikiViewCur.ListColumns[i]);
				if(WikiViewCur.OrderBy==WikiViewCur.ListColumns[i]) {
					comboOrderBy.SelectedIndex=i+1;
				}
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(listAvail.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item on the left first.");
				return;
			}
			//It should to be put back in the same order as the CollsAll, although it's not critical.  Columns may still get out of order later.
			List<string> listColsShowingTemp=new List<string>(WikiViewCur.ListColumns);
			listColsShowingTemp.Add(listAvail.GetSelected<string>());
			WikiViewCur.ListColumns=new List<string>();
			for(int i=0;i<ListColsAll.Count;i++) {
				if(listColsShowingTemp.Contains(ListColsAll[i])) {
					WikiViewCur.ListColumns.Add(ListColsAll[i]);
				}
			}
			FillLists(true);
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(listShowing.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item on the right first.");
				return;
			}
			WikiViewCur.ListColumns.RemoveAt(listShowing.SelectedIndex);
			FillLists(true);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this view?")) {
				return;
			}
			WikiViewCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textViewName.Text=="") {
				MsgBox.Show(this,"Please enter a view name first.");
				return;
			}
			if(WikiViewCur.ListColumns.Count==0) {
				MsgBox.Show(this,"Please select columns to show first.");
				return;
			}
			WikiViewCur.ViewName=textViewName.Text;
			WikiViewCur.OrderBy="";
			if(comboOrderBy.SelectedIndex>0) {
				WikiViewCur.OrderBy=comboOrderBy.SelectedItem.ToString();
			}
			//WikiViewCur will be read and processed from the calling form.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}

	///<summary>Not a db table.  This is stored in a wikipage as xml markup.  Pulled out for manipulation, and then stored back in the markup.</summary>
	public class WikiView {
		public string ViewName;
		public string OrderBy;
		///<summary>The column names must exactly match existing column name.</summary>
		public List<string> ListColumns;
	}
}