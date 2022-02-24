using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormPrinterSetup : FormODBase {

		///<summary></summary>
		public FormPrinterSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPrinterSetup_Load(object sender, System.EventArgs e) {
			PrinterSettings.StringCollection installedPrinters=null;
			try {
				installedPrinters=PrinterSettings.InstalledPrinters;
			}
			catch(Exception ex) {//do not let the window open if printers cannot be accessed
				FriendlyException.Show(Lan.g(this,"Unable to access installed printers."),ex);
				DialogResult=DialogResult.Cancel;
				return;
			}
			checkSimple.Checked=PrefC.GetBool(PrefName.EasyHidePrinters);
			SetSimple();
			SetControls(this,installedPrinters);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelTPandPerio.Text=Lan.g(this,"Treatment Plans");
			}
		}

		///<summary>recursive</summary>
		private void SetControls(Control c,PrinterSettings.StringCollection installedPrinters) {
			foreach(Control control in c.Controls){
				SetControls(control,installedPrinters);
				if(control==checkSimple){
					continue;
				}
				if(control.GetType()==typeof(ComboBox)){
					FillCombo((ComboBox)control,installedPrinters);
				}
				if(control.GetType()==typeof(CheckBox)){
					FillCheck((CheckBox)control);
				}
			}
		}

		private void FillCombo(ComboBox combo,PrinterSettings.StringCollection installedPrinters) {
			PrintSituation sit=GetSit(combo);
			Printer printerForSit=Printers.GetForSit(sit);
			string printerName="";
			if(printerForSit!=null){
				printerName=printerForSit.PrinterName;
			}
			combo.Items.Clear();
			if(combo==comboDefault){
				combo.Items.Add(Lan.g(this,"Windows default"));
			}
			else{
				combo.Items.Add(Lan.g(this,"default"));
			}
			for(int i=0;i<installedPrinters.Count;i++){
				combo.Items.Add(installedPrinters[i]);
				if(printerName==installedPrinters[i]){
					combo.SelectedIndex=i+1;
				}
			}
			if(combo.SelectedIndex==-1){
				combo.SelectedIndex=0;
			}
		}

		private void FillCheck(CheckBox check){
			PrintSituation sit=GetSit(check);
			Printer printerForSit=Printers.GetForSit(sit);
			if(printerForSit==null){
				check.Checked=false;
				return;
			}
			check.Checked=printerForSit.DisplayPrompt;
		}

		private PrintSituation GetSit(Control contr){
			PrintSituation sit=PrintSituation.Default;
			switch(contr.Name){
				default:
					MessageBox.Show("error. "+contr.Name);
					break;
				case "comboDefault":
				case "checkDefault":
					sit=PrintSituation.Default;
					break;
				case "comboAppointments":
				case "checkAppointments":
					sit=PrintSituation.Appointments;
					break;
				case "comboClaim":
				case "checkClaim":
					sit=PrintSituation.Claim;
					break;
				case "comboLabelSheet":
				case "checkLabelSheet":
					sit=PrintSituation.LabelSheet;
					break;
				case "comboLabelSingle":
				case "checkLabelSingle":
					sit=PrintSituation.LabelSingle;
					break;
				case "comboPostcard":
				case "checkPostcard":
					sit=PrintSituation.Postcard;
					break;
				case "comboRx":
				case "checkRx":
					sit=PrintSituation.Rx;
					break;
				case "comboRxControlled":
				case "checkRxControlled":
					sit=PrintSituation.RxControlled;
					break;
				case "comboRxMulti":
				case "checkRxMulti":
					sit=PrintSituation.RxMulti;
					break;
				case "comboStatement":
				case "checkStatement":
					sit=PrintSituation.Statement;
					break;
				case "comboTPPerio":
				case "checkTPPerio":
					sit=PrintSituation.TPPerio;
					break;
				case "comboReceipt":
				case "checkReceipt":
					sit=PrintSituation.Receipt;
					break;
			}
			return sit;
		}

		///<summary>Sets the simple hide based on the status of the checkbox</summary>
		private void SetSimple(){
			panelSimple.Visible=!checkSimple.Checked;
		}

		private void checkSimple_Click(object sender, System.EventArgs e) {

			SetSimple();
		}

		private void butOK_Click(object sender, System.EventArgs e){
			string compName=SystemInformation.ComputerName;
			if(checkSimple.Checked && !PrefC.GetBool(PrefName.EasyHidePrinters)){
				//if user clicked the simple option
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Warning!  You have selected the simple interface option."+
					"  This will force all computers to use the simple mode."+
					"  This will also clear all printing preferences for all other computers and set them back to default."+
					"  Are you sure you wish to continue?"))
				{
					return;
				}
				Printers.ClearAll();
				Printers.RefreshCache();
				string printerName="";
				if(comboDefault.SelectedIndex==0){
					printerName="";
				}
				else{
					printerName=comboDefault.SelectedItem.ToString();
				}
				Printers.PutForSit(PrintSituation.Default,compName,printerName,true);
			}
			else for(int i=0;i<Enum.GetValues(typeof(PrintSituation)).Length;i++){
				//loop through each printSituation
				string printerName="";
				bool isChecked=false;
				//PrintSituation sit=PrintSituation.Default;
				//first: main Default, since not in panel Simple
				if(i==0){//printSituation.Default
					if(comboDefault.SelectedIndex==0){
						printerName="";
					}
					else{
						printerName=comboDefault.SelectedItem.ToString();
					}
				}
				foreach(Control control in panelSimple.Controls){
					if(control.GetType()!=typeof(ComboBox)//skip anything but comboBoxes and CheckBoxes
						&& control.GetType()!=typeof(CheckBox))
					{
						continue;
					}
					//so only two controls out of all will be used in each Enum loop
					if(GetSit(control)!=(PrintSituation)i){
						continue;
					}
					if(control.GetType()==typeof(ComboBox)){
						if(((ComboBox)control).SelectedIndex==0){
							printerName="";
						}
						else{
							printerName=((ComboBox)control).SelectedItem.ToString();
						}
					}
					else{//checkBox
						isChecked=((CheckBox)control).Checked;
					}
				}
				Printers.PutForSit((PrintSituation)i,compName,printerName,isChecked);
			}
			DataValid.SetInvalid(InvalidType.Computers);
			if(checkSimple.Checked!=PrefC.GetBool(PrefName.EasyHidePrinters)){
				Prefs.UpdateBool(PrefName.EasyHidePrinters,checkSimple.Checked);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Printers.RefreshCache();//the other computers don't care
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}
