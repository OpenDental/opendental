using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormResellerEdit:FormODBase {
		private Reseller _resellerCur;
		private DataTable _tableCustomers;
		private List<ResellerService> _listServices;

		public FormResellerEdit(Reseller reseller) {
			_resellerCur=reseller;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=menuRightClick;
		}

		private void FormResellerEdit_Load(object sender,EventArgs e) {
			//Only Jordan should be able to alter reseller credentials.
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				labelCredentials.Text="Only users with Security Admin can edit credentials.";
				textUserName.ReadOnly=true;
				textPassword.ReadOnly=true;
			}
			textUserName.Text=_resellerCur.UserName;
			string password="";
			if(_resellerCur.PasswordHash.Trim()!="") {
				password="********";//Don't show the password hash.
			}
			textPassword.Text=password;
			FillGridMain();
			FillGridServices();
			comboBillingType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes,true));
			ODException.SwallowAnyException(() => comboBillingType.SetSelectedDefNum(_resellerCur.BillingType));
			textNote.Text=_resellerCur.Note;
			textVotesAllotted.Text=_resellerCur.VotesAllotted.ToString();
		}

		private void FillGridMain() {
			double total=0;
			_tableCustomers=Resellers.GetResellerCustomersList(_resellerCur.PatNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("RegKey",130);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("ProcCode",60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Descript",180);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Fee",70);
			col.TextAlign=HorizontalAlignment.Right;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("DateStart",80);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("DateStop",80);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableCustomers.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableCustomers.Rows[i]["PatNum"].ToString());
				row.Cells.Add(_tableCustomers.Rows[i]["RegKey"].ToString());
				row.Cells.Add(_tableCustomers.Rows[i]["ProcCode"].ToString());
				row.Cells.Add(_tableCustomers.Rows[i]["Descript"].ToString());
				double fee=PIn.Double(_tableCustomers.Rows[i]["Fee"].ToString());
				row.Cells.Add(fee.ToString("F"));
				total+=fee;
				DateTime dateStart=PIn.Date(_tableCustomers.Rows[i]["DateStart"].ToString());
				row.Cells.Add(dateStart.Year>1880 ? dateStart.ToShortDateString() : "");
				DateTime dateStop=PIn.Date(_tableCustomers.Rows[i]["DateStop"].ToString());
				row.Cells.Add(dateStop.Year>1880 ? dateStop.ToShortDateString() : "");
				row.Cells.Add(_tableCustomers.Rows[i]["Note"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			labelTotal.Text="Total: "+total.ToString("C");
		}

		private void FillGridServices() {
			_listServices=ResellerServices.GetServicesForReseller(_resellerCur.ResellerNum);
			gridServices.BeginUpdate();
			gridServices.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Description",180);
			gridServices.ListGridColumns.Add(col);
			col=new GridColumn("Fee",40){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Right;
			gridServices.ListGridColumns.Add(col);
			gridServices.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listServices.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ProcedureCodes.GetLaymanTerm(_listServices[i].CodeNum));
				row.Cells.Add(_listServices[i].Fee.ToString("F"));
				gridServices.ListGridRows.Add(row);
			}
			gridServices.EndUpdate();
			//The Available Services grid has changed, re-evaluate the bundle requirement label visibility.
			labelBundleRequired.Visible=IsBundleMissing();
		}

		///<summary>Returns true if there are services available to the reseller but the bundle is not one of them.
		///Also causes a red label to show to the user that indicates that the Bundle service is required.
		///Returns false if the Bundle service is present OR if there are no available services at all (both are acceptable).</summary>
		private bool IsBundleMissing() {
			//All resellers MUST be given the "Bundle" eService code as an option otherwise the Signup Portal will not load correctly.
			long codeNumBundle=ProcedureCodes.GetCodeNum("042");//Bundle.
			if(_listServices!=null && _listServices.Count > 0 && !_listServices.Any(x => x.CodeNum==codeNumBundle)) {
				return true;
			}
			return false;
		}

		private void menuItemAccount_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select a customer first.");
				return;
			}
			GotoModule.GotoAccount(PIn.Long(_tableCustomers.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString()));
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//Only Jordan should be able to add services.
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			ResellerService resellerService=new ResellerService();
			resellerService.ResellerNum=_resellerCur.ResellerNum;
			using FormResellerServiceEdit FormRSE=new FormResellerServiceEdit(resellerService);
			FormRSE.IsNew=true;
			FormRSE.ShowDialog();
			if(FormRSE.DialogResult==DialogResult.OK) {
				FillGridServices();
			}
		}

		private void gridServices_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Only Jordan should be able to edit services.
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				return;
			}
			ResellerService resellerService=_listServices[gridServices.GetSelectedIndex()];
			using FormResellerServiceEdit FormRSE=new FormResellerServiceEdit(resellerService);
			FormRSE.ShowDialog();
			if(FormRSE.DialogResult==DialogResult.OK) {
				FillGridMain();
				FillGridServices();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//Only Jordan should be able to delete resellers.
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			//Do not let the reseller be deleted if they have customers in their list.
			if(Resellers.HasActiveResellerCustomers(_resellerCur)) {
				MsgBox.Show(this,"This reseller cannot be deleted until all active services are removed from their customers.  This should be done using the reseller portal.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will update PatStatus to inactive and set every registartion key's stop date.\r\nContinue?")) {
				return;
			}
			Patient patOld=Patients.GetPat(_resellerCur.PatNum);
			Patient patCur=patOld.Copy();
			patCur.PatStatus=PatientStatus.Inactive;
			Patients.Update(patCur,patOld);
			string logEntry=Lan.g(this,"Patient's status changed from ")+patOld.PatStatus.GetDescription()+Lan.g(this," to ")
				+patCur.PatStatus.GetDescription()+Lan.g(this," from the Reseller Edit window.");
			SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patCur.PatNum,logEntry);
			RegistrationKey[] regKeys=RegistrationKeys.GetForPatient(patCur.PatNum);
			for(int i=0;i<regKeys.Length;i++) {
				DateTime dateTimeNow=MiscData.GetNowDateTime();
				if(regKeys[i].DateEnded.Year>1880 && regKeys[i].DateEnded<dateTimeNow) {
					continue;//Key already ended.  Nothing to do.
				}
				regKeys[i].DateEnded=MiscData.GetNowDateTime();
				RegistrationKeys.Update(regKeys[i]);
			}
			Resellers.Delete(_resellerCur.ResellerNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textPassword.Text!="" && textUserName.Text.Trim()=="") {
				MsgBox.Show(this,"User Name cannot be blank.");
				return;
			}
			if(textUserName.Text!="" && textPassword.Text.Trim()=="") {
				MsgBox.Show(this,"Password cannot be blank.");
				return;
			}
			if(textUserName.Text!="" && Resellers.IsUserNameInUse(_resellerCur.PatNum,textUserName.Text)) {
				MsgBox.Show(this,"User Name already in use.");
				return;
			}
			if(IsBundleMissing()) {
				if(MessageBox.Show("The Signup Portal will not load correctly for the customers of this reseller.  "
						+"The 'Bundle' eService (042) MUST be included in the Available Services grid first.\r\n\r\n"
						+"Continue anyway?","!!!WARNING!!!",MessageBoxButtons.YesNo)==DialogResult.No)
				{
					return;
				}
			}
			long billingType;
			try {
				billingType=comboBillingType.GetSelectedDefNum();
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Billing Type is invalid.");
				return;
			}
			if(!int.TryParse(textVotesAllotted.Text,out int votesAllotted) || votesAllotted < 0) {
				MsgBox.Show(this,"Votes Allotted is invalid.");
				return;
			}
			_resellerCur.VotesAllotted=votesAllotted;
			_resellerCur.Note=textNote.Text;
			_resellerCur.BillingType=billingType;
			_resellerCur.UserName=textUserName.Text;
			if(textPassword.Text!="********") {
				_resellerCur.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
			}
			Resellers.Update(_resellerCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}