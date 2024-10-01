using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormSchoolCourseEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private SchoolCourse _schoolCourse;

		///<summary></summary>
		public FormSchoolCourseEdit(SchoolCourse schoolCourseCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_schoolCourse=schoolCourseCur.Copy();
			Lan.F(this);
		}

		private void FormSchoolCourseEdit_Load(object sender, System.EventArgs e) {
			textCourseID.Text=_schoolCourse.CourseID;
			textDescript.Text=_schoolCourse.Descript;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Course?")){
				return;
			}
			try{
				SchoolCourses.Delete(_schoolCourse.SchoolCourseNum);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			_schoolCourse.CourseID=textCourseID.Text;
			_schoolCourse.Descript=textDescript.Text;
			SchoolCourses.InsertOrUpdate(_schoolCourse,IsNew);
			DialogResult=DialogResult.OK;
		}

		private void FormSchoolCourseEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.DentalSchools);
			}
		}

	}
}