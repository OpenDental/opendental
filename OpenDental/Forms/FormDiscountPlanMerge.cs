using System;
using System.IO;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDiscountPlanMerge:FormODBase {

		private DiscountPlan _discountPlanInto;
		private DiscountPlan _discountPlanFrom;

		public FormDiscountPlanMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDiscountPlanMerge_Load(object sender,EventArgs e) {
		}

		private void butChangePlanInto_Click(object sender,EventArgs e) {
			using FormDiscountPlans formDiscountPlans=new FormDiscountPlans();
			formDiscountPlans.IsSelectionMode=true;
			if(formDiscountPlans.ShowDialog()!=DialogResult.OK){
				CheckUIState();
				return;
			}
			_discountPlanInto=formDiscountPlans.DiscountPlanSelected;
			textDescriptionInto.Text=_discountPlanInto.Description;
			textFeeSchedInto.Text=FeeScheds.GetDescription(_discountPlanInto.FeeSchedNum);
			textAdjTypeInto.Text=Defs.GetName(DefCat.AdjTypes,_discountPlanInto.DefNum);
			CheckUIState();
		}

		private void butChangePlanFrom_Click(object sender,EventArgs e) {
			using FormDiscountPlans formDiscountPlans=new FormDiscountPlans();
			formDiscountPlans.IsSelectionMode=true;
			if(formDiscountPlans.ShowDialog()!=DialogResult.OK){
				CheckUIState();
				return;
			}
			_discountPlanFrom=formDiscountPlans.DiscountPlanSelected;
			textDescriptionFrom.Text=_discountPlanFrom.Description;
			textFeeSchedFrom.Text=FeeScheds.GetDescription(_discountPlanFrom.FeeSchedNum);
			textAdjTypeFrom.Text=Defs.GetName(DefCat.AdjTypes,_discountPlanFrom.DefNum);
			CheckUIState();
		}

		private void CheckUIState(){
			butMerge.Enabled=(_discountPlanInto!=null && _discountPlanFrom!=null);
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(_discountPlanFrom.DiscountPlanNum==_discountPlanInto.DiscountPlanNum) {
				MsgBox.Show(this,"You must select two different Discount Plans to merge.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Merge the Discount Plan at the bottom into the Discount Plan shown at the top?")) {
				return;//The user chose not to merge.
			}
			Cursor=Cursors.WaitCursor;
			DiscountPlans.MergeTwoPlans(_discountPlanInto,_discountPlanFrom);
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.DiscountPlanMerge,0,$"{_discountPlanFrom.Description} merged into {_discountPlanInto.Description}");
			MsgBox.Show(this,"Plans merged successfully.");
			_discountPlanFrom=null;
			textDescriptionFrom.Text="";
			textFeeSchedFrom.Text="";
			textAdjTypeFrom.Text="";
			CheckUIState();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}