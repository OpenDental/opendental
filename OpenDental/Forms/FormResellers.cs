using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormResellers:FormODBase {
		private DataTable _tableResellers;

		public FormResellers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=menuRightClick;
		}

		private void FormResellers_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			_tableResellers=Resellers.GetResellerList();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("PatNum",60);
			gridMain.Columns.Add(col);
			col=new GridColumn("LName",150);
			gridMain.Columns.Add(col);
			col=new GridColumn("FName",130);
			gridMain.Columns.Add(col);
			col=new GridColumn("Email",200);
			gridMain.Columns.Add(col);
			col=new GridColumn("WkPhone",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("PhoneNumberVal",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Address",180);
			gridMain.Columns.Add(col);
			col=new GridColumn("City",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("State",40);
			gridMain.Columns.Add(col);
			col=new GridColumn("PatStatus",80);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableResellers.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableResellers.Rows[i]["PatNum"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["LName"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["FName"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["Email"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["WkPhone"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["PhoneNumberVal"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["Address"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["City"].ToString());
				row.Cells.Add(_tableResellers.Rows[i]["State"].ToString());
				row.Cells.Add(((PatientStatus)PIn.Int(_tableResellers.Rows[i]["PatStatus"].ToString())).ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Reseller reseller=Resellers.GetOne(PIn.Long(_tableResellers.Rows[e.Row]["ResellerNum"].ToString()));
			using FormResellerEdit formResellerEdit=new FormResellerEdit(reseller);
			formResellerEdit.ShowDialog();
			FillGrid();//Could have deleted the reseller.
		}

		private void menuItemAccount_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select a reseller first.");
				return;
			}
			GotoModule.GotoAccount(PIn.Long(_tableResellers.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString()));
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			Patient patientSelected=Patients.GetPat(formPatientSelect.PatNumSelected);
			if(patientSelected.Guarantor!=formPatientSelect.PatNumSelected) {
				MsgBox.Show(this,"Customer must be a guarantor before they can be added as a reseller.");
				return;
			}
			if(Resellers.IsResellerFamily(patientSelected)) {
				MsgBox.Show(this,"Customer is already a reseller or part of a reseller family.");
				return;
			}
			Reseller reseller=new Reseller() {
				PatNum=formPatientSelect.PatNumSelected,
				BillingType=42,//Hardcoded to HQs "No Support: Developer/Reseller"
				VotesAllotted=0,
				Note="This is a customer of a reseller.  We do not directly support this customer.",
			};
			Resellers.Insert(reseller);
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}