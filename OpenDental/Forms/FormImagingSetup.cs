using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
///<summary></summary>
	public partial class FormImagingSetup:FormODBase {
		private bool _scanDocSelectSourceOld;
		//private ComputerPref computerPrefs;

		///<summary></summary>
		public FormImagingSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			//too many labels to use Lan.F()
			Lan.C(this, new System.Windows.Forms.Control[]
			{
				this,
				this.groupBox1,
				this.groupBox2,
				this.groupBox3,
				this.label1,
				this.label2,
				this.label3,
				this.label4,
				this.label5,
				this.label6,
				this.label7,
				this.label12,
				this.labelPanoBW,
				this.label37
			});
			Lan.C("All", new System.Windows.Forms.Control[] {
				butOK,
				butCancel,
			});
		}

		private void FormImagingSetup_Load(object sender, System.EventArgs e) {
			comboType.Items.Add("B");
			comboType.Items.Add("D");
			_scanDocSelectSourceOld=ComputerPrefs.LocalComputer.ScanDocSelectSource;
			checkScanDocSelectSource.Checked=_scanDocSelectSourceOld;
			if(ComputerPrefs.LocalComputer.ScanDocShowOptions) {
				radioScanDocShowOptions.Checked=true;
				radioScanDocUseOptionsBelow.Checked=false;
				groupScanningOptions.Enabled=false;
			}
			else {
				radioScanDocShowOptions.Checked=false;
				radioScanDocUseOptionsBelow.Checked=true;
				groupScanningOptions.Enabled=true;
			}
			checkScanDocDuplex.Checked=ComputerPrefs.LocalComputer.ScanDocDuplex;
			checkScanDocGrayscale.Checked=ComputerPrefs.LocalComputer.ScanDocGrayscale;
			textScanDocResolution.Text=ComputerPrefs.LocalComputer.ScanDocResolution.ToString();
			textScanDocQuality.Text=ComputerPrefs.LocalComputer.ScanDocQuality.ToString();
			//textScanDocQuality.Text=PrefC.GetLong(PrefName.ScannerCompression).ToString();
			slider.MinVal=PrefC.GetInt(PrefName.ImageWindowingMin);
			slider.MaxVal=PrefC.GetInt(PrefName.ImageWindowingMax);
			Program programSuni=Programs.GetFirstOrDefault(x => x.ProgDesc=="Suni");
			if(programSuni!=null && programSuni.Enabled){
				int exposureLevelVal=ComputerPrefs.LocalComputer.SensorExposure;
				if(exposureLevelVal<(int)upDownExposure.Minimum || exposureLevelVal>(int)upDownExposure.Maximum){
					exposureLevelVal=(int)upDownExposure.Minimum;//Play it safe with the default exposure.
				}
				upDownExposure.Value=exposureLevelVal;
				upDownPort.Value=ComputerPrefs.LocalComputer.SensorPort;
				comboType.Text=ComputerPrefs.LocalComputer.SensorType;
				checkBinned.Checked=ComputerPrefs.LocalComputer.SensorBinned;
			}
			else{
				groupBoxSuni.Visible=false;
			}
			//checkScanDocShowOptions.Checked=PrefC.GetBool(PrefName.ScannerSuppressDialog);
			//textScanDocResolution.Text=PrefC.GetString(PrefName.ScannerResolution);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelPanoBW.Visible=false;
			}
		}

		private void radioScanDocShowOptions_CheckedChanged(object sender,EventArgs e) {
			if(radioScanDocShowOptions.Checked) {
				groupScanningOptions.Enabled=false;
			}
			else {
				groupScanningOptions.Enabled=true;
			}
		}

		private void butSetScanner_Click(object sender,EventArgs e) {
			try {
				xImageDeviceManager.Obfuscator.ActivateEZTwain();
			}
			catch {
				MsgBox.Show(this,"EzTwain4.dll not found.  Please run the setup file in your images folder.");
				return;
			}
			ImagingDeviceManager.EZTwain.SelectImageSource(this.Handle);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textScanDocQuality.IsValid() || !textScanDocResolution.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(textScanDocQuality.Text=="100"
				|| (radioScanDocUseOptionsBelow.Checked && PIn.Int(textScanDocResolution.Text)>300)) 
			{
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"With the provided settings the file created may be extremely large.  Would you like to continue?")) {
					return;
				}
			}
			ComputerPrefs.LocalComputer.ScanDocSelectSource=checkScanDocSelectSource.Checked;
			ComputerPrefs.LocalComputer.ScanDocShowOptions=radioScanDocShowOptions.Checked;
			ComputerPrefs.LocalComputer.ScanDocDuplex=checkScanDocDuplex.Checked;
			ComputerPrefs.LocalComputer.ScanDocGrayscale=checkScanDocGrayscale.Checked;
			ComputerPrefs.LocalComputer.ScanDocResolution=PIn.Int(textScanDocResolution.Text);
			ComputerPrefs.LocalComputer.ScanDocQuality=PIn.Byte(textScanDocQuality.Text);
			//Prefs.UpdateLong(PrefName.ScannerCompression,PIn.Long(textScanDocQuality.Text));
			Prefs.UpdateLong(PrefName.ImageWindowingMin,slider.MinVal);
			Prefs.UpdateLong(PrefName.ImageWindowingMax,slider.MaxVal);
			//Prefs.UpdateBool(PrefName.ScannerSuppressDialog,checkScanDocShowOptions.Checked);
			//Prefs.UpdateLong(PrefName.ScannerResolution,PIn.Long(textScanDocResolution.Text));
			if(groupBoxSuni.Visible){
				ComputerPrefs.LocalComputer.SensorExposure=(int)upDownExposure.Value;
				ComputerPrefs.LocalComputer.SensorPort=(int)upDownPort.Value;
				ComputerPrefs.LocalComputer.SensorType=comboType.Text;
				ComputerPrefs.LocalComputer.SensorBinned=checkBinned.Checked;
			}
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			DataValid.SetInvalid(InvalidType.Prefs);
			if(_scanDocSelectSourceOld!=checkScanDocSelectSource.Checked) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0
					,Lan.g(this,"Show Select Scanner Window option changed from")+" "
					+(_scanDocSelectSourceOld?Lan.g(this,"true"):Lan.g(this,"false"))+" "
					+Lan.g(this,"to")+" "+(checkScanDocSelectSource.Checked?Lan.g(this,"true"):Lan.g(this,"false")));
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


		
	}
}
