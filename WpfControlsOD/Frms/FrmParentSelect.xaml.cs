using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;
using CodeBase;

namespace OpenDental {
	public partial class FrmParentSelect:FrmODBase {
		///<summary>This value will be filled when the frm closes with IsDialogOK=true.</summary>
		public long UserNumSelected;

		///<summary></summary>
		public FrmParentSelect() {
			InitializeComponent();
			Load+=FrmFamilyMemberSelect_Load;
		}

		private void FrmFamilyMemberSelect_Load(object sender, System.EventArgs e) {
			FillListBox();
		}

		private void FillListBox() {
			listUsers.Items.Clear();
			List<Userod> listUserods=Userods.GetAll();
			listUsers.Items.AddList(listUserods,x => x.UserName);
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(listUsers.SelectedIndex==-1) {
				MsgBox.Show("Please pick a user first.");
				return;
			}
			UserNumSelected=listUsers.GetSelected<Userod>().UserNum;
			IsDialogOK=true;
		}
	}
}