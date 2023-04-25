using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormClaimCustomTrackingUpdate:FormODBase {
		/// <summary>List of claims selected from outstanding claim report</summary>
		private List<Claim> _listClaims;
		///<summary>When creating a new claimTracking this list contains 1 for every claim in _listClaims. Otherwise null</summary>
		public List<ClaimTracking> ListClaimTrackingsNew;

		///<summary>Used when creating a brand new claimcustomtracking.</summary>
		public FormClaimCustomTrackingUpdate(List<Claim> listClaims) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listClaims=listClaims;
			ListClaimTrackingsNew=new List<ClaimTracking>(new ClaimTracking[_listClaims.Count]);
		}

		///<summary>Used when creating a brand new claimcustomtracking.</summary>
		public FormClaimCustomTrackingUpdate(Claim claimCur,string noteText) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listClaims=new List<Claim>() {claimCur};
			ListClaimTrackingsNew=new List<ClaimTracking>(new ClaimTracking[_listClaims.Count]);//Default to list of nulls.
			textNotes.Text=noteText;
		}

		///<summary>Used for editing a ClaimTracking object from FormClaimEdit.</summary>
		public FormClaimCustomTrackingUpdate(Claim claimCur,ClaimTracking claimTracking) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listClaims=new List<Claim>() { claimCur };
			ListClaimTrackingsNew=new List<ClaimTracking>() { claimTracking };
		}

		private void FormClaimCustomTrackingUpdate_Load(object sender,EventArgs e) {
			if(!PrefC.GetBool(PrefName.ClaimTrackingStatusExcludesNone)) {
				//None is allowed as an option
				comboCustomTracking.Items.AddDefNone();
			}
			List<Def> listDefsClaimCustomTracking=Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true);
			comboCustomTracking.Items.AddDefs(listDefsClaimCustomTracking);
			ClaimTracking claimTracking=ListClaimTrackingsNew.FirstOrDefault();
			if(claimTracking!=null){//Creating a new ClaimTracking
				comboCustomTracking.SetSelectedDefNum(claimTracking.TrackingDefNum);
				//An existing ClaimTracking could still have a TrackingDefNum of 0='None'.
				//Even if the new pref blocks "None" from showing.
				//In that case, the setter above, will show "None", but selectedIndex will be -1,
				//preventing user from saving down below.
			}
			textNotes.Text=claimTracking?.Note??"";
			FillComboErrorCode();
		}

		private void FillComboErrorCode() {
			Def[] defArrayErrorCodes = Defs.GetDefsForCategory(DefCat.ClaimErrorCode,true).ToArray();
			comboErrorCode.Items.Clear();
			//Add "none" option.
			comboErrorCode.Items.Add(Lan.g(this,"None"),new Def() {ItemValue="",DefNum=0});
			comboErrorCode.SelectedIndex=0;
			if(defArrayErrorCodes.Length==0) {
				//if the list is empty, then disable the comboBox.
				comboErrorCode.Enabled=false;
				return;
			}
			//Fill comboErrorCode.
			ClaimTracking claimTracking=ListClaimTrackingsNew.FirstOrDefault();
			for(int i=0;i<defArrayErrorCodes.Length;i++) {
				//hooray for using new ODBoxItems!
				comboErrorCode.Items.Add(defArrayErrorCodes[i].ItemName,defArrayErrorCodes[i]);
				if(claimTracking?.TrackingErrorDefNum==defArrayErrorCodes[i].DefNum) {
					comboErrorCode.SelectedIndex=i+1;//adding 1 to the index because we have added a 'none' option above
				}
			}
		}

		private void comboErrorCode_SelectionChangeCommitted(object sender,EventArgs e) {
			if((!comboErrorCode.Enabled) || comboErrorCode.GetSelected<Def>()==null) {
				textErrorDesc.Text="";
			}
			else {
				textErrorDesc.Text=comboErrorCode.GetSelected<Def>().ItemValue.ToString();
			}	
		}

		private void butUpdate_Click(object sender,EventArgs e) {
			if(comboCustomTracking.SelectedIndex==-1) {
				//Defaults to -1 when editing and old ClaimTracking where TrackingDefNum is 0 ('None') and ClaimTrackingStatusExcludesNone is true.
				MsgBox.Show(this,"You must specify a Custom Track Status.");
				return;
			}
			if(PrefC.GetBool(PrefName.ClaimTrackingRequiresError) 
				&& comboErrorCode.GetSelected<Def>()==null 
				&& comboErrorCode.Enabled)
			{
				MsgBox.Show(this,"You must specify an error code."); //Do they have to specify an error code even if they set the status to None?
				return;
			}
			if(comboCustomTracking.GetSelectedDefNum()==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Setting the status to none will disable filtering in the Outstanding Claims Report."
						+"  Do you wish to set the status of this claim to none?")) {
					return;
				}
			}
			Def defErrorCode=comboErrorCode.GetSelected<Def>();
			for(int i=0;i<_listClaims.Count;i++) { //when not called from FormRpOutstandingIns, this should only have one claim.
				_listClaims[i].CustomTracking=comboCustomTracking.GetSelectedDefNum();
				Claims.Update(_listClaims[i]);
				ClaimTracking claimTrackingCur=ListClaimTrackingsNew[i];
				if(claimTrackingCur==null) {
					claimTrackingCur=new ClaimTracking();
					claimTrackingCur.ClaimNum=_listClaims[i].ClaimNum;
				}
				claimTrackingCur.Note=textNotes.Text;
				claimTrackingCur.TrackingDefNum=comboCustomTracking.GetSelectedDefNum();
				claimTrackingCur.TrackingErrorDefNum=defErrorCode.DefNum;
				claimTrackingCur.UserNum=Security.CurUser.UserNum;
				if(claimTrackingCur.ClaimTrackingNum==0) { //new claim tracking status.
					ClaimTrackings.Insert(claimTrackingCur);
				}
				else { //existing claim tracking status
					ClaimTrackings.Update(claimTrackingCur);
				}
				ListClaimTrackingsNew[i]=claimTrackingCur;//Update list.
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}