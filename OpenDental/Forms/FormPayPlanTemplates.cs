using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPayPlanTemplates:FormODBase {
		public bool IsSelectionMode;
		public PayPlanTemplate PayPlanTemplateCur;//set on ok and doubleclick
		private bool _isUserClinicRestricted;

		public FormPayPlanTemplates() {
			InitializeComponent();
			InitializeLayoutManager();
			_isUserClinicRestricted=UserClinics.GetForUser(Security.CurUser.UserNum).Count>0;
			Lan.F(this);
		}

		private void FormPayPlanTemplates_Load(object sender,EventArgs e) {
			//if !IsSelectionMode, then OK.Visible=false
			if(!IsSelectionMode) {
				butOK.Visible=false;
				butAddTemplate.Visible=true;
				checkShowHidden.Visible=true;
			}
			comboBoxClinic.IsAllSelected=true;
			if(_isUserClinicRestricted) {
				comboBoxClinic.IncludeAll=false;
			}
			FillTemplates();
		}

		private void FillTemplates() {
			gridPayPlanTemplates.BeginUpdate();
			gridPayPlanTemplates.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Name"),64,HorizontalAlignment.Center);//0
			gridPayPlanTemplates.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Clinic"),64,HorizontalAlignment.Center);//1
				gridPayPlanTemplates.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"APR"),64,HorizontalAlignment.Center);//2
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Interest Delay"),105,HorizontalAlignment.Center);//3
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Payment Amount"),110,HorizontalAlignment.Right);//4
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Number of Payments"),130,HorizontalAlignment.Center);//5
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Down Payment"),100,HorizontalAlignment.Right);//6
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Frequency"),100,HorizontalAlignment.Center);//7
			gridPayPlanTemplates.Columns.Add(col);
			col=new GridColumn(Lan.g(gridPayPlanTemplates.TranslationName,"Treatment Plan Option"),120,HorizontalAlignment.Center);//8
			gridPayPlanTemplates.Columns.Add(col);
			gridPayPlanTemplates.ListGridRows.Clear();
			List<PayPlanTemplate> listTemplates;
			if(!comboBoxClinic.IsAllSelected) {
				listTemplates=PayPlanTemplates.GetMany(comboBoxClinic.ClinicNumSelected);//Gets a list of all templates using the clinicNum from the DB.
			}
			else {
				listTemplates=PayPlanTemplates.GetAll();//Gets a list of all templates from the DB.
			}
			for(int i=0;i<listTemplates.Count;i++) {
				if(listTemplates[i].IsHidden && !checkShowHidden.Checked) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(listTemplates[i].PayPlanTemplateName);//Name
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(listTemplates[i].ClinicNum);
					row.Cells.Add(clinicAbbr);//Clinic
				}
				row.Cells.Add(listTemplates[i].APR.ToString());//APR
				row.Cells.Add(listTemplates[i].InterestDelay.ToString());//Interest Delay
				//If Payment Amount was used, we don't want to show Number of Payments and vice versa.
				if(listTemplates[i].PayAmt==0) {
					row.Cells.Add("");//Payment Amount
					row.Cells.Add(listTemplates[i].NumberOfPayments.ToString());//Number of Payments
				}
				else {
					row.Cells.Add(listTemplates[i].PayAmt.ToString("f"));//Payment Amount
					row.Cells.Add("");//Number of Payments
				}
				row.Cells.Add(listTemplates[i].DownPayment.ToString("f"));//Down Payment
				row.Cells.Add(listTemplates[i].ChargeFrequency.ToString());//Frequency
				row.Cells.Add(listTemplates[i].DynamicPayPlanTPOption.ToString());//Treatment Plan Option
				row.Tag=listTemplates[i];
				gridPayPlanTemplates.ListGridRows.Add(row);
			}
			gridPayPlanTemplates.EndUpdate();
		}

		private void comboBoxClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillTemplates();
		}

		private void gridPayPlanTemplates_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			PayPlanTemplateCur=gridPayPlanTemplates.SelectedTag<PayPlanTemplate>();
			if(IsSelectionMode) {
				DialogResult=DialogResult.OK;
				return;
			}
			OpenEditForm(PayPlanTemplateCur);
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillTemplates();
		}

		private void butAddTemplate_Click(object sender,EventArgs e) {
			OpenEditForm();
		}

		private void OpenEditForm(PayPlanTemplate payPlanTempate=null) {
			using FormPayPlanTemplateEdit formPayPlanTemplateEdit=new FormPayPlanTemplateEdit(payPlanTempate);
			formPayPlanTemplateEdit.ShowDialog();
			FillTemplates();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not visible unless selectionmode
			PayPlanTemplateCur=gridPayPlanTemplates.SelectedTag<PayPlanTemplate>();
			DialogResult=DialogResult.OK;
		}

	}
}