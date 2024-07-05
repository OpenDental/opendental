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
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildCareMap:FrmODBase {

		///<summary></summary>
		public FrmChildCareMap() {
			InitializeComponent();
			Load+=FrmChildren_Load;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			StartMaximized=true;
			//TODO: If not a daycare worker, hide buttons
			//call all fillgrids
			FillGridChildrenAbsent();
			FillGridSpecified(gridChildRoom1);
			FillGridSpecified(gridChildRoom2);
			FillGridSpecified(gridChildRoom3);
			FillGridSpecified(gridChildRoom4);
			FillGridSpecified(gridChildRoom5);
			FillGridSpecified(gridChildRoom6);
			FillGridSpecified(gridChildRoom7);
			FillGridSpecified(gridChildRoom8);
		}

		private void FillGridChildrenAbsent() {
			//Not implemented
		}

		private void FillGridSpecified(Grid grid) {
			//Not implemented
			//Fill teachers
			//Then students
			//Final row is child to teacher ratio
		}

		private void butChildren_Click(object sender,EventArgs e) {
			FrmChildren frmChildren=new FrmChildren();
			frmChildren.ShowDialog();
		}

		private void butClassrooms_Click(object sender,EventArgs e) {
			FrmChildRooms frmChildRooms=new FrmChildRooms();
			frmChildRooms.ShowDialog();
		}

		private void butParents_Click(object sender,EventArgs e) {
			FrmChildParents frmChildParents=new FrmChildParents();
			frmChildParents.ShowDialog();
		}


	}
}