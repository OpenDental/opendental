using System;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildParentEdit:FrmODBase {
		///<summary></summary>
		public ChildParent ChildParentCur;

		///<summary></summary>
		public FrmChildParentEdit() {
			InitializeComponent();
			Load+=FrmChildParentEdit_Load;
		}

		private void FrmChildParentEdit_Load(object sender,EventArgs e) {
			textFirstName.Text=ChildParentCur.FName;
			textLastName.Text=ChildParentCur.LName;
			checkHidden.Checked=ChildParentCur.IsHidden;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textFirstName.Text=="") {
				MsgBox.Show("This parent must have a first name.");
				return;
			}
			if(textLastName.Text=="") {
				MsgBox.Show("This parent must have a last name.");
				return;
			}
			ChildParentCur.FName=textFirstName.Text;
			ChildParentCur.LName=textLastName.Text;
			ChildParentCur.IsHidden=checkHidden.Checked==true;
			if(ChildParentCur.IsNew) {
				ChildParents.Insert(ChildParentCur);
			}
			else {
				ChildParents.Update(ChildParentCur);
			}
			IsDialogOK=true;
		}


	}
}