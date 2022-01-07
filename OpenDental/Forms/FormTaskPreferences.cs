using System;
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
			FillComboGlobalFilter();
		}

		///<summary>Fills the Global Task filter combobox with options.  Only visible if Clinics are enabled, or if previous selection no is longer 
		///available, example: Clinics have been turned off, Clinic filter no longer available.</summary>
		private void FillComboGlobalFilter() {
			GlobalTaskFilterType globalPref=(GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType);
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Disabled.GetDescription()),GlobalTaskFilterType.Disabled);
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.None.GetDescription()),GlobalTaskFilterType.None);
			if(PrefC.HasClinicsEnabled) {
				labelGlobalFilter.Visible=true;
				comboGlobalFilter.Visible=true;
				comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Clinic.GetDescription()),GlobalTaskFilterType.Clinic);
				if(Defs.GetDefsForCategory(DefCat.Regions).Count>0) {
					comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Region.GetDescription()),GlobalTaskFilterType.Region);
				}
			}
			comboGlobalFilter.SetSelectedEnum(globalPref);
			if(comboGlobalFilter.SelectedIndex==-1) {
				labelGlobalFilter.Visible=true;
				comboGlobalFilter.Visible=true;
				errorProvider1.SetError(comboGlobalFilter,$"Previous selection \"{globalPref.GetDescription()}\" is no longer available.  "
					+"Saving will overwrite previous setting.");
				comboGlobalFilter.SelectedIndex=0;
			}
		}

		private void comboGlobalFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			errorProvider1.SetError(comboGlobalFilter,string.Empty);//Clear the error, if applicable.
		}

		private void butTaskInboxSetup_Click(object sender,EventArgs e) {
			//If we ever allow users to enter this window without Setup permissions add Setup permission check here.
			using FormTaskInboxSetup FormT=new FormTaskInboxSetup();
			FormT.ShowDialog();
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

		private void butOK_Click(object sender,EventArgs e) {
			if(!validNumX.IsValid() | !validNumY.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(FormOpenDental.IsDashboardVisible && checkTaskListAlwaysShow.Checked && !checkBoxTaskKeepListHidden.Checked && radioRight.Checked) {
				MsgBox.Show(this,"Tasks cannot be docked to the right when Dashboards are in use.");
				return;
			}
			bool changed = false;
			if(Prefs.UpdateBool(PrefName.TaskListAlwaysShowsAtBottom,checkTaskListAlwaysShow.Checked)
				| Prefs.UpdateBool(PrefName.TasksUseRepeating,checkShowLegacyRepeatingTasks.Checked)
				| Prefs.UpdateBool(PrefName.TasksNewTrackedByUser,checkTasksNewTrackedByUser.Checked)
				| Prefs.UpdateBool(PrefName.TasksShowOpenTickets,checkShowOpenTickets.Checked)
				| Prefs.UpdateBool(PrefName.TaskSortApptDateTime,checkTaskSortApptDateTime.Checked)
				| Prefs.UpdateInt(PrefName.TasksGlobalFilterType,(int)comboGlobalFilter.GetSelected<GlobalTaskFilterType>())
				) {
				changed=true;
			}
			if(ComputerPrefs.LocalComputer.TaskKeepListHidden!=checkBoxTaskKeepListHidden.Checked) {
				ComputerPrefs.LocalComputer.TaskKeepListHidden=checkBoxTaskKeepListHidden.Checked;
				changed=true;//needed to trigger screen refresh
			}
			if(radioBottom.Checked && ComputerPrefs.LocalComputer.TaskDock!=0) {
				ComputerPrefs.LocalComputer.TaskDock=0;
				changed=true;
			}
			else if(!radioBottom.Checked && ComputerPrefs.LocalComputer.TaskDock!=1) {
				ComputerPrefs.LocalComputer.TaskDock=1;
				changed=true;
			}
			if(ComputerPrefs.LocalComputer.TaskX!=PIn.Int(validNumX.Text)) {
				ComputerPrefs.LocalComputer.TaskX=PIn.Int(validNumX.Text);
				changed=true;
			}
			if(ComputerPrefs.LocalComputer.TaskY!=PIn.Int(validNumY.Text)) {
				ComputerPrefs.LocalComputer.TaskY=PIn.Int(validNumY.Text);
				changed=true;
			}
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs,InvalidType.Computers);
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}