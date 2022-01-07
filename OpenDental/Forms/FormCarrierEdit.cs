using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormCarrierEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Carrier CarrierCur;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormCarrierEditCanada";
			}
			return "FormCarrierEdit";
		}

		///<summary></summary>
		public FormCarrierEdit(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCarrierEdit_Load(object sender,System.EventArgs e) {
			if(CarrierCur.CarrierNum!=0) {
				textCarrierNum.Text=CarrierCur.CarrierNum.ToString();
			}
			textCarrierName.Text=CarrierCur.CarrierName;
			textPhone.Text=CarrierCur.Phone;
			textAddress.Text=CarrierCur.Address;
			textAddress2.Text=CarrierCur.Address2;
			textCity.Text=CarrierCur.City;
			textState.Text=CarrierCur.State;
			textZip.Text=CarrierCur.Zip;
			textElectID.Text=CarrierCur.ElectID;
			List<Def> listDefsCarrierGroupNames=Defs.GetDefsForCategory(DefCat.CarrierGroupNames,true);//Only Add non hidden definitions
				//new List<Def> { new Def() { DefNum=0,ItemName=Lan.g(this,"Unspecified") } };
			//listDefsCarrierGroupNames.AddRange(Defs.GetDefsForCategory(DefCat.CarrierGroupNames,true));
			if(listDefsCarrierGroupNames.Count>0) {//only show if at least one CarrierGroupName definition
				labelCarrierGroupName.Visible=true;
				comboCarrierGroupName.Visible=true;
				comboCarrierGroupName.Items.Add(Lan.g(this,"Unspecified"),new Def());//defNum 0
				comboCarrierGroupName.Items.AddDefs(listDefsCarrierGroupNames);
				comboCarrierGroupName.SetSelectedDefNum(CarrierCur.CarrierGroupName);
			}
			comboCobSendPaidByInsAt.Items.AddEnums<EclaimCobInsPaidBehavior>();
			comboCobSendPaidByInsAt.SetSelectedEnum(CarrierCur.CobInsPaidBehaviorOverride);
			comboSendElectronically.Items.AddEnums<NoSendElectType>();
			comboSendElectronically.SetSelectedEnum(CarrierCur.NoSendElect);
			comboEraAutomation.Items.AddEnums<EraAutomationMode>();
			comboEraAutomation.SetSelectedEnum(CarrierCur.EraAutomationOverride);
			string strOrthoConsolidatePref=$" ({(PrefC.GetBool(PrefName.OrthoInsPayConsolidated) ? "On" : "Off")})";
			comboOrthoConsolidate.Items.Add(Lan.g("enumOrthoInsPayConsolidate",EnumOrthoInsPayConsolidate.Global.GetDescription()+strOrthoConsolidatePref),
				EnumOrthoInsPayConsolidate.Global);
			comboOrthoConsolidate.Items.Add(Lan.g("enumOrthoInsPayConsolidate",EnumOrthoInsPayConsolidate.ForceConsolidateOn.GetDescription()),
				EnumOrthoInsPayConsolidate.ForceConsolidateOn);
			comboOrthoConsolidate.Items.Add(Lan.g("enumOrthoInsPayConsolidate",EnumOrthoInsPayConsolidate.ForceConsolidateOff.GetDescription()),
				EnumOrthoInsPayConsolidate.ForceConsolidateOff);
			comboOrthoConsolidate.SetSelectedEnum(CarrierCur.OrthoInsPayConsolidate);
			checkIsHidden.Checked=CarrierCur.IsHidden;
			checkRealTimeEligibility.Checked=CarrierCur.TrustedEtransFlags.HasFlag(TrustedEtransTypes.RealTimeEligibility);
			radioBenefitSendsPat.Checked=(!CarrierCur.IsCoinsuranceInverted);//Default behaviour.
			radioBenefitSendsIns.Checked=(CarrierCur.IsCoinsuranceInverted);
			List<string> listDependentPlans=Carriers.DependentPlans(CarrierCur);
			textPlans.Text=listDependentPlans.Count.ToString();
			comboPlans.Items.Clear();
			for(int i=0;i<listDependentPlans.Count;i++){
				comboPlans.Items.Add(listDependentPlans[i]);
			}
			if(listDependentPlans.Count>0){
				comboPlans.SelectedIndex=0;
			}
			//textTemplates.Text=Carriers.DependentTemplates().ToString();
			checkIsCDAnet.Checked=CarrierCur.IsCDA;//Can be checked but not visible.
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelCitySt.Text="City,Province,PostalCode";
				labelElectID.Text="Carrier Identification Number";
				groupCDAnet.Visible=checkIsCDAnet.Checked;
			}
			else{//everyone but Canada
				checkIsCDAnet.Visible=false;
				groupCDAnet.Visible=false;
				int newHeight=(this.Height-groupCDAnet.Height-checkIsCDAnet.Height);//Dynamically hide the CDAnet groupbox and Is CDAnet Carrier checkbox.
				this.Size=new Size(525,newHeight);
			}
			//Canadian stuff is filled in for everyone, because a Canadian user might sometimes have a computer set to American.
			//So a computer set to American would not be able to SEE the Canadian fields, but they at least would not be damaged.
			comboNetwork.Items.Add(Lan.g(this,"none"));
			comboNetwork.SelectedIndex=0;
			List<CanadianNetwork> listCanadianNetworks=CanadianNetworks.GetDeepCopy();
			for(int i=0;i<listCanadianNetworks.Count;i++) {
				comboNetwork.Items.Add(listCanadianNetworks[i].Abbrev+" - "+listCanadianNetworks[i].Descript);
				if(CarrierCur.CanadianNetworkNum==listCanadianNetworks[i].CanadianNetworkNum) {
					comboNetwork.SelectedIndex=i+1;
				}
			}
			textVersion.Text=CarrierCur.CDAnetVersion;
			textEncryptionMethod.Text=CarrierCur.CanadianEncryptionMethod.ToString();
			check08.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.EligibilityTransaction_08) == CanSupTransTypes.EligibilityTransaction_08);
			check07.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.CobClaimTransaction_07) == CanSupTransTypes.CobClaimTransaction_07);
			check02.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.ClaimReversal_02) == CanSupTransTypes.ClaimReversal_02);
			check03.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.PredeterminationSinglePage_03) == CanSupTransTypes.PredeterminationSinglePage_03);
			check03m.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.PredeterminationMultiPage_03) == CanSupTransTypes.PredeterminationMultiPage_03);
			check04.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.RequestForOutstandingTrans_04) == CanSupTransTypes.RequestForOutstandingTrans_04);
			check05.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.RequestForSummaryReconciliation_05) == CanSupTransTypes.RequestForSummaryReconciliation_05);
			check06.Checked=((CarrierCur.CanadianSupportedTypes & CanSupTransTypes.RequestForPaymentReconciliation_06) == CanSupTransTypes.RequestForPaymentReconciliation_06);
			odColorPickerBack.AllowTransparentColor=true;
			if(CarrierCur.IsNew) {
				odColorPickerBack.BackgroundColor=Color.Black;//Black means no color
			}
			else {
				odColorPickerBack.BackgroundColor=CarrierCur.ApptTextBackColor;
			}
		}

		private void textCarrierName_TextChanged(object sender, System.EventArgs e) {
			if(textCarrierName.Text.Length==1){
				textCarrierName.Text=textCarrierName.Text.ToUpper();
				textCarrierName.SelectionStart=1;
			}
		}

		private void textAddress_TextChanged(object sender, System.EventArgs e) {
			if(textAddress.Text.Length==1){
				textAddress.Text=textAddress.Text.ToUpper();
				textAddress.SelectionStart=1;
			}
		}

		private void textAddress2_TextChanged(object sender, System.EventArgs e) {
			if(textAddress2.Text.Length==1){
				textAddress2.Text=textAddress2.Text.ToUpper();
				textAddress2.SelectionStart=1;
			}
		}

		private void textCity_TextChanged(object sender, System.EventArgs e) {
			if(textCity.Text.Length==1){
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender, System.EventArgs e) {
			int cursor=textState.SelectionStart;
			//for all countries, capitalize the first letter
			if(textState.Text.Length==1){
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=cursor;
				return;
			}
			//for US and Canada, capitalize second letter as well.
			if(CultureInfo.CurrentCulture.Name=="en-US"
				|| CultureInfo.CurrentCulture.Name=="en-CA"){
				if(textState.Text.Length==2){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=cursor;
				}
			}
		}

		private void checkIsCDAnet_Click(object sender,EventArgs e) {
			groupCDAnet.Visible=checkIsCDAnet.Checked;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Delete Carrier?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			try{
				Carriers.Delete(CarrierCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textCarrierName.Text==""){
				MessageBox.Show(Lan.g(this,"Carrier Name cannot be blank."));
				return;
			}
			Carrier carrierOld =CarrierCur.Copy();
			CarrierCur.CarrierName=textCarrierName.Text;
			CarrierCur.Phone=textPhone.Text;
			CarrierCur.Address=textAddress.Text;
			CarrierCur.Address2=textAddress2.Text;
			CarrierCur.City=textCity.Text;
			CarrierCur.State=textState.Text;
			CarrierCur.Zip=textZip.Text;
			CarrierCur.ElectID=textElectID.Text;
			//The SelectedItem will be null if hidden. Don't change if the def selected is still hidden.
			//DefNum will be 0 if "Unspecified" is selected. 
			if(comboCarrierGroupName.GetSelectedDefNum()!=0) {
				CarrierCur.CarrierGroupName=comboCarrierGroupName.GetSelectedDefNum();
			}
			CarrierCur.CobInsPaidBehaviorOverride=comboCobSendPaidByInsAt.GetSelected<EclaimCobInsPaidBehavior>();
			CarrierCur.NoSendElect=comboSendElectronically.GetSelected<NoSendElectType>();
			CarrierCur.EraAutomationOverride=comboEraAutomation.GetSelected<EraAutomationMode>();
			CarrierCur.IsHidden=checkIsHidden.Checked;
			if(checkRealTimeEligibility.Checked) {
				CarrierCur.TrustedEtransFlags=TrustedEtransTypes.RealTimeEligibility;
			}
			else {
				CarrierCur.TrustedEtransFlags=TrustedEtransTypes.None;
			}
			CarrierCur.IsCDA=checkIsCDAnet.Checked;
			CarrierCur.ApptTextBackColor=odColorPickerBack.BackgroundColor;
			CarrierCur.IsCoinsuranceInverted=radioBenefitSendsIns.Checked;
			CarrierCur.OrthoInsPayConsolidate=comboOrthoConsolidate.GetSelected<EnumOrthoInsPayConsolidate>();
			if(IsNew){
				try{
					Carriers.Insert(CarrierCur);
					SecurityLogs.MakeLogEntry(Permissions.CarrierCreate,0,Lan.g(this,"Carrier ")+CarrierCur.CarrierName+Lan.g(this," manually created."));
				}
				catch(ApplicationException ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			else{
				try{
					Carriers.Update(CarrierCur,carrierOld);
					//If the carrier name has changed loop through all the insplans that use this carrier and make a securitylog entry.
					if(carrierOld.CarrierName!=CarrierCur.CarrierName) {
						SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeCarrierName,0,Lan.g(this,"Carrier name changed in Edit Carrier window from")+" "
							+carrierOld.CarrierName+" "+Lan.g(this,"to")+" "+CarrierCur.CarrierName);
					}
				}
				catch(ApplicationException ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















