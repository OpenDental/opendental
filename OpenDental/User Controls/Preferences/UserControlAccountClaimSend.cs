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
	public partial class UserControlAccountClaimSend:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountClaimSend() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void butReplacements_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			FrmMessageReplacements frmMessageReplacements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplacements.IsSelectionMode=true;
			frmMessageReplacements.ShowDialog();
			if(frmMessageReplacements.IsDialogCancel) {
				return;
			}
			textClaimIdPrefix.Focus();
			int cursorIndex=textClaimIdPrefix.SelectionStart;
			textClaimIdPrefix.Text=textClaimIdPrefix.Text.Insert(cursorIndex,frmMessageReplacements.ReplacementTextSelected);
			textClaimIdPrefix.SelectionStart=cursorIndex+frmMessageReplacements.ReplacementTextSelected.Length;
		}

		#region Methods - Event Handlers Sync

		private void textClaimIdPrefix_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimIdPrefix);
			prefValSync.PrefVal=textClaimIdPrefix.Text;
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Method - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountClaimSend() {
			checkPpoUseUcr.Checked=PrefC.GetBool(PrefName.InsPpoAlwaysUseUcrFee);
			checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked=PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical);
			checkClaimFormTreatDentSaysSigOnFile.Checked=PrefC.GetBool(PrefName.ClaimFormTreatDentSaysSigOnFile);
			textInsWriteoffDescript.Text=PrefC.GetString(PrefName.InsWriteoffDescript);
			textClaimAttachPath.Text=PrefC.GetString(PrefName.ClaimAttachExportPath);
			checkEclaimsMedicalProvTreatmentAsOrdering.Checked=PrefC.GetBool(PrefName.ClaimMedProvTreatmentAsOrdering);
			checkEclaimsSeparateTreatProv.Checked=PrefC.GetBool(PrefName.EclaimsSeparateTreatProv);
			checkEclaimsSubscIDUsesPatID.Checked=PrefC.GetBool(PrefName.EclaimsSubscIDUsesPatID);
			checkClaimsValidateACN.Checked=PrefC.GetBool(PrefName.ClaimsValidateACN);
			//textClaimIdPrefix.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			foreach(ClaimZeroDollarProcBehavior procBehavior in Enum.GetValues(typeof(ClaimZeroDollarProcBehavior))) {
				comboZeroDollarProcClaimBehavior.Items.Add(Lan.g(this,procBehavior.ToString()));
			}
			comboZeroDollarProcClaimBehavior.SelectedIndex=PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior);
			checkPriClaimAllowSetToHoldUntilPriReceived.Checked=PrefC.GetBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived);
			checkCanadianPpoLabEst.Checked=PrefC.GetBool(PrefName.CanadaCreatePpoLabEst);
			checkClaimEditRequireNoMissingData.Checked=PrefC.GetBool(PrefName.ClaimEditRequireNoMissingData);
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				checkCanadianPpoLabEst.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				checkEclaimsSubscIDUsesPatID.Visible=false;
			}
		}

		public bool SaveAccountClaimSend() {
			Changed|=Prefs.UpdateBool(PrefName.InsPpoAlwaysUseUcrFee,checkPpoUseUcr.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimFormTreatDentSaysSigOnFile,checkClaimFormTreatDentSaysSigOnFile.Checked);
			Changed|=Prefs.UpdateString(PrefName.ClaimAttachExportPath,textClaimAttachPath.Text);
			Changed|=Prefs.UpdateBool(PrefName.EclaimsSeparateTreatProv,checkEclaimsSeparateTreatProv.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EclaimsSubscIDUsesPatID,checkEclaimsSubscIDUsesPatID.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimsValidateACN,checkClaimsValidateACN.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical,checkClaimMedTypeIsInstWhenInsPlanIsMedical.Checked);
			Changed|=Prefs.UpdateString(PrefName.InsWriteoffDescript,textInsWriteoffDescript.Text);
			Changed|=Prefs.UpdateBool(PrefName.ClaimMedProvTreatmentAsOrdering,checkEclaimsMedicalProvTreatmentAsOrdering.Checked);
			//Changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdPrefix.Text);
			Changed|=Prefs.UpdateInt(PrefName.ClaimZeroDollarProcBehavior,comboZeroDollarProcClaimBehavior.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.CanadaCreatePpoLabEst,checkCanadianPpoLabEst.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived,checkPriClaimAllowSetToHoldUntilPriReceived.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimEditRequireNoMissingData,checkClaimEditRequireNoMissingData.Checked);
			return true;
		}

		public void FillSynced() {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimIdPrefix);
			textClaimIdPrefix.Text=prefValSync.PrefVal;
		}
		#endregion Methods - Public
	}
}
