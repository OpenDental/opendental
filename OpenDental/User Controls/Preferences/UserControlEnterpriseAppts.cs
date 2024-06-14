using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlEnterpriseAppts:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlEnterpriseAppts() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers

		#region Methods - Event Handlers Sync

		private void checkApptsRequireProc_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptsRequireProc);	
			prefValSync.PrefVal=POut.Bool(checkApptsRequireProc.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkApptSecondaryProviderConsiderOpOnly_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptSecondaryProviderConsiderOpOnly);	
			prefValSync.PrefVal=POut.Bool(checkApptSecondaryProviderConsiderOpOnly.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillEnterpriseAppts() {
			//checkApptsRequireProc.Checked=PrefC.GetBool(PrefName.ApptsRequireProc);
			//checkApptSecondaryProviderConsiderOpOnly.Checked=PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly);
			checkEnterpriseApptList.Checked=PrefC.GetBool(PrefName.EnterpriseApptList);
			checkEnterpriseNoneApptViewDefaultDisabled.Checked=PrefC.GetBool(PrefName.EnterpriseNoneApptViewDefaultDisabled);
			checkEnterpriseHygProcUsePriProvFee.Checked=PrefC.GetBool(PrefName.EnterpriseHygProcUsePriProvFee);
		}

		public bool SaveEnterpriseAppts() {
			bool hasChangesViews=false;
			//Changed|=Prefs.UpdateBool(PrefName.ApptsRequireProc,checkApptsRequireProc.Checked);
			//Changed|=Prefs.UpdateBool(PrefName.ApptSecondaryProviderConsiderOpOnly,checkApptSecondaryProviderConsiderOpOnly.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EnterpriseApptList,checkEnterpriseApptList.Checked);
			if(Prefs.UpdateBool(PrefName.EnterpriseNoneApptViewDefaultDisabled,checkEnterpriseNoneApptViewDefaultDisabled.Checked)) {
				Changed=true;
				hasChangesViews=true;
			}
			Changed|=Prefs.UpdateBool(PrefName.EnterpriseHygProcUsePriProvFee,checkEnterpriseHygProcUsePriProvFee.Checked);
			if(Changed && hasChangesViews) {
				DataValid.SetInvalid(InvalidType.Views);
			}
			return true;
		}

		public void FillSynced() {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptsRequireProc);
			checkApptsRequireProc.Checked=PIn.Bool(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptSecondaryProviderConsiderOpOnly);
			checkApptSecondaryProviderConsiderOpOnly.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}
