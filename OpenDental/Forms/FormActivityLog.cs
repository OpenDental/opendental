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
		private List<string> _listActionDescriptions;
		private List<EServiceLog> _listEServiceLogs=new List<EServiceLog>();

		public FormActivityLog() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormActivityLog_Load(object sender,System.EventArgs e) {
			comboBoxClinicMulti.IsAllSelected=true;
			DateTime dateFirstDayOfTheMonth=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			datePicker.SetDateTimeFrom(dateFirstDayOfTheMonth);
			datePicker.SetDateTimeTo(dateFirstDayOfTheMonth.AddMonths(1));
			checkDistinctLogGuid.Checked=false;
			//"All" first, then alphabetical
			List<eServiceType> listEserviceTypes=Enum.GetValues(typeof(eServiceType)).AsEnumerable<eServiceType>()
				.OrderByDescending(x => x==eServiceType.Unknown).ThenBy(x => x.GetDescription(useShortVersionIfAvailable:true)).ToList();
			for(int i=0;i<listEserviceTypes.Count;i++) {
				comboBoxTypes.Items.Add(listEserviceTypes[i].GetDescription(useShortVersionIfAvailable:true), listEserviceTypes[i]);
			}
			List<eServiceAction> listEserviceActions=EServiceLogs.GetEServiceActions(eServiceType.Unknown); //Unknown will return all
			_listActionDescriptions=new List<string>();
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
			List<EServiceLog> listEServiceLogs=_listEServiceLogs;
			if(textPatNum.Text!="" && PIn.Long(textPatNum.Text)>-1) {
				listEServiceLogs=listEServiceLogs.Where(x => x.PatNum.ToString()==textPatNum.Text).ToList();
			}
			if(comboBoxTypes.SelectedIndex!=-1 && comboBoxTypes.GetSelected<eServiceType>()!=eServiceType.Unknown) {
				listEServiceLogs=listEServiceLogs.Where(x => x.EServiceType==comboBoxTypes.GetSelected<eServiceType>()).ToList();
			}
			if(comboBoxActions.SelectedIndex!=-1 && _listActionDescriptions[comboBoxActions.SelectedIndex]!="All") {
				listEServiceLogs=listEServiceLogs.Where(x => x.EServiceAction.GetDescription()==comboBoxActions.GetSelected<string>()).ToList();
			}
			if(textLogGuid.Text!="") {
				listEServiceLogs=listEServiceLogs.Where(x => x.LogGuid.Contains(textLogGuid.Text)).ToList();
			}
			if(checkDistinctLogGuid.Checked) {
				listEServiceLogs=listEServiceLogs.GroupBy(x => x.LogGuid).Select(x => x.OrderByDescending(y => y.LogDateTime).ThenByDescending(y => y.EServiceLogNum).First()).ToList();
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
			for(int i = 0;i<listEServiceLogs.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listEServiceLogs[i].EServiceType.GetDescription());
				row.Cells.Add(listEServiceLogs[i].EServiceAction.GetDescription());
				row.Cells.Add(listEServiceLogs[i].KeyType.ToString());
				row.Cells.Add(listEServiceLogs[i].FKey.ToString());
				row.Cells.Add(listEServiceLogs[i].LogDateTime.ToString());
				row.Cells.Add(listEServiceLogs[i].PatNum.ToString());
				if(PrefC.HasClinicsEnabled) {
					if(listEServiceLogs[i].ClinicNum==0) {
						row.Cells.Add("HQ");
					}
					else {
						row.Cells.Add(OpenDentBusiness.Clinics.GetClinic(listEServiceLogs[i].ClinicNum).Abbr);
					}
				}
				row.Cells.Add(listEServiceLogs[i].LogGuid.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			labelRows.Text=$"Row Count: {listEServiceLogs.Count}";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_listEServiceLogs=OpenDentBusiness.EServiceLogs.GetEServiceLog(comboBoxClinicMulti.SelectedClinicNum,datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo());
			FillGrid();
		}
		
		private void comboBoxActions_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}
		
		private void comboBoxTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			comboBoxActions.Items.Clear();
			_listActionDescriptions=new List<string>();
			eServiceType eServiceTypeSelected=comboBoxTypes.GetSelected<eServiceType>();
			List<eServiceAction> listEserviceActions=EServiceLogs.GetEServiceActions(eServiceTypeSelected);
			for(int i=0;i<listEserviceActions.Count;i++) {
				_listActionDescriptions.Add(listEserviceActions[i].GetDescription());
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
