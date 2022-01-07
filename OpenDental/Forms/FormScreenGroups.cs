using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormScreenGroups:FormODBase {
		private List<ScreenGroup> _listScreenGroups;
		private DateTime _dateCur;

		///<summary></summary>
		public FormScreenGroups()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormScreenings_Load(object sender, System.EventArgs e) {
			_dateCur=DateTime.Today;
			textDateFrom.Text=DateTime.Today.ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		private void FillGrid() {
			_listScreenGroups=ScreenGroups.Refresh(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text));
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),140);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(ScreenGroup screenGroup in _listScreenGroups) {
				row=new GridRow();
				row.Cells.Add(screenGroup.SGDate.ToShortDateString());
				row.Cells.Add(screenGroup.Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormScreenGroupEdit FormSG=new FormScreenGroupEdit(_listScreenGroups[gridMain.GetSelectedIndex()]);
			FormSG.ShowDialog();
			FillGrid();
		}

		private void textDateFrom_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(textDateFrom.Text=="") {
				return;
			}
			try {
				DateTime.Parse(textDateFrom.Text);
			}
			catch {
				MessageBox.Show("Date invalid");
				e.Cancel=true;
			}
		}

		private void textDateTo_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(textDateTo.Text=="") {
				return;
			}
			try {
				DateTime.Parse(textDateTo.Text);
			}
			catch {
				MessageBox.Show("Date invalid");
				e.Cancel=true;
			}
		}

		private void butRefresh_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			ScreenGroup screenGroup=new ScreenGroup();
			if(_listScreenGroups.Count!=0) {
				screenGroup=_listScreenGroups[_listScreenGroups.Count-1];//'remembers' the last entry
			}
			screenGroup.SGDate=DateTime.Today;//except date will be today
			screenGroup.IsNew=true;
			using FormScreenGroupEdit FormSG=new FormScreenGroupEdit(screenGroup);
			FormSG.ShowDialog();
			FillGrid();
		}

		private void butToday_Click(object sender,EventArgs e) {
			_dateCur=DateTime.Today;
			textDateFrom.Text=DateTime.Today.ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			_dateCur=_dateCur.AddDays(-1);
			textDateFrom.Text=_dateCur.ToShortDateString();
			textDateTo.Text=_dateCur.ToShortDateString();
		}

		private void butRight_Click(object sender,EventArgs e) {
			_dateCur=_dateCur.AddDays(1);
			textDateFrom.Text=_dateCur.ToShortDateString();
			textDateTo.Text=_dateCur.ToShortDateString();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1){
				MessageBox.Show("Please select one item first.");
				return;
			}
			ScreenGroup screenGroupCur=_listScreenGroups[gridMain.GetSelectedIndex()];
			List<OpenDentBusiness.Screen> listScreens=Screens.GetScreensForGroup(screenGroupCur.ScreenGroupNum);
			if(listScreens.Count>0) {
				MessageBox.Show("Not allowed to delete a screening group with items in it.");
				return;
			}
			ScreenGroups.Delete(screenGroupCur);
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		


	}
}





















