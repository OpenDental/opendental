using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormTreatmentPlanDiscount:FormODBase {
		private List<Procedure> _oldListProcs;
		private List<Procedure> _listProcs;

		public FormTreatmentPlanDiscount(List<Procedure> listProcs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_oldListProcs=new List<Procedure>();
			for(int i=0;i<listProcs.Count;i++) {
				_oldListProcs.Add(listProcs[i].Copy());
			}
			_listProcs=listProcs;
		}

		private void FormTreatmentPlanDiscount_Load(object sender,EventArgs e) {
			textPercentage.Text=PrefC.GetString(PrefName.TreatPlanDiscountPercent);
		}

		private void butOK_Click(object sender,EventArgs e) {
			int countProcsLinkedToOrthoCase=0;
			float percent=0;//Placeholder
			if(!float.TryParse(textPercentage.Text,out percent)) {
				MsgBox.Show(this,"Percent is invalid. Please enter a valid number to continue.");
				return;
			}
			bool hasDiscount=false;
			for(int i=0;i<_listProcs.Count;i++) {
				if(_listProcs[i].Discount!=0) {
					hasDiscount=true;
					break;
				}
			}
			if(hasDiscount //A discount exists for a procedure
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"One or more of the selected procedures has a discount value already set.  This will overwrite current discount values with a new value.  Continue?")) 
			{
				return;
			}
			List<long> listProcNumsLinkedToOrthoCases=OrthoProcLinks.GetManyForProcs(_listProcs.Select(x => x.ProcNum).ToList()).Select(y => y.ProcNum).ToList();
			for(int j=0;j<_listProcs.Count;j++) {
				if(listProcNumsLinkedToOrthoCases.Contains(_listProcs[j].ProcNum)) {
					countProcsLinkedToOrthoCase++;
				}
				else if(percent==0) {
					_listProcs[j].Discount=0;//Potentially clears out old discount.
				}
				else {
					_listProcs[j].Discount=_listProcs[j].ProcFee*(percent/100);
				}
				if(_listProcs[j].Discount!=_oldListProcs[j].Discount) {//Discount was changed
					string message=Lan.g(this,"Discount created or changed from Treat Plan module for procedure")
						+": "+ProcedureCodes.GetProcCode(_listProcs[j].CodeNum).ProcCode+"  "+Lan.g(this,"Dated")
						+": "+_listProcs[j].ProcDate.ToShortDateString()+"  "+Lan.g(this,"With a Fee of")+": "+_listProcs[j].ProcFee.ToString("c")+".  "+Lan.g(this,"Attributed a")+" "+percent
					+" "+Lan.g(this,"percent discount, changing the discount value from")+" "+_oldListProcs[j].Discount.ToString("c")+" "+Lan.g(this,"to")+" "+_listProcs[j].Discount.ToString("c");
					SecurityLogs.MakeLogEntry(Permissions.TreatPlanDiscountEdit,_listProcs[j].PatNum,message);
				}
				Procedures.Update(_listProcs[j],_oldListProcs[j]);
			}
			if(countProcsLinkedToOrthoCase>0) {
				string countProcsSkipped=countProcsLinkedToOrthoCase.ToString();
				MessageBox.Show(this,Lans.g(this,"Procedures attached to ortho cases cannot have discounts. Procedures skipped:")+" "+countProcsSkipped);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}