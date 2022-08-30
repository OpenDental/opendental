using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrProviderKeyEdit:FormODBase {
		private EhrProvKey _keyCur;

		///<summary>Only used from FormEhrProviderKeys.  keyCur can be a blank new key.  keyCur cannot be null.</summary>
		public FormEhrProviderKeyEdit(EhrProvKey keyCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_keyCur=keyCur;
		}

		private void FormEhrProviderKeyEdit_Load(object sender,EventArgs e) {
			textYear.Text=_keyCur.YearValue.ToString();
			textKey.Text=_keyCur.ProvKey;
			textLName.Text=_keyCur.LName;
			textFName.Text=_keyCur.FName;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_keyCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			EhrProvKeys.Delete(_keyCur.EhrProvKeyNum);
			//if the radiology proc alert is enabled and they just deleted the last ehrprovkey, disable the alert (OpenDentalService will stop alerting)
			if(PrefC.GetBool(PrefName.IsAlertRadiologyProcsEnabled) && !EhrProvKeys.HasEhrKeys()) {
				Prefs.UpdateBool(PrefName.IsAlertRadiologyProcsEnabled,false);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textYear.Text==""){
				MessageBox.Show("Please enter a year.");
				return;
			}
			if(!textYear.IsValid()) {
				MessageBox.Show("Invalid year, must be two digits.");
				return;
			}
			if(!FormEHR.ProvKeyIsValid(textLName.Text,textFName.Text,PIn.Int(textYear.Text),textKey.Text)) {
				MsgBox.Show(this,"Invalid provider key");
				return;
			}
			_keyCur.LName=textLName.Text;
			_keyCur.FName=textFName.Text;
			_keyCur.YearValue=PIn.Int(textYear.Text);
			_keyCur.ProvKey=textKey.Text;
			if(_keyCur.IsNew) {
				bool isFirstKey=false;
				if(!PrefC.GetBool(PrefName.IsAlertRadiologyProcsEnabled)) {
					isFirstKey=!EhrProvKeys.HasEhrKeys();
				}
				EhrProvKeys.Insert(_keyCur);
				//if radiology procs alert is disabled and this is the first ehrprovkey, enable the alert (alert handled by the OpenDentalService)
				if(!PrefC.GetBool(PrefName.IsAlertRadiologyProcsEnabled) && isFirstKey) {
					Prefs.UpdateBool(PrefName.IsAlertRadiologyProcsEnabled,true);
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				EhrProvKeys.Update(_keyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}