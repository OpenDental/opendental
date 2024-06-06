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
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormProviderSetup:FormODBase {
		///<summary>Indicates that something about the providers has changed and needs to send out an invalid signal to the other workstations.</summary>
		private bool _hasChanged;
		//private User user;
		private DataTable _tableProvs;
		///<summary>Set when prov picker button is used.  textMoveTo shows this prov in human readable format.</summary>
		private long _provNumMoveTo=-1;
		private List<UserGroup> _listUserGroups;
		private ToolTip _toolTipPriProvEdit=new ToolTip() { ShowAlways=true };
		///<summary>A stale copy of all providers.  Gets a lazy update whenever needed (e.g. after ProvEdit window closes with changes)/</summary>
		private List<Provider> _listProviders;
		private List<SchoolClass> _listSchoolClasses;

		///<summary>Not used for selection.  Use FormProviderPick or FormProviderMultiPick for that.</summary>
		public FormProviderSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(FormODBase.AreBordersMS) {
				this.AutoSize=true;
			}
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				this.Width=960;
			}
			_waitFilterMs=200;//because we are not doing any database calls, we want a lower time to make the application feel more responsive
		}

		private void FormProviderSetup_Load(object sender, System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(false),textSearch);
			//There are two permissions which allow access to this window: Providers and AdminDentalStudents.  SecurityAdmin allows some extra functions.
			if(!Security.IsAuthorized(EnumPermType.ProviderAlphabetize,true)) {
				butAlphabetize.Enabled=false;
			}
			if(!Security.IsAuthorized(EnumPermType.ProviderAdd,suppressMessage:true)) {
				butAdd.Enabled=false;
			}
			_listProviders=Providers.GetDeepCopy();
			if(Security.IsAuthorized(EnumPermType.SecurityAdmin,true)){
				_listUserGroups=UserGroups.GetList();
				for(int i=0;i<_listUserGroups.Count;i++){
					comboUserGroup.Items.Add(_listUserGroups[i].Description,_listUserGroups[i]);
				}
				if(comboUserGroup.Items.Count>0) {
					comboUserGroup.SetSelected(0,true);
				}
			}
			else{
				groupCreateUsers.Enabled=false;
				groupMovePats.Enabled=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)){
				groupDentalSchools.Visible=false;
				butStudBulkEdit.Visible=false;
			}
			else{
				comboClass.Items.Add(Lan.g(this,"All"));
				comboClass.SelectedIndex=0;
				_listSchoolClasses=SchoolClasses.GetDeepCopy();
				for(int i=0;i<_listSchoolClasses.Count;i++){
					comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
				}
				butUp.Visible=false;
				butDown.Visible=false;
			}
			checkShowHidden.Checked=PrefC.GetBool(PrefName.EasyHideDentalSchools);
			if(Security.IsAuthorized(EnumPermType.PatPriProvEdit,DateTime.MinValue,true,true)){
				return;
			}
			string strToolTip=Lan.g("Security","Not authorized for")+" "+GroupPermissions.GetDesc(EnumPermType.PatPriProvEdit);
			_toolTipPriProvEdit.SetToolTip(butReassign,strToolTip);
			_toolTipPriProvEdit.SetToolTip(butMovePri,strToolTip);
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormProviderSetup_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		///<summary>Refreshed the table that is showing in the grid.  Also corrects the item order of the provider table.</summary>
		private void RefreshTable(int indexComboClassSelected) {
			if(groupDentalSchools.Visible) {
				long schoolClass=0;
				if(indexComboClassSelected>0) {
					schoolClass=_listSchoolClasses[indexComboClassSelected-1].SchoolClassNum;
				}
				_tableProvs=Providers.RefreshForDentalSchool(schoolClass,textLastName.Text,textFirstName.Text,textProvNum.Text,radioInstructors.Checked,radioAll.Checked);
			return;
			}
			_tableProvs=Providers.RefreshStandard(checkShowPatientCount.Checked);
			//fix orders
			bool hasChanged=false;
			Provider provider;
			for(int i=0;i<_tableProvs.Rows.Count;i++) {
				if(_tableProvs.Rows[i]["ItemOrder"].ToString()!=i.ToString()) {
					provider=_listProviders.Find(x => x.ProvNum==PIn.Long(_tableProvs.Rows[i]["ProvNum"].ToString()));
					provider.ItemOrder=i;
					Providers.Update(provider);
					_tableProvs.Rows[i]["ItemOrder"]=i.ToString();
					hasChanged=true;
				}
			}
			if(hasChanged) {
				DataValid.SetInvalid(InvalidType.Providers);
			}
		}

		private void FillGrid(bool needsRefresh=true) {
			if(needsRefresh) {
				int selectedIndex=comboClass.SelectedIndex;
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => RefreshTable(selectedIndex);
				progressOD.StartingMessage="Refreshing data...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			List<long> listProvNumsSelected=gridMain.SelectedIndices.OfType<int>().Select(x => ((Provider)gridMain.ListGridRows[x].Tag).ProvNum).ToList();
			int scroll=gridMain.ScrollValue;
			int indexSortCol=gridMain.GetSortedByColumnIdx();
			bool isSortAsc=gridMain.IsSortedAscending();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","ProvNum"),60,GridSortingStrategy.AmountParse));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","Abbrev"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","Last Name"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","First Name"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","User Name"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","Hidden"),50,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","HideOnReports"),100,HorizontalAlignment.Center));
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","Class"),90));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","Instructor"),60,HorizontalAlignment.Center));
			}
			if(checkShowPatientCount.Checked) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","PriPats"),50,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableProviderSetup","SecPats"),50,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<string> listSearchWords=textSearch.Text.ToLower().Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
			for(int i=0;i<_tableProvs.Rows.Count;i++) {
				if(!checkShowHidden.Checked && _tableProvs.Rows[i]["IsHidden"].ToString()=="1") {
					continue;
				}
				List<string> listColsToSearch=new List<string> { "Abbr","LName","FName" };
				//Do not add the row if the user typed something into the search box and no cell contains the text that was typed in.
				if(listSearchWords.Count>0 && !listSearchWords.All(x => listColsToSearch.Any(y => _tableProvs.Rows[i][y].ToString().ToLower().Contains(x)))) {
					continue;
				}
				row=new GridRow();
				if(_tableProvs.Rows[i]["ProvStatus"].ToString()==((int)ProviderStatus.Deleted).ToString()) {
					if(!checkShowDeleted.Checked) {
						continue;
					}
					row.ColorText=Color.Red;
				}
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(_tableProvs.Rows[i]["ProvNum"].ToString());
				}
				row.Cells.Add(_tableProvs.Rows[i]["Abbr"].ToString());
				row.Cells.Add(_tableProvs.Rows[i]["LName"].ToString());
				row.Cells.Add(_tableProvs.Rows[i]["FName"].ToString());
				row.Cells.Add(_tableProvs.Rows[i]["UserName"].ToString());
				row.Cells.Add(_tableProvs.Rows[i]["IsHidden"].ToString()=="1"?"X":"");
				row.Cells.Add(_tableProvs.Rows[i]["IsHiddenReport"].ToString()=="1"?"X":"");
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(_tableProvs.Rows[i]["GradYear"].ToString()!=""?(_tableProvs.Rows[i]["GradYear"].ToString()+"-"+_tableProvs.Rows[i]["Descript"].ToString()):"");
					row.Cells.Add(_tableProvs.Rows[i]["IsInstructor"].ToString()=="1"?"X":"");
				}
				if(checkShowPatientCount.Checked) {
					row.Cells.Add(_tableProvs.Rows[i]["PatCountPri"].ToString());
					row.Cells.Add(_tableProvs.Rows[i]["PatCountSec"].ToString());
				}
				long provNumCur=PIn.Long(_tableProvs.Rows[i]["ProvNum"].ToString());
				row.Tag=_listProviders.Find(x => x.ProvNum==provNumCur);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(indexSortCol>-1 && indexSortCol<gridMain.Columns.Count) {
				gridMain.SortForced(indexSortCol,isSortAsc);
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				long provNumCur=((Provider)gridMain.ListGridRows[i].Tag).ProvNum;
				if(listProvNumsSelected.Contains(provNumCur)) {
					gridMain.SetSelected(i,true);
				}
			}
			gridMain.ScrollValue=scroll;
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void textLastName_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textFirstName_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textProvNum_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioAll_Click(object sender,EventArgs e) {
			comboClass.SelectedIndex=0;//Only students are attached to classes
			comboClass.Enabled=true;//Re-enable classes when all is selected
			FillGrid();
		}

		private void radioStudents_Click(object sender,EventArgs e) {
			comboClass.Enabled=true;//Re-enable classes when students are selected
			FillGrid();
		}

		private void radioInstructors_Click(object sender,EventArgs e) {
			comboClass.SelectedIndex=0;//Only students are attached to classes
			comboClass.Enabled=false;//Disable classes when instructors are selected
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ProviderAdd)) {
				return;//Should not be possible, button should be disabled. This is just in case.
			}
			using FormProvEdit formProvEdit=new FormProvEdit();
			formProvEdit.ProviderCur=new Provider();
			formProvEdit.ProviderCur.IsNew=true;
			using FormProvStudentEdit formProvStudentEdit=new FormProvStudentEdit();
			formProvStudentEdit.ProviderStudent=new Provider();
			formProvStudentEdit.ProviderStudent.IsNew=true;
			Provider provider=new Provider();
			if(groupDentalSchools.Visible) {
				//Dental schools do not worry about item orders.
				if(radioStudents.Checked) {
					if(!Security.IsAuthorized(EnumPermType.AdminDentalStudents)) {
						return;
					}
					if(comboClass.SelectedIndex==0) {
						MsgBox.Show(this,"A class must be selected from the drop down box before a new student can be created");
						return;
					}
					formProvStudentEdit.ProviderStudent.SchoolClassNum=_listSchoolClasses[comboClass.SelectedIndex-1].SchoolClassNum;
					formProvStudentEdit.ProviderStudent.FName=textFirstName.Text;
					formProvStudentEdit.ProviderStudent.LName=textLastName.Text;
				}
				if(radioInstructors.Checked && !Security.IsAuthorized(EnumPermType.AdminDentalInstructors)) {
					return;
				}
				formProvEdit.ProviderCur.IsInstructor=radioInstructors.Checked;
				formProvEdit.ProviderCur.FName=textFirstName.Text;
				formProvEdit.ProviderCur.LName=textLastName.Text;
			}
			else {//Not using Dental Schools feature.
				if(gridMain.SelectedIndices.Length>0) {//place new provider after the first selected index. No changes are made to DB until after provider is actually inserted.
					formProvEdit.ProviderCur.ItemOrder=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).ItemOrder;//now two with this itemorder
				}
				else if(gridMain.ListGridRows.Count>0) {
					formProvEdit.ProviderCur.ItemOrder=((Provider)gridMain.ListGridRows[gridMain.ListGridRows.Count-1].Tag).ItemOrder+1;
				}
				else {
					formProvEdit.ProviderCur.ItemOrder=0;
				}
			}
			if(!radioStudents.Checked) {
				if(radioInstructors.Checked && PrefC.GetLong(PrefName.SecurityGroupForInstructors)==0) {
					MsgBox.Show(this,"Security Group for Instructors must be set from the Dental School Setup window before adding instructors.");
					return;
				}
				formProvEdit.IsNew=true;
				formProvEdit.ShowDialog();
				if(formProvEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				provider=formProvEdit.ProviderCur;
				SecurityLogs.MakeLogEntry(EnumPermType.ProviderAdd,0,"Provider: "+formProvEdit.ProviderCur.Abbr+" added.");
			}
			else {
				if(radioStudents.Checked && PrefC.GetLong(PrefName.SecurityGroupForStudents)==0) {
					MsgBox.Show(this,"Security Group for Students must be set from the Dental School Setup window before adding students.");
					return;
				}
				formProvStudentEdit.ShowDialog();
				if(formProvStudentEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				provider=formProvStudentEdit.ProviderStudent;
			}
			//new provider has already been inserted into DB from above
			Providers.MoveDownBelow(provider);//safe to run even if none selected.
			_hasChanged=true;
			Cache.Refresh(InvalidType.Providers);
			_listProviders=Providers.GetDeepCopy();
			FillGrid();
			gridMain.ScrollToEnd();//should change this to scroll to the same place as before.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {//Providers.ListShallow.Count;i++) {
				if(((Provider)gridMain.ListGridRows[i].Tag).ProvNum==provider.ProvNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butStudBulkEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AdminDentalStudents)) {
				return;
			}
			using FormProvStudentBulkEdit formProvStudentBulkEdit=new FormProvStudentBulkEdit();
			formProvStudentBulkEdit.ShowDialog();
		}

		///<summary>Won't be visible if using Dental Schools.  So list will be unfiltered and ItemOrders won't get messed up.</summary>
		private void butUp_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one provider first.");
				return;
			}
			if(gridMain.SelectedIndices[0]==0) {//already at top
				return;
			}
			//Note: sourceProv will always be the selected prov, but destProv isn't necessarily the provider that is +1 idx in the table.
			//The grid is filtered, the table is not.
			//The provider's position in the table needs to reflect their item orders.
			Provider providerSource=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			Provider providerDestination=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]-1].Tag);
			int indexSource=providerSource.ItemOrder;
			providerSource.ItemOrder=providerDestination.ItemOrder;
			Providers.Update(providerSource);
			providerDestination.ItemOrder=indexSource;
			Providers.Update(providerDestination);	
			_hasChanged=true;
			int indexSelected=gridMain.SelectedIndices[0];
			SwapGridMainLocations(indexSelected,indexSelected-1);
			gridMain.SetSelected(indexSelected-1,true);
		}

		///<summary>Won't be visible if using Dental Schools.  So list will be unfiltered and ItemOrders won't get messed up.</summary>
		private void butDown_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one provider first.");
				return;
			}
			if(gridMain.SelectedIndices[0]==gridMain.ListGridRows.Count-1) {//already at bottom
				return;
			}
			//Note: sourceProv will always be the selected prov, but destProv isn't necessarily the provider that is +1 idx in the table.
			//The grid is filtered, the table is not.
			//The provider's position in the table needs to reflect their item orders.
			Provider providerSource=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			Provider providerDestination=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]+1].Tag);
			int indexSource=providerSource.ItemOrder;
			providerSource.ItemOrder=providerDestination.ItemOrder;
			Providers.Update(providerSource);
			providerDestination.ItemOrder=indexSource;
			Providers.Update(providerDestination);
			_hasChanged=true;		
			int selectedIdx=gridMain.SelectedIndices[0];	
			SwapGridMainLocations(selectedIdx,selectedIdx+1);
			gridMain.SetSelected(selectedIdx+1,true);
		}

		private void SwapGridMainLocations(int indexMoveFrom, int indexMoveTo) {
			gridMain.BeginUpdate();
			GridRow row=gridMain.ListGridRows[indexMoveFrom];
			gridMain.ListGridRows.RemoveAt(indexMoveFrom);
			gridMain.ListGridRows.Insert(indexMoveTo,row);
			gridMain.EndUpdate();		
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Provider providerSelected=(Provider)gridMain.ListGridRows[e.Row].Tag;
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) && Providers.IsAttachedToUser(providerSelected.ProvNum)) {//Dental schools is turned on and the provider selected is attached to a user.
				//provSelected could be a student, an instructor, or other provider at this point.
				if(!providerSelected.IsInstructor && providerSelected.SchoolClassNum!=0 && !Security.IsAuthorized(EnumPermType.AdminDentalStudents)) {//student
					return;
				}
				if(providerSelected.IsInstructor && !Security.IsAuthorized(EnumPermType.AdminDentalInstructors)) {//instructor
					return;
				}
				if(!providerSelected.IsInstructor && providerSelected.SchoolClassNum==0 && !Security.IsAuthorized(EnumPermType.ProviderEdit)) {//other provider
					return;
				}
				if(!radioStudents.Checked) {
					using FormProvEdit formProvEdit=new FormProvEdit();
					formProvEdit.ProviderCur=(Provider)gridMain.ListGridRows[e.Row].Tag;
					formProvEdit.ShowDialog();
					if(formProvEdit.DialogResult!=DialogResult.OK) {
						return;
					}
					if(Security.IsAuthorized(EnumPermType.ProviderEdit,suppressMessage:true)) {//Other provider. Dental schools specific permissions do not log.
						SecurityLogs.MakeLogEntry(EnumPermType.ProviderEdit,0,"Provider: "+formProvEdit.ProviderCur.Abbr+" edited.",formProvEdit.ProviderCur.ProvNum,SecurityLogs.LogSource,DateTime.MinValue);
					}
				}
				else {
					using FormProvStudentEdit formProvStudentEdit=new FormProvStudentEdit();
					formProvStudentEdit.ProviderStudent=(Provider)gridMain.ListGridRows[e.Row].Tag;
					formProvStudentEdit.ShowDialog();
					if(formProvStudentEdit.DialogResult!=DialogResult.OK) {
						return;
					}
				}
			}
			else {//No Dental Schools or provider is not attached to a user
				if(!Security.IsAuthorized(EnumPermType.ProviderEdit)) {
					return;
				}
				using FormProvEdit formProvEdit=new FormProvEdit();
				formProvEdit.ProviderCur=(Provider)gridMain.ListGridRows[e.Row].Tag;
				formProvEdit.ShowDialog();
				if(formProvEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				SecurityLogs.MakeLogEntry(EnumPermType.ProviderEdit,0,"Provider: "+formProvEdit.ProviderCur.Abbr+" edited.",formProvEdit.ProviderCur.ProvNum,SecurityLogs.LogSource,DateTime.MinValue);
			}
			_hasChanged=true;
			Cache.Refresh(InvalidType.Providers);
			_listProviders=Providers.GetDeepCopy();
			FillGrid();
		}

		private void butProvPick_Click(object sender,EventArgs e) {
			//This button is used instead of a dropdown because the order of providers can frequently change in the grid.
			using FormProviderPick formProviderPick=new FormProviderPick();
			formProviderPick.IsNoneAvailable=true;
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_provNumMoveTo=formProviderPick.ProvNumSelected;
			if(_provNumMoveTo>0) {
				Provider provider=_listProviders.Find(x => x.ProvNum==_provNumMoveTo);
				textMoveTo.Text=provider.GetLongDesc();
				return;
			}
			textMoveTo.Text="None";
		}

		///<summary>Not possible if no security admin or no PatPriProvEdit permission.</summary>
		private void butMovePri_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatPriProvEdit)) {//shouldn't be possible, button should be disabled if not authorized, just in case
				return;
			}
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"You must select at least one provider to move patients from.");
				return;
			}
			List<Provider> listProvidersFrom=gridMain.SelectedIndices.OfType<int>().Select(x => (Provider)gridMain.ListGridRows[x].Tag).ToList();
			if(_provNumMoveTo==-1){
				MsgBox.Show(this,"You must pick a 'To' provider in the box above to move patients to.");
				return;
			}
			if(_provNumMoveTo==0) {
				MsgBox.Show(this,"'None' is not a valid primary provider.");
				return;
			}
			Provider provider=_listProviders.FirstOrDefault(x => x.ProvNum==_provNumMoveTo);
			if(provider==null) {
				MsgBox.Show(this,"The provider could not be found.");
				return;
			}
			Lookup<long,long> lookupPriProvPats = null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				//get pats with original (from) priprov
				List<long> listProvNums=listProvidersFrom.Select(x => x.ProvNum).ToList();
				DataTable table=Patients.GetPatNumsByPriProvs(listProvNums);
				DataRow[] dataRowArray=table.Select();
				//key=ProvNum, gives list of PatNums
				lookupPriProvPats= (Lookup<long,long>)dataRowArray.ToLookup(x => PIn.Long(x["PriProv"].ToString()),x => PIn.Long(x["PatNum"].ToString()));
			};
			progressOD.StartingMessage=Lan.g(this,"Gathering patient data")+"...";
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			int patCountTotal=0;
			List<long> listKeys=lookupPriProvPats.Select(x => x.Key).ToList();
			for(int i=0;i<listKeys.Count;i++){
				patCountTotal+=lookupPriProvPats[listKeys[i]].Count();
			}
			if(patCountTotal==0) {
				MsgBox.Show(this,"The selected providers are not primary providers for any patients.");
				return;
			}
			string strProvFromDesc=string.Join(", ",listProvidersFrom.FindAll(x => lookupPriProvPats.Contains(x.ProvNum)).Select(x => x.Abbr));
			string strProvToDesc=provider.Abbr;
			string msg=Lan.g(this,"Move all primary patients to")+" "+strProvToDesc+" "+Lan.g(this,"from the following providers")+": "+strProvFromDesc+"?";
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			int patsMoved=0;
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				List<Action> listActions = lookupPriProvPats.Select(x => new Action(() => {
					patsMoved+=x.Count();
					ODEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Moving patients")+": "+patsMoved+" out of "+patCountTotal);
					Patients.ChangePrimaryProviders(x.Key,provider.ProvNum);//update all priprovs to new provider
					SecurityLogs.MakeLogEntry(EnumPermType.PatPriProvEdit,0,"Primary provider changed for "+x.Count()+" patients from "
						+Providers.GetLongDesc(x.Key)+" to "+provider.GetLongDesc()+".");
				})).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));
			};
			progressOD.StartingMessage=Lan.g(this,"Moving patients")+"...";
			progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
			//if(!progressOD.IsSuccess){//it might be partly done, so we will continue.
			_hasChanged=true;
			FillGrid();
		}

		///<summary>Not possible if no security admin.</summary>
		private void butMoveSec_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"You must select at least one provider to move patients from.");
				return;
			}
			List<Provider> listProvidersFrom=gridMain.SelectedIndices.OfType<int>().Select(x => (Provider)gridMain.ListGridRows[x].Tag).ToList();
			if(_provNumMoveTo==-1) {
				MsgBox.Show(this,"You must pick a 'To' provider in the box above to move patients to.");
				return;
			}
			Provider provider=_listProviders.FirstOrDefault(x => x.ProvNum==_provNumMoveTo);
			string msg;
			if(provider==null) {
				msg=Lan.g(this,"Remove all secondary patients from the selected providers")+"?";
			}
			else {
				string strProvsFrom=string.Join(", ",listProvidersFrom.Select(x => x.Abbr));
				msg=Lan.g(this,"Move all secondary patients to")+" "+provider.Abbr+" "+Lan.g(this,"from the following providers")+": "+strProvsFrom+"?";
			}
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				List<Action> listActions=listProvidersFrom.Select(x => new Action(() => { Patients.ChangeSecondaryProviders(x.ProvNum,
					provider?.ProvNum??0); })).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));//each group of actions gets 2 minutes
			};
			progressOD.StartingMessage=Lan.g(this,"Reassigning patients")+"...";
			progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
			_hasChanged=true;
			FillGrid();
		}

		//used to map which pats have which providers
		private class PatProv{
			public long PatNum;
			public long ProvNum;
		}

		private void butReassign_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatPriProvEdit)) {//shouldn't be possible, button should be disabled if not authorized, just in case
				return;
			}
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select a provider, first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Ready to look for possible reassignments.  This will take a few minutes, and may make the program unresponsive on other computers during that time.  You will be given one more chance after this to cancel before changes are made to the database.  Running this for one provider at a time can help minimize database slowdown.  Continue?"))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			List<long> listProvNumsFrom=listProvNumsFrom=gridMain.SelectedIndices.OfType<int>().Select(x => ((Provider)gridMain.ListGridRows[x].Tag).ProvNum).ToList();
			DataTable tablePatNums=Patients.GetPatNumsByPriProvs(listProvNumsFrom);//list of all patients who are using the selected providers.
			if(tablePatNums.Rows.Count==0){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"No patients to reassign.");
				return;
			}
			List<PatProv> listPatProvsFrom=new List<PatProv>();
			for(int i=0;i<tablePatNums.Rows.Count;i++){
				PatProv patProv=new PatProv();
				patProv.PatNum=PIn.Long(tablePatNums.Rows[i]["PatNum"].ToString());
				patProv.ProvNum=PIn.Long(tablePatNums.Rows[i]["PriProv"].ToString());
				listPatProvsFrom.Add(patProv);
			}
			//This will contain one row per patient.
			//Excludes patients that we don't want to reassign because they are already set to the correct provider.
			List<PatProv> listPatProvsSeen=new List<PatProv>();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				List<long> listPatNums=listPatProvsFrom.Select(x => x.PatNum).ToList();
				DataTable table = Procedures.GetTablePatProvUsed(listPatNums);//big list getting passed in.
				//Result table will be huge.
				//It will contain multiple rows for some patients if they saw multiple providers.
				for(int i=0;i<table.Rows.Count;i++) {
					PatProv patProv=new PatProv();
					patProv.PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
					patProv.ProvNum=PIn.Long(table.Rows[i]["ProvNum"].ToString());
					if(listPatProvsSeen.Any(x => x.PatNum==patProv.PatNum)) {
						continue;// exclude Patients already added
					}
					PatProv patProvOld=listPatProvsFrom.Find(x=>x.PatNum==patProv.PatNum);//guaranteed to work
					long provNumOld=patProvOld.ProvNum;
					if(patProv.ProvNum==provNumOld){
						continue;//exclude if patNum is already correctly assigned
					}
					listPatProvsSeen.Add(patProv);
				}
			};
			progressOD.StartingMessage=Lan.g(this,"Gathering patient and provider details")+"...";
			progressOD.ShowDialogProgress();
			Cursor=Cursors.Default;
			if(listPatProvsSeen.Count==0){
				MsgBox.Show(this,"No patients to reassign.");
				return;
			}
			string msg=Lan.g(this,"You are about to reassign")+" "+listPatProvsSeen.Count+" "+Lan.g(this,"patients to different providers.  Continue?");
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			//display the progress bar, updated by odThread.ProgressLog.UpdateProgress()
			Cursor=Cursors.WaitCursor;
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				for(int i=0;i<listPatProvsSeen.Count;i++){
					long patNum=listPatProvsSeen[i].PatNum;
					long provNumNew=listPatProvsSeen[i].ProvNum;
					Patients.UpdateProv(patNum, provNumNew);
				}
			};
			progressOD.StartingMessage=Lan.g(this,"Reassigning patients")+"...";
			progressOD.ShowDialogProgress();
			Cursor=Cursors.Default;
			//changed=true;//We didn't change any providers
			FillGrid();
			MsgBox.Show(this,"Done");
		}

		///<summary>Not possible if no security admin.</summary>
		private void butCreateUsers_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select one or more providers first.");
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				if(Providers.IsAttachedToUser(((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).ProvNum)) {
					MsgBox.Show(this,"Not allowed to create users on providers which already have users.");
					return;
				}
			}
			if(comboUserGroup.GetListSelected<UserGroup>().Count == 0){
				MsgBox.Show(this,"Please select at least one User Group first.");
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				Provider provider=(Provider)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				Userod userod=new Userod();
				userod.ProvNum=provider.ProvNum;
				userod.UserName=GetUniqueUserName(provider.LName,provider.FName);
				if(userod.UserName.TrimEnd()!=userod.UserName) {
					MsgBox.Show(this,"User Name cannot end with white space.");
					_hasChanged=true;
					return;
				}
				userod.LoginDetails=Authentication.GenerateLoginDetailsSHA512(userod.UserName);
				try{
					Userods.Insert(userod,comboUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
				}
				catch(ApplicationException ex){
					MessageBox.Show(ex.Message);
					_hasChanged=true;
					return;
				}
			}
			_hasChanged=true;
			FillGrid();
		}

		private string GetUniqueUserName(string lname,string fname){
			string name=lname;
			if(fname.Length>0){
				name+=fname.Substring(0,1);
			}
			if(Userods.IsUserNameUnique(name,0,false)){
				return name;
			}
			int fnameI=1;
			while(fnameI<fname.Length){
				name+=fname.Substring(fnameI,1);
				if(Userods.IsUserNameUnique(name,0,false)) {
					return name;
				}
				fnameI++;
			}
			//should be entire lname+fname at this point, but still not unique
			do{
				name+="x";
			}
			while(!Userods.IsUserNameUnique(name,0,false));
			return name;
		}

		private void checkShowDeleted_CheckedChanged(object sender,EventArgs e) {
			if(checkShowDeleted.Checked) {
				checkShowHidden.Checked=true;
			}
			FillGrid(checkShowDeleted.Checked);
		}

		private void checkShowPatientCount_CheckedChanged(object sender,EventArgs e) {
			FillGrid(checkShowPatientCount.Checked);
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ProviderAlphabetize,false)) {
				return;//should not be possible, button should be disabled. This is just in case.
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Alphabetize all providers (by Abbrev) and move hidden providers to the bottom, followed by all non-person providers? This cannot be undone.")) {
				return;
			}
			//According to original task the form should display providers in the following order:
			//1) Is a person, not hidden -sorted alphabetically by abbreviation
			//2) Is not a person, not hidden - sorted alphabetically by abbreviation
			//3) all hidden providers, sorted alphabetically by abbreviation (is a person and is not a person would be mixed)
			List<Provider> listProvidersAll = Providers.GetAll()
				.OrderBy(x => x.IsHidden)
				.ThenBy(x => x.IsHidden || x.IsNotPerson)
				.ThenBy(x => x.GetAbbr()).ToList();
			bool changed = false; 
			for(int i = 0;i<listProvidersAll.Count;i++) {
				Provider provider = listProvidersAll[i];
				if(provider.ItemOrder==i) {
					continue;
				}
				provider.ItemOrder=i;
				Providers.Update(provider);
				changed=true;
			}
			if(changed) {
				Signalods.SetInvalid(InvalidType.Providers);
			}
			_listProviders=listProvidersAll;
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormProviderSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			string duplicates=Providers.GetDuplicateAbbrs();
			if(duplicates!="" && PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				if(MessageBox.Show(Lan.g(this,"Warning.  The following abbreviations are duplicates.  Continue anyway?\r\n")+duplicates,
					"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
				{
					e.Cancel=true;
					return;
				}
			}
			if(_hasChanged){
				DataValid.SetInvalid(InvalidType.Providers, InvalidType.Security);
			}
			//SecurityLogs.MakeLogEntry("Providers","Altered Providers",user);
		}
	}

	
}
