using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlOrtho:UserControl {

		#region Fields - Private
		///<summary>Set to the OrthoAutoProcCodeNum Pref value on load.  Can be changed by the user via this form.</summary>
		private long _orthoAutoProcCodeNum;
		///<summary>Filled upon load.</summary>
		private List<long> _listOrthoPlacementCodeNums= new List<long>();
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlOrtho() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butOrthoDisplayFields_Click(object sender,EventArgs e) {
			using FormDisplayFieldsOrthoChart formDisplayFieldsOrthoChart = new FormDisplayFieldsOrthoChart();
			formDisplayFieldsOrthoChart.ShowDialog();
		}

		private void butOrthoHardwareSpecs_Click(object sender,EventArgs e) {
			using FormOrthoHardwareSpecs formOrthoHardwareSpecs=new FormOrthoHardwareSpecs();
			formOrthoHardwareSpecs.ShowDialog();
		}

		private void butOrthoPrescriptions_Click(object sender, EventArgs e){
			using FormOrthoRxSetup formOrthoRxSetup=new FormOrthoRxSetup();
			formOrthoRxSetup.ShowDialog();
		}

		private void butPickOrthoProc_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes = new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult == DialogResult.OK) {
				_orthoAutoProcCodeNum=formProcCodes.CodeNumSelected;
				textOrthoAutoProc.Text=ProcedureCodes.GetStringProcCode(_orthoAutoProcCodeNum);
			}
		}

		private void butPlacementProcsEdit_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes = new FormProcCodes();
			formProcCodes.IsSelectionMode = true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult == DialogResult.OK) {
				_listOrthoPlacementCodeNums.Add(formProcCodes.CodeNumSelected);
			}
			RefreshListBoxProcs();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(listboxOrthoPlacementProcs.SelectedIndices.Count == 0) {
				MsgBox.Show(this,"Select an item to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete the selected items?")) {
				return;
			}
			List<ProcedureCode> listProcedureCodes=listboxOrthoPlacementProcs.GetListSelected<ProcedureCode>();
			for(int i=0;i<listProcedureCodes.Count;i++) {
				_listOrthoPlacementCodeNums.Remove(listProcedureCodes[i].CodeNum);
			}
			RefreshListBoxProcs();
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private void RefreshListBoxProcs() {
			listboxOrthoPlacementProcs.Items.Clear();
			for(int i=0;i<_listOrthoPlacementCodeNums.Count;i++) {
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_listOrthoPlacementCodeNums[i]);
				listboxOrthoPlacementProcs.Items.Add(procedureCode.ProcCode,procedureCode);
			}
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillOrtho() {
			checkOrthoShowInChart.Checked=PrefC.GetBool(PrefName.OrthoShowInChart);
			checkOrthoEnabled.Checked=PrefC.GetBool(PrefName.OrthoEnabled);
			checkOrthoFinancialInfoInChart.Checked=PrefC.GetBool(PrefName.OrthoCaseInfoInOrthoChart);
			checkOrthoClaimMarkAsOrtho.Checked=PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho);
			checkOrthoClaimUseDatePlacement.Checked=PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement);
			checkDebondOverridesMonthsTreat.Checked=PrefC.GetBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat);
			textOrthoMonthsTreat.Text=PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat).ToString();
			checkApptModuleShowOrthoChartItem.Checked=PrefC.GetBool(PrefName.ApptModuleShowOrthoChartItem);
			checkPatClone.Checked=PrefC.GetBool(PrefName.ShowFeaturePatientClone);
			_orthoAutoProcCodeNum=PrefC.GetLong(PrefName.OrthoAutoProcCodeNum);
			textOrthoAutoProc.Text=ProcedureCodes.GetStringProcCode(_orthoAutoProcCodeNum);
			checkConsolidateInsPayment.Checked=PrefC.GetBool(PrefName.OrthoInsPayConsolidated);
			string strListOrthoNums = PrefC.GetString(PrefName.OrthoPlacementProcsList);
			if(strListOrthoNums!="") {
				_listOrthoPlacementCodeNums.AddRange(strListOrthoNums.Split(new char[] { ',' }).ToList().Select(x => PIn.Long(x)));
			}
			RefreshListBoxProcs();
			textBandingCodes.Text=PrefC.GetString(PrefName.OrthoBandingCodes);
			textVisitCodes.Text=PrefC.GetString(PrefName.OrthoVisitCodes);
			textDebondCodes.Text=PrefC.GetString(PrefName.OrthoDebondCodes);
			checkOrthoChartLoggingOn.Checked=PrefC.GetBool(PrefName.OrthoChartLoggingOn);
		}

		public bool SaveOrtho() {
			if(!textOrthoMonthsTreat.IsValid()) {
				MsgBox.Show(this,"Default months treatment must be between 0 and 255 months.");
				return false;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)!=checkPatClone.Checked) {
				MsgBox.Show(this,"You will need to restart OpenDental for this change to take effect.");
			}
			Changed|=Prefs.UpdateBool(PrefName.OrthoShowInChart,checkOrthoShowInChart.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OrthoEnabled,checkOrthoEnabled.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OrthoCaseInfoInOrthoChart,checkOrthoFinancialInfoInChart.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OrthoClaimMarkAsOrtho,checkOrthoClaimMarkAsOrtho.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OrthoClaimUseDatePlacement,checkOrthoClaimUseDatePlacement.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat,checkDebondOverridesMonthsTreat.Checked);
			Changed|=Prefs.UpdateByte(PrefName.OrthoDefaultMonthsTreat,PIn.Byte(textOrthoMonthsTreat.Text));
			Changed|=Prefs.UpdateBool(PrefName.ApptModuleShowOrthoChartItem,checkApptModuleShowOrthoChartItem.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowFeaturePatientClone,checkPatClone.Checked);
			Changed|=Prefs.UpdateLong(PrefName.OrthoAutoProcCodeNum,_orthoAutoProcCodeNum);
			Changed|=Prefs.UpdateBool(PrefName.OrthoInsPayConsolidated,checkConsolidateInsPayment.Checked);
			Changed|=Prefs.UpdateString(PrefName.OrthoPlacementProcsList,string.Join(",",_listOrthoPlacementCodeNums));
			Changed|=Prefs.UpdateString(PrefName.OrthoBandingCodes,PIn.String(textBandingCodes.Text));
			Changed|=Prefs.UpdateString(PrefName.OrthoVisitCodes,PIn.String(textVisitCodes.Text));
			Changed|=Prefs.UpdateString(PrefName.OrthoDebondCodes,PIn.String(textDebondCodes.Text));
			Changed|=Prefs.UpdateBool(PrefName.OrthoChartLoggingOn,checkOrthoChartLoggingOn.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
