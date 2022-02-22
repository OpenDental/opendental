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
    private SchoolCourse CourseCur;

		///<summary></summary>
		public FormSchoolCourseEdit(SchoolCourse courseCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			CourseCur=courseCur.Copy();
			Lan.F(this);
		}

		private void FormSchoolCourseEdit_Load(object sender, System.EventArgs e) {
			textCourseID.Text=CourseCur.CourseID;
			textDescript.Text=CourseCur.Descript;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Course?")){
					return;
				}
				try{
					SchoolCourses.Delete(CourseCur.SchoolCourseNum);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			CourseCur.CourseID=textCourseID.Text;
			CourseCur.Descript=textDescript.Text;
			SchoolCourses.InsertOrUpdate(CourseCur,IsNew);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormSchoolCourseEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.DentalSchools);
			}
		}
	}
}





















