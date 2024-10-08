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
		///<summary>Set to true when this Frm is being used from FrmEFormDefs to pick an internal form to add to list of custom forms. Only custom forms will show.</summary>
		public bool IsInternalPicker;

		public FrmEFormPicker(bool isWebForm = false):base() {
			InitializeComponent();
			Load+=FrmEFormPicker_Load;
			listMain.MouseDoubleClick+=listMain_DoubleClick;
			PreviewKeyDown+=FrmEFormPicker_PreviewKeyDown;
		}

		private void FrmEFormPicker_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(!IsInternalPicker){
				//later on, we will make labelHead.Visible=false, but for now we will reuse it to warn users
				labelHead.Text="eForms is a new feature that is not yet available to patients";
				butBlank.Visible=false;
				labelAdd.Visible=false;
			}
			//This code is similar to that in FrmEFormDefs.
			_listEFormDefs=new List<EFormDef>();
			List<EFormDef> listEFormDefsInternal=EFormInternal.GetAllInternal();
			List<EFormDef> listEFormDefsCustom=EFormDefs.GetDeepCopy();
			List<EFormDef> listEFormDefsInternalShort=new List<EFormDef>();//this one will not have hidden/deleted
			for(int i=0;i<listEFormDefsInternal.Count;i++){
				if(listEFormDefsCustom.Exists(x=>x.IsInternalHidden && x.Description==listEFormDefsInternal[i].Description)){
					continue;//don't add forms that user had hidden/deleted
				}
				listEFormDefsInternalShort.Add(listEFormDefsInternal[i]);
			}
			//patient forms
			if(IsInternalPicker){
				_listEFormDefs.AddRange(listEFormDefsInternal.FindAll(x=>x.FormType==EnumEFormType.PatientForm));
			}
			else {
				_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.PatientForm && !x.IsInternalHidden));//ignore the ones used for internalHidden
				_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.PatientForm));
			}
			//medical history
			if(IsInternalPicker){
				_listEFormDefs.AddRange(listEFormDefsInternal.FindAll(x=>x.FormType==EnumEFormType.MedicalHistory));
			}
			else {
				_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.MedicalHistory && !x.IsInternalHidden));
				_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.MedicalHistory));
			}
			//consent
			if(IsInternalPicker){
				_listEFormDefs.AddRange(listEFormDefsInternal.FindAll(x=>x.FormType==EnumEFormType.Consent));
			}
			else {
				_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.Consent && !x.IsInternalHidden));
				_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.Consent));
			}
			listMain.Items.Clear();
			listMain.Items.AddList(_listEFormDefs,x => x.Description);
		}

		private void listMain_DoubleClick(object sender,MouseButtonEventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			EFormDefSelected=listMain.GetSelected<EFormDef>();
			//If this is an internal EFormDef, all the fields will already be attached,
			if(!EFormDefSelected.IsInternal){
				EFormDefSelected.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==EFormDefSelected.EFormDefNum);
			}
			IsDialogOK=true;
		}

		private void butBlank_Click(object sender,EventArgs e) {
			//only visible when IsCustomPicker
			EFormDefSelected=new EFormDef();
			EFormDefSelected.ListEFormFieldDefs=new List<EFormFieldDef>();
			EFormDefSelected.ShowLabelsBold=true;
			EFormDefSelected.FormType=EnumEFormType.PatientForm;
			EFormDefSelected.Description="Form";
			EFormDefSelected.MaxWidth=450;
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
			//If this is an internal EFormDef, all the fields will already be attached,
			if(!EFormDefSelected.IsInternal){
				EFormDefSelected.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==EFormDefSelected.EFormDefNum);
			}
			IsDialogOK=true;
		}
	}
}