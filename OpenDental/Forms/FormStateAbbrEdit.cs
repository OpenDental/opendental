using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormStateAbbrEdit:FormODBase {
		private StateAbbr _stateAbbr;

		public FormStateAbbrEdit(StateAbbr stateAbbr) {
			_stateAbbr=stateAbbr;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormStateAbbrEdit_Load(object sender,EventArgs e) {
			textDescription.Text=_stateAbbr.Description;
			textAbbr.Text=_stateAbbr.Abbr;
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				if(_stateAbbr.MedicaidIDLength!=0) {
					textMedIDLength.Text=_stateAbbr.MedicaidIDLength.ToString();
				}
			}
			else {
				labelMedIDLength.Visible=false;
				textMedIDLength.Visible=false;
				this.Height-=30;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_stateAbbr.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete State Abbr?")) {
				return;
			}
			StateAbbrs.Delete(_stateAbbr.StateAbbrNum);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
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
			_stateAbbr.Description=textDescription.Text;
			_stateAbbr.Abbr=textAbbr.Text;
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				_stateAbbr.MedicaidIDLength=0;
				if(textMedIDLength.Text!="") {
					_stateAbbr.MedicaidIDLength=PIn.Int(textMedIDLength.Text);
				}
			}
			if(_stateAbbr.IsNew) {
				StateAbbrs.Insert(_stateAbbr);
			}
			else {
				StateAbbrs.Update(_stateAbbr);
			}
			DialogResult=DialogResult.OK;
		}

	}
}