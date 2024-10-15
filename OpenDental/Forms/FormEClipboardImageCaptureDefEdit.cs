using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using static OpenDentBusiness.SheetFieldsAvailable;

namespace OpenDental {
	public partial class FormEClipboardImageCaptureDefEdit : FormODBase {
		///<summary>The eClipboardImageCaptureDef we are currently editing </summary>
		public EClipboardImageCaptureDef EClipboardImageCaptureDefCur;
		///<summary>List of all eClipboardImageCaptureDefs. Only needed to check if the selected EnumOCRCaptureType in listOCRCaptureType is already in use by a different eClipboardImageCaptureDef.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefs;
		///<summary>Gets set to true if an eClipboardImageCaptureDef has been marked for deletion. Deletion occurs in parent form.</summary>
		public bool IsDeleted=false;

		public FormEClipboardImageCaptureDefEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClipboardImageCaptureDefEdit_Load(object sender,EventArgs e) {
			textFrequency.Text=EClipboardImageCaptureDefCur.FrequencyDays.ToString();
			if(EClipboardImageCaptureDefCur.IsSelfPortrait){
				textImage.Text="Self Portrait";
				textValue.Text="Allows patient to submit a self-portrait upon checkin";
			}
			else{
				textImage.Text=Defs.GetName(DefCat.EClipboardImageCapture,EClipboardImageCaptureDefCur.DefNum);
				textValue.Text=Defs.GetValue(DefCat.EClipboardImageCapture,EClipboardImageCaptureDefCur.DefNum);
			}
			listOCRCaptureType.Items.AddEnums<EnumOcrCaptureType>();
			listOCRCaptureType.SetSelectedEnum(EClipboardImageCaptureDefCur.OcrCaptureType);
		}

		private void butSave_Click(object sender,EventArgs e) {
			//text box is an int greater than or equal to zero.
			if(!textFrequency.IsValid() || textFrequency.Value<0) {
				MsgBox.Show(this, "Frequency (days) must be a valid whole number, 0 or greater.");
				return;
			}
			int freq=textFrequency.Value;
			//make sure selected enum from listOCRCaptureType isn't already in use by a different eClipboardImageCaptureDef for this clinic.
			EnumOcrCaptureType enumOcrCaptureTypeSelected=listOCRCaptureType.GetSelected<EnumOcrCaptureType>();
			if(enumOcrCaptureTypeSelected!=EnumOcrCaptureType.Miscellaneous){//If not a misc image, do this check.
				List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefsForClinic=ListEClipboardImageCaptureDefs.FindAll(x=>
					x.ClinicNum==EClipboardImageCaptureDefCur.ClinicNum 
					&& x.OcrCaptureType==enumOcrCaptureTypeSelected
					&& x.DefNum!=EClipboardImageCaptureDefCur.DefNum);
				if(listEClipboardImageCaptureDefsForClinic.Count>0){
					//there is already an image capture def for this scanner type at this clinic.
					MsgBox.Show(Lan.g(this, "There is already an eClipboard Image with the Image Capture Type")+" "+enumOcrCaptureTypeSelected.GetDescription()+" "+Lan.g(this,"at this clinic."));
					return;
				}
			}
			EClipboardImageCaptureDefCur.OcrCaptureType=enumOcrCaptureTypeSelected;
			EClipboardImageCaptureDefCur.FrequencyDays=freq;
			DialogResult=DialogResult.OK;
			//No need to update yet. That happens way later on FormEServicesEClipboard.
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}

	}
}