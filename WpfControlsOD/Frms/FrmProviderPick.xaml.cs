using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
///<summary>Pick a provider from the list.</summary>
	public partial class FrmProviderPick:FrmODBase {
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
		private FilterControlsAndAction _filterControlsAndAction;
		
		///<summary></summary>
		public FrmProviderPick(List<Provider> listProviders=null) {
			InitializeComponent();
			_listProviders=listProviders;
			_filterControlsAndAction=new FilterControlsAndAction();
			_filterControlsAndAction.AddControl(textFName);
			_filterControlsAndAction.AddControl(textLName);
			_filterControlsAndAction.AddControl(textProvNum);
			_filterControlsAndAction.AddControl(comboClass);
			_filterControlsAndAction.AddControl(textFilter);
			_filterControlsAndAction.FuncDb=RefreshDBForGrid;
			_filterControlsAndAction.ActionComplete=FillGrid;
			Load+=FrmProviderSelect_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			PreviewKeyDown+=FrmProviderPick_PreviewKeyDown;
		}

		private void FrmProviderSelect_Load(object sender, System.EventArgs e) {
			Lang.F(this);
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
			List<Provider> listProviders=RefreshDBForGrid();
			FillGrid(listProviders);
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
			textFilter.Focus();
		}

		private List<Provider> RefreshDBForGrid(){
			long provNum;
			string txtProvNum="";
			Dispatcher.Invoke(()=>txtProvNum=textProvNum.Text);
			if(!long.TryParse(txtProvNum,out provNum)) {
				provNum=0;
			}
			long classNum=0;
			ComboBox comboBoxClass=null;
			Dispatcher.Invoke(()=>comboBoxClass=comboClass);
			if(IsStudentPicker) {
				classNum=_listSchoolClasses[comboBoxClass.SelectedIndex].SchoolClassNum;
			}
			List<Provider> listProviders;
			CheckBox checkBoxShowAll=null;
			Dispatcher.Invoke(()=>checkBoxShowAll=checkShowAll);
			if(_listProviders!=null && checkBoxShowAll.Checked==false) {//User wants to use a specific list of providers.
				listProviders=GetFilteredProviderList(_listProviders);
			}
			else {
				string txtLName="";
				Dispatcher.Invoke(()=>txtLName=textLName.Text);
				string txtFName="";
				Dispatcher.Invoke(()=>txtFName=textFName.Text);
				listProviders=Providers.GetFilteredProviderList(provNum,txtLName,txtFName,classNum);
				listProviders=GetFilteredProviderList(listProviders);//Filters the list of all providers. 
			}
			return listProviders;
		}

		private void FillGrid(object data){
			List<Provider> listProviders=(List<Provider>)data;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				col=new GridColumn(Lang.g("TableProviders","ProvNum"),60);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lang.g("TableProviders","Abbrev"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lang.g("TableProviders","LName"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lang.g("TableProviders","FName"),100);
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

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			ProvNumSelected=PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString());
			IsDialogOK=true;
		}

		private void checkShowAll_Click(object sender,EventArgs e) {
			List<Provider> listProviders=RefreshDBForGrid();
			FillGrid(listProviders);
		}

		private void FrmProviderPick_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			ProvNumSelected=PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString());
			IsDialogOK=true;
		}

		private void butSelectNone_Click(object sender,EventArgs e) {
			ProvNumSelected=0;
			IsDialogOK=true;
		}

	}
}