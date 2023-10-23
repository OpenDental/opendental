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
	public partial class FrmTestCombos:FrmODBase {

		///<summary></summary>
		public FrmTestCombos(){
			InitializeComponent();
			//comboClinic.IsTestModeNoDb=true;
			//comboClinic.IncludeHiddenInAll=true;
			//comboBox.IncludeAll=true;
			Load+=FrmFrmTestCombos_Load;
		}

		private void FrmFrmTestCombos_Load(object sender,EventArgs e) {
			//for(int i=1;i<40;i++){
			//	Clinic clinic=new Clinic();
			//	clinic.ClinicNum=i;
			//	clinic.Description="Clinic"+i.ToString();
			//	clinic.Abbr="Clinic"+i.ToString();
			//	comboBox.Items.Add(clinic.Description,clinic);
			//}
			//comboClinic.ClinicNumSelected=50;
		}

		private void butGetValues_Click(object sender,EventArgs e) {
			//List<long> listClinicNums=comboClinic.ListClinicNumsSelected;

		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textVDate.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			MsgBox.Show(textVDate.Value.ToString());
		}

		private void butGetValues_Copy_Click(object sender,EventArgs e) {

		}
	}
}
