using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;

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
			for(int i=0;i<c.Controls.Count;i++) {
				SetControls(c.Controls[i],installedPrinters);
				if(c.Controls[i]==checkSimple){
					continue;
				}
				if(c.Controls[i].GetType()==typeof(UI.ComboBox)){
					FillCombo((UI.ComboBox)c.Controls[i],installedPrinters);
				}
				if(c.Controls[i].GetType()==typeof(UI.CheckBox)){
					FillCheck((UI.CheckBox)c.Controls[i]);
				}
			}
		}

		private void FillCombo(UI.ComboBox comboBox,PrinterSettings.StringCollection installedPrinters) {
			PrintSituation printerSituation=GetSit(comboBox);
			Printer printerForSit=Printers.GetForSit(printerSituation);
			string printerName="";
			if(printerForSit!=null){
				printerName=printerForSit.PrinterName;
			}
			comboBox.Items.Clear();
			if(comboBox==comboDefault){
				comboBox.Items.Add(Lan.g(this,"Windows default"));
			}
			else{
				comboBox.Items.Add(Lan.g(this,"default"));
			}
			for(int i=0;i<installedPrinters.Count;i++){
				comboBox.Items.Add(installedPrinters[i]);
				if(printerName==installedPrinters[i]){
					comboBox.SelectedIndex=i+1;
				}
			}
			if(comboBox.SelectedIndex==-1){
				comboBox.SelectedIndex=0;
			}
		}

		private void FillCheck(UI.CheckBox checkBox){
			PrintSituation printSituationSit=GetSit(checkBox);
			Printer printerForSit=Printers.GetForSit(printSituationSit);
			if(printerForSit==null){
				checkBox.Checked=false;
				return;
			}
			checkBox.Checked=printerForSit.DisplayPrompt;
		}

		private PrintSituation GetSit(Control contr){
			PrintSituation printSituation=PrintSituation.Default;
			switch(contr.Name){
				default:
					MessageBox.Show("error. "+contr.Name);
					break;
				case "comboDefault":
				case "checkDefault":
					printSituation=PrintSituation.Default;
					break;
				case "comboAppointments":
				case "checkAppointments":
					printSituation=PrintSituation.Appointments;
					break;
				case "comboClaim":
				case "checkClaim":
					printSituation=PrintSituation.Claim;
					break;
				case "comboLabelSheet":
				case "checkLabelSheet":
					printSituation=PrintSituation.LabelSheet;
					break;
				case "comboLabelSingle":
				case "checkLabelSingle":
					printSituation=PrintSituation.LabelSingle;
					break;
				case "comboPostcard":
				case "checkPostcard":
					printSituation=PrintSituation.Postcard;
					break;
				case "comboRx":
				case "checkRx":
					printSituation=PrintSituation.Rx;
					break;
				case "comboRxControlled":
				case "checkRxControlled":
					printSituation=PrintSituation.RxControlled;
					break;
				case "comboRxMulti":
				case "checkRxMulti":
					printSituation=PrintSituation.RxMulti;
					break;
				case "comboStatement":
				case "checkStatement":
					printSituation=PrintSituation.Statement;
					break;
				case "comboTPPerio":
				case "checkTPPerio":
					printSituation=PrintSituation.TPPerio;
					break;
				case "comboReceipt":
				case "checkReceipt":
					printSituation=PrintSituation.Receipt;
					break;
			}
			return printSituation;
		}

		///<summary>Sets the simple hide based on the status of the checkbox</summary>
		private void SetSimple(){
			panelSimple.Visible=!checkSimple.Checked;
		}

		private void checkSimple_Click(object sender, System.EventArgs e) {
			SetSimple();
		}

		private void butOK_Click(object sender, System.EventArgs e){
			string compName=ODEnvironment.MachineName;
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
				for(int j=0;j<panelSimple.Controls.Count;j++){
					if(panelSimple.Controls[j].GetType()!=typeof(UI.ComboBox)//skip anything but comboBoxes and CheckBoxes
						&& panelSimple.Controls[j].GetType()!=typeof(UI.CheckBox))
					{
						continue;
					}
					//so only two controls out of all will be used in each Enum loop
					if(GetSit(panelSimple.Controls[j])!=(PrintSituation)i){
						continue;
					}
					if(panelSimple.Controls[j].GetType()==typeof(UI.ComboBox)){
						if(((UI.ComboBox)panelSimple.Controls[j]).SelectedIndex==0){
							printerName="";
						}
						else{
							printerName=((UI.ComboBox)panelSimple.Controls[j]).SelectedItem.ToString();
						}
					}
					else{//checkBox
						isChecked=((UI.CheckBox)panelSimple.Controls[j]).Checked;
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
