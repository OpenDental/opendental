using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoRxSelect:FormODBase {
		private List<OrthoRx> _listOrthoRxsAll;
		///<summary>This is a derived list. It's every item from _listOrthoRxsAll that's not in ListOrthoRxsSelected.</summary>
		private List<OrthoRx> _listOrthoRxsAvail;
		public List<OrthoRx> ListOrthoRxsSelected;

		public FormOrthoRxSelect() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoRxSelect_Load(object sender,EventArgs e) {
			_listOrthoRxsAll=OrthoRxs.GetDeepCopy();
			ListOrthoRxsSelected=new List<OrthoRx>();
			FillLists();
		}

		private void FillLists(){
			//List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			_listOrthoRxsAvail=new List<OrthoRx>();
			for(int i=0;i<_listOrthoRxsAll.Count;i++){
				if(ListOrthoRxsSelected.Contains(_listOrthoRxsAll[i])){
					continue;
				}
				_listOrthoRxsAvail.Add(_listOrthoRxsAll[i]);
			}
			listBoxAvail.Items.Clear();
			listBoxAvail.Items.AddList(_listOrthoRxsAvail,x=>x.Description);
			listBoxSelected.Items.Clear();
			listBoxSelected.Items.AddList(ListOrthoRxsSelected,x=>x.Description);
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listBoxSelected.SelectedIndices.Count<1){
				MsgBox.Show(this,"Please select items on the right first.");
				return;
			}
			for(int i=listBoxSelected.SelectedIndices.Count-1;i>=0;i--){//backwards
				ListOrthoRxsSelected.Remove(ListOrthoRxsSelected[listBoxSelected.SelectedIndices[i]]);
			}
			FillLists();
		}

		private void butSelect_Click(object sender,EventArgs e) {
			if(listBoxAvail.SelectedIndices.Count<1){
				MsgBox.Show(this,"Please select items on the left first.");
				return;
			}
			for(int i=0;i<listBoxAvail.SelectedIndices.Count;i++){
				ListOrthoRxsSelected.Add(_listOrthoRxsAvail[listBoxAvail.SelectedIndices[i]]);
			}
			FillLists();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxSelected.Items.Count<1){
				MsgBox.Show(this,"Please move items to the list on the right first.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}