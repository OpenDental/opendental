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
	public partial class FormSchoolClassEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
    private SchoolClass ClassCur;

		///<summary></summary>
		public FormSchoolClassEdit(SchoolClass classCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			ClassCur=classCur.Copy();
			Lan.F(this);
		}

		private void FormSchoolClassEdit_Load(object sender, System.EventArgs e) {
			if(ClassCur.GradYear!=0){
				textGradYear.Text=ClassCur.GradYear.ToString();
			}
			textDescript.Text=ClassCur.Descript;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Dental School Class?")){
					return;
				}
				try{
					SchoolClasses.Delete(ClassCur.SchoolClassNum);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textGradYear.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textGradYear.Text==""){
				MsgBox.Show(this,"Please enter a graduation year.");
				return;
			}
			ClassCur.GradYear=PIn.Int(textGradYear.Text);
			ClassCur.Descript=textDescript.Text;
			SchoolClasses.InsertOrUpdate(ClassCur,IsNew);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormSchoolClassEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.DentalSchools);
			}
		}
	}
}





















