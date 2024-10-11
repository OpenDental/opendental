using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using static OpenDentBusiness.SheetFieldsAvailable;

namespace OpenDental {
	public partial class FormEClipboardImageCaptureDefEdit : FormODBase {
		/// <summary> The imageCaptureDef we are currently editing </summary>
		private EClipboardImageCaptureDef _eClipboardImageCaptureDef;
		/// <summary> List of all imageCaptureDefs </summary>
		private List<EClipboardImageCaptureDef> _listEClipboardImageCaptureDefs;
		/// <summary></summary>
		public bool IsDeleted=false;

		public FormEClipboardImageCaptureDefEdit(EClipboardImageCaptureDef eClipboardImageCaptureDef, List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eClipboardImageCaptureDef=eClipboardImageCaptureDef;
			_listEClipboardImageCaptureDefs=listEClipboardImageCaptureDefs;
		}

		private void FormEClipboardImageCaptureDefEdit_Load(object sender,EventArgs e) {
			comboOcrCaptureType.Items.AddEnums<EnumOcrCaptureType>();
			comboOcrCaptureType.SetSelectedEnum(_eClipboardImageCaptureDef.OcrCaptureType);
			textFrequency.Text=_eClipboardImageCaptureDef.FrequencyDays.ToString();
			if(_eClipboardImageCaptureDef.IsSelfPortrait){
				textImage.Text="Self Portrait";
				textValue.Text="Allows patient to submit a self-portrait upon checkin";
			}
			else{
				textImage.Text=Defs.GetName(DefCat.EClipboardImageCapture,_eClipboardImageCaptureDef.DefNum);
				textValue.Text=Defs.GetValue(DefCat.EClipboardImageCapture,_eClipboardImageCaptureDef.DefNum);
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//text box is an int greater than or equal to zero.
			if(!textFrequency.IsValid() || textFrequency.Value<0) {
				MsgBox.Show(this, "Frequency (days) must be a valid whole number, 0 or greater.");
				return;
			}
			int freq=textFrequency.Value;
			//combo box isnt a duplicate ins scanner type for this clinic.
			//If not a misc image, do this check.
			EnumOcrCaptureType enumOcrCaptureTypeSelected=comboOcrCaptureType.GetSelected<EnumOcrCaptureType>();
			if(enumOcrCaptureTypeSelected!=EnumOcrCaptureType.Miscellaneous) {
				List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefsForClinic=_listEClipboardImageCaptureDefs.FindAll(x=>
					x.ClinicNum==_eClipboardImageCaptureDef.ClinicNum 
					&& x.OcrCaptureType==enumOcrCaptureTypeSelected
					&& x.DefNum!=_eClipboardImageCaptureDef.DefNum);
				if(listEClipboardImageCaptureDefsForClinic.Count>0) {
					//there is already an image capture def for this scanner type at this clinic.
					MsgBox.Show(Lan.g(this, "There is already an eClipboard Image with the Image Capture Type")+" "+enumOcrCaptureTypeSelected.GetDescription()+" "+Lan.g(this,"at this clinic."));
					return;
				}
			}
			_eClipboardImageCaptureDef.OcrCaptureType=enumOcrCaptureTypeSelected;
			_eClipboardImageCaptureDef.FrequencyDays=freq;
			DialogResult=DialogResult.OK;
			//No need to update yet. That happens way later on FormEServicesEClipboard.
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}

	}
}