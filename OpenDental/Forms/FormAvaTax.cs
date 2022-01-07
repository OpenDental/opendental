using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAvaTax:FormODBase {

		private Def _defCurrentSalesTaxAdjType;
		private Def _defCurrentSalesTaxReturnAdjType;
		private PatFieldDef _patFieldDefCurrentTaxExempt;
		public Program ProgramCur;

		public FormAvaTax() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAvaTax_Load(object sender,EventArgs e) {
			FillSettings();
			FillListStates();
		}

		///<summary>Fill both listboxes with the states that are taxed and non-taxed based on the current bridge settings.</summary>
		private void FillListStates() {
			foreach(USlocale locale in USlocales.ListAll) {
				if(AvaTax.ListTaxableStates.Contains(locale.PostalAbbr)) {
					listBoxTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
			}
		}

		///<summary>Fill the rest of the UI with the current bridge settings.</summary>
		private void FillSettings() {
			//Set Enabled
			checkEnabled.Checked=Programs.IsEnabled(ProgramName.AvaTax);
			//Set radio buttons
			if(AvaTax.IsProduction) {
				radioProdEnv.Checked=true;
			}
			else {
				radioTestEnv.Checked=true;
			}
			//Set username and password
			textUsername.Text=ProgramProperties.GetPropVal(ProgramName.AvaTax,ProgramProperties.PropertyDescs.Username);
			textPassword.Text=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(ProgramName.AvaTax,ProgramProperties.PropertyDescs.Password));
			//Fill Log Level options
			listBoxLogLevel.Items.Clear();
			foreach(LogLevel lv in Enum.GetValues(typeof(LogLevel))) {
				listBoxLogLevel.Items.Add(lv.ToString(),lv);
				if(lv==AvaTax.LogDetailLevel) {
					listBoxLogLevel.SelectedItem=lv.ToString();
				}
			}
			//Set company code and sales tax def
			textCompanyCode.Text=AvaTax.CompanyCode;
			_defCurrentSalesTaxAdjType=Defs.GetDef(DefCat.AdjTypes,AvaTax.SalesTaxAdjType);
			textAdjType.Text=_defCurrentSalesTaxAdjType.ItemName;
			_defCurrentSalesTaxReturnAdjType=Defs.GetDef(DefCat.AdjTypes,AvaTax.SalesTaxReturnAdjType)??new Def();
			textReturnAdjType.Text=_defCurrentSalesTaxReturnAdjType.ItemName;
			_patFieldDefCurrentTaxExempt=AvaTax.TaxExemptPatField;
			textTaxExempt.Text=(_patFieldDefCurrentTaxExempt!=null) ? _patFieldDefCurrentTaxExempt.FieldName : "";
			validTaxLockDate.Text=AvaTax.TaxLockDate.ToShortDateString();
			//Set list of procCodes
			textPrePayCodes.Text=ProgramProperties.GetPropVal(ProgramName.AvaTax,"Prepay Proc Codes");
			textDiscountCodes.Text=ProgramProperties.GetPropVal(ProgramName.AvaTax,"Discount Proc Codes");
			//Set the list of overrides
			textOverrides.Text=ProgramProperties.GetPropVal(ProgramName.AvaTax,"Tax Code Overrides");
		}

		private void butPing_Click(object sender,EventArgs e) {
			if(AvaTax.IsApiAvailable(radioProdEnv.Checked,textUsername.Text,textPassword.Text)) {
				MsgBox.Show(this,"Success connecting to API.");
			}
			else {
				MsgBox.Show(this,"Failure connecting to API.  Check that username and password are correct.");
			}
		}

		///<summary>Pop a def picker to allow the user to select which def they wish to use for sales tax</summary>
		private void butChooseTaxAdjType_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDP=new FormDefinitionPicker(DefCat.AdjTypes);
			if(formDP.ShowDialog()==DialogResult.OK) {
				_defCurrentSalesTaxAdjType=formDP.ListDefsSelected[0];
				textAdjType.Text=_defCurrentSalesTaxAdjType.ItemName;
			}
		}

		///<summary>Pop a def picker to allow the user to select which def they wish to use for sales tax returns</summary>
		private void butChooseRetAdjType_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDP=new FormDefinitionPicker(DefCat.AdjTypes);
			if(formDP.ShowDialog()==DialogResult.OK) {
				_defCurrentSalesTaxReturnAdjType=formDP.ListDefsSelected[0];
				textReturnAdjType.Text=_defCurrentSalesTaxReturnAdjType.ItemName;
			}
		}

		///<summary>Pop a def picker to allow the user to select which def they wish to use for tax exempt customers</summary>
		private void butChooseTaxExempt_Click(object sender,EventArgs e) {
			using FormPatFieldDefs formPFD=new FormPatFieldDefs(true);
			formPFD.ShowDialog();
			if(formPFD.DialogResult==DialogResult.OK) {
				_patFieldDefCurrentTaxExempt=formPFD.SelectedPatFieldDef;
				textTaxExempt.Text=_patFieldDefCurrentTaxExempt.FieldName;
			}
		}
		
		///<summary>Adds a state to the list of states we do tax.</summary>
		private void butRight_Click(object sender,EventArgs e) {
			List<string> listStatesToMove=listBoxNonTaxedStates.GetListSelected<string>();
			List<string> listStatesInTaxed=listBoxTaxedStates.Items.GetAll<string>();
			listBoxTaxedStates.Items.Clear();
			listBoxNonTaxedStates.Items.Clear();
			foreach(USlocale locale in USlocales.ListAll) {
				if(listStatesToMove.Contains(locale.PostalAbbr)) {
					listBoxTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
					continue;
				}
				if(listStatesInTaxed.Contains(locale.PostalAbbr)) {
					listBoxTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
			}
		}

		///<summary>Removes a state from the list of states we do tax</summary>
		private void butLeft_Click(object sender,EventArgs e) {
			List<string> listStatesToMove=listBoxTaxedStates.GetListSelected<string>();
			List<string> listStatesInTaxed=listBoxTaxedStates.Items.GetAll<string>();
			listBoxTaxedStates.Items.Clear();
			listBoxNonTaxedStates.Items.Clear();
			foreach(USlocale locale in USlocales.ListAll) {
				if(listStatesToMove.Contains(locale.PostalAbbr)) {
					listBoxNonTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
					continue;
				}
				if(listStatesInTaxed.Contains(locale.PostalAbbr)) {
					listBoxTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(locale.Name,locale.PostalAbbr);
				}
			}
		}

		///<summary>Only save program properties on close</summary>
		private void butOK_Click(object sender,EventArgs e) {
			if(!validTaxLockDate.IsValid()) {
				MsgBox.Show(this,"Enter a valid tax lock date");
				return;
			}
			long progNum=ProgramCur.ProgramNum;
			ProgramCur.Enabled=checkEnabled.Checked;
			Programs.Update(ProgramCur);
			ProgramProperties.SetProperty(progNum,"Test (T) or Production (P)",radioProdEnv.Checked?"P":"T");
			ProgramProperties.SetProperty(progNum,ProgramProperties.PropertyDescs.Username,textUsername.Text);
			ProgramProperties.SetProperty(progNum,ProgramProperties.PropertyDescs.Password,CDT.Class1.TryEncrypt(textPassword.Text));
			ProgramProperties.SetProperty(progNum,"Company Code",textCompanyCode.Text);
			ProgramProperties.SetProperty(progNum,"Sales Tax Adjustment Type",POut.Long(_defCurrentSalesTaxAdjType.DefNum));
			ProgramProperties.SetProperty(progNum,"Sales Tax Return Adjustment Type",POut.Long(_defCurrentSalesTaxReturnAdjType.DefNum));
			ProgramProperties.SetProperty(progNum,"Taxable States",string.Join(",",listBoxTaxedStates.Items.GetAll<string>()));
			ProgramProperties.SetProperty(progNum,"Log Level",POut.Int((int)listBoxLogLevel.GetSelected<LogLevel>()));
			ProgramProperties.SetProperty(progNum,"Prepay Proc Codes",POut.String(textPrePayCodes.Text));
			ProgramProperties.SetProperty(progNum,"Discount Proc Codes",POut.String(textDiscountCodes.Text));
			ProgramProperties.SetProperty(progNum,"Tax Exempt Pat Field Def",_patFieldDefCurrentTaxExempt==null ? "0" : POut.Long(_patFieldDefCurrentTaxExempt.PatFieldDefNum));
			ProgramProperties.SetProperty(progNum,"Tax Code Overrides",POut.String(textOverrides.Text));
			ProgramProperties.SetProperty(progNum,"Tax Lock Date",POut.String(validTaxLockDate.Text));
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}