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
		private List<Procedure> _listProceduresOld;
		private List<Procedure> _listProcedures;

		public FormTreatmentPlanDiscount(List<Procedure> listProcedures) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listProceduresOld=new List<Procedure>();
			for(int i=0;i<listProcedures.Count;i++) {
				_listProceduresOld.Add(listProcedures[i].Copy());
			}
			_listProcedures=listProcedures;
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
			for(int i=0;i<_listProcedures.Count;i++) {
				if(_listProcedures[i].Discount!=0) {
					hasDiscount=true;
					break;
				}
			}
			if(hasDiscount //A discount exists for a procedure
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"One or more of the selected procedures has a discount value already set.  This will overwrite current discount values with a new value.  Continue?")) 
			{
				return;
			}
			List<long> listProcNumsLinkedToOrthoCases=OrthoProcLinks.GetManyForProcs(_listProcedures.Select(x => x.ProcNum).ToList()).Select(y => y.ProcNum).ToList();
			for(int j=0;j<_listProcedures.Count;j++) {
				if(listProcNumsLinkedToOrthoCases.Contains(_listProcedures[j].ProcNum)) {
					countProcsLinkedToOrthoCase++;
				}
				else if(percent==0) {
					_listProcedures[j].Discount=0;//Potentially clears out old discount.
				}
				else {
					_listProcedures[j].Discount=_listProcedures[j].ProcFee*(percent/100);
				}
				if(_listProcedures[j].Discount!=_listProceduresOld[j].Discount) {//Discount was changed
					string message=Lan.g(this,"Discount created or changed from Treat Plan module for procedure")
						+": "+ProcedureCodes.GetProcCode(_listProcedures[j].CodeNum).ProcCode+"  "+Lan.g(this,"Dated")
						+": "+_listProcedures[j].ProcDate.ToShortDateString()+"  "+Lan.g(this,"With a Fee of")+": "+_listProcedures[j].ProcFee.ToString("c")+".  "+Lan.g(this,"Attributed a")+" "+percent
					+" "+Lan.g(this,"percent discount, changing the discount value from")+" "+_listProceduresOld[j].Discount.ToString("c")+" "+Lan.g(this,"to")+" "+_listProcedures[j].Discount.ToString("c");
					SecurityLogs.MakeLogEntry(Permissions.TreatPlanDiscountEdit,_listProcedures[j].PatNum,message);
				}
				Procedures.Update(_listProcedures[j],_listProceduresOld[j]);
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