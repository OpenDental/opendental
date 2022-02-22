using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBillingTypeMerge:FormODBase {
		private long _defNumInto;
		private long _defNumFrom;

		public FormBillingTypeMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butChangeInto_Click(object sender,EventArgs e) {
			using FormDefinitionPicker FormDefinitionPicker=new FormDefinitionPicker(DefCat.BillingTypes);
			FormDefinitionPicker.IsMultiSelectionMode=false;
			if(FormDefinitionPicker.ShowDialog()==DialogResult.OK && FormDefinitionPicker.ListDefsSelected.Count>0) {
				textDefNumInto.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum.ToString();
				textNameInto.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemName;
				textItemValueInto.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemValue;
				_defNumInto=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum;
				CheckUIState();
			}
		}

		private void butChangeFrom_Click(object sender,EventArgs e) {
			using FormDefinitionPicker FormDefinitionPicker=new FormDefinitionPicker(DefCat.BillingTypes);
			FormDefinitionPicker.IsMultiSelectionMode=false;
			if(FormDefinitionPicker.ShowDialog()==DialogResult.OK && FormDefinitionPicker.ListDefsSelected.Count>0) {
				textDefNumFrom.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum.ToString();
				textNameFrom.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemName;
				textItemValueFrom.Text=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemValue;
				_defNumFrom=FormDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum;
				CheckUIState();
			}
		}
		
		///<summary>Double check the state and status of the form.</summary>
		private void CheckUIState() {
			butMerge.Enabled=(_defNumFrom>0 && _defNumInto>0);
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
				return;
			}
			if(_defNumInto==_defNumFrom) { 
				MsgBox.Show(this,"Cannot merge the same Billing Type. Please update either the merge into field or the merge From field.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure? The results are permanent and cannot be undone.")) {
				return;
			}
			try {
				Defs.MergeBillingTypeDefNums(_defNumFrom,_defNumInto);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Billing Types failed to merge."),ex);
				return;
			}
			DefL.HideDef(Defs.GetDef(DefCat.BillingTypes,_defNumFrom));
			DataValid.SetInvalid(InvalidType.Defs,InvalidType.ClinicPrefs,InvalidType.Prefs,InvalidType.Programs);
			string logText=Lan.g(this,"Billing Type Merge from")
				+" "+Defs.GetName(DefCat.BillingTypes,_defNumFrom)+" "+Lan.g(this,"to")+" "+Defs.GetName(DefCat.BillingTypes,_defNumInto);
			//Make log entry here.
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,logText);
			MsgBox.Show(this,"Billing Types merged successfully.");
			textDefNumFrom.Clear();
			textNameFrom.Clear();
			textItemValueFrom.Clear();
			_defNumFrom=0;
			CheckUIState();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}