using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEvaluations:FormODBase {
		private Provider _provider;
		private List<Provider> _listProvidersInstructors;
		private List<SchoolCourse> _listSchoolCourses;

		public FormEvaluations() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEvaluations_Load(object sender,EventArgs e) {
			_provider=Providers.GetProv(Security.CurUser.ProvNum);
			//_userProv will only be allowed to be null if the user is an admin. Checking for null in this block is not necessary.
			if(!Security.IsAuthorized(EnumPermType.AdminDentalEvaluations,suppressMessage:true)) {
				//Admins are allowed to look at and edit all evaluations, but they cannot add new evaluations
				//This could easily be added in the future if desired.
				groupAdmin.Visible=false;
			}
			if(_provider==null || !_provider.IsInstructor) {
				//only Instructors are able to add new evaluations.
				butAdd.Visible=false;
			}
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			comboCourse.Items.Add("All");
			for(int i=0;i<_listSchoolCourses.Count;i++) {
				comboCourse.Items.Add(_listSchoolCourses[i].CourseID);
			}
			comboCourse.SelectedIndex=0;
			_listProvidersInstructors=Providers.GetInstructors();
			comboInstructor.Items.Add("All");
			for(int i=0;i<_listProvidersInstructors.Count;i++) {
				comboInstructor.Items.Add(_listProvidersInstructors[i].GetLongDesc());
				if(_provider!=null && _provider.IsInstructor) {
					if(_listProvidersInstructors[i].ProvNum==_provider.ProvNum) {
						//add 1 to account for the 'all' item that is not in the list of provs but is in the combobox. If the user is not an admin then they are locked into only viewing their own evaluations.
						comboInstructor.SelectedIndex=i+1;
					}
				}
			}
			//Not an instructor so default to having 'All' selected.
			if(_provider==null || !_provider.IsInstructor) {
				comboInstructor.SelectedIndex=0;
			}
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		private void FillGrid() {
			long schoolCourseNum=0;
			if(comboCourse.SelectedIndex!=0) {
				schoolCourseNum=_listSchoolCourses[comboCourse.SelectedIndex-1].SchoolCourseNum;
			}
			long provNum=0;
			if(comboInstructor.SelectedIndex!=0) {
				provNum=_listProvidersInstructors[comboInstructor.SelectedIndex-1].ProvNum;
			}
			DataTable table=Evaluations.GetFilteredList(DateTime.Parse(textDateStart.Text),DateTime.Parse(textDateEnd.Text),textLastName.Text,textFirstName.Text,PIn.Long(textProvNum.Text),schoolCourseNum,provNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEvaluations","Date"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Title"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Instructor"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","ProvNum"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Last Name"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","First Name"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Course"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Grade"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEvaluations","Grading Scale"),90);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DateTime.Parse(table.Rows[i]["DateEval"].ToString()).ToShortDateString());
				row.Cells.Add(table.Rows[i]["EvalTitle"].ToString());
				row.Cells.Add(table.Rows[i]["InstructNum"].ToString());
				row.Cells.Add(table.Rows[i]["StudentNum"].ToString());
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				row.Cells.Add(table.Rows[i]["CourseID"].ToString());
				row.Cells.Add(table.Rows[i]["OverallgradeShowing"].ToString());
				row.Cells.Add(table.Rows[i]["Description"].ToString());
				row.Tag=table.Rows[i]["EvaluationNum"].ToString();//To keep the correct reference to the Evaluation even when filtering the list.
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEvaluationEdit formEvaluationEdit=new FormEvaluationEdit(Evaluations.GetOne(PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString())));
			formEvaluationEdit.ShowDialog();
			FillGrid();
		}

		private void comboCourse_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateStart.Text=="" || textDateEnd.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEvaluationDefs formEvaluationDefs=new FormEvaluationDefs();
			formEvaluationDefs.IsSelectionMode=true;
			formEvaluationDefs.CourseIndex=comboCourse.SelectedIndex;
			formEvaluationDefs.ShowDialog();
			if(formEvaluationDefs.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butReport_Click(object sender,EventArgs e) {
			//This is currently hidden until it is finished.
			using FormEvaluationReport formEvaluationReport=new FormEvaluationReport();
			formEvaluationReport.ShowDialog();
		}

	}
}