using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormStateAbbrEdit:FormODBase {
		private StateAbbr _stateAbbrCur;

		public FormStateAbbrEdit(StateAbbr stateAbbr) {
			_stateAbbrCur=stateAbbr;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormStateAbbrEdit_Load(object sender,EventArgs e) {
			textDescription.Text=_stateAbbrCur.Description;
			textAbbr.Text=_stateAbbrCur.Abbr;
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				if(_stateAbbrCur.MedicaidIDLength!=0) {
					textMedIDLength.Text=_stateAbbrCur.MedicaidIDLength.ToString();
				}
			}
			else {
				labelMedIDLength.Visible=false;
				textMedIDLength.Visible=false;
				this.Height-=30;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_stateAbbrCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete State Abbr?")) {
				return;
			}
			StateAbbrs.Delete(_stateAbbrCur.StateAbbrNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			if(textAbbr.Text=="") {
				MsgBox.Show(this,"Abbrevation cannot be blank.");
				return;
			}
			if(textMedIDLength.Visible && !textMedIDLength.IsValid()) {
				MsgBox.Show(this,"Medicaid ID length is invalid.");
				return;
			}
			_stateAbbrCur.Description=textDescription.Text;
			_stateAbbrCur.Abbr=textAbbr.Text;
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				_stateAbbrCur.MedicaidIDLength=0;
				if(textMedIDLength.Text!="") {
					_stateAbbrCur.MedicaidIDLength=PIn.Int(textMedIDLength.Text);
				}
			}
			if(_stateAbbrCur.IsNew) {
				StateAbbrs.Insert(_stateAbbrCur);
			}
			else {
				StateAbbrs.Update(_stateAbbrCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}