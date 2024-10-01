using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTaskPreferences:FormODBase {

		public FormTaskPreferences() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskPreferences_Load(object sender,EventArgs e) {
			checkShowLegacyRepeatingTasks.Checked=PrefC.GetBool(PrefName.TasksUseRepeating);
			checkTaskListAlwaysShow.Checked=PrefC.GetBool(PrefName.TaskListAlwaysShowsAtBottom);
			if(checkTaskListAlwaysShow.Checked) {
				groupBoxComputerDefaults.Enabled=true;
			}
			else {
				groupBoxComputerDefaults.Enabled=false;
			}
			checkTasksNewTrackedByUser.Checked=PrefC.GetBool(PrefName.TasksNewTrackedByUser);
			checkShowOpenTickets.Checked=PrefC.GetBool(PrefName.TasksShowOpenTickets);
			checkBoxTaskKeepListHidden.Checked=ComputerPrefs.LocalComputer.TaskKeepListHidden;
			if(ComputerPrefs.LocalComputer.TaskDock==0) {
				radioBottom.Checked=true;
			}
			else {
				radioRight.Checked=true;
			}
			validNumX.Text=ComputerPrefs.LocalComputer.TaskX.ToString();
			validNumY.Text=ComputerPrefs.LocalComputer.TaskY.ToString();
			checkTaskSortApptDateTime.Checked=PrefC.GetBool(PrefName.TaskSortApptDateTime);
			checkTaskToApptOneToOne.Checked=PrefC.GetBool(PrefName.TasksForApptAllowMultiple);
			FillComboGlobalFilter();
			FillComboImageCategoryFolders();
		}

		///<summary>Fills the Global Task filter combobox with options.  Only visible if Clinics are enabled, or if previous selection no is longer 
		///available, example: Clinics have been turned off, Clinic filter no longer available.</summary>
		private void FillComboGlobalFilter() {
			EnumTaskFilterType globalPref=(EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType);
			comboFilterDefault.Items.Add(Lan.g(this,EnumTaskFilterType.Disabled.GetDescription()),EnumTaskFilterType.Disabled);
			comboFilterDefault.Items.Add(Lan.g(this,EnumTaskFilterType.None.GetDescription()),EnumTaskFilterType.None);
			if(PrefC.HasClinicsEnabled) {
				labelGlobalFilter.Visible=true;
				comboFilterDefault.Visible=true;
				comboFilterDefault.Items.Add(Lan.g(this,EnumTaskFilterType.Clinic.GetDescription()),EnumTaskFilterType.Clinic);
				if(Defs.GetDefsForCategory(DefCat.Regions).Count>0) {
					comboFilterDefault.Items.Add(Lan.g(this,EnumTaskFilterType.Region.GetDescription()),EnumTaskFilterType.Region);
				}
			}
			comboFilterDefault.SetSelectedEnum(globalPref);
			if(comboFilterDefault.SelectedIndex==-1) {
				labelGlobalFilter.Visible=true;
				comboFilterDefault.Visible=true;
				errorProvider1.SetError(comboFilterDefault,$"Previous selection \"{globalPref.GetDescription()}\" is no longer available.  "
					+"Saving will overwrite previous setting.");
				comboFilterDefault.SelectedIndex=0;
			}
		}

		///<summary></summary>
		private void FillComboImageCategoryFolders() {
			long defNumImageCategory=PrefC.GetLong(PrefName.TaskAttachmentCategory);
			List<Def> listDefsImageCategories=Defs.GetDefsForCategory(DefCat.ImageCats).FindAll(x => x.ItemValue.Contains("Y"));//Image categories that have task attachment usage
			comboImageCategoryFolders.Items.Clear();
			comboImageCategoryFolders.Items.Add("none");
			int selectedIndex=0;
			if(listDefsImageCategories.Count==0) {
				comboImageCategoryFolders.SetSelected(selectedIndex);
				return;
			}
			comboImageCategoryFolders.Items.AddDefs(listDefsImageCategories);
			comboImageCategoryFolders.SetSelectedDefNum(defNumImageCategory);
		}

		private void comboGlobalFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			errorProvider1.SetError(comboFilterDefault,string.Empty);//Clear the error, if applicable.
		}

		private void butTaskInboxSetup_Click(object sender,EventArgs e) {
			//If we ever allow users to enter this window without Setup permissions add Setup permission check here.
			using FormTaskInboxSetup formTaskInboxSetup=new FormTaskInboxSetup();
			formTaskInboxSetup.ShowDialog();
		}

		private void checkTaskListAlwaysShow_CheckedChanged(object sender,EventArgs e) {
			if(checkTaskListAlwaysShow.Checked) {
				groupBoxComputerDefaults.Enabled=true;
			}
			else {
				groupBoxComputerDefaults.Enabled=false;
			}
		}

		private void checkBoxTaskKeepListHidden_CheckedChanged(object sender,EventArgs e) {
			if(checkBoxTaskKeepListHidden.Checked) {
				radioBottom.Enabled=false;
				radioRight.Enabled=false;
				labelX.Enabled=false;
				labelY.Enabled=false;
				validNumX.Enabled=false;
				validNumY.Enabled=false;
			}
			else {
				radioBottom.Enabled=true;
				radioRight.Enabled=true;
				labelX.Enabled=true;
				labelY.Enabled=true;
				validNumX.Enabled=true;
				validNumY.Enabled=true;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!validNumX.IsValid() | !validNumY.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(FormOpenDental.IsDashboardVisible && checkTaskListAlwaysShow.Checked && !checkBoxTaskKeepListHidden.Checked && radioRight.Checked) {
				MsgBox.Show(this,"Tasks cannot be docked to the right when Dashboards are in use.");
				return;
			}
			bool isChanged=false;
			isChanged |= Prefs.UpdateBool(PrefName.TaskListAlwaysShowsAtBottom,checkTaskListAlwaysShow.Checked);
			isChanged |= Prefs.UpdateBool(PrefName.TasksUseRepeating,checkShowLegacyRepeatingTasks.Checked);
			isChanged |= Prefs.UpdateBool(PrefName.TasksNewTrackedByUser,checkTasksNewTrackedByUser.Checked);
			isChanged |= Prefs.UpdateBool(PrefName.TasksShowOpenTickets,checkShowOpenTickets.Checked);
			isChanged |= Prefs.UpdateBool(PrefName.TaskSortApptDateTime,checkTaskSortApptDateTime.Checked);
			isChanged |= Prefs.UpdateBool(PrefName.TasksForApptAllowMultiple,checkTaskToApptOneToOne.Checked);
			isChanged |= Prefs.UpdateInt(PrefName.TasksGlobalFilterType,(int)comboFilterDefault.GetSelected<EnumTaskFilterType>());
			isChanged |= Prefs.UpdateLong(PrefName.TaskAttachmentCategory,comboImageCategoryFolders.GetSelected<Def>()?.DefNum??0);//If 'none' is selected, def will be null so set the pref to 0
			if(ComputerPrefs.LocalComputer.TaskKeepListHidden!=checkBoxTaskKeepListHidden.Checked) {
				ComputerPrefs.LocalComputer.TaskKeepListHidden=checkBoxTaskKeepListHidden.Checked;
				isChanged=true;//needed to trigger screen refresh
			}
			if(radioBottom.Checked && ComputerPrefs.LocalComputer.TaskDock!=0) {
				ComputerPrefs.LocalComputer.TaskDock=0;
				isChanged=true;
			}
			else if(!radioBottom.Checked && ComputerPrefs.LocalComputer.TaskDock!=1) {
				ComputerPrefs.LocalComputer.TaskDock=1;
				isChanged=true;
			}
			if(ComputerPrefs.LocalComputer.TaskX!=PIn.Int(validNumX.Text)) {
				ComputerPrefs.LocalComputer.TaskX=PIn.Int(validNumX.Text);
				isChanged=true;
			}
			if(ComputerPrefs.LocalComputer.TaskY!=PIn.Int(validNumY.Text)) {
				ComputerPrefs.LocalComputer.TaskY=PIn.Int(validNumY.Text);
				isChanged=true;
			}
			if(isChanged) {
				DataValid.SetInvalid(InvalidType.Prefs,InvalidType.Computers);
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			DialogResult=DialogResult.OK;
		}

	}
}