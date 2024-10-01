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
	public partial class FrmBadgeEdit:FrmODBase {
		///<summary></summary>
		public Userod UserodCur;

		///<summary></summary>
		public FrmBadgeEdit() {
			InitializeComponent();
			Load+=FrmBadgeEdit_Load;
		}

		private void FrmBadgeEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textUserName.Text=UserodCur.UserName;
			textBadgeID.Text=UserodCur.BadgeId;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			UserodCur.BadgeId=textBadgeID.Text;
			Userods.Update(UserodCur);
			SecurityLogs.MakeLogEntry(EnumPermType.BadgeIdEdit,0,"The BadgeId for "+UserodCur.UserName+" was edited.");
			IsDialogOK=true;
			DataValid.SetInvalid(InvalidType.Security);
		}

	}
}