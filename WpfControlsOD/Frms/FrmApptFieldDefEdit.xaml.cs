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
	public partial class FrmApptFieldDefEdit:FrmODBase {
		///<summary></summary>
		public bool IsNew;
		public ApptFieldDef ApptFieldDef;
		private string _fieldNameOld;

		///<summary></summary>
		public FrmApptFieldDefEdit()
		{
			InitializeComponent();
			KeyDown+=Frm_KeyDown;
			//Lan.F(this);
		}

		private void FrmApptFieldDefEdit_Loaded(object sender,RoutedEventArgs e) {
			textName.Text=ApptFieldDef.FieldName;
			textPickList.Visible=false;
			labelWarning.Visible=false;
			comboFieldType.Items.Clear();
			comboFieldType.Items.AddList<string>(Enum.GetNames(typeof(ApptFieldType)));
			comboFieldType.SelectedIndex=(int)ApptFieldDef.FieldType;
			if(!IsNew){
				_fieldNameOld=ApptFieldDef.FieldName;
			}
			if(comboFieldType.SelectedIndex==(int)ApptFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
				textPickList.Text=ApptFieldDef.PickList;
			}
		}

		private void comboFieldType_SelectedIndexChanged(object sender,EventArgs e) {
			textPickList.Visible=false;
			labelWarning.Visible=false;
			if(comboFieldType.SelectedIndex==(int)ApptFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
			}
		}

		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				IsDialogOK=false;
				return;
			}
			try{
				ApptFieldDefs.Delete(ApptFieldDef);//Throws if in use.
				FieldDefLinks.DeleteForFieldDefNum(ApptFieldDef.ApptFieldDefNum,FieldDefTypes.Appointment);//Delete any FieldDefLinks to this ApptFieldDef
				ApptFieldDef=null;
				ApptFieldDefs.RefreshCache();
				IsDialogOK=true;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(_fieldNameOld!=textName.Text) {
				if(ApptFieldDefs.GetExists(x => x.FieldName==textName.Text)) {
					MsgBox.Show(this,"Field name currently being used.");
					return;
				}
			}
			ApptFieldDef.FieldName=textName.Text;
			ApptFieldDef.FieldType=(ApptFieldType)comboFieldType.SelectedIndex;
			if(ApptFieldDef.FieldType==ApptFieldType.PickList) {
			  if(textPickList.Text=="") {
			    MsgBox.Show(this,"List cannot be blank.");
			    return;
			  }
			  ApptFieldDef.PickList=textPickList.Text;
			}
			if(IsNew) {
			  ApptFieldDefs.Insert(ApptFieldDef);
			}
			else {
			  ApptFieldDefs.Update(ApptFieldDef,_fieldNameOld);
			}
			ApptFieldDefs.RefreshCache();
			IsDialogOK=true;
		}
	}
}





















