using CodeBase;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ServiceManager {
	public partial class FormMain:Form {
		public FormMain() {
			InitializeComponent();
		}

		private void FormMain_Load(object sender,EventArgs e) {
			FillList(isLoad:true);
		}

		private void FillList(bool isLoad=false) {
			listMain.Items.Clear();
			List<ServiceController> listServicesAll=ServicesHelper.GetAllOpenDentServices();
			try {
				listServicesAll.AddRange(ServicesHelper.GetServicesByExe("mysqld.exe"));
			}
			catch(Exception ex) {
				ex.DoNothing();
				if(isLoad) {
					MessageBox.Show("Error loading 'mysqld.exe' Window services.\r\n"+ex.Message);
				}
			}
			listServicesAll.ForEach(x => listMain.Items.Add(x.ServiceName));
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
