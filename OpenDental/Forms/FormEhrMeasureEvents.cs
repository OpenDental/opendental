using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrMeasureEvents:FormODBase {
		private List<string> _typeNames;
		private List<EhrMeasureEvent> _listEhrMeasureEvents;

		public FormEhrMeasureEvents() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrMeasureEvents_Load(object sender,System.EventArgs e) {
			comboType.Items.Add(Lan.g(this,"All"));
			comboType.SelectedIndex=0;
			_typeNames=new List<string>(Enum.GetNames(typeof(EhrMeasureEventType)));
			for(int i=0;i<_typeNames.Count;i++) {
				comboType.Items.Add(Lan.g(this,_typeNames[i]));
			}
			textDateStart.Text=new DateTime(DateTime.Today.Year,1,1).ToShortDateString();
			textDateEnd.Text=new DateTime(DateTime.Today.Year,12,31).ToShortDateString();
			FillGrid();
		}

		private void FillGrid() {
			if(comboType.SelectedIndex==0) {
				_listEhrMeasureEvents=EhrMeasureEvents.GetAllByTypeFromDB(PIn.DateT(textDateStart.Text),PIn.DateT(textDateEnd.Text),(EhrMeasureEventType)comboType.SelectedIndex,true);
			}
			else {
				_listEhrMeasureEvents=EhrMeasureEvents.GetAllByTypeFromDB(PIn.DateT(textDateStart.Text),PIn.DateT(textDateEnd.Text),(EhrMeasureEventType)comboType.SelectedIndex-1,false);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProviders","Event Type"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","Date"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","PatNum"),60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","More Info"),160);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEhrMeasureEvents.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_typeNames[(int)_listEhrMeasureEvents[i].EventType]);
				row.Cells.Add(_listEhrMeasureEvents[i].DateTEvent.ToShortDateString());
				row.Cells.Add(_listEhrMeasureEvents[i].PatNum.ToString());
				row.Cells.Add(_listEhrMeasureEvents[i].MoreInfo);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrMeasureEventEdit FormEMEE=new FormEhrMeasureEventEdit(_listEhrMeasureEvents[e.Row]);
			FormEMEE.ShowDialog();
			if(FormEMEE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butAuditTrail_Click(object sender,EventArgs e) {
			List<Permissions> listPermissions=new List<Permissions>();
			listPermissions.Add(Permissions.EhrMeasureEventEdit);
			using FormAuditOneType FormAOT=new FormAuditOneType(0,listPermissions,Lan.g(this,"EHR Measure Event Edits"),0);
			FormAOT.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboType_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}