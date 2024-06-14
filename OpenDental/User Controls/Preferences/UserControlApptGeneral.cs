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
	public partial class UserControlApptGeneral:UserControl {

		#region Fields - Private
		private List<Def> _listDefsPosAdjTypes;
		private List<BrokenApptProcedure> _listBrokenApptProcedures=new List<BrokenApptProcedure>();
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlApptGeneral() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void checkAppointmentTimeIsLocked_MouseUp(object sender,MouseEventArgs e) {
			if(checkAppointmentTimeIsLocked.Checked) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to lock appointment times for all existing appointments?")){
					Appointments.SetAptTimeLocked();
				}
			}
		}

		private void checkApptsRequireProcs_CheckedChanged(object sender,EventArgs e) {
			textApptWithoutProcsDefaultLength.Enabled=(!checkApptsRequireProc.Checked);
		}

		#region Methods - Event Handlers Sync

		private void checkApptsRequireProc_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptsRequireProc);	
			prefValSync.PrefVal=POut.Bool(checkApptsRequireProc.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillApptGeneral() {
			BrokenApptProcedure brokenApptCodeDB=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			foreach(BrokenApptProcedure option in Enum.GetValues(typeof(BrokenApptProcedure))) {
				if(option==BrokenApptProcedure.Missed && !ProcedureCodes.HasMissedCode()) {
					continue;
				}
				if(option==BrokenApptProcedure.Cancelled && !ProcedureCodes.HasCancelledCode()) {
					continue;
				}
				if(option==BrokenApptProcedure.Both && (!ProcedureCodes.HasMissedCode() || !ProcedureCodes.HasCancelledCode())) {
					continue;
				}
				_listBrokenApptProcedures.Add(option);
				int index=comboBrokenApptProc.Items.Add(Lans.g(this,option.ToString()));
				if(option==brokenApptCodeDB) {
					comboBrokenApptProc.SelectedIndex=index;
				}
			}
			if(comboBrokenApptProc.Items.Count==1) {//None
				comboBrokenApptProc.SelectedIndex=0;
				comboBrokenApptProc.Enabled=false;
			}
			checkBrokenApptAdjustment.Checked=PrefC.GetBool(PrefName.BrokenApptAdjustment);
			checkBrokenApptCommLog.Checked=PrefC.GetBool(PrefName.BrokenApptCommLog);
			_listDefsPosAdjTypes=Defs.GetPositiveAdjTypes();
			comboBrokenApptAdjType.Items.AddDefs(_listDefsPosAdjTypes);
			comboBrokenApptAdjType.SetSelectedDefNum(PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType));
			foreach(SearchBehaviorCriteria searchBehavior in Enum.GetValues(typeof(SearchBehaviorCriteria))) {
				comboSearchBehavior.Items.Add(Lan.g(this,searchBehavior.GetDescription()));
			}
			comboSearchBehavior.SelectedIndex=PrefC.GetInt(PrefName.AppointmentSearchBehavior);
			checkAppointmentTimeIsLocked.Checked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			checkApptTimeReset.Checked=PrefC.GetBool(PrefName.AppointmentClinicTimeReset);
			if(!PrefC.HasClinicsEnabled) {
				checkApptTimeReset.Visible=false;
			}
			checkApptModuleAdjInProd.Checked=PrefC.GetBool(PrefName.ApptModuleAdjustmentsInProd);
			checkApptModuleProductionUsesOps.Checked=PrefC.GetBool(PrefName.ApptModuleProductionUsesOps);
			//checkApptsRequireProc.Checked=PrefC.GetBool(PrefName.ApptsRequireProc);
			checkApptAllowFutureComplete.Checked=PrefC.GetBool(PrefName.ApptAllowFutureComplete);
			checkApptAllowEmptyComplete.Checked=PrefC.GetBool(PrefName.ApptAllowEmptyComplete);
			textApptWithoutProcsDefaultLength.Text=PrefC.GetString(PrefName.AppointmentWithoutProcsDefaultLength);
			checkUnscheduledListNoRecalls.Checked=PrefC.GetBool(PrefName.UnscheduledListNoRecalls);
			textApptAutoRefreshRange.Text=PrefC.GetString(PrefName.ApptAutoRefreshRange);
			checkPreventChangesToComplAppts.Checked=PrefC.GetBool(PrefName.ApptPreventChangesToCompleted);
			checkApptsAllowOverlap.Checked=PrefC.GetBool(PrefName.ApptsAllowOverlap);
			checkBrokenApptRequiredOnMove.Checked=PrefC.GetBool(PrefName.BrokenApptRequiredOnMove);
		}

		public bool SaveApptGeneral() {
			if(!textApptWithoutProcsDefaultLength.IsValid()
				| !textApptAutoRefreshRange.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.BrokenApptAdjustment,checkBrokenApptAdjustment.Checked);
			Changed|=Prefs.UpdateBool(PrefName.BrokenApptCommLog,checkBrokenApptCommLog.Checked);
			Changed|=Prefs.UpdateInt(PrefName.BrokenApptProcedure,(int)_listBrokenApptProcedures[comboBrokenApptProc.SelectedIndex]);
			Changed|=Prefs.UpdateInt(PrefName.AppointmentSearchBehavior,comboSearchBehavior.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.AppointmentTimeIsLocked,checkAppointmentTimeIsLocked.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AppointmentClinicTimeReset,checkApptTimeReset.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptModuleAdjustmentsInProd,checkApptModuleAdjInProd.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptModuleProductionUsesOps,checkApptModuleProductionUsesOps.Checked);
			//Changed|=Prefs.UpdateBool(PrefName.ApptsRequireProc,checkApptsRequireProc.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptAllowFutureComplete,checkApptAllowFutureComplete.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptAllowEmptyComplete,checkApptAllowEmptyComplete.Checked);
			Changed|=Prefs.UpdateString(PrefName.AppointmentWithoutProcsDefaultLength,textApptWithoutProcsDefaultLength.Text);
			Changed|=Prefs.UpdateBool(PrefName.UnscheduledListNoRecalls,checkUnscheduledListNoRecalls.Checked);
			Changed|=Prefs.UpdateString(PrefName.ApptAutoRefreshRange,textApptAutoRefreshRange.Text);
			Changed|=Prefs.UpdateBool(PrefName.ApptPreventChangesToCompleted,checkPreventChangesToComplAppts.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptsAllowOverlap,checkApptsAllowOverlap.Checked);
			Changed|=Prefs.UpdateBool(PrefName.BrokenApptRequiredOnMove,checkBrokenApptRequiredOnMove.Checked);
			if(comboBrokenApptAdjType.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.BrokenAppointmentAdjustmentType,comboBrokenApptAdjType.GetSelectedDefNum());
			}
			return true;
		}

		public void FillSynced() {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptsRequireProc);
			checkApptsRequireProc.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}
