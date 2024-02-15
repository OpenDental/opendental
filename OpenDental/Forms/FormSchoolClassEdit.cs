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
		public SchoolClass SchoolClassCur;

		///<summary></summary>
		public FormSchoolClassEdit() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSchoolClassEdit_Load(object sender, System.EventArgs e) {
			if(SchoolClassCur.GradYear!=0){
				textGradYear.Text=SchoolClassCur.GradYear.ToString();
			}
			textDescript.Text=SchoolClassCur.Descript;
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
					SchoolClasses.Delete(SchoolClassCur.SchoolClassNum);
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textGradYear.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textGradYear.Text==""){
				MsgBox.Show(this,"Please enter a graduation year.");
				return;
			}
			SchoolClassCur.GradYear=PIn.Int(textGradYear.Text);
			SchoolClassCur.Descript=textDescript.Text;
			SchoolClasses.InsertOrUpdate(SchoolClassCur,IsNew);
			DialogResult=DialogResult.OK;
		}

		private void FormSchoolClassEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.DentalSchools);
			}
		}

	}
}