using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>Lets the user choose which payment plan to attach a payment to if there are more than one available.</summary>
	public partial class FormPayPlanSelect : FormODBase {
		/// <summary>A list of plans passed to this form which are to be displayed.</summary>
		private List<PayPlan> _listPayPlansValid;
		private List<PayPlanCharge> _listPayPlanCharges;
		///<summary>Have the option to not select a payment plan.</summary>
		private bool _doIncludeNone;

		/// <summary>The pk of the plan selected.</summary>
		public long PayPlanNumSelected;

		///<summary>Optionally pass in the ability to not select a payment plan (include a None button)</summary>
		public FormPayPlanSelect(List<PayPlan> listPayPlansValid,bool doIncludeNone=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_doIncludeNone=doIncludeNone;
			_listPayPlansValid=listPayPlansValid;
		}

		private void FormPayPlanSelect_Load(object sender, System.EventArgs e) {
			if(_doIncludeNone) {
				this.Text=Lan.g(this,"Attach to payment plan?");
				labelExpl.Visible=true;
				butNone.Visible=true;
			}
			_listPayPlanCharges=PayPlanCharges.GetForPayPlans(_listPayPlansValid.Select(x => x.PayPlanNum).ToList());
			FillGrid();
			gridMain.SetSelected(0,true); 
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Patient",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Category",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Total Cost",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Balance",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Due Now",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Active",80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			List<long> listPayPlanNums=_listPayPlansValid.Select(x => x.PayPlanNum).ToList();
			List<PaySplit> listPaySplits=PaySplits.GetForPayPlans(listPayPlanNums);
			List<Patient> listPatients=Patients.GetLimForPats(_listPayPlansValid.Select(x=>x.PatNum).ToList());
			gridMain.ListGridRows.Clear();
			for(int i=0;i < _listPayPlansValid.Count;i++) {
				//no db calls are made in this loop because we have all the necessary information already.
				PayPlan planPlan=_listPayPlansValid[i];
				if(checkShowActiveOnly.Checked && planPlan.IsClosed){//User doesn't want to show closed payment plans.
					continue;
				}
				Patient patient=listPatients.Where(x => x.PatNum == planPlan.PatNum).FirstOrDefault();
				GridRow row=new GridRow();
				row.Cells.Add(planPlan.PayPlanDate.ToShortDateString());//date
				row.Cells.Add(patient.LName+", "+patient.FName);//patient			
				if(planPlan.PlanCategory==0) {
					row.Cells.Add(Lan.g(this,"None"));
				}
				else {
					row.Cells.Add(Defs.GetDef(DefCat.PayPlanCategories,planPlan.PlanCategory).ItemName);
				}
				row.Cells.Add(PayPlans.GetTotalCost(planPlan.PayPlanNum,_listPayPlanCharges).ToString("F"));//total cost
				row.Cells.Add(PayPlans.GetBalance(planPlan.PayPlanNum,_listPayPlanCharges,listPaySplits).ToString("F"));//balance
				row.Cells.Add(PayPlans.GetDueNow(planPlan.PayPlanNum,_listPayPlanCharges,listPaySplits).ToString("F"));//due now
				if(planPlan.IsClosed) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("X");//active pay plan
				}
				row.Tag=planPlan.PayPlanNum;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			PayPlanNumSelected=(long)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butNone_Click(object sender,EventArgs e) {
			PayPlanNumSelected=0;
			DialogResult=DialogResult.OK;
		}

		private void gridMain_KeyDown(object sender,KeyEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1 || (e.KeyCode!=Keys.Enter)) {
				return;
			}
			PayPlanNumSelected=(long)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void checkShowActiveOnly_MouseClick(object sender,MouseEventArgs e){
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select a payment plan first."));
				return;
			}
			PayPlanNumSelected=(long)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}




































