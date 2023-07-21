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
		private List<InsEditPatLog> _listInsEditPatLogs;

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
			if(_listInsEditPatLogs==null) {
				Cursor=Cursors.WaitCursor;
				_listInsEditPatLogs=InsEditPatLogs.GetLogsForPatPlan(_patPlan.PatPlanNum,_patPlan.InsSubNum);
				Cursor=Cursors.Default;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("Log Date",135));
			gridMain.Columns.Add(new GridColumn("User",100));
			gridMain.Columns.Add(new GridColumn("LogType",100));
			gridMain.Columns.Add(new GridColumn("Key",55));
			gridMain.Columns.Add(new GridColumn("Description",150));
			gridMain.Columns.Add(new GridColumn("Field",110));
			gridMain.Columns.Add(new GridColumn("Before",150));
			gridMain.Columns.Add(new GridColumn("After",150));
			gridMain.ListGridRows.Clear();
			List<GridRow> listGridRows = ConstructGridRows();
			for(int i=0;i<listGridRows.Count;i++) {
				gridMain.ListGridRows.Add(listGridRows[i]);
			}
			gridMain.EndUpdate();
		}

		///<summary>Actually creates the GridRows and returns them in a list.</summary>
		private List<GridRow> ConstructGridRows() {
			DateTime dateFrom=dateRangePicker.GetDateTimeFrom();
			DateTime dateTo=dateRangePicker.GetDateTimeTo();
			if(dateTo==DateTime.MinValue) {
				dateTo=DateTime.Now;
			}
			GridRow row;
			List<GridRow> listGridRows=new List<GridRow>();
			List<Userod> listUserods = Userods.GetDeepCopy();
			Dictionary<long,Userod> dictionaryLongUserods=Userods.GetDeepCopy().ToDictionary(x => x.UserNum,x => x);
			for(int i=0;i<_listInsEditPatLogs.Count;i++){
				if(!_listInsEditPatLogs[i].DateTStamp.Between(dateFrom,dateTo)) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_listInsEditPatLogs[i].DateTStamp.ToString());
				Userod userod = listUserods.Find(x=>x.UserNum==_listInsEditPatLogs[i].UserNum);
				if(userod==null) {
					row.Cells.Add(Lan.g(this,"Unknown")+"("+POut.Long(_listInsEditPatLogs[i].UserNum)+")");//Unable to find the corresponding user.  
				}
				else {
					row.Cells.Add(userod.UserName); 
				}
				row.Cells.Add(_listInsEditPatLogs[i].LogType.ToString());
				row.Cells.Add(_listInsEditPatLogs[i].FKey.ToString());
				row.Cells.Add(_listInsEditPatLogs[i].Description);
				row.Cells.Add(_listInsEditPatLogs[i].FieldName);
				row.Cells.Add(_listInsEditPatLogs[i].OldValue);
				row.Cells.Add(_listInsEditPatLogs[i].NewValue);
				row.Tag=_listInsEditPatLogs[i];
				listGridRows.Add(row);
			}
			return listGridRows;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}