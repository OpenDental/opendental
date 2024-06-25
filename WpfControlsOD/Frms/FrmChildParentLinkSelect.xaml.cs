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
	public partial class FrmChildParentLinkSelect:FrmODBase {
		///<summary>This value will be filled when the frm closes with IsDialogOK=true.</summary>
		public long ChildParentNumSelected;

		///<summary></summary>
		public FrmChildParentLinkSelect() {
			InitializeComponent();
			Load+=FrmChildParentLinkSelect_Load;
			listBoxChildParents.MouseDoubleClick+=listBoxChildParents_MouseDoubleClick;
		}

		private void FrmChildParentLinkSelect_Load(object sender,EventArgs e) {
			FillListBox();
		}

		private void FillListBox() {
			listBoxChildParents.Items.Clear();
			List<ChildParent> listChildParents=ChildParents.GetAll();
			listBoxChildParents.Items.AddList(listChildParents,x => x.FName+" "+x.LName);
		}

		private void listBoxChildParents_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			ChildParentNumSelected=listBoxChildParents.GetSelected<ChildParent>().ChildParentNum;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxChildParents.SelectedIndex==-1) {
				MsgBox.Show("Please pick a parent first.");
				return;
			}
			ChildParentNumSelected=listBoxChildParents.GetSelected<ChildParent>().ChildParentNum;
			IsDialogOK=true;
		}
	}
}