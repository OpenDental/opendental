using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOnlinePayments:FormODBase {
		private List<Payment> _listPayments;
		private List<Clinic> _listClinics;
		private Patient[] _arrayPats;

		public FormOnlinePayments() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPendingOnlinePayments_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				FillClinics();
			}
			else {
				comboClinic.Visible=false;
				labelClinic.Visible=false;
			}
			FillProcessStatus();
			FillPaymentSource();
			dateStart.Value=DateTime.Today.AddDays(-30);
			RefreshPayments();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshPayments();
		}

		private void RefreshPayments() {
			List<ProcessStat> listProcessStatuses = comboProcessStatus.GetListSelected<ProcessStat>();
			List<CreditCardSource> listCreditCardSources = comboPaymentSource.GetListSelected<CreditCardSource>();
			if(PrefC.HasClinicsEnabled && Security.CurUser.ClinicIsRestricted) {
				_listPayments=Payments.GetPaymentsUsingFilters(_listClinics.Select(x => x.ClinicNum).ToList(),dateStart.Value,dateEnd.Value,listProcessStatuses,listCreditCardSources);
			}
			else {
				_listPayments=Payments.GetPaymentsUsingFilters(new List<long>(),dateStart.Value,dateEnd.Value,listProcessStatuses,listCreditCardSources);
			}
			_arrayPats=Patients.GetMultPats(_listPayments.Select(x => x.PatNum).ToList());
			FillGrid();
		}

		private void FillGrid() {
			List<Payment> listPaymentsClinic=new List<Payment>();
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedIndex!=0) {//Not 'All' selected
				if(Security.CurUser.ClinicIsRestricted) {
					long clinicNum=_listClinics[comboClinic.SelectedIndex-1].ClinicNum;//Minus 1 for 'All'
					listPaymentsClinic=_listPayments.FindAll(x => x.ClinicNum==clinicNum);
				}
				else {
					if(comboClinic.SelectedIndex==1) {//'Unassigned' selected
						listPaymentsClinic=_listPayments.FindAll(x => x.ClinicNum==0);
					}
					else if(comboClinic.SelectedIndex>1) {
						long clinicNum=_listClinics[comboClinic.SelectedIndex-2].ClinicNum;//Minus 2 for 'All' and 'Unassigned'
						listPaymentsClinic=_listPayments.FindAll(x => x.ClinicNum==clinicNum);
					}
				}
			}
			else {
				listPaymentsClinic=_listPayments;//Use all clinics
			}
			listPaymentsClinic=listPaymentsClinic.OrderBy(x => Clinics.GetAbbr(x.ClinicNum))
				.ThenBy(x => x.PayDate)
				.ThenBy(x => Patients.GetOnePat(_arrayPats,x.PatNum).GetNameLFnoPref()).ToList();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Patient"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Payment pay in listPaymentsClinic) {
				row=new GridRow();
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(pay.ClinicNum);
					row.Cells.Add(clinicAbbr=="" ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				row.Cells.Add(Patients.GetOnePat(_arrayPats,pay.PatNum).GetNameLFnoPref());
				row.Cells.Add(pay.PayDate.ToShortDateString());
				row.Cells.Add(pay.PayAmt.ToString("F"));
				row.Cells.Add(pay.PayNote);
				row.Tag=pay;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		public void FillClinics() {
			_listClinics=Clinics.GetForUserod(Security.CurUser);
			comboClinic.Items.Add(Lan.g(this,"All"));
			comboClinic.SelectedIndex=0;
			int offset=1;
			if(!Security.CurUser.ClinicIsRestricted) {
				comboClinic.Items.Add(Lan.g(this,"Unassigned"));
				offset++;
			}
			_listClinics.ForEach(x => comboClinic.Items.Add(x.Abbr));
			comboClinic.SelectedIndex=_listClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum)+offset;
			if(comboClinic.SelectedIndex-offset==-1) {
				comboClinic.SelectedIndex=0;
			}
		}

		public void FillProcessStatus() {
			comboProcessStatus.IncludeAll=true;
			comboProcessStatus.Items.AddEnums<ProcessStat>();
			comboProcessStatus.SelectedIndex=(int)ProcessStat.OnlinePending;
		}

		public void FillPaymentSource() {
			comboPaymentSource.IncludeAll=true;
			//Adding only online payment sources to combobox
			List<CreditCardSource> onlinePaymentSources=CreditCards.GetCreditCardSourcesForOnlinePayments();
			comboPaymentSource.Items.AddListEnum<CreditCardSource>(onlinePaymentSources);
			comboPaymentSource.IsAllSelected=true;
		}

		private void gridMain_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right) {
				gridMain.SetAll(false);
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Payment pay=(Payment)gridMain.ListGridRows[e.Row].Tag;
			Patient pat=Patients.GetOnePat(_arrayPats,pay.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			using FormPayment FormP=new FormPayment(pat,fam,pay,false);
			FormP.ShowDialog();
			RefreshPayments();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one payment first.");
				return;
			}
			long patNum=((Payment)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).PatNum;
			GlobalFormOpenDental.PatientSelected(Patients.GetPat(patNum),false);
			GlobalFormOpenDental.GoToModule(EnumModuleType.Account,patNum:0);
		}

	}
}