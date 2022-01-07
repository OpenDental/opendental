using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTelephone : FormODBase {

		///<summary></summary>
		public FormTelephone()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTelephone_Load(object sender, System.EventArgs e) {
		
		}
		
		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void butReformat_Click(object sender, System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name!="en-US"){
				if(MessageBox.Show(Lan.g(this,"Are you sure?  The phone number formatting is only meant for the United States?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			Patients.ReformatAllPhoneNumbers();
			//refresh carriers:
			DataValid.SetInvalid(InvalidType.Carriers);
			MessageBox.Show(Lan.g(this,"Telephone numbers reformatted."));
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Telephone");
		}
			
		
		

	}
}










