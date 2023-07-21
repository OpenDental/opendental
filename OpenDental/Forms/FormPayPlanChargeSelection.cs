using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormPayPlanChargeSelection:FormODBase {
		private List<PayPlanCharge> _listPayPlanCharges;
		private PayPlan _payPlan;

		public FormPayPlanChargeSelection(List<PayPlanCharge> listPayPlanCharges,PayPlan payPlan) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPayPlanCharges=listPayPlanCharges;
			_payPlan=payPlan;
		}

		private void FormPayPlanChargeSelection_Load(object sender,EventArgs e) {
			FillListBox();
		}

		private void FillListBox() {
			listBoxPayPlanCharges.Items.Clear();
			int numCharges=0;
			DateTime datePrevCharge=DateTime.MinValue;
			List<long> listProcNums=_listPayPlanCharges.Where(x => x.LinkType==PayPlanLinkType.Procedure).Select(x => x.FKey).ToList();
			List<Procedure> listProcedures=Procedures.GetManyProc(listProcNums,false);
			List<long> listCodeNums=listProcedures.Select(x => x.CodeNum).ToList();
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetCodesForCodeNums(listCodeNums);
			for(int i=0;i<_listPayPlanCharges.Count;i++) {
				if(_listPayPlanCharges[i].ChargeDate!=datePrevCharge) {
					numCharges++;
				}
				string descript="#"+numCharges;
				if(_listPayPlanCharges[i].LinkType==PayPlanLinkType.Procedure) {
					Procedure curProc=listProcedures.FirstOrDefault(x => x.ProcNum==_listPayPlanCharges[i].FKey);
					if(curProc!=null) {
						if(_listPayPlanCharges[i].ChargeDate==DateTime.MaxValue && (curProc.ProcStatus==ProcStat.TP || curProc.ProcStatus==ProcStat.TPi)) {
							descript="";
						}
						ProcedureCode curProcCode=listProcedureCodes.FirstOrDefault(x=> x.CodeNum==curProc.CodeNum);
						if(curProcCode!=null) {
							descript+=" "+curProcCode.ProcCode;
						}
						if(curProcCode.AbbrDesc!="") {
							descript+=" - "+curProcCode.AbbrDesc;
						}
					}
				}
				if(_listPayPlanCharges[i].LinkType==PayPlanLinkType.Adjustment) {
					descript+=" - "+Lan.g("Payment Plan","Adjustment");
				}
				if(_listPayPlanCharges[i].Note!="") {
					descript+=" "+_listPayPlanCharges[i].Note;
				}
				listBoxPayPlanCharges.Items.Add(descript);	
				datePrevCharge=_listPayPlanCharges[i].ChargeDate;
			}
		}

		private void listBoxPayPlanCharges_DoubleClick(object sender,EventArgs e) {
			if(listBoxPayPlanCharges.SelectedIndex!=-1) {
				using FormPayPlanChargeEdit formPayPlanChargeEdit=new FormPayPlanChargeEdit(_listPayPlanCharges[listBoxPayPlanCharges.SelectedIndex],_payPlan);
				formPayPlanChargeEdit.ShowDialog();
				if(formPayPlanChargeEdit.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(formPayPlanChargeEdit.PayPlanChargeCur==null) {
					_listPayPlanCharges.RemoveAt(listBoxPayPlanCharges.SelectedIndex);
				}
				PayPlanCharges.Sync(_listPayPlanCharges,_payPlan.PayPlanNum);
				FillListBox();//Refreshes the list box
			}
		}

		private void butZeroOutCharges_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to zero out all charges for this charge date. Do you still want to continue?")) {
				return;
			}
			//Zeros out all charges from inside of the _listPayPlanCharges list. 
			for(int i=0;i<_listPayPlanCharges.Count;i++) {
				_listPayPlanCharges[i].Principal=0;
				_listPayPlanCharges[i].Interest=0;
			}
			PayPlanCharges.Sync(_listPayPlanCharges,_payPlan.PayPlanNum);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxPayPlanCharges.SelectedIndex!=-1) {
				using FormPayPlanChargeEdit formPayPlanChargeEdit=new FormPayPlanChargeEdit(_listPayPlanCharges[listBoxPayPlanCharges.SelectedIndex],_payPlan);
				formPayPlanChargeEdit.ShowDialog();
				if(formPayPlanChargeEdit.DialogResult==DialogResult.Cancel) {
					return;
				}
				if(formPayPlanChargeEdit.PayPlanChargeCur==null) {
					_listPayPlanCharges.RemoveAt(listBoxPayPlanCharges.SelectedIndex);
				}
				PayPlanCharges.Sync(_listPayPlanCharges,_payPlan.PayPlanNum);
				FillListBox();//Refreshes the list box
			}
			else {
				MsgBox.Show(this,"You must select a charge first.");
			}
		}
	}
}