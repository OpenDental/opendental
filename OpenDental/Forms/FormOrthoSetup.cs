using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormOrthoSetup:FormODBase {
		private bool _hasChanges;
		///<summary>Set to the OrthoAutoProcCodeNum Pref value on load.  Can be changed by the user via this form.</summary>
		private long _orthoAutoProcCodeNum;
		///<summary>Filled upon load.</summary>
		private List<long> _listOrthoPlacementCodeNums= new List<long>();

		public FormOrthoSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Font=LayoutManagerForms.FontInitial;
			Lan.F(this);
		}

		private void FormOrthoSetup_Load(object sender,EventArgs e) {
			checkOrthoShowInChart.Checked=PrefC.GetBool(PrefName.OrthoShowInChart);
			checkPatClone.Checked=PrefC.GetBool(PrefName.ShowFeaturePatientClone);
			checkApptModuleShowOrthoChartItem.Checked=PrefC.GetBool(PrefName.ApptModuleShowOrthoChartItem);
			checkOrthoEnabled.Checked=PrefC.GetBool(PrefName.OrthoEnabled);
			checkOrthoFinancialInfoInChart.Checked=PrefC.GetBool(PrefName.OrthoCaseInfoInOrthoChart);
			checkOrthoClaimMarkAsOrtho.Checked=PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho);
			checkOrthoClaimUseDatePlacement.Checked=PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement);
			textOrthoMonthsTreat.Text=PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat).ToString();
			_orthoAutoProcCodeNum=PrefC.GetLong(PrefName.OrthoAutoProcCodeNum);
			textOrthoAutoProc.Text=ProcedureCodes.GetStringProcCode(_orthoAutoProcCodeNum);
			checkConsolidateInsPayment.Checked=PrefC.GetBool(PrefName.OrthoInsPayConsolidated);
			checkDebondOverridesMonthsTreat.Checked=PrefC.GetBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat);
			checkOrthoChartLoggingOn.Checked=PrefC.GetBool(PrefName.OrthoChartLoggingOn);
			string strListOrthoNums = PrefC.GetString(PrefName.OrthoPlacementProcsList);
			if(strListOrthoNums!="") {
				_listOrthoPlacementCodeNums.AddRange(strListOrthoNums.Split(new char[] { ',' }).ToList().Select(x => PIn.Long(x)));
			}
			textBandingCodes.Text=PrefC.GetString(PrefName.OrthoBandingCodes);
			textVisitCodes.Text=PrefC.GetString(PrefName.OrthoVisitCodes);
			textDebondCodes.Text=PrefC.GetString(PrefName.OrthoDebondCodes);
			RefreshListBoxProcs();
		}

		private void RefreshListBoxProcs() {
			listboxOrthoPlacementProcs.Items.Clear();
			for(int i=0;i<_listOrthoPlacementCodeNums.Count;i++) {
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_listOrthoPlacementCodeNums[i]);
				listboxOrthoPlacementProcs.Items.Add(procedureCode.ProcCode,procedureCode);
			}
		}

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

		private void butSave_Click(object sender,EventArgs e) {
			if(!textOrthoMonthsTreat.IsValid()) {
				MsgBox.Show(this,"Default months treatment must be between 0 and 255 months.");
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)!=checkPatClone.Checked) {
				MsgBox.Show(this,"You will need to restart OpenDental for this change to take effect.");
			}
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoShowInChart,checkOrthoShowInChart.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.ShowFeaturePatientClone,checkPatClone.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.ApptModuleShowOrthoChartItem,checkApptModuleShowOrthoChartItem.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoEnabled,checkOrthoEnabled.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoCaseInfoInOrthoChart,checkOrthoFinancialInfoInChart.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoClaimMarkAsOrtho,checkOrthoClaimMarkAsOrtho.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoClaimUseDatePlacement,checkOrthoClaimUseDatePlacement.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoChartLoggingOn,checkOrthoChartLoggingOn.Checked);
			_hasChanges|=Prefs.UpdateByte(PrefName.OrthoDefaultMonthsTreat,PIn.Byte(textOrthoMonthsTreat.Text));
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoInsPayConsolidated,checkConsolidateInsPayment.Checked);
			_hasChanges|=Prefs.UpdateBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat,checkDebondOverridesMonthsTreat.Checked);
			_hasChanges|=Prefs.UpdateLong(PrefName.OrthoAutoProcCodeNum,_orthoAutoProcCodeNum);
			_hasChanges|=Prefs.UpdateString(PrefName.OrthoPlacementProcsList,string.Join(",",_listOrthoPlacementCodeNums));
			_hasChanges|=Prefs.UpdateString(PrefName.OrthoBandingCodes,PIn.String(textBandingCodes.Text));
			_hasChanges|=Prefs.UpdateString(PrefName.OrthoVisitCodes,PIn.String(textVisitCodes.Text));
			_hasChanges|=Prefs.UpdateString(PrefName.OrthoDebondCodes,PIn.String(textDebondCodes.Text));
			DialogResult=DialogResult.OK;
		}

		private void FormOrthoSetup_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel) {
				return;
			}
			if(_hasChanges) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

	}
}