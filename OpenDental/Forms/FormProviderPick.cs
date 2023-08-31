using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
///<summary>Pick a provider from the list.</summary>
	public partial class FormProviderPick:FormODBase {
		//private bool changed;
		//private User user;
		//private DataTable table;
		///<summary>This can be set ahead of time to preselect a provider.  After closing with OK, this will have the selected provider number.</summary>
		public long ProvNumSelected;
		private List<SchoolClass> _listSchoolClasses;
		public bool IsStudentPicker=false;
		///<summary>Setting to true will show a none button and will allow 0 to be returned in the SelectedProvNum variable.  It will be -1 if the user cancels out of the window.</summary>
		public bool IsNoneAvailable=false;
		///<summary>Will be set to a specific list of providers passed in.  Will be null if no defined list of providers is desired.</summary>
		private List<Provider> _listProviders;
		///<summary>Will enable the checkbox that shows all non-hidden providers regardless of schedule, clinic, or what _listProviders was set to initially</summary>
		public bool IsShowAllAvailable=false;
		
		///<summary></summary>
		public FormProviderPick(List<Provider> listProviders=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listProviders=listProviders;
		}

		private void FormProviderSelect_Load(object sender, System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textFName,textLName,textProvNum);
			checkShowAll.Visible=IsShowAllAvailable;
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				groupDentalSchools.Visible=false;
			}
			else if(IsStudentPicker) {
				this.Text="Student Picker";
				gridMain.Title="Students";
				_listSchoolClasses=SchoolClasses.GetDeepCopy();
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					comboClass.Items.Add(_listSchoolClasses[i].GradYear+" "+_listSchoolClasses[i].Descript);
				}
				if(comboClass.Items.Count>0) {
					comboClass.SelectedIndex=0;
				}
			}
			else {
				comboClass.Visible=false;
				labelClass.Visible=false;
			}
			FillGrid();
			if(_listProviders!=null) {
				for(int i=0;i<_listProviders.Count;i++) {
					if(_listProviders[i].ProvNum==ProvNumSelected) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
			else if(ProvNumSelected!=0) {
				gridMain.SetSelected(Providers.GetIndex(ProvNumSelected),true);
			}
			butSelectNone.Visible=IsNoneAvailable;
			if(IsNoneAvailable) {
				//Default value for the selected provider when none is an option is always -1
				ProvNumSelected=-1;
			}
		}

		private void FillGrid(){
			long provNum;
			if(!long.TryParse(textProvNum.Text,out provNum)) {
				provNum=0;
			}
			long classNum=0;
			if(IsStudentPicker) {
				classNum=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			}
			List<Provider> listProviders;
			if(_listProviders!=null && !checkShowAll.Checked) {//User wants to use a specific list of providers.
				listProviders=GetFilteredProviderList(_listProviders);
			}
			else {
				listProviders=Providers.GetFilteredProviderList(provNum,textLName.Text,textFName.Text,classNum);
				listProviders=GetFilteredProviderList(listProviders);//Filters the list of all providers. 
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				col=new GridColumn(Lan.g("TableProviders","ProvNum"),60);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableProviders","Abbrev"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","LName"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","FName"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listProviders.Count;i++) {
				if(IsStudentPicker && listProviders[i].SchoolClassNum==0) {
					continue;
				}
				row=new GridRow();
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(listProviders[i].ProvNum.ToString());
				}
				row.Cells.Add(listProviders[i].Abbr);
				row.Cells.Add(listProviders[i].LName);
				row.Cells.Add(listProviders[i].FName);
				row.Tag=listProviders[i].ProvNum;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		/// <summary>Filters the list of providers by search terms and returns the filtered list. If used outside of FormProviderPick, make sure your list of providers isn't null before calling this method.</summary>
		private List<Provider> GetFilteredProviderList(List<Provider> listProviders) {
			if(string.IsNullOrWhiteSpace(textFilter.Text)) { 
				return listProviders;	
			}
			List<Provider> listProvidersFiltered=new List<Provider>();
			for(int i=0;i<listProviders.Count;i++) {
				if(listProviders[i].FName==null || listProviders[i].LName==null || listProviders[i].Abbr==null) {
					continue;
				}
				if(listProviders[i].FName.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim()) ||
					listProviders[i].LName.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim()) ||
					listProviders[i].Abbr.ToUpper().Trim().Contains(textFilter.Text.ToUpper().Trim())) 
				{
					listProvidersFiltered.Add(listProviders[i]);
				}
			}
			listProvidersFiltered=listProvidersFiltered
				.OrderByDescending(x=>x.FName.ToUpper().Trim().StartsWith(textFilter.Text.ToUpper().Trim()))
				.ThenByDescending(x=>x.LName.ToUpper().Trim().StartsWith(textFilter.Text.ToUpper().Trim()))
				.ThenByDescending(x=>x.Abbr.ToUpper().Trim().StartsWith(textFilter.Text.ToUpper().Trim())).ToList();
			return listProvidersFiltered;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ProvNumSelected=PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString());
			DialogResult=DialogResult.OK;
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkShowAll_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textFilter_Enter(object sender,EventArgs e) {
			textFilter.SelectAll();
		}

		private void textFilter_DoubleClick(object sender,EventArgs e) {
			textFilter.SelectAll();
		}
		
		private void textFilter_TextChanged(object sender,EventArgs e) {
			FillGrid();
		} 

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			ProvNumSelected=PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butSelectNone_Click(object sender,EventArgs e) {
			ProvNumSelected=0;
			DialogResult=DialogResult.OK;
		}
		



	

	}
}
