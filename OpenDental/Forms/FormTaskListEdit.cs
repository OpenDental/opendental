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
		private TaskList Cur;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormTaskListEdit(TaskList cur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Cur=cur;
			Lan.F(this);
		}

		private void FormTaskListEdit_Load(object sender, System.EventArgs e) {
			if(ODBuild.IsDebug()) {
				labelTaskListNum.Visible=true;
				textTaskListNum.Visible=true;
				textTaskListNum.Text=Cur.TaskListNum.ToString();
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
			textDescript.Text=Cur.Descript;
			if(Cur.DateTL.Year>1880){
				textDateTL.Text=Cur.DateTL.ToShortDateString();
			}
			listDateType.Items.AddEnums<TaskDateType>();
			listDateType.SetSelectedEnum(Cur.DateType);
			if(Cur.FromNum==0){
				checkFromNum.Checked=false;
				checkFromNum.Enabled=false;
			}
			else{
				checkFromNum.Checked=true;
			}
			if(Cur.IsRepeating){
				textDateTL.Enabled=false;
				listObjectType.Enabled=false;
				if(Cur.Parent!=0){//not a main parent
					listDateType.Enabled=false;
				}
			}
			listObjectType.Items.AddEnums<TaskObjectType>();
			listObjectType.SetSelectedEnum(Cur.ObjectType);
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
			comboGlobalFilter.SetSelectedEnum(Cur.GlobalTaskFilterType);
			if(comboGlobalFilter.SelectedIndex==-1) {
				errorProvider1.SetError(comboGlobalFilter,$"Previous selection \"{Cur.GlobalTaskFilterType.GetDescription()}\" is no longer available.  "
					+"Saving will overwrite previous setting.");
				comboGlobalFilter.SelectedIndex=0;
			}
		}

		private void comboGlobalFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			errorProvider1.SetError(comboGlobalFilter,string.Empty);//Clear the error, if applicable.
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateTL.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			Cur.Descript=textDescript.Text;
			Cur.DateTL=PIn.Date(textDateTL.Text);
			Cur.DateType=listDateType.GetSelected<TaskDateType>();
			if(!checkFromNum.Checked){//user unchecked the box
				Cur.FromNum=0;
			}
			Cur.ObjectType=listObjectType.GetSelected<TaskObjectType>();
			Cur.GlobalTaskFilterType=comboGlobalFilter.GetSelected<GlobalTaskFilterType>();
			try{
				if(IsNew) {
					TaskLists.Insert(Cur);
					SecurityLogs.MakeLogEntry(Permissions.TaskListCreate,0,Cur.Descript+" "+Lan.g(this,"added"));
				}
				else {
					TaskLists.Update(Cur);
				}
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





















