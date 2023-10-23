using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;


namespace OpenDental {
	public partial class UserControlAccountAdjustments:UserControl {
		
		#region Fields - Private
		private List<Def> _listDefsPosAdjTypes;
		private List<Def> _listDefsNegAdjTypes;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountAdjustments() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butBadDebt_Click(object sender,EventArgs e) {
			string[] arrayDefNums=PrefC.GetString(PrefName.BadDebtAdjustmentTypes).Split(new char[] {','}); //comma-delimited list.
			List<long> listBadAdjDefNums = new List<long>();
			foreach(string strDefNum in arrayDefNums) {
				listBadAdjDefNums.Add(PIn.Long(strDefNum));
			}
			List<Def> listBadAdjDefs=Defs.GetDefs(DefCat.AdjTypes,listBadAdjDefNums);
			using FormDefinitionPicker FormDP = new FormDefinitionPicker(DefCat.AdjTypes,listBadAdjDefs);
			FormDP.HasShowHiddenOption=true;
			FormDP.IsMultiSelectionMode=true;
			FormDP.ShowDialog();
			if(FormDP.DialogResult==DialogResult.OK) {
				FillListboxBadDebt(FormDP.ListDefsSelected);
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private void FillListboxBadDebt(List<Def> listSelectedDefs) {
			listboxBadDebtAdjs.Items.Clear();
			foreach(Def defCur in listSelectedDefs) {
				listboxBadDebtAdjs.Items.Add(defCur.ItemName,defCur);
			}
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountAdjustments() {
			_listDefsPosAdjTypes=Defs.GetPositiveAdjTypes();
			_listDefsNegAdjTypes=Defs.GetNegativeAdjTypes();
			comboPayPlanAdj.Items.AddDefs(_listDefsNegAdjTypes);
			comboPayPlanAdj.SetSelectedDefNum(PrefC.GetLong(PrefName.PayPlanAdjType));
			comboFinanceChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboFinanceChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.FinanceChargeAdjustmentType));
			comboBillingChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboBillingChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.BillingChargeAdjustmentType));
			comboLateChargeAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboLateChargeAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.LateChargeAdjustmentType));
			comboSalesTaxAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboSalesTaxAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.SalesTaxAdjustmentType));
			//hide the UI for this feature until it is fully implemented.
			comboRefundAdjustmentType.Visible=false;
			labelRefundAdjustmentType.Visible=false;
			textTaxPercent.Text=PrefC.GetDouble(PrefName.SalesTaxPercentage).ToString();
			string[] arrayDefNums=PrefC.GetString(PrefName.BadDebtAdjustmentTypes).Split(new char[] { ',' }); //comma-delimited list.
			List<long> listBadAdjDefNums=new List<long>();
			foreach(string strDefNum in arrayDefNums) {
				listBadAdjDefNums.Add(PIn.Long(strDefNum));
			}
			FillListboxBadDebt(Defs.GetDefs(DefCat.AdjTypes,listBadAdjDefNums));
			//Fill the combobox with providers
			comboSalesTaxDefaultProvider.Items.AddProvNone("Default");
			comboSalesTaxDefaultProvider.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboSalesTaxDefaultProvider.SetSelectedProvNum(PrefC.GetLong(PrefName.SalesTaxDefaultProvider));
			checkAutomateSalesTax.Checked=PrefC.GetBool(PrefName.SalesTaxDoAutomate);
			// Fill negative adjustment combo with Enum preference values and set selected option
			comboNegativeAdjustments.Items.AddEnums<EnumAdjustmentBlockOrWarn>();
			comboNegativeAdjustments.SetSelected(PrefC.GetInt(PrefName.AdjustmentBlockNegativeExceedingPatPortion));
		}

		public bool SaveAccountAdjustments() {
			double taxPercent=0;
			if(!double.TryParse(textTaxPercent.Text,out taxPercent)) {
				MsgBox.Show(this,"Sales Tax percent is invalid.  Please enter a valid number to continue.");
				return false;
			}
			if(taxPercent<0) {
				MsgBox.Show(this,"Sales Tax percent cannot be a negative number.");
				return false;
			}
			string strListBadDebtAdjTypes=string.Join(",",listboxBadDebtAdjs.Items.GetAll<Def>().Select(x => x.DefNum));
			Changed|=Prefs.UpdateDouble(PrefName.SalesTaxPercentage,taxPercent,false);//Do not round this double for Hawaii
			Changed|=Prefs.UpdateString(PrefName.BadDebtAdjustmentTypes,strListBadDebtAdjTypes);
			Changed|=Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,comboSalesTaxDefaultProvider.GetSelectedProvNum());
			Changed|=Prefs.UpdateBool(PrefName.SalesTaxDoAutomate,checkAutomateSalesTax.Checked);
			if(comboFinanceChargeAdjType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.FinanceChargeAdjustmentType,comboFinanceChargeAdjType.GetSelectedDefNum());
			}
			if(comboBillingChargeAdjType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.BillingChargeAdjustmentType,comboBillingChargeAdjType.GetSelectedDefNum());
			}
			if(comboLateChargeAdjType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.LateChargeAdjustmentType,comboLateChargeAdjType.GetSelectedDefNum());
			}
			if(comboSalesTaxAdjType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.SalesTaxAdjustmentType,comboSalesTaxAdjType.GetSelectedDefNum());
			}
			if(comboPayPlanAdj.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.PayPlanAdjType,comboPayPlanAdj.GetSelectedDefNum());
			}
			if(comboNegativeAdjustments.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateInt(PrefName.AdjustmentBlockNegativeExceedingPatPortion,(int)comboNegativeAdjustments.GetSelected<EnumAdjustmentBlockOrWarn>());
			}
			return true;
		}
		#endregion Methods - Public
	}
}
