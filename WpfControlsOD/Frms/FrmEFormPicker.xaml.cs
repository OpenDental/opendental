using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary>This can be used from FormPatientForms to pick a form for a patient. It can also be used from FrmEFormDefs to pick an internal form to add to list of custom forms.</summary>
	public partial class FrmEFormPicker:FrmODBase {
		private List<EFormDef> _listEFormDefs;
		///<summary>After window closes, and only if IsDialogOK=true. ListEFormFieldDefs will also be properly filled because we may or may not be dealing with an internal EForm. If user clicks Blank, this will be a new EFormDef with no fields and not inserted in db.</summary>
		public EFormDef EFormDefSelected;
		///<summary>Set to true when this Frm is being used from FrmEFormDefs to pick an internal form to add to list of custom forms. Otherwise, only custom forms will show.</summary>
		public bool IsInternalPicker;

		public FrmEFormPicker(bool isWebForm = false):base() {
			InitializeComponent();
			Load+=FrmEFormPicker_Load;
			listMain.MouseDoubleClick+=listMain_DoubleClick;
			PreviewKeyDown+=FrmEFormPicker_PreviewKeyDown;
		}

		private void FrmEFormPicker_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(IsInternalPicker){
				_listEFormDefs=EFormInternal.GetAllInternal();
			}
			else{
				//picking from existing custom forms from db
				labelHead.Visible=false;
				butBlank.Visible=false;
				labelAdd.Visible=false;
				_listEFormDefs=EFormDefs.GetDeepCopy();
			}
			listMain.Items.Clear();
			listMain.Items.AddList(_listEFormDefs,x => x.Description);
		}

		private void listMain_DoubleClick(object sender,MouseButtonEventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			EFormDefSelected=listMain.GetSelected<EFormDef>();
			if(!IsInternalPicker){
				EFormDefSelected.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==EFormDefSelected.EFormDefNum);
			}
			IsDialogOK=true;
		}

		private void butBlank_Click(object sender,EventArgs e) {
			//only visible when not IsInternalPicker
			EFormDefSelected=new EFormDef();
			EFormDefSelected.ListEFormFieldDefs=new List<EFormFieldDef>();
			EFormDefSelected.ShowLabelsBold=true;
			EFormDefSelected.FormType=EnumEFormType.PatientForm;
			EFormDefSelected.Description="Form";
			EFormDefSelected.MaxWidth=450;
			EFormDefSelected.SpaceBelowEachField=-1;
			EFormDefSelected.SpaceToRightEachField=-1;
			IsDialogOK=true;
		}

		private void FrmEFormPicker_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listMain.SelectedIndex==-1){
				MsgBox.Show(this,"Please select one item first.");
				return;
			}
			EFormDefSelected=listMain.GetSelected<EFormDef>();
			if(!IsInternalPicker){
				EFormDefSelected.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==EFormDefSelected.EFormDefNum);
			}
			IsDialogOK=true;
		}
	}
}