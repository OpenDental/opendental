using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormSchoolCourses : FormODBase {
		public bool IsSelectionMode;
		public SchoolCourse CourseSelected;
		private List<SchoolCourse> _listSchoolCourses;

		///<summary></summary>
		public FormSchoolCourses(){
			InitializeComponent();
			InitializeLayoutManager();
			//Providers.Selected=-1;
			Lan.F(this);
		}

		private void FormSchoolCourses_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode) {
				butAdd.Visible=false;
				butOK.Visible=true;
				butClose.Text="Cancel";
			}
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormSchoolCourses","Course ID"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationDefEdit","Description"),80);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSchoolCourses.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listSchoolCourses[i].CourseID);
				row.Cells.Add(_listSchoolCourses[i].Descript);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			SchoolCourse cur=new SchoolCourse();
			using FormSchoolCourseEdit FormS=new FormSchoolCourseEdit(cur);
			FormS.IsNew=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			if(IsSelectionMode) {
				CourseSelected=_listSchoolCourses[gridMain.GetSelectedIndex()];
				DialogResult=DialogResult.OK;
				return;
			}
			using FormSchoolCourseEdit FormS=new FormSchoolCourseEdit(_listSchoolCourses[gridMain.GetSelectedIndex()]);
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a course.");
				return;
			}
			if(IsSelectionMode) {
				CourseSelected=_listSchoolCourses[gridMain.GetSelectedIndex()];
				DialogResult=DialogResult.OK;
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
