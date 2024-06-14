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
	public partial class FrmChildren:FrmODBase {
		///<summary></summary>
		private List<Child> _listChilds;

		///<summary></summary>
		public FrmChildren() {
			InitializeComponent();
			Load+=FrmChildren_Load;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			//not implemented
		}

		private void gridChildren_CellDoubleClick(object sender,GridClickEventArgs e) {
			//not implemented
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//not implemented
		}

	
	}
}