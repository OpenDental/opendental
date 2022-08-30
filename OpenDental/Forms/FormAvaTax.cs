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
			for(int i=0;i<USlocales.ListAll.Count;i++) {
				if(AvaTax.GetListTaxableStates().Contains(USlocales.ListAll[i].PostalAbbr)) {
					listBoxTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
			}
		}

		///<summary>Fill the rest of the UI with the current bridge settings.</summary>
		private void FillSettings() {
			//Set Enabled
			checkEnabled.Checked=Programs.IsEnabled(ProgramName.AvaTax);
			//Set radio buttons
			if(AvaTax.IsProduction()) {
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
			for(int i=0;i<Enum.GetValues(typeof(LogLevel)).Length;i++) {
				listBoxLogLevel.Items.Add(((LogLevel)i).ToString(),(LogLevel)i);
				if((LogLevel)i==AvaTax.GetLogLevelDetail()) {
					listBoxLogLevel.SelectedItem=((LogLevel)i).ToString();
				}
			}
			//Set company code and sales tax def
			textCompanyCode.Text=AvaTax.GetCompanyCode();
			_defCurrentSalesTaxAdjType=Defs.GetDef(DefCat.AdjTypes,AvaTax.GetSalesTaxAdjType());
			textAdjType.Text=_defCurrentSalesTaxAdjType.ItemName;
			_defCurrentSalesTaxReturnAdjType=Defs.GetDef(DefCat.AdjTypes,AvaTax.GetSalesTaxReturnAdjType())??new Def();
			textReturnAdjType.Text=_defCurrentSalesTaxReturnAdjType.ItemName;
			_patFieldDefCurrentTaxExempt=AvaTax.GetTaxExemptPatField();
			textTaxExempt.Text=(_patFieldDefCurrentTaxExempt!=null) ? _patFieldDefCurrentTaxExempt.FieldName : "";
			validTaxLockDate.Text=AvaTax.GetTaxLockDate().ToShortDateString();
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
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.AdjTypes);
			if(formDefinitionPicker.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_defCurrentSalesTaxAdjType=formDefinitionPicker.ListDefsSelected[0];
			textAdjType.Text=_defCurrentSalesTaxAdjType.ItemName;
		}

		///<summary>Pop a def picker to allow the user to select which def they wish to use for sales tax returns</summary>
		private void butChooseRetAdjType_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.AdjTypes);
			if(formDefinitionPicker.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_defCurrentSalesTaxReturnAdjType=formDefinitionPicker.ListDefsSelected[0];
			textReturnAdjType.Text=_defCurrentSalesTaxReturnAdjType.ItemName;
		}

		///<summary>Pop a def picker to allow the user to select which def they wish to use for tax exempt customers</summary>
		private void butChooseTaxExempt_Click(object sender,EventArgs e) {
			using FormPatFieldDefs formPatFieldDefs=new FormPatFieldDefs(true);
			formPatFieldDefs.ShowDialog();
			if(formPatFieldDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			_patFieldDefCurrentTaxExempt=formPatFieldDefs.PatFieldDefSelected;
			textTaxExempt.Text=_patFieldDefCurrentTaxExempt.FieldName;
		}
		
		///<summary>Adds a state to the list of states we do tax.</summary>
		private void butRight_Click(object sender,EventArgs e) {
			List<string> listStringsStatesToMove=listBoxNonTaxedStates.GetListSelected<string>();
			List<string> listStringsStatesInTaxed=listBoxTaxedStates.Items.GetAll<string>();
			listBoxTaxedStates.Items.Clear();
			listBoxNonTaxedStates.Items.Clear();
			for(int i=0;i<USlocales.ListAll.Count;i++) {
				if(listStringsStatesToMove.Contains(USlocales.ListAll[i].PostalAbbr)) {
					listBoxTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
					continue;
				}
				if(listStringsStatesInTaxed.Contains(USlocales.ListAll[i].PostalAbbr)) {
					listBoxTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
			}
		}

		///<summary>Removes a state from the list of states we do tax</summary>
		private void butLeft_Click(object sender,EventArgs e) {
			List<string> listStringsStatesToMove=listBoxTaxedStates.GetListSelected<string>();
			List<string> listStringsStatesInTaxed=listBoxTaxedStates.Items.GetAll<string>();
			listBoxTaxedStates.Items.Clear();
			listBoxNonTaxedStates.Items.Clear();
			for(int i=0;i<USlocales.ListAll.Count;i++) {
				if(listStringsStatesToMove.Contains(USlocales.ListAll[i].PostalAbbr)) {
					listBoxNonTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
					continue;
				}
				if(listStringsStatesInTaxed.Contains(USlocales.ListAll[i].PostalAbbr)) {
					listBoxTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
				else {
					listBoxNonTaxedStates.Items.Add(USlocales.ListAll[i].Name,USlocales.ListAll[i].PostalAbbr);
				}
			}
		}

		///<summary>Only save program properties on close</summary>
		private void butOK_Click(object sender,EventArgs e) {
			if(!validTaxLockDate.IsValid()) {
				MsgBox.Show(this,"Enter a valid tax lock date");
				return;
			}
			long programNum=ProgramCur.ProgramNum;
			ProgramCur.Enabled=checkEnabled.Checked;
			Programs.Update(ProgramCur);
			ProgramProperties.SetProperty(programNum,"Test (T) or Production (P)",radioProdEnv.Checked?"P":"T");
			ProgramProperties.SetProperty(programNum,ProgramProperties.PropertyDescs.Username,textUsername.Text);
			ProgramProperties.SetProperty(programNum,ProgramProperties.PropertyDescs.Password,CDT.Class1.TryEncrypt(textPassword.Text));
			ProgramProperties.SetProperty(programNum,"Company Code",textCompanyCode.Text);
			ProgramProperties.SetProperty(programNum,"Sales Tax Adjustment Type",POut.Long(_defCurrentSalesTaxAdjType.DefNum));
			ProgramProperties.SetProperty(programNum,"Sales Tax Return Adjustment Type",POut.Long(_defCurrentSalesTaxReturnAdjType.DefNum));
			ProgramProperties.SetProperty(programNum,"Taxable States",string.Join(",",listBoxTaxedStates.Items.GetAll<string>()));
			ProgramProperties.SetProperty(programNum,"Log Level",POut.Int((int)listBoxLogLevel.GetSelected<LogLevel>()));
			ProgramProperties.SetProperty(programNum,"Prepay Proc Codes",POut.String(textPrePayCodes.Text));
			ProgramProperties.SetProperty(programNum,"Discount Proc Codes",POut.String(textDiscountCodes.Text));
			ProgramProperties.SetProperty(programNum,"Tax Exempt Pat Field Def",_patFieldDefCurrentTaxExempt==null ? "0" : POut.Long(_patFieldDefCurrentTaxExempt.PatFieldDefNum));
			ProgramProperties.SetProperty(programNum,"Tax Code Overrides",POut.String(textOverrides.Text));
			ProgramProperties.SetProperty(programNum,"Tax Lock Date",POut.String(validTaxLockDate.Text));
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}