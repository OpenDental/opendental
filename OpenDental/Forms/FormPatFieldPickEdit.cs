using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental {
	public partial class FormPatFieldPickEdit:FormODBase {
		private PatField _patField;
		private PatField _patFieldOld;
		private PatFieldPickItem _patFieldPickItem;
		private List<PatFieldPickItem> _listPatFieldPickItems;

		///<summary></summary>
		public FormPatFieldPickEdit(PatField patField) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patField=patField;
			_patFieldOld=_patField.Copy();
		}

		private void FormPatFieldPickEdit_Load(object sender, EventArgs e) {
			labelName.Text=_patField.FieldName;
			PatFieldDef patFieldDef=PatFieldDefs.GetFieldDefByFieldName(_patField.FieldName);
			_listPatFieldPickItems=PatFieldPickItems
				.GetWhere(x=>x.PatFieldDefNum==patFieldDef.PatFieldDefNum)
				.OrderBy(x=>x.ItemOrder).ToList();
			_patFieldPickItem=_listPatFieldPickItems.Find(x=>x.Name==_patField.FieldValue); //This will be null if fieldValue & name weren't properly synched after a change of the fieldVal or a rename of the pickItem
			listBoxPick.Items.AddList(_listPatFieldPickItems.FindAll(x=>!x.IsHidden),x=>x.Name);
			if(!_patField.IsNew && _patFieldPickItem?.IsHidden==true) {
				//this adds hidden item to list if patient already had it assigned.
				listBoxPick.Items.Add(_patFieldPickItem.Name,_patFieldPickItem);
			}
			if(!_patField.IsNew) {
				listBoxPick.SelectedItem=_patField.FieldValue;
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//no validation
			//no delete button, which is a unique pattern.
			if(_patFieldPickItem==(PatFieldPickItem)listBoxPick.SelectedItem) {
				//If nothing changed.
				//If new, then it's comparing null==null to see if nothing was selected.
				DialogResult=DialogResult.Cancel;
				return;
			}
			//If fieldValue & name weren't properly synched after a change of the fieldVal or a rename of the pickItem then we get a null _patFieldPickItem here.
			//This makes sure to update the selected item to avoid database corruption and null reference errors. See jobNum 52130.
			_patFieldPickItem??=(PatFieldPickItem)listBoxPick.SelectedItem;
			// Value changed to blank, i.e. user hit clear then save.
			if(listBoxPick.SelectedIndex==-1) {
				//guaranteed to be not new.
				PatFields.MakeDeleteLogEntry(_patFieldOld);
				PatFields.Delete(_patField);
				DialogResult=DialogResult.OK;
				return;
			}
			// A selection was made/changed.
			_patField.FieldValue=((PatFieldPickItem)listBoxPick.SelectedItem).Name;
			if(_patField.IsNew) {
				PatFields.Insert(_patField);
				DialogResult=DialogResult.OK;
				return;
			}
			if(_patFieldPickItem.IsHidden && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"You will not be able to revert this change because the previous value is hidden.  Continue?")) {
				return;
			}
			PatFields.Update(_patField);
			PatFields.MakeEditLogEntry(_patFieldOld,_patField);
			DialogResult=DialogResult.OK;
		}

		private void butClear_Click(object sender, EventArgs e) {
			listBoxPick.ClearSelected();
		}
	}
}