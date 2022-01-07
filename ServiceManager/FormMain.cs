using CodeBase;
using System;
using System.Windows.Forms;

namespace ServiceManager {
	public partial class FormMain:Form {
		public FormMain() {
			InitializeComponent();
		}

		private void FormMain_Load(object sender,EventArgs e) {
			FillList();
		}

		private void FillList() {
			listMain.Items.Clear();
			ServicesHelper.GetAllOpenDentServices().ForEach(x => listMain.Items.Add(x.ServiceName));
		}

		private void listMain_DoubleClick(object sender,EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormServiceManage FormS=new FormServiceManage(listMain.SelectedItem.ToString(),false,false);
			FormS.ShowDialog();
			FillList();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormServiceManage FormS=new FormServiceManage("OpenDent",false,true);
			FormS.ShowDialog();
			FillList();
		}
	}
}
