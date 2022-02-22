using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCDSILabResult:FormODBase {
		private List<Ucum> _listUcums;
		///<summary>CDSI Trigger formatted text. This is the form result.</summary>
		public string ResultCDSITriggerText;

		public FormCDSILabResult() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabResultEdit_Load(object sender,EventArgs e) {
			fillCombos();
			checkAllResults.Checked=true;

		}

		private void fillCombos() {
			comboComparator.Items.Add("=");
			comboComparator.Items.Add(">=");
			comboComparator.Items.Add(">");
			comboComparator.Items.Add("<");
			comboComparator.Items.Add("<=");
			comboComparator.SelectedIndex=0;//not sure if this code works. Test it.
			_listUcums=Ucums.GetAll();
			if(_listUcums.Count==0) {
				MsgBox.Show(this,"Units of measure have not been imported. Go to the code system importer window to import UCUM codes to continue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			int _tempSelectedIndex=0;
			for(int i=0;i<_listUcums.Count;i++) {
				comboUnits.Items.Add(_listUcums[i].UcumCode);
				if(_listUcums[i].UcumCode=="mg/dL") {//arbitrarily chosen common unit of measure.
					_tempSelectedIndex=i;
				}
			}
			comboUnits.SelectedIndex=_tempSelectedIndex;
		}

		private void butLoinc_Click(object sender,EventArgs e) {
			using FormLoincs formLoincs=new FormLoincs();
			formLoincs.IsSelectionMode=true;
			formLoincs.ShowDialog();
			if(formLoincs.DialogResult!=DialogResult.OK) {
				return;
			}
			textLoinc.Text=formLoincs.SelectedLoinc.LoincCode;
			textLoincDescription.Text=formLoincs.SelectedLoinc.NameLongCommon;
			//if(FormL.SelectedLoinc.UnitsUCUM!="") {
			comboUnits.Text=formLoincs.SelectedLoinc.UnitsUCUM;//may be values that are not available otherwise. There are 270 units in the Loinc table that are not in the UCUM table.
			//}
		}

		private void butSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				return;
			}
			//Clear other options
			checkAllResults.Checked=false;
			textObsValue.Text="";
			//Set Microbiology results
			textSnomed.Text=formSnomeds.SelectedSnomed.SnomedCode;
			textSnomedDescription.Text=formSnomeds.SelectedSnomed.Description;

		}

		private void textObsValue_TextChanged(object sender,EventArgs e) {
			if(textObsValue.Text=="" && textSnomed.Text=="") {//cleared the text
				checkAllResults.Checked=true;//user tried unchecking box but nothing else is selected.
				return;
			}
			checkAllResults.Checked=false;
			textSnomed.Text="";
			textSnomedDescription.Text="";
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOk_Click(object sender,EventArgs e) {
			ResultCDSITriggerText=textLoinc.Text;/* +";";
			if(!checkAllResults.Checked && textObsValue.Text=="" && textSnomed.Text=="") {
				MsgBox.Show(this,"Please select a valid lab result comparison.");
				return;//should never happen.  Somehow they have an invalid comparison set up.
			}
			else if(checkAllResults.Checked && textObsValue.Text=="" && textSnomed.Text=="") {
				ResultCDSITriggerText+=";";//loinc comparison only.
			}
			else if(!checkAllResults.Checked && textObsValue.Text!="" && textSnomed.Text=="") {
				ResultCDSITriggerText+=comboComparator.Text+textObsValue.Text+";"+comboUnits.Text;//Example:  >150;mg/dL
			}
			else if(!checkAllResults.Checked && textObsValue.Text=="" && textSnomed.Text!="") {
				ResultCDSITriggerText+=textSnomed.Text+";";//leave units blank to signify snomed.
			}
			else {
				MsgBox.Show(this,"Please select a valid lab result comparison.");
				return;//should never happen.  Somehow they have an invalid comparison set up.
			}*/
			DialogResult=DialogResult.OK;
		}

		
	}
}
