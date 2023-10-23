using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormProcCodes:FormODBase {
		///<summary>If IsSelectionMode=true and DialogResult=OK, then this will contain the selected CodeNum.</summary>
		public long CodeNumSelected;
		//public string SelectedADA;
		///<summary>This is just set once for the whole session.  It doesn't get toggled back to false.  So it only get acted upon when the form closes, sending a signal to other computers telling them to refresh their proc codes.</summary>
		private bool _doSetInvalidProcCodes;
		///<summary>Once a synch is done, this will switch back to false.  This also triggers SaveLogs, which then clears _listSecurityLogs to prepare for more logs.</summary>
		private bool _needsSynch;
		///<summary>Set to true externally in order to let user select one procedure code.</summary>
		public bool IsSelectionMode;
		///<summary>The list of definitions that is currently showing in the category list.</summary>
		private Def[] _defArrayCat;
		private List<FeeSched> _listFeeScheds; //Note to reviewer: I'm doing these to avoid using calls like FeeSchedC.ListShort[idx] later.
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private Color _colorClinic;
		private Color _colorProv;
		private Color _colorProvClinic;
		private Color _colorDefault;
		//<summary>Local copy of a FeeCache class that contains all fees, stored in memory for easy access and editing.  Synced on form closing.</summary>
		//private FeeCache _feeCache;
		///<summary>List of all fees for all three selected fee schedules.  This includes all clinic and provider overrides, regardless of selected clinic and provider.  We could do three separate lists, but that doesn't save us much.  And it's common to use all three columns with the same feeschedule, which would make synching separate lists difficult. Gets synched to db every time selected feescheds change.  This keeps it snappy when entering a series of fees because there is no db write.</summary>
		private List<Fee> _listFees;
		///<summary>The orginal list of fees used for synch.</summary>
		private List<Fee> _listFeesDb;
		private ProcCodeListSort _procCodeListSorts;
		/// <summary> List should contain two logs per fee because we are inserting two security logs every time we update a fee.</summary>
		private List<SecurityLog> _listSecurityLogs;
		private bool _canShowHidden;
		///<summary>Contains all of the procedure codes that were selected if IsSelectionMode is true.
		///If IsSelectionMode is true and this list is prefilled with procedure codes then the grid will preselect as many codes as possible.
		///It is not guaranteed that all procedure codes will be selected due to filters.
		///This list should only be read from externally after DialogResult.OK has been returned.</summary>
		public List<ProcedureCode> ListProcedureCodesSelected=new List<ProcedureCode>();
		///<summary>Set to true when IsSelectionMode is true and the user should be able to select multiple procedure codes instead of just one.
		///ListSelectedProcCodes will contain all of the procedure codes that the user selected.</summary>
		public bool CanAllowMultipleSelections;

		///<summary>When canShowHidden is true to the "Hidden" checkbox and "default" button are visible.</summary>
		public FormProcCodes(bool canShowHidden=false) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_canShowHidden=canShowHidden;
		}
		
		private void FormProcCodes_Load(object sender,System.EventArgs e) {
			_listSecurityLogs=new List<SecurityLog>();
			if(!Security.IsAuthorized(EnumPermType.Setup,DateTime.MinValue,true)) {
				butEditFeeSched.Visible=false;
				butTools.Visible=false;
			}
			if(!Security.IsAuthorized(EnumPermType.DefEdit,true)) {
				butEditCategories.Visible=false;
			}
			if(!Security.IsAuthorized(EnumPermType.ProcCodeEdit,true)) {
				groupProcCodeSetup.Visible=false;
			}
			if(!IsSelectionMode) {
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
			}
			else if(CanAllowMultipleSelections) {
				//Allow the user to select multiple rows by changing the grid selection mode.
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			if(_canShowHidden) {
				checkShowHidden.Checked=PrefC.GetBool(PrefName.ProcCodeListShowHidden);
			}
			else {//checkShowHidden will always be unchecked.
				checkShowHidden.Visible=false;
				butShowHiddenDefault.Visible=false;
			}
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			FillCats();
			listCategories.SetAll(true);
			_listClinics=Clinics.GetForUserod(Security.CurUser);
			_listProviders=Providers.GetDeepCopy(true);
			_colorProv=Defs.GetColor(DefCat.FeeColors,Defs.GetByExactName(DefCat.FeeColors,"Provider"));
			_colorProvClinic=Defs.GetColor(DefCat.FeeColors,Defs.GetByExactName(DefCat.FeeColors,"Provider and Clinic"));
			_colorClinic=Defs.GetColor(DefCat.FeeColors,Defs.GetByExactName(DefCat.FeeColors,"Clinic"));
			_colorDefault=Defs.GetColor(DefCat.FeeColors,Defs.GetByExactName(DefCat.FeeColors,"Default"));
			butColorProvider.BackColor=_colorProv;
			butColorClinicProv.BackColor=_colorProvClinic;
			butColorClinic.BackColor=_colorClinic;
			butColorDefault.BackColor=_colorDefault;
			labelSched1.ForeColor=_colorDefault;
			labelSched2.ForeColor=_colorDefault;
			labelSched3.ForeColor=_colorDefault;
			labelClinic1.ForeColor=_colorClinic;
			labelClinic2.ForeColor=_colorClinic;
			labelClinic3.ForeColor=_colorClinic;
			labelProvider1.ForeColor=_colorProv;
			labelProvider2.ForeColor=_colorProv;
			labelProvider3.ForeColor=_colorProv;
			bool isShowingGroups=PrefC.GetBool(PrefName.ShowFeeSchedGroups);
			checkGroups1.Visible=isShowingGroups;
			checkGroups2.Visible=isShowingGroups;
			checkGroups3.Visible=isShowingGroups;
			LayoutManager.MoveLocation(comboFeeSchedGroup1,new Point(14,96));
			LayoutManager.MoveLocation(comboFeeSchedGroup2,new Point(14,96));
			LayoutManager.MoveLocation(comboFeeSchedGroup3,new Point(14,96));
			FillComboBoxes();
			SynchAndFillListFees(false);
			_needsSynch=false;
			for(int i=0;i<Enum.GetNames(typeof(ProcCodeListSort)).Length;i++) {
				comboSort.Items.Add(Enum.GetNames(typeof(ProcCodeListSort))[i]);
			}
			_procCodeListSorts=(ProcCodeListSort)PrefC.GetInt(PrefName.ProcCodeListSortOrder);
			comboSort.SelectedIndex=(int)_procCodeListSorts;
			FillGrid();
			//Preselect corresponding procedure codes once on load.  Do not do it within FillGrid().
			if(ListProcedureCodesSelected.Count==0) {
				return;
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(!ListProcedureCodesSelected.Any(x => x.CodeNum==((ProcedureCode)gridMain.ListGridRows[i].Tag).CodeNum)) {
					continue;
				}
				gridMain.SetSelected(i,true);
			}
		}

		///<summary>Called on Load and anytime the feeschedule list might have changed.  Also called when different fee schedule selected to handle global fee schedules and the enabling/disabling of prov and clinic selectors.  This does not make any calls to db, so should be fast.</summary>
		private void FillComboBoxes() {
			//js This was getting called slightly too often.  It was getting called on all 6 combo_SelectionChangeCommitted and all 3 butPickFeeSched_Click.  Curiously, it was not getting called on the 3 butPickClinic_Click events or butPickProvider_Click events, suggesting that it didn't need to be called from the other places, either.  I removed the places where it didn't seem needed.
			//Save combo box selected indexes prior to changing stuff.
			long feeSchedNum1Selected=0;//Default to the first 
			long feeSchedNum2Selected=0;//Default to none
			long feeSchedNum3Selected=0;//Default to none
			if(_listFeeScheds.Count > 0) {
				if(comboFeeSched1.GetSelectedKey<FeeSched>(x => x.FeeSchedNum)>0) {//comboFeeSched1 doesn't usually contain a 'None' option, but can if there are no FeeScheds in DB
					feeSchedNum1Selected=comboFeeSched1.GetSelected<FeeSched>().FeeSchedNum;
				}
				if(comboFeeSched2.SelectedIndex > 0) {
					feeSchedNum2Selected=comboFeeSched2.GetSelected<FeeSched>().FeeSchedNum;
				}
				if(comboFeeSched3.SelectedIndex > 0) {
					feeSchedNum3Selected=comboFeeSched3.GetSelected<FeeSched>().FeeSchedNum;
				}
			}
			//Always update _listFeeScheds to reflect any potential changes.
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			//Check if feschednums from above were set to hidden, if so set selected index to 0 for the combo
			if(feeSchedNum1Selected > 0 && !_listFeeScheds.Any(x => x.FeeSchedNum==feeSchedNum1Selected)) {
				comboFeeSched1.SelectedIndex=0;
				feeSchedNum1Selected=comboFeeSched1.GetSelected<FeeSched>().FeeSchedNum;
			}
			if(feeSchedNum2Selected > 0 && !_listFeeScheds.Any(x => x.FeeSchedNum==feeSchedNum2Selected)) {
				comboFeeSched2.SelectedIndex=0;
				feeSchedNum2Selected=0;
			}
			if(feeSchedNum3Selected > 0 && !_listFeeScheds.Any(x => x.FeeSchedNum==feeSchedNum3Selected)) {
				comboFeeSched3.SelectedIndex=0;
				feeSchedNum3Selected=0;
			}
			int comboProv1Idx=comboProvider1.SelectedIndex;
			int comboProv2Idx=comboProvider2.SelectedIndex;
			int comboProv3Idx=comboProvider3.SelectedIndex;
			long feeSchedGroupNum1Selected=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
			long feeSchedGroupNum2Selected=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
			long feeSchedGroupNum3Selected=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
			comboFeeSched1.Items.Clear();
			comboFeeSched2.Items.Clear();
			comboFeeSched3.Items.Clear();
			comboProvider1.Items.Clear();
			comboProvider2.Items.Clear();
			comboProvider3.Items.Clear();
			comboFeeSchedGroup1.Items.Clear();
			comboFeeSchedGroup2.Items.Clear();
			comboFeeSchedGroup3.Items.Clear();
			//Fill fee sched combo boxes (FeeSched 1 doesn't get the "None" option)
			if(_listFeeScheds.Count==0) {//No fee schedules in the database so set the first item to none.
				comboFeeSched1.Items.Add("None",new FeeSched());
			}
			comboFeeSched2.Items.Add("None",new FeeSched());
			comboFeeSched3.Items.Add("None",new FeeSched());
			comboFeeSched1.Items.AddList(_listFeeScheds,x => x.Description+(x.FeeSchedType==FeeScheduleType.Normal?"":" ("+x.FeeSchedType.ToString()+")"));
			comboFeeSched1.SetSelectedKey<FeeSched>(feeSchedNum1Selected,x => x.FeeSchedNum);
			comboFeeSched2.Items.AddList(_listFeeScheds,x => x.Description+(x.FeeSchedType==FeeScheduleType.Normal?"":" ("+x.FeeSchedType.ToString()+")"));
			comboFeeSched2.SetSelectedKey<FeeSched>(feeSchedNum2Selected,x => x.FeeSchedNum);
			comboFeeSched3.Items.AddList(_listFeeScheds,x => x.Description+(x.FeeSchedType==FeeScheduleType.Normal?"":" ("+x.FeeSchedType.ToString()+")"));
			comboFeeSched3.SetSelectedKey<FeeSched>(feeSchedNum3Selected,x => x.FeeSchedNum);
			comboFeeSched1.SelectedIndex=Math.Max(0,comboFeeSched1.SelectedIndex);
			//Fill clinic combo boxes
			//Add none even if clinics is turned off so that 0 is a valid index to select.
			string defaultClinicText=(PrefC.HasClinicsEnabled ? "Default" : "None");
			//The clinic pickers have different unassigned text when disabled
			comboClinic1.HqDescription=defaultClinicText;
			comboClinic2.HqDescription=defaultClinicText;
			comboClinic3.HqDescription=defaultClinicText;
			if(!PrefC.HasClinicsEnabled) {//No clinics
				//For UI reasons, leave the clinic combo boxes visible for users not using clinics and they will just say "none".
				comboClinic1.Enabled=false;
				comboClinic2.Enabled=false;
				comboClinic3.Enabled=false;
				//The clinic pickers need to remain visible even with clinics disabled
				comboClinic1.Visible=true;
				comboClinic2.Visible=true;
				comboClinic3.Visible=true;
				//The unassigned option needs to be forced shown, otherwise 0 will be present 
				comboClinic1.ForceShowUnassigned=true;
				comboClinic2.ForceShowUnassigned=true;
				comboClinic3.ForceShowUnassigned=true;
				butPickClinic1.Enabled=false;
				butPickClinic2.Enabled=false;
				butPickClinic3.Enabled=false;
			}
			else {
				if(PrefC.GetBool(PrefName.ShowFeeSchedGroups)
					&& (checkGroups1.Checked || checkGroups2.Checked || checkGroups3.Checked))
				{
					List<long> listFeeSchedNumsSelected=new List<long>() { feeSchedNum1Selected,feeSchedNum2Selected,feeSchedNum3Selected }.FindAll(x => x>0);
					List<FeeSchedGroup> listFeeSchedGroups=FeeSchedGroups.GetListFeeSchedGroups(listFeeSchedNumsSelected);
					List<long> listClinicNums=_listClinics.Select(x => x.ClinicNum).ToList();
					List<FeeSchedGroup> listFeeSchedGroups1=listFeeSchedGroups.FindAll(x => x.FeeSchedNum==feeSchedNum1Selected);
					List<FeeSchedGroup> listFeeSchedGroups2=listFeeSchedGroups.FindAll(x => x.FeeSchedNum==feeSchedNum2Selected);
					List<FeeSchedGroup> listFeeSchedGroups3=listFeeSchedGroups.FindAll(x => x.FeeSchedNum==feeSchedNum3Selected);
					for(int i = 0;i<listClinicNums.Count();i++) {
						if(feeSchedNum1Selected>0 
							&& checkGroups1.Checked 
							&& listFeeSchedGroups1.Count>0)
						{
							List<FeeSchedGroup> listFeeSchedGroups1Fees=listFeeSchedGroups1.FindAll(x => x.ListClinicNumsAll.Contains(listClinicNums[i]));
							for(int g=0;g<listFeeSchedGroups1Fees.Count;g++){
								AddFeeSchedGroupToComboBox(listFeeSchedGroups1Fees[g],comboFeeSchedGroup1,feeSchedGroupNum1Selected);
							}
						}
						if(feeSchedNum2Selected>0 
							&& checkGroups2.Checked 
							&& listFeeSchedGroups2.Count>0)
						{
							List<FeeSchedGroup> listFeeSchedGroups2Fees=listFeeSchedGroups2.FindAll(x => x.ListClinicNumsAll.Contains(listClinicNums[i]));
							for(int g=0;g<listFeeSchedGroups2Fees.Count;g++){
								AddFeeSchedGroupToComboBox(listFeeSchedGroups2Fees[g],comboFeeSchedGroup2,feeSchedGroupNum2Selected);
							}
						}
						if(feeSchedNum3Selected>0 
							&& checkGroups3.Checked 
							&& listFeeSchedGroups3.Count>0)
						{
							List<FeeSchedGroup> listFeeSchedGroups3Fees=listFeeSchedGroups3.FindAll(x => x.ListClinicNumsAll.Contains(listClinicNums[i]));
							for(int g=0;g<listFeeSchedGroups3Fees.Count;g++){
								AddFeeSchedGroupToComboBox(listFeeSchedGroups3Fees[g],comboFeeSchedGroup3,feeSchedGroupNum3Selected);
							}
						}
					}
				}
			}
			//Fill provider combo boxes
			string[] stringArrayProvAbbrs=new[] { Lan.g(this,"None") }.Concat(_listProviders.Select(x => x.Abbr)).ToArray();
			comboProvider1.Items.AddList(stringArrayProvAbbrs);
			comboProvider2.Items.AddList(stringArrayProvAbbrs);
			comboProvider3.Items.AddList(stringArrayProvAbbrs);
			comboProvider1.SelectedIndex=Math.Max(0,comboProv1Idx);
			comboProvider2.SelectedIndex=Math.Max(0,comboProv2Idx);
			comboProvider3.SelectedIndex=Math.Max(0,comboProv3Idx);
			//If previously selected FeeSched was global, and the newly selected FeeSched is NOT global, select OD's selected Clinic in the combo box.
			if(_listFeeScheds.Count > 0 && comboFeeSched1.GetSelected<FeeSched>()!=null && comboFeeSched1.GetSelected<FeeSched>().IsGlobal) {
				comboClinic1.Enabled=false;
				butPickClinic1.Enabled=false;
				comboClinic1.ClinicNumSelected=0;				
				comboProvider1.Enabled=false;
				butPickProv1.Enabled=false;
				comboProvider1.SelectedIndex=0;
				comboFeeSchedGroup1.Enabled=false;
			}
			else {//Newly selected FeeSched is NOT global
				if(PrefC.HasClinicsEnabled) {
					if(feeSchedNum1Selected==0 || comboClinic1.Enabled==false) {
						//Previously selected FeeSched WAS global or there was none selected previously, select OD's selected Clinic
						comboClinic1.ClinicNumSelected=Clinics.ClinicNum;
					}
					comboClinic1.Enabled=true;
					butPickClinic1.Enabled=true;
					comboFeeSchedGroup1.Enabled=true;
				}
				comboProvider1.Enabled=true;
				butPickProv1.Enabled=true;
			}
			if(comboFeeSched2.SelectedIndex==0 || (comboFeeSched2.GetSelected<FeeSched>()!=null && comboFeeSched2.GetSelected<FeeSched>().IsGlobal)) {
				comboClinic2.Enabled=false;
				butPickClinic2.Enabled=false;
				comboClinic2.ClinicNumSelected=0;
				comboProvider2.Enabled=false;
				butPickProv2.Enabled=false;
				comboProvider2.SelectedIndex=0;
				comboFeeSchedGroup2.Enabled=false;
			}
			else {//Newly selected FeeSched is NOT global
				if(PrefC.HasClinicsEnabled) {
					if(comboClinic2.Enabled==false) {
						//Previously selected FeeSched WAS global, select OD's selected Clinic
						comboClinic2.ClinicNumSelected=Clinics.ClinicNum;
					}
					comboClinic2.Enabled=true;
					butPickClinic2.Enabled=true;
					comboFeeSchedGroup2.Enabled=true;
				}
				comboProvider2.Enabled=true;
				butPickProv2.Enabled=true;
			}
			if(comboFeeSched3.SelectedIndex==0 || (comboFeeSched3.GetSelected<FeeSched>()!=null && comboFeeSched3.GetSelected<FeeSched>().IsGlobal)) {
				comboClinic3.Enabled=false;
				butPickClinic3.Enabled=false;
				comboClinic3.ClinicNumSelected=0;
				comboProvider3.Enabled=false;
				butPickProv3.Enabled=false;
				comboProvider3.SelectedIndex=0;
				comboFeeSchedGroup3.Enabled=false;
				return;
			}
			//Newly selected FeeSched is NOT global
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic3.Enabled==false) {//Previously selected FeeSched WAS global
					//Select OD's selected Clinic
					comboClinic3.ClinicNumSelected=Clinics.ClinicNum;
				}
				comboClinic3.Enabled=true;
				butPickClinic3.Enabled=true;
				comboFeeSchedGroup3.Enabled=true;
			}
			comboProvider3.Enabled=true;
			butPickProv3.Enabled=true;
		}

		private void AddFeeSchedGroupToComboBox(FeeSchedGroup feeSchedGroup,UI.ComboBox comboBoxFeeSchedGroup,long feeSchedGroupNumSelected) {
			if(feeSchedGroup==null || feeSchedGroup.ListClinicNumsAll.Count==0){ //skip if empty group
				return;
			}
			if(!feeSchedGroup.ListClinicNumsAll.Exists(x => _listClinics.Exists(y => y.ClinicNum==x))){ 
				//skip if user doesn't have access to any of the clinics in the group
				return;
			}
			if(comboBoxFeeSchedGroup.Items.GetAll<FeeSchedGroup>().Exists(x => x.FeeSchedGroupNum==feeSchedGroup.FeeSchedGroupNum)){
				return;
			}
			comboBoxFeeSchedGroup.Items.Add(feeSchedGroup.Description,feeSchedGroup);
			if(feeSchedGroup.FeeSchedGroupNum==feeSchedGroupNumSelected) {
				comboBoxFeeSchedGroup.SelectedIndex=comboBoxFeeSchedGroup.Items.Count-1;
			}
		}

		///<summary>The only reason we use this is to keep interface snappy when entering a series of fees in the grid.  So the moment the user stops doing that and switches to something else is when we take the time to synch the changes to the db and start over fresh the next time.  We also do that whenever the selected feesched is changed, when closing form, when double clicking a code, importing, etc.  Use this method liberally.  Run it first with includeSynch=true, then run it again after doing other things, just to make sure we have fresh data for the new situation.</summary>
		private void SynchAndFillListFees(bool doIncludeSynch){
			//first, synch the old list
			Cursor=Cursors.WaitCursor;
			if(doIncludeSynch && _listFeesDb!=null){
				Fees.SynchList(_listFees,_listFeesDb);
				_needsSynch=false;
			}
			//Then, fill a new list
			long feeSched1=0;
			if(comboFeeSched1.GetSelectedKey<FeeSched>(x => x.FeeSchedNum)>0) {//0 FeeSchedNum is "none"
				feeSched1=_listFeeScheds[comboFeeSched1.SelectedIndex].FeeSchedNum;
			}
			long feeSched2=0;
			if(comboFeeSched2.SelectedIndex>0){//0 idx is "none"
				feeSched2=_listFeeScheds[comboFeeSched2.SelectedIndex-1].FeeSchedNum;
			}
			long feeSched3=0;
			if(comboFeeSched3.SelectedIndex>0){
				feeSched3=_listFeeScheds[comboFeeSched3.SelectedIndex-1].FeeSchedNum;
			}
			long provider1Num=0;
			long provider2Num=0;
			long provider3Num=0;
			if(comboProvider1.SelectedIndex>0) {
				provider1Num=_listProviders[comboProvider1.SelectedIndex-1].ProvNum;
			}
			if(comboProvider2.SelectedIndex>0) {
				provider2Num=_listProviders[comboProvider2.SelectedIndex-1].ProvNum;
			}
			if(comboProvider3.SelectedIndex>0) {
				provider3Num=_listProviders[comboProvider3.SelectedIndex-1].ProvNum;
			}
			List<long> listClinicNums1=new List<long> { 0 };
			List<long> listClinicNums2=new List<long> { 0 };
			List<long> listClinicNums3=new List<long> { 0 };
			if(PrefC.HasClinicsEnabled) { //Clinics is on
				listClinicNums1=new List<long> { comboClinic1.ClinicNumSelected };
				listClinicNums2=new List<long> { comboClinic2.ClinicNumSelected };
				listClinicNums3=new List<long> { comboClinic3.ClinicNumSelected };
				if(PrefC.GetBool(PrefName.ShowFeeSchedGroups)) {
					//First groupbox
					if(comboFeeSchedGroup1.Items.Count>0 && checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						listClinicNums1=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
					}
					//Second groupbox
					if(comboFeeSchedGroup2.Items.Count>0 && checkGroups2.Checked && comboFeeSchedGroup2.SelectedIndex>-1) {
						listClinicNums2=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
					}
					//Third groupbox
					if(comboFeeSchedGroup3.Items.Count> 0 && checkGroups3.Checked && comboFeeSchedGroup3.SelectedIndex>-1) {
						listClinicNums3=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
					}
				}
			}
			_listFees=Fees.GetListForSchedsAndClinics(feeSched1,listClinicNums1,provider1Num,feeSched2,listClinicNums2,provider2Num,feeSched3,listClinicNums3,provider3Num);
			_listFeesDb=_listFees.Select(x => x.Copy()).ToList();
			SaveLogs();//two entires for each fee.  There could be a lot.
			Cursor=Cursors.Default;
		}

		private void SaveLogs() {
			for(int i=0;i<_listSecurityLogs.Count;i++) {
				SecurityLogs.MakeLogEntry(_listSecurityLogs[i]);
			}
			_listSecurityLogs.Clear();
		}

		private void FillCats() {
			List<long> listDefNums=new List<long>();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				listDefNums.Add(_defArrayCat[listCategories.SelectedIndices[i]].DefNum);
			}
			if(checkShowHidden.Checked) {
				_defArrayCat=Defs.GetDefsForCategory(DefCat.ProcCodeCats).ToArray();
			}
			else {
				_defArrayCat=Defs.GetDefsForCategory(DefCat.ProcCodeCats,true).ToArray();
			}
			listCategories.Items.Clear();
			for(int i=0;i<_defArrayCat.Length;i++) {
				listCategories.Items.Add(_defArrayCat[i].ItemName);
				if(!listDefNums.Contains(_defArrayCat[i].DefNum)) {
					continue;
				}
				listCategories.SetSelected(i);
			}
		}

		///<summary>FillGrid does not go to the db for fees.  That's done separately when new feescheds are selected.</summary>
		private void FillGrid() {
			if(_listFeeScheds.Count==0) {
				gridMain.BeginUpdate();
				gridMain.ListGridRows.Clear();
				gridMain.EndUpdate();
				MsgBox.Show(this,"You must have at least one fee schedule created.");
				return;
			}
			int scroll=gridMain.ScrollValue;
			List<Def> listDefsCat=new List<Def>();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				listDefsCat.Add(_defArrayCat[listCategories.SelectedIndices[i]]);
			}
			FeeSched feeSched1=_listFeeScheds[comboFeeSched1.SelectedIndex]; //First feesched will always have something selected.
			FeeSched feeSched2=null;
			if(comboFeeSched2.SelectedIndex>0) {
				feeSched2=_listFeeScheds[comboFeeSched2.SelectedIndex-1];
			} 
			FeeSched feeSched3=null;
			if(comboFeeSched3.SelectedIndex>0){
				feeSched3=_listFeeScheds[comboFeeSched3.SelectedIndex-1];
			}
			//Provider nums will be 0 for "None" value.
			long provider1Num=0;
			long provider2Num=0;
			long provider3Num=0;
			if(comboProvider1.SelectedIndex>0) {
				provider1Num=_listProviders[comboProvider1.SelectedIndex-1].ProvNum;
			}
			if(comboProvider2.SelectedIndex>0) {
				provider2Num=_listProviders[comboProvider2.SelectedIndex-1].ProvNum;
			}
			if(comboProvider3.SelectedIndex>0) {
				provider3Num=_listProviders[comboProvider3.SelectedIndex-1].ProvNum;
			}
			//Clinic nums will be 0 for "Default" or "Off" value.
			long clinic1Num=0;
			long clinic2Num=0;
			long clinic3Num=0;
			if(PrefC.HasClinicsEnabled) { //Clinics is on
				if(PrefC.GetBool(PrefName.ShowFeeSchedGroups)) {
					//First groupbox
					if(comboFeeSchedGroup1.Items.Count>0 && checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						clinic1Num=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic1Num=comboClinic1.ClinicNumSelected;
					}
					//Second groupbox
					if(comboFeeSchedGroup2.Items.Count>0 && checkGroups2.Checked && comboFeeSchedGroup2.SelectedIndex>-1) {
						clinic2Num=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic2Num=comboClinic2.ClinicNumSelected;
					}
					//Third groupbox
					if(comboFeeSchedGroup3.Items.Count>0 && checkGroups3.Checked && comboFeeSchedGroup3.SelectedIndex>-1) {
						clinic3Num=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic3Num=comboClinic3.ClinicNumSelected;
					}
				}
				else {
					clinic1Num=comboClinic1.ClinicNumSelected;
					clinic2Num=comboClinic2.ClinicNumSelected;
					clinic3Num=comboClinic3.ClinicNumSelected;
				}
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			//The order of these columns are important for gridMain_CellDoubleClick(), gridMain_CellLeave(), and GridMain_CellEnter()
			GridColumn col=new GridColumn(Lan.g("TableProcedures","Category"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Description"),206);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Abbr"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Code"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Fee 1",50,HorizontalAlignment.Right,true);
			gridMain.Columns.Add(col);
			col=new GridColumn("Fee 2",50,HorizontalAlignment.Right,true);
			gridMain.Columns.Add(col);
			col=new GridColumn("Fee 3",50,HorizontalAlignment.Right,true);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string searchAbbr=textAbbreviation.Text;
			string searchDesc=textDescription.Text;
			string searchCode=textCode.Text;
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			//Loop through the list of categories which are ordered by def.ItemOrder.
			if(_procCodeListSorts==ProcCodeListSort.Category) {
				for(int i=0;i<listDefsCat.Count;i++) {
					//Get all procedure codes that are part of the selected category.  Then order the list of procedures by ProcCodes.
					//Append the list of ordered procedures to the master list of procedures for the selected categories.
					//Appending the procedure codes in this fashion keeps them ordered correctly via the definitions ItemOrder.
					listProcedureCodes.AddRange(ProcedureCodes.GetWhereFromList(x => x.ProcCat==listDefsCat[i].DefNum)
						.OrderBy(x => x.ProcCode).ToList());
				}
			}
			else if(_procCodeListSorts==ProcCodeListSort.ProcCode) {
				for(int i = 0;i<listDefsCat.Count;i++) {
					listProcedureCodes.AddRange(ProcedureCodes.GetWhereFromList(x => x.ProcCat==listDefsCat[i].DefNum).ToList());
				}
				listProcedureCodes=listProcedureCodes.OrderBy(x => x.ProcCode).ToList();
			}
			//Remove any procedures that do not meet our filters.
			listProcedureCodes.RemoveAll(x => !x.AbbrDesc.ToLower().Contains(searchAbbr.ToLower()));
			listProcedureCodes.RemoveAll(x => !x.Descript.ToLower().Contains(searchDesc.ToLower()));
			listProcedureCodes.RemoveAll(x => !x.ProcCode.ToLower().Contains(searchCode.ToLower()));
			if(IsSelectionMode) {
				listProcedureCodes.RemoveAll(x => x.ProcCode==ProcedureCodes.GroupProcCode);
			}
			string lastCategoryName="";
			for(int i=0;i<listProcedureCodes.Count;i++) {
				row=new GridRow();
				row.Tag=listProcedureCodes[i];
				//Only show the category on the first procedure code in that category.
				string categoryName=Defs.GetName(DefCat.ProcCodeCats,listProcedureCodes[i].ProcCat);
				if(lastCategoryName!=categoryName && _procCodeListSorts==ProcCodeListSort.Category) {
					row.Cells.Add(categoryName);
					lastCategoryName=categoryName;
				}
				else {//This proc code is in the same category or we are not sorting by category.
					row.Cells.Add("");
				}
				row.Cells.Add(listProcedureCodes[i].Descript);
				row.Cells.Add(listProcedureCodes[i].AbbrDesc);
				row.Cells.Add(listProcedureCodes[i].ProcCode);
				Fee fee1=Fees.GetFee(listProcedureCodes[i].CodeNum,feeSched1.FeeSchedNum,clinic1Num,provider1Num,_listFees);
				Fee fee2=null;
				if(feeSched2!=null) {
					fee2=Fees.GetFee(listProcedureCodes[i].CodeNum,feeSched2.FeeSchedNum,clinic2Num,provider2Num,_listFees);
				}
				Fee fee3=null;
				if(feeSched3!=null) {
					fee3=Fees.GetFee(listProcedureCodes[i].CodeNum,feeSched3.FeeSchedNum,clinic3Num,provider3Num,_listFees);
				}
				if(fee1==null || fee1.Amount==-1) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(fee1.Amount.ToString("n"));
					row.Cells[row.Cells.Count-1].ColorText=GetColorForFee(fee1);
				}
				if(fee2==null || fee2.Amount==-1) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(fee2.Amount.ToString("n"));
					row.Cells[row.Cells.Count-1].ColorText=GetColorForFee(fee2);
				}
				if(fee3==null || fee3.Amount==-1) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(fee3.Amount.ToString("n"));
					row.Cells[row.Cells.Count-1].ColorText=GetColorForFee(fee3);
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=scroll;
		}

		private Color GetColorForFee(Fee fee) {
			if(fee.ClinicNum!=0 && fee.ProvNum!=0) {
				return _colorProvClinic;
			}
			if(fee.ClinicNum!=0) {
				return _colorClinic;
			}
			if(fee.ProvNum!=0) {
				return _colorProv;
			}
			return _colorDefault;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			if(IsSelectionMode) {
				CodeNumSelected=((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).CodeNum;
				ListProcedureCodesSelected.Clear();
				ListProcedureCodesSelected.Add(((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).Copy());
				DialogResult=DialogResult.OK;
				return;
			}
			//else not selecting a code
			if(!Security.IsAuthorized(EnumPermType.ProcCodeEdit)) {
				return;
			}
			if(e.Col>3) {
				//Do nothing. All columns > 3 are editable (You cannot double click).
				return;
			}
			//not on a fee: Edit code instead
			//changed=false;//We just updated the database and synced our cache, set changed to false.
			ProcedureCode procedureCode=(ProcedureCode)gridMain.ListGridRows[e.Row].Tag;
			Def defProcCat=Defs.GetDefsForCategory(DefCat.ProcCodeCats).FirstOrDefault(x => x.DefNum==procedureCode.ProcCat);
			using FormProcCodeEdit formProcCodeEdit=new FormProcCodeEdit(procedureCode);
			formProcCodeEdit.IsNew=false;
			formProcCodeEdit.ShowHiddenCategories=defProcCat.IsHidden;
			if(defProcCat==null) {
				formProcCodeEdit.ShowHiddenCategories=false;
			}
			formProcCodeEdit.ShowDialog();
			_doSetInvalidProcCodes=true;
			//The user could have edited a fee within the Procedure Code Edit window or within one of it's children so we need to refresh our cache.
			//Yes, it could have even changed if the user Canceled out of the Proc Code Edit window (e.g. use FormProcCodeEditMore.cs)
			SynchAndFillListFees(false);
			FillGrid();
		}

		private void GridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			Security.IsAuthorized(EnumPermType.FeeSchedEdit);//Show message if user does not have permission.
		}

		///<summary>Takes care of individual cell edits.  Calls FillGrid to refresh other columns using the same data.</summary>
		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			//This is where the real fee editing logic is.
			if(!Security.IsAuthorized(EnumPermType.FeeSchedEdit,true)) { //Don't do anything if they don't have permission.
				return;
			}
			//Logic only works for columns 4 to 6.
			long codeNum=((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).CodeNum;
			FeeSched feeSched=null;
			long provNum=0;
			long clinicNum=0;
			Fee fee=null;
			bool isEditingGroup=false;
			FeeSchedGroup feeSchedGroupSelected=null;
			if(e.Col==4) {
				feeSched=_listFeeScheds[comboFeeSched1.SelectedIndex];
				if(comboProvider1.SelectedIndex>0) {
					provNum=_listProviders[comboProvider1.SelectedIndex-1].ProvNum;
				}
				if(PrefC.HasClinicsEnabled) {
					if(checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						clinicNum=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
						feeSchedGroupSelected=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic1.ClinicNumSelected;
					}
				}
				fee=Fees.GetFee(codeNum,feeSched.FeeSchedNum,clinicNum,provNum,_listFees);
			}
			else if(e.Col==5) {
				if(comboFeeSched2.SelectedIndex==0) {//It's on the "none" option
					gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
					return;
				}
				feeSched=_listFeeScheds[comboFeeSched2.SelectedIndex-1];
				if(comboProvider2.SelectedIndex>0) {
					provNum=_listProviders[comboProvider2.SelectedIndex-1].ProvNum;
				}
				if(PrefC.HasClinicsEnabled) {
					if(checkGroups2.Checked && comboFeeSchedGroup2.SelectedIndex>-1) {
						clinicNum=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
						feeSchedGroupSelected=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic2.ClinicNumSelected;
					}
				}
				fee=Fees.GetFee(codeNum,feeSched.FeeSchedNum,clinicNum,provNum,_listFees);
			}
			else if(e.Col==6) {
				if(comboFeeSched3.SelectedIndex==0) {//It's on the "none" option
					gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
					return;
				}
				feeSched=_listFeeScheds[comboFeeSched3.SelectedIndex-1];
				if(comboProvider3.SelectedIndex>0) {
					provNum=_listProviders[comboProvider3.SelectedIndex-1].ProvNum;
				}
				if(PrefC.HasClinicsEnabled) {
					if(checkGroups3.Checked && comboFeeSchedGroup3.SelectedIndex>-1) {
						clinicNum=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
						feeSchedGroupSelected=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic3.ClinicNumSelected;
					}
				}
				fee=Fees.GetFee(codeNum,feeSched.FeeSchedNum,clinicNum,provNum,_listFees);
			}
			Fee feeOrig=fee?.Copy();
			//Fees set in the 3 ifs above.  If too slow, we will just put the fee into a Tag during FillGrid() and run FillGrid less
			//Fee fee=_feeCache.GetFee(codeNum,feeSched.FeeSchedNum,clinicNum,provNum);
			DateTime datePrevious=DateTime.MinValue;
			string feeAmtOld="";
			if(fee!=null){
				datePrevious = fee.SecDateTEdit;
				feeAmtOld=fee.Amount.ToString("n");
			}
			string feeAmtNewStr=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
			double feeAmtNew=0;
			//Attempt to parse the entered value for errors.
			if(feeAmtNewStr!="") {
				try {
					feeAmtNew=double.Parse(gridMain.ListGridRows[e.Row].Cells[e.Col].Text);
				}
				catch {
					gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtOld;
					MessageBox.Show(Lan.g(this,"Please fix data entry error first."));
					return;
				}
			}
			if(Fees.IsFeeAmtEqual(fee,feeAmtNewStr) || !FeeL.CanEditFee(feeSched,provNum,clinicNum)) {
				if(fee==null || fee.Amount==-1) {
					gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
				}
				else {
					gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtOld;
				}
				return;
			}
			//Can't use values from fee object as it could be null. Instead use values pulled from UI that are also used to set new fees below.
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && provNum==0 && !isEditingGroup) {//Ignore provider fees and don't block from editing a group.
				FeeSchedGroup feeSchedGroupForClinic=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(feeSchedGroupForClinic!=null) {
					//Duplicating if check from above to prevent us from accidentally hitting users with infinite popups. We want the same end result for both of the checks
					//but we need to do the group check 2nd.
					if(fee==null || fee.Amount==-1) {
						gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
					}
					else {
						gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtOld;
					}
					MsgBox.Show(Lans.g(this,"Fee Schedule")+": "+feeSched.Description+" "+Lans.g(this,"for Clinic")+": "+(_listClinics.FirstOrDefault(x => x.ClinicNum==clinicNum)?.Abbr??"")+" "
						+Lans.g(this,"is part of Fee Schedule Group")+": "+feeSchedGroupForClinic.Description+". "+Lans.g(this,"The fees must be edited at the group level."));
					return;
				}
			}
			if(feeAmtNewStr!="") {
				gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtNew.ToString("n"); //Fix the number formatting and display it.
			}
			if(feeSched.IsGlobal) { //Global fee schedules have only one fee so blindly insert/update the fee. There will be no more localized copy.
				if(fee==null) { //Fee doesn't exist, insert
					fee=new Fee();
					fee.FeeSched=feeSched.FeeSchedNum;
					fee.CodeNum=codeNum;
					fee.Amount=feeAmtNew;
					fee.ClinicNum=0;
					fee.ProvNum=0;
					_listFees.Add(fee);
				}
				else { //Fee does exist, update or delete.
					if(feeAmtNewStr=="") { //They want to delete the fee
						_listFees.Remove(fee);
					}
					else { //They want to update the fee
						fee.Amount=feeAmtNew;
					}
				}
			}
			else { //FeeSched isn't global.
				if(feeAmtNewStr=="") { //They want to delete the fee
					//NOTE: If they are deleting the HQ fee we insert a blank (-1) for the current settings.
					if((fee.ClinicNum==0 && fee.ProvNum==0)
						&& (clinicNum!=0 || provNum!=0))
					{ 
						//The best match found was the default fee which should never be deleted when editing a fee schedule for a clinic or provider.
						//In this specific scenario, we have to add a fee to the database for the selected clinic and/or provider with an amount of -1.
						fee=new Fee();
						fee.FeeSched=feeSched.FeeSchedNum;
						fee.CodeNum=codeNum;
						fee.Amount=-1.0;
						fee.ClinicNum=clinicNum;
						fee.ProvNum=provNum;
						_listFees.Add(fee);
						if(isEditingGroup
							&& feeSchedGroupSelected!=null
							&& !feeSchedGroupSelected.ListClinicNumsAll.IsNullOrEmpty()
							&& feeSchedGroupSelected.ListClinicNumsAll.Contains(clinicNum)
							&& feeSchedGroupSelected.ListClinicNumsAll.Any(x => x!=clinicNum))
						{
							for(int i=0;i<feeSchedGroupSelected.ListClinicNumsAll.Count;i++){
								if(feeSchedGroupSelected.ListClinicNumsAll[i]==clinicNum) {
									continue;
								}
								List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==feeSchedGroupSelected.ListClinicNumsAll[i] && x.ProvNum==provNum);
								if(listFees.IsNullOrEmpty()) {
									Fee feeAdd=new Fee();
									feeAdd.FeeSched=feeSched.FeeSchedNum;
									feeAdd.CodeNum=codeNum;
									feeAdd.Amount=-1.0;
									feeAdd.ClinicNum=feeSchedGroupSelected.ListClinicNumsAll[i];
									feeAdd.ProvNum=provNum;
									_listFees.Add(feeAdd);
									continue;
								}
								for(int j=0;j<listFees.Count;j++){
									listFees[j].Amount=-1.0;
								}
							}
						}
					}
					else {//They want to delete a fee for their current settings.
						_listFees.Remove(fee);
						if(isEditingGroup
							&& feeSchedGroupSelected!=null
							&& !feeSchedGroupSelected.ListClinicNumsAll.IsNullOrEmpty()
							&& feeSchedGroupSelected.ListClinicNumsAll.Contains(clinicNum)
							&& feeSchedGroupSelected.ListClinicNumsAll.Any(x => x!=clinicNum))
						{
							_listFees.RemoveAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && feeSchedGroupSelected.ListClinicNumsAll.Contains(x.ClinicNum) && x.ProvNum==provNum);
						}
					}
				}
				//The fee did not previously exist, or the fee found isn't for the currently set settings.
				else if(fee==null || fee.ClinicNum!=clinicNum || fee.ProvNum!=provNum) {
					fee=new Fee();
					fee.FeeSched=feeSched.FeeSchedNum;
					fee.CodeNum=codeNum;
					fee.Amount=feeAmtNew;
					fee.ClinicNum=clinicNum;
					fee.ProvNum=provNum;
					_listFees.Add(fee);
					if(isEditingGroup
						&& feeSchedGroupSelected!=null
						&& !feeSchedGroupSelected.ListClinicNumsAll.IsNullOrEmpty()
						&& feeSchedGroupSelected.ListClinicNumsAll.Contains(clinicNum)
						&& feeSchedGroupSelected.ListClinicNumsAll.Any(x => x!=clinicNum))
					{
						for(int i=0;i<feeSchedGroupSelected.ListClinicNumsAll.Count;i++){
							if(feeSchedGroupSelected.ListClinicNumsAll[i]==clinicNum) {
								continue;
							}
							List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==feeSchedGroupSelected.ListClinicNumsAll[i] && fee.ProvNum==provNum);
							if(listFees.IsNullOrEmpty()) {
								Fee feeAdd=new Fee();
								feeAdd.FeeSched=feeSched.FeeSchedNum;
								feeAdd.CodeNum=codeNum;
								feeAdd.Amount=feeAmtNew;
								feeAdd.ClinicNum=feeSchedGroupSelected.ListClinicNumsAll[i];
								feeAdd.ProvNum=provNum;
								_listFees.Add(feeAdd);
								continue;
							}
							for(int j=0;j<listFees.Count;j++){
								listFees[j].Amount=feeAmtNew;
							}
						}
					}
				}
				else { //Fee isn't null, is for our current clinic, is for the selected provider, and they don't want to delete it.  Just update.
					fee.Amount=feeAmtNew;
					if(isEditingGroup
						&& feeSchedGroupSelected!=null
						&& !feeSchedGroupSelected.ListClinicNumsAll.IsNullOrEmpty()
						&& feeSchedGroupSelected.ListClinicNumsAll.Contains(clinicNum)
						&& feeSchedGroupSelected.ListClinicNumsAll.Any(x => x!=clinicNum))
					{
						for(int i=0;i<feeSchedGroupSelected.ListClinicNumsAll.Count();i++){
							if(feeSchedGroupSelected.ListClinicNumsAll[i]==clinicNum) {
								continue;
							}
							List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==feeSchedGroupSelected.ListClinicNumsAll[i] && fee.ProvNum==provNum);
							if(listFees.IsNullOrEmpty()) {
								Fee feeAdd=new Fee();
								feeAdd.FeeSched=feeSched.FeeSchedNum;
								feeAdd.CodeNum=codeNum;
								feeAdd.Amount=feeAmtNew;
								feeAdd.ClinicNum=feeSchedGroupSelected.ListClinicNumsAll[i];
								feeAdd.ProvNum=provNum;
								_listFees.Add(feeAdd);
								continue;
							}
							for(int j=0;j<listFees.Count;j++){
								listFees[j].Amount=feeAmtNew;
							}
						}
					}
				}
			}
			SecurityLog securityLog=SecurityLogs.MakeLogEntryNoInsert(EnumPermType.ProcFeeEdit,0,"Procedure: "+ProcedureCodes.GetStringProcCode(fee.CodeNum)
				+", Fee: "+fee.Amount.ToString("c")
				+", Fee Schedule: "+FeeScheds.GetDescription(fee.FeeSched)
				+". Manual edit in grid from Procedure Codes list.",fee.CodeNum,LogSources.None);
			_listSecurityLogs.Add(securityLog);
			_listSecurityLogs.Add(SecurityLogs.MakeLogEntryNoInsert(EnumPermType.LogFeeEdit,0,"Fee changed",fee.FeeNum,LogSources.None,
				DateTPrevious:fee.SecDateTEdit));
			_needsSynch=true;
			FillGrid();
		}

		#region Search

		private void butAll_Click(object sender,EventArgs e) {
			listCategories.SetAll(true);
			FillGrid();
		}

		private void butEditCategories_Click(object sender,EventArgs e) {
			//won't even be visible if no permission
			List<long> listDefNums=new List<long>();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				listDefNums.Add(_defArrayCat[listCategories.SelectedIndices[i]].DefNum);
			}
			using FormDefinitions formDefinitions=new FormDefinitions(DefCat.ProcCodeCats);
			formDefinitions.ShowDialog();
			DataValid.SetInvalid(InvalidType.Defs);
			FillCats();
			for(int i=0;i<_defArrayCat.Length;i++) {
				if(listDefNums.Contains(_defArrayCat[i].DefNum)) {
					listCategories.SetSelected(i);
				}
			}
			//we need to move security log to within the definition window for more complete tracking
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Definitions");
			FillGrid();
		}

		private void textAbbreviation_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textCode_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void listCategories_MouseUp(object sender,MouseEventArgs e) {
			FillGrid();
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillCats();
			FillGrid();
		}

		private void butShowHiddenDefault_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Set the Show Hidden checkbox to default to current status?")){
				return;
			}
			Prefs.UpdateBool(PrefName.ProcCodeListShowHidden,checkShowHidden.Checked);
			MsgBox.Show(this,"Done");
		}

		#endregion

		#region Procedure Codes

		private void butEditFeeSched_Click(object sender,System.EventArgs e) {
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			//We are launching in edit mode thus we must check the FeeSchedEdit permission type.
			if(!Security.IsAuthorized(EnumPermType.FeeSchedEdit)) {
				return;
			}
			using FormFeeScheds formFeeScheds=new FormFeeScheds(false); //The Fee Scheds window can add or hide schedules.  It cannot delete schedules.
			formFeeScheds.ShowDialog();
			FillComboBoxes();
			//I don't think it would highlight a new sched, so refresh fees should not be needed.
			FillGrid();
		}

		private void butTools_Click(object sender,System.EventArgs e) {
			if(_listFeeScheds.Count==0) {
				MsgBox.Show(this,"At least one fee schedule is required before using Fee Tools.");
				return;
			}
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			long schedNum=_listFeeScheds[comboFeeSched1.SelectedIndex].FeeSchedNum;
			using FormFeeSchedTools formFeeSchedTools=new FormFeeSchedTools(schedNum,_listFeeScheds,_listProviders,_listClinics);
			formFeeSchedTools.ShowDialog();
			//Fees could have changed from within the FeeSchedTools window.  Refresh our local fees.
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)) {
				FillComboBoxes();//To show possible added fee schedule.
			}
			SynchAndFillListFees(false);
			FillGrid();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Fee Schedule Tools");
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(ProcedureCodes.GetCount()==0) {
				MsgBox.Show(this,"No procedurecodes are displayed for export.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Only the codes showing in this list will be exported.  Continue?")) {
				return;
			}
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				ProcedureCode procedureCode=(ProcedureCode)gridMain.ListGridRows[i].Tag;
				if(procedureCode.ProcCode=="") {
					continue;
				}
				procedureCode.ProvNumDefault=0;  //We do not want to export ProvNumDefault because the receiving DB will not have the same exact provNums.
				listProcedureCodes.Add(procedureCode);
			}
			string filename="ProcCodes.xml";
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),filename); 
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				using SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				saveFileDialog.FileName=filename;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveFileDialog.FileName;
			}
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(List<ProcedureCode>));
			TextWriter textWriter=new StreamWriter(filePath);
			xmlSerializer.Serialize(textWriter,listProcedureCodes);
			textWriter.Close();
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MsgBox.Show(this,"Exported");
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			int rowsInserted=0;
			try {
				rowsInserted=ImportProcCodes(openFileDialog.FileName,null,"");
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				FillGrid();
				return;
			}
			MessageBox.Show(Lan.g(this,"Procedure codes inserted")+": "+rowsInserted);
			DataValid.SetInvalid(InvalidType.Defs,InvalidType.ProcCodes);
			ProcedureCodes.RefreshCache();
			FillCats();
			SynchAndFillListFees(false);//just in case there is a new fee?
			FillGrid();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,Lan.g(this,"Imported Procedure Codes"));
		}

		///<summary>Can be called externally.  Surround with try catch.  Returns number of codes inserted. 
		///Supply path to file to import or a list of procedure codes, or an xml string.  Make sure to set the other two values blank or null.</summary>
		public static int ImportProcCodes(string path,List<ProcedureCode> listProcedureCodes,string xmlData) {
			if(listProcedureCodes==null) {
				listProcedureCodes=new List<ProcedureCode>();
			}
			//xmlData should already be tested ahead of time to make sure it's not blank.
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(List<ProcedureCode>));
			if(path!="") {
				if(!File.Exists(path)) {
					throw new ApplicationException(Lan.g("FormProcCodes","File does not exist."));
				}
				try {
					using TextReader textReader=new StreamReader(path);
					listProcedureCodes=(List<ProcedureCode>)xmlSerializer.Deserialize(textReader);
				}
				catch(InvalidOperationException) {
					throw new ApplicationException(Lan.g("FormProcCodes","Invalid file format"));
				}
			}
			else if(xmlData!="") {
				try {
					using TextReader textReader=new StringReader(xmlData);
					listProcedureCodes=(List<ProcedureCode>)xmlSerializer.Deserialize(textReader);
				}
				catch(InvalidOperationException) {
					throw new ApplicationException(Lan.g("FormProcCodes","Bad xml format"));
				}
				XmlDocument xmlDocumentNcodes=new XmlDocument();
				xmlDocumentNcodes.LoadXml(xmlData);
				//Currently this will only run for NoFeeProcCodes.txt
				//If we run this for another file we will need to double check the structure of the file and make changes to this if needed.
				List<XmlNode> listXmlNodes=xmlDocumentNcodes.ChildNodes[1].ChildNodes.AsEnumerable<XmlNode>().ToList();
				for(int i=0;i<listXmlNodes.Count;i++){//loop through procedureCodes
				//foreach(XmlNode procNode in xmlDocumentNcodes.ChildNodes[1]){//1=ArrayOfProcedureCode
					string procCode="";
					string procCatDescript="";
					for(int j=0;j<listXmlNodes[i].ChildNodes.Count;j++){
						if(listXmlNodes[i].ChildNodes[j].Name=="ProcCode") {
							procCode=listXmlNodes[i].ChildNodes[j].InnerText;
						}
						if(listXmlNodes[i].ChildNodes[j].Name=="ProcCatDescript") {
							procCatDescript=listXmlNodes[i].ChildNodes[j].InnerText;
						}
					}
					listProcedureCodes.First(x => x.ProcCode==procCode).ProcCatDescript=procCatDescript;
				}
			}
			int retVal=0;
			for(int i=0;i<listProcedureCodes.Count;i++) {
				if(ProcedureCodes.GetContainsKey(listProcedureCodes[i].ProcCode)) {
					continue;//don't import duplicates.
				}
				listProcedureCodes[i].ProcCat=Defs.GetByExactName(DefCat.ProcCodeCats,listProcedureCodes[i].ProcCatDescript);
				if(listProcedureCodes[i].ProcCat==0) {//no category exists with that name
					Def def=new Def();
					def.Category=DefCat.ProcCodeCats;
					def.ItemName=listProcedureCodes[i].ProcCatDescript;
					def.ItemOrder=Defs.GetDefsForCategory(DefCat.ProcCodeCats).Count;
					DefL.Insert(def);
					Cache.Refresh(InvalidType.Defs);
					listProcedureCodes[i].ProcCat=def.DefNum;
				}
				listProcedureCodes[i].ProvNumDefault=0;//Always import procedure codes with no specific provider set.  The incoming prov might not exist.
				ProcedureCodes.Insert(listProcedureCodes[i]);				
				SecurityLogs.MakeLogEntry(EnumPermType.ProcCodeEdit,0,"Code"+listProcedureCodes[i].ProcCode+" added from procedure code import.",listProcedureCodes[i].CodeNum,
					DateTime.MinValue);
				retVal++;
			}
			return retVal;
			//don't forget to refresh procedurecodes
		}

		private void butProcTools_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin, DateTime.MinValue)) {
				return;
			}
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			using FormProcTools formProcTools=new FormProcTools();
			formProcTools.ShowDialog();
			if(!formProcTools.Changed) {
				return;
			}
			FillCats();
			ProcedureCodes.RefreshCache();
			//the form above already fired off all necessary invalidation signals
			SynchAndFillListFees(false);
			FillGrid();
		}

		private void butNew_Click(object sender,System.EventArgs e) {
			//won't be visible if no permission
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			using FormProcCodeNew formProcCodeNew=new FormProcCodeNew();
			formProcCodeNew.ShowDialog();
			if(!formProcCodeNew.IsChanged) {
				return;
			}
			_doSetInvalidProcCodes=true;
			ProcedureCodes.RefreshCache();
			SynchAndFillListFees(false);
			FillGrid();
		}
		#endregion

		#region Compare Fee Schedules
		///<summary>For all three combo feescheds</summary>
		private void comboFeeSched_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboBoxes();//for global fee scheds, to disable other combos
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		///<summary>For all 6 clinic and provider combos</summary>
		private void comboClinicProv_SelectionChangeCommitted(object sender,EventArgs e) {
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void butPickFeeSched_Click(object sender,EventArgs e) {
			int selectedIndex=GetFeeSchedIndexFromPicker();
			//If the selectedIndex is -1, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==-1) {
				return;
			}
			UI.Button buttonPicker=(UI.Button)sender;
			if(buttonPicker==butPickSched1) { //First FeeSched combobox doesn't have "None" option.
				comboFeeSched1.SelectedIndex=selectedIndex;
			}
			else if(buttonPicker==butPickSched2) {
				comboFeeSched2.SelectedIndex=selectedIndex+1;
			}
			else if(buttonPicker==butPickSched3) {
				comboFeeSched3.SelectedIndex=selectedIndex+1;
			}
			FillComboBoxes();//to handle global fee scheds
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void butPickClinic_Click(object sender,EventArgs e){
			List<Clinic> listClinicsToShow=new List<Clinic>();
			List<FeeSchedGroup> listFeeSchedGroupsToShow=new List<FeeSchedGroup>();
			UI.Button buttonPicker=(UI.Button)sender;
			bool isPickingFeeSchedGroup=false;
			//Build the list of clinics to show from the combobox tags.
			//We get the list of clinics from the comboboxes because that list has been filtered to prevent clinics already in FeeSchedGroups from showing.
			//TODO: overload to show generic picker for feeschedgroups.
			if(buttonPicker==butPickClinic1) {
				if(checkGroups1.Checked){
					listFeeSchedGroupsToShow=comboFeeSchedGroup1.Items.GetAll<FeeSchedGroup>();
					isPickingFeeSchedGroup=true;
				}
				else {
					listClinicsToShow=comboClinic1.ListClinics;
				}
			}
			else if(buttonPicker==butPickClinic2) {
				if(checkGroups2.Checked) {
					listFeeSchedGroupsToShow=comboFeeSchedGroup2.Items.GetAll<FeeSchedGroup>();
					isPickingFeeSchedGroup=true;
				}
				else {
					listClinicsToShow=comboClinic2.ListClinics;
				}
			}
			else if(buttonPicker==butPickClinic3) {
				if(checkGroups3.Checked) {
					listFeeSchedGroupsToShow=comboFeeSchedGroup3.Items.GetAll<FeeSchedGroup>();
					isPickingFeeSchedGroup=true;
				}
				else {
					listClinicsToShow=comboClinic3.ListClinics;
				}
			}
			else {
				listClinicsToShow=_listClinics;
			}
			int selectedIndex=0;
			long selectedClinicNum=0;
			if(isPickingFeeSchedGroup) {
				selectedIndex=GetFeeSchedGroupIndexFromPicker(listFeeSchedGroupsToShow);
				if(buttonPicker==butPickClinic1 && comboFeeSchedGroup1.Items.Count>0) {
					comboFeeSchedGroup1.SelectedIndex=selectedIndex;
				}
				else if(buttonPicker==butPickClinic2 && comboFeeSchedGroup2.Items.Count>0) {
					comboFeeSchedGroup2.SelectedIndex=selectedIndex;
				}
				else if(buttonPicker==butPickClinic3 && comboFeeSchedGroup3.Items.Count>0) {
					comboFeeSchedGroup3.SelectedIndex=selectedIndex;
				}
			}
			else {
				selectedClinicNum=GetClinicNumFromPicker(listClinicsToShow);//All clinic combo boxes have a none option, so always add 1.
				if(buttonPicker==butPickClinic1) {
					comboClinic1.ClinicNumSelected=selectedClinicNum;
				}
				else if(buttonPicker==butPickClinic2) {
					comboClinic2.ClinicNumSelected=selectedClinicNum;
				}
				else if(buttonPicker==butPickClinic3) {
					comboClinic3.ClinicNumSelected=selectedClinicNum;
				}
			}
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void butPickProvider_Click(object sender,EventArgs e){
			int selectedIndex=GetProviderIndexFromPicker()+1;//All provider combo boxes have a none option, so always add 1.
			//If the selectedIndex is 0, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==0) {
				return;
			}
			UI.Button buttonPicker=(UI.Button)sender;
			if(buttonPicker==butPickProv1) {
				comboProvider1.SelectedIndex=selectedIndex;
			}
			else if(buttonPicker==butPickProv2) {
				comboProvider2.SelectedIndex=selectedIndex;
			}
			else if(buttonPicker==butPickProv3) {
				comboProvider3.SelectedIndex=selectedIndex;
			}
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		///<summary>Launches the Provider picker and lets the user pick a specific provider.
		///Returns the index of the selected provider within the Provider Cache (short).  Returns -1 if the user cancels out of the window.</summary>
		private int GetProviderIndexFromPicker() {
			using FormProviderPick formProviderPick=new FormProviderPick();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return -1;
			}
			return Providers.GetIndex(formProviderPick.ProvNumSelected);
		}

		///<summary>Launches the Clinics window and lets the user pick a specific clinic.
		///Returns the index of the selected clinic within _arrayClinics.  Returns -1 if the user cancels out of the window.</summary>
		private long GetClinicNumFromPicker(List<Clinic> listClinics) {
			using FormClinics formClinics=new FormClinics();
			formClinics.IsSelectionMode=true;
			formClinics.ListClinics=listClinics.ToList();
			formClinics.ShowDialog();
			return formClinics.ClinicNumSelected;
		}

		private int GetFeeSchedGroupIndexFromPicker(List<FeeSchedGroup> listFeeSchedGroups) {
			List<GridColumn> listGridColumnsHeaders=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Description"),100){ IsWidthDynamic=true }
			};
			List<GridRow> listGridRowValues=new List<GridRow>();
			for(int i = 0;i<listFeeSchedGroups.Count;i++) {
				GridRow row=new GridRow(listFeeSchedGroups[i].Description);
				row.Tag=listFeeSchedGroups[i];
				listGridRowValues.Add(row);
			}
			string formTitle=Lan.g(this,"Fee Schedule Group Picker");
			string gridTitle=Lan.g(this,"Fee Schedule Groups");
			using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnsHeaders,listGridRowValues,formTitle,gridTitle);
			if(formGridSelection.ShowDialog()==DialogResult.OK) {
				return listFeeSchedGroups.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)formGridSelection.ListSelectedTags[0]).FeeSchedGroupNum));
			}
			//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
			return 0;
		}

		///<summary>Launches the Fee Schedules window and lets the user pick a specific schedule.
		///Returns the index of the selected schedule within _listFeeScheds.  Returns -1 if the user cancels out of the window.</summary>
		private int GetFeeSchedIndexFromPicker() {
			//No need to check security because we are launching the form in selection mode.
			using FormFeeScheds formFeeScheds=new FormFeeScheds(true);
			formFeeScheds.ShowDialog();
			return _listFeeScheds.FindIndex(x => x.FeeSchedNum==formFeeScheds.SelectedFeeSchedNum);//Returns index of the found element or -1.
		}

		#endregion

		private void comboSort_SelectionChangeCommitted(object sender,EventArgs e) {
			_procCodeListSorts=(ProcCodeListSort)comboSort.SelectedIndex;
			FillGrid();
		}

		private void checkGroups1_CheckedChanged(object sender,EventArgs e) {
			if(checkGroups1.Checked) {
				//Hide clinic combobox and wipe selection.
				comboClinic1.Visible=false;
				comboClinic1.ClinicNumSelected=0;
				comboFeeSchedGroup1.Visible=true;
				labelClinic1.Text="Fee Schedule Group";
			}
			else {
				comboClinic1.Visible=true;
				comboFeeSchedGroup1.Visible=false;
				labelClinic1.Text="Clinic";
			}
			//Making the picker window display FeeSchedGroups could be an enhamcement in the future.
			butPickClinic1.Enabled=!checkGroups1.Checked;
			FillComboBoxes();
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void checkGroups2_CheckedChanged(object sender,EventArgs e) {
			if(checkGroups2.Checked) {
				//Hide clinic combobox and wipe selection.
				comboClinic2.Visible=false;
				comboClinic2.ClinicNumSelected=0;
				comboFeeSchedGroup2.Visible=true;
				labelClinic2.Text="Fee Schedule Group";
			}
			else {
				comboClinic2.Visible=true;
				comboFeeSchedGroup2.Visible=false;
				labelClinic2.Text="Clinic";
			}
			//Making the picker window display FeeSchedGroups could be an enhamcement in the future.
			butPickClinic2.Enabled=!checkGroups2.Checked;
			FillComboBoxes();
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void checkGroups3_CheckedChanged(object sender,EventArgs e) {
			if(checkGroups3.Checked) {
				//Hide clinic combobox and wipe selection.
				comboClinic3.Visible=false;
				comboClinic3.ClinicNumSelected=0;
				comboFeeSchedGroup3.Visible=true;
				labelClinic3.Text="Fee Schedule Group";
			}
			else {
				comboClinic3.Visible=true;
				comboFeeSchedGroup3.Visible=false;
				labelClinic3.Text="Clinic";
			}
			//Making the picker window display FeeSchedGroups could be an enhamcement in the future.
			butPickClinic3.Enabled=!checkGroups3.Checked;
			FillComboBoxes();
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void comboFeeSchedGroup_SelectionChangeCommitted(object sender,EventArgs e) {
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a procedure code first.");
				return;
			}
			ListProcedureCodesSelected=gridMain.SelectedTags<ProcedureCode>().Select(x => x.Copy()).ToList();
			CodeNumSelected=ListProcedureCodesSelected.First().CodeNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProcedures_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			if(_needsSynch){
				Cursor=Cursors.WaitCursor;
				SynchAndFillListFees(true);
				Cursor=Cursors.Default;
			}
			if(_doSetInvalidProcCodes) {
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
		}
	}
}
