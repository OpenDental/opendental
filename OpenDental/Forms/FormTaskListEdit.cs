using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTaskListEdit : FormODBase {
		private TaskList _taskList;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormTaskListEdit(TaskList taskList)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_taskList=taskList;
			Lan.F(this);
		}

		private void FormTaskListEdit_Load(object sender, System.EventArgs e) {
			if(ODBuild.IsDebug()) {
				labelTaskListNum.Visible=true;
				textTaskListNum.Visible=true;
				textTaskListNum.Text=_taskList.TaskListNum.ToString();
			}
			bool isTasksUseRepeating=PrefC.GetBool(PrefName.TasksUseRepeating);
			if(!isTasksUseRepeating){//Repeating Task List (Legacy) disabled.
				labelDate.Visible=false;
				labelDate2.Visible=false;
				labelDateType.Visible=false;
				textDateTL.Visible=false;
				listDateType.Visible=false;
				checkFromNum.Visible=false;
			}
			textDescript.Text=_taskList.Descript;
			if(_taskList.DateTL.Year>1880){
				textDateTL.Text=_taskList.DateTL.ToShortDateString();
			}
			listDateType.Items.AddEnums<TaskDateType>();
			listDateType.SetSelectedEnum(_taskList.DateType);
			if(_taskList.FromNum==0){
				checkFromNum.Checked=false;
				checkFromNum.Enabled=false;
			}
			else{
				checkFromNum.Checked=true;
			}
			if(_taskList.IsRepeating){
				textDateTL.Enabled=false;
				listObjectType.Enabled=false;
				if(_taskList.Parent!=0){//not a main parent
					listDateType.Enabled=false;
				}
			}
			listObjectType.Items.AddEnums<TaskObjectType>();
			listObjectType.SetSelectedEnum(_taskList.ObjectType);
			FillComboGlobalFilter();
		}

		private void FillComboGlobalFilter() {
			if((GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==GlobalTaskFilterType.Disabled) {
				comboGlobalFilter.Visible=false;
				labelGlobalFilter.Visible=false;
				return;
			}
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Default.GetDescription()),GlobalTaskFilterType.Default);
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.None.GetDescription()),GlobalTaskFilterType.None);
			if(PrefC.HasClinicsEnabled) {
				comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Clinic.GetDescription()),GlobalTaskFilterType.Clinic);
				if(Defs.GetDefsForCategory(DefCat.Regions).Count>0) {
					comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Region.GetDescription()),GlobalTaskFilterType.Region);
				}
			}
			comboGlobalFilter.SetSelectedEnum(_taskList.GlobalTaskFilterType);
			if(comboGlobalFilter.SelectedIndex!=-1) {
				return;
			}
			errorProvider1.SetError(comboGlobalFilter,$"Previous selection \"{_taskList.GlobalTaskFilterType.GetDescription()}\" is no longer available.  "
				+"Saving will overwrite previous setting.");
			comboGlobalFilter.SelectedIndex=0;
		}

		private void comboGlobalFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			errorProvider1.SetError(comboGlobalFilter,string.Empty);//Clear the error, if applicable.
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateTL.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_taskList.Descript=textDescript.Text;
			_taskList.DateTL=PIn.Date(textDateTL.Text);
			_taskList.DateType=listDateType.GetSelected<TaskDateType>();
			if(!checkFromNum.Checked){//user unchecked the box
				_taskList.FromNum=0;
			}
			_taskList.ObjectType=listObjectType.GetSelected<TaskObjectType>();
			_taskList.GlobalTaskFilterType=comboGlobalFilter.GetSelected<GlobalTaskFilterType>();
			if(IsNew) {
				try{
					TaskLists.Insert(_taskList);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.TaskListCreate,0,_taskList.Descript+" "+Lan.g(this,"added"));
				DialogResult=DialogResult.OK;
				return;
			}
			try{
				TaskLists.Update(_taskList);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















