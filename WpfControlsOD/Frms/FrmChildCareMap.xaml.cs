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
		}

		private void FillGridChildRoom1() {
			//Not implemented
			//Fill first with students
			//then teachers
			//final row is child to teacher ratio
		}

		private void FillGridChildRoom2() {
			//Not implemented
		}

		private void FillGridChildRoom3() {
			//Not implemented
		}

		private void FillGridChildRoom4() {
			//Not implemented
		}

		private void FillGridChildRoom5() {
			//Not implemented
		}

		private void FillGridChildRoom6() {
			//Not implemented
		}

		private void FillGridChildRoom7() {
			//Not implemented
		}

		private void FillGridChildRoom8() {
			//Not implemented
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