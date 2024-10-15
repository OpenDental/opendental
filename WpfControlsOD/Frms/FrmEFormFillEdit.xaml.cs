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
		///<summary>When this form opens or after any save to db, this list gets reset as a copy of the fields. This allows us to compare to see if anything changed before saving.</summary>
		private List<EFormField> _listEFormFieldsOld;
		//these three fields are the result for TryToSaveData().
		private bool _wasError;
		private bool _wasUnchanged;
		private bool _wasSaved;
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
			_listEFormFieldsOld=EFormFields.GetDeepCopy(EFormCur.ListEFormFields);
			Lang.F(this);
			ctrlEFormFill.ListEFormFields=EFormCur.ListEFormFields;//two references to same list of objects
			ctrlEFormFill.ShowLabelsBold=EFormCur.ShowLabelsBold;
			ctrlEFormFill.SpaceBelowEachField=EFormCur.SpaceBelowEachField;
			ctrlEFormFill.SpaceToRightEachField=EFormCur.SpaceToRightEachField;
			ctrlEFormFill.RefreshLayout();
			_maskedSSNOld=EFormCur.ListEFormFields.Find(x=>x.DbLink=="SSN")?.ValueString;//null is ok
			textDescription.Text=EFormCur.Description;
			textDateTime.Text=EFormCur.DateTimeShown.ToShortDateString()+" "+EFormCur.DateTimeShown.ToShortTimeString();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,isShort:true);
			comboImageCat.Items.AddDefNone();
			comboImageCat.Items.AddDefs(listDefs);
			comboImageCat.SetSelectedDefNum(EFormCur.SaveImageCategory);
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
				butUnlockSig.Visible=false;
			}
			_isLoaded=true;
			SetCtrlWidth();
		}

		private void FrmEFormFillEdit_SizeChanged(object sender,System.Windows.SizeChangedEventArgs e) {
			SetCtrlWidth();
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
			EForms.Delete(EFormCur.EFormNum,EFormCur.PatNum);
			//There is no need to send any signal to calling form that user deleted.
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

		private void butPrint_Click(object sender,EventArgs e) {
			//Unlike sheets, we have no need to save first because we are just printing whatever is on the screen at the moment.
			ctrlEFormFill.FillFieldsFromControls();//so that RefreshLayout will work
			Printout printout=new Printout();
			printout.FuncPrintPage=ctrlEFormFill.Pd_PrintPage;
			printout.thicknessMarginInches=new Thickness(0.5);
			ctrlEFormFill.PagesPrinted=0;
			WpfControls.PrinterL.TryPrintOrDebugClassicPreview(printout);
			ctrlEFormFill.RefreshLayout();//todo: have this run while the preview window is still open?
		}

		private void butUnlockSig_Click(object sender,EventArgs e) {
			ctrlEFormFill.IsReadOnly=false;
			butUnlockSig.Visible=false;
		}

		private void butChangePat_Click(object sender,EventArgs e) {
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
//todo: EnumPermType.EformEdit
			//SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,SheetCur.PatNum,Lan.g(this,"Sheet with ID")+" "+SheetCur.SheetNum+" "+Lan.g(this,"moved to PatNum")+" "+frmPatientSelect.PatNumSelected);
			//SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,frmPatientSelect.PatNumSelected,Lan.g(this,"Sheet with ID")+" "+SheetCur.SheetNum+" "+Lan.g(this,"moved from PatNum")+" "+SheetCur.PatNum);
			EFormCur.PatNum=frmPatientSelect.PatNumSelected;
		}

		private void butEClipboard_Click(object sender,EventArgs e) {
			EFormCur.Status=EnumEFormStatus.ShowInEClipboard;
			TryToSaveData();
			if(_wasError){
				return;
			}
			MobileNotifications.CI_AddEForm(EFormCur.PatNum,EFormCur.EFormNum);//tells eClipboard to pull the eForm
//todo EnumPermType.EformEdit
			//SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,EFormCur.PatNum,EFormCur.Description+" from "+EFormCur.DateTimeShown.ToShortDateString());
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			TryToSaveData();
			if(_wasError){
				return;
			}
			IsDialogOK=true;
		}

		private void FrmEFormFillEdit_FormClosing(object sender,CancelEventArgs e) {
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++){
				if(EFormCur.ListEFormFields[i].FieldType!=EnumEFormFieldType.SigBox){
					continue;
				}
				System.Windows.Controls.Border borderBox=EFormCur.ListEFormFields[i].TagOD as System.Windows.Controls.Border;
				System.Windows.Controls.StackPanel stackPanel=borderBox.Child as System.Windows.Controls.StackPanel;
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

		private void SetCtrlWidth(){
			//This only sets control width, not width of form.
			//We let it grow as much as possible, limited by max width and by space available.
			if(!_isLoaded){
				return;
			}
			int maxWidth=EFormCur.MaxWidth;//no validation of range needed here
			maxWidth+=17+2;//scrollbar plus border width
			int avail=(int)ActualWidth-(int)ctrlEFormFill.Margin.Left-147;//147 is our chosen right margin
			if(maxWidth>avail){
				maxWidth=avail;
			}
			if(maxWidth<50){
				maxWidth=50;
				//if window gets extremely narrow, this control might spill out to the right
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

		///<summary>3 variables track the result.</summary>
		private void TryToSaveData() {
			_wasError=false;
			_wasUnchanged=false;
			_wasSaved=false;
			ctrlEFormFill.FillFieldsFromControls();
			EFormValidation eFormValidation = EForms.Validate(EFormCur,_maskedSSNOld);//this line enforces required fields.
			if(eFormValidation.ErrorMsg!="") {
				ctrlEFormFill.SetVisibilities(eFormValidation.PageNum);
				MsgBox.Show(eFormValidation.ErrorMsg);
				_wasError=true;
				return;
			}
			DateTime dateTimeShown=DateTime.MinValue;
			try {
				dateTimeShown=DateTime.Parse(textDateTime.Text);
			}
			catch(Exception) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				_wasError=true;
				return;
			}
			long saveImageCategory=comboImageCat.GetSelectedDefNum();
			//End of validation.
			//Test to see if any changes were made.
			bool isChanged=false;
			if(EFormCur.IsNew){
				isChanged=true;
			}
			if(EFormCur.Description!=textDescription.Text
				|| EFormCur.DateTimeShown!=dateTimeShown
				|| EFormCur.SaveImageCategory!=saveImageCategory)
			{
				isChanged=true;
			}
			isChanged|=EFormFields.IsAnyChanged(EFormCur.ListEFormFields,_listEFormFieldsOld);
			if(!isChanged){
				_wasUnchanged=true;
				return;
			}
			_listEFormFieldsOld=EFormFields.GetDeepCopy(EFormCur.ListEFormFields);
			//from here down we will actually save
			_wasSaved=true;
			EFormCur.Description=textDescription.Text;
			EFormCur.SaveImageCategory=saveImageCategory;
			if(EFormCur.IsNew){
				EFormCur.DateTimeShown=DateTime.Now;//instead of what user might have typed in
				//DateTEdited gets set automatically for insert
				EForms.Insert(EFormCur);
				EFormCur.IsNew=false;//because we frequently stay in this form
				for(int i=0;i<EFormCur.ListEFormFields.Count;i++) {
					EFormCur.ListEFormFields[i].EFormNum=EFormCur.EFormNum;
					EFormCur.ListEFormFields[i].ItemOrder=i;
					EFormFields.Insert(EFormCur.ListEFormFields[i]);
				}
			}
			else{
				EFormCur.DateTimeShown=dateTimeShown;
				EFormCur.DateTEdited=DateTime.Now;
				EForms.Update(EFormCur);
			}
			//Synching here will be very easy compared to other places because user can't delete, add, or reorder. It's all just simple edits.
			//Could do this in a number of different ways.
			//Decided in this case that the simplest approach is to just
			//delete all of the fields that are on the form from the database and re-insert them.
			EFormFields.DeleteForForm(EFormCur.EFormNum);
			for(int i=0;i<EFormCur.ListEFormFields.Count;i++) {
				EFormCur.ListEFormFields[i].EFormNum=EFormCur.EFormNum;
				EFormCur.ListEFormFields[i].ItemOrder=i;
				EFormFields.Insert(EFormCur.ListEFormFields[i]);//ignores any existing PK when inserting
			}
//todo EnumPermType.EformEdit
			//SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,EFormCur.PatNum,"EForm: "+EFormCur.Description+" from "+EFormCur.DateTimeShown.ToShortDateString());
		}
		#endregion Methods - private

		
	}
}