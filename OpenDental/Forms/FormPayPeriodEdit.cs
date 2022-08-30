using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPayPeriodEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private PayPeriod _payPeriodCur;
		private PayPeriod _payPeriodOld;
		public bool IsSaveToDb=true;
		///<summary>List of PayPeriods that have not been inserted into the database.</summary>
		private List<PayPeriod> _listNonInsertedPayPeriods;

		///<summary></summary>
		public FormPayPeriodEdit(PayPeriod payPeriodCur,List<PayPeriod> listPayPeriods=null)
		{
			//
			// Required for Windows Form Designer support
			//
			_payPeriodCur=payPeriodCur;
			_payPeriodOld=payPeriodCur.Copy();
			if(listPayPeriods!=null) { 
				_listNonInsertedPayPeriods=listPayPeriods.Where(x => x.PayPeriodNum==0).ToList();
			}
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayPeriodEdit_Load(object sender, System.EventArgs e) {
			if(_payPeriodCur.DateStart.Year>1880){
				textDateStart.Text=_payPeriodCur.DateStart.ToShortDateString();
			}
			if(_payPeriodCur.DateStop.Year>1880){
				textDateStop.Text=_payPeriodCur.DateStop.ToShortDateString();
			}
			if(_payPeriodCur.DatePaycheck.Year>1880){
				textDatePaycheck.Text=_payPeriodCur.DatePaycheck.ToShortDateString();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Abort;
				return;
			}
			PayPeriods.Delete(_payPeriodCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateStart.IsValid()
				|| !textDateStop.IsValid()
				|| !textDatePaycheck.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateStart.Text=="" || textDateStop.Text=="") {
				MsgBox.Show(this,"Start and end dates are required.");
				return;
			}
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateStop=PIn.Date(textDateStop.Text);
			DateTime datePaycheck=PIn.Date(textDatePaycheck.Text);
			if(dateStart>dateStop) {
				MsgBox.Show(this,"The End Date cannot be before the Start Date.  Please change the date range.");
				return;
			}
			if(dateStop>datePaycheck) {
				MsgBox.Show(this,"The Paycheck Date must be on or after the End Date.  Please change the End Date or the Paycheck Date.");
				return;
			}
			_payPeriodCur.DateStart=PIn.Date(textDateStart.Text);
			_payPeriodCur.DateStop=PIn.Date(textDateStop.Text);
			_payPeriodCur.DatePaycheck=PIn.Date(textDatePaycheck.Text);
			PayPeriods.RefreshCache(); //Refresh the cache to include any other changes that might have been made in FormTimeCardSetup.
			List<PayPeriod> listExistingPayPeriods=PayPeriods.GetDeepCopy();
			if(_listNonInsertedPayPeriods!=null) {
				//Add any payperiods that have not been inserted into the db. 
				listExistingPayPeriods.AddRange(_listNonInsertedPayPeriods.FindAll(x => !x.IsSame(_payPeriodCur)));
			}
			if(PayPeriods.AreAnyOverlapping(listExistingPayPeriods,new List<PayPeriod>() { _payPeriodCur })) {
				MsgBox.Show(this,"This pay period overlaps with existing pay periods. Please fix this pay period first.");
				return;
			}
			if(IsSaveToDb) {
				if(IsNew){
					PayPeriods.Insert(_payPeriodCur);
				}
				else{
					PayPeriods.Update(_payPeriodCur);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			_payPeriodCur.DateStart=_payPeriodOld.DateStart;
			_payPeriodCur.DateStop=_payPeriodOld.DateStop;
			_payPeriodCur.DatePaycheck=_payPeriodOld.DatePaycheck;
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		


	}
}





















