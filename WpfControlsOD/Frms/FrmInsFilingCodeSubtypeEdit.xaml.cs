using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmInsFilingCodeSubtypeEdit:FrmODBase {
		public InsFilingCodeSubtype InsFilingCodeSubtypeCur;

		///<summary></summary>
		public FrmInsFilingCodeSubtypeEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmInsFilingCodeSubtypeEdit_Load;
		}

		private void FrmInsFilingCodeSubtypeEdit_Load(object sender,EventArgs e) {
			textDescription.Text=InsFilingCodeSubtypeCur.Descript;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(InsFilingCodeSubtypeCur.IsNew){
				IsDialogOK=false;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try{
				InsFilingCodeSubtypes.Delete(InsFilingCodeSubtypeCur.InsFilingCodeSubtypeNum);
				IsDialogOK=true;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(this.textDescription.Text==""){
				MessageBox.Show(Lans.g(this,"Please enter a description."));
				return;
			}
			InsFilingCodeSubtypeCur.Descript=textDescription.Text;
			IsDialogOK=true;
		}

	}
}





















