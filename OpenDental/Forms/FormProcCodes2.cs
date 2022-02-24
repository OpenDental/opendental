/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
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
		public long SelectedCodeNum;
		//public string SelectedADA;
		///<summary>This is just set once for the whole session.  It doesn't get toggled back to false.  So it only get acted upon when the form closes, sending a signal to other computers telling them to refresh their proc codes.</summary>
		private bool _setInvalidProcCodes;
		///<summary>Once a synch is done, this will switch back to false.  This also triggers SaveLogs, which then clears _dictFeeLogs to prepare for more logs.</summary>
		private bool _needsSynch;
		///<summary>Set to true externally in order to let user select one procedure code.</summary>
		public bool IsSelectionMode;
		///<summary>The list of definitions that is currently showing in the category list.</summary>
		private Def[] CatList;
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
		private ProcCodeListSort _procCodeSort;
		/// <summary> List should contain two logs per fee because we are inserting two security logs everytime we update a fee.</summary>
		private Dictionary<long,List<SecurityLog>> _dictFeeLogs;
		private bool _canShowHidden;
		///<summary>Contains all of the procedure codes that were selected if IsSelectionMode is true.
		///If IsSelectionMode is true and this list is prefilled with procedure codes then the grid will preselect as many codes as possible.
		///It is not guaranteed that all procedure codes will be selected due to filters.
		///This list should only be read from externally after DialogResult.OK has been returned.</summary>
		public List<ProcedureCode> ListSelectedProcCodes=new List<ProcedureCode>();
		///<summary>Set to true when IsSelectionMode is true and the user should be able to select multiple procedure codes instead of just one.
		///ListSelectedProcCodes will contain all of the procedure codes that the user selected.</summary>
		public bool AllowMultipleSelections;

		///<summary>When canShowHidden is true to the "Hidden" checkbox and "default" button are visible.</summary>
		public FormProcCodes(bool canShowHidden=false) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_canShowHidden=canShowHidden;
		}
		
		private void FormProcCodes_Load(object sender,System.EventArgs e) {
			_dictFeeLogs=new Dictionary<long,List<SecurityLog>>();
			if(!Security.IsAuthorized(Permissions.Setup,DateTime.MinValue,true)) {
				butEditFeeSched.Visible=false;
				butTools.Visible=false;
				butEditCategories.Visible=false;
			}
			if(!Security.IsAuthorized(Permissions.ProcCodeEdit,true)) {
				groupProcCodeSetup.Visible=false;
			}
			if(!IsSelectionMode) {
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
			}
			else if(AllowMultipleSelections) {
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
			bool _isShowingGroups=PrefC.GetBool(PrefName.ShowFeeSchedGroups);
			checkGroups1.Visible=_isShowingGroups;
			checkGroups2.Visible=_isShowingGroups;
			checkGroups3.Visible=_isShowingGroups;
			LayoutManager.MoveLocation(comboFeeSchedGroup1,new Point(14,96));
			LayoutManager.MoveLocation(comboFeeSchedGroup2,new Point(14,96));
			LayoutManager.MoveLocation(comboFeeSchedGroup3,new Point(14,96));
			FillComboBoxes();
			SynchAndFillListFees(false);
			_needsSynch=false;
			for(int i=0;i<Enum.GetNames(typeof(ProcCodeListSort)).Length;i++) {
				comboSort.Items.Add(Enum.GetNames(typeof(ProcCodeListSort))[i]);
			}
			_procCodeSort=(ProcCodeListSort)PrefC.GetInt(PrefName.ProcCodeListSortOrder);
			comboSort.SelectedIndex=(int)_procCodeSort;
			FillGrid();
			//Preselect corresponding procedure codes once on load.  Do not do it within FillGrid().
			if(ListSelectedProcCodes.Count > 0) {
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(ListSelectedProcCodes.Any(x => x.CodeNum==((ProcedureCode)gridMain.ListGridRows[i].Tag).CodeNum)) {
						gridMain.SetSelected(i,true);
					}
				}
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
					Dictionary<long,List<FeeSchedGroup>> dictFeeSchedClinics=FeeSchedGroups.GetDictFeeSchedGroups(listFeeSchedNumsSelected);
					foreach(long clinicNum in _listClinics.Select(x => x.ClinicNum)) {
						if(feeSchedNum1Selected>0
							&& checkGroups1.Checked
							&& dictFeeSchedClinics.TryGetValue(feeSchedNum1Selected,out List<FeeSchedGroup> listFeeSchedGroups1))
						{
							 listFeeSchedGroups1.FindAll(x => x.ListClinicNumsAll.Contains(clinicNum))
								.ForEach(x => AddFeeSchedGroupToComboBox(x,comboFeeSchedGroup1,feeSchedGroupNum1Selected));
						}
						if(feeSchedNum2Selected>0
							&& checkGroups2.Checked
							&& dictFeeSchedClinics.TryGetValue(feeSchedNum2Selected,out List<FeeSchedGroup> listFeeSchedGroups2))
						{
							listFeeSchedGroups2.FindAll(x => x.ListClinicNumsAll.Contains(clinicNum))
								.ForEach(x => AddFeeSchedGroupToComboBox(x,comboFeeSchedGroup2,feeSchedGroupNum2Selected));
						}
						if(feeSchedNum3Selected>0
							&& checkGroups3.Checked
							&& dictFeeSchedClinics.TryGetValue(feeSchedNum3Selected,out List<FeeSchedGroup> listFeeSchedGroups3))
						{
							listFeeSchedGroups3.FindAll(x => x.ListClinicNumsAll.Contains(clinicNum))
								.ForEach(x => AddFeeSchedGroupToComboBox(x,comboFeeSchedGroup3,feeSchedGroupNum3Selected));
						}
					}
				}
			}
			//Fill provider combo boxes
			string[] arrayProvAbbrs=new[] { Lan.g(this,"None") }.Concat(_listProviders.Select(x => x.Abbr)).ToArray();
			comboProvider1.Items.AddRange(arrayProvAbbrs);
			comboProvider2.Items.AddRange(arrayProvAbbrs);
			comboProvider3.Items.AddRange(arrayProvAbbrs);
			comboProvider1.SelectedIndex=Math.Max(0,comboProv1Idx);
			comboProvider2.SelectedIndex=Math.Max(0,comboProv2Idx);
			comboProvider3.SelectedIndex=Math.Max(0,comboProv3Idx);
			//If previously selected FeeSched was global, and the newly selected FeeSched is NOT global, select OD's selected Clinic in the combo box.
			if(_listFeeScheds.Count > 0 && comboFeeSched1.GetSelected<FeeSched>()!=null && comboFeeSched1.GetSelected<FeeSched>().IsGlobal) {
				comboClinic1.Enabled=false;
				butPickClinic1.Enabled=false;
				comboClinic1.SelectedClinicNum=0;				
				comboProvider1.Enabled=false;
				butPickProv1.Enabled=false;
				comboProvider1.SelectedIndex=0;
				comboFeeSchedGroup1.Enabled=false;
			}
			else {//Newly selected FeeSched is NOT global
				if(PrefC.HasClinicsEnabled) {
					if(feeSchedNum1Selected==0 || comboClinic1.Enabled==false) {
						//Previously selected FeeSched WAS global or there was none selected previously, select OD's selected Clinic
						comboClinic1.SelectedClinicNum=Clinics.ClinicNum;
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
				comboClinic2.SelectedClinicNum=0;
				comboProvider2.Enabled=false;
				butPickProv2.Enabled=false;
				comboProvider2.SelectedIndex=0;
				comboFeeSchedGroup2.Enabled=false;
			}
			else {//Newly selected FeeSched is NOT global
				if(PrefC.HasClinicsEnabled) {
					if(comboClinic2.Enabled==false) {
						//Previously selected FeeSched WAS global, select OD's selected Clinic
						comboClinic2.SelectedClinicNum=Clinics.ClinicNum;
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
				comboClinic3.SelectedClinicNum=0;
				comboProvider3.Enabled=false;
				butPickProv3.Enabled=false;
				comboProvider3.SelectedIndex=0;
				comboFeeSchedGroup3.Enabled=false;
			}
			else {//Newly selected FeeSched is NOT global
				if(PrefC.HasClinicsEnabled) {
					if(comboClinic3.Enabled==false) {//Previously selected FeeSched WAS global
						//Select OD's selected Clinic
						comboClinic3.SelectedClinicNum=Clinics.ClinicNum;
					}
					comboClinic3.Enabled=true;
					butPickClinic3.Enabled=true;
					comboFeeSchedGroup3.Enabled=true;
				}
				comboProvider3.Enabled=true;
				butPickProv3.Enabled=true;
			}
		}

		private void AddFeeSchedGroupToComboBox(FeeSchedGroup feeSchedGroupCur,ComboBoxOD comboFeeSchedGroup,long feeSchedGroupNumSelected) {
			if(feeSchedGroupCur==null
				|| feeSchedGroupCur.ListClinicNumsAll.Count==0 //skip if empty group
				|| !feeSchedGroupCur.ListClinicNumsAll.Exists(x => _listClinics.Any(y => y.ClinicNum==x)) //skip if user doesn't have access to any of the clinics in the group
				|| comboFeeSchedGroup.Items.GetAll<FeeSchedGroup>().Any(x => x.FeeSchedGroupNum==feeSchedGroupCur.FeeSchedGroupNum))
			{
				return;
			}
			comboFeeSchedGroup.Items.Add(feeSchedGroupCur.Description,feeSchedGroupCur);
			if(feeSchedGroupCur.FeeSchedGroupNum==feeSchedGroupNumSelected) {
				comboFeeSchedGroup.SelectedIndex=comboFeeSchedGroup.Items.Count-1;
			}
		}

		///<summary>The only reason we use this is to keep interface snappy when entering a series of fees in the grid.  So the moment the user stops doing that and switches to something else is when we take the time to synch the changes to the db and start over fresh the next time.  We also do that whenever the selected feesched is changed, when closing form, when double clicking a code, importing, etc.  Use this method liberally.  Run it first with includeSynch=true, then run it again after doing other things, just to make sure we have fresh data for the new situation.</summary>
		private void SynchAndFillListFees(bool includeSynch){
			//first, synch the old list
			Cursor=Cursors.WaitCursor;
			if(includeSynch && _listFeesDb!=null){
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
				listClinicNums1=new List<long> { comboClinic1.SelectedClinicNum };
				listClinicNums2=new List<long> { comboClinic2.SelectedClinicNum };
				listClinicNums3=new List<long> { comboClinic3.SelectedClinicNum };
				if(PrefC.GetBool(PrefName.ShowFeeSchedGroups)) {
					//First groupbox
					if(checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						listClinicNums1=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
					}
					//Second groupbox
					if(checkGroups2.Checked && comboFeeSchedGroup2.SelectedIndex>-1) {
						listClinicNums2=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
					}
					//Third groupbox
					if(checkGroups3.Checked && comboFeeSchedGroup3.SelectedIndex>-1) {
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
			foreach(long feeNum in _dictFeeLogs.Keys) {
				foreach(SecurityLog secLog in _dictFeeLogs[feeNum]) {
					SecurityLogs.MakeLogEntry(secLog);
				}
			}
			_dictFeeLogs.Clear();
		}

		private void FillCats() {
			ArrayList selected=new ArrayList();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				selected.Add(CatList[listCategories.SelectedIndices[i]].DefNum);
			}
			if(checkShowHidden.Checked) {
				CatList=Defs.GetDefsForCategory(DefCat.ProcCodeCats).ToArray();
			}
			else {
				CatList=Defs.GetDefsForCategory(DefCat.ProcCodeCats,true).ToArray();
			}
			listCategories.Items.Clear();
			for(int i=0;i<CatList.Length;i++) {
				listCategories.Items.Add(CatList[i].ItemName);
				if(selected.Contains(CatList[i].DefNum)) {
					listCategories.SetSelected(i);
				}
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
			List<Def> listCatDefs=new List<Def>();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				listCatDefs.Add(CatList[listCategories.SelectedIndices[i]]);
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
					if(checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						clinic1Num=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic1Num=comboClinic1.SelectedClinicNum;
					}
					//Second groupbox
					if(checkGroups2.Checked && comboFeeSchedGroup2.SelectedIndex>-1) {
						clinic2Num=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic2Num=comboClinic2.SelectedClinicNum;
					}
					//Third groupbox
					if(checkGroups3.Checked && comboFeeSchedGroup3.SelectedIndex>-1) {
						clinic3Num=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
					}
					else {
						clinic3Num=comboClinic3.SelectedClinicNum;
					}
				}
				else {
					clinic1Num=comboClinic1.SelectedClinicNum;
					clinic2Num=comboClinic2.SelectedClinicNum;
					clinic3Num=comboClinic3.SelectedClinicNum;
				}
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			//The order of these columns are important for gridMain_CellDoubleClick(), gridMain_CellLeave(), and GridMain_CellEnter()
			GridColumn col=new GridColumn(Lan.g("TableProcedures","Category"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Description"),206);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Abbr"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcedures","Code"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Fee 1",50,HorizontalAlignment.Right,true);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Fee 2",50,HorizontalAlignment.Right,true);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Fee 3",50,HorizontalAlignment.Right,true);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string searchAbbr=textAbbreviation.Text;
			string searchDesc=textDescription.Text;
			string searchCode=textCode.Text;
			List<ProcedureCode> listProcsForCats=new List<ProcedureCode>();
			//Loop through the list of categories which are ordered by def.ItemOrder.
			if(_procCodeSort==ProcCodeListSort.Category) {
				for(int i=0;i<listCatDefs.Count;i++) {
					//Get all procedure codes that are part of the selected category.  Then order the list of procedures by ProcCodes.
					//Append the list of ordered procedures to the master list of procedures for the selected categories.
					//Appending the procedure codes in this fashion keeps them ordered correctly via the definitions ItemOrder.
					listProcsForCats.AddRange(ProcedureCodes.GetWhereFromList(proc => proc.ProcCat==listCatDefs[i].DefNum)
						.OrderBy(proc => proc.ProcCode).ToList());
				}
			}
			else if(_procCodeSort==ProcCodeListSort.ProcCode) {
				for(int i = 0;i<listCatDefs.Count;i++) {
					listProcsForCats.AddRange(ProcedureCodes.GetWhereFromList(proc => proc.ProcCat==listCatDefs[i].DefNum).ToList());
				}
				listProcsForCats=listProcsForCats.OrderBy(proc => proc.ProcCode).ToList();
			}
			//Remove any procedures that do not meet our filters.
			listProcsForCats.RemoveAll(proc => !proc.AbbrDesc.ToLower().Contains(searchAbbr.ToLower()));
			listProcsForCats.RemoveAll(proc => !proc.Descript.ToLower().Contains(searchDesc.ToLower()));
			listProcsForCats.RemoveAll(proc => !proc.ProcCode.ToLower().Contains(searchCode.ToLower()));
			if(IsSelectionMode) {
				listProcsForCats.RemoveAll(proc => proc.ProcCode==ProcedureCodes.GroupProcCode);
			}
			string lastCategoryName="";
			foreach(ProcedureCode procCode in listProcsForCats) { 
				row=new GridRow();
				row.Tag=procCode;
				//Only show the category on the first procedure code in that category.
				string categoryName=Defs.GetName(DefCat.ProcCodeCats,procCode.ProcCat);
				if(lastCategoryName!=categoryName && _procCodeSort==ProcCodeListSort.Category) {
					row.Cells.Add(categoryName);
					lastCategoryName=categoryName;
				}
				else {//This proc code is in the same category or we are not sorting by category.
					row.Cells.Add("");
				}
				row.Cells.Add(procCode.Descript);
				row.Cells.Add(procCode.AbbrDesc);
				row.Cells.Add(procCode.ProcCode);
				Fee fee1=Fees.GetFee(procCode.CodeNum,feeSched1.FeeSchedNum,clinic1Num,provider1Num,_listFees);
				Fee fee2=null;
				if(feeSched2!=null) {
					fee2=Fees.GetFee(procCode.CodeNum,feeSched2.FeeSchedNum,clinic2Num,provider2Num,_listFees);
				}
				Fee fee3=null;
				if(feeSched3!=null) {
					fee3=Fees.GetFee(procCode.CodeNum,feeSched3.FeeSchedNum,clinic3Num,provider3Num,_listFees);
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
				SelectedCodeNum=((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).CodeNum;
				ListSelectedProcCodes.Clear();
				ListSelectedProcCodes.Add(((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).Copy());
				DialogResult=DialogResult.OK;
				return;
			}
			//else not selecting a code
			if(!Security.IsAuthorized(Permissions.ProcCodeEdit)) {
				return;
			}
			if(e.Col>3) {
				//Do nothing. All columns > 3 are editable (You cannot double click).
				return;
			}
			//not on a fee: Edit code instead
			//changed=false;//We just updated the database and synced our cache, set changed to false.
			ProcedureCode procCode=(ProcedureCode)gridMain.ListGridRows[e.Row].Tag;
			Def defProcCat=Defs.GetDefsForCategory(DefCat.ProcCodeCats).FirstOrDefault(x => x.DefNum==procCode.ProcCat);
			using FormProcCodeEdit formProcCodeEdit=new FormProcCodeEdit(procCode);
			formProcCodeEdit.IsNew=false;
			formProcCodeEdit.ShowHiddenCategories=(defProcCat!=null ? defProcCat.IsHidden : false);
			formProcCodeEdit.ShowDialog();
			_setInvalidProcCodes=true;
			//The user could have edited a fee within the Procedure Code Edit window or within one of it's children so we need to refresh our cache.
			//Yes, it could have even changed if the user Canceled out of the Proc Code Edit window (e.g. use FormProcCodeEditMore.cs)
			SynchAndFillListFees(false);
			FillGrid();
		}

		private void GridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			Security.IsAuthorized(Permissions.FeeSchedEdit);//Show message if user does not have permission.
		}

		///<summary>Takes care of individual cell edits.  Calls FillGrid to refresh other columns using the same data.</summary>
		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			//This is where the real fee editing logic is.
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit,true)) { //Don't do anything if they don't have permission.
				return;
			}
			//Logic only works for columns 4 to 6.
			long codeNum=((ProcedureCode)gridMain.ListGridRows[e.Row].Tag).CodeNum;
			FeeSched feeSched=null;
			long provNum=0;
			long clinicNum=0;
			Fee fee=null;
			bool isEditingGroup=false;
			FeeSchedGroup selectedFeeSchedGroup=null;
			if(e.Col==4) {
				feeSched=_listFeeScheds[comboFeeSched1.SelectedIndex];
				if(comboProvider1.SelectedIndex>0) {
					provNum=_listProviders[comboProvider1.SelectedIndex-1].ProvNum;
				}
				if(PrefC.HasClinicsEnabled) {
					if(checkGroups1.Checked && comboFeeSchedGroup1.SelectedIndex>-1) {
						clinicNum=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
						selectedFeeSchedGroup=comboFeeSchedGroup1.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic1.SelectedClinicNum;
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
						selectedFeeSchedGroup=comboFeeSchedGroup2.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic2.SelectedClinicNum;
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
						selectedFeeSchedGroup=comboFeeSchedGroup3.GetSelected<FeeSchedGroup>();
						isEditingGroup=true;
					}
					else {
						clinicNum=comboClinic3.SelectedClinicNum;
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
			if(feeAmtNewStr!="" && !Double.TryParse(gridMain.ListGridRows[e.Row].Cells[e.Col].Text,out feeAmtNew)) {
				gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtOld;
				MessageBox.Show(Lan.g(this,"Please fix data entry error first."));
				return;
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
				FeeSchedGroup groupForClinic=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(groupForClinic!=null) {
					//Duplicating if check from above to prevent us from accidentally hitting users with infinite popups. We want the same end result for both of the checks
					//but we need to do the group check 2nd.
					if(fee==null || fee.Amount==-1) {
						gridMain.ListGridRows[e.Row].Cells[e.Col].Text="";
					}
					else {
						gridMain.ListGridRows[e.Row].Cells[e.Col].Text=feeAmtOld;
					}
					MsgBox.Show(Lans.g(this,"Fee Schedule")+": "+feeSched.Description+" "+Lans.g(this,"for Clinic")+": "+(_listClinics.FirstOrDefault(x => x.ClinicNum==clinicNum)?.Abbr??"")+" "
						+Lans.g(this,"is part of Fee Schedule Group")+": "+groupForClinic.Description+". "+Lans.g(this,"The fees must be edited at the group level."));
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
							&& selectedFeeSchedGroup!=null
							&& !selectedFeeSchedGroup.ListClinicNumsAll.IsNullOrEmpty()
							&& selectedFeeSchedGroup.ListClinicNumsAll.Contains(clinicNum)
							&& selectedFeeSchedGroup.ListClinicNumsAll.Any(x => x!=clinicNum))
						{
							foreach(long clinicNumCur in selectedFeeSchedGroup.ListClinicNumsAll) {
								if(clinicNumCur==clinicNum) {
									continue;
								}
								List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==clinicNumCur && x.ProvNum==provNum);
								if(listFees.IsNullOrEmpty()) {
									_listFees.Add(new Fee() { FeeSched=feeSched.FeeSchedNum,CodeNum=codeNum,Amount=-1.0,ClinicNum=clinicNumCur,ProvNum=provNum });
								}
								else {
									listFees.ForEach(x => x.Amount=-1.0);
								}
							}
						}
					}
					else {//They want to delete a fee for their current settings.
						_listFees.Remove(fee);
						if(isEditingGroup
							&& selectedFeeSchedGroup!=null
							&& !selectedFeeSchedGroup.ListClinicNumsAll.IsNullOrEmpty()
							&& selectedFeeSchedGroup.ListClinicNumsAll.Contains(clinicNum)
							&& selectedFeeSchedGroup.ListClinicNumsAll.Any(x => x!=clinicNum))
						{
							_listFees.RemoveAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && ListTools.In(x.ClinicNum,
								selectedFeeSchedGroup.ListClinicNumsAll) && x.ProvNum==provNum);
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
						&& selectedFeeSchedGroup!=null
						&& !selectedFeeSchedGroup.ListClinicNumsAll.IsNullOrEmpty()
						&& selectedFeeSchedGroup.ListClinicNumsAll.Contains(clinicNum)
						&& selectedFeeSchedGroup.ListClinicNumsAll.Any(x => x!=clinicNum))
					{
						foreach(long clinicNumCur in selectedFeeSchedGroup.ListClinicNumsAll) {
							if(clinicNumCur==clinicNum) {
								continue;
							}
							List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==clinicNumCur && fee.ProvNum==provNum);
							if(listFees.IsNullOrEmpty()) {
								_listFees.Add(new Fee() { FeeSched=feeSched.FeeSchedNum,CodeNum=codeNum,Amount=feeAmtNew,ClinicNum=clinicNumCur,ProvNum=provNum });
							}
							else {
								listFees.ForEach(x => x.Amount=feeAmtNew);
							}
						}
					}
				}
				else { //Fee isn't null, is for our current clinic, is for the selected provider, and they don't want to delete it.  Just update.
					fee.Amount=feeAmtNew;
					if(isEditingGroup
						&& selectedFeeSchedGroup!=null
						&& !selectedFeeSchedGroup.ListClinicNumsAll.IsNullOrEmpty()
						&& selectedFeeSchedGroup.ListClinicNumsAll.Contains(clinicNum)
						&& selectedFeeSchedGroup.ListClinicNumsAll.Any(x => x!=clinicNum))
					{
						foreach(long clinicNumCur in selectedFeeSchedGroup.ListClinicNumsAll) {
							if(clinicNumCur==clinicNum) {
								continue;
							}
							List<Fee> listFees=_listFees.FindAll(x => x.FeeSched==feeSched.FeeSchedNum && x.CodeNum==codeNum && x.ClinicNum==clinicNumCur && fee.ProvNum==provNum);
							if(listFees.IsNullOrEmpty()) {
								_listFees.Add(new Fee() { FeeSched=feeSched.FeeSchedNum,CodeNum=codeNum,Amount=feeAmtNew,ClinicNum=clinicNumCur,ProvNum=provNum });
							}
							else {
								listFees.ForEach(x => x.Amount=feeAmtNew);
							}
						}
					}
				}
			}
			SecurityLog secLog=SecurityLogs.MakeLogEntryNoInsert(Permissions.ProcFeeEdit,0,"Procedure: "+ProcedureCodes.GetStringProcCode(fee.CodeNum)
				+", Fee: "+fee.Amount.ToString("c")
				+", Fee Schedule: "+FeeScheds.GetDescription(fee.FeeSched)
				+". Manual edit in grid from Procedure Codes list.",fee.CodeNum,LogSources.None);
			_dictFeeLogs[fee.FeeNum]=new List<SecurityLog>();
			_dictFeeLogs[fee.FeeNum].Add(secLog);
			_dictFeeLogs[fee.FeeNum].Add(SecurityLogs.MakeLogEntryNoInsert(Permissions.LogFeeEdit,0,"Fee changed",fee.FeeNum,LogSources.None,
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
			ArrayList selected=new ArrayList();
			for(int i=0;i<listCategories.SelectedIndices.Count;i++) {
				selected.Add(CatList[listCategories.SelectedIndices[i]].DefNum);
			}
			using FormDefinitions FormD=new FormDefinitions(DefCat.ProcCodeCats);
			FormD.ShowDialog();
			DataValid.SetInvalid(InvalidType.Defs);
			FillCats();
			for(int i=0;i<CatList.Length;i++) {
				if(selected.Contains(CatList[i].DefNum)) {
					listCategories.SetSelected(i);
				}
			}
			//we need to move security log to within the definition window for more complete tracking
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Definitions");
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
			Prefs.UpdateBool(PrefName.ProcCodeListShowHidden,checkShowHidden.Checked);
			string hiddenStatus="";
			if(checkShowHidden.Checked) {
				hiddenStatus=Lan.g(this,"checked.");
			}
			else {
				hiddenStatus=Lan.g(this,"unchecked.");
			}
			MessageBox.Show(Lan.g(this,"Show Hidden will default to")+" "+hiddenStatus);
		}

		#endregion

		#region Procedure Codes

		private void butEditFeeSched_Click(object sender,System.EventArgs e) {
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			//We are launching in edit mode thus we must check the FeeSchedEdit permission type.
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit)) {
				return;
			}
			using FormFeeScheds FormF=new FormFeeScheds(false); //The Fee Scheds window can add or hide schedules.  It cannot delete schedules.
			FormF.ShowDialog();
			FillComboBoxes();
			//I don't think it would highlight a new sched, so refresh fees should not be needed.
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.FeeSchedEdit,0,"Fee Schedules");
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
			using(FormFeeSchedTools FormF=new FormFeeSchedTools(schedNum,_listFeeScheds,_listProviders,_listClinics)) {
				FormF.ShowDialog();
			}
			//Fees could have changed from within the FeeSchedTools window.  Refresh our local fees.
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)) {
				FillComboBoxes();//To show possible added fee schedule.
			}
			SynchAndFillListFees(false);
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Fee Schedule Tools");
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(ProcedureCodes.GetCount()==0) {
				MsgBox.Show(this,"No procedurecodes are displayed for export.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Only the codes showing in this list will be exported.  Continue?")) {
				return;
			}
			List<ProcedureCode> listCodes=new List<ProcedureCode>();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				ProcedureCode procCode=(ProcedureCode)gridMain.ListGridRows[i].Tag;
				if(procCode.ProcCode=="") {
					continue;
				}
				procCode.ProvNumDefault=0;  //We do not want to export ProvNumDefault because the receiving DB will not have the same exact provNums.
				listCodes.Add(procCode);
			}
			string filename="ProcCodes.xml";
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),filename); 
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				using SaveFileDialog saveDlg=new SaveFileDialog();
				saveDlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				saveDlg.FileName=filename;
				if(saveDlg.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveDlg.FileName;
			}
			XmlSerializer serializer=new XmlSerializer(typeof(List<ProcedureCode>));
			TextWriter writer=new StreamWriter(filePath);
			serializer.Serialize(writer,listCodes);
			writer.Close();
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MsgBox.Show(this,"Exported");
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			using OpenFileDialog openDlg=new OpenFileDialog();
			openDlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			if(openDlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			int rowsInserted=0;
			try {
				rowsInserted=ImportProcCodes(openDlg.FileName,null,"");
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
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,Lan.g(this,"Imported Procedure Codes"));
		}

		///<summary>Can be called externally.  Surround with try catch.  Returns number of codes inserted. 
		///Supply path to file to import or a list of procedure codes, or an xml string.  Make sure to set the other two values blank or null.</summary>
		public static int ImportProcCodes(string path,List<ProcedureCode> listCodes,string xmlData) {
			if(listCodes==null) {
				listCodes=new List<ProcedureCode>();
			}
			//xmlData should already be tested ahead of time to make sure it's not blank.
			XmlSerializer serializer=new XmlSerializer(typeof(List<ProcedureCode>));
			if(path!="") {
				if(!File.Exists(path)) {
					throw new ApplicationException(Lan.g("FormProcCodes","File does not exist."));
				}
				try {
					using(TextReader reader=new StreamReader(path)) {
						listCodes=(List<ProcedureCode>)serializer.Deserialize(reader);
					}
				}
				catch {
					throw new ApplicationException(Lan.g("FormProcCodes","Invalid file format"));
				}
			}
			else if(xmlData!="") {
				try {
					using(TextReader reader=new StringReader(xmlData)) {
						listCodes=(List<ProcedureCode>)serializer.Deserialize(reader);
					}
				}
				catch {
					throw new ApplicationException(Lan.g("FormProcCodes","xml format"));
				}
				XmlDocument xmlDocNcodes=new XmlDocument();
				xmlDocNcodes.LoadXml(xmlData);
				//Currently this will only run for NoFeeProcCodes.txt
				//If we run this for another file we will need to double check the structure of the file and make changes to this if needed.
				foreach(XmlNode procNode in xmlDocNcodes.ChildNodes[1]){//1=ArrayOfProcedureCode
					string procCode="";
					string procCatDescript="";
					foreach(XmlNode procFieldNode in procNode.ChildNodes) {
						if(procFieldNode.Name=="ProcCode") {
							procCode=procFieldNode.InnerText;
						}
						if(procFieldNode.Name=="ProcCatDescript") {
							procCatDescript=procFieldNode.InnerText;
						}
					}
					listCodes.First(x => x.ProcCode==procCode).ProcCatDescript=procCatDescript;
				}
			}
			int retVal=0;
			for(int i=0;i<listCodes.Count;i++) {
				if(ProcedureCodes.GetContainsKey(listCodes[i].ProcCode)) {
					continue;//don't import duplicates.
				}
				listCodes[i].ProcCat=Defs.GetByExactName(DefCat.ProcCodeCats,listCodes[i].ProcCatDescript);
				if(listCodes[i].ProcCat==0) {//no category exists with that name
					Def def=new Def();
					def.Category=DefCat.ProcCodeCats;
					def.ItemName=listCodes[i].ProcCatDescript;
					def.ItemOrder=Defs.GetDefsForCategory(DefCat.ProcCodeCats).Count;
					Defs.Insert(def);
					Cache.Refresh(InvalidType.Defs);
					listCodes[i].ProcCat=def.DefNum;
				}
				listCodes[i].ProvNumDefault=0;//Always import procedure codes with no specific provider set.  The incoming prov might not exist.
				ProcedureCodes.Insert(listCodes[i]);				
				SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,"Code"+listCodes[i].ProcCode+" added from procedure code import.",listCodes[i].CodeNum,
					DateTime.MinValue);
				retVal++;
			}
			return retVal;
			//don't forget to refresh procedurecodes
		}

		private void butProcTools_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin, DateTime.MinValue)) {
				return;
			}
			if(_needsSynch){
				SynchAndFillListFees(true);
			}
			using FormProcTools FormP=new FormProcTools();
			FormP.ShowDialog();
			if(!FormP.Changed) {
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
			if(!formProcCodeNew.Changed) {
				return;
			}
			_setInvalidProcCodes=true;
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
			UI.Button pickerButton=(UI.Button)sender;
			if(pickerButton==butPickSched1) { //First FeeSched combobox doesn't have "None" option.
				comboFeeSched1.SelectedIndex=selectedIndex;
			}
			else if(pickerButton==butPickSched2) {
				comboFeeSched2.SelectedIndex=selectedIndex+1;
			}
			else if(pickerButton==butPickSched3) {
				comboFeeSched3.SelectedIndex=selectedIndex+1;
			}
			FillComboBoxes();//to handle global fee scheds
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		private void butPickClinic_Click(object sender,EventArgs e){
			List<Clinic> listClinicsToShow=new List<Clinic>();
			List<FeeSchedGroup> listGroupsToShow=new List<FeeSchedGroup>();
			UI.Button pickerButton=(UI.Button)sender;
			bool isPickingFeeSchedGroup=false;
			//Build the list of clinics to show from the combobox tags.
			//We get the list of clinics from the comboboxes because that list has been filtered to prevent clinics already in FeeSchedGroups from showing.
			//TODO: overload to show generic picker for feeschedgroups.
			if(pickerButton==butPickClinic1) {
				if(checkGroups1.Checked){
					listGroupsToShow=comboFeeSchedGroup1.Items.GetAll<FeeSchedGroup>();
					isPickingFeeSchedGroup=true;
				}
				else {
					listClinicsToShow=comboClinic1.ListClinics;
				}
			}
			else if(pickerButton==butPickClinic2) {
				if(checkGroups2.Checked) {
					listGroupsToShow=comboFeeSchedGroup2.Items.GetAll<FeeSchedGroup>();
					isPickingFeeSchedGroup=true;
				}
				else {
					listClinicsToShow=comboClinic2.ListClinics;
				}
			}
			else if(pickerButton==butPickClinic3) {
				if(checkGroups3.Checked) {
					listGroupsToShow=comboFeeSchedGroup3.Items.GetAll<FeeSchedGroup>();
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
				selectedIndex=GetFeeSchedGroupIndexFromPicker(listGroupsToShow);
				if(pickerButton==butPickClinic1 && comboFeeSchedGroup1.Items.Count>0) {
					comboFeeSchedGroup1.SelectedIndex=selectedIndex;
				}
				else if(pickerButton==butPickClinic2 && comboFeeSchedGroup2.Items.Count>0) {
					comboFeeSchedGroup2.SelectedIndex=selectedIndex;
				}
				else if(pickerButton==butPickClinic3 && comboFeeSchedGroup3.Items.Count>0) {
					comboFeeSchedGroup3.SelectedIndex=selectedIndex;
				}
			}
			else {
				selectedClinicNum=GetClinicNumFromPicker(listClinicsToShow);//All clinic combo boxes have a none option, so always add 1.
				if(pickerButton==butPickClinic1) {
					comboClinic1.SelectedClinicNum=selectedClinicNum;
				}
				else if(pickerButton==butPickClinic2) {
					comboClinic2.SelectedClinicNum=selectedClinicNum;
				}
				else if(pickerButton==butPickClinic3) {
					comboClinic3.SelectedClinicNum=selectedClinicNum;
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
			UI.Button pickerButton=(UI.Button)sender;
			if(pickerButton==butPickProv1) {
				comboProvider1.SelectedIndex=selectedIndex;
			}
			else if(pickerButton==butPickProv2) {
				comboProvider2.SelectedIndex=selectedIndex;
			}
			else if(pickerButton==butPickProv3) {
				comboProvider3.SelectedIndex=selectedIndex;
			}
			SynchAndFillListFees(_needsSynch);
			FillGrid();
		}

		///<summary>Launches the Provider picker and lets the user pick a specific provider.
		///Returns the index of the selected provider within the Provider Cache (short).  Returns -1 if the user cancels out of the window.</summary>
		private int GetProviderIndexFromPicker() {
			using FormProviderPick FormP=new FormProviderPick();
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return -1;
			}
			return Providers.GetIndex(FormP.SelectedProvNum);
		}

		///<summary>Launches the Clinics window and lets the user pick a specific clinic.
		///Returns the index of the selected clinic within _arrayClinics.  Returns -1 if the user cancels out of the window.</summary>
		private long GetClinicNumFromPicker(List<Clinic> listClinics) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			FormC.ListClinics=listClinics.ToList();
			FormC.ShowDialog();
			return FormC.SelectedClinicNum;
		}

		private int GetFeeSchedGroupIndexFromPicker(List<FeeSchedGroup> listFeeSchedGroup) {
			List<GridColumn> listColumnHeaders=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Description"),100){ IsWidthDynamic=true }
			};
			List<GridRow> listRowValues=new List<GridRow>();
			listFeeSchedGroup.ForEach(x => {
				GridRow row=new GridRow(x.Description);
				row.Tag=x;
				listRowValues.Add(row);
			});
			string formTitle=Lan.g(this,"Fee Schedule Group Picker");
			string gridTitle=Lan.g(this,"Fee Schedule Groups");
			using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
			if(form.ShowDialog()==DialogResult.OK) {
				return listFeeSchedGroup.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)form.ListSelectedTags[0]).FeeSchedGroupNum));
			}
			//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
			return 0;
		}

		///<summary>Launches the Fee Schedules window and lets the user pick a specific schedule.
		///Returns the index of the selected schedule within _listFeeScheds.  Returns -1 if the user cancels out of the window.</summary>
		private int GetFeeSchedIndexFromPicker() {
			//No need to check security because we are launching the form in selection mode.
			using FormFeeScheds FormFS=new FormFeeScheds(true);
			FormFS.ShowDialog();
			return _listFeeScheds.FindIndex(x => x.FeeSchedNum==FormFS.SelectedFeeSchedNum);//Returns index of the found element or -1.
		}

		#endregion

		private void comboSort_SelectionChangeCommitted(object sender,EventArgs e) {
			_procCodeSort=(ProcCodeListSort)comboSort.SelectedIndex;
			FillGrid();
		}

		private void checkGroups1_CheckedChanged(object sender,EventArgs e) {
			if(checkGroups1.Checked) {
				//Hide clinic combobox and wipe selection.
				comboClinic1.Visible=false;
				comboClinic1.SelectedClinicNum=0;
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
				comboClinic2.SelectedClinicNum=0;
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
				comboClinic3.SelectedClinicNum=0;
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
			ListSelectedProcCodes=gridMain.SelectedTags<ProcedureCode>().Select(x => x.Copy()).ToList();
			SelectedCodeNum=ListSelectedProcCodes.First().CodeNum;
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
			if(_setInvalidProcCodes) {
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
		}
	}
}
