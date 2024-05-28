using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormSheetFieldEditPatImage:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetField SheetFieldCur;
		public Sheet SheetCur;
		///<summary>The Y value to limit placement of PatImage to, should be set by caller.</summary>
		public int BottomYLimit;

		public FormSheetFieldEditPatImage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldPatImage_Load(object sender,EventArgs e) {
			if(BottomYLimit==0) {
				BottomYLimit=SheetCur.Height;
			}
			FillFields();
			textXPos.Text=SheetFieldCur.XPos.ToString();
			textYPos.Text=SheetFieldCur.YPos.ToString();
			textWidth.Text=SheetFieldCur.Width.ToString();
			textHeight.Text=SheetFieldCur.Height.ToString();
		}

		private void FillFields(){
			textFieldValueDoc.Text="";
			textFieldValueMount.Text="";
			if(SheetFieldCur.FieldValue.StartsWith("MountNum:")){
				long mountNum=PIn.Long(SheetFieldCur.FieldValue.Substring(9));
				Mount mount=Mounts.GetByNum(mountNum);
				textFieldValueMount.Text=mount.DateCreated.ToShortDateString()+" "+mount.Description;
			}
			else if(SheetFieldCur.FieldValue!=""){
				long docNum=PIn.Long(SheetFieldCur.FieldValue);
				Document document=Documents.GetByNum(docNum);
				textFieldValueDoc.Text=document.DateCreated.ToShortDateString()+" "+document.Description;
			}
		}

		private void butChange_Click(object sender, EventArgs e){
			//MsgBox.Show(sheetField.FieldValue);
			using FormImagePickerPatient formImagePickerPatient=new FormImagePickerPatient();
			Patient patient=Patients.GetPat(SheetCur.PatNum);
			formImagePickerPatient.PatientCur=patient;
			if(SheetFieldCur.FieldValue.StartsWith("MountNum:")){
				long mountNum=PIn.Long(SheetFieldCur.FieldValue.Substring(9));
				formImagePickerPatient.MountNumSelected=mountNum;
			}
			else if(SheetFieldCur.FieldValue!=""){
				long docNum=PIn.Long(SheetFieldCur.FieldValue);
				formImagePickerPatient.DocNumSelected=docNum;
			}
			formImagePickerPatient.ShowDialog();
			if(formImagePickerPatient.DialogResult!=DialogResult.OK){
				return;
			}
			if(formImagePickerPatient.DocNumSelected>0){
				long docNumSelected=formImagePickerPatient.DocNumSelected;
				SheetFieldCur.FieldValue=docNumSelected.ToString();
				SheetFieldCur.FieldName=Documents.GetByNum(docNumSelected).DocCategory.ToString();//Returns new document if docnum is not found, so no need to check for null
			}
			if(formImagePickerPatient.MountNumSelected>0){
				long mountNumSelected=formImagePickerPatient.MountNumSelected;
				SheetFieldCur.FieldValue="MountNum:"+mountNumSelected.ToString();
				SheetFieldCur.FieldName=Mounts.GetByNum(mountNumSelected).DocCategory.ToString();//Returns new mount if mountnum is not found so no need to check for null
			}
			FillFields();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			SheetFieldCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//The maximum y-value of the sheet field must be within the sheet vertically.
			textYPos.MaxVal=BottomYLimit-PIn.Int(textHeight.Text);
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SheetFieldCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldCur.Width=PIn.Int(textWidth.Text);
			SheetFieldCur.Height=PIn.Int(textHeight.Text);
			//don't save to database here.
			SheetFieldCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}