using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEFormFillEdit : FrmODBase {
		#region Fields
		///<summary>This is the object we are editing.</summary>
		public EForm EFormCur;
		///<summary>All the fields for this eForm. We could have instead used the list attached to the EForm, but using a separate list like this as soon as we are able matches all our existing patterns better. If we are comming from Patient Forms and clicked "Add EForm", this list is referenced from there and EFormCur.ListEFormFields will have the same list. If we double click on an exisiting eForm, this list will be fields from the database.</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.</summary>
		private string _maskedSSNOld;
		#endregion Fields

		#region Constructor
		///<summary></summary>
		public FrmEFormFillEdit() {
			InitializeComponent();
			Load+=FrmEFormFillEdit_Load;
		}
		#endregion Constructor

		#region Methods - event handlers
		private void FrmEFormFillEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			ctrlEFormFill.ListEFormFields=ListEFormFields;
			ctrlEFormFill.RefreshLayout();
			_maskedSSNOld=ListEFormFields.Find(x=>x.DbLink=="SSN")?.ValueString;//null is ok
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire form?")){
				return;
			}
			if(EFormCur.IsNew){
				IsDialogCancel=true;
				return;
			}
			EFormFields.DeleteForForm(EFormCur.EFormNum);
			EForms.Delete(EFormCur.EFormNum);
			IsDialogOK=true;
		}

		private void butSaveImport_Click(object sender, EventArgs e) {
			if(!ValidateAndSave()){
				return;
			}
			EFormCur.ListEFormFields=ListEFormFields;
			EFormImport.Import(EFormCur);
			IsDialogOK=true;
		}

		private void butSaveOnly_Click(object sender,EventArgs e) {
			if(!ValidateAndSave()){
				return;
			}
			IsDialogOK=true;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			
		}
		#endregion Methods - event handlers

		#region Methods - private
		///<summary>If an error won't allow, then it shows a MsgBox and then returns false.</summary>
		private bool TryToSaveData() {
			if(EFormCur.IsNew){
				EFormCur.DateTimeShown=DateTime.Now;
				EForms.Insert(EFormCur);
			}
			else{
				EForms.Update(EFormCur);
			}
			//Delete all of the fieldDefs that are on the form from the database and re-insert them.
			EFormFields.DeleteForForm(EFormCur.EFormNum);
			for(int i=0;i<ListEFormFields.Count;i++) {
				ListEFormFields[i].EFormNum=EFormCur.EFormNum;
				ListEFormFields[i].ItemOrder=i;
				EFormFields.Insert(ListEFormFields[i]);//ignores any existing PK when inserting
			}
			return true;
		}

		private bool ValidateAndSave() {
			ctrlEFormFill.FillFieldsFromControls();
			EFormCur.ListEFormFields=ListEFormFields;
			EFormValidation eFormValidation=EForms.Validate(EFormCur,_maskedSSNOld);
			if(eFormValidation.ErrorMsg!="") {
				MsgBox.Show(eFormValidation.ErrorMsg);
				ctrlEFormFill.SetVisibilities(eFormValidation.PageNum);
				return false;
			}
			if(!TryToSaveData()) {//This method never returns false.
				return false;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,EFormCur.PatNum,"EForm: "+EFormCur.Description+" from "+EFormCur.DateTimeShown.ToShortDateString());
			return true;
		}

		#endregion Methods - private


	}
}