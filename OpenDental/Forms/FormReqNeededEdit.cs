using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReqNeededEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
    public ReqNeeded ReqCur;

		///<summary></summary>
		public FormReqNeededEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqNeededEdit_Load(object sender, System.EventArgs e) {
			/*for(int i=0;i<SchoolClasses.List.Length;i++) {
				comboClass.Items.Add(SchoolClasses.GetDescript(SchoolClasses.List[i]));
				if(SchoolClasses.List[i].SchoolClassNum==ReqCur.SchoolClassNum){
					comboClass.SelectedIndex=i;
				}
			}
			for(int i=0;i<SchoolCourses.List.Length;i++) {
				comboCourse.Items.Add(SchoolCourses.GetDescript(SchoolCourses.List[i]));
				if(SchoolCourses.List[i].SchoolCourseNum==ReqCur.SchoolCourseNum) {
					comboCourse.SelectedIndex=i;
				}
			}*/
			textDescript.Text=ReqCur.Descript;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				string inuseby=ReqStudents.InUseBy(ReqCur.ReqNeededNum);
				if(inuseby!=""){
					using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(Lan.g(this,"Requirement is already in use by student(s) with grade point(s) attached."
						+"\r\n"+Lan.g(this,"Delete anyway?  Student grades will not be affected."))
						+"\r\n"+inuseby);
					msgBox.ShowDialog();
					if(msgBox.DialogResult != DialogResult.OK) {
						return;
					}
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Requirement?")){
					return;
				}
			}
			ReqCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescript.Text==""){
				MsgBox.Show(this,"Please enter a description first.");
				return;
			}
			ReqCur.Descript=textDescript.Text;
			//ReqCur.SchoolClassNum=SchoolClasses.List[comboClass.SelectedIndex].SchoolClassNum;
			//ReqCur.SchoolCourseNum=SchoolCourses.List[comboCourse.SelectedIndex].SchoolCourseNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















