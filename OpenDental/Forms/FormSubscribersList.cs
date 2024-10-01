using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSubscribersList:FormODBase {
		///<summary>Ins sub for the currently selected plan.</summary>
		public InsSub InsSubCur;
		///<summary>Currently selected plan in the window.</summary>
		public InsPlan InsPlanCur;

		public FormSubscribersList() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSubscribersList_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridSubscribers.BeginUpdate();
			gridSubscribers.Columns.Clear();
			gridSubscribers.Columns.Add(new GridColumn(Lan.g(this,"Name"),200));
			gridSubscribers.ListGridRows.Clear();
			long excludeSub=-1;
			if(InsSubCur!=null){
				excludeSub=InsSubCur.InsSubNum;
			}
			List<string> listSubs=InsSubs.GetSubscribersForPlan(InsPlanCur.PlanNum,excludeSub);
			for(int i=0;i<listSubs.Count;i++) {
				gridSubscribers.ListGridRows.Add(new GridRow(listSubs[i]));
			}
			gridSubscribers.EndUpdate();
		}

	}
}