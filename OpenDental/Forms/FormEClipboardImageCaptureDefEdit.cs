using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using static OpenDentBusiness.SheetFieldsAvailable;

namespace OpenDental {
	public partial class FormEClipboardImageCaptureDefEdit : FormODBase {
		/// <summary> The imageCaptureDef we are currently editing </summary>
		private EClipboardImageCaptureDef _imageCaptureDef;
		/// <summary> List of all imageCaptureDefs </summary>
		private List<EClipboardImageCaptureDef> _listEClipboardImageCaptureDef;
		/// <summary></summary>
		public bool IsDeleted=false;

		public FormEClipboardImageCaptureDefEdit(EClipboardImageCaptureDef eClipboardImageCaptureDef, List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_imageCaptureDef= eClipboardImageCaptureDef;
			_listEClipboardImageCaptureDef=listEClipboardImageCaptureDefs;
		}

		private void FormEClipboardImageCaptureDefEdit_Load(object sender,EventArgs e) {
			comboImageCaptureType.Items.AddEnums<EnumOcrCaptureType>();
			comboImageCaptureType.SetSelectedEnum(_imageCaptureDef.OcrCaptureType);
			textFrequency.Text=_imageCaptureDef.FrequencyDays.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//text box is an int greater than or equal to zero.
			if(!textFrequency.IsValid() || textFrequency.Value<0) {
				MsgBox.Show(this, "Frequency (days) must be a valid whole number, 0 or greater.");
				return;
			}
			int freq=textFrequency.Value;
			//combo box isnt a duplicate ins scanner type for this clinic.
			//If not a misc image, do this check.
			EnumOcrCaptureType ocrCaptureTypeSelected=comboImageCaptureType.GetSelected<EnumOcrCaptureType>();
			if(ocrCaptureTypeSelected!=EnumOcrCaptureType.Miscellaneous) {
				List<EClipboardImageCaptureDef> listImageCaptureDefsForClinic=_listEClipboardImageCaptureDef.FindAll(x=>
					x.ClinicNum==_imageCaptureDef.ClinicNum 
					&& x.OcrCaptureType==ocrCaptureTypeSelected
					&& x.DefNum!=_imageCaptureDef.DefNum);
				if(listImageCaptureDefsForClinic.Count>0) {
					//there is already an image capture def for this scanner type at this clinic.
					MsgBox.Show(Lan.g(this, "There is already an eClipboard Image with the Image Capture Type")+" "+ocrCaptureTypeSelected.GetDescription()+" "+Lan.g(this,"at this clinic."));
					return;
				}
			}
			_imageCaptureDef.OcrCaptureType=ocrCaptureTypeSelected;
			_imageCaptureDef.FrequencyDays=freq;
			DialogResult=DialogResult.OK;
			//No need to update yet. That happens way later on FormEServicesEClipboard.
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}

	}
}