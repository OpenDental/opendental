using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormActivityLog:FormODBase {
		private List<string> _listTypeDescriptions=new List<string>();
		private List<string> _listActionDescriptions=new List<string>();
		private List<EServiceLog> _logTable=new List<EServiceLog>();
		private Dictionary<eServiceType,string> _dictEserviceTypes=new Dictionary<eServiceType, string>();

		public FormActivityLog() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebSchedLog_Load(object sender,System.EventArgs e) {
			comboBoxClinicMulti.IsAllSelected=true;
			DateTime firstDayOfTheMonth=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			datePicker.SetDateTimeFrom(firstDayOfTheMonth);
			datePicker.SetDateTimeTo(firstDayOfTheMonth.AddMonths(1));
			checkDistinctLogGuid.Checked=false;
			_dictEserviceTypes.Add(eServiceType.WSGeneral, eServiceType.WSGeneral.GetDescription());
			_dictEserviceTypes.Add(eServiceType.WSNewPat,eServiceType.WSNewPat.GetDescription());
			_dictEserviceTypes.Add(eServiceType.WSExistingPat,eServiceType.WSExistingPat.GetDescription());
			_dictEserviceTypes.Add(eServiceType.WSRecall,eServiceType.WSRecall.GetDescription());
			_dictEserviceTypes.Add(eServiceType.WSAsap,eServiceType.WSAsap.GetDescription());
			_dictEserviceTypes.Add(eServiceType.PatientPortal,eServiceType.PatientPortal.GetDescription());
			_dictEserviceTypes.Add(eServiceType.ApptConfirmations,eServiceType.ApptConfirmations.GetDescription());
			_dictEserviceTypes.Add(eServiceType.EClipboard,eServiceType.EClipboard.GetDescription());
			_dictEserviceTypes.Add(eServiceType.WebForms,eServiceType.WebForms.GetDescription());
			for(int i=0;i<_dictEserviceTypes.Count;i++) {
				_listTypeDescriptions.Add(_dictEserviceTypes.ElementAt(i).Value);
			}
			_dictEserviceTypes.Add(eServiceType.Unknown,"All");
			_listTypeDescriptions.Sort();
			_listTypeDescriptions.Insert(0,"All");
			comboBoxTypes.Items.AddList<string>(_listTypeDescriptions, x => x);
			List<eServiceAction> listEserviceActions=EServiceLogs.GetEServiceActions(eServiceType.Unknown); //Unknown will return all
			for(int i=0;i<listEserviceActions.Count;i++) {
				_listActionDescriptions.Add(listEserviceActions[i].GetDescription());
			}
			_listActionDescriptions.Sort();
			_listActionDescriptions.Insert(0,"All");
			comboBoxActions.Items.AddList<string>(_listActionDescriptions,x => x);
			comboBoxTypes.SelectedIndex=0;
			comboBoxActions.SelectedIndex=0;
			comboBoxClinicMulti.SelectedClinicNum=Clinics.ClinicNum;
		}

		private void FillGrid() {
			List<EServiceLog> data=_logTable;
			if(textPatNum.Text!="" && PIn.Long(textPatNum.Text)>-1) {
				data=data.Where(x => x.PatNum.ToString()==textPatNum.Text).ToList();
			}
			if(comboBoxTypes.SelectedIndex!=-1 && _listTypeDescriptions[comboBoxTypes.SelectedIndex]!="All") {
				data=data.Where(x => x.EServiceType.GetDescription()==comboBoxTypes.GetSelected<string>()).ToList();
			}
			if(comboBoxActions.SelectedIndex!=-1 && _listActionDescriptions[comboBoxActions.SelectedIndex]!="All") {
				data=data.Where(x => x.EServiceAction.GetDescription()==comboBoxActions.GetSelected<string>()).ToList();
			}
			if(textLogGuid.Text!="") {
				data=data.Where(x => x.LogGuid.Contains(textLogGuid.Text)).ToList();
			}
			List<EServiceLog> listEserviceLogs=new List<EServiceLog>();
			if(checkDistinctLogGuid.Checked) {
				listEserviceLogs=data.GroupBy(x => x.LogGuid).Select(x => x.OrderByDescending(y => y.LogDateTime).ThenByDescending(y => y.EServiceLogNum).First()).ToList();
			}
			else {
				listEserviceLogs=data;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"eService Type"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"eService Action"),250);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"FKeyType"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"FKey"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Log DateTime"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PatNum"),50);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic Abbr"),100);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Log GUID"),100);
			gridMain.ListGridColumns.Add(col);
			//Rows
			gridMain.ListGridRows.Clear();
			for(int i = 0;i<listEserviceLogs.Count;i++) {
				GridRow newRow=new GridRow();
				newRow.Cells.Add(listEserviceLogs[i].EServiceType.GetDescription());
				newRow.Cells.Add(listEserviceLogs[i].EServiceAction.GetDescription());
				newRow.Cells.Add(listEserviceLogs[i].KeyType.ToString());
				newRow.Cells.Add(listEserviceLogs[i].FKey.ToString());
				newRow.Cells.Add(listEserviceLogs[i].LogDateTime.ToString());
				newRow.Cells.Add(listEserviceLogs[i].PatNum.ToString());
				if(PrefC.HasClinicsEnabled) {
					if(listEserviceLogs[i].ClinicNum==0) {
						newRow.Cells.Add("HQ");
					}
					else {
						newRow.Cells.Add(OpenDentBusiness.Clinics.GetClinic(listEserviceLogs[i].ClinicNum).Abbr);
					}
				}
				newRow.Cells.Add(listEserviceLogs[i].LogGuid.ToString());
				gridMain.ListGridRows.Add(newRow);
			}
			gridMain.EndUpdate();
			labelRows.Text=$"Row Count: {listEserviceLogs.Count}";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_logTable=OpenDentBusiness.EServiceLogs.GetEServiceLog(comboBoxClinicMulti.SelectedClinicNum,datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo());
			FillGrid();
		}

		private void comboBoxActions_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboBoxTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			comboBoxActions.Items.Clear();
			_listActionDescriptions.Clear();
			eServiceType selectedType=_dictEserviceTypes.FirstOrDefault(x => x.Value==comboBoxTypes.GetSelected<string>()).Key;
			List<eServiceAction> listActions=EServiceLogs.GetEServiceActions(selectedType);
			for(int i=0;i<listActions.Count;i++) {
				_listActionDescriptions.Add(listActions[i].GetDescription());
			}
			_listActionDescriptions.Sort();
			_listActionDescriptions.Insert(0,"All");
			comboBoxActions.Items.AddList<string>(_listActionDescriptions,x => x);
			comboBoxActions.SelectedIndex=0;
			FillGrid();
		}

		private void textbox_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkDistinctLogGuid_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
