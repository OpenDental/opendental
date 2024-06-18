using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildEdit:FrmODBase {
		///<summary></summary>
		public Child ChildCur;
		///<summary>True if the child is a new entry.</summary>
		public bool IsNew;

		///<summary></summary>
		public FrmChildEdit() {
			InitializeComponent();
			Load+=FrmChildEdit_Load;
			listAuthorized.MouseDoubleClick+=listAuthorized_MouseDoubleClick;
		}

		private void FrmChildEdit_Load(object sender,EventArgs e) {
			FillListBox();
		}

		private void FillListBox() {
			List<ChildParent> listChildParents=ChildParents.GetChildParentsByChildNum(ChildCur.ChildNum);
			listAuthorized.Items.Clear();
			for(int i=0;i<listChildParents.Count;i++) {
				Userod userod=Userods.GetUser(listChildParents[i].Parent);
				listAuthorized.Items.Add(userod.UserName,userod);
			}
		}

		private void listAuthorized_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			//not implemented
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmParentEdit frmParentEdit=new FrmParentEdit();
			frmParentEdit.IsNew=true;
			ChildParent childParent=new ChildParent();
			childParent.ChildNum=ChildCur.ChildNum;//Assign child
			frmParentEdit.ChildParentCur=childParent;
			frmParentEdit.ShowDialog();
			if(!frmParentEdit.IsDialogOK) {
				return;
			}
			FillListBox();//Refill to show changes
		}

		private void butRoomSelect_Click(object sender,EventArgs e) {
			//not implemented
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//not implemented
			//also delete ChildParent relationships
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//not implemented
		}


	}
}