using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormCustomerManagement : FormODBase {
		///<Summary>This will only contain a value if the user clicked GoTo.</Summary>
		public long PatNumSelected;
		private DataTable _tableRegKeys;

		///<summary></summary>
		public FormCustomerManagement()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=contextMain;
		}

		private void FormCustomerManagement_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			Cursor.Current=Cursors.WaitCursor;
			_tableRegKeys=RegistrationKeys.GetAllWithoutCharges();
			Cursor.Current=Cursors.Default;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("PatNum",60);
			gridMain.Columns.Add(col);
			col=new GridColumn("RegKey",140);
			gridMain.Columns.Add(col);
			col=new GridColumn("Family",200);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn("Repeating Charge",150);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableRegKeys.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(_tableRegKeys.Rows[i]["PatNum"].ToString());
				row.Cells.Add(_tableRegKeys.Rows[i]["RegKey"].ToString());
				row.Cells.Add(_tableRegKeys.Rows[i]["LName"].ToString()+", "+_tableRegKeys.Rows[i]["FName"].ToString());
				//row.Cells.Add(table.Rows[i]["dateStop"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				return;
			}
			PatNumSelected=PIn.Long(_tableRegKeys.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			Close();
		}

	}
}