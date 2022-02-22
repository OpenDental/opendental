using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormApptFieldEdit:FormODBase {
		private ApptField _field;

		///<summary></summary>
		public FormApptFieldEdit(ApptField field) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_field=field;
		}

		private void FormApptFieldEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_field.FieldName;
			textValue.Text=_field.FieldValue;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			_field.FieldValue=textValue.Text;
			if(_field.FieldValue=="") {//if blank, then delete
				if(_field.IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				ApptFields.DeleteFieldForAppt(_field.FieldName,_field.AptNum);
			}
			else {
				ApptFields.Upsert(_field);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















