using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI{
	/*
Jordan is the only one allowed to edit this file.
Height should always be 21.
One common use use of this control is as a filter for reports, etc.
There should be no problems with this use.  The user cannot "see" clinics that they shouldn't, which is great and very useful.
But we need to be very careful in the case of using this combobox as a picker for fields on an object.
Since clinics with no permission would be hidden, this could inadvertently cause changes in which clinics are linked.  
Also, if the "all" option really means "all", then the calling form must specifically test IsAllSelected. 
"All" should not be used when the intent is ClinicNum=0
If the calling form instead uses ListSelectedClinicNums when All is selected, that's very different, and will exclude clinics not showing due to no permission.
This control is currently used in about 49 places (including the WinForms version).  Many of these are at the top, in the filter section of reports. 
Others are simple fields on objects.
Here are some complex situations to watch out for:
FormEServiceSetup has 4 comboBoxes.  I understand clinicPickerEClipboard, but not the other 3, so I can't police those 3.
FormPatientAddAll has 5 comboBoxes.  They don't include All or Unassigned, and they are all totally harmless, without potential for bugs.
FormPharmacyEdit: Includes All and Unassigned. It's multiselect. Does not test "All".  Because it uses a synch mechanism, clinics that aren't showing aren't affected either way.
FormRxEdit: Does not include All.  Includes Unassigned.  Single select. On save, if no selection, then it doesn't change the existing clinicNum.  Users can get into this window with "unrestricted search" permission, so ForceShowUnassigned true.
FormPatientEdit: Users can access with "unrestricted search" permission, so ForceShowUnassigned true.
FormProcCodes2: This form makes sure that the clinic pickers stay visible, even when clinics are disabled and they'd normally no longer be visible. 
Here's a list of 8 forms with most of the remaining comboboxes that still need to be switched out eventually: 
FormDoseSpotAssignClinicId(pickerbox & picker), FormEhrPatientExport, FormEServicesECR, FormMedLabs, 
FormPayConnectSetup, FormPaySimpleSetup, FormOnlinePayments, FormXWebTransactions

Common Pattern#1, Clinic as a field:---------------------------------------------------------------------------------
In the UI, set these properties:
	IncludeUnassigned=true
	HqDescription="None" //depending on situation
comboClinic.SelectedClinicNum=adj.ClinicNum;
...
adj.ClinicNum=comboClinic.SelectedClinicNum;

Common Pattern#2, Clinic as report filter----------------------------------------------------------------------------
In the UI, set these properties:
	IncludeAll=true
	IncludeUnassigned=true
	IsMultiSelect=true
string stringDisplayClinics=comboClinics.GetStringSelectedClinics();
List<long> listClinicNums=comboClinics.ListSelectedClinicNums;
RunReport(listClinicNums,stringDisplayClinics);

	*/

	///<summary></summary>
	public partial class ComboClinic : UserControl{
		#region Fields - Private Constant 
		///<summary>HQ/unassigned/default/none clinic with ClinicNum=0. Sometimes this dummy is filled with info from pref table instead of clinic table.</summary>
		private const long CLINIC_NUM_UNASSIGNED=0;
		#endregion Fields - Private Constant 

		#region Fields - Private
		//<summary>If the SelectedClinicNum gets set to a clinic that's not in the list because the user has no permission, then this is where that info is stored.  If this is null, then it instead behaves normally.</summary>
		//This was moved to get stored in comboBox, just like any other missing key
		//private Clinic _clinicSelectedNoPermission;
		///<summary>As this combo is initialized, the user defaults to CurUser. Can be changed.</summary>
		private Userod _userod=Security.CurUser;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _forceShowUnassigned=false;
		private string _hqDescription="Unassigned";
		private bool _includeHiddenInAll=false;
		private bool _includeUnassigned=false;
		private bool _showLabel=true;
		#endregion Fields - Private for Properties

		#region Constructor
		public ComboClinic(){
			InitializeComponent();
			Name="comboClinic";//default that can be changed. Doesn't seem to work.
			Loaded+=ComboClinic_Loaded;
			PreviewMouseLeftButtonDown+=ComboClinic_PreviewMouseLeftButtonDown;
			comboBox.SelectedIndexChanged+=(sender,e)=>SelectedIndexChanged?.Invoke(sender,e);
			comboBox.SelectionChangeCommitted+=(sender,e)=>SelectionChangeCommitted?.Invoke(this,e);
		}
		#endregion Constructor

		#region Events
		///<summary>Occurs when user selects a Clinic from the drop-down list.  This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change.</summary>
		[Category("OD")]
		[Description("Occurs when user selects a Clinic from the drop-down list.  This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change.")]
		public event EventHandler SelectionChangeCommitted;

		///<summary>Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes.</summary>
		[Category("OD")]
		[Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events

		#region Properties - Public Browsable
		///<summary>This will be set to true if we always need to show Unassigned/0, regardless of user permissions.</summary>
		[Category("OD")]
		[Description("This will be set to true if we always need to show Unassigned/0, regardless of user permissions.")]
		[DefaultValue(false)]
		public bool ForceShowUnassigned {
			get {
				return _forceShowUnassigned;
			}
			set {
				_forceShowUnassigned=value;
				FillClinics();
			}
		}

		///<summary>The display value for ClinicNum 0. Default is 'Unassigned', but might want 'Default', 'HQ', 'None', 'Practice', etc.  Do not specify 'All' here, because that is not accurate.  Only used when 'DoIncludeUnassigned'</summary>
		[Category("OD")]
		[Description("The display value for ClinicNum 0. Default is 'Unassigned', but might want 'Default', 'HQ', 'None', 'Practice', etc.  Do not specify 'All' here, because that is not accurate.  Only used when 'DoIncludeUnassigned'")]
		[DefaultValue("Unassigned")]
		public string HqDescription {
			get {
				return _hqDescription;
			}
			set {
				_hqDescription=value;
				List<Clinic> listClinicsSelected=comboBox.GetListSelected<Clinic>();
				FillClinics();//this wipes out the selected clinic(s), which can rarely be a problem.
				for(int i=0;i<comboBox.Items.Count;i++) {
					long clinicNum=((Clinic)comboBox.Items.GetObjectAt(i)).ClinicNum;
					if(listClinicsSelected.Exists(x=>x.ClinicNum==clinicNum)){
						comboBox.SetSelected(i);
					}
				}
			}
		}

		///<summary>Set to true to include 'All' as a selection option. 'All' can sometimes (e.g. FormOperatories) be intended to included more clinics than are actually showing in list.</summary>
		[Category("OD")]
		[Description("Set to true to include 'All' as a selection option. 'All' can sometimes (e.g. FormOperatories) be intended to included more clinics than are actually showing in list.")]
		[DefaultValue(false)]
		//This is browsable, unlike ComboBoxPlus.  It's because this comboBox is intentionally slightly more automated. 
		//You never have to load the clinics manually, so there's no logcical place in the code where you might also want to specify all.
		public bool IncludeAll {
			get {
				return comboBox.IncludeAll;
			}
			set {
				comboBox.IncludeAll=value;
				FillClinics();
			}
		}

		///<summary>Set to true to include hidden clinics in ListSelectedClinicNums when 'All' is selected.  This also causes (includes hidden) to show next to All.  Used for reports where we don't want to miss any entries.  List will always include 'unassigned/0' clinic.</summary>
		[Category("OD")]
		[Description("Set to true to include hidden clinics in ListSelectedClinicNums when 'All' is selected.  This also causes (includes hidden) to show next to All.  Used for reports where we don't want to miss any entries.  List will always include 'unassigned/0' clinic.")]
		[DefaultValue(false)]
		public bool IncludeHiddenInAll{ 
			get{
				return _includeHiddenInAll;
			}
			set{
				if(_includeHiddenInAll==value){
					return;
				}
				_includeHiddenInAll=value;
				if(_includeHiddenInAll){
					comboBox.SetTextForAllOption("All (includes hidden)");
				}
				else{
					comboBox.SetTextForAllOption("All");
				}
			}
		}

		///<summary>Set to true to include 'Unassigned' as a selection option. The word 'Unassigned' can be changed with the HqDescription property.  This is ClinicNum=0.</summary>
		[Category("OD")]
		[Description("Set to true to include 'Unassigned' as a selection option. The word 'Unassigned' can be changed with the HqDescription property.  This is ClinicNum=0.")]
		[DefaultValue(false)]//yes, true might be slightly better default, but then this could not be a drop-in replacement for ComboBoxClinicPicker without causing bugs -- many bugs.
		public bool IncludeUnassigned {
			get {
				return _includeUnassigned;
			}
			set {
				_includeUnassigned=value;
				FillClinics();
			}
		}

		[Category("OD")]
		[DefaultValue(true)]
		public new bool IsEnabled{
			//This doesn't actually ever get hit. 
			//It's just here to move IsEnabled down into the OD category.
			get{
				return base.IsEnabled;
			}
			set{
				base.IsEnabled=value;
			}
		}

		///<summary>Set true for multi-select, false for single-select.</summary>
		[Category("OD")]
		[Description("Set true for multi-select, false for single-select.")]
		[DefaultValue(false)]
		public bool IsMultiSelect{
			get{
				return comboBox.IsMultiSelect;
			}
			set{
				if(comboBox.IsMultiSelect==value){
					return;
				}
				comboBox.IsMultiSelect=value;
				if(comboBox.IsMultiSelect){
					textBlock.Text="Clinics";
				}
				else{
					textBlock.Text="Clinic";
				}
			}
		}

		///<summary>Normally true to show label on left.  Set to false if you want to do your own separate label.  Make sure to manually set visibility of that label, based on whether Clinic feature is turned on.</summary>
		[Category("OD")]
		[Description("Normally true to show label on left.  Set to false if you want to do your own separate label.  Make sure to manually set visibility of that label, based on whether Clinic feature is turned on.")]
		[DefaultValue(true)]
		public bool ShowLabel {
			get {
				return _showLabel;
			}
			set {
				if(_showLabel==value){
					return;
				}
				_showLabel=value;
				grid.ColumnDefinitions.Clear();
				ColumnDefinition columnDefinition;
				columnDefinition=new ColumnDefinition();
				if(_showLabel) {
					textBlock.Visibility=Visibility.Visible;
					columnDefinition.Width=new GridLength(37);//label
				}
				else {
					textBlock.Visibility=Visibility.Collapsed;
					columnDefinition.Width=new GridLength(0);
				}
				grid.ColumnDefinitions.Add(columnDefinition);
				columnDefinition=new ColumnDefinition();
				columnDefinition.Width=new GridLength(1,GridUnitType.Star);//comboBox
				grid.ColumnDefinitions.Add(columnDefinition);
			}
		}
		#endregion Properties - Public Browsable

		#region Properties - Public not Browsable
		///<summary>Getter returns -1 if no clinic is selected. The setter is special.  If you set it to a clinicNum that this user does not have permission for, then the combobox displays that and becomes read-only to prevent changing it. It will also remember such a clinicNum and return it back in subsequent get. That is intended for single select and can fail in certain multiselect scenarios like when All is selected.  0 (Unassigned) can also be set from here if it's present. This is common if pulling zero from the database.  But if you need to manually set to 0 in other situations, you should use the property IsUnassignedSelected.  You are not allowed to manually set to -1 (none) from here.  Instead, set IsNothingSelected=true.  On initial load, this control will automatically select the current clinic, which you can of course change by setting a different clinic.  For example, you must manually IsAllSelected=true.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long ClinicNumSelected {
			get {
				bool isDesignMode=DesignerProperties.GetIsInDesignMode(this);
				if(!isDesignMode && !IsTestModeNoDb) {
					if(!PrefC.HasClinicsEnabled) {
						return 0; //when clinics are turned off, this control doesn't appear and the 0 clinic is implicitly always selected
					}
				}
				//if(_clinicSelectedNoPermission!=null){
				//	return _clinicSelectedNoPermission.ClinicNum;
				//}
				if(IsMultiSelect){
					//comboBox.SelectedIndex,get and GetSelected, and GetSelectedKey all throw exception for multi
					if(comboBox.SelectedIndices.Count==0) {
						return -1;
					}
					return comboBox.GetListSelected<Clinic>()[0].ClinicNum;
				}
				//single select
				if(comboBox.SelectedIndex==-1) {
					long keyWhenMissing=comboBox.GetSelectedKey<Clinic>(x=>x.ClinicNum);
					if(keyWhenMissing==0){//nuance of whether this might be HQ is ignored.
						return -1;//typical
					}
					return keyWhenMissing;//user didn't have permission for this clinic, but it was assigned here earlier.
				}
				return comboBox.GetSelected<Clinic>().ClinicNum;//yes, this can return 0 for unassigned.
			}
			set {
				if(value==-1){
					throw new ApplicationException("Clinic num cannot be set to -1.  Instead, set IsNothingSelected.");
				}
				List<Clinic> listClinics=comboBox.Items.GetAll<Clinic>();
				int idx=listClinics.FindIndex(x=>x.ClinicNum==value);
				if(idx>-1){//the clinic they are trying to set is present in the list
					//_clinicSelectedNoPermission=null;//the next line does it
					comboBox.SelectedIndex=-1;
					comboBox.SetSelected(idx);
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					return;
				}
				//this user does not have permission for the selected clinic, so it's not in the list
				Func<long,string> funcOverrideText=x=>{
					if(IsTestModeNoDb){
						return value.ToString();
					}
					return Clinics.GetAbbr(x);//returns empty string if not found. In that case, comboBox will use the ClinicNum as the text.
				};
				comboBox.SetSelectedKey<Clinic>(value,x=>x.ClinicNum,funcOverrideText);
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
		}

		///<summary>True if the special dummy 'All' option is selected (regardless of any other additional selections). All needs to have been added, first.  The intent of All can vary, and the processing logic would be in the calling form.  On start, setting All would be done manually, not automatically.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAllSelected{
			get{
				return comboBox.IsAllSelected;
			}
			set{
				comboBox.IsAllSelected=true;
			}
		}

				///<summary>True if SelectedIndex==-1.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsNothingSelected {
			get{
				if(IsMultiSelect){
					//comboBox.SelectedIndex, get throws exception for multi
					return comboBox.SelectedIndices.Count==0;
				}
				else{
					return comboBox.SelectedIndex==-1;//this could be true if _selectedClinicNoPermission was set to some value that user did not have permission for
				}
			}
			set{
				//not going to check permission here because it's coming from code rather than user.
				//_clinicSelectedNoPermission=null;//handled in line below
				comboBox.SelectedIndex=-1;
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
		}

		///<summary>True if we are testing and db connection makes no sense.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public bool IsTestModeNoDb { get; set; } = false;

		///<summary>True if the 'unassigned'/default/hq/none/all clinic with ClinicNum=0 is selected.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUnassignedSelected {
			get{
				return ClinicNumSelected==CLINIC_NUM_UNASSIGNED;
			}
			set{
				ClinicNumSelected=CLINIC_NUM_UNASSIGNED;
			}
		}

		///<summary>Also can used when IsMultiSelect=false.  In the case where "All" is selected (regardless of any other additional selection), this will return a list of all clinicNums in the list.  This is not technically the same as All clinics in the database, because some clinics might be hidden from this user.  If unassigned(=0) is in the list when All is selected, then it will be included here.  If the calling form wishes to instead test All, and use other logic, it should test IsAllSelected. When setting, this isn't as rigorous as setting SelectedClinicNum.  This property will simply skip setting any clinics that aren't present because of no permission, but it will not disable the control to prevent changes. If !PrefC.HasClinicsEnabled, then this will return an empty list.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<long> ListClinicNumsSelected {
			get{
				bool isDesignMode=DesignerProperties.GetIsInDesignMode(this);
				if(!isDesignMode && !IsTestModeNoDb) {
					if(!PrefC.HasClinicsEnabled) {
						return new List<long>(); //when clinics are turned off this control doesn't appear and selected clinics is always empty list
					}
				}
				List<long> listClinicNumsSelected=new List<long>();
				//_clinicSelectedNoPermission should be meaningless for multiselect, but we'll give it to them anyway because we're not throwing exception
				long keyWhenMissing=comboBox.GetSelectedKey<Clinic>(x=>x.ClinicNum);
				if(keyWhenMissing>0){//nuance, we ignore possibility of HQ clinic
					listClinicNumsSelected.Add(keyWhenMissing);
					return listClinicNumsSelected;
				}
				if(comboBox.SelectedIndices.Count==0) {
					return listClinicNumsSelected;
				}
				if(IncludeAll && IsAllSelected){
					if(IncludeHiddenInAll){
						List<Clinic> listClinicsAll=Clinics.GetAllForUserod(_userod);
						List<long> listClinicNums=listClinicsAll.Select(x=>x.ClinicNum).ToList();
						listClinicNums.Add(0);
						return listClinicNums;
					}
					//does not include All
					//does include the "unassigned/default/hq/none/all" clinicNum=0, if present
					return comboBox.Items.GetAll<Clinic>().Select(x=>x.ClinicNum).ToList();
				}
				return comboBox.GetListSelected<Clinic>().Select(x=>x.ClinicNum).ToList();
			}
			set{
				//_clinicSelectedNoPermission=null;//handled on next line
				comboBox.SelectedIndex=-1;
				List<Clinic> listClinics=comboBox.Items.GetAll<Clinic>();
				for(int i=0;i<listClinics.Count;i++) {
					if(value.Contains(listClinics[i].ClinicNum)) {
						comboBox.SetSelected(i);
					}
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
		}

				///<summary>Sometimes, you need a list of clinics to pass to a clinic picker window. This returns a copy, not a reference to the internal list.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<Clinic> ListClinics {
			get{
				List<Clinic> listClinics=new List<Clinic>(comboBox.Items.GetAll<Clinic>());//copy
				return listClinics;
			}
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties - Public not Browsable

		#region Methods - Public
		///<summary>Returns empty string if no clinic is selected.  Can return "All" or the "Unassigned" override abbreviation.</summary>
		public string GetSelectedAbbr() {
			if(IsMultiSelect){//not the intended use, but we will support it
				//GetSelected would throw an exception
				List<Clinic> listClinics=comboBox.GetListSelected<Clinic>();
				if(listClinics.Count==0){
					return "";
				}
				return listClinics[0].Abbr;
			}
			Clinic clinic=comboBox.GetSelected<Clinic>();
			if(clinic==null){
				return "";
			}
			return clinic.Abbr;
		}

		///<summary>Returns null if no clinic selected.</summary>
		public Clinic GetSelectedClinic() {
			if(IsMultiSelect){//not the intended use, but we will support it
				//GetSelected would throw an exception
				List<Clinic> listClinics=comboBox.GetListSelected<Clinic>();
				if(listClinics.Count==0){
					return null;
				}
				return listClinics[0];
			}
			return comboBox.GetSelected<Clinic>();
		}

		///<summary>Gets a string of all selected clinic Abbr's, separated by commas.  If "All" is selected, then it simply returns "All Clinics", which might not be techinically true.  If ListSelectedClinicNums was used instead of testing IsAllSelected, then it could only include clinics that user has permission for.</summary>
		public string GetStringSelectedClinics() {
			if(comboBox.IsAllSelected){
				return "All Clinics";
			}
			List<Clinic> listClinicsSelected=comboBox.GetListSelected<Clinic>();
			return string.Join(",",listClinicsSelected.Select(clinic => clinic.Abbr));
		}

		///<summary>Just used in one spot: FormScheduleDayEdit for convenience.</summary>
		public string GetText(){
			return comboBox.GetText();
		}

		///<summary>Lets you change which user is used to load the allowed clinics.</summary>
		public void SetUser(Userod userod){
			_userod=userod;
			FillClinics();
		}

		///<summary>If the calling code set this combo to a clinic that the user does not have permission to, then the user will already be blocked from changing the clinic by clicking on this combo.  But if you want them to also be blocked from doing other things, use this method to see if the combo is currently giving them permission to change clinics, and then take action accordingly.</summary>
		public bool UserHasPermission(){
			if(IsMultiSelect){
				return true;//setting a hidden clinic is meaningless in multiselect.
			}
			if(comboBox.SelectedIndex>-1){
				return true;//if a hidden clinic was set, then selectedIndex would still be -1;
			}
			long keyWhenMissing=comboBox.GetSelectedKey<Clinic>(x=>x.ClinicNum);
			if(keyWhenMissing==0){
				return true;//typical
			}
			return false;//user does not have permission
		}
		#endregion Methods - Public

		#region Methods - event handlers
		private void ComboClinic_Loaded(object sender,RoutedEventArgs e) {
			FillClinics();
			bool isDesignMode=DesignerProperties.GetIsInDesignMode(this);
			if(!isDesignMode && !IsTestModeNoDb) {
				 Visible=PrefC.HasClinicsEnabled;
			}		
		}

		private void ComboClinic_PreviewMouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			if(UserHasPermission()){
				return;
			}
			if(IsTestModeNoDb){
				//OpenDental.MsgBox.Show(this,"Not allowed");
				//e.Handled=true;
				return;
			}
			if(Security.IsAuthorized(EnumPermType.UnrestrictedSearch,true)) {
				//Clinic is hidden, but user is allowed to move this object (example patient) to different clinic
				//FormComboPicker will come up, and the displayed Abbr will disappear because form has no access to the hidden clinic.  That's ok.
				//If user then selects a non-hidden clinic, _selectedClinicNoPermission will get set to null.
				return;
			}
			OpenDental.MsgBox.Show(this,"Not allowed");
			e.Handled=true;
		}
		#endregion Methods - event handlers

		#region Methods - private
		///<summary>This runs on load and with certain property changes that would only change at initialization.  Performance hit should be very small. This also automatically selects a reasonable initial clinic, usually the current clinic. </summary>
		private void FillClinics() {
			if(!IsTestModeNoDb){
				if(!Db.HasDatabaseConnection() && !Security.IsUserLoggedIn) {
					return;
				}
			}
			if(!IsTestModeNoDb){
				if(!PrefC.HasClinicsEnabled) {//Clinics are turned off, but
					if(Visible && !IsEnabled) {//this combobox was set visible after the Load (FormProcCodes)
						if(ForceShowUnassigned) {
							Clinic clinic=new Clinic();
							clinic.Abbr=HqDescription;
							clinic.Description=HqDescription;
							clinic.ClinicNum=0;
							comboBox.Items.Add(HqDescription,clinic);
						}
					}
					return;
				}
			}
			comboBox.Items.Clear();
			//IncludeAll comes first but is already handled
			//Does not  guarantee that HQ clinic will be included. Only if user has permission to view it.
			List<Clinic> listClinicsForUser=null;
			if(IsTestModeNoDb){
				listClinicsForUser=new List<Clinic>();
				for(int i=1;i<40;i++){
					Clinic clinic=new Clinic();
					clinic.Abbr="Clinic"+i.ToString();
					clinic.Description="Clinic"+i.ToString();
					clinic.ClinicNum=i;
					if(i==5 || i==7){
						clinic.IsHidden=true;
					}
					listClinicsForUser.Add(clinic);
				}
			}
			else{
				listClinicsForUser=Clinics.GetForUserod(_userod,true,HqDescription);
			}
			//Add HQ clinic when necessary.
			Clinic clinicUnassigned=listClinicsForUser.Find(x => x.ClinicNum==CLINIC_NUM_UNASSIGNED);
			//unassigned is next
			if(_forceShowUnassigned){
				Clinic clinic=new Clinic();
				clinic.Abbr=HqDescription;
				clinic.Description=HqDescription;
				clinic.ClinicNum=0;
				comboBox.Items.Add(HqDescription,clinic);
			}
			else if(IncludeUnassigned  && clinicUnassigned!=null) {
				comboBox.Items.Add(clinicUnassigned.Description,clinicUnassigned);
			}
			//then, the other items, except unassigned
			listClinicsForUser.RemoveAll(x => x.ClinicNum==CLINIC_NUM_UNASSIGNED);
			Func<Clinic,string> funcItemToString=x=>{
				if(x.IsHidden){
					return x.Abbr+OpenDental.Lang.g(this," (hidden)");
				}
				return x.Abbr;
			};
			comboBox.Items.AddList(listClinicsForUser,funcItemToString);
			//Will already be ordered alphabetically if that pref was set.  Unfortunately, a restart is required for that pref.
			//Setting selected---------------------------------------------------------------------------------------------------------------
			if(Clinics.ClinicNum==0) {
				if(IncludeUnassigned) {
					ClinicNumSelected=CLINIC_NUM_UNASSIGNED;
				}
				else if(IncludeAll) {
					comboBox.IsAllSelected=true;
				}
			}
			else {
				//if Security.CurUser.ClinicIsRestricted, there will be only one clinic in the list, and it will not include default (0).
				ClinicNumSelected=Clinics.ClinicNum;
			}
		}
		#endregion Methods - private
	}
}

/*todo: 
Add default sizes for all controls and explanation of how that works in FrmODBase
*/

