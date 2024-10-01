using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPrinterEdit:FormODBase {
		private PrintSituation _printSituation;

		public FormPrinterEdit(Printer printer) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_printSituation=printer.PrintSit;
			textFileExtension.Text=printer.FileExtension;
			checkPrompt.Checked=printer.DisplayPrompt;
			checkVirtualPrinter.Checked=printer.IsVirtualPrinter;
			textSituation.Text=_printSituation.GetDescription();
			FillComboPrinter(printer.PrinterName);
		}

		private void FillComboPrinter(string printerName) {
			PrinterSettings.StringCollection installedPrinters=null;
			try {
				installedPrinters=PrinterSettings.InstalledPrinters;
			}
			catch(Exception ex) {//do not let the window open if printers cannot be accessed
				FriendlyException.Show(Lan.g(this,"Unable to access installed printers."),ex);
				DialogResult=DialogResult.Cancel;
				return;
			}
			comboPrinter.Items.Clear();
			if(_printSituation==PrintSituation.Default){
				comboPrinter.Items.Add(Lan.g(this,"Windows default"));
			}
			else{
				comboPrinter.Items.Add(Lan.g(this,"default"));
			}
			for(int i=0;i<installedPrinters.Count;i++){
				comboPrinter.Items.Add(installedPrinters[i]);
				if(printerName==installedPrinters[i]){
					comboPrinter.SelectedIndex=i+1;
				}
			}
			if(comboPrinter.SelectedIndex==-1){
				comboPrinter.SelectedIndex=0;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			string compName=ODEnvironment.MachineName;
			string printerName="";
			bool isChecked=checkPrompt.Checked;
			//PrintSituation sit=PrintSituation.Default;
			//first: main Default, since not in panel Simple
			if(comboPrinter.SelectedIndex>0){
				printerName=comboPrinter.SelectedItem.ToString();
			}
			Printers.PutForSit(_printSituation,compName,printerName,isChecked,isVirtual:checkVirtualPrinter.Checked,fileExtension:textFileExtension.Text);
			DataValid.SetInvalid(InvalidType.Computers);
			Printers.RefreshCache();//the other computers don't care
			DialogResult=DialogResult.OK;
		}
	}
}