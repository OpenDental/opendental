using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CodeBase;
using OpenDental.Drawing;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEFormFillEdit : FrmODBase {
		#region Fields
		///<summary>This is the object we are editing.</summary>
		public EForm EFormCur;
		private bool _isLoaded;
		///<summary>Used to keep track of what masked SSN was shown when the form was loaded, and stop us from storing masked SSNs on accident.</summary>
		private string _maskedSSNOld;
		#endregion Fields

		#region Constructor
		///<summary></summary>
		public FrmEFormFillEdit() {
			InitializeComponent();
			Load+=FrmEFormFillEdit_Load;
			SizeChanged+=FrmEFormFillEdit_SizeChanged;
			FormClosing+=FrmEFormFillEdit_FormClosing;
		}
		#endregion Constructor

		#region Methods - event handlers
		private void FrmEFormFillEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			ctrlEFormFill.ListEFormFields=EFormCur.ListEFormFields;//two references to same list of objects
			ctrlEFormFill.RefreshLayout();
			_maskedSSNOld=EFormCur.ListEFormFields.Find(x=>x.DbLink=="SSN")?.ValueString;//null is ok
			textDescription.Text=EFormCur.Description;
			textDateTime.Text=EFormCur.DateTimeShown.ToShortDateString()+" "+EFormCur.DateTimeShown.ToShortTimeString();
			bool isSigned=false;
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++) {
				if(EFormCur.ListEFormFields[i].FieldType.In(EnumEFormFieldType.SigBox)
					&& EFormCur.ListEFormFields[i].ValueString.Length>1) 
				{
					isSigned=true;
					break;
				}
			}
			if(isSigned) {
				ctrlEFormFill.IsReadOnly=true;
			}
			else{
				butUnlock.Visible=false;
			}
			_isLoaded=true;
			SetCtrlWidth();
		}

		private void FrmEFormFillEdit_SizeChanged(object sender,System.Windows.SizeChangedEventArgs e) {
			SetCtrlWidth();
		}

		private void SetCtrlWidth(){
			if(!_isLoaded){
				return;
			}
			int maxWidth=EFormCur.MaxWidth;//no validation of range needed here
			int avail=(int)ActualWidth-(int)ctrlEFormFill.Margin.Left-117;
			if(maxWidth>avail){
				maxWidth=avail;
			}
			ctrlEFormFill.Width=maxWidth;
			ctrlEFormFill.UpdateLayout();
			ctrlEFormFill.FillFieldsFromControls();
			ctrlEFormFill.RefreshLayout();
			//The one thing this doesn't do perfectly is
			//if already signed in this session, then size changed, then change text,
			//signature disappears when it shouldn't. 
			//I can live with that. User can just sign again. Why would they be resizing that much anyway?
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

		//private void butSaveImport_Click(object sender, EventArgs e) {
		//	if(!ValidateAndSave()){
		//		return;
		//	}
		//	EFormCur.ListEFormFields=ListEFormFields;
		//	EFormImport.Import(EFormCur);
		//	IsDialogOK=true;
		//}

		private void butSaveOnly_Click(object sender,EventArgs e) {
			if(!ValidateAndSave()){
				return;
			}
			IsDialogOK=true;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			Printout printout=new Printout();
			printout.FuncPrintPage=ctrlEFormFill.Pd_PrintPage;
			printout.thicknessMarginInches=new Thickness(0.5);
			ctrlEFormFill.PagesPrinted=0;
			WpfControls.PrinterL.TryPrintOrDebugClassicPreview(printout);
			ctrlEFormFill.RefreshLayout();//todo: have this run while the preview window is still open?
		}

		private void butUnlock_Click(object sender,EventArgs e) {
			//this button will not be needed in the eClipboard
			ctrlEFormFill.IsReadOnly=false;
			butUnlock.Visible=false;
		}

		private void FrmEFormFillEdit_FormClosing(object sender,CancelEventArgs e) {
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++){
				if(EFormCur.ListEFormFields[i].FieldType!=EnumEFormFieldType.SigBox){
					continue;
				}
				System.Windows.Controls.Grid gridForField=EFormCur.ListEFormFields[i].TagOD as System.Windows.Controls.Grid;
				System.Windows.Controls.StackPanel stackPanel=gridForField.Children[1] as System.Windows.Controls.StackPanel;
				SignatureBoxWrapper signatureBoxWrapper=stackPanel.Children[1] as SignatureBoxWrapper;
				signatureBoxWrapper?.SetTabletState(0);
			}
		}
		#endregion Methods - event handlers

		#region Methods - private
//todo: This will be called each time we change any field
		private void ClearSigs(){
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++){
				if(EFormCur.ListEFormFields[i].FieldType!=EnumEFormFieldType.SigBox){
					continue;
				}
				//todo:
				//When any field values change, the user is purposefully "invalidating" the old signature. They can resign.
				//But maybe only clear it if it was signed when this form was opened.
				//If it was unsigned when opened, they should be able to sign in any order.
				//Or if they signed it again since opening, that should count too.
				//So maybe the boolean should be if it was signed this session.
				//Use event, and use FrmCommItem as an example.
				//sigBox.ClearSignature(clearTopazTablet:false);
			}
		}

		///<summary>If an error won't allow, then it shows a MsgBox and then returns false.</summary>
		private bool TryToSaveData() {
			DateTime dateTimeShown=DateTime.MinValue;
			try {
				dateTimeShown=DateTime.Parse(textDateTime.Text);
			}
			catch(Exception) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			EFormCur.Description=textDescription.Text;
			EFormCur.DateTimeShown=dateTimeShown;
			EFormCur.DateTEdited=DateTime.Now;//Will get overwritten on insert, but used for update.  Fill even if user did not make changes.
			if(EFormCur.IsNew){
				EFormCur.DateTimeShown=DateTime.Now;
				EForms.Insert(EFormCur);
			}
			else{
				EForms.Update(EFormCur);
			}
			//Delete all of the fieldDefs that are on the form from the database and re-insert them.
			EFormFields.DeleteForForm(EFormCur.EFormNum);
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++) {
				EFormCur.ListEFormFields[i].EFormNum=EFormCur.EFormNum;
				EFormCur.ListEFormFields[i].ItemOrder=i;
				EFormFields.Insert(EFormCur.ListEFormFields[i]);//ignores any existing PK when inserting
			}
			return true;
		}

		private bool ValidateAndSave() {
			ctrlEFormFill.FillFieldsFromControls();
			//EFormCur.ListEFormFields=ListEFormFields;
			EFormValidation eFormValidation=EForms.Validate(EFormCur,_maskedSSNOld);
			if(eFormValidation.ErrorMsg!="") {
				MsgBox.Show(eFormValidation.ErrorMsg);
				ctrlEFormFill.SetVisibilities(eFormValidation.PageNum);
				return false;
			}
			if(!TryToSaveData()) {
				return false;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,EFormCur.PatNum,"EForm: "+EFormCur.Description+" from "+EFormCur.DateTimeShown.ToShortDateString());
			return true;
		}
		#endregion Methods - private
	}
}