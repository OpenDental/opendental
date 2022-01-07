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
		private ToolTip _priProvEditToolTip=new ToolTip() { ShowAlways=true };
		///<summary>A stale copy of all providers.  Gets a lazy update whenever needed (e.g. after ProvEdit window closes with changes)/</summary>
		private List<Provider> _listProvs;
		private List<SchoolClass> _listSchoolClasses;

		///<summary>Not used for selection.  Use FormProviderPick or FormProviderMultiPick for that.</summary>
		public FormProviderSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				this.Width=960;
			}
			_filterCommitMs=200;//because we are not doing any database calls, we want a lower time to make the application feel more responsive
		}

		private void FormProviderSetup_Load(object sender, System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(false),textSearch);
			//There are two permissions which allow access to this window: Providers and AdminDentalStudents.  SecurityAdmin allows some extra functions.
			if(!Security.IsAuthorized(Permissions.ProviderAlphabetize,true)) {
				butAlphabetize.Enabled=false;
			}
			_listProvs=Providers.GetDeepCopy();
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)){
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
			if(!Security.IsAuthorized(Permissions.PatPriProvEdit,DateTime.MinValue,true,true)) {
				string strToolTip=Lan.g("Security","Not authorized for")+" "+GroupPermissions.GetDesc(Permissions.PatPriProvEdit);
				_priProvEditToolTip.SetToolTip(butReassign,strToolTip);
				_priProvEditToolTip.SetToolTip(butMovePri,strToolTip);
			}
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormProviderSetup_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		///<summary>Refreshed the table that is showing in the grid.  Also corrects the item order of the provider table.</summary>
		private void RefreshTable(int comboClassSelectedIndex) {
			if(groupDentalSchools.Visible) {
				long schoolClass=0;
				if(comboClassSelectedIndex>0) {
					schoolClass=_listSchoolClasses[comboClassSelectedIndex-1].SchoolClassNum;
				}
				_tableProvs=Providers.RefreshForDentalSchool(schoolClass,textLastName.Text,textFirstName.Text,textProvNum.Text,radioInstructors.Checked,radioAll.Checked);
			}
			else {
				_tableProvs=Providers.RefreshStandard(checkShowPatientCount.Checked);
				//fix orders
				bool hasChanged=false;
				Provider prov;
				for(int i=0;i<_tableProvs.Rows.Count;i++) {
					if(_tableProvs.Rows[i]["ItemOrder"].ToString()!=i.ToString()) {
						prov=_listProvs.Find(x => x.ProvNum==PIn.Long(_tableProvs.Rows[i]["ProvNum"].ToString()));
						prov.ItemOrder=i;
						Providers.Update(prov);
						_tableProvs.Rows[i]["ItemOrder"]=i.ToString();
						hasChanged=true;
					}
				}
				if(hasChanged) {
					DataValid.SetInvalid(InvalidType.Providers);
				}
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
			List<long> listSelectedProvNums=gridMain.SelectedIndices.OfType<int>().Select(x => ((Provider)gridMain.ListGridRows[x].Tag).ProvNum).ToList();
			int scroll=gridMain.ScrollValue;
			int sortColIndx=gridMain.SortedByColumnIdx;
			bool isSortAsc=gridMain.SortedIsAscending;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","ProvNum"),60,GridSortingStrategy.AmountParse));
			}
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","Abbrev"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","Last Name"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","First Name"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","User Name"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","Hidden"),50,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","HideOnReports"),100,HorizontalAlignment.Center));
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","Class"),90));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","Instructor"),60,HorizontalAlignment.Center));
			}
			if(checkShowPatientCount.Checked) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","PriPats"),50,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderSetup","SecPats"),50,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<string> listSearchWords=textSearch.Text.ToLower().Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
			foreach(DataRow rowCur in _tableProvs.Rows) {
				if(!checkShowHidden.Checked && rowCur["IsHidden"].ToString()=="1") {
					continue;
				}
				List<string> listColsToSearch=new List<string> { "Abbr","LName","FName" };
				//Do not add the row if the user typed something into the search box and no cell contains the text that was typed in.
				if(listSearchWords.Count>0 && !listSearchWords.All(x => listColsToSearch.Any(y => rowCur[y].ToString().ToLower().Contains(x)))) {
					continue;
				}
				row=new GridRow();
				if(rowCur["ProvStatus"].ToString()==((int)ProviderStatus.Deleted).ToString()) {
					if(!checkShowDeleted.Checked) {
						continue;
					}
					row.ColorText=Color.Red;
				}
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(rowCur["ProvNum"].ToString());
				}
				row.Cells.Add(rowCur["Abbr"].ToString());
				row.Cells.Add(rowCur["LName"].ToString());
				row.Cells.Add(rowCur["FName"].ToString());
				row.Cells.Add(rowCur["UserName"].ToString());
				row.Cells.Add(rowCur["IsHidden"].ToString()=="1"?"X":"");
				row.Cells.Add(rowCur["IsHiddenReport"].ToString()=="1"?"X":"");
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(rowCur["GradYear"].ToString()!=""?(rowCur["GradYear"].ToString()+"-"+rowCur["Descript"].ToString()):"");
					row.Cells.Add(rowCur["IsInstructor"].ToString()=="1"?"X":"");
				}
				if(checkShowPatientCount.Checked) {
					row.Cells.Add(rowCur["PatCountPri"].ToString());
					row.Cells.Add(rowCur["PatCountSec"].ToString());
				}
				long provNumCur=PIn.Long(rowCur["ProvNum"].ToString());
				row.Tag=_listProvs.Find(x => x.ProvNum==provNumCur);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(sortColIndx>-1 && sortColIndx<gridMain.ListGridColumns.Count) {
				gridMain.SortForced(sortColIndx,isSortAsc);
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				long provNumCur=((Provider)gridMain.ListGridRows[i].Tag).ProvNum;
				if(listSelectedProvNums.Contains(provNumCur)) {
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
			FillGrid();
		}

		private void radioStudents_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioInstructors_Click(object sender,EventArgs e) {
			comboClass.SelectedIndex=0;//Only students are attached to classes
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormProvEdit FormPE=new FormProvEdit();
			FormPE.ProvCur=new Provider();
			FormPE.ProvCur.IsNew=true;
			using FormProvStudentEdit FormPSE=new FormProvStudentEdit();
			FormPSE.ProvStudent=new Provider();
			FormPSE.ProvStudent.IsNew=true;
			Provider provCur=new Provider();
			if(groupDentalSchools.Visible) {
				//Dental schools do not worry about item orders.
				if(radioStudents.Checked) {
					if(!Security.IsAuthorized(Permissions.AdminDentalStudents)) {
						return;
					}
					if(comboClass.SelectedIndex==0) {
						MsgBox.Show(this,"A class must be selected from the drop down box before a new student can be created");
						return;
					}
					FormPSE.ProvStudent.SchoolClassNum=_listSchoolClasses[comboClass.SelectedIndex-1].SchoolClassNum;
					FormPSE.ProvStudent.FName=textFirstName.Text;
					FormPSE.ProvStudent.LName=textLastName.Text;
				}
				if(radioInstructors.Checked && !Security.IsAuthorized(Permissions.AdminDentalInstructors)) {
					return;
				}
				FormPE.ProvCur.IsInstructor=radioInstructors.Checked;
				FormPE.ProvCur.FName=textFirstName.Text;
				FormPE.ProvCur.LName=textLastName.Text;
			}
			else {//Not using Dental Schools feature.
				if(gridMain.SelectedIndices.Length>0) {//place new provider after the first selected index. No changes are made to DB until after provider is actually inserted.
					FormPE.ProvCur.ItemOrder=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).ItemOrder;//now two with this itemorder
				}
				else if(gridMain.ListGridRows.Count>0) {
					FormPE.ProvCur.ItemOrder=((Provider)gridMain.ListGridRows[gridMain.ListGridRows.Count-1].Tag).ItemOrder+1;
				}
				else {
					FormPE.ProvCur.ItemOrder=0;
				}
			}
			if(!radioStudents.Checked) {
				if(radioInstructors.Checked && PrefC.GetLong(PrefName.SecurityGroupForInstructors)==0) {
					MsgBox.Show(this,"Security Group for Instructors must be set from the Dental School Setup window before adding instructors.");
					return;
				}
				FormPE.IsNew=true;
				FormPE.ShowDialog();
				if(FormPE.DialogResult!=DialogResult.OK) {
					return;
				}
				provCur=FormPE.ProvCur;
			}
			else {
				if(radioStudents.Checked && PrefC.GetLong(PrefName.SecurityGroupForStudents)==0) {
					MsgBox.Show(this,"Security Group for Students must be set from the Dental School Setup window before adding students.");
					return;
				}
				FormPSE.ShowDialog();
				if(FormPSE.DialogResult!=DialogResult.OK) {
					return;
				}
				provCur=FormPSE.ProvStudent;
			}
			//new provider has already been inserted into DB from above
			Providers.MoveDownBelow(provCur);//safe to run even if none selected.
			_hasChanged=true;
			Cache.Refresh(InvalidType.Providers);
			_listProvs=Providers.GetDeepCopy();
			FillGrid();
			gridMain.ScrollToEnd();//should change this to scroll to the same place as before.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {//Providers.ListShallow.Count;i++) {
				if(((Provider)gridMain.ListGridRows[i].Tag).ProvNum==provCur.ProvNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butStudBulkEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AdminDentalStudents)) {
				return;
			}
			using FormProvStudentBulkEdit FormPSBE=new FormProvStudentBulkEdit();
			FormPSBE.ShowDialog();
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
			Provider sourceProv=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			Provider destProv=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]-1].Tag);
			int sourceIdx=sourceProv.ItemOrder;
			sourceProv.ItemOrder=destProv.ItemOrder;
			Providers.Update(sourceProv);
			destProv.ItemOrder=sourceIdx;
			Providers.Update(destProv);	
			_hasChanged=true;
			int selectedIdx=gridMain.SelectedIndices[0];
			SwapGridMainLocations(selectedIdx,selectedIdx-1);
			gridMain.SetSelected(selectedIdx-1,true);
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
			Provider sourceProv=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			Provider destProv=((Provider)gridMain.ListGridRows[gridMain.SelectedIndices[0]+1].Tag);
			int sourceIdx=sourceProv.ItemOrder;
			sourceProv.ItemOrder=destProv.ItemOrder;
			Providers.Update(sourceProv);
			destProv.ItemOrder=sourceIdx;
			Providers.Update(destProv);
			_hasChanged=true;		
			int selectedIdx=gridMain.SelectedIndices[0];	
			SwapGridMainLocations(selectedIdx,selectedIdx+1);
			gridMain.SetSelected(selectedIdx+1,true);
		}

		private void SwapGridMainLocations(int indxMoveFrom, int indxMoveTo) {
			gridMain.BeginUpdate();
			GridRow dataRow=gridMain.ListGridRows[indxMoveFrom];
			gridMain.ListGridRows.RemoveAt(indxMoveFrom);
			gridMain.ListGridRows.Insert(indxMoveTo,dataRow);
			gridMain.EndUpdate();		
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Provider provSelected=(Provider)gridMain.ListGridRows[e.Row].Tag;
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) && Providers.IsAttachedToUser(provSelected.ProvNum)) {//Dental schools is turned on and the provider selected is attached to a user.
				//provSelected could be a provider or a student at this point.
				if(!provSelected.IsInstructor && !Security.IsAuthorized(Permissions.AdminDentalStudents)) {
					return;
				}
				if(provSelected.IsInstructor && !Security.IsAuthorized(Permissions.AdminDentalInstructors)) {
					return;
				}
				if(!radioStudents.Checked) {
					using FormProvEdit FormPE=new FormProvEdit();
					FormPE.ProvCur=(Provider)gridMain.ListGridRows[e.Row].Tag;
					FormPE.ShowDialog();
					if(FormPE.DialogResult!=DialogResult.OK) {
						return;
					}
				}
				else {
					using FormProvStudentEdit FormPSE=new FormProvStudentEdit();
					FormPSE.ProvStudent=(Provider)gridMain.ListGridRows[e.Row].Tag;
					FormPSE.ShowDialog();
					if(FormPSE.DialogResult!=DialogResult.OK) {
						return;
					}
				}
			}
			else {//No Dental Schools or provider is not attached to a user
				using FormProvEdit FormPE=new FormProvEdit();
				FormPE.ProvCur=(Provider)gridMain.ListGridRows[e.Row].Tag;
				FormPE.ShowDialog();
				if(FormPE.DialogResult!=DialogResult.OK) {
					return;
				}
			}
			_hasChanged=true;
			Cache.Refresh(InvalidType.Providers);
			_listProvs=Providers.GetDeepCopy();
			FillGrid();
		}

		private void butProvPick_Click(object sender,EventArgs e) {
			//This button is used instead of a dropdown because the order of providers can frequently change in the grid.
			using FormProviderPick formPick=new FormProviderPick();
			formPick.IsNoneAvailable=true;
			formPick.ShowDialog();
			if(formPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_provNumMoveTo=formPick.SelectedProvNum;
			if(_provNumMoveTo>0) {
				Provider provTo=_listProvs.Find(x => x.ProvNum==_provNumMoveTo);
				textMoveTo.Text=provTo.GetLongDesc();
			}
			else {
				textMoveTo.Text="None";
			}
		}

		///<summary>Not possible if no security admin or no PatPriProvEdit permission.</summary>
		private void butMovePri_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PatPriProvEdit)) {//shouldn't be possible, button should be disabled if not authorized, just in case
				return;
			}
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"You must select at least one provider to move patients from.");
				return;
			}
			List<Provider> listProvsFrom=gridMain.SelectedIndices.OfType<int>().Select(x => (Provider)gridMain.ListGridRows[x].Tag).ToList();
			if(_provNumMoveTo==-1){
				MsgBox.Show(this,"You must pick a 'To' provider in the box above to move patients to.");
				return;
			}
			if(_provNumMoveTo==0) {
				MsgBox.Show(this,"'None' is not a valid primary provider.");
				return;
			}
			Provider provTo=_listProvs.FirstOrDefault(x => x.ProvNum==_provNumMoveTo);
			if(provTo==null) {
				MsgBox.Show(this,"The provider could not be found.");
				return;
			}
			Dictionary<long,List<long>> dictPriProvPats=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				//get pats with original (from) priprov
				dictPriProvPats=Patients.GetPatNumsByPriProvs(listProvsFrom.Select(x => x.ProvNum).ToList()).Select()
					.GroupBy(x => PIn.Long(x["PriProv"].ToString()),x => PIn.Long(x["PatNum"].ToString()))
					.ToDictionary(x => x.Key,x => x.ToList());
			};
			progressOD.StartingMessage=Lan.g(this,"Gathering patient data")+"...";
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			int totalPatCount=dictPriProvPats.Sum(x => x.Value.Count);
			if(totalPatCount==0) {
				MsgBox.Show(this,"The selected providers are not primary providers for any patients.");
				return;
			}
			string strProvFromDesc=string.Join(", ",listProvsFrom.FindAll(x => dictPriProvPats.ContainsKey(x.ProvNum)).Select(x => x.Abbr));
			string strProvToDesc=provTo.Abbr;
			string msg=Lan.g(this,"Move all primary patients to")+" "+strProvToDesc+" "+Lan.g(this,"from the following providers")+": "+strProvFromDesc+"?";
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			int patsMoved=0;
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				List<Action> listActions=dictPriProvPats.Select(x => new Action(() => {
					patsMoved+=x.Value.Count;
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Moving patients")+": "+patsMoved+" out of "+totalPatCount);
					Patients.ChangePrimaryProviders(x.Key,provTo.ProvNum);//update all priprovs to new provider
					SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,0,"Primary provider changed for "+x.Value.Count+" patients from "
						+Providers.GetLongDesc(x.Key)+" to "+provTo.GetLongDesc()+".");
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
			List<Provider> listProvsFrom=gridMain.SelectedIndices.OfType<int>().Select(x => (Provider)gridMain.ListGridRows[x].Tag).ToList();
			if(_provNumMoveTo==-1) {
				MsgBox.Show(this,"You must pick a 'To' provider in the box above to move patients to.");
				return;
			}
			Provider provTo=_listProvs.FirstOrDefault(x => x.ProvNum==_provNumMoveTo);
			string msg;
			if(provTo==null) {
				msg=Lan.g(this,"Remove all secondary patients from the selected providers")+"?";
			}
			else {
				string strProvsFrom=string.Join(", ",listProvsFrom.Select(x => x.Abbr));
				msg=Lan.g(this,"Move all secondary patients to")+" "+provTo.Abbr+" "+Lan.g(this,"from the following providers")+": "+strProvsFrom+"?";
			}
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				List<Action> listActions=listProvsFrom.Select(x => new Action(() => { Patients.ChangeSecondaryProviders(x.ProvNum,
					provTo?.ProvNum??0); })).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));//each group of actions gets 2 minutes
			};
			progressOD.StartingMessage=Lan.g(this,"Reassigning patients")+"...";
			progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
			_hasChanged=true;
			FillGrid();
		}

		private void butReassign_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PatPriProvEdit)) {//shouldn't be possible, button should be disabled if not authorized, just in case
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
			//Convert DataTable to a var that is a list of objects with PatNum and ProvNum properties.  var is temporary, used to create a list of actions
			var listPatProvFrom=tablePatNums.Select().Select(x => new { PatNum=PIn.Long(x["PatNum"].ToString()),ProvNum=PIn.Long(x["PriProv"].ToString()) }).ToList();
			//Create dictionary of key=PatNum, value=ProvNum for the pat's most used provider. Most used is the prov with the most completed procs with a
			//tie broken by the most recent ProcDate.
			Dictionary<long,long> dictPatProvTo=new Dictionary<long, long>();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				DataTable tableProcs=Procedures.GetTablePatProvUsed(listPatProvFrom.Select(x=>x.PatNum).ToList());
				dictPatProvTo=tableProcs.Select()
					.GroupBy(x => PIn.Long(x["PatNum"].ToString()),x => new {
						ProvNum =PIn.Long(x["ProvNum"].ToString()),
						procCount =PIn.Int(x["procCount"].ToString()),
						maxProcDate =PIn.Date(x["maxProcDate"].ToString()) })
					.ToDictionary(x => x.Key,x => x.OrderByDescending(y => y.procCount).ThenByDescending(y => y.maxProcDate).First().ProvNum);
				//Remove all pats who don't have any procedures or whose PriProv is already the most used prov
				listPatProvFrom.RemoveAll(x => !dictPatProvTo.ContainsKey(x.PatNum) || x.ProvNum==dictPatProvTo[x.PatNum]);
				};
			progressOD.StartingMessage=Lan.g(this,"Gathering patient and provider details")+"...";
			progressOD.ShowDialogProgress();
			Cursor=Cursors.Default;
			if(listPatProvFrom.Count==0){
				MsgBox.Show(this,"No patients to reassign.");
				return;
			}
			string msg=Lan.g(this,"You are about to reassign")+" "+listPatProvFrom.Count+" "+Lan.g(this,"patients to different providers.  Continue?");
			if(MessageBox.Show(msg,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			//display the progress bar, updated by odThread.ProgressLog.UpdateProgress()
			Cursor=Cursors.WaitCursor;
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
					//Create list of actions that will set the pat's PriProv to the most used, create a securitylog entry, and update the progress 
					//bar if necessary
					int patsReassigned=0;
					List<Action> listActions=listPatProvFrom.GroupBy(x=>new { From=x.ProvNum,To=dictPatProvTo[x.PatNum] })
						.ToDictionary(x => x.Key,x=>x.Select(y=>y.PatNum).ToList())
						.Select(x => new Action(() => {
							patsReassigned+=x.Value.Count;
							PatientEvent.Fire(ODEventType.Patient,Lan.g(this,"Reassigning patients")+": "+patsReassigned
								+" "+Lan.g(this,"out of")+" "+listPatProvFrom.Count);
							Patients.ReassignProv(x.Key.To,x.Value);
							SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,0,"Primary provider changed for "+x.Value.Count+" patients from "
								+Providers.GetLongDesc(x.Key.From)+" to "+Providers.GetLongDesc(x.Key.To)+".");
						})).ToList();
					ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));//each group of actions gets 2 minutes
				};
			progressOD.StartingMessage=Lan.g(this,"Reassigning patients")+"...";
			progressOD.TypeEvent=typeof(PatientEvent);
			progressOD.ODEventType=ODEventType.Patient;
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
				Provider prov=(Provider)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				Userod user=new Userod();
				user.ProvNum=prov.ProvNum;
				user.UserName=GetUniqueUserName(prov.LName,prov.FName);
				user.LoginDetails=Authentication.GenerateLoginDetailsSHA512(user.UserName);
				try{
					Userods.Insert(user,comboUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
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
				FillGrid(checkShowDeleted.Checked);
		}

		private void checkShowPatientCount_CheckedChanged(object sender,EventArgs e) {
			FillGrid(checkShowPatientCount.Checked);
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ProviderAlphabetize,false)) {
				return;//should not be possible, button should be disabled. This is just in case.
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Alphabetize all providers (by Abbrev) and move hidden providers to the bottom, followed by all non-person providers? This cannot be undone.")) {
				return;
			}
			//According to original task the form should display providers in the following order:
			//1) Is a person, not hidden -sorted alphabetically by abbreviation
			//2) Is not a person, not hidden - sorted alphabetically by abbreviation
			//3) all hidden providers, sorted alphabetically by abbreviation (is a person and is not a person would be mixed)
			List<Provider> listProvsAll = Providers.GetAll()
				.OrderBy(x => x.IsHidden)
				.ThenBy(x => x.IsHidden || x.IsNotPerson)
				.ThenBy(x => x.GetAbbr()).ToList();
			bool changed = false; 
			for(int i = 0;i<listProvsAll.Count;i++) {
				Provider prov = listProvsAll[i];
				if(prov.ItemOrder==i) {
					continue;
				}
				prov.ItemOrder=i;
				Providers.Update(prov);
				changed=true;
			}
			if(changed) {
				Signalods.SetInvalid(InvalidType.Providers);
			}
			_listProvs=listProvsAll;
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
