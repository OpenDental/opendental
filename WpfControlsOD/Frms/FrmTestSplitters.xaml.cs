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
	public partial class FrmTestSplitters:FrmODBase {
		private int _countLoop=60;
		private DataTable _table;

		///<summary></summary>
		public FrmTestSplitters(){
			InitializeComponent();
			Load+=FrmFrmTestSplitters_Load;
		}

		private void FrmFrmTestSplitters_Load(object sender,EventArgs e) {
			webBrowser.Source=new Uri("https://www.opendental.com/");
		}

	}
}
