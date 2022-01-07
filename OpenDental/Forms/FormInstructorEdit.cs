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
	public partial class FormInstructorEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
    //private Instructor InstCur;

		///<summary></summary>
		public FormInstructorEdit()//Instructor instCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			//InstCur=instCur.Clone();
			Lan.F(this);
		}

		private void FormInstructorEdit_Load(object sender, System.EventArgs e) {
			/*textLName.Text=InstCur.LName;
			textFName.Text=InstCur.FName;
			textSuffix.Text=InstCur.Suffix;*/
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			/*if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				if(!MsgBox.Show(this,true,"Delete this Instructor?")){
					return;
				}
				try{
					Instructors.Delete(InstCur);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;*/
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			/*InstCur.LName=textLName.Text;
			InstCur.FName=textFName.Text;
			InstCur.Suffix=textSuffix.Text;
			Instructors.InsertOrUpdate(InstCur,IsNew);
			DialogResult=DialogResult.OK;*/
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			//DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















