using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsEditPatLog:FormODBase {
		private PatPlan _patPlan;
		private List<InsEditPatLog> _listLogs;

		///<summary>Opens the window with the passed-in parameters set as the default.</summary>
		public FormInsEditPatLog(PatPlan patPlan) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patPlan=patPlan;
		}

		private void FormInsEditLogs_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				dateRangePicker);
			dateRangePicker.SetDateTimeFrom(DateTime.Now.AddMonths(-1));
			dateRangePicker.SetDateTimeTo(DateTime.Now);
			FillGrid();
		}

		private void FillGrid() {
			if(_listLogs==null) {
				Cursor=Cursors.WaitCursor;
				_listLogs=InsEditPatLogs.GetLogsForPatPlan(_patPlan.PatPlanNum,_patPlan.InsSubNum);
				Cursor=Cursors.Default;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Log Date",135));
			gridMain.ListGridColumns.Add(new GridColumn("User",100));
			gridMain.ListGridColumns.Add(new GridColumn("LogType",100));
			gridMain.ListGridColumns.Add(new GridColumn("Key",55));
			gridMain.ListGridColumns.Add(new GridColumn("Description",150));
			gridMain.ListGridColumns.Add(new GridColumn("Field",110));
			gridMain.ListGridColumns.Add(new GridColumn("Before",150));
			gridMain.ListGridColumns.Add(new GridColumn("After",150));
			gridMain.ListGridRows.Clear();
			ConstructGridRows().ForEach(x => gridMain.ListGridRows.Add(x));
			gridMain.EndUpdate();
		}

		///<summary>Actually creates the GridRows and returns them in a list.</summary>
		private List<GridRow> ConstructGridRows() {
			DateTime dateFrom=dateRangePicker.GetDateTimeFrom();
			DateTime dateTo=dateRangePicker.GetDateTimeTo();
			dateTo=(dateTo==DateTime.MinValue ? DateTime.Now : dateTo);
			GridRow row;
			List<GridRow> listGridRows=new List<GridRow>();
			Dictionary<long,Userod> dictUsers=Userods.GetDeepCopy().ToDictionary(x => x.UserNum,x => x);
			foreach(InsEditPatLog logCur in _listLogs) {
				if(!logCur.DateTStamp.Between(dateFrom,dateTo)) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(logCur.DateTStamp.ToString());
				if(dictUsers.TryGetValue(logCur.UserNum,out Userod user)) {
					row.Cells.Add(user.UserName);
				}
				else {
					row.Cells.Add(Lan.g(this,"Unknown")+"("+POut.Long(logCur.UserNum)+")");//Unable to find the corresponding user.
				}
				row.Cells.Add(logCur.LogType.ToString());
				row.Cells.Add(logCur.FKey.ToString());
				row.Cells.Add(logCur.Description);
				row.Cells.Add(logCur.FieldName);
				row.Cells.Add(logCur.OldValue);
				row.Cells.Add(logCur.NewValue);
				row.Tag=logCur;
				listGridRows.Add(row);
			}
			return listGridRows;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}