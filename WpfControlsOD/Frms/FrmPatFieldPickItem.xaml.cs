using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>Allows user to edit account. Form can be found at Manage->Accounting->Click on any Account->Edit</summary>
	public partial class FrmPatFieldPickItem : FrmODBase {
		public PatFieldPickItem PatFieldPickItemCur;

		public FrmPatFieldPickItem() {
			InitializeComponent();
			Load+=FrmAccountEdit_Load;
		}

		private void FrmAccountEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textAbbreviation.Text=PatFieldPickItemCur.Abbreviation;
			textName.Text=PatFieldPickItemCur.Name;
			checkHidden.Checked=PatFieldPickItemCur.IsHidden;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(PatFieldPickItemCur.IsNew) {
				IsDialogOK=false;
				return;
			}
			//if List item is currently in-use on any patients, block deletion.
			string PatFieldName=PatFieldDefs.GetFieldName(PatFieldPickItemCur.PatFieldDefNum);
			int InUse=PatFields.PickListItemInUseCount(PatFieldPickItemCur.Name,PatFieldName);
			if(InUse>0) {
				string message=Lang.g(this,"Cannot delete this item because it's currently associated with a number of patients:")+" "+InUse;
				MsgBox.Show(this,message);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This item is not currently associated with any patients. Delete this item?")) {
				return;
			}
			PatFieldPickItems.Delete(PatFieldPickItemCur.PatFieldPickItemNum);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!PatFieldPickItemCur.IsNew
				&& PatFieldPickItemCur.Name==textName.Text
				&& PatFieldPickItemCur.Abbreviation==textAbbreviation.Text
				&& PatFieldPickItemCur.IsHidden==checkHidden.Checked)
			{
				IsDialogOK=false;
				return;
			}
			List<PatFieldPickItem> listPatFieldPickItems=PatFieldPickItems.GetWhere(x=>x.PatFieldDefNum==PatFieldPickItemCur.PatFieldDefNum);
			if(PatFieldPickItemCur.Name!=textName.Text && listPatFieldPickItems.Exists(x=>x.Name==textName.Text)) {
				MsgBox.Show(this,"That name already exists in this picklist, please choose a different name.");
				return;
			}
			if(PatFieldPickItemCur.Abbreviation!=textAbbreviation.Text && listPatFieldPickItems.Exists(x=>x.Abbreviation==textAbbreviation.Text)) {
				MsgBox.Show(this,"That abbreviation already exists in this picklist, please choose a different abbreviation.");
				return;
			}
			if(!PatFieldPickItemCur.IsNew && PatFieldPickItemCur.Name!=textName.Text) {
				string patFieldName=PatFieldDefs.GetFieldName(PatFieldPickItemCur.PatFieldDefNum);
				PatFields.UpdatePatFieldValues(patFieldName,textName.Text,PatFieldPickItemCur.Name);
			}
			PatFieldPickItemCur.Name=textName.Text;
			PatFieldPickItemCur.Abbreviation=textAbbreviation.Text;
			PatFieldPickItemCur.IsHidden=checkHidden.Checked==true;
			if(PatFieldPickItemCur.IsNew) {
				PatFieldPickItems.Insert(PatFieldPickItemCur);
			}
			else {
				PatFieldPickItems.Update(PatFieldPickItemCur);
			}
			IsDialogOK=true;
		}
	}
}