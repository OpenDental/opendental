using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlExperimentalPrefs:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlExperimentalPrefs() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butAgingProcLifoDetails_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"3 state checkbox. When indeterminate (filled), it behaves as checked. Recommended is checked. Adjustment and payment plan credits attached to a " +
				"procedure are summed by date. If the sum of the attached charges and credits results in a credit, the credit is applied to the balance of the procedure's aging " +
				"category. Any remaining credit is aged normally. This is a LIFO strategy.");
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillExperimentalPrefs() {
			checkAgingProcLifo.CheckState=PrefC.GetYNCheckState(PrefName.AgingProcLifo);
		}

		public bool SaveExperimentalPrefs() {
			Changed|=Prefs.UpdateYN(PrefName.AgingProcLifo,checkAgingProcLifo.CheckState);
			return true;
		}
		#endregion Methods - Public
	}
}
