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
		public List<string> ColsAll;
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
			List<string> ColsAvail=new List<string>();
			for(int a=0;a<ColsAll.Count;a++) {
				if(!WikiViewCur.Columns.Contains(ColsAll[a])) {
					ColsAvail.Add(ColsAll[a]);
				}
			}
			listAvail.Items.Clear();
			listAvail.Items.AddList(ColsAvail,x => x.ToString());
			listShowing.Items.Clear();
			listShowing.Items.AddList(WikiViewCur.Columns,x => x.ToString());
			comboOrderBy.Items.Clear();
			comboOrderBy.Items.Add(Lan.g(this,"none"));
			for(int i=0;i<WikiViewCur.Columns.Count;i++) {
				comboOrderBy.Items.Add(WikiViewCur.Columns[i]);
				if(WikiViewCur.OrderBy==WikiViewCur.Columns[i]) {
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
			List<string> colsShowingTemp=new List<string>(WikiViewCur.Columns);
			colsShowingTemp.Add(listAvail.GetSelected<string>());
			WikiViewCur.Columns=new List<string>();
			for(int i=0;i<ColsAll.Count;i++) {
				if(colsShowingTemp.Contains(ColsAll[i])) {
					WikiViewCur.Columns.Add(ColsAll[i]);
				}
			}
			FillLists(true);
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(listShowing.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item on the right first.");
				return;
			}
			WikiViewCur.Columns.RemoveAt(listShowing.SelectedIndex);
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
			if(WikiViewCur.Columns.Count==0) {
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
		public List<string> Columns;
	}
}