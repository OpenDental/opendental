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
		public long SelectedPatNum;
		private DataTable TableRegKeys;

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
			TableRegKeys=RegistrationKeys.GetAllWithoutCharges();
			Cursor.Current=Cursors.Default;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("RegKey",140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Family",200);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Repeating Charge",150);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<TableRegKeys.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(TableRegKeys.Rows[i]["PatNum"].ToString());
				row.Cells.Add(TableRegKeys.Rows[i]["RegKey"].ToString());
				row.Cells.Add(TableRegKeys.Rows[i]["LName"].ToString()+", "+TableRegKeys.Rows[i]["FName"].ToString());
				//row.Cells.Add(table.Rows[i]["dateStop"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				return;
			}
			SelectedPatNum=PIn.Long(TableRegKeys.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			Close();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		


	}
}





















